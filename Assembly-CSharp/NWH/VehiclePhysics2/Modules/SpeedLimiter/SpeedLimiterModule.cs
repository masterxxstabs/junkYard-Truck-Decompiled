using System;
using System.Linq;
using NWH.VehiclePhysics2.Demo;
using NWH.VehiclePhysics2.Powertrain;
using NWH.VehiclePhysics2.Utility;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.SpeedLimiter
{
	// Token: 0x0200028F RID: 655
	[Serializable]
	public class SpeedLimiterModule : VehicleModule
	{
		// Token: 0x060011B6 RID: 4534 RVA: 0x000B61C6 File Offset: 0x000B43C6
		public override void Initialize()
		{
			this.initialized = true;
		}

		// Token: 0x060011B7 RID: 4535 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x060011B8 RID: 4536 RVA: 0x00002188 File Offset: 0x00000388
		public override void Update()
		{
		}

		// Token: 0x060011B9 RID: 4537 RVA: 0x000C2CE0 File Offset: 0x000C0EE0
		public override void Enable()
		{
			base.Enable();
			if (this.vc != null && this.vc.powertrain.engine.powerModifiers.All((EngineComponent.PowerModifier p) => p != new EngineComponent.PowerModifier(this.SpeedPowerLimiter)))
			{
				this.vc.powertrain.engine.powerModifiers.Add(new EngineComponent.PowerModifier(this.SpeedPowerLimiter));
			}
		}

		// Token: 0x060011BA RID: 4538 RVA: 0x000C2D50 File Offset: 0x000C0F50
		public override void Disable()
		{
			base.Disable();
			this.active = false;
			if (this.vc != null)
			{
				this.vc.powertrain.engine.powerModifiers.RemoveAll((EngineComponent.PowerModifier p) => p == new EngineComponent.PowerModifier(this.SpeedPowerLimiter));
			}
		}

		// Token: 0x060011BB RID: 4539 RVA: 0x000C2B9B File Offset: 0x000C0D9B
		public override VehicleModule.ModuleCategory GetModuleCategory()
		{
			return VehicleModule.ModuleCategory.DrivingAssists;
		}

		// Token: 0x060011BC RID: 4540 RVA: 0x000C2DA0 File Offset: 0x000C0FA0
		public float SpeedPowerLimiter()
		{
			if (!base.Active || this.speedLimit == 0f)
			{
				this.active = false;
				return 1f;
			}
			float num = 0f;
			if (this.speedUnits == SpeedLimiterModule.SpeedUnits.ms)
			{
				num = this.speedLimit;
			}
			else if (this.speedUnits == SpeedLimiterModule.SpeedUnits.kmh)
			{
				num = UnitConverter.Speed_kmhToMs(this.speedLimit);
			}
			else if (this.speedUnits == SpeedLimiterModule.SpeedUnits.mph)
			{
				num = UnitConverter.Speed_mphToMs(this.speedLimit);
			}
			if (this.vc.Speed > num)
			{
				this.active = true;
				return 0f;
			}
			this.active = false;
			return 1f;
		}

		// Token: 0x040021EF RID: 8687
		public bool active;

		// Token: 0x040021F0 RID: 8688
		[ShowInSettings("Speed Limit", 0f, 100f, 5f)]
		[Tooltip("    Speed limit above which the throttle will be cut.")]
		public float speedLimit;

		// Token: 0x040021F1 RID: 8689
		[ShowInSettings("Units")]
		[Tooltip("    Units which will be used for speed limiter. Defaults to m/s.")]
		public SpeedLimiterModule.SpeedUnits speedUnits;

		// Token: 0x020004C0 RID: 1216
		public enum SpeedUnits
		{
			// Token: 0x04002C15 RID: 11285
			ms,
			// Token: 0x04002C16 RID: 11286
			kmh,
			// Token: 0x04002C17 RID: 11287
			mph
		}
	}
}
