using System;
using UnityEngine;

// Token: 0x0200013E RID: 318
public class StarTrack : MonoBehaviour
{
	// Token: 0x06000833 RID: 2099 RVA: 0x0006CD04 File Offset: 0x0006AF04
	public void Star()
	{
		this.mc.TrackMission(this.slotNum);
	}

	// Token: 0x06000834 RID: 2100 RVA: 0x0006CD17 File Offset: 0x0006AF17
	public void UnStar()
	{
		this.mc.unTrackMission(this.slotNum);
	}

	// Token: 0x0400131C RID: 4892
	public int slotNum;

	// Token: 0x0400131D RID: 4893
	public MissionController mc;
}
