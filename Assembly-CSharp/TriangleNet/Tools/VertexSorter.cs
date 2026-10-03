using System;
using TriangleNet.Geometry;

namespace TriangleNet.Tools
{
	// Token: 0x02000217 RID: 535
	public class VertexSorter
	{
		// Token: 0x06000D6B RID: 3435 RVA: 0x000A7F8D File Offset: 0x000A618D
		private VertexSorter(Vertex[] points, int seed)
		{
			this.points = points;
			this.rand = new Random(seed);
		}

		// Token: 0x06000D6C RID: 3436 RVA: 0x000A7FA8 File Offset: 0x000A61A8
		public static void Sort(Vertex[] array, int seed = 57113)
		{
			new VertexSorter(array, seed).QuickSort(0, array.Length - 1);
		}

		// Token: 0x06000D6D RID: 3437 RVA: 0x000A7FBC File Offset: 0x000A61BC
		public static void Alternate(Vertex[] array, int length, int seed = 57113)
		{
			VertexSorter vertexSorter = new VertexSorter(array, seed);
			int num = length >> 1;
			if (length - num >= 2)
			{
				if (num >= 2)
				{
					vertexSorter.AlternateAxes(0, num - 1, 1);
				}
				vertexSorter.AlternateAxes(num, length - 1, 1);
			}
		}

		// Token: 0x06000D6E RID: 3438 RVA: 0x000A7FF8 File Offset: 0x000A61F8
		private void QuickSort(int left, int right)
		{
			int num = left;
			int num2 = right;
			int num3 = right - left + 1;
			Vertex[] array = this.points;
			if (num3 < 32)
			{
				for (int i = left + 1; i <= right; i++)
				{
					Vertex vertex = array[i];
					int num4 = i - 1;
					while (num4 >= left && (array[num4].x > vertex.x || (array[num4].x == vertex.x && array[num4].y > vertex.y)))
					{
						array[num4 + 1] = array[num4];
						num4--;
					}
					array[num4 + 1] = vertex;
				}
				return;
			}
			int num5 = this.rand.Next(left, right);
			double x = array[num5].x;
			double y = array[num5].y;
			left--;
			right++;
			while (left < right)
			{
				do
				{
					left++;
				}
				while (left <= right && (array[left].x < x || (array[left].x == x && array[left].y < y)));
				do
				{
					right--;
				}
				while (left <= right && (array[right].x > x || (array[right].x == x && array[right].y > y)));
				if (left < right)
				{
					Vertex vertex2 = array[left];
					array[left] = array[right];
					array[right] = vertex2;
				}
			}
			if (left > num)
			{
				this.QuickSort(num, left);
			}
			if (num2 > right + 1)
			{
				this.QuickSort(right + 1, num2);
			}
		}

		// Token: 0x06000D6F RID: 3439 RVA: 0x000A8158 File Offset: 0x000A6358
		private void AlternateAxes(int left, int right, int axis)
		{
			int num = right - left + 1;
			int num2 = num >> 1;
			if (num <= 3)
			{
				axis = 0;
			}
			if (axis == 0)
			{
				this.VertexMedianX(left, right, left + num2);
			}
			else
			{
				this.VertexMedianY(left, right, left + num2);
			}
			if (num - num2 >= 2)
			{
				if (num2 >= 2)
				{
					this.AlternateAxes(left, left + num2 - 1, 1 - axis);
				}
				this.AlternateAxes(left + num2, right, 1 - axis);
			}
		}

		// Token: 0x06000D70 RID: 3440 RVA: 0x000A81B8 File Offset: 0x000A63B8
		private void VertexMedianX(int left, int right, int median)
		{
			int num = right - left + 1;
			int left2 = left;
			int right2 = right;
			Vertex[] array = this.points;
			if (num == 2)
			{
				if (array[left].x > array[right].x || (array[left].x == array[right].x && array[left].y > array[right].y))
				{
					Vertex vertex = array[right];
					array[right] = array[left];
					array[left] = vertex;
				}
				return;
			}
			int num2 = this.rand.Next(left, right);
			double x = array[num2].x;
			double y = array[num2].y;
			left--;
			right++;
			while (left < right)
			{
				do
				{
					left++;
				}
				while (left <= right && (array[left].x < x || (array[left].x == x && array[left].y < y)));
				do
				{
					right--;
				}
				while (left <= right && (array[right].x > x || (array[right].x == x && array[right].y > y)));
				if (left < right)
				{
					Vertex vertex = array[left];
					array[left] = array[right];
					array[right] = vertex;
				}
			}
			if (left > median)
			{
				this.VertexMedianX(left2, left - 1, median);
			}
			if (right < median - 1)
			{
				this.VertexMedianX(right + 1, right2, median);
			}
		}

		// Token: 0x06000D71 RID: 3441 RVA: 0x000A82F4 File Offset: 0x000A64F4
		private void VertexMedianY(int left, int right, int median)
		{
			int num = right - left + 1;
			int left2 = left;
			int right2 = right;
			Vertex[] array = this.points;
			if (num == 2)
			{
				if (array[left].y > array[right].y || (array[left].y == array[right].y && array[left].x > array[right].x))
				{
					Vertex vertex = array[right];
					array[right] = array[left];
					array[left] = vertex;
				}
				return;
			}
			int num2 = this.rand.Next(left, right);
			double y = array[num2].y;
			double x = array[num2].x;
			left--;
			right++;
			while (left < right)
			{
				do
				{
					left++;
				}
				while (left <= right && (array[left].y < y || (array[left].y == y && array[left].x < x)));
				do
				{
					right--;
				}
				while (left <= right && (array[right].y > y || (array[right].y == y && array[right].x > x)));
				if (left < right)
				{
					Vertex vertex = array[left];
					array[left] = array[right];
					array[right] = vertex;
				}
			}
			if (left > median)
			{
				this.VertexMedianY(left2, left - 1, median);
			}
			if (right < median - 1)
			{
				this.VertexMedianY(right + 1, right2, median);
			}
		}

		// Token: 0x04001EAD RID: 7853
		private const int RANDOM_SEED = 57113;

		// Token: 0x04001EAE RID: 7854
		private Random rand;

		// Token: 0x04001EAF RID: 7855
		private Vertex[] points;
	}
}
