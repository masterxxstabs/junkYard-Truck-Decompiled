using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x020000CE RID: 206
public class ImpoundCanvas : MonoBehaviour
{
	// Token: 0x060004C8 RID: 1224 RVA: 0x00031174 File Offset: 0x0002F374
	private void OnEnable()
	{
		if (this.interactor.diamondbackImpounded)
		{
			this.option[1].SetActive(true);
		}
		if (this.interactor.f100Impounded)
		{
			this.option[2].SetActive(true);
		}
		if (this.interactor.amcImpounded)
		{
			this.option[3].SetActive(true);
		}
		if (this.interactor.dirtbikeImpounded)
		{
			this.option[4].SetActive(true);
		}
		if (this.interactor.golfcartImpounded)
		{
			this.option[5].SetActive(true);
		}
		if (this.officer.activeCitations > 0)
		{
			this.option[6].SetActive(true);
		}
	}

	// Token: 0x060004C9 RID: 1225 RVA: 0x00031224 File Offset: 0x0002F424
	public void Release(int carNum)
	{
		if (this.currency.money >= 200f)
		{
			if (!this.gate.isOpen)
			{
				this.gate.PerformAction();
			}
			this.inv.SubtractMoney(100f);
			this.currency.money -= 100f;
			this.option[carNum].SetActive(false);
			switch (carNum)
			{
			case 1:
				this.interactor.diamondbackImpounded = false;
				return;
			case 2:
				this.interactor.f100Impounded = false;
				return;
			case 3:
				this.interactor.amcImpounded = false;
				return;
			case 4:
				this.interactor.dirtbikeImpounded = false;
				return;
			case 5:
				this.interactor.golfcartImpounded = false;
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x060004CA RID: 1226 RVA: 0x000312F0 File Offset: 0x0002F4F0
	public void Reprint()
	{
		this.printCoroutine = base.StartCoroutine(this.PrintNew());
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x00031304 File Offset: 0x0002F504
	private IEnumerator PrintNew()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("citation");
		foreach (GameObject citationObj in array)
		{
			yield return new WaitForSeconds(0.2f);
			citationObj.transform.position = this.printLoc.position;
			citationObj.transform.rotation = this.printLoc.rotation;
			citationObj = null;
		}
		GameObject[] array2 = null;
		yield return new WaitForSeconds(0.1f);
		this.OptionExit();
		yield break;
	}

	// Token: 0x060004CC RID: 1228 RVA: 0x00031313 File Offset: 0x0002F513
	public void OptionExit()
	{
		this.impoundPanel.SetActive(false);
		this.fpc.LockMouse();
		this.fpc.enabled = true;
	}

	// Token: 0x04000989 RID: 2441
	public Interactor interactor;

	// Token: 0x0400098A RID: 2442
	public FirstPersonController fpc;

	// Token: 0x0400098B RID: 2443
	public Currency currency;

	// Token: 0x0400098C RID: 2444
	public InteractiveObject gate;

	// Token: 0x0400098D RID: 2445
	public InventoryItems inv;

	// Token: 0x0400098E RID: 2446
	public GameObject[] option;

	// Token: 0x0400098F RID: 2447
	public GameObject impoundPanel;

	// Token: 0x04000990 RID: 2448
	private int fullAmount;

	// Token: 0x04000991 RID: 2449
	public Officer officer;

	// Token: 0x04000992 RID: 2450
	private Coroutine printCoroutine;

	// Token: 0x04000993 RID: 2451
	public Transform printLoc;
}
