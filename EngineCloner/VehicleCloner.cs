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

		public static void Clone(GameObject original)
		{
			Interactor interactor = Object.FindObjectOfType<Interactor>();
			if (interactor != null && interactor.drivingCar)
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

			GameObject clone = Object.Instantiate(original, position, rotation, original.transform.parent);
			// Lots of game code finds vehicles by name ("dirt pickup truck", "f1003",
			// "amc", "DirtBike"), so the clone must keep it.
			clone.name = original.name;
			clones.Add(clone.GetInstanceID());
			foreach (Rigidbody body in clone.GetComponentsInChildren<Rigidbody>())
			{
				body.velocity = Vector3.zero;
				body.angularVelocity = Vector3.zero;
			}

			// Instantiate re-points joints inside the copy at the copy, but a joint to
			// something outside it (the original truck the bike was strapped into, a
			// trailer) would weld the clone to that object. Engine blocks get their
			// own anchor; anything else is cut loose.
			int cut = 0;
			foreach (Joint joint in clone.GetComponentsInChildren<Joint>(true))
			{
				Rigidbody other = joint.connectedBody;
				if (other == null || other.transform.IsChildOf(clone.transform))
				{
					continue;
				}
				if (EngineClonerMod.IsEngineBlock(joint.gameObject))
				{
					EngineClonerMod.Release(joint.gameObject);
				}
				else
				{
					Object.Destroy(joint);
					cut++;
				}
			}
			EngineClonerMod.RegisterClonedBlocks(clone);
			vehicles.Add(clone);
			safePose[clone.GetInstanceID()] = new SavedPose(position, rotation);
			EngineClonerMod.Log("Cloned " + original.name + (cut > 0 ? " (cut " + cut + " joint(s) to outside objects)." : "."));
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
			}
		}
	}

	// Dropping an engine into an engine bay: PickUp finds the vehicle with
	// GameObject.Find("dirt pickup truck") etc., which can return the original
	// instead of the clone whose bay you're at. Wire the game to the vehicle that
	// owns the bay, and hand PickUp that vehicle so it can't look up the wrong one.
	[HarmonyPatch(typeof(PickUp), "LetGo")]
	internal static class MountEnginePatch
	{
		private static void Prefix(PickUp __instance, GameObject ___validTrigObject, bool ___canSnap, ref GameObject ___truck, ref GameObject ___truck2, ref GameObject ___amc, out GameObject __state)
		{
			__state = null;
			if (!___canSnap || ___validTrigObject == null || !EngineClonerMod.IsEngineBlock(__instance.gameObject))
			{
				return;
			}
			GameObject vehicle = VehicleCloner.FindVehicleRoot(___validTrigObject.transform);
			if (vehicle == null)
			{
				return;
			}
			VehicleCloner.Wire(vehicle);
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
			__state = vehicle;
		}

		// The dirt bike's bay is looked up with an uncached GameObject.Find("DirtBike").
		// If the engine landed in a different vehicle than the bay it was dropped
		// into, move it to the right one.
		private static void Postfix(PickUp __instance, GameObject __state)
		{
			if (__state == null)
			{
				return;
			}
			Transform parent = __instance.transform.parent;
			if (parent == null || parent.gameObject == __state || VehicleCloner.FindVehicleRoot(parent) == null)
			{
				return;
			}
			__instance.transform.parent = __state.transform;
			FixedJoint joint = __instance.GetComponent<FixedJoint>();
			if (joint != null)
			{
				joint.connectedBody = __state.GetComponent<Rigidbody>();
			}
		}
	}
}
