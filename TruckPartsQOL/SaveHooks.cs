using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace TruckPartsQOL
{
	// Keep Truck Parts QOL's data in step with the game's save slots.
	//
	// The game saves with MainMenu.OptionSave / OptionSave2 / OptionSave3 /
	// OptionSaveAuto, which call ES3AutoSaveMgr.Current.Save1/2/3/SaveAuto (Easy
	// Save 3, files JY.es3, JY2.es3, JY3.es3, JYAuto.es3). Both levels are hooked,
	// because a save button in the scene could call either one. The save happens
	// in a prefix, while everything is still in the world.
	internal static class SaveHooks
	{
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
				return PartSave.AutoSlot;
			}
		}

		// ES3AutoSaveMgr lives in the Easy Save plugin's assembly, so it's found by
		// name rather than referenced.
		public static void PatchEasySave(HarmonyLib.Harmony harmony)
		{
			System.Type manager = AccessTools.TypeByName("ES3AutoSaveMgr");
			if (manager == null)
			{
				return;
			}
			HarmonyMethod prefix = new HarmonyMethod(typeof(SaveHooks).GetMethod("EasySavePrefix", BindingFlags.Static | BindingFlags.NonPublic));
			foreach (string name in new[] { "Save1", "Save2", "Save3", "SaveAuto" })
			{
				MethodInfo method = AccessTools.Method(manager, name);
				if (method != null)
				{
					harmony.Patch(method, prefix);
				}
			}
		}

		private static void EasySavePrefix(MethodBase __originalMethod)
		{
			TruckPartsQOLMod.OnGameSave(SlotOf(__originalMethod.Name));
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
			TruckPartsQOLMod.OnGameSave(SaveHooks.SlotOf(__originalMethod.Name));
		}
	}

	// Deleting a save slot in the menu deletes our data for it too.
	[HarmonyPatch(typeof(MainMenu), "DeleteSlot")]
	internal static class DeleteSlotPatch
	{
		private static void Prefix(int ___slotToDelete)
		{
			PartSave.Delete(___slotToDelete);
		}
	}

	// "New game" over slot 1 deletes JY.es3; drop our slot 1 data with it.
	[HarmonyPatch(typeof(MainMenu), "PlayGameOverwrite")]
	internal static class OverwritePatch
	{
		private static void Prefix()
		{
			PartSave.Delete(1);
		}
	}
}
