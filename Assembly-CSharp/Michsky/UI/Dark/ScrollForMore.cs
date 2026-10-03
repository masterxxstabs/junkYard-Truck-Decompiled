using System;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Dark
{
	// Token: 0x02000321 RID: 801
	public class ScrollForMore : MonoBehaviour
	{
		// Token: 0x0600148D RID: 5261 RVA: 0x000DB54B File Offset: 0x000D974B
		private void Start()
		{
			this.SFMAnimator = base.gameObject.GetComponent<Animator>();
			this.CheckValue();
		}

		// Token: 0x0600148E RID: 5262 RVA: 0x000DB564 File Offset: 0x000D9764
		public void CheckValue()
		{
			if (!this.invertValue)
			{
				if (this.SFMAnimator != null && this.listScrollbar.value >= this.fadeOutValue)
				{
					this.SFMAnimator.Play("SFM In");
					return;
				}
				if (this.SFMAnimator != null && this.listScrollbar.value <= this.fadeOutValue)
				{
					this.SFMAnimator.Play("SFM Out");
					return;
				}
			}
			else
			{
				if (this.SFMAnimator != null && this.listScrollbar.value <= this.fadeOutValue)
				{
					this.SFMAnimator.Play("SFM In");
					return;
				}
				if (this.SFMAnimator != null && this.listScrollbar.value >= this.fadeOutValue)
				{
					this.SFMAnimator.Play("SFM Out");
				}
			}
		}

		// Token: 0x040024F2 RID: 9458
		[Header("RESOURCES")]
		public Scrollbar listScrollbar;

		// Token: 0x040024F3 RID: 9459
		private Animator SFMAnimator;

		// Token: 0x040024F4 RID: 9460
		[Header("SETTINGS")]
		public float fadeOutValue;

		// Token: 0x040024F5 RID: 9461
		public bool invertValue;
	}
}
