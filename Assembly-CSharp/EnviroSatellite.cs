using System;
using UnityEngine;

// Token: 0x02000073 RID: 115
[Serializable]
public class EnviroSatellite
{
	// Token: 0x04000523 RID: 1315
	[Tooltip("Name of this satellite")]
	public string name;

	// Token: 0x04000524 RID: 1316
	[Tooltip("Prefab with model that get instantiated.")]
	public GameObject prefab;

	// Token: 0x04000525 RID: 1317
	[Tooltip("Orbit distance.")]
	public float orbit;

	// Token: 0x04000526 RID: 1318
	[Tooltip("Orbit modification on x axis.")]
	public float xRot;

	// Token: 0x04000527 RID: 1319
	[Tooltip("Orbit modification on y axis.")]
	public float yRot;
}
