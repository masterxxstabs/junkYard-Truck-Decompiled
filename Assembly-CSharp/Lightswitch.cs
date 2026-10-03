using System;
using UnityEngine;

// Token: 0x020000E9 RID: 233
public class Lightswitch : MonoBehaviour
{
	// Token: 0x060005CC RID: 1484 RVA: 0x0004770C File Offset: 0x0004590C
	public void Switch()
	{
		if (this.light1 != null)
		{
			this.light1.enabled = !this.on;
		}
		if (this.light2 != null)
		{
			this.light2.enabled = !this.on;
		}
		if (this.light3 != null)
		{
			this.light3.enabled = !this.on;
		}
		if (this.light4 != null)
		{
			this.light4.enabled = !this.on;
		}
		if (this.light5 != null)
		{
			this.light5.enabled = !this.on;
		}
		this.on = !this.on;
	}

	// Token: 0x04000C92 RID: 3218
	public Light light1;

	// Token: 0x04000C93 RID: 3219
	public Light light2;

	// Token: 0x04000C94 RID: 3220
	public Light light3;

	// Token: 0x04000C95 RID: 3221
	public Light light4;

	// Token: 0x04000C96 RID: 3222
	public Light light5;

	// Token: 0x04000C97 RID: 3223
	public bool on;
}
