using System;
using NWH.VehiclePhysics2.Cameras;
using NWH.VehiclePhysics2.SceneManagement;
using UnityEngine;

namespace NWH.VehiclePhysics2.Input
{
	// Token: 0x020002C1 RID: 705
	public class MobileInputProvider : InputProvider
	{
		// Token: 0x0600130F RID: 4879 RVA: 0x000C9CE4 File Offset: 0x000C7EE4
		public override bool EngineStartStop()
		{
			return this.engineStartStopButton != null && this.engineStartStopButton.hasBeenClicked;
		}

		// Token: 0x06001310 RID: 4880 RVA: 0x000C9D04 File Offset: 0x000C7F04
		private void Update()
		{
			if (this.changeCameraButton != null && this.changeCameraButton.hasBeenClicked)
			{
				CameraChanger componentInChildren = VehicleChanger.ActiveVehicleController.gameObject.GetComponentInChildren<CameraChanger>();
				if (componentInChildren != null)
				{
					componentInChildren.NextCamera();
				}
				else
				{
					Debug.LogError("Change camera button was pressed but the vehicle does not have CameraChanger in any of its children.");
				}
			}
			if (this.changeVehicleButton != null && this.changeVehicleButton.hasBeenClicked)
			{
				VehicleChanger.Instance.NextVehicle();
			}
		}

		// Token: 0x06001311 RID: 4881 RVA: 0x000C9D7C File Offset: 0x000C7F7C
		public override bool ChangeCamera()
		{
			return this.changeCameraButton != null && this.changeCameraButton.hasBeenClicked;
		}

		// Token: 0x06001312 RID: 4882 RVA: 0x000C9D99 File Offset: 0x000C7F99
		public override bool ChangeVehicle()
		{
			return this.changeVehicleButton != null && this.changeVehicleButton.hasBeenClicked;
		}

		// Token: 0x06001313 RID: 4883 RVA: 0x000C9DB6 File Offset: 0x000C7FB6
		public override float Clutch()
		{
			return 0f;
		}

		// Token: 0x06001314 RID: 4884 RVA: 0x000C9DBD File Offset: 0x000C7FBD
		public override bool ExtraLights()
		{
			return this.extraLightsButton != null && this.extraLightsButton.hasBeenClicked;
		}

		// Token: 0x06001315 RID: 4885 RVA: 0x000C9DDA File Offset: 0x000C7FDA
		public override bool HighBeamLights()
		{
			return this.highBeamLightsButton != null && this.highBeamLightsButton.hasBeenClicked;
		}

		// Token: 0x06001316 RID: 4886 RVA: 0x000C9DF7 File Offset: 0x000C7FF7
		public override float Handbrake()
		{
			return (float)((this.handbrakeButton == null) ? 0 : (this.handbrakeButton.isPressed ? 1 : 0));
		}

		// Token: 0x06001317 RID: 4887 RVA: 0x000C9E1C File Offset: 0x000C801C
		public override bool HazardLights()
		{
			return this.hazardLightsButton != null && this.hazardLightsButton.hasBeenClicked;
		}

		// Token: 0x06001318 RID: 4888 RVA: 0x000C9E3C File Offset: 0x000C803C
		public override float Horizontal()
		{
			if (this.horizontalInputType == MobileInputProvider.HorizontalAxisType.SteeringWheel)
			{
				if (this.steeringWheel != null)
				{
					return this.steeringWheel.GetClampedValue();
				}
				Debug.LogWarning("HorizontalAxisType is set to SteeringWheel but no Steering Wheel has been assigned.");
			}
			else
			{
				if (this.horizontalInputType == MobileInputProvider.HorizontalAxisType.Accelerometer)
				{
					return Input.acceleration.x * this.tiltSensitivity;
				}
				if (this.horizontalInputType == MobileInputProvider.HorizontalAxisType.Button)
				{
					if (!(this.steerLeftButton != null) || !(this.steerRightButton != null))
					{
						Debug.LogWarning("HorizontalAxisType is set to button but buttons have not been assigned.");
						return 0f;
					}
					if (this.steerLeftButton.isPressed)
					{
						return -1f;
					}
					if (!this.steerRightButton.isPressed)
					{
						return 0f;
					}
					return 1f;
				}
			}
			return 0f;
		}

		// Token: 0x06001319 RID: 4889 RVA: 0x000C9EF8 File Offset: 0x000C80F8
		public override bool Horn()
		{
			return this.hornButton != null && this.hornButton.hasBeenClicked;
		}

		// Token: 0x0600131A RID: 4890 RVA: 0x000C9F15 File Offset: 0x000C8115
		public override bool LeftBlinker()
		{
			return this.leftBlinkerButton != null && this.leftBlinkerButton.hasBeenClicked;
		}

		// Token: 0x0600131B RID: 4891 RVA: 0x000C9F32 File Offset: 0x000C8132
		public override bool LowBeamLights()
		{
			return this.lowBeamLightsButton != null && this.lowBeamLightsButton.hasBeenClicked;
		}

		// Token: 0x0600131C RID: 4892 RVA: 0x000C9F4F File Offset: 0x000C814F
		public override bool RightBlinker()
		{
			return this.rightBlinkerButton != null && this.rightBlinkerButton.hasBeenClicked;
		}

		// Token: 0x0600131D RID: 4893 RVA: 0x000C9F6C File Offset: 0x000C816C
		public override bool ShiftDown()
		{
			return this.shiftDownButton != null && this.shiftDownButton.hasBeenClicked;
		}

