using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TruckPartsQOL
{
	// Sells the stereo parts through the Junkyard Terminal's Parts Store (the
	// ComputerPartStore mod, ComputerFeatures.PartsStoreService) when it's
	// installed.
	//
	// The store lists PartEntry { prefab, displayName, price, category } from
	// GetCatalog(), and TryPurchase() charges the wallet and Instantiates the
	// prefab next to the player. A postfix on GetCatalog adds our entries to
	// whatever list it returns: the cached catalog once the junkyard has loaded, or
	// the fresh (otherwise empty) list it builds before that. Everything is done by
	// reflection, so this mod still loads without the store.
	internal static class StoreIntegration
	{
		private const string StoreType = "ComputerFeatures.PartsStoreService";

		// The store's "Truck Parts" filter shows category == "Truck Part".
		private const string Category = "Truck Part";

		private static Type entryType;
		private static FieldInfo prefabField;
		private static FieldInfo nameField;
		private static FieldInfo priceField;
		private static FieldInfo categoryField;

		// Inactive, kept across scenes: templates never run, are never saved, and
		// stay valid in the store's static catalog cache.
		private static GameObject templateRoot;
		private static readonly Dictionary<string, GameObject> templates = new Dictionary<string, GameObject>();

		private static List<Wanted> wanted = new List<Wanted>();
		private static float nextCdScan;
		private static int version;
		private static object lastList;
		private static int lastVersion = -1;
		private static bool reportedError;

		public static Func<PartKind, float> PriceOf;

		public static bool Active { get; private set; }

		private struct Wanted
		{
			public string key;
			public PartKind kind;
			public int cd;
			public string name;
			public float price;
		}

		// Patch the store if it's loaded. Safe to call repeatedly.
		public static void TryInstall(HarmonyLib.Harmony harmony)
		{
			if (Active)
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
				TruckPartsQOLMod.Log("Found the parts store, but not the GetCatalog/PartEntry it should have; using the F9 shop instead.");
				return;
			}
			prefabField = entryType.GetField("prefab");
			nameField = entryType.GetField("displayName");
			priceField = entryType.GetField("price");
			categoryField = entryType.GetField("category");
			if (prefabField == null || nameField == null || priceField == null || categoryField == null)
			{
				TruckPartsQOLMod.Log("The parts store's PartEntry has changed; using the F9 shop instead.");
				return;
			}
			harmony.Patch(getCatalog, null, new HarmonyMethod(typeof(StoreIntegration).GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic)));
			Active = true;
			TruckPartsQOLMod.Log("Truck Parts QOL items are now sold in the Junkyard Terminal's Parts Store.");
		}

		private static void Postfix(object __result)
		{
			IList list = __result as IList;
			if (list == null)
			{
				return;
			}
			try
			{
				RefreshWanted();
				// GetCatalog runs every frame the store is open; only touch the list
				// when it's a new one or the CD folders changed.
				if (ReferenceEquals(list, lastList) && lastVersion == version)
				{
					return;
				}
				Sync(list);
				lastList = list;
				lastVersion = version;
			}
			catch (Exception e)
			{
				if (!reportedError)
				{
					reportedError = true;
					TruckPartsQOLMod.Log("Couldn't add stereo parts to the store: " + e);
				}
			}
		}

		private static void RefreshWanted()
		{
			if (Time.realtimeSinceStartup < nextCdScan)
			{
				return;
			}
			nextCdScan = Time.realtimeSinceStartup + 5f;
			List<Wanted> now = new List<Wanted>();
			now.Add(Make("headunit", PartKind.HeadUnit, 0, "Stereo - CD head unit"));
			now.Add(Make("speaker", PartKind.Speaker, 0, "Stereo - 6.5\" door speaker"));
			now.Add(Make("subwoofer", PartKind.Subwoofer, 0, "Stereo - 12\" subwoofer box"));
			now.Add(Make("amp", PartKind.Amplifier, 0, "Stereo - 4-channel amplifier"));
			now.Add(Make("scanner", PartKind.Scanner, 0, "Tool - OBD scanner"));
			now.Add(Make("tarp", PartKind.Tarp, 0, "Bed - Tarp"));
			now.Add(Make("tonneau", PartKind.Tonneau, 0, "Bed - Tonneau cover (tri-fold)"));
			now.Add(Make("hardtop", PartKind.HardTop, 0, "Bed - Hard top (camper shell)"));
			for (int i = 1; i <= MusicLibrary.MaxCds; i++)
			{
				int tracks = MusicLibrary.Tracks(i).Count;
				if (tracks > 0)
				{
					now.Add(Make("cd" + i, PartKind.CD, i, "Stereo - CD " + i + " (" + tracks + " tracks)"));
				}
			}
			if (!SameAs(now))
			{
				wanted = now;
				version++;
			}
		}

		private static Wanted Make(string key, PartKind kind, int cd, string name)
		{
			Wanted w;
			w.key = key;
			w.kind = kind;
			w.cd = cd;
			w.name = name;
			w.price = PriceOf != null ? PriceOf(kind) : 0f;
			return w;
		}

		private static bool SameAs(List<Wanted> now)
		{
			if (now.Count != wanted.Count)
			{
				return false;
			}
			for (int i = 0; i < now.Count; i++)
			{
				if (now[i].key != wanted[i].key || now[i].name != wanted[i].name || now[i].price != wanted[i].price)
				{
					return false;
				}
			}
			return true;
		}

		private static void Sync(IList list)
		{
			// Index our existing entries by template.
			Dictionary<GameObject, object> ours = new Dictionary<GameObject, object>();
			foreach (object entry in list)
			{
				GameObject prefab = prefabField.GetValue(entry) as GameObject;
				if (prefab != null && prefab.transform.parent == TemplateRoot().transform)
				{
					ours[prefab] = entry;
				}
			}
			bool changed = false;
			HashSet<GameObject> keep = new HashSet<GameObject>();
			foreach (Wanted w in wanted)
			{
				GameObject template = Template(w);
				keep.Add(template);
				object entry;
				if (!ours.TryGetValue(template, out entry))
				{
					entry = Activator.CreateInstance(entryType);
					prefabField.SetValue(entry, template);
					categoryField.SetValue(entry, Category);
					list.Add(entry);
					changed = true;
				}
				if ((string)nameField.GetValue(entry) != w.name || (float)priceField.GetValue(entry) != w.price)
				{
					nameField.SetValue(entry, w.name);
					priceField.SetValue(entry, w.price);
					changed = true;
				}
			}
			// A CD whose folder was emptied.
			foreach (KeyValuePair<GameObject, object> pair in ours)
			{
				if (!keep.Contains(pair.Key))
				{
					list.Remove(pair.Value);
					changed = true;
				}
			}
			if (changed)
			{
				SortLikeStore(list);
			}
		}

		// The store sorts by category, then name.
		private static void SortLikeStore(IList list)
		{
			object[] items = new object[list.Count];
			list.CopyTo(items, 0);
			Array.Sort(items, delegate(object a, object b)
			{
				int c = string.Compare((string)categoryField.GetValue(a), (string)categoryField.GetValue(b), StringComparison.OrdinalIgnoreCase);
				return c != 0 ? c : string.Compare((string)nameField.GetValue(a), (string)nameField.GetValue(b), StringComparison.OrdinalIgnoreCase);
			});
			list.Clear();
			foreach (object item in items)
			{
				list.Add(item);
			}
		}

		private static GameObject TemplateRoot()
		{
			if (templateRoot == null)
			{
				templateRoot = new GameObject("TruckPartsQOL_StoreTemplates");
				templateRoot.SetActive(false);
				Object.DontDestroyOnLoad(templateRoot);
				templates.Clear();
			}
			return templateRoot;
		}

		private static GameObject Template(Wanted w)
		{
			GameObject template;
			if (templates.TryGetValue(w.key, out template) && template != null)
			{
				return template;
			}
			template = PartFactory.Create(w.kind, w.cd, Vector3.zero, Quaternion.identity, TemplateRoot().transform).gameObject;
			templates[w.key] = template;
			return template;
		}
	}
}
