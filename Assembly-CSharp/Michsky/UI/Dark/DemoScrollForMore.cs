using System;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Dark
{
	// Token: 0x02000312 RID: 786
	public class DemoScrollForMore : MonoBehaviour
	{
		// Token: 0x06001444 RID: 5188 RVA: 0x000D9F98 File Offset: 0x000D8198
		private void Update()
		{
			if (this.listScrollbar.value >= this.fadeOutValue)
			{
				this.SFMAnimator.Play("SFM In");
				return;
			}
			this.SFMAnimator.Play("SFM Out");
		}

		// Token: 0x0400249F RID: 9375
		[Header("RESOURCES")]
		public Scrollbar listScrollbar;

		// Token: 0x040024A0 RID: 9376
		public Animator SFMAnimator;

		// Token: 0x040024A1 RID: 9377
		[Header("SETTINGS")]
		public float fadeOutValue;
	}
}
