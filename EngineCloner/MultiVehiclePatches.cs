using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace EngineCloner
{
	// An engine decides it can run and crank by its parent's name ("dirt pickup
	// truck"), so with two Diamondbacks the one you're in could start on the engine
	// in the other. An engine sitting in a vehicle the game isn't wired to (one
	// you're not using) can't run or crank; getting in that vehicle wires it, and
	// the next refresh lets it run again.
	[HarmonyPatch]
	internal static class UnwiredEnginePatch
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			foreach (System.Type type in new[] { typeof(engine), typeof(enginev8), typeof(enginei6) })
			{
				MethodInfo m = AccessTools.Method(type, "Refresh");
				if (m != null)
				{
					yield return m;
				}
			}
		}

		private static void Postfix(MonoBehaviour __instance)
		{
			Transform parent = __instance.transform.parent;
			GameObject vehicle = parent != null ? VehicleCloner.FindVehicleRoot(parent) : null;
			if (vehicle == null || VehicleCloner.IsWired(vehicle))
			{
				return;
			}
			EngineShop.SetField(__instance, "canRun", false);
			EngineShop.SetField(__instance, "canCrank", false);
		}
	}

	// Fuel and additive go into the vehicle the game is wired to (FluidHandler's
	// truck and fuel lid). Holding a can to a vehicle's filler wires the game to
	// that vehicle, so it fills the one you're at.
	[HarmonyPatch(typeof(PickUp), "OnTriggerEnter")]
	internal static class FuelInletPatch
	{
		private static void Postfix(PickUp __instance, Collider other)
		{
			if (other == null || !__instance.holding || (other.name != "gastankinput" && !other.name.StartsWith("FuelInlet", System.StringComparison.Ordinal)))
			{
				return;
			}
			if (__instance.GetComponent("FluidHandler") == null)
			{
				return;
			}
			GameObject vehicle = VehicleCloner.FindVehicleRoot(other.transform);
			if (vehicle != null && !VehicleCloner.IsWired(vehicle))
			{
				VehicleCloner.Wire(vehicle);
			}
		}
	}
}
