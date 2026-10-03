using System;
using Steamworks.Data;
using UnityEngine;

// Token: 0x02000031 RID: 49
public class BuyHouse : MonoBehaviour
{
	// Token: 0x060000DC RID: 220 RVA: 0x00002188 File Offset: 0x00000388
	private void Start()
	{
	}

	// Token: 0x060000DD RID: 221 RVA: 0x0000B5C0 File Offset: 0x000097C0
	public void BuyH()
	{
		if (!this.atm.ownsHouse && this.atm.balance >= 20000f)
		{
			this.atm.ownsHouse = true;
			this.atm.balance -= 20000f;
			this.frontDoor.GetComponent<InteractiveObject>().enabled = true;
			this.newOpener = Object.Instantiate<GameObject>(this.opener, this.openerLoc.transform.position, this.openerLoc.transform.rotation);
			this.newOpener = Object.Instantiate<GameObject>(this.opener, this.openerLoc.transform.position, this.openerLoc.transform.rotation);
			this.newOpener = Object.Instantiate<GameObject>(this.opener, this.openerLoc.transform.position, this.openerLoc.transform.rotation);
			this.bed.SetActive(true);
			this.computer.SetActive(true);
			this.asource.Play();
			for (int i = 0; i < 26; i++)
			{
				this.spawnPosition = new Vector3(this.trashLoc.position.x + Random.Range(-10f, 10f), this.trashLoc.position.y + Random.Range(-1f, 1f), this.trashLoc.position.z + Random.Range(-10f, 10f));
				this.newTrash = Object.Instantiate<GameObject>(this.garbageItem[Random.Range(0, 20)], this.spawnPosition, Quaternion.Euler(new Vector3(90f, (float)Random.Range(0, 360), 0f)));
			}
			this.ps.Start();
			base.gameObject.active = false;
			Achievement achievement = new Achievement("ACH_HOME");
			achievement.Trigger(true);
		}
	}

	// Token: 0x0400023A RID: 570
	public Atm atm;

	// Token: 0x0400023B RID: 571
	public Transform trashLoc;

	// Token: 0x0400023C RID: 572
	public GameObject[] garbageItem;

	// Token: 0x0400023D RID: 573
	public GameObject bed;

	// Token: 0x0400023E RID: 574
	public GameObject computer;

	// Token: 0x0400023F RID: 575
	public GameObject frontDoor;

	// Token: 0x04000240 RID: 576
	public Transform openerLoc;

	// Token: 0x04000241 RID: 577
	public GameObject opener;

	// Token: 0x04000242 RID: 578
	public AudioSource asource;

	// Token: 0x04000243 RID: 579
	private GameObject newOpener;

	// Token: 0x04000244 RID: 580
	private GameObject newTrash;

	// Token: 0x04000245 RID: 581
	private Vector3 spawnPosition;

	// Token: 0x04000246 RID: 582
	public PhoneScript ps;
}
