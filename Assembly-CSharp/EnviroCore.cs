using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000076 RID: 118
public class EnviroCore : MonoBehaviour
{
	// Token: 0x060001D4 RID: 468 RVA: 0x00012F1C File Offset: 0x0001111C
	public void UpdateEnviroment()
	{
		if (this.Seasons.calcSeasons)
		{
			this.UpdateSeason();
		}
		if (this.EnviroVegetationInstances.Count > 0)
		{
			for (int i = 0; i < this.EnviroVegetationInstances.Count; i++)
			{
				if (this.EnviroVegetationInstances[i] != null)
				{
					this.EnviroVegetationInstances[i].UpdateInstance();
				}
			}
		}
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x00012F88 File Offset: 0x00011188
	public void PlayAmbient(AudioClip sfx)
	{
		if (sfx == this.Audio.currentAmbientSource.audiosrc.clip)
		{
			if (!this.Audio.currentAmbientSource.audiosrc.isPlaying)
			{
				this.Audio.currentAmbientSource.audiosrc.Play();
			}
			return;
		}
		if (this.Audio.currentAmbientSource == this.Audio.AudioSourceAmbient)
		{
			this.Audio.AudioSourceAmbient.FadeOut();
			this.Audio.AudioSourceAmbient2.FadeIn(sfx);
			this.Audio.currentAmbientSource = this.Audio.AudioSourceAmbient2;
			return;
		}
		if (this.Audio.currentAmbientSource == this.Audio.AudioSourceAmbient2)
		{
			this.Audio.AudioSourceAmbient2.FadeOut();
			this.Audio.AudioSourceAmbient.FadeIn(sfx);
			this.Audio.currentAmbientSource = this.Audio.AudioSourceAmbient;
		}
	}

	// Token: 0x060001D6 RID: 470 RVA: 0x00013088 File Offset: 0x00011288
	public void TryPlayAmbientSFX()
	{
		if (this.Weather.currentActiveWeatherPreset == null)
		{
			return;
		}
		if (this.isNight)
		{
			switch (this.Seasons.currentSeasons)
			{
			case EnviroSeasons.Seasons.Spring:
				if (this.Weather.currentActiveWeatherPreset.SpringNightAmbient != null)
				{
					this.PlayAmbient(this.Weather.currentActiveWeatherPreset.SpringNightAmbient);
					return;
				}
				this.Audio.AudioSourceAmbient.FadeOut();
				this.Audio.AudioSourceAmbient2.FadeOut();
				return;
			case EnviroSeasons.Seasons.Summer:
				if (this.Weather.currentActiveWeatherPreset.SummerNightAmbient != null)
				{
					this.PlayAmbient(this.Weather.currentActiveWeatherPreset.SummerNightAmbient);
					return;
				}
				this.Audio.AudioSourceAmbient.FadeOut();
				this.Audio.AudioSourceAmbient2.FadeOut();
				return;
			case EnviroSeasons.Seasons.Autumn:
				if (this.Weather.currentActiveWeatherPreset.AutumnNightAmbient != null)
				{
					this.PlayAmbient(this.Weather.currentActiveWeatherPreset.AutumnNightAmbient);
					return;
				}
				this.Audio.AudioSourceAmbient.FadeOut();
				this.Audio.AudioSourceAmbient2.FadeOut();
				return;
			case EnviroSeasons.Seasons.Winter:
				if (this.Weather.currentActiveWeatherPreset.WinterNightAmbient != null)
				{
					this.PlayAmbient(this.Weather.currentActiveWeatherPreset.WinterNightAmbient);
					return;
				}
				this.Audio.AudioSourceAmbient.FadeOut();
				this.Audio.AudioSourceAmbient2.FadeOut();
				return;
			default:
				return;
			}
		}
		else
		{
			switch (this.Seasons.currentSeasons)
			{
			case EnviroSeasons.Seasons.Spring:
				if (this.Weather.currentActiveWeatherPreset.SpringDayAmbient != null)
				{
					this.PlayAmbient(this.Weather.currentActiveWeatherPreset.SpringDayAmbient);
					return;
				}
				this.Audio.AudioSourceAmbient.FadeOut();
				this.Audio.AudioSourceAmbient2.FadeOut();
				return;
			case EnviroSeasons.Seasons.Summer:
				if (this.Weather.currentActiveWeatherPreset.SummerDayAmbient != null)
				{
					this.PlayAmbient(this.Weather.currentActiveWeatherPreset.SummerDayAmbient);
					return;
				}
				this.Audio.AudioSourceAmbient.FadeOut();
				this.Audio.AudioSourceAmbient2.FadeOut();
				return;
			case EnviroSeasons.Seasons.Autumn:
				if (this.Weather.currentActiveWeatherPreset.AutumnDayAmbient != null)
				{
					this.PlayAmbient(this.Weather.currentActiveWeatherPreset.AutumnDayAmbient);
					return;
				}
				this.Audio.AudioSourceAmbient.FadeOut();
				this.Audio.AudioSourceAmbient2.FadeOut();
				return;
			case EnviroSeasons.Seasons.Winter:
				if (this.Weather.currentActiveWeatherPreset.WinterDayAmbient != null)
				{
					this.PlayAmbient(this.Weather.currentActiveWeatherPreset.WinterDayAmbient);
					return;
				}
				this.Audio.AudioSourceAmbient.FadeOut();
				this.Audio.AudioSourceAmbient2.FadeOut();
				return;
			default:
				return;
			}
		}
	}

	// Token: 0x060001D7 RID: 471 RVA: 0x0001337C File Offset: 0x0001157C
	public void CreateWeatherEffectHolder(string name)
	{
		if (this.Weather.VFXHolder == null)
		{
			this.Weather.VFXHolder = GameObject.Find(name + "/VFX");
		}
		if (this.Weather.VFXHolder == null)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "VFX";
			gameObject.transform.parent = this.EffectsHolder.transform;
			gameObject.transform.localPosition = Vector3.zero;
			this.Weather.VFXHolder = gameObject;
		}
	}

