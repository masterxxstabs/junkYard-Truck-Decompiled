using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x0200018C RID: 396
public class AudioControlCar : MonoBehaviour
{
	// Token: 0x060009A8 RID: 2472 RVA: 0x000822BC File Offset: 0x000804BC
	private void Start()
	{
		this.aSources = base.GetComponents<AudioSource>();
		this.rb = this.car1.GetComponent<Rigidbody>();
		for (int i = 0; i < this.aSources.Length; i++)
		{
			if (!this.car1.userControlled)
			{
				this.aSources[i].Stop();
			}
		}
	}

	// Token: 0x060009A9 RID: 2473 RVA: 0x00082314 File Offset: 0x00080514
	private void FixedUpdate()
	{
		this.skidPlay = false;
		if (Time.time >= (float)this.engineCheckInterval)
		{
			if (this.idScript.AverageStructuralDamage > 0.1f)
			{
				this.engingBroken = true;
			}
			else
			{
				this.engingBroken = false;
				this.aSources[34].volume = 0f;
			}
			this.engineCheckInterval = Mathf.FloorToInt(Time.time) + 10;
		}
		if (!this.car1.userControlled && this.car1.temperature > 179f && !this.aSources[15].isPlaying)
		{
			this.aSources[15].Play();
		}
		if (this.car1.userControlled)
		{
			this.procentPitch = this.Procents(this.maxPitch - 1f, 1f);
			if (this.car1.userControlled)
			{
				this.aSources[2].pitch = 1f + this.procentPitch * this.ProcentOfValue(this.GB.speed, 50f);
			}
			if (this.car1.userControlled)
			{
				if (this.car1.deepWater == 1)
				{
					this.aSources[16].volume = this.GB.speed / 20f;
				}
				else
				{
					this.aSources[16].volume -= 0.1f;
				}
				if (this.surfaceType == 1f)
				{
					this.aSources[2].volume = this.GB.speed / 150f;
				}
				else
				{
					this.aSources[2].volume = this.GB.speed / 10f;
					Vector3 velocity = this.rb.velocity;
					Vector3 vector = base.transform.InverseTransformDirection(velocity);
					if (this.cr.Vert < 0f && vector.z < 0f)
					{
						this.aSources[27].volume = this.GB.speed / 10f;
						this.aSources[27].pitch = 1f + this.procentPitch * this.ProcentOfValue(this.GB.speed, 70f);
					}
					else
					{
						this.aSources[27].volume = 0f;
					}
				}
			}
			this.aSources[13].volume = this.GB.speed / 40f;
			if (!this.car1.usingI6)
			{
				this.aSources[0].pitch = this.GB.currentPitch;
				this.aSources[44].volume = 0f;
			}
			else
			{
				this.aSources[42].pitch = this.GB.currentPitch;
				this.aSources[43].pitch = this.GB.currentPitch / 1.5f;
				this.aSources[44].pitch = this.GB.currentPitch / 3.5f;
				this.aSources[44].volume = this.GB.currentPitch / 10f;
			}
			if (!this.car1.usingI6)
			{
				if (!this.GB.shiftinGear && this.aSources[0].volume < this.GB.currentPitch)
				{
					this.aSources[0].volume += 0.05f;
					this.aSources[42].volume += 0f;
				}
			}
			else
			{
				if (!this.GB.shiftinGear && this.aSources[42].volume < this.GB.currentPitch)
				{
					this.aSources[42].volume += 0.05f;
					this.aSources[0].volume += 0f;
				}
				this.aSources[44].pitch = this.GB.currentPitch / 3.5f;
				this.aSources[44].volume = this.GB.currentPitch / 10f;
			}
			if (this.car1.usingI6)
			{
				if (!this.GB.shiftinGear && this.aSources[42].volume < this.GB.currentPitch && (double)this.GB.currentPitch < 1.7)
				{
					this.aSources[42].volume += 0.05f;
					this.aSources[43].volume -= 0.05f;
				}
				if (this.GB.shiftinGear && (double)this.aSources[42].volume > 0.2)
				{
					this.aSources[42].volume -= 0.05f;
				}
				if (!this.GB.shiftinGear && this.aSources[43].volume < this.GB.currentPitch && (double)this.GB.currentPitch > 1.69)
				{
					this.aSources[42].volume += 0.1f;
					this.aSources[43].volume -= 0.05f;
				}
				if (this.GB.shiftinGear && (double)this.aSources[43].volume > 0.2)
				{
					this.aSources[43].volume -= 0.05f;
				}
			}
			bool flag = this.engingBroken;
			if (!this.car1.usingI6)
			{
				if (this.GB.shiftinGear && (double)this.aSources[0].volume > 0.2)
				{
					this.aSources[0].volume -= 0.05f;
					this.aSources[42].volume -= 0f;
				}
			}
			else if (this.GB.shiftinGear && (double)this.aSources[42].volume > 0.2)
			{
				this.aSources[42].volume -= 0.05f;
				this.aSources[0].volume -= 0f;
			}
			for (int i = 0; i < this.SM.Length; i++)
			{
				if (this.SM[i].skid && this.car1.userControlled)
				{
					this.skidPlay = true;
					break;
				}
			}
			if (this.skidPlay)
			{
				if (!this.aSources[1].isPlaying)
				{
					this.aSources[1].Play();
				}
				if (this.surfaceType == 1f)
				{
					this.aSources[17].volume = this.GB.currentPitch / 2f;
					this.aSources[17].pitch = this.GB.currentPitch;
					if (!this.aSources[17].isPlaying)
					{
						this.aSources[17].Play();
					}
				}
			}
			else
			{
				this.aSources[1].Stop();
			}
			for (int j = 0; j < this.SM.Length; j++)
			{
				if (this.SM[j].bump)
				{
					this.ranNum = Random.Range(3, 13);
					this.ranNum2 = Random.Range(18, 22);
					this.ranNum3 = Random.Range(1, 10);
					this.ranNum5 = Random.Range(28, 34);
					this.ranNum6 = Random.Range(34, 41);
					this.fiveOrSix = Random.Range(1, 3);
					this.aSources[this.ranNum].volume = this.SM[j].bumpVolume * 1.6f;
					this.aSources[this.ranNum].pitch = this.SM[j].bumpPitch;
					if (!this.aSources[this.ranNum].isPlaying)
					{
						this.aSources[this.ranNum].Play();
					}
					if (this.fiveOrSix == 1)
					{
						this.aSources[this.ranNum5].volume = this.SM[j].bumpVolume * 1.6f;
						this.aSources[this.ranNum5].pitch = this.SM[j].bumpPitch;
						if (!this.aSources[this.ranNum5].isPlaying)
						{
							this.aSources[this.ranNum5].Play();
						}
					}
					else
					{
						this.aSources[this.ranNum6].volume = this.SM[j].bumpVolume * 1.6f;
						this.aSources[this.ranNum6].pitch = this.SM[j].bumpPitch;
						if (!this.aSources[this.ranNum6].isPlaying)
						{
							this.aSources[this.ranNum6].Play();
						}
					}
					if (this.ranNum3 == 1)
					{
						this.aSources[this.ranNum2].volume = this.SM[j].bumpVolume / 3f;
						this.aSources[this.ranNum2].Play();
					}
				}
			}
		}
		if (!this.car1.userControlled && this.oldConrolled)
		{
			for (int k = 0; k < this.aSources.Length; k++)
			{
				if (k != 22 && k != 23 && k != 24 && k != 25)
				{
					this.aSources[k].Stop();
				}
			}
		}
		if (this.car1.userControlled && !this.oldConrolled)
		{
			for (int l = 0; l < this.aSources.Length; l++)
			{
				if (l < 4 || l == 13)
				{
					this.aSources[l].Play();
				}
				if (this.car1.usingI6)
				{
					this.aSources[42].Play();
					this.aSources[43].Play();
					this.aSources[44].Play();
				}
			}
		}
		this.oldConrolled = this.car1.userControlled;
	}

