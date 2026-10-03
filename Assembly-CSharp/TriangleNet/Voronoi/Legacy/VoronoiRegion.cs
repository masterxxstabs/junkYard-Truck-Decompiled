using System;
using System.Collections.Generic;
using TriangleNet.Geometry;

namespace TriangleNet.Voronoi.Legacy
{
	// Token: 0x02000206 RID: 518
	public class VoronoiRegion
	{
		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000CA1 RID: 3233 RVA: 0x000A476C File Offset: 0x000A296C
		public int ID
		{
			get
			{
				return this.id;
			}
		}

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000CA2 RID: 3234 RVA: 0x000A4774 File Offset: 0x000A2974
		public Point Generator
		{
			get
			{
				return this.generator;
			}
		}

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000CA3 RID: 3235 RVA: 0x000A477C File Offset: 0x000A297C
		public ICollection<Point> Vertices
		{
			get
			{
				return this.vertices;
			}
		}

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000CA4 RID: 3236 RVA: 0x000A4784 File Offset: 0x000A2984
		// (set) Token: 0x06000CA5 RID: 3237 RVA: 0x000A478C File Offset: 0x000A298C
		public bool Bounded
		{
			get
			{
				return this.bounded;
			}
			set
			{
				this.bounded = value;
			}
		}

		// Token: 0x06000CA6 RID: 3238 RVA: 0x000A4795 File Offset: 0x000A2995
		public VoronoiRegion(Vertex generator)
		{
			this.id = generator.id;
			this.generator = generator;
			this.vertices = new List<Point>();
			this.bounded = true;
			this.neighbors = new Dictionary<int, VoronoiRegion>();
		}

		// Token: 0x06000CA7 RID: 3239 RVA: 0x000A47CD File Offset: 0x000A29CD
		public void Add(Point point)
		{
			this.vertices.Add(point);
		}

		// Token: 0x06000CA8 RID: 3240 RVA: 0x000A47DB File Offset: 0x000A29DB
		public void Add(List<Point> points)
		{
			this.vertices.AddRange(points);
		}

		// Token: 0x06000CA9 RID: 3241 RVA: 0x000A47EC File Offset: 0x000A29EC
		public VoronoiRegion GetNeighbor(Point p)
		{
			VoronoiRegion result;
			if (this.neighbors.TryGetValue(p.id, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06000CAA RID: 3242 RVA: 0x000A4811 File Offset: 0x000A2A11
		internal void AddNeighbor(int id, VoronoiRegion neighbor)
		{
			this.neighbors.Add(id, neighbor);
		}

		// Token: 0x06000CAB RID: 3243 RVA: 0x000A4820 File Offset: 0x000A2A20
		public override string ToString()
		{
			return string.Format("R-ID {0}", this.id);
		}

		// Token: 0x04001E63 RID: 7779
		private int id;

		// Token: 0x04001E64 RID: 7780
		private Point generator;

		// Token: 0x04001E65 RID: 7781
		private List<Point> vertices;

		// Token: 0x04001E66 RID: 7782
		private bool bounded;

		// Token: 0x04001E67 RID: 7783
		private Dictionary<int, VoronoiRegion> neighbors;
	}
}
