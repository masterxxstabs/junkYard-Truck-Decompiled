using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x0200012A RID: 298
public class SaveSlots : MonoBehaviour
{
	// Token: 0x060007BE RID: 1982 RVA: 0x00063A48 File Offset: 0x00061C48
	private void OnEnable()
	{
		if (!this.pcCanvas.activeSelf)
		{
			this.coin.Play();
		}
		this.GetSaves();
	}

	// Token: 0x060007BF RID: 1983 RVA: 0x00063A68 File Offset: 0x00061C68
	public void GetSaves()
	{
		if (ES3.FileExists("JY.es3"))
		{
			this.date1 = ES3.GetTimestamp("JY.es3").ToString();
			this.saveTxt1.text = "Save 1: (" + this.date1 + ")";
		}
		if (ES3.FileExists("JY2.es3"))
		{
			this.date2 = ES3.GetTimestamp("JY2.es3").ToString();
			this.saveTxt2.text = "Save 2: (" + this.date2 + ")";
		}
		if (ES3.FileExists("JY3.es3"))
		{
			this.date3 = ES3.GetTimestamp("JY3.es3").ToString();
			this.saveTxt3.text = "Save 3: (" + this.date3 + ")";
		}
	}

	// Token: 0x060007C0 RID: 1984 RVA: 0x00063B41 File Offset: 0x00061D41
	public void Save1()
	{
		this.mm.OptionSave();
		this.Cancel();
	}

	// Token: 0x060007C1 RID: 1985 RVA: 0x00063B54 File Offset: 0x00061D54
	public void Save2()
	{
		this.mm.OptionSave2();
		this.Cancel();
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x00063B67 File Offset: 0x00061D67
	public void Save3()
	{
		this.mm.OptionSave3();
		this.Cancel();
	}

	// Token: 0x060007C3 RID: 1987 RVA: 0x00063B7A File Offset: 0x00061D7A
	public void Cancel()
	{
		this.thisCanvas.SetActive(false);
		if (!this.pcCanvas.active)
		{
			this.fpc.LockMouse();
			this.fpc.enabled = true;
		}
	}

	// Token: 0x040011BF RID: 4543
	public MainMenu mm;

	// Token: 0x040011C0 RID: 4544
	public Text saveTxt1;

	// Token: 0x040011C1 RID: 4545
	public Text saveTxt2;

	// Token: 0x040011C2 RID: 4546
	public Text saveTxt3;

	// Token: 0x040011C3 RID: 4547
	public string date1;

	// Token: 0x040011C4 RID: 4548
	public string date2;

	// Token: 0x040011C5 RID: 4549
	public string date3;

	// Token: 0x040011C6 RID: 4550
	public GameObject thisCanvas;

	// Token: 0x040011C7 RID: 4551
	public FirstPersonController fpc;

	// Token: 0x040011C8 RID: 4552
	public GameObject pcCanvas;

	// Token: 0x040011C9 RID: 4553
	public AudioSource coin;
}
