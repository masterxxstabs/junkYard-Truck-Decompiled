using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EngineCloner
{
	// Engines this mod made (bought, or debug clones) aren't part of the game's own
	// save, so they're saved here with the game's save slots, like our other mods:
	// UserData/EngineCloner/slotN.txt (auto.txt). One line per engine:
	//
	//   ENGINE;kind;place;vehicle;px;py;pz;rx;ry;rz;rw;state
	//
	// place: loose (world pose), stand (on the engine stand) or vehicle (bolted in
	// the named vehicle, pose relative to it). state is a snapshot of the block:
	// the engine's condition and fluid numbers, and for every part slot and bolt
	// in it whether it's there, its colliders, condition and bolt turns. A restored
	// engine is a fresh copy with that snapshot put back.
	internal static class EngineSave
	{
		private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
		public const int AutoSlot = 4;

		private static readonly string[] EngineNumbers = { "newOilLevel", "newCoolantLevel", "newFuelLevel", "newTemperature", "oilLoss" };

		private static string File(int slot)
		{
			string dir = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.dataPath), "UserData"), "EngineCloner");
			return Path.Combine(dir, slot == AutoSlot ? "auto.txt" : "slot" + slot + ".txt");
		}

		public static string SlotName(int slot)
		{
			return slot == AutoSlot ? "the autosave" : "save slot " + slot;
		}

		// --- Saving. ---

		public static void Save(int slot)
		{
			try
			{
				StringBuilder sb = new StringBuilder();
				// Stands first: engines on them are put back on the nearest stand.
				StandShop.SaveLines(sb);
				int n = 0;
				foreach (ShopEngine e in Object.FindObjectsOfType<ShopEngine>())
				{
					if (e == null || !e.ready)
					{
						continue;
					}
					Transform t = e.transform;
					FixedJoint joint = e.GetComponent<FixedJoint>();
					string place = "loose";
					string vehicle = "";
					Vector3 p = t.position;
					Quaternion r = t.rotation;
					Rigidbody to = joint != null ? joint.connectedBody : null;
					if (to != null && to.name == "DK_9")
					{
						place = "stand";
					}
					else if (to != null && t.parent != null && to.transform == t.parent)
					{
						place = "vehicle";
						vehicle = t.parent.name;
						p = t.localPosition;
						r = t.localRotation;
					}
					sb.Append(string.Join(";", new[]
					{
						"ENGINE", e.kind, place, Clean(vehicle), F(p.x), F(p.y), F(p.z), F(r.x), F(r.y), F(r.z), F(r.w), Snapshot(e)
					})).Append('\n');
					n++;
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
				EngineClonerMod.Log("Saved " + n + " bought/cloned engine(s) with " + SlotName(slot) + ".");
			}
			catch (Exception e)
			{
				EngineClonerMod.Log("Saving engines failed: " + e.Message);
			}
		}

		// N=path,name,active,renderer,colliders,health,canDetach,boltStr  (part slots)
		// B=path,turns,renderer,collider,lx,ly,lz,qx,qy,qz,qw          (bolts)
		// F=name,value  I=name,value                                     (engine numbers)
		private static string Snapshot(ShopEngine e)
		{
			List<string> items = new List<string>();
			EngineShop.Kind k = EngineShop.KindOf(e.kind);
			Component script = k != null ? e.GetComponent(k.script) : null;
			if (script != null)
			{
				foreach (FieldInfo f in script.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (f.FieldType == typeof(float) && (f.Name.EndsWith("_cnd", StringComparison.Ordinal) || Array.IndexOf(EngineNumbers, f.Name) >= 0))
					{
						items.Add("F=" + f.Name + "," + F((float)f.GetValue(script)));
					}
					else if (f.FieldType == typeof(int) && f.Name == "whichTruck")
					{
						items.Add("I=" + f.Name + "," + ((int)f.GetValue(script)).ToString(Inv));
					}
				}
			}
			foreach (durability d in e.GetComponentsInChildren<durability>(true))
			{
				Renderer r = d.GetComponent<Renderer>();
				StringBuilder cols = new StringBuilder();
				foreach (BoxCollider c in d.GetComponents<BoxCollider>())
				{
					cols.Append(c.enabled ? '1' : '0');
				}
				items.Add("N=" + PathOf(e.transform, d.transform) + "," + Clean(d.name) + "," + B(d.gameObject.activeSelf) + "," + B(r != null && r.enabled) + "," + (cols.Length > 0 ? cols.ToString() : "-") + "," + F(d.health) + "," + B(d.canDetach) + "," + F(d.boltStr));
			}
			foreach (BoltScript b in e.GetComponentsInChildren<BoltScript>(true))
			{
				Transform t = b.transform;
				Renderer r = b.GetComponent<Renderer>();
				Collider c = b.GetComponent<Collider>();
				items.Add("B=" + PathOf(e.transform, t) + "," + b.boltturns.ToString(Inv) + "," + B(r != null && r.enabled) + "," + B(c != null && c.enabled) + "," + F(t.localPosition.x) + "," + F(t.localPosition.y) + "," + F(t.localPosition.z) + "," + F(t.localRotation.x) + "," + F(t.localRotation.y) + "," + F(t.localRotation.z) + "," + F(t.localRotation.w));
			}
			// Everything else that shows or can be clicked (rockers, oil caps, ...):
			// whether its mesh and each box collider are on.
			foreach (Transform t in e.GetComponentsInChildren<Transform>(true))
			{
				Renderer r = t.GetComponent<Renderer>();
				BoxCollider[] boxes = t.GetComponents<BoxCollider>();
				if (t == e.transform || (r == null && boxes.Length == 0))
				{
					continue;
				}
				StringBuilder cols = new StringBuilder();
				foreach (BoxCollider c in boxes)
				{
					cols.Append(c.enabled ? '1' : '0');
				}
				items.Add("C=" + PathOf(e.transform, t) + "," + (r == null ? "-" : B(r.enabled)) + "," + (cols.Length > 0 ? cols.ToString() : "-"));
			}
			return string.Join("|", items.ToArray());
		}

		// --- Loading. ---

		public static void Load(int slot)
		{
			if (slot <= 0 || !System.IO.File.Exists(File(slot)))
			{
				return;
			}
			int count = 0;
			string[] all = System.IO.File.ReadAllLines(File(slot));
			int stands = StandShop.Load(all);
			if (stands > 0)
			{
				EngineClonerMod.Log("Restored " + stands + " bought engine stand(s).");
			}
			foreach (string line in all)
			{
				string[] f = line.Split(';');
				if (f.Length < 12 || f[0] != "ENGINE")
				{
					continue;
				}
				try
				{
					EngineShop.Kind k = EngineShop.KindOf(f[1]);
					GameObject template = k != null ? EngineShop.Template(k) : null;
					if (template == null)
					{
						EngineClonerMod.Log("Couldn't restore a " + f[1] + " engine: none in this level to rebuild it from.");
						continue;
					}
					Vector3 p = new Vector3(P(f[4]), P(f[5]), P(f[6]));
					Quaternion r = new Quaternion(P(f[7]), P(f[8]), P(f[9]), P(f[10]));
					// Made where it belongs: it takes its private anchor where it's created.
					if (f[2] == "vehicle")
					{
						GameObject vehicle = FindVehicle(f[3]);
						if (vehicle != null)
						{
							p = vehicle.transform.TransformPoint(p);
							r = vehicle.transform.rotation * r;
						}
					}
					GameObject block = Object.Instantiate(template, p, r);
					block.name = template.name;
					ShopEngine e = block.GetComponent<ShopEngine>();
					e.pendingState = string.Join(";", f, 2, 10);
					count++;
				}
				catch (Exception e)
				{
					EngineClonerMod.Log("Skipping a bad engine save line: " + e.Message);
				}
			}
			EngineClonerMod.Log("Restored " + count + " bought/cloned engine(s) from " + SlotName(slot) + ".");
		}

		// Once the restored copy is set up: put the snapshot back and mount it.
		public static void Apply(ShopEngine e, string saved)
		{
			try
			{
				string[] f = saved.Split(new[] { ';' }, 10);
				string place = f[0];
				string vehicleName = f[1];
				Vector3 p = new Vector3(P(f[2]), P(f[3]), P(f[4]));
				Quaternion r = new Quaternion(P(f[5]), P(f[6]), P(f[7]), P(f[8]));
				ApplySnapshot(e, f[9]);
				Place(e, place, vehicleName, p, r);
				EngineShop.Kind k = EngineShop.KindOf(e.kind);
				Component script = k != null ? e.GetComponent(k.script) : null;
				MethodInfo refresh = script != null ? script.GetType().GetMethod("Refresh", Type.EmptyTypes) : null;
				if (refresh != null)
				{
					refresh.Invoke(script, null);
				}
			}
			catch (Exception ex)
			{
				EngineClonerMod.Log("Couldn't fully restore an engine: " + ex);
			}
		}

		private static void ApplySnapshot(ShopEngine e, string snapshot)
		{
			EngineShop.Kind k = EngineShop.KindOf(e.kind);
			Component script = k != null ? e.GetComponent(k.script) : null;
			foreach (string item in snapshot.Split('|'))
			{
				if (item.Length < 3)
				{
					continue;
				}
				string[] v = item.Substring(2).Split(',');
				switch (item[0])
				{
				case 'F':
					if (script != null && v.Length >= 2)
					{
						EngineShop.SetField(script, v[0], P(v[1]));
					}
					break;
				case 'I':
					if (script != null && v.Length >= 2)
					{
						EngineShop.SetField(script, v[0], int.Parse(v[1], Inv));
					}
					break;
				case 'N':
				{
					Transform t = Find(e.transform, v[0]);
					durability d = t != null ? t.GetComponent<durability>() : null;
					if (d == null || v.Length < 8 || Clean(d.name) != v[1])
					{
						break;
					}
					t.gameObject.SetActive(v[2] == "1");
					Renderer r = t.GetComponent<Renderer>();
					if (r != null)
					{
						r.enabled = v[3] == "1";
					}
					BoxCollider[] cols = t.GetComponents<BoxCollider>();
					for (int i = 0; i < cols.Length && i < v[4].Length && v[4] != "-"; i++)
					{
						cols[i].enabled = v[4][i] == '1';
					}
					d.health = P(v[5]);
					d.canDetach = v[6] == "1";
					d.boltStr = P(v[7]);
					break;
				}
				case 'C':
				{
					Transform t = Find(e.transform, v[0]);
					if (t == null || v.Length < 3)
					{
						break;
					}
					Renderer r = t.GetComponent<Renderer>();
					if (r != null && v[1] != "-")
					{
						r.enabled = v[1] == "1";
					}
					BoxCollider[] boxes = t.GetComponents<BoxCollider>();
					for (int i = 0; i < boxes.Length && v[2] != "-" && i < v[2].Length; i++)
					{
						boxes[i].enabled = v[2][i] == '1';
					}
					break;
				}
				case 'B':
				{
					Transform t = Find(e.transform, v[0]);
					BoltScript b = t != null ? t.GetComponent<BoltScript>() : null;
					if (b == null || v.Length < 11)
					{
						break;
					}
					b.boltturns = int.Parse(v[1], Inv);
					Renderer r = t.GetComponent<Renderer>();
					if (r != null)
					{
						r.enabled = v[2] == "1";
					}
					Collider c = t.GetComponent<Collider>();
					if (c != null)
					{
						c.enabled = v[3] == "1";
					}
					t.localPosition = new Vector3(P(v[4]), P(v[5]), P(v[6]));
					t.localRotation = new Quaternion(P(v[7]), P(v[8]), P(v[9]), P(v[10]));
					break;
				}
				}
			}
		}

		// Where it sat: loose (as placed), on the engine stand, or bolted in a vehicle
		// the way the game mounts one (pose, joint to the vehicle, its engine flags).
		private static void Place(ShopEngine e, string place, string vehicleName, Vector3 p, Quaternion r)
		{
			GameObject block = e.gameObject;
			FixedJoint joint = block.GetComponent<FixedJoint>();
			PickUp pick = block.GetComponent<PickUp>();
			if (place == "stand")
			{
				Rigidbody stand = StandShop.NearestStand(block.transform.position);
				if (stand != null && joint != null)
				{
					joint.connectedBody = stand;
					if (pick != null)
					{
						pick.pickable = false;
					}
				}
				return;
			}
			if (place != "vehicle")
			{
				return;
			}
			GameObject vehicle = FindVehicle(vehicleName);
			if (vehicle == null || joint == null)
			{
				EngineClonerMod.Log("Couldn't find the vehicle \"" + vehicleName + "\" a saved " + block.name + " was in; it's left loose.");
				return;
			}
			block.transform.SetParent(vehicle.transform, false);
			block.transform.localPosition = p;
			block.transform.localRotation = r;
			joint.connectedBody = vehicle.GetComponent<Rigidbody>();
			if (pick != null)
			{
				pick.pickable = false;
			}
			// What the game's mounting sets on the vehicle (PickUp.LetGo).
			Component car = vehicle.GetComponent("car");
			Component car4 = vehicle.GetComponent("car4");
			Component car3 = vehicle.GetComponent("car3");
			if (car != null)
			{
				EngineShop.SetField(car, "usingV8", block.name == "v8_block");
				EngineShop.SetField(car, "usingI6", block.name == "i6block");
				Call(car, block.name == "v8_block" ? "v8Brackets" : block.name == "i6block" ? "i6Brackets" : "v4Brackets");
			}
			else if (car4 != null && block.name == "v8_block")
			{
				EngineShop.SetField(car4, "usingV8", true);
				Call(car4, "v8Brackets");
			}
			else if (car3 != null && block.name == "i6block")
			{
				EngineShop.SetField(car3, "usingI6", true);
			}
		}

		private static GameObject FindVehicle(string name)
		{
			foreach (string type in new[] { "car", "car4", "car3" })
			{
				Type t = AccessTools.TypeByName(type);
				if (t == null)
				{
					continue;
				}
				foreach (Object o in Object.FindObjectsOfType(t))
				{
					GameObject go = ((Component)o).gameObject;
					if (Clean(go.name) == name)
					{
						return go;
					}
				}
			}
			return null;
		}

		private static void Call(Component c, string method)
		{
			MethodInfo m = c.GetType().GetMethod(method, Type.EmptyTypes);
			if (m != null)
			{
				m.Invoke(c, null);
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
				EngineClonerMod.Log("Couldn't delete engine data for " + SlotName(slot) + ": " + e.Message);
			}
		}

		// --- Helpers. ---

		// Child indexes from root down to t ("" for root itself), e.g. "3/0/2".
		private static string PathOf(Transform root, Transform t)
		{
			List<string> parts = new List<string>();
			for (; t != null && t != root; t = t.parent)
			{
				parts.Insert(0, t.GetSiblingIndex().ToString(Inv));
			}
			return string.Join("/", parts.ToArray());
		}

		private static Transform Find(Transform root, string path)
		{
			Transform t = root;
			if (path.Length == 0)
			{
				return t;
			}
			foreach (string s in path.Split('/'))
			{
				int i = int.Parse(s, Inv);
				if (i < 0 || i >= t.childCount)
				{
					return null;
				}
				t = t.GetChild(i);
			}
			return t;
		}

		private static string Clean(string s)
		{
			return (s ?? "").Replace(";", "").Replace("|", "").Replace(",", "").Replace("\n", "");
		}

		private static string B(bool b)
		{
			return b ? "1" : "0";
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

	// The game's save points, like our other mods: MainMenu.OptionSave/2/3/Auto and
	// Easy Save's ES3AutoSaveMgr.Save1/2/3/SaveAuto.
	[HarmonyPatch]
	internal static class EngineMainMenuSavePatch
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
			EngineClonerMod.OnGameSave(__originalMethod.Name);
		}
	}

	[HarmonyPatch(typeof(MainMenu), "DeleteSlot")]
	internal static class EngineDeleteSlotPatch
	{
		private static void Prefix(int ___slotToDelete)
		{
			EngineSave.Delete(___slotToDelete);
		}
	}

	[HarmonyPatch(typeof(MainMenu), "PlayGameOverwrite")]
	internal static class EngineOverwritePatch
	{
		public static bool NewGame;

		private static void Prefix()
		{
			EngineSave.Delete(1);
			NewGame = true;
		}
	}
}
