using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FurnitureTweaks
{
	// Builds furniture out of boxes and cylinders: one mesh per material (so a
	// piece is a handful of draw calls), UVs projected per face in meters so the
	// textures tile at the same scale on every part, and box colliders for the
	// solid parts.
	internal class MeshKit
	{
		private class Part
		{
			public readonly List<Vector3> v = new List<Vector3>();
			public readonly List<Vector3> n = new List<Vector3>();
			public readonly List<Vector2> uv = new List<Vector2>();
			public readonly List<int> t = new List<int>();
		}

		private readonly Dictionary<Material, Part> parts = new Dictionary<Material, Part>();
		private readonly List<Material> order = new List<Material>();
		private readonly List<KeyValuePair<Vector3, Vector3>> colliders = new List<KeyValuePair<Vector3, Vector3>>();

		// Texture meters per UV unit.
		public float uvScale = 1f;

		private Part Of(Material m)
		{
			Part p;
			if (!parts.TryGetValue(m, out p))
			{
				p = new Part();
				parts[m] = p;
				order.Add(m);
			}
			return p;
		}

		// An axis-aligned box (in the piece's frame), optionally solid.
		public void Box(Material m, Vector3 center, Vector3 size, bool solid = false)
		{
			Box(m, center, size, Quaternion.identity);
			if (solid)
			{
				colliders.Add(new KeyValuePair<Vector3, Vector3>(center, size));
			}
		}

		// A box turned by `rot` about its center (braces, the safe's handle).
		public void Box(Material m, Vector3 center, Vector3 size, Quaternion rot)
		{
			Part p = Of(m);
			Vector3 h = size * 0.5f;
			Vector3[] axes = { Vector3.right, Vector3.up, Vector3.forward };
			for (int a = 0; a < 3; a++)
			{
				for (int s = -1; s <= 1; s += 2)
				{
					Vector3 normal = axes[a] * s;
					Vector3 u = axes[(a + 1) % 3];
					Vector3 w = axes[(a + 2) % 3];
					if (s < 0)
					{
						Vector3 tmp = u;
						u = w;
						w = tmp;
					}
					float hu = Vector3.Scale(h, Abs(u)).magnitude;
					float hw = Vector3.Scale(h, Abs(w)).magnitude;
					Vector3 c = Vector3.Scale(normal, h);
					int start = p.v.Count;
					for (int k = 0; k < 4; k++)
					{
						float su = k == 0 || k == 3 ? -1f : 1f;
						float sw = k < 2 ? -1f : 1f;
						Vector3 local = c + u * hu * su + w * hw * sw;
						p.v.Add(center + rot * local);
						p.n.Add(rot * normal);
						// Projected in meters, offset by the box's position so
						// neighboring parts don't repeat the same patch.
						Vector3 world = center + local;
						p.uv.Add(new Vector2(Vector3.Dot(world, u) / uvScale, Vector3.Dot(world, w) / uvScale));
					}
					p.t.Add(start);
					p.t.Add(start + 1);
					p.t.Add(start + 2);
					p.t.Add(start);
					p.t.Add(start + 2);
					p.t.Add(start + 3);
				}
			}
		}

		// A cylinder along `axis` (a unit vector: right, up or forward).
		public void Cylinder(Material m, Vector3 center, float radius, float length, Vector3 axis, int segments = 20)
		{
			Part p = Of(m);
			Quaternion rot = Quaternion.FromToRotation(Vector3.up, axis);
			float hl = length * 0.5f;
			// Side.
			int start = p.v.Count;
			for (int i = 0; i <= segments; i++)
			{
				float a = i / (float)segments * Mathf.PI * 2f;
				Vector3 dir = new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a));
				for (int e = 0; e < 2; e++)
				{
					p.v.Add(center + rot * (dir * radius + Vector3.up * (e == 0 ? -hl : hl)));
					p.n.Add(rot * dir);
					p.uv.Add(new Vector2(a * radius / uvScale, (e == 0 ? -hl : hl) / uvScale));
				}
			}
			for (int i = 0; i < segments; i++)
			{
				int a0 = start + i * 2;
				p.t.Add(a0);
				p.t.Add(a0 + 1);
				p.t.Add(a0 + 3);
				p.t.Add(a0);
				p.t.Add(a0 + 3);
				p.t.Add(a0 + 2);
			}
			// Caps.
			for (int e = 0; e < 2; e++)
			{
				float y = e == 0 ? -hl : hl;
				Vector3 normal = rot * (e == 0 ? Vector3.down : Vector3.up);
				int c = p.v.Count;
				p.v.Add(center + rot * (Vector3.up * y));
				p.n.Add(normal);
				p.uv.Add(Vector2.zero);
				for (int i = 0; i <= segments; i++)
				{
					float a = i / (float)segments * Mathf.PI * 2f;
					Vector3 dir = new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a));
					p.v.Add(center + rot * (dir * radius + Vector3.up * y));
					p.n.Add(normal);
					p.uv.Add(new Vector2(dir.x * radius / uvScale, dir.z * radius / uvScale));
				}
				for (int i = 0; i < segments; i++)
				{
					if (e == 0)
					{
						p.t.Add(c);
						p.t.Add(c + 1 + i);
						p.t.Add(c + 2 + i);
					}
					else
					{
						p.t.Add(c);
						p.t.Add(c + 2 + i);
						p.t.Add(c + 1 + i);
					}
				}
			}
		}

		// A collider without a visible part (the inside of the crate, say).
		public void Solid(Vector3 center, Vector3 size)
		{
			colliders.Add(new KeyValuePair<Vector3, Vector3>(center, size));
		}

		// The meshes and colliders, under `root`.
		public void Build(GameObject root)
		{
			foreach (Material m in order)
			{
				Part p = parts[m];
				Mesh mesh = new Mesh();
				mesh.name = root.name + "_" + m.name;
				mesh.SetVertices(p.v);
				mesh.SetNormals(p.n);
				mesh.SetUVs(0, p.uv);
				mesh.SetTriangles(p.t, 0);
				mesh.RecalculateBounds();
				mesh.RecalculateTangents();
				GameObject go = new GameObject(m.name);
				go.transform.SetParent(root.transform, false);
				go.AddComponent<MeshFilter>().sharedMesh = mesh;
				go.AddComponent<MeshRenderer>().sharedMaterial = m;
			}
			foreach (KeyValuePair<Vector3, Vector3> c in colliders)
			{
				BoxCollider box = root.AddComponent<BoxCollider>();
				box.center = c.Key;
				box.size = c.Value;
			}
		}

		private static Vector3 Abs(Vector3 v)
		{
			return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
		}
	}

	// The furniture's materials: the game's Standard shader with textures made
	// here (painted steel, worn wood, brass...), made once per level.
	internal static class Looks
	{
		private static readonly Dictionary<string, Material> made = new Dictionary<string, Material>();
		private static Shader standard;

		public static void Reset()
		{
			made.Clear();
		}

		private static Shader Standard()
		{
			if (standard != null)
			{
				return standard;
			}
			// The game's own Standard materials carry the shader with the variants
			// the game was built with.
			foreach (Material m in Resources.FindObjectsOfTypeAll<Material>())
			{
				if (m != null && m.shader != null && m.shader.name == "Standard")
				{
					standard = m.shader;
					return standard;
				}
			}
			standard = Shader.Find("Standard");
			return standard;
		}

		private static Material Make(string name, Texture2D tex, Color tint, float metallic, float smoothness)
		{
			Material m;
			if (made.TryGetValue(name, out m) && m != null)
			{
				return m;
			}
			m = new Material(Standard());
			m.name = "Furniture_" + name;
			m.color = tint;
			if (tex != null)
			{
				m.mainTexture = tex;
			}
			if (m.HasProperty("_Metallic"))
			{
				m.SetFloat("_Metallic", metallic);
			}
			if (m.HasProperty("_Glossiness"))
			{
				m.SetFloat("_Glossiness", smoothness);
			}
			made[name] = m;
			return m;
		}

		// Shelving uprights: blue powder coat, chipped at the edges.
		public static Material RackBlue()
		{
			return Make("RackBlue", Paint("rackblue", new Color(0.12f, 0.27f, 0.52f), 0.05f, 0.25f, 11), Color.white, 0.35f, 0.42f);
		}

		// Shelf beams: safety orange.
		public static Material RackOrange()
		{
			return Make("RackOrange", Paint("rackorange", new Color(0.86f, 0.38f, 0.07f), 0.05f, 0.2f, 12), Color.white, 0.3f, 0.45f);
		}

		// Decking boards and crate slats.
		public static Material Wood()
		{
			return Make("Wood", WoodTexture("wood", new Color(0.62f, 0.47f, 0.3f), 21), Color.white, 0f, 0.18f);
		}

		public static Material DarkWood()
		{
			return Make("DarkWood", WoodTexture("darkwood", new Color(0.36f, 0.26f, 0.16f), 22), Color.white, 0f, 0.12f);
		}

		// The safe's body: deep green-black enamel.
		public static Material SafeBody()
		{
			return Make("SafeBody", Paint("safe", new Color(0.09f, 0.15f, 0.12f), 0.03f, 0.08f, 31), Color.white, 0.55f, 0.62f);
		}

		public static Material Brass()
		{
			return Make("Brass", Paint("brass", new Color(0.78f, 0.6f, 0.28f), 0.06f, 0f, 41), Color.white, 0.9f, 0.7f);
		}

		public static Material Chrome()
		{
			return Make("Chrome", Paint("chrome", new Color(0.78f, 0.79f, 0.8f), 0.03f, 0f, 42), Color.white, 0.95f, 0.85f);
		}

		// Bare, slightly rusty steel: bolts, crate corners, safe hinges.
		public static Material Steel()
		{
			return Make("Steel", Paint("steel", new Color(0.4f, 0.4f, 0.41f), 0.1f, 0.35f, 43), Color.white, 0.8f, 0.4f);
		}

		public static Material Black()
		{
			return Make("Black", null, new Color(0.03f, 0.03f, 0.03f), 0f, 0.3f);
		}

		public static Material Rope()
		{
			return Make("Rope", WoodTexture("rope", new Color(0.66f, 0.56f, 0.38f), 51), Color.white, 0f, 0.05f);
		}

		// --- Textures. ---

		private const int Size = 256;

		// Paint over steel: fine speckle, faint streaks, and chips through to bare
		// metal (`chips`), with darker grime (`grime`) in blotches.
		private static Texture2D Paint(string name, Color paint, float chips, float grime, int seed)
		{
			Texture2D t = New(name);
			Color metal = new Color(0.45f, 0.44f, 0.42f);
			Color dirt = new Color(0.16f, 0.13f, 0.1f);
			float ox = seed * 17.3f, oy = seed * 9.1f;
			Color[] px = new Color[Size * Size];
			for (int y = 0; y < Size; y++)
			{
				for (int x = 0; x < Size; x++)
				{
					float u = x / (float)Size, v = y / (float)Size;
					float speckle = (Hash(x, y, seed) - 0.5f) * 0.06f;
					float streak = (Tile(u, v, 2f, 18f, ox, oy) - 0.5f) * 0.05f;
					Color c = paint * (1f + speckle + streak);
					float chip = Tile(u, v, 9f, ox + 3f, oy + 5f);
					if (chip > 1f - chips)
					{
						c = Color.Lerp(metal, c, 0.25f);
					}
					float blot = Tile(u, v, 3f, ox + 11f, oy + 7f);
					c = Color.Lerp(c, dirt, Mathf.Clamp01((blot - 0.55f) * 2.2f) * grime);
					c.a = 1f;
					px[y * Size + x] = c;
				}
			}
			t.SetPixels(px);
			t.Apply(true);
			return t;
		}

		// Planks: grain lines warped by noise, darker knots, and lighter wear.
		private static Texture2D WoodTexture(string name, Color wood, int seed)
		{
			Texture2D t = New(name);
			float ox = seed * 13.7f, oy = seed * 5.3f;
			Color[] px = new Color[Size * Size];
			for (int y = 0; y < Size; y++)
			{
				for (int x = 0; x < Size; x++)
				{
					float u = x / (float)Size, v = y / (float)Size;
					float warp = Tile(u, v, 2f, 6f, ox, oy) * 3f;
					float grain = Mathf.Sin((v * 24f + warp) * Mathf.PI * 2f);
					float fine = (Hash(x, y, seed) - 0.5f) * 0.08f;
					float knot = Mathf.Clamp01((Tile(u, v, 5f, ox + 9f, oy + 2f) - 0.78f) * 6f);
					float shade = 0.85f + 0.12f * grain + fine - 0.35f * knot;
					float wear = Tile(u, v, 2f, ox + 4f, oy + 8f);
					Color c = wood * shade;
					c = Color.Lerp(c, new Color(0.3f, 0.27f, 0.24f), Mathf.Clamp01((wear - 0.6f) * 1.5f) * 0.35f);
					c.a = 1f;
					px[y * Size + x] = c;
				}
			}
			t.SetPixels(px);
			t.Apply(true);
			return t;
		}

		private static Texture2D New(string name)
		{
			Texture2D t = new Texture2D(Size, Size, TextureFormat.RGBA32, true);
			t.name = "Furniture_" + name;
			t.wrapMode = TextureWrapMode.Repeat;
			t.filterMode = FilterMode.Trilinear;
			t.anisoLevel = 4;
			return t;
		}

		// Perlin noise with `cells` cells across the texture (u, v in 0..1), made
		// to tile: blended with its copies one period over.
		private static float Tile(float u, float v, float cells, float ox, float oy)
		{
			float x = u * cells, y = v * cells;
			float a = Mathf.PerlinNoise(ox + x, oy + y);
			float b = Mathf.PerlinNoise(ox + x - cells, oy + y);
			float c = Mathf.PerlinNoise(ox + x, oy + y - cells);
			float d = Mathf.PerlinNoise(ox + x - cells, oy + y - cells);
			return Mathf.Lerp(Mathf.Lerp(a, b, u), Mathf.Lerp(c, d, u), v);
		}

		// Stretched noise: `cu` cells across, `cv` down (both whole numbers).
		private static float Tile(float u, float v, float cu, float cv, float ox, float oy)
		{
			float x = u * cu, y = v * cv;
			float a = Mathf.PerlinNoise(ox + x, oy + y);
			float b = Mathf.PerlinNoise(ox + x - cu, oy + y);
			float c = Mathf.PerlinNoise(ox + x, oy + y - cv);
			float d = Mathf.PerlinNoise(ox + x - cu, oy + y - cv);
			return Mathf.Lerp(Mathf.Lerp(a, b, u), Mathf.Lerp(c, d, u), v);
		}

		private static float Hash(int x, int y, int seed)
		{
			unchecked
			{
				uint h = (uint)(x * 374761393 + y * 668265263 + seed * 362437);
				h = (h ^ (h >> 13)) * 1274126177;
				return ((h ^ (h >> 16)) & 0xFFFF) / 65535f;
			}
		}
	}
}
