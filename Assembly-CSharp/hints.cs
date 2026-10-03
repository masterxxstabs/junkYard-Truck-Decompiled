using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000175 RID: 373
public class hints : MonoBehaviour
{
	// Token: 0x06000930 RID: 2352 RVA: 0x0007D1AD File Offset: 0x0007B3AD
	private void Start()
	{
		this.max = this.hinttext.Length;
		this.hintNum = Random.Range(0, this.max);
		this.ShowHint();
	}

	// Token: 0x06000931 RID: 2353 RVA: 0x0007D1D5 File Offset: 0x0007B3D5
	public void ShowHint()
	{
		this.hinttextOb.GetComponent<Text>().text = this.hinttext[this.hintNum];
	}

	// Token: 0x06000932 RID: 2354 RVA: 0x0007D1F4 File Offset: 0x0007B3F4
	public void NextHint()
	{
		this.hintNum++;
		if (this.hintNum >= this.max)
		{
			this.hintNum = 0;
		}
		this.ShowHint();
	}

	// Token: 0x06000933 RID: 2355 RVA: 0x0007D21F File Offset: 0x0007B41F
	public void PrevHint()
	{
		this.hintNum--;
		if (this.hintNum < 0)
		{
			this.hintNum = this.max;
		}
		this.ShowHint();
	}

	// Token: 0x04001897 RID: 6295
	private int max;

	// Token: 0x04001898 RID: 6296
	private int hintNum;

	// Token: 0x04001899 RID: 6297
	public GameObject hintText;

	// Token: 0x0400189A RID: 6298
	public GameObject hinttextOb;

	// Token: 0x0400189B RID: 6299
	private string[] hinttext = new string[]
	{
		"Need to hop a tall fence? Park your vehicle next to it, climb on top of your vehicle, and jump from there.",
		"A full service manual can be found in your garage.",
		"New jobs regularly appear on your computer. Or you can create a randomly generated mission using the job board at the gas station.",
		"Use your phone to fast travel or recover your vehicles for a small fee.",
		"There's a faucet you can use on the side of your garage. There's also another faucet at the gas station.",
		"Don't know what's wrong with your truck? Jimmy Junks Auto Repair can perform an inspection. Jimmy can also fix it, but it's much cheaper to fix it yourself.",
		"You can buy food and drinks at the mini mart.",
		"You will move very slowly if you don't eat, sleep, or stay hydrated. Carrying heavy objects dehydrates you.",
		"Your truck comes with an off road jack. It can help if you get stuck or need to change a tire. It's mounted to the inside if your truck bed.",
		"Maybe you wedged your vehicle between two trees and got trapped in the cab because both doors were pinned shut. You can still climb out of the vehicle by pressing [X].",
		"After going to sleep, you will wake up at 6:30 AM, unless your sanity level is low. Low sanity levels make your sleep schedule unpredictable and make you more tired in the morning.",
		"Owned something that just disappeared or fell into the void? Check the lost and found area near Jimmy Junks's Auto Repair.",
		"Your car doesn't need maintenance other than topping off the oil regularly. It burns oil and will start overheating if it gets too low.",
		"If you've mined all of the iron ore on the hillside, a rainy day will eventually come along and expose more ore and loosen some dirt.",
		"In the abandoned iron quarry, iron ore can be seen exposed in the hillside. It can also be found underneath chunks of loose dirt.",
		"Dead trees marked with a red ribbon can be cut down with the chainsaw, and then cut into smaller pieces to transport to the forge.",
		"The pickaxe can be carried, but is never part of your regular toolset. It's best to leave the pickaxe at the mining site.",
		"Early on you will encounter several missions and off road challenges that seem impossible. Come back to them later after you've made some upgrades to your truck.",
		"It's easy to get lost and mismanage your time and money in the beginning. Sometimes starting over and getting a stronger start is the best way forward.",
		"Challenges can be obtained from NPCs or by getting in close proximity to the challenge.",
		"4WD can be engaged by manually moving the 4WD shifer.",
		"You can place small items in your inventory by holding them and pressing [i] or [middle mouse button].",
		"Cash is treated like an item. If you want to buy something, don't forget to bring your cash.",
		"Having too much cash can clutter your inventory. You might have to stockpile your rolls of cash somewhere. Or deposit it using the ATM.",
		"Lean in by pressing [Q]. This helps you see bolts, and helps with drinking water from the faucet.",
		"You can save your game using the computer in your garage, the payphone at the auto parts store, or the payphone at the junkyard.",
		"Revving the engine and dropping the clutch will give you a quick boost of power, but it's not good for your clutch and transmission system.",
		"The welder in the garage can be used to fix body panels. It consumes welding wire which you will need to buy."
	};
}
