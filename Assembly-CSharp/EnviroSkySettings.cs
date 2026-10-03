using System;
using UnityEngine;

// Token: 0x0200007C RID: 124
[Serializable]
public class EnviroSkySettings
{
	// Token: 0x040005C5 RID: 1477
	[Header("Sky Mode:")]
	[Tooltip("Select if you want to use enviro skybox your custom material.")]
	public EnviroSkySettings.SkyboxModi skyboxMode;

	// Token: 0x040005C6 RID: 1478
	[Tooltip("Select if you want to use enviro skybox your custom material.")]
	public EnviroSkySettings.SkyboxModiLW skyboxModeLW;

	// Token: 0x040005C7 RID: 1479
	[Tooltip("If SkyboxMode == CustomSkybox : Assign your skybox material here!")]
	public Material customSkyboxMaterial;

	// Token: 0x040005C8 RID: 1480
	[Tooltip("If SkyboxMode == CustomColor : Select your sky color here!")]
	public Color customSkyboxColor;

	// Token: 0x040005C9 RID: 1481
	[Tooltip("Enable to render black skybox at ground level.")]
	public bool blackGroundMode;

	// Token: 0x040005CA RID: 1482
	[Header("Scattering")]
	[Tooltip("Light Wavelength used for atmospheric scattering. Keep it near defaults for earthlike atmospheres, or change for alien or fantasy atmospheres for example.")]
	public Vector3 waveLength = new Vector3(540f, 496f, 437f);

	// Token: 0x040005CB RID: 1483
	[Tooltip("Influence atmospheric scattering.")]
	public float rayleigh = 5.15f;

	// Token: 0x040005CC RID: 1484
	[Tooltip("Sky turbidity. Particle in air. Influence atmospheric scattering.")]
	public float turbidity = 1f;

	// Token: 0x040005CD RID: 1485
	[Tooltip("Influence scattering near sun.")]
	public float mie = 5f;

	// Token: 0x040005CE RID: 1486
	[Tooltip("Influence scattering near sun.")]
	public float g = 0.8f;

	// Token: 0x040005CF RID: 1487
	[Tooltip("Intensity gradient for atmospheric scattering. Influence atmospheric scattering based on current sun altitude.")]
	public AnimationCurve scatteringCurve = new AnimationCurve();

	// Token: 0x040005D0 RID: 1488
	[Tooltip("Color gradient for atmospheric scattering. Influence atmospheric scattering based on current sun altitude.")]
	public Gradient scatteringColor;

	// Token: 0x040005D1 RID: 1489
	[Header("Sun")]
	public EnviroSkySettings.SunAndMoonCalc sunAndMoonPosition = EnviroSkySettings.SunAndMoonCalc.Realistic;

	// Token: 0x040005D2 RID: 1490
	[Tooltip("Intensity of Sun Influence Scale and Dropoff of sundisk.")]
	public float sunIntensity = 100f;

	// Token: 0x040005D3 RID: 1491
	[Tooltip("Scale of rendered sundisk.")]
	public float sunDiskScale = 20f;

	// Token: 0x040005D4 RID: 1492
	[Tooltip("Intenisty of rendered sundisk.")]
	public float sunDiskIntensity = 3f;

	// Token: 0x040005D5 RID: 1493
	[Tooltip("Color gradient for sundisk. Influence sundisk color based on current sun altitude")]
	public Gradient sunDiskColor;

	// Token: 0x040005D6 RID: 1494
	[Tooltip("Top color of simple skybox.")]
	public Gradient simpleSkyColor;

	// Token: 0x040005D7 RID: 1495
	[Tooltip("Horizon color of simple skybox.")]
	public Gradient simpleHorizonColor;

	// Token: 0x040005D8 RID: 1496
	[Tooltip("Sun color of simple skybox.")]
	public Gradient simpleSunColor;

	// Token: 0x040005D9 RID: 1497
	[Tooltip("Size of sun in simple skybox mode.")]
	public AnimationCurve simpleSunDiskSize = new AnimationCurve();

	// Token: 0x040005DA RID: 1498
	[Header("Moon")]
	[Tooltip("Whether to render the moon.")]
	public bool renderMoon = true;

