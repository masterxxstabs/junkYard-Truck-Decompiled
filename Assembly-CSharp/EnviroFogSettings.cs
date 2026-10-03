using System;
using UnityEngine;

// Token: 0x02000081 RID: 129
[Serializable]
public class EnviroFogSettings
{
	// Token: 0x0400060D RID: 1549
	[Header("Mode")]
	[Tooltip("Unity's fog mode.")]
	public FogMode Fogmode = FogMode.Exponential;

	// Token: 0x0400060E RID: 1550
	[Tooltip("Simple fog = just plain color without scattering.")]
	public bool useSimpleFog;

	// Token: 0x0400060F RID: 1551
	[Tooltip("Use Unity Forward Rendering Fog.")]
	public bool useUnityFog;

	// Token: 0x04000610 RID: 1552
	[Header("Distance Fog")]
	[Tooltip("Use distance fog?")]
	public bool distanceFog = true;

	// Token: 0x04000611 RID: 1553
	[Tooltip("Use radial distance fog?")]
	public bool useRadialDistance = true;

	// Token: 0x04000612 RID: 1554
	[Tooltip("The distance where fog starts.")]
	public float startDistance;

	// Token: 0x04000613 RID: 1555
	[Range(0f, 10f)]
	[Tooltip("The intensity of distance fog.")]
	public float distanceFogIntensity = 4f;

	// Token: 0x04000614 RID: 1556
	[Range(0f, 1f)]
	[Tooltip("The maximum density of fog.")]
	public float maximumFogDensity = 0.9f;

	// Token: 0x04000615 RID: 1557
	[Header("Height Fog")]
	[Tooltip("Use heightbased fog?")]
	public bool heightFog = true;

	// Token: 0x04000616 RID: 1558
	[Tooltip("The height of heightbased fog.")]
	public float height = 90f;

	// Token: 0x04000617 RID: 1559
	[Range(0f, 1f)]
	[Tooltip("The intensity of heightbased fog.")]
	public float heightFogIntensity = 1f;

	// Token: 0x04000618 RID: 1560
	[HideInInspector]
	public float heightDensity = 0.15f;

	// Token: 0x04000619 RID: 1561
	[Header("Height Fog Noise")]
	[Range(0f, 1f)]
	[Tooltip("The noise intensity of height based fog.")]
	public float noiseIntensity = 1f;

	// Token: 0x0400061A RID: 1562
	[Tooltip("The noise intensity offset of height based fog.")]
	[Range(0f, 1f)]
	public float noiseIntensityOffset = 0.3f;

	// Token: 0x0400061B RID: 1563
	[Range(0f, 0.1f)]
	[Tooltip("The noise scaling of height based fog.")]
	public float noiseScale = 0.001f;

	// Token: 0x0400061C RID: 1564
	[Tooltip("The speed and direction of height based fog.")]
	public Vector2 noiseVelocity = new Vector2(3f, 1.5f);

	// Token: 0x0400061D RID: 1565
	[Tooltip("Influence scattering near sun.")]
	public float mie = 5f;

	// Token: 0x0400061E RID: 1566
	[Tooltip("Influence scattering near sun.")]
	public float g = 5f;

	// Token: 0x0400061F RID: 1567
	[Header("Fog Dithering")]
	public bool dithering = true;

	// Token: 0x04000620 RID: 1568
	[Tooltip("Color gradient for Top Fog")]
	public Gradient simpleFogColor;

	// Token: 0x04000621 RID: 1569
	[HideInInspector]
	public float skyFogIntensity = 1f;
}
