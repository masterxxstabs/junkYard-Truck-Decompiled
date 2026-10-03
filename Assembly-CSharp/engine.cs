using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000170 RID: 368
public class engine : MonoBehaviour
{
	// Token: 0x06000903 RID: 2307 RVA: 0x00075BDC File Offset: 0x00073DDC
	private void Start()
	{
		base.GetComponent<Rigidbody>().angularDrag = 0f;
		if (base.transform.parent != null)
		{
			if (base.transform.parent.name != "dirt pickup truck")
			{
				base.gameObject.GetComponent<PickUp>().pickable = true;
			}
			if (base.transform.parent.name == "dirt pickup truck")
			{
				base.transform.position = this.ebtemplate.position;
				base.transform.rotation = this.ebtemplate.rotation;
				base.gameObject.GetComponent<FixedJoint>().connectedBody = this.truckRb;
			}
		}
		else
		{
			base.gameObject.GetComponent<PickUp>().pickable = true;
		}
		if (base.transform.parent == null)
		{
			base.GetComponent<Rigidbody>().isKinematic = true;
			if (base.gameObject.GetComponent<FixedJoint>().connectedBody.name == "DK_9")
			{
				this.emptyRb.position = base.transform.position;
				base.gameObject.GetComponent<FixedJoint>().connectedBody = this.emptyRb.GetComponent<Rigidbody>();
			}
		}
		if (!this.intakeManEFI.GetComponent<Renderer>().enabled)
		{
			this.intakeManEFI.GetComponent<BoxCollider>().enabled = false;
		}
		if (this.airFilterTurboXL == null)
		{
			this.airFilterTurboXL = GameObject.Find("conefilterD_e");
			this.airFilterTurboXL_cnd_c = this.airFilterTurboXL.GetComponent<durability>();
		}
		this.avgDmgCab = this.idCab.AverageStructuralDamage;
		this.prevDmgCab = this.avgDmgCab;
		base.gameObject.transform.parent == this.truck.transform;
		base.StartCoroutine(this.RemoveDrag());
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x00075DBB File Offset: 0x00073FBB
	private IEnumerator RemoveDrag()
	{
		yield return new WaitForSeconds(1f);
		base.GetComponent<Rigidbody>().isKinematic = false;
		yield break;
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x00075DCC File Offset: 0x00073FCC
	public void CrashDamageCab()
	{
		this.avgDmgCab = this.idCab.AverageStructuralDamage;
		if (this.avgDmgCab > this.prevDmgCab)
		{
			this.dmgAmount = this.avgDmgCab - this.prevDmgCab;
			if (this.dmgAmount > 0.04f)
			{
				this.prevDmgCab = this.avgDmgCab;
				int num = Random.Range(0, 10);
				if (this.breakableParts[num].GetComponent<Renderer>().enabled)
				{
					foreach (object obj in this.breakableParts[num].transform)
					{
						Transform transform = (Transform)obj;
						this.totalLoosened = 0;
						if (transform.gameObject.name == "bolt" && transform.gameObject.GetComponent<BoltScript>().boltturns > 0)
						{
							int num2 = 1;
							if (this.dmgAmount > 0.01f)
							{
								num2 = 2;
							}
							if (this.dmgAmount > 0.02f)
							{
								num2 = 4;
							}
							transform.gameObject.GetComponent<BoltScript>().boltturns -= num2;
							transform.transform.Translate(-Vector3.forward * (0.002f * (float)num2));
							this.totalLoosened++;
						}
					}
					if (this.totalLoosened <= 0)
					{
						string name = this.breakableParts[num].name;
						this.addPart(name, false);
						this.breakableParts[num].GetComponent<Renderer>().enabled = false;
						this.breakableParts[num].GetComponent<BoxCollider>().enabled = false;
						foreach (object obj2 in this.breakableParts[num].transform)
						{
							Transform transform2 = (Transform)obj2;
							if (transform2.gameObject.GetComponent<Renderer>())
							{
								transform2.gameObject.GetComponent<Renderer>().enabled = false;
							}
						}
						GameObject gameObject = Object.Instantiate<GameObject>(this.breakableParts[num].GetComponent<durability>().template, this.breakableParts[num].transform.position, this.breakableParts[num].transform.rotation);
						gameObject.GetComponent<PickUp>().thisDurability = this.breakableParts[num].GetComponent<durability>().health;
						gameObject.GetComponent<PickUp>().price = 0f;
					}
				}
			}
		}
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x0007605C File Offset: 0x0007425C
	public void DrainBat()
	{
		if (this.battery_cnd > 0f)
		{
			this.battery_cnd -= 1f;
		}
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x00076080 File Offset: 0x00074280
	public void Refresh()
	{
		this.airFilter_cnd = this.airFilter_cnd_c.health;
		this.airFilterEFI_cnd = this.airFilterEFI_cnd_c.health;
		this.airFilterTurbo_cnd = this.airFilterTurbo_cnd_c.health;
		this.battery_cnd = this.battery_cnd_c.health;
		this.crank_cnd = this.crank_cnd_c.health;
		this.piston1_cnd = this.piston1_cnd_c.health;
		this.piston2_cnd = this.piston2_cnd_c.health;
		this.piston3_cnd = this.piston3_cnd_c.health;
		this.piston4_cnd = this.piston4_cnd_c.health;
		this.timingGear16_cnd = this.timingGear16_cnd_c.health;
		this.timingGear16r_cnd = this.timingGear16r_cnd_c.health;
		this.timingGear32_cnd = this.timingGear32_cnd_c.health;
		this.oilFilter_cnd = this.oilFilter_cnd_c.health;
		this.mainBearing1_cnd = this.mainBearing1_cnd_c.health;
		this.mainBearing2_cnd = this.mainBearing2_cnd_c.health;
		this.mainBearing3_cnd = this.mainBearing3_cnd_c.health;
		this.mainBearing4_cnd = this.mainBearing4_cnd_c.health;
		this.mainBearing5_cnd = this.mainBearing5_cnd_c.health;
		this.oilPan_cnd = this.oilPan_cnd_c.health;
		this.timingCover_cnd = this.timingCover_cnd_c.health;
		this.alternator_cnd = this.alternator_cnd_c.health;
		this.pistonbearing1_cnd = this.pistonbearing1_cnd_c.health;
		this.pistonbearing2_cnd = this.pistonbearing2_cnd_c.health;
		this.pistonbearing3_cnd = this.pistonbearing3_cnd_c.health;
		this.pistonbearing4_cnd = this.pistonbearing4_cnd_c.health;
		this.head_cnd = this.head_cnd_c.health;
		this.flyWheel_cnd = this.flyWheel_cnd_c.health;
		this.clutch_cnd = this.clutch_cnd_c.health;
		this.clutchDia_cnd = this.clutchDia_cnd_c.health;
		this.transmission_cnd = this.transmission_cnd_c.health;
		this.starterMotor_cnd = this.starterMotor_cnd_c.health;
		this.transferCase_cnd = this.transferCase_cnd_c.health;
		this.crankPulley_cnd = this.crankPulley_cnd_c.health;
		this.fanPulley_cnd = this.fanPulley_cnd_c.health;
		this.fanBelt_cnd = this.fanBelt_cnd_c.health;
		this.fanClutch_cnd = this.fanClutch_cnd_c.health;
		this.fan_cnd = this.fan_cnd_c.health;
		this.headgasket_cnd = this.headgasket_cnd_c.health;
		this.valveAssembly_cnd = this.valveAssembly_cnd_c.health;
		this.camshaft_cnd = this.camshaft_cnd_c.health;
		this.camBearing1_cnd = this.camBearing1_cnd_c.health;
		this.camBearing2_cnd = this.camBearing2_cnd_c.health;
		this.camBearing3_cnd = this.camBearing3_cnd_c.health;
		this.camGear_cnd = this.camGear_cnd_c.health;
		this.intakeMan_cnd = this.intakeMan_cnd_c.health;
		this.intakeManEFI_cnd = this.intakeManEFI_cnd_c.health;
		this.intakeEFIThrottle_cnd = this.intakeEFIThrottle_cnd_c.health;
		this.carb_cnd = this.carb_cnd_c.health;
		this.exhaustMan_cnd = this.exhaustMan_cnd_c.health;
		this.exhaustMan2_cnd = this.exhaustMan2_cnd_c.health;
		this.exhaustMan3_cnd = this.exhaustMan3_cnd_c.health;
		this.valveCover_cnd = this.valveCover_cnd_c.health;
		this.distributor_cnd = this.distributor_cnd_c.health;
		this.turbo_cnd = this.turbo_cnd_c.health;
		this.turboPipe_cnd = this.turboPipe_cnd_c.health;
		this.compound1_cnd = this.compound1_cnd_c.health;
		this.compound2_cnd = this.compound2_cnd_c.health;
		this.turboXL_cnd = this.turboXL_cnd_c.health;
		this.airFilterTurboXL_cnd = this.airFilterTurboXL_cnd_c.health;
		this.driveRingGearF4_cnd = this.driveRingGearF4_cnd_c.health;
		this.driveRingGearR4_cnd = this.driveRingGearR4_cnd_c.health;
		this.driveRingGearF5_cnd = this.driveRingGearF5_cnd_c.health;
		this.driveRingGearR5_cnd = this.driveRingGearR5_cnd_c.health;
		this.newFuelLevel = this.truckScript.fuel;
		this.torqueReduction = 0f;
		this.radiator_cnd = 100f;
		this.sparkPlug1_cnd = 100f;
		this.sparkPlug2_cnd = 100f;
		this.sparkPlug3_cnd = 100f;
		this.sparkPlug4_cnd = 100f;
		this.canRun = true;
		this.canCrank = true;
		this.canAcc = true;
		this.canEasyStart = true;
		this.canMove = true;
		this.can4wd = true;
		this.fanSpin = true;
		if (this.oilFilter_cnd < 1f)
		{
			this.oilLoss = 4f;
		}
		if (this.valveCover_cnd < 1f)
		{
			this.oilLoss = 1f;
		}
		if (this.oilPan_cnd < 1f)
		{
			this.oilLoss = 100f;
			this.newOilLevel = 0f;
		}
		if (this.camGear_cnd == 0f || this.timingGear16_cnd == 0f || this.timingGear16r_cnd == 0f || this.timingGear32_cnd == 0f || this.crank_cnd == 0f || this.head_cnd == 0f || this.valveAssembly_cnd == 0f || this.camshaft_cnd == 0f || this.distributor_cnd == 0f || this.newFuelLevel < 1f || this.plugwires_cnd == 0f || this.battery_cnd == 0f)
		{
			this.canRun = false;
		}
		if ((this.intakeMan_cnd == 0f || this.carb_cnd == 0f) && (this.intakeManEFI_cnd == 0f || this.intakeEFIThrottle_cnd == 0f))
		{
			this.canRun = false;
		}
		if (!this.truckScript.usingV8)
		{
			if (base.transform.parent == null)
			{
				this.canRun = false;
				this.canCrank = false;
			}
			else if (base.transform.parent.name != "dirt pickup truck")
			{
				this.canRun = false;
				this.canCrank = false;
			}
		}
		if (this.intakeMan_cnd == 0f && (this.intakeManEFI_cnd == 0f || this.intakeEFIThrottle_cnd == 0f))
		{
			this.canRun = false;
		}
		if (this.battery_cnd == 0f || this.starterMotor_cnd == 0f)
		{
			this.canCrank = false;
		}
		if (this.battery_cnd > 0f)
		{
			this.canAcc = true;
		}
		else
		{
			this.canAcc = false;
		}
		int num = 0;
		if (this.camBearing1_cnd > 1f)
		{
			num++;
		}
		if (this.camBearing2_cnd > 1f)
		{
			num++;
		}
		if (this.camBearing3_cnd > 1f)
		{
			num++;
		}
		if (num < 2)
		{
			this.canRun = false;
		}
		int num2 = 0;
		if (this.piston1_cnd > 1f)
		{
			num2++;
		}
		if (this.piston2_cnd > 1f)
		{
			num2++;
		}
		if (this.piston3_cnd > 1f)
		{
			num2++;
		}
		if (this.piston4_cnd > 1f)
		{
			num2++;
		}
		if (num2 < 3)
		{
			this.canRun = false;
		}
		int num3 = 0;
		if (this.mainBearing1_cnd > 0f)
		{
			num3++;
		}
		if (this.mainBearing2_cnd > 0f)
		{
			num3++;
		}
		if (this.mainBearing3_cnd > 0f)
		{
			num3++;
		}
		if (this.mainBearing4_cnd > 0f)
		{
			num3++;
		}
		if (this.mainBearing5_cnd > 0f)
		{
			num3++;
		}
		if (num3 < 2)
		{
			this.canRun = false;
		}
		if (this.clutch_cnd == 0f || this.flyWheel_cnd == 0f || this.clutchDia_cnd == 0f || this.transmission_cnd == 0f || this.transferCase_cnd == 0f || (this.driveRingGearF4_cnd == 0f && this.driveRingGearF5_cnd == 0f && this.driveRingGearR4_cnd == 0f && this.driveRingGearR5_cnd == 0f))
		{
			this.canMove = false;
		}
		else
		{
			this.canMove = true;
			this.truckScript.maxTorque = 600f - this.torqueReduction;
			this.truckScript.maxTorqueStatic = 600f - this.torqueReduction;
		}
		if (this.exhaustMan_cnd == 0f && this.exhaustMan2_cnd == 0f && this.exhaustMan3_cnd == 0f)
		{
			this.hasExhaustMan = false;
		}
		else
		{
			this.hasExhaustMan = true;
		}
		this.checkEngine = false;
		if (this.alternator_cnd < 10f || this.distributor_cnd < 10f || this.fanBelt_cnd < 1f || this.piston1_cnd < 1f || this.piston2_cnd < 1f || this.piston3_cnd < 1f || this.piston4_cnd < 1f)
		{
			this.checkEngine = true;
		}
		if (this.alternator_cnd < 10f && this.battery_cnd > 0f)
		{
			this.battery_cnd -= 0.1f;
		}
		if (this.alternator_cnd > 10f && this.battery_cnd < 100f)
		{
			this.battery_cnd += 0.5f;
		}
		if (this.airFilter_cnd < 10f && this.airFilter.GetComponent<Renderer>().enabled)
		{
			this.checkEngine = true;
		}
		else if (this.airFilterEFI_cnd < 10f && this.airFilterEFI.GetComponent<Renderer>().enabled)
		{
			this.checkEngine = true;
		}
		else if (this.airFilterTurbo_cnd < 10f && this.airFilterTurbo.GetComponent<Renderer>().enabled)
		{
			this.checkEngine = true;
		}
		if (this.carb_cnd < 10f && this.carb.GetComponent<Renderer>().enabled)
		{
			this.checkEngine = true;
		}
		else if (this.intakeEFIThrottle_cnd < 10f && this.intakeEFIThrottle.GetComponent<Renderer>().enabled)
		{
			this.checkEngine = true;
		}
		this.fancool = 50f;
		this.tempIncrease = 0f;
		this.fanClutchSeized = false;
		this.beltWhine = false;
		if (this.airFilter_cnd < 20f && this.airFilter_cnd > 0f)
		{
			this.torqueReduction += 26f - this.airFilter_cnd;
		}
		if (this.piston1_cnd < 30f)
		{
			this.torqueReduction += (30f - this.piston1_cnd) * 5f;
		}
		if (this.piston2_cnd < 30f)
		{
			this.torqueReduction += (30f - this.piston2_cnd) * 5f;
		}
		if (this.piston3_cnd < 30f)
		{
			this.torqueReduction += (30f - this.piston3_cnd) * 5f;
		}
		if (this.piston4_cnd < 30f)
		{
			this.torqueReduction += (30f - this.piston4_cnd) * 5f;
		}
		if (this.headgasket_cnd < 30f)
		{
			this.torqueReduction += (30f - this.headgasket_cnd) * 2f;
		}
		if (this.head_cnd < 30f)
		{
			this.torqueReduction += 30f - this.head_cnd;
		}
		if (this.fanClutch_cnd < 10f)
		{
			this.tempIncrease = 15f;
			this.fanClutchSeized = true;
		}
		if (this.fan_cnd == 0f || this.fanBelt_cnd == 0f || this.fanPulley_cnd == 0f || this.crankPulley_cnd == 0f)
		{
			this.fancool = 0f;
			this.fanSpin = false;
			this.truckScript.fanSpin = false;
		}
		if (this.fanSpin)
		{
			this.truckScript.fanSpin = true;
			this.fancool = 50f;
		}
		if (this.fanBelt_cnd > 0f && this.fanBelt_cnd < 20f)
		{
			this.beltWhine = true;
		}
		if (this.intakeManEFI_cnd > 0f && this.intakeEFIThrottle_cnd > 5f)
		{
			this.torqueIncrease = 25f;
		}
		if (this.turbo_cnd > 5f && this.turboPipe_cnd > 0f && this.exhaustMan3_cnd > 0f)
		{
			this.hasTurbo = true;
			this.torqueIncrease = 200f - (100f - this.turbo_cnd);
		}
		else
		{
			this.hasTurbo = false;
		}
		if (this.turbo_cnd > 5f && this.nonOemTurbo && this.truckScript.userControlled)
		{
			this.turbo_cnd_c.health -= 5f;
			this.turbo_cnd = this.turbo_cnd_c.health;
			if (this.turbo_cnd <= 5f)
			{
				this.BlowTurbo();
			}
		}
		if (this.turbo_cnd > 5f && this.turboPipe_cnd > 0f && this.exhaustMan3_cnd > 0f && this.compound1_cnd > 0f && this.compound2_cnd > 0f && this.turboXL_cnd > 0f)
		{
			this.hasTurbo2 = true;
			this.torqueIncrease = 400f - (100f - this.turbo_cnd);
		}
		else
		{
			this.hasTurbo2 = false;
		}
		if (this.exhaustMan2_cnd > 0f)
		{
			this.torqueIncrease = 30f;
		}
		if (this.debugRun)
		{
			this.canRun = true;
			this.canAcc = true;
			this.canCrank = true;
		}
		this.truckScript.checkEngine = this.checkEngine;
		this.truckScript.canRun = this.canRun;
		this.truckScript.canCrank = this.canCrank;
		this.truckScript.canAcc = this.canAcc;
		this.truckScript.canEasyStart = this.canEasyStart;
		this.newCoolantLevel = this.truckScript.coolantLevel;
		float additive = this.truckScript.additive;
		float num4 = 0f;
		if (additive > 3f)
		{
			num4 = 80f;
		}
		this.maxTemp = 440f - this.newOilLevel - this.newCoolantLevel - this.fancool + this.tempIncrease + num4;
		this.truckScript.maxTemp = this.maxTemp;
		if (this.canMove)
		{
			this.truckScript.maxTorque = 550f - this.torqueReduction + this.torqueIncrease;
			this.truckScript.maxTorqueStatic = 550f - this.torqueReduction + this.torqueIncrease;
			return;
		}
		this.truckScript.maxTorque = 0f;
		this.truckScript.maxTorqueStatic = 0f;
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x00076F48 File Offset: 0x00075148
	public void DegradeEngine()
	{
		this.CrashDamageCab();
		if (this.headgasket_cnd < 40f)
		{
			this.exhaustTrails.GetComponent<ParticleSystem>().startColor = new Color(1f, 1f, 1f, 0.4f);
		}
		else if (this.newOilLevel < 42f)
		{
			this.exhaustTrails.GetComponent<ParticleSystem>().startColor = new Color(0.34f, 0.43f, 0.55f, 0.4f);
		}
		else
		{
			this.exhaustTrails.GetComponent<ParticleSystem>().startColor = new Color(0.56f, 0.56f, 0.56f, 0.23f);
		}
		this.randPart = Random.Range(0, 15);
		switch (this.randPart)
		{
		case 0:
			if (this.airFilter_cnd_c.health > 1f)
			{
				this.airFilter_cnd_c.health -= 2f;
			}
			break;
		case 1:
			this.alternator_cnd_c.health -= 1f;
			break;
		case 2:
			this.clutch_cnd_c.health -= 1f;
			break;
		case 3:
			this.fanBelt_cnd_c.health -= 2f;
			break;
		case 4:
			this.headgasket_cnd_c.health -= 1f;
			break;
		case 5:
			this.oilFilter_cnd_c.health -= 1f;
			break;
		case 6:
			this.piston1_cnd_c.health -= 1f;
			break;
		case 7:
			this.piston2_cnd_c.health -= 1f;
			break;
		case 8:
			this.piston3_cnd_c.health -= 1f;
			break;
		case 9:
			this.piston4_cnd_c.health -= 1f;
			break;
		case 10:
			this.transmission_cnd_c.health -= 1f;
			break;
		case 11:
			this.transferCase_cnd_c.health -= 1f;
			break;
		case 12:
			this.turbo_cnd_c.health -= 1f;
			break;
		}
		if (this.newOilLevel < 42f)
		{
			this.randPart = Random.Range(0, 12);
			switch (this.randPart)
			{
			case 0:
				this.camshaft_cnd_c.health -= 1f;
				return;
			case 1:
				this.crank_cnd_c.health -= 1f;
				return;
			case 2:
				this.oilFilter_cnd_c.health -= 1f;
				return;
			case 3:
				this.piston1_cnd_c.health -= 1f;
				return;
			case 4:
				this.piston2_cnd_c.health -= 1f;
				return;
			case 5:
				this.piston3_cnd_c.health -= 1f;
				return;
			case 6:
				this.piston4_cnd_c.health -= 1f;
				return;
			case 7:
				this.valveAssembly_cnd_c.health -= 1f;
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x000772C0 File Offset: 0x000754C0
	public void addPart(string newPart, bool connect)
	{
		if (connect)
		{
			this.thisHealth = GameObject.Find(newPart).GetComponent<durability>().health;
		}
		else
		{
			this.thisHealth = 0f;
		}
		if (newPart == "intakeEFI_e" && connect)
		{
			this.intakeMan.SetActive(false);
		}
		if (newPart == "intakeEFI_e" && !connect)
		{
			this.intakeMan.SetActive(true);
			this.intakeManEFI.SetActive(true);
		}
		if (newPart == "exhaustMan_e" && connect)
		{
			this.exhaustMan2.SetActive(false);
			this.exhaustMan3.SetActive(false);
		}
		if (newPart == "exhaustMan_e" && !connect)
		{
			this.exhaustMan2.SetActive(true);
			this.exhaustMan3.SetActive(true);
		}
		if (newPart == "exhaustMan2_e" && connect)
		{
			this.exhaustMan.SetActive(false);
			this.exhaustMan3.SetActive(false);
		}
		if (newPart == "exhaustMan2_e" && !connect)
		{
			this.exhaustMan.SetActive(true);
			this.exhaustMan3.SetActive(true);
		}
		if (newPart == "exhaustMan3_e" && connect)
		{
			this.exhaustMan2.SetActive(false);
			this.exhaustMan.SetActive(false);
		}
		if (newPart == "exhaustMan3_e" && !connect)
		{
			this.exhaustMan2.SetActive(true);
			this.exhaustMan.SetActive(true);
		}
		if (newPart == "conefilter_e" && !connect)
		{
			this.compound2.SetActive(true);
		}
		if (newPart == "compound2_e" && connect)
		{
			this.airFilterTurbo.SetActive(false);
		}
		if (newPart == "compound2_e" && !connect)
		{
			this.airFilterTurbo.SetActive(true);
		}
		uint num = <PrivateImplementationDetails>.ComputeStringHash(newPart);
		if (num <= 2363679877U)
		{
			if (num <= 1208536501U)
			{
				if (num <= 367642094U)
				{
					if (num <= 174210553U)
					{
						if (num <= 52291886U)
						{
							if (num != 9854079U)
							{
								if (num != 52291886U)
								{
									return;
								}
								if (!(newPart == "oilPan_e"))
								{
									return;
								}
								this.oilPan_cnd = this.thisHealth;
								return;
							}
							else
							{
								if (!(newPart == "intakeMan_e"))
								{
									return;
								}
								this.intakeMan_cnd = this.thisHealth;
								return;
							}
						}
						else if (num != 81343284U)
						{
							if (num != 174210553U)
							{
								return;
							}
							if (!(newPart == "plugwires_e"))
							{
								return;
							}
							this.plugwires_cnd = this.thisHealth;
							return;
						}
						else
						{
							if (!(newPart == "valveCover_e"))
							{
								return;
							}
							this.valveCover_cnd = this.thisHealth;
							return;
						}
					}
					else if (num <= 303868820U)
					{
						if (num != 281939837U)
						{
							if (num != 303868820U)
							{
								return;
							}
							if (!(newPart == "frontDriveShaft_e"))
							{
								return;
							}
							this.frontDriveShaft_cnd = this.thisHealth;
							return;
						}
						else
						{
							if (!(newPart == "mainBearing1_e"))
							{
								return;
							}
							this.mainBearing1_cnd = this.thisHealth;
							return;
						}
					}
					else if (num != 353885855U)
					{
						if (num != 367642094U)
						{
							return;
						}
						if (!(newPart == "sparkPlug4_e"))
						{
							return;
						}
						this.sparkPlug4_cnd = this.thisHealth;
						return;
					}
					else
					{
						if (!(newPart == "timingGear16r_e"))
						{
							return;
						}
						this.timingGear16r_cnd = this.thisHealth;
						return;
					}
				}
				else if (num <= 810995921U)
				{
					if (num <= 641770908U)
					{
						if (num != 544470791U)
						{
							if (num != 641770908U)
							{
								return;
							}
							if (!(newPart == "camBearing2_e"))
							{
								return;
							}
							this.camBearing2_cnd = this.thisHealth;
							return;
						}
						else
						{
							if (!(newPart == "compound1_e"))
							{
								return;
							}
							this.compound1_cnd = this.thisHealth;
							return;
						}
					}
					else if (num != 674536975U)
					{
						if (num != 810995921U)
						{
							return;
						}
						if (!(newPart == "oilFilter_e"))
						{
							return;
						}
						this.oilFilter_cnd = this.thisHealth;
						return;
					}
					else
					{
						if (!(newPart == "exhaustMan_e"))
						{
							return;
						}
						this.exhaustMan_cnd = this.thisHealth;
						return;
					}
				}
				else if (num <= 920453885U)
				{
					if (num != 823186295U)
					{
						if (num != 920453885U)
						{
							return;
						}
						if (!(newPart == "airFilter_e"))
						{
							return;
						}
						this.airFilter_cnd = this.thisHealth;
						return;
					}
					else
					{
						if (!(newPart == "fanPulley_e"))
						{
							return;
						}
						this.fanPulley_cnd = this.thisHealth;
						return;
					}
				}
				else if (num != 971399780U)
				{
					if (num != 1056678460U)
					{
						if (num != 1208536501U)
						{
							return;
						}
						if (!(newPart == "pistonbearing1_e"))
						{
							return;
						}
						this.pistonbearing1_cnd = this.thisHealth;
						return;
					}
					else
					{
						if (!(newPart == "exhaustMan3_e"))
						{
							return;
						}
						this.exhaustMan3_cnd = this.thisHealth;
						return;
					}
				}
				else
				{
					if (!(newPart == "piston4_e"))
					{
						return;
					}
					this.piston4_cnd = this.thisHealth;
					return;
				}
			}
			else if (num <= 1876519369U)
			{
				if (num <= 1415914193U)
				{
					if (num <= 1268218419U)
					{
						if (num != 1267621612U)
						{
							if (num != 1268218419U)
							{
								return;
							}
							if (!(newPart == "pistonbearing3_e"))
							{
								return;
							}
							this.pistonbearing3_cnd = this.thisHealth;
							return;
						}
						else
						{
							if (!(newPart == "crank_e"))
							{
								return;
							}
							this.crank_cnd = this.thisHealth;
							return;
						}
					}
					else if (num != 1373709499U)
					{
						if (num != 1415914193U)
						{
							return;
						}
						if (!(newPart == "airFilterEFI_e"))
						{
							return;
						}
						this.airFilterEFI_cnd = this.thisHealth;
						return;
					}
					else
					{
						if (!(newPart == "mainBearing3_e"))
						{
							return;
						}
						this.mainBearing3_cnd = this.thisHealth;
						return;
					}
				}
				else if (num <= 1671286021U)
				{
					if (num != 1449680990U)
					{
						if (num != 1671286021U)
						{
							return;
						}
						if (!(newPart == "flyWheel_e"))
						{
							return;
						}
						this.flyWheel_cnd = this.thisHealth;
						return;
					}
					else
					{
						if (!(newPart == "mainBearing4_e"))
						{
							return;
						}
						this.mainBearing4_cnd = this.thisHealth;
						return;
					}
				}
				else if (num != 1726167345U)
				{
					if (num != 1876519369U)
					{
						return;
					}
					if (!(newPart == "camBearing1_e"))
					{
						return;
					}
					this.camBearing1_cnd = this.thisHealth;
					return;
				}
				else
				{
					if (!(newPart == "transmission_e"))
					{
						return;
					}
					this.transmission_cnd = this.thisHealth;
					return;
				}
			}
			else if (num <= 2044223320U)
			{
				if (num <= 1910697127U)
				{
					if (num != 1906011646U)
					{
						if (num != 1910697127U)
						{
							return;
						}
						if (!(newPart == "fanBelt_e"))
						{
							return;
						}
						this.fanBelt_cnd = this.thisHealth;
						return;
					}
					else
					{
						if (!(newPart == "compound2_e"))
						{
							return;
						}
						this.compound2_cnd = this.thisHealth;
						return;
					}
				}
				else if (num != 1939758576U)
				{
					if (num != 2044223320U)
					{
						return;
					}
					if (!(newPart == "camshaft_e"))
					{
						return;
					}
					this.camshaft_cnd = this.thisHealth;
					return;
				}
				else
				{
					if (!(newPart == "sparkPlug2_e"))
					{
						return;
					}
					this.sparkPlug2_cnd = this.thisHealth;
					return;
				}
			}
			else if (num <= 2127005802U)
			{
				if (num != 2091048574U)
				{
					if (num != 2127005802U)
					{
						return;
					}
					if (!(newPart == "distributor_e"))
					{
						return;
					}
					this.distributor_cnd = this.thisHealth;
					return;
				}
				else
				{
					if (!(newPart == "clutchDia_e"))
					{
						return;
					}
					this.clutchDia_cnd = this.thisHealth;
					return;
				}
			}
			else if (num != 2204540023U)
			{
				if (num != 2238967098U)
				{
					if (num != 2363679877U)
					{
						return;
					}
					if (!(newPart == "timingGear32_e"))
					{
						return;
					}
					this.timingGear32_cnd = this.thisHealth;
					return;
				}
				else
				{
					if (!(newPart == "piston2_e"))
					{
						return;
					}
					this.piston2_cnd = this.thisHealth;
					return;
				}
			}
			else
			{
				if (!(newPart == "camBearing3_e"))
				{
					return;
				}
				this.camBearing3_cnd = this.thisHealth;
				return;
			}
		}
		else if (num <= 3492185099U)
		{
			if (num <= 2788332359U)
			{
				if (num <= 2707362920U)
				{
					if (num <= 2536873381U)
					{
						if (num != 2417854358U)
						{
							if (num != 2536873381U)
							{
								return;
							}
							if (!(newPart == "valveAssembly_e"))
							{
								return;
							}
							this.valveAssembly_cnd = this.thisHealth;
							return;
						}
						else
						{
							if (!(newPart == "pistonbearing4_e"))
							{
								return;
							}
							this.pistonbearing4_cnd = this.thisHealth;
							return;
						}
					}
					else if (num != 2640339095U)
					{
						if (num != 2707362920U)
						{
							return;
						}
						if (!(newPart == "clutch_e"))
						{
							return;
						}
						this.clutch_cnd = this.thisHealth;
						return;
					}
					else
					{
						if (!(newPart == "exhaustMan2_e"))
						{
							return;
						}
						this.exhaustMan2_cnd = this.thisHealth;
						return;
					}
				}
				else if (num <= 2750239128U)
				{
					if (num != 2738013703U)
					{
						if (num != 2750239128U)
						{
							return;
						}
						if (!(newPart == "rearDiff_e"))
						{
							return;
						}
						this.rearDiff_cnd = this.thisHealth;
						return;
					}
					else
					{
						if (!(newPart == "alternator_e"))
						{
							return;
						}
						this.alternator_cnd = this.thisHealth;
						return;
					}
				}
				else if (num != 2783869835U)
				{
					if (num != 2788332359U)
					{
						return;
					}
					if (!(newPart == "timingGear16_e"))
					{
						return;
					}
					this.timingGear16_cnd = this.thisHealth;
					return;
				}
				else
				{
					if (!(newPart == "radiator_e"))
					{
						return;
					}
					this.radiator_cnd = this.thisHealth;
					return;
				}
			}
			else if (num <= 3002107513U)
			{
				if (num <= 2840574821U)
				{
					if (num != 2814709327U)
					{
						if (num != 2840574821U)
						{
							return;
						}
						if (!(newPart == "crankPulley_e"))
						{
							return;
						}
						this.crankPulley_cnd = this.thisHealth;
						return;
					}
					else
					{
						if (!(newPart == "camGear_e"))
						{
							return;
						}
						this.camGear_cnd = this.thisHealth;
						return;
					}
				}
				else if (num != 2888045059U)
				{
					if (num != 3002107513U)
					{
						return;
					}
					if (!(newPart == "mainBearing5_e"))
					{
						return;
					}
					this.mainBearing5_cnd = this.thisHealth;
					return;
				}
				else
				{
					if (!(newPart == "intakeManEFI_e"))
					{
						return;
					}
					this.intakeManEFI_cnd = this.thisHealth;
					return;
				}
			}
			else if (num <= 3110926065U)
			{
				if (num != 3032140064U)
				{
					if (num != 3110926065U)
					{
						return;
					}
					if (!(newPart == "starterMotor_e"))
					{
						return;
					}
					this.starterMotor_cnd = this.thisHealth;
					return;
				}
				else
				{
					if (!(newPart == "mainBearing2_e"))
					{
						return;
					}
					this.mainBearing2_cnd = this.thisHealth;
					return;
				}
			}
			else if (num != 3324226599U)
			{
				if (num != 3484525645U)
				{
					if (num != 3492185099U)
					{
						return;
					}
					if (!(newPart == "sparkPlug3_e"))
					{
						return;
					}
					this.sparkPlug3_cnd = this.thisHealth;
					return;
				}
				else
				{
					if (!(newPart == "sparkPlug1_e"))
					{
						return;
					}
					this.sparkPlug1_cnd = this.thisHealth;
					return;
				}
			}
			else
			{
				if (!(newPart == "turboPipe_e"))
				{
					return;
				}
				this.turboPipe_cnd = this.thisHealth;
				return;
			}
		}
		else if (num <= 4010656024U)
		{
			if (num <= 3830287187U)
			{
				if (num <= 3688506277U)
				{
					if (num != 3584080847U)
					{
						if (num != 3688506277U)
						{
							return;
						}
						if (!(newPart == "intakeEFIThrottle_e"))
						{
							return;
						}
						this.intakeMan_cnd = this.thisHealth;
						return;
					}
					else
					{
						if (!(newPart == "turboXL_e"))
						{
							return;
						}
						this.turboXL_cnd = this.thisHealth;
						return;
					}
				}
				else if (num != 3791496789U)
				{
					if (num != 3830287187U)
					{
						return;
					}
					if (!(newPart == "piston1_e"))
					{
						return;
					}
					this.piston1_cnd = this.thisHealth;
					return;
				}
				else
				{
					if (!(newPart == "piston3_e"))
					{
						return;
					}
					this.piston3_cnd = this.thisHealth;
					return;
				}
			}
			else if (num <= 3936066132U)
			{
				if (num != 3899163339U)
				{
					if (num != 3936066132U)
					{
						return;
					}
					if (!(newPart == "headgasket_e"))
					{
						return;
					}
					this.headgasket_cnd = this.thisHealth;
					return;
				}
				else
				{
					if (!(newPart == "rearDriveShaft_e"))
					{
						return;
					}
					this.rearDriveShaft_cnd = this.thisHealth;
					return;
				}
			}
			else if (num != 3944105470U)
			{
				if (num != 4010656024U)
				{
					return;
				}
				if (!(newPart == "pistonbearing2_e"))
				{
					return;
				}
				this.pistonbearing2_cnd = this.thisHealth;
				return;
			}
			else
			{
				if (!(newPart == "transferCase_e"))
				{
					return;
				}
				this.transferCase_cnd = this.thisHealth;
				return;
			}
		}
		else if (num <= 4165522486U)
		{
			if (num <= 4073520775U)
			{
				if (num != 4013365720U)
				{
					if (num != 4073520775U)
					{
						return;
					}
					if (!(newPart == "fanClutch_e"))
					{
						return;
					}
					this.fanClutch_cnd = this.thisHealth;
					return;
				}
				else
				{
					if (!(newPart == "conefilterD_e"))
					{
						return;
					}
					this.airFilterTurboXL_cnd = this.thisHealth;
					return;
				}
			}
			else if (num != 4151373155U)
			{
				if (num != 4165522486U)
				{
					return;
				}
				if (!(newPart == "fan_e"))
				{
					return;
				}
				this.fan_cnd = this.thisHealth;
				return;
			}
			else
			{
				if (!(newPart == "head_e"))
				{
					return;
				}
				this.head_cnd = this.thisHealth;
				return;
			}
		}
		else if (num <= 4219090711U)
		{
			if (num != 4212894421U)
			{
				if (num != 4219090711U)
				{
					return;
				}
				if (!(newPart == "airFilterTurbo_e"))
				{
					return;
				}
				this.airFilterTurbo_cnd = this.thisHealth;
				return;
			}
			else
			{
				if (!(newPart == "carb_e"))
				{
					return;
				}
				this.carb_cnd = this.thisHealth;
				return;
			}
		}
		else if (num != 4248663954U)
		{
			if (num != 4266323179U)
			{
				if (num != 4294780482U)
				{
					return;
				}
				if (!(newPart == "timingCover_e"))
				{
					return;
				}
				this.timingCover_cnd = this.thisHealth;
				return;
			}
			else
			{
				if (!(newPart == "turbo_e"))
				{
					return;
				}
				this.turbo_cnd = this.thisHealth;
				return;
			}
		}
		else
		{
			if (!(newPart == "battery_e"))
			{
				return;
			}
			this.battery_cnd = this.thisHealth;
			return;
		}
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x00078080 File Offset: 0x00076280
	private void BlowTurbo()
	{
		this.brokenTurboVis.SetActive(true);
		this.turbo.GetComponent<Renderer>().enabled = false;
		this.brokenTurboVis.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().Play();
		this.brokenTurboVis.GetComponent<AudioSource>().Play();
		this.modWoman.GetComponent<ModWomanJobs>().job4Condition = true;
		base.StartCoroutine(this.StopSmoke());
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x000780F8 File Offset: 0x000762F8
	private IEnumerator StopSmoke()
	{
		yield return new WaitForSeconds(15f);
		this.brokenTurboVis.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().Stop();
		yield break;
	}

	// Token: 0x0400157F RID: 5503
	public GameObject truck;

	// Token: 0x04001580 RID: 5504
	public GameObject airFilter;

	// Token: 0x04001581 RID: 5505
	public GameObject airFilterEFI;

	// Token: 0x04001582 RID: 5506
	public GameObject airFilterTurbo;

	// Token: 0x04001583 RID: 5507
	public GameObject alternator;

	// Token: 0x04001584 RID: 5508
	public GameObject battery;

	// Token: 0x04001585 RID: 5509
	public GameObject block;

	// Token: 0x04001586 RID: 5510
	public GameObject camshaft;

	// Token: 0x04001587 RID: 5511
	public GameObject camBearing1;

	// Token: 0x04001588 RID: 5512
	public GameObject camBearing2;

	// Token: 0x04001589 RID: 5513
	public GameObject camBearing3;

	// Token: 0x0400158A RID: 5514
	public GameObject camGear;

	// Token: 0x0400158B RID: 5515
	public GameObject carb;

	// Token: 0x0400158C RID: 5516
	public GameObject clutch;

	// Token: 0x0400158D RID: 5517
	public GameObject clutchDia;

	// Token: 0x0400158E RID: 5518
	public GameObject crank;

	// Token: 0x0400158F RID: 5519
	public GameObject crankPulley;

	// Token: 0x04001590 RID: 5520
	public GameObject distributor;

	// Token: 0x04001591 RID: 5521
	public GameObject driveRingGearF4;

	// Token: 0x04001592 RID: 5522
	public GameObject driveRingGearR4;

	// Token: 0x04001593 RID: 5523
	public GameObject driveRingGearF5;

	// Token: 0x04001594 RID: 5524
	public GameObject driveRingGearR5;

	// Token: 0x04001595 RID: 5525
	public GameObject exhaustMan;

	// Token: 0x04001596 RID: 5526
	public GameObject exhaustMan2;

	// Token: 0x04001597 RID: 5527
	public GameObject exhaustMan3;

	// Token: 0x04001598 RID: 5528
	public GameObject fanPulley;

	// Token: 0x04001599 RID: 5529
	public GameObject fanBelt;

	// Token: 0x0400159A RID: 5530
	public GameObject fanClutch;

	// Token: 0x0400159B RID: 5531
	public GameObject fan;

	// Token: 0x0400159C RID: 5532
	public GameObject flyWheel;

	// Token: 0x0400159D RID: 5533
	public GameObject frontDriveShaft;

	// Token: 0x0400159E RID: 5534
	public GameObject headgasket;

	// Token: 0x0400159F RID: 5535
	public GameObject head;

	// Token: 0x040015A0 RID: 5536
	public GameObject intakeMan;

	// Token: 0x040015A1 RID: 5537
	public GameObject intakeManEFI;

	// Token: 0x040015A2 RID: 5538
	public GameObject intakeEFIThrottle;

	// Token: 0x040015A3 RID: 5539
	public GameObject mainBearing1;

	// Token: 0x040015A4 RID: 5540
	public GameObject mainBearing2;

	// Token: 0x040015A5 RID: 5541
	public GameObject mainBearing3;

	// Token: 0x040015A6 RID: 5542
	public GameObject mainBearing4;

	// Token: 0x040015A7 RID: 5543
	public GameObject mainBearing5;

	// Token: 0x040015A8 RID: 5544
	public GameObject oilFilter;

	// Token: 0x040015A9 RID: 5545
	public GameObject oilPan;

	// Token: 0x040015AA RID: 5546
	public GameObject piston1;

	// Token: 0x040015AB RID: 5547
	public GameObject piston2;

	// Token: 0x040015AC RID: 5548
	public GameObject piston3;

	// Token: 0x040015AD RID: 5549
	public GameObject piston4;

	// Token: 0x040015AE RID: 5550
	public GameObject pistonbearing1;

	// Token: 0x040015AF RID: 5551
	public GameObject pistonbearing2;

	// Token: 0x040015B0 RID: 5552
	public GameObject pistonbearing3;

	// Token: 0x040015B1 RID: 5553
	public GameObject pistonbearing4;

	// Token: 0x040015B2 RID: 5554
	public GameObject plugwires;

	// Token: 0x040015B3 RID: 5555
	public GameObject radiator;

	// Token: 0x040015B4 RID: 5556
	public GameObject rearDiff;

	// Token: 0x040015B5 RID: 5557
	public GameObject rearDriveShaft;

	// Token: 0x040015B6 RID: 5558
	public GameObject sparkPlug1;

	// Token: 0x040015B7 RID: 5559
	public GameObject sparkPlug2;

	// Token: 0x040015B8 RID: 5560
	public GameObject sparkPlug3;

	// Token: 0x040015B9 RID: 5561
	public GameObject sparkPlug4;

	// Token: 0x040015BA RID: 5562
	public GameObject starterMotor;

	// Token: 0x040015BB RID: 5563
	public GameObject timingCover;

	// Token: 0x040015BC RID: 5564
	public GameObject timingGear16;

	// Token: 0x040015BD RID: 5565
	public GameObject timingGear16r;

	// Token: 0x040015BE RID: 5566
	public GameObject timingGear32;

	// Token: 0x040015BF RID: 5567
	public GameObject transmission;

	// Token: 0x040015C0 RID: 5568
	public GameObject transferCase;

	// Token: 0x040015C1 RID: 5569
	public GameObject turbo;

	// Token: 0x040015C2 RID: 5570
	public GameObject turboPipe;

	// Token: 0x040015C3 RID: 5571
	public GameObject valveAssembly;

	// Token: 0x040015C4 RID: 5572
	public GameObject valveCover;

	// Token: 0x040015C5 RID: 5573
	public GameObject compound1;

	// Token: 0x040015C6 RID: 5574
	public GameObject compound2;

	// Token: 0x040015C7 RID: 5575
	public GameObject turboXL;

	// Token: 0x040015C8 RID: 5576
	public GameObject airFilterTurboXL;

	// Token: 0x040015C9 RID: 5577
	public float airFilter_cnd;

	// Token: 0x040015CA RID: 5578
	public float airFilterEFI_cnd;

	// Token: 0x040015CB RID: 5579
	public float airFilterTurbo_cnd;

	// Token: 0x040015CC RID: 5580
	public float alternator_cnd;

	// Token: 0x040015CD RID: 5581
	public float battery_cnd;

	// Token: 0x040015CE RID: 5582
	public float camshaft_cnd;

	// Token: 0x040015CF RID: 5583
	public float camBearing1_cnd;

	// Token: 0x040015D0 RID: 5584
	public float camBearing2_cnd;

	// Token: 0x040015D1 RID: 5585
	public float camBearing3_cnd;

	// Token: 0x040015D2 RID: 5586
	public float camGear_cnd;

	// Token: 0x040015D3 RID: 5587
	public float carb_cnd;

	// Token: 0x040015D4 RID: 5588
	public float clutch_cnd;

	// Token: 0x040015D5 RID: 5589
	public float clutchDia_cnd;

	// Token: 0x040015D6 RID: 5590
	public float crank_cnd;

	// Token: 0x040015D7 RID: 5591
	public float crankPulley_cnd;

	// Token: 0x040015D8 RID: 5592
	public float distributor_cnd;

	// Token: 0x040015D9 RID: 5593
	public float driveRingGearF4_cnd;

	// Token: 0x040015DA RID: 5594
	public float driveRingGearR4_cnd;

	// Token: 0x040015DB RID: 5595
	public float driveRingGearF5_cnd;

	// Token: 0x040015DC RID: 5596
	public float driveRingGearR5_cnd;

	// Token: 0x040015DD RID: 5597
	public float exhaustMan_cnd;

	// Token: 0x040015DE RID: 5598
	public float exhaustMan2_cnd;

	// Token: 0x040015DF RID: 5599
	public float exhaustMan3_cnd;

	// Token: 0x040015E0 RID: 5600
	public float fanPulley_cnd;

	// Token: 0x040015E1 RID: 5601
	public float fanBelt_cnd;

	// Token: 0x040015E2 RID: 5602
	public float fanClutch_cnd;

	// Token: 0x040015E3 RID: 5603
	public float fan_cnd;

	// Token: 0x040015E4 RID: 5604
	public float flyWheel_cnd;

	// Token: 0x040015E5 RID: 5605
	public float frontDriveShaft_cnd;

	// Token: 0x040015E6 RID: 5606
	public float headgasket_cnd;

	// Token: 0x040015E7 RID: 5607
	public float head_cnd;

	// Token: 0x040015E8 RID: 5608
	public float intakeMan_cnd;

	// Token: 0x040015E9 RID: 5609
	public float intakeManEFI_cnd;

	// Token: 0x040015EA RID: 5610
	public float intakeEFIThrottle_cnd;

	// Token: 0x040015EB RID: 5611
	public float mainBearing1_cnd;

	// Token: 0x040015EC RID: 5612
	public float mainBearing2_cnd;

	// Token: 0x040015ED RID: 5613
	public float mainBearing3_cnd;

	// Token: 0x040015EE RID: 5614
	public float mainBearing4_cnd;

	// Token: 0x040015EF RID: 5615
	public float mainBearing5_cnd;

	// Token: 0x040015F0 RID: 5616
	public float oilFilter_cnd;

	// Token: 0x040015F1 RID: 5617
	public float oilPan_cnd;

	// Token: 0x040015F2 RID: 5618
	public float piston1_cnd;

	// Token: 0x040015F3 RID: 5619
	public float piston2_cnd;

	// Token: 0x040015F4 RID: 5620
	public float piston3_cnd;

	// Token: 0x040015F5 RID: 5621
	public float piston4_cnd;

	// Token: 0x040015F6 RID: 5622
	public float pistonbearing1_cnd;

	// Token: 0x040015F7 RID: 5623
	public float pistonbearing2_cnd;

	// Token: 0x040015F8 RID: 5624
	public float pistonbearing3_cnd;

	// Token: 0x040015F9 RID: 5625
	public float pistonbearing4_cnd;

	// Token: 0x040015FA RID: 5626
	public float plugwires_cnd;

	// Token: 0x040015FB RID: 5627
	public float radiator_cnd;

	// Token: 0x040015FC RID: 5628
	public float rearDiff_cnd;

	// Token: 0x040015FD RID: 5629
	public float rearDriveShaft_cnd;

	// Token: 0x040015FE RID: 5630
	public float starterMotor_cnd;

	// Token: 0x040015FF RID: 5631
	public float sparkPlug1_cnd;

	// Token: 0x04001600 RID: 5632
	public float sparkPlug2_cnd;

	// Token: 0x04001601 RID: 5633
	public float sparkPlug3_cnd;

	// Token: 0x04001602 RID: 5634
	public float sparkPlug4_cnd;

	// Token: 0x04001603 RID: 5635
	public float timingCover_cnd;

	// Token: 0x04001604 RID: 5636
	public float timingGear16_cnd;

	// Token: 0x04001605 RID: 5637
	public float timingGear16r_cnd;

	// Token: 0x04001606 RID: 5638
	public float timingGear32_cnd;

	// Token: 0x04001607 RID: 5639
	public float transmission_cnd;

	// Token: 0x04001608 RID: 5640
	public float transferCase_cnd;

	// Token: 0x04001609 RID: 5641
	public float turbo_cnd;

	// Token: 0x0400160A RID: 5642
	public float turboPipe_cnd;

	// Token: 0x0400160B RID: 5643
	public float valveAssembly_cnd;

	// Token: 0x0400160C RID: 5644
	public float valveCover_cnd;

	// Token: 0x0400160D RID: 5645
	public float compound1_cnd;

	// Token: 0x0400160E RID: 5646
	public float compound2_cnd;

	// Token: 0x0400160F RID: 5647
	public float turboXL_cnd;

	// Token: 0x04001610 RID: 5648
	public float airFilterTurboXL_cnd;

	// Token: 0x04001611 RID: 5649
	public durability airFilter_cnd_c;

	// Token: 0x04001612 RID: 5650
	public durability airFilterEFI_cnd_c;

	// Token: 0x04001613 RID: 5651
	public durability airFilterTurbo_cnd_c;

	// Token: 0x04001614 RID: 5652
	public durability alternator_cnd_c;

	// Token: 0x04001615 RID: 5653
	public durability battery_cnd_c;

	// Token: 0x04001616 RID: 5654
	public durability camshaft_cnd_c;

	// Token: 0x04001617 RID: 5655
	public durability camBearing1_cnd_c;

	// Token: 0x04001618 RID: 5656
	public durability camBearing2_cnd_c;

	// Token: 0x04001619 RID: 5657
	public durability camBearing3_cnd_c;

	// Token: 0x0400161A RID: 5658
	public durability camGear_cnd_c;

	// Token: 0x0400161B RID: 5659
	public durability carb_cnd_c;

	// Token: 0x0400161C RID: 5660
	public durability clutch_cnd_c;

	// Token: 0x0400161D RID: 5661
	public durability clutchDia_cnd_c;

	// Token: 0x0400161E RID: 5662
	public durability crank_cnd_c;

	// Token: 0x0400161F RID: 5663
	public durability crankPulley_cnd_c;

	// Token: 0x04001620 RID: 5664
	public durability distributor_cnd_c;

	// Token: 0x04001621 RID: 5665
	public durability driveRingGearF4_cnd_c;

	// Token: 0x04001622 RID: 5666
	public durability driveRingGearR4_cnd_c;

	// Token: 0x04001623 RID: 5667
	public durability driveRingGearF5_cnd_c;

	// Token: 0x04001624 RID: 5668
	public durability driveRingGearR5_cnd_c;

	// Token: 0x04001625 RID: 5669
	public durability exhaustMan_cnd_c;

	// Token: 0x04001626 RID: 5670
	public durability exhaustMan2_cnd_c;

	// Token: 0x04001627 RID: 5671
	public durability exhaustMan3_cnd_c;

	// Token: 0x04001628 RID: 5672
	public durability fanPulley_cnd_c;

	// Token: 0x04001629 RID: 5673
	public durability fanBelt_cnd_c;

	// Token: 0x0400162A RID: 5674
	public durability fanClutch_cnd_c;

	// Token: 0x0400162B RID: 5675
	public durability fan_cnd_c;

	// Token: 0x0400162C RID: 5676
	public durability flyWheel_cnd_c;

	// Token: 0x0400162D RID: 5677
	public durability frontDriveShaft_cnd_c;

	// Token: 0x0400162E RID: 5678
	public durability headgasket_cnd_c;

	// Token: 0x0400162F RID: 5679
	public durability head_cnd_c;

	// Token: 0x04001630 RID: 5680
	public durability intakeMan_cnd_c;

	// Token: 0x04001631 RID: 5681
	public durability intakeManEFI_cnd_c;

	// Token: 0x04001632 RID: 5682
	public durability intakeEFIThrottle_cnd_c;

	// Token: 0x04001633 RID: 5683
	public durability mainBearing1_cnd_c;

	// Token: 0x04001634 RID: 5684
	public durability mainBearing2_cnd_c;

	// Token: 0x04001635 RID: 5685
	public durability mainBearing3_cnd_c;

	// Token: 0x04001636 RID: 5686
	public durability mainBearing4_cnd_c;

	// Token: 0x04001637 RID: 5687
	public durability mainBearing5_cnd_c;

	// Token: 0x04001638 RID: 5688
	public durability oilFilter_cnd_c;

	// Token: 0x04001639 RID: 5689
	public durability oilPan_cnd_c;

	// Token: 0x0400163A RID: 5690
	public durability piston1_cnd_c;

	// Token: 0x0400163B RID: 5691
	public durability piston2_cnd_c;

	// Token: 0x0400163C RID: 5692
	public durability piston3_cnd_c;

	// Token: 0x0400163D RID: 5693
	public durability piston4_cnd_c;

	// Token: 0x0400163E RID: 5694
	public durability pistonbearing1_cnd_c;

	// Token: 0x0400163F RID: 5695
	public durability pistonbearing2_cnd_c;

	// Token: 0x04001640 RID: 5696
	public durability pistonbearing3_cnd_c;

	// Token: 0x04001641 RID: 5697
	public durability pistonbearing4_cnd_c;

	// Token: 0x04001642 RID: 5698
	public durability plugwires_cnd_c;

	// Token: 0x04001643 RID: 5699
	public durability radiator_cnd_c;

	// Token: 0x04001644 RID: 5700
	public durability rearDiff_cnd_c;

	// Token: 0x04001645 RID: 5701
	public durability rearDriveShaft_cnd_c;

	// Token: 0x04001646 RID: 5702
	public durability starterMotor_cnd_c;

	// Token: 0x04001647 RID: 5703
	public durability sparkPlug1_cnd_c;

	// Token: 0x04001648 RID: 5704
	public durability sparkPlug2_cnd_c;

	// Token: 0x04001649 RID: 5705
	public durability sparkPlug3_cnd_c;

	// Token: 0x0400164A RID: 5706
	public durability sparkPlug4_cnd_c;

	// Token: 0x0400164B RID: 5707
	public durability timingCover_cnd_c;

	// Token: 0x0400164C RID: 5708
	public durability timingGear16_cnd_c;

	// Token: 0x0400164D RID: 5709
	public durability timingGear16r_cnd_c;

	// Token: 0x0400164E RID: 5710
	public durability timingGear32_cnd_c;

	// Token: 0x0400164F RID: 5711
	public durability transmission_cnd_c;

	// Token: 0x04001650 RID: 5712
	public durability transferCase_cnd_c;

	// Token: 0x04001651 RID: 5713
	public durability turbo_cnd_c;

	// Token: 0x04001652 RID: 5714
	public durability turboPipe_cnd_c;

	// Token: 0x04001653 RID: 5715
	public durability valveAssembly_cnd_c;

	// Token: 0x04001654 RID: 5716
	public durability valveCover_cnd_c;

	// Token: 0x04001655 RID: 5717
	public durability compound1_cnd_c;

	// Token: 0x04001656 RID: 5718
	public durability compound2_cnd_c;

	// Token: 0x04001657 RID: 5719
	public durability turboXL_cnd_c;

	// Token: 0x04001658 RID: 5720
	public durability airFilterTurboXL_cnd_c;

	// Token: 0x04001659 RID: 5721
	public GameObject exhaustTrails;

	// Token: 0x0400165A RID: 5722
	private float newRust;

	// Token: 0x0400165B RID: 5723
	private GameObject rustPart;

	// Token: 0x0400165C RID: 5724
	public float newOilLevel;

	// Token: 0x0400165D RID: 5725
	public float newCoolantLevel;

	// Token: 0x0400165E RID: 5726
	public float newFuelLevel;

	// Token: 0x0400165F RID: 5727
	public float newTemperature;

	// Token: 0x04001660 RID: 5728
	public float newMaxTorque;

	// Token: 0x04001661 RID: 5729
	public float torqueReduction;

	// Token: 0x04001662 RID: 5730
	public float torqueIncrease;

	// Token: 0x04001663 RID: 5731
	public bool canRun = true;

	// Token: 0x04001664 RID: 5732
	public bool canCrank = true;

	// Token: 0x04001665 RID: 5733
	public bool canAcc = true;

	// Token: 0x04001666 RID: 5734
	public bool canEasyStart = true;

	// Token: 0x04001667 RID: 5735
	public bool canMove = true;

	// Token: 0x04001668 RID: 5736
	public bool can4wd = true;

	// Token: 0x04001669 RID: 5737
	public bool fanSpin = true;

	// Token: 0x0400166A RID: 5738
	public bool checkEngine;

	// Token: 0x0400166B RID: 5739
	public bool beltWhine;

	// Token: 0x0400166C RID: 5740
	public bool knocking;

	// Token: 0x0400166D RID: 5741
	public bool accelLag;

	// Token: 0x0400166E RID: 5742
	public bool whiteSmoke;

	// Token: 0x0400166F RID: 5743
	public bool blackSmoke;

	// Token: 0x04001670 RID: 5744
	public bool roughIdle;

	// Token: 0x04001671 RID: 5745
	public bool fire;

	// Token: 0x04001672 RID: 5746
	public float oilLoss;

	// Token: 0x04001673 RID: 5747
	public bool hasExhaustMan = true;

	// Token: 0x04001674 RID: 5748
	public bool fanClutchSeized;

	// Token: 0x04001675 RID: 5749
	public bool hasTurbo;

	// Token: 0x04001676 RID: 5750
	public bool hasTurbo2;

	// Token: 0x04001677 RID: 5751
	private string newPart;

	// Token: 0x04001678 RID: 5752
	private float thisHealth;

	// Token: 0x04001679 RID: 5753
	private int randPart;

	// Token: 0x0400167A RID: 5754
	public float maxTemp;

	// Token: 0x0400167B RID: 5755
	private float fancool;

	// Token: 0x0400167C RID: 5756
	private float batCharge;

	// Token: 0x0400167D RID: 5757
	private float tempIncrease;

	// Token: 0x0400167E RID: 5758
	public bool debugRun;

	// Token: 0x0400167F RID: 5759
	public float avgDmgCab;

	// Token: 0x04001680 RID: 5760
	public float prevDmgCab;

	// Token: 0x04001681 RID: 5761
	public ImpactDeformable idCab;

	// Token: 0x04001682 RID: 5762
	public GameObject[] breakableParts;

	// Token: 0x04001683 RID: 5763
	private int totalLoosened;

	// Token: 0x04001684 RID: 5764
	private float dmgAmount;

	// Token: 0x04001685 RID: 5765
	public Transform ebtemplate;

	// Token: 0x04001686 RID: 5766
	public Transform emptyRb;

	// Token: 0x04001687 RID: 5767
	public Rigidbody truckRb;

	// Token: 0x04001688 RID: 5768
	public bool nonOemTurbo;

	// Token: 0x04001689 RID: 5769
	public GameObject turboParticles;

	// Token: 0x0400168A RID: 5770
	public GameObject brokenTurboVis;

	// Token: 0x0400168B RID: 5771
	public int nonOemTurboPsi;

	// Token: 0x0400168C RID: 5772
	public GameObject modWoman;

	// Token: 0x0400168D RID: 5773
	public car truckScript;
}
