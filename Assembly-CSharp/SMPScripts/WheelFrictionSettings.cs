using System;
using UnityEngine;

namespace SMPScripts
{
	// Token: 0x020001B7 RID: 439
	[Serializable]
	public class WheelFrictionSettings
	{
		// Token: 0x04001CCB RID: 7371
		public PhysicMaterial fPhysicMaterial;

		// Token: 0x04001CCC RID: 7372
		public PhysicMaterial rPhysicMaterial;

		// Token: 0x04001CCD RID: 7373
		public Vector2 fFriction;

		// Token: 0x04001CCE RID: 7374
		public Vector2 rFriction;
	}
}
