using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.VehiclePhysics2.Cameras
{
	// Token: 0x020002CA RID: 714
	public class CameraOnboard : VehicleCamera
	{
		// Token: 0x06001356 RID: 4950 RVA: 0x000CB35F File Offset: 0x000C955F
		public override void Awake()
		{
			base.Awake();
			this._targetTransform = this.target.transform;
			this._initialPosition = this._targetTransform.InverseTransformPoint(base.transform.position);
		}

		// Token: 0x06001357 RID: 4951 RVA: 0x000CB394 File Offset: 0x000C9594
		private void LateUpdate()
		{
			base.transform.position = this._targetTransform.TransformPoint(this._initialPosition);
			this._localAcceleration = Vector3.zero;
			if (this.target != null)
			{
				this._localAcceleration = this._targetTransform.TransformDirection(this.target.Acceleration);
			}
			this._newPositionOffset = Vector3.SmoothDamp(this._prevAcceleration, this._localAcceleration, ref this._accelerationChangeVelocity, this.movementSmoothing) / 100f * this.movementIntensity;
			this._newPositionOffset = Vector3.Scale(this._newPositionOffset, this.axisIntensity);
			this._positionOffset = Vector3.SmoothDamp(this._positionOffset, this._newPositionOffset, ref this._offsetChangeVelocity, this.movementSmoothing);
			this._positionOffset = Vector3.ClampMagnitude(this._positionOffset, this.maxMovementOffset);
			base.transform.position -= this._targetTransform.TransformDirection(this._positionOffset) * Mathf.Clamp01(this.target.Speed * 0.5f);
			if (this.target != null)
			{
				this._prevAcceleration = this.target.Acceleration;
			}
		}

		// Token: 0x040023D0 RID: 9168
		[FormerlySerializedAs("maxPositionOffsetMagnitude")]
		[Range(0f, 1f)]
		[Tooltip("    Maximum head movement from the initial position.")]
		public float maxMovementOffset = 0.2f;

		// Token: 0x040023D1 RID: 9169
		[FormerlySerializedAs("positionIntensity")]
		[Range(0f, 1f)]
		[Tooltip("    How much will the head move around for the given g-force.")]
		public float movementIntensity = 0.125f;

		// Token: 0x040023D2 RID: 9170
		[FormerlySerializedAs("positionSmoothing")]
		[Range(0f, 1f)]
		[Tooltip("    Smoothing of the head movement.")]
		public float movementSmoothing = 0.3f;

		// Token: 0x040023D3 RID: 9171
		[Tooltip("Movement intensity per axis. Set to 0 to disable movement on that axis or negative to reverse it.")]
		public Vector3 axisIntensity = new Vector3(1f, 0f, 1f);

		// Token: 0x040023D4 RID: 9172
		private Vector3 _accelerationChangeVelocity;

		// Token: 0x040023D5 RID: 9173
		private Vector3 _initialPosition;

		// Token: 0x040023D6 RID: 9174
		private Vector3 _localAcceleration;

		// Token: 0x040023D7 RID: 9175
		private Vector3 _newPositionOffset;

		// Token: 0x040023D8 RID: 9176
		private Vector3 _offsetChangeVelocity;

		// Token: 0x040023D9 RID: 9177
		private Vector3 _positionOffset;

		// Token: 0x040023DA RID: 9178
		private Vector3 _prevAcceleration;

		// Token: 0x040023DB RID: 9179
		private Transform _targetTransform;
	}
}
