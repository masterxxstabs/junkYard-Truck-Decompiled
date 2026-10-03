using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001D1 RID: 465
	public sealed class UserLutComponent : PostProcessingComponentRenderTexture<UserLutModel>
	{
		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000B42 RID: 2882 RVA: 0x00093670 File Offset: 0x00091870
		public override bool active
		{
			get
			{
				UserLutModel.Settings settings = base.model.settings;
				return base.model.enabled && settings.lut != null && settings.contribution > 0f && settings.lut.height == (int)Mathf.Sqrt((float)settings.lut.width) && !this.context.interrupted;
			}
		}

		// Token: 0x06000B43 RID: 2883 RVA: 0x000936E0 File Offset: 0x000918E0
		public override void Prepare(Material uberMaterial)
		{
			UserLutModel.Settings settings = base.model.settings;
			uberMaterial.EnableKeyword("USER_LUT");
			uberMaterial.SetTexture(UserLutComponent.Uniforms._UserLut, settings.lut);
			uberMaterial.SetVector(UserLutComponent.Uniforms._UserLut_Params, new Vector4(1f / (float)settings.lut.width, 1f / (float)settings.lut.height, (float)settings.lut.height - 1f, settings.contribution));
		}

		// Token: 0x06000B44 RID: 2884 RVA: 0x00093764 File Offset: 0x00091964
		public void OnGUI()
		{
			UserLutModel.Settings settings = base.model.settings;
			GUI.DrawTexture(new Rect(this.context.viewport.x * (float)Screen.width + 8f, 8f, (float)settings.lut.width, (float)settings.lut.height), settings.lut);
		}

		// Token: 0x0200045F RID: 1119
		private static class Uniforms
		{
			// Token: 0x04002A94 RID: 10900
			internal static readonly int _UserLut = Shader.PropertyToID("_UserLut");

			// Token: 0x04002A95 RID: 10901
			internal static readonly int _UserLut_Params = Shader.PropertyToID("_UserLut_Params");
		}
	}
}
