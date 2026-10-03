using System;
using UnityEngine;

// Token: 0x020000B5 RID: 181
public class GardenHose : MonoBehaviour
{
	// Token: 0x06000450 RID: 1104 RVA: 0x0002EC98 File Offset: 0x0002CE98
	public void Toggle()
	{
		if (this.hoseAttach.activeSelf)
		{
			this.hoseAttach.SetActive(false);
			this.fpsNozzle.SetActive(false);
			this.inter.holdingHose = false;
			if (this.faucet.isOn)
			{
				this.faucet.TurnOnOff();
				return;
			}
		}
		else
		{
			this.hoseAttach.SetActive(true);
			this.fpsNozzle.SetActive(true);
			this.inter.holdingHose = true;
			if (this.faucet.isOn)
			{
				this.fpsWaterParticles.SetActive(true);
				this.waterParticles.SetActive(false);
			}
		}
	}

	// Token: 0x040008EE RID: 2286
	public GameObject waterCollider;

	// Token: 0x040008EF RID: 2287
	public GameObject waterParticles;

	// Token: 0x040008F0 RID: 2288
	public bool isOn;

	// Token: 0x040008F1 RID: 2289
	public GameObject hoseAttach;

	// Token: 0x040008F2 RID: 2290
	public GameObject fpsNozzle;

	// Token: 0x040008F3 RID: 2291
	public Interactor inter;

	// Token: 0x040008F4 RID: 2292
	public Faucet faucet;

	// Token: 0x040008F5 RID: 2293
	public GameObject fpsWaterParticles;
}
