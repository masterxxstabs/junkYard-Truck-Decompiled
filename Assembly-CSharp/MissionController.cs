using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000F7 RID: 247
public class MissionController : MonoBehaviour
{
	// Token: 0x06000653 RID: 1619 RVA: 0x0004B0EA File Offset: 0x000492EA
	public void Start()
	{
		this.ActivateMission(0);
		this.activeMissions.Remove(31);
		this.PopulateMissionMenu();
	}

	// Token: 0x06000654 RID: 1620 RVA: 0x0004B108 File Offset: 0x00049308
	public void PopulateMissionMenu()
	{
		this.j = 0;
		for (int i = 0; i < this.missionbars.Length; i++)
		{
			if (this.activeMissions.Count > i)
			{
				GameObject gameObject = this.missionbars[i].transform.GetChild(1).gameObject;
				GameObject gameObject2 = this.missionbars[i].transform.GetChild(2).gameObject;
				gameObject.GetComponent<TextMeshProUGUI>().text = this.missiontext[this.activeMissions[this.j]];
				gameObject2.GetComponent<TextMeshProUGUI>().text = this.missiondesc[this.activeMissions[this.j]];
				if (this.activeMissions[this.j] == 49)
				{
					gameObject2.GetComponent<TextMeshProUGUI>().text = this.specialReq;
				}
				this.j++;
			}
			else
			{
				GameObject gameObject3 = this.missionbars[i].transform.GetChild(1).gameObject;
				GameObject gameObject4 = this.missionbars[i].transform.GetChild(2).gameObject;
				gameObject3.GetComponent<TextMeshProUGUI>().text = "";
				gameObject4.GetComponent<TextMeshProUGUI>().text = "";
			}
		}
		for (int j = 0; j < this.missionbars_c.Length; j++)
		{
			if (this.completedMissions.Count > j)
			{
				GameObject gameObject5 = this.missionbars_c[j].transform.GetChild(1).gameObject;
				GameObject gameObject6 = this.missionbars_c[j].transform.GetChild(2).gameObject;
				gameObject5.GetComponent<TextMeshProUGUI>().text = this.missiontext[this.completedMissions[j]];
				gameObject6.GetComponent<TextMeshProUGUI>().text = this.missiondesc[this.completedMissions[j]];
			}
		}
	}

	// Token: 0x06000655 RID: 1621 RVA: 0x0004B2D8 File Offset: 0x000494D8
	public void ActivateMission(int missionNum)
	{
		bool flag = this.activeMissions.Contains(missionNum);
		bool flag2 = this.completedMissions.Contains(missionNum);
		if (!flag && !flag2)
		{
			this.activeMissions.Add(missionNum);
			if (missionNum == 0 || missionNum == 1 || missionNum == 2 || missionNum == 46)
			{
				this.TrackMission_N(missionNum);
				if (missionNum == 1)
				{
					this.inter.ShowToolTips(7);
				}
			}
			else if (missionNum < this.textMessage.Length && this.textMessage[missionNum] == "")
			{
				this.ShowMissionTrackTool(missionNum);
			}
			this.PopulateMissionMenu();
			if (this.textMessage[missionNum] != "")
			{
				this.ShowSMS(missionNum);
			}
		}
	}

