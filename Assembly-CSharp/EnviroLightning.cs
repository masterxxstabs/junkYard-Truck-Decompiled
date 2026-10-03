using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200009A RID: 154
public class EnviroLightning : MonoBehaviour
{
	// Token: 0x06000359 RID: 857 RVA: 0x0001EDE0 File Offset: 0x0001CFE0
	public void Lightning()
	{
		base.StartCoroutine(this.LightningBolt());
	}

	// Token: 0x0600035A RID: 858 RVA: 0x0001EDEF File Offset: 0x0001CFEF
	public void StopLightning()
	{
		base.StopAllCoroutines();
		base.GetComponent<Light>().enabled = false;
		EnviroSkyMgr.instance.SetLightningFlashTrigger(0f);
	}

	// Token: 0x0600035B RID: 859 RVA: 0x0001EE12 File Offset: 0x0001D012
	public IEnumerator LightningBolt()
	{
		base.GetComponent<Light>().enabled = true;
		float defaultIntensity = base.GetComponent<Light>().intensity;
		int flashCount = Random.Range(2, 5);
		int num;
		for (int thisFlash = 0; thisFlash < flashCount; thisFlash = num + 1)
		{
			base.GetComponent<Light>().intensity = defaultIntensity * Random.Range(1f, 1.5f);
			EnviroSkyMgr.instance.SetLightningFlashTrigger(Random.Range(5f, 10f));
			yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
			base.GetComponent<Light>().intensity = defaultIntensity;
			EnviroSkyMgr.instance.SetLightningFlashTrigger(1f);
			num = thisFlash;
		}
		base.GetComponent<Light>().enabled = false;
		yield break;
	}
}
