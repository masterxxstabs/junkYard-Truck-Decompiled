using System;
using System.Collections.Generic;

namespace TriangleNet.Geometry
{
	// Token: 0x02000243 RID: 579
	public class Polygon : IPolygon
	{
		// Token: 0x1700011C RID: 284
		// (get) Token: 0x06000EA8 RID: 3752 RVA: 0x000B14B0 File Offset: 0x000AF6B0
		public List<Vertex> Points
		{
			get
			{
				return this.points;
			}
		}

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x06000EA9 RID: 3753 RVA: 0x000B14B8 File Offset: 0x000AF6B8
		public List<Point> Holes
		{
			get
			{
				return this.holes;
			}
		}

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06000EAA RID: 3754 RVA: 0x000B14C0 File Offset: 0x000AF6C0
		public List<RegionPointer> Regions
		{
			get
			{
				return this.regions;
			}
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x06000EAB RID: 3755 RVA: 0x000B14C8 File Offset: 0x000AF6C8
		public List<ISegment> Segments
		{
			get
			{
				return this.segments;
			}
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x06000EAC RID: 3756 RVA: 0x000B14D0 File Offset: 0x000AF6D0
		// (set) Token: 0x06000EAD RID: 3757 RVA: 0x000B14D8 File Offset: 0x000AF6D8
		public bool HasPointMarkers { get; set; }

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x06000EAE RID: 3758 RVA: 0x000B14E1 File Offset: 0x000AF6E1
		// (set) Token: 0x06000EAF RID: 3759 RVA: 0x000B14E9 File Offset: 0x000AF6E9
		public bool HasSegmentMarkers { get; set; }

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x06000EB0 RID: 3760 RVA: 0x000B14F2 File Offset: 0x000AF6F2
		public int Count
		{
			get
			{
				return this.points.Count;
			}
		}

		// Token: 0x06000EB1 RID: 3761 RVA: 0x000B14FF File Offset: 0x000AF6FF
		public Polygon() : this(3, false)
		{
		}

		// Token: 0x06000EB2 RID: 3762 RVA: 0x000B14FF File Offset: 0x000AF6FF
		public Polygon(int capacity) : this(3, false)
		{
		}

		// Token: 0x06000EB3 RID: 3763 RVA: 0x000B150C File Offset: 0x000AF70C
		public Polygon(int capacity, bool markers)
		{
			this.points = new List<Vertex>(capacity);
			this.holes = new List<Point>();
			this.regions = new List<RegionPointer>();
			this.segments = new List<ISegment>();
			this.HasPointMarkers = markers;
			this.HasSegmentMarkers = markers;
		}

		// Token: 0x06000EB4 RID: 3764 RVA: 0x000B155A File Offset: 0x000AF75A
		[Obsolete("Use polygon.Add(contour) method instead.")]
		public void AddContour(IEnumerable<Vertex> points, int marker = 0, bool hole = false, bool convex = false)
		{
			this.Add(new Contour(points, marker, convex), hole);
		}

		// Token: 0x06000EB5 RID: 3765 RVA: 0x000B156C File Offset: 0x000AF76C
		[Obsolete("Use polygon.Add(contour) method instead.")]
		public void AddContour(IEnumerable<Vertex> points, int marker, Point hole)
		{
			this.Add(new Contour(points, marker), hole);
		}

		// Token: 0x06000EB6 RID: 3766 RVA: 0x000B157C File Offset: 0x000AF77C
		public Rectangle Bounds()
		{
			List<Point> list = this.points.ConvertAll<Point>((Vertex x) => x);
			Rectangle rectangle = new Rectangle();
			rectangle.Expand(list);
			return rectangle;
		}

		// Token: 0x06000EB7 RID: 3767 RVA: 0x000B15C0 File Offset: 0x000AF7C0
		public void Add(Vertex vertex)
		{
			this.points.Add(vertex);
		}

		// Token: 0x06000EB8 RID: 3768 RVA: 0x000B15CE File Offset: 0x000AF7CE
		public void Add(ISegment segment, bool insert = false)
		{
			this.segments.Add(segment);
			if (insert)
			{
				this.points.Add(segment.GetVertex(0));
				this.points.Add(segment.GetVertex(1));
			}
		}

		// Token: 0x06000EB9 RID: 3769 RVA: 0x000B1603 File Offset: 0x000AF803
		public void Add(ISegment segment, int index)
		{
			this.segments.Add(segment);
			this.points.Add(segment.GetVertex(index));
		}

		// Token: 0x06000EBA RID: 3770 RVA: 0x000B1623 File Offset: 0x000AF823
		public void Add(Contour contour, bool hole = false)
		{
			if (hole)
			{
				this.Add(contour, contour.FindInteriorPoint(5, 2E-05));
				return;
			}
			this.points.AddRange(contour.Points);
			this.segments.AddRange(contour.GetSegments());
		}

		// Token: 0x06000EBB RID: 3771 RVA: 0x000B1662 File Offset: 0x000AF862
		public void Add(Contour contour, Point hole)
		{
			this.points.AddRange(contour.Points);
			this.segments.AddRange(contour.GetSegments());
			this.holes.Add(hole);
		}

		// Token: 0x04001F16 RID: 7958
		private List<Vertex> points;

		// Token: 0x04001F17 RID: 7959
		private List<Point> holes;

		// Token: 0x04001F18 RID: 7960
		private List<RegionPointer> regions;

		// Token: 0x04001F19 RID: 7961
		private List<ISegment> segments;
	}
}
