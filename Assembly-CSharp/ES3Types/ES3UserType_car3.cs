using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x02000308 RID: 776
	[Preserve]
	[ES3Properties(new string[]
	{
		"centerOfMass",
		"steerWheel",
		"steerAngle",
		"cams",
		"color",
		"Cdrag",
		"Fdrag",
		"maxFuel",
		"minFuel",
		"fuel",
		"temperature",
		"maxTemp",
		"coolantLevel",
		"oilLevel",
		"maxTorque",
		"maxTorqueStatic",
		"maxBrakeTorque",
		"MaxWheelRotateAngle",
		"lowestSpeedAtSteer",
		"lowSpeedSteerAngle",
		"highSpeedSteerAngle",
		"torque",
		"frontTorque",
		"rearTorque",
		"maxSpeed",
		"steerWheelAngle",
		"steerWheelRotateFactor",
		"braked",
		"WheelFrontRight",
		"WheelFrontLeft",
		"WheelRearRight",
		"WheelRearLeft",
		"wheelRay",
		"surfaceType",
		"intLight1",
		"wheelFR",
		"wheelFL",
		"wheelRR",
		"wheelRL",
		"deepWater",
		"oilEmissive",
		"tempEmissive",
		"fuelEmissive",
		"ebrakeEmissive",
		"oilIcon",
		"encheckIcon",
		"fuelIcon",
		"ebrakeIcon",
		"checkEngine",
		"enableOil",
		"pointerFuel",
		"engineFan",
		"canRun",
		"canCrank",
		"canAcc",
		"canEasyStart",
		"canMove",
		"jumptime",
		"enginescript",
		"exhaustSmoke",
		"backfireSmoke",
		"doorP",
		"doorD",
		"hood",
		"idScriptDoorP",
		"idScriptDoorD",
		"idScriptHood",
		"waterFlood",
		"audioCtrl",
		"splashFx",
		"gravShell",
		"rb",
		"groundDetect",
		"element",
		"person"
	})]
	public class ES3UserType_car3 : ES3ComponentType
	{
		// Token: 0x0600141D RID: 5149 RVA: 0x000D7C1D File Offset: 0x000D5E1D
		public ES3UserType_car3() : base(typeof(car3))
		{
			ES3UserType_car3.Instance = this;
			this.priority = 1;
		}

		// Token: 0x0600141E RID: 5150 RVA: 0x000D7C3C File Offset: 0x000D5E3C
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			car3 car = (car3)obj;
			writer.WritePropertyByRef("centerOfMass", car.centerOfMass);
			writer.WritePropertyByRef("steerWheel", car.steerWheel);
			writer.WriteProperty("steerAngle", car.steerAngle, ES3Type_float.Instance);
			writer.WriteProperty("cams", car.cams, ES3Type_GameObjectArray.Instance);
			writer.WriteProperty("color", car.color, ES3Type_Color.Instance);
			writer.WriteProperty("Cdrag", car.Cdrag, ES3Type_float.Instance);
			writer.WriteProperty("Fdrag", car.Fdrag, ES3Type_float.Instance);
			writer.WriteProperty("maxFuel", car.maxFuel, ES3Type_float.Instance);
			writer.WriteProperty("minFuel", car.minFuel, ES3Type_float.Instance);
			writer.WriteProperty("fuel", car.fuel, ES3Type_float.Instance);
			writer.WriteProperty("temperature", car.temperature, ES3Type_float.Instance);
			writer.WriteProperty("maxTemp", car.maxTemp, ES3Type_float.Instance);
			writer.WriteProperty("coolantLevel", car.coolantLevel, ES3Type_float.Instance);
			writer.WriteProperty("oilLevel", car.oilLevel, ES3Type_float.Instance);
			writer.WriteProperty("maxTorque", car.maxTorque, ES3Type_float.Instance);
			writer.WriteProperty("maxTorqueStatic", car.maxTorqueStatic, ES3Type_float.Instance);
			writer.WriteProperty("maxBrakeTorque", car.maxBrakeTorque, ES3Type_float.Instance);
			writer.WriteProperty("MaxWheelRotateAngle", car.MaxWheelRotateAngle, ES3Type_float.Instance);
			writer.WriteProperty("lowestSpeedAtSteer", car.lowestSpeedAtSteer, ES3Type_float.Instance);
			writer.WriteProperty("lowSpeedSteerAngle", car.lowSpeedSteerAngle, ES3Type_float.Instance);
			writer.WriteProperty("highSpeedSteerAngle", car.highSpeedSteerAngle, ES3Type_float.Instance);
			writer.WriteProperty("torque", car.torque, ES3Type_float.Instance);
			writer.WriteProperty("frontTorque", car.frontTorque, ES3Type_float.Instance);
			writer.WriteProperty("rearTorque", car.rearTorque, ES3Type_float.Instance);
			writer.WriteProperty("maxSpeed", car.maxSpeed, ES3Type_float.Instance);
			writer.WriteProperty("steerWheelAngle", car.steerWheelAngle, ES3Type_float.Instance);
			writer.WriteProperty("steerWheelRotateFactor", car.steerWheelRotateFactor, ES3Type_float.Instance);
			writer.WriteProperty("braked", car.braked, ES3Type_bool.Instance);
			writer.WritePropertyByRef("WheelFrontRight", car.WheelFrontRight);
			writer.WritePropertyByRef("WheelFrontLeft", car.WheelFrontLeft);
			writer.WritePropertyByRef("WheelRearRight", car.WheelRearRight);
			writer.WritePropertyByRef("WheelRearLeft", car.WheelRearLeft);
			writer.WritePropertyByRef("wheelRay", car.wheelRay);
			writer.WriteProperty("surfaceType", car.surfaceType, ES3Type_float.Instance);
			writer.WritePropertyByRef("intLight1", car.intLight1);
			writer.WritePropertyByRef("wheelFR", car.wheelFR);
			writer.WritePropertyByRef("wheelFL", car.wheelFL);
			writer.WritePropertyByRef("wheelRR", car.wheelRR);
			writer.WritePropertyByRef("wheelRL", car.wheelRL);
			writer.WriteProperty("deepWater", car.deepWater, ES3Type_int.Instance);
			writer.WriteProperty("oilEmissive", car.oilEmissive, ES3Type_float.Instance);
			writer.WriteProperty("tempEmissive", car.tempEmissive, ES3Type_float.Instance);
			writer.WriteProperty("fuelEmissive", car.fuelEmissive, ES3Type_float.Instance);
			writer.WriteProperty("ebrakeEmissive", car.ebrakeEmissive, ES3Type_float.Instance);
			writer.WritePropertyByRef("oilIcon", car.oilIcon);
			writer.WritePropertyByRef("encheckIcon", car.encheckIcon);
			writer.WritePropertyByRef("fuelIcon", car.fuelIcon);
			writer.WritePropertyByRef("ebrakeIcon", car.ebrakeIcon);
			writer.WriteProperty("checkEngine", car.checkEngine, ES3Type_bool.Instance);
			writer.WriteProperty("enableOil", car.enableOil, ES3Type_bool.Instance);
			writer.WritePropertyByRef("pointerFuel", car.pointerFuel);
			writer.WritePropertyByRef("engineFan", car.engineFan);
			writer.WriteProperty("canRun", car.canRun, ES3Type_bool.Instance);
			writer.WriteProperty("canCrank", car.canCrank, ES3Type_bool.Instance);
			writer.WriteProperty("canAcc", car.canAcc, ES3Type_bool.Instance);
			writer.WriteProperty("canEasyStart", car.canEasyStart, ES3Type_bool.Instance);
			writer.WriteProperty("canMove", car.canMove, ES3Type_bool.Instance);
			writer.WriteProperty("jumptime", car.jumptime, ES3Type_float.Instance);
			writer.WritePropertyByRef("enginescript", car.enginescript);
			writer.WritePropertyByRef("exhaustSmoke", car.exhaustSmoke);
			writer.WritePropertyByRef("backfireSmoke", car.backfireSmoke);
			writer.WritePropertyByRef("doorP", car.doorP);
			writer.WritePropertyByRef("doorD", car.doorD);
			writer.WritePropertyByRef("hood", car.hood);
			writer.WritePropertyByRef("idScriptDoorP", car.idScriptDoorP);
			writer.WritePropertyByRef("idScriptDoorD", car.idScriptDoorD);
			writer.WritePropertyByRef("idScriptHood", car.idScriptHood);
			writer.WriteProperty("waterFlood", car.waterFlood, ES3Type_int.Instance);
			writer.WritePropertyByRef("audioCtrl", car.audioCtrl);
			writer.WriteProperty("splashFx", car.splashFx, ES3Type_GameObjectArray.Instance);
			writer.WritePropertyByRef("gravShell", car.gravShell);
			writer.WritePropertyByRef("rb", car.rb);
			writer.WritePropertyByRef("groundDetect", car.groundDetect);
			writer.WritePropertyByRef("element", car.element);
			writer.WritePropertyByRef("person", car.person);
		}

		// Token: 0x0600141F RID: 5151 RVA: 0x000D82E8 File Offset: 0x000D64E8
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			car3 car = (car3)obj;
			foreach (object obj2 in reader.Properties)
			{
				string text = (string)obj2;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 2375241169U)
				{
					if (num <= 1004529100U)
					{
						if (num <= 323965700U)
						{
							if (num <= 98103033U)
							{
								if (num <= 76942167U)
								{
									if (num != 61176541U)
									{
										if (num == 76942167U)
										{
											if (text == "maxTorque")
											{
												car.maxTorque = reader.Read<float>(ES3Type_float.Instance);
												continue;
											}
										}
									}
									else if (text == "maxTorqueStatic")
									{
										car.maxTorqueStatic = reader.Read<float>(ES3Type_float.Instance);
										continue;
									}
								}
								else if (num != 90731405U)
								{
									if (num == 98103033U)
									{
										if (text == "canEasyStart")
										{
											car.canEasyStart = reader.Read<bool>(ES3Type_bool.Instance);
											continue;
										}
									}
								}
								else if (text == "fuel")
								{
									car.fuel = reader.Read<float>(ES3Type_float.Instance);
									continue;
								}
							}
							else if (num <= 159966952U)
							{
								if (num != 106106960U)
								{
									if (num == 159966952U)
									{
										if (text == "fuelIcon")
										{
											car.fuelIcon = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "WheelRearRight")
								{
									car.WheelRearRight = reader.Read<WheelCollider>();
									continue;
								}
							}
							else if (num != 207190873U)
							{
								if (num != 290208940U)
								{
									if (num == 323965700U)
									{
										if (text == "maxSpeed")
										{
											car.maxSpeed = reader.Read<float>(ES3Type_float.Instance);
											continue;
										}
									}
								}
								else if (text == "enginescript")
								{
									car.enginescript = reader.Read<engine>();
									continue;
								}
							}
							else if (text == "groundDetect")
							{
								car.groundDetect = reader.Read<GroundDetect>();
								continue;
							}
						}
						else if (num <= 798595260U)
						{
							if (num <= 740176158U)
							{
								if (num != 440906180U)
								{
									if (num == 740176158U)
									{
										if (text == "MaxWheelRotateAngle")
										{
											car.MaxWheelRotateAngle = reader.Read<float>(ES3Type_float.Instance);
											continue;
										}
									}
								}
								else if (text == "highSpeedSteerAngle")
								{
									car.highSpeedSteerAngle = reader.Read<float>(ES3Type_float.Instance);
									continue;
								}
							}
							else if (num != 771954031U)
							{
								if (num == 798595260U)
								{
									if (text == "waterFlood")
									{
										car.waterFlood = reader.Read<int>(ES3Type_int.Instance);
										continue;
									}
								}
							}
							else if (text == "backfireSmoke")
							{
								car.backfireSmoke = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num <= 851604344U)
						{
							if (num != 817708776U)
							{
								if (num == 851604344U)
								{
									if (text == "wheelRay")
									{
										car.wheelRay = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "lowestSpeedAtSteer")
							{
								car.lowestSpeedAtSteer = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
						else if (num != 947485984U)
						{
							if (num != 959407159U)
							{
								if (num == 1004529100U)
								{
									if (text == "jumptime")
									{
										car.jumptime = reader.Read<float>(ES3Type_float.Instance);
										continue;
									}
								}
							}
							else if (text == "Fdrag")
							{
								car.Fdrag = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
						else if (text == "oilEmissive")
						{
							car.oilEmissive = reader.Read<float>(ES3Type_float.Instance);
							continue;
						}
					}
					else if (num <= 1479322555U)
					{
						if (num <= 1298130101U)
						{
							if (num <= 1109450902U)
							{
								if (num != 1031692888U)
								{
									if (num == 1109450902U)
									{
										if (text == "frontTorque")
										{
											car.frontTorque = reader.Read<float>(ES3Type_float.Instance);
											continue;
										}
									}
								}
								else if (text == "color")
								{
									car.color = reader.Read<Color>(ES3Type_Color.Instance);
									continue;
								}
							}
							else if (num != 1195336803U)
							{
								if (num == 1298130101U)
								{
									if (text == "coolantLevel")
									{
										car.coolantLevel = reader.Read<float>(ES3Type_float.Instance);
										continue;
									}
								}
							}
							else if (text == "torque")
							{
								car.torque = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
						else if (num <= 1330461687U)
						{
							if (num != 1308802079U)
							{
								if (num == 1330461687U)
								{
									if (text == "element")
									{
										car.element = reader.Read<Renderer>();
										continue;
									}
								}
							}
							else if (text == "WheelRearLeft")
							{
								car.WheelRearLeft = reader.Read<WheelCollider>();
								continue;
							}
						}
						else if (num != 1413139572U)
						{
							if (num != 1430818598U)
							{
								if (num == 1479322555U)
								{
									if (text == "oilLevel")
									{
										car.oilLevel = reader.Read<float>(ES3Type_float.Instance);
										continue;
									}
								}
							}
							else if (text == "exhaustSmoke")
							{
								car.exhaustSmoke = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (text == "surfaceType")
						{
							car.surfaceType = reader.Read<float>(ES3Type_float.Instance);
							continue;
						}
					}
					else if (num <= 1923458214U)
					{
						if (num <= 1514965763U)
						{
							if (num != 1503767242U)
							{
								if (num == 1514965763U)
								{
									if (text == "maxTemp")
									{
										car.maxTemp = reader.Read<float>(ES3Type_float.Instance);
										continue;
									}
								}
							}
							else if (text == "oilIcon")
							{
								car.oilIcon = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num != 1599373397U)
						{
							if (num != 1833050408U)
							{
								if (num == 1923458214U)
								{
									if (text == "wheelFL")
									{
										car.wheelFL = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "audioCtrl")
							{
								car.audioCtrl = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (text == "rb")
						{
							car.rb = reader.Read<Rigidbody>(ES3UserType_Rigidbody.Instance);
							continue;
						}
					}
					else if (num <= 2284576574U)
					{
						if (num != 2222759721U)
						{
							if (num == 2284576574U)
							{
								if (text == "deepWater")
								{
									car.deepWater = reader.Read<int>(ES3Type_int.Instance);
									continue;
								}
							}
						}
						else if (text == "intLight1")
						{
							car.intLight1 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num != 2354326850U)
					{
						if (num != 2359676308U)
						{
							if (num == 2375241169U)
							{
								if (text == "idScriptHood")
								{
									car.idScriptHood = reader.Read<ImpactDeformable>();
									continue;
								}
							}
						}
						else if (text == "wheelFR")
						{
							car.wheelFR = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (text == "maxBrakeTorque")
					{
						car.maxBrakeTorque = reader.Read<float>(ES3Type_float.Instance);
						continue;
					}
				}
				else if (num <= 3324546841U)
				{
					if (num <= 2901359373U)
					{
						if (num <= 2618419572U)
						{
							if (num <= 2559430583U)
							{
								if (num != 2544129665U)
								{
									if (num == 2559430583U)
									{
										if (text == "encheckIcon")
										{
											car.encheckIcon = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "maxFuel")
								{
									car.maxFuel = reader.Read<float>(ES3Type_float.Instance);
									continue;
								}
							}
							else if (num != 2560268759U)
							{
								if (num == 2618419572U)
								{
									if (text == "engineFan")
									{
										car.engineFan = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "gravShell")
							{
								car.gravShell = reader.Read<CarTrunkGrav>();
								continue;
							}
						}
						else if (num <= 2629291871U)
						{
							if (num != 2619080378U)
							{
								if (num == 2629291871U)
								{
									if (text == "WheelFrontRight")
									{
										car.WheelFrontRight = reader.Read<WheelCollider>();
										continue;
									}
								}
							}
							else if (text == "pointerFuel")
							{
								car.pointerFuel = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num != 2681007987U)
						{
							if (num != 2859943884U)
							{
								if (num == 2901359373U)
								{
									if (text == "cams")
									{
										car.cams = reader.Read<GameObject[]>(ES3Type_GameObjectArray.Instance);
										continue;
									}
								}
							}
							else if (text == "tempEmissive")
							{
								car.tempEmissive = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
						else if (text == "hood")
						{
							car.hood = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num <= 3156625349U)
					{
						if (num <= 2995254354U)
						{
							if (num != 2985344196U)
							{
								if (num == 2995254354U)
								{
									if (text == "wheelRL")
									{
										car.wheelRL = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "canCrank")
							{
								car.canCrank = reader.Read<bool>(ES3Type_bool.Instance);
								continue;
							}
						}
						else if (num != 3077523073U)
						{
							if (num != 3095373512U)
							{
								if (num == 3156625349U)
								{
									if (text == "steerWheel")
									{
										car.steerWheel = reader.Read<Transform>(ES3UserType_Transform.Instance);
										continue;
									}
								}
							}
							else if (text == "splashFx")
							{
								car.splashFx = reader.Read<GameObject[]>(ES3Type_GameObjectArray.Instance);
								continue;
							}
						}
						else if (text == "centerOfMass")
						{
							car.centerOfMass = reader.Read<Transform>(ES3UserType_Transform.Instance);
							continue;
						}
					}
					else if (num <= 3235317062U)
					{
						if (num != 3224526154U)
						{
							if (num == 3235317062U)
							{
								if (text == "enableOil")
								{
									car.enableOil = reader.Read<bool>(ES3Type_bool.Instance);
									continue;
								}
							}
						}
						else if (text == "canRun")
						{
							car.canRun = reader.Read<bool>(ES3Type_bool.Instance);
							continue;
						}
					}
					else if (num != 3297251496U)
					{
						if (num != 3300011627U)
						{
							if (num == 3324546841U)
							{
								if (text == "checkEngine")
								{
									car.checkEngine = reader.Read<bool>(ES3Type_bool.Instance);
									continue;
								}
							}
						}
						else if (text == "minFuel")
						{
							car.minFuel = reader.Read<float>(ES3Type_float.Instance);
							continue;
						}
					}
					else if (text == "wheelRR")
					{
						car.wheelRR = reader.Read<GameObject>(ES3Type_GameObject.Instance);
						continue;
					}
				}
				else if (num <= 3686661906U)
				{
					if (num <= 3549934208U)
					{
						if (num <= 3328234388U)
						{
							if (num != 3326546360U)
							{
								if (num == 3328234388U)
								{
									if (text == "canMove")
									{
										car.canMove = reader.Read<bool>(ES3Type_bool.Instance);
										continue;
									}
								}
							}
							else if (text == "steerWheelAngle")
							{
								car.steerWheelAngle = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
						else if (num != 3357481341U)
						{
							if (num == 3549934208U)
							{
								if (text == "person")
								{
									car.person = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
						}
						else if (text == "steerWheelRotateFactor")
						{
							car.steerWheelRotateFactor = reader.Read<float>(ES3Type_float.Instance);
							continue;
						}
					}
					else if (num <= 3621747277U)
					{
						if (num != 3618600218U)
						{
							if (num == 3621747277U)
							{
								if (text == "steerAngle")
								{
									car.steerAngle = reader.Read<float>(ES3Type_float.Instance);
									continue;
								}
							}
						}
						else if (text == "canAcc")
						{
							car.canAcc = reader.Read<bool>(ES3Type_bool.Instance);
							continue;
						}
					}
					else if (num != 3644230834U)
					{
						if (num != 3682917512U)
						{
							if (num == 3686661906U)
							{
								if (text == "ebrakeIcon")
								{
									car.ebrakeIcon = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
						}
						else if (text == "ebrakeEmissive")
						{
							car.ebrakeEmissive = reader.Read<float>(ES3Type_float.Instance);
							continue;
						}
					}
					else if (text == "WheelFrontLeft")
					{
						car.WheelFrontLeft = reader.Read<WheelCollider>();
						continue;
					}
				}
				else if (num <= 3927255045U)
				{
					if (num <= 3805266071U)
					{
						if (num != 3690043878U)
						{
							if (num == 3805266071U)
							{
								if (text == "doorD")
								{
									car.doorD = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
						}
						else if (text == "fuelEmissive")
						{
							car.fuelEmissive = reader.Read<float>(ES3Type_float.Instance);
							continue;
						}
					}
					else if (num != 3810726972U)
					{
						if (num != 3924994357U)
						{
							if (num == 3927255045U)
							{
								if (text == "idScriptDoorD")
								{
									car.idScriptDoorD = reader.Read<ImpactDeformable>();
									continue;
								}
							}
						}
						else if (text == "temperature")
						{
							car.temperature = reader.Read<float>(ES3Type_float.Instance);
							continue;
						}
					}
					else if (text == "braked")
					{
						car.braked = reader.Read<bool>(ES3Type_bool.Instance);
						continue;
					}
				}
				else if (num <= 4035582048U)
				{
					if (num != 4006597499U)
					{
						if (num == 4035582048U)
						{
							if (text == "lowSpeedSteerAngle")
							{
								car.lowSpeedSteerAngle = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
					}
					else if (text == "doorP")
					{
						car.doorP = reader.Read<GameObject>(ES3Type_GameObject.Instance);
						continue;
					}
				}
				else if (num != 4069004972U)
				{
					if (num != 4128586473U)
					{
						if (num == 4223114689U)
						{
							if (text == "rearTorque")
							{
								car.rearTorque = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
					}
					else if (text == "idScriptDoorP")
					{
						car.idScriptDoorP = reader.Read<ImpactDeformable>();
						continue;
					}
				}
				else if (text == "Cdrag")
				{
					car.Cdrag = reader.Read<float>(ES3Type_float.Instance);
					continue;
				}
				reader.Skip();
			}
		}

		// Token: 0x04002474 RID: 9332
		public static ES3Type Instance;
	}
}
