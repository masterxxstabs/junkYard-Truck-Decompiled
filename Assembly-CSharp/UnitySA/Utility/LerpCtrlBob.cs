using System;
using System.Collections;
using UnityEngine;

namespace UnitySA.Utility
{
	// Token: 0x020001AD RID: 429
	[Serializable]
	public class LerpCtrlBob
	{
		// Token: 0x06000A92 RID: 2706 RVA: 0x0008D3B0 File Offset: 0x0008B5B0
		public float Offset()
		{
			return this.m_Offset;
		}

		// Token: 0x06000A93 RID: 2707 RVA: 0x0008D3B8 File Offset: 0x0008B5B8
		public IEnumerator DoBobCycle()
		{
			float t = 0f;
			while (t < this.BobDuration)
			{
				this.m_Offset = Mathf.Lerp(0f, this.BobAmount, t / this.BobDuration);
				t += Time.deltaTime;
				yield return new WaitForFixedUpdate();
			}
			t = 0f;
			while (t < this.BobDuration)
			{
				this.m_Offset = Mathf.Lerp(this.BobAmount, 0f, t / this.BobDuration);
				t += Time.deltaTime;
				yield return new WaitForFixedUpdate();
			}
			this.m_Offset = 0f;
			yield break;
		}

		// Token: 0x04001CA4 RID: 7332
		public float BobDuration;

		// Token: 0x04001CA5 RID: 7333
		public float BobAmount;

		// Token: 0x04001CA6 RID: 7334
		private float m_Offset;
	}
}
