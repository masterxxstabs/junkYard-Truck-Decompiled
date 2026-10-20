using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace ControllerSupport
{
	// The game reads the keyboard and mouse with UnityEngine.Input everywhere
	// (bolts with the scroll wheel, tools with the number keys, looking with the
	// mouse...). Every method in the game (and in other mods) that calls Input is
	// rewritten to call the same-named method here instead, which returns what the
	// keyboard/mouse says plus what the controller says. Nothing else in those
	// methods changes.
	internal static class InputRedirect
	{
		private static readonly Dictionary<IntPtr, MethodInfo> map = new Dictionary<IntPtr, MethodInfo>();
		private static MethodInfo getAxis;
		private static MethodInfo getAxisPerSecond;

		public static void Install(HarmonyLib.Harmony harmony)
		{
			BuildMap();
			HarmonyMethod transpiler = new HarmonyMethod(typeof(InputRedirect).GetMethod("Transpiler", BindingFlags.Static | BindingFlags.NonPublic));
			Stopwatch watch = Stopwatch.StartNew();
			int patched = 0;
			int failed = 0;
			foreach (Assembly asm in Targets())
			{
				foreach (MethodBase m in Callers(asm))
				{
					try
					{
						harmony.Patch(m, transpiler: transpiler);
						patched++;
					}
					catch (Exception e)
					{
						failed++;
						if (failed <= 5)
						{
							ControllerSupportMod.Log("Couldn't hook " + m.DeclaringType + "." + m.Name + ": " + e.Message);
						}
					}
				}
			}
			ControllerSupportMod.Log("Hooked " + patched + " input readers in " + watch.ElapsedMilliseconds + " ms" + (failed > 0 ? " (" + failed + " skipped)." : "."));
		}

		// Input method -> the replacement in PadInput with the same name and parameters.
		private static void BuildMap()
		{
			foreach (MethodInfo m in typeof(Input).GetMethods(BindingFlags.Public | BindingFlags.Static))
			{
				Type[] args = Array.ConvertAll(m.GetParameters(), p => p.ParameterType);
				string name = m.Name == "get_anyKey" ? "AnyKey" : m.Name == "get_anyKeyDown" ? "AnyKeyDown" : m.Name;
				MethodInfo r = typeof(PadInput).GetMethod(name, BindingFlags.Public | BindingFlags.Static, null, args, null);
				if (r != null && r.ReturnType == m.ReturnType)
				{
					map[m.MethodHandle.Value] = r;
				}
			}
			getAxis = typeof(PadInput).GetMethod("GetAxis", new[] { typeof(string) });
			getAxisPerSecond = typeof(PadInput).GetMethod("GetAxisPerSecond", new[] { typeof(string) });
		}

		// The game's own code, and other mods (not this one).
		private static IEnumerable<Assembly> Targets()
		{
			try
			{
				Assembly.Load("Assembly-CSharp-firstpass");
			}
			catch
			{
			}
			Assembly self = typeof(InputRedirect).Assembly;
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (asm == self || asm is AssemblyBuilder || IsDynamic(asm))
				{
					continue;
				}
				string name = asm.GetName().Name;
				if (name.StartsWith("Unity", StringComparison.Ordinal) || name.StartsWith("System", StringComparison.Ordinal)
					|| name.StartsWith("Mono", StringComparison.Ordinal) || name.StartsWith("Microsoft", StringComparison.Ordinal)
					|| name == "mscorlib" || name == "MelonLoader" || name == "0Harmony" || name.StartsWith("Il2Cpp", StringComparison.Ordinal))
				{
					continue;
				}
				bool usesInput = false;
				foreach (AssemblyName r in asm.GetReferencedAssemblies())
				{
					if (r.Name == "UnityEngine.InputLegacyModule" || r.Name == "UnityEngine")
					{
						usesInput = true;
						break;
					}
				}
				if (usesInput)
				{
					yield return asm;
				}
			}
		}

		private static bool IsDynamic(Assembly asm)
		{
			try
			{
				return asm.ManifestModule is ModuleBuilder || string.IsNullOrEmpty(asm.Location);
			}
			catch
			{
				return true;
			}
		}

		// Methods whose IL calls one of the Input methods we replace.
		private static IEnumerable<MethodBase> Callers(Assembly asm)
		{
			Type[] types;
			try
			{
				types = asm.GetTypes();
			}
			catch (ReflectionTypeLoadException e)
			{
				types = e.Types;
			}
			Dictionary<int, bool> tokens = new Dictionary<int, bool>();
			const BindingFlags all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
			foreach (Type t in types)
			{
				if (t == null || t.ContainsGenericParameters)
				{
					continue;
				}
				MethodInfo[] methods;
				try
				{
					methods = t.GetMethods(all);
				}
				catch
				{
					continue;
				}
				foreach (MethodInfo m in methods)
				{
					if (m.IsAbstract || m.ContainsGenericParameters)
					{
						continue;
					}
					if (CallsInput(m, tokens))
					{
						yield return m;
					}
				}
			}
		}

		private static bool CallsInput(MethodInfo m, Dictionary<int, bool> tokens)
		{
			byte[] il;
			try
			{
				MethodBody body = m.GetMethodBody();
				il = body != null ? body.GetILAsByteArray() : null;
			}
			catch
			{
				return false;
			}
			if (il == null)
			{
				return false;
			}
			// Any call (0x28) to a member reference (token table 0x0A): Input lives in
			// another assembly, so it's always one. A stray byte match only costs a look.
			for (int i = 0; i + 4 < il.Length; i++)
			{
				if (il[i] != 0x28 || il[i + 4] != 0x0A)
				{
					continue;
				}
				int token = BitConverter.ToInt32(il, i + 1);
				bool hit;
				if (!tokens.TryGetValue(token, out hit))
				{
					hit = false;
					try
					{
						MethodBase target = m.Module.ResolveMethod(token);
						hit = target != null && target.DeclaringType == typeof(Input) && map.ContainsKey(target.MethodHandle.Value);
					}
					catch
					{
					}
					tokens[token] = hit;
				}
				if (hit)
				{
					return true;
				}
			}
			return false;
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			// The cab camera multiplies the mouse by deltaTime itself.
			bool perSecond = original != null && original.DeclaringType != null && original.DeclaringType.Name == "MouseLookCab";
			foreach (CodeInstruction ci in instructions)
			{
				MethodInfo target = ci.operand as MethodInfo;
				MethodInfo r;
				if ((ci.opcode == OpCodes.Call || ci.opcode == OpCodes.Callvirt) && target != null
					&& target.DeclaringType == typeof(Input) && map.TryGetValue(target.MethodHandle.Value, out r))
				{
					ci.opcode = OpCodes.Call;
					ci.operand = perSecond && r == getAxis ? getAxisPerSecond : r;
				}
				yield return ci;
			}
		}
	}

	// Drop-in replacements for UnityEngine.Input: the keyboard and mouse, plus the pad.
	public static class PadInput
	{
		public static float GetAxis(string axisName)
		{
			return Mathf.Clamp(Input.GetAxis(axisName) + Pad.Axis(axisName, true), -Limit(axisName), Limit(axisName));
		}

		public static float GetAxisPerSecond(string axisName)
		{
			return Mathf.Clamp(Input.GetAxis(axisName) + Pad.Axis(axisName, false), -Limit(axisName), Limit(axisName));
		}

		public static float GetAxisRaw(string axisName)
		{
			return Mathf.Clamp(Input.GetAxisRaw(axisName) + Pad.Axis(axisName, true), -Limit(axisName), Limit(axisName));
		}

		// Movement axes stay in -1..1; mouse movement can be anything.
		private static float Limit(string axisName)
		{
			return axisName == "Horizontal" || axisName == "Vertical" ? 1f : float.MaxValue;
		}

		public static bool GetKey(KeyCode key)
		{
			return Input.GetKey(key) || Pad.Held(key);
		}

		public static bool GetKeyDown(KeyCode key)
		{
			return Input.GetKeyDown(key) || Pad.Down(key);
		}

		public static bool GetKeyUp(KeyCode key)
		{
			return Input.GetKeyUp(key) || Pad.Up(key);
		}

		public static bool GetKey(string name)
		{
			KeyCode k = KeyByName(name);
			return Input.GetKey(name) || (k != KeyCode.None && Pad.Held(k));
		}

		public static bool GetKeyDown(string name)
		{
			KeyCode k = KeyByName(name);
			return Input.GetKeyDown(name) || (k != KeyCode.None && Pad.Down(k));
		}

		public static bool GetKeyUp(string name)
		{
			KeyCode k = KeyByName(name);
			return Input.GetKeyUp(name) || (k != KeyCode.None && Pad.Up(k));
		}

		public static bool GetMouseButton(int button)
		{
			return Input.GetMouseButton(button) || Pad.Mouse(button, 0);
		}

		public static bool GetMouseButtonDown(int button)
		{
			return Input.GetMouseButtonDown(button) || Pad.Mouse(button, 1);
		}

		public static bool GetMouseButtonUp(int button)
		{
			return Input.GetMouseButtonUp(button) || Pad.Mouse(button, -1);
		}

		public static bool GetButton(string buttonName)
		{
			return Input.GetButton(buttonName) || Pad.Button(buttonName, 0);
		}

		public static bool GetButtonDown(string buttonName)
		{
			return Input.GetButtonDown(buttonName) || Pad.Button(buttonName, 1);
		}

		public static bool GetButtonUp(string buttonName)
		{
			return Input.GetButtonUp(buttonName) || Pad.Button(buttonName, -1);
		}

		public static bool AnyKey()
		{
			return Input.anyKey || Pad.AnyHeld();
		}

		public static bool AnyKeyDown()
		{
			return Input.anyKeyDown || Pad.AnyDown();
		}

		private static readonly Dictionary<string, KeyCode> names = new Dictionary<string, KeyCode>();

		// Input.GetKey("space") style names.
		private static KeyCode KeyByName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return KeyCode.None;
			}
			KeyCode k;
			if (names.TryGetValue(name, out k))
			{
				return k;
			}
			string n = name.Trim().ToLowerInvariant();
			switch (n)
			{
			case "space": k = KeyCode.Space; break;
			case "escape": k = KeyCode.Escape; break;
			case "return":
			case "enter": k = KeyCode.Return; break;
			case "tab": k = KeyCode.Tab; break;
			case "left shift": k = KeyCode.LeftShift; break;
			case "right shift": k = KeyCode.RightShift; break;
			case "left ctrl": k = KeyCode.LeftControl; break;
			case "mouse 0": k = KeyCode.Mouse0; break;
			case "mouse 1": k = KeyCode.Mouse1; break;
			case "mouse 2": k = KeyCode.Mouse2; break;
			default:
				if (n.Length == 1 && n[0] >= 'a' && n[0] <= 'z')
				{
					k = KeyCode.A + (n[0] - 'a');
				}
				else if (n.Length == 1 && n[0] >= '0' && n[0] <= '9')
				{
					k = KeyCode.Alpha0 + (n[0] - '0');
				}
				else
				{
					k = KeyCode.None;
				}
				break;
			}
			names[name] = k;
			return k;
		}
	}
}
