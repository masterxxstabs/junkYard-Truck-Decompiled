using System;
using UnityEngine;
using UnityEngine.Events;

namespace Michsky.UI.Dark
{
	// Token: 0x0200031F RID: 799
	public class PressKeyEvent : MonoBehaviour
	{
		// Token: 0x06001474 RID: 5236 RVA: 0x000DAF0B File Offset: 0x000D910B
		private void Update()
		{
			if (this.pressAnyKey)
			{
				if (Input.anyKeyDown)
				{
					this.pressAction.Invoke();
					return;
				}
			}
			else if (Input.GetKeyDown(this.hotkey))
			{
				this.pressAction.Invoke();
			}
		}

		// Token: 0x040024DE RID: 9438
		[Header("KEY")]
		[SerializeField]
		public KeyCode hotkey;

		// Token: 0x040024DF RID: 9439
		public bool pressAnyKey;

		// Token: 0x040024E0 RID: 9440
		[Header("KEY ACTION")]
		[SerializeField]
		public UnityEvent pressAction;
	}
}
