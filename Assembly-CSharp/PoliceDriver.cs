using System;
using UnityEngine;

// Token: 0x02000118 RID: 280
public class PoliceDriver : MonoBehaviour
{
	// Token: 0x06000768 RID: 1896 RVA: 0x00060FE5 File Offset: 0x0005F1E5
	private void Awake()
	{
		this.carRigidbody = base.GetComponent<Rigidbody>();
		this.aSources = base.GetComponents<AudioSource>();
		this.lastPosition = base.transform.position;
	}

	// Token: 0x06000769 RID: 1897 RVA: 0x00061010 File Offset: 0x0005F210
	private void FixedUpdate()
	{
		if (this.isReversing)
		{
			this.HandleReverse();
			return;
		}
		this.CheckIfStuck();
		if (!this.isTurning && Physics.Raycast(this.rayCastTransform.position, base.transform.forward, this.raycastDistance, this.obstacleLayer))
		{
			Debug.Log("obstacle");
			this.isTurning = true;
			this.turnTimer = this.turnDuration;
		}
		if (this.isTurning)
		{
			this.turnTimer -= Time.deltaTime;
			if (this.turnTimer <= 0f)
			{
				this.isTurning = false;
			}
		}
		WheelHit wheelHit;
		this.FLwc.GetGroundHit(out wheelHit);
		WheelHit wheelHit2;
		this.FRwc.GetGroundHit(out wheelHit2);
		if (wheelHit.point.ToString() == "(0.0, 0.0, 0.0)" && wheelHit2.point.ToString() == "(0.0, 0.0, 0.0)")
		{
			if (!this.aSources[1].isPlaying)
			{
				this.aSources[1].Play();
				if (this.aSources[0].volume > 0f)
				{
					this.aSources[0].volume -= 0.1f;
				}
			}
		}
		else
		{
			if (this.aSources[0].volume < 1f)
			{
				this.aSources[0].volume += 0.1f;
			}
			this.aSources[1].Stop();
		}
		if (this.forwardAmount > 0f)
		{
			this.speed += this.forwardAmount * this.acceleration * Time.deltaTime * this.ai.dot;
			if (this.aSources[0].pitch < 1.3f)
			{
				this.aSources[0].pitch += 0.05f;
			}
			this.WheelMesh[0].transform.Rotate(720f * Time.deltaTime, 0f, 0f);
			this.WheelMesh[1].transform.Rotate(720f * Time.deltaTime, 0f, 0f);
			this.WheelMesh[2].transform.Rotate(720f * Time.deltaTime, 0f, 0f);
			this.WheelMesh[3].transform.Rotate(720f * Time.deltaTime, 0f, 0f);
		}
		if (this.forwardAmount < 0f)
		{
			if (this.aSources[0].pitch > 0f)
			{
				this.aSources[0].pitch -= 0.1f;
			}
			if (this.speed > 0f)
			{
				this.speed += this.forwardAmount * this.brakeSpeed * Time.deltaTime * this.ai.dot;
			}
			else
			{
				this.speed += this.forwardAmount * this.reverseSpeed * Time.deltaTime;
			}
		}
		if (this.forwardAmount == 0f)
		{
			if (this.speed > 0f)
			{
				this.speed -= this.idleSlowdown * Time.deltaTime;
			}
			if (this.speed < 0f)
			{
				this.speed += this.idleSlowdown * Time.deltaTime;
			}
		}
		this.speed = Mathf.Clamp(this.speed, this.speedMin, this.speedMax);
		if (this.isTurning)
		{
			Quaternion rhs = Quaternion.Euler(0f, (this.turnSpeed + 20f) * Time.fixedDeltaTime, 0f);
			base.transform.rotation = base.transform.rotation * rhs;
		}
		Vector3 velocity = this.carRigidbody.velocity;
		velocity.x = base.transform.forward.x * this.speed;
		velocity.z = base.transform.forward.z * this.speed;
		if (!this.isReversing)
		{
			this.carRigidbody.velocity = velocity;
		}
		if (this.speed < 0f)
		{
			this.turnAmount *= -1f;
		}
		if (this.turnAmount > 0f || this.turnAmount < 0f)
		{
			if ((this.turnSpeed > 0f && this.turnAmount < 0f) || (this.turnSpeed < 0f && this.turnAmount > 0f))
			{
				float num = 20f;
				this.turnSpeed = this.turnAmount * num;
			}
			this.turnSpeed += this.turnAmount * this.turnSpeedAcceleration * Time.deltaTime;
		}
		else
		{
			if (this.turnSpeed > 0f)
			{
				this.turnSpeed -= this.turnIdleSlowdown * Time.deltaTime;
			}
			if (this.turnSpeed < 0f)
			{
				this.turnSpeed += this.turnIdleSlowdown * Time.deltaTime;
			}
			if (this.turnSpeed > -1f && this.turnSpeed < 1f)
			{
				this.turnSpeed = 0f;
			}
		}
		float num2 = this.speed / this.speedMax;
		float num3 = Mathf.Clamp(1f - num2, 0.75f, 1f);
		this.turnSpeed = Mathf.Clamp(this.turnSpeed, -this.turnSpeedMax, this.turnSpeedMax);
		float num4 = 1f;
		if (this.aiReverse)
		{
			num4 = -1f;
		}
		this.carRigidbody.angularVelocity = new Vector3(0f, this.turnSpeed * (num3 * num4) * 0.017453292f, 0f);
		if ((base.transform.eulerAngles.x > 50f && base.transform.eulerAngles.x < 310f) || (base.transform.eulerAngles.z > 40f && base.transform.eulerAngles.z < 310f))
		{
			base.transform.eulerAngles = new Vector3(0f, base.transform.eulerAngles.y, 0f);
		}
	}

