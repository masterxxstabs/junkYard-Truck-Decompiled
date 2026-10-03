using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
	// Token: 0x020002B7 RID: 695
	public class SkidmarkDestroy : MonoBehaviour
	{
		// Token: 0x06001284 RID: 4740 RVA: 0x000C6F3B File Offset: 0x000C513B
		private void Start()
		{
			base.InvokeRepeating("Check", Random.Range(1f, 2f), 1f);
		}

		// Token: 0x06001285 RID: 4741 RVA: 0x000C6F5C File Offset: 0x000C515C
		private void Check()
		{
			if (this.targetTransform == null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			if (!this.skidmarkIsBeingUsed && Vector3.Distance(base.transform.position, this.targetTransform.position) > this.distanceThreshold)
			{
				Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x040022FD RID: 8957
		[Tooltip("    Distance at which the GameObject will be destroyed.")]
		public float distanceThreshold = 100f;

		// Token: 0x040022FE RID: 8958
		public bool skidmarkIsBeingUsed;

		// Token: 0x040022FF RID: 8959
		[Tooltip("    Transform to which the object belongs to.")]
		public Transform targetTransform;
	}
}
