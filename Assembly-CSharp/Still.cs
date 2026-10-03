using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000140 RID: 320
public class Still : MonoBehaviour
{
	// Token: 0x0600083F RID: 2111 RVA: 0x0006CF70 File Offset: 0x0006B170
	public void Start()
	{
		float num = this.quality;
		if (this.quality <= 0f)
		{
			this.quality = 1f;
		}
		if (float.IsNaN(this.quality))
		{
			this.quality = 1f;
		}
		if (this.alcohol <= 0f)
		{
			this.quality = 0.01f;
			this.alcohol = 0.01f;
		}
		if (this.quality == float.PositiveInfinity)
		{
			this.quality = 0.01f;
		}
		if (this.infuser.GetComponent<Renderer>().enabled)
		{
			this.straightpipe.SetActive(false);
		}
		this.KegVisual();
		this.fuel = this.fuelTank.GetComponent<durability>().health;
		if (this.burner)
		{
			this.burnerLight.GetComponent<Light>().enabled = true;
			this.burnerLight.GetComponent<AudioSource>().Play();
		}
	}

	// Token: 0x06000840 RID: 2112 RVA: 0x0006D054 File Offset: 0x0006B254
	public void OnOff()
	{
		this.fuel = this.fuelTank.GetComponent<durability>().health;
		this.burner = !this.burner;
		if (this.fuel < 1f)
		{
			this.burner = false;
		}
		if (this.burner)
		{
			this.burnerLight.GetComponent<Light>().enabled = true;
			this.burnerLight.GetComponent<AudioSource>().Play();
			return;
		}
		this.burnerLight.GetComponent<Light>().enabled = false;
		this.burnerLight.GetComponent<AudioSource>().Stop();
	}

	// Token: 0x06000841 RID: 2113 RVA: 0x0006D0E8 File Offset: 0x0006B2E8
	public void Distill()
	{
		this.fuel = this.fuelTank.GetComponent<durability>().health;
		if (this.heat > 0f)
		{
			this.heat -= 1f;
		}
		if (this.heat < 96f)
		{
			this.boilloop.Stop();
		}
		if (this.fuel > 0f && this.burner)
		{
			this.fuel -= 4f;
			this.pumpBattery.GetComponent<durability>().health -= 0.05f;
			if (this.heat < 99f)
			{
				this.heat += 5f;
			}
			if (this.heat > 95f && this.alcohol > 0f)
			{
				this.subtractQuality = 0f;
				if (this.pumpBattery.GetComponent<durability>().health < 1f)
				{
					this.subtractQuality = 3f;
				}
				this.alcohol -= 0.75f;
				this.boilloop.Play();
				float num = 0.15f / (this.vatScript.moonshine + 0.15f);
				this.vatScript.quality = this.vatScript.quality * (1f - num) + this.quality * num;
				this.vatScript.moonshine += 0.15f;
				if (this.vatScript.moonshine < 3f && !this.vaSource.isPlaying)
				{
					this.vaSource.Play();
				}
				if (this.infuserLevelB.health > 0f)
				{
					this.vatScript.blackberryPercent += 0.15f;
					this.infuserLevelB.health -= 0.15f;
					if (this.infuserLevelB.health <= 0f)
					{
						this.blackberries.enabled = false;
					}
				}
				if (this.infuserLevelL.health > 0f)
				{
					this.vatScript.limePercent += 0.15f;
					this.infuserLevelL.health -= 0.15f;
					if (this.infuserLevelL.health <= 0f)
					{
						this.limes.enabled = false;
					}
				}
				if (this.infuserLevelO.health > 0f)
				{
					this.vatScript.orangePercent += 0.15f;
					this.infuserLevelO.health -= 0.15f;
					if (this.infuserLevelO.health <= 0f)
					{
						this.oranges.enabled = false;
					}
				}
				if (this.infuserLevelA.health > 0f)
				{
					this.vatScript.ambrosiaPercent += 0.15f;
					this.infuserLevelA.health -= 0.15f;
					if (this.infuserLevelA.health <= 0f)
					{
						this.ambrosia.enabled = false;
					}
				}
				this.vatScript.UpdateVisual();
				this.KegVisual();
			}
			else
			{
				this.boilloop.Stop();
			}
			this.fuelTank.GetComponent<durability>().health = this.fuel;
		}
		if (this.fuel <= 0f && this.burner)
		{
			this.burner = false;
			this.burnerLight.GetComponent<Light>().enabled = false;
			this.burnerLight.GetComponent<AudioSource>().Stop();
		}
	}

	// Token: 0x06000842 RID: 2114 RVA: 0x0006D47D File Offset: 0x0006B67D
	private void Update()
	{
		if (Time.time >= this.nextTime)
		{
			this.Distill();
			this.nextTime += 1f;
		}
	}

