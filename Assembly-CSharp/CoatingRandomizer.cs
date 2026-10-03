using System;
using UnityEngine;

// Token: 0x02000129 RID: 297
public class CoatingRandomizer : MonoBehaviour
{
	// Token: 0x060007BC RID: 1980 RVA: 0x0006381C File Offset: 0x00061A1C
	private void Awake()
	{
		this.propertyBlock = new MaterialPropertyBlock();
		this.rendererComp = base.GetComponent<Renderer>();
		this.rendererComp.GetPropertyBlock(this.propertyBlock);
		if (this.shaderType == ShaderType.Coating2Layers)
		{
			this.layer2Intensity = "_MaskIntensityOffset";
			this.layer2Contrast = "_MaskContrastOffset";
			if (this.bRandomizeLayer2Intensity)
			{
				this.propertyBlock.SetFloat(this.layer2Intensity, Random.Range(this.layer2IntensityMin, this.layer2IntensityMax));
			}
			if (this.bRandomizeLayer2Contrast)
			{
				this.propertyBlock.SetFloat(this.layer2Contrast, Random.Range(this.layer2ContrastMin, this.layer2ContrastMax));
			}
		}
		else if (this.shaderType == ShaderType.Coating3Layers)
		{
			this.layer2Intensity = "_L2MaskIntensityOffset";
			this.layer2Contrast = "_L2MaskContrastOffset";
			this.layer3Intensity = "_L3MaskIntensityOffset";
			this.layer3Contrast = "_L3MaskContrastOffset";
			if (this.bRandomizeLayer2Intensity)
			{
				this.propertyBlock.SetFloat(this.layer2Intensity, Random.Range(this.layer2IntensityMin, this.layer2IntensityMax));
			}
			if (this.bRandomizeLayer2Contrast)
			{
				this.propertyBlock.SetFloat(this.layer2Contrast, Random.Range(this.layer2ContrastMin, this.layer2ContrastMax));
			}
			if (this.bRandomizeLayer3Intensity)
			{
				this.propertyBlock.SetFloat(this.layer3Intensity, Random.Range(this.layer3IntensityMin, this.layer3IntensityMax));
			}
			if (this.bRandomizeLayer3Contrast)
			{
				this.propertyBlock.SetFloat(this.layer3Contrast, Random.Range(this.layer3ContrastMin, this.layer3ContrastMax));
			}
		}
		this.rendererComp.SetPropertyBlock(this.propertyBlock);
	}

	// Token: 0x040011AB RID: 4523
	public ShaderType shaderType;

	// Token: 0x040011AC RID: 4524
	public bool bRandomizeLayer2Intensity;

	// Token: 0x040011AD RID: 4525
	public bool bRandomizeLayer2Contrast;

	// Token: 0x040011AE RID: 4526
	public bool bRandomizeLayer3Intensity;

	// Token: 0x040011AF RID: 4527
	public bool bRandomizeLayer3Contrast;

	// Token: 0x040011B0 RID: 4528
	[Range(1f, 10f)]
	public float test = 1f;

	// Token: 0x040011B1 RID: 4529
	public float layer2IntensityMin;

	// Token: 0x040011B2 RID: 4530
	public float layer2IntensityMax = 10f;

	// Token: 0x040011B3 RID: 4531
	public float layer2ContrastMin = 0.1f;

	// Token: 0x040011B4 RID: 4532
	public float layer2ContrastMax = 10f;

	// Token: 0x040011B5 RID: 4533
	public float layer3IntensityMin;

	// Token: 0x040011B6 RID: 4534
	public float layer3IntensityMax = 10f;

	// Token: 0x040011B7 RID: 4535
	public float layer3ContrastMin = 0.1f;

	// Token: 0x040011B8 RID: 4536
	public float layer3ContrastMax = 10f;

	// Token: 0x040011B9 RID: 4537
	private string layer2Intensity = "_L2MaskIntensityOffset";

	// Token: 0x040011BA RID: 4538
	private string layer2Contrast = "_L2MaskContrastOffset";

	// Token: 0x040011BB RID: 4539
	private string layer3Intensity = "_L3MaskIntensityOffset";

	// Token: 0x040011BC RID: 4540
	private string layer3Contrast = "_L3MaskContrastOffset";

	// Token: 0x040011BD RID: 4541
	private Renderer rendererComp;

	// Token: 0x040011BE RID: 4542
	private MaterialPropertyBlock propertyBlock;
}
