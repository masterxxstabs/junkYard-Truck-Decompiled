using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x02000177 RID: 375
public class jiggs : MonoBehaviour
{
	// Token: 0x06000938 RID: 2360 RVA: 0x0007D3CC File Offset: 0x0007B5CC
	private void Start()
	{
		this.anim = base.GetComponent<Animation>();
		if (this.missionNum == 3)
		{
			this.stump1.SetActive(true);
			this.stump2.SetActive(true);
			this.stump3.SetActive(true);
		}
		if (this.missionNum > 3)
		{
			this.recipe.SetActive(true);
		}
	}

	// Token: 0x06000939 RID: 2361 RVA: 0x0007D428 File Offset: 0x0007B628
	public void Interact()
	{
		if (this.missionNum == 0 && this.jake.missionNum == 10)
		{
			this.mc.CompleteMission(43);
			this.mc.ActivateMission(10);
			this.mc.TrackMission_N(10);
			this.anim.Play("jiggs3");
			this.bomb.GetComponent<PickUp>().pickable = true;
			this.deliveryZone1.GetComponent<DeliveryZone>().amountDelivered = 0;
			this.deliveryZone1.GetComponent<DeliveryZone>().delivered = false;
			AudioSource.PlayClipAtPoint(this.clip1, this.audioLoc.transform.position, 0.9f);
			this.missionNum = 1;
			this.dialogue = "Moonshine you say? Heh, I don't make that stuff anymore. Sit down, let me tell you a story. When I was a boy I took out a college loan... but I never went to college. Well they started askin' for the money. I told them I used the money to build a nuclear bomb, and that I would appreciate it if they stopped callin'. Well they stopped callin'. So it's been sitting here since then and I should probably get rid of it. Maybe you can haul it to the dump for me. But...ehh... do it at night so nobody sees you. Do that and I'll tell you how to make yer own Moonshine!";
			this.interactor.Subtitle(this.dialogue, this.clip1);
			base.StartCoroutine(this.SecondAnim(42f));
			return;
		}
		if (this.missionNum == 1)
		{
			if (this.deliveryZone1.GetComponent<DeliveryZone>().CheckDelivery())
			{
				this.mc.CompleteMission(10);
				this.missionNum = 2;
				this.eventSystem.GetComponent<MissionController>().CompleteMission(43);
				this.eventSystem.GetComponent<MissionController>().CompleteMission(10);
				AudioSource.PlayClipAtPoint(this.clip4, this.audioLoc.transform.position, 0.9f);
				this.deliveryZone1.GetComponent<DeliveryZone>().delivered = false;
				this.dialogue = "Thanks. I'll mark my old moonshine still on your map! With that still you can make and sell all the moonshine you want. And if you don't know what to do, my old journal should be lyin' around there somewhere, should have all the info you need to get started.";
				this.interactor.Subtitle(this.dialogue, this.clip4);
				base.StartCoroutine(this.SecondAnim2(18f));
				this.isBusy = true;
				base.StartCoroutine(this.UnBusy());
				this.mc.ActivateMission(44);
				return;
			}
		}
		else
		{
			if (!this.isBusy)
			{
				if (this.jake.missionNum < 10)
				{
					AudioSource.PlayClipAtPoint(this.clip7, this.audioLoc.transform.position, 0.9f);
					this.dialogue = "If you ever lose anything important out in the wilderness, chances are I may have seen it.";
					this.interactor.Subtitle(this.dialogue, this.clip7);
				}
				else if (this.missionNum == 2)
				{
					this.missionNum = 3;
					AudioSource.PlayClipAtPoint(this.clip15, this.audioLoc.transform.position, 0.9f);
					this.dialogue = "You think you could use that truck to rip out some of these old stumps?";
					this.interactor.Subtitle(this.dialogue, this.clip15);
					this.stump1.SetActive(true);
					this.stump2.SetActive(true);
					this.stump3.SetActive(true);
					this.mc.ActivateMission(71);
				}
				else if (this.missionNum == 3 && this.pulled1 && this.pulled2 && this.pulled3)
				{
					this.missionNum = 4;
					AudioSource.PlayClipAtPoint(this.clip16, this.audioLoc.transform.position, 0.9f);
					this.dialogue = "I'll give you my top secret moonshine recipe.";
					this.interactor.Subtitle(this.dialogue, this.clip16);
					this.recipe.SetActive(true);
					this.stump1.SetActive(false);
					this.stump2.SetActive(false);
					this.stump3.SetActive(false);
					this.mc.CompleteMission(71);
				}
				else if (this.missionNum == 4)
				{
					this.missionNum = 5;
					AudioSource.PlayClipAtPoint(this.clip17, this.audioLoc.transform.position, 0.9f);
					this.dialogue = "Now if you cut down those dead trees for me...and haul away that big old pile of shit behind my house...I'll give you this. ";
					this.interactor.Subtitle(this.dialogue, this.clip17);
					this.spawnedpaddle = Object.Instantiate<GameObject>(this.paddle, this.waterwheelLoc.position, this.waterwheelLoc.rotation);
					this.spawnedpaddle.GetComponent<PickUp>().pickable = false;
					base.StartCoroutine(this.SpawnTires());
					base.StartCoroutine(this.SpawnBarrels());
					this.mc.ActivateMission(72);
				}
				else if (this.missionNum == 5)
				{
					bool flag = false;
					this.itemsInsideZone = Physics.OverlapSphere(base.transform.position, 10f);
					foreach (Collider collider in this.itemsInsideZone)
					{
						if (collider.name.Contains("radbarrel") || collider.name.Contains("GarbageTire1"))
						{
							flag = true;
						}
					}
					if (!flag)
					{
						AudioSource.PlayClipAtPoint(this.clip18, this.audioLoc.transform.position, 0.9f);
						this.dialogue = "Here ya go. I'm sure you can figure this thing out on your own.";
						this.interactor.Subtitle(this.dialogue, this.clip18);
						if (this.spawnedpaddle != null)
						{
							this.spawnedpaddle.GetComponent<PickUp>().pickable = true;
						}
						else
						{
							this.spawnedpaddle = GameObject.Find("waterwheel");
							if (this.spawnedpaddle == null)
							{
								this.spawnedpaddle = GameObject.Find("waterwheel(Clone)");
							}
							if (this.spawnedpaddle != null)
							{
								this.spawnedpaddle.GetComponent<PickUp>().pickable = true;
							}
						}
						this.missionNum = 6;
						this.mc.CompleteMission(72);
					}
				}
				else
				{
					this.ranDialogue++;
					if (this.ranDialogue > 6)
					{
						this.ranDialogue = 1;
					}
					if (this.ranDialogue == 1)
					{
						AudioSource.PlayClipAtPoint(this.clip9, this.audioLoc.transform.position, 0.9f);
						AudioSource.PlayClipAtPoint(this.clip10, this.audioLoc2.transform.position, 0.9f);
						this.dialogue = "How's that moonshine business coming along? -I mean that soda pop business? ...Dammit Lady!";
						this.interactor.Subtitle(this.dialogue, this.clip10);
					}
					if (this.ranDialogue == 2)
					{
						AudioSource.PlayClipAtPoint(this.clip11, this.audioLoc.transform.position, 0.9f);
						AudioSource.PlayClipAtPoint(this.clip12, this.audioLoc2.transform.position, 0.9f);
						this.dialogue = "Hey, can you do me a favor? Come closer... you think you can bring me some moonshine? .... Aww hell, nevermind!";
						this.interactor.Subtitle(this.dialogue, this.clip11);
					}
					if (this.ranDialogue == 3)
					{
						AudioSource.PlayClipAtPoint(this.clip13, this.audioLoc.transform.position, 0.9f);
						AudioSource.PlayClipAtPoint(this.clip14, this.audioLoc2.transform.position, 0.9f);
						this.dialogue = "Moonshinin' is an art. Too much sugar, and it tastes bad. You think it would be the other way around. The perfect balance is somewhere between- ... nevermind.";
						this.interactor.Subtitle(this.dialogue, this.clip13);
					}
					if (this.ranDialogue > 3)
					{
						AudioSource.PlayClipAtPoint(this.clip7, this.audioLoc.transform.position, 0.9f);
						this.dialogue = "If you ever lose anything important out in the wilderness, chances are I may have seen it.";
						this.interactor.Subtitle(this.dialogue, this.clip7);
					}
				}
			}
			this.isBusy = true;
			this.lostUI.SetActive(true);
			this.fpc.UnlockMouse();
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			this.fpc.enabled = false;
			base.StartCoroutine(this.UnBusy());
		}
	}

