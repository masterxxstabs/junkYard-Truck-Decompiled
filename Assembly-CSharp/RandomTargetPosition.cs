using System;
using UnityEngine;

// Token: 0x0200000B RID: 11
public class RandomTargetPosition : MonoBehaviour
{
	// Token: 0x0600001E RID: 30 RVA: 0x00002618 File Offset: 0x00000818
	public void setRandomPos()
	{
		float num = (float)Random.Range(-200, 200);
		base.transform.position = new Vector3(num, 0f, num);
	}

	// Token: 0x0600001F RID: 31 RVA: 0x0000264D File Offset: 0x0000084D
	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("yes");
		this.setRandomPos();
	}
}
