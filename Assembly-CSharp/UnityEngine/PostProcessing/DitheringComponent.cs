using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001C9 RID: 457
	public sealed class DitheringComponent : PostProcessingComponentRenderTexture<DitheringModel>
	{
		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000B09 RID: 2825 RVA: 0x000919F6 File Offset: 0x0008FBF6
		public override bool active
		{
			get
			{
				return base.model.enabled && !this.context.interrupted;
			}
		}

		// Token: 0x06000B0A RID: 2826 RVA: 0x00091A15 File Offset: 0x0008FC15
		public override void OnDisable()
		{
			this.noiseTextures = null;
		}

		// Token: 0x06000B0B RID: 2827 RVA: 0x00091A20 File Offset: 0x0008FC20
		private void LoadNoiseTextures()
		{
			this.noiseTextures = new Texture2D[64];
			for (int i = 0; i < 64; i++)
			{
				this.noiseTextures[i] = Resources.Load<Texture2D>("Bluenoise64/LDR_LLL1_" + i);
			}
		}

		// Token: 0x06000B0C RID: 2828 RVA: 0x00091A64 File Offset: 0x0008FC64
		public override void Prepare(Material uberMaterial)
		{
			int num = this.textureIndex + 1;
			this.textureIndex = num;
			if (num >= 64)
			{
				this.textureIndex = 0;
			}
			float value = Random.value;
			float value2 = Random.value;
			if (this.noiseTextures == null)
			{
				this.LoadNoiseTextures();
			}
			Texture2D texture2D = this.noiseTextures[this.textureIndex];
			uberMaterial.EnableKeyword("DITHERING");
			uberMaterial.SetTexture(DitheringComponent.Uniforms._DitheringTex, texture2D);
			uberMaterial.SetVector(DitheringComponent.Uniforms._DitheringCoords, new Vector4((float)this.context.width / (float)texture2D.width, (float)this.context.height / (float)texture2D.height, value, value2));
		}

		// Token: 0x04001D4E RID: 7502
		private Texture2D[] noiseTextures;

		// Token: 0x04001D4F RID: 7503
		private int textureIndex;

		// Token: 0x04001D50 RID: 7504
		private const int k_TextureCount = 64;

		// Token: 0x02000453 RID: 1107
		private static class Uniforms
		{
			// Token: 0x04002A25 RID: 10789
			internal static readonly int _DitheringTex = Shader.PropertyToID("_DitheringTex");

			// Token: 0x04002A26 RID: 10790
			internal static readonly int _DitheringCoords = Shader.PropertyToID("_DitheringCoords");
		}
	}
}
