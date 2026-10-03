using System;
using UnityEngine;

// Token: 0x0200019A RID: 410
public class showmission : MonoBehaviour
{
	// Token: 0x06000A08 RID: 2568 RVA: 0x0008A1B4 File Offset: 0x000883B4
	public void AddWaypoint()
	{
		if (!this.manualNextActivated)
		{
			if (this.missionNum == 49)
			{
				this.waypointScript.useMag = true;
			}
			else
			{
				this.waypointScript.useMag = false;
			}
			this.waypointScript.target = base.transform;
			this.waypointScript.EnablePointer();
			this.isTracking = true;
			return;
		}
		this.manualNextWaypoint.GetComponent<showmission>().AddWaypoint();
	}

	// Token: 0x06000A09 RID: 2569 RVA: 0x0008A221 File Offset: 0x00088421
	public void RemoveThisWaypoint()
	{
		this.waypointScript.HidePointer();
	}

	// Token: 0x06000A0A RID: 2570 RVA: 0x0008A22E File Offset: 0x0008842E
	public void RemoveWaypoint()
	{
		if (this.waypointScript.target == base.transform)
		{
			this.waypointScript.HidePointer();
			if (this.manualNextActivated)
			{
				this.manualNextWaypoint.GetComponent<showmission>().RemoveWaypoint();
			}
		}
	}

	// Token: 0x06000A0B RID: 2571 RVA: 0x0008A26B File Offset: 0x0008846B
	public void ActivateNextWaypoint()
	{
		this.RemoveThisWaypoint();
		this.manualNextActivated = true;
		this.AddWaypoint();
	}

	// Token: 0x06000A0C RID: 2572 RVA: 0x0008A280 File Offset: 0x00088480
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.name == "FPSController" || other.gameObject.name == "seat" || other.gameObject.name == "seat_left")
		{
			if (this.endTrigger.GetComponent<Endmission>().completed)
			{
				this.completed = true;
			}
			else
			{
				this.completed = false;
				this.endTrigger.GetComponent<Endmission>().failed = false;
			}
			if (!this.completed)
			{
				this.mc.ActivateMission_sub(this.missionNum);
				this.inter.discoveredMission = this.missionNum;
			}
			this.endTrigger.GetComponent<Endmission>().initiated = true;
			if (this.isTracking)
			{
				this.RemoveWaypoint();
				if (this.nextWaypoint != null)
				{
					this.nextWaypoint.GetComponent<showmission>().AddWaypoint();
				}
			}
		}
	}

	// Token: 0x04001BF3 RID: 7155
	public int missionNum;

	// Token: 0x04001BF4 RID: 7156
	public bool completed;

	// Token: 0x04001BF5 RID: 7157
	public GameObject endTrigger;

	// Token: 0x04001BF6 RID: 7158
	public MissionWaypoint waypointScript;

	// Token: 0x04001BF7 RID: 7159
	public Interactor inter;

	// Token: 0x04001BF8 RID: 7160
	public MissionController mc;

	// Token: 0x04001BF9 RID: 7161
	public int midpoints;

	// Token: 0x04001BFA RID: 7162
	public GameObject nextWaypoint;

	// Token: 0x04001BFB RID: 7163
	private bool isTracking;

	// Token: 0x04001BFC RID: 7164
	public bool manualNextActivated;

	// Token: 0x04001BFD RID: 7165
	public GameObject manualNextWaypoint;
}
