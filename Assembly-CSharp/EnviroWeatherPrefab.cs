using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000090 RID: 144
[Serializable]
public class EnviroWeatherPrefab : MonoBehaviour
{
	// Token: 0x040006E6 RID: 1766
	public EnviroWeatherPreset weatherPreset;

	// Token: 0x040006E7 RID: 1767
	[HideInInspector]
	public List<ParticleSystem> effectSystems = new List<ParticleSystem>();

	// Token: 0x040006E8 RID: 1768
	[HideInInspector]
	public List<float> effectEmmisionRates = new List<float>();
}
