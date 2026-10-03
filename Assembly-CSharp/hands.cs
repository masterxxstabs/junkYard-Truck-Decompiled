using System;
using UnityEngine;

// Token: 0x02000192 RID: 402
public class hands : MonoBehaviour
{
	// Token: 0x060009E0 RID: 2528 RVA: 0x0008742C File Offset: 0x0008562C
	private void Start()
	{
		this.anim = base.GetComponent<Animator>();
		this.rightHandDefaulRot = this.rightIkTarget.localRotation;
		this.rightHandDefaultPos = this.rightIkTarget.localPosition;
		this.leftHandDefaulRot = this.leftIkTarget.localRotation;
		this.leftHandDefaultPos = this.leftIkTarget.localPosition;
		this.rightHandDefaulRot3 = this.rightIkTarget3.localRotation;
		this.rightHandDefaultPos3 = this.rightIkTarget3.localPosition;
		this.leftHandDefaulRot3 = this.leftIkTarget3.localRotation;
		this.leftHandDefaultPos3 = this.leftIkTarget3.localPosition;
		this.rightHandDefaulRot5 = this.rightIkTarget5.localRotation;
		this.rightHandDefaultPos5 = this.rightIkTarget5.localPosition;
		this.leftHandDefaulRot5 = this.leftIkTarget5.localRotation;
		this.leftHandDefaultPos5 = this.leftIkTarget5.localPosition;
	}

