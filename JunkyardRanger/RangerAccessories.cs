using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JunkyardRanger
{
	// The Ranger's accessories, fitted the game's own way: each has a slot on the
	// Ranger, like the game's winch or brush guard slots (a hidden part named
	// "..._e" with a solid box, a snap trigger, a durability and maybe bolts).
	// Carry the part to its place: the game shows its snap mark, let go and it
	// pops on; tighten any bolts with the ratchet. Loosen them, right-click it,
	// and the game hands back the loose part (the slot's durability.template).
	//
	//   Bull bar     the Diamondback's own brush guard slot, renamed so only the
	//                bull bar fits (it must start with "brus" for the game's snap
	//                code); its 4 frame bolts
	//   Rain guards  a slot on top of each door window, no bolts (they snap on);
	//                one loose rain guard fits either door
	//   Sun visor    a slot over the windshield, 4 bolts on the roof; body colored
	internal static class RangerAccessories
	{
		internal class Slot
		{
			public string name;       // the slot's name on a Ranger
			public string group;      // the OBJ group shown in it (in the parent's frame)
			public string parent;     // where it hangs, or null to take over gameSlot
			public string gameSlot;   // a slot the Diamondback already has
		}

		internal class Kind
		{
			public string id;         // saved, and the loose item's component
			public string shopName;
			public string itemName;   // the loose item's object name
			public string description;
			public string group;      // its look when loose
			public float mass;
			public int perPurchase;
			public Func<float> price;
			public string[] fits;     // slot names (attachTo, attachTo2)
		}

		public static readonly Slot[] Slots =
		{
			new Slot { name = "brushguardR_e", group = "BullBar", gameSlot = "brushguard_e" },
			new Slot { name = "rainguardD_e", group = "RainGuardD", parent = "cabin_low/cardoor_d" },
			new Slot { name = "rainguardP_e", group = "RainGuardP", parent = "cabin_low/cardoor_p" },
			new Slot { name = "sunvisor_e", group = "SunVisor", parent = "cabin_low" },
		};

		public static readonly Kind[] Kinds =
		{
			new Kind { id = "BullBar", shopName = "Ranger - Bull bar", itemName = "rangerbullbar", description = "Bull bar (2004 Ford Ranger)", group = "BullBar", mass = 28f, perPurchase = 1, price = () => RangerMod.BullBarPrice, fits = new[] { "brushguardR_e" } },
			new Kind { id = "RainGuard", shopName = "Ranger - Rain guards (pair)", itemName = "rangerrainguard", description = "Rain guard (2004 Ford Ranger, either door)", group = "RainGuardD", mass = 0.6f, perPurchase = 2, price = () => RangerMod.RainGuardPrice, fits = new[] { "rainguardD_e", "rainguardP_e" } },
			new Kind { id = "SunVisor", shopName = "Ranger - Sun visor", itemName = "rangersunvisor", description = "Sun visor (2004 Ford Ranger)", group = "SunVisor", mass = 9f, perPurchase = 1, price = () => RangerMod.SunVisorPrice, fits = new[] { "sunvisor_e" } },
		};

		public static bool IsAccessoryGroup(string group)
		{
			foreach (Slot s in Slots)
			{
				if (s.group == group)
				{
					return true;
				}
			}
			return false;
		}

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

		// --- On a Ranger. ---

		// On a new copy, before any snapshot is applied: every slot, empty.
		public static void FitSlots(GameObject copy, RangerBody body)
		{
			Transform gameBolt = null;
			foreach (Slot s in Slots)
			{
				Transform slot = s.gameSlot != null ? (copy.transform.Find(s.gameSlot) ?? copy.transform.Find(s.name)) : MakeSlot(copy, s);
				if (slot == null)
				{
					RangerMod.Log("The Diamondback has no " + (s.gameSlot ?? s.parent) + "; this Ranger can't take a " + s.group + ".");
					continue;
				}
				slot.name = s.name;
				if (gameBolt == null)
				{
					gameBolt = slot.Find("bolt");
				}
				Mesh mesh;
				Material[] materials;
				if (!RangerModel.Accessory(s.group, body, out mesh, out materials))
				{
					continue;
				}
				(slot.GetComponent<MeshFilter>() ?? slot.gameObject.AddComponent<MeshFilter>()).sharedMesh = mesh;
				MeshRenderer renderer = slot.GetComponent<MeshRenderer>();
				if (renderer != null)
				{
					renderer.sharedMaterials = materials;
				}
				Bounds b = mesh.bounds;
				foreach (BoxCollider box in slot.GetComponents<BoxCollider>())
				{
					box.center = b.center;
					// The trigger reaches a little past the part, so the snap mark
					// shows as you bring it up.
					box.size = box.isTrigger ? b.size + Vector3.one * 0.25f : b.size;
				}
				durability d = slot.GetComponent<durability>();
				if (d != null)
				{
					d.template = Template(KindFor(s.name));
				}
				List<Vector3> bolts = RangerModel.AccessoryBolts(s.group);
				if (s.gameSlot == null && bolts.Count > 0 && gameBolt != null)
				{
					AddBolts(copy, slot, bolts, gameBolt);
				}
				if (d != null)
				{
					d.numBolts = CountBolts(slot);
				}
			}
		}

		// A slot of our own: hidden, with its solid box (first, as the game expects),
		// its snap trigger and a durability.
		private static Transform MakeSlot(GameObject copy, Slot s)
		{
			Transform parent = copy.transform.Find(s.parent);
			if (parent == null)
			{
				return null;
			}
			Transform existing = parent.Find(s.name);
			if (existing != null)
			{
				return existing;
			}
			GameObject go = new GameObject(s.name);
			go.layer = 0;
			go.transform.SetParent(parent, false);
			go.AddComponent<MeshFilter>();
			go.AddComponent<MeshRenderer>().enabled = false;
			BoxCollider solid = go.AddComponent<BoxCollider>();
			solid.enabled = false;
			BoxCollider trigger = go.AddComponent<BoxCollider>();
			trigger.isTrigger = true;
			durability d = go.AddComponent<durability>();
			d.health = 0f;
			d.canDetach = false;
			return go.transform;
		}

		// Copies of the game's own bolt (from the brush guard slot), pointing into
		// the surface they hold the part to (here: down, into the roof).
		private static void AddBolts(GameObject copy, Transform slot, List<Vector3> seats, Transform gameBolt)
		{
			foreach (Vector3 seat in seats)
			{
				Transform bolt = Object.Instantiate(gameBolt.gameObject, slot, false).transform;
				bolt.name = "bolt";
				bolt.position = copy.transform.TransformPoint(seat);
				bolt.rotation = Quaternion.LookRotation(-copy.transform.up, copy.transform.forward);
				bolt.localScale = gameBolt.localScale;
				Hide(bolt);
			}
		}

		private static int CountBolts(Transform slot)
		{
			int n = 0;
			foreach (Transform child in slot)
			{
				if (child.name == "bolt")
				{
					n++;
				}
			}
			return n;
		}

		private static Kind KindFor(string slotName)
		{
			foreach (Kind k in Kinds)
			{
				if (Array.IndexOf(k.fits, slotName) >= 0)
				{
					return k;
				}
			}
			return null;
		}

		// A new Ranger comes without any (the Diamondback it's copied from may have
		// its brush guard on).
		public static void UnfitAll(GameObject copy)
		{
			foreach (Transform t in copy.GetComponentsInChildren<Transform>(true))
			{
				foreach (Slot s in Slots)
				{
					if (t.name == s.name)
					{
						Unfit(t);
					}
				}
			}
		}

		private static void Unfit(Transform slot)
		{
			Renderer renderer = slot.GetComponent<Renderer>();
			if (renderer != null)
			{
				renderer.enabled = false;
			}
			foreach (BoxCollider box in slot.GetComponents<BoxCollider>())
			{
				box.enabled = box.isTrigger;
			}
			durability d = slot.GetComponent<durability>();
			if (d != null)
			{
				d.health = 0f;
				d.canDetach = false;
			}
			foreach (Transform child in slot)
			{
				if (child.name == "bolt")
				{
					Hide(child);
				}
			}
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

		// --- Loose parts. ---

		private static GameObject holder;
		private static readonly Dictionary<string, GameObject> templates = new Dictionary<string, GameObject>();
		private static readonly Dictionary<string, GameObject> markers = new Dictionary<string, GameObject>();

		// A dormant loose part (under an inactive holder); the shops and the game's
		// part removal make live copies of it. Null without the body files.
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
			string why;
			if (!RangerModel.Load(out why) || !RangerModel.Accessory(kind.group, null, out mesh, out materials))
			{
				return null;
			}
			GameObject go = new GameObject(kind.itemName);
			go.transform.SetParent(Holder().transform, false);
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
			pick.attachTo = kind.fits[0];
			if (kind.fits.Length > 1)
			{
				pick.attachTo2 = kind.fits[1];
			}
			pick.pickable = true;
			pick.price = 0f;
			pick.thisDurability = 100f;
			pick.trueMass = kind.mass;
			pick.description = kind.description;
			go.AddComponent<RangerAccessoryItem>().kind = kind.id;
			templates[kind.id] = go;
			return go;
		}

		// What a shop sells: the part itself, or for a pair, a marker that drops two.
		public static GameObject ShopPrefab(Kind kind)
		{
			if (kind.perPurchase <= 1)
			{
				return Template(kind);
			}
			GameObject marker;
			if (markers.TryGetValue(kind.id, out marker) && marker != null)
			{
				return marker;
			}
			if (Template(kind) == null)
			{
				return null;
			}
			marker = new GameObject(kind.itemName + " delivery");
			marker.transform.SetParent(Holder().transform, false);
			RangerAccessoryDelivery d = marker.AddComponent<RangerAccessoryDelivery>();
			d.kind = kind.id;
			d.count = kind.perPurchase;
			markers[kind.id] = marker;
			return marker;
		}

		private static GameObject Holder()
		{
			if (holder == null)
			{
				holder = new GameObject("JunkyardRanger_Accessories");
				holder.SetActive(false);
				Object.DontDestroyOnLoad(holder);
			}
			return holder;
		}

		// A loose part in the world (bought, or as saved).
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

		// A new scene: new templates, with the materials of what's loaded now.
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
			foreach (GameObject go in markers.Values)
			{
				if (go != null)
				{
					Object.Destroy(go);
				}
			}
			markers.Clear();
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

	// Marks a loose accessory (copied along with it), so it's saved with the Rangers.
	public class RangerAccessoryItem : MonoBehaviour
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

	// What a shop delivers for a pair: drops that many loose parts where it lands.
	public class RangerAccessoryDelivery : MonoBehaviour
	{
		public string kind;
		public int count = 1;

		private void Start()
		{
			for (int i = 0; i < count; i++)
			{
				RangerAccessories.Spawn(kind, transform.position + transform.right * (0.35f * i), transform.rotation, 100f);
			}
			Destroy(gameObject);
		}
	}
}
