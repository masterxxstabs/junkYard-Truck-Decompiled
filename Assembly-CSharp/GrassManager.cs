using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000149 RID: 329
public class GrassManager : MonoBehaviour
{
	// Token: 0x0600085C RID: 2140 RVA: 0x0006DBF0 File Offset: 0x0006BDF0
	private void Update()
	{
		List<Vector4> list = new List<Vector4>();
		List<float> list2 = new List<float>();
		int num = 0;
		foreach (GrassCollider grassCollider in this.TerrainGrassCollider.ToArray())
		{
			if (grassCollider.Collider != null)
			{
				list.Add(new Vector4(grassCollider.Collider.position.x, grassCollider.Collider.position.y, grassCollider.Collider.position.z, 0f));
				list2.Add(grassCollider.Radius);
				num++;
			}
		}
		if (list.Count > 0)
		{
			Shader.SetGlobalVectorArray("_Obstacles", list);
			Shader.SetGlobalFloatArray("_ObstacleRadius", list2);
			Shader.SetGlobalFloat("_ObstacleMax", (float)num);
			Shader.SetGlobalFloat("_BendIntensity", this.GrassBendingIntensity);
		}
		else
		{
			Shader.SetGlobalFloat("_BendIntensity", 0f);
		}
		Shader.SetGlobalFloat("_ShakeWindspeed", this.GrassSwaySpeed);
		Shader.SetGlobalFloat("_ShakeBending", this.GrassSwayBendIntensity);
		Shader.SetGlobalFloat("_ShakeTime", this.GrassSwayBendTime);
		Shader.SetGlobalFloat("_ShakeDisplacement", this.GrassSwayDisplacement);
	}

	// Token: 0x04001359 RID: 4953
	[Range(1f, 100f)]
	public float GrassBendingIntensity = 7.3f;

	// Token: 0x0400135A RID: 4954
	[Range(0f, 1f)]
	public float GrassSpecularIntensity;

	// Token: 0x0400135B RID: 4955
	[Range(0f, 100f)]
	public float GrassSwaySpeed = 1f;

	// Token: 0x0400135C RID: 4956
	[Range(0f, 100f)]
	public float GrassSwayBendIntensity = 1f;

	// Token: 0x0400135D RID: 4957
	[Range(0f, 100f)]
	public float GrassSwayBendTime = 1f;

	// Token: 0x0400135E RID: 4958
	[Range(0f, 100f)]
	public float GrassSwayDisplacement = 1f;

	// Token: 0x0400135F RID: 4959
	public List<GrassCollider> TerrainGrassCollider = new List<GrassCollider>();
}
