using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020000A0 RID: 160
[ExecuteInEditMode]
public class EnviroSkyLite : EnviroCore
{
	// Token: 0x17000035 RID: 53
	// (get) Token: 0x06000375 RID: 885 RVA: 0x0001F40D File Offset: 0x0001D60D
	public static EnviroSkyLite instance
	{
		get
		{
			if (EnviroSkyLite._instance == null)
			{
				EnviroSkyLite._instance = Object.FindObjectOfType<EnviroSkyLite>();
			}
			return EnviroSkyLite._instance;
		}
	}

	// Token: 0x06000376 RID: 886 RVA: 0x0001F42C File Offset: 0x0001D62C
	private void Start()
	{
		if (EnviroSkyMgr.instance == null)
		{
			Debug.Log("Please use the EnviroSky Manager!");
			base.gameObject.SetActive(false);
			return;
		}
		base.SetTime(this.GameTime.Years, this.GameTime.Days, this.GameTime.Hours, this.GameTime.Minutes, this.GameTime.Seconds);
		this.lastHourUpdate = (float)Mathf.RoundToInt(this.internalHour);
		this.currentTimeInHours = base.GetInHours(this.internalHour, (float)this.GameTime.Days, (float)this.GameTime.Years, this.GameTime.DaysInYear);
		this.Weather.weatherFullyChanged = false;
		this.thunder = 0f;
		if (this.profileLoaded)
		{
			base.InvokeRepeating("UpdateEnviroment", 0f, this.qualitySettings.UpdateInterval);
			base.CreateEffects("Enviro Effects LW");
			if (this.weatherSettings.lightningEffect != null && this.lightningEffect == null)
			{
				this.lightningEffect = Object.Instantiate<GameObject>(this.weatherSettings.lightningEffect, this.EffectsHolder.transform).GetComponent<ParticleSystem>();
			}
			if (this.PlayerCamera != null && this.Player != null && !this.AssignInRuntime && this.profile != null)
			{
				this.Init();
			}
		}
		base.StartCoroutine(this.SetSkyBoxLateAdditive());
	}

	// Token: 0x06000377 RID: 887 RVA: 0x0001F5B5 File Offset: 0x0001D7B5
	private IEnumerator SetSkyBoxLateAdditive()
	{
		yield return 0;
		if (this.skyMat != null && RenderSettings.skybox != this.skyMat)
		{
			this.SetupSkybox();
		}
		yield break;
	}

	// Token: 0x06000378 RID: 888 RVA: 0x0001F5C4 File Offset: 0x0001D7C4
	private void OnEnable()
	{
		if (EnviroSkyMgr.instance == null)
		{
			return;
		}
		if (this.Weather.zones.Count < 1)
		{
			this.Weather.zones.Add(base.GetComponent<EnviroZone>());
		}
		this.Weather.currentActiveWeatherPreset = this.Weather.zones[0].currentActiveZoneWeatherPreset;
		this.Weather.lastActiveWeatherPreset = this.Weather.currentActiveWeatherPreset;
		if (this.profile == null)
		{
			Debug.LogError("No profile assigned!");
			return;
		}
		if (!this.profileLoaded)
		{
			base.ApplyProfile(this.profile);
		}
		this.PreInit();
		if (this.AssignInRuntime)
		{
			this.started = false;
			return;
		}
		if (this.PlayerCamera != null && this.Player != null)
		{
			this.Init();
		}
	}

	// Token: 0x06000379 RID: 889 RVA: 0x0001F6A4 File Offset: 0x0001D8A4
	public void ReInit()
	{
		this.OnEnable();
	}

