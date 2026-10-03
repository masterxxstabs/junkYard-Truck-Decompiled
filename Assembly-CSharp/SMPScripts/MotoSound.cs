using System;
using UnityEngine;

namespace SMPScripts
{
	// Token: 0x020001BB RID: 443
	public class MotoSound : MonoBehaviour
	{
		// Token: 0x06000ABB RID: 2747 RVA: 0x0008F180 File Offset: 0x0008D380
		public void StartAudio()
		{
			this.revUp = base.GetComponents<AudioSource>()[4];
			this.suspension1 = base.GetComponents<AudioSource>()[5];
			base.GetComponents<AudioSource>()[0].Play();
			base.GetComponents<AudioSource>()[1].Play();
			base.GetComponents<AudioSource>()[2].Play();
		}

		// Token: 0x06000ABC RID: 2748 RVA: 0x0008F1D0 File Offset: 0x0008D3D0
		public void GetOff()
		{
			base.GetComponents<AudioSource>()[0].Stop();
			base.GetComponents<AudioSource>()[1].Stop();
			base.GetComponents<AudioSource>()[2].Stop();
			base.enabled = false;
		}

		// Token: 0x06000ABD RID: 2749 RVA: 0x0008F200 File Offset: 0x0008D400
		private void Update()
		{
			float num = 0f;
			for (int i = 0; i < this.engineNotes.Length; i++)
			{
				num += (MotoSound.workingVolumes[i] = this.engineNotes[i].SetPitchAndGetVolumeForRPM(this.rpm));
			}
			if (num > 0f)
			{
				for (int j = 0; j < this.engineNotes.Length; j++)
				{
					this.engineNotes[j].SetVolume(this.masterVolume * MotoSound.workingVolumes[j] / num);
				}
			}
			if (this.motoController.rawCustomAccelerationAxis > 0f && !this.motoController.isAirborne)
			{
				if (!this.revUpLimit)
				{
					this.revUpLimit = true;
				}
				this.revDownLimit = true;
				this.rpm = (this.motoController.engineSettings.gearRatio + (float)this.motoController.engineSettings.currentGear) * 0.1f + 0.5f + this.motoController.rb.velocity.magnitude * 0.05f;
				if (this.suspensionHit)
				{
					this.suspension1.Play();
					this.suspensionHit = false;
					return;
				}
			}
			else
			{
				if (this.motoController.isAirborne)
				{
					if (this.rpm > 0.5f)
					{
						this.rpm -= 0.01f;
					}
					if (this.revDownLimit)
					{
						this.revDownLimit = false;
					}
					this.suspensionHit = true;
					return;
				}
				this.revUpLimit = false;
				if (this.revDownLimit)
				{
					this.revDownLimit = false;
				}
				if (this.suspensionHit)
				{
					this.suspension1.Play();
					this.suspensionHit = false;
				}
				this.rpm = (this.motoController.engineSettings.gearRatio + (float)this.motoController.engineSettings.currentGear) * 0.1f + 0.5f + this.motoController.rb.velocity.magnitude * 0.05f;
			}
		}

		// Token: 0x04001D0F RID: 7439
		private AudioSource revDown;

		// Token: 0x04001D10 RID: 7440
		private AudioSource revUp;

		// Token: 0x04001D11 RID: 7441
		public AudioSource suspension1;

		// Token: 0x04001D12 RID: 7442
		public MotoController motoController;

		// Token: 0x04001D13 RID: 7443
		public EngineNote2[] engineNotes;

		// Token: 0x04001D14 RID: 7444
		public float rpm;

		// Token: 0x04001D15 RID: 7445
		public float masterVolume;

		// Token: 0x04001D16 RID: 7446
		private static float[] workingVolumes = new float[3];

		// Token: 0x04001D17 RID: 7447
		private bool revUpLimit;

		// Token: 0x04001D18 RID: 7448
		private bool revDownLimit;

		// Token: 0x04001D19 RID: 7449
		private bool suspensionHit;
	}
}
