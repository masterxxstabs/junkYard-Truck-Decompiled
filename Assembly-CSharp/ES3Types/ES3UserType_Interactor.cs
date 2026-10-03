using System;
using Michsky.UI.Dark;
using UnityEngine;
using UnityEngine.Scripting;
using UnityStandardAssets.Characters.FirstPerson;

namespace ES3Types
{
	// Token: 0x020002E6 RID: 742
	[Preserve]
	[ES3Properties(new string[]
	{
		"interactRange",
		"specialfab",
		"FirstPersonCharacter",
		"drivingCar",
		"pickedUpObject",
		"person",
		"personFX",
		"pcCanvas",
		"inspectionCanvas",
		"inventoryCanvas",
		"inv",
		"myPC",
		"myPCtower",
		"seatMount",
		"seatMountCar",
		"seatMountCart",
		"creeperMount",
		"creeperUnmount",
		"exitMount",
		"exitMountP",
		"watchMount",
		"leftGlass",
		"exitMountCar",
		"exitMountCarP",
		"exitMountCart",
		"truck",
		"fourcyl",
		"v8",
		"ucar",
		"cart",
		"creeper",
		"fpsHand",
		"fpsRatchet",
		"ratchetLever",
		"fpsHook",
		"winchHook",
		"winchObject",
		"mNeedle",
		"multimeter",
		"screwdriver",
		"tirepump",
		"nokia",
		"tiregauge",
		"tiregaugemeter",
		"jumpercables",
		"crowbar",
		"pickaxe",
		"pickaxeObj",
		"weldgun",
		"holdingWelder",
		"weldingFlash",
		"welderMachine",
		"welderVisual",
		"phonePanel",
		"lottoPanel",
		"moonPanel",
		"tirePanel",
		"racePanel",
		"escapePanel",
		"toolTipPanel",
		"mainPanelMgr",
		"newRadius",
		"deflation",
		"percentageInflated",
		"wheelContainerHeight",
		"wheelHolderFL",
		"wheelHolderFR",
		"wheelHolderRL",
		"wheelHolderRR",
		"FLbonus",
		"FRbonus",
		"RLbonus",
		"RRbonus",
		"w4bonus",
		"wheelColFL",
		"wheelColFR",
		"wheelColRL",
		"wheelColRR",
		"eventSystem",
		"ignition_keyO",
		"ignition_keyC",
		"emptyCan",
		"truckLightR",
		"truckLightL",
		"truckLightRL",
		"carLightR",
		"carLightL",
		"carLightRL",
		"JackObj",
		"recycleZone",
		"recycleZoneTrash",
		"recycleZoneWood",
		"recycleZoneOre",
		"johnny1",
		"newageGirl",
		"chainsawobj",
		"gasStationTill",
		"trailer",
		"trailerGen",
		"hitchPoint",
		"hitch",
		"hitchG",
		"hitchLink",
		"fpc",
		"weatherId",
		"pumpMachine",
		"paSources",
		"canRun",
		"canCrank",
		"enterposition",
		"carscript",
		"ucarAudio",
		"still",
		"businessman",
		"fireman",
		"jimmy",
		"jake",
		"jiggs",
		"racenpc",
		"towerstatus1",
		"overflowvalve",
		"emissiveBolt",
		"resetEmissive",
		"radio",
		"lightscript",
		"dk9",
		"EngStand",
		"haltDetach",
		"oilBolt",
		"oilBoltV8",
		"sleepScript",
		"FSM",
		"mapCanvas",
		"playerArrow",
		"canTrack",
		"discoveredMission",
		"mail",
		"leanDest",
		"leanDestO",
		"cameraTransform",
		"mapImage",
		"subtitles",
		"JarCratePrefab",
		"restockableItems",
		"wheelObj",
		"RemoveFL",
		"RemoveFR",
		"RemoveRL",
		"RemoveRR",
		"truckBed",
		"gd",
		"currency",
		"trb",
		"raceActive",
		"checkpoints",
		"targetPositionTranform",
		"RaceTargets",
		"nextTarget",
		"moneyRoll",
		"pokerScript",
		"holdingHose",
		"holdingHoseTank",
		"fpshose",
		"garageFaucet",
		"waterValve",
		"debugMenu",
		"diamondbackImpounded",
		"f100Impounded",
		"amcImpounded",
		"dirtbikeImpounded",
		"golfcartImpounded",
		"timeOfDay"
	})]
	public class ES3UserType_Interactor : ES3ComponentType
	{
		// Token: 0x060013C8 RID: 5064 RVA: 0x000CD355 File Offset: 0x000CB555
		public ES3UserType_Interactor() : base(typeof(Interactor))
		{
			ES3UserType_Interactor.Instance = this;
			this.priority = 1;
		}

