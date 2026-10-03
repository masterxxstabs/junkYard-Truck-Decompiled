using System;
using TriangleNet.Geometry;

namespace TriangleNet.IO
{
	// Token: 0x02000237 RID: 567
	public class InputTriangle : ITriangle
	{
		// Token: 0x06000E28 RID: 3624 RVA: 0x000AF2AC File Offset: 0x000AD4AC
		public InputTriangle(int p0, int p1, int p2)
		{
			this.vertices = new int[]
			{
				p0,
				p1,
				p2
			};
		}

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x06000E29 RID: 3625 RVA: 0x000116EA File Offset: 0x0000F8EA
		// (set) Token: 0x06000E2A RID: 3626 RVA: 0x00002188 File Offset: 0x00000388
		public int ID
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x06000E2B RID: 3627 RVA: 0x000AF2CC File Offset: 0x000AD4CC
		// (set) Token: 0x06000E2C RID: 3628 RVA: 0x000AF2D4 File Offset: 0x000AD4D4
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

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x06000E2D RID: 3629 RVA: 0x000AF2DD File Offset: 0x000AD4DD
		// (set) Token: 0x06000E2E RID: 3630 RVA: 0x000AF2E5 File Offset: 0x000AD4E5
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

		// Token: 0x06000E2F RID: 3631 RVA: 0x00018FC3 File Offset: 0x000171C3
		public Vertex GetVertex(int index)
		{
			return null;
		}

		// Token: 0x06000E30 RID: 3632 RVA: 0x000AF2EE File Offset: 0x000AD4EE
		public int GetVertexID(int index)
		{
			return this.vertices[index];
		}

		// Token: 0x06000E31 RID: 3633 RVA: 0x00018FC3 File Offset: 0x000171C3
		public ITriangle GetNeighbor(int index)
		{
			return null;
		}

		// Token: 0x06000E32 RID: 3634 RVA: 0x000AF2F8 File Offset: 0x000AD4F8
		public int GetNeighborID(int index)
		{
			return -1;
		}

		// Token: 0x06000E33 RID: 3635 RVA: 0x00018FC3 File Offset: 0x000171C3
		public ISegment GetSegment(int index)
		{
			return null;
		}

		// Token: 0x04001F05 RID: 7941
		internal int[] vertices;

		// Token: 0x04001F06 RID: 7942
		internal int label;

		// Token: 0x04001F07 RID: 7943
		internal double area;
	}
}
