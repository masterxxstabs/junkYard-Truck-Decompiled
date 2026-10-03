using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
	// Token: 0x0200026F RID: 623
	[Serializable]
	public class GearChangeComponent : SoundComponent
	{
		// Token: 0x06001077 RID: 4215 RVA: 0x000BC0BC File Offset: 0x000BA2BC
		public override void Initialize()
		{
			if (base.Clips.Count != 0)
			{
				base.Source = this.container.AddComponent<AudioSource>();
				this.vc.soundManager.SetAudioSourceDefaults(base.Source, false, false, this.baseVolume, base.RandomClip);
				base.AddSourcesToMixer();
				base.Source.dopplerLevel = 0f;
			}
			this.initialized = true;
		}

		// Token: 0x06001078 RID: 4216 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x06001079 RID: 4217 RVA: 0x000BC128 File Offset: 0x000BA328
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			if (base.Clips.Count != 0)
			{
				if (this.previousGear != this.vc.powertrain.transmission.Gear && !base.Source.isPlaying)
				{
					base.Source.clip = base.RandomClip;
					base.SetVolume(this.baseVolume + this.baseVolume * Random.Range(-this.randomVolumeRange, this.randomVolumeRange));
					base.SetPitch(this.basePitch + this.basePitch * Random.Range(-this.randomPitchRange, this.randomPitchRange));
					if (base.Source.enabled)
					{
						base.Play();
					}
				}
				this.previousGear = this.vc.powertrain.transmission.Gear;
			}
		}

		// Token: 0x0600107A RID: 4218 RVA: 0x000BC204 File Offset: 0x000BA404
		public override void SetDefaults(VehicleController vc)
		{
			base.SetDefaults(vc);
			this.baseVolume = 0.16f;
			this.basePitch = 0.8f;
			if (base.Clip == null)
			{
				base.Clip = (Resources.Load("NWH Vehicle Physics/Defaults/Sound/GearChange") as AudioClip);
				if (base.Clip == null)
				{
					Debug.LogWarning("Audio Clip for sound component " + base.GetType().Name + " could not be loaded from resources. Source will not play.");
				}
			}
		}

		// Token: 0x040020E8 RID: 8424
		[Range(0f, 0.5f)]
		[Tooltip("Determines how much pitch of the gear shift sound can vary from one shift to another.\r\nFinal pitch is calculated as base pitch +- randomPitchRange.")]
		public float randomPitchRange = 0.2f;

		// Token: 0x040020E9 RID: 8425
		[Range(0f, 0.5f)]
		[Tooltip("Determines how much volume of the gear shift sound car vary.\r\nFinal volume is caulculated as base volume +- randomVolumeRange.")]
		public float randomVolumeRange = 0.1f;

		// Token: 0x040020EA RID: 8426
		private int previousGear;
	}
}
