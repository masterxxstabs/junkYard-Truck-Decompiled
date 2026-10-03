using System;
using System.Collections.Generic;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.ModuleTemplate
{
	// Token: 0x02000296 RID: 662
	[Serializable]
	public class ModuleTemplate : VehicleModule
	{
		// Token: 0x060011D7 RID: 4567 RVA: 0x000B61C6 File Offset: 0x000B43C6
		public override void Initialize()
		{
			this.initialized = true;
		}

		// Token: 0x060011D8 RID: 4568 RVA: 0x000C3283 File Offset: 0x000C1483
		public override void FixedUpdate()
		{
			bool active = base.Active;
		}

		// Token: 0x060011D9 RID: 4569 RVA: 0x000C3283 File Offset: 0x000C1483
		public override void Update()
		{
			bool active = base.Active;
		}

		// Token: 0x060011DA RID: 4570 RVA: 0x000116EA File Offset: 0x0000F8EA
		public override VehicleModule.ModuleCategory GetModuleCategory()
		{
			return VehicleModule.ModuleCategory.Other;
		}

		// Token: 0x04002200 RID: 8704
		[Range(0f, 1f)]
		[Tooltip("    Example float field.")]
		public float floatExample;

		// Token: 0x04002201 RID: 8705
		[Tooltip("    Example list field.")]
		public List<int> listExample = new List<int>();
	}
}
