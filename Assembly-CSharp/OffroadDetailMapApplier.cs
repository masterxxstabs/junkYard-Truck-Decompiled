using System;
using UnityEngine;

// Token: 0x020000E1 RID: 225
public class OffroadDetailMapApplier : MonoBehaviour
{
	// Token: 0x060005A7 RID: 1447 RVA: 0x00045F98 File Offset: 0x00044198
	public void ApplyDetailMaps()
	{
		foreach (Material material in this.materials)
		{
			material.SetTexture("_DetailAlbedoMap", this.DiffuseMap);
			material.SetTexture("_DetailNormalMap", this.NormalMap);
		}
	}

	// Token: 0x060005A8 RID: 1448 RVA: 0x00045FE0 File Offset: 0x000441E0
	public void DeleteDetailMaps()
	{
		foreach (Material material in this.materials)
		{
			material.SetTexture("_DetailAlbedoMap", null);
			material.SetTexture("_DetailNormalMap", null);
		}
	}

	// Token: 0x060005A9 RID: 1449 RVA: 0x0004601C File Offset: 0x0004421C
	public void RandomizeDetailMaps()
	{
		Material[] array = this.materials;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetTextureOffset("_DetailAlbedoMap", new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)));
		}
	}

	// Token: 0x04000C22 RID: 3106
	public Material[] materials;

	// Token: 0x04000C23 RID: 3107
	public Texture DiffuseMap;

	// Token: 0x04000C24 RID: 3108
	public Texture NormalMap;
}
