using System;
using UnityEngine;

// Token: 0x020000F2 RID: 242
[ExecuteAlways]
public class MicroSplatObject : MonoBehaviour
{
	// Token: 0x060005F8 RID: 1528 RVA: 0x00048B68 File Offset: 0x00046D68
	protected long GetOverrideHash()
	{
		long num = 3L * (long)(((this.propData == null) ? 3 : this.propData.GetHashCode()) * 3) * (((this.perPixelNormal == null) ? 7L : this.perPixelNormal.GetNativeTexturePtr().ToInt64()) * 7L) * (long)(((this.keywordSO == null) ? 11 : this.keywordSO.GetHashCode()) * 11) * (((this.streamTexture == null) ? 41L : this.streamTexture.GetNativeTexturePtr().ToInt64()) * 41L);
		if (num == 0L)
		{
			Debug.Log("Override hash returned 0, this should not happen");
		}
		return num;
	}

	// Token: 0x060005F9 RID: 1529 RVA: 0x00048C1A File Offset: 0x00046E1A
	protected void SetMap(Material m, string name, Texture tex)
	{
		if (m.HasProperty(name) && tex != null)
		{
			m.SetTexture(name, tex);
		}
	}

	// Token: 0x060005FA RID: 1530 RVA: 0x00048C36 File Offset: 0x00046E36
	protected void ApplySharedData(Material m)
	{
		if (this.propData != null)
		{
			m.SetTexture("_PerTexProps", this.propData.GetTexture());
		}
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x00048C5C File Offset: 0x00046E5C
	protected void ApplyMaps(Material m)
	{
		this.SetMap(m, "_StreamControl", this.streamTexture);
		this.SetMap(m, "_PerPixelNormal", this.perPixelNormal);
		MicroSplatObject.TerrainDescriptor terrainDescriptor = this.GetTerrainDescriptor();
		if (this.perPixelNormal == null && terrainDescriptor.normalMap != null)
		{
			this.SetMap(m, "_PerPixelNormal", terrainDescriptor.normalMap);
		}
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x00048CC4 File Offset: 0x00046EC4
	protected void ApplyControlTextures(Texture2D[] controls, Material m)
	{
		m.SetTexture("_Control0", (controls.Length != 0) ? controls[0] : Texture2D.blackTexture);
		m.SetTexture("_Control1", (controls.Length > 1) ? controls[1] : Texture2D.blackTexture);
		m.SetTexture("_Control2", (controls.Length > 2) ? controls[2] : Texture2D.blackTexture);
		m.SetTexture("_Control3", (controls.Length > 3) ? controls[3] : Texture2D.blackTexture);
		m.SetTexture("_Control4", (controls.Length > 4) ? controls[4] : Texture2D.blackTexture);
		m.SetTexture("_Control5", (controls.Length > 5) ? controls[5] : Texture2D.blackTexture);
		m.SetTexture("_Control6", (controls.Length > 6) ? controls[6] : Texture2D.blackTexture);
		m.SetTexture("_Control7", (controls.Length > 7) ? controls[7] : Texture2D.blackTexture);
	}

	// Token: 0x060005FD RID: 1533 RVA: 0x00048DA8 File Offset: 0x00046FA8
	protected void SyncBlendMat(Vector3 size)
	{
		if (this.blendMatInstance != null && this.matInstance != null)
		{
			this.blendMatInstance.CopyPropertiesFromMaterial(this.matInstance);
			Vector4 value = default(Vector4);
			value.z = size.x;
			value.w = size.z;
			value.x = base.transform.position.x;
			value.y = base.transform.position.z;
			this.blendMatInstance.SetVector("_TerrainBounds", value);
			MicroSplatObject.TerrainDescriptor terrainDescriptor = this.GetTerrainDescriptor();
			this.blendMatInstance.SetTexture("_TerrainHeightmapTexture", terrainDescriptor.heightMap);
			this.blendMatInstance.SetTexture("_TerrainNormalmapTexture", terrainDescriptor.normalMap);
			this.blendMatInstance.SetVector("_TerrainHeightmapScale", terrainDescriptor.heightMapScale);
			if (terrainDescriptor.normalMap != null)
			{
				this.blendMatInstance.SetTexture("_PerPixelNormal", terrainDescriptor.normalMap);
			}
		}
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x00048EBC File Offset: 0x000470BC
	public virtual MicroSplatObject.TerrainDescriptor GetTerrainDescriptor()
	{
		return default(MicroSplatObject.TerrainDescriptor);
	}

	// Token: 0x060005FF RID: 1535 RVA: 0x00048ED4 File Offset: 0x000470D4
	public virtual Bounds GetBounds()
	{
		return default(Bounds);
	}

	// Token: 0x06000600 RID: 1536 RVA: 0x00048EEC File Offset: 0x000470EC
	public Material GetBlendMatInstance()
	{
		if (this.blendMat != null)
		{
			if (this.blendMatInstance == null)
			{
				this.blendMatInstance = new Material(this.blendMat);
				this.SyncBlendMat(this.GetBounds().size);
			}
			if (this.blendMatInstance.shader != this.blendMat.shader)
			{
				this.blendMatInstance.shader = this.blendMat.shader;
				this.SyncBlendMat(this.GetBounds().size);
			}
		}
		return this.blendMatInstance;
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x00048F88 File Offset: 0x00047188
	public void ApplyBlendMap()
	{
		if (this.blendMat != null)
		{
			if (this.blendMatInstance == null)
			{
				this.blendMatInstance = new Material(this.blendMat);
			}
			this.SyncBlendMat(this.GetBounds().size);
		}
	}

	// Token: 0x06000602 RID: 1538 RVA: 0x00002188 File Offset: 0x00000388
	public void RevisionFromMat()
	{
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x00048FD6 File Offset: 0x000471D6
	public static void SyncAll()
	{
		MicroSplatTerrain.SyncAll();
	}

	// Token: 0x04000CF5 RID: 3317
	[HideInInspector]
	public Material templateMaterial;

	// Token: 0x04000CF6 RID: 3318
	[HideInInspector]
	[NonSerialized]
	public Material matInstance;

	// Token: 0x04000CF7 RID: 3319
	[HideInInspector]
	public Material blendMat;

	// Token: 0x04000CF8 RID: 3320
	[HideInInspector]
	public Material blendMatInstance;

	// Token: 0x04000CF9 RID: 3321
	[HideInInspector]
	public MicroSplatKeywords keywordSO;

	// Token: 0x04000CFA RID: 3322
	[HideInInspector]
	public Texture2D perPixelNormal;

	// Token: 0x04000CFB RID: 3323
	[HideInInspector]
	public Texture2D streamTexture;

	// Token: 0x04000CFC RID: 3324
	[HideInInspector]
	public MicroSplatPropData propData;

	// Token: 0x020003DC RID: 988
	public struct TerrainDescriptor
	{
		// Token: 0x04002847 RID: 10311
		public Texture heightMap;

		// Token: 0x04002848 RID: 10312
		public Texture normalMap;

		// Token: 0x04002849 RID: 10313
		public Vector3 heightMapScale;
	}
}
