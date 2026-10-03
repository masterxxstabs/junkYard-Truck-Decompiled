using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TruckPartsQOL
{
	// A handheld OBD scanner on the number keys, next to the game's own tools.
	//
	// The game's tools are Interactor's Item1..Item7 keys (hands, ratchet,
	// multimeter, depth gauge, phone, tire gauge, crowbar). Each one first calls
	// Interactor.SwitchItem() to put away whatever was in hand. The scanner does the
	// same, and a postfix on SwitchItem puts the scanner away when another tool is
	// picked.
	//
	// It reads the engine scripts (engine, enginev8, enginei6, Engine250) live: each
	// keeps a durability reference per part (fields named *_cnd_c, .health 0-100),
	// and a part is fitted when its renderer is on (the game's own check).
	internal static class ObdScanner
	{
		public static bool Equipped { get; private set; }

		private static GameObject model;
		private static MonoBehaviour engine;
		private static string targetName = "";
		private static bool switching;
		private static Vector2 panelScroll;

		public static void Toggle()
		{
			if (Equipped)
			{
				Put();
				return;
			}
			Interactor interactor = Object.FindObjectOfType<Interactor>();
			if (interactor != null)
			{
				// Put away the game's tool first, like pressing 1-7 does.
				switching = true;
				try
				{
					interactor.SwitchItem();
				}
				finally
				{
					switching = false;
				}
			}
			Equipped = true;
			ShowModel(true);
		}

		public static void Put()
		{
			Equipped = false;
			ShowModel(false);
		}

		// Called by the SwitchItem postfix.
		public static void OnGameSwitchedItem()
		{
			if (!switching && Equipped)
			{
				Put();
			}
		}

		public static void Reset()
		{
			Equipped = false;
			engine = null;
			targetName = "";
			if (model != null)
			{
				Object.Destroy(model);
			}
			model = null;
		}

		public static void Update()
		{
			if (!Equipped)
			{
				return;
			}
			KeepModelInHand();
			Camera cam = Camera.main;
			if (cam == null)
			{
				return;
			}
			RaycastHit hit;
			if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 4f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
			{
				string name;
				MonoBehaviour found = FindEngine(hit.collider.transform, out name);
				if (found != null && found != engine)
				{
					engine = found;
					targetName = name;
					panelScroll = Vector2.zero;
				}
				else if (found == null && name.Length > 0)
				{
					// A vehicle with no engine in it.
					engine = null;
					targetName = name;
				}
			}
		}

		// The engine block itself (or a part on it), else the engine mounted in the
		// vehicle that was hit.
		private static MonoBehaviour FindEngine(Transform t, out string name)
		{
			name = "";
			for (Transform p = t; p != null; p = p.parent)
			{
				MonoBehaviour e = EngineOn(p.gameObject);
				if (e != null)
				{
					GameObject vehicle = Vehicles.FindRoot(p);
					name = EngineLabel(e) + (vehicle != null ? " in " + vehicle.name : "");
					return e;
				}
			}
			GameObject root = Vehicles.FindRoot(t);
			if (root == null)
			{
				return null;
			}
			foreach (Type type in EngineTypes)
			{
				Component e = root.GetComponentInChildren(type);
				if (e != null)
				{
					name = EngineLabel((MonoBehaviour)e) + " in " + root.name;
					return (MonoBehaviour)e;
				}
			}
			name = root.name + ": no engine fitted";
			return null;
		}

		private static readonly Type[] EngineTypes = { typeof(engine), typeof(enginev8), typeof(enginei6), typeof(Engine250) };

		private static string EngineLabel(MonoBehaviour e)
		{
			if (e is enginev8)
			{
				return "V8";
			}
			if (e is enginei6)
			{
				return "Inline-6";
			}
			if (e is Engine250)
			{
				return "250 dirt bike engine";
			}
			return "4-cylinder";
		}

		private static MonoBehaviour EngineOn(GameObject go)
		{
			foreach (Type type in EngineTypes)
			{
				Component c = go.GetComponent(type);
				if (c != null)
				{
					return (MonoBehaviour)c;
				}
			}
			return null;
		}

		// --- Reading the engine. ---

		private struct Reading
		{
			public string name;
			public string key;
			public float health;
			public bool fitted;
		}

		private static readonly Dictionary<Type, List<FieldInfo>> partFields = new Dictionary<Type, List<FieldInfo>>();

		private static List<Reading> Read(MonoBehaviour e)
		{
			Type type = e.GetType();
			List<FieldInfo> fields;
			if (!partFields.TryGetValue(type, out fields))
			{
				fields = new List<FieldInfo>();
				foreach (FieldInfo f in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (f.Name.EndsWith("_cnd_c", StringComparison.Ordinal) && typeof(durability).IsAssignableFrom(f.FieldType))
					{
						fields.Add(f);
					}
				}
				partFields[type] = fields;
			}
			List<Reading> readings = new List<Reading>();
			foreach (FieldInfo f in fields)
			{
				durability d = f.GetValue(e) as durability;
				if (d == null)
				{
					continue;
				}
				Reading r;
				r.key = f.Name.Substring(0, f.Name.Length - "_cnd_c".Length);
				r.name = Pretty(r.key);
				r.health = d.health;
				Renderer renderer = d.GetComponent<Renderer>();
				r.fitted = renderer == null || renderer.enabled;
				readings.Add(r);
			}
			// Missing parts first, then worst condition first.
			readings.Sort((a, b) => a.fitted != b.fitted ? (a.fitted ? 1 : -1) : a.health.CompareTo(b.health));
			return readings;
		}

		private static object Field(MonoBehaviour e, string name)
		{
			FieldInfo f = e.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return f != null ? f.GetValue(e) : null;
		}

		// Trouble codes made up from part condition, OBD-II style.
		private static List<string> Codes(List<Reading> readings, MonoBehaviour e)
		{
			List<string> codes = new List<string>();
			foreach (Reading r in readings)
			{
				string k = r.key.ToLowerInvariant();
				if (!r.fitted)
				{
					continue;
				}
				if (k.StartsWith("piston") && !k.StartsWith("pistonbearing") && r.health < 30f)
				{
					string n = TrailingNumber(r.key);
					codes.Add("P030" + (n.Length == 1 ? n : "0") + "  Cylinder " + (n.Length > 0 ? n : "?") + " misfire (piston " + r.health.ToString("0") + "%)");
				}
				else if ((k.StartsWith("mainbearing") || k.StartsWith("pistonbearing") || k.StartsWith("cambearing")) && r.health < 20f)
				{
					codes.Add("P0325  Knock detected: " + r.name.ToLowerInvariant());
				}
				else if ((k.StartsWith("timinggear") || k == "camgear") && r.health < 20f)
				{
					codes.Add("P0016  Crank/cam timing correlation: " + r.name.ToLowerInvariant());
				}
				else if (k.StartsWith("battery") && r.health < 20f)
				{
					codes.Add("P0562  System voltage low");
				}
				else if (k == "alternator" && r.health < 10f)
				{
					codes.Add("P0620  Generator (alternator) circuit");
				}
				else if (k.StartsWith("airfilter") && r.health < 10f)
				{
					codes.Add("P0101  Air flow restricted (air filter)");
				}
				else if (k.StartsWith("headgasket") && r.health < 30f)
				{
					codes.Add("P0217  Overheat condition (head gasket)");
				}
				else if (k == "distributor" && r.health < 20f)
				{
					codes.Add("P0340  Ignition / distributor fault");
				}
				else if ((k == "carb" || k.StartsWith("intake")) && r.health < 15f)
				{
					codes.Add("P0171  System too lean (" + r.name.ToLowerInvariant() + ")");
				}
				else if (k == "oilfilter" && r.health < 10f)
				{
					codes.Add("P0521  Oil pressure range (oil filter)");
				}
			}
			object oil = Field(e, "newOilLevel");
			if (oil is float && (float)oil < 25f)
			{
				codes.Add("P0520  Oil level/pressure low");
			}
			// Same code twice (several bearings) only once.
			List<string> unique = new List<string>();
			foreach (string c in codes)
			{
				if (!unique.Contains(c))
				{
					unique.Add(c);
				}
			}
			return unique;
		}

		private static string TrailingNumber(string s)
		{
			int i = s.Length;
			while (i > 0 && char.IsDigit(s[i - 1]))
			{
				i--;
			}
			return s.Substring(i);
		}

		// "mainBearing3" -> "Main bearing 3", "intakeManEFI" -> "Intake manifold EFI".
		public static string Pretty(string key)
		{
			string s = key.Replace("am_", "aftermarket ").Replace("_", " ");
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				char prev = i > 0 ? s[i - 1] : ' ';
				bool acronymEnds = char.IsUpper(c) && char.IsUpper(prev) && i + 1 < s.Length && char.IsLower(s[i + 1]);
				// "piston1" -> "piston 1", but "F4" stays together.
				bool digitAfterWord = char.IsDigit(c) && char.IsLower(prev);
				bool boundary = i > 0 && prev != ' ' && (char.IsUpper(c) && !char.IsUpper(prev) || acronymEnds || digitAfterWord);
				if (boundary)
				{
					sb.Append(' ');
				}
				sb.Append(c);
			}
			string[] raw = sb.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			List<string> words = new List<string>();
			string suffix = "";
			for (int i = 0; i < raw.Length; i++)
			{
				string w = raw[i];
				string lower = w.ToLowerInvariant();
				// V8 bank suffixes: headD / headP.
				if (i == raw.Length - 1 && i > 0 && (w == "D" || w == "P"))
				{
					suffix = w == "D" ? " (driver side)" : " (passenger side)";
					continue;
				}
				string mapped;
				if (Words.TryGetValue(lower, out mapped))
				{
					words.Add(mapped);
				}
				else if (w.Length == 1 || w.ToUpperInvariant() == w)
				{
					words.Add(w.ToUpperInvariant()); // F4, EFI, XL
				}
				else
				{
					words.Add(lower);
				}
			}
			string result = string.Join(" ", words.ToArray()).Replace("fly wheel", "flywheel").Replace("fl wheel", "flywheel");
			result = result.Length > 0 ? char.ToUpperInvariant(result[0]) + result.Substring(1) : key;
			return result + suffix;
		}

		private static readonly Dictionary<string, string> Words = new Dictionary<string, string>
		{
			{ "man", "manifold" },
			{ "carb", "carburetor" },
			{ "dia", "disc" },
			{ "con", "converter" },
			{ "ac", "AC" },
			{ "efi", "EFI" },
			{ "pistonbearing", "piston bearing" },
			{ "headgasket", "head gasket" },
			{ "fuelpump", "fuel pump" },
			{ "fuelrail", "fuel rail" },
			{ "plugwires", "plug wires" },
			{ "oilcooler", "oil cooler" },
			{ "diff", "differential" }
		};


		// --- Screen. ---

		private static GUIStyle line;

		public static void OnGUI()
		{
			if (!Equipped)
			{
				return;
			}
			if (line == null)
			{
				line = new GUIStyle(GUI.skin.label);
				line.fontSize = 13;
				line.richText = true;
				line.wordWrap = false;
				line.margin = new RectOffset(0, 0, 0, 0);
				line.padding = new RectOffset(0, 0, 1, 1);
			}
			float width = 360f;
			Rect rect = new Rect(Screen.width - width - 20f, 60f, width, Mathf.Min(560f, Screen.height - 120f));
			GUI.color = new Color(1f, 1f, 1f, 0.92f);
			GUI.Box(rect, "");
			GUI.Box(rect, "");
			GUI.color = Color.white;
			GUILayout.BeginArea(new Rect(rect.x + 10f, rect.y + 8f, rect.width - 20f, rect.height - 16f));
			GUILayout.Label("<b><color=#ffd23f>OBD SCANNER</color></b>", line);
			if (engine == null)
			{
				GUILayout.Label(targetName.Length > 0 ? targetName : "Aim at a vehicle or an engine to link.", line);
				GUILayout.EndArea();
				return;
			}
			GUILayout.Label("<b>" + targetName + "</b>", line);
			bool canRun = Field(engine, "canRun") as bool? ?? false;
			bool canCrank = Field(engine, "canCrank") as bool? ?? false;
			bool check = Field(engine, "checkEngine") as bool? ?? false;
			GUILayout.Label("Runs: " + YesNo(canRun) + "   Cranks: " + YesNo(canCrank) + "   Check engine: " + (check ? "<color=#ff5a4a>ON</color>" : "<color=#7bd88f>off</color>") + "  <color=#8a8a8a>(last check)</color>", line);
			object oil = Field(engine, "newOilLevel");
			object torque = Field(engine, "torqueReduction");
			GUILayout.Label((oil is float ? "Oil level: " + ((float)oil).ToString("0") + "   " : "") + (torque is float && (float)torque > 0f ? "Torque lost: " + ((float)torque).ToString("0") : ""), line);

			List<Reading> readings = Read(engine);
			List<string> codes = Codes(readings, engine);
			GUILayout.Space(4f);
			GUILayout.Label("<b>Trouble codes</b>" + (codes.Count == 0 ? "  <color=#7bd88f>none</color>" : ""), line);
			foreach (string c in codes)
			{
				GUILayout.Label("<color=#ffb347>" + c + "</color>", line);
			}
			GUILayout.Space(4f);
			GUILayout.Label("<b>Parts</b> (worst first, mouse wheel scrolls)", line);
			panelScroll = GUILayout.BeginScrollView(panelScroll);
			foreach (Reading r in readings)
			{
				string value = !r.fitted ? "<color=#9a9a9a>MISSING</color>" : "<color=" + Shade(r.health) + ">" + Bar(r.health) + " " + r.health.ToString("0").PadLeft(3) + "%</color>";
				GUILayout.BeginHorizontal();
				GUILayout.Label(r.name, line, GUILayout.Width(170f));
				GUILayout.Label(value, line);
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		// Scroll the parts list with the mouse wheel while the scanner is out.
		public static void Scroll(float wheel)
		{
			if (Equipped && wheel != 0f)
			{
				panelScroll.y = Mathf.Max(0f, panelScroll.y - wheel * 200f);
			}
		}

		private static string YesNo(bool b)
		{
			return b ? "<color=#7bd88f>yes</color>" : "<color=#ff5a4a>no</color>";
		}

		private static string Shade(float health)
		{
			return health >= 60f ? "#7bd88f" : health >= 30f ? "#ffd23f" : "#ff5a4a";
		}

		private static string Bar(float health)
		{
			int filled = Mathf.Clamp(Mathf.RoundToInt(health / 10f), 0, 10);
			return new string('|', filled) + "<color=#444444>" + new string('|', 10 - filled) + "</color>";
		}

		// --- The handheld model. ---

		private static void ShowModel(bool show)
		{
			if (show && model == null)
			{
				model = BuildModel();
			}
			if (model != null)
			{
				model.SetActive(show);
			}
		}

		private static void KeepModelInHand()
		{
			Camera cam = Camera.main;
			if (model == null || cam == null || model.transform.parent == cam.transform)
			{
				return;
			}
			model.transform.SetParent(cam.transform, false);
			model.transform.localPosition = new Vector3(0.17f, -0.15f, 0.34f);
			model.transform.localRotation = Quaternion.Euler(-20f, -12f, 0f);
		}

		private static GameObject BuildModel()
		{
			GameObject root = new GameObject("TPQ_ObdScanner");
			Part(root.transform, PrimitiveType.Cube, new Vector3(0.075f, 0.13f, 0.028f), Vector3.zero, new Color(0.95f, 0.75f, 0.1f));
			Part(root.transform, PrimitiveType.Cube, new Vector3(0.058f, 0.045f, 0.004f), new Vector3(0f, 0.03f, -0.015f), new Color(0.05f, 0.12f, 0.08f));
			for (int i = 0; i < 4; i++)
			{
				Part(root.transform, PrimitiveType.Cube, new Vector3(0.014f, 0.01f, 0.006f), new Vector3(-0.022f + i * 0.0147f, -0.02f, -0.015f), new Color(0.1f, 0.1f, 0.1f));
			}
			// The cable to the OBD plug.
			Part(root.transform, PrimitiveType.Cylinder, new Vector3(0.008f, 0.06f, 0.008f), new Vector3(0f, -0.12f, 0f), new Color(0.08f, 0.08f, 0.08f));
			Part(root.transform, PrimitiveType.Cube, new Vector3(0.03f, 0.02f, 0.015f), new Vector3(0f, -0.185f, 0f), new Color(0.15f, 0.15f, 0.15f));
			root.SetActive(false);
			return root;
		}

		private static void Part(Transform parent, PrimitiveType type, Vector3 scale, Vector3 position, Color color)
		{
			GameObject go = GameObject.CreatePrimitive(type);
			Object.DestroyImmediate(go.GetComponent<Collider>());
			go.transform.SetParent(parent, false);
			go.transform.localPosition = position;
			go.transform.localScale = scale;
			Renderer r = go.GetComponent<Renderer>();
			r.material.color = color;
			r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		}
	}

	[HarmonyPatch(typeof(Interactor), "SwitchItem")]
	internal static class SwitchItemPatch
	{
		private static void Postfix()
		{
			ObdScanner.OnGameSwitchedItem();
		}
	}
}
