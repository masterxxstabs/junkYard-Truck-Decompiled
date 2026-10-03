using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020000EE RID: 238
public class MailScript : MonoBehaviour
{
	// Token: 0x060005DB RID: 1499 RVA: 0x000483B5 File Offset: 0x000465B5
	private void Start()
	{
		if (!this.receivedEmails.Contains(0))
		{
			this.receivedEmails.Add(0);
			this.receivedEmails.Add(1);
		}
		this.RefreshEmail();
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x000483E3 File Offset: 0x000465E3
	public void Init()
	{
		this.saveLink.SetActive(true);
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x000483F4 File Offset: 0x000465F4
	public void RefreshEmail()
	{
		this.saveLink.SetActive(true);
		int num = 0;
		foreach (int num2 in this.receivedEmails)
		{
			this.emails[num].GetComponent<Text>().text = this.subjects[num2];
			num++;
		}
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x0004846C File Offset: 0x0004666C
	public void HideSaveLink()
	{
		this.saveLink.SetActive(false);
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x0004847C File Offset: 0x0004667C
	public void LoadEmail()
	{
		int index = int.Parse(EventSystem.current.currentSelectedGameObject.name.Substring(9));
		int num = this.receivedEmails[index];
		if (this.mc.activeMissions.Contains(46))
		{
			this.mc.CompleteMission(46);
		}
		this.emailbody.GetComponent<Text>().text = this.bodies[num];
		switch (num)
		{
		case 0:
			this.mc.CompleteMission(0);
			this.mc.ActivateMission(1);
			return;
		case 1:
		case 5:
		case 11:
		case 12:
		case 13:
			break;
		case 2:
			this.mc.ActivateMission(3);
			return;
		case 3:
			this.mc.ActivateMission(5);
			return;
		case 4:
			this.mc.ActivateMission(4);
			this.businessman.SetActive(true);
			return;
		case 6:
			this.mc.ActivateMission(35);
			this.businessman.SetActive(true);
			if (this.businessman.GetComponent<Businessman>().missionNum == 2)
			{
				this.businessman.GetComponent<Businessman>().missionNum = 3;
				this.beampallet.SetActive(true);
				return;
			}
			break;
		case 7:
			this.mc.ActivateMission(37);
			if (this.businessman.GetComponent<Businessman>().missionNum == 8)
			{
				this.businessman.GetComponent<Businessman>().missionNum = 9;
				this.generator1.SetActive(true);
				this.generator2.SetActive(true);
				return;
			}
			break;
		case 8:
			this.jake.SetActive(true);
			this.mc.ActivateMission(40);
			if (this.jake.GetComponent<Jake>().missionNum == 0)
			{
				this.jake.GetComponent<Jake>().missionNum = 1;
				return;
			}
			break;
		case 9:
			if (this.jake.GetComponent<Jake>().missionNum == 3)
			{
				this.jake.GetComponent<Jake>().missionNum = 4;
			}
			this.mc.ActivateMission(41);
			return;
		case 10:
			if (this.jake.GetComponent<Jake>().missionNum == 6)
			{
				this.jake.GetComponent<Jake>().missionNum = 7;
			}
			this.mc.ActivateMission(42);
			return;
		case 14:
			this.dirtbike.SetActive(true);
			this.engine250.SetActive(true);
			break;
		default:
			return;
		}
	}

	// Token: 0x04000CD7 RID: 3287
	private string[] subjects = new string[]
	{
		"Johnny Junks                   I think I found what you wanted.",
		"HookZilla                          Towing Insurance",
		"Johnny Junks                   Cash For Your Scrap Metal",
		"Nick Garbaggio                 Looking for Help",
		"Sean M.                            Breaker Switch",
		"Nick Garbaggio                 Truckload of Recyclables",
		"Sean M.                            Steel Beams",
		"Sean M.                            Generators",
		"Jake B.                              Beer Run",
		"Jake B.                              no subject",
		"Jake B.                              About those wheels",
		"Rowhammer Forge           Logging",
		"Jiggs Casey                    stuck like chuck!",
		"Rowhammer Forge            Mining"
	};

	// Token: 0x04000CD8 RID: 3288
	private string[] bodies = new string[]
	{
		"You still lookin for a truck?\nMy junkyard received a truck this morning- and not just any truck- a 1982 Diamondback.\nThis year and model is legendary for being damn near indestructible.\nIt needs some work but I'll sell it for an easy $400. Come on down and take a look if you're interested.\np.s. Don't forget to bring cash. We take cash only!\nJohnny Junks\nAuto Junktion",
		"Your insurance plan has been upgraded to include 24/7 towing at a fraction of the price!\nStranded? Pick up your phone and give us a call.\nHookZilla Towing\n204 W. Elm St.\nRowhammer, WA",
		"In an effort to clean up our town and the surrounding area, Auto Junktion will be giving cash\nfor your scrap metal. Appliances! Car parts! Scrap metal! No questions asked!\nJohnny Junks\nAuto Junktion",
		"I'm in a bit of a jam.\nI broke the garbage truck's u-joints again. Nobody can know about this.\nI will give you $10 per garbage bag that you deliver from the town to my landfill.\nI can also give you money for old tires.\nThanks,\nNick",
		"I'm looking for someone with a truck capable of driving up to our cell towers. Come talk to me if you do.\nSean\nRowhammer Cellular Service inc",
		"I have a truckload of scrap metal here at the dump.\nYou can haul it to Auto Junktion and make a profit. But I get a 10% finder's fee!",
		"I have another job if you're interested. Come talk to me if you are.\nSean\nRowhammer Cellular Service inc",
		"I have a delivery I need help with. Come talk to me if you're interested.\nSean\nRowhammer Cellular Service inc",
		"Is your truck running yet? I want to see it. Also, can you drop by the gas station and pick up a 12 pack for me?\nDon't worry, I got the money.",
		"Are you doing anything? Come down here, I have to show you something.\nAlso, be on the lookout. I heard Sheriff Langford is back in town.",
		"I figured out who stole my wheels the other day, and I want you to help me do something devious.\nCome talk to me. Being your mechanic's creeper.",
		"We need some wood delivered. As much as you can get.\nAny tree around town marked with a red ribbon can legally be cut down.\nSo if you have a chainsaw and a decent truck, we pay $20 per log you deliver.\nJust throw them in the bin and collect your money at the nearby scale.",
		"i was drivin out to my favorite fishing spot and i got my car stuck in the mud. i had to abandon it.\nif you pull it out i'll pay you. you'll need a winch.\nit's near the river on the back road between your garage and the junkyard\nJiggs",
		"Our forge will also buy iron ore.\nThe abandoned mining site has a lot of raw iron close to the surface, which can be mined with a pickaxe.\nIf you've mined everything you can find, wait until the rain comes and exposes more ore."
	};

	// Token: 0x04000CD9 RID: 3289
	public List<int> receivedEmails = new List<int>();

	// Token: 0x04000CDA RID: 3290
	public GameObject[] emails;

	// Token: 0x04000CDB RID: 3291
	public GameObject emailbody;

	// Token: 0x04000CDC RID: 3292
	public MissionController mc;

	// Token: 0x04000CDD RID: 3293
	public GameObject businessman;

	// Token: 0x04000CDE RID: 3294
	public GameObject jake;

	// Token: 0x04000CDF RID: 3295
	public GameObject beampallet;

	// Token: 0x04000CE0 RID: 3296
	public GameObject generator1;

	// Token: 0x04000CE1 RID: 3297
	public GameObject generator2;

	// Token: 0x04000CE2 RID: 3298
	public GameObject welder;

	// Token: 0x04000CE3 RID: 3299
	public GameObject stuckCar;

	// Token: 0x04000CE4 RID: 3300
	public GameObject saveLink;

	// Token: 0x04000CE5 RID: 3301
	public GameObject dirtbike;

	// Token: 0x04000CE6 RID: 3302
	public GameObject engine250;

	// Token: 0x04000CE7 RID: 3303
	public Currency curr;
}
