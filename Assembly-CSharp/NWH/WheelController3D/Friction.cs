using System;
using NWH.VehiclePhysics2.Demo;
using UnityEngine;

namespace NWH.WheelController3D
{
	// Token: 0x0200024A RID: 586
	[Serializable]
	public class Friction
	{
		// Token: 0x06000EE8 RID: 3816 RVA: 0x000B1E64 File Offset: 0x000B0064
		public void Initialize()
		{
			this.Ki = 0.08f * (0.02f / Time.fixedDeltaTime);
			this.Kp = 0.26f * (0.02f / Time.fixedDeltaTime);
		}

		// Token: 0x06000EE9 RID: 3817 RVA: 0x000B1E94 File Offset: 0x000B0094
		public static void CalculateLateralSlip(float dt, float velocityMagnitude, float angularVelocity, float loadCoefficient, float forwardSpeed, ref FrictionPreset frictionPreset, ref Friction friction, bool hasHit, out float surfaceForce)
		{
			surfaceForce = 0f;
			float num = friction.speed;
			float x = (forwardSpeed < 0f) ? (-forwardSpeed) : forwardSpeed;
			float num2 = (angularVelocity < 0f) ? (-angularVelocity) : angularVelocity;
			if (hasHit)
			{
				if (velocityMagnitude < 0.35f && num2 < 1f)
				{
					friction.PI_error = friction.speed;
					friction.PI_integral += friction.PI_error;
					friction.slip = friction.Kp * friction.PI_error + friction.Ki * friction.PI_integral;
					friction.slip = ((friction.slip < -1f) ? -1f : ((friction.slip > 1f) ? 1f : friction.slip));
				}
				else
				{
					if (velocityMagnitude < 0.8f && num2 < 6f)
					{
						friction.slip = num * 0.25f;
					}
					else
					{
						friction.slip = Mathf.Atan2(num, x) * 57.29578f / 80f;
					}
					friction.PI_error = 0f;
					friction.PI_integral = 0f;
				}
				friction.slip *= friction.slipCoefficient;
				friction.slip = ((friction.slip < -1f) ? -1f : ((friction.slip > 1f) ? 1f : friction.slip));
				float time = (friction.slip < 0f) ? (-friction.slip) : friction.slip;
				float num3 = (friction.slip < 0f) ? -1f : 1f;
				float num4 = frictionPreset.Curve.Evaluate(time);
				surfaceForce = num3 * num4 * loadCoefficient * friction.forceCoefficient;
			}
		}

		// Token: 0x06000EEA RID: 3818 RVA: 0x000B208C File Offset: 0x000B028C
		public static float CalculateLongitudinalSlip(float torque, float brakeTorque, float dragTorque, float wheelRadius, float wheelInertia, float dt, float fixedDeltaTime, float loadCoefficient, float BCDEz, ref Friction friction, ref float angularVelocity, ref float outSurfaceTorque)
		{
			float num = friction.speed;
			float num2 = (friction.speed < 0f) ? (-friction.speed) : friction.speed;
			float num3 = angularVelocity;
			angularVelocity += torque / wheelInertia * dt;
			brakeTorque += dragTorque;
			brakeTorque *= ((angularVelocity > 0f) ? -1f : 1f);
			float num4 = ((angularVelocity < 0f) ? (-angularVelocity) : angularVelocity) * wheelInertia / dt;
			brakeTorque = ((brakeTorque > num4) ? num4 : ((brakeTorque < -num4) ? (-num4) : brakeTorque));
			angularVelocity += brakeTorque / wheelInertia * dt;
			float num5 = num / wheelRadius;
			float num6 = (angularVelocity - num5) * wheelInertia / dt;
			float num7 = loadCoefficient * BCDEz * friction.forceCoefficient * 0.8f;
			float num8 = (num6 < -num7) ? (-num7) : ((num6 > num7) ? num7 : num6);
			float num9 = 0.8f;
			if (num2 > num9)
			{
				friction.slip = (num - angularVelocity * wheelRadius) / num2;
			}
			else
			{
				float num10 = num - angularVelocity * wheelRadius;
				friction.slip = 2f * num10 / (num9 + num * num / num9);
			}
			friction.slip *= friction.slipCoefficient;
			friction.slip = ((friction.slip < -1f) ? -1f : ((friction.slip > 1f) ? 1f : friction.slip));
			angularVelocity -= num8 / wheelInertia * dt;
			float num11 = (angularVelocity - num3) * wheelInertia / dt;
			outSurfaceTorque += num8 * (dt / fixedDeltaTime);
			return (-num8 + brakeTorque - num11) * 0.9f;
		}

		// Token: 0x04001F43 RID: 8003
		[ShowInTelemetry]
		[Tooltip("    Current force in friction direction.")]
		public float force;

		// Token: 0x04001F44 RID: 8004
		[Tooltip("    Modifies force value.")]
		public float forceCoefficient = 1f;

		// Token: 0x04001F45 RID: 8005
		public float PI_error;

		// Token: 0x04001F46 RID: 8006
		[ShowInTelemetry]
		[Tooltip("    Current slip in friction direction.")]
		public float slip;

		// Token: 0x04001F47 RID: 8007
		[Tooltip("    Modifies slip value.")]
		public float slipCoefficient = 1f;

		// Token: 0x04001F48 RID: 8008
		[Tooltip("    Speed at the point of contact with the surface.")]
		public float speed;

		// Token: 0x04001F49 RID: 8009
		[SerializeField]
		private float Ki = 0.06f;

		// Token: 0x04001F4A RID: 8010
		[SerializeField]
		private float Kp = 0.16f;

		// Token: 0x04001F4B RID: 8011
		private float PI_integral;
	}
}
