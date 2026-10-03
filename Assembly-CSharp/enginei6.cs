using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000171 RID: 369
public class enginei6 : MonoBehaviour
{
	// Token: 0x0600090D RID: 2317 RVA: 0x00078148 File Offset: 0x00076348
	private void Start()
	{
		if (base.transform.parent != null)
		{
			if (base.transform.parent.name != "dirt pickup truck" && base.transform.parent.name != "amc" && base.transform.parent.name != "supra6")
			{
				base.gameObject.GetComponent<PickUp>().pickable = true;
				this.whichTruck = 0;
			}
			if (base.transform.parent.name == "dirt pickup truck")
			{
				base.transform.position = this.ebtemplatei6.position;
				base.transform.rotation = this.ebtemplatei6.rotation;
				base.gameObject.GetComponent<FixedJoint>().connectedBody = this.truckRb;
				this.whichTruck = 1;
			}
			else if (base.transform.parent.name == "amc")
			{
				base.transform.position = this.ebtemplatei6Car.position;
				base.transform.rotation = this.ebtemplatei6Car.rotation;
				base.gameObject.GetComponent<FixedJoint>().connectedBody = this.carRb;
				this.whichTruck = 2;
			}
			else if (base.transform.parent.name == "supra6")
			{
			}
		}
		else
		{
			base.gameObject.GetComponent<PickUp>().pickable = true;
		}
		if (base.transform.parent == null)
		{
			base.GetComponent<Rigidbody>().useGravity = true;
			this.emptyRbi6.position = base.transform.position;
			base.gameObject.GetComponent<FixedJoint>().connectedBody = this.emptyRbi6.GetComponent<Rigidbody>();
			base.GetComponent<Rigidbody>().isKinematic = false;
			base.StartCoroutine(this.RemoveDrag());
		}
		this.avgDmgCab = this.idCab.AverageStructuralDamage;
		this.prevDmgCab = this.avgDmgCab;
		if (base.gameObject.transform.parent == this.truck.transform || base.gameObject.transform.parent == this.car.transform)
		{
			base.StartCoroutine(this.AlignEngine());
		}
	}

	// Token: 0x0600090E RID: 2318 RVA: 0x000783AE File Offset: 0x000765AE
	private IEnumerator RemoveDrag()
	{
		base.GetComponent<Rigidbody>().isKinematic = true;
		yield return new WaitForSeconds(1f);
		base.GetComponent<Rigidbody>().isKinematic = false;
		yield break;
	}

	// Token: 0x0600090F RID: 2319 RVA: 0x000783BD File Offset: 0x000765BD
	private IEnumerator AlignEngine()
	{
		base.GetComponent<FixedJoint>().connectedBody = null;
		yield return new WaitForSeconds(1.25f);
		if (this.whichTruck == 1)
		{
			base.transform.rotation = this.ebtemplatei6.rotation;
			base.transform.position = this.ebtemplatei6.position;
			base.gameObject.GetComponent<FixedJoint>().connectedBody = this.truckRb;
			this.truck.GetComponent<car>().usingI6 = true;
		}
		else if (this.whichTruck == 2)
		{
			base.transform.rotation = this.ebtemplatei6Car.rotation;
			base.transform.position = this.ebtemplatei6Car.position;
			base.gameObject.GetComponent<FixedJoint>().connectedBody = this.carRb;
			this.car.GetComponent<car3>().usingI6 = true;
		}
		yield break;
	}

	// Token: 0x06000910 RID: 2320 RVA: 0x000783CC File Offset: 0x000765CC
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

