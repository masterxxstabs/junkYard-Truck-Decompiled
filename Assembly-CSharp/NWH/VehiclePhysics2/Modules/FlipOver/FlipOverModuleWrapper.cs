using System;

namespace NWH.VehiclePhysics2.Modules.FlipOver
{
	// Token: 0x0200029D RID: 669
	[Serializable]
	public class FlipOverModuleWrapper : ModuleWrapper
	{
		// Token: 0x060011FB RID: 4603 RVA: 0x000C3A28 File Offset: 0x000C1C28
		public override VehicleModule GetModule()
		{
			return this.module;
		}

		// Token: 0x060011FC RID: 4604 RVA: 0x000C3A30 File Offset: 0x000C1C30
		public override void SetModule(VehicleModule module)
		{
			this.module = (module as FlipOverModule);
		}

		// Token: 0x04002228 RID: 8744
		public FlipOverModule module = new FlipOverModule();
	}
}
