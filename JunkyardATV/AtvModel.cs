using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JunkyardATV
{
	// Builds the ATV's look and works out where everything goes.
	//
	// The model is loaded (or the placeholder built), turned/scaled/moved per the
	// config, then measured: parts whose names contain a wheel word are grouped by
	// corner (front/back x left/right) under one pivot each, so each wheel spins and
	// steers about its own center; the rest is the body. Wheel colliders, the body
	// collider, the seat, engine and fuel inlet are placed from those measurements.
	internal class AtvLayout
	{
		public Transform visual;
		public Transform[] wheels = new Transform[4]; // FL, FR, RL, RR (null = none)
		public Vector3[] wheelCenters = new Vector3[4]; // vehicle-local
		public float wheelRadius = 0.29f;
		public Bounds body; // vehicle-local
		public Vector3 seat;
		public Vector3 engine;
		public Vector3 fuelInlet;
		public Transform handlebars;
	}

	internal static class AtvModel
	{
		public const int FL = 0, FR = 1, RL = 2, RR = 3;

		private static ModelData cached;
		private static string cachedPath;
		private static DateTime cachedTime;

		// Parse the model file once and reuse it until the file changes.
		private static ModelData Data(AtvConfig cfg)
		{
			string path = cfg.ModelPath;
			if (path == null)
			{
				return null;
			}
			if (!System.IO.File.Exists(path))
			{
				AtvMod.Log("Model file not found: " + path + ". Using the placeholder.");
				return null;
			}
			DateTime time = System.IO.File.GetLastWriteTimeUtc(path);
			if (cached != null && cachedPath == path && cachedTime == time)
			{
				return cached;
			}
			try
			{
				ModelData data = ModelData.Load(path);
				AtvMod.Log("Loaded model " + cfg.model + ": " + data.CountMeshes() + " meshes, " + data.materials.Count + " materials.");
				cached = data;
				cachedPath = path;
				cachedTime = time;
				return data;
			}
			catch (Exception e)
			{
				AtvMod.Log("Couldn't load " + cfg.model + ": " + e.Message + ". Using the placeholder.");
				return null;
			}
		}

		public static AtvLayout Build(Transform vehicle, AtvConfig cfg)
		{
			AtvLayout layout = new AtvLayout();
			ModelData data = Data(cfg);
			GameObject model = data != null ? ModelBuilder.Build(data, "Model") : Placeholder();
			model.transform.SetParent(vehicle, false);
			model.transform.localPosition = cfg.offset;
			model.transform.localRotation = Quaternion.Euler(cfg.rotation);
			model.transform.localScale = Vector3.one * cfg.scale;
			layout.visual = model.transform;
			foreach (string name in cfg.HideList())
			{
				Transform t = FindDeep(model.transform, name);
				if (t != null)
				{
					t.gameObject.SetActive(false);
				}
			}

			// Group wheel parts by corner.
			List<string> words = cfg.WheelNameList();
			List<Renderer> bodyParts = new List<Renderer>();
			List<Renderer> wheelParts = new List<Renderer>();
			foreach (Renderer r in model.GetComponentsInChildren<Renderer>())
			{
				(IsWheel(r.transform, model.transform, words) ? wheelParts : bodyParts).Add(r);
			}
			Bounds all = LocalBounds(vehicle, model.GetComponentsInChildren<Renderer>());
			layout.body = bodyParts.Count > 0 ? LocalBounds(vehicle, bodyParts.ToArray()) : all;
			List<Renderer>[] corners = { new List<Renderer>(), new List<Renderer>(), new List<Renderer>(), new List<Renderer>() };
			foreach (Renderer r in wheelParts)
			{
				Vector3 c = vehicle.InverseTransformPoint(r.bounds.center);
				bool front = c.z >= all.center.z;
				bool left = c.x < all.center.x;
				corners[front ? (left ? FL : FR) : (left ? RL : RR)].Add(r);
			}
			bool haveWheels = corners[FL].Count > 0 && corners[FR].Count > 0 && corners[RL].Count > 0 && corners[RR].Count > 0;
			float radius = 0f;
			if (haveWheels)
			{
				for (int i = 0; i < 4; i++)
				{
					Bounds b = LocalBounds(vehicle, corners[i].ToArray());
					GameObject pivot = new GameObject(new[] { "WheelFL", "WheelFR", "WheelRL", "WheelRR" }[i]);
					pivot.transform.SetParent(vehicle, false);
					pivot.transform.localPosition = b.center;
					// Re-parent this corner's parts (or their wheel-named parent) to it.
					HashSet<Transform> moved = new HashSet<Transform>();
					foreach (Renderer r in corners[i])
					{
						Transform t = WheelRoot(r.transform, model.transform, words);
						if (moved.Add(t))
						{
							t.SetParent(pivot.transform, true);
						}
					}
					layout.wheels[i] = pivot.transform;
					layout.wheelCenters[i] = b.center;
					radius = Mathf.Max(radius, b.extents.y);
				}
			}
			else
			{
				// No wheels found: guess the corners from the overall size. The
				// model's own wheels (if any) just won't turn.
				if (wheelParts.Count > 0)
				{
					AtvMod.Log("Found wheel parts, but not one at each corner; using estimated wheel positions.");
				}
				Bounds b = all;
				radius = Mathf.Clamp(b.size.y * 0.25f, 0.2f, 0.4f);
				float x = Mathf.Max(0.3f, b.extents.x - radius * 0.5f);
				float z = Mathf.Max(0.4f, b.extents.z - radius * 1.1f);
				float y = b.min.y + radius;
				layout.wheelCenters[FL] = new Vector3(b.center.x - x, y, b.center.z + z);
				layout.wheelCenters[FR] = new Vector3(b.center.x + x, y, b.center.z + z);
				layout.wheelCenters[RL] = new Vector3(b.center.x - x, y, b.center.z - z);
				layout.wheelCenters[RR] = new Vector3(b.center.x + x, y, b.center.z - z);
			}
			layout.wheelRadius = cfg.wheelRadius > 0f ? cfg.wheelRadius : Mathf.Clamp(radius, 0.12f, 0.6f);

			Bounds bd = layout.body;
			layout.seat = AtvConfig.IsAuto(cfg.seat) ? new Vector3(bd.center.x, bd.max.y, bd.center.z - bd.extents.z * 0.2f) : cfg.seat;
			layout.engine = AtvConfig.IsAuto(cfg.engine) ? new Vector3(bd.center.x, bd.min.y + bd.size.y * 0.4f, bd.center.z) : cfg.engine;
			layout.fuelInlet = AtvConfig.IsAuto(cfg.fuelInlet) ? new Vector3(bd.center.x, bd.max.y, bd.center.z + bd.extents.z * 0.25f) : cfg.fuelInlet;
			if (cfg.handlebars.Length > 0)
			{
				layout.handlebars = FindDeep(model.transform, cfg.handlebars);
			}
			if (layout.handlebars == null)
			{
				layout.handlebars = FindDeep(model.transform, "Handlebars");
			}
			return layout;
		}

		private static bool IsWheel(Transform t, Transform stop, List<string> words)
		{
			return WheelRoot(t, stop, words) != null;
		}

		// The highest ancestor (below the model root) whose name has a wheel word:
		// a "Wheel_FL" group with "rim" and "tire" children moves as one.
		private static Transform WheelRoot(Transform t, Transform stop, List<string> words)
		{
			Transform found = null;
			for (; t != null && t != stop; t = t.parent)
			{
				string name = t.name.ToLowerInvariant();
				foreach (string w in words)
				{
					if (name.Contains(w))
					{
						found = t;
						break;
					}
				}
			}
			return found;
		}

		private static Bounds LocalBounds(Transform frame, Renderer[] renderers)
		{
			bool first = true;
			Bounds b = new Bounds();
			foreach (Renderer r in renderers)
			{
				if (!r.enabled || !r.gameObject.activeInHierarchy)
				{
					continue;
				}
				MeshFilter mf = r.GetComponent<MeshFilter>();
				if (mf == null || mf.sharedMesh == null)
				{
					continue;
				}
				// The mesh's own box, corner by corner into the vehicle's frame.
				Bounds mb = mf.sharedMesh.bounds;
				for (int i = 0; i < 8; i++)
				{
					Vector3 corner = mb.center + Vector3.Scale(mb.extents, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
					Vector3 p = frame.InverseTransformPoint(r.transform.TransformPoint(corner));
					if (first)
					{
						b = new Bounds(p, Vector3.zero);
						first = false;
					}
					else
					{
						b.Encapsulate(p);
					}
				}
			}
			return first ? new Bounds(new Vector3(0f, 0.5f, 0f), new Vector3(1.1f, 0.8f, 1.9f)) : b;
		}

		public static Transform FindDeep(Transform root, string name)
		{
			foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
			{
				if (string.Equals(t.name, name, StringComparison.OrdinalIgnoreCase))
				{
					return t;
				}
			}
			return null;
		}

		// --- Built-in placeholder: a simple sport quad built from primitives. ---

		private static GameObject Placeholder()
		{
			GameObject root = new GameObject("Model");
			Color red = new Color(0.7f, 0.08f, 0.06f);
			Color black = new Color(0.06f, 0.06f, 0.06f);
			Color grey = new Color(0.35f, 0.35f, 0.37f);
			Box(root, "Frame", new Vector3(0.45f, 0.18f, 1.35f), new Vector3(0f, 0.38f, 0f), grey);
			Box(root, "Tank", new Vector3(0.42f, 0.22f, 0.4f), new Vector3(0f, 0.68f, 0.2f), red);
			Box(root, "Seat", new Vector3(0.36f, 0.1f, 0.62f), new Vector3(0f, 0.75f, -0.32f), black);
			Box(root, "FenderFront", new Vector3(1.05f, 0.06f, 0.55f), new Vector3(0f, 0.66f, 0.6f), red);
			Box(root, "FenderRear", new Vector3(1.05f, 0.06f, 0.6f), new Vector3(0f, 0.66f, -0.6f), red);
			Box(root, "BumperFront", new Vector3(0.6f, 0.06f, 0.08f), new Vector3(0f, 0.45f, 0.92f), grey);
			Box(root, "Footrests", new Vector3(1.0f, 0.04f, 0.32f), new Vector3(0f, 0.33f, 0f), grey);
			GameObject bars = new GameObject("Handlebars");
			bars.transform.SetParent(root.transform, false);
			bars.transform.localPosition = new Vector3(0f, 0.86f, 0.42f);
			Box(bars, "Stem", new Vector3(0.05f, 0.22f, 0.05f), new Vector3(0f, -0.08f, 0f), grey);
			Box(bars, "Bar", new Vector3(0.72f, 0.035f, 0.035f), new Vector3(0f, 0.03f, 0f), black);
			string[] names = { "Wheel_FL", "Wheel_FR", "Wheel_RL", "Wheel_RR" };
			Vector3[] at = { new Vector3(-0.48f, 0.29f, 0.62f), new Vector3(0.48f, 0.29f, 0.62f), new Vector3(-0.5f, 0.29f, -0.6f), new Vector3(0.5f, 0.29f, -0.6f) };
			for (int i = 0; i < 4; i++)
			{
				GameObject wheel = new GameObject(names[i]);
				wheel.transform.SetParent(root.transform, false);
				wheel.transform.localPosition = at[i];
				Cylinder(wheel, "Tire", 0.58f, 0.24f, black);
				Cylinder(wheel, "Rim", 0.34f, 0.25f, grey);
				// Tread blocks, so you can see it roll.
				for (int k = 0; k < 8; k++)
				{
					float a = k * 45f;
					GameObject lug = Box(wheel, "Lug" + k, new Vector3(0.25f, 0.05f, 0.05f), Quaternion.Euler(a, 0f, 0f) * new Vector3(0f, 0.28f, 0f), black);
					lug.transform.localRotation = Quaternion.Euler(a, 0f, 0f);
				}
			}
			return root;
		}

		private static GameObject Box(GameObject parent, string name, Vector3 size, Vector3 position, Color color)
		{
			GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
			go.name = name;
			Object.DestroyImmediate(go.GetComponent<Collider>());
			go.transform.SetParent(parent.transform, false);
			go.transform.localPosition = position;
			go.transform.localScale = size;
			go.GetComponent<Renderer>().material.color = color;
			return go;
		}

		// Unity's cylinder is 1 wide and 2 tall along Y; turned to lie along X.
		private static GameObject Cylinder(GameObject parent, string name, float diameter, float width, Color color)
		{
			GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			go.name = name;
			Object.DestroyImmediate(go.GetComponent<Collider>());
			go.transform.SetParent(parent.transform, false);
			go.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
			go.transform.localScale = new Vector3(diameter, width / 2f, diameter);
			go.GetComponent<Renderer>().material.color = color;
			return go;
		}
	}
}
