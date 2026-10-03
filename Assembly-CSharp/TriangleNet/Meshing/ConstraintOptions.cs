using System;

namespace TriangleNet.Meshing
{
	// Token: 0x0200021C RID: 540
	public class ConstraintOptions
	{
		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x06000D90 RID: 3472 RVA: 0x000A9DBD File Offset: 0x000A7FBD
		// (set) Token: 0x06000D91 RID: 3473 RVA: 0x000A9DC5 File Offset: 0x000A7FC5
		[Obsolete("Not used anywhere, will be removed in beta 4.")]
		public bool UseRegions { get; set; }

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x06000D92 RID: 3474 RVA: 0x000A9DCE File Offset: 0x000A7FCE
		// (set) Token: 0x06000D93 RID: 3475 RVA: 0x000A9DD6 File Offset: 0x000A7FD6
		public bool ConformingDelaunay { get; set; }

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x06000D94 RID: 3476 RVA: 0x000A9DDF File Offset: 0x000A7FDF
		// (set) Token: 0x06000D95 RID: 3477 RVA: 0x000A9DE7 File Offset: 0x000A7FE7
		public bool Convex { get; set; }

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000D96 RID: 3478 RVA: 0x000A9DF0 File Offset: 0x000A7FF0
		// (set) Token: 0x06000D97 RID: 3479 RVA: 0x000A9DF8 File Offset: 0x000A7FF8
		public int SegmentSplitting { get; set; }
	}
}
