using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace TruckExporter
{
	// One vehicle to model.obj/.mtl (+ shared textures) and hierarchy.txt.
	//
	// The OBJ is in the vehicle's own frame: meters, Y up, front +Z, X mirrored
	// (OBJ is right-handed), the same frame our mods' OBJ models use. Every
	// renderer is a group named by its path under the vehicle; a group ending in
	// "#off" was hidden at export (a part not fitted, an upgrade not bought).
	internal class Exporter
	{
		private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

		private readonly string textureDir;
		private readonly Dictionary<int, string> textures = new Dictionary<int, string>();
		private readonly HashSet<string> textureNames = new HashSet<string>();

		public Exporter(string textureDir)
		{
			this.textureDir = textureDir;
		}

		public string Export(GameObject vehicle, string dir)
		{
			Directory.CreateDirectory(dir);
			Directory.CreateDirectory(textureDir);
			Transform root = vehicle.transform;
			StringBuilder obj = new StringBuilder();
			StringBuilder mtl = new StringBuilder();
			obj.AppendLine("# " + vehicle.name + " exported by Truck Exporter");
			obj.AppendLine("# Vehicle frame: meters, Y up, front +Z, X mirrored (right-handed OBJ)");
			obj.AppendLine("mtllib model.mtl");
			Dictionary<Material, string> materials = new Dictionary<Material, string>();
			int vBase = 1, tBase = 1, nBase = 1;
			int parts = 0, gpu = 0, boxes = 0;
			foreach (Renderer r in root.GetComponentsInChildren<Renderer>(true))
			{
				Mesh mesh = null;
				MeshFilter filter = r.GetComponent<MeshFilter>();
				if (filter != null)
				{
					mesh = filter.sharedMesh;
				}
				else if (r is SkinnedMeshRenderer)
				{
					mesh = ((SkinnedMeshRenderer)r).sharedMesh;
				}
				if (mesh == null)
				{
					continue;
				}
				MeshData data;
				string how;
				try
				{
					data = MeshReader.Read(mesh, out how);
				}
				catch (Exception e)
				{
					data = MeshReader.Box(mesh);
					how = "box (" + e.GetType().Name + ": " + e.Message + ")";
				}
				if (how.StartsWith("gpu", StringComparison.Ordinal))
				{
					gpu++;
				}
				else if (how.StartsWith("box", StringComparison.Ordinal))
				{
					boxes++;
				}
				bool on = r.enabled && r.gameObject.activeInHierarchy;
				obj.AppendLine("o " + Clean(PathOf(root, r.transform)) + (on ? "" : "#off"));
				obj.AppendLine("# mesh " + mesh.name + " (" + how + ")");
				Matrix4x4 toVehicle = root.worldToLocalMatrix * r.transform.localToWorldMatrix;
				Matrix4x4 normalMatrix = toVehicle.inverse.transpose;
				foreach (Vector3 v in data.positions)
				{
					Vector3 p = toVehicle.MultiplyPoint3x4(v);
					obj.Append("v ").Append(F(-p.x)).Append(' ').Append(F(p.y)).Append(' ').Append(F(p.z)).Append('\n');
				}
				bool hasUv = data.uvs != null && data.uvs.Length == data.positions.Length;
				bool hasNormals = data.normals != null && data.normals.Length == data.positions.Length;
				if (hasUv)
				{
					foreach (Vector2 t in data.uvs)
					{
						obj.Append("vt ").Append(F(t.x)).Append(' ').Append(F(t.y)).Append('\n');
					}
				}
				if (hasNormals)
				{
					foreach (Vector3 n in data.normals)
					{
						Vector3 m = normalMatrix.MultiplyVector(n).normalized;
						obj.Append("vn ").Append(F(-m.x)).Append(' ').Append(F(m.y)).Append(' ').Append(F(m.z)).Append('\n');
					}
				}
				Material[] mats = r.sharedMaterials;
				for (int s = 0; s < data.submeshes.Count; s++)
				{
					Material m = mats.Length > 0 ? mats[Math.Min(s, mats.Length - 1)] : null;
					obj.AppendLine("usemtl " + MaterialName(m, materials, mtl));
					int[] tris = data.submeshes[s];
					for (int i = 0; i + 2 < tris.Length; i += 3)
					{
						// Mirroring X flips the winding back.
						obj.Append("f ").Append(Corner(tris[i], vBase, tBase, nBase, hasUv, hasNormals)).Append(' ')
							.Append(Corner(tris[i + 2], vBase, tBase, nBase, hasUv, hasNormals)).Append(' ')
							.Append(Corner(tris[i + 1], vBase, tBase, nBase, hasUv, hasNormals)).Append('\n');
					}
				}
				vBase += data.positions.Length;
				if (hasUv)
				{
					tBase += data.uvs.Length;
				}
				if (hasNormals)
				{
					nBase += data.normals.Length;
				}
				parts++;
			}
			File.WriteAllText(Path.Combine(dir, "model.obj"), obj.ToString());
			File.WriteAllText(Path.Combine(dir, "model.mtl"), mtl.ToString());
			File.WriteAllText(Path.Combine(dir, "hierarchy.txt"), Hierarchy(vehicle));
			return parts + " parts (" + gpu + " read back from the GPU, " + boxes + " only as boxes), " + materials.Count + " materials, " + textures.Count + " textures so far.";
		}

		private static string Corner(int i, int v, int t, int n, bool hasUv, bool hasNormals)
		{
			string s = (v + i).ToString(Inv);
			if (hasUv && hasNormals)
			{
				return s + "/" + (t + i).ToString(Inv) + "/" + (n + i).ToString(Inv);
			}
			if (hasUv)
			{
				return s + "/" + (t + i).ToString(Inv);
			}
			if (hasNormals)
			{
				return s + "//" + (n + i).ToString(Inv);
			}
			return s;
		}

		// --- Materials and textures. ---

		private string MaterialName(Material m, Dictionary<Material, string> materials, StringBuilder mtl)
		{
			if (m == null)
			{
				return "none";
			}
			string name;
			if (materials.TryGetValue(m, out name))
			{
				return name;
			}
			name = "m" + materials.Count + "_" + Clean(m.name);
			materials[m] = name;
			mtl.AppendLine("newmtl " + name);
			mtl.AppendLine("# material \"" + m.name + "\", shader \"" + (m.shader != null ? m.shader.name : "?") + "\", render queue " + m.renderQueue);
			mtl.AppendLine("# keywords: " + string.Join(" ", m.shaderKeywords));
			Color c = m.HasProperty("_Color") ? m.color : Color.white;
			mtl.AppendLine("Kd " + F(c.r) + " " + F(c.g) + " " + F(c.b));
			mtl.AppendLine("d " + F(c.a));
			if (m.HasProperty("_Metallic"))
			{
				mtl.AppendLine("Pm " + F(m.GetFloat("_Metallic")));
			}
			if (m.HasProperty("_Glossiness"))
			{
				mtl.AppendLine("Pr " + F(1f - m.GetFloat("_Glossiness")));
			}
			Shader shader = m.shader;
			if (shader != null)
			{
				for (int i = 0; i < shader.GetPropertyCount(); i++)
				{
					string prop = shader.GetPropertyName(i);
					ShaderPropertyType type = shader.GetPropertyType(i);
					try
					{
						switch (type)
						{
						case ShaderPropertyType.Color:
						case ShaderPropertyType.Vector:
							Vector4 v = m.GetVector(prop);
							mtl.AppendLine("# " + prop + " = " + F(v.x) + " " + F(v.y) + " " + F(v.z) + " " + F(v.w));
							break;
						case ShaderPropertyType.Float:
						case ShaderPropertyType.Range:
							mtl.AppendLine("# " + prop + " = " + F(m.GetFloat(prop)));
							break;
						case ShaderPropertyType.Texture:
							Texture tex = m.GetTexture(prop);
							if (tex == null)
							{
								break;
							}
							string file = SaveTexture(tex, IsLinear(prop));
							Vector2 scale = m.GetTextureScale(prop);
							Vector2 offset = m.GetTextureOffset(prop);
							mtl.AppendLine("# " + prop + " = " + (file ?? "(couldn't save " + tex.name + ")") + " scale " + F(scale.x) + " " + F(scale.y) + " offset " + F(offset.x) + " " + F(offset.y));
							if (file != null)
							{
								string key = prop == "_MainTex" ? "map_Kd" : prop == "_BumpMap" ? "map_Bump" : prop == "_MetallicGlossMap" ? "map_Pm" : prop == "_OcclusionMap" ? "map_Ka" : prop == "_EmissionMap" ? "map_Ke" : null;
								if (key != null)
								{
									mtl.AppendLine(key + " ../textures/" + file);
								}
							}
							break;
						}
					}
					catch (Exception e)
					{
						mtl.AppendLine("# " + prop + ": " + e.Message);
					}
				}
			}
			mtl.AppendLine();
			return name;
		}

		private static bool IsLinear(string prop)
		{
			string p = prop.ToLowerInvariant();
			return p.Contains("bump") || p.Contains("normal") || p.Contains("metallic") || p.Contains("occlusion") || p.Contains("parallax") || p.Contains("height") || p.Contains("mask") || p.Contains("rough");
		}

		// Copied through a render texture, so it works whether or not the game kept
		// the texture readable.
		private string SaveTexture(Texture tex, bool linear)
		{
			string file;
			if (textures.TryGetValue(tex.GetInstanceID(), out file))
			{
				return file;
			}
			file = Clean(tex.name.Length > 0 ? tex.name : "texture").Replace('/', '_');
			string baseName = file;
			for (int n = 2; !textureNames.Add(file); n++)
			{
				file = baseName + "_" + n;
			}
			file += ".png";
			textures[tex.GetInstanceID()] = file;
			int w = Mathf.Min(tex.width, 4096);
			int h = Mathf.Min(tex.height, 4096);
			if (w <= 0 || h <= 0 || tex.dimension != TextureDimension.Tex2D)
			{
				return null;
			}
			RenderTexture rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32, linear ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB);
			RenderTexture previous = RenderTexture.active;
			Texture2D copy = null;
			try
			{
				Graphics.Blit(tex, rt);
				RenderTexture.active = rt;
				copy = new Texture2D(w, h, TextureFormat.RGBA32, false, linear);
				copy.ReadPixels(new Rect(0, 0, w, h), 0, 0);
				copy.Apply();
				File.WriteAllBytes(Path.Combine(textureDir, file), copy.EncodeToPNG());
				return file;
			}
			catch (Exception)
			{
				return null;
			}
			finally
			{
				RenderTexture.active = previous;
				RenderTexture.ReleaseTemporary(rt);
				if (copy != null)
				{
					Object.Destroy(copy);
				}
			}
		}

		// --- hierarchy.txt ---

		private static string Hierarchy(GameObject vehicle)
		{
			StringBuilder sb = new StringBuilder();
			Transform root = vehicle.transform;
			sb.AppendLine("# " + vehicle.name + " at " + V(root.position) + ", rotation " + V(root.eulerAngles) + ", active " + vehicle.activeInHierarchy);
			sb.AppendLine("# Positions after '@' are in the vehicle's frame (Unity: x right, y up, z forward). Bounds are vehicle-frame boxes.");
			sb.AppendLine();
			Walk(root, root, 0, sb);
			sb.AppendLine();
			sb.AppendLine("# ---- Vehicle script fields ----");
			foreach (MonoBehaviour b in vehicle.GetComponents<MonoBehaviour>())
			{
				if (b != null)
				{
					Fields(b, root, sb);
				}
			}
			return sb.ToString();
		}

		private static void Walk(Transform t, Transform root, int depth, StringBuilder sb)
		{
			string pad = new string(' ', depth * 2);
			Vector3 p = root.InverseTransformPoint(t.position);
			Quaternion q = Quaternion.Inverse(root.rotation) * t.rotation;
			sb.Append(pad).Append(t.name).Append(t.gameObject.activeSelf ? "" : " [inactive]")
				.Append(" @ ").Append(V(p)).Append(" rot ").Append(V(q.eulerAngles)).Append(" scale ").Append(V(t.lossyScale))
				.Append(" layer ").Append(LayerMask.LayerToName(t.gameObject.layer)).Append(t.tag != "Untagged" ? " tag " + t.tag : "").Append('\n');
			foreach (Component c in t.GetComponents<Component>())
			{
				if (c == null || c is Transform)
				{
					continue;
				}
				sb.Append(pad).Append("  - ").Append(Describe(c, root)).Append('\n');
			}
			foreach (Transform child in t)
			{
				Walk(child, root, depth + 1, sb);
			}
		}

		private static string Describe(Component c, Transform root)
		{
			string name = c.GetType().Name;
			try
			{
				if (c is MeshFilter)
				{
					Mesh m = ((MeshFilter)c).sharedMesh;
					return m == null ? name + " (none)" : name + " " + m.name + " verts " + m.vertexCount + " readable " + m.isReadable + " bounds " + B(BoundsIn(root, c.transform, m.bounds));
				}
				if (c is Renderer)
				{
					Renderer r = (Renderer)c;
					List<string> mats = new List<string>();
					foreach (Material m in r.sharedMaterials)
					{
						mats.Add(m == null ? "null" : m.name);
					}
					return name + (r.enabled ? "" : " [disabled]") + " materials [" + string.Join(", ", mats.ToArray()) + "]";
				}
				if (c is WheelCollider)
				{
					WheelCollider w = (WheelCollider)c;
					return name + " radius " + F(w.radius) + " suspension " + F(w.suspensionDistance) + " center " + V(w.center) + " mass " + F(w.mass);
				}
				if (c is BoxCollider)
				{
					BoxCollider b = (BoxCollider)c;
					return name + (b.enabled ? "" : " [disabled]") + (b.isTrigger ? " trigger" : "") + " center " + V(b.center) + " size " + V(b.size);
				}
				if (c is SphereCollider)
				{
					SphereCollider s = (SphereCollider)c;
					return name + (s.enabled ? "" : " [disabled]") + (s.isTrigger ? " trigger" : "") + " center " + V(s.center) + " radius " + F(s.radius);
				}
				if (c is CapsuleCollider)
				{
					CapsuleCollider s = (CapsuleCollider)c;
					return name + (s.enabled ? "" : " [disabled]") + (s.isTrigger ? " trigger" : "") + " center " + V(s.center) + " radius " + F(s.radius) + " height " + F(s.height);
				}
				if (c is MeshCollider)
				{
					MeshCollider s = (MeshCollider)c;
					return name + (s.enabled ? "" : " [disabled]") + (s.isTrigger ? " trigger" : "") + (s.convex ? " convex" : "") + " mesh " + (s.sharedMesh != null ? s.sharedMesh.name : "none");
				}
				if (c is Rigidbody)
				{
					Rigidbody r = (Rigidbody)c;
					return name + " mass " + F(r.mass) + (r.isKinematic ? " kinematic" : "") + " drag " + F(r.drag) + " com " + V(r.centerOfMass);
				}
				if (c is HingeJoint)
				{
					HingeJoint h = (HingeJoint)c;
					return name + " to " + (h.connectedBody != null ? h.connectedBody.name : "world") + " anchor " + V(h.anchor) + " axis " + V(h.axis) + " limits " + (h.useLimits ? F(h.limits.min) + ".." + F(h.limits.max) : "off") + " motor " + (h.useMotor ? F(h.motor.targetVelocity) : "off");
				}
				if (c is Joint)
				{
					Joint j = (Joint)c;
					return name + " to " + (j.connectedBody != null ? j.connectedBody.name : "world") + " anchor " + V(j.anchor);
				}
				if (c is Light)
				{
					Light l = (Light)c;
					return name + " " + l.type + (l.enabled ? "" : " [disabled]") + " range " + F(l.range);
				}
				if (c is AudioSource)
				{
					AudioSource a = (AudioSource)c;
					return name + " " + (a.clip != null ? a.clip.name : "no clip");
				}
				if (c is Behaviour && !((Behaviour)c).enabled)
				{
					return name + " [disabled]";
				}
			}
			catch (Exception e)
			{
				return name + " (" + e.Message + ")";
			}
			return name;
		}

		// The vehicle script's settings and what its object fields point at.
		private static void Fields(MonoBehaviour b, Transform root, StringBuilder sb)
		{
			sb.AppendLine("[" + b.GetType().Name + "]");
			foreach (FieldInfo f in b.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				object v;
				try
				{
					v = f.GetValue(b);
				}
				catch
				{
					continue;
				}
				string text = Value(v, root);
				if (text != null)
				{
					sb.AppendLine("  " + f.Name + " = " + text);
				}
			}
		}

		private static string Value(object v, Transform root)
		{
			if (v == null)
			{
				return "null";
			}
			if (v is Object)
			{
				Object o = (Object)v;
				if (o == null)
				{
					return "null (destroyed)";
				}
				Transform t = o is Component ? ((Component)o).transform : o is GameObject ? ((GameObject)o).transform : null;
				if (t == null)
				{
					return o.GetType().Name + " " + o.name;
				}
				return o.GetType().Name + " " + (t.IsChildOf(root) ? PathOf(root, t) : "(outside) " + t.name);
			}
			if (v is IList && !(v is string))
			{
				IList list = (IList)v;
				List<string> items = new List<string>();
				for (int i = 0; i < list.Count && i < 40; i++)
				{
					items.Add(Value(list[i], root) ?? "?");
				}
				return "[" + string.Join("; ", items.ToArray()) + (list.Count > 40 ? "; ..." : "") + "]";
			}
			if (v is float)
			{
				return F((float)v);
			}
			if (v is bool || v is int || v is string || v is Enum)
			{
				return v.ToString();
			}
			if (v is Vector3)
			{
				return V((Vector3)v);
			}
			if (v is Color)
			{
				Color c = (Color)v;
				return F(c.r) + " " + F(c.g) + " " + F(c.b) + " " + F(c.a);
			}
			return null;
		}

		// --- Helpers. ---

		private static Bounds BoundsIn(Transform root, Transform t, Bounds local)
		{
			Bounds b = new Bounds(root.InverseTransformPoint(t.TransformPoint(local.center)), Vector3.zero);
			for (int i = 0; i < 8; i++)
			{
				Vector3 corner = local.center + Vector3.Scale(local.extents, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
				b.Encapsulate(root.InverseTransformPoint(t.TransformPoint(corner)));
			}
			return b;
		}

		public static string PathOf(Transform root, Transform t)
		{
			List<string> parts = new List<string>();
			for (; t != null && t != root; t = t.parent)
			{
				parts.Insert(0, t.name);
			}
			return parts.Count == 0 ? "(root)" : string.Join("/", parts.ToArray());
		}

		public static string Clean(string s)
		{
			StringBuilder sb = new StringBuilder();
			foreach (char ch in s)
			{
				sb.Append(char.IsLetterOrDigit(ch) || ch == '/' || ch == '.' || ch == '-' ? ch : '_');
			}
			return sb.Length > 0 ? sb.ToString() : "part";
		}

		private static string F(float v)
		{
			return v.ToString("0.#####", Inv);
		}

		private static string V(Vector3 v)
		{
			return "(" + F(v.x) + ", " + F(v.y) + ", " + F(v.z) + ")";
		}

		private static string B(Bounds b)
		{
			return "min " + V(b.min) + " max " + V(b.max);
		}
	}
}
