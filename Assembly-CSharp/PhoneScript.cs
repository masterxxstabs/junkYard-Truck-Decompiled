using System;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x02000110 RID: 272
public class PhoneScript : MonoBehaviour
{
	// Token: 0x0600071B RID: 1819 RVA: 0x0005B18C File Offset: 0x0005938C
	public void Start()
	{
		this.vehMenu.SetActive(true);
		this.destMenu.SetActive(false);
		if (this.atm.ownsHouse)
		{
			this.optionhouse2.GetComponent<Button>().enabled = true;
			this.optionhouse2.GetComponent<Text>().color = UnityEngine.Color.white;
		}
		if (this.dirtBike.GetComponent<PickUp>().pickable)
		{
			this.optionMoto.GetComponent<Button>().enabled = true;
			this.optionMoto.GetComponent<Text>().color = UnityEngine.Color.white;
		}
		if (this.truck2.GetComponent<car4>().hasTruck2)
		{
			this.optiontruck2.GetComponent<Button>().enabled = true;
			this.optiontruck2.GetComponent<Text>().color = UnityEngine.Color.white;
		}
		if (this.interactor.diamondbackImpounded)
		{
			this.optionDiamondback.GetComponent<Button>().enabled = false;
			this.optionDiamondback.GetComponent<Text>().color = UnityEngine.Color.gray;
		}
		else
		{
			this.optionDiamondback.GetComponent<Button>().enabled = true;
			this.optionDiamondback.GetComponent<Text>().color = UnityEngine.Color.white;
		}
		if (this.interactor.f100Impounded)
		{
			this.optiontruck2.GetComponent<Button>().enabled = false;
			this.optiontruck2.GetComponent<Text>().color = UnityEngine.Color.gray;
		}
		else
		{
			this.optiontruck2.GetComponent<Button>().enabled = true;
			this.optiontruck2.GetComponent<Text>().color = UnityEngine.Color.white;
		}
		if (this.interactor.amcImpounded)
		{
			this.optionEagle.GetComponent<Button>().enabled = false;
			this.optionEagle.GetComponent<Text>().color = UnityEngine.Color.gray;
		}
		else
		{
			this.optionEagle.GetComponent<Button>().enabled = true;
			this.optionEagle.GetComponent<Text>().color = UnityEngine.Color.white;
		}
		if (this.interactor.dirtbikeImpounded)
		{
			this.optionMoto.GetComponent<Button>().enabled = false;
			this.optionMoto.GetComponent<Text>().color = UnityEngine.Color.gray;
			return;
		}
		this.optionMoto.GetComponent<Button>().enabled = true;
		this.optionMoto.GetComponent<Text>().color = UnityEngine.Color.white;
	}

	// Token: 0x0600071C RID: 1820 RVA: 0x0005B3BF File Offset: 0x000595BF
	public void SelectObject(int veh)
	{
		this.vehMenu.SetActive(false);
		this.destMenu.SetActive(true);
		this.thisVeh = veh;
	}

