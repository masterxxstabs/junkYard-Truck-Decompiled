using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Michsky.UI.Dark
{
	// Token: 0x0200031C RID: 796
	public class PanelTabButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x06001466 RID: 5222 RVA: 0x000DA9BF File Offset: 0x000D8BBF
		private void Start()
		{
			this.buttonAnimator = base.gameObject.GetComponent<Animator>();
		}

		// Token: 0x06001467 RID: 5223 RVA: 0x000DA9D4 File Offset: 0x000D8BD4
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (!this.buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hover to Pressed"))
			{
				this.buttonAnimator.Play("Normal to Hover");
			}
		}

		// Token: 0x06001468 RID: 5224 RVA: 0x000DAA0C File Offset: 0x000D8C0C
		public void OnPointerExit(PointerEventData eventData)
		{
			if (!this.buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hover to Pressed"))
			{
				this.buttonAnimator.Play("Hover to Normal");
			}
		}

		// Token: 0x040024CB RID: 9419
		private Animator buttonAnimator;
	}
}
