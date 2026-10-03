using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace JunkyardATV
{
	// Turns parsed ModelData into GameObjects with meshes, materials and textures.
	internal static class ModelBuilder
	{
		private static Material baseMaterial;

		public static GameObject Build(ModelData data, string name)
		{
			GameObject root = new GameObject(name);
			List<Material> materials = new List<Material>();
			foreach (ModelMaterial m in data.materials)
			{
				materials.Add(MakeMaterial(m));
			}
			foreach (ModelNode node in data.roots)
			{
				BuildNode(node, root.transform, materials);
			}
			return root;
		}

		private static void BuildNode(ModelNode node, Transform parent, List<Material> materials)
		{
			GameObject go = new GameObject(node.name);
			go.transform.SetParent(parent, false);
			go.transform.localPosition = new Vector3(node.translation.x, node.translation.y, node.translation.z);
			go.transform.localRotation = new Quaternion(node.rotation.x, node.rotation.y, node.rotation.z, node.rotation.w);
			go.transform.localScale = new Vector3(node.scale.x, node.scale.y, node.scale.z);
			// glTF lets a node hold several primitives; each gets its own child so
			// every renderer has one mesh.
			for (int i = 0; i < node.meshes.Count; i++)
			{
				GameObject target = node.meshes.Count == 1 && node.children.Count == 0 ? go : new GameObject(node.name + "_mesh" + i);
				if (target != go)
				{
					target.transform.SetParent(go.transform, false);
				}
				AddMesh(target, node.meshes[i], materials);
			}
			foreach (ModelNode child in node.children)
			{
				BuildNode(child, go.transform, materials);
			}
		}

		private static void AddMesh(GameObject go, ModelMesh data, List<Material> materials)
		{
			Mesh mesh = new Mesh();
			mesh.name = go.name;
			if (data.positions.Count > 65000)
			{
				mesh.indexFormat = IndexFormat.UInt32;
			}
			Vector3[] vertices = new Vector3[data.positions.Count];
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = new Vector3(data.positions[i].x, data.positions[i].y, data.positions[i].z);
			}
			mesh.vertices = vertices;
			if (data.uvs.Count == vertices.Length)
			{
				Vector2[] uv = new Vector2[vertices.Length];
				for (int i = 0; i < uv.Length; i++)
				{
					uv[i] = new Vector2(data.uvs[i].x, data.uvs[i].y);
				}
				mesh.uv = uv;
			}
			mesh.subMeshCount = data.submeshes.Count;
			for (int s = 0; s < data.submeshes.Count; s++)
			{
				mesh.SetTriangles(data.submeshes[s].ToArray(), s);
			}
			if (data.normals.Count == vertices.Length)
			{
				Vector3[] normals = new Vector3[vertices.Length];
				for (int i = 0; i < normals.Length; i++)
				{
					normals[i] = new Vector3(data.normals[i].x, data.normals[i].y, data.normals[i].z);
				}
				mesh.normals = normals;
			}
			else
			{
				mesh.RecalculateNormals();
			}
			mesh.RecalculateBounds();
			go.AddComponent<MeshFilter>().sharedMesh = mesh;
			Material[] slots = new Material[data.submeshes.Count];
			for (int s = 0; s < slots.Length; s++)
			{
				int index = data.materials[s];
				slots[s] = index >= 0 && index < materials.Count ? materials[index] : MakeMaterial(new ModelMaterial());
			}
			go.AddComponent<MeshRenderer>().sharedMaterials = slots;
		}

		// The default material of a Unity primitive is always in the build (its
		// shader is the Standard one), unlike a Shader.Find lookup.
		public static Material NewMaterial()
		{
			if (baseMaterial == null)
			{
				GameObject probe = GameObject.CreatePrimitive(PrimitiveType.Cube);
				baseMaterial = new Material(probe.GetComponent<Renderer>().sharedMaterial);
				Object.DestroyImmediate(probe);
			}
			return new Material(baseMaterial);
		}

		private static Material MakeMaterial(ModelMaterial m)
		{
			Material material = NewMaterial();
			material.name = m.name;
			material.color = new Color(m.color[0], m.color[1], m.color[2], 1f);
			if (material.HasProperty("_Metallic"))
			{
				material.SetFloat("_Metallic", m.metallic);
			}
			if (material.HasProperty("_Glossiness"))
			{
				material.SetFloat("_Glossiness", m.smoothness);
			}
			if (m.image != null)
			{
				Texture2D texture = new Texture2D(2, 2);
				if (ImageConversion.LoadImage(texture, m.image))
				{
					texture.wrapMode = TextureWrapMode.Repeat;
					material.mainTexture = texture;
				}
			}
			return material;
		}
	}
}
