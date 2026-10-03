using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001D6 RID: 470
	[Serializable]
	public class BuiltinDebugViewsModel : PostProcessingModel
	{
		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000B55 RID: 2901 RVA: 0x0009398B File Offset: 0x00091B8B
		// (set) Token: 0x06000B56 RID: 2902 RVA: 0x00093993 File Offset: 0x00091B93
		public BuiltinDebugViewsModel.Settings settings
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

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000B57 RID: 2903 RVA: 0x0009399C File Offset: 0x00091B9C
		public bool willInterrupt
		{
			get
			{
				return !this.IsModeActive(BuiltinDebugViewsModel.Mode.None) && !this.IsModeActive(BuiltinDebugViewsModel.Mode.EyeAdaptation) && !this.IsModeActive(BuiltinDebugViewsModel.Mode.PreGradingLog) && !this.IsModeActive(BuiltinDebugViewsModel.Mode.LogLut) && !this.IsModeActive(BuiltinDebugViewsModel.Mode.UserLut);
			}
		}

		// Token: 0x06000B58 RID: 2904 RVA: 0x000939CF File Offset: 0x00091BCF
		public override void Reset()
		{
			this.settings = BuiltinDebugViewsModel.Settings.defaultSettings;
		}

		// Token: 0x06000B59 RID: 2905 RVA: 0x000939DC File Offset: 0x00091BDC
		public bool IsModeActive(BuiltinDebugViewsModel.Mode mode)
		{
			return this.m_Settings.mode == mode;
		}

		// Token: 0x04001D70 RID: 7536
		[SerializeField]
		private BuiltinDebugViewsModel.Settings m_Settings = BuiltinDebugViewsModel.Settings.defaultSettings;

		// Token: 0x0200046D RID: 1133
		[Serializable]
		public struct DepthSettings
		{
			// Token: 0x17000379 RID: 889
			// (get) Token: 0x06001A5F RID: 6751 RVA: 0x000F6370 File Offset: 0x000F4570
			public static BuiltinDebugViewsModel.DepthSettings defaultSettings
			{
				get
				{
					return new BuiltinDebugViewsModel.DepthSettings
					{
						scale = 1f
					};
				}
			}

			// Token: 0x04002ACA RID: 10954
			[Range(0f, 1f)]
			[Tooltip("Scales the camera far plane before displaying the depth map.")]
			public float scale;
		}

		// Token: 0x0200046E RID: 1134
		[Serializable]
		public struct MotionVectorsSettings
		{
			// Token: 0x1700037A RID: 890
			// (get) Token: 0x06001A60 RID: 6752 RVA: 0x000F6394 File Offset: 0x000F4594
			public static BuiltinDebugViewsModel.MotionVectorsSettings defaultSettings
			{
				get
				{
					return new BuiltinDebugViewsModel.MotionVectorsSettings
					{
						sourceOpacity = 1f,
						motionImageOpacity = 0f,
						motionImageAmplitude = 16f,
						motionVectorsOpacity = 1f,
						motionVectorsResolution = 24,
						motionVectorsAmplitude = 64f
					};
				}
			}

			// Token: 0x04002ACB RID: 10955
			[Range(0f, 1f)]
			[Tooltip("Opacity of the source render.")]
			public float sourceOpacity;

			// Token: 0x04002ACC RID: 10956
			[Range(0f, 1f)]
			[Tooltip("Opacity of the per-pixel motion vector colors.")]
			public float motionImageOpacity;

			// Token: 0x04002ACD RID: 10957
			[Min(0f)]
			[Tooltip("Because motion vectors are mainly very small vectors, you can use this setting to make them more visible.")]
			public float motionImageAmplitude;

			// Token: 0x04002ACE RID: 10958
			[Range(0f, 1f)]
			[Tooltip("Opacity for the motion vector arrows.")]
			public float motionVectorsOpacity;

			// Token: 0x04002ACF RID: 10959
			[Range(8f, 64f)]
			[Tooltip("The arrow density on screen.")]
			public int motionVectorsResolution;

			// Token: 0x04002AD0 RID: 10960
			[Min(0f)]
			[Tooltip("Tweaks the arrows length.")]
			public float motionVectorsAmplitude;
		}

		// Token: 0x0200046F RID: 1135
		public enum Mode
		{
			// Token: 0x04002AD2 RID: 10962
			None,
			// Token: 0x04002AD3 RID: 10963
			Depth,
			// Token: 0x04002AD4 RID: 10964
			Normals,
			// Token: 0x04002AD5 RID: 10965
			MotionVectors,
			// Token: 0x04002AD6 RID: 10966
			AmbientOcclusion,
			// Token: 0x04002AD7 RID: 10967
			EyeAdaptation,
			// Token: 0x04002AD8 RID: 10968
			FocusPlane,
			// Token: 0x04002AD9 RID: 10969
			PreGradingLog,
			// Token: 0x04002ADA RID: 10970
			LogLut,
			// Token: 0x04002ADB RID: 10971
			UserLut
		}

		// Token: 0x02000470 RID: 1136
		[Serializable]
		public struct Settings
		{
			// Token: 0x1700037B RID: 891
			// (get) Token: 0x06001A61 RID: 6753 RVA: 0x000F63F0 File Offset: 0x000F45F0
			public static BuiltinDebugViewsModel.Settings defaultSettings
			{
				get
				{
					return new BuiltinDebugViewsModel.Settings
					{
						mode = BuiltinDebugViewsModel.Mode.None,
						depth = BuiltinDebugViewsModel.DepthSettings.defaultSettings,
						motionVectors = BuiltinDebugViewsModel.MotionVectorsSettings.defaultSettings
					};
				}
			}

			// Token: 0x04002ADC RID: 10972
			public BuiltinDebugViewsModel.Mode mode;

			// Token: 0x04002ADD RID: 10973
			public BuiltinDebugViewsModel.DepthSettings depth;

			// Token: 0x04002ADE RID: 10974
			public BuiltinDebugViewsModel.MotionVectorsSettings motionVectors;
		}
	}
}
