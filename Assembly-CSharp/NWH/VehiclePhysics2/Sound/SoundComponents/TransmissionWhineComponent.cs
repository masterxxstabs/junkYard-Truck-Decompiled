using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
	// Token: 0x02000273 RID: 627
	[Serializable]
	public class TransmissionWhineComponent : SoundComponent
	{
		// Token: 0x0600108B RID: 4235 RVA: 0x000BC828 File Offset: 0x000BAA28
		public override void Initialize()
		{
			if (base.Clip != null)
			{
				base.Source = this.container.AddComponent<AudioSource>();
				this.vc.soundManager.SetAudioSourceDefaults(base.Source, true, true, 0f, base.Clip);
				base.AddSourcesToMixer();
			}
			this.initialized = true;
		}

		// Token: 0x0600108C RID: 4236 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x0600108D RID: 4237 RVA: 0x000BC884 File Offset: 0x000BAA84
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			if (base.Clip != null)
			{
				float speed = this.vc.Speed;
				float num = this.basePitch;
				if (this.vc.powertrain.transmission.Gear != 0)
				{
					num += Mathf.Clamp01(speed / this.maxSpeed) * this.pitchRange;
				}
				base.SetPitch(num);
				float num2 = this.vc.powertrain.engine.generatedPower / this.vc.powertrain.engine.maxPower;
				num2 = ((num2 < 0f) ? 0f : ((num2 > 1f) ? 1f : num2));
				float value = ((speed < 0f) ? (-speed) : speed) * 0.2f;
				float num3 = this.baseVolume * ((1f - num2) * this.offThrottleVolumeCoeff + num2 * this.onThrottleVolumeCoeff) * Mathf.Clamp01(value);
				if (this.smoothing > 0f)
				{
					this._volume = Mathf.SmoothDamp(this._volume, num3, ref this._whineVelocity, this.smoothing);
					base.SetVolume(this._volume);
					return;
				}
				this._volume = num3;
				base.SetVolume(this._volume);
			}
		}

		// Token: 0x0600108E RID: 4238 RVA: 0x000BC9C8 File Offset: 0x000BABC8
		public override void SetDefaults(VehicleController vc)
		{
			base.SetDefaults(vc);
			this.baseVolume = 0.06f;
			this.basePitch = 0.16f;
			if (base.Clip == null)
			{
				base.Clip = (Resources.Load("NWH Vehicle Physics/Defaults/Sound/TransmissionWhine") as AudioClip);
				if (base.Clip == null)
				{
					Debug.LogWarning("Audio Clip for sound component " + base.GetType().Name + "  from resources. Source will not play.");
				}
			}
		}

		// Token: 0x040020EE RID: 8430
		[Tooltip("Maximum speed value [m/s] of the vehicle at which the pitch will be at the top end of the pitchRange.")]
		public float maxSpeed = 80f;

		// Token: 0x040020EF RID: 8431
		[Range(0f, 1f)]
		[Tooltip("    Volume coefficient when transmission is not under load.")]
		public float offThrottleVolumeCoeff = 0.2f;

		// Token: 0x040020F0 RID: 8432
		[Range(0f, 1f)]
		[Tooltip("    Volume coefficient when transmission is under load.")]
		public float onThrottleVolumeCoeff = 1f;

		// Token: 0x040020F1 RID: 8433
		[Range(0f, 5f)]
		[Tooltip("    Pitch range that will be added to the base pitch depending on transmission state.")]
		public float pitchRange = 0.7f;

		// Token: 0x040020F2 RID: 8434
		[Range(0f, 0.2f)]
		[Tooltip("    Smoothing of the transmission whine.")]
		public float smoothing = 0.05f;

		// Token: 0x040020F3 RID: 8435
		private float _volume;

		// Token: 0x040020F4 RID: 8436
		private float _whineVelocity;
	}
}
