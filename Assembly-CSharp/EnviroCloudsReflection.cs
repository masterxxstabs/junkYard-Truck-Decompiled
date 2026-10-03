using System;
using UnityEngine;

// Token: 0x020000AC RID: 172
[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
public class EnviroCloudsReflection : MonoBehaviour
{
	// Token: 0x06000422 RID: 1058 RVA: 0x0002B67D File Offset: 0x0002987D
	private void OnEnable()
	{
		this.myCam = base.GetComponent<Camera>();
		this.CreateMaterialsAndTextures();
		this.SetReprojectionPixelSize(this.reprojectionResolution);
		this.currentReprojectionPixelSize = this.reprojectionResolution;
	}

	// Token: 0x06000423 RID: 1059 RVA: 0x0002B6AC File Offset: 0x000298AC
	private void CreateMaterialsAndTextures()
	{
		if (this.mat == null)
		{
			this.mat = new Material(Shader.Find("Enviro/Standard/RaymarchClouds"));
		}
		if (this.blitMat == null)
		{
			this.blitMat = new Material(Shader.Find("Enviro/Standard/Blit"));
		}
		if (this.curlMap == null)
		{
			this.curlMap = (Resources.Load("tex_enviro_curl") as Texture2D);
		}
		if (this.noiseTexture == null)
		{
			this.noiseTexture = (Resources.Load("enviro_clouds_base_low") as Texture3D);
		}
		if (this.noiseTextureHigh == null)
		{
			this.noiseTextureHigh = (Resources.Load("enviro_clouds_base") as Texture3D);
		}
		if (this.detailNoiseTexture == null)
		{
			this.detailNoiseTexture = (Resources.Load("enviro_clouds_detail_low") as Texture3D);
		}
		if (this.detailNoiseTextureHigh == null)
		{
			this.detailNoiseTextureHigh = (Resources.Load("enviro_clouds_detail_high") as Texture3D);
		}
		if (this.blueNoise == null)
		{
			this.blueNoise = (Resources.Load("tex_enviro_blueNoise", typeof(Texture2D)) as Texture2D);
		}
	}

	// Token: 0x06000424 RID: 1060 RVA: 0x0002B7DC File Offset: 0x000299DC
	private void SetCloudProperties()
	{
		if (this.mat == null)
		{
			this.mat = new Material(Shader.Find("Enviro/Standard/RaymarchClouds"));
		}
		this.mat.SetTexture("_WeatherMap", EnviroSky.instance.weatherMap);
		this.mat.SetTexture("_Noise", this.noiseTextureHigh);
		this.mat.SetTexture("_NoiseLow", this.noiseTexture);
		if (EnviroSky.instance.cloudsSettings.cloudsQualitySettings.detailQuality == EnviroVolumeCloudsQualitySettings.CloudDetailQuality.Low)
		{
			this.mat.SetTexture("_DetailNoise", this.detailNoiseTexture);
		}
		else
		{
			this.mat.SetTexture("_DetailNoise", this.detailNoiseTextureHigh);
		}
		switch (this.myCam.stereoActiveEye)
		{
		case Camera.MonoOrStereoscopicEye.Left:
		{
			this.projection = this.myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
			Matrix4x4 inverse = this.projection.inverse;
			this.mat.SetMatrix("_InverseProjection", inverse);
			this.inverseRotation = this.myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Left).inverse;
			this.mat.SetMatrix("_InverseRotation", this.inverseRotation);
			if (this.myCam.stereoEnabled && EnviroSky.instance.singlePassVR)
			{
				Matrix4x4 inverse2 = this.myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right).inverse;
				this.mat.SetMatrix("_InverseProjection_SP", inverse2);
				this.inverseRotationSPVR = this.myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right).inverse;
				this.mat.SetMatrix("_InverseRotation_SP", this.inverseRotationSPVR);
			}
			break;
		}
		case Camera.MonoOrStereoscopicEye.Right:
		{
			this.projection = this.myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
			Matrix4x4 inverse3 = this.projection.inverse;
			this.mat.SetMatrix("_InverseProjection", inverse3);
			this.inverseRotation = this.myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right).inverse;
			this.mat.SetMatrix("_InverseRotation", this.inverseRotation);
			break;
		}
		case Camera.MonoOrStereoscopicEye.Mono:
		{
			this.projection = this.myCam.projectionMatrix;
			Matrix4x4 inverse4 = this.projection.inverse;
			this.mat.SetMatrix("_InverseProjection", inverse4);
			this.inverseRotation = this.myCam.cameraToWorldMatrix;
			this.mat.SetMatrix("_InverseRotation", this.inverseRotation);
			break;
		}
		}
		if (EnviroSky.instance.cloudsSettings.customWeatherMap == null)
		{
			this.mat.SetTexture("_WeatherMap", EnviroSky.instance.weatherMap);
		}
		else
		{
			this.mat.SetTexture("_WeatherMap", EnviroSky.instance.cloudsSettings.customWeatherMap);
		}
		this.mat.SetTexture("_CurlNoise", this.curlMap);
		this.mat.SetVector("_Steps", new Vector4((float)EnviroSky.instance.cloudsSettings.cloudsQualitySettings.raymarchSteps * EnviroSky.instance.cloudsConfig.raymarchingScale, (float)EnviroSky.instance.cloudsSettings.cloudsQualitySettings.raymarchSteps * EnviroSky.instance.cloudsConfig.raymarchingScale, 0f, 0f));
		this.mat.SetFloat("_BaseNoiseUV", EnviroSky.instance.cloudsSettings.cloudsQualitySettings.baseNoiseUV);
		this.mat.SetFloat("_DetailNoiseUV", EnviroSky.instance.cloudsSettings.cloudsQualitySettings.detailNoiseUV);
		this.mat.SetFloat("_AmbientSkyColorIntensity", EnviroSky.instance.cloudsSettings.ambientLightIntensity.Evaluate(EnviroSky.instance.GameTime.solarTime));
		this.mat.SetVector("_CloudsLighting", new Vector4(EnviroSky.instance.cloudsConfig.scatteringCoef, EnviroSky.instance.cloudsSettings.hgPhase, EnviroSky.instance.cloudsSettings.silverLiningIntensity, EnviroSky.instance.cloudsSettings.silverLiningSpread.Evaluate(EnviroSky.instance.GameTime.solarTime)));
		float z = this.tonemapping ? 0f : 1f;
		this.mat.SetVector("_CloudsLightingExtended", new Vector4(EnviroSky.instance.cloudsConfig.edgeDarkness, EnviroSky.instance.cloudsConfig.ambientSkyColorIntensity, z, EnviroSky.instance.cloudsSettings.cloudsExposure));
		this.mat.SetColor("_AmbientLightColor", EnviroSky.instance.cloudsSettings.volumeCloudsAmbientColor.Evaluate(EnviroSky.instance.GameTime.solarTime));
		float num = EnviroSky.instance.cloudsSettings.cloudsQualitySettings.bottomCloudHeight + EnviroSky.instance.cloudsSettings.cloudsHeightMod;
		float num2 = EnviroSky.instance.cloudsSettings.cloudsQualitySettings.topCloudHeight + EnviroSky.instance.cloudsSettings.cloudsHeightMod;
		if (this.myCam.transform.position.y < num - 250f)
		{
			this.mat.SetVector("_CloudsParameter", new Vector4(num, num2, num2 - num, EnviroSky.instance.cloudsSettings.cloudsWorldScale * 10f));
		}
		else
		{
			this.mat.SetVector("_CloudsParameter", new Vector4(this.myCam.transform.position.y + 250f, num2 + (this.myCam.transform.position.y + 250f - num), num2 + (this.myCam.transform.position.y + 250f - num) - (this.myCam.transform.position.y + 250f), EnviroSky.instance.cloudsSettings.cloudsWorldScale * 10f));
		}
		this.mat.SetVector("_CloudDensityScale", new Vector4(EnviroSky.instance.cloudsConfig.density, EnviroSky.instance.cloudsConfig.lightStepModifier, 0f, 0f));
		this.mat.SetFloat("_CloudsType", EnviroSky.instance.cloudsConfig.cloudType);
		this.mat.SetVector("_CloudsCoverageSettings", new Vector4(EnviroSky.instance.cloudsConfig.coverage * EnviroSky.instance.cloudsSettings.globalCloudCoverage, 0f, 0f, 0f));
		this.mat.SetVector("_CloudsAnimation", new Vector4(EnviroSky.instance.cloudAnim.x, EnviroSky.instance.cloudAnim.y, EnviroSky.instance.cloudsSettings.cloudsWindDirectionX, EnviroSky.instance.cloudsSettings.cloudsWindDirectionY));
		this.mat.SetColor("_LightColor", EnviroSky.instance.cloudsSettings.volumeCloudsColor.Evaluate(EnviroSky.instance.GameTime.solarTime));
		this.mat.SetColor("_MoonLightColor", EnviroSky.instance.cloudsSettings.volumeCloudsMoonColor.Evaluate(EnviroSky.instance.GameTime.lunarTime));
		this.mat.SetFloat("_stepsInDepth", EnviroSky.instance.cloudsSettings.cloudsQualitySettings.stepsInDepthModificator);
		this.mat.SetFloat("_LODDistance", EnviroSky.instance.cloudsSettings.cloudsQualitySettings.lodDistance);
		this.mat.SetVector("_LightDir", -EnviroSky.instance.Components.DirectLight.transform.forward);
		this.mat.SetFloat("_LightIntensity", EnviroSky.instance.cloudsSettings.lightIntensity.Evaluate(EnviroSky.instance.GameTime.solarTime));
		this.mat.SetVector("_CloudsErosionIntensity", new Vector4(1f - EnviroSky.instance.cloudsConfig.baseErosionIntensity, EnviroSky.instance.cloudsConfig.detailErosionIntensity, 0f, 0f));
		this.mat.SetTexture("_BlueNoise", this.blueNoise);
		this.mat.SetVector("_Randomness", new Vector4(Random.value, Random.value, Random.value, Random.value));
	}

	// Token: 0x06000425 RID: 1061 RVA: 0x0002C054 File Offset: 0x0002A254
	public void SetBlitmaterialProperties()
	{
		Matrix4x4 inverse = this.projection.inverse;
		this.blitMat.SetMatrix("_PreviousRotation", this.previousRotation);
		this.blitMat.SetMatrix("_Projection", this.projection);
		this.blitMat.SetMatrix("_InverseRotation", this.inverseRotation);
		this.blitMat.SetMatrix("_InverseProjection", inverse);
		if (EnviroSky.instance.singlePassVR)
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

	// Token: 0x06000426 RID: 1062 RVA: 0x0002C1A9 File Offset: 0x0002A3A9
	public void RenderClouds(RenderTexture src, RenderTexture tex)
	{
		this.SetCloudProperties();
		this.mat.SetTexture("_MainTex", src);
		Graphics.Blit(src, tex, this.mat);
	}

	// Token: 0x06000427 RID: 1063 RVA: 0x0002C1D0 File Offset: 0x0002A3D0
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
			if (EnviroSky.instance.singlePassVR)
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
			if (EnviroSky.instance.singlePassVR)
			{
				desc2.vrUsage = VRTextureUsage.TwoEyes;
			}
			this.prevFrameTex = new RenderTexture(desc2);
			this.prevFrameTex.filterMode = FilterMode.Bilinear;
			this.prevFrameTex.hideFlags = HideFlags.HideAndDontSave;
			this.isFirstFrame = true;
		}
	}

	// Token: 0x06000428 RID: 1064 RVA: 0x0002C2F6 File Offset: 0x0002A4F6
	private void Update()
	{
		if (EnviroSky.instance == null)
		{
			return;
		}
		if (this.currentReprojectionPixelSize != this.reprojectionResolution)
		{
			this.currentReprojectionPixelSize = this.reprojectionResolution;
			this.SetReprojectionPixelSize(this.reprojectionResolution);
		}
	}

	// Token: 0x06000429 RID: 1065 RVA: 0x0002C32C File Offset: 0x0002A52C
	[ImageEffectOpaque]
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (EnviroSky.instance == null)
		{
			Graphics.Blit(source, destination);
			return;
		}
		if (EnviroSky.instance.useVolumeClouds)
		{
			this.StartFrame();
			if (this.subFrameTex == null || this.prevFrameTex == null || this.textureDimensionChanged)
			{
				this.CreateCloudsRenderTextures(source);
			}
			this.RenderClouds(source, this.subFrameTex);
			if (this.isFirstFrame)
			{
				Graphics.Blit(this.subFrameTex, this.prevFrameTex);
				this.isFirstFrame = false;
			}
			this.blitMat.SetTexture("_MainTex", source);
			this.blitMat.SetTexture("_SubFrame", this.subFrameTex);
			this.blitMat.SetTexture("_PrevFrame", this.prevFrameTex);
			this.SetBlitmaterialProperties();
			Graphics.Blit(source, destination, this.blitMat);
			Graphics.Blit(this.subFrameTex, this.prevFrameTex);
			this.FinalizeFrame();
			return;
		}
		Graphics.Blit(source, destination);
	}

	// Token: 0x0600042A RID: 1066 RVA: 0x0002C42C File Offset: 0x0002A62C
	public void SetReprojectionPixelSize(EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize pSize)
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

	// Token: 0x0600042B RID: 1067 RVA: 0x0002C488 File Offset: 0x0002A688
	public void StartFrame()
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
			if (this.resetCameraProjection)
			{
				this.myCam.ResetProjectionMatrix();
			}
			this.projection = this.myCam.projectionMatrix;
			this.rotation = this.myCam.worldToCameraMatrix;
			this.inverseRotation = this.myCam.cameraToWorldMatrix;
			return;
		default:
			return;
		}
	}

	// Token: 0x0600042C RID: 1068 RVA: 0x0002C5B4 File Offset: 0x0002A7B4
	public void FinalizeFrame()
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

	// Token: 0x0600042D RID: 1069 RVA: 0x0002C618 File Offset: 0x0002A818
	private bool UpdateFrameDimensions()
	{
		int num = this.myCam.pixelWidth / EnviroSky.instance.cloudsSettings.cloudsQualitySettings.cloudsRenderResolution;
		int num2 = this.myCam.pixelHeight / EnviroSky.instance.cloudsSettings.cloudsQualitySettings.cloudsRenderResolution;
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

	// Token: 0x0600042E RID: 1070 RVA: 0x0002C6F8 File Offset: 0x0002A8F8
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

	// Token: 0x0400084D RID: 2125
	public bool resetCameraProjection = true;

	// Token: 0x0400084E RID: 2126
	public bool tonemapping = true;

	// Token: 0x0400084F RID: 2127
	public EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize reprojectionResolution = EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize.Medium;

	// Token: 0x04000850 RID: 2128
	public Camera myCam;

	// Token: 0x04000851 RID: 2129
	private Material mat;

	// Token: 0x04000852 RID: 2130
	private Material blitMat;

	// Token: 0x04000853 RID: 2131
	private Material weatherMapMat;

	// Token: 0x04000854 RID: 2132
	private RenderTexture subFrameTex;

	// Token: 0x04000855 RID: 2133
	private RenderTexture prevFrameTex;

	// Token: 0x04000856 RID: 2134
	private Texture2D curlMap;

	// Token: 0x04000857 RID: 2135
	private Texture2D blueNoise;

	// Token: 0x04000858 RID: 2136
	private Texture3D noiseTexture;

	// Token: 0x04000859 RID: 2137
	private Texture3D noiseTextureHigh;

	// Token: 0x0400085A RID: 2138
	private Texture3D detailNoiseTexture;

	// Token: 0x0400085B RID: 2139
	private Texture3D detailNoiseTextureHigh;

	// Token: 0x0400085C RID: 2140
	private Matrix4x4 projection;

	// Token: 0x0400085D RID: 2141
	private Matrix4x4 projectionSPVR;

	// Token: 0x0400085E RID: 2142
	private Matrix4x4 inverseRotation;

	// Token: 0x0400085F RID: 2143
	private Matrix4x4 inverseRotationSPVR;

	// Token: 0x04000860 RID: 2144
	private Matrix4x4 rotation;

	// Token: 0x04000861 RID: 2145
	private Matrix4x4 rotationSPVR;

	// Token: 0x04000862 RID: 2146
	private Matrix4x4 previousRotation;

	// Token: 0x04000863 RID: 2147
	private Matrix4x4 previousRotationSPVR;

	// Token: 0x04000864 RID: 2148
	[HideInInspector]
	public EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize currentReprojectionPixelSize;

	// Token: 0x04000865 RID: 2149
	private int reprojectionPixelSize;

	// Token: 0x04000866 RID: 2150
	private bool isFirstFrame;

	// Token: 0x04000867 RID: 2151
	private int subFrameNumber;

	// Token: 0x04000868 RID: 2152
	private int[] frameList;

	// Token: 0x04000869 RID: 2153
	private int renderingCounter;

	// Token: 0x0400086A RID: 2154
	private int subFrameWidth;

	// Token: 0x0400086B RID: 2155
	private int subFrameHeight;

	// Token: 0x0400086C RID: 2156
	private int frameWidth;

	// Token: 0x0400086D RID: 2157
	private int frameHeight;

	// Token: 0x0400086E RID: 2158
	private bool textureDimensionChanged;
}
