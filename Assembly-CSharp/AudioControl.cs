using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x0200018B RID: 395
public class AudioControl : MonoBehaviour
{
	// Token: 0x060009A0 RID: 2464 RVA: 0x00081014 File Offset: 0x0007F214
	private void Start()
	{
		this.aSources = base.GetComponents<AudioSource>();
		this.rb = this.car1.GetComponent<Rigidbody>();
		for (int i = 0; i < this.aSources.Length; i++)
		{
			if (this.car1.controlled)
			{
				if (i != 15 && i != 14 && i != 12 && i != 3 && i != 35)
				{
					this.aSources[i].Play();
					if (i == 13)
					{
						this.aSources[i].time = Random.value * this.aSources[i].clip.length;
					}
				}
			}
			else
			{
				this.aSources[i].Stop();
			}
		}
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x000810BC File Offset: 0x0007F2BC
	public void PlaySpecified(int clipNum)
	{
		this.aSources[clipNum].Play();
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x000810CC File Offset: 0x0007F2CC
	public void BlowOff()
	{
		if (this.engineScript.hasTurbo && !this.car1.usingV8)
		{
			this.aSources[31].Play();
		}
		if ((this.engineScriptV8.hasTurboD || this.engineScriptV8.hasTurboP) && this.car1.usingV8)
		{
			this.aSources[31].Play();
		}
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x00081138 File Offset: 0x0007F338
	private void Update()
	{
		this.skidPlay = false;
		if (!this.car1.controlled && this.car1.temperature > 179f && !this.aSources[15].isPlaying)
		{
			this.aSources[15].Play();
		}
		if (this.car1.controlled)
		{
			this.procentPitch = this.Procents(this.maxPitch - 1f, 1f);
			if (this.car1.userControlled)
			{
				if (!this.GB.stuck)
				{
					this.aSources[2].pitch = 1f + this.procentPitch * this.ProcentOfValue(this.GB.speed, 100f);
					this.aSources[2].volume = this.GB.speed / 150f;
				}
				else
				{
					this.aSources[2].volume = 0f;
					this.aSources[2].pitch = 1f;
				}
				Vector3 velocity = this.rb.velocity;
				Vector3 vector = base.transform.InverseTransformDirection(velocity);
				if (this.cr.Vert < 0f && vector.z < 0f)
				{
					if (this.aSources[30].volume < this.GB.speed / 10f && this.aSources[30].volume < 1f)
					{
						this.aSources[30].volume += 0.02f;
						this.aSources[30].pitch = 1f + this.procentPitch * this.ProcentOfValue(this.GB.speed, 70f);
					}
					this.car1.revLightL.enabled = true;
					this.car1.revLightR.enabled = true;
				}
				else
				{
					this.aSources[30].volume = 0f;
					this.car1.revLightL.enabled = false;
					this.car1.revLightR.enabled = false;
				}
				if (this.car1.deepWater == 1)
				{
					this.aSources[16].volume = this.GB.speed / 20f;
				}
				else
				{
					this.aSources[16].volume -= 0.1f;
				}
				if (this.groundDetect.deepMud == 1)
				{
					this.aSources[14].volume = this.GB.speed / 2f;
				}
				else
				{
					this.aSources[14].volume = 0f;
				}
				if (this.groundDetect.leaves == 1)
				{
					if (this.aSources[28].volume < this.GB.speed / 4f)
					{
						this.aSources[28].volume += 0.05f;
					}
					else
					{
						this.aSources[28].volume -= 0.5f;
					}
				}
				else if (this.aSources[28].volume > 0f)
				{
					this.aSources[28].volume -= 0.1f;
				}
				if (this.engineScript.transmission_cnd < 25f && this.engineScript.transmission_cnd > 0f)
				{
					if (this.GB.speed < 1f)
					{
						if (this.aSources[22].volume < 0.3f)
						{
							this.aSources[22].volume += 0.05f;
						}
					}
					else
					{
						this.aSources[22].volume -= 0.1f;
					}
				}
				else
				{
					this.aSources[22].volume = 0f;
				}
				if (!this.car1.usingV8 && !this.car1.usingI6)
				{
					if (this.engineScript.hasTurbo)
					{
						this.aSources[27].pitch = this.GB.currentPitch / 3.5f;
						this.aSources[27].volume = this.GB.currentPitch / 10f;
					}
					else
					{
						this.aSources[27].volume = 0f;
					}
				}
				else if (this.car1.usingI6)
				{
					this.aSources[27].pitch = this.GB.currentPitch / 3.5f;
					this.aSources[27].volume = this.GB.currentPitch / 10f;
				}
				else if (this.car1.usingV8)
				{
					if (this.engineScriptV8.hasTurboD && this.engineScriptV8.hasTurboP)
					{
						this.aSources[27].pitch = this.GB.currentPitch / 3.5f;
						this.aSources[27].volume = this.GB.currentPitch / 10f;
					}
					else
					{
						this.aSources[27].volume = 0f;
					}
				}
				if (!this.engineScript.hasExhaustMan && !this.car1.usingV8)
				{
					this.aSources[26].pitch = 1f + this.GB.currentPitch / 4f;
					if (this.aSources[26].volume < this.GB.speed / 2f + 0.5f)
					{
						this.aSources[26].volume += 0.05f;
					}
					else
					{
						this.aSources[26].volume -= 0.5f;
					}
				}
				else
				{
					this.aSources[26].volume = 0f;
				}
				if (this.engineScript.fanClutchSeized && !this.car1.usingV8)
				{
					this.aSources[25].pitch = this.GB.currentPitch / 2f;
					this.aSources[25].volume = this.GB.speed / 3f;
				}
				else
				{
					this.aSources[25].volume = 0f;
				}
				if (this.rotorCount > 0)
				{
					this.aSources[29].volume = this.GB.speed / (float)(20 - this.rotorCount);
				}
				else
				{
					this.aSources[29].volume = 0f;
				}
				if (this.flatCount > 0)
				{
					this.aSources[36].volume = this.GB.speed / (float)(20 - this.flatCount);
					this.aSources[36].pitch = this.GB.currentPitch / 2f;
				}
				else
				{
					this.aSources[36].volume = 0f;
				}
				if (this.engineScript.beltWhine && !this.car1.usingV8)
				{
					this.aSources[23].pitch = this.GB.currentPitch;
					if (this.aSources[23].pitch > 1.2f)
					{
						this.aSources[23].volume -= 0.2f;
					}
					else if (this.aSources[23].volume < this.GB.speed / 2f)
					{
						this.aSources[23].volume += 0.1f;
					}
					else
					{
						this.aSources[23].volume -= 0.1f;
					}
				}
				else
				{
					this.aSources[23].volume -= 0.2f;
				}
			}
			else
			{
				this.aSources[22].volume = 0f;
			}
			this.aSources[13].volume = this.GB.speed / 40f;
			if (this.car1.additive > 2f)
			{
				if (!this.GB.shiftinGear && this.aSources[39].volume < this.GB.currentPitch)
				{
					this.aSources[39].volume += 0.05f;
				}
				if (this.GB.shiftinGear && (double)this.aSources[39].volume > 0.2)
				{
					this.aSources[0].volume -= 0.05f;
				}
				this.aSources[39].pitch = this.GB.currentPitch;
			}
			else
			{
				this.aSources[39].volume = 0f;
			}
			if (!this.car1.usingV8 && !this.car1.usingI6)
			{
				this.aSources[32].volume = 0f;
				this.aSources[33].volume = 0f;
				this.aSources[37].volume = 0f;
				this.aSources[38].volume = 0f;
				this.aSources[0].pitch = this.GB.currentPitch;
				if (!this.GB.shiftinGear && this.aSources[0].volume < this.GB.currentPitch)
				{
					this.aSources[0].volume += 0.05f;
				}
				if (this.GB.shiftinGear && (double)this.aSources[0].volume > 0.2)
				{
					this.aSources[0].volume -= 0.05f;
				}
			}
			else if (this.car1.usingI6)
			{
				this.aSources[0].volume = 0f;
				this.aSources[32].volume = 0f;
				this.aSources[33].volume = 0f;
				this.aSources[37].pitch = this.GB.currentPitch / 1.5f;
				this.aSources[38].pitch = this.GB.currentPitch / 1.5f;
				if (!this.GB.shiftinGear && this.aSources[37].volume < this.GB.currentPitch && (double)this.GB.currentPitch < 1.7)
				{
					this.aSources[37].volume += 0.05f;
					this.aSources[38].volume -= 0.05f;
				}
				if (this.GB.shiftinGear && (double)this.aSources[37].volume > 0.2)
				{
					this.aSources[37].volume -= 0.05f;
				}
				if (!this.GB.shiftinGear && this.aSources[38].volume < this.GB.currentPitch && (double)this.GB.currentPitch > 1.69)
				{
					this.aSources[38].volume += 0.1f;
					this.aSources[37].volume -= 0.05f;
				}
				if (this.GB.shiftinGear && (double)this.aSources[38].volume > 0.2)
				{
					this.aSources[38].volume -= 0.05f;
				}
			}
			else
			{
				this.aSources[0].volume = 0f;
				this.aSources[37].volume = 0f;
				this.aSources[38].volume = 0f;
				this.aSources[0].volume = 0f;
				this.aSources[32].pitch = this.GB.currentPitch / 1.5f;
				this.aSources[33].pitch = this.GB.currentPitch / 1.5f;
				if (!this.GB.shiftinGear && this.aSources[32].volume < this.GB.currentPitch && (double)this.GB.currentPitch < 1.7)
				{
					this.aSources[32].volume += 0.05f;
					this.aSources[33].volume -= 0.05f;
				}
				if (this.GB.shiftinGear && (double)this.aSources[32].volume > 0.2)
				{
					this.aSources[32].volume -= 0.05f;
				}
				if (!this.GB.shiftinGear && this.aSources[33].volume < this.GB.currentPitch && (double)this.GB.currentPitch > 1.69)
				{
					this.aSources[33].volume += 0.1f;
					this.aSources[32].volume -= 0.05f;
				}
				if (this.GB.shiftinGear && (double)this.aSources[33].volume > 0.2)
				{
					this.aSources[33].volume -= 0.05f;
				}
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
				if (this.groundDetect.deepMud == 1 && !this.car1.clutchIn)
				{
					if (this.aSources[17].volume < this.GB.currentPitch / 2f)
					{
						this.aSources[17].volume += 0.01f;
					}
					if (this.GB.currentPitch < 1.8f && this.aSources[17].pitch < 1.8f)
					{
						this.aSources[17].pitch += 0.01f;
					}
					if (this.aSources[17].pitch > this.GB.currentPitch)
					{
						this.aSources[17].pitch -= 0.01f;
					}
					if (!this.aSources[17].isPlaying)
					{
						this.aSources[17].Play();
					}
				}
				else
				{
					this.aSources[17].volume = 0f;
				}
			}
			else
			{
				this.aSources[1].Stop();
			}
			for (int j = 0; j < this.SM.Length; j++)
			{
				if (this.SM[j].bump && !this.car1.soundMat)
				{
					this.ranNum = Random.Range(3, 13);
					this.ranNum2 = Random.Range(18, 22);
					this.ranNum3 = Random.Range(1, 10);
					this.aSources[this.ranNum].volume = this.SM[j].bumpVolume;
					this.aSources[this.ranNum].pitch = this.SM[j].bumpPitch;
					if (!this.aSources[this.ranNum].isPlaying)
					{
						if (this.ranNum == this.lastSound)
						{
							this.ranNum = Random.Range(3, 13);
						}
						this.aSources[this.ranNum].Play();
						this.lastSound = this.ranNum;
					}
					if (this.ranNum3 == 1)
					{
						this.aSources[this.ranNum2].volume = this.SM[j].bumpVolume / 2f;
						this.aSources[this.ranNum2].Play();
					}
				}
			}
		}
		if (!this.car1.controlled && this.oldConrolled)
		{
			for (int k = 0; k < this.aSources.Length; k++)
			{
				this.aSources[k].Stop();
			}
		}
		if (this.car1.controlled && !this.oldConrolled)
		{
			for (int l = 0; l < this.aSources.Length; l++)
			{
				if (l != 14 && l != 15 && l != 17 && l != 18 && l != 19 && l != 20 && l != 21 && l != 31 && l != 3 && l != 10 && l != 11 && l != 12 && l != 35)
				{
					this.aSources[l].Play();
				}
			}
		}
		this.oldConrolled = this.car1.controlled;
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x0008226D File Offset: 0x0008046D
	public void EnableRain(bool rainOn)
	{
		if (rainOn)
		{
			if (this.truck.activeSelf)
			{
				this.aSources[34].Play();
				return;
			}
		}
		else if (this.truck.activeSelf)
		{
			this.aSources[34].Stop();
		}
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x00004264 File Offset: 0x00002464
	private float Procents(float value, float procents)
	{
		return value / 100f * procents;
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x0000426F File Offset: 0x0000246F
	private float ProcentOfValue(float firstValue, float secondValue)
	{
		return (float)((int)(firstValue / (secondValue / 100f)));
	}

	// Token: 0x040019D8 RID: 6616
	public car car1;

	// Token: 0x040019D9 RID: 6617
	public GameObject truck;

	// Token: 0x040019DA RID: 6618
	public GameObject wheelRay;

	// Token: 0x040019DB RID: 6619
	public GearBox GB;

	// Token: 0x040019DC RID: 6620
	public skidMarks[] SM;

	// Token: 0x040019DD RID: 6621
	public AudioMixer masterMixer;

	// Token: 0x040019DE RID: 6622
	public float maxPitch = 2.4f;

	// Token: 0x040019DF RID: 6623
	private float maxEngineLevel;

	// Token: 0x040019E0 RID: 6624
	private float maxSkidLevel;

	// Token: 0x040019E1 RID: 6625
	private float maxRollLevel;

	// Token: 0x040019E2 RID: 6626
	private float EngineLevel;

	// Token: 0x040019E3 RID: 6627
	private float SkidLevel;

	// Token: 0x040019E4 RID: 6628
	private float RollLevel;

	// Token: 0x040019E5 RID: 6629
	private float enginePitch;

	// Token: 0x040019E6 RID: 6630
	private float procentPitch;

	// Token: 0x040019E7 RID: 6631
	private int ranNum;

	// Token: 0x040019E8 RID: 6632
	private int ranNum2;

	// Token: 0x040019E9 RID: 6633
	private int ranNum3;

	// Token: 0x040019EA RID: 6634
	public AudioSource[] aSources;

	// Token: 0x040019EB RID: 6635
	private bool skidPlay;

	// Token: 0x040019EC RID: 6636
	private bool oldConrolled;

	// Token: 0x040019ED RID: 6637
	public GroundDetect groundDetect;

	// Token: 0x040019EE RID: 6638
	public engine engineScript;

	// Token: 0x040019EF RID: 6639
	public enginev8 engineScriptV8;

	// Token: 0x040019F0 RID: 6640
	private int lastSound;

	// Token: 0x040019F1 RID: 6641
	public int rotorCount;

	// Token: 0x040019F2 RID: 6642
	public int flatCount;

	// Token: 0x040019F3 RID: 6643
	private Rigidbody rb;

	// Token: 0x040019F4 RID: 6644
	public ControlRef cr;
}
