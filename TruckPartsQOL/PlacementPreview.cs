using System.Collections.Generic;
using UnityEngine;

namespace TruckPartsQOL
{
	// While you hold a part and look at a spot on a vehicle: a see-through green
	// copy of it, exactly where and how it will be fitted.
	internal static class PlacementPreview
	{
		private static GameObject ghost;
		private static TruckPart of;
		private static Material material;

		public static void Show(TruckPart part, Vector3 position, Quaternion rotation)
		{
			if (ghost == null || of != part)
			{
				Build(part);
			}
			ghost.transform.SetPositionAndRotation(position, rotation);
			ghost.SetActive(true);
		}

		public static void Hide()
		{
			if (ghost != null)
			{
				ghost.SetActive(false);
			}
		}

		private static void Build(TruckPart part)
		{
			if (ghost != null)
			{
				Object.Destroy(ghost);
			}
			of = part;
			ghost = new GameObject("TruckPartsQOL_Preview");
			Transform root = part.transform;
			foreach (MeshFilter filter in part.GetComponentsInChildren<MeshFilter>())
			{
				Renderer r = filter.GetComponent<Renderer>();
				if (filter.sharedMesh == null || r == null || !r.enabled || UnderBolts(filter.transform, root))
				{
					continue;
				}
				GameObject copy = new GameObject(filter.name);
				copy.transform.SetParent(ghost.transform, false);
				copy.transform.localPosition = root.InverseTransformPoint(filter.transform.position);
				copy.transform.localRotation = Quaternion.Inverse(root.rotation) * filter.transform.rotation;
				copy.transform.localScale = filter.transform.lossyScale;
				copy.AddComponent<MeshFilter>().sharedMesh = filter.sharedMesh;
				Material[] mats = new Material[filter.sharedMesh.subMeshCount];
				for (int i = 0; i < mats.Length; i++)
				{
					mats[i] = Material();
				}
				copy.AddComponent<MeshRenderer>().sharedMaterials = mats;
			}
			ghost.SetActive(false);
		}

		// The bolts aren't part of the look (they show once it's fitted).
		private static bool UnderBolts(Transform t, Transform root)
		{
			for (; t != null && t != root; t = t.parent)
			{
				if (t.name == "Bolts")
				{
					return true;
				}
			}
			return false;
		}

		// Green glass: the game's Standard shader (the primitives' material) in
		// fade mode; if that variant isn't in the game, it's solid green instead.
		private static Material Material()
		{
			if (material != null)
			{
				return material;
			}
			GameObject probe = GameObject.CreatePrimitive(PrimitiveType.Cube);
			material = new Material(probe.GetComponent<Renderer>().sharedMaterial);
			Object.Destroy(probe);
			material.name = "TruckPartsQOL_Preview";
			material.SetFloat("_Mode", 2f);
			material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			material.SetInt("_ZWrite", 0);
			material.DisableKeyword("_ALPHATEST_ON");
			material.EnableKeyword("_ALPHABLEND_ON");
			material.renderQueue = 3000;
			material.color = new Color(0.35f, 1f, 0.45f, 0.4f);
			material.EnableKeyword("_EMISSION");
			material.SetColor("_EmissionColor", new Color(0.1f, 0.5f, 0.15f));
			Object.DontDestroyOnLoad(material);
			return material;
		}
	}
}
