using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NWH.VehiclePhysics2.Demo
{
	// Token: 0x0200025E RID: 606
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	public class RigidbodyFPSController : MonoBehaviour
	{
		// Token: 0x06000FF1 RID: 4081 RVA: 0x000B9C8D File Offset: 0x000B7E8D
		private void Awake()
		{
			this.rb = base.GetComponent<Rigidbody>();
			this.rb.freezeRotation = true;
			this.rb.useGravity = false;
		}

		// Token: 0x06000FF2 RID: 4082 RVA: 0x000B9CB3 File Offset: 0x000B7EB3
		private float CalculateJumpVerticalSpeed()
		{
			return Mathf.Sqrt(2f * this.jumpHeight * this.gravity);
		}

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x06000FF3 RID: 4083 RVA: 0x000B9CCD File Offset: 0x000B7ECD
		private bool PointerOverUI
		{
			get
			{
				return EventSystem.current.IsPointerOverGameObject();
			}
		}

		// Token: 0x06000FF4 RID: 4084 RVA: 0x000B9CDC File Offset: 0x000B7EDC
		private void LateUpdate()
		{
			if ((float)Time.frameCount < 10f)
			{
				return;
			}
			if (this.grounded)
			{
				Vector3 vector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
				vector = base.transform.TransformDirection(vector);
				vector *= this.speed;
				Vector3 velocity = this.rb.velocity;
				Vector3 vector2 = vector - velocity;
				vector2.x = Mathf.Clamp(vector2.x, -this.maxVelocityChange, this.maxVelocityChange);
				vector2.z = Mathf.Clamp(vector2.z, -this.maxVelocityChange, this.maxVelocityChange);
				vector2.y = 0f;
				this.rb.AddForce(vector2, ForceMode.VelocityChange);
				if (this.canJump && Input.GetKeyDown(KeyCode.Space))
				{
					this.rb.velocity = new Vector3(velocity.x, this.CalculateJumpVerticalSpeed(), velocity.z);
				}
			}
			if (!this.PointerOverUI)
			{
				float num = Time.deltaTime * 20f;
				float y = base.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * this.sensitivityX * num;
				this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY * num;
				this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);
				base.transform.localEulerAngles = new Vector3(-this.rotationY, y, 0f);
			}
			this.rb.AddForce(new Vector3(0f, -this.gravity * this.rb.mass, 0f));
			this.grounded = false;
		}

		// Token: 0x06000FF5 RID: 4085 RVA: 0x000B9EA6 File Offset: 0x000B80A6
		private void OnCollisionStay()
		{
			this.grounded = true;
		}

		// Token: 0x04002088 RID: 8328
		public bool canJump = true;

		// Token: 0x04002089 RID: 8329
		public float gravity = 10f;

		// Token: 0x0400208A RID: 8330
		public float jumpHeight = 2f;

		// Token: 0x0400208B RID: 8331
		public float maximumY = 60f;

		// Token: 0x0400208C RID: 8332
		public float maxVelocityChange = 10f;

		// Token: 0x0400208D RID: 8333
		public float minimumY = -60f;

		// Token: 0x0400208E RID: 8334
		public float sensitivityX = 15f;

		// Token: 0x0400208F RID: 8335
		public float sensitivityY = 15f;

		// Token: 0x04002090 RID: 8336
		public float speed = 10f;

		// Token: 0x04002091 RID: 8337
		private bool grounded;

		// Token: 0x04002092 RID: 8338
		private Rigidbody rb;

		// Token: 0x04002093 RID: 8339
		private float rotationY;
	}
}
