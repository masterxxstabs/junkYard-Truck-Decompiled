using System;
using System.Collections.Generic;
using System.Linq;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Tools
{
	// Token: 0x02000216 RID: 534
	public class TriangleQuadTree
	{
		// Token: 0x06000D67 RID: 3431 RVA: 0x000A7DA8 File Offset: 0x000A5FA8
		public TriangleQuadTree(Mesh mesh, int maxDepth = 10, int sizeBound = 10)
		{
			this.maxDepth = maxDepth;
			this.sizeBound = sizeBound;
			ITriangle[] array = mesh.Triangles.ToArray<Triangle>();
			this.triangles = array;
			int num = 0;
			this.root = new TriangleQuadTree.QuadNode(mesh.Bounds, this, true);
			this.root.CreateSubRegion(num + 1);
		}

		// Token: 0x06000D68 RID: 3432 RVA: 0x000A7E04 File Offset: 0x000A6004
		public ITriangle Query(double x, double y)
		{
			Point point = new Point(x, y);
			foreach (int num in this.root.FindTriangles(point))
			{
				ITriangle triangle = this.triangles[num];
				if (TriangleQuadTree.IsPointInTriangle(point, triangle.GetVertex(0), triangle.GetVertex(1), triangle.GetVertex(2)))
				{
					return triangle;
				}
			}
			return null;
		}

		// Token: 0x06000D69 RID: 3433 RVA: 0x000A7E90 File Offset: 0x000A6090
		internal static bool IsPointInTriangle(Point p, Point t0, Point t1, Point t2)
		{
			Point point = new Point(t1.x - t0.x, t1.y - t0.y);
			Point point2 = new Point(t2.x - t0.x, t2.y - t0.y);
			Point p2 = new Point(p.x - t0.x, p.y - t0.y);
			Point q = new Point(-point.y, point.x);
			Point q2 = new Point(-point2.y, point2.x);
			double num = TriangleQuadTree.DotProduct(p2, q2) / TriangleQuadTree.DotProduct(point, q2);
			double num2 = TriangleQuadTree.DotProduct(p2, q) / TriangleQuadTree.DotProduct(point2, q);
			return num >= 0.0 && num2 >= 0.0 && num + num2 <= 1.0;
		}

		// Token: 0x06000D6A RID: 3434 RVA: 0x000A7F70 File Offset: 0x000A6170
		internal static double DotProduct(Point p, Point q)
		{
			return p.x * q.x + p.y * q.y;
		}

		// Token: 0x04001EA9 RID: 7849
		private TriangleQuadTree.QuadNode root;

		// Token: 0x04001EAA RID: 7850
		internal ITriangle[] triangles;

		// Token: 0x04001EAB RID: 7851
		internal int sizeBound;

		// Token: 0x04001EAC RID: 7852
		internal int maxDepth;

		// Token: 0x02000496 RID: 1174
		private class QuadNode
		{
			// Token: 0x06001AAB RID: 6827 RVA: 0x000F75AB File Offset: 0x000F57AB
			public QuadNode(Rectangle box, TriangleQuadTree tree) : this(box, tree, false)
			{
			}

			// Token: 0x06001AAC RID: 6828 RVA: 0x000F75B8 File Offset: 0x000F57B8
			public QuadNode(Rectangle box, TriangleQuadTree tree, bool init)
			{
				this.tree = tree;
				this.bounds = new Rectangle(box.Left, box.Bottom, box.Width, box.Height);
				this.pivot = new Point((box.Left + box.Right) / 2.0, (box.Bottom + box.Top) / 2.0);
				this.bitRegions = 0;
				this.regions = new TriangleQuadTree.QuadNode[4];
				this.triangles = new List<int>();
				if (init)
				{
					int num = tree.triangles.Length;
					this.triangles.Capacity = num;
					for (int i = 0; i < num; i++)
					{
						this.triangles.Add(i);
					}
				}
			}

			// Token: 0x06001AAD RID: 6829 RVA: 0x000F767C File Offset: 0x000F587C
			public List<int> FindTriangles(Point searchPoint)
			{
				int num = this.FindRegion(searchPoint);
				if (this.regions[num] == null)
				{
					return this.triangles;
				}
				return this.regions[num].FindTriangles(searchPoint);
			}

			// Token: 0x06001AAE RID: 6830 RVA: 0x000F76B0 File Offset: 0x000F58B0
			public void CreateSubRegion(int currentDepth)
			{
				double width = this.bounds.Right - this.pivot.x;
				double height = this.bounds.Top - this.pivot.y;
				Rectangle box = new Rectangle(this.bounds.Left, this.bounds.Bottom, width, height);
				this.regions[0] = new TriangleQuadTree.QuadNode(box, this.tree);
				box = new Rectangle(this.pivot.x, this.bounds.Bottom, width, height);
				this.regions[1] = new TriangleQuadTree.QuadNode(box, this.tree);
				box = new Rectangle(this.bounds.Left, this.pivot.y, width, height);
				this.regions[2] = new TriangleQuadTree.QuadNode(box, this.tree);
				box = new Rectangle(this.pivot.x, this.pivot.y, width, height);
				this.regions[3] = new TriangleQuadTree.QuadNode(box, this.tree);
				Point[] array = new Point[3];
				foreach (int num in this.triangles)
				{
					ITriangle triangle = this.tree.triangles[num];
					array[0] = triangle.GetVertex(0);
					array[1] = triangle.GetVertex(1);
					array[2] = triangle.GetVertex(2);
					this.AddTriangleToRegion(array, num);
				}
				for (int i = 0; i < 4; i++)
				{
					if (this.regions[i].triangles.Count > this.tree.sizeBound && currentDepth < this.tree.maxDepth)
					{
						this.regions[i].CreateSubRegion(currentDepth + 1);
					}
				}
			}

			// Token: 0x06001AAF RID: 6831 RVA: 0x000F7888 File Offset: 0x000F5A88
			private void AddTriangleToRegion(Point[] triangle, int index)
			{
				this.bitRegions = 0;
				if (TriangleQuadTree.IsPointInTriangle(this.pivot, triangle[0], triangle[1], triangle[2]))
				{
					this.AddToRegion(index, 0);
					this.AddToRegion(index, 1);
					this.AddToRegion(index, 2);
					this.AddToRegion(index, 3);
					return;
				}
				this.FindTriangleIntersections(triangle, index);
				if (this.bitRegions == 0)
				{
					int num = this.FindRegion(triangle[0]);
					this.regions[num].triangles.Add(index);
				}
			}

			// Token: 0x06001AB0 RID: 6832 RVA: 0x000F7900 File Offset: 0x000F5B00
			private void FindTriangleIntersections(Point[] triangle, int index)
			{
				int num = 2;
				int i = 0;
				while (i < 3)
				{
					double num2 = triangle[i].x - triangle[num].x;
					double num3 = triangle[i].y - triangle[num].y;
					if (num2 != 0.0)
					{
						this.FindIntersectionsWithX(num2, num3, triangle, index, num);
					}
					if (num3 != 0.0)
					{
						this.FindIntersectionsWithY(num2, num3, triangle, index, num);
					}
					num = i++;
				}
			}

			// Token: 0x06001AB1 RID: 6833 RVA: 0x000F7970 File Offset: 0x000F5B70
			private void FindIntersectionsWithX(double dx, double dy, Point[] triangle, int index, int k)
			{
				double num = (this.pivot.x - triangle[k].x) / dx;
				if (num < 1.000001 && num > -1E-06)
				{
					double num2 = triangle[k].y + num * dy;
					if (num2 < this.pivot.y && num2 >= this.bounds.Bottom)
					{
						this.AddToRegion(index, 0);
						this.AddToRegion(index, 1);
					}
					else if (num2 <= this.bounds.Top)
					{
						this.AddToRegion(index, 2);
						this.AddToRegion(index, 3);
					}
				}
				num = (this.bounds.Left - triangle[k].x) / dx;
				if (num < 1.000001 && num > -1E-06)
				{
					double num3 = triangle[k].y + num * dy;
					if (num3 < this.pivot.y && num3 >= this.bounds.Bottom)
					{
						this.AddToRegion(index, 0);
					}
					else if (num3 <= this.bounds.Top)
					{
						this.AddToRegion(index, 2);
					}
				}
				num = (this.bounds.Right - triangle[k].x) / dx;
				if (num < 1.000001 && num > -1E-06)
				{
					double num4 = triangle[k].y + num * dy;
					if (num4 < this.pivot.y && num4 >= this.bounds.Bottom)
					{
						this.AddToRegion(index, 1);
						return;
					}
					if (num4 <= this.bounds.Top)
					{
						this.AddToRegion(index, 3);
					}
				}
			}

			// Token: 0x06001AB2 RID: 6834 RVA: 0x000F7B04 File Offset: 0x000F5D04
			private void FindIntersectionsWithY(double dx, double dy, Point[] triangle, int index, int k)
			{
				double num = (this.pivot.y - triangle[k].y) / dy;
				if (num < 1.000001 && num > -1E-06)
				{
					double num2 = triangle[k].x + num * dx;
					if (num2 > this.pivot.x && num2 <= this.bounds.Right)
					{
						this.AddToRegion(index, 1);
						this.AddToRegion(index, 3);
					}
					else if (num2 >= this.bounds.Left)
					{
						this.AddToRegion(index, 0);
						this.AddToRegion(index, 2);
					}
				}
				num = (this.bounds.Bottom - triangle[k].y) / dy;
				if (num < 1.000001 && num > -1E-06)
				{
					double num2 = triangle[k].x + num * dx;
					if (num2 > this.pivot.x && num2 <= this.bounds.Right)
					{
						this.AddToRegion(index, 1);
					}
					else if (num2 >= this.bounds.Left)
					{
						this.AddToRegion(index, 0);
					}
				}
				num = (this.bounds.Top - triangle[k].y) / dy;
				if (num < 1.000001 && num > -1E-06)
				{
					double num2 = triangle[k].x + num * dx;
					if (num2 > this.pivot.x && num2 <= this.bounds.Right)
					{
						this.AddToRegion(index, 3);
						return;
					}
					if (num2 >= this.bounds.Left)
					{
						this.AddToRegion(index, 2);
					}
				}
			}

			// Token: 0x06001AB3 RID: 6835 RVA: 0x000F7C98 File Offset: 0x000F5E98
			private int FindRegion(Point point)
			{
				int num = 2;
				if (point.y < this.pivot.y)
				{
					num = 0;
				}
				if (point.x > this.pivot.x)
				{
					num++;
				}
				return num;
			}

			// Token: 0x06001AB4 RID: 6836 RVA: 0x000F7CD4 File Offset: 0x000F5ED4
			private void AddToRegion(int index, int region)
			{
				if ((this.bitRegions & TriangleQuadTree.QuadNode.BITVECTOR[region]) == 0)
				{
					this.regions[region].triangles.Add(index);
					this.bitRegions |= TriangleQuadTree.QuadNode.BITVECTOR[region];
				}
			}

			// Token: 0x04002B87 RID: 11143
			private const int SW = 0;

			// Token: 0x04002B88 RID: 11144
			private const int SE = 1;

			// Token: 0x04002B89 RID: 11145
			private const int NW = 2;

			// Token: 0x04002B8A RID: 11146
			private const int NE = 3;

			// Token: 0x04002B8B RID: 11147
			private const double EPS = 1E-06;

			// Token: 0x04002B8C RID: 11148
			private static readonly byte[] BITVECTOR = new byte[]
			{
				1,
				2,
				4,
				8
			};

			// Token: 0x04002B8D RID: 11149
			private Rectangle bounds;

			// Token: 0x04002B8E RID: 11150
			private Point pivot;

			// Token: 0x04002B8F RID: 11151
			private TriangleQuadTree tree;

			// Token: 0x04002B90 RID: 11152
			private TriangleQuadTree.QuadNode[] regions;

			// Token: 0x04002B91 RID: 11153
			private List<int> triangles;

			// Token: 0x04002B92 RID: 11154
			private byte bitRegions;
		}
	}
}
