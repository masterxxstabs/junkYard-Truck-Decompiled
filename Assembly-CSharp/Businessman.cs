using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000030 RID: 48
public class Businessman : MonoBehaviour
{
	// Token: 0x060000D7 RID: 215 RVA: 0x0000A9F0 File Offset: 0x00008BF0
	private void Start()
	{
		if (this.missionNum == 19)
		{
			this.xltree.SetActive(true);
		}
		else
		{
			this.xltree.SetActive(false);
		}
		if (!this.mailscript.receivedEmails.Contains(6))
		{
			if (this.missionNum > 1 && !this.mailscript.receivedEmails.Contains(6))
			{
				this.mailscript.receivedEmails.Add(6);
			}
			if (this.missionNum > 6 && !this.mailscript.receivedEmails.Contains(7))
			{
				this.mailscript.receivedEmails.Add(7);
			}
		}
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x0000AA90 File Offset: 0x00008C90
	public void Interact()
	{
		if (this.missionNum == 0)
		{
			AudioSource.PlayClipAtPoint(this.clip1, this.audioLoc.transform.position, 0.9f);
			this.missionNum = 1;
			this.interactor.towerstatus1 = 1;
			this.pointer.ActivateNextWaypoint();
			this.dialogue = "You here for that job? My service guy quit and I have all these cell towers that need maintenance. Easy work, but the problem is getting up there. If you're able to get up there, all you need to do is flip a breaker switch. You'll also have to find a way to hop the fence since I can't find the gate key.";
			this.interactor.Subtitle(this.dialogue, this.clip1);
		}
		if (this.missionNum == 1 && this.interactor.towerstatus1 == 0)
		{
			AudioSource.PlayClipAtPoint(this.clip2, this.audioLoc.transform.position, 0.9f);
			this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
			this.newRoll.GetComponent<PickUp>().thisDurability = 300f;
			this.missionNum = 2;
			this.eventSystem.GetComponent<MissionController>().CompleteMission(4);
			this.mailscript.receivedEmails.Add(6);
			this.mailscript.RefreshEmail();
			this.dialogue = "Looks like the tower is back online. Here's for your trouble.\nWait up. I'll have some more work soon if you're interested. I'll send you a message.";
			this.interactor.Subtitle(this.dialogue, this.clip2);
		}
		if (this.missionNum == 3)
		{
			this.deliveryZone1.GetComponent<DeliveryZone>().amountDelivered = 0;
			this.deliveryZone1.GetComponent<DeliveryZone>().delivered = false;
			AudioSource.PlayClipAtPoint(this.clip3, this.audioLoc.transform.position, 0.9f);
			this.missionNum = 4;
			this.pointer2.ActivateNextWaypoint();
			this.dialogue = "Got this pallet of steel beams I need hauled up to one of my towers.";
			this.interactor.Subtitle(this.dialogue, this.clip3);
		}
		if (this.missionNum == 4 && this.deliveryZone1.GetComponent<DeliveryZone>().CheckDelivery())
		{
			this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
			this.newRoll.GetComponent<PickUp>().thisDurability = 300f;
			this.missionNum = 5;
			this.eventSystem.GetComponent<MissionController>().CompleteMission(35);
			AudioSource.PlayClipAtPoint(this.clip4, this.audioLoc.transform.position, 0.9f);
			this.deliveryZone1.GetComponent<DeliveryZone>().delivered = false;
			this.dialogue = "Thank you. Here you go.";
			this.interactor.Subtitle(this.dialogue, this.clip4);
			this.isBusy = true;
			base.StartCoroutine(this.UnBusy());
		}
		if (this.missionNum == 5 && !this.isBusy)
		{
			this.missionNum = 6;
		}
		if (this.missionNum == 6)
		{
			this.mc.ActivateMission(36);
			AudioSource.PlayClipAtPoint(this.clip5, this.audioLoc.transform.position, 0.9f);
			this.missionNum = 7;
			this.pointer3.ActivateNextWaypoint();
			this.dialogue = "One of my towers is getting some interference. Probaly a narco antenna. Once in a while some asshole will strap their own antenna to one of my towers. See if you can find it, and pull it off.";
			this.interactor.Subtitle(this.dialogue, this.clip5);
			this.narco1.SetActive(true);
		}
		if (this.missionNum == 7 && (!this.narco1.GetComponent<Rigidbody>().isKinematic || this.narco1 == null))
		{
			AudioSource.PlayClipAtPoint(this.clip2, this.audioLoc.transform.position, 0.9f);
			this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
			this.newRoll.GetComponent<PickUp>().thisDurability = 300f;
			this.missionNum = 8;
			this.eventSystem.GetComponent<MissionController>().CompleteMission(36);
			this.dialogue = "Looks like the tower is back online. Here's for your trouble.\nWait up. I'll have some more work soon if you're interested. I'll send you a message.";
			this.interactor.Subtitle(this.dialogue, this.clip4);
			if (!this.mailscript.receivedEmails.Contains(7))
			{
				this.mailscript.receivedEmails.Add(7);
			}
			this.mailscript.RefreshEmail();
		}
		if (this.missionNum == 9)
		{
			this.deliveryZone2.GetComponent<DeliveryZone>().amountDelivered = 0;
			this.deliveryZone2.GetComponent<DeliveryZone>().delivered = false;
			AudioSource.PlayClipAtPoint(this.clip6, this.audioLoc.transform.position, 0.9f);
			this.missionNum = 10;
			this.pointer4.ActivateNextWaypoint();
			this.dialogue = "I have some small generators I need delivered.";
			this.interactor.Subtitle(this.dialogue, this.clip6);
		}
		if (this.missionNum == 10 && this.deliveryZone2.GetComponent<DeliveryZone>().CheckDelivery())
		{
			this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
			this.newRoll.GetComponent<PickUp>().thisDurability = 300f;
			this.missionNum = 11;
			this.eventSystem.GetComponent<MissionController>().CompleteMission(37);
			AudioSource.PlayClipAtPoint(this.clip4, this.audioLoc.transform.position, 0.9f);
			this.deliveryZone2.GetComponent<DeliveryZone>().delivered = false;
			this.deliveryZone2.GetComponent<DeliveryZone>().amountDelivered = 0;
			this.dialogue = "Thank you. Here you go.";
			this.interactor.Subtitle(this.dialogue, this.clip4);
			this.isBusy = true;
			base.StartCoroutine(this.UnBusy());
		}
		if (this.missionNum == 11 && !this.isBusy)
		{
			this.missionNum = 12;
		}
		if (this.missionNum == 12)
		{
			this.welder.SetActive(true);
			this.mc.ActivateMission(38);
			this.deliveryZone3.GetComponent<DeliveryZone>().amountDelivered = 0;
			this.deliveryZone3.GetComponent<DeliveryZone>().delivered = false;
			AudioSource.PlayClipAtPoint(this.clip7, this.audioLoc.transform.position, 0.9f);
			this.missionNum = 13;
			this.pointer5.AddWaypoint();
			this.dialogue = "I need this welder delivered. Be careful with it. This damn thing is expensive.";
			this.interactor.Subtitle(this.dialogue, this.clip7);
		}
		if (this.missionNum == 13 && this.deliveryZone3.GetComponent<DeliveryZone>().CheckDelivery())
		{
			this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
			this.newRoll.GetComponent<PickUp>().thisDurability = 300f;
			this.missionNum = 14;
			this.eventSystem.GetComponent<MissionController>().CompleteMission(38);
			AudioSource.PlayClipAtPoint(this.clip4, this.audioLoc.transform.position, 0.9f);
			this.deliveryZone3.GetComponent<DeliveryZone>().delivered = false;
			this.dialogue = "Thank you. Here you go.";
			this.interactor.Subtitle(this.dialogue, this.clip4);
			this.isBusy = true;
			base.StartCoroutine(this.UnBusy());
		}
		if (this.missionNum == 14 && !this.isBusy)
		{
			this.missionNum = 15;
		}
		if (this.missionNum == 15)
		{
			this.generatorTrailer.SetActive(true);
			this.mc.ActivateMission(65);
			this.deliveryZone4.GetComponent<DeliveryZone>().amountDelivered = 0;
			this.deliveryZone4.GetComponent<DeliveryZone>().delivered = false;
			AudioSource.PlayClipAtPoint(this.clip8, this.audioLoc.transform.position, 0.9f);
			this.missionNum = 16;
			this.pointer65.ActivateNextWaypoint();
			this.dialogue = "I need this generator towed to one of my job sites. Don't let looks deceive you. This thing is fuckin' heavy.";
			this.interactor.Subtitle(this.dialogue, this.clip8);
		}
		if (this.missionNum == 16 && this.deliveryZone4.GetComponent<DeliveryZone>().CheckDelivery())
		{
			this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
			this.newRoll.GetComponent<PickUp>().thisDurability = 300f;
			this.missionNum = 17;
			this.eventSystem.GetComponent<MissionController>().CompleteMission(65);
			AudioSource.PlayClipAtPoint(this.clip4, this.audioLoc.transform.position, 0.9f);
			this.deliveryZone4.GetComponent<DeliveryZone>().delivered = false;
			this.dialogue = "Thank you. Here you go.";
			this.interactor.Subtitle(this.dialogue, this.clip4);
			this.isBusy = true;
			this.generatorTrailer.SetActive(false);
			base.StartCoroutine(this.UnBusy());
		}
		if (this.missionNum == 17 && !this.isBusy)
		{
			this.missionNum = 18;
		}
		if (this.missionNum == 18 && (this.officer.numArrests > 0 || this.officer.warrant || this.officer.activeCitations > 1))
		{
			this.mc.ActivateMission(66);
			AudioSource.PlayClipAtPoint(this.clip9, this.audioLoc.transform.position, 0.9f);
			this.missionNum = 19;
			this.pointer66.ActivateNextWaypoint();
			this.dialogue = "I get the impression you're not afraid of breaking the law. Which is great because I have some questionable work I need done. A large tree is causing interference with one of my towers, and I don't have a permit to cut it down. So I want you to take care of it.";
			this.interactor.Subtitle(this.dialogue, this.clip9);
			this.xltree.SetActive(true);
		}
		else if (this.missionNum == 18)
		{
			AudioSource.PlayClipAtPoint(this.clip10, this.audioLoc.transform.position, 0.9f);
			this.dialogue = "I have one more piece of work, but it isn't exactly legal. So I'm not sure you're the right person I should be asking.";
			this.interactor.Subtitle(this.dialogue, this.clip10);
		}
		if (this.missionNum == 19 && (this.xltree.transform.GetChild(0).GetComponent<treeinfo>().dead || this.xltree.transform.GetChild(0).GetComponent<treeinfo>().falling))
		{
			AudioSource.PlayClipAtPoint(this.clip4, this.audioLoc.transform.position, 0.9f);
			this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
			this.newRoll.GetComponent<PickUp>().thisDurability = 300f;
			this.missionNum = 20;
			this.eventSystem.GetComponent<MissionController>().CompleteMission(66);
			this.dialogue = "Thank you. Here you go.";
			this.interactor.Subtitle(this.dialogue, this.clip4);
		}
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x0000B58F File Offset: 0x0000978F
	public void TreeCutUpdate()
	{
		this.pointer66.RemoveWaypoint();
		this.pointer66b.nextWaypoint.GetComponent<showmission>().AddWaypoint();
	}

	// Token: 0x060000DA RID: 218 RVA: 0x0000B5B1 File Offset: 0x000097B1
	private IEnumerator UnBusy()
	{
		yield return new WaitForSeconds(2f);
		this.isBusy = false;
		yield break;
	}

	// Token: 0x04000213 RID: 531
	public AudioSource aSource;

	// Token: 0x04000214 RID: 532
	public int missionNum;

	// Token: 0x04000215 RID: 533
	public Interactor interactor;

	// Token: 0x04000216 RID: 534
	public GameObject eventSystem;

	// Token: 0x04000217 RID: 535
	public AudioClip clip1;

	// Token: 0x04000218 RID: 536
	public AudioClip clip2;

	// Token: 0x04000219 RID: 537
	public AudioClip clip3;

	// Token: 0x0400021A RID: 538
	public AudioClip clip4;

	// Token: 0x0400021B RID: 539
	public AudioClip clip5;

	// Token: 0x0400021C RID: 540
	public AudioClip clip6;

	// Token: 0x0400021D RID: 541
	public AudioClip clip7;

	// Token: 0x0400021E RID: 542
	public AudioClip clip8;

	// Token: 0x0400021F RID: 543
	public AudioClip clip9;

	// Token: 0x04000220 RID: 544
	public AudioClip clip10;

	// Token: 0x04000221 RID: 545
	public Transform audioLoc;

	// Token: 0x04000222 RID: 546
	public MailScript mailscript;

	// Token: 0x04000223 RID: 547
	public showmission pointer;

	// Token: 0x04000224 RID: 548
	public showmission pointer2;

	// Token: 0x04000225 RID: 549
	public showmission pointer3;

	// Token: 0x04000226 RID: 550
	public showmission pointer4;

	// Token: 0x04000227 RID: 551
	public showmission pointer5;

	// Token: 0x04000228 RID: 552
	public showmission pointer65;

	// Token: 0x04000229 RID: 553
	public showmission pointer66;

	// Token: 0x0400022A RID: 554
	public showmission pointer66b;

	// Token: 0x0400022B RID: 555
	public GameObject deliveryZone1;

	// Token: 0x0400022C RID: 556
	public GameObject deliveryZone2;

	// Token: 0x0400022D RID: 557
	public GameObject deliveryZone3;

	// Token: 0x0400022E RID: 558
	public GameObject deliveryZone4;

	// Token: 0x0400022F RID: 559
	public GameObject welder;

	// Token: 0x04000230 RID: 560
	public GameObject narco1;

	// Token: 0x04000231 RID: 561
	private string dialogue;

	// Token: 0x04000232 RID: 562
	public bool isBusy;

	// Token: 0x04000233 RID: 563
	public MissionController mc;

	// Token: 0x04000234 RID: 564
	public Transform moneyLoc;

	// Token: 0x04000235 RID: 565
	public GameObject moneyRoll;

	// Token: 0x04000236 RID: 566
	private GameObject newRoll;

	// Token: 0x04000237 RID: 567
	public GameObject generatorTrailer;

	// Token: 0x04000238 RID: 568
	public GameObject xltree;

	// Token: 0x04000239 RID: 569
	public Officer officer;
}
