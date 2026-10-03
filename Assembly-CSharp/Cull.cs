using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000044 RID: 68
public class Cull : MonoBehaviour
{
	// Token: 0x06000144 RID: 324 RVA: 0x0000EF90 File Offset: 0x0000D190
	private void Start()
	{
		float[] array = new float[32];
		array[0] = this.cullDist;
		array[1] = this.cullDist;
		array[2] = this.cullDist;
		array[3] = this.cullDist;
		array[4] = this.cullDist;
		array[5] = this.cullDist;
		array[6] = this.cullDist;
		array[7] = this.cullDist;
		array[8] = this.cullDist;
		array[9] = this.cullDist;
		array[10] = this.cullDist;
		array[11] = this.cullDist;
		array[12] = this.cullDist;
		array[13] = this.cullDist;
		array[14] = this.cullDist;
		array[15] = this.cullDist;
		array[16] = this.cullDist;
		array[17] = this.cullDist;
		array[18] = this.cullDist;
		array[19] = this.cullDist;
		array[20] = this.cullDist;
		array[21] = this.cullDist;
		array[22] = this.cullDist;
		array[23] = this.cullDist;
		array[24] = this.cullDist;
		array[25] = 2000f;
		this.camera.layerCullDistances = array;
		base.StartCoroutine(this.LimitDepth());
	}

	// Token: 0x06000145 RID: 325 RVA: 0x0000F0B8 File Offset: 0x0000D2B8
	public IEnumerator LimitDepth()
	{
		yield return new WaitForSeconds(1f);
		Camera.main.depthTextureMode = DepthTextureMode.None;
		yield break;
	}

	// Token: 0x0400038F RID: 911
	public Camera camera;

	// Token: 0x04000390 RID: 912
	public float cullDist;
}
