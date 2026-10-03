using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x0200015C RID: 348
public class ViolationCanv : MonoBehaviour
{
	// Token: 0x060008A4 RID: 2212 RVA: 0x000700D4 File Offset: 0x0006E2D4
	public void ViewTicket(GameObject go)
	{
		this.fullCanvas.SetActive(true);
		this.citationObj = go;
		this.citation = go.GetComponent<Citation>();
		int num = 0;
		this.totalPrice = 0;
		this.dateField.GetComponent<Text>().text = string.Concat(new object[]
		{
			this.citation.month,
			"-",
			this.citation.day,
			"-03"
		});
		this.timeField.GetComponent<Text>().text = this.citation.hour;
		if (this.citation.charge_dui > 0)
		{
			this.charges.GetChild(num).GetComponent<Text>().text = "dui";
			if (this.citation.charge_dui > 1)
			{
				Text component = this.charges.GetChild(num).GetComponent<Text>();
				component.text = component.text + " x" + this.citation.charge_dui;
			}
			this.totalPrice += 300 * this.citation.charge_dui;
			num++;
		}
		if (this.citation.charge_evasion > 0)
		{
			this.charges.GetChild(num).GetComponent<Text>().text = "evasion";
			if (this.citation.charge_evasion > 1)
			{
				Text component2 = this.charges.GetChild(num).GetComponent<Text>();
				component2.text = component2.text + " x" + this.citation.charge_evasion;
			}
			this.totalPrice += 200 * this.citation.charge_evasion;
			num++;
		}
		if (this.citation.charge_posession > 0)
		{
			this.charges.GetChild(num).GetComponent<Text>().text = "possession - moonshine";
			if (this.citation.charge_posession > 1)
			{
				Text component3 = this.charges.GetChild(num).GetComponent<Text>();
				component3.text = component3.text + " x" + this.citation.charge_posession;
			}
			this.totalPrice += 200 * this.citation.charge_posession;
			num++;
		}
		if (this.citation.charge_reckless > 0)
		{
			this.charges.GetChild(num).GetComponent<Text>().text = "reckless driving";
			if (this.citation.charge_reckless > 1)
			{
				Text component4 = this.charges.GetChild(num).GetComponent<Text>();
				component4.text = component4.text + " x" + this.citation.charge_reckless;
			}
			this.totalPrice += 150 * this.citation.charge_reckless;
			num++;
		}
		if (this.citation.charge_hitandrun > 0)
		{
			this.charges.GetChild(num).GetComponent<Text>().text = "hit and run";
			if (this.citation.charge_hitandrun > 1)
			{
				Text component5 = this.charges.GetChild(num).GetComponent<Text>();
				component5.text = component5.text + " x" + this.citation.charge_hitandrun;
			}
			this.totalPrice += 300 * this.citation.charge_hitandrun;
			num++;
		}
		if (this.citation.charge_speeding > 0)
		{
			this.charges.GetChild(num).GetComponent<Text>().text = "excessive speed";
			if (this.citation.charge_speeding > 1)
			{
				Text component6 = this.charges.GetChild(num).GetComponent<Text>();
				component6.text = component6.text + " x" + this.citation.charge_speeding;
			}
			this.totalPrice += 75 * this.citation.charge_speeding;
			num++;
		}
		if (this.citation.charge_headlights > 0)
		{
			this.charges.GetChild(num).GetComponent<Text>().text = "headlamps";
			if (this.citation.charge_headlights > 1)
			{
				Text component7 = this.charges.GetChild(num).GetComponent<Text>();
				component7.text = component7.text + " x" + this.citation.charge_headlights;
			}
			this.totalPrice += 50 * this.citation.charge_headlights;
			num++;
		}
		if (this.citation.charge_opencontainer > 0)
		{
			this.charges.GetChild(num).GetComponent<Text>().text = "open container";
			if (this.citation.charge_opencontainer > 1)
			{
				Text component8 = this.charges.GetChild(num).GetComponent<Text>();
				component8.text = component8.text + " x" + this.citation.charge_opencontainer;
			}
			this.totalPrice += 100 * this.citation.charge_opencontainer;
			num++;
		}
		if (this.citation.charge_theft > 0)
		{
			this.charges.GetChild(num).GetComponent<Text>().text = "theft (fuel)";
			if (this.citation.charge_theft > 1)
			{
				Text component9 = this.charges.GetChild(num).GetComponent<Text>();
				component9.text = component9.text + " x" + this.citation.charge_theft;
			}
			this.totalPrice += 50 * this.citation.charge_theft;
			num++;
		}
		if (num < 8 && this.citation.charge_obstruction > 0)
		{
			this.charges.GetChild(num).GetComponent<Text>().text = "obstruction";
			if (this.citation.charge_obstruction > 1)
			{
				Text component10 = this.charges.GetChild(num).GetComponent<Text>();
				component10.text = component10.text + " x" + this.citation.charge_obstruction;
			}
			this.totalPrice += 75 * this.citation.charge_obstruction;
			num++;
		}
		if (num < 8 && this.citation.charge_pubIntox > 0)
		{
			this.charges.GetChild(num).GetComponent<Text>().text = "public intox";
			if (this.citation.charge_pubIntox > 1)
			{
				Text component11 = this.charges.GetChild(num).GetComponent<Text>();
				component11.text = component11.text + " x" + this.citation.charge_pubIntox;
			}
			this.totalPrice += 75 * this.citation.charge_pubIntox;
			num++;
		}
		this.payTxt.GetComponent<Text>().text = this.totalPrice.ToString();
	}

