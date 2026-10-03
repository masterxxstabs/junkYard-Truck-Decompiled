using System;
using NWH.VehiclePhysics2.Demo;
using NWH.VehiclePhysics2.Powertrain;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.ESC
{
	// Token: 0x0200029E RID: 670
	[Serializable]
	public class ESCModule : VehicleModule
	{
		// Token: 0x060011FE RID: 4606 RVA: 0x000B61C6 File Offset: 0x000B43C6
		public override void Initialize()
		{
			this.initialized = true;
		}

		// Token: 0x060011FF RID: 4607 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x06001200 RID: 4608 RVA: 0x000C3A54 File Offset: 0x000C1C54
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			if (this.vc.ForwardVelocity < this.lowerSpeedThreshold)
			{
				return;
			}
			float num = Vector3.SignedAngle(this.vc.vehicleRigidbody.velocity, this.vc.vehicleTransform.forward, this.vc.vehicleTransform.up);
			num -= this.vc.steering.Angle * 0.5f;
			float num2 = (num < 0f) ? (-num) : num;
			if (this.vc.powertrain.engine.revLimiterActive || num2 < 2f)
			{
				return;
			}
			foreach (WheelComponent wheelComponent in this.vc.Wheels)
			{
				if (wheelComponent.IsGrounded)
				{
					float torque = -num * (float)wheelComponent.wheelController.vehicleSide * 50f * this.intensity;
					wheelComponent.AddBrakeTorque(torque, false);
				}
			}
		}

		// Token: 0x06001201 RID: 4609 RVA: 0x000C2B9B File Offset: 0x000C0D9B
		public override VehicleModule.ModuleCategory GetModuleCategory()
		{
			return VehicleModule.ModuleCategory.DrivingAssists;
		}

		// Token: 0x04002229 RID: 8745
		[Range(0f, 1f)]
		[ShowInSettings("ESC Intensity", 0f, 1f, 0.05f)]
		[Tooltip("    Intensity of stability control.")]
		public float intensity = 0.4f;

		// Token: 0x0400222A RID: 8746
		[Tooltip("ESC will not work below this speed.\r\nSetting this to a too low value might cause vehicle to be hard to steer at very low speeds.")]
		public float lowerSpeedThreshold = 4f;
	}
}
