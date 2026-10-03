using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Demo;
using NWH.VehiclePhysics2.Powertrain;
using UnityEngine;
using UnityEngine.Events;

namespace NWH.VehiclePhysics2.Modules.TCS
{
	// Token: 0x0200028D RID: 653
	[Serializable]
	public class TCSModule : VehicleModule
	{
		// Token: 0x060011AA RID: 4522 RVA: 0x000B61C6 File Offset: 0x000B43C6
		public override void Initialize()
		{
			this.initialized = true;
		}

		// Token: 0x060011AB RID: 4523 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x060011AC RID: 4524 RVA: 0x00002188 File Offset: 0x00000388
		public override void Update()
		{
		}

		// Token: 0x060011AD RID: 4525 RVA: 0x000C2A9C File Offset: 0x000C0C9C
		public override void Enable()
		{
			base.Enable();
			if (this.vc != null)
			{
				bool flag = true;
				using (List<EngineComponent.PowerModifier>.Enumerator enumerator = this.vc.powertrain.engine.powerModifiers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current == new EngineComponent.PowerModifier(this.TCSPowerLimiter))
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					this.vc.powertrain.engine.powerModifiers.Add(new EngineComponent.PowerModifier(this.TCSPowerLimiter));
				}
			}
		}

		// Token: 0x060011AE RID: 4526 RVA: 0x000C2B4C File Offset: 0x000C0D4C
		public override void Disable()
		{
			base.Disable();
			this.active = false;
			if (this.vc != null)
			{
				this.vc.powertrain.engine.powerModifiers.RemoveAll((EngineComponent.PowerModifier p) => p == new EngineComponent.PowerModifier(this.TCSPowerLimiter));
			}
		}

		// Token: 0x060011AF RID: 4527 RVA: 0x000C2B9B File Offset: 0x000C0D9B
		public override VehicleModule.ModuleCategory GetModuleCategory()
		{
			return VehicleModule.ModuleCategory.DrivingAssists;
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x000C2BA0 File Offset: 0x000C0DA0
		public float TCSPowerLimiter()
		{
			this.active = false;
			if (!base.Active)
			{
				return 1f;
			}
			if (this.vc.Speed > this.lowerSpeedThreshold)
			{
				foreach (WheelComponent wheelComponent in this.vc.Wheels)
				{
					if (wheelComponent.IsGrounded && !this.vc.powertrain.transmission.IsShifting)
					{
						float longitudinalSlip = wheelComponent.LongitudinalSlip;
						if (longitudinalSlip < 0f && longitudinalSlip < -this.slipThreshold)
						{
							this.active = true;
							this.TCSActive.Invoke();
							return 0f;
						}
					}
				}
			}
			return 1f;
		}

		// Token: 0x040021EA RID: 8682
		public bool active;

		// Token: 0x040021EB RID: 8683
		[Tooltip("    Speed under which TCS will not work.")]
		public float lowerSpeedThreshold = 2f;

		// Token: 0x040021EC RID: 8684
		[Range(0f, 1f)]
		[ShowInSettings("Slip Threshold", 0f, 1f, 0.05f)]
		[Tooltip("    Longitudinal slip threshold at which TCS will activate.")]
		public float slipThreshold = 0.1f;

		// Token: 0x040021ED RID: 8685
		[Tooltip("    Called each frame while TCS is active.")]
		public UnityEvent TCSActive = new UnityEvent();
	}
}
