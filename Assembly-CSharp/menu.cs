using System;
using UnityEngine;

// Token: 0x02000187 RID: 391
public class menu : MonoBehaviour
{
	// Token: 0x0600098E RID: 2446 RVA: 0x00080488 File Offset: 0x0007E688
	private void Start()
	{
		this.menuRect = new Rect(this.xPosRight, 25f, 100f, 30f);
		this.lightRect = new Rect(this.xPosLeft, 25f, 100f, 30f);
		this.gateRightRect = new Rect(this.xPosLeft, 60f, 100f, 30f);
		this.gateLeftRect = new Rect(this.xPosLeft, 95f, 100f, 30f);
		this.carMenuRect = new Rect(this.xPosLeft, 130f, 100f, 30f);
		this.closeRect = new Rect(this.xPosLeft, 165f, 100f, 30f);
		this.changeRect = new Rect((float)(Screen.width / 2 - 50), (float)(Screen.height + 30), 100f, 30f);
		base.GetComponent<cameraControl>().target = this.carPrefabs[0].transform;
		Object.Destroy(GameObject.FindGameObjectWithTag("Player"));
		this.car = Object.Instantiate<GameObject>(this.carPrefabs[this.currentCar], this.carPrefabs[this.currentCar].transform.position, this.carPrefabs[this.currentCar].transform.rotation);
	}

	// Token: 0x0600098F RID: 2447 RVA: 0x000805EC File Offset: 0x0007E7EC
	private void Update()
	{
		this.timer -= Time.deltaTime;
		this.changeRect.x = (float)(Screen.width / 2 - 50);
		if (this.menuEnable)
		{
			if (this.menuRect.x > this.xPosLeft)
			{
				this.menuRect.x = this.menuRect.x - Time.deltaTime * this.scrollSpeed;
			}
			if (this.lightRect.x < this.xPosRight)
			{
				this.lightRect.x = this.lightRect.x + Time.deltaTime * this.scrollSpeed;
				this.closeRect.x = this.lightRect.x;
				this.gateRightRect.x = this.lightRect.x;
				this.gateLeftRect.x = this.lightRect.x;
				this.carMenuRect.x = this.lightRect.x;
			}
			if (this.changeRect.y > (float)(Screen.height - 30))
			{
				this.changeRect.y = this.changeRect.y - Time.deltaTime * this.scrollSpeed;
			}
			else if (this.changeRect.y < (float)(Screen.height - 30))
			{
				this.changeRect.y = (float)(Screen.height - 30);
			}
		}
		else
		{
			if (this.menuRect.x < this.xPosRight)
			{
				this.menuRect.x = this.menuRect.x + Time.deltaTime * this.scrollSpeed;
			}
			if (this.lightRect.x > this.xPosLeft)
			{
				this.lightRect.x = this.lightRect.x - Time.deltaTime * this.scrollSpeed;
				this.closeRect.x = this.lightRect.x;
				this.gateRightRect.x = this.lightRect.x;
				this.gateLeftRect.x = this.lightRect.x;
				this.carMenuRect.x = this.lightRect.x;
			}
			if (this.changeRect.y < (float)(Screen.height + 30))
			{
				this.changeRect.y = this.changeRect.y + Time.deltaTime * this.scrollSpeed;
			}
		}
		if (!this.closeWindow)
		{
			this.windowRect = new Rect((float)(Screen.width / 2 - 150), (float)(Screen.height / 2 - 100), 300f, 200f);
		}
	}

	// Token: 0x06000990 RID: 2448 RVA: 0x00080878 File Offset: 0x0007EA78
	private void OnGUI()
	{
		if (GUI.Button(this.menuRect, "menu"))
		{
			this.menuEnable = true;
		}
		if (GUI.Button(this.closeRect, "close menu"))
		{
			this.menuEnable = false;
		}
		if (GUI.Button(this.carMenuRect, "car menu"))
		{
			this.carMenu = !this.carMenu;
		}
		if (GUI.Button(this.changeRect, "change car"))
		{
			this.direction = 1;
		}
		if (!this.closeWindow)
		{
			this.windowRect = GUI.Window(0, this.windowRect, new GUI.WindowFunction(this.WindowFunction), "Help");
		}
		this.lightEnable = GUI.Toggle(this.lightRect, this.lightEnable, "enable light");
		this.rightOpen = GUI.Toggle(this.gateRightRect, this.rightOpen, "open right gate");
		this.leftOpen = GUI.Toggle(this.gateLeftRect, this.leftOpen, "open left gate");
		if (GUI.changed)
		{
			this.getInput = true;
		}
		else
		{
			this.getInput = false;
		}
		this.FadeScreen(this.direction);
	}

