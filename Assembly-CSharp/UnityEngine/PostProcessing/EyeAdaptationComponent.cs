using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001CA RID: 458
	public sealed class EyeAdaptationComponent : PostProcessingComponentRenderTexture<EyeAdaptationModel>
	{
		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000B0E RID: 2830 RVA: 0x00091B0E File Offset: 0x0008FD0E
		public override bool active
		{
			get
			{
				return base.model.enabled && SystemInfo.supportsComputeShaders && !this.context.interrupted;
			}
		}

		// Token: 0x06000B0F RID: 2831 RVA: 0x00091B34 File Offset: 0x0008FD34
		public void ResetHistory()
		{
			this.m_FirstFrame = true;
		}

		// Token: 0x06000B10 RID: 2832 RVA: 0x00091B34 File Offset: 0x0008FD34
		public override void OnEnable()
		{
			this.m_FirstFrame = true;
		}

		// Token: 0x06000B11 RID: 2833 RVA: 0x00091B40 File Offset: 0x0008FD40
		public override void OnDisable()
		{
			RenderTexture[] autoExposurePool = this.m_AutoExposurePool;
			for (int i = 0; i < autoExposurePool.Length; i++)
			{
				GraphicsUtils.Destroy(autoExposurePool[i]);
			}
			if (this.m_HistogramBuffer != null)
			{
				this.m_HistogramBuffer.Release();
			}
			this.m_HistogramBuffer = null;
			if (this.m_DebugHistogram != null)
			{
				this.m_DebugHistogram.Release();
			}
			this.m_DebugHistogram = null;
		}

		// Token: 0x06000B12 RID: 2834 RVA: 0x00091BA4 File Offset: 0x0008FDA4
		private Vector4 GetHistogramScaleOffsetRes()
		{
			EyeAdaptationModel.Settings settings = base.model.settings;
			float num = (float)(settings.logMax - settings.logMin);
			float num2 = 1f / num;
			float y = (float)(-(float)settings.logMin) * num2;
			return new Vector4(num2, y, Mathf.Floor((float)this.context.width / 2f), Mathf.Floor((float)this.context.height / 2f));
		}

		// Token: 0x06000B13 RID: 2835 RVA: 0x00091C14 File Offset: 0x0008FE14
		public Texture Prepare(RenderTexture source, Material uberMaterial)
		{
			EyeAdaptationModel.Settings settings = base.model.settings;
			if (this.m_EyeCompute == null)
			{
				this.m_EyeCompute = Resources.Load<ComputeShader>("Shaders/EyeHistogram");
			}
			Material material = this.context.materialFactory.Get("Hidden/Post FX/Eye Adaptation");
			material.shaderKeywords = null;
			if (this.m_HistogramBuffer == null)
			{
				this.m_HistogramBuffer = new ComputeBuffer(64, 4);
			}
			if (EyeAdaptationComponent.s_EmptyHistogramBuffer == null)
			{
				EyeAdaptationComponent.s_EmptyHistogramBuffer = new uint[64];
			}
			Vector4 histogramScaleOffsetRes = this.GetHistogramScaleOffsetRes();
			RenderTexture renderTexture = this.context.renderTextureFactory.Get((int)histogramScaleOffsetRes.z, (int)histogramScaleOffsetRes.w, 0, source.format, RenderTextureReadWrite.Default, FilterMode.Bilinear, TextureWrapMode.Clamp, "FactoryTempTexture");
			Graphics.Blit(source, renderTexture);
			if (this.m_AutoExposurePool[0] == null || !this.m_AutoExposurePool[0].IsCreated())
			{
				this.m_AutoExposurePool[0] = new RenderTexture(1, 1, 0, RenderTextureFormat.RFloat);
			}
			if (this.m_AutoExposurePool[1] == null || !this.m_AutoExposurePool[1].IsCreated())
			{
				this.m_AutoExposurePool[1] = new RenderTexture(1, 1, 0, RenderTextureFormat.RFloat);
			}
			this.m_HistogramBuffer.SetData(EyeAdaptationComponent.s_EmptyHistogramBuffer);
			int kernelIndex = this.m_EyeCompute.FindKernel("KEyeHistogram");
			this.m_EyeCompute.SetBuffer(kernelIndex, "_Histogram", this.m_HistogramBuffer);
			this.m_EyeCompute.SetTexture(kernelIndex, "_Source", renderTexture);
			this.m_EyeCompute.SetVector("_ScaleOffsetRes", histogramScaleOffsetRes);
			this.m_EyeCompute.Dispatch(kernelIndex, Mathf.CeilToInt((float)renderTexture.width / 16f), Mathf.CeilToInt((float)renderTexture.height / 16f), 1);
			this.context.renderTextureFactory.Release(renderTexture);
			settings.highPercent = Mathf.Clamp(settings.highPercent, 1.01f, 99f);
			settings.lowPercent = Mathf.Clamp(settings.lowPercent, 1f, settings.highPercent - 0.01f);
			material.SetBuffer("_Histogram", this.m_HistogramBuffer);
			material.SetVector(EyeAdaptationComponent.Uniforms._Params, new Vector4(settings.lowPercent * 0.01f, settings.highPercent * 0.01f, Mathf.Exp(settings.minLuminance * 0.6931472f), Mathf.Exp(settings.maxLuminance * 0.6931472f)));
			material.SetVector(EyeAdaptationComponent.Uniforms._Speed, new Vector2(settings.speedDown, settings.speedUp));
			material.SetVector(EyeAdaptationComponent.Uniforms._ScaleOffsetRes, histogramScaleOffsetRes);
			material.SetFloat(EyeAdaptationComponent.Uniforms._ExposureCompensation, settings.keyValue);
			if (settings.dynamicKeyValue)
			{
				material.EnableKeyword("AUTO_KEY_VALUE");
			}
			if (this.m_FirstFrame || !Application.isPlaying)
			{
				this.m_CurrentAutoExposure = this.m_AutoExposurePool[0];
				Graphics.Blit(null, this.m_CurrentAutoExposure, material, 1);
				Graphics.Blit(this.m_AutoExposurePool[0], this.m_AutoExposurePool[1]);
			}
			else
			{
				int num = this.m_AutoExposurePingPing;
				Texture source2 = this.m_AutoExposurePool[++num % 2];
				RenderTexture renderTexture2 = this.m_AutoExposurePool[++num % 2];
				Graphics.Blit(source2, renderTexture2, material, (int)settings.adaptationType);
				this.m_AutoExposurePingPing = (num + 1) % 2;
				this.m_CurrentAutoExposure = renderTexture2;
			}
			if (this.context.profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.EyeAdaptation))
			{
				if (this.m_DebugHistogram == null || !this.m_DebugHistogram.IsCreated())
				{
					this.m_DebugHistogram = new RenderTexture(256, 128, 0, RenderTextureFormat.ARGB32)
					{
						filterMode = FilterMode.Point,
						wrapMode = TextureWrapMode.Clamp
					};
				}
				material.SetFloat(EyeAdaptationComponent.Uniforms._DebugWidth, (float)this.m_DebugHistogram.width);
				Graphics.Blit(null, this.m_DebugHistogram, material, 2);
			}
			this.m_FirstFrame = false;
			return this.m_CurrentAutoExposure;
		}

		// Token: 0x06000B14 RID: 2836 RVA: 0x00091FD8 File Offset: 0x000901D8
		public void OnGUI()
		{
			if (this.m_DebugHistogram == null || !this.m_DebugHistogram.IsCreated())
			{
				return;
			}
			GUI.DrawTexture(new Rect(this.context.viewport.x * (float)Screen.width + 8f, 8f, (float)this.m_DebugHistogram.width, (float)this.m_DebugHistogram.height), this.m_DebugHistogram);
		}

		// Token: 0x04001D51 RID: 7505
		private ComputeShader m_EyeCompute;

		// Token: 0x04001D52 RID: 7506
		private ComputeBuffer m_HistogramBuffer;

		// Token: 0x04001D53 RID: 7507
		private readonly RenderTexture[] m_AutoExposurePool = new RenderTexture[2];

		// Token: 0x04001D54 RID: 7508
		private int m_AutoExposurePingPing;

		// Token: 0x04001D55 RID: 7509
		private RenderTexture m_CurrentAutoExposure;

		// Token: 0x04001D56 RID: 7510
		private RenderTexture m_DebugHistogram;

		// Token: 0x04001D57 RID: 7511
		private static uint[] s_EmptyHistogramBuffer;

		// Token: 0x04001D58 RID: 7512
		private bool m_FirstFrame = true;

		// Token: 0x04001D59 RID: 7513
		private const int k_HistogramBins = 64;

		// Token: 0x04001D5A RID: 7514
		private const int k_HistogramThreadX = 16;

		// Token: 0x04001D5B RID: 7515
		private const int k_HistogramThreadY = 16;

		// Token: 0x02000454 RID: 1108
		private static class Uniforms
		{
			// Token: 0x04002A27 RID: 10791
			internal static readonly int _Params = Shader.PropertyToID("_Params");

			// Token: 0x04002A28 RID: 10792
			internal static readonly int _Speed = Shader.PropertyToID("_Speed");

			// Token: 0x04002A29 RID: 10793
			internal static readonly int _ScaleOffsetRes = Shader.PropertyToID("_ScaleOffsetRes");

			// Token: 0x04002A2A RID: 10794
			internal static readonly int _ExposureCompensation = Shader.PropertyToID("_ExposureCompensation");

			// Token: 0x04002A2B RID: 10795
			internal static readonly int _AutoExposure = Shader.PropertyToID("_AutoExposure");

			// Token: 0x04002A2C RID: 10796
			internal static readonly int _DebugWidth = Shader.PropertyToID("_DebugWidth");
		}
	}
}
