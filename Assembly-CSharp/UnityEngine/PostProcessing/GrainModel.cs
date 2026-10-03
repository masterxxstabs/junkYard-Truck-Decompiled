using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001DD RID: 477
	[Serializable]
	public class GrainModel : PostProcessingModel
	{
		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000B78 RID: 2936 RVA: 0x00093B5C File Offset: 0x00091D5C
		// (set) Token: 0x06000B79 RID: 2937 RVA: 0x00093B64 File Offset: 0x00091D64
		public GrainModel.Settings settings
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

		// Token: 0x06000B7A RID: 2938 RVA: 0x00093B6D File Offset: 0x00091D6D
		public override void Reset()
		{
			this.m_Settings = GrainModel.Settings.defaultSettings;
		}

		// Token: 0x04001D79 RID: 7545
		[SerializeField]
		private GrainModel.Settings m_Settings = GrainModel.Settings.defaultSettings;

		// Token: 0x02000482 RID: 1154
		[Serializable]
		public struct Settings
		{
			// Token: 0x17000389 RID: 905
			// (get) Token: 0x06001A6F RID: 6767 RVA: 0x000F6A28 File Offset: 0x000F4C28
			public static GrainModel.Settings defaultSettings
			{
				get
				{
					return new GrainModel.Settings
					{
						colored = true,
						intensity = 0.5f,
						size = 1f,
						luminanceContribution = 0.8f
					};
				}
			}

			// Token: 0x04002B2D RID: 11053
			[Tooltip("Enable the use of colored grain.")]
			public bool colored;

			// Token: 0x04002B2E RID: 11054
			[Range(0f, 1f)]
			[Tooltip("Grain strength. Higher means more visible grain.")]
			public float intensity;

			// Token: 0x04002B2F RID: 11055
			[Range(0.3f, 3f)]
			[Tooltip("Grain particle size.")]
			public float size;

			// Token: 0x04002B30 RID: 11056
			[Range(0f, 1f)]
			[Tooltip("Controls the noisiness response curve based on scene luminance. Lower values mean less noise in dark areas.")]
			public float luminanceContribution;
		}
	}
}
