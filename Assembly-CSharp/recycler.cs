using System;
using System.Collections;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.Video;

// Token: 0x02000196 RID: 406
public class recycler : MonoBehaviour
{
	// Token: 0x060009F3 RID: 2547 RVA: 0x00089058 File Offset: 0x00087258
	private void Start()
	{
		if (this.supraEng.price == 1500f)
		{
			this.supraEng.price = 1499f;
			int num = Random.Range(0, 7);
			int num2 = Random.Range(0, 7);
			int num3 = Random.Range(0, 7);
			this.supraParts[num].GetComponent<durability>().health = 0f;
			this.supraParts[num2].GetComponent<durability>().health = 0f;
			this.supraParts[num3].GetComponent<durability>().health = 0f;
			this.supraParts[num].GetComponent<durability>().Start();
			this.supraParts[num2].GetComponent<durability>().Start();
			this.supraParts[num3].GetComponent<durability>().Start();
		}
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x0008911D File Offset: 0x0008731D
	public bool IsBusy()
	{
		return this.aSource.isPlaying;
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x00089130 File Offset: 0x00087330
	public void CheckRecycleTotal()
	{
		Debug.Log("1");
		if (!this.IsBusy())
		{
			Debug.Log("notbusy");
			this.itemsInsideZone = Physics.OverlapSphere(base.transform.position, 3f);
			foreach (Collider collider in this.itemsInsideZone)
			{
				if (collider.GetComponent<PickUp>() && collider.GetComponent<PickUp>().tradein > 0f && collider.name.Substring(0, 4) != "furn")
				{
					this.itemsInArea = true;
				}
			}
			if (!this.safetyvideo && this.itemsInArea && !this.johnnybusy)
			{
				this.johnnybusy = true;
				this.aSource.clip = this.clip[0];
				this.aSource.Play();
				this.dialogue = "You got some things to recycle? ...allright... But before we do any recycling I'm requred by law to show you this instructional film over here- Watch.";
				this.interactor.Subtitle(this.dialogue, this.aSource.clip);
				Debug.Log("2");
				base.StartCoroutine(this.playVideo());
			}
			if (!this.itemsInArea && !this.johnnybusy)
			{
				if (this.dialogueNum != 5)
				{
					this.aSource.clip = this.clip[this.dialogueNum];
					this.aSource.Play();
				}
				if (this.dialogueNum == 3)
				{
					this.dialogue = "Put your recyclables in the recycling area, then come talk to me.";
					this.interactor.Subtitle(this.dialogue, this.aSource.clip);
				}
				if (this.dialogueNum == 4)
				{
					this.dialogue = "How you doin'?";
					this.interactor.Subtitle(this.dialogue, this.aSource.clip);
				}
				if (this.dialogueNum == 5)
				{
					if (!this.jzdialogue && this.safetyvideo && !this.supraEng.pickable)
					{
						this.jzdialogue = true;
						this.aSource.clip = this.clip[7];
						this.aSource.Play();
						this.dialogue = "Now let me tell you something. What's the one thing every schmuck in this town needs? Power. Raw, unfiltered horsepower. I got a six cylinder masterpiece here. Not just any six, mind you. This here is a legend. The kinda engine you don't just find lying around. Nah, this beauty's been places, seen things, if engines could talk, this one would write a fuckin best seller.";
						this.dialogueNum = 7;
						this.interactor.Subtitle(this.dialogue, this.aSource.clip);
					}
					else
					{
						this.dialogue = "Welcome to, uh, Auto Junktion.";
						this.interactor.Subtitle(this.dialogue, this.aSource.clip);
						this.aSource.clip = this.clip[5];
						this.aSource.Play();
					}
				}
				if (this.dialogueNum == 6)
				{
					this.dialogue = "Here to browse the junkyard? We get new stuff every day!";
					this.interactor.Subtitle(this.dialogue, this.aSource.clip);
				}
				this.dialogueNum++;
				if (this.dialogueNum > 6)
				{
					this.dialogueNum = 3;
				}
			}
			if (this.itemsInArea && this.safetyvideo)
			{
				this.eventSystem.GetComponent<MissionController>().CompleteMission(3);
				this.newcashj = 0f;
				foreach (Collider collider2 in this.itemsInsideZone)
				{
					if (collider2.GetComponent<PickUp>() && collider2.GetComponent<PickUp>().tradein > 0f && collider2.name.Substring(0, 4) != "furn")
					{
						if (collider2.name.Contains("missionMassive") && this.mg.missionType == 7)
						{
							this.eventSystem.GetComponent<MissionController>().CompleteMission(49);
						}
						if (collider2.name.Contains("tractionbuddy"))
						{
							this.tractionBuddy = true;
						}
						this.newcashj += collider2.GetComponent<PickUp>().tradein;
						Object.Destroy(collider2.gameObject);
					}
				}
				this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
				this.newRoll.GetComponent<PickUp>().thisDurability = this.newcashj;
				this.itemsInArea = false;
				this.ranNum = Random.Range(1, 3);
				if (this.tractionBuddy)
				{
					this.ranNum = 8;
				}
				this.aSource.clip = this.clip[this.ranNum];
				this.aSource.Play();
				if (this.ranNum == 1)
				{
					this.dialogue = "Okay.";
					this.interactor.Subtitle(this.dialogue, this.aSource.clip);
				}
				if (this.ranNum == 2)
				{
					this.dialogue = "Here ya go.";
					this.interactor.Subtitle(this.dialogue, this.aSource.clip);
				}
				if (this.ranNum == 8)
				{
					this.dialogue = "Is that an actual traction buddy?! Nice!";
					this.interactor.Subtitle(this.dialogue, this.aSource.clip);
				}
				if (this.newcashj > 600f)
				{
					Achievement achievement = new Achievement("ACH_SCRAPPER");
					achievement.Trigger(true);
				}
			}
		}
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x00089639 File Offset: 0x00087839
	private IEnumerator playVideo()
	{
		yield return new WaitForSeconds(12f);
		this.recycleTv.GetComponent<Renderer>().enabled = true;
		this.recycleTv.GetComponent<VideoPlayer>().Play();
		yield return new WaitForSeconds(26f);
		this.recycleTv.GetComponent<Renderer>().enabled = false;
		this.safetyvideo = true;
		this.johnnybusy = false;
		yield break;
	}

	// Token: 0x04001BB1 RID: 7089
	private float newcashj;

	// Token: 0x04001BB2 RID: 7090
	private float newmoney;

	// Token: 0x04001BB3 RID: 7091
	private float money;

	// Token: 0x04001BB4 RID: 7092
	public GameObject recycleTv;

	// Token: 0x04001BB5 RID: 7093
	private Collider[] itemsInsideZone;

	// Token: 0x04001BB6 RID: 7094
	public bool safetyvideo;

	// Token: 0x04001BB7 RID: 7095
	public GameObject johnny;

	// Token: 0x04001BB8 RID: 7096
	public bool itemsInArea;

	// Token: 0x04001BB9 RID: 7097
	public AudioSource aSource;

	// Token: 0x04001BBA RID: 7098
	public AudioClip[] clip;

	// Token: 0x04001BBB RID: 7099
	public bool johnnybusy;

	// Token: 0x04001BBC RID: 7100
	private int ranNum;

	// Token: 0x04001BBD RID: 7101
	private int dialogueNum = 3;

	// Token: 0x04001BBE RID: 7102
	public GameObject eventSystem;

	// Token: 0x04001BBF RID: 7103
	private string dialogue;

	// Token: 0x04001BC0 RID: 7104
	public Interactor interactor;

	// Token: 0x04001BC1 RID: 7105
	public Transform moneyLoc;

	// Token: 0x04001BC2 RID: 7106
	public GameObject moneyRoll;

	// Token: 0x04001BC3 RID: 7107
	private GameObject newRoll;

	// Token: 0x04001BC4 RID: 7108
	public MissionGen mg;

	// Token: 0x04001BC5 RID: 7109
	public bool jzdialogue;

	// Token: 0x04001BC6 RID: 7110
	public GameObject supra;

	// Token: 0x04001BC7 RID: 7111
	public PickUp supraEng;

	// Token: 0x04001BC8 RID: 7112
	public GameObject[] supraParts;

	// Token: 0x04001BC9 RID: 7113
	private bool tractionBuddy;
}
