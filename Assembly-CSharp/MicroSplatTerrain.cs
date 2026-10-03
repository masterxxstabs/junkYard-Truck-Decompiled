using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000F4 RID: 244
[ExecuteInEditMode]
[DisallowMultipleComponent]
public class MicroSplatTerrain : MicroSplatObject
{
	// Token: 0x1400000E RID: 14
	// (add) Token: 0x06000612 RID: 1554 RVA: 0x000494D0 File Offset: 0x000476D0
	// (remove) Token: 0x06000613 RID: 1555 RVA: 0x00049504 File Offset: 0x00047704
	public static event MicroSplatTerrain.MaterialSyncAll OnMaterialSyncAll;

	// Token: 0x1400000F RID: 15
	// (add) Token: 0x06000614 RID: 1556 RVA: 0x00049538 File Offset: 0x00047738
	// (remove) Token: 0x06000615 RID: 1557 RVA: 0x00049570 File Offset: 0x00047770
	public event MicroSplatTerrain.MaterialSync OnMaterialSync;

	// Token: 0x06000616 RID: 1558 RVA: 0x000495A5 File Offset: 0x000477A5
	private void Awake()
	{
		this.terrain = base.GetComponent<Terrain>();
	}

	// Token: 0x06000617 RID: 1559 RVA: 0x000495B3 File Offset: 0x000477B3
	private void OnEnable()
	{
		this.terrain = base.GetComponent<Terrain>();
		MicroSplatTerrain.sInstances.Add(this);
		if (this.reenabled)
		{
			this.Sync();
		}
	}

	// Token: 0x06000618 RID: 1560 RVA: 0x000495DA File Offset: 0x000477DA
	private void Start()
	{
		this.Sync();
	}

	// Token: 0x06000619 RID: 1561 RVA: 0x000495E2 File Offset: 0x000477E2
	private void OnDisable()
	{
		MicroSplatTerrain.sInstances.Remove(this);
		this.Cleanup();
		this.reenabled = true;
	}

	// Token: 0x0600061A RID: 1562 RVA: 0x000495FD File Offset: 0x000477FD
	private void Cleanup()
	{
		if (this.matInstance != null && this.matInstance != this.templateMaterial)
		{
			Object.DestroyImmediate(this.matInstance);
			this.terrain.materialTemplate = null;
		}
	}

	// Token: 0x0600061B RID: 1563 RVA: 0x00049638 File Offset: 0x00047838
	public override MicroSplatObject.TerrainDescriptor GetTerrainDescriptor()
	{
		MicroSplatObject.TerrainDescriptor result = default(MicroSplatObject.TerrainDescriptor);
		result.heightMap = this.terrain.terrainData.heightmapTexture;
		result.normalMap = this.terrain.normalmapTexture;
		if (this.perPixelNormal != null)
		{
			result.normalMap = this.perPixelNormal;
		}
		result.heightMapScale = this.terrain.terrainData.heightmapScale;
		return result;
	}

