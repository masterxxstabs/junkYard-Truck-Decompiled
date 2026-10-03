using System;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x0200010F RID: 271
public class PayPhone : MonoBehaviour
{
	// Token: 0x06000717 RID: 1815 RVA: 0x0005B10D File Offset: 0x0005930D
	private void OnEnable()
	{
		Debug.Log("enabling");
		this.coin.Play();
		this.saveTxt.SetActive(true);
	}

	// Token: 0x06000718 RID: 1816 RVA: 0x0005B130 File Offset: 0x00059330
	public void OptionSave50()
	{
		if (this.person.transform.parent == null)
		{
			this.saveTxt.SetActive(false);
			this.mm.OptionSave();
			this.OptionExit();
		}
	}

	// Token: 0x06000719 RID: 1817 RVA: 0x0005B167 File Offset: 0x00059367
	public void OptionExit()
	{
		this.payPhonePanel.SetActive(false);
		this.fpc.LockMouse();
		this.fpc.enabled = true;
	}

	// Token: 0x04000FEF RID: 4079
	public Currency currency;

	// Token: 0x04000FF0 RID: 4080
	public MainMenu mm;

	// Token: 0x04000FF1 RID: 4081
	public FirstPersonController fpc;

	// Token: 0x04000FF2 RID: 4082
	public GameObject payPhonePanel;

	// Token: 0x04000FF3 RID: 4083
	public GameObject saveTxt;

	// Token: 0x04000FF4 RID: 4084
	private int cost;

	// Token: 0x04000FF5 RID: 4085
	public GameObject person;

	// Token: 0x04000FF6 RID: 4086
	public AudioSource coin;
}
