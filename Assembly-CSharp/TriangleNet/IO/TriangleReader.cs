using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TriangleNet.Geometry;

namespace TriangleNet.IO
{
	// Token: 0x02000239 RID: 569
	public class TriangleReader
	{
		// Token: 0x06000E3C RID: 3644 RVA: 0x000AF451 File Offset: 0x000AD651
		public static bool IsNullOrWhiteSpace(string value)
		{
			return value == null || string.IsNullOrEmpty(value.Trim());
		}

		// Token: 0x06000E3D RID: 3645 RVA: 0x000AF464 File Offset: 0x000AD664
		private bool TryReadLine(StreamReader reader, out string[] token)
		{
			token = null;
			if (reader.EndOfStream)
			{
				return false;
			}
			string text = reader.ReadLine().Trim();
			while (TriangleReader.IsNullOrWhiteSpace(text) || text.StartsWith("#"))
			{
				if (reader.EndOfStream)
				{
					return false;
				}
				text = reader.ReadLine().Trim();
			}
			token = text.Split(new char[]
			{
				' ',
				'\t'
			}, StringSplitOptions.RemoveEmptyEntries);
			return true;
		}

		// Token: 0x06000E3E RID: 3646 RVA: 0x000AF4D4 File Offset: 0x000AD6D4
		private void ReadVertex(List<Vertex> data, int index, string[] line, int attributes, int marks)
		{
			double x = double.Parse(line[1], TriangleReader.nfi);
			double y = double.Parse(line[2], TriangleReader.nfi);
			Vertex vertex = new Vertex(x, y);
			if (marks > 0 && line.Length > 3 + attributes)
			{
				vertex.Label = int.Parse(line[3 + attributes]);
			}
			data.Add(vertex);
		}

		// Token: 0x06000E3F RID: 3647 RVA: 0x000AF530 File Offset: 0x000AD730
		public void Read(string filename, out Polygon polygon)
		{
			polygon = null;
			string text = Path.ChangeExtension(filename, ".poly");
			if (File.Exists(text))
			{
				polygon = this.ReadPolyFile(text);
				return;
			}
			text = Path.ChangeExtension(filename, ".node");
			polygon = this.ReadNodeFile(text);
		}

		// Token: 0x06000E40 RID: 3648 RVA: 0x000AF574 File Offset: 0x000AD774
		public void Read(string filename, out Polygon geometry, out List<ITriangle> triangles)
		{
			triangles = null;
			this.Read(filename, out geometry);
			string text = Path.ChangeExtension(filename, ".ele");
			if (File.Exists(text) && geometry != null)
			{
				triangles = this.ReadEleFile(text);
			}
		}

		// Token: 0x06000E41 RID: 3649 RVA: 0x000AF5B0 File Offset: 0x000AD7B0
		public IPolygon Read(string filename)
		{
			Polygon result = null;
			this.Read(filename, out result);
			return result;
		}

		// Token: 0x06000E42 RID: 3650 RVA: 0x000AF5C9 File Offset: 0x000AD7C9
		public Polygon ReadNodeFile(string nodefilename)
		{
			return this.ReadNodeFile(nodefilename, false);
		}

		// Token: 0x06000E43 RID: 3651 RVA: 0x000AF5D4 File Offset: 0x000AD7D4
		public Polygon ReadNodeFile(string nodefilename, bool readElements)
		{
			this.startIndex = 0;
			int attributes = 0;
			int marks = 0;
			Polygon polygon;
			using (StreamReader streamReader = new StreamReader(nodefilename))
			{
				string[] array;
				if (!this.TryReadLine(streamReader, out array))
				{
					throw new Exception("Can't read input file.");
				}
				int num = int.Parse(array[0]);
				if (num < 3)
				{
					throw new Exception("Input must have at least three input vertices.");
				}
				if (array.Length > 1 && int.Parse(array[1]) != 2)
				{
					throw new Exception("Triangle only works with two-dimensional meshes.");
				}
				if (array.Length > 2)
				{
					attributes = int.Parse(array[2]);
				}
				if (array.Length > 3)
				{
					marks = int.Parse(array[3]);
				}
				polygon = new Polygon(num);
				if (num > 0)
				{
					for (int i = 0; i < num; i++)
					{
						if (!this.TryReadLine(streamReader, out array))
						{
							throw new Exception("Can't read input file (vertices).");
						}
						if (array.Length < 3)
						{
							throw new Exception("Invalid vertex.");
						}
						if (i == 0)
						{
							this.startIndex = int.Parse(array[0], TriangleReader.nfi);
						}
						this.ReadVertex(polygon.Points, i, array, attributes, marks);
					}
				}
			}
			if (readElements)
			{
				string text = Path.ChangeExtension(nodefilename, ".ele");
				if (File.Exists(text))
				{
					this.ReadEleFile(text, true);
				}
			}
			return polygon;
		}

		// Token: 0x06000E44 RID: 3652 RVA: 0x000AF710 File Offset: 0x000AD910
		public Polygon ReadPolyFile(string polyfilename)
		{
			return this.ReadPolyFile(polyfilename, false, false);
		}

