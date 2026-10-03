using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001E8 RID: 488
	[Serializable]
	public abstract class PostProcessingModel
	{
		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000BB8 RID: 3000 RVA: 0x00094938 File Offset: 0x00092B38
		// (set) Token: 0x06000BB9 RID: 3001 RVA: 0x00094940 File Offset: 0x00092B40
		public bool enabled
		{
			get
			{
				return this.m_Enabled;
			}
			set
			{
				this.m_Enabled = value;
				if (value)
				{
					this.OnValidate();
				}
			}
		}

		// Token: 0x06000BBA RID: 3002
		public abstract void Reset();

		// Token: 0x06000BBB RID: 3003 RVA: 0x00002188 File Offset: 0x00000388
		public virtual void OnValidate()
		{
		}

		// Token: 0x04001DA2 RID: 7586
		[SerializeField]
		[GetSet("enabled")]
		private bool m_Enabled;
	}
}
