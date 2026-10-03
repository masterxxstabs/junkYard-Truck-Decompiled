using System;
using UnityEngine;

// Token: 0x02000166 RID: 358
public class WinchType : MonoBehaviour
{
	// Token: 0x060008CD RID: 2253 RVA: 0x0007196C File Offset: 0x0006FB6C
	public void Start()
	{
		if (this.winchType == 2)
		{
			base.GetComponent<Renderer>().material = this.mat2;
			base.GetComponent<durability>().template = this.winch4;
			this.ws.winchForce = 19000;
			return;
		}
		if (this.winchType == 1)
		{
			base.GetComponent<Renderer>().material = this.mat1;
			base.GetComponent<durability>().template = this.winch2;
			this.ws.winchForce = 10000;
		}
	}

	// Token: 0x0400144F RID: 5199
	public int winchType;

	// Token: 0x04001450 RID: 5200
	public Material mat1;

	// Token: 0x04001451 RID: 5201
	public Material mat2;

	// Token: 0x04001452 RID: 5202
	public GameObject winch2;

	// Token: 0x04001453 RID: 5203
	public GameObject winch4;

	// Token: 0x04001454 RID: 5204
	public Winch ws;
}
