using System;

namespace NWH.VehiclePhysics2.Modules.CruiseControl
{
	// Token: 0x020002A1 RID: 673
	[Serializable]
	public class CruiseControlModuleWrapper : ModuleWrapper
	{
		// Token: 0x0600120C RID: 4620 RVA: 0x000C3D7A File Offset: 0x000C1F7A
		public override VehicleModule GetModule()
		{
			return this.module;
		}

		// Token: 0x0600120D RID: 4621 RVA: 0x000C3D82 File Offset: 0x000C1F82
		public override void SetModule(VehicleModule module)
		{
			this.module = (module as CruiseControlModule);
		}

		// Token: 0x04002237 RID: 8759
		public CruiseControlModule module = new CruiseControlModule();
	}
}
