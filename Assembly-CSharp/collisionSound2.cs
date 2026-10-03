using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000039 RID: 57
public class collisionSound2 : MonoBehaviour
{
	// Token: 0x06000110 RID: 272 RVA: 0x0000D958 File Offset: 0x0000BB58
	private void Start()
	{
		this.colAudio = base.gameObject.AddComponent<AudioSource>();
		this.colAudio.loop = false;
		this.colAudio.clip = this.middleSound[Random.Range(0, this.middleSound.Length)];
		this.colAudio.volume = 1f;
		this.colAudio.spatialBlend = 0.8f;
	}

	// Token: 0x06000111 RID: 273 RVA: 0x0000D9C4 File Offset: 0x0000BBC4
	private void Update()
	{
		this.colAudio.outputAudioMixerGroup = this.masterMixer.FindMatchingGroups("collisions")[0];
		if (Time.time - this.lastTime >= this.interval)
		{
			this.localImpactVelocity = this.sumImpactVelocity;
			this.sumImpactVelocity = Vector3.zero;
			this.lastTime = Time.time + this.interval * Random.Range(-this.intervalRand, this.intervalRand);
		}
		else
		{
			this.localImpactVelocity = Vector3.zero;
		}
		this.impactSpeed = this.localImpactVelocity.magnitude;
		if (this.impactSpeed > this.middleDamage && this.impactSpeed < this.greatDamage)
		{
			this.colAudio.volume = 0.4f + (this.impactSpeed - this.middleDamage) / (this.greatDamage - this.middleDamage);
			this.colAudio.clip = this.middleSound[Random.Range(0, this.middleSound.Length)];
			this.colAudio.Play();
			return;
		}
		if (this.impactSpeed > this.greatDamage)
		{
			this.colAudio.volume = 1f;
			this.colAudio.clip = this.greatSound[Random.Range(0, this.greatSound.Length)];
			this.colAudio.Play();
		}
	}

	// Token: 0x06000112 RID: 274 RVA: 0x0000DB1E File Offset: 0x0000BD1E
	private void OnCollisionEnter(Collision collision)
	{
		this.ProcessContact(collision);
	}

	// Token: 0x06000113 RID: 275 RVA: 0x0000DB28 File Offset: 0x0000BD28
	private void ProcessContact(Collision col)
	{
		Vector3 vector = Vector3.zero;
		foreach (ContactPoint contactPoint in col.contacts)
		{
			Collider otherCollider = contactPoint.otherCollider;
			Collider thisCollider = contactPoint.thisCollider;
			if ((otherCollider == null || otherCollider.attachedRigidbody != base.GetComponent<Rigidbody>()) && otherCollider.GetType() != typeof(WheelCollider) && thisCollider.GetType() != typeof(WheelCollider))
			{
				vector += col.relativeVelocity;
				this.sumImpactVelocity += base.transform.InverseTransformDirection(vector);
			}
		}
	}

	// Token: 0x0400031D RID: 797
	public AudioMixer masterMixer;

	// Token: 0x0400031E RID: 798
	public AudioClip[] middleSound;

	// Token: 0x0400031F RID: 799
	public AudioClip[] greatSound;

	// Token: 0x04000320 RID: 800
	private AudioSource colAudio;

	// Token: 0x04000321 RID: 801
	public float middleDamage = 20f;

	// Token: 0x04000322 RID: 802
	public float greatDamage = 100f;

	// Token: 0x04000323 RID: 803
	private float lastTime;

	// Token: 0x04000324 RID: 804
	private float interval = 0.1f;

	// Token: 0x04000325 RID: 805
	private float intervalRand = 0.4f;

	// Token: 0x04000326 RID: 806
	private float impactSpeed;

	// Token: 0x04000327 RID: 807
	private Vector3 sumImpactVelocity;

	// Token: 0x04000328 RID: 808
	private Vector3 localImpactVelocity;
}
