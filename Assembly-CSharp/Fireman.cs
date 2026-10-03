using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000B1 RID: 177
public class Fireman : MonoBehaviour
{
	// Token: 0x06000442 RID: 1090 RVA: 0x0002CF4B File Offset: 0x0002B14B
	private void Start()
	{
		base.StartCoroutine(this.Animate());
		if (this.isBusy)
		{
			this.isBusy = false;
		}
		if (this.missionNum == 2)
		{
			this.missionNum = 3;
		}
	}

	// Token: 0x06000443 RID: 1091 RVA: 0x0002CF79 File Offset: 0x0002B179
	public IEnumerator Animate()
	{
		if (!this.isBusy)
		{
			if (Random.Range(0, 2) == 0)
			{
				AudioSource.PlayClipAtPoint(this.grinder1, this.audioLoc.transform.position, 0.7f);
				yield return new WaitForSeconds(1f);
				this.sparks.SetActive(true);
				yield return new WaitForSeconds(3f);
			}
			else
			{
				AudioSource.PlayClipAtPoint(this.grinder2, this.audioLoc.transform.position, 0.7f);
				yield return new WaitForSeconds(1f);
				this.sparks.SetActive(true);
				yield return new WaitForSeconds(1f);
			}
			this.sparks.SetActive(false);
		}
		yield return new WaitForSeconds(14f);
		base.StartCoroutine(this.Animate());
		yield break;
	}

	// Token: 0x06000444 RID: 1092 RVA: 0x0002CF88 File Offset: 0x0002B188
	public void Interact()
	{
		this.isBusy = true;
		if (this.missionNum == 0)
		{
			AudioSource.PlayClipAtPoint(this.clip1, this.audioLoc.transform.position, 0.9f);
			this.missionNum = 1;
			this.interactor.overflowvalve = 1;
			this.dialogue = "I need someone to shut off the overflow valve on the water tower. Problem is the access road is gone. The weather this year pretty much destroyed everything.";
			this.interactor.Subtitle(this.dialogue, this.clip1);
			this.mc.ActivateMission_sub(6);
			this.interactor.discoveredMission = 6;
		}
		else if (this.missionNum == 1 && this.interactor.overflowvalve == 0)
		{
			AudioSource.PlayClipAtPoint(this.clip2, this.audioLoc.transform.position, 0.9f);
			this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
			this.newRoll.GetComponent<PickUp>().thisDurability = 300f;
			this.missionNum = 2;
			this.eventSystem.GetComponent<MissionController>().CompleteMission(6);
			this.dialogue = "Here you go. I might have another job tomorrow.";
			this.interactor.Subtitle(this.dialogue, this.clip2);
		}
		else if (this.missionNum == 3)
		{
			this.deliveryZone1.GetComponent<DeliveryZone>().amountDelivered = 0;
			this.deliveryZone1.GetComponent<DeliveryZone>().delivered = false;
			AudioSource.PlayClipAtPoint(this.clip3, this.audioLoc.transform.position, 0.9f);
			this.missionNum = 4;
			this.pointer2.ActivateNextWaypoint();
			this.dialogue = "My truck is in the shop, and I have some deliveries on hold. I have this pallet of pipes I need moved. I really can't do it without some help.";
			this.interactor.Subtitle(this.dialogue, this.clip3);
			this.pipepallet.SetActive(true);
			this.mc.ActivateMission(7);
		}
		else if (this.missionNum == 4)
		{
			if (this.deliveryZone1.GetComponent<DeliveryZone>().CheckDelivery())
			{
				this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
				this.newRoll.GetComponent<PickUp>().thisDurability = 300f;
				this.missionNum = 5;
				this.mc.CompleteMission(7);
				AudioSource.PlayClipAtPoint(this.clip2, this.audioLoc.transform.position, 0.9f);
				this.deliveryZone1.GetComponent<DeliveryZone>().delivered = false;
				this.dialogue = "Here you go. I might have another job tomorrow.";
				this.interactor.Subtitle(this.dialogue, this.clip2);
				this.isBusy = true;
			}
		}
		else if (this.missionNum == 6)
		{
			this.deliveryZone2b.GetComponent<DeliveryZone>().amountDelivered = 0;
			this.deliveryZone2b.GetComponent<DeliveryZone>().delivered = false;
			AudioSource.PlayClipAtPoint(this.clip4b, this.audioLoc.transform.position, 0.9f);
			this.missionNum = 7;
			this.dialogue = "Hey, a new guy on my maintenance crew left a box of tools out in the middle of nowhere. I already had to fire the guy, so can you run and grab them for me?";
			this.interactor.Subtitle(this.dialogue, this.clip4b);
			this.toolbox.SetActive(true);
			this.mc.ActivateMission(39);
			base.StartCoroutine(this.UnBusy());
		}
		else if (this.missionNum == 7 && this.deliveryZone2b.GetComponent<DeliveryZone>().CheckDelivery())
		{
			this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
			this.newRoll.GetComponent<PickUp>().thisDurability = 300f;
			this.missionNum = 8;
			AudioSource.PlayClipAtPoint(this.clip2, this.audioLoc.transform.position, 0.9f);
			this.deliveryZone2b.GetComponent<DeliveryZone>().delivered = false;
			this.dialogue = "Here you go. I might have another job tomorrow.";
			this.interactor.Subtitle(this.dialogue, this.clip2);
			this.mc.CompleteMission(39);
			this.isBusy = true;
		}
		base.StartCoroutine(this.UnBusy());
	}

