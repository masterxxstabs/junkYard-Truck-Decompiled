using System;
using System.Collections;
using Steamworks.Data;
using UnityEngine;

// Token: 0x02000112 RID: 274
public class Pickaxe : MonoBehaviour
{
	// Token: 0x0600073C RID: 1852 RVA: 0x0005E714 File Offset: 0x0005C914
	private void Start()
	{
		this.iTweenArgs = iTween.Hash(Array.Empty<object>());
		this.iTweenArgs.Add("position", this.openPosition);
		this.iTweenArgs.Add("time", this.animationTime);
		this.iTweenArgs.Add("islocal", true);
		this.aSources = base.GetComponents<AudioSource>();
	}

	// Token: 0x0600073D RID: 1853 RVA: 0x0005E789 File Offset: 0x0005C989
	public void ResetAction()
	{
		base.StartCoroutine(this.ResetAction2());
	}

	// Token: 0x0600073E RID: 1854 RVA: 0x0005E798 File Offset: 0x0005C998
	public IEnumerator ResetAction2()
	{
		yield return new WaitForSeconds(0.6f);
		this.Swing(null);
		yield break;
	}

	// Token: 0x0600073F RID: 1855 RVA: 0x0005E7A7 File Offset: 0x0005C9A7
	public IEnumerator Hit()
	{
		this.randDmg = Random.Range(1, 5);
		yield return new WaitForSeconds(0.2f);
		if (this.colliderName == "ore")
		{
			this.aSources[0].Play();
			if (this.randDmg == 1)
			{
				this.hitObj.GetChild(1).gameObject.GetComponent<ParticleSystem>().Play();
			}
			this.hitObj.GetChild(0).gameObject.GetComponent<ParticleSystem>().Play();
		}
		else
		{
			this.hitObj.GetChild(5).gameObject.GetComponent<ParticleSystem>().Play();
		}
		if (this.randDmg == 3)
		{
			if (this.colliderName == "ore")
			{
				float num = Random.Range(0.5f, 3f);
				float num2 = Random.Range(0.5f, 3f);
				float num3 = Random.Range(0.5f, 3f);
				float num4 = num + num2 + num3;
				float mass = num4 * 3f * 33f;
				float tradein = num4 * 3f * 20f;
				this.hitObj.gameObject.GetComponent<Renderer>().enabled = false;
				this.hitObj.GetChild(0).gameObject.GetComponent<ParticleSystem>().Play();
				GameObject gameObject = Object.Instantiate<GameObject>(this.ore1, base.transform.position, base.transform.rotation);
				gameObject.transform.localScale = new Vector3(num / 2f, num2 / 2f, num3 / 2f);
				gameObject.GetComponent<IronStats>().xSize = num;
				gameObject.GetComponent<IronStats>().ySize = num2;
				gameObject.GetComponent<IronStats>().zSize = num3;
				gameObject.GetComponent<PickUp>().tradein = tradein;
				gameObject.GetComponent<Rigidbody>().mass = mass;
				gameObject.name = "Ore";
				if ((double)num4 > 8.4)
				{
					Achievement achievement = new Achievement("ACH_ORE");
					achievement.Trigger(true);
				}
			}
			else
			{
				this.hitObj.GetChild(0).gameObject.SetActive(false);
				this.hitObj.GetChild(1).gameObject.SetActive(false);
				this.hitObj.GetChild(2).gameObject.SetActive(false);
				this.hitObj.GetChild(3).gameObject.SetActive(false);
				this.hitObj.GetChild(4).gameObject.GetComponent<ParticleSystem>().Play();
			}
			this.hitObj.GetComponent<SphereCollider>().enabled = false;
		}
		this.aSources[this.cn].Play();
		this.nextSwing = (float)(Mathf.FloorToInt(Time.time) + 1);
		yield break;
	}

	// Token: 0x06000740 RID: 1856 RVA: 0x0005E7B8 File Offset: 0x0005C9B8
	public void Swing(Transform hitTrans)
	{
		this.hitObj = hitTrans;
		if (this.isOpen)
		{
			if (Time.time > this.nextSwing)
			{
				if (this.hitObj != null)
				{
					this.colliderName = this.hitObj.name;
					base.StartCoroutine(this.Hit());
				}
				this.cn = Random.Range(1, 5);
				this.iTweenArgs["position"] = this.closedPosition;
				this.iTweenArgs["rotation"] = this.closedPosition;
				base.StartCoroutine(this.ResetAction2());
			}
		}
		else
		{
			this.iTweenArgs["position"] = this.openPosition;
			this.iTweenArgs["rotation"] = this.openPosition;
		}
		this.isOpen = !this.isOpen;
		Pickaxe.MovementType movementType = this.movementType;
		if (movementType == Pickaxe.MovementType.Slide)
		{
			iTween.MoveTo(base.gameObject, this.iTweenArgs);
			return;
		}
		if (movementType != Pickaxe.MovementType.Rotate)
		{
			return;
		}
		iTween.RotateTo(base.gameObject, this.iTweenArgs);
	}

	// Token: 0x04001034 RID: 4148
	[SerializeField]
	public Vector3 openPosition;

	// Token: 0x04001035 RID: 4149
	[SerializeField]
	public Vector3 closedPosition;

	// Token: 0x04001036 RID: 4150
	[SerializeField]
	private float animationTime;

	// Token: 0x04001037 RID: 4151
	[SerializeField]
	public bool isOpen;

	// Token: 0x04001038 RID: 4152
	[SerializeField]
	private Pickaxe.MovementType movementType;

	// Token: 0x04001039 RID: 4153
	private Hashtable iTweenArgs;

	// Token: 0x0400103A RID: 4154
	private string colliderName;

	// Token: 0x0400103B RID: 4155
	public AudioSource[] aSources;

	// Token: 0x0400103C RID: 4156
	private Transform hitObj;

	// Token: 0x0400103D RID: 4157
	private int cn;

	// Token: 0x0400103E RID: 4158
	private int randDmg;

	// Token: 0x0400103F RID: 4159
	public GameObject ore1;

	// Token: 0x04001040 RID: 4160
	private float nextSwing;

	// Token: 0x04001041 RID: 4161
	public CharacterController firstPersonController;

	// Token: 0x02000402 RID: 1026
	private enum MovementType
	{
		// Token: 0x040028E5 RID: 10469
		Slide,
		// Token: 0x040028E6 RID: 10470
		Rotate
	}
}
