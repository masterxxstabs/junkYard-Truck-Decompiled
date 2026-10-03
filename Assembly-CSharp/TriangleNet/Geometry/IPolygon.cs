using System;
using System.Collections.Generic;

namespace TriangleNet.Geometry
{
	// Token: 0x0200023F RID: 575
	public interface IPolygon
	{
		// Token: 0x1700010E RID: 270
		// (get) Token: 0x06000E77 RID: 3703
		List<Vertex> Points { get; }

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000E78 RID: 3704
		List<ISegment> Segments { get; }

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000E79 RID: 3705
		List<Point> Holes { get; }

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000E7A RID: 3706
		List<RegionPointer> Regions { get; }

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000E7B RID: 3707
		// (set) Token: 0x06000E7C RID: 3708
		bool HasPointMarkers { get; set; }

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06000E7D RID: 3709
		// (set) Token: 0x06000E7E RID: 3710
		bool HasSegmentMarkers { get; set; }

		// Token: 0x06000E7F RID: 3711
		[Obsolete("Use polygon.Add(contour) method instead.")]
		void AddContour(IEnumerable<Vertex> points, int marker, bool hole, bool convex);

		// Token: 0x06000E80 RID: 3712
		[Obsolete("Use polygon.Add(contour) method instead.")]
		void AddContour(IEnumerable<Vertex> points, int marker, Point hole);

		// Token: 0x06000E81 RID: 3713
		Rectangle Bounds();

		// Token: 0x06000E82 RID: 3714
		void Add(Vertex vertex);

		// Token: 0x06000E83 RID: 3715
		void Add(ISegment segment, bool insert = false);

		// Token: 0x06000E84 RID: 3716
		void Add(ISegment segment, int index);

		// Token: 0x06000E85 RID: 3717
		void Add(Contour contour, bool hole = false);

		// Token: 0x06000E86 RID: 3718
		void Add(Contour contour, Point hole);
	}
}
