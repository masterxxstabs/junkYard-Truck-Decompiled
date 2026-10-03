using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001D5 RID: 469
	[Serializable]
	public class BloomModel : PostProcessingModel
	{
		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000B51 RID: 2897 RVA: 0x0009395A File Offset: 0x00091B5A
		// (set) Token: 0x06000B52 RID: 2898 RVA: 0x00093962 File Offset: 0x00091B62
		public BloomModel.Settings settings
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

		// Token: 0x06000B53 RID: 2899 RVA: 0x0009396B File Offset: 0x00091B6B
		public override void Reset()
		{
			this.m_Settings = BloomModel.Settings.defaultSettings;
		}

		// Token: 0x04001D6F RID: 7535
		[SerializeField]
		private BloomModel.Settings m_Settings = BloomModel.Settings.defaultSettings;

		// Token: 0x0200046A RID: 1130
		[Serializable]
		public struct BloomSettings
		{
			// Token: 0x17000375 RID: 885
			// (get) Token: 0x06001A5B RID: 6747 RVA: 0x000F62B4 File Offset: 0x000F44B4
			// (set) Token: 0x06001A5A RID: 6746 RVA: 0x000F62A6 File Offset: 0x000F44A6
			public float thresholdLinear
			{
				get
				{
					return Mathf.GammaToLinearSpace(this.threshold);
				}
				set
				{
					this.threshold = Mathf.LinearToGammaSpace(value);
				}
			}

			// Token: 0x17000376 RID: 886
			// (get) Token: 0x06001A5C RID: 6748 RVA: 0x000F62C4 File Offset: 0x000F44C4
			public static BloomModel.BloomSettings defaultSettings
			{
				get
				{
					return new BloomModel.BloomSettings
					{
						intensity = 0.5f,
						threshold = 1.1f,
						softKnee = 0.5f,
						radius = 4f,
						antiFlicker = false
					};
				}
			}

			// Token: 0x04002AC1 RID: 10945
			[Min(0f)]
			[Tooltip("Strength of the bloom filter.")]
			public float intensity;

			// Token: 0x04002AC2 RID: 10946
			[Min(0f)]
			[Tooltip("Filters out pixels under this level of brightness.")]
			public float threshold;

			// Token: 0x04002AC3 RID: 10947
			[Range(0f, 1f)]
			[Tooltip("Makes transition between under/over-threshold gradual (0 = hard threshold, 1 = soft threshold).")]
			public float softKnee;

			// Token: 0x04002AC4 RID: 10948
			[Range(1f, 7f)]
			[Tooltip("Changes extent of veiling effects in a screen resolution-independent fashion.")]
			public float radius;

			// Token: 0x04002AC5 RID: 10949
			[Tooltip("Reduces flashing noise with an additional filter.")]
			public bool antiFlicker;
		}

		// Token: 0x0200046B RID: 1131
		[Serializable]
		public struct LensDirtSettings
		{
			// Token: 0x17000377 RID: 887
			// (get) Token: 0x06001A5D RID: 6749 RVA: 0x000F6314 File Offset: 0x000F4514
			public static BloomModel.LensDirtSettings defaultSettings
			{
				get
				{
					return new BloomModel.LensDirtSettings
					{
						texture = null,
						intensity = 3f
					};
				}
			}

			// Token: 0x04002AC6 RID: 10950
			[Tooltip("Dirtiness texture to add smudges or dust to the lens.")]
			public Texture texture;

			// Token: 0x04002AC7 RID: 10951
			[Min(0f)]
			[Tooltip("Amount of lens dirtiness.")]
			public float intensity;
		}

		// Token: 0x0200046C RID: 1132
		[Serializable]
		public struct Settings
		{
			// Token: 0x17000378 RID: 888
			// (get) Token: 0x06001A5E RID: 6750 RVA: 0x000F6340 File Offset: 0x000F4540
			public static BloomModel.Settings defaultSettings
			{
				get
				{
					return new BloomModel.Settings
					{
						bloom = BloomModel.BloomSettings.defaultSettings,
						lensDirt = BloomModel.LensDirtSettings.defaultSettings
					};
				}
			}

			// Token: 0x04002AC8 RID: 10952
			public BloomModel.BloomSettings bloom;

			// Token: 0x04002AC9 RID: 10953
			public BloomModel.LensDirtSettings lensDirt;
		}
	}
}
