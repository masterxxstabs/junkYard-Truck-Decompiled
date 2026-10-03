using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000138 RID: 312
public class suspensionLogic2 : MonoBehaviour
{
	// Token: 0x06000816 RID: 2070 RVA: 0x0006C330 File Offset: 0x0006A530
	private void Start()
	{
		foreach (AxleInfo axleInfo in this.axleInfos)
		{
			axleInfo.SetupDefaultValues();
			axleInfo.SetBrakeTorque(this.maxBrakeTorque);
			this.truck = GameObject.Find("car");
		}
		base.GetComponent<Rigidbody>().centerOfMass = this.centerOfMass.localPosition;
	}

	// Token: 0x06000817 RID: 2071 RVA: 0x0006C3B4 File Offset: 0x0006A5B4
	private void FixedUpdate()
	{
		if (this.controlled)
		{
			this.speed = base.GetComponent<Rigidbody>().velocity.magnitude;
			this.currentTorque = this.maxMotorTorque * Input.GetAxis("Vertical");
			if (this.speed > this.maxSpeed)
			{
				this.currentTorque = 0f;
			}
			this.handBrake = (Input.GetKey(KeyCode.Space) || ((int)this.speed == 0 && this.currentTorque == 0f));
			this.angle += Mathf.Lerp(this.angleChangeSpeed, 0f, this.speed / this.maxSpeed) * Input.GetAxis("Horizontal");
			if (Input.GetAxis("Horizontal") == 0f)
			{
				this.angle = Mathf.Lerp(this.angle, 0f, this.speed / this.maxSpeed);
			}
			this.angle = Mathf.Clamp(this.angle, -this.maxAngle, this.maxAngle);
		}
		foreach (AxleInfo axleInfo in this.axleInfos)
		{
			if (this.controlled)
			{
				axleInfo.RotateWheels(this.angle);
				axleInfo.SetTorque(this.currentTorque, this.maxBrakeTorque, this.handBrake);
			}
			Vector3 position;
			Quaternion rotation;
			axleInfo.leftWheelCol.GetWorldPose(out position, out rotation);
			axleInfo.leftTire.position = position;
			axleInfo.leftTire.rotation = rotation;
			axleInfo.rightWheelCol.GetWorldPose(out position, out rotation);
			axleInfo.rightTire.position = position;
			axleInfo.rightTire.rotation = rotation;
			axleInfo.CylinderLocalPos();
		}
		this.SetBonePosition(this.jointLeft, this.boneLeftA);
		this.SetBonePosition(this.jointRight, this.boneRightA);
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x0006C5B4 File Offset: 0x0006A7B4
	private void SetBonePosition(Transform trans, Transform bone)
	{
		bone.position = trans.position;
	}

	// Token: 0x040012EF RID: 4847
	public bool controlled;

	// Token: 0x040012F0 RID: 4848
	public List<AxleInfo> axleInfos;

	// Token: 0x040012F1 RID: 4849
	public Transform centerOfMass;

	// Token: 0x040012F2 RID: 4850
	public float angle;

	// Token: 0x040012F3 RID: 4851
	public float angleChangeSpeed = 2f;

	// Token: 0x040012F4 RID: 4852
	public float maxAngle = 40f;

	// Token: 0x040012F5 RID: 4853
	public Transform jointLeft;

	// Token: 0x040012F6 RID: 4854
	public Transform jointRight;

	// Token: 0x040012F7 RID: 4855
	public Transform boneRightA;

	// Token: 0x040012F8 RID: 4856
	public Transform boneLeftA;

	// Token: 0x040012F9 RID: 4857
	public float maxMotorTorque = 2000f;

	// Token: 0x040012FA RID: 4858
	public float maxBrakeTorque = 2000f;

	// Token: 0x040012FB RID: 4859
	public float maxSpeed = 120f;

	// Token: 0x040012FC RID: 4860
	private bool handBrake;

	// Token: 0x040012FD RID: 4861
	private float speed;

	// Token: 0x040012FE RID: 4862
	private float currentTorque;

	// Token: 0x040012FF RID: 4863
	private GameObject truck;
}
