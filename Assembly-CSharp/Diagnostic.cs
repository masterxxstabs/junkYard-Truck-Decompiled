using System;
using System.Collections;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200004B RID: 75
public class Diagnostic : MonoBehaviour
{
	// Token: 0x06000163 RID: 355 RVA: 0x0000F8C8 File Offset: 0x0000DAC8
	private void Start()
	{
		this.anim = base.GetComponent<Animation>();
		base.GetComponent<AudioSource>();
		this.CreateCall();
		this.sitpos = base.transform.position;
		this.sitrot = base.transform.rotation;
	}

	// Token: 0x06000164 RID: 356 RVA: 0x0000F908 File Offset: 0x0000DB08
	public void CreateCall()
	{
		this.clip1 = null;
		this.clip2 = null;
		this.clip3 = null;
		this.clip4 = null;
		this.clip5 = null;
		this.clip6 = null;
		this.clip7 = null;
		int num = Random.Range(0, 7);
		if (num == 1)
		{
			this.clip1 = this.ring;
			this.clip2 = this.jimmyspeaking[Random.Range(0, 4)];
			this.clip3 = this.yeah[Random.Range(0, 3)];
			this.clip4 = this.okay[Random.Range(0, 2)];
			this.clip6 = this.hangup;
		}
		if (num == 2)
		{
			this.clip1 = this.ring;
			this.clip2 = this.jimmyspeaking[Random.Range(0, 4)];
			this.clip3 = this.what[Random.Range(0, 2)];
			this.clip4 = this.pencil[Random.Range(0, 2)];
			this.clip5 = this.okay[Random.Range(0, 2)];
			this.clip6 = this.hangup;
		}
		if (num == 3)
		{
			this.clip1 = this.ring;
			this.clip2 = this.jimmyspeaking[Random.Range(0, 4)];
			this.clip3 = this.what[Random.Range(0, 2)];
			this.clip4 = this.wedont;
			this.clip5 = this.yeah[Random.Range(0, 3)];
			this.clip6 = this.hangup;
		}
		if (num == 4)
		{
			this.clip1 = this.ring;
			this.clip2 = this.jimmyspeaking[Random.Range(0, 4)];
			this.clip3 = this.yeah[Random.Range(0, 3)];
			this.clip4 = this.carready;
			this.clip5 = this.okay[Random.Range(0, 2)];
			this.clip6 = this.hangup;
		}
		if (num == 5)
		{
			this.clip1 = this.ring;
			this.clip2 = this.jimmyspeaking[Random.Range(0, 4)];
			this.clip3 = this.homeloan[Random.Range(0, 2)];
			this.clip6 = this.hangup;
			int num2 = Random.Range(0, 9);
			if (num2 == 1)
			{
				this.clip4 = this.yeah[Random.Range(0, 3)];
				this.clip6 = this.hangup;
				this.clip7 = this.unbelievable;
			}
			if (num2 == 2 || num2 == 3 || num2 == 4)
			{
				this.clip7 = this.ass;
			}
		}
		if (num == 6)
		{
			this.clip1 = this.ring;
			this.clip2 = this.jimmyspeaking[Random.Range(0, 4)];
			this.clip3 = this.yeah[Random.Range(0, 3)];
			this.clip4 = this.haveitdone;
			this.clip5 = this.okay[Random.Range(0, 2)];
			this.clip6 = this.hangup;
		}
		base.StartCoroutine(this.PlayCall());
	}

	// Token: 0x06000165 RID: 357 RVA: 0x0000FBE1 File Offset: 0x0000DDE1
	private IEnumerator PlayCall()
	{
		if (!this.isWalking1 && !this.isWalking2 && !this.isWalking3 && !this.isWalking4 && !this.isWalking5 && !this.isWorking && !this.isTalking)
		{
			if (this.clip1 != null)
			{
				this.inCall = true;
				this.anim.Play("jj_answer");
				this.aSource.clip = this.clip1;
				this.aSource.Play();
				yield return new WaitForSeconds(1f);
				this.phone.parent = this.leftHand;
				yield return new WaitForSeconds(1f);
				this.aSource.clip = this.clip2;
				this.aSource.Play();
				yield return new WaitForSeconds(this.clip2.length);
				this.aSource.clip = this.clip3;
				this.aSource.Play();
				yield return new WaitForSeconds(this.clip3.length);
				if (this.clip4 != null)
				{
					this.aSource.clip = this.clip4;
					this.aSource.Play();
					yield return new WaitForSeconds(this.clip4.length);
				}
				if (this.clip5 != null)
				{
					this.aSource.clip = this.clip5;
					this.aSource.Play();
					yield return new WaitForSeconds(this.clip5.length);
				}
				if (this.clip6 != null)
				{
					this.anim.Play("jj_hangup");
					yield return new WaitForSeconds(1f);
					this.phone.parent = null;
					this.aSource.clip = this.clip6;
					this.aSource.Play();
					yield return new WaitForSeconds(1f);
					this.anim.Play("jj_phone");
				}
				if (this.clip7 != null)
				{
					this.aSource.clip = this.clip7;
					this.aSource.Play();
				}
			}
			this.inCall = false;
			int num = Random.Range(45, 60);
			yield return new WaitForSeconds((float)num);
			this.CreateCall();
		}
		else
		{
			this.inCall = false;
			int num2 = Random.Range(60, 120);
			yield return new WaitForSeconds((float)num2);
			this.CreateCall();
		}
		yield break;
	}

	// Token: 0x06000166 RID: 358 RVA: 0x0000FBF0 File Offset: 0x0000DDF0
	public void Inspect()
	{
		if (!this.inCall && !this.inspecting)
		{
			if (Vector3.Distance(base.transform.position, this.mytruckpos.position) > 25f)
			{
				AudioSource.PlayClipAtPoint(this.tooFar, this.audioLoc.transform.position, 1f);
				this.isBusy = true;
				base.StartCoroutine(this.Unbusy());
				return;
			}
			if (this.eventSystem.GetComponent<Currency>().money > 49f || this.fixing)
			{
				if (!this.myTruck.GetComponent<car>().usingV8 && !this.myTruck.GetComponent<car>().usingI6)
				{
					if (this.isWalking1 && this.isWalking2 && this.isWalking3 && this.isWalking4 && this.isWalking5 && this.isWorking)
					{
						return;
					}
					if (!this.fixing)
					{
						AudioSource.PlayClipAtPoint(this.allright, this.audioLoc.transform.position, 1f);
						this.i = 0;
						while (this.i < 36)
						{
							this.diag.listEntries[this.i].GetComponent<Text>().text = "    ";
							this.i++;
						}
						this.i = 0;
					}
					this.anim.Play("jj_walk7");
					this.isWalking1 = true;
					this.truckWinch.SetActive(false);
					this.myTruck.SetActive(false);
					this.myTruck.transform.position = this.truckpos.position;
					this.myTruck.transform.rotation = this.truckpos.rotation;
					this.drivDoor.transform.rotation = this.truckpos.rotation;
					this.passDoor.transform.rotation = this.truckpos.rotation;
					this.fob.transform.rotation = Quaternion.Euler(new Vector3(50f, 326f, 18f));
					this.inspectionPaper.SetActive(false);
					this.myTruck.SetActive(true);
					this.inspecting = true;
					if (!this.fixing)
					{
						this.eventSystem.GetComponent<Currency>().money -= 50f;
						this.inv.SubtractMoney(50f);
						return;
					}
				}
				else
				{
					this.isBusy = true;
					AudioSource.PlayClipAtPoint(this.noway, this.audioLoc.transform.position, 1f);
					this.dialogue = "No way. That engine is out of my league. But I can help you with your inline four.";
					this.interactor.Subtitle(this.dialogue, this.noway);
					base.StartCoroutine(this.Unbusy3());
				}
			}
		}
	}

	// Token: 0x06000167 RID: 359 RVA: 0x0000FECA File Offset: 0x0000E0CA
	private IEnumerator Unbusy()
	{
		yield return new WaitForSeconds(8f);
		this.isBusy = false;
		yield break;
	}

	// Token: 0x06000168 RID: 360 RVA: 0x0000FED9 File Offset: 0x0000E0D9
	private IEnumerator Unbusy3()
	{
		yield return new WaitForSeconds(3f);
		this.isBusy = false;
		yield break;
	}

	// Token: 0x06000169 RID: 361 RVA: 0x0000FEE8 File Offset: 0x0000E0E8
	private IEnumerator Unbusy4()
	{
		yield return new WaitForSeconds(24f);
		this.isBusy = false;
		this.isTalking = false;
		base.GetComponent<Animator>().enabled = true;
		yield break;
	}

