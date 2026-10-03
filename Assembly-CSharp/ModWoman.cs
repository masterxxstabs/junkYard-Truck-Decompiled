using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000FA RID: 250
public class ModWoman : MonoBehaviour
{
	// Token: 0x06000669 RID: 1641 RVA: 0x0004C75C File Offset: 0x0004A95C
	public void BuyItemD(string itemName, float spawnPrice, GameObject buyObject)
	{
		this.disableObj = buyObject;
		float num = Vector3.Distance(this.npc.transform.position, this.truck.transform.position);
		float num2 = Vector3.Distance(this.npc.transform.position, this.i4.position);
		this.price = spawnPrice;
		if (itemName.Contains("plugs4pack"))
		{
			if (num2 < this.minDist)
			{
				this.truck.fastStart = true;
				this.Pay();
			}
		}
		else if (itemName.Contains("dashboard_digitalD"))
		{
			Debug.Log(num);
			if (num < this.minDist)
			{
				this.truck.isDigital = true;
				this.truck.digitalDisplay.SetActive(true);
				this.truck.pointerFuel.GetComponent<Renderer>().enabled = false;
				this.truck.pointerTemp.GetComponent<Renderer>().enabled = false;
				this.truck.tachNeedle.GetComponent<Renderer>().enabled = false;
				this.truck.speedNeedle.GetComponent<Renderer>().enabled = false;
				this.truck.rgbDial.SetActive(true);
				this.truck.DarkenDash();
				this.Pay();
			}
		}
		else if (itemName.Contains("seat_after"))
		{
			if (num < this.minDist)
			{
				this.truck.passSeat.SetActive(true);
				this.truck.hasSeat = true;
				this.Pay();
			}
		}
		else if (itemName.Contains("interiormatD"))
		{
			if (num < this.minDist)
			{
				this.truck.cleanInterior = true;
				this.truck.interior.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", Color.black);
				this.Pay();
			}
		}
		else if (itemName.Contains("deadmatD"))
		{
			if (num < this.minDist)
			{
				this.truck.soundMat = true;
				this.truck.lDoor.clip = this.truck.heavyDoor[0];
				this.truck.rDoor.clip = this.truck.heavyDoor[0];
				this.Pay();
			}
		}
		else if (itemName.Contains("tuneupi4"))
		{
			if (num < this.minDist)
			{
				this.truck.fuelSaving = true;
				this.Pay();
			}
		}
		else if (itemName.Contains("difflockD"))
		{
			if (num < this.minDist)
			{
				this.truck.hasDiffLock = true;
				this.truck.diffLockButton.SetActive(true);
				this.truck.diffLock1.SetActive(true);
				this.truck.diffLock2.SetActive(true);
				this.Pay();
			}
		}
		else if (itemName.Contains("5thgearD"))
		{
			if (num < this.minDist)
			{
				this.truck.has5th = true;
				this.Pay();
			}
		}
		else if (itemName.Contains("kclightD"))
		{
			if (num < this.minDist)
			{
				this.truck.kcSwitch.SetActive(true);
				this.truck.hasRollbar = true;
				this.truck.rollbar.SetActive(true);
				this.Pay();
			}
		}
		else if (itemName.Contains("xltire"))
		{
			this.Say(10);
		}
		if (num >= this.minDist)
		{
			this.BringTruck();
		}
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x0004CAE0 File Offset: 0x0004ACE0
	public void BuyItemF(string itemName, float spawnPrice, GameObject buyObject)
	{
		this.disableObj = buyObject;
		float num = Vector3.Distance(this.npc.transform.position, this.truck2.transform.position);
		float num2 = Vector3.Distance(this.npc.transform.position, this.v8.position);
		this.price = spawnPrice;
		if (itemName.Contains("plugs8pack"))
		{
			if (num2 < this.minDist)
			{
				this.truck2.fastStart = true;
				this.Pay();
			}
		}
		else if (itemName.Contains("dashboard_digitalF"))
		{
			if (num < this.minDist)
			{
				this.truck2.isDigital = true;
				this.truck2.digitalDisplay.SetActive(true);
				this.truck2.rgbDial.SetActive(true);
				this.truck2.DarkenDash();
				this.Pay();
			}
		}
		else if (itemName.Contains("interiormatF"))
		{
			if (num < this.minDist)
			{
				this.truck2.cleanInterior = true;
				this.truck2.interior.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", Color.black);
				this.Pay();
			}
		}
		else if (itemName.Contains("deadmatF"))
		{
			if (num < this.minDist)
			{
				this.truck2.soundMat = true;
				this.truck2.lDoor.clip = this.truck.heavyDoor[0];
				this.truck2.rDoor.clip = this.truck.heavyDoor[0];
				this.Pay();
			}
		}
		else if (itemName.Contains("tuneupv8"))
		{
			if (num2 < this.minDist)
			{
				this.truck2.fuelSaving = true;
				this.Pay();
			}
		}
		else if (itemName.Contains("difflockF"))
		{
			if (num < this.minDist)
			{
				this.truck2.hasDiffLock = true;
				this.truck2.diffLockButton.SetActive(true);
				this.truck2.diffLock1.SetActive(true);
				this.truck2.diffLock2.SetActive(true);
				this.Pay();
			}
		}
		else if (itemName.Contains("5thgearF"))
		{
			if (num < this.minDist)
			{
				this.truck2.has5th = true;
				this.Pay();
			}
		}
		else if (itemName.Contains("kclightF") && num < this.minDist)
		{
			this.truck2.kcSwitch.SetActive(true);
			this.truck2.hasRollbar = true;
			this.truck2.rollbar.SetActive(true);
			this.Pay();
		}
		if (num >= this.minDist)
		{
			this.BringTruck();
		}
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x0004CDA0 File Offset: 0x0004AFA0
	private void BringTruck()
	{
		int voiceNum = Random.Range(0, 3);
		this.Say(voiceNum);
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x0004CDBC File Offset: 0x0004AFBC
	public void Greet()
	{
		int voiceNum = Random.Range(13, 18);
		if (Random.Range(0, 3) == 0)
		{
			this.Say(voiceNum);
		}
		else
		{
			List<int> list = new List<int>();
			if (Vector3.Distance(this.npc.transform.position, this.eagle.position) < this.minDist && !this.carDialogue)
			{
				this.carDialogue = true;
				list.Add(4);
			}
			if (EnviroSkyMgr.instance.GetTimeOfDay() < 6f)
			{
				list.Add(5);
			}
			if (this.curr.drunk > 1)
			{
				list.Add(8);
			}
			int num = 0;
			using (IEnumerator enumerator = this.shopInventory.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (((Transform)enumerator.Current).gameObject.active)
					{
						num++;
					}
				}
			}
			if (num < 3)
			{
				list.Add(Random.Range(6, 8));
			}
			if (this.mwb.jobNum > 9)
			{
				list.Add(19);
			}
			if (list.Count > 0 && Random.Range(0, 2) == 0)
			{
				int voiceNum2 = list[Random.Range(0, list.Count)];
				this.Say(voiceNum2);
			}
		}
		base.StartCoroutine(this.IdleTalk());
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x0004CF18 File Offset: 0x0004B118
	public void NoMoney()
	{
		if (Random.Range(0, 3) == 1)
		{
			this.Say(9);
		}
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x0004CF2C File Offset: 0x0004B12C
	private IEnumerator IdleTalk()
	{
		if (!this.idling)
		{
			this.idling = true;
			yield return new WaitForSeconds(60f);
			if (this.chime.inStore)
			{
				int[] array = new int[]
				{
					11,
					17,
					20
				};
				int voiceNum = array[Random.Range(0, array.Length)];
				this.Say(voiceNum);
				this.idling = false;
			}
		}
		yield break;
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x0004CF3B File Offset: 0x0004B13B
	private void Say(int voiceNum)
	{
		if (!this.aSource.isPlaying)
		{
			this.aSource.clip = this.aClip[voiceNum];
			this.aSource.Play();
		}
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x0004CF68 File Offset: 0x0004B168
	private void Pay()
	{
		this.fps.GetComponent<Interactor>().inv.SubtractMoney(this.price);
		float num = this.eventSystem.GetComponent<Currency>().money - this.price;
		this.eventSystem.GetComponent<Currency>().money = Mathf.Round(num * 100f) / 100f;
		this.disableObj.SetActive(false);
		this.playSound = Random.Range(0, 6);
		if (this.playSound == 3)
		{
			this.Say(3);
		}
		else if (this.playSound == 4)
		{
			this.Say(17);
		}
		this.aSource2.Play();
	}

	// Token: 0x04000D5E RID: 3422
	public GameObject npc;

	// Token: 0x04000D5F RID: 3423
	public Currency curr;

	// Token: 0x04000D60 RID: 3424
	public car truck;

	// Token: 0x04000D61 RID: 3425
	public car4 truck2;

	// Token: 0x04000D62 RID: 3426
	public GameObject fps;

	// Token: 0x04000D63 RID: 3427
	public GameObject eventSystem;

	// Token: 0x04000D64 RID: 3428
	public AudioSource aSource;

	// Token: 0x04000D65 RID: 3429
	public AudioSource aSource2;

	// Token: 0x04000D66 RID: 3430
	public AudioClip[] aClip;

	// Token: 0x04000D67 RID: 3431
	private float price;

	// Token: 0x04000D68 RID: 3432
	public Transform i4;

	// Token: 0x04000D69 RID: 3433
	public Transform v8;

	// Token: 0x04000D6A RID: 3434
	public Transform eagle;

	// Token: 0x04000D6B RID: 3435
	public float minDist = 50f;

	// Token: 0x04000D6C RID: 3436
	public Transform shopInventory;

	// Token: 0x04000D6D RID: 3437
	public DoorChime chime;

	// Token: 0x04000D6E RID: 3438
	private GameObject disableObj;

	// Token: 0x04000D6F RID: 3439
	private bool carDialogue;

	// Token: 0x04000D70 RID: 3440
	private bool idling;

	// Token: 0x04000D71 RID: 3441
	private int playSound;

	// Token: 0x04000D72 RID: 3442
	private int Count;

	// Token: 0x04000D73 RID: 3443
	public ModWomanJobs mwb;
}
