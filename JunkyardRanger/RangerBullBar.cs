using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JunkyardRanger
{
	// The Ranger's bull bar, fitted the game's own way. The Diamondback has an
	// empty brush guard slot ("brushguard_e": a hidden part with a trigger, four
	// frame bolts and a durability); on a Ranger the slot shows the bull bar
	// instead and is renamed, so only the bull bar fits it (the Diamondback's own
	// brush guard doesn't fit a Ranger, nor the bull bar a Diamondback).
	//
	// Carry the bull bar to the front of a Ranger: the game shows its snap mark,
	// let go and it pops on; then tighten the four bolts with the ratchet. Loosen
	// them all and pull it off, and the game hands back the loose bull bar.
	internal static class RangerBullBar
	{
		public const string GameSlot = "brushguard_e";
		// Must start with "brus": the game's snap code treats those as fitted to a
		// vehicle, not to another part.
		public const string Slot = "brushguardR_e";
		public const string ItemName = "rangerbullbar";
		public const string ShopName = "Ranger - Bull bar";
		public const float Mass = 28f;

		private static GameObject holder;
		private static GameObject template;

		// On a new copy, before any snapshot is applied: the slot shows the bull bar.
		public static void FitSlot(GameObject copy)
		{
			Transform slot = copy.transform.Find(GameSlot);
			if (slot == null)
			{
				slot = copy.transform.Find(Slot);
			}
			Mesh mesh;
			Material[] materials;
			if (slot == null || !RangerModel.BullBar(out mesh, out materials))
			{
				if (slot == null)
				{
					RangerMod.Log("The Diamondback has no brush guard slot; this Ranger can't take a bull bar.");
				}
				return;
			}
			slot.name = Slot;
			MeshFilter filter = slot.GetComponent<MeshFilter>() ?? slot.gameObject.AddComponent<MeshFilter>();
			filter.sharedMesh = mesh;
			MeshRenderer renderer = slot.GetComponent<MeshRenderer>();
			if (renderer != null)
			{
				renderer.sharedMaterials = materials;
			}
			Bounds b = mesh.bounds;
			foreach (BoxCollider box in slot.GetComponents<BoxCollider>())
			{
				box.center = b.center;
				// The trigger reaches a little past the bar, so the snap mark shows
				// as you bring it up to the front.
				box.size = box.isTrigger ? b.size + Vector3.one * 0.3f : b.size;
			}
			durability d = slot.GetComponent<durability>();
			if (d != null)
			{
				d.template = Template();
			}
		}

		// A new Ranger comes without one (the Diamondback it's copied from may have
		// its brush guard on).
		public static void Unfit(GameObject copy)
		{
			Transform slot = copy.transform.Find(Slot);
			if (slot == null)
			{
				return;
			}
			Renderer renderer = slot.GetComponent<Renderer>();
			if (renderer != null)
			{
				renderer.enabled = false;
			}
			foreach (BoxCollider box in slot.GetComponents<BoxCollider>())
			{
				box.enabled = box.isTrigger;
			}
			durability d = slot.GetComponent<durability>();
			if (d != null)
			{
				d.health = 0f;
				d.canDetach = false;
			}
			foreach (Transform child in slot)
			{
				if (child.name != "bolt")
				{
					continue;
				}
				Renderer r = child.GetComponent<Renderer>();
				if (r != null)
				{
					r.enabled = false;
				}
				Collider c = child.GetComponent<Collider>();
				if (c != null)
				{
					c.enabled = false;
				}
				BoltScript bolt = child.GetComponent<BoltScript>();
				if (bolt != null)
				{
					bolt.boltturns = 0;
				}
			}
		}

		// The loose bull bar: a dormant template (under an inactive holder); the
		// shops and the game's part removal make live copies of it. Null without
		// the body files.
		public static GameObject Template()
		{
			if (template != null)
			{
				return template;
			}
			Mesh mesh;
			Material[] materials;
			string why;
			if (!RangerModel.Load(out why) || !RangerModel.BullBar(out mesh, out materials))
			{
				return null;
			}
			if (holder == null)
			{
				holder = new GameObject("JunkyardRanger_BullBar");
				holder.SetActive(false);
				Object.DontDestroyOnLoad(holder);
			}
			GameObject go = new GameObject(ItemName);
			go.transform.SetParent(holder.transform, false);
			go.AddComponent<MeshFilter>().sharedMesh = mesh;
			go.AddComponent<MeshRenderer>().sharedMaterials = materials;
			Bounds b = mesh.bounds;
			// PickUp wants the solid box first, then the trigger it snaps with.
			BoxCollider solid = go.AddComponent<BoxCollider>();
			solid.center = b.center;
			solid.size = b.size;
			BoxCollider trigger = go.AddComponent<BoxCollider>();
			trigger.center = b.center;
			trigger.size = b.size;
			trigger.isTrigger = true;
			Rigidbody body = go.AddComponent<Rigidbody>();
			body.mass = Mass;
			body.interpolation = RigidbodyInterpolation.Interpolate;
			PickUp pick = go.AddComponent<PickUp>();
			InitLikeUnity(pick);
			pick.attachTo = Slot;
			pick.pickable = true;
			pick.price = 0f;
			pick.thisDurability = 100f;
			pick.numBolts = 4f;
			pick.trueMass = Mass;
			pick.description = "Bull bar (2004 Ford Ranger)";
			go.AddComponent<RangerBullBarItem>();
			template = go;
			return template;
		}

		// A loose bull bar in the world (bought, or as saved).
		public static GameObject Spawn(Vector3 position, Quaternion rotation, float health)
		{
			GameObject t = Template();
			if (t == null)
			{
				return null;
			}
			GameObject go = Object.Instantiate(t, position, rotation);
			go.name = ItemName;
			go.GetComponent<PickUp>().thisDurability = health;
			return go;
		}

		// A new scene: a new template, with the materials of what's loaded now.
		public static void Reset()
		{
			if (template != null)
			{
				Object.Destroy(template);
			}
			template = null;
		}

		// A component added from code starts with null strings and arrays; one
		// loaded with a scene has "" and empty arrays, which PickUp expects (it
		// checks attachTo2 != "" and so on).
		private static void InitLikeUnity(Component component)
		{
			for (Type type = component.GetType(); type != null && type != typeof(MonoBehaviour); type = type.BaseType)
			{
				foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
				{
					bool serialized = field.IsPublic && !field.IsNotSerialized || field.IsDefined(typeof(SerializeField), false);
					if (!serialized || field.GetValue(component) != null)
					{
						continue;
					}
					Type fieldType = field.FieldType;
					if (fieldType == typeof(string))
					{
						field.SetValue(component, "");
					}
					else if (fieldType.IsArray)
					{
						field.SetValue(component, Array.CreateInstance(fieldType.GetElementType(), 0));
					}
					else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
					{
						field.SetValue(component, Activator.CreateInstance(fieldType));
					}
				}
			}
		}
	}

	// Marks a loose bull bar (copied along with it), so it's saved with the Rangers.
	public class RangerBullBarItem : MonoBehaviour
	{
		// Shops drop items at the player's feet: lift it clear of the ground once.
		private void Start()
		{
			if (transform.parent != null)
			{
				return;
			}
			float lowest = float.MinValue;
			foreach (RaycastHit hit in Physics.RaycastAll(transform.position + Vector3.up * 2f, Vector3.down, 6f, ~0, QueryTriggerInteraction.Ignore))
			{
				if (!hit.transform.IsChildOf(transform) && hit.point.y > lowest && hit.collider.GetComponentInParent<CharacterController>() == null)
				{
					lowest = hit.point.y;
				}
			}
			Renderer r = GetComponent<Renderer>();
			if (lowest > float.MinValue && r != null && r.bounds.min.y < lowest + 0.05f)
			{
				transform.position += Vector3.up * (lowest + 0.05f - r.bounds.min.y);
			}
		}
	}
}
