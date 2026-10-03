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

		public static void Reset()
		{
			wired.Clear();
			clones.Clear();
		}

		// After a level loads the game is wired to the vehicles it shipped with.
		public static void Scan()
		{
			foreach (Type type in VehicleTypes)
			{
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

		public static void Clone(GameObject original)
		{
			Interactor interactor = Object.FindObjectOfType<Interactor>();
			if (interactor != null && interactor.drivingCar)
			{
				EngineClonerMod.Log("Get out of the vehicle before cloning.");
				return;
			}
			// Drop it beside the player, far enough out that it doesn't land on them.
			Bounds bounds = new Bounds(original.transform.position, Vector3.zero);
			foreach (Renderer renderer in original.GetComponentsInChildren<Renderer>())
			{
				bounds.Encapsulate(renderer.bounds);
			}
			float reach = Mathf.Max(bounds.extents.x, bounds.extents.z) + 2f;
			Transform cam = Camera.main.transform;
			Vector3 forward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
			Vector3 position = cam.position + forward * reach;
			position.y = original.transform.position.y + 0.5f;
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
			EngineClonerMod.Log("Cloned " + original.name + (cut > 0 ? " (cut " + cut + " joint(s) to outside objects)." : "."));
		}
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
