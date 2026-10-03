using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.IO;
using TriangleNet.Meshing.Algorithm;

namespace TriangleNet.Meshing
{
	// Token: 0x0200021E RID: 542
	public class GenericMesher
	{
		// Token: 0x06000D9E RID: 3486 RVA: 0x000AA8F0 File Offset: 0x000A8AF0
		public GenericMesher() : this(new Dwyer(), new Configuration())
		{
		}

		// Token: 0x06000D9F RID: 3487 RVA: 0x000AA902 File Offset: 0x000A8B02
		public GenericMesher(ITriangulator triangulator) : this(triangulator, new Configuration())
		{
		}

		// Token: 0x06000DA0 RID: 3488 RVA: 0x000AA910 File Offset: 0x000A8B10
		public GenericMesher(Configuration config) : this(new Dwyer(), config)
		{
		}

		// Token: 0x06000DA1 RID: 3489 RVA: 0x000AA91E File Offset: 0x000A8B1E
		public GenericMesher(ITriangulator triangulator, Configuration config)
		{
			this.config = config;
			this.triangulator = triangulator;
		}

		// Token: 0x06000DA2 RID: 3490 RVA: 0x000AA934 File Offset: 0x000A8B34
		public IMesh Triangulate(IList<Vertex> points)
		{
			return this.triangulator.Triangulate(points, this.config);
		}

		// Token: 0x06000DA3 RID: 3491 RVA: 0x000AA948 File Offset: 0x000A8B48
		public IMesh Triangulate(IPolygon polygon)
		{
			return this.Triangulate(polygon, null, null);
		}

		// Token: 0x06000DA4 RID: 3492 RVA: 0x000AA953 File Offset: 0x000A8B53
		public IMesh Triangulate(IPolygon polygon, ConstraintOptions options)
		{
			return this.Triangulate(polygon, options, null);
		}

		// Token: 0x06000DA5 RID: 3493 RVA: 0x000AA95E File Offset: 0x000A8B5E
		public IMesh Triangulate(IPolygon polygon, QualityOptions quality)
		{
			return this.Triangulate(polygon, null, quality);
		}

		// Token: 0x06000DA6 RID: 3494 RVA: 0x000AA96C File Offset: 0x000A8B6C
		public IMesh Triangulate(IPolygon polygon, ConstraintOptions options, QualityOptions quality)
		{
			Mesh mesh = (Mesh)this.triangulator.Triangulate(polygon.Points, this.config);
			ConstraintMesher constraintMesher = new ConstraintMesher(mesh, this.config);
			QualityMesher qualityMesher = new QualityMesher(mesh, this.config);
			mesh.SetQualityMesher(qualityMesher);
			constraintMesher.Apply(polygon, options);
			qualityMesher.Apply(quality, false);
			return mesh;
		}

		// Token: 0x06000DA7 RID: 3495 RVA: 0x000AA9C8 File Offset: 0x000A8BC8
		public static IMesh StructuredMesh(double width, double height, int nx, int ny)
		{
			if (width <= 0.0)
			{
				throw new ArgumentException("width");
			}
			if (height <= 0.0)
			{
				throw new ArgumentException("height");
			}
			return GenericMesher.StructuredMesh(new Rectangle(0.0, 0.0, width, height), nx, ny);
		}

		// Token: 0x06000DA8 RID: 3496 RVA: 0x000AAA24 File Offset: 0x000A8C24
		public static IMesh StructuredMesh(Rectangle bounds, int nx, int ny)
		{
			Polygon polygon = new Polygon((nx + 1) * (ny + 1));
			double num = bounds.Width / (double)nx;
			double num2 = bounds.Height / (double)ny;
			double left = bounds.Left;
			double bottom = bounds.Bottom;
			int num3 = 0;
			Vertex[] array = new Vertex[(nx + 1) * (ny + 1)];
			for (int i = 0; i <= nx; i++)
			{
				double x = left + (double)i * num;
				for (int j = 0; j <= ny; j++)
				{
					double y = bottom + (double)j * num2;
					array[num3++] = new Vertex(x, y);
				}
			}
			polygon.Points.AddRange(array);
			num3 = 0;
			foreach (Vertex vertex in array)
			{
				vertex.hash = (vertex.id = num3++);
			}
			List<ISegment> segments = polygon.Segments;
			segments.Capacity = 2 * (nx + ny);
			for (int j = 0; j < ny; j++)
			{
				Vertex vertex2 = array[j];
				Vertex vertex3 = array[j + 1];
				segments.Add(new Segment(vertex2, vertex3, 1));
				vertex2.Label = (vertex3.Label = 1);
				vertex2 = array[nx * (ny + 1) + j];
				vertex3 = array[nx * (ny + 1) + (j + 1)];
				segments.Add(new Segment(vertex2, vertex3, 1));
				vertex2.Label = (vertex3.Label = 1);
			}
			for (int i = 0; i < nx; i++)
			{
				Vertex vertex2 = array[(ny + 1) * i];
				Vertex vertex3 = array[(ny + 1) * (i + 1)];
				segments.Add(new Segment(vertex2, vertex3, 1));
				vertex2.Label = (vertex3.Label = 1);
				vertex2 = array[ny + (ny + 1) * i];
				vertex3 = array[ny + (ny + 1) * (i + 1)];
				segments.Add(new Segment(vertex2, vertex3, 1));
				vertex2.Label = (vertex3.Label = 1);
			}
			InputTriangle[] array3 = new InputTriangle[2 * nx * ny];
			num3 = 0;
			for (int i = 0; i < nx; i++)
			{
				for (int j = 0; j < ny; j++)
				{
					int num4 = j + (ny + 1) * i;
					int num5 = j + (ny + 1) * (i + 1);
					if ((i + j) % 2 == 0)
					{
						array3[num3++] = new InputTriangle(num4, num5, num5 + 1);
						array3[num3++] = new InputTriangle(num4, num5 + 1, num4 + 1);
					}
					else
					{
						array3[num3++] = new InputTriangle(num4, num5, num4 + 1);
						array3[num3++] = new InputTriangle(num5, num5 + 1, num4 + 1);
					}
				}
			}
			Polygon polygon2 = polygon;
			ITriangle[] triangles = array3;
			return Converter.ToMesh(polygon2, triangles);
		}

		// Token: 0x04001EC1 RID: 7873
		private Configuration config;

		// Token: 0x04001EC2 RID: 7874
		private ITriangulator triangulator;
	}
}
