using System;

namespace TriangleNet.Geometry
{
	// Token: 0x02000241 RID: 577
	public interface ITriangle
	{
		// Token: 0x17000114 RID: 276
		// (get) Token: 0x06000E89 RID: 3721
		// (set) Token: 0x06000E8A RID: 3722
		int ID { get; set; }

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x06000E8B RID: 3723
		// (set) Token: 0x06000E8C RID: 3724
		int Label { get; set; }

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x06000E8D RID: 3725
		// (set) Token: 0x06000E8E RID: 3726
		double Area { get; set; }

		// Token: 0x06000E8F RID: 3727
		Vertex GetVertex(int index);

		// Token: 0x06000E90 RID: 3728
		int GetVertexID(int index);

		// Token: 0x06000E91 RID: 3729
		ITriangle GetNeighbor(int index);

		// Token: 0x06000E92 RID: 3730
		int GetNeighborID(int index);

		// Token: 0x06000E93 RID: 3731
		ISegment GetSegment(int index);
	}
}
