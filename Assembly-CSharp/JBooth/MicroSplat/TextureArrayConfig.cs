using System;
using System.Collections.Generic;
using UnityEngine;

namespace JBooth.MicroSplat
{
	// Token: 0x020002CF RID: 719
	[CreateAssetMenu(menuName = "MicroSplat/Texture Array Config", order = 1)]
	[ExecuteInEditMode]
	public class TextureArrayConfig : ScriptableObject
	{
		// Token: 0x06001366 RID: 4966 RVA: 0x000116EA File Offset: 0x0000F8EA
		public bool IsScatter()
		{
			return false;
		}

		// Token: 0x06001367 RID: 4967 RVA: 0x000CB7C8 File Offset: 0x000C99C8
		public bool IsDecal()
		{
			return this.textureMode == TextureArrayConfig.TextureMode.Decal;
		}

		// Token: 0x06001368 RID: 4968 RVA: 0x000CB7D3 File Offset: 0x000C99D3
		public bool IsDecalSplat()
		{
			return this.textureMode == TextureArrayConfig.TextureMode.DecalSplatMap;
		}

		// Token: 0x06001369 RID: 4969 RVA: 0x000CB7DE File Offset: 0x000C99DE
		private void Awake()
		{
			TextureArrayConfig.sAllConfigs.Add(this);
		}

		// Token: 0x0600136A RID: 4970 RVA: 0x000CB7EB File Offset: 0x000C99EB
		private void OnDestroy()
		{
			TextureArrayConfig.sAllConfigs.Remove(this);
		}

		// Token: 0x0600136B RID: 4971 RVA: 0x000CB7FC File Offset: 0x000C99FC
		public static TextureArrayConfig FindConfig(Texture2DArray diffuse)
		{
			for (int i = 0; i < TextureArrayConfig.sAllConfigs.Count; i++)
			{
				if (TextureArrayConfig.sAllConfigs[i].diffuseArray == diffuse)
				{
					return TextureArrayConfig.sAllConfigs[i];
				}
			}
			return null;
		}

		// Token: 0x040023E4 RID: 9188
		public bool diffuseIsLinear;

		// Token: 0x040023E5 RID: 9189
		[HideInInspector]
		public bool antiTileArray;

		// Token: 0x040023E6 RID: 9190
		[HideInInspector]
		public bool emisMetalArray;

		// Token: 0x040023E7 RID: 9191
		public bool traxArray;

		// Token: 0x040023E8 RID: 9192
		[HideInInspector]
		public TextureArrayConfig.TextureMode textureMode = TextureArrayConfig.TextureMode.PBR;

		// Token: 0x040023E9 RID: 9193
		[HideInInspector]
		public TextureArrayConfig.ClusterMode clusterMode;

		// Token: 0x040023EA RID: 9194
		[HideInInspector]
		public TextureArrayConfig.PackingMode packingMode;

		// Token: 0x040023EB RID: 9195
		[HideInInspector]
		public TextureArrayConfig.PBRWorkflow pbrWorkflow;

		// Token: 0x040023EC RID: 9196
		[HideInInspector]
		public int hash;

		// Token: 0x040023ED RID: 9197
		private static List<TextureArrayConfig> sAllConfigs = new List<TextureArrayConfig>();

		// Token: 0x040023EE RID: 9198
		[HideInInspector]
		public Texture2DArray splatArray;

		// Token: 0x040023EF RID: 9199
		[HideInInspector]
		public Texture2DArray diffuseArray;

		// Token: 0x040023F0 RID: 9200
		[HideInInspector]
		public Texture2DArray normalSAOArray;

		// Token: 0x040023F1 RID: 9201
		[HideInInspector]
		public Texture2DArray smoothAOArray;

		// Token: 0x040023F2 RID: 9202
		[HideInInspector]
		public Texture2DArray specularArray;

		// Token: 0x040023F3 RID: 9203
		[HideInInspector]
		public Texture2DArray diffuseArray2;

		// Token: 0x040023F4 RID: 9204
		[HideInInspector]
		public Texture2DArray normalSAOArray2;

		// Token: 0x040023F5 RID: 9205
		[HideInInspector]
		public Texture2DArray smoothAOArray2;

