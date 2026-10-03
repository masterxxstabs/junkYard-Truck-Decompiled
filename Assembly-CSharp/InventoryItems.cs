using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000D2 RID: 210
public class InventoryItems : MonoBehaviour
{
	// Token: 0x06000542 RID: 1346 RVA: 0x0003FC64 File Offset: 0x0003DE64
	public void Start()
	{
		this.OpenCheck();
	}

	// Token: 0x06000543 RID: 1347 RVA: 0x0003FC6C File Offset: 0x0003DE6C
	public void OpenCheck()
	{
		this.i = 0;
		this.item0 = null;
		this.item1 = null;
		this.item2 = null;
		this.item3 = null;
		this.item4 = null;
		this.item5 = null;
		this.cashSlot0 = 0f;
		this.cashSlot1 = 0f;
		this.cashSlot2 = 0f;
		this.cashSlot3 = 0f;
		this.cashSlot4 = 0f;
		this.cashSlot5 = 0f;
		this.slot0.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
		this.slot0.transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
		this.slot0.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = false;
		this.slot1.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
		this.slot1.transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
		this.slot1.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = false;
		this.slot2.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
		this.slot2.transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
		this.slot2.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = false;
		this.slot3.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
		this.slot3.transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
		this.slot3.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = false;
		this.slot4.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
		this.slot4.transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
		this.slot4.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = false;
		this.slot5.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
		this.slot5.transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
		this.slot5.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = false;
		foreach (object obj in this.theDest)
		{
			Transform transform = (Transform)obj;
			if (!transform.gameObject.active)
			{
				if (this.i == 0)
				{
					this.item0 = transform.gameObject;
				}
				if (this.i == 1)
				{
					this.item1 = transform.gameObject;
				}
				if (this.i == 2)
				{
					this.item2 = transform.gameObject;
				}
				if (this.i == 3)
				{
					this.item3 = transform.gameObject;
				}
				if (this.i == 4)
				{
					this.item4 = transform.gameObject;
				}
				if (this.i == 5)
				{
					this.item5 = transform.gameObject;
				}
				this.i++;
			}
		}
		if (this.item0 != null)
		{
			if (this.item0.name.Substring(0, 4) == "Cash")
			{
				this.cashSlot0 = this.item0.GetComponent<PickUp>().thisDurability;
				this.slot0.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$" + this.cashSlot0;
				this.slot0.transform.GetChild(2).gameObject.GetComponent<RawImage>().texture = this.moneyTex;
				this.slot0.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = true;
			}
			else
			{
				this.cashSlot0 = 0f;
				string description = this.item0.GetComponent<PickUp>().description;
				int iconNum = this.GetIconNum(description);
				this.slot0.transform.GetChild(2).gameObject.GetComponent<RawImage>().texture = this.consumables[iconNum];
				this.slot0.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = true;
				this.slot0.transform.GetChild(0).gameObject.GetComponent<Text>().text = description;
			}
		}
		if (this.item1 != null)
		{
			if (this.item1.name.Substring(0, 4) == "Cash")
			{
				this.cashSlot1 = this.item1.GetComponent<PickUp>().thisDurability;
				this.slot1.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$" + this.cashSlot1;
				this.slot1.transform.GetChild(2).gameObject.GetComponent<RawImage>().texture = this.moneyTex;
				this.slot1.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = true;
			}
			else
			{
				this.cashSlot1 = 0f;
				string description2 = this.item1.GetComponent<PickUp>().description;
				int iconNum2 = this.GetIconNum(description2);
				this.slot1.transform.GetChild(2).gameObject.GetComponent<RawImage>().texture = this.consumables[iconNum2];
				this.slot1.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = true;
				this.slot1.transform.GetChild(0).gameObject.GetComponent<Text>().text = description2;
			}
		}
		if (this.item2 != null)
		{
			if (this.item2.name.Substring(0, 4) == "Cash")
			{
				this.cashSlot2 = this.item2.GetComponent<PickUp>().thisDurability;
				this.slot2.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$" + this.cashSlot2;
				this.slot2.transform.GetChild(2).gameObject.GetComponent<RawImage>().texture = this.moneyTex;
				this.slot2.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = true;
			}
			else
			{
				this.cashSlot2 = 0f;
				string description3 = this.item2.GetComponent<PickUp>().description;
				int iconNum3 = this.GetIconNum(description3);
				this.slot2.transform.GetChild(2).gameObject.GetComponent<RawImage>().texture = this.consumables[iconNum3];
				this.slot2.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = true;
				this.slot2.transform.GetChild(0).gameObject.GetComponent<Text>().text = description3;
			}
		}
		if (this.item3 != null)
		{
			if (this.item3.name.Substring(0, 4) == "Cash")
			{
				this.cashSlot3 = this.item3.GetComponent<PickUp>().thisDurability;
				this.slot3.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$" + this.cashSlot3;
				this.slot3.transform.GetChild(2).gameObject.GetComponent<RawImage>().texture = this.moneyTex;
				this.slot3.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = true;
			}
			else
			{
				this.cashSlot3 = 0f;
				string description4 = this.item3.GetComponent<PickUp>().description;
				int iconNum4 = this.GetIconNum(description4);
				this.slot3.transform.GetChild(2).gameObject.GetComponent<RawImage>().texture = this.consumables[iconNum4];
				this.slot3.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = true;
				this.slot3.transform.GetChild(0).gameObject.GetComponent<Text>().text = description4;
			}
		}
		if (this.item4 != null)
		{
			if (this.item4.name.Substring(0, 4) == "Cash")
			{
				this.cashSlot4 = this.item4.GetComponent<PickUp>().thisDurability;
				this.slot4.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$" + this.cashSlot4;
				this.slot4.transform.GetChild(2).gameObject.GetComponent<RawImage>().texture = this.moneyTex;
				this.slot4.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = true;
			}
			else
			{
				this.cashSlot4 = 0f;
				string description5 = this.item4.GetComponent<PickUp>().description;
				int iconNum5 = this.GetIconNum(description5);
				this.slot4.transform.GetChild(2).gameObject.GetComponent<RawImage>().texture = this.consumables[iconNum5];
				this.slot4.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = true;
				this.slot4.transform.GetChild(0).gameObject.GetComponent<Text>().text = description5;
			}
		}
		if (this.item5 != null)
		{
			if (this.item5.name.Substring(0, 4) == "Cash")
			{
				this.cashSlot5 = this.item5.GetComponent<PickUp>().thisDurability;
				this.slot5.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$" + this.cashSlot5;
				this.slot5.transform.GetChild(2).gameObject.GetComponent<RawImage>().texture = this.moneyTex;
				this.slot5.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = true;
			}
			else
			{
				this.cashSlot5 = 0f;
				string description6 = this.item5.GetComponent<PickUp>().description;
				int iconNum6 = this.GetIconNum(description6);
				this.slot5.transform.GetChild(2).gameObject.GetComponent<RawImage>().texture = this.consumables[iconNum6];
				this.slot5.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = true;
				this.slot5.transform.GetChild(0).gameObject.GetComponent<Text>().text = description6;
			}
		}
		this.currency.money = this.cashSlot0 + this.cashSlot1 + this.cashSlot2 + this.cashSlot3 + this.cashSlot4 + this.cashSlot5;
		this.currency.money = Mathf.Round(this.currency.money * 100f) / 100f;
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x000408D4 File Offset: 0x0003EAD4
	public void AddObject(GameObject addObj)
	{
		if (addObj != null && addObj.activeSelf)
		{
			this.pickUp = addObj.GetComponent<PickUp>();
			if (this.pickUp && this.pickUp.pickable && addObj.GetComponent<Rigidbody>().mass < 3f)
			{
				addObj.GetComponent<BoxCollider>().enabled = true;
				if (addObj.name.Substring(0, 4) == "Cash")
				{
					this.CatalogueCash(addObj);
					return;
				}
				if (this.item0 == null || this.item1 == null || this.item2 == null || this.item3 == null || this.item4 == null || this.item5 == null)
				{
					this.Catalogue(addObj);
				}
			}
		}
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x000409C4 File Offset: 0x0003EBC4
	public void CatalogueCash(GameObject addObj)
	{
		this.moneyVal = addObj.GetComponent<PickUp>().thisDurability;
		if (this.currency.money == 0f)
		{
			this.slotX = this.FindEmptySlot();
			if (this.slotX != null)
			{
				this.slotX.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Cash";
				this.slotX.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$" + this.pickUp.thisDurability;
				this.slotX.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = true;
				this.slotX.transform.GetChild(2).gameObject.GetComponent<RawImage>().texture = this.moneyTex;
				if (this.slotX == this.slot0)
				{
					this.cashSlot0 = this.pickUp.thisDurability;
					this.item0 = addObj;
				}
				if (this.slotX == this.slot1)
				{
					this.cashSlot1 = this.pickUp.thisDurability;
					this.item1 = addObj;
				}
				if (this.slotX == this.slot2)
				{
					this.cashSlot2 = this.pickUp.thisDurability;
					this.item2 = addObj;
				}
				if (this.slotX == this.slot3)
				{
					this.cashSlot3 = this.pickUp.thisDurability;
					this.item3 = addObj;
				}
				if (this.slotX == this.slot4)
				{
					this.cashSlot4 = this.pickUp.thisDurability;
					this.item4 = addObj;
				}
				if (this.slotX == this.slot5)
				{
					this.cashSlot5 = this.pickUp.thisDurability;
					this.item5 = addObj;
				}
				this.currency.money = Mathf.Round(this.pickUp.thisDurability * 100f) / 100f;
				addObj.SetActive(false);
				addObj = null;
				return;
			}
		}
		else
		{
			this.remainder = this.pickUp.thisDurability;
			if (this.cashSlot0 > 0f && this.cashSlot0 < 400f)
			{
				if (this.cashSlot0 + this.remainder <= 400f)
				{
					this.slot0.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$" + (this.cashSlot0 + this.remainder);
					this.item0.GetComponent<PickUp>().thisDurability += Mathf.Round(this.remainder * 100f) / 100f;
					this.cashSlot0 += this.remainder;
					Object.Destroy(addObj);
					Debug.Log("addto1");
					this.remainder = 0f;
				}
				else
				{
					this.slot0.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$400";
					this.item0.GetComponent<PickUp>().thisDurability = 400f;
					this.remainder = this.cashSlot0 + this.pickUp.thisDurability - 400f;
					this.cashSlot0 = 400f;
					Debug.Log("carry over to 2" + this.remainder);
				}
			}
			if (this.cashSlot1 > 0f && this.cashSlot1 < 400f && this.remainder > 0f)
			{
				if (this.cashSlot1 + this.remainder <= 400f)
				{
					this.slot1.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$" + (this.cashSlot1 + this.remainder);
					this.item1.GetComponent<PickUp>().thisDurability += Mathf.Round(this.remainder * 100f) / 100f;
					this.cashSlot1 += this.remainder;
					Object.Destroy(addObj);
					Debug.Log("addto2");
					this.remainder = 0f;
				}
				else
				{
					this.slot1.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$400";
					this.item1.GetComponent<PickUp>().thisDurability = 400f;
					this.remainder = this.cashSlot1 + this.pickUp.thisDurability - 400f;
					this.cashSlot1 = 400f;
					Debug.Log("carryoverto3");
				}
			}
			if (this.cashSlot2 > 0f && this.cashSlot2 < 400f && this.remainder > 0f)
			{
				if (this.cashSlot2 + this.remainder <= 400f)
				{
					this.slot2.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$" + (this.cashSlot2 + this.remainder);
					this.item2.GetComponent<PickUp>().thisDurability += Mathf.Round(this.remainder * 100f) / 100f;
					this.cashSlot2 += this.remainder;
					Object.Destroy(addObj);
					this.remainder = 0f;
				}
				else
				{
					this.slot2.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$400";
					this.item2.GetComponent<PickUp>().thisDurability = 400f;
					this.remainder = this.cashSlot2 + this.pickUp.thisDurability - 400f;
					this.cashSlot2 = 400f;
				}
			}
			if (this.cashSlot3 > 0f && this.cashSlot3 < 400f && this.remainder > 0f)
			{
				if (this.cashSlot3 + this.remainder <= 400f)
				{
					this.slot3.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$" + (this.cashSlot3 + this.remainder);
					this.item3.GetComponent<PickUp>().thisDurability += Mathf.Round(this.remainder * 100f) / 100f;
					this.cashSlot3 += this.remainder;
					Object.Destroy(addObj);
					this.remainder = 0f;
				}
				else
				{
					this.slot3.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$400";
					this.item3.GetComponent<PickUp>().thisDurability = 400f;
					this.remainder = this.cashSlot3 + this.pickUp.thisDurability - 400f;
					this.cashSlot3 = 400f;
				}
			}
			if (this.cashSlot4 > 0f && this.cashSlot4 < 400f && this.remainder > 0f)
			{
				if (this.cashSlot4 + this.remainder <= 400f)
				{
					this.slot4.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$" + (this.cashSlot4 + this.remainder);
					this.item4.GetComponent<PickUp>().thisDurability += Mathf.Round(this.remainder * 100f) / 100f;
					this.cashSlot4 += this.remainder;
					Object.Destroy(addObj);
					this.remainder = 0f;
				}
				else
				{
					this.slot4.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$400";
					this.item4.GetComponent<PickUp>().thisDurability = 400f;
					this.remainder = this.cashSlot4 + this.pickUp.thisDurability - 400f;
					this.cashSlot4 = 400f;
				}
			}
			if (this.cashSlot5 > 0f && this.cashSlot5 < 400f && this.remainder > 0f)
			{
				if (this.cashSlot5 + this.remainder <= 400f)
				{
					this.slot5.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$" + (this.cashSlot5 + this.remainder);
					this.item5.GetComponent<PickUp>().thisDurability += Mathf.Round(this.remainder * 100f) / 100f;
					this.cashSlot5 += this.remainder;
					Object.Destroy(addObj);
					this.remainder = 0f;
				}
				else
				{
					Debug.Log("maxslot");
					this.slot5.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$400";
					this.item5.GetComponent<PickUp>().thisDurability = 400f;
					this.remainder = this.cashSlot5 + this.pickUp.thisDurability - 400f;
					Debug.Log("thisremainer" + this.remainder);
					this.cashSlot5 = 400f;
				}
			}
			if (this.remainder > 0f)
			{
				Debug.Log("excessremainder");
				this.slotX = this.FindEmptySlot();
				if (this.slotX == null)
				{
					Debug.Log("nullslot");
					this.pickUp.thisDurability = Mathf.Round(this.remainder * 100f) / 100f;
					this.pickUp.description = "Cash ($" + this.remainder + ")";
					this.currency.money = this.cashSlot0 + this.cashSlot1 + this.cashSlot2 + this.cashSlot3 + this.cashSlot4 + this.cashSlot5;
					this.currency.money = Mathf.Round(this.currency.money * 100f) / 100f;
					if (this.currency.money == 2400f)
					{
						this.slotX = null;
						Achievement achievement = new Achievement("ACH_MONEY");
						achievement.Trigger(true);
					}
				}
				else
				{
					Debug.Log("notnullslot" + this.remainder);
					this.slotX.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Cash";
					this.slotX.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$" + this.remainder;
					this.slotX.transform.GetChild(2).gameObject.GetComponent<RawImage>().texture = this.moneyTex;
					this.slotX.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = true;
					if (this.slotX == this.slot0)
					{
						this.cashSlot0 = this.remainder;
						this.item0 = addObj;
					}
					else if (this.slotX == this.slot1)
					{
						this.cashSlot1 = this.remainder;
						this.item1 = addObj;
					}
					else if (this.slotX == this.slot2)
					{
						this.cashSlot2 = this.remainder;
						this.item2 = addObj;
					}
					else if (this.slotX == this.slot3)
					{
						this.cashSlot3 = this.remainder;
						this.item3 = addObj;
					}
					else if (this.slotX == this.slot4)
					{
						this.cashSlot4 = this.remainder;
						this.item4 = addObj;
					}
					else if (this.slotX == this.slot5)
					{
						this.cashSlot5 = this.remainder;
						this.item5 = addObj;
					}
					this.currency.money = this.cashSlot0 + this.cashSlot1 + this.cashSlot2 + this.cashSlot3 + this.cashSlot4 + this.cashSlot5;
					this.currency.money = Mathf.Round(this.currency.money * 100f) / 100f;
					this.pickUp.thisDurability = Mathf.Round(this.remainder * 100f) / 100f;
					addObj.SetActive(false);
					addObj = null;
				}
			}
			else
			{
				this.currency.money = this.cashSlot0 + this.cashSlot1 + this.cashSlot2 + this.cashSlot3 + this.cashSlot4 + this.cashSlot5;
				this.currency.money = Mathf.Round(this.currency.money * 100f) / 100f;
				Object.Destroy(addObj);
				addObj = null;
			}
			this.slotX = null;
		}
	}

	// Token: 0x06000546 RID: 1350 RVA: 0x00041768 File Offset: 0x0003F968
	public void Catalogue(GameObject addObj)
	{
		this.slotX = this.FindEmptySlot();
		if (this.slotX != null)
		{
			this.slotX.transform.GetChild(0).gameObject.GetComponent<Text>().text = this.pickUp.description;
			this.slotX.transform.GetChild(1).gameObject.GetComponent<Text>().text = this.pickUp.thisDurability + "%";
			string description = this.pickUp.description;
			int iconNum = this.GetIconNum(description);
			this.slotX.transform.GetChild(2).gameObject.GetComponent<RawImage>().texture = this.consumables[iconNum];
			this.slotX.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = true;
			addObj.SetActive(false);
			if (this.slotX == this.slot0)
			{
				this.item0 = addObj;
			}
			if (this.slotX == this.slot1)
			{
				this.item1 = addObj;
			}
			if (this.slotX == this.slot2)
			{
				this.item2 = addObj;
			}
			if (this.slotX == this.slot3)
			{
				this.item3 = addObj;
			}
			if (this.slotX == this.slot4)
			{
				this.item4 = addObj;
			}
			if (this.slotX == this.slot5)
			{
				this.item5 = addObj;
			}
			this.slotX = null;
			addObj = null;
		}
	}

	// Token: 0x06000547 RID: 1351 RVA: 0x00041904 File Offset: 0x0003FB04
	public void DropObject(int slot)
	{
		this.itemX = null;
		if (slot == 0 & this.item0 != null)
		{
			this.itemX = this.item0;
		}
		else if (slot == 1 & this.item1 != null)
		{
			this.itemX = this.item1;
		}
		else if (slot == 2 & this.item2 != null)
		{
			this.itemX = this.item2;
		}
		else if (slot == 3 & this.item3 != null)
		{
			this.itemX = this.item3;
		}
		else if (slot == 4 & this.item4 != null)
		{
			this.itemX = this.item4;
		}
		else if (slot == 5 & this.item5 != null)
		{
			this.itemX = this.item5;
		}
		if (this.itemX != null)
		{
			this.Drop(this.itemX);
		}
	}

	// Token: 0x06000548 RID: 1352 RVA: 0x000419FC File Offset: 0x0003FBFC
	private void Drop(GameObject itemX)
	{
		if (itemX.name.Substring(0, 4) == "Cash")
		{
			this.moneyVal = itemX.GetComponent<PickUp>().thisDurability;
			if (itemX == this.item0)
			{
				this.cashSlot0 = 0f;
			}
			else if (itemX == this.item1)
			{
				this.cashSlot1 = 0f;
			}
			else if (itemX == this.item2)
			{
				this.cashSlot2 = 0f;
			}
			else if (itemX == this.item3)
			{
				this.cashSlot3 = 0f;
			}
			else if (itemX == this.item4)
			{
				this.cashSlot4 = 0f;
			}
			else if (itemX == this.item5)
			{
				this.cashSlot5 = 0f;
			}
			itemX.GetComponent<PickUp>().description = "Cash ($" + this.moneyVal + ")";
		}
		itemX.SetActive(true);
		if (itemX == this.item0)
		{
			this.item0 = null;
		}
		else if (itemX == this.item1)
		{
			this.item1 = null;
		}
		else if (itemX == this.item2)
		{
			this.item2 = null;
		}
		else if (itemX == this.item3)
		{
			this.item3 = null;
		}
		else if (itemX == this.item4)
		{
			this.item4 = null;
		}
		else if (itemX == this.item5)
		{
			this.item5 = null;
		}
		itemX.transform.position = this.theDest.position;
		itemX.GetComponent<PickUp>().LetGo(0);
		itemX = null;
		this.OpenCheck();
	}

	// Token: 0x06000549 RID: 1353 RVA: 0x00041BB8 File Offset: 0x0003FDB8
	public void RetrieveObject2(int slot)
	{
		this.itemX = null;
		if (slot == 0 & this.item0 != null)
		{
			this.itemX = this.item0;
		}
		else if (slot == 1 & this.item1 != null)
		{
			this.itemX = this.item1;
		}
		else if (slot == 2 & this.item2 != null)
		{
			this.itemX = this.item2;
		}
		else if (slot == 3 & this.item3 != null)
		{
			this.itemX = this.item3;
		}
		else if (slot == 4 & this.item4 != null)
		{
			this.itemX = this.item4;
		}
		else if (slot == 5 & this.item5 != null)
		{
			this.itemX = this.item5;
		}
		if (this.itemX != null)
		{
			this.Retrieve2(this.itemX);
		}
	}

	// Token: 0x0600054A RID: 1354 RVA: 0x00041CB0 File Offset: 0x0003FEB0
	public void Retrieve2(GameObject itemX)
	{
		if (itemX.name.Substring(0, 4) == "Cash")
		{
			this.moneyVal = itemX.GetComponent<PickUp>().thisDurability;
			this.currency.money -= this.moneyVal;
			if (itemX == this.item0)
			{
				this.cashSlot0 = 0f;
			}
			else if (itemX == this.item1)
			{
				this.cashSlot1 = 0f;
			}
			else if (itemX == this.item2)
			{
				this.cashSlot2 = 0f;
			}
			else if (itemX == this.item3)
			{
				this.cashSlot3 = 0f;
			}
			else if (itemX == this.item4)
			{
				this.cashSlot4 = 0f;
			}
			else if (itemX == this.item5)
			{
				this.cashSlot5 = 0f;
			}
			itemX.GetComponent<PickUp>().description = "Cash ($" + this.moneyVal + ")";
		}
		itemX.SetActive(true);
		if (itemX == this.item0)
		{
			this.item0 = null;
		}
		else if (itemX == this.item1)
		{
			this.item1 = null;
		}
		else if (itemX == this.item2)
		{
			this.item2 = null;
		}
		else if (itemX == this.item3)
		{
			this.item3 = null;
		}
		else if (itemX == this.item4)
		{
			this.item4 = null;
		}
		else if (itemX == this.item5)
		{
			this.item5 = null;
		}
		itemX.transform.position = this.theDest.position;
		itemX.GetComponent<PickUp>().LetGo(0);
		itemX.GetComponent<PickUp>().ForceGrab();
		itemX = null;
	}

	// Token: 0x0600054B RID: 1355 RVA: 0x00041E8C File Offset: 0x0004008C
	public void RetrieveObject(int slot)
	{
		this.interactor.DropObject();
		this.itemX = null;
		if (slot == 0 & this.item0 != null)
		{
			this.itemX = this.item0;
		}
		else if (slot == 1 & this.item1 != null)
		{
			this.itemX = this.item1;
		}
		else if (slot == 2 & this.item2 != null)
		{
			this.itemX = this.item2;
		}
		else if (slot == 3 & this.item3 != null)
		{
			this.itemX = this.item3;
		}
		else if (slot == 4 & this.item4 != null)
		{
			this.itemX = this.item4;
		}
		else if (slot == 5 & this.item5 != null)
		{
			this.itemX = this.item5;
		}
		if (this.itemX != null)
		{
			base.StartCoroutine(this.Retrieve(this.itemX));
		}
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x00041F96 File Offset: 0x00040196
	private IEnumerator Retrieve(GameObject itemX)
	{
		yield return new WaitForSeconds(0.1f);
		if (itemX.name.Substring(0, 4) == "Cash")
		{
			this.moneyVal = this.item0.GetComponent<PickUp>().thisDurability;
			this.currency.money -= this.moneyVal;
			if (itemX == this.item0)
			{
				this.cashSlot0 = 0f;
			}
			else if (itemX == this.item1)
			{
				this.cashSlot1 = 0f;
			}
			else if (itemX == this.item2)
			{
				this.cashSlot2 = 0f;
			}
			else if (itemX == this.item3)
			{
				this.cashSlot3 = 0f;
			}
			else if (itemX == this.item4)
			{
				this.cashSlot4 = 0f;
			}
			else if (itemX == this.item5)
			{
				this.cashSlot5 = 0f;
			}
			itemX.GetComponent<PickUp>().description = "Cash ($" + this.moneyVal + ")";
		}
		itemX.SetActive(true);
		itemX.transform.position = this.theDest.position;
		itemX.transform.parent = this.theDest;
		this.interactor.pickedUpObject = itemX;
		if (itemX == this.item0)
		{
			this.item0 = null;
		}
		else if (itemX == this.item1)
		{
			this.item1 = null;
		}
		else if (itemX == this.item2)
		{
			this.item2 = null;
		}
		else if (itemX == this.item3)
		{
			this.item3 = null;
		}
		else if (itemX == this.item4)
		{
			this.item4 = null;
		}
		else if (itemX == this.item5)
		{
			this.item5 = null;
		}
		itemX = null;
		yield break;
	}

	// Token: 0x0600054D RID: 1357 RVA: 0x00041FAC File Offset: 0x000401AC
	public GameObject FindEmptySlot()
	{
		if (this.item0 == null)
		{
			this.slotX = this.slot0;
		}
		else if (this.item1 == null)
		{
			this.slotX = this.slot1;
		}
		else if (this.item2 == null)
		{
			this.slotX = this.slot2;
		}
		else if (this.item3 == null)
		{
			this.slotX = this.slot3;
		}
		else if (this.item4 == null)
		{
			this.slotX = this.slot4;
		}
		else if (this.item5 == null)
		{
			this.slotX = this.slot5;
		}
		else
		{
			this.slotX = null;
		}
		return this.slotX;
	}

	// Token: 0x0600054E RID: 1358 RVA: 0x00042074 File Offset: 0x00040274
	public void SubtractMoney(float amount)
	{
		this.OpenCheck();
		this.remainder = amount;
		if (amount > 0f)
		{
			if (this.cashSlot5 > 0f && this.remainder > 0f)
			{
				if (this.remainder >= this.cashSlot5)
				{
					this.slot5.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
					this.slot5.transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
					this.slot5.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = false;
					Object.Destroy(this.item5);
					this.item5 = null;
					this.remainder -= this.cashSlot5;
					this.remainder = Mathf.Round(this.remainder * 100f) / 100f;
					this.cashSlot5 = 0f;
				}
				else
				{
					this.cashSlot5 -= this.remainder;
					this.cashSlot5 = Mathf.Round(this.cashSlot5 * 100f) / 100f;
					this.slot5.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$" + this.cashSlot5;
					this.item5.GetComponent<PickUp>().thisDurability = this.cashSlot5;
					this.remainder = 0f;
				}
			}
			if (this.cashSlot4 > 0f && this.remainder > 0f)
			{
				if (this.remainder >= this.cashSlot4)
				{
					this.slot4.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
					this.slot4.transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
					this.slot4.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = false;
					Object.Destroy(this.item4);
					this.item4 = null;
					this.remainder -= this.cashSlot4;
					this.remainder = Mathf.Round(this.remainder * 100f) / 100f;
					this.cashSlot4 = 0f;
				}
				else
				{
					this.cashSlot4 -= this.remainder;
					this.cashSlot4 = Mathf.Round(this.cashSlot4 * 100f) / 100f;
					this.slot4.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$" + this.cashSlot4;
					this.item4.GetComponent<PickUp>().thisDurability = this.cashSlot4;
					this.remainder = 0f;
				}
			}
			if (this.cashSlot3 > 0f && this.remainder > 0f)
			{
				if (this.remainder >= this.cashSlot3)
				{
					this.slot3.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
					this.slot3.transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
					this.slot3.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = false;
					Object.Destroy(this.item3);
					this.item3 = null;
					this.remainder -= this.cashSlot3;
					this.remainder = Mathf.Round(this.remainder * 100f) / 100f;
					this.cashSlot3 = 0f;
				}
				else
				{
					this.cashSlot3 -= this.remainder;
					this.cashSlot3 = Mathf.Round(this.cashSlot3 * 100f) / 100f;
					this.slot3.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$" + this.cashSlot3;
					this.item3.GetComponent<PickUp>().thisDurability = this.cashSlot3;
					this.remainder = 0f;
				}
			}
			if (this.cashSlot2 > 0f && this.remainder > 0f)
			{
				if (this.remainder >= this.cashSlot2)
				{
					this.slot2.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
					this.slot2.transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
					this.slot2.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = false;
					Object.Destroy(this.item2);
					this.item2 = null;
					this.remainder -= this.cashSlot2;
					this.remainder = Mathf.Round(this.remainder * 100f) / 100f;
					this.cashSlot2 = 0f;
				}
				else
				{
					this.cashSlot2 -= this.remainder;
					this.cashSlot2 = Mathf.Round(this.cashSlot2 * 100f) / 100f;
					this.slot2.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$" + this.cashSlot2;
					this.item2.GetComponent<PickUp>().thisDurability = this.cashSlot2;
					this.remainder = 0f;
				}
			}
			if (this.cashSlot1 > 0f && this.remainder > 0f)
			{
				if (this.remainder >= this.cashSlot1)
				{
					this.slot1.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
					this.slot1.transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
					this.slot1.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = false;
					Object.Destroy(this.item1);
					this.item1 = null;
					this.remainder -= this.cashSlot1;
					this.remainder = Mathf.Round(this.remainder * 100f) / 100f;
					this.cashSlot1 = 0f;
				}
				else
				{
					this.cashSlot1 -= this.remainder;
					this.cashSlot1 = Mathf.Round(this.cashSlot1 * 100f) / 100f;
					this.slot1.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$" + this.cashSlot1;
					this.item1.GetComponent<PickUp>().thisDurability = this.cashSlot1;
					this.remainder = 0f;
				}
			}
			if (this.cashSlot0 > 0f && this.remainder > 0f)
			{
				if (this.remainder >= this.cashSlot0)
				{
					this.slot0.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
					this.slot0.transform.GetChild(1).gameObject.GetComponent<Text>().text = "";
					this.slot0.transform.GetChild(2).gameObject.GetComponent<RawImage>().enabled = false;
					Object.Destroy(this.item0);
					this.item0 = null;
					this.remainder -= this.cashSlot0;
					this.remainder = Mathf.Round(this.remainder * 100f) / 100f;
					this.cashSlot0 = 0f;
				}
				else
				{
					this.cashSlot0 -= this.remainder;
					this.cashSlot0 = Mathf.Round(this.cashSlot0 * 100f) / 100f;
					this.slot0.transform.GetChild(1).gameObject.GetComponent<Text>().text = "$" + this.cashSlot0;
					this.item0.GetComponent<PickUp>().thisDurability = this.cashSlot0;
					this.remainder = 0f;
				}
			}
		}
		this.interactor.RefreshCash();
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x0004295C File Offset: 0x00040B5C
	private int GetIconNum(string itemName)
	{
		int result;
		if (InventoryItems.itemLookup.TryGetValue(itemName, out result))
		{
			return result;
		}
		return 0;
	}

	// Token: 0x04000B27 RID: 2855
	public GameObject item0;

	// Token: 0x04000B28 RID: 2856
	public GameObject item1;

	// Token: 0x04000B29 RID: 2857
	public GameObject item2;

	// Token: 0x04000B2A RID: 2858
	public GameObject item3;

	// Token: 0x04000B2B RID: 2859
	public GameObject item4;

	// Token: 0x04000B2C RID: 2860
	public GameObject item5;

	// Token: 0x04000B2D RID: 2861
	public GameObject slot0;

	// Token: 0x04000B2E RID: 2862
	public GameObject slot1;

	// Token: 0x04000B2F RID: 2863
	public GameObject slot2;

	// Token: 0x04000B30 RID: 2864
	public GameObject slot3;

	// Token: 0x04000B31 RID: 2865
	public GameObject slot4;

	// Token: 0x04000B32 RID: 2866
	public GameObject slot5;

	// Token: 0x04000B33 RID: 2867
	public float cashSlot0;

	// Token: 0x04000B34 RID: 2868
	public float cashSlot1;

	// Token: 0x04000B35 RID: 2869
	public float cashSlot2;

	// Token: 0x04000B36 RID: 2870
	public float cashSlot3;

	// Token: 0x04000B37 RID: 2871
	public float cashSlot4;

	// Token: 0x04000B38 RID: 2872
	public float cashSlot5;

	// Token: 0x04000B39 RID: 2873
	private float remainder;

	// Token: 0x04000B3A RID: 2874
	public GameObject itemX;

	// Token: 0x04000B3B RID: 2875
	public GameObject addObj;

	// Token: 0x04000B3C RID: 2876
	public GameObject slotX;

	// Token: 0x04000B3D RID: 2877
	private PickUp pickUp;

	// Token: 0x04000B3E RID: 2878
	public Transform theDest;

	// Token: 0x04000B3F RID: 2879
	public Currency currency;

	// Token: 0x04000B40 RID: 2880
	public float moneyVal;

	// Token: 0x04000B41 RID: 2881
	public Texture defaultTex;

	// Token: 0x04000B42 RID: 2882
	public Texture moneyTex;

	// Token: 0x04000B43 RID: 2883
	public Texture[] consumables;

	// Token: 0x04000B44 RID: 2884
	private int i;

	// Token: 0x04000B45 RID: 2885
	private FixedJoint m_HoldJoint;

	// Token: 0x04000B46 RID: 2886
	public Interactor interactor;

	// Token: 0x04000B47 RID: 2887
	private static Dictionary<string, int> itemLookup = new Dictionary<string, int>
	{
		{
			"Beef-A-Reeno",
			1
		},
		{
			"Cornmeal",
			2
		},
		{
			"Cram",
			3
		},
		{
			"Blackberries",
			4
		},
		{
			"Limes",
			4
		},
		{
			"Oranges",
			4
		},
		{
			"Ambrosia",
			4
		},
		{
			"Lottery Ticket",
			5
		},
		{
			"Energy Drink",
			6
		},
		{
			"Sugar",
			7
		},
		{
			"Yeast",
			8
		},
		{
			"Zen",
			9
		},
		{
			"Citation",
			10
		}
	};
}
