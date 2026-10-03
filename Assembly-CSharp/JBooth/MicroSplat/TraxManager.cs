using System;
using UnityEngine;

namespace JBooth.MicroSplat
{
	// Token: 0x020002D1 RID: 721
	[ExecuteInEditMode]
	public class TraxManager : MonoBehaviour
	{
		// Token: 0x06001370 RID: 4976 RVA: 0x000CB998 File Offset: 0x000C9B98
		public void Setup()
		{
			this.TearDown();
			RenderTextureDescriptor desc;
			if (this.precision == TraxManager.Precision.Full)
			{
				if (this.useTime && (this.repairTotal > 0f || this.repairDelay > 0f))
				{
					desc = new RenderTextureDescriptor(this.bufferSize, this.bufferSize, RenderTextureFormat.RGFloat, 0);
				}
				else
				{
					desc = new RenderTextureDescriptor(this.bufferSize, this.bufferSize, RenderTextureFormat.RFloat, 0);
				}
			}
			else if (this.useTime)
			{
				desc = new RenderTextureDescriptor(this.bufferSize, this.bufferSize, RenderTextureFormat.RGFloat, 0);
			}
			else
			{
				desc = new RenderTextureDescriptor(this.bufferSize, this.bufferSize, RenderTextureFormat.RHalf, 0);
			}
			this.bufferA = new RenderTexture(desc)
			{
				name = "rtTraxBufferA"
			};
			this.bufferB = new RenderTexture(desc)
			{
				name = "rtTraxBufferB"
			};
			this.depthRT = new RenderTexture(this.bufferSize, this.bufferSize, 32, RenderTextureFormat.Depth, RenderTextureReadWrite.Linear)
			{
				name = "rtTraxDepth"
			};
			if (this.cam == null)
			{
				this.cam = new GameObject("Trax Camera")
				{
					hideFlags = HideFlags.HideInHierarchy
				}.AddComponent<Camera>();
			}
			this.cam.orthographic = true;
			this.cam.orthographicSize = this.worldSize;
			this.cam.enabled = false;
			this.cam.transform.forward = Vector3.up;
			this.cam.orthographicSize = this.worldSize;
			this.cam.nearClipPlane = 0f;
			this.cam.farClipPlane = 2000f;
			this.cam.cullingMask = this.layerMask;
			this.cam.clearFlags = CameraClearFlags.Color;
			this.cam.backgroundColor = new Color(99999f, 0f, 0f, 1f);
			this.bufferCopyMat = new Material(Shader.Find("Hidden/MicroSplat/TraxBuffer"));
			this.cam.targetTexture = this.depthRT;
			this.cam.Render();
			this.cam.targetTexture = this.bufferA;
			this.cam.Render();
			this.cam.targetTexture = this.bufferB;
			this.cam.Render();
		}

		// Token: 0x06001371 RID: 4977 RVA: 0x000CBBDC File Offset: 0x000C9DDC
		public void TearDown()
		{
			RenderTexture.active = null;
			this.DisposeRenderTexture(ref this.depthRT);
			this.DisposeRenderTexture(ref this.bufferA);
			this.DisposeRenderTexture(ref this.bufferB);
			if (this.cam != null)
			{
				Object.DestroyImmediate(this.cam.gameObject);
			}
			if (this.bufferCopyMat != null)
			{
				Object.DestroyImmediate(this.bufferCopyMat);
			}
			this.bufferCopyMat = null;
			this.cam = null;
		}

		// Token: 0x06001372 RID: 4978 RVA: 0x000CBC58 File Offset: 0x000C9E58
		private void DisposeRenderTexture(ref RenderTexture rt)
		{
			if (rt == null)
			{
				return;
			}
			rt.Release();
			Object.DestroyImmediate(rt);
			rt = null;
		}

