using System;
using UnityEngine;

// Token: 0x0200018F RID: 399
public class carCamera : MonoBehaviour
{
	// Token: 0x060009D4 RID: 2516 RVA: 0x00086D21 File Offset: 0x00084F21
	private void Start()
	{
		this.raycastLayers = ~this.ignoreLayers;
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x00086D3C File Offset: 0x00084F3C
	private void Update()
	{
		if (this.car1.controlled)
		{
			Vector3 velocity = this.target.root.GetComponent<Rigidbody>().velocity;
			if ((double)this.target.root.GetComponent<Rigidbody>().velocity.magnitude < 0.01)
			{
				this.velocityDamping = 0f;
			}
			else
			{
				this.velocityDamping = 3f;
			}
			this.currentVelocity = Vector3.Lerp(this.prevVelocity, velocity, this.velocityDamping * Time.deltaTime);
			this.currentVelocity.y = 0f;
			this.prevVelocity = this.currentVelocity;
		}
	}

	// Token: 0x060009D6 RID: 2518 RVA: 0x00086DEC File Offset: 0x00084FEC
	private void LateUpdate()
	{
		if (this.car1.controlled && Camera.main != null)
		{
			float num = Mathf.Clamp01(this.target.root.GetComponent<Rigidbody>().velocity.magnitude / 60f);
			if (num < 0.01f)
			{
				num = 0.01f;
			}
			Camera.main.fieldOfView = Mathf.Lerp(40f, 65f, num);
			float num2 = Mathf.Lerp(7.5f, 6.5f, num);
			this.currentVelocity = this.currentVelocity.normalized;
			Vector3 vector = this.target.position + Vector3.up * this.height;
			Vector3 vector2 = vector - this.currentVelocity * num2;
			vector2.y = vector.y;
			Vector3 direction = vector2 - vector;
			if (Physics.Raycast(vector, direction, out this.hit, num2, this.raycastLayers))
			{
				vector2 = this.hit.point;
			}
			Camera.main.transform.position = vector2;
			Camera.main.transform.LookAt(vector);
			return;
		}
		if (Camera.main == null)
		{
			Debug.Log("please add a camera to the scene with tag 'MainCamera' ");
		}
	}

	// Token: 0x04001B35 RID: 6965
	public car car1;

	// Token: 0x04001B36 RID: 6966
	public Transform target;

	// Token: 0x04001B37 RID: 6967
	public float height = 1f;

	// Token: 0x04001B38 RID: 6968
	public float positionDamping = 3f;

	// Token: 0x04001B39 RID: 6969
	public float velocityDamping = 3f;

	// Token: 0x04001B3A RID: 6970
	public float distance = 4f;

	// Token: 0x04001B3B RID: 6971
	public LayerMask ignoreLayers = -1;

	// Token: 0x04001B3C RID: 6972
	private RaycastHit hit;

	// Token: 0x04001B3D RID: 6973
	private Vector3 prevVelocity = Vector3.zero;

	// Token: 0x04001B3E RID: 6974
	private LayerMask raycastLayers = -1;

	// Token: 0x04001B3F RID: 6975
	private Vector3 currentVelocity = Vector3.zero;
}
