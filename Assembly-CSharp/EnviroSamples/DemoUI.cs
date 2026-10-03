using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EnviroSamples
{
	// Token: 0x020002D2 RID: 722
	public class DemoUI : MonoBehaviour
	{
		// Token: 0x0600137A RID: 4986 RVA: 0x000CC1EB File Offset: 0x000CA3EB
		private void Start()
		{
			if (EnviroSkyMgr.instance == null || !EnviroSkyMgr.instance.IsAvailable())
			{
				base.enabled = false;
				return;
			}
			EnviroSkyMgr.instance.OnWeatherChanged += delegate(EnviroWeatherPreset type)
			{
				this.UpdateWeatherSlider();
			};
		}

		// Token: 0x0600137B RID: 4987 RVA: 0x000CC224 File Offset: 0x000CA424
		private IEnumerator setupDrodown()
		{
			this.started = true;
			yield return new WaitForSeconds(0.1f);
			for (int i = 0; i < EnviroSkyMgr.instance.GetCurrentWeatherPresetList().Count; i++)
			{
				Dropdown.OptionData optionData = new Dropdown.OptionData();
				optionData.text = EnviroSkyMgr.instance.GetCurrentWeatherPresetList()[i].Name;
				this.weatherDropdown.options.Add(optionData);
			}
			yield return new WaitForSeconds(0.1f);
			this.UpdateWeatherSlider();
			yield break;
		}

		// Token: 0x0600137C RID: 4988 RVA: 0x000CC233 File Offset: 0x000CA433
		public void ChangeTimeSlider()
		{
			if (this.sliderTime.value < 0f)
			{
				this.sliderTime.value = 0f;
			}
			EnviroSkyMgr.instance.SetTimeOfDay(this.sliderTime.value * 24f);
		}

		// Token: 0x0600137D RID: 4989 RVA: 0x000CC272 File Offset: 0x000CA472
		public void ChangeCloudQuality(int value)
		{
			EnviroSky.instance.ApplyVolumeCloudsQualityPreset(value);
		}

		// Token: 0x0600137E RID: 4990 RVA: 0x000CC27F File Offset: 0x000CA47F
		public void ChangeAmbientVolume(float value)
		{
			EnviroSkyMgr.instance.ambientAudioVolume = value;
		}

		// Token: 0x0600137F RID: 4991 RVA: 0x000CC28C File Offset: 0x000CA48C
		public void ChangeWeatherVolume(float value)
		{
			EnviroSkyMgr.instance.weatherAudioVolume = value;
		}

		// Token: 0x06001380 RID: 4992 RVA: 0x000CC299 File Offset: 0x000CA499
		public void SetWeatherID(int id)
		{
			EnviroSkyMgr.instance.ChangeWeather(id);
		}

		// Token: 0x06001381 RID: 4993 RVA: 0x000CC2A6 File Offset: 0x000CA4A6
		public void SetVolumeClouds(bool b)
		{
			EnviroSkyMgr.instance.useVolumeClouds = b;
		}

		// Token: 0x06001382 RID: 4994 RVA: 0x000CC2B3 File Offset: 0x000CA4B3
		public void SetVolumeLighting(bool b)
		{
			EnviroSkyMgr.instance.useVolumeLighting = b;
		}

		// Token: 0x06001383 RID: 4995 RVA: 0x000CC2C0 File Offset: 0x000CA4C0
		public void SetFlatClouds(bool b)
		{
			EnviroSkyMgr.instance.useFlatClouds = b;
		}

		// Token: 0x06001384 RID: 4996 RVA: 0x000CC2CD File Offset: 0x000CA4CD
		public void SetParticleClouds(bool b)
		{
			EnviroSkyMgr.instance.useParticleClouds = b;
		}

		// Token: 0x06001385 RID: 4997 RVA: 0x000CC2DA File Offset: 0x000CA4DA
		public void SetSunShafts(bool b)
		{
			EnviroSkyMgr.instance.useSunShafts = b;
		}

		// Token: 0x06001386 RID: 4998 RVA: 0x000CC2E7 File Offset: 0x000CA4E7
		public void SetMoonShafts(bool b)
		{
			EnviroSkyMgr.instance.useMoonShafts = b;
		}

		// Token: 0x06001387 RID: 4999 RVA: 0x000CC2F4 File Offset: 0x000CA4F4
		public void SetSeason(int id)
		{
			switch (id)
			{
			case 0:
				EnviroSkyMgr.instance.ChangeSeason(EnviroSeasons.Seasons.Spring);
				return;
			case 1:
				EnviroSkyMgr.instance.ChangeSeason(EnviroSeasons.Seasons.Summer);
				return;
			case 2:
				EnviroSkyMgr.instance.ChangeSeason(EnviroSeasons.Seasons.Autumn);
				return;
			case 3:
				EnviroSkyMgr.instance.ChangeSeason(EnviroSeasons.Seasons.Winter);
				return;
			default:
				return;
			}
		}

		// Token: 0x06001388 RID: 5000 RVA: 0x000CC347 File Offset: 0x000CA547
		public void SetTimeProgress(int id)
		{
			switch (id)
			{
			case 0:
				EnviroSkyMgr.instance.SetTimeProgress(EnviroTime.TimeProgressMode.None);
				return;
			case 1:
				EnviroSkyMgr.instance.SetTimeProgress(EnviroTime.TimeProgressMode.Simulated);
				return;
			case 2:
				EnviroSkyMgr.instance.SetTimeProgress(EnviroTime.TimeProgressMode.SystemTime);
				return;
			default:
				return;
			}
		}

		// Token: 0x06001389 RID: 5001 RVA: 0x000CC380 File Offset: 0x000CA580
		private void UpdateWeatherSlider()
		{
			if (EnviroSkyMgr.instance.GetCurrentWeatherPreset() != null)
			{
				for (int i = 0; i < this.weatherDropdown.options.Count; i++)
				{
					if (this.weatherDropdown.options[i].text == EnviroSkyMgr.instance.GetCurrentWeatherPreset().Name)
					{
						this.weatherDropdown.value = i;
					}
				}
			}
		}

		// Token: 0x0600138A RID: 5002 RVA: 0x000CC3F4 File Offset: 0x000CA5F4
		private void Update()
		{
			if (!EnviroSkyMgr.instance.IsStarted())
			{
				return;
			}
			if (!this.started)
			{
				base.StartCoroutine(this.setupDrodown());
			}
			this.timeText.text = EnviroSkyMgr.instance.GetTimeString();
			if (EnviroSkyMgr.instance.GetCurrentWeatherPreset() != null)
			{
				this.weatherText.text = EnviroSkyMgr.instance.GetCurrentWeatherPreset().Name;
			}
			this.temperatureText.text = EnviroSkyMgr.instance.GetCurrentTemperatureString();
		}

		// Token: 0x0600138B RID: 5003 RVA: 0x000CC479 File Offset: 0x000CA679
		private void LateUpdate()
		{
			this.sliderTime.value = EnviroSkyMgr.instance.GetTimeOfDay() / 24f;
		}

		// Token: 0x04002429 RID: 9257
		public Slider sliderTime;

		// Token: 0x0400242A RID: 9258
		private Slider sliderQuality;

		// Token: 0x0400242B RID: 9259
		public Text timeText;

		// Token: 0x0400242C RID: 9260
		public Text weatherText;

		// Token: 0x0400242D RID: 9261
		public Text temperatureText;

		// Token: 0x0400242E RID: 9262
		public Dropdown weatherDropdown;

		// Token: 0x0400242F RID: 9263
		private bool started;
	}
}
