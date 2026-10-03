using System;
using UnityEngine;

// Token: 0x02000085 RID: 133
[Serializable]
public class EnviroCloudSettings
{
	// Token: 0x04000639 RID: 1593
	public EnviroVolumeCloudsQualitySettings cloudsQualitySettings;

	// Token: 0x0400063A RID: 1594
	[Range(10000f, 486000f)]
	[Tooltip("Clouds world scale. This settings will influece rendering of clouds at horizon.")]
	public float cloudsWorldScale = 113081f;

	// Token: 0x0400063B RID: 1595
	[Tooltip("Change Clouds Height.")]
	[Range(-2000f, 2000f)]
	public float cloudsHeightMod;

	// Token: 0x0400063C RID: 1596
	[Tooltip("Enable this option to blend clouds with your scene.")]
	public bool depthBlending;

	// Token: 0x0400063D RID: 1597
	[Tooltip("Use this option to minimize blending artifacts of downsampled clouds with full resolution scene.")]
	public bool bilateralUpsampling;

	// Token: 0x0400063E RID: 1598
	[Header("Clouds Wind Animation")]
	public bool useWindZoneDirection;

	// Token: 0x0400063F RID: 1599
	[Range(-1f, 1f)]
	[Tooltip("Time scale / wind animation speed of clouds.")]
	public float cloudsTimeScale = 1f;

	// Token: 0x04000640 RID: 1600
	[Range(0f, 1f)]
	[Tooltip("Global clouds wind speed modificator.")]
	public float cloudsWindIntensity = 0.001f;

	// Token: 0x04000641 RID: 1601
	[Range(0f, 1f)]
	[Tooltip("Global clouds upwards wind speed modificator.")]
	public float cloudsUpwardsWindIntensity = 0.001f;

	// Token: 0x04000642 RID: 1602
	[Range(0f, 1f)]
	[Tooltip("Cirrus clouds wind speed modificator.")]
	public float cirrusWindIntensity = 0.001f;

	// Token: 0x04000643 RID: 1603
	[Range(-1f, 1f)]
	[Tooltip("Global clouds wind direction X axes.")]
	public float cloudsWindDirectionX = 1f;

	// Token: 0x04000644 RID: 1604
	[Range(-1f, 1f)]
	[Tooltip("Global clouds wind direction Y axes.")]
	public float cloudsWindDirectionY = 1f;

	// Token: 0x04000645 RID: 1605
	[Header("Clouds Lighting")]
	[Tooltip("Clamps directional shadows on clouds.")]
	public AnimationCurve attenuationClamp = new AnimationCurve();

	// Token: 0x04000646 RID: 1606
	[Tooltip("Sun highlight in near of sun.")]
	[Range(0.01f, 1f)]
	public float hgPhase = 0.5f;

	// Token: 0x04000647 RID: 1607
	[Range(0.01f, 1f)]
	[Tooltip("SilverLining intensity away from sun. Evaluated based on sun position. Keep between 0-1 range!")]
	public float silverLiningIntensity = 0.5f;

	// Token: 0x04000648 RID: 1608
	[Tooltip("SilverLining spread away from sun. Evaluated based on sun position. Keep between 0-1 range!")]
	public AnimationCurve silverLiningSpread = new AnimationCurve();

	// Token: 0x04000649 RID: 1609
	[Tooltip("Global Color for volume clouds based sun positon.")]
	public Gradient volumeCloudsColor = new Gradient();

	// Token: 0x0400064A RID: 1610
	[Tooltip("Global Color for clouds based moon positon.")]
	public Gradient volumeCloudsMoonColor = new Gradient();

	// Token: 0x0400064B RID: 1611
	[Tooltip("Global ambient color add for volume clouds based sun positon.")]
	public Gradient volumeCloudsAmbientColor = new Gradient();

	// Token: 0x0400064C RID: 1612
	[Tooltip("Raie or lower the light intensity based on sun altitude.")]
	public AnimationCurve lightIntensity = new AnimationCurve();

	// Token: 0x0400064D RID: 1613
	[Tooltip("Tweak the ambient lighting from sky based on sun altitude.")]
	public AnimationCurve ambientLightIntensity = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x0400064E RID: 1614
	[Header("Tonemapping")]
	[Tooltip("Use color tonemapping?")]
	public bool tonemapping;

