using System;

namespace TriangleNet.Geometry
{
	// Token: 0x02000240 RID: 576
	public interface ISegment : IEdge
	{
		// Token: 0x06000E87 RID: 3719
		Vertex GetVertex(int index);

		// Token: 0x06000E88 RID: 3720
		ITriangle GetTriangle(int index);
	}
}
