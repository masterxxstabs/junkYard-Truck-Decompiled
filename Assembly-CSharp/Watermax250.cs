using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000161 RID: 353
public class Watermax250 : MonoBehaviour
{
	// Token: 0x060008B9 RID: 2233 RVA: 0x00070C8D File Offset: 0x0006EE8D
	private void OnTriggerEnter(Collider collidedObj)
	{
		if (collidedObj.tag == "water" || collidedObj.tag == "DynamicWater")
		{
			this.inWater = true;
			base.StartCoroutine(this.WaterCheck());
		}
	}

	// Token: 0x060008BA RID: 2234 RVA: 0x00070CC7 File Offset: 0x0006EEC7
	private IEnumerator WaterCheck()
	{
		yield return new WaitForSeconds(3f);
		if (this.inWater)
		{
			this.waterCount++;
			if (this.waterCount == 1)
			{
				base.StartCoroutine(this.WaterCheck());
			}
			else
			{
				this.db.waterFlood = 1;
			}
		}
		yield break;
	}

	// Token: 0x060008BB RID: 2235 RVA: 0x00070CD8 File Offset: 0x0006EED8
	public void CheckWaterAgain()
	{
		this.waterInsideZone = Physics.OverlapSphere(base.transform.position, 0.2f);
		foreach (Collider component in this.waterInsideZone)
		{
			this.inWater = false;
			if (component.tag == "water")
			{
				this.inWater = true;
			}
		}
		if (!this.inWater)
		{
			this.db.waterFlood = 0;
		}
	}

	// Token: 0x060008BC RID: 2236 RVA: 0x00070D4B File Offset: 0x0006EF4B
	private void OnTriggerExit(Collider collidedObj)
	{
		if (collidedObj.tag == "water" || collidedObj.tag == "DynamicWater")
		{
			this.db.waterFlood = 0;
			this.waterCount = 0;
			this.inWater = false;
		}
	}

	// Token: 0x04001419 RID: 5145
	public Dirtbike db;

	// Token: 0x0400141A RID: 5146
	private int waterCount;

	// Token: 0x0400141B RID: 5147
	private bool inWater;

	// Token: 0x0400141C RID: 5148
	private Collider[] waterInsideZone;
}
