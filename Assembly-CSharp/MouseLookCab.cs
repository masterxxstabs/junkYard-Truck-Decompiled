using System;
using UnityEngine;

// Token: 0x020000FD RID: 253
public class MouseLookCab : MonoBehaviour
{
	// Token: 0x0600067D RID: 1661 RVA: 0x0004DD30 File Offset: 0x0004BF30
	private void Start()
	{
		this.currentCameraIndex = 0;
		for (int i = 1; i < this.cameras.Length; i++)
		{
			this.cameras[i].gameObject.SetActive(false);
		}
		if (this.cameras.Length != 0)
		{
			this.cameras[0].gameObject.SetActive(true);
			Debug.Log("Camera with name: " + this.cameras[0].GetComponent<Camera>().name + ", is now enabled");
		}
		Cursor.lockState = CursorLockMode.Locked;
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x0004DDB4 File Offset: 0x0004BFB4
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			this.currentCameraIndex++;
			Debug.Log("C button has been pressed. Switching to the next camera");
			if (this.currentCameraIndex < this.cameras.Length)
			{
				this.cameras[this.currentCameraIndex - 1].gameObject.SetActive(false);
				this.cameras[this.currentCameraIndex].gameObject.SetActive(true);
				Debug.Log("Camera with name: " + this.cameras[this.currentCameraIndex].GetComponent<Camera>().name + ", is now enabled");
			}
			else
			{
				this.cameras[this.currentCameraIndex - 1].gameObject.SetActive(false);
				this.currentCameraIndex = 0;
				this.cameras[this.currentCameraIndex].gameObject.SetActive(true);
				Debug.Log("Camera with name: " + this.cameras[this.currentCameraIndex].GetComponent<Camera>().name + ", is now enabled");
			}
		}
		float num = Input.GetAxis("Mouse X") * this.mouseSensitivity * Time.deltaTime;
		float num2 = Input.GetAxis("Mouse Y") * this.mouseSensitivity * Time.deltaTime;
		this.xRotation -= num2;
		this.xRotation = Mathf.Clamp(this.xRotation, -90f, 90f);
		this.yRotation -= num;
		this.yRotation = Mathf.Clamp(this.yRotation, -90f, 90f);
		base.transform.localRotation = Quaternion.Euler(this.xRotation, 0f, 0f);
		this.playerBody.Rotate(Vector3.up * num);
	}

	// Token: 0x04000DA1 RID: 3489
	public float mouseSensitivity = 100f;

	// Token: 0x04000DA2 RID: 3490
	public Transform playerBody;

	// Token: 0x04000DA3 RID: 3491
	private float xRotation;

	// Token: 0x04000DA4 RID: 3492
	private float yRotation;

	// Token: 0x04000DA5 RID: 3493
	public Camera[] cameras;

	// Token: 0x04000DA6 RID: 3494
	private int currentCameraIndex;
}
