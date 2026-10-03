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
				parts.Add(f.Name + "=" + part.health.ToString("R", Inv) + ":" + (r == null || r.enabled ? "1" : "0"));
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
				int eq = entry.IndexOf('=');
				int colon = entry.LastIndexOf(':');
				FieldInfo f;
				if (eq < 0 || colon < eq || !byName.TryGetValue(entry.Substring(0, eq), out f))
				{
					continue;
				}
				durability part = f.GetValue(engine) as durability;
				if (part == null)
				{
					continue;
				}
				part.health = P(entry.Substring(eq + 1, colon - eq - 1));
				SetFitted(part, entry.Substring(colon + 1) == "1");
			}
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
