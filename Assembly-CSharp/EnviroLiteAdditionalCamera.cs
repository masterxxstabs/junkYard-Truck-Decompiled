using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000A2 RID: 162
[ExecuteInEditMode]
[AddComponentMenu("Enviro/Lite/AddionalCamera")]
public class EnviroLiteAdditionalCamera : MonoBehaviour
{
	// Token: 0x0600039C RID: 924 RVA: 0x00021A83 File Offset: 0x0001FC83
	private void OnEnable()
	{
		this.myCam = base.GetComponent<Camera>();
		if (this.myCam != null)
		{
			this.InitImageEffects();
		}
	}

	// Token: 0x0600039D RID: 925 RVA: 0x00021AA5 File Offset: 0x0001FCA5
	private void Start()
	{
		if (this.addWeatherEffects)
		{
			this.CreateEffectHolder();
			base.StartCoroutine(this.SetupWeatherEffects());
		}
	}

	// Token: 0x0600039E RID: 926 RVA: 0x00021AC2 File Offset: 0x0001FCC2
	private void Update()
	{
		if (this.addWeatherEffects)
		{
			this.UpdateWeatherEffects();
		}
		if (EnviroSkyLite.instance != null)
		{
			this.UpdateSkyRenderer();
		}
	}

	// Token: 0x0600039F RID: 927 RVA: 0x00021AE8 File Offset: 0x0001FCE8
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

	// Token: 0x060003A0 RID: 928 RVA: 0x00021BC2 File Offset: 0x0001FDC2
	private IEnumerator SetupWeatherEffects()
	{
		yield return new WaitForSeconds(1f);
		for (int i = 0; i < EnviroSkyMgr.instance.Weather.weatherPresets.Count; i++)
		{
			GameObject gameObject = new GameObject();
			EnviroWeatherPrefab enviroWeatherPrefab = gameObject.AddComponent<EnviroWeatherPrefab>();
			enviroWeatherPrefab.weatherPreset = EnviroSkyMgr.instance.Weather.weatherPresets[i];
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
		if (EnviroSkyMgr.instance.Weather.currentActiveWeatherPrefab != null)
		{
			for (int m = 0; m < this.zoneWeather.Count; m++)
			{
				if (this.zoneWeather[m].weatherPreset == EnviroSkyMgr.instance.Weather.currentActiveWeatherPrefab.weatherPreset)
				{
					this.currentWeather = this.zoneWeather[m];
				}
			}
		}
		yield break;
	}

	// Token: 0x060003A1 RID: 929 RVA: 0x00021BD4 File Offset: 0x0001FDD4
	private void UpdateWeatherEffects()
	{
		if (EnviroSkyMgr.instance.Weather.currentActiveWeatherPrefab == null || this.currentWeather == null)
		{
			return;
		}
		if (EnviroSkyMgr.instance.Weather.currentActiveWeatherPrefab.weatherPreset != this.currentWeather.weatherPreset)
		{
			for (int i = 0; i < this.zoneWeather.Count; i++)
			{
				if (this.zoneWeather[i].weatherPreset == EnviroSkyMgr.instance.Weather.currentActiveWeatherPrefab.weatherPreset)
				{
					this.currentWeather = this.zoneWeather[i];
				}
			}
		}
		this.UpdateEffectSystems(this.currentWeather, true);
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x00021C90 File Offset: 0x0001FE90
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
				float emissionRate = Mathf.Lerp(EnviroSkyMgr.instance.GetEmissionRate(id.effectSystems[i]), id.effectEmmisionRates[i] * EnviroSkyLite.instance.qualitySettings.GlobalParticleEmissionRates, t) * EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorWeatherEffectMod;
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

	// Token: 0x060003A3 RID: 931 RVA: 0x00021E80 File Offset: 0x00020080
	private void InitImageEffects()
	{
		if (this.addEnviroSkyRendering)
		{
			this.skyRender = this.myCam.gameObject.GetComponent<EnviroSkyRenderingLW>();
			if (this.skyRender == null)
			{
				this.skyRender = this.myCam.gameObject.AddComponent<EnviroSkyRenderingLW>();
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

	// Token: 0x060003A4 RID: 932 RVA: 0x00021F20 File Offset: 0x00020120
	private void UpdateSkyRenderer()
	{
		if (EnviroSkyMgr.instance.FogSettings.useUnityFog && EnviroSkyMgr.instance.Camera != null && EnviroSkyMgr.instance.Camera.renderingPath == RenderingPath.Forward)
		{
			RenderSettings.fog = true;
			if (this.skyRender != null && this.skyRender.isActiveAndEnabled)
			{
				this.skyRender.enabled = false;
				return;
			}
		}
		else
		{
			if (EnviroSkyLite.instance.usePostEffectFog && this.skyRender != null && !this.skyRender.isActiveAndEnabled)
			{
				this.skyRender.enabled = true;
				return;
			}
			if (!EnviroSkyLite.instance.usePostEffectFog && this.skyRender != null && this.skyRender.isActiveAndEnabled)
			{
				this.skyRender.enabled = false;
			}
		}
	}

	// Token: 0x040007A2 RID: 1954
	public bool addEnviroSkyRendering = true;

	// Token: 0x040007A3 RID: 1955
	public bool addEnviroSkyPostProcessing = true;

	// Token: 0x040007A4 RID: 1956
	public bool addWeatherEffects = true;

	// Token: 0x040007A5 RID: 1957
	private Camera myCam;

	// Token: 0x040007A6 RID: 1958
	private EnviroSkyRenderingLW skyRender;

	// Token: 0x040007A7 RID: 1959
	private EnviroPostProcessing enviroPostProcessing;

	// Token: 0x040007A8 RID: 1960
	private GameObject EffectHolder;

	// Token: 0x040007A9 RID: 1961
	private GameObject VFX;

	// Token: 0x040007AA RID: 1962
	private List<EnviroWeatherPrefab> zoneWeather = new List<EnviroWeatherPrefab>();

	// Token: 0x040007AB RID: 1963
	private EnviroWeatherPrefab currentWeather;
}
