using System;

namespace NWH.VehiclePhysics2.Modules.Rigging
{
	// Token: 0x02000293 RID: 659
	[Serializable]
	public class RiggingModuleWrapper : ModuleWrapper
	{
		// Token: 0x060011CC RID: 4556 RVA: 0x000C30CE File Offset: 0x000C12CE
		public override VehicleModule GetModule()
		{
			return this.module;
		}

		// Token: 0x060011CD RID: 4557 RVA: 0x000C30D6 File Offset: 0x000C12D6
		public override void SetModule(VehicleModule module)
		{
			this.module = (module as RiggingModule);
		}

		// Token: 0x040021FC RID: 8700
		public RiggingModule module = new RiggingModule();
	}
}
