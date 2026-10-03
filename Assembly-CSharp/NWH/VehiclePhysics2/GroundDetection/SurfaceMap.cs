using System;
using System.Collections.Generic;
using UnityEngine;

namespace NWH.VehiclePhysics2.GroundDetection
{
	// Token: 0x020002A9 RID: 681
	[Serializable]
	public class SurfaceMap
	{
		// Token: 0x0400225A RID: 8794
		[Tooltip("    Name of the surface map. For display purposes only.")]
		public string name;

		// Token: 0x0400225B RID: 8795
		public SurfacePreset surfacePreset;

		// Token: 0x0400225C RID: 8796
		[Tooltip("    Objects with tags in this list will be recognized as this type of surface.")]
		public List<string> tags = new List<string>();

		// Token: 0x0400225D RID: 8797
		[Tooltip("Indices of terrain textures that represent this type of surface. Starts with 0 with the first texture being in the top left corner under terrain settings - Paint Texture.")]
		public List<int> terrainTextureIndices = new List<int>();
	}
}