	// Token: 0x0600016A RID: 362 RVA: 0x0000FEF8 File Offset: 0x0000E0F8
	public void Interact()
	{
		if (!this.inCall && !this.isBusy)
		{
			if (this.jobs == 0)
			{
				this.isTalking = true;
				this.isBusy = true;
				this.aSource.clip = this.jobClip[0];
				this.aSource.Play();
				this.anim.Play("jj_job1");
				this.jobs = 1;
				base.StartCoroutine(this.Unbusy4());
				this.vette.SetActive(true);
				this.mc.ActivateMission(70);
				base.GetComponent<Animator>().enabled = false;
				this.dialogue = "So I'm out at the lake, right? Figured I'd go fishing, ya know, blow off some steam. Then, out of the corner of my eye, I see my car rollin' down the hill like it's in the god damn Boston marathon. Car goes nose first into the lake like it's tryin' out for the swim team. You got a winch, right? I gotta tow this thing out before it turns into an artificial reef!";
				this.interactor.Subtitle(this.dialogue, this.jobClip[0]);
				base.StartCoroutine(this.Unbusy4());
				return;
			}
			if (this.jobs == 1 && !this.vwc.inWater)
			{
				this.jobs = 2;
				this.mc.CompleteMission(70);
				this.aSource.clip = this.jobClip[1];
				this.aSource.Play();
				this.dialogue = "Thank you. I gotta fix that parking brake. First thing tomorrow...I'm gonna call a mechanic, heh heh.";
				this.interactor.Subtitle(this.dialogue, this.jobClip[1]);
				this.isBusy = true;
				this.vette.SetActive(false);
				this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
				this.newRoll.GetComponent<PickUp>().thisDurability = 200f;
				base.StartCoroutine(this.Unbusy3());
				return;
			}
			this.isBusy = true;
			AudioSource.PlayClipAtPoint(this.choosePaper, this.audioLoc.transform.position, 1f);
			base.StartCoroutine(this.Unbusy3());
		}
	}

	// Token: 0x0600016B RID: 363 RVA: 0x000100DC File Offset: 0x0000E2DC
	private void Update()
	{
		base.transform.LookAt(this.phonepos);
		if (this.isWalking1)
		{
			base.transform.LookAt(this.walkto1);
			base.transform.position += base.transform.forward * this.moveSpeed * Time.deltaTime;
			if (Vector3.Distance(base.transform.position, this.walkto1.position) < 0.2f)
			{
				this.isWalking1 = false;
				this.isWalking2 = true;
				this.isWalking3 = false;
				base.transform.LookAt(this.walkto2);
			}
		}
		if (this.isWalking2)
		{
			base.transform.LookAt(this.walkto2);
			base.transform.position += base.transform.forward * this.moveSpeed * Time.deltaTime;
			if (Vector3.Distance(base.transform.position, this.walkto2.position) < 0.2f)
			{
				this.isWalking1 = false;
				this.isWalking2 = false;
				this.isWalking3 = true;
				base.transform.LookAt(this.walkto3);
			}
		}
		if (this.isWalking3)
		{
			base.transform.LookAt(this.walkto3);
			base.transform.position += base.transform.forward * this.moveSpeed * Time.deltaTime;
			if (Vector3.Distance(base.transform.position, this.walkto3.position) < 0.2f)
			{
				this.isWalking1 = false;
				this.isWalking2 = false;
				this.isWalking3 = false;
				this.isWorking = true;
				if (!this.truckHood.GetComponent<InteractiveObject>().isOpen)
				{
					this.truckHood.GetComponent<InteractiveObject>().PerformAction();
				}
				this.skip = false;
				this.numIssues = 0;
				base.StartCoroutine(this.Diagnostics());
				this.anim.Play("jj_inspect");
			}
		}
		if (this.isWorking)
		{
			base.transform.LookAt(this.walkto3);
		}
		if (this.isWalking4)
		{
			base.transform.LookAt(this.walkto1);
			base.transform.position += base.transform.forward * this.moveSpeed * Time.deltaTime;
			if (Vector3.Distance(base.transform.position, this.walkto1.position) < 0.2f)
			{
				this.isWalking4 = false;
				this.isWalking5 = true;
				base.transform.LookAt(this.walkto4);
			}
		}
		if (this.isWalking5)
		{
			base.transform.LookAt(this.walkto4);
			base.transform.position += base.transform.forward * this.moveSpeed * Time.deltaTime;
			if (Vector3.Distance(base.transform.position, this.walkto4.position) < 0.2f)
			{
				this.truckWinch.SetActive(true);
				base.transform.LookAt(this.phonepos);
				this.isWalking5 = false;
				this.anim.Play("jj_phone");
				if (!this.fixing)
				{
					AudioSource.PlayClipAtPoint(this.aCopy, this.audioLoc.transform.position, 1f);
					base.StartCoroutine(this.Print());
				}
				base.transform.position = this.sitpos;
				base.transform.rotation = this.sitrot;
			}
		}
	}

	// Token: 0x0600016C RID: 364 RVA: 0x000104BB File Offset: 0x0000E6BB
	private IEnumerator Print()
	{
		yield return new WaitForSeconds(6f);
		this.inspectionPaper.SetActive(true);
		yield break;
	}

