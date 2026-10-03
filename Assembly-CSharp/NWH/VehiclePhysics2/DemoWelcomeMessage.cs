using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NWH.VehiclePhysics2
{
	// Token: 0x0200025A RID: 602
	public class DemoWelcomeMessage : MonoBehaviour
	{
		// Token: 0x06000FD4 RID: 4052 RVA: 0x000B8A98 File Offset: 0x000B6C98
		private void Start()
		{
			if (!Application.isEditor)
			{
				base.gameObject.SetActive(true);
			}
			this.closeButton.onClick.AddListener(new UnityAction(this.Close));
		}

		// Token: 0x06000FD5 RID: 4053 RVA: 0x000B8AC9 File Offset: 0x000B6CC9
		private void Close()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x0400207A RID: 8314
		public Button closeButton;
	}
}
