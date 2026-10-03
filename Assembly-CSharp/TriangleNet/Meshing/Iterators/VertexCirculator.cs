using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Meshing.Iterators
{
	// Token: 0x02000227 RID: 551
	public class VertexCirculator
	{
		// Token: 0x06000DD7 RID: 3543 RVA: 0x000AC50C File Offset: 0x000AA70C
		public VertexCirculator(Mesh mesh)
		{
			mesh.MakeVertexMap();
		}

		// Token: 0x06000DD8 RID: 3544 RVA: 0x000AC525 File Offset: 0x000AA725
		public IEnumerable<Vertex> EnumerateVertices(Vertex vertex)
		{
			this.BuildCache(vertex, true);
			foreach (Otri otri in this.cache)
			{
				yield return otri.Dest();
			}
			List<Otri>.Enumerator enumerator = default(List<Otri>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06000DD9 RID: 3545 RVA: 0x000AC53C File Offset: 0x000AA73C
		public IEnumerable<ITriangle> EnumerateTriangles(Vertex vertex)
		{
			this.BuildCache(vertex, false);
			foreach (Otri otri in this.cache)
			{
				yield return otri.tri;
			}
			List<Otri>.Enumerator enumerator = default(List<Otri>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06000DDA RID: 3546 RVA: 0x000AC554 File Offset: 0x000AA754
		private void BuildCache(Vertex vertex, bool vertices)
		{
			this.cache.Clear();
			Otri tri = vertex.tri;
			Otri otri = default(Otri);
			Otri item = default(Otri);
			tri.Copy(ref otri);
			while (otri.tri.id != -1)
			{
				this.cache.Add(otri);
				otri.Copy(ref item);
				otri.Onext();
				if (otri.Equals(tri))
				{
					break;
				}
			}
			if (otri.tri.id == -1)
			{
				tri.Copy(ref otri);
				if (vertices)
				{
					item.Lnext();
					this.cache.Add(item);
				}
				otri.Oprev();
				while (otri.tri.id != -1)
				{
					this.cache.Insert(0, otri);
					otri.Oprev();
					if (otri.Equals(tri))
					{
						break;
					}
				}
			}
		}

		// Token: 0x04001ED9 RID: 7897
		private List<Otri> cache = new List<Otri>();
	}
}
