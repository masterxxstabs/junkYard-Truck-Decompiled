using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x02000126 RID: 294
public class RgbChange : MonoBehaviour
{
	// Token: 0x060007B4 RID: 1972 RVA: 0x00063470 File Offset: 0x00061670
	private void Update()
	{
		if (this.which == 1)
		{
			this.dbDash.SetColor("_EmissionColor", new Color(this.rSlider.value, this.gSlider.value, this.bSlider.value, 1f));
			this.tachColor.color = new Color(this.rSlider.value, this.gSlider.value, this.bSlider.value, 1f);
			this.mphColor.color = new Color(this.rSlider.value, this.gSlider.value, this.bSlider.value, 1f);
		}
		if (this.which == 2)
		{
			this.dbDashF.SetColor("_EmissionColor", new Color(this.rSlider.value, this.gSlider.value, this.bSlider.value, 1f));
			this.tachColorF.color = new Color(this.rSlider.value, this.gSlider.value, this.bSlider.value, 1f);
			this.mphColorF.color = new Color(this.rSlider.value, this.gSlider.value, this.bSlider.value, 1f);
		}
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x000635E4 File Offset: 0x000617E4
	public void Confirm()
	{
		if (this.which == 1)
		{
			this.truck.rgbR = this.rSlider.value;
			this.truck.rgbG = this.gSlider.value;
			this.truck.rgbB = this.bSlider.value;
		}
		if (this.which == 2)
		{
			this.truckF.rgbR = this.rSlider.value;
			this.truckF.rgbG = this.gSlider.value;
			this.truckF.rgbB = this.bSlider.value;
		}
		base.gameObject.SetActive(false);
		this.fpc.LockMouse();
		this.fpc.enabled = true;
	}

	// Token: 0x04001191 RID: 4497
	public FirstPersonController fpc;

	// Token: 0x04001192 RID: 4498
	public Slider rSlider;

	// Token: 0x04001193 RID: 4499
	public Slider gSlider;

	// Token: 0x04001194 RID: 4500
	public Slider bSlider;

	// Token: 0x04001195 RID: 4501
	public Material dbDash;

	// Token: 0x04001196 RID: 4502
	public Image tachColor;

	// Token: 0x04001197 RID: 4503
	public Image mphColor;

	// Token: 0x04001198 RID: 4504
	public Material dbDashF;

	// Token: 0x04001199 RID: 4505
	public Image tachColorF;

	// Token: 0x0400119A RID: 4506
	public Image mphColorF;

	// Token: 0x0400119B RID: 4507
	public car truck;

	// Token: 0x0400119C RID: 4508
	public car4 truckF;

	// Token: 0x0400119D RID: 4509
	public int which;
}
