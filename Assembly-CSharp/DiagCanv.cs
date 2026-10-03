using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x0200004A RID: 74
public class DiagCanv : MonoBehaviour
{
	// Token: 0x0600015E RID: 350 RVA: 0x0000F74C File Offset: 0x0000D94C
	public void DoFix()
	{
		this.diag.Fix();
		this.fullCanvas.SetActive(false);
		this.fpc.LockMouse();
		this.fpc.enabled = true;
	}

	// Token: 0x0600015F RID: 351 RVA: 0x0000F77C File Offset: 0x0000D97C
	public void CheckBox(int boxNum)
	{
		GameObject gameObject = this.panel.GetChild(boxNum).gameObject;
		string text = gameObject.GetComponent<Text>().text;
		if (text.Substring(0, 3) == "[ ]")
		{
			this.newText = text.Replace("[ ]", "[x]");
		}
		else
		{
			this.newText = text.Replace("[x]", "[ ]");
		}
		gameObject.GetComponent<Text>().text = this.newText;
		this.UpdateTotal();
	}

	// Token: 0x06000160 RID: 352 RVA: 0x0000F800 File Offset: 0x0000DA00
	public void UpdateTotal()
	{
		this.totalPrice = 0;
		this.i = 0;
		while (this.i <= 35)
		{
			if (this.listEntries[this.i].GetComponent<Text>().text.Substring(0, 3) == "[x]")
			{
				this.totalPrice += this.diag.priceArr[this.i];
			}
			this.i++;
		}
		this.repairTxt.GetComponent<Text>().text = "Repair Estimate: $" + this.totalPrice;
	}

	// Token: 0x06000161 RID: 353 RVA: 0x0000F8A3 File Offset: 0x0000DAA3
	public void OptionExit()
	{
		this.diagPanel.SetActive(false);
		this.fpc.LockMouse();
		this.fpc.enabled = true;
	}

	// Token: 0x040003B8 RID: 952
	public Diagnostic diag;

	// Token: 0x040003B9 RID: 953
	public GameObject fullCanvas;

	// Token: 0x040003BA RID: 954
	public FirstPersonController fpc;

	// Token: 0x040003BB RID: 955
	public Transform panel;

	// Token: 0x040003BC RID: 956
	private string newText;

	// Token: 0x040003BD RID: 957
	public GameObject repairTxt;

	// Token: 0x040003BE RID: 958
	private int newPrice;

	// Token: 0x040003BF RID: 959
	private int totalPrice;

	// Token: 0x040003C0 RID: 960
	private int i;

	// Token: 0x040003C1 RID: 961
	public GameObject[] listEntries;

	// Token: 0x040003C2 RID: 962
	public GameObject diagPanel;
}
