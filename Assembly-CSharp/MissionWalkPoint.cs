using System;
using UnityEngine;

// Token: 0x020000F8 RID: 248
public class MissionWalkPoint : MonoBehaviour
{
	// Token: 0x06000662 RID: 1634 RVA: 0x0004C5C0 File Offset: 0x0004A7C0
	private void OnTriggerEnter(Collider other)
	{
		if (other.name == "FPSController" && this.mc.activeMissions.Contains(this.trackerNum))
		{
			this.pointer.ActivateNextWaypoint();
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x04000D54 RID: 3412
	public MissionController mc;

	// Token: 0x04000D55 RID: 3413
	public showmission pointer;

	// Token: 0x04000D56 RID: 3414
	public int trackerNum;
}
