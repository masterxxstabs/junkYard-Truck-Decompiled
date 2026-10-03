using System;
using UnityEngine;

// Token: 0x0200007B RID: 123
[Serializable]
public class EnviroWeatherSettings
{
	// Token: 0x040005B6 RID: 1462
	[Header("Zones Setup:")]
	[Tooltip("Tag for zone triggers. Create and assign a tag to this gameObject")]
	public bool useTag;

	// Token: 0x040005B7 RID: 1463
	[Header("Weather Transition Settings:")]
	[Tooltip("Defines the speed of wetness will raise when it is raining.")]
	public float wetnessAccumulationSpeed = 0.05f;

	// Token: 0x040005B8 RID: 1464
	[Tooltip("Defines the speed of wetness will dry when it is not raining.")]
	public float wetnessDryingSpeed = 0.05f;

	// Token: 0x040005B9 RID: 1465
	[Tooltip("Defines the speed of snow will raise when it is snowing.")]
	public float snowAccumulationSpeed = 0.05f;

	// Token: 0x040005BA RID: 1466
	[Tooltip("Defines the speed of snow will meld when it is not snowing.")]
	public float snowMeltingSpeed = 0.05f;

	// Token: 0x040005BB RID: 1467
	[Tooltip("Defines the temperature when snow starts to melt.")]
	public float snowMeltingTresholdTemperature = 1f;

	// Token: 0x040005BC RID: 1468
	[Tooltip("Defines the speed of clouds will change when weather conditions changed.")]
	public float cloudTransitionSpeed = 1f;

	// Token: 0x040005BD RID: 1469
	[Tooltip("Defines the speed of fog will change when weather conditions changed.")]
	public float fogTransitionSpeed = 1f;

	// Token: 0x040005BE RID: 1470
	[Tooltip("Defines the speed of wind intensity will change when weather conditions changed.")]
	public float windIntensityTransitionSpeed = 1f;

	// Token: 0x040005BF RID: 1471
	[Tooltip("Defines the speed of particle effects will change when weather conditions changed.")]
	public float effectTransitionSpeed = 1f;

	// Token: 0x040005C0 RID: 1472
	[Tooltip("Defines the speed of sfx will fade in and out when weather conditions changed.")]
	public float audioTransitionSpeed = 0.1f;

	// Token: 0x040005C1 RID: 1473
	[Header("Lightning Effect:")]
	public GameObject lightningEffect;

	// Token: 0x040005C2 RID: 1474
	[Range(500f, 10000f)]
	public float lightningRange = 500f;

	// Token: 0x040005C3 RID: 1475
	[Range(500f, 5000f)]
	public float lightningHeight = 750f;

	// Token: 0x040005C4 RID: 1476
	[Header("Temperature:")]
	[Tooltip("Defines the speed of temperature changes.")]
	public float temperatureChangingSpeed = 10f;
}
