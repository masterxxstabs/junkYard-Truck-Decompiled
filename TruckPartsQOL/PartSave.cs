using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace TruckPartsQOL
{
	// One line per part in UserData/TruckPartsQOL.txt:
	// kind;cd;vehicle;mountPath;px;py;pz;rx;ry;rz;rw;power;volume;mode;track;insertedCd;coverOpen
	// (coverOpen was added later; older lines without it still load.)
	// Installed parts store their pose relative to what they're mounted on and are
	// re-installed on the first vehicle with that name; loose parts store world pose.
	internal static class PartSave
	{
		private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

		private static string UserData
		{
			get { return Path.Combine(Path.GetDirectoryName(Application.dataPath), "UserData"); }
		}

		private static string FilePath
		{
			get { return Path.Combine(UserData, "TruckPartsQOL.txt"); }
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

		public static void Save()
		{
			try
			{
				StringBuilder sb = new StringBuilder();
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
						part.GetComponent<BedCover>() != null && part.GetComponent<BedCover>().open ? "1" : "0"
					}));
					sb.Append("\n");
				}
				Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
				File.WriteAllText(FilePath, sb.ToString());
			}
			catch (Exception e)
			{
				TruckPartsQOLMod.Log("Saving stereo parts failed: " + e.Message);
			}
		}

		public static void Load()
		{
			if (!File.Exists(FilePath))
			{
				return;
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
			foreach (string line in File.ReadAllLines(FilePath))
			{
				string[] f = line.Split(';');
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
			TruckPartsQOLMod.Log("Restored " + count + " stereo part(s).");
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
