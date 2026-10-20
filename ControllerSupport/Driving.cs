using UnityEngine;

namespace ControllerSupport
{
	// Runs after the game's ControlRef.Update, which turns the keyboard into the
	// axes the game drives and walks from. The vehicles already take analog input
	// (the game supports Logitech wheels): GasInput (negative = reverse),
	// BrakeInput and xAxes (steering). So, like a driving game:
	//   R2  gas
	//   L2  brake while rolling forward, reverse once stopped
	//   left stick  steer
	// On foot the left stick walks (Horiz / Vert).
	internal static class Driving
	{
		// Rolling forward faster than this (m/s), L2 brakes; slower, it reverses.
		private const float BrakeAbove = 1f;

		// Whether we set the vehicle axes last frame (so they're cleared once after).
		private static bool drove;

		public static void Postfix(object __instance)
		{
			Game.Controls = __instance;
			Pad.Poll();
			if (!Pad.Connected || Game.GetBool(__instance, "usingWheel"))
			{
				Release(__instance);
				return;
			}
			if (Pad.Driving)
			{
				Drive(__instance);
			}
			else
			{
				Release(__instance);
				Walk(__instance);
			}
		}

		private static void Drive(object cr)
		{
			float threshold = ControllerSupportMod.TriggerThreshold;
			float gas = Pad.RightTrigger > threshold ? Pad.RightTrigger : 0f;
			float pedal = Pad.LeftTrigger > threshold ? Pad.LeftTrigger : 0f;
			float throttle = gas;
			float brake = 0f;
			if (pedal > 0f)
			{
				if (Game.ForwardSpeed() > BrakeAbove)
				{
					// Braking: the engine pulls back gently, the brakes bite when pressed harder.
					if (gas == 0f)
					{
						throttle = -pedal;
					}
					if (pedal > 0.5f)
					{
						brake = pedal;
					}
				}
				else if (gas == 0f)
				{
					throttle = -pedal;
				}
			}

			float steer = Pad.Move.x;
			float curve = Mathf.Max(1f, ControllerSupportMod.SteeringCurve);
			steer = Mathf.Sign(steer) * Mathf.Pow(Mathf.Abs(steer), curve);

			Game.SetFloat(cr, "GasInput", throttle);
			Game.SetFloat(cr, "BrakeInput", brake);
			Game.SetFloat(cr, "xAxes", steer);
			drove = true;
		}

		private static void Release(object cr)
		{
			if (!drove)
			{
				return;
			}
			drove = false;
			Game.SetFloat(cr, "GasInput", 0f);
			Game.SetFloat(cr, "BrakeInput", 0f);
			Game.SetFloat(cr, "xAxes", 0f);
		}

		private static void Walk(object cr)
		{
			// The keyboard wins while a walking key is down.
			if (Input.GetKey(Game.Key("Forward")) || Input.GetKey(Game.Key("Backward"))
				|| Input.GetKey(Game.Key("Left")) || Input.GetKey(Game.Key("Right")))
			{
				Game.SetFloat(cr, "Vert", Mathf.Clamp(Game.GetFloat(cr, "Vert"), -1f, 1f));
				Game.SetFloat(cr, "Horiz", Mathf.Clamp(Game.GetFloat(cr, "Horiz"), -1f, 1f));
				return;
			}
			Game.SetFloat(cr, "Vert", Pad.Move.y);
			Game.SetFloat(cr, "Horiz", Pad.Move.x);
		}
	}
}
