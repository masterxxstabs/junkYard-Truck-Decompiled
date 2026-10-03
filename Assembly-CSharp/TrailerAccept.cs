using System;
using UnityEngine;

// Token: 0x02000155 RID: 341
public class TrailerAccept : MonoBehaviour
{
	// Token: 0x06000888 RID: 2184 RVA: 0x0006F11F File Offset: 0x0006D31F
	private void OnTriggerEnter(Collider other)
	{
		if (other.name == this.deliveryObj)
		{
			this.completed = true;
		}
	}

	// Token: 0x06000889 RID: 2185 RVA: 0x0006F13B File Offset: 0x0006D33B
	private void OnTriggerExit(Collider other)
	{
		if (other.name == this.deliveryObj)
		{
			this.completed = false;
		}
	}

	// Token: 0x040013BD RID: 5053
	public string deliveryObj;

	// Token: 0x040013BE RID: 5054
	public bool completed;
}
