using System;

namespace TriangleNet.Geometry
{
	// Token: 0x0200023C RID: 572
	public class Edge : IEdge
	{
		// Token: 0x17000108 RID: 264
		// (get) Token: 0x06000E63 RID: 3683 RVA: 0x000B10F2 File Offset: 0x000AF2F2
		// (set) Token: 0x06000E64 RID: 3684 RVA: 0x000B10FA File Offset: 0x000AF2FA
		public int P0 { get; private set; }

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x06000E65 RID: 3685 RVA: 0x000B1103 File Offset: 0x000AF303
		// (set) Token: 0x06000E66 RID: 3686 RVA: 0x000B110B File Offset: 0x000AF30B
		public int P1 { get; private set; }

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x06000E67 RID: 3687 RVA: 0x000B1114 File Offset: 0x000AF314
		// (set) Token: 0x06000E68 RID: 3688 RVA: 0x000B111C File Offset: 0x000AF31C
		public int Label { get; private set; }

		// Token: 0x06000E69 RID: 3689 RVA: 0x000B1125 File Offset: 0x000AF325
		public Edge(int p0, int p1) : this(p0, p1, 0)
		{
		}

		// Token: 0x06000E6A RID: 3690 RVA: 0x000B1130 File Offset: 0x000AF330
		public Edge(int p0, int p1, int label)
		{
			this.P0 = p0;
			this.P1 = p1;
			this.Label = label;
		}
	}
}