		// Token: 0x040023F6 RID: 9206
		[HideInInspector]
		public Texture2DArray specularArray2;

		// Token: 0x040023F7 RID: 9207
		[HideInInspector]
		public Texture2DArray diffuseArray3;

		// Token: 0x040023F8 RID: 9208
		[HideInInspector]
		public Texture2DArray normalSAOArray3;

		// Token: 0x040023F9 RID: 9209
		[HideInInspector]
		public Texture2DArray smoothAOArray3;

		// Token: 0x040023FA RID: 9210
		[HideInInspector]
		public Texture2DArray specularArray3;

		// Token: 0x040023FB RID: 9211
		[HideInInspector]
		public Texture2DArray emisArray;

		// Token: 0x040023FC RID: 9212
		[HideInInspector]
		public Texture2DArray emisArray2;

		// Token: 0x040023FD RID: 9213
		[HideInInspector]
		public Texture2DArray emisArray3;

		// Token: 0x040023FE RID: 9214
		public TextureArrayConfig.TextureArrayGroup defaultTextureSettings = new TextureArrayConfig.TextureArrayGroup();

		// Token: 0x040023FF RID: 9215
		public List<TextureArrayConfig.PlatformTextureOverride> platformOverrides = new List<TextureArrayConfig.PlatformTextureOverride>();

		// Token: 0x04002400 RID: 9216
		public TextureArrayConfig.SourceTextureSize sourceTextureSize;

		// Token: 0x04002401 RID: 9217
		[HideInInspector]
		public TextureArrayConfig.AllTextureChannel allTextureChannelHeight = TextureArrayConfig.AllTextureChannel.G;

		// Token: 0x04002402 RID: 9218
		[HideInInspector]
		public TextureArrayConfig.AllTextureChannel allTextureChannelSmoothness = TextureArrayConfig.AllTextureChannel.G;

		// Token: 0x04002403 RID: 9219
		[HideInInspector]
		public TextureArrayConfig.AllTextureChannel allTextureChannelAO = TextureArrayConfig.AllTextureChannel.G;

		// Token: 0x04002404 RID: 9220
		[HideInInspector]
		public List<TextureArrayConfig.TextureEntry> sourceTextures = new List<TextureArrayConfig.TextureEntry>();

		// Token: 0x04002405 RID: 9221
		[HideInInspector]
		public List<TextureArrayConfig.TextureEntry> sourceTextures2 = new List<TextureArrayConfig.TextureEntry>();

		// Token: 0x04002406 RID: 9222
		[HideInInspector]
		public List<TextureArrayConfig.TextureEntry> sourceTextures3 = new List<TextureArrayConfig.TextureEntry>();

		// Token: 0x020004D0 RID: 1232
		public enum AllTextureChannel
		{
			// Token: 0x04002C41 RID: 11329
			R,
			// Token: 0x04002C42 RID: 11330
			G,
			// Token: 0x04002C43 RID: 11331
			B,
			// Token: 0x04002C44 RID: 11332
			A,
			// Token: 0x04002C45 RID: 11333
			Custom
		}

		// Token: 0x020004D1 RID: 1233
		public enum TextureChannel
		{
			// Token: 0x04002C47 RID: 11335
			R,
			// Token: 0x04002C48 RID: 11336
			G,
			// Token: 0x04002C49 RID: 11337
			B,
			// Token: 0x04002C4A RID: 11338
			A
		}

		// Token: 0x020004D2 RID: 1234
		public enum Compression
		{
			// Token: 0x04002C4C RID: 11340
			AutomaticCompressed,
			// Token: 0x04002C4D RID: 11341
			ForceDXT,
			// Token: 0x04002C4E RID: 11342
			ForcePVR,
			// Token: 0x04002C4F RID: 11343
			ForceETC2,
			// Token: 0x04002C50 RID: 11344
			ForceASTC,
			// Token: 0x04002C51 RID: 11345
			ForceCrunch,
			// Token: 0x04002C52 RID: 11346
			Uncompressed
		}

