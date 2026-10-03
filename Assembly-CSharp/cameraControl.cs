using System;
using UnityEngine;

// Token: 0x02000180 RID: 384
public class cameraControl : MonoBehaviour
{
	// Token: 0x06000962 RID: 2402 RVA: 0x0007E4EE File Offset: 0x0007C6EE
	private void Start()
	{
		this.x = base.transform.eulerAngles.y;
		this.y = base.transform.eulerAngles.x;
	}

	// Token: 0x06000963 RID: 2403 RVA: 0x0007E51C File Offset: 0x0007C71C
	private void Update()
	{
		if (this.target != null)
		{
			if (Input.GetMouseButton(0))
			{
				this.x += Input.GetAxis("Mouse X") * this.xSpeed;
				this.y -= Input.GetAxis("Mouse Y") * this.ySpeed;
			}
			this.tempX = Mathf.SmoothDamp(this.tempX, this.x, ref this.xSmooth, this.smoothTime);
			this.tempY = Mathf.SmoothDamp(this.tempY, this.y, ref this.ySmooth, this.smoothTime);
			if (Input.GetMouseButton(1))
			{
				this.distance += Input.GetAxis("Mouse Y") * this.zoomSpeed;
			}
			this.y = this.ClampAngle(this.y, this.minValueY, this.maxValueY);
			this.rotation = Quaternion.Euler(this.tempY, this.tempX, 0f);
			this.position = this.rotation * new Vector3(0f, 0f, -this.distance) + this.target.position;
			base.transform.rotation = this.rotation;
			base.transform.position = this.position;
		}
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x0007E67D File Offset: 0x0007C87D
	private float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	// Token: 0x040018EF RID: 6383
	public Transform target;

	// Token: 0x040018F0 RID: 6384
	public float xSpeed;

	// Token: 0x040018F1 RID: 6385
	public float ySpeed;

	// Token: 0x040018F2 RID: 6386
	public float zoomSpeed = 10f;

	// Token: 0x040018F3 RID: 6387
	public float minValueY = -20f;

	// Token: 0x040018F4 RID: 6388
	public float maxValueY = 80f;

	// Token: 0x040018F5 RID: 6389
	private Quaternion rotation;

	// Token: 0x040018F6 RID: 6390
	private Vector3 position;

	// Token: 0x040018F7 RID: 6391
	private float distance = 5f;

	// Token: 0x040018F8 RID: 6392
	private float x;

	// Token: 0x040018F9 RID: 6393
	private float y;

	// Token: 0x040018FA RID: 6394
	private float tempX;

	// Token: 0x040018FB RID: 6395
	private float tempY;

	// Token: 0x040018FC RID: 6396
	public float smoothTime = 0.3f;

	// Token: 0x040018FD RID: 6397
	private float xSmooth;

	// Token: 0x040018FE RID: 6398
	private float ySmooth;
}
