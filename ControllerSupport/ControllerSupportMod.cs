using System;
using System.Reflection;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(ControllerSupport.ControllerSupportMod), "Controller Support", "1.0.0", "masterxxstabs")]
[assembly: MelonGame(null, null)]

namespace ControllerSupport
{
	// Play Junkyard Truck with an Xbox-style controller.
	//   On foot: hold L2 to tighten bolts, hold R2 to loosen them.
	//   Driving: R2 gas, L2 brake / reverse, left stick steers.
	// Everything else is set per button in the settings, separately for on foot
	// and driving.
	public class ControllerSupportMod : MelonMod
	{
		private static MelonLogger.Instance log;

		private static MelonPreferences_Entry<bool> enabled;
		private static MelonPreferences_Entry<float> lookSpeed;
		private static MelonPreferences_Entry<bool> invertLookY;
		private static MelonPreferences_Entry<float> boltTurns;
		private static MelonPreferences_Entry<float> deadzone;
		private static MelonPreferences_Entry<float> triggerThreshold;
		private static MelonPreferences_Entry<float> steeringCurve;
		private static readonly MelonPreferences_Entry<PadAction>[] onFoot = new MelonPreferences_Entry<PadAction>[14];
		private static readonly MelonPreferences_Entry<PadAction>[] driving = new MelonPreferences_Entry<PadAction>[14];

		public static bool Enabled
		{
			get { return enabled == null || enabled.Value; }
		}

		public static float LookSpeed
		{
			get { return lookSpeed != null ? lookSpeed.Value : 90f; }
		}

		public static bool InvertLookY
		{
			get { return invertLookY != null && invertLookY.Value; }
		}

		public static float BoltTurnsPerSecond
		{
			get { return boltTurns != null ? boltTurns.Value : 8f; }
		}

		public static float Deadzone
		{
			get { return deadzone != null ? deadzone.Value : 0.2f; }
		}

		public static float TriggerThreshold
		{
			get { return triggerThreshold != null ? triggerThreshold.Value : 0.15f; }
		}

		public static float SteeringCurve
		{
			get { return steeringCurve != null ? steeringCurve.Value : 1.4f; }
		}

		public static PadAction ActionFor(PadButton button, bool inVehicle)
		{
			MelonPreferences_Entry<PadAction> e = (inVehicle ? driving : onFoot)[(int)button];
			return e != null ? e.Value : PadAction.None;
		}

		public static void Log(string message)
		{
			if (log != null)
			{
				log.Msg(message);
			}
		}

		public override void OnInitializeMelon()
		{
			log = LoggerInstance;
			MelonPreferences_Category c = MelonPreferences.CreateCategory("ControllerSupport", "Controller Support");
			enabled = c.CreateEntry("Enabled", true, "Enabled", "Use an Xbox-style controller (PlayStation pads through Steam Input or DS4Windows).");
			lookSpeed = c.CreateEntry("LookSpeed", 90f, "Look speed", "How fast the right stick turns the camera.");
			invertLookY = c.CreateEntry("InvertLookY", false, "Invert look up/down", "On: push the right stick up to look down.");
			boltTurns = c.CreateEntry("BoltTurnsPerSecond", 8f, "Bolt turns per second", "How fast bolts turn while you hold L2 (tighten) or R2 (loosen).");
			deadzone = c.CreateEntry("Deadzone", 0.2f, "Stick deadzone", "How far a stick has to move before it counts (0 to 0.9).");
			triggerThreshold = c.CreateEntry("TriggerThreshold", 0.15f, "Trigger threshold", "How far L2 / R2 have to be pressed before they count (0 to 1).");
			steeringCurve = c.CreateEntry("SteeringCurve", 1.4f, "Steering curve", "1 = straight. Higher = gentler steering near the middle of the stick, full lock at the edge.");

			Bind(onFoot, c, "OnFoot", "On foot", new[]
			{
				PadAction.Jump,           // A
				PadAction.Throw,          // B
				PadAction.Interact,       // X
				PadAction.PutInInventory, // Y
				PadAction.RightClick,     // LB
				PadAction.LeftClick,      // RB
				PadAction.Sprint,         // L3
				PadAction.Crouch,         // R3
				PadAction.Ratchet,        // D-pad up
				PadAction.Flashlight,     // D-pad down
				PadAction.PreviousTool,   // D-pad left
				PadAction.NextTool,       // D-pad right
				PadAction.Map,            // Back
				PadAction.Pause           // Start
			});
			Bind(driving, c, "Driving", "Driving", new[]
			{
				PadAction.Handbrake,      // A
				PadAction.Interact,       // B
				PadAction.Clutch,         // X
				PadAction.ExitVehicle,    // Y
				PadAction.WinchRelease,   // LB
				PadAction.WinchPull,      // RB
				PadAction.Wheelie,        // L3
				PadAction.SwitchCamera,   // R3
				PadAction.Flashlight,     // D-pad up
				PadAction.None,           // D-pad down
				PadAction.LeanOut,        // D-pad left
				PadAction.LeanIn,         // D-pad right
				PadAction.Map,            // Back
				PadAction.Pause           // Start
			});
		}

		private static void Bind(MelonPreferences_Entry<PadAction>[] into, MelonPreferences_Category c, string id, string label, PadAction[] defaults)
		{
			for (int i = 0; i < into.Length; i++)
			{
				PadButton b = (PadButton)i;
				into[i] = c.CreateEntry(id + "_" + b, defaults[i], label + ": " + Pretty(b), "What " + Pretty(b) + " does " + (id == "Driving" ? "in a vehicle." : "on foot."));
			}
		}

		private static string Pretty(PadButton b)
		{
			switch (b)
			{
			case PadButton.DpadUp: return "D-pad up";
			case PadButton.DpadDown: return "D-pad down";
			case PadButton.DpadLeft: return "D-pad left";
			case PadButton.DpadRight: return "D-pad right";
			default: return b.ToString();
			}
		}

		public override void OnLateInitializeMelon()
		{
			try
			{
				InputRedirect.Install(HarmonyInstance);
			}
			catch (Exception e)
			{
				log.Error("Couldn't hook the game's input: " + e);
			}
			Type cr = Game.ControlRefType;
			MethodInfo update = cr != null ? AccessTools.Method(cr, "Update") : null;
			if (update == null)
			{
				log.Warning("The game's ControlRef wasn't found: driving with the controller won't work.");
				return;
			}
			HarmonyInstance.Patch(update, postfix: new HarmonyMethod(typeof(Driving).GetMethod("Postfix")));
			log.Msg("Ready. L2 tightens and R2 loosens bolts; in a vehicle R2 is gas and L2 brake / reverse.");
		}

		public override void OnUpdate()
		{
			// Keeps the controller read every frame, even when nothing asks.
			Pad.Poll();
		}
	}
}
