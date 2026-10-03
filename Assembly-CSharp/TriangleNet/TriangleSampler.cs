using System;
using System.Collections;
using System.Collections.Generic;
using TriangleNet.Topology;

namespace TriangleNet
{
	// Token: 0x020001FD RID: 509
	internal class TriangleSampler : IEnumerable<Triangle>, IEnumerable
	{
		// Token: 0x06000C68 RID: 3176 RVA: 0x000A23D4 File Offset: 0x000A05D4
		public TriangleSampler(Mesh mesh) : this(mesh, new Random(110503))
		{
		}

		// Token: 0x06000C69 RID: 3177 RVA: 0x000A23E7 File Offset: 0x000A05E7
		public TriangleSampler(Mesh mesh, Random random)
		{
			this.mesh = mesh;
			this.random = random;
		}

		// Token: 0x06000C6A RID: 3178 RVA: 0x000A2404 File Offset: 0x000A0604
		public void Reset()
		{
			this.samples = 1;
			this.triangleCount = 0;
		}

		// Token: 0x06000C6B RID: 3179 RVA: 0x000A2414 File Offset: 0x000A0614
		public void Update()
		{
			int count = this.mesh.triangles.Count;
			if (this.triangleCount != count)
			{
				this.triangleCount = count;
				while (11 * this.samples * this.samples * this.samples < count)
				{
					this.samples++;
				}
			}
		}

		// Token: 0x06000C6C RID: 3180 RVA: 0x000A246C File Offset: 0x000A066C
		public IEnumerator<Triangle> GetEnumerator()
		{
			return this.mesh.triangles.Sample(this.samples, this.random).GetEnumerator();
		}

		// Token: 0x06000C6D RID: 3181 RVA: 0x000A248F File Offset: 0x000A068F
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x04001E4A RID: 7754
		private const int RANDOM_SEED = 110503;

		// Token: 0x04001E4B RID: 7755
		private const int samplefactor = 11;

		// Token: 0x04001E4C RID: 7756
		private Random random;

		// Token: 0x04001E4D RID: 7757
		private Mesh mesh;

		// Token: 0x04001E4E RID: 7758
		private int samples = 1;

		// Token: 0x04001E4F RID: 7759
		private int triangleCount;
	}
}
