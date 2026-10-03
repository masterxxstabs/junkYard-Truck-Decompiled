using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001A2 RID: 418
public class watermaxCar : MonoBehaviour
{
	// Token: 0x06000A28 RID: 2600 RVA: 0x0008A888 File Offset: 0x00088A88
	private void OnTriggerEnter(Collider collidedObj)
	{
		if (collidedObj.tag == "water" || collidedObj.tag == "DynamicWater")
		{
			this.inWater = true;
			base.StartCoroutine(this.WaterCheck());
		}
	}

	// Token: 0x06000A29 RID: 2601 RVA: 0x0008A8C2 File Offset: 0x00088AC2
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
				this.amc.waterFlood = 1;
			}
		}
		yield break;
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x0008A8D4 File Offset: 0x00088AD4
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
			this.amc.waterFlood = 0;
		}
	}

	// Token: 0x06000A2B RID: 2603 RVA: 0x0008A947 File Offset: 0x00088B47
	private void OnTriggerExit(Collider collidedObj)
	{
		if (collidedObj.tag == "water" || collidedObj.tag == "DynamicWater")
		{
			this.amc.waterFlood = 0;
			this.waterCount = 0;
			this.inWater = false;
		}
	}

	// Token: 0x04001C26 RID: 7206
	public car3 amc;

	// Token: 0x04001C27 RID: 7207
	private int waterCount;

	// Token: 0x04001C28 RID: 7208
	private bool inWater;

	// Token: 0x04001C29 RID: 7209
	private Collider[] waterInsideZone;
}
