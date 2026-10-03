using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x02000117 RID: 279
public class poker : MonoBehaviour
{
	// Token: 0x0600074A RID: 1866 RVA: 0x00002188 File Offset: 0x00000388
	private void Start()
	{
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x00002188 File Offset: 0x00000388
	public void UpdateText()
	{
	}

	// Token: 0x0600074C RID: 1868 RVA: 0x0005EB20 File Offset: 0x0005CD20
	public void BuyIn()
	{
		if (this.fpc.canMove)
		{
			if (this.currency.money >= 300f)
			{
				this.sitting = true;
				this.fpc.gameObject.GetComponent<Interactor>().inv.SubtractMoney(300f);
				this.currency.money -= 300f;
				this.currency.money = Mathf.Round(this.currency.money * 100f) / 100f;
				this.SitDown();
				this.buyingIn = true;
				this.npc1Chips = 300f;
				this.npc2Chips = 300f;
				this.npc3Chips = 300f;
				this.playerChips = 300f;
				this.bigBlindPos = 1;
				this.smallBlindPos = 4;
				this.GenerateChips(this.playerChipsLoc, 300 - (int)this.bigBlind);
				this.GenerateChips(this.n1ChipsLoc, 300);
				this.GenerateChips(this.n2ChipsLoc, 300);
				this.GenerateChips(this.n3ChipsLoc, 300 - (int)this.smallBlind);
				this.GenerateChips(this.potLoc, 15);
				this.NewRound();
				return;
			}
		}
		else
		{
			this.StandUp();
		}
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x0005EC6C File Offset: 0x0005CE6C
	private IEnumerator StartNewRound()
	{
		yield return new WaitForSeconds(3f);
		if (!this.fpc.canMove)
		{
			this.NewRound();
		}
		yield break;
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x0005EC7B File Offset: 0x0005CE7B
	private IEnumerator GenerateChipsI(Transform chipLoci, int numChipsi)
	{
		yield return new WaitForSeconds(1f);
		this.GenerateChips(chipLoci, numChipsi);
		yield break;
	}

	// Token: 0x0600074F RID: 1871 RVA: 0x0005EC98 File Offset: 0x0005CE98
	private void GenerateChips(Transform chipLoc, int numChips)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		if (numChips > 399)
		{
			num = (int)Mathf.Floor((float)(numChips / 100));
		}
		if (numChips - num * 100 > 24)
		{
			num2 = (int)Mathf.Floor((float)((numChips - num * 100) / 25));
		}
		if (numChips - num * 100 - num2 * 25 > 4)
		{
			num3 = (int)Mathf.Floor((float)((numChips - num * 100 - num2 * 25) / 5));
		}
		if (numChips - num * 100 - num2 * 25 - num3 * 5 > 0)
		{
			num4 = (int)Mathf.Floor((float)((numChips - num * 100 - num2 * 25 - num3 * 5) / 1));
		}
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				Object.Instantiate<GameObject>(this.chip100, chipLoc.position, chipLoc.rotation).transform.parent = chipLoc;
			}
		}
		if (num2 > 0)
		{
			for (int j = 0; j < num2; j++)
			{
				this.chip25V3 = chipLoc.position;
				if (chipLoc.name == "potLoc")
				{
					this.chip25V3 = new Vector3(chipLoc.position.x + Random.Range(-0.05f, 0.05f), chipLoc.position.y + Random.Range(-0.05f, 0.05f), chipLoc.position.z + Random.Range(-0.05f, 0.05f));
				}
				GameObject gameObject = Object.Instantiate<GameObject>(this.chip25, this.chip25V3, chipLoc.rotation);
				gameObject.transform.parent = chipLoc;
				base.StartCoroutine(this.MakeKinematicAfterDelay(gameObject, 3f));
			}
		}
		if (num3 > 0)
		{
			for (int k = 0; k < num3; k++)
			{
				this.chip5V3 = chipLoc.position;
				if (chipLoc.name == "potLoc")
				{
					this.chip5V3 = new Vector3(chipLoc.position.x + Random.Range(-0.05f, 0.05f), chipLoc.position.y + Random.Range(-0.05f, 0.05f), chipLoc.position.z + Random.Range(-0.05f, 0.05f));
				}
				GameObject gameObject2 = Object.Instantiate<GameObject>(this.chip5, this.chip5V3, chipLoc.rotation);
				gameObject2.transform.parent = chipLoc;
				base.StartCoroutine(this.MakeKinematicAfterDelay(gameObject2, 3f));
			}
		}
		if (num4 > 0)
		{
			for (int l = 0; l < num4; l++)
			{
				this.chip1V3 = chipLoc.position;
				if (chipLoc.name == "potLoc")
				{
					this.chip1V3 = new Vector3(chipLoc.position.x + Random.Range(-0.05f, 0.05f), chipLoc.position.y + Random.Range(-0.05f, 0.05f), chipLoc.position.z + Random.Range(-0.05f, 0.05f));
				}
				GameObject gameObject3 = Object.Instantiate<GameObject>(this.chip1, this.chip1V3, chipLoc.rotation);
				gameObject3.transform.parent = chipLoc;
				base.StartCoroutine(this.MakeKinematicAfterDelay(gameObject3, 3f));
			}
		}
		this.aSources = this.audio.GetComponents<AudioSource>();
		int num5 = Random.Range(0, 4);
		this.aSources[num5].Play();
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x00002188 File Offset: 0x00000388
	private void SubtractChips(Transform subtLoc, int subtChips)
	{
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x0005EFFC File Offset: 0x0005D1FC
	private void NewRound()
	{
		Debug.Log("newround");
		this.allChecked = false;
		this.okToCheck = false;
		this.check = false;
		this.numChecks = 0;
		this.paid = false;
		this.unansweredRaises = 3;
		this.roundEnded = false;
		this.gameEnded = false;
		this.numFolds = 0;
		this.playerLastAction = 0;
		this.npc1LastAction = 0;
		this.npc2LastAction = 0;
		this.npc3LastAction = 0;
		this.potSize = this.smallBlind + this.bigBlind;
		this.bbCompensated = false;
		this.sbCompensated = false;
		this.n1Text.text = "";
		this.panelBg1.SetActive(false);
		this.n2Text.text = "";
		this.panelBg2.SetActive(false);
		this.n3Text.text = "";
		this.panelBg3.SetActive(false);
		if (this.playerChips < this.bigBlind)
		{
			this.StandUp();
			return;
		}
		if (this.buyingIn)
		{
			this.bigBlindPos = 1;
			this.smallBlindPos = 4;
			this.buyingIn = false;
			this.turn = 2;
		}
		else
		{
			this.bigBlindPos++;
			if (this.bigBlindPos > 4)
			{
				this.bigBlindPos = 1;
			}
			this.smallBlindPos++;
			if (this.smallBlindPos > 4)
			{
				this.smallBlindPos = 1;
			}
		}
		this.callAmount = this.bigBlind;
		if (this.bigBlindPos == 1)
		{
			this.playerChips -= this.bigBlind;
			this.playerBet = this.bigBlind;
			this.playerLastAction = 4;
			this.turn = 2;
		}
		if (this.smallBlindPos == 1)
		{
			this.playerChips -= this.smallBlind;
			this.playerBet = this.smallBlind;
			this.playerLastAction = 2;
		}
		if (this.bigBlindPos == 2)
		{
			this.npc1Chips -= this.bigBlind;
			this.npc1Bet = this.bigBlind;
			this.npc1LastAction = 4;
			this.turn = 3;
		}
		if (this.smallBlindPos == 2)
		{
			this.npc1Chips -= this.smallBlind;
			this.npc1Bet = this.smallBlind;
			this.npc1LastAction = 2;
		}
		if (this.bigBlindPos == 3)
		{
			this.npc2Chips -= this.bigBlind;
			this.npc2Bet = this.bigBlind;
			this.npc2LastAction = 4;
			this.turn = 4;
		}
		if (this.smallBlindPos == 3)
		{
			this.npc2Chips -= this.smallBlind;
			this.npc2Bet = this.smallBlind;
			this.npc2LastAction = 2;
		}
		if (this.bigBlindPos == 4)
		{
			this.npc3Chips -= this.bigBlind;
			this.npc3Bet = this.bigBlind;
			this.npc3LastAction = 4;
			this.turn = 1;
		}
		if (this.smallBlindPos == 4)
		{
			this.npc3Chips -= this.smallBlind;
			this.npc3Bet = this.smallBlind;
			this.npc3LastAction = 2;
		}
		for (int i = 0; i < this.deck.Length; i++)
		{
			this.tempCardD[0] = this.deck[i];
			int num = Random.Range(i, this.deck.Length);
			this.deck[i] = this.deck[num];
			this.deck[num] = this.tempCardD[0];
		}
		this.river1 = this.deck[0];
		this.river2 = this.deck[1];
		this.river3 = this.deck[2];
		this.river4 = this.deck[3];
		this.river5 = this.deck[4];
		this.npc1card1 = this.deck[5];
		this.npc1card2 = this.deck[6];
		this.npc2card1 = this.deck[7];
		this.npc2card2 = this.deck[8];
		this.npc3card1 = this.deck[9];
		this.npc3card2 = this.deck[10];
		this.playerCard1 = this.deck[11];
		this.playerCard2 = this.deck[12];
		this.cardOb_p1 = Object.Instantiate<GameObject>(this.cardObj[this.playerCard1], this.playerC1Loc.position, this.playerC1Loc.rotation);
		this.cardOb_p2 = Object.Instantiate<GameObject>(this.cardObj[this.playerCard2], this.playerC2Loc.position, this.playerC2Loc.rotation);
		this.cardOb_p1.transform.parent = this.playerC1Loc;
		this.cardOb_p2.transform.parent = this.playerC2Loc;
		this.cardOb_n11 = Object.Instantiate<GameObject>(this.cardObj[this.npc1card1], this.n1c1Loc.position, this.n1c1Loc.rotation);
		this.cardOb_n12 = Object.Instantiate<GameObject>(this.cardObj[this.npc1card2], this.n1c2Loc.position, this.n1c2Loc.rotation);
		this.cardOb_n21 = Object.Instantiate<GameObject>(this.cardObj[this.npc2card1], this.n2c1Loc.position, this.n2c1Loc.rotation);
		this.cardOb_n22 = Object.Instantiate<GameObject>(this.cardObj[this.npc2card2], this.n2c2Loc.position, this.n2c2Loc.rotation);
		this.cardOb_n31 = Object.Instantiate<GameObject>(this.cardObj[this.npc3card1], this.n3c1Loc.position, this.n3c1Loc.rotation);
		this.cardOb_n32 = Object.Instantiate<GameObject>(this.cardObj[this.npc3card2], this.n3c2Loc.position, this.n3c2Loc.rotation);
		this.riverC1 = null;
		this.riverC2 = null;
		this.riverC3 = null;
		this.riverC4 = null;
		this.riverC5 = null;
		this.playerMoved = false;
		this.playerFolded = false;
		this.npc1Folded = false;
		this.npc2Folded = false;
		this.npc3Folded = false;
		this.GenerateChips(this.potLoc, 15);
		base.StartCoroutine(this.NpcPeer1());
		base.StartCoroutine(this.NpcPeer2());
		base.StartCoroutine(this.NpcPeer3());
		this.UpdateText();
		base.StartCoroutine(this.Think());
		Debug.Log("think-a");
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x0005F63C File Offset: 0x0005D83C
	private void BumpTurn()
	{
		if (this.turn == 1 && this.playerFolded)
		{
			this.turn = 2;
		}
		if (this.turn == 2 && this.npc1Folded)
		{
			this.turn = 3;
		}
		if (this.turn == 3 && this.npc2Folded)
		{
			this.turn = 4;
		}
		if (this.turn == 4 && this.npc3Folded)
		{
			this.turn = 1;
		}
		if (this.turn != 1)
		{
			this.HideButtons();
		}
		if (this.unansweredRaises == 0)
		{
			int num = 4;
			if (this.playerFolded)
			{
				num--;
			}
			if (this.npc1Folded)
			{
				num--;
			}
			if (this.npc2Folded)
			{
				num--;
			}
			if (this.npc3Folded)
			{
				num--;
			}
			if (this.numChecks >= num)
			{
				this.roundEnded = true;
				this.allChecked = true;
			}
			this.okToCheck = true;
		}
		this.okToCheck = true;
		if (!this.allChecked)
		{
			this.roundEnded = false;
		}
		if (this.turn == 1 && !this.playerFolded && !this.roundEnded)
		{
			this.playerMoved = false;
			this.DisplayActions();
			Debug.Log("display");
		}
		else if (this.turn == 2 && !this.roundEnded)
		{
			if (!this.npc1Folded)
			{
				base.StartCoroutine(this.Think());
				Debug.Log("think1");
				this.tempAction = this.CalculateTotal(2);
				this.npcAction = Random.Range(1, 4);
				if (this.tempAction > 40)
				{
					this.npcAction = Random.Range(2, 3);
				}
				if ((this.callAmount == 0f || this.unansweredRaises == 0) && this.npcAction == 1)
				{
					this.npcAction = 2;
				}
				if (this.npcAction == 1)
				{
					this.npc1Folded = true;
					this.numFolds++;
					if (this.unansweredRaises > 0)
					{
						this.unansweredRaises--;
					}
					Debug.Log("p1fold1320");
					this.npc1Anim.Play("n1fold");
					Object.Destroy(this.cardOb_n11);
					Object.Destroy(this.cardOb_n12);
					this.n1Text.text = "Fold";
					this.panelBg1.SetActive(true);
				}
				else if (this.npcAction == 2)
				{
					this.numChecks++;
					this.npc1Bet = this.callAmount;
					this.npc1Chips -= this.callAmount;
					if (this.unansweredRaises > 0)
					{
						this.unansweredRaises--;
					}
					if ((this.callAmount == 0f || this.unansweredRaises == 0) && this.okToCheck)
					{
						this.check = true;
						this.npc1Anim.Play("n1check");
						this.n1Text.text = "Check";
						this.panelBg1.SetActive(true);
					}
					this.potSize += this.callAmount;
					if (this.callAmount > 0f)
					{
						this.npc1Anim.Play("n1bet");
						base.StartCoroutine(this.GenerateChipsI(this.potLoc, (int)this.callAmount));
						this.n1Text.text = "Call " + this.callAmount;
						this.panelBg1.SetActive(true);
					}
				}
				else if (this.npcAction == 3)
				{
					this.numChecks = 1;
					float num2 = (float)Random.Range(15, 20);
					this.npc1Bet = this.callAmount + num2;
					this.callAmount = this.npc1Bet;
					this.npc1Chips -= this.callAmount;
					this.unansweredRaises = 3 - this.numFolds;
					this.potSize += this.callAmount;
					this.npc1Anim.Play("n1bet");
					base.StartCoroutine(this.GenerateChipsI(this.potLoc, (int)this.callAmount));
					this.n1Text.text = "Raise " + num2;
					this.panelBg1.SetActive(true);
				}
				this.npc1LastAction = this.npcAction;
			}
			this.UpdateText();
			base.StartCoroutine(this.CheckEnd());
			this.turn = 3;
		}
		else if (this.turn == 3 && !this.roundEnded)
		{
			if (!this.npc2Folded)
			{
				base.StartCoroutine(this.Think());
				Debug.Log("think2");
				this.npcAction = Random.Range(1, 4);
				this.tempAction = this.CalculateTotal(3);
				this.npcAction = Random.Range(1, 4);
				if (this.tempAction > 40)
				{
					this.npcAction = Random.Range(2, 3);
				}
				if ((this.callAmount == 0f || this.unansweredRaises == 0) && this.npcAction == 1)
				{
					this.npcAction = 2;
				}
				if (this.npcAction == 1)
				{
					this.npc2Folded = true;
					this.numFolds++;
					if (this.unansweredRaises > 0)
					{
						this.unansweredRaises--;
					}
					this.npc2Anim.Play("n2fold");
					Object.Destroy(this.cardOb_n21);
					Object.Destroy(this.cardOb_n22);
					this.n2Text.text = "Fold";
					this.panelBg2.SetActive(true);
				}
				else if (this.npcAction == 2)
				{
					this.numChecks++;
					this.npc2Bet = this.callAmount;
					if (this.unansweredRaises > 0)
					{
						this.unansweredRaises--;
					}
					if ((this.callAmount == 0f || this.unansweredRaises == 0) && this.okToCheck)
					{
						this.check = true;
						this.npc2Anim.Play("n2check");
						this.n2Text.text = "Check";
						this.panelBg2.SetActive(true);
					}
					this.npc2Chips -= this.callAmount;
					this.potSize += this.callAmount;
					if (this.callAmount > 0f)
					{
						base.StartCoroutine(this.GenerateChipsI(this.potLoc, (int)this.callAmount));
						this.npc2Anim.Play("n2bet");
						this.n2Text.text = "Call " + this.callAmount;
						this.panelBg2.SetActive(true);
					}
				}
				else if (this.npcAction == 3)
				{
					this.numChecks = 1;
					float num3 = (float)Random.Range(15, 20);
					this.npc2Bet = this.callAmount + num3;
					this.callAmount = this.npc2Bet;
					this.npc2Chips -= this.callAmount;
					this.unansweredRaises = 3 - this.numFolds;
					this.potSize += this.callAmount;
					this.npc2Anim.Play("n2bet");
					this.n2Text.text = "Raise " + num3;
					this.panelBg2.SetActive(true);
					base.StartCoroutine(this.GenerateChipsI(this.potLoc, (int)this.callAmount));
				}
				this.npc2LastAction = this.npcAction;
			}
			this.UpdateText();
			base.StartCoroutine(this.CheckEnd());
			this.turn = 4;
		}
		else if (this.turn == 4 && !this.roundEnded)
		{
			if (!this.npc3Folded)
			{
				base.StartCoroutine(this.Think());
				Debug.Log("think3");
				this.npcAction = Random.Range(1, 4);
				this.tempAction = this.CalculateTotal(4);
				this.npcAction = Random.Range(1, 4);
				if (this.tempAction > 40)
				{
					this.npcAction = Random.Range(2, 3);
				}
				if ((this.callAmount == 0f || this.unansweredRaises == 0) && this.npcAction == 1)
				{
					this.npcAction = 2;
				}
				if (this.npcAction == 1)
				{
					this.npc3Folded = true;
					this.numFolds++;
					if (this.unansweredRaises > 0)
					{
						this.unansweredRaises--;
					}
					this.npc3Anim.Play("n3fold");
					Object.Destroy(this.cardOb_n31);
					Object.Destroy(this.cardOb_n32);
					this.n3Text.text = "Fold";
					this.panelBg3.SetActive(true);
				}
				else if (this.npcAction == 2)
				{
					this.numChecks++;
					this.npc3Bet = this.callAmount;
					if (this.unansweredRaises > 0)
					{
						this.unansweredRaises--;
					}
					if ((this.callAmount == 0f || this.unansweredRaises == 0) && this.okToCheck)
					{
						this.check = true;
						this.npc3Anim.Play("n3check");
						this.n3Text.text = "Check";
						this.panelBg3.SetActive(true);
					}
					this.npc3Chips -= this.callAmount;
					this.potSize += this.callAmount;
					if (this.callAmount > 0f)
					{
						base.StartCoroutine(this.GenerateChipsI(this.potLoc, (int)this.callAmount));
						this.npc3Anim.Play("n3bet");
						this.n3Text.text = "Call " + this.callAmount;
						this.panelBg3.SetActive(true);
					}
				}
				else if (this.npcAction == 3)
				{
					this.numChecks = 1;
					float num4 = (float)Random.Range(15, 20);
					this.npc3Bet = this.callAmount + num4;
					this.callAmount = this.npc3Bet;
					this.npc3Chips -= this.callAmount;
					this.unansweredRaises = 3 - this.numFolds;
					this.potSize += this.callAmount;
					this.npc3Anim.Play("n3bet");
					this.n3Text.text = "Raise " + num4;
					this.panelBg3.SetActive(true);
					base.StartCoroutine(this.GenerateChipsI(this.potLoc, (int)this.callAmount));
				}
				this.npc3LastAction = this.npcAction;
			}
			this.UpdateText();
			base.StartCoroutine(this.CheckEnd());
			this.turn = 1;
		}
		if (this.roundEnded)
		{
			base.StartCoroutine(this.CheckEnd());
		}
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x000600C4 File Offset: 0x0005E2C4
	private void PlayerAction(int action, float raise)
	{
		this.HideButtons();
		if (this.turn == 1 && !this.playerFolded && !this.gameEnded)
		{
			if (action == 1)
			{
				this.playerFolded = true;
				this.numFolds++;
				if (this.unansweredRaises > 0)
				{
					this.unansweredRaises--;
				}
				Object.Destroy(this.cardOb_p1);
				Object.Destroy(this.cardOb_p2);
			}
			else if (action == 2)
			{
				this.numChecks++;
				this.potSize += this.callAmount;
				if (this.unansweredRaises > 0)
				{
					this.unansweredRaises--;
				}
				if ((this.callAmount == 0f || this.unansweredRaises == 0) && this.okToCheck)
				{
					this.check = true;
				}
				if (this.playerChips >= this.callAmount)
				{
					this.playerChips -= this.callAmount;
				}
				else
				{
					this.callAmount = this.playerChips;
					this.playerChips = 0f;
				}
				Debug.Log("ca" + this.callAmount);
				if (this.callAmount > 0f)
				{
					this.GenerateChips(this.potLoc, (int)this.callAmount);
				}
			}
			else if (action == 3)
			{
				this.numChecks = 1;
				this.potSize += this.callAmount + raise;
				this.callAmount += raise;
				this.unansweredRaises = 3 - this.numFolds;
				if (this.playerChips >= this.callAmount)
				{
					this.playerChips -= this.callAmount;
				}
				else
				{
					this.callAmount = this.playerChips;
					this.playerChips = 0f;
				}
				this.GenerateChips(this.potLoc, (int)this.callAmount);
			}
			this.playerLastAction = action;
			this.turn = 2;
			this.playerMoved = true;
			base.StartCoroutine(this.CheckEnd());
		}
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x00002188 File Offset: 0x00000388
	private void EvalHands()
	{
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x000602D4 File Offset: 0x0005E4D4
	public int CardVal(int card1, int card2)
	{
		string text = this.IntToCard(card1).Substring(0, 1);
		string text2 = this.IntToCard(card2).Substring(0, 1);
		if (text == "A")
		{
			this.val1 = 14;
		}
		else if (text == "K")
		{
			this.val1 = 13;
		}
		else if (text == "Q")
		{
			this.val1 = 12;
		}
		else if (text == "J")
		{
			this.val1 = 11;
		}
		else
		{
			this.val1 = int.Parse(text);
		}
		if (text2 == "A")
		{
			this.val2 = 14;
		}
		else if (text2 == "K")
		{
			this.val2 = 13;
		}
		else if (text2 == "Q")
		{
			this.val2 = 12;
		}
		else if (text2 == "J")
		{
			this.val2 = 11;
		}
		else
		{
			this.val2 = int.Parse(text2);
		}
		if (this.val1 > this.val2)
		{
			return this.val1;
		}
		return this.val2;
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x000603EC File Offset: 0x0005E5EC
	public int CalculateTotal(int place)
	{
		this.royalFlush = 0;
		this.straightFlush = 0;
		this.fullHouse = 0;
		this.four = 0;
		this.flush = 0;
		this.straight = 0;
		this.three = 0;
		this.twoPair = 0;
		this.pair = 0;
		this.highCard = 2;
		this.tempVal = 0;
		if ((place == 1 && !this.playerFolded) || (place == 2 && !this.npc1Folded) || (place == 3 && !this.npc2Folded) || (place == 4 && !this.npc3Folded))
		{
			if (place == 1)
			{
				this.card1x = this.playerCard1;
				this.card2x = this.playerCard2;
			}
			else if (place == 2)
			{
				this.card1x = this.npc1card1;
				this.card2x = this.npc1card2;
			}
			else if (place == 3)
			{
				this.card1x = this.npc2card1;
				this.card2x = this.npc2card2;
			}
			else if (place == 4)
			{
				this.card1x = this.npc3card1;
				this.card2x = this.npc3card2;
			}
			string[] array = new string[]
			{
				this.IntToCard(this.card1x).Substring(0, 1),
				this.IntToCard(this.card2x).Substring(0, 1),
				this.IntToCard(this.river1).Substring(0, 1),
				this.IntToCard(this.river2).Substring(0, 1),
				this.IntToCard(this.river3).Substring(0, 1),
				this.IntToCard(this.river4).Substring(0, 1),
				this.IntToCard(this.river5).Substring(0, 1)
			};
			string[] array2 = new string[]
			{
				this.IntToCard(this.card1x).Substring(1, 1),
				this.IntToCard(this.card2x).Substring(1, 1),
				this.IntToCard(this.river1).Substring(1, 1),
				this.IntToCard(this.river2).Substring(1, 1),
				this.IntToCard(this.river3).Substring(1, 1),
				this.IntToCard(this.river4).Substring(1, 1),
				this.IntToCard(this.river5).Substring(1, 1)
			};
			for (int i = 0; i < 7; i++)
			{
				if (array[i] == "A")
				{
					array[i] = "14";
				}
				if (array[i] == "K")
				{
					array[i] = "13";
				}
				if (array[i] == "Q")
				{
					array[i] = "12";
				}
				if (array[i] == "J")
				{
					array[i] = "11";
				}
			}
			int[] array3 = new int[]
			{
				int.Parse(array[0]),
				int.Parse(array[1]),
				int.Parse(array[2]),
				int.Parse(array[3]),
				int.Parse(array[4]),
				int.Parse(array[5]),
				int.Parse(array[6])
			};
			Array.Sort<int>(array3);
			int num = 0;
			for (int j = 0; j < 7; j++)
			{
				if (j < 6 && array3[j] == array3[j + 1])
				{
					this.pair = 1;
					if (num == 0)
					{
						num = array3[j];
						this.pairHighCard = array3[j];
					}
					if (num != 0 && num != array3[j])
					{
						this.twoPair = 1;
						if (num < array3[j])
						{
							this.pairHighCard = array3[j];
						}
					}
					if (j < 5 && array3[j] == array3[j + 2])
					{
						this.three = 1;
						if (j < 4 && array3[j] == array3[j + 3])
						{
							this.four = 1;
						}
					}
				}
			}
			if (this.three == 1 && this.twoPair > 0)
			{
				this.fullHouse = 1;
			}
			this.tempC = 0;
			this.tempD = 0;
			this.tempS = 0;
			this.tempH = 0;
			Array.Sort<string>(array2);
			for (int k = 0; k < 7; k++)
			{
				if (array2[k] == "H")
				{
					this.tempH++;
				}
				else if (array2[k] == "S")
				{
					this.tempS++;
				}
				else if (array2[k] == "C")
				{
					this.tempC++;
				}
				else if (array2[k] == "D")
				{
					this.tempD++;
				}
			}
			if (this.tempD > 4 || this.tempS > 4 || this.tempH > 4 || this.tempC > 4)
			{
				this.flush = 1;
			}
			int.Parse(array3[0].ToString());
			int.Parse(array3[1].ToString());
			int.Parse(array3[2].ToString());
			int.Parse(array3[3].ToString());
			int num2 = int.Parse(array3[4].ToString());
			int num3 = int.Parse(array3[5].ToString());
			int num4 = int.Parse(array3[6].ToString());
			Array.Sort<int>(array3);
			Array.Reverse(array3);
			this.highCard = num4;
			if (array3.Contains(num2 - 1) && array3.Contains(num2 - 2) && array3.Contains(num2 - 3) && array3.Contains(num2 - 4))
			{
				this.straight = 1;
				this.highCard = num2;
			}
			if (array3.Contains(num3 - 1) && array3.Contains(num3 - 2) && array3.Contains(num3 - 3) && array3.Contains(num3 - 4))
			{
				this.straight = 1;
				this.highCard = num3;
			}
			if (array3.Contains(num4 - 1) && array3.Contains(num4 - 2) && array3.Contains(num4 - 3) && array3.Contains(num4 - 4))
			{
				this.straight = 1;
				this.highCard = num4;
			}
			if (array3.Contains(2) && array3.Contains(3) && array3.Contains(4) && array3.Contains(5) && array3.Contains(14))
			{
				this.straight = 1;
				this.highCard = 14;
			}
			if (this.straight == 1 && this.flush == 1)
			{
				this.straightFlush = 1;
			}
			if (this.straightFlush == 1 && this.highCard == 14)
			{
				this.royalFlush = 1;
			}
			this.tempVal = this.highCard;
			this.nXhandName = "High Card";
			if (this.pair == 1)
			{
				this.tempVal = 20 + this.pairHighCard;
				this.nXhandName = "Pair";
			}
			if (this.twoPair == 1)
			{
				this.tempVal = 40 + this.pairHighCard;
				this.nXhandName = "Two Pair";
			}
			if (this.three == 1)
			{
				this.tempVal = 60 + this.pairHighCard;
				this.nXhandName = "3 of a Kind";
			}
			if (this.straight == 1)
			{
				this.tempVal = 80 + this.highCard;
				this.nXhandName = "Straight";
			}
			if (this.flush == 1)
			{
				this.tempVal = 100 + this.highCard;
				this.nXhandName = "Flush";
			}
			if (this.fullHouse == 1)
			{
				this.tempVal = 120 + this.pairHighCard;
				this.nXhandName = "Full House";
			}
			if (this.four == 1)
			{
				this.tempVal = 140 + this.pairHighCard;
				this.nXhandName = "4 of a Kind";
			}
			if (this.straightFlush == 1)
			{
				this.tempVal = 160 + this.highCard;
				this.nXhandName = "Straight Flush";
			}
			if (this.royalFlush == 1)
			{
				this.tempVal = 200;
				this.nXhandName = "Royal Flush";
			}
			if (place == 2)
			{
				this.n1handName = this.nXhandName;
			}
			if (place == 3)
			{
				this.n2handName = this.nXhandName;
			}
			if (place == 4)
			{
				this.n3handName = this.nXhandName;
			}
		}
		return this.tempVal;
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x00060BDC File Offset: 0x0005EDDC
	private void DisplayActions()
	{
		if (!this.playerFolded && (!this.npc1Folded || !this.npc2Folded || !this.npc3Folded) && this.sitting)
		{
			base.StartCoroutine(this.ShowButtons());
		}
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x00060C13 File Offset: 0x0005EE13
	public void Raise()
	{
		this.PlayerAction(3, 20f);
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x00060C21 File Offset: 0x0005EE21
	public void Fold()
	{
		this.PlayerAction(1, 0f);
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x00060C2F File Offset: 0x0005EE2F
	public void Call()
	{
		this.PlayerAction(2, 0f);
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x00060C3D File Offset: 0x0005EE3D
	private void HideButtons()
	{
		this.pokerCanvas.SetActive(false);
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x00060C4B File Offset: 0x0005EE4B
	private IEnumerator ShowButtons()
	{
		yield return new WaitForSeconds(1f);
		if (this.callAmount > 0f && !this.bbCompensated && this.bigBlindPos == 1)
		{
			this.callAmount -= this.bigBlind;
			this.bbCompensated = true;
		}
		if (this.callAmount > 0f && !this.sbCompensated && this.smallBlindPos == 1)
		{
			this.callAmount -= this.smallBlind;
			this.sbCompensated = true;
		}
		if (this.turn == 1 && !this.roundEnded && !this.gameEnded && this.sitting)
		{
			this.pokerCanvas.SetActive(true);
			if (this.callAmount > 0f)
			{
				this.callText.text = "Call " + this.callAmount;
			}
			else
			{
				this.callText.text = "Check";
			}
		}
		yield break;
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x00060C5A File Offset: 0x0005EE5A
	private IEnumerator CheckEnd()
	{
		yield return new WaitForSeconds(2f);
		this.UpdateText();
		if (this.numFolds > 2 && !this.gameEnded)
		{
			this.gameEnded = true;
			if (!this.playerFolded)
			{
				this.playerChips += this.potSize;
				this.GenerateChips(this.playerChipsLoc, (int)this.potSize);
			}
			else if (!this.npc1Folded)
			{
				this.npc1Chips += this.potSize;
				this.GenerateChips(this.n1ChipsLoc, (int)this.potSize);
			}
			else if (!this.npc2Folded)
			{
				this.npc2Chips += this.potSize;
				this.GenerateChips(this.n2ChipsLoc, (int)this.potSize);
			}
			else if (!this.npc3Folded)
			{
				this.npc3Chips += this.potSize;
				this.GenerateChips(this.n3ChipsLoc, (int)this.potSize);
			}
			this.paid = true;
			base.StartCoroutine(this.DestroyPot());
		}
		if (this.unansweredRaises < 1 && !this.gameEnded && this.allChecked)
		{
			this.check = false;
			this.roundEnded = true;
			this.callAmount = 0f;
			this.unansweredRaises = 0;
			this.playerMoved = false;
			this.okToCheck = false;
			this.numChecks = 0;
			this.playerLastAction = 0;
			this.npc1LastAction = 0;
			this.npc2LastAction = 0;
			this.npc3LastAction = 0;
			this.allChecked = false;
			if (this.riverC5 != null)
			{
				this.gameEnded = true;
			}
			else if (this.riverC5 == null && this.riverC4 != null)
			{
				this.riverC5 = Object.Instantiate<GameObject>(this.cardObj[this.river5], this.river5T.position, this.river5T.rotation);
				this.aSources[4].Play();
			}
			else if (this.riverC4 == null && this.riverC1 != null)
			{
				this.riverC4 = Object.Instantiate<GameObject>(this.cardObj[this.river4], this.river4T.position, this.river4T.rotation);
				this.aSources[4].Play();
			}
			else if (this.riverC1 == null)
			{
				this.aSources[4].Play();
				this.riverC1 = Object.Instantiate<GameObject>(this.cardObj[this.river1], this.river1T.position, this.river1T.rotation);
				this.riverC2 = Object.Instantiate<GameObject>(this.cardObj[this.river2], this.river2T.position, this.river2T.rotation);
				this.riverC3 = Object.Instantiate<GameObject>(this.cardObj[this.river3], this.river3T.position, this.river3T.rotation);
			}
		}
		if (this.gameEnded && !this.paid)
		{
			this.handTotalp = this.CalculateTotal(1);
			this.handTotaln1 = this.CalculateTotal(2);
			this.handTotaln2 = this.CalculateTotal(3);
			this.handTotaln3 = this.CalculateTotal(4);
			if (!this.npc1Folded)
			{
				this.cardOb_n11.transform.RotateAround(this.cardOb_n11.transform.position, this.cardOb_n11.transform.up, 180f);
				this.cardOb_n12.transform.RotateAround(this.cardOb_n12.transform.position, this.cardOb_n12.transform.up, 180f);
			}
			if (!this.npc2Folded)
			{
				this.cardOb_n21.transform.RotateAround(this.cardOb_n21.transform.position, this.cardOb_n21.transform.up, 180f);
				this.cardOb_n22.transform.RotateAround(this.cardOb_n22.transform.position, this.cardOb_n22.transform.up, 180f);
			}
			if (!this.npc3Folded)
			{
				this.cardOb_n31.transform.RotateAround(this.cardOb_n31.transform.position, this.cardOb_n31.transform.up, 180f);
				this.cardOb_n32.transform.RotateAround(this.cardOb_n32.transform.position, this.cardOb_n32.transform.up, 180f);
			}
			Debug.Log(this.handTotalp);
			Debug.Log(this.handTotaln1);
			Debug.Log(this.handTotaln2);
			Debug.Log(this.handTotaln3);
			if (this.handTotalp > this.handTotaln1 && this.handTotalp > this.handTotaln2 && this.handTotalp > this.handTotaln3)
			{
				this.playerChips += this.potSize;
				Debug.Log("won:playerChips");
				Debug.Log("ht " + this.handTotalp);
				this.GenerateChips(this.playerChipsLoc, (int)this.potSize);
			}
			if (this.handTotaln1 > this.handTotalp && this.handTotaln1 > this.handTotaln2 && this.handTotaln1 > this.handTotaln3)
			{
				this.npc1Chips += this.potSize;
				Debug.Log("won:npc1");
				Debug.Log("ht " + this.handTotaln1);
				this.GenerateChips(this.n1ChipsLoc, (int)this.potSize);
			}
			if (this.handTotaln2 > this.handTotalp && this.handTotaln2 > this.handTotaln1 && this.handTotaln2 > this.handTotaln3)
			{
				this.npc2Chips += this.potSize;
				Debug.Log("won:npc2");
				Debug.Log("ht " + this.handTotaln2);
				this.GenerateChips(this.n2ChipsLoc, (int)this.potSize);
			}
			if (this.handTotaln3 > this.handTotalp && this.handTotaln3 > this.handTotaln1 && this.handTotaln3 > this.handTotaln2)
			{
				this.npc3Chips += this.potSize;
				Debug.Log("won:npc3");
				Debug.Log("ht " + this.handTotaln3);
				this.GenerateChips(this.n3ChipsLoc, (int)this.potSize);
			}
			this.paid = true;
			base.StartCoroutine(this.DestroyPot());
		}
		this.UpdateText();
		if (!this.gameEnded && (this.roundEnded || this.playerMoved))
		{
			Debug.Log("thinkx");
			base.StartCoroutine(this.Think());
		}
		yield break;
	}

	// Token: 0x0600075E RID: 1886 RVA: 0x00060C69 File Offset: 0x0005EE69
	private IEnumerator DestroyPot()
	{
		Debug.Log("destroypot");
		this.HideButtons();
		if (!this.npc1Folded)
		{
			this.n1Text.text = this.n1handName;
		}
		if (!this.npc2Folded)
		{
			this.n2Text.text = this.n2handName;
		}
		if (!this.npc3Folded)
		{
			this.n3Text.text = this.n3handName;
		}
		yield return new WaitForSeconds(4f);
		this.n1Text.text = "";
		this.panelBg1.SetActive(false);
		this.n2Text.text = "";
		this.panelBg2.SetActive(false);
		this.n3Text.text = "";
		this.panelBg3.SetActive(false);
		foreach (object obj in this.potLoc)
		{
			Object.Destroy(((Transform)obj).gameObject);
		}
		if (this.cardOb_p1 != null)
		{
			Object.Destroy(this.cardOb_p1);
			Object.Destroy(this.cardOb_p2);
		}
		if (!this.npc1Folded)
		{
			Object.Destroy(this.cardOb_n11);
			Object.Destroy(this.cardOb_n12);
		}
		if (!this.npc2Folded)
		{
			Object.Destroy(this.cardOb_n21);
			Object.Destroy(this.cardOb_n22);
		}
		if (!this.npc3Folded)
		{
			Object.Destroy(this.cardOb_n31);
			Object.Destroy(this.cardOb_n32);
		}
		if (this.riverC1 != null)
		{
			Object.Destroy(this.riverC1);
		}
		if (this.riverC2 != null)
		{
			Object.Destroy(this.riverC2);
		}
		if (this.riverC3 != null)
		{
			Object.Destroy(this.riverC3);
		}
		if (this.riverC4 != null)
		{
			Object.Destroy(this.riverC4);
		}
		if (this.riverC5 != null)
		{
			Object.Destroy(this.riverC5);
		}
		base.StartCoroutine(this.StartNewRound());
		yield break;
	}

	// Token: 0x0600075F RID: 1887 RVA: 0x00060C78 File Offset: 0x0005EE78
	private IEnumerator Think()
	{
		yield return new WaitForSeconds(3f);
		this.BumpTurn();
		yield break;
	}

	// Token: 0x06000760 RID: 1888 RVA: 0x00060C87 File Offset: 0x0005EE87
	private IEnumerator NpcPeer1()
	{
		float seconds = Random.Range(0.5f, 2f);
		yield return new WaitForSeconds(seconds);
		yield break;
	}

	// Token: 0x06000761 RID: 1889 RVA: 0x00060C8F File Offset: 0x0005EE8F
	private IEnumerator NpcPeer2()
	{
		float seconds = Random.Range(0.5f, 2f);
		yield return new WaitForSeconds(seconds);
		yield break;
	}

	// Token: 0x06000762 RID: 1890 RVA: 0x00060C97 File Offset: 0x0005EE97
	private IEnumerator NpcPeer3()
	{
		float seconds = Random.Range(0.5f, 2f);
		yield return new WaitForSeconds(seconds);
		yield break;
	}

	// Token: 0x06000763 RID: 1891 RVA: 0x00060C9F File Offset: 0x0005EE9F
	private IEnumerator MakeKinematicAfterDelay(GameObject chip, float delay)
	{
		yield return new WaitForSeconds(delay);
		Rigidbody component = chip.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = true;
		}
		yield break;
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x00060CB8 File Offset: 0x0005EEB8
	public void StandUp()
	{
		this.fpc.canMove = true;
		this.gameEnded = true;
		this.person.transform.position = this.exitMount.transform.position;
		this.playerFolded = true;
		if (this.cardOb_p1 != null)
		{
			Object.Destroy(this.cardOb_p1);
			Object.Destroy(this.cardOb_p2);
		}
		foreach (object obj in this.playerChipsLoc)
		{
			Object.Destroy(((Transform)obj).gameObject);
		}
		this.HideButtons();
		if (this.playerChips > 0f)
		{
			Object.Instantiate<GameObject>(this.moneyRoll, this.playerChipsLoc.position, this.playerChipsLoc.rotation).GetComponent<PickUp>().thisDurability = this.playerChips;
		}
		this.sitting = false;
	}

	// Token: 0x06000765 RID: 1893 RVA: 0x00060DBC File Offset: 0x0005EFBC
	private void SitDown()
	{
		this.fpc.canMove = false;
		this.person.transform.position = this.seatMount.transform.position;
	}

	// Token: 0x06000766 RID: 1894 RVA: 0x00060DEC File Offset: 0x0005EFEC
	public string IntToCard(int card)
	{
		card++;
		if (card < 14)
		{
			if (card == 1)
			{
				this.tempCard = "A";
			}
			else if (card == 11)
			{
				this.tempCard = "J";
			}
			else if (card == 12)
			{
				this.tempCard = "Q";
			}
			else if (card == 13)
			{
				this.tempCard = "K";
			}
			else
			{
				this.tempCard = card.ToString();
			}
			this.tempSuit = "H";
		}
		else if (card > 13 && card < 27)
		{
			if (card == 14)
			{
				this.tempCard = "A";
			}
			else if (card == 24)
			{
				this.tempCard = "J";
			}
			else if (card == 25)
			{
				this.tempCard = "Q";
			}
			else if (card == 26)
			{
				this.tempCard = "K";
			}
			else
			{
				this.tempCard = (card - 13).ToString();
			}
			this.tempSuit = "D";
		}
		else if (card > 26 && card < 40)
		{
			if (card == 27)
			{
				this.tempCard = "A";
			}
			else if (card == 37)
			{
				this.tempCard = "J";
			}
			else if (card == 38)
			{
				this.tempCard = "Q";
			}
			else if (card == 39)
			{
				this.tempCard = "K";
			}
			else
			{
				this.tempCard = (card - 26).ToString();
			}
			this.tempSuit = "C";
		}
		else if (card > 39)
		{
			if (card == 40)
			{
				this.tempCard = "A";
			}
			else if (card == 50)
			{
				this.tempCard = "J";
			}
			else if (card == 51)
			{
				this.tempCard = "Q";
			}
			else if (card == 52)
			{
				this.tempCard = "K";
			}
			else
			{
				this.tempCard = (card - 39).ToString();
			}
			this.tempSuit = "S";
		}
		return this.tempCard + this.tempSuit;
	}

	// Token: 0x0400104F RID: 4175
	public FirstPersonController fpc;

	// Token: 0x04001050 RID: 4176
	public Transform seatMount;

	// Token: 0x04001051 RID: 4177
	public Transform exitMount;

	// Token: 0x04001052 RID: 4178
	public GameObject person;

	// Token: 0x04001053 RID: 4179
	public Animator npc1Anim;

	// Token: 0x04001054 RID: 4180
	public Animator npc2Anim;

	// Token: 0x04001055 RID: 4181
	public Animator npc3Anim;

	// Token: 0x04001056 RID: 4182
	public GameObject audio;

	// Token: 0x04001057 RID: 4183
	private AudioSource[] aSources;

	// Token: 0x04001058 RID: 4184
	public int[] deck;

	// Token: 0x04001059 RID: 4185
	public int[] tempCardD;

	// Token: 0x0400105A RID: 4186
	public GameObject[] cardObj;

	// Token: 0x0400105B RID: 4187
	public int npc1card1;

	// Token: 0x0400105C RID: 4188
	public int npc1card2;

	// Token: 0x0400105D RID: 4189
	public int npc2card1;

	// Token: 0x0400105E RID: 4190
	public int npc2card2;

	// Token: 0x0400105F RID: 4191
	public int npc3card1;

	// Token: 0x04001060 RID: 4192
	public int npc3card2;

	// Token: 0x04001061 RID: 4193
	public int playerCard1;

	// Token: 0x04001062 RID: 4194
	public int playerCard2;

	// Token: 0x04001063 RID: 4195
	private int smallBlindPos;

	// Token: 0x04001064 RID: 4196
	private int bigBlindPos;

	// Token: 0x04001065 RID: 4197
	private bool sbCompensated;

	// Token: 0x04001066 RID: 4198
	private bool bbCompensated;

	// Token: 0x04001067 RID: 4199
	private bool buyingIn;

	// Token: 0x04001068 RID: 4200
	private float npc1Chips;

	// Token: 0x04001069 RID: 4201
	private float npc2Chips;

	// Token: 0x0400106A RID: 4202
	private float npc3Chips;

	// Token: 0x0400106B RID: 4203
	private float playerChips;

	// Token: 0x0400106C RID: 4204
	private int numFolds;

	// Token: 0x0400106D RID: 4205
	private float npc1Bet;

	// Token: 0x0400106E RID: 4206
	private float npc2Bet;

	// Token: 0x0400106F RID: 4207
	private float npc3Bet;

	// Token: 0x04001070 RID: 4208
	private float playerBet;

	// Token: 0x04001071 RID: 4209
	private float callAmount;

	// Token: 0x04001072 RID: 4210
	private float potSize;

	// Token: 0x04001073 RID: 4211
	private bool npc1Folded;

	// Token: 0x04001074 RID: 4212
	private bool npc2Folded;

	// Token: 0x04001075 RID: 4213
	private bool npc3Folded;

	// Token: 0x04001076 RID: 4214
	private bool playerFolded;

	// Token: 0x04001077 RID: 4215
	private int river1;

	// Token: 0x04001078 RID: 4216
	private int river2;

	// Token: 0x04001079 RID: 4217
	private int river3;

	// Token: 0x0400107A RID: 4218
	private int river4;

	// Token: 0x0400107B RID: 4219
	private int river5;

	// Token: 0x0400107C RID: 4220
	public GameObject riverC1;

	// Token: 0x0400107D RID: 4221
	public GameObject riverC2;

	// Token: 0x0400107E RID: 4222
	public GameObject riverC3;

	// Token: 0x0400107F RID: 4223
	public GameObject riverC4;

	// Token: 0x04001080 RID: 4224
	public GameObject riverC5;

	// Token: 0x04001081 RID: 4225
	public Transform river1T;

	// Token: 0x04001082 RID: 4226
	public Transform river2T;

	// Token: 0x04001083 RID: 4227
	public Transform river3T;

	// Token: 0x04001084 RID: 4228
	public Transform river4T;

	// Token: 0x04001085 RID: 4229
	public Transform river5T;

	// Token: 0x04001086 RID: 4230
	public Transform playerChipsLoc;

	// Token: 0x04001087 RID: 4231
	public Transform n1ChipsLoc;

	// Token: 0x04001088 RID: 4232
	public Transform n2ChipsLoc;

	// Token: 0x04001089 RID: 4233
	public Transform n3ChipsLoc;

	// Token: 0x0400108A RID: 4234
	public Transform potLoc;

	// Token: 0x0400108B RID: 4235
	public Transform playerC1Loc;

	// Token: 0x0400108C RID: 4236
	public Transform playerC2Loc;

	// Token: 0x0400108D RID: 4237
	public Transform n1c1Loc;

	// Token: 0x0400108E RID: 4238
	public Transform n1c2Loc;

	// Token: 0x0400108F RID: 4239
	public Transform n2c1Loc;

	// Token: 0x04001090 RID: 4240
	public Transform n2c2Loc;

	// Token: 0x04001091 RID: 4241
	public Transform n3c1Loc;

	// Token: 0x04001092 RID: 4242
	public Transform n3c2Loc;

	// Token: 0x04001093 RID: 4243
	private GameObject cardOb_p1;

	// Token: 0x04001094 RID: 4244
	private GameObject cardOb_p2;

	// Token: 0x04001095 RID: 4245
	public GameObject cardOb_n11;

	// Token: 0x04001096 RID: 4246
	public GameObject cardOb_n12;

	// Token: 0x04001097 RID: 4247
	public GameObject cardOb_n21;

	// Token: 0x04001098 RID: 4248
	public GameObject cardOb_n22;

	// Token: 0x04001099 RID: 4249
	public GameObject cardOb_n31;

	// Token: 0x0400109A RID: 4250
	public GameObject cardOb_n32;

	// Token: 0x0400109B RID: 4251
	public GameObject chip1;

	// Token: 0x0400109C RID: 4252
	public GameObject chip5;

	// Token: 0x0400109D RID: 4253
	public GameObject chip25;

	// Token: 0x0400109E RID: 4254
	public GameObject chip100;

	// Token: 0x0400109F RID: 4255
	private int turn;

	// Token: 0x040010A0 RID: 4256
	private float bigBlind = 10f;

	// Token: 0x040010A1 RID: 4257
	private float smallBlind = 5f;

	// Token: 0x040010A2 RID: 4258
	private bool playerOut;

	// Token: 0x040010A3 RID: 4259
	private bool npc1Out;

	// Token: 0x040010A4 RID: 4260
	private bool npc2Out;

	// Token: 0x040010A5 RID: 4261
	private bool npc3Out;

	// Token: 0x040010A6 RID: 4262
	private bool roundEnded;

	// Token: 0x040010A7 RID: 4263
	private bool gameEnded;

	// Token: 0x040010A8 RID: 4264
	private int playerLastAction;

	// Token: 0x040010A9 RID: 4265
	private int npc1LastAction;

	// Token: 0x040010AA RID: 4266
	private int npc2LastAction;

	// Token: 0x040010AB RID: 4267
	private int npc3LastAction;

	// Token: 0x040010AC RID: 4268
	private int npcAction;

	// Token: 0x040010AD RID: 4269
	private string tempCard;

	// Token: 0x040010AE RID: 4270
	private string tempSuit;

	// Token: 0x040010AF RID: 4271
	public int unansweredRaises;

	// Token: 0x040010B0 RID: 4272
	public GameObject pokerCanvas;

	// Token: 0x040010B1 RID: 4273
	public GameObject playerText;

	// Token: 0x040010B2 RID: 4274
	public GameObject npc1Text;

	// Token: 0x040010B3 RID: 4275
	public GameObject npc2Text;

	// Token: 0x040010B4 RID: 4276
	public GameObject npc3Text;

	// Token: 0x040010B5 RID: 4277
	public GameObject potText;

	// Token: 0x040010B6 RID: 4278
	public GameObject pcards;

	// Token: 0x040010B7 RID: 4279
	public GameObject n1cards;

	// Token: 0x040010B8 RID: 4280
	public GameObject n2cards;

	// Token: 0x040010B9 RID: 4281
	public GameObject n3cards;

	// Token: 0x040010BA RID: 4282
	public GameObject paction;

	// Token: 0x040010BB RID: 4283
	public GameObject n1action;

	// Token: 0x040010BC RID: 4284
	public GameObject n2action;

	// Token: 0x040010BD RID: 4285
	public GameObject n3action;

	// Token: 0x040010BE RID: 4286
	public Text callText;

	// Token: 0x040010BF RID: 4287
	public GameObject foldButton;

	// Token: 0x040010C0 RID: 4288
	public GameObject callButton;

	// Token: 0x040010C1 RID: 4289
	public GameObject raiseButton;

	// Token: 0x040010C2 RID: 4290
	public int handTotalp;

	// Token: 0x040010C3 RID: 4291
	public int handTotaln1;

	// Token: 0x040010C4 RID: 4292
	public int handTotaln2;

	// Token: 0x040010C5 RID: 4293
	public int handTotaln3;

	// Token: 0x040010C6 RID: 4294
	private int val1;

	// Token: 0x040010C7 RID: 4295
	private int val2;

	// Token: 0x040010C8 RID: 4296
	private int tempVal;

	// Token: 0x040010C9 RID: 4297
	private int tempH;

	// Token: 0x040010CA RID: 4298
	private int tempD;

	// Token: 0x040010CB RID: 4299
	private int tempC;

	// Token: 0x040010CC RID: 4300
	private int tempS;

	// Token: 0x040010CD RID: 4301
	private int royalFlush;

	// Token: 0x040010CE RID: 4302
	private int straightFlush;

	// Token: 0x040010CF RID: 4303
	private int fullHouse;

	// Token: 0x040010D0 RID: 4304
	private int four;

	// Token: 0x040010D1 RID: 4305
	private int flush;

	// Token: 0x040010D2 RID: 4306
	private int straight;

	// Token: 0x040010D3 RID: 4307
	private int three;

	// Token: 0x040010D4 RID: 4308
	private int twoPair;

	// Token: 0x040010D5 RID: 4309
	private int pair;

	// Token: 0x040010D6 RID: 4310
	private int highCard;

	// Token: 0x040010D7 RID: 4311
	private bool paid;

	// Token: 0x040010D8 RID: 4312
	private bool check;

	// Token: 0x040010D9 RID: 4313
	private int card1x;

	// Token: 0x040010DA RID: 4314
	private int card2x;

	// Token: 0x040010DB RID: 4315
	private bool playerMoved;

	// Token: 0x040010DC RID: 4316
	private bool okToCheck;

	// Token: 0x040010DD RID: 4317
	private bool allChecked;

	// Token: 0x040010DE RID: 4318
	private int numChecks;

	// Token: 0x040010DF RID: 4319
	private Vector3 chip1V3;

	// Token: 0x040010E0 RID: 4320
	private Vector3 chip5V3;

	// Token: 0x040010E1 RID: 4321
	private Vector3 chip25V3;

	// Token: 0x040010E2 RID: 4322
	public GameObject moneyRoll;

	// Token: 0x040010E3 RID: 4323
	public Currency currency;

	// Token: 0x040010E4 RID: 4324
	public bool sitting;

	// Token: 0x040010E5 RID: 4325
	private int pairHighCard;

	// Token: 0x040010E6 RID: 4326
	public GameObject panelBg1;

	// Token: 0x040010E7 RID: 4327
	public GameObject panelBg2;

	// Token: 0x040010E8 RID: 4328
	public GameObject panelBg3;

	// Token: 0x040010E9 RID: 4329
	public Text n1Text;

	// Token: 0x040010EA RID: 4330
	public Text n2Text;

	// Token: 0x040010EB RID: 4331
	public Text n3Text;

	// Token: 0x040010EC RID: 4332
	private string n1handName;

	// Token: 0x040010ED RID: 4333
	private string n2handName;

	// Token: 0x040010EE RID: 4334
	private string n3handName;

	// Token: 0x040010EF RID: 4335
	private string nXhandName;

	// Token: 0x040010F0 RID: 4336
	private int tempAction;
}
