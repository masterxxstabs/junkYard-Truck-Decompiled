using System;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Dark
{
	// Token: 0x02000322 RID: 802
	public class ScrollGamepadManager : MonoBehaviour
	{
		// Token: 0x06001490 RID: 5264 RVA: 0x000DB644 File Offset: 0x000D9844
		private void Update()
		{
			float axis = Input.GetAxis(this.inputAxis);
			if (!this.invertAxis)
			{
				if (axis == 1f)
				{
					this.scrollbarObject.value -= this.changeValue;
					return;
				}
				if (axis == -1f)
				{
					this.scrollbarObject.value += this.changeValue;
					return;
				}
			}
			else
			{
				if (axis == 1f)
				{
					this.scrollbarObject.value += this.changeValue;
					return;
				}
				if (axis == -1f)
				{
					this.scrollbarObject.value -= this.changeValue;
				}
			}
		}

		// Token: 0x040024F6 RID: 9462
		[Header("SLIDER")]
		public Scrollbar scrollbarObject;

		// Token: 0x040024F7 RID: 9463
		public float changeValue = 0.05f;

		// Token: 0x040024F8 RID: 9464
		[Header("INPUT")]
		public string inputAxis = "Xbox Right Stick Vertical";

		// Token: 0x040024F9 RID: 9465
		public bool invertAxis;
	}
}
