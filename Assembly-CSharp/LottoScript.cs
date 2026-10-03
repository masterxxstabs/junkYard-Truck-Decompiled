using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x020000ED RID: 237
public class LottoScript : MonoBehaviour
{
	// Token: 0x060005D6 RID: 1494 RVA: 0x00047D90 File Offset: 0x00045F90
	private void Start()
	{
		this.op1.GetComponent<RawImage>().texture = this.blank;
		this.op2.GetComponent<RawImage>().texture = this.blank;
		this.op3.GetComponent<RawImage>().texture = this.blank;
		this.op4.GetComponent<RawImage>().texture = this.blank;
		this.op5.GetComponent<RawImage>().texture = this.blank;
		this.op6.GetComponent<RawImage>().texture = this.blank;
		this.won = 0;
		this.num1s = 0;
		this.num2s = 0;
		this.num3s = 0;
		this.num4s = 0;
		this.num5s = 0;
		this.num6s = 0;
		this.opClose.SetActive(false);
		this.numScratched = 0;
		this.square1 = Random.Range(1, 7);
		this.square2 = Random.Range(2, this.square1);
		this.square3 = Random.Range(1, this.square2);
		this.square4 = Random.Range(1, this.square3);
		this.square5 = Random.Range(1, this.square4);
		this.square6 = Random.Range(1, 7);
		this.finished = false;
		if (this.square1 == 5 && this.square6 == 5)
		{
			this.square6 = Random.Range(4, 7);
		}
		if (this.square1 == 7 && this.square6 == 7)
		{
			this.square6 = Random.Range(1, 7);
		}
	}

