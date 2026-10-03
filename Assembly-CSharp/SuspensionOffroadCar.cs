using System;
using UnityEngine;

// Token: 0x020000E5 RID: 229
public class SuspensionOffroadCar : MonoBehaviour
{
	// Token: 0x060005B2 RID: 1458 RVA: 0x000461C0 File Offset: 0x000443C0
	private void OnValidate()
	{
		if (this._suspensionWidth != this.SuspensionWidth || this._frontAxleOffset != this.FrontAxleOffset || this._rearAxlesOffset != this.RearAxlesOffset || this._absorbersOffset != this.AbsorbersOffset || this._rearRearAxlesOffset != this.RearRearAxlesOffset)
		{
			this.DoSuspensionWidth();
		}
		this._frontAxleOffset = this.FrontAxleOffset;
		this._suspensionWidth = this.SuspensionWidth;
		this._rearAxlesOffset = this.RearAxlesOffset;
		this._rearRearAxlesOffset = this.RearRearAxlesOffset;
		this._absorbersOffset = this.AbsorbersOffset;
		if (this._wheelsRadius != this.WheelsRadius || this._wheelsWidth != this.WheelsWidth)
		{
			this.DoWheelsRadiusWidth();
		}
		this._wheelsRadius = this.WheelsRadius;
		this._wheelsWidth = this.WheelsWidth;
		if (this._centerHeight != this.CenterHeight)
		{
			this.DoCenterHeight();
		}
		this._centerHeight = this.CenterHeight;
		this.DoCardans();
		this.DoTieRods();
		foreach (OffroadWheel wheel in this.Wheels)
		{
			this.DoAbsorbers(wheel);
			this.DoArms(wheel);
			this.DoSprings(wheel);
		}
	}

	// Token: 0x060005B3 RID: 1459 RVA: 0x00002188 File Offset: 0x00000388
	private void DoSteeringWheelOffset()
	{
	}

	// Token: 0x060005B4 RID: 1460 RVA: 0x000462EC File Offset: 0x000444EC
	private void DoSuspensionWidth()
	{
		this.otherElements.FrontLeftAxle.transform.localPosition = this.otherElements.FrontLeftAxleDefPos - new Vector3(this.SuspensionWidth, 0f, -this.FrontAxleOffset);
		this.otherElements.FrontMiddleAxle.transform.localPosition = this.otherElements.FrontMiddleAxleDefPos + new Vector3(0f, 0f, this.SuspensionWidth);
		this.otherElements.FrontRightAxle.transform.localPosition = new Vector3(this.SuspensionWidth, 0f, 0f);
		this.otherElements.RearLeftAxle.transform.localPosition = this.otherElements.RearLeftAxleDefPos - new Vector3(this.SuspensionWidth, 0f, this.RearAxlesOffset);
		this.otherElements.RearMiddleAxle.transform.localPosition = this.otherElements.RearMiddleAxleDefPos + new Vector3(0f, 0f, this.SuspensionWidth);
		this.otherElements.RearRightAxle.transform.localPosition = new Vector3(this.SuspensionWidth, 0f, 0f);
		if (this.Wheels.Length > 4)
		{
			this.otherElements.RearRearLeftAxle.transform.localPosition = this.otherElements.RearRearLeftAxleDefPos - new Vector3(this.SuspensionWidth, 0f, this.RearAxlesOffset + this.RearRearAxlesOffset);
			this.otherElements.RearRearMiddleAxle.transform.localPosition = this.otherElements.RearRearMiddleAxleDefPos + new Vector3(0f, 0f, this.SuspensionWidth);
			this.otherElements.RearRearRightAxle.transform.localPosition = new Vector3(this.SuspensionWidth, 0f, 0f);
		}
		this.otherElements.Steering.transform.localPosition = this.otherElements.SteeringDefPos + new Vector3(0f, 0f, this.FrontAxleOffset);
		this.Wheels[0].wheelCollider.transform.localPosition = this.Wheels[0].WheelColliderDefPos - new Vector3(this.SuspensionWidth, 0f, -this.FrontAxleOffset);
		this.Wheels[1].wheelCollider.transform.localPosition = this.Wheels[1].WheelColliderDefPos + new Vector3(this.SuspensionWidth, 0f, this.FrontAxleOffset);
		this.Wheels[2].wheelCollider.transform.localPosition = this.Wheels[2].WheelColliderDefPos - new Vector3(this.SuspensionWidth, 0f, this.RearAxlesOffset);
		this.Wheels[3].wheelCollider.transform.localPosition = this.Wheels[3].WheelColliderDefPos + new Vector3(this.SuspensionWidth, 0f, -this.RearAxlesOffset);
		if (this.Wheels.Length > 4)
		{
			this.Wheels[4].AbsorberUp.transform.localPosition = this.Wheels[4].AbsorberUpDefPos - new Vector3(this.SuspensionWidth - this.AbsorbersOffset, 0f, this.RearAxlesOffset + this.RearRearAxlesOffset);
			this.Wheels[4].wheelCollider.transform.localPosition = this.Wheels[4].WheelColliderDefPos - new Vector3(this.SuspensionWidth, 0f, this.RearAxlesOffset + this.RearRearAxlesOffset);
			this.Wheels[5].AbsorberUp.transform.localPosition = this.Wheels[5].AbsorberUpDefPos + new Vector3(this.SuspensionWidth - this.AbsorbersOffset, 0f, -this.RearAxlesOffset - this.RearRearAxlesOffset);
			this.Wheels[5].wheelCollider.transform.localPosition = this.Wheels[5].WheelColliderDefPos + new Vector3(this.SuspensionWidth, 0f, -this.RearAxlesOffset - this.RearRearAxlesOffset);
		}
		this.otherElements.CenterLeft.transform.localPosition = -new Vector3(this.SuspensionWidth, 0f, 0f);
		this.otherElements.CenterRight.transform.localPosition = new Vector3(this.SuspensionWidth, 0f, 0f);
	}