		// Token: 0x0600131E RID: 4894 RVA: 0x000C9F89 File Offset: 0x000C8189
		public override int ShiftInto()
		{
			return -999;
		}

		// Token: 0x0600131F RID: 4895 RVA: 0x000C9F90 File Offset: 0x000C8190
		public override bool ShiftUp()
		{
			return this.shiftUpButton != null && this.shiftUpButton.hasBeenClicked;
		}

		// Token: 0x06001320 RID: 4896 RVA: 0x000C9FAD File Offset: 0x000C81AD
		public override bool TrailerAttachDetach()
		{
			return this.trailerAttachDetachButton != null && this.trailerAttachDetachButton.hasBeenClicked;
		}

		// Token: 0x06001321 RID: 4897 RVA: 0x000C9FCC File Offset: 0x000C81CC
		public override float Vertical()
		{
			if (this.verticalInputType == MobileInputProvider.VerticalAxisType.Accelerometer)
			{
				return Input.acceleration.y * this.tiltSensitivity;
			}
			if (this.verticalInputType != MobileInputProvider.VerticalAxisType.Button)
			{
				return 0f;
			}
			if (!(this.brakeButton != null) || !(this.throttleButton != null))
			{
				Debug.LogWarning("VerticalAxisType is set to button but buttons have not been assigned.");
				return 0f;
			}
			if (this.brakeButton.isPressed)
			{
				return -1f;
			}
			if (!this.throttleButton.isPressed)
			{
				return 0f;
			}
			return 1f;
		}

		// Token: 0x06001322 RID: 4898 RVA: 0x000CA059 File Offset: 0x000C8259
		public override bool FlipOver()
		{
			return this.flipOverButton != null && this.flipOverButton.hasBeenClicked;
		}

		// Token: 0x06001323 RID: 4899 RVA: 0x000CA076 File Offset: 0x000C8276
		public override bool Boost()
		{
			return this.boostButton != null && this.boostButton.hasBeenClicked;
		}

		// Token: 0x06001324 RID: 4900 RVA: 0x000CA093 File Offset: 0x000C8293
		public override bool CruiseControl()
		{
			return this.cruiseControlButton != null && this.cruiseControlButton.hasBeenClicked;
		}

		// Token: 0x0400236D RID: 9069
		public MobileInputButton boostButton;

		// Token: 0x0400236E RID: 9070
		public MobileInputButton brakeButton;

		// Token: 0x0400236F RID: 9071
		public MobileInputButton changeCameraButton;

		// Token: 0x04002370 RID: 9072
		public MobileInputButton changeVehicleButton;

		// Token: 0x04002371 RID: 9073
		public MobileInputButton cruiseControlButton;

		// Token: 0x04002372 RID: 9074
		public MobileInputButton engineStartStopButton;

		// Token: 0x04002373 RID: 9075
		public MobileInputButton extraLightsButton;

		// Token: 0x04002374 RID: 9076
		public MobileInputButton flipOverButton;

		// Token: 0x04002375 RID: 9077
		public MobileInputButton handbrakeButton;

		// Token: 0x04002376 RID: 9078
		public MobileInputButton hazardLightsButton;

		// Token: 0x04002377 RID: 9079
		public MobileInputButton highBeamLightsButton;

		// Token: 0x04002378 RID: 9080
		[Tooltip("    Active steer devices.")]
		public MobileInputProvider.HorizontalAxisType horizontalInputType = MobileInputProvider.HorizontalAxisType.SteeringWheel;

		// Token: 0x04002379 RID: 9081
		public MobileInputButton hornButton;

		// Token: 0x0400237A RID: 9082
		public MobileInputButton leftBlinkerButton;

		// Token: 0x0400237B RID: 9083
		public MobileInputButton lowBeamLightsButton;

		// Token: 0x0400237C RID: 9084
		public MobileInputButton rightBlinkerButton;

		// Token: 0x0400237D RID: 9085
		public MobileInputButton shiftDownButton;

		// Token: 0x0400237E RID: 9086
		public MobileInputButton shiftUpButton;

		// Token: 0x0400237F RID: 9087
		[Tooltip("    Steering wheel script. Optional and not needed if SteeringWheel option is not used.")]
		public SteeringWheel steeringWheel;

		// Token: 0x04002380 RID: 9088
		public MobileInputButton steerLeftButton;

		// Token: 0x04002381 RID: 9089
		public MobileInputButton steerRightButton;

		// Token: 0x04002382 RID: 9090
		public MobileInputButton throttleButton;

		// Token: 0x04002383 RID: 9091
		[Tooltip("    Higher value will result in higher steer angle for same tilt.")]
		public float tiltSensitivity = 1.5f;

		// Token: 0x04002384 RID: 9092
		public MobileInputButton trailerAttachDetachButton;

		// Token: 0x04002385 RID: 9093
		[Tooltip("    Active steer devices.")]
		public MobileInputProvider.VerticalAxisType verticalInputType = MobileInputProvider.VerticalAxisType.Button;

		// Token: 0x020004CE RID: 1230
		public enum HorizontalAxisType
		{
			// Token: 0x04002C38 RID: 11320
			Accelerometer,
			// Token: 0x04002C39 RID: 11321
			SteeringWheel,
			// Token: 0x04002C3A RID: 11322
			Button,
			// Token: 0x04002C3B RID: 11323
			Screen
		}

		// Token: 0x020004CF RID: 1231
		public enum VerticalAxisType
		{
			// Token: 0x04002C3D RID: 11325
			Accelerometer,
			// Token: 0x04002C3E RID: 11326
			Button,
			// Token: 0x04002C3F RID: 11327
			Screen
		}
	}
}
