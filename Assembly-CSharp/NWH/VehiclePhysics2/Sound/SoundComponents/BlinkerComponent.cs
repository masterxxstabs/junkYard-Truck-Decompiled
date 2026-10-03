using System;
using NWH.VehiclePhysics2.Effects;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
	// Token: 0x02000269 RID: 617
	[Serializable]
	public class BlinkerComponent : SoundComponent
	{
		// Token: 0x06001053 RID: 4179 RVA: 0x000BB440 File Offset: 0x000B9640
		public override void Initialize()
		{
			if (base.Clip != null)
			{
				base.Source = this.container.AddComponent<AudioSource>();
				this.vc.soundManager.SetAudioSourceDefaults(base.Source, false, false, this.baseVolume, null);
				base.AddSourcesToMixer();
				base.Source.dopplerLevel = 0f;
			}
			this.initialized = true;
			foreach (LightSource lightSource in this.vc.effectsManager.lightsManager.leftBlinkers.lightSources)
			{
				lightSource.onLightTurnedOn.AddListener(delegate()
				{
					this._onFlag = true;
				});
				lightSource.onLightTurnedOff.AddListener(delegate()
				{
					this._offFlag = true;
				});
			}
			foreach (LightSource lightSource2 in this.vc.effectsManager.lightsManager.rightBlinkers.lightSources)
			{
				lightSource2.onLightTurnedOn.AddListener(delegate()
				{
					this._onFlag = true;
				});
				lightSource2.onLightTurnedOff.AddListener(delegate()
				{
					this._offFlag = true;
				});
			}
		}

		// Token: 0x06001054 RID: 4180 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x06001055 RID: 4181 RVA: 0x000BB5A4 File Offset: 0x000B97A4
		public override void Update()
		{
			if (!base.Active || base.Clips.Count == 0)
			{
				return;
			}
			base.Source.volume = this.baseVolume;
			base.Source.pitch = this.basePitch;
			if (this._onFlag)
			{
				base.Source.clip = base.Clips[0];
				base.Play();
			}
			if (this._offFlag)
			{
				if (base.Clips.Count == 2)
				{
					base.Source.clip = base.Clips[1];
				}
				else
				{
					base.Source.clip = base.Clips[0];
				}
				base.Play();
			}
			this._onFlag = false;
			this._offFlag = false;
		}

		// Token: 0x06001056 RID: 4182 RVA: 0x000BB668 File Offset: 0x000B9868
		public override void SetDefaults(VehicleController vc)
		{
			base.SetDefaults(vc);
			this.baseVolume = 0.8f;
			this.basePitch = 1f;
			if (base.Clip == null || base.Clips.Count == 0)
			{
				AudioClip audioClip = Resources.Load("NWH Vehicle Physics/Defaults/Sound/BlinkerOn") as AudioClip;
				if (audioClip == null)
				{
					Debug.LogWarning("Audio Clip for sound component " + base.GetType().Name + " could not be loaded from resources. Source will not play.");
				}
				else
				{
					base.Clips.Add(audioClip);
				}
				AudioClip audioClip2 = Resources.Load("NWH Vehicle Physics/Defaults/Sound/BlinkerOff") as AudioClip;
				if (audioClip2 == null)
				{
					Debug.LogWarning("Audio Clip for sound component " + base.GetType().Name + " could not be loaded from resources. Source will not play.");
					return;
				}
				base.Clips.Add(audioClip2);
			}
		}

		// Token: 0x040020DA RID: 8410
		private bool _onFlag;

		// Token: 0x040020DB RID: 8411
		private bool _offFlag;
	}
}
