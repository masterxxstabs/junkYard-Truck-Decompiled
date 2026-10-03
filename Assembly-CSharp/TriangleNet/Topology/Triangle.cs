using System;
using TriangleNet.Geometry;

namespace TriangleNet.Topology
{
	// Token: 0x0200020A RID: 522
	public class Triangle : ITriangle
	{
		// Token: 0x06000CF2 RID: 3314 RVA: 0x000A54BE File Offset: 0x000A36BE
		public Triangle()
		{
			this.vertices = new Vertex[3];
			this.subsegs = new Osub[3];
			this.neighbors = new Otri[3];
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000CF3 RID: 3315 RVA: 0x000A54EA File Offset: 0x000A36EA
		// (set) Token: 0x06000CF4 RID: 3316 RVA: 0x000A54F2 File Offset: 0x000A36F2
		public int ID
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000CF5 RID: 3317 RVA: 0x000A54FB File Offset: 0x000A36FB
		// (set) Token: 0x06000CF6 RID: 3318 RVA: 0x000A5503 File Offset: 0x000A3703
		public int Label
		{
			get
			{
				return this.label;
			}
			set
			{
				this.label = value;
			}
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000CF7 RID: 3319 RVA: 0x000A550C File Offset: 0x000A370C
		// (set) Token: 0x06000CF8 RID: 3320 RVA: 0x000A5514 File Offset: 0x000A3714
		public double Area
		{
			get
			{
				return this.area;
			}
			set
			{
				this.area = value;
			}
		}

		// Token: 0x06000CF9 RID: 3321 RVA: 0x000A551D File Offset: 0x000A371D
		public Vertex GetVertex(int index)
		{
			return this.vertices[index];
		}

		// Token: 0x06000CFA RID: 3322 RVA: 0x000A5527 File Offset: 0x000A3727
		public int GetVertexID(int index)
		{
			return this.vertices[index].id;
		}

		// Token: 0x06000CFB RID: 3323 RVA: 0x000A5536 File Offset: 0x000A3736
		public ITriangle GetNeighbor(int index)
		{
			if (this.neighbors[index].tri.hash != -1)
			{
				return this.neighbors[index].tri;
			}
			return null;
		}

		// Token: 0x06000CFC RID: 3324 RVA: 0x000A5564 File Offset: 0x000A3764
		public int GetNeighborID(int index)
		{
			if (this.neighbors[index].tri.hash != -1)
			{
				return this.neighbors[index].tri.id;
			}
			return -1;
		}

		// Token: 0x06000CFD RID: 3325 RVA: 0x000A5597 File Offset: 0x000A3797
		public ISegment GetSegment(int index)
		{
			if (this.subsegs[index].seg.hash != -1)
			{
				return this.subsegs[index].seg;
			}
			return null;
		}

		// Token: 0x06000CFE RID: 3326 RVA: 0x000A55C5 File Offset: 0x000A37C5
		public override int GetHashCode()
		{
			return this.hash;
		}

		// Token: 0x06000CFF RID: 3327 RVA: 0x000A55CD File Offset: 0x000A37CD
		public override string ToString()
		{
			return string.Format("TID {0}", this.hash);
		}

		// Token: 0x04001E73 RID: 7795
		internal int hash;

		// Token: 0x04001E74 RID: 7796
		internal int id;

		// Token: 0x04001E75 RID: 7797
		internal Otri[] neighbors;

		// Token: 0x04001E76 RID: 7798
		internal Vertex[] vertices;

		// Token: 0x04001E77 RID: 7799
		internal Osub[] subsegs;

		// Token: 0x04001E78 RID: 7800
		internal int label;

		// Token: 0x04001E79 RID: 7801
		internal double area;

		// Token: 0x04001E7A RID: 7802
		internal bool infected;
	}
}