	// Token: 0x06000656 RID: 1622 RVA: 0x0004B380 File Offset: 0x00049580
	public void NextMarker(int missionNum)
	{
		this.trackerString = "tracker" + missionNum;
		this.marker = GameObject.Find(this.trackerString);
		this.marker.GetComponent<showmission>().RemoveWaypoint();
		this.marker.GetComponent<showmission>().manualNextActivated = true;
		this.trackerString = "tracker" + missionNum + "_mid";
		this.marker = GameObject.Find(this.trackerString);
		this.marker.GetComponent<showmission>().AddWaypoint();
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x0004B414 File Offset: 0x00049614
	public void ActivateMission_sub(int missionNum)
	{
		bool flag = this.activeMissions.Contains(missionNum);
		bool flag2 = this.completedMissions.Contains(missionNum);
		if (!flag && !flag2)
		{
			this.activeMissions.Add(missionNum);
			this.inter.ShowToolTips(1);
			this.PopulateMissionMenu();
		}
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x0004B460 File Offset: 0x00049660
	public void CompleteMission(int missionNum)
	{
		this.completetedUI.GetComponent<Text>().text = "COMPLETED: " + this.missiontext[missionNum];
		int num = this.activeMissions.IndexOf(missionNum);
		for (int i = 0; i < this.missionbars.Length; i++)
		{
			if (i == num)
			{
				this.missionbars[i].transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(false);
				this.missionbars[i].transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.SetActive(true);
			}
		}
		this.unTrackMission_N(missionNum);
		this.activeMissions.Remove(missionNum);
		if (!this.completedMissions.Contains(missionNum) && missionNum != 49)
		{
			this.completedMissions.Add(missionNum);
		}
		this.PopulateMissionMenu();
		this.CheckTrophies();
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x0004B550 File Offset: 0x00049750
	public void TrackMission(int slotNum)
	{
		for (int i = 0; i < this.missionbars.Length; i++)
		{
			if (i == slotNum)
			{
				if (this.missionbars[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text.Length > 2)
				{
					this.missionbars[i].transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(true);
					this.missionbars[i].transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.SetActive(false);
				}
			}
			else
			{
				this.missionbars[i].transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(false);
				this.missionbars[i].transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.SetActive(true);
			}
		}
		int num = this.activeMissions[slotNum];
		this.trackerString = "tracker" + num;
		this.marker = GameObject.Find(this.trackerString);
		this.marker.GetComponent<showmission>().AddWaypoint();
	}

	// Token: 0x0600065A RID: 1626 RVA: 0x0004B6A8 File Offset: 0x000498A8
	public void TrackMission_N(int missionNum)
	{
		Debug.Log("tracking" + missionNum);
		int num = this.activeMissions.IndexOf(missionNum);
		for (int i = 0; i < this.missionbars.Length; i++)
		{
			if (i == num)
			{
				this.missionbars[i].transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(true);
				this.missionbars[i].transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.SetActive(false);
			}
			else
			{
				this.missionbars[i].transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(false);
				this.missionbars[i].transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.SetActive(true);
			}
		}
		this.trackerString = "tracker" + missionNum;
		this.marker = GameObject.Find(this.trackerString);
		this.marker.GetComponent<showmission>().AddWaypoint();
	}

	// Token: 0x0600065B RID: 1627 RVA: 0x0004B7E8 File Offset: 0x000499E8
	public void unTrackMission(int slotNum)
	{
		Debug.Log("untrackSL" + slotNum);
		int num = this.activeMissions[slotNum];
		this.trackerString = "tracker" + num;
		this.marker = GameObject.Find(this.trackerString);
		this.marker.GetComponent<showmission>().RemoveWaypoint();
		this.missionbars[slotNum].transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(false);
		this.missionbars[slotNum].transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.SetActive(true);
		if (this.marker.GetComponent<showmission>().midpoints > 0)
		{
			foreach (object obj in this.marker.transform)
			{
				Transform transform = (Transform)obj;
				GameObject nextWaypoint = transform.gameObject.GetComponent<showmission>().nextWaypoint;
				GameObject manualNextWaypoint = transform.gameObject.GetComponent<showmission>().manualNextWaypoint;
				if (nextWaypoint != null)
				{
					nextWaypoint.GetComponent<showmission>().RemoveWaypoint();
				}
				if (manualNextWaypoint != null)
				{
					manualNextWaypoint.GetComponent<showmission>().RemoveWaypoint();
				}
			}
		}
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x0004B958 File Offset: 0x00049B58
	public void unTrackMission_N(int missionNum)
	{
		Debug.Log("untrackMN" + missionNum);
		this.trackerString = "tracker" + missionNum;
		this.marker = GameObject.Find(this.trackerString);
		this.marker.GetComponent<showmission>().RemoveWaypoint();
		if (this.marker.GetComponent<showmission>().midpoints > 0)
		{
			foreach (object obj in this.marker.transform)
			{
				Transform transform = (Transform)obj;
				transform.gameObject.GetComponent<showmission>().RemoveWaypoint();
				GameObject nextWaypoint = transform.gameObject.GetComponent<showmission>().nextWaypoint;
				GameObject manualNextWaypoint = transform.gameObject.GetComponent<showmission>().manualNextWaypoint;
				if (nextWaypoint != null)
				{
					nextWaypoint.GetComponent<showmission>().RemoveWaypoint();
				}
				if (manualNextWaypoint != null)
				{
					manualNextWaypoint.GetComponent<showmission>().RemoveWaypoint();
				}
			}
		}
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x0004BA6C File Offset: 0x00049C6C
	public void ShowMissionTrackTool(int trackNum)
	{
		this.inter.discoveredMission = trackNum;
		this.toolTipPanel.SetActive(true);
		this.toolTipPanel3.SetActive(true);
		this.trackText.GetComponent<Text>().text = this.missiontext[trackNum] + " [PRESS T TO TRACK]";
		base.StartCoroutine(this.HideToolTips());
		this.inter.canTrack = true;
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x0004BAD8 File Offset: 0x00049CD8
	public void ShowSMS(int missionNum)
	{
		this.toolTipPanel4.SetActive(true);
		this.smsText.GetComponent<Text>().text = this.textMessage[missionNum];
		base.StartCoroutine(this.HideToolTips());
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x0004BB0B File Offset: 0x00049D0B
	private IEnumerator HideToolTips()
	{
		Debug.Log("hiding");
		yield return new WaitForSeconds(6f);
		this.toolTipPanel.SetActive(false);
		this.toolTipPanel3.SetActive(false);
		this.toolTipPanel4.SetActive(false);
		this.inter.canTrack = false;
		yield break;
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x0004BB1C File Offset: 0x00049D1C
	public void CheckTrophies()
	{
		if (this.trophyJump < 7)
		{
			int[] targets = new int[]
			{
				18,
				19,
				20,
				21,
				22,
				23,
				24,
				25,
				26
			};
			this.trophyJump = this.completedMissions.Count((int m) => targets.Contains(m));
			if (this.trophyJump > 6)
			{
				this.trophyJumpG.SetActive(true);
				this.trophyJumpG.GetComponent<PickUp>().pickable = true;
			}
		}
		if (this.trophyHill < 4)
		{
			int[] targets = new int[]
			{
				27,
				28,
				29,
				61
			};
			this.trophyHill = this.completedMissions.Count((int m) => targets.Contains(m));
			if (this.trophyJump > 3)
			{
				this.trophyHillG.SetActive(true);
				this.trophyHillG.GetComponent<PickUp>().pickable = true;
			}
		}
		if (this.trophyTrial < 3)
		{
			int[] targets = new int[]
			{
				30,
				31,
				32,
				33,
				34,
				47,
				48
			};
			this.trophyTrial = this.completedMissions.Count((int m) => targets.Contains(m));
			if (this.trophyJump > 2)
			{
				this.trophyTrialG.SetActive(true);
				this.trophyTrialG.GetComponent<PickUp>().pickable = true;
			}
		}
		if (this.trophyAirtime == 1)
		{
			this.trophyAirtime = 2;
			this.trophyAirtimeG.SetActive(true);
			this.trophyAirtimeG.GetComponent<PickUp>().pickable = true;
		}
		if (this.trophyAirtimeF == 1)
		{
			this.trophyAirtimeF = 2;
			this.trophyAirtimeFG.SetActive(true);
			this.trophyAirtimeFG.GetComponent<PickUp>().pickable = true;
		}
	}

	// Token: 0x04000D37 RID: 3383
	public GameObject[] missionbars;

	// Token: 0x04000D38 RID: 3384
	public GameObject[] missionbars_c;

	// Token: 0x04000D39 RID: 3385
	public List<int> activeMissions = new List<int>();

	// Token: 0x04000D3A RID: 3386
	public List<int> completedMissions = new List<int>();

	// Token: 0x04000D3B RID: 3387
	private string trackerString;

	// Token: 0x04000D3C RID: 3388
	private string trackerString2;

	// Token: 0x04000D3D RID: 3389
	private GameObject marker;

	// Token: 0x04000D3E RID: 3390
	public Interactor inter;

	// Token: 0x04000D3F RID: 3391
	public GameObject completetedUI;

	// Token: 0x04000D40 RID: 3392
	public GameObject toolTipPanel;

	// Token: 0x04000D41 RID: 3393
	public GameObject toolTipPanel3;

	// Token: 0x04000D42 RID: 3394
	public GameObject toolTipPanel4;

	// Token: 0x04000D43 RID: 3395
	public GameObject trackText;

	// Token: 0x04000D44 RID: 3396
	public GameObject smsText;

	// Token: 0x04000D45 RID: 3397
	private int j;

	// Token: 0x04000D46 RID: 3398
	public int trophyJump;

	// Token: 0x04000D47 RID: 3399
	public int trophyHill;

	// Token: 0x04000D48 RID: 3400
	public int trophyTrial;

	// Token: 0x04000D49 RID: 3401
	public int trophyAirtime;

	// Token: 0x04000D4A RID: 3402
	public int trophyAirtimeF;

	// Token: 0x04000D4B RID: 3403
	public GameObject trophyJumpG;

	// Token: 0x04000D4C RID: 3404
	public GameObject trophyHillG;

	// Token: 0x04000D4D RID: 3405
	public GameObject trophyTrialG;

	// Token: 0x04000D4E RID: 3406
	public GameObject trophyAirtimeG;

	// Token: 0x04000D4F RID: 3407
	public GameObject trophyAirtimeFG;

	// Token: 0x04000D50 RID: 3408
	private string[] missiontext = new string[]
	{
		"CHECK YOUR COMPUTER",
		"VISIT THE JUNKYARD",
		"CALL IT A DAY",
		"[MISSION] RECYCLING",
		"[MISSION] BREAKER SWITCH",
		"[MISSION] LANDFILLER",
		"[MISSION] OVERFLOW VALVE",
		"[MISSION] LAYING PIPE",
		"[MISSION] placeholder",
		"[MISSION] LOADER TROUBLE",
		"[MISSION] NUCLEAR BOMB",
		"[MISSION] NUCLEAR BOMB PART II",
		"[MISSION] GENERATOR",
		"[MISSION] GENERATOR FUEL",
		"[MISSION] RETURN THE GENERATOR AND FUEL TRAILER",
		"[MISSION] FURNITURE",
		"[MISSION] SABOTAGE",
		"[MISSION] PARASITE ANTENNA",
		"[JUMP] MAIN STREET REVERSE STEP-UP",
		"[JUMP] RIVER JUMP",
		"[JUMP] TRANSFER STATION TRANSFER",
		"[JUMP] MAROONED",
		"[JUMP] 60 FEET UNDER",
		"[JUMP] NOT COOL!",
		"[JUMP] NEW AGE STEP-UP",
		"[JUMP] WHAT BRIDGE?",
		"[JUMP] COVER STORY",
		"[HILL CLIMB] JUNKYARD HILL",
		"[HILL CLIMB] THREAD THE NEEDLE",
		"[HILL CLIMB] IN A HURRY",
		"[TIME TRIAL] ACCELERATED DRAGON",
		"[TIME TRIAL] ROWHAMMER LOOP",
		"[TIME TRIAL] ROWHAMMER HALF LOOP",
		"[TIME TRIAL] RADIO ACTIVE",
		"[TIME TRIAL] DEATHWISH RELAY RACE",
		"[MISSION] STEEL BEAMS",
		"[MISSION] NARCO ANTENNA",
		"[MISSION] GENERATORS",
		"[MISSION] WELDER",
		"[MISSION] LOST TOOLS",
		"[MISSION] BEER RUN",
		"[MISSION] REPLACEMENT WHEELS",
		"[MISSION] REVENGE",
		"[MISSION] MOONSHINE",
		"[MISSION] MOONSHINE STILL",
		"[MISSION] 170 PROOF",
		"CHECK YOUR COMPUTER",
		"[TIME TRIAL] REVERSED DRAGON",
		"[TIME TRIAL] RETURN ACE",
		"[MISSION] SPECIAL REQUEST",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"[MISSION] MOONSHINE INFUSING",
		"[HILL CLIMB] IRON MOUNTAIN",
		"[JUMP] SEND IT",
		"[MISSION] IRON WILL",
		"[MISSION] DEADWEIGHT HAUL",
		"[MISSION] WRECKING BALL",
		"[MISSION] CLEAR THE AIRWAVES",
		"[MISSION] REVERSE LOGISTICS",
		"[MISSION] MOUNT TRASHMORE",
		"[MISSION] EVEL KNIEVEL",
		"[MISSION] SWIMMING WITH THE FISH",
		"[MISSION] DIGGING UP GHOSTS",
		"[MISSION] CLEANUP CREW"
	};

	// Token: 0x04000D51 RID: 3409
	private string[] missiondesc = new string[]
	{
		"CHECK YOUR COMPUTER FOR NEW MISSIONS. AND GRAB YOUR MONEY.",
		"JOHNNY JUNKS SAYS HE HAS AN OFF ROAD TRUCK FOR SALE AT HIS JUNKYARD. DON'T FORGET TO PICK UP YOUR CASH ON YOUR DESK.",
		"GET SOME REST BEFORE YOUR TRUCK IS DELIVERED",
		"SEARCH FOR SOME SCRAP METAL. DEPOSIT IT IN JUNKYARD LOADING AREA AND TALK TO JOHNNY.",
		"FLIP THE BREAKER AT THE RADIO TOWER. YOU WILL PROBABLY NEED A TRUCK.",
		"PICK UP 10 TRASH BAGS FROM TOWN AND DELIVER THEM TO THE LOCAL DUMP. DEPOSIT THE BAGS INTO THE ORANGE CARGO CONTAINER.",
		"I WILL GET $300 FOR DRIVING TO THE WATER TOWER AND TURNING A KNOB?",
		"A PALLET OF HEAVY PIPES NEEDS TO BE DELIVERED TO THE WATER TOWER. CARRYING IT IS PROBABLY OUT OF THE QUESTION",
		"A CAR STUCK IN THE MUD. IT CAN BE PULLED OUT IF ONLY I HAD A WINCH",
		"NICK NEEDS A NEW TURBOCHARGER FOR HIS LOADER. THE JUNKYARD MIGHT HAVE ONE. YOU WILL ALSO HAVE TO INSTALL IT.",
		"DISPOSE OF AN OLD NUCLEAR BOMB AT THE LOCAL LANDFILL. HOPEFULLY THEY TAKE NUCLEAR BOMBS.",
		"DISPOSE OF THE NUCLEAR BOMB AT THE EDGE OF TOWN",
		"DELIVER THE GENERATOR TRAILER TO THE CELL TOWER",
		"DELIVER THE FUEL TRAILER TO THE CELL TOWER",
		"RETURN THE FUEL TRAILER AND GENERATOR TO THE PARKING AREA",
		"YOU HAVE A TRUCK SO NOW YOU HAVE FRIENDS ASKING FOR HELP MOVING",
		"YOU OWN A CREEPER AND A WRENCH. BUT MAKE SURE YOU DO IT AT NIGHT.",
		"CLIMB THE TOWER. FIND AND REMOVE THE UNAUTHORIZED NARCO ANTENNA.",
		"USE THE EMBANKMENT TO JUMP OVER THE STREET, ALSO CLEARING THE DITCH ON THE OTHER SIDE.",
		"JUMP ACROSS THE RIVER. EASY, BUT SCREWING THIS UP COULD COST A LOT.",
		"USE THE TRASH PILE TO JUMP TO THE OTHER TRASH PILE",
		"USE THE SUNKEN CARGO CONTAINER TO JUMP TO THE ISLAND.",
		"THE LANDING AREA IS A GOOD 60 FEET BELOW THE KICKER. TRY IT IF YOU HAVE THE CASH FOR REPAIRS.",
		"JUMP OVER JAKE'S HOUSE",
		"LAND ON TOP OF NEW AGE AUTO PARTS",
		"JUMP THE RAVINE AND LAND ON THE OPPOSITE EMBANKMENT",
		"LAND ON TOP OF THE ARCHON CANOPY",
		"MAKE IT TO THE TOP, AND THEN DOWN THE OTHER SIDE",
		"THE TOUGHEST PART IS THE LAST 15 FEET",
		"RACE UP THE EMBANKMENT TO THE STREET",
		"MAKE IT UP THE HILL, ACROSS THE TOP, AND DOWN THE OTHER SIDE AS QUICKLY AS POSSIBLE",
		"FOLLOW THE MARKERS AROUND THE STREETS CIRCLING THE LAKE",
		"FOLLOW THE MARKERS AROUND THE EAST LAKE, CUTTING THROUGH THE ISTHMUS",
		"REACH ALL THREE RADIO TOWERS AS QUICKLY AS POSSIBLE, IN ANY ORDER",
		"STOP YOUR VEHICLE AND CHUG A BEER AT EACH CHECKPOINT",
		"DELIVER THE PALLET OF STEEL BEAMS TO THE TOWER",
		"LOCATE AND REMOVE THE NARCO ANTENNA.",
		"DELIVER THE TWO GENERATORS TO THE PALLET IN FRONT OF THE TOWER",
		"DELIVER THE WELDER TO THE PALLET IN FRONT OF THE TOWER",
		"FIND THE LOST TOOLBOX AND RETURN IT",
		"MY FRIEND JAKE ASKED ME TO PICK UP A 12 PACK OF BEER FOR HIM AND WANTS TO SEE MY NEW TRUCK.",
		"SOMEONE STOLE THE WHEELS OFF OF JAKE'S CAR. I CAN PROBABLY FIND SOME CHEAP REPLACEMENT WHEELS IF I SCAVENGE THE JUNKYARD. I WILL NEED TO FIND FOUR OF THEM.",
		"JAKE FIGURED OUT WHO STOLE HIS WHEELS. THEY LIVE IN TOWN, AND HE WANTS ME TO LOOSEN THEIR OIL DRAIN BOLT. BONUS PAY IF I PUT SUGAR IN THEIR GAS TANK.",
		"ASK JIGGS CASEY ABOUT GETTING SOME MOONSHINE.",
		"DISCOVER JIGGS CASEY'S MOONSHINE STILL AND LOCATE HIS JOURNAL.",
		"BRING YOUR TRUCK AND VISIT JAKE WHEN YOUR TRUCK IS ALMOST OUT OF GAS.",
		"CHECK YOUR COMPUTER FOR NEW MISSIONS",
		"ACCELERATED DRAGON, BUT BACKWARDS",
		"LOOP AROUND THE TREE",
		"SPECIAL REQUEST.",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"THE BAR IS REQUESTINS BLACKBERRY, LIME, ORANGE, AND AMBROSIA MOONSHINE. I CAN BUY AN INFUSER FROM JIGGS AND ATTACH IT TO MY STILL. I ALSO NEED TO FIND THE FRUIT TO PUT IN IT.",
		"FOLLOW THE TRACKS UP THE MOUNTAIN SIDE. YOU WILL NEED A LOT OF SPEED TO CLEAR THE TERRACES.",
		"JUMP OVER THE ROCK AND LAND ON THE CLEARING BELOW.",
		"DIG UP 1000Kg OF IRON AND TRADE IT AT THE FORGE.",
		"CUT AND HAUL THE FOUR DEAD TREES IN THE AREA. SELL THE WOOD AT THE FORGE.",
		"TOW THE MOBILE GENERATOR TO THE JOB SITE.",
		"CUT DOWN THE LARGE TREE BLOCKING THE TOWER'S SIGNAL.",
		"EVENRYTHING YOU HAULED NEEDS TO BE BROUGHT BACK.",
		"TRANSPORT THE LARGE PILE OF TRASH TO THE LANDFILL.",
		"THE GAS STATION CLERK NEEDS HELP GETTING HER KEYS. SHE SAYS THEY'RE ON TOP OF THE STATION'S CANOPY.",
		"TOW JIMMY'S CAR FROM THE BOTTOM OF THE LAKE.",
		"RIP OUT THE OLD TREE STUMPS.",
		"REMOVE THE GARBAGE AND CUT AND HAUL THE TREES BEHIND JIGGS' HOUSE."
	};

	// Token: 0x04000D52 RID: 3410
	public string specialReq;

	// Token: 0x04000D53 RID: 3411
	private string[] textMessage = new string[]
	{
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"Special Request.",
		"You busy? I need someone with a truck and a chainsaw.",
		"I ran out of gas. I could use some help.",
		"Are you busy? I ran out of chainsaw fuel and can't afford to make another trip into town.",
		"I could really use your help. There's an illegal dump site just outside of town I need help cleaning up.",
		"There's some kind of boat anchor blocking one of the back roads. I need help moving it.",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		""
	};
}
