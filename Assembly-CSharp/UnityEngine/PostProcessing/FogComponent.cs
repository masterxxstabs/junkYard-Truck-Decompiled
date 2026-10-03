using System;
using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001CB RID: 459
	public sealed class FogComponent : PostProcessingComponentCommandBuffer<FogModel>
	{
		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000B16 RID: 2838 RVA: 0x00092069 File Offset: 0x00090269
		public override bool active
		{
			get
			{
				return base.model.enabled && this.context.isGBufferAvailable && RenderSettings.fog && !this.context.interrupted;
			}
		}

		// Token: 0x06000B17 RID: 2839 RVA: 0x0009209C File Offset: 0x0009029C
		public override string GetName()
		{
			return "Fog";
		}

		// Token: 0x06000B18 RID: 2840 RVA: 0x000915D6 File Offset: 0x0008F7D6
		public override DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth;
		}

		// Token: 0x06000B19 RID: 2841 RVA: 0x000920A3 File Offset: 0x000902A3
		public override CameraEvent GetCameraEvent()
		{
			return CameraEvent.AfterImageEffectsOpaque;
		}

		// Token: 0x06000B1A RID: 2842 RVA: 0x000920A8 File Offset: 0x000902A8
		public override void PopulateCommandBuffer(CommandBuffer cb)
		{
			FogModel.Settings settings = base.model.settings;
			Material material = this.context.materialFactory.Get("Hidden/Post FX/Fog");
			material.shaderKeywords = null;
			Color value = GraphicsUtils.isLinearColorSpace ? RenderSettings.fogColor.linear : RenderSettings.fogColor;
			material.SetColor(FogComponent.Uniforms._FogColor, value);
			material.SetFloat(FogComponent.Uniforms._Density, RenderSettings.fogDensity);
			material.SetFloat(FogComponent.Uniforms._Start, RenderSettings.fogStartDistance);
			material.SetFloat(FogComponent.Uniforms._End, RenderSettings.fogEndDistance);
			switch (RenderSettings.fogMode)
			{
			case FogMode.Linear:
				material.EnableKeyword("FOG_LINEAR");
				break;
			case FogMode.Exponential:
				material.EnableKeyword("FOG_EXP");
				break;
			case FogMode.ExponentialSquared:
				material.EnableKeyword("FOG_EXP2");
				break;
			}
			RenderTextureFormat format = this.context.isHdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
			cb.GetTemporaryRT(FogComponent.Uniforms._TempRT, this.context.width, this.context.height, 24, FilterMode.Bilinear, format);
			cb.Blit(BuiltinRenderTextureType.CameraTarget, FogComponent.Uniforms._TempRT);
			cb.Blit(FogComponent.Uniforms._TempRT, BuiltinRenderTextureType.CameraTarget, material, settings.excludeSkybox ? 1 : 0);
			cb.ReleaseTemporaryRT(FogComponent.Uniforms._TempRT);
		}

		// Token: 0x04001D5C RID: 7516
		private const string k_ShaderString = "Hidden/Post FX/Fog";

		// Token: 0x02000455 RID: 1109
		private static class Uniforms
		{
			// Token: 0x04002A2D RID: 10797
			internal static readonly int _FogColor = Shader.PropertyToID("_FogColor");

			// Token: 0x04002A2E RID: 10798
			internal static readonly int _Density = Shader.PropertyToID("_Density");

			// Token: 0x04002A2F RID: 10799
			internal static readonly int _Start = Shader.PropertyToID("_Start");

			// Token: 0x04002A30 RID: 10800
			internal static readonly int _End = Shader.PropertyToID("_End");

			// Token: 0x04002A31 RID: 10801
			internal static readonly int _TempRT = Shader.PropertyToID("_TempRT");
		}
	}
}
