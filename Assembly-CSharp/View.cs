using System;
using UnityEngine;

// Token: 0x020000C9 RID: 201
public class View : MonoBehaviour
{
	// Token: 0x060004A3 RID: 1187 RVA: 0x0002FFBA File Offset: 0x0002E1BA
	private void Update()
	{
		base.transform.LookAt(this.Target, Vector3.up);
	}

	// Token: 0x04000962 RID: 2402
	public Transform Target;
}
