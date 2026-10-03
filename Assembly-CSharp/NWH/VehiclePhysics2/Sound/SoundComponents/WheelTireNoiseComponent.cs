using System;
using NWH.VehiclePhysics2.GroundDetection;
using NWH.VehiclePhysics2.Powertrain;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
	// Token: 0x02000277 RID: 631
	[Serializable]
	public class WheelTireNoiseComponent : SoundComponent
	{
		// Token: 0x0600109F RID: 4255 RVA: 0x000BD1B4 File Offset: 0x000BB3B4
		public override void Initialize()
		{
			this._wheelCount = this.vc.Wheels.Count;
			for (int i = 0; i < this._wheelCount; i++)
			{
				AudioSource audioSource = this.vc.Wheels[i].ControllerGO.AddComponent<AudioSource>();
				this.vc.soundManager.SetAudioSourceDefaults(audioSource, true, true, 0f, null);
				base.Sources.Add(audioSource);
			}
			base.AddSourcesToMixer();
			this._prevVolume = new float[this._wheelCount];
			this._prevPitch = new float[this._wheelCount];
			this.initialized = true;
		}

		// Token: 0x060010A0 RID: 4256 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x060010A1 RID: 4257 RVA: 0x000BD258 File Offset: 0x000BB458
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			for (int i = 0; i < this._wheelCount; i++)
			{
				WheelComponent wheelComponent = this.vc.Wheels[i];
				SurfacePreset surfacePreset = wheelComponent.surfacePreset;
				float num = 0f;
				float num2 = this.basePitch;
				if (wheelComponent.IsGrounded && surfacePreset != null && surfacePreset.playSurfaceSounds)
				{
					if (surfacePreset.surfaceSoundClip != null)
					{
						AudioSource audioSource = base.Sources[i];
						if (!audioSource.isPlaying)
						{
							audioSource.Play();
						}
						if (audioSource.clip != surfacePreset.surfaceSoundClip)
						{
							audioSource.clip = surfacePreset.surfaceSoundClip;
							audioSource.time = Random.Range(0f, surfacePreset.surfaceSoundClip.length);
							audioSource.time = Random.Range(0f, audioSource.clip.length);
						}
						float num3 = 1f;
						if (surfacePreset.slipSensitiveSurfaceSound)
						{
							num3 = wheelComponent.NormalizedLateralSlip / this.vc.longitudinalSlipThreshold;
							num3 = ((num3 < 0f) ? 0f : ((num3 > 1f) ? 1f : num3));
						}
						float num4 = this.vc.Speed / 20f;
						num4 = ((num4 < 0f) ? 0f : ((num4 > 1f) ? 1f : num4));
						num = surfacePreset.surfaceSoundVolume * num3 * num4;
						num = ((num < 0f) ? 0f : ((num > 1f) ? 1f : num));
						num = Mathf.Lerp(this._prevVolume[i], num, this.vc.deltaTime * 12f);
						num2 = surfacePreset.surfaceSoundPitch * 0.5f + num4;
					}
				}
				else
				{
					num = Mathf.Lerp(this._prevVolume[i], 0f, this.vc.deltaTime * 12f);
					num2 = Mathf.Lerp(this._prevPitch[i], this.basePitch, this.vc.deltaTime * 12f);
				}
				base.SetVolume(num, i);
				base.SetPitch(num2, i);
				this._prevVolume[i] = num;
				this._prevPitch[i] = num2;
			}
		}

		// Token: 0x060010A2 RID: 4258 RVA: 0x000BD4A3 File Offset: 0x000BB6A3
		public override void SetDefaults(VehicleController vc)
		{
			base.SetDefaults(vc);
			this.baseVolume = 0.4f;
			this.basePitch = 1f;
		}

		// Token: 0x040020FA RID: 8442
		private float[] _prevPitch;

		// Token: 0x040020FB RID: 8443
		private float[] _prevVolume;

		// Token: 0x040020FC RID: 8444
		private int _wheelCount;
	}
}
