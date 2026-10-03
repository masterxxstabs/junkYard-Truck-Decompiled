using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x020002EA RID: 746
	[Preserve]
	[ES3Properties(new string[]
	{
		"moonshineSphere",
		"aSource",
		"audioLoc",
		"missionNum",
		"clip1",
		"clip2",
		"clip3",
		"clip4",
		"pointer1",
		"pointer2",
		"pointer3",
		"pointer3b",
		"pointer4",
		"pointer5",
		"deliveryZone1",
		"deliveryZone2",
		"deliveryZone3",
		"deliveryZone4",
		"eventSystem",
		"isBusy",
		"mc",
		"interactor",
		"wheel1",
		"wheel2",
		"wheel3",
		"wheel4",
		"washingmachine",
		"revengeCar",
		"revengeCarInlet",
		"revengeCarBolt",
		"abomb",
		"jiggs",
		"boughtTires",
		"moonTank",
		"moonBag",
		"furniture1",
		"furniture2",
		"furniture3",
		"furnitureLoc1",
		"furnitureLoc2",
		"furnitureLoc3",
		"spawnedFurniture",
		"moneyLoc",
		"moneyRoll"
	})]
	public class ES3UserType_Jake : ES3ComponentType
	{
		// Token: 0x060013D2 RID: 5074 RVA: 0x000D0CD1 File Offset: 0x000CEED1
		public ES3UserType_Jake() : base(typeof(Jake))
		{
			ES3UserType_Jake.Instance = this;
			this.priority = 1;
		}

		// Token: 0x060013D3 RID: 5075 RVA: 0x000D0CF0 File Offset: 0x000CEEF0
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			Jake jake = (Jake)obj;
			writer.WritePropertyByRef("moonshineSphere", jake.moonshineSphere);
			writer.WritePropertyByRef("aSource", jake.aSource);
			writer.WritePropertyByRef("audioLoc", jake.audioLoc);
			writer.WriteProperty("missionNum", jake.missionNum, ES3Type_int.Instance);
			writer.WritePropertyByRef("clip1", jake.clip1);
			writer.WritePropertyByRef("clip2", jake.clip2);
			writer.WritePropertyByRef("clip3", jake.clip3);
			writer.WritePropertyByRef("clip4", jake.clip4);
			writer.WritePropertyByRef("pointer1", jake.pointer1);
			writer.WritePropertyByRef("pointer2", jake.pointer2);
			writer.WritePropertyByRef("pointer3", jake.pointer3);
			writer.WritePropertyByRef("pointer3b", jake.pointer3b);
			writer.WritePropertyByRef("pointer4", jake.pointer4);
			writer.WritePropertyByRef("pointer5", jake.pointer5);
			writer.WritePropertyByRef("deliveryZone1", jake.deliveryZone1);
			writer.WritePropertyByRef("deliveryZone2", jake.deliveryZone2);
			writer.WritePropertyByRef("deliveryZone3", jake.deliveryZone3);
			writer.WritePropertyByRef("deliveryZone4", jake.deliveryZone4);
			writer.WritePropertyByRef("eventSystem", jake.eventSystem);
			writer.WriteProperty("isBusy", jake.isBusy, ES3Type_bool.Instance);
			writer.WritePropertyByRef("mc", jake.mc);
			writer.WritePropertyByRef("interactor", jake.interactor);
			writer.WritePropertyByRef("wheel1", jake.wheel1);
			writer.WritePropertyByRef("wheel2", jake.wheel2);
			writer.WritePropertyByRef("wheel3", jake.wheel3);
			writer.WritePropertyByRef("wheel4", jake.wheel4);
			writer.WritePropertyByRef("washingmachine", jake.washingmachine);
			writer.WritePropertyByRef("revengeCar", jake.revengeCar);
			writer.WritePropertyByRef("revengeCarInlet", jake.revengeCarInlet);
			writer.WritePropertyByRef("revengeCarBolt", jake.revengeCarBolt);
			writer.WritePropertyByRef("abomb", jake.abomb);
			writer.WritePropertyByRef("jiggs", jake.jiggs);
			writer.WriteProperty("boughtTires", jake.boughtTires, ES3Type_int.Instance);
			writer.WriteProperty("moonTank", jake.moonTank, ES3Type_bool.Instance);
			writer.WriteProperty("moonBag", jake.moonBag, ES3Type_bool.Instance);
			writer.WritePropertyByRef("furniture1", jake.furniture1);
			writer.WritePropertyByRef("furniture2", jake.furniture2);
			writer.WritePropertyByRef("furniture3", jake.furniture3);
			writer.WritePropertyByRef("furnitureLoc1", jake.furnitureLoc1);
			writer.WritePropertyByRef("furnitureLoc2", jake.furnitureLoc2);
			writer.WritePropertyByRef("furnitureLoc3", jake.furnitureLoc3);
			writer.WritePrivateField("spawnedFurniture", jake);
			writer.WritePropertyByRef("moneyLoc", jake.moneyLoc);
			writer.WritePropertyByRef("moneyRoll", jake.moneyRoll);
		}

		// Token: 0x060013D4 RID: 5076 RVA: 0x000D1020 File Offset: 0x000CF220
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			Jake jake = (Jake)obj;
			foreach (object obj2 in reader.Properties)
			{
				string text = (string)obj2;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 1865927281U)
				{
					if (num <= 1191096124U)
					{
						if (num <= 480768262U)
						{
							if (num <= 135290811U)
							{
								if (num != 15166377U)
								{
									if (num == 135290811U)
									{
										if (text == "pointer3b")
										{
											jake.pointer3b = reader.Read<showmission>();
											continue;
										}
									}
								}
								else if (text == "boughtTires")
								{
									jake.boughtTires = reader.Read<int>(ES3Type_int.Instance);
									continue;
								}
							}
							else if (num != 272309904U)
							{
								if (num != 373080187U)
								{
									if (num == 480768262U)
									{
										if (text == "moonBag")
										{
											jake.moonBag = reader.Read<bool>(ES3Type_bool.Instance);
											continue;
										}
									}
								}
								else if (text == "revengeCarInlet")
								{
									jake.revengeCarInlet = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (text == "moneyRoll")
							{
								jake.moneyRoll = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num <= 808380087U)
						{
							if (num != 634227712U)
							{
								if (num != 724491992U)
								{
									if (num == 808380087U)
									{
										if (text == "wheel1")
										{
											jake.wheel1 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
											continue;
										}
									}
								}
								else if (text == "wheel4")
								{
									jake.wheel4 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
							else if (text == "moonTank")
							{
								jake.moonTank = reader.Read<bool>(ES3Type_bool.Instance);
								continue;
							}
						}
						else if (num != 825157706U)
						{
							if (num != 841935325U)
							{
								if (num == 1191096124U)
								{
									if (text == "furniture1")
									{
										jake.furniture1 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "wheel3")
							{
								jake.wheel3 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (text == "wheel2")
						{
							jake.wheel2 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num <= 1246471553U)
					{
						if (num <= 1212916315U)
						{
							if (num != 1196138696U)
							{
								if (num == 1212916315U)
								{
									if (text == "pointer3")
									{
										jake.pointer3 = reader.Read<showmission>();
										continue;
									}
								}
							}
							else if (text == "pointer2")
							{
								jake.pointer2 = reader.Read<showmission>();
								continue;
							}
						}
						else if (num != 1224651362U)
						{
							if (num != 1241428981U)
							{
								if (num == 1246471553U)
								{
									if (text == "pointer1")
									{
										jake.pointer1 = reader.Read<showmission>();
										continue;
									}
								}
							}
							else if (text == "furniture2")
							{
								jake.furniture2 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (text == "furniture3")
						{
							jake.furniture3 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num <= 1310127275U)
					{
						if (num != 1264464778U)
						{
							if (num != 1296804410U)
							{
								if (num == 1310127275U)
								{
									if (text == "washingmachine")
									{
										jake.washingmachine = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "pointer4")
							{
								jake.pointer4 = reader.Read<showmission>();
								continue;
							}
						}
						else if (text == "moonshineSphere")
						{
							jake.moonshineSphere = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num != 1313582029U)
					{
						if (num != 1714300801U)
						{
							if (num == 1865927281U)
							{
								if (text == "spawnedFurniture")
								{
									reader.SetPrivateField("spawnedFurniture", reader.Read<bool>(), jake);
									continue;
								}
							}
						}
						else if (text == "mc")
						{
							jake.mc = reader.Read<MissionController>();
							continue;
						}
					}
					else if (text == "pointer5")
					{
						jake.pointer5 = reader.Read<showmission>();
						continue;
					}
				}
				else if (num <= 2393098238U)
				{
					if (num <= 2146150212U)
					{
						if (num <= 2111219545U)
						{
							if (num != 2076092721U)
							{
								if (num == 2111219545U)
								{
									if (text == "deliveryZone4")
									{
										jake.deliveryZone4 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "revengeCar")
							{
								jake.revengeCar = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num != 2127997164U)
						{
							if (num != 2144774783U)
							{
								if (num == 2146150212U)
								{
									if (text == "furnitureLoc1")
									{
										jake.furnitureLoc1 = reader.Read<Transform>(ES3UserType_Transform.Instance);
										continue;
									}
								}
							}
							else if (text == "deliveryZone2")
							{
								jake.deliveryZone2 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (text == "deliveryZone3")
						{
							jake.deliveryZone3 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num <= 2196483069U)
					{
						if (num != 2161552402U)
						{
							if (num != 2179705450U)
							{
								if (num == 2196483069U)
								{
									if (text == "furnitureLoc2")
									{
										jake.furnitureLoc2 = reader.Read<Transform>(ES3UserType_Transform.Instance);
										continue;
									}
								}
							}
							else if (text == "furnitureLoc3")
							{
								jake.furnitureLoc3 = reader.Read<Transform>(ES3UserType_Transform.Instance);
								continue;
							}
						}
						else if (text == "deliveryZone1")
						{
							jake.deliveryZone1 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (num != 2359543000U)
					{
						if (num != 2376320619U)
						{
							if (num == 2393098238U)
							{
								if (text == "clip1")
								{
									jake.clip1 = reader.Read<AudioClip>(ES3Type_AudioClip.Instance);
									continue;
								}
							}
						}
						else if (text == "clip2")
						{
							jake.clip2 = reader.Read<AudioClip>(ES3Type_AudioClip.Instance);
							continue;
						}
					}
					else if (text == "clip3")
					{
						jake.clip3 = reader.Read<AudioClip>(ES3Type_AudioClip.Instance);
						continue;
					}
				}
				else if (num <= 2863242192U)
				{
					if (num <= 2518392755U)
					{
						if (num != 2476986333U)
						{
							if (num == 2518392755U)
							{
								if (text == "jiggs")
								{
									jake.jiggs = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
						}
						else if (text == "clip4")
						{
							jake.clip4 = reader.Read<AudioClip>(ES3Type_AudioClip.Instance);
							continue;
						}
					}
					else if (num != 2567131678U)
					{
						if (num != 2731742166U)
						{
							if (num == 2863242192U)
							{
								if (text == "isBusy")
								{
									jake.isBusy = reader.Read<bool>(ES3Type_bool.Instance);
									continue;
								}
							}
						}
						else if (text == "interactor")
						{
							jake.interactor = reader.Read<Interactor>();
							continue;
						}
					}
					else if (text == "eventSystem")
					{
						jake.eventSystem = reader.Read<GameObject>(ES3Type_GameObject.Instance);
						continue;
					}
				}
				else if (num <= 3925544682U)
				{
					if (num != 3040391196U)
					{
						if (num != 3207676435U)
						{
							if (num == 3925544682U)
							{
								if (text == "revengeCarBolt")
								{
									jake.revengeCarBolt = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
						}
						else if (text == "moneyLoc")
						{
							jake.moneyLoc = reader.Read<Transform>(ES3UserType_Transform.Instance);
							continue;
						}
					}
					else if (text == "abomb")
					{
						jake.abomb = reader.Read<GameObject>(ES3Type_GameObject.Instance);
						continue;
					}
				}
				else if (num != 3956593849U)
				{
					if (num != 4008009335U)
					{
						if (num == 4071080281U)
						{
							if (text == "aSource")
							{
								jake.aSource = reader.Read<AudioSource>();
								continue;
							}
						}
					}
					else if (text == "missionNum")
					{
						jake.missionNum = reader.Read<int>(ES3Type_int.Instance);
						continue;
					}
				}
				else if (text == "audioLoc")
				{
					jake.audioLoc = reader.Read<Transform>(ES3UserType_Transform.Instance);
					continue;
				}
				reader.Skip();
			}
		}

		// Token: 0x04002456 RID: 9302
		public static ES3Type Instance;
	}
}
