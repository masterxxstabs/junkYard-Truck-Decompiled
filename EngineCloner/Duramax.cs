using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace EngineCloner
{
	// The 8.3L Duramax: a big V8 built on the game's own V8, so it fits the same
	// engine bays, takes the same parts (heads, pistons, intake, turbos...),
	// mounts, oil and gauges, and is bought, bolted in and saved like our other
	// engines. Its own look (a charcoal block with gloss black valve covers and
	// intake) and more torque: the game sets the truck's torque from the V8 each
	// time it refreshes the engine, and the Duramax multiplies it (DuramaxPower).
	internal static class Duramax
	{
		private static readonly Color Block = new Color(0.17f, 0.18f, 0.19f);
		private static readonly Color Covers = new Color(0.035f, 0.035f, 0.04f);

		// On the shop's template: every copy (bought, saved, bolted into a truck)
		// is made from it.
		public static void Style(GameObject block)
		{
			Renderer r = block.GetComponent<Renderer>();
			if (r != null)
			{
				Material[] mats = r.sharedMaterials;
				for (int i = 0; i < mats.Length; i++)
				{
					if (mats[i] != null)
					{
						mats[i] = new Material(mats[i]) { name = mats[i].name + " (Duramax)", color = Block };
					}
				}
				r.sharedMaterials = mats;
			}
			int painted = 0;
			foreach (durability d in block.GetComponentsInChildren<durability>(true))
			{
				string n = d.name;
				if (n == "valveCoverD_e" || n == "valveCoverP_e" || n == "intakeMan_e")
				{
					// The game's own paint (as from a paint can): it's applied when the
					// part starts, and goes with the part when it's taken off.
					EngineShop.SetField(d, "painted", true);
					EngineShop.SetField(d, "red", Covers.r);
					EngineShop.SetField(d, "green", Covers.g);
					EngineShop.SetField(d, "blue", Covers.b);
					EngineShop.SetField(d, "metallic", 0.3f);
					EngineShop.SetField(d, "smoothness", 0.75f);
					EngineShop.SetField(d, "paintSlot", 0);
					painted++;
				}
			}
			EngineClonerMod.Log("8.3L Duramax: charcoal block, " + painted + " part(s) in gloss black.");
		}

		public static bool Is(Component engine)
		{
			ShopEngine marker = engine != null ? engine.GetComponent<ShopEngine>() : null;
			return marker != null && marker.kind == "duramax";
		}
	}

	[HarmonyPatch(typeof(enginev8), "Refresh")]
	internal static class DuramaxPowerPatch
	{
		private static readonly FieldInfo WhichTruck = AccessTools.Field(typeof(enginev8), "whichTruck");
		private static readonly FieldInfo Truck = AccessTools.Field(typeof(enginev8), "truck");
		private static readonly FieldInfo Truck2 = AccessTools.Field(typeof(enginev8), "truck2");

		private static Component CarOf(enginev8 engine)
		{
			int which = WhichTruck != null ? (int)WhichTruck.GetValue(engine) : 1;
			FieldInfo field = which == 2 ? Truck2 : Truck;
			GameObject truck = field != null ? field.GetValue(engine) as GameObject : null;
			return truck != null ? (truck.GetComponent("car") ?? truck.GetComponent("car4")) : null;
		}

		private static float Get(Component car, string name)
		{
			FieldInfo f = AccessTools.Field(car.GetType(), name);
			return f != null && f.FieldType == typeof(float) ? (float)f.GetValue(car) : -1f;
		}

		// The torque before the refresh: only a value the V8 set during it is
		// multiplied (a refresh that leaves it alone mustn't multiply it again).
		private static void Prefix(enginev8 __instance, out float __state)
		{
			Component car = Duramax.Is(__instance) ? CarOf(__instance) : null;
			__state = car != null ? Get(car, "maxTorqueStatic") : -1f;
			if (car != null)
			{
				// A marker the V8 overwrites when it sets the torque.
				AccessTools.Field(car.GetType(), "maxTorqueStatic")?.SetValue(car, -12345f);
			}
		}

		private static void Postfix(enginev8 __instance, float __state)
		{
			Component car = Duramax.Is(__instance) ? CarOf(__instance) : null;
			if (car == null)
			{
				return;
			}
			float set = Get(car, "maxTorqueStatic");
			if (set == -12345f)
			{
				// Not set this time: put it back as it was.
				AccessTools.Field(car.GetType(), "maxTorqueStatic")?.SetValue(car, __state);
				return;
			}
			foreach (string name in new[] { "maxTorque", "maxTorqueStatic" })
			{
				FieldInfo f = AccessTools.Field(car.GetType(), name);
				float v = Get(car, name);
				if (f != null && v > 0f)
				{
					f.SetValue(car, v * EngineClonerMod.DuramaxPower);
				}
			}
		}

		// If Refresh throws (an engine missing parts), the marker mustn't stay (also
		// when another finalizer already swallowed the exception).
		private static System.Exception Finalizer(System.Exception __exception, enginev8 __instance, float __state)
		{
			Component car = Duramax.Is(__instance) ? CarOf(__instance) : null;
			if (car != null && Get(car, "maxTorqueStatic") == -12345f)
			{
				AccessTools.Field(car.GetType(), "maxTorqueStatic")?.SetValue(car, __state);
			}
			return __exception;
		}
	}
}
