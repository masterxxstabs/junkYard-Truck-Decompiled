using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000010 RID: 16
public class AudioControlF : MonoBehaviour
{
	// Token: 0x06000035 RID: 53 RVA: 0x000035DC File Offset: 0x000017DC
	private void Start()
	{
		this.aSources = base.GetComponents<AudioSource>();
		this.rb = this.car1.GetComponent<Rigidbody>();
		for (int i = 0; i < this.aSources.Length; i++)
		{
			if (this.car1.controlled)
			{
				if (i != 15 && i != 14 && i != 12 && i != 3 && i != 23 && i != 35 && i != 36)
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

	// Token: 0x06000036 RID: 54 RVA: 0x00003691 File Offset: 0x00001891
	public void PlaySpecified(int clipNum)
	{
		this.aSources[clipNum].Play();
	}

	// Token: 0x06000037 RID: 55 RVA: 0x000036A0 File Offset: 0x000018A0
	public void BlowOff()
	{
		if ((this.engineScriptV8.hasTurboD || this.engineScriptV8.hasTurboP) && this.car1.usingV8)
		{
			this.aSources[31].Play();
		}
	}

	// Token: 0x06000038 RID: 56 RVA: 0x000036D8 File Offset: 0x000018D8
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
					this.aSources[2].volume = this.GB.speed / 160f;
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
				if (this.engineScriptV8.am_transmissionF_cnd < 25f && this.engineScriptV8.am_transmissionF_cnd > 0f)
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
				if (this.car1.usingV8)
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
				this.aSources[26].volume = 0f;
				this.aSources[25].volume = 0f;
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
					this.aSources[37].volume = this.GB.speed / (float)(20 - this.flatCount);
					this.aSources[37].pitch = this.GB.currentPitch / 2f;
				}
				else
				{
					this.aSources[37].volume = 0f;
				}
			}
			this.aSources[13].volume = this.GB.speed / 130f;
			if (!this.car1.usingV8)
			{
				this.aSources[32].volume = 0f;
				this.aSources[33].volume = 0f;
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
			else
			{
				this.aSources[0].volume = 0f;
				this.aSources[32].pitch = this.GB.currentPitch / 1.5f;
				this.aSources[33].pitch = this.GB.currentPitch / 2.5f;
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
					this.aSources[33].volume += 0.05f;
					this.aSources[32].volume -= 0.01f;
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
				if (l != 14 && l != 15 && l != 17 && l != 18 && l != 19 && l != 20 && l != 21 && l != 31 && l != 3 && l != 10 && l != 11 && l != 12 && l != 35 && l != 36)
				{
					this.aSources[l].Play();
				}
			}
		}
		this.oldConrolled = this.car1.controlled;
	}

	// Token: 0x06000039 RID: 57 RVA: 0x00004228 File Offset: 0x00002428
	public void EnableRain(bool rainOn)
	{
		if (rainOn)
		{
			if (this.truck.activeSelf)
			{
				this.aSources[35].Play();
				return;
			}
		}
		else if (this.truck.activeSelf)
		{
			this.aSources[35].Stop();
		}
	}

	// Token: 0x0600003A RID: 58 RVA: 0x00004264 File Offset: 0x00002464
	private float Procents(float value, float procents)
	{
		return value / 100f * procents;
	}

	// Token: 0x0600003B RID: 59 RVA: 0x0000426F File Offset: 0x0000246F
	private float ProcentOfValue(float firstValue, float secondValue)
	{
		return (float)((int)(firstValue / (secondValue / 100f)));
	}

	// Token: 0x0400008B RID: 139
	public car4 car1;

	// Token: 0x0400008C RID: 140
	public GameObject truck;

	// Token: 0x0400008D RID: 141
	public GameObject wheelRay;

	// Token: 0x0400008E RID: 142
	public GearBox3 GB;

	// Token: 0x0400008F RID: 143
	public skidMarks[] SM;

	// Token: 0x04000090 RID: 144
	public AudioMixer masterMixer;

	// Token: 0x04000091 RID: 145
	public float maxPitch = 2.4f;

	// Token: 0x04000092 RID: 146
	private float maxEngineLevel;

	// Token: 0x04000093 RID: 147
	private float maxSkidLevel;

	// Token: 0x04000094 RID: 148
	private float maxRollLevel;

	// Token: 0x04000095 RID: 149
	private float EngineLevel;

	// Token: 0x04000096 RID: 150
	private float SkidLevel;

	// Token: 0x04000097 RID: 151
	private float RollLevel;

	// Token: 0x04000098 RID: 152
	private float enginePitch;

	// Token: 0x04000099 RID: 153
	private float procentPitch;

	// Token: 0x0400009A RID: 154
	private int ranNum;

	// Token: 0x0400009B RID: 155
	private int ranNum2;

	// Token: 0x0400009C RID: 156
	private int ranNum3;

	// Token: 0x0400009D RID: 157
	public AudioSource[] aSources;

	// Token: 0x0400009E RID: 158
	private bool skidPlay;

	// Token: 0x0400009F RID: 159
	private bool oldConrolled;

	// Token: 0x040000A0 RID: 160
	public GroundDetect groundDetect;

	// Token: 0x040000A1 RID: 161
	public enginev8 engineScriptV8;

	// Token: 0x040000A2 RID: 162
	private int lastSound;

	// Token: 0x040000A3 RID: 163
	public int rotorCount;

	// Token: 0x040000A4 RID: 164
	public int flatCount;

	// Token: 0x040000A5 RID: 165
	private Rigidbody rb;

	// Token: 0x040000A6 RID: 166
	public ControlRef cr;
}
