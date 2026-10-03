using System;
using System.Collections;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Meshing.Iterators
{
	// Token: 0x02000225 RID: 549
	public class EdgeIterator : IEnumerator<Edge>, IEnumerator, IDisposable
	{
		// Token: 0x06000DCD RID: 3533 RVA: 0x000AC170 File Offset: 0x000AA370
		public EdgeIterator(Mesh mesh)
		{
			this.triangles = mesh.triangles.GetEnumerator();
			this.triangles.MoveNext();
			this.tri.tri = this.triangles.Current;
			this.tri.orient = 0;
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x06000DCE RID: 3534 RVA: 0x000AC1C2 File Offset: 0x000AA3C2
		public Edge Current
		{
			get
			{
				return this.current;
			}
		}

		// Token: 0x06000DCF RID: 3535 RVA: 0x000AC1CA File Offset: 0x000AA3CA
		public void Dispose()
		{
			this.triangles.Dispose();
		}

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06000DD0 RID: 3536 RVA: 0x000AC1C2 File Offset: 0x000AA3C2
		object IEnumerator.Current
		{
			get
			{
				return this.current;
			}
		}

		// Token: 0x06000DD1 RID: 3537 RVA: 0x000AC1D8 File Offset: 0x000AA3D8
		public bool MoveNext()
		{
			if (this.tri.tri == null)
			{
				return false;
			}
			this.current = null;
			while (this.current == null)
			{
				if (this.tri.orient == 3)
				{
					if (!this.triangles.MoveNext())
					{
						return false;
					}
					this.tri.tri = this.triangles.Current;
					this.tri.orient = 0;
				}
				this.tri.Sym(ref this.neighbor);
				if (this.tri.tri.id < this.neighbor.tri.id || this.neighbor.tri.id == -1)
				{
					this.p1 = this.tri.Org();
					this.p2 = this.tri.Dest();
					this.tri.Pivot(ref this.sub);
					this.current = new Edge(this.p1.id, this.p2.id, this.sub.seg.boundary);
				}
				this.tri.orient = this.tri.orient + 1;
			}
			return true;
		}

		// Token: 0x06000DD2 RID: 3538 RVA: 0x000AC307 File Offset: 0x000AA507
		public void Reset()
		{
			this.triangles.Reset();
		}

		// Token: 0x04001ED1 RID: 7889
		private IEnumerator<Triangle> triangles;

		// Token: 0x04001ED2 RID: 7890
		private Otri tri;

		// Token: 0x04001ED3 RID: 7891
		private Otri neighbor;

		// Token: 0x04001ED4 RID: 7892
		private Osub sub;

		// Token: 0x04001ED5 RID: 7893
		private Edge current;

		// Token: 0x04001ED6 RID: 7894
		private Vertex p1;

		// Token: 0x04001ED7 RID: 7895
		private Vertex p2;
	}
}
