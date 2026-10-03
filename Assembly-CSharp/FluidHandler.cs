using System;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000B2 RID: 178
public class FluidHandler : MonoBehaviour
{
	// Token: 0x06000447 RID: 1095 RVA: 0x0002D3E4 File Offset: 0x0002B5E4
	private void Start()
	{
		this.addingFluid = false;
		if (this.aSource == null && base.GetComponent<AudioSource>() != null)
		{
			this.aSource = base.GetComponent<AudioSource>();
		}
		if (this.fuelNozzle != null)
		{
			this.nozzlePos = this.fuelNozzle.transform.position;
			this.nozzleRot = this.fuelNozzle.transform.rotation;
		}
		if (this.fluidType == 7 || this.fluidType == 21 || this.fluidType == 22)
		{
			if (this.truck == null)
			{
				this.truck = GameObject.Find("dirt pickup truck");
			}
			if (this.diamondbackscript == null)
			{
				this.diamondbackscript = this.truck.GetComponent<car>();
			}
			if (this.truck2 == null)
			{
				this.truck2 = GameObject.Find("f1003");
			}
			if (this.f100script == null)
			{
				this.f100script = this.truck2.GetComponent<car4>();
			}
		}
		if (this.fluidType == 23 || this.RWGtruck == null)
		{
			this.RWGtruck = GameObject.Find("RWGtruck");
			if (this.RWGtruck == null)
			{
				this.RWGtruck = GameObject.Find("RWGtruck(Clone)");
			}
		}
		if (this.fluidType == 13 && this.chainsaw == null)
		{
			this.chainsaw = GameObject.Find("Chainsaw2");
			this.chainsawFuelLid = this.chainsaw.transform.GetChild(0).gameObject.GetComponent<InteractiveObject>();
		}
		if (base.name.Contains("MotorOil"))
		{
			if (this.engineblock4 == null || this.amc == null)
			{
				this.engineblock4 = GameObject.Find("engineblock");
				this.amc = GameObject.Find("amc");
				this.amcscript = this.amc.GetComponent<car3>();
			}
			if (this.engineblockv8 == null)
			{
				this.engineblockv8 = GameObject.Find("v8_block");
			}
			if (this.engine250 == null)
			{
				this.engine250 = GameObject.Find("250_block").GetComponent<Engine250>();
			}
			if (this.engineblocki6 == null)
			{
				this.engineblocki6 = GameObject.Find("i6block");
			}
			if (this.engine == null || this.enginev8 == null || this.enginei6 == null)
			{
				this.engine = this.engineblock4.GetComponent<engine>();
				this.enginev8 = this.engineblockv8.GetComponent<enginev8>();
				this.enginei6 = this.engineblocki6.GetComponent<enginei6>();
			}
		}
		if (this.fluidType == 15 && this.engine250 == null)
		{
			this.engine250 = GameObject.Find("250_block").GetComponent<Engine250>();
		}
		if (base.name.Contains("coolant") && this.engine250 == null)
		{
			this.engine250 = GameObject.Find("250_block").GetComponent<Engine250>();
		}
		if (base.name.Contains("additive") || base.name.Contains("jerrycan"))
		{
			if (this.truck == null)
			{
				this.truck = GameObject.Find("dirt pickup truck");
			}
			if (this.truck2 == null)
			{
				this.truck2 = GameObject.Find("f1003");
			}
			if (this.engine250 == null)
			{
				this.engine250 = GameObject.Find("250_block").GetComponent<Engine250>();
			}
			if (this.amc == null)
			{
				this.amc = GameObject.Find("amc");
			}
			if (base.name.Contains("jerrycan"))
			{
				this.jerrycan = base.gameObject;
				this.jerryFuelLid = base.gameObject.transform.GetChild(1).gameObject.GetComponent<InteractiveObject>();
			}
		}
	}

