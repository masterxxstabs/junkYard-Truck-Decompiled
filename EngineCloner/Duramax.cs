using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EngineCloner
{
	// The 6.6L Duramax turbo-diesel: a big V8 built on the game's own V8, so it fits the same
	// engine bays, takes the same parts (heads, pistons, intake, turbos...),
	// mounts, oil and gauges, and is bought, bolted in and saved like our other
	// engines. It has its own model (UserData/EngineCloner/models/duramax.obj,
	// made by tools/duramax_model.py), each piece shown on one of the V8's own
	// parts (block, heads, covers, intake, turbo on the distributor, fuel lines on
	// the plug wires, air inlet and filter on the carb and air cleaner, CP3 on
	// the fuel pump...), so it still comes apart, wears and goes back together
	// the game's way. And more torque: the game sets the truck's torque from the V8
	// each time it refreshes the engine, and the Duramax multiplies it.
	internal static class Duramax
	{
		private static readonly Color Block = new Color(0.17f, 0.18f, 0.19f);
		private static readonly Color Covers = new Color(0.035f, 0.035f, 0.04f);

		// The model's pieces by the V8 part they're shown on, loaded once.
		private static Dictionary<string, KeyValuePair<Mesh, Material[]>> pieces;
		private static bool tried;

		public static string ModelPath
		{
			get { return Path.Combine(Path.Combine(Path.Combine(Path.Combine(Path.GetDirectoryName(Application.dataPath), "UserData"), "EngineCloner"), "models"), "duramax.obj"); }
		}

		// On the shop's template: every copy (bought, saved, bolted into a truck)
		// is made from it.
		public static void Style(GameObject block)
		{
			if (Load() && pieces.Count > 0)
			{
				int shown = 0;
				foreach (Transform t in block.GetComponentsInChildren<Transform>(true))
				{
					KeyValuePair<Mesh, Material[]> piece;
					if (!pieces.TryGetValue(t == block.transform ? "v8_block" : t.name, out piece))
					{
						continue;
					}
					MeshFilter f = t.GetComponent<MeshFilter>();
					MeshRenderer r = t.GetComponent<MeshRenderer>();
					if (f == null || r == null)
					{
						continue;
					}
					f.sharedMesh = piece.Key;
					r.sharedMaterials = piece.Value;
					shown++;
				}
				// The V8's lower-detail look at a distance would come back: one look.
				foreach (LODGroup lod in block.GetComponentsInChildren<LODGroup>(true))
				{
					lod.enabled = false;
				}
				EngineClonerMod.Log("6.6L Duramax: its own model on " + shown + " part(s).");
				return;
			}
			// No model: the V8 in Duramax colors.
			Renderer br = block.GetComponent<Renderer>();
			if (br != null)
			{
				Material[] mats = br.sharedMaterials;
				for (int i = 0; i < mats.Length; i++)
				{
					if (mats[i] != null)
					{
						mats[i] = new Material(mats[i]) { name = mats[i].name + " (Duramax)", color = Block };
					}
				}
				br.sharedMaterials = mats;
			}
			foreach (durability d in block.GetComponentsInChildren<durability>(true))
			{
				string n = d.name;
				if (n == "v8_valvecoverD_e" || n == "v8_valvecoverP_e" || n == "v8_intakeman_e" || n == "valveCoverD_e" || n == "valveCoverP_e" || n == "intakeMan_e")
				{
					// The game's own paint (as from a paint can).
					EngineShop.SetField(d, "painted", true);
					EngineShop.SetField(d, "red", Covers.r);
					EngineShop.SetField(d, "green", Covers.g);
					EngineShop.SetField(d, "blue", Covers.b);
					EngineShop.SetField(d, "metallic", 0.3f);
					EngineShop.SetField(d, "smoothness", 0.75f);
					EngineShop.SetField(d, "paintSlot", 0);
				}
			}
			EngineClonerMod.Log("6.6L Duramax: no " + ModelPath + ", so it's the V8 in Duramax colors.");
		}

		private static bool Load()
		{
			if (tried)
			{
				return pieces != null;
			}
			tried = true;
			if (!File.Exists(ModelPath))
			{
				return false;
			}
			try
			{
				ModelData data = ModelData.Load(ModelPath);
				Material template = StandardTemplate();
				Dictionary<string, Material> materials = new Dictionary<string, Material>();
				pieces = new Dictionary<string, KeyValuePair<Mesh, Material[]>>();
				foreach (ModelNode node in data.roots)
				{
					foreach (ModelMesh m in node.meshes)
					{
						List<Material> mats = new List<Material>();
						Mesh mesh = BuildMesh(m, data, template, materials, mats);
						mesh.name = "Duramax_" + node.name;
						Object.DontDestroyOnLoad(mesh);
						pieces[node.name] = new KeyValuePair<Mesh, Material[]>(mesh, mats.ToArray());
					}
				}
				EngineClonerMod.Log("Loaded the 6.6L Duramax model: " + pieces.Count + " parts.");
				return true;
			}
			catch (Exception e)
			{
				pieces = null;
				EngineClonerMod.Log("Couldn't load " + ModelPath + ": " + e.Message);
				return false;
			}
		}

		// The game's Standard shader, from a material it already uses.
		private static Material StandardTemplate()
		{
			foreach (Material m in Resources.FindObjectsOfTypeAll<Material>())
			{
				if (m != null && m.shader != null && m.shader.name == "Standard")
				{
					return m;
				}
			}
			return null;
		}

		private static Mesh BuildMesh(ModelMesh m, ModelData data, Material template, Dictionary<string, Material> made, List<Material> mats)
		{
			Mesh mesh = new Mesh();
			if (m.positions.Count > 65000)
			{
				mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
			}
			Vector3[] v = new Vector3[m.positions.Count];
			for (int i = 0; i < v.Length; i++)
			{
				v[i] = new Vector3(m.positions[i].x, m.positions[i].y, m.positions[i].z);
			}
			mesh.vertices = v;
			if (m.uvs.Count == v.Length)
			{
				Vector2[] uv = new Vector2[v.Length];
				for (int i = 0; i < uv.Length; i++)
				{
					uv[i] = new Vector2(m.uvs[i].x, m.uvs[i].y);
				}
				mesh.uv = uv;
			}
			mesh.subMeshCount = m.submeshes.Count;
			for (int s = 0; s < m.submeshes.Count; s++)
			{
				mesh.SetTriangles(m.submeshes[s].ToArray(), s);
				ModelMaterial spec = m.materials[s] >= 0 && m.materials[s] < data.materials.Count ? data.materials[m.materials[s]] : null;
				string name = spec != null ? spec.name : "D_Black";
				Material mat;
				if (!made.TryGetValue(name, out mat))
				{
					mat = template != null ? new Material(template) : new Material(Shader.Find("Standard"));
					mat.name = "Duramax_" + name;
					if (mat.HasProperty("_MainTex"))
					{
						mat.mainTexture = null;
					}
					foreach (string tex in new[] { "_BumpMap", "_MetallicGlossMap", "_OcclusionMap", "_DetailAlbedoMap", "_DetailNormalMap", "_EmissionMap" })
					{
						if (mat.HasProperty(tex))
						{
							mat.SetTexture(tex, null);
						}
					}
					float[] c = spec != null ? spec.color : new[] { 0.1f, 0.1f, 0.1f, 1f };
					mat.color = new Color(c[0], c[1], c[2], 1f);
					foreach (string k in new[] { "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A", "_EMISSION", "_DETAIL_MULX2", "_PARALLAXMAP", "_ALPHATEST_ON", "_ALPHABLEND_ON", "_ALPHAPREMULTIPLY_ON", "_METALLICGLOSSMAP", "_NORMALMAP" })
					{
						mat.DisableKeyword(k);
					}
					// The part's baked textures: color (with its shading, grime and
					// wear), normal map, and metal / smoothness.
					if (spec != null && spec.image != null && mat.HasProperty("_MainTex"))
					{
						mat.mainTexture = Texture(spec.image, name, false);
					}
					if (spec != null && spec.normalImage != null && mat.HasProperty("_BumpMap"))
					{
						mat.SetTexture("_BumpMap", Texture(spec.normalImage, name + "_n", true));
						if (mat.HasProperty("_BumpScale"))
						{
							mat.SetFloat("_BumpScale", 1f);
						}
						mat.EnableKeyword("_NORMALMAP");
					}
					// Also the fallback if the game's build left out the Standard
					// shader's metal-map variant.
					if (mat.HasProperty("_Metallic"))
					{
						mat.SetFloat("_Metallic", spec != null ? spec.metallic : 0f);
					}
					if (mat.HasProperty("_Glossiness"))
					{
						mat.SetFloat("_Glossiness", spec != null ? spec.smoothness : 0.4f);
					}
					if (spec != null && spec.metalImage != null && mat.HasProperty("_MetallicGlossMap"))
					{
						mat.SetTexture("_MetallicGlossMap", Texture(spec.metalImage, name + "_m", true));
						if (mat.HasProperty("_GlossMapScale"))
						{
							mat.SetFloat("_GlossMapScale", 1f);
						}
						mat.EnableKeyword("_METALLICGLOSSMAP");
					}
					Object.DontDestroyOnLoad(mat);
					made[name] = mat;
				}
				mats.Add(mat);
			}
			if (m.normals.Count == v.Length)
			{
				Vector3[] n = new Vector3[v.Length];
				for (int i = 0; i < n.Length; i++)
				{
					n[i] = new Vector3(m.normals[i].x, m.normals[i].y, m.normals[i].z);
				}
				mesh.normals = n;
			}
			else
			{
				mesh.RecalculateNormals();
			}
			mesh.RecalculateTangents();
			mesh.RecalculateBounds();
			return mesh;
		}

		private static Texture2D Texture(byte[] image, string name, bool linear)
		{
			Texture2D t = new Texture2D(2, 2, TextureFormat.RGBA32, true, linear);
			t.LoadImage(image);
			t.name = "Duramax_" + name;
			// Each part's textures are its own atlas: no wrapping between edges.
			t.wrapMode = TextureWrapMode.Clamp;
			t.anisoLevel = 4;
			// Compressed on the graphics card (a quarter of the memory), and the
			// copy in memory freed.
			if (t.width % 4 == 0 && t.height % 4 == 0)
			{
				t.Compress(true);
			}
			t.Apply(true, true);
			Object.DontDestroyOnLoad(t);
			return t;
		}

		public static bool Is(Component engine)
		{
			ShopEngine marker = engine != null ? engine.GetComponent<ShopEngine>() : null;
			return marker != null && marker.kind == "duramax";
		}
	}

	[HarmonyPatch(typeof(enginev8), "Refresh")]
	internal static class DuramaxPowerPatch
	{
		private static readonly FieldInfo WhichTruck = AccessTools.Field(typeof(enginev8), "whichTruck");
		private static readonly FieldInfo Truck = AccessTools.Field(typeof(enginev8), "truck");
		private static readonly FieldInfo Truck2 = AccessTools.Field(typeof(enginev8), "truck2");

		private static Component CarOf(enginev8 engine)
		{
			int which = WhichTruck != null ? (int)WhichTruck.GetValue(engine) : 1;
			FieldInfo field = which == 2 ? Truck2 : Truck;
			GameObject truck = field != null ? field.GetValue(engine) as GameObject : null;
			return truck != null ? (truck.GetComponent("car") ?? truck.GetComponent("car4")) : null;
		}

		private static float Get(Component car, string name)
		{
			FieldInfo f = AccessTools.Field(car.GetType(), name);
			return f != null && f.FieldType == typeof(float) ? (float)f.GetValue(car) : -1f;
		}

		// The torque before the refresh: only a value the V8 set during it is
		// multiplied (a refresh that leaves it alone mustn't multiply it again).
		private static void Prefix(enginev8 __instance, out float __state)
		{
			Component car = Duramax.Is(__instance) ? CarOf(__instance) : null;
			__state = car != null ? Get(car, "maxTorqueStatic") : -1f;
			if (car != null)
			{
				// A marker the V8 overwrites when it sets the torque.
				AccessTools.Field(car.GetType(), "maxTorqueStatic")?.SetValue(car, -12345f);
			}
		}

		private static void Postfix(enginev8 __instance, float __state)
		{
			Component car = Duramax.Is(__instance) ? CarOf(__instance) : null;
			if (car == null)
			{
				return;
			}
			float set = Get(car, "maxTorqueStatic");
			if (set == -12345f)
			{
				// Not set this time: put it back as it was.
				AccessTools.Field(car.GetType(), "maxTorqueStatic")?.SetValue(car, __state);
				return;
			}
			foreach (string name in new[] { "maxTorque", "maxTorqueStatic" })
			{
				FieldInfo f = AccessTools.Field(car.GetType(), name);
				float v = Get(car, name);
				if (f != null && v > 0f)
				{
					f.SetValue(car, v * EngineClonerMod.DuramaxPower);
				}
			}
		}

		// If Refresh throws (an engine missing parts), the marker mustn't stay (also
		// when another finalizer already swallowed the exception).
		private static System.Exception Finalizer(System.Exception __exception, enginev8 __instance, float __state)
		{
			Component car = Duramax.Is(__instance) ? CarOf(__instance) : null;
			if (car != null && Get(car, "maxTorqueStatic") == -12345f)
			{
				AccessTools.Field(car.GetType(), "maxTorqueStatic")?.SetValue(car, __state);
			}
			return __exception;
		}
	}
}
