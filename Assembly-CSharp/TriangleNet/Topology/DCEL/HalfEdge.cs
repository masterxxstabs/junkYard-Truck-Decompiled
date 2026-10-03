using System;

namespace TriangleNet.Topology.DCEL
{
	// Token: 0x0200020D RID: 525
	public class HalfEdge
	{
		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06000D14 RID: 3348 RVA: 0x000A5B96 File Offset: 0x000A3D96
		// (set) Token: 0x06000D15 RID: 3349 RVA: 0x000A5B9E File Offset: 0x000A3D9E
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

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000D16 RID: 3350 RVA: 0x000A5BA7 File Offset: 0x000A3DA7
		// (set) Token: 0x06000D17 RID: 3351 RVA: 0x000A5BAF File Offset: 0x000A3DAF
		public int Boundary
		{
			get
			{
				return this.mark;
			}
			set
			{
				this.mark = value;
			}
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000D18 RID: 3352 RVA: 0x000A5BB8 File Offset: 0x000A3DB8
		// (set) Token: 0x06000D19 RID: 3353 RVA: 0x000A5BC0 File Offset: 0x000A3DC0
		public Vertex Origin
		{
			get
			{
				return this.origin;
			}
			set
			{
				this.origin = value;
			}
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000D1A RID: 3354 RVA: 0x000A5BC9 File Offset: 0x000A3DC9
		// (set) Token: 0x06000D1B RID: 3355 RVA: 0x000A5BD1 File Offset: 0x000A3DD1
		public Face Face
		{
			get
			{
				return this.face;
			}
			set
			{
				this.face = value;
			}
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000D1C RID: 3356 RVA: 0x000A5BDA File Offset: 0x000A3DDA
		// (set) Token: 0x06000D1D RID: 3357 RVA: 0x000A5BE2 File Offset: 0x000A3DE2
		public HalfEdge Twin
		{
			get
			{
				return this.twin;
			}
			set
			{
				this.twin = value;
			}
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000D1E RID: 3358 RVA: 0x000A5BEB File Offset: 0x000A3DEB
		// (set) Token: 0x06000D1F RID: 3359 RVA: 0x000A5BF3 File Offset: 0x000A3DF3
		public HalfEdge Next
		{
			get
			{
				return this.next;
			}
			set
			{
				this.next = value;
			}
		}

		// Token: 0x06000D20 RID: 3360 RVA: 0x000A5BFC File Offset: 0x000A3DFC
		public HalfEdge(Vertex origin)
		{
			this.origin = origin;
		}

		// Token: 0x06000D21 RID: 3361 RVA: 0x000A5C0B File Offset: 0x000A3E0B
		public HalfEdge(Vertex origin, Face face)
		{
			this.origin = origin;
			this.face = face;
			if (face != null && face.edge == null)
			{
				face.edge = this;
			}
		}

		// Token: 0x06000D22 RID: 3362 RVA: 0x000A5C33 File Offset: 0x000A3E33
		public override string ToString()
		{
			return string.Format("HE-ID {0} (Origin = VID-{1})", this.id, this.origin.id);
		}

		// Token: 0x04001E83 RID: 7811
		internal int id;

		// Token: 0x04001E84 RID: 7812
		internal int mark;

		// Token: 0x04001E85 RID: 7813
		internal Vertex origin;

		// Token: 0x04001E86 RID: 7814
		internal Face face;

		// Token: 0x04001E87 RID: 7815
		internal HalfEdge twin;

		// Token: 0x04001E88 RID: 7816
		internal HalfEdge next;
	}
}
