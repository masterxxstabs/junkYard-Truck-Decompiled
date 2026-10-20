using System;
using System.Collections.Generic;
using UnityEngine;

namespace ControllerSupport
{
	public enum PadButton
	{
		A,
		B,
		X,
		Y,
		LB,
		RB,
		L3,
		R3,
		DpadUp,
		DpadDown,
		DpadLeft,
		DpadRight,
		Back,
		Start
	}

	// What a button does. Most are the game's own controls (whatever key you set
	// for them in the game's options), so the pad follows your key bindings.
	public enum PadAction
	{
		None,
		Jump,
		Interact,
		Throw,
		PutInInventory,
		Inventory,
		Map,
		Flashlight,
		Sprint,
		Crouch,
		LeftClick,
		RightClick,
		MiddleClick,
		Pause,
		Ratchet,
		Hands,
		NextTool,
		PreviousTool,
		ExitVehicle,
		Handbrake,
		Clutch,
		WinchPull,
		WinchRelease,
		Wheelie,
		LeanIn,
		LeanOut,
		SwitchCamera,
		Help,
		TrackMission,
		Space,
		Enter,
		Tab
	}

	// The controller's state for this frame, and what the game should see from it.
	// Read once per frame, the first time anything asks (so key-down/up edges are
	// right whichever script runs first).
	internal static class Pad
	{
		private static readonly ushort[] Masks =
		{
			XInput.A, XInput.B, XInput.X, XInput.Y,
			XInput.LeftShoulder, XInput.RightShoulder, XInput.LeftThumb, XInput.RightThumb,
			XInput.DpadUp, XInput.DpadDown, XInput.DpadLeft, XInput.DpadRight,
			XInput.Back, XInput.Start
		};

		// The game's tool keys in order: hands, ratchet, multimeter, depth gauge,
		// phone, tire gauge, crowbar.
		private static readonly string[] ToolKeys = { "Item1", "Item2", "Item3", "Item4", "Item5", "Item6", "Item7" };

		private static int frame = -1;
		private static int user = -1;
		private static float nextScan;

		public static bool Connected;
		public static bool Driving;

		// Sticks after the deadzone (-1..1) and triggers (0..1).
		public static Vector2 Move;
		public static Vector2 Look;
		public static float LeftTrigger;
		public static float RightTrigger;

		private static ushort buttons;
		private static ushort lastButtons;

		private static HashSet<KeyCode> keys = new HashSet<KeyCode>();
		private static HashSet<KeyCode> lastKeys = new HashSet<KeyCode>();
		private static readonly bool[] mouse = new bool[3];
		private static readonly bool[] lastMouse = new bool[3];
		private static bool jump;
		private static bool lastJump;

		// Bolt turning: one scroll-wheel notch at a time while a trigger is held.
		public static float Scroll;
		private static float boltClock;
		private static int boltDir;

		private static int tool;

		public static void Poll()
		{
			if (frame == Time.frameCount)
			{
				return;
			}
			frame = Time.frameCount;

			HashSet<KeyCode> swap = lastKeys;
			lastKeys = keys;
			keys = swap;
			keys.Clear();
			for (int i = 0; i < 3; i++)
			{
				lastMouse[i] = mouse[i];
				mouse[i] = false;
			}
			lastJump = jump;
			jump = false;
			lastButtons = buttons;
			buttons = 0;
			Scroll = 0f;
			Move = Vector2.zero;
			Look = Vector2.zero;
			LeftTrigger = 0f;
			RightTrigger = 0f;

			XInput.State state;
			if (!ControllerSupportMod.Enabled || !Read(out state))
			{
				Connected = false;
				boltDir = 0;
				return;
			}
			Connected = true;
			Driving = Game.Driving();

			// The game window must have focus, like the keyboard and mouse.
			if (!Application.isFocused)
			{
				boltDir = 0;
				return;
			}

			float dead = Mathf.Clamp(ControllerSupportMod.Deadzone, 0f, 0.9f);
			Move = Stick(state.pad.leftX, state.pad.leftY, dead);
			Look = Stick(state.pad.rightX, state.pad.rightY, dead);
			LeftTrigger = Trigger(state.pad.leftTrigger);
			RightTrigger = Trigger(state.pad.rightTrigger);
			buttons = state.pad.buttons;

			for (int i = 0; i < Masks.Length; i++)
			{
				if ((buttons & Masks[i]) != 0)
				{
					Apply(ControllerSupportMod.ActionFor((PadButton)i, Driving), (lastButtons & Masks[i]) == 0);
				}
			}

			if (!Driving)
			{
				// Walking keys too, for the scripts that read them directly (ladders).
				float half = 0.5f;
				if (Move.y > half) Add(Game.Key("Forward"));
				if (Move.y < -half) Add(Game.Key("Backward"));
				if (Move.x < -half) Add(Game.Key("Left"));
				if (Move.x > half) Add(Game.Key("Right"));
			}

			BoltTurns();
		}

