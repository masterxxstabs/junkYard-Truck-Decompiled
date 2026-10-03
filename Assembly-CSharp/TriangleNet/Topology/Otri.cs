using System;
using TriangleNet.Geometry;

namespace TriangleNet.Topology
{
	// Token: 0x02000208 RID: 520
	public struct Otri
	{
		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000CC2 RID: 3266 RVA: 0x000A4A93 File Offset: 0x000A2C93
		// (set) Token: 0x06000CC3 RID: 3267 RVA: 0x000A4A9B File Offset: 0x000A2C9B
		public Triangle Triangle
		{
			get
			{
				return this.tri;
			}
			set
			{
				this.tri = value;
			}
		}

		// Token: 0x06000CC4 RID: 3268 RVA: 0x000A4AA4 File Offset: 0x000A2CA4
		public override string ToString()
		{
			if (this.tri == null)
			{
				return "O-TID [null]";
			}
			return string.Format("O-TID {0}", this.tri.hash);
		}

		// Token: 0x06000CC5 RID: 3269 RVA: 0x000A4AD0 File Offset: 0x000A2CD0
		public void Sym(ref Otri ot)
		{
			ot.tri = this.tri.neighbors[this.orient].tri;
			ot.orient = this.tri.neighbors[this.orient].orient;
		}

		// Token: 0x06000CC6 RID: 3270 RVA: 0x000A4B20 File Offset: 0x000A2D20
		public void Sym()
		{
			int num = this.orient;
			this.orient = this.tri.neighbors[num].orient;
			this.tri = this.tri.neighbors[num].tri;
		}

		// Token: 0x06000CC7 RID: 3271 RVA: 0x000A4B6C File Offset: 0x000A2D6C
		public void Lnext(ref Otri ot)
		{
			ot.tri = this.tri;
			ot.orient = Otri.plus1Mod3[this.orient];
		}

		// Token: 0x06000CC8 RID: 3272 RVA: 0x000A4B8C File Offset: 0x000A2D8C
		public void Lnext()
		{
			this.orient = Otri.plus1Mod3[this.orient];
		}

		// Token: 0x06000CC9 RID: 3273 RVA: 0x000A4BA0 File Offset: 0x000A2DA0
		public void Lprev(ref Otri ot)
		{
			ot.tri = this.tri;
			ot.orient = Otri.minus1Mod3[this.orient];
		}

		// Token: 0x06000CCA RID: 3274 RVA: 0x000A4BC0 File Offset: 0x000A2DC0
		public void Lprev()
		{
			this.orient = Otri.minus1Mod3[this.orient];
		}

		// Token: 0x06000CCB RID: 3275 RVA: 0x000A4BD4 File Offset: 0x000A2DD4
		public void Onext(ref Otri ot)
		{
			ot.tri = this.tri;
			ot.orient = Otri.minus1Mod3[this.orient];
			int num = ot.orient;
			ot.orient = ot.tri.neighbors[num].orient;
			ot.tri = ot.tri.neighbors[num].tri;
		}

		// Token: 0x06000CCC RID: 3276 RVA: 0x000A4C40 File Offset: 0x000A2E40
		public void Onext()
		{
			this.orient = Otri.minus1Mod3[this.orient];
			int num = this.orient;
			this.orient = this.tri.neighbors[num].orient;
			this.tri = this.tri.neighbors[num].tri;
		}

		// Token: 0x06000CCD RID: 3277 RVA: 0x000A4CA0 File Offset: 0x000A2EA0
		public void Oprev(ref Otri ot)
		{
			ot.tri = this.tri.neighbors[this.orient].tri;
			ot.orient = this.tri.neighbors[this.orient].orient;
			ot.orient = Otri.plus1Mod3[ot.orient];
		}

		// Token: 0x06000CCE RID: 3278 RVA: 0x000A4D04 File Offset: 0x000A2F04
		public void Oprev()
		{
			int num = this.orient;
			this.orient = this.tri.neighbors[num].orient;
			this.tri = this.tri.neighbors[num].tri;
			this.orient = Otri.plus1Mod3[this.orient];
		}

