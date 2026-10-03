using System;
using UnityEngine;
using UnityEngine.Events;

namespace NWH.VehiclePhysics2.Utility
{
	// Token: 0x02000261 RID: 609
	public class FollowVehicleState : MonoBehaviour
	{
		// Token: 0x06000FFC RID: 4092 RVA: 0x000B9FDC File Offset: 0x000B81DC
		private void Awake()
		{
			this._vc = base.GetComponentInParent<VehicleController>();
			if (this._vc == null)
			{
				Debug.LogError("VehicleController not found.");
			}
			this._vc.onWake.AddListener(new UnityAction(this.OnVehicleWake));
			this._vc.onSleep.AddListener(new UnityAction(this.OnVehicleSleep));
			if (this._vc.IsAwake)
			{
				this.OnVehicleWake();
				return;
			}
			this.OnVehicleSleep();
		}

		// Token: 0x06000FFD RID: 4093 RVA: 0x000BA05F File Offset: 0x000B825F
		private void OnVehicleWake()
		{
			base.gameObject.SetActive(true);
		}

		// Token: 0x06000FFE RID: 4094 RVA: 0x000B8AC9 File Offset: 0x000B6CC9
		private void OnVehicleSleep()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x04002098 RID: 8344
		private VehicleController _vc;
	}
}
