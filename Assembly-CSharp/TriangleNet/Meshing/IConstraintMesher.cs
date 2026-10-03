using System;
using TriangleNet.Geometry;

namespace TriangleNet.Meshing
{
	// Token: 0x0200021F RID: 543
	public interface IConstraintMesher
	{
		// Token: 0x06000DA9 RID: 3497
		IMesh Triangulate(IPolygon polygon);

		// Token: 0x06000DAA RID: 3498
		IMesh Triangulate(IPolygon polygon, ConstraintOptions options);
	}
}
