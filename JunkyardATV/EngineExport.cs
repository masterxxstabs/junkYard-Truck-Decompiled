using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace JunkyardATV
{
	// Fit mode, F6: writes an ATV's 250 engine, as placed, to
	// <game>/JunkyardATV/engine250_export.obj (+ .mtl), in the same frame as the
	// rigged model (meters, Y up, front +Z, right-handed like quadzilla.obj), so
	// the two can be loaded together in Blender or the rig tools to work on the fit.
	internal static class EngineExport
	{
		private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

		public static string Write(Transform vehicle, Engine250 engine)
		{
			if (engine == null)
			{
				return "This ATV has no engine to export.";
			}
			string objPath = Path.Combine(AtvConfig.Folder, "engine250_export.obj");
			string mtlName = "engine250_export.mtl";
			StringBuilder obj = new StringBuilder();
			StringBuilder mtl = new StringBuilder();
			obj.AppendLine("# Junkyard Truck 250 engine as placed on the ATV (Junkyard ATV fit mode export)");
			obj.AppendLine("# Meters, Y up, front +Z, same frame as quadzilla.obj");
			obj.AppendLine("mtllib " + mtlName);
			Dictionary<string, string> materials = new Dictionary<string, string>();
			int baseIndex = 1;
			int parts = 0;
			int boxes = 0;
			foreach (MeshFilter filter in engine.GetComponentsInChildren<MeshFilter>(true))
			{
				Renderer r = filter.GetComponent<Renderer>();
				Mesh mesh = filter.sharedMesh;
				if (mesh == null || r == null || !r.enabled || !filter.gameObject.activeInHierarchy)
				{
					continue;
				}
				string mat = Material(r, materials, mtl);
				obj.AppendLine("o " + Clean(filter.gameObject.name) + "_" + parts);
				obj.AppendLine("usemtl " + mat);
				Vector3[] verts = null;
				int[] tris = null;
				if (mesh.isReadable)
				{
					try
					{
						verts = mesh.vertices;
						tris = mesh.triangles;
					}
					catch (Exception)
					{
						verts = null;
					}
				}
				if (verts == null || verts.Length == 0 || tris == null || tris.Length == 0)
				{
					// The game didn't keep this mesh readable: its box stands in.
					Bounds b = mesh.bounds;
					verts = new Vector3[8];
					for (int i = 0; i < 8; i++)
					{
						verts[i] = b.center + Vector3.Scale(b.extents, new Vector3((i & 1) == 0 ? -1f : 1f, (i & 2) == 0 ? -1f : 1f, (i & 4) == 0 ? -1f : 1f));
					}
					tris = new int[] { 0, 2, 1, 1, 2, 3, 4, 5, 6, 5, 7, 6, 0, 1, 4, 1, 5, 4, 2, 6, 3, 3, 6, 7, 0, 4, 2, 2, 4, 6, 1, 3, 5, 3, 7, 5 };
					boxes++;
				}
				foreach (Vector3 v in verts)
				{
					Vector3 p = vehicle.InverseTransformPoint(filter.transform.TransformPoint(v));
					// Unity is left-handed; mirror X like the model loader does in reverse.
					obj.Append("v ").Append((-p.x).ToString("0.#####", Inv)).Append(' ').Append(p.y.ToString("0.#####", Inv)).Append(' ').Append(p.z.ToString("0.#####", Inv)).AppendLine();
				}
				for (int i = 0; i + 2 < tris.Length; i += 3)
				{
					// Mirroring flips the winding back.
					obj.Append("f ").Append(baseIndex + tris[i]).Append(' ').Append(baseIndex + tris[i + 2]).Append(' ').Append(baseIndex + tris[i + 1]).AppendLine();
				}
				baseIndex += verts.Length;
				parts++;
			}
			try
			{
				Directory.CreateDirectory(AtvConfig.Folder);
				File.WriteAllText(objPath, obj.ToString());
				File.WriteAllText(Path.Combine(AtvConfig.Folder, mtlName), mtl.ToString());
			}
			catch (Exception e)
			{
				return "Couldn't write the engine export: " + e.Message;
			}
			string msg = "Exported the engine (" + parts + " parts) to " + objPath + ".";
			if (boxes > 0)
			{
				msg += " " + boxes + " parts weren't readable and went in as boxes.";
			}
			return msg;
		}

		private static string Material(Renderer r, Dictionary<string, string> materials, StringBuilder mtl)
		{
			Material m = r.sharedMaterial;
			Color c = m != null && m.HasProperty("_Color") ? m.color : Color.grey;
			string key = m != null ? m.name : "none";
			string name;
			if (materials.TryGetValue(key, out name))
			{
				return name;
			}
			name = "m" + materials.Count + "_" + Clean(key);
			materials[key] = name;
			mtl.AppendLine("newmtl " + name);
			mtl.AppendLine("Kd " + c.r.ToString("0.###", Inv) + " " + c.g.ToString("0.###", Inv) + " " + c.b.ToString("0.###", Inv));
			mtl.AppendLine("d 1");
			mtl.AppendLine();
			return name;
		}

		private static string Clean(string s)
		{
			StringBuilder sb = new StringBuilder();
			foreach (char ch in s)
			{
				sb.Append(char.IsLetterOrDigit(ch) ? ch : '_');
			}
			return sb.Length > 0 ? sb.ToString() : "part";
		}
	}
}
