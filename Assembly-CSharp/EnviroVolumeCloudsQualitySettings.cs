using System;
using UnityEngine;

// Token: 0x020000A7 RID: 167
[Serializable]
public class EnviroVolumeCloudsQualitySettings
{
	// Token: 0x04000823 RID: 2083
	[Header("Clouds Height Settings")]
	[Tooltip("Clouds start height.")]
	public float bottomCloudHeight = 3000f;

	// Token: 0x04000824 RID: 2084
	[Tooltip("Clouds end height.")]
	public float topCloudHeight = 7000f;

	// Token: 0x04000825 RID: 2085
	[Header("Raymarch Step Settings")]
	[Range(32f, 256f)]
	[Tooltip("Number of raymarching samples.")]
	public int raymarchSteps = 150;

	// Token: 0x04000826 RID: 2086
	[Tooltip("Increase performance by using less steps when clouds are hidden by objects.")]
	[Range(0.1f, 1f)]
	public float stepsInDepthModificator = 0.75f;

	// Token: 0x04000827 RID: 2087
	[Tooltip("Increase performance by using early exit expensive raymarching. Higher values = more performant but less accurate lighting.")]
	[Range(0f, 0.5f)]
	public float transmissionToExit = 0.05f;

	// Token: 0x04000828 RID: 2088
	[Range(1f, 8f)]
	[Header("Resolution, Upsample and Reprojection")]
	[Tooltip("Downsampling of clouds rendering. 1 = full res, 2 = half Res, ...")]
	public int cloudsRenderResolution = 1;

	// Token: 0x04000829 RID: 2089
	public EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize reprojectionPixelSize;

	// Token: 0x0400082A RID: 2090
	[Header("Clouds Modelling")]
	[Tooltip("LOD Distance for using lower res 3d texture for far away clouds. ")]
	[Range(0f, 1f)]
	public float lodDistance = 0.5f;

	// Token: 0x0400082B RID: 2091
	[Tooltip("The UV scale of base noise. High Values = Low performance!")]
	[Range(2f, 100f)]
	public float baseNoiseUV = 20f;

	// Token: 0x0400082C RID: 2092
	[Tooltip("The UV scale of detail noise. High Values = Low performance!")]
	[Range(2f, 100f)]
	public float detailNoiseUV = 50f;

	// Token: 0x0400082D RID: 2093
	[Tooltip("Enable to use a curl noise to further enhance the detail erode.")]
	public bool useCurlNoise;

	// Token: 0x0400082E RID: 2094
	[Tooltip("Resolution of Detail Noise Texture.")]
	public EnviroVolumeCloudsQualitySettings.CloudDetailQuality detailQuality;

	// Token: 0x020003A2 RID: 930
	public enum ReprojectionPixelSize
	{
		// Token: 0x0400277D RID: 10109
		Off,
		// Token: 0x0400277E RID: 10110
		Low,
		// Token: 0x0400277F RID: 10111
		Medium,
		// Token: 0x04002780 RID: 10112
		High
	}

	// Token: 0x020003A3 RID: 931
	public enum CloudDetailQuality
	{
		// Token: 0x04002782 RID: 10114
		Low,
		// Token: 0x04002783 RID: 10115
		High
	}
}
