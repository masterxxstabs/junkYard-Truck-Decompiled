using System;
using System.Runtime.InteropServices;

namespace ControllerSupport
{
	// Reads Xbox-style controllers straight from Windows (XInput), so no Unity
	// input setup is needed. PlayStation and other pads work through Steam Input
	// or DS4Windows, which make them look like an Xbox controller.
	internal static class XInput
	{
		public const ushort DpadUp = 0x0001;
		public const ushort DpadDown = 0x0002;
		public const ushort DpadLeft = 0x0004;
		public const ushort DpadRight = 0x0008;
		public const ushort Start = 0x0010;
		public const ushort Back = 0x0020;
		public const ushort LeftThumb = 0x0040;
		public const ushort RightThumb = 0x0080;
		public const ushort LeftShoulder = 0x0100;
		public const ushort RightShoulder = 0x0200;
		public const ushort A = 0x1000;
		public const ushort B = 0x2000;
		public const ushort X = 0x4000;
		public const ushort Y = 0x8000;

		[StructLayout(LayoutKind.Sequential)]
		public struct Gamepad
		{
			public ushort buttons;
			public byte leftTrigger;
			public byte rightTrigger;
			public short leftX;
			public short leftY;
			public short rightX;
			public short rightY;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct State
		{
			public uint packet;
			public Gamepad pad;
		}

		[DllImport("xinput1_4.dll", EntryPoint = "XInputGetState")]
		private static extern uint GetState14(uint user, out State state);

		[DllImport("xinput1_3.dll", EntryPoint = "XInputGetState")]
		private static extern uint GetState13(uint user, out State state);

		[DllImport("xinput9_1_0.dll", EntryPoint = "XInputGetState")]
		private static extern uint GetState910(uint user, out State state);

		// Which XInput DLL answered: 0 = not tried yet, 1..3 = that one, -1 = none.
		private static int library;

		public static bool Available
		{
			get { return library >= 0; }
		}

		public static bool TryGet(uint user, out State state)
		{
			state = default(State);
			if (library < 0)
			{
				return false;
			}
			if (library == 0)
			{
				for (int i = 1; i <= 3; i++)
				{
					try
					{
						Call(i, user, out state);
						library = i;
						break;
					}
					catch (DllNotFoundException)
					{
					}
					catch (EntryPointNotFoundException)
					{
					}
				}
				if (library == 0)
				{
					library = -1;
					return false;
				}
			}
			return Call(library, user, out state) == 0;
		}

		private static uint Call(int which, uint user, out State state)
		{
			switch (which)
			{
			case 1:
				return GetState14(user, out state);
			case 2:
				return GetState13(user, out state);
			default:
				return GetState910(user, out state);
			}
		}
	}
}