	// Token: 0x060005D7 RID: 1495 RVA: 0x00047F0C File Offset: 0x0004610C
	public void Option(GameObject op)
	{
		this.sqNum = 0;
		string name = op.name;
		if (!(name == "Option1"))
		{
			if (!(name == "Option2"))
			{
				if (!(name == "Option3"))
				{
					if (!(name == "Option4"))
					{
						if (!(name == "Option5"))
						{
							if (name == "Option6")
							{
								if (this.op6.GetComponent<RawImage>().texture == this.blank)
								{
									this.sqNum = this.square6;
								}
							}
						}
						else if (this.op5.GetComponent<RawImage>().texture == this.blank)
						{
							this.sqNum = this.square5;
						}
					}
					else if (this.op4.GetComponent<RawImage>().texture == this.blank)
					{
						this.sqNum = this.square4;
					}
				}
				else if (this.op3.GetComponent<RawImage>().texture == this.blank)
				{
					this.sqNum = this.square3;
				}
			}
			else if (this.op2.GetComponent<RawImage>().texture == this.blank)
			{
				this.sqNum = this.square2;
			}
		}
		else if (this.op1.GetComponent<RawImage>().texture == this.blank)
		{
			this.sqNum = this.square1;
		}
		if (this.sqNum > 0)
		{
			this.numScratched++;
		}
		if (this.numScratched < 7)
		{
			if (this.sqNum == 1)
			{
				op.GetComponent<RawImage>().texture = this.ten;
				this.num1s++;
			}
			else if (this.sqNum == 2)
			{
				op.GetComponent<RawImage>().texture = this.fifty;
				this.num2s++;
			}
			else if (this.sqNum == 3)
			{
				op.GetComponent<RawImage>().texture = this.hundred;
				this.num3s++;
			}
			else if (this.sqNum == 4)
			{
				op.GetComponent<RawImage>().texture = this.twofifty;
				this.num4s++;
			}
			else if (this.sqNum == 5)
			{
				op.GetComponent<RawImage>().texture = this.thousand;
				this.num5s++;
			}
			else if (this.sqNum == 6)
			{
				op.GetComponent<RawImage>().texture = this.hundredk;
				this.num6s++;
			}
			if (this.numScratched == 6 && !this.finished)
			{
				this.finished = true;
				this.CalcWinner();
			}
		}
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x000481D8 File Offset: 0x000463D8
	private void CalcWinner()
	{
		this.opClose.SetActive(true);
		if (this.num1s > 2)
		{
			this.won += 10;
		}
		if (this.num1s == 6)
		{
			this.won += 20;
		}
		if (this.num2s > 2)
		{
			this.won += 50;
		}
		if (this.num2s == 6)
		{
			this.won += 100;
		}
		if (this.num3s > 2)
		{
			this.won += 100;
		}
		if (this.num3s == 6)
		{
			this.won += 200;
		}
		if (this.num4s > 2)
		{
			this.won += 250;
		}
		if (this.num4s == 6)
		{
			this.won += 500;
		}
		if (this.num5s > 2)
		{
			this.won += 1000;
		}
		if (this.num5s == 6)
		{
			this.won += 1000;
		}
		if (this.num6s > 2)
		{
			this.won += 10000;
		}
		if (this.num6s == 6)
		{
			this.won += 20000;
		}
		if (this.won > 0)
		{
			this.newRoll = Object.Instantiate<GameObject>(this.moneyRoll, this.moneyLoc.transform.position, this.moneyLoc.transform.rotation);
			this.newRoll.GetComponent<PickUp>().thisDurability = (float)this.won;
		}
	}

	// Token: 0x060005D9 RID: 1497 RVA: 0x00048377 File Offset: 0x00046577
	public void OptionExit()
	{
		this.Start();
		this.lottoPanel.SetActive(false);
		this.fpc.LockMouse();
		this.fpc.enabled = true;
		this.ticketRestockable.SetActive(true);
		this.finished = false;
	}

	// Token: 0x04000CB0 RID: 3248
	public FirstPersonController fpc;

	// Token: 0x04000CB1 RID: 3249
	public GameObject lottoPanel;

	// Token: 0x04000CB2 RID: 3250
	public int square1;

	// Token: 0x04000CB3 RID: 3251
	public int square2;

	// Token: 0x04000CB4 RID: 3252
	public int square3;

	// Token: 0x04000CB5 RID: 3253
	public int square4;

	// Token: 0x04000CB6 RID: 3254
	public int square5;

	// Token: 0x04000CB7 RID: 3255
	public int square6;

	// Token: 0x04000CB8 RID: 3256
	public Texture ten;

	// Token: 0x04000CB9 RID: 3257
	public Texture fifty;

	// Token: 0x04000CBA RID: 3258
	public Texture hundred;

	// Token: 0x04000CBB RID: 3259
	public Texture twofifty;

	// Token: 0x04000CBC RID: 3260
	public Texture thousand;

	// Token: 0x04000CBD RID: 3261
	public Texture hundredk;

	// Token: 0x04000CBE RID: 3262
	public Texture blank;

	// Token: 0x04000CBF RID: 3263
	public GameObject op1;

	// Token: 0x04000CC0 RID: 3264
	public GameObject op2;

	// Token: 0x04000CC1 RID: 3265
	public GameObject op3;

	// Token: 0x04000CC2 RID: 3266
	public GameObject op4;

	// Token: 0x04000CC3 RID: 3267
	public GameObject op5;

	// Token: 0x04000CC4 RID: 3268
	public GameObject op6;

	// Token: 0x04000CC5 RID: 3269
	public GameObject opClose;

	// Token: 0x04000CC6 RID: 3270
	public int numScratched;

	// Token: 0x04000CC7 RID: 3271
	private int num1s;

	// Token: 0x04000CC8 RID: 3272
	private int num2s;

	// Token: 0x04000CC9 RID: 3273
	private int num3s;

	// Token: 0x04000CCA RID: 3274
	private int num4s;

	// Token: 0x04000CCB RID: 3275
	private int num5s;

	// Token: 0x04000CCC RID: 3276
	private int num6s;

	// Token: 0x04000CCD RID: 3277
	private int won;

	// Token: 0x04000CCE RID: 3278
	public Currency currency;

	// Token: 0x04000CCF RID: 3279
	public Transform spawnLoc;

	// Token: 0x04000CD0 RID: 3280
	public GameObject ticketPrefab;

	// Token: 0x04000CD1 RID: 3281
	private int sqNum;

	// Token: 0x04000CD2 RID: 3282
	public Transform moneyLoc;

	// Token: 0x04000CD3 RID: 3283
	public GameObject moneyRoll;

	// Token: 0x04000CD4 RID: 3284
	private GameObject newRoll;

	// Token: 0x04000CD5 RID: 3285
	public GameObject ticketRestockable;

	// Token: 0x04000CD6 RID: 3286
	public bool finished;
}
