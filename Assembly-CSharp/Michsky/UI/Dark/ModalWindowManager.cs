using System;
using UnityEngine;

namespace Michsky.UI.Dark
{
	// Token: 0x0200031A RID: 794
	public class ModalWindowManager : MonoBehaviour
	{
		// Token: 0x0600145F RID: 5215 RVA: 0x000DA925 File Offset: 0x000D8B25
		private void Start()
		{
			this.mWindowAnimator = base.gameObject.GetComponent<Animator>();
		}

		// Token: 0x06001460 RID: 5216 RVA: 0x000DA938 File Offset: 0x000D8B38
		public void ModalWindowIn()
		{
			this.mWindowAnimator.Play("Modal Window In");
			if (this.enableSplash)
			{
				this.brushAnimator.Play("Transition Out");
			}
		}

		// Token: 0x06001461 RID: 5217 RVA: 0x000DA962 File Offset: 0x000D8B62
		public void ModalWindowOut()
		{
			this.mWindowAnimator.Play("Modal Window Out");
			if (this.enableSplash)
			{
				this.brushAnimator.Play("Transition In");
			}
		}

		// Token: 0x040024C7 RID: 9415
		[Header("BRUSH ANIMATION")]
		public Animator brushAnimator;

		// Token: 0x040024C8 RID: 9416
		public bool enableSplash = true;

		// Token: 0x040024C9 RID: 9417
		private Animator mWindowAnimator;
	}
}
