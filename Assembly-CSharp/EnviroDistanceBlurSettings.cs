using System;
using UnityEngine;

// Token: 0x02000080 RID: 128
[Serializable]
public class EnviroDistanceBlurSettings
{
	// Token: 0x0400060A RID: 1546
	public bool antiFlicker = true;

	// Token: 0x0400060B RID: 1547
	public bool highQuality = true;

	// Token: 0x0400060C RID: 1548
	[Range(1f, 7f)]
	public float radius = 7f;
}
