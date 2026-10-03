using System;

namespace TriangleNet.Geometry
{
	// Token: 0x0200023E RID: 574
	public interface IEdge
	{
		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000E74 RID: 3700
		int P0 { get; }

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000E75 RID: 3701
		int P1 { get; }

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000E76 RID: 3702
		int Label { get; }
	}
}
