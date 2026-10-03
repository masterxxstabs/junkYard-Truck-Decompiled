using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020000A3 RID: 163
[ExecuteInEditMode]
public class EnviroSky : EnviroCore
{
	// Token: 0x17000036 RID: 54
	// (get) Token: 0x060003A6 RID: 934 RVA: 0x00022022 File Offset: 0x00020222
	public static EnviroSky instance
	{
		get
		{
			if (EnviroSky._instance == null)
			{
				EnviroSky._instance = Object.FindObjectOfType<EnviroSky>();
			}
			return EnviroSky._instance;
		}
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x00022040 File Offset: 0x00020240
	private void Start()
	{
		if (EnviroSkyMgr.instance == null)
		{
			Debug.Log("Please use the EnviroSky Manager!");
			base.gameObject.SetActive(false);
			return;
		}
		this.started = false;
		base.SetTime(this.GameTime.Years, this.GameTime.Days, this.GameTime.Hours, this.GameTime.Minutes, this.GameTime.Seconds);
		this.lastHourUpdate = (float)Mathf.RoundToInt(this.internalHour);
		this.currentTimeInHours = base.GetInHours(this.internalHour, (float)this.GameTime.Days, (float)this.GameTime.Years, this.GameTime.DaysInYear);
		this.Weather.weatherFullyChanged = false;
		this.thunder = 0f;
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
		if (this.profileLoaded)
		{
			base.InvokeRepeating("UpdateEnviroment", 0f, this.qualitySettings.UpdateInterval);
			if (this.PlayerCamera != null && this.Player != null && !this.AssignInRuntime && this.startMode == EnviroCore.EnviroStartMode.Started)
			{
				this.Init();
			}
		}
	}

	// Token: 0x060003A8 RID: 936 RVA: 0x00022214 File Offset: 0x00020414
	private IEnumerator SetSceneSettingsLate()
	{
		yield return 0;
		if (this.skyMat != null && RenderSettings.skybox != this.skyMat)
		{
			this.SetupSkybox();
		}
		if (RenderSettings.fogMode != this.fogSettings.Fogmode)
		{
			RenderSettings.fogMode = this.fogSettings.Fogmode;
		}
		if (RenderSettings.ambientMode != this.lightSettings.ambientMode)
		{
			RenderSettings.ambientMode = this.lightSettings.ambientMode;
		}
		yield break;
	}

	// Token: 0x060003A9 RID: 937 RVA: 0x00022224 File Offset: 0x00020424
	private void OnEnable()
	{
		if (EnviroSkyMgr.instance == null)
		{
			return;
		}
		this.Weather.currentActiveWeatherPreset = this.Weather.zones[0].currentActiveZoneWeatherPreset;
		this.Weather.lastActiveWeatherPreset = this.Weather.currentActiveWeatherPreset;
		if (this.weatherMapMat == null)
		{
			this.weatherMapMat = new Material(Shader.Find("Enviro/Standard/WeatherMap"));
		}
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
		}
		else if (this.PlayerCamera != null && this.Player != null && this.startMode == EnviroCore.EnviroStartMode.Started)
		{
			this.Init();
		}
		this.PopulateCloudsQualityList();
		if (this.currentActiveCloudsQualityPreset != null)
		{
			this.ApplyVolumeCloudsQualityPreset(this.currentActiveCloudsQualityPreset);
		}
	}

	// Token: 0x060003AA RID: 938 RVA: 0x00022327 File Offset: 0x00020527
	public void ReInit()
	{
		this.OnEnable();
	}

