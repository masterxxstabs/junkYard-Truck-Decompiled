using System;
using UnityEngine;

// Token: 0x0200019E RID: 414
public class testenable : MonoBehaviour
{
	// Token: 0x06000A19 RID: 2585 RVA: 0x00002188 File Offset: 0x00000388
	private void Start()
	{
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x0008A47B File Offset: 0x0008867B
	private void Update()
	{
		if (Input.GetKeyDown("space"))
		{
			this.canv.SetActive(true);
		}
	}

	// Token: 0x04001C02 RID: 7170
	public GameObject canv;
}
