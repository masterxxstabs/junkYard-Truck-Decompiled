using System;
using NWH.VehiclePhysics2.Demo;
using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.WheelController3D
{
	// Token: 0x02000249 RID: 585
	[Serializable]
	public class Damper
	{
		// Token: 0x04001F3E RID: 7998
		public const float maxVelocity = 100f;

		// Token: 0x04001F3F RID: 7999
		[FormerlySerializedAs("unitBumpForce")]
		[ShowInSettings("Damper Bump Force", 500f, 5000f, 200f)]
		[Tooltip("    Bump force of the damper.")]
		public float bumpForce = 2000f;

		// Token: 0x04001F40 RID: 8000
		[Tooltip("Curve where X axis represents speed of travel of the suspension and Y axis represents resultant force.\r\nBoth values are normalized to [0,1].")]
		public AnimationCurve curve;

		// Token: 0x04001F41 RID: 8001
		[ShowInTelemetry]
		[Tooltip("    Current damper force.")]
		public float force;

		// Token: 0x04001F42 RID: 8002
		[FormerlySerializedAs("unitReboundForce")]
		[ShowInSettings("Damper Rebound Force", 500f, 5000f, 200f)]
		[Tooltip("    Rebound force of the damper.")]
		public float reboundForce = 2400f;
	}
}
