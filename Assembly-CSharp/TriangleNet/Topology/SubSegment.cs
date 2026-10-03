using System;
using TriangleNet.Geometry;

namespace TriangleNet.Topology
{
	// Token: 0x02000209 RID: 521
	public class SubSegment : ISegment, IEdge
	{
		// Token: 0x06000CEA RID: 3306 RVA: 0x000A540E File Offset: 0x000A360E
		public SubSegment()
		{
			this.vertices = new Vertex[4];
			this.boundary = 0;
			this.subsegs = new Osub[2];
			this.triangles = new Otri[2];
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000CEB RID: 3307 RVA: 0x000A5441 File Offset: 0x000A3641
		public int P0
		{
			get
			{
				return this.vertices[0].id;
			}
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000CEC RID: 3308 RVA: 0x000A5450 File Offset: 0x000A3650
		public int P1
		{
			get
			{
				return this.vertices[1].id;
			}
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000CED RID: 3309 RVA: 0x000A545F File Offset: 0x000A365F
		public int Label
		{
			get
			{
				return this.boundary;
			}
		}

		// Token: 0x06000CEE RID: 3310 RVA: 0x000A5467 File Offset: 0x000A3667
		public Vertex GetVertex(int index)
		{
			return this.vertices[index];
		}

		// Token: 0x06000CEF RID: 3311 RVA: 0x000A5471 File Offset: 0x000A3671
		public ITriangle GetTriangle(int index)
		{
			if (this.triangles[index].tri.hash != -1)
			{
				return this.triangles[index].tri;
			}
			return null;
		}

		// Token: 0x06000CF0 RID: 3312 RVA: 0x000A549F File Offset: 0x000A369F
		public override int GetHashCode()
		{
			return this.hash;
		}

		// Token: 0x06000CF1 RID: 3313 RVA: 0x000A54A7 File Offset: 0x000A36A7
		public override string ToString()
		{
			return string.Format("SID {0}", this.hash);
		}

		// Token: 0x04001E6E RID: 7790
		internal int hash;

		// Token: 0x04001E6F RID: 7791
		internal Osub[] subsegs;

		// Token: 0x04001E70 RID: 7792
		internal Vertex[] vertices;

		// Token: 0x04001E71 RID: 7793
		internal Otri[] triangles;

		// Token: 0x04001E72 RID: 7794
		internal int boundary;
	}
}
