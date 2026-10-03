using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
	// Token: 0x02000275 RID: 629
	[Serializable]
	public class TurboWhistleComponent : SoundComponent
	{
		// Token: 0x06001095 RID: 4245 RVA: 0x000BCC98 File Offset: 0x000BAE98
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

		// Token: 0x06001096 RID: 4246 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x06001097 RID: 4247 RVA: 0x000BCCF4 File Offset: 0x000BAEF4
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			if (base.Clip != null && this.vc.powertrain.engine.IsRunning && this.vc.powertrain.engine.forcedInduction.useForcedInduction)
			{
				base.SetVolume(Mathf.Clamp01(this.baseVolume * Mathf.Pow(this.vc.powertrain.engine.forcedInduction.boost, 1.5f)) * this.vc.soundManager.masterVolume);
				base.SetPitch(this.basePitch + this.pitchRange * this.vc.powertrain.engine.forcedInduction.boost);
				base.Play();
				return;
			}
			if (base.Source != null)
			{
				base.SetVolume(0f);
				base.Stop();
			}
		}

		// Token: 0x06001098 RID: 4248 RVA: 0x000BCDEC File Offset: 0x000BAFEC
		public override void SetDefaults(VehicleController vc)
		{
			base.SetDefaults(vc);
			this.baseVolume = 0.12f;
			this.basePitch = 0f;
			this.pitchRange = 0.8f;
			if (base.Clip == null)
			{
				base.Clip = (Resources.Load("NWH Vehicle Physics/Defaults/Sound/TurboWhistle") as AudioClip);
				if (base.Clip == null)
				{
					Debug.LogWarning("Audio Clip for sound component " + base.GetType().Name + " could not be loaded from resources. Source will not play.");
				}
			}
		}

		// Token: 0x040020F6 RID: 8438
		[Range(0f, 5f)]
		[Tooltip("    Pitch range that will be added to the base pitch depending on turbos's RPM.")]
		public float pitchRange = 0.9f;
	}
}
