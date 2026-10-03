using System;
using Steamworks.Data;
using UnityEngine;

// Token: 0x0200005B RID: 91
public class Endmission : MonoBehaviour
{
	// Token: 0x060001A8 RID: 424 RVA: 0x000118E8 File Offset: 0x0000FAE8
	private void OnTriggerEnter(Collider other)
	{
		if ((other.gameObject.name == "seat_left" || other.gameObject.name == "seat") && this.initiated)
		{
			if (!this.completed && !this.failed)
			{
				this.completed = true;
				base.GetComponent<AudioSource>().Play();
				this.inter.ShowToolTips(2);
				int missionNum = base.transform.parent.GetComponent<showmission>().missionNum;
				this.mc.CompleteMission(missionNum);
			}
			if (!this.failed && this.inter.watching)
			{
				int missionNum2 = base.transform.parent.GetComponent<showmission>().missionNum;
				if (missionNum2 > 17 && missionNum2 < 27)
				{
					Achievement achievement = new Achievement("ACH_GHOST");
					achievement.Trigger(true);
					if (this.inter.fpsHand2.active)
					{
						Achievement achievement2 = new Achievement("ACH_GHOST2");
						achievement2.Trigger(true);
					}
				}
			}
		}
	}

	// Token: 0x04000492 RID: 1170
	public bool completed;

	// Token: 0x04000493 RID: 1171
	public Currency playerCurrency;

	// Token: 0x04000494 RID: 1172
	public int keynum;

	// Token: 0x04000495 RID: 1173
	public bool failed = true;

	// Token: 0x04000496 RID: 1174
	public GameObject key;

	// Token: 0x04000497 RID: 1175
	public Interactor inter;

	// Token: 0x04000498 RID: 1176
	public MissionController mc;

	// Token: 0x04000499 RID: 1177
	public bool initiated;
}
