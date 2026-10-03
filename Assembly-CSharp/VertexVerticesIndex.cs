using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x020000CA RID: 202
internal class VertexVerticesIndex
{
	// Token: 0x1700003E RID: 62
	// (get) Token: 0x060004A5 RID: 1189 RVA: 0x0002FFD2 File Offset: 0x0002E1D2
	// (set) Token: 0x060004A6 RID: 1190 RVA: 0x0002FFE0 File Offset: 0x0002E1E0
	public int Capacity
	{
		get
		{
			return this.Vertices.Count;
		}
		set
		{
			this.Vertices.Clear();
			for (int i = 0; i < value; i++)
			{
				this.Vertices.Add(new VertexVerticesIndex.Vertex());
			}
		}
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x00030014 File Offset: 0x0002E214
	public void Add(int vertex, int connected)
	{
		this.Vertices[vertex].Vertices.Add(connected);
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x0003002D File Offset: 0x0002E22D
	public void ConsolidateIndex()
	{
		this.Vertices.ForEach(delegate(VertexVerticesIndex.Vertex t)
		{
			t.Vertices = t.Vertices.Distinct<int>().ToList<int>();
		});
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x00030059 File Offset: 0x0002E259
	public void ListConnectedVertices(int vertex, List<int> vertices)
	{
		vertices.AddRange(this.Vertices[vertex].Vertices);
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x00030074 File Offset: 0x0002E274
	public List<int> ListConnectedVertices(int vertex)
	{
		List<int> list = new List<int>();
		this.ListConnectedVertices(vertex, list);
		return list;
	}

	// Token: 0x04000963 RID: 2403
	private List<VertexVerticesIndex.Vertex> Vertices = new List<VertexVerticesIndex.Vertex>();

	// Token: 0x020003AF RID: 943
	internal class Vertex
	{
		// Token: 0x040027A9 RID: 10153
		public List<int> Vertices = new List<int>();
	}
}
