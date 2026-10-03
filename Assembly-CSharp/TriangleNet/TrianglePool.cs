using System;
using System.Collections;
using System.Collections.Generic;
using TriangleNet.Topology;

namespace TriangleNet
{
	// Token: 0x020001FC RID: 508
	public class TrianglePool : ICollection<Triangle>, IEnumerable<Triangle>, IEnumerable
	{
		// Token: 0x06000C59 RID: 3161 RVA: 0x000A2030 File Offset: 0x000A0230
		public TrianglePool()
		{
			this.size = 0;
			int num = Math.Max(1, 64);
			this.pool = new Triangle[num][];
			this.pool[0] = new Triangle[1024];
			this.stack = new Stack<Triangle>(1024);
		}

		// Token: 0x06000C5A RID: 3162 RVA: 0x000A2084 File Offset: 0x000A0284
		public Triangle Get()
		{
			Triangle triangle;
			if (this.stack.Count > 0)
			{
				triangle = this.stack.Pop();
				triangle.hash = -triangle.hash - 1;
				this.Cleanup(triangle);
			}
			else if (this.count < this.size)
			{
				triangle = this.pool[this.count / 1024][this.count % 1024];
				triangle.id = triangle.hash;
				this.Cleanup(triangle);
				this.count++;
			}
			else
			{
				triangle = new Triangle();
				triangle.hash = this.size;
				triangle.id = triangle.hash;
				int num = this.size / 1024;
				if (this.pool[num] == null)
				{
					this.pool[num] = new Triangle[1024];
					if (num + 1 == this.pool.Length)
					{
						Array.Resize<Triangle[]>(ref this.pool, 2 * this.pool.Length);
					}
				}
				this.pool[num][this.size % 1024] = triangle;
				int num2 = this.size + 1;
				this.size = num2;
				this.count = num2;
			}
			return triangle;
		}

		// Token: 0x06000C5B RID: 3163 RVA: 0x000A21B2 File Offset: 0x000A03B2
		public void Release(Triangle triangle)
		{
			this.stack.Push(triangle);
			triangle.hash = -triangle.hash - 1;
		}

		// Token: 0x06000C5C RID: 3164 RVA: 0x000A21D0 File Offset: 0x000A03D0
		public TrianglePool Restart()
		{
			foreach (Triangle triangle in this.stack)
			{
				triangle.hash = -triangle.hash - 1;
			}
			this.stack.Clear();
			this.count = 0;
			return this;
		}

		// Token: 0x06000C5D RID: 3165 RVA: 0x000A223C File Offset: 0x000A043C
		internal IEnumerable<Triangle> Sample(int k, Random random)
		{
			int count = this.Count;
			if (k > count)
			{
				k = count;
			}
			while (k > 0)
			{
				int num = random.Next(0, count);
				Triangle triangle = this.pool[num / 1024][num % 1024];
				if (triangle.hash >= 0)
				{
					int num2 = k;
					k = num2 - 1;
					yield return triangle;
				}
			}
			yield break;
		}

		// Token: 0x06000C5E RID: 3166 RVA: 0x000A225C File Offset: 0x000A045C
		private void Cleanup(Triangle triangle)
		{
			triangle.label = 0;
			triangle.area = 0.0;
			triangle.infected = false;
			for (int i = 0; i < 3; i++)
			{
				triangle.vertices[i] = null;
				triangle.subsegs[i] = default(Osub);
				triangle.neighbors[i] = default(Otri);
			}
		}

		// Token: 0x06000C5F RID: 3167 RVA: 0x000A22BF File Offset: 0x000A04BF
		public void Add(Triangle item)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000C60 RID: 3168 RVA: 0x000A22C8 File Offset: 0x000A04C8
		public void Clear()
		{
			this.stack.Clear();
			int num = this.size / 1024 + 1;
			for (int i = 0; i < num; i++)
			{
				Triangle[] array = this.pool[i];
				int num2 = (this.size - i * 1024) % 1024;
				for (int j = 0; j < num2; j++)
				{
					array[j] = null;
				}
			}
			this.size = (this.count = 0);
		}

