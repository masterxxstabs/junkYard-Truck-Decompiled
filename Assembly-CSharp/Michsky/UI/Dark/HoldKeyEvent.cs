using System;
using UnityEngine;
using UnityEngine.Events;

namespace Michsky.UI.Dark
{
	// Token: 0x02000315 RID: 789
	public class HoldKeyEvent : MonoBehaviour
	{
		// Token: 0x0600144E RID: 5198 RVA: 0x000DA340 File Offset: 0x000D8540
		private void Update()
		{
			if (Input.GetKey(this.hotkey))
			{
				this.isHolding = true;
				this.isOn = false;
			}
			else
			{
				this.isHolding = false;
				this.isOn = true;
			}
			if (this.isOn && !this.isHolding)
			{
				this.releaseAction.Invoke();
				this.isHolding = false;
				this.isOn = false;
				return;
			}
			if (!this.isOn && this.isHolding)
			{
				this.holdAction.Invoke();
				this.isHolding = true;
			}
		}

		// Token: 0x040024B2 RID: 9394
		[Header("KEY")]
		[SerializeField]
		public KeyCode hotkey;

		// Token: 0x040024B3 RID: 9395
		[Header("KEY ACTION")]
		[SerializeField]
		public UnityEvent holdAction;

		// Token: 0x040024B4 RID: 9396
		[SerializeField]
		public UnityEvent releaseAction;

		// Token: 0x040024B5 RID: 9397
		private bool isOn;

		// Token: 0x040024B6 RID: 9398
		private bool isHolding;
	}
}
