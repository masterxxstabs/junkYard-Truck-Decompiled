using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001C4 RID: 452
	public sealed class BloomComponent : PostProcessingComponentRenderTexture<BloomModel>
	{
		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000AD7 RID: 2775 RVA: 0x0008FE63 File Offset: 0x0008E063
		public override bool active
		{
			get
			{
				return base.model.enabled && base.model.settings.bloom.intensity > 0f && !this.context.interrupted;
			}
		}

		// Token: 0x06000AD8 RID: 2776 RVA: 0x0008FEA0 File Offset: 0x0008E0A0
		public void Prepare(RenderTexture source, Material uberMaterial, Texture autoExposure)
		{
			BloomModel.BloomSettings bloom = base.model.settings.bloom;
			BloomModel.LensDirtSettings lensDirt = base.model.settings.lensDirt;
			Material material = this.context.materialFactory.Get("Hidden/Post FX/Bloom");
			material.shaderKeywords = null;
			material.SetTexture(BloomComponent.Uniforms._AutoExposure, autoExposure);
			int width = this.context.width / 2;
			int num = this.context.height / 2;
			RenderTextureFormat format = Application.isMobilePlatform ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;
			float num2 = Mathf.Log((float)num, 2f) + bloom.radius - 8f;
			int num3 = (int)num2;
			int num4 = Mathf.Clamp(num3, 1, 16);
			float thresholdLinear = bloom.thresholdLinear;
			material.SetFloat(BloomComponent.Uniforms._Threshold, thresholdLinear);
			float num5 = thresholdLinear * bloom.softKnee + 1E-05f;
			Vector3 v = new Vector3(thresholdLinear - num5, num5 * 2f, 0.25f / num5);
			material.SetVector(BloomComponent.Uniforms._Curve, v);
			material.SetFloat(BloomComponent.Uniforms._PrefilterOffs, bloom.antiFlicker ? -0.5f : 0f);
			float num6 = 0.5f + num2 - (float)num3;
			material.SetFloat(BloomComponent.Uniforms._SampleScale, num6);
			if (bloom.antiFlicker)
			{
				material.EnableKeyword("ANTI_FLICKER");
			}
			RenderTexture renderTexture = this.context.renderTextureFactory.Get(width, num, 0, format, RenderTextureReadWrite.Default, FilterMode.Bilinear, TextureWrapMode.Clamp, "FactoryTempTexture");
			Graphics.Blit(source, renderTexture, material, 0);
			RenderTexture renderTexture2 = renderTexture;
			for (int i = 0; i < num4; i++)
			{
				this.m_BlurBuffer1[i] = this.context.renderTextureFactory.Get(renderTexture2.width / 2, renderTexture2.height / 2, 0, format, RenderTextureReadWrite.Default, FilterMode.Bilinear, TextureWrapMode.Clamp, "FactoryTempTexture");
				int pass = (i == 0) ? 1 : 2;
				Graphics.Blit(renderTexture2, this.m_BlurBuffer1[i], material, pass);
				renderTexture2 = this.m_BlurBuffer1[i];
			}
			for (int j = num4 - 2; j >= 0; j--)
			{
				RenderTexture renderTexture3 = this.m_BlurBuffer1[j];
				material.SetTexture(BloomComponent.Uniforms._BaseTex, renderTexture3);
				this.m_BlurBuffer2[j] = this.context.renderTextureFactory.Get(renderTexture3.width, renderTexture3.height, 0, format, RenderTextureReadWrite.Default, FilterMode.Bilinear, TextureWrapMode.Clamp, "FactoryTempTexture");
				Graphics.Blit(renderTexture2, this.m_BlurBuffer2[j], material, 3);
				renderTexture2 = this.m_BlurBuffer2[j];
			}
			RenderTexture renderTexture4 = renderTexture2;
			for (int k = 0; k < 16; k++)
			{
				if (this.m_BlurBuffer1[k] != null)
				{
					this.context.renderTextureFactory.Release(this.m_BlurBuffer1[k]);
				}
				if (this.m_BlurBuffer2[k] != null && this.m_BlurBuffer2[k] != renderTexture4)
				{
					this.context.renderTextureFactory.Release(this.m_BlurBuffer2[k]);
				}
				this.m_BlurBuffer1[k] = null;
				this.m_BlurBuffer2[k] = null;
			}
			this.context.renderTextureFactory.Release(renderTexture);
			uberMaterial.SetTexture(BloomComponent.Uniforms._BloomTex, renderTexture4);
			uberMaterial.SetVector(BloomComponent.Uniforms._Bloom_Settings, new Vector2(num6, bloom.intensity));
			if (lensDirt.intensity > 0f && lensDirt.texture != null)
			{
				uberMaterial.SetTexture(BloomComponent.Uniforms._Bloom_DirtTex, lensDirt.texture);
				uberMaterial.SetFloat(BloomComponent.Uniforms._Bloom_DirtIntensity, lensDirt.intensity);
				uberMaterial.EnableKeyword("BLOOM_LENS_DIRT");
				return;
			}
			uberMaterial.EnableKeyword("BLOOM");
		}

		// Token: 0x04001D40 RID: 7488
		private const int k_MaxPyramidBlurLevel = 16;

		// Token: 0x04001D41 RID: 7489
		private readonly RenderTexture[] m_BlurBuffer1 = new RenderTexture[16];

		// Token: 0x04001D42 RID: 7490
		private readonly RenderTexture[] m_BlurBuffer2 = new RenderTexture[16];

		// Token: 0x0200044C RID: 1100
		private static class Uniforms
		{
			// Token: 0x040029EA RID: 10730
			internal static readonly int _AutoExposure = Shader.PropertyToID("_AutoExposure");

			// Token: 0x040029EB RID: 10731
			internal static readonly int _Threshold = Shader.PropertyToID("_Threshold");

			// Token: 0x040029EC RID: 10732
			internal static readonly int _Curve = Shader.PropertyToID("_Curve");

			// Token: 0x040029ED RID: 10733
			internal static readonly int _PrefilterOffs = Shader.PropertyToID("_PrefilterOffs");

			// Token: 0x040029EE RID: 10734
			internal static readonly int _SampleScale = Shader.PropertyToID("_SampleScale");

			// Token: 0x040029EF RID: 10735
			internal static readonly int _BaseTex = Shader.PropertyToID("_BaseTex");

			// Token: 0x040029F0 RID: 10736
			internal static readonly int _BloomTex = Shader.PropertyToID("_BloomTex");

			// Token: 0x040029F1 RID: 10737
			internal static readonly int _Bloom_Settings = Shader.PropertyToID("_Bloom_Settings");

			// Token: 0x040029F2 RID: 10738
			internal static readonly int _Bloom_DirtTex = Shader.PropertyToID("_Bloom_DirtTex");

			// Token: 0x040029F3 RID: 10739
			internal static readonly int _Bloom_DirtIntensity = Shader.PropertyToID("_Bloom_DirtIntensity");
		}
	}
}
