using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001E6 RID: 486
	public abstract class PostProcessingComponentRenderTexture<T> : PostProcessingComponent<T> where T : PostProcessingModel
	{
		// Token: 0x06000BAC RID: 2988 RVA: 0x00002188 File Offset: 0x00000388
		public virtual void Prepare(Material material)
		{
		}
	}
}
