using System;
using UnityEngine;

// Token: 0x02000005 RID: 5
[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour
{
	// Token: 0x0600000E RID: 14 RVA: 0x0000234C File Offset: 0x0000054C
	private void Start()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		this.x = eulerAngles.y;
		this.y = eulerAngles.x;
		this.rigidbody = base.GetComponent<Rigidbody>();
		if (this.rigidbody != null)
		{
			this.rigidbody.freezeRotation = true;
		}
	}

	// Token: 0x0600000F RID: 15 RVA: 0x000023A4 File Offset: 0x000005A4
	private void LateUpdate()
	{
		if (this.target)
		{
			if (Input.GetKey(this.rotationKey))
			{
				this.x += Input.GetAxis("Mouse X") * this.xSpeed * this.distance * 0.02f;
				this.y -= Input.GetAxis("Mouse Y") * this.ySpeed * 0.02f;
				this.y = MouseOrbitImproved.ClampAngle(this.y, this.yMinLimit, this.yMaxLimit);
			}
			Quaternion rotation = Quaternion.Euler(this.y, this.x, 0f);
			this.distance = Mathf.Clamp(this.distance - Input.GetAxis("Mouse ScrollWheel") * 5f, this.distanceMin, this.distanceMax);
			Vector3 point = new Vector3(0f, 0f, -this.distance);
			Vector3 position = rotation * point + this.target.position;
			base.transform.rotation = rotation;
			base.transform.position = position;
		}
	}

	// Token: 0x06000010 RID: 16 RVA: 0x000024C7 File Offset: 0x000006C7
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

	// Token: 0x04000006 RID: 6
	public Transform target;

	// Token: 0x04000007 RID: 7
	public float distance = 5f;

	// Token: 0x04000008 RID: 8
	public float xSpeed = 120f;

	// Token: 0x04000009 RID: 9
	public float ySpeed = 120f;

	// Token: 0x0400000A RID: 10
	public float yMinLimit = -20f;

	// Token: 0x0400000B RID: 11
	public float yMaxLimit = 80f;

	// Token: 0x0400000C RID: 12
	public float distanceMin = 0.5f;

	// Token: 0x0400000D RID: 13
	public float distanceMax = 15f;

	// Token: 0x0400000E RID: 14
	private Rigidbody rigidbody;

	// Token: 0x0400000F RID: 15
	private float x;

	// Token: 0x04000010 RID: 16
	private float y;

	// Token: 0x04000011 RID: 17
	public KeyCode rotationKey = KeyCode.Mouse1;
}
