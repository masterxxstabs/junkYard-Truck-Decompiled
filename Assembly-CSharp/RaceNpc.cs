using System;
using System.Collections;
using Steamworks.Data;
using UnityEngine;

// Token: 0x02000120 RID: 288
public class RaceNpc : MonoBehaviour
{
	// Token: 0x06000797 RID: 1943 RVA: 0x00062782 File Offset: 0x00060982
	private void Start()
	{
		this.ResetTruck();
	}

	// Token: 0x06000798 RID: 1944 RVA: 0x0006278C File Offset: 0x0006098C
	public void PayEntry(int which)
	{
		this.whichRace = which;
		float num = Vector3.Distance(this.startpos.position, this.mytruckpos.position);
		float num2 = Vector3.Distance(this.startpos.position, this.mytruckposF.position);
		float num3 = Vector3.Distance(this.startpos.position, this.mytruckposM.position);
		if (num > 15f && num2 > 15f && num3 > 15f)
		{
			this.raceCanvas.SetActive(false);
			this.inter.fpc.enabled = true;
			this.inter.fpc.LockMouse();
			Cursor.visible = false;
			this.dialogue = "Where's your truck? Bring your truck to the starting line.";
			this.interactor.Subtitle(this.dialogue, this.clip3);
			return;
		}
		if (this.currency.money > 49f && this.status == 1)
		{
			this.currency.money -= 50f;
			this.fps = GameObject.Find("FPSController");
			this.fps.GetComponent<Interactor>().inv.SubtractMoney(50f);
			this.status = 2;
			this.Interact();
			this.raceCanvas.SetActive(false);
			this.raceCanvas2.SetActive(false);
			this.inter.fpc.enabled = true;
			this.inter.fpc.LockMouse();
			Cursor.visible = false;
		}
	}

	// Token: 0x06000799 RID: 1945 RVA: 0x00062910 File Offset: 0x00060B10
	public void Interact()
	{
		if (this.wonrace)
		{
			this.status = 3;
			this.interactor.raceActive = false;
		}
		if (!this.wonrace && this.status == 5 && this.elapsed)
		{
			this.status = 4;
			this.interactor.raceActive = false;
		}
		if (this.status == 0)
		{
			AudioSource.PlayClipAtPoint(this.clip6, this.audioLoc.transform.position, 0.9f);
			this.dialogue = "Come back tomorrow.";
			this.interactor.Subtitle(this.dialogue, this.clip6);
		}
		if (this.status == 5 && !this.elapsed)
		{
			AudioSource.PlayClipAtPoint(this.clip5, this.audioLoc.transform.position, 0.9f);
			this.dialogue = "Hurry up. Get in your truck.";
			this.interactor.Subtitle(this.dialogue, this.clip5);
		}
		if (this.status == 1)
		{
			AudioSource.PlayClipAtPoint(this.clip1, this.audioLoc.transform.position, 0.9f);
			this.dialogue = "Sign the form if you want to race. Entry is fifty dollars.";
			this.interactor.Subtitle(this.dialogue, this.clip1);
		}
		if (this.status == 2)
		{
			float num = Vector3.Distance(this.startpos.position, this.mytruckpos.position);
			float num2 = Vector3.Distance(this.startpos.position, this.mytruckposF.position);
			float num3 = Vector3.Distance(this.startpos.position, this.mytruckposM.position);
			if (num > 15f && num2 > 15f && num3 > 15f)
			{
				AudioSource.PlayClipAtPoint(this.clip3, this.audioLoc.transform.position, 0.9f);
				this.dialogue = "Where's your truck? Bring your truck to the starting line.";
				this.interactor.Subtitle(this.dialogue, this.clip3);
			}
			else
			{
				AudioSource.PlayClipAtPoint(this.clip2, this.audioLoc.transform.position, 0.9f);
				this.dialogue = "Get in your truck. Race starts in ten seconds.";
				this.interactor.Subtitle(this.dialogue, this.clip2);
				if (this.whichRace == 0)
				{
					base.StartCoroutine(this.StartRace());
					this.raceTruck1.GetComponents<AudioSource>()[0].enabled = true;
				}
				if (this.whichRace == 1)
				{
					base.StartCoroutine(this.StartRace2());
					this.raceCar1.GetComponents<AudioSource>()[2].Play();
				}
				this.elapsed = false;
				this.status = 5;
				this.finished = false;
			}
		}
		if (this.status == 3)
		{
			AudioSource.PlayClipAtPoint(this.clip4, this.audioLoc.transform.position, 0.9f);
			this.dialogue = "Good job. This is for you.";
			this.DisableTruck();
			this.interactor.Subtitle(this.dialogue, this.clip4);
			this.wonrace = false;
			this.status = 0;
			if (this.whichRace == 0)
			{
				if (this.whichCar == 0)
				{
					this.trophy.pickable = true;
				}
				else if (this.whichCar == 1)
				{
					this.trophy1f.pickable = true;
				}
				else if (this.whichCar == 2)
				{
					this.trophy1m.pickable = true;
				}
			}
			if (this.whichRace == 1)
			{
				if (this.whichCar == 0)
				{
					this.trophy2.pickable = true;
				}
				else if (this.whichCar == 1)
				{
					this.trophy2f.pickable = true;
				}
				else if (this.whichCar == 2)
				{
					this.trophy2m.pickable = true;
				}
				Achievement achievement = new Achievement("ACH_DITCHWITCH");
				achievement.Trigger(true);
			}
		}
		if (this.status == 4)
		{
			AudioSource.PlayClipAtPoint(this.clip7, this.audioLoc.transform.position, 0.9f);
			this.dialogue = "Tough luck. Try again another day.";
			this.DisableTruck();
			this.interactor.Subtitle(this.dialogue, this.clip7);
			this.status = 0;
		}
	}