		// Token: 0x020004D3 RID: 1235
		public enum TextureSize
		{
			// Token: 0x04002C54 RID: 11348
			k4096 = 4096,
			// Token: 0x04002C55 RID: 11349
			k2048 = 2048,
			// Token: 0x04002C56 RID: 11350
			k1024 = 1024,
			// Token: 0x04002C57 RID: 11351
			k512 = 512,
			// Token: 0x04002C58 RID: 11352
			k256 = 256,
			// Token: 0x04002C59 RID: 11353
			k128 = 128,
			// Token: 0x04002C5A RID: 11354
			k64 = 64,
			// Token: 0x04002C5B RID: 11355
			k32 = 32
		}

		// Token: 0x020004D4 RID: 1236
		[Serializable]
		public class TextureArraySettings
		{
			// Token: 0x06001B37 RID: 6967 RVA: 0x000F87B5 File Offset: 0x000F69B5
			public TextureArraySettings(TextureArrayConfig.TextureSize s, TextureArrayConfig.Compression c, FilterMode f, int a = 1)
			{
				this.textureSize = s;
				this.compression = c;
				this.filterMode = f;
				this.Aniso = a;
			}

			// Token: 0x04002C5C RID: 11356
			public TextureArrayConfig.TextureSize textureSize;

			// Token: 0x04002C5D RID: 11357
			public TextureArrayConfig.Compression compression;

			// Token: 0x04002C5E RID: 11358
			public FilterMode filterMode;

			// Token: 0x04002C5F RID: 11359
			[Range(0f, 16f)]
			public int Aniso = 1;
		}

		// Token: 0x020004D5 RID: 1237
		public enum PBRWorkflow
		{
			// Token: 0x04002C61 RID: 11361
			Metallic,
			// Token: 0x04002C62 RID: 11362
			Specular
		}

		// Token: 0x020004D6 RID: 1238
		public enum PackingMode
		{
			// Token: 0x04002C64 RID: 11364
			Fastest,
			// Token: 0x04002C65 RID: 11365
			Quality
		}

		// Token: 0x020004D7 RID: 1239
		public enum SourceTextureSize
		{
			// Token: 0x04002C67 RID: 11367
			Unchanged,
			// Token: 0x04002C68 RID: 11368
			k32 = 32,
			// Token: 0x04002C69 RID: 11369
			k256 = 256
		}

		// Token: 0x020004D8 RID: 1240
		public enum TextureMode
		{
			// Token: 0x04002C6B RID: 11371
			Basic,
			// Token: 0x04002C6C RID: 11372
			PBR,
			// Token: 0x04002C6D RID: 11373
			Decal = 3,
			// Token: 0x04002C6E RID: 11374
			DecalSplatMap
		}

		// Token: 0x020004D9 RID: 1241
		public enum ClusterMode
		{
			// Token: 0x04002C70 RID: 11376
			None,
			// Token: 0x04002C71 RID: 11377
			TwoVariations,
			// Token: 0x04002C72 RID: 11378
			ThreeVariations
		}

		// Token: 0x020004DA RID: 1242
		[Serializable]
		public class TextureArrayGroup
		{
			// Token: 0x04002C73 RID: 11379
			public TextureArrayConfig.TextureArraySettings diffuseSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x04002C74 RID: 11380
			public TextureArrayConfig.TextureArraySettings normalSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Trilinear, 1);

