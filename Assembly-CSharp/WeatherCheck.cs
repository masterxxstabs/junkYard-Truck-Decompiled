using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000164 RID: 356
public class WeatherCheck : MonoBehaviour
{
	// Token: 0x060008C6 RID: 2246 RVA: 0x00070FC0 File Offset: 0x0006F1C0
	private void FixedUpdate()
	{
		if (Time.time >= this.rainCheckInterval)
		{
			if (EnviroSky.instance.Weather.currentActiveWeatherPreset == this.LR || EnviroSky.instance.Weather.currentActiveWeatherPreset == this.HR)
			{
				this.carAudio.EnableRain(true);
				this.truckAudio.EnableRain(true);
				this.truck2Audio.EnableRain(true);
			}
			else
			{
				this.carAudio.EnableRain(false);
				this.truckAudio.EnableRain(false);
				this.truck2Audio.EnableRain(false);
			}
			this.rainCheckInterval = (float)(Mathf.FloorToInt(Time.time) + 40);
			if (EnviroSky.instance.Weather.lastActiveWeatherPreset == this.LR || EnviroSky.instance.Weather.lastActiveWeatherPreset == this.HR)
			{
				if (!this.newCheck)
				{
					return;
				}
				this.newCheck = false;
				using (IEnumerator enumerator = this.nodes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						Transform transform = (Transform)obj;
						if (transform.gameObject.name == "dirtcake")
						{
							this.active = Random.Range(1, 3);
							if (this.active == 1)
							{
								transform.gameObject.SetActive(true);
							}
						}
						if (transform.gameObject.name == "ore")
						{
							this.active = Random.Range(1, 3);
							if (this.active == 1)
							{
								transform.gameObject.SetActive(true);
							}
						}
					}
					return;
				}
			}
			this.newCheck = true;
		}
	}

	// Token: 0x04001428 RID: 5160
	public EnviroWeatherPreset LR;

	// Token: 0x04001429 RID: 5161
	public EnviroWeatherPreset HR;

	// Token: 0x0400142A RID: 5162
	public float rainCheckInterval;

	// Token: 0x0400142B RID: 5163
	public bool newCheck;

	// Token: 0x0400142C RID: 5164
	public Transform nodes;

	// Token: 0x0400142D RID: 5165
	private int active;

	// Token: 0x0400142E RID: 5166
	public AudioControlCar carAudio;

	// Token: 0x0400142F RID: 5167
	public AudioControl truckAudio;

	// Token: 0x04001430 RID: 5168
	public AudioControlF truck2Audio;
}
