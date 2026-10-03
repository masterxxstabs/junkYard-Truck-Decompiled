using System;
using TriangleNet.Geometry;
using TriangleNet.Topology.DCEL;
using TriangleNet.Voronoi;

namespace TriangleNet.Smoothing
{
	// Token: 0x0200021A RID: 538
	internal class VoronoiFactory : IVoronoiFactory
	{
		// Token: 0x06000D7D RID: 3453 RVA: 0x000A87B9 File Offset: 0x000A69B9
		public VoronoiFactory()
		{
			this.vertices = new VoronoiFactory.ObjectPool<TriangleNet.Topology.DCEL.Vertex>(3);
			this.edges = new VoronoiFactory.ObjectPool<HalfEdge>(3);
			this.faces = new VoronoiFactory.ObjectPool<Face>(3);
		}

		// Token: 0x06000D7E RID: 3454 RVA: 0x000A87E8 File Offset: 0x000A69E8
		public void Initialize(int vertexCount, int edgeCount, int faceCount)
		{
			this.vertices.Capacity = vertexCount;
			this.edges.Capacity = edgeCount;
			this.faces.Capacity = faceCount;
			for (int i = this.vertices.Count; i < vertexCount; i++)
			{
				this.vertices.Put(new TriangleNet.Topology.DCEL.Vertex(0.0, 0.0));
			}
			for (int j = this.edges.Count; j < edgeCount; j++)
			{
				this.edges.Put(new HalfEdge(null));
			}
			for (int k = this.faces.Count; k < faceCount; k++)
			{
				this.faces.Put(new Face(null));
			}
			this.Reset();
		}

		// Token: 0x06000D7F RID: 3455 RVA: 0x000A88A5 File Offset: 0x000A6AA5
		public void Reset()
		{
			this.vertices.Release();
			this.edges.Release();
			this.faces.Release();
		}

		// Token: 0x06000D80 RID: 3456 RVA: 0x000A88C8 File Offset: 0x000A6AC8
		public TriangleNet.Topology.DCEL.Vertex CreateVertex(double x, double y)
		{
			TriangleNet.Topology.DCEL.Vertex vertex;
			if (this.vertices.TryGet(out vertex))
			{
				vertex.x = x;
				vertex.y = y;
				vertex.leaving = null;
				return vertex;
			}
			vertex = new TriangleNet.Topology.DCEL.Vertex(x, y);
			this.vertices.Put(vertex);
			return vertex;
		}

		// Token: 0x06000D81 RID: 3457 RVA: 0x000A8910 File Offset: 0x000A6B10
		public HalfEdge CreateHalfEdge(TriangleNet.Topology.DCEL.Vertex origin, Face face)
		{
			HalfEdge halfEdge;
			if (this.edges.TryGet(out halfEdge))
			{
				halfEdge.origin = origin;
				halfEdge.face = face;
				halfEdge.next = null;
				halfEdge.twin = null;
				if (face != null && face.edge == null)
				{
					face.edge = halfEdge;
				}
				return halfEdge;
			}
			halfEdge = new HalfEdge(origin, face);
			this.edges.Put(halfEdge);
			return halfEdge;
		}

		// Token: 0x06000D82 RID: 3458 RVA: 0x000A8974 File Offset: 0x000A6B74
		public Face CreateFace(TriangleNet.Geometry.Vertex vertex)
		{
			Face face;
			if (this.faces.TryGet(out face))
			{
				face.id = vertex.id;
				face.generator = vertex;
				face.edge = null;
				return face;
			}
			face = new Face(vertex);
			this.faces.Put(face);
			return face;
		}

		// Token: 0x04001EB4 RID: 7860
		private VoronoiFactory.ObjectPool<TriangleNet.Topology.DCEL.Vertex> vertices;

		// Token: 0x04001EB5 RID: 7861
		private VoronoiFactory.ObjectPool<HalfEdge> edges;

		// Token: 0x04001EB6 RID: 7862
		private VoronoiFactory.ObjectPool<Face> faces;

		// Token: 0x02000498 RID: 1176
		private class ObjectPool<T> where T : class
		{
			// Token: 0x17000398 RID: 920
			// (get) Token: 0x06001ABA RID: 6842 RVA: 0x000F7D35 File Offset: 0x000F5F35
			public int Count
			{
				get
				{
					return this.count;
				}
			}

			// Token: 0x17000399 RID: 921
			// (get) Token: 0x06001ABB RID: 6843 RVA: 0x000F7D3D File Offset: 0x000F5F3D
			// (set) Token: 0x06001ABC RID: 6844 RVA: 0x000F7D47 File Offset: 0x000F5F47
			public int Capacity
			{
				get
				{
					return this.pool.Length;
				}
				set
				{
					this.Resize(value);
				}
			}

			// Token: 0x06001ABD RID: 6845 RVA: 0x000F7D50 File Offset: 0x000F5F50
			public ObjectPool(int capacity = 3)
			{
				this.index = 0;
				this.count = 0;
				this.pool = new T[capacity];
			}

			// Token: 0x06001ABE RID: 6846 RVA: 0x000F7D72 File Offset: 0x000F5F72
			public ObjectPool(T[] pool)
			{
				this.index = 0;
				this.count = 0;
				this.pool = pool;
			}

			// Token: 0x06001ABF RID: 6847 RVA: 0x000F7D90 File Offset: 0x000F5F90
			public bool TryGet(out T obj)
			{
				if (this.index < this.count)
				{
					T[] array = this.pool;
					int num = this.index;
					this.index = num + 1;
					obj = array[num];
					return true;
				}
				obj = default(T);
				return false;
			}

			// Token: 0x06001AC0 RID: 6848 RVA: 0x000F7DD8 File Offset: 0x000F5FD8
			public void Put(T obj)
			{
				int num = this.pool.Length;
				if (num <= this.count)
				{
					this.Resize(2 * num);
				}
				T[] array = this.pool;
				int num2 = this.count;
				this.count = num2 + 1;
				array[num2] = obj;
				this.index++;
			}

			// Token: 0x06001AC1 RID: 6849 RVA: 0x000F7E2B File Offset: 0x000F602B
			public void Release()
			{
				this.index = 0;
			}

			// Token: 0x06001AC2 RID: 6850 RVA: 0x000F7E34 File Offset: 0x000F6034
			private void Resize(int size)
			{
				if (size > this.count)
				{
					Array.Resize<T>(ref this.pool, size);
				}
			}

			// Token: 0x04002B96 RID: 11158
			private int index;

			// Token: 0x04002B97 RID: 11159
			private int count;

			// Token: 0x04002B98 RID: 11160
			private T[] pool;
		}
	}
}
