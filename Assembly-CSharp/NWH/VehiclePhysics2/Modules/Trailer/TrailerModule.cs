using System;
using UnityEngine;
using UnityEngine.Events;

namespace NWH.VehiclePhysics2.Modules.Trailer
{
	// Token: 0x0200028B RID: 651
	[Serializable]
	public class TrailerModule : VehicleModule
	{
		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x0600119E RID: 4510 RVA: 0x000C28F4 File Offset: 0x000C0AF4
		// (set) Token: 0x0600119F RID: 4511 RVA: 0x000C28FC File Offset: 0x000C0AFC
		public TrailerHitchModule TrailerHitch
		{
			get
			{
				return this._trailerHitch;
			}
			set
			{
				this._trailerHitch = value;
			}
		}

		// Token: 0x060011A0 RID: 4512 RVA: 0x000C2905 File Offset: 0x000C0B05
		public override void Initialize()
		{
			this.initialized = true;
			this.vc.input.autoSettable = false;
		}

		// Token: 0x060011A1 RID: 4513 RVA: 0x000C2920 File Offset: 0x000C0B20
		public override void FixedUpdate()
		{
			if (base.Active && this.attached && this._trailerHitch != null)
			{
				this.vc.powertrain.transmission.Gear = this.vc.powertrain.transmission.Gear;
			}
		}

		// Token: 0x060011A2 RID: 4514 RVA: 0x00002188 File Offset: 0x00000388
		public override void Update()
		{
		}

		// Token: 0x060011A3 RID: 4515 RVA: 0x000C25F5 File Offset: 0x000C07F5
		public override VehicleModule.ModuleCategory GetModuleCategory()
		{
			return VehicleModule.ModuleCategory.Trailer;
		}

		// Token: 0x060011A4 RID: 4516 RVA: 0x000C2970 File Offset: 0x000C0B70
		public void OnAttach(TrailerHitchModule trailerHitch)
		{
			this._trailerHitch = trailerHitch;
			this.vc.Wake();
			this.vc.input.autoSettable = false;
			if (this.trailerStand != null)
			{
				this.trailerStand.SetActive(false);
			}
			this.attached = true;
			this.onAttach.Invoke();
		}

		// Token: 0x060011A5 RID: 4517 RVA: 0x000C29CC File Offset: 0x000C0BCC
		public void OnDetach()
		{
			if (this.resetInputStatesOnDetach)
			{
				this.vc.input.states.Reset();
			}
			this.vc.input.autoSettable = false;
			if (this.trailerStand != null)
			{
				this.trailerStand.SetActive(true);
			}
			this.vc.effectsManager.lightsManager.Disable();
			this._trailerHitch = null;
			this.vc.Sleep();
			this.attached = false;
			this.onDetach.Invoke();
		}

		// Token: 0x040021E1 RID: 8673
		[Tooltip("True if object is trailer and is attached to a towing vehicle and also true if towing vehicle and has trailer\r\nattached.")]
		public bool attached;

		// Token: 0x040021E2 RID: 8674
		[Tooltip("If the vehicle is a trailer, this is the object placed at the point at which it will connect to the towing vehicle. If the vehicle is towing, this is the object placed at point at which trailer will be coneected.")]
		public Transform attachmentPoint;

		// Token: 0x040021E3 RID: 8675
		public UnityEvent onAttach;

		// Token: 0x040021E4 RID: 8676
		public UnityEvent onDetach;

		// Token: 0x040021E5 RID: 8677
		[Tooltip("    Should the trailer input states be reset when trailer is detached?")]
		public bool resetInputStatesOnDetach = true;

		// Token: 0x040021E6 RID: 8678
		[Tooltip("If enabled the trailer will keep in same gear as the tractor, assuming powertrain on trailer is enabled.")]
		public bool synchronizeGearShifts = true;

		// Token: 0x040021E7 RID: 8679
		[Tooltip("    Object that will be disabled when trailer is attached and disabled when trailer is detached.")]
		public GameObject trailerStand;

		// Token: 0x040021E8 RID: 8680
		[NonSerialized]
		private TrailerHitchModule _trailerHitch;
	}
}
