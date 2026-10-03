using System;
using UnityEngine;

// Token: 0x020000DE RID: 222
public class OffroadCameraController : MonoBehaviour
{
	// Token: 0x06000597 RID: 1431 RVA: 0x00002188 File Offset: 0x00000388
	private void Start()
	{
	}

	// Token: 0x06000598 RID: 1432 RVA: 0x00045ABC File Offset: 0x00043CBC
	private void Update()
	{
		switch (this.camType)
		{
		case OffroadCameraController.CamType.Free:
			if (this.Target != null)
			{
				this.DoFreeCam(false);
				return;
			}
			break;
		case OffroadCameraController.CamType.Rotating:
			if (this.Target != null)
			{
				this.DoFreeCam(true);
				return;
			}
			break;
		case OffroadCameraController.CamType.Point1:
			if (this.Point1 != null)
			{
				this.DoPointCam(this.Point1);
				return;
			}
			break;
		case OffroadCameraController.CamType.Point2:
			if (this.Point2 != null)
			{
				this.DoPointCam(this.Point2);
				return;
			}
			break;
		case OffroadCameraController.CamType.Point3:
			if (this.Point3 != null)
			{
				this.DoPointCam(this.Point3);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06000599 RID: 1433 RVA: 0x00045B68 File Offset: 0x00043D68
	private void DoFreeCam(bool Rotating)
	{
		if (Input.GetKey(KeyCode.Mouse1))
		{
			this.rotX += Input.GetAxis("Mouse X") * this.MouseSpeed;
			this.rotY -= Input.GetAxis("Mouse Y") * this.MouseSpeed;
		}
		if (Rotating)
		{
			this.rotX += Time.deltaTime * this.RotatingSpeed;
		}
		this.rotY = Mathf.Clamp(this.rotY, 0f, 70f);
		this.DistanceCam -= Input.GetAxis("Mouse ScrollWheel") * this.MouseScrollSpeed;
		this.DistanceCam = Mathf.Clamp(this.DistanceCam, this.MinDistance, this.MaxDistance);
		this.DistanceCam1 = Mathf.Lerp(this.DistanceCam1, this.DistanceCam, 7f * Time.deltaTime);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(this.rotY, this.rotX, 0f), Time.deltaTime * 6f);
		base.transform.position = this.Target.position + base.transform.rotation * new Vector3(0f, 0f, -this.DistanceCam1);
	}

	// Token: 0x0600059A RID: 1434 RVA: 0x00045CCE File Offset: 0x00043ECE
	private void DoPointCam(Transform Point)
	{
		base.transform.position = Point.position;
		base.transform.LookAt(this.Target);
	}

	// Token: 0x0600059B RID: 1435 RVA: 0x00045CF2 File Offset: 0x00043EF2
	public void SetFreeCamera()
	{
		this.camType = OffroadCameraController.CamType.Free;
	}

	// Token: 0x0600059C RID: 1436 RVA: 0x00045CFB File Offset: 0x00043EFB
	public void SetRotatingCamera()
	{
		this.camType = OffroadCameraController.CamType.Rotating;
	}

	// Token: 0x0600059D RID: 1437 RVA: 0x00045D04 File Offset: 0x00043F04
	public void SetPoint1Camera()
	{
		this.camType = OffroadCameraController.CamType.Point1;
	}

	// Token: 0x0600059E RID: 1438 RVA: 0x00045D0D File Offset: 0x00043F0D
	public void SetPoint2Camera()
	{
		this.camType = OffroadCameraController.CamType.Point2;
	}

	// Token: 0x0600059F RID: 1439 RVA: 0x00045D16 File Offset: 0x00043F16
	public void SetPoint3Camera()
	{
		this.camType = OffroadCameraController.CamType.Point3;
	}

	// Token: 0x04000C0A RID: 3082
	public OffroadCameraController.CamType camType;

	// Token: 0x04000C0B RID: 3083
	public Transform Target;

	// Token: 0x04000C0C RID: 3084
	public Transform Point1;

	// Token: 0x04000C0D RID: 3085
	public Transform Point2;

	// Token: 0x04000C0E RID: 3086
	public Transform Point3;

	// Token: 0x04000C0F RID: 3087
	private float DistanceCam1;

	// Token: 0x04000C10 RID: 3088
	private float DistanceCam = 5f;

	// Token: 0x04000C11 RID: 3089
	private float rotX = 45f;

	// Token: 0x04000C12 RID: 3090
	private float rotY = 30f;

	// Token: 0x04000C13 RID: 3091
	public float MouseSpeed = 4f;

	// Token: 0x04000C14 RID: 3092
	public float MouseScrollSpeed = 2f;

	// Token: 0x04000C15 RID: 3093
	public float MinDistance = 3f;

	// Token: 0x04000C16 RID: 3094
	public float MaxDistance = 5f;

	// Token: 0x04000C17 RID: 3095
	public float RotatingSpeed = 10f;

	// Token: 0x020003DA RID: 986
	public enum CamType
	{
		// Token: 0x0400283E RID: 10302
		Free,
		// Token: 0x0400283F RID: 10303
		Rotating,
		// Token: 0x04002840 RID: 10304
		Point1,
		// Token: 0x04002841 RID: 10305
		Point2,
		// Token: 0x04002842 RID: 10306
		Point3
	}
}
