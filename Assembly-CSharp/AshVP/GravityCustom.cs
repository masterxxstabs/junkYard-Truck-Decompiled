using System;
using UnityEngine;

namespace AshVP
{
	// Token: 0x0200033C RID: 828
	public class GravityCustom : MonoBehaviour
	{
		// Token: 0x0600153A RID: 5434 RVA: 0x000DFE9D File Offset: 0x000DE09D
		private void Start()
		{
			this.rb = base.GetComponent<Rigidbody>();
			this.rb.useGravity = false;
		}

		// Token: 0x0600153B RID: 5435 RVA: 0x000DFEB7 File Offset: 0x000DE0B7
		private void FixedUpdate()
		{
			this.rb.AddForce(Vector3.up * this.gravity * this.rb.mass);
		}

		// Token: 0x040025F8 RID: 9720
		private Rigidbody rb;

		// Token: 0x040025F9 RID: 9721
		public float gravity = -30f;
	}
}
