using System;
using TriangleNet.Geometry;
using TriangleNet.Topology.DCEL;

namespace TriangleNet.Voronoi
{
	// Token: 0x02000200 RID: 512
	public interface IVoronoiFactory
	{
		// Token: 0x06000C79 RID: 3193
		void Initialize(int vertexCount, int edgeCount, int faceCount);

		// Token: 0x06000C7A RID: 3194
		void Reset();

		// Token: 0x06000C7B RID: 3195
		TriangleNet.Topology.DCEL.Vertex CreateVertex(double x, double y);

		// Token: 0x06000C7C RID: 3196
		HalfEdge CreateHalfEdge(TriangleNet.Topology.DCEL.Vertex origin, Face face);

		// Token: 0x06000C7D RID: 3197
		Face CreateFace(TriangleNet.Geometry.Vertex vertex);
	}
}
