using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001E9 RID: 489
	public class PostProcessingProfile : ScriptableObject
	{
		// Token: 0x04001DA3 RID: 7587
		public BuiltinDebugViewsModel debugViews = new BuiltinDebugViewsModel();

		// Token: 0x04001DA4 RID: 7588
		public FogModel fog = new FogModel();

		// Token: 0x04001DA5 RID: 7589
		public AntialiasingModel antialiasing = new AntialiasingModel();

		// Token: 0x04001DA6 RID: 7590
		public AmbientOcclusionModel ambientOcclusion = new AmbientOcclusionModel();

		// Token: 0x04001DA7 RID: 7591
		public ScreenSpaceReflectionModel screenSpaceReflection = new ScreenSpaceReflectionModel();

		// Token: 0x04001DA8 RID: 7592
		public DepthOfFieldModel depthOfField = new DepthOfFieldModel();

		// Token: 0x04001DA9 RID: 7593
		public MotionBlurModel motionBlur = new MotionBlurModel();

		// Token: 0x04001DAA RID: 7594
		public EyeAdaptationModel eyeAdaptation = new EyeAdaptationModel();

		// Token: 0x04001DAB RID: 7595
		public BloomModel bloom = new BloomModel();

		// Token: 0x04001DAC RID: 7596
		public ColorGradingModel colorGrading = new ColorGradingModel();

		// Token: 0x04001DAD RID: 7597
		public UserLutModel userLut = new UserLutModel();

		// Token: 0x04001DAE RID: 7598
		public ChromaticAberrationModel chromaticAberration = new ChromaticAberrationModel();

		// Token: 0x04001DAF RID: 7599
		public GrainModel grain = new GrainModel();

		// Token: 0x04001DB0 RID: 7600
		public VignetteModel vignette = new VignetteModel();

		// Token: 0x04001DB1 RID: 7601
		public DitheringModel dithering = new DitheringModel();
	}
}
