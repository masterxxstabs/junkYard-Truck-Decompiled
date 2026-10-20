using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace JunkyardRanger
{
	// Spray-painting a Ranger. The game paints the Diamondback's panels by name
	// (Interactor.PaintSurface) and stores the colors on the truck the game is
	// wired to. The Ranger's panels are its own parts, so a spray at a Ranger
	// paints the matching part of its body (cab and bed, hood, either door,
	// tailgate) and is saved with the Ranger, never over the Diamondback's colors.
	[HarmonyPatch(typeof(Interactor), "PaintSurface")]
	internal static class RangerPaintPatch
	{
		private static readonly FieldInfo Range = AccessTools.Field(typeof(Interactor), "interactRange");
		private static readonly FieldInfo Cam = AccessTools.Field(typeof(Interactor), "cam");

		private static bool Prefix(Interactor __instance, RaycastHit ___hit)
		{
			if (___hit.collider == null || !___hit.collider.gameObject.name.Contains("spraypaint"))
			{
				return true;
			}
			GameObject can = ___hit.collider.gameObject;
			Renderer canRenderer = can.GetComponent<Renderer>();
			Camera cam = Cam != null ? Cam.GetValue(__instance) as Camera : null;
			if (canRenderer == null || cam == null)
			{
				return true;
			}
			// Look past the can, like the game does.
			float range = Range != null && Range.GetValue(__instance) is float ? (float)Range.GetValue(__instance) : 3f;
			BoxCollider box = can.GetComponent<BoxCollider>();
			bool wasOn = box != null && box.enabled;
			if (box != null)
			{
				box.enabled = false;
			}
			RaycastHit target;
			bool found = Physics.Raycast(cam.transform.position, cam.transform.forward, out target, range > 0f ? range : 3f);
			if (box != null)
			{
				box.enabled = wasOn;
			}
			RangerBody body = found ? target.collider.GetComponentInParent<RangerBody>() : null;
			if (body == null)
			{
				return true;
			}
			string group = GroupFor(target.collider.transform, body.transform);
			// Two-tone: below the line along the sides is the lower color.
			if (group != null && group != "Hood" && body.transform.InverseTransformPoint(target.point).y < RangerModel.TwoToneLine)
			{
				group = "Lower";
			}
			if (group == null)
			{
				return true; // not a body panel (wheels, engine...): the game's own painting
			}
			Material m = canRenderer.material;
			float metallic = m.HasProperty("_Metallic") ? m.GetFloat("_Metallic") : 0f;
			float gloss = m.HasProperty("_Glossiness") ? m.GetFloat("_Glossiness") : 0.5f;
			body.SetPaint(group, m.color, metallic, gloss);
			if (__instance.paSources != null && __instance.playerAudio != null && __instance.playerAudio.Length > 7)
			{
				__instance.paSources.PlayOneShot(__instance.playerAudio[7], 1f);
			}
			return false;
		}

		// Which paint group a hit on the Diamondback's (hidden) panels means.
		private static string GroupFor(Transform hit, Transform root)
		{
			for (Transform t = hit; t != null && t != root; t = t.parent)
			{
				switch (t.name)
				{
				case "hood_low1":
					return "Hood";
				case "cardoor_d":
					return "DoorD";
				case "cardoor_p":
					return "DoorP";
				case "tailgate":
					return "Tailgate";
				case "cabin_low":
				case "body_low":
					return "Body";
				}
			}
			return null;
		}
	}
}
