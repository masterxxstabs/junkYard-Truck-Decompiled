using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001C0 RID: 448
	public sealed class MinAttribute : PropertyAttribute
	{
		// Token: 0x06000ACC RID: 2764 RVA: 0x0008F9AE File Offset: 0x0008DBAE
		public MinAttribute(float min)
		{
			this.min = min;
		}

		// Token: 0x04001D3B RID: 7483
		public readonly float min;
	}
}
