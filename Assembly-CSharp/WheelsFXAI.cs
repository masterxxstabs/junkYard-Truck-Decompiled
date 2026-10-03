using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200002E RID: 46
public class WheelsFXAI : MonoBehaviour
{
	// Token: 0x1700000B RID: 11
	// (get) Token: 0x060000C1 RID: 193 RVA: 0x00009EE7 File Offset: 0x000080E7
	// (set) Token: 0x060000C2 RID: 194 RVA: 0x00009EEF File Offset: 0x000080EF
	public bool skidding { get; private set; }

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x060000C3 RID: 195 RVA: 0x00009EF8 File Offset: 0x000080F8
	// (set) Token: 0x060000C4 RID: 196 RVA: 0x00009F00 File Offset: 0x00008100
	public bool playingAudio { get; private set; }

	// Token: 0x060000C5 RID: 197 RVA: 0x00009F0C File Offset: 0x0000810C
	private void Start()
	{
		this.skidParticles = base.transform.parent.parent.GetComponentInChildren<ParticleSystem>();
		this.skidTrailPrefab = Resources.Load<Transform>("SkidTrail");
		this.ACAI = base.transform.parent.parent.GetComponent<AnyCarAI>();
		if (this.skidParticles == null)
		{
			Debug.LogWarning(" no particle system found on car to generate smoke particles", base.gameObject);
		}
		else if (!this.ACAI.smokeOn)
		{
			this.skidParticles.Stop();
		}
		this.wheelCollider = base.transform.GetComponent<WheelCollider>();
		if (WheelsFXAI.skidTrailsDetachedParent == null)
		{
			WheelsFXAI.skidTrailsDetachedParent = new GameObject("TemporarySkids").transform;
		}
		this.skidAudio = this.ACAI.skidSound;
		this.skidAudioVolume = this.ACAI.skidVolume;
		if (this.ACAI.skidSource == null)
		{
			this.SetUpSkidAudioSource(this.skidAudio);
		}
		else
		{
			this.skidAudioSource = this.ACAI.skidSource;
		}
		this.playingAudio = false;
		this.suspensionsSound = this.ACAI.suspensionsSound;
		this.suspensionsAudioVolume = this.ACAI.suspensionsVolume;
		if (this.ACAI.suspensionsSource == null)
		{
			this.SetUpSuspensionsAudioSource(this.suspensionsSound);
			return;
		}
		this.suspensionsAudioSource = this.ACAI.suspensionsSource;
	}

