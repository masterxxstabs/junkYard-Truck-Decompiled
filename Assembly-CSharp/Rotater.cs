using System;
using UnityEngine;

// Token: 0x02000034 RID: 52
public class Rotater : MonoBehaviour
{
	// Token: 0x060000E7 RID: 231 RVA: 0x0000BE09 File Offset: 0x0000A009
	private void Update()
	{
		base.transform.Rotate(0f, 1f * this.Speed, 0f, Space.Self);
	}

	// Token: 0x04000283 RID: 643
	public float Speed;
}
