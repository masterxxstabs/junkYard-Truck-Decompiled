using System;
using System.Collections.Generic;
using TriangleNet.Geometry;

namespace TriangleNet.Topology.DCEL
{
	// Token: 0x0200020E RID: 526
	public class Vertex : Point
	{
		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000D23 RID: 3363 RVA: 0x000A5C5A File Offset: 0x000A3E5A
		// (set) Token: 0x06000D24 RID: 3364 RVA: 0x000A5C62 File Offset: 0x000A3E62
		public HalfEdge Leaving
		{
			get
			{
				return this.leaving;
			}
			set
			{
				this.leaving = value;
			}
		}

		// Token: 0x06000D25 RID: 3365 RVA: 0x000A5C6B File Offset: 0x000A3E6B
		public Vertex(double x, double y) : base(x, y)
		{
		}

		// Token: 0x06000D26 RID: 3366 RVA: 0x000A5C75 File Offset: 0x000A3E75
		public Vertex(double x, double y, HalfEdge leaving) : base(x, y)
		{
			this.leaving = leaving;
		}

		// Token: 0x06000D27 RID: 3367 RVA: 0x000A5C86 File Offset: 0x000A3E86
		public IEnumerable<HalfEdge> EnumerateEdges()
		{
			HalfEdge edge = this.Leaving;
			int first = edge.ID;
			do
			{
				yield return edge;
				edge = edge.Twin.Next;
			}
			while (edge.ID != first);
			yield break;
		}

		// Token: 0x06000D28 RID: 3368 RVA: 0x000A5C96 File Offset: 0x000A3E96
		public override string ToString()
		{
			return string.Format("V-ID {0}", this.id);
		}

		// Token: 0x04001E89 RID: 7817
		internal HalfEdge leaving;
	}
}
