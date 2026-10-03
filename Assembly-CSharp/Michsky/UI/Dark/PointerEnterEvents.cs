using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Michsky.UI.Dark
{
	// Token: 0x0200031E RID: 798
	public class PointerEnterEvents : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x06001471 RID: 5233 RVA: 0x000DAEF1 File Offset: 0x000D90F1
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.enterEvent.Invoke();
		}

		// Token: 0x06001472 RID: 5234 RVA: 0x000DAEFE File Offset: 0x000D90FE
		public void OnPointerExit(PointerEventData eventData)
		{
			this.exitEvent.Invoke();
		}

		// Token: 0x040024DC RID: 9436
		[Header("EVENTS")]
		public UnityEvent enterEvent;

		// Token: 0x040024DD RID: 9437
		public UnityEvent exitEvent;
	}
}
