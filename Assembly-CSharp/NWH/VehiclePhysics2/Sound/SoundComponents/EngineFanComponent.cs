using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
	// Token: 0x0200026C RID: 620
	[Serializable]
	public class EngineFanComponent : SoundComponent
	{
		// Token: 0x06001068 RID: 4200 RVA: 0x000BBB30 File Offset: 0x000B9D30
		public override void Initialize()
		{
			if (base.Clips.Count != 0)
			{
				base.Source = this.container.AddComponent<AudioSource>();
				this.vc.soundManager.SetAudioSourceDefaults(base.Source, false, true, this.baseVolume, base.Clip);
				base.AddSourcesToMixer();
			}
			this.initialized = true;
		}

		// Token: 0x06001069 RID: 4201 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x0600106A RID: 4202 RVA: 0x000BBB8C File Offset: 0x000B9D8C
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			if (this.vc.powertrain.engine.IsRunning)
			{
				if (!base.Source.isPlaying)
				{
					base.Play();
				}
				float rpmpercent = this.vc.powertrain.engine.RPMPercent;
				base.SetVolume(rpmpercent * rpmpercent * this.baseVolume);
				base.SetPitch(this.basePitch + this.pitchRange * rpmpercent);
				return;
			}
			if (base.Source.isPlaying)
			{
				base.Stop();
			}
		}

		// Token: 0x0600106B RID: 4203 RVA: 0x000BBC1C File Offset: 0x000B9E1C
		public override void SetDefaults(VehicleController vc)
		{
			base.SetDefaults(vc);
			this.baseVolume = 0.05f;
			this.basePitch = 1f;
			if (base.Clip == null)
			{
				base.Clip = (Resources.Load("NWH Vehicle Physics/Defaults/Sound/EngineFan") as AudioClip);
				if (base.Clip == null)
				{
					Debug.LogWarning("Audio Clip for sound component " + base.GetType().Name + " could not be loaded from resources. Source will not play.");
				}
			}
		}

		// Token: 0x040020E1 RID: 8417
		[Range(0f, 1f)]
		public float pitchRange = 0.5f;
	}
}
