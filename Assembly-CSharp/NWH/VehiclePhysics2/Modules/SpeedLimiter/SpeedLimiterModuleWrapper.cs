using System;

namespace NWH.VehiclePhysics2.Modules.SpeedLimiter
{
	// Token: 0x02000290 RID: 656
	[Serializable]
	public class SpeedLimiterModuleWrapper : ModuleWrapper
	{
		// Token: 0x060011C0 RID: 4544 RVA: 0x000C2E69 File Offset: 0x000C1069
		public override VehicleModule GetModule()
		{
			return this.module;
		}

		// Token: 0x060011C1 RID: 4545 RVA: 0x000C2E71 File Offset: 0x000C1071
		public override void SetModule(VehicleModule module)
		{
			this.module = (module as SpeedLimiterModule);
		}

		// Token: 0x040021F2 RID: 8690
		public SpeedLimiterModule module = new SpeedLimiterModule();
	}
}