	// Token: 0x0600061C RID: 1564 RVA: 0x000496AC File Offset: 0x000478AC
	public void Sync()
	{
		if (this.templateMaterial == null)
		{
			return;
		}
		base.ApplySharedData(this.templateMaterial);
		Material material;
		if (this.terrain.materialTemplate == this.matInstance && this.matInstance != null)
		{
			this.terrain.materialTemplate.CopyPropertiesFromMaterial(this.templateMaterial);
			material = this.terrain.materialTemplate;
		}
		else
		{
			material = new Material(this.templateMaterial);
		}
		if (this.terrain.drawInstanced && this.keywordSO.IsKeywordEnabled("_TESSDISTANCE") && this.keywordSO.IsKeywordEnabled("_MSRENDERLOOP_SURFACESHADER"))
		{
			Debug.LogWarning("Disabling terrain instancing when tessellation is enabled, as Unity has not made surface shader tessellation compatible with terrain instancing");
			this.terrain.drawInstanced = false;
		}
		material.hideFlags = HideFlags.HideAndDontSave;
		this.terrain.materialTemplate = material;
		this.matInstance = material;
		base.ApplyMaps(material);
		if (this.terrain.drawInstanced)
		{
			material.SetTexture("_PerPixelNormal", this.terrain.normalmapTexture);
		}
		if (this.keywordSO.IsKeywordEnabled("_CUSTOMSPLATTEXTURES"))
		{
			material.SetTexture("_CustomControl0", (this.customControl0 != null) ? this.customControl0 : Texture2D.blackTexture);
			material.SetTexture("_CustomControl1", (this.customControl1 != null) ? this.customControl1 : Texture2D.blackTexture);
			material.SetTexture("_CustomControl2", (this.customControl2 != null) ? this.customControl2 : Texture2D.blackTexture);
			material.SetTexture("_CustomControl3", (this.customControl3 != null) ? this.customControl3 : Texture2D.blackTexture);
			material.SetTexture("_CustomControl4", (this.customControl4 != null) ? this.customControl4 : Texture2D.blackTexture);
			material.SetTexture("_CustomControl5", (this.customControl5 != null) ? this.customControl5 : Texture2D.blackTexture);
			material.SetTexture("_CustomControl6", (this.customControl6 != null) ? this.customControl6 : Texture2D.blackTexture);
			material.SetTexture("_CustomControl7", (this.customControl7 != null) ? this.customControl7 : Texture2D.blackTexture);
		}
		else
		{
			if (this.terrain == null || this.terrain.terrainData == null)
			{
				Debug.LogError("Terrain or terrain data is null, cannot sync");
				return;
			}
			Texture2D[] alphamapTextures = this.terrain.terrainData.alphamapTextures;
			base.ApplyControlTextures(alphamapTextures, material);
		}
		base.ApplyBlendMap();
		if (this.OnMaterialSync != null)
		{
			this.OnMaterialSync(material);
		}
	}

	// Token: 0x0600061D RID: 1565 RVA: 0x0004995E File Offset: 0x00047B5E
	public override Bounds GetBounds()
	{
		return this.terrain.terrainData.bounds;
	}

	// Token: 0x0600061E RID: 1566 RVA: 0x00049970 File Offset: 0x00047B70
	public new static void SyncAll()
	{
		for (int i = 0; i < MicroSplatTerrain.sInstances.Count; i++)
		{
			MicroSplatTerrain.sInstances[i].Sync();
		}
		if (MicroSplatTerrain.OnMaterialSyncAll != null)
		{
			MicroSplatTerrain.OnMaterialSyncAll();
		}
	}

	// Token: 0x04000D09 RID: 3337
	private static List<MicroSplatTerrain> sInstances = new List<MicroSplatTerrain>();

	// Token: 0x04000D0A RID: 3338
	public Terrain terrain;

	// Token: 0x04000D0B RID: 3339
	[HideInInspector]
	public Texture2D customControl0;

	// Token: 0x04000D0C RID: 3340
	[HideInInspector]
	public Texture2D customControl1;

	// Token: 0x04000D0D RID: 3341
	[HideInInspector]
	public Texture2D customControl2;

	// Token: 0x04000D0E RID: 3342
	[HideInInspector]
	public Texture2D customControl3;

	// Token: 0x04000D0F RID: 3343
	[HideInInspector]
	public Texture2D customControl4;

	// Token: 0x04000D10 RID: 3344
	[HideInInspector]
	public Texture2D customControl5;

	// Token: 0x04000D11 RID: 3345
	[HideInInspector]
	public Texture2D customControl6;

	// Token: 0x04000D12 RID: 3346
	[HideInInspector]
	public Texture2D customControl7;

	// Token: 0x04000D13 RID: 3347
	[HideInInspector]
	public bool reenabled;

	// Token: 0x020003E0 RID: 992
	// (Invoke) Token: 0x06001808 RID: 6152
	public delegate void MaterialSyncAll();

	// Token: 0x020003E1 RID: 993
	// (Invoke) Token: 0x0600180C RID: 6156
	public delegate void MaterialSync(Material m);
}
