using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.ImageEffects;

// Token: 0x0200012D RID: 301
public class Currency : MonoBehaviour
{
	// Token: 0x060007C9 RID: 1993 RVA: 0x00063D18 File Offset: 0x00061F18
	private void Start()
	{
		this.invokeScale = 1f;
		this.runScale = 1f;
		Application.targetFrameRate = 60;
		this.foodBarL = this.foodBar.rectTransform.localScale;
		this.foodBarL.x = this.hunger / 100f;
		this.waterBarL = this.waterBar.rectTransform.localScale;
		this.waterBarL.x = this.water / 100f;
		this.sleepBarL = this.sleepBar.rectTransform.localScale;
		this.sleepBarL.x = this.sleep / 100f;
		this.sanityBarL = this.sanityBar.rectTransform.localScale;
		this.sanityBarL.x = this.stress / 100f;
		base.InvokeRepeating("losehunger", 0f, 18f);
		if (this.atm.ownsHouse)
		{
			this.frontDoor.GetComponent<InteractiveObject>().enabled = true;
		}
		if (this.hunger < 0f)
		{
			this.hunger = 1f;
		}
		base.StartCoroutine(this.RecordPlayTime());
	}

	// Token: 0x060007CA RID: 1994 RVA: 0x00063E4E File Offset: 0x0006204E
	private IEnumerator RecordPlayTime()
	{
		for (;;)
		{
			yield return new WaitForSeconds(60f);
			this.playMinutes++;
		}
		yield break;
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x00063E60 File Offset: 0x00062060
	private void MoveSlow()
	{
		if (this.strength > 26)
		{
			this.fpc.m_RunSpeed = 6f;
			this.fpc.m_WalkSpeed = 2.6f;
		}
		else if (this.strength > 13)
		{
			this.fpc.m_RunSpeed = 5f;
			this.fpc.m_WalkSpeed = 2.3f;
		}
		else
		{
			this.fpc.m_RunSpeed = 3f;
			this.fpc.m_WalkSpeed = 2f;
		}
		this.fpc.lowStats = true;
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x00063EF4 File Offset: 0x000620F4
	private void MoveRegular()
	{
		this.fpc.m_RunSpeed = 8f;
		if (this.strength > 13)
		{
			this.fpc.m_RunSpeed = 9f;
		}
		this.fpc.m_WalkSpeed = 3f;
		this.fpc.lowStats = false;
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x00063F48 File Offset: 0x00062148
	private void losehunger()
	{
		if (this.hunger > 0f)
		{
			this.hunger -= 0.6f;
			this.foodBarL.x = this.hunger / 100f;
		}
		if (this.water > 0f)
		{
			this.water -= 0.6f * (this.runScale + this.invokeScale);
			this.waterBarL.x = this.water / 100f;
		}
		if (this.stress > 0f)
		{
			this.stress -= 0.2f;
			this.sanityBarL.x = this.stress / 100f;
		}
		if (this.nicotine > 0f)
		{
			this.nicotine -= 0.03f;
		}
		if (this.sleep > 0f)
		{
			this.sleep -= 0.4f;
			if (this.stress == 0f)
			{
				this.sleep -= 0.2f;
			}
			this.sleepBarL.x = this.sleep / 100f;
		}
		if (this.water < 2f || this.hunger < 2f || this.sleep < 2f || this.invokeScale > 1f)
		{
			this.MoveSlow();
		}
		else
		{
			this.MoveRegular();
		}
		if (this.drunk > 0)
		{
			this.drunk--;
			if (this.drunk < 6)
			{
				Camera.main.GetComponent<MotionBlur>().enabled = false;
			}
			if (this.drunk < 13)
			{
				Camera.main.GetComponent<BlurOptimized>().enabled = false;
			}
			if (this.drunk < 20)
			{
				Camera.main.GetComponent<Fisheye>().enabled = false;
			}
		}
		this.fpc.drunk = (float)this.drunk;
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x00064131 File Offset: 0x00062331
	public void WakeUp()
	{
		base.StartCoroutine(this.WakeUpC());
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x00064140 File Offset: 0x00062340
	private IEnumerator WakeUpC()
	{
		Camera.main.GetComponent<BlurOptimized>().enabled = true;
		yield return new WaitForSeconds(2f);
		this.DisableDrunk();
		yield break;
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x0006414F File Offset: 0x0006234F
	public void DisableDrunk()
	{
		Camera.main.GetComponent<Fisheye>().enabled = false;
		Camera.main.GetComponent<BlurOptimized>().enabled = false;
		Camera.main.GetComponent<MotionBlur>().enabled = false;
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x00064184 File Offset: 0x00062384
	private void Update()
	{
		if (Time.time >= (float)this.uiCheckInterval)
		{
			this.uiCheckInterval = Mathf.FloorToInt(Time.time) + 2;
			this.currencyUI.text = "$" + this.money.ToString();
			this.foodBarL.x = this.hunger / 100f;
			this.waterBarL = this.waterBar.rectTransform.localScale;
			this.waterBarL.x = this.water / 100f;
			this.sleepBarL = this.sleepBar.rectTransform.localScale;
			this.sleepBarL.x = this.sleep / 100f;
			this.sanityBarL = this.sanityBar.rectTransform.localScale;
			this.sanityBarL.x = this.stress / 100f;
			if (this.fpc.m_RunSpeed == 3f && this.hunger > 2f && this.sleep > 2f && this.water > 2f && this.invokeScale < 10f)
			{
				this.MoveRegular();
			}
			this.foodBar.rectTransform.localScale = this.foodBarL;
			this.waterBar.rectTransform.localScale = this.waterBarL;
			this.sleepBar.rectTransform.localScale = this.sleepBarL;
			this.sanityBar.rectTransform.localScale = this.sanityBarL;
			if (this.fpc.m_IsWalking)
			{
				this.runScale = 1f;
				return;
			}
			this.runScale = 4f;
		}
	}

	// Token: 0x040011CA RID: 4554
	public FirstPersonController fpc;

	// Token: 0x040011CB RID: 4555
	public float money;

	// Token: 0x040011CC RID: 4556
	public float hunger;

	// Token: 0x040011CD RID: 4557
	public float sleep;

	// Token: 0x040011CE RID: 4558
	public float stress;

	// Token: 0x040011CF RID: 4559
	public float water;

	// Token: 0x040011D0 RID: 4560
	public float nicotine;

	// Token: 0x040011D1 RID: 4561
	public int temperature;

	// Token: 0x040011D2 RID: 4562
	public int fuel;

	// Token: 0x040011D3 RID: 4563
	public float cashin;

	// Token: 0x040011D4 RID: 4564
	public int[] keys;

	// Token: 0x040011D5 RID: 4565
	public int drunk;

	// Token: 0x040011D6 RID: 4566
	public int strength;

	// Token: 0x040011D7 RID: 4567
	public bool tired;

	// Token: 0x040011D8 RID: 4568
	private int uiCheckInterval;

	// Token: 0x040011D9 RID: 4569
	public Text currencyUI;

	// Token: 0x040011DA RID: 4570
	public Text foodUI;

	// Token: 0x040011DB RID: 4571
	public Text sleepUI;

	// Token: 0x040011DC RID: 4572
	public Text sanityUI;

	// Token: 0x040011DD RID: 4573
	public Text waterUI;

	// Token: 0x040011DE RID: 4574
	public Image foodBar;

	// Token: 0x040011DF RID: 4575
	public Image waterBar;

	// Token: 0x040011E0 RID: 4576
	public Image sleepBar;

	// Token: 0x040011E1 RID: 4577
	public Image sanityBar;

	// Token: 0x040011E2 RID: 4578
	private Vector3 foodBarL;

	// Token: 0x040011E3 RID: 4579
	private Vector3 sleepBarL;

	// Token: 0x040011E4 RID: 4580
	private Vector3 sanityBarL;

	// Token: 0x040011E5 RID: 4581
	private Vector3 waterBarL;

	// Token: 0x040011E6 RID: 4582
	public Atm atm;

	// Token: 0x040011E7 RID: 4583
	public GameObject frontDoor;

	// Token: 0x040011E8 RID: 4584
	public float invokeScale = 1f;

	// Token: 0x040011E9 RID: 4585
	public float runScale = 1f;

	// Token: 0x040011EA RID: 4586
	public int playMinutes;
}
