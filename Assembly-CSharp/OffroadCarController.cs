using System;
using UnityEngine;

// Token: 0x020000E0 RID: 224
public class OffroadCarController : MonoBehaviour
{
	// Token: 0x060005A2 RID: 1442 RVA: 0x00045D8C File Offset: 0x00043F8C
	private void Start()
	{
		this.m_rigidbody = base.GetComponent<Rigidbody>();
		Offroad_Wheel[] wheels = this.Wheels;
		for (int i = 0; i < wheels.Length; i++)
		{
			if (wheels[i].wheelCollider == null)
			{
				this.SomethingIsWrong("Some of wheels miss wheel collider");
			}
		}
		if (this.m_rigidbody == null)
		{
			this.SomethingIsWrong("Assign rigidbody to the car");
		}
	}

	// Token: 0x060005A3 RID: 1443 RVA: 0x00045DEE File Offset: 0x00043FEE
	private void SomethingIsWrong(string WhatExactly)
	{
		Debug.LogError(WhatExactly);
		base.enabled = false;
	}

	// Token: 0x060005A4 RID: 1444 RVA: 0x00045E00 File Offset: 0x00044000
	private void Update()
	{
		this.SetCenterOfMass();
		foreach (Offroad_Wheel offroad_Wheel in this.Wheels)
		{
			float num = 1f - Mathf.Clamp(this.m_rigidbody.velocity.magnitude / this.MaxSpeed, 0f, 1f);
			offroad_Wheel.wheelCollider.motorTorque = Input.GetAxis("Vertical") * this.MotorPower * num;
			if (offroad_Wheel.Steering)
			{
				offroad_Wheel.wheelCollider.steerAngle = Input.GetAxis("Horizontal") * this.SteeringAngle;
			}
			if ((Mathf.Sign(Input.GetAxis("Vertical")) != Mathf.Sign(offroad_Wheel.wheelCollider.rpm) && Mathf.Abs(offroad_Wheel.wheelCollider.rpm) > 20f && Input.GetAxis("Vertical") != 0f) || Input.GetKey(KeyCode.Space))
			{
				offroad_Wheel.wheelCollider.brakeTorque = this.BrakingTorque;
			}
			else
			{
				offroad_Wheel.wheelCollider.brakeTorque = 0f;
			}
			offroad_Wheel.brakeTorq = offroad_Wheel.wheelCollider.brakeTorque;
		}
	}

	// Token: 0x060005A5 RID: 1445 RVA: 0x00045F2C File Offset: 0x0004412C
	private void SetCenterOfMass()
	{
		if (this.CenterOfMass == null)
		{
			return;
		}
		if (base.transform.InverseTransformPoint(this.CenterOfMass.position) == this.m_rigidbody.centerOfMass)
		{
			return;
		}
		this.m_rigidbody.centerOfMass = base.transform.InverseTransformPoint(this.CenterOfMass.transform.position);
	}

	// Token: 0x04000C1B RID: 3099
	public Offroad_Wheel[] Wheels;

	// Token: 0x04000C1C RID: 3100
	public float MotorPower;

	// Token: 0x04000C1D RID: 3101
	public float SteeringAngle;

	// Token: 0x04000C1E RID: 3102
	public float BrakingTorque;

	// Token: 0x04000C1F RID: 3103
	public float MaxSpeed;

	// Token: 0x04000C20 RID: 3104
	public Transform CenterOfMass;

	// Token: 0x04000C21 RID: 3105
	private Rigidbody m_rigidbody;
}
