using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Utility
{
	// Token: 0x02000262 RID: 610
	public class HysteresisSmoothedValue
	{
		// Token: 0x06001000 RID: 4096 RVA: 0x000BA06D File Offset: 0x000B826D
		public HysteresisSmoothedValue(float initial, float riseTime, float fallTime)
		{
			this.Value = initial;
			this._riseTime = riseTime;
			this._fallTime = fallTime;
		}

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x06001001 RID: 4097 RVA: 0x000BA0A0 File Offset: 0x000B82A0
		// (set) Token: 0x06001002 RID: 4098 RVA: 0x000BA0A8 File Offset: 0x000B82A8
		public float Value { get; private set; }

		// Token: 0x06001003 RID: 4099 RVA: 0x000BA0B4 File Offset: 0x000B82B4
		public void Tick(float target)
		{
			float smoothTime = (target < this.Value) ? this._fallTime : this._riseTime;
			this.Value = Mathf.SmoothDamp(this.Value, target, ref this._velocity, smoothTime);
		}

		// Token: 0x04002099 RID: 8345
		private readonly float _fallTime = 1f;

		// Token: 0x0400209A RID: 8346
		private readonly float _riseTime = 1f;

		// Token: 0x0400209B RID: 8347
		private float _velocity;
	}
}
