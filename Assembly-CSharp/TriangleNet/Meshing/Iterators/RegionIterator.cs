using System;
using System.Collections.Generic;
using TriangleNet.Topology;

namespace TriangleNet.Meshing.Iterators
{
	// Token: 0x02000226 RID: 550
	public class RegionIterator
	{
		// Token: 0x06000DD3 RID: 3539 RVA: 0x000AC314 File Offset: 0x000AA514
		public RegionIterator(Mesh mesh)
		{
			this.region = new List<Triangle>();
		}

		// Token: 0x06000DD4 RID: 3540 RVA: 0x000AC328 File Offset: 0x000AA528
		public void Process(Triangle triangle, int boundary = 0)
		{
			this.Process(triangle, delegate(Triangle tri)
			{
				tri.label = triangle.label;
				tri.area = triangle.area;
			}, boundary);
		}

		// Token: 0x06000DD5 RID: 3541 RVA: 0x000AC35C File Offset: 0x000AA55C
		public void Process(Triangle triangle, Action<Triangle> action, int boundary = 0)
		{
			if (triangle.id == -1 || Otri.IsDead(triangle))
			{
				return;
			}
			this.region.Add(triangle);
			triangle.infected = true;
			if (boundary == 0)
			{
				this.ProcessRegion(action, (SubSegment seg) => seg.hash == -1);
			}
			else
			{
				this.ProcessRegion(action, (SubSegment seg) => seg.boundary != boundary);
			}
			this.region.Clear();
		}

		// Token: 0x06000DD6 RID: 3542 RVA: 0x000AC3EC File Offset: 0x000AA5EC
		private void ProcessRegion(Action<Triangle> action, Func<SubSegment, bool> protector)
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Osub osub = default(Osub);
			for (int i = 0; i < this.region.Count; i++)
			{
				otri.tri = this.region[i];
				action(otri.tri);
				otri.orient = 0;
				while (otri.orient < 3)
				{
					otri.Sym(ref otri2);
					otri.Pivot(ref osub);
					if (otri2.tri.id != -1 && !otri2.IsInfected() && protector(osub.seg))
					{
						otri2.Infect();
						this.region.Add(otri2.tri);
					}
					otri.orient++;
				}
			}
			foreach (Triangle triangle in this.region)
			{
				triangle.infected = false;
			}
			this.region.Clear();
		}

		// Token: 0x04001ED8 RID: 7896
		private List<Triangle> region;
	}
}
