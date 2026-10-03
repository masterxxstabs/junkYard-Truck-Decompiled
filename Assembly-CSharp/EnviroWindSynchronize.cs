using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200009F RID: 159
public class EnviroWindSynchronize : MonoBehaviour
{
	// Token: 0x06000372 RID: 882 RVA: 0x0001F343 File Offset: 0x0001D543
	private void Start()
	{
		if (this.syncTerrainGrassWind && this.terrains.Count > 0)
		{
			Debug.Log("Please assign Terrain, or deactivate 'syncTerrainGrassWind'!");
			base.enabled = false;
		}
	}

	// Token: 0x06000373 RID: 883 RVA: 0x0001F36C File Offset: 0x0001D56C
	private void Update()
	{
		if (this.syncTerrainGrassWind)
		{
			for (int i = 0; i < this.terrains.Count; i++)
			{
				this.terrains[i].terrainData.wavingGrassStrength = Mathf.Lerp(this.terrains[i].terrainData.wavingGrassStrength, EnviroSkyMgr.instance.Components.windZone.windMain, Time.deltaTime * this.windChangingSpeed);
			}
		}
	}

	// Token: 0x04000792 RID: 1938
	[Header("Terrain Grass")]
	public bool syncTerrainGrassWind = true;

	// Token: 0x04000793 RID: 1939
	public List<Terrain> terrains = new List<Terrain>();

	// Token: 0x04000794 RID: 1940
	[Header("Speed")]
	[Range(0f, 10f)]
	public float windChangingSpeed = 1f;
}