	// Token: 0x06000911 RID: 2321 RVA: 0x0007865C File Offset: 0x0007685C
	public void DrainBat()
	{
		if (this.whichTruck == 1 && this.battery_cnd > 0f)
		{
			this.battery_cnd -= 1f;
		}
		if (this.whichTruck == 2 && this.batteryC_cnd > 0f)
		{
			this.batteryC_cnd -= 1f;
		}
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x000786BC File Offset: 0x000768BC
	public void Refresh()
	{
		this.acPump_cnd = this.acPump_cnd_c.health;
		this.airFilter_cnd = this.airFilter_cnd_c.health;
		this.alternator_cnd = this.alternator_cnd_c.health;
		this.battery_cnd = this.battery_cnd_c.health;
		this.beltTensioner_cnd = this.beltTensioner_cnd_c.health;
		this.beltWheel_cnd = this.beltWheel_cnd_c.health;
		this.block_cnd = this.block_cnd_c.health;
		this.camshaft_cnd = this.camshaft_cnd_c.health;
		this.camshaft2_cnd = this.camshaft2_cnd_c.health;
		this.clutch_cnd = this.clutch_cnd_c.health;
		this.clutchDia_cnd = this.clutchDia_cnd_c.health;
		this.crank_cnd = this.crank_cnd_c.health;
		this.crankPulley_cnd = this.crankPulley_cnd_c.health;
		this.driveRingGearF4_cnd = this.driveRingGearF4_cnd_c.health;
		this.driveRingGearR4_cnd = this.driveRingGearR4_cnd_c.health;
		this.driveRingGearF5_cnd = this.driveRingGearF5_cnd_c.health;
		this.driveRingGearR5_cnd = this.driveRingGearR5_cnd_c.health;
		this.exhaustMan_cnd = this.exhaustMan_cnd_c.health;
		this.fan_cnd = this.fan_cnd_c.health;
		this.fanBelt_cnd = this.fanBelt_cnd_c.health;
		this.flyWheel_cnd = this.flyWheel_cnd_c.health;
		this.head_cnd = this.head_cnd_c.health;
		this.intakeMan_cnd = this.intakeMan_cnd_c.health;
		this.oilFilter_cnd = this.oilFilter_cnd_c.health;
		this.oilPan_cnd = this.oilPan_cnd_c.health;
		this.piston1_cnd = this.piston1_cnd_c.health;
		this.piston2_cnd = this.piston2_cnd_c.health;
		this.piston3_cnd = this.piston3_cnd_c.health;
		this.piston4_cnd = this.piston4_cnd_c.health;
		this.piston5_cnd = this.piston5_cnd_c.health;
		this.piston6_cnd = this.piston6_cnd_c.health;
		this.powerSteeringPump_cnd = this.powerSteeringPump_cnd_c.health;
		this.starterMotor_cnd = this.starterMotor_cnd_c.health;
		this.timingChain_cnd = this.timingChain_cnd_c.health;
		this.transmission_cnd = this.transmission_cnd_c.health;
		this.transferCase_cnd = this.transferCase_cnd_c.health;
		this.valveCover_cnd = this.valveCover_cnd_c.health;
		this.waterPump_cnd = this.waterPump_cnd_c.health;
		if (base.transform.parent != null)
		{
			if (base.transform.parent.name == "dirt pickup truck")
			{
				this.newFuelLevel = this.truck.GetComponent<car>().fuel;
			}
			else if (base.transform.parent.name == "amc")
			{
				this.newFuelLevel = this.car.GetComponent<car3>().fuel;
			}
		}
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
		if (this.timingChain_cnd == 0f || this.crank_cnd == 0f || this.head_cnd == 0f || this.camshaft_cnd == 0f || this.camshaft2_cnd == 0f || this.newFuelLevel < 1f)
		{
			this.canRun = false;
		}
		if (this.whichTruck == 1 && this.battery_cnd < 1f)
		{
			this.canRun = false;
		}
		if (this.intakeMan_cnd == 0f)
		{
			this.canRun = false;
		}
		if (this.truck.GetComponent<car>().usingI6 && base.transform.parent != null && base.transform.parent.name != "dirt pickup truck" && this.whichTruck == 1)
		{
			this.canRun = false;
			this.canCrank = false;
		}
		if (base.transform.parent != null && base.transform.parent.name != "amc" && this.whichTruck == 2)
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
			if (this.starterMotor_cnd == 0f)
			{
				this.canCrank = false;
			}
			if (this.batteryC_cnd > 0f)
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
		if (num < 4)
		{
			this.canRun = false;
			Debug.Log("9");
		}
		if (this.whichTruck == 1)
		{
			if (this.clutch_cnd == 0f || this.clutchDia_cnd == 0f || this.flyWheel_cnd == 0f || this.transmission_cnd == 0f || this.transferCase_cnd == 0f || (this.driveRingGearF4_cnd == 0f && this.driveRingGearF5_cnd == 0f && this.driveRingGearR4_cnd == 0f && this.driveRingGearR5_cnd == 0f))
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
			if (this.flyWheel_cnd == 0f)
			{
				this.canMove = false;
			}
			else
			{
				this.canMove = true;
				this.car.GetComponent<car3>().maxTorque = 2800f - this.torqueReduction;
				this.car.GetComponent<car3>().maxTorqueStatic = 2800f - this.torqueReduction;
			}
		}
		if (this.exhaustMan_cnd == 0f)
		{
			this.hasExhaustMan = false;
		}
		else
		{
			this.hasExhaustMan = true;
		}
		this.checkEngine = false;
		if (this.alternator_cnd < 10f || this.fanBelt_cnd < 1f || this.piston1_cnd < 1f || this.piston2_cnd < 1f || this.piston3_cnd < 1f || this.piston4_cnd < 1f || this.piston5_cnd < 1f || this.piston6_cnd < 1f)
		{
			this.checkEngine = true;
		}
		if (this.airFilter_cnd < 10f && this.airFilter.GetComponent<Renderer>().enabled)
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
		float num2 = this.headgasket_cnd;
		if (this.head_cnd < 30f)
		{
			this.torqueReduction += 30f - this.head_cnd;
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
		if (this.fan_cnd == 0f || this.fanBelt_cnd == 0f || this.waterPump_cnd == 0f || this.crankPulley_cnd == 0f || this.timingChain_cnd == 0f)
		{
			this.fancool = 0f;
			this.fanSpin = false;
			if (this.whichTruck == 1)
			{
				this.truck.GetComponent<car>().fanSpin = false;
			}
			if (this.whichTruck == 2)
			{
				this.car.GetComponent<car3>().fanSpin = false;
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
				this.car.GetComponent<car3>().fanSpin = true;
			}
			this.fancool = 50f;
		}
		if (this.fanBelt_cnd > 0f && this.fanBelt_cnd < 20f)
		{
			this.beltWhine = true;
		}
		if (this.debugRun)
		{
			this.canRun = true;
			this.canAcc = true;
			this.canCrank = true;
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
			float num3 = 0f;
			if (additive > 3f)
			{
				num3 = 40f;
			}
			this.maxTemp = 440f - this.newOilLevel - this.newCoolantLevel - this.fancool + this.tempIncrease + num3;
			this.truck.GetComponent<car>().maxTemp = this.maxTemp;
			if (this.canMove)
			{
				this.truck.GetComponent<car>().maxTorque = 1000f - this.torqueReduction + this.torqueIncrease;
				this.truck.GetComponent<car>().maxTorqueStatic = 1000f - this.torqueReduction + this.torqueIncrease;
			}
			else
			{
				this.truck.GetComponent<car>().maxTorque = 0f;
				this.truck.GetComponent<car>().maxTorqueStatic = 0f;
			}
		}
		if (this.whichTruck == 2)
		{
			this.car.GetComponent<car3>().checkEngine = this.checkEngine;
			this.car.GetComponent<car3>().canRun = this.canRun;
			this.car.GetComponent<car3>().canCrank = this.canCrank;
			this.car.GetComponent<car3>().canAcc = this.canAcc;
			this.car.GetComponent<car3>().canEasyStart = this.canEasyStart;
			this.newCoolantLevel = this.car.GetComponent<car3>().coolantLevel;
			this.maxTemp = 440f - this.newOilLevel - this.newCoolantLevel - this.fancool + this.tempIncrease;
			this.car.GetComponent<car3>().maxTemp = this.maxTemp;
			Debug.Log("cr2" + this.canCrank.ToString());
			if (this.canMove)
			{
				this.car.GetComponent<car3>().maxTorque = 800f - this.torqueReduction + this.torqueIncrease;
				this.car.GetComponent<car3>().maxTorqueStatic = 800f - this.torqueReduction + this.torqueIncrease;
			}
			else
			{
				this.car.GetComponent<car3>().maxTorque = 0f;
				this.car.GetComponent<car3>().maxTorqueStatic = 0f;
			}
		}
		if (this.alternator_cnd > 5f && this.fanBelt_cnd > 5f && this.waterPump_cnd > 0f && this.batteryC_cnd < 100f)
		{
			this.charging = true;
			return;
		}
		this.charging = false;
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x000794A4 File Offset: 0x000776A4
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
		case 14:
			if (this.whichTruck == 1)
			{
				this.transferCase_cnd_c.health -= 1f;
			}
			break;
		}
		if (this.transmission.GetComponent<Renderer>().enabled && this.whichTruck == 1)
		{
			this.transmission_cnd_c.health -= 1f;
		}
		if (this.newOilLevel < 42f)
		{
			this.randPart = Random.Range(0, 13);
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
				this.camshaft2_cnd_c.health -= 1f;
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x0007986C File Offset: 0x00077A6C
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
		uint num = <PrivateImplementationDetails>.ComputeStringHash(newPart);
		if (num <= 3019994026U)
		{
			if (num <= 1726167345U)
			{
				if (num <= 463779669U)
				{
					if (num <= 347702489U)
					{
						if (num != 143857462U)
						{
							if (num != 347702489U)
							{
								return;
							}
							if (!(newPart == "i6_piston5_e"))
							{
								return;
							}
							this.piston5_cnd = this.thisHealth;
							return;
						}
						else
						{
							if (!(newPart == "i6_timingchain"))
							{
								return;
							}
							this.timingChain_cnd = this.thisHealth;
							return;
						}
					}
					else if (num != 377631872U)
					{
						if (num != 463779669U)
						{
							return;
						}
						if (!(newPart == "i6_beltwheel_e"))
						{
							return;
						}
						this.beltWheel_cnd = this.thisHealth;
						return;
					}
					else
					{
						if (!(newPart == "i6_piston2_e"))
						{
							return;
						}
						this.piston2_cnd = this.thisHealth;
						return;
					}
				}
				else if (num <= 1020987842U)
				{
					if (num != 882163223U)
					{
						if (num != 1020987842U)
						{
							return;
						}
						if (!(newPart == "i6_headgasket_e"))
						{
							return;
						}
						this.headgasket_cnd = this.thisHealth;
						return;
					}
					else
					{
						if (!(newPart == "i6_flyWheel_e"))
						{
							return;
						}
						this.flyWheel_cnd = this.thisHealth;
						return;
					}
				}
				else if (num != 1212213611U)
				{
					if (num != 1726167345U)
					{
						return;
					}
					if (!(newPart == "transmission_e"))
					{
						return;
					}
					this.transmission_cnd = this.thisHealth;
					return;
				}
				else
				{
					if (!(newPart == "i6_crankpulley_e"))
					{
						return;
					}
					this.crankPulley_cnd = this.thisHealth;
					return;
				}
			}
			else if (num <= 2415489542U)
			{
				if (num <= 2238882383U)
				{
					if (num != 1922502109U)
					{
						if (num != 2238882383U)
						{
							return;
						}
						if (!(newPart == "i6_belttensioner_e"))
						{
							return;
						}
						this.beltTensioner_cnd = this.thisHealth;
						return;
					}
					else
					{
						if (!(newPart == "i6_piston1_e"))
						{
							return;
						}
						this.piston1_cnd = this.thisHealth;
						return;
					}
				}
				else if (num != 2250666087U)
				{
					if (num != 2415489542U)
					{
						return;
					}
					if (!(newPart == "i6_starter_e"))
					{
						return;
					}
					this.starterMotor_cnd = this.thisHealth;
					return;
				}
				else
				{
					if (!(newPart == "i6_airFilter_e"))
					{
						return;
					}
					this.airFilter_cnd = this.thisHealth;
					return;
				}
			}
			else if (num <= 2783869835U)
			{
				if (num != 2457372075U)
				{
					if (num != 2783869835U)
					{
						return;
					}
					if (!(newPart == "radiator_e"))
					{
						return;
					}
					this.radiator_cnd = this.thisHealth;
					return;
				}
				else
				{
					if (!(newPart == "i6_intake_e"))
					{
						return;
					}
					this.intakeMan_cnd = this.thisHealth;
					return;
				}
			}
			else if (num != 2954280684U)
			{
				if (num != 3019994026U)
				{
					return;
				}
				if (!(newPart == "i6_crank_e"))
				{
					return;
				}
				this.crank_cnd = this.thisHealth;
				return;
			}
			else
			{
				if (!(newPart == "i6_fan_e"))
				{
					return;
				}
				this.fan_cnd = this.thisHealth;
				return;
			}
		}
		else if (num <= 3673570664U)
		{
			if (num <= 3090140094U)
			{
				if (num <= 3084336248U)
				{
					if (num != 3035060123U)
					{
						if (num != 3084336248U)
						{
							return;
						}
						if (!(newPart == "i6_oilpan_e"))
						{
							return;
						}
						this.oilPan_cnd = this.thisHealth;
						return;
					}
					else
					{
						if (!(newPart == "i6_piston3_e"))
						{
							return;
						}
						this.piston3_cnd = this.thisHealth;
						return;
					}
				}
				else if (num != 3086334891U)
				{
					if (num != 3090140094U)
					{
						return;
					}
					if (!(newPart == "i6_piston4_e"))
					{
						return;
					}
					this.piston4_cnd = this.thisHealth;
					return;
				}
				else
				{
					if (!(newPart == "i6_oilfilter_e"))
					{
						return;
					}
					this.oilFilter_cnd = this.thisHealth;
					return;
				}
			}
			else if (num <= 3518993550U)
			{
				if (num != 3397475564U)
				{
					if (num != 3518993550U)
					{
						return;
					}
					if (!(newPart == "i6_valvecover_e"))
					{
						return;
					}
					this.valveCover_cnd = this.thisHealth;
					return;
				}
				else
				{
					if (!(newPart == "i6_piston6_e"))
					{
						return;
					}
					this.piston6_cnd = this.thisHealth;
					return;
				}
			}
			else if (num != 3632541539U)
			{
				if (num != 3673570664U)
				{
					return;
				}
				if (!(newPart == "i6_cam2_e"))
				{
					return;
				}
				this.camshaft2_cnd = this.thisHealth;
				return;
			}
			else
			{
				if (!(newPart == "i6_powersteering_e"))
				{
					return;
				}
				this.powerSteeringPump_cnd = this.thisHealth;
				return;
			}
		}
		else if (num <= 3944105470U)
		{
			if (num <= 3704278057U)
			{
				if (num != 3691861533U)
				{
					if (num != 3704278057U)
					{
						return;
					}
					if (!(newPart == "i6_alternator_e"))
					{
						return;
					}
					this.alternator_cnd = this.thisHealth;
					return;
				}
				else
				{
					if (!(newPart == "i6_head_e"))
					{
						return;
					}
					this.head_cnd = this.thisHealth;
					return;
				}
			}
			else if (num != 3891680017U)
			{
				if (num != 3944105470U)
				{
					return;
				}
				if (!(newPart == "transferCase_e"))
				{
					return;
				}
				this.transferCase_cnd = this.thisHealth;
				return;
			}
			else
			{
				if (!(newPart == "i6_acpump_e"))
				{
					return;
				}
				this.acPump_cnd = this.thisHealth;
				return;
			}
		}
		else if (num <= 4160335462U)
		{
			if (num != 4075267931U)
			{
				if (num != 4160335462U)
				{
					return;
				}
				if (!(newPart == "i6_cam_e"))
				{
					return;
				}
				this.camshaft_cnd = this.thisHealth;
				return;
			}
			else
			{
				if (!(newPart == "i6_exhaust_e"))
				{
					return;
				}
				this.exhaustMan_cnd = this.thisHealth;
				return;
			}
		}
		else if (num != 4210243638U)
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
			if (!(newPart == "i6_belt_e"))
			{
				return;
			}
			this.fanBelt_cnd = this.thisHealth;
			return;
		}
	}

	// Token: 0x0400168E RID: 5774
	public GameObject truck;

	// Token: 0x0400168F RID: 5775
	public GameObject car;

	// Token: 0x04001690 RID: 5776
	public GameObject acPump;

	// Token: 0x04001691 RID: 5777
	public GameObject airFilter;

	// Token: 0x04001692 RID: 5778
	public GameObject alternator;

	// Token: 0x04001693 RID: 5779
	public GameObject battery;

	// Token: 0x04001694 RID: 5780
	public GameObject batteryC;

	// Token: 0x04001695 RID: 5781
	public GameObject beltTensioner;

	// Token: 0x04001696 RID: 5782
	public GameObject beltWheel;

	// Token: 0x04001697 RID: 5783
	public GameObject block;

	// Token: 0x04001698 RID: 5784
	public GameObject camshaft;

	// Token: 0x04001699 RID: 5785
	public GameObject camshaft2;

	// Token: 0x0400169A RID: 5786
	public GameObject clutch;

	// Token: 0x0400169B RID: 5787
	public GameObject clutchDia;

	// Token: 0x0400169C RID: 5788
	public GameObject crank;

	// Token: 0x0400169D RID: 5789
	public GameObject crankPulley;

	// Token: 0x0400169E RID: 5790
	public GameObject driveRingGearF4;

	// Token: 0x0400169F RID: 5791
	public GameObject driveRingGearR4;

	// Token: 0x040016A0 RID: 5792
	public GameObject driveRingGearF5;

	// Token: 0x040016A1 RID: 5793
	public GameObject driveRingGearR5;

	// Token: 0x040016A2 RID: 5794
	public GameObject exhaustMan;

	// Token: 0x040016A3 RID: 5795
	public GameObject fan;

	// Token: 0x040016A4 RID: 5796
	public GameObject fanBelt;

	// Token: 0x040016A5 RID: 5797
	public GameObject flyWheel;

	// Token: 0x040016A6 RID: 5798
	public GameObject headgasket;

	// Token: 0x040016A7 RID: 5799
	public GameObject head;

	// Token: 0x040016A8 RID: 5800
	public GameObject intakeMan;

	// Token: 0x040016A9 RID: 5801
	public GameObject oilFilter;

	// Token: 0x040016AA RID: 5802
	public GameObject oilPan;

	// Token: 0x040016AB RID: 5803
	public GameObject piston1;

	// Token: 0x040016AC RID: 5804
	public GameObject piston2;

	// Token: 0x040016AD RID: 5805
	public GameObject piston3;

	// Token: 0x040016AE RID: 5806
	public GameObject piston4;

	// Token: 0x040016AF RID: 5807
	public GameObject piston5;

	// Token: 0x040016B0 RID: 5808
	public GameObject piston6;

	// Token: 0x040016B1 RID: 5809
	public GameObject powerSteeringPump;

	// Token: 0x040016B2 RID: 5810
	public GameObject radiator;

	// Token: 0x040016B3 RID: 5811
	public GameObject starterMotor;

	// Token: 0x040016B4 RID: 5812
	public GameObject timingChain;

	// Token: 0x040016B5 RID: 5813
	public GameObject transmission;

	// Token: 0x040016B6 RID: 5814
	public GameObject transferCase;

	// Token: 0x040016B7 RID: 5815
	public GameObject valveCover;

	// Token: 0x040016B8 RID: 5816
	public GameObject waterPump;

	// Token: 0x040016B9 RID: 5817
	public float acPump_cnd;

	// Token: 0x040016BA RID: 5818
	public float airFilter_cnd;

	// Token: 0x040016BB RID: 5819
	public float alternator_cnd;

	// Token: 0x040016BC RID: 5820
	public float battery_cnd;

	// Token: 0x040016BD RID: 5821
	public float batteryC_cnd;

	// Token: 0x040016BE RID: 5822
	public float beltTensioner_cnd;

	// Token: 0x040016BF RID: 5823
	public float beltWheel_cnd;

	// Token: 0x040016C0 RID: 5824
	public float block_cnd;

	// Token: 0x040016C1 RID: 5825
	public float camshaft_cnd;

	// Token: 0x040016C2 RID: 5826
	public float camshaft2_cnd;

	// Token: 0x040016C3 RID: 5827
	public float clutch_cnd;

	// Token: 0x040016C4 RID: 5828
	public float clutchDia_cnd;

	// Token: 0x040016C5 RID: 5829
	public float crank_cnd;

	// Token: 0x040016C6 RID: 5830
	public float crankPulley_cnd;

	// Token: 0x040016C7 RID: 5831
	public float driveRingGearF4_cnd;

	// Token: 0x040016C8 RID: 5832
	public float driveRingGearR4_cnd;

	// Token: 0x040016C9 RID: 5833
	public float driveRingGearF5_cnd;

	// Token: 0x040016CA RID: 5834
	public float driveRingGearR5_cnd;

	// Token: 0x040016CB RID: 5835
	public float exhaustMan_cnd;

	// Token: 0x040016CC RID: 5836
	public float fan_cnd;

	// Token: 0x040016CD RID: 5837
	public float fanBelt_cnd;

	// Token: 0x040016CE RID: 5838
	public float flyWheel_cnd;

	// Token: 0x040016CF RID: 5839
	public float headgasket_cnd;

	// Token: 0x040016D0 RID: 5840
	public float head_cnd;

	// Token: 0x040016D1 RID: 5841
	public float intakeMan_cnd;

	// Token: 0x040016D2 RID: 5842
	public float oilFilter_cnd;

	// Token: 0x040016D3 RID: 5843
	public float oilPan_cnd;

	// Token: 0x040016D4 RID: 5844
	public float piston1_cnd;

	// Token: 0x040016D5 RID: 5845
	public float piston2_cnd;

	// Token: 0x040016D6 RID: 5846
	public float piston3_cnd;

	// Token: 0x040016D7 RID: 5847
	public float piston4_cnd;

	// Token: 0x040016D8 RID: 5848
	public float piston5_cnd;

	// Token: 0x040016D9 RID: 5849
	public float piston6_cnd;

	// Token: 0x040016DA RID: 5850
	public float powerSteeringPump_cnd;

	// Token: 0x040016DB RID: 5851
	public float starterMotor_cnd;

	// Token: 0x040016DC RID: 5852
	public float timingChain_cnd;

	// Token: 0x040016DD RID: 5853
	public float transmission_cnd;

	// Token: 0x040016DE RID: 5854
	public float transferCase_cnd;

	// Token: 0x040016DF RID: 5855
	public float valveCover_cnd;

	// Token: 0x040016E0 RID: 5856
	public float waterPump_cnd;

	// Token: 0x040016E1 RID: 5857
	public float radiator_cnd;

	// Token: 0x040016E2 RID: 5858
	public float sparkPlug1_cnd;

	// Token: 0x040016E3 RID: 5859
	public float sparkPlug2_cnd;

	// Token: 0x040016E4 RID: 5860
	public float sparkPlug3_cnd;

	// Token: 0x040016E5 RID: 5861
	public float sparkPlug4_cnd;

	// Token: 0x040016E6 RID: 5862
	public durability acPump_cnd_c;

	// Token: 0x040016E7 RID: 5863
	public durability airFilter_cnd_c;

	// Token: 0x040016E8 RID: 5864
	public durability alternator_cnd_c;

	// Token: 0x040016E9 RID: 5865
	public durability battery_cnd_c;

	// Token: 0x040016EA RID: 5866
	public durability batteryC_cnd_c;

	// Token: 0x040016EB RID: 5867
	public durability beltTensioner_cnd_c;

	// Token: 0x040016EC RID: 5868
	public durability beltWheel_cnd_c;

	// Token: 0x040016ED RID: 5869
	public durability block_cnd_c;

	// Token: 0x040016EE RID: 5870
	public durability camshaft_cnd_c;

	// Token: 0x040016EF RID: 5871
	public durability camshaft2_cnd_c;

	// Token: 0x040016F0 RID: 5872
	public durability clutch_cnd_c;

	// Token: 0x040016F1 RID: 5873
	public durability clutchDia_cnd_c;

	// Token: 0x040016F2 RID: 5874
	public durability crank_cnd_c;

	// Token: 0x040016F3 RID: 5875
	public durability crankPulley_cnd_c;

	// Token: 0x040016F4 RID: 5876
	public durability driveRingGearF4_cnd_c;

	// Token: 0x040016F5 RID: 5877
	public durability driveRingGearR4_cnd_c;

	// Token: 0x040016F6 RID: 5878
	public durability driveRingGearF5_cnd_c;

	// Token: 0x040016F7 RID: 5879
	public durability driveRingGearR5_cnd_c;

	// Token: 0x040016F8 RID: 5880
	public durability exhaustMan_cnd_c;

	// Token: 0x040016F9 RID: 5881
	public durability fan_cnd_c;

	// Token: 0x040016FA RID: 5882
	public durability fanBelt_cnd_c;

	// Token: 0x040016FB RID: 5883
	public durability flyWheel_cnd_c;

	// Token: 0x040016FC RID: 5884
	public durability headgasket_cnd_c;

	// Token: 0x040016FD RID: 5885
	public durability head_cnd_c;

	// Token: 0x040016FE RID: 5886
	public durability intakeMan_cnd_c;

	// Token: 0x040016FF RID: 5887
	public durability oilFilter_cnd_c;

	// Token: 0x04001700 RID: 5888
	public durability oilPan_cnd_c;

	// Token: 0x04001701 RID: 5889
	public durability piston1_cnd_c;

	// Token: 0x04001702 RID: 5890
	public durability piston2_cnd_c;

	// Token: 0x04001703 RID: 5891
	public durability piston3_cnd_c;

	// Token: 0x04001704 RID: 5892
	public durability piston4_cnd_c;

	// Token: 0x04001705 RID: 5893
	public durability piston5_cnd_c;

	// Token: 0x04001706 RID: 5894
	public durability piston6_cnd_c;

	// Token: 0x04001707 RID: 5895
	public durability powerSteeringPump_cnd_c;

	// Token: 0x04001708 RID: 5896
	public durability radiator_cnd_c;

	// Token: 0x04001709 RID: 5897
	public durability starterMotor_cnd_c;

	// Token: 0x0400170A RID: 5898
	public durability timingChain_cnd_c;

	// Token: 0x0400170B RID: 5899
	public durability transmission_cnd_c;

	// Token: 0x0400170C RID: 5900
	public durability transferCase_cnd_c;

	// Token: 0x0400170D RID: 5901
	public durability valveCover_cnd_c;

	// Token: 0x0400170E RID: 5902
	public durability waterPump_cnd_c;

	// Token: 0x0400170F RID: 5903
	public GameObject exhaustTrails;

	// Token: 0x04001710 RID: 5904
	private float newRust;

	// Token: 0x04001711 RID: 5905
	private GameObject rustPart;

	// Token: 0x04001712 RID: 5906
	public float newOilLevel;

	// Token: 0x04001713 RID: 5907
	public float newCoolantLevel;

	// Token: 0x04001714 RID: 5908
	public float newFuelLevel;

	// Token: 0x04001715 RID: 5909
	public float newTemperature;

	// Token: 0x04001716 RID: 5910
	public float newMaxTorque;

	// Token: 0x04001717 RID: 5911
	public float torqueReduction;

	// Token: 0x04001718 RID: 5912
	public float torqueIncrease;

	// Token: 0x04001719 RID: 5913
	public bool canRun = true;

	// Token: 0x0400171A RID: 5914
	public bool canCrank = true;

	// Token: 0x0400171B RID: 5915
	public bool canAcc = true;

	// Token: 0x0400171C RID: 5916
	public bool canEasyStart = true;

	// Token: 0x0400171D RID: 5917
	public bool canMove = true;

	// Token: 0x0400171E RID: 5918
	public bool can4wd = true;

	// Token: 0x0400171F RID: 5919
	public bool fanSpin = true;

	// Token: 0x04001720 RID: 5920
	public bool checkEngine;

	// Token: 0x04001721 RID: 5921
	public bool beltWhine;

	// Token: 0x04001722 RID: 5922
	public bool knocking;

	// Token: 0x04001723 RID: 5923
	public bool accelLag;

	// Token: 0x04001724 RID: 5924
	public bool whiteSmoke;

	// Token: 0x04001725 RID: 5925
	public bool blackSmoke;

	// Token: 0x04001726 RID: 5926
	public bool roughIdle;

	// Token: 0x04001727 RID: 5927
	public bool fire;

	// Token: 0x04001728 RID: 5928
	public float oilLoss;

	// Token: 0x04001729 RID: 5929
	public bool hasExhaustMan = true;

	// Token: 0x0400172A RID: 5930
	public bool fanClutchSeized;

	// Token: 0x0400172B RID: 5931
	public bool hasTurboD;

	// Token: 0x0400172C RID: 5932
	public bool hasTurboP;

	// Token: 0x0400172D RID: 5933
	private string newPart;

	// Token: 0x0400172E RID: 5934
	private float thisHealth;

	// Token: 0x0400172F RID: 5935
	private int randPart;

	// Token: 0x04001730 RID: 5936
	public float maxTemp;

	// Token: 0x04001731 RID: 5937
	private float fancool;

	// Token: 0x04001732 RID: 5938
	private float batCharge;

	// Token: 0x04001733 RID: 5939
	private float tempIncrease;

	// Token: 0x04001734 RID: 5940
	public bool debugRun;

	// Token: 0x04001735 RID: 5941
	public float avgDmgCab;

	// Token: 0x04001736 RID: 5942
	public float prevDmgCab;

	// Token: 0x04001737 RID: 5943
	public ImpactDeformable idCab;

	// Token: 0x04001738 RID: 5944
	public GameObject[] breakableParts;

	// Token: 0x04001739 RID: 5945
	private int totalLoosened;

	// Token: 0x0400173A RID: 5946
	private float dmgAmount;

	// Token: 0x0400173B RID: 5947
	public Transform ebtemplatei6;

	// Token: 0x0400173C RID: 5948
	public Transform ebtemplatei6Car;

	// Token: 0x0400173D RID: 5949
	public Transform emptyRbi6;

	// Token: 0x0400173E RID: 5950
	public Rigidbody truckRb;

	// Token: 0x0400173F RID: 5951
	public Rigidbody carRb;

	// Token: 0x04001740 RID: 5952
	public int whichTruck;

	// Token: 0x04001741 RID: 5953
	public bool charging;

	// Token: 0x04001742 RID: 5954
	public Material rust;
}
