using System;
using UnityEngine;

namespace SMPScripts
{
	// Token: 0x020001B5 RID: 437
	[Serializable]
	public class MotoGeometry
	{
		// Token: 0x04001CBE RID: 7358
		public GameObject handles;

		// Token: 0x04001CBF RID: 7359
		public GameObject fVisualWheel;

		// Token: 0x04001CC0 RID: 7360
		public GameObject fPhysicsWheel;

		// Token: 0x04001CC1 RID: 7361
		public GameObject rPhysicsWheel;

		// Token: 0x04001CC2 RID: 7362
		[Space]
		[Header("Optional")]
		public GameObject secondaryFVisualWheel;

		// Token: 0x04001CC3 RID: 7363
		public GameObject secondaryFPhysicsWheel;
	}
}
