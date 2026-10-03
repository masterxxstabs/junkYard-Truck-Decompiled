using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x0200000E RID: 14
public class Atm : MonoBehaviour
{
	// Token: 0x06000027 RID: 39 RVA: 0x00003054 File Offset: 0x00001254
	public void Start()
	{
		this.dCashF = 0f;
		this.wCashF = 0f;
		this.dCash.GetComponent<Text>().text = "$" + 0;
		this.wCash.GetComponent<Text>().text = "$" + 0;
		this.balanceUi.GetComponent<Text>().text = "Balance: $" + this.balance;
		this.Beep();
	}

	// Token: 0x06000028 RID: 40 RVA: 0x000030E2 File Offset: 0x000012E2
	private void Beep()
	{
		this.asource.Play();
	}

	// Token: 0x06000029 RID: 41 RVA: 0x000030F0 File Offset: 0x000012F0
	public void IncreaseDep()
	{
		this.wCash.GetComponent<Text>().text = "$" + 0;
		this.wCashF = 0f;
		if (this.currency.money >= this.dCashF + 20f)
		{
			this.dCashF += 20f;
			this.dCash.GetComponent<Text>().text = "$" + this.dCashF;
			return;
		}
		this.dCashF = this.currency.money;
		this.dCash.GetComponent<Text>().text = "$" + this.currency.money;
	}

	// Token: 0x0600002A RID: 42 RVA: 0x000031B4 File Offset: 0x000013B4
	public void DecreaseDep()
	{
		this.wCash.GetComponent<Text>().text = "$" + 0;
		this.wCashF = 0f;
		if (this.dCashF > 0f && this.dCashF >= 20f)
		{
			this.dCashF -= 20f;
			this.dCash.GetComponent<Text>().text = "$" + this.dCashF;
		}
	}

	// Token: 0x0600002B RID: 43 RVA: 0x00003240 File Offset: 0x00001440
	public void IncreaseWith()
	{
		this.dCash.GetComponent<Text>().text = "$" + 0;
		this.dCashF = 0f;
		if (this.balance >= this.wCashF + 20f)
		{
			this.wCashF += 20f;
			this.wCash.GetComponent<Text>().text = "$" + this.wCashF;
			return;
		}
		this.wCashF = this.balance;
		this.wCash.GetComponent<Text>().text = "$" + this.balance;
	}

	// Token: 0x0600002C RID: 44 RVA: 0x000032F8 File Offset: 0x000014F8
	public void DecreaseWith()
	{
		this.dCash.GetComponent<Text>().text = "$" + 0;
		this.dCashF = 0f;
		if (this.wCashF > 0f && this.wCashF >= 20f)
		{
			this.dCashF -= 20f;
			this.dCash.GetComponent<Text>().text = "$" + this.dCashF;
		}
	}

	// Token: 0x0600002D RID: 45 RVA: 0x00003384 File Offset: 0x00001584
	public void DepAll()
	{
		this.dCashF = this.currency.money;
		this.dCash.GetComponent<Text>().text = "$0";
		this.balance += this.currency.money;
		this.inter.inv.SubtractMoney(this.dCashF);
		this.currency.money = 0f;
		this.balanceUi.GetComponent<Text>().text = "Balance: $" + this.balance;
		this.Beep();
	}

	// Token: 0x0600002E RID: 46 RVA: 0x00003420 File Offset: 0x00001620
	public void Confirm()
	{
		if (this.dCashF > 0f)
		{
			if (this.currency.money >= this.dCashF)
			{
				this.currency.money -= this.dCashF;
				this.inter.inv.SubtractMoney(this.dCashF);
				this.balance += this.dCashF;
			}
		}
		else if (this.wCashF > 0f && this.balance >= this.wCashF)
		{
			this.balance -= this.wCashF;
			this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc1.position, this.moneyLoc1.rotation);
			this.newRoll.GetComponent<PickUp>().thisDurability = this.wCashF;
		}
		this.balanceUi.GetComponent<Text>().text = "Balance: $" + this.balance;
		this.dCashF = 0f;
		this.wCashF = 0f;
		this.dCash.GetComponent<Text>().text = "$" + 0;
		this.wCash.GetComponent<Text>().text = "$" + 0;
		this.Beep();
	}

	// Token: 0x0600002F RID: 47 RVA: 0x0000357F File Offset: 0x0000177F
	public void With400()
	{
		this.wCashF = 400f;
		this.Confirm();
	}

	// Token: 0x06000030 RID: 48 RVA: 0x00003592 File Offset: 0x00001792
	public void With200()
	{
		this.wCashF = 200f;
		this.Confirm();
	}

	// Token: 0x06000031 RID: 49 RVA: 0x000035A5 File Offset: 0x000017A5
	public void ExitAtm()
	{
		this.Beep();
		this.atmPanel.SetActive(false);
		this.fpc.LockMouse();
		this.fpc.enabled = true;
	}

	// Token: 0x0400007C RID: 124
	public float balance;

	// Token: 0x0400007D RID: 125
	public GameObject dCash;

	// Token: 0x0400007E RID: 126
	public GameObject wCash;

	// Token: 0x0400007F RID: 127
	public float dCashF;

	// Token: 0x04000080 RID: 128
	public float wCashF;

	// Token: 0x04000081 RID: 129
	public GameObject moneyRoll;

	// Token: 0x04000082 RID: 130
	public Transform moneyLoc1;

	// Token: 0x04000083 RID: 131
	public Currency currency;

	// Token: 0x04000084 RID: 132
	public Interactor inter;

	// Token: 0x04000085 RID: 133
	public GameObject balanceUi;

	// Token: 0x04000086 RID: 134
	private GameObject newRoll;

	// Token: 0x04000087 RID: 135
	public GameObject atmPanel;

	// Token: 0x04000088 RID: 136
	public FirstPersonController fpc;

	// Token: 0x04000089 RID: 137
	public AudioSource asource;

	// Token: 0x0400008A RID: 138
	public bool ownsHouse;
}