	// Token: 0x0600076A RID: 1898 RVA: 0x00061674 File Offset: 0x0005F874
	private void CheckIfStuck()
	{
		if (Vector3.Distance(base.transform.position, this.lastPosition) < 0.1f)
		{
			if (!this.isWaiting)
			{
				this.stuckTimer += Time.fixedDeltaTime;
				if (this.stuckTimer > this.stuckCheckInterval)
				{
					this.StartReversing();
				}
			}
		}
		else
		{
			this.stuckTimer = 0f;
		}
		this.lastPosition = base.transform.position;
	}

	// Token: 0x0600076B RID: 1899 RVA: 0x000616EC File Offset: 0x0005F8EC
	private void StartReversing()
	{
		Debug.Log("startreversing");
		this.isReversing = true;
		this.reverseTimer = this.reverseDuration;
		Vector3 a = Quaternion.Euler(0f, this.reverseAngle, 0f) * -base.transform.forward;
		this.carRigidbody.velocity = a * this.reverseSpeed;
	}

	// Token: 0x0600076C RID: 1900 RVA: 0x00061758 File Offset: 0x0005F958
	private void HandleReverse()
	{
		this.reverseTimer -= Time.fixedDeltaTime;
		if (this.reverseTimer <= 0f)
		{
			this.isReversing = false;
			this.stuckTimer = 0f;
		}
	}

	// Token: 0x0600076D RID: 1901 RVA: 0x00002188 File Offset: 0x00000388
	private void OnCollisionEnter(Collision collision)
	{
	}

