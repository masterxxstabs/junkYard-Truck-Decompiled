using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
	// Token: 0x02000271 RID: 625
	[Serializable]
	public class ReverseBeepComponent : SoundComponent
	{
		// Token: 0x06001081 RID: 4225 RVA: 0x000BC3F4 File Offset: 0x000BA5F4
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

		// Token: 0x06001082 RID: 4226 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x06001083 RID: 4227 RVA: 0x000BC450 File Offset: 0x000BA650
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			int gear = this.vc.powertrain.transmission.Gear;
			if ((this.beepOnReverseGear && gear < 0) || (this.beepOnNegativeVelocity && this.vc.ForwardVelocity < -0.2f && gear <= 0))
			{
				if (!base.Source.isPlaying)
				{
					base.Play();
					return;
				}
			}
			else if (base.Source.isPlaying)
			{
				base.Stop();
			}
		}

		// Token: 0x06001084 RID: 4228 RVA: 0x000BC4D0 File Offset: 0x000BA6D0
		public override void SetDefaults(VehicleController vc)
		{
			base.SetDefaults(vc);
			if (base.Clip == null)
			{
				base.Clip = (Resources.Load("NWH Vehicle Physics/Defaults/Sound/ReverseBeep") as AudioClip);
				if (base.Clip == null)
				{
					Debug.LogWarning("Audio Clip for sound component " + base.GetType().Name + " could not be loaded from resources. Source will not play.");
				}
			}
		}

		// Token: 0x040020EB RID: 8427
		public bool beepOnNegativeVelocity = true;

		// Token: 0x040020EC RID: 8428
		public bool beepOnReverseGear = true;
	}
}