	// Token: 0x0600079A RID: 1946 RVA: 0x00062D14 File Offset: 0x00060F14
	public void ActivateTrophy()
	{
		if (this.whichCar == 1)
		{
			if (this.whichRace == 0)
			{
				this.trophy1f.gameObject.SetActive(true);
			}
			else
			{
				this.trophy2f.gameObject.SetActive(true);
			}
		}
		if (this.whichCar == 2)
		{
			if (this.whichRace == 0)
			{
				this.trophy1m.gameObject.SetActive(true);
				return;
			}
			this.trophy2m.gameObject.SetActive(true);
		}
	}

	// Token: 0x0600079B RID: 1947 RVA: 0x00062D8C File Offset: 0x00060F8C
	public void ResetTruck()
	{
		this.raceTruck1.transform.position = this.aiStartPos.position;
		this.raceCar1.transform.position = this.aiStartPos2.position;
		this.raceTruck1.transform.rotation = this.aiStartPos.rotation;
		this.raceCar1.transform.rotation = this.aiStartPos2.rotation;
		this.DisableTruck();
		if (Random.Range(1, 3) == 1)
		{
			this.raceCar1.SetActive(false);
			this.raceTruck1.SetActive(true);
			this.raceForm1.SetActive(true);
			this.raceForm2.SetActive(false);
			return;
		}
		this.raceTruck1.SetActive(false);
		this.raceCar1.SetActive(true);
		this.raceForm1.SetActive(false);
		this.raceForm2.SetActive(true);
	}

	// Token: 0x0600079C RID: 1948 RVA: 0x00062E78 File Offset: 0x00061078
	public void DisableTruck()
	{
		AudioSource[] components = this.raceTruck1.GetComponents<AudioSource>();
		components[0].enabled = false;
		components[1].enabled = false;
		this.raceTruck1.GetComponent<CarDriver>().enabled = false;
		this.raceTruck1.GetComponent<CarDriverAI>().enabled = false;
		AudioSource[] components2 = this.raceCar1.GetComponents<AudioSource>();
		components2[0].enabled = false;
		components2[1].enabled = false;
		this.raceCar1.GetComponent<CarDriver>().enabled = false;
		this.raceCar1.GetComponent<CarDriverAI>().enabled = false;
		this.raceCar1.GetComponent<DemoCarMotion>().enabled = false;
	}

	// Token: 0x0600079D RID: 1949 RVA: 0x00062F12 File Offset: 0x00061112
	private IEnumerator StartRace()
	{
		this.interactor.whichRace = 0;
		this.interactor.targetPositionTranform = GameObject.Find("target").transform;
		this.interactor.nextTarget = GameObject.Find("target (1)").transform;
		yield return new WaitForSeconds(5f);
		this.inter.Heartbeat();
		yield return new WaitForSeconds(5f);
		this.startpos.gameObject.GetComponent<AudioSource>().Play();
		this.raceTruck1.GetComponent<CarDriver>().enabled = true;
		this.raceTruck1.GetComponent<CarDriverAI>().enabled = true;
		this.raceTruck1.GetComponent<CarDriverAI>().Start();
		this.raceTruck1.GetComponents<AudioSource>()[1].enabled = true;
		this.interactor.raceActive = true;
		yield return new WaitForSeconds(30f);
		this.elapsed = true;
		yield break;
	}

