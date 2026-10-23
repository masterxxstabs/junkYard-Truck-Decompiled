using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EngineCloner
{
	// The game saves its engines and trucks with Easy Save, references and all.
	// A reference to something in one of our copies (a Ranger) can't be saved:
	// it comes back empty. This mod points an engine bolted into a copy at that
	// copy (its truck, battery, gearbox, bay), so saving with the game's own
	// engine in a Ranger, or after one had been, left that engine with no truck,
	// no battery and no transmission when the save loaded, and it could never run
	// again. Two fixes:
	//
	//   SaveGuard  just before the game saves, everything the game saves that
	//              points into a copy is pointed at the same part of the game's
	//              own vehicle instead, and put back right after.
	//   Repair     the engines' truck-side references that are empty (from a save
	//              made before this fix) are filled in again: with the vehicle
	//              the engine is bolted into, or the game's own truck if it's
	//              loose; then the usual localizing points it at its vehicle.
	internal static class EngineRepair
	{
		private const BindingFlags Fields = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private enum Side
		{
			Truck,     // the Diamondback (car), or the vehicle it's bolted into
			F100,      // the F100 (car4)
			Amc        // the AMC (car3)
		}

		private struct Link
		{
			public Side side;
			public string path;   // under the vehicle; "" for the vehicle itself

			public Link(Side side, string path)
			{
				this.side = side;
				this.path = path;
			}
		}

		// Field name -> where it points, from the game's own truck (the I4 in the
		// Diamondback's prefab, the V8 in the F100's) and the trucks' part names.
		// A "X" field and its "X_cnd_c" durability point at the same part.
		private static readonly Dictionary<string, Link> Links = new Dictionary<string, Link>
		{
			{ "truck", new Link(Side.Truck, "") },
			{ "truckRb", new Link(Side.Truck, "") },
			{ "truckScript", new Link(Side.Truck, "") },
			{ "battery", new Link(Side.Truck, "battery_e") },
			{ "radiator", new Link(Side.Truck, "radiator_e") },
			{ "transmission", new Link(Side.Truck, "Drivetrain/transmission_e") },
			{ "am_transmission", new Link(Side.Truck, "Drivetrain/v8_trans_e") },
			{ "transferCase", new Link(Side.Truck, "Drivetrain/transferCase_e") },
			{ "driveRingGearF4", new Link(Side.Truck, "SuspensionOffroadCar/Visuals/AxleFrontLeft/AxleFrontMiddle/diffringgearF4_e") },
			{ "driveRingGearF5", new Link(Side.Truck, "SuspensionOffroadCar/Visuals/AxleFrontLeft/AxleFrontMiddle/diffringgearF5_e") },
			{ "driveRingGearR4", new Link(Side.Truck, "SuspensionOffroadCar/Visuals/AxleRearLeft/AxleRearMiddle/diffringgearR4_e") },
			{ "driveRingGearR5", new Link(Side.Truck, "SuspensionOffroadCar/Visuals/AxleRearLeft/AxleRearMiddle/diffringgearR5_e") },
			{ "idCab", new Link(Side.Truck, "cabin_low") },
			{ "ebtemplate", new Link(Side.Truck, "Engine/ebtemplate2") },
			{ "ebtemplatev8", new Link(Side.Truck, "Engine/ebtemplate2v8") },
			{ "ebtemplatei6", new Link(Side.Truck, "Engine/ebtemplatei62") },
			{ "truck2", new Link(Side.F100, "") },
			{ "truck2Rb", new Link(Side.F100, "") },
			{ "batteryF", new Link(Side.F100, "battery_e") },
			{ "transferCaseF", new Link(Side.F100, "transferCaseF_e") },
			{ "am_transmissionF", new Link(Side.F100, "v8_transF_e") },
			{ "driveRingGearF4F", new Link(Side.F100, "SuspensionOffroadCar2/Visuals/AxleFrontLeft/AxleFrontMiddle/diffringgearF4_e") },
			{ "driveRingGearF5F", new Link(Side.F100, "SuspensionOffroadCar2/Visuals/AxleFrontLeft/AxleFrontMiddle/diffringgearF5_e") },
			{ "driveRingGearR4F", new Link(Side.F100, "SuspensionOffroadCar2/Visuals/AxleRearLeftF/AxleRearMiddle/diffringgearR4_e") },
			{ "driveRingGearR5F", new Link(Side.F100, "SuspensionOffroadCar2/Visuals/AxleRearLeftF/AxleRearMiddle/diffringgearR5_e") },
			{ "ebtemplatev8F", new Link(Side.F100, "ebtemplate3v8") },
		};

		private static readonly HashSet<string> reported = new HashSet<string>();

		public static void Reset()
		{
			reported.Clear();
		}

		// --- Repair. ---

		// Every engine script's empty truck-side references, filled in again.
		public static void RepairAll()
		{
			foreach (Type t in new[] { typeof(engine), typeof(enginev8), typeof(enginei6) })
			{
				foreach (Object o in Object.FindObjectsOfType(t))
				{
					try
					{
						Repair((MonoBehaviour)o);
					}
					catch (Exception e)
					{
						EngineClonerMod.Log("Couldn't check " + ((Component)o).name + "'s references: " + e.Message);
					}
				}
			}
		}

		public static int Repair(MonoBehaviour script)
		{
			GameObject block = script.gameObject;
			// The vehicle it's bolted into, if any.
			GameObject mountedIn = null;
			FixedJoint joint = block.GetComponent<FixedJoint>();
			if (block.transform.parent != null && joint != null && joint.connectedBody != null && joint.connectedBody.transform == block.transform.parent)
			{
				mountedIn = VehicleCloner.FindVehicleRoot(block.transform.parent);
			}
			List<string> filled = new List<string>();
			List<string> missing = new List<string>();
			foreach (FieldInfo f in AllFields(script.GetType()))
			{
				if (!typeof(Object).IsAssignableFrom(f.FieldType))
				{
					continue;
				}
				Object current = f.GetValue(script) as Object;
				if (current != null)
				{
					continue;
				}
				Link link;
				string stem = f.Name.EndsWith("_cnd_c", StringComparison.Ordinal) ? f.Name.Substring(0, f.Name.Length - 6) : f.Name;
				if (!Links.TryGetValue(stem, out link))
				{
					continue;
				}
				GameObject vehicle = Home(link.side, mountedIn);
				// An engine in the F100 reads its transfer case from the F100.
				if (vehicle != null && stem == "transferCase" && vehicle.GetComponent<car4>() != null)
				{
					link = new Link(Side.F100, "transferCaseF_e");
				}
				Transform at = vehicle != null ? HierarchyPath.Find(vehicle.transform, link.path) : null;
				Object value = at != null ? As(at, f.FieldType) : null;
				if (value == null)
				{
					missing.Add(f.Name);
					continue;
				}
				f.SetValue(script, value);
				filled.Add(f.Name);
			}
			string key = block.GetInstanceID() + ":" + string.Join(",", filled.ToArray()) + "|" + string.Join(",", missing.ToArray());
			if ((filled.Count > 0 || missing.Count > 0) && reported.Add(key))
			{
				EngineClonerMod.Log(Diagnostics.Name(block) + ": " + (filled.Count > 0 ? "filled in " + filled.Count + " empty reference(s) the save had lost (" + string.Join(", ", filled.ToArray()) + ")" : "")
					+ (missing.Count > 0 ? (filled.Count > 0 ? "; " : "") + "couldn't fill " + string.Join(", ", missing.ToArray()) : "") + ".");
			}
			if (filled.Count > 0 && mountedIn != null)
			{
				EngineClonerMod.ForgetLocalized(block);
				EngineClonerMod.LocalizeMounted(block);
			}
			return filled.Count;
		}

		// Where a link's side points for this engine: the truck it's bolted into
		// (for that kind of truck), else the game's own.
		private static GameObject Home(Side side, GameObject mountedIn)
		{
			Type type = side == Side.Truck ? typeof(car) : side == Side.F100 ? typeof(car4) : typeof(car3);
			if (mountedIn != null && mountedIn.GetComponent(type) != null)
			{
				return mountedIn;
			}
			return VehicleCloner.Original(type);
		}

		private static Object As(Transform t, Type type)
		{
			if (type == typeof(GameObject))
			{
				return t.gameObject;
			}
			if (type == typeof(Transform))
			{
				return t;
			}
			return typeof(Component).IsAssignableFrom(type) ? t.GetComponent(type) : null;
		}

		private static IEnumerable<FieldInfo> AllFields(Type type)
		{
			for (Type t = type; t != null && t != typeof(MonoBehaviour); t = t.BaseType)
			{
				foreach (FieldInfo f in t.GetFields(Fields | BindingFlags.DeclaredOnly))
				{
					yield return f;
				}
			}
		}

		// The engine's references that are empty (for the diagnostics).
		public static List<string> EmptyLinks(MonoBehaviour script)
		{
			List<string> empty = new List<string>();
			foreach (FieldInfo f in AllFields(script.GetType()))
			{
				if (typeof(Object).IsAssignableFrom(f.FieldType) && (f.GetValue(script) as Object) == null)
				{
					empty.Add(f.Name);
				}
			}
			return empty;
		}

		// --- SaveGuard. ---

		private struct Change
		{
			public object owner;
			public FieldInfo field;
			public int index;     // -1 for the field itself, else the list element
			public Object old;
		}

		private static readonly List<Change> changes = new List<Change>();

		// Just before the game saves: what it saves points at the game's own
		// vehicles, not into copies.
		public static void BeforeSave()
		{
			changes.Clear();
			Dictionary<GameObject, GameObject> originalOf = new Dictionary<GameObject, GameObject>();
			foreach (MonoBehaviour b in Resources.FindObjectsOfTypeAll<MonoBehaviour>())
			{
				if (b == null || !b.gameObject.scene.IsValid() || b.GetComponent("ES3AutoSave") == null)
				{
					continue;
				}
				foreach (FieldInfo f in AllFields(b.GetType()))
				{
					Type ft = f.FieldType;
					if (typeof(Object).IsAssignableFrom(ft))
					{
						Object v = f.GetValue(b) as Object;
						Object to = v != null ? InOriginal(v, ft, originalOf) : null;
						if (to != null)
						{
							changes.Add(new Change { owner = b, field = f, index = -1, old = v });
							f.SetValue(b, to);
						}
					}
					else if ((ft.IsArray && typeof(Object).IsAssignableFrom(ft.GetElementType())) || (ft.IsGenericType && ft.GetGenericTypeDefinition() == typeof(List<>) && typeof(Object).IsAssignableFrom(ft.GetGenericArguments()[0])))
					{
						IList list = f.GetValue(b) as IList;
						Type element = ft.IsArray ? ft.GetElementType() : ft.GetGenericArguments()[0];
						for (int i = 0; list != null && i < list.Count; i++)
						{
							Object v = list[i] as Object;
							Object to = v != null ? InOriginal(v, element, originalOf) : null;
							if (to != null)
							{
								changes.Add(new Change { owner = b, field = f, index = i, old = v });
								list[i] = to;
							}
						}
					}
				}
			}
			if (changes.Count > 0)
			{
				EngineClonerMod.Log("Saving: pointed " + changes.Count + " saved reference(s) at the game's own vehicles for the save (copies can't be saved).");
			}
		}

		public static void AfterSave()
		{
			for (int i = changes.Count - 1; i >= 0; i--)
			{
				Change c = changes[i];
				try
				{
					if (c.index < 0)
					{
						c.field.SetValue(c.owner, c.old);
					}
					else
					{
						IList list = c.field.GetValue(c.owner) as IList;
						if (list != null && c.index < list.Count)
						{
							list[c.index] = c.old;
						}
					}
				}
				catch (Exception)
				{
				}
			}
			changes.Clear();
		}

		// The same part in the game's own vehicle, if v is in a copy; else null.
		private static Object InOriginal(Object v, Type type, Dictionary<GameObject, GameObject> originalOf)
		{
			Transform t = v is GameObject ? ((GameObject)v).transform : v is Component ? ((Component)v).transform : null;
			if (t == null)
			{
				return null;
			}
			GameObject copy = VehicleCloner.FindVehicleRoot(t);
			if (copy == null || !VehicleCloner.IsCopy(copy))
			{
				return null;
			}
			// Engines are saved by the game wherever they're bolted (an engine and
			// its own parts stay as they are).
			for (Transform up = t; up != null && up != copy.transform; up = up.parent)
			{
				if (EngineClonerMod.IsEngineBlock(up.gameObject))
				{
					return null;
				}
			}
			GameObject original;
			if (!originalOf.TryGetValue(copy, out original))
			{
				Type vt = VehicleCloner.VehicleType(copy);
				original = vt != null ? VehicleCloner.Original(vt) : null;
				originalOf[copy] = original;
			}
			if (original == null)
			{
				return null;
			}
			Transform there = HierarchyPath.Find(original.transform, HierarchyPath.Of(copy.transform, t));
			if (there == null)
			{
				return null;
			}
			Object result = As(there, v is GameObject ? typeof(GameObject) : v is Transform ? typeof(Transform) : v.GetType());
			return result != null && type.IsInstanceOfType(result) ? result : null;
		}
	}
}
