using System;
using NWH.VehiclePhysics2.Demo;
using NWH.VehiclePhysics2.Powertrain;
using NWH.VehiclePhysics2.Powertrain.Wheel;
using NWH.WheelController3D;
using UnityEngine;

namespace NWH.VehiclePhysics2
{
	// Token: 0x02000253 RID: 595
	[Serializable]
	public class Steering : VehicleComponent
	{
		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06000F73 RID: 3955 RVA: 0x000B67EC File Offset: 0x000B49EC
		// (set) Token: 0x06000F74 RID: 3956 RVA: 0x000B67F4 File Offset: 0x000B49F4
		public float Angle { get; set; }

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06000F75 RID: 3957 RVA: 0x000B67FD File Offset: 0x000B49FD
		// (set) Token: 0x06000F76 RID: 3958 RVA: 0x000B6805 File Offset: 0x000B4A05
		public float AdditionalAngle { get; set; }

		// Token: 0x06000F77 RID: 3959 RVA: 0x000B6810 File Offset: 0x000B4A10
		public override void Initialize()
		{
			if (this.steeringWheel != null)
			{
				this._initialSteeringWheelRotation = this.steeringWheel.transform.localRotation.eulerAngles;
			}
			this.AdjustGeometry();
			this.initialized = true;
		}

		// Token: 0x06000F78 RID: 3960 RVA: 0x000B6858 File Offset: 0x000B4A58
		public override void FixedUpdate()
		{
			if (!base.Active)
			{
				return;
			}
			float horizontal = this.vc.input.Horizontal;
			if (!this.returnToCenter && horizontal > -0.1f && horizontal < 0.1f)
			{
				return;
			}
			if (this.useDirectInput)
			{
				this.Angle = horizontal * this.maximumSteerAngle;
			}
			else
			{
				float time = (horizontal < 0f) ? (-horizontal) : horizontal;
				float num = (float)((horizontal < 0f) ? -1 : 1);
				float target = this.speedSensitiveSteeringCurve.Evaluate(this.vc.Speed / 50f) * this.maximumSteerAngle * this.linearity.Evaluate(time) * num;
				this._targetAngle = Mathf.SmoothDamp(this._targetAngle, target, ref this._steerVelocity, this.smoothing);
				this.Angle = Mathf.MoveTowards(this.Angle, this._targetAngle, this.degreesPerSecondLimit * this.vc.fixedDeltaTime);
			}
			foreach (WheelGroup wheelGroup in this.vc.WheelGroups)
			{
				float num2 = (this.Angle + this.AdditionalAngle) * wheelGroup.steerCoefficient;
				float num3 = num2 * wheelGroup.ackermanPercent;
				if (wheelGroup.Wheels.Count == 2)
				{
					float num4 = (wheelGroup.LeftWheel.SteerAngle < 0f) ? 1f : -1f;
					wheelGroup.LeftWheel.SteerAngle = num2 + num3 * num4;
					wheelGroup.RightWheel.SteerAngle = num2 - num3 * num4;
					if (this.vc.damageHandler.IsEnabled && !this.vc.damageHandler.visualOnly)
					{
						wheelGroup.LeftWheel.SteerAngle += wheelGroup.LeftWheel.Damage * this.maximumSteerAngle * wheelGroup.LeftWheel.DamageSteerDirection;
						wheelGroup.RightWheel.SteerAngle += wheelGroup.RightWheel.Damage * this.maximumSteerAngle * wheelGroup.RightWheel.DamageSteerDirection;
					}
				}
				else
				{
					foreach (WheelComponent wheelComponent in wheelGroup.Wheels)
					{
						wheelComponent.SteerAngle = num2;
					}
				}
			}
			if (this.steeringWheel != null)
			{
				float angle = this.Angle * this.steeringWheelTurnRatio;
				this.steeringWheel.transform.localRotation = Quaternion.Euler(this._initialSteeringWheelRotation);
				this.steeringWheel.transform.Rotate(Vector3.forward, angle);
			}
		}

		// Token: 0x06000F79 RID: 3961 RVA: 0x00002188 File Offset: 0x00000388
		public override void Update()
		{
		}