		// Token: 0x06000CCF RID: 3279 RVA: 0x000A4D64 File Offset: 0x000A2F64
		public void Dnext(ref Otri ot)
		{
			ot.tri = this.tri.neighbors[this.orient].tri;
			ot.orient = this.tri.neighbors[this.orient].orient;
			ot.orient = Otri.minus1Mod3[ot.orient];
		}

		// Token: 0x06000CD0 RID: 3280 RVA: 0x000A4DC8 File Offset: 0x000A2FC8
		public void Dnext()
		{
			int num = this.orient;
			this.orient = this.tri.neighbors[num].orient;
			this.tri = this.tri.neighbors[num].tri;
			this.orient = Otri.minus1Mod3[this.orient];
		}

		// Token: 0x06000CD1 RID: 3281 RVA: 0x000A4E28 File Offset: 0x000A3028
		public void Dprev(ref Otri ot)
		{
			ot.tri = this.tri;
			ot.orient = Otri.plus1Mod3[this.orient];
			int num = ot.orient;
			ot.orient = ot.tri.neighbors[num].orient;
			ot.tri = ot.tri.neighbors[num].tri;
		}

		// Token: 0x06000CD2 RID: 3282 RVA: 0x000A4E94 File Offset: 0x000A3094
		public void Dprev()
		{
			this.orient = Otri.plus1Mod3[this.orient];
			int num = this.orient;
			this.orient = this.tri.neighbors[num].orient;
			this.tri = this.tri.neighbors[num].tri;
		}

		// Token: 0x06000CD3 RID: 3283 RVA: 0x000A4EF4 File Offset: 0x000A30F4
		public void Rnext(ref Otri ot)
		{
			ot.tri = this.tri.neighbors[this.orient].tri;
			ot.orient = this.tri.neighbors[this.orient].orient;
			ot.orient = Otri.plus1Mod3[ot.orient];
			int num = ot.orient;
			ot.orient = ot.tri.neighbors[num].orient;
			ot.tri = ot.tri.neighbors[num].tri;
		}

		// Token: 0x06000CD4 RID: 3284 RVA: 0x000A4F94 File Offset: 0x000A3194
		public void Rnext()
		{
			int num = this.orient;
			this.orient = this.tri.neighbors[num].orient;
			this.tri = this.tri.neighbors[num].tri;
			this.orient = Otri.plus1Mod3[this.orient];
			num = this.orient;
			this.orient = this.tri.neighbors[num].orient;
			this.tri = this.tri.neighbors[num].tri;
		}

		// Token: 0x06000CD5 RID: 3285 RVA: 0x000A5034 File Offset: 0x000A3234
		public void Rprev(ref Otri ot)
		{
			ot.tri = this.tri.neighbors[this.orient].tri;
			ot.orient = this.tri.neighbors[this.orient].orient;
			ot.orient = Otri.minus1Mod3[ot.orient];
			int num = ot.orient;
			ot.orient = ot.tri.neighbors[num].orient;
			ot.tri = ot.tri.neighbors[num].tri;
		}

		// Token: 0x06000CD6 RID: 3286 RVA: 0x000A50D4 File Offset: 0x000A32D4
		public void Rprev()
		{
			int num = this.orient;
			this.orient = this.tri.neighbors[num].orient;
			this.tri = this.tri.neighbors[num].tri;
			this.orient = Otri.minus1Mod3[this.orient];
			num = this.orient;
			this.orient = this.tri.neighbors[num].orient;
			this.tri = this.tri.neighbors[num].tri;
		}

		// Token: 0x06000CD7 RID: 3287 RVA: 0x000A5171 File Offset: 0x000A3371
		public Vertex Org()
		{
			return this.tri.vertices[Otri.plus1Mod3[this.orient]];
		}

		// Token: 0x06000CD8 RID: 3288 RVA: 0x000A518B File Offset: 0x000A338B
		public Vertex Dest()
		{
			return this.tri.vertices[Otri.minus1Mod3[this.orient]];
		}

		// Token: 0x06000CD9 RID: 3289 RVA: 0x000A51A5 File Offset: 0x000A33A5
		public Vertex Apex()
		{
			return this.tri.vertices[this.orient];
		}

		// Token: 0x06000CDA RID: 3290 RVA: 0x000A51B9 File Offset: 0x000A33B9
		public void Copy(ref Otri ot)
		{
			ot.tri = this.tri;
			ot.orient = this.orient;
		}

