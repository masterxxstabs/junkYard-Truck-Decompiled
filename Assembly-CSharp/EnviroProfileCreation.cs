using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000089 RID: 137
public static class EnviroProfileCreation
{
	// Token: 0x0600023A RID: 570 RVA: 0x00017904 File Offset: 0x00015B04
	public static void SetupDefaults(EnviroProfile profile)
	{
		EnviroProfile defaultProfile = EnviroProfileCreation.GetDefaultProfile("enviro_internal_default_profile");
		profile.audioSettings = defaultProfile.audioSettings;
		profile.reflectionSettings = defaultProfile.reflectionSettings;
		profile.cloudsSettings = defaultProfile.cloudsSettings;
		profile.distanceBlurSettings = defaultProfile.distanceBlurSettings;
		profile.fogSettings = defaultProfile.fogSettings;
		profile.lightSettings = defaultProfile.lightSettings;
		profile.lightshaftsSettings = defaultProfile.lightshaftsSettings;
		profile.qualitySettings = defaultProfile.qualitySettings;
		profile.satelliteSettings = defaultProfile.satelliteSettings;
		profile.seasonsSettings = defaultProfile.seasonsSettings;
		profile.skySettings = defaultProfile.skySettings;
		profile.volumeLightSettings = defaultProfile.volumeLightSettings;
		profile.reflectionSettings = defaultProfile.reflectionSettings;
		profile.auroraSettings = defaultProfile.auroraSettings;
		profile.weatherSettings = defaultProfile.weatherSettings;
		profile.version = defaultProfile.version;
	}

