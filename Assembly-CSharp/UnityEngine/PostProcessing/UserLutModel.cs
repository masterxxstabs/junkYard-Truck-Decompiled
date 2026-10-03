using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001E0 RID: 480
	[Serializable]
	public class UserLutModel : PostProcessingModel
	{
		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000B84 RID: 2948 RVA: 0x00093BEF File Offset: 0x00091DEF
		// (set) Token: 0x06000B85 RID: 2949 RVA: 0x00093BF7 File Offset: 0x00091DF7
		public UserLutModel.Settings settings
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

		// Token: 0x06000B86 RID: 2950 RVA: 0x00093C00 File Offset: 0x00091E00
		public override void Reset()
		{
			this.m_Settings = UserLutModel.Settings.defaultSettings;
		}

		// Token: 0x04001D7C RID: 7548
		[SerializeField]
		private UserLutModel.Settings m_Settings = UserLutModel.Settings.defaultSettings;

		// Token: 0x0200048A RID: 1162
		[Serializable]
		public struct Settings
		{
			// Token: 0x1700038C RID: 908
			// (get) Token: 0x06001A72 RID: 6770 RVA: 0x000F6B78 File Offset: 0x000F4D78
			public static UserLutModel.Settings defaultSettings
			{
				get
				{
					return new UserLutModel.Settings
					{
						lut = null,
						contribution = 1f
					};
				}
			}

			// Token: 0x04002B4A RID: 11082
			[Tooltip("Custom lookup texture (strip format, e.g. 256x16).")]
			public Texture2D lut;

			// Token: 0x04002B4B RID: 11083
			[Range(0f, 1f)]
			[Tooltip("Blending factor.")]
			public float contribution;
		}
	}
}
