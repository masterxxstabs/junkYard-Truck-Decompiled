using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200002C RID: 44
public class SkidTrailAI : MonoBehaviour
{
	// Token: 0x060000B4 RID: 180 RVA: 0x00009961 File Offset: 0x00007B61
	private IEnumerator Start()
	{
		for (;;)
		{
			yield return null;
			if (base.transform.parent.parent == null)
			{
				Object.Destroy(base.gameObject, this.persistTime);
			}
		}
		yield break;
	}

	// Token: 0x040001DE RID: 478
	[SerializeField]
	public float persistTime;
}
