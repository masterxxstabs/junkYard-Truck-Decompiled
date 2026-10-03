using System;
using UnityEngine;

// Token: 0x0200015B RID: 347
public class VetteWaterCheck : MonoBehaviour
{
	// Token: 0x060008A1 RID: 2209 RVA: 0x00070092 File Offset: 0x0006E292
	private void OnTriggerEnter(Collider collidedObj)
	{
		if (collidedObj.tag == "water")
		{
			this.inWater = true;
		}
	}

	// Token: 0x060008A2 RID: 2210 RVA: 0x000700AD File Offset: 0x0006E2AD
	private void OnTriggerExit(Collider collidedObj)
	{
		if (collidedObj.tag == "water")
		{
			this.inWater = false;
			this.sm.RemoveThisWaypoint();
		}
	}

	// Token: 0x040013F7 RID: 5111
	public showmission sm;

	// Token: 0x040013F8 RID: 5112
	public bool inWater;
}