		// L2 held tightens, R2 held loosens: the scroll wheel, one notch at a time,
		// the first one straight away and then BoltTurnsPerSecond.
		private static void BoltTurns()
		{
			float threshold = ControllerSupportMod.TriggerThreshold;
			int dir = 0;
			if (!Driving)
			{
				bool tighten = LeftTrigger > threshold;
				bool loosen = RightTrigger > threshold;
				if (tighten != loosen)
				{
					dir = tighten ? 1 : -1;
				}
			}
			if (dir == 0)
			{
				boltDir = 0;
				return;
			}
			if (dir != boltDir)
			{
				boltDir = dir;
				boltClock = 1f;
			}
			else
			{
				boltClock += Time.unscaledDeltaTime * Mathf.Max(0.5f, ControllerSupportMod.BoltTurnsPerSecond);
			}
			if (boltClock >= 1f)
			{
				boltClock = Mathf.Min(boltClock - 1f, 1f);
				Scroll = 0.1f * dir;
			}
		}

		private static bool Read(out XInput.State state)
		{
			state = default(XInput.State);
			if (user >= 0 && XInput.TryGet((uint)user, out state))
			{
				return true;
			}
			user = -1;
			// Looking for a controller in an empty slot is slow, so only now and then.
			if (Time.unscaledTime < nextScan || !XInput.Available)
			{
				return false;
			}
			nextScan = Time.unscaledTime + 2f;
			for (uint i = 0; i < 4; i++)
			{
				if (XInput.TryGet(i, out state))
				{
					user = (int)i;
					ControllerSupportMod.Log("Controller " + (i + 1) + " connected.");
					return true;
				}
			}
			return false;
		}

		private static Vector2 Stick(short x, short y, float dead)
		{
			Vector2 v = new Vector2(Mathf.Clamp(x / 32767f, -1f, 1f), Mathf.Clamp(y / 32767f, -1f, 1f));
			float m = v.magnitude;
			if (m <= dead)
			{
				return Vector2.zero;
			}
			float scaled = Mathf.Min(1f, (m - dead) / (1f - dead));
			return v / m * scaled;
		}

		private static float Trigger(byte value)
		{
			float t = value / 255f;
			return t < 0.02f ? 0f : t;
		}

		private static void Add(KeyCode key)
		{
			if (key != KeyCode.None)
			{
				keys.Add(key);
			}
		}

		private static void Apply(PadAction action, bool pressed)
		{
			switch (action)
			{
			case PadAction.None:
				return;
			case PadAction.Jump:
				Add(Game.Key("Jump"));
				jump = true;
				return;
			case PadAction.LeftClick:
				mouse[0] = true;
				Add(KeyCode.Mouse0);
				return;
			case PadAction.RightClick:
				mouse[1] = true;
				Add(KeyCode.Mouse1);
				return;
			case PadAction.MiddleClick:
				mouse[2] = true;
				Add(KeyCode.Mouse2);
				return;
			case PadAction.Pause:
				Add(KeyCode.Escape);
				return;
			case PadAction.SwitchCamera:
				Add(KeyCode.C);
				return;
			case PadAction.Help:
				Add(KeyCode.H);
				return;
			case PadAction.TrackMission:
				Add(KeyCode.T);
				return;
			case PadAction.Space:
				Add(KeyCode.Space);
				return;
			case PadAction.Enter:
				Add(KeyCode.Return);
				return;
			case PadAction.Tab:
				Add(KeyCode.Tab);
				return;
			case PadAction.Hands:
				tool = 0;
				Add(Game.Key(ToolKeys[0]));
				return;
			case PadAction.Ratchet:
				tool = 1;
				Add(Game.Key(ToolKeys[1]));
				return;
			case PadAction.NextTool:
			case PadAction.PreviousTool:
				// A single tap of the next tool's key, once per press.
				if (pressed)
				{
					int step = action == PadAction.NextTool ? 1 : ToolKeys.Length - 1;
					tool = (tool + step) % ToolKeys.Length;
					Add(Game.Key(ToolKeys[tool]));
				}
				return;
			default:
				Add(Game.Key(GameKeyName(action)));
				return;
			}
		}

