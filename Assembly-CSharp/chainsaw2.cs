using System;
using UnityEngine;

// Token: 0x0200016B RID: 363
public class chainsaw2 : MonoBehaviour
{
	// Token: 0x060008F2 RID: 2290 RVA: 0x00074EE1 File Offset: 0x000730E1
	private void Start()
	{
		this.aSources = base.GetComponents<AudioSource>();
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x00074EF0 File Offset: 0x000730F0
	private bool GetIntFromEnd(string text, out int number)
	{
		int num = text.Length - 1;
		while (num >= 0 && char.IsNumber(text[num]))
		{
			num--;
		}
		return int.TryParse(text.Substring(num + 1), out number);
	}

	// Token: 0x060008F4 RID: 2292 RVA: 0x00074F30 File Offset: 0x00073130
	private void Update()
	{
		if (this.engineOn)
		{
			if (!this.aSources[0].isPlaying && !this.aSources[1].isPlaying)
			{
				this.aSources[1].Play();
			}
			this.temperature += 0.001f;
			if (this.temperature > 200f)
			{
				this.temperature = 100f;
			}
			this.fuel -= 0.003f;
			if (!this.cutting)
			{
				this.aSources[2].Stop();
				return;
			}
			this.fuel -= 0.03f;
			if (this.fuel < 1f)
			{
				this.aSources[0].Stop();
				this.aSources[1].Stop();
				this.aSources[2].Stop();
				this.aSources[3].Stop();
				this.aSources[4].Stop();
				this.StartSaw();
			}
			if (!this.aSources[2].isPlaying)
			{
				this.aSources[2].Play();
			}
			this.info.health--;
			if (this.info.health < 1)
			{
				this.cutting = false;
				if (this.tree.GetComponent<FixedJoint>() != null)
				{
					Object.Destroy(this.tree.GetComponent<FixedJoint>());
					this.tree.name = "StumpCut";
					this.randX = Random.Range(-0.5f, 0.5f);
					this.randY = Random.Range(-0.5f, 0.5f);
					this.randZ = Random.Range(-0.5f, 0.5f);
					this.shaft.transform.eulerAngles = new Vector3(this.shaft.transform.eulerAngles.x + this.randX, this.shaft.transform.eulerAngles.y + this.randY, this.shaft.transform.eulerAngles.z + this.randZ);
					this.cuttingParticles.SetActive(false);
					this.aSources[2].Stop();
					string name = this.tree.transform.parent.name;
					int num;
					this.GetIntFromEnd(name, out num);
					this.dtm.cutTimes[num] = this.curr.playMinutes;
					return;
				}
				if (this.tree.GetComponent<FixedJoint>() == null && this.tree.name == "stump0")
				{
					this.tree.SetActive(false);
					this.cuttingParticles.SetActive(false);
					this.aSources[2].Stop();
					return;
				}
				this.secnum = int.Parse(this.tree.name.Substring(5));
				if (this.secnum == 1)
				{
					BoxCollider[] components = this.shaft.transform.GetChild(3).gameObject.GetComponents<BoxCollider>();
					components[0].enabled = true;
					components[1].enabled = true;
				}
				if (this.secnum == 2)
				{
					BoxCollider[] components2 = this.shaft.transform.GetChild(2).gameObject.GetComponents<BoxCollider>();
					components2[0].enabled = true;
					components2[1].enabled = true;
				}
				this.shaft.GetComponent<CapsuleCollider>().height = this.shaft.GetComponent<CapsuleCollider>().height * 0.66f;
				this.logpos1 = new Vector3(this.tree.transform.position.x + 0.1f, this.tree.transform.position.y + 0.3f, this.tree.transform.position.z);
				this.logpos2 = new Vector3(this.tree.transform.position.x, this.tree.transform.position.y + 0.6f, this.tree.transform.position.z + 0.1f);
				this.logRot = new Vector3(this.shaft.transform.rotation.x, this.shaft.transform.rotation.y, this.shaft.transform.rotation.z);
				Object.Instantiate<GameObject>(this.tree.GetComponent<treeinfolog>().logSpawn, this.logpos1, Quaternion.Euler(this.logRot));
				Object.Instantiate<GameObject>(this.tree.GetComponent<treeinfolog>().logSpawn, this.logpos2, Quaternion.Euler(this.logRot));
				Object.Destroy(this.tree);
				this.cuttingParticles.SetActive(false);
				this.aSources[2].Stop();
				this.branches = this.tree.GetComponent<treeinfolog>().branches;
				this.ribbon = this.tree.GetComponent<treeinfolog>().ribbon;
				if (this.branches != null)
				{
					Object.Destroy(this.branches);
				}
				if (this.ribbon != null)
				{
					Object.Destroy(this.ribbon);
				}
				if (this.secnum == 3)
				{
					if (this.treetop == null)
					{
						this.treetop = this.tree.GetComponent<treeinfolog>().treetop;
					}
					Object.Destroy(this.treetop);
					if (this.info.isMissionTree)
					{
						this.mg.NextTracker();
					}
				}
				if (this.shaft.GetComponent<CapsuleCollider>().height < 5f)
				{
					Object.Destroy(this.shaft);
					return;
				}
			}
		}
		else if (this.temperature > 0f)
		{
			this.temperature -= 0.001f;
		}
	}

	// Token: 0x060008F5 RID: 2293 RVA: 0x000754FC File Offset: 0x000736FC
	private void OnTriggerEnter(Collider other)
	{
		if (this.engineOn && (other.name == "stump" || other.name == "trunk1" || other.name == "trunk2" || other.name == "trunk3" || other.name == "stump0"))
		{
			this.cutting = true;
			this.tree = other.gameObject;
			this.info = this.tree.GetComponent<treeinfolog>();
			this.shaft = this.info.shaft;
			this.cuttingParticles.SetActive(true);
			if (other.name == "stump" && !this.sideways)
			{
				base.transform.eulerAngles = new Vector3(base.transform.eulerAngles.x, base.transform.eulerAngles.y, base.transform.eulerAngles.z - 90f);
				this.sideways = true;
				other.GetComponent<treeinfo>().enabled = true;
			}
			if (other.name == "stump0")
			{
				other.GetComponent<treeinfo>().enabled = true;
			}
		}
	}

	// Token: 0x060008F6 RID: 2294 RVA: 0x00075644 File Offset: 0x00073844
	private void OnTriggerExit(Collider other)
	{
		if (other.name == "stump")
		{
			this.cutting = false;
			this.cuttingParticles.SetActive(false);
		}
		if (this.sideways)
		{
			base.transform.eulerAngles = new Vector3(base.transform.eulerAngles.x, base.transform.eulerAngles.y, base.transform.eulerAngles.z + 90f);
			this.sideways = false;
		}
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x000756CC File Offset: 0x000738CC
	public void StartSaw()
	{
		if (!this.engineOn)
		{
			this.aSources[0].Stop();
			this.aSources[1].Stop();
			this.aSources[2].Stop();
			this.aSources[3].Stop();
			this.aSources[4].Stop();
			this.doStart = Random.Range(1, 4);
			if (this.fuel < 1f)
			{
				this.doStart = Random.Range(1, 3);
			}
			if (this.doStart == 1)
			{
				this.aSources[3].Play();
			}
			if (this.doStart == 2)
			{
				this.aSources[4].Play();
			}
			if (this.temperature > 50f || this.doStart == 3)
			{
				this.aSources[3].Play();
				this.engineOn = true;
				this.aSources[0].Play();
				this.smoke.SetActive(true);
				return;
			}
		}
		else
		{
			this.engineOn = false;
			this.cutting = false;
			this.cuttingParticles.SetActive(false);
			this.smoke.SetActive(false);
			this.aSources[0].Stop();
			this.aSources[1].Stop();
			this.aSources[2].Stop();
			this.aSources[3].Stop();
			this.aSources[4].Stop();
		}
	}

	// Token: 0x0400154B RID: 5451
	public bool engineOn;

	// Token: 0x0400154C RID: 5452
	public AudioSource[] aSources;

	// Token: 0x0400154D RID: 5453
	public bool cutting;

	// Token: 0x0400154E RID: 5454
	public GameObject tree;

	// Token: 0x0400154F RID: 5455
	public GameObject shaft;

	// Token: 0x04001550 RID: 5456
	public GameObject branches;

	// Token: 0x04001551 RID: 5457
	public GameObject ribbon;

	// Token: 0x04001552 RID: 5458
	public GameObject treetop;

	// Token: 0x04001553 RID: 5459
	public int thisHealth;

	// Token: 0x04001554 RID: 5460
	public treeinfolog info;

	// Token: 0x04001555 RID: 5461
	public GameObject cuttingParticles;

	// Token: 0x04001556 RID: 5462
	public GameObject smoke;

	// Token: 0x04001557 RID: 5463
	private bool sideways;

	// Token: 0x04001558 RID: 5464
	private int secnum;

	// Token: 0x04001559 RID: 5465
	private Vector3 logpos1;

	// Token: 0x0400155A RID: 5466
	private Vector3 logpos2;

	// Token: 0x0400155B RID: 5467
	private Vector3 logRot;

	// Token: 0x0400155C RID: 5468
	private float randX;

	// Token: 0x0400155D RID: 5469
	private float randY;

	// Token: 0x0400155E RID: 5470
	private float randZ;

	// Token: 0x0400155F RID: 5471
	private float temperature;

	// Token: 0x04001560 RID: 5472
	private int doStart;

	// Token: 0x04001561 RID: 5473
	public float fuel;

	// Token: 0x04001562 RID: 5474
	public MissionGen mg;

	// Token: 0x04001563 RID: 5475
	public DeadTreeMgr dtm;

	// Token: 0x04001564 RID: 5476
	public Currency curr;
}
