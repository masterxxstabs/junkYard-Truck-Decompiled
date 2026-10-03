using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000F6 RID: 246
[ExecuteAlways]
public class MicroSplatDecalReceiver : MonoBehaviour
{
	// Token: 0x17000042 RID: 66
	// (get) Token: 0x0600062B RID: 1579 RVA: 0x00049C45 File Offset: 0x00047E45
	// (set) Token: 0x0600062C RID: 1580 RVA: 0x00049C4D File Offset: 0x00047E4D
	public MicroSplatObject msObj { get; private set; }

	// Token: 0x0600062D RID: 1581 RVA: 0x00049C58 File Offset: 0x00047E58
	private void InitSystem()
	{
		if (this.decalBlock == null)
		{
			this.decalBlock = new MaterialPropertyBlock();
			this.msObj = base.GetComponent<MicroSplatObject>();
			if (this.msObj == null)
			{
				Debug.LogError("MicroSplatDecalReceiver must be on MicroSplat Object");
			}
			else
			{
				this.terrain = base.GetComponent<Terrain>();
				this.rend = base.GetComponent<Renderer>();
				if (this.msObj.keywordSO == null)
				{
					Debug.LogError("MicroSplatDecalReceiver cannot find keyword data on MicroSplatObject, please make sure this is assigned");
				}
			}
			this.InitStatic();
			this.InitDynamic();
		}
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x00049CDF File Offset: 0x00047EDF
	public bool RegisterDecal(MicroSplatDecal d)
	{
		if (this.decalBlock == null)
		{
			this.InitSystem();
		}
		if (!(this.terrain != null))
		{
			this.RegisterDynamicDecal(d);
			return true;
		}
		if (d.dynamic)
		{
			this.RegisterDynamicDecal(d);
			return true;
		}
		this.RegisterStaticDecal(d);
		return false;
	}

	// Token: 0x0600062F RID: 1583 RVA: 0x00049D1F File Offset: 0x00047F1F
	public void UnregisterDecal(MicroSplatDecal d)
	{
		if (!(this.terrain != null))
		{
			this.UnregisterDynamicDecal(d);
			return;
		}
		if (d.dynamic)
		{
			this.UnregisterDynamicDecal(d);
			return;
		}
		this.UnregisterStaticDecal(d);
	}

	// Token: 0x06000630 RID: 1584 RVA: 0x00049D50 File Offset: 0x00047F50
	private void SetData(MicroSplatDecal d, int index, Texture2D tex)
	{
		if (d == null)
		{
			return;
		}
		Matrix4x4 worldToLocalMatrix = d.transform.worldToLocalMatrix;
		tex.SetPixel(index, 0, new Color(worldToLocalMatrix.m00, worldToLocalMatrix.m01, worldToLocalMatrix.m02, worldToLocalMatrix.m03));
		tex.SetPixel(index, 1, new Color(worldToLocalMatrix.m10, worldToLocalMatrix.m11, worldToLocalMatrix.m12, worldToLocalMatrix.m13));
		tex.SetPixel(index, 2, new Color(worldToLocalMatrix.m20, worldToLocalMatrix.m21, worldToLocalMatrix.m22, worldToLocalMatrix.m23));
		tex.SetPixel(index, 3, new Color(worldToLocalMatrix.m30, worldToLocalMatrix.m31, worldToLocalMatrix.m32, worldToLocalMatrix.m33));
		Vector4 v;
		Vector4 v2;
		d.GetShaderData(out v, out v2);
		tex.SetPixel(index, 4, v);
		tex.SetPixel(index, 5, v2);
		tex.SetPixel(index, 6, d.splatIndexes);
		tex.SetPixel(index, 7, d.tint);
	}

	// Token: 0x06000631 RID: 1585 RVA: 0x00049E4E File Offset: 0x0004804E
	private void OnEnable()
	{
		this.InitSystem();
	}

	// Token: 0x06000632 RID: 1586 RVA: 0x00049E56 File Offset: 0x00048056
	private void OnDisable()
	{
		this.decalBlock = null;
	}

	// Token: 0x06000633 RID: 1587 RVA: 0x00049E60 File Offset: 0x00048060
	private void OnDestroy()
	{
		this.decalBlock = null;
		if (this.staticCacheData)
		{
			Object.DestroyImmediate(this.staticCacheData);
		}
		if (this.cacheMask != null)
		{
			Object.DestroyImmediate(this.cacheMask);
		}
		if (this.dynamicCacheData)
		{
			Object.DestroyImmediate(this.dynamicCacheData);
		}
		if (this.dynamicCullData)
		{
			Object.DestroyImmediate(this.dynamicCullData);
		}
	}

	// Token: 0x06000634 RID: 1588 RVA: 0x00049ED8 File Offset: 0x000480D8
	private void Update()
	{
		if (this.needsStaticUpdate)
		{
			this.needsStaticUpdate = false;
			this.UpdateStaticCache();
			this.UpdatePropertyBlocks();
			if (!this.generateCacheOnLoad && this.loadStaticFromCache)
			{
				this.loadStaticFromCache = false;
				this.LoadFromCache();
				return;
			}
			this.RerenderCacheMap();
		}
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x00049F24 File Offset: 0x00048124
	private void UpdatePropertyBlocks()
	{
		if (this.decalBlock == null)
		{
			return;
		}
		if (this.terrain != null)
		{
			this.terrain.GetSplatMaterialPropertyBlock(this.decalBlock);
		}
		else if (this.rend != null)
		{
			this.rend.GetPropertyBlock(this.decalBlock);
		}
		this.UpdateDynamicPropertyBlocks();
		this.UpdateStaticPropertyBlocks();
		if (this.terrain != null)
		{
			this.terrain.SetSplatMaterialPropertyBlock(this.decalBlock);
			return;
		}
		if (this.rend != null)
		{
			this.rend.SetPropertyBlock(this.decalBlock);
		}
	}

	// Token: 0x17000043 RID: 67
	// (get) Token: 0x06000636 RID: 1590 RVA: 0x00049FC5 File Offset: 0x000481C5
	public int dynamicCount
	{
		get
		{
			if (this.dynamicDecals != null)
			{
				return this.dynamicDecals.Count;
			}
			return 0;
		}
	}

	// Token: 0x06000637 RID: 1591 RVA: 0x00049FDC File Offset: 0x000481DC
	private void ClearDynamicCacheData()
	{
		if (this.staticCacheData != null)
		{
			Object.DestroyImmediate(this.dynamicCacheData);
		}
		if (this.dynamicCullData != null)
		{
			Object.DestroyImmediate(this.dynamicCullData);
		}
		this.dynamicCacheData = new Texture2D(this.maxDynamicDecals, 8, TextureFormat.RGBAFloat, false, true);
		this.dynamicCacheData.Apply(false, false);
		this.dynamicCullData = new Texture2D(this.maxDynamicDecals, 1, TextureFormat.RGBAFloat, false, true);
		this.dynamicCullData.Apply(false, false);
		this.dynamicCacheData.hideFlags = HideFlags.HideAndDontSave;
		this.dynamicCullData.hideFlags = HideFlags.HideAndDontSave;
	}

	// Token: 0x06000638 RID: 1592 RVA: 0x0004A07C File Offset: 0x0004827C
	private void InitDynamic()
	{
		this.maxDynamicDecals = 8;
		if (this.msObj.keywordSO.IsKeywordEnabled("_DECAL_MAX0"))
		{
			this.maxDynamicDecals = 1;
		}
		if (this.msObj.keywordSO.IsKeywordEnabled("_DECAL_MAX16"))
		{
			this.maxDynamicDecals = 16;
		}
		else if (this.msObj.keywordSO.IsKeywordEnabled("_DECAL_MAX32"))
		{
			this.maxDynamicDecals = 32;
		}
		else if (this.msObj.keywordSO.IsKeywordEnabled("_DECAL_MAX64"))
		{
			this.maxDynamicDecals = 64;
		}
		else if (this.msObj.keywordSO.IsKeywordEnabled("_DECAL_MAX128"))
		{
			this.maxDynamicDecals = 128;
		}
		else if (this.msObj.keywordSO.IsKeywordEnabled("_DECAL_MAX256"))
		{
			this.maxDynamicDecals = 256;
		}
		this.dynamicDecals = new List<MicroSplatDecal>(this.maxDynamicDecals);
		this.ClearDynamicCacheData();
	}

	// Token: 0x06000639 RID: 1593 RVA: 0x0004A174 File Offset: 0x00048374
	private void RegisterDynamicDecal(MicroSplatDecal d)
	{
		if (!this.staticDecals.Contains(d))
		{
			this.dynamicDecals.Add(d);
			if (this.dynamicDecals.Count > 1 && d.sortOrder != this.dynamicDecals[this.dynamicDecals.Count - 2].sortOrder)
			{
				this.dynamicDecals.Sort((MicroSplatDecal x, MicroSplatDecal y) => x.sortOrder.CompareTo(y.sortOrder));
			}
		}
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x0004A1F8 File Offset: 0x000483F8
	private void UpdateDynamicPropertyBlocks()
	{
		int count = this.dynamicDecals.Count;
		if (count > this.maxDynamicDecals)
		{
			count = this.maxDynamicDecals;
		}
		for (int i = 0; i < count; i++)
		{
			float a = (this.dynamicDecals[i].transform.lossyScale - Vector3.zero).sqrMagnitude * 0.5f;
			Vector3 position = this.dynamicDecals[i].transform.position;
			this.dynamicCullData.SetPixel(i, 0, new Color(position.x, position.y, position.z, a));
			this.SetData(this.dynamicDecals[i], i, this.dynamicCacheData);
		}
		this.dynamicCacheData.Apply(false, false);
		this.dynamicCullData.Apply(false, false);
		this.decalBlock.SetInt("_MSDecalCount", count);
		this.decalBlock.SetTexture("_DecalCullData", this.dynamicCullData);
		this.decalBlock.SetTexture("_DecalDynamicData", this.dynamicCacheData);
	}

	// Token: 0x0600063B RID: 1595 RVA: 0x0004A312 File Offset: 0x00048512
	private void UnregisterDynamicDecal(MicroSplatDecal d)
	{
		if (this.dynamicDecals != null && this.dynamicDecals.Contains(d))
		{
			this.dynamicDecals.Remove(d);
		}
	}

	// Token: 0x17000044 RID: 68
	// (get) Token: 0x0600063C RID: 1596 RVA: 0x0004A337 File Offset: 0x00048537
	public int staticCount
	{
		get
		{
			if (this.staticDecals != null)
			{
				return this.staticDecals.Count;
			}
			return 0;
		}
	}

	// Token: 0x0600063D RID: 1597 RVA: 0x0004A350 File Offset: 0x00048550
	private void ClearStaticCacheData()
	{
		if (this.staticCacheData != null)
		{
			Object.DestroyImmediate(this.staticCacheData);
		}
		this.staticCacheData = new Texture2D(this.maxStaticDecals, 8, TextureFormat.RGBAFloat, false, true);
		this.staticCacheData.Apply(false, false);
		this.staticCacheData.hideFlags = HideFlags.HideAndDontSave;
	}

	// Token: 0x0600063E RID: 1598 RVA: 0x0004A3A8 File Offset: 0x000485A8
	private void ClearCacheMask()
	{
		if (this.cacheMask != null)
		{
			Object.DestroyImmediate(this.cacheMask);
		}
		int num = (int)this.staticCacheSize;
		if (this.cacheMaskBuffer == null || this.cacheMaskBuffer.Length != num * num)
		{
			this.ClearCacheMaskBuffer();
		}
		this.cacheMask = new Texture2D(num, num, TextureFormat.RGBAHalf, false, true);
		this.cacheMask.hideFlags = HideFlags.HideAndDontSave;
		this.cacheMask.filterMode = FilterMode.Point;
		this.cacheMask.wrapMode = TextureWrapMode.Clamp;
		this.cacheMask.SetPixels(this.cacheMaskBuffer);
		this.cacheMask.Apply(false, false);
	}

	// Token: 0x0600063F RID: 1599 RVA: 0x0004A444 File Offset: 0x00048644
	private void ClearCacheMaskBuffer()
	{
		int num = (int)this.staticCacheSize;
		this.cacheMaskBuffer = new Color[num * num];
		Color color = new Color(0f, 0f, 0f, 0f);
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				this.cacheMaskBuffer[j * num + i] = color;
			}
		}
	}

	// Token: 0x06000640 RID: 1600 RVA: 0x0004A4AC File Offset: 0x000486AC
	private void InitStatic()
	{
		this.maxStaticDecals = 1;
		if (this.msObj.keywordSO.IsKeywordEnabled("_DECAL_STATICMAX64"))
		{
			this.maxStaticDecals = 64;
		}
		else if (this.msObj.keywordSO.IsKeywordEnabled("_DECAL_STATICMAX128"))
		{
			this.maxStaticDecals = 128;
		}
		else if (this.msObj.keywordSO.IsKeywordEnabled("_DECAL_STATICMAX256"))
		{
			this.maxStaticDecals = 256;
		}
		else if (this.msObj.keywordSO.IsKeywordEnabled("_DECAL_STATICMAX512"))
		{
			this.maxStaticDecals = 512;
		}
		else if (this.msObj.keywordSO.IsKeywordEnabled("_DECAL_STATICMAX1024"))
		{
			this.maxStaticDecals = 1024;
		}
		else if (this.msObj.keywordSO.IsKeywordEnabled("_DECAL_STATICMAX2048"))
		{
			this.maxStaticDecals = 2048;
		}
		this.staticDecals = new List<MicroSplatDecal>(this.maxStaticDecals);
		if (Application.IsPlaying(this) && this.cacheMaskBuffer != null && this.cacheMaskBuffer.Length == this.maxStaticDecals * this.maxStaticDecals)
		{
			this.loadStaticFromCache = true;
			return;
		}
		this.ClearCacheMask();
		this.ClearStaticCacheData();
		this.needsStaticUpdate = true;
	}

	// Token: 0x06000641 RID: 1601 RVA: 0x0004A5EC File Offset: 0x000487EC
	private void RegisterStaticDecal(MicroSplatDecal d)
	{
		this.staticDecals.Add(d);
		this.needsStaticUpdate = true;
	}

	// Token: 0x06000642 RID: 1602 RVA: 0x0004A601 File Offset: 0x00048801
	private void UnregisterStaticDecal(MicroSplatDecal d)
	{
		if (this.terrain != null && this.staticDecals != null && this.staticDecals.Contains(d))
		{
			this.staticDecals.Remove(d);
			this.needsStaticUpdate = true;
		}
	}

	// Token: 0x06000643 RID: 1603 RVA: 0x0004A63C File Offset: 0x0004883C
	private void UpdateStaticCache()
	{
		if (this.staticDecals == null)
		{
			return;
		}
		int count = this.staticDecals.Count;
		if (count > this.maxStaticDecals)
		{
			count = this.maxStaticDecals;
		}
		for (int i = 0; i < count; i++)
		{
			this.SetData(this.staticDecals[i], i, this.staticCacheData);
		}
		this.staticCacheData.Apply(false, false);
	}

	// Token: 0x06000644 RID: 1604 RVA: 0x0004A6A0 File Offset: 0x000488A0
	private void UpdateStaticPropertyBlocks()
	{
		this.decalBlock.SetTexture("_DecalControl", this.cacheMask);
		this.decalBlock.SetTexture("_DecalStaticData", this.staticCacheData);
	}

	// Token: 0x06000645 RID: 1605 RVA: 0x0004A6D0 File Offset: 0x000488D0
	private Vector2 WorldToTerrainPixel(Vector3 terrainPos, Vector3 terrainSize, Vector3 point, Texture2D splatControl)
	{
		point -= terrainPos;
		float x = point.x / terrainSize.x * (float)splatControl.width;
		float y = point.z / terrainSize.z * (float)splatControl.height;
		return new Vector2(x, y);
	}

	// Token: 0x06000646 RID: 1606 RVA: 0x0004A71C File Offset: 0x0004891C
	private Vector3 TerrainPixelToWorld(Vector3 terrainPos, Vector3 terrainSize, int x, int y, Texture2D splatControl)
	{
		Vector3 a = new Vector3((float)x, 0f, (float)y);
		a.x *= terrainSize.x / (float)splatControl.width;
		a.z *= terrainSize.z / (float)splatControl.height;
		return a += terrainPos;
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x0004A778 File Offset: 0x00048978
	private Vector3 TerrainPixelToWorldWithHeight(Terrain t, Vector3 terrainPos, Vector3 terrainSize, int x, int y, Texture2D splatControl)
	{
		Vector3 a = new Vector3((float)x, 0f, (float)y);
		a.x *= terrainSize.x / (float)splatControl.width;
		a.y = t.terrainData.GetInterpolatedHeight((float)x, (float)y);
		a.z *= terrainSize.z / (float)splatControl.height;
		return a += terrainPos;
	}

	// Token: 0x06000648 RID: 1608 RVA: 0x0004A7EC File Offset: 0x000489EC
	private bool GetDecalPixelBounds(Vector3 terrainPos, Vector3 terrainSize, Matrix4x4 decalMtx, ref MicroSplatDecalReceiver.PixelBounds bounds)
	{
		float num = 0.5f;
		Bounds bounds2 = new Bounds(decalMtx.MultiplyPoint(new Vector3(-num, -num, -num)), Vector3.one);
		bounds2.Encapsulate(decalMtx.MultiplyPoint(new Vector3(num, num, num)));
		bounds2.Encapsulate(decalMtx.MultiplyPoint(new Vector3(-num, num, num)));
		bounds2.Encapsulate(decalMtx.MultiplyPoint(new Vector3(num, -num, num)));
		bounds2.Encapsulate(decalMtx.MultiplyPoint(new Vector3(num, num, -num)));
		bounds2.Encapsulate(decalMtx.MultiplyPoint(new Vector3(-num, -num, num)));
		bounds2.Encapsulate(decalMtx.MultiplyPoint(new Vector3(num, -num, -num)));
		bounds2.Encapsulate(decalMtx.MultiplyPoint(new Vector3(-num, num, -num)));
		Vector3 min = bounds2.min;
		Vector3 max = bounds2.max;
		Vector2 vector = this.WorldToTerrainPixel(terrainPos, terrainSize, min, this.cacheMask);
		Vector2 vector2 = this.WorldToTerrainPixel(terrainPos, terrainSize, max, this.cacheMask);
		bounds.xmin = Mathf.FloorToInt(vector.x - 1f);
		bounds.ymin = Mathf.FloorToInt(vector.y - 1f);
		bounds.xmax = Mathf.FloorToInt(vector2.x + 1f);
		bounds.ymax = Mathf.FloorToInt(vector2.y + 1f);
		if (bounds.xmin < 0 && bounds.xmax < 0)
		{
			return false;
		}
		if (bounds.ymin < 0 && bounds.ymax < 0)
		{
			return false;
		}
		if (bounds.xmin >= this.cacheMask.width && bounds.xmax >= this.cacheMask.width)
		{
			return false;
		}
		if (bounds.ymin >= this.cacheMask.height && bounds.ymax >= this.cacheMask.height)
		{
			return false;
		}
		bounds.xmin = Mathf.Clamp(bounds.xmin, 0, this.cacheMask.width);
		bounds.xmax = Mathf.Clamp(bounds.xmax, 0, this.cacheMask.width);
		bounds.ymin = Mathf.Clamp(bounds.ymin, 0, this.cacheMask.height);
		bounds.ymax = Mathf.Clamp(bounds.ymax, 0, this.cacheMask.height);
		if (bounds.xmin == bounds.xmax)
		{
			if (bounds.xmax < this.cacheMask.width)
			{
				bounds.xmax++;
			}
			else
			{
				bounds.xmin--;
			}
		}
		if (bounds.ymin == bounds.ymax)
		{
			if (bounds.ymax < this.cacheMask.height)
			{
				bounds.ymax++;
			}
			else
			{
				bounds.ymin--;
			}
		}
		return true;
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x0004AAD0 File Offset: 0x00048CD0
	private bool PointInOABB(Vector3 pt, Matrix4x4 decalMtx)
	{
		Vector3 vector = decalMtx.MultiplyPoint(pt);
		return vector.x < 1f && vector.x > -1f && vector.y < 1f && vector.y > -1f && vector.z < 1f && vector.z > -1f;
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x0004AB38 File Offset: 0x00048D38
	private void ClearDecalInCache(Vector3 terrainPos, Vector3 terrainSize, Matrix4x4 dmtx, int index, MicroSplatDecalReceiver.PixelBounds pb)
	{
		int width = this.cacheMask.width;
		for (int i = pb.xmin; i < pb.xmax; i++)
		{
			for (int j = pb.ymin; j < pb.ymax; j++)
			{
				int num = j * width + i;
				Color color = this.cacheMaskBuffer[num];
				if (Mathf.RoundToInt(color.r - 1f) == index)
				{
					color.r = color.g;
					color.g = color.b;
					color.b = color.a;
					color.a = 0f;
					this.cacheMaskBuffer[num] = color;
				}
				else if (Mathf.RoundToInt(color.g - 1f) == index)
				{
					color.g = color.b;
					color.b = color.a;
					color.a = 0f;
					this.cacheMaskBuffer[num] = color;
				}
				else if (Mathf.RoundToInt(color.b - 1f) == index)
				{
					color.b = color.a;
					color.a = 0f;
					this.cacheMaskBuffer[num] = color;
				}
				else if (Mathf.RoundToInt(color.a - 1f) == index)
				{
					color.a = 0f;
					this.cacheMaskBuffer[num] = color;
				}
			}
		}
	}

	// Token: 0x0600064B RID: 1611 RVA: 0x0004ACC0 File Offset: 0x00048EC0
	private void ClearDecalInCache(Vector3 terrainPos, Vector3 terrainSize, MicroSplatDecal d, Matrix4x4 oldMtx, int index)
	{
		MicroSplatDecalReceiver.PixelBounds pb = default(MicroSplatDecalReceiver.PixelBounds);
		if (this.GetDecalPixelBounds(terrainPos, terrainSize, oldMtx, ref pb))
		{
			this.ClearDecalInCache(terrainPos, terrainSize, oldMtx, index, pb);
		}
	}

	// Token: 0x0600064C RID: 1612 RVA: 0x0004ACF0 File Offset: 0x00048EF0
	private void RenderDecalIntoCache(int index, Vector3 terrainPos, Vector3 terrainSize, MicroSplatDecal d, MicroSplatDecalReceiver.PixelBounds pb)
	{
		Matrix4x4 worldToLocalMatrix = d.transform.worldToLocalMatrix;
		int width = this.cacheMask.width;
		for (int i = pb.xmin; i < pb.xmax; i++)
		{
			for (int j = pb.ymin; j < pb.ymax; j++)
			{
				int num = j * width + i;
				Color color = this.cacheMaskBuffer[num];
				if (Mathf.RoundToInt(color.r - 1f) != index && Mathf.RoundToInt(color.g - 1f) != index && Mathf.RoundToInt(color.b - 1f) != index && Mathf.RoundToInt(color.a - 1f) != index)
				{
					if (color.r < 0.5f)
					{
						color.r = (float)(index + 1);
						color.g = 0f;
						color.b = 0f;
						color.a = 0f;
					}
					else if (color.g < 0.5f)
					{
						color.g = color.r;
						color.r = (float)(index + 1);
						color.b = 0f;
						color.a = 0f;
					}
					else if (color.b < 0.5f)
					{
						color.b = color.g;
						color.g = color.r;
						color.r = (float)(index + 1);
						color.a = 0f;
					}
					else
					{
						color.a = color.b;
						color.b = color.g;
						color.g = color.r;
						color.r = (float)(index + 1);
					}
					this.cacheMaskBuffer[num] = color;
				}
			}
		}
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x0004AED0 File Offset: 0x000490D0
	private void RenderDecalIntoCache(Vector3 terrainPos, Vector3 terrainSize, MicroSplatDecal d, int index)
	{
		if (d.isActiveAndEnabled)
		{
			MicroSplatDecalReceiver.PixelBounds pb = default(MicroSplatDecalReceiver.PixelBounds);
			if (this.GetDecalPixelBounds(terrainPos, terrainSize, d.transform.localToWorldMatrix, ref pb))
			{
				this.RenderDecalIntoCache(index, terrainPos, terrainSize, d, pb);
			}
		}
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x0004AF10 File Offset: 0x00049110
	public void UpdateDecalInCache(Vector3 terrainPos, Vector3 terrainSize, MicroSplatDecal d, Matrix4x4 oldMtx)
	{
		int index = this.staticDecals.IndexOf(d);
		this.ClearDecalInCache(terrainPos, terrainSize, d, oldMtx, index);
		this.RenderDecalIntoCache(terrainPos, terrainSize, d, index);
		this.cacheMask.SetPixels(this.cacheMaskBuffer);
		this.cacheMask.Apply(false, false);
		this.UpdateStaticPropertyBlocks();
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x0004AF64 File Offset: 0x00049164
	public void RerenderCacheMap()
	{
		this.ClearCacheMaskBuffer();
		this.ClearCacheMask();
		this.SortStaticDecals();
		this.UpdateStaticCache();
		this.UpdatePropertyBlocks();
		if (this.terrain != null)
		{
			for (int i = 0; i < this.staticDecals.Count; i++)
			{
				this.RenderDecalIntoCache(this.terrain.transform.position, this.terrain.terrainData.size, this.staticDecals[i], i);
			}
		}
		this.cacheMask.SetPixels(this.cacheMaskBuffer);
		this.cacheMask.Apply(false, false);
		this.staticCacheData.Apply(false, false);
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x0004B014 File Offset: 0x00049214
	private void SortStaticDecals()
	{
		if (this.staticDecals == null)
		{
			return;
		}
		this.staticDecals.Sort((MicroSplatDecal x, MicroSplatDecal y) => x.GetHashCode().CompareTo(y.GetHashCode()));
		this.staticDecals.Sort((MicroSplatDecal x, MicroSplatDecal y) => x.sortOrder.CompareTo(y.sortOrder));
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x0004B080 File Offset: 0x00049280
	public void LoadFromCache()
	{
		this.ClearCacheMask();
		this.SortStaticDecals();
		this.UpdateStaticCache();
		this.UpdatePropertyBlocks();
		this.cacheMask.SetPixels(this.cacheMaskBuffer);
		this.cacheMask.Apply(false, false);
		this.staticCacheData.Apply(false, false);
	}

	// Token: 0x04000D26 RID: 3366
	private MaterialPropertyBlock decalBlock;

	// Token: 0x04000D28 RID: 3368
	private Terrain terrain;

	// Token: 0x04000D29 RID: 3369
	private Renderer rend;

	// Token: 0x04000D2A RID: 3370
	public bool generateCacheOnLoad;

	// Token: 0x04000D2B RID: 3371
	private bool needsStaticUpdate = true;

	// Token: 0x04000D2C RID: 3372
	private bool loadStaticFromCache;

	// Token: 0x04000D2D RID: 3373
	[HideInInspector]
	public List<MicroSplatDecal> dynamicDecals;

	// Token: 0x04000D2E RID: 3374
	[HideInInspector]
	public Texture2D dynamicCacheData;

	// Token: 0x04000D2F RID: 3375
	[HideInInspector]
	public Texture2D dynamicCullData;

	// Token: 0x04000D30 RID: 3376
	private int maxDynamicDecals;

	// Token: 0x04000D31 RID: 3377
	private List<MicroSplatDecal> staticDecals;

	// Token: 0x04000D32 RID: 3378
	[HideInInspector]
	public Texture2D cacheMask;

	// Token: 0x04000D33 RID: 3379
	[HideInInspector]
	public Color[] cacheMaskBuffer;

	// Token: 0x04000D34 RID: 3380
	[HideInInspector]
	public Texture2D staticCacheData;

	// Token: 0x04000D35 RID: 3381
	private int maxStaticDecals;

	// Token: 0x04000D36 RID: 3382
	public MicroSplatDecalReceiver.StaticCacheSize staticCacheSize = MicroSplatDecalReceiver.StaticCacheSize.k256;

	// Token: 0x020003E5 RID: 997
	public enum StaticCacheSize
	{
		// Token: 0x0400288B RID: 10379
		k64 = 64,
		// Token: 0x0400288C RID: 10380
		k128 = 128,
		// Token: 0x0400288D RID: 10381
		k256 = 256,
		// Token: 0x0400288E RID: 10382
		k512 = 512,
		// Token: 0x0400288F RID: 10383
		k1024 = 1024
	}

	// Token: 0x020003E6 RID: 998
	private struct PixelBounds
	{
		// Token: 0x04002890 RID: 10384
		public int xmin;

		// Token: 0x04002891 RID: 10385
		public int xmax;

		// Token: 0x04002892 RID: 10386
		public int ymin;

		// Token: 0x04002893 RID: 10387
		public int ymax;
	}
}
