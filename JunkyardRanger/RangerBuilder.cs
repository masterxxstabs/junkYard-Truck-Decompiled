using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using EngineCloner;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JunkyardRanger
{
	// What a saved Ranger needs to come back.
	internal class RangerState
	{
		public string id;
		public Vector3 position;
		public Quaternion rotation;
		public string paint;
		public string gameEngine;   // "i4"/"v8"/"i6" if the game's own engine was in it
		public string snapshot;
	}

	internal static class RangerBuilder
	{
		// The Diamondback's body: hidden from view (renderers stay on, so the game's
		// own logic is unchanged; colliders, doors, hood and lights all still work).
		private static readonly string[] Panels = { "body_low", "cabin_low" };

		// A bought Ranger waiting for you to get out of a vehicle, with the engine
		// chosen when it was bought.
		private struct Delivery
		{
			public Vector3 near;
			public string engine;
		}

		private static readonly List<Delivery> pending = new List<Delivery>();
		private static float nextTry;
		private static float nextSpawn;

		public static void Reset()
		{
			pending.Clear();
			RangerModel.Reset();
			RangerAccessories.Reset();
		}

		public static void Update()
		{
			if (pending.Count == 0 || Time.time < nextTry)
			{
				return;
			}
			nextTry = Time.time + 1f;
			if (VehicleApi.PlayerInVehicle())
			{
				return;
			}
			Delivery d = pending[0];
			pending.RemoveAt(0);
			Deliver(d.near, d.engine, "Your 2004 Ford Ranger has been delivered.");
		}

		// Bought: a new Ranger on open ground near where the shop delivered it. Not
		// while you're sitting in a vehicle (the copy would include you): then it
		// comes when you get out.
		public static void Order(Vector3 near)
		{
			string engine = RangerMod.EngineKind;
			if (VehicleApi.PlayerInVehicle())
			{
				pending.Add(new Delivery { near = near, engine = engine });
				RangerMod.Hint("Your Ranger will be delivered when you get out of the vehicle.");
				return;
			}
			Deliver(near, engine, "Your 2004 Ford Ranger has been delivered.");
		}

		// The spawn key: a free Ranger on open ground in front of you, with nothing
		// in the engine bay, to build your own engine into.
		public static void Spawn()
		{
			if (Time.time < nextSpawn)
			{
				return;
			}
			nextSpawn = Time.time + 2f;
			if (VehicleApi.PlayerInVehicle())
			{
				RangerMod.Hint("Get out of the vehicle to spawn a Ranger.");
				return;
			}
			Camera cam = Camera.main;
			if (cam == null || VehicleApi.Original("car") == null)
			{
				RangerMod.Hint("Rangers can only be spawned in the level.");
				return;
			}
			Vector3 ahead = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
			if (ahead.sqrMagnitude < 0.01f)
			{
				ahead = Vector3.ProjectOnPlane(cam.transform.up, Vector3.up);   // looking straight down
			}
			Deliver(cam.transform.position + ahead.normalized * 7f, null, "Here's your Ranger, with an empty engine bay.");
		}

		// engine: "i4"/"v8"/"i6" for a new engine bolted in, or null for none.
		private static void Deliver(Vector3 near, string engine, string done)
		{
			GameObject original = VehicleApi.Original("car");
			if (original == null)
			{
				RangerMod.Log("Couldn't deliver the Ranger: there's no Diamondback in this level to build it on.");
				RangerMod.Hint("Couldn't deliver the Ranger here (see the log).");
				return;
			}
			string why;
			if (!RangerModel.Load(out why))
			{
				RangerMod.Log("Couldn't deliver the Ranger: " + why);
				RangerMod.Hint("Couldn't deliver the Ranger: its body files are missing (see the log).");
				return;
			}
			Vector3 position;
			if (!VehicleApi.FindGroundNear(original, near, out position))
			{
				position = near + Vector3.up * 1.5f;
			}
			// Side-on to the player, so you see it arrive.
			Camera cam = Camera.main;
			Vector3 toPlayer = cam != null ? Vector3.ProjectOnPlane(cam.transform.position - position, Vector3.up) : Vector3.back;
			Vector3 forward = toPlayer.sqrMagnitude > 0.01f ? Vector3.Cross(Vector3.up, toPlayer.normalized) : Vector3.forward;
			GameObject ranger = Build(original, position, Quaternion.LookRotation(forward, Vector3.up), null, engine);
			if (ranger != null)
			{
				RangerMod.Hint(done);
			}
		}

		// A Ranger: a copy of the Diamondback in the Ranger's body. New (state null,
		// with a new engine of that kind, or none) or as saved.
		public static GameObject Build(GameObject original, Vector3 position, Quaternion rotation, RangerState state, string newEngine = null)
		{
			string why;
			if (!RangerModel.Load(out why))
			{
				RangerMod.Log("Can't build a Ranger: " + why);
				return null;
			}
			GameObject copy = VehicleApi.CopyInactive(original);
			try
			{
				int engines = VehicleApi.StripEngines(copy, original);
				RangerBody body = copy.AddComponent<RangerBody>();
				body.id = state != null ? state.id : NewId();
				body.LoadPaint(state != null ? state.paint : null);
				HidePanels(copy);
				RangerModel.Dress(copy, body);
				RangerAccessories.FitSlots(copy, body);
				MoveFuelInlet(copy, body);
				if (state != null && !string.IsNullOrEmpty(state.snapshot))
				{
					VehicleApi.ApplySnapshot(copy, state.snapshot, Skip);
				}
				else
				{
					MakeNew(copy);
				}
				VehicleApi.Activate(copy, original, position, rotation);
				VehicleApi.Tag(copy, body.id);
				string kind = state == null ? newEngine : null;
				if (kind != null)
				{
					GameObject block = VehicleApi.MountNewEngine(copy, kind, RangerMod.CompleteEngine, true);
					RangerMod.Log("New Ranger " + body.id + (block != null ? " with a new " + block.name + "." : ": couldn't fit the " + kind + " engine; it's delivered without one."));
				}
				else if (state != null && state.gameEngine != null)
				{
					RemountGameEngine(copy, state.gameEngine);
				}
				else if (state == null)
				{
					RangerMod.Log("New Ranger " + body.id + " without an engine.");
				}
				if (engines > 0 && state == null)
				{
					RangerMod.Log("(The Diamondback's own engine wasn't copied.)");
				}
				return copy;
			}
			catch (Exception e)
			{
				RangerMod.Log("Couldn't build the Ranger: " + e);
				Object.Destroy(copy);
				return null;
			}
		}

		// The Ranger's own body parts aren't part of the Diamondback's state.
		public static bool Skip(Transform t)
		{
			return t.GetComponent<RangerPart>() != null;
		}

		private static void HidePanels(GameObject copy)
		{
			int hidden = 0;
			foreach (string name in Panels)
			{
				Transform t = copy.transform.Find(name);
				if (t == null)
				{
					RangerMod.Log("The Diamondback has no " + name + "; its body may show through.");
					continue;
				}
				foreach (Renderer r in t.GetComponentsInChildren<Renderer>(true))
				{
					if (r.GetComponentInParent<RangerPart>() == null)
					{
						r.forceRenderingOff = true;
						hidden++;
					}
				}
			}
			RangerMod.Log("Hid " + hidden + " of the Diamondback's body meshes.");
		}

		// The Ranger's filler is on the driver's side (the F100 body's), the
		// Diamondback's on the right: move the game's fuel inlet trigger and its fuel
		// lid there, and hang the cap on the lid so it opens with it.
		private static void MoveFuelInlet(GameObject copy, RangerBody body)
		{
			Transform bodyLow = copy.transform.Find("body_low");
			if (bodyLow == null || RangerModel.FuelCap == Vector3.zero)
			{
				return;
			}
			Vector3 at = copy.transform.TransformPoint(RangerModel.FuelCap);
			Transform inlet = bodyLow.Find("gastankinput");
			Transform lid = bodyLow.Find("cap_low");
			if (inlet != null)
			{
				inlet.position = at - copy.transform.right * 0.02f;
			}
			if (lid != null)
			{
				Quaternion turn = copy.transform.rotation * Quaternion.Euler(0f, 180f, 0f) * Quaternion.Inverse(copy.transform.rotation);
				lid.SetPositionAndRotation(at, turn * lid.rotation);
				if (body.gasDoor != null)
				{
					body.gasDoor.SetParent(lid, true);
				}
			}
		}

		// A new truck: a full tank, fresh fluids, every fitted part new, a hood, and
		// stock wheels on any corner that has none.
		private static void MakeNew(GameObject copy)
		{
			RangerAccessories.UnfitAll(copy);
			Component car = copy.GetComponent("car");
			if (car != null)
			{
				SetField(car, "fuel", 1300f);
				SetField(car, "coolantLevel", 100f);
				SetField(car, "temperature", 60f);
			}
			foreach (durability d in copy.GetComponentsInChildren<durability>(true))
			{
				Renderer r = d.GetComponent<Renderer>();
				if (r != null && r.enabled && d.health > 0f && d.health < 100f)
				{
					d.health = 100f;
				}
			}
			Transform hood = copy.transform.Find("cabin_low/hood_low1");
			if (hood != null)
			{
				hood.gameObject.SetActive(true);
			}
			int fitted = 0;
			foreach (Transform holder in copy.GetComponentsInChildren<Transform>(true))
			{
				if (holder.name != "WHEEL_HOLDER")
				{
					continue;
				}
				bool hasTire = false;
				foreach (Transform part in holder)
				{
					if (part.gameObject.activeSelf && part.name.StartsWith("tire_", StringComparison.Ordinal))
					{
						hasTire = true;
					}
				}
				if (hasTire)
				{
					continue;
				}
				Transform rim = holder.Find("standardrim");
				Transform tire = holder.Find("tire_standard");
				if (rim != null && tire != null)
				{
					rim.gameObject.SetActive(true);
					tire.gameObject.SetActive(true);
					durability disk = holder.parent != null ? holder.parent.GetComponent<durability>() : null;
					if (disk != null)
					{
						disk.health = 100f;
					}
					fitted++;
				}
			}
			if (fitted > 0)
			{
				RangerMod.Log("Put stock wheels on " + fitted + " corner(s) that had none.");
			}
		}

		private static void RemountGameEngine(GameObject ranger, string kind)
		{
			GameObject block = VehicleApi.LooseGameEngine(kind);
			if (block == null)
			{
				RangerMod.Log("The game's " + kind + " engine was in a Ranger when you saved, but it's not loose now; leaving it where the game put it.");
				return;
			}
			if (VehicleApi.MountEngine(block, ranger))
			{
				RangerMod.Log("Put the game's " + block.name + " back in Ranger " + VehicleApi.TagOf(ranger) + ".");
			}
		}

		private static string NewId()
		{
			return "ranger-" + DateTime.UtcNow.Ticks.ToString("x", CultureInfo.InvariantCulture).Substring(6);
		}

		private static void SetField(object o, string name, object value)
		{
			FieldInfo f = o.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (f != null && f.FieldType.IsInstanceOfType(value))
			{
				f.SetValue(o, value);
			}
		}
	}

	// On every Ranger: its id (for saves) and paint. Each paintable part has its
	// own paint layer, like the game's trucks, which paint per panel.
	public class RangerBody : MonoBehaviour
	{
		// The paint groups: the body (cab, bed, front end), the hood, the doors, the
		// tailgate, the two-tone's lower color (below the line along the sides, and
		// the bumper), and the sun visor.
		public static readonly string[] Groups = { "Body", "Hood", "DoorD", "DoorP", "Tailgate", "Lower", "SunVisor" };

		public string id;
		public Transform gasDoor;

		private readonly Dictionary<string, Color> colors = new Dictionary<string, Color>();
		private readonly Dictionary<string, Vector2> finish = new Dictionary<string, Vector2>(); // metallic, smoothness
		private readonly Dictionary<string, Material> materials = new Dictionary<string, Material>();

		private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

		public static string GroupOf(string part)
		{
			if (part.EndsWith("Lower", StringComparison.Ordinal) || part == "Bumper")
			{
				return "Lower";
			}
			return Array.IndexOf(Groups, part) >= 0 ? part : "Body";
		}

		// The paint layer for one of the body's parts (shared by the parts of a group).
		public Material PaintMaterial(string part, Material template, Texture2D paintTexture, bool weathered)
		{
			string group = GroupOf(part);
			Material m;
			if (materials.TryGetValue(group, out m) && m != null)
			{
				return m;
			}
			m = template != null ? new Material(template) : new Material(GameMaterials.Standard() ?? new Material(Shader.Find("Standard")));
			m.name = "Ranger_Paint_" + group;
			// Paint shows wherever the paint mask is (no leftover rust or dirt from
			// whichever copy of the F100's layer this came from).
			if (m.HasProperty("_Cutoff"))
			{
				m.SetFloat("_Cutoff", 0.5f);
			}
			if (m.HasProperty("_RustIntensity"))
			{
				m.SetFloat("_RustIntensity", 0f);
			}
			if (!weathered && paintTexture != null)
			{
				m.mainTexture = paintTexture;
				if (m.HasProperty("_BumpMap"))
				{
					m.SetTexture("_BumpMap", null);
				}
			}
			materials[group] = m;
			Apply(group);
			return m;
		}

		public void SetPaint(string group, Color color, float metallic, float smoothness)
		{
			colors[group] = color;
			finish[group] = new Vector2(metallic, smoothness);
			Apply(group);
		}

		private void Apply(string group)
		{
			Material m;
			if (!materials.TryGetValue(group, out m) || m == null)
			{
				return;
			}
			Color c;
			Vector2 f;
			if (!colors.TryGetValue(group, out c))
			{
				c = DefaultColor(group);
			}
			if (!finish.TryGetValue(group, out f))
			{
				f = new Vector2(0.25f, 0.62f);
			}
			m.color = new Color(c.r, c.g, c.b, 1f);
			if (m.HasProperty("_Metallic"))
			{
				m.SetFloat("_Metallic", f.x);
			}
			if (m.HasProperty("_Glossiness"))
			{
				m.SetFloat("_Glossiness", f.y);
			}
		}

		// "Body=r,g,b,m,s/Hood=..." (missing groups get the default color).
		// A new Ranger's colors: the two-tone's lower color below the line, the
		// paint color everywhere else.
		private static Color DefaultColor(string group)
		{
			return group == "Lower" ? RangerMod.LowerColor : RangerMod.PaintColor;
		}

		public void LoadPaint(string saved)
		{
			foreach (string g in Groups)
			{
				colors[g] = DefaultColor(g);
				finish[g] = new Vector2(0.25f, 0.62f);
			}
			if (string.IsNullOrEmpty(saved))
			{
				return;
			}
			foreach (string item in saved.Split('/'))
			{
				int eq = item.IndexOf('=');
				if (eq <= 0)
				{
					continue;
				}
				string[] v = item.Substring(eq + 1).Split(',');
				if (v.Length < 5)
				{
					continue;
				}
				try
				{
					string g = item.Substring(0, eq);
					colors[g] = new Color(F(v[0]), F(v[1]), F(v[2]));
					finish[g] = new Vector2(F(v[3]), F(v[4]));
				}
				catch (FormatException)
				{
				}
			}
			// Saved before the sun visor had its own paint: it was the body's.
			if (saved.IndexOf("SunVisor=", StringComparison.Ordinal) < 0)
			{
				colors["SunVisor"] = colors["Body"];
				finish["SunVisor"] = finish["Body"];
			}
			foreach (string g in Groups)
			{
				Apply(g);
			}
		}

		public string SavePaint()
		{
			List<string> parts = new List<string>();
			foreach (string g in Groups)
			{
				Color c = colors.ContainsKey(g) ? colors[g] : DefaultColor(g);
				Vector2 f = finish.ContainsKey(g) ? finish[g] : new Vector2(0.25f, 0.62f);
				parts.Add(g + "=" + S(c.r) + "," + S(c.g) + "," + S(c.b) + "," + S(f.x) + "," + S(f.y));
			}
			return string.Join("/", parts.ToArray());
		}

		private static float F(string s)
		{
			return float.Parse(s, NumberStyles.Float, Inv);
		}

		private static string S(float v)
		{
			return v.ToString("0.####", Inv);
		}
	}
}
