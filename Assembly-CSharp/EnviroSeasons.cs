using System;
using UnityEngine;

// Token: 0x0200006C RID: 108
[Serializable]
public class EnviroSeasons
{
	// Token: 0x040004EB RID: 1259
	[Tooltip("When enabled the system will change seasons automaticly when enough days passed.")]
	public bool calcSeasons;

	// Token: 0x040004EC RID: 1260
	[Tooltip("The current season.")]
	public EnviroSeasons.Seasons currentSeasons;

	// Token: 0x040004ED RID: 1261
	[HideInInspector]
	public EnviroSeasons.Seasons lastSeason;

	// Token: 0x02000373 RID: 883
	public enum Seasons
	{
		// Token: 0x040026DB RID: 9947
		Spring,
		// Token: 0x040026DC RID: 9948
		Summer,
		// Token: 0x040026DD RID: 9949
		Autumn,
		// Token: 0x040026DE RID: 9950
		Winter
	}
}
