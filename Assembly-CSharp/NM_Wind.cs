using System;
using UnityEngine;

// Token: 0x02000101 RID: 257
[ExecuteInEditMode]
public class NM_Wind : MonoBehaviour
{
	// Token: 0x06000689 RID: 1673 RVA: 0x0004E19F File Offset: 0x0004C39F
	private void Start()
	{
		this.ApplySettings();
	}

	// Token: 0x0600068A RID: 1674 RVA: 0x0004E19F File Offset: 0x0004C39F
	private void Update()
	{
		this.ApplySettings();
	}

	// Token: 0x0600068B RID: 1675 RVA: 0x0004E19F File Offset: 0x0004C39F
	private void OnValidate()
	{
		this.ApplySettings();
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x0004E1A8 File Offset: 0x0004C3A8
	private void ApplySettings()
	{
		Shader.SetGlobalTexture("WIND_SETTINGS_TexNoise", this.NoiseTexture);
		Shader.SetGlobalTexture("WIND_SETTINGS_TexGust", this.GustMaskTexture);
		Shader.SetGlobalVector("WIND_SETTINGS_WorldDirectionAndSpeed", this.GetDirectionAndSpeed());
		Shader.SetGlobalFloat("WIND_SETTINGS_FlexNoiseScale", 1f / Mathf.Max(0.01f, this.FlexNoiseWorldSize));
		Shader.SetGlobalFloat("WIND_SETTINGS_ShiverNoiseScale", 1f / Mathf.Max(0.01f, this.ShiverNoiseWorldSize));
		Shader.SetGlobalFloat("WIND_SETTINGS_Turbulence", this.WindSpeed * this.Turbulence);
		Shader.SetGlobalFloat("WIND_SETTINGS_GustSpeed", this.GustSpeed);
		Shader.SetGlobalFloat("WIND_SETTINGS_GustScale", this.GustScale);
		Shader.SetGlobalFloat("WIND_SETTINGS_GustWorldScale", 1f / Mathf.Max(0.01f, this.GustWorldSize));
	}

	// Token: 0x0600068D RID: 1677 RVA: 0x0004E27C File Offset: 0x0004C47C
	private Vector4 GetDirectionAndSpeed()
	{
		Vector3 normalized = base.transform.forward.normalized;
		return new Vector4(normalized.x, normalized.y, normalized.z, this.WindSpeed * 0.2777f);
	}

	// Token: 0x04000DB9 RID: 3513
	[Header("General Parameters")]
	[Tooltip("Wind Speed in Kilometers per hour")]
	public float WindSpeed = 30f;

	// Token: 0x04000DBA RID: 3514
	[Range(0f, 2f)]
	[Tooltip("Wind Turbulence in percentage of wind Speed")]
	public float Turbulence = 0.25f;

	// Token: 0x04000DBB RID: 3515
	[Header("Noise Parameters")]
	[Tooltip("Texture used for wind turbulence")]
	public Texture2D NoiseTexture;

	// Token: 0x04000DBC RID: 3516
	[Tooltip("Size of one world tiling patch of the Noise Texture, for bending trees")]
	public float FlexNoiseWorldSize = 175f;

	// Token: 0x04000DBD RID: 3517
	[Tooltip("Size of one world tiling patch of the Noise Texture, for leaf shivering")]
	public float ShiverNoiseWorldSize = 10f;

	// Token: 0x04000DBE RID: 3518
	[Header("Gust Parameters")]
	[Tooltip("Texture used for wind gusts")]
	public Texture2D GustMaskTexture;

	// Token: 0x04000DBF RID: 3519
	[Tooltip("Size of one world tiling patch of the Gust Texture, for leaf shivering")]
	public float GustWorldSize = 600f;

	// Token: 0x04000DC0 RID: 3520
	[Tooltip("Wind Gust Speed in Kilometers per hour")]
	public float GustSpeed = 50f;

	// Token: 0x04000DC1 RID: 3521
	[Tooltip("Wind Gust Influence on trees")]
	public float GustScale = 1f;
}
