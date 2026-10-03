using System;
using System.Collections.Generic;
using UnityEngine;

namespace Michsky.UI.Dark
{
	// Token: 0x0200031D RID: 797
	public class PanelTabManager : MonoBehaviour
	{
		// Token: 0x0600146A RID: 5226 RVA: 0x000DAA44 File Offset: 0x000D8C44
		private void Start()
		{
			this.currentButton = this.buttons[this.currentPanelIndex];
			this.currentButtonAnimator = this.currentButton.GetComponent<Animator>();
			this.currentButtonAnimator.Play(this.buttonFadeIn);
			this.currentPanel = this.panels[this.currentPanelIndex];
			this.currentPanelAnimator = this.currentPanel.GetComponent<Animator>();
			this.currentPanelAnimator.Play(this.panelFadeIn);
		}

		// Token: 0x0600146B RID: 5227 RVA: 0x000DAAC3 File Offset: 0x000D8CC3
		private void OnEnable()
		{
			this.Start();
		}

		// Token: 0x0600146C RID: 5228 RVA: 0x000DAACC File Offset: 0x000D8CCC
		public void OpenFirstTab()
		{
			this.currentPanel = this.panels[this.currentPanelIndex];
			this.currentPanelAnimator = this.currentPanel.GetComponent<Animator>();
			this.currentPanelAnimator.Play(this.panelFadeIn);
			this.currentButton = this.buttons[this.currentPanelIndex];
			this.currentButtonAnimator = this.currentButton.GetComponent<Animator>();
			this.currentButtonAnimator.Play(this.buttonFadeIn);
		}

		// Token: 0x0600146D RID: 5229 RVA: 0x000DAB4C File Offset: 0x000D8D4C
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
				this.currentButton = this.buttons[this.currentButtonlIndex];
				this.currentButtonlIndex = newPanel;
				this.nextButton = this.buttons[this.currentButtonlIndex];
				this.currentButtonAnimator = this.currentButton.GetComponent<Animator>();
				this.nextButtonAnimator = this.nextButton.GetComponent<Animator>();
				this.currentButtonAnimator.Play(this.buttonFadeOut);
				this.nextButtonAnimator.Play(this.buttonFadeIn);
			}
		}

		// Token: 0x0600146E RID: 5230 RVA: 0x000DAC58 File Offset: 0x000D8E58
		public void NextPage()
		{
			if (this.currentPanelIndex <= this.panels.Count - 2)
			{
				this.currentPanel = this.panels[this.currentPanelIndex];
				this.currentButton = this.buttons[this.currentButtonlIndex];
				this.nextButton = this.buttons[this.currentButtonlIndex + 1];
				this.currentPanelAnimator = this.currentPanel.GetComponent<Animator>();
				this.currentButtonAnimator = this.currentButton.GetComponent<Animator>();
				this.currentButtonAnimator.Play(this.buttonFadeOut);
				this.currentPanelAnimator.Play(this.panelFadeOut);
				this.currentPanelIndex++;
				this.currentButtonlIndex++;
				this.nextPanel = this.panels[this.currentPanelIndex];
				this.nextPanelAnimator = this.nextPanel.GetComponent<Animator>();
				this.nextButtonAnimator = this.nextButton.GetComponent<Animator>();
				this.nextPanelAnimator.Play(this.panelFadeIn);
				this.nextButtonAnimator.Play(this.buttonFadeIn);
			}
		}

		// Token: 0x0600146F RID: 5231 RVA: 0x000DAD80 File Offset: 0x000D8F80
		public void PrevPage()
		{
			if (this.currentPanelIndex >= 1)
			{
				this.currentPanel = this.panels[this.currentPanelIndex];
				this.currentButton = this.buttons[this.currentButtonlIndex];
				this.nextButton = this.buttons[this.currentButtonlIndex - 1];
				this.currentPanelAnimator = this.currentPanel.GetComponent<Animator>();
				this.currentButtonAnimator = this.currentButton.GetComponent<Animator>();
				this.currentButtonAnimator.Play(this.buttonFadeOut);
				this.currentPanelAnimator.Play(this.panelFadeOut);
				this.currentPanelIndex--;
				this.currentButtonlIndex--;
				this.nextPanel = this.panels[this.currentPanelIndex];
				this.nextPanelAnimator = this.nextPanel.GetComponent<Animator>();
				this.nextButtonAnimator = this.nextButton.GetComponent<Animator>();
				this.nextPanelAnimator.Play(this.panelFadeIn);
				this.nextButtonAnimator.Play(this.buttonFadeIn);
			}
		}

		// Token: 0x040024CC RID: 9420
		[Header("PANEL LIST")]
		public List<GameObject> panels = new List<GameObject>();

		// Token: 0x040024CD RID: 9421
		[Header("BUTTON LIST")]
		public List<GameObject> buttons = new List<GameObject>();

		// Token: 0x040024CE RID: 9422
		private GameObject currentPanel;

		// Token: 0x040024CF RID: 9423
		private GameObject nextPanel;

		// Token: 0x040024D0 RID: 9424
		private GameObject currentButton;

		// Token: 0x040024D1 RID: 9425
		private GameObject nextButton;

		// Token: 0x040024D2 RID: 9426
		[Header("SETTINGS")]
		public int currentPanelIndex;

		// Token: 0x040024D3 RID: 9427
		private int currentButtonlIndex;

		// Token: 0x040024D4 RID: 9428
		private Animator currentPanelAnimator;

		// Token: 0x040024D5 RID: 9429
		private Animator nextPanelAnimator;

		// Token: 0x040024D6 RID: 9430
		private Animator currentButtonAnimator;

		// Token: 0x040024D7 RID: 9431
		private Animator nextButtonAnimator;

		// Token: 0x040024D8 RID: 9432
		private string panelFadeIn = "Panel In";

		// Token: 0x040024D9 RID: 9433
		private string panelFadeOut = "Panel Out";

		// Token: 0x040024DA RID: 9434
		private string buttonFadeIn = "Hover to Pressed";

		// Token: 0x040024DB RID: 9435
		private string buttonFadeOut = "Pressed to Normal";
	}
}
