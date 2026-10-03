using System;
using UnityEngine;
using UnityEngine.Events;

namespace NWH.VehiclePhysics2.Cameras
{
	// Token: 0x020002CB RID: 715
	[RequireComponent(typeof(VehicleCamera))]
	public class CameraShake : MonoBehaviour
	{
		// Token: 0x06001359 RID: 4953 RVA: 0x000CB52C File Offset: 0x000C972C
		private void Start()
		{
			this._vehicleCamera = base.GetComponent<VehicleCamera>();
			if (this._vehicleCamera.target != null)
			{
				this._vehicleCamera.target.damageHandler.OnCollision.AddListener(new UnityAction<Collision>(this.Shake));
			}
		}

		// Token: 0x0600135A RID: 4954 RVA: 0x000CB580 File Offset: 0x000C9780
		private void Update()
		{
			if (this._elapsed > 0f)
			{
				this._elapsed -= Time.deltaTime;
				base.transform.localPosition += Random.insideUnitSphere * (this.shakeAmount * (this._elapsed / this.shakeDuration));
			}
		}

		// Token: 0x0600135B RID: 4955 RVA: 0x000CB5E0 File Offset: 0x000C97E0
		public void Shake(Collision collision)
		{
			this._elapsed = this.shakeDuration;
		}

		// Token: 0x040023DC RID: 9180
		public float shakeAmount = 0.2f;

		// Token: 0x040023DD RID: 9181
		public float shakeDuration = 0.5f;

		// Token: 0x040023DE RID: 9182
		private float _elapsed;

		// Token: 0x040023DF RID: 9183
		private VehicleCamera _vehicleCamera;

		// Token: 0x040023E0 RID: 9184
		private VehicleController _targetVehicle;
	}
}
