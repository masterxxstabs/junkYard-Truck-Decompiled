using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001E7 RID: 487
	public class PostProcessingContext
	{
		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000BAE RID: 2990 RVA: 0x000948B4 File Offset: 0x00092AB4
		// (set) Token: 0x06000BAF RID: 2991 RVA: 0x000948BC File Offset: 0x00092ABC
		public bool interrupted { get; private set; }

		// Token: 0x06000BB0 RID: 2992 RVA: 0x000948C5 File Offset: 0x00092AC5
		public void Interrupt()
		{
			this.interrupted = true;
		}

		// Token: 0x06000BB1 RID: 2993 RVA: 0x000948CE File Offset: 0x00092ACE
		public PostProcessingContext Reset()
		{
			this.profile = null;
			this.camera = null;
			this.materialFactory = null;
			this.renderTextureFactory = null;
			this.interrupted = false;
			return this;
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000BB2 RID: 2994 RVA: 0x000948F4 File Offset: 0x00092AF4
		public bool isGBufferAvailable
		{
			get
			{
				return this.camera.actualRenderingPath == RenderingPath.DeferredShading;
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000BB3 RID: 2995 RVA: 0x00094904 File Offset: 0x00092B04
		public bool isHdr
		{
			get
			{
				return this.camera.allowHDR;
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000BB4 RID: 2996 RVA: 0x00094911 File Offset: 0x00092B11
		public int width
		{
			get
			{
				return this.camera.pixelWidth;
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000BB5 RID: 2997 RVA: 0x0009491E File Offset: 0x00092B1E
		public int height
		{
			get
			{
				return this.camera.pixelHeight;
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000BB6 RID: 2998 RVA: 0x0009492B File Offset: 0x00092B2B
		public Rect viewport
		{
			get
			{
				return this.camera.rect;
			}
		}

		// Token: 0x04001D9D RID: 7581
		public PostProcessingProfile profile;

		// Token: 0x04001D9E RID: 7582
		public Camera camera;

		// Token: 0x04001D9F RID: 7583
		public MaterialFactory materialFactory;

		// Token: 0x04001DA0 RID: 7584
		public RenderTextureFactory renderTextureFactory;
	}
}
