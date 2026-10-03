using System;
using System.Collections.Generic;

namespace TriangleNet.Geometry
{
	// Token: 0x02000244 RID: 580
	public class Rectangle
	{
		// Token: 0x06000EBC RID: 3772 RVA: 0x000B1694 File Offset: 0x000AF894
		public Rectangle()
		{
			this.xmin = (this.ymin = double.MaxValue);
			this.xmax = (this.ymax = double.MinValue);
		}

		// Token: 0x06000EBD RID: 3773 RVA: 0x000B16D7 File Offset: 0x000AF8D7
		public Rectangle(Rectangle other) : this(other.Left, other.Bottom, other.Right, other.Top)
		{
		}

		// Token: 0x06000EBE RID: 3774 RVA: 0x000B16F7 File Offset: 0x000AF8F7
		public Rectangle(double x, double y, double width, double height)
		{
			this.xmin = x;
			this.ymin = y;
			this.xmax = x + width;
			this.ymax = y + height;
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x06000EBF RID: 3775 RVA: 0x000B1720 File Offset: 0x000AF920
		public double Left
		{
			get
			{
				return this.xmin;
			}
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x06000EC0 RID: 3776 RVA: 0x000B1728 File Offset: 0x000AF928
		public double Right
		{
			get
			{
				return this.xmax;
			}
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06000EC1 RID: 3777 RVA: 0x000B1730 File Offset: 0x000AF930
		public double Bottom
		{
			get
			{
				return this.ymin;
			}
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x06000EC2 RID: 3778 RVA: 0x000B1738 File Offset: 0x000AF938
		public double Top
		{
			get
			{
				return this.ymax;
			}
		}

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x06000EC3 RID: 3779 RVA: 0x000B1740 File Offset: 0x000AF940
		public double Width
		{
			get
			{
				return this.xmax - this.xmin;
			}
		}

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x06000EC4 RID: 3780 RVA: 0x000B174F File Offset: 0x000AF94F
		public double Height
		{
			get
			{
				return this.ymax - this.ymin;
			}
		}

		// Token: 0x06000EC5 RID: 3781 RVA: 0x000B175E File Offset: 0x000AF95E
		public void Resize(double dx, double dy)
		{
			this.xmin -= dx;
			this.xmax += dx;
			this.ymin -= dy;
			this.ymax += dy;
		}

		// Token: 0x06000EC6 RID: 3782 RVA: 0x000B1798 File Offset: 0x000AF998
		public void Expand(Point p)
		{
			this.xmin = Math.Min(this.xmin, p.x);
			this.ymin = Math.Min(this.ymin, p.y);
			this.xmax = Math.Max(this.xmax, p.x);
			this.ymax = Math.Max(this.ymax, p.y);
		}

		// Token: 0x06000EC7 RID: 3783 RVA: 0x000B1804 File Offset: 0x000AFA04
		public void Expand(IEnumerable<Point> points)
		{
			foreach (Point p in points)
			{
				this.Expand(p);
			}
		}

		// Token: 0x06000EC8 RID: 3784 RVA: 0x000B184C File Offset: 0x000AFA4C
		public void Expand(Rectangle other)
		{
			this.xmin = Math.Min(this.xmin, other.xmin);
			this.ymin = Math.Min(this.ymin, other.ymin);
			this.xmax = Math.Max(this.xmax, other.xmax);
			this.ymax = Math.Max(this.ymax, other.ymax);
		}

		// Token: 0x06000EC9 RID: 3785 RVA: 0x000B18B5 File Offset: 0x000AFAB5
		public bool Contains(double x, double y)
		{
			return x >= this.xmin && x <= this.xmax && y >= this.ymin && y <= this.ymax;
		}

		// Token: 0x06000ECA RID: 3786 RVA: 0x000B18E0 File Offset: 0x000AFAE0
		public bool Contains(Point pt)
		{
			return this.Contains(pt.x, pt.y);
		}

		// Token: 0x06000ECB RID: 3787 RVA: 0x000B18F4 File Offset: 0x000AFAF4
		public bool Contains(Rectangle other)
		{
			return this.xmin <= other.Left && other.Right <= this.xmax && this.ymin <= other.Bottom && other.Top <= this.ymax;
		}

		// Token: 0x06000ECC RID: 3788 RVA: 0x000B1933 File Offset: 0x000AFB33
		public bool Intersects(Rectangle other)
		{
			return other.Left < this.xmax && this.xmin < other.Right && other.Bottom < this.ymax && this.ymin < other.Top;
		}

		// Token: 0x04001F1C RID: 7964
		private double xmin;

		// Token: 0x04001F1D RID: 7965
		private double ymin;

		// Token: 0x04001F1E RID: 7966
		private double xmax;

		// Token: 0x04001F1F RID: 7967
		private double ymax;
	}
}