	// Token: 0x060009AA RID: 2474 RVA: 0x00082D44 File Offset: 0x00080F44
	public void TurnOff()
	{
		this.ranNum3 = Random.Range(22, 26);
		if (this.ranNum3 == 25)
		{
			this.ranNum3 = Random.Range(22, 26);
		}
		if (!this.car1.usingI6)
		{
			this.aSources[this.ranNum3].Play();
		}
	}

	// Token: 0x060009AB RID: 2475 RVA: 0x00082D98 File Offset: 0x00080F98
	public void Backfire()
	{
		this.ranNum4 = Random.Range(1, 27);
		if (this.ranNum4 == 1 && !this.car1.usingI6)
		{
			this.aSources[26].Play();
		}
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x00082DCC File Offset: 0x00080FCC
	public void EnableRain(bool rainOn)
	{
		if (rainOn)
		{
			this.aSources[41].Play();
			return;
		}
		this.aSources[41].Stop();
	}

	// Token: 0x060009AD RID: 2477 RVA: 0x00004264 File Offset: 0x00002464
	private float Procents(float value, float procents)
	{
		return value / 100f * procents;
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x0000426F File Offset: 0x0000246F
	private float ProcentOfValue(float firstValue, float secondValue)
	{
		return (float)((int)(firstValue / (secondValue / 100f)));
	}

	// Token: 0x040019F5 RID: 6645
	public car3 car1;

	// Token: 0x040019F6 RID: 6646
	public GameObject wheelRay;

	// Token: 0x040019F7 RID: 6647
	public GearBox2 GB;

	// Token: 0x040019F8 RID: 6648
	public skidMarksCar[] SM;

	// Token: 0x040019F9 RID: 6649
	public AudioMixer masterMixer;

	// Token: 0x040019FA RID: 6650
	public float maxPitch = 2.4f;

	// Token: 0x040019FB RID: 6651
	private float maxEngineLevel;

	// Token: 0x040019FC RID: 6652
	private float maxSkidLevel;

	// Token: 0x040019FD RID: 6653
	private float maxRollLevel;

	// Token: 0x040019FE RID: 6654
	private float EngineLevel;

	// Token: 0x040019FF RID: 6655
	private float SkidLevel;

	// Token: 0x04001A00 RID: 6656
	private float RollLevel;

	// Token: 0x04001A01 RID: 6657
	private float enginePitch;

	// Token: 0x04001A02 RID: 6658
	private float procentPitch;

	// Token: 0x04001A03 RID: 6659
	private float surfaceType;

	// Token: 0x04001A04 RID: 6660
	private int ranNum;

	// Token: 0x04001A05 RID: 6661
	private int ranNum2;

	// Token: 0x04001A06 RID: 6662
	private int ranNum3;

	// Token: 0x04001A07 RID: 6663
	private int ranNum4;

	// Token: 0x04001A08 RID: 6664
	private int ranNum5;

	// Token: 0x04001A09 RID: 6665
	private int ranNum6;

	// Token: 0x04001A0A RID: 6666
	private int fiveOrSix;

	// Token: 0x04001A0B RID: 6667
	public AudioSource[] aSources;

	// Token: 0x04001A0C RID: 6668
	private Rigidbody rb;

	// Token: 0x04001A0D RID: 6669
	private bool skidPlay;

	// Token: 0x04001A0E RID: 6670
	private bool oldConrolled;

	// Token: 0x04001A0F RID: 6671
	private int engineCheckInterval = 2;

	// Token: 0x04001A10 RID: 6672
	public ImpactDeformable idScript;

	// Token: 0x04001A11 RID: 6673
	public bool engingBroken;

	// Token: 0x04001A12 RID: 6674
	public ControlRef cr;
}
