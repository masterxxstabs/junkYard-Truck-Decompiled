using System;
using UnityEngine;

// Token: 0x0200017E RID: 382
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/FXAA")]
public class FXAA : FXAAPostEffectsBase
{
	// Token: 0x06000951 RID: 2385 RVA: 0x0007DEF5 File Offset: 0x0007C0F5
	private void CreateMaterials()
	{
		if (this.mat == null)
		{
			this.mat = base.CheckShaderAndCreateMaterial(this.shader, this.mat);
		}
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x0007DF1D File Offset: 0x0007C11D
	private void Start()
	{
		this.shader = Shader.Find("Hidden/FXAA3");
		this.CreateMaterials();
		base.CheckSupport(false);
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x0007DF40 File Offset: 0x0007C140
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.CreateMaterials();
		float num = 1f / (float)Screen.width;
		float num2 = 1f / (float)Screen.height;
		this.mat.SetVector("_rcpFrame", new Vector4(num, num2, 0f, 0f));
		this.mat.SetVector("_rcpFrameOpt", new Vector4(num * 2f, num2 * 2f, num * 0.5f, num2 * 0.5f));
		Graphics.Blit(source, destination, this.mat);
	}

	// Token: 0x040018EB RID: 6379
	public Shader shader;

	// Token: 0x040018EC RID: 6380
	private Material mat;
}
