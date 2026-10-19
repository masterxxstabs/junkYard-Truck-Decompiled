using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EngineCloner
{
	// For other mods that build vehicles out of the game's own (Junkyard Ranger is
	// a copy of the Diamondback with a new body). A copy made here works like the
	// debug clones: the game is wired to whichever one you get into, each has its
	// own engine, and engines bolted into it are saved with it.
	//
	// Typical use: Original("car") -> CopyInactive -> StripEngines -> (change the
	// copy: it hasn't run yet) -> Activate -> MountNewEngine -> Tag. Save and load
	// your own data from Saving/Loading/Deleting, which follow the game's save slots
	// (1-3, 4 = autosave; Loading gets 0 for a new game) and run before this mod
	// saves or restores its engines.
	public static class VehicleApi
	{
		public const int Version = 1;

		public static event Action<int> Saving;
		public static event Action<int> Loading;
		public static event Action<int> Deleting;

		internal static void RaiseSave(int slot)
		{
			Raise(Saving, slot, "saving");
		}

		internal static void RaiseLoad(int slot)
		{
			Raise(Loading, slot, "loading");
		}

		internal static void RaiseDelete(int slot)
		{
			Raise(Deleting, slot, "deleting save data for");
		}

		private static void Raise(Action<int> handlers, int slot, string what)
		{
			if (handlers == null)
			{
				return;
			}
			foreach (Delegate d in handlers.GetInvocationList())
			{
				try
				{
					((Action<int>)d)(slot);
				}
				catch (Exception e)
				{
					EngineClonerMod.Log("A mod failed " + what + " " + EngineSave.SlotName(slot) + ": " + e);
				}
			}
		}

		// --- Copies. ---

		// The player is sitting in a vehicle (copying it would copy them too).
		public static bool PlayerInVehicle()
		{
			return VehicleCloner.PlayerInVehicle();
		}

		// The game's own vehicle with this script ("car" = the Diamondback,
		// "car4" = the F100, "car3" = the AMC), not a copy.
		public static GameObject Original(string script)
		{
			Type type = AccessTools.TypeByName(script);
			return type != null ? VehicleCloner.Original(type) : null;
		}

		public static bool IsCopy(GameObject vehicle)
		{
			return VehicleCloner.IsCopy(vehicle);
		}

		public static GameObject CopyInactive(GameObject original)
		{
			return VehicleCloner.CopyInactive(original);
		}

		public static void Activate(GameObject copy, GameObject original, Vector3 position, Quaternion rotation)
		{
			VehicleCloner.Activate(copy, original, position, rotation);
		}

		public static bool FindGroundNear(GameObject original, Vector3 near, out Vector3 position)
		{
			return VehicleCloner.FindGroundNear(original, near, out position);
		}

		// Point the game at this vehicle, as getting in does.
		public static void Wire(GameObject vehicle)
		{
			VehicleCloner.Wire(vehicle);
		}

		// Take the engine blocks out of a copy that hasn't been activated. What
		// pointed at them points at the original's instead, as for any engine the
		// copy doesn't have, so the copy has no engine until one is bolted in.
		public static int StripEngines(GameObject copy, GameObject original)
		{
			List<GameObject> found = new List<GameObject>();
			foreach (Transform t in copy.GetComponentsInChildren<Transform>(true))
			{
				if (EngineClonerMod.IsEngineBlock(t.gameObject) && !found.Contains(t.gameObject))
				{
					found.Add(t.gameObject);
				}
			}
			foreach (GameObject block in found)
			{
				Transform counterpart = HierarchyPath.Find(original.transform, HierarchyPath.Of(copy.transform, block.transform));
				if (counterpart != null)
				{
					Dictionary<Object, Object> map = new Dictionary<Object, Object>();
					ReferenceSwapper.MapOneWay(block.transform, counterpart, map);
					ReferenceSwapper.Redirect(copy.transform, map, block.transform);
				}
				Object.DestroyImmediate(block);
			}
			return found.Count;
		}

		// --- Engines. ---

		// The engine bay's snap point for each engine, per vehicle (PickUp.LetGo).
		private static string BayName(GameObject vehicle, string kind)
		{
			if (vehicle.GetComponent<car>() != null)
			{
				return kind == "v8" ? "ebtemplate2v8" : kind == "i6" ? "ebtemplatei62" : "ebtemplate2";
			}
			if (vehicle.GetComponent<car4>() != null)
			{
				return kind == "v8" ? "ebtemplate3v8" : null;
			}
			if (vehicle.GetComponent<car3>() != null)
			{
				return kind == "i6" ? "ebtemplatei6C" : null;
			}
			return null;
		}

		private static Transform Bay(GameObject vehicle, string kind)
		{
			string name = BayName(vehicle, kind);
			if (name == null)
			{
				return null;
			}
			foreach (Transform t in vehicle.GetComponentsInChildren<Transform>(true))
			{
				if (t.name == name)
				{
					return t;
				}
			}
			return null;
		}

		// A new engine ("i4", "v8", "i6") bolted into the vehicle's engine bay:
		// complete (every part on, new) or a bare block, with or without oil. It's
		// one of this mod's engines, so it's saved with the vehicle.
		public static GameObject MountNewEngine(GameObject vehicle, string kind, bool complete, bool withOil)
		{
			EngineShop.Kind k = EngineShop.KindOf(kind);
			Transform bay = k != null ? Bay(vehicle, kind) : null;
			if (bay == null)
			{
				EngineClonerMod.Log("No engine bay for a " + kind + " in " + vehicle.name + ".");
				return null;
			}
			GameObject template = EngineShop.Template(k, !complete);
			if (template == null)
			{
				return null;
			}
			GameObject block = Object.Instantiate(template, bay.position, bay.rotation);
			block.name = template.name;
			ShopEngine marker = block.GetComponent<ShopEngine>();
			if (marker != null)
			{
				marker.mountedAtStart = true;
			}
			if (withOil)
			{
				EngineShop.SetField(block.GetComponent(k.script), "newOilLevel", 100f);
			}
			EngineSave.Mount(block, vehicle, vehicle.transform.InverseTransformPoint(bay.position), Quaternion.Inverse(vehicle.transform.rotation) * bay.rotation);
			return block;
		}

		// Bolt an existing block (e.g. the game's own engine) into the vehicle.
		public static bool MountEngine(GameObject block, GameObject vehicle)
		{
			EngineShop.Kind k = EngineShop.KindOf(block);
			Transform bay = k != null ? Bay(vehicle, k.id) : null;
			if (bay == null)
			{
				return false;
			}
			block.transform.SetPositionAndRotation(bay.position, bay.rotation);
			EngineSave.Mount(block, vehicle, vehicle.transform.InverseTransformPoint(bay.position), Quaternion.Inverse(vehicle.transform.rotation) * bay.rotation);
			return true;
		}

		// The engine block bolted into the vehicle, if any.
		public static GameObject MountedEngine(GameObject vehicle)
		{
			foreach (Transform t in vehicle.transform)
			{
				FixedJoint joint = t.GetComponent<FixedJoint>();
				if (EngineClonerMod.IsEngineBlock(t.gameObject) && joint != null && joint.connectedBody != null && joint.connectedBody.transform == vehicle.transform)
				{
					return t.gameObject;
				}
			}
			return null;
		}

		// The game's own engine of this kind ("i4", "v8", "i6"), if it's not bolted
		// into any vehicle right now.
		public static GameObject LooseGameEngine(string kind)
		{
			EngineShop.Kind k = EngineShop.KindOf(kind);
			if (k == null)
			{
				return null;
			}
			foreach (Object o in Object.FindObjectsOfType(k.script))
			{
				GameObject block = ((Component)o).gameObject;
				if (!EngineClonerMod.IsEngineBlock(block) || IsOurEngine(block))
				{
					continue;
				}
				Transform parent = block.transform.parent;
				if (parent == null || VehicleCloner.FindVehicleRoot(parent) == null)
				{
					return block;
				}
			}
			return null;
		}

		// "i4", "v8", "i6", or null.
		public static string KindOf(GameObject block)
		{
			EngineShop.Kind k = block != null ? EngineShop.KindOf(block) : null;
			return k != null ? k.id : null;
		}

		// One of this mod's engines (bought, cloned or mounted by MountNewEngine),
		// which it saves itself; false for the game's own engines.
		public static bool IsOurEngine(GameObject block)
		{
			return block != null && block.GetComponent<ShopEngine>() != null;
		}

		// --- Telling copies apart in saves. ---

		// Copies keep the original's name (the game finds vehicles by name), so
		// engines saved in a copy need its tag to find it again.
		public static void Tag(GameObject vehicle, string id)
		{
			VehicleTag tag = vehicle.GetComponent<VehicleTag>();
			if (tag == null)
			{
				tag = vehicle.AddComponent<VehicleTag>();
			}
			tag.id = id;
		}

		public static string TagOf(GameObject vehicle)
		{
			VehicleTag tag = vehicle != null ? vehicle.GetComponent<VehicleTag>() : null;
			return tag != null && !string.IsNullOrEmpty(tag.id) ? tag.id : null;
		}

		internal static string SaveName(GameObject vehicle)
		{
			string tag = TagOf(vehicle);
			return tag != null ? vehicle.name + "#" + tag : vehicle.name;
		}

		// --- Snapshots: a vehicle's state as text, to save and put back. ---

		private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

		// Fields of the vehicle script that are moment-to-moment state, not worth
		// saving (and wrong to restore).
		private static readonly HashSet<string> Transient = new HashSet<string>
		{
			"controlled", "userControlled", "keyState", "speed", "torque", "frontTorque", "rearTorque", "braked", "clutchIn", "physClutchIn",
			"canRun", "canCrank", "canAcc", "canMove", "canEasyStart", "steerAngle", "steerWheelAngle", "i", "surfaceType", "deepMud",
			"waterFlood", "deepWater", "jumptime", "maxTorque", "maxSpeed", "jacked"
		};

		// Every part's on/off state, condition and bolts, and the vehicle script's
		// numbers, keyed by object names (stable between copies). Engines bolted in
		// are left out (saved on their own), and anything skip() says.
		public static string Snapshot(GameObject vehicle, Func<Transform, bool> skip)
		{
			List<string> items = new List<string>();
			MonoBehaviour script = VehicleScript(vehicle);
			if (script != null)
			{
				foreach (FieldInfo f in script.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
				{
					if (Transient.Contains(f.Name))
					{
						continue;
					}
					object v = f.GetValue(script);
					if (v is float)
					{
						items.Add("F=" + f.Name + "," + ((float)v).ToString("R", Inv));
					}
					else if (v is int)
					{
						items.Add("I=" + f.Name + "," + ((int)v).ToString(Inv));
					}
					else if (v is bool)
					{
						items.Add("Z=" + f.Name + "," + ((bool)v ? "1" : "0"));
					}
				}
			}
			Walk(vehicle.transform, vehicle.transform, skip, items);
			return string.Join("|", items.ToArray());
		}

		private static void Walk(Transform root, Transform t, Func<Transform, bool> skip, List<string> items)
		{
			foreach (Transform child in t)
			{
				if (EngineClonerMod.IsEngineBlock(child.gameObject) || (skip != null && skip(child)))
				{
					continue;
				}
				string path = HierarchyPath.Of(root, child);
				Renderer r = child.GetComponent<Renderer>();
				BoxCollider[] boxes = child.GetComponents<BoxCollider>();
				StringBuilder cols = new StringBuilder();
				foreach (BoxCollider c in boxes)
				{
					cols.Append(c.enabled ? '1' : '0');
				}
				items.Add("T=" + path + "," + (child.gameObject.activeSelf ? "1" : "0") + "," + (r == null ? "-" : r.enabled ? "1" : "0") + "," + (cols.Length > 0 ? cols.ToString() : "-"));
				durability d = child.GetComponent<durability>();
				if (d != null)
				{
					items.Add("D=" + path + "," + d.health.ToString("R", Inv) + "," + (d.canDetach ? "1" : "0") + "," + d.boltStr.ToString("R", Inv));
				}
				BoltScript b = child.GetComponent<BoltScript>();
				if (b != null)
				{
					items.Add("B=" + path + "," + b.boltturns.ToString(Inv) + "," + Pose(child));
				}
				BoltScriptSusp sb = child.GetComponent<BoltScriptSusp>();
				if (sb != null)
				{
					items.Add("S=" + path + "," + sb.boltTurns.ToString(Inv) + "," + Pose(child));
				}
				Walk(root, child, skip, items);
			}
		}

		public static void ApplySnapshot(GameObject vehicle, string snapshot, Func<Transform, bool> skip)
		{
			MonoBehaviour script = VehicleScript(vehicle);
			Transform root = vehicle.transform;
			int missing = 0;
			if (script == null)
			{
				return;
			}
			foreach (string item in snapshot.Split('|'))
			{
				if (item.Length < 3)
				{
					continue;
				}
				string[] v = item.Substring(2).Split(',');
				try
				{
					switch (item[0])
					{
					case 'F':
						EngineShop.SetField(script, v[0], float.Parse(v[1], NumberStyles.Float, Inv));
						break;
					case 'I':
						EngineShop.SetField(script, v[0], int.Parse(v[1], Inv));
						break;
					case 'Z':
						EngineShop.SetField(script, v[0], v[1] == "1");
						break;
					case 'T':
					{
						Transform t = HierarchyPath.Find(root, v[0]);
						if (t == null || (skip != null && skip(t)))
						{
							missing++;
							break;
						}
						t.gameObject.SetActive(v[1] == "1");
						Renderer r = t.GetComponent<Renderer>();
						if (r != null && v[2] != "-")
						{
							r.enabled = v[2] == "1";
						}
						BoxCollider[] boxes = t.GetComponents<BoxCollider>();
						for (int i = 0; i < boxes.Length && v[3] != "-" && i < v[3].Length; i++)
						{
							boxes[i].enabled = v[3][i] == '1';
						}
						break;
					}
					case 'D':
					{
						Transform t = HierarchyPath.Find(root, v[0]);
						durability d = t != null ? t.GetComponent<durability>() : null;
						if (d != null)
						{
							d.health = float.Parse(v[1], NumberStyles.Float, Inv);
							d.canDetach = v[2] == "1";
							d.boltStr = float.Parse(v[3], NumberStyles.Float, Inv);
						}
						break;
					}
					case 'B':
					{
						Transform t = HierarchyPath.Find(root, v[0]);
						BoltScript b = t != null ? t.GetComponent<BoltScript>() : null;
						if (b != null)
						{
							b.boltturns = int.Parse(v[1], Inv);
							SetPose(t, v, 2);
						}
						break;
					}
					case 'S':
					{
						Transform t = HierarchyPath.Find(root, v[0]);
						BoltScriptSusp b = t != null ? t.GetComponent<BoltScriptSusp>() : null;
						if (b != null)
						{
							b.boltTurns = int.Parse(v[1], Inv);
							SetPose(t, v, 2);
						}
						break;
					}
					}
				}
				catch (Exception)
				{
					missing++;
				}
			}
			if (missing > 0)
			{
				EngineClonerMod.Log(vehicle.name + ": " + missing + " saved part state(s) didn't match this copy and were skipped.");
			}
		}

		private static MonoBehaviour VehicleScript(GameObject vehicle)
		{
			Type type = VehicleCloner.VehicleType(vehicle);
			return type != null ? (MonoBehaviour)vehicle.GetComponent(type) : null;
		}

		private static string Pose(Transform t)
		{
			Vector3 p = t.localPosition;
			Quaternion q = t.localRotation;
			return string.Join(",", new[] { p.x, p.y, p.z, q.x, q.y, q.z, q.w }.Select(f => f.ToString("R", Inv)));
		}

		private static void SetPose(Transform t, string[] v, int at)
		{
			if (v.Length < at + 7)
			{
				return;
			}
			float[] f = new float[7];
			for (int i = 0; i < 7; i++)
			{
				f[i] = float.Parse(v[at + i], NumberStyles.Float, Inv);
			}
			t.localPosition = new Vector3(f[0], f[1], f[2]);
			t.localRotation = new Quaternion(f[3], f[4], f[5], f[6]);
		}
	}

	// On a vehicle copy another mod tagged, so saved engines find it again.
	public class VehicleTag : MonoBehaviour
	{
		public string id;
	}

	// A child's path by names, "cabin_low/cardoor_d", with "~n" for the nth of
	// several siblings of the same name. Unlike sibling indexes it survives an
	// engine being mounted (which moves the block to the end of the list).
	internal static class HierarchyPath
	{
		public static string Of(Transform root, Transform t)
		{
			List<string> parts = new List<string>();
			for (; t != null && t != root; t = t.parent)
			{
				int occurrence = 0;
				if (t.parent != null)
				{
					foreach (Transform sibling in t.parent)
					{
						if (sibling == t)
						{
							break;
						}
						if (sibling.name == t.name)
						{
							occurrence++;
						}
					}
				}
				parts.Insert(0, Escape(t.name) + (occurrence > 0 ? "~" + occurrence.ToString(CultureInfo.InvariantCulture) : ""));
			}
			return string.Join("/", parts.ToArray());
		}

		public static Transform Find(Transform root, string path)
		{
			Transform t = root;
			if (string.IsNullOrEmpty(path))
			{
				return t;
			}
			foreach (string part in path.Split('/'))
			{
				string name = part;
				int occurrence = 0;
				int tilde = part.LastIndexOf('~');
				if (tilde >= 0)
				{
					int.TryParse(part.Substring(tilde + 1), NumberStyles.Integer, CultureInfo.InvariantCulture, out occurrence);
					name = part.Substring(0, tilde);
				}
				name = Unescape(name);
				Transform next = null;
				foreach (Transform child in t)
				{
					if (child.name == name && occurrence-- == 0)
					{
						next = child;
						break;
					}
				}
				if (next == null)
				{
					return null;
				}
				t = next;
			}
			return t;
		}

		private static string Escape(string s)
		{
			StringBuilder sb = new StringBuilder();
			foreach (char c in s)
			{
				if (c == '%' || c == '/' || c == '~' || c == ',' || c == '|' || c == ';' || c == '\n' || c == '\r')
				{
					sb.Append('%').Append(((int)c).ToString("X2", CultureInfo.InvariantCulture));
				}
				else
				{
					sb.Append(c);
				}
			}
			return sb.ToString();
		}

		private static string Unescape(string s)
		{
			if (s.IndexOf('%') < 0)
			{
				return s;
			}
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] == '%' && i + 2 < s.Length)
				{
					sb.Append((char)int.Parse(s.Substring(i + 1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
					i += 2;
				}
				else
				{
					sb.Append(s[i]);
				}
			}
			return sb.ToString();
		}
	}
}
