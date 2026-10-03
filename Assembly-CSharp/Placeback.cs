using System;
using UnityEngine;

// Token: 0x02000113 RID: 275
public class Placeback : MonoBehaviour
{
	// Token: 0x06000742 RID: 1858 RVA: 0x0005E8DC File Offset: 0x0005CADC
	private void Start()
	{
		this.pos = base.transform.position;
		this.rot = base.transform.rotation;
	}

	// Token: 0x04001042 RID: 4162
	[HideInInspector]
	public Vector3 pos;

	// Token: 0x04001043 RID: 4163
	[HideInInspector]
	public Quaternion rot;
}
