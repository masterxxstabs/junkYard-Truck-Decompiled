using System;
using UnityEngine;

// Token: 0x020000D7 RID: 215
public class JerryCan : MonoBehaviour
{
	// Token: 0x0600056C RID: 1388 RVA: 0x0004466C File Offset: 0x0004286C
	private void Start()
	{
		float num = (float)Mathf.RoundToInt((float)this.fh.fluidlevel / 500f * 100f);
		base.gameObject.GetComponent<PickUp>().description = "Fuel Can(" + num + ")%";
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x000446BD File Offset: 0x000428BD
	public void UpdatePerc()
	{
		this.Start();
	}

	// Token: 0x04000BC8 RID: 3016
	public FluidHandler fh;
}
