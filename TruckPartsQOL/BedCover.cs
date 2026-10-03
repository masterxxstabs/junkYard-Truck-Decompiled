using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TruckPartsQOL
{
	// A tarp, tonneau or hard top fitted over a pickup bed.
	//
	// The game marks each bed with a TruckBedGrav: its transform's position and
	// scale are the bed volume (that's how it finds cargo to strap down), and
	// .truck is the vehicle. The cover measures that volume, finds the bed rails
	// and the cab with raycasts, and builds itself to fit, so it works on any
	// pickup (and on clones) without hard-coded sizes.
	//
	// Fitted, the cover is a child of the vehicle with solid colliders, part of the
	// vehicle's own collider set: cargo can't bounce out through a closed cover. The
	// vehicle's center of mass is kept where the game put it; the thin panels add
	// next to nothing to its inertia.
	public class BedCover : MonoBehaviour
	{
		// Saved and copied when a vehicle is cloned.
		public bool open;

		// Player's fine-tuning of the fitted height (Page Up / Page Down).
		public float heightOffset;

		private const string FittedName = "Fitted";

		public TruckPart Part
		{
			get { return GetComponent<TruckPart>(); }
		}

		public bool IsFitted
		{
			get { return transform.Find(FittedName) != null; }
		}

		public static bool IsCover(PartKind kind)
		{
			return kind == PartKind.Tarp || kind == PartKind.Tonneau || kind == PartKind.HardTop;
		}

		public static TruckBedGrav FindBed(GameObject vehicle)
		{
			foreach (TruckBedGrav bed in Resources.FindObjectsOfTypeAll<TruckBedGrav>())
			{
				if (bed != null && bed.gameObject.scene.IsValid() && bed.truck != null && bed.truck.gameObject == vehicle)
				{
					return bed;
				}
			}
			return null;
		}

		public static BedCover CoverOn(GameObject vehicle)
		{
			foreach (TruckPart part in TruckPart.All)
			{
				BedCover cover = part.GetComponent<BedCover>();
				if (cover != null && part.installed && part.Vehicle == vehicle)
				{
					return cover;
				}
			}
			return null;
		}

		// The bed in the vehicle's own frame.
		private struct Bed
		{
			public float width;
			public float length;
			public Vector3 center; // x/z: bed center; y: top of the rails
			public float cabSign;  // +1 if the cab is toward +z of the bed center
			public float shellHeight; // hard top: rails up to cab-roof height
		}

		public bool Fit(GameObject vehicle, bool checkClear, out string problem)
		{
			problem = null;
			TruckBedGrav bedZone = FindBed(vehicle);
			if (bedZone == null)
			{
				problem = "This vehicle has no bed.";
				return false;
			}
			Bed bed = Measure(bedZone, vehicle.transform);
			if (checkClear && Blocked(vehicle.transform, bed, Part.kind == PartKind.HardTop ? bed.shellHeight : 0.06f))
			{
				problem = "Clear the cargo sticking up out of the bed first.";
				return false;
			}
			TruckPart part = Part;
			if (part.IsHeld)
			{
				part.Pick.LetGo(0);
			}
			Rigidbody own = GetComponent<Rigidbody>();
			if (own != null)
			{
				own.isKinematic = true;
				Object.Destroy(own);
			}
			SetKitVisible(false);

			Rigidbody body = vehicle.GetComponent<Rigidbody>();
			PhysicsState saved = PhysicsState.Of(body);
			// Built in the vehicle's own units, like everything Measure returns.
			transform.SetParent(vehicle.transform, false);
			transform.localScale = Vector3.one;
			transform.localRotation = Quaternion.identity;
			transform.localPosition = bed.center + Vector3.up * heightOffset;
			GameObject fitted = new GameObject(FittedName);
			fitted.transform.SetParent(transform, false);
			BuildFitted(fitted.transform, part.kind, bed);
			ApplyOpen(fitted.transform);
			part.installed = true;
			if (part.Pick != null)
			{
				part.Pick.pickable = false;
			}
			saved.Restore(body);
			return true;
		}

		public void Unfit()
		{
			Transform fitted = transform.Find(FittedName);
			if (fitted == null)
			{
				return;
			}
			GameObject vehicle = Part.Vehicle;
			Rigidbody body = vehicle != null ? vehicle.GetComponent<Rigidbody>() : null;
			PhysicsState saved = PhysicsState.Of(body);
			// Detach first so the colliders leave the vehicle right away.
			fitted.SetParent(null, false);
			Object.Destroy(fitted.gameObject);
			saved.Restore(body);
			// Lift the kit above the bed before it drops.
			transform.position += Vector3.up * 0.6f;
			SetKitVisible(true);
		}

		public void AdjustHeight(float delta)
		{
			if (!IsFitted)
			{
				return;
			}
			heightOffset = Mathf.Clamp(heightOffset + delta, -1.5f, 1.5f);
			GameObject vehicle = Part.Vehicle;
			Rigidbody body = vehicle != null ? vehicle.GetComponent<Rigidbody>() : null;
			PhysicsState saved = PhysicsState.Of(body);
			transform.localPosition += Vector3.up * delta;
			saved.Restore(body);
		}

		public bool SetOpen(bool value, out string problem)
		{
			problem = null;
			Transform fitted = transform.Find(FittedName);
			if (fitted == null || value == open)
			{
				return false;
			}
			if (!value)
			{
				GameObject vehicle = Part.Vehicle;
				TruckBedGrav bedZone = vehicle != null ? FindBed(vehicle) : null;
				if (bedZone != null && Part.kind != PartKind.HardTop && Blocked(vehicle.transform, Measure(bedZone, vehicle.transform), 0.06f))
				{
					problem = "Something's sticking up in the way.";
					return false;
				}
			}
			open = value;
			GameObject v = Part.Vehicle;
			Rigidbody body = v != null ? v.GetComponent<Rigidbody>() : null;
			PhysicsState saved = PhysicsState.Of(body);
			ApplyOpen(fitted);
			saved.Restore(body);
			return true;
		}

		private void SetKitVisible(bool visible)
		{
			foreach (Transform child in transform)
			{
				if (child.name.StartsWith("Kit", StringComparison.Ordinal))
				{
					child.gameObject.SetActive(visible);
				}
			}
			BoxCollider kitBox = GetComponent<BoxCollider>();
			if (kitBox != null)
			{
				kitBox.enabled = visible;
				kitBox.isTrigger = false;
			}
		}

		// Show the open or closed pieces ("Open*" / "Closed*" children).
		private void ApplyOpen(Transform fitted)
		{
			foreach (Transform child in fitted)
			{
				if (child.name.StartsWith("Closed", StringComparison.Ordinal))
				{
					child.gameObject.SetActive(!open);
				}
				else if (child.name.StartsWith("Open", StringComparison.Ordinal))
				{
					child.gameObject.SetActive(open);
				}
				else if (child.name == "HatchPivot")
				{
					// Hard top: the rear glass swings up and out.
					float cabSign = child.localPosition.z < 0f ? 1f : -1f;
					child.localRotation = Quaternion.Euler(open ? cabSign * 75f : 0f, 0f, 0f);
				}
			}
		}

		private static Bed Measure(TruckBedGrav zone, Transform vehicle)
		{
			Transform z = zone.transform;
			// Bed size along the vehicle's axes, whatever the zone's own rotation.
			Quaternion relative = Quaternion.Inverse(vehicle.rotation) * z.rotation;
			Vector3 scale = z.lossyScale;
			Vector3 vs = vehicle.lossyScale;
			Vector3 size = Abs(relative * Vector3.right) * Mathf.Abs(scale.x) + Abs(relative * Vector3.up) * Mathf.Abs(scale.y) + Abs(relative * Vector3.forward) * Mathf.Abs(scale.z);
			size = new Vector3(size.x / vs.x, size.y / vs.y, size.z / vs.z);
			Vector3 c = vehicle.InverseTransformPoint(z.position);
			float top = c.y + size.y / 2f;
			float bottom = c.y - size.y / 2f;

			Bed bed;
			bed.width = Mathf.Clamp(size.x, 0.8f, 2.5f);
			bed.length = Mathf.Clamp(size.z, 0.8f, 3.5f);

			// Floor, then the rails: the highest truck surface along each side. The
			// game's bed zone can be a bit narrower than the bed, so look a little
			// past its edge too and keep the highest surface at each spot.
			float? floorHit = Probe(vehicle, zone, new Vector3(c.x, top + 1f, c.z), bottom - 0.5f);
			float floor = floorHit ?? bottom;
			List<float> rails = new List<float>();
			foreach (float side in new[] { -1f, 1f })
			{
				foreach (float along in new[] { -0.35f, 0f, 0.35f })
				{
					float? highest = null;
					foreach (float outward in new[] { 0f, 0.04f, 0.08f, 0.12f })
					{
						float x = c.x + side * (bed.width / 2f + outward);
						float? y = Probe(vehicle, zone, new Vector3(x, top + 1f, c.z + along * bed.length), floor);
						if (y.HasValue && y.Value > floor + 0.15f && y.Value < top + 0.6f && (!highest.HasValue || y.Value > highest.Value))
						{
							highest = y;
						}
					}
					if (highest.HasValue)
					{
						rails.Add(highest.Value);
					}
				}
			}
			float rail;
			if (rails.Count > 0)
			{
				rails.Sort();
				rail = rails[rails.Count / 2];
			}
			else
			{
				rail = floor + 0.45f;
			}
			bed.center = new Vector3(c.x, rail, c.z);

			// The cab is the taller end.
			float half = bed.length / 2f + 0.45f;
			float? plus = Probe(vehicle, zone, new Vector3(c.x, rail + 3f, c.z + half), rail);
			float? minus = Probe(vehicle, zone, new Vector3(c.x, rail + 3f, c.z - half), rail);
			float plusY = plus ?? rail;
			float minusY = minus ?? rail;
			if (Mathf.Abs(plusY - minusY) > 0.2f)
			{
				bed.cabSign = plusY > minusY ? 1f : -1f;
			}
			else
			{
				// Fall back to: the cab is toward the middle of the vehicle.
				bed.cabSign = c.z < 0f ? 1f : -1f;
			}
			float roof = Mathf.Max(plusY, minusY);
			bed.shellHeight = Mathf.Clamp(roof - rail, 0.35f, 0.9f);
			if (roof - rail < 0.3f)
			{
				bed.shellHeight = 0.55f;
			}
			TruckPartsQOLMod.Log(string.Format("Bed on {0}: zone center {1} size {2}; floor {3}; rails {4} from {5} samples; cab {6}; roof {7}; shell {8:0.00}",
				vehicle.name, c.ToString("F2"), size.ToString("F2"), floorHit.HasValue ? floorHit.Value.ToString("F2") : "not found (using zone bottom " + bottom.ToString("F2") + ")",
				rail.ToString("F2"), rails.Count, bed.cabSign > 0f ? "+z" : "-z", roof.ToString("F2"), bed.shellHeight));
			LogHits(vehicle, zone, new Vector3(c.x, top + 1f, c.z), bottom - 0.5f);
			return bed;
		}

		// Highest point of the vehicle below `from` (vehicle-local), straight down
		// to `downTo`.
		private static float? Probe(Transform vehicle, TruckBedGrav zone, Vector3 from, float downTo)
		{
			float? best = null;
			foreach (RaycastHit hit in VehicleHits(vehicle, zone, from, downTo))
			{
				float y = vehicle.InverseTransformPoint(hit.point).y;
				if (!best.HasValue || y > best.Value)
				{
					best = y;
				}
			}
			return best;
		}

		// Rays down through the vehicle's own surfaces only: not triggers, not the
		// Ignore Raycast layer (invisible helpers), not cargo lying in the bed (it
		// has a PickUp), not the bed zone itself, not our own parts. Body panels with
		// their own rigidbody still count as the vehicle.
		private static List<RaycastHit> VehicleHits(Transform vehicle, TruckBedGrav zone, Vector3 from, float downTo)
		{
			List<RaycastHit> result = new List<RaycastHit>();
			float distance = (from.y - downTo) * vehicle.lossyScale.y + 0.01f;
			if (distance <= 0f)
			{
				return result;
			}
			foreach (RaycastHit hit in Physics.RaycastAll(vehicle.TransformPoint(from), -vehicle.up, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
			{
				Transform t = hit.collider.transform;
				if (Vehicles.FindRoot(t) != vehicle.gameObject || t.IsChildOf(zone.transform) || hit.collider.GetComponentInParent<TruckPart>() != null)
				{
					continue;
				}
				if (hit.rigidbody != null && hit.rigidbody.GetComponent<PickUp>() != null)
				{
					continue;
				}
				result.Add(hit);
			}
			return result;
		}

		// For the log: what the floor probe hit, so a bad fit can be diagnosed.
		private static void LogHits(Transform vehicle, TruckBedGrav zone, Vector3 from, float downTo)
		{
			List<string> names = new List<string>();
			foreach (RaycastHit hit in VehicleHits(vehicle, zone, from, downTo))
			{
				names.Add(hit.collider.name + "@" + vehicle.InverseTransformPoint(hit.point).y.ToString("F2") + (hit.rigidbody != null && hit.rigidbody.transform != vehicle ? "(rb " + hit.rigidbody.name + ")" : ""));
			}
			TruckPartsQOLMod.Log("  floor probe hit: " + (names.Count > 0 ? string.Join(", ", names.ToArray()) : "nothing"));
		}

		// Any loose item poking into the band from the rails up to `height`?
		private static bool Blocked(Transform vehicle, Bed bed, float height)
		{
			Vector3 center = vehicle.TransformPoint(bed.center + Vector3.up * (height / 2f + 0.02f));
			Vector3 half = new Vector3(bed.width / 2f - 0.02f, height / 2f, bed.length / 2f - 0.02f);
			Rigidbody body = vehicle.GetComponent<Rigidbody>();
			foreach (Collider collider in Physics.OverlapBox(center, half, vehicle.rotation, ~0, QueryTriggerInteraction.Ignore))
			{
				Rigidbody other = collider.attachedRigidbody;
				// The vehicle's own panels don't count, only cargo.
				bool ownPanel = other != null && other.GetComponent<PickUp>() == null && Vehicles.FindRoot(other.transform) == vehicle.gameObject;
				if (other != null && other != body && !ownPanel && !other.isKinematic && collider.GetComponentInParent<CharacterController>() == null)
				{
					return true;
				}
			}
			return false;
		}

		private static Vector3 Abs(Vector3 v)
		{
			return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
		}

		// --- Geometry. Origin: bed center at rail height; +z along the vehicle. ---

		private static readonly Color TarpBlue = new Color(0.12f, 0.28f, 0.62f);
		private static readonly Color Black = new Color(0.04f, 0.04f, 0.045f);
		private static readonly Color DarkGrey = new Color(0.14f, 0.14f, 0.15f);
		private static readonly Color ShellWhite = new Color(0.9f, 0.9f, 0.88f);
		private static readonly Color Glass = new Color(0.05f, 0.07f, 0.09f);

		private static void BuildFitted(Transform root, PartKind kind, Bed bed)
		{
			float w = bed.width;
			float l = bed.length;
			float cab = bed.cabSign;
			if (kind == PartKind.Tarp)
			{
				// Slight peak down the middle, flaps over the rails, three straps.
				float tilt = Mathf.Atan2(0.03f, w / 2f) * Mathf.Rad2Deg;
				Piece(root, "ClosedTarpL", new Vector3(w / 2f + 0.04f, 0.008f, l + 0.04f), new Vector3(-w / 4f, 0.035f, 0f), TarpBlue).localRotation = Quaternion.Euler(0f, 0f, tilt);
				Piece(root, "ClosedTarpR", new Vector3(w / 2f + 0.04f, 0.008f, l + 0.04f), new Vector3(w / 4f, 0.035f, 0f), TarpBlue).localRotation = Quaternion.Euler(0f, 0f, -tilt);
				Piece(root, "ClosedFlapL", new Vector3(0.006f, 0.08f, l + 0.04f), new Vector3(-w / 2f - 0.04f, -0.02f, 0f), TarpBlue);
				Piece(root, "ClosedFlapR", new Vector3(0.006f, 0.08f, l + 0.04f), new Vector3(w / 2f + 0.04f, -0.02f, 0f), TarpBlue);
				Piece(root, "ClosedFlapRear", new Vector3(w + 0.08f, 0.08f, 0.006f), new Vector3(0f, -0.02f, -cab * (l / 2f + 0.02f)), TarpBlue);
				foreach (float z in new[] { -l / 3f, 0f, l / 3f })
				{
					Piece(root, "ClosedStrap", new Vector3(w + 0.1f, 0.012f, 0.025f), new Vector3(0f, 0.055f, z), Black);
				}
				Lid(root, "ClosedLid", w, l, 0.05f);
				Transform roll = Piece(root, "OpenRoll", new Vector3(0.16f, (w + 0.04f) / 2f, 0.16f), new Vector3(0f, 0.08f, cab * (l / 2f - 0.1f)), TarpBlue, PrimitiveType.Cylinder);
				roll.localRotation = Quaternion.Euler(0f, 0f, 90f);
			}
			else if (kind == PartKind.Tonneau)
			{
				// Tri-fold: three panels flat when closed, stood up against the cab
				// when open. Rails stay on the bed either way.
				float panel = l / 3f;
				for (int i = 0; i < 3; i++)
				{
					Piece(root, "ClosedPanel" + i, new Vector3(w + 0.04f, 0.025f, panel - 0.006f), new Vector3(0f, 0.013f, cab * (panel * (1 - i))), Black);
					Transform up = Piece(root, "OpenPanel" + i, new Vector3(w + 0.04f, panel - 0.006f, 0.025f), new Vector3(0f, panel / 2f + 0.01f, cab * (l / 2f - 0.03f - i * 0.03f)), Black);
					up.name = "OpenPanel" + i;
				}
				Piece(root, "RailL", new Vector3(0.035f, 0.03f, l), new Vector3(-w / 2f - 0.005f, -0.005f, 0f), DarkGrey);
				Piece(root, "RailR", new Vector3(0.035f, 0.03f, l), new Vector3(w / 2f + 0.005f, -0.005f, 0f), DarkGrey);
				Lid(root, "ClosedLid", w, l, 0.04f);
			}
			else
			{
				// Camper shell up to cab-roof height, with side windows and a rear
				// glass hatch hinged at the top.
				float h = bed.shellHeight;
				float rear = -cab * (l / 2f + 0.02f);
				Solid(root, "Roof", new Vector3(w + 0.08f, 0.035f, l + 0.04f), new Vector3(0f, h, 0f), ShellWhite);
				Solid(root, "Front", new Vector3(w + 0.08f, h, 0.03f), new Vector3(0f, h / 2f, cab * (l / 2f + 0.02f)), ShellWhite);
				Piece(root, "FrontWindow", new Vector3(w * 0.6f, h * 0.4f, 0.034f), new Vector3(0f, h * 0.6f, cab * (l / 2f + 0.02f)), Glass, PrimitiveType.Cube, true);
				foreach (float side in new[] { -1f, 1f })
				{
					float x = side * (w / 2f + 0.025f);
					string n = side < 0f ? "L" : "R";
					Solid(root, "Side" + n, new Vector3(0.03f, h, l + 0.04f), new Vector3(x, h / 2f, 0f), ShellWhite);
					Piece(root, "Window" + n, new Vector3(0.034f, h - 0.16f, l - 0.2f), new Vector3(x, 0.08f + (h - 0.16f) / 2f + 0.02f, 0f), Glass, PrimitiveType.Cube, true);
				}
				Transform pivot = new GameObject("HatchPivot").transform;
				pivot.SetParent(root, false);
				pivot.localPosition = new Vector3(0f, h, rear);
				Solid(pivot, "HatchGlass", new Vector3(w + 0.04f, h - 0.02f, 0.02f), new Vector3(0f, -(h - 0.02f) / 2f, 0f), Glass).GetComponent<Renderer>().material.SetFloat("_Glossiness", 0.9f);
				Piece(pivot, "HatchFrame", new Vector3(w + 0.08f, 0.04f, 0.03f), new Vector3(0f, -(h - 0.02f), 0f), ShellWhite);
				Piece(pivot, "HatchHandle", new Vector3(0.12f, 0.02f, 0.03f), new Vector3(0f, -(h - 0.06f), -cab * 0.02f), DarkGrey);
			}
		}

		// A solid panel: visible and collidable.
		private static Transform Solid(Transform parent, string name, Vector3 size, Vector3 position, Color color)
		{
			Transform t = Piece(parent, name, size, position, color);
			t.gameObject.AddComponent<BoxCollider>();
			return t;
		}

		// An invisible collider that keeps cargo in while closed.
		private static void Lid(Transform parent, string name, float w, float l, float thickness)
		{
			GameObject lid = new GameObject(name);
			lid.transform.SetParent(parent, false);
			lid.transform.localPosition = new Vector3(0f, thickness / 2f, 0f);
			BoxCollider box = lid.AddComponent<BoxCollider>();
			box.size = new Vector3(w, thickness, l);
		}

		private static Transform Piece(Transform parent, string name, Vector3 size, Vector3 position, Color color, PrimitiveType type = PrimitiveType.Cube, bool glossy = false)
		{
			GameObject go = GameObject.CreatePrimitive(type);
			go.name = name;
			Object.DestroyImmediate(go.GetComponent<Collider>());
			go.transform.SetParent(parent, false);
			go.transform.localPosition = position;
			go.transform.localScale = size;
			Material material = go.GetComponent<Renderer>().material;
			material.color = color;
			if (glossy)
			{
				material.SetFloat("_Glossiness", 0.9f);
			}
			return go.transform;
		}

		// The vehicle's center of mass, kept where the game set it across collider
		// changes. (Inertia is left automatic so it still scales with the cargo mass
		// the game adds in TruckBedGrav.CheckBed.)
		private struct PhysicsState
		{
			private bool valid;
			private Vector3 centerOfMass;

			public static PhysicsState Of(Rigidbody body)
			{
				PhysicsState s = new PhysicsState();
				if (body != null)
				{
					s.valid = true;
					s.centerOfMass = body.centerOfMass;
				}
				return s;
			}

			public void Restore(Rigidbody body)
			{
				if (valid && body != null)
				{
					body.centerOfMass = centerOfMass;
				}
			}
		}
	}
}