	// Token: 0x060003AB RID: 939 RVA: 0x00022330 File Offset: 0x00020530
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
		base.CreateEffects("Enviro Effects");
		if (this.weatherSettings.lightningEffect != null && this.lightningEffect == null)
		{
			this.lightningEffect = Object.Instantiate<GameObject>(this.weatherSettings.lightningEffect, this.EffectsHolder.transform).GetComponent<ParticleSystem>();
		}
		if (this.serverMode)
		{
			return;
		}
		base.CheckSatellites();
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
			this.customMoonPhase = this.skySettings.startMoonPhase;
		}
		else
		{
			Debug.LogError("Please set moon object in inspector!");
		}
		if (this.weatherMap != null)
		{
			Object.DestroyImmediate(this.weatherMap);
		}
		if (this.weatherMap == null)
		{
			this.weatherMap = new RenderTexture(512, 512, 0, RenderTextureFormat.Default);
			this.weatherMap.wrapMode = TextureWrapMode.Repeat;
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
		if (this.cloudShadowMap != null)
		{
			Object.DestroyImmediate(this.cloudShadowMap);
		}
		this.cloudShadowMap = new RenderTexture(2048, 2048, 0, RenderTextureFormat.Default);
		this.cloudShadowMap.wrapMode = TextureWrapMode.Repeat;
		if (this.cloudShadowMat != null)
		{
			Object.DestroyImmediate(this.cloudShadowMat);
		}
		this.cloudShadowMat = new Material(Shader.Find("Enviro/Standard/ShadowCookie"));
		if (this.cloudsSettings.shadowIntensity > 0f)
		{
			Graphics.Blit(this.weatherMap, this.cloudShadowMap, this.cloudShadowMat);
			this.MainLight.cookie = this.cloudShadowMap;
			this.MainLight.cookieSize = 10000f;
		}
		else
		{
			this.MainLight.cookie = null;
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

	// Token: 0x060003AC RID: 940 RVA: 0x000226DC File Offset: 0x000208DC
	public void SetupSkybox()
	{
		if (this.skySettings.skyboxMode == EnviroSkySettings.SkyboxModi.Simple)
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
		else if (this.skySettings.skyboxMode == EnviroSkySettings.SkyboxModi.Default)
		{
			if (this.skyMat != null)
			{
				Object.DestroyImmediate(this.skyMat);
			}
			if (!this.useFlatClouds)
			{
				this.skyMat = new Material(Shader.Find("Enviro/Standard/Skybox"));
				this.flatCloudsSkybox = false;
			}
			else
			{
				this.skyMat = new Material(Shader.Find("Enviro/Standard/SkyboxFlatClouds"));
				this.flatCloudsSkybox = true;
			}
			if (this.skySettings.starsCubeMap != null)
			{
				this.skyMat.SetTexture("_Stars", this.skySettings.starsCubeMap);
			}
			if (this.skySettings.galaxyCubeMap != null)
			{
				this.skyMat.SetTexture("_Galaxy", this.skySettings.galaxyCubeMap);
			}
			Cubemap cubemap = Resources.Load("cube_enviro_starsNoise") as Cubemap;
			if (cubemap != null)
			{
				this.skyMat.SetTexture("_StarsTwinklingNoise", cubemap);
			}
			Texture2D texture2D = Resources.Load("tex_enviro_dither") as Texture2D;
			if (texture2D != null)
			{
				this.skyMat.SetTexture("_DitheringTex", texture2D);
			}
			Texture2D texture2D2 = Resources.Load("tex_enviro_aurora_layer_1") as Texture2D;
			if (texture2D2 != null)
			{
				this.skyMat.SetTexture("_Aurora_Layer_1", texture2D2);
			}
			Texture2D texture2D3 = Resources.Load("tex_enviro_aurora_layer_2") as Texture2D;
			if (texture2D3 != null)
			{
				this.skyMat.SetTexture("_Aurora_Layer_2", texture2D3);
			}
			Texture2D texture2D4 = Resources.Load("tex_enviro_aurora_colorshift") as Texture2D;
			if (texture2D4 != null)
			{
				this.skyMat.SetTexture("_Aurora_Colorshift", texture2D4);
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

	// Token: 0x060003AD RID: 941 RVA: 0x00022988 File Offset: 0x00020B88
	private IEnumerator UpdateAmbientLightWithDelay()
	{
		yield return 0;
		DynamicGI.UpdateEnvironment();
		yield break;
	}

	// Token: 0x060003AE RID: 942 RVA: 0x00022990 File Offset: 0x00020B90
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
		if (this.skyMat != null && RenderSettings.skybox != this.skyMat)
		{
			this.SetupSkybox();
		}
		else if (this.skyMat == null)
		{
			this.SetupSkybox();
		}
		if (RenderSettings.fogMode != this.fogSettings.Fogmode)
		{
			RenderSettings.fogMode = this.fogSettings.Fogmode;
		}
		if (RenderSettings.ambientMode != this.lightSettings.ambientMode)
		{
			RenderSettings.ambientMode = this.lightSettings.ambientMode;
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
		}
		if (this.satelliteSettings.additionalSatellites.Count > 0)
		{
			this.CreateSatCamera();
		}
		this.started = true;
	}

	// Token: 0x060003AF RID: 943 RVA: 0x00022AB4 File Offset: 0x00020CB4
	private void InitImageEffects()
	{
		this.EnviroSkyRender = this.PlayerCamera.gameObject.GetComponent<EnviroSkyRendering>();
		if (this.EnviroSkyRender == null)
		{
			this.EnviroSkyRender = this.PlayerCamera.gameObject.AddComponent<EnviroSkyRendering>();
		}
		this.EnviroPostProcessing = this.PlayerCamera.gameObject.GetComponent<EnviroPostProcessing>();
		if (this.EnviroPostProcessing == null)
		{
			this.EnviroPostProcessing = this.PlayerCamera.gameObject.AddComponent<EnviroPostProcessing>();
		}
	}

	// Token: 0x060003B0 RID: 944 RVA: 0x00022B38 File Offset: 0x00020D38
	public void CreateSatCamera()
	{
		Camera[] array = Object.FindObjectsOfType<Camera>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].cullingMask &= ~(1 << this.satelliteRenderingLayer);
		}
		Object.DestroyImmediate(GameObject.Find("Enviro Sat Camera"));
		this.satCamera = new GameObject
		{
			name = "Enviro Sat Camera",
			transform = 
			{
				position = this.PlayerCamera.transform.position,
				rotation = this.PlayerCamera.transform.rotation
			},
			hideFlags = HideFlags.DontSave
		}.AddComponent<Camera>();
		this.satCamera.farClipPlane = this.PlayerCamera.farClipPlane;
		this.satCamera.nearClipPlane = this.PlayerCamera.nearClipPlane;
		this.satCamera.aspect = this.PlayerCamera.aspect;
		base.SetCameraHDR(this.satCamera, this.HDR);
		this.satCamera.useOcclusionCulling = false;
		this.satCamera.renderingPath = RenderingPath.Forward;
		this.satCamera.fieldOfView = this.PlayerCamera.fieldOfView;
		this.satCamera.clearFlags = CameraClearFlags.Color;
		this.satCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
		this.satCamera.cullingMask = 1 << this.satelliteRenderingLayer;
		this.satCamera.depth = this.PlayerCamera.depth + 1f;
		this.satCamera.enabled = true;
		this.PlayerCamera.cullingMask &= ~(1 << this.satelliteRenderingLayer);
		RenderTextureFormat format = base.GetCameraHDR(this.satCamera) ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
		this.satRenderTarget = new RenderTexture(Screen.currentResolution.width, Screen.currentResolution.height, 16, format);
		this.satCamera.targetTexture = this.satRenderTarget;
		this.satCamera.enabled = false;
	}

	// Token: 0x060003B1 RID: 945 RVA: 0x00022D48 File Offset: 0x00020F48
	private void SetupMainLight()
	{
		if (this.Components.DirectLight)
		{
			this.MainLight = this.Components.DirectLight.GetComponent<Light>();
			if (this.directVolumeLight == null)
			{
				this.directVolumeLight = this.Components.DirectLight.GetComponent<EnviroVolumeLight>();
			}
			if (this.directVolumeLight == null)
			{
				this.directVolumeLight = this.Components.DirectLight.gameObject.AddComponent<EnviroVolumeLight>();
			}
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
			if (this.directVolumeLight == null)
			{
				this.directVolumeLight = this.Components.DirectLight.GetComponent<EnviroVolumeLight>();
			}
			if (this.directVolumeLight == null)
			{
				this.directVolumeLight = this.Components.DirectLight.gameObject.AddComponent<EnviroVolumeLight>();
			}
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

	// Token: 0x060003B2 RID: 946 RVA: 0x00022EF0 File Offset: 0x000210F0
	private void SetupAdditionalLight()
	{
		if (this.Components.AdditionalDirectLight)
		{
			this.AdditionalLight = this.Components.AdditionalDirectLight.GetComponent<Light>();
			if (this.additionalDirectVolumeLight == null)
			{
				this.additionalDirectVolumeLight = this.Components.AdditionalDirectLight.GetComponent<EnviroVolumeLight>();
			}
			if (this.additionalDirectVolumeLight == null)
			{
				this.additionalDirectVolumeLight = this.Components.AdditionalDirectLight.gameObject.AddComponent<EnviroVolumeLight>();
			}
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
			if (this.additionalDirectVolumeLight == null)
			{
				this.additionalDirectVolumeLight = this.Components.AdditionalDirectLight.GetComponent<EnviroVolumeLight>();
			}
			if (this.additionalDirectVolumeLight == null)
			{
				this.additionalDirectVolumeLight = this.Components.AdditionalDirectLight.gameObject.AddComponent<EnviroVolumeLight>();
			}
			if (EnviroSkyMgr.instance.dontDestroy && Application.isPlaying)
			{
				Object.DontDestroyOnLoad(this.Components.AdditionalDirectLight);
			}
		}
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x00023060 File Offset: 0x00021260
	private void RenderFlatCloudsMap()
	{
		if (this.flatCloudsMat == null)
		{
			this.flatCloudsMat = new Material(Shader.Find("Enviro/Standard/FlatCloudMap"));
		}
		this.flatCloudsRenderTarget = RenderTexture.GetTemporary((int)((EnviroCloudSettings.FlatCloudResolution)512 * (this.cloudsSettings.flatCloudsResolution + 1)), (int)((EnviroCloudSettings.FlatCloudResolution)512 * (this.cloudsSettings.flatCloudsResolution + 1)), 0, RenderTextureFormat.DefaultHDR);
		this.flatCloudsRenderTarget.wrapMode = TextureWrapMode.Repeat;
		this.flatCloudsMat.SetVector("_CloudAnimation", this.cloudAnimNonScaled);
		this.flatCloudsMat.SetTexture("_NoiseTex", this.cloudsSettings.flatCloudsNoiseTexture);
		this.flatCloudsMat.SetFloat("_CloudScale", this.cloudsSettings.flatCloudsScale);
		this.flatCloudsMat.SetFloat("_Coverage", this.cloudsConfig.flatCoverage);
		this.flatCloudsMat.SetInt("noiseOctaves", this.cloudsSettings.flatCloudsNoiseOctaves);
		this.flatCloudsMat.SetFloat("_Softness", this.cloudsConfig.flatSoftness);
		this.flatCloudsMat.SetFloat("_Brightness", this.cloudsConfig.flatBrightness);
		this.flatCloudsMat.SetFloat("_MorphingSpeed", this.cloudsSettings.flatCloudsMorphingSpeed);
		Graphics.Blit(null, this.flatCloudsRenderTarget, this.flatCloudsMat);
		RenderTexture.ReleaseTemporary(this.flatCloudsRenderTarget);
	}

	// Token: 0x060003B4 RID: 948 RVA: 0x000231C8 File Offset: 0x000213C8
	private void RenderWeatherMap()
	{
		if (this.cloudsSettings.customWeatherMap == null)
		{
			this.weatherMapMat.SetVector("_WindDir", this.cloudAnimNonScaled);
			this.weatherMapMat.SetFloat("_AnimSpeedScale", this.cloudsSettings.weatherAnimSpeedScale);
			this.weatherMapMat.SetInt("_Tiling", this.cloudsSettings.weatherMapTiling);
			this.weatherMapMat.SetVector("_Location", this.cloudsSettings.locationOffset);
			double value = (double)(this.cloudsConfig.coverage * this.cloudsSettings.globalCloudCoverage);
			this.weatherMapMat.SetFloat("_Coverage", (float)Math.Round(value, 4));
			this.weatherMapMat.SetFloat("_CloudsType", this.cloudsConfig.cloudType);
			this.weatherMapMat.SetFloat("_CoverageType", this.cloudsConfig.coverageType);
			Graphics.Blit(null, this.weatherMap, this.weatherMapMat);
		}
	}

	// Token: 0x060003B5 RID: 949 RVA: 0x000232D8 File Offset: 0x000214D8
	private void RenderCloudMaps()
	{
		if (Application.isPlaying)
		{
			if (this.useVolumeClouds)
			{
				this.RenderWeatherMap();
			}
			if (this.useFlatClouds)
			{
				this.RenderFlatCloudsMap();
				return;
			}
		}
		else
		{
			if (this.useVolumeClouds && this.showVolumeCloudsInEditor)
			{
				this.RenderWeatherMap();
			}
			if (this.useFlatClouds && this.showFlatCloudsInEditor)
			{
				this.RenderFlatCloudsMap();
			}
		}
	}

	// Token: 0x060003B6 RID: 950 RVA: 0x00023338 File Offset: 0x00021538
	private void Update()
	{
		if (this.profile == null)
		{
			Debug.Log("No profile applied! Please create and assign a profile.");
			return;
		}
		if (!Application.isPlaying && this.startMode != EnviroCore.EnviroStartMode.Started)
		{
			if (this.startMode == EnviroCore.EnviroStartMode.Paused)
			{
				this.Stop(true, true);
			}
			else
			{
				this.GameTime.ProgressTime = EnviroTime.TimeProgressMode.Simulated;
				this.Stop(true, false);
			}
		}
		else if (!Application.isPlaying && this.startMode == EnviroCore.EnviroStartMode.Started && !this.started)
		{
			this.Play(this.GameTime.ProgressTime);
		}
		if (!this.started && !this.serverMode)
		{
			base.UpdateTime(this.GameTime.DaysInYear);
			base.UpdateSunAndMoonPosition();
			base.UpdateSceneView();
			base.CalculateDirectLight();
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
			if (this.useFlatClouds != this.flatCloudsSkybox)
			{
				this.SetupSkybox();
			}
			base.UpdateSceneView();
			if (!Application.isPlaying && this.Weather.startWeatherPreset != null && this.startMode == EnviroCore.EnviroStartMode.Started)
			{
				this.UpdateClouds(this.Weather.startWeatherPreset, false);
				this.UpdateFog(this.Weather.startWeatherPreset, false);
				this.UpdatePostProcessing(this.Weather.startWeatherPreset, false);
				base.UpdateWeatherVariables(this.Weather.startWeatherPreset);
			}
			this.RenderCloudMaps();
			base.UpdateAmbientLight();
			base.UpdateReflections();
			this.UpdateWeather();
			if (this.Weather.currentActiveWeatherPreset != null && this.Weather.currentActiveWeatherPreset.cloudsConfig.particleCloudsOverwrite)
			{
				base.UpdateParticleClouds(true);
			}
			else
			{
				base.UpdateParticleClouds(this.useParticleClouds);
			}
			this.UpdateCloudShadows();
			this.UpdateSkyRenderingComponent();
			base.UpdateSunAndMoonPosition();
			base.CalculateDirectLight();
			this.SetMaterialsVariables();
			base.CalculateSatPositions(this.LST);
			if (this.directVolumeLight != null && !this.directVolumeLight.isActiveAndEnabled && this.volumeLightSettings.dirVolumeLighting)
			{
				this.directVolumeLight.enabled = true;
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

	// Token: 0x060003B7 RID: 951 RVA: 0x0002372C File Offset: 0x0002192C
	private void LateUpdate()
	{
		if (!this.serverMode && this.PlayerCamera != null && this.Player != null)
		{
			base.transform.position = this.Player.transform.position;
			base.transform.localScale = new Vector3(this.PlayerCamera.farClipPlane, this.PlayerCamera.farClipPlane, this.PlayerCamera.farClipPlane);
			if (this.EffectsHolder != null)
			{
				if (this.cloudsSettings.cloudsQualitySettings != null && this.Player.transform.position.y > this.cloudsSettings.cloudsQualitySettings.bottomCloudHeight + this.cloudsSettings.cloudsHeightMod)
				{
					this.EffectsHolder.transform.position = new Vector3(this.Player.transform.position.x, this.cloudsSettings.cloudsQualitySettings.bottomCloudHeight + this.cloudsSettings.cloudsHeightMod, this.Player.transform.position.z);
					return;
				}
				this.EffectsHolder.transform.position = this.Player.transform.position;
			}
		}
	}

	// Token: 0x060003B8 RID: 952 RVA: 0x00023884 File Offset: 0x00021A84
	private void UpdateCloudShadows()
	{
		if (this.cloudsSettings.shadowIntensity == 0f || !this.useVolumeClouds)
		{
			if (this.MainLight.cookie != null)
			{
				this.MainLight.cookie = null;
				return;
			}
		}
		else if (this.cloudsSettings.shadowIntensity > 0f)
		{
			this.cloudShadowMap.DiscardContents(true, true);
			this.cloudShadowMat.SetFloat("_shadowIntensity", this.cloudsSettings.shadowIntensity);
			if (this.useVolumeClouds)
			{
				this.cloudShadowMat.SetTexture("_MainTex", this.weatherMap);
				Graphics.Blit(this.weatherMap, this.cloudShadowMap, this.cloudShadowMat);
			}
			if (Application.isPlaying)
			{
				this.MainLight.cookie = this.cloudShadowMap;
			}
			else
			{
				this.MainLight.cookie = null;
			}
			this.MainLight.cookieSize = (float)this.cloudsSettings.shadowCookieSize;
		}
	}

	// Token: 0x060003B9 RID: 953 RVA: 0x00023980 File Offset: 0x00021B80
	private void SetMaterialsVariables()
	{
		if (this.skyMat != null)
		{
			if (this.skySettings.skyboxMode == EnviroSkySettings.SkyboxModi.Simple)
			{
				this.skyMat.SetColor("_SkyColor", this.skySettings.simpleSkyColor.Evaluate(this.GameTime.solarTime));
				this.skyMat.SetColor("_HorizonColor", this.skySettings.simpleHorizonColor.Evaluate(this.GameTime.solarTime));
				this.skyMat.SetColor("_SunColor", this.skySettings.simpleSunColor.Evaluate(this.GameTime.solarTime));
				this.skyMat.SetFloat("_SunDiskSizeSimple", this.skySettings.simpleSunDiskSize.Evaluate(this.GameTime.solarTime));
			}
			else
			{
				this.skyMat.SetVector("_SunDir", -this.Components.Sun.transform.forward);
				this.skyMat.SetVector("_MoonDir", this.Components.Moon.transform.forward);
				this.skyMat.SetColor("_MoonColor", this.skySettings.moonColor);
				this.skyMat.SetColor("_scatteringColor", this.skySettings.scatteringColor.Evaluate(this.GameTime.solarTime));
				this.skyMat.SetColor("_sunDiskColor", this.skySettings.sunDiskColor.Evaluate(this.GameTime.solarTime));
				this.skyMat.SetColor("_weatherSkyMod", Color.Lerp(this.currentWeatherSkyMod, this.interiorZoneSettings.currentInteriorSkyboxMod, this.interiorZoneSettings.currentInteriorSkyboxMod.a));
				this.skyMat.SetColor("_weatherFogMod", Color.Lerp(this.currentWeatherFogMod, this.interiorZoneSettings.currentInteriorFogColorMod, this.interiorZoneSettings.currentInteriorFogColorMod.a));
				this.skyMat.SetVector("_Bm", base.BetaMie(this.skySettings.turbidity, this.skySettings.waveLength) * (this.skySettings.mie * this.Fog.scatteringStrenght));
				this.skyMat.SetVector("_Br", base.BetaRay(this.skySettings.waveLength) * this.skySettings.rayleigh);
				this.skyMat.SetVector("_mieG", base.GetMieG(this.skySettings.g));
				this.skyMat.SetFloat("_SunIntensity", this.skySettings.sunIntensity);
				this.skyMat.SetFloat("_SunDiskSize", this.skySettings.sunDiskScale);
				this.skyMat.SetFloat("_SunDiskIntensity", this.skySettings.sunDiskIntensity);
				this.skyMat.SetFloat("_SunDiskSize", this.skySettings.sunDiskScale);
				this.skyMat.SetFloat("_Exposure", this.skySettings.skyExposure);
				this.skyMat.SetFloat("_SkyLuminance", this.skySettings.skyLuminence.Evaluate(this.GameTime.solarTime));
				this.skyMat.SetFloat("_scatteringPower", this.skySettings.scatteringCurve.Evaluate(this.GameTime.solarTime));
				this.skyMat.SetFloat("_SkyColorPower", this.skySettings.skyColorPower.Evaluate(this.GameTime.solarTime));
				this.skyMat.SetFloat("_StarsIntensity", this.skySettings.starsIntensity.Evaluate(this.GameTime.solarTime));
				this.skyMat.SetFloat("_GalaxyIntensity", this.skySettings.galaxyIntensity.Evaluate(this.GameTime.solarTime));
				if (this.skySettings.dithering)
				{
					this.skyMat.SetInt("_UseDithering", 1);
				}
				else
				{
					this.skyMat.SetInt("_UseDithering", 0);
				}
				if (this.skySettings.moonPhaseMode == EnviroSkySettings.MoonPhases.Realistic)
				{
					float num = Vector3.SignedAngle(this.Components.Moon.transform.forward, this.Components.Sun.transform.forward, base.transform.forward);
					if (this.GameTime.Latitude >= 0f)
					{
						if (num < 0f)
						{
							this.customMoonPhase = base.Remap(num, 0f, -180f, -2f, 0f);
						}
						else
						{
							this.customMoonPhase = base.Remap(num, 0f, 180f, 2f, 0f);
						}
					}
					else if (num < 0f)
					{
						this.customMoonPhase = base.Remap(num, 0f, -180f, 2f, 0f);
					}
					else
					{
						this.customMoonPhase = base.Remap(num, 0f, 180f, -2f, 0f);
					}
				}
				this.skyMat.SetColor("_moonGlowColor", this.skySettings.moonGlowColor);
				this.skyMat.SetVector("_moonParams", new Vector4(this.skySettings.moonSize, this.skySettings.glowSize, this.skySettings.moonGlow.Evaluate(this.GameTime.solarTime), this.customMoonPhase));
				if (this.skySettings.renderMoon)
				{
					this.skyMat.SetTexture("_MoonTex", this.skySettings.moonTexture);
					this.skyMat.SetTexture("_GlowTex", this.skySettings.glowTexture);
				}
				else
				{
					this.skyMat.SetTexture("_MoonTex", null);
					this.skyMat.SetTexture("_GlowTex", null);
				}
				if (this.skySettings.blackGroundMode)
				{
					this.skyMat.SetInt("_blackGround", 1);
				}
				else
				{
					this.skyMat.SetInt("_blackGround", 0);
				}
				float value = this.HDR ? 1f : 0f;
				this.skyMat.SetFloat("_hdr", value);
				this.skyMat.SetFloat("_StarsTwinkling", this.skySettings.starsTwinklingRate);
				if (this.skySettings.starsTwinklingRate > 0f)
				{
					this.starsTwinklingRot += this.skySettings.starsTwinklingRate * Time.deltaTime;
					Quaternion q = Quaternion.Euler(this.starsTwinklingRot, this.starsTwinklingRot, this.starsTwinklingRot);
					Matrix4x4 value2 = Matrix4x4.TRS(Vector3.zero, q, new Vector3(1f, 1f, 1f));
					this.skyMat.SetMatrix("_StarsTwinklingMatrix", value2);
				}
				if (this.useAurora)
				{
					this.skyMat.EnableKeyword("ENVIRO_AURORA");
					this.skyMat.SetFloat("_AuroraIntensity", Mathf.Clamp01(this.auroraIntensity * this.auroraSettings.auroraIntensity.Evaluate(this.GameTime.solarTime)));
					this.skyMat.SetFloat("_AuroraBrightness", this.auroraSettings.auroraBrightness);
					this.skyMat.SetFloat("_AuroraContrast", this.auroraSettings.auroraContrast);
					this.skyMat.SetColor("_AuroraColor", this.auroraSettings.auroraColor);
					this.skyMat.SetFloat("_AuroraHeight", this.auroraSettings.auroraHeight);
					this.skyMat.SetFloat("_AuroraScale", this.auroraSettings.auroraScale);
					this.skyMat.SetFloat("_AuroraSpeed", this.auroraSettings.auroraSpeed);
					this.skyMat.SetFloat("_AuroraSteps", (float)this.auroraSettings.auroraSteps);
					this.skyMat.SetFloat("_AuroraSteps", (float)this.auroraSettings.auroraSteps);
					this.skyMat.SetVector("_Aurora_Tiling_Layer1", this.auroraSettings.auroraLayer1Settings);
					this.skyMat.SetVector("_Aurora_Tiling_Layer2", this.auroraSettings.auroraLayer2Settings);
					this.skyMat.SetVector("_Aurora_Tiling_ColorShift", this.auroraSettings.auroraColorshiftSettings);
				}
				else
				{
					this.skyMat.DisableKeyword("ENVIRO_AURORA");
				}
			}
			this.skyMat.SetVector("_CloudAnimation", this.cloudAnim);
			this.skyMat.SetVector("_CloudCirrusAnimation", this.cirrusAnim);
			if (this.cloudsSettings.cirrusCloudsTexture != null)
			{
				this.skyMat.SetTexture("_CloudMap", this.cloudsSettings.cirrusCloudsTexture);
			}
			this.skyMat.SetColor("_CloudColor", this.cloudsSettings.cirrusCloudsColor.Evaluate(this.GameTime.solarTime));
			this.skyMat.SetFloat("_CloudAltitude", this.cloudsSettings.cirrusCloudsAltitude);
			this.skyMat.SetFloat("_CloudAlpha", this.cloudsConfig.cirrusAlpha);
			this.skyMat.SetFloat("_CloudCoverage", this.cloudsConfig.cirrusCoverage);
			this.skyMat.SetFloat("_CloudColorPower", this.cloudsConfig.cirrusColorPow);
			if (this.flatCloudsRenderTarget != null)
			{
				this.skyMat.SetTexture("_Cloud1Map", this.flatCloudsRenderTarget);
				this.skyMat.SetColor("_Cloud1Color", this.cloudsSettings.flatCloudsColor.Evaluate(this.GameTime.solarTime));
				this.skyMat.SetFloat("_Cloud1Altitude", this.cloudsSettings.flatCloudsAltitude);
				this.skyMat.SetFloat("_Cloud1Alpha", this.cloudsConfig.flatAlpha);
				this.skyMat.SetFloat("_Cloud1ColorPower", this.cloudsConfig.flatColorPow);
			}
		}
		Shader.SetGlobalColor("_EnviroLighting", this.lightSettings.LightColor.Evaluate(this.GameTime.solarTime));
		Shader.SetGlobalVector("_SunDirection", -this.Components.Sun.transform.forward);
		Shader.SetGlobalVector("_SunPosition", this.Components.Sun.transform.localPosition + -this.Components.Sun.transform.forward * 10000f);
		Shader.SetGlobalVector("_MoonPosition", this.Components.Moon.transform.localPosition);
		Shader.SetGlobalVector("_SunDir", -this.Components.Sun.transform.forward);
		Shader.SetGlobalVector("_MoonDir", -this.Components.Moon.transform.forward);
		Shader.SetGlobalColor("_scatteringColor", this.skySettings.scatteringColor.Evaluate(this.GameTime.solarTime));
		Shader.SetGlobalColor("_sunDiskColor", this.skySettings.sunDiskColor.Evaluate(this.GameTime.solarTime));
		Shader.SetGlobalColor("_weatherSkyMod", Color.Lerp(this.currentWeatherSkyMod, this.interiorZoneSettings.currentInteriorSkyboxMod, this.interiorZoneSettings.currentInteriorSkyboxMod.a));
		Shader.SetGlobalColor("_weatherFogMod", Color.Lerp(this.currentWeatherFogMod, this.interiorZoneSettings.currentInteriorFogColorMod, this.interiorZoneSettings.currentInteriorFogColorMod.a));
		Shader.SetGlobalFloat("_gameTime", Mathf.Clamp(1f - this.GameTime.solarTime, 0.5f, 1f));
		Shader.SetGlobalVector("_EnviroSkyFog", new Vector4(this.Fog.skyFogHeight, this.Fog.skyFogIntensity, this.Fog.skyFogStart, this.fogSettings.heightFogIntensity));
		Shader.SetGlobalFloat("_scatteringStrenght", this.Fog.scatteringStrenght);
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
		Shader.SetGlobalFloat("_scatteringPower", this.skySettings.scatteringCurve.Evaluate(this.GameTime.solarTime));
		Shader.SetGlobalFloat("_SkyColorPower", this.skySettings.skyColorPower.Evaluate(this.GameTime.solarTime));
		Shader.SetGlobalFloat("_distanceFogIntensity", this.fogSettings.distanceFogIntensity);
		if (this.cloudsSettings.depthBlending)
		{
			Shader.SetGlobalTexture("_EnviroCloudsTex", this.cloudsRenderTarget);
		}
		if (Application.isPlaying || this.showFogInEditor)
		{
			Shader.SetGlobalFloat("_maximumFogDensity", 1f - this.fogSettings.maximumFogDensity);
		}
		else if (!this.showFogInEditor)
		{
			Shader.SetGlobalFloat("_maximumFogDensity", 1f);
		}
		Shader.SetGlobalFloat("_lightning", this.thunder);
		if (this.fogSettings.useSimpleFog)
		{
			Shader.EnableKeyword("ENVIRO_SIMPLE_FOG");
			return;
		}
		Shader.DisableKeyword("ENVIRO_SIMPLE_FOG");
	}

	// Token: 0x060003BA RID: 954 RVA: 0x000248E4 File Offset: 0x00022AE4
	private void UpdateSkyRenderingComponent()
	{
		if (this.EnviroSkyRender == null)
		{
			return;
		}
		if (this.EnviroSkyRender.fogMat != null)
		{
			this.EnviroSkyRender.fogMat.SetTexture("_Clouds", this.cloudsRenderTarget);
			float value = this.HDR ? 1f : 0f;
			this.EnviroSkyRender.fogMat.SetFloat("_hdr", value);
		}
	}

	// Token: 0x060003BB RID: 955 RVA: 0x0002495C File Offset: 0x00022B5C
	private void ValidateParameters()
	{
		this.internalHour = Mathf.Repeat(this.internalHour, 24f);
		this.GameTime.Longitude = Mathf.Clamp(this.GameTime.Longitude, -180f, 180f);
		this.GameTime.Latitude = Mathf.Clamp(this.GameTime.Latitude, -90f, 90f);
	}

	// Token: 0x060003BC RID: 956 RVA: 0x000249CC File Offset: 0x00022BCC
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
		this.cloudsConfig.coverage = Mathf.Lerp(this.cloudsConfig.coverage, i.cloudsConfig.coverage, num);
		this.cloudsConfig.ambientSkyColorIntensity = Mathf.Lerp(this.cloudsConfig.ambientSkyColorIntensity, i.cloudsConfig.ambientSkyColorIntensity, num);
		if (this.useVolumeClouds)
		{
			this.cloudsConfig.raymarchingScale = Mathf.Lerp(this.cloudsConfig.raymarchingScale, i.cloudsConfig.raymarchingScale, num);
			this.cloudsConfig.ambientSkyColorIntensity = Mathf.Lerp(this.cloudsConfig.ambientSkyColorIntensity, i.cloudsConfig.ambientSkyColorIntensity, num);
			this.cloudsConfig.density = Mathf.Lerp(this.cloudsConfig.density, i.cloudsConfig.density, num);
			this.cloudsConfig.lightStepModifier = Mathf.Lerp(this.cloudsConfig.lightStepModifier, i.cloudsConfig.lightStepModifier, num);
			this.cloudsConfig.lightAbsorbtion = Mathf.Lerp(this.cloudsConfig.lightAbsorbtion, i.cloudsConfig.lightAbsorbtion, num);
			this.cloudsConfig.scatteringCoef = Mathf.Lerp(this.cloudsConfig.scatteringCoef, i.cloudsConfig.scatteringCoef, num);
			this.cloudsConfig.cloudType = Mathf.Lerp(this.cloudsConfig.cloudType, i.cloudsConfig.cloudType, num);
			this.cloudsConfig.coverageType = Mathf.Lerp(this.cloudsConfig.coverageType, i.cloudsConfig.coverageType, num);
			this.cloudsConfig.edgeDarkness = Mathf.Lerp(this.cloudsConfig.edgeDarkness, i.cloudsConfig.edgeDarkness, num);
			this.cloudsConfig.baseErosionIntensity = Mathf.Lerp(this.cloudsConfig.baseErosionIntensity, i.cloudsConfig.baseErosionIntensity, num);
			this.cloudsConfig.detailErosionIntensity = Mathf.Lerp(this.cloudsConfig.detailErosionIntensity, i.cloudsConfig.detailErosionIntensity, num);
		}
		if (this.useFlatClouds)
		{
			this.cloudsConfig.flatAlpha = Mathf.Lerp(this.cloudsConfig.flatAlpha, i.cloudsConfig.flatAlpha, num);
			this.cloudsConfig.flatCoverage = Mathf.Lerp(this.cloudsConfig.flatCoverage, i.cloudsConfig.flatCoverage, num);
			this.cloudsConfig.flatColorPow = Mathf.Lerp(this.cloudsConfig.flatColorPow, i.cloudsConfig.flatColorPow, num);
			this.cloudsConfig.flatSoftness = Mathf.Lerp(this.cloudsConfig.flatSoftness, i.cloudsConfig.flatSoftness, num);
			this.cloudsConfig.flatBrightness = Mathf.Lerp(this.cloudsConfig.flatBrightness, i.cloudsConfig.flatBrightness, num);
		}
		this.cloudsConfig.particleLayer1Alpha = Mathf.Lerp(this.cloudsConfig.particleLayer1Alpha, i.cloudsConfig.particleLayer1Alpha, num * 0.25f);
		this.cloudsConfig.particleLayer1Brightness = Mathf.Lerp(this.cloudsConfig.particleLayer1Brightness, i.cloudsConfig.particleLayer1Brightness, num * 0.25f);
		this.cloudsConfig.particleLayer2Alpha = Mathf.Lerp(this.cloudsConfig.particleLayer2Alpha, i.cloudsConfig.particleLayer2Alpha, num * 0.25f);
		this.cloudsConfig.particleLayer2Brightness = Mathf.Lerp(this.cloudsConfig.particleLayer2Brightness, i.cloudsConfig.particleLayer2Brightness, num * 0.25f);
		this.globalVolumeLightIntensity = Mathf.Lerp(this.globalVolumeLightIntensity, i.volumeLightIntensity, num);
		this.shadowIntensityMod = Mathf.Lerp(this.shadowIntensityMod, i.shadowIntensityMod, num);
		this.currentWeatherSkyMod = Color.Lerp(this.currentWeatherSkyMod, i.weatherSkyMod.Evaluate(this.GameTime.solarTime), num);
		this.currentWeatherFogMod = Color.Lerp(this.currentWeatherFogMod, i.weatherFogMod.Evaluate(this.GameTime.solarTime), num * 10f);
		this.currentWeatherLightMod = Color.Lerp(this.currentWeatherLightMod, i.weatherLightMod.Evaluate(this.GameTime.solarTime), num);
		this.auroraIntensity = Mathf.Lerp(this.auroraIntensity, i.auroraIntensity, num);
	}

	// Token: 0x060003BD RID: 957 RVA: 0x00024EC8 File Offset: 0x000230C8
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
			else if (this.updateFogDensity)
			{
				RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, i.fogDensity, t) * this.interiorZoneSettings.currentInteriorFogMod;
			}
			this.fogSettings.heightDensity = Mathf.Lerp(this.fogSettings.heightDensity, i.heightFogDensity, t);
			this.Fog.skyFogStart = Mathf.Lerp(this.Fog.skyFogStart, i.skyFogStart, t);
			this.Fog.skyFogHeight = Mathf.Lerp(this.Fog.skyFogHeight, i.SkyFogHeight, t);
			this.Fog.skyFogIntensity = Mathf.Lerp(this.Fog.skyFogIntensity, i.SkyFogIntensity, t);
			this.fogSettings.skyFogIntensity = Mathf.Lerp(this.fogSettings.skyFogIntensity, i.SkyFogIntensity, t);
			this.Fog.scatteringStrenght = Mathf.Lerp(this.Fog.scatteringStrenght, i.FogScatteringIntensity, t);
			this.Fog.sunBlocking = Mathf.Lerp(this.Fog.sunBlocking, i.fogSunBlocking, t);
		}
	}

	// Token: 0x060003BE RID: 958 RVA: 0x000250A0 File Offset: 0x000232A0
	private void UpdatePostProcessing(EnviroWeatherPreset i, bool withTransition)
	{
		if (i != null)
		{
			float t = 500f * Time.deltaTime;
			if (withTransition)
			{
				t = 10f * Time.deltaTime;
			}
			this.blurDistance = Mathf.Lerp(this.blurDistance, i.blurDistance, t);
			this.blurIntensity = Mathf.Lerp(this.blurIntensity, i.blurIntensity, t);
			this.blurSkyIntensity = Mathf.Lerp(this.blurSkyIntensity, i.blurSkyIntensity, t);
		}
	}

	// Token: 0x060003BF RID: 959 RVA: 0x0002511C File Offset: 0x0002331C
	private void UpdateEffectSystems(EnviroWeatherPrefab id, bool withTransition)
	{
		if (id != null)
		{
			float num = 500f * Time.deltaTime;
			if (withTransition)
			{
				num = this.weatherSettings.effectTransitionSpeed * Time.deltaTime;
			}
			for (int i = 0; i < id.effectSystems.Count; i++)
			{
				if (id.effectSystems[i].isStopped)
				{
					id.effectSystems[i].Play();
				}
				float emissionRate = Mathf.Lerp(EnviroSkyMgr.instance.GetEmissionRate(id.effectSystems[i]), id.effectEmmisionRates[i] * this.qualitySettings.GlobalParticleEmissionRates, num) * this.interiorZoneSettings.currentInteriorWeatherEffectMod;
				EnviroSkyMgr.instance.SetEmissionRate(id.effectSystems[i], emissionRate);
			}
			for (int j = 0; j < this.Weather.WeatherPrefabs.Count; j++)
			{
				if (this.Weather.WeatherPrefabs[j].gameObject != id.gameObject)
				{
					for (int k = 0; k < this.Weather.WeatherPrefabs[j].effectSystems.Count; k++)
					{
						float num2 = Mathf.Lerp(EnviroSkyMgr.instance.GetEmissionRate(this.Weather.WeatherPrefabs[j].effectSystems[k]), 0f, num * 10f);
						if (num2 < 1f)
						{
							num2 = 0f;
						}
						EnviroSkyMgr.instance.SetEmissionRate(this.Weather.WeatherPrefabs[j].effectSystems[k], num2);
						if (num2 == 0f && !this.Weather.WeatherPrefabs[j].effectSystems[k].isStopped)
						{
							this.Weather.WeatherPrefabs[j].effectSystems[k].Stop();
						}
					}
				}
			}
			base.UpdateWeatherVariables(id.weatherPreset);
		}
	}

	// Token: 0x060003C0 RID: 960 RVA: 0x00025334 File Offset: 0x00023534
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
			this.UpdatePostProcessing(this.Weather.currentActiveWeatherPreset, true);
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

	// Token: 0x060003C1 RID: 961 RVA: 0x00002188 File Offset: 0x00000388
	public void PopulateCloudsQualityList()
	{
	}

	// Token: 0x060003C2 RID: 962 RVA: 0x000254F8 File Offset: 0x000236F8
	public void ApplyVolumeCloudsQualityPreset(EnviroVolumeCloudsQuality preset)
	{
		this.cloudsSettings.cloudsQualitySettings = preset.qualitySettings;
		this.currentActiveCloudsQualityPreset = preset;
	}

	// Token: 0x060003C3 RID: 963 RVA: 0x00025514 File Offset: 0x00023714
	public void ApplyVolumeCloudsQualityPreset(string name)
	{
		for (int i = 0; i < this.cloudsQualityList.Count; i++)
		{
			if (this.cloudsQualityList[i].name == name)
			{
				this.cloudsSettings.cloudsQualitySettings = this.cloudsQualityList[i].qualitySettings;
				this.currentActiveCloudsQualityPreset = this.cloudsQualityList[i];
				this.selectedCloudsQuality = i;
			}
		}
	}

	// Token: 0x060003C4 RID: 964 RVA: 0x00025588 File Offset: 0x00023788
	public void ApplyVolumeCloudsQualityPreset(int id)
	{
		if (id < this.cloudsQualityList.Count && id >= 0)
		{
			this.cloudsSettings.cloudsQualitySettings = this.cloudsQualityList[id].qualitySettings;
			this.currentActiveCloudsQualityPreset = this.cloudsQualityList[id];
			this.selectedCloudsQuality = id;
		}
	}

	// Token: 0x060003C5 RID: 965 RVA: 0x000255DC File Offset: 0x000237DC
	public void InstantWeatherChange(EnviroWeatherPreset preset, EnviroWeatherPrefab prefab)
	{
		this.UpdateClouds(preset, false);
		this.UpdateFog(preset, false);
		this.UpdatePostProcessing(preset, false);
		this.UpdateEffectSystems(prefab, false);
	}

	// Token: 0x060003C6 RID: 966 RVA: 0x000255FE File Offset: 0x000237FE
	public void AssignAndStart(GameObject player, Camera Camera)
	{
		this.Player = player;
		this.PlayerCamera = Camera;
		this.Init();
		this.started = true;
	}

	// Token: 0x060003C7 RID: 967 RVA: 0x0002561B File Offset: 0x0002381B
	public void StartAsServer()
	{
		this.Player = base.gameObject;
		this.serverMode = true;
		this.Init();
	}

	// Token: 0x060003C8 RID: 968 RVA: 0x00025636 File Offset: 0x00023836
	public void ChangeFocus(GameObject player, Camera Camera)
	{
		this.Player = player;
		this.RemoveEnviroCameraComponents(this.PlayerCamera);
		this.PlayerCamera = Camera;
		this.InitImageEffects();
	}

	// Token: 0x060003C9 RID: 969 RVA: 0x00025658 File Offset: 0x00023858
	private void RemoveEnviroCameraComponents(Camera cam)
	{
		EnviroSkyRendering component = cam.GetComponent<EnviroSkyRendering>();
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

	// Token: 0x060003CA RID: 970 RVA: 0x00025694 File Offset: 0x00023894
	public void Play(EnviroTime.TimeProgressMode progressMode = EnviroTime.TimeProgressMode.Simulated)
	{
		base.StartCoroutine(this.SetSceneSettingsLate());
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
		if (this.EnviroPostProcessing != null)
		{
			this.EnviroPostProcessing.enabled = true;
		}
		base.TryPlayAmbientSFX();
		if (this.Weather.currentAudioSource != null)
		{
			this.Weather.currentAudioSource.audiosrc.Play();
		}
		this.started = true;
	}

	// Token: 0x060003CB RID: 971 RVA: 0x0002576C File Offset: 0x0002396C
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

	// Token: 0x060003CC RID: 972 RVA: 0x000257F8 File Offset: 0x000239F8
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

	// Token: 0x060003CD RID: 973 RVA: 0x0002586C File Offset: 0x00023A6C
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

	// Token: 0x040007AC RID: 1964
	private static EnviroSky _instance;

	// Token: 0x040007AD RID: 1965
	public string prefabVersion = "2.2.0";

	// Token: 0x040007AE RID: 1966
	[Header("Virtual Reality")]
	[Tooltip("Enable this when using singlepass rendering.")]
	public bool singlePassVR;

	// Token: 0x040007AF RID: 1967
	[Tooltip("Enable this to activate volume lighing")]
	[HideInInspector]
	public bool useVolumeLighting = true;

	// Token: 0x040007B0 RID: 1968
	[HideInInspector]
	public bool useVolumeClouds = true;

	// Token: 0x040007B1 RID: 1969
	[HideInInspector]
	public bool useFog = true;

	// Token: 0x040007B2 RID: 1970
	[HideInInspector]
	public bool useFlatClouds;

	// Token: 0x040007B3 RID: 1971
	[HideInInspector]
	public bool useParticleClouds;

	// Token: 0x040007B4 RID: 1972
	[HideInInspector]
	public bool useDistanceBlur = true;

	// Token: 0x040007B5 RID: 1973
	[HideInInspector]
	public bool useAurora;

	// Token: 0x040007B6 RID: 1974
	private bool flatCloudsSkybox;

	// Token: 0x040007B7 RID: 1975
	[Header("Scene View Preview")]
	public bool showVolumeLightingInEditor = true;

	// Token: 0x040007B8 RID: 1976
	public bool showVolumeCloudsInEditor = true;

	// Token: 0x040007B9 RID: 1977
	public bool showFlatCloudsInEditor = true;

	// Token: 0x040007BA RID: 1978
	public bool showFogInEditor = true;

	// Token: 0x040007BB RID: 1979
	public bool showDistanceBlurInEditor = true;

	// Token: 0x040007BC RID: 1980
	public bool showSettings;

	// Token: 0x040007BD RID: 1981
	[HideInInspector]
	public Camera satCamera;

	// Token: 0x040007BE RID: 1982
	[HideInInspector]
	public EnviroVolumeLight directVolumeLight;

	// Token: 0x040007BF RID: 1983
	[HideInInspector]
	public EnviroVolumeLight additionalDirectVolumeLight;

	// Token: 0x040007C0 RID: 1984
	[HideInInspector]
	public EnviroSkyRendering EnviroSkyRender;

	// Token: 0x040007C1 RID: 1985
	public float globalVolumeLightIntensity;

	// Token: 0x040007C2 RID: 1986
	public float auroraIntensity;

	// Token: 0x040007C3 RID: 1987
	public EnviroVolumeCloudsQuality currentActiveCloudsQualityPreset;

	// Token: 0x040007C4 RID: 1988
	[HideInInspector]
	public RenderTexture cloudsRenderTarget;

	// Token: 0x040007C5 RID: 1989
	[HideInInspector]
	public RenderTexture flatCloudsRenderTarget;

	// Token: 0x040007C6 RID: 1990
	[HideInInspector]
	public RenderTexture weatherMap;

	// Token: 0x040007C7 RID: 1991
	[HideInInspector]
	public RenderTexture satRenderTarget;

	// Token: 0x040007C8 RID: 1992
	[HideInInspector]
	public RenderTexture cloudShadowMap;

	// Token: 0x040007C9 RID: 1993
	[HideInInspector]
	public Material skyMat;

	// Token: 0x040007CA RID: 1994
	[HideInInspector]
	public Material flatCloudsMat;

	// Token: 0x040007CB RID: 1995
	private Material weatherMapMat;

	// Token: 0x040007CC RID: 1996
	private Material cloudShadowMat;

	// Token: 0x040007CD RID: 1997
	public List<EnviroVolumeCloudsQuality> cloudsQualityList = new List<EnviroVolumeCloudsQuality>();

	// Token: 0x040007CE RID: 1998
	private string[] cloudsQualityPresetsFound;

	// Token: 0x040007CF RID: 1999
	public int selectedCloudsQuality;

	// Token: 0x040007D0 RID: 2000
	private float starsTwinklingRot;

	// Token: 0x040007D1 RID: 2001
	public float blurDistance = 100f;

	// Token: 0x040007D2 RID: 2002
	public float blurIntensity = 1f;

	// Token: 0x040007D3 RID: 2003
	public float blurSkyIntensity = 1f;
}
