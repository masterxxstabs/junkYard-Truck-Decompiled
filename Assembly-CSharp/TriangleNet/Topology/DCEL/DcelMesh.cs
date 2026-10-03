using System;
using System.Collections.Generic;
using TriangleNet.Geometry;

namespace TriangleNet.Topology.DCEL
{
	// Token: 0x0200020B RID: 523
	public class DcelMesh
	{
		// Token: 0x06000D00 RID: 3328 RVA: 0x000A55E4 File Offset: 0x000A37E4
		public DcelMesh() : this(true)
		{
		}

		// Token: 0x06000D01 RID: 3329 RVA: 0x000A55ED File Offset: 0x000A37ED
		protected DcelMesh(bool initialize)
		{
			if (initialize)
			{
				this.vertices = new List<Vertex>();
				this.edges = new List<HalfEdge>();
				this.faces = new List<Face>();
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000D02 RID: 3330 RVA: 0x000A5619 File Offset: 0x000A3819
		public List<Vertex> Vertices
		{
			get
			{
				return this.vertices;
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000D03 RID: 3331 RVA: 0x000A5621 File Offset: 0x000A3821
		public List<HalfEdge> HalfEdges
		{
			get
			{
				return this.edges;
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000D04 RID: 3332 RVA: 0x000A5629 File Offset: 0x000A3829
		public List<Face> Faces
		{
			get
			{
				return this.faces;
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000D05 RID: 3333 RVA: 0x000A5631 File Offset: 0x000A3831
		public IEnumerable<IEdge> Edges
		{
			get
			{
				return this.EnumerateEdges();
			}
		}

		// Token: 0x06000D06 RID: 3334 RVA: 0x000A563C File Offset: 0x000A383C
		public virtual bool IsConsistent(bool closed = true, int depth = 0)
		{
			foreach (Vertex vertex in this.vertices)
			{
				if (vertex.id >= 0)
				{
					if (vertex.leaving == null)
					{
						return false;
					}
					if (vertex.Leaving.Origin.id != vertex.id)
					{
						return false;
					}
				}
			}
			foreach (Face face in this.faces)
			{
				if (face.ID >= 0)
				{
					if (face.edge == null)
					{
						return false;
					}
					if (face.id != face.edge.face.id)
					{
						return false;
					}
				}
			}
			foreach (HalfEdge halfEdge in this.edges)
			{
				if (halfEdge.id >= 0)
				{
					if (halfEdge.twin == null)
					{
						return false;
					}
					if (halfEdge.origin == null)
					{
						return false;
					}
					if (halfEdge.face == null)
					{
						return false;
					}
					if (closed && halfEdge.next == null)
					{
						return false;
					}
				}
			}
			foreach (HalfEdge halfEdge2 in this.edges)
			{
				if (halfEdge2.id >= 0)
				{
					HalfEdge twin = halfEdge2.twin;
					HalfEdge next = halfEdge2.next;
					if (halfEdge2.id != twin.twin.id)
					{
						return false;
					}
					if (closed)
					{
						if (next.origin.id != twin.origin.id)
						{
							return false;
						}
						if (next.twin.next.origin.id != halfEdge2.twin.origin.id)
						{
							return false;
						}
					}
				}
			}
			if (closed && depth > 0)
			{
				foreach (Face face2 in this.faces)
				{
					if (face2.id >= 0)
					{
						HalfEdge edge = face2.edge;
						HalfEdge next2 = edge.next;
						int id = edge.id;
						int num = 0;
						while (next2.id != id && num < depth)
						{
							next2 = next2.next;
							num++;
						}
						if (next2.id != id)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		// Token: 0x06000D07 RID: 3335 RVA: 0x000A5938 File Offset: 0x000A3B38
		public void ResolveBoundaryEdges()
		{
			Dictionary<int, HalfEdge> dictionary = new Dictionary<int, HalfEdge>();
			foreach (HalfEdge halfEdge in this.edges)
			{
				if (halfEdge.twin == null)
				{
					HalfEdge halfEdge2 = halfEdge.twin = new HalfEdge(halfEdge.next.origin, Face.Empty);
					halfEdge2.twin = halfEdge;
					dictionary.Add(halfEdge2.origin.id, halfEdge2);
				}
			}
			int count = this.edges.Count;
			foreach (HalfEdge halfEdge3 in dictionary.Values)
			{
				halfEdge3.id = count++;
				halfEdge3.next = dictionary[halfEdge3.twin.origin.id];
				this.edges.Add(halfEdge3);
			}
		}

		// Token: 0x06000D08 RID: 3336 RVA: 0x000A5A50 File Offset: 0x000A3C50
		protected virtual IEnumerable<IEdge> EnumerateEdges()
		{
			List<IEdge> list = new List<IEdge>(this.edges.Count / 2);
			foreach (HalfEdge halfEdge in this.edges)
			{
				HalfEdge twin = halfEdge.twin;
				if (halfEdge.id < twin.id)
				{
					list.Add(new Edge(halfEdge.origin.id, twin.origin.id));
				}
			}
			return list;
		}

		// Token: 0x04001E7B RID: 7803
		protected List<Vertex> vertices;

		// Token: 0x04001E7C RID: 7804
		protected List<HalfEdge> edges;

		// Token: 0x04001E7D RID: 7805
		protected List<Face> faces;
	}
}
