using System;
using UnityEngine;

// Token: 0x02000071 RID: 113
[Serializable]
public class EnviroTime
{
	// Token: 0x0400050A RID: 1290
	[Tooltip("None = No time auto time progressing, Simulated = Time calculated with DayLenghtInMinutes, SystemTime = uses your systemTime.")]
	public EnviroTime.TimeProgressMode ProgressTime = EnviroTime.TimeProgressMode.Simulated;

	// Token: 0x0400050B RID: 1291
	[Tooltip("Current Time: minutes")]
	[Range(0f, 60f)]
	public int Seconds;

	// Token: 0x0400050C RID: 1292
	[Tooltip("Current Time: minutes")]
	[Range(0f, 60f)]
	public int Minutes;

	// Token: 0x0400050D RID: 1293
	[Tooltip("Current Time: hours")]
	[Range(0f, 24f)]
	public int Hours = 12;

	// Token: 0x0400050E RID: 1294
	[Tooltip("Current Time: Days")]
	public int Days = 1;

	// Token: 0x0400050F RID: 1295
	[Tooltip("Current Time: Years")]
	public int Years = 1;

	// Token: 0x04000510 RID: 1296
	[Space(20f)]
	[Tooltip("How many days in one year?")]
	public int DaysInYear = 365;

	// Token: 0x04000511 RID: 1297
	[Tooltip("Day lenght in realtime minutes.")]
	public float DayLengthInMinutes = 5f;

	// Token: 0x04000512 RID: 1298
	[Tooltip("Night lenght in realtime minutes.")]
	public float NightLengthInMinutes = 5f;

	// Token: 0x04000513 RID: 1299
	[Range(-13f, 13f)]
	[Tooltip("Time offset for timezones")]
	public int utcOffset;

	// Token: 0x04000514 RID: 1300
	[Range(-90f, 90f)]
	[Tooltip("-90,  90   Horizontal earth lines")]
	public float Latitude;

	// Token: 0x04000515 RID: 1301
	[Range(-180f, 180f)]
	[Tooltip("-180, 180  Vertical earth line")]
	public float Longitude;

	// Token: 0x04000516 RID: 1302
	[HideInInspector]
	public float solarTime;

	// Token: 0x04000517 RID: 1303
	[HideInInspector]
	public float lunarTime;

	// Token: 0x04000518 RID: 1304
	[Tooltip("This setting will change the timing when system switches from day to night and night to day. It uses the sun position in sky: 0 -> Night, ~0.5 -> Dawn/Dusk, 1 -> Midday.")]
	[Range(0.3f, 0.7f)]
	public float dayNightSwitch = 0.45f;

	// Token: 0x02000374 RID: 884
	public enum TimeProgressMode
	{
		// Token: 0x040026E0 RID: 9952
		None,
		// Token: 0x040026E1 RID: 9953
		Simulated,
		// Token: 0x040026E2 RID: 9954
		OneDay,
		// Token: 0x040026E3 RID: 9955
		SystemTime
	}
}