	// Token: 0x0600093A RID: 2362 RVA: 0x0007DB78 File Offset: 0x0007BD78
	public void PlayFound()
	{
		if (!this.isBusy)
		{
			AudioSource.PlayClipAtPoint(this.clip8, this.audioLoc.transform.position, 0.9f);
		}
		this.dialogue = "Yeah, I actually found that the other day... Here ya go.";
		this.interactor.Subtitle(this.dialogue, this.clip8);
		this.isBusy = true;
		this.inv.SubtractMoney(30f);
		this.currency.money -= 30f;
		base.StartCoroutine(this.UnBusy());
	}

	// Token: 0x0600093B RID: 2363 RVA: 0x0007DC0A File Offset: 0x0007BE0A
	public void PullStump(int whichStump)
	{
		if (whichStump == 1)
		{
			this.pulled1 = true;
			return;
		}
		if (whichStump == 2)
		{
			this.pulled2 = true;
			return;
		}
		if (whichStump == 3)
		{
			this.pulled3 = true;
		}
	}

	// Token: 0x0600093C RID: 2364 RVA: 0x0007DC2F File Offset: 0x0007BE2F
	private IEnumerator SpawnTires()
	{
		int num;
		for (int i = 0; i < 3; i = num + 1)
		{
			Object.Instantiate<GameObject>(this.trashprefab1, this.trashLoc.position, Random.rotation);
			yield return new WaitForSeconds(1f);
			num = i;
		}
		yield break;
	}

