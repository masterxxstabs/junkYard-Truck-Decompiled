using System;
using UnityEngine;

// Token: 0x02000051 RID: 81
public class DrainCoolantF : MonoBehaviour
{
	// Token: 0x06000188 RID: 392 RVA: 0x0001112A File Offset: 0x0000F32A
	private void Start()
	{
		this.scaleChange = new Vector3(-0.002f, -0.002f, 0f);
		this.laminarCoolant.transform.localScale = new Vector3(2f, 2f, 18f);
	}

	// Token: 0x06000189 RID: 393 RVA: 0x0001116C File Offset: 0x0000F36C
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

	// Token: 0x04000474 RID: 1140
	public GameObject laminarCoolant;

	// Token: 0x04000475 RID: 1141
	private Vector3 scaleChange;

	// Token: 0x04000476 RID: 1142
	public car4 truckscript;

	// Token: 0x04000477 RID: 1143
	public GameObject truck;
}