		// Token: 0x06000E45 RID: 3653 RVA: 0x000AF71B File Offset: 0x000AD91B
		public Polygon ReadPolyFile(string polyfilename, bool readElements)
		{
			return this.ReadPolyFile(polyfilename, readElements, false);
		}

		// Token: 0x06000E46 RID: 3654 RVA: 0x000AF728 File Offset: 0x000AD928
		public Polygon ReadPolyFile(string polyfilename, bool readElements, bool readArea)
		{
			this.startIndex = 0;
			int attributes = 0;
			int marks = 0;
			Polygon polygon;
			using (StreamReader streamReader = new StreamReader(polyfilename))
			{
				string[] array;
				if (!this.TryReadLine(streamReader, out array))
				{
					throw new Exception("Can't read input file.");
				}
				int num = int.Parse(array[0]);
				if (array.Length > 1 && int.Parse(array[1]) != 2)
				{
					throw new Exception("Triangle only works with two-dimensional meshes.");
				}
				if (array.Length > 2)
				{
					attributes = int.Parse(array[2]);
				}
				if (array.Length > 3)
				{
					marks = int.Parse(array[3]);
				}
				if (num > 0)
				{
					polygon = new Polygon(num);
					for (int i = 0; i < num; i++)
					{
						if (!this.TryReadLine(streamReader, out array))
						{
							throw new Exception("Can't read input file (vertices).");
						}
						if (array.Length < 3)
						{
							throw new Exception("Invalid vertex.");
						}
						if (i == 0)
						{
							this.startIndex = int.Parse(array[0], TriangleReader.nfi);
						}
						this.ReadVertex(polygon.Points, i, array, attributes, marks);
					}
				}
				else
				{
					polygon = this.ReadNodeFile(Path.ChangeExtension(polyfilename, ".node"));
					num = polygon.Points.Count;
				}
				List<Vertex> points = polygon.Points;
				if (points.Count == 0)
				{
					throw new Exception("No nodes available.");
				}
				if (!this.TryReadLine(streamReader, out array))
				{
					throw new Exception("Can't read input file (segments).");
				}
				int num2 = int.Parse(array[0]);
				int num3 = 0;
				if (array.Length > 1)
				{
					num3 = int.Parse(array[1]);
				}
				for (int j = 0; j < num2; j++)
				{
					if (!this.TryReadLine(streamReader, out array))
					{
						throw new Exception("Can't read input file (segments).");
					}
					if (array.Length < 3)
					{
						throw new Exception("Segment has no endpoints.");
					}
					int num4 = int.Parse(array[1]) - this.startIndex;
					int num5 = int.Parse(array[2]) - this.startIndex;
					int label = 0;
					if (num3 > 0 && array.Length > 3)
					{
						label = int.Parse(array[3]);
					}
					if (num4 < 0 || num4 >= num)
					{
						if (Log.Verbose)
						{
							Log.Instance.Warning("Invalid first endpoint of segment.", "MeshReader.ReadPolyfile()");
						}
					}
					else if (num5 < 0 || num5 >= num)
					{
						if (Log.Verbose)
						{
							Log.Instance.Warning("Invalid second endpoint of segment.", "MeshReader.ReadPolyfile()");
						}
					}
					else
					{
						polygon.Add(new Segment(points[num4], points[num5], label), false);
					}
				}
				if (!this.TryReadLine(streamReader, out array))
				{
					throw new Exception("Can't read input file (holes).");
				}
				int num6 = int.Parse(array[0]);
				if (num6 > 0)
				{
					for (int k = 0; k < num6; k++)
					{
						if (!this.TryReadLine(streamReader, out array))
						{
							throw new Exception("Can't read input file (holes).");
						}
						if (array.Length < 3)
						{
							throw new Exception("Invalid hole.");
						}
						polygon.Holes.Add(new Point(double.Parse(array[1], TriangleReader.nfi), double.Parse(array[2], TriangleReader.nfi)));
					}
				}
				if (this.TryReadLine(streamReader, out array))
				{
					int num7 = int.Parse(array[0]);
					if (num7 > 0)
					{
						for (int l = 0; l < num7; l++)
						{
							if (!this.TryReadLine(streamReader, out array))
							{
								throw new Exception("Can't read input file (region).");
							}
							if (array.Length < 4)
							{
								throw new Exception("Invalid region attributes.");
							}
							int id;
							if (!int.TryParse(array[3], out id))
							{
								id = l;
							}
							double area = 0.0;
							if (array.Length > 4)
							{
								double.TryParse(array[4], NumberStyles.Number, TriangleReader.nfi, out area);
							}
							polygon.Regions.Add(new RegionPointer(double.Parse(array[1], TriangleReader.nfi), double.Parse(array[2], TriangleReader.nfi), id, area));
						}
					}
				}
			}
			if (readElements)
			{
				string text = Path.ChangeExtension(polyfilename, ".ele");
				if (File.Exists(text))
				{
					this.ReadEleFile(text, readArea);
				}
			}
			return polygon;
		}

