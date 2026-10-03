using System;
using UnityEngine;

// Token: 0x0200004C RID: 76
public class FlyCamera : MonoBehaviour
{
	// Token: 0x06000173 RID: 371 RVA: 0x000106D8 File Offset: 0x0000E8D8
	private void Update()
	{
		if (Input.GetMouseButtonDown(1))
		{
			this.isRotating = true;
		}
		if (Input.GetMouseButtonUp(1))
		{
			this.isRotating = false;
		}
		if (this.isRotating)
		{
			float y = base.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * this.mouseSensitivity;
			this.rotationY += Input.GetAxis("Mouse Y") * this.mouseSensitivity;
			this.rotationY = Mathf.Clamp(this.rotationY, -90f, 90f);
			base.transform.localEulerAngles = new Vector3(-this.rotationY, y, 0f);
		}
		Vector3 vector = this.GetBaseInput();
		if (Input.GetKey(KeyCode.LeftShift))
		{
			this.totalRun += Time.deltaTime;
			vector = vector * this.totalRun * this.shiftAdd;
			vector.x = Mathf.Clamp(vector.x, -this.maxShift, this.maxShift);
			vector.y = Mathf.Clamp(vector.y, -this.maxShift, this.maxShift);
			vector.z = Mathf.Clamp(vector.z, -this.maxShift, this.maxShift);
			this.speedMultiplier = this.totalRun * this.shiftAdd * Time.deltaTime;
			this.speedMultiplier = Mathf.Clamp(this.speedMultiplier, -this.maxShift, this.maxShift);
		}
		else
		{
			this.totalRun = Mathf.Clamp(this.totalRun * 0.5f, 1f, 1000f);
			vector *= this.mainSpeed;
			this.speedMultiplier = this.mainSpeed * Time.deltaTime;
		}
		vector *= Time.deltaTime;
		Vector3 position = base.transform.position;
		base.transform.Translate(vector);
		position.x = base.transform.position.x;
		position.z = base.transform.position.z;
		if (Input.GetKey(KeyCode.Q))
		{
			position.y += -this.speedMultiplier;
		}
		if (Input.GetKey(KeyCode.E))
		{
			position.y += this.speedMultiplier;
		}
		base.transform.position = position;
	}

	// Token: 0x06000174 RID: 372 RVA: 0x0001092D File Offset: 0x0000EB2D
	public bool amIRotating()
	{
		return this.isRotating;
	}

	// Token: 0x06000175 RID: 373 RVA: 0x00010938 File Offset: 0x0000EB38
	private Vector3 GetBaseInput()
	{
		Vector3 vector = default(Vector3);
		if (Input.GetKey(KeyCode.W))
		{
			vector += new Vector3(0f, 0f, 1f);
		}
		if (Input.GetKey(KeyCode.S))
		{
			vector += new Vector3(0f, 0f, -1f);
		}
		if (Input.GetKey(KeyCode.A))
		{
			vector += new Vector3(-1f, 0f, 0f);
		}
		if (Input.GetKey(KeyCode.D))
		{
			vector += new Vector3(1f, 0f, 0f);
		}
		return vector;
	}

	// Token: 0x04000446 RID: 1094
	public float mainSpeed = 100f;

	// Token: 0x04000447 RID: 1095
	public float shiftAdd = 250f;

	// Token: 0x04000448 RID: 1096
	public float maxShift = 1000f;

	// Token: 0x04000449 RID: 1097
	public float camSens = 0.25f;

	// Token: 0x0400044A RID: 1098
	private Vector3 lastMouse = new Vector3(255f, 255f, 255f);

	// Token: 0x0400044B RID: 1099
	private float totalRun = 1f;

	// Token: 0x0400044C RID: 1100
	private bool isRotating;

	// Token: 0x0400044D RID: 1101
	private float speedMultiplier;

	// Token: 0x0400044E RID: 1102
	public float mouseSensitivity = 5f;

	// Token: 0x0400044F RID: 1103
	private float rotationY;
}
