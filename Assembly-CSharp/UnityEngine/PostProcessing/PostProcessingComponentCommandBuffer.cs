using System;
using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001E5 RID: 485
	public abstract class PostProcessingComponentCommandBuffer<T> : PostProcessingComponent<T> where T : PostProcessingModel
	{
		// Token: 0x06000BA8 RID: 2984
		public abstract CameraEvent GetCameraEvent();

		// Token: 0x06000BA9 RID: 2985
		public abstract string GetName();

		// Token: 0x06000BAA RID: 2986
		public abstract void PopulateCommandBuffer(CommandBuffer cb);
	}
}
