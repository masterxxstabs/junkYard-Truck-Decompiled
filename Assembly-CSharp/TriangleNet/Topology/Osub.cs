using System;
using TriangleNet.Geometry;

namespace TriangleNet.Topology
{
	// Token: 0x02000207 RID: 519
	public struct Osub
	{
		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000CAC RID: 3244 RVA: 0x000A4837 File Offset: 0x000A2A37
		public SubSegment Segment
		{
			get
			{
				return this.seg;
			}
		}

		// Token: 0x06000CAD RID: 3245 RVA: 0x000A483F File Offset: 0x000A2A3F
		public override string ToString()
		{
			if (this.seg == null)
			{
				return "O-TID [null]";
			}
			return string.Format("O-SID {0}", this.seg.hash);
		}

		// Token: 0x06000CAE RID: 3246 RVA: 0x000A4869 File Offset: 0x000A2A69
		public void Sym(ref Osub os)
		{
			os.seg = this.seg;
			os.orient = 1 - this.orient;
		}

		// Token: 0x06000CAF RID: 3247 RVA: 0x000A4885 File Offset: 0x000A2A85
		public void Sym()
		{
			this.orient = 1 - this.orient;
		}

		// Token: 0x06000CB0 RID: 3248 RVA: 0x000A4895 File Offset: 0x000A2A95
		public void Pivot(ref Osub os)
		{
			os = this.seg.subsegs[this.orient];
		}

		// Token: 0x06000CB1 RID: 3249 RVA: 0x000A48B3 File Offset: 0x000A2AB3
		internal void Pivot(ref Otri ot)
		{
			ot = this.seg.triangles[this.orient];
		}

		// Token: 0x06000CB2 RID: 3250 RVA: 0x000A48D1 File Offset: 0x000A2AD1
		public void Next(ref Osub ot)
		{
			ot = this.seg.subsegs[1 - this.orient];
		}

		// Token: 0x06000CB3 RID: 3251 RVA: 0x000A48F1 File Offset: 0x000A2AF1
		public void Next()
		{
			this = this.seg.subsegs[1 - this.orient];
		}

		// Token: 0x06000CB4 RID: 3252 RVA: 0x000A4911 File Offset: 0x000A2B11
		public Vertex Org()
		{
			return this.seg.vertices[this.orient];
		}

		// Token: 0x06000CB5 RID: 3253 RVA: 0x000A4925 File Offset: 0x000A2B25
		public Vertex Dest()
		{
			return this.seg.vertices[1 - this.orient];
		}

		// Token: 0x06000CB6 RID: 3254 RVA: 0x000A493B File Offset: 0x000A2B3B
		internal void SetOrg(Vertex vertex)
		{
			this.seg.vertices[this.orient] = vertex;
		}

		// Token: 0x06000CB7 RID: 3255 RVA: 0x000A4950 File Offset: 0x000A2B50
		internal void SetDest(Vertex vertex)
		{
			this.seg.vertices[1 - this.orient] = vertex;
		}

		// Token: 0x06000CB8 RID: 3256 RVA: 0x000A4967 File Offset: 0x000A2B67
		internal Vertex SegOrg()
		{
			return this.seg.vertices[2 + this.orient];
		}

		// Token: 0x06000CB9 RID: 3257 RVA: 0x000A497D File Offset: 0x000A2B7D
		internal Vertex SegDest()
		{
			return this.seg.vertices[3 - this.orient];
		}

		// Token: 0x06000CBA RID: 3258 RVA: 0x000A4993 File Offset: 0x000A2B93
		internal void SetSegOrg(Vertex vertex)
		{
			this.seg.vertices[2 + this.orient] = vertex;
		}

		// Token: 0x06000CBB RID: 3259 RVA: 0x000A49AA File Offset: 0x000A2BAA
		internal void SetSegDest(Vertex vertex)
		{
			this.seg.vertices[3 - this.orient] = vertex;
		}

		// Token: 0x06000CBC RID: 3260 RVA: 0x000A49C1 File Offset: 0x000A2BC1
		internal void Bond(ref Osub os)
		{
			this.seg.subsegs[this.orient] = os;
			os.seg.subsegs[os.orient] = this;
		}

		// Token: 0x06000CBD RID: 3261 RVA: 0x000A49FB File Offset: 0x000A2BFB
		internal void Dissolve(SubSegment dummy)
		{
			this.seg.subsegs[this.orient].seg = dummy;
		}

		// Token: 0x06000CBE RID: 3262 RVA: 0x000A4A19 File Offset: 0x000A2C19
		internal bool Equal(Osub os)
		{
			return this.seg == os.seg && this.orient == os.orient;
		}

		// Token: 0x06000CBF RID: 3263 RVA: 0x000A4A39 File Offset: 0x000A2C39
		internal void TriDissolve(Triangle dummy)
		{
			this.seg.triangles[this.orient].tri = dummy;
		}

		// Token: 0x06000CC0 RID: 3264 RVA: 0x000A4A57 File Offset: 0x000A2C57
		internal static bool IsDead(SubSegment sub)
		{
			return sub.subsegs[0].seg == null;
		}

		// Token: 0x06000CC1 RID: 3265 RVA: 0x000A4A6D File Offset: 0x000A2C6D
		internal static void Kill(SubSegment sub)
		{
			sub.subsegs[0].seg = null;
			sub.subsegs[1].seg = null;
		}

		// Token: 0x04001E68 RID: 7784
		internal SubSegment seg;

		// Token: 0x04001E69 RID: 7785
		internal int orient;
	}
}
