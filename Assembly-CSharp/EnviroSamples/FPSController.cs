using System;
using UnityEngine;

namespace EnviroSamples
{
	// Token: 0x020002D4 RID: 724
	public class FPSController : MonoBehaviour
	{
		// Token: 0x06001397 RID: 5015 RVA: 0x000CC603 File Offset: 0x000CA803
		private void Start()
		{
			this.player = base.GetComponent<CharacterController>();
		}

		// Token: 0x06001398 RID: 5016 RVA: 0x000CC614 File Offset: 0x000CA814
		private void Update()
		{
			this.moveFB = Input.GetAxis("Vertical") * this.speed;
			this.moveLR = Input.GetAxis("Horizontal") * this.speed;
			this.rotX = Input.GetAxis("Mouse X") * this.sensitivity;
			this.rotY -= Input.GetAxis("Mouse Y") * this.sensitivity;
			this.rotY = Mathf.Clamp(this.rotY, -60f, 60f);
			Vector3 vector = new Vector3(this.moveLR, 0f, this.moveFB);
			base.transform.Rotate(0f, this.rotX, 0f);
			this.eyes.transform.localRotation = Quaternion.Euler(this.rotY, 0f, 0f);
			vector = base.transform.rotation * vector;
			vector.y -= 4000f * Time.deltaTime;
			this.player.Move(vector * Time.deltaTime);
		}

		// Token: 0x04002430 RID: 9264
		public float speed = 2f;

		// Token: 0x04002431 RID: 9265
		public float sensitivity = 2f;

		// Token: 0x04002432 RID: 9266
		private CharacterController player;

		// Token: 0x04002433 RID: 9267
		public GameObject eyes;

		// Token: 0x04002434 RID: 9268
		private float moveFB;

		// Token: 0x04002435 RID: 9269
		private float moveLR;

		// Token: 0x04002436 RID: 9270
		private float rotX;

		// Token: 0x04002437 RID: 9271
		private float rotY;
	}
}