		private static string GameKeyName(PadAction action)
		{
			switch (action)
			{
			case PadAction.PutInInventory:
				return "PlaceInv";
			case PadAction.ExitVehicle:
				return "ExitVeh";
			case PadAction.Handbrake:
				return "Brake";
			default:
				return action.ToString();
			}
		}

		// What the redirected UnityEngine.Input calls see (see InputRedirect).

		public static bool Held(KeyCode key)
		{
			Poll();
			return keys.Contains(key);
		}

		public static bool Down(KeyCode key)
		{
			Poll();
			return keys.Contains(key) && !lastKeys.Contains(key);
		}

		public static bool Up(KeyCode key)
		{
			Poll();
			return !keys.Contains(key) && lastKeys.Contains(key);
		}

		public static bool Mouse(int button, int edge)
		{
			Poll();
			if (button < 0 || button > 2)
			{
				return false;
			}
			if (edge > 0)
			{
				return mouse[button] && !lastMouse[button];
			}
			if (edge < 0)
			{
				return !mouse[button] && lastMouse[button];
			}
			return mouse[button];
		}

		public static bool Button(string name, int edge)
		{
			Poll();
			switch (name)
			{
			case "Jump":
				return edge > 0 ? jump && !lastJump : edge < 0 ? !jump && lastJump : jump;
			case "Fire1":
				return Mouse(0, edge);
			case "Fire2":
				return Mouse(1, edge);
			case "Fire3":
				return Mouse(2, edge);
			case "Cancel":
				return edge > 0 ? Down(KeyCode.Escape) : edge < 0 ? Up(KeyCode.Escape) : Held(KeyCode.Escape);
			default:
				return false;
			}
		}

		public static bool AnyHeld()
		{
			Poll();
			return keys.Count > 0 || mouse[0] || mouse[1] || mouse[2];
		}

		public static bool AnyDown()
		{
			Poll();
			return Connected && (buttons & ~lastButtons) != 0;
		}

		// Extra for an axis, added to the mouse/keyboard's own value.
		// perFrame: the value is a per-frame mouse movement (most look scripts);
		// otherwise the caller multiplies by Time.deltaTime itself.
		public static float Axis(string name, bool perFrame)
		{
			Poll();
			if (!Connected)
			{
				return 0f;
			}
			switch (name)
			{
			case "Mouse X":
				return LookAxis(Look.x, perFrame);
			case "Mouse Y":
				return LookAxis(ControllerSupportMod.InvertLookY ? -Look.y : Look.y, perFrame);
			case "Mouse ScrollWheel":
				return Scroll;
			case "Horizontal":
				return Move.x;
			case "Vertical":
				if (Driving)
				{
					return RightTrigger - LeftTrigger;
				}
				return Move.y;
			default:
				return 0f;
			}
		}

		private static float LookAxis(float v, bool perFrame)
		{
			// Squared for fine aiming near the middle, full speed at the edge.
			float curved = v * Mathf.Abs(v);
			float speed = ControllerSupportMod.LookSpeed;
			if (perFrame)
			{
				return curved * speed * Time.unscaledDeltaTime;
			}
			// Scripts like the cab camera multiply by deltaTime and a sensitivity of
			// about 100; this turns at about the same rate as the walking camera.
			return curved * speed * 0.02f;
		}
	}
}
