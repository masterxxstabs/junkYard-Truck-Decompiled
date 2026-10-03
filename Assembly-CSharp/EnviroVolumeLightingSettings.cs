using System;
using UnityEngine;

// Token: 0x02000082 RID: 130
[Serializable]
public class EnviroVolumeLightingSettings
{
	// Token: 0x04000622 RID: 1570
	[Tooltip("Downsampling of volume light rendering.")]
	public EnviroSkyRendering.VolumtericResolution Resolution = EnviroSkyRendering.VolumtericResolution.Quarter;

	// Token: 0x04000623 RID: 1571
	[Tooltip("Activate or deactivate directional volume light rendering.")]
	public bool dirVolumeLighting = true;

	// Token: 0x04000624 RID: 1572
	[Header("Quality")]
	[Range(1f, 64f)]
	public int SampleCount = 8;

	// Token: 0x04000625 RID: 1573
	[Header("Light Settings")]
	public AnimationCurve ScatteringCoef = new AnimationCurve();

	// Token: 0x04000626 RID: 1574
	[Range(0f, 0.1f)]
	public float ExtinctionCoef = 0.05f;

	// Token: 0x04000627 RID: 1575
	[Range(0f, 0.999f)]
	public float Anistropy = 0.1f;

	// Token: 0x04000628 RID: 1576
	public float MaxRayLength = 10f;

	// Token: 0x04000629 RID: 1577
	[Header("3D Noise")]
	[Tooltip("Use 3D noise for directional lighting. Attention: Expensive operation for directional lights with high sample count!")]
	public bool directLightNoise;

	// Token: 0x0400062A RID: 1578
	[Range(0f, 1f)]
	[Tooltip("The noise intensity volume lighting.")]
	public float noiseIntensity = 1f;

	// Token: 0x0400062B RID: 1579
	[Tooltip("The noise intensity offset of volume lighting.")]
	[Range(0f, 1f)]
	public float noiseIntensityOffset = 0.3f;

	// Token: 0x0400062C RID: 1580
	[Range(0f, 0.1f)]
	[Tooltip("The noise scaling of volume lighting.")]
	public float noiseScale = 0.001f;

	// Token: 0x0400062D RID: 1581
	[Tooltip("The speed and direction of volume lighting.")]
	public Vector2 noiseVelocity = new Vector2(3f, 1.5f);
}
