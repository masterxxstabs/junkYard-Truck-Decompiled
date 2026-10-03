using System;
using UnityEngine;

// Token: 0x02000145 RID: 325
public class SparksExecutioner : MonoBehaviour
{
	// Token: 0x06000853 RID: 2131 RVA: 0x0006DA6C File Offset: 0x0006BC6C
	private void Start()
	{
		base.Invoke("Kill", this.Lifetime);
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x0006DA7F File Offset: 0x0006BC7F
	private void Kill()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x0400134E RID: 4942
	public float Lifetime = 1f;
}
