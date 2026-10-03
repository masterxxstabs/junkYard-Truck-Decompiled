using System;
using UnityEngine;

// Token: 0x02000087 RID: 135
[Serializable]
public class EnviroAuroraSettings
{
	// Token: 0x04000667 RID: 1639
	public AnimationCurve auroraIntensity = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(0.5f, 0.1f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x04000668 RID: 1640
	[Header("Aurora Color and Brightness")]
	public Color auroraColor = new Color(0.1f, 0.5f, 0.7f);

	// Token: 0x04000669 RID: 1641
	public float auroraBrightness = 75f;

	// Token: 0x0400066A RID: 1642
	public float auroraContrast = 10f;

	// Token: 0x0400066B RID: 1643
	[Header("Aurora Height and Scale")]
	public float auroraHeight = 20000f;

	// Token: 0x0400066C RID: 1644
	[Range(0f, 0.025f)]
	public float auroraScale = 0.01f;

	// Token: 0x0400066D RID: 1645
	[Header("Aurora Performance")]
	[Range(8f, 32f)]
	public int auroraSteps = 20;

	// Token: 0x0400066E RID: 1646
	[Header("Aurora Modelling and Animation")]
	public Vector4 auroraLayer1Settings = new Vector4(0.1f, 0.1f, 0f, 0.5f);

	// Token: 0x0400066F RID: 1647
	public Vector4 auroraLayer2Settings = new Vector4(5f, 5f, 0f, 0.5f);

	// Token: 0x04000670 RID: 1648
	public Vector4 auroraColorshiftSettings = new Vector4(0.05f, 0.05f, 0f, 5f);

	// Token: 0x04000671 RID: 1649
	[Range(0f, 0.1f)]
	public float auroraSpeed = 0.005f;
}
