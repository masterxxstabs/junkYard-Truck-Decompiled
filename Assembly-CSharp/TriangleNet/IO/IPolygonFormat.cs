using System;
using System.IO;
using TriangleNet.Geometry;

namespace TriangleNet.IO
{
	// Token: 0x02000236 RID: 566
	public interface IPolygonFormat : IFileFormat
	{
		// Token: 0x06000E25 RID: 3621
		IPolygon Read(string filename);

		// Token: 0x06000E26 RID: 3622
		void Write(IPolygon polygon, string filename);

		// Token: 0x06000E27 RID: 3623
		void Write(IPolygon polygon, Stream stream);
	}
}
