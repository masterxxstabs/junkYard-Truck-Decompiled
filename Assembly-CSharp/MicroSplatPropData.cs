using System;
using UnityEngine;

// Token: 0x020000F3 RID: 243
public class MicroSplatPropData : ScriptableObject
{
	// Token: 0x06000605 RID: 1541 RVA: 0x00048FE0 File Offset: 0x000471E0
	private void RevisionData()
	{
		if (this.values.Length == 256)
		{
			Color[] array = new Color[1024];
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					array[j * 32 + i] = this.values[j * 32 + i];
				}
			}
			this.values = array;
			return;
		}
		if (this.values.Length == 512)
		{
			Color[] array2 = new Color[1024];
			for (int k = 0; k < 32; k++)
			{
				for (int l = 0; l < 16; l++)
				{
					array2[l * 32 + k] = this.values[l * 32 + k];
				}
			}
			this.values = array2;
		}
	}

	// Token: 0x06000606 RID: 1542 RVA: 0x000490AA File Offset: 0x000472AA
	public Color GetValue(int x, int y)
	{
		this.RevisionData();
		return this.values[y * 32 + x];
	}

	// Token: 0x06000607 RID: 1543 RVA: 0x000490C3 File Offset: 0x000472C3
	public void SetValue(int x, int y, Color c)
	{
		this.RevisionData();
		this.values[y * 32 + x] = c;
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x000490E0 File Offset: 0x000472E0
	public void SetValue(int x, int y, int channel, float value)
	{
		this.RevisionData();
		int num = y * 32 + x;
		Color color = this.values[num];
		color[channel] = value;
		this.values[num] = color;
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x00049120 File Offset: 0x00047320
	public void SetValue(int x, int y, int channel, Vector2 value)
	{
		this.RevisionData();
		int num = y * 32 + x;
		Color color = this.values[num];
		if (channel == 0)
		{
			color.r = value.x;
			color.g = value.y;
		}
		else
		{
			color.b = value.x;
			color.a = value.y;
		}
		this.values[num] = color;
	}

	// Token: 0x0600060A RID: 1546 RVA: 0x00049194 File Offset: 0x00047394
	public void SetValue(int textureIndex, MicroSplatPropData.PerTexFloat channel, float value)
	{
		float num = (float)channel / 4f;
		int num2 = (int)num;
		int channel2 = Mathf.RoundToInt((num - (float)num2) * 4f);
		this.SetValue(textureIndex, num2, channel2, value);
	}

	// Token: 0x0600060B RID: 1547 RVA: 0x000491C8 File Offset: 0x000473C8
	public void SetValue(int textureIndex, MicroSplatPropData.PerTexColor channel, Color value)
	{
		int y = (int)((float)channel / 4f);
		this.SetValue(textureIndex, y, value);
	}

	// Token: 0x0600060C RID: 1548 RVA: 0x000491E8 File Offset: 0x000473E8
	public void SetValue(int textureIndex, MicroSplatPropData.PerTexVector2 channel, Vector2 value)
	{
		float num = (float)channel / 4f;
		int num2 = (int)num;
		int channel2 = Mathf.RoundToInt((num - (float)num2) * 4f);
		this.SetValue(textureIndex, num2, channel2, value);
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x0004921C File Offset: 0x0004741C
	public Texture2D GetTexture()
	{
		this.RevisionData();
		if (this.propTex == null)
		{
			if (SystemInfo.SupportsTextureFormat(TextureFormat.RGBAFloat))
			{
				this.propTex = new Texture2D(32, 32, TextureFormat.RGBAFloat, false, true);
			}
			else if (SystemInfo.SupportsTextureFormat(TextureFormat.RGBAHalf))
			{
				this.propTex = new Texture2D(32, 32, TextureFormat.RGBAHalf, false, true);
			}
			else
			{
				Debug.LogError("Could not create RGBAFloat or RGBAHalf format textures, per texture properties will be clamped to 0-1 range, which will break things");
				this.propTex = new Texture2D(32, 32, TextureFormat.RGBA32, false, true);
			}
			this.propTex.wrapMode = TextureWrapMode.Clamp;
			this.propTex.filterMode = FilterMode.Point;
			this.propTex.hideFlags = HideFlags.None;
		}
		this.propTex.SetPixels(this.values);
		this.propTex.Apply();
		return this.propTex;
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x000492DC File Offset: 0x000474DC
	public Texture2D GetGeoCurve()
	{
		if (this.geoTex == null)
		{
			this.geoTex = new Texture2D(256, 1, TextureFormat.RHalf, false, true);
		}
		for (int i = 0; i < 256; i++)
		{
			float num = this.geoCurve.Evaluate((float)i / 255f);
			this.geoTex.SetPixel(i, 0, new Color(num, num, num, num));
		}
		this.geoTex.Apply();
		return this.geoTex;
	}

	// Token: 0x0600060F RID: 1551 RVA: 0x00049358 File Offset: 0x00047558
	public Texture2D GetGeoSlopeFilter()
	{
		if (this.geoSlopeTex == null)
		{
			this.geoSlopeTex = new Texture2D(256, 1, TextureFormat.Alpha8, false, true);
		}
		for (int i = 0; i < 256; i++)
		{
			float num = this.geoSlopeFilter.Evaluate((float)i / 255f);
			this.geoSlopeTex.SetPixel(i, 0, new Color(num, num, num, num));
		}
		this.geoSlopeTex.Apply();
		return this.geoSlopeTex;
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x000493D4 File Offset: 0x000475D4
	public Texture2D GetGlobalSlopeFilter()
	{
		if (this.globalSlopeTex == null)
		{
			this.globalSlopeTex = new Texture2D(256, 1, TextureFormat.Alpha8, false, true);
		}
		for (int i = 0; i < 256; i++)
		{
			float num = this.globalSlopeFilter.Evaluate((float)i / 255f);
			this.globalSlopeTex.SetPixel(i, 0, new Color(num, num, num, num));
		}
		this.globalSlopeTex.Apply();
		return this.globalSlopeTex;
	}

	// Token: 0x04000CFD RID: 3325
	public const int sMaxTextures = 32;

	// Token: 0x04000CFE RID: 3326
	public const int sMaxAttributes = 32;

	// Token: 0x04000CFF RID: 3327
	[HideInInspector]
	public Color[] values = new Color[1024];

	// Token: 0x04000D00 RID: 3328
	[HideInInspector]
	public Texture2D propTex;

	// Token: 0x04000D01 RID: 3329
	[HideInInspector]
	public AnimationCurve geoCurve = AnimationCurve.Linear(0f, 0f, 0f, 0f);

	// Token: 0x04000D02 RID: 3330
	[HideInInspector]
	public Texture2D geoTex;

	// Token: 0x04000D03 RID: 3331
	[HideInInspector]
	public AnimationCurve geoSlopeFilter = AnimationCurve.Linear(0f, 0.2f, 0.4f, 1f);

	// Token: 0x04000D04 RID: 3332
	[HideInInspector]
	public Texture2D geoSlopeTex;

	// Token: 0x04000D05 RID: 3333
	[HideInInspector]
	public AnimationCurve globalSlopeFilter = AnimationCurve.Linear(0f, 0.2f, 0.4f, 1f);

	// Token: 0x04000D06 RID: 3334
	[HideInInspector]
	public Texture2D globalSlopeTex;

	// Token: 0x020003DD RID: 989
	public enum PerTexVector2
	{
		// Token: 0x0400284B RID: 10315
		SplatUVScale,
		// Token: 0x0400284C RID: 10316
		SplatUVOffset = 2
	}

	// Token: 0x020003DE RID: 990
	public enum PerTexColor
	{
		// Token: 0x0400284E RID: 10318
		Tint = 4,
		// Token: 0x0400284F RID: 10319
		SSSRTint = 72
	}

	// Token: 0x020003DF RID: 991
	public enum PerTexFloat
	{
		// Token: 0x04002851 RID: 10321
		InterpolationContrast = 5,
		// Token: 0x04002852 RID: 10322
		NormalStrength = 8,
		// Token: 0x04002853 RID: 10323
		Smoothness,
		// Token: 0x04002854 RID: 10324
		AO,
		// Token: 0x04002855 RID: 10325
		Metallic,
		// Token: 0x04002856 RID: 10326
		Brightness,
		// Token: 0x04002857 RID: 10327
		Contrast,
		// Token: 0x04002858 RID: 10328
		Porosity,
		// Token: 0x04002859 RID: 10329
		Foam,
		// Token: 0x0400285A RID: 10330
		DetailNoiseStrength,
		// Token: 0x0400285B RID: 10331
		DistanceNoiseStrength,
		// Token: 0x0400285C RID: 10332
		DistanceResample,
		// Token: 0x0400285D RID: 10333
		DisplacementMip,
		// Token: 0x0400285E RID: 10334
		GeoTexStrength,
		// Token: 0x0400285F RID: 10335
		GeoTintStrength,
		// Token: 0x04002860 RID: 10336
		GeoNormalStrength,
		// Token: 0x04002861 RID: 10337
		GlobalSmoothMetalAOStength,
		// Token: 0x04002862 RID: 10338
		DisplacementStength,
		// Token: 0x04002863 RID: 10339
		DisplacementBias,
		// Token: 0x04002864 RID: 10340
		DisplacementOffset,
		// Token: 0x04002865 RID: 10341
		GlobalEmisStength,
		// Token: 0x04002866 RID: 10342
		NoiseNormal0Strength,
		// Token: 0x04002867 RID: 10343
		NoiseNormal1Strength,
		// Token: 0x04002868 RID: 10344
		NoiseNormal2Strength,
		// Token: 0x04002869 RID: 10345
		WindParticulateStrength,
		// Token: 0x0400286A RID: 10346
		SnowAmount,
		// Token: 0x0400286B RID: 10347
		GlitterAmount,
		// Token: 0x0400286C RID: 10348
		GeoHeightFilter,
		// Token: 0x0400286D RID: 10349
		GeoHeightFilterStrength,
		// Token: 0x0400286E RID: 10350
		TriplanarMode,
		// Token: 0x0400286F RID: 10351
		TriplanarContrast,
		// Token: 0x04002870 RID: 10352
		StochatsicEnabled,
		// Token: 0x04002871 RID: 10353
		Saturation,
		// Token: 0x04002872 RID: 10354
		TextureClusterContrast,
		// Token: 0x04002873 RID: 10355
		TextureClusterBoost,
		// Token: 0x04002874 RID: 10356
		HeightOffset,
		// Token: 0x04002875 RID: 10357
		HeightContrast,
		// Token: 0x04002876 RID: 10358
		AntiTileArrayNormalStrength = 56,
		// Token: 0x04002877 RID: 10359
		AntiTileArrayDetailStrength,
		// Token: 0x04002878 RID: 10360
		AntiTileArrayDistanceStrength,
		// Token: 0x04002879 RID: 10361
		DisplaceShaping,
		// Token: 0x0400287A RID: 10362
		UVRotation = 64,
		// Token: 0x0400287B RID: 10363
		TriplanarRotationX,
		// Token: 0x0400287C RID: 10364
		TriplanarRotationY,
		// Token: 0x0400287D RID: 10365
		FuzzyShadingCore = 68,
		// Token: 0x0400287E RID: 10366
		FuzzyShadingEdge,
		// Token: 0x0400287F RID: 10367
		FuzzyShadingPower,
		// Token: 0x04002880 RID: 10368
		SSSThickness = 75
	}
}
