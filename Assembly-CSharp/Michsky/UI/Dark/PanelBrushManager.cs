using System;
using UnityEngine;

namespace Michsky.UI.Dark
{
	// Token: 0x0200031B RID: 795
	public class PanelBrushManager : MonoBehaviour
	{
		// Token: 0x06001463 RID: 5219 RVA: 0x000DA99B File Offset: 0x000D8B9B
		public void BrushSplashIn()
		{
			this.brushAnimator.Play("Transition In");
		}

		// Token: 0x06001464 RID: 5220 RVA: 0x000DA9AD File Offset: 0x000D8BAD
		public void BrushSplashOut()
		{
			this.brushAnimator.Play("Transition Out");
		}

		// Token: 0x040024CA RID: 9418
		[Header("BRUSH ANIMATION")]
		public Animator brushAnimator;
	}
}
