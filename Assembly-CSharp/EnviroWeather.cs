using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000074 RID: 116
[Serializable]
public class EnviroWeather
{
	// Token: 0x04000528 RID: 1320
	[Tooltip("If disabled the weather will never change.")]
	public bool updateWeather = true;

	// Token: 0x04000529 RID: 1321
	public List<EnviroWeatherPreset> weatherPresets = new List<EnviroWeatherPreset>();

	// Token: 0x0400052A RID: 1322
	public List<EnviroWeatherPrefab> WeatherPrefabs = new List<EnviroWeatherPrefab>();

	// Token: 0x0400052B RID: 1323
	[Tooltip("List of additional zones. Will be updated on startup!")]
	public List<EnviroZone> zones = new List<EnviroZone>();

	// Token: 0x0400052C RID: 1324
	public EnviroWeatherPreset startWeatherPreset;

	// Token: 0x0400052D RID: 1325
	[Tooltip("The current active zone.")]
	public EnviroZone currentActiveZone;

	// Token: 0x0400052E RID: 1326
	[Tooltip("The current active weather conditions.")]
	public EnviroWeatherPrefab currentActiveWeatherPrefab;

	// Token: 0x0400052F RID: 1327
	public EnviroWeatherPreset currentActiveWeatherPreset;

	// Token: 0x04000530 RID: 1328
	[HideInInspector]
	public EnviroWeatherPrefab lastActiveWeatherPrefab;

	// Token: 0x04000531 RID: 1329
	[HideInInspector]
	public EnviroWeatherPreset lastActiveWeatherPreset;

	// Token: 0x04000532 RID: 1330
	[HideInInspector]
	public GameObject VFXHolder;

	// Token: 0x04000533 RID: 1331
	[HideInInspector]
	public float wetness;

	// Token: 0x04000534 RID: 1332
	[HideInInspector]
	public float curWetness;

	// Token: 0x04000535 RID: 1333
	[HideInInspector]
	public float snowStrength;

	// Token: 0x04000536 RID: 1334
	[HideInInspector]
	public float curSnowStrength;

	// Token: 0x04000537 RID: 1335
	[HideInInspector]
	public int thundersfx;

	// Token: 0x04000538 RID: 1336
	[HideInInspector]
	public EnviroAudioSource currentAudioSource;

	// Token: 0x04000539 RID: 1337
	[HideInInspector]
	public bool weatherFullyChanged;

	// Token: 0x0400053A RID: 1338
	[HideInInspector]
	public float currentTemperature;
}
