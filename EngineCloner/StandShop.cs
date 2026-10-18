using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace EngineCloner
{
	// Extra engine stands, sold like the engines. The game has one stand and finds
	// it by name: its swinging part "DK_9" (what a block is jointed to), the
	// "EngRelease" lever, the hinge Interactor.EngStand it turns, and the snap
	// points "ebtemplate"/"ebtemplatev8"/"ebtemplatei6". A copy of the smallest
	// object holding all of those is a complete second stand; the patches below
	// make snapping, turning and releasing work on the stand you actually use.
	//
	// A bought stand is placed upright on the ground where it's delivered (a
	// joint fixes to the world where it's created, so it's built in place rather
	// than moved there). Move it later: look at it, press the move key (K), look
	// where you want it, press K again; only with no engine on it.
	internal static class StandShop
	{
		public const string ShopName = "Tool - Engine stand";
		private const string Category = "Truck Part";

		private static GameObject holder;
		private static GameObject template;
		private static GameObject marker;
		private static bool failed;

		public static void Reset()
		{
			holder = null;
			template = null;
			marker = null;
			failed = false;
			moving = null;
		}

		// The original stand's root: the smallest object holding all of its parts.
		private static Transform OriginalRoot()
		{
			Interactor interactor = Object.FindObjectOfType<Interactor>();
			List<Transform> parts = new List<Transform>();
			foreach (string name in new[] { "DK_9", "EngRelease", "ebtemplate", "ebtemplatev8", "ebtemplatei6" })
			{
				GameObject go = GameObject.Find(name);
				if (go != null && go.GetComponentInParent<BoughtStand>() == null)
				{
					parts.Add(go.transform);
				}
			}
			if (interactor != null && interactor.EngStand != null)
			{
				parts.Add(interactor.EngStand.transform);
			}
			if (parts.Count < 2)
			{
				return null;
			}
			Transform root = parts[0];
			foreach (Transform t in parts)
			{
				while (root != null && !t.IsChildOf(root))
				{
					root = root.parent;
				}
			}
			return root;
		}

		public static GameObject Template()
		{
			if (template != null || failed)
			{
				return template;
			}
			Transform root = OriginalRoot();
			string why = null;
			if (root == null)
			{
				why = "couldn't find the engine stand";
			}
			else if (root.GetComponentInChildren<car>(true) != null || root.GetComponentInChildren<Interactor>(true) != null)
			{
				why = "the stand is part of something bigger (" + root.name + ")";
			}
			else
			{
				Bounds b = new Bounds(root.position, Vector3.zero);
				foreach (Renderer r in root.GetComponentsInChildren<Renderer>(true))
				{
					b.Encapsulate(r.bounds);
				}
				if (b.size.magnitude > 4f)
				{
					why = "the stand's object (" + root.name + ") is " + b.size.magnitude.ToString("0.0") + " m across, too big to be just the stand";
				}
			}
			if (why != null)
			{
				failed = true;
				EngineClonerMod.Log("Engine stands aren't for sale: " + why + ".");
				return null;
			}
			if (holder == null)
			{
				holder = new GameObject("StandShop_Templates");
				holder.SetActive(false);
				SceneManager.MoveGameObjectToScene(holder, root.gameObject.scene);
			}
			template = Object.Instantiate(root.gameObject, holder.transform, false);
			template.name = root.name;
			EngineShop.StripGameSave(template);
			template.AddComponent<BoughtStand>();
			EngineClonerMod.Log("Engine stands for sale (a copy of \"" + root.name + "\").");
			return template;
		}

		// What shops sell: a marker that builds the stand where it's delivered.
		private static GameObject Marker()
		{
			if (marker == null)
			{
				if (holder == null)
				{
					return null;
				}
				marker = new GameObject("Engine stand (delivery)");
				marker.transform.SetParent(holder.transform, false);
				marker.AddComponent<StandDelivery>();
			}
			return marker;
		}

		public static object[][] ShopItems()
		{
			return Template() != null && Marker() != null ? new[] { new object[] { Marker(), ShopName, EngineClonerMod.StandPrice, Category } } : new object[0][];
		}

		// A stand standing upright on the ground at `near`, facing `face`.
		public static GameObject Build(Vector3 near, Vector3 face)
		{
			GameObject t = Template();
			if (t == null)
			{
				return null;
			}
			Vector3 spot = near;
			RaycastHit hit;
			if (Physics.Raycast(near + Vector3.up * 1.5f, Vector3.down, out hit, 6f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
			{
				spot = hit.point;
			}
			Vector3 flat = Vector3.ProjectOnPlane(face, Vector3.up);
			Quaternion yaw = flat.sqrMagnitude > 0.001f ? Quaternion.LookRotation(flat.normalized, Vector3.up) : Quaternion.identity;
			// The template keeps the original's tilt and the offset of its pivot
			// above its lowest point; keep that offset so it stands on the ground.
			float lift = t.transform.position.y - LowestPoint(t);
			// Its own pose, turned about the vertical to face the player.
			Quaternion rotation = yaw * Quaternion.Inverse(Quaternion.Euler(0f, t.transform.eulerAngles.y, 0f)) * t.transform.rotation;
			GameObject stand = Object.Instantiate(t, spot + Vector3.up * Mathf.Max(0f, lift), rotation);
			stand.name = t.name;
			return stand;
		}

		private static float LowestPoint(GameObject go)
		{
			float low = float.MaxValue;
			foreach (Renderer r in go.GetComponentsInChildren<Renderer>(true))
			{
				low = Mathf.Min(low, r.bounds.min.y);
			}
			return low == float.MaxValue ? go.transform.position.y : low;
		}

		// --- Moving a bought stand. ---

		private static BoughtStand moving;

		public static string Update(KeyCode key)
		{
			Camera cam = Camera.main;
			if (cam == null)
			{
				return null;
			}
			RaycastHit hit;
			bool looking = Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 8f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
			if (moving != null)
			{
				if (Input.GetKeyDown(key) && looking)
				{
					BoughtStand old = moving;
					moving = null;
					GameObject stand = Build(hit.point, cam.transform.position - hit.point);
					if (stand != null)
					{
						Object.Destroy(old.gameObject);
						return "Engine stand moved.";
					}
				}
				return "[" + key + "] Put the engine stand here";
			}
			BoughtStand target = looking ? hit.collider.GetComponentInParent<BoughtStand>() : null;
			if (target == null || !Input.GetKeyDown(key))
			{
				return null;
			}
			if (EngineOn(target) != null)
			{
				return "Take the engine off the stand before moving it.";
			}
			moving = target;
			return "[" + key + "] Put the engine stand here";
		}

		// The engine block jointed to this stand, if any.
		public static GameObject EngineOn(Component stand)
		{
			foreach (FixedJoint j in Object.FindObjectsOfType<FixedJoint>())
			{
				if (j.connectedBody != null && j.connectedBody.name == "DK_9" && j.connectedBody.transform.IsChildOf(stand.transform) && EngineClonerMod.IsEngineBlock(j.gameObject))
				{
					return j.gameObject;
				}
			}
			return null;
		}

		// The stand rigidbody ("DK_9") nearest a point: the one you used.
		public static Rigidbody NearestStand(Vector3 point)
		{
			Rigidbody best = null;
			float bestDist = float.MaxValue;
			foreach (Rigidbody rb in Object.FindObjectsOfType<Rigidbody>())
			{
				if (rb.name != "DK_9")
				{
					continue;
				}
				float d = (rb.position - point).sqrMagnitude;
				if (d < bestDist)
				{
					best = rb;
					bestDist = d;
				}
			}
			return best;
		}

		// --- Saving (with the engines, in UserData/EngineCloner). ---

		private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

		public static void SaveLines(StringBuilder sb)
		{
			foreach (BoughtStand s in Object.FindObjectsOfType<BoughtStand>())
			{
				Transform t = s.transform;
				Vector3 f = t.forward;
				sb.Append("STAND;").Append(F(t.position.x)).Append(';').Append(F(t.position.y)).Append(';').Append(F(t.position.z)).Append(';').Append(F(f.x)).Append(';').Append(F(f.z)).Append('\n');
			}
		}

		public static int Load(string[] lines)
		{
			int n = 0;
			foreach (string line in lines)
			{
				string[] f = line.Split(';');
				if (f.Length < 6 || f[0] != "STAND")
				{
					continue;
				}
				try
				{
					Vector3 p = new Vector3(P(f[1]), P(f[2]), P(f[3]));
					if (Build(p, new Vector3(P(f[4]), 0f, P(f[5]))) != null)
					{
						n++;
					}
				}
				catch (Exception e)
				{
					EngineClonerMod.Log("Skipping a bad engine stand save line: " + e.Message);
				}
			}
			return n;
		}

		private static string F(float v)
		{
			return v.ToString("R", Inv);
		}

		private static float P(string s)
		{
			return float.Parse(s, NumberStyles.Float, Inv);
		}
	}

	// On a stand this mod built: so it's saved and can be moved.
	public class BoughtStand : MonoBehaviour
	{
	}

	// What a shop delivers: builds a stand on the ground here, facing the player.
	public class StandDelivery : MonoBehaviour
	{
		private void Start()
		{
			Camera cam = Camera.main;
			Vector3 face = cam != null ? cam.transform.position - transform.position : Vector3.forward;
			if (StandShop.Build(transform.position, face) == null)
			{
				EngineClonerMod.Log("Couldn't deliver the engine stand: there's no stand in this level to copy.");
			}
			Destroy(gameObject);
		}
	}

	// --- The game's stand code, for more than one stand. ---

	// Snapping a block onto a stand joints it to GameObject.Find("DK_9"): the first
	// stand found. Joint it to the stand it was snapped onto instead.
	[HarmonyPatch(typeof(PickUp), "LetGo")]
	internal static class StandSnapPatch
	{
		private static void Postfix(PickUp __instance)
		{
			if (__instance == null || !EngineClonerMod.IsEngineBlock(__instance.gameObject))
			{
				return;
			}
			FixedJoint joint = __instance.GetComponent<FixedJoint>();
			if (joint == null || joint.connectedBody == null || joint.connectedBody.name != "DK_9")
			{
				return;
			}
			Rigidbody nearest = StandShop.NearestStand(__instance.transform.position);
			if (nearest != null && nearest != joint.connectedBody)
			{
				joint.connectedBody = nearest;
			}
		}
	}

	// Clicking a stand turns Interactor.EngStand: the first stand's hinge. Turn the
	// hinge of the stand that was clicked.
	[HarmonyPatch(typeof(Interactor), "RotateEngineStand")]
	internal static class StandRotatePatch
	{
		private static string hingeName;

		private static void Prefix(Interactor __instance, RaycastHit ___hit)
		{
			if (__instance.EngStand == null || ___hit.collider == null)
			{
				return;
			}
			if (hingeName == null)
			{
				hingeName = __instance.EngStand.name;
			}
			GameObject best = null;
			float bestDist = float.MaxValue;
			foreach (HingeJoint h in Object.FindObjectsOfType<HingeJoint>())
			{
				if (h.name != hingeName)
				{
					continue;
				}
				float d = (h.transform.position - ___hit.point).sqrMagnitude;
				if (d < bestDist)
				{
					best = h.gameObject;
					bestDist = d;
				}
			}
			if (best != null)
			{
				__instance.EngStand = best;
			}
		}
	}
}
