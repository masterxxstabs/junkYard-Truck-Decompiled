using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Michsky.UI.Dark
{
	// Token: 0x02000314 RID: 788
	public class GamepadTriggerEvent : MonoBehaviour
	{
		// Token: 0x0600144B RID: 5195 RVA: 0x000DA260 File Offset: 0x000D8460
		private void Update()
		{
			float axisRaw = Input.GetAxisRaw(this.leftTriggerInput);
			float axisRaw2 = Input.GetAxisRaw(this.rightTriggerInput);
			if (axisRaw == 1f && !this.canClick)
			{
				base.StartCoroutine("TimedEvent");
			}
			if (axisRaw2 == 1f && !this.canClick)
			{
				base.StartCoroutine("TimedEvent");
			}
			if (axisRaw == 1f && this.canClick)
			{
				this.leftTriggerEvent.Invoke();
				this.canClick = false;
			}
			if (axisRaw2 == 1f && this.canClick)
			{
				this.rightTriggerEvent.Invoke();
				this.canClick = false;
			}
		}

		// Token: 0x0600144C RID: 5196 RVA: 0x000DA2FF File Offset: 0x000D84FF
		private IEnumerator TimedEvent()
		{
			yield return new WaitForSeconds(this.cooldownTimer);
			this.canClick = true;
			base.StopCoroutine("TimedEvent");
			yield break;
		}

		// Token: 0x040024AC RID: 9388
		[Header("EVENTS")]
		public UnityEvent leftTriggerEvent;

		// Token: 0x040024AD RID: 9389
		public UnityEvent rightTriggerEvent;

		// Token: 0x040024AE RID: 9390
		public float cooldownTimer = 0.25f;

		// Token: 0x040024AF RID: 9391
		[Header("INPUT")]
		public string leftTriggerInput = "Xbox Left Trigger";

		// Token: 0x040024B0 RID: 9392
		public string rightTriggerInput = "Xbox Right Trigger";

		// Token: 0x040024B1 RID: 9393
		private bool canClick = true;
	}
}
