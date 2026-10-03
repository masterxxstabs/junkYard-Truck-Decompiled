using System;
using UnityEngine;

namespace WaveMaker
{
	// Token: 0x020001A3 RID: 419
	[RequireComponent(typeof(Rigidbody))]
	public class WaveMaker_SampleScene_Ship_Mover : MonoBehaviour
	{
		// Token: 0x06000A2D RID: 2605 RVA: 0x0008A987 File Offset: 0x00088B87
		private void Start()
		{
			this.rb = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06000A2E RID: 2606 RVA: 0x0008A998 File Offset: 0x00088B98
		private void Update()
		{
			if (Input.GetKey(KeyCode.UpArrow))
			{
				this.rb.AddForce(base.transform.localToWorldMatrix.MultiplyVector(Vector3.forward * this.frontThrust), ForceMode.Force);
			}
			else if (Input.GetKey(KeyCode.DownArrow))
			{
				this.rb.AddForce(base.transform.localToWorldMatrix.MultiplyVector(Vector3.back * this.backThrust), ForceMode.Force);
			}
			if (Input.GetKey(KeyCode.RightArrow))
			{
				this.rb.AddTorque(base.transform.localToWorldMatrix.MultiplyVector(Vector3.up * this.sideTorque), ForceMode.Force);
			}
			else if (Input.GetKey(KeyCode.LeftArrow))
			{
				this.rb.AddTorque(base.transform.localToWorldMatrix.MultiplyVector(-Vector3.up * this.sideTorque), ForceMode.Force);
			}
			if (this.rb.angularVelocity.magnitude > this.maxVelocity)
			{
				this.rb.angularVelocity = this.rb.angularVelocity.normalized * this.maxVelocity;
			}
			if (this.rb.velocity.magnitude > this.maxVelocity)
			{
				this.rb.velocity = this.rb.velocity.normalized * this.maxVelocity;
			}
		}

		// Token: 0x04001C2A RID: 7210
		private Rigidbody rb;

		// Token: 0x04001C2B RID: 7211
		public float frontThrust = 1f;

		// Token: 0x04001C2C RID: 7212
		public float backThrust = 0.5f;

		// Token: 0x04001C2D RID: 7213
		public float sideTorque = 1f;

		// Token: 0x04001C2E RID: 7214
		public float maxVelocity = 6f;
	}
}