	// Token: 0x06000448 RID: 1096 RVA: 0x0002D7E3 File Offset: 0x0002B9E3
	public void ReturnNozzle()
	{
		this.addingFluid = false;
		base.transform.position = this.nozzlePos;
		base.transform.rotation = this.nozzleRot;
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x0002D810 File Offset: 0x0002BA10
	private void Update()
	{
		if (base.transform.parent == null)
		{
			base.enabled = false;
		}
		if (this.fluidType == 7 && this.truck2 == null)
		{
			this.truck2 = GameObject.Find("f1003");
		}
		if (base.name == "FuelSage" && (base.transform.parent == null || base.transform.parent.gameObject.name != "pump4"))
		{
			this.newNozzlePos = this.fuelNozzle.transform.position;
			if (this.newNozzlePos != this.nozzlePos && Vector3.Distance(this.nozzlePos, this.newNozzlePos) > 6f)
			{
				base.GetComponent<PickUp>().LetGo(0);
				this.ReturnNozzle();
			}
			float num = Vector3.Distance(base.transform.position, this.truckNozzle.position);
			float num2 = Vector3.Distance(base.transform.position, this.carNozzle.position);
			float num3 = Vector3.Distance(base.transform.position, this.canNozzle.position);
			float num4 = Vector3.Distance(base.transform.position, this.bikeNozzle.position);
			float num5 = Vector3.Distance(base.transform.position, this.f100Nozzle.position);
			if (num < 0.5f || num2 < 0.5f || num3 < 0.5f || num4 < 0.5f || num5 < 0.5f)
			{
				this.addingFluid = true;
			}
			else
			{
				this.addingFluid = false;
			}
		}
		if (this.addingFluid)
		{
			if (this.fluidType == 1)
			{
				if (this.engine == null || this.engineblock4 == null)
				{
					this.engineblock4 = GameObject.Find("engineblock");
					this.engine = this.engineblock4.GetComponent<engine>();
				}
				if (this.engine.newOilLevel < 100f && this.fluidlevel > 0)
				{
					this.engine.newOilLevel += 1f;
					this.fluidlevel--;
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
			}
			else if (this.fluidType == 14)
			{
				if (this.enginev8 == null || this.engineblockv8 == null)
				{
					this.engineblockv8 = GameObject.Find("v8_block");
					this.enginev8 = this.engineblockv8.GetComponent<enginev8>();
				}
				if (this.enginev8.newOilLevel < 100f && this.fluidlevel > 0)
				{
					this.enginev8.newOilLevel += 1f;
					this.fluidlevel--;
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
			}
			else if (this.fluidType == 16)
			{
				if (this.engine250 == null)
				{
					this.engine250 = GameObject.Find("250_block").GetComponent<Engine250>();
				}
				if (this.engine250.newOilLevel < 100f && this.fluidlevel > 0)
				{
					this.engine250.newOilLevel += 1f;
					this.fluidlevel--;
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
			}
			else if (this.fluidType == 2 && this.truckFuelLid.isOpen)
			{
				if (this.truck == null || this.diamondbackscript == null)
				{
					this.truck = GameObject.Find("dirt pickup truck");
					this.diamondbackscript = this.truck.GetComponent<car>();
				}
				if (this.diamondbackscript.fuel < 1300f && this.fluidlevel > 0)
				{
					this.diamondbackscript.fuel += 1f;
					this.fuelPrice += 0.03f;
					this.fuelPrice2 += 0.03f;
					this.gasStationTill.AddGas();
					this.fuelpricetext.text = "$ " + this.fuelPrice.ToString("F2");
					this.fuelpricetext2.text = "$ " + this.fuelPrice.ToString("F2");
					this.fuelGallons += 0.01;
					this.fuelgallonstext.text = this.fuelGallons.ToString("F2");
					this.fuelgallonstext2.text = this.fuelGallons.ToString("F2");
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
			}
			else if (this.fluidType == 3 && this.carFuelLid.isOpen)
			{
				if (this.amc == null || this.amcscript == null)
				{
					this.amc = GameObject.Find("amc");
					this.amcscript = this.amc.GetComponent<car3>();
				}
				if (this.amcscript.fuel < 1300f && this.fluidlevel > 0)
				{
					this.amcscript.fuel += 1f;
					this.fuelPrice += 0.03f;
					this.fuelPrice2 += 0.03f;
					this.gasStationTill.AddGas();
					this.fuelpricetext.text = "$ " + this.fuelPrice.ToString("F2");
					this.fuelpricetext2.text = "$ " + this.fuelPrice.ToString("F2");
					this.fuelGallons += 0.01;
					this.fuelgallonstext.text = this.fuelGallons.ToString("F2");
					this.fuelgallonstext2.text = this.fuelGallons.ToString("F2");
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
					}
					if (this.amc.GetComponent<car3>().fuel >= 1300f)
					{
						Achievement achievement = new Achievement("ACH_TYCOON");
						achievement.Trigger(true);
						return;
					}
				}
			}
			else if (this.fluidType == 17)
			{
				if (this.engine250 == null)
				{
					this.engine250 = GameObject.Find("250_block").GetComponent<Engine250>();
				}
				if (this.engine250.newFuelLevel < 1300f && this.fluidlevel > 0)
				{
					this.engine250.newFuelLevel += 1f;
					this.fuelPrice += 0.03f;
					this.fuelPrice2 += 0.03f;
					this.gasStationTill.AddGas();
					this.fuelpricetext.text = "$ " + this.fuelPrice.ToString("F2");
					this.fuelpricetext2.text = "$ " + this.fuelPrice.ToString("F2");
					this.fuelGallons += 0.01;
					this.fuelgallonstext.text = this.fuelGallons.ToString("F2");
					this.fuelgallonstext2.text = this.fuelGallons.ToString("F2");
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
			}
			else if (this.fluidType == 8)
			{
				if (this.amc == null || this.amcscript == null)
				{
					this.amc = GameObject.Find("amc");
					this.amcscript = this.amc.GetComponent<car3>();
				}
				if (this.amcscript.oilLevel < 100f && this.fluidlevel > 0)
				{
					this.amc.GetComponent<car3>().oilLevel += 1f;
					this.fluidlevel--;
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
			}
			else if (this.fluidType == 7)
			{
				if (this.truck == null || this.diamondbackscript == null)
				{
					this.truck = GameObject.Find("dirt pickup truck");
					this.diamondbackscript = this.truck.GetComponent<car>();
				}
				if (this.diamondbackscript.coolantLevel < 100f && this.fluidlevel > 0)
				{
					this.diamondbackscript.coolantLevel += 1f;
					this.fluidlevel--;
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
			}
			else if (this.fluidType == 19)
			{
				if (this.engine250 == null)
				{
					this.engine250 = GameObject.Find("250_block").GetComponent<Engine250>();
				}
				if (this.engine250.newCoolantLevel < 100f && this.fluidlevel > 0)
				{
					this.engine250.newCoolantLevel += 1f;
					this.fluidlevel--;
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
			}
			else if (this.fluidType == 22)
			{
				if (this.truck2 == null || this.f100script == null)
				{
					this.truck2 = GameObject.Find("f1003");
					this.f100script = this.truck2.GetComponent<car4>();
				}
				if (this.f100script.coolantLevel < 100f && this.fluidlevel > 0)
				{
					this.f100script.coolantLevel += 1f;
					this.fluidlevel--;
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
			}
			else if (this.fluidType == 9 && this.jerryFuelLid.isOpen)
			{
				if (this.jerrycan.GetComponent<FluidHandler>().fluidlevel < 500 && this.fluidlevel > 0)
				{
					this.aSource = base.GetComponent<AudioSource>();
					this.jerrycan.GetComponent<FluidHandler>().fluidlevel++;
					this.jerrycan.GetComponent<JerryCan>().UpdatePerc();
					this.fuelPrice += 0.03f;
					this.fuelPrice2 += 0.03f;
					this.gasStationTill.AddGas();
					this.fuelpricetext.text = "$ " + this.fuelPrice.ToString("F2");
					this.fuelpricetext2.text = "$ " + this.fuelPrice.ToString("F2");
					this.fuelGallons += 0.01;
					this.fuelgallonstext.text = this.fuelGallons.ToString("F2");
					this.fuelgallonstext2.text = this.fuelGallons.ToString("F2");
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
			}
			else if (this.fluidType == 11)
			{
				if (this.truckFuelLid.isOpen && this.truck.GetComponent<car>().fuel < 1300f && this.fluidlevel > 0)
				{
					this.truck.GetComponent<car>().fuel += 1f;
					this.fluidlevel--;
					this.jerrycan.GetComponent<JerryCan>().UpdatePerc();
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
			}
			else if (this.fluidType == 12)
			{
				if (this.carFuelLid.isOpen && this.amc.GetComponent<car3>().fuel < 1300f && this.fluidlevel > 0)
				{
					this.amc.GetComponent<car3>().fuel += 1f;
					this.fluidlevel--;
					this.jerrycan.GetComponent<JerryCan>().UpdatePerc();
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
			}
			else if (this.fluidType == 18)
			{
				if (this.engine250.newFuelLevel < 1300f && this.fluidlevel > 0)
				{
					this.engine250.newFuelLevel += 1f;
					this.fluidlevel--;
					this.jerrycan.GetComponent<JerryCan>().UpdatePerc();
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
			}
			else if (this.fluidType == 13)
			{
				if (this.chainsawFuelLid.isOpen && this.chainsaw.GetComponent<chainsaw2>().fuel < 100f && this.fluidlevel > 0)
				{
					this.chainsaw.GetComponent<chainsaw2>().fuel += 1f;
					this.fluidlevel--;
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
			}
			else if (this.fluidType == 15)
			{
				if (this.engine250.transFluid < 100f && this.fluidlevel > 0)
				{
					this.engine250.transFluid += 1f;
					this.fluidlevel--;
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
			}
			else if (this.fluidType == 20 && this.f100FuelLid.isOpen)
			{
				if (this.f100script.fuel < 1300f && this.fluidlevel > 0)
				{
					this.f100script.fuel += 1f;
					this.fuelPrice += 0.03f;
					this.fuelPrice2 += 0.03f;
					this.gasStationTill.AddGas();
					this.fuelpricetext.text = "$ " + this.fuelPrice.ToString("F2");
					this.fuelpricetext2.text = "$ " + this.fuelPrice.ToString("F2");
					this.fuelGallons += 0.01;
					this.fuelgallonstext.text = this.fuelGallons.ToString("F2");
					this.fuelgallonstext2.text = this.fuelGallons.ToString("F2");
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
			}
			else if (this.fluidType == 21)
			{
				if (this.f100FuelLid.isOpen && this.truck2.GetComponent<car4>().fuel < 1300f && this.fluidlevel > 0)
				{
					this.truck2.GetComponent<car4>().fuel += 1f;
					this.fluidlevel--;
					this.jerrycan.GetComponent<JerryCan>().UpdatePerc();
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
			}
			else if (this.fluidType == 23)
			{
				if (this.RWGtruck.GetComponent<RWGfuel>().fuel < 100 && this.fluidlevel > 0)
				{
					this.RWGtruck.GetComponent<RWGfuel>().fuel++;
					this.fluidlevel--;
					this.jerrycan.GetComponent<JerryCan>().UpdatePerc();
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
				else if (this.RWGtruck.GetComponent<RWGfuel>().fuel >= 100 && this.mg.missionType == 10)
				{
					this.mg.DoCompletion();
					return;
				}
			}
			else if (this.fluidType == 24)
			{
				if (this.enginei6 == null || this.engineblocki6 == null)
				{
					this.engineblocki6 = GameObject.Find("i6block");
					this.enginei6 = this.engineblocki6.GetComponent<enginei6>();
				}
				if (this.enginei6.newOilLevel < 100f && this.fluidlevel > 0)
				{
					this.enginei6.newOilLevel += 1f;
					this.fluidlevel--;
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
			}
			else if (this.fluidType == 25)
			{
				if (this.truck.GetComponent<car>().additive < 20f && this.fluidlevel > 0)
				{
					this.truck.GetComponent<car>().additive += 1f;
					this.fluidlevel--;
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
			}
			else if (this.fluidType == 26)
			{
				if (this.truck2.GetComponent<car4>().additive < 20f && this.fluidlevel > 0)
				{
					this.truck2.GetComponent<car4>().additive += 1f;
					this.fluidlevel--;
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
			}
			else if (this.fluidType == 27)
			{
				if (this.engine250.additive < 20f && this.fluidlevel > 0)
				{
					this.engine250.additive += 1f;
					this.fluidlevel--;
					if (!this.aSource.isPlaying)
					{
						this.aSource.Play();
						return;
					}
				}
			}
			else if (this.fluidType == 28 && this.amc.GetComponent<car3>().additive < 20f && this.fluidlevel > 0)
			{
				this.amc.GetComponent<car3>().additive += 1f;
				this.fluidlevel--;
				if (!this.aSource.isPlaying)
				{
					this.aSource.Play();
				}
			}
		}
	}

	// Token: 0x040008B9 RID: 2233
	private bool canPour;

	// Token: 0x040008BA RID: 2234
	public int fluidlevel;

	// Token: 0x040008BB RID: 2235
	public GameObject truck;

	// Token: 0x040008BC RID: 2236
	public GameObject truck2;

	// Token: 0x040008BD RID: 2237
	public GameObject RWGtruck;

	// Token: 0x040008BE RID: 2238
	public GameObject amc;

	// Token: 0x040008BF RID: 2239
	public GameObject engineblock4;

	// Token: 0x040008C0 RID: 2240
	public GameObject engineblockv8;

	// Token: 0x040008C1 RID: 2241
	public GameObject engineblocki6;

	// Token: 0x040008C2 RID: 2242
	public GameObject jerrycan;

	// Token: 0x040008C3 RID: 2243
	public GameObject chainsaw;

	// Token: 0x040008C4 RID: 2244
	public bool addingFluid;

	// Token: 0x040008C5 RID: 2245
	public int fluidType;

	// Token: 0x040008C6 RID: 2246
	public float fuelPrice;

	// Token: 0x040008C7 RID: 2247
	public float fuelPrice2;

	// Token: 0x040008C8 RID: 2248
	public float thispressure;

	// Token: 0x040008C9 RID: 2249
	public float newpressure;

	// Token: 0x040008CA RID: 2250
	public double fuelGallons;

	// Token: 0x040008CB RID: 2251
	public AudioSource aSource;

	// Token: 0x040008CC RID: 2252
	public GameObject fuelNozzle;

	// Token: 0x040008CD RID: 2253
	public InteractiveObject truckFuelLid;

	// Token: 0x040008CE RID: 2254
	public InteractiveObject carFuelLid;

	// Token: 0x040008CF RID: 2255
	public InteractiveObject jerryFuelLid;

	// Token: 0x040008D0 RID: 2256
	public InteractiveObject chainsawFuelLid;

	// Token: 0x040008D1 RID: 2257
	public InteractiveObject f100FuelLid;

	// Token: 0x040008D2 RID: 2258
	public TillScript gasStationTill;

	// Token: 0x040008D3 RID: 2259
	private Vector3 nozzlePos;

	// Token: 0x040008D4 RID: 2260
	private Quaternion nozzleRot;

	// Token: 0x040008D5 RID: 2261
	private Vector3 newNozzlePos;

	// Token: 0x040008D6 RID: 2262
	public Transform truckNozzle;

	// Token: 0x040008D7 RID: 2263
	public Transform carNozzle;

	// Token: 0x040008D8 RID: 2264
	public Transform canNozzle;

	// Token: 0x040008D9 RID: 2265
	public Transform bikeNozzle;

	// Token: 0x040008DA RID: 2266
	public Transform f100Nozzle;

	// Token: 0x040008DB RID: 2267
	public Engine250 engine250;

	// Token: 0x040008DC RID: 2268
	public MissionGen mg;

	// Token: 0x040008DD RID: 2269
	public MissionController mc;

	// Token: 0x040008DE RID: 2270
	public engine engine;

	// Token: 0x040008DF RID: 2271
	public enginev8 enginev8;

	// Token: 0x040008E0 RID: 2272
	public enginei6 enginei6;

	// Token: 0x040008E1 RID: 2273
	public car diamondbackscript;

	// Token: 0x040008E2 RID: 2274
	public car3 amcscript;

	// Token: 0x040008E3 RID: 2275
	public car4 f100script;

	// Token: 0x040008E4 RID: 2276
	public Text fuelpricetext;

	// Token: 0x040008E5 RID: 2277
	public Text fuelpricetext2;

	// Token: 0x040008E6 RID: 2278
	public Text fuelgallonstext;

	// Token: 0x040008E7 RID: 2279
	public Text fuelgallonstext2;
}