	// Token: 0x060009E1 RID: 2529 RVA: 0x00087514 File Offset: 0x00085714
	private void Update()
	{
		this.steerangle = base.gameObject.GetComponentInParent<car>().steerWheelAngle;
		if (this.steerangle < -340f)
		{
			this.steerangle = -340f;
		}
		if (this.steerangle > 340f)
		{
			this.steerangle = 340f;
		}
		this.rightHandAngle = -this.steerangle - 40f;
		if (this.steerangle < -190f)
		{
			this.rightHandAngle = -this.steerangle - 280f;
		}
		if (this.rightHandAngle < 0f)
		{
			this.rightHandAngle = 0f;
		}
		if (this.rightHandAngle > 60f)
		{
			this.rightHandAngle = 60f;
		}
		this.rightIkTarget.localRotation = this.rightHandDefaulRot * Quaternion.Euler(new Vector3(-this.rightHandAngle, 0f, this.rightHandAngle * 0.5f));
		this.rightIkTarget.localPosition = this.rightHandDefaultPos - new Vector3(this.rightHandAngle * 0.0013f, 0f, 0f);
		if (this.steerangle > 60f)
		{
			this.rightIkTarget.position = Vector3.Lerp(this.rightIkTarget.position, this.rightIkTarget2.position, this.steerangle / 30f - 2f);
			this.rightIkTarget.rotation = Quaternion.Lerp(this.rightIkTarget.rotation, this.rightIkTarget2.rotation, this.steerangle / 30f - 2f);
			this.anim.SetBool("rightOpen", true);
		}
		else
		{
			this.anim.SetBool("rightOpen", false);
		}
		if (this.steerangle > 90f)
		{
			this.rightIkTarget.position = Vector3.Lerp(this.rightIkTarget2.position, this.rightIkTarget3.position, this.steerangle / 30f - 3f);
			this.rightIkTarget.rotation = Quaternion.Lerp(this.rightIkTarget2.rotation, this.rightIkTarget3.rotation, this.steerangle / 30f - 3f);
		}
		if (this.steerangle > 110f)
		{
			this.anim.SetBool("rightOpen", false);
			this.rightIkTarget.localRotation = this.rightIkTarget3.localRotation * Quaternion.Euler(new Vector3(-60f, 0f, 30f));
			this.rightIkTarget.localPosition = this.rightIkTarget3.localPosition - new Vector3(0f, 0f, -0.078f);
		}
		if (this.steerangle < -110f)
		{
			this.anim.SetBool("rightOpen", true);
			this.rightIkTarget.position = Vector3.Lerp(this.rightIkTarget.position, this.rightIkTarget4.position, -this.steerangle / 50f - 2f);
			this.rightIkTarget.rotation = Quaternion.Lerp(this.rightIkTarget.rotation, this.rightIkTarget4.rotation, -this.steerangle / 50f - 2f);
		}
		if (this.steerangle < -150f)
		{
			this.rightIkTarget.position = Vector3.Lerp(this.rightIkTarget.position, this.rightIkTarget5.position, -this.steerangle / 50f - 3f);
			this.rightIkTarget.rotation = Quaternion.Lerp(this.rightIkTarget.rotation, this.rightIkTarget5.rotation, -this.steerangle / 50f - 3f);
		}
		if (this.steerangle < -190f)
		{
			this.anim.SetBool("rightOpen", false);
			this.rightIkTarget5.localRotation = this.rightHandDefaulRot5 * Quaternion.Euler(new Vector3(-this.rightHandAngle, 0f, this.rightHandAngle * 0.5f));
			this.rightIkTarget5.localPosition = this.rightHandDefaultPos5 - new Vector3(0f, 0f, this.rightHandAngle * 0.0016f);
		}
		if (this.steerangle > 200f)
		{
			this.rightHandAngle = this.steerangle - 260f;
			if (this.rightHandAngle > 0f)
			{
				this.rightHandAngle = 0f;
			}
			if (this.rightHandAngle < -60f)
			{
				this.rightHandAngle = -60f;
			}
			this.rightIkTarget.localRotation = this.rightHandDefaulRot3 * Quaternion.Euler(new Vector3(this.rightHandAngle, 0f, this.rightHandAngle * -0.5f));
			this.rightIkTarget.localPosition = this.rightHandDefaultPos3 - new Vector3(0f, 0f, this.rightHandAngle * 0.0013f);
		}
		this.leftHandAngle = this.steerangle - 40f;
		if (this.steerangle > 190f)
		{
			this.leftHandAngle = this.steerangle - 280f;
		}
		if (this.leftHandAngle < 0f)
		{
			this.leftHandAngle = 0f;
		}
		if (this.leftHandAngle > 60f)
		{
			this.leftHandAngle = 60f;
		}
		this.leftIkTarget.localRotation = this.leftHandDefaulRot * Quaternion.Euler(new Vector3(-this.leftHandAngle, 0f, this.leftHandAngle * -0.5f));
		this.leftIkTarget.localPosition = this.leftHandDefaultPos - new Vector3(this.leftHandAngle * -0.0013f, 0f, 0f);
		if (this.steerangle < -60f)
		{
			this.leftIkTarget.position = Vector3.Lerp(this.leftIkTarget.position, this.leftIkTarget2.position, -this.steerangle / 30f - 2f);
			this.leftIkTarget.rotation = Quaternion.Lerp(this.leftIkTarget.rotation, this.leftIkTarget2.rotation, -this.steerangle / 30f - 2f);
			this.anim.SetBool("leftOpen", true);
		}
		else
		{
			this.anim.SetBool("leftOpen", false);
		}
		if (this.steerangle < -90f)
		{
			this.leftIkTarget.position = Vector3.Lerp(this.leftIkTarget2.position, this.leftIkTarget3.position, -this.steerangle / 30f - 3f);
			this.leftIkTarget.rotation = Quaternion.Lerp(this.leftIkTarget2.rotation, this.leftIkTarget3.rotation, -this.steerangle / 30f - 3f);
		}
		if (this.steerangle < -110f)
		{
			this.anim.SetBool("leftOpen", false);
			this.leftIkTarget.localRotation = this.leftIkTarget3.localRotation * Quaternion.Euler(new Vector3(-60f, 0f, -30f));
			this.leftIkTarget.localPosition = this.leftIkTarget3.localPosition - new Vector3(0f, 0f, -0.078f);
		}
		if (this.steerangle > 110f)
		{
			this.anim.SetBool("leftOpen", true);
			this.leftIkTarget.position = Vector3.Lerp(this.leftIkTarget.position, this.leftIkTarget4.position, this.steerangle / 50f - 2f);
			this.leftIkTarget.rotation = Quaternion.Lerp(this.leftIkTarget.rotation, this.leftIkTarget4.rotation, this.steerangle / 50f - 2f);
		}
		if (this.steerangle > 150f)
		{
			this.leftIkTarget.position = Vector3.Lerp(this.leftIkTarget.position, this.leftIkTarget5.position, this.steerangle / 50f - 3f);
			this.leftIkTarget.rotation = Quaternion.Lerp(this.leftIkTarget.rotation, this.leftIkTarget5.rotation, this.steerangle / 50f - 3f);
		}
		if (this.steerangle > 190f)
		{
			this.anim.SetBool("leftOpen", false);
			this.leftIkTarget5.localRotation = this.leftHandDefaulRot5 * Quaternion.Euler(new Vector3(-this.leftHandAngle, 0f, this.leftHandAngle * -0.5f));
			this.leftIkTarget5.localPosition = this.leftHandDefaultPos5 - new Vector3(0f, 0f, this.leftHandAngle * 0.0016f);
		}
		if (this.steerangle < -200f)
		{
			this.leftHandAngle = this.steerangle + 260f;
			if (this.leftHandAngle > 60f)
			{
				this.leftHandAngle = 60f;
			}
			if (this.leftHandAngle < 0f)
			{
				this.leftHandAngle = 0f;
			}
			this.leftIkTarget.localRotation = this.leftHandDefaulRot3 * Quaternion.Euler(new Vector3(-this.leftHandAngle, 0f, this.leftHandAngle * -0.5f));
			this.leftIkTarget.localPosition = this.leftHandDefaultPos3 - new Vector3(0f, 0f, this.leftHandAngle * -0.0013f);
		}
	}