		// Token: 0x06000CDB RID: 3291 RVA: 0x000A51D3 File Offset: 0x000A33D3
		public bool Equals(Otri ot)
		{
			return this.tri == ot.tri && this.orient == ot.orient;
		}

		// Token: 0x06000CDC RID: 3292 RVA: 0x000A51F3 File Offset: 0x000A33F3
		internal void SetOrg(Vertex v)
		{
			this.tri.vertices[Otri.plus1Mod3[this.orient]] = v;
		}

		// Token: 0x06000CDD RID: 3293 RVA: 0x000A520E File Offset: 0x000A340E
		internal void SetDest(Vertex v)
		{
			this.tri.vertices[Otri.minus1Mod3[this.orient]] = v;
		}

		// Token: 0x06000CDE RID: 3294 RVA: 0x000A5229 File Offset: 0x000A3429
		internal void SetApex(Vertex v)
		{
			this.tri.vertices[this.orient] = v;
		}

		// Token: 0x06000CDF RID: 3295 RVA: 0x000A5240 File Offset: 0x000A3440
		internal void Bond(ref Otri ot)
		{
			this.tri.neighbors[this.orient].tri = ot.tri;
			this.tri.neighbors[this.orient].orient = ot.orient;
			ot.tri.neighbors[ot.orient].tri = this.tri;
			ot.tri.neighbors[ot.orient].orient = this.orient;
		}

		// Token: 0x06000CE0 RID: 3296 RVA: 0x000A52D1 File Offset: 0x000A34D1
		internal void Dissolve(Triangle dummy)
		{
			this.tri.neighbors[this.orient].tri = dummy;
			this.tri.neighbors[this.orient].orient = 0;
		}

		// Token: 0x06000CE1 RID: 3297 RVA: 0x000A530B File Offset: 0x000A350B
		internal void Infect()
		{
			this.tri.infected = true;
		}

		// Token: 0x06000CE2 RID: 3298 RVA: 0x000A5319 File Offset: 0x000A3519
		internal void Uninfect()
		{
			this.tri.infected = false;
		}

		// Token: 0x06000CE3 RID: 3299 RVA: 0x000A5327 File Offset: 0x000A3527
		internal bool IsInfected()
		{
			return this.tri.infected;
		}

		// Token: 0x06000CE4 RID: 3300 RVA: 0x000A5334 File Offset: 0x000A3534
		internal void Pivot(ref Osub os)
		{
			os = this.tri.subsegs[this.orient];
		}

		// Token: 0x06000CE5 RID: 3301 RVA: 0x000A5352 File Offset: 0x000A3552
		internal void SegBond(ref Osub os)
		{
			this.tri.subsegs[this.orient] = os;
			os.seg.triangles[os.orient] = this;
		}

		// Token: 0x06000CE6 RID: 3302 RVA: 0x000A538C File Offset: 0x000A358C
		internal void SegDissolve(SubSegment dummy)
		{
			this.tri.subsegs[this.orient].seg = dummy;
		}

		// Token: 0x06000CE7 RID: 3303 RVA: 0x000A53AA File Offset: 0x000A35AA
		internal static bool IsDead(Triangle tria)
		{
			return tria.neighbors[0].tri == null;
		}

		// Token: 0x06000CE8 RID: 3304 RVA: 0x000A53C0 File Offset: 0x000A35C0
		internal static void Kill(Triangle tri)
		{
			tri.neighbors[0].tri = null;
			tri.neighbors[2].tri = null;
		}

		// Token: 0x06000CE9 RID: 3305 RVA: 0x000A53E6 File Offset: 0x000A35E6
		// Note: this type is marked as 'beforefieldinit'.
		static Otri()
		{
			int[] array = new int[3];
			array[0] = 1;
			array[1] = 2;
			Otri.plus1Mod3 = array;
			Otri.minus1Mod3 = new int[]
			{
				2,
				0,
				1
			};
		}

		// Token: 0x04001E6A RID: 7786
		internal Triangle tri;

		// Token: 0x04001E6B RID: 7787
		internal int orient;

		// Token: 0x04001E6C RID: 7788
		private static readonly int[] plus1Mod3;

		// Token: 0x04001E6D RID: 7789
		private static readonly int[] minus1Mod3;
	}
}
