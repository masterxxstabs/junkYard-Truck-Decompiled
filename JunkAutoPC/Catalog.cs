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
	}

	// What JunkAuto sells: everything the Junkyard Terminal's Parts Store sells
	// (its GetCatalog, which our other mods and Parts Store Plus add to), or, without
	// that mod, what the junkyard spawns (JunkSpawner's truck and bike part lists).
	// Grouped like RockAuto: vehicle, then part category.
	internal static class Catalog
	{
		public const string Truck = "1982 DIAMONDBACK PICKUP";
		public const string Bike = "250CC DIRT BIKE";
		public const string Other = "UNIVERSAL / OTHER";

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
			if (list.Count > 0 || products.Count == 0)
			{
				list.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase));
				products = list;
			}
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
			// Whole vehicles (the ATV) get their own shelf.
			if (storeName.StartsWith("Vehicle", StringComparison.OrdinalIgnoreCase))
			{
				p.vehicle = "VEHICLES";
				p.category = "COMPLETE VEHICLES";
			}
			list.Add(p);
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
