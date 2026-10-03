using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JunkyardATV
{
	// The ATV runs on a real copy of the game's 250 engine (the dirt bike's
	// Engine250 block): all its parts can wear, break and be swapped, and its fuel,
	// oil and condition work exactly as on the bike.
	//
	// Engine250 is tied to its bike in a few places, which the copy is cut loose
	// from: its FixedJoint and Rigidbody go (the block becomes part of the ATV),
	// it can't be picked up, and its `db` (the Dirtbike whose powerDivision
	// Refresh() writes) points at a dormant stand-in on the ATV instead of the real
	// bike. The ATV reads that powerDivision as its own power loss.
	internal static class AtvEngine
	{
		private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

		private static GameObject holder;
		private static GameObject template;
		// The template's meshes in its own space, every part fitted: Place() sits
		// the engine on its mount by this box, whatever the block's pivot is.
		private static Bounds shape;

		// A factory-fresh 250 engine, made once per level from the game's own.
		private static GameObject Template()
		{
			if (template != null)
			{
				return template;
			}
			Engine250 source = null;
			foreach (Engine250 e in Resources.FindObjectsOfTypeAll<Engine250>())
			{
				if (e == null || !e.gameObject.scene.IsValid() || e.GetComponentInParent<AtvVehicle>() != null)
				{
					continue;
				}
				if (source == null || e.gameObject.name == "250_block")
				{
					source = e;
				}
			}
			if (source == null)
			{
				AtvMod.Log("No 250 engine in this level to copy; ATVs will have no engine.");
				return null;
			}
			holder = new GameObject("JunkyardATV_EngineTemplate");
			holder.SetActive(false);
			GameObject copy = Object.Instantiate(source.gameObject, holder.transform);
			// Not "250_block": the game finds the bike's engine by that name.
			copy.name = "250_block_atv";
			copy.transform.localPosition = Vector3.zero;
			copy.transform.localRotation = Quaternion.identity;
			FixedJoint joint = copy.GetComponent<FixedJoint>();
			if (joint != null)
			{
				Object.DestroyImmediate(joint);
			}
			Rigidbody body = copy.GetComponent<Rigidbody>();
			if (body != null)
			{
				Object.DestroyImmediate(body);
			}
			PickUp pick = copy.GetComponent<PickUp>();
			if (pick != null)
			{
				pick.pickable = false;
				pick.price = 0f;
			}
			Engine250 engine = copy.GetComponent<Engine250>();
			MakeNew(engine);
			shape = MeshBox(copy.transform);
			Vector3 size = Vector3.Scale(shape.size, copy.transform.localScale);
			AtvMod.Log("The 250 engine measures " + size.x.ToString("0.00", Inv) + " wide x " + size.y.ToString("0.00", Inv) + " tall x " + size.z.ToString("0.00", Inv) + " long (m).");
			template = copy;
			return template;
		}

		// A new engine for an ATV, mounted on `mount`.
		public static Engine250 Fit(Transform mount, Transform vehicle)
		{
			GameObject t = Template();
			if (t == null)
			{
				return null;
			}
			GameObject copy = Object.Instantiate(t, mount, false);
			copy.name = t.name;
			copy.transform.localPosition = Vector3.zero;
			copy.transform.localRotation = Quaternion.identity;
			Engine250 engine = copy.GetComponent<Engine250>();
			// The stand-in for the dirt bike Refresh() writes powerDivision into.
			GameObject state = new GameObject("PowerState");
			state.SetActive(false);
			state.transform.SetParent(vehicle, false);
			engine.db = state.AddComponent<Dirtbike>();
			engine.db.powerDivision = 1;
			// (Not ShowBolts(): its bolts belong to the dirt bike's frame.)
			return engine;
		}

		// The box around root's visible meshes, in root's space. Mesh bounds, not
		// Renderer.bounds: those are world boxes, and wrong while inactive. Particle
		// effects (exhaust smoke) have no MeshFilter and are left out.
		private static Bounds MeshBox(Transform root)
		{
			bool any = false;
			Bounds box = new Bounds();
			foreach (MeshFilter filter in root.GetComponentsInChildren<MeshFilter>(true))
			{
				Renderer r = filter.GetComponent<Renderer>();
				if (filter.sharedMesh == null || r == null || !r.enabled)
				{
					continue;
				}
				Bounds b = filter.sharedMesh.bounds;
				for (int i = 0; i < 8; i++)
				{
					Vector3 corner = b.center + Vector3.Scale(b.extents, new Vector3((i & 1) == 0 ? -1f : 1f, (i & 2) == 0 ? -1f : 1f, (i & 4) == 0 ? -1f : 1f));
					Vector3 p = root.InverseTransformPoint(filter.transform.TransformPoint(corner));
					if (!any)
					{
						box = new Bounds(p, Vector3.zero);
						any = true;
					}
					else
					{
						box.Encapsulate(p);
					}
				}
			}
			return box;
		}

		// Turn and size the engine on its mount, then sit it so the bottom middle
		// of its box is at the mount point (atv.cfg's engine= is that spot).
		public static void Place(Engine250 engine, Vector3 rotation, float scale)
		{
			if (engine == null || template == null)
			{
				return;
			}
			Vector3 min, max;
			TurnedBox(rotation, scale, out min, out max);
			Transform t = engine.transform;
			t.localRotation = Quaternion.Euler(rotation);
			t.localScale = template.transform.localScale * Mathf.Max(0.01f, scale);
			t.localPosition = -new Vector3((min.x + max.x) / 2f, min.y, (min.z + max.z) / 2f);
		}

		// The engine's size on the ATV as placed (meters).
		public static Vector3 PlacedSize(Vector3 rotation, float scale)
		{
			if (template == null)
			{
				return Vector3.zero;
			}
			Vector3 min, max;
			TurnedBox(rotation, scale, out min, out max);
			return max - min;
		}

		// The engine's box once turned and sized, in its mount's space.
		private static void TurnedBox(Vector3 rotation, float scale, out Vector3 min, out Vector3 max)
		{
			Quaternion turn = Quaternion.Euler(rotation);
			Vector3 size = template.transform.localScale * Mathf.Max(0.01f, scale);
			min = Vector3.one * float.MaxValue;
			max = Vector3.one * float.MinValue;
			for (int i = 0; i < 8; i++)
			{
				Vector3 corner = shape.center + Vector3.Scale(shape.extents, new Vector3((i & 1) == 0 ? -1f : 1f, (i & 2) == 0 ? -1f : 1f, (i & 4) == 0 ? -1f : 1f));
				Vector3 p = turn * Vector3.Scale(corner, size);
				min = Vector3.Min(min, p);
				max = Vector3.Max(max, p);
			}
		}

		// Every part fitted and at 100%, fluids full, a little fuel in the tank.
		private static void MakeNew(Engine250 engine)
		{
			foreach (FieldInfo f in PartFields(engine))
			{
				durability part = f.GetValue(engine) as durability;
				if (part == null)
				{
					continue;
				}
				part.health = 100f;
				SetFitted(part, true);
				// The condition the engine itself tracks for the part (addPart keeps
				// it; Refresh only re-reads some): new, not whatever the bike had.
				SetFloat(engine, CndName(f), 100f);
			}
			SetFloat(engine, "newOilLevel", 100f);
			SetFloat(engine, "newCoolantLevel", 100f);
			SetFloat(engine, "transFluid", 100f);
			SetFloat(engine, "newFuelLevel", 250f);
			SetFloat(engine, "tempIncrease", 0f);
		}

		private static void SetFitted(durability part, bool fitted)
		{
			Renderer r = part.GetComponent<Renderer>();
			if (r != null)
			{
				r.enabled = fitted;
			}
			foreach (Collider c in part.GetComponents<Collider>())
			{
				if (!c.isTrigger)
				{
					c.enabled = fitted;
				}
			}
		}

		private static readonly Dictionary<Type, List<FieldInfo>> partFields = new Dictionary<Type, List<FieldInfo>>();

		private static List<FieldInfo> PartFields(Engine250 engine)
		{
			Type type = engine.GetType();
			List<FieldInfo> fields;
			if (!partFields.TryGetValue(type, out fields))
			{
				fields = new List<FieldInfo>();
				foreach (FieldInfo f in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (f.Name.EndsWith("_cnd_c", StringComparison.Ordinal) && typeof(durability).IsAssignableFrom(f.FieldType))
					{
						fields.Add(f);
					}
				}
				partFields[type] = fields;
			}
			return fields;
		}

		// "fuel,oil,coolant,trans|part=health:1,part=health:0,..."
		public static string Save(Engine250 engine)
		{
			List<string> parts = new List<string>();
			foreach (FieldInfo f in PartFields(engine))
			{
				durability part = f.GetValue(engine) as durability;
				if (part == null)
				{
					continue;
				}
				Renderer r = part.GetComponent<Renderer>();
				parts.Add(f.Name + "=" + part.health.ToString("R", Inv) + ":" + (r == null || r.enabled ? "1" : "0") + ":" + GetFloat(engine, CndName(f)).ToString("R", Inv));
			}
			return string.Join(",", new[]
			{
				GetFloat(engine, "newFuelLevel").ToString("R", Inv),
				GetFloat(engine, "newOilLevel").ToString("R", Inv),
				GetFloat(engine, "newCoolantLevel").ToString("R", Inv),
				GetFloat(engine, "transFluid").ToString("R", Inv)
			}) + "|" + string.Join(",", parts.ToArray());
		}

		public static void Restore(Engine250 engine, string saved)
		{
			string[] halves = saved.Split('|');
			string[] fluids = halves[0].Split(',');
			if (fluids.Length >= 4)
			{
				SetFloat(engine, "newFuelLevel", P(fluids[0]));
				SetFloat(engine, "newOilLevel", P(fluids[1]));
				SetFloat(engine, "newCoolantLevel", P(fluids[2]));
				SetFloat(engine, "transFluid", P(fluids[3]));
			}
			if (halves.Length < 2 || halves[1].Length == 0)
			{
				return;
			}
			Dictionary<string, FieldInfo> byName = new Dictionary<string, FieldInfo>();
			foreach (FieldInfo f in PartFields(engine))
			{
				byName[f.Name] = f;
			}
			foreach (string entry in halves[1].Split(','))
			{
				// name=health:fitted[:recorded condition]
				int eq = entry.IndexOf('=');
				FieldInfo f;
				if (eq < 0 || !byName.TryGetValue(entry.Substring(0, eq), out f))
				{
					continue;
				}
				string[] v = entry.Substring(eq + 1).Split(':');
				durability part = f.GetValue(engine) as durability;
				if (part == null || v.Length < 2)
				{
					continue;
				}
				part.health = P(v[0]);
				bool fitted = v[1] == "1";
				SetFitted(part, fitted);
				SetFloat(engine, CndName(f), v.Length > 2 ? P(v[2]) : (fitted ? part.health : 0f));
			}
		}

		// carb_cnd_c -> carb_cnd
		private static string CndName(FieldInfo partField)
		{
			return partField.Name.Substring(0, partField.Name.Length - 2);
		}

		public static float GetFloat(Engine250 engine, string name)
		{
			FieldInfo f = engine.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return f != null && f.FieldType == typeof(float) ? (float)f.GetValue(engine) : 0f;
		}

		public static void SetFloat(Engine250 engine, string name, float value)
		{
			FieldInfo f = engine.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (f != null && f.FieldType == typeof(float))
			{
				f.SetValue(engine, value);
			}
		}

		private static void Invoke(object target, string method)
		{
			MethodInfo m = target.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
			if (m != null)
			{
				try
				{
					m.Invoke(target, null);
				}
				catch (Exception e)
				{
					AtvMod.Log("Engine250." + method + " failed: " + e.InnerException?.Message);
				}
			}
		}

		private static float P(string s)
		{
			return float.Parse(s, NumberStyles.Float, Inv);
		}

		public static void Reset()
		{
			if (holder != null)
			{
				Object.Destroy(holder);
			}
			holder = null;
			template = null;
		}
	}
}
