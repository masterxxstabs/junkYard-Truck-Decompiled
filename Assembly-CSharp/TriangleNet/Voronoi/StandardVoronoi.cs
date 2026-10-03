using System;
using TriangleNet.Geometry;
using TriangleNet.Tools;
using TriangleNet.Topology.DCEL;

namespace TriangleNet.Voronoi
{
	// Token: 0x02000201 RID: 513
	public class StandardVoronoi : VoronoiBase
	{
		// Token: 0x06000C7E RID: 3198 RVA: 0x000A283C File Offset: 0x000A0A3C
		public StandardVoronoi(Mesh mesh) : this(mesh, mesh.bounds, new DefaultVoronoiFactory(), RobustPredicates.Default)
		{
		}

		// Token: 0x06000C7F RID: 3199 RVA: 0x000A2855 File Offset: 0x000A0A55
		public StandardVoronoi(Mesh mesh, Rectangle box) : this(mesh, box, new DefaultVoronoiFactory(), RobustPredicates.Default)
		{
		}

		// Token: 0x06000C80 RID: 3200 RVA: 0x000A2869 File Offset: 0x000A0A69
		public StandardVoronoi(Mesh mesh, Rectangle box, IVoronoiFactory factory, IPredicates predicates) : base(mesh, factory, predicates, true)
		{
			box.Expand(mesh.bounds);
			this.PostProcess(box);
		}

		// Token: 0x06000C81 RID: 3201 RVA: 0x000A288C File Offset: 0x000A0A8C
		private void PostProcess(Rectangle box)
		{
			foreach (HalfEdge halfEdge in this.rays)
			{
				Point origin = halfEdge.origin;
				Point origin2 = halfEdge.twin.origin;
				if (box.Contains(origin) || box.Contains(origin2))
				{
					IntersectionHelper.BoxRayIntersection(box, origin, origin2, ref origin2);
				}
			}
		}
	}
}
