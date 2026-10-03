using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Michsky.UI.Dark
{
	// Token: 0x0200032C RID: 812
	public class VirtualCursorAnimate : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x060014C9 RID: 5321 RVA: 0x000DCDE3 File Offset: 0x000DAFE3
		private void Start()
		{
			if (this.virtualCursor == null)
			{
				Debug.Log("Looking for Virtual Cursor automatically.");
				this.virtualCursor = GameObject.Find("Virtual Cursor").GetComponent<VirtualCursor>();
			}
		}

		// Token: 0x060014CA RID: 5322 RVA: 0x000DCE12 File Offset: 0x000DB012
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.virtualCursor != null)
			{
				this.virtualCursor.AnimateCursorIn();
			}
		}

		// Token: 0x060014CB RID: 5323 RVA: 0x000DCE2D File Offset: 0x000DB02D
		public void OnPointerExit(PointerEventData eventData)
		{
			if (this.virtualCursor != null)
			{
				this.virtualCursor.AnimateCursorOut();
			}
		}

		// Token: 0x04002542 RID: 9538
		[Header("RESOURCES")]
		public VirtualCursor virtualCursor;
	}
}