	// Token: 0x0600093D RID: 2365 RVA: 0x0007DC3E File Offset: 0x0007BE3E
	private IEnumerator SpawnBarrels()
	{
		int num;
		for (int i = 0; i < 3; i = num + 1)
		{
			Object.Instantiate<GameObject>(this.trashprefab2, this.trashLoc2.position, Random.rotation);
			yield return new WaitForSeconds(1f);
			num = i;
		}
		yield break;
	}

	// Token: 0x0600093E RID: 2366 RVA: 0x0007DC4D File Offset: 0x0007BE4D
	private IEnumerator UnBusy()
	{
		yield return new WaitForSeconds(6f);
		this.isBusy = false;
		yield break;
	}

	// Token: 0x0600093F RID: 2367 RVA: 0x0007DC5C File Offset: 0x0007BE5C
	private IEnumerator SecondAnim(float customSec)
	{
		yield return new WaitForSeconds(customSec);
		this.anim.Play("jiggs2");
		AudioSource.PlayClipAtPoint(this.clip2, this.audioLoc2.transform.position, 0.9f);
		this.dialogue = "Jiggs Casey! What did I tell you about moonshine?! No more moonshinin' you old fool!";
		this.interactor.Subtitle(this.dialogue, this.clip2);
		yield return new WaitForSeconds(7.3f);
		AudioSource.PlayClipAtPoint(this.clip3, this.audioLoc.transform.position, 0.9f);
		this.dialogue = "That's...that's not even what I'm doin out here, lady!";
		this.interactor.Subtitle(this.dialogue, this.clip3);
		yield return new WaitForSeconds(4.5f);
		AudioSource.PlayClipAtPoint(this.clip3b, this.audioLoc2.transform.position, 0.9f);
		this.dialogue = "And get rid of that god damn nuclear bomb! I don't want that thing sittin' in the yard no more!";
		this.interactor.Subtitle(this.dialogue, this.clip3b);
		yield return new WaitForSeconds(1f);
		this.anim.Play("jiggs4");
		yield return new WaitForSeconds(6f);
		this.anim.Play("jiggs");
		yield break;
	}

	// Token: 0x06000940 RID: 2368 RVA: 0x0007DC72 File Offset: 0x0007BE72
	private IEnumerator SecondAnim2(float customSec)
	{
		yield return new WaitForSeconds(customSec);
		this.anim.Play("jiggs2");
		AudioSource.PlayClipAtPoint(this.clip5, this.audioLoc2.transform.position, 0.9f);
		this.dialogue = "Jiggs! I hear you out there talkin about moonshine. No more moonshinin!";
		this.interactor.Subtitle(this.dialogue, this.clip5);
		yield return new WaitForSeconds(6f);
		AudioSource.PlayClipAtPoint(this.clip6, this.audioLoc.transform.position, 0.9f);
		this.dialogue = "Does it look like I'm moonshinin out here? I'm just... sittin here talkin! Look out the winda' See that?! The nuclear bomb is gone! Thanks partner!";
		this.interactor.Subtitle(this.dialogue, this.clip6);
		yield return new WaitForSeconds(7f);
		this.anim.Play("jiggs");
		yield break;
	}

	// Token: 0x0400189E RID: 6302
	public AudioSource aSource;

	// Token: 0x0400189F RID: 6303
	public AudioClip clip1;

