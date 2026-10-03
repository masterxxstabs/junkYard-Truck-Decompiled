using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000D6 RID: 214
public class Jake8 : MonoBehaviour
{
	// Token: 0x06000561 RID: 1377 RVA: 0x000439D8 File Offset: 0x00041BD8
	private void Start()
	{
		if (this.displayTime > 0f)
		{
			this.display.SetActive(true);
		}
		if (this.displayBTime > 0f)
		{
			this.displayB.SetActive(true);
		}
		if (this.displayLTime > 0f)
		{
			this.displayL.SetActive(true);
		}
		if (this.displayOTime > 0f)
		{
			this.displayO.SetActive(true);
		}
		if (this.displayATime > 0f)
		{
			this.displayA.SetActive(true);
		}
		if (this.displayTTime > 0f)
		{
			this.displayT.SetActive(true);
		}
		this.officer.SetActive(true);
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x00043A88 File Offset: 0x00041C88
	private void Update()
	{
		this.elapsed += Time.deltaTime;
		if (this.elapsed >= this.clipLength)
		{
			int num = Random.Range(0, 100);
			if (num < 16)
			{
				this.clipLength = 35.1f;
				this.anim.Play("bar2");
			}
			else if (num < 50)
			{
				this.clipLength = 3.1f;
				this.anim.Play("bar1");
			}
			else
			{
				this.clipLength = 9.2f;
				this.anim.Play("bar3");
			}
			this.elapsed = 0f;
			this.RefreshDisplay();
		}
	}

	// Token: 0x06000563 RID: 1379 RVA: 0x00043B30 File Offset: 0x00041D30
	private void RefreshDisplay()
	{
		if (this.displayTime > 0f)
		{
			this.displayTime -= Time.deltaTime;
			if (this.displayTime <= 0f)
			{
				this.display.SetActive(false);
			}
		}
		if (this.displayBTime > 0f)
		{
			this.displayBTime -= Time.deltaTime;
			if (this.displayBTime <= 0f)
			{
				this.displayB.SetActive(false);
			}
		}
		if (this.displayLTime > 0f)
		{
			this.displayLTime -= Time.deltaTime;
			if (this.displayLTime <= 0f)
			{
				this.displayL.SetActive(false);
			}
		}
		if (this.displayOTime > 0f)
		{
			this.displayOTime -= Time.deltaTime;
			if (this.displayOTime <= 0f)
			{
				this.displayO.SetActive(false);
			}
		}
		if (this.displayATime > 0f)
		{
			this.displayATime -= Time.deltaTime;
			if (this.displayATime <= 0f)
			{
				this.displayA.SetActive(false);
			}
		}
		if (this.displayTTime > 0f)
		{
			this.displayTTime -= Time.deltaTime;
			if (this.displayTTime <= 0f)
			{
				this.displayT.SetActive(false);
			}
		}
	}

	// Token: 0x06000564 RID: 1380 RVA: 0x00043C90 File Offset: 0x00041E90
	public void InteractB()
	{
		if (!this.isBusy)
		{
			this.dialogue = "";
			this.price = this.CheckMoon();
			if (this.price == 0f && this.infuserMission)
			{
				AudioSource.PlayClipAtPoint(this.clip2, this.audioLocB.transform.position, 0.9f);
				this.dialogue = "Keep bringing me your moonshine and tobacco. Only this time the profits will be bigger.";
				this.interactor.Subtitle(this.dialogue, this.clip2);
				base.StartCoroutine(this.UnBusy());
			}
			else if (this.price == 0f && !this.infuserMission)
			{
				AudioSource.PlayClipAtPoint(this.clip8, this.audioLocB.transform.position, 0.9f);
				this.dialogue = "I need you to find some limes, oranges, blackberries, and ambrosia. I need lime moonshine, orange moonshine, blackberry moonshine, and ambrosia moonshine.";
				this.interactor.Subtitle(this.dialogue, this.clip8);
				this.infuserMission = true;
				this.mc.ActivateMission(60);
				base.StartCoroutine(this.UnBusy());
			}
			if (this.price > 0f)
			{
				this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
				this.newRoll.GetComponent<PickUp>().thisDurability = this.price;
				float num = this.price / (float)this.crates;
				if (num >= 145f)
				{
					AudioSource.PlayClipAtPoint(this.clip6, this.audioLoc.transform.position, 0.9f);
					this.dialogue = "AGH! WOOO THAT'S STRONG!";
					this.interactor.Subtitle(this.dialogue, this.clip6);
				}
				else if (num >= 120f)
				{
					AudioSource.PlayClipAtPoint(this.clip5, this.audioLoc.transform.position, 0.9f);
					this.dialogue = "AGH! It's decent. I'll pay you for it.";
					this.interactor.Subtitle(this.dialogue, this.clip5);
				}
				else if (num >= 95f)
				{
					AudioSource.PlayClipAtPoint(this.clip4, this.audioLoc.transform.position, 0.9f);
					this.dialogue = "AGH! Wow that's rough. I think something's off. You'll have to work on that recipe. Here's some cash though.";
					this.interactor.Subtitle(this.dialogue, this.clip4);
				}
				else
				{
					AudioSource.PlayClipAtPoint(this.clip3, this.audioLoc.transform.position, 0.9f);
					this.dialogue = "UGH! Get this outa here. This ain't moonshine.";
					this.interactor.Subtitle(this.dialogue, this.clip3);
				}
				this.price = 0f;
				this.crates = 0;
				base.StartCoroutine(this.DeleteCrates());
			}
		}
		this.CheckTobacco();
	}

	// Token: 0x06000565 RID: 1381 RVA: 0x00043F54 File Offset: 0x00042154
	public void Interact()
	{
		if (!this.isBusy)
		{
			this.dialogue = "";
			this.price = this.CheckMoon();
			if (this.price == 0f)
			{
				AudioSource.PlayClipAtPoint(this.clip1, this.audioLoc.transform.position, 0.9f);
				this.dialogue = "Hey look bud, let me know when you get some moonshine.";
				this.interactor.Subtitle(this.dialogue, this.clip1);
				base.StartCoroutine(this.UnBusy());
			}
			if (this.price > 0f)
			{
				this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
				this.newRoll.GetComponent<PickUp>().thisDurability = this.price;
				float num = this.price / (float)this.crates;
				if (num >= 145f)
				{
					AudioSource.PlayClipAtPoint(this.clip6, this.audioLoc.transform.position, 0.9f);
					this.dialogue = "AGH! WOOO THAT'S STRONG!";
					this.interactor.Subtitle(this.dialogue, this.clip6);
				}
				else if (num >= 120f)
				{
					AudioSource.PlayClipAtPoint(this.clip5, this.audioLoc.transform.position, 0.9f);
					this.dialogue = "AGH! It's decent. I'll pay you for it.";
					this.interactor.Subtitle(this.dialogue, this.clip5);
				}
				else if (num >= 95f)
				{
					AudioSource.PlayClipAtPoint(this.clip4, this.audioLoc.transform.position, 0.9f);
					this.dialogue = "AGH! Wow that's rough. I think something's off. You'll have to work on that recipe. Here's some cash though.";
					this.interactor.Subtitle(this.dialogue, this.clip4);
				}
				else
				{
					AudioSource.PlayClipAtPoint(this.clip3, this.audioLoc.transform.position, 0.9f);
					this.dialogue = "UGH! Get this outa here. This ain't moonshine.";
					this.interactor.Subtitle(this.dialogue, this.clip3);
				}
				if (this.lime && this.orange && this.blackberry && this.ambrosia && this.bootLeggerMission == 0)
				{
					this.bootLeggerMission = 1;
					this.mc.CompleteMission(60);
					this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLocB.transform.position, this.moneyLocB.transform.rotation);
					this.newRoll.GetComponent<PickUp>().thisDurability = 1000f;
					AudioSource.PlayClipAtPoint(this.clip9, this.audioLocB.transform.position, 0.9f);
					this.dialogue = "Okay, perfect!";
					this.interactor.Subtitle(this.dialogue, this.clip9);
				}
				this.price = 0f;
				this.crates = 0;
				base.StartCoroutine(this.DeleteCrates());
			}
		}
		this.CheckTobacco();
	}

