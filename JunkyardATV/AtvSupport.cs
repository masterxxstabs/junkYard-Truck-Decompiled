using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JunkyardATV
{
	// Saved with the game's save slots, like Truck Parts QOL: the game saves via
	// MainMenu.OptionSave/2/3/Auto -> ES3AutoSaveMgr.Save1/2/3/SaveAuto, and loads
	// the slot in PlayerPrefs "LoadSlot" (0 = new game). One line per ATV in
	// UserData/JunkyardATV/slotN.txt (auto.txt):  ATV;px;py;pz;rx;ry;rz;rw;engine
	internal static class AtvSave
	{
		private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

		public const int AutoSlot = 4;

		private static string File(int slot)
		{
			string dir = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.dataPath), "UserData"), "JunkyardATV");
			return Path.Combine(dir, slot == AutoSlot ? "auto.txt" : "slot" + slot + ".txt");
		}

		public static string SlotName(int slot)
		{
			return slot == AutoSlot ? "the autosave" : "save slot " + slot;
		}

		public static void Save(int slot)
		{
			try
			{
				StringBuilder sb = new StringBuilder();
				foreach (AtvVehicle atv in AtvVehicle.All)
				{
					Transform t = atv.transform;
					sb.Append(string.Join(";", new[]
					{
						"ATV", F(t.position.x), F(t.position.y), F(t.position.z), F(t.rotation.x), F(t.rotation.y), F(t.rotation.z), F(t.rotation.w), atv.SaveState()
					}));
					sb.Append("\n");
				}
				string path = File(slot);
				Directory.CreateDirectory(Path.GetDirectoryName(path));
				string temp = path + ".tmp";
				System.IO.File.WriteAllText(temp, sb.ToString());
				if (System.IO.File.Exists(path))
				{
					System.IO.File.Delete(path);
				}
				System.IO.File.Move(temp, path);
				AtvMod.Log("Saved " + AtvVehicle.All.Count + " ATV(s) with " + SlotName(slot) + ".");
			}
			catch (Exception e)
			{
				AtvMod.Log("Saving ATVs failed: " + e.Message);
			}
		}

		public static void Load(int slot)
		{
			if (slot <= 0 || !System.IO.File.Exists(File(slot)))
			{
				return;
			}
			int count = 0;
			foreach (string line in System.IO.File.ReadAllLines(File(slot)))
			{
				string[] f = line.Split(';');
				if (f.Length < 9 || f[0] != "ATV")
				{
					continue;
				}
				try
				{
					Vector3 p = new Vector3(P(f[1]), P(f[2]), P(f[3]));
					Quaternion r = new Quaternion(P(f[4]), P(f[5]), P(f[6]), P(f[7]));
					AtvVehicle.Create(p + Vector3.up * 0.15f, r, f[8]);
					count++;
				}
				catch (Exception e)
				{
					AtvMod.Log("Skipping a bad ATV save line: " + e.Message);
				}
			}
			AtvMod.Log("Restored " + count + " ATV(s) from " + SlotName(slot) + ".");
		}

		public static void Delete(int slot)
		{
			try
			{
				if (System.IO.File.Exists(File(slot)))
				{
					System.IO.File.Delete(File(slot));
				}
			}
			catch (Exception e)
			{
				AtvMod.Log("Couldn't delete ATV data for " + SlotName(slot) + ": " + e.Message);
			}
		}

		private static string F(float v)
		{
			return v.ToString("R", Inv);
		}

		private static float P(string s)
		{
			return float.Parse(s, NumberStyles.Float, Inv);
		}

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
				return AutoSlot;
			}
		}

		// ES3AutoSaveMgr is in the Easy Save plugin's assembly; found by name.
		public static void PatchEasySave(HarmonyLib.Harmony harmony)
		{
			Type manager = AccessTools.TypeByName("ES3AutoSaveMgr");
			if (manager == null)
			{
				return;
			}
			HarmonyMethod prefix = new HarmonyMethod(typeof(AtvSave).GetMethod("EasySavePrefix", BindingFlags.Static | BindingFlags.NonPublic));
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
			AtvMod.OnGameSave(SlotOf(__originalMethod.Name));
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
			AtvMod.OnGameSave(AtvSave.SlotOf(__originalMethod.Name));
		}
	}

	[HarmonyPatch(typeof(MainMenu), "DeleteSlot")]
	internal static class DeleteSlotPatch
	{
		private static void Prefix(int ___slotToDelete)
		{
			AtvSave.Delete(___slotToDelete);
		}
	}

	// New game over slot 1: it leaves PlayerPrefs "LoadSlot" as it was, so
	// remember that this one is a new game.
	[HarmonyPatch(typeof(MainMenu), "PlayGameOverwrite")]
	internal static class OverwritePatch
	{
		public static bool NewGame;

		private static void Prefix()
		{
			AtvSave.Delete(1);
			NewGame = true;
		}
	}

	// Fuel and oil. The game's fuel nozzle / oil bottle (PickUp + FluidHandler)
	// entering a trigger named 250FuelInlet / 250oilinput / 250transinput sets the
	// fluid type for the 250 engine, and FluidHandler then fills
	// GameObject.Find("250_block"): always the dirt bike's. When the trigger is an
	// ATV's, point that FluidHandler at the ATV's engine instead (and back to the
	// game's own lookup for any other trigger).
	[HarmonyPatch(typeof(PickUp), "OnTriggerEnter")]
	internal static class FuelPatch
	{
		private static readonly HashSet<string> Inlets = new HashSet<string> { "250FuelInlet", "250oilinput", "250transinput" };

		private static void Postfix(PickUp __instance, Collider other)
		{
			if (other == null || !Inlets.Contains(other.gameObject.name))
			{
				return;
			}
			FluidHandler fluid = __instance.GetComponent<FluidHandler>();
			if (fluid == null)
			{
				return;
			}
			AtvVehicle atv = other.GetComponentInParent<AtvVehicle>();
			fluid.engine250 = atv != null ? atv.Engine : null;
		}
	}

	// Sold in the Junkyard Terminal's Parts Store (ComputerPartStore mod) when it's
	// installed: a postfix on its GetCatalog adds an entry whose prefab is a
	// dormant delivery marker; ordering instantiates the marker next to the player,
	// and the marker builds a new ATV on open ground nearby.
	internal static class AtvStore
	{
		private static Type entryType;
		private static FieldInfo prefabField;
		private static FieldInfo nameField;
		private static FieldInfo priceField;
		private static FieldInfo categoryField;
		private static GameObject templateRoot;
		private static GameObject marker;

		public static bool Active { get; private set; }

		public static void TryInstall(HarmonyLib.Harmony harmony)
		{
			if (Active)
			{
				return;
			}
			Type store = null;
			foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
			{
				store = a.GetType("ComputerFeatures.PartsStoreService", false);
				if (store != null)
				{
					break;
				}
			}
			if (store == null)
			{
				return;
			}
			MethodInfo getCatalog = store.GetMethod("GetCatalog", BindingFlags.Static | BindingFlags.Public);
			entryType = store.GetNestedType("PartEntry", BindingFlags.Public | BindingFlags.NonPublic);
			if (getCatalog == null || entryType == null)
			{
				return;
			}
			prefabField = entryType.GetField("prefab");
			nameField = entryType.GetField("displayName");
			priceField = entryType.GetField("price");
			categoryField = entryType.GetField("category");
			if (prefabField == null || nameField == null || priceField == null || categoryField == null)
			{
				return;
			}
			harmony.Patch(getCatalog, null, new HarmonyMethod(typeof(AtvStore).GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic)));
			Active = true;
			AtvMod.Log("The ATV is now sold in the Junkyard Terminal's Parts Store.");
		}

		private static GameObject Marker()
		{
			if (marker == null)
			{
				templateRoot = new GameObject("JunkyardATV_StoreTemplates");
				templateRoot.SetActive(false);
				Object.DontDestroyOnLoad(templateRoot);
				marker = new GameObject("ATV delivery");
				marker.transform.SetParent(templateRoot.transform, false);
				marker.AddComponent<AtvDelivery>();
			}
			return marker;
		}

		private static void Postfix(object __result)
		{
			IList list = __result as IList;
			if (list == null)
			{
				return;
			}
			try
			{
				GameObject m = Marker();
				foreach (object entry in list)
				{
					if (prefabField.GetValue(entry) as GameObject == m)
					{
						priceField.SetValue(entry, AtvMod.Price);
						return;
					}
				}
				object e = Activator.CreateInstance(entryType);
				prefabField.SetValue(e, m);
				nameField.SetValue(e, "Vehicle - ATV (250cc)");
				priceField.SetValue(e, AtvMod.Price);
				// Under "Dirt Bike Parts" (and All), next to the other 250 parts.
				categoryField.SetValue(e, "Dirt Bike Part");
				list.Add(e);
			}
			catch (Exception ex)
			{
				AtvMod.Log("Couldn't list the ATV in the store: " + ex.Message);
			}
		}
	}

	// Ordered from the store: turns into an ATV on open ground near the player.
	public class AtvDelivery : MonoBehaviour
	{
		private void Start()
		{
			AtvMod.SpawnNearPlayer();
			Destroy(gameObject);
		}
	}
}
