using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x020002E8 RID: 744
	[Preserve]
	[ES3Properties(new string[]
	{
		"item0",
		"item1",
		"item2",
		"item3",
		"item4",
		"item5",
		"cashSlot0",
		"cashSlot1",
		"cashSlot2",
		"cashSlot3",
		"cashSlot4",
		"cashSlot5",
		"moneyVal"
	})]
	public class ES3UserType_InventoryItems : ES3ComponentType
	{
		// Token: 0x060013CD RID: 5069 RVA: 0x000D0805 File Offset: 0x000CEA05
		public ES3UserType_InventoryItems() : base(typeof(InventoryItems))
		{
			ES3UserType_InventoryItems.Instance = this;
			this.priority = 1;
		}

		// Token: 0x060013CE RID: 5070 RVA: 0x000D0824 File Offset: 0x000CEA24
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			InventoryItems inventoryItems = (InventoryItems)obj;
			writer.WritePropertyByRef("item0", inventoryItems.item0);
			writer.WritePropertyByRef("item1", inventoryItems.item1);
			writer.WritePropertyByRef("item2", inventoryItems.item2);
			writer.WritePropertyByRef("item3", inventoryItems.item3);
			writer.WritePropertyByRef("item4", inventoryItems.item4);
			writer.WritePropertyByRef("item5", inventoryItems.item5);
			writer.WriteProperty("cashSlot0", inventoryItems.cashSlot0, ES3Type_float.Instance);
			writer.WriteProperty("cashSlot1", inventoryItems.cashSlot1, ES3Type_float.Instance);
			writer.WriteProperty("cashSlot2", inventoryItems.cashSlot2, ES3Type_float.Instance);
			writer.WriteProperty("cashSlot3", inventoryItems.cashSlot3, ES3Type_float.Instance);
			writer.WriteProperty("cashSlot4", inventoryItems.cashSlot4, ES3Type_float.Instance);
			writer.WriteProperty("cashSlot5", inventoryItems.cashSlot5, ES3Type_float.Instance);
			writer.WriteProperty("moneyVal", inventoryItems.moneyVal, ES3Type_float.Instance);
		}

		// Token: 0x060013CF RID: 5071 RVA: 0x000D095C File Offset: 0x000CEB5C
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			InventoryItems inventoryItems = (InventoryItems)obj;
			foreach (object obj2 in reader.Properties)
			{
				string text = (string)obj2;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 2071566818U)
				{
					if (num <= 2021233961U)
					{
						if (num != 661346438U)
						{
							if (num != 2004456342U)
							{
								if (num == 2021233961U)
								{
									if (text == "item5")
									{
										inventoryItems.item5 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
										continue;
									}
								}
							}
							else if (text == "item4")
							{
								inventoryItems.item4 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
								continue;
							}
						}
						else if (text == "moneyVal")
						{
							inventoryItems.moneyVal = reader.Read<float>(ES3Type_float.Instance);
							continue;
						}
					}
					else if (num != 2038011580U)
					{
						if (num != 2054789199U)
						{
							if (num == 2071566818U)
							{
								if (text == "item0")
								{
									inventoryItems.item0 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
									continue;
								}
							}
						}
						else if (text == "item3")
						{
							inventoryItems.item3 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
							continue;
						}
					}
					else if (text == "item2")
					{
						inventoryItems.item2 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
						continue;
					}
				}
				else if (num <= 3063513753U)
				{
					if (num != 2088344437U)
					{
						if (num != 3046736134U)
						{
							if (num == 3063513753U)
							{
								if (text == "cashSlot5")
								{
									inventoryItems.cashSlot5 = reader.Read<float>(ES3Type_float.Instance);
									continue;
								}
							}
						}
						else if (text == "cashSlot4")
						{
							inventoryItems.cashSlot4 = reader.Read<float>(ES3Type_float.Instance);
							continue;
						}
					}
					else if (text == "item1")
					{
						inventoryItems.item1 = reader.Read<GameObject>(ES3Type_GameObject.Instance);
						continue;
					}
				}
				else if (num <= 3097068991U)
				{
					if (num != 3080291372U)
					{
						if (num == 3097068991U)
						{
							if (text == "cashSlot3")
							{
								inventoryItems.cashSlot3 = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
					}
					else if (text == "cashSlot2")
					{
						inventoryItems.cashSlot2 = reader.Read<float>(ES3Type_float.Instance);
						continue;
					}
				}
				else if (num != 3113846610U)
				{
					if (num == 3130624229U)
					{
						if (text == "cashSlot1")
						{
							inventoryItems.cashSlot1 = reader.Read<float>(ES3Type_float.Instance);
							continue;
						}
					}
				}
				else if (text == "cashSlot0")
				{
					inventoryItems.cashSlot0 = reader.Read<float>(ES3Type_float.Instance);
					continue;
				}
				reader.Skip();
			}
		}

		// Token: 0x04002454 RID: 9300
		public static ES3Type Instance;
	}
}
