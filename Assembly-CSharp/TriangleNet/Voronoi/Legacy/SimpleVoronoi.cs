using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Voronoi.Legacy
{
	// Token: 0x02000205 RID: 517
	[Obsolete("Use TriangleNet.Voronoi.StandardVoronoi class instead.")]
	public class SimpleVoronoi : IVoronoi
	{
		// Token: 0x06000C98 RID: 3224 RVA: 0x000A3F38 File Offset: 0x000A2138
		public SimpleVoronoi(Mesh mesh)
		{
			this.mesh = mesh;
			this.Generate();
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000C99 RID: 3225 RVA: 0x000A3F58 File Offset: 0x000A2158
		public Point[] Points
		{
			get
			{
				return this.points;
			}
		}

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000C9A RID: 3226 RVA: 0x000A3F60 File Offset: 0x000A2160
		public ICollection<VoronoiRegion> Regions
		{
			get
			{
				return this.regions.Values;
			}
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000C9B RID: 3227 RVA: 0x000A3F6D File Offset: 0x000A216D
		public IEnumerable<IEdge> Edges
		{
			get
			{
				return this.EnumerateEdges();
			}
		}

		// Token: 0x06000C9C RID: 3228 RVA: 0x000A3F78 File Offset: 0x000A2178
		private void Generate()
		{
			this.mesh.Renumber();
			this.mesh.MakeVertexMap();
			this.points = new Point[this.mesh.triangles.Count + this.mesh.hullsize];
			this.regions = new Dictionary<int, VoronoiRegion>(this.mesh.vertices.Count);
			this.rayPoints = new Dictionary<int, Point>();
			this.rayIndex = 0;
			this.bounds = new Rectangle();
			this.ComputeCircumCenters();
			foreach (Vertex vertex in this.mesh.vertices.Values)
			{
				this.regions.Add(vertex.id, new VoronoiRegion(vertex));
			}
			foreach (VoronoiRegion region in this.regions.Values)
			{
				this.ConstructCell(region);
			}
		}

		// Token: 0x06000C9D RID: 3229 RVA: 0x000A40A8 File Offset: 0x000A22A8
		private void ComputeCircumCenters()
		{
			Otri otri = default(Otri);
			double num = 0.0;
			double num2 = 0.0;
			foreach (Triangle triangle in this.mesh.triangles)
			{
				otri.tri = triangle;
				Point point = this.predicates.FindCircumcenter(otri.Org(), otri.Dest(), otri.Apex(), ref num, ref num2);
				point.id = triangle.id;
				this.points[triangle.id] = point;
				this.bounds.Expand(point);
			}
			double num3 = Math.Max(this.bounds.Width, this.bounds.Height);
			this.bounds.Resize(num3, num3);
		}

		// Token: 0x06000C9E RID: 3230 RVA: 0x000A4198 File Offset: 0x000A2398
		private void ConstructCell(VoronoiRegion region)
		{
			Vertex vertex = region.Generator as Vertex;
			List<Point> list = new List<Point>();
			Otri otri = default(Otri);
			Otri ot = default(Otri);
			Otri otri2 = default(Otri);
			Otri otri3 = default(Otri);
			Osub osub = default(Osub);
			vertex.tri.Copy(ref ot);
			ot.Copy(ref otri);
			ot.Onext(ref otri2);
			if (otri2.tri.id == -1)
			{
				ot.Oprev(ref otri3);
				if (otri3.tri.id != -1)
				{
					ot.Copy(ref otri2);
					ot.Oprev();
					ot.Copy(ref otri);
				}
			}
			while (otri2.tri.id != -1)
			{
				list.Add(this.points[otri.tri.id]);
				region.AddNeighbor(otri.tri.id, this.regions[otri.Apex().id]);
				if (otri2.Equals(ot))
				{
					region.Add(list);
					return;
				}
				otri2.Copy(ref otri);
				otri2.Onext();
			}
			region.Bounded = false;
			int count = this.mesh.triangles.Count;
			otri.Lprev(ref otri2);
			otri2.Pivot(ref osub);
			int hash = osub.seg.hash;
			list.Add(this.points[otri.tri.id]);
			region.AddNeighbor(otri.tri.id, this.regions[otri.Apex().id]);
			Point point;
			if (!this.rayPoints.TryGetValue(hash, out point))
			{
				Vertex vertex2 = otri.Org();
				Vertex vertex3 = otri.Apex();
				this.BoxRayIntersection(this.points[otri.tri.id], vertex2.y - vertex3.y, vertex3.x - vertex2.x, out point);
				point.id = count + this.rayIndex;
				this.points[count + this.rayIndex] = point;
				this.rayIndex++;
				this.rayPoints.Add(hash, point);
			}
			list.Add(point);
			list.Reverse();
			ot.Copy(ref otri);
			otri.Oprev(ref otri3);
			while (otri3.tri.id != -1)
			{
				list.Add(this.points[otri3.tri.id]);
				region.AddNeighbor(otri3.tri.id, this.regions[otri3.Apex().id]);
				otri3.Copy(ref otri);
				otri3.Oprev();
			}
			otri.Pivot(ref osub);
			hash = osub.seg.hash;
			if (!this.rayPoints.TryGetValue(hash, out point))
			{
				Vertex vertex2 = otri.Org();
				Vertex vertex4 = otri.Dest();
				this.BoxRayIntersection(this.points[otri.tri.id], vertex4.y - vertex2.y, vertex2.x - vertex4.x, out point);
				point.id = count + this.rayIndex;
				this.rayPoints.Add(hash, point);
				this.points[count + this.rayIndex] = point;
				this.rayIndex++;
			}
			list.Add(point);
			region.AddNeighbor(point.id, this.regions[otri.Dest().id]);
			list.Reverse();
			region.Add(list);
		}

		// Token: 0x06000C9F RID: 3231 RVA: 0x000A4538 File Offset: 0x000A2738
		private bool BoxRayIntersection(Point pt, double dx, double dy, out Point intersect)
		{
			double x = pt.x;
			double y = pt.y;
			double left = this.bounds.Left;
			double right = this.bounds.Right;
			double bottom = this.bounds.Bottom;
			double top = this.bounds.Top;
			if (x < left || x > right || y < bottom || y > top)
			{
				intersect = null;
				return false;
			}
			double num;
			double x2;
			double y2;
			if (dx < 0.0)
			{
				num = (left - x) / dx;
				x2 = left;
				y2 = y + num * dy;
			}
			else if (dx > 0.0)
			{
				num = (right - x) / dx;
				x2 = right;
				y2 = y + num * dy;
			}
			else
			{
				num = double.MaxValue;
				y2 = (x2 = 0.0);
			}
			double num2;
			double x3;
			double y3;
			if (dy < 0.0)
			{
				num2 = (bottom - y) / dy;
				x3 = x + num2 * dx;
				y3 = bottom;
			}
			else if (dy > 0.0)
			{
				num2 = (top - y) / dy;
				x3 = x + num2 * dx;
				y3 = top;
			}
			else
			{
				num2 = double.MaxValue;
				y3 = (x3 = 0.0);
			}
			if (num < num2)
			{
				intersect = new Point(x2, y2);
			}
			else
			{
				intersect = new Point(x3, y3);
			}
			return true;
		}

		// Token: 0x06000CA0 RID: 3232 RVA: 0x000A4674 File Offset: 0x000A2874
		private IEnumerable<IEdge> EnumerateEdges()
		{
			List<IEdge> list = new List<IEdge>(this.Regions.Count * 2);
			foreach (VoronoiRegion voronoiRegion in this.Regions)
			{
				Point point = null;
				Point point2 = null;
				foreach (Point point3 in voronoiRegion.Vertices)
				{
					if (point == null)
					{
						point = point3;
						point2 = point3;
					}
					else
					{
						list.Add(new Edge(point2.id, point3.id));
						point2 = point3;
					}
				}
				if (voronoiRegion.Bounded && point != null)
				{
					list.Add(new Edge(point2.id, point.id));
				}
			}
			return list;
		}

		// Token: 0x04001E5C RID: 7772
		private IPredicates predicates = RobustPredicates.Default;

		// Token: 0x04001E5D RID: 7773
		private Mesh mesh;

		// Token: 0x04001E5E RID: 7774
		private Point[] points;

		// Token: 0x04001E5F RID: 7775
		private Dictionary<int, VoronoiRegion> regions;

		// Token: 0x04001E60 RID: 7776
		private Dictionary<int, Point> rayPoints;

		// Token: 0x04001E61 RID: 7777
		private int rayIndex;

		// Token: 0x04001E62 RID: 7778
		private Rectangle bounds;
	}
}
