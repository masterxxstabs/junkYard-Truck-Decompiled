using System;
using UnityEngine;

namespace JBooth.MicroSplat
{
	// Token: 0x020002CE RID: 718
	public class MicroSplatRuntimeUtil
	{
		// Token: 0x06001363 RID: 4963 RVA: 0x000CB6EC File Offset: 0x000C98EC
		public static Vector2 UnityUVScaleToUVScale(Vector2 uv, Terrain t)
		{
			float x = t.terrainData.size.x;
			float z = t.terrainData.size.z;
			uv.x = 1f / (uv.x / x);
			uv.y = 1f / (uv.y / z);
			return uv;
		}

		// Token: 0x06001364 RID: 4964 RVA: 0x000CB748 File Offset: 0x000C9948
		public static Vector2 UVScaleToUnityUVScale(Vector2 uv, Terrain t)
		{
			float x = t.terrainData.size.x;
			float z = t.terrainData.size.z;
			if (uv.x < 0f)
			{
				uv.x = 0.001f;
			}
			if (uv.y < 0f)
			{
				uv.y = 0.001f;
			}
			uv.x = x / uv.x;
			uv.y = z / uv.y;
			return uv;
		}
	}
}
