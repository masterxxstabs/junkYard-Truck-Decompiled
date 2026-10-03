using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
	// Token: 0x0200026D RID: 621
	[Serializable]
	public class EngineRunningComponent : SoundComponent
	{
		// Token: 0x0600106D RID: 4205 RVA: 0x000BBCAC File Offset: 0x000B9EAC
		public override void Initialize()
		{
			if (base.Clip != null)
			{
				base.Source = this.container.AddComponent<AudioSource>();
				this.vc.soundManager.SetAudioSourceDefaults(base.Source, false, true, 0f, base.Clip);
				base.AddSourcesToMixer();
				base.Stop();
				base.SetVolume(0f);
			}
			this.initialized = true;
		}

		// Token: 0x0600106E RID: 4206 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x0600106F RID: 4207 RVA: 0x000BBD1C File Offset: 0x000B9F1C
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			if (base.Source != null && base.Clip != null)
			{
				if (this.vc.powertrain.engine.IsRunning || this.vc.powertrain.engine.starterActive)
				{
					if (!base.Source.isPlaying && base.Source.enabled)
					{
						base.Play();
					}
					float pitch = this.basePitch + this.vc.powertrain.engine.RPMPercent * this.pitchRange;
					float num = this.vc.powertrain.engine.revLimiterActive ? 1f : this.vc.powertrain.engine.ThrottlePosition;
					base.SetPitch(pitch);
					float target = this.baseVolume + num * this.volumeRange;
					this.audioMixerGroup.audioMixer.SetFloat("engineDistortion", num * this.maxDistortion);
					if (this.vc.powertrain.engine.starterActive)
					{
						target = this.baseVolume;
					}
					this._volume = Mathf.SmoothDamp(this._volume, target, ref this._volumeVelocity, this.smoothing);
					base.SetVolume(this._volume);
					return;
				}
				if (base.Source.isPlaying)
				{
					base.Stop();
				}
				base.SetVolume(0f);
				base.SetPitch(0f);
			}
		}

		// Token: 0x06001070 RID: 4208 RVA: 0x000BBEA4 File Offset: 0x000BA0A4
		public override void SetDefaults(VehicleController vc)
		{
			base.SetDefaults(vc);
			this.baseVolume = 0.2f;
			this.basePitch = 0.4f;
			if (base.Clip == null)
			{
				base.Clip = (Resources.Load("NWH Vehicle Physics/Defaults/Sound/EngineRunning") as AudioClip);
				if (base.Clip == null)
				{
					Debug.LogWarning("Audio Clip for sound component " + base.GetType().Name + " could not be loaded from resources. Source will not play.");
				}
			}
		}

		// Token: 0x040020E2 RID: 8418
		[Range(0f, 1f)]
		[Tooltip("    Distortion at maximum engine load.")]
		public float maxDistortion = 0.4f;

		// Token: 0x040020E3 RID: 8419
		[Range(0f, 4f)]
		[Tooltip("    Pitch added to the base engine pitch depending on engine RPM.")]
		public float pitchRange = 2.5f;

		// Token: 0x040020E4 RID: 8420
		[Range(0f, 1f)]
		[Tooltip("    Smoothing of engine volume.")]
		public float smoothing = 0.05f;

		// Token: 0x040020E5 RID: 8421
		[Range(0f, 1f)]
		[Tooltip("    Volume added to the base engine volume depending on engine state.")]
		public float volumeRange = 0.1f;

		// Token: 0x040020E6 RID: 8422
		private float _volume;

		// Token: 0x040020E7 RID: 8423
		private float _volumeVelocity;
	}
}
