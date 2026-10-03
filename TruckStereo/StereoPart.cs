using System;
using System.Collections.Generic;
using UnityEngine;

namespace TruckStereo
{
	public enum PartKind
	{
		HeadUnit,
		Speaker,
		Subwoofer,
		CD
	}

	// One stereo item: carried with the game's own PickUp, installed by parenting
	// it to the vehicle with its colliders turned into triggers (so it adds no
	// mass and doesn't collide with the vehicle it's bolted to).
	//
	// Note for the Engine Cloner mod: nothing here keeps a reference to the vehicle.
	// Its reference swapper rewrites fields that point at vehicles, so the vehicle
	// is always looked up from the transform hierarchy instead.
	public class StereoPart : MonoBehaviour
	{
		public static readonly List<StereoPart> All = new List<StereoPart>();

		// Public so Object.Instantiate (e.g. cloning a vehicle) copies them.
		public PartKind kind;

		public int cdNumber;

		public bool installed;

		public float mass = 1f;

		// Distance from the part's center to its back face, so it sits flush.
		public float halfDepth;

		public float halfHeight;

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
					return "6.5\" speaker";
				case PartKind.Subwoofer:
					return "subwoofer";
				default:
					return "CD " + cdNumber;
				}
			}
		}

		// Mount on the surface the player is looking at. mountTo is the nearest
		// rigidbody on the vehicle (the body, or a door so it swings with it).
		public void Install(Transform mountTo, Vector3 point, Vector3 normal)
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
			foreach (Collider collider in GetComponentsInChildren<Collider>())
			{
				collider.isTrigger = true;
			}
			transform.SetParent(mountTo, true);
			Vector3 up = Mathf.Abs(Vector3.Dot(normal, mountTo.up)) > 0.9f ? mountTo.forward : mountTo.up;
			transform.rotation = Quaternion.LookRotation(normal, up);
			transform.position = point + normal * halfDepth;
			gameObject.layer = 0;
			if (Pick != null)
			{
				Pick.pickable = false;
			}
			installed = true;
		}

		public void Remove()
		{
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
