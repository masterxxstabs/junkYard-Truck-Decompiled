using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Demo;
using NWH.VehiclePhysics2.Powertrain;
using UnityEngine;
using UnityEngine.Events;

namespace NWH.VehiclePhysics2.Modules.ABS
{
	// Token: 0x020002A5 RID: 677
	[Serializable]
	public class ABSModule : VehicleModule
	{
		// Token: 0x0600121A RID: 4634 RVA: 0x000B61C6 File Offset: 0x000B43C6
		public override void Initialize()
		{
			this.initialized = true;
		}

		// Token: 0x0600121B RID: 4635 RVA: 0x000C4127 File Offset: 0x000C2327
		public override void Awake(VehicleController vc)
		{
			base.Awake(vc);
		}

		// Token: 0x0600121C RID: 4636 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x0600121D RID: 4637 RVA: 0x00002188 File Offset: 0x00000388
		public override void Update()
		{
		}

		// Token: 0x0600121E RID: 4638 RVA: 0x000C4130 File Offset: 0x000C2330
		public override void Enable()
		{
			base.Enable();
			bool flag = true;
			using (List<Brakes.BrakeTorqueModifier>.Enumerator enumerator = this.vc.brakes.brakeTorqueModifiers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == new Brakes.BrakeTorqueModifier(this.BrakeTorqueModifier))
					{
						flag = false;
						break;
					}
				}
			}
			if (this.vc != null && flag)
			{
				this.vc.brakes.brakeTorqueModifiers.Add(new Brakes.BrakeTorqueModifier(this.BrakeTorqueModifier));
			}
		}

		// Token: 0x0600121F RID: 4639 RVA: 0x000C41D8 File Offset: 0x000C23D8
		public override void Disable()
		{
			base.Disable();
			this.active = false;
			if (this.vc != null)
			{
				this.vc.brakes.brakeTorqueModifiers.RemoveAll((Brakes.BrakeTorqueModifier p) => p == new Brakes.BrakeTorqueModifier(this.BrakeTorqueModifier));
			}
		}

		// Token: 0x06001220 RID: 4640 RVA: 0x000C4218 File Offset: 0x000C2418
		public float BrakeTorqueModifier()
		{
			if (!base.Active)
			{
				return 1f;
			}
			this.active = false;
			this.slipThreshold = ((this.slipThreshold < 0f) ? 0f : ((this.slipThreshold > 1f) ? 1f : this.slipThreshold));
			if (this.vc.ForwardVelocity < this.lowerSpeedThreshold)
			{
				return 1f;
			}
			if (this.vc.brakes.Active && !this.vc.powertrain.engine.revLimiterActive && this.vc.input.Handbrake < 0.1f)
			{
				for (int i = 0; i < this.vc.Wheels.Count; i++)
				{
					WheelComponent wheelComponent = this.vc.Wheels[i];
					if (wheelComponent.IsGrounded)
					{
						float longitudinalSlip = wheelComponent.LongitudinalSlip;
						if (longitudinalSlip > 0f && longitudinalSlip > this.slipThreshold)
						{
							this.active = true;
							this.ABSActive.Invoke();
							return 0.01f;
						}
					}
				}
			}
			return 1f;
		}

		// Token: 0x06001221 RID: 4641 RVA: 0x000C2B9B File Offset: 0x000C0D9B
		public override VehicleModule.ModuleCategory GetModuleCategory()
		{
			return VehicleModule.ModuleCategory.DrivingAssists;
		}

		// Token: 0x04002249 RID: 8777
		[Tooltip("    Called each frame while ABS is a active.")]
		public UnityEvent ABSActive = new UnityEvent();

		// Token: 0x0400224A RID: 8778
		[Tooltip("    Is ABS currently active?")]
		public bool active;

		// Token: 0x0400224B RID: 8779
		[Tooltip("    ABS will not work below this speed.")]
		public float lowerSpeedThreshold = 1f;

		// Token: 0x0400224C RID: 8780
		[Range(0f, 1f)]
		[Tooltip("Longitudinal slip required for ABS to trigger.")]
		[ShowInSettings("Slip Threshold", 0f, 1f, 0.05f)]
		public float slipThreshold = 0.1f;
	}
}
