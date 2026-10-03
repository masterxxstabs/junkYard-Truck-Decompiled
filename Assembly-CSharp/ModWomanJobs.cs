using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000FB RID: 251
public class ModWomanJobs : MonoBehaviour
{
	// Token: 0x06000672 RID: 1650 RVA: 0x0004D024 File Offset: 0x0004B224
	private void Start()
	{
		if (this.jobNum > 9)
		{
			this.additiveShelf.SetActive(true);
		}
		this.SpawnNextItem();
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x0004D044 File Offset: 0x0004B244
	public void Interact()
	{
		if (this.isIdle)
		{
			this.PlayClip(3);
			this.dialogue = "I'll have some more things to test tomorrow.";
			this.interactor.Subtitle(this.dialogue, this.clip[3]);
		}
		else if (this.jobNum == 1)
		{
			this.PlayClip(0);
			this.dialogue = "Hey, you got a minute? I've got a pile of mystery parts that need testing. I need someone brave enough to bolt on this junk and see if it works.";
			this.interactor.Subtitle(this.dialogue, this.clip[0]);
			this.jobNum = 2;
		}
		else if (this.jobNum == 2)
		{
			this.PlayClip(1);
			this.dialogue = "So listen. Somebody brought this tire back, swore up and down that it tried to kill him. Personally I think he doesn't know how to install a tire. So I want you to test it out and report back.";
			this.interactor.Subtitle(this.dialogue, this.clip[2]);
			if (this.spawnedItem == null)
			{
				this.spawnedItem = GameObject.Find("truckwheelx");
			}
			this.spawnedItem.GetComponent<PickUp>().pickable = true;
			this.jobNum = 3;
		}
		else if (this.jobNum == 3)
		{
			this.CheckCompletion(1);
			if (this.job1Condition)
			{
				this.PlayClip(2);
				this.dialogue = "Oh wow, so it exploded? Look on the bright side- you didn't die. Thanks for the field test.";
				this.interactor.Subtitle(this.dialogue, this.clip[2]);
				this.jobNum = 4;
				this.isIdle = true;
			}
		}
		else if (this.jobNum == 5 && !this.isIdle)
		{
			this.PlayClip(4);
			this.dialogue = "I got a return on this radar detector. It's supposed to detect police, but also jam their speed reading equipment. The customer said it didn't work. Will you test it out? Just try not to get arrested.";
			this.interactor.Subtitle(this.dialogue, this.clip[4]);
			if (this.spawnedItem == null)
			{
				this.spawnedItem = GameObject.Find("radar(Clone)");
			}
			this.spawnedItem.GetComponent<PickUp>().pickable = true;
			this.jobNum = 6;
		}
		else if (this.jobNum == 6 && this.job2Condition)
		{
			this.PlayClip(5);
			this.dialogue = "Did you get a speeding ticket? It sounds like the detector works, just not the whole thing about jamming. You can go ahead and keep it.";
			this.interactor.Subtitle(this.dialogue, this.clip[5]);
			this.jobNum = 7;
			this.isIdle = true;
		}
		else if (this.jobNum == 8 && !this.isIdle)
		{
			this.PlayClip(6);
			this.dialogue = "Okay, I got this fuell additive. It's supposed to increase the horsepower. But the instructions are all in Russian. So I guess you pour it into the fuel tank? I'm assuming you're supposed to pour the whole bottle? At least I think. I don't really know.";
			this.interactor.Subtitle(this.dialogue, this.clip[6]);
			if (this.spawnedItem == null)
			{
				this.spawnedItem = GameObject.Find("additive(Clone)");
			}
			this.spawnedItem.GetComponent<PickUp>().pickable = true;
			this.jobNum = 9;
		}
		else if (this.jobNum == 9 && !this.job3Condition)
		{
			this.CheckCompletion(3);
			if (this.job3Condition)
			{
				this.jobNum = 10;
				this.isIdle = true;
				this.PlayClip(7);
				this.dialogue = "It sounds like it worked. Your engine might disagree though. I don't mind selling it, but don't come complaining if you fry your powertrain.";
				this.interactor.Subtitle(this.dialogue, this.clip[7]);
				this.additiveShelf.SetActive(true);
			}
		}
		else if (this.jobNum == 11 && !this.isIdle)
		{
			this.PlayClip(8);
			this.dialogue = "I got something for ya. A supplier sent me this turbocharger. No box, no instructions. And I'm pretty sure the boost gauge came from a boat. Bolt it on and see if it works.";
			this.interactor.Subtitle(this.dialogue, this.clip[8]);
			if (this.spawnedItem == null || this.spawnedItem2 == null)
			{
				this.spawnedItem = GameObject.Find("TurboA");
				this.spawnedItem2 = GameObject.Find("turbogauge(Clone)");
			}
			if (this.spawnedItem != null)
			{
				this.spawnedItem.GetComponent<PickUp>().pickable = true;
			}
			if (this.spawnedItem2 != null)
			{
				this.spawnedItem2.GetComponent<PickUp>().pickable = true;
			}
			this.jobNum = 12;
		}
		else if (this.jobNum == 12 && this.job4Condition)
		{
			this.jobNum = 13;
			this.isIdle = true;
			this.PlayClip(9);
			this.dialogue = "It exploded, didn't it? I heard that from all the way over here. I do appreciate the data. You're basically a scientist now.";
			this.interactor.Subtitle(this.dialogue, this.clip[9]);
		}
		else if (this.jobNum == 14 && !this.isIdle)
		{
			this.PlayClip(10);
			this.dialogue = "Okay, I've got something really weird this time. Some kind of traction assist AI module. Supposedly it monitors terrain and adjusts tire pressure automatically. Sounds cool, right? Except it talks. Like actually talks. I've never seen anything like it, and frankly I'm too scared to test it.";
			this.interactor.Subtitle(this.dialogue, this.clip[10]);
			if (this.spawnedItem == null)
			{
				this.spawnedItem = GameObject.Find("tractionbuddy(Clone)");
			}
			this.spawnedItem.GetComponent<PickUp>().pickable = true;
			this.jobNum = 15;
		}
		if (this.jobNum == 15 && this.job5Condition)
		{
			this.PlayClip(11);
			this.dialogue = "I assume our little friend didn't make it. Maybe the junkyard will give you something for it.";
			this.interactor.Subtitle(this.dialogue, this.clip[11]);
			this.jobNum = 16;
			this.isIdle = true;
			return;
		}
		if (this.jobNum == 17)
		{
			this.PlayClip(12);
			this.dialogue = "I don't have anything else right now. But, I'll give you this. I don't think it works. I guess I don't really know.";
			this.interactor.Subtitle(this.dialogue, this.clip[12]);
			if (this.spawnedItem == null)
			{
				this.spawnedItem = GameObject.Find("cbradio(Clone)");
			}
			this.spawnedItem.GetComponent<PickUp>().pickable = true;
			this.jobNum = 18;
		}
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x0004D58B File Offset: 0x0004B78B
	private void PlayClip(int clipNum)
	{
		this.aSource.clip = this.clip[clipNum];
		this.aSource.Play();
		base.StartCoroutine(this.UnBusy(this.clip[clipNum].length));
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x0004D5C5 File Offset: 0x0004B7C5
	private IEnumerator UnBusy(float length)
	{
		yield return new WaitForSeconds(length);
		this.busy = false;
		yield break;
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x0004D5DC File Offset: 0x0004B7DC
	public void CheckCompletion(int job)
	{
		if (job == 1)
		{
			this.job1Condition = false;
			GameObject gameObject = GameObject.Find("truckwheelx");
			if (gameObject != null)
			{
				if (gameObject.GetComponent<PickUp>().thisDurability < 2f || gameObject.GetComponent<PickUp>().thisDurability > 3f)
				{
					this.job1Condition = true;
					gameObject.GetComponent<PickUp>().tradein = 50f;
					gameObject.name = "truckwheel";
				}
			}
			else if (this.tireSlot != null)
			{
				Transform transform = GameObject.Find(this.tireSlot).transform.Find("BrakeDisk");
				if (((transform != null) ? transform.gameObject : null).GetComponent<durability>().health < 2f)
				{
					this.job1Condition = true;
					this.tireSlot = null;
				}
			}
		}
		if (job == 2 && this.jobNum == 6 && !this.job2Condition)
		{
			float num = Vector3.Distance(this.fps.position, this.radarD.transform.position);
			float num2 = Vector3.Distance(this.fps.position, this.radarF.transform.position);
			float num3 = Vector3.Distance(this.fps.position, this.radarE.transform.position);
			if (num < 4f)
			{
				if (this.radarD.gameObject.GetComponent<Renderer>().enabled)
				{
					this.job2Condition = true;
				}
			}
			else if (num2 < 4f)
			{
				if (this.radarF.gameObject.GetComponent<Renderer>().enabled)
				{
					this.job2Condition = true;
				}
			}
			else if (num3 < 4f && this.radarE.gameObject.GetComponent<Renderer>().enabled)
			{
				this.job2Condition = true;
			}
		}
		if (job == 3)
		{
			if (this.truck.additive > 0f && this.truck.additive < 10f)
			{
				this.job3Condition = true;
			}
			if (this.amc.additive > 0f && this.amc.additive < 10f)
			{
				this.job3Condition = true;
			}
			if (this.truckF.additive > 0f && this.truckF.additive < 10f)
			{
				this.job3Condition = true;
			}
			if (this.engine250.additive > 0f && this.engine250.additive < 10f)
			{
				this.job3Condition = true;
			}
		}
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x0004D848 File Offset: 0x0004BA48
	public void SpawnNextItem()
	{
		this.isIdle = false;
		if (this.jobNum == 0)
		{
			this.jobNum = 1;
			this.spawnedItem = Object.Instantiate<GameObject>(this.testTire, this.spawnLoc.position, this.spawnLoc.rotation);
			this.spawnedItem.GetComponent<PickUp>().pickable = false;
			this.spawnedItem.GetComponent<PickUp>().attachTo2 = null;
			this.spawnedItem.GetComponent<PickUp>().attachTo3 = null;
			this.spawnedItem.GetComponent<PickUp>().tradein = 0f;
			this.spawnedItem.GetComponent<PickUp>().description = "Test Wheel";
			this.spawnedItem.name = "truckwheelx";
			this.spawnedItem.GetComponent<PickUp>().thisDurability = 3f;
			this.spawnedItem.GetComponent<TireAssign>().tireNumX = 14;
			this.spawnedItem.GetComponent<TireAssign>().rimNumX = 4;
			this.spawnedItem.GetComponent<TireAssign>().Start();
			this.spawnedTire = this.spawnedItem;
			this.resetJobs = true;
		}
		else if (this.jobNum == 4)
		{
			this.jobNum = 5;
			this.spawnedItem = Object.Instantiate<GameObject>(this.radarDetector, this.spawnLoc.position, this.spawnLoc.rotation);
			this.spawnedItem.GetComponent<PickUp>().pickable = false;
			this.spawnedRadar = true;
		}
		else if (this.jobNum == 7)
		{
			this.jobNum = 8;
			this.spawnedItem = Object.Instantiate<GameObject>(this.additive, this.spawnLoc.position, this.spawnLoc.rotation);
			this.spawnedItem.GetComponent<PickUp>().pickable = false;
			this.spawnedAdditive = true;
		}
		else if (this.jobNum == 10)
		{
			this.jobNum = 11;
			this.spawnedItem = Object.Instantiate<GameObject>(this.turboGauge, this.spawnLoc.position, this.spawnLoc.rotation);
			this.spawnedItem.GetComponent<PickUp>().pickable = false;
			this.spawnedItem2 = Object.Instantiate<GameObject>(this.turbo, this.spawnLoc2.position, this.spawnLoc2.rotation);
			this.spawnedItem2.GetComponent<PickUp>().pickable = false;
			this.spawnedItem2.GetComponent<PickUp>().price = 0f;
			this.spawnedItem2.GetComponent<PickUp>().tradein = 0f;
			this.spawnedItem2.GetComponent<PickUp>().description = "Unmarked Turbo";
			this.spawnedItem2.name = "TurboA";
			this.spawnedTurbo = this.spawnedItem2;
		}
		else if (this.jobNum == 13)
		{
			this.jobNum = 14;
			this.spawnedItem = Object.Instantiate<GameObject>(this.buddy, this.spawnLoc.position, this.spawnLoc.rotation);
			this.spawnedItem.GetComponent<PickUp>().pickable = false;
			this.spawnedBuddy = true;
		}
		else if (this.jobNum == 16)
		{
			this.jobNum = 17;
			this.spawnedItem = Object.Instantiate<GameObject>(this.radio, this.spawnLoc.position, this.spawnLoc.rotation);
			this.spawnedItem.GetComponent<PickUp>().pickable = false;
		}
		if (this.jobNum < 4 && !this.resetJobs)
		{
			this.resetJobs = true;
			this.spawnedItem = Object.Instantiate<GameObject>(this.testTire, this.spawnLoc.position, this.spawnLoc.rotation);
			this.spawnedItem.GetComponent<PickUp>().pickable = false;
			this.spawnedItem.GetComponent<PickUp>().attachTo2 = null;
			this.spawnedItem.GetComponent<PickUp>().attachTo3 = null;
			this.spawnedItem.GetComponent<PickUp>().tradein = 0f;
			this.spawnedItem.GetComponent<PickUp>().description = "Test Wheel";
			this.spawnedItem.name = "truckwheelx";
			this.spawnedItem.GetComponent<PickUp>().thisDurability = 3f;
			this.spawnedItem.GetComponent<TireAssign>().tireNumX = 14;
			this.spawnedItem.GetComponent<TireAssign>().rimNumX = 4;
			this.spawnedItem.GetComponent<TireAssign>().Start();
			this.spawnedTire = this.spawnedItem;
		}
	}

	// Token: 0x04000D74 RID: 3444
	public Interactor interactor;

	// Token: 0x04000D75 RID: 3445
	public int jobNum;

	// Token: 0x04000D76 RID: 3446
	public AudioClip[] clip;

	// Token: 0x04000D77 RID: 3447
	public AudioSource aSource;

	// Token: 0x04000D78 RID: 3448
	private bool busy;

	// Token: 0x04000D79 RID: 3449
	public Transform spawnLoc;

	// Token: 0x04000D7A RID: 3450
	public Transform spawnLoc2;

	// Token: 0x04000D7B RID: 3451
	public GameObject testTire;

	// Token: 0x04000D7C RID: 3452
	public GameObject radarDetector;

	// Token: 0x04000D7D RID: 3453
	public GameObject additive;

	// Token: 0x04000D7E RID: 3454
	public GameObject turbo;

	// Token: 0x04000D7F RID: 3455
	public GameObject turboGauge;

	// Token: 0x04000D80 RID: 3456
	public GameObject buddy;

	// Token: 0x04000D81 RID: 3457
	public GameObject radio;

	// Token: 0x04000D82 RID: 3458
	private GameObject spawnedItem;

	// Token: 0x04000D83 RID: 3459
	private GameObject spawnedItem2;

	// Token: 0x04000D84 RID: 3460
	public bool job1Condition;

	// Token: 0x04000D85 RID: 3461
	public bool job2Condition;

	// Token: 0x04000D86 RID: 3462
	public bool job3Condition;

	// Token: 0x04000D87 RID: 3463
	public bool job4Condition;

	// Token: 0x04000D88 RID: 3464
	public bool job5Condition;

	// Token: 0x04000D89 RID: 3465
	public string tireSlot;

	// Token: 0x04000D8A RID: 3466
	public bool isIdle;

	// Token: 0x04000D8B RID: 3467
	public Transform fps;

	// Token: 0x04000D8C RID: 3468
	public Transform radarD;

	// Token: 0x04000D8D RID: 3469
	public Transform radarF;

	// Token: 0x04000D8E RID: 3470
	public Transform radarE;

	// Token: 0x04000D8F RID: 3471
	public car truck;

	// Token: 0x04000D90 RID: 3472
	public car3 amc;

	// Token: 0x04000D91 RID: 3473
	public car4 truckF;

	// Token: 0x04000D92 RID: 3474
	public Engine250 engine250;

	// Token: 0x04000D93 RID: 3475
	public bool resetJobs;

	// Token: 0x04000D94 RID: 3476
	public GameObject spawnedTire;

	// Token: 0x04000D95 RID: 3477
	public GameObject spawnedTire2;

	// Token: 0x04000D96 RID: 3478
	public GameObject spawnedTurbo;

	// Token: 0x04000D97 RID: 3479
	public bool spawnedRadar;

	// Token: 0x04000D98 RID: 3480
	public bool spawnedAdditive;

	// Token: 0x04000D99 RID: 3481
	public bool spawnedBuddy;

	// Token: 0x04000D9A RID: 3482
	private string dialogue;

	// Token: 0x04000D9B RID: 3483
	public GameObject additiveShelf;
}
