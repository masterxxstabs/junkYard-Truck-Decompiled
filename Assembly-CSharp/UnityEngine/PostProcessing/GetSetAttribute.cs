using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001BF RID: 447
	public sealed class GetSetAttribute : PropertyAttribute
	{
		// Token: 0x06000ACB RID: 2763 RVA: 0x0008F99F File Offset: 0x0008DB9F
		public GetSetAttribute(string name)
		{
			this.name = name;
		}

		// Token: 0x04001D39 RID: 7481
		public readonly string name;

		// Token: 0x04001D3A RID: 7482
		public bool dirty;
	}
}
