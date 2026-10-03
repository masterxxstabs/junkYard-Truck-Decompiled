using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001DE RID: 478
	[Serializable]
	public class MotionBlurModel : PostProcessingModel
	{
		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000B7C RID: 2940 RVA: 0x00093B8D File Offset: 0x00091D8D
		// (set) Token: 0x06000B7D RID: 2941 RVA: 0x00093B95 File Offset: 0x00091D95
		public MotionBlurModel.Settings settings
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

		// Token: 0x06000B7E RID: 2942 RVA: 0x00093B9E File Offset: 0x00091D9E
		public override void Reset()
		{
			this.m_Settings = MotionBlurModel.Settings.defaultSettings;
		}

		// Token: 0x04001D7A RID: 7546
		[SerializeField]
		private MotionBlurModel.Settings m_Settings = MotionBlurModel.Settings.defaultSettings;

		// Token: 0x02000483 RID: 1155
		[Serializable]
		public struct Settings
		{
			// Token: 0x1700038A RID: 906
			// (get) Token: 0x06001A70 RID: 6768 RVA: 0x000F6A6C File Offset: 0x000F4C6C
			public static MotionBlurModel.Settings defaultSettings
			{
				get
				{
					return new MotionBlurModel.Settings
					{
						shutterAngle = 270f,
						sampleCount = 10,
						frameBlending = 0f
					};
				}
			}

			// Token: 0x04002B31 RID: 11057
			[Range(0f, 360f)]
			[Tooltip("The angle of rotary shutter. Larger values give longer exposure.")]
			public float shutterAngle;

			// Token: 0x04002B32 RID: 11058
			[Range(4f, 32f)]
			[Tooltip("The amount of sample points, which affects quality and performances.")]
			public int sampleCount;

			// Token: 0x04002B33 RID: 11059
			[Range(0f, 1f)]
			[Tooltip("The strength of multiple frame blending. The opacity of preceding frames are determined from this coefficient and time differences.")]
			public float frameBlending;
		}
	}
}
