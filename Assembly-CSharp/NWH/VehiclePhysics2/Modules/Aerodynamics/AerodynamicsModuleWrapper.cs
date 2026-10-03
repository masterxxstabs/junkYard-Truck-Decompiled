using System;

namespace NWH.VehiclePhysics2.Modules.Aerodynamics
{
	// Token: 0x020002A3 RID: 675
	[Serializable]
	public class AerodynamicsModuleWrapper : ModuleWrapper
	{
		// Token: 0x06001215 RID: 4629 RVA: 0x000C40F1 File Offset: 0x000C22F1
		public override VehicleModule.ModuleCategory GetCategory()
		{
			return this.module.GetModuleCategory();
		}

		// Token: 0x06001216 RID: 4630 RVA: 0x000C40FE File Offset: 0x000C22FE
		public override VehicleModule GetModule()
		{
			return this.module;
		}

		// Token: 0x06001217 RID: 4631 RVA: 0x000C4106 File Offset: 0x000C2306
		public override void SetModule(VehicleModule module)
		{
			this.module = (module as AerodynamicsModule);
		}

		// Token: 0x04002246 RID: 8774
		public AerodynamicsModule module = new AerodynamicsModule();
	}
}
