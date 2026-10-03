using System;
using UnityEngine;

// Token: 0x020000F5 RID: 245
[ExecuteAlways]
public class MicroSplatDecal : MonoBehaviour
{
	// Token: 0x17000041 RID: 65
	// (get) Token: 0x06000622 RID: 1570 RVA: 0x000499F5 File Offset: 0x00047BF5
	// (set) Token: 0x06000621 RID: 1569 RVA: 0x000499C7 File Offset: 0x00047BC7
	public bool dynamic
	{
		get
		{
			return this._dynamic;
		}
		set
		{
			if (value != this._dynamic)
			{
				if (base.enabled)
				{
					this.OnDisable();
				}
				this._dynamic = value;
				if (base.enabled)
				{
					this.OnEnable();
				}
			}
		}
	}

	// Token: 0x06000623 RID: 1571 RVA: 0x00049A00 File Offset: 0x00047C00
	public void GetShaderData(out Vector4 data1, out Vector4 data2)
	{
		float w = Mathf.Floor(this.tessOffset * 256f) + this.tessOpacity * 0.95f;
		float z = (this.splatOpacity + 1f) * (float)((this.splatMode == MicroSplatDecal.SplatMode.SplatMap) ? 1 : -1);
		float y = (this.normalOpacity + 1f) * (float)((this.normalBlend == MicroSplatDecal.NormalBlend.Replace) ? 1 : -1);
		float x = (this.albedoOpacity + 1f) * (float)((this.albedoBlend == MicroSplatDecal.AlbedoBlend.Blend) ? 1 : -1);
		float x2 = (float)(this.splatTextureIndex * 100 + this.textureIndex);
		data1 = new Vector4(x2, base.transform.lossyScale.y, z, w);
		data2 = new Vector4(x, y, this.smoothnessOpacity, this.heightBlend);
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x00049ACE File Offset: 0x00047CCE
	private void OnEnable()
	{
		this.oldMtx = base.transform.localToWorldMatrix;
		if (this.targetObject != null)
		{
			this.targetObject.RegisterDecal(this);
		}
	}

	// Token: 0x06000625 RID: 1573 RVA: 0x00049AFC File Offset: 0x00047CFC
	private void OnDisable()
	{
		if (this.targetObject != null)
		{
			this.targetObject.UnregisterDecal(this);
		}
	}

	// Token: 0x06000626 RID: 1574 RVA: 0x00049B18 File Offset: 0x00047D18
	private void OnDestroy()
	{
		this.OnDisable();
	}

	// Token: 0x06000627 RID: 1575 RVA: 0x00049B20 File Offset: 0x00047D20
	public void Reset()
	{
		this.OnDisable();
		this.OnEnable();
	}

	// Token: 0x06000628 RID: 1576 RVA: 0x00049B30 File Offset: 0x00047D30
	private void UpdateRendering()
	{
		if (this.targetObject != null && this.targetObject.msObj != null)
		{
			MicroSplatTerrain microSplatTerrain = this.targetObject.msObj as MicroSplatTerrain;
			if (microSplatTerrain != null)
			{
				this.targetObject.UpdateDecalInCache(microSplatTerrain.terrain.transform.position, microSplatTerrain.terrain.terrainData.size, this, this.oldMtx);
				this.oldMtx = base.transform.localToWorldMatrix;
			}
		}
	}

	// Token: 0x06000629 RID: 1577 RVA: 0x00049BBB File Offset: 0x00047DBB
	private void Update()
	{
		if (base.transform.hasChanged)
		{
			base.transform.hasChanged = false;
			this.UpdateRendering();
		}
	}

	// Token: 0x04000D14 RID: 3348
	public MicroSplatDecalReceiver targetObject;

	// Token: 0x04000D15 RID: 3349
	public int textureIndex;

	// Token: 0x04000D16 RID: 3350
	public int splatTextureIndex;

	// Token: 0x04000D17 RID: 3351
	public float albedoOpacity = 1f;

	// Token: 0x04000D18 RID: 3352
	public float smoothnessOpacity = 1f;

	// Token: 0x04000D19 RID: 3353
	public float heightBlend;

	// Token: 0x04000D1A RID: 3354
	public float normalOpacity = 1f;

	// Token: 0x04000D1B RID: 3355
	public float splatOpacity;

	// Token: 0x04000D1C RID: 3356
	public Color tint = Color.white;

	// Token: 0x04000D1D RID: 3357
	public MicroSplatDecal.AlbedoBlend albedoBlend;

	// Token: 0x04000D1E RID: 3358
	public MicroSplatDecal.SplatMode splatMode;

	// Token: 0x04000D1F RID: 3359
	public MicroSplatDecal.NormalBlend normalBlend;

	// Token: 0x04000D20 RID: 3360
	public float tessOpacity = 1f;

	// Token: 0x04000D21 RID: 3361
	public float tessOffset;

	// Token: 0x04000D22 RID: 3362
	public int sortOrder;

	// Token: 0x04000D23 RID: 3363
	public Vector4 splatIndexes = new Vector4(0f, 1f, 2f, 3f);

	// Token: 0x04000D24 RID: 3364
	[SerializeField]
	private bool _dynamic;

	// Token: 0x04000D25 RID: 3365
	private Matrix4x4 oldMtx;

	// Token: 0x020003E2 RID: 994
	public enum SplatMode
	{
		// Token: 0x04002882 RID: 10370
		SplatMap,
		// Token: 0x04002883 RID: 10371
		StreamMap
	}

	// Token: 0x020003E3 RID: 995
	public enum NormalBlend
	{
		// Token: 0x04002885 RID: 10373
		Replace,
		// Token: 0x04002886 RID: 10374
		Blend
	}

	// Token: 0x020003E4 RID: 996
	public enum AlbedoBlend
	{
		// Token: 0x04002888 RID: 10376
		Blend,
		// Token: 0x04002889 RID: 10377
		Multiply2X
	}
}
