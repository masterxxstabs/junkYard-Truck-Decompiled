using System;
using UnityEngine;

namespace NWH.WheelController3D
{
	// Token: 0x0200024D RID: 589
	public class APITest : MonoBehaviour
	{
		// Token: 0x06000EF4 RID: 3828 RVA: 0x000B23B8 File Offset: 0x000B05B8
		private void FixedUpdate()
		{
			this.brakeTorque = this.wheel.brakeTorque;
			this.center = this.wheel.center;
			this.forwardFriction = this.wheel.forwardFriction;
			this.isGrounded = this.wheel.isGrounded;
			this.mass = this.wheel.mass;
			this.motorTorque = this.wheel.motorTorque;
			this.radius = this.wheel.radius;
			this.rpm = this.wheel.rpm;
			this.sidewaysFriction = this.wheel.sideFriction;
			this.steerAngle = this.wheel.steerAngle;
			this.suspensionDistance = this.wheel.suspensionDistance;
			WheelHit wheelHit;
			this.wheel.GetGroundHit(out wheelHit);
			this.wheel.GetWorldPose(out this.position, out this.rotation);
		}

		// Token: 0x04001F5C RID: 8028
		public float brakeTorque;

		// Token: 0x04001F5D RID: 8029
		public Vector3 center;

		// Token: 0x04001F5E RID: 8030
		public Friction forwardFriction;

		// Token: 0x04001F5F RID: 8031
		public WheelHit hit;

		// Token: 0x04001F60 RID: 8032
		public bool isGrounded;

		// Token: 0x04001F61 RID: 8033
		public float mass;

		// Token: 0x04001F62 RID: 8034
		public float motorTorque;

		// Token: 0x04001F63 RID: 8035
		public Vector3 position;

		// Token: 0x04001F64 RID: 8036
		public float radius;

		// Token: 0x04001F65 RID: 8037
		public Quaternion rotation;

		// Token: 0x04001F66 RID: 8038
		public float rpm;

		// Token: 0x04001F67 RID: 8039
		public Friction sidewaysFriction;

		// Token: 0x04001F68 RID: 8040
		public float steerAngle;

		// Token: 0x04001F69 RID: 8041
		public float suspensionDistance;

		// Token: 0x04001F6A RID: 8042
		public WheelController wheel;
	}
}
