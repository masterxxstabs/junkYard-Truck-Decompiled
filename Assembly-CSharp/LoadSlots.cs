using System;
using TMPro;
using UnityEngine;

// Token: 0x020000EB RID: 235
public class LoadSlots : MonoBehaviour
{
	// Token: 0x060005D0 RID: 1488 RVA: 0x0004782C File Offset: 0x00045A2C
	private void Start()
	{
		this.GetSaves();
	}

	// Token: 0x060005D1 RID: 1489 RVA: 0x00047834 File Offset: 0x00045A34
	public void GetSaves()
	{
		this.icon1.SetActive(false);
		this.icon1b.SetActive(false);
		this.icon2.SetActive(false);
		this.icon2b.SetActive(false);
		this.icon3.SetActive(false);
		this.icon3b.SetActive(false);
		this.icon4.SetActive(false);
		this.icon4b.SetActive(false);
		if (ES3.FileExists("JY.es3"))
		{
			this.date1 = ES3.GetTimestamp("JY.es3").ToString();
			this.saveTxt1.text = "Save 1: (" + this.date1 + ")";
			this.saveTxt1b.text = "Save 1: (" + this.date1 + ")";
		}
		if (ES3.FileExists("JY2.es3"))
		{
			this.date2 = ES3.GetTimestamp("JY2.es3").ToString();
			this.saveTxt2.text = "Save 2: (" + this.date2 + ")";
			this.saveTxt2b.text = "Save 2: (" + this.date2 + ")";
		}
		if (ES3.FileExists("JY3.es3"))
		{
			this.date3 = ES3.GetTimestamp("JY3.es3").ToString();
			this.saveTxt3.text = "Save 3: (" + this.date3 + ")";
			this.saveTxt3b.text = "Save 3: (" + this.date3 + ")";
		}
		if (ES3.FileExists("JYAuto.es3"))
		{
			this.date4 = ES3.GetTimestamp("JYAuto.es3").ToString();
			this.saveTxt4.text = "Auto Save: (" + this.date4 + ")";
			this.saveTxt4b.text = "Auto Save: (" + this.date4 + ")";
		}
	}

	// Token: 0x060005D2 RID: 1490 RVA: 0x00047A34 File Offset: 0x00045C34
	public void LoadGame(int slot)
	{
		if (slot == 1)
		{
			PlayerPrefs.SetInt("LoadSlot", 1);
		}
		else if (slot == 2)
		{
			PlayerPrefs.SetInt("LoadSlot", 2);
		}
		else if (slot == 3)
		{
			PlayerPrefs.SetInt("LoadSlot", 3);
		}
		else if (slot == 4)
		{
			PlayerPrefs.SetInt("LoadSlot", 4);
		}
		this.mm.Continue();
	}

	// Token: 0x04000C98 RID: 3224
	public TextMeshProUGUI saveTxt1;

	// Token: 0x04000C99 RID: 3225
	public TextMeshProUGUI saveTxt2;

	// Token: 0x04000C9A RID: 3226
	public TextMeshProUGUI saveTxt3;

	// Token: 0x04000C9B RID: 3227
	public TextMeshProUGUI saveTxt4;

	// Token: 0x04000C9C RID: 3228
	public TextMeshProUGUI saveTxt1b;

	// Token: 0x04000C9D RID: 3229
	public TextMeshProUGUI saveTxt2b;

	// Token: 0x04000C9E RID: 3230
	public TextMeshProUGUI saveTxt3b;

	// Token: 0x04000C9F RID: 3231
	public TextMeshProUGUI saveTxt4b;

	// Token: 0x04000CA0 RID: 3232
	public GameObject icon1;

	// Token: 0x04000CA1 RID: 3233
	public GameObject icon1b;

	// Token: 0x04000CA2 RID: 3234
	public GameObject icon2;

	// Token: 0x04000CA3 RID: 3235
	public GameObject icon2b;

	// Token: 0x04000CA4 RID: 3236
	public GameObject icon3;

	// Token: 0x04000CA5 RID: 3237
	public GameObject icon3b;

	// Token: 0x04000CA6 RID: 3238
	public GameObject icon4;

	// Token: 0x04000CA7 RID: 3239
	public GameObject icon4b;

	// Token: 0x04000CA8 RID: 3240
	public string date1;

	// Token: 0x04000CA9 RID: 3241
	public string date2;

	// Token: 0x04000CAA RID: 3242
	public string date3;

	// Token: 0x04000CAB RID: 3243
	public string date4;

	// Token: 0x04000CAC RID: 3244
	public MainMenu mm;
}
