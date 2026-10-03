using System;
using UnityEngine;

namespace AshVP
{
	// Token: 0x0200033A RID: 826
	public class AshSuspension : MonoBehaviour
	{
		// Token: 0x0600152F RID: 5423 RVA: 0x000DF9B4 File Offset: 0x000DDBB4
		private void Awake()
		{
			base.transform.position = this.wheel.position;
			Vector3 localPosition = base.transform.localPosition;
			localPosition.y = 0f;
			base.transform.localPosition = localPosition;
			this.suspensionRestDist = Vector3.Distance(base.transform.position, this.wheel.position);
			this.wheelRestDistance = Vector3.Distance(base.transform.position, this.wheel.position);
			if (this.wheel.GetComponent<ConfigurableJoint>())
			{
				Object.Destroy(this.wheel.GetComponent<ConfigurableJoint>());
			}
			if (this.wheel.GetComponent<SphereCollider>())
			{
				this.wheelRadius = this.wheel.GetComponent<SphereCollider>().radius;
				Object.Destroy(this.wheel.GetComponent<SphereCollider>());
			}
			if (this.wheel.GetComponent<Rigidbody>())
			{
				Object.Destroy(this.wheel.GetComponent<Rigidbody>());
			}
		}

		// Token: 0x06001530 RID: 5424 RVA: 0x000DFAB9 File Offset: 0x000DDCB9
		private void FixedUpdate()
		{
			if (this.grounded())
			{
				this.rayDidHit = true;
			}
			else
			{
				this.rayDidHit = false;
			}
			this.SuspensionLogic();
		}

		// Token: 0x06001531 RID: 5425 RVA: 0x000DFADC File Offset: 0x000DDCDC
		public void SuspensionLogic()
		{
			if (this.rayDidHit)
			{
				Vector3 up = base.transform.up;
				Vector3 pointVelocity = this.carRigidBody.GetPointVelocity(base.transform.position);
				float num = this.suspensionRestDist - this.hit.distance;
				float num2 = -Vector3.Dot(up, pointVelocity) / Time.fixedDeltaTime;
				float num3 = Vector3.Dot(up, pointVelocity);
				float d = num * this.springStrength - num3 * this.springDamper;
				this.carRigidBody.AddForceAtPosition(up * d, base.transform.position);
			}
			if (this.rayDidHit)
			{
				this.wheel.position = base.transform.position - base.transform.up * this.hit.distance;
				return;
			}
			this.wheel.position = Vector3.Lerp(this.wheel.position, base.transform.position - base.transform.up * this.wheelRestDistance, 0.1f);
		}

		// Token: 0x06001532 RID: 5426 RVA: 0x000DFBF0 File Offset: 0x000DDDF0
		public bool grounded()
		{
			Vector3 direction = -base.transform.up;
			return Physics.SphereCast(base.transform.position, this.wheelRadius, direction, out this.hit, this.suspensionRestDist);
		}

		// Token: 0x06001533 RID: 5427 RVA: 0x000DFC38 File Offset: 0x000DDE38
		private void OnDrawGizmos()
		{
			if (Application.isPlaying)
			{
				Gizmos.color = Color.green;
				if (this.rayDidHit)
				{
					Gizmos.DrawWireSphere(base.transform.position - base.transform.up * this.hit.distance, this.wheelRadius);
					Gizmos.DrawLine(base.transform.position, this.hit.point);
					return;
				}
				Gizmos.color = Color.red;
				Gizmos.DrawLine(base.transform.position, this.wheel.position);
				Gizmos.DrawWireSphere(this.wheel.position, this.wheelRadius);
			}
		}

		// Token: 0x040025EA RID: 9706
		private float suspensionRestDist;

		// Token: 0x040025EB RID: 9707
		private float wheelRadius;

		// Token: 0x040025EC RID: 9708
		public float springStrength = 40000f;

		// Token: 0x040025ED RID: 9709
		public float springDamper = 2000f;

		// Token: 0x040025EE RID: 9710
		public bool rayDidHit;

		// Token: 0x040025EF RID: 9711
		private RaycastHit hit;

		// Token: 0x040025F0 RID: 9712
		public Rigidbody carRigidBody;

		// Token: 0x040025F1 RID: 9713
		public Transform wheel;

		// Token: 0x040025F2 RID: 9714
		private float wheelRestDistance;
	}
}
