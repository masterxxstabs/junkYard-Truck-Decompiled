using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules
{
	// Token: 0x02000286 RID: 646
	[Serializable]
	public abstract class ModuleWrapper : MonoBehaviour
	{
		// Token: 0x0600117D RID: 4477 RVA: 0x000C1F93 File Offset: 0x000C0193
		public virtual VehicleModule.ModuleCategory GetCategory()
		{
			return this.GetModule().GetModuleCategory();
		}

		// Token: 0x0600117E RID: 4478
		public abstract VehicleModule GetModule();

		// Token: 0x0600117F RID: 4479
		public abstract void SetModule(VehicleModule vehicleModule);
	}
}