	// Token: 0x060001D8 RID: 472 RVA: 0x00013410 File Offset: 0x00011610
	public void CreateEffects(string name)
	{
		if (this.EffectsHolder == null)
		{
			this.EffectsHolder = GameObject.Find(name);
		}
		if (this.EffectsHolder == null)
		{
			this.EffectsHolder = new GameObject();
			this.EffectsHolder.name = name;
			this.EffectsHolder.transform.parent = base.transform;
			this.EffectsHolder.transform.parent = null;
		}
		this.CreateWeatherEffectHolder(name);
		if (Application.isPlaying && EnviroSkyMgr.instance.dontDestroy)
		{
			Object.DontDestroyOnLoad(this.EffectsHolder);
		}
		if (this.Player != null)
		{
			this.EffectsHolder.transform.position = this.Player.transform.position;
		}
		else
		{
			this.EffectsHolder.transform.position = base.transform.position;
		}
		GameObject gameObject = GameObject.Find(name + "/SFX Holder(Clone)");
		if (gameObject == null)
		{
			gameObject = Object.Instantiate<GameObject>(this.Audio.SFXHolderPrefab, Vector3.zero, Quaternion.identity);
			gameObject.transform.parent = this.EffectsHolder.transform;
		}
		EnviroAudioSource[] componentsInChildren = gameObject.GetComponentsInChildren<EnviroAudioSource>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			switch (componentsInChildren[i].myFunction)
			{
			case EnviroAudioSource.AudioSourceFunction.Weather1:
				this.Audio.AudioSourceWeather = componentsInChildren[i];
				break;
			case EnviroAudioSource.AudioSourceFunction.Weather2:
				this.Audio.AudioSourceWeather2 = componentsInChildren[i];
				break;
			case EnviroAudioSource.AudioSourceFunction.Ambient:
				this.Audio.AudioSourceAmbient = componentsInChildren[i];
				break;
			case EnviroAudioSource.AudioSourceFunction.Ambient2:
				this.Audio.AudioSourceAmbient2 = componentsInChildren[i];
				break;
			case EnviroAudioSource.AudioSourceFunction.Thunder:
				this.Audio.AudioSourceThunder = componentsInChildren[i];
				break;
			case EnviroAudioSource.AudioSourceFunction.ZoneAmbient:
				this.Audio.AudioSourceZone = componentsInChildren[i];
				break;
			}
		}
		this.Weather.currentAudioSource = this.Audio.AudioSourceWeather;
		this.Audio.currentAmbientSource = this.Audio.AudioSourceAmbient;
		this.TryPlayAmbientSFX();
	}

	// Token: 0x060001D9 RID: 473 RVA: 0x00013614 File Offset: 0x00011814
	public Transform CreateDirectionalLight(bool additional)
	{
		GameObject gameObject = new GameObject();
		if (!additional)
		{
			gameObject.name = "Enviro Directional Light";
		}
		else
		{
			gameObject.name = "Enviro Directional Light - Moon";
		}
		gameObject.transform.parent = base.transform;
		gameObject.transform.parent = null;
		Light light = gameObject.AddComponent<Light>();
		light.type = LightType.Directional;
		light.shadows = LightShadows.Soft;
		return gameObject.transform;
	}

	// Token: 0x060001DA RID: 474 RVA: 0x00013678 File Offset: 0x00011878
	public Vector3 BetaRay(Vector3 waveLength)
	{
		Vector3 vector = waveLength * 1E-09f;
		Vector3 result;
		result.x = 8f * Mathf.Pow(3.1415927f, 3f) * Mathf.Pow(Mathf.Pow(1.0003f, 2f) - 1f, 2f) * 6.105f / (7.635E+25f * Mathf.Pow(vector.x, 4f) * 5.755f) * 2000f;
		result.y = 8f * Mathf.Pow(3.1415927f, 3f) * Mathf.Pow(Mathf.Pow(1.0003f, 2f) - 1f, 2f) * 6.105f / (7.635E+25f * Mathf.Pow(vector.y, 4f) * 5.755f) * 2000f;
		result.z = 8f * Mathf.Pow(3.1415927f, 3f) * Mathf.Pow(Mathf.Pow(1.0003f, 2f) - 1f, 2f) * 6.105f / (7.635E+25f * Mathf.Pow(vector.z, 4f) * 5.755f) * 2000f;
		return result;
	}

	// Token: 0x060001DB RID: 475 RVA: 0x000137C4 File Offset: 0x000119C4
	public Vector3 BetaMie(float turbidity, Vector3 waveLength)
	{
		float num = 0.2f * turbidity * 10f;
		Vector3 vector;
		vector.x = 434f * num * 3.1415927f * Mathf.Pow(6.2831855f / waveLength.x, 2f) * this.K.x;
		vector.y = 434f * num * 3.1415927f * Mathf.Pow(6.2831855f / waveLength.y, 2f) * this.K.y;
		vector.z = 434f * num * 3.1415927f * Mathf.Pow(6.2831855f / waveLength.z, 2f) * this.K.z;
		vector.x = Mathf.Pow(vector.x, -1f);
		vector.y = Mathf.Pow(vector.y, -1f);
		vector.z = Mathf.Pow(vector.z, -1f);
		return vector;
	}

	// Token: 0x060001DC RID: 476 RVA: 0x000138CA File Offset: 0x00011ACA
	public Vector3 GetMieG(float g)
	{
		if (g == 1f)
		{
			g = 0.99f;
		}
		return new Vector3(1f - g * g, 1f + g * g, 2f * g);
	}

	// Token: 0x060001DD RID: 477 RVA: 0x000138CA File Offset: 0x00011ACA
	public Vector3 GetMieGScene(float g)
	{
		if (g == 1f)
		{
			g = 0.99f;
		}
		return new Vector3(1f - g * g, 1f + g * g, 2f * g);
	}

	// Token: 0x060001DE RID: 478 RVA: 0x000138FC File Offset: 0x00011AFC
	public void UpdateTime(int daysInYear)
	{
		if (Application.isPlaying)
		{
			float num;
			if (!this.isNight)
			{
				num = 0.4f / this.GameTime.DayLengthInMinutes;
			}
			else
			{
				num = 0.4f / this.GameTime.NightLengthInMinutes;
			}
			this.hourTime = num * Time.deltaTime;
			switch (this.GameTime.ProgressTime)
			{
			case EnviroTime.TimeProgressMode.None:
				this.SetTime(this.GameTime.Years, this.GameTime.Days, this.GameTime.Hours, this.GameTime.Minutes, this.GameTime.Seconds);
				break;
			case EnviroTime.TimeProgressMode.Simulated:
				this.internalHour += this.hourTime;
				this.SetGameTime();
				break;
			case EnviroTime.TimeProgressMode.OneDay:
				this.internalHour += this.hourTime;
				this.SetGameTime();
				break;
			case EnviroTime.TimeProgressMode.SystemTime:
				this.SetTime(DateTime.Now);
				break;
			}
		}
		else
		{
			this.SetTime(this.GameTime.Years, this.GameTime.Days, this.GameTime.Hours, this.GameTime.Minutes, this.GameTime.Seconds);
		}
		if (this.internalHour > this.lastHourUpdate + 1f)
		{
			this.lastHourUpdate = this.internalHour;
			EnviroSkyMgr.instance.NotifyHourPassed();
		}
		if (this.GameTime.Days >= daysInYear)
		{
			this.GameTime.Years = this.GameTime.Years + 1;
			this.GameTime.Days = 0;
			EnviroSkyMgr.instance.NotifyYearPassed();
		}
		this.currentHour = this.internalHour;
		this.currentDay = (float)this.GameTime.Days;
		this.currentYear = (float)this.GameTime.Years;
		this.currentTimeInHours = this.GetInHours(this.internalHour, this.currentDay, this.currentYear, daysInYear);
	}

	// Token: 0x060001DF RID: 479 RVA: 0x00013AF4 File Offset: 0x00011CF4
	public void SetInternalTime(int year, int dayOfYear, int hour, int minute, int seconds)
	{
		this.GameTime.Years = year;
		this.GameTime.Days = dayOfYear;
		this.GameTime.Minutes = minute;
		this.GameTime.Hours = hour;
		this.internalHour = (float)hour + (float)minute * 0.0166667f + (float)seconds * 0.000277778f;
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x00013B50 File Offset: 0x00011D50
	public void SetGameTime()
	{
		if (this.internalHour >= 24f)
		{
			this.internalHour -= 24f;
			EnviroSkyMgr.instance.NotifyHourPassed();
			this.lastHourUpdate = this.internalHour;
			if (this.GameTime.ProgressTime != EnviroTime.TimeProgressMode.OneDay)
			{
				this.GameTime.Days = this.GameTime.Days + 1;
				EnviroSkyMgr.instance.NotifyDayPassed();
			}
		}
		else if (this.internalHour < 0f)
		{
			this.internalHour = 24f + this.internalHour;
			this.lastHourUpdate = this.internalHour;
			if (this.GameTime.ProgressTime != EnviroTime.TimeProgressMode.OneDay)
			{
				this.GameTime.Days = this.GameTime.Days - 1;
				EnviroSkyMgr.instance.NotifyDayPassed();
			}
		}
		float num = this.internalHour;
		this.GameTime.Hours = (int)num;
		num -= (float)this.GameTime.Hours;
		this.GameTime.Minutes = (int)(num * 60f);
		num -= (float)this.GameTime.Minutes * 0.0166667f;
		this.GameTime.Seconds = (int)(num * 3600f);
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x00013C80 File Offset: 0x00011E80
	public void SetTime(DateTime date)
	{
		this.GameTime.Years = date.Year;
		this.GameTime.Days = date.DayOfYear;
		this.GameTime.Minutes = date.Minute;
		this.GameTime.Seconds = date.Second;
		this.GameTime.Hours = date.Hour;
		this.internalHour = (float)date.Hour + (float)date.Minute * 0.0166667f + (float)date.Second * 0.000277778f;
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x00013D13 File Offset: 0x00011F13
	public void ResetHourEventTimer()
	{
		this.lastHourUpdate = this.internalHour;
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x00013D24 File Offset: 0x00011F24
	public void SetTime(int year, int dayOfYear, int hour, int minute, int seconds)
	{
		this.GameTime.Years = year;
		this.GameTime.Days = dayOfYear;
		this.GameTime.Minutes = minute;
		this.GameTime.Hours = hour;
		this.internalHour = (float)hour + (float)minute * 0.0166667f + (float)seconds * 0.000277778f;
	}

	// Token: 0x060001E4 RID: 484 RVA: 0x00013D80 File Offset: 0x00011F80
	public void SetInternalTimeOfDay(float inHours)
	{
		this.internalHour = inHours;
		this.GameTime.Hours = (int)inHours;
		inHours -= (float)this.GameTime.Hours;
		this.GameTime.Minutes = (int)(inHours * 60f);
		inHours -= (float)this.GameTime.Minutes * 0.0166667f;
		this.GameTime.Seconds = (int)(inHours * 3600f);
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x00013DED File Offset: 0x00011FED
	public string GetTimeStringWithSeconds()
	{
		return string.Format("{0:00}:{1:00}:{2:00}", this.GameTime.Hours, this.GameTime.Minutes, this.GameTime.Seconds);
	}

	// Token: 0x060001E6 RID: 486 RVA: 0x00013E29 File Offset: 0x00012029
	public string GetTimeString()
	{
		return string.Format("{0:00}:{1:00}", this.GameTime.Hours, this.GameTime.Minutes);
	}

	// Token: 0x060001E7 RID: 487 RVA: 0x00013E58 File Offset: 0x00012058
	public DateTime CreateSystemDate()
	{
		return default(DateTime).AddYears(this.GameTime.Years - 1).AddDays((double)(this.GameTime.Days - 1));
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x00013E99 File Offset: 0x00012099
	public float Remap(float value, float from1, float to1, float from2, float to2)
	{
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x00013EAC File Offset: 0x000120AC
	public Vector3 OrbitalToLocal(float theta, float phi)
	{
		float num = Mathf.Sin(theta);
		float y = Mathf.Cos(theta);
		float num2 = Mathf.Sin(phi);
		float num3 = Mathf.Cos(phi);
		Vector3 result;
		result.z = num * num3;
		result.y = y;
		result.x = num * num2;
		return result;
	}

	// Token: 0x060001EA RID: 490 RVA: 0x00013EF4 File Offset: 0x000120F4
	public void CalculateSunPosition(float d, float ecl, bool simpleMoon)
	{
		float num = 282.9404f + 4.70935E-05f * d;
		float num2 = 0.016709f - 1.151E-09f * d;
		float num3 = 356.047f + 0.98560023f * d;
		float num4 = num3 + num2 * 57.29578f * Mathf.Sin(0.017453292f * num3) * (1f + num2 * Mathf.Cos(0.017453292f * num3));
		float num5 = Mathf.Cos(0.017453292f * num4) - num2;
		float num6 = Mathf.Sin(0.017453292f * num4) * Mathf.Sqrt(1f - num2 * num2);
		float num7 = 57.29578f * Mathf.Atan2(num6, num5);
		float num8 = Mathf.Sqrt(num5 * num5 + num6 * num6);
		float num9 = num7 + num;
		float num10 = num8 * Mathf.Cos(0.017453292f * num9);
		float num11 = num8 * Mathf.Sin(0.017453292f * num9);
		float num12 = num10;
		float num13 = num11 * Mathf.Cos(0.017453292f * ecl);
		float f = Mathf.Atan2(num11 * Mathf.Sin(0.017453292f * ecl), Mathf.Sqrt(num12 * num12 + num13 * num13));
		float num14 = Mathf.Sin(f);
		float num15 = Mathf.Cos(f);
		float num16 = num9 + 180f + this.GetUniversalTimeOfDay() * 15f;
		this.LST = num16 + this.GameTime.Longitude;
		float num17 = this.LST - 57.29578f * Mathf.Atan2(num13, num12);
		float f2 = 0.017453292f * num17;
		float num18 = Mathf.Sin(f2);
		float num19 = Mathf.Cos(f2) * num15;
		float num20 = num18 * num15;
		float num21 = num14;
		float num22 = Mathf.Sin(0.017453292f * this.GameTime.Latitude);
		float num23 = Mathf.Cos(0.017453292f * this.GameTime.Latitude);
		float num24 = num19 * num22 - num21 * num23;
		float num25 = num20;
		float y = num19 * num23 + num21 * num22;
		float num26 = Mathf.Atan2(num25, num24) + 3.1415927f;
		float num27 = Mathf.Atan2(y, Mathf.Sqrt(num24 * num24 + num25 * num25));
		float num28 = 1.5707964f - num27;
		float phi = num26;
		this.GameTime.solarTime = Mathf.Clamp01(this.Remap(num28, -1.5f, 0f, 1.5f, 1f));
		this.Components.Sun.transform.localPosition = this.OrbitalToLocal(num28, phi);
		this.Components.Sun.transform.LookAt(base.transform.position);
		if (simpleMoon)
		{
			this.Components.Moon.transform.localPosition = this.OrbitalToLocal(num28 - 3.1415927f, phi);
			this.GameTime.lunarTime = Mathf.Clamp01(this.Remap(num28 - 3.1415927f, -3f, 0f, 0f, 1f));
			this.Components.Moon.transform.LookAt(base.transform.position);
		}
	}

	// Token: 0x060001EB RID: 491 RVA: 0x000141DC File Offset: 0x000123DC
	public void CalculateMoonPosition(float d, float ecl)
	{
		float num = 125.1228f - 0.05295381f * d;
		float num2 = 5.1454f;
		float num3 = 318.0634f + 0.16435732f * d;
		float num4 = 60.2666f;
		float num5 = 0.0549f;
		float num6 = 115.3654f + 13.064993f * d;
		float num7 = 0.017453292f * num6;
		float f = num7 + num5 * Mathf.Sin(num7) * (1f + num5 * Mathf.Cos(num7));
		float num8 = num4 * (Mathf.Cos(f) - num5);
		float num9 = num4 * (Mathf.Sqrt(1f - num5 * num5) * Mathf.Sin(f));
		float num10 = 57.29578f * Mathf.Atan2(num9, num8);
		float num11 = Mathf.Sqrt(num8 * num8 + num9 * num9);
		float f2 = 0.017453292f * num;
		float num12 = Mathf.Sin(f2);
		float num13 = Mathf.Cos(f2);
		float f3 = 0.017453292f * (num10 + num3);
		float num14 = Mathf.Sin(f3);
		float num15 = Mathf.Cos(f3);
		float f4 = 0.017453292f * num2;
		float num16 = Mathf.Cos(f4);
		float num17 = num11 * (num13 * num15 - num12 * num14 * num16);
		float num18 = num11 * (num12 * num15 + num13 * num14 * num16);
		float num19 = num11 * (num14 * Mathf.Sin(f4));
		float num20 = Mathf.Cos(0.017453292f * ecl);
		float num21 = Mathf.Sin(0.017453292f * ecl);
		float num22 = num17;
		float num23 = num18 * num20 - num19 * num21;
		float y = num18 * num21 + num19 * num20;
		float num24 = Mathf.Atan2(num23, num22);
		float f5 = Mathf.Atan2(y, Mathf.Sqrt(num22 * num22 + num23 * num23));
		float f6 = 0.017453292f * this.LST - num24;
		float num25 = Mathf.Cos(f6) * Mathf.Cos(f5);
		float num26 = Mathf.Sin(f6) * Mathf.Cos(f5);
		float num27 = Mathf.Sin(f5);
		float f7 = 0.017453292f * this.GameTime.Latitude;
		float num28 = Mathf.Sin(f7);
		float num29 = Mathf.Cos(f7);
		float num30 = num25 * num28 - num27 * num29;
		float num31 = num26;
		float y2 = num25 * num29 + num27 * num28;
		float num32 = Mathf.Atan2(num31, num30) + 3.1415927f;
		float num33 = Mathf.Atan2(y2, Mathf.Sqrt(num30 * num30 + num31 * num31));
		float num34 = 1.5707964f - num33;
		float phi = num32;
		this.Components.Moon.transform.localPosition = this.OrbitalToLocal(num34, phi);
		this.GameTime.lunarTime = Mathf.Clamp01(this.Remap(num34, -1.5f, 0f, 1.5f, 1f));
		this.Components.Moon.transform.LookAt(base.transform.position);
	}

	// Token: 0x060001EC RID: 492 RVA: 0x00014474 File Offset: 0x00012674
	public Vector3 UpdateSatellitePosition(float orbit, float orbit2, float speed)
	{
		float f = 0.017453292f * this.GameTime.Latitude;
		float num = Mathf.Sin(f);
		float num2 = Mathf.Cos(f);
		float num3 = 0.017453292f * this.GameTime.Longitude;
		float f2 = orbit2 * Mathf.Sin(0.017073873f * ((float)this.GameTime.Days - 81f));
		float num4 = Mathf.Sin(f2);
		float num5 = Mathf.Cos(f2);
		float num6 = (float)((int)(this.GameTime.Longitude / 15f));
		float num7 = 0.2617994f * num6;
		float num8 = this.GetUniversalTimeOfDay() + orbit * Mathf.Sin(0.03333255f * ((float)this.GameTime.Days - 80f)) - speed * Mathf.Sin(0.008849557f * ((float)this.GameTime.Days - 8f)) + 3.8197186f * (num7 - num3);
		float f3 = 0.2617994f * num8;
		float num9 = Mathf.Sin(f3);
		float num10 = Mathf.Cos(f3);
		float num11 = Mathf.Asin(num * num4 - num2 * num5 * num10);
		float y = -num5 * num9;
		float x = num2 * num4 - num * num5 * num10;
		float num12 = Mathf.Atan2(y, x);
		float theta = 1.5707964f - num11;
		float phi = num12;
		return this.OrbitalToLocal(theta, phi);
	}

	// Token: 0x060001ED RID: 493 RVA: 0x000145AC File Offset: 0x000127AC
	public void CalculateStarsPosition(float siderealTime)
	{
		if (siderealTime > 24f)
		{
			siderealTime -= 24f;
		}
		else if (siderealTime < 0f)
		{
			siderealTime += 24f;
		}
		Quaternion quaternion = Quaternion.Euler(90f - this.GameTime.Latitude, 0.017453292f * this.GameTime.Longitude, 0f);
		quaternion *= Quaternion.Euler(0f, siderealTime, 0f);
		this.Components.starsRotation.localRotation = quaternion;
		Shader.SetGlobalMatrix("_StarsMatrix", this.Components.starsRotation.worldToLocalMatrix);
	}

	// Token: 0x060001EE RID: 494 RVA: 0x0001464C File Offset: 0x0001284C
	public void UpdateSunAndMoonPosition()
	{
		DateTime dateTime = this.CreateSystemDate();
		float num = (float)(367 * dateTime.Year - 7 * (dateTime.Year + (dateTime.Month / 12 + 9) / 12) / 4 + 275 * dateTime.Month / 9 + dateTime.Day - 730530);
		num += this.GetUniversalTimeOfDay() / 24f;
		float ecl = 23.4393f - 3.563E-07f * num;
		if (this.skySettings.sunAndMoonPosition == EnviroSkySettings.SunAndMoonCalc.Realistic)
		{
			this.CalculateSunPosition(num, ecl, false);
			this.CalculateMoonPosition(num, ecl);
		}
		else
		{
			this.CalculateSunPosition(num, ecl, true);
		}
		this.CalculateStarsPosition(this.LST);
	}

	// Token: 0x060001EF RID: 495 RVA: 0x000146FE File Offset: 0x000128FE
	public float GetUniversalTimeOfDay()
	{
		return this.internalHour - (float)this.GameTime.utcOffset;
	}

	// Token: 0x060001F0 RID: 496 RVA: 0x00014713 File Offset: 0x00012913
	public float GetTimeOfDay()
	{
		return this.internalHour;
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x0001471B File Offset: 0x0001291B
	public double GetInHours(float hours, float days, float years, int daysInYear)
	{
		return (double)(hours + days * 24f + years * (float)daysInYear * 24f);
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x00014734 File Offset: 0x00012934
	public void UpdateSeason()
	{
		if (this.currentDay >= (float)this.seasonsSettings.SpringStart && this.currentDay <= (float)this.seasonsSettings.SpringEnd)
		{
			this.ChangeSeason(EnviroSeasons.Seasons.Spring);
			return;
		}
		if (this.currentDay >= (float)this.seasonsSettings.SummerStart && this.currentDay <= (float)this.seasonsSettings.SummerEnd)
		{
			this.ChangeSeason(EnviroSeasons.Seasons.Summer);
			return;
		}
		if (this.currentDay >= (float)this.seasonsSettings.AutumnStart && this.currentDay <= (float)this.seasonsSettings.AutumnEnd)
		{
			this.ChangeSeason(EnviroSeasons.Seasons.Autumn);
			return;
		}
		if (this.currentDay >= (float)this.seasonsSettings.WinterStart || this.currentDay <= (float)this.seasonsSettings.WinterEnd)
		{
			this.ChangeSeason(EnviroSeasons.Seasons.Winter);
		}
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x00014800 File Offset: 0x00012A00
	public void ChangeSeason(EnviroSeasons.Seasons season)
	{
		if (this.Seasons.lastSeason != season)
		{
			EnviroSkyMgr.instance.NotifySeasonChanged(season);
			this.Seasons.lastSeason = this.Seasons.currentSeasons;
			this.Seasons.currentSeasons = season;
		}
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x00014840 File Offset: 0x00012A40
	public void ApplyProfile(EnviroProfile p)
	{
		this.profile = p;
		this.lightSettings = JsonUtility.FromJson<EnviroLightSettings>(JsonUtility.ToJson(p.lightSettings));
		this.volumeLightSettings = JsonUtility.FromJson<EnviroVolumeLightingSettings>(JsonUtility.ToJson(p.volumeLightSettings));
		this.distanceBlurSettings = JsonUtility.FromJson<EnviroDistanceBlurSettings>(JsonUtility.ToJson(p.distanceBlurSettings));
		this.skySettings = JsonUtility.FromJson<EnviroSkySettings>(JsonUtility.ToJson(p.skySettings));
		this.cloudsSettings = JsonUtility.FromJson<EnviroCloudSettings>(JsonUtility.ToJson(p.cloudsSettings));
		this.weatherSettings = JsonUtility.FromJson<EnviroWeatherSettings>(JsonUtility.ToJson(p.weatherSettings));
		this.fogSettings = JsonUtility.FromJson<EnviroFogSettings>(JsonUtility.ToJson(p.fogSettings));
		this.lightshaftsSettings = JsonUtility.FromJson<EnviroLightShaftsSettings>(JsonUtility.ToJson(p.lightshaftsSettings));
		this.audioSettings = JsonUtility.FromJson<EnviroAudioSettings>(JsonUtility.ToJson(p.audioSettings));
		this.satelliteSettings = JsonUtility.FromJson<EnviroSatellitesSettings>(JsonUtility.ToJson(p.satelliteSettings));
		this.qualitySettings = JsonUtility.FromJson<EnviroQualitySettings>(JsonUtility.ToJson(p.qualitySettings));
		this.seasonsSettings = JsonUtility.FromJson<EnviroSeasonSettings>(JsonUtility.ToJson(p.seasonsSettings));
		this.reflectionSettings = JsonUtility.FromJson<EnviroReflectionSettings>(JsonUtility.ToJson(p.reflectionSettings));
		this.auroraSettings = JsonUtility.FromJson<EnviroAuroraSettings>(JsonUtility.ToJson(p.auroraSettings));
		this.profileLoaded = true;
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x00014990 File Offset: 0x00012B90
	public void SaveProfile()
	{
		this.profile.lightSettings = JsonUtility.FromJson<EnviroLightSettings>(JsonUtility.ToJson(this.lightSettings));
		this.profile.volumeLightSettings = JsonUtility.FromJson<EnviroVolumeLightingSettings>(JsonUtility.ToJson(this.volumeLightSettings));
		this.profile.distanceBlurSettings = JsonUtility.FromJson<EnviroDistanceBlurSettings>(JsonUtility.ToJson(this.distanceBlurSettings));
		this.profile.skySettings = JsonUtility.FromJson<EnviroSkySettings>(JsonUtility.ToJson(this.skySettings));
		this.profile.cloudsSettings = JsonUtility.FromJson<EnviroCloudSettings>(JsonUtility.ToJson(this.cloudsSettings));
		this.profile.weatherSettings = JsonUtility.FromJson<EnviroWeatherSettings>(JsonUtility.ToJson(this.weatherSettings));
		this.profile.fogSettings = JsonUtility.FromJson<EnviroFogSettings>(JsonUtility.ToJson(this.fogSettings));
		this.profile.lightshaftsSettings = JsonUtility.FromJson<EnviroLightShaftsSettings>(JsonUtility.ToJson(this.lightshaftsSettings));
		this.profile.audioSettings = JsonUtility.FromJson<EnviroAudioSettings>(JsonUtility.ToJson(this.audioSettings));
		this.profile.satelliteSettings = JsonUtility.FromJson<EnviroSatellitesSettings>(JsonUtility.ToJson(this.satelliteSettings));
		this.profile.qualitySettings = JsonUtility.FromJson<EnviroQualitySettings>(JsonUtility.ToJson(this.qualitySettings));
		this.profile.seasonsSettings = JsonUtility.FromJson<EnviroSeasonSettings>(JsonUtility.ToJson(this.seasonsSettings));
		this.profile.reflectionSettings = JsonUtility.FromJson<EnviroReflectionSettings>(JsonUtility.ToJson(this.reflectionSettings));
		this.profile.auroraSettings = JsonUtility.FromJson<EnviroAuroraSettings>(JsonUtility.ToJson(this.auroraSettings));
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x00014B18 File Offset: 0x00012D18
	public void UpdateReflections()
	{
		if (this.Components.GlobalReflectionProbe == null)
		{
			Debug.Log("Global Reflection Probe not assigned in 'Components' menu of Enviro Sky Instance!");
			return;
		}
		if (EnviroSkyMgr.instance != null && EnviroSkyMgr.instance.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			this.Components.GlobalReflectionProbe.customRendering = this.reflectionSettings.globalReflectionCustomRendering;
			if (this.reflectionSettings.reflectionCloudsQuality != null)
			{
				this.Components.GlobalReflectionProbe.customCloudsQuality = this.reflectionSettings.reflectionCloudsQuality;
			}
			this.Components.GlobalReflectionProbe.useFog = this.reflectionSettings.globalReflectionUseFog;
		}
		if (!this.reflectionSettings.globalReflections)
		{
			this.Components.GlobalReflectionProbe.enabled = false;
			return;
		}
		if (!this.Components.GlobalReflectionProbe.isActiveAndEnabled)
		{
			this.Components.GlobalReflectionProbe.enabled = true;
		}
		this.Components.GlobalReflectionProbe.myProbe.cullingMask = this.reflectionSettings.globalReflectionLayers;
		this.Components.GlobalReflectionProbe.myProbe.intensity = this.reflectionSettings.globalReflectionsIntensity;
		this.Components.GlobalReflectionProbe.myProbe.size = base.transform.localScale * this.reflectionSettings.globalReflectionsScale;
		switch (this.reflectionSettings.globalReflectionResolution)
		{
		case EnviroReflectionSettings.GlobalReflectionResolution.R16:
			this.Components.GlobalReflectionProbe.myProbe.resolution = 16;
			break;
		case EnviroReflectionSettings.GlobalReflectionResolution.R32:
			this.Components.GlobalReflectionProbe.myProbe.resolution = 32;
			break;
		case EnviroReflectionSettings.GlobalReflectionResolution.R64:
			this.Components.GlobalReflectionProbe.myProbe.resolution = 64;
			break;
		case EnviroReflectionSettings.GlobalReflectionResolution.R128:
			this.Components.GlobalReflectionProbe.myProbe.resolution = 128;
			break;
		case EnviroReflectionSettings.GlobalReflectionResolution.R256:
			this.Components.GlobalReflectionProbe.myProbe.resolution = 256;
			break;
		case EnviroReflectionSettings.GlobalReflectionResolution.R512:
			this.Components.GlobalReflectionProbe.myProbe.resolution = 512;
			break;
		case EnviroReflectionSettings.GlobalReflectionResolution.R1024:
			this.Components.GlobalReflectionProbe.myProbe.resolution = 1024;
			break;
		case EnviroReflectionSettings.GlobalReflectionResolution.R2048:
			this.Components.GlobalReflectionProbe.myProbe.resolution = 2048;
			break;
		}
		if ((this.currentTimeInHours > this.lastRelfectionUpdate + (double)this.reflectionSettings.globalReflectionsUpdateTreshhold || this.currentTimeInHours < this.lastRelfectionUpdate - (double)this.reflectionSettings.globalReflectionsUpdateTreshhold) && this.reflectionSettings.globalReflectionsUpdateOnGameTime)
		{
			this.lastRelfectionUpdate = this.currentTimeInHours;
			this.Components.GlobalReflectionProbe.RefreshReflection(this.reflectionSettings.globalReflectionTimeSlicing);
			return;
		}
		if ((base.transform.position.magnitude > this.lastRelfectionPositionUpdate.magnitude + 0.25f || base.transform.position.magnitude < this.lastRelfectionPositionUpdate.magnitude - 0.25f) && this.reflectionSettings.globalReflectionsUpdateOnPosition)
		{
			this.lastRelfectionPositionUpdate = base.transform.position;
			this.Components.GlobalReflectionProbe.RefreshReflection(this.reflectionSettings.globalReflectionTimeSlicing);
		}
	}

	// Token: 0x060001F7 RID: 503 RVA: 0x00014E80 File Offset: 0x00013080
	public void UpdateAmbientLight()
	{
		switch (this.lightSettings.ambientMode)
		{
		case AmbientMode.Skybox:
			RenderSettings.ambientIntensity = this.lightSettings.ambientIntensity.Evaluate(this.GameTime.solarTime);
			if (this.lastAmbientSkyUpdate < this.internalHour || this.lastAmbientSkyUpdate > this.internalHour + 0.101f)
			{
				DynamicGI.UpdateEnvironment();
				this.lastAmbientSkyUpdate = this.internalHour + 0.1f;
			}
			break;
		case AmbientMode.Trilight:
			RenderSettings.ambientSkyColor = Color.Lerp(Color.Lerp(this.lightSettings.ambientSkyColor.Evaluate(this.GameTime.solarTime), this.currentWeatherLightMod, this.currentWeatherLightMod.a) * this.lightSettings.ambientIntensity.Evaluate(this.GameTime.solarTime), this.interiorZoneSettings.currentInteriorAmbientLightMod, this.interiorZoneSettings.currentInteriorAmbientLightMod.a);
			RenderSettings.ambientEquatorColor = Color.Lerp(Color.Lerp(this.lightSettings.ambientEquatorColor.Evaluate(this.GameTime.solarTime), this.currentWeatherLightMod, this.currentWeatherLightMod.a) * this.lightSettings.ambientIntensity.Evaluate(this.GameTime.solarTime), this.interiorZoneSettings.currentInteriorAmbientEQLightMod, this.interiorZoneSettings.currentInteriorAmbientEQLightMod.a);
			RenderSettings.ambientGroundColor = Color.Lerp(Color.Lerp(this.lightSettings.ambientGroundColor.Evaluate(this.GameTime.solarTime), this.currentWeatherLightMod, this.currentWeatherLightMod.a) * this.lightSettings.ambientIntensity.Evaluate(this.GameTime.solarTime), this.interiorZoneSettings.currentInteriorAmbientGRLightMod, this.interiorZoneSettings.currentInteriorAmbientGRLightMod.a);
			return;
		case (AmbientMode)2:
			break;
		case AmbientMode.Flat:
			RenderSettings.ambientSkyColor = Color.Lerp(Color.Lerp(this.lightSettings.ambientSkyColor.Evaluate(this.GameTime.solarTime), this.currentWeatherLightMod, this.currentWeatherLightMod.a) * this.lightSettings.ambientIntensity.Evaluate(this.GameTime.solarTime), this.interiorZoneSettings.currentInteriorAmbientLightMod, this.interiorZoneSettings.currentInteriorAmbientLightMod.a);
			return;
		default:
			return;
		}
	}

	// Token: 0x060001F8 RID: 504 RVA: 0x000150E4 File Offset: 0x000132E4
	public void CalculateDirectLight()
	{
		if (this.MainLight == null)
		{
			this.MainLight = this.Components.DirectLight.GetComponent<Light>();
		}
		if (this.lightSettings.directionalLightMode == EnviroLightSettings.LightingMode.Single || this.Components.AdditionalDirectLight == null)
		{
			Color a = Color.Lerp(this.lightSettings.LightColor.Evaluate(this.GameTime.solarTime), this.currentWeatherLightMod, this.currentWeatherLightMod.a);
			this.MainLight.color = Color.Lerp(a, this.interiorZoneSettings.currentInteriorDirectLightMod, this.interiorZoneSettings.currentInteriorDirectLightMod.a);
			float b;
			if (!this.isNight)
			{
				b = this.lightSettings.directLightSunIntensity.Evaluate(this.GameTime.solarTime);
				this.Components.Sun.transform.LookAt(new Vector3(base.transform.position.x, base.transform.position.y - this.lightSettings.directLightAngleOffset, base.transform.position.z));
				if (!this.lightSettings.stopRotationAtHigh || (this.lightSettings.stopRotationAtHigh && this.GameTime.solarTime >= this.lightSettings.rotationStopHigh))
				{
					this.Components.DirectLight.rotation = this.Components.Sun.transform.rotation;
				}
			}
			else
			{
				b = this.lightSettings.directLightMoonIntensity.Evaluate(this.GameTime.lunarTime);
				this.Components.Moon.transform.LookAt(new Vector3(base.transform.position.x, base.transform.position.y - this.lightSettings.directLightAngleOffset, base.transform.position.z));
				if (!this.lightSettings.stopRotationAtHigh || (this.lightSettings.stopRotationAtHigh && this.GameTime.lunarTime >= this.lightSettings.rotationStopHigh))
				{
					this.Components.DirectLight.rotation = this.Components.Moon.transform.rotation;
				}
			}
			this.MainLight.intensity = Mathf.Lerp(this.MainLight.intensity, b, Time.deltaTime * this.lightSettings.lightIntensityTransitionSpeed);
			this.MainLight.shadowStrength = Mathf.Clamp01(this.lightSettings.shadowIntensity.Evaluate(this.GameTime.solarTime) + this.shadowIntensityMod);
			return;
		}
		if (this.lightSettings.directionalLightMode == EnviroLightSettings.LightingMode.Dual && this.Components.AdditionalDirectLight != null)
		{
			if (this.AdditionalLight == null)
			{
				this.AdditionalLight = this.Components.AdditionalDirectLight.GetComponent<Light>();
			}
			Color a2 = Color.Lerp(this.lightSettings.LightColor.Evaluate(this.GameTime.solarTime), this.currentWeatherLightMod, this.currentWeatherLightMod.a);
			this.MainLight.color = Color.Lerp(a2, this.interiorZoneSettings.currentInteriorDirectLightMod, this.interiorZoneSettings.currentInteriorDirectLightMod.a);
			this.AdditionalLight.color = this.MainLight.color;
			float b2 = this.lightSettings.directLightSunIntensity.Evaluate(this.GameTime.solarTime);
			float b3 = this.lightSettings.directLightMoonIntensity.Evaluate(this.GameTime.lunarTime) * (1f - this.GameTime.solarTime);
			this.Components.Sun.transform.LookAt(new Vector3(base.transform.position.x, base.transform.position.y - this.lightSettings.directLightAngleOffset, base.transform.position.z));
			if (!this.lightSettings.stopRotationAtHigh || (this.lightSettings.stopRotationAtHigh && this.GameTime.solarTime >= this.lightSettings.rotationStopHigh))
			{
				this.Components.DirectLight.rotation = this.Components.Sun.transform.rotation;
			}
			this.Components.Moon.transform.LookAt(new Vector3(base.transform.position.x, base.transform.position.y - this.lightSettings.directLightAngleOffset, base.transform.position.z));
			if (!this.lightSettings.stopRotationAtHigh || (this.lightSettings.stopRotationAtHigh && this.GameTime.lunarTime >= this.lightSettings.rotationStopHigh))
			{
				this.Components.AdditionalDirectLight.rotation = this.Components.Moon.transform.rotation;
			}
			this.MainLight.intensity = Mathf.Lerp(this.MainLight.intensity, b2, Time.deltaTime * this.lightSettings.lightIntensityTransitionSpeed);
			this.MainLight.shadowStrength = Mathf.Clamp01(this.lightSettings.shadowIntensity.Evaluate(this.GameTime.solarTime) + this.shadowIntensityMod);
			this.AdditionalLight.intensity = Mathf.Lerp(this.AdditionalLight.intensity, b3, Time.deltaTime * this.lightSettings.lightIntensityTransitionSpeed);
			this.AdditionalLight.shadowStrength = Mathf.Clamp01(this.lightSettings.shadowIntensity.Evaluate(this.GameTime.solarTime) + this.shadowIntensityMod);
		}
	}

	// Token: 0x060001F9 RID: 505 RVA: 0x000156C4 File Offset: 0x000138C4
	public void UpdateSceneView()
	{
		if (this.Weather.startWeatherPreset != null && !Application.isPlaying)
		{
			this.currentWeatherSkyMod = this.Weather.startWeatherPreset.weatherSkyMod.Evaluate(this.GameTime.solarTime);
			this.currentWeatherFogMod = this.Weather.startWeatherPreset.weatherFogMod.Evaluate(this.GameTime.solarTime);
			this.currentWeatherLightMod = this.Weather.startWeatherPreset.weatherLightMod.Evaluate(this.GameTime.solarTime);
		}
	}

	// Token: 0x060001FA RID: 506 RVA: 0x00015760 File Offset: 0x00013960
	public void UpdateWind(EnviroWeatherPreset preset)
	{
		if (preset != null)
		{
			this.windIntensity = Mathf.Lerp(this.windIntensity, preset.WindStrenght, this.weatherSettings.windIntensityTransitionSpeed * Time.deltaTime);
		}
		if (this.cloudsSettings.useWindZoneDirection)
		{
			this.cloudsSettings.cloudsWindDirectionX = -this.Components.windZone.transform.forward.x;
			this.cloudsSettings.cloudsWindDirectionY = -this.Components.windZone.transform.forward.z;
		}
		this.cloudAnim += new Vector3(this.cloudsSettings.cloudsTimeScale * (this.windIntensity * this.cloudsSettings.cloudsWindDirectionX) * this.cloudsSettings.cloudsWindIntensity * Time.deltaTime, this.cloudsSettings.cloudsTimeScale * (this.windIntensity * this.cloudsSettings.cloudsWindDirectionY) * this.cloudsSettings.cloudsWindIntensity * Time.deltaTime, this.cloudsSettings.cloudsTimeScale * -1f * this.cloudsSettings.cloudsUpwardsWindIntensity * Time.deltaTime);
		this.cloudAnimNonScaled += new Vector2(this.cloudsSettings.cloudsTimeScale * (this.windIntensity * this.cloudsSettings.cloudsWindDirectionX) * this.cloudsSettings.cloudsWindIntensity * Time.deltaTime * 0.1f, this.cloudsSettings.cloudsTimeScale * (this.windIntensity * this.cloudsSettings.cloudsWindDirectionY) * this.cloudsSettings.cloudsWindIntensity * Time.deltaTime * 0.1f);
		this.cirrusAnim += new Vector2(this.cloudsSettings.cloudsTimeScale * (this.windIntensity * this.cloudsSettings.cloudsWindDirectionX) * this.cloudsSettings.cirrusWindIntensity * Time.deltaTime * 0.1f, this.cloudsSettings.cloudsTimeScale * (this.windIntensity * this.cloudsSettings.cloudsWindDirectionY) * this.cloudsSettings.cirrusWindIntensity * Time.deltaTime * 0.1f);
		if (this.cloudAnim.x > 1f)
		{
			this.cloudAnim.x = -1f;
		}
		else if (this.cloudAnim.x < -1f)
		{
			this.cloudAnim.x = 1f;
		}
		if (this.cloudAnim.y > 1f)
		{
			this.cloudAnim.y = -1f;
		}
		else if (this.cloudAnim.y < -1f)
		{
			this.cloudAnim.y = 1f;
		}
		if (this.cloudAnim.z > 1f)
		{
			this.cloudAnim.z = -1f;
		}
		else if (this.cloudAnim.z < -1f)
		{
			this.cloudAnim.z = 1f;
		}
		if (this.cirrusAnim.x > 1f)
		{
			this.cirrusAnim.x = -1f;
		}
		else if (this.cirrusAnim.x < -1f)
		{
			this.cirrusAnim.x = 1f;
		}
		if (this.cirrusAnim.y > 1f)
		{
			this.cirrusAnim.y = -1f;
		}
		else if (this.cirrusAnim.y < -1f)
		{
			this.cirrusAnim.y = 1f;
		}
		this.Components.windZone.windMain = this.windIntensity;
	}

	// Token: 0x060001FB RID: 507 RVA: 0x00015B08 File Offset: 0x00013D08
	public int GetActiveWeatherID()
	{
		for (int i = 0; i < this.Weather.WeatherPrefabs.Count; i++)
		{
			if (this.Weather.WeatherPrefabs[i].weatherPreset == this.Weather.currentActiveWeatherPreset)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060001FC RID: 508 RVA: 0x00015B5C File Offset: 0x00013D5C
	public void UpdateWeatherVariables(EnviroWeatherPreset p)
	{
		this.UpdateWind(p);
		if (this.Weather.wetness < p.wetnessLevel)
		{
			this.Weather.wetness = Mathf.Lerp(this.Weather.curWetness, p.wetnessLevel, this.weatherSettings.wetnessAccumulationSpeed * Time.deltaTime);
		}
		else
		{
			this.Weather.wetness = Mathf.Lerp(this.Weather.curWetness, p.wetnessLevel, this.weatherSettings.wetnessDryingSpeed * Time.deltaTime);
		}
		this.Weather.wetness = Mathf.Clamp(this.Weather.wetness, 0f, 1f);
		this.Weather.curWetness = this.Weather.wetness;
		if (this.Weather.snowStrength < p.snowLevel)
		{
			this.Weather.snowStrength = Mathf.Lerp(this.Weather.curSnowStrength, p.snowLevel, this.weatherSettings.snowAccumulationSpeed * Time.deltaTime);
		}
		else if (this.Weather.currentTemperature > this.weatherSettings.snowMeltingTresholdTemperature)
		{
			this.Weather.snowStrength = Mathf.Lerp(this.Weather.curSnowStrength, p.snowLevel, this.weatherSettings.snowMeltingSpeed * Time.deltaTime);
		}
		this.Weather.snowStrength = Mathf.Clamp(this.Weather.snowStrength, 0f, 1f);
		this.Weather.curSnowStrength = this.Weather.snowStrength;
		Shader.SetGlobalFloat("_EnviroGrassSnow", this.Weather.curSnowStrength);
		float num = 0f;
		switch (this.Seasons.currentSeasons)
		{
		case EnviroSeasons.Seasons.Spring:
			num = this.seasonsSettings.springBaseTemperature.Evaluate(this.GetUniversalTimeOfDay() / 24f);
			break;
		case EnviroSeasons.Seasons.Summer:
			num = this.seasonsSettings.summerBaseTemperature.Evaluate(this.GetUniversalTimeOfDay() / 24f);
			break;
		case EnviroSeasons.Seasons.Autumn:
			num = this.seasonsSettings.autumnBaseTemperature.Evaluate(this.GetUniversalTimeOfDay() / 24f);
			break;
		case EnviroSeasons.Seasons.Winter:
			num = this.seasonsSettings.winterBaseTemperature.Evaluate(this.GetUniversalTimeOfDay() / 24f);
			break;
		}
		num += p.temperatureLevel;
		this.Weather.currentTemperature = Mathf.Lerp(this.Weather.currentTemperature, num, Time.deltaTime * this.weatherSettings.temperatureChangingSpeed);
	}

	// Token: 0x060001FD RID: 509 RVA: 0x00015DDF File Offset: 0x00013FDF
	public IEnumerator PlayThunderRandom()
	{
		yield return new WaitForSeconds(Random.Range(this.Weather.currentActiveWeatherPreset.lightningInterval, this.Weather.currentActiveWeatherPreset.lightningInterval * 2f));
		if (this.Weather.currentActiveWeatherPrefab.weatherPreset.isLightningStorm)
		{
			if (this.Weather.weatherFullyChanged)
			{
				this.PlayLightning();
			}
			base.StartCoroutine(this.PlayThunderRandom());
		}
		else
		{
			base.StopCoroutine(this.PlayThunderRandom());
			this.Components.LightningGenerator.StopLightning();
		}
		yield break;
	}

	// Token: 0x060001FE RID: 510 RVA: 0x00015DEE File Offset: 0x00013FEE
	public IEnumerator PlayLightningEffect(Vector3 position)
	{
		this.lightningEffect.transform.position = position;
		this.lightningEffect.transform.eulerAngles = new Vector3(Random.Range(-80f, -100f), 0f, 0f);
		this.lightningEffect.Play();
		yield return new WaitForSeconds(0.5f);
		this.lightningEffect.Stop();
		yield break;
	}

	// Token: 0x060001FF RID: 511 RVA: 0x00015E04 File Offset: 0x00014004
	public void PlayLightning()
	{
		if (this.lightningEffect != null)
		{
			base.StartCoroutine(this.PlayLightningEffect(new Vector3(base.transform.position.x + Random.Range(-this.weatherSettings.lightningRange, this.weatherSettings.lightningRange), this.weatherSettings.lightningHeight, base.transform.position.z + Random.Range(-this.weatherSettings.lightningRange, this.weatherSettings.lightningRange))));
		}
		int index = Random.Range(0, this.audioSettings.ThunderSFX.Count);
		this.Audio.AudioSourceThunder.PlayOneShot(this.audioSettings.ThunderSFX[index]);
		this.Components.LightningGenerator.Lightning();
	}

	// Token: 0x06000200 RID: 512 RVA: 0x00015EE0 File Offset: 0x000140E0
	public void ForceWeatherUpdate()
	{
		this.Weather.lastActiveWeatherPreset = this.Weather.currentActiveWeatherPreset;
		this.Weather.lastActiveWeatherPrefab = this.Weather.currentActiveWeatherPrefab;
		this.Weather.currentActiveWeatherPreset = this.Weather.currentActiveZone.currentActiveZoneWeatherPreset;
		this.Weather.currentActiveWeatherPrefab = this.Weather.currentActiveZone.currentActiveZoneWeatherPrefab;
		if (this.Weather.currentActiveWeatherPreset != null)
		{
			EnviroSkyMgr.instance.NotifyWeatherChanged(this.Weather.currentActiveWeatherPreset);
			this.Weather.weatherFullyChanged = false;
			if (!this.serverMode)
			{
				if (this.Weather.currentActiveWeatherPrefab.weatherPreset.isLightningStorm)
				{
					base.StartCoroutine(this.PlayThunderRandom());
					return;
				}
				base.StopCoroutine(this.PlayThunderRandom());
				this.Components.LightningGenerator.StopLightning();
			}
		}
	}

	// Token: 0x06000201 RID: 513 RVA: 0x00015FCC File Offset: 0x000141CC
	public void CalcWeatherTransitionState()
	{
		bool weatherFullyChanged = false;
		if (EnviroSkyMgr.instance != null && EnviroSkyMgr.instance.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			weatherFullyChanged = (this.cloudsConfig.coverage >= this.Weather.currentActiveWeatherPreset.cloudsConfig.coverage - 0.01f);
		}
		else if (EnviroSkyMgr.instance != null && EnviroSkyMgr.instance.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.LW)
		{
			weatherFullyChanged = (this.cloudsConfig.particleLayer1Alpha >= this.Weather.currentActiveWeatherPreset.cloudsConfig.particleLayer1Alpha - 0.01f && this.cloudsConfig.particleLayer2Alpha >= this.Weather.currentActiveWeatherPreset.cloudsConfig.particleLayer2Alpha - 0.01f);
		}
		this.Weather.weatherFullyChanged = weatherFullyChanged;
	}

	// Token: 0x06000202 RID: 514 RVA: 0x000160A4 File Offset: 0x000142A4
	public void SetWeatherOverwrite(int weatherId)
	{
		if (weatherId < 0 || weatherId > this.Weather.WeatherPrefabs.Count)
		{
			Debug.Log(this.Weather.WeatherPrefabs.Count);
			return;
		}
		if (this.Weather.WeatherPrefabs[weatherId] != this.Weather.currentActiveWeatherPrefab)
		{
			this.Weather.currentActiveZone.currentActiveZoneWeatherPrefab = this.Weather.WeatherPrefabs[weatherId];
			this.Weather.currentActiveZone.currentActiveZoneWeatherPreset = this.Weather.WeatherPrefabs[weatherId].weatherPreset;
			EnviroSkyMgr.instance.NotifyZoneWeatherChanged(this.Weather.WeatherPrefabs[weatherId].weatherPreset, this.Weather.currentActiveZone);
			Debug.Log("weatherchange");
		}
		EnviroSkyMgr.instance.InstantWeatherChange(this.Weather.currentActiveZone.currentActiveZoneWeatherPreset, this.Weather.currentActiveZone.currentActiveZoneWeatherPrefab);
	}

	// Token: 0x06000203 RID: 515 RVA: 0x000161AC File Offset: 0x000143AC
	public void SetWeatherOverwrite(EnviroWeatherPreset preset)
	{
		if (preset == null)
		{
			return;
		}
		if (preset != this.Weather.currentActiveWeatherPreset)
		{
			for (int i = 0; i < this.Weather.WeatherPrefabs.Count; i++)
			{
				if (preset == this.Weather.WeatherPrefabs[i].weatherPreset)
				{
					this.Weather.currentActiveZone.currentActiveZoneWeatherPrefab = this.Weather.WeatherPrefabs[i];
					this.Weather.currentActiveZone.currentActiveZoneWeatherPreset = preset;
					EnviroSkyMgr.instance.NotifyZoneWeatherChanged(preset, this.Weather.currentActiveZone);
				}
			}
		}
		EnviroSkyMgr.instance.InstantWeatherChange(this.Weather.currentActiveZone.currentActiveZoneWeatherPreset, this.Weather.currentActiveZone.currentActiveZoneWeatherPrefab);
	}

	// Token: 0x06000204 RID: 516 RVA: 0x00016284 File Offset: 0x00014484
	public void ChangeWeather(int weatherId)
	{
		if (weatherId < 0 || weatherId > this.Weather.WeatherPrefabs.Count)
		{
			return;
		}
		if (this.Weather.WeatherPrefabs[weatherId] != this.Weather.currentActiveWeatherPrefab)
		{
			this.Weather.currentActiveZone.currentActiveZoneWeatherPrefab = this.Weather.WeatherPrefabs[weatherId];
			this.Weather.currentActiveZone.currentActiveZoneWeatherPreset = this.Weather.WeatherPrefabs[weatherId].weatherPreset;
			EnviroSkyMgr.instance.NotifyZoneWeatherChanged(this.Weather.WeatherPrefabs[weatherId].weatherPreset, this.Weather.currentActiveZone);
		}
	}

	// Token: 0x06000205 RID: 517 RVA: 0x00016340 File Offset: 0x00014540
	public void ChangeWeather(EnviroWeatherPreset preset)
	{
		if (preset == null)
		{
			return;
		}
		if (preset != this.Weather.currentActiveWeatherPreset)
		{
			for (int i = 0; i < this.Weather.WeatherPrefabs.Count; i++)
			{
				if (preset == this.Weather.WeatherPrefabs[i].weatherPreset)
				{
					this.Weather.currentActiveZone.currentActiveZoneWeatherPrefab = this.Weather.WeatherPrefabs[i];
					this.Weather.currentActiveZone.currentActiveZoneWeatherPreset = preset;
					EnviroSkyMgr.instance.NotifyZoneWeatherChanged(preset, this.Weather.currentActiveZone);
				}
			}
		}
	}

	// Token: 0x06000206 RID: 518 RVA: 0x000163F0 File Offset: 0x000145F0
	public void ChangeWeather(string weatherName)
	{
		for (int i = 0; i < this.Weather.WeatherPrefabs.Count; i++)
		{
			if (this.Weather.WeatherPrefabs[i].weatherPreset.Name == weatherName && this.Weather.WeatherPrefabs[i] != this.Weather.currentActiveWeatherPrefab)
			{
				this.ChangeWeather(i);
				EnviroSkyMgr.instance.NotifyZoneWeatherChanged(this.Weather.WeatherPrefabs[i].weatherPreset, this.Weather.currentActiveZone);
			}
		}
	}

	// Token: 0x06000207 RID: 519 RVA: 0x00016494 File Offset: 0x00014694
	public void UpdateAudioSource(EnviroWeatherPreset i)
	{
		if (i != null && i.weatherSFX != null)
		{
			if (i.weatherSFX == this.Weather.currentAudioSource.audiosrc.clip)
			{
				if (this.Weather.currentAudioSource.audiosrc.volume < 0.1f)
				{
					this.Weather.currentAudioSource.FadeIn(i.weatherSFX);
				}
				return;
			}
			if (this.Weather.currentAudioSource == this.Audio.AudioSourceWeather)
			{
				this.Audio.AudioSourceWeather.FadeOut();
				this.Audio.AudioSourceWeather2.FadeIn(i.weatherSFX);
				this.Weather.currentAudioSource = this.Audio.AudioSourceWeather2;
				return;
			}
			if (this.Weather.currentAudioSource == this.Audio.AudioSourceWeather2)
			{
				this.Audio.AudioSourceWeather2.FadeOut();
				this.Audio.AudioSourceWeather.FadeIn(i.weatherSFX);
				this.Weather.currentAudioSource = this.Audio.AudioSourceWeather;
				return;
			}
		}
		else
		{
			this.Audio.AudioSourceWeather.FadeOut();
			this.Audio.AudioSourceWeather2.FadeOut();
		}
	}

	// Token: 0x06000208 RID: 520 RVA: 0x000165E7 File Offset: 0x000147E7
	public void RegisterZone(EnviroZone zoneToAdd)
	{
		this.Weather.zones.Add(zoneToAdd);
	}

	// Token: 0x06000209 RID: 521 RVA: 0x000165FA File Offset: 0x000147FA
	public void EnterZone(EnviroZone zone)
	{
		this.Weather.currentActiveZone = zone;
	}

	// Token: 0x0600020A RID: 522 RVA: 0x00002188 File Offset: 0x00000388
	public void ExitZone()
	{
	}

	// Token: 0x0600020B RID: 523 RVA: 0x00016608 File Offset: 0x00014808
	public void UpdateParticleClouds(bool active)
	{
		if (this.particleClouds.layer1System == null || this.particleClouds.layer2System == null)
		{
			return;
		}
		if (active)
		{
			if (!this.particleClouds.layer1System.gameObject.activeSelf)
			{
				this.particleClouds.layer1System.gameObject.SetActive(true);
			}
			if (!this.particleClouds.layer2System.gameObject.activeSelf)
			{
				this.particleClouds.layer2System.gameObject.SetActive(true);
			}
			this.particleClouds.layer1System.transform.position = new Vector3(this.particleClouds.layer1System.transform.position.x, base.transform.localScale.y * this.cloudsSettings.ParticleCloudsLayer1.height, this.particleClouds.layer1System.transform.position.z);
			this.particleClouds.layer2System.transform.position = new Vector3(this.particleClouds.layer2System.transform.position.x, base.transform.localScale.y * this.cloudsSettings.ParticleCloudsLayer2.height, this.particleClouds.layer2System.transform.position.z);
			if (this.cloudsSettings.ParticleCloudsLayer1.height >= this.cloudsSettings.ParticleCloudsLayer2.height)
			{
				this.particleClouds.layer1Material.renderQueue = 3001;
				this.particleClouds.layer2Material.renderQueue = 3002;
			}
			else
			{
				this.particleClouds.layer1Material.renderQueue = 3002;
				this.particleClouds.layer2Material.renderQueue = 3001;
			}
			Color value = this.cloudsSettings.ParticleCloudsLayer1.particleCloudsColor.Evaluate(this.GameTime.solarTime) * this.cloudsConfig.particleLayer1Brightness;
			value.a = this.cloudsConfig.particleLayer1Alpha;
			this.particleClouds.layer1Material.SetColor("_CloudsColor", value);
			Color value2 = this.cloudsSettings.ParticleCloudsLayer2.particleCloudsColor.Evaluate(this.GameTime.solarTime) * this.cloudsConfig.particleLayer2Brightness;
			value2.a = this.cloudsConfig.particleLayer2Alpha;
			this.particleClouds.layer2Material.SetColor("_CloudsColor", value2);
			return;
		}
		if (this.particleClouds.layer1System != null && this.particleClouds.layer1System.isPlaying)
		{
			this.particleClouds.layer1System.gameObject.SetActive(false);
		}
		if (this.particleClouds.layer2System != null && this.particleClouds.layer2System.isPlaying)
		{
			this.particleClouds.layer2System.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600020C RID: 524 RVA: 0x0001691C File Offset: 0x00014B1C
	public void CreateSatellite(int id)
	{
		if (this.satelliteSettings.additionalSatellites[id].prefab == null)
		{
			Debug.Log("Satellite without prefab! Pleae assign a prefab to all satellites.");
			return;
		}
		GameObject gameObject = new GameObject();
		gameObject.name = this.satelliteSettings.additionalSatellites[id].name;
		gameObject.transform.parent = this.Components.satellites;
		this.satellitesRotation.Add(gameObject);
		GameObject gameObject2 = Object.Instantiate<GameObject>(this.satelliteSettings.additionalSatellites[id].prefab, gameObject.transform);
		gameObject2.layer = this.satelliteRenderingLayer;
		this.satellites.Add(gameObject2);
	}

	// Token: 0x0600020D RID: 525 RVA: 0x000169D0 File Offset: 0x00014BD0
	public void CheckSatellites()
	{
		this.satellites = new List<GameObject>();
		for (int i = this.Components.satellites.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(this.Components.satellites.GetChild(i).gameObject);
		}
		this.satellites.Clear();
		this.satellitesRotation.Clear();
		for (int j = 0; j < this.satelliteSettings.additionalSatellites.Count; j++)
		{
			this.CreateSatellite(j);
		}
	}

	// Token: 0x0600020E RID: 526 RVA: 0x00016A58 File Offset: 0x00014C58
	public void CalculateSatPositions(float siderealTime)
	{
		for (int i = 0; i < this.satelliteSettings.additionalSatellites.Count; i++)
		{
			Quaternion quaternion = Quaternion.Euler(90f - this.GameTime.Latitude, this.GameTime.Longitude, 0f);
			quaternion *= Quaternion.Euler(this.satelliteSettings.additionalSatellites[i].yRot, siderealTime, this.satelliteSettings.additionalSatellites[i].xRot);
			if (this.satellites.Count >= 1 && this.satellites.Count >= i)
			{
				this.satellites[i].transform.localPosition = new Vector3(0f, this.satelliteSettings.additionalSatellites[i].orbit, 0f);
			}
			if (this.satellitesRotation.Count >= 1 && this.satellitesRotation.Count >= i)
			{
				this.satellitesRotation[i].transform.localRotation = quaternion;
			}
		}
	}

	// Token: 0x0600020F RID: 527 RVA: 0x00016B70 File Offset: 0x00014D70
	public void SetCameraHDR(Camera cam, bool hdr)
	{
		cam.allowHDR = hdr;
	}

	// Token: 0x06000210 RID: 528 RVA: 0x00016B79 File Offset: 0x00014D79
	public bool GetCameraHDR(Camera cam)
	{
		return cam.allowHDR;
	}

	// Token: 0x06000211 RID: 529 RVA: 0x00016B81 File Offset: 0x00014D81
	private Quaternion LightLookAt(Quaternion inputRotation, Quaternion newRotation)
	{
		return Quaternion.Lerp(inputRotation, newRotation, 500f * Time.deltaTime);
	}

	// Token: 0x06000212 RID: 530 RVA: 0x00016B95 File Offset: 0x00014D95
	public int RegisterMe(EnviroVegetationInstance me)
	{
		this.EnviroVegetationInstances.Add(me);
		return this.EnviroVegetationInstances.Count - 1;
	}

	// Token: 0x06000213 RID: 531 RVA: 0x00016BB0 File Offset: 0x00014DB0
	public void Save()
	{
		PlayerPrefs.SetFloat("Time_Hours", this.internalHour);
		PlayerPrefs.SetInt("Time_Days", this.GameTime.Days);
		PlayerPrefs.SetInt("Time_Years", this.GameTime.Years);
		for (int i = 0; i < this.Weather.WeatherPrefabs.Count; i++)
		{
			if (this.Weather.WeatherPrefabs[i] == this.Weather.currentActiveWeatherPrefab)
			{
				PlayerPrefs.SetInt("currentWeather", i);
			}
		}
	}

	// Token: 0x06000214 RID: 532 RVA: 0x00016C40 File Offset: 0x00014E40
	public void Load()
	{
		if (PlayerPrefs.HasKey("Time_Hours"))
		{
			this.SetInternalTimeOfDay(PlayerPrefs.GetFloat("Time_Hours"));
		}
		if (PlayerPrefs.HasKey("Time_Days"))
		{
			this.GameTime.Days = PlayerPrefs.GetInt("Time_Days");
		}
		if (PlayerPrefs.HasKey("Time_Years"))
		{
			this.GameTime.Years = PlayerPrefs.GetInt("Time_Years");
		}
		if (PlayerPrefs.HasKey("currentWeather"))
		{
			this.SetWeatherOverwrite(PlayerPrefs.GetInt("currentWeather"));
		}
	}

	// Token: 0x04000542 RID: 1346
	[Header("Profile")]
	public EnviroProfile profile;

	// Token: 0x04000543 RID: 1347
	[HideInInspector]
	public bool profileLoaded;

	// Token: 0x04000544 RID: 1348
	[Tooltip("Assign your player gameObject here. Required Field! or enable AssignInRuntime!")]
	public GameObject Player;

	// Token: 0x04000545 RID: 1349
	[Tooltip("Assign your main camera here. Required Field! or enable AssignInRuntime!")]
	public Camera PlayerCamera;

	// Token: 0x04000546 RID: 1350
	[Tooltip("If enabled Enviro will search for your Player and Camera by Tag!")]
	public bool AssignInRuntime;

	// Token: 0x04000547 RID: 1351
	[Tooltip("Your Player Tag")]
	public string PlayerTag = "";

	// Token: 0x04000548 RID: 1352
	[Tooltip("Your CameraTag")]
	public string CameraTag = "MainCamera";

	// Token: 0x04000549 RID: 1353
	[Header("Camera Settings")]
	[Tooltip("Enable HDR Rendering. You want to use a third party tonemapping effect for best results!")]
	public bool HDR = true;

	// Token: 0x0400054A RID: 1354
	public EnviroCore.EnviroStartMode startMode;

	// Token: 0x0400054B RID: 1355
	[HideInInspector]
	public bool started;

	// Token: 0x0400054C RID: 1356
	[HideInInspector]
	public bool serverMode;

	// Token: 0x0400054D RID: 1357
	[HideInInspector]
	public EnviroWeatherCloudsConfig cloudsConfig;

	// Token: 0x0400054E RID: 1358
	[HideInInspector]
	public float thunder;

	// Token: 0x0400054F RID: 1359
	[HideInInspector]
	public bool isNight = true;

	// Token: 0x04000550 RID: 1360
	[HideInInspector]
	public List<GameObject> satellites = new List<GameObject>();

	// Token: 0x04000551 RID: 1361
	[HideInInspector]
	public List<GameObject> satellitesRotation = new List<GameObject>();

	// Token: 0x04000552 RID: 1362
	[HideInInspector]
	public List<EnviroVegetationInstance> EnviroVegetationInstances = new List<EnviroVegetationInstance>();

	// Token: 0x04000553 RID: 1363
	[HideInInspector]
	public EnviroLightSettings lightSettings = new EnviroLightSettings();

	// Token: 0x04000554 RID: 1364
	[HideInInspector]
	public EnviroVolumeLightingSettings volumeLightSettings = new EnviroVolumeLightingSettings();

	// Token: 0x04000555 RID: 1365
	[HideInInspector]
	public EnviroSkySettings skySettings = new EnviroSkySettings();

	// Token: 0x04000556 RID: 1366
	[HideInInspector]
	public EnviroReflectionSettings reflectionSettings = new EnviroReflectionSettings();

	// Token: 0x04000557 RID: 1367
	[HideInInspector]
	public EnviroCloudSettings cloudsSettings = new EnviroCloudSettings();

	// Token: 0x04000558 RID: 1368
	[HideInInspector]
	public EnviroWeatherSettings weatherSettings = new EnviroWeatherSettings();

	// Token: 0x04000559 RID: 1369
	[HideInInspector]
	public EnviroFogSettings fogSettings = new EnviroFogSettings();

	// Token: 0x0400055A RID: 1370
	[HideInInspector]
	public EnviroLightShaftsSettings lightshaftsSettings = new EnviroLightShaftsSettings();

	// Token: 0x0400055B RID: 1371
	[HideInInspector]
	public EnviroSeasonSettings seasonsSettings = new EnviroSeasonSettings();

	// Token: 0x0400055C RID: 1372
	[HideInInspector]
	public EnviroAudioSettings audioSettings = new EnviroAudioSettings();

	// Token: 0x0400055D RID: 1373
	[HideInInspector]
	public EnviroSatellitesSettings satelliteSettings = new EnviroSatellitesSettings();

	// Token: 0x0400055E RID: 1374
	[HideInInspector]
	public EnviroQualitySettings qualitySettings = new EnviroQualitySettings();

	// Token: 0x0400055F RID: 1375
	[HideInInspector]
	public EnviroInteriorZoneSettings interiorZoneSettings = new EnviroInteriorZoneSettings();

	// Token: 0x04000560 RID: 1376
	[HideInInspector]
	public EnviroDistanceBlurSettings distanceBlurSettings = new EnviroDistanceBlurSettings();

	// Token: 0x04000561 RID: 1377
	[HideInInspector]
	public EnviroAuroraSettings auroraSettings = new EnviroAuroraSettings();

	// Token: 0x04000562 RID: 1378
	[HideInInspector]
	public DateTime dateTime;

	// Token: 0x04000563 RID: 1379
	[HideInInspector]
	public float internalHour;

	// Token: 0x04000564 RID: 1380
	[HideInInspector]
	public float currentHour;

	// Token: 0x04000565 RID: 1381
	[HideInInspector]
	public float currentDay;

	// Token: 0x04000566 RID: 1382
	[HideInInspector]
	public float currentYear;

	// Token: 0x04000567 RID: 1383
	[HideInInspector]
	public double currentTimeInHours;

	// Token: 0x04000568 RID: 1384
	[HideInInspector]
	public float LST;

	// Token: 0x04000569 RID: 1385
	[HideInInspector]
	public float lastHourUpdate;

	// Token: 0x0400056A RID: 1386
	[HideInInspector]
	public float hourTime;

	// Token: 0x0400056B RID: 1387
	[HideInInspector]
	public Vector3 cloudAnim;

	// Token: 0x0400056C RID: 1388
	[HideInInspector]
	public Vector2 cloudAnimNonScaled;

	// Token: 0x0400056D RID: 1389
	[HideInInspector]
	public Vector2 cirrusAnim;

	// Token: 0x0400056E RID: 1390
	[HideInInspector]
	public float windIntensity;

	// Token: 0x0400056F RID: 1391
	[HideInInspector]
	public float shadowIntensityMod;

	// Token: 0x04000570 RID: 1392
	[HideInInspector]
	public bool interiorMode;

	// Token: 0x04000571 RID: 1393
	[HideInInspector]
	public EnviroInterior lastInteriorZone;

	// Token: 0x04000572 RID: 1394
	[HideInInspector]
	public bool updateFogDensity = true;

	// Token: 0x04000573 RID: 1395
	[HideInInspector]
	public Color customFogColor = Color.black;

	// Token: 0x04000574 RID: 1396
	[HideInInspector]
	public float customFogIntensity;

	// Token: 0x04000575 RID: 1397
	[HideInInspector]
	public Color currentWeatherSkyMod;

	// Token: 0x04000576 RID: 1398
	[HideInInspector]
	public Color currentWeatherLightMod;

	// Token: 0x04000577 RID: 1399
	[HideInInspector]
	public Color currentWeatherFogMod;

	// Token: 0x04000578 RID: 1400
	[HideInInspector]
	[Range(0f, 2f)]
	public float customMoonPhase;

	// Token: 0x04000579 RID: 1401
	public Light MainLight;

	// Token: 0x0400057A RID: 1402
	public Light AdditionalLight;

	// Token: 0x0400057B RID: 1403
	public Transform MoonTransform;

	// Token: 0x0400057C RID: 1404
	public Renderer MoonRenderer;

	// Token: 0x0400057D RID: 1405
	public Material MoonShader;

	// Token: 0x0400057E RID: 1406
	[HideInInspector]
	public float lastAmbientSkyUpdate;

	// Token: 0x0400057F RID: 1407
	[HideInInspector]
	public double lastRelfectionUpdate;

	// Token: 0x04000580 RID: 1408
	[HideInInspector]
	public Vector3 lastRelfectionPositionUpdate;

	// Token: 0x04000581 RID: 1409
	[HideInInspector]
	public GameObject EffectsHolder;

	// Token: 0x04000582 RID: 1410
	public ParticleSystem lightningEffect;

	// Token: 0x04000583 RID: 1411
	public const float pi = 3.1415927f;

	// Token: 0x04000584 RID: 1412
	private Vector3 K = new Vector3(686f, 678f, 666f);

	// Token: 0x04000585 RID: 1413
	private const float n = 1.0003f;

	// Token: 0x04000586 RID: 1414
	private const float N = 2.545E+25f;

	// Token: 0x04000587 RID: 1415
	private const float pn = 0.035f;

	// Token: 0x04000588 RID: 1416
	public EnviroTime GameTime;

	// Token: 0x04000589 RID: 1417
	public EnviroAudio Audio;

	// Token: 0x0400058A RID: 1418
	public EnviroWeather Weather;

	// Token: 0x0400058B RID: 1419
	public EnviroSeasons Seasons;

	// Token: 0x0400058C RID: 1420
	public EnviroComponents Components;

	// Token: 0x0400058D RID: 1421
	public EnviroFogging Fog;

	// Token: 0x0400058E RID: 1422
	public EnviroLightshafts LightShafts;

	// Token: 0x0400058F RID: 1423
	public EnviroParticleCloud particleClouds;

	// Token: 0x04000590 RID: 1424
	[HideInInspector]
	public EnviroPostProcessing EnviroPostProcessing;

	// Token: 0x04000591 RID: 1425
	[Header("Layer Setup")]
	[Tooltip("This is the layer id forfor the moon.")]
	public int moonRenderingLayer = 29;

	// Token: 0x04000592 RID: 1426
	[Tooltip("This is the layer id for additional satellites like moons, planets.")]
	public int satelliteRenderingLayer = 30;

	// Token: 0x04000593 RID: 1427
	[Tooltip("Activate to set recommended maincamera clear flag.")]
	public bool setCameraClearFlags = true;

	// Token: 0x02000375 RID: 885
	public enum EnviroStartMode
	{
		// Token: 0x040026E5 RID: 9957
		Started,
		// Token: 0x040026E6 RID: 9958
		Paused,
		// Token: 0x040026E7 RID: 9959
		PausedButTimeProgress
	}
}
