using System;
using System.Collections.Generic;
using TriangleNet.Geometry;

namespace TriangleNet.Meshing
{
	// Token: 0x02000222 RID: 546
	public interface ITriangulator
	{
		// Token: 0x06000DB5 RID: 3509
		IMesh Triangulate(IList<Vertex> points, Configuration config);
	}
}