	// Token: 0x06000445 RID: 1093 RVA: 0x0002D3D3 File Offset: 0x0002B5D3
	private IEnumerator UnBusy()
	{
		yield return new WaitForSeconds(8f);
		this.isBusy = false;
		yield break;
	}

	// Token: 0x0400089F RID: 2207
	public AudioSource aSource;

	// Token: 0x040008A0 RID: 2208
	public int missionNum;

	// Token: 0x040008A1 RID: 2209
	public Transform audioLoc;

	// Token: 0x040008A2 RID: 2210
	public AudioClip clip1;

	// Token: 0x040008A3 RID: 2211
	public AudioClip clip2;

	// Token: 0x040008A4 RID: 2212
	public AudioClip clip3;

	// Token: 0x040008A5 RID: 2213
	public AudioClip clip4b;

	// Token: 0x040008A6 RID: 2214
	public AudioClip grinder1;

	// Token: 0x040008A7 RID: 2215
	public AudioClip grinder2;

	// Token: 0x040008A8 RID: 2216
	public showmission pointer1;

	// Token: 0x040008A9 RID: 2217
	public showmission pointer2;

	// Token: 0x040008AA RID: 2218
	public showmission pointer3b;

	// Token: 0x040008AB RID: 2219
	public GameObject deliveryZone1;

	// Token: 0x040008AC RID: 2220
	public GameObject deliveryZone2b;

	// Token: 0x040008AD RID: 2221
	public GameObject eventSystem;

	// Token: 0x040008AE RID: 2222
	private string dialogue;

	// Token: 0x040008AF RID: 2223
	public bool isBusy;

	// Token: 0x040008B0 RID: 2224
	public MissionController mc;

	// Token: 0x040008B1 RID: 2225
	public Interactor interactor;

	// Token: 0x040008B2 RID: 2226
	public GameObject pipepallet;

	// Token: 0x040008B3 RID: 2227
	public GameObject toolbox;

	// Token: 0x040008B4 RID: 2228
	public Animator anim;

	// Token: 0x040008B5 RID: 2229
	public GameObject sparks;

	// Token: 0x040008B6 RID: 2230
	public Transform moneyLoc;

	// Token: 0x040008B7 RID: 2231
	public GameObject moneyRoll;

	// Token: 0x040008B8 RID: 2232
	private GameObject newRoll;
}
