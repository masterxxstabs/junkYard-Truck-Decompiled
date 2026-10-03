using System;
using System.Collections.Generic;
using UnityEngine;

namespace Michsky.UI.Dark
{
	// Token: 0x02000319 RID: 793
	public class MainPanelManager : MonoBehaviour
	{
		// Token: 0x06001459 RID: 5209 RVA: 0x000DA48C File Offset: 0x000D868C
		private void Start()
		{
			this.currentPanel = this.panels[this.currentPanelIndex];
			this.currentPanelAnimator = this.currentPanel.GetComponent<Animator>();
			this.currentPanelAnimator.Play(this.panelFadeIn);
			if (this.enableHomeBlur)
			{
				this.homeBlurManager.BlurInAnim();
			}
		}

		// Token: 0x0600145A RID: 5210 RVA: 0x000DA4E8 File Offset: 0x000D86E8
		public void OpenFirstTab()
		{
			this.currentPanel = this.panels[this.currentPanelIndex];
			this.currentPanelAnimator = this.currentPanel.GetComponent<Animator>();
			this.currentPanelAnimator.Play(this.panelFadeIn);
			if (this.enableHomeBlur)
			{
				this.homeBlurManager.BlurInAnim();
			}
		}

		// Token: 0x0600145B RID: 5211 RVA: 0x000DA544 File Offset: 0x000D8744
		public void PanelAnim(int newPanel)
		{
			if (newPanel != this.currentPanelIndex)
			{
				this.currentPanel = this.panels[this.currentPanelIndex];
				this.currentPanelIndex = newPanel;
				this.nextPanel = this.panels[this.currentPanelIndex];
				this.currentPanelAnimator = this.currentPanel.GetComponent<Animator>();
				this.nextPanelAnimator = this.nextPanel.GetComponent<Animator>();
				this.currentPanelAnimator.Play(this.panelFadeOut);
				this.nextPanelAnimator.Play(this.panelFadeIn);
				if (this.enableBrushAnimation)
				{
					this.currentBrush = this.currentPanel.GetComponent<PanelBrushManager>();
					if (this.currentBrush.brushAnimator != null)
					{
						this.currentBrush.BrushSplashOut();
					}
					this.nextBrush = this.nextPanel.GetComponent<PanelBrushManager>();
					if (this.nextBrush.brushAnimator != null)
					{
						this.nextBrush.BrushSplashIn();
					}
				}
				if (this.currentPanelIndex == 0 && this.enableHomeBlur)
				{
					this.homeBlurManager.BlurInAnim();
					return;
				}
				if (this.currentPanelIndex != 0 && this.enableHomeBlur)
				{
					this.homeBlurManager.BlurOutAnim();
				}
			}
		}

		// Token: 0x0600145C RID: 5212 RVA: 0x000DA674 File Offset: 0x000D8874
		public void NextPage()
		{
			if (this.currentPanelIndex <= this.panels.Count - 2)
			{
				this.currentPanel = this.panels[this.currentPanelIndex];
				this.currentPanelAnimator = this.currentPanel.GetComponent<Animator>();
				this.currentPanelAnimator.Play(this.panelFadeOut);
				this.currentPanelIndex++;
				this.nextPanel = this.panels[this.currentPanelIndex];
				this.nextPanelAnimator = this.nextPanel.GetComponent<Animator>();
				this.nextPanelAnimator.Play(this.panelFadeIn);
				if (this.enableBrushAnimation)
				{
					this.currentBrush = this.currentPanel.GetComponent<PanelBrushManager>();
					if (this.currentBrush.brushAnimator != null)
					{
						this.currentBrush.BrushSplashOut();
					}
					this.nextBrush = this.nextPanel.GetComponent<PanelBrushManager>();
					if (this.nextBrush.brushAnimator != null)
					{
						this.nextBrush.BrushSplashIn();
					}
				}
				if (this.currentPanelIndex == 0 && this.enableHomeBlur)
				{
					this.homeBlurManager.BlurInAnim();
					return;
				}
				if (this.currentPanelIndex != 0 && this.enableHomeBlur)
				{
					this.homeBlurManager.BlurOutAnim();
				}
			}
		}

		// Token: 0x0600145D RID: 5213 RVA: 0x000DA7B8 File Offset: 0x000D89B8
		public void PrevPage()
		{
			if (this.currentPanelIndex >= 1)
			{
				this.currentPanel = this.panels[this.currentPanelIndex];
				this.currentPanelAnimator = this.currentPanel.GetComponent<Animator>();
				this.currentPanelAnimator.Play(this.panelFadeOut);
				this.currentPanelIndex--;
				this.nextPanel = this.panels[this.currentPanelIndex];
				this.nextPanelAnimator = this.nextPanel.GetComponent<Animator>();
				this.nextPanelAnimator.Play(this.panelFadeIn);
				if (this.enableBrushAnimation)
				{
					this.currentBrush = this.currentPanel.GetComponent<PanelBrushManager>();
					if (this.currentBrush.brushAnimator != null)
					{
						this.currentBrush.BrushSplashOut();
					}
					this.nextBrush = this.nextPanel.GetComponent<PanelBrushManager>();
					if (this.nextBrush.brushAnimator != null)
					{
						this.nextBrush.BrushSplashIn();
					}
				}
				if (this.currentPanelIndex == 0 && this.enableHomeBlur)
				{
					this.homeBlurManager.BlurInAnim();
					return;
				}
				if (this.currentPanelIndex != 0 && this.enableHomeBlur)
				{
					this.homeBlurManager.BlurOutAnim();
				}
			}
		}

		// Token: 0x040024BA RID: 9402
		[Header("PANEL LIST")]
		public List<GameObject> panels = new List<GameObject>();

		// Token: 0x040024BB RID: 9403
		[Header("RESOURCES")]
		public BlurManager homeBlurManager;

		// Token: 0x040024BC RID: 9404
		[Header("SETTINGS")]
		public int currentPanelIndex;

		// Token: 0x040024BD RID: 9405
		public bool enableBrushAnimation = true;

		// Token: 0x040024BE RID: 9406
		public bool enableHomeBlur = true;

		// Token: 0x040024BF RID: 9407
		private GameObject currentPanel;

		// Token: 0x040024C0 RID: 9408
		private GameObject nextPanel;

		// Token: 0x040024C1 RID: 9409
		private Animator currentPanelAnimator;

		// Token: 0x040024C2 RID: 9410
		private Animator nextPanelAnimator;

		// Token: 0x040024C3 RID: 9411
		private string panelFadeIn = "Panel In";

		// Token: 0x040024C4 RID: 9412
		private string panelFadeOut = "Panel Out";

		// Token: 0x040024C5 RID: 9413
		private PanelBrushManager currentBrush;

		// Token: 0x040024C6 RID: 9414
		private PanelBrushManager nextBrush;
	}
}
