using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001D9 RID: 473
	[Serializable]
	public class DepthOfFieldModel : PostProcessingModel
	{
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000B68 RID: 2920 RVA: 0x00093A98 File Offset: 0x00091C98
		// (set) Token: 0x06000B69 RID: 2921 RVA: 0x00093AA0 File Offset: 0x00091CA0
		public DepthOfFieldModel.Settings settings
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

		// Token: 0x06000B6A RID: 2922 RVA: 0x00093AA9 File Offset: 0x00091CA9
		public override void Reset()
		{
			this.m_Settings = DepthOfFieldModel.Settings.defaultSettings;
		}

		// Token: 0x04001D75 RID: 7541
		[SerializeField]
		private DepthOfFieldModel.Settings m_Settings = DepthOfFieldModel.Settings.defaultSettings;

		// Token: 0x0200047C RID: 1148
		public enum KernelSize
		{
			// Token: 0x04002B15 RID: 11029
			Small,
			// Token: 0x04002B16 RID: 11030
			Medium,
			// Token: 0x04002B17 RID: 11031
			Large,
			// Token: 0x04002B18 RID: 11032
			VeryLarge
		}

		// Token: 0x0200047D RID: 1149
		[Serializable]
		public struct Settings
		{
			// Token: 0x17000385 RID: 901
			// (get) Token: 0x06001A6B RID: 6763 RVA: 0x000F6918 File Offset: 0x000F4B18
			public static DepthOfFieldModel.Settings defaultSettings
			{
				get
				{
					return new DepthOfFieldModel.Settings
					{
						focusDistance = 10f,
						aperture = 5.6f,
						focalLength = 50f,
						useCameraFov = false,
						kernelSize = DepthOfFieldModel.KernelSize.Medium
					};
				}
			}

			// Token: 0x04002B19 RID: 11033
			[Min(0.1f)]
			[Tooltip("Distance to the point of focus.")]
			public float focusDistance;

			// Token: 0x04002B1A RID: 11034
			[Range(0.05f, 32f)]
			[Tooltip("Ratio of aperture (known as f-stop or f-number). The smaller the value is, the shallower the depth of field is.")]
			public float aperture;

			// Token: 0x04002B1B RID: 11035
			[Range(1f, 300f)]
			[Tooltip("Distance between the lens and the film. The larger the value is, the shallower the depth of field is.")]
			public float focalLength;

			// Token: 0x04002B1C RID: 11036
			[Tooltip("Calculate the focal length automatically from the field-of-view value set on the camera. Using this setting isn't recommended.")]
			public bool useCameraFov;

			// Token: 0x04002B1D RID: 11037
			[Tooltip("Convolution kernel size of the bokeh filter, which determines the maximum radius of bokeh. It also affects the performance (the larger the kernel is, the longer the GPU time is required).")]
			public DepthOfFieldModel.KernelSize kernelSize;
		}
	}
}
