using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.Rigging
{
	// Token: 0x02000291 RID: 657
	[Serializable]
	public class Bone
	{
		// Token: 0x060011C3 RID: 4547 RVA: 0x000C2E92 File Offset: 0x000C1092
		public void Initialize()
		{
			this._initDistance = Vector3.Distance(this.thisTransform.position, this.targetTransform.position);
			this._initZScale = this.thisTransform.localScale.z;
		}

		// Token: 0x060011C4 RID: 4548 RVA: 0x000C2ECC File Offset: 0x000C10CC
		public void Update(Vector3 forward, Vector3 up)
		{
			if (this.doubleSided)
			{
				Vector3 position = (this.targetTransform.position + this.targetTransformB.position) / 2f;
				this.thisTransform.position = position;
				this.thisTransform.LookAt(this.targetTransform, up);
				return;
			}
			if (this.lookAtTarget)
			{
				Vector3 eulerAngles = Quaternion.LookRotation(this.targetTransform.position - this.thisTransform.position, up).eulerAngles;
				this.thisTransform.rotation = Quaternion.Euler(eulerAngles);
			}
			if (this.stretchToTarget && this._initDistance != 0f)
			{
				float z = Vector3.Distance(this.thisTransform.position, this.targetTransform.position) / this._initDistance * this._initZScale;
				Vector3 localScale = this.thisTransform.localScale;
				this.thisTransform.localScale = new Vector3(localScale.x, localScale.y, z);
			}
		}

		// Token: 0x040021F3 RID: 8691
		[Tooltip("    Should the object be positioned between two points. Also affects position besides rotation.")]
		public bool doubleSided;

		// Token: 0x040021F4 RID: 8692
		[Tooltip("    Should the object be rotated to look at the target?")]
		public bool lookAtTarget = true;

		// Token: 0x040021F5 RID: 8693
		[Tooltip("    Should the object be stretched between pivot and target?")]
		public bool stretchToTarget = true;

		// Token: 0x040021F6 RID: 8694
		[Tooltip("    The transform that represents the lookAtTarget and stretch target.")]
		public Transform targetTransform;

		// Token: 0x040021F7 RID: 8695
		[Tooltip("    Second target. Object will be stretched positioned between targetTransform and targetTransformB if doubleSided is\r\n    true.")]
		public Transform targetTransformB;

		// Token: 0x040021F8 RID: 8696
		[Tooltip("    The transform that represents the bone.")]
		public Transform thisTransform;

		// Token: 0x040021F9 RID: 8697
		private float _initDistance;

		// Token: 0x040021FA RID: 8698
		private float _initZScale;
	}
}