	// Token: 0x060005B5 RID: 1461 RVA: 0x000467B3 File Offset: 0x000449B3
	private void DoCenterHeight()
	{
		this.otherElements.CenterMiddle.transform.localPosition = this.otherElements.CenterDefPos + new Vector3(0f, this.CenterHeight, 0f);
	}

	// Token: 0x060005B6 RID: 1462 RVA: 0x000467F0 File Offset: 0x000449F0
	private void DoWheelsRadiusWidth()
	{
		foreach (OffroadWheel offroadWheel in this.Wheels)
		{
			offroadWheel.Wheel.transform.localScale = new Vector3(this.WheelsRadius, this.WheelsRadius, this.WheelsWidth);
			offroadWheel.wheelCollider.radius = 0.4f * this.WheelsRadius;
		}
	}

	// Token: 0x060005B7 RID: 1463 RVA: 0x00046854 File Offset: 0x00044A54
	private void DoCardans()
	{
		this.otherElements.FrontCardan2.transform.LookAt(this.otherElements.FrontCardan3.transform, this.otherElements.FrontCardan4.transform.up);
		this.otherElements.FrontCardan3.transform.LookAt(this.otherElements.FrontCardan2.transform, this.otherElements.FrontCardan4.transform.up);
		this.otherElements.RearCardan2.transform.LookAt(this.otherElements.RearCardan3.transform, this.otherElements.RearCardan4.transform.up);
		this.otherElements.RearCardan3.transform.LookAt(this.otherElements.RearCardan2.transform, this.otherElements.RearCardan4.transform.up);
		if (this.otherElements.RearRearCardan1 != null && this.otherElements.RearRearCardan2 != null && this.otherElements.RearRearCardan3 != null && this.otherElements.RearRearCardan4 != null)
		{
			this.otherElements.RearRearCardan2.transform.LookAt(this.otherElements.RearRearCardan3.transform, this.otherElements.RearRearCardan1.transform.up);
			this.otherElements.RearRearCardan3.transform.LookAt(this.otherElements.RearRearCardan2.transform, this.otherElements.RearRearCardan4.transform.up);
		}
		if (Application.isPlaying)
		{
			this.otherElements.FrontCardan4.transform.Rotate(new Vector3(0f, 0f, this.Wheels[0].AngularVelocity));
			this.otherElements.FrontCardan1.transform.Rotate(new Vector3(0f, 0f, -this.Wheels[0].AngularVelocity));
			this.otherElements.RearCardan4.transform.Rotate(new Vector3(0f, 0f, this.Wheels[0].AngularVelocity));
			this.otherElements.RearCardan1.transform.Rotate(new Vector3(0f, 0f, -this.Wheels[0].AngularVelocity));
			if (this.otherElements.RearRearCardan1 != null)
			{
				this.otherElements.RearRearCardan1.transform.Rotate(new Vector3(0f, 0f, -this.Wheels[0].AngularVelocity));
			}
			if (this.otherElements.RearRearCardan4 != null)
			{
				this.otherElements.RearRearCardan4.transform.Rotate(new Vector3(0f, 0f, -this.Wheels[0].AngularVelocity));
			}
		}
	}

