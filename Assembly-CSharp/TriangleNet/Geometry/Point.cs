using System;

namespace TriangleNet.Geometry
{
	// Token: 0x02000242 RID: 578
	public class Point : IComparable<Point>, IEquatable<Point>
	{
		// Token: 0x06000E94 RID: 3732 RVA: 0x000B12F2 File Offset: 0x000AF4F2
		public Point() : this(0.0, 0.0, 0)
		{
		}

		// Token: 0x06000E95 RID: 3733 RVA: 0x000B130D File Offset: 0x000AF50D
		public Point(double x, double y) : this(x, y, 0)
		{
		}

		// Token: 0x06000E96 RID: 3734 RVA: 0x000B1318 File Offset: 0x000AF518
		public Point(double x, double y, int label)
		{
			this.x = x;
			this.y = y;
			this.label = label;
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06000E97 RID: 3735 RVA: 0x000B1335 File Offset: 0x000AF535
		// (set) Token: 0x06000E98 RID: 3736 RVA: 0x000B133D File Offset: 0x000AF53D
		public int ID
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x06000E99 RID: 3737 RVA: 0x000B1346 File Offset: 0x000AF546
		// (set) Token: 0x06000E9A RID: 3738 RVA: 0x000B134E File Offset: 0x000AF54E
		public double X
		{
			get
			{
				return this.x;
			}
			set
			{
				this.x = value;
			}
		}

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x06000E9B RID: 3739 RVA: 0x000B1357 File Offset: 0x000AF557
		// (set) Token: 0x06000E9C RID: 3740 RVA: 0x000B135F File Offset: 0x000AF55F
		public double Y
		{
			get
			{
				return this.y;
			}
			set
			{
				this.y = value;
			}
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06000E9D RID: 3741 RVA: 0x000B1368 File Offset: 0x000AF568
		// (set) Token: 0x06000E9E RID: 3742 RVA: 0x000B1370 File Offset: 0x000AF570
		public double Z
		{
			get
			{
				return this.z;
			}
			set
			{
				this.z = value;
			}
		}

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06000E9F RID: 3743 RVA: 0x000B1379 File Offset: 0x000AF579
		// (set) Token: 0x06000EA0 RID: 3744 RVA: 0x000B1381 File Offset: 0x000AF581
		public int Label
		{
			get
			{
				return this.label;
			}
			set
			{
				this.label = value;
			}
		}

		// Token: 0x06000EA1 RID: 3745 RVA: 0x000B138A File Offset: 0x000AF58A
		public static bool operator ==(Point a, Point b)
		{
			return a == b || (a != null && b != null && a.Equals(b));
		}

		// Token: 0x06000EA2 RID: 3746 RVA: 0x000B13A1 File Offset: 0x000AF5A1
		public static bool operator !=(Point a, Point b)
		{
			return !(a == b);
		}

		// Token: 0x06000EA3 RID: 3747 RVA: 0x000B13B0 File Offset: 0x000AF5B0
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			Point point = obj as Point;
			return point != null && this.x == point.x && this.y == point.y;
		}

		// Token: 0x06000EA4 RID: 3748 RVA: 0x000B13EC File Offset: 0x000AF5EC
		public bool Equals(Point p)
		{
			return p != null && this.x == p.x && this.y == p.y;
		}

		// Token: 0x06000EA5 RID: 3749 RVA: 0x000B1414 File Offset: 0x000AF614
		public int CompareTo(Point other)
		{
			if (this.x == other.x && this.y == other.y)
			{
				return 0;
			}
			if (this.x >= other.x && (this.x != other.x || this.y >= other.y))
			{
				return 1;
			}
			return -1;
		}

		// Token: 0x06000EA6 RID: 3750 RVA: 0x000B146C File Offset: 0x000AF66C
		public override int GetHashCode()
		{
			return (19 * 31 + this.x.GetHashCode()) * 31 + this.y.GetHashCode();
		}

		// Token: 0x06000EA7 RID: 3751 RVA: 0x000B148E File Offset: 0x000AF68E
		public override string ToString()
		{
			return string.Format("[{0},{1}]", this.x, this.y);
		}

		// Token: 0x04001F11 RID: 7953
		internal int id;

		// Token: 0x04001F12 RID: 7954
		internal int label;

		// Token: 0x04001F13 RID: 7955
		internal double x;

		// Token: 0x04001F14 RID: 7956
		internal double y;

		// Token: 0x04001F15 RID: 7957
		internal double z;
	}
}
