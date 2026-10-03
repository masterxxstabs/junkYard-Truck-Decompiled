using System;
using System.Collections;
using SMPScripts;
using UnityEngine;

// Token: 0x0200004D RID: 77
public class Dirtbike : MonoBehaviour
{
	// Token: 0x06000177 RID: 375 RVA: 0x00010A4F File Offset: 0x0000EC4F
	private void Start()
	{
		this.running = false;
		if (base.GetComponent<PickUp>().pickable)
		{
			base.GetComponent<InteractiveObject>().description = "";
		}
	}

	// Token: 0x06000178 RID: 376 RVA: 0x00010A78 File Offset: 0x0000EC78
	public void Kick()
	{
		int num = Random.Range(0, 3);
		AudioClip clip = this.audioClips[num];
		this.aSource.clip = clip;
		this.aSource.Play();
		Vector3 localEulerAngles = new Vector3(0f, 0f, -90f);
		this.kickStartOpen.transform.localEulerAngles = localEulerAngles;
		base.StartCoroutine(this.Unkick());
	}

	// Token: 0x06000179 RID: 377 RVA: 0x00010AE1 File Offset: 0x0000ECE1
	private IEnumerator Unkick()
	{
		yield return new WaitForSeconds(0.3f);
		Vector3 localEulerAngles = new Vector3(90f, 0f, -90f);
		this.kickStartOpen.transform.localEulerAngles = localEulerAngles;
		if (Random.Range(0, 2) == 1)
		{
			this.engine250.Refresh();
			if (this.engine250.canRun)
			{
				this.StartUp();
			}
		}
		yield break;
	}

	// Token: 0x0600017A RID: 378 RVA: 0x00010AF0 File Offset: 0x0000ECF0
	public void Sit()
	{
		this.engineBlock.GetComponent<Rigidbody>().useGravity = false;
		this.engineBlock.GetComponent<Rigidbody>().mass = 0.1f;
		base.GetComponent<PickUp>().pickable = false;
		base.GetComponent<PickUp>().enabled = false;
		base.GetComponent<MotoController>().enabled = true;
		base.GetComponent<MotoStatus>().enabled = true;
		base.GetComponent<MotoStatus>().dislodged = false;
		base.GetComponent<MotoSuspensionManager>().enabled = true;
		this.kickStart.SetActive(false);
		this.kickStartOpen.SetActive(true);
		base.GetComponent<Rigidbody>().drag = 0.1f;
		base.StartCoroutine(this.EnableFS());
	}

	// Token: 0x0600017B RID: 379 RVA: 0x00010BA0 File Offset: 0x0000EDA0
	private IEnumerator EnableFS()
	{
		yield return new WaitForSeconds(2f);
		base.GetComponent<MotoController>().airTimeSettings.freestyle = true;
		yield break;
	}

	// Token: 0x0600017C RID: 380 RVA: 0x00010BB0 File Offset: 0x0000EDB0
	public void Stand()
	{
		this.engineBlock.GetComponent<Rigidbody>().useGravity = true;
		this.engineBlock.GetComponent<Rigidbody>().mass = 1f;
		base.GetComponent<MotoController>().enabled = false;
		base.GetComponent<MotoStatus>().enabled = false;
		base.GetComponent<PickUp>().enabled = true;
		base.GetComponent<PickUp>().pickable = true;
		this.kickStart.SetActive(true);
		this.kickStartOpen.SetActive(false);
		base.GetComponent<Rigidbody>().drag = 1f;
		base.GetComponent<MotoController>().airTimeSettings.freestyle = false;
	}

	// Token: 0x0600017D RID: 381 RVA: 0x00010C4C File Offset: 0x0000EE4C
	public void ShutOff()
	{
		this.engineSound.GetComponent<MotoSound>().GetOff();
		this.engineSound.GetComponent<MotoSound>().enabled = false;
		this.kickStart.SetActive(false);
		this.kickStartOpen.SetActive(true);
		this.running = false;
		this.exhaustTrails.GetComponent<ParticleSystem>().Stop();
		this.steamTrails.GetComponent<ParticleSystem>().Stop();
	}

