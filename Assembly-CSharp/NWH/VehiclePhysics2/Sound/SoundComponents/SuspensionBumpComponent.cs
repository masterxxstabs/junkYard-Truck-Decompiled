using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Powertrain;
using NWH.WheelController3D;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
	// Token: 0x02000272 RID: 626
	[Serializable]
	public class SuspensionBumpComponent : SoundComponent
	{
		// Token: 0x06001086 RID: 4230 RVA: 0x000BC54C File Offset: 0x000BA74C
		public override void Initialize()
		{
			foreach (WheelComponent wheelComponent in this.vc.Wheels)
			{
				AudioSource audioSource = wheelComponent.ControllerGO.AddComponent<AudioSource>();
				this.vc.soundManager.SetAudioSourceDefaults(audioSource, false, false, this.baseVolume, null);
				base.Sources.Add(audioSource);
				bool item = true;
				this.prevHasHits.Add(item);
			}
			base.AddSourcesToMixer();
			this.initialized = true;
		}

		// Token: 0x06001087 RID: 4231 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x06001088 RID: 4232 RVA: 0x000BC5E8 File Offset: 0x000BA7E8
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			if (base.Clip != null && base.Sources != null && base.Sources.Count == this.vc.Wheels.Count)
			{
				int count = this.vc.Wheels.Count;
				for (int i = 0; i < count; i++)
				{
					WheelController wheelController = this.vc.Wheels[i].wheelController;
					if (!base.Sources[i].isPlaying)
					{
						float angleForward = wheelController.wheelHit.angleForward;
						if ((wheelController.isGrounded && !this.prevHasHits[i]) || (wheelController.forwardFriction.speed > 0.8f && (angleForward > 15f || angleForward < -15f)))
						{
							float pitch = Random.Range(0.8f, 1.2f) * this.basePitch;
							float num = (wheelController.damperForce < 0f) ? (-wheelController.damperForce) : wheelController.damperForce;
							float volume = this.baseVolume * Mathf.Clamp01(num / Mathf.Max(wheelController.damper.bumpForce, wheelController.damper.reboundForce));
							base.SetVolume(volume, i);
							base.SetPitch(pitch, i);
							if (!base.Sources[i].isPlaying)
							{
								base.Sources[i].clip = base.RandomClip;
								base.Sources[i].Play();
							}
						}
					}
					this.prevHasHits[i] = wheelController.isGrounded;
				}
			}
		}

		// Token: 0x06001089 RID: 4233 RVA: 0x000BC798 File Offset: 0x000BA998
		public override void SetDefaults(VehicleController vc)
		{
			base.SetDefaults(vc);
			this.baseVolume = 0.12f;
			this.basePitch = 1f;
			if (base.Clip == null)
			{
				base.Clip = (Resources.Load("NWH Vehicle Physics/Defaults/Sound/SuspensionBump") as AudioClip);
				if (base.Clip == null)
				{
					Debug.LogWarning("Audio Clip for sound component " + base.GetType().Name + " could not be loaded from resources. Source will not play.");
				}
			}
		}

		// Token: 0x040020ED RID: 8429
		private List<bool> prevHasHits = new List<bool>();
	}
}
