using System;
using UnityEngine;

// Token: 0x020000E6 RID: 230
public class LeanBehavior : MonoBehaviour
{
	// Token: 0x060005C4 RID: 1476 RVA: 0x00002188 File Offset: 0x00000388
	private void Start()
	{
	}

	// Token: 0x060005C5 RID: 1477 RVA: 0x00047540 File Offset: 0x00045740
	private void Update()
	{
		if (Input.GetKey(KeyCode.T))
		{
			this.curAngle = Mathf.MoveTowardsAngle(this.curAngle, this.maxAngle, this.speed * Time.deltaTime);
		}
		else
		{
			this.curAngle = Mathf.MoveTowardsAngle(this.curAngle, 0f, this.speed * Time.deltaTime);
		}
		this._Pivot.transform.localRotation = Quaternion.AngleAxis(this.curAngle, Vector3.forward);
	}

	// Token: 0x04000C85 RID: 3205
	public Transform _Pivot;

	// Token: 0x04000C86 RID: 3206
	public float speed = 100f;

	// Token: 0x04000C87 RID: 3207
	public float maxAngle = 20f;

	// Token: 0x04000C88 RID: 3208
	private float curAngle;
}
