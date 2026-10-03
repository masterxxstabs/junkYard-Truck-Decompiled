using System;

namespace NWH.VehiclePhysics2.Modules.NOS
{
	// Token: 0x02000294 RID: 660
	[Serializable]
	public class NOSModuleWrapper : ModuleWrapper
	{
		// Token: 0x060011CF RID: 4559 RVA: 0x000C30F7 File Offset: 0x000C12F7
		public override VehicleModule GetModule()
		{
			return this.module;
		}

		// Token: 0x060011D0 RID: 4560 RVA: 0x000C30FF File Offset: 0x000C12FF
		public override void SetModule(VehicleModule module)
		{
			this.module = (module as NOSModule);
		}

		// Token: 0x040021FD RID: 8701
		public NOSModule module = new NOSModule();
	}
}
