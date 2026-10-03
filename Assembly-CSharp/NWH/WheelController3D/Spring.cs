using System;
using NWH.VehiclePhysics2.Demo;
using UnityEngine;

namespace NWH.WheelController3D
{
	// Token: 0x0200024C RID: 588
	[Serializable]
	public class Spring
	{
		// Token: 0x04001F50 RID: 8016
		[Tooltip("    Is the suspension currently bottomed out? True when spring.length <= 0.")]
		public bool bottomedOut;

		// Token: 0x04001F51 RID: 8017
		[Tooltip("Coefficient modifying the force of suspension hitting bump stop (fully compressing).\r\nToo low values will result with wheel passing through ground as the reaction force will be too low,\r\nand too high values will result in vehicle overreacting and bouncing up after bottoming out.\r\nBottoming out usually happens due to:\r\n- Too weak springs\r\n- Falling from large height\r\n- Too large Time.fixedDeltaTime combined with short suspension travel")]
		public float bottomOutForceCoefficient = 1f;

		// Token: 0x04001F52 RID: 8018
		[Tooltip("    How much is spring currently compressed. 0 means fully relaxed and 1 fully compressed.")]
		public float compressionPercent;

		// Token: 0x04001F53 RID: 8019
		[ShowInTelemetry]
		[Tooltip("    Current force the spring is exerting in [N].")]
		public float force;

		// Token: 0x04001F54 RID: 8020
		[Tooltip("Force curve where X axis represents spring travel [0,1] and Y axis represents force coefficient [0, 1].\r\nForce coefficient is multiplied by maxForce to get the final spring force.")]
		public AnimationCurve forceCurve;

		// Token: 0x04001F55 RID: 8021
		[ShowInTelemetry]
		[Tooltip("    Current length of the spring.")]
		public float length;

		// Token: 0x04001F56 RID: 8022
		[ShowInSettings("Spring Force", 5000f, 30000f, 1000f)]
		[Tooltip("    Maximum force spring can exert.")]
		public float maxForce = 16000f;

		// Token: 0x04001F57 RID: 8023
		[ShowInSettings("Spring Length", 0.2f, 0.8f, 0.025f)]
		[Tooltip("    Length of fully relaxed spring.")]
		public float maxLength = 0.35f;

		// Token: 0x04001F58 RID: 8024
		[Tooltip("    Is the spring over extended. Opposite of bottomed out.")]
		public bool overExtended;

		// Token: 0x04001F59 RID: 8025
		public float prevLength;

		// Token: 0x04001F5A RID: 8026
		public Vector3 targetPoint;

		// Token: 0x04001F5B RID: 8027
		[Tooltip("    Rate of change of the length of the spring in [m/s].")]
		public float velocity;
	}
}
