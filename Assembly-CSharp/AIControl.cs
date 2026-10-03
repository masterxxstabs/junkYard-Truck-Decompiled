using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000BE RID: 190
public class AIControl : CarControl
{
	// Token: 0x06000471 RID: 1137 RVA: 0x0002F5AE File Offset: 0x0002D7AE
	private void Start()
	{
		base.StartCoroutine(this.ChangeIdea());
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x0002F5BD File Offset: 0x0002D7BD
	private IEnumerator ChangeIdea()
	{
		while (base.enabled)
		{
			base.ControlCar((float)(Random.Range(0, 3) - 1), (float)(Random.Range(0, 3) - 1));
			yield return new WaitForSeconds(Random.value * 3f);
		}
		yield break;
	}
}
