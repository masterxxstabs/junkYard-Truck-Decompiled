using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(PartsStorePlus.PartsStorePlusMod), "Parts Store Plus", "1.0.0", "masterxxstabs")]
[assembly: MelonGame(null, null)]

namespace PartsStorePlus
{
	// Fills the gaps in the Junkyard Terminal's Parts Store (ComputerPartStore).
	//
	// The store lists what the junkyard can spawn: JunkSpawner.spawnItems as truck
	// parts and spawn250Items as dirt bike parts. Bike parts the junkyard never
	// spawns aren't sold. Every part slot on the dirt bike and its 250 engine is a
	// `durability` whose `template` is the loose part the game spawns when you
	// take that part off, so those templates are every dirt bike part there is.
	// This adds the ones the store is missing, under "Dirt Bike Part", at the
	// part's own price.
	public class PartsStorePlusMod : MelonMod
	{
		private const string StoreType = "ComputerFeatures.PartsStoreService";
		private const string BikeCategory = "Dirt Bike Part";

		private static MelonLogger.Instance log;
		private static MelonPreferences_Entry<bool> addBikeParts;
		private static MelonPreferences_Entry<float> priceMultiplier;
		private static MelonPreferences_Entry<float> minimumPrice;

		private static FieldInfo prefabField;
		private static FieldInfo nameField;
		private static FieldInfo priceField;
		private static FieldInfo categoryField;
		private static Type entryType;
		private bool installed;

		// The bike part templates found so far (prefab -> name). Kept across scans:
		// a part that's off the bike right now still has a slot on it, but a bike
		// that's gone (another scene) shouldn't empty the list.
		private static readonly Dictionary<GameObject, string> bikeParts = new Dictionary<GameObject, string>();
		private static float nextScan;
		private static int version;
		private static object lastList;
		private static int lastVersion = -1;
		private static string lastSettings = "";
		private static bool reportedError;
		private static int reportedCount = -1;

		public override void OnInitializeMelon()
		{
			log = LoggerInstance;
			MelonPreferences_Category c = MelonPreferences.CreateCategory("PartsStorePlus", "Parts Store Plus");
			addBikeParts = c.CreateEntry("AddMissingBikeParts", true, "Sell every dirt bike part", "Add the dirt bike parts the junkyard never spawns to the Parts Store.");
			priceMultiplier = c.CreateEntry("PriceMultiplier", 1f, "Price multiplier", "Multiplies the price of the parts this mod adds (1 = the game's own part price).");
			minimumPrice = c.CreateEntry("MinimumPrice", 5f, "Minimum price", "Parts the game prices lower than this (or not at all) cost this much.");
		}

		public override void OnLateInitializeMelon()
		{
			Install();
		}

		public override void OnSceneWasInitialized(int buildIndex, string sceneName)
		{
			bikeParts.Clear();
			nextScan = 0f;
			version++;
			Install(); // in case the store mod loaded after us
		}

