using System;

namespace NWH.VehiclePhysics2.Modules.TCS
{
	// Token: 0x0200028E RID: 654
	[Serializable]
	public class TCSModuleWrapper : ModuleWrapper
	{
		// Token: 0x060011B3 RID: 4531 RVA: 0x000C2CB5 File Offset: 0x000C0EB5
		public override VehicleModule GetModule()
		{
			return this.module;
		}

		// Token: 0x060011B4 RID: 4532 RVA: 0x000C2CBD File Offset: 0x000C0EBD
		public override void SetModule(VehicleModule module)
		{
			this.module = (module as TCSModule);
		}

		// Token: 0x040021EE RID: 8686
		public TCSModule module = new TCSModule();
	}
}
