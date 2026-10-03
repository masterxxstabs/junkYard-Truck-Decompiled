using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Cameras
{
	// Token: 0x020002C7 RID: 711
	public class CameraFollow : VehicleCamera
	{
		// Token: 0x0600134C RID: 4940 RVA: 0x000CAD90 File Offset: 0x000C8F90
		private void LateUpdate()
		{
			Vector3 current = this.targetForward;
			if (!this.firstFrame)
			{
				this.targetForward = Vector3.SmoothDamp(current, this.target.transform.forward, ref this.targetForwardVelocity, this.smoothing);
			}
			else
			{
				this.targetForward = this.target.transform.forward;
				this.firstFrame = false;
			}
			Vector3 vector = this.target.transform.position + this.targetForward * -this.distance + Vector3.up * this.height;
			RaycastHit raycastHit;
			if (Physics.Raycast(vector, -Vector3.up, out raycastHit, 0.8f))
			{
				vector = raycastHit.point + Vector3.up * 0.8f;
			}
			base.transform.position = vector;
			base.transform.LookAt(this.target.transform.position + Vector3.up * this.targetUpOffset + this.target.transform.forward * this.targetForwardOffset);
		}

		// Token: 0x0600134D RID: 4941 RVA: 0x000CAEC1 File Offset: 0x000C90C1
		private void OnEnable()
		{
			this.firstFrame = true;
		}

		// Token: 0x040023AF RID: 9135
		[Range(0f, 30f)]
		[Tooltip("    Distance at which camera will follow.")]
		public float distance = 5f;

		// Token: 0x040023B0 RID: 9136
		[Range(0f, 10f)]
		[Tooltip("    Height in relation to the target at which the camera will follow.")]
		public float height = 2.5f;

		// Token: 0x040023B1 RID: 9137
		[Range(0f, 1f)]
		[Tooltip("    Positional and rotational smoothing of the camera.")]
		public float smoothing = 0.2f;

		// Token: 0x040023B2 RID: 9138
		[Range(-10f, 10f)]
		[Tooltip("    Offset in the forward direction from the target. Use this if you do not want to use camera baits.")]
		public float targetForwardOffset;

		// Token: 0x040023B3 RID: 9139
		[Range(-5f, 5f)]
		[Tooltip("    Offset in the up direction from the target. Use this if you do not want to use camera baits.")]
		public float targetUpOffset = 1.25f;

		// Token: 0x040023B4 RID: 9140
		private bool firstFrame = true;

		// Token: 0x040023B5 RID: 9141
		private Vector3 targetForward;

		// Token: 0x040023B6 RID: 9142
		private Vector3 targetForwardVelocity;
	}
}
