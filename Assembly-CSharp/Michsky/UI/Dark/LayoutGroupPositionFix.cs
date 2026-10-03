using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Dark
{
	// Token: 0x02000317 RID: 791
	public class LayoutGroupPositionFix : MonoBehaviour
	{
		// Token: 0x06001452 RID: 5202 RVA: 0x000DA3D1 File Offset: 0x000D85D1
		private void Start()
		{
			this.lg = base.gameObject.GetComponent<LayoutGroup>();
			base.StartCoroutine(this.ExecuteAfterTime(0.01f));
		}

		// Token: 0x06001453 RID: 5203 RVA: 0x000DA3F6 File Offset: 0x000D85F6
		private IEnumerator ExecuteAfterTime(float time)
		{
			yield return new WaitForSeconds(time);
			this.lg.enabled = false;
			this.lg.enabled = true;
			base.StopCoroutine(this.ExecuteAfterTime(0.01f));
			Object.Destroy(this);
			yield break;
		}

		// Token: 0x040024B8 RID: 9400
		private LayoutGroup lg;
	}
}
