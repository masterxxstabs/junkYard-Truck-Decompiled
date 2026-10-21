using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JunkyardRanger
{
	// What the Ranger's accessories do once they're on:
	//   Light bar    its two work lamps light up while it's switched on (the light
	//                bar key; every Ranger's bar at once)
	//   Exhaust      the truck pulls 8% harder: after the game's car.control()
	//                sets the wheels' drive torque for this frame, it's raised
	internal static class RangerUpgrades
	{
		public const float ExhaustPull = 1.08f;

		public static bool LightBarOn;

		private static float nextLights;

		// Lamps follow the switch and whether the bar is on the truck (its slot
		// shows), checked a few times a second.
		public static void UpdateLights()
		{
			if (Time.time < nextLights)
			{
				return;
			}
			nextLights = Time.time + 0.25f;
			foreach (RangerBody body in Object.FindObjectsOfType<RangerBody>())
			{
				Transform slot = Find(body.transform, "lightbar_e");
				if (slot == null)
				{
					continue;
				}
				Renderer r = slot.GetComponent<Renderer>();
				bool on = LightBarOn && r != null && r.enabled;
				foreach (Transform child in slot)
				{
					if (child.name != RangerAccessories.LampName)
					{
						continue;
					}
					Light light = child.GetComponent<Light>();
					if (light != null && light.enabled != on)
					{
						light.enabled = on;
					}
				}
				if (r != null)
				{
					SetLens(r, LightBarOn);
				}
			}
		}

		// The lens glows while the bar's switched on.
		private static void SetLens(Renderer r, bool on)
		{
			foreach (Material m in r.sharedMaterials)
			{
				if (m == null || !m.name.Contains("LightBarLens") || !m.HasProperty("_EmissionColor"))
				{
					continue;
				}
				m.EnableKeyword("_EMISSION");
				m.SetColor("_EmissionColor", on ? new Color(1.6f, 1.65f, 1.7f) : new Color(0.06f, 0.06f, 0.06f));
			}
		}

		public static void Reset()
		{
			cars.Clear();
		}

		private static Transform Find(Transform root, string name)
		{
			foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
			{
				if (t.name == name)
				{
					return t;
				}
			}
			return null;
		}

		// --- The exhaust. ---

		private class CarInfo
		{
			public Renderer exhaust;      // the exhaust slot's look (shows when fitted)
			public bool ranger;
			public float nextCheck;
		}

		private static readonly Dictionary<int, CarInfo> cars = new Dictionary<int, CarInfo>();
		private static FieldInfo[] wheels;
		private static PropertyInfo motorTorque;

		public static void Patch(HarmonyLib.Harmony harmony)
		{
			Type car = AccessTools.TypeByName("car");
			MethodInfo control = car != null ? AccessTools.Method(car, "control") : null;
			if (control == null)
			{
				RangerMod.Log("The game's car.control wasn't found: the performance exhaust won't add power.");
				return;
			}
			wheels = new[]
			{
				AccessTools.Field(car, "WheelRearRight"), AccessTools.Field(car, "WheelRearLeft"),
				AccessTools.Field(car, "WheelFrontLeft"), AccessTools.Field(car, "WheelFrontRight")
			};
			harmony.Patch(control, postfix: new HarmonyMethod(typeof(RangerUpgrades).GetMethod("ControlPostfix", BindingFlags.Static | BindingFlags.NonPublic)));
		}

		private static void ControlPostfix(Component __instance)
		{
			try
			{
				if (!Fitted(__instance))
				{
					return;
				}
				foreach (FieldInfo f in wheels)
				{
					object wheel = f != null ? f.GetValue(__instance) : null;
					if (wheel == null)
					{
						continue;
					}
					if (motorTorque == null)
					{
						motorTorque = wheel.GetType().GetProperty("motorTorque");
					}
					float t = (float)motorTorque.GetValue(wheel, null);
					if (t != 0f)
					{
						motorTorque.SetValue(wheel, t * ExhaustPull, null);
					}
				}
			}
			catch (Exception e)
			{
				if (!warned)
				{
					warned = true;
					RangerMod.Log("Performance exhaust: " + e.Message);
				}
			}
		}

		private static bool warned;

		private static bool Fitted(Component car)
		{
			CarInfo info;
			int id = car.GetInstanceID();
			if (!cars.TryGetValue(id, out info))
			{
				info = new CarInfo();
				cars[id] = info;
			}
			if (Time.time >= info.nextCheck)
			{
				info.nextCheck = Time.time + 1f;
				info.ranger = car.GetComponent<RangerBody>() != null;
				Transform slot = info.ranger ? Find(car.transform, "rexhaust_e") : null;
				info.exhaust = slot != null ? slot.GetComponent<Renderer>() : null;
			}
			return info.ranger && info.exhaust != null && info.exhaust.enabled;
		}
	}
}
