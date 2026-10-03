using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Meshing
{
	// Token: 0x02000220 RID: 544
	public interface IMesh
	{
		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000DAB RID: 3499
		ICollection<Vertex> Vertices { get; }

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000DAC RID: 3500
		IEnumerable<Edge> Edges { get; }

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06000DAD RID: 3501
		ICollection<SubSegment> Segments { get; }

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06000DAE RID: 3502
		ICollection<Triangle> Triangles { get; }

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x06000DAF RID: 3503
		IList<Point> Holes { get; }

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000DB0 RID: 3504
		Rectangle Bounds { get; }

		// Token: 0x06000DB1 RID: 3505
		void Renumber();

		// Token: 0x06000DB2 RID: 3506
		void Refine(QualityOptions quality, bool delaunay);
	}
}
