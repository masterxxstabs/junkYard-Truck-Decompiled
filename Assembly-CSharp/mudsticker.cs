using System;
using UnityEngine;

// Token: 0x0200017D RID: 381
public class mudsticker : MonoBehaviour
{
	// Token: 0x0600094F RID: 2383 RVA: 0x0007DEC6 File Offset: 0x0007C0C6
	public void OnTriggerEnter(Collider c)
	{
		if (c.transform.tag == "Mudblob")
		{
			c.transform.parent = base.gameObject.transform;
		}
	}
}
