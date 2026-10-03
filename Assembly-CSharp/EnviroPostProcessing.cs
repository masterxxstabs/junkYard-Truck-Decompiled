using System;
using UnityEngine;

// Token: 0x0200006B RID: 107
[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
[RequireComponent(typeof(Camera))]
public class EnviroPostProcessing : MonoBehaviour
{
	// Token: 0x060001C2 RID: 450 RVA: 0x000122E8 File Offset: 0x000104E8
	private void OnEnable()
	{
		if (this.cam == null)
		{
			this.cam = base.GetComponent<Camera>();
		}
		this.CreateMaterialsAndTextures();
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x0001230A File Offset: 0x0001050A
	private void OnDisable()
	{
		this.CleanupMaterials();
		if (this.cam.actualRenderingPath == RenderingPath.Forward && this.cam.depthTextureMode == DepthTextureMode.Depth && this.cam.depthTextureMode == DepthTextureMode.Depth)
		{
			this.cam.depthTextureMode = DepthTextureMode.None;
		}
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x00012348 File Offset: 0x00010548
	private void CreateMaterialsAndTextures()
	{
		this.sunShaftsMaterial = new Material(Shader.Find("Enviro/Effects/LightShafts"));
		this.moonShaftsMaterial = new Material(Shader.Find("Enviro/Effects/LightShafts"));
		this.simpleSunClearMaterial = new Material(Shader.Find("Enviro/Effects/ClearLightShafts"));
		this.simpleMoonClearMaterial = new Material(Shader.Find("Enviro/Effects/ClearLightShafts"));
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x000123AC File Offset: 0x000105AC
	private void CleanupMaterials()
	{
		if (this.sunShaftsMaterial != null)
		{
			Object.DestroyImmediate(this.sunShaftsMaterial);
		}
		if (this.moonShaftsMaterial != null)
		{
			Object.DestroyImmediate(this.moonShaftsMaterial);
		}
		if (this.simpleSunClearMaterial != null)
		{
			Object.DestroyImmediate(this.simpleSunClearMaterial);
		}
		if (this.simpleMoonClearMaterial != null)
		{
			Object.DestroyImmediate(this.simpleMoonClearMaterial);
		}
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x00012420 File Offset: 0x00010620
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (EnviroSkyMgr.instance == null || !EnviroSkyMgr.instance.IsAvailable() || (!EnviroSkyMgr.instance.useSunShafts && !EnviroSkyMgr.instance.useMoonShafts))
		{
			Graphics.Blit(source, destination);
			if (this.cam.actualRenderingPath == RenderingPath.Forward && this.cam.depthTextureMode == DepthTextureMode.Depth)
			{
				this.cam.depthTextureMode = DepthTextureMode.None;
			}
			return;
		}
		if (this.cam.actualRenderingPath == RenderingPath.Forward)
		{
			this.cam.depthTextureMode |= DepthTextureMode.Depth;
		}
		RenderTextureDescriptor desc = new RenderTextureDescriptor(source.width, source.height, source.format, source.depth);
		desc.msaaSamples = source.antiAliasing;
		if (this.cam.stereoEnabled && EnviroSky.instance.singlePassVR)
		{
			desc.vrUsage = VRTextureUsage.TwoEyes;
		}
		this.tempTexture = RenderTexture.GetTemporary(desc);
		if (this.sunShaftsMaterial == null)
		{
			this.CleanupMaterials();
			this.CreateMaterialsAndTextures();
		}
		if (EnviroSkyMgr.instance.useSunShafts && EnviroSkyMgr.instance.useMoonShafts)
		{
			this.RenderLightShaft(source, this.tempTexture, this.sunShaftsMaterial, this.simpleSunClearMaterial, EnviroSkyMgr.instance.Components.Sun.transform, EnviroSkyMgr.instance.LightShaftsSettings.thresholdColorSun.Evaluate(EnviroSkyMgr.instance.Time.solarTime), EnviroSkyMgr.instance.LightShaftsSettings.lightShaftsColorSun.Evaluate(EnviroSkyMgr.instance.Time.solarTime));
			this.RenderLightShaft(this.tempTexture, destination, this.moonShaftsMaterial, this.simpleMoonClearMaterial, EnviroSkyMgr.instance.Components.Moon.transform, EnviroSkyMgr.instance.LightShaftsSettings.thresholdColorMoon.Evaluate(EnviroSkyMgr.instance.Time.solarTime), EnviroSkyMgr.instance.LightShaftsSettings.lightShaftsColorMoon.Evaluate(EnviroSkyMgr.instance.Time.solarTime));
		}
		else if (EnviroSkyMgr.instance.useSunShafts)
		{
			this.RenderLightShaft(source, destination, this.sunShaftsMaterial, this.simpleSunClearMaterial, EnviroSkyMgr.instance.Components.Sun.transform, EnviroSkyMgr.instance.LightShaftsSettings.thresholdColorSun.Evaluate(EnviroSkyMgr.instance.Time.solarTime), EnviroSkyMgr.instance.LightShaftsSettings.lightShaftsColorSun.Evaluate(EnviroSkyMgr.instance.Time.solarTime));
		}
		else if (EnviroSkyMgr.instance.useMoonShafts)
		{
			this.RenderLightShaft(source, destination, this.moonShaftsMaterial, this.simpleMoonClearMaterial, EnviroSkyMgr.instance.Components.Moon.transform, EnviroSkyMgr.instance.LightShaftsSettings.thresholdColorMoon.Evaluate(EnviroSkyMgr.instance.Time.solarTime), EnviroSkyMgr.instance.LightShaftsSettings.lightShaftsColorMoon.Evaluate(EnviroSkyMgr.instance.Time.solarTime));
		}
		else
		{
			Graphics.Blit(source, destination);
		}
		RenderTexture.ReleaseTemporary(this.tempTexture);
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x00012738 File Offset: 0x00010938
	private void RenderLightShaft(RenderTexture source, RenderTexture destination, Material mat, Material clearMat, Transform lightSource, Color treshold, Color clr)
	{
		int num = 4;
		if (EnviroSkyMgr.instance.LightShaftsSettings.resolution == EnviroPostProcessing.SunShaftsResolution.Normal)
		{
			num = 2;
		}
		else if (EnviroSkyMgr.instance.LightShaftsSettings.resolution == EnviroPostProcessing.SunShaftsResolution.High)
		{
			num = 1;
		}
		Vector3 vector = Vector3.one * 0.5f;
		if (lightSource)
		{
			vector = this.cam.WorldToViewportPoint(lightSource.position);
		}
		else
		{
			vector = new Vector3(0.5f, 0.5f, 0f);
		}
		int width = source.width / num;
		int height = source.height / num;
		RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default, 1, RenderTextureMemoryless.None, source.vrUsage);
		mat.SetVector("_BlurRadius4", new Vector4(1f, 1f, 0f, 0f) * EnviroSkyMgr.instance.LightShaftsSettings.blurRadius);
		mat.SetVector("_SunPosition", new Vector4(vector.x, vector.y, vector.z, EnviroSkyMgr.instance.LightShaftsSettings.maxRadius));
		mat.SetVector("_SunThreshold", treshold);
		if (!EnviroSkyMgr.instance.LightShaftsSettings.useDepthTexture)
		{
			RenderTextureFormat format = EnviroSkyMgr.instance.Camera.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
			RenderTexture temporary2 = RenderTexture.GetTemporary(source.width, source.height, 0, format);
			RenderTexture.active = temporary2;
			GL.ClearWithSkybox(false, this.cam);
			mat.SetTexture("_Skybox", temporary2);
			Graphics.Blit(source, temporary, mat, 3);
			RenderTexture.ReleaseTemporary(temporary2);
		}
		else
		{
			Graphics.Blit(source, temporary, mat, 2);
		}
		if (this.cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Mono)
		{
			this.DrawBorder(temporary, clearMat);
		}
		this.radialBlurIterations = Mathf.Clamp(this.radialBlurIterations, 1, 4);
		float num2 = EnviroSkyMgr.instance.LightShaftsSettings.blurRadius * 0.0013020834f;
		mat.SetVector("_BlurRadius4", new Vector4(num2, num2, 0f, 0f));
		mat.SetVector("_SunPosition", new Vector4(vector.x, vector.y, vector.z, EnviroSkyMgr.instance.LightShaftsSettings.maxRadius));
		for (int i = 0; i < this.radialBlurIterations; i++)
		{
			RenderTexture temporary3 = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default, 1, RenderTextureMemoryless.None, source.vrUsage);
			Graphics.Blit(temporary, temporary3, mat, 1);
			RenderTexture.ReleaseTemporary(temporary);
			num2 = EnviroSkyMgr.instance.LightShaftsSettings.blurRadius * (((float)i * 2f + 1f) * 6f) / 768f;
			mat.SetVector("_BlurRadius4", new Vector4(num2, num2, 0f, 0f));
			temporary = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default, 1, RenderTextureMemoryless.None, source.vrUsage);
			Graphics.Blit(temporary3, temporary, mat, 1);
			RenderTexture.ReleaseTemporary(temporary3);
			num2 = EnviroSkyMgr.instance.LightShaftsSettings.blurRadius * (((float)i * 2f + 2f) * 6f) / 768f;
			mat.SetVector("_BlurRadius4", new Vector4(num2, num2, 0f, 0f));
		}
		if (vector.z >= 0f)
		{
			mat.SetVector("_SunColor", new Vector4(clr.r, clr.g, clr.b, clr.a) * EnviroSkyMgr.instance.LightShaftsSettings.intensity);
		}
		else
		{
			mat.SetVector("_SunColor", Vector4.zero);
		}
		mat.SetTexture("_ColorBuffer", temporary);
		Graphics.Blit(source, destination, mat, (EnviroSkyMgr.instance.LightShaftsSettings.screenBlendMode == EnviroPostProcessing.ShaftsScreenBlendMode.Screen) ? 0 : 4);
		RenderTexture.ReleaseTemporary(temporary);
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x00012AF0 File Offset: 0x00010CF0
	private void DrawBorder(RenderTexture dest, Material material)
	{
		RenderTexture.active = dest;
		bool flag = true;
		GL.PushMatrix();
		GL.LoadOrtho();
		for (int i = 0; i < material.passCount; i++)
		{
			material.SetPass(i);
			float y;
			float y2;
			if (flag)
			{
				y = 1f;
				y2 = 0f;
			}
			else
			{
				y = 0f;
				y2 = 1f;
			}
			float x = 0f;
			float x2 = 0f + 1f / ((float)dest.width * 1f);
			float y3 = 0f;
			float y4 = 1f;
			GL.Begin(7);
			GL.TexCoord2(0f, y);
			GL.Vertex3(x, y3, 0.1f);
			GL.TexCoord2(1f, y);
			GL.Vertex3(x2, y3, 0.1f);
			GL.TexCoord2(1f, y2);
			GL.Vertex3(x2, y4, 0.1f);
			GL.TexCoord2(0f, y2);
			GL.Vertex3(x, y4, 0.1f);
			float x3 = 1f - 1f / ((float)dest.width * 1f);
			x2 = 1f;
			y3 = 0f;
			y4 = 1f;
			GL.TexCoord2(0f, y);
			GL.Vertex3(x3, y3, 0.1f);
			GL.TexCoord2(1f, y);
			GL.Vertex3(x2, y3, 0.1f);
			GL.TexCoord2(1f, y2);
			GL.Vertex3(x2, y4, 0.1f);
			GL.TexCoord2(0f, y2);
			GL.Vertex3(x3, y4, 0.1f);
			float x4 = 0f;
			x2 = 1f;
			y3 = 0f;
			y4 = 0f + 1f / ((float)dest.height * 1f);
			GL.TexCoord2(0f, y);
			GL.Vertex3(x4, y3, 0.1f);
			GL.TexCoord2(1f, y);
			GL.Vertex3(x2, y3, 0.1f);
			GL.TexCoord2(1f, y2);
			GL.Vertex3(x2, y4, 0.1f);
			GL.TexCoord2(0f, y2);
			GL.Vertex3(x4, y4, 0.1f);
			float x5 = 0f;
			x2 = 1f;
			y3 = 1f - 1f / ((float)dest.height * 1f);
			y4 = 1f;
			GL.TexCoord2(0f, y);
			GL.Vertex3(x5, y3, 0.1f);
			GL.TexCoord2(1f, y);
			GL.Vertex3(x2, y3, 0.1f);
			GL.TexCoord2(1f, y2);
			GL.Vertex3(x2, y4, 0.1f);
			GL.TexCoord2(0f, y2);
			GL.Vertex3(x5, y4, 0.1f);
			GL.End();
		}
		GL.PopMatrix();
	}

	// Token: 0x040004E4 RID: 1252
	private Camera cam;

	// Token: 0x040004E5 RID: 1253
	[HideInInspector]
	public int radialBlurIterations = 2;

	// Token: 0x040004E6 RID: 1254
	private Material sunShaftsMaterial;

	// Token: 0x040004E7 RID: 1255
	private Material moonShaftsMaterial;

	// Token: 0x040004E8 RID: 1256
	private Material simpleSunClearMaterial;

	// Token: 0x040004E9 RID: 1257
	private Material simpleMoonClearMaterial;

	// Token: 0x040004EA RID: 1258
	private RenderTexture tempTexture;

	// Token: 0x02000371 RID: 881
	public enum SunShaftsResolution
	{
		// Token: 0x040026D4 RID: 9940
		Low,
		// Token: 0x040026D5 RID: 9941
		Normal,
		// Token: 0x040026D6 RID: 9942
		High
	}

	// Token: 0x02000372 RID: 882
	public enum ShaftsScreenBlendMode
	{
		// Token: 0x040026D8 RID: 9944
		Screen,
		// Token: 0x040026D9 RID: 9945
		Add
	}
}
