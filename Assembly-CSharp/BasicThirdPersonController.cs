using System;
using UnityEngine;

// Token: 0x02000147 RID: 327
public class BasicThirdPersonController : MonoBehaviour
{
	// Token: 0x06000859 RID: 2137 RVA: 0x0006DAC0 File Offset: 0x0006BCC0
	private void Update()
	{
		if (this.grounded)
		{
			this.moveDirection = new Vector3(0f, 0f, Input.GetAxis("Vertical"));
			this.moveDirection = base.transform.TransformDirection(this.moveDirection);
			if (Input.GetButton("Jump"))
			{
				this.moveDirection *= this.runSpeed;
			}
			else
			{
				this.moveDirection *= this.speed;
			}
		}
		this.moveDirection.y = this.moveDirection.y - this.gravity * Time.deltaTime;
		base.GetComponent<CharacterController>().Move(this.moveDirection * Time.deltaTime);
		base.transform.Rotate(0f, this.rotateSpeed * Time.deltaTime * Input.GetAxis("Horizontal"), 0f);
		this.grounded = true;
	}

	// Token: 0x04001351 RID: 4945
	public float speed = 6f;

	// Token: 0x04001352 RID: 4946
	public float runSpeed = 9f;

	// Token: 0x04001353 RID: 4947
	public float rotateSpeed = 90f;

	// Token: 0x04001354 RID: 4948
	public float gravity = 20f;

	// Token: 0x04001355 RID: 4949
	private Vector3 moveDirection = Vector3.zero;

	// Token: 0x04001356 RID: 4950
	private bool grounded;
}