		// Token: 0x06001373 RID: 4979 RVA: 0x000CBC78 File Offset: 0x000C9E78
		public float GetBufferAtPosition(Vector3 terrainPosition)
		{
			if (this.bufferA == null)
			{
				return 99999f;
			}
			if (this.bufferFetch == null)
			{
				this.bufferFetch = new Texture2D(1, 1, TextureFormat.RGBAFloat, false, true);
			}
			RenderTexture renderTexture = this.bufferBActive ? this.bufferB : this.bufferA;
			Vector2 vector = (new Vector2(terrainPosition.x, terrainPosition.z) - new Vector2(base.transform.position.x, base.transform.position.z) + new Vector2(this.worldSize * 0.5f, this.worldSize * 0.5f)) / this.worldSize * (float)renderTexture.width;
			int num = (int)vector.x;
			int num2 = (int)vector.y;
			if (num > renderTexture.width || num2 > renderTexture.height || num < 0 || num2 < 0)
			{
				return 99999f;
			}
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = renderTexture;
			this.bufferFetch.ReadPixels(new Rect((float)num, (float)num2, 1f, 1f), 0, 0);
			this.bufferFetch.Apply();
			ref Color pixel = this.bufferFetch.GetPixel(0, 0);
			RenderTexture.active = active;
			return pixel.r;
		}

		// Token: 0x06001374 RID: 4980 RVA: 0x000CBDC1 File Offset: 0x000C9FC1
		private void OnEnable()
		{
			this.Setup();
		}

		// Token: 0x06001375 RID: 4981 RVA: 0x000CBDC9 File Offset: 0x000C9FC9
		private void OnDisable()
		{
			this.TearDown();
		}

		// Token: 0x06001376 RID: 4982 RVA: 0x000CBDD4 File Offset: 0x000C9FD4
		private float SnapToPixel(float v, int textureSize, float orthoSize)
		{
			float num = orthoSize * 2f / (float)textureSize;
			v = (float)((int)(v / num));
			v *= num;
			return v;
		}

		// Token: 0x06001377 RID: 4983 RVA: 0x000CBDFC File Offset: 0x000C9FFC
		private void LateUpdate()
		{
			if (!Application.isPlaying)
			{
				this.cam.targetTexture = this.bufferA;
				this.cam.Render();
				Shader.SetGlobalTexture(ShaderID._GMSTraxBuffer, this.bufferA);
				return;
			}
			Vector3 vector = base.transform.position + Vector3.up;
			vector.x = this.SnapToPixel(vector.x, this.bufferSize, this.worldSize);
			vector.z = this.SnapToPixel(vector.z, this.bufferSize, this.worldSize);
			vector.y -= 1000f;
			this.cam.transform.position = vector;
			Vector3 vector2 = this.lastPosition - vector;
			vector2.x = this.SnapToPixel(vector2.x, this.bufferSize, this.worldSize);
			vector2.z = this.SnapToPixel(vector2.z, this.bufferSize, this.worldSize);
			vector2.x /= this.worldSize;
			vector2.z /= this.worldSize;
			vector2.x *= 0.5f;
			vector2.z *= 0.5f;
			this.bufferCopyMat.SetVector(ShaderID._Offset, new Vector2(vector2.x, vector2.z));
			this.cam.targetTexture = this.depthRT;
			this.cam.Render();
			this.bufferCopyMat.SetTexture(ShaderID._DepthRT, this.depthRT);
			this.bufferCopyMat.SetFloat(ShaderID._RepairDelay, this.repairDelay);
			this.bufferCopyMat.SetFloat(ShaderID._RepairRate, 1f / Mathf.Max(0.001f, this.repairRate));
			this.bufferCopyMat.SetFloat(ShaderID._UseTime, (float)(this.useTime ? 1 : 0));
			this.bufferCopyMat.SetFloat(ShaderID._RepairTotal, this.repairTotal);
			this.bufferCopyMat.SetFloat(ShaderID._BufferBlend, this.bufferBlend);
			this.bufferCopyMat.SetFloat(ShaderID._SinkStrength, this.sinkStrength);
			this.bufferCopyMat.SetFloat(ShaderID._CamCaptureHeight, vector.y);
			this.bufferCopyMat.SetFloat(ShaderID._CamFarClipPlane, this.cam.farClipPlane);
			RenderTexture renderTexture = this.bufferA;
			RenderTexture renderTexture2 = this.bufferB;
			if (this.bufferBActive)
			{
				this.Swap<RenderTexture>(ref renderTexture, ref renderTexture2);
			}
			Graphics.Blit(renderTexture, renderTexture2, this.bufferCopyMat);
			this.bufferBActive = !this.bufferBActive;
			this.bufferCopyMat.SetVector(ShaderID._Offset, new Vector2(0f, 0f));
			this.bufferCopyMat.SetFloat(ShaderID._UseTime, 0f);
			Shader.SetGlobalTexture(ShaderID._GMSTraxBuffer, renderTexture2);
			for (int i = 0; i < this.bufferBlits; i++)
			{
				Graphics.Blit(renderTexture2, renderTexture, this.bufferCopyMat);
				this.bufferBActive = !this.bufferBActive;
				Shader.SetGlobalTexture(ShaderID._GMSTraxBuffer, renderTexture);
				this.Swap<RenderTexture>(ref renderTexture, ref renderTexture2);
			}
			Shader.SetGlobalVector(ShaderID._GMSTraxBufferPosition, vector);
			Shader.SetGlobalFloat(ShaderID._GMSTraxBufferWorldSize, this.worldSize);
			Shader.SetGlobalFloat(ShaderID._GMSTraxFudgeFactor, this.collsionDistance);
			this.lastPosition = vector;
		}

