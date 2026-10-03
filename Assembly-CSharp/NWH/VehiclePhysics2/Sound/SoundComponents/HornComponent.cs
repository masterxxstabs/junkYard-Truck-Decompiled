using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
	// Token: 0x02000270 RID: 624
	[Serializable]
	public class HornComponent : SoundComponent
	{
		// Token: 0x0600107C RID: 4220 RVA: 0x000BC29C File Offset: 0x000BA49C
		public override void Initialize()
		{
			if (base.Clips.Count != 0)
			{
				base.Source = this.container.AddComponent<AudioSource>();
				this.vc.soundManager.SetAudioSourceDefaults(base.Source, false, true, this.baseVolume, base.RandomClip);
				base.AddSourcesToMixer();
			}
			this.initialized = true;
		}

		// Token: 0x0600107D RID: 4221 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x0600107E RID: 4222 RVA: 0x000BC2F8 File Offset: 0x000BA4F8
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			if (base.Source != null && base.Clip != null)
			{
				if (this.vc.input.Horn && !base.Source.isPlaying)
				{
					base.SetPitch(this.basePitch);
					base.SetVolume(this.baseVolume);
					base.Play();
					return;
				}
				if (!this.vc.input.Horn && base.Source.isPlaying)
				{
					base.Stop();
				}
			}
		}

		// Token: 0x0600107F RID: 4223 RVA: 0x000BC390 File Offset: 0x000BA590
		public override void SetDefaults(VehicleController vc)
		{
			base.SetDefaults(vc);
			if (base.Clip == null)
			{
				base.Clip = (Resources.Load("NWH Vehicle Physics/Defaults/Sound/Horn") as AudioClip);
				if (base.Clip == null)
				{
					Debug.LogWarning("Audio Clip for sound component " + base.GetType().Name + " could not be loaded from resources. Source will not play.");
				}
			}
		}
	}
}
