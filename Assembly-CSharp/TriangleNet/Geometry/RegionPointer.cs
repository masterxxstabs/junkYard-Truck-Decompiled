using System;

namespace TriangleNet.Geometry
{
	// Token: 0x02000245 RID: 581
	public class RegionPointer
	{
		// Token: 0x17000129 RID: 297
		// (get) Token: 0x06000ECD RID: 3789 RVA: 0x000B196F File Offset: 0x000AFB6F
		// (set) Token: 0x06000ECE RID: 3790 RVA: 0x000B1977 File Offset: 0x000AFB77
		public double Area
		{
			get
			{
				return this.area;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException("Area constraints must not be negative.");
				}
				this.area = value;
			}
		}

		// Token: 0x06000ECF RID: 3791 RVA: 0x000B1997 File Offset: 0x000AFB97
		public RegionPointer(double x, double y, int id) : this(x, y, id, 0.0)
		{
		}

		// Token: 0x06000ED0 RID: 3792 RVA: 0x000B19AB File Offset: 0x000AFBAB
		public RegionPointer(double x, double y, int id, double area)
		{
			this.point = new Point(x, y);
			this.id = id;
			this.area = area;
		}

		// Token: 0x04001F20 RID: 7968
		internal Point point;

		// Token: 0x04001F21 RID: 7969
		internal int id;

		// Token: 0x04001F22 RID: 7970
		internal double area;
	}
}
