using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001DC RID: 476
	[Serializable]
	public class FogModel : PostProcessingModel
	{
		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000B74 RID: 2932 RVA: 0x00093B2B File Offset: 0x00091D2B
		// (set) Token: 0x06000B75 RID: 2933 RVA: 0x00093B33 File Offset: 0x00091D33
		public FogModel.Settings settings
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

		// Token: 0x06000B76 RID: 2934 RVA: 0x00093B3C File Offset: 0x00091D3C
		public override void Reset()
		{
			this.m_Settings = FogModel.Settings.defaultSettings;
		}

		// Token: 0x04001D78 RID: 7544
		[SerializeField]
		private FogModel.Settings m_Settings = FogModel.Settings.defaultSettings;

		// Token: 0x02000481 RID: 1153
		[Serializable]
		public struct Settings
		{
			// Token: 0x17000388 RID: 904
			// (get) Token: 0x06001A6E RID: 6766 RVA: 0x000F6A08 File Offset: 0x000F4C08
			public static FogModel.Settings defaultSettings
			{
				get
				{
					return new FogModel.Settings
					{
						excludeSkybox = true
					};
				}
			}

			// Token: 0x04002B2C RID: 11052
			[Tooltip("Should the fog affect the skybox?")]
			public bool excludeSkybox;
		}
	}
}
