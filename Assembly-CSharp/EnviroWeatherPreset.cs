using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000093 RID: 147
[Serializable]
public class EnviroWeatherPreset : ScriptableObject
{
	// Token: 0x04000707 RID: 1799
	public string version;

	// Token: 0x04000708 RID: 1800
	public string Name;

	// Token: 0x04000709 RID: 1801
	[Header("Season Settings")]
	public bool Spring = true;

	// Token: 0x0400070A RID: 1802
	[Range(1f, 100f)]
	public float possibiltyInSpring = 50f;

	// Token: 0x0400070B RID: 1803
	public bool Summer = true;

	// Token: 0x0400070C RID: 1804
	[Range(1f, 100f)]
	public float possibiltyInSummer = 50f;

	// Token: 0x0400070D RID: 1805
	public bool Autumn = true;

	// Token: 0x0400070E RID: 1806
	[Range(1f, 100f)]
	public float possibiltyInAutumn = 50f;

	// Token: 0x0400070F RID: 1807
	public bool winter = true;

	// Token: 0x04000710 RID: 1808
	[Range(1f, 100f)]
	public float possibiltyInWinter = 50f;

	// Token: 0x04000711 RID: 1809
	[Header("Cloud Settings")]
	public EnviroWeatherCloudsConfig cloudsConfig;

	// Token: 0x04000712 RID: 1810
	[Header("Linear Fog")]
	public float fogStartDistance;

	// Token: 0x04000713 RID: 1811
	public float fogDistance = 1000f;

	// Token: 0x04000714 RID: 1812
	[Header("Exp Fog")]
	public float fogDensity = 0.0001f;

	// Token: 0x04000715 RID: 1813
	[Tooltip("Used to modify sky, direct, ambient light and fog color. The color alpha value defines the intensity")]
	public Gradient weatherSkyMod;

	// Token: 0x04000716 RID: 1814
	public Gradient weatherLightMod;

	// Token: 0x04000717 RID: 1815
	public Gradient weatherFogMod;

	// Token: 0x04000718 RID: 1816
	[Range(0f, 2f)]
	public float volumeLightIntensity = 1f;

	// Token: 0x04000719 RID: 1817
	[Range(-1f, 1f)]
	public float shadowIntensityMod;

	// Token: 0x0400071A RID: 1818
	[Range(0f, 100f)]
	[Tooltip("The density of height based fog for this weather.")]
	public float heightFogDensity = 1f;

	// Token: 0x0400071B RID: 1819
	[Range(0f, 2f)]
	[Tooltip("Define the height of fog rendered in sky.")]
	public float SkyFogHeight = 0.5f;

	// Token: 0x0400071C RID: 1820
	[Range(0f, 1f)]
	[Tooltip("Define the start height of fog rendered in sky.")]
	public float skyFogStart;

	// Token: 0x0400071D RID: 1821
	[Tooltip("Define the intensity of fog rendered in sky.")]
	[Range(0f, 2f)]
	public float SkyFogIntensity = 1f;

	// Token: 0x0400071E RID: 1822
	[Range(1f, 10f)]
	[Tooltip("Define the scattering intensity of fog.")]
	public float FogScatteringIntensity = 1f;

	// Token: 0x0400071F RID: 1823
	[Range(0f, 1f)]
	[Tooltip("Block the sundisk with fog.")]
	public float fogSunBlocking = 0.25f;

	// Token: 0x04000720 RID: 1824
	[Range(0f, 1f)]
	[Tooltip("Block the moon with fog.")]
	public float moonIntensity = 1f;

	// Token: 0x04000721 RID: 1825
	[Header("Weather Settings")]
	public List<EnviroWeatherEffects> effectSystems = new List<EnviroWeatherEffects>();

	// Token: 0x04000722 RID: 1826
	[Range(0f, 1f)]
	[Tooltip("Wind intensity that will applied to wind zone.")]
	public float WindStrenght = 0.5f;

	// Token: 0x04000723 RID: 1827
	[Range(0f, 1f)]
	[Tooltip("The maximum wetness level that can be reached.")]
	public float wetnessLevel;

	// Token: 0x04000724 RID: 1828
	[Range(0f, 1f)]
	[Tooltip("The maximum snow level that can be reached.")]
	public float snowLevel;

	// Token: 0x04000725 RID: 1829
	[Range(-50f, 50f)]
	[Tooltip("The temperature modifcation for this weather type. (Will be added or substracted)")]
	public float temperatureLevel;

	// Token: 0x04000726 RID: 1830
	[Tooltip("Activate this to enable thunder and lightning.")]
	public bool isLightningStorm;

	// Token: 0x04000727 RID: 1831
	[Range(0f, 2f)]
	[Tooltip("The Intervall of lightning in seconds. Random(lightningInterval,lightningInterval * 2). ")]
	public float lightningInterval = 10f;

	// Token: 0x04000728 RID: 1832
	[Header("Aurora Settings")]
	[Range(0f, 1f)]
	public float auroraIntensity;

	// Token: 0x04000729 RID: 1833
	[Header("Audio Settings - SFX")]
	[Tooltip("Define an sound effect for this weather preset.")]
	public AudioClip weatherSFX;

	// Token: 0x0400072A RID: 1834
	[Header("Audio Settings - Ambient")]
	[Tooltip("This sound wil be played in spring at day.(looped)")]
	public AudioClip SpringDayAmbient;

	// Token: 0x0400072B RID: 1835
	[Tooltip("This sound wil be played in spring at night.(looped)")]
	public AudioClip SpringNightAmbient;

	// Token: 0x0400072C RID: 1836
	[Tooltip("This sound wil be played in summer at day.(looped)")]
	public AudioClip SummerDayAmbient;

	// Token: 0x0400072D RID: 1837
	[Tooltip("This sound wil be played in summer at night.(looped)")]
	public AudioClip SummerNightAmbient;

	// Token: 0x0400072E RID: 1838
	[Tooltip("This sound wil be played in autumn at day.(looped)")]
	public AudioClip AutumnDayAmbient;

	// Token: 0x0400072F RID: 1839
	[Tooltip("This sound wil be played in autumn at night.(looped)")]
	public AudioClip AutumnNightAmbient;

	// Token: 0x04000730 RID: 1840
	[Tooltip("This sound wil be played in winter at day.(looped)")]
	public AudioClip WinterDayAmbient;

	// Token: 0x04000731 RID: 1841
	[Tooltip("This sound wil be played in winter at night.(looped)")]
	public AudioClip WinterNightAmbient;

	// Token: 0x04000732 RID: 1842
	public float blurDistance = 100f;

	// Token: 0x04000733 RID: 1843
	public float blurIntensity = 1f;

	// Token: 0x04000734 RID: 1844
	public float blurSkyIntensity = 1f;
}
