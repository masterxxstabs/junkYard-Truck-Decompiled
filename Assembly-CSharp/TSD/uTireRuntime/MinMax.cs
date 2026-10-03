using System;

namespace TSD.uTireRuntime
{
	// Token: 0x02000349 RID: 841
	[Serializable]
	public class MinMax
	{
		// Token: 0x0600158C RID: 5516 RVA: 0x000E0BA2 File Offset: 0x000DEDA2
		public MinMax(float m_min, float m_max)
		{
			this.min = m_min;
			this.max = m_max;
		}

		// Token: 0x0400262D RID: 9773
		public float min;

		// Token: 0x0400262E RID: 9774
		public float max;
	}
}
