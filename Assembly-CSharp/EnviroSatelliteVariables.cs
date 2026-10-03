using System;
using UnityEngine;

// Token: 0x02000079 RID: 121
[Serializable]
public class EnviroSatelliteVariables
{
	// Token: 0x040005A5 RID: 1445
	[Tooltip("Name of this satellite")]
	public string name;

	// Token: 0x040005A6 RID: 1446
	[Tooltip("Prefab with model that get instantiated.")]
	public GameObject prefab;

	// Token: 0x040005A7 RID: 1447
	[Tooltip("This value will influence the satellite orbitpositions.")]
	public float orbit_X;

	// Token: 0x040005A8 RID: 1448
	[Tooltip("This value will influence the satellite orbitpositions.")]
	public float orbit_Y;

	// Token: 0x040005A9 RID: 1449
	[Tooltip("The speed of the satellites orbit.")]
	public float speed;
}
