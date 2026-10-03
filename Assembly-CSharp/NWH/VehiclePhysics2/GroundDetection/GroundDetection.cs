using System;
using NWH.WheelController3D;
using UnityEngine;

namespace NWH.VehiclePhysics2.GroundDetection
{
	// Token: 0x020002A7 RID: 679
	[Serializable]
	public class GroundDetection : VehicleComponent
	{
		// Token: 0x06001228 RID: 4648 RVA: 0x000B61C6 File Offset: 0x000B43C6
		public override void Initialize()
		{
			this.initialized = true;
		}

		// Token: 0x06001229 RID: 4649 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x0600122A RID: 4650 RVA: 0x00002188 File Offset: 0x00000388
		public override void Update()
		{
		}

		// Token: 0x0600122B RID: 4651 RVA: 0x000C43A8 File Offset: 0x000C25A8
		public bool GetCurrentSurfaceMap(WheelController wheelController, ref int surfaceIndex, ref SurfacePreset outSurfacePreset)
		{
			surfaceIndex = -1;
			outSurfacePreset = null;
			if (!base.IsEnabled)
			{
				return false;
			}
			if (this.groundDetectionPreset == null)
			{
				Debug.LogError("GroundDetectionPreset is required but is null. Go to VehicleController > FX > Grnd. Det. and assign a GroundDetectionPreset.");
				return false;
			}
			NWH.WheelController3D.WheelHit wheelHit = wheelController.wheelHit;
			this.hitTransform = ((wheelHit != null) ? wheelHit.raycastHit.transform : null);
			if (wheelController.isGrounded && this.hitTransform != null)
			{
				NWH.WheelController3D.WheelHit wheelHit2;
				wheelController.GetGroundHit(out wheelHit2);
				int count = this.groundDetectionPreset.surfaceMaps.Count;
				for (int i = 0; i < count; i++)
				{
					SurfaceMap surfaceMap = this.groundDetectionPreset.surfaceMaps[i];
					int count2 = surfaceMap.tags.Count;
					for (int j = 0; j < count2; j++)
					{
						if (this.hitTransform.CompareTag(surfaceMap.tags[j]))
						{
							outSurfacePreset = surfaceMap.surfacePreset;
							surfaceIndex = i;
							return true;
						}
					}
				}
				this.activeTerrain = this.hitTransform.GetComponent<Terrain>();
				if (this.activeTerrain)
				{
					int dominantTerrainTexture = this.GetDominantTerrainTexture(wheelHit2.point, this.activeTerrain);
					if (dominantTerrainTexture != -1)
					{
						for (int k = 0; k < this.groundDetectionPreset.surfaceMaps.Count; k++)
						{
							SurfaceMap surfaceMap2 = this.groundDetectionPreset.surfaceMaps[k];
							int count3 = surfaceMap2.terrainTextureIndices.Count;
							for (int l = 0; l < count3; l++)
							{
								if (surfaceMap2.terrainTextureIndices[l] == dominantTerrainTexture)
								{
									outSurfacePreset = surfaceMap2.surfacePreset;
									surfaceIndex = k;
									return true;
								}
							}
						}
					}
				}
			}
			if (this.groundDetectionPreset.fallbackSurfacePreset != null)
			{
				outSurfacePreset = this.groundDetectionPreset.fallbackSurfacePreset;
				surfaceIndex = -1;
				return true;
			}
			Debug.LogError("Fallback surface map of ground detection preset " + this.groundDetectionPreset.name + " not assigned.");
			outSurfacePreset = null;
			surfaceIndex = -1;
			return false;
		}

		// Token: 0x0600122C RID: 4652 RVA: 0x000C4590 File Offset: 0x000C2790
		public int GetDominantTerrainTexture(Vector3 worldPos, Terrain terrain)
		{
			this.GetTerrainTextureComposition(worldPos, terrain, ref this.mix);
			if (this.mix != null)
			{
				float num = 0f;
				int result = 0;
				for (int i = 0; i < this.mix.Length; i++)
				{
					if (this.mix[i] > num)
					{
						result = i;
						num = this.mix[i];
					}
				}
				return result;
			}
			return -1;
		}

		// Token: 0x0600122D RID: 4653 RVA: 0x000C45E8 File Offset: 0x000C27E8
		public void GetTerrainTextureComposition(Vector3 worldPos, Terrain terrain, ref float[] cellMix)
		{
			this.terrainData = terrain.terrainData;
			this.terrainPos = terrain.transform.position;
			int x = (int)((worldPos.x - this.terrainPos.x) / this.terrainData.size.x * (float)this.terrainData.alphamapWidth);
			int y = (int)((worldPos.z - this.terrainPos.z) / this.terrainData.size.z * (float)this.terrainData.alphamapHeight);
			this.splatmapData = this.terrainData.GetAlphamaps(x, y, 1, 1);
			cellMix = new float[this.splatmapData.GetUpperBound(2) + 1];
			for (int i = 0; i < cellMix.Length; i++)
			{
				cellMix[i] = this.splatmapData[0, 0, i];
			}
		}

		// Token: 0x0600122E RID: 4654 RVA: 0x000C46C1 File Offset: 0x000C28C1
		public override void SetDefaults(VehicleController vc)
		{
			base.SetDefaults(vc);
			if (this.groundDetectionPreset == null)
			{
				this.groundDetectionPreset = (Resources.Load("NWH Vehicle Physics/Defaults/DefaultGroundDetectionPreset") as GroundDetectionPreset);
			}
		}

		// Token: 0x0600122F RID: 4655 RVA: 0x000C46F0 File Offset: 0x000C28F0
		public override void Validate(VehicleController vc)
		{
			base.Validate(vc);
			if (this.groundDetectionPreset != null)
			{
				for (int i = 0; i < this.groundDetectionPreset.surfaceMaps.Count; i++)
				{
					for (int j = 0; j < this.groundDetectionPreset.surfaceMaps[i].tags.Count; j++)
					{
						string text = this.groundDetectionPreset.surfaceMaps[i].tags[j];
						try
						{
							vc.transform.CompareTag(text);
						}
						catch
						{
							Debug.LogWarning(string.Concat(new string[]
							{
								"Tag '",
								text,
								"' does not exist in the scene yet the SurfaceMap ",
								this.groundDetectionPreset.surfaceMaps[i].name,
								" uses it. Make sure to add the missing tag or to remove it from the surface map if not needed. This could happen if you are using default/demo GroundDetectionPreset in a project where these tags are not defined."
							}));
							throw;
						}
					}
				}
			}
		}

		// Token: 0x0400224E RID: 8782
		public GroundDetectionPreset groundDetectionPreset;

		// Token: 0x0400224F RID: 8783
		private Terrain activeTerrain;

		// Token: 0x04002250 RID: 8784
		private int currentIndex;

		// Token: 0x04002251 RID: 8785
		private Transform hitTransform;

		// Token: 0x04002252 RID: 8786
		private float[] mix;

		// Token: 0x04002253 RID: 8787
		private float[,,] splatmapData;

		// Token: 0x04002254 RID: 8788
		private TerrainData terrainData;

		// Token: 0x04002255 RID: 8789
		private Vector3 terrainPos;
	}
}
