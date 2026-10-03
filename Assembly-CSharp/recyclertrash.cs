using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Steamworks.Data;
using UnityEngine;

// Token: 0x02000197 RID: 407
public class recyclertrash : MonoBehaviour
{
	// Token: 0x060009F8 RID: 2552 RVA: 0x00089657 File Offset: 0x00087857
	private void Start()
	{
		this.aSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x00089668 File Offset: 0x00087868
	public void CheckRecycleTotal()
	{
		if (this.missionNum == 3)
		{
			this.johnnybusy = true;
			int num = 0;
			GameObject[] array = this.missionObject;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == null)
				{
					num++;
				}
			}
			if (num > 5)
			{
				this.missionNum = 4;
				this.mc.CompleteMission(68);
				this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
				this.newRoll.GetComponent<PickUp>().thisDurability = 300f;
			}
			base.StartCoroutine(this.<CheckRecycleTotal>g__JohnnyWait|30_0());
		}
		if (this.missionNum == 2)
		{
			this.missionNum = 3;
			this.johnnybusy = true;
			this.dialogue = "The city got the nerve to tell me there is a mountain of garbage on main street. And they want me to clean it up, like I'm some kinda sanitation fairy. That pile of garbage is the size of a Buick. I took one look at it and said Hell No. You gotta help me out here.";
			this.aSource.clip = this.clip[8];
			this.interactor.Subtitle(this.dialogue, this.aSource.clip);
			this.aSource.Play();
			this.johnnybusy = true;
			this.mc.ActivateMission(68);
			base.StartCoroutine(this.<CheckRecycleTotal>g__SpawnMultiple|30_1());
			base.StartCoroutine(this.<CheckRecycleTotal>g__JohnnyWait|30_0());
			return;
		}
		if (!this.startedMission && this.jiggsScript.missionNum > 1 && this.jakeScript.missionNum > 9)
		{
			this.missionNum = 1;
			this.startedMission = true;
			this.dialogue = "We have a major problem...Someone dropped off what looks like a fucking atom bomb. I need you to take this far, far away from here.";
			this.aSource.clip = this.clip[7];
			this.interactor.Subtitle(this.dialogue, this.aSource.clip);
			this.aSource.Play();
			this.johnnybusy = true;
			base.StartCoroutine(this.<CheckRecycleTotal>g__JohnnyWait|30_0());
			this.mc.ActivateMission(11);
			this.abomb.SetActive(false);
			this.abomb2.SetActive(true);
			this.deliveryZone1.GetComponent<DeliveryZone>().amountDelivered = 0;
			this.deliveryZone1.GetComponent<DeliveryZone>().delivered = false;
			return;
		}
		if (this.deliveryZone1.GetComponent<DeliveryZone>().CheckDelivery() && this.missionNum == 1)
		{
			this.missionNum = 2;
			this.mc.CompleteMission(11);
			this.dialogue = "Let's pretend this never happened.";
			this.aSource.clip = this.clip[1];
			this.interactor.Subtitle(this.dialogue, this.aSource.clip);
			this.aSource.Play();
			this.johnnybusy = true;
			this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
			this.newRoll.GetComponent<PickUp>().thisDurability = 300f;
			base.StartCoroutine(this.<CheckRecycleTotal>g__JohnnyWait|30_0());
			this.abomb2.SetActive(false);
			Achievement achievement = new Achievement("ACH_NUCLEAR");
			achievement.Trigger(true);
			return;
		}
		this.newcashj = 0f;
		this.itemsInsideZone = Physics.OverlapSphere(this.trashZone.transform.position, 2f);
		foreach (Collider collider in this.itemsInsideZone)
		{
			if (collider.GetComponent<PickUp>() && collider.GetComponent<PickUp>().tradein > 0f)
			{
				if (collider.gameObject.name.Contains("missionGarbage"))
				{
					this.missionItemsProcessed++;
				}
				this.newcashj += collider.GetComponent<PickUp>().tradein;
				Object.Destroy(collider.gameObject);
				this.totalBags++;
			}
		}
		if (this.newcashj > 0f)
		{
			this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
			this.newRoll.GetComponent<PickUp>().thisDurability = this.newcashj;
		}
		if (this.totalBags > 9)
		{
			this.eventSystem.GetComponent<MissionController>().CompleteMission(5);
		}
		if (this.missionItemsProcessed > 5)
		{
			if (this.mg.missionType == 2)
			{
				this.eventSystem.GetComponent<MissionController>().CompleteMission(49);
				this.missionItemsProcessed = 0;
			}
			else
			{
				this.missionItemsProcessed = 0;
			}
		}
		if (!this.johnnybusy)
		{
			if (this.newcashj > 0f)
			{
				this.ranNum = Random.Range(0, 7);
				this.johnnybusy = true;
				this.aSource.clip = this.clip[this.ranNum];
				if (this.ranNum == 0)
				{
					this.dialogue = "Okay. Thes bags... Thank You... but nobody can know about this.";
				}
				if (this.ranNum == 1)
				{
					this.dialogue = "Let's pretend this never happened.";
				}
				if (this.ranNum == 2)
				{
					this.dialogue = "Keep bringing these bags, okay?";
				}
				if (this.ranNum == 3)
				{
					this.dialogue = "Listen. Don't tell anyone about this.";
				}
				if (this.ranNum > 3)
				{
					this.dialogue = "Okay.";
					this.aSource.clip = this.clip[6];
				}
				this.interactor.Subtitle(this.dialogue, this.aSource.clip);
				this.aSource.Play();
				base.StartCoroutine(this.<CheckRecycleTotal>g__JohnnyWait|30_0());
				return;
			}
			this.ranNum = Random.Range(4, 7);
			this.johnnybusy = true;
			this.aSource.clip = this.clip[this.ranNum];
			if (this.ranNum == 4)
			{
				this.dialogue = "Okay. Just put the fuckin' bags in the recepticle.";
			}
			if (this.ranNum == 5)
			{
				this.dialogue = "The city is gonna be all over me with this.";
			}
			if (this.ranNum == 6)
			{
				this.dialogue = "Okay.";
			}
			this.interactor.Subtitle(this.dialogue, this.aSource.clip);
			this.aSource.Play();
			base.StartCoroutine(this.<CheckRecycleTotal>g__JohnnyWait|30_0());
		}
	}

	// Token: 0x060009FB RID: 2555 RVA: 0x00089C78 File Offset: 0x00087E78
	[CompilerGenerated]
	private IEnumerator <CheckRecycleTotal>g__JohnnyWait|30_0()
	{
		yield return new WaitForSeconds(5f);
		this.johnnybusy = false;
		yield break;
	}

	// Token: 0x060009FC RID: 2556 RVA: 0x00089C87 File Offset: 0x00087E87
	[CompilerGenerated]
	private IEnumerator <CheckRecycleTotal>g__SpawnMultiple|30_1()
	{
		int num;
		for (int i = 0; i < 8; i = num + 1)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.tire, this.tireLoc.position, Random.rotation);
			this.missionObject[i] = gameObject;
			yield return new WaitForSeconds(1f);
			num = i;
		}
		yield break;
	}

	// Token: 0x04001BCA RID: 7114
	private float newcashj;

	// Token: 0x04001BCB RID: 7115
	private float newmoney;

	// Token: 0x04001BCC RID: 7116
	private float money;

	// Token: 0x04001BCD RID: 7117
	private int totalBags;

	// Token: 0x04001BCE RID: 7118
	private Collider[] itemsInsideZone;

	// Token: 0x04001BCF RID: 7119
	public GameObject eventSystem;

	// Token: 0x04001BD0 RID: 7120
	public GameObject trashZone;

	// Token: 0x04001BD1 RID: 7121
	public AudioSource aSource;

	// Token: 0x04001BD2 RID: 7122
	public AudioClip[] clip;

	// Token: 0x04001BD3 RID: 7123
	public bool johnnybusy;

	// Token: 0x04001BD4 RID: 7124
	private int ranNum;

	// Token: 0x04001BD5 RID: 7125
	public Interactor interactor;

	// Token: 0x04001BD6 RID: 7126
	private string dialogue;

	// Token: 0x04001BD7 RID: 7127
	public Transform moneyLoc;

	// Token: 0x04001BD8 RID: 7128
	public GameObject moneyRoll;

	// Token: 0x04001BD9 RID: 7129
	private GameObject newRoll;

	// Token: 0x04001BDA RID: 7130
	public bool startedMission;

	// Token: 0x04001BDB RID: 7131
	public jiggs jiggsScript;

	// Token: 0x04001BDC RID: 7132
	public Jake jakeScript;

	// Token: 0x04001BDD RID: 7133
	public MissionController mc;

	// Token: 0x04001BDE RID: 7134
	public GameObject abomb;

	// Token: 0x04001BDF RID: 7135
	public GameObject abomb2;

	// Token: 0x04001BE0 RID: 7136
	public GameObject deliveryZone1;

	// Token: 0x04001BE1 RID: 7137
	public int missionNum;

	// Token: 0x04001BE2 RID: 7138
	private int missionItemsProcessed;

	// Token: 0x04001BE3 RID: 7139
	public MissionGen mg;

	// Token: 0x04001BE4 RID: 7140
	public Transform tireLoc;

	// Token: 0x04001BE5 RID: 7141
	public GameObject tire;

	// Token: 0x04001BE6 RID: 7142
	public GameObject[] missionObject;
}
