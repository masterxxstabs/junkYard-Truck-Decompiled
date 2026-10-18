using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace EngineCloner
{
	// The game's speed limiters: every physics step, each vehicle's control()
	// sets maxSpeed to a fixed number (Diamondback pickup 85, or 100 with a 5th
	// gear; F100 95/100; golf cart 100) and cuts the engine (the cart also brakes
	// hard) once it's faster. With "Remove speed limits" on, those forward caps
	// become unreachable, so a vehicle goes as fast as its engine, gearing and
	// drag allow. maxSpeed = 0 (no gear selected) is left as it is, and so is the
	// AMC's reverse-only limit.
	//
	// A transpiler turns each `maxSpeed = <constant>` into
	// `maxSpeed = SpeedLimits.Cap(<constant>)`, so the setting applies live.
	[HarmonyPatch]
	internal static class SpeedLimits
	{
		public const float Unlimited = 100000f;

		private static IEnumerable<MethodBase> TargetMethods()
		{
			foreach (string type in new[] { "car", "car4", "golfcart" })
			{
				Type t = AccessTools.TypeByName(type);
				MethodInfo m = t != null ? AccessTools.Method(t, "control") : null;
				if (m != null)
				{
					yield return m;
				}
			}
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase __originalMethod)
		{
			List<CodeInstruction> code = new List<CodeInstruction>(instructions);
			MethodInfo cap = AccessTools.Method(typeof(SpeedLimits), "Cap");
			int patched = 0;
			for (int i = 0; i < code.Count - 1; i++)
			{
				FieldInfo field = code[i + 1].operand as FieldInfo;
				if (code[i].opcode == OpCodes.Ldc_R4 && (float)code[i].operand > 0f && code[i + 1].opcode == OpCodes.Stfld && field != null && field.Name == "maxSpeed")
				{
					code.Insert(i + 1, new CodeInstruction(OpCodes.Call, cap));
					patched++;
					i++;
				}
			}
			EngineClonerMod.Log("Speed limits: patched " + patched + " limit(s) in " + __originalMethod.DeclaringType.Name + ".control.");
			return code;
		}

		public static float Cap(float vanilla)
		{
			return EngineClonerMod.RemoveSpeedLimits ? Unlimited : vanilla;
		}
	}
}
