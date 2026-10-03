using System;
using TriangleNet.Topology;

namespace TriangleNet.Geometry
{
	// Token: 0x02000247 RID: 583
	public class Vertex : Point
	{
		// Token: 0x06000ED9 RID: 3801 RVA: 0x000B1A3E File Offset: 0x000AFC3E
		public Vertex() : this(0.0, 0.0, 0)
		{
		}

		// Token: 0x06000EDA RID: 3802 RVA: 0x000B1A59 File Offset: 0x000AFC59
		public Vertex(double x, double y) : this(x, y, 0)
		{
		}

		// Token: 0x06000EDB RID: 3803 RVA: 0x000B1A64 File Offset: 0x000AFC64
		public Vertex(double x, double y, int mark) : base(x, y, mark)
		{
			this.type = VertexType.InputVertex;
		}

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06000EDC RID: 3804 RVA: 0x000B1A76 File Offset: 0x000AFC76
		public VertexType Type
		{
			get
			{
				return this.type;
			}
		}

		// Token: 0x1700012E RID: 302
		public double this[int i]
		{
			get
			{
				if (i == 0)
				{
					return this.x;
				}
				if (i == 1)
				{
					return this.y;
				}
				throw new ArgumentOutOfRangeException("Index must be 0 or 1.");
			}
		}

		// Token: 0x06000EDE RID: 3806 RVA: 0x000B1A9F File Offset: 0x000AFC9F
		public override int GetHashCode()
		{
			return this.hash;
		}

		// Token: 0x04001F26 RID: 7974
		internal int hash;

		// Token: 0x04001F27 RID: 7975
		internal VertexType type;

		// Token: 0x04001F28 RID: 7976
		internal Otri tri;
	}
}
