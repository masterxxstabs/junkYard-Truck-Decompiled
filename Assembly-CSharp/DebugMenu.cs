using System;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x02000046 RID: 70
public class DebugMenu : MonoBehaviour
{
	// Token: 0x0600014A RID: 330 RVA: 0x00002188 File Offset: 0x00000388
	private void Start()
	{
	}

	// Token: 0x0600014B RID: 331 RVA: 0x0000F214 File Offset: 0x0000D414
	public void Holdem()
	{
		this.holdem.SetActive(true);
	}

	// Token: 0x0600014C RID: 332 RVA: 0x0000F224 File Offset: 0x0000D424
	public void SpawnMoney()
	{
		this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.leanDest.transform.position, this.leanDest.transform.rotation);
		this.newRoll.GetComponent<PickUp>().thisDurability = 300f;
	}

	// Token: 0x0600014D RID: 333 RVA: 0x0000F277 File Offset: 0x0000D477
	public void SpawnTruck()
	{
		this.inter.BuyTruck(1);
	}

	// Token: 0x0600014E RID: 334 RVA: 0x0000F288 File Offset: 0x0000D488
	public void RelocV4()
	{
		this.v4.transform.parent = null;
		this.v4.GetComponent<FixedJoint>().connectedBody = GameObject.Find("EmptyObjRigidbody").GetComponent<Rigidbody>();
		this.v4.transform.position = this.leanDest.position;
		this.v4.GetComponent<PickUp>().pickable = true;
	}

	// Token: 0x0600014F RID: 335 RVA: 0x0000F2F1 File Offset: 0x0000D4F1
	public void RelocB1()
	{
		this.bucket1.transform.parent = null;
		this.bucket1.transform.position = this.leanDest.position;
	}

	// Token: 0x06000150 RID: 336 RVA: 0x0000F31F File Offset: 0x0000D51F
	public void RelocCan()
	{
		this.can.transform.parent = null;
		this.can.transform.position = this.leanDest.position;
	}

	// Token: 0x06000151 RID: 337 RVA: 0x0000F34D File Offset: 0x0000D54D
	public void RelocCreep()
	{
		this.creeper.transform.parent = null;
		this.creeper.transform.position = this.leanDest.position;
	}

	// Token: 0x06000152 RID: 338 RVA: 0x0000F37B File Offset: 0x0000D57B
	public void RelocChain()
	{
		this.chainsaw.transform.parent = null;
		this.chainsaw.transform.position = this.leanDest.position;
	}

	// Token: 0x06000153 RID: 339 RVA: 0x0000F3A9 File Offset: 0x0000D5A9
	public void RelocB2()
	{
		this.bucket2.transform.parent = null;
		this.bucket2.transform.position = this.leanDest.position;
	}

	// Token: 0x06000154 RID: 340 RVA: 0x0000F3D8 File Offset: 0x0000D5D8
	public void RelocV8()
	{
		this.v8.transform.parent = null;
		this.v8.GetComponent<FixedJoint>().connectedBody = GameObject.Find("EmptyObjRigidbodyV8").GetComponent<Rigidbody>();
		this.v8.transform.position = this.leanDest.position;
		this.v4.GetComponent<PickUp>().pickable = true;
	}

	// Token: 0x06000155 RID: 341 RVA: 0x0000F441 File Offset: 0x0000D641
	public void OptionExit()
	{
		this.debugPanel.SetActive(false);
		this.fpc.LockMouse();
		this.fpc.enabled = true;
	}

	// Token: 0x04000397 RID: 919
	public FirstPersonController fpc;

	// Token: 0x04000398 RID: 920
	public GameObject moneyRoll;

	// Token: 0x04000399 RID: 921
	public Interactor inter;

	// Token: 0x0400039A RID: 922
	public GameObject v4;

	// Token: 0x0400039B RID: 923
	public GameObject v8;

	// Token: 0x0400039C RID: 924
	public GameObject bucket1;

	// Token: 0x0400039D RID: 925
	public GameObject bucket2;

	// Token: 0x0400039E RID: 926
	public GameObject can;

	// Token: 0x0400039F RID: 927
	public GameObject chainsaw;

	// Token: 0x040003A0 RID: 928
	public GameObject creeper;

	// Token: 0x040003A1 RID: 929
	private GameObject newRoll;

	// Token: 0x040003A2 RID: 930
	public Transform leanDest;

	// Token: 0x040003A3 RID: 931
	public GameObject debugPanel;

	// Token: 0x040003A4 RID: 932
	public GameObject holdem;
}
