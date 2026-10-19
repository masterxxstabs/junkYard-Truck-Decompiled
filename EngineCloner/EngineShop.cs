using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace EngineCloner
{
	// New engines for sale: the I4 (the truck's engineblock), the V8 (v8_block) and
	// the 2J inline-6 (i6block). Sold in the Parts Store (ComputerPartStore) and in
	// every shop that reads the mods' shared shop list (JunkAuto).
	//
	// A bought engine is a new bare block by default: a copy of an engine of that
	// type in the level with every part taken off (the game's own detach steps),
	// mounting bolts out, dry. With "New engines: bare block" off it's instead a
	// factory-fresh copy of the most complete one, every part on it new. Only parts inside
	// the block are touched: the engine scripts also point at parts that live on
	// the truck (battery, radiator, transmission...), which stay as they are.
	internal static class EngineShop
	{
		internal class Kind
		{
			public string id;
			public string name;
			public string blockName;
			public Type script;
		}

		public static readonly Kind[] Kinds =
		{
			new Kind { id = "i4", name = "Engine - I4 (new)", blockName = "engineblock", script = typeof(engine) },
			new Kind { id = "v8", name = "Engine - V8 (new)", blockName = "v8_block", script = typeof(enginev8) },
			new Kind { id = "i6", name = "Engine - 2J inline-6 (new)", blockName = "i6block", script = typeof(enginei6) },
		};

		private const string Category = "Truck Part";

		private static GameObject holder;
		private static readonly Dictionary<string, GameObject> templates = new Dictionary<string, GameObject>();
		private static readonly HashSet<string> noDonor = new HashSet<string>();

		public static Kind KindOf(string id)
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

		public static Kind KindOf(GameObject block)
		{
			foreach (Kind k in Kinds)
			{
				if (block.GetComponent(k.script) != null)
				{
					return k;
				}
			}
			return null;
		}

		public static void Reset()
		{
			holder = null;
			templates.Clear();
			noDonor.Clear();
		}

		public static float Price(Kind k)
		{
			return EngineClonerMod.PriceOf(k.id);
		}

		// The dormant engine shops sell (and saves restore from), made once per level.
		public static GameObject Template(Kind k)
		{
			return Template(k, EngineClonerMod.BareBlocks);
		}

		// Bare (every part off) or complete (every part on, new); both are kept,
		// so other mods can ask for a complete one whatever the setting.
		public static GameObject Template(Kind k, bool bare)
		{
			GameObject template;
			string key = k.id + (bare ? "|bare" : "|complete");
			if (templates.TryGetValue(key, out template) && template != null)
			{
				return template;
			}
			GameObject donor = BestDonor(k);
			if (donor == null)
			{
				if (noDonor.Add(k.id))
				{
					EngineClonerMod.Log("No " + k.blockName + " in this level to build a new one from; the " + k.name + " isn't for sale here.");
				}
				return null;
			}
			if (holder == null)
			{
				holder = new GameObject("EngineShop_Templates");
				holder.SetActive(false);
				// In the level, like the engines it copies (they point at level objects).
				SceneManager.MoveGameObjectToScene(holder, donor.scene);
			}
			template = Object.Instantiate(donor, holder.transform, false);
			template.name = k.blockName;
			StripGameSave(template);
			Freshen(template, k, bare);
			ShopEngine marker = template.AddComponent<ShopEngine>();
			marker.kind = k.id;
			marker.RecordMountBolts();
			templates[key] = template;
			EngineClonerMod.Log("New " + k.blockName + " ready (" + (bare ? "bare block" : "complete, from the most complete one in the level") + ").");
			return template;
		}

		// The game's own save tracking (Easy Save's ES3AutoSave) would make a copy
		// save over the original's entry; ours save on their own (EngineSave).
		public static void StripGameSave(GameObject root)
		{
			foreach (Component c in root.GetComponentsInChildren<Component>(true))
			{
				if (c != null && c.GetType().Name.StartsWith("ES3AutoSave", StringComparison.Ordinal))
				{
					Object.DestroyImmediate(c);
				}
			}
		}

		// The engine of this type with the most parts on it (not one we made).
		private static GameObject BestDonor(Kind k)
		{
			GameObject best = null;
			int bestCount = -1;
			foreach (Object o in Object.FindObjectsOfType(k.script))
			{
				GameObject go = ((Component)o).gameObject;
				if (!EngineClonerMod.IsEngineBlock(go) || go.GetComponent<ShopEngine>() != null)
				{
					continue;
				}
				int count = 0;
				foreach (durability d in InBlockParts(go, (Component)o))
				{
					Renderer r = d.GetComponent<Renderer>();
					if (r != null && r.enabled)
					{
						count++;
					}
				}
				if (count > bestCount)
				{
					best = go;
					bestCount = count;
				}
			}
			return best;
		}

		// The engine script's parts (its *_cnd_c durability fields) inside the block.
		public static List<durability> InBlockParts(GameObject block, Component script)
		{
			List<durability> parts = new List<durability>();
			foreach (FieldInfo f in script.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (!f.Name.EndsWith("_cnd_c", StringComparison.Ordinal) || !typeof(durability).IsAssignableFrom(f.FieldType))
				{
					continue;
				}
				durability d = f.GetValue(script) as durability;
				if (d != null && d.transform.IsChildOf(block.transform) && !parts.Contains(d))
				{
					parts.Add(d);
				}
			}
			return parts;
		}

		private static void Freshen(GameObject block, Kind k, bool bare)
		{
			Component script = block.GetComponent(k.script);
			if (bare)
			{
				Strip(block, script);
			}
			foreach (FieldInfo f in script.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (!f.Name.EndsWith("_cnd_c", StringComparison.Ordinal) || !typeof(durability).IsAssignableFrom(f.FieldType))
				{
					continue;
				}
				durability d = f.GetValue(script) as durability;
				if (d == null || !d.transform.IsChildOf(block.transform))
				{
					continue; // on the truck, not the engine
				}
				Renderer r = d.GetComponent<Renderer>();
				if (r == null || !r.enabled)
				{
					continue; // not fitted on the donor: stays off
				}
				d.health = 100f;
				SetField(script, f.Name.Substring(0, f.Name.Length - 2), 100f);
				foreach (Material m in r.materials)
				{
					if (m != null && m.HasProperty("_RustIntensity"))
					{
						m.SetFloat("_RustIntensity", 0f);
					}
				}
			}
			// The block casting itself (the I4 has no condition number for it): new metal.
			foreach (Renderer r in new[] { block.GetComponent<Renderer>() })
			{
				if (r == null)
				{
					continue;
				}
				foreach (Material m in r.materials)
				{
					if (m != null && m.HasProperty("_RustIntensity"))
					{
						m.SetFloat("_RustIntensity", 0f);
					}
				}
			}
			SetField(script, "newOilLevel", EngineClonerMod.ShipWithOil ? 100f : 0f);
			SetField(script, "oilLoss", 0f);
			SetField(script, "fire", false);
			PickUp pick = block.GetComponent<PickUp>();
			if (pick != null)
			{
				pick.pickable = true;
			}
		}

		// Children of a part slot the game leaves alone when it takes the part off.
		private static readonly HashSet<string> KeepColliders = new HashSet<string>
		{
			"rocker", "WHEEL_HOLDER", "Primary Drive Gear", "distwire", "OilDrain", "OilDrainV8", "OilDrainI6", "laminarOil", "laminarOilV8",
			"origin", "altfan", "turbine", "250_kickstarter", "250_cam", "250_gear2", "Plane001.003", "universalpulley", "brokenTurbo", "turbotach", "buddyscreen"
		};

		private static readonly HashSet<string> HideWithPart = new HashSet<string>
		{
			"bolt", "oilcap", "rocker", "Primary Drive Gear", "distwire", "radhose_therm", "altfan", "hookw", "hookwF", "turbine", "universalpulley"
		};

		// A bare block: every part taken off the way the game does it when you remove
		// one (Interactor's detach): the part's mesh and solid collider off, its snap
		// trigger left on so parts go back in, its bolts and bits hidden, condition 0.
		// Inner parts first, as the game requires. The block itself stays.
		private static void Strip(GameObject block, Component script)
		{
			HashSet<Transform> keep = new HashSet<Transform> { block.transform };
			FieldInfo body = script.GetType().GetField("block_cnd_c", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			durability bodyPart = body != null ? body.GetValue(script) as durability : null;
			if (bodyPart != null)
			{
				keep.Add(bodyPart.transform);
			}
			Dictionary<durability, string> cndFields = new Dictionary<durability, string>();
			foreach (FieldInfo f in script.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				durability d = f.Name.EndsWith("_cnd_c", StringComparison.Ordinal) ? f.GetValue(script) as durability : null;
				if (d != null && !cndFields.ContainsKey(d))
				{
					cndFields[d] = f.Name.Substring(0, f.Name.Length - 2);
				}
			}
			List<durability> parts = new List<durability>(block.GetComponentsInChildren<durability>(true));
			parts.Sort((a, b) => Depth(b.transform).CompareTo(Depth(a.transform)));
			int stripped = 0;
			foreach (durability d in parts)
			{
				Renderer r = d.GetComponent<Renderer>();
				if (keep.Contains(d.transform) || d.template == null || r == null || !r.enabled)
				{
					continue; // the block, not a removable part, or already off
				}
				foreach (Transform child in d.transform)
				{
					BoxCollider childBox = child.GetComponent<BoxCollider>();
					if (childBox != null && !KeepColliders.Contains(child.name) && child.tag != "ignoreattach")
					{
						childBox.enabled = false;
					}
					if (HideWithPart.Contains(child.name))
					{
						Renderer cr = child.GetComponent<Renderer>();
						if (cr != null)
						{
							cr.enabled = false;
						}
					}
				}
				r.enabled = false;
				BoxCollider[] boxes = d.GetComponents<BoxCollider>();
				if (boxes.Length > 0 && !boxes[0].isTrigger)
				{
					boxes[0].enabled = false;
				}
				else if (boxes.Length > 1)
				{
					boxes[1].enabled = false;
				}
				d.health = 0f;
				string cnd;
				if (cndFields.TryGetValue(d, out cnd))
				{
					SetField(script, cnd, 0f);
				}
				stripped++;
			}
			EngineClonerMod.Log("New " + block.name + ": a bare block (" + stripped + " parts taken off the donor copy).");
		}

		private static int Depth(Transform t)
		{
			int n = 0;
			for (; t.parent != null; t = t.parent)
			{
				n++;
			}
			return n;
		}

		public static void SetField(object o, string name, object value)
		{
			FieldInfo f = o.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (f != null && f.FieldType.IsInstanceOfType(value))
			{
				f.SetValue(o, value);
			}
		}

		// --- Selling. ---

		// For the mods' shared shop list (JunkAuto).
		public static object[][] ShopItems()
		{
			List<object[]> items = new List<object[]>();
			foreach (Kind k in Kinds)
			{
				try
				{
					GameObject t = Template(k);
					if (t != null)
					{
						items.Add(new object[] { t, k.name, Price(k), Category });
					}
				}
				catch (Exception e)
				{
					if (noDonor.Add(k.id))
					{
						EngineClonerMod.Log("Couldn't make a new " + k.blockName + " for sale: " + e);
					}
				}
			}
			return items.ToArray();
		}

		// The Parts Store (ComputerPartStore mod): a postfix on its GetCatalog.
		private static FieldInfo prefabField, nameField, priceField, categoryField;
		private static Type entryType;
		public static bool StoreHooked { get; private set; }

		public static void HookStore(HarmonyLib.Harmony harmony)
		{
			if (StoreHooked)
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
			harmony.Patch(getCatalog, null, new HarmonyMethod(typeof(EngineShop).GetMethod("StorePostfix", BindingFlags.Static | BindingFlags.NonPublic)));
			StoreHooked = true;
			EngineClonerMod.Log("New engines are sold in the Parts Store.");
		}

		private static bool storeError;

		private static void StorePostfix(object __result)
		{
			IList list = __result as IList;
			if (list == null)
			{
				return;
			}
			try
			{
				foreach (Kind k in Kinds)
				{
					// Our entry for this engine (by name: templates are remade per level).
					object entry = null;
					foreach (object e in list)
					{
						if ((string)nameField.GetValue(e) == k.name)
						{
							entry = e;
							break;
						}
					}
					GameObject t = Template(k);
					if (t == null)
					{
						if (entry != null)
						{
							list.Remove(entry);
						}
						continue;
					}
					if (entry == null)
					{
						entry = Activator.CreateInstance(entryType);
						nameField.SetValue(entry, k.name);
						categoryField.SetValue(entry, Category);
						list.Add(entry);
					}
					prefabField.SetValue(entry, t);
					priceField.SetValue(entry, Price(k));
				}
				// The engine stand, too.
				object[][] stand = StandShop.ShopItems();
				object standEntry = null;
				foreach (object e in list)
				{
					if ((string)nameField.GetValue(e) == StandShop.ShopName)
					{
						standEntry = e;
						break;
					}
				}
				if (stand.Length == 0)
				{
					if (standEntry != null)
					{
						list.Remove(standEntry);
					}
				}
				else
				{
					if (standEntry == null)
					{
						standEntry = Activator.CreateInstance(entryType);
						nameField.SetValue(standEntry, StandShop.ShopName);
						categoryField.SetValue(standEntry, Category);
						list.Add(standEntry);
					}
					prefabField.SetValue(standEntry, stand[0][0]);
					priceField.SetValue(standEntry, (float)stand[0][2]);
				}
			}
			catch (Exception e)
			{
				if (!storeError)
				{
					storeError = true;
					EngineClonerMod.Log("Couldn't add engines to the Parts Store: " + e);
				}
			}
		}
	}

	// On every engine this mod makes (bought, restored, or a debug clone). A copy
	// coming out of the shop gets its own anchor at once (before any physics), and
	// one frame later, after the game's own Start()s, its mounting bolts are put out
	// so it lifts off the delivery spot. Kept on the engine so saves find it.
	public class ShopEngine : MonoBehaviour
	{
		public string kind = "";
		// True once set up (and on debug clones, which keep their state).
		public bool ready;
		// The block's own mounting bolts and where each sits when tight.
		public List<Transform> mountBolts = new List<Transform>();
		public List<Vector3> seats = new List<Vector3>();

		// A saved state to put back once set up (EngineSave).
		[NonSerialized]
		public string pendingState;

		// Bolted into a vehicle as it's made (VehicleApi.MountNewEngine): the mounting
		// bolts stay in and it stays put.
		[NonSerialized]
		public bool mountedAtStart;

		private void Awake()
		{
			EngineClonerMod.Adopt(gameObject);
		}

		private void Start()
		{
			StartCoroutine(Setup());
		}

		private IEnumerator Setup()
		{
			yield return null;
			if (!ready && mountedAtStart)
			{
				ready = true;
			}
			if (!ready)
			{
				ready = true;
				for (int i = 0; i < mountBolts.Count && i < seats.Count; i++)
				{
					Transform b = mountBolts[i];
					BoltScript script = b != null ? b.GetComponent<BoltScript>() : null;
					if (script == null)
					{
						continue;
					}
					script.boltturns = 0;
					b.localPosition = seats[i] - b.localRotation * Vector3.forward * 0.02f;
				}
				durability own = GetComponent<durability>();
				if (own != null)
				{
					own.canDetach = true;
					own.boltStr = 0f;
				}
				PickUp pick = GetComponent<PickUp>();
				if (pick != null)
				{
					pick.pickable = true;
				}
			}
			if (pendingState != null)
			{
				string state = pendingState;
				pendingState = null;
				EngineSave.Apply(this, state);
			}
		}

		// Called on the template: the block's direct "bolt" children and their tight
		// spots (a bolt sits 2 mm further out per turn short of 10).
		public void RecordMountBolts()
		{
			mountBolts.Clear();
			seats.Clear();
			foreach (Transform child in transform)
			{
				BoltScript script = child.name == "bolt" ? child.GetComponent<BoltScript>() : null;
				if (script == null)
				{
					continue;
				}
				mountBolts.Add(child);
				seats.Add(child.localPosition + child.localRotation * Vector3.forward * (0.002f * (10 - script.boltturns)));
			}
		}
	}
}
