using System;
using UnityEngine;

// Token: 0x02000053 RID: 83
public class DrainOilI6 : MonoBehaviour
{
	// Token: 0x0600018E RID: 398 RVA: 0x00011368 File Offset: 0x0000F568
	private void Start()
	{
		this.scaleChange = new Vector3(-0.01f, -0.01f, 0f);
		this.laminarOil.transform.localScale = new Vector3(12.5f, 12.5f, 59f);
	}

	// Token: 0x0600018F RID: 399 RVA: 0x000113A8 File Offset: 0x0000F5A8
	private void FixedUpdate()
	{
		if (this.engineblock.newOilLevel > 0f)
		{
			this.laminarOil.SetActive(true);
			this.laminarOil.transform.localScale += this.scaleChange;
			this.laminarOil.transform.rotation = Quaternion.Euler(this.block.transform.rotation.x + -270f, this.block.transform.rotation.y * -1f, this.laminarOil.transform.rotation.z - 3f);
			this.engineblock.newOilLevel -= 0.1f;
			return;
		}
		this.laminarOil.SetActive(false);
		base.enabled = false;
	}

	// Token: 0x0400047C RID: 1148
	public GameObject laminarOil;

	// Token: 0x0400047D RID: 1149
	private Vector3 scaleChange;

	// Token: 0x0400047E RID: 1150
	public GameObject block;

	// Token: 0x0400047F RID: 1151
	public enginei6 engineblock;
}
