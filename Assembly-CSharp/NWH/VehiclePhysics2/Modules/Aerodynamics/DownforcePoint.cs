using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.Aerodynamics
{
	// Token: 0x020002A4 RID: 676
	[Serializable]
	public class DownforcePoint
	{
		// Token: 0x04002247 RID: 8775
		[Tooltip("Maximim force in [N] that can be applied as a result of downforce.\r\nPutting in a too large value will make the vehicle bottom out at high speeds if suspension is too soft.")]
		public float maxForce;

		// Token: 0x04002248 RID: 8776
		[Tooltip("Position relative to the vehicle at which downforce will be applied. Marked by red arrow gizmo.\r\nY component should be at about the spring anchor height (i.e. WheelController position).")]
		public Vector3 position;
	}
}
