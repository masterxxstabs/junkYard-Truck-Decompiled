using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
	// Token: 0x02000274 RID: 628
	[Serializable]
	public class TurboFlutterComponent : SoundComponent
	{
		// Token: 0x06001090 RID: 4240 RVA: 0x000BCA84 File Offset: 0x000BAC84
		public override void Initialize()
		{
			if (base.Clip != null)
			{
				base.Source = this.container.AddComponent<AudioSource>();
				this.vc.soundManager.SetAudioSourceDefaults(base.Source, false, false, this.baseVolume, base.Clip);
				base.AddSourcesToMixer();
			}
			this.initialized = true;
		}

		// Token: 0x06001091 RID: 4241 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x06001092 RID: 4242 RVA: 0x000BCAE4 File Offset: 0x000BACE4
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			if (base.Clip != null && this.vc.powertrain.engine.forcedInduction.useForcedInduction && this.vc.powertrain.engine.forcedInduction.hasWastegate && this.vc.powertrain.engine.forcedInduction.wastegateFlag)
			{
				base.Source.pitch = this.basePitch + this.basePitch * Random.Range(-this.pitchRandomnessRange, this.pitchRandomnessRange);
				float num = this.baseVolume * this.vc.powertrain.engine.forcedInduction.wastegateBoost;
				num = ((num < 0f) ? 0f : ((num > 1f) ? 1f : num));
				base.SetVolume(num);
				base.Play();
				this.vc.powertrain.engine.forcedInduction.wastegateFlag = false;
			}
		}

		// Token: 0x06001093 RID: 4243 RVA: 0x000BCC00 File Offset: 0x000BAE00
		public override void SetDefaults(VehicleController vc)
		{
			base.SetDefaults(vc);
			this.baseVolume = 0.1f;
			this.basePitch = 1f;
			this.pitchRandomnessRange = 0.3f;
			if (base.Clip == null)
			{
				base.Clip = (Resources.Load("NWH Vehicle Physics/Defaults/Sound/TurboFlutter") as AudioClip);
				if (base.Clip == null)
				{
					Debug.LogWarning("Audio Clip for sound component " + base.GetType().Name + " could not be loaded from resources. Source will not play.");
				}
			}
		}

		// Token: 0x040020F5 RID: 8437
		[Range(0f, 0.4f)]
		[Tooltip("Final pitch will be a random value in the interval [pitch - pitchRandomnessRange, pitch + pitchRandomnessRange].\r\nMake sure that the value is not larger than base pitch value as to avoid negative values.")]
		public float pitchRandomnessRange = 0.3f;
	}
}
