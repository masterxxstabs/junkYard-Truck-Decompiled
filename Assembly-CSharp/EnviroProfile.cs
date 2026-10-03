using System;
using UnityEngine;

// Token: 0x02000088 RID: 136
[Serializable]
public class EnviroProfile : ScriptableObject
{
	// Token: 0x04000672 RID: 1650
	public string version;

	// Token: 0x04000673 RID: 1651
	public EnviroLightSettings lightSettings = new EnviroLightSettings();

	// Token: 0x04000674 RID: 1652
	public EnviroReflectionSettings reflectionSettings = new EnviroReflectionSettings();

	// Token: 0x04000675 RID: 1653
	public EnviroVolumeLightingSettings volumeLightSettings = new EnviroVolumeLightingSettings();

	// Token: 0x04000676 RID: 1654
	public EnviroDistanceBlurSettings distanceBlurSettings = new EnviroDistanceBlurSettings();

	// Token: 0x04000677 RID: 1655
	public EnviroSkySettings skySettings = new EnviroSkySettings();

	// Token: 0x04000678 RID: 1656
	public EnviroCloudSettings cloudsSettings = new EnviroCloudSettings();

	// Token: 0x04000679 RID: 1657
	public EnviroWeatherSettings weatherSettings = new EnviroWeatherSettings();

	// Token: 0x0400067A RID: 1658
	public EnviroFogSettings fogSettings = new EnviroFogSettings();

	// Token: 0x0400067B RID: 1659
	public EnviroLightShaftsSettings lightshaftsSettings = new EnviroLightShaftsSettings();

	// Token: 0x0400067C RID: 1660
	public EnviroSeasonSettings seasonsSettings = new EnviroSeasonSettings();

	// Token: 0x0400067D RID: 1661
	public EnviroAudioSettings audioSettings = new EnviroAudioSettings();

	// Token: 0x0400067E RID: 1662
	public EnviroSatellitesSettings satelliteSettings = new EnviroSatellitesSettings();

	// Token: 0x0400067F RID: 1663
	public EnviroQualitySettings qualitySettings = new EnviroQualitySettings();

	// Token: 0x04000680 RID: 1664
	public EnviroAuroraSettings auroraSettings = new EnviroAuroraSettings();

	// Token: 0x04000681 RID: 1665
	[HideInInspector]
	public EnviroProfile.settingsMode viewMode;

	// Token: 0x04000682 RID: 1666
	[HideInInspector]
	public EnviroProfile.settingsModeLW viewModeLW;

	// Token: 0x04000683 RID: 1667
	[HideInInspector]
	public bool showPlayerSetup = true;

	// Token: 0x04000684 RID: 1668
	[HideInInspector]
	public bool showRenderingSetup;

	// Token: 0x04000685 RID: 1669
	[HideInInspector]
	public bool showComponentsSetup;

	// Token: 0x04000686 RID: 1670
	[HideInInspector]
	public bool showTimeUI;

	// Token: 0x04000687 RID: 1671
	[HideInInspector]
	public bool showWeatherUI;

	// Token: 0x04000688 RID: 1672
	[HideInInspector]
	public bool showAudioUI;

	// Token: 0x04000689 RID: 1673
	[HideInInspector]
	public bool showEffectsUI;

	// Token: 0x0400068A RID: 1674
	[HideInInspector]
	public bool modified;

	// Token: 0x02000380 RID: 896
	public enum settingsMode
	{
		// Token: 0x04002710 RID: 10000
		Lighting,
		// Token: 0x04002711 RID: 10001
		Sky,
		// Token: 0x04002712 RID: 10002
		Reflections,
		// Token: 0x04002713 RID: 10003
		Weather,
		// Token: 0x04002714 RID: 10004
		Season,
		// Token: 0x04002715 RID: 10005
		Clouds,
		// Token: 0x04002716 RID: 10006
		Fog,
		// Token: 0x04002717 RID: 10007
		VolumeLighting,
		// Token: 0x04002718 RID: 10008
		Lightshafts,
		// Token: 0x04002719 RID: 10009
		DistanceBlur,
		// Token: 0x0400271A RID: 10010
		Aurora,
		// Token: 0x0400271B RID: 10011
		Satellites,
		// Token: 0x0400271C RID: 10012
		Audio,
		// Token: 0x0400271D RID: 10013
		Quality
	}

	// Token: 0x02000381 RID: 897
	public enum settingsModeLW
	{
		// Token: 0x0400271F RID: 10015
		Lighting,
		// Token: 0x04002720 RID: 10016
		Sky,
		// Token: 0x04002721 RID: 10017
		Reflections,
		// Token: 0x04002722 RID: 10018
		Weather,
		// Token: 0x04002723 RID: 10019
		Season,
		// Token: 0x04002724 RID: 10020
		Clouds,
		// Token: 0x04002725 RID: 10021
		Fog,
		// Token: 0x04002726 RID: 10022
		Lightshafts,
		// Token: 0x04002727 RID: 10023
		Satellites,
		// Token: 0x04002728 RID: 10024
		Audio,
		// Token: 0x04002729 RID: 10025
		Quality
	}
}