	// Token: 0x0600079E RID: 1950 RVA: 0x00062F21 File Offset: 0x00061121
	private IEnumerator StartRace2()
	{
		this.interactor.whichRace = 1;
		this.interactor.targetPositionTranform = GameObject.Find("targetb").transform;
		this.interactor.nextTarget = GameObject.Find("targetb (1)").transform;
		this.raceCar1.GetComponent<DemoCarMotion>().enabled = true;
		AudioSource[] asources = this.raceCar1.GetComponents<AudioSource>();
		yield return new WaitForSeconds(5f);
		this.inter.Heartbeat();
		yield return new WaitForSeconds(5f);
		this.startpos.gameObject.GetComponent<AudioSource>().Play();
		this.raceCar1.GetComponent<CarDriver>().enabled = true;
		this.raceCar1.GetComponent<CarDriverAI>().enabled = true;
		this.raceCar1.GetComponent<CarDriverAI>().Start();
		this.interactor.raceActive = true;
		asources[0].enabled = true;
		asources[1].enabled = true;
		yield return new WaitForSeconds(20f);
		this.elapsed = true;
		yield break;
	}

	// Token: 0x04001143 RID: 4419
	public AudioClip clip1;

	// Token: 0x04001144 RID: 4420
	public AudioClip clip2;

	// Token: 0x04001145 RID: 4421
	public AudioClip clip3;

	// Token: 0x04001146 RID: 4422
	public AudioClip clip4;

	// Token: 0x04001147 RID: 4423
	public AudioClip clip5;

	// Token: 0x04001148 RID: 4424
	public AudioClip clip6;

	// Token: 0x04001149 RID: 4425
	public AudioClip clip7;

	// Token: 0x0400114A RID: 4426
	public int status;

	// Token: 0x0400114B RID: 4427
	public AudioSource aSource;

	// Token: 0x0400114C RID: 4428
	public Transform audioLoc;

	// Token: 0x0400114D RID: 4429
	public Transform mytruckpos;

	// Token: 0x0400114E RID: 4430
	public Transform mytruckposF;

	// Token: 0x0400114F RID: 4431
	public Transform mytruckposM;

	// Token: 0x04001150 RID: 4432
	public Transform startpos;

	// Token: 0x04001151 RID: 4433
	private string dialogue;

	// Token: 0x04001152 RID: 4434
	public Interactor interactor;

	// Token: 0x04001153 RID: 4435
	public GameObject raceTruck1;

	// Token: 0x04001154 RID: 4436
	public GameObject raceCar1;

	// Token: 0x04001155 RID: 4437
	public bool wonrace;

	// Token: 0x04001156 RID: 4438
	public bool elapsed;

	// Token: 0x04001157 RID: 4439
	public Transform aiStartPos;

	// Token: 0x04001158 RID: 4440
	public Transform aiStartPos2;

	// Token: 0x04001159 RID: 4441
	public GameObject raceCanvas;

	// Token: 0x0400115A RID: 4442
	public GameObject raceCanvas2;

	// Token: 0x0400115B RID: 4443
	public Currency currency;

	// Token: 0x0400115C RID: 4444
	private GameObject fps;

	// Token: 0x0400115D RID: 4445
	public Interactor inter;

	// Token: 0x0400115E RID: 4446
	public PickUp trophy;

	// Token: 0x0400115F RID: 4447
	public PickUp trophy2;

	// Token: 0x04001160 RID: 4448
	public PickUp trophy1f;

	// Token: 0x04001161 RID: 4449
	public PickUp trophy2f;

	// Token: 0x04001162 RID: 4450
	public PickUp trophy1m;

	// Token: 0x04001163 RID: 4451
	public PickUp trophy2m;

	// Token: 0x04001164 RID: 4452
	public bool finished = true;

	// Token: 0x04001165 RID: 4453
	public int whichRace;

	// Token: 0x04001166 RID: 4454
	public GameObject raceForm1;

	// Token: 0x04001167 RID: 4455
	public GameObject raceForm2;

	// Token: 0x04001168 RID: 4456
	public int whichCar;
}