	// Token: 0x06000843 RID: 2115 RVA: 0x0006D4A4 File Offset: 0x0006B6A4
	public void AddBucket(int addedAlcohol, int addedQuality, GameObject thisBucket)
	{
		float num = (float)addedAlcohol / (this.alcohol + (float)addedAlcohol);
		this.quality = this.quality * (1f - num) + (float)addedQuality * num;
		this.alcohol += (float)addedAlcohol;
		base.StartCoroutine(this.BucketAnim(thisBucket));
		this.fill1 = thisBucket.transform.GetChild(2).gameObject;
		this.fill2 = thisBucket.transform.GetChild(3).gameObject;
		this.fill3 = thisBucket.transform.GetChild(4).gameObject;
		this.fill4 = thisBucket.transform.GetChild(0).gameObject;
		if (!this.fill1.activeSelf)
		{
			this.fill1.SetActive(true);
		}
		else if (!this.fill2.activeSelf)
		{
			this.fill2.SetActive(true);
		}
		else if (!this.fill3.activeSelf)
		{
			this.fill3.SetActive(true);
		}
		else if (!this.fill4.activeSelf)
		{
			this.fill4.SetActive(true);
		}
		this.KegVisual();
	}

	// Token: 0x06000844 RID: 2116 RVA: 0x0006D5C0 File Offset: 0x0006B7C0
	private void KegVisual()
	{
		this.kegfill1.SetActive(false);
		this.kegfill2.SetActive(false);
		this.kegfill3.SetActive(false);
		this.kegfill4.SetActive(false);
		if (this.alcohol >= 700f)
		{
			this.kegfill4.SetActive(true);
			return;
		}
		if (this.alcohol >= 500f)
		{
			this.kegfill3.SetActive(true);
			return;
		}
		if (this.alcohol >= 300f)
		{
			this.kegfill2.SetActive(true);
			return;
		}
		if (this.alcohol >= 100f)
		{
			this.kegfill1.SetActive(true);
		}
	}

	// Token: 0x06000845 RID: 2117 RVA: 0x0006D664 File Offset: 0x0006B864
	private IEnumerator BucketAnim(GameObject thisBucket)
	{
		thisBucket.SetActive(false);
		this.bucketVisual.GetComponent<MeshRenderer>().enabled = true;
		this.bucketVisual.GetComponent<InteractiveObject>().PerformAction();
		yield return new WaitForSeconds(1f);
		this.bucketVisual.GetComponent<MeshRenderer>().enabled = false;
		this.bucketVisual.GetComponent<InteractiveObject>().PerformAction();
		thisBucket.SetActive(true);
		thisBucket.GetComponent<Rigidbody>().useGravity = true;
		thisBucket.transform.parent = null;
		yield break;
	}

	// Token: 0x04001320 RID: 4896
	public float alcohol;

	// Token: 0x04001321 RID: 4897
	public float quality;

	// Token: 0x04001322 RID: 4898
	public float fuel;

	// Token: 0x04001323 RID: 4899
	public bool burner;

	// Token: 0x04001324 RID: 4900
	public GameObject burnerLight;

	// Token: 0x04001325 RID: 4901
	public GameObject fuelTank;

	// Token: 0x04001326 RID: 4902
	private float nextTime;

	// Token: 0x04001327 RID: 4903
	public float heat;

	// Token: 0x04001328 RID: 4904
	private int addedAlcohol;

	// Token: 0x04001329 RID: 4905
	private int addedQuality;

	// Token: 0x0400132A RID: 4906
	private GameObject fill1;

	// Token: 0x0400132B RID: 4907
	private GameObject fill2;

	// Token: 0x0400132C RID: 4908
	private GameObject fill3;

	// Token: 0x0400132D RID: 4909
	private GameObject fill4;

	// Token: 0x0400132E RID: 4910
	public GameObject kegfill1;

	// Token: 0x0400132F RID: 4911
	public GameObject kegfill2;

	// Token: 0x04001330 RID: 4912
	public GameObject kegfill3;

	// Token: 0x04001331 RID: 4913
	public GameObject kegfill4;

	// Token: 0x04001332 RID: 4914
	public GameObject bucketVisual;

	// Token: 0x04001333 RID: 4915
	private GameObject thisBucket;

	// Token: 0x04001334 RID: 4916
	public GameObject vatFluid;

	// Token: 0x04001335 RID: 4917
	public Vat vatScript;

	// Token: 0x04001336 RID: 4918
	public GameObject pumpBattery;

	// Token: 0x04001337 RID: 4919
	private float subtractQuality;

	// Token: 0x04001338 RID: 4920
	public AudioSource boilloop;

	// Token: 0x04001339 RID: 4921
	public GameObject infuser;

	// Token: 0x0400133A RID: 4922
	public Renderer blackberries;

	// Token: 0x0400133B RID: 4923
	public Renderer limes;

	// Token: 0x0400133C RID: 4924
	public Renderer oranges;

	// Token: 0x0400133D RID: 4925
	public Renderer ambrosia;

	// Token: 0x0400133E RID: 4926
	public durability infuserLevelB;

	// Token: 0x0400133F RID: 4927
	public durability infuserLevelL;

	// Token: 0x04001340 RID: 4928
	public durability infuserLevelO;

	// Token: 0x04001341 RID: 4929
	public durability infuserLevelA;

	// Token: 0x04001342 RID: 4930
	public GameObject straightpipe;

	// Token: 0x04001343 RID: 4931
	public AudioSource vaSource;
}
