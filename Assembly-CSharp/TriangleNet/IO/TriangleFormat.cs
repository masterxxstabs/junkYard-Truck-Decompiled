using System;
using System.Collections.Generic;
using System.IO;
using TriangleNet.Geometry;
using TriangleNet.Meshing;

namespace TriangleNet.IO
{
	// Token: 0x02000238 RID: 568
	public class TriangleFormat : IPolygonFormat, IFileFormat, IMeshFormat
	{
		// Token: 0x06000E34 RID: 3636 RVA: 0x000AF2FC File Offset: 0x000AD4FC
		public bool IsSupported(string file)
		{
			string a = Path.GetExtension(file).ToLower();
			return a == ".node" || a == ".poly" || a == ".ele";
		}

		// Token: 0x06000E35 RID: 3637 RVA: 0x000AF340 File Offset: 0x000AD540
		public IMesh Import(string filename)
		{
			string extension = Path.GetExtension(filename);
			if (extension == ".node" || extension == ".poly" || extension == ".ele")
			{
				Polygon polygon;
				List<ITriangle> list;
				new TriangleReader().Read(filename, out polygon, out list);
				if (polygon != null && list != null)
				{
					return Converter.ToMesh(polygon, list.ToArray());
				}
			}
			throw new NotSupportedException("Could not load '" + filename + "' file.");
		}

		// Token: 0x06000E36 RID: 3638 RVA: 0x000AF3B2 File Offset: 0x000AD5B2
		public void Write(IMesh mesh, string filename)
		{
			TriangleWriter triangleWriter = new TriangleWriter();
			triangleWriter.WritePoly((Mesh)mesh, Path.ChangeExtension(filename, ".poly"));
			triangleWriter.WriteElements((Mesh)mesh, Path.ChangeExtension(filename, ".ele"));
		}

		// Token: 0x06000E37 RID: 3639 RVA: 0x000A22BF File Offset: 0x000A04BF
		public void Write(IMesh mesh, Stream stream)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000E38 RID: 3640 RVA: 0x000AF3E8 File Offset: 0x000AD5E8
		public IPolygon Read(string filename)
		{
			string extension = Path.GetExtension(filename);
			if (extension == ".node")
			{
				return new TriangleReader().ReadNodeFile(filename);
			}
			if (extension == ".poly")
			{
				return new TriangleReader().ReadPolyFile(filename);
			}
			throw new NotSupportedException("File format '" + extension + "' not supported.");
		}

		// Token: 0x06000E39 RID: 3641 RVA: 0x000AF443 File Offset: 0x000AD643
		public void Write(IPolygon polygon, string filename)
		{
			new TriangleWriter().WritePoly(polygon, filename);
		}

		// Token: 0x06000E3A RID: 3642 RVA: 0x000A22BF File Offset: 0x000A04BF
		public void Write(IPolygon polygon, Stream stream)
		{
			throw new NotImplementedException();
		}
	}
}
