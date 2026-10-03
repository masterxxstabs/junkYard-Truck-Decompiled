using System;
using UnityEngine;

namespace SMPScripts
{
	// Token: 0x020001B8 RID: 440
	[Serializable]
	public class AirTimeSettings
	{
		// Token: 0x04001CCF RID: 7375
		public bool freestyle;

		// Token: 0x04001CD0 RID: 7376
		public float airTimeRotationSensitivity;

		// Token: 0x04001CD1 RID: 7377
		[Range(0.5f, 10f)]
		public float heightThreshold;

		// Token: 0x04001CD2 RID: 7378
		public float groundSnapSensitivity;
	}
}
