using System;
using UnityEngine;

// Token: 0x02000019 RID: 25
public class BoltScript : MonoBehaviour
{
	// Token: 0x0600005D RID: 93 RVA: 0x00004D00 File Offset: 0x00002F00
	public void Start()
	{
		if (this.boltturns < 10)
		{
			base.transform.Translate(Vector3.forward * (-0.002f * (float)(10 - this.boltturns)));
		}
	}

	// Token: 0x040000E3 RID: 227
	public int boltturns;
}
