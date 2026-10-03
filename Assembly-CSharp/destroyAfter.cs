using System;
using UnityEngine;

// Token: 0x02000191 RID: 401
public class destroyAfter : MonoBehaviour
{
	// Token: 0x060009DE RID: 2526 RVA: 0x000873EC File Offset: 0x000855EC
	private void Update()
	{
		this.timer += Time.deltaTime;
		if (this.destroyTime <= this.timer)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x04001B4F RID: 6991
	public float destroyTime = 5f;

	// Token: 0x04001B50 RID: 6992
	private float timer;
}
