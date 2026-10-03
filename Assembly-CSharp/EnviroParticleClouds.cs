using System;
using UnityEngine;

// Token: 0x02000086 RID: 134
[Serializable]
public class EnviroParticleClouds
{
	// Token: 0x04000665 RID: 1637
	[Tooltip("Particle clouds height.")]
	[Range(0.01f, 0.2f)]
	public float height = 0.1f;

	// Token: 0x04000666 RID: 1638
	[Tooltip("Global Color for flat clouds based sun positon.")]
	public Gradient particleCloudsColor;
}
