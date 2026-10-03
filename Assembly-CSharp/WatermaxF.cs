using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000162 RID: 354
public class WatermaxF : MonoBehaviour
{
	// Token: 0x060008BE RID: 2238 RVA: 0x00070D8C File Offset: 0x0006EF8C
	private void OnTriggerEnter(Collider collidedObj)
	{
		if (collidedObj.tag == "water" || collidedObj.tag == "DynamicWater")
		{
			this.inWater = true;
			Debug.Log(collidedObj.gameObject.name);
			base.StartCoroutine(this.WaterCheck());
		}
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x00070DE1 File Offset: 0x0006EFE1
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
				this.truck.waterFlood = 1;
			}
		}
		yield break;
	}

	// Token: 0x060008C0 RID: 2240 RVA: 0x00070DF0 File Offset: 0x0006EFF0
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
			this.truck.waterFlood = 0;
		}
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x00070E64 File Offset: 0x0006F064
	private void OnTriggerExit(Collider collidedObj)
	{
		if (collidedObj.tag == "water" || collidedObj.tag == "DynamicWater")
		{
			this.truck.waterFlood = 0;
			this.waterCount = 0;
			this.inWater = false;
		}
		Debug.Log("waterexit");
	}

	// Token: 0x0400141D RID: 5149
	public car4 truck;

	// Token: 0x0400141E RID: 5150
	private int waterCount;

	// Token: 0x0400141F RID: 5151
	private bool inWater;

	// Token: 0x04001420 RID: 5152
	private Collider[] waterInsideZone;
}
