using System;
using UnityEngine;

// Token: 0x020000AF RID: 175
public class Faucet : MonoBehaviour
{
	// Token: 0x0600043E RID: 1086 RVA: 0x0002CC1C File Offset: 0x0002AE1C
	public void TurnOnOff()
	{
		if (!this.isOn)
		{
			this.isOn = true;
			if (this.hoseAttach != null)
			{
				if (!this.hoseAttach.activeSelf)
				{
					this.waterCollider.SetActive(true);
					this.waterParticles.SetActive(true);
				}
				else
				{
					this.fpsWaterParticles.SetActive(true);
				}
			}
			else
			{
				this.waterCollider.SetActive(true);
				this.waterParticles.SetActive(true);
			}
			this.aSource.Play();
			return;
		}
		this.isOn = false;
		this.waterCollider.SetActive(false);
		this.waterParticles.SetActive(false);
		if (this.hoseAttach != null)
		{
			this.fpsWaterParticles.SetActive(false);
		}
		this.aSource.Stop();
	}

	// Token: 0x04000894 RID: 2196
	public GameObject waterCollider;

	// Token: 0x04000895 RID: 2197
	public GameObject waterParticles;

	// Token: 0x04000896 RID: 2198
	public bool isOn;

	// Token: 0x04000897 RID: 2199
	public AudioSource aSource;

	// Token: 0x04000898 RID: 2200
	public GameObject hoseAttach;

	// Token: 0x04000899 RID: 2201
	public GameObject fpsWaterParticles;
}
