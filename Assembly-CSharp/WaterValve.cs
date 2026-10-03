using System;
using UnityEngine;

// Token: 0x0200015F RID: 351
public class WaterValve : MonoBehaviour
{
	// Token: 0x060008AE RID: 2222 RVA: 0x000709CD File Offset: 0x0006EBCD
	private void Start()
	{
		this.aSources = base.GetComponents<AudioSource>();
	}

	// Token: 0x060008AF RID: 2223 RVA: 0x000709DC File Offset: 0x0006EBDC
	public void TurnOnOff()
	{
		this.isOn = !this.isOn;
		if (this.isOn)
		{
			this.streamTrigger.SetActive(true);
			this.waterParticles.SetActive(true);
			this.aSources[1].Play();
			if (this.filling)
			{
				this.aSources[0].Play();
				return;
			}
		}
		else
		{
			this.streamTrigger.SetActive(false);
			this.waterParticles.SetActive(false);
			this.aSources[1].Stop();
			this.aSources[0].Stop();
		}
	}

	// Token: 0x060008B0 RID: 2224 RVA: 0x00070A70 File Offset: 0x0006EC70
	private void FixedUpdate()
	{
		if (this.filling && Time.time > (float)this.interval)
		{
			this.interval = Mathf.FloorToInt(Time.time) + 1;
			if (this.lid.isOpen)
			{
				if (!this.aSources[0].isPlaying)
				{
					this.aSources[0].Play();
				}
				if (this.tank.waterLevel < 1000f)
				{
					this.tank.waterLevel += 20f;
					return;
				}
				this.aSources[0].Stop();
				return;
			}
			else
			{
				this.aSources[0].Stop();
			}
		}
	}

	// Token: 0x060008B1 RID: 2225 RVA: 0x00070B19 File Offset: 0x0006ED19
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "water" && !this.filling)
		{
			this.filling = true;
		}
	}

	// Token: 0x060008B2 RID: 2226 RVA: 0x00070B3C File Offset: 0x0006ED3C
	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "water" && this.filling)
		{
			this.filling = false;
			this.aSources[0].Stop();
		}
	}

	// Token: 0x0400140D RID: 5133
	public GameObject streamTrigger;

	// Token: 0x0400140E RID: 5134
	public GameObject waterParticles;

	// Token: 0x0400140F RID: 5135
	private bool isOn;

	// Token: 0x04001410 RID: 5136
	public AudioSource[] aSources;

	// Token: 0x04001411 RID: 5137
	public bool filling;

	// Token: 0x04001412 RID: 5138
	public WaterTank tank;

	// Token: 0x04001413 RID: 5139
	private int interval = 1;

	// Token: 0x04001414 RID: 5140
	public InteractiveObject lid;
}
