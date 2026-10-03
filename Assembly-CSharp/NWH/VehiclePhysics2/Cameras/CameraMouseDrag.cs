using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NWH.VehiclePhysics2.Cameras
{
	// Token: 0x020002C9 RID: 713
	public class CameraMouseDrag : VehicleCamera
	{
		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x06001350 RID: 4944 RVA: 0x000B9CCD File Offset: 0x000B7ECD
		private bool PointerOverUI
		{
			get
			{
				return EventSystem.current.IsPointerOverGameObject();
			}
		}

		// Token: 0x06001351 RID: 4945 RVA: 0x000CAF14 File Offset: 0x000C9114
		private void Start()
		{
			this.distance = Mathf.Clamp(this.distance, this.minDistance, this.maxDistance);
			this._rot.x = this.initXRotation;
			this._rot.y = this.initYRotation;
			this._isFirstFrame = false;
		}

		// Token: 0x06001352 RID: 4946 RVA: 0x000CAF68 File Offset: 0x000C9168
		private void LateUpdate()
		{
			if (this.target == null)
			{
				return;
			}
			if (!this.PointerOverUI)
			{
				float num = 0.02f;
				if (this.allowRotation && Input.GetMouseButton(0))
				{
					this._rot.y = this._rot.y + Input.GetAxis("Mouse X") * this.rotationSensitivity.x;
					this._rot.x = this._rot.x - Input.GetAxis("Mouse Y") * this.rotationSensitivity.y;
				}
				if (this.allowPanning && Input.GetMouseButton(1))
				{
					float d = Input.GetAxis("Mouse X") * this.panningSensitivity.x;
					float d2 = Input.GetAxis("Mouse Y") * this.panningSensitivity.y;
					this._pan -= this.targetTransform.InverseTransformDirection(base.transform.right * d);
					this._pan -= this.targetTransform.InverseTransformDirection(base.transform.up * d2);
				}
				this._rot.x = this.ClampAngle(this._rot.x, this.verticalMinAngle, this.verticalMaxAngle);
				if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > num)
				{
					this.distance -= Input.GetAxis("Mouse ScrollWheel") * this.zoomSensitivity;
				}
			}
			this.distance = Mathf.Clamp(this.distance, this.minDistance, this.maxDistance);
			Vector3 point = this.followTargetsRotation ? this.targetTransform.forward : Vector3.forward;
			Vector3 axis = this.followTargetsRotation ? this.targetTransform.up : Vector3.up;
			Vector3 axis2 = this.followTargetsRotation ? this.targetTransform.right : Vector3.right;
			this._lookAtPosition = this.targetTransform.position + this.targetTransform.TransformDirection(this.targetPositionOffset + this._pan);
			this._newLookDir = Quaternion.AngleAxis(this._rot.x, axis2) * point;
			this._newLookDir = Quaternion.AngleAxis(this._rot.y, axis) * this._newLookDir;
			if (this._isFirstFrame)
			{
				this._lookDir = this._newLookDir;
				this._isFirstFrame = false;
			}
			else
			{
				this._lookDir = Vector3.SmoothDamp(this._lookDir, this._newLookDir, ref this._lookDirVel, this.rotationSmoothing);
			}
			base.transform.position = this._lookAtPosition - this._lookDir * this.distance;
			base.transform.forward = this._lookDir;
			if (!this.followTargetsRotation)
			{
				base.transform.LookAt(this._lookAtPosition);
			}
		}

		// Token: 0x06001353 RID: 4947 RVA: 0x000CB259 File Offset: 0x000C9459
		public void OnDrawGizmosSelected()
		{
			Gizmos.DrawSphere(this._lookAtPosition, 0.5f);
			Gizmos.DrawRay(this._lookAtPosition, this._lookDir);
		}

		// Token: 0x06001354 RID: 4948 RVA: 0x000CB27C File Offset: 0x000C947C
		public float ClampAngle(float angle, float min, float max)
		{
			while (angle < -360f || angle > 360f)
			{
				if (angle < -360f)
				{
					angle += 360f;
				}
				if (angle > 360f)
				{
					angle -= 360f;
				}
			}
			return Mathf.Clamp(angle, min, max);
		}

		// Token: 0x040023B8 RID: 9144
		[Tooltip("Can the camera be rotated by the user?")]
		public bool allowRotation = true;

		// Token: 0x040023B9 RID: 9145
		[Tooltip("Can the camera be panned by the user?")]
		public bool allowPanning = true;

		// Token: 0x040023BA RID: 9146
		[Range(0f, 100f)]
		[Tooltip("    Distance from target at which camera will be positioned. Might vary depending on smoothing.")]
		public float distance = 5f;

		// Token: 0x040023BB RID: 9147
		[Tooltip("    If true the camera will rotate with the vehicle.")]
		public bool followTargetsRotation;

		// Token: 0x040023BC RID: 9148
		[Range(0f, 100f)]
		[Tooltip("    Maximum distance that will be reached when zooming out.")]
		public float maxDistance = 13f;

		// Token: 0x040023BD RID: 9149
		[Range(0f, 100f)]
		[Tooltip("    Minimum distance that will be reached when zooming in.")]
		public float minDistance = 3f;

		// Token: 0x040023BE RID: 9150
		[Range(0f, 15f)]
		[Tooltip("    Sensitivity of the middle mouse button / wheel.")]
		public float zoomSensitivity = 8f;

		// Token: 0x040023BF RID: 9151
		[Range(0f, 1f)]
		[Tooltip("    Smoothing of the camera rotation.")]
		public float rotationSmoothing = 0.25f;

		// Token: 0x040023C0 RID: 9152
		[Range(-90f, 90f)]
		[Tooltip("Maximum vertical angle the camera can achieve.")]
		public float verticalMaxAngle = 80f;

		// Token: 0x040023C1 RID: 9153
		[Range(-90f, 90f)]
		[Tooltip("Minimum vertical angle the camera can achieve.")]
		public float verticalMinAngle = -40f;

		// Token: 0x040023C2 RID: 9154
		[Tooltip("Sensitivity of rotation input.")]
		public Vector2 rotationSensitivity = new Vector2(5f, 5f);

		// Token: 0x040023C3 RID: 9155
		[Tooltip("Sensitivity of panning input.")]
		public Vector2 panningSensitivity = new Vector2(0.06f, 0.06f);

		// Token: 0x040023C4 RID: 9156
		[Tooltip("Initial rotation around the X axis (up/down)")]
		public float initXRotation;

		// Token: 0x040023C5 RID: 9157
		[Tooltip("Initial rotation around the Y axis (left/right)")]
		public float initYRotation;

		// Token: 0x040023C6 RID: 9158
		[Tooltip("Look position offset from the target center.")]
		public Vector3 targetPositionOffset = Vector3.zero;

		// Token: 0x040023C7 RID: 9159
		private Vector3 _lookDir;

		// Token: 0x040023C8 RID: 9160
		private Vector3 _newLookDir;

		// Token: 0x040023C9 RID: 9161
		private Vector3 _lookDirVel;

		// Token: 0x040023CA RID: 9162
		private Vector3 _camPosVel;

		// Token: 0x040023CB RID: 9163
		private Vector3 _lookAtPosition;

		// Token: 0x040023CC RID: 9164
		private Vector2 _rot;

		// Token: 0x040023CD RID: 9165
		private Vector3 _pan;

		// Token: 0x040023CE RID: 9166
		private Quaternion _lookAngle;

		// Token: 0x040023CF RID: 9167
		private bool _isFirstFrame;
	}
}
