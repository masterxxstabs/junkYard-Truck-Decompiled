using System;
using UnityEngine;
using UnityEngine.Scripting;
using UnityStandardAssets.Characters.FirstPerson;

namespace ES3Types
{
	// Token: 0x020002FA RID: 762
	[Preserve]
	[ES3Properties(new string[]
	{
		"FPS",
		"fixedRotation",
		"attachTo",
		"attachTo2",
		"attachTo3",
		"pickable",
		"price",
		"tradein",
		"description",
		"shoulderCarry",
		"numBolts",
		"thisDurability",
		"template",
		"canDetach",
		"coolantBolt",
		"holding",
		"controller",
		"interactWhileHolding",
		"deflation",
		"deflateOffset",
		"trueMass",
		"partType",
		"tireNum",
		"rimNum",
		"isAllowedToTrigger",
		"mat",
		"mat2",
		"noMat",
		"missionObj",
		"painted",
		"red",
		"green",
		"blue",
		"metallic",
		"smoothness",
		"paintSlot",
		"enabled",
		"name"
	})]
	public class ES3UserType_PickUp : ES3ComponentType
	{
		// Token: 0x060013FA RID: 5114 RVA: 0x000D3111 File Offset: 0x000D1311
		public ES3UserType_PickUp() : base(typeof(PickUp))
		{
			ES3UserType_PickUp.Instance = this;
			this.priority = 1;
		}

		// Token: 0x060013FB RID: 5115 RVA: 0x000D3130 File Offset: 0x000D1330
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			PickUp pickUp = (PickUp)obj;
			writer.WritePropertyByRef("FPS", pickUp.FPS);
			writer.WriteProperty("fixedRotation", pickUp.fixedRotation, ES3Type_float.Instance);
			writer.WriteProperty("attachTo", pickUp.attachTo, ES3Type_string.Instance);
			writer.WriteProperty("attachTo2", pickUp.attachTo2, ES3Type_string.Instance);
			writer.WriteProperty("attachTo3", pickUp.attachTo3, ES3Type_string.Instance);
			writer.WriteProperty("pickable", pickUp.pickable, ES3Type_bool.Instance);
			writer.WriteProperty("price", pickUp.price, ES3Type_float.Instance);
			writer.WriteProperty("tradein", pickUp.tradein, ES3Type_float.Instance);
			writer.WriteProperty("description", pickUp.description, ES3Type_string.Instance);
			writer.WriteProperty("shoulderCarry", pickUp.shoulderCarry, ES3Type_bool.Instance);
			writer.WriteProperty("numBolts", pickUp.numBolts, ES3Type_float.Instance);
			writer.WriteProperty("thisDurability", pickUp.thisDurability, ES3Type_float.Instance);
			writer.WritePropertyByRef("template", pickUp.template);
			writer.WriteProperty("canDetach", pickUp.canDetach, ES3Type_bool.Instance);
			writer.WritePropertyByRef("coolantBolt", pickUp.coolantBolt);
			writer.WriteProperty("holding", pickUp.holding, ES3Type_bool.Instance);
			writer.WritePropertyByRef("controller", pickUp.controller);
			writer.WriteProperty("interactWhileHolding", pickUp.interactWhileHolding, ES3Type_bool.Instance);
			writer.WriteProperty("deflation", pickUp.deflation, ES3Type_float.Instance);
			writer.WriteProperty("deflateOffset", pickUp.deflateOffset, ES3Type_float.Instance);
			writer.WriteProperty("trueMass", pickUp.trueMass, ES3Type_float.Instance);
			writer.WriteProperty("partType", pickUp.partType, ES3Type_int.Instance);
			writer.WritePrivateField("tireNum", pickUp);
			writer.WritePrivateField("rimNum", pickUp);
			writer.WriteProperty("isAllowedToTrigger", pickUp.isAllowedToTrigger, ES3Type_bool.Instance);
			writer.WritePropertyByRef("mat", pickUp.mat);
			writer.WritePropertyByRef("mat2", pickUp.mat2);
			writer.WriteProperty("noMat", pickUp.noMat, ES3Type_bool.Instance);
			writer.WritePrivateFieldByRef("missionObj", pickUp);
			writer.WriteProperty("painted", pickUp.painted, ES3Type_bool.Instance);
			writer.WriteProperty("red", pickUp.red, ES3Type_float.Instance);
			writer.WriteProperty("green", pickUp.green, ES3Type_float.Instance);
			writer.WriteProperty("blue", pickUp.blue, ES3Type_float.Instance);
			writer.WriteProperty("metallic", pickUp.metallic, ES3Type_float.Instance);
			writer.WriteProperty("smoothness", pickUp.smoothness, ES3Type_float.Instance);
			writer.WriteProperty("paintSlot", pickUp.paintSlot, ES3Type_int.Instance);
			writer.WriteProperty("enabled", pickUp.enabled, ES3Type_bool.Instance);
		}