		// Token: 0x06001378 RID: 4984 RVA: 0x000CC160 File Offset: 0x000CA360
		private void Swap<T>(ref T a, ref T b)
		{
			T t = a;
			a = b;
			b = t;
		}

		// Token: 0x04002415 RID: 9237
		public TraxManager.Precision precision = TraxManager.Precision.Full;

		// Token: 0x04002416 RID: 9238
		public int bufferSize = 1024;

		// Token: 0x04002417 RID: 9239
		public float worldSize = 128f;

		// Token: 0x04002418 RID: 9240
		public LayerMask layerMask;

		// Token: 0x04002419 RID: 9241
		public bool useTime;

		// Token: 0x0400241A RID: 9242
		public float repairDelay;

		// Token: 0x0400241B RID: 9243
		public float repairRate;

		// Token: 0x0400241C RID: 9244
		public float repairTotal;

		// Token: 0x0400241D RID: 9245
		public float bufferBlend = 0.5f;

		// Token: 0x0400241E RID: 9246
		public float collsionDistance = 1f;

		// Token: 0x0400241F RID: 9247
		public float sinkStrength = 0.5f;

		// Token: 0x04002420 RID: 9248
		public int bufferBlits = 1;

		// Token: 0x04002421 RID: 9249
		[HideInInspector]
		public Camera cam;

		// Token: 0x04002422 RID: 9250
		private RenderTexture depthRT;

		// Token: 0x04002423 RID: 9251
		private RenderTexture bufferA;

		// Token: 0x04002424 RID: 9252
		private RenderTexture bufferB;

		// Token: 0x04002425 RID: 9253
		private bool bufferBActive;

		// Token: 0x04002426 RID: 9254
		private Material bufferCopyMat;

		// Token: 0x04002427 RID: 9255
		private Vector3 lastPosition = Vector3.zero;

		// Token: 0x04002428 RID: 9256
		private Texture2D bufferFetch;

		// Token: 0x020004DD RID: 1245
		public enum Precision
		{
			// Token: 0x04002C9A RID: 11418
			Half,
			// Token: 0x04002C9B RID: 11419
			Full
		}
	}
}
