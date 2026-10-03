using System;
using UnityEngine;

// Token: 0x02000054 RID: 84
public class DrainOilV8 : MonoBehaviour
{
	// Token: 0x06000191 RID: 401 RVA: 0x00011488 File Offset: 0x0000F688
	private void Start()
	{
		this.scaleChange = new Vector3(-0.01f, -0.01f, 0f);
		this.laminarOil.transform.localScale = new Vector3(12.5f, 12.5f, 59f);
	}

	// Token: 0x06000192 RID: 402 RVA: 0x000114C8 File Offset: 0x0000F6C8
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

	// Token: 0x04000480 RID: 1152
	public GameObject laminarOil;

	// Token: 0x04000481 RID: 1153
	private Vector3 scaleChange;

	// Token: 0x04000482 RID: 1154
	public GameObject block;

	// Token: 0x04000483 RID: 1155
	public enginev8 engineblock;
}
