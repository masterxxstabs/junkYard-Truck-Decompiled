using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001DA RID: 474
	[Serializable]
	public class DitheringModel : PostProcessingModel
	{
		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000B6C RID: 2924 RVA: 0x00093AC9 File Offset: 0x00091CC9
		// (set) Token: 0x06000B6D RID: 2925 RVA: 0x00093AD1 File Offset: 0x00091CD1
		public DitheringModel.Settings settings
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

		// Token: 0x06000B6E RID: 2926 RVA: 0x00093ADA File Offset: 0x00091CDA
		public override void Reset()
		{
			this.m_Settings = DitheringModel.Settings.defaultSettings;
		}

		// Token: 0x04001D76 RID: 7542
		[SerializeField]
		private DitheringModel.Settings m_Settings = DitheringModel.Settings.defaultSettings;

		// Token: 0x0200047E RID: 1150
		[Serializable]
		public struct Settings
		{
			// Token: 0x17000386 RID: 902
			// (get) Token: 0x06001A6C RID: 6764 RVA: 0x000F6964 File Offset: 0x000F4B64
			public static DitheringModel.Settings defaultSettings
			{
				get
				{
					return default(DitheringModel.Settings);
				}
			}
		}
	}
}
