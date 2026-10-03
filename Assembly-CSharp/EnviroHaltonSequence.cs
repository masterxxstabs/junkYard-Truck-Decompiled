using System;

// Token: 0x020000A4 RID: 164
public class EnviroHaltonSequence
{
	// Token: 0x060003CF RID: 975 RVA: 0x00025998 File Offset: 0x00023B98
	public float Get()
	{
		float num = 0f;
		float num2 = 1f / (float)this.radix;
		int i = this.storedIndex;
		while (i > 0)
		{
			num += (float)(i % this.radix) * num2;
			i /= this.radix;
			num2 /= (float)this.radix;
		}
		this.storedIndex++;
		return num;
	}

	// Token: 0x040007D4 RID: 2004
	public int radix = 3;

	// Token: 0x040007D5 RID: 2005
	private int storedIndex;
}
