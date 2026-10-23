using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EngineCloner
{
	// Why won't it run? An on-screen report (F5) on the vehicle you're in, or
	// looking at, and its engine; also written to the log each time it's opened
	// (and with Shift+F5), so it can be sent in. The log also gets a line whenever
	// an engine's canRun changes, and whenever the key is turned.
	//
	// The report states facts, not the game's every rule: what the engine and the
	// vehicle point at (and anything that points into a DIFFERENT vehicle or engine,
	// which is what this mod's rewiring is meant to rule out), the parts at 0%, the
	// fuel, battery and flags the game checks.
	internal static class Diagnostics
	{
		public static bool open;
		private static string text = "";
		private static float nextBuild;
		private static Vector2 scroll;
		private static GUIStyle style;

		private const BindingFlags All = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		public static void Update(KeyCode key)
		{
			if (key == KeyCode.None || !Input.GetKeyDown(key))
			{
				if (open && Time.unscaledTime >= nextBuild)
				{
					nextBuild = Time.unscaledTime + 1.5f;
					text = Report(false);
				}
				return;
			}
			bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			if (!shift)
			{
				open = !open;
			}
			if (open || shift)
			{
				text = Report(true);
				EngineClonerMod.Log("Diagnostics report:\n" + text);
				nextBuild = Time.unscaledTime + 0.5f;
			}
		}

		public static void Draw()
		{
			if (!open)
			{
				return;
			}
			if (style == null)
			{
				style = new GUIStyle(GUI.skin.label);
				style.fontSize = Mathf.Max(11, Screen.height / 75);
				style.normal.textColor = Color.white;
				style.richText = true;
				style.wordWrap = true;
			}
			Rect r = new Rect(10f, 10f, Mathf.Min(Screen.width - 20f, 820f), Screen.height - 20f);
			GUI.color = new Color(0f, 0f, 0f, 0.8f);
			GUI.DrawTexture(r, Texture2D.whiteTexture);
			GUI.color = Color.white;
			GUILayout.BeginArea(new Rect(r.x + 8f, r.y + 6f, r.width - 16f, r.height - 12f));
			scroll = GUILayout.BeginScrollView(scroll);
			GUILayout.Label(text, style);
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		// --- The report. ---

		private static string Report(bool refresh)
		{
			StringBuilder sb = new StringBuilder();
			try
			{
				Interactor interactor = Object.FindObjectOfType<Interactor>();
				GameObject vehicle = Target(interactor);
				sb.Append("<b>ENGINE CLONER DIAGNOSTICS</b>  (F5 close · Shift+F5 write to log)\n");
				if (vehicle == null)
				{
					sb.Append("Sit in a vehicle, or look at one (or an engine), to see its report.\n");
				}
				else
				{
					VehicleSection(sb, vehicle, refresh);
				}
				GlobalSection(sb, interactor);
			}
			catch (Exception e)
			{
				sb.Append("\n(report failed: " + e + ")");
			}
			return sb.ToString();
		}

		// The vehicle you're sitting in, else the one (or the engine's) you look at.
		private static GameObject Target(Interactor interactor)
		{
			if (interactor != null && interactor.drivingCar && interactor.person != null && interactor.person.transform.parent != null)
			{
				GameObject v = VehicleCloner.FindVehicleRoot(interactor.person.transform.parent);
				if (v != null)
				{
					return v;
				}
			}
			Camera cam = Camera.main;
			RaycastHit hit;
			if (cam != null && Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 8f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
			{
				GameObject v = VehicleCloner.FindVehicleRoot(hit.collider.transform);
				if (v != null)
				{
					return v;
				}
				GameObject block = BlockOf(hit.collider.transform);
				if (block != null)
				{
					return block;
				}
			}
			return null;
		}

		private static void VehicleSection(StringBuilder sb, GameObject target, bool refresh)
		{
			GameObject vehicle = VehicleCloner.VehicleType(target) != null ? target : null;
			GameObject block = vehicle != null ? VehicleApi.MountedEngine(vehicle) : target;
			if (vehicle != null)
			{
				Component car = vehicle.GetComponent(VehicleCloner.VehicleType(vehicle));
				sb.Append("\n<b>VEHICLE</b> ").Append(Name(vehicle)).Append(VehicleCloner.IsCopy(vehicle) ? "  (a copy)" : "  (the game's own)")
					.Append(VehicleCloner.IsWired(vehicle) ? "  · game is WIRED to it" : "  · <color=#ff8080>game is NOT wired to it</color> (get in to wire it)").Append('\n');
				sb.Append("  fuel ").Append(Get(car, "fuel")).Append(" · usingV8 ").Append(Get(car, "usingV8")).Append(" · usingI6 ").Append(Get(car, "usingI6"))
					.Append(" · car.canRun ").Append(Flag(Get(car, "canRun"))).Append(" · canCrank ").Append(Flag(Get(car, "canCrank")))
					.Append(" · controlled ").Append(Get(car, "controlled")).Append(" · coolant ").Append(Get(car, "coolantLevel")).Append('\n');
				sb.Append("  its engine scripts: ");
				foreach (string f in new[] { "enginescript", "enginescriptv8", "enginescripti6" })
				{
					object v = Field(car, f);
					if (v != null || FieldInfoOf(car, f) != null)
					{
						sb.Append(f).Append(" → ").Append(Where(v as Object, vehicle, block)).Append("  ");
					}
				}
				sb.Append('\n');
				Foreign(sb, "  vehicle scripts pointing elsewhere", vehicle, vehicle, block, true);
				if (block == null)
				{
					sb.Append("\n<b>ENGINE</b> none bolted in.\n");
					return;
				}
			}
			if (block == null)
			{
				return;
			}
			Component script = EngineScript(block);
			if (script == null)
			{
				sb.Append("\n<b>ENGINE</b> ").Append(Name(block)).Append(": no engine script.\n");
				return;
			}
			if (refresh)
			{
				MethodInfo m = script.GetType().GetMethod("Refresh", Type.EmptyTypes);
				try
				{
					if (m != null)
					{
						m.Invoke(script, null);
					}
				}
				catch (Exception e)
				{
					sb.Append("  (Refresh threw: " + (e.InnerException ?? e).Message + ")\n");
				}
			}
			EngineShop.Kind kind = EngineShop.KindOf(block);
			sb.Append("\n<b>ENGINE</b> ").Append(Name(block)).Append(kind != null ? "  kind " + kind.id : "")
				.Append(block.GetComponent<ShopEngine>() != null ? "  (bought/cloned)" : "  (the game's own)").Append('\n');
			Transform parent = block.transform.parent;
			FixedJoint joint = block.GetComponent<FixedJoint>();
			sb.Append("  parent ").Append(parent != null ? "\"" + parent.name + "\"" : "none")
				.Append(" · joint → ").Append(joint != null && joint.connectedBody != null ? Where(joint.connectedBody, vehicle, block) : "nothing")
				.Append(" · whichTruck ").Append(Get(script, "whichTruck")).Append('\n');
			if (vehicle != null && kind != null)
			{
				Transform bay = VehicleApi.Bay(vehicle, kind.game);
				if (bay != null)
				{
					sb.Append("  distance from its bay ").Append((block.transform.position - bay.position).magnitude.ToString("0.00")).Append(" m\n");
				}
			}
			sb.Append("  engine.canRun ").Append(Flag(Get(script, "canRun"))).Append(" · canCrank ").Append(Flag(Get(script, "canCrank")))
				.Append(" · fuel it sees ").Append(Get(script, "newFuelLevel")).Append(" · oil ").Append(Get(script, "newOilLevel"))
				.Append(" · battery ").Append(Get(script, "battery_cnd")).Append('\n');
			sb.Append("  it reads from: ");
			foreach (string f in new[] { "truck", "truckScript", "truckRb", "truck2", "truck2Rb", "ebtemplate", "ebtemplatev8", "ebtemplatei6", "battery_cnd_c", "transmission_cnd_c" })
			{
				if (FieldInfoOf(script, f) != null)
				{
					sb.Append(f).Append(" → ").Append(Where(Field(script, f) as Object, vehicle, block)).Append("  ");
				}
			}
			sb.Append('\n');
			// The parts the game counts as missing.
			List<string> zero = new List<string>();
			foreach (FieldInfo f in script.GetType().GetFields(All))
			{
				if (f.FieldType == typeof(float) && f.Name.EndsWith("_cnd", StringComparison.Ordinal))
				{
					float v = (float)f.GetValue(script);
					if (v <= 0f)
					{
						zero.Add(f.Name.Substring(0, f.Name.Length - 4));
					}
				}
			}
			sb.Append("  parts at 0% (missing or broken; the am_ ones are optional upgrades): ")
				.Append(zero.Count > 0 ? string.Join(", ", zero.ToArray()) : "none").Append('\n');
			if (vehicle != null)
			{
				Foreign(sb, "  engine fields pointing elsewhere", block, vehicle, block, false);
			}
		}

		// Fields of the scripts under root that point into another vehicle of the
		// same kind, or another engine block: what should never be.
		private static void Foreign(StringBuilder sb, string title, GameObject root, GameObject vehicle, GameObject block, bool skipBlock)
		{
			List<string> found = new List<string>();
			foreach (MonoBehaviour b in root.GetComponentsInChildren<MonoBehaviour>(true))
			{
				if (b == null || (skipBlock && block != null && b.transform.IsChildOf(block.transform)))
				{
					continue;
				}
				for (Type t = b.GetType(); t != null && t != typeof(MonoBehaviour); t = t.BaseType)
				{
					foreach (FieldInfo f in t.GetFields(All | BindingFlags.DeclaredOnly))
					{
						if (!typeof(Object).IsAssignableFrom(f.FieldType))
						{
							continue;
						}
						Object o = f.GetValue(b) as Object;
						if (o == null)
						{
							continue;
						}
						Transform ot = TransformOf(o);
						if (ot == null)
						{
							continue;
						}
						GameObject ov = VehicleCloner.FindVehicleRoot(ot);
						GameObject ob = BlockOf(ot);
						bool otherVehicle = ov != null && ov != vehicle && VehicleCloner.VehicleType(ov) == VehicleCloner.VehicleType(vehicle);
						bool otherBlock = ob != null && block != null && ob != block && EngineScript(ob) != null && EngineScript(block) != null && EngineScript(ob).GetType() == EngineScript(block).GetType();
						if (otherVehicle || otherBlock)
						{
							found.Add(b.GetType().Name + "." + f.Name + " → " + Where(o, vehicle, block));
						}
					}
				}
			}
			sb.Append(title).Append(": ");
			if (found.Count == 0)
			{
				sb.Append("<color=#80ff80>none</color>\n");
				return;
			}
			sb.Append("<color=#ff8080>").Append(found.Count).Append("</color>\n");
			for (int i = 0; i < found.Count && i < 25; i++)
			{
				sb.Append("    ").Append(found[i]).Append('\n');
			}
			if (found.Count > 25)
			{
				sb.Append("    ... and ").Append(found.Count - 25).Append(" more\n");
			}
		}

		// What the game's own singletons drive: the player (Interactor), the gearbox,
		// the engine sound.
		private static void GlobalSection(StringBuilder sb, Interactor interactor)
		{
			sb.Append("\n<b>GAME WIRING</b>\n");
			if (interactor != null)
			{
				sb.Append("  Interactor: drivingCar ").Append(interactor.drivingCar).Append(" · keyState ").Append(Get(interactor, "keyState"))
					.Append(" · canRun ").Append(Flag(Get(interactor, "canRun"))).Append(" · canCrank ").Append(Flag(Get(interactor, "canCrank"))).Append('\n');
				foreach (string f in new[] { "truck", "carscript", "truck2", "fcarscript", "fourcyl", "v8", "i6" })
				{
					sb.Append("    ").Append(f).Append(" → ").Append(Where(Field(interactor, f) as Object, null, null)).Append('\n');
				}
			}
			foreach (string type in new[] { "GearBox", "AudioControl", "FluidHandler" })
			{
				Type t = AccessTools.TypeByName(type);
				if (t == null)
				{
					continue;
				}
				foreach (Object o in Object.FindObjectsOfType(t))
				{
					List<string> refs = new List<string>();
					foreach (FieldInfo f in t.GetFields(All))
					{
						if (!typeof(Object).IsAssignableFrom(f.FieldType))
						{
							continue;
						}
						Object v = f.GetValue(o) as Object;
						Transform vt = v != null ? TransformOf(v) : null;
						if (vt != null && (VehicleCloner.FindVehicleRoot(vt) != null || BlockOf(vt) != null))
						{
							refs.Add(f.Name + " → " + Where(v, null, null));
						}
					}
					if (refs.Count > 0)
					{
						sb.Append("  ").Append(type).Append(" on ").Append(((Component)o).name).Append(": ").Append(string.Join(" · ", refs.Count > 8 ? refs.GetRange(0, 8).ToArray() : refs.ToArray())).Append(refs.Count > 8 ? " ..." : "").Append('\n');
					}
				}
			}
			sb.Append("  wired blocks: ").Append(EngineClonerMod.DescribeWiring()).Append('\n');
			sb.Append("  vehicles: ");
			foreach (Type t in new[] { typeof(car) })
			{
				foreach (Object o in Object.FindObjectsOfType(t))
				{
					GameObject v = ((Component)o).gameObject;
					GameObject e = VehicleApi.MountedEngine(v);
					sb.Append(Name(v)).Append(VehicleCloner.IsWired(v) ? "[wired]" : "").Append(" engine ").Append(e != null ? Name(e) : "none").Append(" · ");
				}
			}
			sb.Append('\n');
		}

		// --- Naming things. ---

		public static string Name(GameObject go)
		{
			if (go == null)
			{
				return "null";
			}
			string tag = VehicleApi.TagOf(go);
			return go.name + (tag != null ? "#" + tag : "") + "@" + go.GetInstanceID();
		}

		// Where an object lives: which vehicle, which engine, relative to the ones
		// in the report (THIS VEHICLE / THIS ENGINE / OTHER ...).
		private static string Where(Object o, GameObject vehicle, GameObject block)
		{
			if (o == null)
			{
				return "null";
			}
			Transform t = TransformOf(o);
			if (t == null)
			{
				return o.name;
			}
			GameObject b = BlockOf(t);
			GameObject v = VehicleCloner.FindVehicleRoot(t);
			string what = o is GameObject || o is Transform ? t.name : o.GetType().Name + " on " + t.name;
			string at;
			if (b != null)
			{
				at = b == block ? "<color=#80ff80>THIS ENGINE</color>" : (block != null ? "<color=#ff8080>OTHER ENGINE</color> " : "engine ") + Name(b) + (b.transform.parent != null ? " in " + Name(VehicleCloner.FindVehicleRoot(b.transform) ?? b.transform.parent.gameObject) : " (loose/stand)");
			}
			else if (v != null)
			{
				at = v == vehicle ? "<color=#80ff80>THIS VEHICLE</color>" : (vehicle != null ? "<color=#ff8080>OTHER VEHICLE</color> " : "") + Name(v);
			}
			else
			{
				at = "outside any vehicle";
			}
			return what + " (" + at + ")";
		}

		private static Transform TransformOf(Object o)
		{
			if (o is GameObject)
			{
				return ((GameObject)o).transform;
			}
			if (o is Component)
			{
				return ((Component)o).transform;
			}
			return null;
		}

		private static GameObject BlockOf(Transform t)
		{
			for (; t != null; t = t.parent)
			{
				if (EngineClonerMod.IsEngineBlock(t.gameObject))
				{
					return t.gameObject;
				}
			}
			return null;
		}

		public static Component EngineScript(GameObject block)
		{
			if (block == null)
			{
				return null;
			}
			return (Component)block.GetComponent<enginev8>() ?? (Component)block.GetComponent<enginei6>() ?? (Component)block.GetComponent<engine>() ?? block.GetComponent<Engine250>();
		}

		private static FieldInfo FieldInfoOf(object o, string name)
		{
			return o != null ? AccessTools.Field(o.GetType(), name) : null;
		}

		private static object Field(object o, string name)
		{
			FieldInfo f = FieldInfoOf(o, name);
			try
			{
				return f != null ? f.GetValue(o) : null;
			}
			catch
			{
				return null;
			}
		}

		private static string Get(object o, string name)
		{
			FieldInfo f = FieldInfoOf(o, name);
			if (f == null)
			{
				return "-";
			}
			object v = Field(o, name);
			if (v is float)
			{
				return ((float)v).ToString("0.##");
			}
			return v != null ? v.ToString() : "null";
		}

		private static string Flag(string v)
		{
			return v == "True" ? "<color=#80ff80>True</color>" : v == "False" ? "<color=#ff8080>False</color>" : v;
		}

		// --- Logging as it happens. ---

		private static readonly Dictionary<int, string> lastState = new Dictionary<int, string>();

		// After every engine Refresh (and this mod's own check on it): log when what
		// the engine says changes.
		public static void AfterRefresh(MonoBehaviour engine)
		{
			try
			{
				string state = "canRun " + Get(engine, "canRun") + ", canCrank " + Get(engine, "canCrank") + ", fuel " + Get(engine, "newFuelLevel") + ", battery " + Get(engine, "battery_cnd") + ", whichTruck " + Get(engine, "whichTruck");
				string last;
				int id = engine.GetInstanceID();
				if (lastState.TryGetValue(id, out last) && last == state)
				{
					return;
				}
				lastState[id] = state;
				Transform p = engine.transform.parent;
				GameObject v = p != null ? VehicleCloner.FindVehicleRoot(p) : null;
				EngineClonerMod.Log("[diag] " + Name(engine.gameObject) + " in " + (v != null ? Name(v) + (VehicleCloner.IsWired(v) ? " (wired)" : " (not wired)") : "no vehicle") + ": " + state + "; its truck field → " + Where(Field(engine, "truck") as Object ?? Field(engine, "truckScript") as Object, v, engine.gameObject));
			}
			catch (Exception e)
			{
				EngineClonerMod.Log("[diag] " + e.Message);
			}
		}

		public static void Reset()
		{
			lastState.Clear();
			open = false;
		}
	}

	// The key: what the player's ignition sees when it's turned.
	[HarmonyPatch(typeof(Interactor), "StartCar")]
	internal static class DiagStartCarPatch
	{
		private static void Postfix(Interactor __instance)
		{
			try
			{
				object car = AccessTools.Field(typeof(Interactor), "carscript").GetValue(__instance);
				GameObject vehicle = car is Component ? ((Component)car).gameObject : null;
				EngineClonerMod.Log("[diag] Key turned (StartCar) in " + Diagnostics.Name(vehicle) + ": Interactor.canRun " + AccessTools.Field(typeof(Interactor), "canRun").GetValue(__instance)
					+ ", canCrank " + AccessTools.Field(typeof(Interactor), "canCrank").GetValue(__instance)
					+ "; v8 → " + Diagnostics.Name(__instance.v8) + ", fourcyl → " + Diagnostics.Name(__instance.fourcyl) + ", i6 → " + Diagnostics.Name(__instance.i6)
					+ "; that truck's engine is " + (vehicle != null ? Diagnostics.Name(VehicleApi.MountedEngine(vehicle)) : "?"));
			}
			catch (Exception e)
			{
				EngineClonerMod.Log("[diag] " + e.Message);
			}
		}
	}

	// Turning the key: what it's about to act on.
	[HarmonyPatch(typeof(Interactor), "KeyStateChange")]
	internal static class DiagKeyPatch
	{
		private static void Prefix(Interactor __instance)
		{
			try
			{
				object car = AccessTools.Field(typeof(Interactor), "carscript").GetValue(__instance);
				EngineClonerMod.Log("[diag] Key (state " + AccessTools.Field(typeof(Interactor), "keyState").GetValue(__instance) + ") in "
					+ Diagnostics.Name(car is Component ? ((Component)car).gameObject : null)
					+ ": car.canRun " + AccessTools.Field(car.GetType(), "canRun").GetValue(car) + ", car.canCrank " + AccessTools.Field(car.GetType(), "canCrank").GetValue(car)
					+ ", fuel " + AccessTools.Field(car.GetType(), "fuel").GetValue(car) + ", usingV8 " + AccessTools.Field(car.GetType(), "usingV8").GetValue(car)
					+ ", usingI6 " + AccessTools.Field(car.GetType(), "usingI6").GetValue(car));
			}
			catch (Exception e)
			{
				EngineClonerMod.Log("[diag] key: " + e.Message);
			}
		}
	}
}
