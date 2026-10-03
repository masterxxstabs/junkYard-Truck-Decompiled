using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000160 RID: 352
public class Watermax : MonoBehaviour
{
	// Token: 0x060008B4 RID: 2228 RVA: 0x00070B7B File Offset: 0x0006ED7B
	private void OnTriggerEnter(Collider collidedObj)
	{
		if (collidedObj.tag == "water" || collidedObj.tag == "DynamicWater")
		{
			this.inWater = true;
			base.StartCoroutine(this.WaterCheck());
		}
	}

	// Token: 0x060008B5 RID: 2229 RVA: 0x00070BB5 File Offset: 0x0006EDB5
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

	// Token: 0x060008B6 RID: 2230 RVA: 0x00070BC4 File Offset: 0x0006EDC4
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

	// Token: 0x060008B7 RID: 2231 RVA: 0x00070C38 File Offset: 0x0006EE38
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

	// Token: 0x04001415 RID: 5141
	public car truck;

	// Token: 0x04001416 RID: 5142
	private int waterCount;

	// Token: 0x04001417 RID: 5143
	private bool inWater;

	// Token: 0x04001418 RID: 5144
	private Collider[] waterInsideZone;
}
