using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001D7 RID: 471
	[Serializable]
	public class ChromaticAberrationModel : PostProcessingModel
	{
		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000B5B RID: 2907 RVA: 0x000939FF File Offset: 0x00091BFF
		// (set) Token: 0x06000B5C RID: 2908 RVA: 0x00093A07 File Offset: 0x00091C07
		public ChromaticAberrationModel.Settings settings
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

		// Token: 0x06000B5D RID: 2909 RVA: 0x00093A10 File Offset: 0x00091C10
		public override void Reset()
		{
			this.m_Settings = ChromaticAberrationModel.Settings.defaultSettings;
		}

		// Token: 0x04001D71 RID: 7537
		[SerializeField]
		private ChromaticAberrationModel.Settings m_Settings = ChromaticAberrationModel.Settings.defaultSettings;

		// Token: 0x02000471 RID: 1137
		[Serializable]
		public struct Settings
		{
			// Token: 0x1700037C RID: 892
			// (get) Token: 0x06001A62 RID: 6754 RVA: 0x000F6428 File Offset: 0x000F4628
			public static ChromaticAberrationModel.Settings defaultSettings
			{
				get
				{
					return new ChromaticAberrationModel.Settings
					{
						spectralTexture = null,
						intensity = 0.1f
					};
				}
			}

			// Token: 0x04002ADF RID: 10975
			[Tooltip("Shift the hue of chromatic aberrations.")]
			public Texture2D spectralTexture;

			// Token: 0x04002AE0 RID: 10976
			[Range(0f, 1f)]
			[Tooltip("Amount of tangential distortion.")]
			public float intensity;
		}
	}
}
