using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200007F RID: 127
[Serializable]
public class EnviroLightSettings
{
	// Token: 0x040005FB RID: 1531
	[Header("Direct")]
	[Tooltip("Whether you want to use two direcitonal lights for sun and moon or only one that will switch. Dual mode can be expensive in complex scenes!")]
	public EnviroLightSettings.LightingMode directionalLightMode;

	// Token: 0x040005FC RID: 1532
	[Tooltip("Color gradient for sun and moon light based on sun position in sky.")]
	public Gradient LightColor;

	// Token: 0x040005FD RID: 1533
	[Tooltip("Direct light sun intensity based on sun position in sky")]
	public AnimationCurve directLightSunIntensity = new AnimationCurve();

	// Token: 0x040005FE RID: 1534
	[Tooltip("Direct light moon intensity based on moon position in sky")]
	public AnimationCurve directLightMoonIntensity = new AnimationCurve();

	// Token: 0x040005FF RID: 1535
	[Tooltip("Set the speed of how fast light intensity will update.")]
	[Range(0.01f, 10f)]
	public float lightIntensityTransitionSpeed = 1f;

	// Token: 0x04000600 RID: 1536
	[Tooltip("Set this mod to multiplicate the default light intensity for hdrp.")]
	public float lightIntensityToLumen = 7500f;

	// Token: 0x04000601 RID: 1537
	[Tooltip("Realtime shadow strength of the directional light.")]
	public AnimationCurve shadowIntensity = new AnimationCurve();

	// Token: 0x04000602 RID: 1538
	[Tooltip("Direct lighting y-offset.")]
	[Range(0f, 5000f)]
	public float directLightAngleOffset;

	// Token: 0x04000603 RID: 1539
	[Header("Ambient")]
	[Tooltip("Ambient Rendering Mode.")]
	public AmbientMode ambientMode = AmbientMode.Flat;

	// Token: 0x04000604 RID: 1540
	[Tooltip("Ambientlight intensity based on sun position in sky.")]
	public AnimationCurve ambientIntensity = new AnimationCurve();

	// Token: 0x04000605 RID: 1541
	[Tooltip("Ambientlight sky color based on sun position in sky.")]
	public Gradient ambientSkyColor;

	// Token: 0x04000606 RID: 1542
	[Tooltip("Ambientlight Equator color based on sun position in sky.")]
	public Gradient ambientEquatorColor;

	// Token: 0x04000607 RID: 1543
	[Tooltip("Ambientlight Ground color based on sun position in sky.")]
	public Gradient ambientGroundColor;

	// Token: 0x04000608 RID: 1544
	[Tooltip("Activate to stop the rotation of sun and moon at 'rotationStopHigh' sun/moon altitude in sky.")]
	public bool stopRotationAtHigh;

	// Token: 0x04000609 RID: 1545
	[Range(0f, 1f)]
	[Tooltip("The altitude of sun/moon in sky (Same as 'DayNightSwitch' or the evaluatation of gradients.")]
	public float rotationStopHigh = 0.5f;

	// Token: 0x0200037E RID: 894
	public enum LightingMode
	{
		// Token: 0x04002708 RID: 9992
		Single,
		// Token: 0x04002709 RID: 9993
		Dual
	}
}
