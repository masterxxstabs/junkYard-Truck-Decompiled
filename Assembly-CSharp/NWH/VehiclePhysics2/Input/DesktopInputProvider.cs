using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.VehiclePhysics2.Input
{
	// Token: 0x020002BF RID: 703
	[DisallowMultipleComponent]
	public class DesktopInputProvider : InputProvider
	{
		// Token: 0x060012F1 RID: 4849 RVA: 0x000C9834 File Offset: 0x000C7A34
		public override bool EngineStartStop()
		{
			return this.TryGetButtonDown("EngineStartStop", KeyCode.E, true);
		}

		// Token: 0x060012F2 RID: 4850 RVA: 0x000C9844 File Offset: 0x000C7A44
		public override float Clutch()
		{
			return this.TryGetAxis("Clutch", true);
		}

		// Token: 0x060012F3 RID: 4851 RVA: 0x000116EA File Offset: 0x0000F8EA
		public override bool ExtraLights()
		{
			return false;
		}

		// Token: 0x060012F4 RID: 4852 RVA: 0x000C9852 File Offset: 0x000C7A52
		public override bool ChangeCamera()
		{
			return this.TryGetButtonDown("ChangeCamera", KeyCode.C, true);
		}

		// Token: 0x060012F5 RID: 4853 RVA: 0x000C9862 File Offset: 0x000C7A62
		public override bool ChangeVehicle()
		{
			return this.TryGetButtonDown("ChangeVehicle", KeyCode.V, true);
		}

		// Token: 0x060012F6 RID: 4854 RVA: 0x000C9872 File Offset: 0x000C7A72
		public override bool HighBeamLights()
		{
			return this.TryGetButtonDown("HighBeamLights", KeyCode.K, true);
		}

		// Token: 0x060012F7 RID: 4855 RVA: 0x000C9884 File Offset: 0x000C7A84
		public override float Handbrake()
		{
			float result = 0f;
			try
			{
				result = this.TryGetAxis("Handbrake", true);
			}
			catch
			{
				result = (Input.GetKey(KeyCode.Space) ? 1f : 0f);
			}
			return result;
		}

		// Token: 0x060012F8 RID: 4856 RVA: 0x000C98D0 File Offset: 0x000C7AD0
		public override bool HazardLights()
		{
			return this.TryGetButtonDown("HazardLights", KeyCode.J, true);
		}

		// Token: 0x060012F9 RID: 4857 RVA: 0x000C98E0 File Offset: 0x000C7AE0
		public override float Horizontal()
		{
			float result;
			if (this.inputType == DesktopInputProvider.InputType.Standard)
			{
				result = this.TryGetAxisRaw("Horizontal", true);
			}
			else
			{
				result = Mathf.Clamp(InputUtility.GetMouseHorizontal(), -1f, 1f);
			}
			return result;
		}

		// Token: 0x060012FA RID: 4858 RVA: 0x000C9920 File Offset: 0x000C7B20
		public override bool Horn()
		{
			return this.TryGetButton("Horn", KeyCode.H, true);
		}

		// Token: 0x060012FB RID: 4859 RVA: 0x000C9930 File Offset: 0x000C7B30
		public override bool LeftBlinker()
		{
			return this.TryGetButtonDown("LeftBlinker", KeyCode.Z, true);
		}

		// Token: 0x060012FC RID: 4860 RVA: 0x000C9940 File Offset: 0x000C7B40
		public override bool LowBeamLights()
		{
			return this.TryGetButtonDown("LowBeamLights", KeyCode.L, true);
		}

		// Token: 0x060012FD RID: 4861 RVA: 0x000C9950 File Offset: 0x000C7B50
		public override bool RightBlinker()
		{
			return this.TryGetButtonDown("RightBlinker", KeyCode.X, true);
		}

		// Token: 0x060012FE RID: 4862 RVA: 0x000C9960 File Offset: 0x000C7B60
		public override bool ShiftDown()
		{
			return this.TryGetButtonDown("ShiftDown", KeyCode.F, true);
		}

		// Token: 0x060012FF RID: 4863 RVA: 0x000C9970 File Offset: 0x000C7B70
		public override int ShiftInto()
		{
			for (int i = -1; i < 7; i++)
			{
				if (this.TryGetButtonDown(this.shiftInputNames[i + 1], KeyCode.Alpha0, false))
				{
					return i;
				}
			}
			return -999;
		}

		// Token: 0x06001300 RID: 4864 RVA: 0x000C99A5 File Offset: 0x000C7BA5
		public override bool ShiftUp()
		{
			return this.TryGetButtonDown("ShiftUp", KeyCode.R, true);
		}

		// Token: 0x06001301 RID: 4865 RVA: 0x000C99B5 File Offset: 0x000C7BB5
		public override bool TrailerAttachDetach()
		{
			return this.TryGetButtonDown("TrailerAttachDetach", KeyCode.T, true);
		}

		// Token: 0x06001302 RID: 4866 RVA: 0x000C99C8 File Offset: 0x000C7BC8
		public override float Vertical()
		{
			float result = 0f;
			if (this.inputType == DesktopInputProvider.InputType.Standard)
			{
				if (this.verticalInputMapping == DesktopInputProvider.VerticalInputMapping.Standard)
				{
					result = this.TryGetAxisRaw("Vertical", true);
				}
				else if (this.verticalInputMapping == DesktopInputProvider.VerticalInputMapping.ZeroToOne)
				{
					result = (Mathf.Clamp01(this.TryGetAxisRaw("Vertical", true)) - 0.5f) * 2f;
				}
				else if (this.verticalInputMapping == DesktopInputProvider.VerticalInputMapping.Composite)
				{
					float num = Mathf.Clamp01(this.TryGetAxisRaw("Accelerator", true));
					float num2 = Mathf.Clamp01(this.TryGetAxisRaw("Brake", true));
					result = num - num2;
				}
			}
			else if (this.inputType == DesktopInputProvider.InputType.Mouse)
			{
				result = Mathf.Clamp(InputUtility.GetMouseVertical(), -1f, 1f);
			}
			else if (Input.GetMouseButton(0))
			{
				result = 1f;
			}
			else if (Input.GetMouseButton(1))
			{
				result = -1f;
			}
			return result;
		}

		// Token: 0x06001303 RID: 4867 RVA: 0x000C9A96 File Offset: 0x000C7C96
		public override bool FlipOver()
		{
			return this.TryGetButtonDown("FlipOver", KeyCode.M, true);
		}

		// Token: 0x06001304 RID: 4868 RVA: 0x000C9AA6 File Offset: 0x000C7CA6
		public override bool Boost()
		{
			return this.TryGetButton("Boost", KeyCode.LeftShift, true);
		}

		// Token: 0x06001305 RID: 4869 RVA: 0x000C9AB9 File Offset: 0x000C7CB9
		public override bool CruiseControl()
		{
			return this.TryGetButtonDown("CruiseControl", KeyCode.N, true);
		}

		// Token: 0x06001306 RID: 4870 RVA: 0x000C9ACC File Offset: 0x000C7CCC
		private bool TryGetButton(string buttonName, KeyCode altKey, bool showWarning = true)
		{
			bool result;
			try
			{
				result = Input.GetButton(buttonName);
			}
			catch
			{
				if (this._warningCount < 100 && showWarning)
				{
					Debug.LogWarning(buttonName + " input binding missing, falling back to default. Check Input section in manual for more info.");
					this._warningCount++;
				}
				result = Input.GetKey(altKey);
			}
			return result;
		}

		// Token: 0x06001307 RID: 4871 RVA: 0x000C9B2C File Offset: 0x000C7D2C
		private bool TryGetButtonDown(string buttonName, KeyCode altKey, bool showWarning = true)
		{
			bool result;
			try
			{
				result = Input.GetButtonDown(buttonName);
			}
			catch
			{
				if (this._warningCount < 100 && showWarning)
				{
					Debug.LogWarning(buttonName + " input binding missing, falling back to default. Check Input section in manual for more info.");
					this._warningCount++;
				}
				result = Input.GetKeyDown(altKey);
			}
			return result;
		}

		// Token: 0x06001308 RID: 4872 RVA: 0x000C9B8C File Offset: 0x000C7D8C
		private float TryGetAxis(string axisName, bool showWarning = true)
		{
			try
			{
				return Input.GetAxis(axisName);
			}
			catch
			{
				if (this._warningCount < 100 && showWarning)
				{
					Debug.LogWarning(axisName + " input binding missing. Check Input section in manual for more info.");
					this._warningCount++;
				}
			}
			return 0f;
		}

		// Token: 0x06001309 RID: 4873 RVA: 0x000C9BE8 File Offset: 0x000C7DE8
		private float TryGetAxisRaw(string axisName, bool showWarning = true)
		{
			try
			{
				return Input.GetAxisRaw(axisName);
			}
			catch
			{
				if (this._warningCount < 100 && showWarning)
				{
					Debug.LogWarning(axisName + " input binding missing. Check Input section in manual for more info.");
					this._warningCount++;
				}
			}
			return 0f;
		}

		// Token: 0x04002366 RID: 9062
		[Tooltip("Input type. Standard - uses standard input manager for all the inputs. Mouse - uses mouse position for steering and throttle. MouseSteer - uses mouse position for steering, LMB and RMB for braking / throttle.")]
		public DesktopInputProvider.InputType inputType;

		// Token: 0x04002367 RID: 9063
		[Tooltip("Names of input bindings for each individual gears. If you need to add more gears modify this and the corresponding\r\niterator in the\r\nShiftInto() function.")]
		[NonSerialized]
		public string[] shiftInputNames = new string[]
		{
			"ShiftIntoR1",
			"ShiftInto0",
			"ShiftInto1",
			"ShiftInto2",
			"ShiftInto3",
			"ShiftInto4",
			"ShiftInto5",
			"ShiftInto6",
			"ShiftInto7"
		};

		// Token: 0x04002368 RID: 9064
		[FormerlySerializedAs("verticalInputType")]
		[Tooltip("Vertical input type.Standard - uses vertical axis in range of [-1, 1] where -1 is maximum braking and 1 maximum accleration.ZeroToOne - uses vertical axis in range of [0, 1], 0 being maximum braking and 1 maximum accelerationMag.Composite - uses separate axes, 'Accelerator' and 'Brake' to set the vertical axis value. Still uses a single vartical axis value [-1, 1] throughout the system so applying full brakes and gas simultaneously is not possible.")]
		public DesktopInputProvider.VerticalInputMapping verticalInputMapping;

		// Token: 0x04002369 RID: 9065
		private string _tmpStr;

		// Token: 0x0400236A RID: 9066
		private int _warningCount;

		// Token: 0x020004CC RID: 1228
		public enum InputType
		{
			// Token: 0x04002C31 RID: 11313
			Standard,
			// Token: 0x04002C32 RID: 11314
			Mouse
		}

		// Token: 0x020004CD RID: 1229
		public enum VerticalInputMapping
		{
			// Token: 0x04002C34 RID: 11316
			Standard,
			// Token: 0x04002C35 RID: 11317
			ZeroToOne,
			// Token: 0x04002C36 RID: 11318
			Composite
		}
	}
}