	// Token: 0x040005DB RID: 1499
	[Tooltip("The Moon phase mode. Custom = for customizable phase.")]
	public EnviroSkySettings.MoonPhases moonPhaseMode = EnviroSkySettings.MoonPhases.Realistic;

	// Token: 0x040005DC RID: 1500
	[Tooltip("The Moon texture.")]
	public Texture moonTexture;

	// Token: 0x040005DD RID: 1501
	[Tooltip("The Moon's Glow texture.")]
	public Texture glowTexture;

	// Token: 0x040005DE RID: 1502
	[Tooltip("The color of the moon")]
	public Color moonColor;

	// Token: 0x040005DF RID: 1503
	[Range(0f, 5f)]
	[Tooltip("Brightness of the moon.")]
	public float moonBrightness = 1f;

	// Token: 0x040005E0 RID: 1504
	[Range(0f, 20f)]
	[Tooltip("Size of the moon.")]
	public float moonSize = 10f;

	// Token: 0x040005E1 RID: 1505
	[Range(0f, 20f)]
	[Tooltip("Size of the moon glowing effect.")]
	public float glowSize = 10f;

	// Token: 0x040005E2 RID: 1506
	[Tooltip("Glow around moon.")]
	public AnimationCurve moonGlow = new AnimationCurve();

	// Token: 0x040005E3 RID: 1507
	[Tooltip("Glow color around moon.")]
	public Color moonGlowColor;

	// Token: 0x040005E4 RID: 1508
	[Tooltip("Start moon phase when using custom phase mode.(-1f - 1f)")]
	[Range(-1f, 1f)]
	public float startMoonPhase;

	// Token: 0x040005E5 RID: 1509
	[Header("Sky Color Corrections")]
	[Tooltip("Higher values = brighter sky.")]
	public AnimationCurve skyLuminence = new AnimationCurve();

	// Token: 0x040005E6 RID: 1510
	[Tooltip("Higher values = stronger colors applied BEFORE clouds rendered!")]
	public AnimationCurve skyColorPower = new AnimationCurve();

	// Token: 0x040005E7 RID: 1511
	[Header("Tonemapping - LDR")]
	[Tooltip("Tonemapping when using LDR")]
	public float skyExposure = 1.5f;

	// Token: 0x040005E8 RID: 1512
	[Header("Stars")]
	[Tooltip("A cubemap for night sky.")]
	public Cubemap starsCubeMap;

	// Token: 0x040005E9 RID: 1513
	[Tooltip("Intensity of stars based on time of day.")]
	public AnimationCurve starsIntensity = new AnimationCurve();

	// Token: 0x040005EA RID: 1514
	[Tooltip("Stars Twinkling Speed")]
	[Range(0f, 10f)]
	public float starsTwinklingRate = 1f;

	// Token: 0x040005EB RID: 1515
	[Header("Galaxy")]
	[Tooltip("A cubemap for night galaxy.")]
	public Cubemap galaxyCubeMap;

	// Token: 0x040005EC RID: 1516
	[Tooltip("Intensity of galaxy based on time of day.")]
	public AnimationCurve galaxyIntensity = new AnimationCurve();

	// Token: 0x040005ED RID: 1517
	[Header("Sky Dithering")]
	public bool dithering = true;

	// Token: 0x02000379 RID: 889
	public enum SunAndMoonCalc
	{
		// Token: 0x040026F0 RID: 9968
		Simple,
		// Token: 0x040026F1 RID: 9969
		Realistic
	}

	// Token: 0x0200037A RID: 890
	public enum MoonPhases
	{
		// Token: 0x040026F3 RID: 9971
		Custom,
		// Token: 0x040026F4 RID: 9972
		Realistic
	}

	// Token: 0x0200037B RID: 891
	public enum SkyboxModi
	{
		// Token: 0x040026F6 RID: 9974
		Default,
		// Token: 0x040026F7 RID: 9975
		Simple,
		// Token: 0x040026F8 RID: 9976
		CustomSkybox,
		// Token: 0x040026F9 RID: 9977
		CustomColor
	}

	// Token: 0x0200037C RID: 892
	public enum SkyboxModiLW
	{
		// Token: 0x040026FB RID: 9979
		Simple,
		// Token: 0x040026FC RID: 9980
		CustomSkybox,
		// Token: 0x040026FD RID: 9981
		CustomColor
	}
}
