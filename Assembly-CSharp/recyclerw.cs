using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000198 RID: 408
public class recyclerw : MonoBehaviour
{
	// Token: 0x060009FD RID: 2557 RVA: 0x00089C98 File Offset: 0x00087E98
	public void CheckRecycleTotal()
	{
		this.money = GameObject.FindWithTag("GameController").GetComponent<Currency>().money;
		this.newcashj = 0f;
		this.itemsInsideZone = Physics.OverlapSphere(base.transform.position, 2f);
		foreach (Collider collider in this.itemsInsideZone)
		{
			if (collider.GetComponent<PickUp>() && collider.GetComponent<PickUp>().description == "Log")
			{
				if (collider.name.Contains("mission"))
				{
					this.missionLogs++;
				}
				this.newcashj += 30f;
				Object.Destroy(collider.gameObject);
			}
		}
		if (this.newcashj > 0f)
		{
			this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
			this.newRoll.GetComponent<PickUp>().thisDurability = this.newcashj;
			this.weightText.GetComponent<Text>().text = "0 Kg";
			if (this.missionLogs > 0)
			{
				this.missionLogs = 0;
				if (this.mg.missionType == 2 || this.mg.missionType == 5)
				{
					this.mg.DoCompletion();
				}
			}
		}
		this.weight = 0;
	}

	// Token: 0x060009FE RID: 2558 RVA: 0x00089E0C File Offset: 0x0008800C
	public void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<PickUp>().description == "Log")
		{
			this.weight += 80;
			this.weightText.GetComponent<Text>().text = this.weight + " Kg";
		}
	}

	// Token: 0x060009FF RID: 2559 RVA: 0x00089E64 File Offset: 0x00088064
	public void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<PickUp>().description == "Log")
		{
			this.weight -= 80;
			this.weightText.GetComponent<Text>().text = this.weight + " Kg";
		}
	}

	// Token: 0x04001BE7 RID: 7143
	private float newcashj;

	// Token: 0x04001BE8 RID: 7144
	private float newmoney;

	// Token: 0x04001BE9 RID: 7145
	private float money;

	// Token: 0x04001BEA RID: 7146
	private Collider[] itemsInsideZone;

	// Token: 0x04001BEB RID: 7147
	public GameObject weightText;

	// Token: 0x04001BEC RID: 7148
	private int weight;

	// Token: 0x04001BED RID: 7149
	public Transform moneyLoc;

	// Token: 0x04001BEE RID: 7150
	public GameObject moneyRoll;

	// Token: 0x04001BEF RID: 7151
	private GameObject newRoll;

	// Token: 0x04001BF0 RID: 7152
	public MissionGen mg;

	// Token: 0x04001BF1 RID: 7153
	private int missionLogs;
}
