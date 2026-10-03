using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TruckPartsQOL
{
	// Spray-painting rims and turbos with the game's paint cans.
	//
	// The game paints in Interactor.PaintSurface(): it takes the color, metallic
	// and smoothness of the can in your hand, raycasts past the can, and matches
	// the target by name (body panels, the V8 block, valve covers, heads...).
	// Rims and turbos aren't on its list, so a prefix handles them first.
	//
	// Turbos are engine parts like valve covers: their durability slot (fitted)
	// or PickUp (loose) has painted/red/green/blue/metallic/smoothness/paintSlot
	// fields that the game itself copies across when the part is unbolted or bolted
	// on, and re-applies on load. So turbos are painted exactly the way the game
	// paints a valve cover.
	//
	// Rims have no paint fields. A loose wheel has a TireAssign whose children are
	// the models: 0 the stock rim, 1-5 the other rims, 6+ the tires. A mounted
	// wheel's slot has a WHEEL_HOLDER child laid out the same way (rims 1-5). The
	// active rim model gets a RimPaint, which is carried over when a wheel is
	// mounted or taken off, and saved by this mod.
	internal static class Painting
	{
		public static bool TryPaint(Interactor interactor, RaycastHit canHit)
		{
			GameObject can = canHit.collider.gameObject;
			Renderer canRenderer = can.GetComponent<Renderer>();
			if (canRenderer == null)
			{
				return false;
			}
			Material canMaterial = canRenderer.material;
			PaintColor paint = new PaintColor(canMaterial.color.r, canMaterial.color.g, canMaterial.color.b, Get(canMaterial, "_Metallic"), Get(canMaterial, "_Glossiness"));

			// Look past the can, like the game does.
			Camera cam = Camera.main;
			if (cam == null)
			{
				return false;
			}
			BoxCollider canBox = can.GetComponent<BoxCollider>();
			bool wasEnabled = canBox != null && canBox.enabled;
			if (canBox != null)
			{
				canBox.enabled = false;
			}
			RaycastHit target;
			bool found = Physics.Raycast(cam.transform.position, cam.transform.forward, out target, InteractRange(interactor), Physics.DefaultRaycastLayers);
			if (canBox != null)
			{
				canBox.enabled = wasEnabled;
			}
			if (!found)
			{
				return false;
			}
			bool painted = PaintTurbo(target.collider.gameObject, paint) || PaintRim(target.collider.transform, paint);
			if (painted && interactor.paSources != null && interactor.playerAudio != null && interactor.playerAudio.Length > 7)
			{
				interactor.paSources.PlayOneShot(interactor.playerAudio[7], 1f);
			}
			return painted;
		}

		private static float InteractRange(Interactor interactor)
		{
			FieldInfo f = typeof(Interactor).GetField("interactRange", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			object v = f != null ? f.GetValue(interactor) : null;
			return v is float && (float)v > 0f ? (float)v : 3f;
		}

		private static float Get(Material m, string property)
		{
			return m.HasProperty(property) ? m.GetFloat(property) : 0f;
		}

		// --- Turbos. ---

		private static readonly string[] NotTurbos = { "pipe", "gauge", "tach", "filter", "intercooler", "oilcooler" };

		public static bool IsTurbo(GameObject go)
		{
			string name = go.name.ToLowerInvariant();
			if (!name.Contains("turbo"))
			{
				return false;
			}
			foreach (string word in NotTurbos)
			{
				if (name.Contains(word))
				{
					return false;
				}
			}
			return go.GetComponent<durability>() != null || go.GetComponent<PickUp>() != null;
		}

		private static bool PaintTurbo(GameObject go, PaintColor paint)
		{
			if (!IsTurbo(go))
			{
				return false;
			}
			Renderer renderer = go.GetComponent<Renderer>();
			if (renderer == null || !renderer.enabled)
			{
				return false; // an empty turbo slot
			}
			durability slot = go.GetComponent<durability>();
			PickUp loose = go.GetComponent<PickUp>();
			int paintSlot = slot != null ? slot.paintSlot : loose.paintSlot;
			Material[] materials = renderer.materials;
			if (paintSlot < 0 || paintSlot >= materials.Length)
			{
				paintSlot = 0;
			}
			paint.ApplyTo(materials[paintSlot]);
			// The fields the game copies on unbolt / bolt-on and re-applies on load.
			if (slot != null)
			{
				slot.painted = true;
				slot.red = paint.r;
				slot.green = paint.g;
				slot.blue = paint.b;
				slot.metallic = paint.metallic;
				slot.smoothness = paint.smoothness;
			}
			if (loose != null)
			{
				loose.painted = true;
				loose.red = paint.r;
				loose.green = paint.g;
				loose.blue = paint.b;
				loose.metallic = paint.metallic;
				loose.smoothness = paint.smoothness;
			}
			return true;
		}

		// --- Rims. ---

		private static bool IsWheel(Transform t)
		{
			return t.GetComponent<TireAssign>() != null || t.name == "WHEEL_HOLDER";
		}

		// The object holding the rim/tire models: what was hit, a parent of it (a
		// rim model was hit), or a child of it (the mounted slot was hit).
		public static Transform WheelOf(Transform hit)
		{
			Transform t = hit;
			for (int i = 0; i < 3 && t != null; i++, t = t.parent)
			{
				if (IsWheel(t))
				{
					return t;
				}
			}
			foreach (Transform child in hit)
			{
				if (IsWheel(child))
				{
					return child;
				}
				foreach (Transform grandchild in child)
				{
					if (IsWheel(grandchild))
					{
						return grandchild;
					}
				}
			}
			return null;
		}

		// The rim model that's showing: one of children 1-5, else (on a loose wheel)
		// the stock rim, child 0.
		public static Transform ActiveRim(Transform wheel)
		{
			for (int i = 1; i < 6 && i < wheel.childCount; i++)
			{
				if (wheel.GetChild(i).gameObject.activeSelf)
				{
					return wheel.GetChild(i);
				}
			}
			bool stockRimAtZero = wheel.GetComponent<TireAssign>() != null;
			return stockRimAtZero && wheel.childCount > 0 && wheel.GetChild(0).gameObject.activeSelf ? wheel.GetChild(0) : null;
		}

		private static bool PaintRim(Transform hit, PaintColor paint)
		{
			Transform wheel = WheelOf(hit);
			if (wheel == null)
			{
				return false;
			}
			Transform rim = ActiveRim(wheel);
			if (rim == null)
			{
				return false;
			}
			RimPaint.Set(rim, paint);
			return true;
		}

		// A rim index (0-5) and its paint, carried from one wheel object to another.
		public static void CopyRim(Transform fromWheel, int index, Transform toWheel)
		{
			if (fromWheel == null || toWheel == null || index < 0 || index >= toWheel.childCount)
			{
				return;
			}
			RimPaint from = index < fromWheel.childCount ? fromWheel.GetChild(index).GetComponent<RimPaint>() : null;
			Transform to = toWheel.GetChild(index);
			if (from != null)
			{
				RimPaint.Set(to, from.Color);
			}
			else
			{
				RimPaint.Clear(to);
			}
		}
	}

	public struct PaintColor
	{
		public float r;
		public float g;
		public float b;
		public float metallic;
		public float smoothness;

		public PaintColor(float r, float g, float b, float metallic, float smoothness)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.metallic = metallic;
			this.smoothness = smoothness;
		}

		public void ApplyTo(Material m)
		{
			m.color = new Color(r, g, b, 1f);
			if (m.HasProperty("_Metallic"))
			{
				m.SetFloat("_Metallic", metallic);
			}
			if (m.HasProperty("_Glossiness"))
			{
				m.SetFloat("_Glossiness", smoothness);
			}
		}
	}

	// Paint on one rim model (a child of a wheel). Remembers the original look so
	// the paint can be taken off again, e.g. when an unpainted rim is mounted in its
	// place. Public fields: copied if the vehicle is cloned.
	public class RimPaint : MonoBehaviour
	{
		public float r;
		public float g;
		public float b;
		public float metallic;
		public float smoothness;

		public bool hasOriginal;
		public float or;
		public float og;
		public float ob;
		public float om;
		public float os;

		public PaintColor Color
		{
			get { return new PaintColor(r, g, b, metallic, smoothness); }
		}

		public static void Set(Transform rim, PaintColor paint)
		{
			RimPaint rp = rim.GetComponent<RimPaint>();
			if (rp == null)
			{
				rp = rim.gameObject.AddComponent<RimPaint>();
			}
			if (!rp.hasOriginal)
			{
				Material m = FirstMaterial(rim);
				if (m != null)
				{
					rp.hasOriginal = true;
					rp.or = m.color.r;
					rp.og = m.color.g;
					rp.ob = m.color.b;
					rp.om = m.HasProperty("_Metallic") ? m.GetFloat("_Metallic") : 0f;
					rp.os = m.HasProperty("_Glossiness") ? m.GetFloat("_Glossiness") : 0f;
				}
			}
			rp.r = paint.r;
			rp.g = paint.g;
			rp.b = paint.b;
			rp.metallic = paint.metallic;
			rp.smoothness = paint.smoothness;
			rp.Apply();
		}

		public static void Clear(Transform rim)
		{
			RimPaint rp = rim.GetComponent<RimPaint>();
			if (rp == null)
			{
				return;
			}
			if (rp.hasOriginal)
			{
				foreach (Renderer renderer in rim.GetComponentsInChildren<Renderer>(true))
				{
					new PaintColor(rp.or, rp.og, rp.ob, rp.om, rp.os).ApplyTo(renderer.material);
				}
			}
			Destroy(rp);
		}

		// The rim's main material (slot 0) on every renderer of the rim model.
		public void Apply()
		{
			foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
			{
				Color.ApplyTo(renderer.material);
			}
		}

		private void Start()
		{
			Apply();
		}

		private static Material FirstMaterial(Transform rim)
		{
			Renderer renderer = rim.GetComponentInChildren<Renderer>(true);
			return renderer != null ? renderer.sharedMaterial : null;
		}
	}

	[HarmonyPatch(typeof(Interactor), "PaintSurface")]
	internal static class PaintSurfacePatch
	{
		private static bool Prefix(Interactor __instance, RaycastHit ___hit)
		{
			try
			{
				if (___hit.collider == null || !___hit.collider.gameObject.name.Contains("spraypaint"))
				{
					return true;
				}
				// Painted a rim or turbo: done. Anything else: the game's own code.
				return !Painting.TryPaint(__instance, ___hit);
			}
			catch (Exception e)
			{
				TruckPartsQOLMod.Log("Painting failed: " + e.Message);
				return true;
			}
		}
	}

	// Mounting a wheel: PickUp.LetGo switches on the loose wheel's rim index in the
	// wheel slot and destroys the loose wheel. Carry the rim's paint over.
	[HarmonyPatch(typeof(PickUp), "LetGo")]
	internal static class MountWheelPaintPatch
	{
		private struct State
		{
			public Transform wheel;
			public Transform slot;
			public int rim;
			public bool slotRimWasOn;
		}

		private static void Prefix(PickUp __instance, GameObject ___validTrigObject, string ___validTrigName, bool ___canSnap, out State __state)
		{
			__state = default(State);
			__state.rim = -1;
			if (!___canSnap || ___validTrigObject == null || ___validTrigName == null || !___validTrigName.Contains("truckwheel") || __instance.GetComponent<TireAssign>() == null)
			{
				return;
			}
			Transform rim = Painting.ActiveRim(__instance.transform);
			Transform slot = Painting.WheelOf(___validTrigObject.transform);
			if (rim == null || slot == null || rim.GetSiblingIndex() >= slot.childCount)
			{
				return;
			}
			__state.wheel = __instance.transform;
			__state.slot = slot;
			__state.rim = rim.GetSiblingIndex();
			__state.slotRimWasOn = slot.GetChild(__state.rim).gameObject.activeSelf;
		}

		private static void Postfix(State __state)
		{
			if (__state.rim < 0 || __state.slot == null || __state.wheel == null)
			{
				return;
			}
			// Only if the rim really went onto the slot just now.
			if (!__state.slotRimWasOn && __state.slot.GetChild(__state.rim).gameObject.activeSelf)
			{
				Painting.CopyRim(__state.wheel, __state.rim, __state.slot);
			}
		}
	}

	// Taking a wheel off: Interactor.Update instantiates a new loose wheel
	// (newTire) with the slot's rim index switched on. Carry the paint over.
	[HarmonyPatch(typeof(Interactor), "Update")]
	internal static class RemoveWheelPaintPatch
	{
		private struct State
		{
			public GameObject newTireBefore;
			public Transform slot;
		}

		private static void Prefix(GameObject ___newTire, RaycastHit ___hit, out State __state)
		{
			__state.newTireBefore = ___newTire;
			__state.slot = ___hit.collider != null ? Painting.WheelOf(___hit.collider.transform) : null;
		}

		private static void Postfix(GameObject ___newTire, State __state)
		{
			if (___newTire == null || ___newTire == __state.newTireBefore || __state.slot == null)
			{
				return;
			}
			TireAssign assign = ___newTire.GetComponent<TireAssign>();
			if (assign == null)
			{
				return;
			}
			int rim = assign.rimNumX;
			if (rim < 0 || rim >= __state.slot.childCount)
			{
				return;
			}
			RimPaint slotPaint = __state.slot.GetChild(rim).GetComponent<RimPaint>();
			if (slotPaint == null || __state.slot.GetChild(rim).gameObject.activeSelf)
			{
				return; // nothing painted, or the rim is still on the slot (tire-only removal)
			}
			Painting.CopyRim(__state.slot, rim, ___newTire.transform);
			RimPaint.Clear(__state.slot.GetChild(rim));
		}
	}
}
