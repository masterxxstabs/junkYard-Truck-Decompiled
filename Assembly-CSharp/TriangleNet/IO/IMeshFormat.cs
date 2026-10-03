using System;
using System.IO;
using TriangleNet.Meshing;

namespace TriangleNet.IO
{
	// Token: 0x02000235 RID: 565
	public interface IMeshFormat : IFileFormat
	{
		// Token: 0x06000E22 RID: 3618
		IMesh Import(string filename);

		// Token: 0x06000E23 RID: 3619
		void Write(IMesh mesh, string filename);

		// Token: 0x06000E24 RID: 3620
		void Write(IMesh mesh, Stream stream);
	}
}
