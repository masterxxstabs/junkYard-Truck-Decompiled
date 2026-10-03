using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
	// Token: 0x0200026A RID: 618
	[Serializable]
	public class BrakeHissComponent : SoundComponent
	{
		// Token: 0x0600105C RID: 4188 RVA: 0x000BB758 File Offset: 0x000B9958
		public override void Initialize()
		{
			if (base.Clip != null)
			{
				base.Source = this.container.AddComponent<AudioSource>();
				this.vc.soundManager.SetAudioSourceDefaults(base.Source, false, false, this.baseVolume, null);
				base.AddSourcesToMixer();
			}
			this.initialized = true;
		}

		// Token: 0x0600105D RID: 4189 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x0600105E RID: 4190 RVA: 0x000BB7B0 File Offset: 0x000B99B0
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			if (base.Clip != null)
			{
				if (this._prevActive && !this.vc.brakes.Active && this.vc.Speed < 1f && !base.Source.isPlaying)
				{
					base.Source.clip = base.RandomClip;
					base.Play();
				}
				this._prevActive = this.vc.brakes.Active;
			}
		}

		// Token: 0x0600105F RID: 4191 RVA: 0x000BB83C File Offset: 0x000B9A3C
		public override void SetDefaults(VehicleController vc)
		{
			base.SetDefaults(vc);
			this.baseVolume = 0.1f;
			this.basePitch = 1f;
			if (base.Clip == null)
			{
				base.Clip = (Resources.Load("NWH Vehicle Physics/Defaults/Sound/AirBrakes") as AudioClip);
				if (base.Clip == null)
				{
					Debug.LogWarning("Audio Clip for sound component " + base.GetType().Name + " could not be loaded from resources. Source will not play.");
				}
			}
		}

		// Token: 0x040020DC RID: 8412
		private bool _prevActive;
	}
}
