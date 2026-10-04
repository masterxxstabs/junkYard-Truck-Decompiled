using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace JunkAutoPC
{
	// Saved with the game's save slots, like our other mods: the game saves via
	// MainMenu.OptionSave/2/3/Auto -> ES3AutoSaveMgr.Save1/2/3/SaveAuto, and loads
	// the slot in PlayerPrefs "LoadSlot" (0 = new game).
	internal static class Patches
	{
		// New game over slot 1 leaves "LoadSlot" as it was.
		public static bool NewGame;

		private static int lastFrame = -1;
		private static int lastSlot;

		public static int SlotOf(string method)
		{
			switch (method)
			{
			case "OptionSave":
			case "Save1":
				return 1;
			case "OptionSave2":
			case "Save2":
				return 2;
			case "OptionSave3":
			case "Save3":
				return 3;
			default:
				return Orders.AutoSlot;
			}
		}

		public static void Saved(string method)
		{
			int slot = SlotOf(method);
			// MainMenu.OptionSave calls ES3AutoSaveMgr.Save1: save once.
			if (lastFrame == UnityEngine.Time.frameCount && lastSlot == slot)
			{
				return;
			}
			lastFrame = UnityEngine.Time.frameCount;
			lastSlot = slot;
			JunkAutoMod.OnGameSave(slot);
		}

		// ES3AutoSaveMgr is in the Easy Save plugin's assembly; found by name.
		public static void PatchEasySave(HarmonyLib.Harmony harmony)
		{
			System.Type manager = AccessTools.TypeByName("ES3AutoSaveMgr");
			if (manager == null)
			{
				return;
			}
			HarmonyMethod prefix = new HarmonyMethod(typeof(Patches).GetMethod("EasySavePrefix", BindingFlags.Static | BindingFlags.NonPublic));
			foreach (string name in new[] { "Save1", "Save2", "Save3", "SaveAuto" })
			{
				MethodInfo m = AccessTools.Method(manager, name);
				if (m != null)
				{
					harmony.Patch(m, prefix);
				}
			}
		}

		private static void EasySavePrefix(MethodBase __originalMethod)
		{
			Saved(__originalMethod.Name);
		}
	}

	[HarmonyPatch]
	internal static class MainMenuSavePatch
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			foreach (string name in new[] { "OptionSave", "OptionSave2", "OptionSave3", "OptionSaveAuto" })
			{
				yield return AccessTools.Method(typeof(MainMenu), name);
			}
		}

		private static void Prefix(MethodBase __originalMethod)
		{
			Patches.Saved(__originalMethod.Name);
		}
	}

	[HarmonyPatch(typeof(MainMenu), "DeleteSlot")]
	internal static class DeleteSlotPatch
	{
		private static void Prefix(int ___slotToDelete)
		{
			Orders.Delete(___slotToDelete);
		}
	}

	[HarmonyPatch(typeof(MainMenu), "PlayGameOverwrite")]
	internal static class OverwritePatch
	{
		private static void Prefix()
		{
			Orders.Delete(1);
			Patches.NewGame = true;
		}
	}
}
