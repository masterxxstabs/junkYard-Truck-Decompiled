using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Meshing;

namespace TriangleNet.IO
{
	// Token: 0x02000233 RID: 563
	public static class FileProcessor
	{
		// Token: 0x06000E1A RID: 3610 RVA: 0x000AF058 File Offset: 0x000AD258
		static FileProcessor()
		{
			FileProcessor.formats.Add(new TriangleFormat());
		}

		// Token: 0x06000E1B RID: 3611 RVA: 0x000AF073 File Offset: 0x000AD273
		public static void Add(IFileFormat format)
		{
			FileProcessor.formats.Add(format);
		}

		// Token: 0x06000E1C RID: 3612 RVA: 0x000AF080 File Offset: 0x000AD280
		public static bool IsSupported(string file)
		{
			using (List<IFileFormat>.Enumerator enumerator = FileProcessor.formats.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsSupported(file))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000E1D RID: 3613 RVA: 0x000AF0DC File Offset: 0x000AD2DC
		public static IPolygon Read(string filename)
		{
			foreach (IFileFormat fileFormat in FileProcessor.formats)
			{
				IPolygonFormat polygonFormat = (IPolygonFormat)fileFormat;
				if (polygonFormat != null && polygonFormat.IsSupported(filename))
				{
					return polygonFormat.Read(filename);
				}
			}
			throw new Exception("File format not supported.");
		}

		// Token: 0x06000E1E RID: 3614 RVA: 0x000AF150 File Offset: 0x000AD350
		public static void Write(IPolygon polygon, string filename)
		{
			foreach (IFileFormat fileFormat in FileProcessor.formats)
			{
				IPolygonFormat polygonFormat = (IPolygonFormat)fileFormat;
				if (polygonFormat != null && polygonFormat.IsSupported(filename))
				{
					polygonFormat.Write(polygon, filename);
					return;
				}
			}
			throw new Exception("File format not supported.");
		}

		// Token: 0x06000E1F RID: 3615 RVA: 0x000AF1C4 File Offset: 0x000AD3C4
		public static IMesh Import(string filename)
		{
			foreach (IFileFormat fileFormat in FileProcessor.formats)
			{
				IMeshFormat meshFormat = (IMeshFormat)fileFormat;
				if (meshFormat != null && meshFormat.IsSupported(filename))
				{
					return meshFormat.Import(filename);
				}
			}
			throw new Exception("File format not supported.");
		}

		// Token: 0x06000E20 RID: 3616 RVA: 0x000AF238 File Offset: 0x000AD438
		public static void Write(IMesh mesh, string filename)
		{
			foreach (IFileFormat fileFormat in FileProcessor.formats)
			{
				IMeshFormat meshFormat = (IMeshFormat)fileFormat;
				if (meshFormat != null && meshFormat.IsSupported(filename))
				{
					meshFormat.Write(mesh, filename);
					return;
				}
			}
			throw new Exception("File format not supported.");
		}

		// Token: 0x04001F04 RID: 7940
		private static List<IFileFormat> formats = new List<IFileFormat>();
	}
}
