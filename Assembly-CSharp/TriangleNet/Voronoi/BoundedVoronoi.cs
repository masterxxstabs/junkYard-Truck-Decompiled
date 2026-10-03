using System;
using TriangleNet.Geometry;
using TriangleNet.Tools;
using TriangleNet.Topology.DCEL;

namespace TriangleNet.Voronoi
{
	// Token: 0x020001FE RID: 510
	public class BoundedVoronoi : VoronoiBase
	{
		// Token: 0x06000C6E RID: 3182 RVA: 0x000A2497 File Offset: 0x000A0697
		public BoundedVoronoi(Mesh mesh) : this(mesh, new DefaultVoronoiFactory(), RobustPredicates.Default)
		{
		}

		// Token: 0x06000C6F RID: 3183 RVA: 0x000A24AC File Offset: 0x000A06AC
		public BoundedVoronoi(Mesh mesh, IVoronoiFactory factory, IPredicates predicates) : base(mesh, factory, predicates, true)
		{
			this.offset = this.vertices.Count;
			this.vertices.Capacity = this.offset + mesh.hullsize;
			this.PostProcess();
			base.ResolveBoundaryEdges();
		}

		// Token: 0x06000C70 RID: 3184 RVA: 0x000A24F8 File Offset: 0x000A06F8
		private void PostProcess()
		{
			foreach (HalfEdge halfEdge in this.rays)
			{
				HalfEdge twin = halfEdge.twin;
				TriangleNet.Geometry.Vertex vertex = (TriangleNet.Geometry.Vertex)halfEdge.face.generator;
				TriangleNet.Geometry.Vertex vertex2 = (TriangleNet.Geometry.Vertex)twin.face.generator;
				if (this.predicates.CounterClockwise(vertex, vertex2, halfEdge.origin) <= 0.0)
				{
					this.HandleCase1(halfEdge, vertex, vertex2);
				}
				else
				{
					this.HandleCase2(halfEdge, vertex, vertex2);
				}
			}
		}

		// Token: 0x06000C71 RID: 3185 RVA: 0x000A25A0 File Offset: 0x000A07A0
		private void HandleCase1(HalfEdge edge, TriangleNet.Geometry.Vertex v1, TriangleNet.Geometry.Vertex v2)
		{
			TriangleNet.Topology.DCEL.Vertex origin = edge.twin.origin;
			origin.x = (v1.x + v2.x) / 2.0;
			origin.y = (v1.y + v2.y) / 2.0;
			TriangleNet.Topology.DCEL.Vertex vertex = this.factory.CreateVertex(v1.x, v1.y);
			HalfEdge halfEdge = this.factory.CreateHalfEdge(edge.twin.origin, edge.face);
			HalfEdge halfEdge2 = this.factory.CreateHalfEdge(vertex, edge.face);
			edge.next = halfEdge;
			halfEdge.next = halfEdge2;
			halfEdge2.next = edge.face.edge;
			vertex.leaving = halfEdge2;
			edge.face.edge = halfEdge2;
			this.edges.Add(halfEdge);
			this.edges.Add(halfEdge2);
			int count = this.edges.Count;
			halfEdge.id = count;
			halfEdge2.id = count + 1;
			Point point = vertex;
			int num = this.offset;
			this.offset = num + 1;
			point.id = num;
			this.vertices.Add(vertex);
		}

		// Token: 0x06000C72 RID: 3186 RVA: 0x000A26C8 File Offset: 0x000A08C8
		private void HandleCase2(HalfEdge edge, TriangleNet.Geometry.Vertex v1, TriangleNet.Geometry.Vertex v2)
		{
			Point origin = edge.origin;
			Point origin2 = edge.twin.origin;
			HalfEdge next = edge.twin.next;
			HalfEdge next2 = next.twin.next;
			IntersectionHelper.IntersectSegments(v1, v2, next.origin, next.twin.origin, ref origin2);
			IntersectionHelper.IntersectSegments(v1, v2, next2.origin, next2.twin.origin, ref origin);
			next.twin.next = edge.twin;
			edge.twin.next = next2;
			edge.twin.face = next2.face;
			next.origin = edge.twin.origin;
			edge.twin.twin = null;
			edge.twin = null;
			TriangleNet.Topology.DCEL.Vertex vertex = this.factory.CreateVertex(v1.x, v1.y);
			HalfEdge halfEdge = this.factory.CreateHalfEdge(vertex, edge.face);
			edge.next = halfEdge;
			halfEdge.next = edge.face.edge;
			edge.face.edge = halfEdge;
			this.edges.Add(halfEdge);
			halfEdge.id = this.edges.Count;
			Point point = vertex;
			int num = this.offset;
			this.offset = num + 1;
			point.id = num;
			this.vertices.Add(vertex);
		}

		// Token: 0x04001E50 RID: 7760
		private int offset;
	}
}