		// Token: 0x06000C61 RID: 3169 RVA: 0x000A2340 File Offset: 0x000A0540
		public bool Contains(Triangle item)
		{
			int hash = item.hash;
			return hash >= 0 && hash <= this.size && this.pool[hash / 1024][hash % 1024].hash >= 0;
		}

		// Token: 0x06000C62 RID: 3170 RVA: 0x000A2384 File Offset: 0x000A0584
		public void CopyTo(Triangle[] array, int index)
		{
			foreach (Triangle triangle in this)
			{
				array[index] = triangle;
				index++;
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000C63 RID: 3171 RVA: 0x000A23B0 File Offset: 0x000A05B0
		public int Count
		{
			get
			{
				return this.count - this.stack.Count;
			}
		}

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000C64 RID: 3172 RVA: 0x000915D6 File Offset: 0x0008F7D6
		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000C65 RID: 3173 RVA: 0x000A22BF File Offset: 0x000A04BF
		public bool Remove(Triangle item)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000C66 RID: 3174 RVA: 0x000A23C4 File Offset: 0x000A05C4
		public IEnumerator<Triangle> GetEnumerator()
		{
			return new TrianglePool.Enumerator(this);
		}

		// Token: 0x06000C67 RID: 3175 RVA: 0x000A23CC File Offset: 0x000A05CC
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x04001E45 RID: 7749
		private const int BLOCKSIZE = 1024;

		// Token: 0x04001E46 RID: 7750
		private int size;

		// Token: 0x04001E47 RID: 7751
		private int count;

		// Token: 0x04001E48 RID: 7752
		private Triangle[][] pool;

		// Token: 0x04001E49 RID: 7753
		private Stack<Triangle> stack;

		// Token: 0x0200048F RID: 1167
		private class Enumerator : IEnumerator<Triangle>, IEnumerator, IDisposable
		{
			// Token: 0x06001A81 RID: 6785 RVA: 0x000F6D27 File Offset: 0x000F4F27
			public Enumerator(TrianglePool pool)
			{
				this.count = pool.Count;
				this.pool = pool.pool;
				this.index = 0;
				this.offset = 0;
			}

			// Token: 0x17000390 RID: 912
			// (get) Token: 0x06001A82 RID: 6786 RVA: 0x000F6D55 File Offset: 0x000F4F55
			public Triangle Current
			{
				get
				{
					return this.current;
				}
			}

			// Token: 0x06001A83 RID: 6787 RVA: 0x00002188 File Offset: 0x00000388
			public void Dispose()
			{
			}

			// Token: 0x17000391 RID: 913
			// (get) Token: 0x06001A84 RID: 6788 RVA: 0x000F6D55 File Offset: 0x000F4F55
			object IEnumerator.Current
			{
				get
				{
					return this.current;
				}
			}

			// Token: 0x06001A85 RID: 6789 RVA: 0x000F6D60 File Offset: 0x000F4F60
			public bool MoveNext()
			{
				while (this.index < this.count)
				{
					this.current = this.pool[this.offset / 1024][this.offset % 1024];
					this.offset++;
					if (this.current.hash >= 0)
					{
						this.index++;
						return true;
					}
				}
				return false;
			}

			// Token: 0x06001A86 RID: 6790 RVA: 0x000F6DD0 File Offset: 0x000F4FD0
			public void Reset()
			{
				this.index = (this.offset = 0);
			}

			// Token: 0x04002B61 RID: 11105
			private int count;

			// Token: 0x04002B62 RID: 11106
			private Triangle[][] pool;

			// Token: 0x04002B63 RID: 11107
			private Triangle current;

			// Token: 0x04002B64 RID: 11108
			private int index;

			// Token: 0x04002B65 RID: 11109
			private int offset;
		}
	}
}
