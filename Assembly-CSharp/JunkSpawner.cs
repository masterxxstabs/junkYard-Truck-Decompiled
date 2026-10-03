using System;
using UnityEngine;

// Token: 0x020000DB RID: 219
public class JunkSpawner : MonoBehaviour
{
	// Token: 0x06000585 RID: 1413 RVA: 0x00045368 File Offset: 0x00043568
	private void Start()
	{
		this.DailyParts();
		this.DailyBikeParts();
		this.SpawnV8();
		foreach (object obj in this.junkContainer.transform)
		{
			Transform transform = (Transform)obj;
			if (transform.gameObject.name.Contains("truckwheel") && transform.gameObject.GetComponent<TireAssign>().tireNumX == 0)
			{
				Object.Destroy(transform.gameObject);
			}
		}
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x00045408 File Offset: 0x00043608
	private void SpawnV8()
	{
		if (!this.spawnedv8s)
		{
			this.spawnedv8s = true;
			Object.Instantiate<GameObject>(this.v8junk, this.v8transform[0].position, this.v8transform[0].rotation);
			Object.Instantiate<GameObject>(this.v8junk, this.v8transform[1].position, this.v8transform[1].rotation);
			Object.Instantiate<GameObject>(this.v8junk, this.v8transform[2].position, this.v8transform[2].rotation);
			Object.Instantiate<GameObject>(this.v8junk, this.v8transform[3].position, this.v8transform[3].rotation);
			Object.Instantiate<GameObject>(this.v8junk, this.v8transform[4].position, this.v8transform[4].rotation);
		}
	}

	// Token: 0x06000587 RID: 1415 RVA: 0x000454E8 File Offset: 0x000436E8
	public void NewPart()
	{
		this.quality = (float)Random.Range(65, 90);
		this.rand = Random.Range(0, this.spawnItems.Length);
		if (this.spawned18 && this.rand == 42)
		{
			this.rand = Random.Range(0, 41);
		}
		this.rot = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
		this.price = Mathf.Round(this.spawnItems[this.rand].GetComponent<PickUp>().price / (float)Random.Range(3, 6)) + 5f;
		this.spawnPosition = new Vector3(base.transform.position.x + Random.Range(0f, 50f), base.transform.position.y, base.transform.position.z + Random.Range(0f, -47f));
		this.spawned = Object.Instantiate<GameObject>(this.spawnItems[this.rand], this.spawnPosition, this.rot);
		this.spawned.GetComponent<PickUp>().price = this.price;
		this.spawned.GetComponent<PickUp>().thisDurability = this.quality;
		this.spawned.GetComponent<PickUp>().pickable = false;
		if (this.rand != 42)
		{
			this.spawned.transform.SetParent(this.junkContainer.transform);
		}
		if (this.rand == 42)
		{
			this.spawned18 = true;
		}
		if (this.spawned.name.Contains("enginelow3"))
		{
			this.spawned.transform.position = this.oldEnginePos.position;
			this.spawned.GetComponent<PickUp>().price = (float)Random.Range(210, 255);
			this.spawned.GetComponent<PickUp>().thisDurability = (float)Random.Range(94, 100);
		}
		if (this.spawned.name.Contains("truckwheel"))
		{
			this.spawned.GetComponent<TireAssign>().tireNumX = 6;
			this.spawned.GetComponent<TireAssign>().Start();
		}
	}

	// Token: 0x06000588 RID: 1416 RVA: 0x00045738 File Offset: 0x00043938
	public void NewBikePart()
	{
		this.quality = 100f;
		this.rand = Random.Range(0, this.spawn250Items.Length);
		this.rot = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
		this.price = this.spawn250Items[this.rand].GetComponent<PickUp>().price;
		this.spawnPosition = new Vector3(this.bikeContainer.transform.position.x + Random.Range(0f, 1f), this.bikeContainer.transform.position.y, this.bikeContainer.transform.position.z + Random.Range(0f, -1f));
		this.spawned = Object.Instantiate<GameObject>(this.spawn250Items[this.rand], this.spawnPosition, this.rot);
		this.spawned.GetComponent<PickUp>().price = this.price;
		this.spawned.GetComponent<PickUp>().thisDurability = this.quality;
		this.spawned.GetComponent<PickUp>().pickable = false;
		this.spawned.transform.SetParent(this.bikeContainer.transform);
	}

	// Token: 0x06000589 RID: 1417 RVA: 0x0004589F File Offset: 0x00043A9F
	public void DailyParts()
	{
		this.CleanParts();
		this.SpawnNewParts();
	}

	// Token: 0x0600058A RID: 1418 RVA: 0x000458AD File Offset: 0x00043AAD
	public void DailyBikeParts()
	{
		this.CleanBikeParts();
		this.SpawnBikeParts();
	}

	// Token: 0x0600058B RID: 1419 RVA: 0x000458BC File Offset: 0x00043ABC
	public void CleanParts()
	{
		if (this.junkContainer.transform.childCount > 20)
		{
			this.i = 0;
			while (this.i < 10)
			{
				Object.Destroy(this.junkContainer.transform.GetChild(this.i).gameObject);
				this.i++;
			}
		}
	}

	// Token: 0x0600058C RID: 1420 RVA: 0x0004591E File Offset: 0x00043B1E
	public void SpawnNewParts()
	{
		if (this.junkContainer.transform.childCount < 50)
		{
			this.i = 0;
			while (this.i < 10)
			{
				this.NewPart();
				this.i++;
			}
		}
	}

	// Token: 0x0600058D RID: 1421 RVA: 0x0004595C File Offset: 0x00043B5C
	public void CleanBikeParts()
	{
		if (this.bikeContainer.transform.childCount > 8)
		{
			this.i = 0;
			while (this.i < 3)
			{
				Object.Destroy(this.bikeContainer.transform.GetChild(this.i).gameObject);
				this.i++;
			}
		}
	}

	// Token: 0x0600058E RID: 1422 RVA: 0x000459BC File Offset: 0x00043BBC
	public void SpawnBikeParts()
	{
		if (this.bikeContainer.transform.childCount < 10)
		{
			this.i = 0;
			while (this.i < 3)
			{
				this.NewBikePart();
				this.i++;
			}
		}
	}

	// Token: 0x04000BF4 RID: 3060
	private Vector3 spawnPosition;

	// Token: 0x04000BF5 RID: 3061
	private Quaternion rot;

	// Token: 0x04000BF6 RID: 3062
	public GameObject[] spawnItems;

	// Token: 0x04000BF7 RID: 3063
	private float quality;

	// Token: 0x04000BF8 RID: 3064
	public GameObject junkContainer;

	// Token: 0x04000BF9 RID: 3065
	private int rand;

	// Token: 0x04000BFA RID: 3066
	private GameObject spawned;

	// Token: 0x04000BFB RID: 3067
	private int i;

	// Token: 0x04000BFC RID: 3068
	private float price;

	// Token: 0x04000BFD RID: 3069
	public GameObject bikeContainer;

	// Token: 0x04000BFE RID: 3070
	public GameObject[] spawn250Items;

	// Token: 0x04000BFF RID: 3071
	public bool spawned18;

	// Token: 0x04000C00 RID: 3072
	private bool spawnedv8s;

	// Token: 0x04000C01 RID: 3073
	public Transform[] v8transform;

	// Token: 0x04000C02 RID: 3074
	public GameObject v8junk;

	// Token: 0x04000C03 RID: 3075
	public Transform oldEnginePos;
}
