using System;
using UnityEngine;

// Token: 0x020000B6 RID: 182
public class GardenHose2 : MonoBehaviour
{
	// Token: 0x06000452 RID: 1106 RVA: 0x0002ED38 File Offset: 0x0002CF38
	public void Toggle()
	{
		if (!this.inter.holdingHoseTank)
		{
			this.inter.holdingHoseTank = true;
			this.fpsNozzle.SetActive(true);
			this.fpsWaterParticles.SetActive(true);
			return;
		}
		this.inter.holdingHoseTank = false;
		this.fpsNozzle.SetActive(false);
		this.fpsWaterParticles.SetActive(false);
	}

	// Token: 0x06000453 RID: 1107 RVA: 0x0002ED9C File Offset: 0x0002CF9C
	public void FixedUpdate()
	{
		if (this.inter.holdingHoseTank && Time.time > (float)this.interval)
		{
			this.interval = Mathf.FloorToInt(Time.time) + 1;
			if (Vector3.Distance(this.fpsNozzle.transform.position, this.faucetTrans.position) > 10f)
			{
				this.inter.holdingHoseTank = false;
				this.fpsWaterParticles.SetActive(false);
				this.fpsNozzle.SetActive(false);
			}
			if (this.tank.waterLevel > 0f)
			{
				this.tank.waterLevel -= 4f;
				Debug.Log(this.tank.waterLevel);
				if (this.faucetHandle.isOpen)
				{
					this.fpsWaterParticles.SetActive(true);
					return;
				}
				this.fpsWaterParticles.SetActive(false);
				return;
			}
			else
			{
				this.fpsWaterParticles.SetActive(false);
			}
		}
	}

	// Token: 0x040008F6 RID: 2294
	public WaterTank tank;

	// Token: 0x040008F7 RID: 2295
	public InteractiveObject faucetHandle;

	// Token: 0x040008F8 RID: 2296
	public GameObject fpsWaterParticles;

	// Token: 0x040008F9 RID: 2297
	public GameObject fpsNozzle;

	// Token: 0x040008FA RID: 2298
	public Interactor inter;

	// Token: 0x040008FB RID: 2299
	private int interval = 2;

	// Token: 0x040008FC RID: 2300
	public Transform faucetTrans;
}
