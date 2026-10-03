using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200008A RID: 138
[AddComponentMenu("Enviro/Reflection Probe")]
[RequireComponent(typeof(ReflectionProbe))]
[ExecuteInEditMode]
public class EnviroReflectionProbe : MonoBehaviour
{
	// Token: 0x06000247 RID: 583 RVA: 0x00019240 File Offset: 0x00017440
	private void OnEnable()
	{
		this.myProbe = base.GetComponent<ReflectionProbe>();
		if (!this.standalone && this.myProbe != null)
		{
			this.myProbe.enabled = true;
		}
		if (this.customRendering)
		{
			this.myProbe.mode = ReflectionProbeMode.Custom;
			this.myProbe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
			this.CreateCubemap();
			this.CreateTexturesAndMaterial();
			this.CreateRenderCamera();
			this.currentRes = this.myProbe.resolution;
			this.rendering = false;
			base.StartCoroutine(this.RefreshFirstTime());
			return;
		}
		this.myProbe.mode = ReflectionProbeMode.Realtime;
		this.myProbe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
		this.myProbe.RenderProbe();
	}

	// Token: 0x06000248 RID: 584 RVA: 0x000192F7 File Offset: 0x000174F7
	private void OnDisable()
	{
		this.Cleanup();
		if (!this.standalone && this.myProbe != null)
		{
			this.myProbe.enabled = false;
		}
	}

	// Token: 0x06000249 RID: 585 RVA: 0x00019324 File Offset: 0x00017524
	private void Cleanup()
	{
		if (this.refreshing != null)
		{
			base.StopCoroutine(this.refreshing);
		}
		if (this.cubemap != null)
		{
			Object.DestroyImmediate(this.cubemap);
		}
		if (this.renderCamObj != null)
		{
			Object.DestroyImmediate(this.renderCamObj);
		}
		if (this.mirrorTexture != null)
		{
			Object.DestroyImmediate(this.mirrorTexture);
		}
		if (this.renderTexture != null)
		{
			Object.DestroyImmediate(this.renderTexture);
		}
	}

