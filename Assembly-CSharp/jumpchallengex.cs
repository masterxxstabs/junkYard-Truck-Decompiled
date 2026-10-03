using System;
using UnityEngine;

// Token: 0x0200017A RID: 378
public class jumpchallengex : MonoBehaviour
{
	// Token: 0x06000946 RID: 2374 RVA: 0x0007DCB0 File Offset: 0x0007BEB0
	private void OnTriggerEnter(Collider other)
	{
		this.truck.GetComponent<car>().jumptime = 0f;
	}

	// Token: 0x040018DE RID: 6366
	public GameObject truck;
}
