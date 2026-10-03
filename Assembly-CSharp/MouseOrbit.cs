using System;
using UnityEngine;

// Token: 0x02000033 RID: 51
public class MouseOrbit : MonoBehaviour
{
	// Token: 0x060000E3 RID: 227 RVA: 0x0000BB94 File Offset: 0x00009D94
	private void Start()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		this.rotationYAxis = eulerAngles.y;
		this.rotationXAxis = eulerAngles.x;
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x0000BBC8 File Offset: 0x00009DC8
	private void LateUpdate()
	{
		if (this.target)
		{
			if (Input.GetMouseButton(1))
			{
				this.start = false;
				this.velocityX += this.xSpeed * Input.GetAxis("Mouse X") * 0.02f;
				this.velocityY += this.ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
			}
			if (this.start)
			{
				return;
			}
			this.rotationYAxis += this.velocityX;
			this.rotationXAxis -= this.velocityY;
			this.rotationXAxis = MouseOrbit.ClampAngle(this.rotationXAxis, this.yMinLimit, this.yMaxLimit);
			Quaternion rotation = Quaternion.Euler(this.rotationXAxis, this.rotationYAxis, 0f);
			Vector3 point = new Vector3(0f, 0f, -this.distance);
			Vector3 position = rotation * point + this.target.position;
			base.transform.rotation = rotation;
			base.transform.position = position;
			this.velocityX = Mathf.Lerp(this.velocityX, 0f, Time.deltaTime * this.smoothTime);
			this.velocityY = Mathf.Lerp(this.velocityY, 0f, Time.deltaTime * this.smoothTime);
			float axis = Input.GetAxis("Mouse ScrollWheel");
			if (axis < 0f && this.distance < this.distanceMax)
			{
				this.distance += this.scrollSpeed;
				return;
			}
			if (axis > 0f && this.distance > this.distanceMin)
			{
				this.distance -= this.scrollSpeed;
			}
		}
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x000024C7 File Offset: 0x000006C7
	public static float ClampAngle(float angle, float min, float max)
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

	// Token: 0x04000274 RID: 628
	public Transform target;

	// Token: 0x04000275 RID: 629
	public float distance = 5f;

	// Token: 0x04000276 RID: 630
	public float xSpeed = 120f;

	// Token: 0x04000277 RID: 631
	public float ySpeed = 120f;

	// Token: 0x04000278 RID: 632
	public float scrollSpeed = 1f;

	// Token: 0x04000279 RID: 633
	public float yMinLimit = -20f;

	// Token: 0x0400027A RID: 634
	public float yMaxLimit = 80f;

	// Token: 0x0400027B RID: 635
	public float distanceMin = 0.5f;

	// Token: 0x0400027C RID: 636
	public float distanceMax = 15f;

	// Token: 0x0400027D RID: 637
	public float smoothTime = 2f;

	// Token: 0x0400027E RID: 638
	private float rotationYAxis;

	// Token: 0x0400027F RID: 639
	private float rotationXAxis;

	// Token: 0x04000280 RID: 640
	private float velocityX;

	// Token: 0x04000281 RID: 641
	private float velocityY;

	// Token: 0x04000282 RID: 642
	private bool start = true;
}
