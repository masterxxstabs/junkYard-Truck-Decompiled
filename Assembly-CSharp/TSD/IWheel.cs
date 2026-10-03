using System;
using UnityEngine;

namespace TSD
{
	// Token: 0x0200033E RID: 830
	public interface IWheel
	{
		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x06001543 RID: 5443
		// (set) Token: 0x06001544 RID: 5444
		[SerializeField]
		Object wheelObject { get; set; }

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x06001545 RID: 5445
		Transform wheelTransform { get; }

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x06001546 RID: 5446
		bool isGrounded { get; }

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x06001547 RID: 5447
		// (set) Token: 0x06001548 RID: 5448
		float radius { get; set; }

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x06001549 RID: 5449
		float suspensionDistance { get; }

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x0600154A RID: 5450
		// (set) Token: 0x0600154B RID: 5451
		float steerAngle { get; set; }

		// Token: 0x0600154C RID: 5452
		void GetWorldPose(out Vector3 pos, out Quaternion quat);

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x0600154D RID: 5453
		float targetDistance { get; }

		// Token: 0x0600154E RID: 5454
		Vector3 GetGroundHitPoint();

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x0600154F RID: 5455
		float springCompression { get; }

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x06001550 RID: 5456
		Vector3 center { get; }
	}
}