	// Token: 0x0600071D RID: 1821 RVA: 0x0005B3E0 File Offset: 0x000595E0
	public void SelectDestination(int dest)
	{
		if (!this.interactor.arrested)
		{
			if (dest == 1)
			{
				if (this.thisVeh == 1)
				{
					this.Option5();
					return;
				}
				if (this.thisVeh == 2)
				{
					this.Option1();
					return;
				}
				if (this.thisVeh == 3)
				{
					this.Option3();
					return;
				}
				if (this.thisVeh == 4)
				{
					this.Option7();
					return;
				}
				if (this.thisVeh == 5)
				{
					this.Option13();
					return;
				}
			}
			else if (dest == 2)
			{
				if (this.thisVeh == 1)
				{
					this.Option6();
					return;
				}
				if (this.thisVeh == 2)
				{
					this.Option2();
					return;
				}
				if (this.thisVeh == 3)
				{
					this.Option4();
					return;
				}
				if (this.thisVeh == 4)
				{
					this.Option8();
					return;
				}
				if (this.thisVeh == 5)
				{
					this.Option14();
					return;
				}
			}
			else if (dest == 3)
			{
				if (this.thisVeh == 1)
				{
					this.Option9();
					return;
				}
				if (this.thisVeh == 2)
				{
					this.Option11();
					return;
				}
				if (this.thisVeh == 3)
				{
					this.Option10();
					return;
				}
				if (this.thisVeh == 4)
				{
					this.Option12();
					return;
				}
				if (this.thisVeh == 5)
				{
					this.Option15();
				}
			}
		}
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x0005B4FC File Offset: 0x000596FC
	public void Option1()
	{
		this.cost = 45;
		if (this.currency.money >= (float)this.cost && this.truck.activeSelf && this.person.transform.parent == null)
		{
			if (Vector3.Distance(this.car.transform.position, this.homePosTruck.position) < 4f)
			{
				this.car.SetActive(false);
				this.car.transform.position = this.homePosCar.position;
				this.car.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.cardoor_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.cardoor_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.car.SetActive(true);
			}
			this.DisconnectTrailers();
			this.winch.DetachHook();
			this.winch2.DetachHook();
			this.truck.SetActive(false);
			if (Vector3.Distance(this.person.transform.position, this.homePosMe.position) > 30f)
			{
				this.person.transform.position = this.homePosMe.position;
			}
			this.truck.transform.position = this.homePosTruck.position;
			this.truck.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.truckdoor_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.truckdoor_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.fob.transform.rotation = Quaternion.Euler(new Vector3(50f, 34f, 18f));
			this.phonePanel.SetActive(false);
			this.fpc.LockMouse();
			this.person.GetComponent<Interactor>().inv.SubtractMoney((float)this.cost);
			this.currency.money -= (float)this.cost;
			if (this.currency.nicotine < 3f)
			{
				this.currency.stress -= 50f;
			}
			if (this.currency.stress < 0f)
			{
				this.currency.stress = 0f;
			}
			this.fpc.enabled = true;
			this.truck.SetActive(true);
		}
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x0005B804 File Offset: 0x00059A04
	public void Option1b()
	{
		if (Vector3.Distance(this.car.transform.position, this.homePosTruck.position) < 4f)
		{
			this.car.SetActive(false);
			this.car.transform.position = this.homePosCar.position;
			this.car.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.cardoor_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.cardoor_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.car.SetActive(true);
		}
		this.DisconnectTrailers();
		this.winch.DetachHook();
		this.winch2.DetachHook();
		this.truck.SetActive(false);
		this.person.transform.position = this.homePosMe.position;
		this.truck.transform.position = this.homePosTruck.position;
		this.truck.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		this.truckdoor_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		this.truckdoor_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		this.fob.transform.rotation = Quaternion.Euler(new Vector3(50f, 34f, 18f));
		this.phonePanel.SetActive(false);
		this.fpc.LockMouse();
		this.truck.SetActive(true);
		this.fpc.enabled = true;
	}

	// Token: 0x06000720 RID: 1824 RVA: 0x0005BA1C File Offset: 0x00059C1C
	public void Option2()
	{
		this.cost = 45;
		if (this.currency.money >= (float)this.cost && this.truck.activeSelf && this.person.transform.parent == null)
		{
			this.DisconnectTrailers();
			this.winch.DetachHook();
			this.winch2.DetachHook();
			this.truck.SetActive(false);
			if (Vector3.Distance(this.person.transform.position, this.townPosMe.position) > 30f)
			{
				this.person.transform.position = this.townPosMe.position;
			}
			this.truck.transform.position = this.townPosTruck.position;
			this.truck.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.truckdoor_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.truckdoor_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.fob.transform.rotation = Quaternion.Euler(new Vector3(50f, 34f, 18f));
			this.phonePanel.SetActive(false);
			this.fpc.LockMouse();
			this.currency.money -= (float)this.cost;
			if (this.currency.nicotine < 3f)
			{
				this.currency.stress -= 50f;
			}
			if (this.currency.stress < 0f)
			{
				this.currency.stress = 0f;
			}
			this.person.GetComponent<Interactor>().inv.SubtractMoney((float)this.cost);
			this.fpc.enabled = true;
			this.truck.SetActive(true);
		}
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x0005BC4C File Offset: 0x00059E4C
	public void Option3()
	{
		this.cost = 45;
		if (this.currency.money >= (float)this.cost && this.person.transform.parent == null)
		{
			if (Vector3.Distance(this.truck.transform.position, this.homePosCar.position) < 4f)
			{
				this.truck.SetActive(false);
				this.truck.transform.position = this.homePosTruck.position;
				this.truck.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.truckdoor_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.truckdoor_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.fob.transform.rotation = Quaternion.Euler(new Vector3(50f, 34f, 18f));
				this.truck.SetActive(true);
			}
			this.car.SetActive(false);
			if (Vector3.Distance(this.person.transform.position, this.homePosMe.position) > 30f)
			{
				this.person.transform.position = this.homePosMe.position;
			}
			this.car.transform.position = this.homePosCar.position;
			this.car.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.cardoor_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.cardoor_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.phonePanel.SetActive(false);
			this.fpc.LockMouse();
			this.currency.money -= (float)this.cost;
			if (this.currency.nicotine < 3f)
			{
				this.currency.stress -= 50f;
			}
			if (this.currency.stress < 0f)
			{
				this.currency.stress = 0f;
			}
			this.person.GetComponent<Interactor>().inv.SubtractMoney((float)this.cost);
			this.fpc.enabled = true;
			this.car.SetActive(true);
			this.car.GetComponent<car3>().userControlled = false;
			this.car.GetComponent<car3>().TurnOff();
			this.car.GetComponent<car3>().turnOffAcc();
			this.person.GetComponent<Interactor>().keyStateC = 0;
		}
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x0005BF6C File Offset: 0x0005A16C
	public void Option3b()
	{
		if (Vector3.Distance(this.truck.transform.position, this.homePosCar.position) < 4f)
		{
			this.truck.SetActive(false);
			this.truck.transform.position = this.homePosTruck.position;
			this.truck.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.truckdoor_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.truckdoor_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.fob.transform.rotation = Quaternion.Euler(new Vector3(50f, 34f, 18f));
			this.truck.SetActive(true);
		}
		this.car.SetActive(false);
		this.person.transform.position = this.homePosMe.position;
		this.car.transform.position = this.homePosCar.position;
		this.car.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		this.cardoor_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		this.cardoor_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		this.phonePanel.SetActive(false);
		this.fpc.LockMouse();
		if (this.currency.nicotine < 3f)
		{
			this.currency.stress -= 50f;
		}
		if (this.currency.stress < 0f)
		{
			this.currency.stress = 0f;
		}
		this.fpc.enabled = true;
		this.car.SetActive(true);
		this.car.GetComponent<car3>().userControlled = false;
		this.car.GetComponent<car3>().TurnOff();
		this.car.GetComponent<car3>().turnOffAcc();
		this.person.GetComponent<Interactor>().keyStateC = 0;
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x0005C1F4 File Offset: 0x0005A3F4
	public void Option4()
	{
		this.cost = 45;
		if (this.currency.money >= (float)this.cost && this.fpc.canMove && this.person.transform.parent == null)
		{
			this.car.SetActive(false);
			if (Vector3.Distance(this.person.transform.position, this.townPosMe.position) > 30f)
			{
				this.person.transform.position = this.townPosMe.position;
			}
			this.car.transform.position = this.townPosCar.position;
			this.car.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.cardoor_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.cardoor_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.phonePanel.SetActive(false);
			this.fpc.LockMouse();
			this.person.GetComponent<Interactor>().inv.SubtractMoney((float)this.cost);
			this.currency.money -= (float)this.cost;
			if (this.currency.nicotine < 3f)
			{
				this.currency.stress -= 50f;
			}
			if (this.currency.stress < 0f)
			{
				this.currency.stress = 0f;
			}
			this.fpc.enabled = true;
			this.car.SetActive(true);
			this.car.GetComponent<car3>().userControlled = false;
			this.car.GetComponent<car3>().TurnOff();
			this.car.GetComponent<car3>().turnOffAcc();
			this.person.GetComponent<Interactor>().keyStateC = 0;
		}
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x0005C420 File Offset: 0x0005A620
	public void Option5()
	{
		this.cost = 45;
		if (this.currency.money >= (float)this.cost && this.person.transform.parent == null)
		{
			this.person.transform.position = this.homePosMe.position;
			this.phonePanel.SetActive(false);
			this.fpc.LockMouse();
			this.person.GetComponent<Interactor>().inv.SubtractMoney((float)this.cost);
			this.currency.money -= (float)this.cost;
			if (this.currency.nicotine < 3f)
			{
				this.currency.stress -= 33f;
			}
			if (this.currency.stress < 0f)
			{
				this.currency.stress = 0f;
			}
			this.fpc.enabled = true;
		}
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x0005C528 File Offset: 0x0005A728
	public void Option5b()
	{
		this.phonePanel.SetActive(false);
		this.fpc.LockMouse();
		this.fpc.enabled = true;
		Vector3 position = this.person.transform.position;
		float x = position.x / this.terrainOb.terrainData.size.x;
		float y = position.z / this.terrainOb.terrainData.size.z;
		Vector3 position2 = new Vector3(position.x, this.terrainOb.terrainData.GetInterpolatedHeight(x, y) + 2f, position.z);
		this.person.transform.position = position2;
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x0005C5E4 File Offset: 0x0005A7E4
	public void Option6()
	{
		this.cost = 45;
		if (this.currency.money >= (float)this.cost && this.person.transform.parent == null)
		{
			this.person.transform.position = this.townPosMe.position;
			this.phonePanel.SetActive(false);
			this.fpc.LockMouse();
			this.person.GetComponent<Interactor>().inv.SubtractMoney((float)this.cost);
			this.currency.money -= (float)this.cost;
			if (this.currency.nicotine < 3f)
			{
				this.currency.stress -= 33f;
			}
			if (this.currency.stress < 0f)
			{
				this.currency.stress = 0f;
			}
			this.fpc.enabled = true;
		}
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x0005C6EC File Offset: 0x0005A8EC
	public void Option7()
	{
		this.cost = 45;
		if (this.currency.money >= (float)this.cost && this.dirtBike.GetComponent<PickUp>().price == 0f && this.person.transform.parent == null)
		{
			this.dirtBike.SetActive(false);
			this.dirtBike.transform.position = this.homePos250.position;
			this.dirtBike.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.phonePanel.SetActive(false);
			this.fpc.LockMouse();
			this.person.GetComponent<Interactor>().inv.SubtractMoney((float)this.cost);
			this.currency.money -= (float)this.cost;
			if (this.currency.nicotine < 3f)
			{
				this.currency.stress -= 25f;
			}
			if (this.currency.stress < 0f)
			{
				this.currency.stress = 0f;
			}
			this.fpc.enabled = true;
			this.dirtBike.SetActive(true);
			this.person.transform.position = this.homePosMe.position;
		}
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x0005C868 File Offset: 0x0005AA68
	public void Option8()
	{
		this.cost = 45;
		if (this.currency.money >= (float)this.cost && this.dirtBike.GetComponent<PickUp>().price == 0f && this.person.transform.parent == null)
		{
			this.dirtBike.SetActive(false);
			this.dirtBike.transform.position = this.townPos250.position;
			this.dirtBike.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.phonePanel.SetActive(false);
			this.fpc.LockMouse();
			this.person.GetComponent<Interactor>().inv.SubtractMoney((float)this.cost);
			this.currency.money -= (float)this.cost;
			if (this.currency.nicotine < 3f)
			{
				this.currency.stress -= 25f;
			}
			if (this.currency.stress < 0f)
			{
				this.currency.stress = 0f;
			}
			this.fpc.enabled = true;
			this.dirtBike.SetActive(true);
			this.person.transform.position = this.townPosMe.position;
		}
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x0005C9E4 File Offset: 0x0005ABE4
	public void Option9()
	{
		this.cost = 45;
		if (this.currency.money >= (float)this.cost && this.person.transform.parent == null)
		{
			this.person.transform.position = this.home2PosMe.position;
			this.phonePanel.SetActive(false);
			this.fpc.LockMouse();
			this.person.GetComponent<Interactor>().inv.SubtractMoney((float)this.cost);
			this.currency.money -= (float)this.cost;
			if (this.currency.nicotine < 3f)
			{
				this.currency.stress -= 33f;
			}
			if (this.currency.stress < 0f)
			{
				this.currency.stress = 0f;
			}
			this.fpc.enabled = true;
		}
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x0005CAEC File Offset: 0x0005ACEC
	public void Option10()
	{
		this.cost = 45;
		if (this.currency.money >= (float)this.cost && this.person.transform.parent == null)
		{
			if (Vector3.Distance(this.truck.transform.position, this.home2PosCar.position) < 4f)
			{
				this.truck.SetActive(false);
				this.truck.transform.position = this.home2PosTruck.position;
				this.truck.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.truckdoor_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.truckdoor_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.fob.transform.rotation = Quaternion.Euler(new Vector3(50f, 34f, 18f));
				this.truck.SetActive(true);
			}
			this.car.SetActive(false);
			if (Vector3.Distance(this.person.transform.position, this.home2PosMe.position) > 30f)
			{
				this.person.transform.position = this.home2PosMe.position;
			}
			this.car.transform.position = this.home2PosCar.position;
			this.car.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.cardoor_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.cardoor_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.phonePanel.SetActive(false);
			this.fpc.LockMouse();
			this.currency.money -= (float)this.cost;
			if (this.currency.nicotine < 3f)
			{
				this.currency.stress -= 50f;
			}
			if (this.currency.stress < 0f)
			{
				this.currency.stress = 0f;
			}
			this.person.GetComponent<Interactor>().inv.SubtractMoney((float)this.cost);
			this.fpc.enabled = true;
			this.car.SetActive(true);
			this.car.GetComponent<car3>().userControlled = false;
			this.car.GetComponent<car3>().TurnOff();
			this.car.GetComponent<car3>().turnOffAcc();
			this.person.GetComponent<Interactor>().keyStateC = 0;
		}
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x0005CE0C File Offset: 0x0005B00C
	public void Option11()
	{
		this.cost = 45;
		if (this.currency.money >= (float)this.cost && this.truck.activeSelf && this.person.transform.parent == null)
		{
			if (Vector3.Distance(this.car.transform.position, this.home2PosTruck.position) < 4f)
			{
				this.car.SetActive(false);
				this.car.transform.position = this.home2PosCar.position;
				this.car.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.cardoor_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.cardoor_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.car.SetActive(true);
			}
			this.DisconnectTrailers();
			this.winch.DetachHook();
			this.winch2.DetachHook();
			this.truck.SetActive(false);
			if (Vector3.Distance(this.person.transform.position, this.home2PosMe.position) > 30f)
			{
				this.person.transform.position = this.home2PosMe.position;
			}
			this.truck.transform.position = this.home2PosTruck.position;
			this.truck.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.truckdoor_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.truckdoor_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.fob.transform.rotation = Quaternion.Euler(new Vector3(50f, 34f, 18f));
			this.phonePanel.SetActive(false);
			this.fpc.LockMouse();
			this.person.GetComponent<Interactor>().inv.SubtractMoney((float)this.cost);
			this.currency.money -= (float)this.cost;
			if (this.currency.nicotine < 3f)
			{
				this.currency.stress -= 50f;
			}
			if (this.currency.stress < 0f)
			{
				this.currency.stress = 0f;
			}
			this.fpc.enabled = true;
			this.truck.SetActive(true);
		}
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x0005D114 File Offset: 0x0005B314
	public void Option12()
	{
		this.cost = 45;
		if (this.currency.money >= (float)this.cost && this.dirtBike.GetComponent<PickUp>().price == 0f && this.person.transform.parent == null)
		{
			this.dirtBike.SetActive(false);
			this.dirtBike.transform.position = this.home2Pos250.position;
			this.dirtBike.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.phonePanel.SetActive(false);
			this.fpc.LockMouse();
			this.person.GetComponent<Interactor>().inv.SubtractMoney((float)this.cost);
			this.currency.money -= (float)this.cost;
			if (this.currency.nicotine < 3f)
			{
				this.currency.stress -= 25f;
			}
			if (this.currency.stress < 0f)
			{
				this.currency.stress = 0f;
			}
			this.fpc.enabled = true;
			this.dirtBike.SetActive(true);
			this.person.transform.position = this.home2PosMe.position;
		}
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x0005D290 File Offset: 0x0005B490
	public void Option13()
	{
		this.cost = 45;
		if (this.currency.money >= (float)this.cost && this.truck2.activeSelf && this.person.transform.parent == null)
		{
			if (Vector3.Distance(this.car.transform.position, this.homePosTruck2.position) < 4f)
			{
				this.car.SetActive(false);
				this.car.transform.position = this.homePosCar.position;
				this.car.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.cardoor_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.cardoor_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.car.SetActive(true);
			}
			this.DisconnectTrailers();
			this.winch2.DetachHook();
			this.winch.DetachHook();
			this.truck2.SetActive(false);
			if (Vector3.Distance(this.person.transform.position, this.homePosMe.position) > 30f)
			{
				this.person.transform.position = this.homePosMe.position;
			}
			this.truck2.transform.position = this.homePosTruck2.position;
			this.truck2.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.truckdoor2_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.truckdoor2_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.phonePanel.SetActive(false);
			this.fpc.LockMouse();
			this.person.GetComponent<Interactor>().inv.SubtractMoney((float)this.cost);
			this.currency.money -= (float)this.cost;
			if (this.currency.nicotine < 3f)
			{
				this.currency.stress -= 50f;
			}
			if (this.currency.stress < 0f)
			{
				this.currency.stress = 0f;
			}
			this.fpc.enabled = true;
			this.truck2.SetActive(true);
		}
	}

	// Token: 0x0600072E RID: 1838 RVA: 0x0005D570 File Offset: 0x0005B770
	public void Option14()
	{
		this.cost = 45;
		if (this.currency.money >= (float)this.cost && this.truck2.activeSelf && this.person.transform.parent == null)
		{
			this.DisconnectTrailers();
			this.winch2.DetachHook();
			this.winch.DetachHook();
			this.truck2.SetActive(false);
			if (Vector3.Distance(this.person.transform.position, this.townPosMe.position) > 30f)
			{
				this.person.transform.position = this.townPosMe.position;
			}
			this.truck2.transform.position = this.townPosTruck2.position;
			this.truck2.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.truckdoor2_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.truckdoor2_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.phonePanel.SetActive(false);
			this.fpc.LockMouse();
			this.currency.money -= (float)this.cost;
			if (this.currency.nicotine < 3f)
			{
				this.currency.stress -= 50f;
			}
			if (this.currency.stress < 0f)
			{
				this.currency.stress = 0f;
			}
			this.person.GetComponent<Interactor>().inv.SubtractMoney((float)this.cost);
			this.fpc.enabled = true;
			this.truck2.SetActive(true);
		}
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x0005D778 File Offset: 0x0005B978
	public void Option15()
	{
		this.cost = 45;
		if (this.currency.money >= (float)this.cost && this.truck2.activeSelf && this.person.transform.parent == null)
		{
			if (Vector3.Distance(this.car.transform.position, this.home2PosTruck2.position) < 4f)
			{
				this.car.SetActive(false);
				this.car.transform.position = this.home2PosCar.position;
				this.car.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.cardoor_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.cardoor_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.car.SetActive(true);
			}
			this.DisconnectTrailers();
			this.winch2.DetachHook();
			this.winch.DetachHook();
			this.truck2.SetActive(false);
			if (Vector3.Distance(this.person.transform.position, this.home2PosMe.position) > 30f)
			{
				this.person.transform.position = this.home2PosMe.position;
			}
			this.truck2.transform.position = this.home2PosTruck2.position;
			this.truck2.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.truckdoor2_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.truckdoor2_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.phonePanel.SetActive(false);
			this.fpc.LockMouse();
			this.person.GetComponent<Interactor>().inv.SubtractMoney((float)this.cost);
			this.currency.money -= (float)this.cost;
			if (this.currency.nicotine < 3f)
			{
				this.currency.stress -= 50f;
			}
			if (this.currency.stress < 0f)
			{
				this.currency.stress = 0f;
			}
			this.fpc.enabled = true;
			this.truck2.SetActive(true);
		}
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x0005DA58 File Offset: 0x0005BC58
	public void Option4b()
	{
		Debug.Log("4b");
		if (this.person.transform.parent == null)
		{
			this.dirtBike.SetActive(false);
			this.dirtBike.transform.position = this.homePos250.position;
			this.dirtBike.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.phonePanel.SetActive(false);
			this.fpc.LockMouse();
			this.fpc.enabled = true;
			this.dirtBike.SetActive(true);
			this.person.transform.position = this.homePosMe.position;
		}
	}

	// Token: 0x06000731 RID: 1841 RVA: 0x0005DB24 File Offset: 0x0005BD24
	public void Option6b()
	{
		if (this.truck2.activeSelf && this.person.transform.parent == null)
		{
			if (Vector3.Distance(this.car.transform.position, this.homePosTruck2.position) < 4f)
			{
				this.car.SetActive(false);
				this.car.transform.position = this.homePosCar.position;
				this.car.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.cardoor_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.cardoor_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.car.SetActive(true);
			}
			this.DisconnectTrailers();
			this.winch2.DetachHook();
			this.winch.DetachHook();
			this.truck2.SetActive(false);
			this.person.transform.position = this.homePosMe.position;
			this.truck2.transform.position = this.homePosTruck2.position;
			this.truck2.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.truckdoor2_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.truckdoor2_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.phonePanel.SetActive(false);
			this.fpc.LockMouse();
			this.fpc.enabled = true;
			this.truck2.SetActive(true);
		}
	}

	// Token: 0x06000732 RID: 1842 RVA: 0x0005DD3C File Offset: 0x0005BF3C
	public void OptionWT()
	{
		this.cost = 100;
		if (this.currency.money >= (float)this.cost)
		{
			this.DisconnectTrailers();
			this.waterTrailer.SetActive(false);
			this.waterTrailer.transform.position = this.posWaterTrailer.position;
			this.waterTrailer.transform.rotation = this.posWaterTrailer.rotation;
			this.phonePanel.SetActive(false);
			this.fpc.LockMouse();
			this.person.GetComponent<Interactor>().inv.SubtractMoney((float)this.cost);
			this.currency.money -= (float)this.cost;
			if (this.currency.nicotine < 3f)
			{
				this.currency.stress -= 25f;
			}
			if (this.currency.stress < 0f)
			{
				this.currency.stress = 0f;
			}
			this.fpc.enabled = true;
			this.waterTrailer.SetActive(true);
		}
	}

	// Token: 0x06000733 RID: 1843 RVA: 0x0005DE60 File Offset: 0x0005C060
	public void OptionCT()
	{
		this.cost = 100;
		if (this.currency.money >= (float)this.cost)
		{
			this.DisconnectTrailers();
			this.containerTrailer.SetActive(false);
			this.containerTrailer.transform.position = this.posContainerTrailer.position;
			this.containerTrailer.transform.rotation = this.posContainerTrailer.rotation;
			this.phonePanel.SetActive(false);
			this.fpc.LockMouse();
			this.person.GetComponent<Interactor>().inv.SubtractMoney((float)this.cost);
			this.currency.money -= (float)this.cost;
			if (this.currency.nicotine < 3f)
			{
				this.currency.stress -= 25f;
			}
			if (this.currency.stress < 0f)
			{
				this.currency.stress = 0f;
			}
			this.fpc.enabled = true;
			this.containerTrailer.SetActive(true);
		}
	}

	// Token: 0x06000734 RID: 1844 RVA: 0x0005DF84 File Offset: 0x0005C184
	public void OptionFT()
	{
		this.cost = 100;
		if (this.currency.money >= (float)this.cost)
		{
			this.DisconnectTrailers();
			this.flatTrailer.SetActive(false);
			this.flatTrailer.transform.position = this.posFlatTrailer.position;
			this.flatTrailer.transform.rotation = this.posFlatTrailer.rotation;
			this.phonePanel.SetActive(false);
			this.fpc.LockMouse();
			this.person.GetComponent<Interactor>().inv.SubtractMoney((float)this.cost);
			this.currency.money -= (float)this.cost;
			if (this.currency.nicotine < 3f)
			{
				this.currency.stress -= 25f;
			}
			if (this.currency.stress < 0f)
			{
				this.currency.stress = 0f;
			}
			this.fpc.enabled = true;
			this.flatTrailer.SetActive(true);
		}
	}

	// Token: 0x06000735 RID: 1845 RVA: 0x0005E0A8 File Offset: 0x0005C2A8
	public void Impound(int veh)
	{
		if (veh == 1)
		{
			this.DisconnectTrailers();
			this.winch.DetachHook();
			this.winch2.DetachHook();
			this.CheckForBeefareeno(this.truck.transform);
			this.truck.SetActive(false);
			this.truck.transform.position = this.impoundLoc[0].position;
			this.truck.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.truckdoor_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.truckdoor_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.fob.transform.rotation = Quaternion.Euler(new Vector3(50f, 34f, 18f));
			this.truck.SetActive(true);
			this.interactor.diamondbackImpounded = true;
		}
		if (veh == 2)
		{
			this.DisconnectTrailers();
			this.winch2.DetachHook();
			this.winch.DetachHook();
			this.CheckForBeefareeno(this.truck2.transform);
			this.truck2.SetActive(false);
			this.truck2.transform.position = this.impoundLoc[1].position;
			this.truck2.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.truckdoor2_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.truckdoor2_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.truck2.SetActive(true);
			this.interactor.f100Impounded = true;
		}
		if (veh == 3)
		{
			this.CheckForBeefareeno(this.car.transform);
			this.car.SetActive(false);
			this.car.transform.position = this.impoundLoc[2].position;
			this.car.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.cardoor_p.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.cardoor_d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.car.SetActive(true);
			this.interactor.amcImpounded = true;
		}
		if (veh == 4)
		{
			this.CheckForBeefareeno(this.dirtBike.transform);
			this.dirtBike.SetActive(false);
			this.dirtBike.transform.position = this.impoundLoc[3].position;
			this.dirtBike.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.dirtBike.SetActive(true);
			this.interactor.dirtbikeImpounded = true;
		}
		if (veh == 5)
		{
			this.CheckForBeefareeno(this.golfCart.transform);
			this.golfCart.SetActive(false);
			this.golfCart.transform.position = this.impoundLoc[4].position;
			this.golfCart.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			this.golfCart.SetActive(true);
			this.interactor.golfcartImpounded = true;
		}
	}

	// Token: 0x06000736 RID: 1846 RVA: 0x0005E498 File Offset: 0x0005C698
	public void DisconnectTrailers()
	{
		this.wtcj.connectedBody = null;
		this.wtcj.xMotion = ConfigurableJointMotion.Free;
		this.wtcj.yMotion = ConfigurableJointMotion.Free;
		this.wtcj.zMotion = ConfigurableJointMotion.Free;
		this.htcj.connectedBody = null;
		this.htcj.xMotion = ConfigurableJointMotion.Free;
		this.htcj.yMotion = ConfigurableJointMotion.Free;
		this.htcj.zMotion = ConfigurableJointMotion.Free;
		this.ctcj.connectedBody = null;
		this.ctcj.xMotion = ConfigurableJointMotion.Free;
		this.ctcj.yMotion = ConfigurableJointMotion.Free;
		this.ctcj.zMotion = ConfigurableJointMotion.Free;
		this.gencj.connectedBody = null;
		this.gencj.xMotion = ConfigurableJointMotion.Free;
		this.gencj.yMotion = ConfigurableJointMotion.Free;
		this.gencj.zMotion = ConfigurableJointMotion.Free;
	}

	// Token: 0x06000737 RID: 1847 RVA: 0x0005E568 File Offset: 0x0005C768
	private bool CheckForBeefareeno(Transform thisVeh)
	{
		foreach (Collider collider in Physics.OverlapSphere(thisVeh.position, 2f))
		{
			if (collider.gameObject.name.Contains("beefareeno"))
			{
				Object.Destroy(collider.gameObject);
				if (!this.ach_beefareeno)
				{
					this.ach_beefareeno = true;
					Achievement achievement = new Achievement("ACH_BEEFAREENO");
					achievement.Trigger(true);
				}
			}
		}
		return false;
	}

	// Token: 0x06000738 RID: 1848 RVA: 0x0005E5DF File Offset: 0x0005C7DF
	public void OptionExit()
	{
		this.vehMenu.SetActive(true);
		this.destMenu.SetActive(false);
		this.phonePanel.SetActive(false);
		this.fpc.LockMouse();
		this.fpc.enabled = true;
	}

	// Token: 0x04000FF7 RID: 4087
	public FirstPersonController fpc;

	// Token: 0x04000FF8 RID: 4088
	public GameObject phonePanel;

	// Token: 0x04000FF9 RID: 4089
	public GameObject person;

	// Token: 0x04000FFA RID: 4090
	public GameObject truck;

	// Token: 0x04000FFB RID: 4091
	public GameObject car;

	// Token: 0x04000FFC RID: 4092
	public GameObject truck2;

	// Token: 0x04000FFD RID: 4093
	public GameObject golfCart;

	// Token: 0x04000FFE RID: 4094
	public Transform homePosTruck;

	// Token: 0x04000FFF RID: 4095
	public Transform homePosTruck2;

	// Token: 0x04001000 RID: 4096
	public Transform homePosCar;

	// Token: 0x04001001 RID: 4097
	public Transform homePos250;

	// Token: 0x04001002 RID: 4098
	public Transform homePosMe;

	// Token: 0x04001003 RID: 4099
	public Transform townPosTruck;

	// Token: 0x04001004 RID: 4100
	public Transform townPosTruck2;

	// Token: 0x04001005 RID: 4101
	public Transform townPosCar;

	// Token: 0x04001006 RID: 4102
	public Transform townPos250;

	// Token: 0x04001007 RID: 4103
	public Transform townPosMe;

	// Token: 0x04001008 RID: 4104
	public Transform posWaterTrailer;

	// Token: 0x04001009 RID: 4105
	public Transform posContainerTrailer;

	// Token: 0x0400100A RID: 4106
	public Transform posFlatTrailer;

	// Token: 0x0400100B RID: 4107
	public GameObject waterTrailer;

	// Token: 0x0400100C RID: 4108
	public GameObject containerTrailer;

	// Token: 0x0400100D RID: 4109
	public GameObject flatTrailer;

	// Token: 0x0400100E RID: 4110
	public Transform home2PosTruck;

	// Token: 0x0400100F RID: 4111
	public Transform home2PosTruck2;

	// Token: 0x04001010 RID: 4112
	public Transform home2PosCar;

	// Token: 0x04001011 RID: 4113
	public Transform home2Pos250;

	// Token: 0x04001012 RID: 4114
	public Transform home2PosMe;

	// Token: 0x04001013 RID: 4115
	public Currency currency;

	// Token: 0x04001014 RID: 4116
	public GameObject cardoor_p;

	// Token: 0x04001015 RID: 4117
	public GameObject cardoor_d;

	// Token: 0x04001016 RID: 4118
	public GameObject truckdoor_p;

	// Token: 0x04001017 RID: 4119
	public GameObject truckdoor_d;

	// Token: 0x04001018 RID: 4120
	public GameObject truckdoor2_p;

	// Token: 0x04001019 RID: 4121
	public GameObject truckdoor2_d;

	// Token: 0x0400101A RID: 4122
	public GameObject fob;

	// Token: 0x0400101B RID: 4123
	private int cost;

	// Token: 0x0400101C RID: 4124
	public Winch winch;

	// Token: 0x0400101D RID: 4125
	public Winch winch2;

	// Token: 0x0400101E RID: 4126
	public ConfigurableJoint wtcj;

	// Token: 0x0400101F RID: 4127
	public ConfigurableJoint htcj;

	// Token: 0x04001020 RID: 4128
	public ConfigurableJoint ctcj;

	// Token: 0x04001021 RID: 4129
	public ConfigurableJoint gencj;

	// Token: 0x04001022 RID: 4130
	public Terrain terrainOb;

	// Token: 0x04001023 RID: 4131
	public GameObject dirtBike;

	// Token: 0x04001024 RID: 4132
	public GameObject optionhouse2;

	// Token: 0x04001025 RID: 4133
	public GameObject optiontruck2;

	// Token: 0x04001026 RID: 4134
	public GameObject optionMoto;

	// Token: 0x04001027 RID: 4135
	public GameObject optionDiamondback;

	// Token: 0x04001028 RID: 4136
	public GameObject optionEagle;

	// Token: 0x04001029 RID: 4137
	public Atm atm;

	// Token: 0x0400102A RID: 4138
	public Transform[] impoundLoc;

	// Token: 0x0400102B RID: 4139
	public GameObject vehMenu;

	// Token: 0x0400102C RID: 4140
	public GameObject destMenu;

	// Token: 0x0400102D RID: 4141
	private int thisVeh;

	// Token: 0x0400102E RID: 4142
	public Interactor interactor;

	// Token: 0x0400102F RID: 4143
	private bool ach_beefareeno;
}
