using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001DF RID: 479
	[Serializable]
	public class ScreenSpaceReflectionModel : PostProcessingModel
	{
		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000B80 RID: 2944 RVA: 0x00093BBE File Offset: 0x00091DBE
		// (set) Token: 0x06000B81 RID: 2945 RVA: 0x00093BC6 File Offset: 0x00091DC6
		public ScreenSpaceReflectionModel.Settings settings
		{
			get
			{
				return this.m_Settings;
			}
			set
			{
				this.m_Settings = value;
			}
		}

		// Token: 0x06000B82 RID: 2946 RVA: 0x00093BCF File Offset: 0x00091DCF
		public override void Reset()
		{
			this.m_Settings = ScreenSpaceReflectionModel.Settings.defaultSettings;
		}

		// Token: 0x04001D7B RID: 7547
		[SerializeField]
		private ScreenSpaceReflectionModel.Settings m_Settings = ScreenSpaceReflectionModel.Settings.defaultSettings;

		// Token: 0x02000484 RID: 1156
		public enum SSRResolution
		{
			// Token: 0x04002B35 RID: 11061
			High,
			// Token: 0x04002B36 RID: 11062
			Low = 2
		}

		// Token: 0x02000485 RID: 1157
		public enum SSRReflectionBlendType
		{
			// Token: 0x04002B38 RID: 11064
			PhysicallyBased,
			// Token: 0x04002B39 RID: 11065
			Additive
		}

		// Token: 0x02000486 RID: 1158
		[Serializable]
		public struct IntensitySettings
		{
			// Token: 0x04002B3A RID: 11066
			[Tooltip("Nonphysical multiplier for the SSR reflections. 1.0 is physically based.")]
			[Range(0f, 2f)]
			public float reflectionMultiplier;

			// Token: 0x04002B3B RID: 11067
			[Tooltip("How far away from the maxDistance to begin fading SSR.")]
			[Range(0f, 1000f)]
			public float fadeDistance;

			// Token: 0x04002B3C RID: 11068
			[Tooltip("Amplify Fresnel fade out. Increase if floor reflections look good close to the surface and bad farther 'under' the floor.")]
			[Range(0f, 1f)]
			public float fresnelFade;

			// Token: 0x04002B3D RID: 11069
			[Tooltip("Higher values correspond to a faster Fresnel fade as the reflection changes from the grazing angle.")]
			[Range(0.1f, 10f)]
			public float fresnelFadePower;
		}

		// Token: 0x02000487 RID: 1159
		[Serializable]
		public struct ReflectionSettings
		{
			// Token: 0x04002B3E RID: 11070
			[Tooltip("How the reflections are blended into the render.")]
			public ScreenSpaceReflectionModel.SSRReflectionBlendType blendType;

			// Token: 0x04002B3F RID: 11071
			[Tooltip("Half resolution SSRR is much faster, but less accurate.")]
			public ScreenSpaceReflectionModel.SSRResolution reflectionQuality;

			// Token: 0x04002B40 RID: 11072
			[Tooltip("Maximum reflection distance in world units.")]
			[Range(0.1f, 300f)]
			public float maxDistance;

			// Token: 0x04002B41 RID: 11073
			[Tooltip("Max raytracing length.")]
			[Range(16f, 1024f)]
			public int iterationCount;

			// Token: 0x04002B42 RID: 11074
			[Tooltip("Log base 2 of ray tracing coarse step size. Higher traces farther, lower gives better quality silhouettes.")]
			[Range(1f, 16f)]
			public int stepSize;

			// Token: 0x04002B43 RID: 11075
			[Tooltip("Typical thickness of columns, walls, furniture, and other objects that reflection rays might pass behind.")]
			[Range(0.01f, 10f)]
			public float widthModifier;

			// Token: 0x04002B44 RID: 11076
			[Tooltip("Blurriness of reflections.")]
			[Range(0.1f, 8f)]
			public float reflectionBlur;

			// Token: 0x04002B45 RID: 11077
			[Tooltip("Disable for a performance gain in scenes where most glossy objects are horizontal, like floors, water, and tables. Leave on for scenes with glossy vertical objects.")]
			public bool reflectBackfaces;
		}

		// Token: 0x02000488 RID: 1160
		[Serializable]
		public struct ScreenEdgeMask
		{
			// Token: 0x04002B46 RID: 11078
			[Tooltip("Higher = fade out SSRR near the edge of the screen so that reflections don't pop under camera motion.")]
			[Range(0f, 1f)]
			public float intensity;
		}

		// Token: 0x02000489 RID: 1161
		[Serializable]
		public struct Settings
		{
			// Token: 0x1700038B RID: 907
			// (get) Token: 0x06001A71 RID: 6769 RVA: 0x000F6AA4 File Offset: 0x000F4CA4
			public static ScreenSpaceReflectionModel.Settings defaultSettings
			{
				get
				{
					return new ScreenSpaceReflectionModel.Settings
					{
						reflection = new ScreenSpaceReflectionModel.ReflectionSettings
						{
							blendType = ScreenSpaceReflectionModel.SSRReflectionBlendType.PhysicallyBased,
							reflectionQuality = ScreenSpaceReflectionModel.SSRResolution.Low,
							maxDistance = 100f,
							iterationCount = 256,
							stepSize = 3,
							widthModifier = 0.5f,
							reflectionBlur = 1f,
							reflectBackfaces = false
						},
						intensity = new ScreenSpaceReflectionModel.IntensitySettings
						{
							reflectionMultiplier = 1f,
							fadeDistance = 100f,
							fresnelFade = 1f,
							fresnelFadePower = 1f
						},
						screenEdgeMask = new ScreenSpaceReflectionModel.ScreenEdgeMask
						{
							intensity = 0.03f
						}
					};
				}
			}

			// Token: 0x04002B47 RID: 11079
			public ScreenSpaceReflectionModel.ReflectionSettings reflection;

			// Token: 0x04002B48 RID: 11080
			public ScreenSpaceReflectionModel.IntensitySettings intensity;

			// Token: 0x04002B49 RID: 11081
			public ScreenSpaceReflectionModel.ScreenEdgeMask screenEdgeMask;
		}
	}
}