		// Token: 0x06000F7A RID: 3962 RVA: 0x000B6B50 File Offset: 0x000B4D50
		public void AdjustGeometry()
		{
			foreach (WheelGroup wheelGroup in this.vc.WheelGroups)
			{
				if (wheelGroup.casterAngle == 0f && wheelGroup.toeAngle == 0f)
				{
					break;
				}
				foreach (WheelComponent wheelComponent in wheelGroup.Wheels)
				{
					if (wheelComponent.wheelController.VehicleSide == WheelController.Side.Left)
					{
						wheelComponent.ControllerTransform.localEulerAngles = new Vector3(-wheelGroup.casterAngle, wheelGroup.toeAngle, wheelComponent.ControllerTransform.localEulerAngles.z);
					}
					else if (wheelComponent.wheelController.VehicleSide == WheelController.Side.Right)
					{
						wheelComponent.ControllerTransform.localEulerAngles = new Vector3(-wheelGroup.casterAngle, -wheelGroup.toeAngle, wheelComponent.ControllerTransform.localEulerAngles.z);
					}
				}
			}
		}

		// Token: 0x06000F7B RID: 3963 RVA: 0x000B6C80 File Offset: 0x000B4E80
		public override void SetDefaults(VehicleController vc)
		{
			base.SetDefaults(vc);
			this.speedSensitiveSteeringCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 1f, 0f, 0f),
				new Keyframe(0.3f, 0.4f, -0.6f, -0.6f),
				new Keyframe(1f, 0.2f, -0.1f, 0.1f)
			});
			this.linearity = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 1f, 1f),
				new Keyframe(1f, 1f, 1f, 1f)
			});
		}

		// Token: 0x04002001 RID: 8193
		[Tooltip("Only used if limitSteeringRate is true.Will limit wheels so that they can only steer up to the set degreelimit per second. E.g. 60 degrees per second will mean that the wheels that have 30 degree steer angle willtake 1 second to steer from full left to full right.")]
		[ShowInSettings("Deg/s Limit", 50f, 500f, 20f)]
		public float degreesPerSecondLimit = 180f;

		// Token: 0x04002002 RID: 8194
		[Tooltip("If true direct steering input will be used, without any modification.")]
		public bool useDirectInput;

		// Token: 0x04002003 RID: 8195
		public AnimationCurve linearity = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f, 1f, 1f),
			new Keyframe(1f, 1f, 1f, 1f)
		});

		// Token: 0x04002004 RID: 8196
		[Range(0f, 60f)]
		[ShowInSettings("Max. Steer Angle", 10f, 50f, 2f)]
		[Tooltip("    Maximum steering angle at the wheels.")]
		public float maximumSteerAngle = 25f;

		// Token: 0x04002005 RID: 8197
		[ShowInSettings("Return To Center")]
		[Tooltip("    Should wheels return to neutral position when there is no input?")]
		public bool returnToCenter = true;

		// Token: 0x04002006 RID: 8198
		[Range(0f, 1f)]
		[Tooltip("    Smoothing of the input.\r\n    Since raw data from Horizontal axis is used some smoothing is needed to make the vehicle easier to control with\r\n    binary inputs.")]
		public float smoothing = 0.1f;

		// Token: 0x04002007 RID: 8199
		[Tooltip("Curve that shows how the steering angle behaves at certain speed.\r\nX axis represents velocity in range 0 to 100m/s (normalized to 0,1).\r\nY axis represents 0 to maximumSteerAngle (normalized to 0,1).")]
		public AnimationCurve speedSensitiveSteeringCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 1f),
			new Keyframe(0.3f, 0.6f, 0f, -0.6f),
			new Keyframe(1f, 0.1f, 0.5f, 0f)
		});

		// Token: 0x04002008 RID: 8200
		[Tooltip("    Steering wheel transform that will be rotated when steering. Optional.")]
		public Transform steeringWheel;

		// Token: 0x04002009 RID: 8201
		[Tooltip("Steer angle will be multiplied by this value to get steering wheel angle. Ignored if steering wheel is null.\r\nIf you want the steering wheel to rotate in opposite direction use negative value.")]
		public float steeringWheelTurnRatio = 5f;

		// Token: 0x0400200A RID: 8202
		private Vector3 _initialSteeringWheelRotation;

		// Token: 0x0400200B RID: 8203
		private float _steerVelocity;

		// Token: 0x0400200C RID: 8204
		private float _targetAngle;
	}
}
