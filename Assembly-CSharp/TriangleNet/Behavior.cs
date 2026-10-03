using System;
using TriangleNet.Geometry;

namespace TriangleNet
{
	// Token: 0x020001EE RID: 494
	internal class Behavior
	{
		// Token: 0x06000BD2 RID: 3026 RVA: 0x000950A3 File Offset: 0x000932A3
		public Behavior(bool quality = false, double minAngle = 20.0)
		{
			if (quality)
			{
				this.quality = true;
				this.minAngle = minAngle;
				this.Update();
			}
		}

		// Token: 0x06000BD3 RID: 3027 RVA: 0x000950E0 File Offset: 0x000932E0
		private void Update()
		{
			this.quality = true;
			if (this.minAngle < 0.0 || this.minAngle > 60.0)
			{
				this.minAngle = 0.0;
				this.quality = false;
				Log.Instance.Warning("Invalid quality option (minimum angle).", "Mesh.Behavior");
			}
			if (this.maxAngle != 0.0 && (this.maxAngle < 60.0 || this.maxAngle > 180.0))
			{
				this.maxAngle = 0.0;
				this.quality = false;
				Log.Instance.Warning("Invalid quality option (maximum angle).", "Mesh.Behavior");
			}
			this.useSegments = (this.Poly || this.Quality || this.Convex);
			this.goodAngle = Math.Cos(this.MinAngle * 3.141592653589793 / 180.0);
			this.maxGoodAngle = Math.Cos(this.MaxAngle * 3.141592653589793 / 180.0);
			if (this.goodAngle == 1.0)
			{
				this.offconstant = 0.0;
			}
			else
			{
				this.offconstant = 0.475 * Math.Sqrt((1.0 + this.goodAngle) / (1.0 - this.goodAngle));
			}
			this.goodAngle *= this.goodAngle;
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000BD4 RID: 3028 RVA: 0x00095271 File Offset: 0x00093471
		// (set) Token: 0x06000BD5 RID: 3029 RVA: 0x00095278 File Offset: 0x00093478
		public static bool NoExact { get; set; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000BD6 RID: 3030 RVA: 0x00095280 File Offset: 0x00093480
		// (set) Token: 0x06000BD7 RID: 3031 RVA: 0x00095288 File Offset: 0x00093488
		public bool Quality
		{
			get
			{
				return this.quality;
			}
			set
			{
				this.quality = value;
				if (this.quality)
				{
					this.Update();
				}
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000BD8 RID: 3032 RVA: 0x0009529F File Offset: 0x0009349F
		// (set) Token: 0x06000BD9 RID: 3033 RVA: 0x000952A7 File Offset: 0x000934A7
		public double MinAngle
		{
			get
			{
				return this.minAngle;
			}
			set
			{
				this.minAngle = value;
				this.Update();
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000BDA RID: 3034 RVA: 0x000952B6 File Offset: 0x000934B6
		// (set) Token: 0x06000BDB RID: 3035 RVA: 0x000952BE File Offset: 0x000934BE
		public double MaxAngle
		{
			get
			{
				return this.maxAngle;
			}
			set
			{
				this.maxAngle = value;
				this.Update();
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000BDC RID: 3036 RVA: 0x000952CD File Offset: 0x000934CD
		// (set) Token: 0x06000BDD RID: 3037 RVA: 0x000952D5 File Offset: 0x000934D5
		public double MaxArea
		{
			get
			{
				return this.maxArea;
			}
			set
			{
				this.maxArea = value;
				this.fixedArea = (value > 0.0);
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06000BDE RID: 3038 RVA: 0x000952F0 File Offset: 0x000934F0
		// (set) Token: 0x06000BDF RID: 3039 RVA: 0x000952F8 File Offset: 0x000934F8
		public bool VarArea
		{
			get
			{
				return this.varArea;
			}
			set
			{
				this.varArea = value;
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000BE0 RID: 3040 RVA: 0x00095301 File Offset: 0x00093501
		// (set) Token: 0x06000BE1 RID: 3041 RVA: 0x00095309 File Offset: 0x00093509
		public bool Poly
		{
			get
			{
				return this.poly;
			}
			set
			{
				this.poly = value;
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000BE2 RID: 3042 RVA: 0x00095312 File Offset: 0x00093512
		// (set) Token: 0x06000BE3 RID: 3043 RVA: 0x0009531A File Offset: 0x0009351A
		public Func<ITriangle, double, bool> UserTest
		{
			get
			{
				return this.usertest;
			}
			set
			{
				this.usertest = value;
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000BE4 RID: 3044 RVA: 0x00095323 File Offset: 0x00093523
		// (set) Token: 0x06000BE5 RID: 3045 RVA: 0x0009532B File Offset: 0x0009352B
		public bool Convex
		{
			get
			{
				return this.convex;
			}
			set
			{
				this.convex = value;
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000BE6 RID: 3046 RVA: 0x00095334 File Offset: 0x00093534
		// (set) Token: 0x06000BE7 RID: 3047 RVA: 0x0009533C File Offset: 0x0009353C
		public bool ConformingDelaunay
		{
			get
			{
				return this.conformDel;
			}
			set
			{
				this.conformDel = value;
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000BE8 RID: 3048 RVA: 0x00095345 File Offset: 0x00093545
		// (set) Token: 0x06000BE9 RID: 3049 RVA: 0x0009534D File Offset: 0x0009354D
		public int NoBisect
		{
			get
			{
				return this.noBisect;
			}
			set
			{
				this.noBisect = value;
				if (this.noBisect < 0 || this.noBisect > 2)
				{
					this.noBisect = 0;
				}
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000BEA RID: 3050 RVA: 0x0009536F File Offset: 0x0009356F
		// (set) Token: 0x06000BEB RID: 3051 RVA: 0x00095377 File Offset: 0x00093577
		public bool UseBoundaryMarkers
		{
			get
			{
				return this.boundaryMarkers;
			}
			set
			{
				this.boundaryMarkers = value;
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000BEC RID: 3052 RVA: 0x00095380 File Offset: 0x00093580
		// (set) Token: 0x06000BED RID: 3053 RVA: 0x00095388 File Offset: 0x00093588
		public bool NoHoles
		{
			get
			{
				return this.noHoles;
			}
			set
			{
				this.noHoles = value;
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000BEE RID: 3054 RVA: 0x00095391 File Offset: 0x00093591
		// (set) Token: 0x06000BEF RID: 3055 RVA: 0x00095399 File Offset: 0x00093599
		public bool Jettison
		{
			get
			{
				return this.jettison;
			}
			set
			{
				this.jettison = value;
			}
		}

		// Token: 0x04001DBB RID: 7611
		private bool poly;

		// Token: 0x04001DBC RID: 7612
		private bool quality;

		// Token: 0x04001DBD RID: 7613
		private bool varArea;

		// Token: 0x04001DBE RID: 7614
		private bool convex;

		// Token: 0x04001DBF RID: 7615
		private bool jettison;

		// Token: 0x04001DC0 RID: 7616
		private bool boundaryMarkers = true;

		// Token: 0x04001DC1 RID: 7617
		private bool noHoles;

		// Token: 0x04001DC2 RID: 7618
		private bool conformDel;

		// Token: 0x04001DC3 RID: 7619
		private Func<ITriangle, double, bool> usertest;

		// Token: 0x04001DC4 RID: 7620
		private int noBisect;

		// Token: 0x04001DC5 RID: 7621
		private double minAngle;

		// Token: 0x04001DC6 RID: 7622
		private double maxAngle;

		// Token: 0x04001DC7 RID: 7623
		private double maxArea = -1.0;

		// Token: 0x04001DC8 RID: 7624
		internal bool fixedArea;

		// Token: 0x04001DC9 RID: 7625
		internal bool useSegments = true;

		// Token: 0x04001DCA RID: 7626
		internal bool useRegions;

		// Token: 0x04001DCB RID: 7627
		internal double goodAngle;

		// Token: 0x04001DCC RID: 7628
		internal double maxGoodAngle;

		// Token: 0x04001DCD RID: 7629
		internal double offconstant;
	}
}
