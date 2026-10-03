using System;
using UnityEngine;

// Token: 0x02000041 RID: 65
public class Contractor : MonoBehaviour
{
	// Token: 0x0600013B RID: 315 RVA: 0x0000EA72 File Offset: 0x0000CC72
	private void Start()
	{
		int num = this.dialogueNum;
		if (this.dialogueNum == 5)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600013C RID: 316 RVA: 0x0000EA94 File Offset: 0x0000CC94
	public void Dialogue()
	{
		if (this.dialogueNum == 4)
		{
			bool flag = false;
			this.itemsInsideZone = Physics.OverlapSphere(base.transform.position, 10f);
			foreach (Collider collider in this.itemsInsideZone)
			{
				if (collider.name.Contains("log") || collider.name.Contains("shaft"))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				this.aSource.clip = this.clip[2];
				this.aSource.Play();
				this.dialogue = "Thanks a lot! You really saved my ass!";
				this.interactor.Subtitle(this.dialogue, this.clip[2]);
				this.dialogueNum = 5;
				this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
				this.newRoll.GetComponent<PickUp>().thisDurability = 200f;
				this.mc.CompleteMission(64);
			}
		}
		if (this.dialogueNum == 3)
		{
			this.aSource.clip = this.clip[3];
			this.aSource.Play();
			this.dialogue = "Some pencil pusher from the city told me I have to clear out all these dead trees. Haul them off too. Like I'm some kinda lumberjack now. Can you help me out here?";
			this.interactor.Subtitle(this.dialogue, this.clip[3]);
			this.anim.Play("contractorline3");
			this.dialogueNum = 4;
			this.mc.ActivateMission(64);
		}
		if (this.dialogueNum == 2)
		{
			this.aSource.clip = this.clip[2];
			this.aSource.Play();
			this.dialogue = "Thanks a lot! You really saved my ass!";
			this.interactor.Subtitle(this.dialogue, this.clip[2]);
			this.dialogueNum = 3;
			this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
			this.newRoll.GetComponent<PickUp>().thisDurability = 300f;
			this.mc.CompleteMission(63);
		}
		if (this.dialogueNum == 1 && !this.aSource.isPlaying)
		{
			this.aSource.clip = this.clip[1];
			this.aSource.Play();
			this.anim.Play("contractorline2");
			this.dialogue = "So I'm standing there asking them what I'm supposed to do. And the guy looks me dead in the eye and goes 'You want your beams? Go dig up the iron yourself.' I'm too old for this shit! You gotta help me out here, come on!";
			this.interactor.Subtitle(this.dialogue, this.clip[1]);
			this.dialogueNum = 2;
			this.mc.ActivateMission(63);
			this.oreScript.beamsMissionStarted = true;
		}
		if (this.dialogueNum == 0)
		{
			this.aSource.clip = this.clip[0];
			this.aSource.Play();
			this.anim.Play("contractorline1");
			this.dialogue = "Alright so I'm trying to build this warehouse, right? Got the blueprints. Got the foundation. Everything's ready to go. But the goddamn foundry says they got no iron!";
			this.interactor.Subtitle(this.dialogue, this.clip[0]);
			this.dialogueNum = 1;
		}
	}

	// Token: 0x04000376 RID: 886
	public GameObject eventSystem;

	// Token: 0x04000377 RID: 887
	public Interactor interactor;

	// Token: 0x04000378 RID: 888
	public MissionController mc;

	// Token: 0x04000379 RID: 889
	private string dialogue;

	// Token: 0x0400037A RID: 890
	public Animation anim;

	// Token: 0x0400037B RID: 891
	public AudioClip[] clip;

	// Token: 0x0400037C RID: 892
	public AudioSource aSource;

	// Token: 0x0400037D RID: 893
	public int dialogueNum;

	// Token: 0x0400037E RID: 894
	public Recyclero oreScript;

	// Token: 0x0400037F RID: 895
	public Transform moneyLoc;

	// Token: 0x04000380 RID: 896
	public GameObject moneyRoll;

	// Token: 0x04000381 RID: 897
	private GameObject newRoll;

	// Token: 0x04000382 RID: 898
	private Collider[] itemsInsideZone;

	// Token: 0x04000383 RID: 899
	public GameObject[] shaft;
}