	// Token: 0x060000C6 RID: 198 RVA: 0x0000A07C File Offset: 0x0000827C
	private void Update()
	{
		WheelHit wheelHit;
		base.GetComponent<WheelCollider>().GetGroundHit(out wheelHit);
		if (wheelHit.normal != this.wheelHit.normal)
		{
			if (this.gameStarted)
			{
				this.suspensionSoundOn = true;
			}
			else
			{
				this.gameStarted = true;
			}
		}
		else
		{
			this.suspensionSoundOn = false;
		}
		this.wheelHit.normal = wheelHit.normal;
		this.SuspensionsSound();
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x0000A0EC File Offset: 0x000082EC
	public void EmitTyreSmoke()
	{
		this.skidParticles.transform.position = base.transform.position - base.transform.up * this.wheelCollider.radius;
		this.skidParticles.Emit(1);
		if (!this.skidding)
		{
			base.StartCoroutine(this.StartSkidTrail());
		}
	}

	// Token: 0x060000C8 RID: 200 RVA: 0x0000A155 File Offset: 0x00008355
	public IEnumerator StartSkidTrail()
	{
		this.skidTrail = Object.Instantiate<Transform>(this.skidTrailPrefab);
		this.skidding = true;
		while (this.skidTrail == null)
		{
			yield return null;
		}
		this.skidTrail.parent = base.gameObject.transform;
		this.skidTrail.localPosition = -Vector3.up * this.wheelCollider.radius;
		yield break;
	}

	// Token: 0x060000C9 RID: 201 RVA: 0x0000A164 File Offset: 0x00008364
	public void EndSkidTrail()
	{
		if (!this.skidding)
		{
			return;
		}
		this.skidding = false;
		this.skidTrail.parent = WheelsFXAI.skidTrailsDetachedParent;
		Object.Destroy(this.skidTrail.gameObject, 15f);
	}

	// Token: 0x060000CA RID: 202 RVA: 0x0000A19B File Offset: 0x0000839B
	public void PlayAudio()
	{
		this.skidAudioSource.Play();
		this.playingAudio = true;
	}

	// Token: 0x060000CB RID: 203 RVA: 0x0000A1AF File Offset: 0x000083AF
	public void StopAudio()
	{
		this.skidAudioSource.Stop();
		this.playingAudio = false;
	}

	// Token: 0x060000CC RID: 204 RVA: 0x0000A1C3 File Offset: 0x000083C3
	private void SuspensionsSound()
	{
		if (this.suspensionSoundOn && !this.suspensionsAudioSource.isPlaying)
		{
			this.suspensionsAudioSource.PlayOneShot(this.suspensionsSound);
		}
	}

	// Token: 0x060000CD RID: 205 RVA: 0x0000A1EC File Offset: 0x000083EC
	private AudioSource SetUpSkidAudioSource(AudioClip clip)
	{
		if (this.ACAI.skidSource == null)
		{
			this.skidAudioSource = base.transform.parent.gameObject.AddComponent<AudioSource>();
			this.ACAI.skidSource = this.skidAudioSource;
		}
		else
		{
			this.skidAudioSource = this.ACAI.skidSource;
		}
		this.skidAudioSource.clip = clip;
		this.skidAudioSource.volume = this.skidAudioVolume;
		this.skidAudioSource.loop = false;
		this.skidAudioSource.pitch = 1f;
		this.skidAudioSource.playOnAwake = false;
		this.skidAudioSource.minDistance = 5f;
		this.skidAudioSource.reverbZoneMix = 1.5f;
		this.skidAudioSource.maxDistance = 600f;
		this.skidAudioSource.dopplerLevel = 2f;
		return this.skidAudioSource;
	}

	// Token: 0x060000CE RID: 206 RVA: 0x0000A2D8 File Offset: 0x000084D8
	private AudioSource SetUpSuspensionsAudioSource(AudioClip clip)
	{
		if (this.ACAI.suspensionsSource == null)
		{
			this.suspensionsAudioSource = base.transform.parent.gameObject.AddComponent<AudioSource>();
			this.ACAI.suspensionsSource = this.suspensionsAudioSource;
		}
		else
		{
			this.suspensionsAudioSource = this.ACAI.suspensionsSource;
		}
		this.suspensionsAudioSource.clip = clip;
		this.suspensionsAudioSource.volume = this.suspensionsAudioVolume;
		this.suspensionsAudioSource.loop = false;
		this.suspensionsAudioSource.pitch = 1f;
		this.suspensionsAudioSource.playOnAwake = false;
		this.suspensionsAudioSource.minDistance = 5f;
		this.suspensionsAudioSource.reverbZoneMix = 1.5f;
		this.suspensionsAudioSource.maxDistance = 600f;
		this.suspensionsAudioSource.dopplerLevel = 2f;
		return this.suspensionsAudioSource;
	}

	// Token: 0x040001EF RID: 495
	private AudioClip skidAudio;

	// Token: 0x040001F0 RID: 496
	private AudioSource skidAudioSource;

	// Token: 0x040001F1 RID: 497
	private float skidAudioVolume;

	// Token: 0x040001F2 RID: 498
	private AudioClip suspensionsSound;

	// Token: 0x040001F3 RID: 499
	private AudioSource suspensionsAudioSource;

	// Token: 0x040001F4 RID: 500
	private float suspensionsAudioVolume;

	// Token: 0x040001F5 RID: 501
	private WheelHit wheelHit;

	// Token: 0x040001F6 RID: 502
	private bool gameStarted;

	// Token: 0x040001F7 RID: 503
	private bool suspensionSoundOn;

	// Token: 0x040001F8 RID: 504
	private ParticleSystem skidParticles;

	// Token: 0x040001F9 RID: 505
	private static Transform skidTrailsDetachedParent;

	// Token: 0x040001FA RID: 506
	private Transform skidTrail;

	// Token: 0x040001FB RID: 507
	private Transform skidTrailPrefab;

	// Token: 0x040001FC RID: 508
	private WheelCollider wheelCollider;

	// Token: 0x040001FD RID: 509
	private AnyCarAI ACAI;
}
