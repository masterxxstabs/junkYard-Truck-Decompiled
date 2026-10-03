using System;
using System.Collections;
using System.Collections.Generic;
using AshVP;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x0200010A RID: 266
public class Officer : MonoBehaviour
{
	// Token: 0x060006D6 RID: 1750 RVA: 0x00057BDC File Offset: 0x00055DDC
	private void Start()
	{
		this.driving = false;
		this.chasing = false;
		this.idleAtGasStation = false;
		this.idleAtStation = false;
		this.servingWarrant = false;
		this.noStepOut = false;
		this.nav = base.GetComponent<NavMeshAgent>();
		this.rb = base.GetComponent<Rigidbody>();
		this.RagdollOff();
		this.vehSoundDictionary = new Dictionary<string, AudioClip>();
		this.vehLowSoundDictionary = new Dictionary<string, AudioClip>();
		this.copSoundDictionary = new Dictionary<string, AudioClip>();
		this.miscSoundDictionary = new Dictionary<string, AudioClip>();
		foreach (AudioClip audioClip in this.vehSounds)
		{
			this.vehSoundDictionary[audioClip.name] = audioClip;
		}
		foreach (AudioClip audioClip2 in this.vehSoundsLow)
		{
			this.vehLowSoundDictionary[audioClip2.name] = audioClip2;
		}
		foreach (AudioClip audioClip3 in this.dialogue)
		{
			this.copSoundDictionary[audioClip3.name] = audioClip3;
		}
		foreach (AudioClip audioClip4 in this.miscSounds)
		{
			this.miscSoundDictionary[audioClip4.name] = audioClip4;
		}
		float length = this.radio.clip.length;
		float time = Random.Range(0f, length);
		this.radio.time = time;
		this.radio.Play();
		this.anim.Play("officerwalk3");
		this.charge_opencontainer = 0;
	}

