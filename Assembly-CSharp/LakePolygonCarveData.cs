using System;
using UnityEngine;

// Token: 0x02000103 RID: 259
public class LakePolygonCarveData
{
	// Token: 0x04000E0E RID: 3598
	public float distSmooth;

	// Token: 0x04000E0F RID: 3599
	public float minX = float.MaxValue;

	// Token: 0x04000E10 RID: 3600
	public float maxX = float.MinValue;

	// Token: 0x04000E11 RID: 3601
	public float minZ = float.MaxValue;

	// Token: 0x04000E12 RID: 3602
	public float maxZ = float.MinValue;

	// Token: 0x04000E13 RID: 3603
	public Vector4[,] distances;
}
