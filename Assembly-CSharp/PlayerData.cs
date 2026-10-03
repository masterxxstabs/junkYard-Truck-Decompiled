using System;

// Token: 0x02000115 RID: 277
[Serializable]
public class PlayerData
{
	// Token: 0x06000747 RID: 1863 RVA: 0x0005EA3B File Offset: 0x0005CC3B
	public PlayerData(Currency currency)
	{
		this.water = currency.water;
		this.money = currency.money;
	}

	// Token: 0x04001048 RID: 4168
	public float water;

	// Token: 0x04001049 RID: 4169
	public float money;

	// Token: 0x0400104A RID: 4170
	public float[] position;
}
