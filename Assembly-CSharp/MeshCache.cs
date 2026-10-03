using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020000CB RID: 203
internal class MeshCache
{
	// Token: 0x060004AC RID: 1196 RVA: 0x000300A4 File Offset: 0x0002E2A4
	public static MeshCache GetMeshCache(Mesh sharedMesh)
	{
		MeshCache meshCache = (from c in MeshCache.Cache
		where c.Mesh == sharedMesh
		select c).FirstOrDefault<MeshCache>();
		if (meshCache == null)
		{
			meshCache = new MeshCache(sharedMesh);
			MeshCache.Cache.Add(meshCache);
		}
		return meshCache;
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x000300F8 File Offset: 0x0002E2F8
	private MeshCache(Mesh mesh)
	{
		this.Mesh = mesh;
		this.Vertices = mesh.vertices;
		this.Triangles = mesh.triangles;
		this.Normals = mesh.normals;
		this.VertexVerticesIndex.Capacity = this.Vertices.Length;
		for (int i = 0; i < this.Vertices.Length; i++)
		{
			for (int j = 0; j < this.Triangles.Length / 3; j++)
			{
				int num = j * 3;
				if (this.Triangles[num++] == i || this.Triangles[num++] == i || this.Triangles[num++] == i)
				{
					num = j * 3;
					int num2 = this.Triangles[num++];
					if (num2 != i)
					{
						this.VertexVerticesIndex.Add(i, num2);
					}
					num2 = this.Triangles[num++];
					if (num2 != i)
					{
						this.VertexVerticesIndex.Add(i, num2);
					}
					num2 = this.Triangles[num++];
					if (num2 != i)
					{
						this.VertexVerticesIndex.Add(i, num2);
					}
				}
			}
		}
		this.VertexVerticesIndex.ConsolidateIndex();
		this.Bounds = mesh.bounds;
		this.MeshSize = mesh.bounds.size;
		this.MeshSize.x = Mathf.Max(this.MeshSize.x, 0.1f);
		this.MeshSize.y = Mathf.Max(this.MeshSize.y, 0.1f);
		this.MeshSize.z = Mathf.Max(this.MeshSize.z, 0.1f);
		float num3 = Mathf.Max(new float[]
		{
			this.MeshSize.x,
			this.MeshSize.y,
			this.MeshSize.z
		});
		this.SizeFactor = new Vector3(1f / (this.MeshSize.x / num3), 1f / (this.MeshSize.y / num3), 1f / (this.MeshSize.z / num3));
	}

	// Token: 0x060004AE RID: 1198 RVA: 0x00030328 File Offset: 0x0002E528
	public List<int> FindConnectedVertices(List<int> vertices)
	{
		List<int> list = new List<int>();
		foreach (int vertex in vertices)
		{
			this.VertexVerticesIndex.ListConnectedVertices(vertex, list);
		}
		vertices = list.ToList<int>();
		list.Clear();
		foreach (int vertex2 in vertices)
		{
			this.VertexVerticesIndex.ListConnectedVertices(vertex2, list);
		}
		return list;
	}

	// Token: 0x04000964 RID: 2404
	private static List<MeshCache> Cache = new List<MeshCache>();

	// Token: 0x04000965 RID: 2405
	public Mesh Mesh;

	// Token: 0x04000966 RID: 2406
	public Vector3[] Vertices;

	// Token: 0x04000967 RID: 2407
	public int[] Triangles;

	// Token: 0x04000968 RID: 2408
	public Vector3[] Normals;

	// Token: 0x04000969 RID: 2409
	public Vector3 MeshSize;

	// Token: 0x0400096A RID: 2410
	public Vector3 SizeFactor;

	// Token: 0x0400096B RID: 2411
	public Bounds Bounds;

	// Token: 0x0400096C RID: 2412
	public VertexVerticesIndex VertexVerticesIndex = new VertexVerticesIndex();
}
