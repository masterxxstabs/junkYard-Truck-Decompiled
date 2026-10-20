using System;
using System.Globalization;
using System.IO;
using EngineCloner;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(JunkyardRanger.RangerMod), "Junkyard Ranger", "1.5.3", "masterxxstabs")]
[assembly: MelonGame(null, null)]

namespace JunkyardRanger
{
	public enum RangerEngine
	{
		I4,
		V8,
		I6_2J,
		None
	}

	// A 2004 Ford Ranger regular cab, built from the game's own trucks: the body is
	// the game's F100 made into a Ranger (see tools/ in the repository), and
	// underneath it's a copy of the Diamondback, so it drives like the game's
	// truck and takes the game's engines (I4, V8, 2J), gearbox, wheels and parts.
	//
	// Engine Cloner does the copying: each Ranger is a copy of the Diamondback the
	// game wires itself to when you get in, with its own engine, saved with your
	// save slot.
	public class RangerMod : MelonMod
	{
		private static MelonLogger.Instance log;
		private static MelonPreferences_Entry<float> price;
		private static MelonPreferences_Entry<RangerEngine> engineKind;
		private static MelonPreferences_Entry<bool> completeEngine;
		private static MelonPreferences_Entry<string> paintColor;
		private static MelonPreferences_Entry<bool> weathered;
		private static MelonPreferences_Entry<KeyCode> spawnKey;
		private static MelonPreferences_Entry<float> bullBarPrice;
		private static MelonPreferences_Entry<float> rainGuardPrice;
		private static MelonPreferences_Entry<float> sunVisorPrice;
		private static MelonPreferences_Entry<string> lowerColor;

		private static string hint = "";
		private static float hintUntil;

		public static string Folder
		{
			get { return Path.Combine(Path.Combine(Path.GetDirectoryName(Application.dataPath), "UserData"), "JunkyardRanger"); }
		}

		public override void OnInitializeMelon()
		{
			log = LoggerInstance;
			MelonPreferences_Category c = MelonPreferences.CreateCategory("JunkyardRanger", "Junkyard Ranger");
			price = c.CreateEntry("Price", 4500f, "Price", "What a new Ranger costs in the shops.");
			engineKind = c.CreateEntry("Engine", RangerEngine.I4, "Engine", "The engine a new Ranger comes with (the game's own engines), or None to fit your own.");
			completeEngine = c.CreateEntry("CompleteEngine", true, "Engine: complete", "On: the engine comes with every part on, ready to run. Off: a bare block to build up.");
			paintColor = c.CreateEntry("PaintColor", "#E8E8E4", "Paint color", "A new Ranger's color (above the two-tone line), as #RRGGBB. Repaint it in game with the spray cans.");
			weathered = c.CreateEntry("Weathered", false, "Weathered paint", "On: the paint is worn through to rust in places, like the game's other trucks. Off: clean paint.");
			rainGuardPrice = c.CreateEntry("RainGuardPrice", 60f, "Rain guards price", "What a pair of rain guards costs in the shops.");
			sunVisorPrice = c.CreateEntry("SunVisorPrice", 180f, "Sun visor price", "What the sun visor costs in the shops.");
			lowerColor = c.CreateEntry("LowerColor", "#3A3D40", "Two-tone lower color", "A new Ranger's two-tone: the color below the line along the sides, and the bumper, as #RRGGBB. Repaint it in game with the spray cans (aim below the line).");
			bullBarPrice = c.CreateEntry("BullBarPrice", 350f, "Bull bar price", "What the Ranger's bull bar costs in the shops.");
			spawnKey = c.CreateEntry("SpawnKey", KeyCode.F7, "Spawn key", "Press in the level for a free Ranger in front of you, with an empty engine bay to fit your own engine. None turns it off.");
			ShopProviders.Register(RangerShop.ShopItems);
			VehicleApi.Saving += RangerSave.Save;
			VehicleApi.Loading += RangerSave.Load;
			VehicleApi.Deleting += RangerSave.Delete;
			Log("Ready. The body files are in " + Folder + ".");
		}

		public override void OnLateInitializeMelon()
		{
			RangerShop.HookStore(HarmonyInstance);
		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			RangerBuilder.Reset();
			RangerShop.HookStore(HarmonyInstance);
		}

		public override void OnUpdate()
		{
			if (spawnKey != null && spawnKey.Value != KeyCode.None && Input.GetKeyDown(spawnKey.Value))
			{
				RangerBuilder.Spawn();
			}
			RangerBuilder.Update();
			RangerAccessories.UpdateTriggers();
		}

		public override void OnGUI()
		{
			if (Time.time > hintUntil || string.IsNullOrEmpty(hint))
			{
				return;
			}
			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.fontSize = 18;
			style.alignment = TextAnchor.MiddleCenter;
			style.normal.textColor = Color.white;
			Rect r = new Rect(0, Screen.height * 0.78f, Screen.width, 30);
			GUI.color = new Color(0f, 0f, 0f, 0.7f);
			GUI.Label(new Rect(r.x + 1, r.y + 1, r.width, r.height), hint, style);
			GUI.color = Color.white;
			GUI.Label(r, hint, style);
		}

		public static void Hint(string text, float seconds = 6f)
		{
			hint = text;
			hintUntil = Time.time + seconds;
		}

		public static void Log(string message)
		{
			if (log != null)
			{
				log.Msg(message);
			}
			else
			{
				MelonLogger.Msg("[Junkyard Ranger] " + message);
			}
		}

		public static float Price
		{
			get { return price != null ? price.Value : 4500f; }
		}

		public static float RainGuardPrice
		{
			get { return rainGuardPrice != null ? rainGuardPrice.Value : 60f; }
		}

		public static float SunVisorPrice
		{
			get { return sunVisorPrice != null ? sunVisorPrice.Value : 180f; }
		}

		public static Color LowerColor
		{
			get
			{
				Color c;
				return ParseColor(lowerColor != null ? lowerColor.Value : "", out c) ? c : new Color(0.23f, 0.24f, 0.25f);
			}
		}

		public static float BullBarPrice
		{
			get { return bullBarPrice != null ? bullBarPrice.Value : 350f; }
		}

		// "i4", "v8", "i6", or null for none.
		public static string EngineKind
		{
			get
			{
				switch (engineKind != null ? engineKind.Value : RangerEngine.I4)
				{
				case RangerEngine.V8:
					return "v8";
				case RangerEngine.I6_2J:
					return "i6";
				case RangerEngine.None:
					return null;
				default:
					return "i4";
				}
			}
		}

		public static bool CompleteEngine
		{
			get { return completeEngine == null || completeEngine.Value; }
		}

		public static bool Weathered
		{
			get { return weathered != null && weathered.Value; }
		}

		public static Color PaintColor
		{
			get
			{
				Color c;
				string s = paintColor != null ? paintColor.Value : "";
				return ParseColor(s, out c) ? c : new Color(0.91f, 0.91f, 0.89f);
			}
		}

		public static bool ParseColor(string s, out Color c)
		{
			c = Color.white;
			s = (s ?? "").Trim().TrimStart('#');
			int rgb;
			if (s.Length != 6 || !int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out rgb))
			{
				return false;
			}
			c = new Color(((rgb >> 16) & 255) / 255f, ((rgb >> 8) & 255) / 255f, (rgb & 255) / 255f);
			return true;
		}
	}
}
