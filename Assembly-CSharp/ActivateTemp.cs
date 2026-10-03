using System;
using UnityEngine;

// Token: 0x02000008 RID: 8
public class ActivateTemp : MonoBehaviour
{
	// Token: 0x06000017 RID: 23 RVA: 0x0000258C File Offset: 0x0000078C
	private void Start()
	{
		base.gameObject.GetComponent<Renderer>().enabled = true;
	}
}
