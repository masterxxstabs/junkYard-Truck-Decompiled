using System;

namespace NWH.VehiclePhysics2.Modules.Trailer
{
	// Token: 0x0200028A RID: 650
	[Serializable]
	public class TrailerHitchModuleWrapper : ModuleWrapper
	{
		// Token: 0x0600119B RID: 4507 RVA: 0x000C28CB File Offset: 0x000C0ACB
		public override VehicleModule GetModule()
		{
			return this.module;
		}

		// Token: 0x0600119C RID: 4508 RVA: 0x000C28D3 File Offset: 0x000C0AD3
		public override void SetModule(VehicleModule module)
		{
			this.module = (module as TrailerHitchModule);
		}

		// Token: 0x040021E0 RID: 8672
		public TrailerHitchModule module = new TrailerHitchModule();
	}
}
