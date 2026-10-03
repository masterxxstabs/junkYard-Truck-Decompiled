using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200017C RID: 380
public class mudemitter : MonoBehaviour
{
	// Token: 0x0600094D RID: 2381 RVA: 0x0007DDF0 File Offset: 0x0007BFF0
	public void CreateMudBlob()
	{
		if (this.blobs.Count < this.maxBlobs)
		{
			this.ranX = Random.Range(1, 8);
			this.ranY = Random.Range(1, 8);
			this.ranZ = Random.Range(1, 8);
			this.startSize = Random.Range(1, 2);
			Object.Instantiate<GameObject>(this.mudBlobs[Random.Range(0, this.mudBlobs.Length - 1)], base.transform.position, base.transform.rotation).transform.localScale += new Vector3((float)this.ranX, (float)this.ranY, (float)this.ranZ);
		}
	}

	// Token: 0x040018E4 RID: 6372
	private List<GameObject> blobs = new List<GameObject>();

	// Token: 0x040018E5 RID: 6373
	public GameObject[] mudBlobs;

	// Token: 0x040018E6 RID: 6374
	private int maxBlobs = 200;

	// Token: 0x040018E7 RID: 6375
	private int startSize;

	// Token: 0x040018E8 RID: 6376
	private int ranX;

	// Token: 0x040018E9 RID: 6377
	private int ranY;

	// Token: 0x040018EA RID: 6378
	private int ranZ;
}
