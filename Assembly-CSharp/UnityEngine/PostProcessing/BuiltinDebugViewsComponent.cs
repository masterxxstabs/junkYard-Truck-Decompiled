using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing
{
	// Token: 0x020001C5 RID: 453
	public sealed class BuiltinDebugViewsComponent : PostProcessingComponentCommandBuffer<BuiltinDebugViewsModel>
	{
		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000ADA RID: 2778 RVA: 0x00090250 File Offset: 0x0008E450
		public override bool active
		{
			get
			{
				return base.model.IsModeActive(BuiltinDebugViewsModel.Mode.Depth) || base.model.IsModeActive(BuiltinDebugViewsModel.Mode.Normals) || base.model.IsModeActive(BuiltinDebugViewsModel.Mode.MotionVectors);
			}
		}

		// Token: 0x06000ADB RID: 2779 RVA: 0x0009027C File Offset: 0x0008E47C
		public override DepthTextureMode GetCameraFlags()
		{
			BuiltinDebugViewsModel.Mode mode = base.model.settings.mode;
			DepthTextureMode depthTextureMode = DepthTextureMode.None;
			switch (mode)
			{
			case BuiltinDebugViewsModel.Mode.Depth:
				depthTextureMode |= DepthTextureMode.Depth;
				break;
			case BuiltinDebugViewsModel.Mode.Normals:
				depthTextureMode |= DepthTextureMode.DepthNormals;
				break;
			case BuiltinDebugViewsModel.Mode.MotionVectors:
				depthTextureMode |= (DepthTextureMode.Depth | DepthTextureMode.MotionVectors);
				break;
			}
			return depthTextureMode;
		}

		// Token: 0x06000ADC RID: 2780 RVA: 0x000902C3 File Offset: 0x0008E4C3
		public override CameraEvent GetCameraEvent()
		{
			if (base.model.settings.mode != BuiltinDebugViewsModel.Mode.MotionVectors)
			{
				return CameraEvent.BeforeImageEffectsOpaque;
			}
			return CameraEvent.BeforeImageEffects;
		}

		// Token: 0x06000ADD RID: 2781 RVA: 0x000902DD File Offset: 0x0008E4DD
		public override string GetName()
		{
			return "Builtin Debug Views";
		}

		// Token: 0x06000ADE RID: 2782 RVA: 0x000902E4 File Offset: 0x0008E4E4
		public override void PopulateCommandBuffer(CommandBuffer cb)
		{
			ref BuiltinDebugViewsModel.Settings settings = base.model.settings;
			Material material = this.context.materialFactory.Get("Hidden/Post FX/Builtin Debug Views");
			material.shaderKeywords = null;
			if (this.context.isGBufferAvailable)
			{
				material.EnableKeyword("SOURCE_GBUFFER");
			}
			switch (settings.mode)
			{
			case BuiltinDebugViewsModel.Mode.Depth:
				this.DepthPass(cb);
				break;
			case BuiltinDebugViewsModel.Mode.Normals:
				this.DepthNormalsPass(cb);
				break;
			case BuiltinDebugViewsModel.Mode.MotionVectors:
				this.MotionVectorsPass(cb);
				break;
			}
			this.context.Interrupt();
		}

		// Token: 0x06000ADF RID: 2783 RVA: 0x00090374 File Offset: 0x0008E574
		private void DepthPass(CommandBuffer cb)
		{
			Material mat = this.context.materialFactory.Get("Hidden/Post FX/Builtin Debug Views");
			BuiltinDebugViewsModel.DepthSettings depth = base.model.settings.depth;
			cb.SetGlobalFloat(BuiltinDebugViewsComponent.Uniforms._DepthScale, 1f / depth.scale);
			cb.Blit(null, BuiltinRenderTextureType.CameraTarget, mat, 0);
		}

		// Token: 0x06000AE0 RID: 2784 RVA: 0x000903D0 File Offset: 0x0008E5D0
		private void DepthNormalsPass(CommandBuffer cb)
		{
			Material mat = this.context.materialFactory.Get("Hidden/Post FX/Builtin Debug Views");
			cb.Blit(null, BuiltinRenderTextureType.CameraTarget, mat, 1);
		}

		// Token: 0x06000AE1 RID: 2785 RVA: 0x00090404 File Offset: 0x0008E604
		private void MotionVectorsPass(CommandBuffer cb)
		{
			Material material = this.context.materialFactory.Get("Hidden/Post FX/Builtin Debug Views");
			BuiltinDebugViewsModel.MotionVectorsSettings motionVectors = base.model.settings.motionVectors;
			int nameID = BuiltinDebugViewsComponent.Uniforms._TempRT;
			cb.GetTemporaryRT(nameID, this.context.width, this.context.height, 0, FilterMode.Bilinear);
			cb.SetGlobalFloat(BuiltinDebugViewsComponent.Uniforms._Opacity, motionVectors.sourceOpacity);
			cb.SetGlobalTexture(BuiltinDebugViewsComponent.Uniforms._MainTex, BuiltinRenderTextureType.CameraTarget);
			cb.Blit(BuiltinRenderTextureType.CameraTarget, nameID, material, 2);
			if (motionVectors.motionImageOpacity > 0f && motionVectors.motionImageAmplitude > 0f)
			{
				int tempRT = BuiltinDebugViewsComponent.Uniforms._TempRT2;
				cb.GetTemporaryRT(tempRT, this.context.width, this.context.height, 0, FilterMode.Bilinear);
				cb.SetGlobalFloat(BuiltinDebugViewsComponent.Uniforms._Opacity, motionVectors.motionImageOpacity);
				cb.SetGlobalFloat(BuiltinDebugViewsComponent.Uniforms._Amplitude, motionVectors.motionImageAmplitude);
				cb.SetGlobalTexture(BuiltinDebugViewsComponent.Uniforms._MainTex, nameID);
				cb.Blit(nameID, tempRT, material, 3);
				cb.ReleaseTemporaryRT(nameID);
				nameID = tempRT;
			}
			if (motionVectors.motionVectorsOpacity > 0f && motionVectors.motionVectorsAmplitude > 0f)
			{
				this.PrepareArrows();
				float num = 1f / (float)motionVectors.motionVectorsResolution;
				float x = num * (float)this.context.height / (float)this.context.width;
				cb.SetGlobalVector(BuiltinDebugViewsComponent.Uniforms._Scale, new Vector2(x, num));
				cb.SetGlobalFloat(BuiltinDebugViewsComponent.Uniforms._Opacity, motionVectors.motionVectorsOpacity);
				cb.SetGlobalFloat(BuiltinDebugViewsComponent.Uniforms._Amplitude, motionVectors.motionVectorsAmplitude);
				cb.DrawMesh(this.m_Arrows.mesh, Matrix4x4.identity, material, 0, 4);
			}
			cb.SetGlobalTexture(BuiltinDebugViewsComponent.Uniforms._MainTex, nameID);
			cb.Blit(nameID, BuiltinRenderTextureType.CameraTarget);
			cb.ReleaseTemporaryRT(nameID);
		}

		// Token: 0x06000AE2 RID: 2786 RVA: 0x000905F8 File Offset: 0x0008E7F8
		private void PrepareArrows()
		{
			int motionVectorsResolution = base.model.settings.motionVectors.motionVectorsResolution;
			int num = motionVectorsResolution * Screen.width / Screen.height;
			if (this.m_Arrows == null)
			{
				this.m_Arrows = new BuiltinDebugViewsComponent.ArrowArray();
			}
			if (this.m_Arrows.columnCount != num || this.m_Arrows.rowCount != motionVectorsResolution)
			{
				this.m_Arrows.Release();
				this.m_Arrows.BuildMesh(num, motionVectorsResolution);
			}
		}

		// Token: 0x06000AE3 RID: 2787 RVA: 0x00090670 File Offset: 0x0008E870
		public override void OnDisable()
		{
			if (this.m_Arrows != null)
			{
				this.m_Arrows.Release();
			}
			this.m_Arrows = null;
		}

		// Token: 0x04001D43 RID: 7491
		private const string k_ShaderString = "Hidden/Post FX/Builtin Debug Views";

		// Token: 0x04001D44 RID: 7492
		private BuiltinDebugViewsComponent.ArrowArray m_Arrows;

		// Token: 0x0200044D RID: 1101
		private static class Uniforms
		{
			// Token: 0x040029F4 RID: 10740
			internal static readonly int _DepthScale = Shader.PropertyToID("_DepthScale");

			// Token: 0x040029F5 RID: 10741
			internal static readonly int _TempRT = Shader.PropertyToID("_TempRT");

			// Token: 0x040029F6 RID: 10742
			internal static readonly int _Opacity = Shader.PropertyToID("_Opacity");

			// Token: 0x040029F7 RID: 10743
			internal static readonly int _MainTex = Shader.PropertyToID("_MainTex");

			// Token: 0x040029F8 RID: 10744
			internal static readonly int _TempRT2 = Shader.PropertyToID("_TempRT2");

			// Token: 0x040029F9 RID: 10745
			internal static readonly int _Amplitude = Shader.PropertyToID("_Amplitude");

			// Token: 0x040029FA RID: 10746
			internal static readonly int _Scale = Shader.PropertyToID("_Scale");
		}

		// Token: 0x0200044E RID: 1102
		private enum Pass
		{
			// Token: 0x040029FC RID: 10748
			Depth,
			// Token: 0x040029FD RID: 10749
			Normals,
			// Token: 0x040029FE RID: 10750
			MovecOpacity,
			// Token: 0x040029FF RID: 10751
			MovecImaging,
			// Token: 0x04002A00 RID: 10752
			MovecArrows
		}

		// Token: 0x0200044F RID: 1103
		private class ArrowArray
		{
			// Token: 0x1700036E RID: 878
			// (get) Token: 0x06001A33 RID: 6707 RVA: 0x000F4FAE File Offset: 0x000F31AE
			// (set) Token: 0x06001A34 RID: 6708 RVA: 0x000F4FB6 File Offset: 0x000F31B6
			public Mesh mesh { get; private set; }

			// Token: 0x1700036F RID: 879
			// (get) Token: 0x06001A35 RID: 6709 RVA: 0x000F4FBF File Offset: 0x000F31BF
			// (set) Token: 0x06001A36 RID: 6710 RVA: 0x000F4FC7 File Offset: 0x000F31C7
			public int columnCount { get; private set; }

			// Token: 0x17000370 RID: 880
			// (get) Token: 0x06001A37 RID: 6711 RVA: 0x000F4FD0 File Offset: 0x000F31D0
			// (set) Token: 0x06001A38 RID: 6712 RVA: 0x000F4FD8 File Offset: 0x000F31D8
			public int rowCount { get; private set; }

			// Token: 0x06001A39 RID: 6713 RVA: 0x000F4FE4 File Offset: 0x000F31E4
			public void BuildMesh(int columns, int rows)
			{
				Vector3[] array = new Vector3[]
				{
					new Vector3(0f, 0f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(-1f, 1f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(1f, 1f, 0f)
				};
				int num = 6 * columns * rows;
				List<Vector3> list = new List<Vector3>(num);
				List<Vector2> list2 = new List<Vector2>(num);
				for (int i = 0; i < rows; i++)
				{
					for (int j = 0; j < columns; j++)
					{
						Vector2 item = new Vector2((0.5f + (float)j) / (float)columns, (0.5f + (float)i) / (float)rows);
						for (int k = 0; k < 6; k++)
						{
							list.Add(array[k]);
							list2.Add(item);
						}
					}
				}
				int[] array2 = new int[num];
				for (int l = 0; l < num; l++)
				{
					array2[l] = l;
				}
				this.mesh = new Mesh
				{
					hideFlags = HideFlags.DontSave
				};
				this.mesh.SetVertices(list);
				this.mesh.SetUVs(0, list2);
				this.mesh.SetIndices(array2, MeshTopology.Lines, 0);
				this.mesh.UploadMeshData(true);
				this.columnCount = columns;
				this.rowCount = rows;
			}

			// Token: 0x06001A3A RID: 6714 RVA: 0x000F5187 File Offset: 0x000F3387
			public void Release()
			{
				GraphicsUtils.Destroy(this.mesh);
				this.mesh = null;
			}
		}
	}
}
