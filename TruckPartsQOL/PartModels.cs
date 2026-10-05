using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TruckPartsQOL
{
	// Real 3D models for parts that have one: <game>/UserData/TruckPartsQOL/models/
	// speaker.obj (6.5" door speaker) and amp.obj (4-channel amplifier), made by
	// tools/convert_audio_models.py. Each is loaded once into a hidden prototype;
	// parts get copies of it, which share its meshes and textures. Parts whose
	// model isn't there keep their built-in look.
	internal static class PartModels
	{
		private static GameObject holder;
		private static readonly Dictionary<string, GameObject> prototypes = new Dictionary<string, GameObject>();
		private static readonly HashSet<string> failed = new HashSet<string>();

		public static string Folder
		{
			get { return Path.Combine(Path.Combine(Path.Combine(Path.GetDirectoryName(Application.dataPath), "UserData"), "TruckPartsQOL"), "models"); }
		}

		// A copy of the model under `parent` and its size (the bounding box of
		// the model, which is centered on its origin), or null without a model.
		public static GameObject Attach(string file, Transform parent, out Vector3 size)
		{
			size = Vector3.zero;
			try
			{
				GameObject prototype = Prototype(file);
				if (prototype == null)
				{
					return null;
				}
				GameObject copy = Object.Instantiate(prototype, parent, false);
				copy.name = "Model";
				copy.SetActive(true);
				size = Size(prototype);
				return copy;
			}
			catch (Exception e)
			{
				// Whatever goes wrong, the part still gets made with its built-in look.
				failed.Add(file);
				TruckPartsQOLMod.Log("Couldn't use the " + file + " model, using the built-in look: " + e);
				size = Vector3.zero;
				return null;
			}
		}

		private static GameObject Prototype(string file)
		{
			GameObject prototype;
			if (prototypes.TryGetValue(file, out prototype) && prototype != null)
			{
				return prototype;
			}
			if (failed.Contains(file))
			{
				return null;
			}
			string path = Path.Combine(Folder, file);
			if (!File.Exists(path))
			{
				failed.Add(file);
				TruckPartsQOLMod.Log("No " + file + " in " + Folder + "; that part uses its built-in look.");
				return null;
			}
			try
			{
				if (holder == null)
				{
					holder = new GameObject("TruckPartsQOL_Models");
					holder.SetActive(false);
					Object.DontDestroyOnLoad(holder);
				}
				ModelData data = ModelData.Load(path);
				prototype = ModelBuilder.Build(data, Path.GetFileNameWithoutExtension(file));
				prototype.transform.SetParent(holder.transform, false);
				// No colliders: the part's own box collider covers it.
				foreach (Collider c in prototype.GetComponentsInChildren<Collider>(true))
				{
					Object.DestroyImmediate(c);
				}
				prototypes[file] = prototype;
				TruckPartsQOLMod.Log("Loaded the " + file + " model.");
				return prototype;
			}
			catch (Exception e)
			{
				failed.Add(file);
				TruckPartsQOLMod.Log("Couldn't load " + file + ", using the built-in look: " + e.Message);
				return null;
			}
		}

		private static Vector3 Size(GameObject prototype)
		{
			bool any = false;
			Bounds b = new Bounds();
			foreach (MeshFilter f in prototype.GetComponentsInChildren<MeshFilter>(true))
			{
				if (f.sharedMesh == null)
				{
					continue;
				}
				Bounds m = f.sharedMesh.bounds;
				for (int i = 0; i < 8; i++)
				{
					Vector3 corner = m.center + Vector3.Scale(m.extents, new Vector3((i & 1) == 0 ? -1f : 1f, (i & 2) == 0 ? -1f : 1f, (i & 4) == 0 ? -1f : 1f));
					Vector3 p = prototype.transform.InverseTransformPoint(f.transform.TransformPoint(corner));
					if (!any)
					{
						b = new Bounds(p, Vector3.zero);
						any = true;
					}
					else
					{
						b.Encapsulate(p);
					}
				}
			}
			return b.size;
		}
	}
}
