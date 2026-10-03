using System;
using UnityEngine;

namespace Michsky.UI.Dark
{
	// Token: 0x02000325 RID: 805
	public class SplashScreenManager : MonoBehaviour
	{
		// Token: 0x06001499 RID: 5273 RVA: 0x000DB9B4 File Offset: 0x000D9BB4
		private void Start()
		{
			if (this.disableSplashScreen)
			{
				this.splashScreen.SetActive(true);
				this.splashScreenAnimator = this.splashScreen.GetComponent<Animator>();
				this.splashScreenAnimator.Play("Splash Out");
				this.mainPanels.SetActive(true);
				this.mainPanelsAnimator = this.mainPanels.GetComponent<Animator>();
				this.mainPanelsAnimator.Play("Splash Disabled");
				this.homePanelAnimator = this.homePanel.GetComponent<Animator>();
				this.homePanelAnimator.Play("Panel In");
				return;
			}
			this.splashScreen.SetActive(true);
		}

		// Token: 0x0600149A RID: 5274 RVA: 0x000DBA51 File Offset: 0x000D9C51
		private void OnEnable()
		{
			this.Start();
		}

		// Token: 0x04002508 RID: 9480
		[Header("RESOURCES")]
		public GameObject splashScreen;

		// Token: 0x04002509 RID: 9481
		public GameObject mainPanels;

		// Token: 0x0400250A RID: 9482
		public GameObject homePanel;

		// Token: 0x0400250B RID: 9483
		private Animator splashScreenAnimator;

		// Token: 0x0400250C RID: 9484
		private Animator mainPanelsAnimator;

		// Token: 0x0400250D RID: 9485
		private Animator homePanelAnimator;

		// Token: 0x0400250E RID: 9486
		[Header("SETTINGS")]
		public bool disableSplashScreen;
	}
}