		// Token: 0x060013C9 RID: 5065 RVA: 0x000CD374 File Offset: 0x000CB574
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			Interactor interactor = (Interactor)obj;
			writer.WritePrivateField("interactRange", interactor);
			writer.WritePropertyByRef("specialfab", interactor.specialfab);
			writer.WritePropertyByRef("FirstPersonCharacter", interactor.FirstPersonCharacter);
			writer.WriteProperty("drivingCar", interactor.drivingCar, ES3Type_bool.Instance);
			writer.WritePropertyByRef("pickedUpObject", interactor.pickedUpObject);
			writer.WritePropertyByRef("person", interactor.person);
			writer.WritePropertyByRef("personFX", interactor.personFX);
			writer.WritePropertyByRef("pcCanvas", interactor.pcCanvas);
			writer.WritePropertyByRef("inspectionCanvas", interactor.inspectionCanvas);
			writer.WritePropertyByRef("inventoryCanvas", interactor.inventoryCanvas);
			writer.WritePropertyByRef("inv", interactor.inv);
			writer.WritePropertyByRef("myPC", interactor.myPC);
			writer.WritePropertyByRef("myPCtower", interactor.myPCtower);
			writer.WritePropertyByRef("seatMount", interactor.seatMount);
			writer.WritePropertyByRef("seatMountCar", interactor.seatMountCar);
			writer.WritePropertyByRef("seatMountCart", interactor.seatMountCart);
			writer.WritePropertyByRef("creeperMount", interactor.creeperMount);
			writer.WritePropertyByRef("creeperUnmount", interactor.creeperUnmount);
			writer.WritePropertyByRef("exitMount", interactor.exitMount);
			writer.WritePropertyByRef("exitMountP", interactor.exitMountP);
			writer.WritePropertyByRef("watchMount", interactor.watchMount);
			writer.WritePropertyByRef("leftGlass", interactor.leftGlass);
			writer.WritePropertyByRef("exitMountCar", interactor.exitMountCar);
			writer.WritePropertyByRef("exitMountCarP", interactor.exitMountCarP);
			writer.WritePropertyByRef("exitMountCart", interactor.exitMountCart);
			writer.WritePropertyByRef("truck", interactor.truck);
			writer.WritePropertyByRef("fourcyl", interactor.fourcyl);
			writer.WritePropertyByRef("v8", interactor.v8);
			writer.WritePropertyByRef("ucar", interactor.ucar);
			writer.WritePropertyByRef("cart", interactor.cart);
			writer.WritePropertyByRef("creeper", interactor.creeper);
			writer.WritePropertyByRef("fpsHand", interactor.fpsHand);
			writer.WritePropertyByRef("fpsRatchet", interactor.fpsRatchet);
			writer.WritePropertyByRef("ratchetLever", interactor.ratchetLever);
			writer.WritePropertyByRef("fpsHook", interactor.fpsHook);
			writer.WritePropertyByRef("winchHook", interactor.winchHook);
			writer.WritePropertyByRef("winchObject", interactor.winchObject);
			writer.WritePropertyByRef("mNeedle", interactor.mNeedle);
			writer.WritePropertyByRef("multimeter", interactor.multimeter);
			writer.WritePropertyByRef("screwdriver", interactor.screwdriver);
			writer.WritePropertyByRef("tirepump", interactor.tirepump);
			writer.WritePropertyByRef("nokia", interactor.nokia);
			writer.WritePropertyByRef("tiregauge", interactor.tiregauge);
			writer.WritePropertyByRef("tiregaugemeter", interactor.tiregaugemeter);
			writer.WritePropertyByRef("jumpercables", interactor.jumpercables);
			writer.WritePropertyByRef("crowbar", interactor.crowbar);
			writer.WritePropertyByRef("pickaxe", interactor.pickaxe);
			writer.WritePropertyByRef("pickaxeObj", interactor.pickaxeObj);
			writer.WritePropertyByRef("weldgun", interactor.weldgun);
			writer.WriteProperty("holdingWelder", interactor.holdingWelder, ES3Type_bool.Instance);
			writer.WritePropertyByRef("weldingFlash", interactor.weldingFlash);
			writer.WritePropertyByRef("welderMachine", interactor.welderMachine);
			writer.WritePropertyByRef("welderVisual", interactor.welderVisual);
			writer.WritePropertyByRef("phonePanel", interactor.phonePanel);
			writer.WritePropertyByRef("lottoPanel", interactor.lottoPanel);
			writer.WritePropertyByRef("moonPanel", interactor.moonPanel);
			writer.WritePropertyByRef("tirePanel", interactor.tirePanel);
			writer.WritePropertyByRef("racePanel", interactor.racePanel);
			writer.WritePropertyByRef("escapePanel", interactor.escapePanel);
			writer.WritePropertyByRef("toolTipPanel", interactor.toolTipPanel);
			writer.WritePropertyByRef("mainPanelMgr", interactor.mainPanelMgr);
			writer.WriteProperty("newRadius", interactor.newRadius, ES3Type_double.Instance);
			writer.WriteProperty("deflation", interactor.deflation, ES3Type_float.Instance);
			writer.WriteProperty("percentageInflated", interactor.percentageInflated, ES3Type_double.Instance);
			writer.WriteProperty("wheelContainerHeight", interactor.wheelContainerHeight, ES3Type_double.Instance);
			writer.WritePropertyByRef("wheelHolderFL", interactor.wheelHolderFL);
			writer.WritePropertyByRef("wheelHolderFR", interactor.wheelHolderFR);
			writer.WritePropertyByRef("wheelHolderRL", interactor.wheelHolderRL);
			writer.WritePropertyByRef("wheelHolderRR", interactor.wheelHolderRR);
			writer.WriteProperty("FLbonus", interactor.FLbonus, ES3Type_float.Instance);
			writer.WriteProperty("FRbonus", interactor.FRbonus, ES3Type_float.Instance);
			writer.WriteProperty("RLbonus", interactor.RLbonus, ES3Type_float.Instance);
			writer.WriteProperty("RRbonus", interactor.RRbonus, ES3Type_float.Instance);
			writer.WriteProperty("w4bonus", interactor.w4bonus, ES3Type_float.Instance);
			writer.WritePropertyByRef("wheelColFL", interactor.wheelColFL);
			writer.WritePropertyByRef("wheelColFR", interactor.wheelColFR);
			writer.WritePropertyByRef("wheelColRL", interactor.wheelColRL);
			writer.WritePropertyByRef("wheelColRR", interactor.wheelColRR);
			writer.WritePropertyByRef("eventSystem", interactor.eventSystem);
			writer.WritePropertyByRef("ignition_keyO", interactor.ignition_keyO);
			writer.WritePropertyByRef("ignition_keyC", interactor.ignition_keyC);
			writer.WritePropertyByRef("emptyCan", interactor.emptyCan);
			writer.WritePropertyByRef("truckLightR", interactor.truckLightR);
			writer.WritePropertyByRef("truckLightL", interactor.truckLightL);
			writer.WritePropertyByRef("truckLightRL", interactor.truckLightRL);
			writer.WritePropertyByRef("carLightR", interactor.carLightR);
			writer.WritePropertyByRef("carLightL", interactor.carLightL);
			writer.WritePropertyByRef("carLightRL", interactor.carLightRL);
			writer.WritePropertyByRef("JackObj", interactor.JackObj);
			writer.WritePropertyByRef("recycleZone", interactor.recycleZone);
			writer.WritePropertyByRef("recycleZoneTrash", interactor.recycleZoneTrash);
			writer.WritePropertyByRef("recycleZoneWood", interactor.recycleZoneWood);
			writer.WritePropertyByRef("recycleZoneOre", interactor.recycleZoneOre);
			writer.WritePropertyByRef("johnny1", interactor.johnny1);
			writer.WritePropertyByRef("newageGirl", interactor.newageGirl);
			writer.WritePropertyByRef("chainsawobj", interactor.chainsawobj);
			writer.WritePropertyByRef("gasStationTill", interactor.gasStationTill);
			writer.WritePropertyByRef("trailer", interactor.trailer);
			writer.WritePropertyByRef("trailerGen", interactor.trailerGen);
			writer.WritePropertyByRef("hitchPoint", interactor.hitchPoint);
			writer.WritePropertyByRef("hitch", interactor.hitch);
			writer.WritePropertyByRef("hitchG", interactor.hitchG);
			writer.WritePropertyByRef("hitchLink", interactor.hitchLink);
			writer.WritePropertyByRef("fpc", interactor.fpc);
			writer.WritePrivateField("weatherId", interactor);
			writer.WritePropertyByRef("pumpMachine", interactor.pumpMachine);
			writer.WritePropertyByRef("paSources", interactor.paSources);
			writer.WriteProperty("canRun", interactor.canRun, ES3Type_bool.Instance);
			writer.WriteProperty("canCrank", interactor.canCrank, ES3Type_bool.Instance);
			writer.WriteProperty("enterposition", interactor.enterposition, ES3Type_Vector3.Instance);
			writer.WritePropertyByRef("carscript", interactor.carscript);
			writer.WritePropertyByRef("ucarAudio", interactor.ucarAudio);
			writer.WritePropertyByRef("still", interactor.still);
			writer.WritePropertyByRef("businessman", interactor.businessman);
			writer.WritePropertyByRef("fireman", interactor.fireman);
			writer.WritePropertyByRef("jimmy", interactor.jimmy);
			writer.WritePropertyByRef("jake", interactor.jake);
			writer.WritePropertyByRef("jiggs", interactor.jiggs);
			writer.WritePropertyByRef("racenpc", interactor.racenpc);
			writer.WriteProperty("towerstatus1", interactor.towerstatus1, ES3Type_int.Instance);
			writer.WriteProperty("overflowvalve", interactor.overflowvalve, ES3Type_int.Instance);
			writer.WritePropertyByRef("emissiveBolt", interactor.emissiveBolt);
			writer.WriteProperty("resetEmissive", interactor.resetEmissive, ES3Type_bool.Instance);
			writer.WritePropertyByRef("radio", interactor.radio);
			writer.WritePropertyByRef("lightscript", interactor.lightscript);
			writer.WritePropertyByRef("dk9", interactor.dk9);
			writer.WritePropertyByRef("EngStand", interactor.EngStand);
			writer.WriteProperty("haltDetach", interactor.haltDetach, ES3Type_bool.Instance);
			writer.WritePropertyByRef("oilBolt", interactor.oilBolt);
			writer.WritePropertyByRef("oilBoltV8", interactor.oilBoltV8);
			writer.WritePropertyByRef("sleepScript", interactor.sleepScript);
			writer.WritePropertyByRef("FSM", interactor.FSM);
			writer.WritePropertyByRef("mapCanvas", interactor.mapCanvas);
			writer.WritePropertyByRef("playerArrow", interactor.playerArrow);
			writer.WriteProperty("canTrack", interactor.canTrack, ES3Type_bool.Instance);
			writer.WriteProperty("discoveredMission", interactor.discoveredMission, ES3Type_int.Instance);
			writer.WritePropertyByRef("mail", interactor.mail);
			writer.WritePropertyByRef("leanDest", interactor.leanDest);
			writer.WritePropertyByRef("leanDestO", interactor.leanDestO);
			writer.WritePropertyByRef("cameraTransform", interactor.cameraTransform);
			writer.WritePropertyByRef("mapImage", interactor.mapImage);
			writer.WritePropertyByRef("subtitles", interactor.subtitles);
			writer.WritePropertyByRef("JarCratePrefab", interactor.JarCratePrefab);
			writer.WritePropertyByRef("restockableItems", interactor.restockableItems);
			writer.WritePropertyByRef("wheelObj", interactor.wheelObj);
			writer.WritePropertyByRef("RemoveFL", interactor.RemoveFL);
			writer.WritePropertyByRef("RemoveFR", interactor.RemoveFR);
			writer.WritePropertyByRef("RemoveRL", interactor.RemoveRL);
			writer.WritePropertyByRef("RemoveRR", interactor.RemoveRR);
			writer.WritePropertyByRef("truckBed", interactor.truckBed);
			writer.WritePropertyByRef("gd", interactor.gd);
			writer.WritePropertyByRef("currency", interactor.currency);
			writer.WritePropertyByRef("trb", interactor.trb);
			writer.WriteProperty("raceActive", interactor.raceActive, ES3Type_bool.Instance);
			writer.WriteProperty("checkpoints", interactor.checkpoints, ES3Type_int.Instance);
			writer.WritePropertyByRef("targetPositionTranform", interactor.targetPositionTranform);
			writer.WritePropertyByRef("RaceTargets", interactor.RaceTargets);
			writer.WritePropertyByRef("nextTarget", interactor.nextTarget);
			writer.WritePropertyByRef("moneyRoll", interactor.moneyRoll);
			writer.WritePropertyByRef("pokerScript", interactor.pokerScript);
			writer.WriteProperty("holdingHose", interactor.holdingHose, ES3Type_bool.Instance);
			writer.WriteProperty("holdingHoseTank", interactor.holdingHoseTank, ES3Type_bool.Instance);
			writer.WritePropertyByRef("fpshose", interactor.fpshose);
			writer.WritePropertyByRef("garageFaucet", interactor.garageFaucet);
			writer.WritePropertyByRef("waterValve", interactor.waterValve);
			writer.WritePropertyByRef("debugMenu", interactor.debugMenu);
			writer.WriteProperty("diamondbackImpounded", interactor.diamondbackImpounded, ES3Type_bool.Instance);
			writer.WriteProperty("f100Impounded", interactor.f100Impounded, ES3Type_bool.Instance);
			writer.WriteProperty("amcImpounded", interactor.amcImpounded, ES3Type_bool.Instance);
			writer.WriteProperty("dirtbikeImpounded", interactor.dirtbikeImpounded, ES3Type_bool.Instance);
			writer.WriteProperty("golfcartImpounded", interactor.golfcartImpounded, ES3Type_bool.Instance);
			writer.WriteProperty("timeOfDay", interactor.timeOfDay, ES3Type_float.Instance);
		}

