using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace EngineCloner
{
	// Bolting a part onto an engine: PickUp.LetGo snaps it into the slot and calls
	// addPart(slotName, true) on GameObject.Find("engineblock" / "v8_block" /
	// "i6block" / "250_block"), and addPart reads the part's health with
	// GameObject.Find(slotName). With more than one engine of a type (Engine
	// Cloner copies, Junkyard ATV's 250s) both lookups can land on the wrong
	// engine: the part's condition is recorded on another engine, and the one it
	// was bolted to never learns of it.
	//
	// This re-runs addPart on the engine that owns the slot the part went into,
	// with same-named slots on other engines briefly renamed so Find resolves to
	// the right one. Shared by Engine Cloner and Junkyard ATV: whichever loads
	// first installs it (AppDomain flag), so it never runs twice.
	internal static class AddPartFix
	{
		private const string OwnerKey = "JunkyardTruckMods.AddPartFix";

		private static GameObject slot;
		private static bool redirecting;

		public static void Install(HarmonyLib.Harmony harmony)
		{
			if (AppDomain.CurrentDomain.GetData(OwnerKey) != null)
			{
				return;
			}
			AppDomain.CurrentDomain.SetData(OwnerKey, "EngineCloner");
			MethodInfo letGo = AccessTools.Method(typeof(PickUp), "LetGo");
			harmony.Patch(letGo, new HarmonyMethod(typeof(AddPartFix).GetMethod("LetGoPrefix", BindingFlags.Static | BindingFlags.NonPublic)), new HarmonyMethod(typeof(AddPartFix).GetMethod("LetGoPostfix", BindingFlags.Static | BindingFlags.NonPublic)));
			HarmonyMethod prefix = new HarmonyMethod(typeof(AddPartFix).GetMethod("AddPartPrefix", BindingFlags.Static | BindingFlags.NonPublic));
			foreach (Type type in new[] { typeof(engine), typeof(enginev8), typeof(enginei6), typeof(Engine250) })
			{
				MethodInfo addPart = AccessTools.Method(type, "addPart", new[] { typeof(string), typeof(bool) });
				if (addPart != null)
				{
					harmony.Patch(addPart, prefix);
				}
			}
		}

		private static void LetGoPrefix(GameObject ___validTrigObject, bool ___canSnap)
		{
			slot = ___canSnap ? ___validTrigObject : null;
		}

		private static void LetGoPostfix()
		{
			slot = null;
		}

		private static bool AddPartPrefix(Component __instance, MethodBase __originalMethod, string newPart, bool connect)
		{
			if (redirecting || slot == null || !slot.activeInHierarchy || slot.name != newPart)
			{
				return true;
			}
			Component owner = slot.GetComponentInParent(__instance.GetType());
			if (owner == null)
			{
				return true;
			}
			// Make GameObject.Find(newPart) return this slot.
			List<GameObject> renamed = new List<GameObject>();
			redirecting = true;
			try
			{
				for (GameObject other = GameObject.Find(newPart); other != null && other != slot && renamed.Count < 32; other = GameObject.Find(newPart))
				{
					other.name = newPart + "~";
					renamed.Add(other);
				}
				__originalMethod.Invoke(owner, new object[] { newPart, connect });
			}
			catch (Exception e)
			{
				EngineClonerMod.Log("addPart on the right engine failed: " + (e.InnerException ?? e).Message);
			}
			finally
			{
				foreach (GameObject other in renamed)
				{
					if (other != null)
					{
						other.name = newPart;
					}
				}
				redirecting = false;
			}
			return false;
		}
	}
}
