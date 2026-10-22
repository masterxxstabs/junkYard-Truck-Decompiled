using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EngineCloner
{
	// For other mods that add engines of their own on top of the game's (the
	// 6.6L Duramax mod, for one). Engine Cloner is the patch underneath: it makes
	// every copy of an engine work on its own, sells new ones and saves them;
	// these let another mod plug into that.
	//
	//   RegisterEngine  a new engine kind, built on one of the game's engines
	//                   (its parts, bay and gauges): sold, mounted, saved and
	//                   restored like Engine Cloner's own, with the mod's look on it
	//   KindOf          which kind a block is ("i4", "v8", "i6", or a registered id)
	//   Saving/Loading  lines of the mod's own in Engine Cloner's save files
	public static class EngineApi
	{
		public sealed class EngineKind
		{
			// Saved with each engine of this kind, and how other mods ask for it.
			public string id;
			// The shops' name: "Engine - ... (new)".
			public string shopName;
			// The game engine it's built on: "i4", "v8" or "i6".
			public string baseEngine;
			public Func<float> price;
			// Called on the shop's template (bare or complete) once it's made: every
			// engine of this kind is a copy of it, so it's where the look goes.
			public Action<GameObject> style;
			// Also listed in a Parts Store category of its own, first in its list
			// ("All"); null for none.
			public string storeCategory;
			// More items for that category, listed right after the engine:
			// { GameObject prefab, string name, float price, string category }.
			public Func<object[][]> storeItems;
		}

		// False if the id is taken or the base engine unknown.
		public static bool RegisterEngine(EngineKind kind)
		{
			return EngineShop.Register(kind);
		}

		// The kind of an engine block this mod made ("i4", "v8", "i6" or a
		// registered id), or null for the game's own engines.
		public static string KindOf(GameObject block)
		{
			ShopEngine marker = block != null ? block.GetComponent<ShopEngine>() : null;
			return marker != null ? marker.kind : null;
		}

		// Called while Engine Cloner writes a save file: append lines of your own
		// (one per line, "YOURTAG;..."; not ENGINE or STAND).
		public static event Action<StringBuilder> Saving;

		// Called with each line of a save file that isn't Engine Cloner's own
		// (split on ';'): return true if it's yours. Lines nobody takes are kept
		// as they are, so a mod that's not installed loses nothing.
		public static event Func<string[], bool> Loading;

		internal static void RaiseSaving(StringBuilder sb)
		{
			if (Saving == null)
			{
				return;
			}
			foreach (Action<StringBuilder> handler in Saving.GetInvocationList())
			{
				try
				{
					handler(sb);
				}
				catch (Exception e)
				{
					EngineClonerMod.Log("A mod's save lines failed: " + e.Message);
				}
			}
		}

		internal static bool RaiseLoading(string[] fields)
		{
			if (Loading == null)
			{
				return false;
			}
			foreach (Func<string[], bool> handler in Loading.GetInvocationList())
			{
				try
				{
					if (handler(fields))
					{
						return true;
					}
				}
				catch (Exception e)
				{
					EngineClonerMod.Log("A mod couldn't load a save line (it's kept): " + e.Message);
					return false;
				}
			}
			return false;
		}
	}
}