		// Token: 0x060013CA RID: 5066 RVA: 0x000CE018 File Offset: 0x000CC218
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			Interactor interactor = (Interactor)obj;
			foreach (object obj2 in reader.Properties)
			{
				string text = (string)obj2;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 1947136185U)
				{
					if (num <= 805886525U)
					{
						if (num <= 368510814U)
						{
							if (num <= 240786126U)
							{
								if (num <= 89258017U)
								{
									if (num <= 39742388U)
									{
										if (num != 15679191U)
										{
											if (num == 39742388U)
											{
												if (text == "pickaxe")
												{
													interactor.pickaxe = reader.Read<GameObject>(ES3Type_GameObject.Instance);
													continue;
												}
											}
										}
										else if (text == "phonePanel")
										{
											interactor.phonePanel = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
									else if (num != 48325421U)
									{
										if (num != 65940267U)
										{
											if (num == 89258017U)
											{
												if (text == "tirepump")
												{
													interactor.tirepump = reader.Read<GameObject>(ES3Type_GameObject.Instance);
													continue;
												}
											}
										}
										else if (text == "pickedUpObject")
										{
											interactor.pickedUpObject = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
									else if (text == "haltDetach")
									{
										interactor.haltDetach = reader.Read<bool>(ES3Type_bool.Instance);
										continue;
									}
								}
								else if (num <= 138026643U)
								{
									if (num != 127491772U)
									{
										if (num == 138026643U)
										{
											if (text == "ignition_keyC")
											{
												interactor.ignition_keyC = reader.Read<GameObject>(ES3Type_GameObject.Instance);
												continue;
											}
										}
									}
									else if (text == "canTrack")
									{
										interactor.canTrack = reader.Read<bool>(ES3Type_bool.Instance);
										continue;
									}
								}
								else if (num != 159022252U)
								{
									if (num != 228613474U)
									{
										if (num == 240786126U)
										{
											if (text == "carscript")
											{
												interactor.carscript = reader.Read<car>(ES3UserType_car.Instance);
												continue;
											}
										}
									}
									else if (text == "ratchetLever")
									{
										interactor.ratchetLever = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
								else if (text == "wheelColFL")
								{
									interactor.wheelColFL = reader.Read<WheelCollider>();
									continue;
								}
							}
							else if (num <= 275195160U)
							{
								if (num <= 272251894U)
								{
									if (num != 246396521U)
									{
										if (num == 272251894U)
										{
											if (text == "racePanel")
											{
												interactor.racePanel = reader.Read<GameObject>(ES3Type_GameObject.Instance);
												continue;
											}
										}
									}
									else if (text == "FSM")
									{
										interactor.FSM = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
								else if (num != 272309904U)
								{
									if (num != 273301055U)
									{
										if (num == 275195160U)
										{
											if (text == "jake")
											{
												interactor.jake = reader.Read<GameObject>(ES3Type_GameObject.Instance);
												continue;
											}
										}
									}
									else if (text == "debugMenu")
									{
										interactor.debugMenu = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
								else if (text == "moneyRoll")
								{
									interactor.moneyRoll = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (num <= 339358071U)
							{
								if (num != 281095909U)
								{
									if (num != 283433756U)
									{
										if (num == 339358071U)
										{
											if (text == "ignition_keyO")
											{
												interactor.ignition_keyO = reader.Read<GameObject>(ES3Type_GameObject.Instance);
												continue;
											}
										}
									}
									else if (text == "chainsawobj")
									{
										interactor.chainsawobj = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
								else if (text == "screwdriver")
								{
									interactor.screwdriver = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (num != 341523443U)
							{
								if (num != 347980198U)
								{
									if (num == 368510814U)
									{
										if (text == "personFX")
										{
											interactor.personFX = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "dirtbikeImpounded")
								{
									interactor.dirtbikeImpounded = reader.Read<bool>(ES3Type_bool.Instance);
									continue;
								}
							}
							else if (text == "multimeter")
							{
								interactor.multimeter = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num <= 528129870U)
						{
							if (num <= 448459053U)
							{
								if (num <= 400684215U)
								{
									if (num != 377402584U)
									{
										if (num == 400684215U)
										{
											if (text == "holdingWelder")
											{
												interactor.holdingWelder = reader.Read<bool>(ES3Type_bool.Instance);
												continue;
											}
										}
									}
									else if (text == "lightscript")
									{
										interactor.lightscript = reader.Read<streetlights>();
										continue;
									}
								}
								else if (num != 408845923U)
								{
									if (num != 434215792U)
									{
										if (num == 448459053U)
										{
											if (text == "deflation")
											{
												interactor.deflation = reader.Read<float>(ES3Type_float.Instance);
												continue;
											}
										}
									}
									else if (text == "pcCanvas")
									{
										interactor.pcCanvas = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
								else if (text == "dk9")
								{
									interactor.dk9 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (num <= 500101630U)
							{
								if (num != 450612921U)
								{
									if (num != 459971978U)
									{
										if (num == 500101630U)
										{
											if (text == "subtitles")
											{
												interactor.subtitles = reader.Read<GameObject>(ES3Type_GameObject.Instance);
												continue;
											}
										}
									}
									else if (text == "recycleZoneTrash")
									{
										interactor.recycleZoneTrash = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
								else if (text == "amcImpounded")
								{
									interactor.amcImpounded = reader.Read<bool>(ES3Type_bool.Instance);
									continue;
								}
							}
							else if (num != 516853408U)
							{
								if (num != 518026200U)
								{
									if (num == 528129870U)
									{
										if (text == "wheelColFR")
										{
											interactor.wheelColFR = reader.Read<WheelCollider>();
											continue;
										}
									}
								}
								else if (text == "garageFaucet")
								{
									interactor.garageFaucet = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (text == "percentageInflated")
							{
								interactor.percentageInflated = reader.Read<double>(ES3Type_double.Instance);
								continue;
							}
						}
						else if (num <= 627171178U)
						{
							if (num <= 574336181U)
							{
								if (num != 533856437U)
								{
									if (num == 574336181U)
									{
										if (text == "gasStationTill")
										{
											interactor.gasStationTill = reader.Read<TillScript>();
											continue;
										}
									}
								}
								else if (text == "watchMount")
								{
									interactor.watchMount = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (num != 577323383U)
							{
								if (num != 577725670U)
								{
									if (num == 627171178U)
									{
										if (text == "waterValve")
										{
											interactor.waterValve = reader.Read<WaterValve>();
											continue;
										}
									}
								}
								else if (text == "welderVisual")
								{
									interactor.welderVisual = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (text == "RemoveFL")
							{
								interactor.RemoveFL = reader.Read<Transform>(ES3UserType_Transform.Instance);
								continue;
							}
						}
						else if (num <= 709568433U)
						{
							if (num != 665902946U)
							{
								if (num != 677989097U)
								{
									if (num == 709568433U)
									{
										if (text == "seatMountCar")
										{
											interactor.seatMountCar = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "RemoveFR")
								{
									interactor.RemoveFR = reader.Read<Transform>(ES3UserType_Transform.Instance);
									continue;
								}
							}
							else if (text == "enterposition")
							{
								interactor.enterposition = reader.Read<Vector3>(ES3Type_Vector3.Instance);
								continue;
							}
						}
						else if (num != 713579309U)
						{
							if (num != 721405161U)
							{
								if (num == 805886525U)
								{
									if (text == "fpshose")
									{
										interactor.fpshose = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "pickaxeObj")
							{
								interactor.pickaxeObj = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (text == "hitchLink")
						{
							interactor.hitchLink = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num <= 1458566302U)
					{
						if (num <= 1119276295U)
						{
							if (num <= 991980614U)
							{
								if (num <= 941074430U)
								{
									if (num != 845892171U)
									{
										if (num == 941074430U)
										{
											if (text == "ucar")
											{
												interactor.ucar = reader.Read<GameObject>(ES3Type_GameObject.Instance);
												continue;
											}
										}
									}
									else if (text == "fireman")
									{
										interactor.fireman = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
								else if (num != 952485173U)
								{
									if (num != 954357486U)
									{
										if (num == 991980614U)
										{
											if (text == "gd")
											{
												interactor.gd = reader.Read<GroundDetect>();
												continue;
											}
										}
									}
									else if (text == "moonPanel")
									{
										interactor.moonPanel = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
								else if (text == "hitchPoint")
								{
									interactor.hitchPoint = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (num <= 1016378116U)
							{
								if (num != 1006668012U)
								{
									if (num == 1016378116U)
									{
										if (text == "emptyCan")
										{
											interactor.emptyCan = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "paSources")
								{
									interactor.paSources = reader.Read<AudioSource>();
									continue;
								}
							}
							else if (num != 1078680477U)
							{
								if (num != 1112235715U)
								{
									if (num == 1119276295U)
									{
										if (text == "towerstatus1")
										{
											interactor.towerstatus1 = reader.Read<int>(ES3Type_int.Instance);
											continue;
										}
									}
								}
								else if (text == "RemoveRL")
								{
									interactor.RemoveRL = reader.Read<Transform>(ES3UserType_Transform.Instance);
									continue;
								}
							}
							else if (text == "RemoveRR")
							{
								interactor.RemoveRR = reader.Read<Transform>(ES3UserType_Transform.Instance);
								continue;
							}
						}
						else if (num <= 1298371263U)
						{
							if (num <= 1170115133U)
							{
								if (num != 1120441619U)
								{
									if (num == 1170115133U)
									{
										if (text == "mapCanvas")
										{
											interactor.mapCanvas = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "overflowvalve")
								{
									interactor.overflowvalve = reader.Read<int>(ES3Type_int.Instance);
									continue;
								}
							}
							else if (num != 1197329186U)
							{
								if (num != 1223676388U)
								{
									if (num == 1298371263U)
									{
										if (text == "inspectionCanvas")
										{
											interactor.inspectionCanvas = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "checkpoints")
								{
									interactor.checkpoints = reader.Read<int>(ES3Type_int.Instance);
									continue;
								}
							}
							else if (text == "oilBoltV8")
							{
								interactor.oilBoltV8 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num <= 1327596754U)
						{
							if (num != 1304319563U)
							{
								if (num != 1319725795U)
								{
									if (num == 1327596754U)
									{
										if (text == "wheelHolderFR")
										{
											interactor.wheelHolderFR = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "creeperUnmount")
								{
									interactor.creeperUnmount = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (text == "inventoryCanvas")
							{
								interactor.inventoryCanvas = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num != 1384775983U)
						{
							if (num != 1435381254U)
							{
								if (num == 1458566302U)
								{
									if (text == "wheelHolderRR")
									{
										interactor.wheelHolderRR = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "mapImage")
							{
								interactor.mapImage = reader.Read<RectTransform>(ES3Type_RectTransform.Instance);
								continue;
							}
						}
						else if (text == "resetEmissive")
						{
							interactor.resetEmissive = reader.Read<bool>(ES3Type_bool.Instance);
							continue;
						}
					}
					else if (num <= 1678223317U)
					{
						if (num <= 1626342492U)
						{
							if (num <= 1575568233U)
							{
								if (num != 1498389279U)
								{
									if (num == 1575568233U)
									{
										if (text == "jimmy")
										{
											interactor.jimmy = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "seatMountCart")
								{
									interactor.seatMountCart = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (num != 1582883289U)
							{
								if (num != 1587843633U)
								{
									if (num == 1626342492U)
									{
										if (text == "wheelHolderRL")
										{
											interactor.wheelHolderRL = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "nextTarget")
								{
									interactor.nextTarget = reader.Read<Transform>(ES3UserType_Transform.Instance);
									continue;
								}
							}
							else if (text == "diamondbackImpounded")
							{
								interactor.diamondbackImpounded = reader.Read<bool>(ES3Type_bool.Instance);
								continue;
							}
						}
						else if (num <= 1643210229U)
						{
							if (num != 1629593896U)
							{
								if (num != 1641840327U)
								{
									if (num == 1643210229U)
									{
										if (text == "seatMount")
										{
											interactor.seatMount = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "crowbar")
								{
									interactor.crowbar = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (text == "wheelHolderFL")
							{
								interactor.wheelHolderFL = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num != 1661424064U)
						{
							if (num != 1666919399U)
							{
								if (num == 1678223317U)
								{
									if (text == "recycleZoneWood")
									{
										interactor.recycleZoneWood = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "JackObj")
							{
								interactor.JackObj = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (text == "exitMountCart")
						{
							interactor.exitMountCart = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num <= 1803359099U)
					{
						if (num <= 1756274833U)
						{
							if (num != 1726319425U)
							{
								if (num == 1756274833U)
								{
									if (text == "lottoPanel")
									{
										interactor.lottoPanel = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "wheelObj")
							{
								interactor.wheelObj = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num != 1782429994U)
						{
							if (num != 1793632069U)
							{
								if (num == 1803359099U)
								{
									if (text == "tirePanel")
									{
										interactor.tirePanel = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "holdingHose")
							{
								interactor.holdingHose = reader.Read<bool>(ES3Type_bool.Instance);
								continue;
							}
						}
						else if (text == "truckLightR")
						{
							interactor.truckLightR = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num <= 1855809980U)
					{
						if (num != 1815985232U)
						{
							if (num != 1834530623U)
							{
								if (num == 1855809980U)
								{
									if (text == "wheelContainerHeight")
									{
										interactor.wheelContainerHeight = reader.Read<double>(ES3Type_double.Instance);
										continue;
									}
								}
							}
							else if (text == "truckBed")
							{
								interactor.truckBed = reader.Read<TruckBedGrav>();
								continue;
							}
						}
						else if (text == "truckLightL")
						{
							interactor.truckLightL = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num != 1900738654U)
					{
						if (num != 1915089762U)
						{
							if (num == 1947136185U)
							{
								if (text == "hitch")
								{
									interactor.hitch = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
						}
						else if (text == "FLbonus")
						{
							interactor.FLbonus = reader.Read<float>(ES3Type_float.Instance);
							continue;
						}
					}
					else if (text == "restockableItems")
					{
						interactor.restockableItems = reader.Read<GameObject>(ES3Type_GameObject.Instance);
						continue;
					}
				}
				else if (num <= 2985344196U)
				{
					if (num <= 2518392755U)
					{
						if (num <= 2265418348U)
						{
							if (num <= 2056023484U)
							{
								if (num <= 2021853496U)
								{
									if (num != 2015723316U)
									{
										if (num == 2021853496U)
										{
											if (text == "targetPositionTranform")
											{
												interactor.targetPositionTranform = reader.Read<Transform>(ES3UserType_Transform.Instance);
												continue;
											}
										}
									}
									else if (text == "exitMountCar")
									{
										interactor.exitMountCar = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
								else if (num != 2024333248U)
								{
									if (num != 2042762818U)
									{
										if (num == 2056023484U)
										{
											if (text == "leftGlass")
											{
												interactor.leftGlass = reader.Read<GameObject>(ES3Type_GameObject.Instance);
												continue;
											}
										}
									}
									else if (text == "creeperMount")
									{
										interactor.creeperMount = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
								else if (text == "FRbonus")
								{
									interactor.FRbonus = reader.Read<float>(ES3Type_float.Instance);
									continue;
								}
							}
							else if (num <= 2171596256U)
							{
								if (num != 2145558064U)
								{
									if (num == 2171596256U)
									{
										if (text == "trailer")
										{
											interactor.trailer = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "mainPanelMgr")
								{
									interactor.mainPanelMgr = reader.Read<MainPanelManager>();
									continue;
								}
							}
							else if (num != 2172807873U)
							{
								if (num != 2194436059U)
								{
									if (num == 2265418348U)
									{
										if (text == "exitMountCarP")
										{
											interactor.exitMountCarP = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "FirstPersonCharacter")
								{
									interactor.FirstPersonCharacter = reader.Read<Camera>(ES3Type_Camera.Instance);
									continue;
								}
							}
							else if (text == "specialfab")
							{
								interactor.specialfab = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num <= 2388781508U)
						{
							if (num <= 2351998117U)
							{
								if (num != 2312174803U)
								{
									if (num == 2351998117U)
									{
										if (text == "carLightL")
										{
											interactor.carLightL = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "welderMachine")
								{
									interactor.welderMachine = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (num != 2352366190U)
							{
								if (num != 2370452323U)
								{
									if (num == 2388781508U)
									{
										if (text == "interactRange")
										{
											reader.SetPrivateField("interactRange", reader.Read<float>(), interactor);
											continue;
										}
									}
								}
								else if (text == "v8")
								{
									interactor.v8 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (text == "drivingCar")
							{
								interactor.drivingCar = reader.Read<bool>(ES3Type_bool.Instance);
								continue;
							}
						}
						else if (num <= 2481552184U)
						{
							if (num != 2401802238U)
							{
								if (num != 2468186291U)
								{
									if (num == 2481552184U)
									{
										if (text == "inv")
										{
											interactor.inv = reader.Read<InventoryItems>(ES3UserType_InventoryItems.Instance);
											continue;
										}
									}
								}
								else if (text == "trb")
								{
									interactor.trb = reader.Read<Rigidbody>(ES3UserType_Rigidbody.Instance);
									continue;
								}
							}
							else if (text == "toolTipPanel")
							{
								interactor.toolTipPanel = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num != 2483472042U)
						{
							if (num != 2487412015U)
							{
								if (num == 2518392755U)
								{
									if (text == "jiggs")
									{
										interactor.jiggs = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "discoveredMission")
							{
								interactor.discoveredMission = reader.Read<int>(ES3Type_int.Instance);
								continue;
							}
						}
						else if (text == "trailerGen")
						{
							interactor.trailerGen = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num <= 2771049362U)
					{
						if (num <= 2599732671U)
						{
							if (num <= 2552722063U)
							{
								if (num != 2519771727U)
								{
									if (num == 2552722063U)
									{
										if (text == "cart")
										{
											interactor.cart = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "racenpc")
								{
									interactor.racenpc = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (num != 2553899695U)
							{
								if (num != 2567131678U)
								{
									if (num == 2599732671U)
									{
										if (text == "mNeedle")
										{
											interactor.mNeedle = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "eventSystem")
								{
									interactor.eventSystem = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (text == "weldingFlash")
							{
								interactor.weldingFlash = reader.Read<ParticleSystem>(ES3UserType_ParticleSystem.Instance);
								continue;
							}
						}
						else if (num <= 2699108024U)
						{
							if (num != 2653995259U)
							{
								if (num != 2672819121U)
								{
									if (num == 2699108024U)
									{
										if (text == "pumpMachine")
										{
											interactor.pumpMachine = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "weldgun")
								{
									interactor.weldgun = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (text == "carLightR")
							{
								interactor.carLightR = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num != 2714137072U)
						{
							if (num != 2753063849U)
							{
								if (num == 2771049362U)
								{
									if (text == "truckLightRL")
									{
										interactor.truckLightRL = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "w4bonus")
							{
								interactor.w4bonus = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
						else if (text == "golfcartImpounded")
						{
							interactor.golfcartImpounded = reader.Read<bool>(ES3Type_bool.Instance);
							continue;
						}
					}
					else if (num <= 2875115047U)
					{
						if (num <= 2809798605U)
						{
							if (num != 2798989338U)
							{
								if (num == 2809798605U)
								{
									if (text == "timeOfDay")
									{
										interactor.timeOfDay = reader.Read<float>(ES3Type_float.Instance);
										continue;
									}
								}
							}
							else if (text == "exitMount")
							{
								interactor.exitMount = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num != 2819628244U)
						{
							if (num != 2869385467U)
							{
								if (num == 2875115047U)
								{
									if (text == "JarCratePrefab")
									{
										interactor.JarCratePrefab = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "myPCtower")
							{
								interactor.myPCtower = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (text == "escapePanel")
						{
							interactor.escapePanel = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num <= 2923437748U)
					{
						if (num != 2893824345U)
						{
							if (num != 2916299128U)
							{
								if (num == 2923437748U)
								{
									if (text == "radio")
									{
										interactor.radio = reader.Read<Radio>();
										continue;
									}
								}
							}
							else if (text == "fpc")
							{
								interactor.fpc = reader.Read<FirstPersonController>();
								continue;
							}
						}
						else if (text == "fpsHand")
						{
							interactor.fpsHand = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num != 2975121677U)
					{
						if (num != 2978308058U)
						{
							if (num == 2985344196U)
							{
								if (text == "canCrank")
								{
									interactor.canCrank = reader.Read<bool>(ES3Type_bool.Instance);
									continue;
								}
							}
						}
						else if (text == "hitchG")
						{
							interactor.hitchG = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (text == "winchHook")
					{
						interactor.winchHook = reader.Read<GameObject>(ES3Type_GameObject.Instance);
						continue;
					}
				}
				else if (num <= 3676266173U)
				{
					if (num <= 3306110889U)
					{
						if (num <= 3136244528U)
						{
							if (num <= 3072685577U)
							{
								if (num != 3049047885U)
								{
									if (num == 3072685577U)
									{
										if (text == "emissiveBolt")
										{
											interactor.emissiveBolt = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "creeper")
								{
									interactor.creeper = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (num != 3074238536U)
							{
								if (num != 3099157499U)
								{
									if (num == 3136244528U)
									{
										if (text == "jumpercables")
										{
											interactor.jumpercables = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "sleepScript")
								{
									interactor.sleepScript = reader.Read<SleepScript>();
									continue;
								}
							}
							else if (text == "recycleZoneOre")
							{
								interactor.recycleZoneOre = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num <= 3193038201U)
						{
							if (num != 3183435797U)
							{
								if (num == 3193038201U)
								{
									if (text == "playerArrow")
									{
										interactor.playerArrow = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "carLightRL")
							{
								interactor.carLightRL = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num != 3224526154U)
						{
							if (num != 3292945860U)
							{
								if (num == 3306110889U)
								{
									if (text == "winchObject")
									{
										interactor.winchObject = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "johnny1")
							{
								interactor.johnny1 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (text == "canRun")
						{
							interactor.canRun = reader.Read<bool>(ES3Type_bool.Instance);
							continue;
						}
					}
					else if (num <= 3540200659U)
					{
						if (num <= 3378353624U)
						{
							if (num != 3355610321U)
							{
								if (num == 3378353624U)
								{
									if (text == "wheelColRL")
									{
										interactor.wheelColRL = reader.Read<WheelCollider>();
										continue;
									}
								}
							}
							else if (text == "still")
							{
								interactor.still = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num != 3387273575U)
						{
							if (num != 3492327230U)
							{
								if (num == 3540200659U)
								{
									if (text == "tiregaugemeter")
									{
										interactor.tiregaugemeter = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "RLbonus")
							{
								interactor.RLbonus = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
						else if (text == "fpsHook")
						{
							interactor.fpsHook = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num <= 3613240290U)
					{
						if (num != 3549934208U)
						{
							if (num != 3568859314U)
							{
								if (num == 3613240290U)
								{
									if (text == "wheelColRR")
									{
										interactor.wheelColRR = reader.Read<WheelCollider>();
										continue;
									}
								}
							}
							else if (text == "raceActive")
							{
								interactor.raceActive = reader.Read<bool>(ES3Type_bool.Instance);
								continue;
							}
						}
						else if (text == "person")
						{
							interactor.person = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num != 3670804686U)
					{
						if (num != 3674060525U)
						{
							if (num == 3676266173U)
							{
								if (text == "leanDest")
								{
									interactor.leanDest = reader.Read<Transform>(ES3UserType_Transform.Instance);
									continue;
								}
							}
						}
						else if (text == "nokia")
						{
							interactor.nokia = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (text == "myPC")
					{
						interactor.myPC = reader.Read<GameObject>(ES3Type_GameObject.Instance);
						continue;
					}
				}
				else if (num <= 3952804990U)
				{
					if (num <= 3840100737U)
					{
						if (num <= 3723658806U)
						{
							if (num != 3703074533U)
							{
								if (num == 3723658806U)
								{
									if (text == "currency")
									{
										interactor.currency = reader.Read<Currency>();
										continue;
									}
								}
							}
							else if (text == "businessman")
							{
								interactor.businessman = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num != 3775336775U)
						{
							if (num != 3831658230U)
							{
								if (num == 3840100737U)
								{
									if (text == "EngStand")
									{
										interactor.EngStand = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "leanDestO")
							{
								interactor.leanDestO = reader.Read<Transform>(ES3UserType_Transform.Instance);
								continue;
							}
						}
						else if (text == "fourcyl")
						{
							interactor.fourcyl = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num <= 3885672460U)
					{
						if (num != 3844534447U)
						{
							if (num != 3852863908U)
							{
								if (num == 3885672460U)
								{
									if (text == "oilBolt")
									{
										interactor.oilBolt = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "RRbonus")
							{
								interactor.RRbonus = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
						else if (text == "newRadius")
						{
							interactor.newRadius = reader.Read<double>(ES3Type_double.Instance);
							continue;
						}
					}
					else if (num != 3896935954U)
					{
						if (num != 3931080830U)
						{
							if (num == 3952804990U)
							{
								if (text == "exitMountP")
								{
									interactor.exitMountP = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
						}
						else if (text == "weatherId")
						{
							reader.SetPrivateField("weatherId", reader.Read<int>(), interactor);
							continue;
						}
					}
					else if (text == "tiregauge")
					{
						interactor.tiregauge = reader.Read<GameObject>(ES3Type_GameObject.Instance);
						continue;
					}
				}
				else if (num <= 4036580033U)
				{
					if (num <= 3984050950U)
					{
						if (num != 3968918830U)
						{
							if (num == 3984050950U)
							{
								if (text == "cameraTransform")
								{
									interactor.cameraTransform = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
						}
						else if (text == "mail")
						{
							interactor.mail = reader.Read<MailScript>(ES3UserType_MailScript.Instance);
							continue;
						}
					}
					else if (num != 3987878185U)
					{
						if (num != 4000743041U)
						{
							if (num == 4036580033U)
							{
								if (text == "pokerScript")
								{
									interactor.pokerScript = reader.Read<poker>();
									continue;
								}
							}
						}
						else if (text == "fpsRatchet")
						{
							interactor.fpsRatchet = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (text == "holdingHoseTank")
					{
						interactor.holdingHoseTank = reader.Read<bool>(ES3Type_bool.Instance);
						continue;
					}
				}
				else if (num <= 4169274880U)
				{
					if (num != 4071960168U)
					{
						if (num != 4114791880U)
						{
							if (num == 4169274880U)
							{
								if (text == "recycleZone")
								{
									interactor.recycleZone = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
						}
						else if (text == "truck")
						{
							interactor.truck = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (text == "ucarAudio")
					{
						interactor.ucarAudio = reader.Read<GameObject>(ES3Type_GameObject.Instance);
						continue;
					}
				}
				else if (num != 4213706897U)
				{
					if (num != 4245426262U)
					{
						if (num == 4290851354U)
						{
							if (text == "newageGirl")
							{
								interactor.newageGirl = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
					}
					else if (text == "RaceTargets")
					{
						interactor.RaceTargets = reader.Read<Transform>(ES3UserType_Transform.Instance);
						continue;
					}
				}
				else if (text == "f100Impounded")
				{
					interactor.f100Impounded = reader.Read<bool>(ES3Type_bool.Instance);
					continue;
				}
				reader.Skip();
			}
		}

		// Token: 0x04002452 RID: 9298
		public static ES3Type Instance;
	}
}
