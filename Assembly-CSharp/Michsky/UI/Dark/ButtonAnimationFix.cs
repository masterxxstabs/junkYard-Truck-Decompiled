using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Michsky.UI.Dark
{
	// Token: 0x02000310 RID: 784
	public class ButtonAnimationFix : MonoBehaviour
	{
		// Token: 0x06001436 RID: 5174 RVA: 0x000D9897 File Offset: 0x000D7A97
		private void Start()
		{
			this.fixButton = base.gameObject.GetComponent<Button>();
			this.fixButton.onClick.AddListener(new UnityAction(this.Fix));
		}

		// Token: 0x06001437 RID: 5175 RVA: 0x000D98C6 File Offset: 0x000D7AC6
		public void Fix()
		{
			this.fixButton.gameObject.SetActive(false);
			this.fixButton.gameObject.SetActive(true);
		}

		// Token: 0x04002480 RID: 9344
		private Button fixButton;
	}
}