	// Token: 0x0600023B RID: 571 RVA: 0x000179DC File Offset: 0x00015BDC
	public static bool UpdateProfile(EnviroProfile profile, string fromV, string toV)
	{
		if (profile == null)
		{
			return false;
		}
		EnviroProfile defaultProfile = EnviroProfileCreation.GetDefaultProfile("enviro_internal_default_profile");
		List<Color> list = new List<Color>();
		List<float> list2 = new List<float>();
		if ((fromV == "2.0.0" || fromV == "2.0.1" || fromV == "2.0.2") && toV == "2.2.1")
		{
			profile.skySettings.galaxyIntensity = new AnimationCurve();
			profile.skySettings.galaxyIntensity.AddKey(EnviroProfileCreation.CreateKey(0.4f, 0f));
			profile.skySettings.galaxyIntensity.AddKey(EnviroProfileCreation.CreateKey(0.015f, 0.5f));
			profile.skySettings.galaxyIntensity.AddKey(EnviroProfileCreation.CreateKey(0f, 0.6f));
			profile.skySettings.galaxyIntensity.AddKey(EnviroProfileCreation.CreateKey(0f, 1f));
			profile.skySettings.galaxyCubeMap = EnviroProfileCreation.GetAssetCubemap("cube_enviro_galaxy");
			profile.skySettings.moonTexture = EnviroProfileCreation.GetAssetTexture("tex_enviro_moon_standard");
			profile.weatherSettings.lightningEffect = EnviroProfileCreation.GetAssetPrefab("Enviro_Lightning_Strike");
			list.Add(EnviroProfileCreation.GetColor("#0F1013"));
			list2.Add(0f);
			list.Add(EnviroProfileCreation.GetColor("#272A35"));
			list2.Add(0.465f);
			list.Add(EnviroProfileCreation.GetColor("#277BA5"));
			list2.Add(0.5f);
			list.Add(EnviroProfileCreation.GetColor("#7CA0BA"));
			list2.Add(0.56f);
			list.Add(EnviroProfileCreation.GetColor("#5687B8"));
			list2.Add(1f);
			List<float> list3 = new List<float>();
			List<float> list4 = new List<float>();
			list3.Add(0f);
			list4.Add(0f);
			list3.Add(0.5f);
			list4.Add(0.47f);
			list3.Add(1f);
			list4.Add(1f);
			profile.skySettings.simpleSkyColor = EnviroProfileCreation.CreateGradient(list, list2, list3, list4);
			list = new List<Color>();
			list2 = new List<float>();
			list.Add(EnviroProfileCreation.GetColor("#0F1013"));
			list2.Add(0f);
			list.Add(EnviroProfileCreation.GetColor("#272A35"));
			list2.Add(0.46f);
			list.Add(EnviroProfileCreation.GetColor("#DD8F3B"));
			list2.Add(0.506f);
			list.Add(EnviroProfileCreation.GetColor("#DADADA"));
			list2.Add(0.603f);
			list.Add(EnviroProfileCreation.GetColor("#FFFFFF"));
			list2.Add(1f);
			profile.skySettings.simpleHorizonColor = EnviroProfileCreation.CreateGradient(list, list2);
			list = new List<Color>();
			list2 = new List<float>();
			list.Add(EnviroProfileCreation.GetColor("#000000"));
			list2.Add(0f);
			list.Add(EnviroProfileCreation.GetColor("#FF6340"));
			list2.Add(0.46f);
			list.Add(EnviroProfileCreation.GetColor("#FF7308"));
			list2.Add(0.506f);
			list.Add(EnviroProfileCreation.GetColor("#E3C29E"));
			list2.Add(0.56f);
			list.Add(EnviroProfileCreation.GetColor("#FFE9D4"));
			list2.Add(1f);
			profile.skySettings.simpleSunColor = EnviroProfileCreation.CreateGradient(list, list2);
			list = new List<Color>();
			list2 = new List<float>();
			list.Add(EnviroProfileCreation.GetColor("#101217"));
			list2.Add(0f);
			list.Add(EnviroProfileCreation.GetColor("#4C5570"));
			list2.Add(0.46f);
			list.Add(EnviroProfileCreation.GetColor("#D8A269"));
			list2.Add(0.5f);
			list.Add(EnviroProfileCreation.GetColor("#C7D7E3"));
			list2.Add(0.57f);
			list.Add(EnviroProfileCreation.GetColor("#DBE7F0"));
			list2.Add(1f);
			profile.fogSettings.simpleFogColor = EnviroProfileCreation.CreateGradient(list, list2);
			list = new List<Color>();
			list2 = new List<float>();
			profile.skySettings.simpleSunDiskSize = new AnimationCurve();
			profile.skySettings.simpleSunDiskSize.AddKey(EnviroProfileCreation.CreateKey(0.015f, 0f));
			profile.skySettings.simpleSunDiskSize.AddKey(EnviroProfileCreation.CreateKey(0.035f, 0.37f, -0.1f, -0.1f));
			profile.skySettings.simpleSunDiskSize.AddKey(EnviroProfileCreation.CreateKey(0.015f, 1f));
			list.Add(EnviroProfileCreation.GetColor("#24272D"));
			list2.Add(0f);
			list.Add(EnviroProfileCreation.GetColor("#2E3038"));
			list2.Add(0.46f);
			list.Add(EnviroProfileCreation.GetColor("#544E46"));
			list2.Add(0.51f);
			list.Add(EnviroProfileCreation.GetColor("#B7BABC"));
			list2.Add(0.56f);
			list.Add(EnviroProfileCreation.GetColor("#D2D2D2"));
			list2.Add(1f);
			profile.cloudsSettings.ParticleCloudsLayer1.particleCloudsColor = EnviroProfileCreation.CreateGradient(list, list2);
			profile.cloudsSettings.ParticleCloudsLayer2.particleCloudsColor = EnviroProfileCreation.CreateGradient(list, list2);
			list = new List<Color>();
			list2 = new List<float>();
			profile.volumeLightSettings.ScatteringCoef = new AnimationCurve();
			profile.volumeLightSettings.ScatteringCoef.AddKey(EnviroProfileCreation.CreateKey(0.5f, 0f));
			profile.volumeLightSettings.ScatteringCoef.AddKey(EnviroProfileCreation.CreateKey(0.2f, 0.5f));
			profile.volumeLightSettings.ScatteringCoef.AddKey(EnviroProfileCreation.CreateKey(0.15f, 1f));
			if (defaultProfile != null)
			{
				profile.cloudsSettings.hgPhase = defaultProfile.cloudsSettings.hgPhase;
				profile.cloudsSettings.silverLiningSpread = defaultProfile.cloudsSettings.silverLiningSpread;
				profile.cloudsSettings.silverLiningIntensity = defaultProfile.cloudsSettings.silverLiningIntensity;
				profile.cloudsSettings.attenuationClamp = defaultProfile.cloudsSettings.attenuationClamp;
				profile.cloudsSettings.lightIntensity = defaultProfile.cloudsSettings.lightIntensity;
				profile.cloudsSettings.volumeCloudsAmbientColor = defaultProfile.cloudsSettings.volumeCloudsAmbientColor;
				profile.cloudsSettings.ambientLightIntensity = defaultProfile.cloudsSettings.ambientLightIntensity;
				profile.auroraSettings.auroraIntensity = defaultProfile.auroraSettings.auroraIntensity;
				profile.cloudsSettings.ParticleCloudsLayer1.height = 0.01f;
				profile.cloudsSettings.ParticleCloudsLayer2.height = 0.01f;
				profile.version = toV;
				return true;
			}
			return false;
		}
		else if (fromV == "2.0.3" && toV == "2.2.1")
		{
			profile.weatherSettings.lightningEffect = EnviroProfileCreation.GetAssetPrefab("Enviro_Lightning_Strike");
			list.Add(EnviroProfileCreation.GetColor("#0F1013"));
			list2.Add(0f);
			list.Add(EnviroProfileCreation.GetColor("#272A35"));
			list2.Add(0.465f);
			list.Add(EnviroProfileCreation.GetColor("#277BA5"));
			list2.Add(0.5f);
			list.Add(EnviroProfileCreation.GetColor("#7CA0BA"));
			list2.Add(0.56f);
			list.Add(EnviroProfileCreation.GetColor("#5687B8"));
			list2.Add(1f);
			profile.skySettings.moonTexture = EnviroProfileCreation.GetAssetTexture("tex_enviro_moon_standard");
			List<float> list5 = new List<float>();
			List<float> list6 = new List<float>();
			list5.Add(0f);
			list6.Add(0f);
			list5.Add(0.5f);
			list6.Add(0.47f);
			list5.Add(1f);
			list6.Add(1f);
			profile.skySettings.simpleSkyColor = EnviroProfileCreation.CreateGradient(list, list2, list5, list6);
			list = new List<Color>();
			list2 = new List<float>();
			list.Add(EnviroProfileCreation.GetColor("#0F1013"));
			list2.Add(0f);
			list.Add(EnviroProfileCreation.GetColor("#272A35"));
			list2.Add(0.46f);
			list.Add(EnviroProfileCreation.GetColor("#DD8F3B"));
			list2.Add(0.506f);
			list.Add(EnviroProfileCreation.GetColor("#DADADA"));
			list2.Add(0.603f);
			list.Add(EnviroProfileCreation.GetColor("#FFFFFF"));
			list2.Add(1f);
			profile.skySettings.simpleHorizonColor = EnviroProfileCreation.CreateGradient(list, list2);
			list = new List<Color>();
			list2 = new List<float>();
			list.Add(EnviroProfileCreation.GetColor("#000000"));
			list2.Add(0f);
			list.Add(EnviroProfileCreation.GetColor("#FF6340"));
			list2.Add(0.46f);
			list.Add(EnviroProfileCreation.GetColor("#FF7308"));
			list2.Add(0.506f);
			list.Add(EnviroProfileCreation.GetColor("#E3C29E"));
			list2.Add(0.56f);
			list.Add(EnviroProfileCreation.GetColor("#FFE9D4"));
			list2.Add(1f);
			profile.skySettings.simpleSunColor = EnviroProfileCreation.CreateGradient(list, list2);
			list = new List<Color>();
			list2 = new List<float>();
			list.Add(EnviroProfileCreation.GetColor("#101217"));
			list2.Add(0f);
			list.Add(EnviroProfileCreation.GetColor("#4C5570"));
			list2.Add(0.46f);
			list.Add(EnviroProfileCreation.GetColor("#D8A269"));
			list2.Add(0.5f);
			list.Add(EnviroProfileCreation.GetColor("#C7D7E3"));
			list2.Add(0.57f);
			list.Add(EnviroProfileCreation.GetColor("#DBE7F0"));
			list2.Add(1f);
			profile.fogSettings.simpleFogColor = EnviroProfileCreation.CreateGradient(list, list2);
			list = new List<Color>();
			list2 = new List<float>();
			profile.skySettings.simpleSunDiskSize = new AnimationCurve();
			profile.skySettings.simpleSunDiskSize.AddKey(EnviroProfileCreation.CreateKey(0.015f, 0f));
			profile.skySettings.simpleSunDiskSize.AddKey(EnviroProfileCreation.CreateKey(0.035f, 0.37f, -0.1f, -0.1f));
			profile.skySettings.simpleSunDiskSize.AddKey(EnviroProfileCreation.CreateKey(0.015f, 1f));
			list.Add(EnviroProfileCreation.GetColor("#24272D"));
			list2.Add(0f);
			list.Add(EnviroProfileCreation.GetColor("#2E3038"));
			list2.Add(0.46f);
			list.Add(EnviroProfileCreation.GetColor("#544E46"));
			list2.Add(0.51f);
			list.Add(EnviroProfileCreation.GetColor("#B7BABC"));
			list2.Add(0.56f);
			list.Add(EnviroProfileCreation.GetColor("#D2D2D2"));
			list2.Add(1f);
			profile.cloudsSettings.ParticleCloudsLayer1.particleCloudsColor = EnviroProfileCreation.CreateGradient(list, list2);
			profile.cloudsSettings.ParticleCloudsLayer2.particleCloudsColor = EnviroProfileCreation.CreateGradient(list, list2);
			list = new List<Color>();
			list2 = new List<float>();
			profile.volumeLightSettings.ScatteringCoef = new AnimationCurve();
			profile.volumeLightSettings.ScatteringCoef.AddKey(EnviroProfileCreation.CreateKey(0.5f, 0f));
			profile.volumeLightSettings.ScatteringCoef.AddKey(EnviroProfileCreation.CreateKey(0.2f, 0.5f));
			profile.volumeLightSettings.ScatteringCoef.AddKey(EnviroProfileCreation.CreateKey(0.15f, 1f));
			if (defaultProfile != null)
			{
				profile.cloudsSettings.hgPhase = defaultProfile.cloudsSettings.hgPhase;
				profile.cloudsSettings.silverLiningSpread = defaultProfile.cloudsSettings.silverLiningSpread;
				profile.cloudsSettings.silverLiningIntensity = defaultProfile.cloudsSettings.silverLiningIntensity;
				profile.cloudsSettings.lightIntensity = defaultProfile.cloudsSettings.lightIntensity;
				profile.cloudsSettings.attenuationClamp = defaultProfile.cloudsSettings.attenuationClamp;
				profile.cloudsSettings.volumeCloudsAmbientColor = defaultProfile.cloudsSettings.volumeCloudsAmbientColor;
				profile.cloudsSettings.ambientLightIntensity = defaultProfile.cloudsSettings.ambientLightIntensity;
				profile.auroraSettings.auroraIntensity = defaultProfile.auroraSettings.auroraIntensity;
				profile.cloudsSettings.ParticleCloudsLayer1.height = 0.01f;
				profile.cloudsSettings.ParticleCloudsLayer2.height = 0.01f;
				profile.version = toV;
				return true;
			}
			return false;
		}
		else if (fromV == "2.0.4" || (fromV == "2.0.5" && toV == "2.2.1"))
		{
			list.Add(EnviroProfileCreation.GetColor("#0F1013"));
			list2.Add(0f);
			list.Add(EnviroProfileCreation.GetColor("#272A35"));
			list2.Add(0.465f);
			list.Add(EnviroProfileCreation.GetColor("#277BA5"));
			list2.Add(0.5f);
			list.Add(EnviroProfileCreation.GetColor("#7CA0BA"));
			list2.Add(0.56f);
			list.Add(EnviroProfileCreation.GetColor("#5687B8"));
			list2.Add(1f);
			List<float> list7 = new List<float>();
			List<float> list8 = new List<float>();
			list7.Add(0f);
			list8.Add(0f);
			list7.Add(0.5f);
			list8.Add(0.47f);
			list7.Add(1f);
			list8.Add(1f);
			profile.skySettings.simpleSkyColor = EnviroProfileCreation.CreateGradient(list, list2, list7, list8);
			list = new List<Color>();
			list2 = new List<float>();
			list.Add(EnviroProfileCreation.GetColor("#101217"));
			list2.Add(0f);
			list.Add(EnviroProfileCreation.GetColor("#4C5570"));
			list2.Add(0.46f);
			list.Add(EnviroProfileCreation.GetColor("#D8A269"));
			list2.Add(0.5f);
			list.Add(EnviroProfileCreation.GetColor("#C7D7E3"));
			list2.Add(0.57f);
			list.Add(EnviroProfileCreation.GetColor("#DBE7F0"));
			list2.Add(1f);
			profile.fogSettings.simpleFogColor = EnviroProfileCreation.CreateGradient(list, list2);
			list = new List<Color>();
			list2 = new List<float>();
			list.Add(EnviroProfileCreation.GetColor("#0F1013"));
			list2.Add(0f);
			list.Add(EnviroProfileCreation.GetColor("#272A35"));
			list2.Add(0.46f);
			list.Add(EnviroProfileCreation.GetColor("#DD8F3B"));
			list2.Add(0.506f);
			list.Add(EnviroProfileCreation.GetColor("#DADADA"));
			list2.Add(0.603f);
			list.Add(EnviroProfileCreation.GetColor("#FFFFFF"));
			list2.Add(1f);
			profile.skySettings.simpleHorizonColor = EnviroProfileCreation.CreateGradient(list, list2);
			list = new List<Color>();
			list2 = new List<float>();
			list.Add(EnviroProfileCreation.GetColor("#000000"));
			list2.Add(0f);
			list.Add(EnviroProfileCreation.GetColor("#FF6340"));
			list2.Add(0.46f);
			list.Add(EnviroProfileCreation.GetColor("#FF7308"));
			list2.Add(0.506f);
			list.Add(EnviroProfileCreation.GetColor("#E3C29E"));
			list2.Add(0.56f);
			list.Add(EnviroProfileCreation.GetColor("#FFE9D4"));
			list2.Add(1f);
			profile.skySettings.simpleSunColor = EnviroProfileCreation.CreateGradient(list, list2);
			list = new List<Color>();
			list2 = new List<float>();
			profile.skySettings.simpleSunDiskSize = new AnimationCurve();
			profile.skySettings.simpleSunDiskSize.AddKey(EnviroProfileCreation.CreateKey(0.015f, 0f));
			profile.skySettings.simpleSunDiskSize.AddKey(EnviroProfileCreation.CreateKey(0.035f, 0.37f, -0.1f, -0.1f));
			profile.skySettings.simpleSunDiskSize.AddKey(EnviroProfileCreation.CreateKey(0.015f, 1f));
			list.Add(EnviroProfileCreation.GetColor("#24272D"));
			list2.Add(0f);
			list.Add(EnviroProfileCreation.GetColor("#2E3038"));
			list2.Add(0.46f);
			list.Add(EnviroProfileCreation.GetColor("#544E46"));
			list2.Add(0.51f);
			list.Add(EnviroProfileCreation.GetColor("#B7BABC"));
			list2.Add(0.56f);
			list.Add(EnviroProfileCreation.GetColor("#D2D2D2"));
			list2.Add(1f);
			profile.cloudsSettings.ParticleCloudsLayer1.particleCloudsColor = EnviroProfileCreation.CreateGradient(list, list2);
			profile.cloudsSettings.ParticleCloudsLayer2.particleCloudsColor = EnviroProfileCreation.CreateGradient(list, list2);
			list = new List<Color>();
			list2 = new List<float>();
			profile.volumeLightSettings.ScatteringCoef = new AnimationCurve();
			profile.volumeLightSettings.ScatteringCoef.AddKey(EnviroProfileCreation.CreateKey(0.5f, 0f));
			profile.volumeLightSettings.ScatteringCoef.AddKey(EnviroProfileCreation.CreateKey(0.2f, 0.5f));
			profile.volumeLightSettings.ScatteringCoef.AddKey(EnviroProfileCreation.CreateKey(0.15f, 1f));
			if (defaultProfile != null)
			{
				profile.cloudsSettings.hgPhase = defaultProfile.cloudsSettings.hgPhase;
				profile.cloudsSettings.silverLiningSpread = defaultProfile.cloudsSettings.silverLiningSpread;
				profile.cloudsSettings.silverLiningIntensity = defaultProfile.cloudsSettings.silverLiningIntensity;
				profile.cloudsSettings.lightIntensity = defaultProfile.cloudsSettings.lightIntensity;
				profile.cloudsSettings.attenuationClamp = defaultProfile.cloudsSettings.attenuationClamp;
				profile.cloudsSettings.volumeCloudsAmbientColor = defaultProfile.cloudsSettings.volumeCloudsAmbientColor;
				profile.cloudsSettings.ambientLightIntensity = defaultProfile.cloudsSettings.ambientLightIntensity;
				profile.skySettings.moonTexture = defaultProfile.skySettings.moonTexture;
				profile.auroraSettings.auroraIntensity = defaultProfile.auroraSettings.auroraIntensity;
				profile.cloudsSettings.ParticleCloudsLayer1.height = 0.01f;
				profile.cloudsSettings.ParticleCloudsLayer2.height = 0.01f;
				profile.version = toV;
				return true;
			}
			return false;
		}
		else if (fromV == "2.1.0" || (fromV == "2.1.1" && toV == "2.2.1"))
		{
			if (defaultProfile != null)
			{
				profile.cloudsSettings.hgPhase = defaultProfile.cloudsSettings.hgPhase;
				profile.cloudsSettings.silverLiningSpread = defaultProfile.cloudsSettings.silverLiningSpread;
				profile.cloudsSettings.silverLiningIntensity = defaultProfile.cloudsSettings.silverLiningIntensity;
				profile.cloudsSettings.lightIntensity = defaultProfile.cloudsSettings.lightIntensity;
				profile.cloudsSettings.attenuationClamp = defaultProfile.cloudsSettings.attenuationClamp;
				profile.cloudsSettings.volumeCloudsAmbientColor = defaultProfile.cloudsSettings.volumeCloudsAmbientColor;
				profile.cloudsSettings.ambientLightIntensity = defaultProfile.cloudsSettings.ambientLightIntensity;
				profile.skySettings.moonTexture = defaultProfile.skySettings.moonTexture;
				profile.auroraSettings.auroraIntensity = defaultProfile.auroraSettings.auroraIntensity;
				profile.cloudsSettings.ParticleCloudsLayer1.height = 0.01f;
				profile.cloudsSettings.ParticleCloudsLayer2.height = 0.01f;
				profile.version = toV;
				return true;
			}
			return false;
		}
		else if (fromV == "2.1.2" && toV == "2.2.1")
		{
			if (defaultProfile != null)
			{
				profile.cloudsSettings.hgPhase = defaultProfile.cloudsSettings.hgPhase;
				profile.cloudsSettings.silverLiningSpread = defaultProfile.cloudsSettings.silverLiningSpread;
				profile.cloudsSettings.silverLiningIntensity = defaultProfile.cloudsSettings.silverLiningIntensity;
				profile.cloudsSettings.lightIntensity = defaultProfile.cloudsSettings.lightIntensity;
				profile.cloudsSettings.attenuationClamp = defaultProfile.cloudsSettings.attenuationClamp;
				profile.cloudsSettings.volumeCloudsAmbientColor = defaultProfile.cloudsSettings.volumeCloudsAmbientColor;
				profile.skySettings.moonTexture = defaultProfile.skySettings.moonTexture;
				profile.auroraSettings.auroraIntensity = defaultProfile.auroraSettings.auroraIntensity;
				profile.cloudsSettings.ParticleCloudsLayer1.height = 0.01f;
				profile.cloudsSettings.ParticleCloudsLayer2.height = 0.01f;
				profile.version = toV;
				return true;
			}
			return false;
		}
		else if (fromV == "2.1.3" || fromV == "2.1.4" || (fromV == "2.1.5" && toV == "2.2.1"))
		{
			if (defaultProfile != null)
			{
				profile.skySettings.moonTexture = defaultProfile.skySettings.moonTexture;
				profile.cloudsSettings.hgPhase = defaultProfile.cloudsSettings.hgPhase;
				profile.cloudsSettings.silverLiningSpread = defaultProfile.cloudsSettings.silverLiningSpread;
				profile.cloudsSettings.silverLiningIntensity = defaultProfile.cloudsSettings.silverLiningIntensity;
				profile.cloudsSettings.lightIntensity = defaultProfile.cloudsSettings.lightIntensity;
				profile.cloudsSettings.attenuationClamp = defaultProfile.cloudsSettings.attenuationClamp;
				profile.cloudsSettings.volumeCloudsAmbientColor = defaultProfile.cloudsSettings.volumeCloudsAmbientColor;
				profile.cloudsSettings.ParticleCloudsLayer1.height = 0.01f;
				profile.cloudsSettings.ParticleCloudsLayer2.height = 0.01f;
				profile.version = toV;
				return true;
			}
			return false;
		}
		else
		{
			if (!(fromV == "2.2.0") || !(toV == "2.2.1"))
			{
				return false;
			}
			if (defaultProfile != null)
			{
				profile.version = toV;
				return true;
			}
			return false;
		}
	}

