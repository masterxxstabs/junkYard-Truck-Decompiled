using System;
using UnityEngine;

// Token: 0x02000083 RID: 131
[Serializable]
public class EnviroLightShaftsSettings
{
	// Token: 0x0400062E RID: 1582
	[Header("Quality Settings")]
	[Tooltip("Lightshafts resolution quality setting.")]
	public EnviroPostProcessing.SunShaftsResolution resolution = EnviroPostProcessing.SunShaftsResolution.Normal;

	// Token: 0x0400062F RID: 1583
	[Tooltip("Lightshafts blur mode.")]
	public EnviroPostProcessing.ShaftsScreenBlendMode screenBlendMode;

	// Token: 0x04000630 RID: 1584
	[Tooltip("Use cameras depth to hide lightshafts?")]
	public bool useDepthTexture = true;

	// Token: 0x04000631 RID: 1585
	[Header("Intensity Settings")]
	[Tooltip("Color gradient for lightshafts based on sun position.")]
	public Gradient lightShaftsColorSun;

	// Token: 0x04000632 RID: 1586
	[Tooltip("Color gradient for lightshafts based on moon position.")]
	public Gradient lightShaftsColorMoon;

	// Token: 0x04000633 RID: 1587
	[Tooltip("Treshhold gradient for lightshafts based on sun position. This will influence lightshafts intensity!")]
	public Gradient thresholdColorSun;

	// Token: 0x04000634 RID: 1588
	[Tooltip("Treshhold gradient for lightshafts based on moon position. This will influence lightshafts intensity!")]
	public Gradient thresholdColorMoon;

	// Token: 0x04000635 RID: 1589
	[Tooltip("Radius of blurring applied.")]
	public float blurRadius = 6f;

	// Token: 0x04000636 RID: 1590
	[Tooltip("Global Lightshafts intensity.")]
	public float intensity = 0.6f;

	// Token: 0x04000637 RID: 1591
	[Tooltip("Lightshafts maximum radius.")]
	public float maxRadius = 10f;
}