		private void Install()
		{
			if (installed)
			{
				return;
			}
			Type store = null;
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				store = assembly.GetType(StoreType, false);
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
				log.Warning("Found the Parts Store, but not its GetCatalog/PartEntry; nothing added.");
				installed = true;
				return;
			}
			prefabField = entryType.GetField("prefab");
			nameField = entryType.GetField("displayName");
			priceField = entryType.GetField("price");
			categoryField = entryType.GetField("category");
			if (prefabField == null || nameField == null || priceField == null || categoryField == null)
			{
				log.Warning("The Parts Store's PartEntry has changed; nothing added.");
				installed = true;
				return;
			}
			HarmonyInstance.Patch(getCatalog, null, new HarmonyMethod(typeof(PartsStorePlusMod).GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic)));
			installed = true;
			log.Msg("Hooked the Parts Store.");
		}

		// GetCatalog runs every frame the store is open: only touch the list when
		// it's a new one, the parts found changed, or the settings changed.
		private static void Postfix(object __result)
		{
			IList list = __result as IList;
			if (list == null)
			{
				return;
			}
			try
			{
				Scan();
				string settings = addBikeParts.Value + "|" + priceMultiplier.Value + "|" + minimumPrice.Value;
				if (ReferenceEquals(list, lastList) && lastVersion == version && settings == lastSettings)
				{
					return;
				}
				Sync(list);
				lastList = list;
				lastVersion = version;
				lastSettings = settings;
			}
			catch (Exception e)
			{
				if (!reportedError)
				{
					reportedError = true;
					log.Error("Couldn't add parts to the store: " + e);
				}
			}
		}

		// Every few seconds, collect the part templates of every slot on the dirt
		// bike(s) and every 250 engine (the bike's engine can be out of the bike).
		private static void Scan()
		{
			if (Time.realtimeSinceStartup < nextScan)
			{
				return;
			}
			nextScan = Time.realtimeSinceStartup + 5f;
			int before = bikeParts.Count;
			foreach (Dirtbike bike in Object.FindObjectsOfType<Dirtbike>())
			{
				Collect(bike.transform);
			}
			foreach (Engine250 engine in Object.FindObjectsOfType<Engine250>())
			{
				Collect(engine.transform);
			}
			if (bikeParts.Count != before)
			{
				version++;
			}
		}

		private static void Collect(Transform root)
		{
			foreach (durability slot in root.GetComponentsInChildren<durability>(true))
			{
				Add(slot.template);
				Add(slot.template2);
				Add(slot.template3);
				Add(slot.template4);
				Add(slot.template5);
			}
		}

		private static void Add(GameObject template)
		{
			if (template == null || bikeParts.ContainsKey(template) || template.GetComponent<PickUp>() == null)
			{
				return;
			}
			bikeParts[template] = template.name.Replace("(Clone)", "").Trim();
		}

		// Add what's missing, keep our own entries' prices current, drop ours when
		// turned off. The store's own entries are never changed.
		private static void Sync(IList list)
		{
			HashSet<GameObject> listed = new HashSet<GameObject>();
			HashSet<string> listedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			List<object> ours = new List<object>();
			foreach (object entry in list)
			{
				if (entry == null)
				{
					continue;
				}
				GameObject prefab = prefabField.GetValue(entry) as GameObject;
				if (IsOurs(entry))
				{
					ours.Add(entry);
					continue;
				}
				if (prefab != null)
				{
					listed.Add(prefab);
				}
				if ((string)categoryField.GetValue(entry) == BikeCategory)
				{
					listedNames.Add((string)nameField.GetValue(entry));
				}
			}
			foreach (object entry in ours)
			{
				list.Remove(entry);
				marked.Remove(entry);
			}
			if (!addBikeParts.Value)
			{
				return;
			}
			int added = 0;
			List<string> names = new List<string>();
			foreach (KeyValuePair<GameObject, string> part in bikeParts)
			{
				if (part.Key == null || listed.Contains(part.Key) || listedNames.Contains(part.Value))
				{
					continue; // the store already sells it
				}
				listedNames.Add(part.Value); // two slots, same part: list it once
				object e = Activator.CreateInstance(entryType);
				prefabField.SetValue(e, part.Key);
				nameField.SetValue(e, part.Value);
				priceField.SetValue(e, PriceOf(part.Key));
				categoryField.SetValue(e, BikeCategory);
				Mark(e);
				list.Add(e);
				added++;
				names.Add(part.Value);
			}
			if (added > 0)
			{
				// Same order as the store's own sort: category, then name.
				ArrayList sorted = new ArrayList(list);
				sorted.Sort(new EntryOrder());
				list.Clear();
				foreach (object entry in sorted)
				{
					list.Add(entry);
				}
			}
			if (added != reportedCount)
			{
				reportedCount = added;
				names.Sort(StringComparer.OrdinalIgnoreCase);
				log.Msg(added == 0 ? "The Parts Store already sells every dirt bike part found." : "Added " + added + " dirt bike part(s) to the Parts Store: " + string.Join(", ", names.ToArray()));
			}
		}

		private static float PriceOf(GameObject template)
		{
			float price = template.GetComponent<PickUp>().price * Mathf.Max(0f, priceMultiplier.Value);
			return Mathf.Round(Mathf.Max(price, minimumPrice.Value) * 100f) / 100f;
		}

		// Our entries are remembered by identity, so they can be told apart from the
		// store's own (and other mods') entries on the next pass.
		private static readonly HashSet<object> marked = new HashSet<object>();

		private static void Mark(object entry)
		{
			marked.Add(entry);
		}

		private static bool IsOurs(object entry)
		{
			return marked.Contains(entry);
		}

		private class EntryOrder : IComparer
		{
			public int Compare(object a, object b)
			{
				int c = string.Compare((string)categoryField.GetValue(a), (string)categoryField.GetValue(b), StringComparison.OrdinalIgnoreCase);
				return c != 0 ? c : string.Compare((string)nameField.GetValue(a), (string)nameField.GetValue(b), StringComparison.OrdinalIgnoreCase);
			}
		}
	}
}
