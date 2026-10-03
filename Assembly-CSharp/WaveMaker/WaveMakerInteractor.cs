using System;
using UnityEngine;

namespace WaveMaker
{
	// Token: 0x020001A6 RID: 422
	public class WaveMakerInteractor : MonoBehaviour
	{
		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000A3F RID: 2623 RVA: 0x0008B022 File Offset: 0x00089222
		// (set) Token: 0x06000A40 RID: 2624 RVA: 0x0008B02A File Offset: 0x0008922A
		public Vector3 LinearVelocity { get; private set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000A41 RID: 2625 RVA: 0x0008B033 File Offset: 0x00089233
		// (set) Token: 0x06000A42 RID: 2626 RVA: 0x0008B03B File Offset: 0x0008923B
		public Vector3 AngularVelocity { get; private set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000A43 RID: 2627 RVA: 0x0008B044 File Offset: 0x00089244
		public Vector3 CenterOfMass
		{
			get
			{
				if (!this.usesRigidBody)
				{
					return base.transform.position;
				}
				return base.transform.TransformPoint(this.rb.centerOfMass);
			}
		}

		// Token: 0x06000A44 RID: 2628 RVA: 0x0008B070 File Offset: 0x00089270
		private void Awake()
		{
			this._lastPosition = base.transform.position;
			this._lastRotation = base.transform.rotation;
			this.UpdateRigidBodyStatus();
			foreach (MeshCollider meshCollider in base.GetComponents<MeshCollider>())
			{
				if (!meshCollider.convex)
				{
					meshCollider.enabled = false;
					Debug.LogError("WaveMaker - (" + base.gameObject.name + ") has a mesh collider that is not convex. Mesh colliders are slow in contact, but it will slow down any WaveMaker surface it touches even more. Disabled!");
				}
			}
		}

		// Token: 0x06000A45 RID: 2629 RVA: 0x0008B0EC File Offset: 0x000892EC
		private void FixedUpdate()
		{
			if (this.showVelocities)
			{
				this.UpdateVelocities();
				Debug.DrawRay(base.transform.position, this.LinearVelocity, Color.red);
				Debug.DrawRay(base.transform.position, this.AngularVelocity, Color.blue);
			}
		}

		// Token: 0x06000A46 RID: 2630 RVA: 0x0008B140 File Offset: 0x00089340
		public void UpdateVelocities()
		{
			Vector3 linearVelocity = this.LinearVelocity;
			if (this.usesRigidBody)
			{
				this.LinearVelocity = this.rb.velocity;
				this.AngularVelocity = this.rb.angularVelocity;
			}
			else
			{
				this.LinearVelocity = (base.transform.position - this._lastPosition) / Time.fixedDeltaTime;
				this._lastPosition = base.transform.position;
				this.AngularVelocity = WaveMakerUtils.GetAngularVelocity(this._lastRotation, base.transform.rotation);
				this._lastRotation = base.transform.rotation;
			}
			if (this.speedDampening)
			{
				this.LinearVelocity = Vector3.Lerp(linearVelocity, this.LinearVelocity, 1f - this.speedDampValue);
			}
		}

		// Token: 0x06000A47 RID: 2631 RVA: 0x0008B20A File Offset: 0x0008940A
		public void UpdateRigidBodyStatus()
		{
			this.rb = base.GetComponent<Rigidbody>();
			this.usesRigidBody = (this.rb != null && !this.rb.isKinematic);
		}

		// Token: 0x04001C40 RID: 7232
		[Tooltip("This will make velocity values change softer, making the response of the WaveMaker object softer too. Disable for efficiency gain")]
		public bool speedDampening;

		// Token: 0x04001C41 RID: 7233
		[Tooltip("Higher value means slower velocity change")]
		[Range(0f, 1f)]
		public float speedDampValue;

		// Token: 0x04001C42 RID: 7234
		[Tooltip("Shows the linear and angular velocies in the scene view during play")]
		public bool showVelocities;

		// Token: 0x04001C43 RID: 7235
		private Vector3 _lastPosition;

		// Token: 0x04001C44 RID: 7236
		private Quaternion _lastRotation;

		// Token: 0x04001C45 RID: 7237
		private Rigidbody rb;

		// Token: 0x04001C46 RID: 7238
		private bool usesRigidBody;
	}
}
