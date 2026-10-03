using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000011 RID: 17
public class AudioControlGolf : MonoBehaviour
{
	// Token: 0x0600003D RID: 61 RVA: 0x00004290 File Offset: 0x00002490
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

	// Token: 0x0600003E RID: 62 RVA: 0x000042E8 File Offset: 0x000024E8
	public void StopSounds()
	{
		for (int i = 0; i < this.aSources.Length; i++)
		{
			this.aSources[i].Stop();
		}
	}

	// Token: 0x0600003F RID: 63 RVA: 0x00004318 File Offset: 0x00002518
	private void FixedUpdate()
	{
		this.skidPlay = false;
		if (this.car1.userControlled && !this.oldConrolled)
		{
			for (int i = 0; i < this.aSources.Length; i++)
			{
				this.aSources[i].Play();
			}
		}
		this.oldConrolled = this.car1.userControlled;
		if (this.car1.userControlled)
		{
			this.procentPitch = this.Procents(this.maxPitch - 1f, 1f);
			if (this.car1.userControlled)
			{
				this.aSources[2].pitch = 1f + this.procentPitch * this.ProcentOfValue(this.car1.speed, 50f);
			}
			if (this.car1.userControlled)
			{
				if (this.car1.deepWater == 1)
				{
					this.aSources[2].volume = this.car1.speed / 20f;
				}
				else
				{
					this.aSources[2].volume -= 0.1f;
				}
				if (this.surfaceType == 1f)
				{
					this.aSources[2].volume = this.car1.speed / 150f;
				}
				else
				{
					this.aSources[2].volume = this.car1.speed / 10f;
				}
			}
			this.currentPitch = this.procentPitch * this.ProcentOfValue(this.car1.speed, 100f);
			if (this.currentPitch < 0.7f)
			{
				this.aSources[0].pitch = this.procentPitch * this.ProcentOfValue(this.car1.speed, 50f);
			}
			this.currentVol = this.car1.speed / 8f;
			if (this.currentVol < 0.3f)
			{
				this.aSources[0].volume = this.car1.speed / 8f;
			}
			for (int j = 0; j < this.SM.Length; j++)
			{
				bool bump = this.SM[j].bump;
			}
		}
	}

	// Token: 0x06000040 RID: 64 RVA: 0x00004264 File Offset: 0x00002464
	private float Procents(float value, float procents)
	{
		return value / 100f * procents;
	}

	// Token: 0x06000041 RID: 65 RVA: 0x0000426F File Offset: 0x0000246F
	private float ProcentOfValue(float firstValue, float secondValue)
	{
		return (float)((int)(firstValue / (secondValue / 100f)));
	}

	// Token: 0x040000A7 RID: 167
	public golfcart car1;

	// Token: 0x040000A8 RID: 168
	public GameObject wheelRay;

	// Token: 0x040000A9 RID: 169
	public skidMarksCar[] SM;

	// Token: 0x040000AA RID: 170
	public float maxPitch = 2.4f;

	// Token: 0x040000AB RID: 171
	public AudioMixer masterMixer;

	// Token: 0x040000AC RID: 172
	private float maxEngineLevel;

	// Token: 0x040000AD RID: 173
	private float maxSkidLevel;

	// Token: 0x040000AE RID: 174
	private float maxRollLevel;

	// Token: 0x040000AF RID: 175
	private float EngineLevel;

	// Token: 0x040000B0 RID: 176
	private float SkidLevel;

	// Token: 0x040000B1 RID: 177
	private float RollLevel;

	// Token: 0x040000B2 RID: 178
	private float enginePitch;

	// Token: 0x040000B3 RID: 179
	private float procentPitch;

	// Token: 0x040000B4 RID: 180
	private float currentPitch;

	// Token: 0x040000B5 RID: 181
	private float surfaceType;

	// Token: 0x040000B6 RID: 182
	private int ranNum;

	// Token: 0x040000B7 RID: 183
	private int ranNum2;

	// Token: 0x040000B8 RID: 184
	private int ranNum3;

	// Token: 0x040000B9 RID: 185
	private int ranNum4;

	// Token: 0x040000BA RID: 186
	private int ranNum5;

	// Token: 0x040000BB RID: 187
	private int ranNum6;

	// Token: 0x040000BC RID: 188
	private int fiveOrSix;

	// Token: 0x040000BD RID: 189
	public AudioSource[] aSources;

	// Token: 0x040000BE RID: 190
	private Rigidbody rb;

	// Token: 0x040000BF RID: 191
	private bool skidPlay;

	// Token: 0x040000C0 RID: 192
	private bool oldConrolled;

	// Token: 0x040000C1 RID: 193
	private float currentVol;
}
