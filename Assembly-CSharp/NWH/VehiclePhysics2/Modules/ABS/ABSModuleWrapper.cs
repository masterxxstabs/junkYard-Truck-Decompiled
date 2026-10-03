using System;

namespace NWH.VehiclePhysics2.Modules.ABS
{
	// Token: 0x020002A6 RID: 678
	[Serializable]
	public class ABSModuleWrapper : ModuleWrapper
	{
		// Token: 0x06001224 RID: 4644 RVA: 0x000C4371 File Offset: 0x000C2571
		public override VehicleModule.ModuleCategory GetCategory()
		{
			return this.module.GetModuleCategory();
		}

		// Token: 0x06001225 RID: 4645 RVA: 0x000C437E File Offset: 0x000C257E
		public override VehicleModule GetModule()
		{
			return this.module;
		}

		// Token: 0x06001226 RID: 4646 RVA: 0x000C4386 File Offset: 0x000C2586
		public override void SetModule(VehicleModule module)
		{
			this.module = (module as ABSModule);
		}

		// Token: 0x0400224D RID: 8781
		public ABSModule module = new ABSModule();
	}
}
