using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JunkAutoPC
{
	internal class Product
	{
		public GameObject prefab;
		public string key;       // "<store category>|<store name>": how saves find it again
		public string name;      // shown
		public string partNo;
		public float price;
		public string vehicle;   // tree level 1
		public string category;  // tree level 2

		public Product MemberwiseCloneProduct()
		{
			return (Product)MemberwiseClone();
		}
	}

	// What JunkAuto sells: everything the Junkyard Terminal's Parts Store sells
	// (its GetCatalog, which our other mods and Parts Store Plus add to), or, without
	// that mod, what the junkyard spawns (JunkSpawner's truck and bike part lists).
	// Grouped like RockAuto: vehicle, then part category.
	internal static class Catalog
	{
		// Truck parts that aren't engine parts fit any of the trucks, so they're
		// listed as universal, not under one truck.
		public const string Truck = "UNIVERSAL / OTHER";
		public const string Bike = "250CC DIRT BIKE";
		public const string Other = "UNIVERSAL / OTHER";
		public const string I4 = "I4 ENGINE";
		public const string V8 = "V8 ENGINE";
		public const string I6 = "2J INLINE-6 ENGINE";
		// The 6.6L Duramax mod's engine: its own section, with every part that fits it
		// (it's built on the V8, so it takes the V8's parts).
		public const string Duramax = "6.6L DURAMAX DIESEL";
		public static readonly string[] Engines = { I4, V8, I6 };
		// Furniture Tweaks' shelves, safes and crates (sold as "Furniture - ..."),
		// and the engine stand.
		public const string Furniture = "GARAGE FURNITURE";

		private static MethodInfo getCatalog;
		private static FieldInfo prefabField, nameField, priceField, categoryField;
		private static bool looked;

		private static List<Product> products = new List<Product>();
		private static float builtAt = -100f;

		public static List<Product> All
		{
			get
			{
				// Rebuilt now and then: the store fills in once the junkyard loads,
				// and other mods add to it.
				if (products.Count == 0 || Time.realtimeSinceStartup - builtAt > 10f)
				{
					Build();
				}
				return products;
			}
		}

		// Store entries that were renamed: orders saved under the old name still
		// find the part.
		private static readonly Dictionary<string, string> Renamed = new Dictionary<string, string>
		{
			{ "Truck Part|Stereo - 6.5\" speaker", "Truck Part|Stereo - 6.5\" door speaker" },
		};

		public static Product Find(string key)
		{
			string renamed;
			foreach (string k in Renamed.TryGetValue(key, out renamed) ? new[] { key, renamed } : new[] { key })
			{
				foreach (Product p in All)
				{
					if (p.key == k)
					{
						return p;
					}
				}
			}
			return null;
		}

		private static void Build()
		{
			builtAt = Time.realtimeSinceStartup;
			ScanEngines();
			List<Product> list = new List<Product>();
			HashSet<string> keys = new HashSet<string>();
			IList store = StoreCatalog();
			if (store != null)
			{
				foreach (object e in store)
				{
					Add(list, keys, prefabField.GetValue(e) as GameObject, (string)nameField.GetValue(e), (float)priceField.GetValue(e), (string)categoryField.GetValue(e));
				}
			}
			else
			{
				foreach (JunkSpawner spawner in Object.FindObjectsOfType<JunkSpawner>())
				{
					AddAll(list, keys, spawner.spawnItems, "Truck Part");
					AddAll(list, keys, spawner.spawn250Items, "Dirt Bike Part");
				}
			}
			// Items our other mods sell (stereo parts, the ATV...), published to a
			// shared list so they're here with or without the Parts Store mod. Ones
			// the store already lists are skipped by key.
			int provided = AddProvided(list, keys);
			if (list.Count > 0 || products.Count == 0)
			{
				list.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase));
				if (list.Count != products.Count)
				{
					Report(list, store != null, provided);
				}
				products = list;
			}
		}

		// The shared shop list: AppDomain data "JunkyardTruckMods.ShopProviders", a
		// List<Func<object[][]>>; each item is { GameObject prefab, string name,
		// float price, string category ("Truck Part" / "Dirt Bike Part") }.
		// Only mscorlib types, so mods share it without referencing each other.
		public const string ProvidersKey = "JunkyardTruckMods.ShopProviders";
		private static readonly HashSet<string> badProviders = new HashSet<string>();

		private static int AddProvided(List<Product> list, HashSet<string> keys)
		{
			List<Func<object[][]>> providers = AppDomain.CurrentDomain.GetData(ProvidersKey) as List<Func<object[][]>>;
			if (providers == null)
			{
				return 0;
			}
			int added = 0;
			foreach (Func<object[][]> provider in providers.ToArray())
			{
				try
				{
					foreach (object[] item in provider() ?? new object[0][])
					{
						if (item == null || item.Length < 4)
						{
							continue;
						}
						int before = list.Count;
						Add(list, keys, item[0] as GameObject, item[1] as string, item[2] is float ? (float)item[2] : 0f, item[3] as string ?? "Truck Part");
						added += list.Count - before;
					}
				}
				catch (Exception e)
				{
					string who = provider.Method.DeclaringType != null ? provider.Method.DeclaringType.FullName : "a mod";
					if (badProviders.Add(who))
					{
						JunkAutoMod.Log("Couldn't read the items " + who + " sells: " + e);
					}
				}
			}
			return added;
		}

		// What's on the shelves, in the log, whenever the catalog grows or shrinks.
		private static void Report(List<Product> list, bool fromStore, int provided)
		{
			SortedDictionary<string, int> shelves = new SortedDictionary<string, int>();
			List<string> audio = new List<string>();
			foreach (Product p in list)
			{
				string shelf = p.vehicle + " / " + p.category;
				int n;
				shelves.TryGetValue(shelf, out n);
				shelves[shelf] = n + 1;
				if (p.category == "AUDIO & ELECTRONICS")
				{
					audio.Add(p.name);
				}
			}
			List<string> parts = new List<string>();
			foreach (KeyValuePair<string, int> s in shelves)
			{
				parts.Add(s.Key + ": " + s.Value);
			}
			JunkAutoMod.Log("Catalog: " + list.Count + " parts from " + (fromStore ? "the Parts Store" : "the junkyard's spawn lists (no Parts Store mod)") + ", " + provided + " of them from other mods. " + string.Join("; ", parts.ToArray()));
			JunkAutoMod.Log("Audio & electronics: " + (audio.Count > 0 ? string.Join(", ", audio.ToArray()) : "none (is Truck Parts QOL installed and up to date?)"));
		}

		private static void AddAll(List<Product> list, HashSet<string> keys, GameObject[] prefabs, string storeCategory)
		{
			if (prefabs == null)
			{
				return;
			}
			foreach (GameObject prefab in prefabs)
			{
				PickUp pick = prefab != null ? prefab.GetComponent<PickUp>() : null;
				if (pick != null)
				{
					Add(list, keys, prefab, prefab.name.Replace("(Clone)", "").Trim(), pick.price, storeCategory);
				}
			}
		}

		private static void Add(List<Product> list, HashSet<string> keys, GameObject prefab, string storeName, float price, string storeCategory)
		{
			if (prefab == null || string.IsNullOrEmpty(storeName))
			{
				return;
			}
			string key = storeCategory + "|" + storeName;
			if (!keys.Add(key))
			{
				return;
			}
			PickUp pick = prefab.GetComponent<PickUp>();
			string description = pick != null ? pick.description : null;
			Product p = new Product();
			p.prefab = prefab;
			p.key = key;
			p.name = Pretty(storeName, description);
			p.partNo = PartNumber(storeName);
			p.price = Mathf.Max(0f, price);
			p.vehicle = storeCategory == "Truck Part" ? Truck : storeCategory == "Dirt Bike Part" ? Bike : Other;
			p.category = Classify(storeName + " " + description);
			// Garage furniture: its own section, with the engine stand.
			bool furniture = storeName.StartsWith("Furniture - ", StringComparison.OrdinalIgnoreCase);
			if (furniture || storeName.StartsWith("Tool - Engine stand", StringComparison.OrdinalIgnoreCase))
			{
				p.vehicle = Furniture;
				p.category = furniture ? "STORAGE" : "SHOP EQUIPMENT";
				list.Add(p);
				return;
			}
			// The Duramax heads its own section, with its accessories.
			if (storeName.IndexOf("Duramax", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				p.vehicle = Duramax;
				p.category = storeName.StartsWith("Engine - ", StringComparison.OrdinalIgnoreCase) ? "ENGINE" : "PERFORMANCE & ACCESSORIES";
				list.Add(p);
				return;
			}
			// Whole vehicles (the ATV) get their own shelf.
			if (storeName.StartsWith("Vehicle", StringComparison.OrdinalIgnoreCase))
			{
				p.vehicle = "VEHICLES";
				p.category = "COMPLETE VEHICLES";
				list.Add(p);
				return;
			}
			// Truck engine parts go under their engine; a part that fits more than
			// one engine is listed under each, like RockAuto does.
			List<string> engines = EnginesOf(prefab, storeName);
			if (storeCategory == "Truck Part" && engines.Count > 0)
			{
				List<string> shelves = new List<string>(engines);
				if (engines.Contains(V8))
				{
					shelves.Add(Duramax);
				}
				for (int i = 0; i < shelves.Count; i++)
				{
					Product copy = i == 0 ? p : p.MemberwiseCloneProduct();
					copy.vehicle = shelves[i];
					list.Add(copy);
				}
				return;
			}
			list.Add(p);
		}

		// --- Which truck engine a part belongs to. ---

		// Every part slot on a block is a `durability` whose templates are the loose
		// parts it takes, so the blocks in the level say which part fits which engine.
		// Kept by part name across scans: the store's prefab and the slot's template
		// can be different objects for the same part.
		private static readonly Dictionary<string, List<string>> engineParts = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

		private static void ScanEngines()
		{
			Scan<engine>(I4);
			Scan<enginev8>(V8);
			Scan<enginei6>(I6);
		}

		private static void Scan<T>(string engineName) where T : Component
		{
			foreach (T block in Object.FindObjectsOfType<T>())
			{
				foreach (durability slot in block.GetComponentsInChildren<durability>(true))
				{
					Note(slot.template, engineName);
					Note(slot.template2, engineName);
					Note(slot.template3, engineName);
					Note(slot.template4, engineName);
					Note(slot.template5, engineName);
				}
			}
		}

		private static void Note(GameObject template, string engineName)
		{
			if (template == null)
			{
				return;
			}
			string name = template.name.Replace("(Clone)", "").Trim();
			List<string> engines;
			if (!engineParts.TryGetValue(name, out engines))
			{
				engineParts[name] = engines = new List<string>();
			}
			if (!engines.Contains(engineName))
			{
				engines.Add(engineName);
				engines.Sort((a, b) => Array.IndexOf(Engines, a).CompareTo(Array.IndexOf(Engines, b)));
			}
		}

		private static List<string> EnginesOf(GameObject prefab, string storeName)
		{
			// Engine Cloner's blocks: "Engine - I4 (new)", "Engine - V8 (new)", "Engine - 2J inline-6 (new)".
			if (storeName.StartsWith("Engine - ", StringComparison.OrdinalIgnoreCase))
			{
				string kind = storeName.Substring(9).ToLowerInvariant();
				string engineName = kind.StartsWith("i4") ? I4 : kind.StartsWith("v8") ? V8 : kind.StartsWith("2j") || kind.StartsWith("i6") ? I6 : null;
				if (engineName != null)
				{
					return new List<string> { engineName };
				}
			}
			List<string> engines;
			if (engineParts.TryGetValue(prefab.name.Replace("(Clone)", "").Trim(), out engines) || engineParts.TryGetValue(storeName, out engines))
			{
				return engines;
			}
			return new List<string>();
		}

		private static IList StoreCatalog()
		{
			if (!looked)
			{
				looked = true;
				foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
				{
					Type store = a.GetType("ComputerFeatures.PartsStoreService", false);
					if (store == null)
					{
						continue;
					}
					Type entry = store.GetNestedType("PartEntry", BindingFlags.Public | BindingFlags.NonPublic);
					getCatalog = store.GetMethod("GetCatalog", BindingFlags.Static | BindingFlags.Public);
					if (entry != null)
					{
						prefabField = entry.GetField("prefab");
						nameField = entry.GetField("displayName");
						priceField = entry.GetField("price");
						categoryField = entry.GetField("category");
					}
					if (prefabField == null || nameField == null || priceField == null || categoryField == null)
					{
						getCatalog = null;
					}
					break;
				}
			}
			if (getCatalog == null)
			{
				return null;
			}
			try
			{
				return getCatalog.Invoke(null, null) as IList;
			}
			catch (Exception e)
			{
				JunkAutoMod.Log("Couldn't read the Parts Store catalog: " + e.Message);
				return null;
			}
		}

		// --- Names. ---

		// The part's own in-game description when it has a short one ("Carburetor"),
		// else its prefab name tidied up ("v8_intake" -> "V8 Intake").
		private static string Pretty(string raw, string description)
		{
			if (!string.IsNullOrEmpty(description))
			{
				string d = description.Trim();
				int nl = d.IndexOf('\n');
				if (nl > 0)
				{
					d = d.Substring(0, nl).Trim();
				}
				if (d.Length > 1 && d.Length <= 40)
				{
					return d;
				}
			}
			StringBuilder sb = new StringBuilder();
			char prev = ' ';
			foreach (char ch in raw)
			{
				char c = ch == '_' || ch == '-' ? ' ' : ch;
				if (c != ' ' && char.IsUpper(c) && char.IsLower(prev))
				{
					sb.Append(' ');
				}
				sb.Append(c);
				prev = c;
			}
			string[] words = sb.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < words.Length; i++)
			{
				string w = words[i];
				words[i] = w.Length <= 2 ? w.ToUpperInvariant() : char.ToUpperInvariant(w[0]) + w.Substring(1);
			}
			return string.Join(" ", words);
		}

		// A stable made-up part number, like a catalog would show.
		private static string PartNumber(string raw)
		{
			uint h = 2166136261;
			foreach (char c in raw)
			{
				h = (h ^ c) * 16777619;
			}
			return "JA-" + (h % 900000 + 100000);
		}

		// --- RockAuto-style categories, by keywords in the part's name. ---

		private static readonly string[][] Categories =
		{
			new[] { "BRAKE & WHEEL HUB", "brake", "rotor", "disk", "disc", "caliper", "pad", "wheel", "tire", "tyre", "rim", "hub", "lug" },
			new[] { "COOLING SYSTEM", "radiator", "fan", "thermostat", "coolant", "waterpump", "water pump", "hose" },
			new[] { "EXHAUST & EMISSION", "exhaust", "muffler", "header", "pipe", "catalytic" },
			new[] { "FUEL & AIR", "carb", "fuel", "airfilter", "air filter", "intake", "throttle", "injector", "tank", "jerry" },
			new[] { "IGNITION & ELECTRICAL", "spark", "plug", "coil", "distributor", "alternator", "battery", "starter", "wire", "light", "lamp", "switch", "stator", "cdi", "fuse" },
			new[] { "AUDIO & ELECTRONICS", "stereo", "speaker", "subwoofer", "head unit", "headunit", "radio", " cd", "scanner", "obd" },
			new[] { "TRANSMISSION", "trans", "clutch", "gear", "shifter" },
			new[] { "DRIVELINE & AXLE", "diff", "axle", "driveshaft", "drive shaft", "ujoint", "u-joint", "chain", "sprocket", "transfer" },
			new[] { "SUSPENSION & STEERING", "shock", "spring", "strut", "steer", "tie rod", "tierod", "suspension", "fork", "swingarm", "leaf" },
			new[] { "ENGINE", "block", "piston", "head", "cam", "crank", "gasket", "valve", "oil", "flywheel", "fly wheel", "cylinder", "engine", "pulley", "timing", "rocker", "manifold", "case", "belt", "motor" },
			new[] { "BODY & ACCESSORIES", "door", "hood", "bumper", "fender", "mirror", "seat", "bed", "tailgate", "winch", "cover", "tarp", "glass", "grill", "tonneau", "hard top", "hardtop", "rack", "mount", "hitch", "jack", "frame", "plate" },
		};

		private static string Classify(string text)
		{
			string t = " " + text.ToLowerInvariant().Replace('_', ' ') + " ";
			foreach (string[] cat in Categories)
			{
				for (int i = 1; i < cat.Length; i++)
				{
					if (t.Contains(cat[i]))
					{
						return cat[0];
					}
				}
			}
			return "MISCELLANEOUS";
		}
	}
}
