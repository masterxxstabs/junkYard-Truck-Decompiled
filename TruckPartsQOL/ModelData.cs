using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace TruckPartsQOL
{
	// Engine-free model data, so the parsers can be tested outside the game.
	// Everything here is already in Unity's left-handed space (+Y up): glTF and
	// OBJ are right-handed, so X is mirrored and triangle winding reversed.
	public struct V2
	{
		public float x, y;

		public V2(float x, float y)
		{
			this.x = x;
			this.y = y;
		}
	}

	public struct V3
	{
		public float x, y, z;

		public V3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
	}

	public struct Q
	{
		public float x, y, z, w;

		public Q(float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public static readonly Q Identity = new Q(0f, 0f, 0f, 1f);
	}

	public class ModelMaterial
	{
		public string name = "";
		public float[] color = { 1f, 1f, 1f, 1f };
		public float metallic;
		public float smoothness = 0.3f;
		public byte[] image; // PNG/JPG bytes of the base color texture
	}

	public class ModelMesh
	{
		public List<V3> positions = new List<V3>();
		public List<V3> normals = new List<V3>();
		public List<V2> uvs = new List<V2>();
		public List<List<int>> submeshes = new List<List<int>>();
		public List<int> materials = new List<int>(); // per submesh, -1 = none
	}

	public class ModelNode
	{
		public string name = "";
		public V3 translation;
		public Q rotation = Q.Identity;
		public V3 scale = new V3(1f, 1f, 1f);
		public List<ModelMesh> meshes = new List<ModelMesh>();
		public List<ModelNode> children = new List<ModelNode>();
	}

	public class ModelData
	{
		public List<ModelNode> roots = new List<ModelNode>();
		public List<ModelMaterial> materials = new List<ModelMaterial>();

		public int CountMeshes()
		{
			int n = 0;
			Stack<ModelNode> stack = new Stack<ModelNode>(roots);
			while (stack.Count > 0)
			{
				ModelNode node = stack.Pop();
				n += node.meshes.Count;
				foreach (ModelNode child in node.children)
				{
					stack.Push(child);
				}
			}
			return n;
		}

		public static ModelData Load(string path)
		{
			string ext = Path.GetExtension(path).ToLowerInvariant();
			string dir = Path.GetDirectoryName(path);
			if (ext == ".obj")
			{
				return ObjParser.Parse(File.ReadAllText(path), dir);
			}
			if (ext == ".glb" || ext == ".gltf")
			{
				return GltfParser.Parse(File.ReadAllBytes(path), dir);
			}
			throw new NotSupportedException("Model format " + ext + " isn't supported; use .glb, .gltf or .obj.");
		}
	}

	// Wavefront OBJ + MTL. Each o/g becomes a node; each usemtl a submesh.
	public static class ObjParser
	{
		private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

		public static ModelData Parse(string text, string dir)
		{
			ModelData model = new ModelData();
			Dictionary<string, int> materialIndex = new Dictionary<string, int>();
			List<V3> v = new List<V3>();
			List<V2> vt = new List<V2>();
			List<V3> vn = new List<V3>();

			ModelNode node = null;
			ModelMesh mesh = null;
			Dictionary<string, int> vertexIndex = null;
			List<int> submesh = null;
			int currentMaterial = -1;
			string pendingName = "default";

			foreach (string raw in text.Split('\n'))
			{
				string line = raw.Trim();
				if (line.Length == 0 || line[0] == '#')
				{
					continue;
				}
				string[] p = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
				switch (p[0])
				{
				case "v":
					v.Add(new V3(-F(p[1]), F(p[2]), F(p[3])));
					break;
				case "vt":
					vt.Add(new V2(F(p[1]), p.Length > 2 ? F(p[2]) : 0f));
					break;
				case "vn":
					vn.Add(new V3(-F(p[1]), F(p[2]), F(p[3])));
					break;
				case "o":
				case "g":
					pendingName = p.Length > 1 ? string.Join(" ", p, 1, p.Length - 1) : "group";
					node = null;
					break;
				case "mtllib":
					string lib = Path.Combine(dir ?? "", string.Join(" ", p, 1, p.Length - 1));
					if (File.Exists(lib))
					{
						ParseMtl(File.ReadAllText(lib), Path.GetDirectoryName(lib), model, materialIndex);
					}
					break;
				case "usemtl":
					string matName = p.Length > 1 ? string.Join(" ", p, 1, p.Length - 1) : "";
					int idx;
					currentMaterial = materialIndex.TryGetValue(matName, out idx) ? idx : -1;
					submesh = null;
					break;
				case "f":
					if (node == null)
					{
						node = new ModelNode();
						node.name = pendingName;
						mesh = new ModelMesh();
						node.meshes.Add(mesh);
						model.roots.Add(node);
						vertexIndex = new Dictionary<string, int>();
						submesh = null;
					}
					if (submesh == null)
					{
						// Reuse the submesh for this material if this group had it before.
						int at = mesh.materials.IndexOf(currentMaterial);
						if (at >= 0)
						{
							submesh = mesh.submeshes[at];
						}
						else
						{
							submesh = new List<int>();
							mesh.submeshes.Add(submesh);
							mesh.materials.Add(currentMaterial);
						}
					}
					int[] face = new int[p.Length - 1];
					for (int k = 1; k < p.Length; k++)
					{
						face[k - 1] = Vertex(p[k], v, vt, vn, mesh, vertexIndex);
					}
					// Fan-triangulate, winding reversed for the mirrored X.
					for (int k = 1; k + 1 < face.Length; k++)
					{
						submesh.Add(face[0]);
						submesh.Add(face[k + 1]);
						submesh.Add(face[k]);
					}
					break;
				}
			}
			// Groups whose faces had no normals: let Unity compute them.
			foreach (ModelNode n in model.roots)
			{
				foreach (ModelMesh m in n.meshes)
				{
					if (m.normals.Count != m.positions.Count)
					{
						m.normals.Clear();
					}
					if (m.uvs.Count != m.positions.Count)
					{
						m.uvs.Clear();
					}
				}
			}
			return model;
		}

		private static int Vertex(string token, List<V3> v, List<V2> vt, List<V3> vn, ModelMesh mesh, Dictionary<string, int> index)
		{
			int existing;
			if (index.TryGetValue(token, out existing))
			{
				return existing;
			}
			string[] parts = token.Split('/');
			int vi = Resolve(parts[0], v.Count);
			mesh.positions.Add(v[vi]);
			if (parts.Length > 1 && parts[1].Length > 0)
			{
				mesh.uvs.Add(vt[Resolve(parts[1], vt.Count)]);
			}
			if (parts.Length > 2 && parts[2].Length > 0)
			{
				mesh.normals.Add(vn[Resolve(parts[2], vn.Count)]);
			}
			int result = mesh.positions.Count - 1;
			index[token] = result;
			return result;
		}

		// OBJ indexes are 1-based; negative ones count back from the end.
		private static int Resolve(string s, int count)
		{
			int i = int.Parse(s, Inv);
			return i > 0 ? i - 1 : count + i;
		}

		private static void ParseMtl(string text, string dir, ModelData model, Dictionary<string, int> index)
		{
			ModelMaterial current = null;
			foreach (string raw in text.Split('\n'))
			{
				string line = raw.Trim();
				if (line.Length == 0 || line[0] == '#')
				{
					continue;
				}
				string[] p = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
				switch (p[0])
				{
				case "newmtl":
					current = new ModelMaterial();
					current.name = p.Length > 1 ? string.Join(" ", p, 1, p.Length - 1) : "";
					index[current.name] = model.materials.Count;
					model.materials.Add(current);
					break;
				case "Kd":
					if (current != null)
					{
						current.color = new[] { F(p[1]), F(p[2]), F(p[3]), current.color[3] };
					}
					break;
				case "d":
					if (current != null)
					{
						current.color[3] = F(p[1]);
					}
					break;
				case "Ns":
					if (current != null)
					{
						current.smoothness = Math.Min(1f, Math.Max(0f, F(p[1]) / 1000f));
					}
					break;
				// PBR extension (Blender's exporter writes these when asked).
				case "Pm":
					if (current != null)
					{
						current.metallic = Math.Min(1f, Math.Max(0f, F(p[1])));
					}
					break;
				case "Pr":
					if (current != null)
					{
						current.smoothness = 1f - Math.Min(1f, Math.Max(0f, F(p[1])));
					}
					break;
				case "map_Kd":
					if (current != null)
					{
						string file = Path.Combine(dir ?? "", p[p.Length - 1]);
						if (File.Exists(file))
						{
							current.image = File.ReadAllBytes(file);
						}
					}
					break;
				}
			}
		}

		private static float F(string s)
		{
			return float.Parse(s, NumberStyles.Float, Inv);
		}
	}

	// glTF 2.0, binary (.glb) or text (.gltf with .bin / data: URIs).
	public static class GltfParser
	{
		private const uint Magic = 0x46546C67; // "glTF"

		private class Ctx
		{
			public Dictionary<string, object> json;
			public List<byte[]> buffers = new List<byte[]>();
		}

		public static ModelData Parse(byte[] file, string dir)
		{
			Ctx ctx = new Ctx();
			if (file.Length >= 12 && BitConverter.ToUInt32(file, 0) == Magic)
			{
				int offset = 12;
				byte[] bin = null;
				string json = null;
				while (offset + 8 <= file.Length)
				{
					int length = BitConverter.ToInt32(file, offset);
					uint type = BitConverter.ToUInt32(file, offset + 4);
					if (type == 0x4E4F534A) // JSON
					{
						json = Encoding.UTF8.GetString(file, offset + 8, length);
					}
					else if (type == 0x004E4942) // BIN
					{
						bin = new byte[length];
						Buffer.BlockCopy(file, offset + 8, bin, 0, length);
					}
					offset += 8 + length;
				}
				ctx.json = (Dictionary<string, object>)MiniJson.Parse(json);
				foreach (object b in List(ctx.json, "buffers"))
				{
					Dictionary<string, object> buffer = (Dictionary<string, object>)b;
					ctx.buffers.Add(buffer.ContainsKey("uri") ? ReadUri((string)buffer["uri"], dir) : bin);
				}
			}
			else
			{
				ctx.json = (Dictionary<string, object>)MiniJson.Parse(Encoding.UTF8.GetString(file));
				foreach (object b in List(ctx.json, "buffers"))
				{
					ctx.buffers.Add(ReadUri((string)((Dictionary<string, object>)b)["uri"], dir));
				}
			}

			ModelData model = new ModelData();
			foreach (object m in List(ctx.json, "materials"))
			{
				model.materials.Add(ReadMaterial(ctx, (Dictionary<string, object>)m, dir));
			}
			List<object> meshes = List(ctx.json, "meshes");
			List<object> nodes = List(ctx.json, "nodes");
			List<object> roots;
			List<object> scenes = List(ctx.json, "scenes");
			if (scenes.Count > 0)
			{
				int scene = ctx.json.ContainsKey("scene") ? Int(ctx.json["scene"]) : 0;
				roots = List((Dictionary<string, object>)scenes[scene], "nodes");
			}
			else
			{
				// No scene list: every node that isn't someone's child is a root.
				HashSet<int> children = new HashSet<int>();
				foreach (object n in nodes)
				{
					foreach (object c in List((Dictionary<string, object>)n, "children"))
					{
						children.Add(Int(c));
					}
				}
				roots = new List<object>();
				for (int i = 0; i < nodes.Count; i++)
				{
					if (!children.Contains(i))
					{
						roots.Add((double)i);
					}
				}
			}
			foreach (object r in roots)
			{
				model.roots.Add(ReadNode(ctx, nodes, meshes, Int(r), 0));
			}
			return model;
		}

		private static ModelNode ReadNode(Ctx ctx, List<object> nodes, List<object> meshes, int index, int depth)
		{
			if (depth > 64)
			{
				throw new FormatException("glTF node hierarchy too deep (cycle?)");
			}
			Dictionary<string, object> n = (Dictionary<string, object>)nodes[index];
			ModelNode node = new ModelNode();
			node.name = n.ContainsKey("name") ? (string)n["name"] : "node" + index;
			if (n.ContainsKey("matrix"))
			{
				Decompose(Floats(n["matrix"]), node);
			}
			else
			{
				if (n.ContainsKey("translation"))
				{
					float[] t = Floats(n["translation"]);
					node.translation = new V3(-t[0], t[1], t[2]);
				}
				if (n.ContainsKey("rotation"))
				{
					float[] q = Floats(n["rotation"]);
					node.rotation = new Q(q[0], -q[1], -q[2], q[3]);
				}
				if (n.ContainsKey("scale"))
				{
					float[] sc = Floats(n["scale"]);
					node.scale = new V3(sc[0], sc[1], sc[2]);
				}
			}
			if (n.ContainsKey("mesh"))
			{
				Dictionary<string, object> mesh = (Dictionary<string, object>)meshes[Int(n["mesh"])];
				foreach (object p in List(mesh, "primitives"))
				{
					ModelMesh m = ReadPrimitive(ctx, (Dictionary<string, object>)p);
					if (m != null)
					{
						node.meshes.Add(m);
					}
				}
			}
			foreach (object c in List(n, "children"))
			{
				node.children.Add(ReadNode(ctx, nodes, meshes, Int(c), depth + 1));
			}
			return node;
		}

		// Column-major 4x4 -> T, R, S (mirrored into Unity space).
		private static void Decompose(float[] m, ModelNode node)
		{
			node.translation = new V3(-m[12], m[13], m[14]);
			float sx = Len(m[0], m[1], m[2]);
			float sy = Len(m[4], m[5], m[6]);
			float sz = Len(m[8], m[9], m[10]);
			node.scale = new V3(sx, sy, sz);
			float r00 = m[0] / sx, r10 = m[1] / sx, r20 = m[2] / sx;
			float r01 = m[4] / sy, r11 = m[5] / sy, r21 = m[6] / sy;
			float r02 = m[8] / sz, r12 = m[9] / sz, r22 = m[10] / sz;
			Q q = FromRotationMatrix(r00, r01, r02, r10, r11, r12, r20, r21, r22);
			node.rotation = new Q(q.x, -q.y, -q.z, q.w);
		}

		private static float Len(float a, float b, float c)
		{
			return (float)Math.Sqrt(a * a + b * b + c * c);
		}

		private static Q FromRotationMatrix(float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22)
		{
			float trace = m00 + m11 + m22;
			if (trace > 0f)
			{
				float s = (float)Math.Sqrt(trace + 1f) * 2f;
				return new Q((m21 - m12) / s, (m02 - m20) / s, (m10 - m01) / s, 0.25f * s);
			}
			if (m00 > m11 && m00 > m22)
			{
				float s = (float)Math.Sqrt(1f + m00 - m11 - m22) * 2f;
				return new Q(0.25f * s, (m01 + m10) / s, (m02 + m20) / s, (m21 - m12) / s);
			}
			if (m11 > m22)
			{
				float s = (float)Math.Sqrt(1f + m11 - m00 - m22) * 2f;
				return new Q((m01 + m10) / s, 0.25f * s, (m12 + m21) / s, (m02 - m20) / s);
			}
			float s2 = (float)Math.Sqrt(1f + m22 - m00 - m11) * 2f;
			return new Q((m02 + m20) / s2, (m12 + m21) / s2, 0.25f * s2, (m10 - m01) / s2);
		}

		private static ModelMesh ReadPrimitive(Ctx ctx, Dictionary<string, object> p)
		{
			int mode = p.ContainsKey("mode") ? Int(p["mode"]) : 4;
			if (mode != 4)
			{
				return null; // only plain triangles
			}
			Dictionary<string, object> attributes = (Dictionary<string, object>)p["attributes"];
			if (!attributes.ContainsKey("POSITION"))
			{
				return null;
			}
			ModelMesh mesh = new ModelMesh();
			float[] pos = ReadFloats(ctx, Int(attributes["POSITION"]), 3);
			for (int i = 0; i < pos.Length; i += 3)
			{
				mesh.positions.Add(new V3(-pos[i], pos[i + 1], pos[i + 2]));
			}
			if (attributes.ContainsKey("NORMAL"))
			{
				float[] nrm = ReadFloats(ctx, Int(attributes["NORMAL"]), 3);
				for (int i = 0; i < nrm.Length; i += 3)
				{
					mesh.normals.Add(new V3(-nrm[i], nrm[i + 1], nrm[i + 2]));
				}
			}
			if (attributes.ContainsKey("TEXCOORD_0"))
			{
				float[] uv = ReadFloats(ctx, Int(attributes["TEXCOORD_0"]), 2);
				for (int i = 0; i < uv.Length; i += 2)
				{
					mesh.uvs.Add(new V2(uv[i], 1f - uv[i + 1])); // glTF UV origin is top-left
				}
			}
			List<int> tris = new List<int>();
			if (p.ContainsKey("indices"))
			{
				int[] idx = ReadInts(ctx, Int(p["indices"]));
				for (int i = 0; i + 2 < idx.Length; i += 3)
				{
					tris.Add(idx[i]);
					tris.Add(idx[i + 2]);
					tris.Add(idx[i + 1]);
				}
			}
			else
			{
				for (int i = 0; i + 2 < mesh.positions.Count; i += 3)
				{
					tris.Add(i);
					tris.Add(i + 2);
					tris.Add(i + 1);
				}
			}
			mesh.submeshes.Add(tris);
			mesh.materials.Add(p.ContainsKey("material") ? Int(p["material"]) : -1);
			return mesh;
		}

		private static ModelMaterial ReadMaterial(Ctx ctx, Dictionary<string, object> m, string dir)
		{
			ModelMaterial mat = new ModelMaterial();
			mat.name = m.ContainsKey("name") ? (string)m["name"] : "";
			if (m.ContainsKey("pbrMetallicRoughness"))
			{
				Dictionary<string, object> pbr = (Dictionary<string, object>)m["pbrMetallicRoughness"];
				if (pbr.ContainsKey("baseColorFactor"))
				{
					mat.color = Floats(pbr["baseColorFactor"]);
				}
				mat.metallic = pbr.ContainsKey("metallicFactor") ? (float)(double)pbr["metallicFactor"] : 1f;
				mat.smoothness = 1f - (pbr.ContainsKey("roughnessFactor") ? (float)(double)pbr["roughnessFactor"] : 1f);
				if (pbr.ContainsKey("baseColorTexture"))
				{
					int texture = Int(((Dictionary<string, object>)pbr["baseColorTexture"])["index"]);
					mat.image = ReadImage(ctx, texture, dir);
				}
			}
			return mat;
		}

		private static byte[] ReadImage(Ctx ctx, int textureIndex, string dir)
		{
			List<object> textures = List(ctx.json, "textures");
			if (textureIndex >= textures.Count)
			{
				return null;
			}
			Dictionary<string, object> texture = (Dictionary<string, object>)textures[textureIndex];
			if (!texture.ContainsKey("source"))
			{
				return null;
			}
			Dictionary<string, object> image = (Dictionary<string, object>)List(ctx.json, "images")[Int(texture["source"])];
			if (image.ContainsKey("bufferView"))
			{
				return View(ctx, Int(image["bufferView"]));
			}
			return image.ContainsKey("uri") ? ReadUri((string)image["uri"], dir) : null;
		}

		private static byte[] View(Ctx ctx, int index)
		{
			Dictionary<string, object> view = (Dictionary<string, object>)List(ctx.json, "bufferViews")[index];
			byte[] buffer = ctx.buffers[Int(view["buffer"])];
			int offset = view.ContainsKey("byteOffset") ? Int(view["byteOffset"]) : 0;
			int length = Int(view["byteLength"]);
			byte[] result = new byte[length];
			Buffer.BlockCopy(buffer, offset, result, 0, length);
			return result;
		}

		private static int Components(string type)
		{
			switch (type)
			{
			case "SCALAR":
				return 1;
			case "VEC2":
				return 2;
			case "VEC3":
				return 3;
			case "VEC4":
				return 4;
			default:
				return 16;
			}
		}

		private static int ComponentSize(int componentType)
		{
			switch (componentType)
			{
			case 5120:
			case 5121:
				return 1;
			case 5122:
			case 5123:
				return 2;
			default:
				return 4;
			}
		}

		// Read an accessor as floats (normalized integers become 0-1).
		private static float[] ReadFloats(Ctx ctx, int accessorIndex, int expectComponents)
		{
			Dictionary<string, object> a = (Dictionary<string, object>)List(ctx.json, "accessors")[accessorIndex];
			int count = Int(a["count"]);
			int components = Components((string)a["type"]);
			int componentType = Int(a["componentType"]);
			bool normalized = a.ContainsKey("normalized") && (bool)a["normalized"];
			float[] result = new float[count * expectComponents];
			if (!a.ContainsKey("bufferView"))
			{
				return result; // all zeros (sparse accessors aren't supported)
			}
			Dictionary<string, object> view = (Dictionary<string, object>)List(ctx.json, "bufferViews")[Int(a["bufferView"])];
			byte[] buffer = ctx.buffers[Int(view["buffer"])];
			int size = ComponentSize(componentType);
			int start = (view.ContainsKey("byteOffset") ? Int(view["byteOffset"]) : 0) + (a.ContainsKey("byteOffset") ? Int(a["byteOffset"]) : 0);
			int stride = view.ContainsKey("byteStride") ? Int(view["byteStride"]) : size * components;
			for (int i = 0; i < count; i++)
			{
				int at = start + i * stride;
				for (int c = 0; c < expectComponents && c < components; c++)
				{
					result[i * expectComponents + c] = ReadComponent(buffer, at + c * size, componentType, normalized);
				}
			}
			return result;
		}

		private static float ReadComponent(byte[] b, int at, int type, bool normalized)
		{
			switch (type)
			{
			case 5126:
				return BitConverter.ToSingle(b, at);
			case 5121:
				return normalized ? b[at] / 255f : b[at];
			case 5120:
				return normalized ? Math.Max((sbyte)b[at] / 127f, -1f) : (sbyte)b[at];
			case 5123:
				return normalized ? BitConverter.ToUInt16(b, at) / 65535f : BitConverter.ToUInt16(b, at);
			case 5122:
				return normalized ? Math.Max(BitConverter.ToInt16(b, at) / 32767f, -1f) : BitConverter.ToInt16(b, at);
			default:
				return BitConverter.ToUInt32(b, at);
			}
		}

		private static int[] ReadInts(Ctx ctx, int accessorIndex)
		{
			Dictionary<string, object> a = (Dictionary<string, object>)List(ctx.json, "accessors")[accessorIndex];
			int count = Int(a["count"]);
			int componentType = Int(a["componentType"]);
			Dictionary<string, object> view = (Dictionary<string, object>)List(ctx.json, "bufferViews")[Int(a["bufferView"])];
			byte[] buffer = ctx.buffers[Int(view["buffer"])];
			int size = ComponentSize(componentType);
			int start = (view.ContainsKey("byteOffset") ? Int(view["byteOffset"]) : 0) + (a.ContainsKey("byteOffset") ? Int(a["byteOffset"]) : 0);
			int stride = view.ContainsKey("byteStride") ? Int(view["byteStride"]) : size;
			int[] result = new int[count];
			for (int i = 0; i < count; i++)
			{
				int at = start + i * stride;
				result[i] = size == 1 ? buffer[at] : size == 2 ? BitConverter.ToUInt16(buffer, at) : (int)BitConverter.ToUInt32(buffer, at);
			}
			return result;
		}

		private static byte[] ReadUri(string uri, string dir)
		{
			if (uri.StartsWith("data:", StringComparison.Ordinal))
			{
				return Convert.FromBase64String(uri.Substring(uri.IndexOf(',') + 1));
			}
			return File.ReadAllBytes(Path.Combine(dir ?? "", Uri.UnescapeDataString(uri)));
		}

		private static List<object> List(Dictionary<string, object> obj, string key)
		{
			object value;
			return obj.TryGetValue(key, out value) && value is List<object> ? (List<object>)value : new List<object>();
		}

		private static float[] Floats(object array)
		{
			List<object> list = (List<object>)array;
			float[] result = new float[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				result[i] = (float)(double)list[i];
			}
			return result;
		}

		private static int Int(object value)
		{
			return (int)(double)value;
		}
	}
}