	// Token: 0x060005B8 RID: 1464 RVA: 0x00046B6C File Offset: 0x00044D6C
	private void DoTieRods()
	{
		this.otherElements.TieRod1.transform.LookAt(this.otherElements.TieRod2.transform);
		this.otherElements.TieRod2.transform.LookAt(this.otherElements.TieRod1.transform);
		this.otherElements.TieRod3.transform.LookAt(this.otherElements.TieRod4.transform);
		this.otherElements.TieRod4.transform.LookAt(this.otherElements.TieRod3.transform);
	}

	// Token: 0x060005B9 RID: 1465 RVA: 0x00046C0D File Offset: 0x00044E0D
	private void DoSteering()
	{
		this.Wheels[0].Knuckle.transform.localEulerAngles = this.Wheels[1].Knuckle.transform.localEulerAngles;
	}

	// Token: 0x060005BA RID: 1466 RVA: 0x00002188 File Offset: 0x00000388
	private void DoSteeringWheel()
	{
	}

	// Token: 0x060005BB RID: 1467 RVA: 0x00046C40 File Offset: 0x00044E40
	private void DoAxles()
	{
		Vector3 localPosition = this.otherElements.FrontLeftAxle.transform.localPosition;
		this.tempPosY1 = localPosition.y;
		localPosition.y = this.otherElements.FrontLeftAxle.transform.parent.transform.InverseTransformPoint(this.Wheels[0].WCPosition).y;
		if (Mathf.Abs(this.tempPosY1 - localPosition.y) > 0.001f)
		{
			this.otherElements.FrontLeftAxle.transform.localPosition = localPosition;
			this.otherElements.FrontLeftAxle.transform.rotation = Quaternion.LookRotation(this.Wheels[1].WCPosition - this.otherElements.FrontLeftAxle.transform.position, -base.transform.forward);
		}
		localPosition = this.otherElements.RearLeftAxle.transform.localPosition;
		localPosition.y = this.otherElements.RearLeftAxle.transform.parent.transform.InverseTransformPoint(this.Wheels[2].WCPosition).y;
		if (Mathf.Abs(this.tempPosY1 - localPosition.y) > 0.001f)
		{
			this.otherElements.RearLeftAxle.transform.localPosition = localPosition;
			this.otherElements.RearLeftAxle.transform.rotation = Quaternion.LookRotation(this.Wheels[3].WCPosition - this.otherElements.RearLeftAxle.transform.position, base.transform.up);
		}
	}

