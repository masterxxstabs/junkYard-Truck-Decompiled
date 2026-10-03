using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Cameras
{
	// Token: 0x020002CC RID: 716
	public class VehicleCamera : MonoBehaviour
	{
		// Token: 0x0600135D RID: 4957 RVA: 0x000CB60C File Offset: 0x000C980C
		public virtual void Awake()
		{
			if (this.target == null)
			{
				this.target = base.transform.GetComponentInParent<VehicleController>();
				if (this.target == null)
				{
					Debug.LogError("No parent object of VehicleCamera " + base.name + " has VehicleController component. Make sure the VehicleCamera is a child of a vehicle.");
				}
			}
			this.targetTransform = this.target.transform;
		}

		// Token: 0x040023E1 RID: 9185
		[Tooltip("Vehicle Controller that this script is targeting. Can be left empty if head movement is not being used.")]
		public VehicleController target;

		// Token: 0x040023E2 RID: 9186
		[Tooltip("Transform of the target object.")]
		public Transform targetTransform;
	}
}
