using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Topology;
using TriangleNet.Topology.DCEL;
using TriangleNet.Voronoi;

namespace TriangleNet.Smoothing
{
	// Token: 0x02000219 RID: 537
	public class SimpleSmoother : ISmoother
	{
		// Token: 0x06000D74 RID: 3444 RVA: 0x000A842D File Offset: 0x000A662D
		public SimpleSmoother() : this(new VoronoiFactory())
		{
		}

		// Token: 0x06000D75 RID: 3445 RVA: 0x000A843C File Offset: 0x000A663C
		public SimpleSmoother(IVoronoiFactory factory)
		{
			this.factory = factory;
			this.pool = new TrianglePool();
			this.config = new Configuration(() => RobustPredicates.Default, () => this.pool.Restart());
			this.options = new ConstraintOptions
			{
				ConformingDelaunay = true
			};
		}

		// Token: 0x06000D76 RID: 3446 RVA: 0x000A84A9 File Offset: 0x000A66A9
		public SimpleSmoother(IVoronoiFactory factory, Configuration config)
		{
			this.factory = factory;
			this.config = config;
			this.options = new ConstraintOptions
			{
				ConformingDelaunay = true
			};
		}

		// Token: 0x06000D77 RID: 3447 RVA: 0x000A84D1 File Offset: 0x000A66D1
		public void Smooth(IMesh mesh)
		{
			this.Smooth(mesh, 10);
		}

		// Token: 0x06000D78 RID: 3448 RVA: 0x000A84DC File Offset: 0x000A66DC
		public void Smooth(IMesh mesh, int limit)
		{
			Mesh mesh2 = (Mesh)mesh;
			GenericMesher genericMesher = new GenericMesher(this.config);
			IPredicates predicates = this.config.Predicates();
			this.options.SegmentSplitting = mesh2.behavior.NoBisect;
			for (int i = 0; i < limit; i++)
			{
				this.Step(mesh2, this.factory, predicates);
				mesh2 = (Mesh)genericMesher.Triangulate(this.Rebuild(mesh2), this.options);
				this.factory.Reset();
			}
			mesh2.CopyTo((Mesh)mesh);
		}

		// Token: 0x06000D79 RID: 3449 RVA: 0x000A8570 File Offset: 0x000A6770
		private void Step(Mesh mesh, IVoronoiFactory factory, IPredicates predicates)
		{
			foreach (Face face in new BoundedVoronoi(mesh, factory, predicates).Faces)
			{
				if (face.generator.label == 0)
				{
					double x;
					double y;
					this.Centroid(face, out x, out y);
					face.generator.x = x;
					face.generator.y = y;
				}
			}
		}

		// Token: 0x06000D7A RID: 3450 RVA: 0x000A85F4 File Offset: 0x000A67F4
		private void Centroid(Face face, out double x, out double y)
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			HalfEdge halfEdge = face.Edge;
			int id = halfEdge.Next.ID;
			do
			{
				Point origin = halfEdge.Origin;
				Point origin2 = halfEdge.Twin.Origin;
				double num4 = origin.x * origin2.y - origin2.x * origin.y;
				num += num4;
				num2 += (origin2.x + origin.x) * num4;
				num3 += (origin2.y + origin.y) * num4;
				halfEdge = halfEdge.Next;
			}
			while (halfEdge.Next.ID != id);
			x = num2 / (3.0 * num);
			y = num3 / (3.0 * num);
		}

		// Token: 0x06000D7B RID: 3451 RVA: 0x000A86D0 File Offset: 0x000A68D0
		private Polygon Rebuild(Mesh mesh)
		{
			Polygon polygon = new Polygon(mesh.vertices.Count);
			foreach (TriangleNet.Geometry.Vertex vertex in mesh.vertices.Values)
			{
				vertex.type = VertexType.InputVertex;
				polygon.Points.Add(vertex);
			}
			List<ISegment> collection = new List<SubSegment>(mesh.subsegs.Values).ConvertAll<ISegment>((SubSegment x) => x);
			polygon.Segments.AddRange(collection);
			polygon.Holes.AddRange(mesh.holes);
			polygon.Regions.AddRange(mesh.regions);
			return polygon;
		}

		// Token: 0x04001EB0 RID: 7856
		private TrianglePool pool;

		// Token: 0x04001EB1 RID: 7857
		private Configuration config;

		// Token: 0x04001EB2 RID: 7858
		private IVoronoiFactory factory;

		// Token: 0x04001EB3 RID: 7859
		private ConstraintOptions options;
	}
}