		// Token: 0x060013FC RID: 5116 RVA: 0x000D34B0 File Offset: 0x000D16B0
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			PickUp pickUp = (PickUp)obj;
			foreach (object obj2 in reader.Properties)
			{
				string text = (string)obj2;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 1428658606U)
				{
					if (num <= 403313192U)
					{
						if (num <= 18738364U)
						{
							if (num <= 9942810U)
							{
								if (num != 3091071U)
								{
									if (num == 9942810U)
									{
										if (text == "attachTo3")
										{
											pickUp.attachTo3 = reader.Read<string>(ES3Type_string.Instance);
											continue;
										}
									}
								}
								else if (text == "paintSlot")
								{
									pickUp.paintSlot = reader.Read<int>(ES3Type_int.Instance);
									continue;
								}
							}
							else if (num != 11362760U)
							{
								if (num == 18738364U)
								{
									if (text == "green")
									{
										pickUp.green = reader.Read<float>(ES3Type_float.Instance);
										continue;
									}
								}
							}
							else if (text == "FPS")
							{
								pickUp.FPS = reader.Read<Transform>(ES3UserType_Transform.Instance);
								continue;
							}
						}
						else if (num <= 49525662U)
						{
							if (num != 26720429U)
							{
								if (num == 49525662U)
								{
									if (text == "enabled")
									{
										pickUp.enabled = reader.Read<bool>(ES3Type_bool.Instance);
										continue;
									}
								}
							}
							else if (text == "attachTo2")
							{
								pickUp.attachTo2 = reader.Read<string>(ES3Type_string.Instance);
								continue;
							}
						}
						else if (num != 95537075U)
						{
							if (num != 189023530U)
							{
								if (num == 403313192U)
								{
									if (text == "shoulderCarry")
									{
										pickUp.shoulderCarry = reader.Read<bool>(ES3Type_bool.Instance);
										continue;
									}
								}
							}
							else if (text == "price")
							{
								pickUp.price = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
						else if (text == "interactWhileHolding")
						{
							pickUp.interactWhileHolding = reader.Read<bool>(ES3Type_bool.Instance);
							continue;
						}
					}
					else if (num <= 753921233U)
					{
						if (num <= 504290566U)
						{
							if (num != 448459053U)
							{
								if (num == 504290566U)
								{
									if (text == "isAllowedToTrigger")
									{
										pickUp.isAllowedToTrigger = reader.Read<bool>(ES3Type_bool.Instance);
										continue;
									}
								}
							}
							else if (text == "deflation")
							{
								pickUp.deflation = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
						else if (num != 680064221U)
						{
							if (num == 753921233U)
							{
								if (text == "trueMass")
								{
									pickUp.trueMass = reader.Read<float>(ES3Type_float.Instance);
									continue;
								}
							}
						}
						else if (text == "mat2")
						{
							pickUp.mat2 = reader.Read<Material>(ES3Type_Material.Instance);
							continue;
						}
					}
					else if (num <= 925808028U)
					{
						if (num != 879704937U)
						{
							if (num == 925808028U)
							{
								if (text == "partType")
								{
									pickUp.partType = reader.Read<int>(ES3Type_int.Instance);
									continue;
								}
							}
						}
						else if (text == "description")
						{
							pickUp.description = reader.Read<string>(ES3Type_string.Instance);
							continue;
						}
					}
					else if (num != 1089765596U)
					{
						if (num != 1132037443U)
						{
							if (num == 1428658606U)
							{
								if (text == "coolantBolt")
								{
									pickUp.coolantBolt = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
						}
						else if (text == "deflateOffset")
						{
							pickUp.deflateOffset = reader.Read<float>(ES3Type_float.Instance);
							continue;
						}
					}
					else if (text == "red")
					{
						pickUp.red = reader.Read<float>(ES3Type_float.Instance);
						continue;
					}
				}
				else if (num <= 2728010544U)
				{
					if (num <= 2197550541U)
					{
						if (num <= 2024658632U)
						{
							if (num != 1766500875U)
							{
								if (num == 2024658632U)
								{
									if (text == "metallic")
									{
										pickUp.metallic = reader.Read<float>(ES3Type_float.Instance);
										continue;
									}
								}
							}
							else if (text == "template")
							{
								pickUp.template = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (num != 2133703742U)
						{
							if (num == 2197550541U)
							{
								if (text == "blue")
								{
									pickUp.blue = reader.Read<float>(ES3Type_float.Instance);
									continue;
								}
							}
						}
						else if (text == "missionObj")
						{
							reader.SetPrivateField("missionObj", reader.Read<GameObject>(), pickUp);
							continue;
						}
					}
					else if (num <= 2491547344U)
					{
						if (num != 2427238402U)
						{
							if (num == 2491547344U)
							{
								if (text == "holding")
								{
									pickUp.holding = reader.Read<bool>(ES3Type_bool.Instance);
									continue;
								}
							}
						}
						else if (text == "thisDurability")
						{
							pickUp.thisDurability = reader.Read<float>(ES3Type_float.Instance);
							continue;
						}
					}
					else if (num != 2648353633U)
					{
						if (num != 2656486541U)
						{
							if (num == 2728010544U)
							{
								if (text == "smoothness")
								{
									pickUp.smoothness = reader.Read<float>(ES3Type_float.Instance);
									continue;
								}
							}
						}
						else if (text == "attachTo")
						{
							pickUp.attachTo = reader.Read<string>(ES3Type_string.Instance);
							continue;
						}
					}
					else if (text == "tireNum")
					{
						reader.SetPrivateField("tireNum", reader.Read<int>(), pickUp);
						continue;
					}
				}
				else if (num <= 3512512370U)
				{
					if (num <= 2911323545U)
					{
						if (num != 2855714522U)
						{
							if (num == 2911323545U)
							{
								if (text == "rimNum")
								{
									reader.SetPrivateField("rimNum", reader.Read<int>(), pickUp);
									continue;
								}
							}
						}
						else if (text == "tradein")
						{
							pickUp.tradein = reader.Read<float>(ES3Type_float.Instance);
							continue;
						}
					}
					else if (num != 3026529409U)
					{
						if (num != 3171715634U)
						{
							if (num == 3512512370U)
							{
								if (text == "painted")
								{
									pickUp.painted = reader.Read<bool>(ES3Type_bool.Instance);
									continue;
								}
							}
						}
						else if (text == "canDetach")
						{
							pickUp.canDetach = reader.Read<bool>(ES3Type_bool.Instance);
							continue;
						}
					}
					else if (text == "controller")
					{
						pickUp.controller = reader.Read<FirstPersonController>();
						continue;
					}
				}
				else if (num <= 3871761072U)
				{
					if (num != 3819107837U)
					{
						if (num == 3871761072U)
						{
							if (text == "pickable")
							{
								pickUp.pickable = reader.Read<bool>(ES3Type_bool.Instance);
								continue;
							}
						}
					}
					else if (text == "mat")
					{
						pickUp.mat = reader.Read<Material>(ES3Type_Material.Instance);
						continue;
					}
				}
				else if (num != 3982857272U)
				{
					if (num != 4181813077U)
					{
						if (num == 4261127163U)
						{
							if (text == "fixedRotation")
							{
								pickUp.fixedRotation = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
					}
					else if (text == "numBolts")
					{
						pickUp.numBolts = reader.Read<float>(ES3Type_float.Instance);
						continue;
					}
				}
				else if (text == "noMat")
				{
					pickUp.noMat = reader.Read<bool>(ES3Type_bool.Instance);
					continue;
				}
				reader.Skip();
			}
		}

		// Token: 0x04002466 RID: 9318
		public static ES3Type Instance;
	}
}
