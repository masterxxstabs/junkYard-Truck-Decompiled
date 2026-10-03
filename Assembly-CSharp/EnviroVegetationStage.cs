using System;
using UnityEngine;

// Token: 0x0200008C RID: 140
[Serializable]
public class EnviroVegetationStage
{
	// Token: 0x040006BE RID: 1726
	[Range(0f, 100f)]
	public float minAgePercent;

	// Token: 0x040006BF RID: 1727
	public EnviroVegetationStage.GrowState growAction;

	// Token: 0x040006C0 RID: 1728
	public GameObject GrowGameobjectSpring;

	// Token: 0x040006C1 RID: 1729
	public GameObject GrowGameobjectSummer;

	// Token: 0x040006C2 RID: 1730
	public GameObject GrowGameobjectAutumn;

	// Token: 0x040006C3 RID: 1731
	public GameObject GrowGameobjectWinter;

	// Token: 0x040006C4 RID: 1732
	public bool billboard;

	// Token: 0x02000392 RID: 914
	public enum GrowState
	{
		// Token: 0x04002744 RID: 10052
		Grow,
		// Token: 0x04002745 RID: 10053
		Stay
	}
}