	// Token: 0x0600023C RID: 572 RVA: 0x00018FC3 File Offset: 0x000171C3
	public static GameObject GetAssetPrefab(string name)
	{
		return null;
	}

	// Token: 0x0600023D RID: 573 RVA: 0x00018FC3 File Offset: 0x000171C3
	public static AudioClip GetAudioClip(string name)
	{
		return null;
	}

	// Token: 0x0600023E RID: 574 RVA: 0x00018FC3 File Offset: 0x000171C3
	public static Cubemap GetAssetCubemap(string name)
	{
		return null;
	}

	// Token: 0x0600023F RID: 575 RVA: 0x00018FC3 File Offset: 0x000171C3
	public static EnviroProfile GetDefaultProfile(string name)
	{
		return null;
	}

	// Token: 0x06000240 RID: 576 RVA: 0x00018FC3 File Offset: 0x000171C3
	public static Texture GetAssetTexture(string name)
	{
		return null;
	}

	// Token: 0x06000241 RID: 577 RVA: 0x00018FC8 File Offset: 0x000171C8
	public static Gradient CreateGradient(Color clr1, float time1, Color clr2, float time2)
	{
		Gradient gradient = new Gradient();
		GradientColorKey[] array = new GradientColorKey[2];
		GradientAlphaKey[] array2 = new GradientAlphaKey[2];
		array[0].color = clr1;
		array[0].time = time1;
		array[1].color = clr2;
		array[1].time = time2;
		array2[0].alpha = 1f;
		array2[0].time = 0f;
		array2[1].alpha = 1f;
		array2[1].time = 1f;
		gradient.SetKeys(array, array2);
		return gradient;
	}

