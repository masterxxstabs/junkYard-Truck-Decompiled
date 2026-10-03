using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Demo;
using NWH.VehiclePhysics2.Input;
using NWH.VehiclePhysics2.Powertrain;
using NWH.VehiclePhysics2.Powertrain.Wheel;
using UnityEngine;

namespace NWH.VehiclePhysics2
{
	// Token: 0x02000252 RID: 594
	[Serializable]
	public class Brakes : VehicleComponent
	{
		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06000F6B RID: 3947 RVA: 0x000B61B5 File Offset: 0x000B43B5
		// (set) Token: 0x06000F6C RID: 3948 RVA: 0x000B61BD File Offset: 0x000B43BD
		public bool IsBraking
		{
			get
			{
				return this._isBraking;
			}
			set
			{
				this._isBraking = value;
			}
		}

		// Token: 0x06000F6D RID: 3949 RVA: 0x000B61C6 File Offset: 0x000B43C6
		public override void Initialize()
		{
			this.initialized = true;
		}

		// Token: 0x06000F6E RID: 3950 RVA: 0x000B61D0 File Offset: 0x000B43D0
		public override void FixedUpdate()
		{
			if (!base.Active)
			{
				return;
			}
			foreach (WheelComponent wheelComponent in this.vc.Wheels)
			{
				wheelComponent.ResetBrakes(0f);
			}
			float num = this.SumBrakeTorqueModifiers();
			float vertical = this.vc.input.Vertical;
			float num2 = (vertical < 0f) ? (-vertical) : vertical;
			if (num < 0.01f)
			{
				return;
			}
			this._isBraking = false;
			int gear = this.vc.powertrain.transmission.Gear;
			if (this.brakeOffThrottle && vertical < 0.05f)
			{
				foreach (WheelGroup wheelGroup in this.vc.WheelGroups)
				{
					if (wheelGroup.handbrakeCoefficient > 0f)
					{
						foreach (WheelComponent wheelComponent2 in wheelGroup.Wheels)
						{
							wheelComponent2.SetBrakeIntensity(this.brakeOffThrottleStrength * wheelGroup.brakeCoefficient);
						}
					}
				}
			}
			if (this.vc.input.Handbrake > 0.02f && this.vc.IsAwake)
			{
				foreach (WheelGroup wheelGroup2 in this.vc.WheelGroups)
				{
					if (wheelGroup2.handbrakeCoefficient > 0f)
					{
						foreach (WheelComponent wheelComponent3 in wheelGroup2.Wheels)
						{
							wheelComponent3.AddBrakeTorque(this.maxTorque * wheelGroup2.handbrakeCoefficient * this.vc.input.Handbrake * num, true);
						}
					}
				}
			}
			bool flag = this.brakeWhileIdle && num2 < 0.01f && gear == 0 && this.vc.Speed < 0.3f;
			bool flag2 = this.brakeWhileAsleep && !this.vc.IsAwake;
			if (flag || flag2)
			{
				foreach (WheelGroup wheelGroup3 in this.vc.WheelGroups)
				{
					foreach (WheelComponent wheelComponent4 in wheelGroup3.Wheels)
					{
						wheelComponent4.SetBrakeIntensity(num);
					}
				}
			}
			if (this.brakeOnReverseDirection)
			{
				this._intensity = Mathf.SmoothDamp(this._intensity, num2, ref this._intensityVelocity, this.smoothing);
				if (!this.vc.powertrain.HasWheelSpin || this.vc.powertrain.engine.throttlePosition <= 0.5f)
				{
					if (this.vc.input.throttleType == NWH.VehiclePhysics2.Input.Input.ThrottleType.WForwardSReverse)
					{
						float forwardVelocity = this.vc.ForwardVelocity;
						bool flag3 = (forwardVelocity < -this.reverseDirectionVelocityThreshold && vertical > 0f) || (forwardVelocity > this.reverseDirectionVelocityThreshold && vertical < 0f);
						bool flag4 = (vertical > 0f && gear < 0) || (vertical < 0f && gear > 0);
						if ((vertical > 0.05f || vertical < -0.05f) && (flag3 || flag4))
						{
							using (List<WheelGroup>.Enumerator enumerator2 = this.vc.WheelGroups.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									WheelGroup wheelGroup4 = enumerator2.Current;
									foreach (WheelComponent wheelComponent5 in wheelGroup4.Wheels)
									{
										wheelComponent5.SetBrakeIntensity(this._intensity * num * wheelGroup4.brakeCoefficient);
									}
								}
								goto IL_4B8;
							}
						}
						this._intensity = 0f;
					}
					else
					{
						if (this.vc.input.Vertical < 0f)
						{
							using (List<WheelGroup>.Enumerator enumerator2 = this.vc.WheelGroups.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									WheelGroup wheelGroup5 = enumerator2.Current;
									foreach (WheelComponent wheelComponent6 in wheelGroup5.Wheels)
									{
										wheelComponent6.SetBrakeIntensity(this._intensity * num * wheelGroup5.brakeCoefficient);
									}
								}
								goto IL_4B8;
							}
						}
						this._intensity = 0f;
					}
				}
			}
			IL_4B8:
			bool flag5 = this.airBrakes;
		}

		// Token: 0x06000F6F RID: 3951 RVA: 0x00002188 File Offset: 0x00000388
		public override void Update()
		{
		}

		// Token: 0x06000F70 RID: 3952 RVA: 0x000B6724 File Offset: 0x000B4924
		public override void Disable()
		{
			base.Disable();
			this._isBraking = false;
		}

		// Token: 0x06000F71 RID: 3953 RVA: 0x000B6734 File Offset: 0x000B4934
		private float SumBrakeTorqueModifiers()
		{
			if (this.brakeTorqueModifiers.Count == 0)
			{
				return 1f;
			}
			float num = 1f;
			int count = this.brakeTorqueModifiers.Count;
			for (int i = 0; i < count; i++)
			{
				num *= this.brakeTorqueModifiers[i]();
			}
			return Mathf.Clamp(num, 0f, float.PositiveInfinity);
		}

		// Token: 0x04001FF3 RID: 8179
		[Tooltip("    Set to true to use the air brake sound effect.")]
		public bool airBrakes;

		// Token: 0x04001FF4 RID: 8180
		public bool airBrakeSoundFlag;

		// Token: 0x04001FF5 RID: 8181
		[Tooltip("Should brakes be applied automatically when throttle is released?")]
		public bool brakeOffThrottle;

		// Token: 0x04001FF6 RID: 8182
		[Range(0f, 1f)]
		public float brakeOffThrottleStrength = 0.2f;

		// Token: 0x04001FF7 RID: 8183
		[Tooltip("Should vehicle automatically brake when input direction is different from vehicle movement direction?")]
		public bool brakeOnReverseDirection = true;

		// Token: 0x04001FF8 RID: 8184
		[Tooltip("Collection of functions that modify the braking performance of the vehicle. Used for modules such as ABS where brakes need to be overriden or their effect reduced/increase. Return 1 for neutral modifier while returning 0 will disable the brakes completely. All brake torque modifiers will be multiplied in order to get the final brake torque coefficient.")]
		public List<Brakes.BrakeTorqueModifier> brakeTorqueModifiers = new List<Brakes.BrakeTorqueModifier>();

		// Token: 0x04001FF9 RID: 8185
		[Tooltip("    Should brakes be applied when vehicle is asleep (IsAwake == false)?")]
		public bool brakeWhileAsleep = true;

		// Token: 0x04001FFA RID: 8186
		[Tooltip("    If true vehicle will break when in neutral and no throttle is applied.")]
		public bool brakeWhileIdle = true;

		// Token: 0x04001FFB RID: 8187
		[Tooltip("Max brake torque that can be applied to each wheel. To adjust braking on per-axle basis change brake coefficients under Axle settings")]
		[ShowInSettings("Brake Torque", 1000f, 10000f, 500f)]
		public float maxTorque = 5000f;

		// Token: 0x04001FFC RID: 8188
		[Tooltip("If the vehicle is traveling in the opposite direction from user input, auto braking will happen above/under this velocity [m/s]. E.g.user is pressing W and the vehicle is going 2 m/s backwards. Vehicle will break until it slows down to under threshold velocity, shift into first and start to accelerate.")]
		[Range(0.05f, 2f)]
		public float reverseDirectionVelocityThreshold = 0.34f;

		// Token: 0x04001FFD RID: 8189
		[Range(0f, 5f)]
		[ShowInSettings("Brake Smoothing")]
		[Tooltip("    Time in seconds needed to reach full braking torque.")]
		public float smoothing;

		// Token: 0x04001FFE RID: 8190
		private float _intensity;

		// Token: 0x04001FFF RID: 8191
		private float _intensityVelocity;

		// Token: 0x04002000 RID: 8192
		[SerializeField]
		private bool _isBraking;

		// Token: 0x020004A5 RID: 1189
		// (Invoke) Token: 0x06001AE8 RID: 6888
		public delegate float BrakeTorqueModifier();
	}
}
