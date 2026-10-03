using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Powertrain;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.FlipOver
{
	// Token: 0x0200029C RID: 668
	[Serializable]
	public class FlipOverModule : VehicleModule
	{
		// Token: 0x060011F6 RID: 4598 RVA: 0x000B61C6 File Offset: 0x000B43C6
		public override void Initialize()
		{
			this.initialized = true;
		}

		// Token: 0x060011F7 RID: 4599 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x060011F8 RID: 4600 RVA: 0x000C3744 File Offset: 0x000C1944
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			this._vehicleAngle = Vector3.Angle(this.vc.transform.up, -Physics.gravity.normalized);
			int num = 0;
			using (List<WheelComponent>.Enumerator enumerator = this.vc.Wheels.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsGrounded)
					{
						num++;
					}
				}
			}
			if (this.vc.Speed < this.maxDetectionSpeed && this._vehicleAngle > this.allowedAngle && (float)num <= (float)this.vc.Wheels.Count / 2f)
			{
				this._timeSinceFlip += this.vc.fixedDeltaTime;
				if (this._timeSinceFlip > this.timeout)
				{
					this.flippedOver = true;
				}
			}
			else
			{
				this._timeAfterRecovery += this.vc.fixedDeltaTime;
				if (this._timeAfterRecovery > 1f || this._vehicleAngle < 45f)
				{
					this.flippedOver = false;
					this._timeSinceFlip = 0f;
					this._timeAfterRecovery = 0f;
				}
			}
			if (this.manual)
			{
				try
				{
					if (Input.GetButtonDown("FlipOver") && this.flippedOver)
					{
						this.flipOverInput = true;
					}
				}
				catch
				{
					Debug.LogError("Flip over is set to manual but 'FlipOverModule' input binding is not set. Either disable manual flip over or set 'FlipOverModule' binding.");
				}
			}
			if (this.manual && this.flipOverInput)
			{
				this._manualFlipoverInProgress = true;
				this.flipOverInput = false;
			}
			if ((this.flippedOver && !this.manual) || (this.flippedOver && this.manual && this._manualFlipoverInProgress))
			{
				if (this._zAngle == 0f && this._xAngle == 0f)
				{
					Vector3 eulerAngles = this.vc.vehicleTransform.eulerAngles;
					this._xAngle = eulerAngles.x * Time.smoothDeltaTime;
					this._zAngle = eulerAngles.z * Time.smoothDeltaTime;
				}
				this.vc.vehicleRigidbody.MoveRotation(this.vc.transform.rotation * Quaternion.Euler(this._xAngle, 0f, this._zAngle));
			}
			else if (this._wasFlippedOver && !this.flippedOver)
			{
				this.vc.vehicleRigidbody.constraints = RigidbodyConstraints.None;
				this._manualFlipoverInProgress = false;
				this._zAngle = (this._xAngle = 0f);
			}
			this._wasFlippedOver = this.flippedOver;
		}

		// Token: 0x060011F9 RID: 4601 RVA: 0x000C2B9B File Offset: 0x000C0D9B
		public override VehicleModule.ModuleCategory GetModuleCategory()
		{
			return VehicleModule.ModuleCategory.DrivingAssists;
		}

		// Token: 0x0400221A RID: 8730
		[Tooltip("    Minimum angle that the vehicle needs to be at for it to be detected as flipped over.")]
		public float allowedAngle = 70f;

		// Token: 0x0400221B RID: 8731
		public bool flipOverInput;

		// Token: 0x0400221C RID: 8732
		[Tooltip("    Is the vehicle flipped over?")]
		public bool flippedOver;

		// Token: 0x0400221D RID: 8733
		[Tooltip("If enabled a prompt will be shown after the timeout, asking player to press the FlipOverModule button.")]
		public bool manual;

		// Token: 0x0400221E RID: 8734
		[Tooltip("    Flip over detection will be disabled if velocity is above this value [m/s].")]
		public float maxDetectionSpeed = 1f;

		// Token: 0x0400221F RID: 8735
		[Tooltip("    Rotation speed of the vehicle while being flipped back.")]
		public float rotationSpeed = 1f;

		// Token: 0x04002220 RID: 8736
		[Tooltip("Time after detecting flip over after which vehicle will be flipped back or the manual button can be used.")]
		public float timeout = 3f;

		// Token: 0x04002221 RID: 8737
		private bool _manualFlipoverInProgress;

		// Token: 0x04002222 RID: 8738
		private float _xAngle;

		// Token: 0x04002223 RID: 8739
		private float _zAngle;

		// Token: 0x04002224 RID: 8740
		private float _timeAfterRecovery;

		// Token: 0x04002225 RID: 8741
		private float _timeSinceFlip;

		// Token: 0x04002226 RID: 8742
		private float _vehicleAngle;

		// Token: 0x04002227 RID: 8743
		private bool _wasFlippedOver;
	}
}
