using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200019D RID: 413
public class streetlights : MonoBehaviour
{
	// Token: 0x06000A14 RID: 2580 RVA: 0x0008A37C File Offset: 0x0008857C
	private void Start()
	{
		base.StartCoroutine(this.LightDelay());
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x0008A38C File Offset: 0x0008858C
	public void TurnOn()
	{
		foreach (Light light in this.lights)
		{
			if (light != null)
			{
				light.enabled = true;
			}
		}
		this.stationCanopy.material.EnableKeyword("_EMISSION");
		this.greatHorn.SetActive(true);
		if (this.jake.missionNum == 7)
		{
			this.jake.revengeCar.SetActive(true);
		}
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x0008A404 File Offset: 0x00088604
	public void TurnOff()
	{
		foreach (Light light in this.lights)
		{
			if (light != null)
			{
				light.enabled = false;
			}
		}
		this.stationCanopy.material.DisableKeyword("_EMISSION");
		this.greatHorn.SetActive(false);
		this.jake.revengeCar.SetActive(false);
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x0008A46C File Offset: 0x0008866C
	private IEnumerator LightDelay()
	{
		yield return new WaitForSeconds(3f);
		if ((EnviroSky.instance.internalHour >= 16f && EnviroSky.instance.internalHour < 24f) || (EnviroSky.instance.internalHour >= 0f && EnviroSky.instance.internalHour < 7f))
		{
			this.TurnOn();
		}
		yield break;
	}

	// Token: 0x04001BFE RID: 7166
	public Light[] lights;

	// Token: 0x04001BFF RID: 7167
	public GameObject greatHorn;

	// Token: 0x04001C00 RID: 7168
	public Jake jake;

	// Token: 0x04001C01 RID: 7169
	public Renderer stationCanopy;
}