	// Token: 0x060005BC RID: 1468 RVA: 0x00046DF0 File Offset: 0x00044FF0
	private void DoAbsorbers(OffroadWheel wheel)
	{
		wheel.AbsorberUp.transform.LookAt(wheel.AbsorberDown.transform, base.transform.forward);
		wheel.AbsorberDown.transform.LookAt(wheel.AbsorberUp.transform, base.transform.forward);
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x00046E4C File Offset: 0x0004504C
	private void DoArms(OffroadWheel wheel)
	{
		if (wheel.Arm1 != null)
		{
			wheel.Arm1.transform.LookAt(wheel.Arm2.transform, wheel.Arm1.transform.parent.transform.up);
		}
		if (wheel.Arm2 != null)
		{
			wheel.Arm2.transform.LookAt(wheel.Arm1.transform, wheel.Arm2.transform.parent.transform.up);
		}
	}

	// Token: 0x060005BE RID: 1470 RVA: 0x00046EE0 File Offset: 0x000450E0
	private void DoSprings(OffroadWheel wheel)
	{
		wheel.Spring.transform.localScale = new Vector3(1f, 1f, (Vector3.Distance(wheel.AbsorberUp.transform.position, wheel.AbsorberDown.transform.position) - 0.1f) / 0.4953054f);
	}

	// Token: 0x060005BF RID: 1471 RVA: 0x00046F40 File Offset: 0x00045140
	private void DoSteeringJoints(OffroadWheel wheel)
	{
		if (wheel.SteeringJoint2 != null)
		{
			wheel.SteeringJoint2.transform.Rotate(new Vector3(0f, 0f, wheel.AngularVelocity));
		}
		if (wheel.SteeringJoint1 != null)
		{
			wheel.SteeringJoint1.transform.Rotate(new Vector3(0f, 0f, wheel.AngularVelocity));
		}
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x00046FB3 File Offset: 0x000451B3
	private void CalculateRotationSpeed(OffroadWheel wheel)
	{
		wheel.AngularVelocity = wheel.wheelCollider.rpm / (0.02f / Time.fixedDeltaTime) / 9.3f;
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x00046FD8 File Offset: 0x000451D8
	private void OnGUI()
	{
		if (!this.ShowGUI)
		{
			return;
		}
		"label".normal.textColor = Color.white;
		GUI.Box(new Rect(0f, 0f, 250f, 270f), "");
		GUI.Label(new Rect(10f, 10f, 120f, 20f), "Suspension width");
		this.SuspensionWidth = GUI.HorizontalSlider(new Rect(130f, 15f, 100f, 20f), this.SuspensionWidth, 0.05f, 0.3f);
		GUI.Label(new Rect(10f, 30f, 100f, 20f), "Front axle offset");
		this.FrontAxleOffset = GUI.HorizontalSlider(new Rect(130f, 35f, 100f, 20f), this.FrontAxleOffset, -0.07f, 0.3f);
		GUI.Label(new Rect(10f, 50f, 100f, 20f), "Rear axles offset");
		this.RearAxlesOffset = GUI.HorizontalSlider(new Rect(130f, 55f, 100f, 20f), this.RearAxlesOffset, -0.14f, 0.3f);
		GUI.Label(new Rect(10f, 70f, 100f, 20f), "Absorbers offset");
		this.AbsorbersOffset = GUI.HorizontalSlider(new Rect(130f, 75f, 100f, 20f), this.AbsorbersOffset, 0f, 0.3f);
		if (GUI.changed)
		{
			this.DoSuspensionWidth();
		}
		GUI.Label(new Rect(10f, 90f, 100f, 20f), "Wheels radius");
		this.WheelsRadius = GUI.HorizontalSlider(new Rect(130f, 95f, 100f, 20f), this.WheelsRadius, 1f, 1.5f);
		GUI.Label(new Rect(10f, 110f, 100f, 20f), "Wheels width");
		this.WheelsWidth = GUI.HorizontalSlider(new Rect(130f, 115f, 100f, 20f), this.WheelsWidth, 1f, 1.5f);
		if (GUI.changed)
		{
			this.DoWheelsRadiusWidth();
		}
		GUI.Label(new Rect(10f, 130f, 100f, 20f), "Center height");
		this.CenterHeight = GUI.HorizontalSlider(new Rect(130f, 135f, 100f, 20f), this.CenterHeight, -0.1f, 0.1f);
		if (GUI.changed)
		{
			this.DoCenterHeight();
		}
		GUI.Label(new Rect(10f, 150f, 100f, 20f), "Steering wheel X");
		this.SteeringWheelOffset_X = GUI.HorizontalSlider(new Rect(130f, 155f, 100f, 20f), this.SteeringWheelOffset_X, -0.1f, 0.1f);
		GUI.Label(new Rect(10f, 170f, 100f, 20f), "Steering wheel Y");
		this.SteeringWheelOffset_Y = GUI.HorizontalSlider(new Rect(130f, 175f, 100f, 20f), this.SteeringWheelOffset_Y, -0.1f, 0.1f);
		GUI.Label(new Rect(10f, 190f, 130f, 20f), "Steering wheel Z");
		this.SteeringWheelOffset_Z = GUI.HorizontalSlider(new Rect(130f, 195f, 100f, 20f), this.SteeringWheelOffset_Z, -0.1f, 0.1f);
		GUI.Label(new Rect(10f, 210f, 130f, 20f), "Steering speed");
		this.SteeringSpeed = GUI.HorizontalSlider(new Rect(130f, 215f, 100f, 20f), this.SteeringSpeed, 70f, 200f);
		if (this.CarBody != null)
		{
			this.CarBody.SetActive(GUI.Toggle(new Rect(10f, 230f, 100f, 20f), this.CarBody.activeSelf, "Show car body"));
		}
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x00047458 File Offset: 0x00045658
	private void FixedUpdate()
	{
		this.SmoothedSteeringAngle = Mathf.MoveTowards(this.SmoothedSteeringAngle, this.Wheels[0].wheelCollider.steerAngle, Time.deltaTime * this.SteeringSpeed);
		foreach (OffroadWheel offroadWheel in this.Wheels)
		{
			offroadWheel.wheelCollider.GetWorldPose(out offroadWheel.WCPosition, out offroadWheel.WCRotation);
			if (!this.EVPConnected)
			{
				this.CalculateRotationSpeed(offroadWheel);
			}
			this.DoSteeringJoints(offroadWheel);
			this.DoAbsorbers(offroadWheel);
			this.DoArms(offroadWheel);
			this.DoSprings(offroadWheel);
			if (offroadWheel.Knuckle != null)
			{
				offroadWheel.Knuckle.transform.localEulerAngles = new Vector3(0f, this.SmoothedSteeringAngle, 0f);
			}
		}
		this.DoCardans();
		this.DoTieRods();
		this.DoSteering();
		this.DoAxles();
	}

	// Token: 0x04000C66 RID: 3174
	[Header("System")]
	public OffroadWheel[] Wheels;

	// Token: 0x04000C67 RID: 3175
	public OtherOffroadElements otherElements;

	// Token: 0x04000C68 RID: 3176
	[Header("Customization")]
	public GameObject CarBody;

	// Token: 0x04000C69 RID: 3177
	[Range(1f, 10f)]
	public float SteeringAngleMultiplier;

	// Token: 0x04000C6A RID: 3178
	[Range(0.05f, 0.3f)]
	public float SuspensionWidth;

	// Token: 0x04000C6B RID: 3179
	private float _suspensionWidth;

	// Token: 0x04000C6C RID: 3180
	[Range(1f, 1.5f)]
	public float WheelsRadius;

	// Token: 0x04000C6D RID: 3181
	private float _wheelsRadius;

	// Token: 0x04000C6E RID: 3182
	[Range(1f, 1.5f)]
	public float WheelsWidth;

	// Token: 0x04000C6F RID: 3183
	private float _wheelsWidth;

	// Token: 0x04000C70 RID: 3184
	[Range(-0.07f, 0.3f)]
	public float FrontAxleOffset;

	// Token: 0x04000C71 RID: 3185
	private float _frontAxleOffset;

	// Token: 0x04000C72 RID: 3186
	[Range(-0.14f, 0.3f)]
	public float RearAxlesOffset;

	// Token: 0x04000C73 RID: 3187
	private float _rearAxlesOffset;

	// Token: 0x04000C74 RID: 3188
	[Range(-0.13f, 0.03f)]
	public float RearRearAxlesOffset;

	// Token: 0x04000C75 RID: 3189
	private float _rearRearAxlesOffset;

	// Token: 0x04000C76 RID: 3190
	[Range(-0.1f, 0.1f)]
	public float CenterHeight;

	// Token: 0x04000C77 RID: 3191
	private float _centerHeight;

	// Token: 0x04000C78 RID: 3192
	[Range(-0.1f, 0.3f)]
	public float AbsorbersOffset;

	// Token: 0x04000C79 RID: 3193
	private float _absorbersOffset;

	// Token: 0x04000C7A RID: 3194
	[Range(-0.1f, 0.1f)]
	public float SteeringWheelOffset_X;

	// Token: 0x04000C7B RID: 3195
	private float _steeringWheelOffset_X;

	// Token: 0x04000C7C RID: 3196
	[Range(-0.1f, 0.1f)]
	public float SteeringWheelOffset_Y;

	// Token: 0x04000C7D RID: 3197
	private float _steeringWheelOffset_Y;

	// Token: 0x04000C7E RID: 3198
	[Range(-0.1f, 0.1f)]
	public float SteeringWheelOffset_Z;

	// Token: 0x04000C7F RID: 3199
	private float _steeringWheelOffset_Z;

	// Token: 0x04000C80 RID: 3200
	[Range(70f, 200f)]
	public float SteeringSpeed;

	// Token: 0x04000C81 RID: 3201
	public bool ShowGUI;

	// Token: 0x04000C82 RID: 3202
	private float SmoothedSteeringAngle;

	// Token: 0x04000C83 RID: 3203
	[HideInInspector]
	public bool EVPConnected;

	// Token: 0x04000C84 RID: 3204
	private float tempPosY1;
}