	// Token: 0x060006D7 RID: 1751 RVA: 0x00057D64 File Offset: 0x00055F64
	private void Update()
	{
		this.timer += Time.deltaTime;
		this.CheckIfStuck();
		this.distanceToPlayer = Vector3.Distance(base.transform.position, this.fps.position);
		this.distanceToUnit = Vector3.Distance(base.transform.position, this.entrance.position);
		if (this.distanceToPlayer < this.fovDistance)
		{
			this.DetectPlayer();
		}
		if (this.servingWarrant)
		{
			this.warrantTimer += Time.deltaTime;
		}
		if (this.warrantTimer > 40f)
		{
			this.servingWarrant = false;
			this.warrantTimer = 0f;
		}
		if (this.chasing && !this.isRagdoll)
		{
			if (!this.driving)
			{
				if (this.distanceToUnit > 40f && !this.servingWarrant)
				{
					this.destination = this.entrance;
				}
				if (this.distanceToPlayer > 70f)
				{
					this.chasing = false;
					this.warrant = true;
					this.charge_evasion++;
					this.destination = this.entrance;
				}
			}
			else if (this.driving)
			{
				if (this.distanceToPlayer < 10f)
				{
					if (this.person.transform.parent == null)
					{
						this.OfficerExitCoroutine();
					}
				}
				else if (this.distanceToPlayer > 70f)
				{
					this.chasing = false;
					this.warrant = true;
					this.charge_evasion++;
					this.patrolCar.Pursue(false);
					this.patrolCar.ResumeOrRecalc();
				}
			}
		}
		if (this.cuffTimerActive && this.playerCuffed && this.destination == this.entrance)
		{
			if (!this.servingWarrant)
			{
				this.cuffTimer += Time.deltaTime;
			}
			if (this.cuffTimer >= 20f)
			{
				this.cuffTimerActive = false;
				NavMeshHit navMeshHit;
				if (NavMesh.SamplePosition(this.entrance.position, out navMeshHit, 1f, -1))
				{
					this.nav.isStopped = true;
					this.nav.updateRotation = false;
					this.nav.Warp(this.entrance.position);
					this.nav.isStopped = false;
					this.nav.updateRotation = true;
				}
				else
				{
					this.officerEnterCoroutine = base.StartCoroutine(this.OfficerEnter());
				}
			}
		}
		if (this.cuffTimerActive && this.playerCuffed && this.destination == this.jail0)
		{
			this.cuffTimer += Time.deltaTime;
			if (this.cuffTimer >= 40f)
			{
				this.cuffTimerActive = false;
				this.nav.isStopped = true;
				this.nav.updateRotation = false;
				this.nav.Warp(this.jail0.position);
				this.nav.isStopped = false;
				this.nav.updateRotation = true;
			}
		}
		if (this.destination == this.entrance && this.distanceToUnit < 1f && !this.driving && base.transform.parent == null && !this.isRagdoll && this.officerEnterCoroutine == null && this.officerExitCoroutine == null)
		{
			this.officerEnterCoroutine = base.StartCoroutine(this.OfficerEnter());
		}
		if (this.driving && this.distanceToPlayer < 12f && Time.time > this.cooldown)
		{
			if (this.fps.parent != null)
			{
				if (this.fps.parent.name == "DriverCameraController" || this.fps.parent.name == "f100seatmount" || this.fps.parent.name == "DriverCameraControllerCar" || this.fps.parent.name == "seatpos" || this.fps.parent.name == "SeatMount")
				{
					if (this.charge_hitandrun > 0 && !this.patrolCar.pursuing)
					{
						this.patrolCar.Pursue(true);
						this.patrolCar.ToggleLights(true);
						this.VehAudio("yelp2");
					}
					if (this.till.unpaidFuel && !this.theftNoted && !this.patrolCar.pursuing)
					{
						this.patrolCar.Pursue(true);
						this.patrolCar.ToggleLights(true);
						this.VehAudio("yelp2");
						this.charge_theft = 1;
						this.theftNoted = true;
						Debug.Log("theft");
					}
					if ((this.fpsBeer.activeSelf || this.fpsBottle.activeSelf) && !this.patrolCar.pursuing)
					{
						Debug.Log("opencontainer111");
						Debug.Log(this.fpsBottle.activeSelf);
						this.charge_opencontainer = 1;
						this.subtractContainer = 1;
						this.patrolCar.Pursue(true);
						this.patrolCar.ToggleLights(true);
						this.VehAudio("yelp2");
					}
				}
				if (this.fps.parent.name == "DriverCameraController")
				{
					if (this.diamondback.speed > 5f && !this.light_db.enabled && !this.light_kc.enabled && !this.patrolCar.pursuing)
					{
						float timeOfDay = EnviroSkyMgr.instance.GetTimeOfDay();
						if (timeOfDay < 6f || timeOfDay > 17f)
						{
							this.charge_headlights++;
							this.patrolCar.Pursue(true);
							this.patrolCar.ToggleLights(true);
							this.VehAudio("yelp2");
						}
					}
					if (this.diamondback.speed > 4f && this.acDiamondback.rotorCount > 0 && !this.patrolCar.pursuing)
					{
						this.patrolCar.Pursue(true);
						this.patrolCar.ToggleLights(true);
						this.VehAudio("yelp2");
						this.charge_reckless = 1;
					}
					if (this.diamondback.speed > 60f && !this.patrolCar.pursuing && this.PursueDelayCoroutine == null)
					{
						this.charge_speeding++;
						this.mwb.CheckCompletion(2);
						this.PursueDelayCoroutine = base.StartCoroutine(this.PursueDelay());
					}
					if (this.diamondback.speed < 2f && this.patrolCar.pursuing)
					{
						this.trafficStop = true;
						this.OfficerExitCoroutine();
					}
				}
				if (this.fps.parent.name == "f100seatmount")
				{
					if (this.f100.speed > 5f && !this.light_f100.enabled && !this.light_kcf.enabled && !this.patrolCar.pursuing)
					{
						float timeOfDay2 = EnviroSkyMgr.instance.GetTimeOfDay();
						if (timeOfDay2 < 6f || timeOfDay2 > 17f)
						{
							this.charge_headlights++;
							this.patrolCar.Pursue(true);
							this.patrolCar.ToggleLights(true);
							this.VehAudio("yelp2");
						}
					}
					if (this.f100.speed > 60f && !this.patrolCar.pursuing && this.PursueDelayCoroutine == null)
					{
						this.charge_speeding++;
						this.mwb.CheckCompletion(2);
						this.PursueDelayCoroutine = base.StartCoroutine(this.PursueDelay());
					}
					if (this.f100.speed > 4f && this.acF100.rotorCount > 0 && !this.patrolCar.pursuing)
					{
						this.patrolCar.Pursue(true);
						this.patrolCar.ToggleLights(true);
						this.VehAudio("yelp2");
						this.charge_reckless = 1;
					}
					if (this.f100.speed < 2f && this.patrolCar.pursuing)
					{
						this.trafficStop = true;
						this.OfficerExitCoroutine();
					}
				}
				if (this.fps.parent.name == "DriverCameraControllerCar")
				{
					if (this.amc.speed > 5f && !this.light_amc.enabled && !this.patrolCar.pursuing)
					{
						float timeOfDay3 = EnviroSkyMgr.instance.GetTimeOfDay();
						if (timeOfDay3 < 6f || timeOfDay3 > 17f)
						{
							this.charge_headlights++;
							this.patrolCar.Pursue(true);
							this.patrolCar.ToggleLights(true);
							this.VehAudio("yelp2");
						}
					}
					if (this.amc.speed > 60f && !this.patrolCar.pursuing && this.PursueDelayCoroutine == null)
					{
						this.charge_speeding++;
						this.mwb.CheckCompletion(2);
						this.PursueDelayCoroutine = base.StartCoroutine(this.PursueDelay());
					}
					if (this.amc.speed < 2f && this.patrolCar.pursuing)
					{
						this.trafficStop = true;
						this.OfficerExitCoroutine();
					}
				}
				if (this.fps.parent.name == "seatpos" && this.golfCart.speed < 2f && this.patrolCar.pursuing)
				{
					this.trafficStop = true;
					this.OfficerExitCoroutine();
				}
				if (this.fps.parent.name == "SeatMount")
				{
					if (this.dirtbike.speed > 60f && !this.patrolCar.pursuing && this.PursueDelayCoroutine == null)
					{
						this.charge_speeding++;
						this.PursueDelayCoroutine = base.StartCoroutine(this.PursueDelay());
					}
					if (this.dirtbike.speed < 2f && this.patrolCar.pursuing)
					{
						this.trafficStop = true;
						this.OfficerExitCoroutine();
					}
				}
			}
			if (this.warrant)
			{
				this.patrolCar.Pursue(true);
				this.patrolCar.ToggleLights(true);
				this.VehAudio("yelp2");
			}
			if (this.fps.parent == null && this.playerCurrency.drunk > 8 && !this.patrolCar.pursuing)
			{
				this.charge_pubIntox = 1;
				this.patrolCar.Pursue(true);
				this.patrolCar.ToggleLights(true);
				this.VehAudio("yelp2");
			}
			if (this.fps.parent == null && this.patrolCar.pursuing && this.distanceToPlayer < 7f)
			{
				this.chasing = true;
				this.OfficerExitCoroutine();
			}
		}
		if (!this.driving && !this.isRagdoll && (this.destination == this.diamondbackEntry || this.destination == this.f100Entry || this.destination == this.amcEntry || this.destination == this.dirtbikeEntry || this.destination == this.golfcartEntry))
		{
			this.distanceToTarget = Vector3.Distance(base.transform.position, this.destination.position);
			bool flag = false;
			if (this.diamondback.speed > 3f && this.destination == this.diamondbackEntry)
			{
				flag = true;
			}
			else if (this.f100.speed > 3f && this.destination == this.f100Entry)
			{
				flag = true;
			}
			else if (this.amc.speed > 3f && this.destination == this.amcEntry)
			{
				flag = true;
			}
			else if (this.dirtbike.speed > 3f && this.destination == this.dirtbikeEntry)
			{
				flag = true;
			}
			else if (this.golfCart.speed > 3f && this.destination == this.golfcartEntry)
			{
				flag = true;
			}
			if (Time.time >= this.lastPlayTime + 20f && flag)
			{
				this.CopAudio("stop1");
				this.lastPlayTime = Time.time;
				this.criminalActivity = true;
				this.chasing = true;
				this.charge_obstruction++;
			}
			if (this.distanceToTarget < 1.3f)
			{
				this.nav.isStopped = true;
				this.destination = null;
				this.patrolCar.ToggleSiren(false);
				if (this.initiateStopCoroutine == null && this.fps.parent != null)
				{
					this.initiateStopCoroutine = base.StartCoroutine(this.InitiateStop());
				}
			}
		}
		if (this.timer >= 1f && !this.isRagdoll)
		{
			this.timer = 0f;
			if (this.trafficStop && this.noStepOut && this.fps.parent == null && this.waitForComplianceCoroutine == null)
			{
				this.CopAudio("backInVehicleAlt");
				this.nav.isStopped = true;
				this.waitForComplianceCoroutine = base.StartCoroutine(this.WaitForCompliance());
			}
			if (this.driving)
			{
				if (this.distanceToPlayer < 50f)
				{
					float timeOfDay4 = EnviroSkyMgr.instance.GetTimeOfDay();
					if (timeOfDay4 < 7f || timeOfDay4 > 17f)
					{
						this.headlights.SetActive(true);
					}
				}
				else
				{
					this.headlights.SetActive(false);
				}
			}
			else
			{
				this.headlights.SetActive(false);
			}
			if (this.driving || this.trafficStop)
			{
				if (this.distanceToPlayer < 50f && !this.playerCuffed && (this.warrant || this.criminalActivity))
				{
					float timeOfDay5 = EnviroSkyMgr.instance.GetTimeOfDay();
					if (timeOfDay5 < 6f || timeOfDay5 > 17f)
					{
						this.spotlight.SetActive(true);
					}
				}
				else
				{
					this.spotlight.SetActive(false);
				}
			}
			if (this.distanceToPlayer < 5f && !this.driving && !this.trafficStop && this.charge_posession < 1)
			{
				float num = Vector3.Distance(base.transform.position, this.bucket1.transform.position);
				float num2 = Vector3.Distance(base.transform.position, this.bucket2.transform.position);
				float num3 = Vector3.Distance(base.transform.position, this.bucket3.transform.position);
				if ((num < 5f || num2 < 5f || num3 < 5f) && (this.bucket1Script.alcohol > 0 || this.bucket2Script.alcohol > 0 || this.bucket3Script.alcohol > 0) && Time.time >= this.lastPlayTime + 20f)
				{
					this.CopAudio("moonshine1");
					this.lastPlayTime = Time.time;
					this.criminalActivity = true;
					if (!this.playerCuffed)
					{
						this.chasing = true;
						this.destination = this.fps;
					}
					this.charge_posession++;
					if (this.waitAtStationCoroutine != null)
					{
						base.StopCoroutine(this.waitAtStationCoroutine);
						this.waitAtStationCoroutine = null;
					}
					this.nav.enabled = true;
					this.nav.isStopped = false;
					this.anim.Play("officerwalk3");
					this.currentlyWaiting = false;
				}
			}
			if (this.playerCurrency.drunk > 2 && !this.trafficStop && !this.playerCuffed && !this.chasing && !this.driving && this.distanceToPlayer < 2f && Time.time >= this.lastPlayTime + 30f)
			{
				Debug.Log("alcohol");
				this.CopAudio("smellAlcohol1");
				this.lastPlayTime = Time.time;
			}
			if (this.warrant && !this.trafficStop && !this.playerCuffed && !this.chasing && !this.driving && this.officerEnterCoroutine == null && this.distanceToPlayer < 5f && Time.time >= this.lastPlayTime + 30f)
			{
				this.DetermineDestination();
				this.CopAudio("comehere");
				this.lastPlayTime = Time.time;
				this.criminalActivity = true;
				this.chasing = true;
				if (this.waitAtStationCoroutine != null)
				{
					base.StopCoroutine(this.waitAtStationCoroutine);
				}
				this.waitAtStationCoroutine = null;
				this.nav.enabled = true;
				this.nav.isStopped = false;
				this.anim.Play("officerwalk3");
				this.currentlyWaiting = false;
			}
			if (this.playerCurrency.drunk > 9 && !this.trafficStop && !this.playerCuffed && !this.driving && this.distanceToPlayer < 5f && Time.time >= this.lastPlayTime + 30f)
			{
				this.DetermineDestination();
				Debug.Log("publicintox");
				this.CopAudio("comehere");
				this.charge_pubIntox = 1;
				this.lastPlayTime = Time.time;
				this.criminalActivity = true;
				this.chasing = true;
				if (this.waitAtStationCoroutine != null)
				{
					base.StopCoroutine(this.waitAtStationCoroutine);
				}
				this.waitAtStationCoroutine = null;
				this.nav.enabled = true;
				this.nav.isStopped = false;
				this.anim.Play("officerwalk3");
				this.currentlyWaiting = false;
			}
			if (this.playerCurrency.drunk > 2 && !this.trafficStop && !this.playerCuffed && !this.driving)
			{
				if (this.fps.parent != null && this.distanceToPlayer < 12f && !this.chasing)
				{
					this.anim.Play("officerwalk3");
					if (Time.time >= this.lastPlayTime + 20f)
					{
						this.CopAudio("stop1");
					}
					this.charge_dui = 1;
					this.lastPlayTime = Time.time;
					this.criminalActivity = true;
					this.chasing = true;
					this.DetermineDestination();
					if (this.waitAtStationCoroutine != null)
					{
						base.StopCoroutine(this.waitAtStationCoroutine);
						this.waitAtStationCoroutine = null;
					}
					this.nav.enabled = true;
					this.nav.isStopped = false;
					this.currentlyWaiting = false;
				}
				if (this.chasing)
				{
					this.DetermineDestination();
				}
			}
			if (this.distanceToPlayer < 2f && this.currentlyWaiting && Time.time >= this.lastPlayTime + 20f && !this.trafficStop && !this.driving && !this.playerCuffed)
			{
				this.CopAudio("getLost1");
				this.lastPlayTime = Time.time;
			}
		}
		if (!this.chasing && this.nav.speed > 3f)
		{
			this.nav.speed = 2f;
		}
		if (this.chasing && this.destination == this.fps && !this.driving)
		{
			if (this.distanceToPlayer < 1.2f)
			{
				this.anim.Stop("officerwalk3");
				this.anim.Play("officerwait");
				this.nav.speed = 3f;
			}
			else if (this.distanceToPlayer < 3f)
			{
				this.anim.Play("officerwalk3");
				this.nav.speed = 3f;
			}
			else
			{
				this.anim.Play("officerrun");
				this.nav.speed = 7f;
			}
		}
		if (this.chasing && !this.driving && this.distanceToPlayer < 1.1f && !this.playerCuffed && !this.isRagdoll)
		{
			Debug.Log("startroutingxuff");
			if (this.handCuffCoroutine == null)
			{
				this.handCuffCoroutine = base.StartCoroutine(this.HandCuff());
			}
		}
		if (!this.chasing && this.destination == this.entrance && !this.driving && !this.isRagdoll)
		{
			if (!this.playerCuffed && !this.anim.IsPlaying("officerwalk3"))
			{
				this.anim.Play("officerwalk3");
			}
			this.speed = 2f;
		}
		if (this.beat)
		{
			bool flag2 = this.driving;
		}
		if ((this.criminalActivity || this.warrant) && this.distanceToPlayer < 25f && this.driving && !this.flashersOn && !this.playerCuffed)
		{
			this.patrolCar.Pursue(true);
		}
		if (this.destination == null)
		{
			return;
		}
		if (!this.driving && this.nav.enabled)
		{
			this.nav.destination = this.destination.position;
		}
		if (this.destination == this.idleStation && Vector3.Distance(base.transform.position, this.idleStation.position) < 1.5f)
		{
			this.nav.isStopped = true;
			this.destination = null;
			this.nav.Warp(this.idleStation.position);
			this.nav.updateRotation = false;
			base.transform.rotation = this.idleStation.rotation;
			this.policeUnit.transform.position = this.suvLocation.position;
			this.policeUnit.transform.rotation = this.suvLocation.rotation;
			this.idle = true;
			base.GetComponent<InteractiveObject>().enabled = true;
			this.nav.updateRotation = true;
			this.anim.Play("officerwait");
			if (this.waitAtStationCoroutine == null)
			{
				this.waitAtStationCoroutine = base.StartCoroutine(this.WaitAtStation());
				Debug.Log("waitatstation1");
			}
		}
		if (this.destination == this.idleGas && Vector3.Distance(base.transform.position, this.idleGas.position) < 1.5f)
		{
			this.nav.isStopped = true;
			this.nav.updateRotation = false;
			this.destination = null;
			this.nav.Warp(this.idleGas.position);
			base.transform.rotation = this.idleGas.rotation;
			this.idle = true;
			this.nav.updateRotation = true;
			this.anim.Play("officerwait");
			if (this.waitAtStationCoroutine == null)
			{
				this.waitAtStationCoroutine = base.StartCoroutine(this.WaitAtStation());
			}
		}
		if (this.destination == this.seatLoc2 && Vector3.Distance(base.transform.position, this.seatLoc2.position) < 1.1f && this.processCoroutine == null)
		{
			this.processCoroutine = base.StartCoroutine(this.Process());
		}
		if (this.destination == this.jail10 && Vector3.Distance(base.transform.position, this.jail10.position) < 1.1f)
		{
			if (!this.jailed)
			{
				if (this.openCellCoroutine == null)
				{
					Debug.Log("opencell");
					this.openCellCoroutine = base.StartCoroutine(this.OpenCell());
				}
			}
			else if (this.releaseCoroutine == null)
			{
				Debug.Log("releasing");
				this.releaseCoroutine = base.StartCoroutine(this.Release());
			}
		}
		if (this.destination == this.jail9 && Vector3.Distance(base.transform.position, this.jail9.position) < 1.1f)
		{
			this.destination = this.jail10;
		}
		if (this.destination == this.jail8 && Vector3.Distance(base.transform.position, this.jail8.position) < 1.1f && this.recuffCoroutine == null)
		{
			this.recuffCoroutine = base.StartCoroutine(this.ReCuff());
		}
		if (this.destination == this.jail7 && Vector3.Distance(base.transform.position, this.jail7.position) < 1.1f)
		{
			if (this.door2.isOpen)
			{
				this.door2.PerformAction();
			}
			if (this.computerCoroutine == null)
			{
				this.computerCoroutine = base.StartCoroutine(this.Computer());
			}
		}
		if (this.destination == this.jail6 && Vector3.Distance(base.transform.position, this.jail6.position) < 1.1f)
		{
			this.destination = this.jail7;
		}
		if (this.destination == this.jail5 && Vector3.Distance(base.transform.position, this.jail5.position) < 1.1f)
		{
			if (!this.door2.isOpen)
			{
				this.door2.enabled = true;
				this.door2.PerformAction();
				this.MiscAudio("jail_dooropen");
			}
			if (this.door1.isOpen)
			{
				this.door1.PerformAction();
				Debug.Log("opening");
			}
			if (this.officerWaitCoroutine == null)
			{
				this.officerWaitCoroutine = base.StartCoroutine(this.OfficerWait());
			}
			this.destination = this.jail6;
		}
		if (this.destination == this.jail4 && Vector3.Distance(base.transform.position, this.jail4.position) < 1.1f)
		{
			this.MiscAudio("jail_doorclose");
			this.destination = this.jail5;
		}
		if (this.destination == this.jail3 && Vector3.Distance(base.transform.position, this.jail3.position) < 1.1f)
		{
			this.MiscAudio("jail_doorbuzz");
			if (this.officerWaitCoroutine == null)
			{
				this.officerWaitCoroutine = base.StartCoroutine(this.OfficerWait());
			}
			if (!this.door1.isOpen)
			{
				this.door1.enabled = true;
				this.door1.PerformAction();
			}
			if (this.officerWaitCoroutine == null)
			{
				this.officerWaitCoroutine = base.StartCoroutine(this.OfficerWait());
			}
			this.destination = this.jail4;
		}
		if (this.destination == this.jail2 && Vector3.Distance(base.transform.position, this.jail2.position) < 1.1f)
		{
			this.destination = this.jail3;
		}
		if (this.destination == this.jail1 && Vector3.Distance(base.transform.position, this.jail1.position) < 1.1f)
		{
			this.destination = this.jail2;
		}
		if (this.destination == this.jail0 && Vector3.Distance(base.transform.position, this.jail0.position) < 1.1f)
		{
			this.destination = this.jail1;
		}
		if (this.destination == this.jailIdle && Vector3.Distance(base.transform.position, this.jailIdle.position) < 1.1f)
		{
			this.currentlyWaiting = true;
			this.policeUnit.transform.position = this.suvLocation.position;
			this.policeUnit.transform.rotation = this.suvLocation.rotation;
			this.anim.Stop("officerwalk3");
			this.anim.Play("officerwait");
		}
		if (this.jailed)
		{
			if (this.jailInterval == 0f)
			{
				this.jailInterval = (float)(this.playerCurrency.drunk * 18);
				if (this.jailInterval < 120f)
				{
					this.jailInterval = 120f;
				}
			}
			this.jailTimer += Time.deltaTime;
			if (this.jailTimer >= this.jailInterval && this.destination != this.jail10)
			{
				this.jailBed.enabled = false;
				this.anim.Play("officerwalk3");
				this.currentlyWaiting = false;
				this.nav.enabled = true;
				this.nav.isStopped = false;
				this.nav.updateRotation = true;
				this.destination = this.jail10;
				if (this.waitAtStationCoroutine != null)
				{
					base.StopCoroutine(this.waitAtStationCoroutine);
				}
				this.waitAtStationCoroutine = null;
				this.jailTimer = 0f;
			}
		}
		if (this.rb != null && (!(this.destination == null) || !this.rb.isKinematic) && this.destination != null && !this.rb.isKinematic)
		{
			this.rb.isKinematic = true;
		}
	}

