using System;

namespace NWH.VehiclePhysics2.Modules.Fuel
{
	// Token: 0x0200029B RID: 667
	[Serializable]
	public class FuelModuleWrapper : ModuleWrapper
	{
		// Token: 0x060011F3 RID: 4595 RVA: 0x000C3719 File Offset: 0x000C1919
		public override VehicleModule GetModule()
		{
			return this.module;
		}

		// Token: 0x060011F4 RID: 4596 RVA: 0x000C3721 File Offset: 0x000C1921
		public override void SetModule(VehicleModule module)
		{
			this.module = (module as FuelModule);
		}

		// Token: 0x04002219 RID: 8729
		public FuelModule module = new FuelModule();
	}
}
