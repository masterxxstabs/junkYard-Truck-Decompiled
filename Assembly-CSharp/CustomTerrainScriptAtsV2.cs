using System;
using UnityEngine;

// Token: 0x0200010B RID: 267
public class CustomTerrainScriptAtsV2 : MonoBehaviour
{
	// Token: 0x060006FD RID: 1789 RVA: 0x0005A888 File Offset: 0x00058A88
	private void Start()
	{
		Terrain terrain = (Terrain)base.GetComponent(typeof(Terrain));
		if (this.Bump0)
		{
			Shader.SetGlobalTexture("_BumpMap0", this.Bump0);
		}
		if (this.Bump1)
		{
			Shader.SetGlobalTexture("_BumpMap1", this.Bump1);
		}
		if (this.Bump2)
		{
			Shader.SetGlobalTexture("_BumpMap2", this.Bump2);
		}
		if (this.Bump3)
		{
			Shader.SetGlobalTexture("_BumpMap3", this.Bump3);
		}
		Shader.SetGlobalFloat("_Tile0", this.Tile0);
		Shader.SetGlobalFloat("_Tile1", this.Tile1);
		Shader.SetGlobalFloat("_Tile2", this.Tile2);
		Shader.SetGlobalFloat("_Tile3", this.Tile3);
		this.terrainSizeX = terrain.terrainData.size.x;
		this.terrainSizeZ = terrain.terrainData.size.z;
		Shader.SetGlobalFloat("_TerrainX", this.terrainSizeX);
		Shader.SetGlobalFloat("_TerrainZ", this.terrainSizeZ);
	}

	// Token: 0x04000FC6 RID: 4038
	public Texture2D Bump0;

	// Token: 0x04000FC7 RID: 4039
	public Texture2D Bump1;

	// Token: 0x04000FC8 RID: 4040
	public Texture2D Bump2;

	// Token: 0x04000FC9 RID: 4041
	public Texture2D Bump3;

	// Token: 0x04000FCA RID: 4042
	public float Tile0;

	// Token: 0x04000FCB RID: 4043
	public float Tile1;

	// Token: 0x04000FCC RID: 4044
	public float Tile2;

	// Token: 0x04000FCD RID: 4045
	public float Tile3;

	// Token: 0x04000FCE RID: 4046
	public float terrainSizeX;

	// Token: 0x04000FCF RID: 4047
	public float terrainSizeZ;
}
