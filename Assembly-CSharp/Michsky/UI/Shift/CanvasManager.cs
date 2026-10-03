using System;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Shift
{
	// Token: 0x0200030E RID: 782
	public class CanvasManager : MonoBehaviour
	{
		// Token: 0x0600142C RID: 5164 RVA: 0x000D97A5 File Offset: 0x000D79A5
		private void Start()
		{
			if (this.canvasScaler == null)
			{
				this.canvasScaler = base.gameObject.GetComponent<CanvasScaler>();
			}
		}

		// Token: 0x0600142D RID: 5165 RVA: 0x000D97C6 File Offset: 0x000D79C6
		public void ScaleCanvas(int scale = 1080)
		{
			this.canvasScaler.referenceResolution = new Vector2(this.canvasScaler.referenceResolution.x, (float)scale);
		}

		// Token: 0x0400247A RID: 9338
		public CanvasScaler canvasScaler;
	}
}
