using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001E4 RID: 484
	public abstract class PostProcessingComponent<T> : PostProcessingComponentBase where T : PostProcessingModel
	{
		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000BA3 RID: 2979 RVA: 0x0009486E File Offset: 0x00092A6E
		// (set) Token: 0x06000BA4 RID: 2980 RVA: 0x00094876 File Offset: 0x00092A76
		public T model { get; internal set; }

		// Token: 0x06000BA5 RID: 2981 RVA: 0x0009487F File Offset: 0x00092A7F
		public virtual void Init(PostProcessingContext pcontext, T pmodel)
		{
			this.context = pcontext;
			this.model = pmodel;
		}

		// Token: 0x06000BA6 RID: 2982 RVA: 0x0009488F File Offset: 0x00092A8F
		public override PostProcessingModel GetModel()
		{
			return this.model;
		}
	}
}
