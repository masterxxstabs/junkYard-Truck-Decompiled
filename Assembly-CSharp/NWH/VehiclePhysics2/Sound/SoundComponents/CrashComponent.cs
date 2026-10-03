using System;
using UnityEngine;
using UnityEngine.Events;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
	// Token: 0x0200026B RID: 619
	[Serializable]
	public class CrashComponent : SoundComponent
	{
		// Token: 0x06001061 RID: 4193 RVA: 0x000BB8B8 File Offset: 0x000B9AB8
		public override void Initialize()
		{
			if (base.Clip != null)
			{
				base.Source = this.container.AddComponent<AudioSource>();
				this.vc.soundManager.SetAudioSourceDefaults(base.Source, false, false, 0f, null);
				base.AddSourcesToMixer();
				base.Source.dopplerLevel = 0f;
			}
			this.vc.damageHandler.OnCollision.AddListener(new UnityAction<Collision>(this.RaiseCollisionFlag));
			this.initialized = true;
		}

		// Token: 0x06001062 RID: 4194 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x06001063 RID: 4195 RVA: 0x000BB940 File Offset: 0x000B9B40
		public override void Update()
		{
			if (!base.Active)
			{
				this.collisionFlag = false;
				return;
			}
			if (this.collisionFlag && !base.Source.isPlaying)
			{
				this.PlayCollisionSound();
				this.collisionFlag = false;
			}
		}

		// Token: 0x06001064 RID: 4196 RVA: 0x000BB974 File Offset: 0x000B9B74
		public void PlayCollisionSound()
		{
			if (!base.IsEnabled || !this.initialized)
			{
				return;
			}
			if (base.Clips.Count == 0 || this.collisionData == null)
			{
				return;
			}
			ContactPoint[] contacts = this.collisionData.contacts;
			int num = contacts.Length;
			for (int i = 0; i < num; i++)
			{
				if (contacts[i].thisCollider.name == "RimCollider")
				{
					return;
				}
			}
			base.Source.transform.position = this.collisionData.contacts[0].point;
			base.Source.clip = base.RandomClip;
			float volume = Mathf.Clamp01(this.collisionData.relativeVelocity.magnitude * 0.025f * this.velocityMagnitudeEffect) * this.baseVolume;
			float pitch = Random.Range(1f - this.pitchRandomness, 1f + this.pitchRandomness) * this.basePitch;
			base.SetVolume(volume);
			base.SetPitch(pitch);
			base.Play();
		}

		// Token: 0x06001065 RID: 4197 RVA: 0x000BBA85 File Offset: 0x000B9C85
		public void RaiseCollisionFlag(Collision collision)
		{
			this.collisionData = collision;
			this.collisionFlag = true;
		}

		// Token: 0x06001066 RID: 4198 RVA: 0x000BBA98 File Offset: 0x000B9C98
		public override void SetDefaults(VehicleController vc)
		{
			base.SetDefaults(vc);
			this.baseVolume = 0.4f;
			this.basePitch = 1f;
			if (base.Clip == null)
			{
				base.Clip = (Resources.Load("NWH Vehicle Physics/Defaults/Sound/Crash") as AudioClip);
				if (base.Clip == null)
				{
					Debug.LogWarning("Audio Clip for sound component " + base.GetType().Name + " could not be loaded from resources. Source will not play.");
				}
			}
		}

		// Token: 0x040020DD RID: 8413
		[Range(0f, 0.5f)]
		[Tooltip("Different random pitch in range [basePitch + (1 +- pitchRandomness)] is set each time a collision happens.")]
		public float pitchRandomness = 0.4f;

		// Token: 0x040020DE RID: 8414
		[Range(0f, 5f)]
		[Tooltip("    Higher values result in collisions getting louder for the given collision velocity magnitude.")]
		public float velocityMagnitudeEffect = 1f;

		// Token: 0x040020DF RID: 8415
		private Collision collisionData;

		// Token: 0x040020E0 RID: 8416
		private bool collisionFlag;
	}
}
