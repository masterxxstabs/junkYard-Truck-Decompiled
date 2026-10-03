using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200008B RID: 139
public class EnviroSkyMgr : MonoBehaviour
{
	// Token: 0x1700000D RID: 13
	// (get) Token: 0x0600025B RID: 603 RVA: 0x0001A2B4 File Offset: 0x000184B4
	public static EnviroSkyMgr instance
	{
		get
		{
			if (EnviroSkyMgr._instance == null)
			{
				EnviroSkyMgr._instance = Object.FindObjectOfType<EnviroSkyMgr>();
			}
			return EnviroSkyMgr._instance;
		}
	}

	// Token: 0x0600025C RID: 604 RVA: 0x0001A2D2 File Offset: 0x000184D2
	private void Start()
	{
		if (Application.isPlaying && this.dontDestroy)
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	// Token: 0x0600025D RID: 605 RVA: 0x0001A2F0 File Offset: 0x000184F0
	public void ActivateHDInstance()
	{
		if (this.enviroHDInstance != null)
		{
			if (this.enviroLWInstance != null)
			{
				this.enviroLWInstance.Deactivate(false);
				this.enviroLWInstance.gameObject.SetActive(false);
			}
			this.enviroHDInstance.gameObject.SetActive(true);
			this.enviroHDInstance.Activate();
			this.currentEnviroSkyVersion = EnviroSkyMgr.EnviroSkyVersion.HD;
		}
	}

	// Token: 0x0600025E RID: 606 RVA: 0x0001A35C File Offset: 0x0001855C
	public void DeactivateHDInstance()
	{
		if (this.enviroHDInstance != null)
		{
			this.enviroHDInstance.Deactivate(false);
			this.enviroHDInstance.gameObject.SetActive(false);
			if (this.enviroLWInstance != null && !this.enviroLWInstance.gameObject.activeSelf)
			{
				this.currentEnviroSkyVersion = EnviroSkyMgr.EnviroSkyVersion.None;
				return;
			}
			this.currentEnviroSkyVersion = EnviroSkyMgr.EnviroSkyVersion.LW;
		}
	}

	// Token: 0x0600025F RID: 607 RVA: 0x0001A3C4 File Offset: 0x000185C4
	public void ActivateLWInstance()
	{
		if (this.enviroLWInstance != null)
		{
			if (this.enviroHDInstance != null)
			{
				this.enviroHDInstance.Deactivate(false);
				this.enviroHDInstance.gameObject.SetActive(false);
			}
			this.enviroLWInstance.gameObject.SetActive(true);
			this.enviroLWInstance.Activate();
			this.currentEnviroSkyVersion = EnviroSkyMgr.EnviroSkyVersion.LW;
		}
	}

	// Token: 0x06000260 RID: 608 RVA: 0x0001A430 File Offset: 0x00018630
	public void DeactivateLWInstance()
	{
		if (this.enviroLWInstance != null)
		{
			this.enviroLWInstance.Deactivate(false);
			this.enviroLWInstance.gameObject.SetActive(false);
			if (this.enviroHDInstance != null && !this.enviroHDInstance.gameObject.activeSelf)
			{
				this.currentEnviroSkyVersion = EnviroSkyMgr.EnviroSkyVersion.None;
				return;
			}
			this.currentEnviroSkyVersion = EnviroSkyMgr.EnviroSkyVersion.HD;
		}
	}

	// Token: 0x06000261 RID: 609 RVA: 0x0001A498 File Offset: 0x00018698
	public void DeleteHDInstance()
	{
		if (this.enviroHDInstance != null)
		{
			Object.DestroyImmediate(this.enviroHDInstance.EffectsHolder);
			Object.DestroyImmediate(this.enviroHDInstance.gameObject);
			if (this.enviroHDInstance.EnviroSkyRender != null)
			{
				Object.DestroyImmediate(this.enviroHDInstance.EnviroSkyRender);
			}
			if (this.enviroHDInstance.EnviroPostProcessing != null)
			{
				Object.DestroyImmediate(this.enviroHDInstance.EnviroPostProcessing);
			}
			this.currentEnviroSkyVersion = EnviroSkyMgr.EnviroSkyVersion.None;
		}
	}

	// Token: 0x06000262 RID: 610 RVA: 0x0001A520 File Offset: 0x00018720
	public void DeleteLWInstance()
	{
		if (this.enviroLWInstance != null)
		{
			Object.DestroyImmediate(this.enviroLWInstance.EffectsHolder);
			Object.DestroyImmediate(this.enviroLWInstance.gameObject);
			if (this.enviroLWInstance.EnviroSkyRender != null)
			{
				Object.DestroyImmediate(this.enviroLWInstance.EnviroSkyRender);
			}
			if (this.enviroLWInstance.EnviroPostProcessing != null)
			{
				Object.DestroyImmediate(this.enviroLWInstance.EnviroPostProcessing);
			}
			this.currentEnviroSkyVersion = EnviroSkyMgr.EnviroSkyVersion.None;
		}
	}

	// Token: 0x06000263 RID: 611 RVA: 0x0001A5A8 File Offset: 0x000187A8
	public void SearchForEnviroInstances()
	{
		this.enviroHDInstance = base.GetComponentInChildren<EnviroSky>();
		this.enviroLWInstance = base.GetComponentInChildren<EnviroSkyLite>();
	}

	// Token: 0x06000264 RID: 612 RVA: 0x0001A5C4 File Offset: 0x000187C4
	public void CreateEnviroHDInstance()
	{
		GameObject assetPrefab = this.GetAssetPrefab("Internal_Enviro_HD");
		if (assetPrefab != null && EnviroSky.instance == null)
		{
			this.DeactivateAllInstances();
			GameObject gameObject = Object.Instantiate<GameObject>(assetPrefab, Vector3.zero, Quaternion.identity);
			gameObject.name = "EnviroSky Standard";
			gameObject.transform.SetParent(base.transform);
			this.enviroHDInstance = gameObject.GetComponent<EnviroSky>();
			gameObject.SetActive(false);
			this.currentEnviroSkyVersion = EnviroSkyMgr.EnviroSkyVersion.None;
		}
	}

	// Token: 0x06000265 RID: 613 RVA: 0x0001A640 File Offset: 0x00018840
	public void CreateEnviroHDVRInstance()
	{
		GameObject assetPrefab = this.GetAssetPrefab("Internal_Enviro_HD_VR");
		if (assetPrefab != null && EnviroSky.instance == null)
		{
			this.DeactivateAllInstances();
			GameObject gameObject = Object.Instantiate<GameObject>(assetPrefab, Vector3.zero, Quaternion.identity);
			gameObject.name = "EnviroSky Standard for VR";
			gameObject.transform.SetParent(base.transform);
			this.enviroHDInstance = gameObject.GetComponent<EnviroSky>();
			gameObject.SetActive(false);
			this.currentEnviroSkyVersion = EnviroSkyMgr.EnviroSkyVersion.None;
		}
	}

	// Token: 0x06000266 RID: 614 RVA: 0x0001A6BC File Offset: 0x000188BC
	public void CreateEnviroLWInstance()
	{
		GameObject assetPrefab = this.GetAssetPrefab("Internal_Enviro_LW");
		if (assetPrefab != null && EnviroSkyLite.instance == null)
		{
			this.DeactivateAllInstances();
			GameObject gameObject = Object.Instantiate<GameObject>(assetPrefab, Vector3.zero, Quaternion.identity);
			gameObject.name = "EnviroSky Lite";
			gameObject.transform.SetParent(base.transform);
			this.enviroLWInstance = gameObject.GetComponent<EnviroSkyLite>();
			gameObject.SetActive(false);
			this.currentEnviroSkyVersion = EnviroSkyMgr.EnviroSkyVersion.None;
		}
	}

	// Token: 0x06000267 RID: 615 RVA: 0x0001A738 File Offset: 0x00018938
	public void CreateEnviroLWMobileInstance()
	{
		GameObject assetPrefab = this.GetAssetPrefab("Internal_Enviro_LW_MOBILE");
		if (assetPrefab != null && EnviroSkyLite.instance == null)
		{
			this.DeactivateAllInstances();
			GameObject gameObject = Object.Instantiate<GameObject>(assetPrefab, Vector3.zero, Quaternion.identity);
			gameObject.name = "EnviroSky Lite for Mobiles";
			gameObject.transform.SetParent(base.transform);
			this.enviroLWInstance = gameObject.GetComponent<EnviroSkyLite>();
			gameObject.SetActive(false);
			this.currentEnviroSkyVersion = EnviroSkyMgr.EnviroSkyVersion.None;
		}
	}

	// Token: 0x06000268 RID: 616 RVA: 0x0001A7B4 File Offset: 0x000189B4
	private void DeactivateAllInstances()
	{
		if (this.enviroHDInstance != null)
		{
			this.DeactivateHDInstance();
		}
		if (this.enviroLWInstance != null)
		{
			this.DeactivateLWInstance();
		}
	}

	// Token: 0x06000269 RID: 617 RVA: 0x00018FC3 File Offset: 0x000171C3
	public GameObject GetAssetPrefab(string name)
	{
		return null;
	}

	// Token: 0x0600026A RID: 618 RVA: 0x0001A7DE File Offset: 0x000189DE
	public void ActivateLWRP()
	{
		this.currentRenderPipeline = EnviroSkyMgr.EnviroRenderPipeline.LWRP;
	}

	// Token: 0x0600026B RID: 619 RVA: 0x0001A7E7 File Offset: 0x000189E7
	public void ActivateLegacyRP()
	{
		this.currentRenderPipeline = EnviroSkyMgr.EnviroRenderPipeline.Legacy;
	}

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x0600026C RID: 620 RVA: 0x0001A7F0 File Offset: 0x000189F0
	// (set) Token: 0x0600026D RID: 621 RVA: 0x0001A810 File Offset: 0x00018A10
	public EnviroComponents Components
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.Components;
			}
			return EnviroSkyLite.instance.Components;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.Components = value;
				return;
			}
			EnviroSkyLite.instance.Components = value;
		}
	}

	// Token: 0x1700000F RID: 15
	// (get) Token: 0x0600026E RID: 622 RVA: 0x0001A832 File Offset: 0x00018A32
	// (set) Token: 0x0600026F RID: 623 RVA: 0x0001A852 File Offset: 0x00018A52
	public EnviroTime Time
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.GameTime;
			}
			return EnviroSkyLite.instance.GameTime;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.GameTime = value;
				return;
			}
			EnviroSkyLite.instance.GameTime = value;
		}
	}

	// Token: 0x17000010 RID: 16
	// (get) Token: 0x06000270 RID: 624 RVA: 0x0001A874 File Offset: 0x00018A74
	// (set) Token: 0x06000271 RID: 625 RVA: 0x0001A894 File Offset: 0x00018A94
	public EnviroSeasons Seasons
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.Seasons;
			}
			return EnviroSkyLite.instance.Seasons;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.Seasons = value;
				return;
			}
			EnviroSkyLite.instance.Seasons = value;
		}
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x06000272 RID: 626 RVA: 0x0001A8B6 File Offset: 0x00018AB6
	// (set) Token: 0x06000273 RID: 627 RVA: 0x0001A8D6 File Offset: 0x00018AD6
	public EnviroSeasonSettings SeasonSettings
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.seasonsSettings;
			}
			return EnviroSkyLite.instance.seasonsSettings;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.seasonsSettings = value;
				return;
			}
			EnviroSkyLite.instance.seasonsSettings = value;
		}
	}

	// Token: 0x17000012 RID: 18
	// (get) Token: 0x06000274 RID: 628 RVA: 0x0001A8F8 File Offset: 0x00018AF8
	// (set) Token: 0x06000275 RID: 629 RVA: 0x0001A90F File Offset: 0x00018B0F
	public EnviroAuroraSettings AuroraSettings
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.auroraSettings;
			}
			return null;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.auroraSettings = value;
				return;
			}
		}
	}

	// Token: 0x17000013 RID: 19
	// (get) Token: 0x06000276 RID: 630 RVA: 0x0001A926 File Offset: 0x00018B26
	// (set) Token: 0x06000277 RID: 631 RVA: 0x0001A946 File Offset: 0x00018B46
	public EnviroReflectionSettings ReflectionSettings
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.reflectionSettings;
			}
			return EnviroSkyLite.instance.reflectionSettings;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.reflectionSettings = value;
				return;
			}
			EnviroSkyLite.instance.reflectionSettings = value;
		}
	}

	// Token: 0x17000014 RID: 20
	// (get) Token: 0x06000278 RID: 632 RVA: 0x0001A968 File Offset: 0x00018B68
	// (set) Token: 0x06000279 RID: 633 RVA: 0x0001A988 File Offset: 0x00018B88
	public EnviroCloudSettings CloudSettings
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.cloudsSettings;
			}
			return EnviroSkyLite.instance.cloudsSettings;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.cloudsSettings = value;
				return;
			}
			EnviroSkyLite.instance.cloudsSettings = value;
		}
	}

	// Token: 0x17000015 RID: 21
	// (get) Token: 0x0600027A RID: 634 RVA: 0x0001A9AA File Offset: 0x00018BAA
	// (set) Token: 0x0600027B RID: 635 RVA: 0x0001A9CA File Offset: 0x00018BCA
	public EnviroInteriorZoneSettings InteriorZoneSettings
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.interiorZoneSettings;
			}
			return EnviroSkyLite.instance.interiorZoneSettings;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.interiorZoneSettings = value;
				return;
			}
			EnviroSkyLite.instance.interiorZoneSettings = value;
		}
	}

	// Token: 0x17000016 RID: 22
	// (get) Token: 0x0600027C RID: 636 RVA: 0x0001A9EC File Offset: 0x00018BEC
	// (set) Token: 0x0600027D RID: 637 RVA: 0x0001AA0C File Offset: 0x00018C0C
	public EnviroAudio AudioSettings
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.Audio;
			}
			return EnviroSkyLite.instance.Audio;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.Audio = value;
				return;
			}
			EnviroSkyLite.instance.Audio = value;
		}
	}

	// Token: 0x17000017 RID: 23
	// (get) Token: 0x0600027E RID: 638 RVA: 0x0001AA2E File Offset: 0x00018C2E
	// (set) Token: 0x0600027F RID: 639 RVA: 0x0001AA4E File Offset: 0x00018C4E
	public EnviroWeatherCloudsConfig Clouds
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.cloudsConfig;
			}
			return EnviroSkyLite.instance.cloudsConfig;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.cloudsConfig = value;
				return;
			}
			EnviroSkyLite.instance.cloudsConfig = value;
		}
	}

	// Token: 0x17000018 RID: 24
	// (get) Token: 0x06000280 RID: 640 RVA: 0x0001AA70 File Offset: 0x00018C70
	// (set) Token: 0x06000281 RID: 641 RVA: 0x0001AA90 File Offset: 0x00018C90
	public EnviroWeatherSettings WeatherSettings
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.weatherSettings;
			}
			return EnviroSkyLite.instance.weatherSettings;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.weatherSettings = value;
				return;
			}
			EnviroSkyLite.instance.weatherSettings = value;
		}
	}

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x06000282 RID: 642 RVA: 0x0001AAB2 File Offset: 0x00018CB2
	// (set) Token: 0x06000283 RID: 643 RVA: 0x0001AAD2 File Offset: 0x00018CD2
	public EnviroLightSettings LightSettings
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.lightSettings;
			}
			return EnviroSkyLite.instance.lightSettings;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.lightSettings = value;
				return;
			}
			EnviroSkyLite.instance.lightSettings = value;
		}
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x06000284 RID: 644 RVA: 0x0001AAF4 File Offset: 0x00018CF4
	// (set) Token: 0x06000285 RID: 645 RVA: 0x0001AB14 File Offset: 0x00018D14
	public EnviroVolumeLightingSettings VolumeLightSettings
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.volumeLightSettings;
			}
			return EnviroSkyLite.instance.volumeLightSettings;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.volumeLightSettings = value;
				return;
			}
			EnviroSkyLite.instance.volumeLightSettings = value;
		}
	}

	// Token: 0x1700001B RID: 27
	// (get) Token: 0x06000286 RID: 646 RVA: 0x0001AB36 File Offset: 0x00018D36
	// (set) Token: 0x06000287 RID: 647 RVA: 0x0001AB56 File Offset: 0x00018D56
	public EnviroLightShaftsSettings LightShaftsSettings
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.lightshaftsSettings;
			}
			return EnviroSkyLite.instance.lightshaftsSettings;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.lightshaftsSettings = value;
				return;
			}
			EnviroSkyLite.instance.lightshaftsSettings = value;
		}
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x06000288 RID: 648 RVA: 0x0001AB78 File Offset: 0x00018D78
	// (set) Token: 0x06000289 RID: 649 RVA: 0x0001AB98 File Offset: 0x00018D98
	public EnviroSkySettings SkySettings
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.skySettings;
			}
			return EnviroSkyLite.instance.skySettings;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.skySettings = value;
				return;
			}
			EnviroSkyLite.instance.skySettings = value;
		}
	}

	// Token: 0x1700001D RID: 29
	// (get) Token: 0x0600028A RID: 650 RVA: 0x0001ABBA File Offset: 0x00018DBA
	// (set) Token: 0x0600028B RID: 651 RVA: 0x0001ABDA File Offset: 0x00018DDA
	public EnviroFogSettings FogSettings
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.fogSettings;
			}
			return EnviroSkyLite.instance.fogSettings;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.fogSettings = value;
				return;
			}
			EnviroSkyLite.instance.fogSettings = value;
		}
	}

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x0600028C RID: 652 RVA: 0x0001ABFC File Offset: 0x00018DFC
	// (set) Token: 0x0600028D RID: 653 RVA: 0x0001AC1C File Offset: 0x00018E1C
	public GameObject Player
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.Player;
			}
			return EnviroSkyLite.instance.Player;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.Player = value;
				return;
			}
			EnviroSkyLite.instance.Player = value;
		}
	}

	// Token: 0x1700001F RID: 31
	// (get) Token: 0x0600028E RID: 654 RVA: 0x0001AC3E File Offset: 0x00018E3E
	// (set) Token: 0x0600028F RID: 655 RVA: 0x0001AC5E File Offset: 0x00018E5E
	public Camera Camera
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.PlayerCamera;
			}
			return EnviroSkyLite.instance.PlayerCamera;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.PlayerCamera = value;
				return;
			}
			EnviroSkyLite.instance.PlayerCamera = value;
		}
	}

	// Token: 0x06000290 RID: 656 RVA: 0x0001AC80 File Offset: 0x00018E80
	public void AssignAndStart(GameObject Player, Camera cam)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.AssignAndStart(Player, cam);
			return;
		}
		EnviroSkyLite.instance.AssignAndStart(Player, cam);
	}

	// Token: 0x06000291 RID: 657 RVA: 0x0001ACA4 File Offset: 0x00018EA4
	public void ChangeFocus(GameObject Player, Camera cam)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.ChangeFocus(Player, cam);
			return;
		}
		EnviroSkyLite.instance.ChangeFocus(Player, cam);
	}

	// Token: 0x06000292 RID: 658 RVA: 0x0001ACC8 File Offset: 0x00018EC8
	public void StartAsServer()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.StartAsServer();
			return;
		}
		EnviroSkyLite.instance.StartAsServer();
	}

	// Token: 0x06000293 RID: 659 RVA: 0x0001ACE8 File Offset: 0x00018EE8
	public void ReInit()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.ReInit();
			return;
		}
		EnviroSkyLite.instance.ReInit();
	}

	// Token: 0x06000294 RID: 660 RVA: 0x0001AD08 File Offset: 0x00018F08
	public void SetupSkybox()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.SetupSkybox();
			return;
		}
		EnviroSkyLite.instance.SetupSkybox();
	}

	// Token: 0x06000295 RID: 661 RVA: 0x0001AD28 File Offset: 0x00018F28
	public bool IsNight()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.isNight;
		}
		return EnviroSkyLite.instance.isNight;
	}

	// Token: 0x06000296 RID: 662 RVA: 0x0001AD48 File Offset: 0x00018F48
	public bool IsStarted()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.started;
		}
		return EnviroSkyLite.instance.started;
	}

	// Token: 0x06000297 RID: 663 RVA: 0x0001AD68 File Offset: 0x00018F68
	public bool IsInterior()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.interiorMode;
		}
		return EnviroSkyLite.instance.interiorMode;
	}

	// Token: 0x06000298 RID: 664 RVA: 0x0001AD88 File Offset: 0x00018F88
	public bool IsEnviroSkyAttached(GameObject obj)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return obj.GetComponent<EnviroSky>();
		}
		return obj.GetComponent<EnviroSkyLite>();
	}

	// Token: 0x06000299 RID: 665 RVA: 0x0001ADAA File Offset: 0x00018FAA
	public bool IsDefaultZone(GameObject zone)
	{
		return zone.GetComponent<EnviroSky>() || zone.GetComponent<EnviroSkyLite>();
	}

	// Token: 0x0600029A RID: 666 RVA: 0x0001ADC9 File Offset: 0x00018FC9
	public bool IsAutoWeatherUpdateActive()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.Weather.updateWeather;
		}
		return EnviroSkyLite.instance.Weather.updateWeather;
	}

	// Token: 0x0600029B RID: 667 RVA: 0x0001ADF3 File Offset: 0x00018FF3
	public bool IsAvailable()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return !(EnviroSky.instance == null);
		}
		return !(EnviroSkyLite.instance == null);
	}

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x0600029C RID: 668 RVA: 0x0001AE1F File Offset: 0x0001901F
	// (set) Token: 0x0600029D RID: 669 RVA: 0x0001AE3F File Offset: 0x0001903F
	public EnviroWeather Weather
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.Weather;
			}
			return EnviroSkyLite.instance.Weather;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.Weather = value;
				return;
			}
			EnviroSkyLite.instance.Weather = value;
		}
	}

	// Token: 0x0600029E RID: 670 RVA: 0x0001AE61 File Offset: 0x00019061
	public bool GetUseWeatherTag()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.weatherSettings.useTag;
		}
		return EnviroSkyLite.instance.weatherSettings.useTag;
	}

	// Token: 0x0600029F RID: 671 RVA: 0x0001AE8B File Offset: 0x0001908B
	public string GetEnviroSkyTag()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.tag;
		}
		return EnviroSkyLite.instance.tag;
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x0001AEAB File Offset: 0x000190AB
	public float GetSnowIntensity()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.Weather.curSnowStrength;
		}
		return EnviroSkyLite.instance.Weather.curSnowStrength;
	}

	// Token: 0x060002A1 RID: 673 RVA: 0x0001AED5 File Offset: 0x000190D5
	public float GetWetnessIntensity()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.Weather.curWetness;
		}
		return EnviroSkyLite.instance.Weather.curWetness;
	}

	// Token: 0x060002A2 RID: 674 RVA: 0x0001AF00 File Offset: 0x00019100
	public string GetCurrentTemperatureString()
	{
		int num;
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			num = (int)EnviroSky.instance.Weather.currentTemperature;
		}
		else
		{
			num = (int)EnviroSkyLite.instance.Weather.currentTemperature;
		}
		return num.ToString() + "°C";
	}

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x060002A3 RID: 675 RVA: 0x0001AF4D File Offset: 0x0001914D
	// (set) Token: 0x060002A4 RID: 676 RVA: 0x0001AF6D File Offset: 0x0001916D
	public float CustomFogIntensity
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.customFogIntensity;
			}
			return EnviroSkyLite.instance.customFogIntensity;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.customFogIntensity = value;
				return;
			}
			EnviroSkyLite.instance.customFogIntensity = value;
		}
	}

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x060002A5 RID: 677 RVA: 0x0001AF8F File Offset: 0x0001918F
	// (set) Token: 0x060002A6 RID: 678 RVA: 0x0001AFAF File Offset: 0x000191AF
	public Color CustomFogColor
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.customFogColor;
			}
			return EnviroSkyLite.instance.customFogColor;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.customFogColor = value;
				return;
			}
			EnviroSkyLite.instance.customFogColor = value;
		}
	}

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x060002A7 RID: 679 RVA: 0x0001AFD1 File Offset: 0x000191D1
	// (set) Token: 0x060002A8 RID: 680 RVA: 0x0001AFF1 File Offset: 0x000191F1
	public bool UpdateFogIntensity
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.updateFogDensity;
			}
			return EnviroSkyLite.instance.updateFogDensity;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.updateFogDensity = value;
				return;
			}
			EnviroSkyLite.instance.updateFogDensity = value;
		}
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x0001B013 File Offset: 0x00019213
	public EnviroZone GetZoneByID(int id)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.Weather.zones[id];
		}
		return EnviroSkyLite.instance.Weather.zones[id];
	}

	// Token: 0x060002AA RID: 682 RVA: 0x0001B049 File Offset: 0x00019249
	public void RegisterZone(EnviroZone z)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.RegisterZone(z);
			return;
		}
		EnviroSkyLite.instance.RegisterZone(z);
	}

	// Token: 0x060002AB RID: 683 RVA: 0x0001B06C File Offset: 0x0001926C
	public float GetUniversalTimeOfDay()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.internalHour - (float)EnviroSky.instance.GameTime.utcOffset;
		}
		return EnviroSkyLite.instance.internalHour - (float)EnviroSkyLite.instance.GameTime.utcOffset;
	}

	// Token: 0x060002AC RID: 684 RVA: 0x0001B0B9 File Offset: 0x000192B9
	public float GetTimeOfDay()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.internalHour;
		}
		return EnviroSkyLite.instance.internalHour;
	}

	// Token: 0x060002AD RID: 685 RVA: 0x0001B0D9 File Offset: 0x000192D9
	public double GetCurrentTimeInHours()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.currentTimeInHours;
		}
		return EnviroSkyLite.instance.currentTimeInHours;
	}

	// Token: 0x060002AE RID: 686 RVA: 0x0001B0F9 File Offset: 0x000192F9
	public EnviroSeasons.Seasons GetCurrentSeason()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.Seasons.currentSeasons;
		}
		return EnviroSkyLite.instance.Seasons.currentSeasons;
	}

	// Token: 0x060002AF RID: 687 RVA: 0x0001B123 File Offset: 0x00019323
	public void SetYears(int year)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.GameTime.Years = year;
			return;
		}
		EnviroSkyLite.instance.GameTime.Years = year;
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x0001B14F File Offset: 0x0001934F
	public void SetDays(int days)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.GameTime.Days = days;
			return;
		}
		EnviroSkyLite.instance.GameTime.Days = days;
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x0001B17B File Offset: 0x0001937B
	public void SetTime(DateTime date)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.SetTime(date);
			return;
		}
		EnviroSkyLite.instance.SetTime(date);
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x0001B19D File Offset: 0x0001939D
	public void SetTime(int year, int dayOfYear, int hour, int minute, int seconds)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.SetTime(year, dayOfYear, hour, minute, seconds);
			return;
		}
		EnviroSkyLite.instance.SetTime(year, dayOfYear, hour, minute, seconds);
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x0001B1CB File Offset: 0x000193CB
	public void ResetHourEventTimer()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.ResetHourEventTimer();
			return;
		}
		EnviroSkyLite.instance.ResetHourEventTimer();
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x0001B1EB File Offset: 0x000193EB
	public void SetTimeOfDay(float timeOfDay)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.SetInternalTimeOfDay(timeOfDay);
			return;
		}
		EnviroSkyLite.instance.SetInternalTimeOfDay(timeOfDay);
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x0001B20D File Offset: 0x0001940D
	public void ChangeSeason(EnviroSeasons.Seasons s)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.ChangeSeason(s);
			return;
		}
		EnviroSkyLite.instance.ChangeSeason(s);
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x0001B22F File Offset: 0x0001942F
	public void SetTimeProgress(EnviroTime.TimeProgressMode tpm)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.GameTime.ProgressTime = tpm;
			return;
		}
		EnviroSkyLite.instance.GameTime.ProgressTime = tpm;
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x0001B25C File Offset: 0x0001945C
	public string GetTimeStringWithSeconds()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return string.Format("{0:00}:{1:00}:{2:00}", EnviroSky.instance.GameTime.Hours, EnviroSky.instance.GameTime.Minutes, EnviroSky.instance.GameTime.Seconds);
		}
		return string.Format("{0:00}:{1:00}:{2:00}", EnviroSkyLite.instance.GameTime.Hours, EnviroSkyLite.instance.GameTime.Minutes, EnviroSkyLite.instance.GameTime.Seconds);
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x0001B300 File Offset: 0x00019500
	public string GetTimeString()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return string.Format("{0:00}:{1:00}", EnviroSky.instance.GameTime.Hours, EnviroSky.instance.GameTime.Minutes);
		}
		return string.Format("{0:00}:{1:00}", EnviroSkyLite.instance.GameTime.Hours, EnviroSkyLite.instance.GameTime.Minutes);
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x0001B37B File Offset: 0x0001957B
	public int GetCurrentYear()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.GameTime.Years;
		}
		return EnviroSkyLite.instance.GameTime.Years;
	}

	// Token: 0x060002BA RID: 698 RVA: 0x0001B3A8 File Offset: 0x000195A8
	public int GetCurrentMonth()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			DateTime dateTime = default(DateTime);
			DateTime dateTime2 = dateTime.AddYears(EnviroSky.instance.GameTime.Years);
			return dateTime.AddDays((double)EnviroSky.instance.GameTime.Days).Month;
		}
		DateTime dateTime3 = default(DateTime);
		DateTime dateTime4 = dateTime3.AddYears(EnviroSkyLite.instance.GameTime.Years);
		return dateTime3.AddDays((double)EnviroSkyLite.instance.GameTime.Days).Month;
	}

	// Token: 0x060002BB RID: 699 RVA: 0x0001B43C File Offset: 0x0001963C
	public DateTime GetDateAsDateTime()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return default(DateTime).AddYears(EnviroSky.instance.GameTime.Years - 1).AddDays((double)(EnviroSky.instance.GameTime.Days - 1)).AddHours((double)EnviroSky.instance.GameTime.Hours).AddMinutes((double)EnviroSky.instance.GameTime.Minutes).AddSeconds((double)EnviroSky.instance.GameTime.Seconds);
		}
		return default(DateTime).AddYears(EnviroSkyLite.instance.GameTime.Years - 1).AddDays((double)(EnviroSkyLite.instance.GameTime.Days - 1)).AddHours((double)EnviroSkyLite.instance.GameTime.Hours).AddMinutes((double)EnviroSkyLite.instance.GameTime.Minutes).AddSeconds((double)EnviroSkyLite.instance.GameTime.Seconds);
	}

	// Token: 0x060002BC RID: 700 RVA: 0x0001B55E File Offset: 0x0001975E
	public int GetCurrentDay()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.GameTime.Days;
		}
		return EnviroSkyLite.instance.GameTime.Days;
	}

	// Token: 0x060002BD RID: 701 RVA: 0x0001B588 File Offset: 0x00019788
	public int GetCurrentHour()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.GameTime.Hours;
		}
		return EnviroSkyLite.instance.GameTime.Hours;
	}

	// Token: 0x060002BE RID: 702 RVA: 0x0001B5B2 File Offset: 0x000197B2
	public int GetCurrentMinute()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.GameTime.Minutes;
		}
		return EnviroSkyLite.instance.GameTime.Minutes;
	}

	// Token: 0x060002BF RID: 703 RVA: 0x0001B5DC File Offset: 0x000197DC
	public int GetCurrentSecond()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.GameTime.Seconds;
		}
		return EnviroSkyLite.instance.GameTime.Seconds;
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x0001B606 File Offset: 0x00019806
	public void ChangeWeatherInstant(int weatherId)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.SetWeatherOverwrite(weatherId);
			return;
		}
		EnviroSkyLite.instance.SetWeatherOverwrite(weatherId);
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x0001B628 File Offset: 0x00019828
	public void ChangeWeatherInstant(EnviroWeatherPreset preset)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.SetWeatherOverwrite(preset);
			return;
		}
		EnviroSkyLite.instance.SetWeatherOverwrite(preset);
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x0001B64A File Offset: 0x0001984A
	public void ChangeWeather(int weatherId)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.ChangeWeather(weatherId);
			return;
		}
		EnviroSkyLite.instance.ChangeWeather(weatherId);
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x0001B66C File Offset: 0x0001986C
	public void ChangeWeather(EnviroWeatherPreset preset)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.ChangeWeather(preset);
			return;
		}
		EnviroSkyLite.instance.ChangeWeather(preset);
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x0001B68E File Offset: 0x0001988E
	public void ChangeWeather(string Name)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.ChangeWeather(Name);
			return;
		}
		EnviroSkyLite.instance.ChangeWeather(Name);
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x0001B6B0 File Offset: 0x000198B0
	public EnviroZone GetCurrentActiveZone()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.Weather.currentActiveZone;
		}
		return EnviroSkyLite.instance.Weather.currentActiveZone;
	}

	// Token: 0x060002C6 RID: 710 RVA: 0x0001B6DA File Offset: 0x000198DA
	public void SetCurrentActiveZone(EnviroZone z)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.Weather.currentActiveZone = z;
			return;
		}
		EnviroSkyLite.instance.Weather.currentActiveZone = z;
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x0001B706 File Offset: 0x00019906
	public void InstantWeatherChange(EnviroWeatherPreset preset, EnviroWeatherPrefab prefab)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.InstantWeatherChange(preset, prefab);
			return;
		}
		EnviroSkyLite.instance.InstantWeatherChange(preset, prefab);
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x0001B72C File Offset: 0x0001992C
	public void SetToZone(int z)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.Weather.currentActiveZone = EnviroSky.instance.Weather.zones[z];
			return;
		}
		EnviroSkyLite.instance.Weather.currentActiveZone = EnviroSkyLite.instance.Weather.zones[z];
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x0001B78C File Offset: 0x0001998C
	public int GetWeatherID()
	{
		for (int i = 0; i < EnviroSky.instance.Weather.WeatherPrefabs.Count; i++)
		{
			if (EnviroSky.instance.Weather.WeatherPrefabs[i].weatherPreset == EnviroSky.instance.Weather.currentActiveWeatherPreset)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060002CA RID: 714 RVA: 0x0001B7EB File Offset: 0x000199EB
	public EnviroWeatherPreset GetCurrentWeatherPreset()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.Weather.currentActiveWeatherPreset;
		}
		return EnviroSkyLite.instance.Weather.currentActiveWeatherPreset;
	}

	// Token: 0x060002CB RID: 715 RVA: 0x0001B815 File Offset: 0x00019A15
	public EnviroWeatherPreset GetStartWeatherPreset()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.Weather.startWeatherPreset;
		}
		return EnviroSkyLite.instance.Weather.startWeatherPreset;
	}

	// Token: 0x060002CC RID: 716 RVA: 0x0001B83F File Offset: 0x00019A3F
	public List<EnviroWeatherPreset> GetCurrentWeatherPresetList()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.Weather.weatherPresets;
		}
		return EnviroSkyLite.instance.Weather.weatherPresets;
	}

	// Token: 0x060002CD RID: 717 RVA: 0x0001B869 File Offset: 0x00019A69
	public List<EnviroWeatherPrefab> GetCurrentWeatherPrefabList()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.Weather.WeatherPrefabs;
		}
		return EnviroSkyLite.instance.Weather.WeatherPrefabs;
	}

	// Token: 0x060002CE RID: 718 RVA: 0x0001B893 File Offset: 0x00019A93
	public List<EnviroZone> GetZoneList()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.Weather.zones;
		}
		return EnviroSkyLite.instance.Weather.zones;
	}

	// Token: 0x060002CF RID: 719 RVA: 0x0001B8C0 File Offset: 0x00019AC0
	public void ChangeZoneWeather(int zoneId, int weatherId)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.Weather.zones[zoneId].currentActiveZoneWeatherPrefab = EnviroSky.instance.Weather.WeatherPrefabs[weatherId];
			EnviroSky.instance.Weather.zones[zoneId].currentActiveZoneWeatherPreset = EnviroSky.instance.Weather.WeatherPrefabs[weatherId].weatherPreset;
			return;
		}
		EnviroSkyLite.instance.Weather.zones[zoneId].currentActiveZoneWeatherPrefab = EnviroSkyLite.instance.Weather.WeatherPrefabs[weatherId];
		EnviroSkyLite.instance.Weather.zones[zoneId].currentActiveZoneWeatherPreset = EnviroSkyLite.instance.Weather.WeatherPrefabs[weatherId].weatherPreset;
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x0001B99D File Offset: 0x00019B9D
	public void SetAutoWeatherUpdates(bool b)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.Weather.updateWeather = b;
			return;
		}
		EnviroSkyLite.instance.Weather.updateWeather = b;
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x060002D1 RID: 721 RVA: 0x0001B9C9 File Offset: 0x00019BC9
	// (set) Token: 0x060002D2 RID: 722 RVA: 0x0001B9F3 File Offset: 0x00019BF3
	public float ambientAudioVolume
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.Audio.ambientSFXVolume;
			}
			return EnviroSkyLite.instance.Audio.ambientSFXVolume;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.Audio.ambientSFXVolume = value;
				return;
			}
			EnviroSkyLite.instance.Audio.ambientSFXVolume = value;
		}
	}

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x060002D3 RID: 723 RVA: 0x0001BA1F File Offset: 0x00019C1F
	// (set) Token: 0x060002D4 RID: 724 RVA: 0x0001BA49 File Offset: 0x00019C49
	public float weatherAudioVolume
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.Audio.weatherSFXVolume;
			}
			return EnviroSkyLite.instance.Audio.weatherSFXVolume;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.Audio.weatherSFXVolume = value;
				return;
			}
			EnviroSkyLite.instance.Audio.weatherSFXVolume = value;
		}
	}

	// Token: 0x17000026 RID: 38
	// (get) Token: 0x060002D5 RID: 725 RVA: 0x0001BA75 File Offset: 0x00019C75
	// (set) Token: 0x060002D6 RID: 726 RVA: 0x0001BA9F File Offset: 0x00019C9F
	public float ambientAudioVolumeModifier
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.Audio.ambientSFXVolumeMod;
			}
			return EnviroSkyLite.instance.Audio.ambientSFXVolumeMod;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.Audio.ambientSFXVolumeMod = value;
				return;
			}
			EnviroSkyLite.instance.Audio.ambientSFXVolumeMod = value;
		}
	}

	// Token: 0x17000027 RID: 39
	// (get) Token: 0x060002D7 RID: 727 RVA: 0x0001BACB File Offset: 0x00019CCB
	// (set) Token: 0x060002D8 RID: 728 RVA: 0x0001BAF5 File Offset: 0x00019CF5
	public float weatherAudioVolumeModifier
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.Audio.weatherSFXVolumeMod;
			}
			return EnviroSkyLite.instance.Audio.weatherSFXVolumeMod;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.Audio.weatherSFXVolumeMod = value;
				return;
			}
			EnviroSkyLite.instance.Audio.weatherSFXVolumeMod = value;
		}
	}

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x060002D9 RID: 729 RVA: 0x0001BB21 File Offset: 0x00019D21
	// (set) Token: 0x060002DA RID: 730 RVA: 0x0001BB4B File Offset: 0x00019D4B
	public float audioTransitionSpeed
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.weatherSettings.audioTransitionSpeed;
			}
			return EnviroSkyLite.instance.weatherSettings.audioTransitionSpeed;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.weatherSettings.audioTransitionSpeed = value;
				return;
			}
			EnviroSkyLite.instance.weatherSettings.audioTransitionSpeed = value;
		}
	}

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x060002DB RID: 731 RVA: 0x0001BB77 File Offset: 0x00019D77
	// (set) Token: 0x060002DC RID: 732 RVA: 0x0001BBA1 File Offset: 0x00019DA1
	public float interiorZoneAudioVolume
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.interiorZoneSettings.currentInteriorZoneAudioVolume;
			}
			return EnviroSkyLite.instance.interiorZoneSettings.currentInteriorZoneAudioVolume;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.interiorZoneSettings.currentInteriorZoneAudioVolume = value;
				return;
			}
			EnviroSkyLite.instance.interiorZoneSettings.currentInteriorZoneAudioVolume = value;
		}
	}

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x060002DD RID: 733 RVA: 0x0001BBCD File Offset: 0x00019DCD
	// (set) Token: 0x060002DE RID: 734 RVA: 0x0001BBF7 File Offset: 0x00019DF7
	public float interiorZoneAudioFadingSpeed
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.interiorZoneSettings.currentInteriorZoneAudioFadingSpeed;
			}
			return EnviroSkyLite.instance.interiorZoneSettings.currentInteriorZoneAudioFadingSpeed;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.interiorZoneSettings.currentInteriorZoneAudioFadingSpeed = value;
				return;
			}
			EnviroSkyLite.instance.interiorZoneSettings.currentInteriorZoneAudioFadingSpeed = value;
		}
	}

	// Token: 0x060002DF RID: 735 RVA: 0x0001BC23 File Offset: 0x00019E23
	public GameObject GetVFXHolder()
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.Weather.VFXHolder;
		}
		return EnviroSkyLite.instance.Weather.VFXHolder;
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x0001BC4D File Offset: 0x00019E4D
	public void SetLightningFlashTrigger(float trigger)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.thunder = trigger;
			return;
		}
		EnviroSkyLite.instance.thunder = trigger;
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x0001BC70 File Offset: 0x00019E70
	public float GetEmissionRate(ParticleSystem system)
	{
		return system.emission.rateOverTime.constantMax;
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x0001BC94 File Offset: 0x00019E94
	public void SetEmissionRate(ParticleSystem sys, float emissionRate)
	{
		ParticleSystem.EmissionModule emission = sys.emission;
		ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
		rateOverTime.constantMax = emissionRate;
		emission.rateOverTime = rateOverTime;
	}

	// Token: 0x060002E3 RID: 739 RVA: 0x0001BCC0 File Offset: 0x00019EC0
	public void RegisterVegetationInstance(EnviroVegetationInstance v)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSky.instance.RegisterMe(v);
			return;
		}
		EnviroSkyLite.instance.RegisterMe(v);
	}

	// Token: 0x060002E4 RID: 740 RVA: 0x0001BCE4 File Offset: 0x00019EE4
	public double GetInHours(float hours, float days, float years)
	{
		if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			return EnviroSky.instance.GetInHours(hours, days, years, EnviroSky.instance.GameTime.DaysInYear);
		}
		return EnviroSkyLite.instance.GetInHours(hours, days, years, EnviroSkyLite.instance.GameTime.DaysInYear);
	}

	// Token: 0x1700002B RID: 43
	// (get) Token: 0x060002E5 RID: 741 RVA: 0x0001BD33 File Offset: 0x00019F33
	// (set) Token: 0x060002E6 RID: 742 RVA: 0x0001BD4A File Offset: 0x00019F4A
	public bool useVolumeClouds
	{
		get
		{
			return this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD && EnviroSky.instance.useVolumeClouds;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.useVolumeClouds = value;
			}
		}
	}

	// Token: 0x1700002C RID: 44
	// (get) Token: 0x060002E7 RID: 743 RVA: 0x0001BD60 File Offset: 0x00019F60
	// (set) Token: 0x060002E8 RID: 744 RVA: 0x0001BD77 File Offset: 0x00019F77
	public bool useAurora
	{
		get
		{
			return this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD && EnviroSky.instance.useAurora;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.useAurora = value;
			}
		}
	}

	// Token: 0x1700002D RID: 45
	// (get) Token: 0x060002E9 RID: 745 RVA: 0x0001BD8D File Offset: 0x00019F8D
	// (set) Token: 0x060002EA RID: 746 RVA: 0x0001BDA4 File Offset: 0x00019FA4
	public bool useVolumeLighting
	{
		get
		{
			return this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD && EnviroSky.instance.useVolumeLighting;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.useVolumeLighting = value;
			}
		}
	}

	// Token: 0x1700002E RID: 46
	// (get) Token: 0x060002EB RID: 747 RVA: 0x0001BDBA File Offset: 0x00019FBA
	// (set) Token: 0x060002EC RID: 748 RVA: 0x0001BDD1 File Offset: 0x00019FD1
	public bool useFlatClouds
	{
		get
		{
			return this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD && EnviroSky.instance.useFlatClouds;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.useFlatClouds = value;
			}
		}
	}

	// Token: 0x1700002F RID: 47
	// (get) Token: 0x060002ED RID: 749 RVA: 0x0001BDE7 File Offset: 0x00019FE7
	// (set) Token: 0x060002EE RID: 750 RVA: 0x0001BE07 File Offset: 0x0001A007
	public bool useParticleClouds
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.useParticleClouds;
			}
			return EnviroSkyLite.instance.useParticleClouds;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.useParticleClouds = value;
				return;
			}
			EnviroSkyLite.instance.useParticleClouds = value;
		}
	}

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x060002EF RID: 751 RVA: 0x0001BE29 File Offset: 0x0001A029
	// (set) Token: 0x060002F0 RID: 752 RVA: 0x0001BE53 File Offset: 0x0001A053
	public bool useSunShafts
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.LightShafts.sunLightShafts;
			}
			return EnviroSkyLite.instance.LightShafts.sunLightShafts;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.LightShafts.sunLightShafts = value;
				return;
			}
			EnviroSkyLite.instance.LightShafts.sunLightShafts = value;
		}
	}

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x060002F1 RID: 753 RVA: 0x0001BE7F File Offset: 0x0001A07F
	// (set) Token: 0x060002F2 RID: 754 RVA: 0x0001BEA9 File Offset: 0x0001A0A9
	public bool useMoonShafts
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.LightShafts.moonLightShafts;
			}
			return EnviroSkyLite.instance.LightShafts.moonLightShafts;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.LightShafts.moonLightShafts = value;
				return;
			}
			EnviroSkyLite.instance.LightShafts.moonLightShafts = value;
		}
	}

	// Token: 0x17000032 RID: 50
	// (get) Token: 0x060002F3 RID: 755 RVA: 0x0001BED5 File Offset: 0x0001A0D5
	// (set) Token: 0x060002F4 RID: 756 RVA: 0x0001BEEC File Offset: 0x0001A0EC
	public bool useDistanceBlur
	{
		get
		{
			return this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD && EnviroSky.instance.useDistanceBlur;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.useDistanceBlur = value;
			}
		}
	}

	// Token: 0x14000001 RID: 1
	// (add) Token: 0x060002F5 RID: 757 RVA: 0x0001BF04 File Offset: 0x0001A104
	// (remove) Token: 0x060002F6 RID: 758 RVA: 0x0001BF3C File Offset: 0x0001A13C
	public event EnviroSkyMgr.HourPassed OnHourPassed;

	// Token: 0x14000002 RID: 2
	// (add) Token: 0x060002F7 RID: 759 RVA: 0x0001BF74 File Offset: 0x0001A174
	// (remove) Token: 0x060002F8 RID: 760 RVA: 0x0001BFAC File Offset: 0x0001A1AC
	public event EnviroSkyMgr.DayPassed OnDayPassed;

	// Token: 0x14000003 RID: 3
	// (add) Token: 0x060002F9 RID: 761 RVA: 0x0001BFE4 File Offset: 0x0001A1E4
	// (remove) Token: 0x060002FA RID: 762 RVA: 0x0001C01C File Offset: 0x0001A21C
	public event EnviroSkyMgr.YearPassed OnYearPassed;

	// Token: 0x14000004 RID: 4
	// (add) Token: 0x060002FB RID: 763 RVA: 0x0001C054 File Offset: 0x0001A254
	// (remove) Token: 0x060002FC RID: 764 RVA: 0x0001C08C File Offset: 0x0001A28C
	public event EnviroSkyMgr.WeatherChanged OnWeatherChanged;

	// Token: 0x14000005 RID: 5
	// (add) Token: 0x060002FD RID: 765 RVA: 0x0001C0C4 File Offset: 0x0001A2C4
	// (remove) Token: 0x060002FE RID: 766 RVA: 0x0001C0FC File Offset: 0x0001A2FC
	public event EnviroSkyMgr.ZoneWeatherChanged OnZoneWeatherChanged;

	// Token: 0x14000006 RID: 6
	// (add) Token: 0x060002FF RID: 767 RVA: 0x0001C134 File Offset: 0x0001A334
	// (remove) Token: 0x06000300 RID: 768 RVA: 0x0001C16C File Offset: 0x0001A36C
	public event EnviroSkyMgr.SeasonChanged OnSeasonChanged;

	// Token: 0x14000007 RID: 7
	// (add) Token: 0x06000301 RID: 769 RVA: 0x0001C1A4 File Offset: 0x0001A3A4
	// (remove) Token: 0x06000302 RID: 770 RVA: 0x0001C1DC File Offset: 0x0001A3DC
	public event EnviroSkyMgr.isNightE OnNightTime;

	// Token: 0x14000008 RID: 8
	// (add) Token: 0x06000303 RID: 771 RVA: 0x0001C214 File Offset: 0x0001A414
	// (remove) Token: 0x06000304 RID: 772 RVA: 0x0001C24C File Offset: 0x0001A44C
	public event EnviroSkyMgr.isDay OnDayTime;

	// Token: 0x14000009 RID: 9
	// (add) Token: 0x06000305 RID: 773 RVA: 0x0001C284 File Offset: 0x0001A484
	// (remove) Token: 0x06000306 RID: 774 RVA: 0x0001C2BC File Offset: 0x0001A4BC
	public event EnviroSkyMgr.ZoneChanged OnZoneChanged;

	// Token: 0x06000307 RID: 775 RVA: 0x0001C2F1 File Offset: 0x0001A4F1
	public virtual void NotifyHourPassed()
	{
		if (this.OnHourPassed != null)
		{
			this.OnHourPassed();
		}
	}

	// Token: 0x06000308 RID: 776 RVA: 0x0001C306 File Offset: 0x0001A506
	public virtual void NotifyDayPassed()
	{
		if (this.OnDayPassed != null)
		{
			this.OnDayPassed();
		}
	}

	// Token: 0x06000309 RID: 777 RVA: 0x0001C31B File Offset: 0x0001A51B
	public virtual void NotifyYearPassed()
	{
		if (this.OnYearPassed != null)
		{
			this.OnYearPassed();
		}
	}

	// Token: 0x0600030A RID: 778 RVA: 0x0001C330 File Offset: 0x0001A530
	public virtual void NotifyWeatherChanged(EnviroWeatherPreset type)
	{
		if (this.OnWeatherChanged != null)
		{
			this.OnWeatherChanged(type);
		}
	}

	// Token: 0x0600030B RID: 779 RVA: 0x0001C346 File Offset: 0x0001A546
	public virtual void NotifyZoneWeatherChanged(EnviroWeatherPreset type, EnviroZone zone)
	{
		if (this.OnZoneWeatherChanged != null)
		{
			this.OnZoneWeatherChanged(type, zone);
		}
	}

	// Token: 0x0600030C RID: 780 RVA: 0x0001C35D File Offset: 0x0001A55D
	public virtual void NotifySeasonChanged(EnviroSeasons.Seasons season)
	{
		if (this.OnSeasonChanged != null)
		{
			this.OnSeasonChanged(season);
		}
	}

	// Token: 0x0600030D RID: 781 RVA: 0x0001C373 File Offset: 0x0001A573
	public virtual void NotifyIsNight()
	{
		if (this.OnNightTime != null)
		{
			this.OnNightTime();
		}
	}

	// Token: 0x0600030E RID: 782 RVA: 0x0001C388 File Offset: 0x0001A588
	public virtual void NotifyIsDay()
	{
		if (this.OnDayTime != null)
		{
			this.OnDayTime();
		}
	}

	// Token: 0x0600030F RID: 783 RVA: 0x0001C39D File Offset: 0x0001A59D
	public virtual void NotifyZoneChanged(EnviroZone zone)
	{
		if (this.OnZoneChanged != null)
		{
			this.OnZoneChanged(zone);
		}
	}

	// Token: 0x17000033 RID: 51
	// (get) Token: 0x06000310 RID: 784 RVA: 0x0001AD68 File Offset: 0x00018F68
	// (set) Token: 0x06000311 RID: 785 RVA: 0x0001C3B3 File Offset: 0x0001A5B3
	public bool interiorMode
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.interiorMode;
			}
			return EnviroSkyLite.instance.interiorMode;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.interiorMode = value;
				return;
			}
			EnviroSkyLite.instance.interiorMode = value;
		}
	}

	// Token: 0x17000034 RID: 52
	// (get) Token: 0x06000312 RID: 786 RVA: 0x0001C3D5 File Offset: 0x0001A5D5
	// (set) Token: 0x06000313 RID: 787 RVA: 0x0001C3F5 File Offset: 0x0001A5F5
	public EnviroInterior lastInteriorZone
	{
		get
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				return EnviroSky.instance.lastInteriorZone;
			}
			return EnviroSkyLite.instance.lastInteriorZone;
		}
		set
		{
			if (this.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				EnviroSky.instance.lastInteriorZone = value;
				return;
			}
			EnviroSkyLite.instance.lastInteriorZone = value;
		}
	}

	// Token: 0x040006A5 RID: 1701
	private static EnviroSkyMgr _instance;

	// Token: 0x040006A6 RID: 1702
	[Header("General")]
	[Tooltip("Enable to make sure thast enviro objects don't get destroyed on scene load.")]
	public bool dontDestroy;

	// Token: 0x040006A7 RID: 1703
	public bool showSetup = true;

	// Token: 0x040006A8 RID: 1704
	public bool showInstances = true;

	// Token: 0x040006A9 RID: 1705
	public bool showThirdParty;

	// Token: 0x040006AA RID: 1706
	public bool showUtilities;

	// Token: 0x040006AB RID: 1707
	public bool showThirdPartyShaders;

	// Token: 0x040006AC RID: 1708
	public bool showThirdPartyMisc;

	// Token: 0x040006AD RID: 1709
	public bool showThirdPartyNetwork;

	// Token: 0x040006AE RID: 1710
	public bool showUtiliies;

	// Token: 0x040006AF RID: 1711
	public RenderTexture cube;

	// Token: 0x040006B0 RID: 1712
	public EnviroSkyMgr.EnviroBaking skyBaking;

	// Token: 0x040006B1 RID: 1713
	public EnviroSkyMgr.EnviroRenderPipeline currentRenderPipeline;

	// Token: 0x040006B2 RID: 1714
	public EnviroSkyMgr.EnviroSkyVersion currentEnviroSkyVersion = EnviroSkyMgr.EnviroSkyVersion.HD;

	// Token: 0x040006B3 RID: 1715
	public EnviroSky enviroHDInstance;

	// Token: 0x040006B4 RID: 1716
	public EnviroSkyLite enviroLWInstance;

	// Token: 0x02000386 RID: 902
	[Serializable]
	public class EnviroBaking
	{
		// Token: 0x0400273B RID: 10043
		public int resolution = 2048;
	}

	// Token: 0x02000387 RID: 903
	public enum EnviroSkyVersion
	{
		// Token: 0x0400273D RID: 10045
		None,
		// Token: 0x0400273E RID: 10046
		LW,
		// Token: 0x0400273F RID: 10047
		HD
	}

	// Token: 0x02000388 RID: 904
	public enum EnviroRenderPipeline
	{
		// Token: 0x04002741 RID: 10049
		Legacy,
		// Token: 0x04002742 RID: 10050
		LWRP
	}

	// Token: 0x02000389 RID: 905
	// (Invoke) Token: 0x0600167A RID: 5754
	public delegate void HourPassed();

	// Token: 0x0200038A RID: 906
	// (Invoke) Token: 0x0600167E RID: 5758
	public delegate void DayPassed();

	// Token: 0x0200038B RID: 907
	// (Invoke) Token: 0x06001682 RID: 5762
	public delegate void YearPassed();

	// Token: 0x0200038C RID: 908
	// (Invoke) Token: 0x06001686 RID: 5766
	public delegate void WeatherChanged(EnviroWeatherPreset weatherType);

	// Token: 0x0200038D RID: 909
	// (Invoke) Token: 0x0600168A RID: 5770
	public delegate void ZoneWeatherChanged(EnviroWeatherPreset weatherType, EnviroZone zone);

	// Token: 0x0200038E RID: 910
	// (Invoke) Token: 0x0600168E RID: 5774
	public delegate void SeasonChanged(EnviroSeasons.Seasons season);

	// Token: 0x0200038F RID: 911
	// (Invoke) Token: 0x06001692 RID: 5778
	public delegate void isNightE();

	// Token: 0x02000390 RID: 912
	// (Invoke) Token: 0x06001696 RID: 5782
	public delegate void isDay();

	// Token: 0x02000391 RID: 913
	// (Invoke) Token: 0x0600169A RID: 5786
	public delegate void ZoneChanged(EnviroZone zone);
}
