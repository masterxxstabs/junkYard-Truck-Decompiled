using System;
using UnityEngine;

// Token: 0x02000091 RID: 145
[Serializable]
public class EnviroWeatherCloudsConfig
{
	// Token: 0x040006E9 RID: 1769
	[Tooltip("Ambient Light Intensity.")]
	[Range(0f, 1f)]
	public float ambientSkyColorIntensity = 1f;

	// Token: 0x040006EA RID: 1770
	[Tooltip("Light extinction factor.")]
	[Range(0f, 2f)]
	public float scatteringCoef = 1f;

	// Token: 0x040006EB RID: 1771
	[Tooltip("Darkens the edges of clouds from in-out scattering.")]
	[Range(1f, 3f)]
	public float edgeDarkness = 2f;

	// Token: 0x040006EC RID: 1772
	public float baseErosionIntensity;

	// Token: 0x040006ED RID: 1773
	public float detailErosionIntensity = 0.2f;

	// Token: 0x040006EE RID: 1774
	[Tooltip("Density factor of clouds.")]
	public float density = 1f;

	// Token: 0x040006EF RID: 1775
	[Tooltip("Light Step modifier.")]
	public float lightStepModifier = 0.5f;

	// Token: 0x040006F0 RID: 1776
	[Tooltip("Global coverage multiplicator of clouds.")]
	[Range(0f, 1f)]
	public float coverage = 1f;

	// Token: 0x040006F1 RID: 1777
	[Tooltip("Defines how much light will be absorbed from cloud particles.")]
	[Range(0f, 1f)]
	public float lightAbsorbtion = 0.4f;

	// Token: 0x040006F2 RID: 1778
	[Tooltip("Coverage type of clouds. 1 = more round scattered shapes , 0 = connected islands style")]
	[Range(0f, 1f)]
	public float coverageType = 1f;

	// Token: 0x040006F3 RID: 1779
	[Tooltip("Clouds raynarching step modifier.")]
	[Range(0.25f, 1f)]
	public float raymarchingScale = 1f;

	// Token: 0x040006F4 RID: 1780
	[Tooltip("Clouds modelling type.")]
	[Range(0f, 1f)]
	public float cloudType = 1f;

	// Token: 0x040006F5 RID: 1781
	[Tooltip("Cirrus Clouds Alpha")]
	[Range(0f, 1f)]
	public float cirrusAlpha;

	// Token: 0x040006F6 RID: 1782
	[Tooltip("Cirrus Clouds Coverage")]
	[Range(0f, 1f)]
	public float cirrusCoverage;

	// Token: 0x040006F7 RID: 1783
	[Tooltip("Cirrus Clouds Color Power")]
	[Range(0f, 1f)]
	public float cirrusColorPow = 2f;

	// Token: 0x040006F8 RID: 1784
	[Tooltip("Flat Clouds Alpha")]
	[Range(0f, 1f)]
	public float flatAlpha;

	// Token: 0x040006F9 RID: 1785
	[Tooltip("Flat Clouds Coverage")]
	[Range(0f, 1f)]
	public float flatCoverage;

	// Token: 0x040006FA RID: 1786
	[Tooltip("Flat Clouds Softness")]
	[Range(0f, 1f)]
	public float flatSoftness = 0.75f;

	// Token: 0x040006FB RID: 1787
	[Tooltip("Flat Clouds Brightness")]
	[Range(0f, 1f)]
	public float flatBrightness = 0.75f;

	// Token: 0x040006FC RID: 1788
	[Tooltip("Flat Clouds Color Power")]
	[Range(0f, 1f)]
	public float flatColorPow = 2f;

	// Token: 0x040006FD RID: 1789
	[Tooltip("Particle Clouds Alpha")]
	[Range(0f, 1f)]
	public float particleLayer1Alpha;

	// Token: 0x040006FE RID: 1790
	[Tooltip("Particle Clouds Brightness")]
	[Range(0f, 1f)]
	public float particleLayer1Brightness = 0.75f;

	// Token: 0x040006FF RID: 1791
	[Tooltip("Particle Clouds Color Power")]
	[Range(0f, 1f)]
	public float particleLayer1ColorPow = 2f;

	// Token: 0x04000700 RID: 1792
	[Tooltip("Particle Clouds Alpha")]
	[Range(0f, 1f)]
	public float particleLayer2Alpha;

	// Token: 0x04000701 RID: 1793
	[Tooltip("Particle Clouds Brightness")]
	[Range(0f, 1f)]
	public float particleLayer2Brightness = 0.75f;

	// Token: 0x04000702 RID: 1794
	[Tooltip("Particle Clouds Color Power")]
	[Range(0f, 1f)]
	public float particleLayer2ColorPow = 2f;

	// Token: 0x04000703 RID: 1795
	[Tooltip("Use particle clouds here even when it is disabled!")]
	public bool particleCloudsOverwrite;
}
