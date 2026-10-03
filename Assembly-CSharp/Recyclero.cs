using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000123 RID: 291
public class Recyclero : MonoBehaviour
{
	// Token: 0x060007A7 RID: 1959 RVA: 0x0006311C File Offset: 0x0006131C
	public void CheckRecycleTotal()
	{
		this.money = GameObject.FindWithTag("GameController").GetComponent<Currency>().money;
		this.newcashj = 0f;
		this.itemsInsideZone = Physics.OverlapSphere(base.transform.position, 2f);
		foreach (Collider collider in this.itemsInsideZone)
		{
			if (collider.GetComponent<PickUp>() && collider.GetComponent<PickUp>().description == "Iron Ore")
			{
				this.newcashj += collider.GetComponent<PickUp>().tradein;
				Object.Destroy(collider.gameObject);
			}
			this.weight = 0f;
		}
		if (this.newcashj > 0f)
		{
			this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
			this.newRoll.GetComponent<PickUp>().thisDurability = this.newcashj;
			this.weightText.GetComponent<Text>().text = "0 Kg";
			if (this.beamsMissionStarted)
			{
				this.oreCollected += this.newcashj;
			}
		}
	}

	// Token: 0x060007A8 RID: 1960 RVA: 0x00063258 File Offset: 0x00061458
	public void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<PickUp>().description == "Iron Ore")
		{
			this.weight += Mathf.Round(other.GetComponent<Rigidbody>().mass);
			this.weightText.GetComponent<Text>().text = this.weight + " Kg";
		}
	}

	// Token: 0x060007A9 RID: 1961 RVA: 0x000632C0 File Offset: 0x000614C0
	public void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<PickUp>().description == "Iron Ore")
		{
			this.weight -= Mathf.Round(other.GetComponent<Rigidbody>().mass);
			this.weightText.GetComponent<Text>().text = this.weight + " Kg";
		}
	}

	// Token: 0x04001175 RID: 4469
	private float newcashj;

	// Token: 0x04001176 RID: 4470
	private float newmoney;

	// Token: 0x04001177 RID: 4471
	private float money;

	// Token: 0x04001178 RID: 4472
	private Collider[] itemsInsideZone;

	// Token: 0x04001179 RID: 4473
	public GameObject weightText;

	// Token: 0x0400117A RID: 4474
	private float weight;

	// Token: 0x0400117B RID: 4475
	public Transform moneyLoc;

	// Token: 0x0400117C RID: 4476
	public GameObject moneyRoll;

	// Token: 0x0400117D RID: 4477
	private GameObject newRoll;

	// Token: 0x0400117E RID: 4478
	public bool beamsMissionStarted;

	// Token: 0x0400117F RID: 4479
	public float oreCollected;
}
