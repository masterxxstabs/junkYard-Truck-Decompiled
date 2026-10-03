using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x02000127 RID: 295
public class RgbChangePaint : MonoBehaviour
{
	// Token: 0x060007B7 RID: 1975 RVA: 0x000636AA File Offset: 0x000618AA
	private void OnEnable()
	{
		this.CheckCan();
		this.paintMat = this.paintCan.GetComponent<Renderer>().material;
	}

	// Token: 0x060007B8 RID: 1976 RVA: 0x000636C8 File Offset: 0x000618C8
	private void Update()
	{
		if (this.canPresent)
		{
			this.paintMat.color = new Color(this.rSlider.value, this.gSlider.value, this.bSlider.value, 1f);
			this.paintMat.SetFloat("_Metallic", this.mSlider.value);
			this.paintMat.SetFloat("_Glossiness", this.sSlider.value);
		}
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x0006374C File Offset: 0x0006194C
	private void CheckCan()
	{
		this.canPresent = false;
		Collider[] array = Physics.OverlapBox(this.paintLoc.position, base.transform.localScale / 5f, this.paintLoc.rotation);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].gameObject == this.paintCan)
			{
				this.canPresent = true;
			}
		}
	}

	// Token: 0x060007BA RID: 1978 RVA: 0x000637BC File Offset: 0x000619BC
	public void Confirm()
	{
		base.gameObject.SetActive(false);
		this.fpc.LockMouse();
		this.fpc.enabled = true;
		this.paintCan.GetComponent<PickUp>().pickable = true;
		if (this.canPresent)
		{
			this.paintLoc.gameObject.GetComponent<AudioSource>().Play();
		}
	}

	// Token: 0x0400119E RID: 4510
	public FirstPersonController fpc;

	// Token: 0x0400119F RID: 4511
	public Slider rSlider;

	// Token: 0x040011A0 RID: 4512
	public Slider gSlider;

	// Token: 0x040011A1 RID: 4513
	public Slider bSlider;

	// Token: 0x040011A2 RID: 4514
	public Slider mSlider;

	// Token: 0x040011A3 RID: 4515
	public Slider sSlider;

	// Token: 0x040011A4 RID: 4516
	public GameObject paintCan;

	// Token: 0x040011A5 RID: 4517
	public Material paintMat;

	// Token: 0x040011A6 RID: 4518
	public Transform paintLoc;

	// Token: 0x040011A7 RID: 4519
	private bool canPresent;
}
