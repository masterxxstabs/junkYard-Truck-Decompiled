using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200014C RID: 332
public class BasicCarController : MonoBehaviour
{
	// Token: 0x06000866 RID: 2150 RVA: 0x0006E370 File Offset: 0x0006C570
	private void Start()
	{
		this.rigid = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000867 RID: 2151 RVA: 0x0006E380 File Offset: 0x0006C580
	private void FixedUpdate()
	{
		float motorTorque = this.torque * Input.GetAxis("Vertical");
		float steerAngle = 30f * Input.GetAxis("Horizontal");
		float brakeTorque = this.torque * Mathf.Abs(Input.GetAxis("Jump"));
		for (int i = 0; i < this.wheels.Count; i++)
		{
			if (this.wheels[i].drive)
			{
				this.wheels[i].wheelCollider.motorTorque = motorTorque;
			}
			if (this.wheels[i].steer)
			{
				this.wheels[i].wheelCollider.steerAngle = steerAngle;
			}
			else
			{
				this.wheels[i].wheelCollider.brakeTorque = brakeTorque;
			}
			this.wheels[i].grounded = this.wheels[i].wheelCollider.GetGroundHit(out this.wheels[i].hit);
			if (this.wheels[i].grounded)
			{
				this.wheels[i].travel = 1f - Mathf.Clamp01((-this.wheels[i].wheelCollider.transform.InverseTransformPoint(this.wheels[i].hit.point).y - this.wheels[i].wheelCollider.radius) / this.wheels[i].wheelCollider.suspensionDistance);
				float num = this.antiRollForce * (1f - this.wheels[i].travel);
				this.rigid.AddForceAtPosition(this.wheels[i].wheelCollider.transform.up * -num, this.wheels[i].wheelCollider.transform.position, ForceMode.Force);
			}
		}
	}

	// Token: 0x06000868 RID: 2152 RVA: 0x0006E58C File Offset: 0x0006C78C
	private void Update()
	{
		for (int i = 0; i < this.wheels.Count; i++)
		{
			this.wheels[i].wheelCollider.GetWorldPose(out this.wheels[i].pos, out this.wheels[i].rot);
			this.wheels[i].wheelMesh.position = this.wheels[i].pos;
			this.wheels[i].wheelMesh.rotation = this.wheels[i].rot;
		}
	}

	// Token: 0x04001383 RID: 4995
	public float torque = 1500f;

	// Token: 0x04001384 RID: 4996
	public float antiRollForce = 5000f;

	// Token: 0x04001385 RID: 4997
	public List<BasicCarController.Wheel> wheels;

	// Token: 0x04001386 RID: 4998
	private Rigidbody rigid;

	// Token: 0x02000424 RID: 1060
	[Serializable]
	public class Wheel
	{
		// Token: 0x0400294E RID: 10574
		public WheelCollider wheelCollider;

		// Token: 0x0400294F RID: 10575
		public Transform wheelMesh;

		// Token: 0x04002950 RID: 10576
		public bool drive;

		// Token: 0x04002951 RID: 10577
		public bool steer;

		// Token: 0x04002952 RID: 10578
		[HideInInspector]
		public Vector3 pos;

		// Token: 0x04002953 RID: 10579
		[HideInInspector]
		public Quaternion rot;

		// Token: 0x04002954 RID: 10580
		[HideInInspector]
		public WheelHit hit;

		// Token: 0x04002955 RID: 10581
		[HideInInspector]
		public bool grounded;

		// Token: 0x04002956 RID: 10582
		[HideInInspector]
		public float travel;
	}
}
