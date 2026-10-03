using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000104 RID: 260
[CreateAssetMenu(fileName = "LakePolygonProfile", menuName = "LakePolygonProfile", order = 1)]
public class LakePolygonProfile : ScriptableObject
{
	// Token: 0x04000E14 RID: 3604
	public Material lakeMaterial;

	// Token: 0x04000E15 RID: 3605
	public float distSmooth = 5f;

	// Token: 0x04000E16 RID: 3606
	public float uvScale = 1f;

	// Token: 0x04000E17 RID: 3607
	public float maximumTriangleSize = 50f;

	// Token: 0x04000E18 RID: 3608
	public float traingleDensity = 0.2f;

	// Token: 0x04000E19 RID: 3609
	public bool receiveShadows;

	// Token: 0x04000E1A RID: 3610
	public ShadowCastingMode shadowCastingMode;

	// Token: 0x04000E1B RID: 3611
	public float automaticFlowMapScale = 0.2f;

	// Token: 0x04000E1C RID: 3612
	public bool noiseflowMap;

	// Token: 0x04000E1D RID: 3613
	public float noiseMultiplierflowMap = 1f;

	// Token: 0x04000E1E RID: 3614
	public float noiseSizeXflowMap = 0.2f;

	// Token: 0x04000E1F RID: 3615
	public float noiseSizeZflowMap = 0.2f;

	// Token: 0x04000E20 RID: 3616
	public AnimationCurve terrainCarve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(10f, -2f)
	});

	// Token: 0x04000E21 RID: 3617
	public float terrainSmoothMultiplier = 1f;

	// Token: 0x04000E22 RID: 3618
	public AnimationCurve terrainPaintCarve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04000E23 RID: 3619
	public bool noiseCarve;

	// Token: 0x04000E24 RID: 3620
	public float noiseMultiplierInside = 1f;

	// Token: 0x04000E25 RID: 3621
	public float noiseMultiplierOutside = 0.25f;

	// Token: 0x04000E26 RID: 3622
	public float noiseSizeX = 0.2f;

	// Token: 0x04000E27 RID: 3623
	public float noiseSizeZ = 0.2f;

	// Token: 0x04000E28 RID: 3624
	public int currentSplatMap = 1;

	// Token: 0x04000E29 RID: 3625
	public bool noisePaint;

	// Token: 0x04000E2A RID: 3626
	public float noiseMultiplierInsidePaint = 1f;

	// Token: 0x04000E2B RID: 3627
	public float noiseMultiplierOutsidePaint = 0.5f;

	// Token: 0x04000E2C RID: 3628
	public float noiseSizeXPaint = 0.2f;

	// Token: 0x04000E2D RID: 3629
	public float noiseSizeZPaint = 0.2f;

	// Token: 0x04000E2E RID: 3630
	public bool mixTwoSplatMaps;

	// Token: 0x04000E2F RID: 3631
	public int secondSplatMap = 1;

	// Token: 0x04000E30 RID: 3632
	public bool addCliffSplatMap;

	// Token: 0x04000E31 RID: 3633
	public int cliffSplatMap = 1;

	// Token: 0x04000E32 RID: 3634
	public float cliffAngle = 25f;

	// Token: 0x04000E33 RID: 3635
	public float cliffBlend = 1f;

	// Token: 0x04000E34 RID: 3636
	public int cliffSplatMapOutside = 1;

	// Token: 0x04000E35 RID: 3637
	public float cliffAngleOutside = 25f;

	// Token: 0x04000E36 RID: 3638
	public float cliffBlendOutside = 1f;

	// Token: 0x04000E37 RID: 3639
	public float distanceClearFoliage = 1f;

	// Token: 0x04000E38 RID: 3640
	public float distanceClearFoliageTrees = 1f;

	// Token: 0x04000E39 RID: 3641
	public int biomeType;
}
