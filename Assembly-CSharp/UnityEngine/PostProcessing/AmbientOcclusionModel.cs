using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001D3 RID: 467
	[Serializable]
	public class AmbientOcclusionModel : PostProcessingModel
	{
		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000B49 RID: 2889 RVA: 0x000938F8 File Offset: 0x00091AF8
		// (set) Token: 0x06000B4A RID: 2890 RVA: 0x00093900 File Offset: 0x00091B00
		public AmbientOcclusionModel.Settings settings
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

		// Token: 0x06000B4B RID: 2891 RVA: 0x00093909 File Offset: 0x00091B09
		public override void Reset()
		{
			this.m_Settings = AmbientOcclusionModel.Settings.defaultSettings;
		}

		// Token: 0x04001D6D RID: 7533
		[SerializeField]
		private AmbientOcclusionModel.Settings m_Settings = AmbientOcclusionModel.Settings.defaultSettings;

		// Token: 0x02000461 RID: 1121
		public enum SampleCount
		{
			// Token: 0x04002A9C RID: 10908
			Lowest = 3,
			// Token: 0x04002A9D RID: 10909
			Low = 6,
			// Token: 0x04002A9E RID: 10910
			Medium = 10,
			// Token: 0x04002A9F RID: 10911
			High = 16
		}

		// Token: 0x02000462 RID: 1122
		[Serializable]
		public struct Settings
		{
			// Token: 0x17000371 RID: 881
			// (get) Token: 0x06001A54 RID: 6740 RVA: 0x000F5F3C File Offset: 0x000F413C
			public static AmbientOcclusionModel.Settings defaultSettings
			{
				get
				{
					return new AmbientOcclusionModel.Settings
					{
						intensity = 1f,
						radius = 0.3f,
						sampleCount = AmbientOcclusionModel.SampleCount.Medium,
						downsampling = true,
						forceForwardCompatibility = false,
						ambientOnly = false,
						highPrecision = false
					};
				}
			}

			// Token: 0x04002AA0 RID: 10912
			[Range(0f, 4f)]
			[Tooltip("Degree of darkness produced by the effect.")]
			public float intensity;

			// Token: 0x04002AA1 RID: 10913
			[Min(0.0001f)]
			[Tooltip("Radius of sample points, which affects extent of darkened areas.")]
			public float radius;

			// Token: 0x04002AA2 RID: 10914
			[Tooltip("Number of sample points, which affects quality and performance.")]
			public AmbientOcclusionModel.SampleCount sampleCount;

			// Token: 0x04002AA3 RID: 10915
			[Tooltip("Halves the resolution of the effect to increase performance at the cost of visual quality.")]
			public bool downsampling;

			// Token: 0x04002AA4 RID: 10916
			[Tooltip("Forces compatibility with Forward rendered objects when working with the Deferred rendering path.")]
			public bool forceForwardCompatibility;

			// Token: 0x04002AA5 RID: 10917
			[Tooltip("Enables the ambient-only mode in that the effect only affects ambient lighting. This mode is only available with the Deferred rendering path and HDR rendering.")]
			public bool ambientOnly;

			// Token: 0x04002AA6 RID: 10918
			[Tooltip("Toggles the use of a higher precision depth texture with the forward rendering path (may impact performances). Has no effect with the deferred rendering path.")]
			public bool highPrecision;
		}
	}
}
