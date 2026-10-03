using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
	// Token: 0x0200026E RID: 622
	[Serializable]
	public class EngineStartComponent : SoundComponent
	{
		// Token: 0x06001072 RID: 4210 RVA: 0x000BBF54 File Offset: 0x000BA154
		public override void Initialize()
		{
			if (base.Clips.Count > 0)
			{
				base.Source = this.container.AddComponent<AudioSource>();
				this.vc.soundManager.SetAudioSourceDefaults(base.Source, false, false, 0f, base.Clip);
				base.AddSourcesToMixer();
				base.Stop();
				base.SetVolume(0f);
			}
			this.initialized = true;
		}

		// Token: 0x06001073 RID: 4211 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x06001074 RID: 4212 RVA: 0x000BBFC4 File Offset: 0x000BA1C4
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			if (base.Source != null && base.Clips.Count > 0)
			{
				if (this.vc.powertrain.engine.StarterActive)
				{
					if (!base.Source.isPlaying)
					{
						base.SetVolume(this.baseVolume);
						base.Play();
						return;
					}
				}
				else if (base.Source.isPlaying)
				{
					base.Stop();
				}
			}
		}

		// Token: 0x06001075 RID: 4213 RVA: 0x000BC040 File Offset: 0x000BA240
		public override void SetDefaults(VehicleController vc)
		{
			base.SetDefaults(vc);
			this.baseVolume = 0.2f;
			this.basePitch = 1f;
			if (base.Clip == null)
			{
				base.Clip = (Resources.Load("NWH Vehicle Physics/Defaults/Sound/EngineStart") as AudioClip);
				if (base.Clip == null)
				{
					Debug.LogWarning("Audio Clip for sound component " + base.GetType().Name + " could not be loaded from resources. Source will not play.");
				}
			}
		}
	}
}
