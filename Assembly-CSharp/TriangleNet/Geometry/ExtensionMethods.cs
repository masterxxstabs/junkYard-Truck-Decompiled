using System;
using TriangleNet.Meshing;

namespace TriangleNet.Geometry
{
	// Token: 0x0200023D RID: 573
	public static class ExtensionMethods
	{
		// Token: 0x06000E6B RID: 3691 RVA: 0x000B114D File Offset: 0x000AF34D
		public static IMesh Triangulate(this IPolygon polygon)
		{
			return new GenericMesher().Triangulate(polygon, null, null);
		}

		// Token: 0x06000E6C RID: 3692 RVA: 0x000B115C File Offset: 0x000AF35C
		public static IMesh Triangulate(this IPolygon polygon, ConstraintOptions options)
		{
			return new GenericMesher().Triangulate(polygon, options, null);
		}

		// Token: 0x06000E6D RID: 3693 RVA: 0x000B116B File Offset: 0x000AF36B
		public static IMesh Triangulate(this IPolygon polygon, QualityOptions quality)
		{
			return new GenericMesher().Triangulate(polygon, null, quality);
		}

		// Token: 0x06000E6E RID: 3694 RVA: 0x000B117A File Offset: 0x000AF37A
		public static IMesh Triangulate(this IPolygon polygon, ConstraintOptions options, QualityOptions quality)
		{
			return new GenericMesher().Triangulate(polygon, options, quality);
		}

		// Token: 0x06000E6F RID: 3695 RVA: 0x000B1189 File Offset: 0x000AF389
		public static IMesh Triangulate(this IPolygon polygon, ConstraintOptions options, QualityOptions quality, ITriangulator triangulator)
		{
			return new GenericMesher(triangulator).Triangulate(polygon, options, quality);
		}

		// Token: 0x06000E70 RID: 3696 RVA: 0x000B1199 File Offset: 0x000AF399
		public static bool Contains(this ITriangle triangle, Point p)
		{
			return triangle.Contains(p.X, p.Y);
		}

		// Token: 0x06000E71 RID: 3697 RVA: 0x000B11B0 File Offset: 0x000AF3B0
		public static bool Contains(this ITriangle triangle, double x, double y)
		{
			Vertex vertex = triangle.GetVertex(0);
			Vertex vertex2 = triangle.GetVertex(1);
			Vertex vertex3 = triangle.GetVertex(2);
			Point point = new Point(vertex2.X - vertex.X, vertex2.Y - vertex.Y);
			Point point2 = new Point(vertex3.X - vertex.X, vertex3.Y - vertex.Y);
			Point p = new Point(x - vertex.X, y - vertex.Y);
			Point q = new Point(-point.Y, point.X);
			Point q2 = new Point(-point2.Y, point2.X);
			double num = ExtensionMethods.DotProduct(p, q2) / ExtensionMethods.DotProduct(point, q2);
			double num2 = ExtensionMethods.DotProduct(p, q) / ExtensionMethods.DotProduct(point2, q);
			return num >= 0.0 && num2 >= 0.0 && num + num2 <= 1.0;
		}

		// Token: 0x06000E72 RID: 3698 RVA: 0x000B12A8 File Offset: 0x000AF4A8
		public static Rectangle Bounds(this ITriangle triangle)
		{
			Rectangle rectangle = new Rectangle();
			for (int i = 0; i < 3; i++)
			{
				rectangle.Expand(triangle.GetVertex(i));
			}
			return rectangle;
		}

		// Token: 0x06000E73 RID: 3699 RVA: 0x000B12D5 File Offset: 0x000AF4D5
		internal static double DotProduct(Point p, Point q)
		{
			return p.X * q.X + p.Y * q.Y;
		}
	}
}
