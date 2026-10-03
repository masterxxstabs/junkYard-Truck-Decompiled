using System;

namespace NWH.VehiclePhysics2.Modules.Trailer
{
	// Token: 0x0200028C RID: 652
	[Serializable]
	public class TrailerModuleWrapper : ModuleWrapper
	{
		// Token: 0x060011A7 RID: 4519 RVA: 0x000C2A70 File Offset: 0x000C0C70
		public override VehicleModule GetModule()
		{
			return this.module;
		}

		// Token: 0x060011A8 RID: 4520 RVA: 0x000C2A78 File Offset: 0x000C0C78
		public override void SetModule(VehicleModule module)
		{
			this.module = (module as TrailerModule);
		}

		// Token: 0x040021E9 RID: 8681
		public TrailerModule module = new TrailerModule();
	}
}
