using System;

// Token: 0x0200008E RID: 142
[Serializable]
public class EnviroVegetationSeasons
{
	// Token: 0x040006CD RID: 1741
	public EnviroVegetationSeasons.SeasonAction seasonAction;

	// Token: 0x040006CE RID: 1742
	public bool GrowInSpring = true;

	// Token: 0x040006CF RID: 1743
	public bool GrowInSummer = true;

	// Token: 0x040006D0 RID: 1744
	public bool GrowInAutumn = true;

	// Token: 0x040006D1 RID: 1745
	public bool GrowInWinter = true;

	// Token: 0x02000393 RID: 915
	public enum SeasonAction
	{
		// Token: 0x04002747 RID: 10055
		SpawnDeadPrefab,
		// Token: 0x04002748 RID: 10056
		Deactivate,
		// Token: 0x04002749 RID: 10057
		Destroy
	}
}
