using System;
using System.Collections.Generic;
using UnityEngine;

namespace CalmWater
{
	// Token: 0x02000331 RID: 817
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshRenderer))]
	public class MirrorReflection : MonoBehaviour
	{
		// Token: 0x060014DD RID: 5341 RVA: 0x000DCFB0 File Offset: 0x000DB1B0
		private void OnEnable()
		{
			base.gameObject.layer = LayerMask.NameToLayer("Water");
			this.setMaterial();
		}

		// Token: 0x060014DE RID: 5342 RVA: 0x000DCFCD File Offset: 0x000DB1CD
		private void OnDisable()
		{
			if (this.m_ReflectionCamera != null)
			{
				Object.DestroyImmediate(this.m_ReflectionCamera);
			}
		}

		// Token: 0x060014DF RID: 5343 RVA: 0x000DCFB0 File Offset: 0x000DB1B0
		private void Start()
		{
			base.gameObject.layer = LayerMask.NameToLayer("Water");
			this.setMaterial();
		}

		// Token: 0x060014E0 RID: 5344 RVA: 0x000DCFE8 File Offset: 0x000DB1E8
		public void setMaterial()
		{
			this.m_SharedMaterial = base.GetComponent<Renderer>().sharedMaterial;
		}

		// Token: 0x060014E1 RID: 5345 RVA: 0x000DCFFC File Offset: 0x000DB1FC
		private Camera CreateReflectionCameraFor(Camera cam)
		{
			string name = base.gameObject.name + "Reflection" + cam.name;
			GameObject gameObject = GameObject.Find(name);
			if (!gameObject)
			{
				gameObject = new GameObject(name, new Type[]
				{
					typeof(Camera)
				});
				gameObject.hideFlags = HideFlags.HideAndDontSave;
			}
			if (!gameObject.GetComponent(typeof(Camera)))
			{
				gameObject.AddComponent(typeof(Camera));
			}
			Camera component = gameObject.GetComponent<Camera>();
			component.backgroundColor = this.clearColor;
			component.clearFlags = (this.reflectSkybox ? CameraClearFlags.Skybox : CameraClearFlags.Color);
			this.SetStandardCameraParameter(component, this.reflectionMask);
			if (!component.targetTexture)
			{
				component.targetTexture = this.CreateTextureFor(cam);
			}
			return component;
		}

		// Token: 0x060014E2 RID: 5346 RVA: 0x000DD0CB File Offset: 0x000DB2CB
		private void SetStandardCameraParameter(Camera cam, LayerMask mask)
		{
			cam.cullingMask = (mask & ~(1 << LayerMask.NameToLayer("Water")));
			cam.backgroundColor = Color.black;
			cam.enabled = false;
		}

		// Token: 0x060014E3 RID: 5347 RVA: 0x000DD0FC File Offset: 0x000DB2FC
		private RenderTexture CreateTextureFor(Camera cam)
		{
			int width = Mathf.FloorToInt((float)(cam.pixelWidth / (int)this.Quality));
			int height = Mathf.FloorToInt((float)(cam.pixelHeight / (int)this.Quality));
			return new RenderTexture(width, height, 24)
			{
				hideFlags = HideFlags.DontSave
			};
		}

		// Token: 0x060014E4 RID: 5348 RVA: 0x000DD140 File Offset: 0x000DB340
		public void RenderHelpCameras(Camera currentCam)
		{
			if (this.m_HelperCameras == null)
			{
				this.m_HelperCameras = new Dictionary<Camera, bool>();
			}
			if (!this.m_HelperCameras.ContainsKey(currentCam))
			{
				this.m_HelperCameras.Add(currentCam, false);
			}
			if (this.m_HelperCameras[currentCam] && !this.UpdateSceneView)
			{
				return;
			}
			if (!this.m_ReflectionCamera)
			{
				this.m_ReflectionCamera = this.CreateReflectionCameraFor(currentCam);
			}
			this.RenderReflectionFor(currentCam, this.m_ReflectionCamera);
			this.m_HelperCameras[currentCam] = true;
		}

		// Token: 0x060014E5 RID: 5349 RVA: 0x000DD1C6 File Offset: 0x000DB3C6
		public void LateUpdate()
		{
			if (this.m_HelperCameras != null)
			{
				this.m_HelperCameras.Clear();
			}
		}

		// Token: 0x060014E6 RID: 5350 RVA: 0x000DD1DB File Offset: 0x000DB3DB
		public void WaterTileBeingRendered(Transform tr, Camera currentCam)
		{
			this.RenderHelpCameras(currentCam);
			if (this.m_ReflectionCamera && this.m_SharedMaterial)
			{
				this.m_SharedMaterial.SetTexture(this.reflectionSampler, this.m_ReflectionCamera.targetTexture);
			}
		}

		// Token: 0x060014E7 RID: 5351 RVA: 0x000DD21A File Offset: 0x000DB41A
		public void OnWillRenderObject()
		{
			this.WaterTileBeingRendered(base.transform, Camera.current);
		}

		// Token: 0x060014E8 RID: 5352 RVA: 0x000DD230 File Offset: 0x000DB430
		private void RenderReflectionFor(Camera cam, Camera reflectCamera)
		{
			if (!reflectCamera)
			{
				return;
			}
			if (this.m_SharedMaterial && !this.m_SharedMaterial.HasProperty(this.reflectionSampler))
			{
				return;
			}
			int pixelLightCount = QualitySettings.pixelLightCount;
			if (this.m_DisablePixelLights)
			{
				QualitySettings.pixelLightCount = 0;
			}
			reflectCamera.cullingMask = (this.reflectionMask & ~(1 << LayerMask.NameToLayer("Water")));
			this.SaneCameraSettings(reflectCamera);
			reflectCamera.backgroundColor = this.clearColor;
			reflectCamera.clearFlags = (this.reflectSkybox ? CameraClearFlags.Skybox : CameraClearFlags.Color);
			if (this.reflectSkybox && cam.gameObject.GetComponent(typeof(Skybox)))
			{
				Skybox skybox = (Skybox)reflectCamera.gameObject.GetComponent(typeof(Skybox));
				if (!skybox)
				{
					skybox = (Skybox)reflectCamera.gameObject.AddComponent(typeof(Skybox));
				}
				skybox.material = ((Skybox)cam.GetComponent(typeof(Skybox))).material;
			}
			GL.invertCulling = true;
			Transform transform = base.transform;
			Vector3 eulerAngles = cam.transform.eulerAngles;
			reflectCamera.transform.eulerAngles = new Vector3(-eulerAngles.x, eulerAngles.y, eulerAngles.z);
			reflectCamera.transform.position = cam.transform.position;
			Vector3 position = transform.transform.position;
			position.y = transform.position.y;
			Vector3 up = transform.transform.up;
			float w = -Vector3.Dot(up, position) - this.clipPlaneOffset;
			Vector4 plane = new Vector4(up.x, up.y, up.z, w);
			Matrix4x4 matrix4x = Matrix4x4.zero;
			matrix4x = MirrorReflection.CalculateReflectionMatrix(matrix4x, plane);
			this.m_Oldpos = cam.transform.position;
			Vector3 position2 = matrix4x.MultiplyPoint(this.m_Oldpos);
			reflectCamera.worldToCameraMatrix = cam.worldToCameraMatrix * matrix4x;
			Vector4 clipPlane = this.CameraSpacePlane(reflectCamera, position, up, 1f);
			Matrix4x4 matrix4x2 = cam.projectionMatrix;
			matrix4x2 = MirrorReflection.CalculateObliqueMatrix(matrix4x2, clipPlane);
			reflectCamera.projectionMatrix = matrix4x2;
			reflectCamera.transform.position = position2;
			Vector3 eulerAngles2 = cam.transform.eulerAngles;
			reflectCamera.transform.eulerAngles = new Vector3(-eulerAngles2.x, eulerAngles2.y, eulerAngles2.z);
			reflectCamera.Render();
			GL.invertCulling = false;
			if (this.m_DisablePixelLights)
			{
				QualitySettings.pixelLightCount = pixelLightCount;
			}
		}

		// Token: 0x060014E9 RID: 5353 RVA: 0x000DD4C0 File Offset: 0x000DB6C0
		private void SaneCameraSettings(Camera helperCam)
		{
			helperCam.depthTextureMode = DepthTextureMode.None;
			helperCam.backgroundColor = Color.black;
			helperCam.clearFlags = CameraClearFlags.Color;
			helperCam.renderingPath = RenderingPath.Forward;
		}

		// Token: 0x060014EA RID: 5354 RVA: 0x000DD4E4 File Offset: 0x000DB6E4
		private static Matrix4x4 CalculateObliqueMatrix(Matrix4x4 projection, Vector4 clipPlane)
		{
			Vector4 b = projection.inverse * new Vector4(MirrorReflection.Sgn(clipPlane.x), MirrorReflection.Sgn(clipPlane.y), 1f, 1f);
			Vector4 vector = clipPlane * (2f / Vector4.Dot(clipPlane, b));
			projection[2] = vector.x - projection[3];
			projection[6] = vector.y - projection[7];
			projection[10] = vector.z - projection[11];
			projection[14] = vector.w - projection[15];
			return projection;
		}

		// Token: 0x060014EB RID: 5355 RVA: 0x000DD598 File Offset: 0x000DB798
		private static Matrix4x4 CalculateReflectionMatrix(Matrix4x4 reflectionMat, Vector4 plane)
		{
			reflectionMat.m00 = 1f - 2f * plane[0] * plane[0];
			reflectionMat.m01 = -2f * plane[0] * plane[1];
			reflectionMat.m02 = -2f * plane[0] * plane[2];
			reflectionMat.m03 = -2f * plane[3] * plane[0];
			reflectionMat.m10 = -2f * plane[1] * plane[0];
			reflectionMat.m11 = 1f - 2f * plane[1] * plane[1];
			reflectionMat.m12 = -2f * plane[1] * plane[2];
			reflectionMat.m13 = -2f * plane[3] * plane[1];
			reflectionMat.m20 = -2f * plane[2] * plane[0];
			reflectionMat.m21 = -2f * plane[2] * plane[1];
			reflectionMat.m22 = 1f - 2f * plane[2] * plane[2];
			reflectionMat.m23 = -2f * plane[3] * plane[2];
			reflectionMat.m30 = 0f;
			reflectionMat.m31 = 0f;
			reflectionMat.m32 = 0f;
			reflectionMat.m33 = 1f;
			return reflectionMat;
		}

		// Token: 0x060014EC RID: 5356 RVA: 0x000DD750 File Offset: 0x000DB950
		private static float Sgn(float a)
		{
			if (a > 0f)
			{
				return 1f;
			}
			if (a < 0f)
			{
				return -1f;
			}
			return 0f;
		}

		// Token: 0x060014ED RID: 5357 RVA: 0x000DD774 File Offset: 0x000DB974
		private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
		{
			Vector3 point = pos + normal * this.clipPlaneOffset;
			Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
			Vector3 lhs = worldToCameraMatrix.MultiplyPoint(point);
			Vector3 vector = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
			return new Vector4(vector.x, vector.y, vector.z, -Vector3.Dot(lhs, vector));
		}

		// Token: 0x0400254E RID: 9550
		public LayerMask reflectionMask = -1;

		// Token: 0x0400254F RID: 9551
		[SerializeField]
		private MirrorReflection.QualityLevels Quality = MirrorReflection.QualityLevels.Medium;

		// Token: 0x04002550 RID: 9552
		[Tooltip("Color used instead of skybox if you choose to not render it.")]
		public Color clearColor = Color.grey;

		// Token: 0x04002551 RID: 9553
		public bool reflectSkybox = true;

		// Token: 0x04002552 RID: 9554
		public bool m_DisablePixelLights;

		// Token: 0x04002553 RID: 9555
		[Tooltip("You won't be able to select objects in the scene when thi is active.")]
		public bool UpdateSceneView = true;

		// Token: 0x04002554 RID: 9556
		public float clipPlaneOffset = 0.07f;

		// Token: 0x04002555 RID: 9557
		private string reflectionSampler = "_ReflectionTex";

		// Token: 0x04002556 RID: 9558
		private Vector3 m_Oldpos;

		// Token: 0x04002557 RID: 9559
		private Camera m_ReflectionCamera;

		// Token: 0x04002558 RID: 9560
		private Material m_SharedMaterial;

		// Token: 0x04002559 RID: 9561
		private Dictionary<Camera, bool> m_HelperCameras;

		// Token: 0x020004EE RID: 1262
		private enum QualityLevels
		{
			// Token: 0x04002CD0 RID: 11472
			High = 1,
			// Token: 0x04002CD1 RID: 11473
			Medium,
			// Token: 0x04002CD2 RID: 11474
			Low = 4,
			// Token: 0x04002CD3 RID: 11475
			VeryLow = 8
		}
	}
}