	// Token: 0x0600076E RID: 1902 RVA: 0x0006178B File Offset: 0x0005F98B
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(this.rayCastTransform.position, base.transform.forward * this.raycastDistance);
	}

	// Token: 0x0600076F RID: 1903 RVA: 0x000617BD File Offset: 0x0005F9BD
	public void SetInputs(float forwardAmount, float turnAmount)
	{
		this.forwardAmount = forwardAmount;
		this.turnAmount = turnAmount;
		if (forwardAmount < 0f)
		{
			this.aiReverse = true;
			return;
		}
		this.aiReverse = false;
	}

	// Token: 0x06000770 RID: 1904 RVA: 0x000617E4 File Offset: 0x0005F9E4
	public void ClearTurnSpeed()
	{
		this.turnSpeed = 0f;
	}

	// Token: 0x06000771 RID: 1905 RVA: 0x000617F1 File Offset: 0x0005F9F1
	public float GetSpeed()
	{
		return this.speed;
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x000617F9 File Offset: 0x0005F9F9
	public void SetSpeedMax(float speedMax)
	{
		this.speedMax = speedMax;
	}

	// Token: 0x06000773 RID: 1907 RVA: 0x00061802 File Offset: 0x0005FA02
	public void SetTurnSpeedMax(float turnSpeedMax)
	{
		this.turnSpeedMax = turnSpeedMax;
	}

	// Token: 0x06000774 RID: 1908 RVA: 0x0006180B File Offset: 0x0005FA0B
	public void SetTurnSpeedAcceleration(float turnSpeedAcceleration)
	{
		this.turnSpeedAcceleration = turnSpeedAcceleration;
	}

	// Token: 0x06000775 RID: 1909 RVA: 0x00061814 File Offset: 0x0005FA14
	public void StopCompletely()
	{
		this.speed = 0f;
		this.turnSpeed = 0f;
	}

	// Token: 0x040010F1 RID: 4337
	private float speed;

	// Token: 0x040010F2 RID: 4338
	public float speedMax = 1f;

	// Token: 0x040010F3 RID: 4339
	private float speedMin = -20f;

	// Token: 0x040010F4 RID: 4340
	private float acceleration = 3f;

	// Token: 0x040010F5 RID: 4341
	private float brakeSpeed = 500f;

	// Token: 0x040010F6 RID: 4342
	private float reverseSpeed = 6f;

	// Token: 0x040010F7 RID: 4343
	private float idleSlowdown = 15f;

	// Token: 0x040010F8 RID: 4344
	private float turnSpeed;

	// Token: 0x040010F9 RID: 4345
	private float turnSpeedMax = 320f;

	// Token: 0x040010FA RID: 4346
	private float turnSpeedAcceleration = 200f;

	// Token: 0x040010FB RID: 4347
	private float turnIdleSlowdown = 500f;

	// Token: 0x040010FC RID: 4348
	private float forwardAmount;

	// Token: 0x040010FD RID: 4349
	private float turnAmount;

	// Token: 0x040010FE RID: 4350
	public WheelCollider FLwc;

	// Token: 0x040010FF RID: 4351
	public WheelCollider FRwc;

	// Token: 0x04001100 RID: 4352
	public GameObject[] WheelMesh;

	// Token: 0x04001101 RID: 4353
	public AudioSource[] aSources;

	// Token: 0x04001102 RID: 4354
	private Rigidbody carRigidbody;

	// Token: 0x04001103 RID: 4355
	public PoliceDriverAI ai;

	// Token: 0x04001104 RID: 4356
	private Vector3 lastPosition;

	// Token: 0x04001105 RID: 4357
	private float stuckTimer;

	// Token: 0x04001106 RID: 4358
	public float stuckCheckInterval = 3f;

	// Token: 0x04001107 RID: 4359
	private bool isReversing;

	// Token: 0x04001108 RID: 4360
	public float reverseDuration = 1f;

	// Token: 0x04001109 RID: 4361
	private float reverseTimer;

	// Token: 0x0400110A RID: 4362
	public float reverseAngle = 45f;

	// Token: 0x0400110B RID: 4363
	private bool aiReverse;

	// Token: 0x0400110C RID: 4364
	private bool isTurning;

	// Token: 0x0400110D RID: 4365
	private float turnTimer;

	// Token: 0x0400110E RID: 4366
	private float turnDuration = 2f;

	// Token: 0x0400110F RID: 4367
	public float raycastDistance = 5f;

	// Token: 0x04001110 RID: 4368
	public LayerMask obstacleLayer;

	// Token: 0x04001111 RID: 4369
	public Transform rayCastTransform;

	// Token: 0x04001112 RID: 4370
	public bool isWaiting;
}
