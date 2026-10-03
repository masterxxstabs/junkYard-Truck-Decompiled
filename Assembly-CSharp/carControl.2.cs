using System;
using UnityEngine;

// Token: 0x02000181 RID: 385
public class carControl : MonoBehaviour
{
	// Token: 0x06000966 RID: 2406 RVA: 0x0007E6E8 File Offset: 0x0007C8E8
	private void Start()
	{
		this.garageMenu = (Object.FindObjectOfType(typeof(menu)) as menu);
		this.rightX = (float)Screen.width + this.paddingForScreen;
		this.leftX = (float)Screen.width - this.paddingForScreen;
		this.lightsRect = new Rect(this.rightX, 30f, 90f, 30f);
		this.sirenAudio = base.gameObject.AddComponent<AudioSource>();
		this.sirenAudio.loop = true;
		this.sirenAudio.clip = this.sirenSound;
		this.sirenAudio.volume = 1f;
		this.sirenAudio.spatialBlend = 1f;
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x0007E7A4 File Offset: 0x0007C9A4
	private void Update()
	{
		this.rightX = (float)Screen.width + this.paddingForScreen;
		this.leftX = (float)Screen.width - this.paddingForScreen;
		if (this.garageMenu != null)
		{
			if (this.garageMenu.carMenu)
			{
				if (this.lightsRect.x > this.leftX)
				{
					this.lightsRect.x = this.lightsRect.x - Time.deltaTime * this.garageMenu.scrollSpeed * 2f;
				}
				if (this.lightsRect.x < this.leftX)
				{
					this.lightsRect.x = this.leftX;
				}
			}
			else
			{
				if (this.lightsRect.x < this.rightX)
				{
					this.lightsRect.x = this.lightsRect.x + Time.deltaTime * this.garageMenu.scrollSpeed * 2f;
				}
				if (this.lightsRect.x > this.rightX)
				{
					this.lightsRect.x = this.rightX;
				}
			}
		}
		if (this.headlights)
		{
			this.headLightsIntensity += Time.deltaTime * 10f;
		}
		else
		{
			this.headLightsIntensity -= Time.deltaTime * 10f;
		}
		this.headLightsIntensity = Mathf.Clamp01(this.headLightsIntensity);
		if (this.backlights)
		{
			this.backlightsIntensity += Time.deltaTime * 10f;
		}
		else
		{
			this.backlightsIntensity -= Time.deltaTime * 10f;
		}
		this.backlightsIntensity = Mathf.Clamp01(this.backlightsIntensity);
		if (this.flashLightGlassLeft != null && this.flashLightGlassRight != null)
		{
			if (this.policeLights && (!this.flashLightGlassLeft.destroyed || !this.flashLightGlassRight.destroyed))
			{
				if (!this.sirenAudio.isPlaying)
				{
					this.sirenAudio.Play();
				}
				for (int i = 0; i < this.leftFlashGroup.Length; i++)
				{
					this.leftFlashGroup[i].lightIntensity += Time.deltaTime * 10f;
					this.leftFlashGroup[i].lightIntensity = Mathf.Clamp01(this.leftFlashGroup[i].lightIntensity);
					this.leftFlashGroup[i].transform.localRotation *= Quaternion.Euler(Vector3.up * (Time.deltaTime * 300f));
					if (this.flashLightGlassLeft.destroyed)
					{
						this.leftFlashGroup[i].SetColorOfFlare(Color.grey);
					}
					if (!this.flashLightGlassLeft.destroyed)
					{
						this.leftFlashGroup[i].SetColorOfFlare(false);
					}
				}
				for (int j = 0; j < this.rightFlashGroup.Length; j++)
				{
					this.rightFlashGroup[j].lightIntensity += Time.deltaTime * 10f;
					this.rightFlashGroup[j].lightIntensity = Mathf.Clamp01(this.rightFlashGroup[j].lightIntensity);
					this.rightFlashGroup[j].transform.localRotation *= Quaternion.Euler(Vector3.up * (Time.deltaTime * 300f));
					if (this.flashLightGlassRight.destroyed)
					{
						this.rightFlashGroup[j].SetColorOfFlare(Color.grey);
					}
					if (!this.flashLightGlassRight.destroyed)
					{
						this.rightFlashGroup[j].SetColorOfFlare(false);
					}
				}
			}
			else
			{
				if (this.sirenAudio.isPlaying)
				{
					this.sirenAudio.Stop();
				}
				for (int k = 0; k < this.leftFlashGroup.Length; k++)
				{
					this.leftFlashGroup[k].lightIntensity -= Time.deltaTime * 10f;
					this.leftFlashGroup[k].lightIntensity = Mathf.Clamp01(this.leftFlashGroup[k].lightIntensity);
				}
				for (int l = 0; l < this.rightFlashGroup.Length; l++)
				{
					this.rightFlashGroup[l].lightIntensity -= Time.deltaTime * 10f;
					this.rightFlashGroup[l].lightIntensity = Mathf.Clamp01(this.rightFlashGroup[l].lightIntensity);
				}
			}
		}
		switch (this.turnSelect)
		{
		case 0:
			this.emergency = false;
			this.enableTurnLeft = false;
			this.enableTurnRight = false;
			this.turnLeft = false;
			this.turnRight = false;
			this.timer = 0f;
			break;
		case 1:
			this.emergency = true;
			this.enableTurnLeft = false;
			this.enableTurnRight = false;
			break;
		case 2:
			this.emergency = false;
			this.enableTurnLeft = true;
			this.enableTurnRight = false;
			break;
		case 3:
			this.emergency = false;
			this.enableTurnLeft = false;
			this.enableTurnRight = true;
			break;
		}
		if (this.emergency)
		{
			this.timer += Time.deltaTime;
			if (this.timer < 0.4f)
			{
				this.turnLeft = false;
				this.turnRight = false;
			}
			if (this.timer > 0.4f)
			{
				this.turnLeft = true;
				this.turnRight = true;
			}
			if (this.timer > 0.8f)
			{
				this.timer = 0f;
			}
		}
		if (this.enableTurnLeft)
		{
			this.timer += Time.deltaTime;
			this.turnRight = false;
			if (this.timer < 0.4f)
			{
				this.turnLeft = false;
			}
			if (this.timer > 0.4f)
			{
				this.turnLeft = true;
			}
			if (this.timer > 0.8f)
			{
				this.timer = 0f;
			}
		}
		if (this.enableTurnRight)
		{
			this.timer += Time.deltaTime;
			this.turnLeft = false;
			if (this.timer < 0.4f)
			{
				this.turnRight = false;
			}
			if (this.timer > 0.4f)
			{
				this.turnRight = true;
			}
			if (this.timer > 0.8f)
			{
				this.timer = 0f;
			}
		}
		if (this.turnLeft)
		{
			this.leftTurnIntensity += Time.deltaTime * 10f;
		}
		else
		{
			this.leftTurnIntensity -= Time.deltaTime * 10f;
		}
		this.leftTurnIntensity = Mathf.Clamp01(this.leftTurnIntensity);
		if (this.turnRight)
		{
			this.rightTurnIntensity += Time.deltaTime * 10f;
		}
		else
		{
			this.rightTurnIntensity -= Time.deltaTime * 10f;
		}
		this.rightTurnIntensity = Mathf.Clamp01(this.rightTurnIntensity);
		if (this.taxi)
		{
			this.taxiLightIntensity += Time.deltaTime * 10f;
		}
		else
		{
			this.taxiLightIntensity -= Time.deltaTime * 10f;
		}
		this.taxiLightIntensity = Mathf.Clamp01(this.taxiLightIntensity);
		this.rightHeadlight.lightIntensity = this.headLightsIntensity;
		this.leftHeadlight.lightIntensity = this.headLightsIntensity;
		this.backlightLeft.lightIntensity = this.backlightsIntensity;
		this.backlightRight.lightIntensity = this.backlightsIntensity;
		this.turnSignalBackLeft.lightIntensity = this.leftTurnIntensity;
		this.turnSignalFrontLeft.lightIntensity = this.leftTurnIntensity;
		this.turnSignalBackRight.lightIntensity = this.rightTurnIntensity;
		this.turnSignalFrontRight.lightIntensity = this.rightTurnIntensity;
		if (this.taxiLamp != null)
		{
			this.taxiLamp.lightIntensity = this.taxiLightIntensity;
		}
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x0007EF44 File Offset: 0x0007D144
	private void OnGUI()
	{
		this.headlights = GUI.Toggle(this.lightsRect, this.headlights, "headlights");
		this.leftHeadlight.destroy = GUI.Toggle(new Rect(this.lightsRect.x - 110f, this.lightsRect.y + 30f, this.lightsRect.width + 50f, this.lightsRect.height), this.leftHeadlight.destroy, "broken left headlight");
		this.rightHeadlight.destroy = GUI.Toggle(new Rect(this.lightsRect.x + 30f, this.lightsRect.y + 30f, this.lightsRect.width + 50f, this.lightsRect.height), this.rightHeadlight.destroy, "broken right headlight");
		this.backlights = GUI.Toggle(new Rect(this.lightsRect.x, this.lightsRect.y + 60f, this.lightsRect.width, this.lightsRect.height), this.backlights, "backlights");
		this.backlightLeft.destroy = GUI.Toggle(new Rect(this.lightsRect.x - 110f, this.lightsRect.y + 90f, this.lightsRect.width + 50f, this.lightsRect.height), this.backlightLeft.destroy, "broken left backlight");
		this.backlightRight.destroy = GUI.Toggle(new Rect(this.lightsRect.x + 30f, this.lightsRect.y + 90f, this.lightsRect.width + 50f, this.lightsRect.height), this.backlightRight.destroy, "broken right backlight");
		this.turnSelect = GUI.SelectionGrid(new Rect(this.lightsRect.x - 50f, this.lightsRect.y + 120f, this.lightsRect.width * 2f, this.lightsRect.height * 2f), this.turnSelect, this.selectionString, 2);
		this.turnSignalFrontLeft.destroy = GUI.Toggle(new Rect(this.lightsRect.x - 50f, this.lightsRect.y + 180f, this.lightsRect.width + 90f, this.lightsRect.height), this.turnSignalFrontLeft.destroy, "broken front left turn signal");
		this.turnSignalFrontRight.destroy = GUI.Toggle(new Rect(this.lightsRect.x - 50f, this.lightsRect.y + 210f, this.lightsRect.width + 90f, this.lightsRect.height), this.turnSignalFrontRight.destroy, "broken front right turn signal");
		this.turnSignalBackLeft.destroy = GUI.Toggle(new Rect(this.lightsRect.x - 50f, this.lightsRect.y + 240f, this.lightsRect.width + 90f, this.lightsRect.height), this.turnSignalBackLeft.destroy, "broken back left turn signal");
		this.turnSignalBackRight.destroy = GUI.Toggle(new Rect(this.lightsRect.x - 50f, this.lightsRect.y + 270f, this.lightsRect.width + 90f, this.lightsRect.height), this.turnSignalBackRight.destroy, "broken back right turn signal");
		if (this.flashLightGlassLeft != null && this.flashLightGlassRight != null)
		{
			this.policeLights = GUI.Toggle(new Rect(this.lightsRect.x, this.lightsRect.y + 300f, this.lightsRect.width, this.lightsRect.height), this.policeLights, "police lights");
			this.flashLightGlassLeft.destroy = GUI.Toggle(new Rect(this.lightsRect.x - 110f, this.lightsRect.y + 330f, this.lightsRect.width + 50f, this.lightsRect.height), this.flashLightGlassLeft.destroy, "broken left flashlight");
			this.flashLightGlassRight.destroy = GUI.Toggle(new Rect(this.lightsRect.x + 30f, this.lightsRect.y + 330f, this.lightsRect.width + 50f, this.lightsRect.height), this.flashLightGlassRight.destroy, "broken right flashlight");
		}
		if (this.taxiLamp != null)
		{
			this.taxi = GUI.Toggle(new Rect(this.lightsRect.x, this.lightsRect.y + 300f, this.lightsRect.width, this.lightsRect.height), this.taxi, "taxi lamp");
			this.taxiLamp.destroy = GUI.Toggle(new Rect(this.lightsRect.x, this.lightsRect.y + 330f, this.lightsRect.width + 50f, this.lightsRect.height), this.taxiLamp.destroy, "broken taxi lamp");
		}
		this.dashBoardLight.enabled = GUI.Toggle(new Rect(this.lightsRect.x, this.lightsRect.y + 360f, this.lightsRect.width, this.lightsRect.height), this.dashBoardLight.enabled, "dashboard");
	}

	// Token: 0x040018FF RID: 6399
	public lights rightHeadlight;

	// Token: 0x04001900 RID: 6400
	public lights leftHeadlight;

	// Token: 0x04001901 RID: 6401
	public lights backlightLeft;

	// Token: 0x04001902 RID: 6402
	public lights backlightRight;

	// Token: 0x04001903 RID: 6403
	public lights turnSignalFrontRight;

	// Token: 0x04001904 RID: 6404
	public lights turnSignalFrontLeft;

	// Token: 0x04001905 RID: 6405
	public lights turnSignalBackRight;

	// Token: 0x04001906 RID: 6406
	public lights turnSignalBackLeft;

	// Token: 0x04001907 RID: 6407
	public lights[] leftFlashGroup;

	// Token: 0x04001908 RID: 6408
	public destroyrepair flashLightGlassLeft;

	// Token: 0x04001909 RID: 6409
	public lights[] rightFlashGroup;

	// Token: 0x0400190A RID: 6410
	public destroyrepair flashLightGlassRight;

	// Token: 0x0400190B RID: 6411
	public lights taxiLamp;

	// Token: 0x0400190C RID: 6412
	public Light dashBoardLight;

	// Token: 0x0400190D RID: 6413
	public AudioClip sirenSound;

	// Token: 0x0400190E RID: 6414
	private AudioSource sirenAudio;

	// Token: 0x0400190F RID: 6415
	private menu garageMenu;

	// Token: 0x04001910 RID: 6416
	private float paddingForScreen = 180f;

	// Token: 0x04001911 RID: 6417
	private float rightX;

	// Token: 0x04001912 RID: 6418
	private float leftX;

	// Token: 0x04001913 RID: 6419
	private float headLightsIntensity;

	// Token: 0x04001914 RID: 6420
	private float backlightsIntensity;

	// Token: 0x04001915 RID: 6421
	private float leftTurnIntensity;

	// Token: 0x04001916 RID: 6422
	private float rightTurnIntensity;

	// Token: 0x04001917 RID: 6423
	private float taxiLightIntensity;

	// Token: 0x04001918 RID: 6424
	private float timer;

	// Token: 0x04001919 RID: 6425
	private int turnSelect;

	// Token: 0x0400191A RID: 6426
	private string[] selectionString = new string[]
	{
		"disable turns",
		"emergensy",
		"left turn",
		"right turn"
	};

	// Token: 0x0400191B RID: 6427
	private Rect lightsRect;

	// Token: 0x0400191C RID: 6428
	private bool headlights;

	// Token: 0x0400191D RID: 6429
	private bool backlights;

	// Token: 0x0400191E RID: 6430
	private bool emergency;

	// Token: 0x0400191F RID: 6431
	private bool turnLeft;

	// Token: 0x04001920 RID: 6432
	private bool enableTurnLeft;

	// Token: 0x04001921 RID: 6433
	private bool turnRight;

	// Token: 0x04001922 RID: 6434
	private bool enableTurnRight;

	// Token: 0x04001923 RID: 6435
	private bool policeLights;

	// Token: 0x04001924 RID: 6436
	private bool taxi;
}
