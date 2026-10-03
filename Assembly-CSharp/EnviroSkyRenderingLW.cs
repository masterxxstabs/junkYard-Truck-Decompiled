using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020000A1 RID: 161
[RequireComponent(typeof(Camera))]
public class EnviroSkyRenderingLW : MonoBehaviour
{
	// Token: 0x06000393 RID: 915 RVA: 0x000215A4 File Offset: 0x0001F7A4
	private void Start()
	{
		if (EnviroSkyMgr.instance == null || EnviroSkyMgr.instance.currentEnviroSkyVersion != EnviroSkyMgr.EnviroSkyVersion.LW)
		{
			Debug.Log("Deactivated EnviroSkyRenderingLW component. Not in LW mode or Manager missing.");
			base.enabled = false;
			return;
		}
		this.myCam = base.GetComponent<Camera>();
		if (this.myCam.actualRenderingPath == RenderingPath.Forward)
		{
			this.myCam.depthTextureMode = DepthTextureMode.Depth;
		}
	}

	// Token: 0x06000394 RID: 916 RVA: 0x00021603 File Offset: 0x0001F803
	private void OnEnable()
	{
		this.CreateFogMaterial();
	}

	// Token: 0x06000395 RID: 917 RVA: 0x0002160B File Offset: 0x0001F80B
	private void OnDisable()
	{
		this.DestroyFogMaterial();
		if (this.myCam.actualRenderingPath == RenderingPath.Forward && this.myCam.depthTextureMode == DepthTextureMode.Depth)
		{
			this.myCam.depthTextureMode = DepthTextureMode.None;
		}
	}

	// Token: 0x06000396 RID: 918 RVA: 0x0002163C File Offset: 0x0001F83C
	private void CreateFogMaterial()
	{
		if (this.material != null)
		{
			Object.DestroyImmediate(this.material);
		}
		if (!this.simpleFog)
		{
			Shader shader = Shader.Find("Enviro/Lite/EnviroFogRendering");
			if (shader == null)
			{
				throw new Exception("Critical Error: \"Enviro/EnviroFogRendering\" shader is missing.");
			}
			this.material = new Material(shader);
		}
		else
		{
			Shader shader2 = Shader.Find("Enviro/Lite/EnviroFogRenderingSimple");
			if (shader2 == null)
			{
				throw new Exception("Critical Error: \"Enviro/EnviroFogRendering\" shader is missing.");
			}
			this.material = new Material(shader2);
		}
		if (EnviroSkyMgr.instance.FogSettings.useSimpleFog)
		{
			Shader.EnableKeyword("ENVIRO_SIMPLE_FOG");
			return;
		}
		Shader.DisableKeyword("ENVIRO_SIMPLE_FOG");
	}

	// Token: 0x06000397 RID: 919 RVA: 0x000216E8 File Offset: 0x0001F8E8
	private void DestroyFogMaterial()
	{
		if (this.material != null)
		{
			Object.Destroy(this.material);
		}
	}

	// Token: 0x06000398 RID: 920 RVA: 0x00021703 File Offset: 0x0001F903
	private void Update()
	{
		if (this.currentSimpleFog != this.simpleFog)
		{
			this.CreateFogMaterial();
			this.currentSimpleFog = this.simpleFog;
		}
	}

