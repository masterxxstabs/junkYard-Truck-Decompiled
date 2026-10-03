using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000190 RID: 400
public class collisionSound : MonoBehaviour
{
	// Token: 0x060009D8 RID: 2520 RVA: 0x00086FA8 File Offset: 0x000851A8
	private void Start()
	{
		this.colAudio = base.gameObject.AddComponent<AudioSource>();
		this.colAudio.loop = false;
		this.colAudio.clip = this.middleSound[Random.Range(0, this.middleSound.Length)];
		this.colAudio.volume = 1f;
		this.colAudio.spatialBlend = 1f;
		this.colAudio.maxDistance = 10f;
		this.colAudio.rolloffMode = AudioRolloffMode.Linear;
	}

	// Token: 0x060009D9 RID: 2521 RVA: 0x00087030 File Offset: 0x00085230
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
			if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
			{
				this.ImpactFeedback(this.impactSpeed);
				return;
			}
		}
		else if (this.impactSpeed > this.greatDamage)
		{
			this.colAudio.volume = 1f;
			int num = Random.Range(0, this.greatSound.Length);
			if (num == 4)
			{
				if (Time.time >= this.lastPlayTime + 600f)
				{
					this.lastPlayTime = Time.time;
				}
				else
				{
					num = Random.Range(0, 4);
				}
			}
			this.colAudio.clip = this.greatSound[num];
			this.colAudio.Play();
			if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
			{
				this.ImpactFeedback(this.impactSpeed);
			}
		}
	}

	// Token: 0x060009DA RID: 2522 RVA: 0x000871FC File Offset: 0x000853FC
	private void ImpactFeedback(float impactSpeed)
	{
		int num = Mathf.RoundToInt(impactSpeed / 2f);
		Debug.Log(num);
		if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
		{
			if (Random.Range(1, 3) == 1)
			{
				LogitechGSDK.LogiPlaySideCollisionForce(0, num);
				return;
			}
			LogitechGSDK.LogiPlayFrontalCollisionForce(0, num);
		}
	}

	// Token: 0x060009DB RID: 2523 RVA: 0x0008724C File Offset: 0x0008544C
	private void OnCollisionEnter(Collision collision)
	{
		this.ProcessContact(collision);
		if (collision.gameObject.name == "AIScript" || collision.gameObject.name == "cop8")
		{
			float num = collision.impulse.magnitude / Time.fixedDeltaTime;
			if (collision.gameObject.name == "AIScript")
			{
				if (num > 34000f)
				{
					this.officer.ReceiveCrash();
					return;
				}
			}
			else if (collision.gameObject.name == "cop8" && num > 10000f)
			{
				this.officer.RagdollOn();
			}
		}
	}

	// Token: 0x060009DC RID: 2524 RVA: 0x000872F8 File Offset: 0x000854F8
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

	// Token: 0x04001B40 RID: 6976
	private LogitechGSDK.LogiControllerPropertiesData properties;

	// Token: 0x04001B41 RID: 6977
	public AudioMixer masterMixer;

	// Token: 0x04001B42 RID: 6978
	public AudioClip[] middleSound;

	// Token: 0x04001B43 RID: 6979
	public AudioClip[] greatSound;

	// Token: 0x04001B44 RID: 6980
	private AudioSource colAudio;

	// Token: 0x04001B45 RID: 6981
	public float middleDamage = 20f;

	// Token: 0x04001B46 RID: 6982
	public float greatDamage = 85f;

	// Token: 0x04001B47 RID: 6983
	private float lastTime;

	// Token: 0x04001B48 RID: 6984
	private float interval = 0.1f;

	// Token: 0x04001B49 RID: 6985
	private float intervalRand = 0.4f;

	// Token: 0x04001B4A RID: 6986
	private float impactSpeed;

	// Token: 0x04001B4B RID: 6987
	private float lastPlayTime;

	// Token: 0x04001B4C RID: 6988
	public Officer officer;

	// Token: 0x04001B4D RID: 6989
	private Vector3 sumImpactVelocity;

	// Token: 0x04001B4E RID: 6990
	private Vector3 localImpactVelocity;
}
