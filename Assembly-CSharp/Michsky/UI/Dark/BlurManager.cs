using System;
using System.Collections;
using UnityEngine;

namespace Michsky.UI.Dark
{
	// Token: 0x0200030F RID: 783
	public class BlurManager : MonoBehaviour
	{
		// Token: 0x0600142F RID: 5167 RVA: 0x000D97EA File Offset: 0x000D79EA
		private void Start()
		{
			if (this.customProperty == null)
			{
				this.customProperty = "_Size";
			}
			this.blurMaterial.SetFloat(this.customProperty, 0f);
		}

		// Token: 0x06001430 RID: 5168 RVA: 0x000D9815 File Offset: 0x000D7A15
		private IEnumerator BlurIn()
		{
			this.currentBlurValue = this.blurMaterial.GetFloat(this.customProperty);
			while (this.currentBlurValue <= this.blurValue)
			{
				this.currentBlurValue += Time.deltaTime * this.animationSpeed;
				if (this.currentBlurValue >= this.blurValue)
				{
					this.currentBlurValue = this.blurValue;
				}
				this.blurMaterial.SetFloat(this.customProperty, this.currentBlurValue);
				yield return null;
			}
			base.StopCoroutine("BlurIn");
			yield break;
		}

		// Token: 0x06001431 RID: 5169 RVA: 0x000D9824 File Offset: 0x000D7A24
		private IEnumerator BlurOut()
		{
			this.currentBlurValue = this.blurMaterial.GetFloat(this.customProperty);
			while (this.currentBlurValue >= 0f)
			{
				this.currentBlurValue -= Time.deltaTime * this.animationSpeed;
				if (this.currentBlurValue <= 0f)
				{
					this.currentBlurValue = 0f;
				}
				this.blurMaterial.SetFloat(this.customProperty, this.currentBlurValue);
				yield return null;
			}
			base.StopCoroutine("BlurOut");
			yield break;
		}

		// Token: 0x06001432 RID: 5170 RVA: 0x000D9833 File Offset: 0x000D7A33
		public void BlurInAnim()
		{
			base.StopCoroutine("BlurOut");
			base.StartCoroutine("BlurIn");
		}

		// Token: 0x06001433 RID: 5171 RVA: 0x000D984C File Offset: 0x000D7A4C
		public void BlurOutAnim()
		{
			base.StopCoroutine("BlurIn");
			base.StartCoroutine("BlurOut");
		}

		// Token: 0x06001434 RID: 5172 RVA: 0x000D9865 File Offset: 0x000D7A65
		public void SetBlurValue(float cbv)
		{
			this.blurValue = cbv;
		}

		// Token: 0x0400247B RID: 9339
		[Header("RESOURCES")]
		public Material blurMaterial;

		// Token: 0x0400247C RID: 9340
		[Header("SETTINGS")]
		[Range(0f, 10f)]
		public float blurValue = 5f;

		// Token: 0x0400247D RID: 9341
		[Range(0.1f, 50f)]
		public float animationSpeed = 25f;

		// Token: 0x0400247E RID: 9342
		public string customProperty = "_Size";

		// Token: 0x0400247F RID: 9343
		private float currentBlurValue;
	}
}
