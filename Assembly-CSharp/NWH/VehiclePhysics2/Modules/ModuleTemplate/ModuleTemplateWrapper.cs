using System;

namespace NWH.VehiclePhysics2.Modules.ModuleTemplate
{
	// Token: 0x02000297 RID: 663
	[Serializable]
	public class ModuleTemplateWrapper : ModuleWrapper
	{
		// Token: 0x060011DC RID: 4572 RVA: 0x000C329F File Offset: 0x000C149F
		public override VehicleModule GetModule()
		{
			return this.module;
		}

		// Token: 0x060011DD RID: 4573 RVA: 0x000C32A7 File Offset: 0x000C14A7
		public override void SetModule(VehicleModule module)
		{
			this.module = (module as ModuleTemplate);
		}

		// Token: 0x04002202 RID: 8706
		public ModuleTemplate module = new ModuleTemplate();
	}
}