	// Token: 0x0600024A RID: 586 RVA: 0x000193AC File Offset: 0x000175AC
	private void CreateRenderCamera()
	{
		if (this.renderCamObj == null)
		{
			this.renderCamObj = new GameObject();
			this.renderCamObj.name = "Reflection Probe Cam";
			this.renderCamObj.hideFlags = HideFlags.HideAndDontSave;
			this.renderCam = this.renderCamObj.AddComponent<Camera>();
			this.renderCam.gameObject.SetActive(true);
			this.renderCam.cameraType = CameraType.Reflection;
			this.renderCam.fieldOfView = 90f;
			this.renderCam.farClipPlane = this.myProbe.farClipPlane;
			this.renderCam.nearClipPlane = this.myProbe.nearClipPlane;
			this.renderCam.clearFlags = (CameraClearFlags)this.myProbe.clearFlags;
			this.renderCam.backgroundColor = this.myProbe.backgroundColor;
			this.renderCam.allowHDR = this.myProbe.hdr;
			this.renderCam.targetTexture = this.cubemap;
			this.renderCam.enabled = false;
			if (EnviroSkyMgr.instance != null && EnviroSkyMgr.instance.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
			{
				this.eSky = this.renderCamObj.AddComponent<EnviroSkyRendering>();
				this.eSky.isAddionalCamera = true;
				this.eSky.useGlobalRenderingSettings = false;
				this.eSky.customRenderingSettings.useVolumeClouds = EnviroSkyMgr.instance.useVolumeClouds;
				this.eSky.customRenderingSettings.useVolumeLighting = false;
				this.eSky.customRenderingSettings.useDistanceBlur = false;
				this.eSky.customRenderingSettings.useFog = true;
				if (this.customCloudsQuality != null)
				{
					this.eSky.customRenderingSettings.customCloudsQuality = this.customCloudsQuality;
				}
			}
		}
	}

	// Token: 0x0600024B RID: 587 RVA: 0x00019578 File Offset: 0x00017778
	private void UpdateCameraSettings()
	{
		if (this.renderCam != null)
		{
			this.renderCam.cullingMask = this.myProbe.cullingMask;
			if (EnviroSkyMgr.instance != null && EnviroSkyMgr.instance.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD && this.eSky != null)
			{
				if (this.customCloudsQuality != null)
				{
					this.eSky.customRenderingSettings.customCloudsQuality = this.customCloudsQuality;
				}
				this.eSky.customRenderingSettings.useVolumeClouds = EnviroSkyMgr.instance.useVolumeClouds;
				this.eSky.customRenderingSettings.useFog = this.useFog;
			}
		}
	}

	// Token: 0x0600024C RID: 588 RVA: 0x00019628 File Offset: 0x00017828
	private Camera CreateBakingCamera()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "Reflection Probe Cam";
		Camera camera = gameObject.AddComponent<Camera>();
		camera.enabled = false;
		camera.gameObject.SetActive(true);
		camera.cameraType = CameraType.Reflection;
		camera.fieldOfView = 90f;
		camera.farClipPlane = this.myProbe.farClipPlane;
		camera.nearClipPlane = this.myProbe.nearClipPlane;
		camera.cullingMask = this.myProbe.cullingMask;
		camera.clearFlags = (CameraClearFlags)this.myProbe.clearFlags;
		camera.backgroundColor = this.myProbe.backgroundColor;
		camera.allowHDR = this.myProbe.hdr;
		camera.targetTexture = this.cubemap;
		if (EnviroSkyMgr.instance != null && EnviroSkyMgr.instance.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
		{
			EnviroSkyRendering enviroSkyRendering = gameObject.AddComponent<EnviroSkyRendering>();
			enviroSkyRendering.isAddionalCamera = true;
			enviroSkyRendering.useGlobalRenderingSettings = false;
			enviroSkyRendering.customRenderingSettings.useVolumeClouds = true;
			enviroSkyRendering.customRenderingSettings.useVolumeLighting = false;
			enviroSkyRendering.customRenderingSettings.useDistanceBlur = false;
			enviroSkyRendering.customRenderingSettings.useFog = true;
		}
		gameObject.hideFlags = HideFlags.HideAndDontSave;
		return camera;
	}

	// Token: 0x0600024D RID: 589 RVA: 0x0001974C File Offset: 0x0001794C
	private void CreateCubemap()
	{
		if (this.cubemap != null && this.myProbe.resolution == this.currentRes)
		{
			return;
		}
		if (this.cubemap != null)
		{
			Object.DestroyImmediate(this.cubemap);
		}
		int resolution = this.myProbe.resolution;
		this.currentRes = resolution;
		RenderTextureFormat format = this.myProbe.hdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
		this.cubemap = new RenderTexture(resolution, resolution, 16, format, RenderTextureReadWrite.Linear);
		this.cubemap.dimension = TextureDimension.Cube;
		this.cubemap.useMipMap = true;
		this.cubemap.autoGenerateMips = false;
		this.cubemap.Create();
		this.finalCubemap = new RenderTexture(resolution, resolution, 16, format, RenderTextureReadWrite.Linear);
		this.finalCubemap.dimension = TextureDimension.Cube;
		this.finalCubemap.useMipMap = true;
		this.finalCubemap.autoGenerateMips = false;
		this.finalCubemap.Create();
	}

	// Token: 0x0600024E RID: 590 RVA: 0x0001983C File Offset: 0x00017A3C
	private void CreateTexturesAndMaterial()
	{
		if (this.mirror == null)
		{
			this.mirror = new Material(Shader.Find("Hidden/Enviro/ReflectionProbe"));
		}
		if (this.convolutionMat == null)
		{
			this.convolutionMat = new Material(Shader.Find("Hidden/CubeBlur"));
		}
		int resolution = base.GetComponent<ReflectionProbe>().resolution;
		RenderTextureFormat format = this.myProbe.hdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
		if (this.mirrorTexture == null || this.mirrorTexture.width != resolution || this.mirrorTexture.height != resolution)
		{
			if (this.mirrorTexture != null)
			{
				Object.DestroyImmediate(this.mirrorTexture);
			}
			this.mirrorTexture = new RenderTexture(resolution, resolution, 16, format, RenderTextureReadWrite.Linear);
			this.mirrorTexture.useMipMap = true;
			this.mirrorTexture.autoGenerateMips = false;
			this.mirrorTexture.Create();
		}
		if (this.renderTexture == null || this.renderTexture.width != resolution || this.renderTexture.height != resolution)
		{
			if (this.renderTexture != null)
			{
				Object.DestroyImmediate(this.renderTexture);
			}
			this.renderTexture = new RenderTexture(resolution, resolution, 16, format, RenderTextureReadWrite.Linear);
			this.renderTexture.useMipMap = true;
			this.renderTexture.autoGenerateMips = false;
			this.renderTexture.Create();
		}
	}

	// Token: 0x0600024F RID: 591 RVA: 0x000199A0 File Offset: 0x00017BA0
	public void RefreshReflection(bool timeSlice = false)
	{
		if (!this.customRendering)
		{
			if (!this.paused)
			{
				this.myProbe.RenderProbe();
			}
			return;
		}
		if (this.rendering || this.paused)
		{
			return;
		}
		this.CreateTexturesAndMaterial();
		if (this.renderCam == null)
		{
			this.CreateRenderCamera();
		}
		this.UpdateCameraSettings();
		this.renderCam.transform.position = base.transform.position;
		this.renderCam.targetTexture = this.renderTexture;
		if (!timeSlice)
		{
			this.refreshing = base.StartCoroutine(this.RefreshInstant(this.renderTexture, this.mirrorTexture));
			return;
		}
		this.refreshing = base.StartCoroutine(this.RefreshOvertime(this.renderTexture, this.mirrorTexture));
	}

	// Token: 0x06000250 RID: 592 RVA: 0x00019A6A File Offset: 0x00017C6A
	private IEnumerator RefreshFirstTime()
	{
		yield return null;
		this.RefreshReflection(false);
		yield break;
	}

	// Token: 0x06000251 RID: 593 RVA: 0x00019A79 File Offset: 0x00017C79
	private IEnumerator HDRPWorkaround()
	{
		yield return null;
		this.paused = false;
		if (this.myProbe != null)
		{
			this.myProbe.enabled = true;
		}
		yield break;
	}

	// Token: 0x06000252 RID: 594 RVA: 0x00019A88 File Offset: 0x00017C88
	private IEnumerator RefreshInstant(RenderTexture renderTex, RenderTexture mirrorTex)
	{
		yield return null;
		for (int i = 0; i < 6; i++)
		{
			this.CreateCubemap();
			this.rendering = true;
			this.renderCam.transform.rotation = EnviroReflectionProbe.orientations[i];
			this.renderCam.Render();
			Graphics.Blit(renderTex, mirrorTex, this.mirror);
			Graphics.CopyTexture(mirrorTex, 0, 0, this.cubemap, i, 0);
			this.ClearTextures();
		}
		this.ConvolutionCubemap();
		this.myProbe.customBakedTexture = this.finalCubemap;
		this.rendering = false;
		this.refreshing = null;
		yield break;
	}

	// Token: 0x06000253 RID: 595 RVA: 0x00019AA5 File Offset: 0x00017CA5
	private IEnumerator RefreshOvertime(RenderTexture renderTex, RenderTexture mirrorTex)
	{
		try
		{
			int num;
			for (int face = 0; face < 6; face = num + 1)
			{
				this.CreateCubemap();
				this.rendering = true;
				this.ClearTextures();
				this.renderCam.transform.rotation = EnviroReflectionProbe.orientations[face];
				this.renderCam.Render();
				Graphics.Blit(renderTex, mirrorTex, this.mirror);
				Graphics.CopyTexture(mirrorTex, 0, 0, this.cubemap, face, 0);
				yield return null;
				num = face;
			}
			this.ConvolutionCubemap();
			this.myProbe.customBakedTexture = this.finalCubemap;
		}
		finally
		{
			this.rendering = false;
			this.refreshing = null;
		}
		yield break;
		yield break;
	}

	// Token: 0x06000254 RID: 596 RVA: 0x00019AC4 File Offset: 0x00017CC4
	public RenderTexture BakeCubemapFace(int face, int res)
	{
		if (this.bakeMat == null)
		{
			this.bakeMat = new Material(Shader.Find("Hidden/Enviro/BakeCubemap"));
		}
		if (this.bakingCam == null)
		{
			this.bakingCam = this.CreateBakingCamera();
		}
		this.bakingCam.transform.rotation = EnviroReflectionProbe.orientations[face];
		RenderTexture temporary = RenderTexture.GetTemporary(res, res, 0, RenderTextureFormat.DefaultHDR);
		this.bakingCam.targetTexture = temporary;
		this.bakingCam.Render();
		RenderTexture renderTexture = new RenderTexture(res, res, 0, RenderTextureFormat.DefaultHDR);
		Graphics.Blit(temporary, renderTexture, this.bakeMat);
		RenderTexture.ReleaseTemporary(temporary);
		return renderTexture;
	}

	// Token: 0x06000255 RID: 597 RVA: 0x00019B6A File Offset: 0x00017D6A
	private void ClearTextures()
	{
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = this.renderTexture;
		GL.Clear(true, true, Color.clear);
		RenderTexture.active = this.mirrorTexture;
		GL.Clear(true, true, Color.clear);
		RenderTexture.active = active;
	}

	// Token: 0x06000256 RID: 598 RVA: 0x00019BA4 File Offset: 0x00017DA4
	private void ConvolutionCubemap()
	{
		int num = 7;
		GL.PushMatrix();
		GL.LoadOrtho();
		for (int i = 0; i < num + 1; i++)
		{
			Graphics.CopyTexture(this.cubemap, 0, i, this.finalCubemap, 0, i);
			Graphics.CopyTexture(this.cubemap, 1, i, this.finalCubemap, 1, i);
			Graphics.CopyTexture(this.cubemap, 2, i, this.finalCubemap, 2, i);
			Graphics.CopyTexture(this.cubemap, 3, i, this.finalCubemap, 3, i);
			Graphics.CopyTexture(this.cubemap, 4, i, this.finalCubemap, 4, i);
			Graphics.CopyTexture(this.cubemap, 5, i, this.finalCubemap, 5, i);
			int num2 = i + 1;
			if (num2 == num)
			{
				break;
			}
			int num3 = this.finalCubemap.width / (int)Mathf.Pow(2f, (float)i);
			this.convolutionMat.SetTexture("_MainTex", this.finalCubemap);
			this.convolutionMat.SetFloat("_Texel", 7f / (float)num3);
			this.convolutionMat.SetFloat("_Level", (float)i);
			this.convolutionMat.SetFloat("_Scale", 0.001f * (float)i);
			this.convolutionMat.SetPass(0);
			Graphics.SetRenderTarget(this.cubemap, num2, CubemapFace.PositiveX);
			GL.Begin(7);
			GL.TexCoord3(1f, 1f, 1f);
			GL.Vertex3(0f, 0f, 1f);
			GL.TexCoord3(1f, -1f, 1f);
			GL.Vertex3(0f, 1f, 1f);
			GL.TexCoord3(1f, -1f, -1f);
			GL.Vertex3(1f, 1f, 1f);
			GL.TexCoord3(1f, 1f, -1f);
			GL.Vertex3(1f, 0f, 1f);
			GL.End();
			Graphics.SetRenderTarget(this.cubemap, num2, CubemapFace.NegativeX);
			GL.Begin(7);
			GL.TexCoord3(-1f, 1f, -1f);
			GL.Vertex3(0f, 0f, 1f);
			GL.TexCoord3(-1f, -1f, -1f);
			GL.Vertex3(0f, 1f, 1f);
			GL.TexCoord3(-1f, -1f, 1f);
			GL.Vertex3(1f, 1f, 1f);
			GL.TexCoord3(-1f, 1f, 1f);
			GL.Vertex3(1f, 0f, 1f);
			GL.End();
			Graphics.SetRenderTarget(this.cubemap, num2, CubemapFace.PositiveY);
			GL.Begin(7);
			GL.TexCoord3(-1f, 1f, -1f);
			GL.Vertex3(0f, 0f, 1f);
			GL.TexCoord3(-1f, 1f, 1f);
			GL.Vertex3(0f, 1f, 1f);
			GL.TexCoord3(1f, 1f, 1f);
			GL.Vertex3(1f, 1f, 1f);
			GL.TexCoord3(1f, 1f, -1f);
			GL.Vertex3(1f, 0f, 1f);
			GL.End();
			Graphics.SetRenderTarget(this.cubemap, num2, CubemapFace.NegativeY);
			GL.Begin(7);
			GL.TexCoord3(-1f, -1f, 1f);
			GL.Vertex3(0f, 0f, 1f);
			GL.TexCoord3(-1f, -1f, -1f);
			GL.Vertex3(0f, 1f, 1f);
			GL.TexCoord3(1f, -1f, -1f);
			GL.Vertex3(1f, 1f, 1f);
			GL.TexCoord3(1f, -1f, 1f);
			GL.Vertex3(1f, 0f, 1f);
			GL.End();
			Graphics.SetRenderTarget(this.cubemap, num2, CubemapFace.PositiveZ);
			GL.Begin(7);
			GL.TexCoord3(-1f, 1f, 1f);
			GL.Vertex3(0f, 0f, 1f);
			GL.TexCoord3(-1f, -1f, 1f);
			GL.Vertex3(0f, 1f, 1f);
			GL.TexCoord3(1f, -1f, 1f);
			GL.Vertex3(1f, 1f, 1f);
			GL.TexCoord3(1f, 1f, 1f);
			GL.Vertex3(1f, 0f, 1f);
			GL.End();
			Graphics.SetRenderTarget(this.cubemap, num2, CubemapFace.NegativeZ);
			GL.Begin(7);
			GL.TexCoord3(1f, 1f, -1f);
			GL.Vertex3(0f, 0f, 1f);
			GL.TexCoord3(1f, -1f, -1f);
			GL.Vertex3(0f, 1f, 1f);
			GL.TexCoord3(-1f, -1f, -1f);
			GL.Vertex3(1f, 1f, 1f);
			GL.TexCoord3(-1f, 1f, -1f);
			GL.Vertex3(1f, 0f, 1f);
			GL.End();
		}
		GL.PopMatrix();
	}

	// Token: 0x06000257 RID: 599 RVA: 0x0001A130 File Offset: 0x00018330
	private void UpdateStandaloneReflection()
	{
		if ((EnviroSkyMgr.instance.GetCurrentTimeInHours() > this.lastRelfectionUpdate + (double)this.reflectionsUpdateTreshhold || EnviroSkyMgr.instance.GetCurrentTimeInHours() < this.lastRelfectionUpdate - (double)this.reflectionsUpdateTreshhold) && this.updateReflectionOnGameTime)
		{
			this.lastRelfectionUpdate = EnviroSkyMgr.instance.GetCurrentTimeInHours();
			this.RefreshReflection(!this.useTimeSlicing);
		}
	}

	// Token: 0x06000258 RID: 600 RVA: 0x0001A198 File Offset: 0x00018398
	private void Update()
	{
		if (this.currentMode != this.customRendering)
		{
			this.currentMode = this.customRendering;
			if (this.customRendering)
			{
				this.OnEnable();
			}
			else
			{
				this.OnEnable();
				this.Cleanup();
			}
		}
		if (EnviroSkyMgr.instance != null && this.standalone)
		{
			this.UpdateStandaloneReflection();
		}
	}

	// Token: 0x0400068B RID: 1675
	public bool standalone;

	// Token: 0x0400068C RID: 1676
	public bool updateReflectionOnGameTime = true;

	// Token: 0x0400068D RID: 1677
	public float reflectionsUpdateTreshhold = 0.025f;

	// Token: 0x0400068E RID: 1678
	public bool useTimeSlicing = true;

	// Token: 0x0400068F RID: 1679
	[HideInInspector]
	public bool rendering;

	// Token: 0x04000690 RID: 1680
	[HideInInspector]
	public ReflectionProbe myProbe;

	// Token: 0x04000691 RID: 1681
	public bool customRendering;

	// Token: 0x04000692 RID: 1682
	public EnviroVolumeCloudsQuality customCloudsQuality;

	// Token: 0x04000693 RID: 1683
	private EnviroSkyRendering eSky;

	// Token: 0x04000694 RID: 1684
	public bool useFog;

	// Token: 0x04000695 RID: 1685
	private Camera bakingCam;

	// Token: 0x04000696 RID: 1686
	private bool currentMode;

	// Token: 0x04000697 RID: 1687
	private int currentRes;

	// Token: 0x04000698 RID: 1688
	private RenderTexture cubemap;

	// Token: 0x04000699 RID: 1689
	private RenderTexture finalCubemap;

	// Token: 0x0400069A RID: 1690
	private RenderTexture mirrorTexture;

	// Token: 0x0400069B RID: 1691
	private RenderTexture renderTexture;

	// Token: 0x0400069C RID: 1692
	private GameObject renderCamObj;

	// Token: 0x0400069D RID: 1693
	private Camera renderCam;

	// Token: 0x0400069E RID: 1694
	private Material mirror;

	// Token: 0x0400069F RID: 1695
	private Material bakeMat;

	// Token: 0x040006A0 RID: 1696
	private Material convolutionMat;

	// Token: 0x040006A1 RID: 1697
	private Coroutine refreshing;

	// Token: 0x040006A2 RID: 1698
	private bool paused;

	// Token: 0x040006A3 RID: 1699
	private static Quaternion[] orientations = new Quaternion[]
	{
		Quaternion.LookRotation(Vector3.right, Vector3.down),
		Quaternion.LookRotation(Vector3.left, Vector3.down),
		Quaternion.LookRotation(Vector3.up, Vector3.forward),
		Quaternion.LookRotation(Vector3.down, Vector3.back),
		Quaternion.LookRotation(Vector3.forward, Vector3.down),
		Quaternion.LookRotation(Vector3.back, Vector3.down)
	};

	// Token: 0x040006A4 RID: 1700
	private double lastRelfectionUpdate;
}
