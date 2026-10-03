using System;
using UnityEngine;

// Token: 0x02000178 RID: 376
public class jumpchallenge : MonoBehaviour
{
	// Token: 0x06000942 RID: 2370 RVA: 0x00002188 File Offset: 0x00000388
	private void OnTriggerExit(Collider other)
	{
	}

	// Token: 0x040018D9 RID: 6361
	public GameObject truck;

	// Token: 0x040018DA RID: 6362
	public TimeManager timeManager;

	// Token: 0x040018DB RID: 6363
	public bool isSlow;
}
