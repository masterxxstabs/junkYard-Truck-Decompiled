using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Performance
{
	// Token: 0x02000248 RID: 584
	public class FpsToText : MonoBehaviour
	{
		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06000EDF RID: 3807 RVA: 0x000B1AA7 File Offset: 0x000AFCA7
		public float Framerate
		{
			get
			{
				return this._fps;
			}
		}

		// Token: 0x06000EE0 RID: 3808 RVA: 0x000B1AB0 File Offset: 0x000AFCB0
		protected virtual void Reset()
		{
			this.SampleSize = 20;
			this.UpdateTextEvery = 1;
			this.MaxTextLength = 5;
			this.Smoothed = true;
			this.UseColors = true;
			this.Good = Color.green;
			this.Okay = Color.yellow;
			this.Bad = Color.red;
			this.OkayBelow = 60;
			this.BadBelow = 30;
			this.UseSystemTick = false;
			this.ForceIntResult = true;
		}

		// Token: 0x06000EE1 RID: 3809 RVA: 0x000B1B20 File Offset: 0x000AFD20
		protected virtual void Start()
		{
			this.FpsSamples = new float[this.SampleSize];
			for (int i = 0; i < this.FpsSamples.Length; i++)
			{
				this.FpsSamples[i] = 0.001f;
			}
			if (!this.TargetText)
			{
				base.enabled = false;
			}
		}

		// Token: 0x06000EE2 RID: 3810 RVA: 0x000B1B74 File Offset: 0x000AFD74
		protected virtual void Update()
		{
			if (this.GroupSampling)
			{
				this.Group();
			}
			else
			{
				this.SingleFrame();
			}
			string text = this._fps.ToString(CultureInfo.CurrentCulture);
			this.SampleIndex = ((this.SampleIndex < this.SampleSize - 1) ? (this.SampleIndex + 1) : 0);
			this.TextUpdateIndex = ((this.TextUpdateIndex > this.UpdateTextEvery) ? 0 : (this.TextUpdateIndex + 1));
			if (this.TextUpdateIndex == this.UpdateTextEvery)
			{
				this.TargetText.text = text.Substring(0, (text.Length < 5) ? text.Length : 5);
			}
			if (!this.UseColors)
			{
				return;
			}
			if (this._fps < (float)this.BadBelow)
			{
				this.TargetText.color = this.Bad;
				return;
			}
			this.TargetText.color = ((this._fps < (float)this.OkayBelow) ? this.Okay : this.Good);
		}

		// Token: 0x06000EE3 RID: 3811 RVA: 0x000B1C6C File Offset: 0x000AFE6C
		protected virtual void SingleFrame()
		{
			this._fps = (this.UseSystemTick ? ((float)this.GetSystemFramerate()) : (this.Smoothed ? (1f / Time.smoothDeltaTime) : (1f / Time.deltaTime)));
			if (this.ForceIntResult)
			{
				this._fps = (float)((int)this._fps);
			}
		}

		// Token: 0x06000EE4 RID: 3812 RVA: 0x000B1CC8 File Offset: 0x000AFEC8
		protected virtual void Group()
		{
			this.FpsSamples[this.SampleIndex] = (this.UseSystemTick ? ((float)this.GetSystemFramerate()) : (this.Smoothed ? (1f / Time.smoothDeltaTime) : (1f / Time.deltaTime)));
			this._fps = 0f;
			bool flag = true;
			int num = 0;
			while (flag)
			{
				if (num == this.SampleSize - 1)
				{
					flag = false;
				}
				this._fps += this.FpsSamples[num];
				num++;
			}
			this._fps /= (float)this.FpsSamples.Length;
			if (this.ForceIntResult)
			{
				this._fps = (float)((int)this._fps);
			}
		}

		// Token: 0x06000EE5 RID: 3813 RVA: 0x000B1D7C File Offset: 0x000AFF7C
		protected virtual int GetSystemFramerate()
		{
			if (Environment.TickCount - this._sysLastSysTick >= 1000)
			{
				this._sysLastFrameRate = this._sysFrameRate;
				this._sysFrameRate = 0;
				this._sysLastSysTick = Environment.TickCount;
			}
			this._sysFrameRate++;
			return this._sysLastFrameRate;
		}

		// Token: 0x04001F29 RID: 7977
		[Header("// Sample Groups of Data ")]
		public bool GroupSampling = true;

		// Token: 0x04001F2A RID: 7978
		public int SampleSize = 20;

		// Token: 0x04001F2B RID: 7979
		[Header("// Config ")]
		public Text TargetText;

		// Token: 0x04001F2C RID: 7980
		public int UpdateTextEvery = 1;

		// Token: 0x04001F2D RID: 7981
		public int MaxTextLength = 5;

		// Token: 0x04001F2E RID: 7982
		public bool Smoothed = true;

		// Token: 0x04001F2F RID: 7983
		public bool ForceIntResult = true;

		// Token: 0x04001F30 RID: 7984
		[Header("// System FPS (updates once/sec)")]
		public bool UseSystemTick;

		// Token: 0x04001F31 RID: 7985
		[Header("// Color Config ")]
		public bool UseColors = true;

		// Token: 0x04001F32 RID: 7986
		public Color Good = Color.green;

		// Token: 0x04001F33 RID: 7987
		public Color Okay = Color.yellow;

		// Token: 0x04001F34 RID: 7988
		public Color Bad = Color.red;

		// Token: 0x04001F35 RID: 7989
		public int OkayBelow = 60;

		// Token: 0x04001F36 RID: 7990
		public int BadBelow = 30;

		// Token: 0x04001F37 RID: 7991
		protected float[] FpsSamples;

		// Token: 0x04001F38 RID: 7992
		protected int SampleIndex;

		// Token: 0x04001F39 RID: 7993
		protected int TextUpdateIndex;

		// Token: 0x04001F3A RID: 7994
		private float _fps;

		// Token: 0x04001F3B RID: 7995
		private int _sysLastSysTick;

		// Token: 0x04001F3C RID: 7996
		private int _sysLastFrameRate;

		// Token: 0x04001F3D RID: 7997
		private int _sysFrameRate;
	}
}
