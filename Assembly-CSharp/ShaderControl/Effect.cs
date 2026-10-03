using System;
using UnityEngine;

namespace ShaderControl
{
	// Token: 0x020001BE RID: 446
	[ExecuteInEditMode]
	public class Effect : MonoBehaviour
	{
		// Token: 0x06000AC8 RID: 2760 RVA: 0x0008F941 File Offset: 0x0008DB41
		private void Start()
		{
			this.mat = Resources.Load<Material>("ChromaScreen");
		}

		// Token: 0x06000AC9 RID: 2761 RVA: 0x0008F953 File Offset: 0x0008DB53
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			this.mat.shaderKeywords = this.keywords;
			Graphics.Blit(source, destination, this.mat);
		}

		// Token: 0x04001D37 RID: 7479
		private Material mat;

		// Token: 0x04001D38 RID: 7480
		private string[] keywords = new string[]
		{
			"ENABLE_RED_CHANNEL",
			"ENABLE_GREEN_CHANNEL",
			"ENABLE_BLUE_CHANNEL"
		};
	}
}
