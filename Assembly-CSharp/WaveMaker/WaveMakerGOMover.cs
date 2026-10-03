using System;
using UnityEngine;

namespace WaveMaker
{
	// Token: 0x020001A5 RID: 421
	public class WaveMakerGOMover : MonoBehaviour
	{
		// Token: 0x06000A3C RID: 2620 RVA: 0x0008AEA6 File Offset: 0x000890A6
		private void Awake()
		{
			this.rb = base.GetComponent<Rigidbody>();
			this.isRb = (this.rb != null && !this.rb.isKinematic);
		}

		// Token: 0x06000A3D RID: 2621 RVA: 0x0008AEDC File Offset: 0x000890DC
		private void FixedUpdate()
		{
			WaveMakerGOMover.MovementType movementType = this.movementType;
			if (movementType != WaveMakerGOMover.MovementType.ComeAndGoTranslation)
			{
				if (movementType != WaveMakerGOMover.MovementType.Rotation)
				{
					return;
				}
				Vector3 vector = base.transform.TransformDirection(this.rotationSpeed * Time.fixedDeltaTime);
				if (this.isRb)
				{
					this.rb.AddTorque(vector.x, vector.y, vector.z, ForceMode.Force);
					return;
				}
				base.transform.Rotate(vector.x, vector.y, vector.z, Space.World);
				return;
			}
			else
			{
				Vector3 vector2 = new Vector3(this.translationDistance.x * Mathf.Sin(Time.time * this.translationSpeed.x), this.translationDistance.y * Mathf.Sin(Time.time * this.translationSpeed.y), this.translationDistance.z * Mathf.Sin(Time.time * this.translationSpeed.z));
				vector2 = base.transform.TransformDirection(vector2);
				if (this.isRb)
				{
					this.rb.AddForce(vector2, ForceMode.Force);
					return;
				}
				base.transform.localPosition = vector2;
				return;
			}
		}

		// Token: 0x04001C38 RID: 7224
		public WaveMakerGOMover.MovementType movementType;

		// Token: 0x04001C39 RID: 7225
		[Header("Come and Go Translation movement")]
		public Vector3 translationSpeed = Vector3.zero;

		// Token: 0x04001C3A RID: 7226
		public Vector3 translationDistance = Vector3.zero;

		// Token: 0x04001C3B RID: 7227
		[Header("Rotation movement")]
		public Vector3 rotationSpeed = Vector3.zero;

		// Token: 0x04001C3C RID: 7228
		private bool isRb;

		// Token: 0x04001C3D RID: 7229
		private Rigidbody rb;

		// Token: 0x02000444 RID: 1092
		public enum MovementType
		{
			// Token: 0x040029C6 RID: 10694
			ComeAndGoTranslation,
			// Token: 0x040029C7 RID: 10695
			Rotation
		}
	}
}
