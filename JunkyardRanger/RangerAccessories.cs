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
	//   Light bar    on the roof behind the visor, 4 bolts; two work lights,
	//                switched with the light bar key
	//   Headache rack  on the bed rails behind the cab, 4 bolts
	//   Side steps   under each door, 2 bolts up into the rocker (pair)
	//   Snorkel      up the passenger's windshield pillar, 2 bolts in the fender
	//   Exhaust      a performance exhaust under the bed, 2 hanger bolts: the
	//                truck pulls harder with it (RangerUpgrades)
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
			// 1.7: added after the others, so saved paths of the old ones stay the same.
			new Slot { name = "lightbar_e", group = "LightBar", parent = "cabin_low" },
			new Slot { name = "headrack_e", group = "HeadacheRack", parent = "body_low" },
			new Slot { name = "sidestepD_e", group = "SideStepD", parent = "cabin_low" },
			new Slot { name = "sidestepP_e", group = "SideStepP", parent = "cabin_low" },
			new Slot { name = "snorkel_e", group = "Snorkel", parent = "cabin_low" },
			new Slot { name = "rexhaust_e", group = "Exhaust", parent = "body_low" },
		};

		public static readonly Kind[] Kinds =
		{
			new Kind { id = "BullBar", shopName = "Ranger - Bull bar", itemName = "rangerbullbar", description = "Bull bar (2004 Ford Ranger)", group = "BullBar", mass = 28f, perPurchase = 1, price = () => RangerMod.BullBarPrice, fits = new[] { "brushguardR_e" } },
			new Kind { id = "RainGuard", shopName = "Ranger - Rain guards (pair)", itemName = "rangerrainguard", description = "Rain guard (2004 Ford Ranger, either door)", group = "RainGuardD", mass = 0.6f, perPurchase = 2, price = () => RangerMod.RainGuardPrice, fits = new[] { "rainguardD_e", "rainguardP_e" } },
			new Kind { id = "SunVisor", shopName = "Ranger - Sun visor", itemName = "rangersunvisor", description = "Sun visor (2004 Ford Ranger)", group = "SunVisor", mass = 9f, perPurchase = 1, price = () => RangerMod.SunVisorPrice, fits = new[] { "sunvisor_e" } },
			new Kind { id = "LightBar", shopName = "Ranger - Roof light bar", itemName = "rangerlightbar", description = "LED roof light bar (2004 Ford Ranger; switch it with the light bar key)", group = "LightBar", mass = 5f, perPurchase = 1, price = () => RangerMod.LightBarPrice, fits = new[] { "lightbar_e" } },
			new Kind { id = "HeadacheRack", shopName = "Ranger - Headache rack", itemName = "rangerheadacherack", description = "Headache rack (2004 Ford Ranger, on the bed rails)", group = "HeadacheRack", mass = 22f, perPurchase = 1, price = () => RangerMod.HeadacheRackPrice, fits = new[] { "headrack_e" } },
			new Kind { id = "SideStep", shopName = "Ranger - Side steps (pair)", itemName = "rangersidestep", description = "Side step (2004 Ford Ranger, either side)", group = "SideStepD", mass = 9f, perPurchase = 2, price = () => RangerMod.SideStepPrice, fits = new[] { "sidestepD_e", "sidestepP_e" } },
			new Kind { id = "Snorkel", shopName = "Ranger - Snorkel", itemName = "rangersnorkel", description = "Snorkel (2004 Ford Ranger, passenger's side)", group = "Snorkel", mass = 4f, perPurchase = 1, price = () => RangerMod.SnorkelPrice, fits = new[] { "snorkel_e" } },
			new Kind { id = "Exhaust", shopName = "Ranger - Performance exhaust", itemName = "rangerexhaust", description = "Performance exhaust (2004 Ford Ranger: the truck pulls 8% harder)", group = "Exhaust", mass = 14f, perPurchase = 1, price = () => RangerMod.ExhaustPrice, fits = new[] { "rexhaust_e" } },
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
				Bounds grip = GripBounds(mesh);
				foreach (BoxCollider box in slot.GetComponents<BoxCollider>())
				{
					// The trigger reaches a little past the part, so the snap mark
					// shows as you bring it up; it's only on while you carry a part
					// that fits (see UpdateTriggers), or it would be in the way of
					// everything you look at (the driver's seat, through the door).
					// The solid box is what you aim at to take the part off: for a
					// rain guard, just its top (its whole bounding box would fill the
					// top of the window).
					box.center = box.isTrigger ? b.center : grip.center;
					box.size = box.isTrigger ? b.size + Vector3.one * 0.25f : grip.size;
					if (box.isTrigger)
					{
						box.enabled = false;
					}
				}
				Register(slot);
				durability d = slot.GetComponent<durability>();
				if (d != null)
				{
					d.template = Template(KindFor(s.name));
				}
				List<Vector3> bolts = RangerModel.AccessoryBolts(s.group);
				if (s.gameSlot == null && bolts.Count > 0 && gameBolt != null && CountBolts(slot) == 0)
				{
					AddBolts(copy, slot, bolts, RangerModel.AccessoryBoltDirections(s.group), gameBolt);
				}
				AddLamps(copy, slot, RangerModel.AccessoryLights(s.group));
				if (d != null)
				{
					d.numBolts = CountBolts(slot);
				}
			}
		}

		// The part's top: what's above the lowest of its highest 8 cm (the bounds
		// of the part for anything flat; the strip along the roof for a rain guard).
		private static Bounds GripBounds(Mesh mesh)
		{
			Bounds all = mesh.bounds;
			if (all.size.y < 0.25f)
			{
				return all;
			}
			Vector3[] v = mesh.vertices;
			float top = all.max.y - 0.08f;
			Bounds b = new Bounds();
			bool any = false;
			foreach (Vector3 p in v)
			{
				if (p.y < top)
				{
					continue;
				}
				if (!any)
				{
					b = new Bounds(p, Vector3.zero);
					any = true;
				}
				else
				{
					b.Encapsulate(p);
				}
			}
			return any ? b : all;
		}

		// Every slot on every Ranger, for UpdateTriggers.
		private static readonly List<Transform> slots = new List<Transform>();

		private static void Register(Transform slot)
		{
			if (!slots.Contains(slot))
			{
				slots.Add(slot);
			}
		}

		private static float nextTriggerCheck;

		// Slot triggers are on only while you carry a loose accessory, and only on
		// the empty slots it fits.
		public static void UpdateTriggers()
		{
			if (Time.time < nextTriggerCheck)
			{
				return;
			}
			nextTriggerCheck = Time.time + 0.2f;
			slots.RemoveAll(t => t == null);
			Kind held = null;
			foreach (RangerAccessoryItem item in Object.FindObjectsOfType<RangerAccessoryItem>())
			{
				PickUp pick = item.GetComponent<PickUp>();
				if (pick != null && pick.holding)
				{
					held = KindById(item.kind);
					break;
				}
			}
			foreach (Transform slot in slots)
			{
				Renderer r = slot.GetComponent<Renderer>();
				bool on = held != null && Array.IndexOf(held.fits, slot.name) >= 0 && (r == null || !r.enabled);
				foreach (BoxCollider box in slot.GetComponents<BoxCollider>())
				{
					if (box.isTrigger && box.enabled != on)
					{
						box.enabled = on;
					}
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
			trigger.enabled = false;
			durability d = go.AddComponent<durability>();
			d.health = 0f;
			d.canDetach = false;
			return go.transform;
		}

		// Copies of the game's own bolt (from the brush guard slot), pointing into
		// the surface they hold the part to (down into the roof, unless the model
		// says which way).
		private static void AddBolts(GameObject copy, Transform slot, List<Vector3> seats, List<Vector3> directions, Transform gameBolt)
		{
			for (int i = 0; i < seats.Count; i++)
			{
				Transform bolt = Object.Instantiate(gameBolt.gameObject, slot, false).transform;
				bolt.name = "bolt";
				bolt.position = copy.transform.TransformPoint(seats[i]);
				Vector3 into = i < directions.Count ? copy.transform.TransformDirection(directions[i]) : -copy.transform.up;
				Vector3 up = Mathf.Abs(Vector3.Dot(into, copy.transform.up)) < 0.9f ? copy.transform.up : copy.transform.forward;
				bolt.rotation = Quaternion.LookRotation(into, up);
				bolt.localScale = gameBolt.localScale;
				Hide(bolt);
			}
		}

		// The light bar's work lamps: spot lights, off until the bar is on the
		// truck and switched on (RangerUpgrades.UpdateLights).
		private static void AddLamps(GameObject copy, Transform slot, List<KeyValuePair<Vector3, Vector3>> lamps)
		{
			foreach (Transform child in slot)
			{
				if (child.name == LampName)
				{
					return;
				}
			}
			foreach (KeyValuePair<Vector3, Vector3> lamp in lamps)
			{
				GameObject go = new GameObject(LampName);
				go.transform.SetParent(slot, false);
				go.transform.position = copy.transform.TransformPoint(lamp.Key);
				go.transform.rotation = Quaternion.LookRotation(copy.transform.TransformDirection(lamp.Value), copy.transform.up);
				Light light = go.AddComponent<Light>();
				light.type = LightType.Spot;
				light.range = 45f;
				light.spotAngle = 75f;
				light.intensity = 2.6f;
				light.color = new Color(0.95f, 0.97f, 1f);
				light.shadows = LightShadows.None;
				light.enabled = false;
			}
		}

		public const string LampName = "lightbarlamp";

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
				box.enabled = false;
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
