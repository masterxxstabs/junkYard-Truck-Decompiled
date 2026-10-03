using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.IO
{
	// Token: 0x02000232 RID: 562
	internal class DebugWriter
	{
		// Token: 0x06000E10 RID: 3600 RVA: 0x00012D9B File Offset: 0x00010F9B
		private DebugWriter()
		{
		}

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x06000E11 RID: 3601 RVA: 0x000AE9E2 File Offset: 0x000ACBE2
		public static DebugWriter Session
		{
			get
			{
				return DebugWriter.instance;
			}
		}

		// Token: 0x06000E12 RID: 3602 RVA: 0x000AE9E9 File Offset: 0x000ACBE9
		public void Start(string session)
		{
			this.iteration = 0;
			this.session = session;
			if (this.stream != null)
			{
				throw new Exception("A session is active. Finish before starting a new.");
			}
			this.tmpFile = Path.GetTempFileName();
			this.stream = new StreamWriter(this.tmpFile);
		}

		// Token: 0x06000E13 RID: 3603 RVA: 0x000AEA28 File Offset: 0x000ACC28
		public void Write(Mesh mesh, bool skip = false)
		{
			this.WriteMesh(mesh, skip);
			this.triangles = mesh.Triangles.Count;
		}

		// Token: 0x06000E14 RID: 3604 RVA: 0x000AEA43 File Offset: 0x000ACC43
		public void Finish()
		{
			this.Finish(this.session + ".mshx");
		}

		// Token: 0x06000E15 RID: 3605 RVA: 0x000AEA5C File Offset: 0x000ACC5C
		private void Finish(string path)
		{
			if (this.stream != null)
			{
				this.stream.Flush();
				this.stream.Dispose();
				this.stream = null;
				string s = "#!N" + this.iteration + Environment.NewLine;
				using (FileStream fileStream = new FileStream(path, FileMode.Create))
				{
					using (GZipStream gzipStream = new GZipStream(fileStream, CompressionMode.Compress, false))
					{
						byte[] array = Encoding.UTF8.GetBytes(s);
						gzipStream.Write(array, 0, array.Length);
						array = File.ReadAllBytes(this.tmpFile);
						gzipStream.Write(array, 0, array.Length);
					}
				}
				File.Delete(this.tmpFile);
			}
		}

		// Token: 0x06000E16 RID: 3606 RVA: 0x000AEB28 File Offset: 0x000ACD28
		private void WriteGeometry(IPolygon geometry)
		{
			TextWriter textWriter = this.stream;
			string format = "#!G{0}";
			int num = this.iteration;
			this.iteration = num + 1;
			textWriter.WriteLine(format, num);
		}

		// Token: 0x06000E17 RID: 3607 RVA: 0x000AEB5C File Offset: 0x000ACD5C
		private void WriteMesh(Mesh mesh, bool skip)
		{
			if (this.triangles == mesh.triangles.Count && skip)
			{
				return;
			}
			TextWriter textWriter = this.stream;
			string format = "#!M{0}";
			int num = this.iteration;
			this.iteration = num + 1;
			textWriter.WriteLine(format, num);
			if (this.VerticesChanged(mesh))
			{
				this.HashVertices(mesh);
				this.stream.WriteLine("{0}", mesh.vertices.Count);
				using (Dictionary<int, Vertex>.ValueCollection.Enumerator enumerator = mesh.vertices.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Vertex vertex = enumerator.Current;
						this.stream.WriteLine("{0} {1} {2} {3}", new object[]
						{
							vertex.id,
							vertex.x.ToString(DebugWriter.nfi),
							vertex.y.ToString(DebugWriter.nfi),
							vertex.label
						});
					}
					goto IL_116;
				}
			}
			this.stream.WriteLine("0");
			IL_116:
			this.stream.WriteLine("{0}", mesh.subsegs.Count);
			Osub osub = default(Osub);
			osub.orient = 0;
			foreach (SubSegment subSegment in mesh.subsegs.Values)
			{
				if (subSegment.hash > 0)
				{
					osub.seg = subSegment;
					Vertex vertex2 = osub.Org();
					Vertex vertex3 = osub.Dest();
					this.stream.WriteLine("{0} {1} {2} {3}", new object[]
					{
						osub.seg.hash,
						vertex2.id,
						vertex3.id,
						osub.seg.boundary
					});
				}
			}
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			otri.orient = 0;
			this.stream.WriteLine("{0}", mesh.triangles.Count);
			foreach (Triangle tri in mesh.triangles)
			{
				otri.tri = tri;
				Vertex vertex2 = otri.Org();
				Vertex vertex3 = otri.Dest();
				Vertex vertex4 = otri.Apex();
				int num2 = (vertex2 == null) ? -1 : vertex2.id;
				int num3 = (vertex3 == null) ? -1 : vertex3.id;
				int num4 = (vertex4 == null) ? -1 : vertex4.id;
				this.stream.Write("{0} {1} {2} {3}", new object[]
				{
					otri.tri.hash,
					num2,
					num3,
					num4
				});
				otri.orient = 1;
				otri.Sym(ref otri2);
				int hash = otri2.tri.hash;
				otri.orient = 2;
				otri.Sym(ref otri2);
				int hash2 = otri2.tri.hash;
				otri.orient = 0;
				otri.Sym(ref otri2);
				int hash3 = otri2.tri.hash;
				this.stream.WriteLine(" {0} {1} {2}", hash, hash2, hash3);
			}
		}

		// Token: 0x06000E18 RID: 3608 RVA: 0x000AEF48 File Offset: 0x000AD148
		private bool VerticesChanged(Mesh mesh)
		{
			if (this.vertices == null || mesh.Vertices.Count != this.vertices.Length)
			{
				return true;
			}
			int num = 0;
			using (IEnumerator<Vertex> enumerator = mesh.Vertices.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.id != this.vertices[num++])
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000E19 RID: 3609 RVA: 0x000AEFC8 File Offset: 0x000AD1C8
		private void HashVertices(Mesh mesh)
		{
			if (this.vertices == null || mesh.Vertices.Count != this.vertices.Length)
			{
				this.vertices = new int[mesh.Vertices.Count];
			}
			int num = 0;
			foreach (Vertex vertex in mesh.Vertices)
			{
				this.vertices[num++] = vertex.id;
			}
		}

		// Token: 0x04001EFC RID: 7932
		private static NumberFormatInfo nfi = CultureInfo.InvariantCulture.NumberFormat;

		// Token: 0x04001EFD RID: 7933
		private int iteration;

		// Token: 0x04001EFE RID: 7934
		private string session;

		// Token: 0x04001EFF RID: 7935
		private StreamWriter stream;

		// Token: 0x04001F00 RID: 7936
		private string tmpFile;

		// Token: 0x04001F01 RID: 7937
		private int[] vertices;

		// Token: 0x04001F02 RID: 7938
		private int triangles;

		// Token: 0x04001F03 RID: 7939
		private static readonly DebugWriter instance = new DebugWriter();
	}
}
