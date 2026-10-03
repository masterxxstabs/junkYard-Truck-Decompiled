using System;
using TriangleNet.Meshing;

namespace TriangleNet.Smoothing
{
	// Token: 0x02000218 RID: 536
	public interface ISmoother
	{
		// Token: 0x06000D72 RID: 3442
		void Smooth(IMesh mesh);

		// Token: 0x06000D73 RID: 3443
		void Smooth(IMesh mesh, int limit);
	}
}
