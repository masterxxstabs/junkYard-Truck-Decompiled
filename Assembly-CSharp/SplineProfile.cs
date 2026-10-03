using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000108 RID: 264
[CreateAssetMenu(fileName = "SplineProfile", menuName = "SplineProfile", order = 1)]
public class SplineProfile : ScriptableObject
{
	// Token: 0x04000ED1 RID: 3793
	public Material splineMaterial;

	// Token: 0x04000ED2 RID: 3794
	public AnimationCurve meshCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x04000ED3 RID: 3795
	public float minVal = 0.5f;

	// Token: 0x04000ED4 RID: 3796
	public float maxVal = 0.5f;

	// Token: 0x04000ED5 RID: 3797
	public int vertsInShape = 3;

	// Token: 0x04000ED6 RID: 3798
	public float traingleDensity = 0.2f;

	// Token: 0x04000ED7 RID: 3799
	public float uvScale = 3f;

	// Token: 0x04000ED8 RID: 3800
	public bool uvRotation = true;

	// Token: 0x04000ED9 RID: 3801
	public bool receiveShadows;

	// Token: 0x04000EDA RID: 3802
	public ShadowCastingMode shadowCastingMode;

	// Token: 0x04000EDB RID: 3803
	public AnimationCurve flowFlat = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0.025f),
		new Keyframe(0.5f, 0.05f),
		new Keyframe(1f, 0.025f)
	});

	// Token: 0x04000EDC RID: 3804
	public AnimationCurve flowWaterfall = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0.25f),
		new Keyframe(1f, 0.25f)
	});

	// Token: 0x04000EDD RID: 3805
	public bool noiseflowMap;

	// Token: 0x04000EDE RID: 3806
	public float noiseMultiplierflowMap = 0.1f;

	// Token: 0x04000EDF RID: 3807
	public float noiseSizeXflowMap = 2f;

	// Token: 0x04000EE0 RID: 3808
	public float noiseSizeZflowMap = 2f;

	// Token: 0x04000EE1 RID: 3809
	public float floatSpeed = 10f;

	// Token: 0x04000EE2 RID: 3810
	public AnimationCurve terrainCarve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(10f, -2f)
	});

	// Token: 0x04000EE3 RID: 3811
	public float distSmooth = 5f;

	// Token: 0x04000EE4 RID: 3812
	public float distSmoothStart = 1f;

	// Token: 0x04000EE5 RID: 3813
	public AnimationCurve terrainPaintCarve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04000EE6 RID: 3814
	public bool noiseCarve;

	// Token: 0x04000EE7 RID: 3815
	public float noiseMultiplierInside = 1f;

	// Token: 0x04000EE8 RID: 3816
	public float noiseMultiplierOutside = 0.25f;

	// Token: 0x04000EE9 RID: 3817
	public float noiseSizeX = 0.2f;

	// Token: 0x04000EEA RID: 3818
	public float noiseSizeZ = 0.2f;

	// Token: 0x04000EEB RID: 3819
	public float terrainSmoothMultiplier = 5f;

	// Token: 0x04000EEC RID: 3820
	public int currentSplatMap = 1;

	// Token: 0x04000EED RID: 3821
	public bool mixTwoSplatMaps;

	// Token: 0x04000EEE RID: 3822
	public int secondSplatMap = 1;

	// Token: 0x04000EEF RID: 3823
	public bool addCliffSplatMap;

	// Token: 0x04000EF0 RID: 3824
	public int cliffSplatMap = 1;

	// Token: 0x04000EF1 RID: 3825
	public float cliffAngle = 45f;

	// Token: 0x04000EF2 RID: 3826
	public float cliffBlend = 1f;

	// Token: 0x04000EF3 RID: 3827
	public int cliffSplatMapOutside = 1;

	// Token: 0x04000EF4 RID: 3828
	public float cliffAngleOutside = 45f;

	// Token: 0x04000EF5 RID: 3829
	public float cliffBlendOutside = 1f;

	// Token: 0x04000EF6 RID: 3830
	public float distanceClearFoliage = 1f;

	// Token: 0x04000EF7 RID: 3831
	public float distanceClearFoliageTrees = 1f;

	// Token: 0x04000EF8 RID: 3832
	public bool noisePaint;

	// Token: 0x04000EF9 RID: 3833
	public float noiseMultiplierInsidePaint = 0.25f;

	// Token: 0x04000EFA RID: 3834
	public float noiseMultiplierOutsidePaint = 0.25f;

	// Token: 0x04000EFB RID: 3835
	public float noiseSizeXPaint = 0.2f;

	// Token: 0x04000EFC RID: 3836
	public float noiseSizeZPaint = 0.2f;

	// Token: 0x04000EFD RID: 3837
	public float simulatedRiverLength = 100f;

	// Token: 0x04000EFE RID: 3838
	public int simulatedRiverPoints = 10;

	// Token: 0x04000EFF RID: 3839
	public float simulatedMinStepSize = 1f;

	// Token: 0x04000F00 RID: 3840
	public bool simulatedNoUp;

	// Token: 0x04000F01 RID: 3841
	public bool simulatedBreakOnUp = true;

	// Token: 0x04000F02 RID: 3842
	public bool noiseWidth;

	// Token: 0x04000F03 RID: 3843
	public float noiseMultiplierWidth = 4f;

	// Token: 0x04000F04 RID: 3844
	public float noiseSizeWidth = 0.5f;

	// Token: 0x04000F05 RID: 3845
	public int biomeType;
}
