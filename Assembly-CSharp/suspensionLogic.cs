using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200018A RID: 394
public class suspensionLogic : MonoBehaviour
{
	// Token: 0x0600099C RID: 2460 RVA: 0x00080D50 File Offset: 0x0007EF50
	private void Start()
	{
		foreach (AxleInfo axleInfo in this.axleInfos)
		{
			axleInfo.SetupDefaultValues();
			axleInfo.SetBrakeTorque(this.maxBrakeTorque);
			this.truck = GameObject.Find("pickup truck standard wheels");
		}
		base.GetComponent<Rigidbody>().centerOfMass = this.centerOfMass.localPosition;
	}

	// Token: 0x0600099D RID: 2461 RVA: 0x00080DD4 File Offset: 0x0007EFD4
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

	// Token: 0x0600099E RID: 2462 RVA: 0x0006C5B4 File Offset: 0x0006A7B4
	private void SetBonePosition(Transform trans, Transform bone)
	{
		bone.position = trans.position;
	}

	// Token: 0x040019BF RID: 6591
	public bool controlled;

	// Token: 0x040019C0 RID: 6592
	public List<AxleInfo> axleInfos;

	// Token: 0x040019C1 RID: 6593
	public Transform centerOfMass;

	// Token: 0x040019C2 RID: 6594
	public float angle;

	// Token: 0x040019C3 RID: 6595
	public float angleChangeSpeed = 2f;

	// Token: 0x040019C4 RID: 6596
	public float maxAngle = 40f;

	// Token: 0x040019C5 RID: 6597
	public Transform jointLeft;

	// Token: 0x040019C6 RID: 6598
	public Transform jointRight;

	// Token: 0x040019C7 RID: 6599
	public Transform boneRightA;

	// Token: 0x040019C8 RID: 6600
	public Transform boneLeftA;

	// Token: 0x040019C9 RID: 6601
	public float maxMotorTorque = 2000f;

	// Token: 0x040019CA RID: 6602
	public float maxBrakeTorque = 2000f;

	// Token: 0x040019CB RID: 6603
	public float maxSpeed = 120f;

	// Token: 0x040019CC RID: 6604
	public GameObject frontShaft;

	// Token: 0x040019CD RID: 6605
	public GameObject rearShaft;

	// Token: 0x040019CE RID: 6606
	private bool handBrake;

	// Token: 0x040019CF RID: 6607
	private float speed;

	// Token: 0x040019D0 RID: 6608
	private float currentTorque;

	// Token: 0x040019D1 RID: 6609
	private GameObject truck;

	// Token: 0x040019D2 RID: 6610
	public GameObject wheelRay;

	// Token: 0x040019D3 RID: 6611
	public GameObject mudBrushRL;

	// Token: 0x040019D4 RID: 6612
	public GameObject mudBrushRR;

	// Token: 0x040019D5 RID: 6613
	public GameObject mudBrushFL;

	// Token: 0x040019D6 RID: 6614
	public GameObject mudBrushFR;

	// Token: 0x040019D7 RID: 6615
	private int surfaceType;
}