	// Token: 0x0400064F RID: 1615
	[Tooltip("Tonemapping exposure")]
	public float cloudsExposure = 1f;

	// Token: 0x04000650 RID: 1616
	[Tooltip("Use Halton Sequence based raymarching offset to help with undersampling raymarching. This option will make clouds more noisy, so use it with TAA only.")]
	public bool useHaltonRaymarchOffset;

	// Token: 0x04000651 RID: 1617
	[Tooltip("Enable this to only use half the amount of raymarching steps when using the halton sequence offset together with TAA.")]
	public bool useLessSteps;

	// Token: 0x04000652 RID: 1618
	[Header("Weather Map")]
	[Tooltip("Tiling of the generated weather map.")]
	public int weatherMapTiling = 5;

	// Token: 0x04000653 RID: 1619
	[Tooltip("Option to add own weather map. Red Channel = Coverage, Blue = Clouds Height")]
	public Texture2D customWeatherMap;

	// Token: 0x04000654 RID: 1620
	[Tooltip("Weathermap sampling offset.")]
	public Vector2 locationOffset;

	// Token: 0x04000655 RID: 1621
	[Range(0f, 1f)]
	[Tooltip("Weathermap animation speed.")]
	public float weatherAnimSpeedScale = 0.33f;

	// Token: 0x04000656 RID: 1622
	[Header("Global Clouds Control")]
	[Range(0f, 2f)]
	public float globalCloudCoverage = 1f;

	// Token: 0x04000657 RID: 1623
	[Tooltip("Texture for cirrus clouds.")]
	public Texture cirrusCloudsTexture;

	// Token: 0x04000658 RID: 1624
	[Tooltip("Global Color for flat clouds based sun positon.")]
	public Gradient cirrusCloudsColor;

	// Token: 0x04000659 RID: 1625
	[Range(5f, 15f)]
	[Tooltip("Flat Clouds Altitude")]
	public float cirrusCloudsAltitude = 10f;

	// Token: 0x0400065A RID: 1626
	[Tooltip("Texture for flat procedural clouds.")]
	public Texture flatCloudsNoiseTexture;

	// Token: 0x0400065B RID: 1627
	[Tooltip("Resolution of generated flat clouds texture.")]
	public EnviroCloudSettings.FlatCloudResolution flatCloudsResolution = EnviroCloudSettings.FlatCloudResolution.R2048;

	// Token: 0x0400065C RID: 1628
	[Tooltip("Global Color for flat clouds based sun positon.")]
	public Gradient flatCloudsColor;

	// Token: 0x0400065D RID: 1629
	[Tooltip("Scale/Tiling of flat clouds.")]
	public float flatCloudsScale = 2f;

	// Token: 0x0400065E RID: 1630
	[Range(1f, 12f)]
	[Tooltip("Flat Clouds texture generation iterations.")]
	public int flatCloudsNoiseOctaves = 6;

	// Token: 0x0400065F RID: 1631
	[Range(30f, 100f)]
	[Tooltip("Flat Clouds Altitude")]
	public float flatCloudsAltitude = 70f;

	// Token: 0x04000660 RID: 1632
	[Range(0.01f, 1f)]
	[Tooltip("Flat Clouds morphing animation speed.")]
	public float flatCloudsMorphingSpeed = 0.2f;

	// Token: 0x04000661 RID: 1633
	[Tooltip("Clouds Shadowcast Intensity. 0 = disabled")]
	[Range(0f, 1f)]
	public float shadowIntensity;

	// Token: 0x04000662 RID: 1634
	[Tooltip("Size of the shadow cookie.")]
	[Range(100f, 100000f)]
	public int shadowCookieSize = 100000;

	// Token: 0x04000663 RID: 1635
	public EnviroParticleClouds ParticleCloudsLayer1 = new EnviroParticleClouds();

	// Token: 0x04000664 RID: 1636
	public EnviroParticleClouds ParticleCloudsLayer2 = new EnviroParticleClouds();

	// Token: 0x0200037F RID: 895
	public enum FlatCloudResolution
	{
		// Token: 0x0400270B RID: 9995
		R512,
		// Token: 0x0400270C RID: 9996
		R1024,
		// Token: 0x0400270D RID: 9997
		R2048,
		// Token: 0x0400270E RID: 9998
		R4096
	}
}
