using System;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Meshing.Data
{
	// Token: 0x02000229 RID: 553
	internal class BadTriQueue
	{
		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x06000DDE RID: 3550 RVA: 0x000AC657 File Offset: 0x000AA857
		public int Count
		{
			get
			{
				return this.count;
			}
		}

		// Token: 0x06000DDF RID: 3551 RVA: 0x000AC660 File Offset: 0x000AA860
		public BadTriQueue()
		{
			this.queuefront = new BadTriangle[4096];
			this.queuetail = new BadTriangle[4096];
			this.nextnonemptyq = new int[4096];
			this.firstnonemptyq = -1;
			this.count = 0;
		}

		// Token: 0x06000DE0 RID: 3552 RVA: 0x000AC6B4 File Offset: 0x000AA8B4
		public void Enqueue(BadTriangle badtri)
		{
			this.count++;
			double num;
			int num2;
			if (badtri.key >= 1.0)
			{
				num = badtri.key;
				num2 = 1;
			}
			else
			{
				num = 1.0 / badtri.key;
				num2 = 0;
			}
			int num3 = 0;
			while (num > 2.0)
			{
				int num4 = 1;
				double num5 = 0.5;
				while (num * num5 * num5 > 1.0)
				{
					num4 *= 2;
					num5 *= num5;
				}
				num3 += num4;
				num *= num5;
			}
			num3 = 2 * num3 + ((num > 1.4142135623730951) ? 1 : 0);
			int num6;
			if (num2 > 0)
			{
				num6 = 2047 - num3;
			}
			else
			{
				num6 = 2048 + num3;
			}
			if (this.queuefront[num6] == null)
			{
				if (num6 > this.firstnonemptyq)
				{
					this.nextnonemptyq[num6] = this.firstnonemptyq;
					this.firstnonemptyq = num6;
				}
				else
				{
					int num7 = num6 + 1;
					while (this.queuefront[num7] == null)
					{
						num7++;
					}
					this.nextnonemptyq[num6] = this.nextnonemptyq[num7];
					this.nextnonemptyq[num7] = num6;
				}
				this.queuefront[num6] = badtri;
			}
			else
			{
				this.queuetail[num6].next = badtri;
			}
			this.queuetail[num6] = badtri;
			badtri.next = null;
		}

		// Token: 0x06000DE1 RID: 3553 RVA: 0x000AC7FC File Offset: 0x000AA9FC
		public void Enqueue(ref Otri enqtri, double minedge, Vertex apex, Vertex org, Vertex dest)
		{
			this.Enqueue(new BadTriangle
			{
				poortri = enqtri,
				key = minedge,
				apex = apex,
				org = org,
				dest = dest
			});
		}

		// Token: 0x06000DE2 RID: 3554 RVA: 0x000AC840 File Offset: 0x000AAA40
		public BadTriangle Dequeue()
		{
			if (this.firstnonemptyq < 0)
			{
				return null;
			}
			this.count--;
			BadTriangle badTriangle = this.queuefront[this.firstnonemptyq];
			this.queuefront[this.firstnonemptyq] = badTriangle.next;
			if (badTriangle == this.queuetail[this.firstnonemptyq])
			{
				this.firstnonemptyq = this.nextnonemptyq[this.firstnonemptyq];
			}
			return badTriangle;
		}

		// Token: 0x04001EDD RID: 7901
		private const double SQRT2 = 1.4142135623730951;

		// Token: 0x04001EDE RID: 7902
		private BadTriangle[] queuefront;

		// Token: 0x04001EDF RID: 7903
		private BadTriangle[] queuetail;

		// Token: 0x04001EE0 RID: 7904
		private int[] nextnonemptyq;

		// Token: 0x04001EE1 RID: 7905
		private int firstnonemptyq;

		// Token: 0x04001EE2 RID: 7906
		private int count;
	}
}
