using System;
using UnityEngine;

// Token: 0x02000183 RID: 387
public class dashBoard : MonoBehaviour
{
	// Token: 0x0600096E RID: 2414 RVA: 0x0007F7AC File Offset: 0x0007D9AC
	private void Start()
	{
		this.speedPointerDefRot = this.speedPointer.localEulerAngles;
		this.rpmPointerDefRot = this.rpmPointer.localEulerAngles;
		this.fuelPointerDefRot = this.fuelPointer.localEulerAngles;
		this.temperaturePointerDefRot = this.temperaturePointer.localEulerAngles;
		this.anim = base.transform.root.GetComponent<Animator>();
	}

	// Token: 0x0600096F RID: 2415 RVA: 0x0007F814 File Offset: 0x0007DA14
	private void Update()
	{
		this.oilEmissive = this.SetEmissive(this.oilIcon, this.oilEmissive, this.enableOil);
		this.beamEmissive = this.SetEmissive(this.beamIcon, this.beamEmissive, this.enableBeam);
		this.chgEmissive = this.SetEmissive(this.chgIcon, this.chgEmissive, this.enableChg);
		this.brakeEmissive = this.SetEmissive(this.brakeIcon, this.brakeEmissive, this.enableBrake);
		this.fastenBeltsEmissive = this.SetEmissive(this.fastenBeltsIcon, this.fastenBeltsEmissive, this.enableFastenBelts);
		this.wd4Emissive = this.SetEmissive(this.wd4Icon, this.wd4Emissive, this.enableWd4);
		this.fuelPointerAngle = Mathf.Lerp(this.fuelPointerDefRot.z, this.maxFuelPointerAngle, this.fuelLevel / this.maxFuelValue);
		this.fuelPointer.localEulerAngles = new Vector3(this.fuelPointerDefRot.x, this.fuelPointerDefRot.y, this.fuelPointerAngle);
		this.temperaturePointerAngle = Mathf.Lerp(this.temperaturePointerDefRot.z, this.maxTemperaturePointerAngle, this.engineTemperature / this.maxTemperatureValue);
		this.temperaturePointer.localEulerAngles = new Vector3(this.temperaturePointerDefRot.x, this.temperaturePointerDefRot.y, this.temperaturePointerAngle);
		Light[] array = this.dashBoardLights;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = this.dashBoardLightEnable;
		}
		this.anim.SetBool("gloveCompartmentOpen", this.gloveCompartmentOpen);
		this.anim.SetInteger("gear", (int)this.currentGear);
	}

	// Token: 0x06000970 RID: 2416 RVA: 0x0007F9D0 File Offset: 0x0007DBD0
	private float SetEmissive(GameObject emissiveObject, float emissive, bool enable)
	{
		if (enable)
		{
			emissive = Mathf.Lerp(emissive, 1f, Time.deltaTime * 20f);
		}
		else
		{
			emissive = Mathf.Lerp(emissive, 0f, Time.deltaTime * 20f);
		}
		emissive = Mathf.Clamp01(emissive);
		emissiveObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white * Mathf.LinearToGammaSpace(emissive));
		return emissive;
	}

	// Token: 0x06000971 RID: 2417 RVA: 0x0007FA40 File Offset: 0x0007DC40
	private void SetEmissive(GameObject emissiveObject, float emissive)
	{
		emissiveObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white * Mathf.LinearToGammaSpace(emissive));
	}

	// Token: 0x06000972 RID: 2418 RVA: 0x0007FA67 File Offset: 0x0007DC67
	public void SetSpeed(float speed)
	{
		this.currentSpeed = speed;
	}

	// Token: 0x06000973 RID: 2419 RVA: 0x0007FA70 File Offset: 0x0007DC70
	public void SetRPM(float rpm)
	{
		this.currentRpm = rpm;
	}

	// Token: 0x06000974 RID: 2420 RVA: 0x0007FA79 File Offset: 0x0007DC79
	public void SetFuelLevel(float level)
	{
		this.fuelLevel = level;
	}

	// Token: 0x06000975 RID: 2421 RVA: 0x0007FA82 File Offset: 0x0007DC82
	public void SetTemperature(float temperature)
	{
		this.engineTemperature = temperature;
	}

	// Token: 0x06000976 RID: 2422 RVA: 0x0007FA8B File Offset: 0x0007DC8B
	public void EnableDashboardLights(bool enable)
	{
		this.dashBoardLightEnable = enable;
	}

	// Token: 0x06000977 RID: 2423 RVA: 0x0007FA94 File Offset: 0x0007DC94
	public void OpenGloveCompartment(bool open)
	{
		this.gloveCompartmentOpen = open;
	}

	// Token: 0x06000978 RID: 2424 RVA: 0x0007FA9D File Offset: 0x0007DC9D
	public void SetGear(int gear)
	{
		this.currentGear = (dashBoard.gears)gear;
	}

	// Token: 0x06000979 RID: 2425 RVA: 0x0007FAA6 File Offset: 0x0007DCA6
	public void SetOil(bool enable)
	{
		this.enableOil = enable;
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x0007FAAF File Offset: 0x0007DCAF
	public void SetBeam(bool enable)
	{
		this.enableBeam = enable;
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x0007FAB8 File Offset: 0x0007DCB8
	public void SetCharge(bool enable)
	{
		this.enableChg = enable;
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x0007FAC1 File Offset: 0x0007DCC1
	public void SetFastenBelts(bool enable)
	{
		this.enableFastenBelts = enable;
	}

	// Token: 0x0600097D RID: 2429 RVA: 0x0007FACA File Offset: 0x0007DCCA
	public void SetBrake(bool enable)
	{
		this.enableBrake = enable;
	}

	// Token: 0x0600097E RID: 2430 RVA: 0x0007FAD3 File Offset: 0x0007DCD3
	public void Set4WD(bool enable)
	{
		this.enableWd4 = enable;
	}

	// Token: 0x0400192D RID: 6445
	public bool gloveCompartmentOpen;

	// Token: 0x0400192E RID: 6446
	public GameObject oilIcon;

	// Token: 0x0400192F RID: 6447
	public bool enableOil;

	// Token: 0x04001930 RID: 6448
	private float oilEmissive;

	// Token: 0x04001931 RID: 6449
	public GameObject beamIcon;

	// Token: 0x04001932 RID: 6450
	public bool enableBeam;

	// Token: 0x04001933 RID: 6451
	private float beamEmissive;

	// Token: 0x04001934 RID: 6452
	public GameObject brakeIcon;

	// Token: 0x04001935 RID: 6453
	public bool enableBrake;

	// Token: 0x04001936 RID: 6454
	private float brakeEmissive;

	// Token: 0x04001937 RID: 6455
	public GameObject chgIcon;

	// Token: 0x04001938 RID: 6456
	public bool enableChg;

	// Token: 0x04001939 RID: 6457
	private float chgEmissive;

	// Token: 0x0400193A RID: 6458
	public GameObject fastenBeltsIcon;

	// Token: 0x0400193B RID: 6459
	public bool enableFastenBelts;

	// Token: 0x0400193C RID: 6460
	private float fastenBeltsEmissive;

	// Token: 0x0400193D RID: 6461
	public GameObject wd4Icon;

	// Token: 0x0400193E RID: 6462
	public bool enableWd4;

	// Token: 0x0400193F RID: 6463
	private float wd4Emissive;

	// Token: 0x04001940 RID: 6464
	public GameObject leftArrowIcon;

	// Token: 0x04001941 RID: 6465
	public bool enableLeftArrow;

	// Token: 0x04001942 RID: 6466
	private float leftArrowEmissive;

	// Token: 0x04001943 RID: 6467
	public GameObject rightArrowIcon;

	// Token: 0x04001944 RID: 6468
	public bool enableRightArrow;

	// Token: 0x04001945 RID: 6469
	private float rightArrowEmissive;

	// Token: 0x04001946 RID: 6470
	public bool dashBoardLightEnable;

	// Token: 0x04001947 RID: 6471
	public Light[] dashBoardLights;

	// Token: 0x04001948 RID: 6472
	public dashBoard.gears currentGear;

	// Token: 0x04001949 RID: 6473
	public float currentSpeed;

	// Token: 0x0400194A RID: 6474
	public float currentRpm;

	// Token: 0x0400194B RID: 6475
	public float fuelLevel;

	// Token: 0x0400194C RID: 6476
	public float engineTemperature;

	// Token: 0x0400194D RID: 6477
	public Transform speedPointer;

	// Token: 0x0400194E RID: 6478
	public float maxSpeedValue;

	// Token: 0x0400194F RID: 6479
	public float maxSpeedPointerAngle;

	// Token: 0x04001950 RID: 6480
	private float speedPointerAngle;

	// Token: 0x04001951 RID: 6481
	private Vector3 speedPointerDefRot;

	// Token: 0x04001952 RID: 6482
	public Transform rpmPointer;

	// Token: 0x04001953 RID: 6483
	public float maxRpmValue;

	// Token: 0x04001954 RID: 6484
	public float maxRpmPointerAngle;

	// Token: 0x04001955 RID: 6485
	private float rpmPointerAngle;

	// Token: 0x04001956 RID: 6486
	private Vector3 rpmPointerDefRot;

	// Token: 0x04001957 RID: 6487
	public Transform fuelPointer;

	// Token: 0x04001958 RID: 6488
	public float maxFuelValue;

	// Token: 0x04001959 RID: 6489
	public float maxFuelPointerAngle;

	// Token: 0x0400195A RID: 6490
	private float fuelPointerAngle;

	// Token: 0x0400195B RID: 6491
	private Vector3 fuelPointerDefRot;

	// Token: 0x0400195C RID: 6492
	public Transform temperaturePointer;

	// Token: 0x0400195D RID: 6493
	public float maxTemperatureValue;

	// Token: 0x0400195E RID: 6494
	public float maxTemperaturePointerAngle;

	// Token: 0x0400195F RID: 6495
	private float temperaturePointerAngle;

	// Token: 0x04001960 RID: 6496
	private Vector3 temperaturePointerDefRot;

	// Token: 0x04001961 RID: 6497
	private Animator anim;

	// Token: 0x02000439 RID: 1081
	public enum gears
	{
		// Token: 0x04002997 RID: 10647
		neutral,
		// Token: 0x04002998 RID: 10648
		first,
		// Token: 0x04002999 RID: 10649
		second,
		// Token: 0x0400299A RID: 10650
		third,
		// Token: 0x0400299B RID: 10651
		fourth,
		// Token: 0x0400299C RID: 10652
		fifth,
		// Token: 0x0400299D RID: 10653
		rear
	}
}
