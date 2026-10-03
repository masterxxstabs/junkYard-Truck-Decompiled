using System;
using UnityEngine;

// Token: 0x0200018D RID: 397
public class GearBox : MonoBehaviour
{
	// Token: 0x060009B0 RID: 2480 RVA: 0x00002188 File Offset: 0x00000388
	private void Awake()
	{
	}

	// Token: 0x060009B1 RID: 2481 RVA: 0x00002188 File Offset: 0x00000388
	private void Update()
	{
	}

	// Token: 0x060009B2 RID: 2482 RVA: 0x00082E08 File Offset: 0x00081008
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
			this.pointerSpeed.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Abs(this.speed) * -0.9f + 117f);
			this.pointerRPM.transform.localRotation = Quaternion.Euler(0f, 0f, -70f * (this.currentPitch - 1.5f));
			return;
		}
		this.pointerRPM.transform.localRotation = Quaternion.Euler(0f, 0f, 118f);
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x00004264 File Offset: 0x00002464
	private float Procents(float value, float procents)
	{
		return value / 100f * procents;
	}

	// Token: 0x060009B4 RID: 2484 RVA: 0x0000426F File Offset: 0x0000246F
	private float ProcentOfValue(float firstValue, float secondValue)
	{
		return (float)((int)(firstValue / (secondValue / 100f)));
	}

	// Token: 0x060009B5 RID: 2485 RVA: 0x00082F4C File Offset: 0x0008114C
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
				if (!this.car1.usingV8)
				{
					if (this.engineScript.canMove)
					{
						this.currentPitch = Mathf.Lerp(this.currentPitch, 1f + this.procentPitch * this.ProcentOfValue(firstValue, secondValue), 0.1f);
					}
					else
					{
						this.currentPitch = Mathf.Lerp(this.currentPitch, 1f + this.procentPitch * this.ProcentOfValue(1f, 1f), 0.1f);
					}
				}
				else if (this.engineScriptV8.canMove)
				{
					this.currentPitch = Mathf.Lerp(this.currentPitch, 1f + this.procentPitch * this.ProcentOfValue(firstValue, secondValue), 0.1f);
				}
				else
				{
					this.currentPitch = Mathf.Lerp(this.currentPitch, 1f + this.procentPitch * this.ProcentOfValue(1f, 1f), 0.1f);
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

	// Token: 0x04001A13 RID: 6675
	public car car1;

	// Token: 0x04001A14 RID: 6676
	public GameObject pointerSpeed;

	// Token: 0x04001A15 RID: 6677
	public GameObject pointerRPM;

	// Token: 0x04001A16 RID: 6678
	public float maxPitch = 2.4f;

	// Token: 0x04001A17 RID: 6679
	public float[] totalSteps;

	// Token: 0x04001A18 RID: 6680
	public float currentPitch = 1f;

	// Token: 0x04001A19 RID: 6681
	public float procentPitch;

	// Token: 0x04001A1A RID: 6682
	private float timeToShift;

	// Token: 0x04001A1B RID: 6683
	public float speed;

	// Token: 0x04001A1C RID: 6684
	public float speedR;

	// Token: 0x04001A1D RID: 6685
	private Vector3 lastPosition = Vector3.zero;

	// Token: 0x04001A1E RID: 6686
	public bool shiftinGear;

	// Token: 0x04001A1F RID: 6687
	private int currentGear = 1;

	// Token: 0x04001A20 RID: 6688
	public engine engineScript;

	// Token: 0x04001A21 RID: 6689
	public enginev8 engineScriptV8;

	// Token: 0x04001A22 RID: 6690
	private Quaternion pointRot;

	// Token: 0x04001A23 RID: 6691
	public AudioControl ac;

	// Token: 0x04001A24 RID: 6692
	public bool stuck;

	// Token: 0x04001A25 RID: 6693
	public WheelCollider wheelColRR;

	// Token: 0x04001A26 RID: 6694
	public Rigidbody rb;

	// Token: 0x04001A27 RID: 6695
	public ControlRef cr;
}
