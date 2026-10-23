using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FurnitureTweaks
{
	// Furniture is saved with the game's save slots, through Engine Cloner's save
	// hooks: UserData/FurnitureTweaks/slot1.txt..slot3.txt and auto.txt, a line a
	// piece:
	//
	//   FURNITURE;1;kind;id;px;py;pz;yaw;data
	//
	// data: a safe's cash; a crate's parts by name, in order; a shelf's parts by
	// spot, name and pose. The parts themselves are saved by the game, where they
	// are (a crate's out of sight inside it, switched off, the way the game's own
	// inventory keeps things), and picked back up by name and place once the game
	// has brought them back.
	internal static class Save
	{
		public const int AutoSlot = 4;
		public static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

		// Restoring: pieces still finding their parts, until this time.
		private static readonly List<Furniture> adopting = new List<Furniture>();
		private static float adoptUntil, nextAdopt;

		private static string File(int slot)
		{
			return Path.Combine(FurnitureMod.Folder, slot == AutoSlot ? "auto.txt" : "slot" + slot + ".txt");
		}

		public static void Reset()
		{
			adopting.Clear();
		}

		public static void Write(int slot)
		{
			StringBuilder sb = new StringBuilder();
			int n = 0;
			foreach (Furniture f in Object.FindObjectsOfType<Furniture>())
			{
				Transform t = f.transform;
				sb.Append(string.Join(";", new[]
				{
					"FURNITURE", "1", f.kind, Items.Escape(f.id),
					F(t.position.x), F(t.position.y), F(t.position.z), F(t.eulerAngles.y),
					f.SaveData()
				})).Append('\n');
				n++;
			}
			string path = File(slot);
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			if (n == 0)
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
			FurnitureMod.Log("Saved " + n + " piece(s) of furniture.");
		}

		public static void Read(int slot)
		{
			adopting.Clear();
			if (slot <= 0 || !System.IO.File.Exists(File(slot)))
			{
				return;
			}
			int n = 0;
			foreach (string line in System.IO.File.ReadAllLines(File(slot)))
			{
				string[] f = line.Split(new[] { ';' }, 9);
				if (f.Length < 9 || f[0] != "FURNITURE")
				{
					continue;
				}
				try
				{
					Vector3 at = new Vector3(P(f[4]), P(f[5]), P(f[6]));
					Furniture piece = Build.Make(f[2], at, P(f[7]), f[3]);
					if (piece == null)
					{
						continue;
					}
					piece.LoadData(f[8]);
					adopting.Add(piece);
					n++;
				}
				catch (Exception e)
				{
					FurnitureMod.Log("Skipping a bad furniture save line: " + e.Message);
				}
			}
			// The game may still be bringing its parts back: keep looking a while.
			adoptUntil = Time.time + 30f;
			nextAdopt = 0f;
			FurnitureMod.Log("Restored " + n + " piece(s) of furniture.");
		}

		public static void Update()
		{
			if (adopting.Count == 0 || Time.time < nextAdopt)
			{
				return;
			}
			nextAdopt = Time.time + 1f;
			bool last = Time.time > adoptUntil;
			for (int i = adopting.Count - 1; i >= 0; i--)
			{
				Furniture f = adopting[i];
				try
				{
					if (f == null || f.Adopt())
					{
						adopting.RemoveAt(i);
					}
					else if (last)
					{
						f.Forget();
						adopting.RemoveAt(i);
					}
				}
				catch (Exception e)
				{
					FurnitureMod.Log("Couldn't put the parts back in a " + (f != null ? Build.NameOf(f.kind) : "piece of furniture") + ": " + e.Message);
					adopting.RemoveAt(i);
				}
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
				FurnitureMod.Log("Couldn't delete the furniture saved with slot " + slot + ": " + e.Message);
			}
		}

		public static string F(float v)
		{
			return v.ToString("R", Inv);
		}

		public static float P(string s)
		{
			return float.Parse(s, NumberStyles.Float, Inv);
		}
	}
}
