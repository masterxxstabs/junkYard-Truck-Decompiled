using System;

namespace TriangleNet.Geometry
{
	// Token: 0x02000246 RID: 582
	public class Segment : ISegment, IEdge
	{
		// Token: 0x1700012A RID: 298
		// (get) Token: 0x06000ED1 RID: 3793 RVA: 0x000B19CF File Offset: 0x000AFBCF
		// (set) Token: 0x06000ED2 RID: 3794 RVA: 0x000B19D7 File Offset: 0x000AFBD7
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

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x06000ED3 RID: 3795 RVA: 0x000B19E0 File Offset: 0x000AFBE0
		public int P0
		{
			get
			{
				return this.v0.id;
			}
		}

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x06000ED4 RID: 3796 RVA: 0x000B19ED File Offset: 0x000AFBED
		public int P1
		{
			get
			{
				return this.v1.id;
			}
		}

		// Token: 0x06000ED5 RID: 3797 RVA: 0x000B19FA File Offset: 0x000AFBFA
		public Segment(Vertex v0, Vertex v1) : this(v0, v1, 0)
		{
		}

		// Token: 0x06000ED6 RID: 3798 RVA: 0x000B1A05 File Offset: 0x000AFC05
		public Segment(Vertex v0, Vertex v1, int label)
		{
			this.v0 = v0;
			this.v1 = v1;
			this.label = label;
		}

		// Token: 0x06000ED7 RID: 3799 RVA: 0x000B1A22 File Offset: 0x000AFC22
		public Vertex GetVertex(int index)
		{
			if (index == 0)
			{
				return this.v0;
			}
			if (index == 1)
			{
				return this.v1;
			}
			throw new IndexOutOfRangeException();
		}

		// Token: 0x06000ED8 RID: 3800 RVA: 0x000A22BF File Offset: 0x000A04BF
		public ITriangle GetTriangle(int index)
		{
			throw new NotImplementedException();
		}

		// Token: 0x04001F23 RID: 7971
		private Vertex v0;

		// Token: 0x04001F24 RID: 7972
		private Vertex v1;

		// Token: 0x04001F25 RID: 7973
		private int label;
	}
}
