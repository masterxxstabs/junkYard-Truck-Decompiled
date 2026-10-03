using System;
using System.Collections.Generic;
using TriangleNet.Geometry;

namespace TriangleNet.Topology.DCEL
{
	// Token: 0x0200020C RID: 524
	public class Face
	{
		// Token: 0x06000D09 RID: 3337 RVA: 0x000A5AE8 File Offset: 0x000A3CE8
		static Face()
		{
			Face.Empty.id = -1;
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000D0A RID: 3338 RVA: 0x000A5B00 File Offset: 0x000A3D00
		// (set) Token: 0x06000D0B RID: 3339 RVA: 0x000A5B08 File Offset: 0x000A3D08
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

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000D0C RID: 3340 RVA: 0x000A5B11 File Offset: 0x000A3D11
		// (set) Token: 0x06000D0D RID: 3341 RVA: 0x000A5B19 File Offset: 0x000A3D19
		public HalfEdge Edge
		{
			get
			{
				return this.edge;
			}
			set
			{
				this.edge = value;
			}
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06000D0E RID: 3342 RVA: 0x000A5B22 File Offset: 0x000A3D22
		// (set) Token: 0x06000D0F RID: 3343 RVA: 0x000A5B2A File Offset: 0x000A3D2A
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

		// Token: 0x06000D10 RID: 3344 RVA: 0x000A5B33 File Offset: 0x000A3D33
		public Face(Point generator) : this(generator, null)
		{
		}

		// Token: 0x06000D11 RID: 3345 RVA: 0x000A5B3D File Offset: 0x000A3D3D
		public Face(Point generator, HalfEdge edge)
		{
			this.generator = generator;
			this.edge = edge;
			this.bounded = true;
			if (generator != null)
			{
				this.id = generator.ID;
			}
		}

		// Token: 0x06000D12 RID: 3346 RVA: 0x000A5B6F File Offset: 0x000A3D6F
		public IEnumerable<HalfEdge> EnumerateEdges()
		{
			HalfEdge edge = this.Edge;
			int first = edge.ID;
			do
			{
				yield return edge;
				edge = edge.Next;
			}
			while (edge.ID != first);
			yield break;
		}

		// Token: 0x06000D13 RID: 3347 RVA: 0x000A5B7F File Offset: 0x000A3D7F
		public override string ToString()
		{
			return string.Format("F-ID {0}", this.id);
		}

		// Token: 0x04001E7E RID: 7806
		public static readonly Face Empty = new Face(null);

		// Token: 0x04001E7F RID: 7807
		internal int id;

		// Token: 0x04001E80 RID: 7808
		internal Point generator;

		// Token: 0x04001E81 RID: 7809
		internal HalfEdge edge;

		// Token: 0x04001E82 RID: 7810
		internal bool bounded;
	}
}
