using System;
using System.Collections.Generic;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001EC RID: 492
	public sealed class MaterialFactory : IDisposable
	{
		// Token: 0x06000BC9 RID: 3017 RVA: 0x00094EB1 File Offset: 0x000930B1
		public MaterialFactory()
		{
			this.m_Materials = new Dictionary<string, Material>();
		}

		// Token: 0x06000BCA RID: 3018 RVA: 0x00094EC4 File Offset: 0x000930C4
		public Material Get(string shaderName)
		{
			Material material;
			if (!this.m_Materials.TryGetValue(shaderName, out material))
			{
				Shader shader = Shader.Find(shaderName);
				if (shader == null)
				{
					throw new ArgumentException(string.Format("Shader not found ({0})", shaderName));
				}
				material = new Material(shader)
				{
					name = string.Format("PostFX - {0}", shaderName.Substring(shaderName.LastIndexOf("/") + 1)),
					hideFlags = HideFlags.DontSave
				};
				this.m_Materials.Add(shaderName, material);
			}
			return material;
		}

		// Token: 0x06000BCB RID: 3019 RVA: 0x00094F40 File Offset: 0x00093140
		public void Dispose()
		{
			foreach (KeyValuePair<string, Material> keyValuePair in this.m_Materials)
			{
				GraphicsUtils.Destroy(keyValuePair.Value);
			}
			this.m_Materials.Clear();
		}

		// Token: 0x04001DB9 RID: 7609
		private Dictionary<string, Material> m_Materials;
	}
}
