using System;
using NWH.VehiclePhysics2.GroundDetection;
using NWH.VehiclePhysics2.Powertrain;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
	// Token: 0x02000276 RID: 630
	[Serializable]
	public class WheelSkidComponent : SoundComponent
	{
		// Token: 0x0600109A RID: 4250 RVA: 0x000BCE84 File Offset: 0x000BB084
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

		// Token: 0x0600109B RID: 4251 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x0600109C RID: 4252 RVA: 0x000BCF28 File Offset: 0x000BB128
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			float num = 0f;
			float num2 = this.basePitch;
			if (this.vc.groundDetection != null)
			{
				for (int i = 0; i < this._wheelCount; i++)
				{
					WheelComponent wheelComponent = this.vc.Wheels[i];
					SurfacePreset surfacePreset = wheelComponent.surfacePreset;
					bool flag = wheelComponent.HasLateralSlip || wheelComponent.HasLongitudinalSlip;
					if (wheelComponent.IsGrounded && surfacePreset != null && surfacePreset.playSkidSounds && flag)
					{
						float num3 = wheelComponent.NormalizedLateralSlip + wheelComponent.NormalizedLongitudinalSlip;
						num3 = ((num3 < 0f) ? 0f : ((num3 > 1f) ? 1f : num3));
						if (surfacePreset.skidSoundClip != null)
						{
							AudioSource audioSource = base.Sources[i];
							if (!audioSource.isPlaying)
							{
								audioSource.Play();
							}
							if (audioSource.clip != surfacePreset.skidSoundClip)
							{
								audioSource.clip = surfacePreset.skidSoundClip;
								audioSource.time = Random.Range(0f, surfacePreset.skidSoundClip.length);
								audioSource.time = Random.Range(0f, audioSource.clip.length);
							}
							float num4 = (wheelComponent.angularVelocity < 0f) ? (-wheelComponent.angularVelocity) : wheelComponent.angularVelocity;
							float num5 = this.vc.Speed / 3f + num4 / 20f;
							num5 = ((num5 > 1f) ? 1f : num5);
							num = num3 * surfacePreset.skidSoundVolume * num5;
							num = Mathf.Lerp(this._prevVolume[i], num, this.vc.deltaTime * 12f);
							float num6 = wheelComponent.wheelController.wheel.load / wheelComponent.wheelController.maximumTireLoad;
							num6 = ((num6 > 1f) ? 1f : num6);
							num2 = surfacePreset.skidSoundPitch + num6 * 0.3f;
							num2 = Mathf.Lerp(this._prevPitch[i], num2, this.vc.deltaTime * 18f);
						}
					}
					else
					{
						num = 0f;
						num2 = this.basePitch;
					}
					base.SetVolume(num, i);
					base.SetPitch(num2, i);
					this._prevVolume[i] = num;
					this._prevPitch[i] = num2;
				}
			}
		}

		// Token: 0x0600109D RID: 4253 RVA: 0x000BD193 File Offset: 0x000BB393
		public override void SetDefaults(VehicleController vc)
		{
			base.SetDefaults(vc);
			this.baseVolume = 0.5f;
			this.basePitch = 1.2f;
		}

		// Token: 0x040020F7 RID: 8439
		private float[] _prevPitch;

		// Token: 0x040020F8 RID: 8440
		private float[] _prevVolume;

		// Token: 0x040020F9 RID: 8441
		private int _wheelCount;
	}
}
