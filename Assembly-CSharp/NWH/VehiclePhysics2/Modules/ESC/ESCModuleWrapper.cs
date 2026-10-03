using System;

namespace NWH.VehiclePhysics2.Modules.ESC
{
	// Token: 0x0200029F RID: 671
	[Serializable]
	public class ESCModuleWrapper : ModuleWrapper
	{
		// Token: 0x06001203 RID: 4611 RVA: 0x000C3B8E File Offset: 0x000C1D8E
		public override VehicleModule GetModule()
		{
			return this.module;
		}

		// Token: 0x06001204 RID: 4612 RVA: 0x000C3B96 File Offset: 0x000C1D96
		public override void SetModule(VehicleModule module)
		{
			this.module = (module as ESCModule);
		}

		// Token: 0x0400222B RID: 8747
		public ESCModule module = new ESCModule();
	}
}