	// Token: 0x06000991 RID: 2449 RVA: 0x00080994 File Offset: 0x0007EB94
	private void ChangeCar()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Respawn");
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i]);
		}
		Object.Destroy(this.car);
		this.currentCar++;
		if (this.currentCar > this.carPrefabs.Length - 1)
		{
			this.currentCar = 0;
		}
		this.car = Object.Instantiate<GameObject>(this.carPrefabs[this.currentCar], this.carPrefabs[this.currentCar].transform.position, this.carPrefabs[this.currentCar].transform.rotation);
	}

	// Token: 0x06000992 RID: 2450 RVA: 0x00080A3C File Offset: 0x0007EC3C
	private void FadeScreen(int dir)
	{
		this.alpha += Time.deltaTime * this.fadeSpeed * (float)dir;
		this.alpha = Mathf.Clamp01(this.alpha);
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.alpha);
		GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), this.fadeOutTexture);
		if (this.alpha >= 0.99f)
		{
			this.ChangeCar();
			this.direction = -1;
		}
	}

	// Token: 0x06000993 RID: 2451 RVA: 0x00080AE4 File Offset: 0x0007ECE4
	private void WindowFunction(int WindowID)
	{
		if (GUI.Button(new Rect(this.windowRect.width - 48f, this.windowRect.height - 48f, (float)(this.closeTexture.width * 2), (float)(this.closeTexture.height * 2)), this.closeTexture))
		{
			this.closeWindow = true;
		}
		GUI.Label(new Rect(48f, 48f, 204f, 104f), "LEFT MOUSE BUTTON + MOUSE = CAMERA ROTATION\n \n RIGHT MOUSE BUTTON + MOUSE = ZOOM");
	}

	// Token: 0x0400198B RID: 6539
	public GameObject[] carPrefabs;

	// Token: 0x0400198C RID: 6540
	private GameObject car;

	// Token: 0x0400198D RID: 6541
	public float scrollSpeed = 10f;

	// Token: 0x0400198E RID: 6542
	public float xPosLeft = -30f;

	// Token: 0x0400198F RID: 6543
	public float xPosRight = 20f;

	// Token: 0x04001990 RID: 6544
	public bool lightEnable;

	// Token: 0x04001991 RID: 6545
	public bool getInput;

	// Token: 0x04001992 RID: 6546
	public bool rightOpen;

	// Token: 0x04001993 RID: 6547
	public bool leftOpen;

	// Token: 0x04001994 RID: 6548
	public bool carMenu;

	// Token: 0x04001995 RID: 6549
	private bool menuEnable;

	// Token: 0x04001996 RID: 6550
	private Rect menuRect;

	// Token: 0x04001997 RID: 6551
	private Rect lightRect;

	// Token: 0x04001998 RID: 6552
	private Rect gateRightRect;

	// Token: 0x04001999 RID: 6553
	private Rect gateLeftRect;

	// Token: 0x0400199A RID: 6554
	private Rect closeRect;

	// Token: 0x0400199B RID: 6555
	private Rect carMenuRect;

	// Token: 0x0400199C RID: 6556
	private Rect changeRect;

	// Token: 0x0400199D RID: 6557
	private Rect windowRect;

	// Token: 0x0400199E RID: 6558
	private int currentCar;

	// Token: 0x0400199F RID: 6559
	public Texture2D fadeOutTexture;

	// Token: 0x040019A0 RID: 6560
	public float fadeSpeed = 2f;

	// Token: 0x040019A1 RID: 6561
	public Texture2D closeTexture;

	// Token: 0x040019A2 RID: 6562
	private bool closeWindow;

	// Token: 0x040019A3 RID: 6563
	private float alpha;

	// Token: 0x040019A4 RID: 6564
	private int direction = -1;

	// Token: 0x040019A5 RID: 6565
	private float timer = 10f;
}
