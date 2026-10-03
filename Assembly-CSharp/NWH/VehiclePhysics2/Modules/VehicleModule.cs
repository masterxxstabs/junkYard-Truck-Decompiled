using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules
{
	// Token: 0x02000287 RID: 647
	[DisallowMultipleComponent]
	[Serializable]
	public abstract class VehicleModule : VehicleComponent
	{
		// Token: 0x06001181 RID: 4481
		public abstract VehicleModule.ModuleCategory GetModuleCategory();

		// Token: 0x020004BF RID: 1215
		public enum ModuleCategory
		{
			// Token: 0x04002C0A RID: 11274
			Other,
			// Token: 0x04002C0B RID: 11275
			Vehicle,
			// Token: 0x04002C0C RID: 11276
			DrivingAssists,
			// Token: 0x04002C0D RID: 11277
			Aero,
			// Token: 0x04002C0E RID: 11278
			Sound,
			// Token: 0x04002C0F RID: 11279
			Effects,
			// Token: 0x04002C10 RID: 11280
			Trailer,
			// Token: 0x04002C11 RID: 11281
			Animation,
			// Token: 0x04002C12 RID: 11282
			Control,
			// Token: 0x04002C13 RID: 11283
			Powertrain
		}
	}
}
