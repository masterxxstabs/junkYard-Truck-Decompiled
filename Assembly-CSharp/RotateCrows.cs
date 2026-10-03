using System;
using UnityEngine;

// Token: 0x02000006 RID: 6
public class RotateCrows : MonoBehaviour
{
	// Token: 0x06000012 RID: 18 RVA: 0x00002188 File Offset: 0x00000388
	private void Start()
	{
	}

	// Token: 0x06000013 RID: 19 RVA: 0x0000255F File Offset: 0x0000075F
	private void Update()
	{
		base.transform.Rotate(new Vector3(this.X, this.Y, this.Z));
	}

	// Token: 0x04000012 RID: 18
	public float X;

	// Token: 0x04000013 RID: 19
	public float Y;

	// Token: 0x04000014 RID: 20
	public float Z;
}
