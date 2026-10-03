using System;
using UnityEngine;

namespace SMPScripts
{
	// Token: 0x020001BA RID: 442
	[Serializable]
	public class EngineNote2
	{
		// Token: 0x06000AB8 RID: 2744 RVA: 0x0008F0E8 File Offset: 0x0008D2E8
		public float SetPitchAndGetVolumeForRPM(float rpm)
		{
			this.source.pitch = rpm / this.pitchReferenceRPM;
			if (rpm < this.minRPM || rpm > this.maxRPM)
			{
				return 0f;
			}
			if (rpm < this.peakRPM)
			{
				return Mathf.InverseLerp(this.minRPM, this.peakRPM, rpm);
			}
			return Mathf.InverseLerp(this.maxRPM, this.peakRPM, rpm);
		}

		// Token: 0x06000AB9 RID: 2745 RVA: 0x0008F150 File Offset: 0x0008D350
		public void SetVolume(float volume)
		{
			AudioSource audioSource = this.source;
			this.source.volume = volume;
			audioSource.mute = (volume == 0f);
		}

		// Token: 0x04001D0A RID: 7434
		public AudioSource source;

		// Token: 0x04001D0B RID: 7435
		public float minRPM;

		// Token: 0x04001D0C RID: 7436
		public float peakRPM;

		// Token: 0x04001D0D RID: 7437
		public float maxRPM;

		// Token: 0x04001D0E RID: 7438
		public float pitchReferenceRPM;
	}
}
