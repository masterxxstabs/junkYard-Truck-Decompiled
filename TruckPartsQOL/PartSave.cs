using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace TruckPartsQOL
{
	// Saved alongside the game's own save slots: when the game saves slot N
	// (MainMenu.OptionSave/2/3/Auto, i.e. ES3 files JY/JY2/JY3/JYAuto.es3), this
	// writes UserData/TruckPartsQOL/slotN.txt (auto.txt for the autosave); when the
	// game loads slot N (PlayerPrefs "LoadSlot", 0 = new game) it restores that file.
	//
	// One line per part:
	// kind;cd;vehicle;mountPath;px;py;pz;rx;ry;rz;rw;power;volume;mode;track;insertedCd;coverOpen;coverHeight;bolts
	// (the cover and bolt fields were added later; older lines without them still
	// load. bolts: each bolt's turns, comma-separated, for fitted parts.)
	// Installed parts store their pose relative to what they're mounted on and are
	// re-installed on the first vehicle with that name; loose parts store world pose.
	// Plus RIM;... lines for painted rims and SCANNER;1 if the OBD scanner is owned.
	internal static class PartSave
	{
		private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

		public const int AutoSlot = 4;

		private static string UserData
		{
			get { return Path.Combine(Path.GetDirectoryName(Application.dataPath), "UserData"); }
		}

		private static string SlotFile(int slot)
		{
			return Path.Combine(Path.Combine(UserData, "TruckPartsQOL"), slot == AutoSlot ? "auto.txt" : "slot" + slot + ".txt");
		}

		// The one shared file used before saves followed the game's slots.
		private static string FilePath
		{
			get { return Path.Combine(UserData, "TruckPartsQOL.txt"); }
		}

		public static string SlotName(int slot)
		{
			return slot == AutoSlot ? "the autosave" : "save slot " + slot;
		}

		public static void Delete(int slot)
		{
			try
			{
				if (File.Exists(SlotFile(slot)))
				{
					File.Delete(SlotFile(slot));
					TruckPartsQOLMod.Log("Deleted Truck Parts QOL data for " + SlotName(slot) + ".");
				}
			}
			catch (Exception e)
			{
				TruckPartsQOLMod.Log("Couldn't delete Truck Parts QOL data for " + SlotName(slot) + ": " + e.Message);
			}
		}

		// Saves from when this mod was called Truck Stereo.
		public static void MigrateOldSave()
		{
			try
			{
				string old = Path.Combine(UserData, "TruckStereo.txt");
				if (File.Exists(old) && !File.Exists(FilePath))
				{
					File.Move(old, FilePath);
					TruckPartsQOLMod.Log("Moved your saved parts from TruckStereo.txt to TruckPartsQOL.txt.");
				}
			}
			catch (Exception e)
			{
				TruckPartsQOLMod.Log("Couldn't move the old Truck Stereo save: " + e.Message);
			}
		}

		public static void Save(int slot)
		{
			try
			{
				StringBuilder sb = new StringBuilder();
				int parts = 0;
				foreach (TruckPart part in TruckPart.All)
				{
					if (part == null)
					{
						continue;
					}
					Transform t = part.transform;
					string vehicle = "";
					string mountPath = "";
					Vector3 p = t.position;
					Quaternion r = t.rotation;
					GameObject root = part.Vehicle;
					if (part.installed && root != null)
					{
						vehicle = root.name;
						mountPath = PathFrom(root.transform, t.parent) + "|" + t.parent.name.Replace(";", "").Replace("|", "");
						p = t.localPosition;
						r = t.localRotation;
					}
					HeadUnit unit = part.GetComponent<HeadUnit>();
					sb.Append(string.Join(";", new[]
					{
						part.kind.ToString(), part.cdNumber.ToString(Inv), vehicle, mountPath,
						F(p.x), F(p.y), F(p.z), F(r.x), F(r.y), F(r.z), F(r.w),
						unit != null && unit.powerOn ? "1" : "0",
						F(unit != null ? unit.volume : 0.6f),
						(unit != null ? unit.mode : 0).ToString(Inv),
						(unit != null ? unit.track : 0).ToString(Inv),
						(unit != null ? unit.insertedCd : 0).ToString(Inv),
						part.GetComponent<BedCover>() != null && part.GetComponent<BedCover>().open ? "1" : "0",
						F(part.GetComponent<BedCover>() != null ? part.GetComponent<BedCover>().heightOffset : 0f),
						part.Bolts != null && part.installed ? part.Bolts.Save() : ""
					}));
					sb.Append("\n");
					parts++;
				}
				int rims = SaveRims(sb);
				if (TruckPartsQOLMod.ScannerOwned)
				{
					sb.Append("SCANNER;1\n");
				}
				// Write to a temp file first so a crash mid-save can't leave half a file.
				string path = SlotFile(slot);
				Directory.CreateDirectory(Path.GetDirectoryName(path));
				string temp = path + ".tmp";
				File.WriteAllText(temp, sb.ToString());
				if (File.Exists(path))
				{
					File.Delete(path);
				}
				File.Move(temp, path);
				TruckPartsQOLMod.Log("Saved " + parts + " part(s) and " + rims + " painted rim(s) with " + SlotName(slot) + ".");
			}
			catch (Exception e)
			{
				TruckPartsQOLMod.Log("Saving Truck Parts QOL data failed: " + e.Message);
			}
		}

		// slot 0 is a new game: nothing to restore.
		public static void Load(int slot)
		{
			TruckPartsQOLMod.ScannerOwned = false;
			if (slot <= 0)
			{
				TruckPartsQOLMod.Log("New game: starting without Truck Parts QOL parts.");
				return;
			}
			string path = SlotFile(slot);
			bool legacy = false;
			if (!File.Exists(path))
			{
				// First load since saves started following the game's slots: bring the
				// old shared file into this slot, once.
				if (!File.Exists(FilePath))
				{
					TruckPartsQOLMod.Log("No Truck Parts QOL data saved with " + SlotName(slot) + " yet.");
					return;
				}
				path = FilePath;
				legacy = true;
				TruckPartsQOLMod.ScannerOwned = TruckPartsQOLMod.LegacyScannerOwned;
			}
			Dictionary<string, GameObject> vehicles = new Dictionary<string, GameObject>();
			foreach (GameObject vehicle in Vehicles.AllVehicles())
			{
				if (!vehicles.ContainsKey(vehicle.name))
				{
					vehicles[vehicle.name] = vehicle;
				}
			}
			int count = 0;
			int rims = 0;
			foreach (string line in File.ReadAllLines(path))
			{
				string[] f = line.Split(';');
				if (f.Length > 0 && f[0] == "RIM")
				{
					rims += LoadRim(f, vehicles) ? 1 : 0;
					continue;
				}
				if (f.Length > 1 && f[0] == "SCANNER")
				{
					TruckPartsQOLMod.ScannerOwned = f[1] == "1";
					continue;
				}
				if (f.Length < 16)
				{
					continue;
				}
				try
				{
					PartKind kind = (PartKind)Enum.Parse(typeof(PartKind), f[0]);
					int cd = int.Parse(f[1], Inv);
					Vector3 p = new Vector3(P(f[4]), P(f[5]), P(f[6]));
					Quaternion r = new Quaternion(P(f[7]), P(f[8]), P(f[9]), P(f[10]));
					GameObject vehicle;
					Transform mount = null;
					if (f[2].Length > 0 && vehicles.TryGetValue(f[2], out vehicle))
					{
						mount = Find(vehicle.transform, f[3]) ?? vehicle.transform;
					}
					TruckPart part;
					if (mount != null && BedCover.IsCover(kind))
					{
						// Covers refit to the bed rather than restoring a pose.
						part = PartFactory.Create(kind, cd, mount.position + Vector3.up, mount.rotation);
						BedCover cover = part.GetComponent<BedCover>();
						cover.open = f.Length > 16 && f[16] == "1";
						cover.heightOffset = f.Length > 17 ? P(f[17]) : 0f;
						string problem;
						if (!cover.Fit(Vehicles.FindRoot(mount) ?? mount.gameObject, false, out problem))
						{
							TruckPartsQOLMod.Log("Couldn't refit a " + part.DisplayName + ": " + problem);
						}
					}
					else if (mount != null)
					{
						part = PartFactory.Create(kind, cd, mount.TransformPoint(p), mount.rotation * r);
						part.Install(mount, mount.TransformPoint(p), mount.rotation * r * Vector3.forward);
						part.transform.localPosition = p;
						part.transform.localRotation = r;
						// Each bolt's turns; parts saved before bolts existed were fitted
						// for good, so they come back tight.
						if (part.Bolts != null)
						{
							if (f.Length > 18 && f[18].Length > 0)
							{
								part.Bolts.Load(f[18]);
							}
							else
							{
								part.Bolts.SetAll(PartBolts.Tight);
							}
						}
					}
					else
					{
						part = PartFactory.Create(kind, cd, p + Vector3.up * 0.1f, r);
					}
					HeadUnit unit = part.GetComponent<HeadUnit>();
					if (unit != null)
					{
						unit.powerOn = f[11] == "1";
						unit.volume = P(f[12]);
						unit.mode = int.Parse(f[13], Inv);
						unit.track = int.Parse(f[14], Inv);
						unit.insertedCd = int.Parse(f[15], Inv);
					}
					count++;
				}
				catch (Exception e)
				{
					TruckPartsQOLMod.Log("Skipping bad stereo save line: " + e.Message);
				}
			}
			TruckPartsQOLMod.Log("Restored " + count + " part(s) and " + rims + " painted rim(s) from " + (legacy ? "the old shared save" : SlotName(slot)) + ".");
			if (legacy)
			{
				try
				{
					File.Move(FilePath, FilePath + ".old");
					TruckPartsQOLMod.Log("Your parts now save with the game's save slots. Save the game to keep them in " + SlotName(slot) + ".");
				}
				catch (Exception e)
				{
					TruckPartsQOLMod.Log("Couldn't retire the old shared save: " + e.Message);
				}
			}
		}

		// RIM;vehicle;path|name;r;g;b;metallic;smoothness;orig r;g;b;m;s  (on a vehicle)
		// RIM;;rootName@x,y,z|path|name;...                              (a loose wheel)
		private static int SaveRims(StringBuilder sb)
		{
			int count = 0;
			foreach (RimPaint rim in Resources.FindObjectsOfTypeAll<RimPaint>())
			{
				if (rim == null || !rim.gameObject.scene.IsValid())
				{
					continue;
				}
				Transform t = rim.transform;
				GameObject vehicle = Vehicles.FindRoot(t);
				string vehicleName = "";
				string where;
				if (vehicle != null)
				{
					vehicleName = vehicle.name;
					where = PathFrom(vehicle.transform, t) + "|" + Clean(t.name);
				}
				else
				{
					Transform root = t.root;
					Vector3 p = root.position;
					where = Clean(root.name) + "@" + F(p.x) + "," + F(p.y) + "," + F(p.z) + "|" + PathFrom(root, t) + "|" + Clean(t.name);
				}
				sb.Append(string.Join(";", new[]
				{
					"RIM", Clean(vehicleName), where, F(rim.r), F(rim.g), F(rim.b), F(rim.metallic), F(rim.smoothness),
					rim.hasOriginal ? "1" : "0", F(rim.or), F(rim.og), F(rim.ob), F(rim.om), F(rim.os)
				}));
				sb.Append("\n");
				count++;
			}
			return count;
		}

		private static bool LoadRim(string[] f, Dictionary<string, GameObject> vehicles)
		{
			if (f.Length < 14)
			{
				return false;
			}
			Transform rim = null;
			GameObject vehicle;
			if (f[1].Length > 0)
			{
				if (vehicles.TryGetValue(f[1], out vehicle))
				{
					rim = Find(vehicle.transform, f[2]);
				}
			}
			else
			{
				// Loose wheel: the same-named root object nearest the saved spot.
				string[] parts = f[2].Split('|');
				int at = parts[0].LastIndexOf('@');
				if (parts.Length == 3 && at > 0)
				{
					string rootName = parts[0].Substring(0, at);
					string[] xyz = parts[0].Substring(at + 1).Split(',');
					Vector3 saved = new Vector3(P(xyz[0]), P(xyz[1]), P(xyz[2]));
					Transform best = null;
					float bestDistance = 1.5f;
					foreach (GameObject go in UnityEngine.Object.FindObjectsOfType<GameObject>())
					{
						if (go.transform.parent == null && Clean(go.name) == rootName)
						{
							float d = Vector3.Distance(go.transform.position, saved);
							if (d < bestDistance)
							{
								bestDistance = d;
								best = go.transform;
							}
						}
					}
					if (best != null)
					{
						rim = Find(best, parts[1] + "|" + parts[2]);
					}
				}
			}
			if (rim == null || rim.GetComponent<RimPaint>() != null)
			{
				return false;
			}
			RimPaint rp = rim.gameObject.AddComponent<RimPaint>();
			rp.hasOriginal = f[8] == "1";
			rp.or = P(f[9]);
			rp.og = P(f[10]);
			rp.ob = P(f[11]);
			rp.om = P(f[12]);
			rp.os = P(f[13]);
			RimPaint.Set(rim, new PaintColor(P(f[3]), P(f[4]), P(f[5]), P(f[6]), P(f[7])));
			return true;
		}

		private static string Clean(string s)
		{
			return s.Replace(";", "").Replace("|", "").Replace("@", "");
		}

		// Child-index path from root to t, e.g. "3/0"; "" when t is root.
		private static string PathFrom(Transform root, Transform t)
		{
			List<string> steps = new List<string>();
			for (; t != null && t != root; t = t.parent)
			{
				steps.Insert(0, t.GetSiblingIndex().ToString(Inv));
			}
			return string.Join("/", steps.ToArray());
		}

		// "3/0|DoorL": follow the indexes, then make sure the name still matches
		// (the hierarchy may have changed, e.g. an engine was swapped).
		private static Transform Find(Transform root, string saved)
		{
			int bar = saved.LastIndexOf('|');
			string path = bar >= 0 ? saved.Substring(0, bar) : saved;
			string name = bar >= 0 ? saved.Substring(bar + 1) : null;
			if (string.IsNullOrEmpty(path))
			{
				return root;
			}
			Transform t = root;
			foreach (string step in path.Split('/'))
			{
				int index = int.Parse(step, Inv);
				if (index >= t.childCount)
				{
					return null;
				}
				t = t.GetChild(index);
			}
			return name == null || t.name.Replace(";", "").Replace("|", "") == name ? t : null;
		}

		private static string F(float v)
		{
			return v.ToString("R", Inv);
		}

		private static float P(string s)
		{
			return float.Parse(s, Inv);
		}
	}
}
