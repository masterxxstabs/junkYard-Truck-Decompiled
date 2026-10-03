using System;
using UnityEngine;

// Token: 0x0200016A RID: 362
public class chainsaw : MonoBehaviour
{
	// Token: 0x060008EF RID: 2287 RVA: 0x000749A4 File Offset: 0x00072BA4
	private void Start()
	{
		this.aSources = base.GetComponents<AudioSource>();
	}

	// Token: 0x060008F0 RID: 2288 RVA: 0x000749B4 File Offset: 0x00072BB4
	private void Update()
	{
		if (this.engineOn)
		{
			if (this.dostartnoise)
			{
				this.cExhaustSmoke.SetActive(true);
				this.aSources[0].Play();
				this.dostartnoise = false;
			}
			if (!this.aSources[0].isPlaying && !this.aSources[1].isPlaying)
			{
				this.aSources[1].Play();
			}
		}
		else
		{
			this.cExhaustSmoke.SetActive(false);
			this.sawdust.SetActive(false);
			this.aSources[0].Stop();
			this.aSources[1].Stop();
			this.aSources[2].Stop();
			this.sawdustOn = false;
			this.sideways = false;
		}
		if (this.cutting && this.engineOn)
		{
			if (this.tree.name == "stump")
			{
				if (this.tree.GetComponent<treeinfo>().health > 0)
				{
					if (!this.sideways)
					{
						base.transform.eulerAngles = new Vector3(base.transform.eulerAngles.x, base.transform.eulerAngles.y, base.transform.eulerAngles.z - 90f);
						this.sideways = true;
					}
					this.tree.GetComponent<treeinfo>().health--;
					if (!this.aSources[2].isPlaying)
					{
						this.aSources[2].Play();
					}
				}
				if (this.tree.GetComponent<treeinfo>().health < 1)
				{
					Object.Destroy(this.tree.GetComponent<FixedJoint>());
				}
			}
			if (this.tree.name == "trunk1" || this.tree.name == "trunk2" || this.tree.name == "trunk3")
			{
				this.shaft = this.tree.GetComponent<treeinfolog>().shaft;
				this.secnum = int.Parse(this.tree.name.Substring(5));
				if (this.tree.GetComponent<treeinfolog>().health > 0)
				{
					if (!this.sawdustOn)
					{
						this.sawdust.SetActive(true);
						this.sawdustOn = true;
					}
					this.tree.GetComponent<treeinfolog>().health--;
					if (!this.aSources[2].isPlaying)
					{
						this.aSources[2].Play();
					}
				}
				if (this.tree.GetComponent<treeinfolog>().health < 1 && this.tree != null)
				{
					this.shaft.GetComponent<CapsuleCollider>().height = this.shaft.GetComponent<CapsuleCollider>().height - 0.1f;
					this.logpos1 = new Vector3(this.tree.transform.position.x, this.tree.transform.position.y, this.tree.transform.position.z);
					this.logpos2 = new Vector3(this.tree.transform.position.x, this.tree.transform.position.y + 4f, this.tree.transform.position.z);
					if (!this.tree.GetComponent<treeinfo>().isMissionTree)
					{
						Object.Instantiate<GameObject>(this.tree.GetComponent<treeinfolog>().logSpawn, this.logpos1, this.tree.transform.rotation);
						Object.Instantiate<GameObject>(this.tree.GetComponent<treeinfolog>().logSpawn, this.logpos2, this.tree.transform.rotation);
					}
					if (this.tree.GetComponent<treeinfo>().isMissionTree)
					{
						this.spawned = Object.Instantiate<GameObject>(this.tree.GetComponent<treeinfolog>().logSpawn, this.logpos1, this.tree.transform.rotation);
						this.spawned.name = "missionlog";
						this.spawned2 = Object.Instantiate<GameObject>(this.tree.GetComponent<treeinfolog>().logSpawn, this.logpos2, this.tree.transform.rotation);
						this.spawned2.name = "missionlog";
						this.numCuts++;
						int num = this.numCuts;
					}
					Object.Destroy(this.tree);
					if (this.branches != null)
					{
						this.branches = this.tree.GetComponent<treeinfolog>().branches;
						Object.Destroy(this.branches);
					}
					if (this.secnum == 3 && this.treetop != null)
					{
						this.treetop = this.tree.GetComponent<treeinfolog>().treetop;
						Object.Destroy(this.treetop);
					}
				}
			}
		}
		if (!this.cutting)
		{
			this.aSources[2].Stop();
			this.sawdust.SetActive(false);
			this.sawdustOn = false;
		}
	}

	// Token: 0x04001536 RID: 5430
	public bool engineOn;

	// Token: 0x04001537 RID: 5431
	public bool cutting;

	// Token: 0x04001538 RID: 5432
	public int treehealth;

	// Token: 0x04001539 RID: 5433
	public GameObject tree;

	// Token: 0x0400153A RID: 5434
	private bool dostartnoise = true;

	// Token: 0x0400153B RID: 5435
	public AudioSource[] aSources;

	// Token: 0x0400153C RID: 5436
	public GameObject sawdust;

	// Token: 0x0400153D RID: 5437
	public GameObject cExhaustSmoke;

	// Token: 0x0400153E RID: 5438
	public bool sawdustOn;

	// Token: 0x0400153F RID: 5439
	private Vector3 logpos1;

	// Token: 0x04001540 RID: 5440
	private Vector3 logpos2;

	// Token: 0x04001541 RID: 5441
	private Vector3 logpos3;

	// Token: 0x04001542 RID: 5442
	private Vector3 logpos4;

	// Token: 0x04001543 RID: 5443
	private GameObject shaft;

	// Token: 0x04001544 RID: 5444
	public bool sideways;

	// Token: 0x04001545 RID: 5445
	public int secnum;

	// Token: 0x04001546 RID: 5446
	private GameObject branches;

	// Token: 0x04001547 RID: 5447
	private GameObject treetop;

	// Token: 0x04001548 RID: 5448
	private GameObject spawned;

	// Token: 0x04001549 RID: 5449
	private GameObject spawned2;

	// Token: 0x0400154A RID: 5450
	private int numCuts;
}
