using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.CruiseControl
{
	// Token: 0x020002A0 RID: 672
	[Serializable]
	public class CruiseControlModule : VehicleModule
	{
		// Token: 0x06001206 RID: 4614 RVA: 0x000B61C6 File Offset: 0x000B43C6
		public override void Initialize()
		{
			this.initialized = true;
		}

		// Token: 0x06001207 RID: 4615 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x06001208 RID: 4616 RVA: 0x000C3BB8 File Offset: 0x000C1DB8
		public override void Update()
		{
			if (this.vc.input.states.cruiseControl)
			{
				base.ToggleState();
			}
			if (!base.Active)
			{
				return;
			}
			if (this.targetSpeed < 0.0001f)
			{
				return;
			}
			float speed = this.vc.Speed;
			float fixedDeltaTime = this.vc.fixedDeltaTime;
			this._eprev = this._e;
			this._e = this.targetSpeed - speed;
			if (this._e > -0.5f && this._e < 0.5f)
			{
				this._ei = 0f;
			}
			if (this.prevTargetSpeed != this.targetSpeed)
			{
				this._ei = 0f;
			}
			this._ei += this._e * fixedDeltaTime;
			this._ed = (this._e - this._eprev) / fixedDeltaTime;
			float num = this._e * this.Kp + this._ei * this.Ki + this._ed * this.Kd;
			num = ((num < -1f) ? -1f : ((num > 1f) ? 1f : num));
			this.output = Mathf.Lerp(this.output, num, this.vc.fixedDeltaTime * 5f);
			this.vc.input.Vertical = this.output;
			this.prevTargetSpeed = this.targetSpeed;
		}

		// Token: 0x06001209 RID: 4617 RVA: 0x000C3D22 File Offset: 0x000C1F22
		public override void Enable()
		{
			base.Enable();
			if (this.vc != null && this.setTargetSpeedOnEnable)
			{
				this.targetSpeed = this.vc.Speed;
			}
		}

		// Token: 0x0600120A RID: 4618 RVA: 0x000C2B9B File Offset: 0x000C0D9B
		public override VehicleModule.ModuleCategory GetModuleCategory()
		{
			return VehicleModule.ModuleCategory.DrivingAssists;
		}

		// Token: 0x0400222C RID: 8748
		[Tooltip("    Derivative gain of PID controller.")]
		public float Kd = 0.1f;

		// Token: 0x0400222D RID: 8749
		[Tooltip("    Integral gain of PID controller.")]
		public float Ki = 0.25f;

		// Token: 0x0400222E RID: 8750
		[Tooltip("    Proportional gain of PID controller.")]
		public float Kp = 0.5f;

		// Token: 0x0400222F RID: 8751
		[Tooltip("    Should the speed be set automatically when the module is enabled?")]
		public bool setTargetSpeedOnEnable;

		// Token: 0x04002230 RID: 8752
		[Tooltip("    The speed the vehicle will try to hold.")]
		public float targetSpeed;

		// Token: 0x04002231 RID: 8753
		private float _e;

		// Token: 0x04002232 RID: 8754
		private float _ed;

		// Token: 0x04002233 RID: 8755
		private float _ei;

		// Token: 0x04002234 RID: 8756
		private float _eprev;

		// Token: 0x04002235 RID: 8757
		private float output;

		// Token: 0x04002236 RID: 8758
		private float prevTargetSpeed;
	}
}
