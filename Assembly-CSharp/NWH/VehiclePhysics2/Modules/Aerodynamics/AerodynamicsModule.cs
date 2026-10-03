using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Demo;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.Aerodynamics
{
	// Token: 0x020002A2 RID: 674
	[Serializable]
	public class AerodynamicsModule : VehicleModule
	{
		// Token: 0x0600120F RID: 4623 RVA: 0x000B61C6 File Offset: 0x000B43C6
		public override void Initialize()
		{
			this.initialized = true;
		}

		// Token: 0x06001210 RID: 4624 RVA: 0x000C3DA4 File Offset: 0x000C1FA4
		public override void FixedUpdate()
		{
			if (!base.Active)
			{
				return;
			}
			if (this.vc.Speed < 1f)
			{
				this.longitudinalDragForce = 0f;
				this.lateralDragForce = 0f;
				return;
			}
			if (this.simulateDrag)
			{
				this._frontalArea = this.vc.vehicleDimensions.x * this.vc.vehicleDimensions.y * 0.85f;
				this._sideArea = this.vc.vehicleDimensions.y * this.vc.vehicleDimensions.z * 0.8f;
				this._forwardSpeed = this.vc.LocalVelocity.z;
				this._sideSpeed = this.vc.LocalVelocity.x;
				float num = this.vc.damageHandler.IsEnabled ? (1f + this.vc.damageHandler.Damage * this.damageDragEffect) : 1f;
				this.longitudinalDragForce = 0.6125f * this._frontalArea * this.frontalCd * (this._forwardSpeed * this._forwardSpeed) * ((this._forwardSpeed > 0f) ? -1f : 1f) * num;
				this.lateralDragForce = 0.6125f * this._sideArea * this.sideCd * (this._sideSpeed * this._sideSpeed) * ((this._sideSpeed > 0f) ? -1f : 1f);
				this.vc.vehicleRigidbody.AddRelativeForce(new Vector3(this.lateralDragForce, 0f, this.longitudinalDragForce));
			}
			if (this.simulateDownforce)
			{
				float f = this.vc.Speed / this.maxDownforceSpeed;
				float num2 = 1f - (1f - Mathf.Pow(f, 2f));
				foreach (DownforcePoint downforcePoint in this.downforcePoints)
				{
					this.vc.vehicleRigidbody.AddForceAtPosition(num2 * downforcePoint.maxForce * -this.vc.transform.up, this.vc.transform.TransformPoint(downforcePoint.position));
				}
			}
		}

		// Token: 0x06001211 RID: 4625 RVA: 0x00002188 File Offset: 0x00000388
		public override void Update()
		{
		}

		// Token: 0x06001212 RID: 4626 RVA: 0x000C4018 File Offset: 0x000C2218
		public override void OnDrawGizmosSelected(VehicleController vc)
		{
			foreach (DownforcePoint downforcePoint in this.downforcePoints)
			{
				Gizmos.color = Color.red;
				vc.transform.TransformPoint(downforcePoint.position);
				Gizmos.DrawSphere(vc.transform.TransformPoint(downforcePoint.position), 0.1f);
			}
		}

		// Token: 0x06001213 RID: 4627 RVA: 0x000C409C File Offset: 0x000C229C
		public override VehicleModule.ModuleCategory GetModuleCategory()
		{
			return VehicleModule.ModuleCategory.Aero;
		}

		// Token: 0x04002238 RID: 8760
		public const float RHO = 1.225f;

		// Token: 0x04002239 RID: 8761
		[Range(0f, 5f)]
		[Tooltip("The amount of drag that will be added when the vehicle is fully damaged.\r\n0.5 equals +50% on top of the original, undamaged, drag value.")]
		public float damageDragEffect = 0.5f;

		// Token: 0x0400223A RID: 8762
		[Tooltip("Points at which downforce will be applied.\r\nAvoid applying force at too high positions as that will negatively influence suspension and steering.")]
		public List<DownforcePoint> downforcePoints = new List<DownforcePoint>();

		// Token: 0x0400223B RID: 8763
		[Range(0f, 1f)]
		[ShowInSettings("Frontal Cd", 0f, 2f, 0.1f)]
		[Tooltip("    Coefficient of drag of the vehicle's frontal profile.\r\n    Also used for reverse.")]
		public float frontalCd = 0.35f;

		// Token: 0x0400223C RID: 8764
		[Tooltip("Speed in [m/s] at which the downforce will reach it's maximum value\r\nassigned under downforce points settings.")]
		public float maxDownforceSpeed = 80f;

		// Token: 0x0400223D RID: 8765
		[Range(0f, 2f)]
		[ShowInSettings("Side Cd", 0f, 2f, 0.1f)]
		[Tooltip("    Coefficient of drag of the vehicle's side profile.")]
		public float sideCd = 1.05f;

		// Token: 0x0400223E RID: 8766
		[ShowInSettings("Simulate Downforce")]
		[Tooltip("    Should downforce be calculated?")]
		public bool simulateDownforce;

		// Token: 0x0400223F RID: 8767
		[ShowInSettings("Simulate Drag")]
		[Tooltip("    Should drag be calculated?")]
		public bool simulateDrag = true;

		// Token: 0x04002240 RID: 8768
		private float _forwardSpeed;

		// Token: 0x04002241 RID: 8769
		private float _frontalArea;

		// Token: 0x04002242 RID: 8770
		private float _sideArea;

		// Token: 0x04002243 RID: 8771
		private float _sideSpeed;

		// Token: 0x04002244 RID: 8772
		[SerializeField]
		private float lateralDragForce;

		// Token: 0x04002245 RID: 8773
		[SerializeField]
		private float longitudinalDragForce;
	}
}
