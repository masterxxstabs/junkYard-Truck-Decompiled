using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TruckPartsQOL
{
	public enum PartKind
	{
		HeadUnit,
		Speaker,
		Subwoofer,
		CD,
		Tarp,
		Tonneau,
		HardTop,
		Scanner,
		Amplifier,
		RockLight,
		WheelSpacer
	}

	// One stereo item: carried with the game's own PickUp, installed by parenting
	// it to the vehicle with its colliders turned into triggers (so it adds no
	// mass and doesn't collide with the vehicle it's bolted to).
	//
	// Note for the Engine Cloner mod: nothing here keeps a reference to the vehicle.
	// Its reference swapper rewrites fields that point at vehicles, so the vehicle
	// is always looked up from the transform hierarchy instead.
	public class TruckPart : MonoBehaviour
	{
		public static readonly List<TruckPart> All = new List<TruckPart>();

		// Public so Object.Instantiate (e.g. cloning a vehicle) copies them.
		public PartKind kind;

		public int cdNumber;

		public bool installed;

		public float mass = 1f;

		// Distance from the part's center to its back face, so it sits flush.
		public float halfDepth;

		public float halfHeight;

		// How far the part sinks into the surface it's fitted to: door speakers go
		// in flush, with only their frame standing proud.
		public float inset;

		// Speakers: set by the head unit each frame it drives this speaker.
		[NonSerialized]
		public int drivenFrame = -1;

		private void OnEnable()
		{
			All.Add(this);
		}

		// Shops drop items at the player's feet; a part spawned partly inside the
		// ground falls through it. Lift loose parts clear of the ground once.
		private void Start()
		{
			if (installed || transform.parent != null)
			{
				return;
			}
			RaycastHit[] hits = Physics.RaycastAll(transform.position + Vector3.up * 2f, Vector3.down, 6f, ~0, QueryTriggerInteraction.Ignore);
			float groundY = float.MinValue;
			foreach (RaycastHit hit in hits)
			{
				if (!hit.transform.IsChildOf(transform) && hit.point.y > groundY && hit.collider.GetComponentInParent<CharacterController>() == null)
				{
					groundY = hit.point.y;
				}
			}
			float lowest = groundY + Mathf.Max(halfHeight, halfDepth) + 0.05f;
			if (groundY > float.MinValue && transform.position.y < lowest)
			{
				transform.position = new Vector3(transform.position.x, lowest, transform.position.z);
			}
		}

		private void OnDisable()
		{
			All.Remove(this);
		}

		public PickUp Pick
		{
			get { return GetComponent<PickUp>(); }
		}

		public bool IsHeld
		{
			get
			{
				PickUp pick = Pick;
				return pick != null && pick.holding;
			}
		}

		public bool IsSpeaker
		{
			get { return kind == PartKind.Speaker || kind == PartKind.Subwoofer; }
		}

		public PartBolts Bolts
		{
			get { return GetComponent<PartBolts>(); }
		}

		public GameObject Vehicle
		{
			get { return installed ? Vehicles.FindRoot(transform) : null; }
		}

		public string DisplayName
		{
			get
			{
				switch (kind)
				{
				case PartKind.HeadUnit:
					return "CD head unit";
				case PartKind.Speaker:
					return "6.5\" door speaker";
				case PartKind.Subwoofer:
					return "subwoofer";
				case PartKind.Tarp:
					return "bed tarp";
				case PartKind.Tonneau:
					return "tonneau cover";
				case PartKind.HardTop:
					return "hard top";
				case PartKind.Scanner:
					return "OBD scanner";
				case PartKind.Amplifier:
					return "4-channel amplifier";
				case PartKind.RockLight:
					return "rock light";
				case PartKind.WheelSpacer:
					return "wheel spacer";
				default:
					return "CD " + cdNumber;
				}
			}
		}

		// Mount on the surface the player is looking at. mountTo is the nearest
		// rigidbody on the vehicle (the body, or a door so it swings with it).
		// Where the part goes when fitted at this point of a surface: flush on it,
		// facing out of it (a rock light: aimed down at the ground), turned by
		// spin degrees round its facing and, for a rock light, tilted by tilt.
		public void PlacementPose(Transform mountTo, Vector3 point, Vector3 normal, float spin, float tilt, out Vector3 position, out Quaternion rotation)
		{
			// A wheel spacer sits on its hub's face, wherever it's aimed.
			Transform hub = kind == PartKind.WheelSpacer ? WheelSpacer.HubOf(mountTo) : null;
			if (hub != null)
			{
				WheelSpacer.Pose(hub, out position, out rotation);
				rotation = rotation * Quaternion.Euler(0f, 0f, spin);
				return;
			}
			Vector3 up = Mathf.Abs(Vector3.Dot(normal, mountTo.up)) > 0.9f ? mountTo.forward : mountTo.up;
			rotation = Quaternion.LookRotation(normal, up);
			position = point + normal * (halfDepth - inset);
			if (kind == PartKind.RockLight)
			{
				AimAtGround(mountTo, point, normal, ref position, ref rotation);
				rotation = rotation * Quaternion.Euler(tilt, 0f, 0f);
			}
			rotation = rotation * Quaternion.Euler(0f, 0f, spin);
		}

		public bool CanTilt
		{
			get { return kind == PartKind.RockLight; }
		}

		public void Install(Transform mountTo, Vector3 point, Vector3 normal)
		{
			Install(mountTo, point, normal, 0f, 0f);
		}

		public void Install(Transform mountTo, Vector3 point, Vector3 normal, float spin, float tilt)
		{
			if (IsHeld)
			{
				Pick.LetGo(0);
			}
			Rigidbody body = GetComponent<Rigidbody>();
			if (body != null)
			{
				// The game's hold joint may still point at this body until the end of
				// the frame, so remove it then rather than right now.
				body.isKinematic = true;
				Destroy(body);
			}
			PartBolts bolts = Bolts;
			foreach (Collider collider in GetComponentsInChildren<Collider>())
			{
				// Bolts stay solid like the game's, so its ratchet can aim at them.
				if (bolts == null || !bolts.IsBolt(collider))
				{
					collider.isTrigger = true;
				}
			}
			transform.SetParent(mountTo, true);
			Vector3 position;
			Quaternion rotation;
			PlacementPose(mountTo, point, normal, spin, tilt, out position, out rotation);
			transform.SetPositionAndRotation(position, rotation);
			if (bolts != null)
			{
				// Fitted loose: bolt it down with the ratchet.
				bolts.Show(true);
				bolts.SetAll(0);
			}
			gameObject.layer = 0;
			if (Pick != null)
			{
				Pick.pickable = false;
			}
			installed = true;
		}

		// A rock light hangs off whatever it's fitted to (an inner fender, the
		// frame, under the body) on its bracket and points down at the ground,
		// tilted a little out from the truck, whichever way that surface faces.
		private static void AimAtGround(Transform mountTo, Vector3 point, Vector3 normal, ref Vector3 position, ref Quaternion rotation)
		{
			GameObject vehicle = Vehicles.FindRoot(mountTo);
			Transform v = vehicle != null ? vehicle.transform : mountTo;
			Vector3 down = -v.up;
			Vector3 outward = Vector3.ProjectOnPlane(point - v.position, v.up);
			outward = outward.sqrMagnitude > 0.0001f ? outward.normalized : Vector3.zero;
			Vector3 aim = (down + outward * 0.35f).normalized;
			// On a surface that faces up (the top of something), down would point
			// into it: then it faces straight out of the surface as before.
			if (Vector3.Dot(aim, normal) < -0.1f)
			{
				return;
			}
			rotation = Quaternion.LookRotation(aim, Mathf.Abs(Vector3.Dot(aim, v.forward)) > 0.95f ? v.up : v.forward);
			// Its back, turned off the surface, must not sink into it.
			position = point + normal * 0.045f;
		}

		public void Remove()
		{
			BedCover cover = GetComponent<BedCover>();
			if (cover != null)
			{
				cover.Unfit();
			}
			if (Bolts != null)
			{
				Bolts.Show(false);
			}
			transform.SetParent(null, true);
			foreach (Collider collider in GetComponentsInChildren<Collider>())
			{
				collider.isTrigger = false;
			}
			Rigidbody body = GetComponent<Rigidbody>();
			if (body == null)
			{
				body = gameObject.AddComponent<Rigidbody>();
			}
			body.mass = mass;
			if (Pick != null)
			{
				Pick.pickable = true;
			}
			installed = false;
		}

		// Speakers fall silent the moment no head unit is driving them (powered off,
		// removed, or the speaker itself was taken out).
		private void LateUpdate()
		{
			if (!IsSpeaker)
			{
				return;
			}
			AudioSource source = GetComponent<AudioSource>();
			if (source != null && source.isPlaying && drivenFrame < Time.frameCount)
			{
				source.Stop();
			}
		}
	}

	internal static class Vehicles
	{
		private static readonly Type[] VehicleTypes = { typeof(car), typeof(car4), typeof(car3), typeof(golfcart), typeof(Dirtbike) };

		public static bool IsVehicle(GameObject go)
		{
			foreach (Type type in VehicleTypes)
			{
				if (go.GetComponent(type) != null)
				{
					return true;
				}
			}
			return false;
		}

		// The name a vehicle is saved under. Copies made by Engine Cloner (a Ranger,
		// a cloned truck) keep the original's name, since the game finds vehicles by
		// name; a tagged one (VehicleTag) is saved as name#tag.
		public static string SaveName(GameObject vehicle)
		{
			string tag = Tag(vehicle);
			return tag != null ? vehicle.name + "#" + tag : vehicle.name;
		}

		public static string Tag(GameObject vehicle)
		{
			Component c = vehicle != null ? vehicle.GetComponent("VehicleTag") : null;
			FieldInfo id = c != null ? c.GetType().GetField("id") : null;
			string value = id != null ? id.GetValue(c) as string : null;
			return string.IsNullOrEmpty(value) ? null : value;
		}

		private static MethodInfo isCopy;
		private static bool lookedForIsCopy;

		// Whether Engine Cloner made this vehicle (false without Engine Cloner).
		public static bool IsCopy(GameObject vehicle)
		{
			if (!lookedForIsCopy)
			{
				lookedForIsCopy = true;
				foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
				{
					Type api = a.GetType("EngineCloner.VehicleApi", false);
					if (api != null)
					{
						isCopy = api.GetMethod("IsCopy", new[] { typeof(GameObject) });
						break;
					}
				}
			}
			try
			{
				return isCopy != null && (bool)isCopy.Invoke(null, new object[] { vehicle });
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static GameObject FindRoot(Transform t)
		{
			for (; t != null; t = t.parent)
			{
				if (IsVehicle(t.gameObject))
				{
					return t.gameObject;
				}
			}
			return null;
		}

		public static List<GameObject> AllVehicles()
		{
			List<GameObject> list = new List<GameObject>();
			foreach (Type type in VehicleTypes)
			{
				foreach (UnityEngine.Object found in UnityEngine.Object.FindObjectsOfType(type))
				{
					list.Add(((Component)found).gameObject);
				}
			}
			return list;
		}
	}
}
