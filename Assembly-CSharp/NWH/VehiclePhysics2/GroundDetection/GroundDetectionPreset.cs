using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.VehiclePhysics2.GroundDetection
{
	// Token: 0x020002A8 RID: 680
	[CreateAssetMenu(fileName = "NWH Vehicle Physics", menuName = "NWH Vehicle Physics/Ground Detection Preset", order = 1)]
	[Serializable]
	public class GroundDetectionPreset : ScriptableObject
	{
		// Token: 0x04002256 RID: 8790
		[FormerlySerializedAs("dustPrefab")]
		[Tooltip("    Prefab of the particle system for generating dust as a result of traveling over sand, gravel, etc.")]
		public GameObject particlePrefab;

		// Token: 0x04002257 RID: 8791
		[Tooltip("    Prefab of the particle system for generating surface chunks / dirt that gets thrown behind the wheel when going over soft surface.")]
		public GameObject chunkPrefab;

		// Token: 0x04002258 RID: 8792
		[Tooltip("Surface preset used when there are no matches in the surfaceMaps list for the current surface.")]
		public SurfacePreset fallbackSurfacePreset;

		// Token: 0x04002259 RID: 8793
		[SerializeField]
		[Tooltip("    Surface maps - each represents a single ground surface.")]
		public List<SurfaceMap> surfaceMaps = new List<SurfaceMap>();
	}
}
