using System;
using TriangleNet.Geometry;

namespace TriangleNet.Meshing
{
	// Token: 0x02000221 RID: 545
	public interface IQualityMesher
	{
		// Token: 0x06000DB3 RID: 3507
		IMesh Triangulate(IPolygon polygon, QualityOptions quality);

		// Token: 0x06000DB4 RID: 3508
		IMesh Triangulate(IPolygon polygon, ConstraintOptions options, QualityOptions quality);
	}
}
