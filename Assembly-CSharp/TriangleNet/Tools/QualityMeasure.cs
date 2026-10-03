using System;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Tools
{
	// Token: 0x02000214 RID: 532
	public class QualityMeasure
	{
		// Token: 0x06000D49 RID: 3401 RVA: 0x000A71E4 File Offset: 0x000A53E4
		public QualityMeasure()
		{
			this.areaMeasure = new QualityMeasure.AreaMeasure();
			this.alphaMeasure = new QualityMeasure.AlphaMeasure();
			this.qMeasure = new QualityMeasure.Q_Measure();
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x06000D4A RID: 3402 RVA: 0x000A720D File Offset: 0x000A540D
		public double AreaMinimum
		{
			get
			{
				return this.areaMeasure.area_min;
			}
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000D4B RID: 3403 RVA: 0x000A721A File Offset: 0x000A541A
		public double AreaMaximum
		{
			get
			{
				return this.areaMeasure.area_max;
			}
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000D4C RID: 3404 RVA: 0x000A7227 File Offset: 0x000A5427
		public double AreaRatio
		{
			get
			{
				return this.areaMeasure.area_max / this.areaMeasure.area_min;
			}
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000D4D RID: 3405 RVA: 0x000A7240 File Offset: 0x000A5440
		public double AlphaMinimum
		{
			get
			{
				return this.alphaMeasure.alpha_min;
			}
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000D4E RID: 3406 RVA: 0x000A724D File Offset: 0x000A544D
		public double AlphaMaximum
		{
			get
			{
				return this.alphaMeasure.alpha_max;
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x06000D4F RID: 3407 RVA: 0x000A725A File Offset: 0x000A545A
		public double AlphaAverage
		{
			get
			{
				return this.alphaMeasure.alpha_ave;
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x06000D50 RID: 3408 RVA: 0x000A7267 File Offset: 0x000A5467
		public double AlphaArea
		{
			get
			{
				return this.alphaMeasure.alpha_area;
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x06000D51 RID: 3409 RVA: 0x000A7274 File Offset: 0x000A5474
		public double Q_Minimum
		{
			get
			{
				return this.qMeasure.q_min;
			}
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000D52 RID: 3410 RVA: 0x000A7281 File Offset: 0x000A5481
		public double Q_Maximum
		{
			get
			{
				return this.qMeasure.q_max;
			}
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06000D53 RID: 3411 RVA: 0x000A728E File Offset: 0x000A548E
		public double Q_Average
		{
			get
			{
				return this.qMeasure.q_ave;
			}
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000D54 RID: 3412 RVA: 0x000A729B File Offset: 0x000A549B
		public double Q_Area
		{
			get
			{
				return this.qMeasure.q_area;
			}
		}

		// Token: 0x06000D55 RID: 3413 RVA: 0x000A72A8 File Offset: 0x000A54A8
		public void Update(Mesh mesh)
		{
			this.mesh = mesh;
			this.areaMeasure.Reset();
			this.alphaMeasure.Reset();
			this.qMeasure.Reset();
			this.Compute();
		}

		// Token: 0x06000D56 RID: 3414 RVA: 0x000A72D8 File Offset: 0x000A54D8
		private void Compute()
		{
			int num = 0;
			foreach (Triangle triangle in this.mesh.triangles)
			{
				num++;
				Point point = triangle.vertices[0];
				Point point2 = triangle.vertices[1];
				Point point3 = triangle.vertices[2];
				double num2 = point.x - point2.x;
				double num3 = point.y - point2.y;
				double ab = Math.Sqrt(num2 * num2 + num3 * num3);
				double num4 = point2.x - point3.x;
				num3 = point2.y - point3.y;
				double bc = Math.Sqrt(num4 * num4 + num3 * num3);
				double num5 = point3.x - point.x;
				num3 = point3.y - point.y;
				double ca = Math.Sqrt(num5 * num5 + num3 * num3);
				double area = this.areaMeasure.Measure(point, point2, point3);
				this.alphaMeasure.Measure(ab, bc, ca, area);
				this.qMeasure.Measure(ab, bc, ca, area);
			}
			this.alphaMeasure.Normalize(num, this.areaMeasure.area_total);
			this.qMeasure.Normalize(num, this.areaMeasure.area_total);
		}

		// Token: 0x06000D57 RID: 3415 RVA: 0x000A7438 File Offset: 0x000A5638
		public int Bandwidth()
		{
			if (this.mesh == null)
			{
				return 0;
			}
			int num = 0;
			int num2 = 0;
			foreach (Triangle triangle in this.mesh.triangles)
			{
				for (int i = 0; i < 3; i++)
				{
					int id = triangle.GetVertex(i).id;
					for (int j = 0; j < 3; j++)
					{
						int id2 = triangle.GetVertex(j).id;
						num2 = Math.Max(num2, id2 - id);
						num = Math.Max(num, id - id2);
					}
				}
			}
			return num + 1 + num2;
		}

		// Token: 0x04001E8F RID: 7823
		private QualityMeasure.AreaMeasure areaMeasure;

		// Token: 0x04001E90 RID: 7824
		private QualityMeasure.AlphaMeasure alphaMeasure;

		// Token: 0x04001E91 RID: 7825
		private QualityMeasure.Q_Measure qMeasure;

		// Token: 0x04001E92 RID: 7826
		private Mesh mesh;

		// Token: 0x02000493 RID: 1171
		private class AreaMeasure
		{
			// Token: 0x06001A9F RID: 6815 RVA: 0x000F712B File Offset: 0x000F532B
			public void Reset()
			{
				this.area_min = double.MaxValue;
				this.area_max = double.MinValue;
				this.area_total = 0.0;
				this.area_zero = 0;
			}

			// Token: 0x06001AA0 RID: 6816 RVA: 0x000F7164 File Offset: 0x000F5364
			public double Measure(Point a, Point b, Point c)
			{
				double num = 0.5 * Math.Abs(a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y));
				this.area_min = Math.Min(this.area_min, num);
				this.area_max = Math.Max(this.area_max, num);
				this.area_total += num;
				if (num == 0.0)
				{
					this.area_zero++;
				}
				return num;
			}

			// Token: 0x04002B7B RID: 11131
			public double area_min = double.MaxValue;

			// Token: 0x04002B7C RID: 11132
			public double area_max = double.MinValue;

			// Token: 0x04002B7D RID: 11133
			public double area_total;

			// Token: 0x04002B7E RID: 11134
			public int area_zero;
		}

		// Token: 0x02000494 RID: 1172
		private class AlphaMeasure
		{
			// Token: 0x06001AA2 RID: 6818 RVA: 0x000F7232 File Offset: 0x000F5432
			public void Reset()
			{
				this.alpha_min = double.MaxValue;
				this.alpha_max = double.MinValue;
				this.alpha_ave = 0.0;
				this.alpha_area = 0.0;
			}

			// Token: 0x06001AA3 RID: 6819 RVA: 0x000F7270 File Offset: 0x000F5470
			private double acos(double c)
			{
				if (c <= -1.0)
				{
					return 3.141592653589793;
				}
				if (1.0 <= c)
				{
					return 0.0;
				}
				return Math.Acos(c);
			}

			// Token: 0x06001AA4 RID: 6820 RVA: 0x000F72A4 File Offset: 0x000F54A4
			public double Measure(double ab, double bc, double ca, double area)
			{
				double num = double.MaxValue;
				double num2 = ab * ab;
				double num3 = bc * bc;
				double num4 = ca * ca;
				double val;
				double val2;
				double val3;
				if (ab == 0.0 && bc == 0.0 && ca == 0.0)
				{
					val = 2.0943951023931953;
					val2 = 2.0943951023931953;
					val3 = 2.0943951023931953;
				}
				else
				{
					if (ca == 0.0 || ab == 0.0)
					{
						val = 3.141592653589793;
					}
					else
					{
						val = this.acos((num4 + num2 - num3) / (2.0 * ca * ab));
					}
					if (ab == 0.0 || bc == 0.0)
					{
						val2 = 3.141592653589793;
					}
					else
					{
						val2 = this.acos((num2 + num3 - num4) / (2.0 * ab * bc));
					}
					if (bc == 0.0 || ca == 0.0)
					{
						val3 = 3.141592653589793;
					}
					else
					{
						val3 = this.acos((num3 + num4 - num2) / (2.0 * bc * ca));
					}
				}
				num = Math.Min(num, val);
				num = Math.Min(num, val2);
				num = Math.Min(num, val3);
				num = num * 3.0 / 3.141592653589793;
				this.alpha_ave += num;
				this.alpha_area += area * num;
				this.alpha_min = Math.Min(num, this.alpha_min);
				this.alpha_max = Math.Max(num, this.alpha_max);
				return num;
			}

			// Token: 0x06001AA5 RID: 6821 RVA: 0x000F7448 File Offset: 0x000F5648
			public void Normalize(int n, double area_total)
			{
				if (n > 0)
				{
					this.alpha_ave /= (double)n;
				}
				else
				{
					this.alpha_ave = 0.0;
				}
				if (0.0 < area_total)
				{
					this.alpha_area /= area_total;
					return;
				}
				this.alpha_area = 0.0;
			}

			// Token: 0x04002B7F RID: 11135
			public double alpha_min;

			// Token: 0x04002B80 RID: 11136
			public double alpha_max;

			// Token: 0x04002B81 RID: 11137
			public double alpha_ave;

			// Token: 0x04002B82 RID: 11138
			public double alpha_area;
		}

		// Token: 0x02000495 RID: 1173
		private class Q_Measure
		{
			// Token: 0x06001AA7 RID: 6823 RVA: 0x000F74A3 File Offset: 0x000F56A3
			public void Reset()
			{
				this.q_min = double.MaxValue;
				this.q_max = double.MinValue;
				this.q_ave = 0.0;
				this.q_area = 0.0;
			}

			// Token: 0x06001AA8 RID: 6824 RVA: 0x000F74E4 File Offset: 0x000F56E4
			public double Measure(double ab, double bc, double ca, double area)
			{
				double num = (bc + ca - ab) * (ca + ab - bc) * (ab + bc - ca) / (ab * bc * ca);
				this.q_min = Math.Min(this.q_min, num);
				this.q_max = Math.Max(this.q_max, num);
				this.q_ave += num;
				this.q_area += num * area;
				return num;
			}

			// Token: 0x06001AA9 RID: 6825 RVA: 0x000F7550 File Offset: 0x000F5750
			public void Normalize(int n, double area_total)
			{
				if (n > 0)
				{
					this.q_ave /= (double)n;
				}
				else
				{
					this.q_ave = 0.0;
				}
				if (area_total > 0.0)
				{
					this.q_area /= area_total;
					return;
				}
				this.q_area = 0.0;
			}

			// Token: 0x04002B83 RID: 11139
			public double q_min;

			// Token: 0x04002B84 RID: 11140
			public double q_max;

			// Token: 0x04002B85 RID: 11141
			public double q_ave;

			// Token: 0x04002B86 RID: 11142
			public double q_area;
		}
	}
}
