using System;
using UnityEngine;

namespace NWH.VehiclePhysics2
{
	// Token: 0x02000254 RID: 596
	[CreateAssetMenu(fileName = "NWH Vehicle Physics", menuName = "NWH Vehicle Physics/LOD", order = 1)]
	[Serializable]
	public class LOD : ScriptableObject
	{
		// Token: 0x0400200F RID: 8207
		public float distance;

		// Token: 0x04002010 RID: 8208
		public bool singleRayGroundDetection;
	}
}