		// Token: 0x06000E47 RID: 3655 RVA: 0x000AFB04 File Offset: 0x000ADD04
		public List<ITriangle> ReadEleFile(string elefilename)
		{
			return this.ReadEleFile(elefilename, false);
		}

		// Token: 0x06000E48 RID: 3656 RVA: 0x000AFB10 File Offset: 0x000ADD10
		private List<ITriangle> ReadEleFile(string elefilename, bool readArea)
		{
			int num = 0;
			List<ITriangle> list;
			using (StreamReader streamReader = new StreamReader(elefilename))
			{
				bool flag = false;
				string[] array;
				if (!this.TryReadLine(streamReader, out array))
				{
					throw new Exception("Can't read input file (elements).");
				}
				num = int.Parse(array[0]);
				int num2 = 0;
				if (array.Length > 2)
				{
					num2 = int.Parse(array[2]);
					flag = true;
				}
				if (num2 > 1)
				{
					Log.Instance.Warning("Triangle attributes not supported.", "FileReader.Read");
				}
				list = new List<ITriangle>(num);
				for (int i = 0; i < num; i++)
				{
					if (!this.TryReadLine(streamReader, out array))
					{
						throw new Exception("Can't read input file (elements).");
					}
					if (array.Length < 4)
					{
						throw new Exception("Triangle has no nodes.");
					}
					InputTriangle inputTriangle = new InputTriangle(int.Parse(array[1]) - this.startIndex, int.Parse(array[2]) - this.startIndex, int.Parse(array[3]) - this.startIndex);
					if (num2 > 0 && flag)
					{
						int label = 0;
						flag = int.TryParse(array[4], out label);
						inputTriangle.label = label;
					}
					list.Add(inputTriangle);
				}
			}
			if (readArea)
			{
				string text = Path.ChangeExtension(elefilename, ".area");
				if (File.Exists(text))
				{
					this.ReadAreaFile(text, num);
				}
			}
			return list;
		}

		// Token: 0x06000E49 RID: 3657 RVA: 0x000AFC60 File Offset: 0x000ADE60
		private double[] ReadAreaFile(string areafilename, int intriangles)
		{
			double[] array = null;
			using (StreamReader streamReader = new StreamReader(areafilename))
			{
				string[] array2;
				if (!this.TryReadLine(streamReader, out array2))
				{
					throw new Exception("Can't read input file (area).");
				}
				if (int.Parse(array2[0]) != intriangles)
				{
					Log.Instance.Warning("Number of area constraints doesn't match number of triangles.", "ReadAreaFile()");
					return null;
				}
				array = new double[intriangles];
				for (int i = 0; i < intriangles; i++)
				{
					if (!this.TryReadLine(streamReader, out array2))
					{
						throw new Exception("Can't read input file (area).");
					}
					if (array2.Length != 2)
					{
						throw new Exception("Triangle has no nodes.");
					}
					array[i] = double.Parse(array2[1], TriangleReader.nfi);
				}
			}
			return array;
		}

		// Token: 0x06000E4A RID: 3658 RVA: 0x000AFD20 File Offset: 0x000ADF20
		public List<Edge> ReadEdgeFile(string edgeFile, int invertices)
		{
			List<Edge> list = null;
			this.startIndex = 0;
			using (StreamReader streamReader = new StreamReader(edgeFile))
			{
				string[] array;
				if (!this.TryReadLine(streamReader, out array))
				{
					throw new Exception("Can't read input file (segments).");
				}
				int num = int.Parse(array[0]);
				int num2 = 0;
				if (array.Length > 1)
				{
					num2 = int.Parse(array[1]);
				}
				if (num > 0)
				{
					list = new List<Edge>(num);
				}
				for (int i = 0; i < num; i++)
				{
					if (!this.TryReadLine(streamReader, out array))
					{
						throw new Exception("Can't read input file (segments).");
					}
					if (array.Length < 3)
					{
						throw new Exception("Segment has no endpoints.");
					}
					int num3 = int.Parse(array[1]) - this.startIndex;
					int num4 = int.Parse(array[2]) - this.startIndex;
					int label = 0;
					if (num2 > 0 && array.Length > 3)
					{
						label = int.Parse(array[3]);
					}
					if (num3 < 0 || num3 >= invertices)
					{
						if (Log.Verbose)
						{
							Log.Instance.Warning("Invalid first endpoint of segment.", "MeshReader.ReadPolyfile()");
						}
					}
					else if (num4 < 0 || num4 >= invertices)
					{
						if (Log.Verbose)
						{
							Log.Instance.Warning("Invalid second endpoint of segment.", "MeshReader.ReadPolyfile()");
						}
					}
					else
					{
						list.Add(new Edge(num3, num4, label));
					}
				}
			}
			return list;
		}

		// Token: 0x04001F08 RID: 7944
		private static NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		// Token: 0x04001F09 RID: 7945
		private int startIndex;
	}
}
