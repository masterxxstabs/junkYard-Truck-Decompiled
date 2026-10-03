using System;
using UnityEngine;

// Token: 0x020000FE RID: 254
public class Oscillate : MonoBehaviour
{
	// Token: 0x06000680 RID: 1664 RVA: 0x0004DF83 File Offset: 0x0004C183
	private void Start()
	{
		this._initPos = base.transform.position;
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x0004DF98 File Offset: 0x0004C198
	private void Update()
	{
		float y = this._initPos.y + Mathf.Sin(this.speed * Time.time) * this.height;
		base.transform.position = new Vector3(this._initPos.x, y, this._initPos.z);
	}

	// Token: 0x04000DA7 RID: 3495
	public float height;

	// Token: 0x04000DA8 RID: 3496
	public float speed;

	// Token: 0x04000DA9 RID: 3497
	private Vector3 _initPos;
}
