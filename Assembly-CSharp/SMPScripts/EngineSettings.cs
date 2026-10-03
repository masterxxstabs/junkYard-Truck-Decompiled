using System;
using UnityEngine;

namespace SMPScripts
{
	// Token: 0x020001B6 RID: 438
	[Serializable]
	public class EngineSettings
	{
		// Token: 0x04001CC4 RID: 7364
		public int numOfGears;

		// Token: 0x04001CC5 RID: 7365
		public float torque;

		// Token: 0x04001CC6 RID: 7366
		public AnimationCurve accelerationCurve;

		// Token: 0x04001CC7 RID: 7367
		public float topSpeed;

		// Token: 0x04001CC8 RID: 7368
		public float reversingSpeed;

		// Token: 0x04001CC9 RID: 7369
		[Space]
		[Header("Engine Info")]
		public int currentGear;

		// Token: 0x04001CCA RID: 7370
		[Range(0f, 1f)]
		public float gearRatio;
	}
}
