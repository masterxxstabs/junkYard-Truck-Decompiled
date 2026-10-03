using System;
using UnityEngine;

// Token: 0x02000047 RID: 71
public class DeliverySphere : MonoBehaviour
{
	// Token: 0x06000157 RID: 343 RVA: 0x0000F468 File Offset: 0x0000D668
	private void OnTriggerEnter(Collider other)
	{
		if (other.name == this.deliveryObj)
		{
			this.currencyScript.money += (float)this.missionCash;
			this.missionCash = 0;
			this.deliveryObj = null;
			this.aSource.Play();
		}
	}

	// Token: 0x040003A5 RID: 933
	public string deliveryObj;

	// Token: 0x040003A6 RID: 934
	public int missionCash;

	// Token: 0x040003A7 RID: 935
	public Currency currencyScript;

	// Token: 0x040003A8 RID: 936
	public AudioSource aSource;
}
