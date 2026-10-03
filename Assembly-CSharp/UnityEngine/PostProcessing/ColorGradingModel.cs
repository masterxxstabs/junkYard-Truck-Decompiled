using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001D8 RID: 472
	[Serializable]
	public class ColorGradingModel : PostProcessingModel
	{
		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000B5F RID: 2911 RVA: 0x00093A30 File Offset: 0x00091C30
		// (set) Token: 0x06000B60 RID: 2912 RVA: 0x00093A38 File Offset: 0x00091C38
		public ColorGradingModel.Settings settings
		{
			get
			{
				return this.m_Settings;
			}
			set
			{
				this.m_Settings = value;
				this.OnValidate();
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000B61 RID: 2913 RVA: 0x00093A47 File Offset: 0x00091C47
		// (set) Token: 0x06000B62 RID: 2914 RVA: 0x00093A4F File Offset: 0x00091C4F
		public bool isDirty { get; internal set; }

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000B63 RID: 2915 RVA: 0x00093A58 File Offset: 0x00091C58
		// (set) Token: 0x06000B64 RID: 2916 RVA: 0x00093A60 File Offset: 0x00091C60
		public RenderTexture bakedLut { get; internal set; }

		// Token: 0x06000B65 RID: 2917 RVA: 0x00093A69 File Offset: 0x00091C69
		public override void Reset()
		{
			this.m_Settings = ColorGradingModel.Settings.defaultSettings;
			this.OnValidate();
		}

		// Token: 0x06000B66 RID: 2918 RVA: 0x00093A7C File Offset: 0x00091C7C
		public override void OnValidate()
		{
			this.isDirty = true;
		}

		// Token: 0x04001D72 RID: 7538
		[SerializeField]
		private ColorGradingModel.Settings m_Settings = ColorGradingModel.Settings.defaultSettings;

		// Token: 0x02000472 RID: 1138
		public enum Tonemapper
		{
			// Token: 0x04002AE2 RID: 10978
			None,
			// Token: 0x04002AE3 RID: 10979
			ACES,
			// Token: 0x04002AE4 RID: 10980
			Neutral
		}

		// Token: 0x02000473 RID: 1139
		[Serializable]
		public struct TonemappingSettings
		{
			// Token: 0x1700037D RID: 893
			// (get) Token: 0x06001A63 RID: 6755 RVA: 0x000F6454 File Offset: 0x000F4654
			public static ColorGradingModel.TonemappingSettings defaultSettings
			{
				get
				{
					return new ColorGradingModel.TonemappingSettings
					{
						tonemapper = ColorGradingModel.Tonemapper.Neutral,
						neutralBlackIn = 0.02f,
						neutralWhiteIn = 10f,
						neutralBlackOut = 0f,
						neutralWhiteOut = 10f,
						neutralWhiteLevel = 5.3f,
						neutralWhiteClip = 10f
					};
				}
			}

			// Token: 0x04002AE5 RID: 10981
			[Tooltip("Tonemapping algorithm to use at the end of the color grading process. Use \"Neutral\" if you need a customizable tonemapper or \"Filmic\" to give a standard filmic look to your scenes.")]
			public ColorGradingModel.Tonemapper tonemapper;

			// Token: 0x04002AE6 RID: 10982
			[Range(-0.1f, 0.1f)]
			public float neutralBlackIn;

			// Token: 0x04002AE7 RID: 10983
			[Range(1f, 20f)]
			public float neutralWhiteIn;

			// Token: 0x04002AE8 RID: 10984
			[Range(-0.09f, 0.1f)]
			public float neutralBlackOut;

			// Token: 0x04002AE9 RID: 10985
			[Range(1f, 19f)]
			public float neutralWhiteOut;

			// Token: 0x04002AEA RID: 10986
			[Range(0.1f, 20f)]
			public float neutralWhiteLevel;

			// Token: 0x04002AEB RID: 10987
			[Range(1f, 10f)]
			public float neutralWhiteClip;
		}

		// Token: 0x02000474 RID: 1140
		[Serializable]
		public struct BasicSettings
		{
			// Token: 0x1700037E RID: 894
			// (get) Token: 0x06001A64 RID: 6756 RVA: 0x000F64BC File Offset: 0x000F46BC
			public static ColorGradingModel.BasicSettings defaultSettings
			{
				get
				{
					return new ColorGradingModel.BasicSettings
					{
						postExposure = 0f,
						temperature = 0f,
						tint = 0f,
						hueShift = 0f,
						saturation = 1f,
						contrast = 1f
					};
				}
			}

			// Token: 0x04002AEC RID: 10988
			[Tooltip("Adjusts the overall exposure of the scene in EV units. This is applied after HDR effect and right before tonemapping so it won't affect previous effects in the chain.")]
			public float postExposure;

			// Token: 0x04002AED RID: 10989
			[Range(-100f, 100f)]
			[Tooltip("Sets the white balance to a custom color temperature.")]
			public float temperature;

			// Token: 0x04002AEE RID: 10990
			[Range(-100f, 100f)]
			[Tooltip("Sets the white balance to compensate for a green or magenta tint.")]
			public float tint;

			// Token: 0x04002AEF RID: 10991
			[Range(-180f, 180f)]
			[Tooltip("Shift the hue of all colors.")]
			public float hueShift;

			// Token: 0x04002AF0 RID: 10992
			[Range(0f, 2f)]
			[Tooltip("Pushes the intensity of all colors.")]
			public float saturation;

			// Token: 0x04002AF1 RID: 10993
			[Range(0f, 2f)]
			[Tooltip("Expands or shrinks the overall range of tonal values.")]
			public float contrast;
		}

		// Token: 0x02000475 RID: 1141
		[Serializable]
		public struct ChannelMixerSettings
		{
			// Token: 0x1700037F RID: 895
			// (get) Token: 0x06001A65 RID: 6757 RVA: 0x000F651C File Offset: 0x000F471C
			public static ColorGradingModel.ChannelMixerSettings defaultSettings
			{
				get
				{
					return new ColorGradingModel.ChannelMixerSettings
					{
						red = new Vector3(1f, 0f, 0f),
						green = new Vector3(0f, 1f, 0f),
						blue = new Vector3(0f, 0f, 1f),
						currentEditingChannel = 0
					};
				}
			}

			// Token: 0x04002AF2 RID: 10994
			public Vector3 red;

			// Token: 0x04002AF3 RID: 10995
			public Vector3 green;

			// Token: 0x04002AF4 RID: 10996
			public Vector3 blue;

			// Token: 0x04002AF5 RID: 10997
			[HideInInspector]
			public int currentEditingChannel;
		}

		// Token: 0x02000476 RID: 1142
		[Serializable]
		public struct LogWheelsSettings
		{
			// Token: 0x17000380 RID: 896
			// (get) Token: 0x06001A66 RID: 6758 RVA: 0x000F658C File Offset: 0x000F478C
			public static ColorGradingModel.LogWheelsSettings defaultSettings
			{
				get
				{
					return new ColorGradingModel.LogWheelsSettings
					{
						slope = Color.clear,
						power = Color.clear,
						offset = Color.clear
					};
				}
			}

			// Token: 0x04002AF6 RID: 10998
			[Trackball("GetSlopeValue")]
			public Color slope;

			// Token: 0x04002AF7 RID: 10999
			[Trackball("GetPowerValue")]
			public Color power;

			// Token: 0x04002AF8 RID: 11000
			[Trackball("GetOffsetValue")]
			public Color offset;
		}

		// Token: 0x02000477 RID: 1143
		[Serializable]
		public struct LinearWheelsSettings
		{
			// Token: 0x17000381 RID: 897
			// (get) Token: 0x06001A67 RID: 6759 RVA: 0x000F65C8 File Offset: 0x000F47C8
			public static ColorGradingModel.LinearWheelsSettings defaultSettings
			{
				get
				{
					return new ColorGradingModel.LinearWheelsSettings
					{
						lift = Color.clear,
						gamma = Color.clear,
						gain = Color.clear
					};
				}
			}

			// Token: 0x04002AF9 RID: 11001
			[Trackball("GetLiftValue")]
			public Color lift;

			// Token: 0x04002AFA RID: 11002
			[Trackball("GetGammaValue")]
			public Color gamma;

			// Token: 0x04002AFB RID: 11003
			[Trackball("GetGainValue")]
			public Color gain;
		}

		// Token: 0x02000478 RID: 1144
		public enum ColorWheelMode
		{
			// Token: 0x04002AFD RID: 11005
			Linear,
			// Token: 0x04002AFE RID: 11006
			Log
		}

		// Token: 0x02000479 RID: 1145
		[Serializable]
		public struct ColorWheelsSettings
		{
			// Token: 0x17000382 RID: 898
			// (get) Token: 0x06001A68 RID: 6760 RVA: 0x000F6604 File Offset: 0x000F4804
			public static ColorGradingModel.ColorWheelsSettings defaultSettings
			{
				get
				{
					return new ColorGradingModel.ColorWheelsSettings
					{
						mode = ColorGradingModel.ColorWheelMode.Log,
						log = ColorGradingModel.LogWheelsSettings.defaultSettings,
						linear = ColorGradingModel.LinearWheelsSettings.defaultSettings
					};
				}
			}

			// Token: 0x04002AFF RID: 11007
			public ColorGradingModel.ColorWheelMode mode;

			// Token: 0x04002B00 RID: 11008
			[TrackballGroup]
			public ColorGradingModel.LogWheelsSettings log;

			// Token: 0x04002B01 RID: 11009
			[TrackballGroup]
			public ColorGradingModel.LinearWheelsSettings linear;
		}

		// Token: 0x0200047A RID: 1146
		[Serializable]
		public struct CurvesSettings
		{
			// Token: 0x17000383 RID: 899
			// (get) Token: 0x06001A69 RID: 6761 RVA: 0x000F663C File Offset: 0x000F483C
			public static ColorGradingModel.CurvesSettings defaultSettings
			{
				get
				{
					return new ColorGradingModel.CurvesSettings
					{
						master = new ColorGradingCurve(new AnimationCurve(new Keyframe[]
						{
							new Keyframe(0f, 0f, 1f, 1f),
							new Keyframe(1f, 1f, 1f, 1f)
						}), 0f, false, new Vector2(0f, 1f)),
						red = new ColorGradingCurve(new AnimationCurve(new Keyframe[]
						{
							new Keyframe(0f, 0f, 1f, 1f),
							new Keyframe(1f, 1f, 1f, 1f)
						}), 0f, false, new Vector2(0f, 1f)),
						green = new ColorGradingCurve(new AnimationCurve(new Keyframe[]
						{
							new Keyframe(0f, 0f, 1f, 1f),
							new Keyframe(1f, 1f, 1f, 1f)
						}), 0f, false, new Vector2(0f, 1f)),
						blue = new ColorGradingCurve(new AnimationCurve(new Keyframe[]
						{
							new Keyframe(0f, 0f, 1f, 1f),
							new Keyframe(1f, 1f, 1f, 1f)
						}), 0f, false, new Vector2(0f, 1f)),
						hueVShue = new ColorGradingCurve(new AnimationCurve(), 0.5f, true, new Vector2(0f, 1f)),
						hueVSsat = new ColorGradingCurve(new AnimationCurve(), 0.5f, true, new Vector2(0f, 1f)),
						satVSsat = new ColorGradingCurve(new AnimationCurve(), 0.5f, false, new Vector2(0f, 1f)),
						lumVSsat = new ColorGradingCurve(new AnimationCurve(), 0.5f, false, new Vector2(0f, 1f)),
						e_CurrentEditingCurve = 0,
						e_CurveY = true,
						e_CurveR = false,
						e_CurveG = false,
						e_CurveB = false
					};
				}
			}

			// Token: 0x04002B02 RID: 11010
			public ColorGradingCurve master;

			// Token: 0x04002B03 RID: 11011
			public ColorGradingCurve red;

			// Token: 0x04002B04 RID: 11012
			public ColorGradingCurve green;

			// Token: 0x04002B05 RID: 11013
			public ColorGradingCurve blue;

			// Token: 0x04002B06 RID: 11014
			public ColorGradingCurve hueVShue;

			// Token: 0x04002B07 RID: 11015
			public ColorGradingCurve hueVSsat;

			// Token: 0x04002B08 RID: 11016
			public ColorGradingCurve satVSsat;

			// Token: 0x04002B09 RID: 11017
			public ColorGradingCurve lumVSsat;

			// Token: 0x04002B0A RID: 11018
			[HideInInspector]
			public int e_CurrentEditingCurve;

			// Token: 0x04002B0B RID: 11019
			[HideInInspector]
			public bool e_CurveY;

			// Token: 0x04002B0C RID: 11020
			[HideInInspector]
			public bool e_CurveR;

			// Token: 0x04002B0D RID: 11021
			[HideInInspector]
			public bool e_CurveG;

			// Token: 0x04002B0E RID: 11022
			[HideInInspector]
			public bool e_CurveB;
		}

		// Token: 0x0200047B RID: 1147
		[Serializable]
		public struct Settings
		{
			// Token: 0x17000384 RID: 900
			// (get) Token: 0x06001A6A RID: 6762 RVA: 0x000F68C4 File Offset: 0x000F4AC4
			public static ColorGradingModel.Settings defaultSettings
			{
				get
				{
					return new ColorGradingModel.Settings
					{
						tonemapping = ColorGradingModel.TonemappingSettings.defaultSettings,
						basic = ColorGradingModel.BasicSettings.defaultSettings,
						channelMixer = ColorGradingModel.ChannelMixerSettings.defaultSettings,
						colorWheels = ColorGradingModel.ColorWheelsSettings.defaultSettings,
						curves = ColorGradingModel.CurvesSettings.defaultSettings
					};
				}
			}

			// Token: 0x04002B0F RID: 11023
			public ColorGradingModel.TonemappingSettings tonemapping;

			// Token: 0x04002B10 RID: 11024
			public ColorGradingModel.BasicSettings basic;

			// Token: 0x04002B11 RID: 11025
			public ColorGradingModel.ChannelMixerSettings channelMixer;

			// Token: 0x04002B12 RID: 11026
			public ColorGradingModel.ColorWheelsSettings colorWheels;

			// Token: 0x04002B13 RID: 11027
			public ColorGradingModel.CurvesSettings curves;
		}
	}
}