			// Token: 0x04002C75 RID: 11381
			public TextureArrayConfig.TextureArraySettings smoothSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x04002C76 RID: 11382
			public TextureArrayConfig.TextureArraySettings antiTileSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x04002C77 RID: 11383
			public TextureArrayConfig.TextureArraySettings emissiveSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x04002C78 RID: 11384
			public TextureArrayConfig.TextureArraySettings specularSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x04002C79 RID: 11385
			public TextureArrayConfig.TextureArraySettings traxDiffuseSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x04002C7A RID: 11386
			public TextureArrayConfig.TextureArraySettings traxNormalSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x04002C7B RID: 11387
			public TextureArrayConfig.TextureArraySettings decalSplatSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);
		}

		// Token: 0x020004DB RID: 1243
		[Serializable]
		public class PlatformTextureOverride
		{
			// Token: 0x04002C7C RID: 11388
			public TextureArrayConfig.TextureArrayGroup settings = new TextureArrayConfig.TextureArrayGroup();
		}

		// Token: 0x020004DC RID: 1244
		[Serializable]
		public class TextureEntry
		{
			// Token: 0x06001B3A RID: 6970 RVA: 0x000F88B8 File Offset: 0x000F6AB8
			public void Reset()
			{
				this.diffuse = null;
				this.height = null;
				this.normal = null;
				this.smoothness = null;
				this.specular = null;
				this.ao = null;
				this.isRoughness = false;
				this.detailNoise = null;
				this.distanceNoise = null;
				this.metal = null;
				this.emis = null;
				this.heightChannel = TextureArrayConfig.TextureChannel.G;
				this.smoothnessChannel = TextureArrayConfig.TextureChannel.G;
				this.aoChannel = TextureArrayConfig.TextureChannel.G;
				this.distanceChannel = TextureArrayConfig.TextureChannel.G;
				this.detailChannel = TextureArrayConfig.TextureChannel.G;
				this.traxDiffuse = null;
				this.traxNormal = null;
				this.traxHeight = null;
				this.traxSmoothness = null;
				this.traxAO = null;
				this.traxHeightChannel = TextureArrayConfig.TextureChannel.G;
				this.traxSmoothnessChannel = TextureArrayConfig.TextureChannel.G;
				this.traxAOChannel = TextureArrayConfig.TextureChannel.G;
				this.splat = null;
			}

			// Token: 0x06001B3B RID: 6971 RVA: 0x000F8974 File Offset: 0x000F6B74
			public bool HasTextures(TextureArrayConfig.PBRWorkflow wf)
			{
				if (wf == TextureArrayConfig.PBRWorkflow.Specular)
				{
					return this.splat != null || this.diffuse != null || this.height != null || this.normal != null || this.smoothness != null || this.specular != null || this.ao != null;
				}
				return this.splat != null || this.diffuse != null || this.height != null || this.normal != null || this.smoothness != null || this.metal != null || this.ao != null;
			}

			// Token: 0x04002C7D RID: 11389
			public Texture2D diffuse;

			// Token: 0x04002C7E RID: 11390
			public Texture2D height;

			// Token: 0x04002C7F RID: 11391
			public TextureArrayConfig.TextureChannel heightChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x04002C80 RID: 11392
			public Texture2D normal;

			// Token: 0x04002C81 RID: 11393
			public Texture2D smoothness;

			// Token: 0x04002C82 RID: 11394
			public TextureArrayConfig.TextureChannel smoothnessChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x04002C83 RID: 11395
			public bool isRoughness;

			// Token: 0x04002C84 RID: 11396
			public Texture2D ao;

			// Token: 0x04002C85 RID: 11397
			public TextureArrayConfig.TextureChannel aoChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x04002C86 RID: 11398
			public Texture2D emis;

			// Token: 0x04002C87 RID: 11399
			public Texture2D metal;

			// Token: 0x04002C88 RID: 11400
			public TextureArrayConfig.TextureChannel metalChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x04002C89 RID: 11401
			public Texture2D specular;

			// Token: 0x04002C8A RID: 11402
			public Texture2D noiseNormal;

			// Token: 0x04002C8B RID: 11403
			public Texture2D detailNoise;

			// Token: 0x04002C8C RID: 11404
			public TextureArrayConfig.TextureChannel detailChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x04002C8D RID: 11405
			public Texture2D distanceNoise;

			// Token: 0x04002C8E RID: 11406
			public TextureArrayConfig.TextureChannel distanceChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x04002C8F RID: 11407
			public Texture2D traxDiffuse;

			// Token: 0x04002C90 RID: 11408
			public Texture2D traxHeight;

			// Token: 0x04002C91 RID: 11409
			public TextureArrayConfig.TextureChannel traxHeightChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x04002C92 RID: 11410
			public Texture2D traxNormal;

			// Token: 0x04002C93 RID: 11411
			public Texture2D traxSmoothness;

			// Token: 0x04002C94 RID: 11412
			public TextureArrayConfig.TextureChannel traxSmoothnessChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x04002C95 RID: 11413
			public bool traxIsRoughness;

			// Token: 0x04002C96 RID: 11414
			public Texture2D traxAO;

			// Token: 0x04002C97 RID: 11415
			public TextureArrayConfig.TextureChannel traxAOChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x04002C98 RID: 11416
			public Texture2D splat;
		}
	}
}
