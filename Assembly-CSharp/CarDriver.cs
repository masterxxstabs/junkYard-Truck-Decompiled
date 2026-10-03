using System;
using UnityEngine;

// Token: 0x0200003A RID: 58
public class CarDriver : MonoBehaviour
{
	// Token: 0x06000115 RID: 277 RVA: 0x0000DC1C File Offset: 0x0000BE1C
	private void Awake()
	{
		this.carRigidbody = base.GetComponent<Rigidbody>();
		this.aSources = base.GetComponents<AudioSource>();
	}

	// Token: 0x06000116 RID: 278 RVA: 0x0000DC38 File Offset: 0x0000BE38
	private void FixedUpdate()
	{
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
		Vector3 velocity = this.carRigidbody.velocity;
		velocity.x = base.transform.forward.x * this.speed;
		velocity.z = base.transform.forward.z * this.speed;
		this.carRigidbody.velocity = velocity;
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
		this.carRigidbody.angularVelocity = new Vector3(0f, this.turnSpeed * (num3 * 1f) * 0.017453292f, 0f);
		if ((base.transform.eulerAngles.x > 50f && base.transform.eulerAngles.x < 310f) || (base.transform.eulerAngles.z > 40f && base.transform.eulerAngles.z < 310f))
		{
			base.transform.eulerAngles = new Vector3(0f, base.transform.eulerAngles.y, 0f);
		}
	}

	// Token: 0x06000117 RID: 279 RVA: 0x00002188 File Offset: 0x00000388
	private void OnCollisionEnter(Collision collision)
	{
	}

	// Token: 0x06000118 RID: 280 RVA: 0x0000E1A0 File Offset: 0x0000C3A0
	public void SetInputs(float forwardAmount, float turnAmount)
	{
		this.forwardAmount = forwardAmount;
		this.turnAmount = turnAmount;
	}

	// Token: 0x06000119 RID: 281 RVA: 0x0000E1B0 File Offset: 0x0000C3B0
	public void ClearTurnSpeed()
	{
		this.turnSpeed = 0f;
	}

	// Token: 0x0600011A RID: 282 RVA: 0x0000E1BD File Offset: 0x0000C3BD
	public float GetSpeed()
	{
		return this.speed;
	}

	// Token: 0x0600011B RID: 283 RVA: 0x0000E1C5 File Offset: 0x0000C3C5
	public void SetSpeedMax(float speedMax)
	{
		this.speedMax = speedMax;
	}

	// Token: 0x0600011C RID: 284 RVA: 0x0000E1CE File Offset: 0x0000C3CE
	public void SetTurnSpeedMax(float turnSpeedMax)
	{
		this.turnSpeedMax = turnSpeedMax;
	}

	// Token: 0x0600011D RID: 285 RVA: 0x0000E1D7 File Offset: 0x0000C3D7
	public void SetTurnSpeedAcceleration(float turnSpeedAcceleration)
	{
		this.turnSpeedAcceleration = turnSpeedAcceleration;
	}

	// Token: 0x0600011E RID: 286 RVA: 0x0000E1E0 File Offset: 0x0000C3E0
	public void StopCompletely()
	{
		this.speed = 0f;
		this.turnSpeed = 0f;
	}

	// Token: 0x04000329 RID: 809
	private float speed;

	// Token: 0x0400032A RID: 810
	public float speedMax = 15f;

	// Token: 0x0400032B RID: 811
	private float speedMin = -50f;

	// Token: 0x0400032C RID: 812
	private float acceleration = 30f;

	// Token: 0x0400032D RID: 813
	private float brakeSpeed = 100f;

	// Token: 0x0400032E RID: 814
	private float reverseSpeed = 30f;

	// Token: 0x0400032F RID: 815
	private float idleSlowdown = 10f;

	// Token: 0x04000330 RID: 816
	private float turnSpeed;

	// Token: 0x04000331 RID: 817
	private float turnSpeedMax = 300f;

	// Token: 0x04000332 RID: 818
	private float turnSpeedAcceleration = 300f;

	// Token: 0x04000333 RID: 819
	private float turnIdleSlowdown = 500f;

	// Token: 0x04000334 RID: 820
	private float forwardAmount;

	// Token: 0x04000335 RID: 821
	private float turnAmount;

	// Token: 0x04000336 RID: 822
	public WheelCollider FLwc;

	// Token: 0x04000337 RID: 823
	public WheelCollider FRwc;

	// Token: 0x04000338 RID: 824
	public GameObject[] WheelMesh;

	// Token: 0x04000339 RID: 825
	public AudioSource[] aSources;

	// Token: 0x0400033A RID: 826
	private Rigidbody carRigidbody;

	// Token: 0x0400033B RID: 827
	public CarDriverAI ai;
}
