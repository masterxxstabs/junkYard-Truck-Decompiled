using System;
using UnityEngine;

// Token: 0x02000137 RID: 311
[Serializable]
public class AxleInfo2
{
	// Token: 0x06000810 RID: 2064 RVA: 0x0006C19D File Offset: 0x0006A39D
	public void RotateWheels(float angle)
	{
		bool flag = this.steering;
	}

	// Token: 0x06000811 RID: 2065 RVA: 0x0006C1A6 File Offset: 0x0006A3A6
	public void SetBrakeTorque(float brakeTorque)
	{
		this.leftWheelCol.brakeTorque = brakeTorque;
		this.rightWheelCol.brakeTorque = brakeTorque;
	}

	// Token: 0x06000812 RID: 2066 RVA: 0x0006C1C0 File Offset: 0x0006A3C0
	public void SetupDefaultValues()
	{
		this.rightTireDefPos = this.rightTire.localPosition;
		this.leftTireDefPos = this.leftTire.localPosition;
		this.rightCylinderDefPos = this.rightCylinder.localPosition;
		this.leftCylinderDefPos = this.leftCylinder.localPosition;
	}

	// Token: 0x06000813 RID: 2067 RVA: 0x0006C214 File Offset: 0x0006A414
	public void CylinderLocalPos()
	{
		Vector3 vector = this.leftTireDefPos - this.leftTire.localPosition;
		this.leftCylinder.localPosition = new Vector3(this.leftCylinderDefPos.x, this.leftCylinderDefPos.y - vector.y, this.leftCylinderDefPos.z);
		vector = this.rightTireDefPos - this.rightTire.localPosition;
		this.rightCylinder.localPosition = new Vector3(this.rightCylinderDefPos.x, this.rightCylinderDefPos.y - vector.y, this.rightCylinderDefPos.z);
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x0006C2C0 File Offset: 0x0006A4C0
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

	// Token: 0x040012DF RID: 4831
	public WheelCollider leftWheelCol;

	// Token: 0x040012E0 RID: 4832
	public WheelCollider rightWheelCol;

	// Token: 0x040012E1 RID: 4833
	public Transform leftTire;

	// Token: 0x040012E2 RID: 4834
	public Transform rightTire;

	// Token: 0x040012E3 RID: 4835
	public Transform leftContainer;

	// Token: 0x040012E4 RID: 4836
	public Transform rightContainer;

	// Token: 0x040012E5 RID: 4837
	public Transform leftCylinder;

	// Token: 0x040012E6 RID: 4838
	public Transform rightCylinder;

	// Token: 0x040012E7 RID: 4839
	public Transform leftBone;

	// Token: 0x040012E8 RID: 4840
	public Transform rightBone;

	// Token: 0x040012E9 RID: 4841
	public bool steering;

	// Token: 0x040012EA RID: 4842
	public float driven;

	// Token: 0x040012EB RID: 4843
	private Vector3 rightTireDefPos;

	// Token: 0x040012EC RID: 4844
	private Vector3 leftTireDefPos;

	// Token: 0x040012ED RID: 4845
	private Vector3 rightCylinderDefPos;

	// Token: 0x040012EE RID: 4846
	private Vector3 leftCylinderDefPos;
}
