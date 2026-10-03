using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace NWH.VehiclePhysics2.Utility
{
	// Token: 0x02000265 RID: 613
	[RequireComponent(typeof(ReflectionProbe))]
	public class VehicleReflectionProbe : MonoBehaviour
	{
		// Token: 0x06001025 RID: 4133 RVA: 0x000BA2E4 File Offset: 0x000B84E4
		private void Start()
		{
			this._vc = base.GetComponentInParent<VehicleController>();
			if (this._vc == null)
			{
				Debug.LogError("VehicleController not found.");
			}
			this._reflectionProbe = base.GetComponent<ReflectionProbe>();
			this._vc.onWake.AddListener(new UnityAction(this.OnVehicleWake));
			this._vc.onSleep.AddListener(new UnityAction(this.OnVehicleSleep));
			if (this.bakeOnStart)
			{
				this._reflectionProbe.RenderProbe();
			}
		}

		// Token: 0x06001026 RID: 4134 RVA: 0x000BA370 File Offset: 0x000B8570
		private void OnVehicleWake()
		{
			this._reflectionProbe.mode = ((this.awakeProbeType == VehicleReflectionProbe.ProbeType.Baked) ? (this._reflectionProbe.mode = ReflectionProbeMode.Baked) : ReflectionProbeMode.Realtime);
		}

		// Token: 0x06001027 RID: 4135 RVA: 0x000BA3A4 File Offset: 0x000B85A4
		private void OnVehicleSleep()
		{
			this._reflectionProbe.mode = ((this.asleepProbeType == VehicleReflectionProbe.ProbeType.Baked) ? (this._reflectionProbe.mode = ReflectionProbeMode.Baked) : ReflectionProbeMode.Realtime);
			if (this.bakeOnSleep)
			{
				this._reflectionProbe.RenderProbe();
			}
		}

		// Token: 0x040020A6 RID: 8358
		public VehicleReflectionProbe.ProbeType awakeProbeType = VehicleReflectionProbe.ProbeType.Realtime;

		// Token: 0x040020A7 RID: 8359
		public VehicleReflectionProbe.ProbeType asleepProbeType;

		// Token: 0x040020A8 RID: 8360
		public bool bakeOnStart = true;

		// Token: 0x040020A9 RID: 8361
		public bool bakeOnSleep = true;

		// Token: 0x040020AA RID: 8362
		private ReflectionProbe _reflectionProbe;

		// Token: 0x040020AB RID: 8363
		private VehicleController _vc;

		// Token: 0x020004AD RID: 1197
		public enum ProbeType
		{
			// Token: 0x04002BD6 RID: 11222
			Baked,
			// Token: 0x04002BD7 RID: 11223
			Realtime
		}
	}
}
