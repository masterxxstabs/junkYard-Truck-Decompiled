using System;

namespace NWH.VehiclePhysics2.Modules.Metrics
{
	// Token: 0x02000299 RID: 665
	[Serializable]
	public class MetricsModuleWrapper : ModuleWrapper
	{
		// Token: 0x060011E4 RID: 4580 RVA: 0x000C341F File Offset: 0x000C161F
		public override VehicleModule GetModule()
		{
			return this.module;
		}

		// Token: 0x060011E5 RID: 4581 RVA: 0x000C3427 File Offset: 0x000C1627
		public override void SetModule(VehicleModule module)
		{
			this.module = (module as MetricsModule);
		}

		// Token: 0x0400220C RID: 8716
		public MetricsModule module = new MetricsModule();
	}
}