	// Token: 0x06000566 RID: 1382 RVA: 0x0004426C File Offset: 0x0004246C
	public float CheckMoon()
	{
		this.price = 0f;
		this.itemsInsideZone = Physics.OverlapSphere(this.boxDetector.position, 4f);
		foreach (Collider collider in this.itemsInsideZone)
		{
			if (collider.name.Substring(0, 3) == "moo" && (collider.name.Contains("mooncrateF") || collider.name.Contains("mooncrateB") || collider.name.Contains("mooncrateL") || collider.name.Contains("mooncrateO") || collider.name.Contains("mooncrateA")))
			{
				if (collider.name.Contains("mooncrateF"))
				{
					this.display.SetActive(true);
					this.displayTime += 1200f;
				}
				if (collider.name.Contains("mooncrateB"))
				{
					this.displayB.SetActive(true);
					this.displayBTime += 1200f;
					this.blackberry = true;
				}
				if (collider.name.Contains("mooncrateO"))
				{
					this.displayO.SetActive(true);
					this.displayOTime += 1200f;
					this.orange = true;
				}
				if (collider.name.Contains("mooncrateL"))
				{
					this.displayL.SetActive(true);
					this.displayLTime += 1200f;
					this.lime = true;
				}
				if (collider.name.Contains("mooncrateA"))
				{
					this.displayA.SetActive(true);
					this.displayATime += 1200f;
					this.ambrosia = true;
				}
				this.pu = collider.GetComponent<PickUp>();
				if (this.pu.pickable)
				{
					this.price += (float)Mathf.RoundToInt(this.pu.price * 1.1f);
					this.pu.pickable = false;
					this.crates++;
					this.pu.price = 0f;
				}
			}
		}
		return this.price;
	}

