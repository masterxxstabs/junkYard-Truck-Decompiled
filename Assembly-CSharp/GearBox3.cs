using System;
using UnityEngine;

// Token: 0x020000B8 RID: 184
public class GearBox3 : MonoBehaviour
{
	// Token: 0x06000459 RID: 1113 RVA: 0x00002188 File Offset: 0x00000388
	private void Awake()
	{
	}

	// Token: 0x0600045A RID: 1114 RVA: 0x0002EF84 File Offset: 0x0002D184
	private void FixedUpdate()
	{
		this.speed = this.rb.velocity.magnitude * 3.6f;
		this.speedR = this.wheelColRR.rpm / 10f;
		if (this.speedR > 150f)
		{
			this.speedR = 150f;
		}
		if (this.speed < 6f)
		{
			this.speed = this.speedR;
			this.stuck = true;
		}
		else
		{
			this.stuck = false;
		}
		if (this.car1.controlled)
		{
			this.procentPitch = this.Procents(this.maxPitch - 1f, 1f);
			this.Transmission();
			this.pointerSpeed.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Abs(this.speed) * -1.3f + 10f);
		}
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x00004264 File Offset: 0x00002464
	private float Procents(float value, float procents)
	{
		return value / 100f * procents;
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x0000426F File Offset: 0x0000246F
	private float ProcentOfValue(float firstValue, float secondValue)
	{
		return (float)((int)(firstValue / (secondValue / 100f)));
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x0002F070 File Offset: 0x0002D270
	private void Transmission()
	{
		float secondValue = 0f;
		float firstValue = 0f;
		if (this.cr.Vert > 0f || this.cr.GasInput > 0f)
		{
			if (!this.shiftinGear)
			{
				for (int i = 0; i < 5; i++)
				{
					secondValue = this.totalSteps[i + 1] - this.totalSteps[i];
					firstValue = this.speed - this.totalSteps[i];
					if (this.currentGear < 5 && this.speed >= this.totalSteps[this.currentGear + 1] && this.currentGear == i)
					{
						this.shiftinGear = true;
					}
					if (this.speed < this.totalSteps[i + 1])
					{
						break;
					}
				}
				if (this.car1.usingV8)
				{
					if (this.engineScriptV8.canMove)
					{
						this.currentPitch = Mathf.Lerp(this.currentPitch, 1f + this.procentPitch * this.ProcentOfValue(firstValue, secondValue), 0.1f);
					}
					else
					{
						this.currentPitch = Mathf.Lerp(this.currentPitch, 1f + this.procentPitch * this.ProcentOfValue(1f, 1f), 0.1f);
					}
				}
				if (!this.car1.userControlled)
				{
					this.currentPitch = 1f;
				}
				if (this.currentPitch > this.maxPitch)
				{
					this.currentPitch = this.maxPitch;
				}
				this.timeToShift = 0f;
			}
		}
		else
		{
			this.currentPitch = Mathf.Lerp(this.currentPitch, 1f, 0.05f);
			if (!this.car1.userControlled)
			{
				this.currentPitch = 1f;
			}
			for (int j = 0; j < 5; j++)
			{
				this.currentGear = j;
				if (this.speed < this.totalSteps[j + 1])
				{
					break;
				}
			}
		}
		if (this.shiftinGear)
		{
			this.currentPitch = Mathf.Lerp(this.currentPitch, 1f, 0.01f);
			this.timeToShift += Time.deltaTime;
			if ((double)this.timeToShift > 0.5)
			{
				if (Random.Range(1, 10) == 1)
				{
					this.ac.BlowOff();
				}
				this.shiftinGear = false;
				this.currentGear++;
			}
		}
		if (this.currentPitch < 1f)
		{
			this.currentPitch = 1f;
		}
	}

	// Token: 0x04000902 RID: 2306
	public car4 car1;

	// Token: 0x04000903 RID: 2307
	public GameObject pointerSpeed;

	// Token: 0x04000904 RID: 2308
	public float maxPitch = 2.4f;

	// Token: 0x04000905 RID: 2309
	public float[] totalSteps;

	// Token: 0x04000906 RID: 2310
	public float currentPitch = 1f;

	// Token: 0x04000907 RID: 2311
	public float procentPitch;

	// Token: 0x04000908 RID: 2312
	private float timeToShift;

	// Token: 0x04000909 RID: 2313
	public float speed;

	// Token: 0x0400090A RID: 2314
	public float speedR;

	// Token: 0x0400090B RID: 2315
	private Vector3 lastPosition = Vector3.zero;

	// Token: 0x0400090C RID: 2316
	public bool shiftinGear;

	// Token: 0x0400090D RID: 2317
	private int currentGear = 1;

	// Token: 0x0400090E RID: 2318
	public enginev8 engineScriptV8;

	// Token: 0x0400090F RID: 2319
	private Quaternion pointRot;

	// Token: 0x04000910 RID: 2320
	public AudioControlF ac;

	// Token: 0x04000911 RID: 2321
	public bool stuck;

	// Token: 0x04000912 RID: 2322
	public WheelCollider wheelColRR;

	// Token: 0x04000913 RID: 2323
	public Rigidbody rb;

	// Token: 0x04000914 RID: 2324
	public ControlRef cr;
}
