using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.Metrics
{
	// Token: 0x02000298 RID: 664
	[Serializable]
	public class MetricsModule : VehicleModule
	{
		// Token: 0x060011DF RID: 4575 RVA: 0x000B61C6 File Offset: 0x000B43C6
		public override void Initialize()
		{
			this.initialized = true;
		}

		// Token: 0x060011E0 RID: 4576 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x060011E1 RID: 4577 RVA: 0x000C32C8 File Offset: 0x000C14C8
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			bool hasWheelSkid = this.vc.powertrain.HasWheelSkid;
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			this.odometer.Update(() => this.vc.Speed * Time.fixedDeltaTime, true);
			this.topSpeed.Update(delegate
			{
				if (this.vc.Speed > this.topSpeed.value)
				{
					return this.vc.Speed;
				}
				return this.topSpeed.value;
			}, false);
			this.averageSpeed.Update(() => this.odometer.value / realtimeSinceStartup, false);
			this.totalDriftTime.Update(delegate
			{
				if (hasWheelSkid)
				{
					return this.vc.fixedDeltaTime;
				}
				return 0f;
			}, true);
			this.continousDriftTime.Update(delegate
			{
				if (hasWheelSkid)
				{
					this.driftEndTime = realtimeSinceStartup;
					return this.vc.fixedDeltaTime;
				}
				if (realtimeSinceStartup < this.driftEndTime + this.driftTimeout)
				{
					return this.vc.fixedDeltaTime;
				}
				return -this.continousDriftTime.value;
			}, true);
			this.totalDriftDistance.Update(delegate
			{
				if (hasWheelSkid)
				{
					return this.vc.fixedDeltaTime * this.vc.Speed;
				}
				return 0f;
			}, true);
			this.continousDriftDistance.Update(delegate
			{
				if (hasWheelSkid)
				{
					this.driftEndTime = realtimeSinceStartup;
					return this.vc.fixedDeltaTime * this.vc.Speed;
				}
				if (realtimeSinceStartup < this.driftEndTime + this.driftTimeout)
				{
					return this.vc.fixedDeltaTime * this.vc.Speed;
				}
				return -this.continousDriftDistance.value;
			}, true);
		}

		// Token: 0x060011E2 RID: 4578 RVA: 0x000915D6 File Offset: 0x0008F7D6
		public override VehicleModule.ModuleCategory GetModuleCategory()
		{
			return VehicleModule.ModuleCategory.Vehicle;
		}

		// Token: 0x04002203 RID: 8707
		public MetricsModule.Metric averageSpeed = new MetricsModule.Metric();

		// Token: 0x04002204 RID: 8708
		public MetricsModule.Metric continousDriftDistance = new MetricsModule.Metric();

		// Token: 0x04002205 RID: 8709
		public MetricsModule.Metric continousDriftTime = new MetricsModule.Metric();

		// Token: 0x04002206 RID: 8710
		public MetricsModule.Metric odometer = new MetricsModule.Metric();

		// Token: 0x04002207 RID: 8711
		public MetricsModule.Metric topSpeed = new MetricsModule.Metric();

		// Token: 0x04002208 RID: 8712
		public MetricsModule.Metric totalDriftDistance = new MetricsModule.Metric();

		// Token: 0x04002209 RID: 8713
		public MetricsModule.Metric totalDriftTime = new MetricsModule.Metric();

		// Token: 0x0400220A RID: 8714
		private float driftEndTime;

		// Token: 0x0400220B RID: 8715
		private float driftTimeout = 0.75f;

		// Token: 0x020004C1 RID: 1217
		[Serializable]
		public class Metric
		{
			// Token: 0x06001B25 RID: 6949 RVA: 0x000F856B File Offset: 0x000F676B
			public void Update(MetricsModule.Metric.UpdateDelegate del, bool increment)
			{
				if (increment)
				{
					this.value += del();
					return;
				}
				this.value = del();
			}

			// Token: 0x06001B26 RID: 6950 RVA: 0x000F8590 File Offset: 0x000F6790
			public void Reset()
			{
				this.value = 0f;
			}

			// Token: 0x04002C18 RID: 11288
			public float value;

			// Token: 0x02000505 RID: 1285
			// (Invoke) Token: 0x06001BA6 RID: 7078
			public delegate float UpdateDelegate();
		}
	}
}
