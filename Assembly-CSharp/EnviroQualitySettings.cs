using System;
using UnityEngine;

// Token: 0x02000078 RID: 120
[Serializable]
public class EnviroQualitySettings
{
	// Token: 0x040005A3 RID: 1443
	[Range(0f, 1f)]
	[Tooltip("Modifies the amount of particles used in weather effects.")]
	public float GlobalParticleEmissionRates = 1f;

	// Token: 0x040005A4 RID: 1444
	[Tooltip("How often Enviro Growth Instances should be updated. Lower value = smoother growth and more frequent updates but more perfomance hungry!")]
	public float UpdateInterval = 0.5f;
}
