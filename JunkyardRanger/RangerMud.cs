using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JunkyardRanger
{
	// Mud on a Ranger, like the game's trucks: driving through dirt muddies the
	// tires (cleaned again on the road), mud builds up on the body (fast in deep
	// mud, slowly on dirt), and the hose washes both off. Each Ranger has its own
	// mud, saved with it, drawn with the F100's mud layer over the paint.
	//
	// The game keeps one mud level per truck model, so a Ranger (a copy of the
	// Diamondback) would share the Diamondback's: while you're in a Ranger, or
	// hosing one, the game's own levels are left as they were.
	public class RangerMud : MonoBehaviour
	{
		// 0..1 each: the tires' mud (the game's "_Blend") and the body's (the mud
		// layer's opacity).
		public float tires;
		public float body;

		private Material layer;
		private readonly List<Material> tireMaterials = new List<Material>();
		private float nextCheck;
		private float nextWash;
		private float shownBody = -1f;

		private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

		// The body parts' mud layer: one per Ranger, so it's its own mud.
		public Material Layer()
		{
			if (layer == null)
			{
				Material template = GameMaterials.Find("Tex_RGB_23Mud");
				if (template == null)
				{
					return null;
				}
				layer = new Material(template);
				layer.name = "Ranger_Mud";
				shownBody = -1f;
			}
			return layer;
		}

		private void Start()
		{
			// The tires the Diamondback copy has (stock or bought, fitted or not).
			foreach (Transform t in GetComponentsInChildren<Transform>(true))
			{
				if (t.name != "WHEEL_HOLDER")
				{
					continue;
				}
				foreach (Renderer r in t.GetComponentsInChildren<Renderer>(true))
				{
					if (r.sharedMaterial != null && r.sharedMaterial.HasProperty("_Blend"))
					{
						tireMaterials.Add(r.material);
					}
				}
			}
		}

		private void Update()
		{
			if (Time.time >= nextCheck)
			{
				nextCheck = Time.time + 1f;
				Drive();
			}
			if (Time.time >= nextWash && Hosing(this))
			{
				nextWash = Time.time + 0.1f;
				tires = Mathf.Max(0f, tires - 0.03f);
				body = Mathf.Max(0f, body - 0.03f);
			}
		}

		// The game may set the tires' mud from its own levels (its Diamondback's);
		// ours go on after it, every frame.
		private void LateUpdate()
		{
			foreach (Material m in tireMaterials)
			{
				if (m != null)
				{
					m.SetFloat("_Blend", tires);
				}
			}
			if (layer != null && body != shownBody)
			{
				Color c = layer.color;
				c.a = body;
				layer.color = c;
				shownBody = body;
			}
		}

		// Once a second while it's moving, like the game: what's under it.
		private void Drive()
		{
			if (Speed() <= 0.5f)
			{
				return;
			}
			// The terrain it's on (there can be more than one).
			float[,,] a = null;
			foreach (Terrain terrain in Terrain.activeTerrains)
			{
				TerrainData data = terrain != null ? terrain.terrainData : null;
				if (data == null)
				{
					continue;
				}
				Vector3 local = transform.position - terrain.transform.position;
				int x = (int)(local.x / data.size.x * data.alphamapWidth);
				int z = (int)(local.z / data.size.z * data.alphamapHeight);
				if (x >= 0 && z >= 0 && x < data.alphamapWidth && z < data.alphamapHeight)
				{
					a = data.GetAlphamaps(x, z, 1, 1);
					break;
				}
			}
			if (a == null)
			{
				return;
			}
			int layers = a.GetLength(2);
			Func<int, bool> on = i => i < layers && a[0, 0, i] > 0.2f;
			bool deepMud = on(10) || on(17);
			if (on(5) || on(9) || deepMud)
			{
				tires = Mathf.Min(1f, tires + 0.2f);
				body = Mathf.Min(1f, body + (deepMud ? 0.05f : 0.01f));
			}
			else if (on(4) || on(11))
			{
				tires = Mathf.Max(0f, tires - 0.2f);
			}
		}

		private FieldInfo speedField;

		private float Speed()
		{
			Component car = GetComponent("car");
			if (car == null)
			{
				return 0f;
			}
			if (speedField == null)
			{
				speedField = AccessTools.Field(car.GetType(), "speed");
			}
			return speedField != null ? (float)speedField.GetValue(car) : 0f;
		}

		// --- Saving: an item on the end of the paint ("/Mud=tires,body"), which
		// older versions skip. ---

		public string Save()
		{
			return "Mud=" + tires.ToString("0.###", Inv) + "," + body.ToString("0.###", Inv);
		}

		public void Load(string paint)
		{
			if (string.IsNullOrEmpty(paint))
			{
				return;
			}
			foreach (string item in paint.Split('/'))
			{
				if (!item.StartsWith("Mud=", StringComparison.Ordinal))
				{
					continue;
				}
				string[] v = item.Substring(4).Split(',');
				float t, b;
				if (v.Length >= 2 && float.TryParse(v[0], NumberStyles.Float, Inv, out t) && float.TryParse(v[1], NumberStyles.Float, Inv, out b))
				{
					tires = Mathf.Clamp01(t);
					body = Mathf.Clamp01(b);
				}
			}
		}

		// --- Keeping the game's own levels to the game's trucks. ---

		private static FieldInfo fpsField, hitField, hoseField, hoseTankField, fpshoseField, gdField;
		private static readonly string[] Levels = { "dirty", "dirty2", "dirtyF", "dirty2F", "dirty250", "dirty2502" };
		private static FieldInfo[] levelFields;
		private static float[] kept;
		private static Component interactor;
		private static bool redrawing;

		public static void Patch(HarmonyLib.Harmony harmony)
		{
			Type gd = AccessTools.TypeByName("GroundDetect");
			Type inter = AccessTools.TypeByName("Interactor");
			if (gd == null || inter == null)
			{
				RangerMod.Log("The game's mud wasn't found: Rangers won't get muddy.");
				return;
			}
			fpsField = AccessTools.Field(gd, "FPSt");
			hitField = AccessTools.Field(inter, "hit");
			hoseField = AccessTools.Field(inter, "holdingHose");
			hoseTankField = AccessTools.Field(inter, "holdingHoseTank");
			fpshoseField = AccessTools.Field(inter, "fpshose");
			gdField = AccessTools.Field(inter, "gd");
			levelFields = new FieldInfo[Levels.Length];
			for (int i = 0; i < Levels.Length; i++)
			{
				levelFields[i] = AccessTools.Field(gd, Levels[i]);
			}
			MethodInfo check = AccessTools.Method(gd, "CheckTexture");
			MethodInfo wash = AccessTools.Method(gd, "WashDirt");
			MethodInfo update = AccessTools.Method(inter, "Update");
			BindingFlags f = BindingFlags.Static | BindingFlags.NonPublic;
			if (check != null)
			{
				harmony.Patch(check, prefix: new HarmonyMethod(typeof(RangerMud).GetMethod("CheckPrefix", f)), postfix: new HarmonyMethod(typeof(RangerMud).GetMethod("CheckPostfix", f)));
			}
			if (wash != null && update != null)
			{
				harmony.Patch(update, prefix: new HarmonyMethod(typeof(RangerMud).GetMethod("InteractorPrefix", f)));
				harmony.Patch(wash, prefix: new HarmonyMethod(typeof(RangerMud).GetMethod("WashPrefix", f)));
			}
		}

		public static void Reset()
		{
			interactor = null;
		}

		private static float[] Read(object gd)
		{
			float[] v = new float[levelFields.Length];
			for (int i = 0; i < v.Length; i++)
			{
				v[i] = levelFields[i] != null ? (float)levelFields[i].GetValue(gd) : 0f;
			}
			return v;
		}

		private static void Write(object gd, float[] v)
		{
			for (int i = 0; i < v.Length; i++)
			{
				if (levelFields[i] != null)
				{
					levelFields[i].SetValue(gd, v[i]);
				}
			}
		}

		// Driving a Ranger: the game checks the ground for the Diamondback's mud.
		private static bool InRanger(object gd)
		{
			Transform fps = fpsField != null ? fpsField.GetValue(gd) as Transform : null;
			return fps != null && fps.parent != null && fps.parent.GetComponentInParent<RangerBody>() != null;
		}

		private static void CheckPrefix(object __instance, out float[] __state)
		{
			__state = null;
			try
			{
				if (InRanger(__instance))
				{
					__state = Read(__instance);
				}
			}
			catch (Exception)
			{
			}
		}

		private static void CheckPostfix(object __instance, float[] __state)
		{
			if (__state == null)
			{
				return;
			}
			try
			{
				Write(__instance, __state);
				// It may have drawn the new levels on the Diamondback: put them back.
				redrawing = true;
				AccessTools.Method(__instance.GetType(), "WashDirt").Invoke(__instance, new object[] { 1 });
			}
			catch (Exception)
			{
			}
			finally
			{
				redrawing = false;
			}
		}

		// Hosing: the game lowers the Diamondback's levels and then draws them
		// (WashDirt). Hosing a Ranger (whose hidden Diamondback colliders are what
		// the hose hits), those are put back first.
		private static void InteractorPrefix(Component __instance)
		{
			interactor = __instance;
			try
			{
				object gd = gdField != null ? gdField.GetValue(__instance) : null;
				kept = gd != null ? Read(gd) : null;
			}
			catch (Exception)
			{
				kept = null;
			}
		}

		private static void WashPrefix(object __instance, int vehNum)
		{
			if (vehNum != 1 || redrawing || kept == null || interactor == null)
			{
				return;
			}
			try
			{
				if (HitRanger() != null)
				{
					Write(__instance, kept);
				}
			}
			catch (Exception)
			{
			}
		}

		private static RangerBody HitRanger()
		{
			if (interactor == null || hitField == null)
			{
				return null;
			}
			RaycastHit hit = (RaycastHit)hitField.GetValue(interactor);
			return hit.collider != null ? hit.collider.GetComponentInParent<RangerBody>() : null;
		}

		// Whether the player's hose is spraying this Ranger.
		private static bool Hosing(RangerMud mud)
		{
			if (interactor == null)
			{
				return false;
			}
			bool hose = hoseField != null && (bool)hoseField.GetValue(interactor);
			bool tank = hoseTankField != null && (bool)hoseTankField.GetValue(interactor);
			if (!hose && !tank)
			{
				return false;
			}
			GameObject fpshose = fpshoseField != null ? fpshoseField.GetValue(interactor) as GameObject : null;
			if (fpshose == null || fpshose.transform.childCount == 0 || !fpshose.transform.GetChild(0).gameObject.activeSelf)
			{
				return false;
			}
			RangerBody hit = HitRanger();
			return hit != null && hit.gameObject == mud.gameObject;
		}
	}
}
