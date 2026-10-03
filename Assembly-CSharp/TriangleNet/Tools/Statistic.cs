using System;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Tools
{
	// Token: 0x02000215 RID: 533
	public class Statistic
	{
		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000D58 RID: 3416 RVA: 0x000A74F0 File Offset: 0x000A56F0
		public double ShortestEdge
		{
			get
			{
				return this.minEdge;
			}
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000D59 RID: 3417 RVA: 0x000A74F8 File Offset: 0x000A56F8
		public double LongestEdge
		{
			get
			{
				return this.maxEdge;
			}
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000D5A RID: 3418 RVA: 0x000A7500 File Offset: 0x000A5700
		public double ShortestAltitude
		{
			get
			{
				return this.minAspect;
			}
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000D5B RID: 3419 RVA: 0x000A7508 File Offset: 0x000A5708
		public double LargestAspectRatio
		{
			get
			{
				return this.maxAspect;
			}
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000D5C RID: 3420 RVA: 0x000A7510 File Offset: 0x000A5710
		public double SmallestArea
		{
			get
			{
				return this.minArea;
			}
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000D5D RID: 3421 RVA: 0x000A7518 File Offset: 0x000A5718
		public double LargestArea
		{
			get
			{
				return this.maxArea;
			}
		}

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x06000D5E RID: 3422 RVA: 0x000A7520 File Offset: 0x000A5720
		public double SmallestAngle
		{
			get
			{
				return this.minAngle;
			}
		}

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06000D5F RID: 3423 RVA: 0x000A7528 File Offset: 0x000A5728
		public double LargestAngle
		{
			get
			{
				return this.maxAngle;
			}
		}

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000D60 RID: 3424 RVA: 0x000A7530 File Offset: 0x000A5730
		public int[] AngleHistogram
		{
			get
			{
				return this.angleTable;
			}
		}

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000D61 RID: 3425 RVA: 0x000A7538 File Offset: 0x000A5738
		public int[] MinAngleHistogram
		{
			get
			{
				return this.minAngles;
			}
		}

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000D62 RID: 3426 RVA: 0x000A7540 File Offset: 0x000A5740
		public int[] MaxAngleHistogram
		{
			get
			{
				return this.maxAngles;
			}
		}

		// Token: 0x06000D63 RID: 3427 RVA: 0x000A7548 File Offset: 0x000A5748
		private void GetAspectHistogram(Mesh mesh)
		{
			int[] array = new int[16];
			double[] array2 = new double[]
			{
				1.5,
				2.0,
				2.5,
				3.0,
				4.0,
				6.0,
				10.0,
				15.0,
				25.0,
				50.0,
				100.0,
				300.0,
				1000.0,
				10000.0,
				100000.0,
				0.0
			};
			Otri otri = default(Otri);
			Vertex[] array3 = new Vertex[3];
			double[] array4 = new double[3];
			double[] array5 = new double[3];
			double[] array6 = new double[3];
			otri.orient = 0;
			foreach (Triangle tri in mesh.triangles)
			{
				otri.tri = tri;
				array3[0] = otri.Org();
				array3[1] = otri.Dest();
				array3[2] = otri.Apex();
				double num = 0.0;
				for (int i = 0; i < 3; i++)
				{
					int num2 = Statistic.plus1Mod3[i];
					int num3 = Statistic.minus1Mod3[i];
					array4[i] = array3[num2].x - array3[num3].x;
					array5[i] = array3[num2].y - array3[num3].y;
					array6[i] = array4[i] * array4[i] + array5[i] * array5[i];
					if (array6[i] > num)
					{
						num = array6[i];
					}
				}
				double num4 = Math.Abs((array3[2].x - array3[0].x) * (array3[1].y - array3[0].y) - (array3[1].x - array3[0].x) * (array3[2].y - array3[0].y)) / 2.0;
				double num5 = num4 * num4 / num;
				double num6 = num / num5;
				int num7 = 0;
				while (num6 > array2[num7] * array2[num7] && num7 < 15)
				{
					num7++;
				}
				array[num7]++;
			}
		}

		// Token: 0x06000D64 RID: 3428 RVA: 0x000A7734 File Offset: 0x000A5934
		public void Update(Mesh mesh, int sampleDegrees)
		{
			Point[] array = new Point[3];
			sampleDegrees = 60;
			double[] array2 = new double[sampleDegrees / 2 - 1];
			double[] array3 = new double[3];
			double[] array4 = new double[3];
			double[] array5 = new double[3];
			double num = 3.141592653589793 / (double)sampleDegrees;
			double num2 = 57.29577951308232;
			this.angleTable = new int[sampleDegrees];
			this.minAngles = new int[sampleDegrees];
			this.maxAngles = new int[sampleDegrees];
			for (int i = 0; i < sampleDegrees / 2 - 1; i++)
			{
				array2[i] = Math.Cos(num * (double)(i + 1));
				array2[i] *= array2[i];
			}
			for (int j = 0; j < sampleDegrees; j++)
			{
				this.angleTable[j] = 0;
			}
			this.minAspect = mesh.bounds.Width + mesh.bounds.Height;
			this.minAspect *= this.minAspect;
			this.maxAspect = 0.0;
			this.minEdge = this.minAspect;
			this.maxEdge = 0.0;
			this.minArea = this.minAspect;
			this.maxArea = 0.0;
			this.minAngle = 0.0;
			this.maxAngle = 2.0;
			bool flag = true;
			bool flag2 = true;
			foreach (Triangle triangle in mesh.triangles)
			{
				double num3 = 0.0;
				double num4 = 1.0;
				array[0] = triangle.vertices[0];
				array[1] = triangle.vertices[1];
				array[2] = triangle.vertices[2];
				double num5 = 0.0;
				for (int k = 0; k < 3; k++)
				{
					int num6 = Statistic.plus1Mod3[k];
					int num7 = Statistic.minus1Mod3[k];
					array3[k] = array[num6].x - array[num7].x;
					array4[k] = array[num6].y - array[num7].y;
					array5[k] = array3[k] * array3[k] + array4[k] * array4[k];
					if (array5[k] > num5)
					{
						num5 = array5[k];
					}
					if (array5[k] > this.maxEdge)
					{
						this.maxEdge = array5[k];
					}
					if (array5[k] < this.minEdge)
					{
						this.minEdge = array5[k];
					}
				}
				double num8 = Math.Abs((array[2].x - array[0].x) * (array[1].y - array[0].y) - (array[1].x - array[0].x) * (array[2].y - array[0].y));
				if (num8 < this.minArea)
				{
					this.minArea = num8;
				}
				if (num8 > this.maxArea)
				{
					this.maxArea = num8;
				}
				double num9 = num8 * num8 / num5;
				if (num9 < this.minAspect)
				{
					this.minAspect = num9;
				}
				double num10 = num5 / num9;
				if (num10 > this.maxAspect)
				{
					this.maxAspect = num10;
				}
				int num13;
				for (int l = 0; l < 3; l++)
				{
					int num6 = Statistic.plus1Mod3[l];
					int num7 = Statistic.minus1Mod3[l];
					double num11 = array3[num6] * array3[num7] + array4[num6] * array4[num7];
					double num12 = num11 * num11 / (array5[num6] * array5[num7]);
					num13 = sampleDegrees / 2 - 1;
					for (int m = num13 - 1; m >= 0; m--)
					{
						if (num12 > array2[m])
						{
							num13 = m;
						}
					}
					if (num11 <= 0.0)
					{
						this.angleTable[num13]++;
						if (num12 > this.minAngle)
						{
							this.minAngle = num12;
						}
						if (flag && num12 < this.maxAngle)
						{
							this.maxAngle = num12;
						}
						if (num12 > num3)
						{
							num3 = num12;
						}
						if (flag2 && num12 < num4)
						{
							num4 = num12;
						}
					}
					else
					{
						this.angleTable[sampleDegrees - num13 - 1]++;
						if (flag || num12 > this.maxAngle)
						{
							this.maxAngle = num12;
							flag = false;
						}
						if (flag2 || num12 > num4)
						{
							num4 = num12;
							flag2 = false;
						}
					}
				}
				num13 = sampleDegrees / 2 - 1;
				for (int n = num13 - 1; n >= 0; n--)
				{
					if (num3 > array2[n])
					{
						num13 = n;
					}
				}
				this.minAngles[num13]++;
				num13 = sampleDegrees / 2 - 1;
				for (int num14 = num13 - 1; num14 >= 0; num14--)
				{
					if (num4 > array2[num14])
					{
						num13 = num14;
					}
				}
				if (flag2)
				{
					this.maxAngles[num13]++;
				}
				else
				{
					this.maxAngles[sampleDegrees - num13 - 1]++;
				}
				flag2 = true;
			}
			this.minEdge = Math.Sqrt(this.minEdge);
			this.maxEdge = Math.Sqrt(this.maxEdge);
			this.minAspect = Math.Sqrt(this.minAspect);
			this.maxAspect = Math.Sqrt(this.maxAspect);
			this.minArea *= 0.5;
			this.maxArea *= 0.5;
			if (this.minAngle >= 1.0)
			{
				this.minAngle = 0.0;
			}
			else
			{
				this.minAngle = num2 * Math.Acos(Math.Sqrt(this.minAngle));
			}
			if (this.maxAngle >= 1.0)
			{
				this.maxAngle = 180.0;
				return;
			}
			if (flag)
			{
				this.maxAngle = num2 * Math.Acos(Math.Sqrt(this.maxAngle));
				return;
			}
			this.maxAngle = 180.0 - num2 * Math.Acos(Math.Sqrt(this.maxAngle));
		}

		// Token: 0x06000D66 RID: 3430 RVA: 0x000A7D34 File Offset: 0x000A5F34
		// Note: this type is marked as 'beforefieldinit'.
		static Statistic()
		{
			int[] array = new int[3];
			array[0] = 1;
			array[1] = 2;
			Statistic.plus1Mod3 = array;
			Statistic.minus1Mod3 = new int[]
			{
				2,
				0,
				1
			};
		}

		// Token: 0x04001E93 RID: 7827
		public static long InCircleCount = 0L;

		// Token: 0x04001E94 RID: 7828
		public static long InCircleAdaptCount = 0L;

		// Token: 0x04001E95 RID: 7829
		public static long CounterClockwiseCount = 0L;

		// Token: 0x04001E96 RID: 7830
		public static long CounterClockwiseAdaptCount = 0L;

		// Token: 0x04001E97 RID: 7831
		public static long Orient3dCount = 0L;

		// Token: 0x04001E98 RID: 7832
		public static long HyperbolaCount = 0L;

		// Token: 0x04001E99 RID: 7833
		public static long CircumcenterCount = 0L;

		// Token: 0x04001E9A RID: 7834
		public static long CircleTopCount = 0L;

		// Token: 0x04001E9B RID: 7835
		public static long RelocationCount = 0L;

		// Token: 0x04001E9C RID: 7836
		private double minEdge;

		// Token: 0x04001E9D RID: 7837
		private double maxEdge;

		// Token: 0x04001E9E RID: 7838
		private double minAspect;

		// Token: 0x04001E9F RID: 7839
		private double maxAspect;

		// Token: 0x04001EA0 RID: 7840
		private double minArea;

		// Token: 0x04001EA1 RID: 7841
		private double maxArea;

		// Token: 0x04001EA2 RID: 7842
		private double minAngle;

		// Token: 0x04001EA3 RID: 7843
		private double maxAngle;

		// Token: 0x04001EA4 RID: 7844
		private int[] angleTable;

		// Token: 0x04001EA5 RID: 7845
		private int[] minAngles;

		// Token: 0x04001EA6 RID: 7846
		private int[] maxAngles;

		// Token: 0x04001EA7 RID: 7847
		private static readonly int[] plus1Mod3;

		// Token: 0x04001EA8 RID: 7848
		private static readonly int[] minus1Mod3;
	}
}
