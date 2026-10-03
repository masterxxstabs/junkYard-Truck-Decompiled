using System;
using UnityEngine;

// Token: 0x0200011A RID: 282
public class PoliceVehicle : MonoBehaviour
{
	// Token: 0x06000783 RID: 1923 RVA: 0x00061ECF File Offset: 0x000600CF
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.lastPosition = base.transform.position;
	}

	// Token: 0x06000784 RID: 1924 RVA: 0x00061EF0 File Offset: 0x000600F0
	private void FixedUpdate()
	{
		if (this.chasing)
		{
			float num = Vector3.Distance(base.transform.position, this.player.position);
			if (!this.isRecovering && Vector3.Distance(base.transform.position, this.lastPosition) < 0.01f)
			{
				this.timeStuck += Time.fixedDeltaTime;
			}
			else
			{
				this.timeStuck = 0f;
			}
			this.lastPosition = base.transform.position;
			if (this.timeStuck > this.stuckTimeThreshold && !this.isRecovering)
			{
				this.isRecovering = true;
				this.recoveryTimer = this.recoveryDuration;
			}
			if (this.isRecovering)
			{
				this.Recover();
				return;
			}
			if (num > this.stoppingDistance)
			{
				this.FollowAndFacePlayer();
				return;
			}
			this.rb.velocity = Vector3.zero;
			this.rb.angularVelocity = Vector3.zero;
		}
	}

	// Token: 0x06000785 RID: 1925 RVA: 0x00061FE4 File Offset: 0x000601E4
	private void FollowAndFacePlayer()
	{
		Vector3 normalized = (this.player.position - base.transform.position).normalized;
		this.rb.velocity = normalized * this.followSpeed;
		Quaternion quaternion = Quaternion.LookRotation(normalized);
		Vector3 a = quaternion.eulerAngles - base.transform.rotation.eulerAngles;
		a.x = Mathf.DeltaAngle(base.transform.rotation.eulerAngles.x, quaternion.eulerAngles.x);
		a.y = Mathf.DeltaAngle(base.transform.rotation.eulerAngles.y, quaternion.eulerAngles.y);
		a.z = Mathf.DeltaAngle(base.transform.rotation.eulerAngles.z, quaternion.eulerAngles.z);
		Vector3 vector = a * 0.017453292f * this.rotationSpeed;
		this.rb.angularVelocity = new Vector3(0f, vector.y, 0f);
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x00062120 File Offset: 0x00060320
	private void Recover()
	{
		this.recoveryTimer -= Time.fixedDeltaTime;
		if (this.recoveryTimer > this.recoveryDuration / 2f)
		{
			Vector3 velocity = -base.transform.forward * this.recoverySpeed;
			this.rb.velocity = velocity;
		}
		else
		{
			Vector3 a = Quaternion.Euler(0f, 30f, 0f) * base.transform.forward;
			this.rb.velocity = a * this.recoverySpeed;
			Vector3 angularVelocity = new Vector3(0f, 0.5235988f * this.recoverySpeed, 0f);
			this.rb.angularVelocity = angularVelocity;
		}
		if (this.recoveryTimer <= 0f)
		{
			this.isRecovering = false;
			this.timeStuck = 0f;
			this.rb.velocity = Vector3.zero;
			this.rb.angularVelocity = Vector3.zero;
		}
	}

	// Token: 0x0400112C RID: 4396
	public Transform player;

	// Token: 0x0400112D RID: 4397
	public float followSpeed;

	// Token: 0x0400112E RID: 4398
	public float rotationSpeed;

	// Token: 0x0400112F RID: 4399
	public float stoppingDistance;

	// Token: 0x04001130 RID: 4400
	public float stuckTimeThreshold = 2f;

	// Token: 0x04001131 RID: 4401
	public float recoverySpeed = 3f;

	// Token: 0x04001132 RID: 4402
	public float recoveryDuration = 1.5f;

	// Token: 0x04001133 RID: 4403
	private Rigidbody rb;

	// Token: 0x04001134 RID: 4404
	public bool chasing;

	// Token: 0x04001135 RID: 4405
	private Vector3 lastPosition;

	// Token: 0x04001136 RID: 4406
	private float timeStuck;

	// Token: 0x04001137 RID: 4407
	private bool isRecovering;

	// Token: 0x04001138 RID: 4408
	private float recoveryTimer;
}
