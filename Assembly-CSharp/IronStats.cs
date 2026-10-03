using System;
using UnityEngine;

// Token: 0x020000D3 RID: 211
public class IronStats : MonoBehaviour
{
	// Token: 0x06000552 RID: 1362 RVA: 0x00042A34 File Offset: 0x00040C34
	private void Start()
	{
		if (this.xSize > 0f)
		{
			base.transform.localScale = new Vector3(this.xSize / 2f, this.ySize / 2f, this.zSize / 2f);
		}
	}

	// Token: 0x04000B48 RID: 2888
	public float xSize;

	// Token: 0x04000B49 RID: 2889
	public float ySize;

	// Token: 0x04000B4A RID: 2890
	public float zSize;
}
