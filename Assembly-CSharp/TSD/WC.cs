using System;
using TSD.uTireRuntime;
using UnityEngine;

namespace TSD
{
	// Token: 0x0200033F RID: 831
	[Serializable]
	public class WC : WheelBase, IWheel
	{
		// Token: 0x170001FC RID: 508
		// (get) Token: 0x06001551 RID: 5457 RVA: 0x000DFF19 File Offset: 0x000DE119
		// (set) Token: 0x06001552 RID: 5458 RVA: 0x000DFF21 File Offset: 0x000DE121
		public override Object wheelObject
		{
			get
			{
				return this.wheelCollider;
			}
			set
			{
				base.m_wheelColliderBase = value;
				this.wheelCollider = (WheelCollider)value;
			}
		}

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x06001553 RID: 5459 RVA: 0x000DFF36 File Offset: 0x000DE136
		public new Transform wheelTransform
		{
			get
			{
				return this.wheelCollider.transform;
			}
		}

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x06001554 RID: 5460 RVA: 0x000DFF43 File Offset: 0x000DE143
		public float suspensionDistance
		{
			get
			{
				return this.wheelCollider.suspensionDistance;
			}
		}

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x06001555 RID: 5461 RVA: 0x000DFF50 File Offset: 0x000DE150
		// (set) Token: 0x06001556 RID: 5462 RVA: 0x000DFF5D File Offset: 0x000DE15D
		public float steerAngle
		{
			get
			{
				return this.wheelCollider.steerAngle;
			}
			set
			{
				this.wheelCollider.steerAngle = value;
			}
		}

		// Token: 0x06001557 RID: 5463 RVA: 0x000DFF6B File Offset: 0x000DE16B
		public void GetWorldPose(out Vector3 pos, out Quaternion quat)
		{
			this.wheelCollider.GetWorldPose(out pos, out quat);
		}

		// Token: 0x06001558 RID: 5464 RVA: 0x000DFF7A File Offset: 0x000DE17A
		public bool GetGroundHit(out WheelHit hit)
		{
			return this.wheelCollider.GetGroundHit(out hit);
		}

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x06001559 RID: 5465 RVA: 0x000DFF88 File Offset: 0x000DE188
		// (set) Token: 0x0600155A RID: 5466 RVA: 0x000DFF95 File Offset: 0x000DE195
		public float radius
		{
			get
			{
				return this.wheelCollider.radius;
			}
			set
			{
				this.wheelCollider.radius = value;
			}
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x0600155B RID: 5467 RVA: 0x000DFFA3 File Offset: 0x000DE1A3
		public bool isGrounded
		{
			get
			{
				return this.wheelCollider.isGrounded;
			}
		}

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x0600155C RID: 5468 RVA: 0x000DFF43 File Offset: 0x000DE143
		public float targetDistance
		{
			get
			{
				return this.wheelCollider.suspensionDistance;
			}
		}

		// Token: 0x0600155D RID: 5469 RVA: 0x000DFFB0 File Offset: 0x000DE1B0
		public Vector3 GetGroundHitPoint()
		{
			this.GetGroundHit(out uTireManager.wHit);
			return uTireManager.wHit.point;
		}

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x0600155E RID: 5470 RVA: 0x000DFFC8 File Offset: 0x000DE1C8
		public float springCompression
		{
			get
			{
				return Mathf.Clamp(-this.wheelTransform.InverseTransformPoint(this.GetGroundHitPoint()).y - this.radius, 0f, this.targetDistance) / (this.targetDistance + Mathf.Epsilon);
			}
		}

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x0600155F RID: 5471 RVA: 0x000E0005 File Offset: 0x000DE205
		public Vector3 center
		{
			get
			{
				return this.wheelCollider.center;
			}
		}

		// Token: 0x040025FC RID: 9724
		[SerializeField]
		private WheelCollider wheelCollider;
	}
}
