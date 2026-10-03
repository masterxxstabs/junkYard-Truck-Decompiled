using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Input
{
	// Token: 0x020002C2 RID: 706
	[Serializable]
	public struct InputStates
	{
		// Token: 0x06001326 RID: 4902 RVA: 0x000CA0D4 File Offset: 0x000C82D4
		public void Reset()
		{
			this.horizontal = 0f;
			this.vertical = 0f;
			this.clutch = 0f;
			this.handbrake = 0f;
			this.shiftUp = false;
			this.shiftDown = false;
			this.shiftInto = -999;
			this.leftBlinker = false;
			this.rightBlinker = false;
			this.lowBeamLights = false;
			this.highBeamLights = false;
			this.hazardLights = false;
			this.extraLights = false;
			this.trailerAttachDetach = false;
			this.horn = false;
			this.engineStartStop = false;
			this.cruiseControl = false;
			this.boost = false;
			this.flipOver = false;
		}

		// Token: 0x04002386 RID: 9094
		[Range(0f, 1f)]
		public float clutch;

		// Token: 0x04002387 RID: 9095
		public bool engineStartStop;

		// Token: 0x04002388 RID: 9096
		public bool extraLights;

		// Token: 0x04002389 RID: 9097
		public bool highBeamLights;

		// Token: 0x0400238A RID: 9098
		[Range(0f, 1f)]
		public float handbrake;

		// Token: 0x0400238B RID: 9099
		public bool hazardLights;

		// Token: 0x0400238C RID: 9100
		[Range(-1f, 1f)]
		public float horizontal;

		// Token: 0x0400238D RID: 9101
		public bool horn;

		// Token: 0x0400238E RID: 9102
		public bool leftBlinker;

		// Token: 0x0400238F RID: 9103
		public bool lowBeamLights;

		// Token: 0x04002390 RID: 9104
		public bool rightBlinker;

		// Token: 0x04002391 RID: 9105
		public bool shiftDown;

		// Token: 0x04002392 RID: 9106
		public int shiftInto;

		// Token: 0x04002393 RID: 9107
		public bool shiftUp;

		// Token: 0x04002394 RID: 9108
		public bool trailerAttachDetach;

		// Token: 0x04002395 RID: 9109
		public bool cruiseControl;

		// Token: 0x04002396 RID: 9110
		public bool boost;

		// Token: 0x04002397 RID: 9111
		public bool flipOver;

		// Token: 0x04002398 RID: 9112
		[Range(-1f, 1f)]
		public float vertical;
	}
}
