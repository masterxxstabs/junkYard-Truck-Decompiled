using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using EngineCloner;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Duramax
{
	// Bolt-on accessories for the Duramax, fitted the game's own way. Each has a
	// slot on the engine like the game's part slots (a hidden part named "..._e"
	// with a solid box, a snap trigger, a durability and its bolts), on the part
	// it mounts to, so it only goes on while that part is on. Carry one there: the
	// game shows its snap mark; let go and it pops on; tighten its bolts with the
	// ratchet. Undo them, right-click it, and the game hands back the loose part.
	// A fitted accessory adds to the Duramax's pull (DuramaxPowerPatch).
	//
	//   Ram intake horn  polished billet elbow on the Y-bridge (on the intake), 4 bolts
	//   Turbo blanket    heat blanket round the turbine (on the turbo), snaps on
	//   Lift pump kit    lift pump and filters on the block, 4 bolts
	//
	// Their looks are groups of the Duramax's model (acc_*), and their bolt seats
	// are in models/duramax_acc.json (both made by tools/duramax_model.py).
	// Loose ones are saved in Engine Cloner's save files (EngineApi.Saving).
	internal static class DuramaxAccessories
	{
		internal class Kind
		{
			public string id;          // saved, and the loose item's component
			public string slot;        // the slot's name on the engine
			public string group;       // its look in the model
			public string shopName;
			public string itemName;    // the loose item's object name
			public string description;
			public float mass;
			public float power;        // what it multiplies the torque by
			public Func<float> price;
		}

		public static readonly Kind[] Kinds =
		{
			new Kind { id = "RamHorn", slot = "dmxramhorn_e", group = "acc_ramhorn", shopName = "Duramax - Ram intake horn (billet)", itemName = "duramaxramhorn", description = "Ram intake horn, billet (6.6L Duramax: bolts to the intake's Y-bridge)", mass = 2.5f, power = 1.06f, price = () => DuramaxMod.RamHornPrice },
			new Kind { id = "TurboBlanket", slot = "dmxblanket_e", group = "acc_turboblanket", shopName = "Duramax - Turbo blanket", itemName = "duramaxturboblanket", description = "Turbo blanket (6.6L Duramax: wraps the turbine)", mass = 0.8f, power = 1.03f, price = () => DuramaxMod.TurboBlanketPrice },
			new Kind { id = "LiftPump", slot = "dmxliftpump_e", group = "acc_liftpump", shopName = "Duramax - Lift pump & filter kit", itemName = "duramaxliftpump", description = "Lift pump and filter kit (6.6L Duramax: bolts to the driver's side of the block)", mass = 6f, power = 1.06f, price = () => DuramaxMod.LiftPumpPrice },
		};

		private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

		public static Kind KindById(string id)
		{
			foreach (Kind k in Kinds)
			{
				if (k.id == id)
				{
					return k;
				}
			}
			return null;
		}

		// --- Where they go (models/duramax_acc.json). ---

		private class Mount
		{
			public string parent;
			public List<Vector3[]> bolts = new List<Vector3[]>();   // seat, direction into the part
		}

		private static Dictionary<string, Mount> mounts;

		private static bool LoadMounts()
		{
			if (mounts != null)
			{
				return mounts.Count > 0;
			}
			mounts = new Dictionary<string, Mount>();
			string path = Path.Combine(Path.GetDirectoryName(DuramaxModel.ModelPath), "duramax_acc.json");
			if (!File.Exists(path))
			{
				return false;
			}
			try
			{
				Dictionary<string, object> root = MiniJson.Parse(File.ReadAllText(path)) as Dictionary<string, object>;
				foreach (KeyValuePair<string, object> e in root)
				{
					Dictionary<string, object> m = e.Value as Dictionary<string, object>;
					if (m == null)
					{
						continue;
					}
					Mount mount = new Mount { parent = m["parent"] as string };
					foreach (object b in (m["bolts"] as List<object>) ?? new List<object>())
					{
						List<object> v = (List<object>)b;
						mount.bolts.Add(new[]
						{
							new Vector3(Num(v[0]), Num(v[1]), Num(v[2])),
							new Vector3(Num(v[3]), Num(v[4]), Num(v[5])).normalized
						});
					}
					mounts[e.Key] = mount;
				}
			}
			catch (Exception ex)
			{
				DuramaxMod.Log("Couldn't read " + path + ": " + ex.Message);
			}
			return mounts.Count > 0;
		}

		private static float Num(object o)
		{
			return Convert.ToSingle(o, Inv);
		}

		// --- On the engine. ---

		// On the Duramax's shop template (every copy is made from it): every slot,
		// empty, added last under its part so saved paths of the other parts stay
		// the same.
		public static void FitSlots(GameObject block)
		{
			if (!LoadMounts())
			{
				return;
			}
			Transform gameBolt = null;
			foreach (Transform child in block.transform)
			{
				if (child.name == "bolt" && child.GetComponent<BoltScript>() != null)
				{
					gameBolt = child;
					break;
				}
			}
			int made = 0;
			foreach (Kind k in Kinds)
			{
				Mesh mesh;
				Material[] materials;
				Mount mount;
				if (!mounts.TryGetValue(k.group, out mount) || !DuramaxModel.Piece(k.group, out mesh, out materials))
				{
					continue;
				}
				Transform parent = mount.parent == "v8_block" ? block.transform : FindDeep(block.transform, mount.parent);
				if (parent == null || parent.Find(k.slot) != null)
				{
					continue;
				}
				GameObject go = new GameObject(k.slot);
				go.layer = parent.gameObject.layer;
				go.transform.SetParent(parent, false);
				go.AddComponent<MeshFilter>().sharedMesh = mesh;
				MeshRenderer renderer = go.AddComponent<MeshRenderer>();
				renderer.sharedMaterials = materials;
				renderer.enabled = false;
				Bounds b = mesh.bounds;
				// The solid box first (what you aim at to take it off), then the
				// trigger it snaps with: a little bigger, so the snap mark shows as
				// you bring the part up; only on while you carry one (UpdateTriggers).
				BoxCollider solid = go.AddComponent<BoxCollider>();
				solid.center = b.center;
				solid.size = Vector3.Max(b.size, Vector3.one * 0.02f);
				solid.enabled = false;
				BoxCollider trigger = go.AddComponent<BoxCollider>();
				trigger.isTrigger = true;
				trigger.center = b.center;
				trigger.size = b.size + Vector3.one * 0.12f;
				trigger.enabled = false;
				durability d = go.AddComponent<durability>();
				d.health = 0f;
				d.canDetach = false;
				d.template = Template(k);
				int bolts = 0;
				if (gameBolt != null)
				{
					foreach (Vector3[] seat in mount.bolts)
					{
						Transform bolt = Object.Instantiate(gameBolt.gameObject, go.transform, false).transform;
						bolt.name = "bolt";
						Vector3 into = seat[1];
						Vector3 up = Mathf.Abs(Vector3.Dot(into, Vector3.up)) < 0.9f ? Vector3.up : Vector3.forward;
						bolt.localRotation = Quaternion.LookRotation(into, up);
						// Undone: ten turns (2 cm) out of its seat; tightening drives it in.
						bolt.localPosition = seat[0] - into * 0.02f;
						Vector3 want = gameBolt.lossyScale;
						Vector3 have = go.transform.lossyScale;
						bolt.localScale = new Vector3(want.x / have.x, want.y / have.y, want.z / have.z);
						Hide(bolt);
						bolts++;
					}
				}
				d.numBolts = bolts;
				made++;
			}
			if (made > 0)
			{
				DuramaxMod.Log("6.6L Duramax: " + made + " accessory slot(s).");
			}
		}

		private static Transform FindDeep(Transform root, string name)
		{
			foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
			{
				if (t.name == name)
				{
					return t;
				}
			}
			return null;
		}

		private static void Hide(Transform bolt)
		{
			Renderer r = bolt.GetComponent<Renderer>();
			if (r != null)
			{
				r.enabled = false;
			}
			Collider c = bolt.GetComponent<Collider>();
			if (c != null)
			{
				c.enabled = false;
			}
			BoltScript b = bolt.GetComponent<BoltScript>();
			if (b != null)
			{
				b.boltturns = 0;
			}
		}

		// How much harder an engine pulls for the accessories on it.
		public static float PowerOf(GameObject block)
		{
			float power = 1f;
			foreach (Kind k in Kinds)
			{
				Transform slot = FindDeep(block.transform, k.slot);
				Renderer r = slot != null ? slot.GetComponent<Renderer>() : null;
				if (r != null && r.enabled)
				{
					power *= k.power;
				}
			}
			return power;
		}

		// Slot triggers are on only while you carry a loose accessory, and only on
		// the empty slots it fits (else they'd be in the way of everything you look
		// at round the engine).
		private static readonly List<BoxCollider> armed = new List<BoxCollider>();
		private static float nextCheck;

		public static void UpdateTriggers()
		{
			if (Time.time < nextCheck)
			{
				return;
			}
			nextCheck = Time.time + 0.2f;
			Kind held = null;
			foreach (DuramaxAccessoryItem item in Object.FindObjectsOfType<DuramaxAccessoryItem>())
			{
				PickUp pick = item.GetComponent<PickUp>();
				if (pick != null && pick.holding)
				{
					held = KindById(item.kind);
					break;
				}
			}
			List<BoxCollider> on = new List<BoxCollider>();
			if (held != null)
			{
				foreach (ShopEngine e in Object.FindObjectsOfType<ShopEngine>())
				{
					if (e.kind != "duramax")
					{
						continue;
					}
					Transform slot = FindDeep(e.transform, held.slot);
					Renderer r = slot != null ? slot.GetComponent<Renderer>() : null;
					if (r == null || r.enabled)
					{
						continue;
					}
					foreach (BoxCollider box in slot.GetComponents<BoxCollider>())
					{
						if (box.isTrigger)
						{
							on.Add(box);
						}
					}
				}
			}
			foreach (BoxCollider box in armed)
			{
				if (box != null && !on.Contains(box))
				{
					box.enabled = false;
				}
			}
			foreach (BoxCollider box in on)
			{
				box.enabled = true;
			}
			armed.Clear();
			armed.AddRange(on);
		}

		// --- Loose parts. ---

		private static GameObject holder;
		private static readonly Dictionary<string, GameObject> templates = new Dictionary<string, GameObject>();

		// A dormant loose part (under an inactive holder): the shops and the game's
		// part removal make live copies of it. Null without the model.
		public static GameObject Template(Kind kind)
		{
			GameObject template;
			if (kind == null)
			{
				return null;
			}
			if (templates.TryGetValue(kind.id, out template) && template != null)
			{
				return template;
			}
			Mesh mesh;
			Material[] materials;
			if (!LoadMounts() || !DuramaxModel.Piece(kind.group, out mesh, out materials))
			{
				return null;
			}
			if (holder == null)
			{
				holder = new GameObject("Duramax_Accessories");
				holder.SetActive(false);
				Object.DontDestroyOnLoad(holder);
			}
			GameObject go = new GameObject(kind.itemName);
			go.transform.SetParent(holder.transform, false);
			go.AddComponent<MeshFilter>().sharedMesh = mesh;
			go.AddComponent<MeshRenderer>().sharedMaterials = materials;
			Bounds b = mesh.bounds;
			// PickUp wants the solid box first, then the trigger it snaps with.
			BoxCollider solid = go.AddComponent<BoxCollider>();
			solid.center = b.center;
			solid.size = Vector3.Max(b.size, Vector3.one * 0.03f);
			BoxCollider trigger = go.AddComponent<BoxCollider>();
			trigger.center = b.center;
			trigger.size = Vector3.Max(b.size, Vector3.one * 0.03f);
			trigger.isTrigger = true;
			Rigidbody body = go.AddComponent<Rigidbody>();
			body.mass = kind.mass;
			body.interpolation = RigidbodyInterpolation.Interpolate;
			PickUp pick = go.AddComponent<PickUp>();
			InitLikeUnity(pick);
			pick.attachTo = kind.slot;
			pick.pickable = true;
			pick.price = 0f;
			pick.thisDurability = 100f;
			pick.trueMass = kind.mass;
			pick.description = kind.description;
			go.AddComponent<DuramaxAccessoryItem>().kind = kind.id;
			templates[kind.id] = go;
			return go;
		}

		// What the shops sell (the shared shop list: JunkAuto, and the Parts Store
		// through EngineShop).
		public static object[][] ShopItems()
		{
			List<object[]> items = new List<object[]>();
			foreach (Kind k in Kinds)
			{
				GameObject t = Template(k);
				if (t != null)
				{
					items.Add(new object[] { t, k.shopName, k.price(), "Truck Part" });
				}
			}
			return items.ToArray();
		}

		public static GameObject Spawn(string kindId, Vector3 position, Quaternion rotation, float health)
		{
			Kind kind = KindById(kindId);
			GameObject t = Template(kind);
			if (t == null)
			{
				return null;
			}
			GameObject go = Object.Instantiate(t, position, rotation);
			go.name = kind.itemName;
			go.GetComponent<PickUp>().thisDurability = health;
			return go;
		}

		// Loose accessories in the world, saved with the engines:
		//   DMXACC;kind;px;py;pz;rx;ry;rz;rw;health
		public static void SaveLines(StringBuilder sb)
		{
			int n = 0;
			foreach (DuramaxAccessoryItem item in Object.FindObjectsOfType<DuramaxAccessoryItem>())
			{
				Transform t = item.transform;
				PickUp pick = item.GetComponent<PickUp>();
				sb.Append(string.Join(";", new[]
				{
					"DMXACC", item.kind, F(t.position.x), F(t.position.y), F(t.position.z), F(t.rotation.x), F(t.rotation.y), F(t.rotation.z), F(t.rotation.w), F(pick != null ? pick.thisDurability : 100f)
				})).Append('\n');
				n++;
			}
			if (n > 0)
			{
				DuramaxMod.Log("Saved " + n + " loose accessor" + (n == 1 ? "y" : "ies") + ".");
			}
		}

		public static bool LoadLine(string[] f)
		{
			if (f.Length < 10 || f[0] != "DMXACC")
			{
				return false;
			}
			Spawn(f[1], new Vector3(P(f[2]), P(f[3]), P(f[4])), new Quaternion(P(f[5]), P(f[6]), P(f[7]), P(f[8])), P(f[9]));
			return true;
		}

		private static string F(float v)
		{
			return v.ToString("R", Inv);
		}

		private static float P(string s)
		{
			return float.Parse(s, NumberStyles.Float, Inv);
		}

		// A new level: the old one's loose templates go (the engines' templates are
		// remade too, and point at the new ones).
		public static void Reset()
		{
			foreach (GameObject go in templates.Values)
			{
				if (go != null)
				{
					Object.Destroy(go);
				}
			}
			templates.Clear();
			armed.Clear();
		}

		// A component added from code starts with null strings and arrays; one
		// loaded with a scene has "" and empty arrays, which PickUp expects (it
		// checks attachTo2 != "" and so on).
		private static void InitLikeUnity(Component component)
		{
			for (Type type = component.GetType(); type != null && type != typeof(MonoBehaviour); type = type.BaseType)
			{
				foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
				{
					bool serialized = field.IsPublic && !field.IsNotSerialized || field.IsDefined(typeof(SerializeField), false);
					if (!serialized || field.GetValue(component) != null)
					{
						continue;
					}
					Type fieldType = field.FieldType;
					if (fieldType == typeof(string))
					{
						field.SetValue(component, "");
					}
					else if (fieldType.IsArray)
					{
						field.SetValue(component, Array.CreateInstance(fieldType.GetElementType(), 0));
					}
					else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
					{
						field.SetValue(component, Activator.CreateInstance(fieldType));
					}
				}
			}
		}
	}

	// Marks a loose Duramax accessory (copied along with it), so it's saved.
	public class DuramaxAccessoryItem : MonoBehaviour
	{
		public string kind;

		// Shops drop items at the player's feet: lift it clear of the ground once.
		private void Start()
		{
			if (transform.parent != null)
			{
				return;
			}
			float lowest = float.MinValue;
			foreach (RaycastHit hit in Physics.RaycastAll(transform.position + Vector3.up * 2f, Vector3.down, 6f, ~0, QueryTriggerInteraction.Ignore))
			{
				if (!hit.transform.IsChildOf(transform) && hit.point.y > lowest && hit.collider.GetComponentInParent<CharacterController>() == null)
				{
					lowest = hit.point.y;
				}
			}
			Renderer r = GetComponent<Renderer>();
			if (lowest > float.MinValue && r != null && r.bounds.min.y < lowest + 0.05f)
			{
				transform.position += Vector3.up * (lowest + 0.05f - r.bounds.min.y);
			}
		}
	}
}
