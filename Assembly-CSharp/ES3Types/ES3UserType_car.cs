using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x02000306 RID: 774
	[Preserve]
	[ES3Properties(new string[]
	{
		"fourWheelDrive",
		"fourcyl",
		"person",
		"centerOfMass",
		"steerWheel",
		"theDest",
		"steerAngle",
		"cams",
		"interactor",
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
		"rockerSpeed",
		"fanSpin",
		"WheelFrontRight",
		"WheelFrontLeft",
		"WheelRearRight",
		"WheelRearLeft",
		"gasPedal",
		"wheelRay",
		"surfaceType",
		"intLight1",
		"intLight2",
		"intLight3",
		"intLight4",
		"wheelFR",
		"wheelFL",
		"wheelRR",
		"wheelRL",
		"rotorFL",
		"frontGear",
		"rearGear",
		"transmission",
		"speed",
		"wd4Emissive",
		"oilEmissive",
		"encheckEmissive",
		"tempEmissive",
		"fuelEmissive",
		"chgEmissive",
		"ebrakeEmissive",
		"wd4Icon",
		"oilIcon",
		"encheckIcon",
		"tempIcon",
		"fuelIcon",
		"chgIcon",
		"ebrakeIcon",
		"checkEngine",
		"enableWd4",
		"enableOil",
		"pointerTemp",
		"pointerFuel",
		"engineBlock",
		"engineFan",
		"camAndGear",
		"helical30r",
		"engineCrank",
		"rocker1",
		"rocker2",
		"rocker3",
		"rocker4",
		"rocker5",
		"rocker6",
		"rocker7",
		"rocker8",
		"camBearing1",
		"camBearing2",
		"camBearing3",
		"engineBlockV8",
		"crankV8",
		"altFan",
		"timingGear",
		"v8Fan",
		"canRun",
		"canCrank",
		"canAcc",
		"canEasyStart",
		"canMove",
		"mudBrushRL",
		"mudBrushRR",
		"mudBrushFL",
		"mudBrushFR",
		"jumptime",
		"heatTrails",
		"exhaustTrails",
		"steamTrails",
		"steamTrails2",
		"steamTrailsOn",
		"steamTrailsOn2",
		"steamTrailsV8",
		"steamTrails2V8",
		"steamTrailsOnV8",
		"steamTrailsOn2V8",
		"heatTrailsOn",
		"exhaustTrailsOn",
		"enginescript",
		"enginescriptv8",
		"groundDetect",
		"deepMud",
		"waterFlood",
		"jacked",
		"gravShell",
		"element",
		"rb",
		"clutchIn",
		"usingV8",
		"RRholder",
		"RLholder",
		"FRholder",
		"FLholder",
		"RRpos",
		"RLpos",
		"FRpos",
		"FLpos",
		"brackets4",
		"brackets8",
		"isDigital",
		"fastStart",
		"fastStartV8",
		"cleanInterior",
		"fuelSaving",
		"soundMat",
		"hasSeat",
		"hasDiffLock",
		"has5th",
		"hasRollbar",
		"rgbR",
		"rgbB",
		"rgbG",
		"audioControl",
		"deepWater",
		"splashFx",
		"hangTimeChallenge",
		"additive",
		"additiveBonus",
		"boostGauge"
	})]
	public class ES3UserType_car : ES3ComponentType
	{
		// Token: 0x06001418 RID: 5144 RVA: 0x000D4965 File Offset: 0x000D2B65
		public ES3UserType_car() : base(typeof(car))
		{
			ES3UserType_car.Instance = this;
			this.priority = 1;
		}

		// Token: 0x06001419 RID: 5145 RVA: 0x000D4984 File Offset: 0x000D2B84
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			car car = (car)obj;
			writer.WriteProperty("fourWheelDrive", car.fourWheelDrive, ES3Type_bool.Instance);
			writer.WritePropertyByRef("fourcyl", car.fourcyl);
			writer.WritePropertyByRef("person", car.person);
			writer.WritePropertyByRef("centerOfMass", car.centerOfMass);
			writer.WritePropertyByRef("steerWheel", car.steerWheel);
			writer.WritePropertyByRef("theDest", car.theDest);
			writer.WriteProperty("steerAngle", car.steerAngle, ES3Type_float.Instance);
			writer.WriteProperty("cams", car.cams, ES3Type_GameObjectArray.Instance);
			writer.WritePropertyByRef("interactor", car.interactor);
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
			writer.WriteProperty("rockerSpeed", car.rockerSpeed, ES3Type_float.Instance);
			writer.WriteProperty("fanSpin", car.fanSpin, ES3Type_bool.Instance);
			writer.WritePropertyByRef("WheelFrontRight", car.WheelFrontRight);
			writer.WritePropertyByRef("WheelFrontLeft", car.WheelFrontLeft);
			writer.WritePropertyByRef("WheelRearRight", car.WheelRearRight);
			writer.WritePropertyByRef("WheelRearLeft", car.WheelRearLeft);
			writer.WritePropertyByRef("gasPedal", car.gasPedal);
			writer.WritePropertyByRef("wheelRay", car.wheelRay);
			writer.WriteProperty("surfaceType", car.surfaceType, ES3Type_int.Instance);
			writer.WritePropertyByRef("intLight1", car.intLight1);
			writer.WritePropertyByRef("intLight2", car.intLight2);
			writer.WritePropertyByRef("intLight3", car.intLight3);
			writer.WritePropertyByRef("intLight4", car.intLight4);
			writer.WritePropertyByRef("wheelFR", car.wheelFR);
			writer.WritePropertyByRef("wheelFL", car.wheelFL);
			writer.WritePropertyByRef("wheelRR", car.wheelRR);
			writer.WritePropertyByRef("wheelRL", car.wheelRL);
			writer.WritePropertyByRef("rotorFL", car.rotorFL);
			writer.WriteProperty("frontGear", car.frontGear, ES3Type_int.Instance);
			writer.WriteProperty("rearGear", car.rearGear, ES3Type_int.Instance);
			writer.WriteProperty("transmission", car.transmission, ES3Type_int.Instance);
			writer.WriteProperty("speed", car.speed, ES3Type_float.Instance);
			writer.WriteProperty("wd4Emissive", car.wd4Emissive, ES3Type_float.Instance);
			writer.WriteProperty("oilEmissive", car.oilEmissive, ES3Type_float.Instance);
			writer.WriteProperty("encheckEmissive", car.encheckEmissive, ES3Type_float.Instance);
			writer.WriteProperty("tempEmissive", car.tempEmissive, ES3Type_float.Instance);
			writer.WriteProperty("fuelEmissive", car.fuelEmissive, ES3Type_float.Instance);
			writer.WriteProperty("chgEmissive", car.chgEmissive, ES3Type_float.Instance);
			writer.WriteProperty("ebrakeEmissive", car.ebrakeEmissive, ES3Type_float.Instance);
			writer.WritePropertyByRef("wd4Icon", car.wd4Icon);
			writer.WritePropertyByRef("oilIcon", car.oilIcon);
			writer.WritePropertyByRef("encheckIcon", car.encheckIcon);
			writer.WritePropertyByRef("tempIcon", car.tempIcon);
			writer.WritePropertyByRef("fuelIcon", car.fuelIcon);
			writer.WritePropertyByRef("chgIcon", car.chgIcon);
			writer.WritePropertyByRef("ebrakeIcon", car.ebrakeIcon);
			writer.WriteProperty("checkEngine", car.checkEngine, ES3Type_bool.Instance);
			writer.WriteProperty("enableWd4", car.enableWd4, ES3Type_bool.Instance);
			writer.WriteProperty("enableOil", car.enableOil, ES3Type_bool.Instance);
			writer.WritePropertyByRef("pointerTemp", car.pointerTemp);
			writer.WritePropertyByRef("pointerFuel", car.pointerFuel);
			writer.WritePropertyByRef("engineBlock", car.engineBlock);
			writer.WritePropertyByRef("engineFan", car.engineFan);
			writer.WritePropertyByRef("camAndGear", car.camAndGear);
			writer.WritePropertyByRef("helical30r", car.helical30r);
			writer.WritePropertyByRef("engineCrank", car.engineCrank);
			writer.WritePropertyByRef("rocker1", car.rocker1);
			writer.WritePropertyByRef("rocker2", car.rocker2);
			writer.WritePropertyByRef("rocker3", car.rocker3);
			writer.WritePropertyByRef("rocker4", car.rocker4);
			writer.WritePropertyByRef("rocker5", car.rocker5);
			writer.WritePropertyByRef("rocker6", car.rocker6);
			writer.WritePropertyByRef("rocker7", car.rocker7);
			writer.WritePropertyByRef("rocker8", car.rocker8);
			writer.WritePropertyByRef("camBearing1", car.camBearing1);
			writer.WritePropertyByRef("camBearing2", car.camBearing2);
			writer.WritePropertyByRef("camBearing3", car.camBearing3);
			writer.WritePropertyByRef("engineBlockV8", car.engineBlockV8);
			writer.WritePropertyByRef("crankV8", car.crankV8);
			writer.WritePropertyByRef("altFan", car.altFan);
			writer.WritePropertyByRef("timingGear", car.timingGear);
			writer.WritePropertyByRef("v8Fan", car.v8Fan);
			writer.WriteProperty("canRun", car.canRun, ES3Type_bool.Instance);
			writer.WriteProperty("canCrank", car.canCrank, ES3Type_bool.Instance);
			writer.WriteProperty("canAcc", car.canAcc, ES3Type_bool.Instance);
			writer.WriteProperty("canEasyStart", car.canEasyStart, ES3Type_bool.Instance);
			writer.WriteProperty("canMove", car.canMove, ES3Type_bool.Instance);
			writer.WritePropertyByRef("mudBrushRL", car.mudBrushRL);
			writer.WritePropertyByRef("mudBrushRR", car.mudBrushRR);
			writer.WritePropertyByRef("mudBrushFL", car.mudBrushFL);
			writer.WritePropertyByRef("mudBrushFR", car.mudBrushFR);
			writer.WriteProperty("jumptime", car.jumptime, ES3Type_float.Instance);
			writer.WritePropertyByRef("heatTrails", car.heatTrails);
			writer.WritePropertyByRef("exhaustTrails", car.exhaustTrails);
			writer.WritePropertyByRef("steamTrails", car.steamTrails);
			writer.WritePropertyByRef("steamTrails2", car.steamTrails2);
			writer.WriteProperty("steamTrailsOn", car.steamTrailsOn, ES3Type_bool.Instance);
			writer.WriteProperty("steamTrailsOn2", car.steamTrailsOn2, ES3Type_bool.Instance);
			writer.WritePropertyByRef("steamTrailsV8", car.steamTrailsV8);
			writer.WritePropertyByRef("steamTrails2V8", car.steamTrails2V8);
			writer.WriteProperty("steamTrailsOnV8", car.steamTrailsOnV8, ES3Type_bool.Instance);
			writer.WriteProperty("steamTrailsOn2V8", car.steamTrailsOn2V8, ES3Type_bool.Instance);
			writer.WriteProperty("heatTrailsOn", car.heatTrailsOn, ES3Type_bool.Instance);
			writer.WriteProperty("exhaustTrailsOn", car.exhaustTrailsOn, ES3Type_bool.Instance);
			writer.WritePropertyByRef("enginescript", car.enginescript);
			writer.WritePropertyByRef("enginescriptv8", car.enginescriptv8);
			writer.WritePropertyByRef("groundDetect", car.groundDetect);
			writer.WriteProperty("deepMud", car.deepMud, ES3Type_int.Instance);
			writer.WriteProperty("waterFlood", car.waterFlood, ES3Type_int.Instance);
			writer.WriteProperty("jacked", car.jacked, ES3Type_bool.Instance);
			writer.WritePropertyByRef("gravShell", car.gravShell);
			writer.WritePropertyByRef("element", car.element);
			writer.WritePropertyByRef("rb", car.rb);
			writer.WriteProperty("clutchIn", car.clutchIn, ES3Type_bool.Instance);
			writer.WriteProperty("usingV8", car.usingV8, ES3Type_bool.Instance);
			writer.WritePropertyByRef("RRholder", car.RRholder);
			writer.WritePropertyByRef("RLholder", car.RLholder);
			writer.WritePropertyByRef("FRholder", car.FRholder);
			writer.WritePropertyByRef("FLholder", car.FLholder);
			writer.WritePropertyByRef("RRpos", car.RRpos);
			writer.WritePropertyByRef("RLpos", car.RLpos);
			writer.WritePropertyByRef("FRpos", car.FRpos);
			writer.WritePropertyByRef("FLpos", car.FLpos);
			writer.WritePropertyByRef("brackets4", car.brackets4);
			writer.WritePropertyByRef("brackets8", car.brackets8);
			writer.WriteProperty("isDigital", car.isDigital, ES3Type_bool.Instance);
			writer.WriteProperty("fastStart", car.fastStart, ES3Type_bool.Instance);
			writer.WriteProperty("fastStartV8", car.fastStartV8, ES3Type_bool.Instance);
			writer.WriteProperty("cleanInterior", car.cleanInterior, ES3Type_bool.Instance);
			writer.WriteProperty("fuelSaving", car.fuelSaving, ES3Type_bool.Instance);
			writer.WriteProperty("soundMat", car.soundMat, ES3Type_bool.Instance);
			writer.WriteProperty("hasSeat", car.hasSeat, ES3Type_bool.Instance);
			writer.WriteProperty("hasDiffLock", car.hasDiffLock, ES3Type_bool.Instance);
			writer.WriteProperty("has5th", car.has5th, ES3Type_bool.Instance);
			writer.WriteProperty("hasRollbar", car.hasRollbar, ES3Type_bool.Instance);
			writer.WriteProperty("rgbR", car.rgbR, ES3Type_float.Instance);
			writer.WriteProperty("rgbB", car.rgbB, ES3Type_float.Instance);
			writer.WriteProperty("rgbG", car.rgbG, ES3Type_float.Instance);
			writer.WritePropertyByRef("audioControl", car.audioControl);
			writer.WriteProperty("deepWater", car.deepWater, ES3Type_int.Instance);
			writer.WriteProperty("splashFx", car.splashFx, ES3Type_GameObjectArray.Instance);
			writer.WritePrivateField("hangTimeChallenge", car);
			writer.WriteProperty("additive", car.additive, ES3Type_float.Instance);
			writer.WritePrivateField("additiveBonus", car);
			writer.WritePropertyByRef("boostGauge", car.boostGauge);
		}

		// Token: 0x0600141A RID: 5146 RVA: 0x000D5704 File Offset: 0x000D3904
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			car car = (car)obj;
			foreach (object obj2 in reader.Properties)
			{
				string text = (string)obj2;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 2356340713U)
				{
					if (num <= 959407159U)
					{
						if (num <= 547551059U)
						{
							if (num <= 106106960U)
							{
								if (num <= 51254718U)
								{
									if (num <= 20189451U)
									{
										if (num != 916779U)
										{
											if (num == 20189451U)
											{
												if (text == "frontGear")
												{
													car.frontGear = reader.Read<int>(ES3Type_int.Instance);
													continue;
												}
											}
										}
										else if (text == "jacked")
										{
											car.jacked = reader.Read<bool>(ES3Type_bool.Instance);
											continue;
										}
									}
									else if (num != 30885729U)
									{
										if (num == 51254718U)
										{
											if (text == "heatTrails")
											{
												car.heatTrails = reader.Read<GameObject>(ES3Type_GameObject.Instance);
												continue;
											}
										}
									}
									else if (text == "usingV8")
									{
										car.usingV8 = reader.Read<bool>(ES3Type_bool.Instance);
										continue;
									}
								}
								else if (num <= 76942167U)
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
									if (num != 98103033U)
									{
										if (num == 106106960U)
										{
											if (text == "WheelRearRight")
											{
												car.WheelRearRight = reader.Read<WheelCollider>();
												continue;
											}
										}
									}
									else if (text == "canEasyStart")
									{
										car.canEasyStart = reader.Read<bool>(ES3Type_bool.Instance);
										continue;
									}
								}
								else if (text == "fuel")
								{
									car.fuel = reader.Read<float>(ES3Type_float.Instance);
									continue;
								}
							}
							else if (num <= 290208940U)
							{
								if (num <= 127041765U)
								{
									if (num != 111583397U)
									{
										if (num == 127041765U)
										{
											if (text == "transmission")
											{
												car.transmission = reader.Read<int>(ES3Type_int.Instance);
												continue;
											}
										}
									}
									else if (text == "additive")
									{
										car.additive = reader.Read<float>(ES3Type_float.Instance);
										continue;
									}
								}
								else if (num != 159966952U)
								{
									if (num != 207190873U)
									{
										if (num == 290208940U)
										{
											if (text == "enginescript")
											{
												car.enginescript = reader.Read<engine>();
												continue;
											}
										}
									}
									else if (text == "groundDetect")
									{
										car.groundDetect = reader.Read<GroundDetect>();
										continue;
									}
								}
								else if (text == "fuelIcon")
								{
									car.fuelIcon = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (num <= 436375575U)
							{
								if (num != 323965700U)
								{
									if (num == 436375575U)
									{
										if (text == "deepMud")
										{
											car.deepMud = reader.Read<int>(ES3Type_int.Instance);
											continue;
										}
									}
								}
								else if (text == "maxSpeed")
								{
									car.maxSpeed = reader.Read<float>(ES3Type_float.Instance);
									continue;
								}
							}
							else if (num != 440906180U)
							{
								if (num != 536594618U)
								{
									if (num == 547551059U)
									{
										if (text == "wd4Icon")
										{
											car.wd4Icon = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "fanSpin")
								{
									car.fanSpin = reader.Read<bool>(ES3Type_bool.Instance);
									continue;
								}
							}
							else if (text == "highSpeedSteerAngle")
							{
								car.highSpeedSteerAngle = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
						else if (num <= 817708776U)
						{
							if (num <= 737234217U)
							{
								if (num <= 708085562U)
								{
									if (num != 689209674U)
									{
										if (num == 708085562U)
										{
											if (text == "engineBlockV8")
											{
												car.engineBlockV8 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
												continue;
											}
										}
									}
									else if (text == "helical30r")
									{
										car.helical30r = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
								else if (num != 720941986U)
								{
									if (num != 735045200U)
									{
										if (num == 737234217U)
										{
											if (text == "wd4Emissive")
											{
												car.wd4Emissive = reader.Read<float>(ES3Type_float.Instance);
												continue;
											}
										}
									}
									else if (text == "chgEmissive")
									{
										car.chgEmissive = reader.Read<float>(ES3Type_float.Instance);
										continue;
									}
								}
								else if (text == "enginescriptv8")
								{
									car.enginescriptv8 = reader.Read<enginev8>();
									continue;
								}
							}
							else if (num <= 747908478U)
							{
								if (num != 740176158U)
								{
									if (num == 747908478U)
									{
										if (text == "has5th")
										{
											car.has5th = reader.Read<bool>(ES3Type_bool.Instance);
											continue;
										}
									}
								}
								else if (text == "MaxWheelRotateAngle")
								{
									car.MaxWheelRotateAngle = reader.Read<float>(ES3Type_float.Instance);
									continue;
								}
							}
							else if (num != 798595260U)
							{
								if (num != 814691424U)
								{
									if (num == 817708776U)
									{
										if (text == "lowestSpeedAtSteer")
										{
											car.lowestSpeedAtSteer = reader.Read<float>(ES3Type_float.Instance);
											continue;
										}
									}
								}
								else if (text == "steamTrailsV8")
								{
									car.steamTrailsV8 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (text == "waterFlood")
							{
								car.waterFlood = reader.Read<int>(ES3Type_int.Instance);
								continue;
							}
						}
						else if (num <= 891653270U)
						{
							if (num <= 858098032U)
							{
								if (num != 851604344U)
								{
									if (num == 858098032U)
									{
										if (text == "rocker3")
										{
											car.rocker3 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "wheelRay")
								{
									car.wheelRay = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (num != 874875651U)
							{
								if (num != 878276345U)
								{
									if (num == 891653270U)
									{
										if (text == "rocker1")
										{
											car.rocker1 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "rotorFL")
								{
									car.rotorFL = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (text == "rocker2")
							{
								car.rocker2 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num <= 941986127U)
						{
							if (num != 925208508U)
							{
								if (num == 941986127U)
								{
									if (text == "rocker6")
									{
										car.rocker6 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "rocker7")
							{
								car.rocker7 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num != 947485984U)
						{
							if (num != 958763746U)
							{
								if (num == 959407159U)
								{
									if (text == "Fdrag")
									{
										car.Fdrag = reader.Read<float>(ES3Type_float.Instance);
										continue;
									}
								}
							}
							else if (text == "rocker5")
							{
								car.rocker5 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (text == "oilEmissive")
						{
							car.oilEmissive = reader.Read<float>(ES3Type_float.Instance);
							continue;
						}
					}
					else if (num <= 1655478574U)
					{
						if (num <= 1283659462U)
						{
							if (num <= 1031692888U)
							{
								if (num <= 985046057U)
								{
									if (num != 975541365U)
									{
										if (num == 985046057U)
										{
											if (text == "hasRollbar")
											{
												car.hasRollbar = reader.Read<bool>(ES3Type_bool.Instance);
												continue;
											}
										}
									}
									else if (text == "rocker4")
									{
										car.rocker4 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
								else if (num != 1004529100U)
								{
									if (num != 1031080680U)
									{
										if (num == 1031692888U)
										{
											if (text == "color")
											{
												car.color = reader.Read<Color>(ES3Type_Color.Instance);
												continue;
											}
										}
									}
									else if (text == "timingGear")
									{
										car.timingGear = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
								else if (text == "jumptime")
								{
									car.jumptime = reader.Read<float>(ES3Type_float.Instance);
									continue;
								}
							}
							else if (num <= 1109450902U)
							{
								if (num != 1042651841U)
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
								else if (text == "rocker8")
								{
									car.rocker8 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (num != 1195336803U)
							{
								if (num != 1231914923U)
								{
									if (num == 1283659462U)
									{
										if (text == "engineCrank")
										{
											car.engineCrank = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "isDigital")
								{
									car.isDigital = reader.Read<bool>(ES3Type_bool.Instance);
									continue;
								}
							}
							else if (text == "torque")
							{
								car.torque = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
						else if (num <= 1413139572U)
						{
							if (num <= 1308802079U)
							{
								if (num != 1298130101U)
								{
									if (num == 1308802079U)
									{
										if (text == "WheelRearLeft")
										{
											car.WheelRearLeft = reader.Read<WheelCollider>();
											continue;
										}
									}
								}
								else if (text == "coolantLevel")
								{
									car.coolantLevel = reader.Read<float>(ES3Type_float.Instance);
									continue;
								}
							}
							else if (num != 1330461687U)
							{
								if (num != 1403238542U)
								{
									if (num == 1413139572U)
									{
										if (text == "surfaceType")
										{
											car.surfaceType = reader.Read<int>(ES3Type_int.Instance);
											continue;
										}
									}
								}
								else if (text == "soundMat")
								{
									car.soundMat = reader.Read<bool>(ES3Type_bool.Instance);
									continue;
								}
							}
							else if (text == "element")
							{
								car.element = reader.Read<Renderer>();
								continue;
							}
						}
						else if (num <= 1503767242U)
						{
							if (num != 1479322555U)
							{
								if (num == 1503767242U)
								{
									if (text == "oilIcon")
									{
										car.oilIcon = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "oilLevel")
							{
								car.oilLevel = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
						else if (num != 1514965763U)
						{
							if (num != 1599373397U)
							{
								if (num == 1655478574U)
								{
									if (text == "cleanInterior")
									{
										car.cleanInterior = reader.Read<bool>(ES3Type_bool.Instance);
										continue;
									}
								}
							}
							else if (text == "rb")
							{
								car.rb = reader.Read<Rigidbody>(ES3UserType_Rigidbody.Instance);
								continue;
							}
						}
						else if (text == "maxTemp")
						{
							car.maxTemp = reader.Read<float>(ES3Type_float.Instance);
							continue;
						}
					}
					else if (num <= 1968638930U)
					{
						if (num <= 1852128937U)
						{
							if (num <= 1741872811U)
							{
								if (num != 1656109060U)
								{
									if (num == 1741872811U)
									{
										if (text == "FRholder")
										{
											car.FRholder = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "additiveBonus")
								{
									reader.SetPrivateField("additiveBonus", reader.Read<float>(), car);
									continue;
								}
							}
							else if (num != 1755665111U)
							{
								if (num != 1761569317U)
								{
									if (num == 1852128937U)
									{
										if (text == "hangTimeChallenge")
										{
											reader.SetPrivateField("hangTimeChallenge", reader.Read<bool>(), car);
											continue;
										}
									}
								}
								else if (text == "fastStart")
								{
									car.fastStart = reader.Read<bool>(ES3Type_bool.Instance);
									continue;
								}
							}
							else if (text == "steamTrailsOn")
							{
								car.steamTrailsOn = reader.Read<bool>(ES3Type_bool.Instance);
								continue;
							}
						}
						else if (num <= 1890937694U)
						{
							if (num != 1873592150U)
							{
								if (num == 1890937694U)
								{
									if (text == "rockerSpeed")
									{
										car.rockerSpeed = reader.Read<float>(ES3Type_float.Instance);
										continue;
									}
								}
							}
							else if (text == "camAndGear")
							{
								car.camAndGear = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num != 1919469360U)
						{
							if (num != 1923458214U)
							{
								if (num == 1968638930U)
								{
									if (text == "audioControl")
									{
										car.audioControl = reader.Read<AudioControl>();
										continue;
									}
								}
							}
							else if (text == "wheelFL")
							{
								car.wheelFL = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (text == "exhaustTrails")
						{
							car.exhaustTrails = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num <= 2222759721U)
					{
						if (num <= 2101557601U)
						{
							if (num != 2072037248U)
							{
								if (num == 2101557601U)
								{
									if (text == "FLholder")
									{
										car.FLholder = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "speed")
							{
								car.speed = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
						else if (num != 2172426864U)
						{
							if (num != 2189204483U)
							{
								if (num == 2222759721U)
								{
									if (text == "intLight1")
									{
										car.intLight1 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "intLight3")
							{
								car.intLight3 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (text == "intLight2")
						{
							car.intLight2 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num <= 2273092578U)
					{
						if (num != 2241834982U)
						{
							if (num == 2273092578U)
							{
								if (text == "intLight4")
								{
									car.intLight4 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
						}
						else if (text == "tempIcon")
						{
							car.tempIcon = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num != 2284576574U)
					{
						if (num != 2354326850U)
						{
							if (num == 2356340713U)
							{
								if (text == "RRpos")
								{
									car.RRpos = reader.Read<Transform>(ES3UserType_Transform.Instance);
									continue;
								}
							}
						}
						else if (text == "maxBrakeTorque")
						{
							car.maxBrakeTorque = reader.Read<float>(ES3Type_float.Instance);
							continue;
						}
					}
					else if (text == "deepWater")
					{
						car.deepWater = reader.Read<int>(ES3Type_int.Instance);
						continue;
					}
				}
				else if (num <= 3326546360U)
				{
					if (num <= 2859943884U)
					{
						if (num <= 2618419572U)
						{
							if (num <= 2544129665U)
							{
								if (num <= 2468172407U)
								{
									if (num != 2359676308U)
									{
										if (num == 2468172407U)
										{
											if (text == "heatTrailsOn")
											{
												car.heatTrailsOn = reader.Read<bool>(ES3Type_bool.Instance);
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
								else if (num != 2488174537U)
								{
									if (num == 2544129665U)
									{
										if (text == "maxFuel")
										{
											car.maxFuel = reader.Read<float>(ES3Type_float.Instance);
											continue;
										}
									}
								}
								else if (text == "steamTrailsOn2V8")
								{
									car.steamTrailsOn2V8 = reader.Read<bool>(ES3Type_bool.Instance);
									continue;
								}
							}
							else if (num <= 2559430583U)
							{
								if (num != 2552395441U)
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
								else if (text == "steamTrailsOnV8")
								{
									car.steamTrailsOnV8 = reader.Read<bool>(ES3Type_bool.Instance);
									continue;
								}
							}
							else if (num != 2560268759U)
							{
								if (num != 2577687804U)
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
								else if (text == "v8Fan")
								{
									car.v8Fan = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (text == "gravShell")
							{
								car.gravShell = reader.Read<TruckBedGrav>();
								continue;
							}
						}
						else if (num <= 2705423999U)
						{
							if (num <= 2625048943U)
							{
								if (num != 2619080378U)
								{
									if (num == 2625048943U)
									{
										if (text == "clutchIn")
										{
											car.clutchIn = reader.Read<bool>(ES3Type_bool.Instance);
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
							else if (num != 2629291871U)
							{
								if (num != 2669597572U)
								{
									if (num == 2705423999U)
									{
										if (text == "steamTrailsOn2")
										{
											car.steamTrailsOn2 = reader.Read<bool>(ES3Type_bool.Instance);
											continue;
										}
									}
								}
								else if (text == "steamTrails2")
								{
									car.steamTrails2 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (text == "WheelFrontRight")
							{
								car.WheelFrontRight = reader.Read<WheelCollider>();
								continue;
							}
						}
						else if (num <= 2745540939U)
						{
							if (num != 2731742166U)
							{
								if (num == 2745540939U)
								{
									if (text == "RLpos")
									{
										car.RLpos = reader.Read<Transform>(ES3UserType_Transform.Instance);
										continue;
									}
								}
							}
							else if (text == "interactor")
							{
								car.interactor = reader.Read<Interactor>(ES3UserType_Interactor.Instance);
								continue;
							}
						}
						else if (num != 2780858824U)
						{
							if (num != 2829747013U)
							{
								if (num == 2859943884U)
								{
									if (text == "tempEmissive")
									{
										car.tempEmissive = reader.Read<float>(ES3Type_float.Instance);
										continue;
									}
								}
							}
							else if (text == "FRpos")
							{
								car.FRpos = reader.Read<Transform>(ES3UserType_Transform.Instance);
								continue;
							}
						}
						else if (text == "theDest")
						{
							car.theDest = reader.Read<Transform>(ES3UserType_Transform.Instance);
							continue;
						}
					}
					else if (num <= 3156625349U)
					{
						if (num <= 2995254354U)
						{
							if (num <= 2901359373U)
							{
								if (num != 2859965226U)
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
								else if (text == "crankV8")
								{
									car.crankV8 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (num != 2946672573U)
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
							else if (text == "encheckEmissive")
							{
								car.encheckEmissive = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
						else if (num <= 3077523073U)
						{
							if (num != 3013902212U)
							{
								if (num == 3077523073U)
								{
									if (text == "centerOfMass")
									{
										car.centerOfMass = reader.Read<Transform>(ES3UserType_Transform.Instance);
										continue;
									}
								}
							}
							else if (text == "fourWheelDrive")
							{
								car.fourWheelDrive = reader.Read<bool>(ES3Type_bool.Instance);
								continue;
							}
						}
						else if (num != 3095373512U)
						{
							if (num != 3116312336U)
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
							else if (text == "pointerTemp")
							{
								car.pointerTemp = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (text == "splashFx")
						{
							car.splashFx = reader.Read<GameObject[]>(ES3Type_GameObjectArray.Instance);
							continue;
						}
					}
					else if (num <= 3234877807U)
					{
						if (num <= 3217606484U)
						{
							if (num != 3204192265U)
							{
								if (num == 3217606484U)
								{
									if (text == "engineBlock")
									{
										car.engineBlock = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "altFan")
							{
								car.altFan = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num != 3224526154U)
						{
							if (num != 3226018014U)
							{
								if (num == 3234877807U)
								{
									if (text == "RRholder")
									{
										car.RRholder = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "steamTrails")
							{
								car.steamTrails = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (text == "canRun")
						{
							car.canRun = reader.Read<bool>(ES3Type_bool.Instance);
							continue;
						}
					}
					else if (num <= 3297251496U)
					{
						if (num != 3235317062U)
						{
							if (num == 3297251496U)
							{
								if (text == "wheelRR")
								{
									car.wheelRR = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
						}
						else if (text == "enableOil")
						{
							car.enableOil = reader.Read<bool>(ES3Type_bool.Instance);
							continue;
						}
					}
					else if (num != 3300011627U)
					{
						if (num != 3324546841U)
						{
							if (num == 3326546360U)
							{
								if (text == "steerWheelAngle")
								{
									car.steerWheelAngle = reader.Read<float>(ES3Type_float.Instance);
									continue;
								}
							}
						}
						else if (text == "checkEngine")
						{
							car.checkEngine = reader.Read<bool>(ES3Type_bool.Instance);
							continue;
						}
					}
					else if (text == "minFuel")
					{
						car.minFuel = reader.Read<float>(ES3Type_float.Instance);
						continue;
					}
				}
				else if (num <= 3761969898U)
				{
					if (num <= 3621747277U)
					{
						if (num <= 3496923356U)
						{
							if (num <= 3357481341U)
							{
								if (num != 3328234388U)
								{
									if (num == 3357481341U)
									{
										if (text == "steerWheelRotateFactor")
										{
											car.steerWheelRotateFactor = reader.Read<float>(ES3Type_float.Instance);
											continue;
										}
									}
								}
								else if (text == "canMove")
								{
									car.canMove = reader.Read<bool>(ES3Type_bool.Instance);
									continue;
								}
							}
							else if (num != 3409087707U)
							{
								if (num != 3487516825U)
								{
									if (num == 3496923356U)
									{
										if (text == "brackets4")
										{
											car.brackets4 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "mudBrushRL")
								{
									car.mudBrushRL = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (text == "enableWd4")
							{
								car.enableWd4 = reader.Read<bool>(ES3Type_bool.Instance);
								continue;
							}
						}
						else if (num <= 3549934208U)
						{
							if (num != 3517820659U)
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
							else if (text == "mudBrushFR")
							{
								car.mudBrushFR = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num != 3564033832U)
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
						else if (text == "brackets8")
						{
							car.brackets8 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num <= 3682917512U)
					{
						if (num <= 3644230834U)
						{
							if (num != 3630413347U)
							{
								if (num == 3644230834U)
								{
									if (text == "WheelFrontLeft")
									{
										car.WheelFrontLeft = reader.Read<WheelCollider>();
										continue;
									}
								}
							}
							else if (text == "fastStartV8")
							{
								car.fastStartV8 = reader.Read<bool>(ES3Type_bool.Instance);
								continue;
							}
						}
						else if (num != 3674845753U)
						{
							if (num != 3678798212U)
							{
								if (num == 3682917512U)
								{
									if (text == "ebrakeEmissive")
									{
										car.ebrakeEmissive = reader.Read<float>(ES3Type_float.Instance);
										continue;
									}
								}
							}
							else if (text == "rearGear")
							{
								car.rearGear = reader.Read<int>(ES3Type_int.Instance);
								continue;
							}
						}
						else if (text == "exhaustTrailsOn")
						{
							car.exhaustTrailsOn = reader.Read<bool>(ES3Type_bool.Instance);
							continue;
						}
					}
					else if (num <= 3690043878U)
					{
						if (num != 3686661906U)
						{
							if (num == 3690043878U)
							{
								if (text == "fuelEmissive")
								{
									car.fuelEmissive = reader.Read<float>(ES3Type_float.Instance);
									continue;
								}
							}
						}
						else if (text == "ebrakeIcon")
						{
							car.ebrakeIcon = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num != 3732699693U)
					{
						if (num != 3760051994U)
						{
							if (num == 3761969898U)
							{
								if (text == "steamTrails2V8")
								{
									car.steamTrails2V8 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
						}
						else if (text == "hasSeat")
						{
							car.hasSeat = reader.Read<bool>(ES3Type_bool.Instance);
							continue;
						}
					}
					else if (text == "RLholder")
					{
						car.RLholder = reader.Read<GameObject>(ES3Type_GameObject.Instance);
						continue;
					}
				}
				else if (num <= 3986705639U)
				{
					if (num <= 3912985690U)
					{
						if (num <= 3775336775U)
						{
							if (num != 3775146795U)
							{
								if (num == 3775336775U)
								{
									if (text == "fourcyl")
									{
										car.fourcyl = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "fuelSaving")
							{
								car.fuelSaving = reader.Read<bool>(ES3Type_bool.Instance);
								continue;
							}
						}
						else if (num != 3810726972U)
						{
							if (num != 3892727146U)
							{
								if (num == 3912985690U)
								{
									if (text == "chgIcon")
									{
										car.chgIcon = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "rgbR")
							{
								car.rgbR = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
						else if (text == "braked")
						{
							car.braked = reader.Read<bool>(ES3Type_bool.Instance);
							continue;
						}
					}
					else if (num <= 3924994357U)
					{
						if (num != 3923734919U)
						{
							if (num == 3924994357U)
							{
								if (text == "temperature")
								{
									car.temperature = reader.Read<float>(ES3Type_float.Instance);
									continue;
								}
							}
						}
						else if (text == "mudBrushRR")
						{
							car.mudBrushRR = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num != 3934402935U)
					{
						if (num != 3972093706U)
						{
							if (num == 3986705639U)
							{
								if (text == "hasDiffLock")
								{
									car.hasDiffLock = reader.Read<bool>(ES3Type_bool.Instance);
									continue;
								}
							}
						}
						else if (text == "gasPedal")
						{
							car.gasPedal = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (text == "boostGauge")
					{
						car.boostGauge = reader.Read<GameObject>(ES3Type_GameObject.Instance);
						continue;
					}
				}
				else if (num <= 4069336743U)
				{
					if (num <= 4035582048U)
					{
						if (num != 4021149229U)
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
						else if (text == "mudBrushFL")
						{
							car.mudBrushFL = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num != 4052559124U)
					{
						if (num != 4069004972U)
						{
							if (num == 4069336743U)
							{
								if (text == "camBearing3")
								{
									car.camBearing3 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
						}
						else if (text == "Cdrag")
						{
							car.Cdrag = reader.Read<float>(ES3Type_float.Instance);
							continue;
						}
					}
					else if (text == "camBearing2")
					{
						car.camBearing2 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
						continue;
					}
				}
				else if (num <= 4110836193U)
				{
					if (num != 4102891981U)
					{
						if (num == 4110836193U)
						{
							if (text == "rgbG")
							{
								car.rgbG = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
					}
					else if (text == "camBearing1")
					{
						car.camBearing1 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
						continue;
					}
				}
				else if (num != 4161169050U)
				{
					if (num != 4223114689U)
					{
						if (num == 4267917295U)
						{
							if (text == "FLpos")
							{
								car.FLpos = reader.Read<Transform>(ES3UserType_Transform.Instance);
								continue;
							}
						}
					}
					else if (text == "rearTorque")
					{
						car.rearTorque = reader.Read<float>(ES3Type_float.Instance);
						continue;
					}
				}
				else if (text == "rgbB")
				{
					car.rgbB = reader.Read<float>(ES3Type_float.Instance);
					continue;
				}
				reader.Skip();
			}
		}

		// Token: 0x04002472 RID: 9330
		public static ES3Type Instance;
	}
}
