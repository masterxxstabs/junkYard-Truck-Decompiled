using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Michsky.UI.Dark
{
	// Token: 0x02000326 RID: 806
	public class SwitchManager : MonoBehaviour
	{
		// Token: 0x0600149C RID: 5276 RVA: 0x000DBA5C File Offset: 0x000D9C5C
		private void Start()
		{
			this.switchAnimator = base.gameObject.GetComponent<Animator>();
			this.switchButton = base.gameObject.GetComponent<Button>();
			this.switchButton.onClick.AddListener(new UnityAction(this.AnimateSwitch));
			if (this.saveValue)
			{
				if (PlayerPrefs.GetString(this.switchTag + "Switch") == "")
				{
					if (this.isOn)
					{
						this.switchAnimator.Play("Switch On");
						this.isOn = true;
						PlayerPrefs.SetString(this.switchTag + "Switch", "true");
					}
					else
					{
						this.switchAnimator.Play("Switch Off");
						this.isOn = false;
						PlayerPrefs.SetString(this.switchTag + "Switch", "false");
					}
				}
				else if (PlayerPrefs.GetString(this.switchTag + "Switch") == "true")
				{
					this.switchAnimator.Play("Switch On");
					this.isOn = true;
				}
				else if (PlayerPrefs.GetString(this.switchTag + "Switch") == "false")
				{
					this.switchAnimator.Play("Switch Off");
					this.isOn = false;
				}
			}
			else if (this.isOn)
			{
				this.switchAnimator.Play("Switch On");
				this.isOn = true;
			}
			else
			{
				this.switchAnimator.Play("Switch Off");
				this.isOn = false;
			}
			if (this.invokeAtStart && this.isOn)
			{
				this.OnEvents.Invoke();
			}
			if (this.invokeAtStart && !this.isOn)
			{
				this.OffEvents.Invoke();
			}
		}

		// Token: 0x0600149D RID: 5277 RVA: 0x000DBC29 File Offset: 0x000D9E29
		private void OnEnable()
		{
			if (this.isOn)
			{
				this.switchAnimator.Play("Switch On");
				return;
			}
			if (this.switchAnimator != null)
			{
				this.switchAnimator.Play("Switch Off");
			}
		}

		// Token: 0x0600149E RID: 5278 RVA: 0x000DBC64 File Offset: 0x000D9E64
		public void AnimateSwitch()
		{
			if (this.isOn)
			{
				this.switchAnimator.Play("Switch Off");
				this.isOn = false;
				this.OffEvents.Invoke();
				if (this.saveValue)
				{
					PlayerPrefs.SetString(this.switchTag + "Switch", "false");
					return;
				}
			}
			else
			{
				this.switchAnimator.Play("Switch On");
				this.isOn = true;
				this.OnEvents.Invoke();
				if (this.saveValue)
				{
					PlayerPrefs.SetString(this.switchTag + "Switch", "true");
				}
			}
		}

		// Token: 0x0400250F RID: 9487
		[Header("SETTINGS")]
		[Tooltip("IMPORTANT! EVERY SWITCH MUST HAVE A DIFFERENT TAG")]
		public string switchTag = "Switch";

		// Token: 0x04002510 RID: 9488
		public bool isOn = true;

		// Token: 0x04002511 RID: 9489
		public bool saveValue = true;

		// Token: 0x04002512 RID: 9490
		public bool invokeAtStart = true;

		// Token: 0x04002513 RID: 9491
		public UnityEvent OnEvents;

		// Token: 0x04002514 RID: 9492
		public UnityEvent OffEvents;

		// Token: 0x04002515 RID: 9493
		private Animator switchAnimator;

		// Token: 0x04002516 RID: 9494
		private Button switchButton;
	}
}
