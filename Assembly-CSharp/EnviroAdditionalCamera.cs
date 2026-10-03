using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000AB RID: 171
[ExecuteInEditMode]
[AddComponentMenu("Enviro/Standard/AddionalCamera")]
public class EnviroAdditionalCamera : MonoBehaviour
{
	// Token: 0x06000419 RID: 1049 RVA: 0x0002B1D0 File Offset: 0x000293D0
	private void OnEnable()
	{
		this.myCam = base.GetComponent<Camera>();
		if (this.myCam != null)
		{
			this.InitImageEffects();
		}
	}

	// Token: 0x0600041A RID: 1050 RVA: 0x0002B1F2 File Offset: 0x000293F2
	private void Start()
	{
		if (this.addWeatherEffects)
		{
			this.CreateEffectHolder();
			base.StartCoroutine(this.SetupWeatherEffects());
		}
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x0002B20F File Offset: 0x0002940F
	private void Update()
	{
		if (this.addWeatherEffects)
		{
			this.UpdateWeatherEffects();
		}
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x0002B220 File Offset: 0x00029420
	private void CreateEffectHolder()
	{
		for (int i = this.myCam.transform.childCount - 1; i >= 0; i--)
		{
			if (this.myCam.transform.GetChild(i).gameObject.name == "Effect Holder")
			{
				Object.DestroyImmediate(this.myCam.transform.GetChild(i).gameObject);
			}
		}
		this.EffectHolder = new GameObject();
		this.EffectHolder.name = "Effect Holder";
		this.EffectHolder.transform.SetParent(this.myCam.transform, false);
		this.VFX = new GameObject();
		this.VFX.name = "VFX";
		this.VFX.transform.SetParent(this.EffectHolder.transform, false);
	}

	// Token: 0x0600041D RID: 1053 RVA: 0x0002B2FA File Offset: 0x000294FA
	private IEnumerator SetupWeatherEffects()
	{
		yield return new WaitForSeconds(1f);
		for (int i = 0; i < EnviroSky.instance.Weather.weatherPresets.Count; i++)
		{
			GameObject gameObject = new GameObject();
			EnviroWeatherPrefab enviroWeatherPrefab = gameObject.AddComponent<EnviroWeatherPrefab>();
			enviroWeatherPrefab.weatherPreset = EnviroSky.instance.Weather.weatherPresets[i];
			gameObject.name = enviroWeatherPrefab.weatherPreset.Name;
			for (int j = 0; j < enviroWeatherPrefab.weatherPreset.effectSystems.Count; j++)
			{
				if (enviroWeatherPrefab.weatherPreset.effectSystems[j] == null || enviroWeatherPrefab.weatherPreset.effectSystems[j].prefab == null)
				{
					Debug.Log("Warning! Missing Particle System Entry: " + enviroWeatherPrefab.weatherPreset.Name);
					Object.Destroy(gameObject);
					break;
				}
				GameObject gameObject2 = Object.Instantiate<GameObject>(enviroWeatherPrefab.weatherPreset.effectSystems[j].prefab, gameObject.transform);
				gameObject2.transform.localPosition = enviroWeatherPrefab.weatherPreset.effectSystems[j].localPositionOffset;
				gameObject2.transform.localEulerAngles = enviroWeatherPrefab.weatherPreset.effectSystems[j].localRotationOffset;
				ParticleSystem particleSystem = gameObject2.GetComponent<ParticleSystem>();
				if (particleSystem != null)
				{
					enviroWeatherPrefab.effectSystems.Add(particleSystem);
				}
				else
				{
					particleSystem = gameObject2.GetComponentInChildren<ParticleSystem>();
					if (!(particleSystem != null))
					{
						Debug.Log("No Particle System found in prefab in weather preset: " + enviroWeatherPrefab.weatherPreset.Name);
						Object.Destroy(gameObject);
						break;
					}
					enviroWeatherPrefab.effectSystems.Add(particleSystem);
				}
			}
			enviroWeatherPrefab.effectEmmisionRates.Clear();
			gameObject.transform.parent = this.VFX.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			this.zoneWeather.Add(enviroWeatherPrefab);
		}
		for (int k = 0; k < this.zoneWeather.Count; k++)
		{
			for (int l = 0; l < this.zoneWeather[k].effectSystems.Count; l++)
			{
				this.zoneWeather[k].effectEmmisionRates.Add(EnviroSkyMgr.instance.GetEmissionRate(this.zoneWeather[k].effectSystems[l]));
				EnviroSkyMgr.instance.SetEmissionRate(this.zoneWeather[k].effectSystems[l], 0f);
			}
		}
		if (EnviroSky.instance.Weather.currentActiveWeatherPrefab != null)
		{
			for (int m = 0; m < this.zoneWeather.Count; m++)
			{
				if (this.zoneWeather[m].weatherPreset == EnviroSky.instance.Weather.currentActiveWeatherPrefab.weatherPreset)
				{
					this.currentWeather = this.zoneWeather[m];
				}
			}
		}
		yield break;
	}

	// Token: 0x0600041E RID: 1054 RVA: 0x0002B30C File Offset: 0x0002950C
	private void UpdateWeatherEffects()
	{
		if (EnviroSky.instance.Weather.currentActiveWeatherPrefab == null || this.currentWeather == null)
		{
			return;
		}
		if (EnviroSky.instance.Weather.currentActiveWeatherPrefab.weatherPreset != this.currentWeather.weatherPreset)
		{
			for (int i = 0; i < this.zoneWeather.Count; i++)
			{
				if (this.zoneWeather[i].weatherPreset == EnviroSky.instance.Weather.currentActiveWeatherPrefab.weatherPreset)
				{
					this.currentWeather = this.zoneWeather[i];
				}
			}
		}
		this.UpdateEffectSystems(this.currentWeather, true);
	}

	// Token: 0x0600041F RID: 1055 RVA: 0x0002B3C8 File Offset: 0x000295C8
	private void UpdateEffectSystems(EnviroWeatherPrefab id, bool withTransition)
	{
		if (id != null)
		{
			float t = 500f * Time.deltaTime;
			if (withTransition)
			{
				t = EnviroSkyMgr.instance.WeatherSettings.effectTransitionSpeed * Time.deltaTime;
			}
			for (int i = 0; i < id.effectSystems.Count; i++)
			{
				if (id.effectSystems[i].isStopped)
				{
					id.effectSystems[i].Play();
				}
				float emissionRate = Mathf.Lerp(EnviroSkyMgr.instance.GetEmissionRate(id.effectSystems[i]), id.effectEmmisionRates[i] * EnviroSky.instance.qualitySettings.GlobalParticleEmissionRates, t) * EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorWeatherEffectMod;
				EnviroSkyMgr.instance.SetEmissionRate(id.effectSystems[i], emissionRate);
			}
			for (int j = 0; j < this.zoneWeather.Count; j++)
			{
				if (this.zoneWeather[j].gameObject != id.gameObject)
				{
					for (int k = 0; k < this.zoneWeather[j].effectSystems.Count; k++)
					{
						float num = Mathf.Lerp(EnviroSkyMgr.instance.GetEmissionRate(this.zoneWeather[j].effectSystems[k]), 0f, t);
						if (num < 1f)
						{
							num = 0f;
						}
						EnviroSkyMgr.instance.SetEmissionRate(this.zoneWeather[j].effectSystems[k], num);
						if (num == 0f && !this.zoneWeather[j].effectSystems[k].isStopped)
						{
							this.zoneWeather[j].effectSystems[k].Stop();
						}
					}
				}
			}
		}
	}

	// Token: 0x06000420 RID: 1056 RVA: 0x0002B5B8 File Offset: 0x000297B8
	private void InitImageEffects()
	{
		if (this.addEnviroSkyRendering)
		{
			this.skyRender = this.myCam.gameObject.GetComponent<EnviroSkyRendering>();
			if (this.skyRender == null)
			{
				this.skyRender = this.myCam.gameObject.AddComponent<EnviroSkyRendering>();
			}
			this.skyRender.isAddionalCamera = true;
		}
		if (this.addEnviroSkyPostProcessing)
		{
			this.enviroPostProcessing = this.myCam.gameObject.GetComponent<EnviroPostProcessing>();
			if (this.enviroPostProcessing == null)
			{
				this.enviroPostProcessing = this.myCam.gameObject.AddComponent<EnviroPostProcessing>();
			}
		}
	}

	// Token: 0x04000843 RID: 2115
	public bool addEnviroSkyRendering = true;

	// Token: 0x04000844 RID: 2116
	public bool addEnviroSkyPostProcessing = true;

	// Token: 0x04000845 RID: 2117
	public bool addWeatherEffects = true;

	// Token: 0x04000846 RID: 2118
	private Camera myCam;

	// Token: 0x04000847 RID: 2119
	private EnviroSkyRendering skyRender;

	// Token: 0x04000848 RID: 2120
	private EnviroPostProcessing enviroPostProcessing;

	// Token: 0x04000849 RID: 2121
	private GameObject EffectHolder;

	// Token: 0x0400084A RID: 2122
	private GameObject VFX;

	// Token: 0x0400084B RID: 2123
	private List<EnviroWeatherPrefab> zoneWeather = new List<EnviroWeatherPrefab>();

	// Token: 0x0400084C RID: 2124
	private EnviroWeatherPrefab currentWeather;
}
