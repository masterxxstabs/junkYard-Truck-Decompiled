using System;
using System.Collections.Generic;
using TriangleNet.Geometry;

namespace TriangleNet.Voronoi.Legacy
{
	// Token: 0x02000204 RID: 516
	public interface IVoronoi
	{
		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000C95 RID: 3221
		Point[] Points { get; }

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000C96 RID: 3222
		ICollection<VoronoiRegion> Regions { get; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000C97 RID: 3223
		IEnumerable<IEdge> Edges { get; }
	}
}
