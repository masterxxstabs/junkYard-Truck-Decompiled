using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Michsky.UI.Dark
{
	// Token: 0x02000327 RID: 807
	public class TimedEvent : MonoBehaviour
	{
		// Token: 0x060014A0 RID: 5280 RVA: 0x000DBD2A File Offset: 0x000D9F2A
		private void Start()
		{
			if (this.enableAtStart)
			{
				base.StartCoroutine("TimedEventStart");
			}
		}

		// Token: 0x060014A1 RID: 5281 RVA: 0x000DBD40 File Offset: 0x000D9F40
		private IEnumerator TimedEventStart()
		{
			yield return new WaitForSeconds(this.timer);
			this.timerAction.Invoke();
			yield break;
		}

		// Token: 0x060014A2 RID: 5282 RVA: 0x000DBD4F File Offset: 0x000D9F4F
		public void StartIEnumerator()
		{
			base.StartCoroutine("TimedEventStart");
		}

		// Token: 0x060014A3 RID: 5283 RVA: 0x000DBD5D File Offset: 0x000D9F5D
		public void StopIEnumerator()
		{
			base.StopCoroutine("TimedEventStart");
		}

		// Token: 0x04002517 RID: 9495
		[Header("TIMING (SECONDS)")]
		public float timer;

		// Token: 0x04002518 RID: 9496
		public bool enableAtStart;

		// Token: 0x04002519 RID: 9497
		[Header("TIMER EVENT")]
		public UnityEvent timerAction;
	}
}
