using System;
using UnityEngine;

// Token: 0x02000075 RID: 117
[Serializable]
public class EnviroFogging
{
	// Token: 0x0400053B RID: 1339
	[HideInInspector]
	public float skyFogStart;

	// Token: 0x0400053C RID: 1340
	[HideInInspector]
	public float skyFogHeight = 1f;

	// Token: 0x0400053D RID: 1341
	[HideInInspector]
	public float skyFogIntensity = 0.1f;

	// Token: 0x0400053E RID: 1342
	[HideInInspector]
	public float skyFogStrength = 0.1f;

	// Token: 0x0400053F RID: 1343
	[HideInInspector]
	public float scatteringStrenght = 0.5f;

	// Token: 0x04000540 RID: 1344
	[HideInInspector]
	public float sunBlocking = 0.5f;

	// Token: 0x04000541 RID: 1345
	[HideInInspector]
	public float moonIntensity = 1f;
}
