using System;
using UnityEngine;

// Token: 0x02000032 RID: 50
public class BuyTruck : MonoBehaviour
{
	// Token: 0x060000DF RID: 223 RVA: 0x0000B7C6 File Offset: 0x000099C6
	private void Start()
	{
		this.SpawnCash();
		if (!this.johnny1.activeSelf)
		{
			this.truck.SetActive(true);
		}
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x0000B7E8 File Offset: 0x000099E8
	private void SpawnCash()
	{
		if (!this.cashSpawned)
		{
			Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation).GetComponent<PickUp>().thisDurability = 200f;
			Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation).GetComponent<PickUp>().thisDurability = 300f;
			this.cashSpawned = true;
		}
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x0000B878 File Offset: 0x00009A78
	public void SpawnTruck(int truckNum)
	{
		if (Vector3.Distance(this.car.transform.position, this.truck.transform.position) < 4f)
		{
			this.car.SetActive(false);
			this.car.transform.position = this.homePosCar.position;
			this.car.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.cardoor_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.cardoor_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.car.SetActive(true);
		}
		if (truckNum == 1 && this.buyTruckNum > 0)
		{
			this.buyTruckNum = 0;
			this.truck.SetActive(true);
			this.truckTemplate1.SetActive(false);
			this.johnny1.SetActive(false);
			this.johnny4.SetActive(true);
			foreach (object obj in this.HolderFL)
			{
				((Transform)obj).gameObject.SetActive(false);
			}
			foreach (object obj2 in this.HolderFR)
			{
				((Transform)obj2).gameObject.SetActive(false);
			}
			foreach (object obj3 in this.HolderRL)
			{
				((Transform)obj3).gameObject.SetActive(false);
			}
			foreach (object obj4 in this.HolderRR)
			{
				((Transform)obj4).gameObject.SetActive(false);
			}
			this.HolderFL.GetChild(1).gameObject.SetActive(true);
			this.HolderFR.GetChild(1).gameObject.SetActive(true);
			this.HolderRL.GetChild(1).gameObject.SetActive(true);
			this.HolderRR.GetChild(1).gameObject.SetActive(true);
			this.HolderFL.GetChild(6).gameObject.SetActive(true);
			this.HolderFR.GetChild(6).gameObject.SetActive(true);
			this.HolderRL.GetChild(6).gameObject.SetActive(true);
			this.HolderRR.GetChild(6).gameObject.SetActive(true);
		}
	}

	// Token: 0x04000247 RID: 583
	public Material cabingreen;

	// Token: 0x04000248 RID: 584
	public Material cabingreen2;

	// Token: 0x04000249 RID: 585
	public Material bodygreen;

	// Token: 0x0400024A RID: 586
	public Material bodygreen2;

	// Token: 0x0400024B RID: 587
	public Material cabinsteel;

	// Token: 0x0400024C RID: 588
	public Material cabinsteel2;

	// Token: 0x0400024D RID: 589
	public Material bodysteel;

	// Token: 0x0400024E RID: 590
	public Material bodysteel2;

	// Token: 0x0400024F RID: 591
	public GameObject truck;

	// Token: 0x04000250 RID: 592
	public GameObject truckCabin;

	// Token: 0x04000251 RID: 593
	public GameObject truckBody;

	// Token: 0x04000252 RID: 594
	public GameObject truckHood;

	// Token: 0x04000253 RID: 595
	public GameObject truckDoorD;

	// Token: 0x04000254 RID: 596
	public GameObject truckDoorP;

	// Token: 0x04000255 RID: 597
	public GameObject truckTailgate;

	// Token: 0x04000256 RID: 598
	public GameObject wheelFL;

	// Token: 0x04000257 RID: 599
	public GameObject wheelFR;

	// Token: 0x04000258 RID: 600
	public GameObject wheelRL;

	// Token: 0x04000259 RID: 601
	public GameObject wheelRR;

	// Token: 0x0400025A RID: 602
	public GameObject jwheelFL;

	// Token: 0x0400025B RID: 603
	public GameObject jwheelFR;

	// Token: 0x0400025C RID: 604
	public GameObject jwheelRL;

	// Token: 0x0400025D RID: 605
	public GameObject jwheelRR;

	// Token: 0x0400025E RID: 606
	public GameObject rollBar;

	// Token: 0x0400025F RID: 607
	public Transform HolderFL;

	// Token: 0x04000260 RID: 608
	public Transform HolderFR;

	// Token: 0x04000261 RID: 609
	public Transform HolderRL;

	// Token: 0x04000262 RID: 610
	public Transform HolderRR;

	// Token: 0x04000263 RID: 611
	public GameObject fan;

	// Token: 0x04000264 RID: 612
	public GameObject valveCover;

	// Token: 0x04000265 RID: 613
	public GameObject oilCap;

	// Token: 0x04000266 RID: 614
	public GameObject transmission;

	// Token: 0x04000267 RID: 615
	public GameObject transfercase;

	// Token: 0x04000268 RID: 616
	public int buyTruckNum;

	// Token: 0x04000269 RID: 617
	public GameObject truckTemplate1;

	// Token: 0x0400026A RID: 618
	public GameObject truckTemplate2;

	// Token: 0x0400026B RID: 619
	public GameObject johnny1;

	// Token: 0x0400026C RID: 620
	public GameObject johnny4;

	// Token: 0x0400026D RID: 621
	public GameObject car;

	// Token: 0x0400026E RID: 622
	public Transform homePosCar;

	// Token: 0x0400026F RID: 623
	public GameObject cardoor_p;

	// Token: 0x04000270 RID: 624
	public GameObject cardoor_d;

	// Token: 0x04000271 RID: 625
	public GameObject moneyRoll;

	// Token: 0x04000272 RID: 626
	public bool cashSpawned;

	// Token: 0x04000273 RID: 627
	public Transform moneyLoc;
}
