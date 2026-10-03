using System;
using TriangleNet.Geometry;
using TriangleNet.Topology.DCEL;

namespace TriangleNet.Voronoi
{
	// Token: 0x020001FF RID: 511
	public class DefaultVoronoiFactory : IVoronoiFactory
	{
		// Token: 0x06000C73 RID: 3187 RVA: 0x00002188 File Offset: 0x00000388
		public void Initialize(int vertexCount, int edgeCount, int faceCount)
		{
		}

		// Token: 0x06000C74 RID: 3188 RVA: 0x00002188 File Offset: 0x00000388
		public void Reset()
		{
		}

		// Token: 0x06000C75 RID: 3189 RVA: 0x000A2822 File Offset: 0x000A0A22
		public TriangleNet.Topology.DCEL.Vertex CreateVertex(double x, double y)
		{
			return new TriangleNet.Topology.DCEL.Vertex(x, y);
		}

		// Token: 0x06000C76 RID: 3190 RVA: 0x000A282B File Offset: 0x000A0A2B
		public HalfEdge CreateHalfEdge(TriangleNet.Topology.DCEL.Vertex origin, Face face)
		{
			return new HalfEdge(origin, face);
		}

		// Token: 0x06000C77 RID: 3191 RVA: 0x000A2834 File Offset: 0x000A0A34
		public Face CreateFace(TriangleNet.Geometry.Vertex vertex)
		{
			return new Face(vertex);
		}
	}
}
