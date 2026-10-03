using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000C7 RID: 199
public class RepairArea : MonoBehaviour
{
	// Token: 0x0600049C RID: 1180 RVA: 0x0002FEF7 File Offset: 0x0002E0F7
	private void Awake()
	{
		base.InvokeRepeating("Repair", 0f, 0.1f);
	}

	// Token: 0x0600049D RID: 1181 RVA: 0x0002FF0E File Offset: 0x0002E10E
	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject == this.PlayerCallHull.gameObject)
		{
			this.repairTime = Time.time;
		}
	}

	// Token: 0x0600049E RID: 1182 RVA: 0x0002FF34 File Offset: 0x0002E134
	private void Repair()
	{
		bool flag = Time.time - this.repairTime <= 0.11f;
		this.Text.gameObject.SetActive(flag);
		if (flag)
		{
			this.PlayerCallHull.Repair(0.1f, null, null);
		}
	}

	// Token: 0x0400095D RID: 2397
	public ImpactDeformable PlayerCallHull;

	// Token: 0x0400095E RID: 2398
	public Text Text;

	// Token: 0x0400095F RID: 2399
	private float repairTime;
}
