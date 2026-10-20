using System.Collections.Generic;
using UnityEngine;

namespace TruckPartsQOL
{
	// A wheel spacer: a billet ring that goes between the brake disc (hub) and the
	// wheel, pushing the wheel out by its thickness, the look and the track width
	// both. Fitted to a hub (look at the brake disc or the wheel and press the
	// install key), it moves that hub's wheel holder and its wheel collider out.
	//
	// The offset is set absolutely (a wheel's rest position along its axle is
	// taken as the hub's face), so saving and reloading can never stack it.
	public class WheelSpacer : MonoBehaviour
	{
		public static float Thickness = 0.038f;   // 1.5", from the settings

		[System.NonSerialized]
		private Transform hub;
		[System.NonSerialized]
		private bool applied;

		// The hub (the game's BrakeDisk: it carries the WHEEL_HOLDER) a hit is on.
		public static Transform HubOf(Transform t)
		{
			for (int i = 0; t != null && i < 4; t = t.parent, i++)
			{
				if (t.Find("WHEEL_HOLDER") != null)
				{
					return t;
				}
			}
			return null;
		}

		// Where a spacer sits on a hub: on the hub's face, facing out.
		public static void Pose(Transform hub, out Vector3 position, out Quaternion rotation)
		{
			Transform holder = hub.Find("WHEEL_HOLDER");
			Vector3 outward = Outward(hub, holder);
			Vector3 face = holder.parent.TransformPoint(Perp(holder.localPosition, holder.parent.InverseTransformDirection(outward)));
			position = face + outward * (Thickness / 2f);
			GameObject vehicle = Vehicles.FindRoot(hub);
			rotation = Quaternion.LookRotation(outward, vehicle != null ? vehicle.transform.up : Vector3.up);
		}

		// Out along the axle: the way the wheel's parts sit from the holder, or
		// failing that, away from the vehicle's middle.
		private static Vector3 Outward(Transform hub, Transform holder)
		{
			GameObject vehicle = Vehicles.FindRoot(hub);
			Vector3 side = vehicle != null ? vehicle.transform.right : hub.right;
			float sign = vehicle != null && Vector3.Dot(hub.position - vehicle.transform.position, side) < 0f ? -1f : 1f;
			Vector3 outward = side * sign;
			foreach (Transform part in holder)
			{
				Vector3 d = part.position - holder.position;
				if (d.magnitude > 0.02f && Mathf.Abs(Vector3.Dot(d.normalized, side)) > 0.7f)
				{
					return Vector3.Dot(d, outward) > 0f ? d.normalized : -d.normalized;
				}
			}
			return outward;
		}

		private static Vector3 Perp(Vector3 v, Vector3 axis)
		{
			axis = axis.normalized;
			return v - axis * Vector3.Dot(v, axis);
		}

		private void LateUpdate()
		{
			TruckPart part = GetComponent<TruckPart>();
			Transform now = part != null && part.installed ? HubOf(transform.parent) : null;
			if (now != hub)
			{
				if (hub != null && applied)
				{
					Set(hub, 0f);
				}
				hub = now;
				applied = false;
			}
			if (hub != null && !applied)
			{
				Set(hub, Thickness);
				applied = true;
			}
		}

		private void OnDisable()
		{
			if (hub != null && applied)
			{
				Set(hub, 0f);
			}
			applied = false;
			hub = null;
		}

		// Push this hub's wheel out by `offset` along its axle (0: back on the hub).
		public static void Set(Transform hub, float offset)
		{
			Transform holder = hub.Find("WHEEL_HOLDER");
			if (holder == null)
			{
				return;
			}
			Vector3 outward = Outward(hub, holder);
			Vector3 axis = holder.parent.InverseTransformDirection(outward);
			holder.localPosition = Perp(holder.localPosition, axis) + axis.normalized * offset;
			WheelCollider wc = ColliderFor(hub);
			if (wc != null)
			{
				Vector3 a = wc.transform.InverseTransformDirection(outward).normalized;
				wc.center = Perp(wc.center, a) + a * offset;
			}
		}

		// The vehicle's wheel collider nearest this hub.
		private static WheelCollider ColliderFor(Transform hub)
		{
			GameObject vehicle = Vehicles.FindRoot(hub);
			if (vehicle == null)
			{
				return null;
			}
			WheelCollider best = null;
			float bestD = 0.8f;
			foreach (WheelCollider wc in vehicle.GetComponentsInChildren<WheelCollider>())
			{
				float d = Vector3.Distance(wc.transform.position, hub.position);
				if (d < bestD)
				{
					bestD = d;
					best = wc;
				}
			}
			return best;
		}

		// After loading (the game may restore a wheel where a spacer had pushed it,
		// and copies of a vehicle take its wheels' positions along): every hub
		// without a spacer goes back to its rest position.
		public static void ResetHubsWithout(List<GameObject> vehicles)
		{
			HashSet<Transform> spaced = new HashSet<Transform>();
			foreach (WheelSpacer s in Object.FindObjectsOfType<WheelSpacer>())
			{
				TruckPart part = s.GetComponent<TruckPart>();
				Transform h = part != null && part.installed ? HubOf(s.transform.parent) : null;
				if (h != null)
				{
					spaced.Add(h);
				}
			}
			foreach (GameObject vehicle in vehicles)
			{
				foreach (Transform t in vehicle.GetComponentsInChildren<Transform>(true))
				{
					if (t.name != "WHEEL_HOLDER" || t.parent == null || spaced.Contains(t.parent))
					{
						continue;
					}
					Transform hub = t.parent;
					Vector3 axis = hub.InverseTransformDirection(Outward(hub, t));
					if (Mathf.Abs(Vector3.Dot(t.localPosition, axis.normalized)) > 0.015f)
					{
						Set(hub, 0f);
					}
				}
			}
		}
	}
}
