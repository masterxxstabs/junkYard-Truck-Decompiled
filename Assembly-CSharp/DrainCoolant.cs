using System;
using UnityEngine;

// Token: 0x02000050 RID: 80
public class DrainCoolant : MonoBehaviour
{
	// Token: 0x06000185 RID: 389 RVA: 0x0001100D File Offset: 0x0000F20D
	private void Start()
	{
		this.scaleChange = new Vector3(-0.002f, -0.002f, 0f);
		this.laminarCoolant.transform.localScale = new Vector3(2f, 2f, 18f);
	}

	// Token: 0x06000186 RID: 390 RVA: 0x00011050 File Offset: 0x0000F250
	private void FixedUpdate()
	{
		if (this.truckscript.coolantLevel > 0f)
		{
			this.laminarCoolant.SetActive(true);
			this.laminarCoolant.transform.localScale += this.scaleChange;
			this.laminarCoolant.transform.rotation = Quaternion.Euler(this.truck.transform.rotation.x + 90f, this.truck.transform.rotation.y, this.laminarCoolant.transform.rotation.z - 3f);
			this.truckscript.coolantLevel -= 0.1f;
			return;
		}
		this.laminarCoolant.SetActive(false);
		base.enabled = false;
	}

	// Token: 0x04000470 RID: 1136
	public GameObject laminarCoolant;

	// Token: 0x04000471 RID: 1137
	private Vector3 scaleChange;

	// Token: 0x04000472 RID: 1138
	public car truckscript;

	// Token: 0x04000473 RID: 1139
	public GameObject truck;
}
