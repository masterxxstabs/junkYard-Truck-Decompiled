using System;
using System.Collections.Generic;
using ch.sycoforge.Decal;
using ch.sycoforge.Decal.Projectors.Geometry;
using UnityEngine;

// Token: 0x02000144 RID: 324
public class RuntimeDecalCombiner
{
	// Token: 0x0600084F RID: 2127 RVA: 0x0006D844 File Offset: 0x0006BA44
	public static List<GameObject> Combine(IList<EasyDecal> decals)
	{
		Dictionary<DecalTextureAtlas, List<EasyDecal>> dictionary = new Dictionary<DecalTextureAtlas, List<EasyDecal>>();
		foreach (EasyDecal easyDecal in decals)
		{
			if (easyDecal.Source == SourceMode.Atlas && easyDecal.Projector != null)
			{
				if (!dictionary.ContainsKey(easyDecal.Atlas))
				{
					dictionary.Add(easyDecal.Atlas, new List<EasyDecal>());
				}
				dictionary[easyDecal.Atlas].Add(easyDecal);
			}
		}
		return RuntimeDecalCombiner.Combine(dictionary);
	}

	// Token: 0x06000850 RID: 2128 RVA: 0x0006D8D4 File Offset: 0x0006BAD4
	private static List<GameObject> Combine(Dictionary<DecalTextureAtlas, List<EasyDecal>> mappings)
	{
		List<GameObject> list = new List<GameObject>();
		if (mappings.Count > 0)
		{
			foreach (DecalTextureAtlas decalTextureAtlas in mappings.Keys)
			{
				IList<EasyDecal> list2 = mappings[decalTextureAtlas];
				foreach (EasyDecal easyDecal in list2)
				{
					GameObject gameObject = RuntimeDecalCombiner.Combine(list2, decalTextureAtlas);
					if (gameObject != null)
					{
						list.Add(gameObject);
					}
				}
			}
		}
		return list;
	}

	// Token: 0x06000851 RID: 2129 RVA: 0x0006D988 File Offset: 0x0006BB88
	private static GameObject Combine(IList<EasyDecal> decals, DecalTextureAtlas atlas)
	{
		if (decals.Count > 0)
		{
			DynamicMesh dynamicMesh = new DynamicMesh(DecalBase.DecalRoot, RecreationMode.Always);
			GameObject gameObject = new GameObject(string.Format("Combined Decals Root [{0}]", atlas.name));
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			foreach (EasyDecal easyDecal in decals)
			{
				if (easyDecal.Source == SourceMode.Atlas && easyDecal.Projector != null)
				{
					dynamicMesh.Add(easyDecal.Projector.Mesh, easyDecal.LocalToWorldMatrix, gameObject.transform.worldToLocalMatrix);
					easyDecal.gameObject.SetActive(false);
				}
			}
			meshRenderer.material = atlas.Material;
			meshFilter.sharedMesh = dynamicMesh.ConvertToMesh(null, false);
		}
		return null;
	}
}
