using System;
using UnityEngine;

// Token: 0x020000FF RID: 255
[RequireComponent(typeof(Rigidbody))]
public class RotateRigidbody : MonoBehaviour
{
	// Token: 0x06000683 RID: 1667 RVA: 0x0004DFF1 File Offset: 0x0004C1F1
	private void Start()
	{
		this._angle = 0f;
		this._rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x0004E00C File Offset: 0x0004C20C
	private void Update()
	{
		this._angle = Time.deltaTime * this.speed;
		this._rb.MoveRotation(base.transform.rotation * Quaternion.Euler(this._angle * this.axis.x, this._angle * this.axis.y, this._angle * this.axis.z));
	}

	// Token: 0x04000DAA RID: 3498
	public float speed;

	// Token: 0x04000DAB RID: 3499
	public Vector3 axis;

	// Token: 0x04000DAC RID: 3500
	private float _angle;

	// Token: 0x04000DAD RID: 3501
	private Rigidbody _rb;
}
