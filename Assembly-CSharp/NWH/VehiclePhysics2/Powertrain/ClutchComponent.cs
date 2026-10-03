using System;
using NWH.VehiclePhysics2.Demo;
using NWH.VehiclePhysics2.Utility;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain
{
	// Token: 0x0200027B RID: 635
	[Serializable]
	public class ClutchComponent : PowertrainComponent
	{
		// Token: 0x060010BA RID: 4282 RVA: 0x000BE9B4 File Offset: 0x000BCBB4
		public override void OnPreSolve()
		{
			base.OnPreSolve();
			if ((this._smoothAcceleration < 0f && this.fwdAcceleration > 0f) || (this._smoothAcceleration > 0f && this.fwdAcceleration < 0f))
			{
				this._smoothAcceleration = 0f;
			}
			this._smoothAcceleration = Mathf.Lerp(this._smoothAcceleration, this.fwdAcceleration, 0.05f);
			int num = (this.gear < 0) ? (-this.gear) : this.gear;
			num = ((num == 0) ? 1 : num);
			float num2 = 1f / (float)(num * num);
			float num3 = Mathf.Clamp01(1f - this._smoothAcceleration * (float)this.gear * 0.2f) * num2;
			this.finalEngagementRPM = this.baseEngagementRPM + this.variableEngagementRPMRange * num3;
			this._cachedTargetAngVel = UnitConverter.RPMToAngularVelocity(this.finalEngagementRPM);
		}

		// Token: 0x060010BB RID: 4283 RVA: 0x000BEA98 File Offset: 0x000BCC98
		public override float QueryAngularVelocity(float inputAngularVelocity, float dt)
		{
			this.angularVelocity = inputAngularVelocity;
			if (this._outputAIsNull)
			{
				return inputAngularVelocity;
			}
			if (this.isAutomatic)
			{
				if (this.shiftSignal)
				{
					this.clutchEngagement = 0f;
				}
				else
				{
					this._ePrev = this._e;
					this._e = this._cachedTargetAngVel - this.angularVelocity;
					this._ei += this._e * dt;
					if (this._e <= 0f)
					{
						this._ei = 0f;
					}
					this._ed = (this._e - this._ePrev) / dt;
					float num = this.PID_Kp * this._e * dt + this.PID_Ki * this._ei * dt + this.PID_Kd * this._ed * dt;
					num *= this.PID_Coefficient;
					this.clutchEngagement -= num;
				}
				this.clutchEngagement = ((this.clutchEngagement < 0f) ? 0f : ((this.clutchEngagement > 1f) ? 1f : this.clutchEngagement));
			}
			float num2 = this.clutchEngagement * this.clutchEngagement;
			float num3 = this.outputA.QueryAngularVelocity(inputAngularVelocity, dt) * num2;
			float num4 = inputAngularVelocity * (1f - num2);
			return num3 + num4;
		}

		// Token: 0x060010BC RID: 4284 RVA: 0x000BEBDE File Offset: 0x000BCDDE
		public override float QueryInertia()
		{
			if (this._outputAIsNull)
			{
				return this.inertia;
			}
			return this.inertia + this.outputA.QueryInertia() * this.clutchEngagement;
		}

		// Token: 0x060010BD RID: 4285 RVA: 0x000BEC08 File Offset: 0x000BCE08
		public override float SendTorque(float torque, float inertiaSum, float dt)
		{
			if (this._outputAIsNull)
			{
				return torque;
			}
			torque = ((torque > this.slipTorque) ? this.slipTorque : ((torque < -this.slipTorque) ? (-this.slipTorque) : torque));
			float num = this.outputA.SendTorque(torque * this.clutchEngagement, (inertiaSum + this.inertia) * this.clutchEngagement, dt) * this.clutchEngagement * this.clutchEngagement;
			return (num > this.slipTorque) ? this.slipTorque : ((num < -this.slipTorque) ? (-this.slipTorque) : num);
		}

		// Token: 0x060010BE RID: 4286 RVA: 0x000BEC9F File Offset: 0x000BCE9F
		public override void Validate(VehicleController vc)
		{
			base.Validate(vc);
		}

		// Token: 0x060010BF RID: 4287 RVA: 0x000BECA8 File Offset: 0x000BCEA8
		public override void OnDisable()
		{
			base.OnDisable();
			this.clutchEngagement = 0f;
		}

		// Token: 0x04002115 RID: 8469
		[ShowInSettings("Base Engagement RPM", 1000f, 4000f, 200f)]
		[Tooltip("    RPM at which automatic clutch will try to engage.")]
		public float baseEngagementRPM = 1200f;

		// Token: 0x04002116 RID: 8470
		[Range(0f, 1f)]
		[ShowInTelemetry]
		[Tooltip("Clutch engagement in range [0,1] where 1 is fully engaged clutch.\r\nAffected by Slip Torque field as the clutch can transfer [clutchEngagement * slipTorque] Nm\r\nmeaning that higher value of slipTorque will result in more sensitive clutch.")]
		public float clutchEngagement;

		// Token: 0x04002117 RID: 8471
		[ShowInTelemetry]
		[Tooltip("    RPM at which the clutch will engage. Equals baseEngagementRPM plus variable engagement range.")]
		public float finalEngagementRPM = 2000f;

		// Token: 0x04002118 RID: 8472
		public float fwdAcceleration;

		// Token: 0x04002119 RID: 8473
		public int gear;

		// Token: 0x0400211A RID: 8474
		[ShowInSettings("Torque Converter")]
		[Tooltip("If true torqueConverterSlip torque will be used instead of slipTorque to calculate clutch/torque converter slip")]
		public bool hasTorqueConverter;

		// Token: 0x0400211B RID: 8475
		[ShowInSettings]
		[Tooltip("Is the clutch automatic? If true any input set manually will be overridden by the result given by PID controller\r\nbased on the difference between engine and clutch RPM.")]
		public bool isAutomatic = true;

		// Token: 0x0400211C RID: 8476
		[Tooltip("Final result of PID controller is multiplied by this value. Used to adjust how fast PID reacts without\r\nhaving to change individual coefficients.")]
		public float PID_Coefficient = 0.015f;

		// Token: 0x0400211D RID: 8477
		[SerializeField]
		[Range(0f, 2f)]
		[Tooltip("Derivative term of automatic clutch PID controller.")]
		public float PID_Kd = 0.5f;

		// Token: 0x0400211E RID: 8478
		[SerializeField]
		[Range(0f, 5f)]
		[Tooltip("Integral term of automatic clutch PID controller.")]
		public float PID_Ki = 2.7f;

		// Token: 0x0400211F RID: 8479
		[SerializeField]
		[Range(0f, 10f)]
		[Tooltip("Proportional term of automatic clutch PID controller.")]
		public float PID_Kp = 4f;

		// Token: 0x04002120 RID: 8480
		public bool shiftSignal;

		// Token: 0x04002121 RID: 8481
		[SerializeField]
		[ShowInSettings("Slip Torque", 100f, 5000f, 200f)]
		[Tooltip("Torque at which the clutch will slip / maximum torque that the clutch can transfer.\r\nThis value also affects clutch engagement as higher slip value will result in clutch\r\nthat grabs higher up / sooner. Too high slip torque value combined with low inertia of\r\npowertrain components might cause instability in powertrain solver.")]
		public float slipTorque = 3000f;

		// Token: 0x04002122 RID: 8482
		[Tooltip("Torque at which the torque converter will slip. Lower value will result in longer and smoother shifts.")]
		public float torqueConverterSlipTorque = 80f;

		// Token: 0x04002123 RID: 8483
		[SerializeField]
		[Tooltip("Maximum RPM value that variableEngagementIntensity field can add to engagement RPM.\r\nFinal clutch engagement RPM is calculated by multiplying this value by variableEngagementIntensity and adding the result\r\nengagementRPM. ")]
		public float variableEngagementRPMRange = 1400f;

		// Token: 0x04002124 RID: 8484
		private float _cachedTargetAngVel;

		// Token: 0x04002125 RID: 8485
		private float _e;

		// Token: 0x04002126 RID: 8486
		private float _ePrev;

		// Token: 0x04002127 RID: 8487
		private float _ed;

		// Token: 0x04002128 RID: 8488
		private float _ei;

		// Token: 0x04002129 RID: 8489
		private float _smoothAcceleration;
	}
}
