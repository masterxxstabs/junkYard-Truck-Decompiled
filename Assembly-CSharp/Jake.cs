using System;
using System.Collections;
using Steamworks.Data;
using UnityEngine;

// Token: 0x020000D5 RID: 213
public class Jake : MonoBehaviour
{
	// Token: 0x06000556 RID: 1366 RVA: 0x00042B18 File Offset: 0x00040D18
	private void Start()
	{
		this.isBusy = false;
		if (this.officer == null)
		{
			Officer officer = Resources.FindObjectsOfTypeAll<Officer>()[0];
			if (officer != null)
			{
				officer.gameObject.SetActive(true);
			}
			else
			{
				Debug.Log("officer not found");
			}
		}
		if (!this.spawnedFurniture)
		{
			Object.Instantiate<GameObject>(this.furniture1, this.furnitureLoc1.transform.position, this.furnitureLoc1.transform.rotation).name = "furniture";
			Object.Instantiate<GameObject>(this.furniture2, this.furnitureLoc2.transform.position, this.furnitureLoc2.transform.rotation).name = "furniture";
			Object.Instantiate<GameObject>(this.furniture3, this.furnitureLoc3.transform.position, this.furnitureLoc3.transform.rotation).name = "furniture";
			this.spawnedFurniture = true;
		}
		if (this.missionNum == 5)
		{
			this.wheel1.SetActive(true);
			this.wheel2.SetActive(true);
			this.wheel3.SetActive(true);
			this.wheel4.SetActive(true);
		}
		if (this.missionNum == 10 && !this.barUnlocked)
		{
			this.bootlegger.SetActive(true);
		}
		if (this.missionNum > 5)
		{
			this.officer.SetActive(true);
		}
		if (this.barUnlocked)
		{
			base.gameObject.SetActive(false);
			this.bootlegger.SetActive(false);
		}
		base.StartCoroutine(this.DeleteCrates());
		if (this.barUnlocked)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000557 RID: 1367 RVA: 0x00042CC4 File Offset: 0x00040EC4
	public void Interact()
	{
		this.dialogue = "";
		if (this.missionNum == 2)
		{
			if (GameObject.Find("furniture") == null && GameObject.Find("furniture1(Clone)") == null)
			{
				this.furnitureGone = true;
			}
			if (this.deliveryZone2.GetComponent<DeliveryZone>().delivered || this.furnitureGone)
			{
				AudioSource.PlayClipAtPoint(this.clip3, this.audioLoc.transform.position, 0.9f);
				this.missionNum = 3;
				this.dialogue = "Hey there you are. Thanks for doing that. I got tired of being her free storage service, you know? Here's some cash.";
				this.jakeAnim.Play("jake3");
				this.interactor.Subtitle(this.dialogue, this.clip3);
				this.deliveryZone2.SetActive(false);
				this.eventSystem.GetComponent<MissionController>().CompleteMission(15);
				this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
				this.newRoll.GetComponent<PickUp>().thisDurability = 250f;
			}
		}
		if (this.missionNum == 1)
		{
			if (this.deliveryZone1.GetComponent<DeliveryZone>().CheckDelivery())
			{
				AudioSource.PlayClipAtPoint(this.clip2, this.audioLoc.transform.position, 0.9f);
				this.missionNum = 2;
				this.dialogue = "Thanks man. Here you go. My girlfriend and I broke up a few months ago, and this is all her furniture. I was hoping you could help deliver it.";
				this.jakeAnim.Play("jake3");
				this.interactor.Subtitle(this.dialogue, this.clip2);
				this.eventSystem.GetComponent<MissionController>().CompleteMission(40);
				this.mc.ActivateMission(15);
				this.pointer2.ActivateNextWaypoint();
				this.deliveryZone1.SetActive(false);
				this.deliveryZone2.SetActive(true);
				this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
				this.newRoll.GetComponent<PickUp>().thisDurability = 25f;
				this.furniture1.GetComponent<PickUp>().pickable = true;
				this.furniture2.GetComponent<PickUp>().pickable = true;
				this.furniture3.GetComponent<PickUp>().pickable = true;
			}
			else if (!this.isBusy)
			{
				AudioSource.PlayClipAtPoint(this.clip1, this.audioLoc.transform.position, 0.9f);
				this.dialogue = "You bring that case of beer?";
				this.jakeAnim.Play("jake2");
				this.interactor.Subtitle(this.dialogue, this.clip1);
				base.StartCoroutine(this.UnBusy());
			}
		}
		if (this.missionNum == 5 && this.deliveryZone3.GetComponent<DeliveryZone>().CheckDelivery())
		{
			AudioSource.PlayClipAtPoint(this.clip5, this.audioLoc.transform.position, 0.9f);
			this.dialogue = "Nice. These will work.";
			this.jakeAnim.Play("jake3");
			this.interactor.Subtitle(this.dialogue, this.clip5);
			this.deliveryZone3.SetActive(false);
			this.eventSystem.GetComponent<MissionController>().CompleteMission(41);
			this.missionNum = 6;
			this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
			this.newRoll.GetComponent<PickUp>().thisDurability = 280f;
			this.wheel1.SetActive(false);
			this.wheel2.SetActive(false);
			this.wheel3.SetActive(false);
			this.wheel4.SetActive(false);
			this.officer.SetActive(true);
		}
		if (this.missionNum == 4)
		{
			AudioSource.PlayClipAtPoint(this.clip4, this.audioLoc.transform.position, 0.9f);
			this.deliveryZone3.SetActive(true);
			this.dialogue = "Some asshole stole the wheels off my car last night... So I ain't going anywhere until I get some new wheels. You think you can find some cheap ones for me at the junkyard? I need uh.... let's see... four of them.";
			this.jakeAnim.Play("jake2");
			this.interactor.Subtitle(this.dialogue, this.clip4);
			this.wheel1.SetActive(true);
			this.wheel2.SetActive(true);
			this.wheel3.SetActive(true);
			this.wheel4.SetActive(true);
			this.missionNum = 5;
			this.pointer3.ActivateNextWaypoint();
			this.mc.ActivateMission(41);
		}
		if (this.missionNum == 10)
		{
			if (this.mc.activeMissions.Contains(42))
			{
				this.mc.CompleteMission(42);
			}
			this.price = this.CheckMoon();
			if (this.price > 0f)
			{
				this.moonJar.SetActive(true);
				this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
				this.newRoll.GetComponent<PickUp>().thisDurability = this.price;
				float num = this.price / (float)this.crates;
				if (num >= 145f)
				{
					AudioSource.PlayClipAtPoint(this.clip10, this.audioLoc.transform.position, 0.9f);
					this.dialogue = "AGH! WOOO THAT'S STRONG!";
					this.interactor.Subtitle(this.dialogue, this.clip10);
					if (!this.barDialogue)
					{
						this.bootlegger.SetActive(true);
						base.StartCoroutine(this.OpenBar());
					}
				}
				else if (num >= 120f)
				{
					AudioSource.PlayClipAtPoint(this.clip11, this.audioLoc.transform.position, 0.9f);
					this.dialogue = "AGH! It's decent. I'll pay you for it.";
					this.interactor.Subtitle(this.dialogue, this.clip11);
				}
				else if (num >= 95f)
				{
					AudioSource.PlayClipAtPoint(this.clip12, this.audioLoc.transform.position, 0.9f);
					this.dialogue = "AGH! Wow that's rough. I think something's off. You'll have to work on that recipe. Here's some cash though.";
					this.interactor.Subtitle(this.dialogue, this.clip12);
				}
				else
				{
					AudioSource.PlayClipAtPoint(this.clip13, this.audioLoc.transform.position, 0.9f);
					this.dialogue = "UGH! Get this outa here. This ain't moonshine.";
					this.interactor.Subtitle(this.dialogue, this.clip13);
				}
				this.jakeAnim.Play("jake4");
				if (num < 145f)
				{
					this.gAnim.Play("drink");
				}
				this.price = 0f;
				this.crates = 0;
				base.StartCoroutine(this.DeleteCrates());
				if (num > 145f)
				{
					Achievement achievement = new Achievement("ACH_190");
					achievement.Trigger(true);
				}
			}
			else if (!this.isBusy && !this.barUnlocked)
			{
				AudioSource.PlayClipAtPoint(this.clip14, this.audioLoc.transform.position, 0.9f);
				this.dialogue = "Let me know when you get some moonshine.";
				this.jakeAnim.Play("jake2");
				this.interactor.Subtitle(this.dialogue, this.clip14);
				base.StartCoroutine(this.UnBusy());
			}
		}
		if (this.missionNum == 8)
		{
			this.missionNum = 10;
			AudioSource.PlayClipAtPoint(this.clip6, this.audioLoc.transform.position, 0.9f);
			this.dialogue = "My friend owns a bar just outside of town, and he's been looking for an alcohol supplier. So here's what I'm thinking- Ask Jiggs Casey to show you how to make moonshine, then make some, and I'll see if we can sell it.";
			this.jakeAnim.Play("jake2");
			this.interactor.Subtitle(this.dialogue, this.clip6);
			this.abomb.SetActive(true);
			this.jiggs.SetActive(true);
			this.mc.ActivateMission(43);
			this.mc.TrackMission_N(43);
			base.StartCoroutine(this.UnBusy());
			base.StartCoroutine(this.RevealBootlegger());
		}
		if (this.missionNum == 7)
		{
			if (!this.carTriggered && this.revengeCarBolt.GetComponent<BoltScript>().boltturns == 10)
			{
				AudioSource.PlayClipAtPoint(this.clip7, this.audioLoc.transform.position, 0.9f);
				this.dialogue = "Okay check this out. I figured out who stole my wheels. They actually live a couple streets over. I want you to get under their car and loosen their oil drain bolt. Also, bonus pay if you add some sugar to their gas tank. And do it at night... Don't forget to bring a creeper.";
				this.jakeAnim.Play("jake2");
				this.interactor.Subtitle(this.dialogue, this.clip7);
				this.carTriggered = true;
				this.pointer4.ActivateNextWaypoint();
			}
			if (this.revengeCarBolt.GetComponent<BoltScript>().boltturns < 10)
			{
				if (this.revengeCarInlet.GetComponents<BoxCollider>()[1].enabled)
				{
					AudioSource.PlayClipAtPoint(this.clip8, this.audioLoc.transform.position, 0.9f);
					this.dialogue = "You actually did it! Hahahaha! I'm going to pay you good for this one.";
					this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
					this.newRoll.GetComponent<PickUp>().thisDurability = 400f;
				}
				else
				{
					AudioSource.PlayClipAtPoint(this.clip8, this.audioLoc.transform.position, 0.9f);
					this.dialogue = "You actually did it! Hahahaha! I'm going to pay you good for this one.";
					this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
					this.newRoll.GetComponent<PickUp>().thisDurability = 300f;
				}
				this.eventSystem.GetComponent<MissionController>().CompleteMission(42);
				this.jakeAnim.Play("jake3");
				this.interactor.Subtitle(this.dialogue, this.clip8);
				this.revengeCar.SetActive(false);
				this.missionNum = 8;
			}
		}
		this.CheckTobacco();
	}

	// Token: 0x06000558 RID: 1368 RVA: 0x000436D1 File Offset: 0x000418D1
	private IEnumerator OpenBar()
	{
		yield return new WaitForSeconds(4f);
		this.gAnim.Play("explain");
		AudioSource.PlayClipAtPoint(this.clip15, this.audioLoc2.transform.position, 1f);
		this.dialogue = "I think we're selling ourselves short. I know there's a bigger market out there... I have an idea. Go into town and buy that black shipping container.";
		this.interactor.Subtitle(this.dialogue, this.clip15);
		this.barDialogue = true;
		this.bar.ToggleSign(true);
		yield break;
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x000436E0 File Offset: 0x000418E0
	public void CheckTobacco()
	{
		this.price = 0f;
		this.itemsInsideZone = Physics.OverlapSphere(base.transform.position, 5f);
		foreach (Collider collider in this.itemsInsideZone)
		{
			if (collider.name.Contains("tobaccocrate"))
			{
				this.pu = collider.GetComponent<PickUp>();
				if (this.pu.pickable)
				{
					this.price += this.pu.thisDurability * 40f;
					this.pu.pickable = false;
				}
			}
		}
		if (this.price > 0f)
		{
			if (this.dialogue == "")
			{
				AudioSource.PlayClipAtPoint(this.clip15, this.audioLoc.transform.position, 0.9f);
				this.dialogue = "Tobacco? Okay.";
				this.interactor.Subtitle(this.dialogue, this.clip15);
				this.jakeAnim.Play("jake3");
			}
			this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
			this.newRoll.GetComponent<PickUp>().thisDurability = this.price;
			base.StartCoroutine(this.DeleteBoxes());
		}
	}

	// Token: 0x0600055A RID: 1370 RVA: 0x0004384C File Offset: 0x00041A4C
	public float CheckMoon()
	{
		this.price = 0f;
		this.itemsInsideZone = Physics.OverlapSphere(base.transform.position, 4f);
		foreach (Collider collider in this.itemsInsideZone)
		{
			if (collider.name.Substring(0, 3) == "moo" && (collider.name.Contains("mooncrateF") || collider.name.Contains("mooncrateB") || collider.name.Contains("mooncrateL") || collider.name.Contains("mooncrateO") || collider.name.Contains("mooncrateA")))
			{
				this.pu = collider.GetComponent<PickUp>();
				if (this.pu.pickable)
				{
					this.price += this.pu.price;
					this.pu.pickable = false;
					this.crates++;
					this.pu.price = 0f;
				}
			}
		}
		return this.price;
	}

	// Token: 0x0600055B RID: 1371 RVA: 0x00043975 File Offset: 0x00041B75
	public void BuyTire()
	{
		this.boughtTires++;
		if (this.boughtTires > 3)
		{
			this.pointer3b.ActivateNextWaypoint();
		}
	}

	// Token: 0x0600055C RID: 1372 RVA: 0x00043999 File Offset: 0x00041B99
	private IEnumerator UnBusy()
	{
		this.isBusy = true;
		yield return new WaitForSeconds(4f);
		this.isBusy = false;
		yield break;
	}

	// Token: 0x0600055D RID: 1373 RVA: 0x000439A8 File Offset: 0x00041BA8
	private IEnumerator RevealBootlegger()
	{
		yield return new WaitForSeconds(30f);
		this.bootlegger.SetActive(true);
		yield break;
	}

	// Token: 0x0600055E RID: 1374 RVA: 0x000439B7 File Offset: 0x00041BB7
	private IEnumerator DeleteCrates()
	{
		yield return new WaitForSeconds(1f);
		this.moonJar.SetActive(false);
		this.jakeAnim.Play("jake3");
		yield return new WaitForSeconds(10f);
		this.itemsInsideZone = Physics.OverlapSphere(base.transform.position, 4f);
		foreach (Collider collider in this.itemsInsideZone)
		{
			if (collider.name.Contains("mooncrateF") || collider.name.Contains("mooncrateB") || collider.name.Contains("mooncrateL") || collider.name.Contains("mooncrateO") || collider.name.Contains("mooncrateA"))
			{
				this.pu = collider.GetComponent<PickUp>();
				if (!this.pu.pickable)
				{
					Object.Destroy(collider.gameObject);
				}
			}
		}
		yield break;
	}

	// Token: 0x0600055F RID: 1375 RVA: 0x000439C6 File Offset: 0x00041BC6
	private IEnumerator DeleteBoxes()
	{
		yield return new WaitForSeconds(1f);
		this.jakeAnim.Play("jake3");
		yield return new WaitForSeconds(10f);
		this.itemsInsideZone = Physics.OverlapSphere(base.transform.position, 4f);
		foreach (Collider collider in this.itemsInsideZone)
		{
			if (collider.name.Contains("tobaccocrate"))
			{
				this.pu = collider.GetComponent<PickUp>();
				if (!this.pu.pickable)
				{
					Object.Destroy(collider.gameObject);
				}
			}
		}
		yield break;
	}

	// Token: 0x04000B50 RID: 2896
	public GameObject moonshineSphere;

	// Token: 0x04000B51 RID: 2897
	public AudioSource aSource;

	// Token: 0x04000B52 RID: 2898
	public Transform audioLoc;

	// Token: 0x04000B53 RID: 2899
	public Transform audioLoc2;

	// Token: 0x04000B54 RID: 2900
	public int missionNum;

	// Token: 0x04000B55 RID: 2901
	public AudioClip clip1;

	// Token: 0x04000B56 RID: 2902
	public AudioClip clip2;

	// Token: 0x04000B57 RID: 2903
	public AudioClip clip3;

	// Token: 0x04000B58 RID: 2904
	public AudioClip clip4;

	// Token: 0x04000B59 RID: 2905
	public AudioClip clip5;

	// Token: 0x04000B5A RID: 2906
	public AudioClip clip6;

	// Token: 0x04000B5B RID: 2907
	public AudioClip clip7;

	// Token: 0x04000B5C RID: 2908
	public AudioClip clip8;

	// Token: 0x04000B5D RID: 2909
	public AudioClip clip9;

	// Token: 0x04000B5E RID: 2910
	public AudioClip clip10;

	// Token: 0x04000B5F RID: 2911
	public AudioClip clip11;

	// Token: 0x04000B60 RID: 2912
	public AudioClip clip12;

	// Token: 0x04000B61 RID: 2913
	public AudioClip clip13;

	// Token: 0x04000B62 RID: 2914
	public AudioClip clip14;

	// Token: 0x04000B63 RID: 2915
	public AudioClip clip15;

	// Token: 0x04000B64 RID: 2916
	public showmission pointer1;

	// Token: 0x04000B65 RID: 2917
	public showmission pointer2;

	// Token: 0x04000B66 RID: 2918
	public showmission pointer3;

	// Token: 0x04000B67 RID: 2919
	public showmission pointer3b;

	// Token: 0x04000B68 RID: 2920
	public showmission pointer4;

	// Token: 0x04000B69 RID: 2921
	public showmission pointer5;

	// Token: 0x04000B6A RID: 2922
	public GameObject deliveryZone1;

	// Token: 0x04000B6B RID: 2923
	public GameObject deliveryZone2;

	// Token: 0x04000B6C RID: 2924
	public GameObject deliveryZone3;

	// Token: 0x04000B6D RID: 2925
	public GameObject deliveryZone4;

	// Token: 0x04000B6E RID: 2926
	public GameObject eventSystem;

	// Token: 0x04000B6F RID: 2927
	private string dialogue;

	// Token: 0x04000B70 RID: 2928
	public bool isBusy;

	// Token: 0x04000B71 RID: 2929
	public MissionController mc;

	// Token: 0x04000B72 RID: 2930
	public Interactor interactor;

	// Token: 0x04000B73 RID: 2931
	public GameObject wheel1;

	// Token: 0x04000B74 RID: 2932
	public GameObject wheel2;

	// Token: 0x04000B75 RID: 2933
	public GameObject wheel3;

	// Token: 0x04000B76 RID: 2934
	public GameObject wheel4;

	// Token: 0x04000B77 RID: 2935
	public GameObject washingmachine;

	// Token: 0x04000B78 RID: 2936
	public GameObject revengeCar;

	// Token: 0x04000B79 RID: 2937
	public GameObject revengeCarInlet;

	// Token: 0x04000B7A RID: 2938
	public GameObject revengeCarBolt;

	// Token: 0x04000B7B RID: 2939
	public GameObject abomb;

	// Token: 0x04000B7C RID: 2940
	public GameObject jiggs;

	// Token: 0x04000B7D RID: 2941
	public int boughtTires;

	// Token: 0x04000B7E RID: 2942
	private Collider[] itemsInsideZone;

	// Token: 0x04000B7F RID: 2943
	private PickUp pu;

	// Token: 0x04000B80 RID: 2944
	private float price;

	// Token: 0x04000B81 RID: 2945
	private int crates;

	// Token: 0x04000B82 RID: 2946
	private int firstMoonshine;

	// Token: 0x04000B83 RID: 2947
	public bool moonTank;

	// Token: 0x04000B84 RID: 2948
	public bool moonBag;

	// Token: 0x04000B85 RID: 2949
	public GameObject furniture1;

	// Token: 0x04000B86 RID: 2950
	public GameObject furniture2;

	// Token: 0x04000B87 RID: 2951
	public GameObject furniture3;

	// Token: 0x04000B88 RID: 2952
	public Transform furnitureLoc1;

	// Token: 0x04000B89 RID: 2953
	public Transform furnitureLoc2;

	// Token: 0x04000B8A RID: 2954
	public Transform furnitureLoc3;

	// Token: 0x04000B8B RID: 2955
	[SerializeField]
	private bool spawnedFurniture;

	// Token: 0x04000B8C RID: 2956
	public Transform moneyLoc;

	// Token: 0x04000B8D RID: 2957
	public GameObject moneyRoll;

	// Token: 0x04000B8E RID: 2958
	private GameObject newRoll;

	// Token: 0x04000B8F RID: 2959
	public Animator jakeAnim;

	// Token: 0x04000B90 RID: 2960
	public Animator gAnim;

	// Token: 0x04000B91 RID: 2961
	public GameObject moonJar;

	// Token: 0x04000B92 RID: 2962
	public bool busy;

	// Token: 0x04000B93 RID: 2963
	public bool furnitureGone;

	// Token: 0x04000B94 RID: 2964
	private bool carTriggered;

	// Token: 0x04000B95 RID: 2965
	public bool barUnlocked;

	// Token: 0x04000B96 RID: 2966
	public bool barDialogue;

	// Token: 0x04000B97 RID: 2967
	public CargoBar bar;

	// Token: 0x04000B98 RID: 2968
	public GameObject bootlegger;

	// Token: 0x04000B99 RID: 2969
	public GameObject officer;
}
