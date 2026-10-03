using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000172 RID: 370
public class enginev8 : MonoBehaviour
{
	// Token: 0x06000916 RID: 2326 RVA: 0x00079E84 File Offset: 0x00078084
	private void Start()
	{
		if (this.airFilter.GetComponent<durability>().mat == null)
		{
			this.airFilter.GetComponent<durability>().mat = this.rust;
			this.alternator.GetComponent<durability>().mat = this.rust;
			this.crank.GetComponent<durability>().mat = this.rust;
			this.camshaft.GetComponent<durability>().mat = this.rust;
			this.piston1.GetComponent<durability>().mat = this.rust;
			this.piston2.GetComponent<durability>().mat = this.rust;
			this.piston3.GetComponent<durability>().mat = this.rust;
			this.piston4.GetComponent<durability>().mat = this.rust;
			this.piston5.GetComponent<durability>().mat = this.rust;
			this.piston6.GetComponent<durability>().mat = this.rust;
			this.piston7.GetComponent<durability>().mat = this.rust;
			this.piston8.GetComponent<durability>().mat = this.rust;
			this.waterPump.GetComponent<durability>().mat = this.rust;
			this.am_turboD.GetComponent<durability>().mat = this.rust;
			this.am_turboP.GetComponent<durability>().mat = this.rust;
			this.am_turboD.GetComponent<durability>().mat2 = this.rust;
			this.am_turboP.GetComponent<durability>().mat2 = this.rust;
		}
		if (base.transform.parent != null)
		{
			if (base.transform.parent.name != "dirt pickup truck" && base.transform.parent.name != "f1003")
			{
				base.gameObject.GetComponent<PickUp>().pickable = true;
				this.whichTruck = 0;
			}
			if (base.transform.parent.name == "dirt pickup truck")
			{
				base.transform.position = this.ebtemplatev8.position;
				base.transform.rotation = this.ebtemplatev8.rotation;
				base.gameObject.GetComponent<FixedJoint>().connectedBody = this.truckRb;
				this.whichTruck = 1;
			}
			else if (base.transform.parent.name == "f1003")
			{
				base.transform.position = this.ebtemplatev8F.position;
				base.transform.rotation = this.ebtemplatev8F.rotation;
				base.gameObject.GetComponent<FixedJoint>().connectedBody = this.truck2Rb;
				this.whichTruck = 2;
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
				this.emptyRbV8.position = base.transform.position;
				base.gameObject.GetComponent<FixedJoint>().connectedBody = this.emptyRbV8.GetComponent<Rigidbody>();
			}
		}
		base.GetComponent<PickUp>().attachTo3 = "ebtemplate3v8";
		this.avgDmgCab = this.idCab.AverageStructuralDamage;
		this.prevDmgCab = this.avgDmgCab;
		if (base.gameObject.transform.parent == this.truck.transform)
		{
			base.StartCoroutine(this.AlignEngine());
		}
		base.StartCoroutine(this.RemoveDrag());
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x0007A241 File Offset: 0x00078441
	private IEnumerator RemoveDrag()
	{
		yield return new WaitForSeconds(1f);
		base.GetComponent<Rigidbody>().isKinematic = false;
		yield break;
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x0007A250 File Offset: 0x00078450
	private IEnumerator AlignEngine()
	{
		base.GetComponent<FixedJoint>().connectedBody = null;
		yield return new WaitForSeconds(0.1f);
		base.transform.rotation = this.ebtemplatev8.rotation;
		base.transform.position = this.ebtemplatev8.position;
		base.gameObject.GetComponent<FixedJoint>().connectedBody = this.truck.GetComponent<Rigidbody>();
		yield break;
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x0007A260 File Offset: 0x00078460
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

	// Token: 0x0600091A RID: 2330 RVA: 0x0007A4F0 File Offset: 0x000786F0
	public void DrainBat()
	{
		if (this.whichTruck == 1 && this.battery_cnd > 0f)
		{
			this.battery_cnd -= 1f;
		}
		if (this.whichTruck == 2 && this.batteryF_cnd > 0f)
		{
			this.batteryF_cnd -= 1f;
		}
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x0007A550 File Offset: 0x00078750
	public void Refresh()
	{
		this.airFilter_cnd = this.airFilter_cnd_c.health;
		this.alternator_cnd = this.alternator_cnd_c.health;
		this.battery_cnd = this.battery_cnd_c.health;
		this.batteryF_cnd = this.batteryF_cnd_c.health;
		this.block_cnd = this.block_cnd_c.health;
		this.camshaft_cnd = this.camshaft_cnd_c.health;
		this.camGear_cnd = this.camGear_cnd_c.health;
		this.carb_cnd = this.carb_cnd_c.health;
		this.crank_cnd = this.crank_cnd_c.health;
		this.crankPulley_cnd = this.crankPulley_cnd_c.health;
		this.distributor_cnd = this.distributor_cnd_c.health;
		this.driveRingGearF4_cnd = this.driveRingGearF4_cnd_c.health;
		this.driveRingGearR4_cnd = this.driveRingGearR4_cnd_c.health;
		this.driveRingGearF5_cnd = this.driveRingGearF5_cnd_c.health;
		this.driveRingGearR5_cnd = this.driveRingGearR5_cnd_c.health;
		this.driveRingGearF4F_cnd = this.driveRingGearF4F_cnd_c.health;
		this.driveRingGearR4F_cnd = this.driveRingGearR4F_cnd_c.health;
		this.driveRingGearF5F_cnd = this.driveRingGearF5F_cnd_c.health;
		this.driveRingGearR5F_cnd = this.driveRingGearR5F_cnd_c.health;
		this.exhaustManD_cnd = this.exhaustManD_cnd_c.health;
		this.exhaustManP_cnd = this.exhaustManP_cnd_c.health;
		this.fan_cnd = this.fan_cnd_c.health;
		this.flyWheel_cnd = this.flyWheel_cnd_c.health;
		this.fuelpump_cnd = this.fuelpump_cnd_c.health;
		this.headD_cnd = this.headD_cnd_c.health;
		this.headP_cnd = this.headP_cnd_c.health;
		this.intakeMan_cnd = this.intakeMan_cnd_c.health;
		this.oilFilter_cnd = this.oilFilter_cnd_c.health;
		this.oilPan_cnd = this.oilPan_cnd_c.health;
		this.piston1_cnd = this.piston1_cnd_c.health;
		this.piston2_cnd = this.piston2_cnd_c.health;
		this.piston3_cnd = this.piston3_cnd_c.health;
		this.piston4_cnd = this.piston4_cnd_c.health;
		this.piston5_cnd = this.piston5_cnd_c.health;
		this.piston6_cnd = this.piston6_cnd_c.health;
		this.piston7_cnd = this.piston7_cnd_c.health;
		this.piston8_cnd = this.piston8_cnd_c.health;
		this.plugwires_cnd = this.plugwires_cnd_c.health;
		this.radiator_cnd = this.radiator_cnd_c.health;
		this.starterMotor_cnd = this.starterMotor_cnd_c.health;
		this.timingChain_cnd = this.timingChain_cnd_c.health;
		this.timingCover_cnd = this.timingCover_cnd_c.health;
		this.timingGearCrank_cnd = this.timingGearCrank_cnd_c.health;
		this.torqueCon_cnd = this.torqueCon_cnd_c.health;
		this.transmission_cnd = this.transmission_cnd_c.health;
		this.transferCase_cnd = this.transferCase_cnd_c.health;
		this.transferCaseF_cnd = this.transferCaseF_cnd_c.health;
		this.valveCoverD_cnd = this.valveCoverD_cnd_c.health;
		this.valveCoverP_cnd = this.valveCoverP_cnd_c.health;
		this.waterPump_cnd = this.waterPump_cnd_c.health;
		this.am_headerD_cnd = this.am_headerD_cnd_c.health;
		this.am_headerP_cnd = this.am_headerP_cnd_c.health;
		this.am_turboD_cnd = this.am_turboD_cnd_c.health;
		this.am_turboP_cnd = this.am_turboP_cnd_c.health;
		this.am_ram_cnd = this.am_ram_cnd_c.health;
		this.am_intercoolerD_cnd = this.am_intercoolerD_cnd_c.health;
		this.am_intercoolerP_cnd = this.am_intercoolerP_cnd_c.health;
		this.am_intake_cnd = this.am_intake_cnd_c.health;
		this.am_oilcoolerD_cnd = this.am_oilcoolerD_cnd_c.health;
		this.am_fuelrail_cnd = this.am_fuelrail_cnd_c.health;
		this.am_transmission_cnd = this.am_transmission_cnd_c.health;
		this.am_transmissionF_cnd = this.am_transmissionF_cnd_c.health;
		if (base.transform.parent != null && base.transform.parent.name == "dirt pickup truck")
		{
			this.newFuelLevel = this.truck.GetComponent<car>().fuel;
		}
		else
		{
			this.newFuelLevel = this.truck2.GetComponent<car4>().fuel;
		}
		this.torqueReduction = 0f;
		this.radiator_cnd = 100f;
		this.sparkPlug1_cnd = 100f;
		this.sparkPlug2_cnd = 100f;
		this.sparkPlug3_cnd = 100f;
		this.sparkPlug4_cnd = 100f;
		this.mainBearing1_cnd = 100f;
		this.mainBearing2_cnd = 100f;
		this.mainBearing3_cnd = 100f;
		this.mainBearing4_cnd = 100f;
		this.mainBearing5_cnd = 100f;
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
		if (this.valveCoverD_cnd < 1f)
		{
			this.oilLoss = 1f;
		}
		if (this.valveCoverP_cnd < 1f)
		{
			this.oilLoss = 1f;
		}
		if (this.oilPan_cnd < 1f)
		{
			this.oilLoss = 100f;
			this.newOilLevel = 0f;
		}
		if (this.camGear_cnd == 0f || this.timingGearCrank_cnd == 0f || this.timingChain_cnd == 0f || this.crank_cnd == 0f || this.headP_cnd == 0f || this.headD_cnd == 0f || this.camshaft_cnd == 0f || this.fuelpump_cnd == 0f || this.distributor_cnd == 0f || this.newFuelLevel < 1f || this.plugwires_cnd == 0f)
		{
			this.canRun = false;
		}
		if (this.whichTruck == 1 && this.battery_cnd < 1f)
		{
			this.canRun = false;
		}
		if (this.whichTruck == 2 && this.batteryF_cnd < 1f)
		{
			this.canRun = false;
		}
		if (this.am_intake_cnd == 0f && (this.intakeMan_cnd == 0f || this.carb_cnd == 0f))
		{
			this.canRun = false;
		}
		if (this.am_intake_cnd > 0f && this.am_fuelrail_cnd == 0f)
		{
			this.canRun = false;
		}
		if (this.truck.GetComponent<car>().usingV8 && base.transform.parent != null && base.transform.parent.name != "dirt pickup truck" && this.whichTruck == 1)
		{
			this.canRun = false;
			this.canCrank = false;
		}
		if (base.transform.parent != null && base.transform.parent.name != "f1003" && this.whichTruck == 2)
		{
			this.canRun = false;
			this.canCrank = false;
		}
		if (base.transform.parent == null)
		{
			this.canRun = false;
			this.canCrank = false;
		}
		if (this.whichTruck == 1)
		{
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
		}
		if (this.whichTruck == 2)
		{
			if (this.batteryF_cnd == 0f || this.starterMotor_cnd == 0f)
			{
				this.canCrank = false;
			}
			if (this.batteryF_cnd > 0f)
			{
				this.canAcc = true;
			}
			else
			{
				this.canAcc = false;
			}
		}
		int num = 0;
		if (this.piston1_cnd > 1f)
		{
			num++;
		}
		if (this.piston2_cnd > 1f)
		{
			num++;
		}
		if (this.piston3_cnd > 1f)
		{
			num++;
		}
		if (this.piston4_cnd > 1f)
		{
			num++;
		}
		if (this.piston5_cnd > 1f)
		{
			num++;
		}
		if (this.piston6_cnd > 1f)
		{
			num++;
		}
		if (this.piston7_cnd > 1f)
		{
			num++;
		}
		if (this.piston8_cnd > 1f)
		{
			num++;
		}
		if (num < 6)
		{
			this.canRun = false;
		}
		int num2 = 0;
		if (this.mainBearing1_cnd > 0f)
		{
			num2++;
		}
		if (this.mainBearing2_cnd > 0f)
		{
			num2++;
		}
		if (this.mainBearing3_cnd > 0f)
		{
			num2++;
		}
		if (this.mainBearing4_cnd > 0f)
		{
			num2++;
		}
		if (this.mainBearing5_cnd > 0f)
		{
			num2++;
		}
		if (num2 < 2)
		{
			this.canRun = false;
		}
		if (this.whichTruck == 1)
		{
			if (this.torqueCon_cnd == 0f || this.flyWheel_cnd == 0f || (this.transmission_cnd == 0f && this.am_transmission_cnd == 0f) || this.transferCase_cnd == 0f || (this.driveRingGearF4_cnd == 0f && this.driveRingGearF5_cnd == 0f && this.driveRingGearR4_cnd == 0f && this.driveRingGearR5_cnd == 0f))
			{
				this.canMove = false;
			}
			else
			{
				this.canMove = true;
				this.truck.GetComponent<car>().maxTorque = 800f - this.torqueReduction;
				this.truck.GetComponent<car>().maxTorqueStatic = 800f - this.torqueReduction;
			}
		}
		if (this.whichTruck == 2)
		{
			if (this.torqueCon_cnd == 0f || this.flyWheel_cnd == 0f || this.am_transmissionF_cnd == 0f || this.transferCaseF_cnd == 0f || (this.driveRingGearF4F_cnd == 0f && this.driveRingGearF5F_cnd == 0f && this.driveRingGearR4F_cnd == 0f && this.driveRingGearR5F_cnd == 0f))
			{
				this.canMove = false;
			}
			else
			{
				this.canMove = true;
				this.truck2.GetComponent<car4>().maxTorque = 800f - this.torqueReduction;
				this.truck2.GetComponent<car4>().maxTorqueStatic = 800f - this.torqueReduction;
			}
		}
		if (this.exhaustManP_cnd == 0f || this.exhaustManD_cnd == 0f)
		{
			this.hasExhaustMan = false;
		}
		else
		{
			this.hasExhaustMan = true;
		}
		this.checkEngine = false;
		if (this.alternator_cnd < 10f || this.distributor_cnd < 10f || this.fanBelt_cnd < 1f || this.piston1_cnd < 1f || this.piston2_cnd < 1f || this.piston3_cnd < 1f || this.piston4_cnd < 1f || this.piston5_cnd < 1f || this.piston6_cnd < 1f || this.piston7_cnd < 1f || this.piston8_cnd < 1f)
		{
			this.checkEngine = true;
		}
		if (this.airFilter_cnd < 10f && this.airFilter.GetComponent<Renderer>().enabled)
		{
			this.checkEngine = true;
		}
		if (this.carb_cnd < 10f && this.carb.GetComponent<Renderer>().enabled)
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
		if (this.piston5_cnd < 30f)
		{
			this.torqueReduction += (30f - this.piston5_cnd) * 5f;
		}
		if (this.piston6_cnd < 30f)
		{
			this.torqueReduction += (30f - this.piston6_cnd) * 5f;
		}
		if (this.piston7_cnd < 30f)
		{
			this.torqueReduction += (30f - this.piston7_cnd) * 5f;
		}
		if (this.piston8_cnd < 30f)
		{
			this.torqueReduction += (30f - this.piston8_cnd) * 5f;
		}
		float num3 = this.headgasketD_cnd;
		float num4 = this.headgasketD_cnd;
		if (this.headD_cnd < 30f)
		{
			this.torqueReduction += 30f - this.headD_cnd;
		}
		if (this.headP_cnd < 30f)
		{
			this.torqueReduction += 30f - this.headP_cnd;
		}
		if (this.waterPump_cnd < 5f)
		{
			this.tempIncrease += 50f;
		}
		if (this.fan_cnd < 10f)
		{
			this.tempIncrease += 15f;
			this.fanClutchSeized = true;
		}
		if (this.fan_cnd == 0f || this.fanBelt_cnd == 0f || this.waterPump_cnd == 0f || this.crankPulley_cnd == 0f)
		{
			this.fancool = 0f;
			this.fanSpin = false;
			if (this.whichTruck == 1)
			{
				this.truck.GetComponent<car>().fanSpin = false;
			}
			if (this.whichTruck == 2)
			{
				this.truck2.GetComponent<car4>().fanSpin = false;
			}
		}
		if (this.fanSpin)
		{
			if (this.whichTruck == 1)
			{
				this.truck.GetComponent<car>().fanSpin = true;
			}
			if (this.whichTruck == 2)
			{
				this.truck2.GetComponent<car4>().fanSpin = true;
			}
			this.fancool = 50f;
		}
		if (this.fanBelt_cnd > 0f && this.fanBelt_cnd < 20f)
		{
			this.beltWhine = true;
		}
		if (this.am_turboD_cnd > 5f && this.am_ram_cnd > 0f && this.am_intake_cnd > 0f && this.am_headerD_cnd > 0f && this.am_intercoolerD_cnd > 0f)
		{
			this.hasTurboD = true;
			this.torqueIncrease = 100f - (100f - this.am_turboD_cnd);
		}
		else
		{
			this.hasTurboD = false;
		}
		if (this.am_turboP_cnd > 5f && this.am_ram_cnd > 0f && this.am_intake_cnd > 0f && this.am_headerP_cnd > 0f && this.am_intercoolerP_cnd > 0f)
		{
			this.hasTurboP = true;
			this.torqueIncrease = this.torqueIncrease + 100f - (100f - this.am_turboP_cnd);
		}
		else
		{
			this.hasTurboP = false;
		}
		if (this.hasTurboD && this.hasTurboP && this.am_oilcoolerD_cnd == 0f)
		{
			this.tempIncrease += 30f;
		}
		if (this.whichTruck == 1)
		{
			this.truck.GetComponent<car>().checkEngine = this.checkEngine;
			this.truck.GetComponent<car>().canRun = this.canRun;
			this.truck.GetComponent<car>().canCrank = this.canCrank;
			this.truck.GetComponent<car>().canAcc = this.canAcc;
			this.truck.GetComponent<car>().canEasyStart = this.canEasyStart;
			this.newCoolantLevel = this.truck.GetComponent<car>().coolantLevel;
			float additive = this.truck.GetComponent<car>().additive;
			float num5 = 0f;
			if (additive > 3f)
			{
				num5 = 40f;
			}
			this.maxTemp = 440f - this.newOilLevel - this.newCoolantLevel - this.fancool + this.tempIncrease + num5;
			this.truck.GetComponent<car>().maxTemp = this.maxTemp;
			if (this.canMove)
			{
				this.truck.GetComponent<car>().maxTorque = 900f - this.torqueReduction + this.torqueIncrease;
				this.truck.GetComponent<car>().maxTorqueStatic = 900f - this.torqueReduction + this.torqueIncrease;
			}
			else
			{
				this.truck.GetComponent<car>().maxTorque = 0f;
				this.truck.GetComponent<car>().maxTorqueStatic = 0f;
			}
		}
		if (this.whichTruck == 2)
		{
			this.truck2.GetComponent<car4>().checkEngine = this.checkEngine;
			this.truck2.GetComponent<car4>().canRun = this.canRun;
			this.truck2.GetComponent<car4>().canCrank = this.canCrank;
			this.truck2.GetComponent<car4>().canAcc = this.canAcc;
			this.truck2.GetComponent<car4>().canEasyStart = this.canEasyStart;
			this.newCoolantLevel = this.truck2.GetComponent<car4>().coolantLevel;
			float additive2 = this.truck2.GetComponent<car4>().additive;
			float num6 = 0f;
			if (additive2 > 3f)
			{
				num6 = 40f;
			}
			this.maxTemp = 440f - this.newOilLevel - this.newCoolantLevel - this.fancool + this.tempIncrease + num6;
			this.truck2.GetComponent<car4>().maxTemp = this.maxTemp;
			if (this.canMove)
			{
				this.truck2.GetComponent<car4>().maxTorque = 900f - this.torqueReduction + this.torqueIncrease;
				this.truck2.GetComponent<car4>().maxTorqueStatic = 900f - this.torqueReduction + this.torqueIncrease;
			}
			else
			{
				this.truck2.GetComponent<car4>().maxTorque = 0f;
				this.truck2.GetComponent<car4>().maxTorqueStatic = 0f;
			}
		}
		if (this.alternator_cnd > 5f && this.fanBelt_cnd > 5f && this.waterPump_cnd > 0f && this.batteryF_cnd < 100f && this.batteryF_cnd > 4f)
		{
			this.batteryF_cnd += 1f;
			this.batteryF_cnd_c.health += 1f;
			this.charging = true;
			return;
		}
		this.charging = false;
	}

	// Token: 0x0600091C RID: 2332 RVA: 0x0007B88C File Offset: 0x00079A8C
	public void DegradeEngine()
	{
		this.CrashDamageCab();
		if (this.headgasketD_cnd < 40f || this.headgasketP_cnd < 40f)
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
		this.randPart = Random.Range(0, 27);
		switch (this.randPart)
		{
		case 0:
			this.airFilter_cnd_c.health -= 2f;
			break;
		case 1:
			this.alternator_cnd_c.health -= 1f;
			break;
		case 2:
			this.waterPump_cnd_c.health -= 1f;
			break;
		case 3:
			this.fanBelt_cnd_c.health -= 2f;
			break;
		case 6:
			this.oilFilter_cnd_c.health -= 1f;
			break;
		case 7:
			this.piston1_cnd_c.health -= 1f;
			break;
		case 8:
			this.piston2_cnd_c.health -= 1f;
			break;
		case 9:
			this.piston3_cnd_c.health -= 1f;
			break;
		case 10:
			this.piston4_cnd_c.health -= 1f;
			break;
		case 11:
			this.piston5_cnd_c.health -= 1f;
			break;
		case 12:
			this.piston6_cnd_c.health -= 1f;
			break;
		case 13:
			this.piston7_cnd_c.health -= 1f;
			break;
		case 14:
			this.piston8_cnd_c.health -= 1f;
			break;
		case 16:
			if (this.whichTruck == 1)
			{
				this.transferCase_cnd_c.health -= 1f;
			}
			else
			{
				this.transferCaseF_cnd_c.health -= 1f;
			}
			break;
		}
		if (this.transmission.GetComponent<Renderer>().enabled && this.whichTruck == 1)
		{
			this.transmission_cnd_c.health -= 1f;
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
				this.piston5_cnd_c.health -= 1f;
				return;
			case 8:
				this.piston6_cnd_c.health -= 1f;
				return;
			case 9:
				this.piston7_cnd_c.health -= 1f;
				return;
			case 10:
				this.piston8_cnd_c.health -= 1f;
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x0007BCD8 File Offset: 0x00079ED8
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
		Debug.Log(newPart);
		uint num = <PrivateImplementationDetails>.ComputeStringHash(newPart);
		if (num <= 2238967098U)
		{
			if (num <= 928208460U)
			{
				if (num <= 475701482U)
				{
					if (num <= 174210553U)
					{
						if (num != 9854079U)
						{
							if (num != 52291886U)
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
								if (!(newPart == "oilPan_e"))
								{
									return;
								}
								this.oilPan_cnd = this.thisHealth;
								return;
							}
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
					else if (num <= 233948812U)
					{
						if (num != 190356788U)
						{
							if (num != 233948812U)
							{
								return;
							}
							if (!(newPart == "am_intercoolerD_e"))
							{
								return;
							}
							this.am_intercoolerD_cnd = this.thisHealth;
							return;
						}
						else
						{
							if (!(newPart == "am_ram_e"))
							{
								return;
							}
							this.am_ram_cnd = this.thisHealth;
							return;
						}
					}
					else if (num != 281939837U)
					{
						if (num != 475701482U)
						{
							return;
						}
						if (!(newPart == "v8_trans_e"))
						{
							return;
						}
						this.am_transmission_cnd = this.thisHealth;
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
				else if (num <= 669411081U)
				{
					if (num != 651080792U)
					{
						if (num != 664167478U)
						{
							if (num != 669411081U)
							{
								return;
							}
							if (!(newPart == "exhaustManP_e"))
							{
								return;
							}
							this.exhaustManP_cnd = this.thisHealth;
							return;
						}
						else
						{
							if (!(newPart == "piston6_e"))
							{
								return;
							}
							this.piston6_cnd = this.thisHealth;
							return;
						}
					}
					else
					{
						if (!(newPart == "am_fuelrail_e"))
						{
							return;
						}
						this.am_fuelrail_cnd = this.thisHealth;
						return;
					}
				}
				else if (num <= 810995921U)
				{
					if (num != 724924004U)
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
						if (!(newPart == "am_turboD_e"))
						{
							return;
						}
						this.am_turboD_cnd = this.thisHealth;
						return;
					}
				}
				else if (num != 920453885U)
				{
					if (num != 928208460U)
					{
						return;
					}
					if (!(newPart == "headgasketP_e"))
					{
						return;
					}
					this.headgasketP_cnd = this.thisHealth;
					return;
				}
				else
				{
					if (!(newPart == "airFilter_e"))
					{
						return;
					}
					this.airFilter_cnd = this.thisHealth;
					return;
				}
			}
			else if (num <= 1671286021U)
			{
				if (num <= 1267621612U)
				{
					if (num != 971399780U)
					{
						if (num != 1219331248U)
						{
							if (num != 1267621612U)
							{
								return;
							}
							if (!(newPart == "crank_e"))
							{
								return;
							}
							this.crank_cnd = this.thisHealth;
							return;
						}
						else
						{
							if (!(newPart == "am_intercoolerP_e"))
							{
								return;
							}
							this.am_intercoolerP_cnd = this.thisHealth;
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
				else if (num <= 1449680990U)
				{
					if (num != 1373709499U)
					{
						if (num != 1449680990U)
						{
							return;
						}
						if (!(newPart == "mainBearing4_e"))
						{
							return;
						}
						this.mainBearing4_cnd = this.thisHealth;
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
				else if (num != 1586455776U)
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
					if (!(newPart == "am_intake_e"))
					{
						return;
					}
					this.am_intake_cnd = this.thisHealth;
					return;
				}
			}
			else if (num <= 1772950300U)
			{
				if (num != 1726167345U)
				{
					if (num != 1731197960U)
					{
						if (num != 1772950300U)
						{
							return;
						}
						if (!(newPart == "v8_fanbelt_e"))
						{
							return;
						}
						this.fanBelt_cnd = this.thisHealth;
						return;
					}
					else
					{
						if (!(newPart == "am_turboP_e"))
						{
							return;
						}
						this.am_turboP_cnd = this.thisHealth;
						return;
					}
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
				if (num != 1913694064U)
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
					if (!(newPart == "headgasketD_e"))
					{
						return;
					}
					this.headgasketD_cnd = this.thisHealth;
					return;
				}
			}
			else if (num != 2127005802U)
			{
				if (num != 2238967098U)
				{
					return;
				}
				if (!(newPart == "piston2_e"))
				{
					return;
				}
				this.piston2_cnd = this.thisHealth;
				return;
			}
			else
			{
				if (!(newPart == "distributor_e"))
				{
					return;
				}
				this.distributor_cnd = this.thisHealth;
				return;
			}
		}
		else if (num <= 3002107513U)
		{
			if (num <= 2750239128U)
			{
				if (num <= 2513163517U)
				{
					if (num != 2247828113U)
					{
						if (num != 2321819551U)
						{
							if (num != 2513163517U)
							{
								return;
							}
							if (!(newPart == "exhaustManD_e"))
							{
								return;
							}
							this.exhaustManD_cnd = this.thisHealth;
							return;
						}
						else
						{
							if (!(newPart == "am_headerD_e"))
							{
								return;
							}
							this.am_headerD_cnd = this.thisHealth;
							return;
						}
					}
					else
					{
						if (!(newPart == "piston7_e"))
						{
							return;
						}
						this.piston7_cnd = this.thisHealth;
						return;
					}
				}
				else if (num <= 2556484634U)
				{
					if (num != 2523929471U)
					{
						if (num != 2556484634U)
						{
							return;
						}
						if (!(newPart == "am_oilcoolerD_e"))
						{
							return;
						}
						this.am_oilcoolerD_cnd = this.thisHealth;
						return;
					}
					else
					{
						if (!(newPart == "piston5_e"))
						{
							return;
						}
						this.piston5_cnd = this.thisHealth;
						return;
					}
				}
				else if (num != 2738013703U)
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
			else if (num <= 2785518523U)
			{
				if (num != 2779615980U)
				{
					if (num != 2783869835U)
					{
						if (num != 2785518523U)
						{
							return;
						}
						if (!(newPart == "timingGearCrank"))
						{
							return;
						}
						this.timingGearCrank_cnd = this.thisHealth;
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
				else
				{
					if (!(newPart == "valveCoverP_e"))
					{
						return;
					}
					this.valveCoverP_cnd = this.thisHealth;
					return;
				}
			}
			else if (num <= 2840574821U)
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
			else if (num != 2915391505U)
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
				if (!(newPart == "headD_e"))
				{
					return;
				}
				this.headD_cnd = this.thisHealth;
				return;
			}
		}
		else if (num <= 3791496789U)
		{
			if (num <= 3110926065U)
			{
				if (num != 3032140064U)
				{
					if (num != 3074763045U)
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
						if (!(newPart == "headP_e"))
						{
							return;
						}
						this.headP_cnd = this.thisHealth;
						return;
					}
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
			else if (num <= 3733144160U)
			{
				if (num != 3317647747U)
				{
					if (num != 3733144160U)
					{
						return;
					}
					if (!(newPart == "piston8_e"))
					{
						return;
					}
					this.piston8_cnd = this.thisHealth;
					return;
				}
				else
				{
					if (!(newPart == "am_headerP_e"))
					{
						return;
					}
					this.am_headerP_cnd = this.thisHealth;
					return;
				}
			}
			else if (num != 3785786768U)
			{
				if (num != 3791496789U)
				{
					return;
				}
				if (!(newPart == "piston3_e"))
				{
					return;
				}
				this.piston3_cnd = this.thisHealth;
				return;
			}
			else
			{
				if (!(newPart == "valveCoverD_e"))
				{
					return;
				}
				this.valveCoverD_cnd = this.thisHealth;
				return;
			}
		}
		else if (num <= 4165522486U)
		{
			if (num != 3830287187U)
			{
				if (num != 3944105470U)
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
					if (!(newPart == "transferCase_e"))
					{
						return;
					}
					this.transferCase_cnd = this.thisHealth;
					return;
				}
			}
			else
			{
				if (!(newPart == "piston1_e"))
				{
					return;
				}
				this.piston1_cnd = this.thisHealth;
				return;
			}
		}
		else if (num <= 4248663954U)
		{
			if (num != 4212894421U)
			{
				if (num != 4248663954U)
				{
					return;
				}
				if (!(newPart == "battery_e"))
				{
					return;
				}
				this.battery_cnd = this.thisHealth;
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
		else if (num != 4282535884U)
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
			if (!(newPart == "timingChain"))
			{
				return;
			}
			this.timingChain_cnd = this.thisHealth;
			return;
		}
	}

	// Token: 0x04001743 RID: 5955
	public GameObject truck;

	// Token: 0x04001744 RID: 5956
	public GameObject truck2;

	// Token: 0x04001745 RID: 5957
	public GameObject airFilter;

	// Token: 0x04001746 RID: 5958
	public GameObject alternator;

	// Token: 0x04001747 RID: 5959
	public GameObject battery;

	// Token: 0x04001748 RID: 5960
	public GameObject batteryF;

	// Token: 0x04001749 RID: 5961
	public GameObject block;

	// Token: 0x0400174A RID: 5962
	public GameObject camshaft;

	// Token: 0x0400174B RID: 5963
	public GameObject camGear;

	// Token: 0x0400174C RID: 5964
	public GameObject carb;

	// Token: 0x0400174D RID: 5965
	public GameObject crank;

	// Token: 0x0400174E RID: 5966
	public GameObject crankPulley;

	// Token: 0x0400174F RID: 5967
	public GameObject distributor;

	// Token: 0x04001750 RID: 5968
	public GameObject driveRingGearF4;

	// Token: 0x04001751 RID: 5969
	public GameObject driveRingGearR4;

	// Token: 0x04001752 RID: 5970
	public GameObject driveRingGearF5;

	// Token: 0x04001753 RID: 5971
	public GameObject driveRingGearR5;

	// Token: 0x04001754 RID: 5972
	public GameObject driveRingGearF4F;

	// Token: 0x04001755 RID: 5973
	public GameObject driveRingGearR4F;

	// Token: 0x04001756 RID: 5974
	public GameObject driveRingGearF5F;

	// Token: 0x04001757 RID: 5975
	public GameObject driveRingGearR5F;

	// Token: 0x04001758 RID: 5976
	public GameObject exhaustManD;

	// Token: 0x04001759 RID: 5977
	public GameObject exhaustManP;

	// Token: 0x0400175A RID: 5978
	public GameObject fan;

	// Token: 0x0400175B RID: 5979
	public GameObject fanBelt;

	// Token: 0x0400175C RID: 5980
	public GameObject flyWheel;

	// Token: 0x0400175D RID: 5981
	public GameObject fuelpump;

	// Token: 0x0400175E RID: 5982
	public GameObject headgasketD;

	// Token: 0x0400175F RID: 5983
	public GameObject headgasketP;

	// Token: 0x04001760 RID: 5984
	public GameObject headD;

	// Token: 0x04001761 RID: 5985
	public GameObject headP;

	// Token: 0x04001762 RID: 5986
	public GameObject intakeMan;

	// Token: 0x04001763 RID: 5987
	public GameObject mainBearing1;

	// Token: 0x04001764 RID: 5988
	public GameObject mainBearing2;

	// Token: 0x04001765 RID: 5989
	public GameObject mainBearing3;

	// Token: 0x04001766 RID: 5990
	public GameObject mainBearing4;

	// Token: 0x04001767 RID: 5991
	public GameObject mainBearing5;

	// Token: 0x04001768 RID: 5992
	public GameObject oilFilter;

	// Token: 0x04001769 RID: 5993
	public GameObject oilPan;

	// Token: 0x0400176A RID: 5994
	public GameObject piston1;

	// Token: 0x0400176B RID: 5995
	public GameObject piston2;

	// Token: 0x0400176C RID: 5996
	public GameObject piston3;

	// Token: 0x0400176D RID: 5997
	public GameObject piston4;

	// Token: 0x0400176E RID: 5998
	public GameObject piston5;

	// Token: 0x0400176F RID: 5999
	public GameObject piston6;

	// Token: 0x04001770 RID: 6000
	public GameObject piston7;

	// Token: 0x04001771 RID: 6001
	public GameObject piston8;

	// Token: 0x04001772 RID: 6002
	public GameObject plugwires;

	// Token: 0x04001773 RID: 6003
	public GameObject radiator;

	// Token: 0x04001774 RID: 6004
	public GameObject rearDiff;

	// Token: 0x04001775 RID: 6005
	public GameObject starterMotor;

	// Token: 0x04001776 RID: 6006
	public GameObject timingChain;

	// Token: 0x04001777 RID: 6007
	public GameObject timingCover;

	// Token: 0x04001778 RID: 6008
	public GameObject timingGearCrank;

	// Token: 0x04001779 RID: 6009
	public GameObject torqueCon;

	// Token: 0x0400177A RID: 6010
	public GameObject transmission;

	// Token: 0x0400177B RID: 6011
	public GameObject transferCase;

	// Token: 0x0400177C RID: 6012
	public GameObject transferCaseF;

	// Token: 0x0400177D RID: 6013
	public GameObject valveCoverD;

	// Token: 0x0400177E RID: 6014
	public GameObject valveCoverP;

	// Token: 0x0400177F RID: 6015
	public GameObject waterPump;

	// Token: 0x04001780 RID: 6016
	public GameObject am_headerD;

	// Token: 0x04001781 RID: 6017
	public GameObject am_headerP;

	// Token: 0x04001782 RID: 6018
	public GameObject am_turboD;

	// Token: 0x04001783 RID: 6019
	public GameObject am_turboP;

	// Token: 0x04001784 RID: 6020
	public GameObject am_ram;

	// Token: 0x04001785 RID: 6021
	public GameObject am_intercoolerD;

	// Token: 0x04001786 RID: 6022
	public GameObject am_intercoolerP;

	// Token: 0x04001787 RID: 6023
	public GameObject am_intake;

	// Token: 0x04001788 RID: 6024
	public GameObject am_oilcoolerD;

	// Token: 0x04001789 RID: 6025
	public GameObject am_fuelrail;

	// Token: 0x0400178A RID: 6026
	public GameObject am_transmission;

	// Token: 0x0400178B RID: 6027
	public GameObject am_transmissionF;

	// Token: 0x0400178C RID: 6028
	public float airFilter_cnd;

	// Token: 0x0400178D RID: 6029
	public float alternator_cnd;

	// Token: 0x0400178E RID: 6030
	public float battery_cnd;

	// Token: 0x0400178F RID: 6031
	public float batteryF_cnd;

	// Token: 0x04001790 RID: 6032
	public float block_cnd;

	// Token: 0x04001791 RID: 6033
	public float camshaft_cnd;

	// Token: 0x04001792 RID: 6034
	public float camGear_cnd;

	// Token: 0x04001793 RID: 6035
	public float carb_cnd;

	// Token: 0x04001794 RID: 6036
	public float crank_cnd;

	// Token: 0x04001795 RID: 6037
	public float crankPulley_cnd;

	// Token: 0x04001796 RID: 6038
	public float distributor_cnd;

	// Token: 0x04001797 RID: 6039
	public float driveRingGearF4_cnd;

	// Token: 0x04001798 RID: 6040
	public float driveRingGearR4_cnd;

	// Token: 0x04001799 RID: 6041
	public float driveRingGearF5_cnd;

	// Token: 0x0400179A RID: 6042
	public float driveRingGearR5_cnd;

	// Token: 0x0400179B RID: 6043
	public float driveRingGearF4F_cnd;

	// Token: 0x0400179C RID: 6044
	public float driveRingGearR4F_cnd;

	// Token: 0x0400179D RID: 6045
	public float driveRingGearF5F_cnd;

	// Token: 0x0400179E RID: 6046
	public float driveRingGearR5F_cnd;

	// Token: 0x0400179F RID: 6047
	public float exhaustManD_cnd;

	// Token: 0x040017A0 RID: 6048
	public float exhaustManP_cnd;

	// Token: 0x040017A1 RID: 6049
	public float fan_cnd;

	// Token: 0x040017A2 RID: 6050
	public float fanBelt_cnd;

	// Token: 0x040017A3 RID: 6051
	public float flyWheel_cnd;

	// Token: 0x040017A4 RID: 6052
	public float fuelpump_cnd;

	// Token: 0x040017A5 RID: 6053
	public float headgasketD_cnd;

	// Token: 0x040017A6 RID: 6054
	public float headgasketP_cnd;

	// Token: 0x040017A7 RID: 6055
	public float headD_cnd;

	// Token: 0x040017A8 RID: 6056
	public float headP_cnd;

	// Token: 0x040017A9 RID: 6057
	public float intakeMan_cnd;

	// Token: 0x040017AA RID: 6058
	public float mainBearing1_cnd;

	// Token: 0x040017AB RID: 6059
	public float mainBearing2_cnd;

	// Token: 0x040017AC RID: 6060
	public float mainBearing3_cnd;

	// Token: 0x040017AD RID: 6061
	public float mainBearing4_cnd;

	// Token: 0x040017AE RID: 6062
	public float mainBearing5_cnd;

	// Token: 0x040017AF RID: 6063
	public float oilFilter_cnd;

	// Token: 0x040017B0 RID: 6064
	public float oilPan_cnd;

	// Token: 0x040017B1 RID: 6065
	public float piston1_cnd;

	// Token: 0x040017B2 RID: 6066
	public float piston2_cnd;

	// Token: 0x040017B3 RID: 6067
	public float piston3_cnd;

	// Token: 0x040017B4 RID: 6068
	public float piston4_cnd;

	// Token: 0x040017B5 RID: 6069
	public float piston5_cnd;

	// Token: 0x040017B6 RID: 6070
	public float piston6_cnd;

	// Token: 0x040017B7 RID: 6071
	public float piston7_cnd;

	// Token: 0x040017B8 RID: 6072
	public float piston8_cnd;

	// Token: 0x040017B9 RID: 6073
	public float plugwires_cnd;

	// Token: 0x040017BA RID: 6074
	public float rearDiff_cnd;

	// Token: 0x040017BB RID: 6075
	public float starterMotor_cnd;

	// Token: 0x040017BC RID: 6076
	public float timingChain_cnd;

	// Token: 0x040017BD RID: 6077
	public float timingCover_cnd;

	// Token: 0x040017BE RID: 6078
	public float timingGearCrank_cnd;

	// Token: 0x040017BF RID: 6079
	public float torqueCon_cnd;

	// Token: 0x040017C0 RID: 6080
	public float transmission_cnd;

	// Token: 0x040017C1 RID: 6081
	public float transferCase_cnd;

	// Token: 0x040017C2 RID: 6082
	public float transferCaseF_cnd;

	// Token: 0x040017C3 RID: 6083
	public float valveCoverD_cnd;

	// Token: 0x040017C4 RID: 6084
	public float valveCoverP_cnd;

	// Token: 0x040017C5 RID: 6085
	public float waterPump_cnd;

	// Token: 0x040017C6 RID: 6086
	public float am_headerD_cnd;

	// Token: 0x040017C7 RID: 6087
	public float am_headerP_cnd;

	// Token: 0x040017C8 RID: 6088
	public float am_turboD_cnd;

	// Token: 0x040017C9 RID: 6089
	public float am_turboP_cnd;

	// Token: 0x040017CA RID: 6090
	public float am_ram_cnd;

	// Token: 0x040017CB RID: 6091
	public float am_intercoolerD_cnd;

	// Token: 0x040017CC RID: 6092
	public float am_intercoolerP_cnd;

	// Token: 0x040017CD RID: 6093
	public float am_intake_cnd;

	// Token: 0x040017CE RID: 6094
	public float am_oilcoolerD_cnd;

	// Token: 0x040017CF RID: 6095
	public float am_fuelrail_cnd;

	// Token: 0x040017D0 RID: 6096
	public float am_transmission_cnd;

	// Token: 0x040017D1 RID: 6097
	public float am_transmissionF_cnd;

	// Token: 0x040017D2 RID: 6098
	public float radiator_cnd;

	// Token: 0x040017D3 RID: 6099
	public float sparkPlug1_cnd;

	// Token: 0x040017D4 RID: 6100
	public float sparkPlug2_cnd;

	// Token: 0x040017D5 RID: 6101
	public float sparkPlug3_cnd;

	// Token: 0x040017D6 RID: 6102
	public float sparkPlug4_cnd;

	// Token: 0x040017D7 RID: 6103
	public durability airFilter_cnd_c;

	// Token: 0x040017D8 RID: 6104
	public durability alternator_cnd_c;

	// Token: 0x040017D9 RID: 6105
	public durability battery_cnd_c;

	// Token: 0x040017DA RID: 6106
	public durability batteryF_cnd_c;

	// Token: 0x040017DB RID: 6107
	public durability block_cnd_c;

	// Token: 0x040017DC RID: 6108
	public durability camshaft_cnd_c;

	// Token: 0x040017DD RID: 6109
	public durability camGear_cnd_c;

	// Token: 0x040017DE RID: 6110
	public durability carb_cnd_c;

	// Token: 0x040017DF RID: 6111
	public durability crank_cnd_c;

	// Token: 0x040017E0 RID: 6112
	public durability crankPulley_cnd_c;

	// Token: 0x040017E1 RID: 6113
	public durability distributor_cnd_c;

	// Token: 0x040017E2 RID: 6114
	public durability driveRingGearF4_cnd_c;

	// Token: 0x040017E3 RID: 6115
	public durability driveRingGearR4_cnd_c;

	// Token: 0x040017E4 RID: 6116
	public durability driveRingGearF5_cnd_c;

	// Token: 0x040017E5 RID: 6117
	public durability driveRingGearR5_cnd_c;

	// Token: 0x040017E6 RID: 6118
	public durability driveRingGearF4F_cnd_c;

	// Token: 0x040017E7 RID: 6119
	public durability driveRingGearR4F_cnd_c;

	// Token: 0x040017E8 RID: 6120
	public durability driveRingGearF5F_cnd_c;

	// Token: 0x040017E9 RID: 6121
	public durability driveRingGearR5F_cnd_c;

	// Token: 0x040017EA RID: 6122
	public durability exhaustManD_cnd_c;

	// Token: 0x040017EB RID: 6123
	public durability exhaustManP_cnd_c;

	// Token: 0x040017EC RID: 6124
	public durability fan_cnd_c;

	// Token: 0x040017ED RID: 6125
	public durability fanBelt_cnd_c;

	// Token: 0x040017EE RID: 6126
	public durability flyWheel_cnd_c;

	// Token: 0x040017EF RID: 6127
	public durability fuelpump_cnd_c;

	// Token: 0x040017F0 RID: 6128
	public durability headgasketD_cnd_c;

	// Token: 0x040017F1 RID: 6129
	public durability headgasketP_cnd_c;

	// Token: 0x040017F2 RID: 6130
	public durability headD_cnd_c;

	// Token: 0x040017F3 RID: 6131
	public durability headP_cnd_c;

	// Token: 0x040017F4 RID: 6132
	public durability intakeMan_cnd_c;

	// Token: 0x040017F5 RID: 6133
	public durability mainBearing1_cnd_c;

	// Token: 0x040017F6 RID: 6134
	public durability mainBearing2_cnd_c;

	// Token: 0x040017F7 RID: 6135
	public durability mainBearing3_cnd_c;

	// Token: 0x040017F8 RID: 6136
	public durability mainBearing4_cnd_c;

	// Token: 0x040017F9 RID: 6137
	public durability mainBearing5_cnd_c;

	// Token: 0x040017FA RID: 6138
	public durability oilFilter_cnd_c;

	// Token: 0x040017FB RID: 6139
	public durability oilPan_cnd_c;

	// Token: 0x040017FC RID: 6140
	public durability piston1_cnd_c;

	// Token: 0x040017FD RID: 6141
	public durability piston2_cnd_c;

	// Token: 0x040017FE RID: 6142
	public durability piston3_cnd_c;

	// Token: 0x040017FF RID: 6143
	public durability piston4_cnd_c;

	// Token: 0x04001800 RID: 6144
	public durability piston5_cnd_c;

	// Token: 0x04001801 RID: 6145
	public durability piston6_cnd_c;

	// Token: 0x04001802 RID: 6146
	public durability piston7_cnd_c;

	// Token: 0x04001803 RID: 6147
	public durability piston8_cnd_c;

	// Token: 0x04001804 RID: 6148
	public durability plugwires_cnd_c;

	// Token: 0x04001805 RID: 6149
	public durability radiator_cnd_c;

	// Token: 0x04001806 RID: 6150
	public durability rearDiff_cnd_c;

	// Token: 0x04001807 RID: 6151
	public durability starterMotor_cnd_c;

	// Token: 0x04001808 RID: 6152
	public durability timingChain_cnd_c;

	// Token: 0x04001809 RID: 6153
	public durability timingCover_cnd_c;

	// Token: 0x0400180A RID: 6154
	public durability timingGearCrank_cnd_c;

	// Token: 0x0400180B RID: 6155
	public durability torqueCon_cnd_c;

	// Token: 0x0400180C RID: 6156
	public durability transmission_cnd_c;

	// Token: 0x0400180D RID: 6157
	public durability transferCase_cnd_c;

	// Token: 0x0400180E RID: 6158
	public durability transferCaseF_cnd_c;

	// Token: 0x0400180F RID: 6159
	public durability valveCoverD_cnd_c;

	// Token: 0x04001810 RID: 6160
	public durability valveCoverP_cnd_c;

	// Token: 0x04001811 RID: 6161
	public durability waterPump_cnd_c;

	// Token: 0x04001812 RID: 6162
	public durability am_headerD_cnd_c;

	// Token: 0x04001813 RID: 6163
	public durability am_headerP_cnd_c;

	// Token: 0x04001814 RID: 6164
	public durability am_turboD_cnd_c;

	// Token: 0x04001815 RID: 6165
	public durability am_turboP_cnd_c;

	// Token: 0x04001816 RID: 6166
	public durability am_ram_cnd_c;

	// Token: 0x04001817 RID: 6167
	public durability am_intercoolerD_cnd_c;

	// Token: 0x04001818 RID: 6168
	public durability am_intercoolerP_cnd_c;

	// Token: 0x04001819 RID: 6169
	public durability am_intake_cnd_c;

	// Token: 0x0400181A RID: 6170
	public durability am_oilcoolerD_cnd_c;

	// Token: 0x0400181B RID: 6171
	public durability am_fuelrail_cnd_c;

	// Token: 0x0400181C RID: 6172
	public durability am_transmission_cnd_c;

	// Token: 0x0400181D RID: 6173
	public durability am_transmissionF_cnd_c;

	// Token: 0x0400181E RID: 6174
	public GameObject exhaustTrails;

	// Token: 0x0400181F RID: 6175
	private float newRust;

	// Token: 0x04001820 RID: 6176
	private GameObject rustPart;

	// Token: 0x04001821 RID: 6177
	public float newOilLevel;

	// Token: 0x04001822 RID: 6178
	public float newCoolantLevel;

	// Token: 0x04001823 RID: 6179
	public float newFuelLevel;

	// Token: 0x04001824 RID: 6180
	public float newTemperature;

	// Token: 0x04001825 RID: 6181
	public float newMaxTorque;

	// Token: 0x04001826 RID: 6182
	public float torqueReduction;

	// Token: 0x04001827 RID: 6183
	public float torqueIncrease;

	// Token: 0x04001828 RID: 6184
	public bool canRun = true;

	// Token: 0x04001829 RID: 6185
	public bool canCrank = true;

	// Token: 0x0400182A RID: 6186
	public bool canAcc = true;

	// Token: 0x0400182B RID: 6187
	public bool canEasyStart = true;

	// Token: 0x0400182C RID: 6188
	public bool canMove = true;

	// Token: 0x0400182D RID: 6189
	public bool can4wd = true;

	// Token: 0x0400182E RID: 6190
	public bool fanSpin = true;

	// Token: 0x0400182F RID: 6191
	public bool checkEngine;

	// Token: 0x04001830 RID: 6192
	public bool beltWhine;

	// Token: 0x04001831 RID: 6193
	public bool knocking;

	// Token: 0x04001832 RID: 6194
	public bool accelLag;

	// Token: 0x04001833 RID: 6195
	public bool whiteSmoke;

	// Token: 0x04001834 RID: 6196
	public bool blackSmoke;

	// Token: 0x04001835 RID: 6197
	public bool roughIdle;

	// Token: 0x04001836 RID: 6198
	public bool fire;

	// Token: 0x04001837 RID: 6199
	public float oilLoss;

	// Token: 0x04001838 RID: 6200
	public bool hasExhaustMan = true;

	// Token: 0x04001839 RID: 6201
	public bool fanClutchSeized;

	// Token: 0x0400183A RID: 6202
	public bool hasTurboD;

	// Token: 0x0400183B RID: 6203
	public bool hasTurboP;

	// Token: 0x0400183C RID: 6204
	private string newPart;

	// Token: 0x0400183D RID: 6205
	private float thisHealth;

	// Token: 0x0400183E RID: 6206
	private int randPart;

	// Token: 0x0400183F RID: 6207
	public float maxTemp;

	// Token: 0x04001840 RID: 6208
	private float fancool;

	// Token: 0x04001841 RID: 6209
	private float batCharge;

	// Token: 0x04001842 RID: 6210
	private float tempIncrease;

	// Token: 0x04001843 RID: 6211
	public bool debugRun;

	// Token: 0x04001844 RID: 6212
	public float avgDmgCab;

	// Token: 0x04001845 RID: 6213
	public float prevDmgCab;

	// Token: 0x04001846 RID: 6214
	public ImpactDeformable idCab;

	// Token: 0x04001847 RID: 6215
	public GameObject[] breakableParts;

	// Token: 0x04001848 RID: 6216
	private int totalLoosened;

	// Token: 0x04001849 RID: 6217
	private float dmgAmount;

	// Token: 0x0400184A RID: 6218
	public Transform ebtemplatev8;

	// Token: 0x0400184B RID: 6219
	public Transform ebtemplatev8F;

	// Token: 0x0400184C RID: 6220
	public Transform emptyRbV8;

	// Token: 0x0400184D RID: 6221
	public Rigidbody truckRb;

	// Token: 0x0400184E RID: 6222
	public Rigidbody truck2Rb;

	// Token: 0x0400184F RID: 6223
	public int whichTruck;

	// Token: 0x04001850 RID: 6224
	public bool charging;

	// Token: 0x04001851 RID: 6225
	public Material rust;
}
