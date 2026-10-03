using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NWH.VehiclePhysics2.Input
{
	// Token: 0x020002C0 RID: 704
	public class MobileInputButton : Button
	{
		// Token: 0x0600130B RID: 4875 RVA: 0x000C9CAC File Offset: 0x000C7EAC
		private void LateUpdate()
		{
			this.hasBeenClicked = false;
		}

		// Token: 0x0600130C RID: 4876 RVA: 0x000C9CB5 File Offset: 0x000C7EB5
		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);
			this.isPressed = true;
			this.hasBeenClicked = true;
		}

		// Token: 0x0600130D RID: 4877 RVA: 0x000C9CCC File Offset: 0x000C7ECC
		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);
			this.isPressed = false;
		}

		// Token: 0x0400236B RID: 9067
		public bool hasBeenClicked;

		// Token: 0x0400236C RID: 9068
		public bool isPressed;
	}
}
