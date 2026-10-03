using System;
using UnityEngine;

// Token: 0x02000048 RID: 72
public class DeliveryZone : MonoBehaviour
{
	// Token: 0x06000159 RID: 345 RVA: 0x0000F4BC File Offset: 0x0000D6BC
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.GetComponent<PickUp>() != null && !this.trailerObject && other.gameObject.GetComponent<PickUp>().description.Contains(this.deliveryname) && this.CheckDelivery() && !this.delivered2)
		{
			this.delivered2 = true;
			if (this.tracker != null)
			{
				this.tracker.GetComponent<showmission>().ActivateNextWaypoint();
			}
		}
		if (this.trailerObject && other.gameObject.name.Contains(this.deliveryname) && this.CheckDelivery() && !this.delivered2)
		{
			this.delivered2 = true;
			if (this.tracker != null)
			{
				this.tracker.GetComponent<showmission>().ActivateNextWaypoint();
			}
		}
	}

	// Token: 0x0600015A RID: 346 RVA: 0x0000F58C File Offset: 0x0000D78C
	public bool CheckDelivery()
	{
		this.amountDelivered = 0;
		this.delivered = false;
		this.itemsInsideZone = Physics.OverlapSphere(base.transform.position, 13f);
		if (this.numDeliveries == 1)
		{
			foreach (Collider collider in this.itemsInsideZone)
			{
				if (!this.trailerObject)
				{
					if (collider.GetComponent<PickUp>() && collider.GetComponent<PickUp>().description.Contains(this.deliveryname))
					{
						this.delivered = true;
					}
				}
				else if (collider.name.Contains(this.deliveryname))
				{
					this.delivered = true;
				}
			}
		}
		if (this.numDeliveries > 1)
		{
			foreach (Collider collider2 in this.itemsInsideZone)
			{
				if (collider2.GetComponent<PickUp>() && collider2.GetComponent<PickUp>().description.Contains(this.deliveryname))
				{
					this.amountDelivered++;
				}
			}
			if (this.amountDelivered >= this.numDeliveries)
			{
				this.delivered = true;
			}
		}
		return this.delivered;
	}

	// Token: 0x040003A9 RID: 937
	public string deliveryname;

	// Token: 0x040003AA RID: 938
	public int numDeliveries;

	// Token: 0x040003AB RID: 939
	public int amountDelivered;

	// Token: 0x040003AC RID: 940
	private Collider[] itemsInsideZone;

	// Token: 0x040003AD RID: 941
	public bool delivered;

	// Token: 0x040003AE RID: 942
	public bool delivered2;

	// Token: 0x040003AF RID: 943
	public bool keepMarkerActive;

	// Token: 0x040003B0 RID: 944
	public GameObject tracker;

	// Token: 0x040003B1 RID: 945
	public bool trailerObject;
}