	// Token: 0x040018A0 RID: 6304
	public AudioClip clip2;

	// Token: 0x040018A1 RID: 6305
	public AudioClip clip3;

	// Token: 0x040018A2 RID: 6306
	public AudioClip clip3b;

	// Token: 0x040018A3 RID: 6307
	public AudioClip clip4;

	// Token: 0x040018A4 RID: 6308
	public AudioClip clip5;

	// Token: 0x040018A5 RID: 6309
	public AudioClip clip6;

	// Token: 0x040018A6 RID: 6310
	public AudioClip clip7;

	// Token: 0x040018A7 RID: 6311
	public AudioClip clip8;

	// Token: 0x040018A8 RID: 6312
	public AudioClip clip9;

	// Token: 0x040018A9 RID: 6313
	public AudioClip clip10;

	// Token: 0x040018AA RID: 6314
	public AudioClip clip11;

	// Token: 0x040018AB RID: 6315
	public AudioClip clip12;

	// Token: 0x040018AC RID: 6316
	public AudioClip clip13;

	// Token: 0x040018AD RID: 6317
	public AudioClip clip14;

	// Token: 0x040018AE RID: 6318
	public AudioClip clip15;

	// Token: 0x040018AF RID: 6319
	public AudioClip clip16;

	// Token: 0x040018B0 RID: 6320
	public AudioClip clip17;

	// Token: 0x040018B1 RID: 6321
	public AudioClip clip18;

	// Token: 0x040018B2 RID: 6322
	private Animation anim;

	// Token: 0x040018B3 RID: 6323
	public Interactor interactor;

	// Token: 0x040018B4 RID: 6324
	public GameObject eventSystem;

	// Token: 0x040018B5 RID: 6325
	public Transform audioLoc;

	// Token: 0x040018B6 RID: 6326
	public Transform audioLoc2;

	// Token: 0x040018B7 RID: 6327
	public showmission pointer;

	// Token: 0x040018B8 RID: 6328
	public GameObject deliveryZone1;

	// Token: 0x040018B9 RID: 6329
	public GameObject bomb;

	// Token: 0x040018BA RID: 6330
	private string dialogue;

	// Token: 0x040018BB RID: 6331
	public bool isBusy;

	// Token: 0x040018BC RID: 6332
	public MissionController mc;

	// Token: 0x040018BD RID: 6333
	public int missionNum;

	// Token: 0x040018BE RID: 6334
	public Jake jake;

	// Token: 0x040018BF RID: 6335
	public GameObject lostUI;

	// Token: 0x040018C0 RID: 6336
	public FirstPersonController fpc;

	// Token: 0x040018C1 RID: 6337
	public Currency currency;

	// Token: 0x040018C2 RID: 6338
	public InventoryItems inv;

	// Token: 0x040018C3 RID: 6339
	private int ranDialogue;

	// Token: 0x040018C4 RID: 6340
	public bool pulled1;

	// Token: 0x040018C5 RID: 6341
	public bool pulled2;

	// Token: 0x040018C6 RID: 6342
	public bool pulled3;

	// Token: 0x040018C7 RID: 6343
	public ConfigurableJoint cj1;

	// Token: 0x040018C8 RID: 6344
	public ConfigurableJoint cj2;

	// Token: 0x040018C9 RID: 6345
	public ConfigurableJoint cj3;

	// Token: 0x040018CA RID: 6346
	public GameObject stump1;

	// Token: 0x040018CB RID: 6347
	public GameObject stump2;

	// Token: 0x040018CC RID: 6348
	public GameObject stump3;

	// Token: 0x040018CD RID: 6349
	public GameObject recipe;

	// Token: 0x040018CE RID: 6350
	public GameObject tree1;

	// Token: 0x040018CF RID: 6351
	public GameObject tree2;

	// Token: 0x040018D0 RID: 6352
	public GameObject tree3;

	// Token: 0x040018D1 RID: 6353
	public GameObject paddle;

	// Token: 0x040018D2 RID: 6354
	public GameObject spawnedpaddle;

	// Token: 0x040018D3 RID: 6355
	public GameObject trashprefab1;

	// Token: 0x040018D4 RID: 6356
	public GameObject trashprefab2;

	// Token: 0x040018D5 RID: 6357
	private Collider[] itemsInsideZone;

	// Token: 0x040018D6 RID: 6358
	public Transform trashLoc;

	// Token: 0x040018D7 RID: 6359
	public Transform trashLoc2;

	// Token: 0x040018D8 RID: 6360
	public Transform waterwheelLoc;
}
