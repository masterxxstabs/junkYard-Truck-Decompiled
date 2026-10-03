using System;
using UnityEngine;

// Token: 0x02000052 RID: 82
public class DrainOil : MonoBehaviour
{
	// Token: 0x0600018B RID: 395 RVA: 0x00011246 File Offset: 0x0000F446
	private void Start()
	{
		this.scaleChange = new Vector3(-0.01f, -0.01f, 0f);
		this.laminarOil.transform.localScale = new Vector3(12.5f, 12.5f, 59f);
	}

	// Token: 0x0600018C RID: 396 RVA: 0x00011288 File Offset: 0x0000F488
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

	// Token: 0x04000478 RID: 1144
	public GameObject laminarOil;

	// Token: 0x04000479 RID: 1145
	private Vector3 scaleChange;

	// Token: 0x0400047A RID: 1146
	public GameObject block;

	// Token: 0x0400047B RID: 1147
	public engine engineblock;
}
