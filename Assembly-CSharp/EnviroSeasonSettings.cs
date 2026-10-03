using System;
using UnityEngine;

// Token: 0x0200007A RID: 122
[Serializable]
public class EnviroSeasonSettings
{
	// Token: 0x040005AA RID: 1450
	[Header("Spring")]
	[Tooltip("Start Day of Year for Spring")]
	[Range(0f, 366f)]
	public int SpringStart = 60;

	// Token: 0x040005AB RID: 1451
	[Tooltip("End Day of Year for Spring")]
	[Range(0f, 366f)]
	public int SpringEnd = 92;

	// Token: 0x040005AC RID: 1452
	[Tooltip("Base Temperature in Spring")]
	public AnimationCurve springBaseTemperature = new AnimationCurve();

	// Token: 0x040005AD RID: 1453
	[Header("Summer")]
	[Tooltip("Start Day of Year for Summer")]
	[Range(0f, 366f)]
	public int SummerStart = 93;

	// Token: 0x040005AE RID: 1454
	[Tooltip("End Day of Year for Summer")]
	[Range(0f, 366f)]
	public int SummerEnd = 185;

	// Token: 0x040005AF RID: 1455
	[Tooltip("Base Temperature in Summer")]
	public AnimationCurve summerBaseTemperature = new AnimationCurve();

	// Token: 0x040005B0 RID: 1456
	[Header("Autumn")]
	[Tooltip("Start Day of Year for Autumn")]
	[Range(0f, 366f)]
	public int AutumnStart = 186;

	// Token: 0x040005B1 RID: 1457
	[Tooltip("End Day of Year for Autumn")]
	[Range(0f, 366f)]
	public int AutumnEnd = 276;

	// Token: 0x040005B2 RID: 1458
	[Tooltip("Base Temperature in Autumn")]
	public AnimationCurve autumnBaseTemperature = new AnimationCurve();

	// Token: 0x040005B3 RID: 1459
	[Header("Winter")]
	[Tooltip("Start Day of Year for Winter")]
	[Range(0f, 366f)]
	public int WinterStart = 277;

	// Token: 0x040005B4 RID: 1460
	[Tooltip("End Day of Year for Winter")]
	[Range(0f, 366f)]
	public int WinterEnd = 59;

	// Token: 0x040005B5 RID: 1461
	[Tooltip("Base Temperature in Winter")]
	public AnimationCurve winterBaseTemperature = new AnimationCurve();
}
