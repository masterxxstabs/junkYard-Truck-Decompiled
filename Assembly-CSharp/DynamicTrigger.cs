using System;
using UnityEngine;

// Token: 0x02000058 RID: 88
public class DynamicTrigger : MonoBehaviour
{
	// Token: 0x0600019B RID: 411 RVA: 0x00011654 File Offset: 0x0000F854
	private void OnTriggerEnter(Collider other)
	{
		if (!this.isStart)
		{
			this.mg.CheckCompletion(other.gameObject);
		}
		if (this.isStart && other.name.Contains("FPSController") && this.proximityBased && !this.waypointed)
		{
			this.waypointed = true;
			this.mg.NextTracker();
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600019C RID: 412 RVA: 0x000116C2 File Offset: 0x0000F8C2
	public void PickUpWaypoint()
	{
		if (this.pickUpBased)
		{
			this.waypointed = true;
			this.mg.NextTracker();
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x04000487 RID: 1159
	public bool isStart;

	// Token: 0x04000488 RID: 1160
	public MissionGen mg;

	// Token: 0x04000489 RID: 1161
	public bool pickUpBased;

	// Token: 0x0400048A RID: 1162
	public bool proximityBased;

	// Token: 0x0400048B RID: 1163
	public bool waypointed;
}
