using System;
using UnityEngine;

// Token: 0x02000037 RID: 55
public class GearBox2 : MonoBehaviour
{
	// Token: 0x060000F3 RID: 243 RVA: 0x00002188 File Offset: 0x00000388
	private void Awake()
	{
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x0000C4A2 File Offset: 0x0000A6A2
	private void Update()
	{
		bool userControlled = this.car1.userControlled;
	}

	// Token: 0x060000F5 RID: 245 RVA: 0x0000C4B0 File Offset: 0x0000A6B0
	private void FixedUpdate()
	{
		this.speed = this.rb.velocity.magnitude * 3.6f;
		if (this.car1.userControlled)
		{
			this.procentPitch = this.Procents(this.maxPitch - 1f, 1f);
			this.Transmission();
			this.pointerSpeed.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Abs(this.speed) * -0.9f + 117f);
		}
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x00004264 File Offset: 0x00002464
	private float Procents(float value, float procents)
	{
		return value / 100f * procents;
	}

	// Token: 0x060000F7 RID: 247 RVA: 0x0000426F File Offset: 0x0000246F
	private float ProcentOfValue(float firstValue, float secondValue)
	{
		return (float)((int)(firstValue / (secondValue / 100f)));
	}

	// Token: 0x060000F8 RID: 248 RVA: 0x0000C544 File Offset: 0x0000A744
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
				this.currentPitch = Mathf.Lerp(this.currentPitch, 1f + this.procentPitch * this.ProcentOfValue(firstValue, secondValue), 0.1f);
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
				this.audioctrl.Backfire();
				this.shiftinGear = false;
				this.currentGear++;
			}
		}
		if (this.currentPitch < 1f)
		{
			this.currentPitch = 1f;
		}
	}

	// Token: 0x0400028E RID: 654
	public car3 car1;

	// Token: 0x0400028F RID: 655
	public GameObject pointerSpeed;

	// Token: 0x04000290 RID: 656
	public float maxPitch = 1.3f;

	// Token: 0x04000291 RID: 657
	public float[] totalSteps;

	// Token: 0x04000292 RID: 658
	public float currentPitch = 1f;

	// Token: 0x04000293 RID: 659
	public float procentPitch;

	// Token: 0x04000294 RID: 660
	private float timeToShift;

	// Token: 0x04000295 RID: 661
	public float speed;

	// Token: 0x04000296 RID: 662
	private Vector3 lastPosition = Vector3.zero;

	// Token: 0x04000297 RID: 663
	public bool shiftinGear;

	// Token: 0x04000298 RID: 664
	private int currentGear = 1;

	// Token: 0x04000299 RID: 665
	private Quaternion pointRot;

	// Token: 0x0400029A RID: 666
	public AudioControlCar audioctrl;

	// Token: 0x0400029B RID: 667
	public WheelCollider wheelColRR;

	// Token: 0x0400029C RID: 668
	public Rigidbody rb;

	// Token: 0x0400029D RID: 669
	public ControlRef cr;
}
