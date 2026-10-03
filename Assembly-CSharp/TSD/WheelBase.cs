using System;
using UnityEngine;

namespace TSD
{
	// Token: 0x0200033D RID: 829
	[Serializable]
	public class WheelBase
	{
		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x0600153D RID: 5437 RVA: 0x000DFEF7 File Offset: 0x000DE0F7
		// (set) Token: 0x0600153E RID: 5438 RVA: 0x000DFEFF File Offset: 0x000DE0FF
		public Object m_wheelColliderBase { get; set; }

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x0600153F RID: 5439 RVA: 0x000DFF08 File Offset: 0x000DE108
		// (set) Token: 0x06001540 RID: 5440 RVA: 0x000DFF10 File Offset: 0x000DE110
		public virtual Object wheelObject { get; set; }

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x06001541 RID: 5441 RVA: 0x000A22BF File Offset: 0x000A04BF
		public Transform wheelTransform
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}
