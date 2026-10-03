using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000099 RID: 153
[AddComponentMenu("Enviro/Interior Zone")]
public class EnviroInterior : MonoBehaviour
{
	// Token: 0x06000351 RID: 849 RVA: 0x00002188 File Offset: 0x00000388
	private void Start()
	{
	}

	// Token: 0x06000352 RID: 850 RVA: 0x0001E3BC File Offset: 0x0001C5BC
	public void CreateNewTrigger()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "Trigger " + this.triggers.Count.ToString();
		gameObject.transform.SetParent(base.transform, false);
		gameObject.AddComponent<BoxCollider>().isTrigger = true;
		EnviroTrigger enviroTrigger = gameObject.AddComponent<EnviroTrigger>();
		enviroTrigger.myZone = this;
		enviroTrigger.name = gameObject.name;
		this.triggers.Add(enviroTrigger);
	}

	// Token: 0x06000353 RID: 851 RVA: 0x0001E436 File Offset: 0x0001C636
	public void RemoveTrigger(EnviroTrigger id)
	{
		Object.DestroyImmediate(id.gameObject);
		this.triggers.Remove(id);
	}

	// Token: 0x06000354 RID: 852 RVA: 0x0001E450 File Offset: 0x0001C650
	public void Enter()
	{
		EnviroSkyMgr.instance.interiorMode = true;
		EnviroSkyMgr.instance.lastInteriorZone = this;
		if (this.directLighting)
		{
			this.fadeOutDirectLight = false;
			this.fadeInDirectLight = true;
		}
		if (this.ambientLighting)
		{
			this.fadeOutAmbientLight = false;
			this.fadeInAmbientLight = true;
		}
		if (this.skybox)
		{
			this.fadeOutSkybox = false;
			this.fadeInSkybox = true;
		}
		if (this.ambientAudio)
		{
			EnviroSkyMgr.instance.ambientAudioVolumeModifier = this.ambientVolume;
		}
		if (this.weatherAudio)
		{
			EnviroSkyMgr.instance.weatherAudioVolumeModifier = this.weatherVolume;
		}
		if (this.zoneAudioClip != null)
		{
			EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorZoneAudioFadingSpeed = this.zoneAudioFadingSpeed;
			EnviroSkyMgr.instance.AudioSettings.AudioSourceZone.FadeIn(this.zoneAudioClip);
			EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorZoneAudioVolume = this.zoneAudioVolume;
		}
		if (this.fog)
		{
			this.fadeOutFog = false;
			this.fadeInFog = true;
		}
		if (this.fogColor)
		{
			this.fadeOutFogColor = false;
			this.fadeInFogColor = true;
		}
		if (this.weatherEffects)
		{
			this.fadeOutWeather = false;
			this.fadeInWeather = true;
		}
	}

	// Token: 0x06000355 RID: 853 RVA: 0x0001E57C File Offset: 0x0001C77C
	public void Exit()
	{
		EnviroSkyMgr.instance.interiorMode = false;
		if (this.directLighting)
		{
			this.fadeInDirectLight = false;
			this.fadeOutDirectLight = true;
		}
		if (this.ambientLighting)
		{
			this.fadeOutAmbientLight = true;
			this.fadeInAmbientLight = false;
		}
		if (this.skybox)
		{
			this.fadeOutSkybox = true;
			this.fadeInSkybox = false;
		}
		if (this.ambientAudio)
		{
			EnviroSkyMgr.instance.ambientAudioVolumeModifier = 0f;
		}
		if (this.weatherAudio)
		{
			EnviroSkyMgr.instance.weatherAudioVolumeModifier = 0f;
		}
		if (this.zoneAudioClip != null)
		{
			EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorZoneAudioFadingSpeed = this.zoneAudioFadingSpeed;
			EnviroSkyMgr.instance.AudioSettings.AudioSourceZone.FadeOut();
		}
		if (this.fog)
		{
			this.fadeOutFog = true;
			this.fadeInFog = false;
		}
		if (this.fogColor)
		{
			this.fadeOutFogColor = true;
			this.fadeInFogColor = false;
		}
		if (this.weatherEffects)
		{
			this.fadeOutWeather = true;
			this.fadeInWeather = false;
		}
	}

	// Token: 0x06000356 RID: 854 RVA: 0x0001E680 File Offset: 0x0001C880
	public void StopAllFading()
	{
		if (this.directLighting)
		{
			this.fadeInDirectLight = false;
			this.fadeOutDirectLight = false;
		}
		if (this.ambientLighting)
		{
			this.fadeOutAmbientLight = false;
			this.fadeInAmbientLight = false;
		}
		if (this.zoneAudioClip != null)
		{
			EnviroSkyMgr.instance.AudioSettings.AudioSourceZone.FadeOut();
		}
		if (this.skybox)
		{
			this.fadeOutSkybox = false;
			this.fadeInSkybox = false;
		}
		if (this.fog)
		{
			this.fadeOutFog = false;
			this.fadeInFog = false;
		}
		if (this.fogColor)
		{
			this.fadeOutFogColor = false;
			this.fadeInFogColor = false;
		}
		if (this.weatherEffects)
		{
			this.fadeOutWeather = false;
			this.fadeInWeather = false;
		}
	}

	// Token: 0x06000357 RID: 855 RVA: 0x0001E734 File Offset: 0x0001C934
	private void Update()
	{
		if (EnviroSkyMgr.instance == null || !EnviroSkyMgr.instance.IsAvailable())
		{
			return;
		}
		if (this.directLighting)
		{
			if (this.fadeInDirectLight)
			{
				this.curDirectLightingMod = Color.Lerp(this.curDirectLightingMod, this.directLightingMod, this.directLightFadeSpeed * Time.deltaTime);
				EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorDirectLightMod = this.curDirectLightingMod;
				if (this.curDirectLightingMod == this.directLightingMod)
				{
					this.fadeInDirectLight = false;
				}
			}
			else if (this.fadeOutDirectLight)
			{
				this.curDirectLightingMod = Color.Lerp(this.curDirectLightingMod, this.fadeOutColor, this.directLightFadeSpeed * Time.deltaTime);
				EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorDirectLightMod = this.curDirectLightingMod;
				if (this.curDirectLightingMod == this.fadeOutColor)
				{
					this.fadeOutDirectLight = false;
				}
			}
		}
		if (this.ambientLighting)
		{
			if (this.fadeInAmbientLight)
			{
				this.curAmbientLightingMod = Color.Lerp(this.curAmbientLightingMod, this.ambientLightingMod, this.ambientLightFadeSpeed * Time.deltaTime);
				EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorAmbientLightMod = this.curAmbientLightingMod;
				if (EnviroSkyMgr.instance.LightSettings.ambientMode == AmbientMode.Trilight)
				{
					this.curAmbientEQLightingMod = Color.Lerp(this.curAmbientEQLightingMod, this.ambientEQLightingMod, this.ambientLightFadeSpeed * Time.deltaTime);
					EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorAmbientEQLightMod = this.curAmbientEQLightingMod;
					this.curAmbientGRLightingMod = Color.Lerp(this.curAmbientGRLightingMod, this.ambientGRLightingMod, this.ambientLightFadeSpeed * Time.deltaTime);
					EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorAmbientGRLightMod = this.curAmbientGRLightingMod;
				}
				if (this.curAmbientLightingMod == this.ambientLightingMod)
				{
					this.fadeInAmbientLight = false;
				}
			}
			else if (this.fadeOutAmbientLight)
			{
				this.curAmbientLightingMod = Color.Lerp(this.curAmbientLightingMod, this.fadeOutColor, 2f * Time.deltaTime);
				EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorAmbientLightMod = this.curAmbientLightingMod;
				if (EnviroSkyMgr.instance.LightSettings.ambientMode == AmbientMode.Trilight)
				{
					this.curAmbientEQLightingMod = Color.Lerp(this.curAmbientEQLightingMod, this.fadeOutColor, 2f * Time.deltaTime);
					EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorAmbientEQLightMod = this.curAmbientEQLightingMod;
					this.curAmbientGRLightingMod = Color.Lerp(this.curAmbientGRLightingMod, this.fadeOutColor, 2f * Time.deltaTime);
					EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorAmbientGRLightMod = this.curAmbientGRLightingMod;
				}
				if (this.curAmbientLightingMod == this.fadeOutColor)
				{
					this.fadeOutAmbientLight = false;
				}
			}
		}
		if (this.skybox)
		{
			if (this.fadeInSkybox)
			{
				this.curskyboxColorMod = Color.Lerp(this.curskyboxColorMod, this.skyboxColorMod, this.skyboxFadeSpeed * Time.deltaTime);
				EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorSkyboxMod = this.curskyboxColorMod;
				if (this.curskyboxColorMod == this.skyboxColorMod)
				{
					this.fadeInSkybox = false;
				}
			}
			else if (this.fadeOutSkybox)
			{
				this.curskyboxColorMod = Color.Lerp(this.curskyboxColorMod, this.fadeOutColor, this.skyboxFadeSpeed * Time.deltaTime);
				EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorSkyboxMod = this.curskyboxColorMod;
				if (this.curskyboxColorMod == this.fadeOutColor)
				{
					this.fadeOutSkybox = false;
				}
			}
		}
		if (this.fog)
		{
			if (this.fadeInFog)
			{
				EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorFogMod = Mathf.Lerp(EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorFogMod, this.minFogMod, this.fogFadeSpeed * Time.deltaTime);
				if ((double)EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorFogMod <= (double)this.minFogMod + 0.001)
				{
					this.fadeInFog = false;
				}
			}
			else if (this.fadeOutFog)
			{
				EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorFogMod = Mathf.Lerp(EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorFogMod, 1f, this.fogFadeSpeed * 2f * Time.deltaTime);
				if ((double)EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorFogMod >= 0.999)
				{
					this.fadeOutFog = false;
				}
			}
		}
		if (this.fogColor)
		{
			if (this.fadeInFogColor)
			{
				this.curFogColorMod = Color.Lerp(this.curFogColorMod, this.fogColorMod, this.fogFadeSpeed * Time.deltaTime);
				EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorFogColorMod = this.curFogColorMod;
				if (this.curFogColorMod == this.fogColorMod)
				{
					this.fadeInFogColor = false;
				}
			}
			else if (this.fadeOutFogColor)
			{
				this.curFogColorMod = Color.Lerp(this.curFogColorMod, this.fadeOutColor, this.fogFadeSpeed * Time.deltaTime);
				EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorFogColorMod = this.curFogColorMod;
				if (this.curFogColorMod == this.fadeOutColor)
				{
					this.fadeOutFogColor = false;
				}
			}
		}
		if (this.weatherEffects)
		{
			if (this.fadeInWeather)
			{
				EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorWeatherEffectMod = Mathf.Lerp(EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorWeatherEffectMod, 0f, this.weatherFadeSpeed * Time.deltaTime);
				if ((double)EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorWeatherEffectMod <= 0.001)
				{
					this.fadeInWeather = false;
					return;
				}
			}
			else if (this.fadeOutWeather)
			{
				EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorWeatherEffectMod = Mathf.Lerp(EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorWeatherEffectMod, 1f, this.weatherFadeSpeed * 2f * Time.deltaTime);
				if ((double)EnviroSkyMgr.instance.InteriorZoneSettings.currentInteriorWeatherEffectMod >= 0.999)
				{
					this.fadeOutWeather = false;
				}
			}
		}
	}

	// Token: 0x04000756 RID: 1878
	public EnviroInterior.ZoneTriggerType zoneTriggerType;

	// Token: 0x04000757 RID: 1879
	public bool directLighting;

	// Token: 0x04000758 RID: 1880
	public bool ambientLighting;

	// Token: 0x04000759 RID: 1881
	public bool weatherAudio;

	// Token: 0x0400075A RID: 1882
	public bool ambientAudio;

	// Token: 0x0400075B RID: 1883
	public bool fog;

	// Token: 0x0400075C RID: 1884
	public bool fogColor;

	// Token: 0x0400075D RID: 1885
	public bool skybox;

	// Token: 0x0400075E RID: 1886
	public bool weatherEffects;

	// Token: 0x0400075F RID: 1887
	public Color directLightingMod = Color.black;

	// Token: 0x04000760 RID: 1888
	public Color ambientLightingMod = Color.black;

	// Token: 0x04000761 RID: 1889
	public Color ambientEQLightingMod = Color.black;

	// Token: 0x04000762 RID: 1890
	public Color ambientGRLightingMod = Color.black;

	// Token: 0x04000763 RID: 1891
	private Color curDirectLightingMod;

	// Token: 0x04000764 RID: 1892
	private Color curAmbientLightingMod;

	// Token: 0x04000765 RID: 1893
	private Color curAmbientEQLightingMod;

	// Token: 0x04000766 RID: 1894
	private Color curAmbientGRLightingMod;

	// Token: 0x04000767 RID: 1895
	public float directLightFadeSpeed = 2f;

	// Token: 0x04000768 RID: 1896
	public float ambientLightFadeSpeed = 2f;

	// Token: 0x04000769 RID: 1897
	public Color skyboxColorMod = Color.black;

	// Token: 0x0400076A RID: 1898
	private Color curskyboxColorMod;

	// Token: 0x0400076B RID: 1899
	public float skyboxFadeSpeed = 2f;

	// Token: 0x0400076C RID: 1900
	private bool fadeInDirectLight;

	// Token: 0x0400076D RID: 1901
	private bool fadeOutDirectLight;

	// Token: 0x0400076E RID: 1902
	private bool fadeInAmbientLight;

	// Token: 0x0400076F RID: 1903
	private bool fadeOutAmbientLight;

	// Token: 0x04000770 RID: 1904
	private bool fadeInSkybox;

	// Token: 0x04000771 RID: 1905
	private bool fadeOutSkybox;

	// Token: 0x04000772 RID: 1906
	public float ambientVolume;

	// Token: 0x04000773 RID: 1907
	public float weatherVolume;

	// Token: 0x04000774 RID: 1908
	public AudioClip zoneAudioClip;

	// Token: 0x04000775 RID: 1909
	public float zoneAudioVolume = 1f;

	// Token: 0x04000776 RID: 1910
	public float zoneAudioFadingSpeed = 1f;

	// Token: 0x04000777 RID: 1911
	public Color fogColorMod = Color.black;

	// Token: 0x04000778 RID: 1912
	private Color curFogColorMod;

	// Token: 0x04000779 RID: 1913
	public float fogFadeSpeed = 2f;

	// Token: 0x0400077A RID: 1914
	public float minFogMod;

	// Token: 0x0400077B RID: 1915
	private bool fadeInFog;

	// Token: 0x0400077C RID: 1916
	private bool fadeOutFog;

	// Token: 0x0400077D RID: 1917
	private bool fadeInFogColor;

	// Token: 0x0400077E RID: 1918
	private bool fadeOutFogColor;

	// Token: 0x0400077F RID: 1919
	public float weatherFadeSpeed = 2f;

	// Token: 0x04000780 RID: 1920
	private bool fadeInWeather;

	// Token: 0x04000781 RID: 1921
	private bool fadeOutWeather;

	// Token: 0x04000782 RID: 1922
	public List<EnviroTrigger> triggers = new List<EnviroTrigger>();

	// Token: 0x04000783 RID: 1923
	private Color fadeOutColor = new Color(0f, 0f, 0f, 0f);

	// Token: 0x02000399 RID: 921
	public enum ZoneTriggerType
	{
		// Token: 0x0400275F RID: 10079
		Entry_Exit,
		// Token: 0x04002760 RID: 10080
		Zone
	}
}
