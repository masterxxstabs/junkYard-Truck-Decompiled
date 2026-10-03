using System;
using UnityEngine;

namespace AshVP
{
	// Token: 0x02000333 RID: 819
	public class ReferencesAI : MonoBehaviour
	{
		// Token: 0x060014FD RID: 5373 RVA: 0x000DE42A File Offset: 0x000DC62A
		private void OnEnable()
		{
			base.transform.GetComponent<VehicleEditorAI>().enabled = false;
		}

		// Token: 0x060014FE RID: 5374 RVA: 0x000DE43D File Offset: 0x000DC63D
		private void OnDisable()
		{
			base.transform.GetComponent<VehicleEditorAI>().enabled = true;
		}

		// Token: 0x04002592 RID: 9618
		public Transform GroundRayPt;

		// Token: 0x04002593 RID: 9619
		public Transform wheels;

		// Token: 0x04002594 RID: 9620
		public Transform wheelFL;

		// Token: 0x04002595 RID: 9621
		public Transform wheelFR;

		// Token: 0x04002596 RID: 9622
		public Transform wheelRL;

		// Token: 0x04002597 RID: 9623
		public Transform wheelRR;
	}
}
