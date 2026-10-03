using System;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Meshing.Data
{
	// Token: 0x0200022A RID: 554
	internal class BadTriangle
	{
		// Token: 0x06000DE3 RID: 3555 RVA: 0x000AC8AB File Offset: 0x000AAAAB
		public override string ToString()
		{
			return string.Format("B-TID {0}", this.poortri.tri.hash);
		}

		// Token: 0x04001EE3 RID: 7907
		public Otri poortri;

		// Token: 0x04001EE4 RID: 7908
		public double key;

		// Token: 0x04001EE5 RID: 7909
		public Vertex org;

		// Token: 0x04001EE6 RID: 7910
		public Vertex dest;

		// Token: 0x04001EE7 RID: 7911
		public Vertex apex;

		// Token: 0x04001EE8 RID: 7912
		public BadTriangle next;
	}
}
