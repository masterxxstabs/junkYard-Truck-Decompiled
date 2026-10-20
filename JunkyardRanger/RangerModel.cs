using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace JunkyardRanger
{
	// The Ranger's body (UserData/JunkyardRanger/models/ranger.obj), made from the
	// game's F100 in the Diamondback's frame:
	//
	//   Body, Fascia, Bumper, Lights, Glass, GasDoor  on the truck itself
	//   DoorD, DoorP, Hood, Tailgate                  in the frame of the Diamondback
	//                                                 part they move with (ranger.json)
	//
	// Materials are the game's own, found by name in the loaded game (the F100's
	// body, glass and trim). The body ("Tex_RGB_23", the F100's) is drawn in the
	// F100's layers: its base, a paint layer (ranger_paint.png: the game's paint
	// mask, tinted with the paint color) and its mud layer. R_* materials (lamps,
	// chrome, black trim) are made from the MTL's values on the game's shader.
	internal static class RangerModel
	{
		public const string BodyMaterial = "Tex_RGB_23";

		internal class Group
		{
			public string name;
			public Mesh mesh;
			public List<string> materials = new List<string>(); // per submesh; the body layer last
			public string follow;                                // path in the Diamondback, or null
		}

		private static List<Group> groups;
		private static Dictionary<string, ModelMaterial> mtl;
		private static Vector3 fuelCap;
		private static Texture2D paintTexture;
		private static string loadError;
		private static DateTime loadedStamp;

		public static Vector3 FuelCap
		{
			get { return fuelCap; }
		}

		// The bull bar isn't part of the body: it's shown in the brush guard slot
		// (RangerBullBar), in that slot's frame.
		public const string BullBarGroup = "BullBar";

		public static bool BullBar(out Mesh mesh, out Material[] materials)
		{
			mesh = null;
			materials = null;
			if (groups == null)
			{
				return false;
			}
			foreach (Group g in groups)
			{
				if (g.name == BullBarGroup)
				{
					GameMaterials.Refresh();
					mesh = g.mesh;
					materials = MaterialsFor(g, null);
					return true;
				}
			}
			return false;
		}

		public static string ModelPath
		{
			get { return Path.Combine(Path.Combine(RangerMod.Folder, "models"), "ranger.obj"); }
		}

		// Parse the files once (again if they change). False with a reason if missing.
		public static bool Load(out string why)
		{
			string obj = ModelPath;
			string dir = Path.GetDirectoryName(obj);
			string json = Path.Combine(dir, "ranger.json");
			if (!File.Exists(obj) || !File.Exists(json))
			{
				why = "the body files are missing: put ranger.obj, ranger.mtl, ranger.json and ranger_paint.png in " + dir;
				return false;
			}
			DateTime stamp = File.GetLastWriteTimeUtc(obj);
			if (groups != null && stamp == loadedStamp)
			{
				why = null;
				return true;
			}
			if (loadError != null && stamp == loadedStamp)
			{
				why = loadError;
				return false;
			}
			loadedStamp = stamp;
			try
			{
				Dictionary<string, object> meta = (Dictionary<string, object>)MiniJson.Parse(File.ReadAllText(json));
				Dictionary<string, object> follow = meta.ContainsKey("follow") ? (Dictionary<string, object>)meta["follow"] : new Dictionary<string, object>();
				List<object> cap = meta.ContainsKey("fuelCap") ? (List<object>)meta["fuelCap"] : null;
				// The fuel cap's center in the Diamondback root's frame (Unity terms).
				fuelCap = cap != null && cap.Count == 3 ? new Vector3((float)(double)cap[0], (float)(double)cap[1], (float)(double)cap[2]) : Vector3.zero;
				ModelData data = ModelData.Load(obj);
				mtl = new Dictionary<string, ModelMaterial>();
				foreach (ModelMaterial m in data.materials)
				{
					mtl[m.name] = m;
				}
				groups = new List<Group>();
				foreach (ModelNode node in data.roots)
				{
					foreach (ModelMesh m in node.meshes)
					{
						Group g = new Group();
						g.name = node.name;
						g.follow = follow.ContainsKey(node.name) ? (string)follow[node.name] : null;
						g.mesh = BuildMesh(m, data, g.materials);
						g.mesh.name = "Ranger_" + node.name;
						groups.Add(g);
					}
				}
				string paintFile = Path.Combine(dir, "ranger_paint.png");
				paintTexture = null;
				if (File.Exists(paintFile))
				{
					paintTexture = new Texture2D(2, 2, TextureFormat.RGBA32, true);
					paintTexture.LoadImage(File.ReadAllBytes(paintFile));
					paintTexture.name = "ranger_paint";
					paintTexture.wrapMode = TextureWrapMode.Repeat;
					Object.DontDestroyOnLoad(paintTexture);
				}
				int verts = 0;
				foreach (Group g in groups)
				{
					verts += g.mesh.vertexCount;
					Object.DontDestroyOnLoad(g.mesh);
				}
				RangerMod.Log("Loaded the Ranger body: " + groups.Count + " parts, " + verts + " vertices, " + mtl.Count + " materials.");
				loadError = null;
				why = null;
				return true;
			}
			catch (Exception e)
			{
				groups = null;
				loadError = "couldn't read the body files: " + e.Message;
				why = loadError;
				RangerMod.Log(loadError + "\n" + e);
				return false;
			}
		}

		// One Unity mesh per OBJ group; the layered body submesh goes last, so the
		// extra layer materials draw over it (Unity draws materials past the last
		// submesh on that submesh again).
		private static Mesh BuildMesh(ModelMesh m, ModelData data, List<string> names)
		{
			Mesh mesh = new Mesh();
			if (m.positions.Count > 65000)
			{
				mesh.indexFormat = IndexFormat.UInt32;
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
			List<int> order = new List<int>();
			for (int s = 0; s < m.submeshes.Count; s++)
			{
				if (MaterialName(data, m.materials[s]) != BodyMaterial)
				{
					order.Add(s);
				}
			}
			for (int s = 0; s < m.submeshes.Count; s++)
			{
				if (MaterialName(data, m.materials[s]) == BodyMaterial)
				{
					order.Add(s);
				}
			}
			mesh.subMeshCount = order.Count;
			for (int i = 0; i < order.Count; i++)
			{
				mesh.SetTriangles(m.submeshes[order[i]].ToArray(), i);
				names.Add(MaterialName(data, m.materials[order[i]]));
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

		private static string MaterialName(ModelData data, int index)
		{
			return index >= 0 && index < data.materials.Count ? data.materials[index].name : "R_Black";
		}

		// --- Building the body on a Ranger. ---

		// The parts' look, on a copy of the Diamondback (root = the copy).
		public static void Dress(GameObject root, RangerBody body)
		{
			GameMaterials.Refresh();
			foreach (Group g in groups)
			{
				if (g.name == BullBarGroup)
				{
					continue;
				}
				Transform parent = root.transform;
				if (g.follow != null)
				{
					Transform target = root.transform.Find(g.follow);
					if (target == null)
					{
						RangerMod.Log("The Diamondback has no " + g.follow + " for the Ranger's " + g.name + "; it's fixed to the body instead.");
					}
					else
					{
						parent = target;
					}
				}
				GameObject go = new GameObject("Ranger" + g.name);
				go.transform.SetParent(parent, false);
				go.AddComponent<RangerPart>().group = g.name;
				go.AddComponent<MeshFilter>().sharedMesh = g.mesh;
				MeshRenderer renderer = go.AddComponent<MeshRenderer>();
				renderer.sharedMaterials = MaterialsFor(g, body);
				if (g.follow != null && parent != root.transform)
				{
					go.AddComponent<RangerFollower>().target = parent.GetComponent<Renderer>();
				}
				if (g.name == "GasDoor")
				{
					body.gasDoor = go.transform;
				}
			}
		}

		private static Material[] MaterialsFor(Group g, RangerBody body)
		{
			List<Material> list = new List<Material>();
			foreach (string name in g.materials)
			{
				if (name == BodyMaterial)
				{
					list.Add(GameMaterials.Find(BodyMaterial) ?? Fallback(name));
					list.Add(body.PaintMaterial(g.name, PaintLayerTemplate(), paintTexture, RangerMod.Weathered));
					Material mud = GameMaterials.Find("Tex_RGB_23Mud");
					if (mud != null)
					{
						list.Add(mud);
					}
				}
				else
				{
					list.Add(GameMaterials.Find(name) ?? Fallback(name));
				}
			}
			return list.ToArray();
		}

		// The F100's paint layer (Standard shader, alpha-tested by the paint mask).
		private static Material PaintLayerTemplate()
		{
			return GameMaterials.Find("Tex_RGB_23Paint");
		}

		private static readonly Dictionary<string, Material> fallbacks = new Dictionary<string, Material>();

		// A material from the MTL's values on the game's Standard shader; see-through
		// ones from the game's fade material (the F100's mud layer), so the game
		// has that shader variant.
		private static Material Fallback(string name)
		{
			Material m;
			if (fallbacks.TryGetValue(name, out m) && m != null)
			{
				return m;
			}
			ModelMaterial spec;
			mtl.TryGetValue(name, out spec);
			float[] c = spec != null ? spec.color : new[] { 0.5f, 0.5f, 0.5f, 1f };
			Material template = c[3] < 0.99f ? GameMaterials.Find("Tex_RGB_23Mud") ?? GameMaterials.Find("vehicleGlass") : null;
			if (template == null)
			{
				template = GameMaterials.Standard();
			}
			m = template != null ? new Material(template) : new Material(Shader.Find("Standard"));
			m.name = "Ranger_" + name;
			if (m.HasProperty("_MainTex"))
			{
				m.mainTexture = null;
			}
			foreach (string tex in new[] { "_BumpMap", "_MetallicGlossMap", "_OcclusionMap", "_DetailAlbedoMap", "_DetailNormalMap" })
			{
				if (m.HasProperty(tex))
				{
					m.SetTexture(tex, null);
				}
			}
			m.color = new Color(c[0], c[1], c[2], c[3]);
			if (m.HasProperty("_Metallic"))
			{
				m.SetFloat("_Metallic", spec != null ? spec.metallic : 0f);
			}
			if (m.HasProperty("_Glossiness"))
			{
				m.SetFloat("_Glossiness", spec != null ? spec.smoothness : 0.4f);
			}
			if (name == "R_LensClear" || name == "R_LensAmber" || name == "R_Bulb")
			{
				// Lamps glow a little, like the game's.
				m.EnableKeyword("_EMISSION");
				if (m.HasProperty("_EmissionColor"))
				{
					m.SetColor("_EmissionColor", new Color(c[0], c[1], c[2]) * 0.08f);
				}
			}
			fallbacks[name] = m;
			return m;
		}

		public static void Reset()
		{
			fallbacks.Clear();
		}
	}

	// The game's materials by name (the F100's body, glass, trim...), from
	// whatever is loaded, including objects not active yet.
	internal static class GameMaterials
	{
		private static Dictionary<string, Material> byName;
		private static float refreshedAt = -100f;

		public static void Refresh()
		{
			if (byName != null && Time.realtimeSinceStartup - refreshedAt < 5f)
			{
				return;
			}
			refreshedAt = Time.realtimeSinceStartup;
			byName = new Dictionary<string, Material>();
			foreach (Material m in Resources.FindObjectsOfTypeAll<Material>())
			{
				if (m == null || m.name.StartsWith("Ranger_", StringComparison.Ordinal))
				{
					continue;
				}
				string name = m.name.Replace(" (Instance)", "");
				if (!byName.ContainsKey(name) || m.name == name)
				{
					byName[name] = m;
				}
			}
		}

		public static Material Find(string name)
		{
			Refresh();
			Material m;
			return byName.TryGetValue(name, out m) && m != null ? m : null;
		}

		public static Material Standard()
		{
			Material m = Find(RangerModel.BodyMaterial);
			if (m != null)
			{
				return m;
			}
			foreach (Material any in byName.Values)
			{
				if (any != null && any.shader != null && any.shader.name == "Standard")
				{
					return any;
				}
			}
			return null;
		}
	}

	// On every part of the Ranger's body (so saves and the dress-up skip them).
	public class RangerPart : MonoBehaviour
	{
		public string group;
	}

	// A body part that moves with one of the Diamondback's (door, hood, tailgate):
	// shown while that part is there (the game hides a part it takes off by
	// turning its renderer off; ours is hidden from view, not off).
	public class RangerFollower : MonoBehaviour
	{
		public Renderer target;
		private Renderer own;
		private float next;

		private void Start()
		{
			own = GetComponent<Renderer>();
		}

		private void Update()
		{
			if (Time.time < next || own == null || target == null)
			{
				return;
			}
			next = Time.time + 0.25f;
			if (own.enabled != target.enabled)
			{
				own.enabled = target.enabled;
			}
		}
	}
}
