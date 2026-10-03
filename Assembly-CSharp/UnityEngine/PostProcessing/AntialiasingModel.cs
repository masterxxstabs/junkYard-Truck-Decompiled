using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001D4 RID: 468
	[Serializable]
	public class AntialiasingModel : PostProcessingModel
	{
		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000B4D RID: 2893 RVA: 0x00093929 File Offset: 0x00091B29
		// (set) Token: 0x06000B4E RID: 2894 RVA: 0x00093931 File Offset: 0x00091B31
		public AntialiasingModel.Settings settings
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

		// Token: 0x06000B4F RID: 2895 RVA: 0x0009393A File Offset: 0x00091B3A
		public override void Reset()
		{
			this.m_Settings = AntialiasingModel.Settings.defaultSettings;
		}

		// Token: 0x04001D6E RID: 7534
		[SerializeField]
		private AntialiasingModel.Settings m_Settings = AntialiasingModel.Settings.defaultSettings;

		// Token: 0x02000463 RID: 1123
		public enum Method
		{
			// Token: 0x04002AA8 RID: 10920
			Fxaa,
			// Token: 0x04002AA9 RID: 10921
			Taa
		}

		// Token: 0x02000464 RID: 1124
		public enum FxaaPreset
		{
			// Token: 0x04002AAB RID: 10923
			ExtremePerformance,
			// Token: 0x04002AAC RID: 10924
			Performance,
			// Token: 0x04002AAD RID: 10925
			Default,
			// Token: 0x04002AAE RID: 10926
			Quality,
			// Token: 0x04002AAF RID: 10927
			ExtremeQuality
		}

		// Token: 0x02000465 RID: 1125
		[Serializable]
		public struct FxaaQualitySettings
		{
			// Token: 0x04002AB0 RID: 10928
			[Tooltip("The amount of desired sub-pixel aliasing removal. Effects the sharpeness of the output.")]
			[Range(0f, 1f)]
			public float subpixelAliasingRemovalAmount;

			// Token: 0x04002AB1 RID: 10929
			[Tooltip("The minimum amount of local contrast required to qualify a region as containing an edge.")]
			[Range(0.063f, 0.333f)]
			public float edgeDetectionThreshold;

			// Token: 0x04002AB2 RID: 10930
			[Tooltip("Local contrast adaptation value to disallow the algorithm from executing on the darker regions.")]
			[Range(0f, 0.0833f)]
			public float minimumRequiredLuminance;

			// Token: 0x04002AB3 RID: 10931
			public static AntialiasingModel.FxaaQualitySettings[] presets = new AntialiasingModel.FxaaQualitySettings[]
			{
				new AntialiasingModel.FxaaQualitySettings
				{
					subpixelAliasingRemovalAmount = 0f,
					edgeDetectionThreshold = 0.333f,
					minimumRequiredLuminance = 0.0833f
				},
				new AntialiasingModel.FxaaQualitySettings
				{
					subpixelAliasingRemovalAmount = 0.25f,
					edgeDetectionThreshold = 0.25f,
					minimumRequiredLuminance = 0.0833f
				},
				new AntialiasingModel.FxaaQualitySettings
				{
					subpixelAliasingRemovalAmount = 0.75f,
					edgeDetectionThreshold = 0.166f,
					minimumRequiredLuminance = 0.0833f
				},
				new AntialiasingModel.FxaaQualitySettings
				{
					subpixelAliasingRemovalAmount = 1f,
					edgeDetectionThreshold = 0.125f,
					minimumRequiredLuminance = 0.0625f
				},
				new AntialiasingModel.FxaaQualitySettings
				{
					subpixelAliasingRemovalAmount = 1f,
					edgeDetectionThreshold = 0.063f,
					minimumRequiredLuminance = 0.0312f
				}
			};
		}

		// Token: 0x02000466 RID: 1126
		[Serializable]
		public struct FxaaConsoleSettings
		{
			// Token: 0x04002AB4 RID: 10932
			[Tooltip("The amount of spread applied to the sampling coordinates while sampling for subpixel information.")]
			[Range(0.33f, 0.5f)]
			public float subpixelSpreadAmount;

			// Token: 0x04002AB5 RID: 10933
			[Tooltip("This value dictates how sharp the edges in the image are kept; a higher value implies sharper edges.")]
			[Range(2f, 8f)]
			public float edgeSharpnessAmount;

			// Token: 0x04002AB6 RID: 10934
			[Tooltip("The minimum amount of local contrast required to qualify a region as containing an edge.")]
			[Range(0.125f, 0.25f)]
			public float edgeDetectionThreshold;

			// Token: 0x04002AB7 RID: 10935
			[Tooltip("Local contrast adaptation value to disallow the algorithm from executing on the darker regions.")]
			[Range(0.04f, 0.06f)]
			public float minimumRequiredLuminance;

			// Token: 0x04002AB8 RID: 10936
			public static AntialiasingModel.FxaaConsoleSettings[] presets = new AntialiasingModel.FxaaConsoleSettings[]
			{
				new AntialiasingModel.FxaaConsoleSettings
				{
					subpixelSpreadAmount = 0.33f,
					edgeSharpnessAmount = 8f,
					edgeDetectionThreshold = 0.25f,
					minimumRequiredLuminance = 0.06f
				},
				new AntialiasingModel.FxaaConsoleSettings
				{
					subpixelSpreadAmount = 0.33f,
					edgeSharpnessAmount = 8f,
					edgeDetectionThreshold = 0.125f,
					minimumRequiredLuminance = 0.06f
				},
				new AntialiasingModel.FxaaConsoleSettings
				{
					subpixelSpreadAmount = 0.5f,
					edgeSharpnessAmount = 8f,
					edgeDetectionThreshold = 0.125f,
					minimumRequiredLuminance = 0.05f
				},
				new AntialiasingModel.FxaaConsoleSettings
				{
					subpixelSpreadAmount = 0.5f,
					edgeSharpnessAmount = 4f,
					edgeDetectionThreshold = 0.125f,
					minimumRequiredLuminance = 0.04f
				},
				new AntialiasingModel.FxaaConsoleSettings
				{
					subpixelSpreadAmount = 0.5f,
					edgeSharpnessAmount = 2f,
					edgeDetectionThreshold = 0.125f,
					minimumRequiredLuminance = 0.04f
				}
			};
		}

		// Token: 0x02000467 RID: 1127
		[Serializable]
		public struct FxaaSettings
		{
			// Token: 0x17000372 RID: 882
			// (get) Token: 0x06001A57 RID: 6743 RVA: 0x000F6208 File Offset: 0x000F4408
			public static AntialiasingModel.FxaaSettings defaultSettings
			{
				get
				{
					return new AntialiasingModel.FxaaSettings
					{
						preset = AntialiasingModel.FxaaPreset.Default
					};
				}
			}

			// Token: 0x04002AB9 RID: 10937
			public AntialiasingModel.FxaaPreset preset;
		}

		// Token: 0x02000468 RID: 1128
		[Serializable]
		public struct TaaSettings
		{
			// Token: 0x17000373 RID: 883
			// (get) Token: 0x06001A58 RID: 6744 RVA: 0x000F6228 File Offset: 0x000F4428
			public static AntialiasingModel.TaaSettings defaultSettings
			{
				get
				{
					return new AntialiasingModel.TaaSettings
					{
						jitterSpread = 0.75f,
						sharpen = 0.3f,
						stationaryBlending = 0.95f,
						motionBlending = 0.85f
					};
				}
			}

			// Token: 0x04002ABA RID: 10938
			[Tooltip("The diameter (in texels) inside which jitter samples are spread. Smaller values result in crisper but more aliased output, while larger values result in more stable but blurrier output.")]
			[Range(0.1f, 1f)]
			public float jitterSpread;

			// Token: 0x04002ABB RID: 10939
			[Tooltip("Controls the amount of sharpening applied to the color buffer.")]
			[Range(0f, 3f)]
			public float sharpen;

			// Token: 0x04002ABC RID: 10940
			[Tooltip("The blend coefficient for a stationary fragment. Controls the percentage of history sample blended into the final color.")]
			[Range(0f, 0.99f)]
			public float stationaryBlending;

			// Token: 0x04002ABD RID: 10941
			[Tooltip("The blend coefficient for a fragment with significant motion. Controls the percentage of history sample blended into the final color.")]
			[Range(0f, 0.99f)]
			public float motionBlending;
		}

		// Token: 0x02000469 RID: 1129
		[Serializable]
		public struct Settings
		{
			// Token: 0x17000374 RID: 884
			// (get) Token: 0x06001A59 RID: 6745 RVA: 0x000F6270 File Offset: 0x000F4470
			public static AntialiasingModel.Settings defaultSettings
			{
				get
				{
					return new AntialiasingModel.Settings
					{
						method = AntialiasingModel.Method.Fxaa,
						fxaaSettings = AntialiasingModel.FxaaSettings.defaultSettings,
						taaSettings = AntialiasingModel.TaaSettings.defaultSettings
					};
				}
			}

			// Token: 0x04002ABE RID: 10942
			public AntialiasingModel.Method method;

			// Token: 0x04002ABF RID: 10943
			public AntialiasingModel.FxaaSettings fxaaSettings;

			// Token: 0x04002AC0 RID: 10944
			public AntialiasingModel.TaaSettings taaSettings;
		}
	}
}
