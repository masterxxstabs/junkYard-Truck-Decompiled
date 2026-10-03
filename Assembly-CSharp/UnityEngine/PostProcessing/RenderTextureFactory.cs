using System;
using System.Collections.Generic;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001ED RID: 493
	public sealed class RenderTextureFactory : IDisposable
	{
		// Token: 0x06000BCC RID: 3020 RVA: 0x00094F83 File Offset: 0x00093183
		public RenderTextureFactory()
		{
			this.m_TemporaryRTs = new HashSet<RenderTexture>();
		}

		// Token: 0x06000BCD RID: 3021 RVA: 0x00094F98 File Offset: 0x00093198
		public RenderTexture Get(RenderTexture baseRenderTexture)
		{
			return this.Get(baseRenderTexture.width, baseRenderTexture.height, baseRenderTexture.depth, baseRenderTexture.format, baseRenderTexture.sRGB ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Linear, baseRenderTexture.filterMode, baseRenderTexture.wrapMode, "FactoryTempTexture");
		}

		// Token: 0x06000BCE RID: 3022 RVA: 0x00094FE0 File Offset: 0x000931E0
		public RenderTexture Get(int width, int height, int depthBuffer = 0, RenderTextureFormat format = RenderTextureFormat.ARGBHalf, RenderTextureReadWrite rw = RenderTextureReadWrite.Default, FilterMode filterMode = FilterMode.Bilinear, TextureWrapMode wrapMode = TextureWrapMode.Clamp, string name = "FactoryTempTexture")
		{
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, depthBuffer, format, rw);
			temporary.filterMode = filterMode;
			temporary.wrapMode = wrapMode;
			temporary.name = name;
			this.m_TemporaryRTs.Add(temporary);
			return temporary;
		}

		// Token: 0x06000BCF RID: 3023 RVA: 0x00095020 File Offset: 0x00093220
		public void Release(RenderTexture rt)
		{
			if (rt == null)
			{
				return;
			}
			if (!this.m_TemporaryRTs.Contains(rt))
			{
				throw new ArgumentException(string.Format("Attempting to remove a RenderTexture that was not allocated: {0}", rt));
			}
			this.m_TemporaryRTs.Remove(rt);
			RenderTexture.ReleaseTemporary(rt);
		}

		// Token: 0x06000BD0 RID: 3024 RVA: 0x00095060 File Offset: 0x00093260
		public void ReleaseAll()
		{
			foreach (RenderTexture temp in this.m_TemporaryRTs)
			{
				RenderTexture.ReleaseTemporary(temp);
			}
			this.m_TemporaryRTs.Clear();
		}

		// Token: 0x06000BD1 RID: 3025 RVA: 0x0009509B File Offset: 0x0009329B
		public void Dispose()
		{
			this.ReleaseAll();
		}

		// Token: 0x04001DBA RID: 7610
		private HashSet<RenderTexture> m_TemporaryRTs;
	}
}
