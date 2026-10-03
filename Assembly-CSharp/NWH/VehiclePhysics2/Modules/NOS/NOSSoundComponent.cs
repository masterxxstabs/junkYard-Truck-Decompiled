using System;
using NWH.VehiclePhysics2.Sound.SoundComponents;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.NOS
{
	// Token: 0x02000295 RID: 661
	[Serializable]
	public class NOSSoundComponent : SoundComponent
	{
		// Token: 0x060011D2 RID: 4562 RVA: 0x000C3120 File Offset: 0x000C1320
		public override void Initialize()
		{
			if (base.Clip != null)
			{
				base.Source = this.container.AddComponent<AudioSource>();
				this.vc.soundManager.SetAudioSourceDefaults(base.Source, false, true, 0f, base.Clip);
				base.AddSourcesToMixer();
				base.Stop();
			}
			this.initialized = true;
		}

		// Token: 0x060011D3 RID: 4563 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x060011D4 RID: 4564 RVA: 0x000C3184 File Offset: 0x000C1384
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			if (base.Clip != null && base.Sources != null)
			{
				if (this.nosModule.active && !this.wasActive)
				{
					base.SetVolume(this.baseVolume);
					base.SetPitch(this.basePitch);
					base.Play();
				}
				else if (!this.nosModule.active && this.wasActive)
				{
					base.Stop();
				}
			}
			this.wasActive = this.nosModule.active;
		}

		// Token: 0x060011D5 RID: 4565 RVA: 0x000C3214 File Offset: 0x000C1414
		public override void SetDefaults(VehicleController vc)
		{
			base.SetDefaults(vc);
			this.baseVolume = 0.2f;
			if (base.Clip == null)
			{
				base.Clip = (Resources.Load("NWH Vehicle Physics/Defaults/Sound/NOS") as AudioClip);
				if (base.Clip == null)
				{
					Debug.LogWarning("Audio Clip for sound component " + base.GetType().Name + "  from resources. Source will not play.");
				}
			}
		}

		// Token: 0x040021FE RID: 8702
		[NonSerialized]
		public NOSModule nosModule;

		// Token: 0x040021FF RID: 8703
		private bool wasActive;
	}
}
