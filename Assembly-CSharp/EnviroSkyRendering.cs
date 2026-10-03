using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020000A6 RID: 166
[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
[RequireComponent(typeof(Camera))]
public class EnviroSkyRendering : MonoBehaviour
{
	// Token: 0x1400000A RID: 10
	// (add) Token: 0x060003D2 RID: 978 RVA: 0x00025A2C File Offset: 0x00023C2C
	// (remove) Token: 0x060003D3 RID: 979 RVA: 0x00025A60 File Offset: 0x00023C60
	public static event Action<EnviroSkyRendering, Matrix4x4, Matrix4x4> PreRenderEvent;

	// Token: 0x17000037 RID: 55
	// (get) Token: 0x060003D4 RID: 980 RVA: 0x00025A93 File Offset: 0x00023C93
	public CommandBuffer GlobalCommandBuffer
	{
		get
		{
			return this._preLightPass;
		}
	}

	// Token: 0x17000038 RID: 56
	// (get) Token: 0x060003D5 RID: 981 RVA: 0x00025A9B File Offset: 0x00023C9B
	public CommandBuffer GlobalCommandBufferForward
	{
		get
		{
			return this._afterLightPass;
		}
	}

	// Token: 0x060003D6 RID: 982 RVA: 0x00025AA3 File Offset: 0x00023CA3
	public static Material GetLightMaterial()
	{
		return EnviroSkyRendering._lightMaterial;
	}

	// Token: 0x060003D7 RID: 983 RVA: 0x00025AAA File Offset: 0x00023CAA
	public static Mesh GetPointLightMesh()
	{
		return EnviroSkyRendering._pointLightMesh;
	}

	// Token: 0x060003D8 RID: 984 RVA: 0x00025AB1 File Offset: 0x00023CB1
	public static Mesh GetSpotLightMesh()
	{
		return EnviroSkyRendering._spotLightMesh;
	}

	// Token: 0x060003D9 RID: 985 RVA: 0x00025AB8 File Offset: 0x00023CB8
	public RenderTexture GetVolumeLightBuffer()
	{
		if (EnviroSky.instance.volumeLightSettings.Resolution == EnviroSkyRendering.VolumtericResolution.Quarter)
		{
			return this._quarterVolumeLightTexture;
		}
		if (EnviroSky.instance.volumeLightSettings.Resolution == EnviroSkyRendering.VolumtericResolution.Half)
		{
			return this._halfVolumeLightTexture;
		}
		return this._volumeLightTexture;
	}

	// Token: 0x060003DA RID: 986 RVA: 0x00025AF2 File Offset: 0x00023CF2
	public RenderTexture GetVolumeLightDepthBuffer()
	{
		if (EnviroSky.instance.volumeLightSettings.Resolution == EnviroSkyRendering.VolumtericResolution.Quarter)
		{
			return this._quarterDepthBuffer;
		}
		if (EnviroSky.instance.volumeLightSettings.Resolution == EnviroSkyRendering.VolumtericResolution.Half)
		{
			return this._halfDepthBuffer;
		}
		return null;
	}

	// Token: 0x060003DB RID: 987 RVA: 0x00025B27 File Offset: 0x00023D27
	public static Texture GetDefaultSpotCookie()
	{
		return EnviroSkyRendering._defaultSpotCookie;
	}

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x060003DC RID: 988 RVA: 0x00025B2E File Offset: 0x00023D2E
	public float thresholdGamma
	{
		get
		{
			return Mathf.Max(0f, 0f);
		}
	}

	// Token: 0x1700003A RID: 58
	// (get) Token: 0x060003DD RID: 989 RVA: 0x00025B3F File Offset: 0x00023D3F
	public float thresholdLinear
	{
		get
		{
			return Mathf.GammaToLinearSpace(this.thresholdGamma);
		}
	}

	// Token: 0x060003DE RID: 990 RVA: 0x00025B4C File Offset: 0x00023D4C
	private void OnEnable()
	{
		if (this.myCam == null)
		{
			this.myCam = base.GetComponent<Camera>();
		}
		if (EnviroSky.instance == null)
		{
			base.enabled = false;
			return;
		}
		this.CreateMaterialsAndTextures();
		this.SetupVolumeFog();
		this.CreateCommandBuffer();
		this.CreateFogMaterial();
		this.UpdateQualitySettings();
		if (EnviroSky.instance != null)
		{
			this.SetReprojectionPixelSize(this.usedCloudsQuality.reprojectionPixelSize);
		}
		if (this.isAddionalCamera && !this.useGlobalRenderingSettings && !this.customRenderingSettings.useVolumeLighting && this.fogMat != null)
		{
			this.fogMat.DisableKeyword("ENVIROVOLUMELIGHT");
			this.fogMat.SetTexture("_EnviroVolumeLightingTex", this.blackTexture);
		}
	}

	// Token: 0x060003DF RID: 991 RVA: 0x00025C15 File Offset: 0x00023E15
	private void OnDisable()
	{
		this.RemoveCommandBuffer();
		this.CleanupMaterials();
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x00025C24 File Offset: 0x00023E24
	private void CleanupMaterials()
	{
		if (this.postProcessMat != null)
		{
			Object.DestroyImmediate(this.postProcessMat);
		}
		if (this.volumeLightMat != null)
		{
			Object.DestroyImmediate(this.volumeLightMat);
		}
		if (this._bilateralBlurMaterial != null)
		{
			Object.DestroyImmediate(this._bilateralBlurMaterial);
		}
		if (this.cloudsMat != null)
		{
			Object.DestroyImmediate(this.cloudsMat);
		}
		if (this.fogMat != null)
		{
			Object.DestroyImmediate(this.fogMat);
		}
		if (this.blitMat != null)
		{
			Object.DestroyImmediate(this.blitMat);
		}
		if (this.compose != null)
		{
			Object.DestroyImmediate(this.compose);
		}
		if (this.downsample != null)
		{
			Object.DestroyImmediate(this.downsample);
		}
	}

	// Token: 0x060003E1 RID: 993 RVA: 0x00025CFC File Offset: 0x00023EFC
	private void CreateCommandBuffer()
	{
		this._preLightPass = new CommandBuffer();
		this._preLightPass.name = "PreLight";
		this._afterLightPass = new CommandBuffer();
		this._afterLightPass.name = "AfterLight";
		if (this.myCam.actualRenderingPath == RenderingPath.Forward)
		{
			this.myCam.AddCommandBuffer(CameraEvent.AfterDepthTexture, this._preLightPass);
			this.myCam.AddCommandBuffer(CameraEvent.AfterForwardOpaque, this._afterLightPass);
			return;
		}
		this.myCam.AddCommandBuffer(CameraEvent.BeforeLighting, this._preLightPass);
	}

	// Token: 0x060003E2 RID: 994 RVA: 0x00025D88 File Offset: 0x00023F88
	private void RemoveCommandBuffer()
	{
		if (this.myCam.actualRenderingPath == RenderingPath.Forward)
		{
			if (this._preLightPass != null)
			{
				this.myCam.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, this._preLightPass);
			}
			if (this._afterLightPass != null)
			{
				this.myCam.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, this._afterLightPass);
				return;
			}
		}
		else if (this._preLightPass != null)
		{
			this.myCam.RemoveCommandBuffer(CameraEvent.BeforeLighting, this._preLightPass);
		}
	}

	// Token: 0x060003E3 RID: 995 RVA: 0x00025DF4 File Offset: 0x00023FF4
	private void SetupVolumeFog()
	{
		if (EnviroSky.instance == null)
		{
			return;
		}
		this.currentVolumeRes = EnviroSky.instance.volumeLightSettings.Resolution;
		this.ChangeResolution();
		if (EnviroSkyRendering._pointLightMesh == null)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			EnviroSkyRendering._pointLightMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
			Object.DestroyImmediate(gameObject);
		}
		if (EnviroSkyRendering._spotLightMesh == null)
		{
			EnviroSkyRendering._spotLightMesh = this.CreateSpotLightMesh();
		}
		if (EnviroSkyRendering._lightMaterial == null)
		{
			Shader shader = Shader.Find("Enviro/Standard/VolumeLight");
			if (shader == null)
			{
				throw new Exception("Critical Error: \"Enviro/VolumeLight\" shader is missing.");
			}
			EnviroSkyRendering._lightMaterial = new Material(shader);
		}
		if (EnviroSkyRendering._defaultSpotCookie == null)
		{
			EnviroSkyRendering._defaultSpotCookie = this.DefaultSpotCookie;
		}
		this.GenerateDitherTexture();
	}

	// Token: 0x060003E4 RID: 996 RVA: 0x00025EC0 File Offset: 0x000240C0
	private void ChangeResolution()
	{
		int pixelWidth = this.myCam.pixelWidth;
		int pixelHeight = this.myCam.pixelHeight;
		if (pixelWidth <= 0 || pixelHeight <= 0)
		{
			return;
		}
		if (this._volumeLightTexture != null)
		{
			Object.DestroyImmediate(this._volumeLightTexture);
		}
		this._volumeLightTexture = new RenderTexture(pixelWidth, pixelHeight, 0, RenderTextureFormat.ARGBHalf);
		this._volumeLightTexture.name = "VolumeLightBuffer";
		this._volumeLightTexture.filterMode = FilterMode.Bilinear;
		if (this.myCam.stereoEnabled && EnviroSky.instance.singlePassVR)
		{
			if (EnviroSky.instance.volumeLightSettings.Resolution == EnviroSkyRendering.VolumtericResolution.Half || EnviroSky.instance.volumeLightSettings.Resolution == EnviroSkyRendering.VolumtericResolution.Quarter)
			{
				this._volumeLightTexture.vrUsage = VRTextureUsage.None;
			}
			else
			{
				this._volumeLightTexture.vrUsage = VRTextureUsage.TwoEyes;
			}
		}
		if (this._halfDepthBuffer != null)
		{
			Object.DestroyImmediate(this._halfDepthBuffer);
		}
		if (this._halfVolumeLightTexture != null)
		{
			Object.DestroyImmediate(this._halfVolumeLightTexture);
		}
		if (EnviroSky.instance.volumeLightSettings.Resolution == EnviroSkyRendering.VolumtericResolution.Half || EnviroSky.instance.volumeLightSettings.Resolution == EnviroSkyRendering.VolumtericResolution.Quarter)
		{
			this._halfVolumeLightTexture = new RenderTexture(pixelWidth / 2, pixelHeight / 2, 0, RenderTextureFormat.ARGBHalf);
			this._halfVolumeLightTexture.name = "VolumeLightBufferHalf";
			this._halfVolumeLightTexture.filterMode = FilterMode.Bilinear;
			if (this.myCam.stereoEnabled && EnviroSky.instance.singlePassVR)
			{
				this._halfVolumeLightTexture.vrUsage = VRTextureUsage.TwoEyes;
			}
			this._halfDepthBuffer = new RenderTexture(pixelWidth / 2, pixelHeight / 2, 0, RenderTextureFormat.RFloat);
			this._halfDepthBuffer.name = "VolumeLightHalfDepth";
			this._halfDepthBuffer.Create();
			this._halfDepthBuffer.filterMode = FilterMode.Point;
		}
		if (this._quarterVolumeLightTexture != null)
		{
			Object.DestroyImmediate(this._quarterVolumeLightTexture);
		}
		if (this._quarterDepthBuffer != null)
		{
			Object.DestroyImmediate(this._quarterDepthBuffer);
		}
		if (EnviroSky.instance.volumeLightSettings.Resolution == EnviroSkyRendering.VolumtericResolution.Quarter)
		{
			this._quarterVolumeLightTexture = new RenderTexture(pixelWidth / 4, pixelHeight / 4, 0, RenderTextureFormat.ARGBHalf);
			this._quarterVolumeLightTexture.name = "VolumeLightBufferQuarter";
			this._quarterVolumeLightTexture.filterMode = FilterMode.Bilinear;
			if (this.myCam.stereoEnabled && EnviroSky.instance.singlePassVR)
			{
				this._quarterVolumeLightTexture.vrUsage = VRTextureUsage.TwoEyes;
			}
			this._quarterDepthBuffer = new RenderTexture(pixelWidth / 4, pixelHeight / 4, 0, RenderTextureFormat.RFloat);
			this._quarterDepthBuffer.name = "VolumeLightQuarterDepth";
			this._quarterDepthBuffer.Create();
			this._quarterDepthBuffer.filterMode = FilterMode.Point;
		}
	}

	// Token: 0x060003E5 RID: 997 RVA: 0x0002614C File Offset: 0x0002434C
	private void CreateFogMaterial()
	{
		if (EnviroSky.instance == null)
		{
			return;
		}
		if (this.fogMat != null)
		{
			Object.DestroyImmediate(this.fogMat);
		}
		if (!this.useFog)
		{
			Shader shader = Shader.Find("Enviro/Standard/EnviroFogRenderingDisabled");
			if (shader == null)
			{
				throw new Exception("Critical Error: \"Enviro/EnviroFogRenderingDisabled\" shader is missing.");
			}
			this.fogMat = new Material(shader);
			this.currentFogType = EnviroSkyRendering.FogType.Disabled;
			return;
		}
		else if (!EnviroSky.instance.fogSettings.useSimpleFog)
		{
			Shader shader2 = Shader.Find("Enviro/Standard/EnviroFogRendering");
			if (shader2 == null)
			{
				throw new Exception("Critical Error: \"Enviro/EnviroFogRendering\" shader is missing.");
			}
			this.fogMat = new Material(shader2);
			this.currentFogType = EnviroSkyRendering.FogType.Standard;
			return;
		}
		else
		{
			Shader shader3 = Shader.Find("Enviro/Standard/EnviroFogRenderingSimple");
			if (shader3 == null)
			{
				throw new Exception("Critical Error: \"Enviro/EnviroFogRenderingSimple\" shader is missing.");
			}
			this.fogMat = new Material(shader3);
			this.currentFogType = EnviroSkyRendering.FogType.Simple;
			return;
		}
	}

	// Token: 0x060003E6 RID: 998 RVA: 0x00026234 File Offset: 0x00024434
	private void CreateMaterialsAndTextures()
	{
		if (this.cloudsMat == null)
		{
			this.cloudsMat = new Material(Shader.Find("Enviro/Standard/RaymarchClouds"));
		}
		if (this.blitMat == null)
		{
			this.blitMat = new Material(Shader.Find("Enviro/Standard/Blit"));
		}
		if (this.compose == null)
		{
			this.compose = new Material(Shader.Find("Hidden/Enviro/Upsample"));
		}
		if (this.downsample == null)
		{
			this.downsample = new Material(Shader.Find("Hidden/Enviro/DepthDownsample"));
		}
		if (this.volumeLightMat != null)
		{
			this.volumeLightMat = new Material(Shader.Find("Enviro/Standard/VolumeLight"));
		}
		if (this.postProcessMat != null)
		{
			this.postProcessMat = new Material(Shader.Find("Hidden/EnviroDistanceBlur"));
		}
		if (this._bilateralBlurMaterial != null)
		{
			this._bilateralBlurMaterial = new Material(Shader.Find("Hidden/EnviroBilateralBlur"));
		}
		if (this.curlMap == null)
		{
			this.curlMap = (Resources.Load("tex_enviro_curl") as Texture2D);
		}
		if (this.blackTexture == null)
		{
			this.blackTexture = (Resources.Load("tex_enviro_black") as Texture2D);
		}
		if (this.noiseTextureHigh == null)
		{
			this.noiseTextureHigh = (Resources.Load("enviro_clouds_base") as Texture3D);
		}
		if (this.noiseTexture == null)
		{
			this.noiseTexture = (Resources.Load("enviro_clouds_base_low") as Texture3D);
		}
		if (this.detailNoiseTexture == null)
		{
			this.detailNoiseTexture = (Resources.Load("enviro_clouds_detail_low") as Texture3D);
		}
		if (this.detailNoiseTextureHigh == null)
		{
			this.detailNoiseTextureHigh = (Resources.Load("enviro_clouds_detail_high") as Texture3D);
		}
		if (this.dither == null)
		{
			this.dither = (Resources.Load("tex_enviro_dither") as Texture2D);
		}
		if (this.distributionTexture == null)
		{
			this.distributionTexture = (Resources.Load("tex_enviro_linear", typeof(Texture2D)) as Texture2D);
		}
		if (this.blueNoise == null)
		{
			this.blueNoise = (Resources.Load("tex_enviro_blueNoise", typeof(Texture2D)) as Texture2D);
		}
	}

	// Token: 0x060003E7 RID: 999 RVA: 0x00026488 File Offset: 0x00024688
	private void OnPreRender()
	{
		if (EnviroSky.instance == null)
		{
			return;
		}
		if (this.useVolumeLighting)
		{
			if (this._bilateralBlurMaterial == null)
			{
				this._bilateralBlurMaterial = new Material(Shader.Find("Hidden/EnviroBilateralBlur"));
			}
			Matrix4x4 matrix4x = Matrix4x4.Perspective(this.myCam.fieldOfView, this.myCam.aspect, 0.01f, this.myCam.farClipPlane);
			Matrix4x4 matrix4x2 = Matrix4x4.Perspective(this.myCam.fieldOfView, this.myCam.aspect, 0.01f, this.myCam.farClipPlane);
			if (this.myCam.stereoEnabled)
			{
				matrix4x = this.myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
				matrix4x = GL.GetGPUProjectionMatrix(matrix4x, true);
				matrix4x2 = this.myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
				matrix4x2 = GL.GetGPUProjectionMatrix(matrix4x2, true);
			}
			else
			{
				matrix4x = Matrix4x4.Perspective(this.myCam.fieldOfView, this.myCam.aspect, 0.01f, this.myCam.farClipPlane);
				matrix4x = GL.GetGPUProjectionMatrix(matrix4x, true);
			}
			if (this.myCam.stereoEnabled)
			{
				this._viewProj = matrix4x * this.myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
				this._viewProjSP = matrix4x2 * this.myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
			}
			else
			{
				this._viewProj = matrix4x * this.myCam.worldToCameraMatrix;
				this._viewProjSP = matrix4x2 * this.myCam.worldToCameraMatrix;
			}
			if (this._preLightPass != null)
			{
				this._preLightPass.Clear();
			}
			if (this._afterLightPass != null)
			{
				this._afterLightPass.Clear();
			}
			bool flag = SystemInfo.graphicsShaderLevel > 40;
			if (EnviroSky.instance.volumeLightSettings.Resolution == EnviroSkyRendering.VolumtericResolution.Quarter)
			{
				Texture source = null;
				this._preLightPass.Blit(source, this._halfDepthBuffer, this._bilateralBlurMaterial, flag ? 4 : 10);
				this._preLightPass.Blit(source, this._quarterDepthBuffer, this._bilateralBlurMaterial, flag ? 6 : 11);
				this._preLightPass.SetRenderTarget(this._quarterVolumeLightTexture);
			}
			else if (EnviroSky.instance.volumeLightSettings.Resolution == EnviroSkyRendering.VolumtericResolution.Half)
			{
				Texture source2 = null;
				this._preLightPass.Blit(source2, this._halfDepthBuffer, this._bilateralBlurMaterial, flag ? 4 : 10);
				this._preLightPass.SetRenderTarget(this._halfVolumeLightTexture);
			}
			else
			{
				this._preLightPass.SetRenderTarget(this._volumeLightTexture);
			}
			this._preLightPass.ClearRenderTarget(false, true, new Color(0f, 0f, 0f, 1f));
			this.UpdateMaterialParameters();
			if (EnviroSkyRendering.PreRenderEvent != null)
			{
				EnviroSkyRendering.PreRenderEvent(this, this._viewProj, this._viewProjSP);
			}
		}
		if (this.myCam.stereoEnabled)
		{
			Matrix4x4 inverse = this.myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Left).inverse;
			Matrix4x4 inverse2 = this.myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right).inverse;
			Matrix4x4 stereoProjectionMatrix = this.myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
			Matrix4x4 stereoProjectionMatrix2 = this.myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
			Matrix4x4 inverse3 = GL.GetGPUProjectionMatrix(stereoProjectionMatrix, true).inverse;
			Matrix4x4 inverse4 = GL.GetGPUProjectionMatrix(stereoProjectionMatrix2, true).inverse;
			if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLCore && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES3)
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
		}
		else
		{
			Matrix4x4 cameraToWorldMatrix = this.myCam.cameraToWorldMatrix;
			Matrix4x4 inverse5 = GL.GetGPUProjectionMatrix(this.myCam.projectionMatrix, true).inverse;
			if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLCore && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES3)
			{
				ref Matrix4x4 ptr = ref inverse5;
				ptr[1, 1] = ptr[1, 1] * -1f;
			}
			Shader.SetGlobalMatrix("_LeftWorldFromView", cameraToWorldMatrix);
			Shader.SetGlobalMatrix("_LeftViewFromScreen", inverse5);
		}
		if (EnviroSky.instance == null)
		{
			return;
		}
		if (this.myCam != null)
		{
			switch (this.myCam.stereoActiveEye)
			{
			case Camera.MonoOrStereoscopicEye.Left:
				if (EnviroSky.instance.satCamera != null)
				{
					this.RenderCamera(EnviroSky.instance.satCamera, Camera.MonoOrStereoscopicEye.Left);
				}
				break;
			case Camera.MonoOrStereoscopicEye.Right:
				if (EnviroSky.instance.satCamera != null)
				{
					this.RenderCamera(EnviroSky.instance.satCamera, Camera.MonoOrStereoscopicEye.Right);
				}
				break;
			case Camera.MonoOrStereoscopicEye.Mono:
				if (EnviroSky.instance.satCamera != null)
				{
					this.RenderCamera(EnviroSky.instance.satCamera, Camera.MonoOrStereoscopicEye.Mono);
				}
				break;
			}
			if (EnviroSky.instance.satCamera != null)
			{
				RenderSettings.skybox.SetTexture("_SatTex", EnviroSky.instance.satCamera.targetTexture);
			}
		}
	}

	// Token: 0x060003E8 RID: 1000 RVA: 0x000269A4 File Offset: 0x00024BA4
	private void RenderCamera(Camera targetCam, Camera.MonoOrStereoscopicEye eye)
	{
		targetCam.fieldOfView = EnviroSky.instance.PlayerCamera.fieldOfView;
		targetCam.aspect = EnviroSky.instance.PlayerCamera.aspect;
		switch (eye)
		{
		case Camera.MonoOrStereoscopicEye.Left:
			targetCam.transform.position = EnviroSky.instance.PlayerCamera.transform.position;
			targetCam.transform.rotation = EnviroSky.instance.PlayerCamera.transform.rotation;
			targetCam.projectionMatrix = EnviroSky.instance.PlayerCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
			targetCam.worldToCameraMatrix = EnviroSky.instance.PlayerCamera.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
			targetCam.Render();
			return;
		case Camera.MonoOrStereoscopicEye.Right:
			targetCam.transform.position = EnviroSky.instance.PlayerCamera.transform.position;
			targetCam.transform.rotation = EnviroSky.instance.PlayerCamera.transform.rotation;
			targetCam.projectionMatrix = EnviroSky.instance.PlayerCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
			targetCam.worldToCameraMatrix = EnviroSky.instance.PlayerCamera.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
			targetCam.Render();
			return;
		case Camera.MonoOrStereoscopicEye.Mono:
			targetCam.transform.position = EnviroSky.instance.PlayerCamera.transform.position;
			targetCam.transform.rotation = EnviroSky.instance.PlayerCamera.transform.rotation;
			targetCam.worldToCameraMatrix = EnviroSky.instance.PlayerCamera.worldToCameraMatrix;
			targetCam.Render();
			return;
		default:
			return;
		}
	}

	// Token: 0x060003E9 RID: 1001 RVA: 0x00026B2C File Offset: 0x00024D2C
	private void UpdateQualitySettings()
	{
		if (this.useGlobalRenderingSettings)
		{
			this.useVolumeClouds = EnviroSky.instance.useVolumeClouds;
			this.useVolumeLighting = EnviroSky.instance.useVolumeLighting;
			this.useDistanceBlur = EnviroSky.instance.useDistanceBlur;
			this.useFog = EnviroSky.instance.useFog;
			this.usedCloudsQuality = EnviroSky.instance.cloudsSettings.cloudsQualitySettings;
			return;
		}
		this.useVolumeClouds = this.customRenderingSettings.useVolumeClouds;
		this.useVolumeLighting = this.customRenderingSettings.useVolumeLighting;
		this.useDistanceBlur = this.customRenderingSettings.useDistanceBlur;
		this.useFog = this.customRenderingSettings.useFog;
		if (this.customRenderingSettings.customCloudsQuality != null)
		{
			this.usedCloudsQuality = this.customRenderingSettings.customCloudsQuality.qualitySettings;
			return;
		}
		this.usedCloudsQuality = EnviroSky.instance.cloudsSettings.cloudsQualitySettings;
	}

	// Token: 0x060003EA RID: 1002 RVA: 0x00026C1C File Offset: 0x00024E1C
	private void Update()
	{
		if (EnviroSky.instance == null || this.myCam == null)
		{
			return;
		}
		this.UpdateQualitySettings();
		if (this.currentReprojectionPixelSize != this.usedCloudsQuality.reprojectionPixelSize)
		{
			this.currentReprojectionPixelSize = this.usedCloudsQuality.reprojectionPixelSize;
			this.SetReprojectionPixelSize(this.usedCloudsQuality.reprojectionPixelSize);
		}
		if (this.currentVolumeRes != EnviroSky.instance.volumeLightSettings.Resolution)
		{
			this.ChangeResolution();
			this.currentVolumeRes = EnviroSky.instance.volumeLightSettings.Resolution;
		}
		if (this._volumeLightTexture != null && (this._volumeLightTexture.width != this.myCam.pixelWidth || this._volumeLightTexture.height != this.myCam.pixelHeight))
		{
			this.ChangeResolution();
		}
		if ((!this.useFog && this.currentFogType != EnviroSkyRendering.FogType.Disabled) || (this.useFog && EnviroSky.instance.fogSettings.useSimpleFog && this.currentFogType != EnviroSkyRendering.FogType.Simple) || (this.useFog && !EnviroSky.instance.fogSettings.useSimpleFog && this.currentFogType != EnviroSkyRendering.FogType.Standard))
		{
			this.CreateFogMaterial();
		}
	}

	// Token: 0x060003EB RID: 1003 RVA: 0x00026D50 File Offset: 0x00024F50
	[ImageEffectOpaque]
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (EnviroSky.instance == null)
		{
			Graphics.Blit(source, destination);
			return;
		}
		if (this.fogMat == null)
		{
			this.CreateFogMaterial();
		}
		if (this.myCam.actualRenderingPath == RenderingPath.Forward)
		{
			this.myCam.depthTextureMode |= DepthTextureMode.Depth;
		}
		int depthBufferBits = source.depth;
		if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal)
		{
			depthBufferBits = 0;
		}
		RenderTextureDescriptor desc = new RenderTextureDescriptor(source.width, source.height, source.format, depthBufferBits);
		desc.msaaSamples = source.antiAliasing;
		if (this.myCam.stereoEnabled && EnviroSky.instance.singlePassVR)
		{
			desc.vrUsage = VRTextureUsage.TwoEyes;
		}
		RenderTexture temporary = RenderTexture.GetTemporary(desc);
		RenderTexture temporary2 = RenderTexture.GetTemporary(desc);
		if (!this.aboveClouds)
		{
			if (this.useVolumeClouds)
			{
				if ((!Application.isPlaying && EnviroSky.instance.showVolumeCloudsInEditor) || Application.isPlaying)
				{
					this.RenderVolumeClouds(source, temporary);
				}
				else
				{
					Graphics.Blit(source, temporary);
				}
			}
			else
			{
				Graphics.Blit(source, temporary);
			}
			if (this.useVolumeLighting)
			{
				if ((!Application.isPlaying && EnviroSky.instance.showVolumeLightingInEditor) || Application.isPlaying)
				{
					this.RenderVolumeFog(temporary, temporary2);
				}
				else
				{
					this.RenderFog(temporary, temporary2);
					if (!this.isAddionalCamera)
					{
						Shader.DisableKeyword("ENVIROVOLUMELIGHT");
						Shader.SetGlobalTexture("_EnviroVolumeLightingTex", this.blackTexture);
					}
					else
					{
						this.fogMat.DisableKeyword("ENVIROVOLUMELIGHT");
						this.fogMat.SetTexture("_EnviroVolumeLightingTex", this.blackTexture);
					}
				}
			}
			else
			{
				this.RenderFog(temporary, temporary2);
				if (!this.isAddionalCamera)
				{
					Shader.DisableKeyword("ENVIROVOLUMELIGHT");
					Shader.SetGlobalTexture("_EnviroVolumeLightingTex", this.blackTexture);
				}
				else
				{
					this.fogMat.DisableKeyword("ENVIROVOLUMELIGHT");
					this.fogMat.SetTexture("_EnviroVolumeLightingTex", this.blackTexture);
				}
			}
			if (this.useDistanceBlur)
			{
				if ((!Application.isPlaying && EnviroSky.instance.showDistanceBlurInEditor) || Application.isPlaying)
				{
					this.RenderDistanceBlur(temporary2, destination);
				}
				else
				{
					Graphics.Blit(temporary2, destination);
				}
			}
			else
			{
				Graphics.Blit(temporary2, destination);
			}
		}
		else
		{
			if (this.useVolumeLighting)
			{
				if ((!Application.isPlaying && EnviroSky.instance.showVolumeLightingInEditor) || Application.isPlaying)
				{
					this.RenderVolumeFog(source, temporary);
				}
				else
				{
					this.RenderFog(source, temporary);
					if (!this.isAddionalCamera)
					{
						Shader.DisableKeyword("ENVIROVOLUMELIGHT");
						Shader.SetGlobalTexture("_EnviroVolumeLightingTex", this.blackTexture);
					}
					else
					{
						this.fogMat.DisableKeyword("ENVIROVOLUMELIGHT");
						this.fogMat.SetTexture("_EnviroVolumeLightingTex", this.blackTexture);
					}
				}
			}
			else
			{
				this.RenderFog(source, temporary);
				if (!this.isAddionalCamera)
				{
					Shader.DisableKeyword("ENVIROVOLUMELIGHT");
					Shader.SetGlobalTexture("_EnviroVolumeLightingTex", this.blackTexture);
				}
				else
				{
					this.fogMat.DisableKeyword("ENVIROVOLUMELIGHT");
					this.fogMat.SetTexture("_EnviroVolumeLightingTex", this.blackTexture);
				}
			}
			if (this.useVolumeClouds)
			{
				if ((!Application.isPlaying && EnviroSky.instance.showVolumeCloudsInEditor) || Application.isPlaying)
				{
					this.RenderVolumeClouds(temporary, temporary2);
				}
				else
				{
					Graphics.Blit(temporary, temporary2);
				}
			}
			else
			{
				Graphics.Blit(temporary, temporary2);
			}
			if (this.useDistanceBlur)
			{
				if ((!Application.isPlaying && EnviroSky.instance.showDistanceBlurInEditor) || Application.isPlaying)
				{
					this.RenderDistanceBlur(temporary2, destination);
				}
				else
				{
					Graphics.Blit(temporary2, destination);
				}
			}
			else
			{
				Graphics.Blit(temporary2, destination);
			}
		}
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
	}

	// Token: 0x060003EC RID: 1004 RVA: 0x000270E0 File Offset: 0x000252E0
	private void RenderVolumeClouds(RenderTexture src, RenderTexture dst)
	{
		if (this.blitMat == null)
		{
			this.blitMat = new Material(Shader.Find("Enviro/Standard/Blit"));
		}
		this.StartFrame();
		if (this.subFrameTex == null || this.prevFrameTex == null || this.textureDimensionChanged)
		{
			this.CreateCloudsRenderTextures(src);
		}
		if (!this.isAddionalCamera)
		{
			EnviroSky.instance.cloudsRenderTarget = this.subFrameTex;
		}
		this.RenderClouds(src, this.subFrameTex);
		if (this.isFirstFrame)
		{
			Graphics.Blit(this.subFrameTex, this.prevFrameTex);
			this.isFirstFrame = false;
		}
		if (EnviroSky.instance.cloudsSettings.depthBlending)
		{
			Shader.EnableKeyword("ENVIRO_DEPTHBLENDING");
		}
		else
		{
			Shader.DisableKeyword("ENVIRO_DEPTHBLENDING");
		}
		int num = EnviroSky.instance.cloudsSettings.bilateralUpsampling ? (this.reprojectionPixelSize * this.usedCloudsQuality.cloudsRenderResolution) : 1;
		if (num > 1)
		{
			if (this.compose == null)
			{
				this.compose = new Material(Shader.Find("Hidden/Enviro/Upsample"));
			}
			if (this.downsample == null)
			{
				this.downsample = new Material(Shader.Find("Hidden/Enviro/DepthDownsample"));
			}
			RenderTexture renderTexture = this.DownsampleDepth(Screen.width, Screen.height, src, this.downsample, num);
			this.compose.SetTexture("_CameraDepthLowRes", renderTexture);
			RenderTexture temporary = RenderTexture.GetTemporary(this.myCam.pixelWidth / num * 2, this.myCam.pixelHeight / num * 2, 0, RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Default);
			temporary.filterMode = FilterMode.Bilinear;
			Vector2 v = new Vector2(1f / (float)renderTexture.width, 1f / (float)renderTexture.height);
			this.compose.SetVector("_LowResPixelSize", v);
			this.compose.SetVector("_LowResTextureSize", new Vector2((float)renderTexture.width, (float)renderTexture.height));
			this.compose.SetFloat("_DepthMult", 32f);
			this.compose.SetFloat("_Threshold", 0.0005f);
			this.compose.SetTexture("_LowResTexture", this.subFrameTex);
			Graphics.Blit(this.subFrameTex, temporary, this.compose);
			RenderTexture.ReleaseTemporary(renderTexture);
			this.blitMat.SetTexture("_MainTex", src);
			this.blitMat.SetTexture("_SubFrame", temporary);
			this.blitMat.SetTexture("_PrevFrame", this.prevFrameTex);
			this.SetBlitmaterialProperties();
			Graphics.Blit(src, dst, this.blitMat);
			Graphics.Blit(temporary, this.prevFrameTex);
			RenderTexture.ReleaseTemporary(temporary);
		}
		else
		{
			this.blitMat.SetTexture("_MainTex", src);
			this.blitMat.SetTexture("_SubFrame", this.subFrameTex);
			this.blitMat.SetTexture("_PrevFrame", this.prevFrameTex);
			this.SetBlitmaterialProperties();
			Graphics.Blit(src, dst, this.blitMat);
			Graphics.Blit(this.subFrameTex, this.prevFrameTex);
		}
		this.FinalizeFrame();
	}

	// Token: 0x060003ED RID: 1005 RVA: 0x000273F8 File Offset: 0x000255F8
	private void RenderFog(RenderTexture src, RenderTexture dst)
	{
		float num = this.myCam.transform.position.y - EnviroSky.instance.fogSettings.height;
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
		if (!EnviroSky.instance.fogSettings.useSimpleFog)
		{
			Shader.SetGlobalVector("_FogNoiseVelocity", new Vector4(-EnviroSky.instance.Components.windZone.transform.forward.x * EnviroSky.instance.windIntensity * 5f, -EnviroSky.instance.Components.windZone.transform.forward.z * EnviroSky.instance.windIntensity * 5f) * EnviroSky.instance.fogSettings.noiseScale);
			Shader.SetGlobalVector("_FogNoiseData", new Vector4(EnviroSky.instance.fogSettings.noiseScale, EnviroSky.instance.fogSettings.noiseIntensity, EnviroSky.instance.fogSettings.noiseIntensityOffset));
			Shader.SetGlobalTexture("_FogNoiseTexture", this.detailNoiseTexture);
		}
		Shader.SetGlobalFloat("_EnviroVolumeDensity", EnviroSky.instance.globalVolumeLightIntensity);
		Shader.SetGlobalVector("_SceneFogParams", value);
		Shader.SetGlobalVector("_SceneFogMode", new Vector4((float)fogMode, (float)(EnviroSky.instance.fogSettings.useRadialDistance ? 1 : 0), 0f, Application.isPlaying ? 1f : 0f));
		Shader.SetGlobalVector("_HeightParams", new Vector4(EnviroSky.instance.fogSettings.height, num, z, EnviroSky.instance.fogSettings.heightDensity * 0.5f));
		Shader.SetGlobalVector("_DistanceParams", new Vector4(-Mathf.Max(EnviroSky.instance.fogSettings.startDistance, 0f), 0f, 0f, 0f));
		if (this.dither != null && EnviroSkyMgr.instance.FogSettings.dithering)
		{
			this.fogMat.SetTexture("_DitheringTex", this.dither);
			this.fogMat.SetInt("_UseDithering", 1);
		}
		else
		{
			this.fogMat.SetInt("_UseDithering", 0);
		}
		this.fogMat.SetTexture("_MainTex", src);
		Graphics.Blit(src, dst, this.fogMat);
	}

	// Token: 0x060003EE RID: 1006 RVA: 0x00027700 File Offset: 0x00025900
	private void RenderVolumeFog(RenderTexture src, RenderTexture dst)
	{
		if (this.volumeLightMat == null)
		{
			this.volumeLightMat = new Material(Shader.Find("Enviro/Standard/VolumeLight"));
		}
		if (this._volumeLightTexture == null || (this._halfVolumeLightTexture == null && EnviroSky.instance.volumeLightSettings.Resolution == EnviroSkyRendering.VolumtericResolution.Half) || (this._quarterVolumeLightTexture == null && EnviroSky.instance.volumeLightSettings.Resolution == EnviroSkyRendering.VolumtericResolution.Quarter))
		{
			this.ChangeResolution();
		}
		if (EnviroSky.instance.volumeLightSettings.dirVolumeLighting)
		{
			Light component = EnviroSky.instance.Components.DirectLight.GetComponent<Light>();
			int pass = 4;
			this.volumeLightMat.SetPass(pass);
			if (EnviroSky.instance.volumeLightSettings.directLightNoise)
			{
				this.volumeLightMat.EnableKeyword("NOISE");
			}
			else
			{
				this.volumeLightMat.DisableKeyword("NOISE");
			}
			this.volumeLightMat.SetVector("_LightDir", new Vector4(component.transform.forward.x, component.transform.forward.y, component.transform.forward.z, 1f / (component.range * component.range)));
			this.volumeLightMat.SetVector("_LightColor", component.color * component.intensity);
			this.volumeLightMat.SetFloat("_MaxRayLength", EnviroSky.instance.volumeLightSettings.MaxRayLength);
			if (component.cookie == null)
			{
				this.volumeLightMat.EnableKeyword("DIRECTIONAL");
				this.volumeLightMat.DisableKeyword("DIRECTIONAL_COOKIE");
			}
			else
			{
				this.volumeLightMat.EnableKeyword("DIRECTIONAL_COOKIE");
				this.volumeLightMat.DisableKeyword("DIRECTIONAL");
				this.volumeLightMat.SetTexture("_LightTexture0", component.cookie);
			}
			this.volumeLightMat.SetInt("_SampleCount", EnviroSky.instance.volumeLightSettings.SampleCount);
			this.volumeLightMat.SetVector("_NoiseVelocity", new Vector4(-EnviroSky.instance.Components.windZone.transform.forward.x * EnviroSky.instance.windIntensity * 10f, -EnviroSky.instance.Components.windZone.transform.forward.z * EnviroSky.instance.windIntensity * 10f) * EnviroSky.instance.volumeLightSettings.noiseScale);
			this.volumeLightMat.SetVector("_NoiseData", new Vector4(EnviroSky.instance.volumeLightSettings.noiseScale, EnviroSky.instance.volumeLightSettings.noiseIntensity, EnviroSky.instance.volumeLightSettings.noiseIntensityOffset));
			this.volumeLightMat.SetVector("_MieG", new Vector4(1f - EnviroSky.instance.volumeLightSettings.Anistropy * EnviroSky.instance.volumeLightSettings.Anistropy, 1f + EnviroSky.instance.volumeLightSettings.Anistropy * EnviroSky.instance.volumeLightSettings.Anistropy, 2f * EnviroSky.instance.volumeLightSettings.Anistropy, 0.07957747f));
			this.volumeLightMat.SetVector("_VolumetricLight", new Vector4(EnviroSky.instance.volumeLightSettings.ScatteringCoef.Evaluate(EnviroSky.instance.GameTime.solarTime), EnviroSky.instance.volumeLightSettings.ExtinctionCoef, component.range, 1f));
			this.volumeLightMat.SetTexture("_CameraDepthTexture", this.GetVolumeLightDepthBuffer());
			if (component.shadows != LightShadows.None)
			{
				this.volumeLightMat.EnableKeyword("SHADOWS_DEPTH");
				Graphics.Blit(null, this.GetVolumeLightBuffer(), this.volumeLightMat, pass);
			}
			else
			{
				this.volumeLightMat.DisableKeyword("SHADOWS_DEPTH");
				Graphics.Blit(null, this.GetVolumeLightBuffer(), this.volumeLightMat, pass);
			}
		}
		if (EnviroSky.instance.volumeLightSettings.Resolution == EnviroSkyRendering.VolumtericResolution.Quarter)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(this._quarterDepthBuffer.width, this._quarterDepthBuffer.height, 0, RenderTextureFormat.ARGBHalf);
			temporary.filterMode = FilterMode.Bilinear;
			if (this.myCam.stereoEnabled && EnviroSky.instance.singlePassVR)
			{
				temporary.vrUsage = VRTextureUsage.TwoEyes;
			}
			Graphics.Blit(this._quarterVolumeLightTexture, temporary, this._bilateralBlurMaterial, 8);
			Graphics.Blit(temporary, this._quarterVolumeLightTexture, this._bilateralBlurMaterial, 9);
			Graphics.Blit(this._quarterVolumeLightTexture, this._volumeLightTexture, this._bilateralBlurMaterial, 7);
			RenderTexture.ReleaseTemporary(temporary);
		}
		else if (EnviroSky.instance.volumeLightSettings.Resolution == EnviroSkyRendering.VolumtericResolution.Half)
		{
			RenderTexture temporary2 = RenderTexture.GetTemporary(this._halfVolumeLightTexture.width, this._halfVolumeLightTexture.height, 0, RenderTextureFormat.ARGBHalf);
			temporary2.filterMode = FilterMode.Bilinear;
			if (this.myCam.stereoEnabled && EnviroSky.instance.singlePassVR)
			{
				temporary2.vrUsage = VRTextureUsage.TwoEyes;
			}
			Graphics.Blit(this._halfVolumeLightTexture, temporary2, this._bilateralBlurMaterial, 2);
			Graphics.Blit(temporary2, this._halfVolumeLightTexture, this._bilateralBlurMaterial, 3);
			Graphics.Blit(this._halfVolumeLightTexture, this._volumeLightTexture, this._bilateralBlurMaterial, 5);
			RenderTexture.ReleaseTemporary(temporary2);
		}
		else
		{
			RenderTexture temporary3 = RenderTexture.GetTemporary(this._volumeLightTexture.width, this._volumeLightTexture.height, 0, RenderTextureFormat.ARGBHalf);
			temporary3.filterMode = FilterMode.Bilinear;
			if (this.myCam.stereoEnabled && EnviroSky.instance.singlePassVR)
			{
				temporary3.vrUsage = VRTextureUsage.TwoEyes;
			}
			Graphics.Blit(this._volumeLightTexture, temporary3, this._bilateralBlurMaterial, 0);
			Graphics.Blit(temporary3, this._volumeLightTexture, this._bilateralBlurMaterial, 1);
			RenderTexture.ReleaseTemporary(temporary3);
		}
		Shader.EnableKeyword("ENVIROVOLUMELIGHT");
		Shader.SetGlobalTexture("_EnviroVolumeLightingTex", this._volumeLightTexture);
		this.RenderFog(src, dst);
	}

	// Token: 0x060003EF RID: 1007 RVA: 0x00027CEC File Offset: 0x00025EEC
	private void RenderDistanceBlur(RenderTexture source, RenderTexture destination)
	{
		bool allowHDR = this.myCam.allowHDR;
		int num = source.width;
		int num2 = source.height;
		if (!EnviroSky.instance.distanceBlurSettings.highQuality)
		{
			num /= 2;
			num2 /= 2;
		}
		if (this.postProcessMat == null)
		{
			this.postProcessMat = new Material(Shader.Find("Hidden/EnviroDistanceBlur"));
		}
		this.postProcessMat.SetTexture("_DistTex", this.distributionTexture);
		this.postProcessMat.SetFloat("_Distance", EnviroSky.instance.blurDistance);
		this.postProcessMat.SetFloat("_Radius", EnviroSky.instance.distanceBlurSettings.radius);
		RenderTextureFormat format = allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
		float num3 = Mathf.Log((float)num2, 2f) + EnviroSky.instance.distanceBlurSettings.radius - 8f;
		int num4 = (int)num3;
		int num5 = Mathf.Clamp(num4, 1, 16);
		float thresholdLinear = this.thresholdLinear;
		this.postProcessMat.SetFloat("_Threshold", thresholdLinear);
		float num6 = thresholdLinear * 0.5f + 1E-05f;
		Vector3 v = new Vector3(thresholdLinear - num6, num6 * 2f, 0.25f / num6);
		this.postProcessMat.SetVector("_Curve", v);
		bool flag = !EnviroSky.instance.distanceBlurSettings.highQuality && EnviroSky.instance.distanceBlurSettings.antiFlicker;
		this.postProcessMat.SetFloat("_PrefilterOffs", flag ? -0.5f : 0f);
		this.postProcessMat.SetFloat("_SampleScale", 0.5f + num3 - (float)num4);
		this.postProcessMat.SetFloat("_Intensity", EnviroSky.instance.blurIntensity);
		this.postProcessMat.SetFloat("_SkyBlurring", EnviroSky.instance.blurSkyIntensity);
		RenderTexture temporary = RenderTexture.GetTemporary(num, num2, 0, format);
		int pass = EnviroSky.instance.distanceBlurSettings.antiFlicker ? 1 : 0;
		Graphics.Blit(source, temporary, this.postProcessMat, pass);
		RenderTexture renderTexture = temporary;
		for (int i = 0; i < num5; i++)
		{
			this._blurBuffer1[i] = RenderTexture.GetTemporary(renderTexture.width / 2, renderTexture.height / 2, 0, format);
			pass = ((i == 0) ? (EnviroSky.instance.distanceBlurSettings.antiFlicker ? 3 : 2) : 4);
			Graphics.Blit(renderTexture, this._blurBuffer1[i], this.postProcessMat, pass);
			renderTexture = this._blurBuffer1[i];
		}
		for (int j = num5 - 2; j >= 0; j--)
		{
			RenderTexture renderTexture2 = this._blurBuffer1[j];
			this.postProcessMat.SetTexture("_BaseTex", renderTexture2);
			this._blurBuffer2[j] = RenderTexture.GetTemporary(renderTexture2.width, renderTexture2.height, 0, format);
			pass = (EnviroSky.instance.distanceBlurSettings.highQuality ? 6 : 5);
			Graphics.Blit(renderTexture, this._blurBuffer2[j], this.postProcessMat, pass);
			renderTexture = this._blurBuffer2[j];
		}
		this.postProcessMat.SetTexture("_BaseTex", source);
		pass = (EnviroSky.instance.distanceBlurSettings.highQuality ? 8 : 7);
		Graphics.Blit(renderTexture, destination, this.postProcessMat, pass);
		for (int k = 0; k < 16; k++)
		{
			if (this._blurBuffer1[k] != null)
			{
				RenderTexture.ReleaseTemporary(this._blurBuffer1[k]);
			}
			if (this._blurBuffer2[k] != null)
			{
				RenderTexture.ReleaseTemporary(this._blurBuffer2[k]);
			}
			this._blurBuffer1[k] = null;
			this._blurBuffer2[k] = null;
		}
		RenderTexture.ReleaseTemporary(temporary);
	}

	// Token: 0x060003F0 RID: 1008 RVA: 0x000280A0 File Offset: 0x000262A0
	private void UpdateMaterialParameters()
	{
		if (this._bilateralBlurMaterial == null)
		{
			this._bilateralBlurMaterial = new Material(Shader.Find("Hidden/EnviroBilateralBlur"));
		}
		this._bilateralBlurMaterial.SetTexture("_HalfResDepthBuffer", this._halfDepthBuffer);
		this._bilateralBlurMaterial.SetTexture("_HalfResColor", this._halfVolumeLightTexture);
		this._bilateralBlurMaterial.SetTexture("_QuarterResDepthBuffer", this._quarterDepthBuffer);
		this._bilateralBlurMaterial.SetTexture("_QuarterResColor", this._quarterVolumeLightTexture);
		Shader.SetGlobalTexture("_DitherTexture", this._ditheringTexture);
		Shader.SetGlobalTexture("_NoiseTexture", this.detailNoiseTexture);
	}

	// Token: 0x060003F1 RID: 1009 RVA: 0x00028148 File Offset: 0x00026348
	private void GenerateDitherTexture()
	{
		if (this._ditheringTexture != null)
		{
			return;
		}
		int num = 8;
		this._ditheringTexture = new Texture2D(num, num, TextureFormat.Alpha8, false, true);
		this._ditheringTexture.filterMode = FilterMode.Point;
		Color32[] array = new Color32[num * num];
		int num2 = 0;
		byte b = 3;
		array[num2++] = new Color32(b, b, b, b);
		b = 192;
		array[num2++] = new Color32(b, b, b, b);
		b = 51;
		array[num2++] = new Color32(b, b, b, b);
		b = 239;
		array[num2++] = new Color32(b, b, b, b);
		b = 15;
		array[num2++] = new Color32(b, b, b, b);
		b = 204;
		array[num2++] = new Color32(b, b, b, b);
		b = 62;
		array[num2++] = new Color32(b, b, b, b);
		b = 251;
		array[num2++] = new Color32(b, b, b, b);
		b = 129;
		array[num2++] = new Color32(b, b, b, b);
		b = 66;
		array[num2++] = new Color32(b, b, b, b);
		b = 176;
		array[num2++] = new Color32(b, b, b, b);
		b = 113;
		array[num2++] = new Color32(b, b, b, b);
		b = 141;
		array[num2++] = new Color32(b, b, b, b);
		b = 78;
		array[num2++] = new Color32(b, b, b, b);
		b = 188;
		array[num2++] = new Color32(b, b, b, b);
		b = 125;
		array[num2++] = new Color32(b, b, b, b);
		b = 35;
		array[num2++] = new Color32(b, b, b, b);
		b = 223;
		array[num2++] = new Color32(b, b, b, b);
		b = 19;
		array[num2++] = new Color32(b, b, b, b);
		b = 207;
		array[num2++] = new Color32(b, b, b, b);
		b = 47;
		array[num2++] = new Color32(b, b, b, b);
		b = 235;
		array[num2++] = new Color32(b, b, b, b);
		b = 31;
		array[num2++] = new Color32(b, b, b, b);
		b = 219;
		array[num2++] = new Color32(b, b, b, b);
		b = 160;
		array[num2++] = new Color32(b, b, b, b);
		b = 98;
		array[num2++] = new Color32(b, b, b, b);
		b = 145;
		array[num2++] = new Color32(b, b, b, b);
		b = 82;
		array[num2++] = new Color32(b, b, b, b);
		b = 172;
		array[num2++] = new Color32(b, b, b, b);
		b = 109;
		array[num2++] = new Color32(b, b, b, b);
		b = 156;
		array[num2++] = new Color32(b, b, b, b);
		b = 94;
		array[num2++] = new Color32(b, b, b, b);
		b = 11;
		array[num2++] = new Color32(b, b, b, b);
		b = 200;
		array[num2++] = new Color32(b, b, b, b);
		b = 58;
		array[num2++] = new Color32(b, b, b, b);
		b = 247;
		array[num2++] = new Color32(b, b, b, b);
		b = 7;
		array[num2++] = new Color32(b, b, b, b);
		b = 196;
		array[num2++] = new Color32(b, b, b, b);
		b = 54;
		array[num2++] = new Color32(b, b, b, b);
		b = 243;
		array[num2++] = new Color32(b, b, b, b);
		b = 137;
		array[num2++] = new Color32(b, b, b, b);
		b = 74;
		array[num2++] = new Color32(b, b, b, b);
		b = 184;
		array[num2++] = new Color32(b, b, b, b);
		b = 121;
		array[num2++] = new Color32(b, b, b, b);
		b = 133;
		array[num2++] = new Color32(b, b, b, b);
		b = 70;
		array[num2++] = new Color32(b, b, b, b);
		b = 180;
		array[num2++] = new Color32(b, b, b, b);
		b = 117;
		array[num2++] = new Color32(b, b, b, b);
		b = 43;
		array[num2++] = new Color32(b, b, b, b);
		b = 231;
		array[num2++] = new Color32(b, b, b, b);
		b = 27;
		array[num2++] = new Color32(b, b, b, b);
		b = 215;
		array[num2++] = new Color32(b, b, b, b);
		b = 39;
		array[num2++] = new Color32(b, b, b, b);
		b = 227;
		array[num2++] = new Color32(b, b, b, b);
		b = 23;
		array[num2++] = new Color32(b, b, b, b);
		b = 211;
		array[num2++] = new Color32(b, b, b, b);
		b = 168;
		array[num2++] = new Color32(b, b, b, b);
		b = 105;
		array[num2++] = new Color32(b, b, b, b);
		b = 153;
		array[num2++] = new Color32(b, b, b, b);
		b = 90;
		array[num2++] = new Color32(b, b, b, b);
		b = 164;
		array[num2++] = new Color32(b, b, b, b);
		b = 102;
		array[num2++] = new Color32(b, b, b, b);
		b = 149;
		array[num2++] = new Color32(b, b, b, b);
		b = 86;
		array[num2++] = new Color32(b, b, b, b);
		this._ditheringTexture.SetPixels32(array);
		this._ditheringTexture.Apply();
	}

	// Token: 0x060003F2 RID: 1010 RVA: 0x000287C4 File Offset: 0x000269C4
	private Mesh CreateSpotLightMesh()
	{
		Mesh mesh = new Mesh();
		Vector3[] array = new Vector3[50];
		Color32[] array2 = new Color32[50];
		array[0] = new Vector3(0f, 0f, 0f);
		array[1] = new Vector3(0f, 0f, 1f);
		float num = 0f;
		float num2 = 0.3926991f;
		float num3 = 0.9f;
		for (int i = 0; i < 16; i++)
		{
			array[i + 2] = new Vector3(-Mathf.Cos(num) * num3, Mathf.Sin(num) * num3, num3);
			array2[i + 2] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			array[i + 2 + 16] = new Vector3(-Mathf.Cos(num), Mathf.Sin(num), 1f);
			array2[i + 2 + 16] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
			array[i + 2 + 32] = new Vector3(-Mathf.Cos(num) * num3, Mathf.Sin(num) * num3, 1f);
			array2[i + 2 + 32] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			num += num2;
		}
		mesh.vertices = array;
		mesh.colors32 = array2;
		int[] array3 = new int[288];
		int num4 = 0;
		for (int j = 2; j < 17; j++)
		{
			array3[num4++] = 0;
			array3[num4++] = j;
			array3[num4++] = j + 1;
		}
		array3[num4++] = 0;
		array3[num4++] = 17;
		array3[num4++] = 2;
		for (int k = 2; k < 17; k++)
		{
			array3[num4++] = k;
			array3[num4++] = k + 16;
			array3[num4++] = k + 1;
			array3[num4++] = k + 1;
			array3[num4++] = k + 16;
			array3[num4++] = k + 16 + 1;
		}
		array3[num4++] = 2;
		array3[num4++] = 17;
		array3[num4++] = 18;
		array3[num4++] = 18;
		array3[num4++] = 17;
		array3[num4++] = 33;
		for (int l = 18; l < 33; l++)
		{
			array3[num4++] = l;
			array3[num4++] = l + 16;
			array3[num4++] = l + 1;
			array3[num4++] = l + 1;
			array3[num4++] = l + 16;
			array3[num4++] = l + 16 + 1;
		}
		array3[num4++] = 18;
		array3[num4++] = 33;
		array3[num4++] = 34;
		array3[num4++] = 34;
		array3[num4++] = 33;
		array3[num4++] = 49;
		for (int m = 34; m < 49; m++)
		{
			array3[num4++] = 1;
			array3[num4++] = m + 1;
			array3[num4++] = m;
		}
		array3[num4++] = 1;
		array3[num4++] = 34;
		array3[num4++] = 49;
		mesh.triangles = array3;
		mesh.RecalculateBounds();
		return mesh;
	}

	// Token: 0x060003F3 RID: 1011 RVA: 0x00028B68 File Offset: 0x00026D68
	private void SetCloudProperties()
	{
		this.cloudsMat.SetTexture("_Noise", this.noiseTextureHigh);
		this.cloudsMat.SetTexture("_NoiseLow", this.noiseTexture);
		if (this.usedCloudsQuality.detailQuality == EnviroVolumeCloudsQualitySettings.CloudDetailQuality.Low)
		{
			this.cloudsMat.SetTexture("_DetailNoise", this.detailNoiseTexture);
		}
		else
		{
			this.cloudsMat.SetTexture("_DetailNoise", this.detailNoiseTextureHigh);
		}
		switch (this.myCam.stereoActiveEye)
		{
		case Camera.MonoOrStereoscopicEye.Left:
		{
			this.projection = this.myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
			Matrix4x4 inverse = this.projection.inverse;
			this.cloudsMat.SetMatrix("_InverseProjection", inverse);
			this.inverseRotation = this.myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Left).inverse;
			this.cloudsMat.SetMatrix("_InverseRotation", this.inverseRotation);
			if (this.myCam.stereoEnabled && EnviroSky.instance.singlePassVR)
			{
				Matrix4x4 inverse2 = this.myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right).inverse;
				this.cloudsMat.SetMatrix("_InverseProjection_SP", inverse2);
				this.inverseRotationSPVR = this.myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right).inverse;
				this.cloudsMat.SetMatrix("_InverseRotation_SP", this.inverseRotationSPVR);
			}
			break;
		}
		case Camera.MonoOrStereoscopicEye.Right:
		{
			this.projection = this.myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
			Matrix4x4 inverse3 = this.projection.inverse;
			this.cloudsMat.SetMatrix("_InverseProjection", inverse3);
			this.inverseRotation = this.myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right).inverse;
			this.cloudsMat.SetMatrix("_InverseRotation", this.inverseRotation);
			break;
		}
		case Camera.MonoOrStereoscopicEye.Mono:
		{
			this.projection = this.myCam.projectionMatrix;
			Matrix4x4 inverse4 = this.projection.inverse;
			this.cloudsMat.SetMatrix("_InverseProjection", inverse4);
			this.inverseRotation = this.myCam.cameraToWorldMatrix;
			this.cloudsMat.SetMatrix("_InverseRotation", this.inverseRotation);
			break;
		}
		}
		if (EnviroSky.instance.cloudsSettings.customWeatherMap == null)
		{
			this.cloudsMat.SetTexture("_WeatherMap", EnviroSky.instance.weatherMap);
		}
		else
		{
			this.cloudsMat.SetTexture("_WeatherMap", EnviroSky.instance.cloudsSettings.customWeatherMap);
		}
		if (EnviroSky.instance.cloudsSettings.cloudsQualitySettings.useCurlNoise)
		{
			this.cloudsMat.EnableKeyword("ENVIRO_CURLNOISE");
			this.cloudsMat.SetTexture("_CurlNoise", this.curlMap);
		}
		else
		{
			this.cloudsMat.DisableKeyword("ENVIRO_CURLNOISE");
		}
		if (EnviroSky.instance.cloudsSettings.useHaltonRaymarchOffset)
		{
			this.cloudsMat.EnableKeyword("ENVIRO_HALTONOFFSET");
			this.cloudsMat.SetFloat("_RaymarchOffset", this.sequence.Get());
			this.cloudsMat.SetVector("_TexelSize", this.subFrameTex.texelSize);
		}
		else
		{
			this.cloudsMat.DisableKeyword("ENVIRO_HALTONOFFSET");
		}
		if (!EnviroSky.instance.cloudsSettings.useLessSteps)
		{
			this.cloudsMat.SetVector("_Steps", new Vector4((float)this.usedCloudsQuality.raymarchSteps * EnviroSky.instance.cloudsConfig.raymarchingScale, (float)this.usedCloudsQuality.raymarchSteps * EnviroSky.instance.cloudsConfig.raymarchingScale, 0f, 0f));
		}
		else
		{
			this.cloudsMat.SetVector("_Steps", new Vector4((float)this.usedCloudsQuality.raymarchSteps * EnviroSky.instance.cloudsConfig.raymarchingScale * 0.75f, (float)this.usedCloudsQuality.raymarchSteps * EnviroSky.instance.cloudsConfig.raymarchingScale * 0.75f, 0f, 0f));
		}
		this.cloudsMat.SetFloat("_BaseNoiseUV", this.usedCloudsQuality.baseNoiseUV);
		this.cloudsMat.SetFloat("_DetailNoiseUV", this.usedCloudsQuality.detailNoiseUV);
		this.cloudsMat.SetFloat("_AmbientSkyColorIntensity", EnviroSky.instance.cloudsSettings.ambientLightIntensity.Evaluate(EnviroSky.instance.GameTime.solarTime));
		this.cloudsMat.SetVector("_CloudsLighting", new Vector4(EnviroSky.instance.cloudsConfig.scatteringCoef, EnviroSky.instance.cloudsSettings.hgPhase, EnviroSky.instance.cloudsSettings.silverLiningIntensity, EnviroSky.instance.cloudsSettings.silverLiningSpread.Evaluate(EnviroSky.instance.GameTime.solarTime)));
		float z = EnviroSky.instance.cloudsSettings.tonemapping ? 0f : 1f;
		this.cloudsMat.SetVector("_CloudsLightingExtended", new Vector4(EnviroSky.instance.cloudsConfig.edgeDarkness, EnviroSky.instance.cloudsConfig.ambientSkyColorIntensity, z, EnviroSky.instance.cloudsSettings.cloudsExposure));
		this.cloudsMat.SetColor("_AmbientLightColor", EnviroSky.instance.cloudsSettings.volumeCloudsAmbientColor.Evaluate(EnviroSky.instance.GameTime.solarTime));
		float num = this.usedCloudsQuality.bottomCloudHeight + EnviroSky.instance.cloudsSettings.cloudsHeightMod;
		float num2 = this.usedCloudsQuality.topCloudHeight + EnviroSky.instance.cloudsSettings.cloudsHeightMod;
		this.cloudsMat.SetVector("_CloudsParameter", new Vector4(num, num2, num2 - num, EnviroSky.instance.cloudsSettings.cloudsWorldScale * 10f));
		if (this.myCam.transform.position.y > num2)
		{
			this.aboveClouds = true;
		}
		else
		{
			this.aboveClouds = false;
		}
		if (EnviroSky.instance.cloudsSettings.useLessSteps)
		{
			this.cloudsMat.SetVector("_CloudDensityScale", new Vector4(EnviroSky.instance.cloudsConfig.density * 1.5f, EnviroSky.instance.cloudsConfig.lightStepModifier, EnviroSky.instance.GameTime.dayNightSwitch, EnviroSky.instance.GameTime.solarTime));
		}
		else
		{
			this.cloudsMat.SetVector("_CloudDensityScale", new Vector4(EnviroSky.instance.cloudsConfig.density, EnviroSky.instance.cloudsConfig.lightStepModifier, EnviroSky.instance.GameTime.dayNightSwitch, EnviroSky.instance.GameTime.solarTime));
		}
		this.cloudsMat.SetFloat("_CloudsType", EnviroSky.instance.cloudsConfig.cloudType);
		this.cloudsMat.SetVector("_CloudsCoverageSettings", new Vector4(EnviroSky.instance.cloudsConfig.coverage * EnviroSky.instance.cloudsSettings.globalCloudCoverage, EnviroSky.instance.cloudsConfig.lightAbsorbtion, EnviroSky.instance.cloudsSettings.cloudsQualitySettings.transmissionToExit, 0f));
		this.cloudsMat.SetVector("_CloudsAnimation", new Vector4(EnviroSky.instance.cloudAnim.x, EnviroSky.instance.cloudAnim.y, EnviroSky.instance.cloudsSettings.cloudsWindDirectionX, EnviroSky.instance.cloudsSettings.cloudsWindDirectionY));
		this.cloudsMat.SetColor("_LightColor", EnviroSky.instance.cloudsSettings.volumeCloudsColor.Evaluate(EnviroSky.instance.GameTime.solarTime));
		this.cloudsMat.SetColor("_MoonLightColor", EnviroSky.instance.cloudsSettings.volumeCloudsMoonColor.Evaluate(EnviroSky.instance.GameTime.lunarTime));
		this.cloudsMat.SetFloat("_stepsInDepth", this.usedCloudsQuality.stepsInDepthModificator);
		this.cloudsMat.SetFloat("_LODDistance", this.usedCloudsQuality.lodDistance);
		if (EnviroSky.instance.lightSettings.directionalLightMode == EnviroLightSettings.LightingMode.Dual)
		{
			if (EnviroSky.instance.GameTime.dayNightSwitch < EnviroSky.instance.GameTime.solarTime)
			{
				this.cloudsMat.SetVector("_LightDir", -EnviroSky.instance.Components.DirectLight.transform.forward);
			}
			else if (EnviroSky.instance.Components.AdditionalDirectLight != null)
			{
				this.cloudsMat.SetVector("_LightDir", -EnviroSky.instance.Components.AdditionalDirectLight.transform.forward);
			}
		}
		else
		{
			this.cloudsMat.SetVector("_LightDir", -EnviroSky.instance.Components.DirectLight.transform.forward);
		}
		this.cloudsMat.SetFloat("_LightIntensity", EnviroSky.instance.cloudsSettings.lightIntensity.Evaluate(EnviroSky.instance.GameTime.solarTime));
		this.cloudsMat.SetVector("_CloudsErosionIntensity", new Vector4(1f - EnviroSky.instance.cloudsConfig.baseErosionIntensity, EnviroSky.instance.cloudsConfig.detailErosionIntensity, EnviroSky.instance.cloudsSettings.attenuationClamp.Evaluate(EnviroSky.instance.GameTime.solarTime), EnviroSky.instance.cloudAnim.z));
	}

	// Token: 0x060003F4 RID: 1012 RVA: 0x00029510 File Offset: 0x00027710
	private void SetBlitmaterialProperties()
	{
		Matrix4x4 inverse = this.projection.inverse;
		this.blitMat.SetMatrix("_PreviousRotation", this.previousRotation);
		this.blitMat.SetMatrix("_Projection", this.projection);
		this.blitMat.SetMatrix("_InverseRotation", this.inverseRotation);
		this.blitMat.SetMatrix("_InverseProjection", inverse);
		if (this.myCam.stereoEnabled && EnviroSky.instance.singlePassVR)
		{
			Matrix4x4 inverse2 = this.projectionSPVR.inverse;
			this.blitMat.SetMatrix("_PreviousRotationSPVR", this.previousRotationSPVR);
			this.blitMat.SetMatrix("_ProjectionSPVR", this.projectionSPVR);
			this.blitMat.SetMatrix("_InverseRotationSPVR", this.inverseRotationSPVR);
			this.blitMat.SetMatrix("_InverseProjectionSPVR", inverse2);
		}
		this.blitMat.SetFloat("_FrameNumber", (float)this.subFrameNumber);
		this.blitMat.SetFloat("_ReprojectionPixelSize", (float)this.reprojectionPixelSize);
		this.blitMat.SetVector("_SubFrameDimension", new Vector2((float)this.subFrameWidth, (float)this.subFrameHeight));
		this.blitMat.SetVector("_FrameDimension", new Vector2((float)this.frameWidth, (float)this.frameHeight));
	}

	// Token: 0x060003F5 RID: 1013 RVA: 0x00029674 File Offset: 0x00027874
	private RenderTexture DownsampleDepth(int X, int Y, Texture src, Material mat, int downsampleFactor)
	{
		Vector2 v = new Vector2(1f / (float)X, 1f / (float)X);
		X /= downsampleFactor;
		Y /= downsampleFactor;
		RenderTexture temporary = RenderTexture.GetTemporary(X, Y, 0);
		mat.SetVector("_PixelSize", v);
		Graphics.Blit(src, temporary, mat);
		return temporary;
	}

	// Token: 0x060003F6 RID: 1014 RVA: 0x000296CC File Offset: 0x000278CC
	private void RenderClouds(RenderTexture source, RenderTexture tex)
	{
		if (this.cloudsMat == null)
		{
			this.cloudsMat = new Material(Shader.Find("Enviro/Standard/RaymarchClouds"));
		}
		this.cloudsMat.SetTexture("_MainTex", source);
		this.SetCloudProperties();
		Graphics.Blit(source, tex, this.cloudsMat);
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x00029720 File Offset: 0x00027920
	private void CreateCloudsRenderTextures(RenderTexture source)
	{
		if (this.subFrameTex != null)
		{
			Object.DestroyImmediate(this.subFrameTex);
			this.subFrameTex = null;
		}
		if (this.prevFrameTex != null)
		{
			Object.DestroyImmediate(this.prevFrameTex);
			this.prevFrameTex = null;
		}
		RenderTextureFormat colorFormat = this.myCam.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
		if (this.subFrameTex == null)
		{
			RenderTextureDescriptor desc = new RenderTextureDescriptor(this.subFrameWidth, this.subFrameHeight, colorFormat, 0);
			if (this.myCam.stereoEnabled && EnviroSky.instance.singlePassVR)
			{
				desc.vrUsage = VRTextureUsage.TwoEyes;
			}
			this.subFrameTex = new RenderTexture(desc);
			this.subFrameTex.filterMode = FilterMode.Bilinear;
			this.subFrameTex.hideFlags = HideFlags.HideAndDontSave;
			this.isFirstFrame = true;
		}
		if (this.prevFrameTex == null)
		{
			RenderTextureDescriptor desc2 = new RenderTextureDescriptor(this.frameWidth, this.frameHeight, colorFormat, 0);
			if (this.myCam.stereoEnabled && EnviroSky.instance.singlePassVR)
			{
				desc2.vrUsage = VRTextureUsage.TwoEyes;
			}
			this.prevFrameTex = new RenderTexture(desc2);
			this.prevFrameTex.filterMode = FilterMode.Bilinear;
			this.prevFrameTex.hideFlags = HideFlags.HideAndDontSave;
			this.isFirstFrame = true;
		}
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x00029860 File Offset: 0x00027A60
	private void SetReprojectionPixelSize(EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize pSize)
	{
		switch (pSize)
		{
		case EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize.Off:
			this.reprojectionPixelSize = 1;
			break;
		case EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize.Low:
			this.reprojectionPixelSize = 2;
			break;
		case EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize.Medium:
			this.reprojectionPixelSize = 4;
			break;
		case EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize.High:
			this.reprojectionPixelSize = 8;
			break;
		}
		this.frameList = this.CalculateFrames(this.reprojectionPixelSize);
	}

	// Token: 0x060003F9 RID: 1017 RVA: 0x000298BC File Offset: 0x00027ABC
	private void StartFrame()
	{
		this.textureDimensionChanged = this.UpdateFrameDimensions();
		switch (this.myCam.stereoActiveEye)
		{
		case Camera.MonoOrStereoscopicEye.Left:
			this.projection = this.myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
			this.rotation = this.myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
			this.inverseRotation = this.rotation.inverse;
			if (EnviroSky.instance.singlePassVR)
			{
				this.projectionSPVR = this.myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
				this.rotationSPVR = this.myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
				this.inverseRotationSPVR = this.rotationSPVR.inverse;
				return;
			}
			break;
		case Camera.MonoOrStereoscopicEye.Right:
			this.projection = this.myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
			this.rotation = this.myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
			this.inverseRotation = this.rotation.inverse;
			break;
		case Camera.MonoOrStereoscopicEye.Mono:
			this.projection = this.myCam.projectionMatrix;
			this.rotation = this.myCam.worldToCameraMatrix;
			this.inverseRotation = this.myCam.cameraToWorldMatrix;
			return;
		default:
			return;
		}
	}

	// Token: 0x060003FA RID: 1018 RVA: 0x000299D4 File Offset: 0x00027BD4
	private void FinalizeFrame()
	{
		this.renderingCounter++;
		this.previousRotation = this.rotation;
		if (EnviroSky.instance.singlePassVR)
		{
			this.previousRotationSPVR = this.rotationSPVR;
		}
		int num = this.reprojectionPixelSize * this.reprojectionPixelSize;
		this.subFrameNumber = this.frameList[this.renderingCounter % num];
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x00029A38 File Offset: 0x00027C38
	private bool UpdateFrameDimensions()
	{
		int num = this.myCam.pixelWidth / EnviroSky.instance.cloudsSettings.cloudsQualitySettings.cloudsRenderResolution;
		int num2 = this.myCam.pixelHeight / EnviroSky.instance.cloudsSettings.cloudsQualitySettings.cloudsRenderResolution;
		if (EnviroSky.instance != null && this.reprojectionPixelSize == 0)
		{
			this.SetReprojectionPixelSize(EnviroSky.instance.cloudsSettings.cloudsQualitySettings.reprojectionPixelSize);
		}
		while (num % this.reprojectionPixelSize != 0)
		{
			num++;
		}
		while (num2 % this.reprojectionPixelSize != 0)
		{
			num2++;
		}
		int num3 = num / this.reprojectionPixelSize;
		int num4 = num2 / this.reprojectionPixelSize;
		if (num != this.frameWidth || num3 != this.subFrameWidth || num2 != this.frameHeight || num4 != this.subFrameHeight)
		{
			this.frameWidth = num;
			this.frameHeight = num2;
			this.subFrameWidth = num3;
			this.subFrameHeight = num4;
			return true;
		}
		this.frameWidth = num;
		this.frameHeight = num2;
		this.subFrameWidth = num3;
		this.subFrameHeight = num4;
		return false;
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x00029B48 File Offset: 0x00027D48
	private int[] CalculateFrames(int reproSize)
	{
		this.subFrameNumber = 0;
		int num = reproSize * reproSize;
		int[] array = new int[num];
		int i;
		for (i = 0; i < num; i++)
		{
			array[i] = i;
		}
		while (i-- > 0)
		{
			int num2 = array[i];
			int num3 = (int)((float)Random.Range(0, 1) * 1000f) % num;
			array[i] = array[num3];
			array[num3] = num2;
		}
		return array;
	}

	// Token: 0x040007DB RID: 2011
	[HideInInspector]
	public bool aboveClouds;

	// Token: 0x040007DC RID: 2012
	[HideInInspector]
	public bool isAddionalCamera;

	// Token: 0x040007DD RID: 2013
	private Camera myCam;

	// Token: 0x040007DE RID: 2014
	private RenderTexture spSatTex;

	// Token: 0x040007DF RID: 2015
	private Camera spSatCam;

	// Token: 0x040007E0 RID: 2016
	public bool useGlobalRenderingSettings = true;

	// Token: 0x040007E1 RID: 2017
	public EnviroCustomRenderingSettings customRenderingSettings = new EnviroCustomRenderingSettings();

	// Token: 0x040007E2 RID: 2018
	private bool useVolumeClouds = true;

	// Token: 0x040007E3 RID: 2019
	private bool useVolumeLighting = true;

	// Token: 0x040007E4 RID: 2020
	private bool useDistanceBlur = true;

	// Token: 0x040007E5 RID: 2021
	private bool useFog = true;

	// Token: 0x040007E6 RID: 2022
	[HideInInspector]
	public EnviroSkyRendering.FogType currentFogType;

	// Token: 0x040007E7 RID: 2023
	private EnviroHaltonSequence sequence = new EnviroHaltonSequence
	{
		radix = 3
	};

	// Token: 0x040007E8 RID: 2024
	private Material cloudsMat;

	// Token: 0x040007E9 RID: 2025
	private Material blitMat;

	// Token: 0x040007EA RID: 2026
	private Material compose;

	// Token: 0x040007EB RID: 2027
	private Material downsample;

	// Token: 0x040007EC RID: 2028
	private RenderTexture subFrameTex;

	// Token: 0x040007ED RID: 2029
	private RenderTexture prevFrameTex;

	// Token: 0x040007EE RID: 2030
	private Texture2D curlMap;

	// Token: 0x040007EF RID: 2031
	private Texture2D dither;

	// Token: 0x040007F0 RID: 2032
	private Texture2D blueNoise;

	// Token: 0x040007F1 RID: 2033
	private Texture3D noiseTexture;

	// Token: 0x040007F2 RID: 2034
	private Texture3D noiseTextureHigh;

	// Token: 0x040007F3 RID: 2035
	private Texture3D detailNoiseTexture;

	// Token: 0x040007F4 RID: 2036
	private Texture3D detailNoiseTextureHigh;

	// Token: 0x040007F5 RID: 2037
	private Matrix4x4 projection;

	// Token: 0x040007F6 RID: 2038
	private Matrix4x4 projectionSPVR;

	// Token: 0x040007F7 RID: 2039
	private Matrix4x4 inverseRotation;

	// Token: 0x040007F8 RID: 2040
	private Matrix4x4 inverseRotationSPVR;

	// Token: 0x040007F9 RID: 2041
	private Matrix4x4 rotation;

	// Token: 0x040007FA RID: 2042
	private Matrix4x4 rotationSPVR;

	// Token: 0x040007FB RID: 2043
	private Matrix4x4 previousRotation;

	// Token: 0x040007FC RID: 2044
	private Matrix4x4 previousRotationSPVR;

	// Token: 0x040007FD RID: 2045
	[HideInInspector]
	public EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize currentReprojectionPixelSize;

	// Token: 0x040007FE RID: 2046
	private int reprojectionPixelSize;

	// Token: 0x040007FF RID: 2047
	private bool isFirstFrame;

	// Token: 0x04000800 RID: 2048
	private int subFrameNumber;

	// Token: 0x04000801 RID: 2049
	private int[] frameList;

	// Token: 0x04000802 RID: 2050
	private int renderingCounter;

	// Token: 0x04000803 RID: 2051
	private int subFrameWidth;

	// Token: 0x04000804 RID: 2052
	private int subFrameHeight;

	// Token: 0x04000805 RID: 2053
	private int frameWidth;

	// Token: 0x04000806 RID: 2054
	private int frameHeight;

	// Token: 0x04000807 RID: 2055
	private bool textureDimensionChanged;

	// Token: 0x04000808 RID: 2056
	private EnviroVolumeCloudsQualitySettings usedCloudsQuality;

	// Token: 0x0400080A RID: 2058
	private static Mesh _pointLightMesh;

	// Token: 0x0400080B RID: 2059
	private static Mesh _spotLightMesh;

	// Token: 0x0400080C RID: 2060
	private static Material _lightMaterial;

	// Token: 0x0400080D RID: 2061
	private CommandBuffer _preLightPass;

	// Token: 0x0400080E RID: 2062
	public CommandBuffer _afterLightPass;

	// Token: 0x0400080F RID: 2063
	private Matrix4x4 _viewProj;

	// Token: 0x04000810 RID: 2064
	private Matrix4x4 _viewProjSP;

	// Token: 0x04000811 RID: 2065
	[HideInInspector]
	public Material fogMat;

	// Token: 0x04000812 RID: 2066
	private Material _bilateralBlurMaterial;

	// Token: 0x04000813 RID: 2067
	private RenderTexture _volumeLightTexture;

	// Token: 0x04000814 RID: 2068
	private RenderTexture _halfVolumeLightTexture;

	// Token: 0x04000815 RID: 2069
	private RenderTexture _quarterVolumeLightTexture;

	// Token: 0x04000816 RID: 2070
	private static Texture _defaultSpotCookie;

	// Token: 0x04000817 RID: 2071
	private RenderTexture _halfDepthBuffer;

	// Token: 0x04000818 RID: 2072
	private RenderTexture _quarterDepthBuffer;

	// Token: 0x04000819 RID: 2073
	private EnviroSkyRendering.VolumtericResolution currentVolumeRes;

	// Token: 0x0400081A RID: 2074
	[HideInInspector]
	public Texture2D _ditheringTexture;

	// Token: 0x0400081B RID: 2075
	private Texture2D blackTexture;

	// Token: 0x0400081C RID: 2076
	[HideInInspector]
	public Texture DefaultSpotCookie;

	// Token: 0x0400081D RID: 2077
	[HideInInspector]
	public Material volumeLightMat;

	// Token: 0x0400081E RID: 2078
	private Material postProcessMat;

	// Token: 0x0400081F RID: 2079
	private const int kMaxIterations = 16;

	// Token: 0x04000820 RID: 2080
	private RenderTexture[] _blurBuffer1 = new RenderTexture[16];

	// Token: 0x04000821 RID: 2081
	private RenderTexture[] _blurBuffer2 = new RenderTexture[16];

	// Token: 0x04000822 RID: 2082
	private Texture2D distributionTexture;

	// Token: 0x020003A0 RID: 928
	public enum FogType
	{
		// Token: 0x04002775 RID: 10101
		Disabled,
		// Token: 0x04002776 RID: 10102
		Simple,
		// Token: 0x04002777 RID: 10103
		Standard
	}

	// Token: 0x020003A1 RID: 929
	public enum VolumtericResolution
	{
		// Token: 0x04002779 RID: 10105
		Full,
		// Token: 0x0400277A RID: 10106
		Half,
		// Token: 0x0400277B RID: 10107
		Quarter
	}
}
