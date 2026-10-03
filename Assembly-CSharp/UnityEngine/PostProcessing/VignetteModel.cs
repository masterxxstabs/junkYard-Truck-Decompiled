using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001E1 RID: 481
	[Serializable]
	public class VignetteModel : PostProcessingModel
	{
		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000B88 RID: 2952 RVA: 0x00093C20 File Offset: 0x00091E20
		// (set) Token: 0x06000B89 RID: 2953 RVA: 0x00093C28 File Offset: 0x00091E28
		public VignetteModel.Settings settings
		{
			get
			{
				return this.m_Settings;
			}
			set
			{
				this.m_Settings = value;
			}
		}

		// Token: 0x06000B8A RID: 2954 RVA: 0x00093C31 File Offset: 0x00091E31
		public override void Reset()
		{
			this.m_Settings = VignetteModel.Settings.defaultSettings;
		}

		// Token: 0x04001D7D RID: 7549
		[SerializeField]
		private VignetteModel.Settings m_Settings = VignetteModel.Settings.defaultSettings;

		// Token: 0x0200048B RID: 1163
		public enum Mode
		{
			// Token: 0x04002B4D RID: 11085
			Classic,
			// Token: 0x04002B4E RID: 11086
			Masked
		}

		// Token: 0x0200048C RID: 1164
		[Serializable]
		public struct Settings
		{
			// Token: 0x1700038D RID: 909
			// (get) Token: 0x06001A73 RID: 6771 RVA: 0x000F6BA4 File Offset: 0x000F4DA4
			public static VignetteModel.Settings defaultSettings
			{
				get
				{
					return new VignetteModel.Settings
					{
						mode = VignetteModel.Mode.Classic,
						color = new Color(0f, 0f, 0f, 1f),
						center = new Vector2(0.5f, 0.5f),
						intensity = 0.45f,
						smoothness = 0.2f,
						roundness = 1f,
						mask = null,
						opacity = 1f,
						rounded = false
					};
				}
			}

			// Token: 0x04002B4F RID: 11087
			[Tooltip("Use the \"Classic\" mode for parametric controls. Use the \"Masked\" mode to use your own texture mask.")]
			public VignetteModel.Mode mode;

			// Token: 0x04002B50 RID: 11088
			[ColorUsage(false)]
			[Tooltip("Vignette color. Use the alpha channel for transparency.")]
			public Color color;

			// Token: 0x04002B51 RID: 11089
			[Tooltip("Sets the vignette center point (screen center is [0.5,0.5]).")]
			public Vector2 center;

			// Token: 0x04002B52 RID: 11090
			[Range(0f, 1f)]
			[Tooltip("Amount of vignetting on screen.")]
			public float intensity;

			// Token: 0x04002B53 RID: 11091
			[Range(0.01f, 1f)]
			[Tooltip("Smoothness of the vignette borders.")]
			public float smoothness;

			// Token: 0x04002B54 RID: 11092
			[Range(0f, 1f)]
			[Tooltip("Lower values will make a square-ish vignette.")]
			public float roundness;

			// Token: 0x04002B55 RID: 11093
			[Tooltip("A black and white mask to use as a vignette.")]
			public Texture mask;

			// Token: 0x04002B56 RID: 11094
			[Range(0f, 1f)]
			[Tooltip("Mask opacity.")]
			public float opacity;

			// Token: 0x04002B57 RID: 11095
			[Tooltip("Should the vignette be perfectly round or be dependent on the current aspect ratio?")]
			public bool rounded;
		}
	}
}
