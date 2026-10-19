using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TruckExporter
{
	internal class MeshData
	{
		public Vector3[] positions;
		public Vector3[] normals;
		public Vector2[] uvs;
		public List<int[]> submeshes = new List<int[]>();
	}

	// Reads a mesh's vertices and triangles. Games usually ship meshes without
	// CPU-side data ("Read/Write" off), so those are copied back from the GPU's
	// vertex and index buffers and decoded with the mesh's vertex layout. Only if
	// that fails too does a part go in as its bounding box.
	internal static class MeshReader
	{
		public static MeshData Read(Mesh mesh, out string how)
		{
			if (mesh.isReadable)
			{
				MeshData d = new MeshData();
				d.positions = mesh.vertices;
				d.normals = mesh.normals;
				d.uvs = mesh.uv;
				for (int s = 0; s < mesh.subMeshCount; s++)
				{
					if (mesh.GetTopology(s) == MeshTopology.Triangles)
					{
						d.submeshes.Add(mesh.GetTriangles(s));
					}
				}
				how = "readable";
				return d;
			}
			string why;
			try
			{
				MeshData d = FromGpu(mesh);
				if (Sane(d, mesh))
				{
					how = "gpu";
					return d;
				}
				why = "read back empty";
			}
			catch (Exception e)
			{
				why = e.Message;
			}
			// Some graphics APIs only read back buffers that can be bound raw.
			try
			{
				mesh.vertexBufferTarget |= GraphicsBuffer.Target.Raw;
				mesh.indexBufferTarget |= GraphicsBuffer.Target.Raw;
				MeshData d = FromGpu(mesh);
				if (Sane(d, mesh))
				{
					how = "gpu (raw)";
					return d;
				}
				why += "; raw: read back empty";
			}
			catch (Exception e)
			{
				why += "; raw: " + e.Message;
			}
			how = "box (not readable, GPU read failed: " + why + ")";
			return Box(mesh);
		}

		// Read-back data that's all zeros (or nowhere near the mesh's bounds) is a
		// failed read, not a real mesh.
		private static bool Sane(MeshData d, Mesh mesh)
		{
			if (d.positions == null || d.positions.Length == 0 || d.submeshes.Count == 0)
			{
				return false;
			}
			Bounds b = mesh.bounds;
			b.Expand(b.size.magnitude * 0.05f + 0.001f);
			int inside = 0, nonZero = 0;
			foreach (Vector3 p in d.positions)
			{
				if (b.Contains(p))
				{
					inside++;
				}
				if (p != Vector3.zero)
				{
					nonZero++;
				}
			}
			return nonZero > 0 && inside >= d.positions.Length * 0.95f;
		}

		private static MeshData FromGpu(Mesh mesh)
		{
			int count = mesh.vertexCount;
			byte[][] streams = new byte[mesh.vertexBufferCount][];
			for (int s = 0; s < streams.Length; s++)
			{
				using (GraphicsBuffer vb = mesh.GetVertexBuffer(s))
				{
					byte[] bytes = new byte[vb.count * vb.stride];
					vb.GetData(bytes);
					streams[s] = bytes;
				}
			}
			MeshData d = new MeshData();
			d.positions = ReadVec3(mesh, streams, VertexAttribute.Position, count);
			d.normals = mesh.HasVertexAttribute(VertexAttribute.Normal) ? ReadVec3(mesh, streams, VertexAttribute.Normal, count) : null;
			if (mesh.HasVertexAttribute(VertexAttribute.TexCoord0))
			{
				Vector3[] uv3 = ReadVec3(mesh, streams, VertexAttribute.TexCoord0, count);
				d.uvs = new Vector2[count];
				for (int i = 0; i < count; i++)
				{
					d.uvs[i] = new Vector2(uv3[i].x, uv3[i].y);
				}
			}
			byte[] indexBytes;
			int indexStride;
			using (GraphicsBuffer ib = mesh.GetIndexBuffer())
			{
				indexBytes = new byte[ib.count * ib.stride];
				ib.GetData(indexBytes);
				indexStride = ib.stride;
			}
			for (int s = 0; s < mesh.subMeshCount; s++)
			{
				SubMeshDescriptor sm = mesh.GetSubMesh(s);
				if (sm.topology != MeshTopology.Triangles)
				{
					continue;
				}
				int[] tris = new int[sm.indexCount];
				for (int i = 0; i < sm.indexCount; i++)
				{
					int at = (sm.indexStart + i) * indexStride;
					int index = indexStride == 2 ? BitConverter.ToUInt16(indexBytes, at) : BitConverter.ToInt32(indexBytes, at);
					tris[i] = index + sm.baseVertex;
				}
				d.submeshes.Add(tris);
			}
			return d;
		}

		private static Vector3[] ReadVec3(Mesh mesh, byte[][] streams, VertexAttribute attr, int count)
		{
			int stream = mesh.GetVertexAttributeStream(attr);
			int offset = mesh.GetVertexAttributeOffset(attr);
			int dim = mesh.GetVertexAttributeDimension(attr);
			VertexAttributeFormat format = mesh.GetVertexAttributeFormat(attr);
			int stride = mesh.GetVertexBufferStride(stream);
			int size = Size(format);
			byte[] b = streams[stream];
			Vector3[] result = new Vector3[count];
			for (int i = 0; i < count; i++)
			{
				int at = i * stride + offset;
				float x = dim > 0 ? Value(b, at, format) : 0f;
				float y = dim > 1 ? Value(b, at + size, format) : 0f;
				float z = dim > 2 ? Value(b, at + 2 * size, format) : 0f;
				result[i] = new Vector3(x, y, z);
			}
			return result;
		}

		private static int Size(VertexAttributeFormat f)
		{
			switch (f)
			{
			case VertexAttributeFormat.Float32:
			case VertexAttributeFormat.UInt32:
			case VertexAttributeFormat.SInt32:
				return 4;
			case VertexAttributeFormat.Float16:
			case VertexAttributeFormat.UNorm16:
			case VertexAttributeFormat.SNorm16:
			case VertexAttributeFormat.UInt16:
			case VertexAttributeFormat.SInt16:
				return 2;
			default:
				return 1;
			}
		}

		private static float Value(byte[] b, int at, VertexAttributeFormat f)
		{
			switch (f)
			{
			case VertexAttributeFormat.Float32:
				return BitConverter.ToSingle(b, at);
			case VertexAttributeFormat.Float16:
				return Mathf.HalfToFloat(BitConverter.ToUInt16(b, at));
			case VertexAttributeFormat.UNorm16:
				return BitConverter.ToUInt16(b, at) / 65535f;
			case VertexAttributeFormat.SNorm16:
				return Mathf.Max(BitConverter.ToInt16(b, at) / 32767f, -1f);
			case VertexAttributeFormat.UNorm8:
				return b[at] / 255f;
			case VertexAttributeFormat.SNorm8:
				return Mathf.Max((sbyte)b[at] / 127f, -1f);
			case VertexAttributeFormat.UInt16:
				return BitConverter.ToUInt16(b, at);
			case VertexAttributeFormat.SInt16:
				return BitConverter.ToInt16(b, at);
			case VertexAttributeFormat.UInt32:
				return BitConverter.ToUInt32(b, at);
			case VertexAttributeFormat.SInt32:
				return BitConverter.ToInt32(b, at);
			case VertexAttributeFormat.UInt8:
				return b[at];
			case VertexAttributeFormat.SInt8:
				return (sbyte)b[at];
			}
			return 0f;
		}

		public static MeshData Box(Mesh mesh)
		{
			Bounds b = mesh.bounds;
			MeshData d = new MeshData();
			d.positions = new Vector3[8];
			for (int i = 0; i < 8; i++)
			{
				d.positions[i] = b.center + Vector3.Scale(b.extents, new Vector3((i & 1) == 0 ? -1f : 1f, (i & 2) == 0 ? -1f : 1f, (i & 4) == 0 ? -1f : 1f));
			}
			d.submeshes.Add(new[] { 0, 2, 1, 1, 2, 3, 4, 5, 6, 5, 7, 6, 0, 1, 4, 1, 5, 4, 2, 6, 3, 3, 6, 7, 0, 4, 2, 2, 4, 6, 1, 3, 5, 3, 7, 5 });
			return d;
		}
	}
}