	// Token: 0x0600037A RID: 890 RVA: 0x0001F6AC File Offset: 0x0001D8AC
	private void PreInit()
	{
		if (this.GameTime.solarTime < this.GameTime.dayNightSwitch)
		{
			this.isNight = true;
		}
		else
		{
			this.isNight = false;
		}
		if (this.serverMode)
		{
			return;
		}
		base.CheckSatellites();
		RenderSettings.fogMode = this.fogSettings.Fogmode;
		this.SetupSkybox();
		RenderSettings.ambientMode = this.lightSettings.ambientMode;
		if (this.Components.GlobalReflectionProbe == null)
		{
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				if (transform.name == "GlobalReflections")
				{
					GameObject gameObject = transform.gameObject;
					this.Components.GlobalReflectionProbe = gameObject.GetComponent<EnviroReflectionProbe>();
					if (this.Components.GlobalReflectionProbe == null)
					{
						this.Components.GlobalReflectionProbe = gameObject.AddComponent<EnviroReflectionProbe>();
					}
				}
			}
		}
		if (!this.Components.Sun)
		{
			Debug.LogError("Please set sun object in inspector!");
		}
		if (!this.Components.satellites)
		{
			Debug.LogError("Please set satellite object in inspector!");
		}
		if (this.Components.Moon)
		{
			this.MoonTransform = this.Components.Moon.transform;
			this.MoonRenderer = this.Components.Moon.GetComponent<Renderer>();
			if (this.MoonRenderer == null)
			{
				this.MoonRenderer = this.Components.Moon.AddComponent<MeshRenderer>();
			}
			this.MoonRenderer.shadowCastingMode = ShadowCastingMode.Off;
			this.MoonRenderer.receiveShadows = false;
			if (this.MoonRenderer.sharedMaterial != null)
			{
				Object.DestroyImmediate(this.MoonRenderer.sharedMaterial);
			}
			if (this.skySettings.moonPhaseMode == EnviroSkySettings.MoonPhases.Realistic)
			{
				this.MoonShader = new Material(Shader.Find("Enviro/Lite/MoonShader"));
			}
			else
			{
				this.MoonShader = new Material(Shader.Find("Enviro/Lite/MoonShaderPhased"));
			}
			this.MoonShader.SetTexture("_MainTex", this.skySettings.moonTexture);
			this.MoonRenderer.sharedMaterial = this.MoonShader;
			this.customMoonPhase = this.skySettings.startMoonPhase;
		}
		else
		{
			Debug.LogError("Please set moon object in inspector!");
		}
		if (this.lightSettings.directionalLightMode == EnviroLightSettings.LightingMode.Single)
		{
			this.SetupMainLight();
		}
		else
		{
			this.SetupMainLight();
			this.SetupAdditionalLight();
		}
		if (this.Components.particleClouds)
		{
			ParticleSystem[] componentsInChildren = this.Components.particleClouds.GetComponentsInChildren<ParticleSystem>();
			if (componentsInChildren.Length != 0)
			{
				this.particleClouds.layer1System = componentsInChildren[0];
			}
			if (componentsInChildren.Length > 1)
			{
				this.particleClouds.layer2System = componentsInChildren[1];
			}
			if (this.particleClouds.layer1System != null)
			{
				this.particleClouds.layer1Material = this.particleClouds.layer1System.GetComponent<ParticleSystemRenderer>().sharedMaterial;
			}
			if (this.particleClouds.layer2System != null)
			{
				this.particleClouds.layer2Material = this.particleClouds.layer2System.GetComponent<ParticleSystemRenderer>().sharedMaterial;
				return;
			}
		}
		else
		{
			Debug.LogError("Please set particleCLouds object in inspector!");
		}
	}

	// Token: 0x0600037B RID: 891 RVA: 0x0001FA00 File Offset: 0x0001DC00
	public void SetupSkybox()
	{
		if (this.skySettings.skyboxModeLW == EnviroSkySettings.SkyboxModiLW.Simple)
		{
			if (this.skyMat != null)
			{
				Object.DestroyImmediate(this.skyMat);
			}
			this.skyMat = new Material(Shader.Find("Enviro/Lite/SkyboxSimple"));
			if (this.skySettings.starsCubeMap != null)
			{
				this.skyMat.SetTexture("_Stars", this.skySettings.starsCubeMap);
			}
			if (this.skySettings.galaxyCubeMap != null)
			{
				this.skyMat.SetTexture("_Galaxy", this.skySettings.galaxyCubeMap);
			}
			RenderSettings.skybox = this.skyMat;
		}
		else if (this.skySettings.skyboxMode == EnviroSkySettings.SkyboxModi.CustomSkybox && this.skySettings.customSkyboxMaterial != null)
		{
			RenderSettings.skybox = this.skySettings.customSkyboxMaterial;
		}
		if (this.lightSettings.ambientMode == AmbientMode.Skybox)
		{
			base.StartCoroutine(this.UpdateAmbientLightWithDelay());
		}
	}

	// Token: 0x0600037C RID: 892 RVA: 0x0001FAFF File Offset: 0x0001DCFF
	private IEnumerator UpdateAmbientLightWithDelay()
	{
		yield return 0;
		DynamicGI.UpdateEnvironment();
		yield break;
	}

	// Token: 0x0600037D RID: 893 RVA: 0x0001FB08 File Offset: 0x0001DD08
	private void Init()
	{
		if (this.profile == null)
		{
			return;
		}
		if (this.serverMode)
		{
			this.started = true;
			return;
		}
		this.InitImageEffects();
		if (this.PlayerCamera != null)
		{
			if (this.setCameraClearFlags)
			{
				this.PlayerCamera.clearFlags = CameraClearFlags.Skybox;
			}
			if (this.PlayerCamera.actualRenderingPath == RenderingPath.DeferredShading)
			{
				base.SetCameraHDR(this.PlayerCamera, true);
			}
			else
			{
				base.SetCameraHDR(this.PlayerCamera, this.HDR);
			}
			this.Components.GlobalReflectionProbe.myProbe.farClipPlane = this.PlayerCamera.farClipPlane;
		}
		this.started = true;
		if (this.MoonShader != null)
		{
			this.MoonShader.SetFloat("_Phase", this.customMoonPhase);
			this.MoonShader.SetColor("_Color", this.skySettings.moonColor);
			this.MoonShader.SetFloat("_Brightness", this.skySettings.moonBrightness * (1f - this.GameTime.solarTime));
		}
	}

	// Token: 0x0600037E RID: 894 RVA: 0x0001FC20 File Offset: 0x0001DE20
	private void InitImageEffects()
	{
		this.EnviroSkyRender = this.PlayerCamera.gameObject.GetComponent<EnviroSkyRenderingLW>();
		if (this.EnviroSkyRender == null)
		{
			this.EnviroSkyRender = this.PlayerCamera.gameObject.AddComponent<EnviroSkyRenderingLW>();
		}
		this.EnviroPostProcessing = this.PlayerCamera.gameObject.GetComponent<EnviroPostProcessing>();
		if (this.EnviroPostProcessing == null)
		{
			this.EnviroPostProcessing = this.PlayerCamera.gameObject.AddComponent<EnviroPostProcessing>();
		}
	}

	// Token: 0x0600037F RID: 895 RVA: 0x0001FCA4 File Offset: 0x0001DEA4
	private void SetupMainLight()
	{
		if (this.Components.DirectLight)
		{
			this.MainLight = this.Components.DirectLight.GetComponent<Light>();
			if (EnviroSkyMgr.instance.dontDestroy && Application.isPlaying)
			{
				Object.DontDestroyOnLoad(this.Components.DirectLight);
			}
		}
		else
		{
			GameObject gameObject = GameObject.Find("Enviro Directional Light");
			if (gameObject != null)
			{
				this.Components.DirectLight = gameObject.transform;
			}
			else
			{
				this.Components.DirectLight = base.CreateDirectionalLight(false);
			}
			this.MainLight = this.Components.DirectLight.GetComponent<Light>();
			if (EnviroSkyMgr.instance.dontDestroy && Application.isPlaying)
			{
				Object.DontDestroyOnLoad(this.Components.DirectLight);
			}
		}
		if (this.lightSettings.directionalLightMode == EnviroLightSettings.LightingMode.Single && this.Components.AdditionalDirectLight != null)
		{
			Object.DestroyImmediate(this.Components.AdditionalDirectLight.gameObject);
		}
	}

	// Token: 0x06000380 RID: 896 RVA: 0x0001FDAC File Offset: 0x0001DFAC
	private void SetupAdditionalLight()
	{
		if (this.Components.AdditionalDirectLight)
		{
			this.AdditionalLight = this.Components.AdditionalDirectLight.GetComponent<Light>();
			if (EnviroSkyMgr.instance.dontDestroy && Application.isPlaying)
			{
				Object.DontDestroyOnLoad(this.Components.AdditionalDirectLight);
				return;
			}
		}
		else
		{
			GameObject gameObject = GameObject.Find("Enviro Directional Light - Moon");
			if (gameObject != null)
			{
				this.Components.AdditionalDirectLight = gameObject.transform;
			}
			else
			{
				this.Components.AdditionalDirectLight = base.CreateDirectionalLight(true);
			}
			this.AdditionalLight = this.Components.DirectLight.GetComponent<Light>();
			if (EnviroSkyMgr.instance.dontDestroy && Application.isPlaying)
			{
				Object.DontDestroyOnLoad(this.Components.AdditionalDirectLight);
			}
		}
	}

	// Token: 0x06000381 RID: 897 RVA: 0x0001FE80 File Offset: 0x0001E080
	private void Update()
	{
		if (this.profile == null)
		{
			Debug.Log("No profile applied! Please create and assign a profile.");
			return;
		}
		if (!this.started && !this.serverMode)
		{
			base.UpdateTime(this.GameTime.DaysInYear);
			base.UpdateSunAndMoonPosition();
			base.UpdateSceneView();
			base.CalculateDirectLight();
			base.UpdateAmbientLight();
			base.UpdateReflections();
			if (!this.AssignInRuntime || !(this.PlayerTag != "") || !(this.CameraTag != "") || !Application.isPlaying)
			{
				this.started = false;
				return;
			}
			GameObject gameObject = GameObject.FindGameObjectWithTag(this.PlayerTag);
			if (gameObject != null)
			{
				this.Player = gameObject;
			}
			for (int i = 0; i < Camera.allCameras.Length; i++)
			{
				if (Camera.allCameras[i].tag == this.CameraTag)
				{
					this.PlayerCamera = Camera.allCameras[i];
				}
			}
			if (!(this.Player != null) || !(this.PlayerCamera != null))
			{
				this.started = false;
				return;
			}
			this.Init();
			this.started = true;
		}
		base.UpdateTime(this.GameTime.DaysInYear);
		this.ValidateParameters();
		if (!this.serverMode)
		{
			base.UpdateSceneView();
			base.UpdateAmbientLight();
			base.UpdateReflections();
			this.UpdateWeather();
			base.UpdateParticleClouds(this.useParticleClouds);
			base.UpdateSunAndMoonPosition();
			base.CalculateDirectLight();
			this.SetMaterialsVariables();
			base.CalculateSatPositions(this.LST);
			if (this.skySettings.renderMoon && !this.Components.Moon.activeSelf)
			{
				this.Components.Moon.SetActive(true);
			}
			else if (!this.skySettings.renderMoon && this.Components.Moon.activeSelf)
			{
				this.Components.Moon.SetActive(false);
			}
			if (this.EnviroSkyRender == null && this.PlayerCamera != null)
			{
				this.InitImageEffects();
			}
			if (this.fogSettings.useUnityFog && this.PlayerCamera != null && this.PlayerCamera.renderingPath == RenderingPath.Forward)
			{
				RenderSettings.fog = true;
				if (this.EnviroSkyRender != null && this.EnviroSkyRender.isActiveAndEnabled)
				{
					this.EnviroSkyRender.enabled = false;
				}
			}
			else if (this.usePostEffectFog && this.EnviroSkyRender != null && !this.EnviroSkyRender.isActiveAndEnabled)
			{
				this.EnviroSkyRender.enabled = true;
			}
			else if (!this.usePostEffectFog && this.EnviroSkyRender != null && this.EnviroSkyRender.isActiveAndEnabled)
			{
				this.EnviroSkyRender.enabled = false;
			}
			if (!this.isNight && this.GameTime.solarTime < this.GameTime.dayNightSwitch)
			{
				this.isNight = true;
				if (this.Audio.AudioSourceAmbient != null)
				{
					base.TryPlayAmbientSFX();
				}
				EnviroSkyMgr.instance.NotifyIsNight();
				return;
			}
			if (this.isNight && this.GameTime.solarTime >= this.GameTime.dayNightSwitch)
			{
				this.isNight = false;
				if (this.Audio.AudioSourceAmbient != null)
				{
					base.TryPlayAmbientSFX();
				}
				EnviroSkyMgr.instance.NotifyIsDay();
				return;
			}
		}
		else
		{
			this.UpdateWeather();
			if (!this.isNight && this.GameTime.solarTime < this.GameTime.dayNightSwitch)
			{
				this.isNight = true;
				EnviroSkyMgr.instance.NotifyIsNight();
				return;
			}
			if (this.isNight && this.GameTime.solarTime >= this.GameTime.dayNightSwitch)
			{
				this.isNight = false;
				EnviroSkyMgr.instance.NotifyIsDay();
			}
		}
	}

	// Token: 0x06000382 RID: 898 RVA: 0x00020260 File Offset: 0x0001E460
	private void LateUpdate()
	{
		if (!this.serverMode && this.PlayerCamera != null && this.Player != null)
		{
			base.transform.position = this.Player.transform.position;
			float num = this.PlayerCamera.farClipPlane - this.PlayerCamera.farClipPlane * 0.1f;
			base.transform.localScale = new Vector3(num, num, num);
			if (this.EffectsHolder != null)
			{
				this.EffectsHolder.transform.position = this.Player.transform.position;
			}
		}
	}

	// Token: 0x06000383 RID: 899 RVA: 0x00020314 File Offset: 0x0001E514
	private void SetMaterialsVariables()
	{
		this.skyMat.SetColor("_SkyColor", this.skySettings.simpleSkyColor.Evaluate(this.GameTime.solarTime));
		this.skyMat.SetColor("_HorizonColor", this.skySettings.simpleHorizonColor.Evaluate(this.GameTime.solarTime));
		this.skyMat.SetColor("_SunColor", this.skySettings.simpleSunColor.Evaluate(this.GameTime.solarTime));
		this.skyMat.SetFloat("_SunDiskSizeSimple", this.skySettings.simpleSunDiskSize.Evaluate(this.GameTime.solarTime));
		this.skyMat.SetFloat("_StarsIntensity", this.skySettings.starsIntensity.Evaluate(this.GameTime.solarTime));
		this.skyMat.SetVector("_CloudAnimation", this.cirrusAnim);
		if (this.cloudsSettings.cirrusCloudsTexture != null)
		{
			this.skyMat.SetTexture("_CloudMap", this.cloudsSettings.cirrusCloudsTexture);
		}
		this.skyMat.SetColor("_CloudColor", this.cloudsSettings.cirrusCloudsColor.Evaluate(this.GameTime.solarTime));
		this.skyMat.SetFloat("_CloudAltitude", this.cloudsSettings.cirrusCloudsAltitude);
		this.skyMat.SetFloat("_CloudAlpha", this.cloudsConfig.cirrusAlpha);
		this.skyMat.SetFloat("_CloudCoverage", this.cloudsConfig.cirrusCoverage);
		this.skyMat.SetFloat("_CloudColorPower", this.cloudsConfig.cirrusColorPow);
		Shader.SetGlobalVector("_SunDir", -this.Components.Sun.transform.forward);
		Shader.SetGlobalColor("_EnviroLighting", this.lightSettings.LightColor.Evaluate(this.GameTime.solarTime));
		Shader.SetGlobalVector("_SunPosition", this.Components.Sun.transform.localPosition + -this.Components.Sun.transform.forward * 10000f);
		Shader.SetGlobalVector("_MoonPosition", this.Components.Moon.transform.localPosition);
		Shader.SetGlobalColor("_weatherSkyMod", Color.Lerp(this.currentWeatherSkyMod, this.interiorZoneSettings.currentInteriorSkyboxMod, this.interiorZoneSettings.currentInteriorSkyboxMod.a));
		Shader.SetGlobalColor("_weatherFogMod", Color.Lerp(this.currentWeatherFogMod, this.interiorZoneSettings.currentInteriorFogColorMod, this.interiorZoneSettings.currentInteriorFogColorMod.a));
		Shader.SetGlobalVector("_EnviroSkyFog", new Vector4(this.Fog.skyFogHeight, this.Fog.skyFogIntensity, this.Fog.skyFogStart, this.fogSettings.heightFogIntensity));
		Shader.SetGlobalFloat("_distanceFogIntensity", this.fogSettings.distanceFogIntensity);
		if (Application.isPlaying)
		{
			Shader.SetGlobalFloat("_maximumFogDensity", 1f - this.fogSettings.maximumFogDensity);
		}
		else
		{
			Shader.SetGlobalFloat("_maximumFogDensity", 1f);
		}
		if (this.fogSettings.useSimpleFog)
		{
			Shader.EnableKeyword("ENVIRO_SIMPLE_FOG");
			Shader.SetGlobalVector("_EnviroParams", new Vector4(Mathf.Clamp(1f - this.GameTime.solarTime, 0.5f, 1f), this.fogSettings.distanceFog ? 1f : 0f, 0f, this.HDR ? 1f : 0f));
		}
		else
		{
			Shader.SetGlobalColor("_scatteringColor", this.skySettings.scatteringColor.Evaluate(this.GameTime.solarTime));
			Shader.SetGlobalFloat("_scatteringStrenght", this.Fog.scatteringStrenght);
			Shader.SetGlobalFloat("_scatteringPower", this.skySettings.scatteringCurve.Evaluate(this.GameTime.solarTime));
			Shader.SetGlobalFloat("_SunBlocking", this.Fog.sunBlocking);
			Shader.SetGlobalVector("_EnviroParams", new Vector4(Mathf.Clamp(1f - this.GameTime.solarTime, 0.5f, 1f), this.fogSettings.distanceFog ? 1f : 0f, this.fogSettings.heightFog ? 1f : 0f, this.HDR ? 1f : 0f));
			Shader.SetGlobalVector("_Bm", base.BetaMie(this.skySettings.turbidity, this.skySettings.waveLength) * (this.skySettings.mie * (this.Fog.scatteringStrenght * this.GameTime.solarTime)));
			Shader.SetGlobalVector("_BmScene", base.BetaMie(this.skySettings.turbidity, this.skySettings.waveLength) * (this.fogSettings.mie * (this.Fog.scatteringStrenght * this.GameTime.solarTime)));
			Shader.SetGlobalVector("_Br", base.BetaRay(this.skySettings.waveLength) * this.skySettings.rayleigh);
			Shader.SetGlobalVector("_mieG", base.GetMieG(this.skySettings.g));
			Shader.SetGlobalVector("_mieGScene", base.GetMieGScene(this.skySettings.g));
			Shader.SetGlobalVector("_SunParameters", new Vector4(this.skySettings.sunIntensity, this.skySettings.sunDiskScale, this.skySettings.sunDiskIntensity, 0f));
			Shader.SetGlobalFloat("_Exposure", this.skySettings.skyExposure);
			Shader.SetGlobalFloat("_SkyLuminance", this.skySettings.skyLuminence.Evaluate(this.GameTime.solarTime));
			Shader.SetGlobalFloat("_SkyColorPower", this.skySettings.skyColorPower.Evaluate(this.GameTime.solarTime));
			Shader.SetGlobalFloat("_heightFogIntensity", this.fogSettings.heightFogIntensity);
			Shader.SetGlobalFloat("_lightning", this.thunder);
			Shader.DisableKeyword("ENVIRO_SIMPLE_FOG");
		}
		Shader.DisableKeyword("ENVIROVOLUMELIGHT");
		if (this.MoonShader != null)
		{
			this.MoonShader.SetFloat("_Phase", this.customMoonPhase);
			this.MoonShader.SetColor("_Color", this.skySettings.moonColor);
			this.MoonShader.SetFloat("_Brightness", this.skySettings.moonBrightness * (1f - this.GameTime.solarTime));
			this.MoonShader.SetFloat("_moonFogIntensity", this.Fog.moonIntensity);
		}
	}

	// Token: 0x06000384 RID: 900 RVA: 0x00020A4C File Offset: 0x0001EC4C
	private void ValidateParameters()
	{
		this.internalHour = Mathf.Repeat(this.internalHour, 24f);
		this.GameTime.Longitude = Mathf.Clamp(this.GameTime.Longitude, -180f, 180f);
		this.GameTime.Latitude = Mathf.Clamp(this.GameTime.Latitude, -90f, 90f);
	}

	// Token: 0x06000385 RID: 901 RVA: 0x00020ABC File Offset: 0x0001ECBC
	private void UpdateClouds(EnviroWeatherPreset i, bool withTransition)
	{
		if (i == null)
		{
			return;
		}
		float num = 500f * Time.deltaTime;
		if (withTransition)
		{
			num = this.weatherSettings.cloudTransitionSpeed * Time.deltaTime;
		}
		this.cloudsConfig.cirrusAlpha = Mathf.Lerp(this.cloudsConfig.cirrusAlpha, i.cloudsConfig.cirrusAlpha, num);
		this.cloudsConfig.cirrusCoverage = Mathf.Lerp(this.cloudsConfig.cirrusCoverage, i.cloudsConfig.cirrusCoverage, num);
		this.cloudsConfig.cirrusColorPow = Mathf.Lerp(this.cloudsConfig.cirrusColorPow, i.cloudsConfig.cirrusColorPow, num);
		this.cloudsConfig.particleLayer1Alpha = Mathf.Lerp(this.cloudsConfig.particleLayer1Alpha, i.cloudsConfig.particleLayer1Alpha, num);
		this.cloudsConfig.particleLayer1Brightness = Mathf.Lerp(this.cloudsConfig.particleLayer1Brightness, i.cloudsConfig.particleLayer1Brightness, num);
		this.cloudsConfig.particleLayer1ColorPow = Mathf.Lerp(this.cloudsConfig.particleLayer1ColorPow, i.cloudsConfig.particleLayer1ColorPow, num);
		this.cloudsConfig.particleLayer2Alpha = Mathf.Lerp(this.cloudsConfig.particleLayer2Alpha, i.cloudsConfig.particleLayer2Alpha, num);
		this.cloudsConfig.particleLayer2Brightness = Mathf.Lerp(this.cloudsConfig.particleLayer2Brightness, i.cloudsConfig.particleLayer2Brightness, num);
		this.cloudsConfig.particleLayer2ColorPow = Mathf.Lerp(this.cloudsConfig.particleLayer2ColorPow, i.cloudsConfig.particleLayer2ColorPow, num);
		this.shadowIntensityMod = Mathf.Lerp(this.shadowIntensityMod, i.shadowIntensityMod, num);
		this.currentWeatherSkyMod = Color.Lerp(this.currentWeatherSkyMod, i.weatherSkyMod.Evaluate(this.GameTime.solarTime), num);
		this.currentWeatherFogMod = Color.Lerp(this.currentWeatherFogMod, i.weatherFogMod.Evaluate(this.GameTime.solarTime), num * 10f);
		this.currentWeatherLightMod = Color.Lerp(this.currentWeatherLightMod, i.weatherLightMod.Evaluate(this.GameTime.solarTime), num);
	}

	// Token: 0x06000386 RID: 902 RVA: 0x00020CEC File Offset: 0x0001EEEC
	private void UpdateFog(EnviroWeatherPreset i, bool withTransition)
	{
		RenderSettings.fogColor = Color.Lerp(Color.Lerp(this.fogSettings.simpleFogColor.Evaluate(this.GameTime.solarTime), this.customFogColor, this.customFogIntensity), this.currentWeatherFogMod, this.currentWeatherFogMod.a);
		if (i != null)
		{
			float t = 500f * Time.deltaTime;
			if (withTransition)
			{
				t = this.weatherSettings.fogTransitionSpeed * Time.deltaTime;
			}
			if (this.fogSettings.Fogmode == FogMode.Linear)
			{
				RenderSettings.fogEndDistance = Mathf.Lerp(RenderSettings.fogEndDistance, i.fogDistance, t);
				RenderSettings.fogStartDistance = Mathf.Lerp(RenderSettings.fogStartDistance, i.fogStartDistance, t);
			}
			else
			{
				float num = i.fogDensity;
				if (this.fogSettings.useUnityFog)
				{
					num *= this.fogSettings.distanceFogIntensity;
				}
				if (this.updateFogDensity)
				{
					RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, num, t) * this.interiorZoneSettings.currentInteriorFogMod;
				}
			}
			this.Fog.scatteringStrenght = Mathf.Lerp(this.Fog.scatteringStrenght, i.FogScatteringIntensity, t);
			this.Fog.sunBlocking = Mathf.Lerp(this.Fog.sunBlocking, i.fogSunBlocking, t);
			this.Fog.moonIntensity = Mathf.Lerp(this.Fog.moonIntensity, i.moonIntensity, t);
			this.fogSettings.heightDensity = Mathf.Lerp(this.fogSettings.heightDensity, i.heightFogDensity, t);
			this.Fog.skyFogStart = Mathf.Lerp(this.Fog.skyFogStart, i.skyFogStart, t);
			this.Fog.skyFogHeight = Mathf.Lerp(this.Fog.skyFogHeight, i.SkyFogHeight, t);
			this.Fog.skyFogIntensity = Mathf.Lerp(this.Fog.skyFogIntensity, i.SkyFogIntensity, t);
		}
	}

	// Token: 0x06000387 RID: 903 RVA: 0x00020EE0 File Offset: 0x0001F0E0
	private void UpdateEffectSystems(EnviroWeatherPrefab id, bool withTransition)
	{
		if (id != null)
		{
			float t = 500f * Time.deltaTime;
			if (withTransition)
			{
				t = this.weatherSettings.effectTransitionSpeed * Time.deltaTime;
			}
			for (int i = 0; i < id.effectSystems.Count; i++)
			{
				if (id.effectSystems[i].isStopped)
				{
					id.effectSystems[i].Play();
				}
				float emissionRate = Mathf.Lerp(EnviroSkyMgr.instance.GetEmissionRate(id.effectSystems[i]), id.effectEmmisionRates[i] * this.qualitySettings.GlobalParticleEmissionRates, t) * this.interiorZoneSettings.currentInteriorWeatherEffectMod;
				EnviroSkyMgr.instance.SetEmissionRate(id.effectSystems[i], emissionRate);
			}
			for (int j = 0; j < this.Weather.WeatherPrefabs.Count; j++)
			{
				if (this.Weather.WeatherPrefabs[j].gameObject != id.gameObject)
				{
					for (int k = 0; k < this.Weather.WeatherPrefabs[j].effectSystems.Count; k++)
					{
						float num = Mathf.Lerp(EnviroSkyMgr.instance.GetEmissionRate(this.Weather.WeatherPrefabs[j].effectSystems[k]), 0f, t);
						if (num < 1f)
						{
							num = 0f;
						}
						EnviroSkyMgr.instance.SetEmissionRate(this.Weather.WeatherPrefabs[j].effectSystems[k], num);
						if (num == 0f && !this.Weather.WeatherPrefabs[j].effectSystems[k].isStopped)
						{
							this.Weather.WeatherPrefabs[j].effectSystems[k].Stop();
						}
					}
				}
			}
			base.UpdateWeatherVariables(id.weatherPreset);
		}
	}

	// Token: 0x06000388 RID: 904 RVA: 0x000210F4 File Offset: 0x0001F2F4
	private void UpdateWeather()
	{
		if (this.Weather.currentActiveWeatherPreset != this.Weather.currentActiveZone.currentActiveZoneWeatherPreset)
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
					base.TryPlayAmbientSFX();
					base.UpdateAudioSource(this.Weather.currentActiveWeatherPreset);
					if (this.Weather.currentActiveWeatherPreset.isLightningStorm)
					{
						base.StartCoroutine(base.PlayThunderRandom());
					}
					else
					{
						base.StopCoroutine(base.PlayThunderRandom());
						this.Components.LightningGenerator.StopLightning();
					}
				}
			}
		}
		if (this.Weather.currentActiveWeatherPrefab != null && !this.serverMode)
		{
			this.UpdateClouds(this.Weather.currentActiveWeatherPreset, true);
			this.UpdateFog(this.Weather.currentActiveWeatherPreset, true);
			this.UpdateEffectSystems(this.Weather.currentActiveWeatherPrefab, true);
			if (!this.Weather.weatherFullyChanged)
			{
				base.CalcWeatherTransitionState();
				return;
			}
		}
		else if (this.Weather.currentActiveWeatherPrefab != null)
		{
			base.UpdateWeatherVariables(this.Weather.currentActiveWeatherPrefab.weatherPreset);
		}
	}

	// Token: 0x06000389 RID: 905 RVA: 0x000212A6 File Offset: 0x0001F4A6
	public void InstantWeatherChange(EnviroWeatherPreset preset, EnviroWeatherPrefab prefab)
	{
		this.UpdateClouds(preset, false);
		this.UpdateFog(preset, false);
		this.UpdateEffectSystems(prefab, false);
	}

	// Token: 0x0600038A RID: 906 RVA: 0x000212C0 File Offset: 0x0001F4C0
	public void AssignAndStart(GameObject player, Camera Camera)
	{
		this.Player = player;
		this.PlayerCamera = Camera;
		this.Init();
		this.started = true;
	}

	// Token: 0x0600038B RID: 907 RVA: 0x000212DD File Offset: 0x0001F4DD
	public void StartAsServer()
	{
		this.Player = base.gameObject;
		this.serverMode = true;
		this.Init();
	}

	// Token: 0x0600038C RID: 908 RVA: 0x000212F8 File Offset: 0x0001F4F8
	public void ChangeFocus(GameObject player, Camera Camera)
	{
		this.Player = player;
		this.RemoveEnviroCameraComponents(this.PlayerCamera);
		this.PlayerCamera = Camera;
		this.InitImageEffects();
	}

	// Token: 0x0600038D RID: 909 RVA: 0x0002131C File Offset: 0x0001F51C
	private void RemoveEnviroCameraComponents(Camera cam)
	{
		EnviroSkyRenderingLW component = cam.GetComponent<EnviroSkyRenderingLW>();
		if (component != null)
		{
			Object.Destroy(component);
		}
		EnviroPostProcessing component2 = cam.GetComponent<EnviroPostProcessing>();
		if (component2 != null)
		{
			Object.Destroy(component2);
		}
	}

	// Token: 0x0600038E RID: 910 RVA: 0x00021358 File Offset: 0x0001F558
	public void Play(EnviroTime.TimeProgressMode progressMode = EnviroTime.TimeProgressMode.Simulated)
	{
		this.SetupSkybox();
		if (!this.Components.DirectLight.gameObject.activeSelf)
		{
			this.Components.DirectLight.gameObject.SetActive(true);
		}
		this.GameTime.ProgressTime = progressMode;
		if (this.EffectsHolder != null)
		{
			this.EffectsHolder.SetActive(true);
		}
		if (this.EnviroSkyRender != null)
		{
			this.EnviroSkyRender.enabled = true;
		}
		this.started = true;
		base.TryPlayAmbientSFX();
	}

	// Token: 0x0600038F RID: 911 RVA: 0x000213E8 File Offset: 0x0001F5E8
	public void Stop(bool disableLight = false, bool stopTime = true)
	{
		if (disableLight)
		{
			this.Components.DirectLight.gameObject.SetActive(false);
		}
		if (stopTime)
		{
			this.GameTime.ProgressTime = EnviroTime.TimeProgressMode.None;
		}
		if (this.EffectsHolder != null)
		{
			this.EffectsHolder.SetActive(false);
		}
		if (this.EnviroSkyRender != null)
		{
			this.EnviroSkyRender.enabled = false;
		}
		if (this.EnviroPostProcessing != null)
		{
			this.EnviroPostProcessing.enabled = false;
		}
		this.started = false;
	}

	// Token: 0x06000390 RID: 912 RVA: 0x00021474 File Offset: 0x0001F674
	public void Deactivate(bool disableLight = false)
	{
		if (disableLight)
		{
			this.Components.DirectLight.gameObject.SetActive(false);
		}
		if (this.EffectsHolder != null)
		{
			this.EffectsHolder.SetActive(false);
		}
		if (this.EnviroSkyRender != null)
		{
			this.EnviroSkyRender.enabled = false;
		}
		if (this.EnviroPostProcessing != null)
		{
			this.EnviroPostProcessing.enabled = false;
		}
	}

	// Token: 0x06000391 RID: 913 RVA: 0x000214E8 File Offset: 0x0001F6E8
	public void Activate()
	{
		this.Components.DirectLight.gameObject.SetActive(true);
		if (this.EffectsHolder != null)
		{
			this.EffectsHolder.SetActive(true);
		}
		if (this.EnviroSkyRender != null)
		{
			this.EnviroSkyRender.enabled = true;
		}
		if (this.EnviroPostProcessing != null)
		{
			this.EnviroPostProcessing.enabled = true;
		}
		base.TryPlayAmbientSFX();
		if (this.Weather.currentAudioSource != null)
		{
			this.Weather.currentAudioSource.audiosrc.Play();
		}
	}

	// Token: 0x04000795 RID: 1941
	private static EnviroSkyLite _instance;

	// Token: 0x04000796 RID: 1942
	public string prefabVersion = "2.1.2";

	// Token: 0x04000797 RID: 1943
	public bool useParticleClouds;

	// Token: 0x04000798 RID: 1944
	public bool usePostEffectFog = true;

	// Token: 0x04000799 RID: 1945
	[HideInInspector]
	public EnviroSkyRenderingLW EnviroSkyRender;

	// Token: 0x0400079A RID: 1946
	[HideInInspector]
	public Material skyMat;

	// Token: 0x0400079B RID: 1947
	private double lastMoonUpdate;

	// Token: 0x0400079C RID: 1948
	[HideInInspector]
	public bool showSettings;
}
