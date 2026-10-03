using System;
using UnityEngine;

// Token: 0x02000094 RID: 148
public class EnviroWeatherPresetCreation
{
	// Token: 0x0600032E RID: 814 RVA: 0x00018FC3 File Offset: 0x000171C3
	public static GameObject GetAssetPrefab(string name)
	{
		return null;
	}

	// Token: 0x0600032F RID: 815 RVA: 0x00018FC3 File Offset: 0x000171C3
	public static Cubemap GetAssetCubemap(string name)
	{
		return null;
	}

	// Token: 0x06000330 RID: 816 RVA: 0x00018FC3 File Offset: 0x000171C3
	public static Texture GetAssetTexture(string name)
	{
		return null;
	}

	// Token: 0x06000331 RID: 817 RVA: 0x0001D024 File Offset: 0x0001B224
	public static Gradient CreateGradient()
	{
		Gradient gradient = new Gradient();
		GradientColorKey[] array = new GradientColorKey[2];
		GradientAlphaKey[] array2 = new GradientAlphaKey[2];
		array[0].color = Color.white;
		array[0].time = 0f;
		array[1].color = Color.white;
		array[1].time = 0f;
		array2[0].alpha = 0f;
		array2[0].time = 0f;
		array2[1].alpha = 0f;
		array2[1].time = 1f;
		gradient.SetKeys(array, array2);
		return gradient;
	}

	// Token: 0x06000332 RID: 818 RVA: 0x0001D0D4 File Offset: 0x0001B2D4
	public static Color GetColor(string hex)
	{
		Color result = default(Color);
		ColorUtility.TryParseHtmlString(hex, out result);
		return result;
	}

	// Token: 0x06000333 RID: 819 RVA: 0x0001D0F4 File Offset: 0x0001B2F4
	public static Keyframe CreateKey(float value, float time)
	{
		return new Keyframe
		{
			value = value,
			time = time
		};
	}

	// Token: 0x06000334 RID: 820 RVA: 0x0001D11C File Offset: 0x0001B31C
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
