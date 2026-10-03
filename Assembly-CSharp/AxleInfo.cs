using System;
using UnityEngine;

// Token: 0x02000189 RID: 393
[Serializable]
public class AxleInfo
{
	// Token: 0x06000996 RID: 2454 RVA: 0x00080BBD File Offset: 0x0007EDBD
	public void RotateWheels(float angle)
	{
		bool flag = this.steering;
	}

	// Token: 0x06000997 RID: 2455 RVA: 0x00080BC6 File Offset: 0x0007EDC6
	public void SetBrakeTorque(float brakeTorque)
	{
		this.leftWheelCol.brakeTorque = brakeTorque;
		this.rightWheelCol.brakeTorque = brakeTorque;
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x00080BE0 File Offset: 0x0007EDE0
	public void SetupDefaultValues()
	{
		this.rightTireDefPos = this.rightTire.localPosition;
		this.leftTireDefPos = this.leftTire.localPosition;
		this.rightCylinderDefPos = this.rightCylinder.localPosition;
		this.leftCylinderDefPos = this.leftCylinder.localPosition;
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x00080C34 File Offset: 0x0007EE34
	public void CylinderLocalPos()
	{
		Vector3 vector = this.leftTireDefPos - this.leftTire.localPosition;
		this.leftCylinder.localPosition = new Vector3(this.leftCylinderDefPos.x, this.leftCylinderDefPos.y - vector.y, this.leftCylinderDefPos.z);
		vector = this.rightTireDefPos - this.rightTire.localPosition;
		this.rightCylinder.localPosition = new Vector3(this.rightCylinderDefPos.x, this.rightCylinderDefPos.y - vector.y, this.rightCylinderDefPos.z);
	}

	// Token: 0x0600099A RID: 2458 RVA: 0x00080CE0 File Offset: 0x0007EEE0
	public void SetTorque(float torque, float brakeTorque, bool brake)
	{
		if (this.driven > 0f)
		{
			this.leftWheelCol.motorTorque = torque;
			this.rightWheelCol.motorTorque = torque;
		}
		this.leftWheelCol.brakeTorque = 0f;
		this.rightWheelCol.brakeTorque = 0f;
		if (brake)
		{
			this.leftWheelCol.brakeTorque = brakeTorque;
			this.rightWheelCol.brakeTorque = brakeTorque;
		}
	}

	// Token: 0x040019AE RID: 6574
	public WheelCollider leftWheelCol;

	// Token: 0x040019AF RID: 6575
	public WheelCollider rightWheelCol;

	// Token: 0x040019B0 RID: 6576
	public Transform leftTire;

	// Token: 0x040019B1 RID: 6577
	public Transform rightTire;

	// Token: 0x040019B2 RID: 6578
	public Transform leftRotor;

	// Token: 0x040019B3 RID: 6579
	public Transform leftContainer;

	// Token: 0x040019B4 RID: 6580
	public Transform rightContainer;

	// Token: 0x040019B5 RID: 6581
	public Transform leftCylinder;

	// Token: 0x040019B6 RID: 6582
	public Transform rightCylinder;

	// Token: 0x040019B7 RID: 6583
	public Transform leftBone;

	// Token: 0x040019B8 RID: 6584
	public Transform rightBone;

	// Token: 0x040019B9 RID: 6585
	public bool steering;

	// Token: 0x040019BA RID: 6586
	public float driven;

	// Token: 0x040019BB RID: 6587
	private Vector3 rightTireDefPos;

	// Token: 0x040019BC RID: 6588
	private Vector3 leftTireDefPos;

	// Token: 0x040019BD RID: 6589
	private Vector3 rightCylinderDefPos;

	// Token: 0x040019BE RID: 6590
	private Vector3 leftCylinderDefPos;
}
