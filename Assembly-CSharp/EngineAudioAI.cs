using System;
using UnityEngine;

// Token: 0x02000029 RID: 41
public class EngineAudioAI : MonoBehaviour
{
	// Token: 0x060000A4 RID: 164 RVA: 0x000091AC File Offset: 0x000073AC
	private void Start()
	{
		this.ACAI = base.transform.GetComponent<AnyCarAI>();
		this.engineVolume = this.ACAI.engineVolume;
		this.lowAccelClip = this.ACAI.lowAcceleration;
		this.lowDecelClip = this.ACAI.lowDeceleration;
		this.highAccelClip = this.ACAI.highAcceleration;
		this.highDecelClip = this.ACAI.highDeceleration;
		this.highAccelSource = this.SetUpEngineAudioSource(this.highAccelClip);
		this.lowAccelSource = this.SetUpEngineAudioSource(this.lowAccelClip);
		this.lowDecelSource = this.SetUpEngineAudioSource(this.lowDecelClip);
		this.highDecelSource = this.SetUpEngineAudioSource(this.highDecelClip);
		this.highAccelSource.spatialBlend = 1f;
		this.highAccelSource.maxDistance = 20f;
		this.highAccelSource.rolloffMode = AudioRolloffMode.Linear;
		this.lowAccelSource.spatialBlend = 1f;
		this.lowAccelSource.maxDistance = 20f;
		this.lowAccelSource.rolloffMode = AudioRolloffMode.Linear;
		this.lowDecelSource.spatialBlend = 1f;
		this.lowDecelSource.maxDistance = 20f;
		this.lowDecelSource.rolloffMode = AudioRolloffMode.Linear;
		this.highDecelSource.spatialBlend = 1f;
		this.highDecelSource.maxDistance = 20f;
		this.highDecelSource.rolloffMode = AudioRolloffMode.Linear;
		if (this.ACAI.turboON)
		{
			this.turboVolume = this.ACAI.turboVolume;
			this.turboAudioClip = this.ACAI.turboAudioClip;
			this.turboAudioSource = this.SetUpEngineAudioSource(this.turboAudioClip);
			this.turboAudioSource.volume = this.turboVolume;
		}
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x00009369 File Offset: 0x00007569
	private void Update()
	{
		this.PlayEngineSound();
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x00009374 File Offset: 0x00007574
	private void PlayEngineSound()
	{
		float num = EngineAudioAI.SoundLerp(this.lowPitchMin, this.lowPitchMax, this.ACAI.RPM);
		num = Mathf.Min(this.lowPitchMax, num);
		this.lowAccelSource.pitch = num * this.pitchMultiplier;
		this.lowDecelSource.pitch = num * this.pitchMultiplier;
		this.highAccelSource.pitch = num * this.highPitchMultiplier * this.pitchMultiplier;
		this.highDecelSource.pitch = num * this.highPitchMultiplier * this.pitchMultiplier;
		float num2 = Mathf.Abs(this.ACAI.AccelInput);
		float num3 = 1f - num2;
		float num4 = Mathf.InverseLerp(0.2f, 0.8f, this.ACAI.RPM);
		float num5 = 1f - num4;
		num4 = 1f - (1f - num4) * (1f - num4);
		num5 = 1f - (1f - num5) * (1f - num5);
		num2 = 1f - (1f - num2) * (1f - num2);
		num3 = 1f - (1f - num3) * (1f - num3);
		this.lowAccelSource.volume = num5 * num2 * this.engineVolume;
		this.lowDecelSource.volume = num5 * num3 * this.engineVolume;
		this.highAccelSource.volume = num4 * num2 * this.engineVolume;
		this.highDecelSource.volume = num4 * num3 * this.engineVolume;
		if (this.ACAI.turboON)
		{
			this.turboAudioSource.pitch = num * this.highPitchMultiplier * this.pitchMultiplier;
			this.turboAudioSource.volume = num4 * num2 * this.turboVolume + 0.1f;
		}
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x00009538 File Offset: 0x00007738
	private AudioSource SetUpEngineAudioSource(AudioClip clip)
	{
		AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 0f;
		audioSource.loop = true;
		audioSource.time = Random.Range(0f, clip.length);
		audioSource.Play();
		audioSource.minDistance = 5f;
		audioSource.dopplerLevel = 0f;
		return audioSource;
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x00006CBC File Offset: 0x00004EBC
	private static float SoundLerp(float from, float to, float value)
	{
		return (1f - value) * from + value * to;
	}

	// Token: 0x040001B6 RID: 438
	private AudioClip lowAccelClip;

	// Token: 0x040001B7 RID: 439
	private AudioClip lowDecelClip;

	// Token: 0x040001B8 RID: 440
	private AudioClip highAccelClip;

	// Token: 0x040001B9 RID: 441
	private AudioClip highDecelClip;

	// Token: 0x040001BA RID: 442
	private AudioClip nosAudioClip;

	// Token: 0x040001BB RID: 443
	private AudioClip turboAudioClip;

	// Token: 0x040001BC RID: 444
	private AudioSource lowAccelSource;

	// Token: 0x040001BD RID: 445
	private AudioSource lowDecelSource;

	// Token: 0x040001BE RID: 446
	private AudioSource highAccelSource;

	// Token: 0x040001BF RID: 447
	private AudioSource highDecelSource;

	// Token: 0x040001C0 RID: 448
	private AudioSource nosAudioSource;

	// Token: 0x040001C1 RID: 449
	private AudioSource turboAudioSource;

	// Token: 0x040001C2 RID: 450
	private AnyCarAI ACAI;

	// Token: 0x040001C3 RID: 451
	private float engineVolume;

	// Token: 0x040001C4 RID: 452
	private float nosVolume;

	// Token: 0x040001C5 RID: 453
	private float turboVolume;

	// Token: 0x040001C6 RID: 454
	private float lowPitchMin = 1f;

	// Token: 0x040001C7 RID: 455
	private float lowPitchMax = 6f;

	// Token: 0x040001C8 RID: 456
	private float pitchMultiplier = 1f;

	// Token: 0x040001C9 RID: 457
	private float highPitchMultiplier = 0.25f;
}