	// Token: 0x060006D8 RID: 1752 RVA: 0x00059AE8 File Offset: 0x00057CE8
	public void ReceiveCrash()
	{
		if (this.driving)
		{
			float num = 0f;
			if (this.person.transform.parent.name == "DriverCameraController")
			{
				num = this.diamondback.speed;
			}
			else if (this.person.transform.parent.name == "DriverCameraControllerCar")
			{
				num = this.amc.speed;
			}
			else if (this.person.transform.parent.name == "f100seatmount")
			{
				num = this.f100.speed;
			}
			else if (this.person.transform.parent.name == "SeatMount")
			{
				num = 5f;
			}
			else if (this.person.transform.parent.name == "seatpos")
			{
				num = 5f;
			}
			if (num > 4f)
			{
				this.patrolCar.Pursue(true);
				this.patrolCar.ToggleLights(true);
				this.VehAudio("yelp2");
				this.charge_reckless++;
			}
		}
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x00059C20 File Offset: 0x00057E20
	private void LightUp()
	{
		this.flashersOn = true;
		this.flashers.SetActive(true);
		int num = Random.Range(0, 3);
		if (this.playerCurrency.nicotine < 3f)
		{
			this.playerCurrency.stress = 0f;
		}
		if (num == 0)
		{
			this.VehAudio("yelp1");
			return;
		}
		if (num == 1)
		{
			this.VehAudio("yelp2");
			return;
		}
		this.VehAudio("yelp3");
	}

	// Token: 0x060006DA RID: 1754 RVA: 0x00002188 File Offset: 0x00000388
	private void DetectPlayer()
	{
	}

	// Token: 0x060006DB RID: 1755 RVA: 0x00059C94 File Offset: 0x00057E94
	private IEnumerator HandCuff()
	{
		if (this.playerCuffed)
		{
			this.handCuffCoroutine = null;
			yield return null;
		}
		this.numArrests++;
		this.anim.Play("officeropen");
		this.patrolCar.ToggleSiren(false);
		if (this.person.transform.parent != null)
		{
			if (this.person.transform.parent.name == "DriverCameraController")
			{
				this.diamondback.turnOffAcc();
				this.diamondback.controlled = false;
				this.diamondback.userControlled = false;
				this.interactor.keyState = 0;
				this.vehicleToImpound = 1;
			}
			else if (this.person.transform.parent.name == "DriverCameraControllerCar")
			{
				this.amc.turnOffAcc();
				this.amc.controlled = false;
				this.amc.userControlled = false;
				this.interactor.keyStateC = 0;
				this.vehicleToImpound = 3;
			}
			else if (this.person.transform.parent.name == "f100seatmount")
			{
				this.f100.turnOffAcc();
				this.f100.controlled = false;
				this.f100.userControlled = false;
				this.interactor.keyStateF = 0;
				this.vehicleToImpound = 2;
			}
			else if (this.person.transform.parent.name == "SeatMount")
			{
				this.dirtbike.ShutOff();
				this.dirtbike.Stand();
				this.vehicleToImpound = 4;
			}
			else if (this.person.transform.parent.name == "seatpos")
			{
				this.vehicleToImpound = 5;
				this.golfCart.controlled = false;
				this.golfCart.userControlled = false;
			}
			this.interactor.drivingCar = false;
		}
		if (this.playerCurrency.nicotine < 3f)
		{
			this.playerCurrency.stress = 0f;
		}
		if (this.interactor.inventoryCanvas.activeSelf)
		{
			Cursor.visible = false;
			this.interactor.fpc.LockMouse();
			this.interactor.inventoryCanvas.SetActive(false);
		}
		this.interactor.SwitchItem();
		this.interactor.handcuffed = true;
		this.interactor.arrested = true;
		this.delinquentDays = 0;
		this.playerCuffed = true;
		this.destination = null;
		this.patrolCar.Pursue(false);
		this.fps.gameObject.GetComponent<Rigidbody>().useGravity = false;
		this.fpc.canMove = false;
		this.person.transform.position = this.holdLoc.position;
		this.person.transform.parent = this.holdLoc;
		this.person.layer = 9;
		this.chasing = false;
		this.warrant = false;
		this.raidWarrant = false;
		this.noStepOut = false;
		this.trafficStop = false;
		this.CopAudio("handcuff");
		yield return new WaitForSeconds(1f);
		int num = Random.Range(0, 2);
		if (this.charge_dui > 0)
		{
			this.CopAudio("underTheInfluence2");
		}
		else if (num == 0 && this.numArrests > 1)
		{
			this.CopAudio("howmanytimesarrest");
		}
		else
		{
			this.CopAudio("arrest2");
		}
		this.anim.Play("officerwalk3");
		if (Vector3.Distance(base.transform.position, this.jail0.position) < 50f)
		{
			this.destination = this.jail0;
			this.cuffTimerActive = true;
			this.gate.Open();
			this.flashersOn = false;
			this.flashers.SetActive(false);
			this.patrolCar.ToggleSiren(false);
		}
		else
		{
			this.destination = this.entrance;
			this.cuffTimerActive = true;
		}
		if (this.interactor.pickedUpObject != null)
		{
			this.interactor.ThrowObject();
		}
		this.handCuffCoroutine = null;
		yield break;
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x00059CA3 File Offset: 0x00057EA3
	public void OfficerExitCoroutine()
	{
		if (this.officerExitCoroutine == null && this.officerEnterCoroutine == null)
		{
			this.officerExitCoroutine = base.StartCoroutine(this.OfficerExit());
		}
	}

	// Token: 0x060006DD RID: 1757 RVA: 0x00059CC7 File Offset: 0x00057EC7
	public IEnumerator OfficerExit()
	{
		this.driving = false;
		this.aicc.enabled = false;
		this.patrolCar.Pursue(false);
		this.patrolCar.ToggleSiren(false);
		Debug.Log("offexit");
		this.destination = null;
		this.patrolCar.ToggleAi(false);
		yield return new WaitForSeconds(2f);
		base.transform.parent = null;
		base.gameObject.AddComponent<Rigidbody>();
		this.rb = base.GetComponent<Rigidbody>();
		this.rb.useGravity = false;
		this.rb.isKinematic = true;
		this.VehAudioLow("turnoff");
		this.patrolCar.ToggleEngineSound(true);
		this.carAnim.Play("open");
		yield return new WaitForSeconds(1f);
		Vector3 position = this.doorExitPosD.position;
		bool altExit = false;
		Vector3 position2 = this.entrance.transform.position;
		if (!this.CanExit(position))
		{
			position2 = this.doorExitPosP.transform.position;
			altExit = true;
		}
		this.anim.Play("officerexit");
		this.iTweenArgs = iTween.Hash(Array.Empty<object>());
		this.iTweenArgs.Add("position", position2);
		this.iTweenArgs.Add("time", 1);
		this.iTweenArgs.Add("islocal", false);
		iTween.MoveTo(base.gameObject, this.iTweenArgs);
		yield return new WaitForSeconds(1f);
		if (!altExit)
		{
			this.nav.Warp(this.entrance.transform.position);
		}
		else
		{
			this.nav.Warp(this.doorExitPosP.transform.position);
		}
		this.rb.useGravity = true;
		this.rb.isKinematic = true;
		this.rb.constraints = (RigidbodyConstraints)80;
		base.gameObject.layer = 0;
		if (this.chasing)
		{
			this.destination = this.fps;
		}
		if (this.idleAtStation)
		{
			this.destination = this.idleStation;
		}
		if (this.idleAtGasStation)
		{
			this.destination = this.idleGas;
		}
		if (this.trafficStop)
		{
			if (this.fps.parent != null)
			{
				this.noStepOut = true;
				if (this.fps.parent.name == "DriverCameraController")
				{
					this.destination = this.diamondbackEntry;
					this.chasing = false;
				}
				else if (this.fps.parent.name == "f100seatmount")
				{
					this.destination = this.f100Entry;
					this.chasing = false;
				}
				else if (this.fps.parent.name == "DriverCameraControllerCar")
				{
					this.destination = this.amcEntry;
					this.chasing = false;
				}
				else if (this.fps.parent.name == "seatpos")
				{
					this.destination = this.golfcartEntry;
					this.chasing = false;
				}
				else if (this.fps.parent.name == "SeatMount")
				{
					this.destination = this.dirtbikeEntry;
					this.chasing = false;
				}
			}
			if (this.fps.parent == null && this.waitForComplianceCoroutine == null)
			{
				this.CopAudio("backInVehicleAlt");
				this.waitForComplianceCoroutine = base.StartCoroutine(this.WaitForCompliance());
			}
		}
		if (this.playerCuffed)
		{
			this.gate.Close();
			this.nav.updateRotation = false;
			base.transform.rotation = this.entrance.rotation;
			this.nav.updateRotation = true;
			this.VehAudioLow("gate");
			this.carAnim.Play("cageopen");
			yield return new WaitForSeconds(1f);
			this.iTweenArgs = iTween.Hash(Array.Empty<object>());
			this.iTweenArgs.Add("position", this.holdLoc.position);
			this.iTweenArgs.Add("time", 1);
			this.iTweenArgs.Add("islocal", false);
			iTween.MoveTo(this.person, this.iTweenArgs);
			this.person.transform.parent = this.holdLoc;
			yield return new WaitForSeconds(1f);
			this.person.transform.position = this.holdLoc.position;
			this.destination = this.jail0;
			this.carAnim.Play("cageclose");
		}
		this.anim.Play("officerwalk3");
		this.nav.enabled = true;
		this.nav.isStopped = false;
		this.idleAtStation = false;
		this.idleAtGasStation = false;
		this.officerExitCoroutine = null;
		yield break;
	}

	// Token: 0x060006DE RID: 1758 RVA: 0x00059CD8 File Offset: 0x00057ED8
	private bool CanExit(Vector3 doorExitPosition)
	{
		Collider[] array = Physics.OverlapSphere(doorExitPosition, 0.44f, LayerMask.GetMask(new string[]
		{
			"Default"
		}));
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].isTrigger)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060006DF RID: 1759 RVA: 0x00059D1F File Offset: 0x00057F1F
	private IEnumerator OfficerEnter()
	{
		this.destination = null;
		this.aicc.enabled = true;
		this.patrolCar.ToggleSiren(false);
		this.flashersOn = false;
		this.flashers.SetActive(false);
		Debug.Log("enterveh");
		this.anim.Stop("officerwalk3");
		this.nav.enabled = false;
		base.transform.SetParent(this.seatpos1);
		base.GetComponent<InteractiveObject>().enabled = false;
		this.anim.Play("officeropen");
		this.carAnim.Play("open");
		this.driving = true;
		yield return new WaitForSeconds(1f);
		if (this.playerCuffed)
		{
			this.cuffTimerActive = false;
			this.cuffTimer = 0f;
			this.chasing = false;
			this.flashersOn = false;
			this.flashers.SetActive(false);
			this.VehAudioLow("gate");
			this.carAnim.Play("cageopen");
			this.CopAudio("getIn");
			yield return new WaitForSeconds(1f);
			if (this.interactor.pickedUpObject != null)
			{
				this.interactor.ThrowObject();
			}
			this.person.transform.parent = this.prisonerPos1;
			this.iTweenArgs = iTween.Hash(Array.Empty<object>());
			this.iTweenArgs.Add("position", this.prisonerPos1.transform.position);
			this.iTweenArgs.Add("time", 1);
			this.iTweenArgs.Add("islocal", false);
			iTween.MoveTo(this.person, this.iTweenArgs);
			this.person.transform.position = this.prisonerPos1.position;
			yield return new WaitForSeconds(1f);
			this.carAnim.Play("cageclose");
			this.VehAudioLow("gate");
			this.patrolCar.carAi2.turnTorque = 350f;
			this.patrolCar.carAi2.suspensionDamper = 9000f;
			yield return new WaitForSeconds(1f);
		}
		if (!this.playerCuffed && this.wpt.circuit == this.wpt.exitCircuit)
		{
			this.gate.Open();
		}
		this.driving = true;
		base.transform.rotation = this.seatpos1.rotation;
		base.GetComponent<Rigidbody>().useGravity = false;
		base.GetComponent<Rigidbody>().isKinematic = true;
		base.gameObject.layer = 9;
		this.anim.Play("officerenter");
		this.iTweenArgs = iTween.Hash(Array.Empty<object>());
		this.iTweenArgs.Add("position", this.seatpos1.transform.position);
		this.iTweenArgs.Add("time", 1);
		this.iTweenArgs.Add("islocal", false);
		iTween.MoveTo(base.gameObject, this.iTweenArgs);
		yield return new WaitForSeconds(1.1f);
		this.VehAudioLow("startup");
		this.patrolCar.ResumeOrRecalc();
		Object.Destroy(base.GetComponent<Rigidbody>());
		this.carAnim.Play("close");
		yield return new WaitForSeconds(1.1f);
		this.patrolCar.ToggleEngineSound(false);
		base.transform.position = this.seatpos1.position;
		this.patrolCar.ToggleAi(true);
		if (!this.chasing)
		{
			this.patrolCar.ToggleLights(false);
			this.patrolCar.ToggleSiren(false);
			this.trafficStop = false;
		}
		this.officerEnterCoroutine = null;
		yield break;
	}

	// Token: 0x060006E0 RID: 1760 RVA: 0x00059D30 File Offset: 0x00057F30
	private void EscalateTrafficStop()
	{
		if (this.fps.parent != null)
		{
			base.StopCoroutine(this.initiateStopCoroutine);
			this.chasing = true;
			this.currentlyWaiting = false;
			this.anim.Play("officerwalk3");
			this.criminalActivity = true;
			this.destination = this.fps;
			this.nav.updateRotation = true;
			this.nav.enabled = true;
			this.nav.isStopped = false;
			this.initiateStopCoroutine = null;
			this.noStepOut = false;
		}
	}

	// Token: 0x060006E1 RID: 1761 RVA: 0x00059DC0 File Offset: 0x00057FC0
	private IEnumerator InitiateStop()
	{
		Debug.Log("initiatingstop");
		this.trafficStop = true;
		this.noStepOut = true;
		this.patrolCar.ToggleSiren(false);
		this.nav.updateRotation = false;
		if (this.fps.parent == null)
		{
			this.nav.Warp(this.fps.position);
		}
		else if (this.fps.parent.name == "DriverCameraController")
		{
			this.driverGlass.SetActive(false);
			this.nav.Warp(this.diamondbackEntry.position);
		}
		else if (this.fps.parent.name == "f100seatmount")
		{
			this.nav.Warp(this.f100Entry.position);
		}
		else if (this.fps.parent.name == "DriverCameraControllerCar")
		{
			this.nav.Warp(this.amcEntry.position);
		}
		else if (this.fps.parent.name == "seatpos")
		{
			this.nav.Warp(this.golfcartEntry.position);
		}
		else if (this.fps.parent.name == "SeatMount")
		{
			this.nav.Warp(this.dirtbikeEntry.position);
		}
		this.anim.Stop("officerwalk3");
		this.anim.Play("trafficstop");
		this.currentlyWaiting = true;
		base.transform.LookAt(this.person.transform);
		if (this.charge_theft > 0)
		{
			this.CopAudio("payfuel");
			yield return new WaitForSeconds(7f);
		}
		if (this.distanceToPlayer > 5f)
		{
			this.EscalateTrafficStop();
		}
		if ((this.fps.parent.name == "f100seatmount" && this.acF100.rotorCount > 0) || (this.fps.parent.name == "DriverCameraController" && this.acDiamondback.rotorCount > 0))
		{
			this.CopAudio("thisisillegal");
			this.charge_reckless = 1;
			yield return new WaitForSeconds(5f);
		}
		if (this.charge_reckless > 0)
		{
			this.CopAudio("reckless");
			yield return new WaitForSeconds(3f);
		}
		if (this.distanceToPlayer > 5f)
		{
			this.EscalateTrafficStop();
		}
		if (this.CheckForMoonshine())
		{
			this.charge_posession++;
			this.CopAudio("moonshine1");
			yield return new WaitForSeconds(7f);
		}
		if (this.distanceToPlayer > 5f)
		{
			this.EscalateTrafficStop();
		}
		if (this.CheckForBeerCan())
		{
			this.charge_opencontainer++;
			if (this.charge_opencontainer > 1)
			{
				this.charge_opencontainer = 1;
			}
			this.CopAudio("beercan");
			this.subtractContainer = 0;
			yield return new WaitForSeconds(3f);
		}
		if (this.distanceToPlayer > 5f)
		{
			this.EscalateTrafficStop();
		}
		if (this.charge_headlights > 0)
		{
			this.CopAudio("noheadlights");
			yield return new WaitForSeconds(5f);
		}
		if (this.charge_speeding > 0)
		{
			this.CopAudio("speeding");
			yield return new WaitForSeconds(2f);
		}
		if (this.charge_hitandrun > 0)
		{
			this.CopAudio("ranmeover");
			yield return new WaitForSeconds(6f);
		}
		if (this.distanceToPlayer > 5f)
		{
			this.EscalateTrafficStop();
		}
		bool warning = false;
		int num = Random.Range(0, 15);
		if (this.numWarnings > 1)
		{
			num = 0;
		}
		if (num > 5)
		{
			warning = true;
			this.numWarnings++;
		}
		if (this.playerCurrency.drunk > 2)
		{
			this.CopAudio("smellAlcohol1");
			yield return new WaitForSeconds(7f);
			this.criminalActivity = true;
			this.charge_dui = 1;
		}
		if (this.distanceToPlayer > 5f)
		{
			this.EscalateTrafficStop();
		}
		if (Random.Range(0, 2) == 0 || warning)
		{
			this.CopAudio("heresTheDealAlt");
			yield return new WaitForSeconds(3.5f);
		}
		if (this.criminalActivity)
		{
			this.CopAudio("stepOut1");
			this.noStepOut = false;
			yield return new WaitForSeconds(3f);
			this.criminalActivity = true;
			while (this.interactor.inoutVehicle)
			{
				yield return new WaitForSeconds(1f);
			}
			if (this.handCuffCoroutine == null)
			{
				base.transform.LookAt(this.person.transform);
				this.handCuffCoroutine = base.StartCoroutine(this.HandCuff());
			}
		}
		if (!this.criminalActivity && !this.warrant)
		{
			this.noStepOut = false;
			if (!warning)
			{
				this.CopAudio("thisIsYours");
				this.nav.updateRotation = false;
				base.transform.LookAt(this.person.transform);
				this.thisTicket = Object.Instantiate<GameObject>(this.ticketObj, this.ticketPos.position, this.ticketPos.rotation);
				this.thisTicket.transform.parent = this.ticketPos;
				this.thisTicket.GetComponent<Rigidbody>().isKinematic = true;
				this.cit = this.thisTicket.GetComponent<Citation>();
				this.cit.charge_dui = this.charge_dui;
				this.cit.charge_evasion = this.charge_evasion;
				this.cit.charge_posession = this.charge_posession;
				this.cit.charge_speeding = this.charge_speeding;
				this.cit.charge_opencontainer = this.charge_opencontainer;
				this.cit.charge_reckless = this.charge_reckless;
				this.cit.charge_hitandrun = this.charge_hitandrun;
				this.cit.charge_headlights = this.charge_headlights;
				this.cit.charge_theft = this.charge_theft;
				this.cit.charge_obstruction = this.charge_obstruction;
				this.cit.month = EnviroSkyMgr.instance.GetCurrentMonth();
				this.cit.day = EnviroSkyMgr.instance.GetCurrentDay();
				this.cit.hour = EnviroSkyMgr.instance.GetTimeString();
				this.activeCitations++;
				this.nav.updateRotation = true;
				this.thisTicket.GetComponent<BoxCollider>().enabled = false;
				this.anim.Play("ticket");
				yield return new WaitForSeconds(2f);
				this.thisTicket.GetComponent<BoxCollider>().enabled = true;
				yield return new WaitForSeconds(3f);
				if (this.thisTicket != null && this.thisTicket.transform.parent == this.ticketPos)
				{
					this.thisTicket.transform.parent = null;
					this.thisTicket.GetComponent<Rigidbody>().isKinematic = false;
					this.thisTicket.transform.position = this.fpsHoldLoc.position;
				}
			}
			else
			{
				if (Random.Range(0, 2) == 0)
				{
					this.CopAudio("warning1");
				}
				else
				{
					this.CopAudio("anothercall");
				}
				yield return new WaitForSeconds(3f);
			}
			this.ClearTempCharges();
			this.cooldown = Time.time + 60f;
			this.destination = this.entrance;
			this.nav.updateRotation = true;
			yield return new WaitForSeconds(2f);
			int num2 = Random.Range(0, 6);
			if (num2 == 0)
			{
				this.CopAudio("luckynotimpounding1");
			}
			else if (num2 == 1)
			{
				this.CopAudio("luckynotimpounding2");
			}
			yield return new WaitForSeconds(1f);
		}
		else
		{
			if (this.warrant)
			{
				this.CopAudio("warrant1");
				this.patrolCar.ToggleSiren(false);
				yield return new WaitForSeconds(3f);
				if (this.handCuffCoroutine == null)
				{
					this.handCuffCoroutine = base.StartCoroutine(this.HandCuff());
				}
			}
			this.destination = this.fps;
			this.chasing = true;
			this.patrolCar.ToggleSiren(false);
		}
		this.nav.updateRotation = true;
		this.nav.enabled = true;
		this.nav.isStopped = false;
		this.driverGlass.SetActive(true);
		this.currentlyWaiting = false;
		this.initiateStopCoroutine = null;
		yield break;
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x00059DD0 File Offset: 0x00057FD0
	private bool CheckForBeerCan()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, 2f);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].gameObject.name.Contains("beercan"))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x00059E20 File Offset: 0x00058020
	private bool CheckForMoonshine()
	{
		float num = Vector3.Distance(base.transform.position, this.bucket1.transform.position);
		float num2 = Vector3.Distance(base.transform.position, this.bucket2.transform.position);
		float num3 = Vector3.Distance(base.transform.position, this.bucket3.transform.position);
		return (num < 5f || num2 < 5f || num3 < 5f) && (this.bucket1Script.alcohol > 0 || this.bucket2Script.alcohol > 0 || this.bucket3Script.alcohol > 0);
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x00059ED4 File Offset: 0x000580D4
	public void VehAudio(string soundName)
	{
		AudioClip clip;
		if (this.vehSoundDictionary.TryGetValue(soundName, out clip))
		{
			this.aSource2.clip = clip;
			this.aSource2.Play();
		}
	}

	// Token: 0x060006E5 RID: 1765 RVA: 0x00059F08 File Offset: 0x00058108
	private void VehAudioLow(string soundName)
	{
		AudioClip clip;
		if (this.vehLowSoundDictionary.TryGetValue(soundName, out clip))
		{
			this.aSource3.clip = clip;
			this.aSource3.Play();
		}
	}

	// Token: 0x060006E6 RID: 1766 RVA: 0x00059F3C File Offset: 0x0005813C
	private void CopAudio(string soundName)
	{
		AudioClip clip;
		if (this.copSoundDictionary.TryGetValue(soundName, out clip))
		{
			this.aSource.clip = clip;
			this.aSource.Play();
			return;
		}
		Debug.Log("noclip");
	}

	// Token: 0x060006E7 RID: 1767 RVA: 0x00059F7C File Offset: 0x0005817C
	private void MiscAudio(string soundName)
	{
		AudioClip clip;
		if (this.miscSoundDictionary.TryGetValue(soundName, out clip))
		{
			this.aSource4.clip = clip;
			this.aSource4.Play();
		}
	}

	// Token: 0x060006E8 RID: 1768 RVA: 0x00002188 File Offset: 0x00000388
	private void Beat()
	{
	}

	// Token: 0x060006E9 RID: 1769 RVA: 0x00059FB0 File Offset: 0x000581B0
	private IEnumerator WaitAtStation()
	{
		this.currentlyWaiting = true;
		int num = Random.Range(10, 30);
		if (this.interactor.diamondbackImpounded || this.interactor.f100Impounded || this.interactor.amcImpounded || this.interactor.dirtbikeImpounded || this.interactor.golfcartImpounded)
		{
			num = Random.Range(120, 240);
		}
		if (this.playerCuffed)
		{
			num = 5;
		}
		this.gate.Close();
		yield return new WaitForSeconds((float)num);
		this.idleAtStation = false;
		this.idleAtGasStation = false;
		base.GetComponent<InteractiveObject>().enabled = false;
		if (!this.isRagdoll)
		{
			this.nav.enabled = true;
			this.nav.isStopped = false;
			this.destination = this.entrance;
			if (Vector3.Distance(base.transform.position, this.idleStation.position) < 5f)
			{
				this.wpt.garageDoor.Open();
			}
			this.anim.Play("officerwalk3");
			this.currentlyWaiting = false;
			this.waitAtStationCoroutine = null;
		}
		yield break;
	}

	// Token: 0x060006EA RID: 1770 RVA: 0x00059FC0 File Offset: 0x000581C0
	private void CheckIfStuck()
	{
		this.positionSampleTimer += Time.deltaTime;
		if (this.positionSampleTimer >= this.positionSampleInterval)
		{
			this.positionSampleTimer = 0f;
			if (!this.driving && !this.chasing && this.destination != null && !this.currentlyWaiting)
			{
				if (Vector3.Distance(base.transform.position, this.lastPosition) < 0.1f)
				{
					this.stuckTimer += this.positionSampleInterval;
					if (this.stuckTimer > this.stuckCheckInterval)
					{
						this.stuckTimer = 0f;
						Debug.Log("stuck..trying to relocate");
						this.nav.isStopped = true;
						this.nav.updateRotation = false;
						this.nav.Warp(this.destination.position);
						this.nav.isStopped = false;
						this.nav.updateRotation = true;
					}
				}
				else
				{
					this.stuckTimer = 0f;
				}
			}
			this.lastPosition = base.transform.position;
		}
	}

	// Token: 0x060006EB RID: 1771 RVA: 0x0005A0E5 File Offset: 0x000582E5
	private IEnumerator OpenJail()
	{
		this.anim.Play("officeropen");
		yield return new WaitForSeconds(1f);
		this.anim.Play("officerwalk3");
		this.openJailCoroutine = null;
		yield break;
	}

	// Token: 0x060006EC RID: 1772 RVA: 0x0005A0F4 File Offset: 0x000582F4
	private IEnumerator OfficerWait()
	{
		this.anim.Stop("officerwalk3");
		this.nav.isStopped = true;
		yield return new WaitForSeconds(1.5f);
		this.anim.Play("officerwalk3");
		this.nav.isStopped = false;
		this.officerWaitCoroutine = null;
		yield break;
	}

	// Token: 0x060006ED RID: 1773 RVA: 0x0005A103 File Offset: 0x00058303
	private IEnumerator Computer()
	{
		this.currentlyWaiting = true;
		this.nav.isStopped = true;
		this.nav.updateRotation = false;
		this.person.transform.parent = this.benchLoc;
		this.iTweenArgs = iTween.Hash(Array.Empty<object>());
		this.iTweenArgs.Add("position", this.benchLoc.transform.position);
		this.iTweenArgs.Add("time", 1);
		this.iTweenArgs.Add("islocal", false);
		iTween.MoveTo(this.person, this.iTweenArgs);
		this.person.transform.position = this.benchLoc.position;
		base.transform.rotation = this.jail7.rotation;
		base.transform.position = this.jail7.position;
		this.anim.Play("officercomputer2");
		yield return new WaitForSeconds(1f);
		this.MiscAudio("policekeyboard");
		yield return new WaitForSeconds(10f);
		if (this.charge_hitandrun > 0)
		{
			this.CopAudio("ranmeover");
			yield return new WaitForSeconds(6f);
		}
		if (this.vehicleToImpound > 0 && Random.Range(0, 3) == 0)
		{
			this.CopAudio("impound1");
		}
		yield return new WaitForSeconds(10f);
		this.destination = this.jail8;
		this.nav.updateRotation = true;
		this.nav.isStopped = false;
		this.currentlyWaiting = false;
		this.computerCoroutine = null;
		yield break;
	}

	// Token: 0x060006EE RID: 1774 RVA: 0x0005A112 File Offset: 0x00058312
	private IEnumerator ReCuff()
	{
		this.nav.isStopped = true;
		this.nav.updateRotation = false;
		base.transform.rotation = this.jail8.rotation;
		this.person.transform.parent = this.holdLoc;
		this.iTweenArgs = iTween.Hash(Array.Empty<object>());
		this.iTweenArgs.Add("position", this.holdLoc.transform.position);
		this.iTweenArgs.Add("time", 1);
		this.iTweenArgs.Add("islocal", false);
		iTween.MoveTo(this.person, this.iTweenArgs);
		this.person.transform.position = this.holdLoc.position;
		this.person.transform.rotation = this.holdLoc.rotation;
		yield return new WaitForSeconds(1f);
		this.nav.updateRotation = true;
		this.anim.Play("officerwalk3");
		this.destination = this.jail9;
		this.nav.isStopped = false;
		this.recuffCoroutine = null;
		yield break;
	}

	// Token: 0x060006EF RID: 1775 RVA: 0x0005A121 File Offset: 0x00058321
	private IEnumerator Process()
	{
		this.currentlyWaiting = true;
		this.nav.isStopped = true;
		this.nav.updateRotation = false;
		base.transform.rotation = this.jail7.rotation;
		base.transform.position = this.seatLoc2.position;
		this.anim.Play("officercomputer2");
		this.MiscAudio("printer");
		this.lastPlayTime = Time.time;
		yield return new WaitForSeconds(7f);
		if (this.activeCitations > 0)
		{
			if (Random.Range(0, 2) == 0)
			{
				this.CopAudio("unpaidtickets");
			}
			else
			{
				this.CopAudio("unpaidtickets2");
			}
			yield return new WaitForSeconds(3f);
		}
		else
		{
			this.CopAudio("thisIsYours");
			this.lastPlayTime = Time.time;
		}
		if (!this.door2.isOpen)
		{
			this.door2.PerformAction();
		}
		this.thisTicket = Object.Instantiate<GameObject>(this.ticketObj, this.printLoc.position, this.printLoc.rotation);
		this.cit = this.thisTicket.GetComponent<Citation>();
		this.cit.charge_dui = this.charge_dui;
		this.cit.charge_evasion = this.charge_evasion;
		this.cit.charge_posession = this.charge_posession;
		this.cit.charge_speeding = this.charge_speeding;
		this.cit.charge_opencontainer = this.charge_opencontainer;
		this.cit.charge_reckless = this.charge_reckless;
		this.cit.charge_hitandrun = this.charge_hitandrun;
		this.cit.charge_headlights = this.charge_headlights;
		this.cit.charge_theft = this.charge_theft;
		this.cit.charge_obstruction = this.charge_obstruction;
		this.cit.charge_pubIntox = this.charge_pubIntox;
		this.cit.month = EnviroSkyMgr.instance.GetCurrentMonth();
		this.cit.day = EnviroSkyMgr.instance.GetCurrentDay();
		this.cit.hour = EnviroSkyMgr.instance.GetTimeString();
		this.activeCitations++;
		this.interactor.arrested = false;
		this.playerCuffed = false;
		this.jailed = false;
		this.EnableHouseDoors(true);
		if (!this.exitDoor.isOpen)
		{
			this.exitDoor.PerformAction();
		}
		yield return new WaitForSeconds(4f);
		int num = Random.Range(0, 6);
		if (this.charge_hitandrun > 0)
		{
			num = 0;
		}
		if (num == 0)
		{
			this.CopAudio("mytown");
			this.lastPlayTime = Time.time;
		}
		this.ClearTempCharges();
		yield return new WaitForSeconds(14f);
		this.destination = this.idleStation;
		this.anim.Play("officerwalk3");
		this.nav.updateRotation = true;
		this.currentlyWaiting = false;
		this.nav.isStopped = false;
		this.processCoroutine = null;
		yield break;
	}

	// Token: 0x060006F0 RID: 1776 RVA: 0x0005A130 File Offset: 0x00058330
	private IEnumerator OpenCell()
	{
		if (this.vehicleToImpound > 0)
		{
			this.phoneScript.Impound(this.vehicleToImpound);
			Achievement achievement = new Achievement("ACH_IMPOUND");
			achievement.Trigger(true);
		}
		this.jailed = true;
		this.nav.isStopped = true;
		this.destination = this.jailIdle;
		if (!this.cellDoor.isOpen)
		{
			this.cellDoor.enabled = true;
			this.cellDoor.PerformAction();
		}
		this.MiscAudio("jail_dooropen");
		yield return new WaitForSeconds(1.5f);
		this.person.layer = 2;
		this.person.transform.parent = null;
		this.person.transform.position = this.bookLoc.position;
		this.cellDoor.PerformAction();
		this.MiscAudio("jail_doorclose");
		yield return new WaitForSeconds(1.5f);
		this.cellDoor.enabled = false;
		this.fps.gameObject.GetComponent<Rigidbody>().useGravity = true;
		this.fpc.canMove = true;
		this.anim.Play("officerwalk3");
		Random.Range(0, 2);
		if (this.charge_dui + this.charge_evasion + this.charge_posession + this.charge_speeding + this.charge_opencontainer + this.charge_reckless + this.charge_hitandrun + this.charge_headlights + this.charge_theft + this.charge_obstruction > 2)
		{
			this.CopAudio("paperwork");
		}
		else
		{
			this.CopAudio("processed1");
		}
		this.nav.enabled = true;
		this.nav.isStopped = false;
		this.interactor.handcuffed = false;
		this.openCellCoroutine = null;
		this.vehicleToImpound = 0;
		this.criminalActivity = false;
		yield return new WaitForSeconds(30f);
		this.jailBed.enabled = true;
		Achievement achievement2 = new Achievement("ACH_BOOKED");
		achievement2.Trigger(true);
		yield break;
	}

	// Token: 0x060006F1 RID: 1777 RVA: 0x0005A140 File Offset: 0x00058340
	private void ClearTempCharges()
	{
		this.charge_dui = 0;
		this.charge_evasion = 0;
		this.charge_posession = 0;
		this.charge_speeding = 0;
		this.charge_opencontainer = 0;
		this.charge_reckless = 0;
		this.charge_hitandrun = 0;
		this.charge_headlights = 0;
		this.charge_theft = 0;
		this.charge_obstruction = 0;
		this.patrolCar.pursuing = false;
		this.criminalActivity = false;
		this.warrant = false;
		this.chasing = false;
	}

	// Token: 0x060006F2 RID: 1778 RVA: 0x0005A1B4 File Offset: 0x000583B4
	private IEnumerator Release()
	{
		this.nav.isStopped = true;
		this.nav.updateRotation = false;
		if (!this.cellDoor.isOpen)
		{
			this.cellDoor.enabled = true;
			this.cellDoor.PerformAction();
		}
		this.MiscAudio("jail_dooropen");
		this.anim.Stop("officerwalk3");
		this.anim.Play("officerwait");
		yield return new WaitForSeconds(1f);
		this.person.transform.parent = null;
		this.fpc.canMove = true;
		yield return new WaitForSeconds(1.5f);
		this.nav.updateRotation = true;
		this.nav.isStopped = false;
		this.anim.Play("officerwalk3");
		this.destination = this.seatLoc2;
		this.cooldown = Time.time + 60f;
		this.jailInterval = 0f;
		this.releaseCoroutine = null;
		yield break;
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x0005A1C3 File Offset: 0x000583C3
	private IEnumerator WaitForCompliance()
	{
		this.anim.Play("officerwait");
		if (this.nav.enabled)
		{
			this.nav.isStopped = true;
		}
		yield return new WaitForSeconds(6f);
		if (this.fps.parent == null)
		{
			this.trafficStop = false;
			this.destination = this.fps;
			this.chasing = true;
		}
		else
		{
			this.anim.Play("officerwalk3");
			this.nav.isStopped = false;
			if (this.fps.parent != null)
			{
				this.noStepOut = true;
				if (this.fps.parent.name == "DriverCameraController")
				{
					this.destination = this.diamondbackEntry;
					this.chasing = false;
				}
				else if (this.fps.parent.name == "f100seatmount")
				{
					this.destination = this.f100Entry;
					this.chasing = false;
				}
				else if (this.fps.parent.name == "DriverCameraControllerCar")
				{
					this.destination = this.amcEntry;
					this.chasing = false;
				}
				else if (this.fps.parent.name == "seatpos")
				{
					this.destination = this.golfcartEntry;
					this.chasing = false;
				}
				else if (this.fps.parent.name == "SeatMount")
				{
					this.destination = this.dirtbikeEntry;
					this.chasing = false;
				}
			}
		}
		this.waitForComplianceCoroutine = null;
		yield break;
	}

	// Token: 0x060006F4 RID: 1780 RVA: 0x0005A1D2 File Offset: 0x000583D2
	private IEnumerator PursueDelay()
	{
		yield return new WaitForSeconds(1.5f);
		this.patrolCar.Pursue(true);
		this.patrolCar.ToggleLights(true);
		this.VehAudio("yelp2");
		this.PursueDelayCoroutine = null;
		yield break;
	}

	// Token: 0x060006F5 RID: 1781 RVA: 0x0005A1E1 File Offset: 0x000583E1
	public void ResetDay()
	{
		base.StartCoroutine(this.ResetDayC());
	}

	// Token: 0x060006F6 RID: 1782 RVA: 0x0005A1F0 File Offset: 0x000583F0
	private IEnumerator ResetDayC()
	{
		this.numWarnings = 0;
		if (this.isRagdoll)
		{
			this.RagdollOff();
		}
		this.nav.isStopped = true;
		if (this.driving)
		{
			base.StartCoroutine(this.OfficerExit());
			yield return new WaitForSeconds(5f);
		}
		this.nav.Warp(this.respawn.position);
		this.nav.updateRotation = false;
		this.destination = this.idleStation;
		this.wheelJoints.SetActive(false);
		this.policeUnit.transform.position = this.suvLocation.position;
		this.policeUnit.transform.rotation = this.suvLocation.rotation;
		if (this.warrant | this.activeCitations > 0)
		{
			this.delinquentDays++;
			if (this.delinquentDays > 5)
			{
				this.raidWarrant = true;
			}
		}
		if (!this.warrant && this.activeCitations == 0)
		{
			this.delinquentDays = 0;
			this.raidWarrant = false;
		}
		this.nav.updateRotation = true;
		this.wheelJoints.SetActive(true);
		yield return new WaitForSeconds(1f);
		this.nav.isStopped = false;
		yield break;
	}

	// Token: 0x060006F7 RID: 1783 RVA: 0x0005A200 File Offset: 0x00058400
	public void Raid(int raidLoc)
	{
		if (this.driving)
		{
			this.delinquentDays = 0;
			this.driving = false;
			this.patrolCar.Pursue(false);
			this.patrolCar.ToggleAi(false);
			base.transform.parent = null;
			base.gameObject.AddComponent<Rigidbody>();
			this.rb = base.GetComponent<Rigidbody>();
			this.rb.useGravity = true;
			this.rb.isKinematic = true;
			this.rb.constraints = (RigidbodyConstraints)80;
			base.gameObject.layer = 0;
			this.nav.enabled = true;
			this.nav.isStopped = false;
			this.idleAtStation = false;
			this.idleAtGasStation = false;
			this.officerExitCoroutine = null;
			this.anim.Play("officerwait");
		}
		this.RagdollOff();
		if (raidLoc == 1)
		{
			this.EnableHouseDoors(false);
			this.nav.Warp(this.raidLoc1.position);
			this.nav.updateRotation = false;
			base.transform.rotation = this.raidLoc1.rotation;
			this.nav.updateRotation = true;
			this.policeUnit.transform.position = this.suvRaidLoc1.position;
			this.policeUnit.transform.rotation = this.suvRaidLoc1.rotation;
		}
		else
		{
			this.EnableHouseDoors(false);
			this.nav.Warp(this.raidLoc2.position);
			this.nav.updateRotation = false;
			base.transform.rotation = this.raidLoc2.rotation;
			this.nav.updateRotation = true;
			this.policeUnit.transform.position = this.suvRaidLoc2.position;
			this.policeUnit.transform.rotation = this.suvRaidLoc2.rotation;
		}
		this.raidWarrant = false;
		this.delinquentDays = 0;
		this.servingWarrant = true;
		this.criminalActivity = true;
		this.chasing = true;
		this.MiscAudio("raid");
		this.destination = this.fps;
	}

	// Token: 0x060006F8 RID: 1784 RVA: 0x0005A424 File Offset: 0x00058624
	private void EnableHouseDoors(bool tf)
	{
		this.deactivateDoors[0].SetActive(tf);
		this.deactivateDoors[1].SetActive(tf);
		this.deactivateDoors[2].SetActive(tf);
		this.deactivateDoors[3].SetActive(tf);
		this.deactivateDoors[4].SetActive(tf);
		this.deactivateDoors[5].SetActive(tf);
	}

	// Token: 0x060006F9 RID: 1785 RVA: 0x0005A488 File Offset: 0x00058688
	private void DetermineDestination()
	{
		if (this.person.transform.parent != null)
		{
			if (this.person.transform.parent.name == "DriverCameraController")
			{
				this.destination = this.diamondbackEntry;
				return;
			}
			if (this.person.transform.parent.name == "DriverCameraControllerCar")
			{
				this.destination = this.amcEntry;
				return;
			}
			if (this.person.transform.parent.name == "f100seatmount")
			{
				this.destination = this.f100Entry;
				return;
			}
			if (this.person.transform.parent.name == "SeatMount")
			{
				this.destination = this.dirtbikeEntry;
				return;
			}
			if (this.person.transform.parent.name == "seatpos")
			{
				this.destination = this.golfcartEntry;
				return;
			}
		}
		else
		{
			this.destination = this.fps;
		}
	}

	// Token: 0x060006FA RID: 1786 RVA: 0x0005A5A4 File Offset: 0x000587A4
	private void RagdollOff()
	{
		this.trafficStop = false;
		this.idleAtGasStation = false;
		this.idleAtStation = false;
		this.currentlyWaiting = false;
		this.noStepOut = false;
		if (base.transform.parent != null)
		{
			base.transform.parent = null;
		}
		Collider[] array = this.ragDollColliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		Rigidbody[] array2 = this.ragDollRigidbodies;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].isKinematic = true;
		}
		this.nav.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		base.transform.GetChild(0).rotation = Quaternion.Euler(0f, 0f, 0f);
		this.anim.enabled = true;
		this.thisCapsule.enabled = true;
		if (this.rb != null)
		{
			this.rb.isKinematic = true;
		}
		this.isRagdoll = false;
		this.nav.enabled = true;
	}

	// Token: 0x060006FB RID: 1787 RVA: 0x0005A6C0 File Offset: 0x000588C0
	public void RagdollOn()
	{
		bool flag = false;
		if (!this.playerCuffed && !this.driving)
		{
			if (this.fps.parent.name == "DriverCameraController")
			{
				if (this.diamondback.speed < 2f || !this.diamondback.controlled)
				{
					flag = true;
				}
			}
			else if (this.fps.parent.name == "DriverCameraControllerCar")
			{
				if (this.amc.speed < 2f || !this.amc.controlled)
				{
					flag = true;
				}
			}
			else if (this.fps.parent.name == "f100seatmount" && (this.f100.speed < 2f || !this.f100.controlled))
			{
				flag = true;
			}
			this.interactor.Heartbeat();
			this.anim.enabled = false;
			this.nav.enabled = false;
			Collider[] array = this.ragDollColliders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
			Rigidbody[] array2 = this.ragDollRigidbodies;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].isKinematic = false;
			}
			this.thisCapsule.enabled = false;
			this.rb.isKinematic = true;
			this.CopAudio("pain");
			if (!flag)
			{
				this.charge_hitandrun++;
				if (Random.Range(0, 2) == 0)
				{
					this.raidWarrant = true;
				}
				this.warrant = true;
			}
			this.isRagdoll = true;
		}
	}

	// Token: 0x04000F0D RID: 3853
	public Transform destination;

	// Token: 0x04000F0E RID: 3854
	public float speed = 5f;

	// Token: 0x04000F0F RID: 3855
	public float rotationSpeed = 10f;

	// Token: 0x04000F10 RID: 3856
	public bool chasing;

	// Token: 0x04000F11 RID: 3857
	public bool driving;

	// Token: 0x04000F12 RID: 3858
	public Transform entrance;

	// Token: 0x04000F13 RID: 3859
	public Transform fps;

	// Token: 0x04000F14 RID: 3860
	public Transform seatpos1;

	// Token: 0x04000F15 RID: 3861
	public Transform prisonerPos1;

	// Token: 0x04000F16 RID: 3862
	public GameObject person;

	// Token: 0x04000F17 RID: 3863
	public Animation anim;

	// Token: 0x04000F18 RID: 3864
	public Animation carAnim;

	// Token: 0x04000F19 RID: 3865
	public Animation jailAnim;

	// Token: 0x04000F1A RID: 3866
	public int crimeLevel;

	// Token: 0x04000F1B RID: 3867
	public float distanceToPlayer;

	// Token: 0x04000F1C RID: 3868
	public float distanceToUnit;

	// Token: 0x04000F1D RID: 3869
	public GameObject policeUnit;

	// Token: 0x04000F1E RID: 3870
	[SerializeField]
	private float fovDistance = 30f;

	// Token: 0x04000F1F RID: 3871
	public FirstPersonController fpc;

	// Token: 0x04000F20 RID: 3872
	public bool obstacleHitBool;

	// Token: 0x04000F21 RID: 3873
	private Vector3 origin;

	// Token: 0x04000F22 RID: 3874
	private Vector3 dest;

	// Token: 0x04000F23 RID: 3875
	private Vector3 direction;

	// Token: 0x04000F24 RID: 3876
	private NavMeshAgent nav;

	// Token: 0x04000F25 RID: 3877
	public AudioSource aSource;

	// Token: 0x04000F26 RID: 3878
	public AudioSource aSource2;

	// Token: 0x04000F27 RID: 3879
	public AudioSource aSource3;

	// Token: 0x04000F28 RID: 3880
	public AudioSource aSource4;

	// Token: 0x04000F29 RID: 3881
	public AudioSource radio;

	// Token: 0x04000F2A RID: 3882
	public AudioClip[] vehSounds;

	// Token: 0x04000F2B RID: 3883
	public AudioClip[] vehSoundsLow;

	// Token: 0x04000F2C RID: 3884
	public AudioClip[] dialogue;

	// Token: 0x04000F2D RID: 3885
	public AudioClip[] miscSounds;

	// Token: 0x04000F2E RID: 3886
	private bool idle;

	// Token: 0x04000F2F RID: 3887
	private bool beat;

	// Token: 0x04000F30 RID: 3888
	public bool jailed;

	// Token: 0x04000F31 RID: 3889
	public float jailTimer;

	// Token: 0x04000F32 RID: 3890
	public float jailInterval;

	// Token: 0x04000F33 RID: 3891
	public PatrolCar patrolCar;

	// Token: 0x04000F34 RID: 3892
	private Vector3 enterposition;

	// Token: 0x04000F35 RID: 3893
	private Hashtable iTweenArgs;

	// Token: 0x04000F36 RID: 3894
	public Transform holdLoc;

	// Token: 0x04000F37 RID: 3895
	private Vector3 lastPosition;

	// Token: 0x04000F38 RID: 3896
	private float stuckTimer;

	// Token: 0x04000F39 RID: 3897
	public float stuckCheckInterval;

	// Token: 0x04000F3A RID: 3898
	private float positionSampleTimer;

	// Token: 0x04000F3B RID: 3899
	private float positionSampleInterval = 1f;

	// Token: 0x04000F3C RID: 3900
	public Transform jail0;

	// Token: 0x04000F3D RID: 3901
	public Transform jail1;

	// Token: 0x04000F3E RID: 3902
	public Transform jail2;

	// Token: 0x04000F3F RID: 3903
	public Transform jail3;

	// Token: 0x04000F40 RID: 3904
	public Transform jail4;

	// Token: 0x04000F41 RID: 3905
	public Transform jail5;

	// Token: 0x04000F42 RID: 3906
	public Transform jail6;

	// Token: 0x04000F43 RID: 3907
	public Transform jail7;

	// Token: 0x04000F44 RID: 3908
	public Transform jail8;

	// Token: 0x04000F45 RID: 3909
	public Transform jail9;

	// Token: 0x04000F46 RID: 3910
	public Transform jail10;

	// Token: 0x04000F47 RID: 3911
	public Transform benchLoc;

	// Token: 0x04000F48 RID: 3912
	public Transform bookLoc;

	// Token: 0x04000F49 RID: 3913
	public Transform jailIdle;

	// Token: 0x04000F4A RID: 3914
	public Transform printLoc;

	// Token: 0x04000F4B RID: 3915
	public Transform seatLoc2;

	// Token: 0x04000F4C RID: 3916
	public Transform idleStation;

	// Token: 0x04000F4D RID: 3917
	public Transform idleGas;

	// Token: 0x04000F4E RID: 3918
	public Transform respawn;

	// Token: 0x04000F4F RID: 3919
	public bool idleAtStation;

	// Token: 0x04000F50 RID: 3920
	public bool idleAtGasStation;

	// Token: 0x04000F51 RID: 3921
	public bool flashersOn;

	// Token: 0x04000F52 RID: 3922
	public GameObject flashers;

	// Token: 0x04000F53 RID: 3923
	public GameObject spotlight;

	// Token: 0x04000F54 RID: 3924
	public GameObject headlights;

	// Token: 0x04000F55 RID: 3925
	public bool playerCuffed;

	// Token: 0x04000F56 RID: 3926
	public bool haulingPlayer;

	// Token: 0x04000F57 RID: 3927
	public bool criminalActivity;

	// Token: 0x04000F58 RID: 3928
	public bool warrant;

	// Token: 0x04000F59 RID: 3929
	public bool raidWarrant;

	// Token: 0x04000F5A RID: 3930
	private Rigidbody rb;

	// Token: 0x04000F5B RID: 3931
	private GameObject thisTicket;

	// Token: 0x04000F5C RID: 3932
	private float lastPlayTime;

	// Token: 0x04000F5D RID: 3933
	private bool currentlyWaiting;

	// Token: 0x04000F5E RID: 3934
	public GameObject bucket1;

	// Token: 0x04000F5F RID: 3935
	public GameObject bucket2;

	// Token: 0x04000F60 RID: 3936
	public GameObject bucket3;

	// Token: 0x04000F61 RID: 3937
	public Bucket bucket1Script;

	// Token: 0x04000F62 RID: 3938
	public Bucket bucket2Script;

	// Token: 0x04000F63 RID: 3939
	public Bucket bucket3Script;

	// Token: 0x04000F64 RID: 3940
	public Currency playerCurrency;

	// Token: 0x04000F65 RID: 3941
	private float timer;

	// Token: 0x04000F66 RID: 3942
	public car diamondback;

	// Token: 0x04000F67 RID: 3943
	public Transform diamondbackEntry;

	// Token: 0x04000F68 RID: 3944
	public car3 amc;

	// Token: 0x04000F69 RID: 3945
	public Transform amcEntry;

	// Token: 0x04000F6A RID: 3946
	public car4 f100;

	// Token: 0x04000F6B RID: 3947
	public Transform f100Entry;

	// Token: 0x04000F6C RID: 3948
	public golfcart golfCart;

	// Token: 0x04000F6D RID: 3949
	public Transform golfcartEntry;

	// Token: 0x04000F6E RID: 3950
	public Dirtbike dirtbike;

	// Token: 0x04000F6F RID: 3951
	public Transform dirtbikeEntry;

	// Token: 0x04000F70 RID: 3952
	public bool trafficStop;

	// Token: 0x04000F71 RID: 3953
	public GameObject ticketObj;

	// Token: 0x04000F72 RID: 3954
	public Transform ticketPos;

	// Token: 0x04000F73 RID: 3955
	private Citation cit;

	// Token: 0x04000F74 RID: 3956
	public Light light_db;

	// Token: 0x04000F75 RID: 3957
	public Light light_f100;

	// Token: 0x04000F76 RID: 3958
	public Light light_amc;

	// Token: 0x04000F77 RID: 3959
	public Light light_kc;

	// Token: 0x04000F78 RID: 3960
	public Light light_kcf;

	// Token: 0x04000F79 RID: 3961
	public GameObject fpsBeer;

	// Token: 0x04000F7A RID: 3962
	public GameObject fpsBottle;

	// Token: 0x04000F7B RID: 3963
	public GameObject driverGlass;

	// Token: 0x04000F7C RID: 3964
	public TillScript till;

	// Token: 0x04000F7D RID: 3965
	public bool theftNoted;

	// Token: 0x04000F7E RID: 3966
	private float cooldown;

	// Token: 0x04000F7F RID: 3967
	public int charge_dui;

	// Token: 0x04000F80 RID: 3968
	public int charge_evasion;

	// Token: 0x04000F81 RID: 3969
	public int charge_posession;

	// Token: 0x04000F82 RID: 3970
	public int charge_reckless;

	// Token: 0x04000F83 RID: 3971
	public int charge_hitandrun;

	// Token: 0x04000F84 RID: 3972
	public int charge_speeding;

	// Token: 0x04000F85 RID: 3973
	public int charge_headlights;

	// Token: 0x04000F86 RID: 3974
	public int charge_opencontainer;

	// Token: 0x04000F87 RID: 3975
	public int charge_theft;

	// Token: 0x04000F88 RID: 3976
	public int charge_obstruction;

	// Token: 0x04000F89 RID: 3977
	public int charge_pubIntox;

	// Token: 0x04000F8A RID: 3978
	private Dictionary<string, AudioClip> vehSoundDictionary;

	// Token: 0x04000F8B RID: 3979
	private Dictionary<string, AudioClip> vehLowSoundDictionary;

	// Token: 0x04000F8C RID: 3980
	private Dictionary<string, AudioClip> copSoundDictionary;

	// Token: 0x04000F8D RID: 3981
	private Dictionary<string, AudioClip> miscSoundDictionary;

	// Token: 0x04000F8E RID: 3982
	public Rigidbody[] ragDollRigidbodies;

	// Token: 0x04000F8F RID: 3983
	public Collider[] ragDollColliders;

	// Token: 0x04000F90 RID: 3984
	public Collider thisCapsule;

	// Token: 0x04000F91 RID: 3985
	private bool isRagdoll;

	// Token: 0x04000F92 RID: 3986
	public Interactor interactor;

	// Token: 0x04000F93 RID: 3987
	private bool acceptedTicket;

	// Token: 0x04000F94 RID: 3988
	public int vehicleToImpound;

	// Token: 0x04000F95 RID: 3989
	public Transform[] impoundLoc;

	// Token: 0x04000F96 RID: 3990
	public GameObject[] playerVehicle;

	// Token: 0x04000F97 RID: 3991
	public PhoneScript phoneScript;

	// Token: 0x04000F98 RID: 3992
	private Coroutine handCuffCoroutine;

	// Token: 0x04000F99 RID: 3993
	private Coroutine officerExitCoroutine;

	// Token: 0x04000F9A RID: 3994
	private Coroutine officerEnterCoroutine;

	// Token: 0x04000F9B RID: 3995
	private Coroutine initiateStopCoroutine;

	// Token: 0x04000F9C RID: 3996
	private Coroutine waitAtStationCoroutine;

	// Token: 0x04000F9D RID: 3997
	private Coroutine openJailCoroutine;

	// Token: 0x04000F9E RID: 3998
	private Coroutine openCellCoroutine;

	// Token: 0x04000F9F RID: 3999
	private Coroutine releaseCoroutine;

	// Token: 0x04000FA0 RID: 4000
	private Coroutine officerWaitCoroutine;

	// Token: 0x04000FA1 RID: 4001
	private Coroutine computerCoroutine;

	// Token: 0x04000FA2 RID: 4002
	private Coroutine processCoroutine;

	// Token: 0x04000FA3 RID: 4003
	private Coroutine recuffCoroutine;

	// Token: 0x04000FA4 RID: 4004
	private Coroutine waitForComplianceCoroutine;

	// Token: 0x04000FA5 RID: 4005
	private Coroutine PursueDelayCoroutine;

	// Token: 0x04000FA6 RID: 4006
	public InteractiveObject door1;

	// Token: 0x04000FA7 RID: 4007
	public InteractiveObject door2;

	// Token: 0x04000FA8 RID: 4008
	public InteractiveObject cellDoor;

	// Token: 0x04000FA9 RID: 4009
	public InteractiveObject exitDoor;

	// Token: 0x04000FAA RID: 4010
	public Transform suvLocation;

	// Token: 0x04000FAB RID: 4011
	public Transform raidLoc1;

	// Token: 0x04000FAC RID: 4012
	public Transform raidLoc2;

	// Token: 0x04000FAD RID: 4013
	public Transform suvRaidLoc1;

	// Token: 0x04000FAE RID: 4014
	public Transform suvRaidLoc2;

	// Token: 0x04000FAF RID: 4015
	private bool servingWarrant;

	// Token: 0x04000FB0 RID: 4016
	private float warrantTimer;

	// Token: 0x04000FB1 RID: 4017
	private float distanceToTarget;

	// Token: 0x04000FB2 RID: 4018
	public PatrolGate gate;

	// Token: 0x04000FB3 RID: 4019
	public WaypointProgressTracker wpt;

	// Token: 0x04000FB4 RID: 4020
	private int subtractContainer;

	// Token: 0x04000FB5 RID: 4021
	private float cuffTimer;

	// Token: 0x04000FB6 RID: 4022
	private bool cuffTimerActive;

	// Token: 0x04000FB7 RID: 4023
	private bool noStepOut;

	// Token: 0x04000FB8 RID: 4024
	public int delinquentDays;

	// Token: 0x04000FB9 RID: 4025
	public int activeCitations;

	// Token: 0x04000FBA RID: 4026
	public int numArrests;

	// Token: 0x04000FBB RID: 4027
	public int numWarnings;

	// Token: 0x04000FBC RID: 4028
	public GameObject[] deactivateDoors;

	// Token: 0x04000FBD RID: 4029
	public InteractiveObject jailBed;

	// Token: 0x04000FBE RID: 4030
	public Transform fpsHoldLoc;

	// Token: 0x04000FBF RID: 4031
	public AudioControl acDiamondback;

	// Token: 0x04000FC0 RID: 4032
	public AudioControlF acF100;

	// Token: 0x04000FC1 RID: 4033
	public Transform doorExitPosD;

	// Token: 0x04000FC2 RID: 4034
	public Transform doorExitPosP;

	// Token: 0x04000FC3 RID: 4035
	public GameObject wheelJoints;

	// Token: 0x04000FC4 RID: 4036
	public ModWomanJobs mwb;

	// Token: 0x04000FC5 RID: 4037
	public AiCarContrtoller aicc;
}