	// Token: 0x06000567 RID: 1383 RVA: 0x000444B0 File Offset: 0x000426B0
	public void CheckTobacco()
	{
		this.price = 0f;
		this.itemsInsideZone = Physics.OverlapSphere(this.boxDetector.position, 5f);
		foreach (Collider collider in this.itemsInsideZone)
		{
			if (collider.name.Contains("tobaccocrate"))
			{
				this.pu = collider.GetComponent<PickUp>();
				if (this.pu.pickable)
				{
					this.displayT.SetActive(true);
					this.displayTTime += 1200f;
					this.price += this.pu.thisDurability * 50f;
					this.pu.pickable = false;
				}
			}
		}
		if (this.price > 0f)
		{
			if (this.dialogue == "")
			{
				AudioSource.PlayClipAtPoint(this.clip7, this.audioLoc.transform.position, 0.9f);
				this.dialogue = "Tobacco? Okay.";
				this.interactor.Subtitle(this.dialogue, this.clip7);
			}
			this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
			this.newRoll.GetComponent<PickUp>().thisDurability = this.price;
			base.StartCoroutine(this.DeleteBoxes());
		}
	}

	// Token: 0x06000568 RID: 1384 RVA: 0x0004462A File Offset: 0x0004282A
	private IEnumerator DeleteBoxes()
	{
		yield return new WaitForSeconds(10f);
		this.itemsInsideZone = Physics.OverlapSphere(this.boxDetector.position, 4f);
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

	// Token: 0x06000569 RID: 1385 RVA: 0x00044639 File Offset: 0x00042839
	private IEnumerator DeleteCrates()
	{
		yield return new WaitForSeconds(10f);
		this.itemsInsideZone = Physics.OverlapSphere(this.boxDetector.position, 4f);
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

	// Token: 0x0600056A RID: 1386 RVA: 0x00044648 File Offset: 0x00042848
	private IEnumerator UnBusy()
	{
		this.isBusy = true;
		yield return new WaitForSeconds(4f);
		this.isBusy = false;
		yield break;
	}

	// Token: 0x04000B9A RID: 2970
	public Animator anim;

	// Token: 0x04000B9B RID: 2971
	private float elapsed;

	// Token: 0x04000B9C RID: 2972
	private float clipLength = 3.1f;

	// Token: 0x04000B9D RID: 2973
	private float price;

	// Token: 0x04000B9E RID: 2974
	private string dialogue;

	// Token: 0x04000B9F RID: 2975
	private Collider[] itemsInsideZone;

	// Token: 0x04000BA0 RID: 2976
	private PickUp pu;

	// Token: 0x04000BA1 RID: 2977
	private int crates;

	// Token: 0x04000BA2 RID: 2978
	public Transform moneyLoc;

	// Token: 0x04000BA3 RID: 2979
	public Transform moneyLocB;

	// Token: 0x04000BA4 RID: 2980
	public GameObject moneyRoll;

	// Token: 0x04000BA5 RID: 2981
	private GameObject newRoll;

	// Token: 0x04000BA6 RID: 2982
	public Transform audioLoc;

	// Token: 0x04000BA7 RID: 2983
	public Transform audioLocB;

	// Token: 0x04000BA8 RID: 2984
	public AudioClip clip1;

	// Token: 0x04000BA9 RID: 2985
	public AudioClip clip2;

	// Token: 0x04000BAA RID: 2986
	public AudioClip clip3;

	// Token: 0x04000BAB RID: 2987
	public AudioClip clip4;

	// Token: 0x04000BAC RID: 2988
	public AudioClip clip5;

	// Token: 0x04000BAD RID: 2989
	public AudioClip clip6;

	// Token: 0x04000BAE RID: 2990
	public AudioClip clip7;

	// Token: 0x04000BAF RID: 2991
	public AudioClip clip8;

	// Token: 0x04000BB0 RID: 2992
	public AudioClip clip9;

	// Token: 0x04000BB1 RID: 2993
	public Interactor interactor;

	// Token: 0x04000BB2 RID: 2994
	private bool isBusy;

	// Token: 0x04000BB3 RID: 2995
	public GameObject display;

	// Token: 0x04000BB4 RID: 2996
	public GameObject displayB;

	// Token: 0x04000BB5 RID: 2997
	public GameObject displayL;

	// Token: 0x04000BB6 RID: 2998
	public GameObject displayO;

	// Token: 0x04000BB7 RID: 2999
	public GameObject displayA;

	// Token: 0x04000BB8 RID: 3000
	public GameObject displayT;

	// Token: 0x04000BB9 RID: 3001
	public float displayTime;

	// Token: 0x04000BBA RID: 3002
	public float displayBTime;

	// Token: 0x04000BBB RID: 3003
	public float displayLTime;

	// Token: 0x04000BBC RID: 3004
	public float displayOTime;

	// Token: 0x04000BBD RID: 3005
	public float displayATime;

	// Token: 0x04000BBE RID: 3006
	public float displayTTime;

	// Token: 0x04000BBF RID: 3007
	public Transform boxDetector;

	// Token: 0x04000BC0 RID: 3008
	public GameObject officer;

	// Token: 0x04000BC1 RID: 3009
	private bool infuserMission;

	// Token: 0x04000BC2 RID: 3010
	private bool lime;

	// Token: 0x04000BC3 RID: 3011
	private bool orange;

	// Token: 0x04000BC4 RID: 3012
	private bool blackberry;

	// Token: 0x04000BC5 RID: 3013
	private bool ambrosia;

	// Token: 0x04000BC6 RID: 3014
	public MissionController mc;

	// Token: 0x04000BC7 RID: 3015
	public int bootLeggerMission;
}
