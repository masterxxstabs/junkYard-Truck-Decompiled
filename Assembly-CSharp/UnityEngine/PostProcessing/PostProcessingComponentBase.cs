using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001E3 RID: 483
	public abstract class PostProcessingComponentBase
	{
		// Token: 0x06000B9D RID: 2973 RVA: 0x000116EA File Offset: 0x0000F8EA
		public virtual DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.None;
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000B9E RID: 2974
		public abstract bool active { get; }

		// Token: 0x06000B9F RID: 2975 RVA: 0x00002188 File Offset: 0x00000388
		public virtual void OnEnable()
		{
		}

		// Token: 0x06000BA0 RID: 2976 RVA: 0x00002188 File Offset: 0x00000388
		public virtual void OnDisable()
		{
		}

		// Token: 0x06000BA1 RID: 2977
		public abstract PostProcessingModel GetModel();

		// Token: 0x04001D9B RID: 7579
		public PostProcessingContext context;
	}
}
