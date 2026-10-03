using System;
using System.Collections.Generic;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.Rigging
{
	// Token: 0x02000292 RID: 658
	[Serializable]
	public class RiggingModule : VehicleModule
	{
		// Token: 0x060011C6 RID: 4550 RVA: 0x000B61C6 File Offset: 0x000B43C6
		public override void Initialize()
		{
			this.initialized = true;
		}

		// Token: 0x060011C7 RID: 4551 RVA: 0x000C2FEC File Offset: 0x000C11EC
		public override void Awake(VehicleController vc)
		{
			base.Awake(vc);
			foreach (Bone bone in this.bones)
			{
				bone.Initialize();
			}
		}

		// Token: 0x060011C8 RID: 4552 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x060011C9 RID: 4553 RVA: 0x000C3044 File Offset: 0x000C1244
		public override void Update()
		{
			Vector3 forward = this.vc.vehicleTransform.forward;
			Vector3 up = this.vc.vehicleTransform.up;
			foreach (Bone bone in this.bones)
			{
				bone.Update(forward, up);
			}
		}

		// Token: 0x060011CA RID: 4554 RVA: 0x000C30B8 File Offset: 0x000C12B8
		public override VehicleModule.ModuleCategory GetModuleCategory()
		{
			return VehicleModule.ModuleCategory.Animation;
		}

		// Token: 0x040021FB RID: 8699
		public List<Bone> bones = new List<Bone>();
	}
}
