using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

namespace Michsky.UI.Dark
{
	// Token: 0x02000324 RID: 804
	public class SliderManager : MonoBehaviour
	{
		// Token: 0x06001495 RID: 5269 RVA: 0x000DB7A8 File Offset: 0x000D99A8
		private void Start()
		{
			this.mainSlider = base.GetComponent<Slider>();
			if (!this.showValue)
			{
				this.valueText.enabled = false;
			}
			if (this.enableSaving)
			{
				if (!PlayerPrefs.HasKey(this.sliderTag + "DarkSliderValue"))
				{
					this.saveValue = this.defaultValue;
				}
				else
				{
					this.saveValue = PlayerPrefs.GetFloat(this.sliderTag + "DarkSliderValue");
				}
				this.mainSlider.value = this.saveValue;
				this.mainSlider.onValueChanged.AddListener(delegate(float <p0>)
				{
					this.saveValue = this.mainSlider.value;
					PlayerPrefs.SetFloat(this.sliderTag + "DarkSliderValue", this.saveValue);
				});
			}
		}

		// Token: 0x06001496 RID: 5270 RVA: 0x000DB84C File Offset: 0x000D9A4C
		private void Update()
		{
			if (this.useRoundValue)
			{
				if (this.usePercent)
				{
					this.valueText.text = Mathf.Round(this.mainSlider.value * 1f).ToString() + "%";
					return;
				}
				this.valueText.text = Mathf.Round(this.mainSlider.value * 1f).ToString();
				return;
			}
			else
			{
				if (this.usePercent)
				{
					this.valueText.text = this.mainSlider.value.ToString("F1") + "%";
					return;
				}
				this.valueText.text = this.mainSlider.value.ToString("F1");
				if (this.sliderNum == 1)
				{
					this.fpc.m_MouseLook.XSensitivity = this.mainSlider.value;
					this.fpc.m_MouseLook.YSensitivity = this.mainSlider.value;
				}
				return;
			}
		}

		// Token: 0x040024FD RID: 9469
		[Header("TEXTS")]
		public TextMeshProUGUI valueText;

		// Token: 0x040024FE RID: 9470
		[Header("SAVING")]
		public bool enableSaving;

		// Token: 0x040024FF RID: 9471
		public string sliderTag = "Tag Text";

		// Token: 0x04002500 RID: 9472
		public float defaultValue = 1f;

		// Token: 0x04002501 RID: 9473
		[Header("SETTINGS")]
		public bool usePercent;

		// Token: 0x04002502 RID: 9474
		public bool showValue = true;

		// Token: 0x04002503 RID: 9475
		public bool useRoundValue;

		// Token: 0x04002504 RID: 9476
		private Slider mainSlider;

		// Token: 0x04002505 RID: 9477
		private float saveValue;

		// Token: 0x04002506 RID: 9478
		public FirstPersonController fpc;

		// Token: 0x04002507 RID: 9479
		public int sliderNum;
	}
}