	// Token: 0x06000399 RID: 921 RVA: 0x00021728 File Offset: 0x0001F928
	private void OnPreRender()
	{
		if (this.myCam.stereoEnabled)
		{
			Matrix4x4 inverse = this.myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Left).inverse;
			Matrix4x4 inverse2 = this.myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right).inverse;
			Matrix4x4 stereoProjectionMatrix = this.myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
			Matrix4x4 stereoProjectionMatrix2 = this.myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
			Matrix4x4 inverse3 = GL.GetGPUProjectionMatrix(stereoProjectionMatrix, true).inverse;
			Matrix4x4 inverse4 = GL.GetGPUProjectionMatrix(stereoProjectionMatrix2, true).inverse;
			if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLCore && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES3 && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES2)
			{
				ref Matrix4x4 ptr = ref inverse3;
				ptr[1, 1] = ptr[1, 1] * -1f;
				ptr = ref inverse4;
				ptr[1, 1] = ptr[1, 1] * -1f;
			}
			Shader.SetGlobalMatrix("_LeftWorldFromView", inverse);
			Shader.SetGlobalMatrix("_RightWorldFromView", inverse2);
			Shader.SetGlobalMatrix("_LeftViewFromScreen", inverse3);
			Shader.SetGlobalMatrix("_RightViewFromScreen", inverse4);
			return;
		}
		Matrix4x4 cameraToWorldMatrix = this.myCam.cameraToWorldMatrix;
		Matrix4x4 inverse5 = GL.GetGPUProjectionMatrix(this.myCam.projectionMatrix, true).inverse;
		if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLCore && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES3 && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES2)
		{
			ref Matrix4x4 ptr = ref inverse5;
			ptr[1, 1] = ptr[1, 1] * -1f;
		}
		Shader.SetGlobalMatrix("_LeftWorldFromView", cameraToWorldMatrix);
		Shader.SetGlobalMatrix("_LeftViewFromScreen", inverse5);
	}

	// Token: 0x0600039A RID: 922 RVA: 0x000218A4 File Offset: 0x0001FAA4
	[ImageEffectOpaque]
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (EnviroSkyLite.instance == null)
		{
			Graphics.Blit(source, destination);
			return;
		}
		if (this.myCam.actualRenderingPath == RenderingPath.Forward)
		{
			this.myCam.depthTextureMode |= DepthTextureMode.Depth;
		}
		float num = this.myCam.transform.position.y - EnviroSkyLite.instance.fogSettings.height;
		float z = (num <= 0f) ? 1f : 0f;
		FogMode fogMode = RenderSettings.fogMode;
		float fogDensity = RenderSettings.fogDensity;
		float fogStartDistance = RenderSettings.fogStartDistance;
		float fogEndDistance = RenderSettings.fogEndDistance;
		bool flag = fogMode == FogMode.Linear;
		float num2 = flag ? (fogEndDistance - fogStartDistance) : 0f;
		float num3 = (Mathf.Abs(num2) > 0.0001f) ? (1f / num2) : 0f;
		Vector4 value;
		value.x = fogDensity * 1.2011224f;
		value.y = fogDensity * 1.442695f;
		value.z = (flag ? (-num3) : 0f);
		value.w = (flag ? (fogEndDistance * num3) : 0f);
		Shader.SetGlobalVector("_SceneFogParams", value);
		Shader.SetGlobalVector("_SceneFogMode", new Vector4((float)fogMode, (float)(EnviroSkyLite.instance.fogSettings.useRadialDistance ? 1 : 0), 0f, 0f));
		Shader.SetGlobalVector("_HeightParams", new Vector4(EnviroSkyLite.instance.fogSettings.height, num, z, EnviroSkyLite.instance.fogSettings.heightDensity * 0.5f));
		Shader.SetGlobalVector("_DistanceParams", new Vector4(-Mathf.Max(EnviroSkyLite.instance.fogSettings.startDistance, 0f), 0f, 0f, 0f));
		this.material.SetTexture("_MainTex", source);
		Graphics.Blit(source, destination, this.material);
	}

	// Token: 0x0400079D RID: 1949
	[HideInInspector]
	public bool isAddionalCamera;

	// Token: 0x0400079E RID: 1950
	private Camera myCam;

	// Token: 0x0400079F RID: 1951
	[HideInInspector]
	public Material material;

	// Token: 0x040007A0 RID: 1952
	[HideInInspector]
	public bool simpleFog;

	// Token: 0x040007A1 RID: 1953
	private bool currentSimpleFog;
}
