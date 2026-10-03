using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Michsky.UI.Dark
{
	// Token: 0x02000318 RID: 792
	public class MainPanelButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x06001455 RID: 5205 RVA: 0x000DA40C File Offset: 0x000D860C
		private void Start()
		{
			this.buttonAnimator = base.GetComponent<Animator>();
		}

		// Token: 0x06001456 RID: 5206 RVA: 0x000DA41C File Offset: 0x000D861C
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (!this.buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hover to Pressed"))
			{
				this.buttonAnimator.Play("Hover");
			}
		}

		// Token: 0x06001457 RID: 5207 RVA: 0x000DA454 File Offset: 0x000D8654
		public void OnPointerExit(PointerEventData eventData)
		{
			if (!this.buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hover to Pressed"))
			{
				this.buttonAnimator.Play("Hover to Normal");
			}
		}

		// Token: 0x040024B9 RID: 9401
		private Animator buttonAnimator;
	}
}
