using System;

// Token: 0x0200008D RID: 141
[Serializable]
public class EnviroVegetationAge
{
	// Token: 0x040006C5 RID: 1733
	public float maxAgeHours = 24f;

	// Token: 0x040006C6 RID: 1734
	public float maxAgeDays = 60f;

	// Token: 0x040006C7 RID: 1735
	public float maxAgeYears;

	// Token: 0x040006C8 RID: 1736
	public bool randomStartAge;

	// Token: 0x040006C9 RID: 1737
	public float startAgeinHours;

	// Token: 0x040006CA RID: 1738
	public double birthdayInHours;

	// Token: 0x040006CB RID: 1739
	public bool Loop = true;

	// Token: 0x040006CC RID: 1740
	public int LoopFromGrowStage;
}