	// Token: 0x0600017E RID: 382 RVA: 0x00010CBC File Offset: 0x0000EEBC
	public void StartUp()
	{
		Debug.Log("startup");
		this.running = true;
		this.exhaustTrails.GetComponent<ParticleSystem>().Play();
		this.engineSound.GetComponent<MotoSound>().enabled = true;
		this.engineSound.GetComponent<MotoSound>().StartAudio();
		base.GetComponent<MotoController>().enabled = true;
		base.GetComponent<MotoSuspensionManager>().enabled = true;
		this.kickStartOpen.SetActive(false);
		this.kickStart.SetActive(true);
	}

	// Token: 0x0600017F RID: 383 RVA: 0x00010D3C File Offset: 0x0000EF3C
	private void FixedUpdate()
	{
		if (this.running)
		{
			if (Time.time >= (float)this.engineCheckInterval)
			{
				this.engineCheckInterval = Mathf.FloorToInt(Time.time) + 8;
				this.engine250.DegradeEngine();
				if (this.engine250.tempIncrease > 40f)
				{
					this.steamTrails.GetComponent<ParticleSystem>().Play();
				}
			}
			this.engine250.newFuelLevel -= 0.8f * Time.deltaTime;
			if (this.engine250.additive > 1f)
			{
				this.engine250.additive -= 0.3f * Time.deltaTime;
				this.additiveBonus = 100;
			}
			else
			{
				this.additiveBonus = 0;
			}
			if (this.engine250.newFuelLevel <= 1f || this.waterFlood == 1)
			{
				this.running = false;
				this.ShutOff();
			}
			if (Time.time > (float)this.materialCheckInterval)
			{
				this.materialCheckInterval = Mathf.FloorToInt(Time.time) + 1;
				this.groundDetect.GetTerrainTexture();
			}
			this.camTransform.Rotate(720f * Time.deltaTime, 0f, 0f, Space.Self);
			this.clutchTransform.Rotate(720f * Time.deltaTime, 0f, 0f, Space.Self);
			this.flywheelTransform.Rotate(720f * Time.deltaTime, 0f, 0f, Space.Self);
			this.speed = this.rb.velocity.magnitude * 3.6f;
		}
	}

	// Token: 0x04000450 RID: 1104
	public GameObject kickStart;

	// Token: 0x04000451 RID: 1105
	public GameObject kickStartOpen;

	// Token: 0x04000452 RID: 1106
	public Engine250 engine250;

	// Token: 0x04000453 RID: 1107
	public GameObject engineBlock;

	// Token: 0x04000454 RID: 1108
	public AudioClip[] audioClips;

	// Token: 0x04000455 RID: 1109
	public AudioSource aSource;

	// Token: 0x04000456 RID: 1110
	public GameObject engineSound;

	// Token: 0x04000457 RID: 1111
	public bool running;

	// Token: 0x04000458 RID: 1112
	private int engineCheckInterval = 2;

	// Token: 0x04000459 RID: 1113
	public GameObject exhaustTrails;

	// Token: 0x0400045A RID: 1114
	public GameObject steamTrails;

	// Token: 0x0400045B RID: 1115
	private int materialCheckInterval = 2;

	// Token: 0x0400045C RID: 1116
	public GroundDetect groundDetect;

	// Token: 0x0400045D RID: 1117
	public int waterFlood;

	// Token: 0x0400045E RID: 1118
	public int powerDivision = 1;

	// Token: 0x0400045F RID: 1119
	public Transform camTransform;

	// Token: 0x04000460 RID: 1120
	public Transform clutchTransform;

	// Token: 0x04000461 RID: 1121
	public Transform flywheelTransform;

	// Token: 0x04000462 RID: 1122
	public Rigidbody rb;

	// Token: 0x04000463 RID: 1123
	public float speed;

	// Token: 0x04000464 RID: 1124
	public ModWomanJobs mw;

	// Token: 0x04000465 RID: 1125
	public int additiveBonus;
}
