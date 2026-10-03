using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200014A RID: 330
public class TillScript : MonoBehaviour
{
	// Token: 0x0600085E RID: 2142 RVA: 0x0006DD75 File Offset: 0x0006BF75
	private void Start()
	{
		if (this.cop == null)
		{
			this.cop = Object.FindObjectOfType<Officer>();
		}
	}

	// Token: 0x0600085F RID: 2143 RVA: 0x0006DD90 File Offset: 0x0006BF90
	public void AddToList(int itemNum)
	{
		base.GetComponent<AudioSource>().Play();
		switch (itemNum)
		{
		case 1:
			this.cram++;
			this.total += this.cramPrice;
			break;
		case 2:
			this.fuego++;
			this.total += this.fuegoPrice;
			break;
		case 3:
			this.beefareeno++;
			this.total += this.beefareenoPrice;
			break;
		case 4:
			this.sugar++;
			this.total += this.sugarPrice;
			break;
		case 5:
			this.cornmeal++;
			this.total += this.cornmealPrice;
			break;
		case 6:
			this.yeast++;
			this.total += this.yeastPrice;
			break;
		case 7:
			this.nrgdrink++;
			this.total += this.energyPrice;
			break;
		case 8:
			this.zen++;
			this.total += this.zenPrice;
			break;
		}
		this.tillText.GetComponent<Text>().text = this.total.ToString("F2");
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x0006DF17 File Offset: 0x0006C117
	public void AddGas()
	{
		this.total += 0.03f;
		this.unpaidFuel = true;
		this.tillText.GetComponent<Text>().text = this.total.ToString("F2");
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x0006DF54 File Offset: 0x0006C154
	public void PayNow()
	{
		this.chime.inside = true;
		if (this.cop == null)
		{
			this.cop = Object.FindObjectOfType<Officer>();
		}
		if (this.total <= this.playerCurrency.money)
		{
			this.inv.SubtractMoney(this.total);
			if (this.cram > 0 || this.fuego > 0 || this.beefareeno > 0 || this.sugar > 0 || this.cornmeal > 0 || this.yeast > 0 || this.nrgdrink > 0 || this.zen > 0)
			{
				this.newBag = Object.Instantiate<GameObject>(this.groceryBag, this.bagSpawn.position, this.bagSpawn.rotation);
				this.newBag.GetComponent<GroceryBag>().cramCount = this.cram;
				this.newBag.GetComponent<GroceryBag>().beefareenoCount = this.beefareeno;
				this.newBag.GetComponent<GroceryBag>().fuegoCount = this.fuego;
				this.newBag.GetComponent<GroceryBag>().sugarCount = this.sugar;
				this.newBag.GetComponent<GroceryBag>().cornmealCount = this.cornmeal;
				this.newBag.GetComponent<GroceryBag>().yeastCount = this.yeast;
				this.newBag.GetComponent<GroceryBag>().nrgCount = this.nrgdrink;
				this.newBag.GetComponent<GroceryBag>().zenCount = this.zen;
			}
			if (this.cornmeal > 5)
			{
				if (Random.Range(0, 3) == 0)
				{
					int voiceNum = Random.Range(6, 8);
					this.clerk.Say(voiceNum);
				}
			}
			else if (this.beefareeno > 3)
			{
				this.clerk.Say(8);
			}
			this.fuel = 0f;
			this.cram = 0;
			this.fuego = 0;
			this.beefareeno = 0;
			this.sugar = 0;
			this.cornmeal = 0;
			this.yeast = 0;
			this.nrgdrink = 0;
			this.zen = 0;
			this.total = 0f;
			this.unpaidFuel = false;
			if (this.cop != null)
			{
				this.cop.theftNoted = false;
			}
			this.tillText.GetComponent<Text>().text = "00.00";
			this.outsidePump.fuelGallons = 0.0;
			this.outsidePump.fuelPrice = 0f;
			this.outsidePump.fuelpricetext.GetComponent<Text>().text = "$ 00.00";
			this.outsidePump.fuelpricetext2.GetComponent<Text>().text = "$ 00.00";
			this.outsidePump.fuelgallonstext.GetComponent<Text>().text = "0.00";
			this.outsidePump.fuelgallonstext2.GetComponent<Text>().text = "0.00";
			base.GetComponent<AudioSource>().Play();
		}
	}

	// Token: 0x04001360 RID: 4960
	public GameObject tillText;

	// Token: 0x04001361 RID: 4961
	public Currency playerCurrency;

	// Token: 0x04001362 RID: 4962
	public Transform bagSpawn;

	// Token: 0x04001363 RID: 4963
	public GameObject groceryBag;

	// Token: 0x04001364 RID: 4964
	private GameObject newBag;

	// Token: 0x04001365 RID: 4965
	public float fuel;

	// Token: 0x04001366 RID: 4966
	public int cram;

	// Token: 0x04001367 RID: 4967
	public int fuego;

	// Token: 0x04001368 RID: 4968
	public int beefareeno;

	// Token: 0x04001369 RID: 4969
	public int sugar;

	// Token: 0x0400136A RID: 4970
	public int cornmeal;

	// Token: 0x0400136B RID: 4971
	public int yeast;

	// Token: 0x0400136C RID: 4972
	public int nrgdrink;

	// Token: 0x0400136D RID: 4973
	public int zen;

	// Token: 0x0400136E RID: 4974
	public float fuelPrice = 2.99f;

	// Token: 0x0400136F RID: 4975
	public float cramPrice = 2.99f;

	// Token: 0x04001370 RID: 4976
	public float fuegoPrice = 1.99f;

	// Token: 0x04001371 RID: 4977
	public float beefareenoPrice = 1.99f;

	// Token: 0x04001372 RID: 4978
	public float beerPrice = 9.99f;

	// Token: 0x04001373 RID: 4979
	public float sugarPrice = 2.49f;

	// Token: 0x04001374 RID: 4980
	public float cornmealPrice = 2.99f;

	// Token: 0x04001375 RID: 4981
	public float yeastPrice = 4.99f;

	// Token: 0x04001376 RID: 4982
	public float energyPrice = 4f;

	// Token: 0x04001377 RID: 4983
	public float zenPrice = 7f;

	// Token: 0x04001378 RID: 4984
	public float total;

	// Token: 0x04001379 RID: 4985
	public bool unpaidFuel;

	// Token: 0x0400137A RID: 4986
	public FluidHandler outsidePump;

	// Token: 0x0400137B RID: 4987
	public InventoryItems inv;

	// Token: 0x0400137C RID: 4988
	public Officer cop;

	// Token: 0x0400137D RID: 4989
	public Gasstationchime chime;

	// Token: 0x0400137E RID: 4990
	public Clerk clerk;
}
