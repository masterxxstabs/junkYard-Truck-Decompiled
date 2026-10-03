using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200013A RID: 314
public class SleepScript : MonoBehaviour
{
	// Token: 0x06000820 RID: 2080 RVA: 0x0006C758 File Offset: 0x0006A958
	public void SleepNow()
	{
		if (!this.isSleeping)
		{
			base.StartCoroutine(this.FadeBlackOutSquare());
			this.isSleeping = true;
		}
	}

	// Token: 0x06000821 RID: 2081 RVA: 0x0006C776 File Offset: 0x0006A976
	public IEnumerator FadeBlackOutSquare()
	{
		Color objectColor = this.blackOutSquare.GetComponent<Image>().color;
		while (this.blackOutSquare.GetComponent<Image>().color.a < 1f)
		{
			float a = objectColor.a + (float)this.fadeSpeed * Time.deltaTime;
			objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, a);
			this.blackOutSquare.GetComponent<Image>().color = objectColor;
			yield return null;
		}
		yield return new WaitForEndOfFrame();
		yield return new WaitForSeconds(3f);
		base.StartCoroutine(this.FadeIn());
		yield break;
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x0006C785 File Offset: 0x0006A985
	public IEnumerator FadeIn()
	{
		Color objectColor = this.blackOutSquare.GetComponent<Image>().color;
		while (this.blackOutSquare.GetComponent<Image>().color.a > 0f)
		{
			float a = objectColor.a - (float)this.fadeSpeed * Time.deltaTime;
			objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, a);
			this.blackOutSquare.GetComponent<Image>().color = objectColor;
			yield return null;
		}
		yield return new WaitForEndOfFrame();
		this.isSleeping = false;
		yield break;
	}

	// Token: 0x04001304 RID: 4868
	public GameObject blackOutSquare;

	// Token: 0x04001305 RID: 4869
	private int fadeSpeed = 3;

	// Token: 0x04001306 RID: 4870
	private bool isSleeping;
}
