using System;
using System.Collections;
using UnityEngine;

namespace EnviroSamples
{
	// Token: 0x020002D5 RID: 725
	public class SampleHUDFPS : MonoBehaviour
	{
		// Token: 0x0600139A RID: 5018 RVA: 0x000CC754 File Offset: 0x000CA954
		private void Start()
		{
			base.StartCoroutine(this.FPS());
		}

		// Token: 0x0600139B RID: 5019 RVA: 0x000CC763 File Offset: 0x000CA963
		private void Update()
		{
			this.accum += Time.timeScale / Time.deltaTime;
			this.frames++;
		}

		// Token: 0x0600139C RID: 5020 RVA: 0x000CC78B File Offset: 0x000CA98B
		private IEnumerator FPS()
		{
			for (;;)
			{
				float num = this.accum / (float)this.frames;
				this.sFPS = num.ToString("f" + Mathf.Clamp(this.nbDecimal, 0, 10));
				this.color = ((num >= 30f) ? Color.green : ((num > 10f) ? Color.red : Color.yellow));
				this.accum = 0f;
				this.frames = 0;
				yield return new WaitForSeconds(this.frequency);
			}
			yield break;
		}

		// Token: 0x0600139D RID: 5021 RVA: 0x000CC79C File Offset: 0x000CA99C
		private void OnGUI()
		{
			if (this.style == null)
			{
				this.style = new GUIStyle(GUI.skin.label);
				this.style.normal.textColor = Color.white;
				this.style.alignment = TextAnchor.MiddleCenter;
			}
			GUI.color = (this.updateColor ? this.color : Color.white);
			this.startRect = GUI.Window(0, this.startRect, new GUI.WindowFunction(this.DoMyWindow), "");
		}

		// Token: 0x0600139E RID: 5022 RVA: 0x000CC824 File Offset: 0x000CAA24
		private void DoMyWindow(int windowID)
		{
			GUI.Label(new Rect(0f, 0f, this.startRect.width, this.startRect.height), this.sFPS + " FPS", this.style);
			if (this.allowDrag)
			{
				GUI.DragWindow(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height));
			}
		}

		// Token: 0x04002438 RID: 9272
		public Rect startRect = new Rect(10f, 10f, 75f, 50f);

		// Token: 0x04002439 RID: 9273
		public bool updateColor = true;

		// Token: 0x0400243A RID: 9274
		public bool allowDrag = true;

		// Token: 0x0400243B RID: 9275
		public float frequency = 0.5f;

		// Token: 0x0400243C RID: 9276
		public int nbDecimal = 1;

		// Token: 0x0400243D RID: 9277
		private float accum;

		// Token: 0x0400243E RID: 9278
		private int frames;

		// Token: 0x0400243F RID: 9279
		private Color color = Color.white;

		// Token: 0x04002440 RID: 9280
		private string sFPS = "";

		// Token: 0x04002441 RID: 9281
		private GUIStyle style;
	}
}
