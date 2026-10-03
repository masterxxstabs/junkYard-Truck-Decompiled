using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001C1 RID: 449
	public sealed class TrackballAttribute : PropertyAttribute
	{
		// Token: 0x06000ACD RID: 2765 RVA: 0x0008F9BD File Offset: 0x0008DBBD
		public TrackballAttribute(string method)
		{
			this.method = method;
		}

		// Token: 0x04001D3C RID: 7484
		public readonly string method;
	}
}
