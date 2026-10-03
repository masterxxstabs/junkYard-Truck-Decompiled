using System;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Meshing.Data
{
	// Token: 0x02000228 RID: 552
	internal class BadSubseg
	{
		// Token: 0x06000DDB RID: 3547 RVA: 0x000AC624 File Offset: 0x000AA824
		public override int GetHashCode()
		{
			return this.subseg.seg.hash;
		}

		// Token: 0x06000DDC RID: 3548 RVA: 0x000AC636 File Offset: 0x000AA836
		public override string ToString()
		{
			return string.Format("B-SID {0}", this.subseg.seg.hash);
		}

		// Token: 0x04001EDA RID: 7898
		public Osub subseg;

		// Token: 0x04001EDB RID: 7899
		public Vertex org;

		// Token: 0x04001EDC RID: 7900
		public Vertex dest;
	}
}
