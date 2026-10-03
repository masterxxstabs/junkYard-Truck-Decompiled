using System;
using UnityEngine;

// Token: 0x0200019C RID: 412
public class sleeper : MonoBehaviour
{
	// Token: 0x06000A11 RID: 2577 RVA: 0x0008A36F File Offset: 0x0008856F
	private void Awake()
	{
		base.GetComponent<Rigidbody>().Sleep();
	}

	// Token: 0x06000A12 RID: 2578 RVA: 0x0008A36F File Offset: 0x0008856F
	private void Update()
	{
		base.GetComponent<Rigidbody>().Sleep();
	}
}
