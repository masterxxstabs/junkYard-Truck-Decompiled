using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EngineCloner
{
	// Vehicles have the same problem engines had, one level up. Interactor is wired
	// to one truck (truck, carscript, seatMount, exitMount...), one F100 (truck2,
	// fcarscript, seatMountF), one AMC (ucar, acarscript), one golf cart and one dirt
	// bike, and so are GearBox, AudioControl, Officer, Winch, FluidHandler and the
	// rest. Getting in a cloned truck would seat you in the original. So, just
	// before you get in a vehicle, every reference to the vehicle of that type the
	// game is currently wired to is swapped over to the one you are getting into.
	internal static class VehicleCloner
	{
		private static readonly Type[] VehicleTypes = { typeof(car), typeof(car4), typeof(car3), typeof(golfcart), typeof(Dirtbike) };

		// Per vehicle type, the vehicle the game is currently wired to.
		private static readonly Dictionary<Type, GameObject> wired = new Dictionary<Type, GameObject>();

		private static readonly HashSet<int> clones = new HashSet<int>();

		private static readonly List<GameObject> vehicles = new List<GameObject>();

		// Last place each vehicle was seen at rest, to put it back if it falls out of
		// the world.
		private static readonly Dictionary<int, SavedPose> safePose = new Dictionary<int, SavedPose>();

		private static readonly Dictionary<int, float> lastRescue = new Dictionary<int, float>();

		public static void Reset()
		{
			saveDepth = 0;
			rewireAfterSave.Clear();
			copyHolder = null;
			wired.Clear();
			clones.Clear();
			vehicles.Clear();
			safePose.Clear();
			lastRescue.Clear();
		}

		public static bool IsWired(GameObject vehicle)
		{
			Scan();
			GameObject current;
			Type type = VehicleType(vehicle);
			return type == null || !wired.TryGetValue(type, out current) || current == null || current == vehicle;
		}

		public static void RecordSafePoses()
		{
			for (int i = vehicles.Count - 1; i >= 0; i--)
			{
				GameObject vehicle = vehicles[i];
				if (vehicle == null)
				{
					vehicles.RemoveAt(i);
					continue;
				}
				Rigidbody body = vehicle.GetComponent<Rigidbody>();
				if (vehicle.activeInHierarchy && body != null && body.velocity.sqrMagnitude < 0.25f)
				{
					safePose[vehicle.GetInstanceID()] = new SavedPose(vehicle.transform.position, vehicle.transform.rotation);
				}
			}
		}

		// A copy the game isn't wired to fell into the out-of-world trigger. The game
		// would warp the vehicle it IS wired to (the original) home, so put the one
		// that actually fell back where it last rested instead.
		public static void Rescue(GameObject vehicle)
		{
			int id = vehicle.GetInstanceID();
			float last;
			// Every collider on the vehicle fires the trigger; handle it once.
			if (lastRescue.TryGetValue(id, out last) && Time.time - last < 2f)
			{
				return;
			}
			lastRescue[id] = Time.time;
			SavedPose pose;
			if (!safePose.TryGetValue(id, out pose))
			{
				return;
			}
			vehicle.transform.position = pose.position + Vector3.up * 0.5f;
			vehicle.transform.rotation = pose.rotation;
			foreach (Rigidbody body in vehicle.GetComponentsInChildren<Rigidbody>())
			{
				body.velocity = Vector3.zero;
				body.angularVelocity = Vector3.zero;
			}
			EngineClonerMod.Log(vehicle.name + " copy fell out of the world; put it back where it last stopped.");
		}

		// After a level loads the game is wired to the vehicles it shipped with.
		public static void Scan()
		{
			foreach (Type type in VehicleTypes)
			{
				foreach (Object found in Object.FindObjectsOfType(type))
				{
					GameObject go = ((Component)found).gameObject;
					if (!vehicles.Contains(go))
					{
						vehicles.Add(go);
					}
				}
				GameObject current;
				if (wired.TryGetValue(type, out current) && current != null)
				{
					continue;
				}
				foreach (Object found in Object.FindObjectsOfType(type))
				{
					GameObject go = ((Component)found).gameObject;
					if (!clones.Contains(go.GetInstanceID()))
					{
						wired[type] = go;
						break;
					}
				}
			}
		}

		public static string DescribeWired()
		{
			List<string> parts = new List<string>();
			foreach (KeyValuePair<Type, GameObject> pair in wired)
			{
				parts.Add(pair.Key.Name + " → " + Diagnostics.Name(pair.Value));
			}
			return string.Join(", ", parts.ToArray());
		}

		public static Type VehicleType(GameObject go)
		{
			foreach (Type type in VehicleTypes)
			{
				if (go.GetComponent(type) != null)
				{
					return type;
				}
			}
			return null;
		}

		public static GameObject FindVehicleRoot(Transform t)
		{
			for (; t != null; t = t.parent)
			{
				if (VehicleType(t.gameObject) != null)
				{
					return t.gameObject;
				}
			}
			return null;
		}

		// Point the game at this vehicle (no-op if it already is).
		public static void Wire(GameObject vehicle)
		{
			Type type = VehicleType(vehicle);
			if (type == null)
			{
				return;
			}
			Scan();
			GameObject current;
			if (!wired.TryGetValue(type, out current) || current == null || current == vehicle)
			{
				wired[type] = vehicle;
				return;
			}
			Dictionary<Object, Object> map;
			int swapped = ReferenceSwapper.Swap(current, vehicle, out map);
			wired[type] = vehicle;
			// Engines mounted in the two vehicles were swapped along with them.
			EngineClonerMod.FollowVehicleSwap(map);
			EngineClonerMod.Log("Switched to " + vehicle.name + ": rewired " + swapped + " game references to it.");
		}

		// Find open, solid ground next to the player: in front, then right, left,
		// behind. A copy that starts even partly inside the terrain falls through it.
		private static bool FindSpawnPosition(GameObject original, out Vector3 position)
		{
			position = Vector3.zero;
			// The copy gets the same heading, so the original's bounds as it stands now
			// are a good fit (only off a little if the original is parked on a slope).
			Bounds bounds = new Bounds(original.transform.position, Vector3.zero);
			foreach (Collider collider in original.GetComponentsInChildren<Collider>())
			{
				if (collider.enabled && !collider.isTrigger)
				{
					bounds.Encapsulate(collider.bounds);
				}
			}
			Vector3 pivotToCenter = bounds.center - original.transform.position;
			float pivotAboveBottom = original.transform.position.y - bounds.min.y;
			float reach = Mathf.Max(bounds.extents.x, bounds.extents.z) + 2.5f;

			Transform cam = Camera.main.transform;
			Vector3 forward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
			Vector3 right = Vector3.Cross(Vector3.up, forward);
			foreach (Vector3 direction in new[] { forward, right, -right, -forward })
			{
				Vector3 spot = cam.position + direction * reach;
				float groundY;
				if (!GroundBelow(spot, cam.position.y + 3f, original, out groundY))
				{
					continue;
				}
				Vector3 candidate = new Vector3(spot.x, groundY + pivotAboveBottom + 0.3f, spot.z);
				// Check the space the body will occupy, lifted a little so gentle
				// slopes and road meshes under the wheels don't count as blocked.
				Vector3 center = candidate + pivotToCenter + Vector3.up * (bounds.extents.y * 0.25f);
				Vector3 half = new Vector3(bounds.extents.x * 0.95f, bounds.extents.y * 0.7f, bounds.extents.z * 0.95f);
				if (!IsBlocked(center, half, original))
				{
					position = candidate;
					return true;
				}
			}
			return false;
		}

		private static bool GroundBelow(Vector3 spot, float fromHeight, GameObject ignore, out float groundY)
		{
			groundY = 0f;
			Vector3 start = new Vector3(spot.x, fromHeight, spot.z);
			RaycastHit[] hits = Physics.RaycastAll(start, Vector3.down, 100f, ~0, QueryTriggerInteraction.Ignore);
			Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
			foreach (RaycastHit hit in hits)
			{
				if (hit.transform.IsChildOf(ignore.transform) || IsPlayer(hit.collider))
				{
					continue;
				}
				// Loose junk lying around is not ground.
				if (hit.rigidbody != null && !hit.rigidbody.isKinematic)
				{
					continue;
				}
				groundY = hit.point.y;
				return true;
			}
			return false;
		}

		private static bool IsBlocked(Vector3 center, Vector3 half, GameObject ignore)
		{
			foreach (Collider collider in Physics.OverlapBox(center, half, Quaternion.identity, ~0, QueryTriggerInteraction.Ignore))
			{
				if (collider is TerrainCollider || collider.transform.IsChildOf(ignore.transform) || IsPlayer(collider))
				{
					continue;
				}
				return true;
			}
			return false;
		}

		private static bool IsPlayer(Collider collider)
		{
			return collider.GetComponentInParent<CharacterController>() != null;
		}

		// The player is sitting in a vehicle: copying it would copy the player too.
		public static bool PlayerInVehicle()
		{
			Interactor interactor = Object.FindObjectOfType<Interactor>();
			return interactor != null && (interactor.drivingCar || FindVehicleRoot(interactor.transform) != null);
		}

		public static void Clone(GameObject original)
		{
			if (PlayerInVehicle())
			{
				EngineClonerMod.Log("Get out of the vehicle before cloning.");
				return;
			}
			Vector3 position;
			if (!FindSpawnPosition(original, out position))
			{
				EngineClonerMod.Log("Not enough room to put a copy of " + original.name + " here. Try somewhere more open.");
				return;
			}
			Quaternion rotation = Quaternion.Euler(0f, original.transform.eulerAngles.y, 0f);
			GameObject clone = CopyInactive(original);
			Activate(clone, original, position, rotation);
			EngineClonerMod.Log("Cloned " + original.name + ".");
		}

		// --- Copies for other mods (VehicleApi). ---

		private static GameObject copyHolder;

		// A copy of the vehicle under an inactive holder: nothing on it has run yet
		// (no Awake), so it can be changed before Activate. The game's own save
		// tracking (ES3AutoSave) is taken off: copies save on their own, never over
		// the original's entry.
		public static GameObject CopyInactive(GameObject original)
		{
			if (copyHolder == null)
			{
				copyHolder = new GameObject("EngineCloner_VehicleCopies");
				copyHolder.SetActive(false);
				UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(copyHolder, original.scene);
			}
			GameObject copy = Object.Instantiate(original, copyHolder.transform, false);
			// Lots of game code finds vehicles by name ("dirt pickup truck", "f1003",
			// "amc", "DirtBike"), so the copy must keep it.
			copy.name = original.name;
			copy.transform.localPosition = Vector3.zero;
			copy.transform.localRotation = Quaternion.identity;
			EngineShop.StripGameSave(copy);
			StripOtherMods(copy);
			return copy;
		}

		// Things other mods fitted to the original (a stereo, speakers, rock lights,
		// a bed cover...) belong to the original: a copy taking them along made a
		// second of each, which those mods then saved too.
		private static void StripOtherMods(GameObject copy)
		{
			Assembly ours = typeof(VehicleCloner).Assembly;
			List<GameObject> drop = new List<GameObject>();
			List<Component> strip = new List<Component>();
			foreach (MonoBehaviour b in copy.GetComponentsInChildren<MonoBehaviour>(true))
			{
				if (b == null || b.GetType().Assembly == ours || !IsMelon(b.GetType().Assembly))
				{
					continue;
				}
				// A mod's own item fitted to the vehicle (something you can pick up:
				// a speaker, a rock light) is left out; a mod's script on one of the
				// game's own objects (a painted rim's paint, on the vehicle itself)
				// is just taken off, keeping the object.
				if (b.gameObject != copy && b.GetComponent<PickUp>() != null)
				{
					if (!drop.Contains(b.gameObject))
					{
						drop.Add(b.gameObject);
					}
				}
				else
				{
					strip.Add(b);
				}
			}
			int n = 0;
			foreach (GameObject go in drop)
			{
				// A child of one already dropped is gone with it.
				if (go != null)
				{
					Object.DestroyImmediate(go);
					n++;
				}
			}
			foreach (Component c in strip)
			{
				if (c != null)
				{
					Object.DestroyImmediate(c);
				}
			}
			if (n > 0)
			{
				EngineClonerMod.Log("Copy of " + copy.name + ": left out " + n + " part(s) other mods fitted to the original.");
			}
		}

		private static readonly Dictionary<Assembly, bool> modAssemblies = new Dictionary<Assembly, bool>();

		private static bool IsMelon(Assembly a)
		{
			bool mod;
			if (modAssemblies.TryGetValue(a, out mod))
			{
				return mod;
			}
			// A MelonLoader mod or plugin carries a MelonInfo attribute.
			try
			{
				mod = a.GetCustomAttributes(typeof(MelonLoader.MelonInfoAttribute), false).Length > 0;
			}
			catch (Exception)
			{
				mod = false;
			}
			modAssemblies[a] = mod;
			return mod;
		}

		// Put a copy made by CopyInactive into the world at this pose and start it.
		public static void Activate(GameObject copy, GameObject original, Vector3 position, Quaternion rotation)
		{
			copy.transform.SetPositionAndRotation(position, rotation);
			// Instantiate re-points joints inside the copy at the copy, but a joint to
			// something outside it (the original truck the bike was strapped into, a
			// trailer) would weld the copy to that object. Engine blocks get their
			// own anchor; anything else is cut loose.
			int cut = 0;
			foreach (Joint joint in copy.GetComponentsInChildren<Joint>(true))
			{
				Rigidbody other = joint.connectedBody;
				if (other == null || other.transform.IsChildOf(copy.transform))
				{
					continue;
				}
				if (EngineClonerMod.IsEngineBlock(joint.gameObject))
				{
					EngineClonerMod.Release(joint.gameObject);
				}
				else
				{
					Object.DestroyImmediate(joint);
					cut++;
				}
			}
			copy.transform.SetParent(original.transform.parent, true);
			clones.Add(copy.GetInstanceID());
			foreach (Rigidbody body in copy.GetComponentsInChildren<Rigidbody>())
			{
				body.velocity = Vector3.zero;
				body.angularVelocity = Vector3.zero;
			}
			EngineClonerMod.RegisterClonedBlocks(copy);
			vehicles.Add(copy);
			safePose[copy.GetInstanceID()] = new SavedPose(position, rotation);
			if (cut > 0)
			{
				EngineClonerMod.Log("Copy of " + original.name + ": cut " + cut + " joint(s) to outside objects.");
			}
		}

		public static bool IsCopy(GameObject vehicle)
		{
			return vehicle != null && clones.Contains(vehicle.GetInstanceID());
		}

		// The game's own vehicle of this type (not one of our copies).
		public static GameObject Original(Type type)
		{
			foreach (Object o in Object.FindObjectsOfType(type))
			{
				GameObject go = ((Component)o).gameObject;
				if (!clones.Contains(go.GetInstanceID()))
				{
					return go;
				}
			}
			return null;
		}

		// Open, solid ground near a point, with room for a vehicle like this one.
		public static bool FindGroundNear(GameObject original, Vector3 near, out Vector3 position)
		{
			position = Vector3.zero;
			Bounds bounds = new Bounds(original.transform.position, Vector3.zero);
			foreach (Collider collider in original.GetComponentsInChildren<Collider>())
			{
				if (collider.enabled && !collider.isTrigger)
				{
					bounds.Encapsulate(collider.bounds);
				}
			}
			Vector3 pivotToCenter = bounds.center - original.transform.position;
			float pivotAboveBottom = original.transform.position.y - bounds.min.y;
			float step = Mathf.Max(bounds.extents.x, bounds.extents.z) + 1f;
			for (int ring = 0; ring <= 4; ring++)
			{
				int tries = ring == 0 ? 1 : 8;
				for (int i = 0; i < tries; i++)
				{
					float a = i * Mathf.PI * 2f / tries;
					Vector3 spot = near + new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * step * ring;
					float groundY;
					if (!GroundBelow(spot, near.y + 4f, original, out groundY))
					{
						continue;
					}
					Vector3 candidate = new Vector3(spot.x, groundY + pivotAboveBottom + 0.3f, spot.z);
					Vector3 center = candidate + pivotToCenter + Vector3.up * (bounds.extents.y * 0.25f);
					Vector3 half = new Vector3(bounds.extents.x * 0.95f, bounds.extents.y * 0.7f, bounds.extents.z * 0.95f);
					if (!IsBlocked(center, half, original))
					{
						position = candidate;
						return true;
					}
				}
			}
			return false;
		}

		// --- Saving. ---

		// The game saves references to its vehicles (Interactor.truck, the seat,
		// the gearbox...). While it saves, wire it to its own vehicles, so it never
		// stores a copy it can't find again; then back to the copies.
		private static int saveDepth;
		private static readonly List<GameObject> rewireAfterSave = new List<GameObject>();

		public static void BeginSave()
		{
			if (saveDepth++ > 0)
			{
				return;
			}
			rewireAfterSave.Clear();
			try
			{
				Scan();
				foreach (KeyValuePair<Type, GameObject> pair in new List<KeyValuePair<Type, GameObject>>(wired))
				{
					GameObject current = pair.Value;
					if (current == null || !clones.Contains(current.GetInstanceID()))
					{
						continue;
					}
					GameObject original = Original(pair.Key);
					if (original != null)
					{
						Wire(original);
						rewireAfterSave.Add(current);
					}
				}
			}
			catch (Exception e)
			{
				EngineClonerMod.Log("Couldn't wire the game to its own vehicles for saving: " + e.Message);
			}
		}

		public static void EndSave()
		{
			if (saveDepth == 0 || --saveDepth > 0)
			{
				return;
			}
			foreach (GameObject vehicle in rewireAfterSave)
			{
				if (vehicle != null)
				{
					try
					{
						Wire(vehicle);
					}
					catch (Exception e)
					{
						EngineClonerMod.Log("Couldn't wire the game back to " + vehicle.name + " after saving: " + e.Message);
					}
				}
			}
			rewireAfterSave.Clear();
		}
	}

	// LostFound is the out-of-world trigger. For a vehicle it calls PhoneScript,
	// which warps the vehicle the game is WIRED to home, not the one that fell (and
	// toggles it off and on, once per collider). Only let that happen for the wired
	// vehicle.
	[HarmonyPatch(typeof(LostFound), "OnTriggerEnter")]
	internal static class LostFoundPatch
	{
		private static bool Prefix(Collider other)
		{
			if (other == null)
			{
				return true;
			}
			GameObject vehicle = VehicleCloner.FindVehicleRoot(other.transform);
			if (vehicle == null || VehicleCloner.IsWired(vehicle))
			{
				return true;
			}
			VehicleCloner.Rescue(vehicle);
			return false;
		}
	}

	internal struct SavedPose
	{
		public SavedPose(Vector3 position, Quaternion rotation)
		{
			this.position = position;
			this.rotation = rotation;
		}

		public Vector3 position;

		public Quaternion rotation;
	}

	// Getting in: wire the game to the vehicle whose seat you actually clicked.
	[HarmonyPatch]
	internal static class GetInPatch
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			foreach (string name in new[] { "GetIn", "GetInF", "GetInCar", "GetInCart", "GetInDirtbike" })
			{
				yield return AccessTools.Method(typeof(Interactor), name);
			}
		}

		private static void Prefix(Interactor __instance, RaycastHit ___hit)
		{
			if (__instance.drivingCar || ___hit.collider == null)
			{
				return;
			}
			GameObject vehicle = VehicleCloner.FindVehicleRoot(___hit.collider.transform);
			if (vehicle != null)
			{
				VehicleCloner.Wire(vehicle);
				GameObject block = VehicleApi.MountedEngine(vehicle);
				if (block != null)
				{
					EngineClonerMod.WireTo(block);
				}
				Prepare(__instance, vehicle);
				Point(__instance, block);
			}
		}

		// The game refreshes and starts the engine of the truck's kind it holds
		// (Interactor.fourcyl / v8 / i6): the one bolted in this vehicle.
		private static void Point(Interactor interactor, GameObject block)
		{
			if (block == null)
			{
				return;
			}
			if (block.GetComponent<enginev8>() != null)
			{
				interactor.v8 = block;
			}
			else if (block.GetComponent<enginei6>() != null)
			{
				interactor.i6 = block;
			}
			else if (block.GetComponent<engine>() != null)
			{
				interactor.fourcyl = block;
			}
		}

		// Once in, the engine works out whether it can run, with its own truck's
		// battery, fuel and parts (it was skipped while the vehicle wasn't wired).
		private static void Postfix(Interactor __instance)
		{
			if (!__instance.drivingCar)
			{
				return;
			}
			try
			{
				Transform seat = __instance.person != null ? __instance.person.transform.parent : null;
				GameObject vehicle = seat != null ? VehicleCloner.FindVehicleRoot(seat) : null;
				GameObject block = vehicle != null ? VehicleApi.MountedEngine(vehicle) : null;
				if (block == null)
				{
					return;
				}
				foreach (MonoBehaviour script in block.GetComponents<MonoBehaviour>())
				{
					if (script is engine || script is enginev8 || script is enginei6)
					{
						MethodInfo refresh = script.GetType().GetMethod("Refresh", Type.EmptyTypes);
						if (refresh != null)
						{
							refresh.Invoke(script, null);
						}
					}
				}
			}
			catch (Exception e)
			{
				EngineClonerMod.Log("Refreshing the engine after getting in: " + e.Message);
			}
		}

		// What the game's GetIn needs from the truck it's wired to, which a copy may
		// lack: its suspension script as the first child (GetIn takes
		// truck.transform.GetChild(0)), and an engine object of the kind the truck
		// says it uses (a copy with no engine, or one bolted in that isn't wired).
		private static void Prepare(Interactor interactor, GameObject vehicle)
		{
			try
			{
				if (interactor.truck == vehicle && vehicle.transform.childCount > 0 && vehicle.transform.GetChild(0).GetComponent<SuspensionOffroadCar>() == null)
				{
					SuspensionOffroadCar s = vehicle.GetComponentInChildren<SuspensionOffroadCar>(true);
					if (s != null && s.transform.parent == vehicle.transform)
					{
						s.transform.SetSiblingIndex(0);
						EngineClonerMod.Log("Put " + vehicle.name + "'s suspension back as its first part (the game looks for it there).");
					}
				}
				if (interactor.fourcyl == null || interactor.fourcyl.GetComponent<engine>() == null)
				{
					engine e = Object.FindObjectOfType<engine>();
					if (e != null)
					{
						interactor.fourcyl = e.gameObject;
					}
				}
				if (interactor.v8 == null || interactor.v8.GetComponent<enginev8>() == null)
				{
					enginev8 e = Object.FindObjectOfType<enginev8>();
					if (e != null)
					{
						interactor.v8 = e.gameObject;
					}
				}
				if (interactor.i6 == null || interactor.i6.GetComponent<enginei6>() == null)
				{
					enginei6 e = Object.FindObjectOfType<enginei6>();
					if (e != null)
					{
						interactor.i6 = e.gameObject;
					}
				}
			}
			catch (Exception e)
			{
				EngineClonerMod.Log("Getting ready to get in: " + e.Message);
			}
		}

		// If the game's GetIn fails anyway, say why in the log (the game's own error
		// only goes to Unity's player log).
		private static Exception Finalizer(Exception __exception, MethodBase __originalMethod)
		{
			if (__exception != null)
			{
				EngineClonerMod.Log("Getting in (" + __originalMethod.Name + ") failed: " + __exception);
			}
			return __exception;
		}
	}

	// Dropping an engine into an engine bay: PickUp finds the vehicle with
	// GameObject.Find("dirt pickup truck") etc., which can return the original
	// instead of the clone whose bay you're at. Wire the game to the vehicle that
	// owns the bay, and hand PickUp that vehicle so it can't look up the wrong one.
	[HarmonyPatch(typeof(PickUp), "LetGo")]
	internal static class MountEnginePatch
	{
		private static void Prefix(PickUp __instance, ref GameObject ___validTrigObject, bool ___canSnap, ref GameObject ___truck, ref GameObject ___truck2, ref GameObject ___amc, out Transform[] __state)
		{
			__state = null;
			if (!___canSnap || ___validTrigObject == null || !EngineClonerMod.IsEngineBlock(__instance.gameObject))
			{
				return;
			}
			GameObject bay = ___validTrigObject;
			GameObject vehicle = VehicleCloner.FindVehicleRoot(bay.transform);
			if (vehicle == null)
			{
				return;
			}
			VehicleCloner.Wire(vehicle);
			// The bay it's held over stays the bay it's held over.
			___validTrigObject = bay;
			Type type = VehicleCloner.VehicleType(vehicle);
			if (type == typeof(car))
			{
				___truck = vehicle;
			}
			else if (type == typeof(car4))
			{
				___truck2 = vehicle;
			}
			else if (type == typeof(car3))
			{
				___amc = vehicle;
			}
			__state = new[] { vehicle.transform, bay.transform };
		}

		// The dirt bike's bay is looked up with an uncached GameObject.Find("DirtBike").
		// If the engine landed in a different vehicle than the bay it was dropped
		// into, move it to the right one.
		private static void Postfix(PickUp __instance, Transform[] __state)
		{
			if (__state == null || __state[0] == null)
			{
				return;
			}
			GameObject vehicle = __state[0].gameObject;
			Transform bay = __state[1];
			Transform parent = __instance.transform.parent;
			if (parent == null || VehicleCloner.FindVehicleRoot(parent) == null)
			{
				return;   // not bolted into a vehicle (an engine stand, say)
			}
			FixedJoint joint = __instance.GetComponent<FixedJoint>();
			bool wrongVehicle = parent.gameObject != vehicle;
			bool wrongPlace = bay != null && (__instance.transform.position - bay.position).sqrMagnitude > 0.0025f;
			if (wrongVehicle || wrongPlace)
			{
				// In the bay it was dropped into, fixed to that vehicle. The joint is
				// let go first: it holds the block where it was when it was connected.
				if (joint != null)
				{
					joint.connectedBody = null;
				}
				__instance.transform.parent = vehicle.transform;
				if (bay != null)
				{
					__instance.transform.SetPositionAndRotation(bay.position, bay.rotation);
				}
				if (joint != null)
				{
					joint.connectedBody = vehicle.GetComponent<Rigidbody>();
				}
				EngineClonerMod.Log("Put " + __instance.name + " in " + vehicle.name + "'s engine bay, where it was dropped" + (wrongVehicle ? " (the game had it in another vehicle)." : "."));
			}
			// The block now talks to the truck it's in, not the one it came from.
			EngineClonerMod.LocalizeMounted(__instance.gameObject);
		}
	}
}
