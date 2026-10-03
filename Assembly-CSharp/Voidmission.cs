using System;
using UnityEngine;

// Token: 0x0200015D RID: 349
public class Voidmission : MonoBehaviour
{
	// Token: 0x060008A9 RID: 2217 RVA: 0x000708D0 File Offset: 0x0006EAD0
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.name == "seat_left" || other.gameObject.name == "seat")
		{
			this.endMission.failed = true;
			this.endMission.initiated = false;
		}
	}

	// Token: 0x0400140B RID: 5131
	public Endmission endMission;
}