	// Token: 0x060009E2 RID: 2530 RVA: 0x00002188 File Offset: 0x00000388
	private void FixedUpdate()
	{
	}

	// Token: 0x060009E3 RID: 2531 RVA: 0x00087EE8 File Offset: 0x000860E8
	private void OnAnimatorIK()
	{
		this.anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, this.ikWeightLeft1);
		this.anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, this.ikWeightRight1);
		this.anim.SetIKHintPosition(AvatarIKHint.LeftElbow, this.hintLeft.position);
		this.anim.SetIKHintPosition(AvatarIKHint.RightElbow, this.hintRight.position);
		this.anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, this.ikWeightLeft1);
		this.anim.SetIKPositionWeight(AvatarIKGoal.RightHand, this.ikWeightRight1);
		this.anim.SetIKPosition(AvatarIKGoal.LeftHand, this.leftIkTarget.position);
		this.anim.SetIKPosition(AvatarIKGoal.RightHand, this.rightIkTarget.position);
		this.anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, this.ikWeightLeft1);
		this.anim.SetIKRotationWeight(AvatarIKGoal.RightHand, this.ikWeightRight1);
		this.anim.SetIKRotation(AvatarIKGoal.LeftHand, this.leftIkTarget.rotation);
		this.anim.SetIKRotation(AvatarIKGoal.RightHand, this.rightIkTarget.rotation);
	}

	// Token: 0x04001B51 RID: 6993
	private Animator anim;

	// Token: 0x04001B52 RID: 6994
	private float ikWeightLeft1 = 1f;

	// Token: 0x04001B53 RID: 6995
	private float ikWeightRight1 = 1f;

	// Token: 0x04001B54 RID: 6996
	public Transform leftIkTarget;

	// Token: 0x04001B55 RID: 6997
	public Transform leftIkTarget2;

	// Token: 0x04001B56 RID: 6998
	public Transform leftIkTarget3;

	// Token: 0x04001B57 RID: 6999
	public Transform leftIkTarget4;

	// Token: 0x04001B58 RID: 7000
	public Transform leftIkTarget5;

	// Token: 0x04001B59 RID: 7001
	public Transform rightIkTarget;

	// Token: 0x04001B5A RID: 7002
	public Transform rightIkTarget2;

	// Token: 0x04001B5B RID: 7003
	public Transform rightIkTarget3;

	// Token: 0x04001B5C RID: 7004
	public Transform rightIkTarget4;

	// Token: 0x04001B5D RID: 7005
	public Transform rightIkTarget5;

	// Token: 0x04001B5E RID: 7006
	public Transform hintLeft;

	// Token: 0x04001B5F RID: 7007
	public Transform hintRight;

	// Token: 0x04001B60 RID: 7008
	private float steerangle;

	// Token: 0x04001B61 RID: 7009
	private float rightHandAngle;

	// Token: 0x04001B62 RID: 7010
	private float leftHandAngle;

	// Token: 0x04001B63 RID: 7011
	private float test;

	// Token: 0x04001B64 RID: 7012
	private Quaternion rightHandDefaulRot;

	// Token: 0x04001B65 RID: 7013
	private Vector3 rightHandDefaultPos;

	// Token: 0x04001B66 RID: 7014
	private Quaternion leftHandDefaulRot;

	// Token: 0x04001B67 RID: 7015
	private Vector3 leftHandDefaultPos;

	// Token: 0x04001B68 RID: 7016
	private Quaternion rightHandDefaulRot3;

	// Token: 0x04001B69 RID: 7017
	private Vector3 rightHandDefaultPos3;

	// Token: 0x04001B6A RID: 7018
	private Quaternion leftHandDefaulRot3;

	// Token: 0x04001B6B RID: 7019
	private Vector3 leftHandDefaultPos3;

	// Token: 0x04001B6C RID: 7020
	private Quaternion rightHandDefaulRot5;

	// Token: 0x04001B6D RID: 7021
	private Vector3 rightHandDefaultPos5;

	// Token: 0x04001B6E RID: 7022
	private Quaternion leftHandDefaulRot5;

	// Token: 0x04001B6F RID: 7023
	private Vector3 leftHandDefaultPos5;
}