	// Token: 0x06000242 RID: 578 RVA: 0x00019068 File Offset: 0x00017268
	public static Gradient CreateGradient(List<Color> clrs, List<float> times)
	{
		Gradient gradient = new Gradient();
		GradientColorKey[] array = new GradientColorKey[clrs.Count];
		GradientAlphaKey[] array2 = new GradientAlphaKey[2];
		for (int i = 0; i < clrs.Count; i++)
		{
			array[i].color = clrs[i];
			array[i].time = times[i];
		}
		array2[0].alpha = 1f;
		array2[0].time = 0f;
		array2[1].alpha = 1f;
		array2[1].time = 1f;
		gradient.SetKeys(array, array2);
		return gradient;
	}

	// Token: 0x06000243 RID: 579 RVA: 0x00019114 File Offset: 0x00017314
	public static Gradient CreateGradient(List<Color> clrs, List<float> times, List<float> alpha, List<float> timesAlpha)
	{
		Gradient gradient = new Gradient();
		GradientColorKey[] array = new GradientColorKey[clrs.Count];
		GradientAlphaKey[] array2 = new GradientAlphaKey[alpha.Count];
		for (int i = 0; i < clrs.Count; i++)
		{
			array[i].color = clrs[i];
			array[i].time = times[i];
		}
		for (int j = 0; j < alpha.Count; j++)
		{
			array2[j].alpha = alpha[j];
			array2[j].time = timesAlpha[j];
		}
		gradient.SetKeys(array, array2);
		return gradient;
	}

	// Token: 0x06000244 RID: 580 RVA: 0x000191C0 File Offset: 0x000173C0
	public static Color GetColor(string hex)
	{
		Color result = default(Color);
		ColorUtility.TryParseHtmlString(hex, out result);
		return result;
	}

	// Token: 0x06000245 RID: 581 RVA: 0x000191E0 File Offset: 0x000173E0
	public static Keyframe CreateKey(float value, float time)
	{
		return new Keyframe
		{
			value = value,
			time = time
		};
	}

	// Token: 0x06000246 RID: 582 RVA: 0x00019208 File Offset: 0x00017408
	public static Keyframe CreateKey(float value, float time, float inTangent, float outTangent)
	{
		return new Keyframe
		{
			value = value,
			time = time,
			inTangent = inTangent,
			outTangent = outTangent
		};
	}
}
