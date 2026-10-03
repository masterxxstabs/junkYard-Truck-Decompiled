using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000154 RID: 340
public class TractionBuddy : MonoBehaviour
{
	// Token: 0x0600087C RID: 2172 RVA: 0x0006EAE8 File Offset: 0x0006CCE8
	private void Start()
	{
		this.TurnOff();
		if (!base.GetComponent<Renderer>().enabled)
		{
			base.enabled = false;
			this.aSource.enabled = false;
		}
		if (this.dead)
		{
			this.screen.material = this.black;
		}
	}

	// Token: 0x0600087D RID: 2173 RVA: 0x0006EB34 File Offset: 0x0006CD34
	public void ScanTerrain()
	{
		if (this.isOn && !this.dead)
		{
			if (this.gd.leaves > 0)
			{
				this.terrainNum = 1;
			}
			if (this.gd.rockSurface > 0)
			{
				this.terrainNum = 0;
			}
			if (this.gd.deepWater > 0)
			{
				this.terrainNum = 1;
			}
			if (this.gd.road > 0)
			{
				this.terrainNum = 0;
			}
			if (this.gd.deepMud > 0)
			{
				this.terrainNum = 1;
			}
			this.ResetWave();
			base.StartCoroutine(this.CheckTerrain());
		}
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x0006EBD0 File Offset: 0x0006CDD0
	private IEnumerator CheckTerrain()
	{
		this.SpeakDialogue(0);
		yield return new WaitForSeconds(3f);
		if (this.terrainNum != this.lastTerrain)
		{
			this.cycles++;
			if (this.cycles < 15)
			{
				this.lastTerrain = this.terrainNum;
				this.SpeakDialogue(1);
				yield return new WaitForSeconds(3f);
				if (this.terrainNum == 0)
				{
					this.SpeakDialogue(3);
					yield return new WaitForSeconds(6f);
					this.ChangePressure(true);
					this.SpeakDialogue(6);
				}
				else
				{
					this.SpeakDialogue(4);
					yield return new WaitForSeconds(6f);
					this.ChangePressure(false);
					this.SpeakDialogue(7);
				}
			}
			else
			{
				this.dead = true;
				base.StartCoroutine(this.Die());
			}
		}
		else if (Random.Range(0, 4) == 0)
		{
			this.SpeakDialogue(2);
			yield return new WaitForSeconds(4f);
		}
		yield break;
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x0006EBE0 File Offset: 0x0006CDE0
	public void TurnOn()
	{
		if (!this.dead)
		{
			this.screen.material = this.wave;
			if (!this.hello)
			{
				this.SpeakDialogue(9);
				this.hello = true;
			}
			this.isOn = true;
			this.dura.canDetach = false;
		}
	}

	// Token: 0x06000880 RID: 2176 RVA: 0x0006EC30 File Offset: 0x0006CE30
	public void TurnOff()
	{
		this.isOn = false;
		if (!this.busy)
		{
			this.screen.material = this.black;
			this.dura.canDetach = true;
		}
	}

	// Token: 0x06000881 RID: 2177 RVA: 0x0006EC5E File Offset: 0x0006CE5E
	private void ResetWave()
	{
		this.wave.mainTextureScale = new Vector2(3.5f, 3.7f);
		this.waveform = false;
	}

	// Token: 0x06000882 RID: 2178 RVA: 0x0006EC81 File Offset: 0x0006CE81
	private void SpeakDialogue(int dialogueNum)
	{
		this.aSource.clip = this.clip[dialogueNum];
		this.aSource.Play();
		base.StartCoroutine(this.VoiceSynth());
	}

	// Token: 0x06000883 RID: 2179 RVA: 0x0006ECAE File Offset: 0x0006CEAE
	private IEnumerator VoiceSynth()
	{
		float length = this.aSource.clip.length;
		this.waveform = true;
		yield return new WaitForSeconds(length);
		this.ResetWave();
		yield break;
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x0006ECBD File Offset: 0x0006CEBD
	private IEnumerator Die()
	{
		this.busy = true;
		this.SpeakDialogue(1);
		yield return new WaitForSeconds(3f);
		this.SpeakDialogue(5);
		yield return new WaitForSeconds(6f);
		this.SpeakDialogue(6);
		yield return new WaitForSeconds(5f);
		this.SpeakDialogue(6);
		yield return new WaitForSeconds(5f);
		this.SpeakDialogue(6);
		yield return new WaitForSeconds(5f);
		this.SpeakDialogue(8);
		yield return new WaitForSeconds(10f);
		this.SpeakDialogue(10);
		this.fireObject.SetActive(true);
		this.mw.job5Condition = true;
		for (int i = 0; i < this.fireObject.transform.childCount; i++)
		{
			if (i != 4 && i != 5)
			{
				this.fireObject.transform.GetChild(i).gameObject.SetActive(false);
			}
		}
		yield return new WaitForSeconds(2f);
		this.busy = false;
		this.screen.material = this.black;
		for (int j = 0; j < this.fireObject.transform.childCount; j++)
		{
			this.fireObject.transform.GetChild(j).gameObject.SetActive(true);
		}
		yield return new WaitForSeconds(13f);
		this.dura.canDetach = true;
		this.dura.health = 0f;
		this.TurnOff();
		yield break;
	}

	// Token: 0x06000885 RID: 2181 RVA: 0x0006ECCC File Offset: 0x0006CECC
	private void ChangePressure(bool inflate)
	{
		foreach (object obj in this.wheelHolderFR)
		{
			Transform transform = (Transform)obj;
			int siblingIndex = transform.GetSiblingIndex();
			if (siblingIndex > 5 && transform.gameObject.active)
			{
				this.tireFR = this.wheelHolderFR.transform.GetChild(siblingIndex).gameObject;
				break;
			}
		}
		foreach (object obj2 in this.wheelHolderFL)
		{
			Transform transform2 = (Transform)obj2;
			int siblingIndex2 = transform2.GetSiblingIndex();
			if (siblingIndex2 > 5 && transform2.gameObject.active)
			{
				this.tireFL = this.wheelHolderFL.transform.GetChild(siblingIndex2).gameObject;
				break;
			}
		}
		foreach (object obj3 in this.wheelHolderRL)
		{
			Transform transform3 = (Transform)obj3;
			int siblingIndex3 = transform3.GetSiblingIndex();
			if (siblingIndex3 > 5 && transform3.gameObject.active)
			{
				this.tireRL = this.wheelHolderRL.transform.GetChild(siblingIndex3).gameObject;
				break;
			}
		}
		foreach (object obj4 in this.wheelHolderRR)
		{
			Transform transform4 = (Transform)obj4;
			int siblingIndex4 = transform4.GetSiblingIndex();
			if (siblingIndex4 > 5 && transform4.gameObject.active)
			{
				this.tireRR = this.wheelHolderRR.transform.GetChild(siblingIndex4).gameObject;
				break;
			}
		}
		this.deflation = 0f;
		if (!inflate)
		{
			this.deflation = 0.7f;
		}
		double num = (double)(1f - this.deflation) * 0.11000000000000004 + 0.29;
		this.tireRL.GetComponent<Renderer>().material.SetFloat("_TireFlatnessT", this.deflation);
		this.tireRR.GetComponent<Renderer>().material.SetFloat("_TireFlatnessT", this.deflation);
		this.tireFL.GetComponent<Renderer>().material.SetFloat("_TireFlatnessT", this.deflation);
		this.tireFR.GetComponent<Renderer>().material.SetFloat("_TireFlatnessT", this.deflation);
		this.wcFL.radius = (float)num;
		this.wcFR.radius = (float)num;
		this.wcRR.radius = (float)num;
		this.wcRL.radius = (float)num;
	}

	// Token: 0x06000886 RID: 2182 RVA: 0x0006EFC8 File Offset: 0x0006D1C8
	private void Update()
	{
		if (this.isOn)
		{
			this.scanTimer += Time.deltaTime;
			if (this.scanTimer >= 20f)
			{
				this.scanTimer = 0f;
				this.ScanTerrain();
			}
			this.offsetX = 0f;
			if (this.waveform)
			{
				this.timer += Time.deltaTime;
				if (this.timer >= this.interval)
				{
					this.toggleState = !this.toggleState;
					this.timer = 0f;
				}
				if (this.toggleState)
				{
					this.offsetX = 1.18f;
					this.wave.mainTextureScale = new Vector2(2f, 3.7f);
				}
				else
				{
					this.offsetX = 0f;
					this.wave.mainTextureScale = new Vector2(3.5f, 3.7f);
				}
			}
			this.offsetY -= this.scrollSpeed * Time.deltaTime;
			if (this.offsetY <= -0.14f)
			{
				this.offsetY = 0f;
			}
			this.wave.mainTextureOffset = new Vector2(this.offsetX, this.offsetY);
		}
	}

	// Token: 0x04001397 RID: 5015
	public AudioSource aSource;

	// Token: 0x04001398 RID: 5016
	public AudioClip[] clip;

	// Token: 0x04001399 RID: 5017
	public int terrainNum;

	// Token: 0x0400139A RID: 5018
	public int lastTerrain;

	// Token: 0x0400139B RID: 5019
	public Material black;

	// Token: 0x0400139C RID: 5020
	public Material wave;

	// Token: 0x0400139D RID: 5021
	public Renderer screen;

	// Token: 0x0400139E RID: 5022
	public float scrollSpeed = 0.01f;

	// Token: 0x0400139F RID: 5023
	private float offsetY;

	// Token: 0x040013A0 RID: 5024
	public float offsetX;

	// Token: 0x040013A1 RID: 5025
	private const float maxOffset = -0.14f;

	// Token: 0x040013A2 RID: 5026
	private bool waveform;

	// Token: 0x040013A3 RID: 5027
	public bool toggleState;

	// Token: 0x040013A4 RID: 5028
	private float timer;

	// Token: 0x040013A5 RID: 5029
	public float interval = 0.1f;

	// Token: 0x040013A6 RID: 5030
	public bool isOn;

	// Token: 0x040013A7 RID: 5031
	public GroundDetect gd;

	// Token: 0x040013A8 RID: 5032
	private bool hello;

	// Token: 0x040013A9 RID: 5033
	private float scanTimer;

	// Token: 0x040013AA RID: 5034
	public int cycles;

	// Token: 0x040013AB RID: 5035
	private bool dead;

	// Token: 0x040013AC RID: 5036
	public Transform wheelHolderFR;

	// Token: 0x040013AD RID: 5037
	public Transform wheelHolderFL;

	// Token: 0x040013AE RID: 5038
	public Transform wheelHolderRR;

	// Token: 0x040013AF RID: 5039
	public Transform wheelHolderRL;

	// Token: 0x040013B0 RID: 5040
	public WheelCollider wcFR;

	// Token: 0x040013B1 RID: 5041
	public WheelCollider wcFL;

	// Token: 0x040013B2 RID: 5042
	public WheelCollider wcRR;

	// Token: 0x040013B3 RID: 5043
	public WheelCollider wcRL;

	// Token: 0x040013B4 RID: 5044
	private GameObject tireRR;

	// Token: 0x040013B5 RID: 5045
	private GameObject tireRL;

	// Token: 0x040013B6 RID: 5046
	private GameObject tireFR;

	// Token: 0x040013B7 RID: 5047
	private GameObject tireFL;

	// Token: 0x040013B8 RID: 5048
	private float deflation;

	// Token: 0x040013B9 RID: 5049
	public GameObject fireObject;

	// Token: 0x040013BA RID: 5050
	private bool busy;

	// Token: 0x040013BB RID: 5051
	public durability dura;

	// Token: 0x040013BC RID: 5052
	public ModWomanJobs mw;
}
