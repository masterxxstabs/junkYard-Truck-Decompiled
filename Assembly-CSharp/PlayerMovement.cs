using System;
using UnityEngine;

// Token: 0x02000116 RID: 278
public class PlayerMovement : MonoBehaviour
{
	// Token: 0x06000748 RID: 1864 RVA: 0x0005EA5C File Offset: 0x0005CC5C
	private void Update()
	{
		float axis = Input.GetAxis("Horizontal");
		float axis2 = Input.GetAxis("Vertical");
		Vector3 a = base.transform.right * axis + base.transform.forward * axis2;
		this.controller.Move(a * this.speed * Time.deltaTime);
		this.velocity.y = this.velocity.y + this.gravity * Time.deltaTime;
		this.controller.Move(this.velocity * Time.deltaTime);
	}

	// Token: 0x0400104B RID: 4171
	public CharacterController controller;

	// Token: 0x0400104C RID: 4172
	public float speed = 12f;

	// Token: 0x0400104D RID: 4173
	public float gravity = -9.81f;

	// Token: 0x0400104E RID: 4174
	private Vector3 velocity;
}
