using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200012F RID: 303
public class MissionGen : MonoBehaviour
{
	// Token: 0x060007DC RID: 2012 RVA: 0x00002188 File Offset: 0x00000388
	public void Start()
	{
	}

	// Token: 0x060007DD RID: 2013 RVA: 0x00065157 File Offset: 0x00063357
	public void TryCreateMission()
	{
		base.StartCoroutine(this.CreateMissionDelay());
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x00065166 File Offset: 0x00063366
	private IEnumerator CreateMissionDelay()
	{
		if (this.canGenerate)
		{
			this.corkboard.description = "Please Wait...";
			this.canGenerate = false;
			this.CreateMission();
		}
		yield return new WaitForSeconds(8f);
		this.canGenerate = true;
		this.corkboard.description = "Find New Mission";
		yield break;
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x00065178 File Offset: 0x00063378
	public void CreateMission()
	{
		this.DestroySpawned();
		this.TrackerXs.GetComponent<showmission>().manualNextActivated = false;
		this.missionType = Random.Range(1, 12);
		if (this.missionType == 12)
		{
			this.missionType = 11;
		}
		if (this.missionType == this.lastMission)
		{
			this.missionType = Random.Range(1, 12);
		}
		if ((this.missionType == 1 || this.missionType == 3 || this.missionType == 8 || this.missionType == 9 || this.missionType == 10) && Random.Range(1, 12) > 4)
		{
			this.missionType = Random.Range(1, 12);
			if (this.missionType == this.lastMission)
			{
				this.missionType = Random.Range(1, 12);
			}
		}
		this.lastMission = this.missionType;
		this.mc.activeMissions.Remove(49);
		if (this.missionType == 1)
		{
			this.part = this.lostPart[Random.Range(0, this.lostPart.Length)];
			this.xpos = Random.Range(100f, 1400f);
			this.zpos = Random.Range(100f, 1400f);
			Vector3 vector = new Vector3(this.xpos, 0f, this.zpos);
			this.xPosF = vector.x / this.terrainOb.terrainData.size.x;
			this.zPosF = vector.z / this.terrainOb.terrainData.size.z;
			this.spawnPosition = new Vector3(this.xpos, this.terrainOb.terrainData.GetInterpolatedHeight(this.xPosF, this.zPosF) + 0.7f, this.zpos);
			this.spawned = Object.Instantiate<GameObject>(this.part);
			this.spawned.transform.position = this.spawnPosition;
			this.missionPart = this.spawned;
			this.missionPart.name = "missionPart_1";
			this.TrackerXs.position = this.spawnPosition;
			int num = Random.Range(0, this.driveway.Length);
			this.TrackerXf.position = this.driveway[num].position;
			this.mc.specialReq = "FIND AND DELIVER A LOST ENGINE PART.";
			this.mc.ActivateMission(49);
			this.startTrigger.SetActive(true);
			this.startTrigger.transform.position = this.spawnPosition;
			this.endTrigger.SetActive(true);
			this.startTrigger.GetComponent<DynamicTrigger>().pickUpBased = true;
			this.startTrigger.GetComponent<DynamicTrigger>().proximityBased = false;
			this.startTrigger.GetComponent<DynamicTrigger>().waypointed = false;
			this.endTrigger.transform.position = this.driveway[num].position;
		}
		if (this.missionType == 2)
		{
			Transform transform = this.raceWaypoints[Random.Range(0, this.raceWaypoints.Length)];
			this.spawnPosition = new Vector3(transform.position.x, transform.position.y + 10f, transform.position.z);
			this.spawned = Object.Instantiate<GameObject>(this.CuttableTreeDown, this.spawnPosition, Random.rotation);
			this.TrackerXs.position = this.spawnPosition;
			this.TrackerXf.position = this.forgeLoc.position;
			this.mc.specialReq = "CUT UP FALLEN TREE AND DELIVER TO FORGE.";
			this.mc.ActivateMission(49);
			this.numParts = 2;
		}
		if (this.missionType == 3)
		{
			this.part = this.jyObj[Random.Range(0, this.lostPart.Length)];
			this.xpos = Random.Range(968f, 1016f);
			this.zpos = Random.Range(1206f, 1252f);
			Vector3 vector2 = new Vector3(this.xpos, 0f, this.zpos);
			this.xPosF = vector2.x / this.terrainOb.terrainData.size.x;
			this.zPosF = vector2.z / this.terrainOb.terrainData.size.z;
			this.spawnPosition = new Vector3(this.xpos, this.terrainOb.terrainData.GetInterpolatedHeight(this.xPosF, this.zPosF) + 0.7f, this.zpos);
			this.spawned = Object.Instantiate<GameObject>(this.part);
			this.spawned.transform.position = this.spawnPosition;
			this.missionPart = this.spawned;
			this.missionPart.name = "missionPart_3";
			this.TrackerXs.position = this.spawnPosition;
			this.TrackerXf.position = this.jimmyDesk.position;
			this.mc.specialReq = "FIND AND DELIVER A JUNKYARD PART TO JIMMY JUNKS.";
			this.mc.ActivateMission(49);
			this.startTrigger.SetActive(true);
			this.startTrigger.transform.position = this.spawnPosition;
			this.endTrigger.SetActive(true);
			this.startTrigger.GetComponent<DynamicTrigger>().pickUpBased = true;
			this.startTrigger.GetComponent<DynamicTrigger>().proximityBased = false;
			this.startTrigger.GetComponent<DynamicTrigger>().waypointed = false;
			this.endTrigger.transform.position = this.jimmyDesk.position;
		}
		if (this.missionType == 4)
		{
			this.numParts = Random.Range(1, 6);
			int num2 = Random.Range(0, this.constructionPart.Length);
			this.part = this.constructionPart[num2];
			if (num2 == 0)
			{
				this.numParts = Random.Range(5, 10);
			}
			else if (num2 == 1)
			{
				this.numParts = Random.Range(1, 3);
			}
			else if (num2 == 2)
			{
				this.numParts = Random.Range(1, 4);
			}
			int num3 = Random.Range(0, this.warehouseLoc.Length);
			this.spawnPosition = this.warehouseLoc[num3].position;
			base.StartCoroutine(this.SpawnMultiple());
			this.TrackerXs.position = this.spawnPosition;
			int num4 = Random.Range(0, this.constructionPoi.Length);
			this.TrackerXf.position = this.constructionPoi[num4].position;
			this.mc.specialReq = "PICK UP AND DELIVER CONSTRUCTION MATERIALS.";
			this.mc.ActivateMission(49);
			this.startTrigger.SetActive(true);
			this.startTrigger.transform.position = this.spawnPosition;
			this.endTrigger.SetActive(true);
			this.startTrigger.GetComponent<DynamicTrigger>().pickUpBased = true;
			this.startTrigger.GetComponent<DynamicTrigger>().proximityBased = false;
			this.startTrigger.GetComponent<DynamicTrigger>().waypointed = false;
			this.endTrigger.transform.position = this.constructionPoi[num4].position;
		}
		if (this.missionType == 5)
		{
			this.part = this.cuttableTreeXL;
			this.xpos = Random.Range(200f, 1300f);
			this.zpos = Random.Range(200f, 1300f);
			Vector3 vector3 = new Vector3(this.xpos, 0f, this.zpos);
			this.xPosF = vector3.x / this.terrainOb.terrainData.size.x;
			this.zPosF = vector3.z / this.terrainOb.terrainData.size.z;
			this.spawnPosition = new Vector3(this.xpos, this.terrainOb.terrainData.GetInterpolatedHeight(this.xPosF, this.zPosF) + 0.7f, this.zpos);
			float num5 = Vector3.Distance(this.spawnPosition, this.cityLoc.position);
			float num6 = Vector3.Distance(this.spawnPosition, this.forgeLoc.position);
			float num7 = Vector3.Distance(this.spawnPosition, this.townLoc.position);
			float num8 = Vector3.Distance(this.spawnPosition, this.jyLoc.position);
			float num9 = Vector3.Distance(this.spawnPosition, this.mygLoc.position);
			if (num5 < 75f || num6 < 25f || num7 < 100f || num8 < 75f || num9 < 50f)
			{
				this.xpos = Random.Range(100f, 1400f);
				this.zpos = Random.Range(100f, 1400f);
				Vector3 vector4 = new Vector3(this.xpos, 0f, this.zpos);
				this.xPosF = vector4.x / this.terrainOb.terrainData.size.x;
				this.zPosF = vector4.z / this.terrainOb.terrainData.size.z;
				this.spawnPosition = new Vector3(this.xpos, this.terrainOb.terrainData.GetInterpolatedHeight(this.xPosF, this.zPosF) + 0.7f, this.zpos);
			}
			this.spawned = Object.Instantiate<GameObject>(this.part);
			this.spawned.transform.position = this.spawnPosition;
			this.missionPart = this.spawned;
			this.TrackerXs.position = this.spawnPosition;
			this.TrackerXf.position = this.forgeLoc.position;
			this.mc.specialReq = "CUT DOWN A MASSIVE TREE AND DELIVER IT TO THE FORGE.";
			this.mc.ActivateMission(49);
		}
		if (this.missionType == 6)
		{
			this.part = this.dumpObj[Random.Range(0, this.dumpObj.Length)];
			this.xpos = Random.Range(300f, 1200f);
			this.zpos = Random.Range(300f, 1200f);
			Vector3 vector5 = new Vector3(this.xpos, 0f, this.zpos);
			this.xPosF = vector5.x / this.terrainOb.terrainData.size.x;
			this.zPosF = vector5.z / this.terrainOb.terrainData.size.z;
			this.numParts = Random.Range(4, 8);
			this.spawnPosition = new Vector3(this.xpos, this.terrainOb.terrainData.GetInterpolatedHeight(this.xPosF, this.zPosF) + 0.7f, this.zpos);
			base.StartCoroutine(this.SpawnMultiple());
			this.TrackerXs.position = this.spawnPosition;
			this.TrackerXf.position = this.dumpLoc.position;
			this.startTrigger.SetActive(true);
			this.startTrigger.transform.position = this.spawnPosition;
			this.endTrigger.SetActive(true);
			this.startTrigger.GetComponent<DynamicTrigger>().pickUpBased = true;
			this.startTrigger.GetComponent<DynamicTrigger>().proximityBased = false;
			this.startTrigger.GetComponent<DynamicTrigger>().waypointed = false;
			this.endTrigger.transform.position = this.dumpLoc.position;
			this.mc.specialReq = "CLEAN UP THE ILLEGAL DUMP SITE.";
			this.mc.ActivateMission(49);
		}
		if (this.missionType == 7)
		{
			this.part = this.recycObj[Random.Range(0, this.recycObj.Length)];
			this.xpos = Random.Range(200f, 1300f);
			this.zpos = Random.Range(200f, 1300f);
			Vector3 vector6 = new Vector3(this.xpos, 0f, this.zpos);
			this.xPosF = vector6.x / this.terrainOb.terrainData.size.x;
			this.zPosF = vector6.z / this.terrainOb.terrainData.size.z;
			this.spawnPosition = new Vector3(this.xpos, this.terrainOb.terrainData.GetInterpolatedHeight(this.xPosF, this.zPosF) + 0.7f, this.zpos);
			this.spawned = Object.Instantiate<GameObject>(this.part, this.spawnPosition, Quaternion.identity);
			this.spawned.name = "missionPart_7";
			this.TrackerXs.position = this.spawnPosition;
			this.TrackerXf.position = this.jyLoc.position;
			this.missionPart = this.spawned;
			this.mc.specialReq = "FIND THE HEAVY PART AND DELIVER IT TO THE JUNKYARD.";
			this.mc.ActivateMission(49);
			this.startTrigger.SetActive(true);
			this.startTrigger.transform.position = this.spawnPosition;
			this.endTrigger.SetActive(true);
			this.startTrigger.GetComponent<DynamicTrigger>().pickUpBased = true;
			this.startTrigger.GetComponent<DynamicTrigger>().proximityBased = false;
			this.startTrigger.GetComponent<DynamicTrigger>().waypointed = false;
			this.endTrigger.transform.position = this.jyLoc.position;
		}
		if (this.missionType == 8)
		{
			this.part = this.meteor;
			this.xpos = Random.Range(100f, 1400f);
			this.zpos = Random.Range(100f, 1400f);
			Vector3 vector7 = new Vector3(this.xpos, 0f, this.zpos);
			this.xPosF = vector7.x / this.terrainOb.terrainData.size.x;
			this.zPosF = vector7.z / this.terrainOb.terrainData.size.z;
			this.spawnPosition = new Vector3(this.xpos, this.terrainOb.terrainData.GetInterpolatedHeight(this.xPosF, this.zPosF) + 0.7f, this.zpos);
			this.spawned = Object.Instantiate<GameObject>(this.part);
			this.spawned.transform.position = this.spawnPosition;
			this.missionPart = this.spawned;
			this.missionPart.name = "missionPart_8";
			this.TrackerXs.position = this.spawnPosition;
			Random.Range(0, this.driveway.Length);
			this.TrackerXf.position = this.jiggsLoc.position;
			this.mc.specialReq = "LOCATE THE FALLEN METEORITE AND DELIVER IT TO JIGGS CASEY.";
			this.mc.ActivateMission(49);
			this.startTrigger.SetActive(true);
			this.startTrigger.transform.position = this.spawnPosition;
			this.endTrigger.SetActive(true);
			this.startTrigger.GetComponent<DynamicTrigger>().pickUpBased = true;
			this.startTrigger.GetComponent<DynamicTrigger>().proximityBased = false;
			this.startTrigger.GetComponent<DynamicTrigger>().waypointed = false;
			this.endTrigger.transform.position = this.jiggsLoc.position;
		}
		if (this.missionType == 9)
		{
			this.part = this.blackbox;
			this.xpos = Random.Range(200f, 1300f);
			this.zpos = Random.Range(200f, 1300f);
			Vector3 vector8 = new Vector3(this.xpos, 0f, this.zpos);
			this.xPosF = vector8.x / this.terrainOb.terrainData.size.x;
			this.zPosF = vector8.z / this.terrainOb.terrainData.size.z;
			this.spawnPosition = new Vector3(this.xpos, this.terrainOb.terrainData.GetInterpolatedHeight(this.xPosF, this.zPosF) + 0.7f, this.zpos);
			this.spawned = Object.Instantiate<GameObject>(this.part);
			this.spawned.transform.position = this.spawnPosition;
			this.missionPart = this.spawned;
			this.missionPart.name = "missionPart_9";
			this.TrackerXs.position = this.spawnPosition;
			this.TrackerXf.position = this.flightDeliveryLoc.position;
			this.mc.specialReq = "FIND AND DELIVER THE CRASHED AIRCRAFT PART.";
			this.mc.ActivateMission(49);
			this.startTrigger.SetActive(true);
			this.startTrigger.transform.position = this.spawnPosition;
			this.endTrigger.SetActive(true);
			this.startTrigger.GetComponent<DynamicTrigger>().pickUpBased = true;
			this.startTrigger.GetComponent<DynamicTrigger>().proximityBased = false;
			this.startTrigger.GetComponent<DynamicTrigger>().waypointed = false;
			this.endTrigger.transform.position = this.flightDeliveryLoc.position;
		}
		if (this.missionType == 10)
		{
			this.part = this.RWGtruck;
			this.xpos = Random.Range(100f, 1400f);
			this.zpos = Random.Range(100f, 1400f);
			Vector3 vector9 = new Vector3(this.xpos, 0f, this.zpos);
			this.xPosF = vector9.x / this.terrainOb.terrainData.size.x;
			this.zPosF = vector9.z / this.terrainOb.terrainData.size.z;
			this.spawnPosition = new Vector3(this.xpos, this.terrainOb.terrainData.GetInterpolatedHeight(this.xPosF, this.zPosF) + 0.7f, this.zpos);
			this.spawned = Object.Instantiate<GameObject>(this.part, this.spawnPosition, Quaternion.identity);
			this.spawned.name = "RWGtruck";
			this.rwgfuel = this.spawned.GetComponent<RWGfuel>();
			this.TrackerXs.position = this.spawnPosition;
			this.TrackerXf.position = this.spawnPosition;
			this.mc.specialReq = "[JERRY CAN REQUIRED] DELIVER SOME FUEL TO THE STRANDED OFF ROAD TRUCK.";
			this.mc.ActivateMission(49);
		}
		if (this.missionType == 11)
		{
			this.numParts = 1;
			int num10 = Random.Range(0, this.heavyObj.Length);
			this.part = this.heavyObj[num10];
			if (num10 == 0)
			{
				this.xpos = Random.Range(200f, 1300f);
				this.zpos = Random.Range(200f, 1300f);
				Vector3 vector10 = new Vector3(this.xpos, 0f, this.zpos);
				this.xPosF = vector10.x / this.terrainOb.terrainData.size.x;
				this.zPosF = vector10.z / this.terrainOb.terrainData.size.z;
				this.spawnPosition = new Vector3(this.xpos, this.terrainOb.terrainData.GetInterpolatedHeight(this.xPosF, this.zPosF) + 0.7f, this.zpos);
				this.deliveryLoc = 0;
			}
			else
			{
				switch (num10)
				{
				case 1:
					this.locArr = new int[]
					{
						0,
						2,
						3,
						4,
						5,
						6,
						7,
						8,
						16
					};
					break;
				case 2:
					this.locArr = new int[]
					{
						1,
						2,
						3,
						4,
						4,
						5,
						16
					};
					break;
				case 3:
					this.locArr = new int[]
					{
						1,
						16,
						5,
						2
					};
					break;
				case 4:
					this.locArr = new int[]
					{
						0,
						1,
						2,
						3,
						6,
						7,
						8,
						16
					};
					break;
				case 5:
					this.locArr = new int[]
					{
						9,
						10,
						11,
						12,
						13,
						14,
						15,
						5,
						3
					};
					break;
				case 6:
					this.locArr = new int[]
					{
						0,
						1,
						2,
						3,
						5,
						6,
						7,
						8,
						14,
						15
					};
					break;
				}
				int num11 = this.locArr[Random.Range(0, this.locArr.Length)];
				this.spawnPosition = this.heavyLoc[num11].position;
				this.deliveryLoc = num11;
				while (this.deliveryLoc == num11)
				{
					this.deliveryLoc = Random.Range(0, this.locArr.Length);
				}
			}
			this.spawned = Object.Instantiate<GameObject>(this.part, this.spawnPosition, Quaternion.identity);
			if (this.heavyObj[num10].name.Contains("watercrate"))
			{
				Vector3 position = new Vector3(this.spawnPosition.x + 3f, this.spawnPosition.y, this.spawnPosition.z);
				Vector3 position2 = new Vector3(this.spawnPosition.x - 3f, this.spawnPosition.y, this.spawnPosition.z);
				this.spawned = Object.Instantiate<GameObject>(this.part, position, Quaternion.identity);
				this.spawned.name = "heavy_watercrate2";
				this.spawned = Object.Instantiate<GameObject>(this.part, position2, Quaternion.identity);
				this.spawned.name = "heavy_watercrate3";
			}
			else if (this.heavyObj[num10].name.Contains("pipe"))
			{
				this.spawned = Object.Instantiate<GameObject>(this.part, this.spawnPosition, Quaternion.identity);
				this.spawned.name = "heavy_pipe2";
			}
			this.missionPart = this.spawned;
			this.TrackerXs.position = this.spawnPosition;
			this.TrackerXf.position = this.heavyLoc[this.deliveryLoc].position;
			this.mc.specialReq = "[FLATBED TRAILER REQUIRED] FIND AND DELIVER THE MASSIVELY HEAVY CARGO.";
			this.mc.ActivateMission(49);
			this.startTrigger.SetActive(true);
			this.startTrigger.transform.position = this.spawnPosition;
			this.endTrigger.SetActive(true);
			this.startTrigger.GetComponent<DynamicTrigger>().pickUpBased = true;
			this.startTrigger.GetComponent<DynamicTrigger>().proximityBased = false;
			this.startTrigger.GetComponent<DynamicTrigger>().waypointed = false;
			this.endTrigger.transform.position = this.heavyLoc[this.deliveryLoc].position;
		}
		this.mc.ShowMissionTrackTool(49);
		base.StartCoroutine(this.UpdateTrackers());
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x00066835 File Offset: 0x00064A35
	private IEnumerator UpdateTrackers()
	{
		yield return new WaitForSeconds(5f);
		this.TrackerXs.position = this.spawned.transform.position;
		this.TrackerXs.position = this.spawnPosition;
		Vector3 position = this.TrackerXf.position;
		if (this.missionType == 1 || this.missionType == 8)
		{
			float num = Random.Range(-10f, 10f);
			float num2 = Random.Range(-10f, 10f);
			Debug.Log(num);
			this.TrackerXs.position = new Vector3(this.TrackerXs.position.x + num, this.TrackerXs.position.y, this.TrackerXs.position.z + num2);
			this.TrackerXf.position = position;
		}
		else if (this.missionType == 9)
		{
			float num3 = Random.Range(-35f, 35f);
			float num4 = Random.Range(-35f, 35f);
			Debug.Log(num3);
			this.TrackerXs.position = new Vector3(this.TrackerXs.position.x + num3, this.TrackerXs.position.y, this.TrackerXs.position.z + num4);
			this.TrackerXf.position = position;
		}
		yield break;
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x00066844 File Offset: 0x00064A44
	private IEnumerator SpawnMultiple()
	{
		int num;
		for (int i = 0; i < this.numParts; i = num + 1)
		{
			this.spawned = Object.Instantiate<GameObject>(this.part);
			this.spawned.transform.position = this.spawnPosition;
			this.missionPart = this.spawned;
			this.missionPart.name = "missionPart_4";
			yield return new WaitForSeconds(1f);
			num = i;
		}
		yield break;
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x00066853 File Offset: 0x00064A53
	public void NextTracker()
	{
		this.missionTracker.GetComponent<showmission>().RemoveWaypoint();
		this.missionTracker.GetComponent<showmission>().manualNextActivated = true;
		this.missionTrackerB.GetComponent<showmission>().AddWaypoint();
	}

	// Token: 0x060007E3 RID: 2019 RVA: 0x00066888 File Offset: 0x00064A88
	public void CheckCompletion(GameObject other)
	{
		if (this.missionType == 1 || this.missionType == 3 || this.missionType == 7 || this.missionType == 8 || this.missionType == 9)
		{
			if (other == this.missionPart)
			{
				this.mc.CompleteMission(49);
				this.spawned.GetComponent<PickUp>().LetGo(0);
				Object.Destroy(this.spawned);
				float num = Vector3.Distance(this.TrackerXf.transform.position, this.TrackerXs.transform.position) / 5f;
				float num2 = 100f + Mathf.Round(num * 100f) / 100f;
				if (this.missionType == 3)
				{
					num2 = 50f + Mathf.Round(num * 100f) / 100f;
				}
				this.atmScript.balance += num2;
				this.toolTipPanel.SetActive(true);
				this.smsPanel.SetActive(true);
				this.smsPanel.GetComponent<AudioSource>().Play();
				this.smsPanel.transform.GetChild(1).GetComponent<Text>().text = "$" + num2 + " transferred to bank balance.";
				base.StartCoroutine(this.CloseSMS());
				return;
			}
		}
		else if ((this.missionType == 4 || this.missionType == 6) && this.numParts > 1)
		{
			Collider[] array = Physics.OverlapSphere(this.endTrigger.transform.position, 3f);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].tag == "DynamicSpawn")
				{
					this.counted++;
					if (this.counted >= this.numParts)
					{
						this.DoCompletion();
					}
				}
			}
		}
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x00066A5C File Offset: 0x00064C5C
	public void DoCompletion()
	{
		if (this.missionType == 2 || this.missionType == 5)
		{
			this.mc.CompleteMission(49);
			this.DestroySpawned();
			float num = 150f;
			if (this.missionType == 5)
			{
				num = 200f;
			}
			this.atmScript.balance += num;
			this.toolTipPanel.SetActive(true);
			this.smsPanel.SetActive(true);
			this.smsPanel.GetComponent<AudioSource>().Play();
			this.smsPanel.transform.GetChild(1).GetComponent<Text>().text = "$" + num + " transferred to bank balance.";
			base.StartCoroutine(this.CloseSMS());
			return;
		}
		if (this.missionType == 4 || this.missionType == 6)
		{
			this.mc.CompleteMission(49);
			float num2 = 150f + (float)(20 * this.numParts);
			this.atmScript.balance += num2;
			this.toolTipPanel.SetActive(true);
			this.smsPanel.SetActive(true);
			this.smsPanel.GetComponent<AudioSource>().Play();
			this.smsPanel.transform.GetChild(1).GetComponent<Text>().text = "$" + num2 + " transferred to bank balance.";
			base.StartCoroutine(this.CloseSMS());
			this.DestroySpawned();
			return;
		}
		if (this.missionType == 10)
		{
			this.mc.CompleteMission(49);
			this.DestroySpawned();
			float num3 = Vector3.Distance(this.TrackerXf.transform.position, this.TrackerXs.transform.position) / 4f;
			float num4 = 100f + Mathf.Round(num3 * 100f) / 100f;
			this.atmScript.balance += num4;
			this.toolTipPanel.SetActive(true);
			this.smsPanel.SetActive(true);
			this.smsPanel.GetComponent<AudioSource>().Play();
			this.smsPanel.transform.GetChild(1).GetComponent<Text>().text = "$" + num4 + " transferred to bank balance.";
			base.StartCoroutine(this.CloseSMS());
			base.StartCoroutine(this.DestroyTruck());
		}
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x00066CBB File Offset: 0x00064EBB
	private IEnumerator CloseSMS()
	{
		yield return new WaitForSeconds(5f);
		this.smsPanel.SetActive(false);
		yield break;
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x00066CCA File Offset: 0x00064ECA
	private IEnumerator DestroyTruck()
	{
		yield return new WaitForSeconds(10f);
		Object.Destroy(this.spawned);
		yield break;
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x00066CDC File Offset: 0x00064EDC
	public void CompleteHeavy()
	{
		this.mc.CompleteMission(49);
		this.DestroySpawned();
		float num = Vector3.Distance(this.TrackerXf.transform.position, this.TrackerXs.transform.position) / 4f;
		float num2 = 100f + Mathf.Round(num * 100f) / 100f;
		this.atmScript.balance += num2;
		this.toolTipPanel.SetActive(true);
		this.smsPanel.SetActive(true);
		this.smsPanel.GetComponent<AudioSource>().Play();
		this.smsPanel.transform.GetChild(1).GetComponent<Text>().text = "$" + num2 + " transferred to bank balance.";
		base.StartCoroutine(this.CloseSMS());
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x00002188 File Offset: 0x00000388
	public void ClearMission()
	{
	}

	// Token: 0x060007E9 RID: 2025 RVA: 0x00066DBC File Offset: 0x00064FBC
	public void DestroySpawned()
	{
		Object.Destroy(this.spawned);
		if (this.numParts > 1)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("DynamicSpawn");
			for (int i = 0; i < array.Length; i++)
			{
				Object.Destroy(array[i]);
			}
		}
		this.numParts = 1;
		this.counted = 0;
	}

	// Token: 0x04001226 RID: 4646
	private Vector3 spawnPosition;

	// Token: 0x04001227 RID: 4647
	private Vector3 endPosition;

	// Token: 0x04001228 RID: 4648
	public Terrain terrainOb;

	// Token: 0x04001229 RID: 4649
	private float xPosF;

	// Token: 0x0400122A RID: 4650
	private float yPosF;

	// Token: 0x0400122B RID: 4651
	private float zPosF;

	// Token: 0x0400122C RID: 4652
	private float xpos;

	// Token: 0x0400122D RID: 4653
	private float ypos;

	// Token: 0x0400122E RID: 4654
	private float zpos;

	// Token: 0x0400122F RID: 4655
	private GameObject spawned;

	// Token: 0x04001230 RID: 4656
	public Transform garageLoc;

	// Token: 0x04001231 RID: 4657
	public Transform cityLoc;

	// Token: 0x04001232 RID: 4658
	public Transform townLoc;

	// Token: 0x04001233 RID: 4659
	public Transform jyLoc;

	// Token: 0x04001234 RID: 4660
	public Transform dumpLoc;

	// Token: 0x04001235 RID: 4661
	public Transform forgeLoc;

	// Token: 0x04001236 RID: 4662
	public Transform jimmyDesk;

	// Token: 0x04001237 RID: 4663
	public Transform jiggsLoc;

	// Token: 0x04001238 RID: 4664
	public Transform mygLoc;

	// Token: 0x04001239 RID: 4665
	public Transform[] warehouseLoc;

	// Token: 0x0400123A RID: 4666
	public Transform flightDeliveryLoc;

	// Token: 0x0400123B RID: 4667
	public Transform[] heavyLoc;

	// Token: 0x0400123C RID: 4668
	private int deliveryLoc;

	// Token: 0x0400123D RID: 4669
	private Vector3 adjustedPos;

	// Token: 0x0400123E RID: 4670
	private Vector3 adjustedRot;

	// Token: 0x0400123F RID: 4671
	public Transform[] driveway;

	// Token: 0x04001240 RID: 4672
	public Transform[] constructionPoi;

	// Token: 0x04001241 RID: 4673
	public Transform[] raceWaypoints;

	// Token: 0x04001242 RID: 4674
	private int[] locArr;

	// Token: 0x04001243 RID: 4675
	public GameObject[] dumpObj;

	// Token: 0x04001244 RID: 4676
	public GameObject[] heavyObj;

	// Token: 0x04001245 RID: 4677
	public GameObject[] jyObj;

	// Token: 0x04001246 RID: 4678
	public GameObject[] recycObj;

	// Token: 0x04001247 RID: 4679
	public GameObject[] lostPart;

	// Token: 0x04001248 RID: 4680
	public GameObject[] constructionPart;

	// Token: 0x04001249 RID: 4681
	public string[] mission1text;

	// Token: 0x0400124A RID: 4682
	public GameObject fourRunner;

	// Token: 0x0400124B RID: 4683
	public GameObject npcDirtbike;

	// Token: 0x0400124C RID: 4684
	public GameObject RWGtruck;

	// Token: 0x0400124D RID: 4685
	public RWGfuel rwgfuel;

	// Token: 0x0400124E RID: 4686
	public GameObject heavyTrailer;

	// Token: 0x0400124F RID: 4687
	public GameObject CuttableTreeDown;

	// Token: 0x04001250 RID: 4688
	public GameObject cuttableTreeXL;

	// Token: 0x04001251 RID: 4689
	public GameObject meteor;

	// Token: 0x04001252 RID: 4690
	public GameObject blackbox;

	// Token: 0x04001253 RID: 4691
	public Transform TrackerXs;

	// Token: 0x04001254 RID: 4692
	public Transform TrackerXf;

	// Token: 0x04001255 RID: 4693
	public int missionType;

	// Token: 0x04001256 RID: 4694
	public float payment;

	// Token: 0x04001257 RID: 4695
	public string missionDesc;

	// Token: 0x04001258 RID: 4696
	public GameObject missionPart;

	// Token: 0x04001259 RID: 4697
	public string[] descAction;

	// Token: 0x0400125A RID: 4698
	public string[] descObj;

	// Token: 0x0400125B RID: 4699
	public string[] descDest;

	// Token: 0x0400125C RID: 4700
	public Atm atmScript;

	// Token: 0x0400125D RID: 4701
	public MissionController mc;

	// Token: 0x0400125E RID: 4702
	public GameObject smsPanel;

	// Token: 0x0400125F RID: 4703
	public GameObject toolTipPanel;

	// Token: 0x04001260 RID: 4704
	public GameObject startTrigger;

	// Token: 0x04001261 RID: 4705
	public GameObject endTrigger;

	// Token: 0x04001262 RID: 4706
	public GameObject missionTracker;

	// Token: 0x04001263 RID: 4707
	public GameObject missionTrackerB;

	// Token: 0x04001264 RID: 4708
	public GameObject part;

	// Token: 0x04001265 RID: 4709
	public int numParts;

	// Token: 0x04001266 RID: 4710
	private int counted;

	// Token: 0x04001267 RID: 4711
	private bool canGenerate = true;

	// Token: 0x04001268 RID: 4712
	public InteractiveObject corkboard;

	// Token: 0x04001269 RID: 4713
	private int lastMission;
}
