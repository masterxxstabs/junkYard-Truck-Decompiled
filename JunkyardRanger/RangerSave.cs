using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using EngineCloner;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JunkyardRanger
{
	// Rangers are saved with the game's save slots, like our other mods:
	// UserData/JunkyardRanger/slot1.txt..slot3.txt and auto.txt, one line each:
	//
	//   RANGER;1;id;px;py;pz;qx;qy;qz;qw;paint;gameEngine;snapshot
	//   ACCESSORY;1;kind;px;py;pz;qx;qy;qz;qw;health   (a loose bull bar, rain guard
	//                                                    or sun visor; fitted ones are
	//                                                    in their Ranger's snapshot)
	//   BULLBAR;1;px;py;pz;qx;qy;qz;qw;health           (1.4.0's loose bull bar)
	//
	// The snapshot is every part's state (Engine Cloner's VehicleApi.Snapshot). New
	// engines bolted in are saved by Engine Cloner (tagged with the Ranger's id);
	// if the game's own engine is in a Ranger, its type is noted here and it's
	// bolted back in on load. Engine Cloner calls these at the right moments:
	// saving with the game, loading before it restores its engines.
	internal static class RangerSave
	{
		private const int AutoSlot = 4;
		private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

		private static string File(int slot)
		{
			return Path.Combine(RangerMod.Folder, slot == AutoSlot ? "auto.txt" : "slot" + slot + ".txt");
		}

		private static string SlotName(int slot)
		{
			return slot == AutoSlot ? "the autosave" : "save slot " + slot;
		}

		public static void Save(int slot)
		{
			StringBuilder sb = new StringBuilder();
			int n = 0;
			foreach (RangerBody body in Object.FindObjectsOfType<RangerBody>())
			{
				GameObject ranger = body.gameObject;
				Transform t = ranger.transform;
				GameObject engine = VehicleApi.MountedEngine(ranger);
				string gameEngine = engine != null && !VehicleApi.IsOurEngine(engine) ? VehicleApi.KindOf(engine) : null;
				sb.Append(string.Join(";", new[]
				{
					"RANGER", "1", body.id,
					F(t.position.x), F(t.position.y), F(t.position.z),
					F(t.rotation.x), F(t.rotation.y), F(t.rotation.z), F(t.rotation.w),
					body.SavePaint() + (body.mud != null ? "/" + body.mud.Save() : ""), gameEngine ?? "-",
					VehicleApi.Snapshot(ranger, RangerBuilder.Skip)
				})).Append('\n');
				n++;
			}
			int bars = 0;
			foreach (RangerAccessoryItem item in Object.FindObjectsOfType<RangerAccessoryItem>())
			{
				Transform t = item.transform;
				PickUp pick = item.GetComponent<PickUp>();
				sb.Append(string.Join(";", new[]
				{
					"ACCESSORY", "1", item.kind,
					F(t.position.x), F(t.position.y), F(t.position.z),
					F(t.rotation.x), F(t.rotation.y), F(t.rotation.z), F(t.rotation.w),
					F(pick != null ? pick.thisDurability : 100f)
				})).Append('\n');
				bars++;
			}
			string path = File(slot);
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			if (n + bars == 0)
			{
				if (System.IO.File.Exists(path))
				{
					System.IO.File.Delete(path);
				}
				return;
			}
			string temp = path + ".tmp";
			System.IO.File.WriteAllText(temp, sb.ToString());
			if (System.IO.File.Exists(path))
			{
				System.IO.File.Delete(path);
			}
			System.IO.File.Move(temp, path);
			RangerMod.Log("Saved " + n + " Ranger(s)" + (bars > 0 ? " and " + bars + " loose accessories" : "") + " with " + SlotName(slot) + ".");
		}

		public static void Load(int slot)
		{
			if (slot <= 0 || !System.IO.File.Exists(File(slot)))
			{
				return;
			}
			GameObject original = VehicleApi.Original("car");
			if (original == null)
			{
				RangerMod.Log("No Diamondback in this level; the saved Rangers can't be rebuilt here.");
				return;
			}
			int n = 0;
			foreach (string line in System.IO.File.ReadAllLines(File(slot)))
			{
				if (line.StartsWith("BULLBAR;", StringComparison.Ordinal) || line.StartsWith("ACCESSORY;", StringComparison.Ordinal))
				{
					LoadAccessory(line);
					continue;
				}
				string[] f = line.Split(new[] { ';' }, 13);
				if (f.Length < 13 || f[0] != "RANGER")
				{
					continue;
				}
				try
				{
					RangerState state = new RangerState();
					state.id = f[2];
					state.position = new Vector3(P(f[3]), P(f[4]), P(f[5]));
					state.rotation = new Quaternion(P(f[6]), P(f[7]), P(f[8]), P(f[9]));
					state.paint = f[10];
					state.gameEngine = f[11] == "-" ? null : f[11];
					state.snapshot = f[12];
					if (RangerBuilder.Build(original, state.position + Vector3.up * 0.05f, state.rotation, state) != null)
					{
						n++;
					}
				}
				catch (Exception e)
				{
					RangerMod.Log("Skipping a bad Ranger save line: " + e.Message);
				}
			}
			RangerMod.Log("Restored " + n + " Ranger(s) from " + SlotName(slot) + ".");
		}

		private static void LoadAccessory(string line)
		{
			string[] f = line.Split(';');
			// BULLBAR lines have no kind.
			int o = f[0] == "BULLBAR" ? 2 : 3;
			string kind = f[0] == "BULLBAR" ? "BullBar" : f[2];
			if (f.Length < o + 8)
			{
				return;
			}
			try
			{
				RangerAccessories.Spawn(kind, new Vector3(P(f[o]), P(f[o + 1]) + 0.05f, P(f[o + 2])), new Quaternion(P(f[o + 3]), P(f[o + 4]), P(f[o + 5]), P(f[o + 6])), P(f[o + 7]));
			}
			catch (Exception e)
			{
				RangerMod.Log("Skipping a bad accessory save line: " + e.Message);
			}
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
				RangerMod.Log("Couldn't delete the Rangers saved with " + SlotName(slot) + ": " + e.Message);
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
	}
}
