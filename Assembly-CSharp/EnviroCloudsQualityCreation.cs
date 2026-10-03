using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000A9 RID: 169
public static class EnviroCloudsQualityCreation
{
	// Token: 0x06000400 RID: 1024 RVA: 0x00018FC3 File Offset: 0x000171C3
	public static GameObject GetAssetPrefab(string name)
	{
		return null;
	}

	// Token: 0x06000401 RID: 1025 RVA: 0x00018FC3 File Offset: 0x000171C3
	public static AudioClip GetAudioClip(string name)
	{
		return null;
	}

	// Token: 0x06000402 RID: 1026 RVA: 0x00018FC3 File Offset: 0x000171C3
	public static Cubemap GetAssetCubemap(string name)
	{
		return null;
	}

	// Token: 0x06000403 RID: 1027 RVA: 0x00018FC3 File Offset: 0x000171C3
	public static EnviroProfile GetDefaultProfile(string name)
	{
		return null;
	}

	// Token: 0x06000404 RID: 1028 RVA: 0x00018FC3 File Offset: 0x000171C3
	public static Texture GetAssetTexture(string name)
	{
		return null;
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x00029C8C File Offset: 0x00027E8C
	public static Gradient CreateGradient(Color clr1, float time1, Color clr2, float time2)
	{
		Gradient gradient = new Gradient();
		GradientColorKey[] array = new GradientColorKey[2];
		GradientAlphaKey[] array2 = new GradientAlphaKey[2];
		array[0].color = clr1;
		array[0].time = time1;
		array[1].color = clr2;
		array[1].time = time2;
		array2[0].alpha = 1f;
		array2[0].time = 0f;
		array2[1].alpha = 1f;
		array2[1].time = 1f;
		gradient.SetKeys(array, array2);
		return gradient;
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x00029D2C File Offset: 0x00027F2C
	public static Gradient CreateGradient(List<Color> clrs, List<float> times)
	{
		Gradient gradient = new Gradient();
		GradientColorKey[] array = new GradientColorKey[clrs.Count];
		GradientAlphaKey[] array2 = new GradientAlphaKey[2];
		for (int i = 0; i < clrs.Count; i++)
		{
			array[i].color = clrs[i];
			array[i].time = times[i];
		}
		array2[0].alpha = 1f;
		array2[0].time = 0f;
		array2[1].alpha = 1f;
		array2[1].time = 1f;
		gradient.SetKeys(array, array2);
		return gradient;
	}

	// Token: 0x06000407 RID: 1031 RVA: 0x00029DD8 File Offset: 0x00027FD8
	public static Gradient CreateGradient(List<Color> clrs, List<float> times, List<float> alpha, List<float> timesAlpha)
	{
		Gradient gradient = new Gradient();
		GradientColorKey[] array = new GradientColorKey[clrs.Count];
		GradientAlphaKey[] array2 = new GradientAlphaKey[alpha.Count];
		for (int i = 0; i < clrs.Count; i++)
		{
			array[i].color = clrs[i];
			array[i].time = times[i];
		}
		for (int j = 0; j < alpha.Count; j++)
		{
			array2[j].alpha = alpha[j];
			array2[j].time = timesAlpha[j];
		}
		gradient.SetKeys(array, array2);
		return gradient;
	}

	// Token: 0x06000408 RID: 1032 RVA: 0x00029E84 File Offset: 0x00028084
	public static Color GetColor(string hex)
	{
		Color result = default(Color);
		ColorUtility.TryParseHtmlString(hex, out result);
		return result;
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x00029EA4 File Offset: 0x000280A4
	public static Keyframe CreateKey(float value, float time)
	{
		return new Keyframe
		{
			value = value,
			time = time
		};
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x00029ECC File Offset: 0x000280CC
	public static Keyframe CreateKey(float value, float time, float inTangent, float outTangent)
	{
		return new Keyframe
		{
			value = value,
			time = time,
			inTangent = inTangent,
			outTangent = outTangent
		};
	}
}
