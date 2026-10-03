using System;
using UnityEngine;

// Token: 0x02000153 RID: 339
public class TowSphere : MonoBehaviour
{
	// Token: 0x0600087A RID: 2170 RVA: 0x0006EA94 File Offset: 0x0006CC94
	private void OnTriggerExit(Collider other)
	{
		if (other.name == this.towObj.name)
		{
			this.currency.money += (float)this.payout;
			this.payout = 0;
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x04001394 RID: 5012
	public GameObject towObj;

	// Token: 0x04001395 RID: 5013
	public int payout;

	// Token: 0x04001396 RID: 5014
	public Currency currency;
}