	// Token: 0x0600016D RID: 365 RVA: 0x000104CA File Offset: 0x0000E6CA
	private IEnumerator Diagnostics()
	{
		this.inspecting = true;
		this.i = 0;
		while (this.i < 36)
		{
			this.priceArr[this.i] = 0;
			this.i++;
		}
		this.i = 0;
		if (!this.fixing)
		{
			AudioSource.PlayClipAtPoint(this.hmm1, this.audioLoc.transform.position, 1f);
			yield return new WaitForSeconds(this.hmm1.length);
			AudioSource.PlayClipAtPoint(this.lookslike, this.audioLoc.transform.position, 1f);
			yield return new WaitForSeconds(this.lookslike.length);
		}
		if (this.fixing)
		{
			yield return new WaitForSeconds(5f);
		}
		this.cost = 0f;
		if (!this.eng.block.GetComponent<Renderer>().enabled)
		{
			this.skip = true;
		}
		this.list1 = "";
		if (!this.skip)
		{
			if (!this.eng.alternator.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_alternator;
				this.issue = this.gone;
				this.list1 = "[x] Alternator: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 150;
				this.cost += 150f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.alternator.GetComponent<Renderer>().enabled = true;
					this.eng.alternator_cnd = 100f;
					this.eng.alternator.GetComponent<durability>().health = 100f;
					this.eng.alternator.GetComponent<durability>().Start();
					this.eng.alternator.GetComponent<BoxCollider>().enabled = true;
					BoxCollider[] components = this.eng.alternator.GetComponents<BoxCollider>();
					components[0].enabled = true;
					components[1].enabled = true;
					foreach (object obj in this.eng.alternator.transform)
					{
						Transform transform = (Transform)obj;
						if (transform.gameObject.name == "bolt")
						{
							transform.gameObject.GetComponent<Renderer>().enabled = true;
							transform.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			else if (this.eng.alternator_cnd < 6f)
			{
				this.part = this.part_alternator;
				this.issue = this.garbage;
				this.list1 = "[x] Alternator: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 150;
				this.cost += 150f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.alternator_cnd = 100f;
					this.eng.alternator.GetComponent<durability>().health = 100f;
					this.eng.alternator.GetComponent<durability>().Start();
					foreach (object obj2 in this.eng.alternator.transform)
					{
						Transform transform2 = (Transform)obj2;
						if (transform2.gameObject.name == "bolt")
						{
							transform2.gameObject.GetComponent<Renderer>().enabled = true;
							transform2.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform2.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			if (!this.eng.battery.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_battery;
				this.issue = this.gone;
				this.list1 = "[x] Battery: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 100;
				this.cost += 100f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.battery.GetComponent<Renderer>().enabled = true;
					this.eng.battery_cnd = 100f;
					this.eng.battery.GetComponent<durability>().health = 100f;
					BoxCollider[] components2 = this.eng.battery.GetComponents<BoxCollider>();
					components2[0].enabled = true;
					components2[1].enabled = true;
				}
				this.i++;
			}
			else if (this.eng.battery_cnd < 6f)
			{
				this.part = this.part_battery;
				this.issue = this.garbage;
				this.list1 = "[x] Battery: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 100;
				this.cost += 100f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.battery_cnd = 100f;
					this.eng.battery.GetComponent<durability>().health = 100f;
				}
				this.i++;
			}
			if (!this.eng.camGear.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_camgear;
				this.issue = this.gone;
				this.list1 = "[x] Cam Gear: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 90;
				this.cost += 90f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.camGear.GetComponent<Renderer>().enabled = true;
					this.eng.camGear_cnd = 100f;
					this.eng.camGear.GetComponent<durability>().health = 100f;
					BoxCollider[] components3 = this.eng.camGear.GetComponents<BoxCollider>();
					components3[0].enabled = true;
					components3[1].enabled = true;
					this.eng.camGear.GetComponent<durability>().Start();
					foreach (object obj3 in this.eng.camGear.transform)
					{
						Transform transform3 = (Transform)obj3;
						if (transform3.gameObject.name == "bolt")
						{
							transform3.gameObject.GetComponent<Renderer>().enabled = true;
							transform3.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform3.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			else if (this.eng.camGear_cnd < 6f)
			{
				this.part = this.part_camgear;
				this.issue = this.garbage;
				this.list1 = "[x] Cam Gear: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 90;
				this.cost += 90f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.camGear_cnd = 100f;
					this.eng.camGear.GetComponent<durability>().health = 100f;
					this.eng.camGear.GetComponent<durability>().Start();
					foreach (object obj4 in this.eng.camGear.transform)
					{
						Transform transform4 = (Transform)obj4;
						if (transform4.gameObject.name == "bolt")
						{
							transform4.gameObject.GetComponent<Renderer>().enabled = true;
							transform4.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform4.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			if (!this.eng.camshaft.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_camshaft;
				this.issue = this.gone;
				this.list1 = "[x] Camshaft: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 170;
				this.cost += 170f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.camshaft.GetComponent<Renderer>().enabled = true;
					this.eng.camshaft_cnd = 100f;
					this.eng.camshaft.GetComponent<durability>().health = 100f;
					BoxCollider[] components4 = this.eng.camshaft.GetComponents<BoxCollider>();
					components4[0].enabled = true;
					components4[1].enabled = true;
					this.eng.camshaft.GetComponent<durability>().Start();
				}
				this.i++;
			}
			else if (this.eng.camshaft_cnd < 6f)
			{
				this.part = this.part_camshaft;
				this.issue = this.garbage;
				this.list1 = "[x] Camshaft: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 170;
				this.cost += 170f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.camshaft_cnd = 100f;
					this.eng.camshaft.GetComponent<durability>().health = 100f;
					this.eng.camshaft.GetComponent<durability>().Start();
				}
				this.i++;
			}
			if (!this.eng.carb.GetComponent<Renderer>().enabled && !this.eng.intakeManEFI.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_carb;
				this.issue = this.gone;
				this.list1 = "[x] Carburetor: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 200;
				this.cost += 200f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.carb.GetComponent<Renderer>().enabled = true;
					this.eng.carb_cnd = 100f;
					this.eng.carb.GetComponent<durability>().health = 100f;
					BoxCollider[] components5 = this.eng.carb.GetComponents<BoxCollider>();
					components5[0].enabled = true;
					components5[1].enabled = true;
					this.eng.carb.GetComponent<durability>().Start();
					foreach (object obj5 in this.eng.carb.transform)
					{
						Transform transform5 = (Transform)obj5;
						if (transform5.gameObject.name == "bolt")
						{
							transform5.gameObject.GetComponent<Renderer>().enabled = true;
							transform5.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform5.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			else if (this.eng.carb_cnd < 6f && !this.eng.intakeManEFI.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_carb;
				this.issue = this.garbage;
				this.list1 = "[x] Carburetor: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 200;
				this.cost += 200f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.carb_cnd = 100f;
					this.eng.carb.GetComponent<durability>().health = 100f;
					this.eng.carb.GetComponent<durability>().Start();
					foreach (object obj6 in this.eng.carb.transform)
					{
						Transform transform6 = (Transform)obj6;
						if (transform6.gameObject.name == "bolt")
						{
							transform6.gameObject.GetComponent<Renderer>().enabled = true;
							transform6.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform6.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			if (!this.eng.clutch.GetComponent<Renderer>().enabled || !this.eng.clutchDia.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_clutch;
				this.issue = this.gone;
				this.list1 = "[x] Clutch: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 400;
				this.cost += 400f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.clutch.GetComponent<Renderer>().enabled = true;
					this.eng.clutch_cnd = 100f;
					this.eng.clutch.GetComponent<durability>().health = 100f;
					BoxCollider[] components6 = this.eng.clutch.GetComponents<BoxCollider>();
					components6[0].enabled = true;
					components6[1].enabled = true;
					this.eng.clutch.GetComponent<durability>().Start();
				}
				this.i++;
			}
			else if (this.eng.clutch_cnd < 6f || this.eng.clutchDia_cnd < 6f)
			{
				this.part = this.part_clutch;
				this.issue = this.garbage;
				this.list1 = "[x] Clutch: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 400;
				this.cost += 400f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.clutch_cnd = 100f;
					this.eng.clutch.GetComponent<durability>().health = 100f;
					this.eng.clutch.GetComponent<durability>().Start();
				}
				this.i++;
			}
			if (this.eng.newCoolantLevel < 60f)
			{
				this.part = this.part_coolant;
				this.issue = this.gone;
				this.list1 = "[x] Coolant: Low/Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 30;
				this.cost += 30f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.newCoolantLevel = 100f;
				}
				this.i++;
			}
			if (!this.eng.crank.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_crank;
				this.issue = this.gone;
				this.list1 = "[x] Crankshaft: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 600;
				this.cost += 600f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.crank.GetComponent<Renderer>().enabled = true;
					this.eng.crank_cnd = 100f;
					this.eng.crank.GetComponent<durability>().health = 100f;
					BoxCollider[] components7 = this.eng.crank.GetComponents<BoxCollider>();
					components7[0].enabled = true;
					components7[1].enabled = true;
					this.eng.crank.GetComponent<durability>().Start();
				}
				this.i++;
			}
			else if (this.eng.crank_cnd < 6f)
			{
				this.part = this.part_crank;
				this.issue = this.garbage;
				this.list1 = "[x] Crankshaft: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 600;
				this.cost += 600f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.crank_cnd = 100f;
					this.eng.crank.GetComponent<durability>().health = 100f;
					this.eng.crank.GetComponent<durability>().Start();
				}
				this.i++;
			}
			if (!this.eng.distributor.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_distributor;
				this.issue = this.gone;
				this.list1 = "[x] Distributor: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 120;
				this.cost += 120f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.distributor.GetComponent<Renderer>().enabled = true;
					this.eng.distributor_cnd = 100f;
					this.eng.distributor.GetComponent<durability>().health = 100f;
					BoxCollider[] components8 = this.eng.distributor.GetComponents<BoxCollider>();
					components8[0].enabled = true;
					components8[1].enabled = true;
				}
				this.i++;
			}
			else if (this.eng.distributor_cnd < 6f)
			{
				this.part = this.part_distributor;
				this.issue = this.garbage;
				this.list1 = "[x] Distributor: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 120;
				this.cost += 120f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.distributor_cnd = 100f;
					this.eng.distributor.GetComponent<durability>().health = 100f;
				}
				this.i++;
			}
			if (!this.eng.exhaustMan.GetComponent<Renderer>().enabled && !this.eng.exhaustMan2.GetComponent<Renderer>().enabled && !this.eng.exhaustMan3.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_exhaust;
				this.issue = this.gone;
				this.list1 = "[x] Exhaust Manifold: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 190;
				this.cost += 190f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.exhaustMan.GetComponent<Renderer>().enabled = true;
					this.eng.exhaustMan_cnd = 100f;
					this.eng.exhaustMan.GetComponent<durability>().health = 100f;
					BoxCollider[] components9 = this.eng.exhaustMan.GetComponents<BoxCollider>();
					components9[0].enabled = true;
					components9[1].enabled = true;
					this.eng.exhaustMan.GetComponent<durability>().Start();
					foreach (object obj7 in this.eng.exhaustMan.transform)
					{
						Transform transform7 = (Transform)obj7;
						if (transform7.gameObject.name == "bolt")
						{
							transform7.gameObject.GetComponent<Renderer>().enabled = true;
							transform7.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform7.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			else if (this.eng.exhaustMan_cnd < 6f && !this.eng.exhaustMan2.GetComponent<Renderer>().enabled && !this.eng.exhaustMan3.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_exhaust;
				this.issue = this.garbage;
				this.list1 = "[x] Exhaust Manifold: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 190;
				this.cost += 190f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.exhaustMan_cnd = 100f;
					this.eng.exhaustMan.GetComponent<durability>().health = 100f;
					this.eng.exhaustMan.GetComponent<durability>().Start();
					foreach (object obj8 in this.eng.exhaustMan.transform)
					{
						Transform transform8 = (Transform)obj8;
						if (transform8.gameObject.name == "bolt")
						{
							transform8.gameObject.GetComponent<Renderer>().enabled = true;
							transform8.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform8.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			if (!this.eng.fan.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_fan;
				this.issue = this.gone;
				this.list1 = "[x] Fan: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 90;
				this.cost += 90f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.fan.GetComponent<Renderer>().enabled = true;
					this.eng.fan_cnd = 100f;
					this.eng.fan.GetComponent<durability>().health = 100f;
					BoxCollider[] components10 = this.eng.fan.GetComponents<BoxCollider>();
					components10[0].enabled = true;
					components10[1].enabled = true;
				}
				this.i++;
			}
			else if (this.eng.fan_cnd < 6f)
			{
				this.part = this.part_fan;
				this.issue = this.garbage;
				this.list1 = "[x] Fan: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 90;
				this.cost += 90f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.fan_cnd = 100f;
					this.eng.fan.GetComponent<durability>().health = 100f;
				}
				this.i++;
			}
			if (!this.eng.fanBelt.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_fanbelt;
				this.issue = this.gone;
				this.list1 = "[x] Fan Belt: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 70;
				this.cost += 70f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.fanBelt.GetComponent<Renderer>().enabled = true;
					this.eng.fanBelt_cnd = 100f;
					this.eng.fanBelt.GetComponent<durability>().health = 100f;
					BoxCollider[] components11 = this.eng.fanBelt.GetComponents<BoxCollider>();
					components11[0].enabled = true;
					components11[1].enabled = true;
				}
				this.i++;
			}
			else if (this.eng.fanBelt_cnd < 6f)
			{
				this.part = this.part_fanbelt;
				this.issue = this.garbage;
				this.list1 = "[x] Fan Belt: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 70;
				this.cost += 70f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.fanBelt_cnd = 100f;
					this.eng.fanBelt.GetComponent<durability>().health = 100f;
				}
				this.i++;
			}
			if (!this.eng.headgasket.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_headgasket;
				this.issue = this.gone;
				this.list1 = "[x] Head Gasket: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 300;
				this.cost += 300f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.headgasket.GetComponent<Renderer>().enabled = true;
					this.eng.headgasket_cnd = 100f;
					this.eng.headgasket.GetComponent<durability>().health = 100f;
					BoxCollider[] components12 = this.eng.headgasket.GetComponents<BoxCollider>();
					components12[0].enabled = true;
					components12[1].enabled = true;
					this.eng.headgasket.GetComponent<durability>().Start();
				}
				this.i++;
			}
			else if (this.eng.headgasket_cnd < 6f)
			{
				this.part = this.part_headgasket;
				this.issue = this.garbage;
				this.list1 = "[x] Head Gasket: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 300;
				this.cost += 300f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.headgasket_cnd = 100f;
					this.eng.headgasket.GetComponent<durability>().health = 100f;
					this.eng.headgasket.GetComponent<durability>().Start();
				}
				this.i++;
			}
			if (!this.eng.intakeMan.GetComponent<Renderer>().enabled && !this.eng.intakeManEFI.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_intake;
				this.issue = this.gone;
				this.list1 = "[x] Intake Manifold: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 250;
				this.cost += 250f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.intakeMan.GetComponent<Renderer>().enabled = true;
					this.eng.intakeMan_cnd = 100f;
					this.eng.intakeMan.GetComponent<durability>().health = 100f;
					BoxCollider[] components13 = this.eng.intakeMan.GetComponents<BoxCollider>();
					components13[0].enabled = true;
					components13[1].enabled = true;
					this.eng.intakeMan.GetComponent<durability>().Start();
					foreach (object obj9 in this.eng.intakeMan.transform)
					{
						Transform transform9 = (Transform)obj9;
						if (transform9.gameObject.name == "bolt")
						{
							transform9.gameObject.GetComponent<Renderer>().enabled = true;
							transform9.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform9.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			else if (this.eng.intakeMan_cnd < 6f && !this.eng.intakeManEFI.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_intake;
				this.issue = this.garbage;
				this.list1 = "[x] Intake Manifold: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 250;
				this.cost += 250f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.intakeMan_cnd = 100f;
					this.eng.intakeMan.GetComponent<durability>().health = 100f;
					this.eng.intakeMan.GetComponent<durability>().Start();
					foreach (object obj10 in this.eng.intakeMan.transform)
					{
						Transform transform10 = (Transform)obj10;
						if (transform10.gameObject.name == "bolt")
						{
							transform10.gameObject.GetComponent<Renderer>().enabled = true;
							transform10.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform10.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			if (this.eng.intakeManEFI_cnd < 6f && this.eng.intakeManEFI.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_intake;
				this.issue = this.garbage;
				this.list1 = "[x] EFI Manifold: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 250;
				this.cost += 250f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.intakeManEFI_cnd = 100f;
					this.eng.intakeManEFI.GetComponent<durability>().health = 100f;
					this.eng.intakeManEFI.GetComponent<durability>().Start();
					foreach (object obj11 in this.eng.intakeManEFI.transform)
					{
						Transform transform11 = (Transform)obj11;
						if (transform11.gameObject.name == "bolt")
						{
							transform11.gameObject.GetComponent<Renderer>().enabled = true;
							transform11.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform11.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			if (this.eng.intakeManEFI.GetComponent<Renderer>().enabled && !this.eng.intakeEFIThrottle.GetComponent<Renderer>().enabled)
			{
				this.issue = this.gone;
				this.list1 = "[x] EFI Throttle Body: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 250;
				this.cost += 250f;
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.intakeEFIThrottle.GetComponent<Renderer>().enabled = true;
					this.eng.intakeEFIThrottle_cnd = 100f;
					this.eng.intakeEFIThrottle.GetComponent<durability>().health = 100f;
					BoxCollider[] components14 = this.eng.intakeEFIThrottle.GetComponents<BoxCollider>();
					components14[0].enabled = true;
					components14[1].enabled = true;
					this.eng.intakeEFIThrottle.GetComponent<durability>().Start();
					foreach (object obj12 in this.eng.intakeEFIThrottle.transform)
					{
						Transform transform12 = (Transform)obj12;
						if (transform12.gameObject.name == "bolt")
						{
							transform12.gameObject.GetComponent<Renderer>().enabled = true;
							transform12.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform12.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			else if (this.eng.intakeEFIThrottle_cnd < 6f && this.eng.intakeManEFI.GetComponent<Renderer>().enabled)
			{
				this.issue = this.garbage;
				this.list1 = "[x] Throttle Body: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 250;
				this.cost += 250f;
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.intakeEFIThrottle_cnd = 100f;
					this.eng.intakeEFIThrottle.GetComponent<durability>().health = 100f;
					this.eng.intakeEFIThrottle.GetComponent<durability>().Start();
					foreach (object obj13 in this.eng.intakeManEFI.transform)
					{
						Transform transform13 = (Transform)obj13;
						if (transform13.gameObject.name == "bolt")
						{
							transform13.gameObject.GetComponent<Renderer>().enabled = true;
							transform13.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform13.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			if (this.eng.turbo.GetComponent<Renderer>().enabled && !this.eng.turboPipe.GetComponent<Renderer>().enabled)
			{
				this.issue = this.gone;
				this.list1 = "[x] Turbo Pipe: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 250;
				this.cost += 250f;
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.turboPipe.GetComponent<Renderer>().enabled = true;
					this.eng.turboPipe_cnd = 100f;
					this.eng.turboPipe.GetComponent<durability>().health = 100f;
					BoxCollider[] components15 = this.eng.turboPipe.GetComponents<BoxCollider>();
					components15[0].enabled = true;
					components15[1].enabled = true;
					foreach (object obj14 in this.eng.turboPipe.transform)
					{
						Transform transform14 = (Transform)obj14;
						if (transform14.gameObject.name == "bolt")
						{
							transform14.gameObject.GetComponent<Renderer>().enabled = true;
							transform14.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform14.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			if (this.eng.exhaustMan3.GetComponent<Renderer>().enabled && !this.eng.turbo.GetComponent<Renderer>().enabled)
			{
				this.issue = this.gone;
				this.list1 = "[x] Turbo: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 700;
				this.cost += 700f;
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.turbo.GetComponent<Renderer>().enabled = true;
					this.eng.turbo_cnd = 100f;
					this.eng.turbo.GetComponent<durability>().health = 100f;
					BoxCollider[] components16 = this.eng.turbo.GetComponents<BoxCollider>();
					components16[0].enabled = true;
					components16[1].enabled = true;
					this.eng.turbo.GetComponent<durability>().Start();
					foreach (object obj15 in this.eng.turbo.transform)
					{
						Transform transform15 = (Transform)obj15;
						if (transform15.gameObject.name == "bolt")
						{
							transform15.gameObject.GetComponent<Renderer>().enabled = true;
							transform15.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform15.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			else if (this.eng.turbo_cnd < 6f && this.eng.turbo.GetComponent<Renderer>().enabled)
			{
				this.issue = this.garbage;
				this.list1 = "[x] Turbo: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 300;
				this.cost += 300f;
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.turbo_cnd = 100f;
					this.eng.turbo.GetComponent<durability>().health = 100f;
					this.eng.turbo.GetComponent<durability>().Start();
					foreach (object obj16 in this.eng.turbo.transform)
					{
						Transform transform16 = (Transform)obj16;
						if (transform16.gameObject.name == "bolt")
						{
							transform16.gameObject.GetComponent<Renderer>().enabled = true;
							transform16.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform16.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			if (this.eng.turbo.GetComponent<Renderer>().enabled && !this.eng.airFilterTurbo.GetComponent<Renderer>().enabled && !this.eng.compound1.GetComponent<Renderer>().enabled)
			{
				this.issue = this.gone;
				this.list1 = "[x] Air Filter: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 80;
				this.cost += 80f;
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.airFilterTurbo.GetComponent<Renderer>().enabled = true;
					this.eng.airFilterTurbo_cnd = 100f;
					this.eng.airFilterTurbo.GetComponent<durability>().health = 100f;
					BoxCollider[] components17 = this.eng.airFilterTurbo.GetComponents<BoxCollider>();
					components17[0].enabled = true;
					components17[1].enabled = true;
					foreach (object obj17 in this.eng.airFilterTurbo.transform)
					{
						Transform transform17 = (Transform)obj17;
						if (transform17.gameObject.name == "bolt")
						{
							transform17.gameObject.GetComponent<Renderer>().enabled = true;
							transform17.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform17.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			else if (this.eng.airFilterTurbo_cnd < 6f && this.eng.airFilterTurbo.GetComponent<Renderer>().enabled)
			{
				this.issue = this.garbage;
				this.list1 = "[x] Air Filter: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 100;
				this.cost += 100f;
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.airFilterTurbo_cnd = 100f;
					this.eng.airFilterTurbo.GetComponent<durability>().health = 100f;
				}
				this.i++;
			}
			if (this.eng.newOilLevel < 60f)
			{
				this.part = this.part_oil;
				this.issue = this.gone;
				this.list1 = "[x] Oil: Low/Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 40;
				this.cost += 40f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.newOilLevel = 100f;
				}
				this.i++;
			}
			if (!this.eng.oilFilter.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_oilfilter;
				this.issue = this.gone;
				this.list1 = "[x] Oil Filter: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 25;
				this.cost += 25f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.oilFilter.GetComponent<Renderer>().enabled = true;
					this.eng.oilFilter_cnd = 100f;
					this.eng.oilFilter.GetComponent<durability>().health = 100f;
					BoxCollider[] components18 = this.eng.oilFilter.GetComponents<BoxCollider>();
					components18[0].enabled = true;
					components18[1].enabled = true;
				}
				this.i++;
			}
			else if (this.eng.oilFilter_cnd < 6f)
			{
				this.part = this.part_oilfilter;
				this.issue = this.garbage;
				this.list1 = "[x] Oil Filter: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 25;
				this.cost += 25f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.oilFilter_cnd = 100f;
					this.eng.oilFilter.GetComponent<durability>().health = 100f;
				}
				this.i++;
			}
			if (!this.eng.piston1.GetComponent<Renderer>().enabled || !this.eng.piston2.GetComponent<Renderer>().enabled || !this.eng.piston3.GetComponent<Renderer>().enabled || !this.eng.piston4.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_pistons;
				this.issue = this.gone;
				this.list1 = "[x] Pistons: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 200;
				this.i++;
				if (!this.eng.piston1.GetComponent<Renderer>().enabled)
				{
					this.cost += 200f;
					if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
					{
						this.eng.piston1.GetComponent<Renderer>().enabled = true;
						this.eng.piston1_cnd = 100f;
						this.eng.piston1.GetComponent<durability>().health = 100f;
						BoxCollider[] components19 = this.eng.piston1.GetComponents<BoxCollider>();
						components19[0].enabled = true;
						components19[1].enabled = true;
						this.eng.piston1.GetComponent<durability>().Start();
					}
				}
				if (!this.eng.piston2.GetComponent<Renderer>().enabled)
				{
					this.cost += 200f;
					if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
					{
						this.eng.piston2.GetComponent<Renderer>().enabled = true;
						this.eng.piston2_cnd = 100f;
						this.eng.piston2.GetComponent<durability>().health = 100f;
						BoxCollider[] components20 = this.eng.piston2.GetComponents<BoxCollider>();
						components20[0].enabled = true;
						components20[1].enabled = true;
						this.eng.piston2.GetComponent<durability>().Start();
					}
				}
				if (!this.eng.piston3.GetComponent<Renderer>().enabled)
				{
					this.cost += 200f;
					if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
					{
						this.eng.piston3.GetComponent<Renderer>().enabled = true;
						this.eng.piston3_cnd = 100f;
						this.eng.piston3.GetComponent<durability>().health = 100f;
						BoxCollider[] components21 = this.eng.piston3.GetComponents<BoxCollider>();
						components21[0].enabled = true;
						components21[1].enabled = true;
						this.eng.piston3.GetComponent<durability>().Start();
					}
				}
				if (!this.eng.piston4.GetComponent<Renderer>().enabled)
				{
					this.cost += 200f;
					if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
					{
						this.eng.piston4.GetComponent<Renderer>().enabled = true;
						this.eng.piston4_cnd = 100f;
						this.eng.piston4.GetComponent<durability>().health = 100f;
						BoxCollider[] components22 = this.eng.piston4.GetComponents<BoxCollider>();
						components22[0].enabled = true;
						components22[1].enabled = true;
						this.eng.piston4.GetComponent<durability>().Start();
					}
				}
				yield return base.StartCoroutine(this.SpeakPart());
			}
			else if (this.eng.piston1_cnd < 6f || this.eng.piston2_cnd < 6f || this.eng.piston3_cnd < 6f || this.eng.piston4_cnd < 6f)
			{
				this.part = this.part_pistons;
				this.issue = this.garbage;
				this.list1 = "[x] Pistons: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				if (this.eng.piston1_cnd < 6f)
				{
					this.cost += 200f;
					this.priceArr[this.i] = this.priceArr[this.i] + 200;
					if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
					{
						this.eng.piston1_cnd = 100f;
						this.eng.piston1.GetComponent<durability>().health = 100f;
						this.eng.piston1.GetComponent<durability>().Start();
					}
				}
				if (this.eng.piston2_cnd < 6f)
				{
					this.cost += 200f;
					this.priceArr[this.i] = this.priceArr[this.i] + 200;
					if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
					{
						this.eng.piston2_cnd = 100f;
						this.eng.piston2.GetComponent<durability>().health = 100f;
						this.eng.piston2.GetComponent<durability>().Start();
					}
				}
				if (this.eng.piston3_cnd < 6f)
				{
					this.cost += 200f;
					this.priceArr[this.i] = this.priceArr[this.i] + 200;
					if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
					{
						this.eng.piston3_cnd = 100f;
						this.eng.piston3.GetComponent<durability>().health = 100f;
						this.eng.piston3.GetComponent<durability>().Start();
					}
				}
				if (this.eng.piston4_cnd < 6f)
				{
					this.cost += 200f;
					this.priceArr[this.i] = this.priceArr[this.i] + 200;
					if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
					{
						this.eng.piston4_cnd = 100f;
						this.eng.piston4.GetComponent<durability>().health = 100f;
						this.eng.piston4.GetComponent<durability>().Start();
					}
				}
				this.i++;
				yield return base.StartCoroutine(this.SpeakPart());
			}
			if (!this.eng.valveAssembly.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_rocker;
				this.issue = this.gone;
				this.list1 = "[x] Valve Assembly: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 300;
				this.cost += 300f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.valveAssembly.GetComponent<Renderer>().enabled = true;
					this.eng.valveAssembly_cnd = 100f;
					this.eng.valveAssembly.GetComponent<durability>().health = 100f;
					BoxCollider[] components23 = this.eng.valveAssembly.GetComponents<BoxCollider>();
					components23[0].enabled = true;
					components23[1].enabled = true;
					this.eng.valveAssembly.GetComponent<durability>().Start();
					foreach (object obj18 in this.eng.valveAssembly.transform)
					{
						Transform transform18 = (Transform)obj18;
						if (transform18.gameObject.name == "bolt")
						{
							transform18.gameObject.GetComponent<Renderer>().enabled = true;
							transform18.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform18.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			else if (this.eng.valveAssembly_cnd < 6f)
			{
				this.part = this.part_rocker;
				this.issue = this.garbage;
				this.list1 = "[x] Valve Assembly: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 300;
				this.cost += 300f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.valveAssembly_cnd = 100f;
					this.eng.valveAssembly.GetComponent<durability>().health = 100f;
					this.eng.valveAssembly.GetComponent<durability>().Start();
					foreach (object obj19 in this.eng.valveAssembly.transform)
					{
						Transform transform19 = (Transform)obj19;
						if (transform19.gameObject.name == "bolt")
						{
							transform19.gameObject.GetComponent<Renderer>().enabled = true;
							transform19.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform19.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			if (!this.eng.timingGear16.GetComponent<Renderer>().enabled || !this.eng.timingGear16r.GetComponent<Renderer>().enabled || !this.eng.timingGear32.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_timing;
				this.issue = this.gone;
				this.list1 = "[x] Timing Gears: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				if (!this.eng.timingGear16.GetComponent<Renderer>().enabled)
				{
					this.cost += 70f;
					this.priceArr[this.i] = this.priceArr[this.i] + 70;
					if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
					{
						this.eng.timingGear16.GetComponent<Renderer>().enabled = true;
						this.eng.timingGear16_cnd = 100f;
						this.eng.timingGear16.GetComponent<durability>().health = 100f;
						BoxCollider[] components24 = this.eng.timingGear16.GetComponents<BoxCollider>();
						components24[0].enabled = true;
						components24[1].enabled = true;
						this.eng.timingGear16.GetComponent<durability>().Start();
						foreach (object obj20 in this.eng.timingGear16.transform)
						{
							Transform transform20 = (Transform)obj20;
							if (transform20.gameObject.name == "bolt")
							{
								transform20.gameObject.GetComponent<Renderer>().enabled = true;
								transform20.gameObject.GetComponent<BoxCollider>().enabled = true;
								transform20.gameObject.GetComponent<BoltScript>().boltturns = 10;
							}
						}
					}
				}
				if (!this.eng.timingGear16r.GetComponent<Renderer>().enabled)
				{
					this.cost += 70f;
					this.priceArr[this.i] = this.priceArr[this.i] + 70;
					if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
					{
						this.eng.timingGear16r.GetComponent<Renderer>().enabled = true;
						this.eng.timingGear16r_cnd = 100f;
						this.eng.timingGear16r.GetComponent<durability>().health = 100f;
						BoxCollider[] components25 = this.eng.timingGear16r.GetComponents<BoxCollider>();
						components25[0].enabled = true;
						components25[1].enabled = true;
						this.eng.timingGear16r.GetComponent<durability>().Start();
						foreach (object obj21 in this.eng.timingGear16r.transform)
						{
							Transform transform21 = (Transform)obj21;
							if (transform21.gameObject.name == "bolt")
							{
								transform21.gameObject.GetComponent<Renderer>().enabled = true;
								transform21.gameObject.GetComponent<BoxCollider>().enabled = true;
								transform21.gameObject.GetComponent<BoltScript>().boltturns = 10;
							}
						}
					}
				}
				if (!this.eng.timingGear32.GetComponent<Renderer>().enabled)
				{
					this.cost += 70f;
					this.priceArr[this.i] = this.priceArr[this.i] + 70;
					if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
					{
						this.eng.timingGear32.GetComponent<Renderer>().enabled = true;
						this.eng.timingGear32_cnd = 100f;
						this.eng.timingGear32.GetComponent<durability>().health = 100f;
						BoxCollider[] components26 = this.eng.timingGear32.GetComponents<BoxCollider>();
						components26[0].enabled = true;
						components26[1].enabled = true;
						this.eng.timingGear32.GetComponent<durability>().Start();
						foreach (object obj22 in this.eng.timingGear32.transform)
						{
							Transform transform22 = (Transform)obj22;
							if (transform22.gameObject.name == "bolt")
							{
								transform22.gameObject.GetComponent<Renderer>().enabled = true;
								transform22.gameObject.GetComponent<BoxCollider>().enabled = true;
								transform22.gameObject.GetComponent<BoltScript>().boltturns = 10;
							}
						}
					}
				}
				this.i++;
				yield return base.StartCoroutine(this.SpeakPart());
			}
			else if (this.eng.timingGear16_cnd < 6f || this.eng.timingGear16r_cnd < 6f || this.eng.timingGear32_cnd < 6f)
			{
				this.part = this.part_timing;
				this.issue = this.garbage;
				this.list1 = "[x] Timing Gears: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				if (this.eng.timingGear16_cnd < 6f)
				{
					this.cost += 70f;
					if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
					{
						this.eng.timingGear16_cnd = 100f;
						this.eng.timingGear16.GetComponent<durability>().health = 100f;
						this.eng.timingGear16.GetComponent<durability>().Start();
						foreach (object obj23 in this.eng.timingGear16.transform)
						{
							Transform transform23 = (Transform)obj23;
							if (transform23.gameObject.name == "bolt")
							{
								transform23.gameObject.GetComponent<Renderer>().enabled = true;
								transform23.gameObject.GetComponent<BoxCollider>().enabled = true;
								transform23.gameObject.GetComponent<BoltScript>().boltturns = 10;
							}
						}
					}
				}
				if (this.eng.timingGear16r_cnd < 6f)
				{
					this.cost += 70f;
					if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
					{
						this.eng.timingGear16r_cnd = 100f;
						this.eng.timingGear16r.GetComponent<durability>().health = 100f;
						this.eng.timingGear16r.GetComponent<durability>().Start();
						foreach (object obj24 in this.eng.timingGear16r.transform)
						{
							Transform transform24 = (Transform)obj24;
							if (transform24.gameObject.name == "bolt")
							{
								transform24.gameObject.GetComponent<Renderer>().enabled = true;
								transform24.gameObject.GetComponent<BoxCollider>().enabled = true;
								transform24.gameObject.GetComponent<BoltScript>().boltturns = 10;
							}
						}
					}
				}
				if (this.eng.timingGear32_cnd < 6f)
				{
					this.cost += 70f;
					if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
					{
						this.eng.timingGear32_cnd = 100f;
						this.eng.timingGear32.GetComponent<durability>().health = 100f;
						this.eng.timingGear32.GetComponent<durability>().Start();
						foreach (object obj25 in this.eng.timingGear32.transform)
						{
							Transform transform25 = (Transform)obj25;
							if (transform25.gameObject.name == "bolt")
							{
								transform25.gameObject.GetComponent<Renderer>().enabled = true;
								transform25.gameObject.GetComponent<BoxCollider>().enabled = true;
								transform25.gameObject.GetComponent<BoltScript>().boltturns = 10;
							}
						}
					}
				}
				this.i++;
				yield return base.StartCoroutine(this.SpeakPart());
			}
			if (!this.eng.transmission.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_transmission;
				this.issue = this.gone;
				this.list1 = "[x] Transmission: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 600;
				this.cost += 600f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.transmission.GetComponent<Renderer>().enabled = true;
					this.eng.transmission_cnd = 100f;
					this.eng.transmission.GetComponent<durability>().health = 100f;
					BoxCollider[] components27 = this.eng.transmission.GetComponents<BoxCollider>();
					components27[0].enabled = true;
					components27[1].enabled = true;
					this.eng.transmission.GetComponent<durability>().Start();
					foreach (object obj26 in this.eng.transmission.transform)
					{
						Transform transform26 = (Transform)obj26;
						if (transform26.gameObject.name == "bolt")
						{
							transform26.gameObject.GetComponent<Renderer>().enabled = true;
							transform26.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform26.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			else if (this.eng.transmission_cnd < 6f)
			{
				this.part = this.part_transmission;
				this.issue = this.garbage;
				this.list1 = "[x] Transmission: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 600;
				this.cost += 600f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.transmission_cnd = 100f;
					this.eng.transmission.GetComponent<durability>().health = 100f;
					this.eng.transmission.GetComponent<durability>().Start();
					foreach (object obj27 in this.eng.transmission.transform)
					{
						Transform transform27 = (Transform)obj27;
						if (transform27.gameObject.name == "bolt")
						{
							transform27.gameObject.GetComponent<Renderer>().enabled = true;
							transform27.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform27.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			if (!this.eng.transferCase.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_transfercase;
				this.issue = this.gone;
				this.list1 = "[x] Transfer Case: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 400;
				this.cost += 400f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.transferCase.GetComponent<Renderer>().enabled = true;
					this.eng.transferCase_cnd = 100f;
					this.eng.transferCase.GetComponent<durability>().health = 100f;
					BoxCollider[] components28 = this.eng.transferCase.GetComponents<BoxCollider>();
					components28[0].enabled = true;
					components28[1].enabled = true;
					this.eng.transferCase.GetComponent<durability>().Start();
					foreach (object obj28 in this.eng.transferCase.transform)
					{
						Transform transform28 = (Transform)obj28;
						if (transform28.gameObject.name == "bolt")
						{
							transform28.gameObject.GetComponent<Renderer>().enabled = true;
							transform28.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform28.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			else if (this.eng.transferCase_cnd < 6f)
			{
				this.part = this.part_transfercase;
				this.issue = this.garbage;
				this.list1 = "[x] Transfer Case: Garbage";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 400;
				this.cost += 400f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.transferCase_cnd = 100f;
					this.eng.transferCase.GetComponent<durability>().health = 100f;
					this.eng.transferCase.GetComponent<durability>().Start();
					foreach (object obj29 in this.eng.transferCase.transform)
					{
						Transform transform29 = (Transform)obj29;
						if (transform29.gameObject.name == "bolt")
						{
							transform29.gameObject.GetComponent<Renderer>().enabled = true;
							transform29.gameObject.GetComponent<BoxCollider>().enabled = true;
							transform29.gameObject.GetComponent<BoltScript>().boltturns = 10;
						}
					}
				}
				this.i++;
			}
			if (!this.eng.plugwires.GetComponent<Renderer>().enabled)
			{
				this.part = this.part_wires;
				this.issue = this.gone;
				this.list1 = "[x] Plug Wires: Missing";
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 1) != "[")
				{
					this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				}
				this.priceArr[this.i] = 75;
				this.cost += 75f;
				yield return base.StartCoroutine(this.SpeakPart());
				if (this.fixing && this.diag.listEntries[this.i].GetComponent<Text>().text == this.list1)
				{
					this.eng.plugwires.GetComponent<Renderer>().enabled = true;
					this.eng.plugwires_cnd = 100f;
					this.eng.plugwires.GetComponent<durability>().health = 100f;
					BoxCollider[] components29 = this.eng.plugwires.GetComponents<BoxCollider>();
					components29[0].enabled = true;
					components29[1].enabled = true;
					this.eng.plugwires.transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = true;
				}
				this.i++;
			}
			if (this.numIssues == 0 && !this.fixing)
			{
				this.list1 = "No engine problems found.";
				this.diag.listEntries[this.i].GetComponent<Text>().text = this.list1;
				AudioSource.PlayClipAtPoint(this.hmm2, this.audioLoc.transform.position, 1f);
				yield return new WaitForSeconds(this.hmm2.length);
				AudioSource.PlayClipAtPoint(this.looksGood, this.audioLoc.transform.position, 1f);
				yield return new WaitForSeconds(this.looksGood.length);
				AudioSource.PlayClipAtPoint(this.notSure, this.audioLoc.transform.position, 1f);
				yield return new WaitForSeconds(this.notSure.length);
			}
			if (this.numIssues == 1)
			{
				AudioSource.PlayClipAtPoint(this.everythingElse, this.audioLoc.transform.position, 1f);
				yield return new WaitForSeconds(this.everythingElse.length);
			}
			if (this.numIssues > 8)
			{
				AudioSource.PlayClipAtPoint(this.part_entireengine, this.audioLoc.transform.position, 1f);
				yield return new WaitForSeconds(this.part_entireengine.length);
			}
			if (this.truckHood.GetComponent<InteractiveObject>().isOpen)
			{
				this.truckHood.GetComponent<InteractiveObject>().PerformAction();
			}
			yield return new WaitForSeconds(1f);
			this.isWorking = false;
			this.anim.Play("jj_walk7");
			this.isWalking4 = true;
			this.costText.GetComponent<Text>().text = "Repair Estimate: $" + this.cost;
			if (this.numIssues > 8)
			{
				Achievement achievement = new Achievement("ACH_GARBAGE");
				achievement.Trigger(true);
			}
		}
		this.inspecting = false;
		yield break;
	}

	// Token: 0x0600016E RID: 366 RVA: 0x000104D9 File Offset: 0x0000E6D9
	private IEnumerator SpeakPart()
	{
		if (!this.fixing)
		{
			if (this.issue == this.garbage)
			{
				this.rand = Random.Range(0, 4);
				if (this.rand == 0 && this.issue == this.garbage)
				{
					this.issue = this.garbage2;
				}
				if (this.rand == 1 && this.issue == this.garbage)
				{
					this.issue = this.garbage3;
				}
				if (this.rand == 2 && this.issue == this.garbage)
				{
					this.issue = this.garbage3;
				}
				AudioSource.PlayClipAtPoint(this.part, this.audioLoc.transform.position, 1f);
				yield return new WaitForSeconds(this.part.length);
				AudioSource.PlayClipAtPoint(this.issue, this.audioLoc.transform.position, 1f);
				yield return new WaitForSeconds(this.issue.length);
				this.numIssues++;
			}
			if (this.issue == this.gone)
			{
				this.rand = Random.Range(0, 3);
				if (this.rand == 1)
				{
					this.issue = this.gone2;
				}
				else if (this.rand == 2)
				{
					this.issue = this.gone3;
				}
				AudioSource.PlayClipAtPoint(this.part, this.audioLoc.transform.position, 1f);
				yield return new WaitForSeconds(this.part.length);
				AudioSource.PlayClipAtPoint(this.issue, this.audioLoc.transform.position, 1f);
				yield return new WaitForSeconds(this.issue.length);
				this.numIssues++;
			}
		}
		yield break;
	}

	// Token: 0x0600016F RID: 367 RVA: 0x000104E8 File Offset: 0x0000E6E8
	public void Fix()
	{
		if (!this.inCall)
		{
			if (Vector3.Distance(base.transform.position, this.mytruckpos.position) > 25f)
			{
				AudioSource.PlayClipAtPoint(this.tooFar, this.audioLoc.transform.position, 1f);
				return;
			}
			this.cost = 0f;
			this.i = 0;
			while (this.i <= 35)
			{
				if (this.diag.listEntries[this.i].GetComponent<Text>().text.Substring(0, 3) == "[x]")
				{
					this.cost += (float)this.priceArr[this.i];
				}
				this.i++;
			}
			if (this.cost > 0f && this.cost <= this.eventSystem.GetComponent<Currency>().money)
			{
				this.eventSystem.GetComponent<Currency>().money -= this.cost;
				this.inv.SubtractMoney(this.cost);
				AudioSource.PlayClipAtPoint(this.yourmoney, this.audioLoc.transform.position, 1f);
				this.cost = 0f;
				this.fixing = true;
				this.Inspect();
			}
		}
	}

	// Token: 0x06000170 RID: 368 RVA: 0x00002188 File Offset: 0x00000388
	public void ReplaceParts()
	{
	}

	// Token: 0x06000171 RID: 369 RVA: 0x00010644 File Offset: 0x0000E844
	public void FixSpecific()
	{
		int num = 0;
		foreach (object obj in this.diagPanel)
		{
			((Transform)obj).GetComponent<Text>().text.Substring(0, 3) == "[x]";
			num++;
		}
	}

	// Token: 0x040003C3 RID: 963
	public AudioSource aSource;

	// Token: 0x040003C4 RID: 964
	public engine eng;

	// Token: 0x040003C5 RID: 965
	public Transform audioLoc;

	// Token: 0x040003C6 RID: 966
	public AudioClip tooFar;

	// Token: 0x040003C7 RID: 967
	public AudioClip choosePaper;

	// Token: 0x040003C8 RID: 968
	public AudioClip yourmoney;

	// Token: 0x040003C9 RID: 969
	public AudioClip aCopy;

	// Token: 0x040003CA RID: 970
	public AudioClip hmm1;

	// Token: 0x040003CB RID: 971
	public AudioClip hmm2;

	// Token: 0x040003CC RID: 972
	public AudioClip lookslike;

	// Token: 0x040003CD RID: 973
	public AudioClip part_airfilter;

	// Token: 0x040003CE RID: 974
	public AudioClip part_alternator;

	// Token: 0x040003CF RID: 975
	public AudioClip part_battery;

	// Token: 0x040003D0 RID: 976
	public AudioClip part_bodywork;

	// Token: 0x040003D1 RID: 977
	public AudioClip part_camgear;

	// Token: 0x040003D2 RID: 978
	public AudioClip part_camshaft;

	// Token: 0x040003D3 RID: 979
	public AudioClip part_carb;

	// Token: 0x040003D4 RID: 980
	public AudioClip part_clutch;

	// Token: 0x040003D5 RID: 981
	public AudioClip part_coolant;

	// Token: 0x040003D6 RID: 982
	public AudioClip part_crank;

	// Token: 0x040003D7 RID: 983
	public AudioClip part_distributor;

	// Token: 0x040003D8 RID: 984
	public AudioClip part_entireengine;

	// Token: 0x040003D9 RID: 985
	public AudioClip part_exhaust;

	// Token: 0x040003DA RID: 986
	public AudioClip part_fan;

	// Token: 0x040003DB RID: 987
	public AudioClip part_fanbelt;

	// Token: 0x040003DC RID: 988
	public AudioClip part_head;

	// Token: 0x040003DD RID: 989
	public AudioClip part_headgasket;

	// Token: 0x040003DE RID: 990
	public AudioClip part_intake;

	// Token: 0x040003DF RID: 991
	public AudioClip part_oil;

	// Token: 0x040003E0 RID: 992
	public AudioClip part_oilfilter;

	// Token: 0x040003E1 RID: 993
	public AudioClip part_pistons;

	// Token: 0x040003E2 RID: 994
	public AudioClip part_radiator;

	// Token: 0x040003E3 RID: 995
	public AudioClip part_rocker;

	// Token: 0x040003E4 RID: 996
	public AudioClip part_thermostat;

	// Token: 0x040003E5 RID: 997
	public AudioClip part_timing;

	// Token: 0x040003E6 RID: 998
	public AudioClip part_transmission;

	// Token: 0x040003E7 RID: 999
	public AudioClip part_transfercase;

	// Token: 0x040003E8 RID: 1000
	public AudioClip part_wires;

	// Token: 0x040003E9 RID: 1001
	public AudioClip loose;

	// Token: 0x040003EA RID: 1002
	public AudioClip garbage;

	// Token: 0x040003EB RID: 1003
	public AudioClip garbage2;

	// Token: 0x040003EC RID: 1004
	public AudioClip garbage3;

	// Token: 0x040003ED RID: 1005
	public AudioClip garbage4;

	// Token: 0x040003EE RID: 1006
	public AudioClip gone;

	// Token: 0x040003EF RID: 1007
	public AudioClip gone2;

	// Token: 0x040003F0 RID: 1008
	public AudioClip gone3;

	// Token: 0x040003F1 RID: 1009
	public AudioClip aintgot;

	// Token: 0x040003F2 RID: 1010
	public AudioClip allright;

	// Token: 0x040003F3 RID: 1011
	public AudioClip looksGood;

	// Token: 0x040003F4 RID: 1012
	public AudioClip notSure;

	// Token: 0x040003F5 RID: 1013
	public AudioClip everythingElse;

	// Token: 0x040003F6 RID: 1014
	private AudioClip part;

	// Token: 0x040003F7 RID: 1015
	private AudioClip issue;

	// Token: 0x040003F8 RID: 1016
	public AudioClip noway;

	// Token: 0x040003F9 RID: 1017
	private int rand;

	// Token: 0x040003FA RID: 1018
	public int numIssues;

	// Token: 0x040003FB RID: 1019
	public bool skip;

	// Token: 0x040003FC RID: 1020
	private Animation anim;

	// Token: 0x040003FD RID: 1021
	private bool isWalking1;

	// Token: 0x040003FE RID: 1022
	private bool isWalking2;

	// Token: 0x040003FF RID: 1023
	private bool isWalking3;

	// Token: 0x04000400 RID: 1024
	private bool isWalking4;

	// Token: 0x04000401 RID: 1025
	private bool isWalking5;

	// Token: 0x04000402 RID: 1026
	private bool isTalking;

	// Token: 0x04000403 RID: 1027
	private float rotSpeed = 10f;

	// Token: 0x04000404 RID: 1028
	private float moveSpeed = 1f;

	// Token: 0x04000405 RID: 1029
	public Transform walkto1;

	// Token: 0x04000406 RID: 1030
	public Transform walkto2;

	// Token: 0x04000407 RID: 1031
	public Transform walkto3;

	// Token: 0x04000408 RID: 1032
	public Transform walkto4;

	// Token: 0x04000409 RID: 1033
	public Transform truckpos;

	// Token: 0x0400040A RID: 1034
	public Transform phonepos;

	// Token: 0x0400040B RID: 1035
	public Transform mytruckpos;

	// Token: 0x0400040C RID: 1036
	private bool isWorking;

	// Token: 0x0400040D RID: 1037
	public GameObject myTruck;

	// Token: 0x0400040E RID: 1038
	public GameObject truckHood;

	// Token: 0x0400040F RID: 1039
	public GameObject drivDoor;

	// Token: 0x04000410 RID: 1040
	public GameObject passDoor;

	// Token: 0x04000411 RID: 1041
	public GameObject fob;

	// Token: 0x04000412 RID: 1042
	public GameObject inspectionPaper;

	// Token: 0x04000413 RID: 1043
	public GameObject InspectionText1;

	// Token: 0x04000414 RID: 1044
	public GameObject InspectionText2;

	// Token: 0x04000415 RID: 1045
	private string list1;

	// Token: 0x04000416 RID: 1046
	private string list2;

	// Token: 0x04000417 RID: 1047
	public float cost;

	// Token: 0x04000418 RID: 1048
	public GameObject costText;

	// Token: 0x04000419 RID: 1049
	public Transform phone;

	// Token: 0x0400041A RID: 1050
	public Transform leftHand;

	// Token: 0x0400041B RID: 1051
	public AudioClip ring;

	// Token: 0x0400041C RID: 1052
	public AudioClip[] jimmyspeaking;

	// Token: 0x0400041D RID: 1053
	public AudioClip[] yeah;

	// Token: 0x0400041E RID: 1054
	public AudioClip carready;

	// Token: 0x0400041F RID: 1055
	public AudioClip[] what;

	// Token: 0x04000420 RID: 1056
	public AudioClip wedont;

	// Token: 0x04000421 RID: 1057
	public AudioClip[] pencil;

	// Token: 0x04000422 RID: 1058
	public AudioClip haveitdone;

	// Token: 0x04000423 RID: 1059
	public AudioClip[] okay;

	// Token: 0x04000424 RID: 1060
	public AudioClip hangup;

	// Token: 0x04000425 RID: 1061
	public AudioClip[] homeloan;

	// Token: 0x04000426 RID: 1062
	public AudioClip ass;

	// Token: 0x04000427 RID: 1063
	public AudioClip unbelievable;

	// Token: 0x04000428 RID: 1064
	public AudioClip[] jobClip;

	// Token: 0x04000429 RID: 1065
	private AudioClip clip1;

	// Token: 0x0400042A RID: 1066
	private AudioClip clip2;

	// Token: 0x0400042B RID: 1067
	private AudioClip clip3;

	// Token: 0x0400042C RID: 1068
	private AudioClip clip4;

	// Token: 0x0400042D RID: 1069
	private AudioClip clip5;

	// Token: 0x0400042E RID: 1070
	private AudioClip clip6;

	// Token: 0x0400042F RID: 1071
	private AudioClip clip7;

	// Token: 0x04000430 RID: 1072
	public bool inCall;

	// Token: 0x04000431 RID: 1073
	private Vector3 sitpos;

	// Token: 0x04000432 RID: 1074
	private Quaternion sitrot;

	// Token: 0x04000433 RID: 1075
	public bool fixing;

	// Token: 0x04000434 RID: 1076
	public GameObject eventSystem;

	// Token: 0x04000435 RID: 1077
	public InventoryItems inv;

	// Token: 0x04000436 RID: 1078
	private bool isBusy;

	// Token: 0x04000437 RID: 1079
	private bool inspecting;

	// Token: 0x04000438 RID: 1080
	public GameObject truckWinch;

	// Token: 0x04000439 RID: 1081
	public Transform diagPanel;

	// Token: 0x0400043A RID: 1082
	public int[] priceArr;

	// Token: 0x0400043B RID: 1083
	private int i;

	// Token: 0x0400043C RID: 1084
	public DiagCanv diag;

	// Token: 0x0400043D RID: 1085
	public int jobs;

	// Token: 0x0400043E RID: 1086
	public GameObject vette;

	// Token: 0x0400043F RID: 1087
	public MissionController mc;

	// Token: 0x04000440 RID: 1088
	public VetteWaterCheck vwc;

	// Token: 0x04000441 RID: 1089
	public GameObject moneyRoll;

	// Token: 0x04000442 RID: 1090
	public GameObject moneyLoc;

	// Token: 0x04000443 RID: 1091
	private GameObject newRoll;

	// Token: 0x04000444 RID: 1092
	public Interactor interactor;

	// Token: 0x04000445 RID: 1093
	private string dialogue;
}
