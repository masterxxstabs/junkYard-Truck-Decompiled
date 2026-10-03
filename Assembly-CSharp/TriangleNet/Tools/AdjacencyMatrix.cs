using System;
using TriangleNet.Topology;

namespace TriangleNet.Tools
{
	// Token: 0x0200020F RID: 527
	public class AdjacencyMatrix
	{
		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000D29 RID: 3369 RVA: 0x000A5CAD File Offset: 0x000A3EAD
		public int[] ColumnPointers
		{
			get
			{
				return this.pcol;
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x06000D2A RID: 3370 RVA: 0x000A5CB5 File Offset: 0x000A3EB5
		public int[] RowIndices
		{
			get
			{
				return this.irow;
			}
		}

		// Token: 0x06000D2B RID: 3371 RVA: 0x000A5CC0 File Offset: 0x000A3EC0
		public AdjacencyMatrix(Mesh mesh)
		{
			this.N = mesh.vertices.Count;
			this.pcol = this.AdjacencyCount(mesh);
			this.nnz = this.pcol[this.N];
			this.irow = this.AdjacencySet(mesh, this.pcol);
			this.SortIndices();
		}

		// Token: 0x06000D2C RID: 3372 RVA: 0x000A5D20 File Offset: 0x000A3F20
		public AdjacencyMatrix(int[] pcol, int[] irow)
		{
			this.N = pcol.Length - 1;
			this.nnz = pcol[this.N];
			this.pcol = pcol;
			this.irow = irow;
			if (pcol[0] != 0)
			{
				throw new ArgumentException("Expected 0-based indexing.", "pcol");
			}
			if (irow.Length < this.nnz)
			{
				throw new ArgumentException();
			}
		}

		// Token: 0x06000D2D RID: 3373 RVA: 0x000A5D80 File Offset: 0x000A3F80
		public int Bandwidth()
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < this.N; i++)
			{
				for (int j = this.pcol[i]; j < this.pcol[i + 1]; j++)
				{
					int num3 = this.irow[j];
					num = Math.Max(num, i - num3);
					num2 = Math.Max(num2, num3 - i);
				}
			}
			return num + 1 + num2;
		}

		// Token: 0x06000D2E RID: 3374 RVA: 0x000A5DE8 File Offset: 0x000A3FE8
		private int[] AdjacencyCount(Mesh mesh)
		{
			int n = this.N;
			int[] array = new int[n + 1];
			for (int i = 0; i < n; i++)
			{
				array[i] = 1;
			}
			foreach (Triangle triangle in mesh.triangles)
			{
				int id = triangle.id;
				int id2 = triangle.vertices[0].id;
				int id3 = triangle.vertices[1].id;
				int id4 = triangle.vertices[2].id;
				int id5 = triangle.neighbors[2].tri.id;
				if (id5 < 0 || id < id5)
				{
					array[id2]++;
					array[id3]++;
				}
				id5 = triangle.neighbors[0].tri.id;
				if (id5 < 0 || id < id5)
				{
					array[id3]++;
					array[id4]++;
				}
				id5 = triangle.neighbors[1].tri.id;
				if (id5 < 0 || id < id5)
				{
					array[id4]++;
					array[id2]++;
				}
			}
			for (int j = n; j > 0; j--)
			{
				array[j] = array[j - 1];
			}
			array[0] = 0;
			for (int k = 1; k <= n; k++)
			{
				array[k] = array[k - 1] + array[k];
			}
			return array;
		}

		// Token: 0x06000D2F RID: 3375 RVA: 0x000A5F98 File Offset: 0x000A4198
		private int[] AdjacencySet(Mesh mesh, int[] pcol)
		{
			int n = this.N;
			int[] array = new int[n];
			Array.Copy(pcol, array, n);
			int[] array2 = new int[pcol[n]];
			for (int i = 0; i < n; i++)
			{
				array2[array[i]] = i;
				array[i]++;
			}
			foreach (Triangle triangle in mesh.triangles)
			{
				int id = triangle.id;
				int id2 = triangle.vertices[0].id;
				int id3 = triangle.vertices[1].id;
				int id4 = triangle.vertices[2].id;
				int id5 = triangle.neighbors[2].tri.id;
				if (id5 < 0 || id < id5)
				{
					int[] array3 = array2;
					int[] array4 = array;
					int num = id2;
					int num2 = array4[num];
					array4[num] = num2 + 1;
					array3[num2] = id3;
					int[] array5 = array2;
					int[] array6 = array;
					int num3 = id3;
					num2 = array6[num3];
					array6[num3] = num2 + 1;
					array5[num2] = id2;
				}
				id5 = triangle.neighbors[0].tri.id;
				if (id5 < 0 || id < id5)
				{
					int[] array7 = array2;
					int[] array8 = array;
					int num4 = id3;
					int num2 = array8[num4];
					array8[num4] = num2 + 1;
					array7[num2] = id4;
					int[] array9 = array2;
					int[] array10 = array;
					int num5 = id4;
					num2 = array10[num5];
					array10[num5] = num2 + 1;
					array9[num2] = id3;
				}
				id5 = triangle.neighbors[1].tri.id;
				if (id5 < 0 || id < id5)
				{
					int[] array11 = array2;
					int[] array12 = array;
					int num6 = id2;
					int num2 = array12[num6];
					array12[num6] = num2 + 1;
					array11[num2] = id4;
					int[] array13 = array2;
					int[] array14 = array;
					int num7 = id4;
					num2 = array14[num7];
					array14[num7] = num2 + 1;
					array13[num2] = id2;
				}
			}
			return array2;
		}

		// Token: 0x06000D30 RID: 3376 RVA: 0x000A615C File Offset: 0x000A435C
		public void SortIndices()
		{
			int n = this.N;
			int[] array = this.irow;
			for (int i = 0; i < n; i++)
			{
				int num = this.pcol[i];
				int num2 = this.pcol[i + 1];
				Array.Sort<int>(array, num, num2 - num);
			}
		}

		// Token: 0x04001E8A RID: 7818
		private int nnz;

		// Token: 0x04001E8B RID: 7819
		private int[] pcol;

		// Token: 0x04001E8C RID: 7820
		private int[] irow;

		// Token: 0x04001E8D RID: 7821
		public readonly int N;
	}
}