	// Token: 0x060008A5 RID: 2213 RVA: 0x000707C0 File Offset: 0x0006E9C0
	public void DoPay()
	{
		if (this.currency.money >= (float)this.totalPrice)
		{
			this.inv.SubtractMoney((float)this.totalPrice);
			this.DestroyTicket();
			return;
		}
		if (this.atm.balance >= (float)this.totalPrice)
		{
			this.atm.balance -= (float)this.totalPrice;
			this.DestroyTicket();
		}
	}

	// Token: 0x060008A6 RID: 2214 RVA: 0x00070830 File Offset: 0x0006EA30
	private void DestroyTicket()
	{
		this.officer.activeCitations--;
		if (this.officer.delinquentDays > 0)
		{
			this.officer.delinquentDays--;
			this.officer.raidWarrant = false;
		}
		Object.Destroy(this.citationObj);
		this.violationPanel.SetActive(false);
		this.fpc.LockMouse();
		this.fpc.enabled = true;
	}

	// Token: 0x060008A7 RID: 2215 RVA: 0x000708AB File Offset: 0x0006EAAB
	public void OptionExit()
	{
		this.violationPanel.SetActive(false);
		this.fpc.LockMouse();
		this.fpc.enabled = true;
	}

	// Token: 0x040013F9 RID: 5113
	public GameObject fullCanvas;

	// Token: 0x040013FA RID: 5114
	public FirstPersonController fpc;

	// Token: 0x040013FB RID: 5115
	public Transform panel;

	// Token: 0x040013FC RID: 5116
	private string newText;

	// Token: 0x040013FD RID: 5117
	public GameObject payTxt;

	// Token: 0x040013FE RID: 5118
	private int totalPrice;

	// Token: 0x040013FF RID: 5119
	private int i;

	// Token: 0x04001400 RID: 5120
	public Transform charges;

	// Token: 0x04001401 RID: 5121
	public GameObject violationPanel;

	// Token: 0x04001402 RID: 5122
	private Citation citation;

	// Token: 0x04001403 RID: 5123
	private GameObject citationObj;

	// Token: 0x04001404 RID: 5124
	public Atm atm;

	// Token: 0x04001405 RID: 5125
	public Currency currency;

	// Token: 0x04001406 RID: 5126
	public InventoryItems inv;

	// Token: 0x04001407 RID: 5127
	private GameObject go;

	// Token: 0x04001408 RID: 5128
	public GameObject dateField;

	// Token: 0x04001409 RID: 5129
	public GameObject timeField;

	// Token: 0x0400140A RID: 5130
	public Officer officer;
}
