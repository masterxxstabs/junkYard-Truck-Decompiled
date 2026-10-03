using System;
using UnityEngine;

// Token: 0x0200006E RID: 110
[Serializable]
public class EnviroAudio
{
	// Token: 0x040004F0 RID: 1264
	[Tooltip("The prefab with AudioSources used by Enviro. Will be instantiated at runtime.")]
	public GameObject SFXHolderPrefab;

	// Token: 0x040004F1 RID: 1265
	[Header("Volume Settings:")]
	[Range(0f, 1f)]
	[Tooltip("The volume of ambient sounds played by enviro.")]
	public float ambientSFXVolume = 0.5f;

	// Token: 0x040004F2 RID: 1266
	[Range(0f, 1f)]
	[Tooltip("The volume of weather sounds played by enviro.")]
	public float weatherSFXVolume = 1f;

	// Token: 0x040004F3 RID: 1267
	[HideInInspector]
	public EnviroAudioSource currentAmbientSource;

	// Token: 0x040004F4 RID: 1268
	[HideInInspector]
	public float ambientSFXVolumeMod;

	// Token: 0x040004F5 RID: 1269
	[HideInInspector]
	public float weatherSFXVolumeMod;

	// Token: 0x040004F6 RID: 1270
	[HideInInspector]
	public EnviroAudioSource AudioSourceWeather;

	// Token: 0x040004F7 RID: 1271
	[HideInInspector]
	public EnviroAudioSource AudioSourceWeather2;

	// Token: 0x040004F8 RID: 1272
	[HideInInspector]
	public EnviroAudioSource AudioSourceAmbient;

	// Token: 0x040004F9 RID: 1273
	[HideInInspector]
	public EnviroAudioSource AudioSourceAmbient2;

	// Token: 0x040004FA RID: 1274
	[HideInInspector]
	public EnviroAudioSource AudioSourceThunder;

	// Token: 0x040004FB RID: 1275
	[HideInInspector]
	public EnviroAudioSource AudioSourceZone;
}
