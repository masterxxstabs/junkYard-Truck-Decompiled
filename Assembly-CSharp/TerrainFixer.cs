using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000146 RID: 326
public class TerrainFixer : MonoBehaviour
{
	// Token: 0x06000856 RID: 2134 RVA: 0x0006DA9F File Offset: 0x0006BC9F
	private void Start()
	{
		base.StartCoroutine(this.ApplyTerrainMaterialDelayed());
	}

	// Token: 0x06000857 RID: 2135 RVA: 0x0006DAAE File Offset: 0x0006BCAE
	private IEnumerator ApplyTerrainMaterialDelayed()
	{
		yield return null;
		yield return null;
		if (this.terrain != null && this.terrainMaterial != null)
		{
			this.terrain.materialTemplate = this.terrainMaterial;
			Debug.Log("Terrain material reassigned after initialization.");
		}
		yield break;
	}

	// Token: 0x0400134F RID: 4943
	public Terrain terrain;

	// Token: 0x04001350 RID: 4944
	public Material terrainMaterial;
}
