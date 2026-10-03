using System;
using System.Collections;
using Michsky.UI.Dark;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.ImageEffects;

// Token: 0x020000D0 RID: 208
public class Interactor : MonoBehaviour
{
	// Token: 0x060004D3 RID: 1235 RVA: 0x00031CB8 File Offset: 0x0002FEB8
	private void Start()
	{
		this.leaned = false;
		this.pickedUpObject = null;
		this.cam = Camera.main;
		this.reticleController = Object.FindObjectOfType<ReticleController>();
		this.creeper.GetComponent<CreeperMovement>().controlled = false;
		this.layer_mask = (LayerMask.GetMask(new string[]
		{
			"Default"
		}) | LayerMask.GetMask(new string[]
		{
			"creeper"
		}) | LayerMask.GetMask(new string[]
		{
			"Engine1"
		}) | LayerMask.GetMask(new string[]
		{
			"RayCast"
		}));
		this.escapePanel.SetActive(false);
		this.prt = this.playerArrow.GetComponent<RectTransform>();
		this.fpc.LockMouse();
		Cursor.visible = false;
		this.inv.Start();
		if (this.eventSystem.GetComponent<MissionController>().completedMissions.Contains(2) && !this.mail.receivedEmails.Contains(2))
		{
			this.mail.receivedEmails.Add(2);
			this.mail.receivedEmails.Add(3);
			this.mail.receivedEmails.Add(4);
			this.mail.receivedEmails.Add(8);
			this.mail.receivedEmails.Add(11);
		}
		this.autoSaveTime = Time.time;
		if (this.timeOfDay != 0f)
		{
			EnviroSkyMgr.instance.SetTimeOfDay(this.timeOfDay);
			base.StartCoroutine(this.SavedWeather());
		}
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x00031E3E File Offset: 0x0003003E
	private IEnumerator SavedWeather()
	{
		yield return new WaitForSeconds(1f);
		EnviroSkyMgr.instance.ChangeWeatherInstant(this.weatherId);
		yield break;
	}

	// Token: 0x060004D5 RID: 1237 RVA: 0x00031E4D File Offset: 0x0003004D
	public void OpenFSM()
	{
		this.FSM.SetActive(true);
		this.fpc.UnlockMouse();
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		this.phonePanel.SetActive(false);
		base.GetComponent<FirstPersonController>().enabled = false;
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x00031E8A File Offset: 0x0003008A
	private IEnumerator StartCarDelay()
	{
		yield return new WaitForSeconds(1f);
		this.StartCar();
		yield break;
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x00031E99 File Offset: 0x00030099
	private IEnumerator StartUCarDelay()
	{
		yield return new WaitForSeconds(1f);
		this.StartUtilityCar();
		yield break;
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x00031EA8 File Offset: 0x000300A8
	private IEnumerator StartFCarDelay()
	{
		yield return new WaitForSeconds(1f);
		this.iaSources[2].Play();
		this.StartF100();
		yield break;
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x00031EB8 File Offset: 0x000300B8
	public void KeyStateChangeCar()
	{
		if (this.keyStateC == 3)
		{
			this.ucar.GetComponent<car3>().turnOffAcc();
			this.iaSources[1].Play();
			this.keyStateC = 0;
			this.StartUtilityCar();
			return;
		}
		if (this.keyStateC == 2)
		{
			this.keyStateC = 3;
			this.ucar.GetComponent<car3>().turnOffAcc();
			base.StartCoroutine(this.StartUCarDelay());
			return;
		}
		if (this.keyStateC == 0)
		{
			this.iaSources = this.ignition_keyC.GetComponents<AudioSource>();
			this.iaSources[0].Play();
			this.iaSources[2].Play();
			this.ucar.GetComponent<car3>().turnOnAcc();
			this.nextUpdate = Mathf.FloorToInt(Time.time) + 1;
			this.keyStateC = 1;
		}
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x00031F84 File Offset: 0x00030184
	public void KeyStateChange()
	{
		if (this.keyState == 3)
		{
			this.carscript.turnOffAcc();
			this.carscript.DarkenDash();
			this.iaSources[1].Play();
			this.keyState = 0;
			if (this.buddy.enabled)
			{
				this.buddy.TurnOff();
			}
			this.StartCar();
			return;
		}
		if (this.keyState == 2)
		{
			this.keyState = 3;
			this.carscript.turnOffAcc();
			base.StartCoroutine(this.StartCarDelay());
			this.carscript.IlluminateDash();
			this.carscript.CheckSusp();
			if (this.buddy.enabled)
			{
				this.buddy.TurnOn();
				return;
			}
		}
		else if (this.keyState == 0)
		{
			this.carscript.updateEngine();
			this.iaSources = this.ignition_keyO.GetComponents<AudioSource>();
			this.iaSources[0].Play();
			this.iaSources[2].Play();
			this.carscript.turnOnAcc();
			this.nextUpdate = Mathf.FloorToInt(Time.time) + 1;
			this.keyState = 1;
		}
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x0003209C File Offset: 0x0003029C
	public void KeyStateChangeF()
	{
		if (this.keyStateF == 3)
		{
			this.fcarscript.turnOffAcc();
			this.fcarscript.DarkenDash();
			this.iaSources[1].Play();
			this.keyStateF = 0;
			this.StartF100();
			return;
		}
		if (this.keyStateF == 2)
		{
			this.keyStateF = 3;
			this.fcarscript.turnOffAcc();
			base.StartCoroutine(this.StartFCarDelay());
			this.fcarscript.IlluminateDash();
			this.fcarscript.CheckSusp();
			return;
		}
		if (this.keyStateF == 0)
		{
			this.fcarscript.updateEngine();
			this.iaSources = this.ignition_keyF.GetComponents<AudioSource>();
			this.iaSources[0].Play();
			this.fcarscript.turnOnAcc();
			this.nextUpdate = Mathf.FloorToInt(Time.time) + 1;
			this.keyStateF = 1;
		}
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x00032178 File Offset: 0x00030378
	public void StartCar()
	{
		if (!this.carscript.usingV8 && !this.carscript.usingI6)
		{
			this.canRun = this.fourcyl.GetComponent<engine>().canRun;
			this.canCrank = this.fourcyl.GetComponent<engine>().canCrank;
		}
		else if (this.carscript.usingI6)
		{
			this.canRun = this.i6.GetComponent<enginei6>().canRun;
			this.canCrank = this.i6.GetComponent<enginei6>().canCrank;
		}
		else
		{
			this.canRun = this.v8.GetComponent<enginev8>().canRun;
			this.canCrank = this.v8.GetComponent<enginev8>().canCrank;
		}
		if (this.carscript.controlled)
		{
			this.carscript.controlled = false;
			this.carscript.userControlled = false;
			return;
		}
		if (this.canRun)
		{
			this.carscript.controlled = true;
			this.carscript.userControlled = true;
			this.carscript.IlluminateDash();
		}
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x00032288 File Offset: 0x00030488
	public void StartF100()
	{
		if (this.fcarscript.usingV8)
		{
			this.canRun = this.v8.GetComponent<enginev8>().canRun;
			this.canCrank = this.v8.GetComponent<enginev8>().canCrank;
		}
		if (this.fcarscript.controlled)
		{
			this.fcarscript.controlled = false;
			this.fcarscript.userControlled = false;
			return;
		}
		if (this.canRun)
		{
			this.fcarscript.controlled = true;
			this.fcarscript.userControlled = true;
			this.fcarscript.IlluminateDash();
		}
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x00032320 File Offset: 0x00030520
	public void StartUtilityCar()
	{
		if (this.ucar.GetComponent<car3>().controlled)
		{
			this.ucar.GetComponent<car3>().controlled = false;
			this.ucar.GetComponent<car3>().userControlled = false;
			this.ucarAudio.GetComponent<AudioControlCar>().TurnOff();
			this.ucar.GetComponent<car3>().TurnOff();
			return;
		}
		this.ucar.GetComponent<car3>().controlled = true;
		this.ucar.GetComponent<car3>().userControlled = true;
		this.ucar.GetComponent<car3>().TurnOn();
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x000323B4 File Offset: 0x000305B4
	public void AdjustTractionSurf(int newSurf)
	{
		if (!this.carscript.enableWd4)
		{
			this.w4bonus = 0f;
		}
		else
		{
			this.w4bonus = 0.5f;
		}
		float num = 0f;
		if (this.FLindex == 0)
		{
			this.FLindex = 1;
		}
		WheelFrictionCurve forwardFriction = this.carscript.WheelFrontLeft.forwardFriction;
		this.rockTrac = this.tractionVals[this.FLindex - 1, 2];
		this.mudTrac = this.tractionVals[this.FLindex - 1, 1];
		num += this.mudTrac;
		if (newSurf == 1)
		{
			this.FLtotal = this.FLbonus + (this.rockTrac + this.mudTrac) / 2f;
		}
		if (newSurf == 2 || newSurf == 4)
		{
			this.FLtotal = this.FLbonus + this.mudTrac;
		}
		if (newSurf == 3)
		{
			this.FLtotal = this.FLbonus + this.rockTrac;
		}
		this.carscript.WheelFrontLeft.forwardFriction = forwardFriction;
		WheelFrictionCurve forwardFriction2 = this.carscript.WheelFrontRight.forwardFriction;
		if (this.FRindex == 0)
		{
			this.FRindex = 1;
		}
		this.rockTrac = this.tractionVals[this.FRindex - 1, 2];
		this.mudTrac = this.tractionVals[this.FRindex - 1, 1];
		num += this.mudTrac;
		if (newSurf == 1)
		{
			this.FRtotal = this.FRbonus + (this.rockTrac + this.mudTrac) / 2f;
		}
		if (newSurf == 2 || newSurf == 4)
		{
			this.FRtotal = this.FRbonus + this.mudTrac;
		}
		if (newSurf == 3)
		{
			this.FRtotal = this.FRbonus + this.rockTrac;
		}
		this.carscript.WheelFrontRight.forwardFriction = forwardFriction2;
		WheelFrictionCurve forwardFriction3 = this.carscript.WheelRearLeft.forwardFriction;
		if (this.RLindex == 0)
		{
			this.RLindex = 1;
		}
		this.rockTrac = this.tractionVals[this.RLindex - 1, 2];
		this.mudTrac = this.tractionVals[this.RLindex - 1, 1];
		num += this.mudTrac;
		if (newSurf == 1)
		{
			this.RLtotal = this.RLbonus + (this.rockTrac + this.mudTrac) / 2f;
		}
		if (newSurf == 2 || newSurf == 4)
		{
			this.RLtotal = this.RLbonus + this.mudTrac;
		}
		if (newSurf == 3)
		{
			this.RLtotal = this.RLbonus + this.rockTrac;
		}
		this.carscript.WheelRearLeft.forwardFriction = forwardFriction3;
		WheelFrictionCurve forwardFriction4 = this.carscript.WheelRearRight.forwardFriction;
		if (this.RRindex == 0)
		{
			this.RRindex = 1;
		}
		this.rockTrac = this.tractionVals[this.RRindex - 1, 2];
		this.mudTrac = this.tractionVals[this.RRindex - 1, 1];
		num += this.mudTrac;
		if (newSurf == 1)
		{
			this.RRtotal = this.RRbonus + (this.rockTrac + this.mudTrac) / 2f;
		}
		if (newSurf == 2 || newSurf == 4)
		{
			this.RRtotal = this.RRbonus + this.mudTrac;
		}
		if (newSurf == 3)
		{
			this.RRtotal = this.RRbonus + this.rockTrac;
		}
		this.carscript.WheelRearRight.forwardFriction = forwardFriction4;
		if (newSurf == 4)
		{
			this.carscript.deepWater = 1;
		}
		else
		{
			this.carscript.deepWater = 0;
		}
		if (newSurf == 2)
		{
			if (this.w4bonus > 0f)
			{
				this.newDrag = -0.29f * num + 2.04f;
			}
			else
			{
				this.newDrag = -0.15f * num + 4.6f;
			}
			if (this.newDrag < 0.05f)
			{
				this.newDrag = 0.1f;
			}
			if (this.newDrag > 7f)
			{
				this.newDrag = 7f;
			}
			this.truck.GetComponent<Rigidbody>().drag = this.newDrag;
			return;
		}
		if (newSurf == 4)
		{
			if (this.w4bonus > 0f)
			{
				this.newDrag = -0.29f * num + 4.04f;
			}
			else
			{
				this.newDrag = -0.15f * num + 7.6f;
			}
			if (this.newDrag < 0.05f)
			{
				this.newDrag = 0.15f;
			}
			if (this.newDrag > 8f)
			{
				this.newDrag = 8f;
			}
			this.truck.GetComponent<Rigidbody>().drag = this.newDrag;
			return;
		}
		this.truck.GetComponent<Rigidbody>().drag = 0.05f;
		this.newDrag = 0.05f;
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x0003283C File Offset: 0x00030A3C
	public void AdjustTractionSurfF(int newSurf)
	{
		if (!this.fcarscript.enableWd4)
		{
			this.w4bonus = 0f;
		}
		else
		{
			this.w4bonus = 0.5f;
		}
		float num = 0f;
		if (this.FLindex == 0)
		{
			this.FLindex = 1;
		}
		WheelFrictionCurve forwardFriction = this.fcarscript.WheelFrontLeft.forwardFriction;
		this.rockTrac = this.tractionVals[this.FLindex - 1, 2];
		this.mudTrac = this.tractionVals[this.FLindex - 1, 1];
		num += this.mudTrac;
		if (newSurf == 1)
		{
			this.FLtotal = this.FLbonus + (this.rockTrac + this.mudTrac) / 2f;
		}
		if (newSurf == 2 || newSurf == 4)
		{
			this.FLtotal = this.FLbonus + this.mudTrac;
		}
		if (newSurf == 3)
		{
			this.FLtotal = this.FLbonus + this.rockTrac;
		}
		this.fcarscript.WheelFrontLeft.forwardFriction = forwardFriction;
		if (this.FRindex == 0)
		{
			this.FRindex = 1;
		}
		WheelFrictionCurve forwardFriction2 = this.fcarscript.WheelFrontRight.forwardFriction;
		this.rockTrac = this.tractionVals[this.FRindex - 1, 2];
		this.mudTrac = this.tractionVals[this.FRindex - 1, 1];
		num += this.mudTrac;
		if (newSurf == 1)
		{
			this.FRtotal = this.FRbonus + (this.rockTrac + this.mudTrac) / 2f;
		}
		if (newSurf == 2 || newSurf == 4)
		{
			this.FRtotal = this.FRbonus + this.mudTrac;
		}
		if (newSurf == 3)
		{
			this.FRtotal = this.FRbonus + this.rockTrac;
		}
		this.fcarscript.WheelFrontRight.forwardFriction = forwardFriction2;
		if (this.RLindex == 0)
		{
			this.RLindex = 1;
		}
		WheelFrictionCurve forwardFriction3 = this.fcarscript.WheelRearLeft.forwardFriction;
		this.rockTrac = this.tractionVals[this.RLindex - 1, 2];
		this.mudTrac = this.tractionVals[this.RLindex - 1, 1];
		num += this.mudTrac;
		if (newSurf == 1)
		{
			this.RLtotal = this.RLbonus + (this.rockTrac + this.mudTrac) / 2f;
		}
		if (newSurf == 2 || newSurf == 4)
		{
			this.RLtotal = this.RLbonus + this.mudTrac;
		}
		if (newSurf == 3)
		{
			this.RLtotal = this.RLbonus + this.rockTrac;
		}
		this.fcarscript.WheelRearLeft.forwardFriction = forwardFriction3;
		if (this.RRindex == 0)
		{
			this.RRindex = 1;
		}
		WheelFrictionCurve forwardFriction4 = this.fcarscript.WheelRearRight.forwardFriction;
		this.rockTrac = this.tractionVals[this.RRindex - 1, 2];
		this.mudTrac = this.tractionVals[this.RRindex - 1, 1];
		num += this.mudTrac;
		if (newSurf == 1)
		{
			this.RRtotal = this.RRbonus + (this.rockTrac + this.mudTrac) / 2f;
		}
		if (newSurf == 2 || newSurf == 4)
		{
			this.RRtotal = this.RRbonus + this.mudTrac;
		}
		if (newSurf == 3)
		{
			this.RRtotal = this.RRbonus + this.rockTrac;
		}
		this.fcarscript.WheelRearRight.forwardFriction = forwardFriction4;
		if (newSurf == 4)
		{
			this.fcarscript.deepWater = 1;
		}
		else
		{
			this.fcarscript.deepWater = 0;
		}
		if (newSurf == 2)
		{
			if (this.w4bonus > 0f)
			{
				this.newDrag = -0.29f * num + 2.04f;
			}
			else
			{
				this.newDrag = -0.15f * num + 4.6f;
			}
			if (this.newDrag < 0.05f)
			{
				this.newDrag = 0.1f;
			}
			if (this.newDrag > 7f)
			{
				this.newDrag = 7f;
			}
			this.truck2.GetComponent<Rigidbody>().drag = this.newDrag;
			return;
		}
		if (newSurf == 4)
		{
			if (this.w4bonus > 0f)
			{
				this.newDrag = -0.29f * num + 4.04f;
			}
			else
			{
				this.newDrag = -0.15f * num + 7.6f;
			}
			if (this.newDrag < 0.05f)
			{
				this.newDrag = 0.15f;
			}
			if (this.newDrag > 8f)
			{
				this.newDrag = 8f;
			}
			this.truck2.GetComponent<Rigidbody>().drag = this.newDrag;
			return;
		}
		this.truck2.GetComponent<Rigidbody>().drag = 0.05f;
		this.newDrag = 0.05f;
	}

	// Token: 0x060004E1 RID: 1249 RVA: 0x00032CC4 File Offset: 0x00030EC4
	public void CalculateTraction()
	{
		if (!this.carscript.enableWd4)
		{
			this.w4bonus = 0f;
		}
		else
		{
			this.w4bonus = 0.5f;
		}
		this.FLbase = 1f;
		this.FRbase = 1f;
		this.RLbase = 1f;
		this.RRbase = 1f;
		this.FLbonus = 0f;
		this.FRbonus = 0f;
		this.RLbonus = 0f;
		this.RRbonus = 0f;
		foreach (object obj in this.wheelHolderFL.transform)
		{
			Transform transform = (Transform)obj;
			int siblingIndex = transform.GetSiblingIndex();
			if (siblingIndex > 5 && transform.gameObject.active)
			{
				this.FLindex = siblingIndex;
				this.FLbonus = transform.gameObject.GetComponent<Renderer>().material.GetFloat("_TireFlatnessT");
				this.gd.tireFL = transform.gameObject.GetComponent<Renderer>();
				this.standardTrac = (this.tractionVals[siblingIndex - 1, 1] + this.tractionVals[siblingIndex - 1, 2]) / 2f;
				this.mudTrac = this.tractionVals[siblingIndex - 1, 1];
				this.rockTrac = this.tractionVals[siblingIndex - 1, 2];
				this.FLtotal = this.FLbonus + this.standardTrac;
				break;
			}
			if (this.FLbonus == 0f)
			{
				this.FLbase = 0.2f;
				this.FLtotal = 0f;
			}
		}
		foreach (object obj2 in this.wheelHolderFR.transform)
		{
			Transform transform2 = (Transform)obj2;
			int siblingIndex2 = transform2.GetSiblingIndex();
			if (siblingIndex2 > 5 && transform2.gameObject.active)
			{
				this.FRindex = siblingIndex2;
				this.FRbonus = transform2.gameObject.GetComponent<Renderer>().material.GetFloat("_TireFlatnessT");
				this.gd.tireFR = transform2.gameObject.GetComponent<Renderer>();
				this.standardTrac = (this.tractionVals[siblingIndex2 - 1, 1] + this.tractionVals[siblingIndex2 - 1, 2]) / 2f;
				this.mudTrac = this.tractionVals[siblingIndex2 - 1, 1];
				this.rockTrac = this.tractionVals[siblingIndex2 - 1, 2];
				this.FRtotal = this.FRbonus + this.standardTrac;
				break;
			}
			if (this.FLbonus == 0f)
			{
				this.FRbase = 0.2f;
				this.FRtotal = 0f;
			}
		}
		foreach (object obj3 in this.wheelHolderRL.transform)
		{
			Transform transform3 = (Transform)obj3;
			int siblingIndex3 = transform3.GetSiblingIndex();
			if (siblingIndex3 > 5 && transform3.gameObject.active)
			{
				this.RLindex = siblingIndex3;
				this.RLbonus = transform3.gameObject.GetComponent<Renderer>().material.GetFloat("_TireFlatnessT");
				this.gd.tireRL = transform3.gameObject.GetComponent<Renderer>();
				this.standardTrac = (this.tractionVals[siblingIndex3 - 1, 1] + this.tractionVals[siblingIndex3 - 1, 2]) / 2f;
				this.mudTrac = this.tractionVals[siblingIndex3 - 1, 1];
				this.rockTrac = this.tractionVals[siblingIndex3 - 1, 2];
				this.RLtotal = this.RLbonus + this.standardTrac;
				break;
			}
			if (this.RLbonus == 0f)
			{
				this.RLbase = 0.2f;
				this.RLtotal = 0f;
			}
		}
		foreach (object obj4 in this.wheelHolderRR.transform)
		{
			Transform transform4 = (Transform)obj4;
			int siblingIndex4 = transform4.GetSiblingIndex();
			if (siblingIndex4 > 5 && transform4.gameObject.active)
			{
				this.RRindex = siblingIndex4;
				this.RRbonus = transform4.gameObject.GetComponent<Renderer>().material.GetFloat("_TireFlatnessT");
				this.gd.tireRR = transform4.gameObject.GetComponent<Renderer>();
				this.standardTrac = (this.tractionVals[siblingIndex4 - 1, 1] + this.tractionVals[siblingIndex4 - 1, 2]) / 2f;
				this.mudTrac = this.tractionVals[siblingIndex4 - 1, 1];
				this.rockTrac = this.tractionVals[siblingIndex4 - 1, 2];
				this.RRtotal = this.RRbonus + this.standardTrac;
				break;
			}
			if (this.RRbonus == 0f)
			{
				this.RRbase = 0.2f;
				this.RRtotal = 0f;
			}
		}
		float num = 0.8f;
		WheelFrictionCurve forwardFriction = this.carscript.WheelFrontLeft.forwardFriction;
		float num2 = 1.2f - (this.FLbase + this.FLtotal + this.w4bonus) * 0.285714f;
		float num3 = 0.68f + (this.FLbase + this.FLtotal + this.w4bonus) * 0.314286f;
		forwardFriction.extremumSlip = num2 * num;
		forwardFriction.asymptoteSlip = num2 * num;
		forwardFriction.extremumValue = num3 * num;
		forwardFriction.asymptoteValue = num3 * num;
		this.carscript.WheelFrontLeft.forwardFriction = forwardFriction;
		WheelFrictionCurve forwardFriction2 = this.carscript.WheelFrontRight.forwardFriction;
		float num4 = 1.2f - (this.FRbase + this.FRtotal + this.w4bonus) * 0.285714f;
		float num5 = 0.68f + (this.FRbase + this.FRtotal + this.w4bonus) * 0.314286f;
		forwardFriction2.extremumSlip = num4 * num;
		forwardFriction2.asymptoteSlip = num4 * num;
		forwardFriction2.extremumValue = num5 * num;
		forwardFriction2.asymptoteValue = num5 * num;
		this.carscript.WheelFrontRight.forwardFriction = forwardFriction2;
		WheelFrictionCurve forwardFriction3 = this.carscript.WheelRearLeft.forwardFriction;
		float num6 = 1.2f - (this.RLbase + this.RLtotal + this.w4bonus) * 0.285714f;
		float num7 = 0.68f + (this.RLbase + this.RLtotal + this.w4bonus) * 0.314286f;
		forwardFriction3.extremumSlip = num6 * num;
		forwardFriction3.asymptoteSlip = num6 * num;
		forwardFriction3.extremumValue = num7 * num;
		forwardFriction3.asymptoteValue = num7 * num;
		this.carscript.WheelRearLeft.forwardFriction = forwardFriction3;
		WheelFrictionCurve forwardFriction4 = this.carscript.WheelRearRight.forwardFriction;
		float num8 = 1.2f - (this.RRbase + this.RRtotal + this.w4bonus) * 0.285714f;
		float num9 = 0.68f + (this.RRbase + this.RRtotal + this.w4bonus) * 0.314286f;
		forwardFriction4.extremumSlip = num8 * num;
		forwardFriction4.asymptoteSlip = num8 * num;
		forwardFriction4.extremumValue = num9 * num;
		forwardFriction4.asymptoteValue = num9 * num;
		this.carscript.WheelRearRight.forwardFriction = forwardFriction4;
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x000334EC File Offset: 0x000316EC
	public void CalculateTractionF()
	{
		if (!this.fcarscript.enableWd4)
		{
			this.w4bonus = 0f;
		}
		else
		{
			this.w4bonus = 0.5f;
		}
		this.FLbase = 1f;
		this.FRbase = 1f;
		this.RLbase = 1f;
		this.RRbase = 1f;
		this.FLbonus = 0f;
		this.FRbonus = 0f;
		this.RLbonus = 0f;
		this.RRbonus = 0f;
		foreach (object obj in this.wheelHolderFLF.transform)
		{
			Transform transform = (Transform)obj;
			int siblingIndex = transform.GetSiblingIndex();
			if (siblingIndex > 5 && transform.gameObject.active)
			{
				this.FLindex = siblingIndex;
				this.FLbonus = transform.gameObject.GetComponent<Renderer>().material.GetFloat("_TireFlatnessT");
				this.gd.tireFLF = transform.gameObject.GetComponent<Renderer>();
				this.standardTrac = (this.tractionVals[siblingIndex - 1, 1] + this.tractionVals[siblingIndex - 1, 2]) / 2f;
				this.mudTrac = this.tractionVals[siblingIndex - 1, 1];
				this.rockTrac = this.tractionVals[siblingIndex - 1, 2];
				this.FLtotal = this.FLbonus + this.standardTrac;
				break;
			}
			if (this.FLbonus == 0f)
			{
				this.FLbase = 0.2f;
				this.FLtotal = 0f;
			}
		}
		foreach (object obj2 in this.wheelHolderFRF.transform)
		{
			Transform transform2 = (Transform)obj2;
			int siblingIndex2 = transform2.GetSiblingIndex();
			if (siblingIndex2 > 5 && transform2.gameObject.active)
			{
				this.FRindex = siblingIndex2;
				this.FRbonus = transform2.gameObject.GetComponent<Renderer>().material.GetFloat("_TireFlatnessT");
				this.gd.tireFRF = transform2.gameObject.GetComponent<Renderer>();
				this.standardTrac = (this.tractionVals[siblingIndex2 - 1, 1] + this.tractionVals[siblingIndex2 - 1, 2]) / 2f;
				this.mudTrac = this.tractionVals[siblingIndex2 - 1, 1];
				this.rockTrac = this.tractionVals[siblingIndex2 - 1, 2];
				this.FRtotal = this.FRbonus + this.standardTrac;
				break;
			}
			if (this.FLbonus == 0f)
			{
				this.FRbase = 0.2f;
				this.FRtotal = 0f;
			}
		}
		foreach (object obj3 in this.wheelHolderRLF.transform)
		{
			Transform transform3 = (Transform)obj3;
			int siblingIndex3 = transform3.GetSiblingIndex();
			if (siblingIndex3 > 5 && transform3.gameObject.active)
			{
				this.RLindex = siblingIndex3;
				this.RLbonus = transform3.gameObject.GetComponent<Renderer>().material.GetFloat("_TireFlatnessT");
				this.gd.tireRLF = transform3.gameObject.GetComponent<Renderer>();
				this.standardTrac = (this.tractionVals[siblingIndex3 - 1, 1] + this.tractionVals[siblingIndex3 - 1, 2]) / 2f;
				this.mudTrac = this.tractionVals[siblingIndex3 - 1, 1];
				this.rockTrac = this.tractionVals[siblingIndex3 - 1, 2];
				this.RLtotal = this.RLbonus + this.standardTrac;
				break;
			}
			if (this.RLbonus == 0f)
			{
				this.RLbase = 0.2f;
				this.RLtotal = 0f;
			}
		}
		foreach (object obj4 in this.wheelHolderRRF.transform)
		{
			Transform transform4 = (Transform)obj4;
			int siblingIndex4 = transform4.GetSiblingIndex();
			if (siblingIndex4 > 5 && transform4.gameObject.active)
			{
				this.RRindex = siblingIndex4;
				this.RRbonus = transform4.gameObject.GetComponent<Renderer>().material.GetFloat("_TireFlatnessT");
				this.gd.tireRRF = transform4.gameObject.GetComponent<Renderer>();
				this.standardTrac = (this.tractionVals[siblingIndex4 - 1, 1] + this.tractionVals[siblingIndex4 - 1, 2]) / 2f;
				this.mudTrac = this.tractionVals[siblingIndex4 - 1, 1];
				this.rockTrac = this.tractionVals[siblingIndex4 - 1, 2];
				this.RRtotal = this.RRbonus + this.standardTrac;
				break;
			}
			if (this.RRbonus == 0f)
			{
				this.RRbase = 0.2f;
				this.RRtotal = 0f;
			}
		}
		float num = 0.8f;
		WheelFrictionCurve forwardFriction = this.fcarscript.WheelFrontLeft.forwardFriction;
		float num2 = 1.2f - (this.FLbase + this.FLtotal + this.w4bonus) * 0.285714f;
		float num3 = 0.68f + (this.FLbase + this.FLtotal + this.w4bonus) * 0.314286f;
		forwardFriction.extremumSlip = num2 * num;
		forwardFriction.asymptoteSlip = num2 * num;
		forwardFriction.extremumValue = num3 * num;
		forwardFriction.asymptoteValue = num3 * num;
		this.fcarscript.WheelFrontLeft.forwardFriction = forwardFriction;
		WheelFrictionCurve forwardFriction2 = this.fcarscript.WheelFrontRight.forwardFriction;
		float num4 = 1.2f - (this.FRbase + this.FRtotal + this.w4bonus) * 0.285714f;
		float num5 = 0.68f + (this.FRbase + this.FRtotal + this.w4bonus) * 0.314286f;
		forwardFriction2.extremumSlip = num4 * num;
		forwardFriction2.asymptoteSlip = num4 * num;
		forwardFriction2.extremumValue = num5 * num;
		forwardFriction2.asymptoteValue = num5 * num;
		this.fcarscript.WheelFrontRight.forwardFriction = forwardFriction2;
		WheelFrictionCurve forwardFriction3 = this.fcarscript.WheelRearLeft.forwardFriction;
		float num6 = 1.2f - (this.RLbase + this.RLtotal + this.w4bonus) * 0.285714f;
		float num7 = 0.68f + (this.RLbase + this.RLtotal + this.w4bonus) * 0.314286f;
		forwardFriction3.extremumSlip = num6 * num;
		forwardFriction3.asymptoteSlip = num6 * num;
		forwardFriction3.extremumValue = num7 * num;
		forwardFriction3.asymptoteValue = num7 * num;
		this.fcarscript.WheelRearLeft.forwardFriction = forwardFriction3;
		WheelFrictionCurve forwardFriction4 = this.fcarscript.WheelRearRight.forwardFriction;
		float num8 = 1.2f - (this.RRbase + this.RRtotal + this.w4bonus) * 0.285714f;
		float num9 = 0.68f + (this.RRbase + this.RRtotal + this.w4bonus) * 0.314286f;
		forwardFriction4.extremumSlip = num8 * num;
		forwardFriction4.asymptoteSlip = num8 * num;
		forwardFriction4.extremumValue = num9 * num;
		forwardFriction4.asymptoteValue = num9 * num;
		this.fcarscript.WheelRearRight.forwardFriction = forwardFriction4;
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x00033D14 File Offset: 0x00031F14
	public void ToggleLights()
	{
		this.truckLightRL.GetComponent<Light>().enabled = !this.truckLightRL.GetComponent<Light>().enabled;
		this.truckLightL.GetComponent<LensFlare>().enabled = !this.truckLightL.GetComponent<LensFlare>().enabled;
		this.truckLightR.GetComponent<LensFlare>().enabled = !this.truckLightR.GetComponent<LensFlare>().enabled;
		this.tailLightL.enabled = !this.tailLightL.enabled;
		this.tailLightR.enabled = !this.tailLightR.enabled;
		this.tailFlareL.enabled = !this.tailFlareL.enabled;
		this.tailFlareR.enabled = !this.tailFlareR.enabled;
	}

	// Token: 0x060004E4 RID: 1252 RVA: 0x00033DF0 File Offset: 0x00031FF0
	public void ToggleLightsF()
	{
		this.truckFLightRL.GetComponent<Light>().enabled = !this.truckFLightRL.GetComponent<Light>().enabled;
		this.truckFLightL.GetComponent<LensFlare>().enabled = !this.truckFLightL.GetComponent<LensFlare>().enabled;
		this.truckFLightR.GetComponent<LensFlare>().enabled = !this.truckFLightR.GetComponent<LensFlare>().enabled;
		this.truckFtaillights.SetActive(this.truckFLightR.GetComponent<LensFlare>().enabled);
	}

	// Token: 0x060004E5 RID: 1253 RVA: 0x00033E84 File Offset: 0x00032084
	public void ToggleLightsCar()
	{
		this.carLightRL.GetComponent<Light>().enabled = !this.carLightRL.GetComponent<Light>().enabled;
		this.carLightL.GetComponent<LensFlare>().enabled = !this.carLightL.GetComponent<LensFlare>().enabled;
		this.carLightR.GetComponent<LensFlare>().enabled = !this.carLightR.GetComponent<LensFlare>().enabled;
		this.carTaillights.SetActive(this.carLightR.GetComponent<LensFlare>().enabled);
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x00033F18 File Offset: 0x00032118
	public void Enable4w()
	{
		if (this.carscript.enableWd4)
		{
			this.carscript.enable4(false);
			this.gd.wd4 = false;
			this.gd.GetTerrainTexture();
			return;
		}
		this.carscript.enable4(true);
		this.gd.wd4 = true;
		this.gd.GetTerrainTexture();
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x00033F7C File Offset: 0x0003217C
	public void Enable4wF()
	{
		if (this.fcarscript.enableWd4)
		{
			this.fcarscript.enable4(false);
			this.gd.wd4 = false;
			this.gd.GetTerrainTexture();
			return;
		}
		this.fcarscript.enable4(true);
		this.gd.wd4 = true;
		this.gd.GetTerrainTexture();
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x00002188 File Offset: 0x00000388
	public void emptyTire()
	{
	}

	// Token: 0x060004E9 RID: 1257 RVA: 0x00033FE0 File Offset: 0x000321E0
	public void LayOnCreeper()
	{
		if (this.onCreeper)
		{
			base.StartCoroutine(this.GetOffCreeper());
			return;
		}
		if (this.creeper.transform.parent == null)
		{
			this.drivingCar = false;
			if (Vector3.Dot(this.creeper.transform.up, Vector3.down) > 0f)
			{
				this.creeper.transform.eulerAngles = new Vector3(0f, this.creeper.transform.eulerAngles.y, 0f);
				return;
			}
			this.whichCar = 3;
			this.creeper.GetComponent<PickUp>().pickable = false;
			this.person.transform.parent = this.creeperMount.transform;
			this.fpc.canMove = false;
			this.person.layer = 9;
			this.person.transform.position = this.creeperMount.transform.position;
			this.onCreeper = true;
			this.creeper.GetComponent<CreeperMovement>().controlled = true;
		}
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x000340FF File Offset: 0x000322FF
	public void Subtitle(string dialogue, AudioClip aClip)
	{
		base.StartCoroutine(this.ShowSubtitle(dialogue, aClip));
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x00034110 File Offset: 0x00032310
	private IEnumerator ShowSubtitle(string dialogue, AudioClip aClip)
	{
		this.subtitles.GetComponent<Text>().text = dialogue;
		float seconds = aClip.length;
		if (aClip.length != 0f)
		{
			float length = aClip.length;
		}
		else
		{
			seconds = 5f;
		}
		yield return new WaitForSeconds(seconds);
		this.subtitles.GetComponent<Text>().text = "";
		yield break;
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x0003412D File Offset: 0x0003232D
	private IEnumerator GetOffCreeper()
	{
		this.itemsInDismount = false;
		foreach (Collider collider in Physics.OverlapSphere(this.creeper.GetComponent<CreeperMovement>().dismountSphere.transform.position, 0f))
		{
			this.itemsInDismount = true;
		}
		if (!this.itemsInDismount)
		{
			this.person.layer = 9;
			this.person.transform.parent = null;
			this.person.transform.position = this.creeperUnmount.transform.position;
			yield return new WaitForSeconds(0.2f);
			this.person.layer = 2;
			this.fpc.canMove = true;
			this.onCreeper = false;
			this.creeper.GetComponent<CreeperMovement>().controlled = false;
			this.creeper.GetComponent<PickUp>().pickable = true;
		}
		yield break;
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x0003413C File Offset: 0x0003233C
	public void LeanIn()
	{
		if (!this.drivingCar)
		{
			this.iTweenArgs = iTween.Hash(Array.Empty<object>());
			this.iTweenArgs.Add("position", this.leanDest.transform.position);
			this.iTweenArgs.Add("time", 0f);
			this.iTweenArgs.Add("islocal", false);
			iTween.MoveTo(this.cameraTransform, this.iTweenArgs);
			this.leaned = true;
		}
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x000341D0 File Offset: 0x000323D0
	public void LeanBack()
	{
		if (this.leaned)
		{
			this.iTweenArgs = iTween.Hash(Array.Empty<object>());
			this.iTweenArgs.Add("position", this.leanDestO.transform.position);
			this.iTweenArgs.Add("time", 0f);
			this.iTweenArgs.Add("islocal", false);
			iTween.MoveTo(this.cameraTransform, this.iTweenArgs);
			this.leaned = false;
		}
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x00034264 File Offset: 0x00032464
	public void LookOut()
	{
		if (this.drivingCar && (this.whichCar == 1 || this.whichCar == 6))
		{
			if (!this.watching)
			{
				this.leftGlass.GetComponent<Renderer>().enabled = false;
				this.iTweenArgs = iTween.Hash(Array.Empty<object>());
				if (this.whichCar == 1)
				{
					this.iTweenArgs.Add("position", this.watchMount.transform.position);
				}
				else
				{
					this.iTweenArgs.Add("position", this.watchMountF.transform.position);
				}
				this.iTweenArgs.Add("time", 0);
				this.iTweenArgs.Add("islocal", false);
				this.iTweenArgs.Add("lookahead", 1);
				iTween.MoveUpdate(base.gameObject, this.iTweenArgs);
				this.watching = true;
				return;
			}
			this.leftGlass.GetComponent<Renderer>().enabled = true;
			this.iTweenArgs = iTween.Hash(Array.Empty<object>());
			if (this.whichCar == 1)
			{
				this.iTweenArgs.Add("position", this.seatMount.transform.position);
			}
			else
			{
				this.iTweenArgs.Add("position", this.seatMountF.transform.position);
			}
			this.iTweenArgs.Add("time", 0);
			this.iTweenArgs.Add("islocal", false);
			this.iTweenArgs.Add("lookahead", 1);
			iTween.MoveUpdate(base.gameObject, this.iTweenArgs);
			this.watching = false;
		}
	}

	// Token: 0x060004F0 RID: 1264 RVA: 0x00034440 File Offset: 0x00032640
	public void GetIn()
	{
		this.inoutVehicle = true;
		if (!this.drivingCar && !this.onCreeper && !Input.GetKeyDown(this.cr.LeanIn))
		{
			this.truck.transform.GetChild(0).GetComponent<SuspensionOffroadCar>().enabled = true;
			this.truckBed.enabled = true;
			this.truckBed.CheckBed();
			this.trailerBedC.enabled = true;
			this.trailerBedH.enabled = true;
			this.trailerBedC.CheckBed();
			this.trailerBedH.CheckBed();
			if (!this.carscript.usingV8 && !this.carscript.usingI6)
			{
				this.fourcyl.GetComponent<engine>().Refresh();
			}
			else if (this.carscript.usingI6)
			{
				this.i6.GetComponent<enginei6>().Refresh();
			}
			else
			{
				this.v8.GetComponent<enginev8>().Refresh();
			}
			if (this.carscript.controlled)
			{
				this.enterTime = 0;
			}
			else
			{
				this.enterTime = 1;
			}
			this.person.transform.parent = this.seatMount.transform;
			this.fpc.canMove = false;
			this.person.layer = 9;
			this.enterposition = new Vector3(this.person.transform.position.x, this.person.transform.position.y, this.person.transform.position.z);
			this.iTweenArgs = iTween.Hash(Array.Empty<object>());
			this.iTweenArgs.Add("position", this.seatMount.transform.position);
			this.iTweenArgs.Add("time", this.enterTime);
			this.iTweenArgs.Add("islocal", false);
			iTween.MoveTo(base.gameObject, this.iTweenArgs);
			this.waterMax.CheckWaterAgain();
			this.drivingCar = true;
			this.whichCar = 1;
			this.fpc.m_MouseLook.m_CharacterTargetRot = Quaternion.Euler(0f, 0f, 0f);
			this.fpc.m_MouseLook.m_CameraTargetRot = Quaternion.Euler(20f, 0f, 0f);
			this.carscript.EnableWC();
			if (this.carscript.controlled)
			{
				this.carscript.userControlled = true;
			}
			if (this.canRun && this.keyState == 2)
			{
				this.carscript.userControlled = true;
			}
			base.StartCoroutine(this.AdjustSeatTruck());
			this.CalculateTraction();
		}
	}

	// Token: 0x060004F1 RID: 1265 RVA: 0x00034702 File Offset: 0x00032902
	private IEnumerator AdjustSeatTruck()
	{
		yield return new WaitForSeconds(1.1f);
		if (this.person.transform.parent == this.seatMount.transform)
		{
			this.person.transform.position = this.seatMount.transform.position;
		}
		this.inoutVehicle = false;
		yield break;
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x00034714 File Offset: 0x00032914
	public void GetInF()
	{
		this.inoutVehicle = true;
		if (!this.drivingCar && !this.onCreeper && !Input.GetKeyDown(this.cr.LeanIn))
		{
			this.truck2.transform.GetChild(0).GetComponent<SuspensionOffroadCar>().enabled = true;
			Debug.Log("susptrue");
			this.truckBed2.enabled = true;
			this.truckBed2.CheckBed();
			this.trailerBedC.enabled = true;
			this.trailerBedH.enabled = true;
			this.trailerBedC.CheckBed();
			this.trailerBedH.CheckBed();
			if (this.fcarscript.usingV8)
			{
				this.v8.GetComponent<enginev8>().Refresh();
			}
			if (this.fcarscript.controlled)
			{
				this.enterTime = 0;
			}
			else
			{
				this.enterTime = 1;
			}
			this.person.transform.parent = this.seatMountF.transform;
			this.fpc.canMove = false;
			this.person.layer = 9;
			this.enterposition = new Vector3(this.person.transform.position.x, this.person.transform.position.y, this.person.transform.position.z);
			this.iTweenArgs = iTween.Hash(Array.Empty<object>());
			this.iTweenArgs.Add("position", this.seatMountF.transform.position);
			this.iTweenArgs.Add("time", this.enterTime);
			this.iTweenArgs.Add("islocal", false);
			iTween.MoveTo(base.gameObject, this.iTweenArgs);
			this.waterMaxF.CheckWaterAgain();
			this.drivingCar = true;
			this.whichCar = 6;
			this.fpc.m_MouseLook.m_CharacterTargetRot = Quaternion.Euler(0f, 0f, 0f);
			this.fpc.m_MouseLook.m_CameraTargetRot = Quaternion.Euler(20f, 0f, 0f);
			this.fcarscript.EnableWC();
			if (this.fcarscript.controlled)
			{
				this.fcarscript.userControlled = true;
			}
			if (this.canRun && this.keyStateF == 2)
			{
				this.fcarscript.userControlled = true;
			}
			base.StartCoroutine(this.AdjustSeatTruckF());
			this.CalculateTractionF();
		}
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x000349A2 File Offset: 0x00032BA2
	private IEnumerator AdjustSeatTruckF()
	{
		yield return new WaitForSeconds(1.1f);
		if (this.person.transform.parent == this.seatMountF.transform)
		{
			this.person.transform.position = this.seatMountF.transform.position;
		}
		this.inoutVehicle = false;
		yield break;
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x000349B4 File Offset: 0x00032BB4
	public void GetInCar()
	{
		if (!this.drivingCar && !this.onCreeper && !Input.GetKeyDown(this.cr.LeanIn))
		{
			this.person.transform.parent = this.seatMountCar.transform;
			this.fpc.canMove = false;
			this.person.layer = 9;
			this.person.transform.position = this.seatMountCar.transform.position;
			this.drivingCar = true;
			this.whichCar = 2;
			this.waterMaxC.CheckWaterAgain();
			this.fpc.m_MouseLook.m_CharacterTargetRot = Quaternion.Euler(0f, 0f, 0f);
			this.fpc.m_MouseLook.m_CameraTargetRot = Quaternion.Euler(20f, 0f, 0f);
			this.ucar.GetComponent<car3>().updateEngine();
			this.ucar.GetComponent<car3>().EnableWC();
			if (this.ucar.GetComponent<car3>().userControlled)
			{
				this.ucar.GetComponent<car3>().controlled = true;
			}
			base.StartCoroutine(this.AdjustSeatCar());
		}
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x00034AF1 File Offset: 0x00032CF1
	private IEnumerator AdjustSeatCar()
	{
		yield return new WaitForSeconds(0.1f);
		if (this.person.transform.parent == this.seatMountCar.transform)
		{
			this.person.transform.position = this.seatMountCar.transform.position;
		}
		yield break;
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x00034B00 File Offset: 0x00032D00
	public void GetInCart()
	{
		if (!this.drivingCar && !this.onCreeper && !Input.GetKeyDown(this.cr.LeanIn))
		{
			this.person.transform.parent = this.seatMountCart.transform;
			this.fpc.canMove = false;
			this.person.layer = 9;
			this.person.transform.position = this.seatMountCart.transform.position;
			this.drivingCar = true;
			this.whichCar = 4;
			this.fpc.m_MouseLook.m_CharacterTargetRot = Quaternion.Euler(0f, 0f, 0f);
			this.fpc.m_MouseLook.m_CameraTargetRot = Quaternion.Euler(20f, 0f, 0f);
			this.cart.GetComponent<golfcart>().controlled = true;
			this.cart.GetComponent<golfcart>().userControlled = true;
		}
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x00034C04 File Offset: 0x00032E04
	public void GetInDirtbike()
	{
		if (!this.drivingCar && !this.onCreeper && !Input.GetKeyDown(this.cr.LeanIn) && this.dirtbike.GetComponent<PickUp>().pickable)
		{
			this.dirtbike.transform.parent = null;
			if (this.dirtbike.GetComponent<FixedJoint>() != null)
			{
				Object.Destroy(this.dirtbike.GetComponent<FixedJoint>());
			}
			this.person.transform.parent = this.seatMountBike.transform;
			this.fpc.canMove = false;
			this.person.layer = 9;
			this.person.transform.position = this.seatMountBike.transform.position;
			this.drivingCar = true;
			this.whichCar = 5;
			this.fpc.m_MouseLook.m_CharacterTargetRot = Quaternion.Euler(0f, 0f, 0f);
			this.fpc.m_MouseLook.m_CameraTargetRot = Quaternion.Euler(20f, 0f, 0f);
			this.dirtbike.GetComponent<Dirtbike>().Sit();
			this.ShowToolTips(0);
		}
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x00034D46 File Offset: 0x00032F46
	private IEnumerator FreezeRot()
	{
		yield return 500;
		yield break;
	}

	// Token: 0x060004F9 RID: 1273 RVA: 0x00034D4E File Offset: 0x00032F4E
	public void GetOut()
	{
		if (this.drivingCar && !this.watching)
		{
			base.StartCoroutine(this.GetOutC());
		}
	}

	// Token: 0x060004FA RID: 1274 RVA: 0x00034D6D File Offset: 0x00032F6D
	private IEnumerator GetOutC()
	{
		this.person.transform.parent = null;
		if (this.whichCar == 1)
		{
			this.person.transform.position = this.exitMount.transform.position;
			this.truckBed.enabled = false;
			this.truckBed.CheckBed();
			this.trailerBedC.enabled = false;
			this.trailerBedH.enabled = false;
			this.trailerBedC.CheckBed();
			this.trailerBedH.CheckBed();
			for (int i = 0; i < 4; i++)
			{
				this.carscript.splashFx[i].SetActive(false);
			}
		}
		else if (this.whichCar == 2)
		{
			this.person.transform.position = this.exitMountCar.transform.position;
			for (int j = 0; j < 4; j++)
			{
				this.ucar.GetComponent<car3>().splashFx[j].SetActive(false);
			}
			this.ucar.GetComponent<car3>().userControlled = false;
		}
		else if (this.whichCar == 4)
		{
			this.person.transform.position = this.exitMountCart.transform.position;
		}
		else if (this.whichCar == 5)
		{
			this.person.transform.position = this.exitMountBike.transform.position;
			this.dirtbike.GetComponent<Dirtbike>().Stand();
		}
		if (this.whichCar == 6)
		{
			this.person.transform.position = this.exitMountF.transform.position;
			this.truckBed2.enabled = false;
			this.truckBed2.CheckBed();
			this.trailerBedC.enabled = false;
			this.trailerBedC.CheckBed();
			this.trailerBedH.enabled = false;
			this.trailerBedH.CheckBed();
		}
		yield return 50;
		this.person.layer = 2;
		this.fpc.canMove = true;
		this.drivingCar = false;
		yield break;
	}

	// Token: 0x060004FB RID: 1275 RVA: 0x00034D7C File Offset: 0x00032F7C
	private IEnumerator CheckAutoSave()
	{
		yield return new WaitForSeconds(2f);
		if (this.person.transform.parent == null)
		{
			this.timeOfDay = EnviroSkyMgr.instance.GetTimeOfDay();
			this.weatherId = EnviroSkyMgr.instance.GetWeatherID();
			if (Time.time - this.autoSaveTime > 480f && this.pickedUpObject == null)
			{
				this.mm.OptionSaveAuto();
				this.autoSaveTime = Time.time;
				this.toolTipPanel.SetActive(true);
				this.autoSavePanel.SetActive(true);
			}
		}
		yield return new WaitForSeconds(2f);
		this.toolTipPanel.SetActive(false);
		this.autoSavePanel.SetActive(false);
		yield break;
	}

	// Token: 0x060004FC RID: 1276 RVA: 0x00034D8C File Offset: 0x00032F8C
	public void GetOut2()
	{
		if (this.person.transform.parent != null)
		{
			if (this.whichCar == 3)
			{
				this.GetOffCreeper();
			}
			else
			{
				this.person.layer = 9;
				this.inoutVehicle = true;
				this.enterposition = new Vector3(this.person.transform.position.x, this.person.transform.position.y, this.person.transform.position.z);
				this.iTweenArgs = iTween.Hash(Array.Empty<object>());
				if (this.whichCar == 1)
				{
					if (!this.exitMount.GetComponent<driverExit>().exitBlocked && !this.exitMount.GetComponent<driverExit>().IsOnDriverSide())
					{
						this.iTweenArgs.Add("position", this.exitMount.transform.position);
					}
					else
					{
						this.iTweenArgs.Add("position", this.exitMountP.transform.position);
					}
				}
				if (this.whichCar == 2)
				{
					if (!this.exitMountCar.GetComponent<driverExit>().exitBlocked && !this.exitMountCar.GetComponent<driverExit>().IsOnDriverSide())
					{
						this.iTweenArgs.Add("position", this.exitMountCar.transform.position);
					}
					else
					{
						this.iTweenArgs.Add("position", this.exitMountCarP.transform.position);
					}
					this.ucar.GetComponent<car3>().controlled = false;
				}
				if (this.whichCar == 4)
				{
					this.iTweenArgs.Add("position", this.exitMountCart.transform.position);
					this.cart.GetComponent<golfcart>().userControlled = false;
					this.cart.GetComponent<golfcart>().controlled = false;
					this.cart.GetComponent<golfcart>().StopCart();
				}
				if (this.whichCar == 5)
				{
					this.iTweenArgs.Add("position", this.exitMountBike.transform.position);
					this.dirtbike.GetComponent<Dirtbike>().Stand();
				}
				if (this.whichCar == 6)
				{
					if (!this.exitMountF.GetComponent<driverExit>().exitBlocked && !this.exitMountF.GetComponent<driverExit>().IsOnDriverSide())
					{
						this.iTweenArgs.Add("position", this.exitMountF.transform.position);
					}
					else
					{
						this.iTweenArgs.Add("position", this.exitMountFP.transform.position);
					}
					this.truck2.GetComponent<car4>().userControlled = false;
				}
				this.iTweenArgs.Add("time", 0.3f);
				this.iTweenArgs.Add("islocal", false);
				this.person.transform.parent = null;
				iTween.MoveTo(base.gameObject, this.iTweenArgs);
				base.StartCoroutine(this.PersonWalk());
				this.carscript.userControlled = false;
				if (this.whichCar == 1 && this.carscript.speed > 80f)
				{
					Achievement achievement = new Achievement("ACH_EXHAUSTED");
					achievement.Trigger(true);
				}
			}
			base.StartCoroutine(this.CheckAutoSave());
		}
	}

	// Token: 0x060004FD RID: 1277 RVA: 0x00035110 File Offset: 0x00033310
	private IEnumerator PersonWalk()
	{
		yield return new WaitForSeconds(0.1f);
		this.fpc.canMove = true;
		this.drivingCar = false;
		this.person.layer = 2;
		this.inoutVehicle = false;
		yield break;
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x00035120 File Offset: 0x00033320
	public void JackDown()
	{
		string name = this.hit.collider.transform.GetChild(1).name;
		int num = int.Parse(name.Substring(8, 1));
		if (name.Length == 10)
		{
			num = int.Parse(name.Substring(8, 2));
		}
		float num2 = (float)this.jackHeight * 0.01f;
		this.jackHeight = 0;
		Object.Instantiate<GameObject>(this.JackObj, this.hiJack[num].transform.position, this.hiJack[num].transform.rotation).name = this.JackObj.name;
		this.jackBody[num].transform.position = new Vector3(this.jackBody[num].transform.position.x, this.jackBody[num].transform.position.y + num2, this.jackBody[num].transform.position.z);
		this.hiJack[num].SetActive(false);
		Debug.Log(num);
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x0003523C File Offset: 0x0003343C
	public void JackUp()
	{
		if (this.jackHeight < 50)
		{
			string name = this.hit.collider.transform.parent.GetChild(1).name;
			int num = int.Parse(name.Substring(8, 1));
			if (name.Length == 10)
			{
				num = int.Parse(name.Substring(8, 2));
			}
			Debug.Log(num);
			this.jackHeight++;
			this.interactiveObject.PerformAction();
			this.jackBody[num].transform.localPosition = new Vector3(this.jackBody[num].transform.localPosition.x, this.jackBody[num].transform.localPosition.y, this.jackBody[num].transform.localPosition.z + 0.01f);
		}
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x00035324 File Offset: 0x00033524
	public void EngReleaseStand()
	{
		GameObject gameObject = GameObject.Find("engineblock");
		bool flag = false;
		if (gameObject != null && gameObject.GetComponent<FixedJoint>().connectedBody.name == "DK_9")
		{
			gameObject.GetComponent<PickUp>().pickable = true;
			flag = true;
		}
		if (!flag)
		{
			gameObject = GameObject.Find("v8_block");
			if (gameObject.GetComponent<FixedJoint>().connectedBody.name == "DK_9")
			{
				gameObject.GetComponent<PickUp>().pickable = true;
				return;
			}
			gameObject = GameObject.Find("i6block");
			if (gameObject.GetComponent<FixedJoint>().connectedBody.name == "DK_9")
			{
				gameObject.GetComponent<PickUp>().pickable = true;
			}
		}
	}

	// Token: 0x06000501 RID: 1281 RVA: 0x000353DB File Offset: 0x000335DB
	public void ThrowObject()
	{
		this.pickedUpObject.GetComponent<PickUp>().LetGo(1);
		this.pickedUpObject = null;
	}

	// Token: 0x06000502 RID: 1282 RVA: 0x000353F5 File Offset: 0x000335F5
	public void DropObject()
	{
		if (this.pickedUpObject != null)
		{
			this.pickedUpObject.GetComponent<PickUp>().LetGo(0);
			this.pickedUpObject = null;
		}
	}

	// Token: 0x06000503 RID: 1283 RVA: 0x00035420 File Offset: 0x00033620
	public void AdvanceTime()
	{
		this.raceActive = false;
		this.checkpoints = 0;
		this.racenpc.GetComponent<RaceNpc>().status = 1;
		this.racenpc.GetComponent<RaceNpc>().ResetTruck();
		this.mail.RefreshEmail();
		this.carscript.temperature = 0f;
		this.carscript.steamTrailsOn = true;
		this.carscript.steamTrailsOn2 = false;
		if (this.fireman.GetComponent<Fireman>().missionNum == 2)
		{
			this.fireman.GetComponent<Fireman>().missionNum = 3;
		}
		else if (this.fireman.GetComponent<Fireman>().missionNum == 5)
		{
			this.fireman.GetComponent<Fireman>().missionNum = 6;
		}
		if (this.eventSystem.GetComponent<MissionController>().activeMissions.Contains(2))
		{
			this.eventSystem.GetComponent<MissionController>().CompleteMission(2);
			this.mail.receivedEmails.Add(2);
			this.mail.receivedEmails.Add(3);
			this.mail.receivedEmails.Add(4);
			this.mail.receivedEmails.Add(8);
			this.mail.receivedEmails.Add(11);
			this.mail.RefreshEmail();
			this.fireman.SetActive(true);
			this.eventSystem.GetComponent<MissionController>().ActivateMission(46);
		}
		if (this.eventSystem.GetComponent<MissionController>().completedMissions.Contains(15) && !this.mail.receivedEmails.Contains(9))
		{
			this.mail.receivedEmails.Add(9);
		}
		if (this.eventSystem.GetComponent<MissionController>().completedMissions.Contains(41) && !this.mail.receivedEmails.Contains(10))
		{
			this.mail.receivedEmails.Add(10);
		}
		this.sleepScript.SleepNow();
		this.currTime = EnviroSkyMgr.instance.GetTimeOfDay();
		this.currDay = EnviroSkyMgr.instance.GetCurrentDay();
		this.currDay++;
		this.newTime = 6.5f;
		float num = 100f;
		if (this.colliderName == "NiceBed")
		{
			this.newTime = 6.5f;
			num = 100f;
			this.currency.stress = 100f;
		}
		else if (this.colliderName == "JailBed")
		{
			this.newTime = Random.Range(4f, 9f);
			num = 40f;
			this.officer.jailTimer = this.officer.jailInterval;
			this.officer.raidWarrant = false;
		}
		else
		{
			num = 99f;
			if (this.currency.stress < 50f)
			{
				this.newTime = Random.Range(4f, 9f);
				num = 90f;
			}
			if (this.currency.stress < 25f)
			{
				this.newTime = Random.Range(2f, 13f);
				num = 80f;
			}
		}
		this.currency.hunger -= 25f;
		if (this.currency.hunger < 0f)
		{
			this.currency.hunger = 0f;
		}
		this.currency.sleep = num;
		this.currency.water -= 20f * ((float)this.currency.drunk + 1f);
		if (this.currency.water < 0f)
		{
			this.currency.water = 1f;
		}
		this.currency.drunk = 0;
		this.currency.tired = false;
		this.bench.UpdatePlates();
		this.currency.WakeUp();
		EnviroSkyMgr.instance.SetTimeOfDay(this.newTime);
		EnviroSkyMgr.instance.SetDays(this.currDay);
		EnviroSkyMgr.instance.ResetHourEventTimer();
		if (this.eventSystem.GetComponent<BuyTruck>().buyTruckNum == 1)
		{
			this.eventSystem.GetComponent<BuyTruck>().SpawnTruck(1);
		}
		else if (this.eventSystem.GetComponent<BuyTruck>().buyTruckNum == 2)
		{
			this.eventSystem.GetComponent<BuyTruck>().SpawnTruck(2);
		}
		foreach (object obj in this.restockableItems.transform)
		{
			Transform transform = (Transform)obj;
			transform.gameObject.SetActive(true);
			if (transform.gameObject.name == "sugar_shelf")
			{
				transform.gameObject.GetComponent<Renderer>().enabled = true;
			}
		}
		foreach (object obj2 in this.restockableBerries.transform)
		{
			((Transform)obj2).gameObject.SetActive(true);
		}
		if ((EnviroSky.instance.internalHour >= 16f && EnviroSky.instance.internalHour < 24f) || (EnviroSky.instance.internalHour >= 0f && EnviroSky.instance.internalHour < 7f))
		{
			this.lightscript.TurnOn();
		}
		else if (EnviroSky.instance.internalHour >= 7f && EnviroSky.instance.internalHour < 16f)
		{
			this.lightscript.TurnOff();
		}
		if (!this.officer.raidWarrant)
		{
			if (this.officer.gameObject.activeInHierarchy)
			{
				this.officer.ResetDay();
			}
		}
		else if (num == 100f)
		{
			this.officer.Raid(2);
		}
		else
		{
			this.officer.Raid(1);
		}
		this.modGirl.GetComponent<ModWomanJobs>().SpawnNextItem();
		this.bar.Start();
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x00035A20 File Offset: 0x00033C20
	public void RgbAdjust(int whichRgb)
	{
		this.fpc.UnlockMouse();
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		base.GetComponent<FirstPersonController>().enabled = false;
		this.rgbPanel.SetActive(true);
		this.rgbPanel.GetComponent<RgbChange>().which = whichRgb;
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x00035A6D File Offset: 0x00033C6D
	public void RgbAdjustPaint()
	{
		this.fpc.UnlockMouse();
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		base.GetComponent<FirstPersonController>().enabled = false;
		this.rgbPanelPaint.SetActive(true);
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x00035A9E File Offset: 0x00033C9E
	public void JohnnyInteract()
	{
		this.johnny1.GetComponent<JohnnyBuy>().JohnnyInteract();
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x00035AB0 File Offset: 0x00033CB0
	public void BuyTruck(int truckNum)
	{
		if (this.eventSystem.GetComponent<BuyTruck>().buyTruckNum == 0 && !this.johnny1.GetComponent<JohnnyBuy>().IsBusy() && this.currency.money > 399f)
		{
			this.eventSystem.GetComponent<MissionController>().CompleteMission(0);
			this.eventSystem.GetComponent<MissionController>().CompleteMission(1);
			this.eventSystem.GetComponent<MissionController>().ActivateMission(2);
			this.eventSystem.GetComponent<MissionController>().TrackMission_N(2);
			this.eventSystem.GetComponent<BuyTruck>().buyTruckNum = truckNum;
			this.johnny1.GetComponent<JohnnyBuy>().dialogueNum = 3;
			this.johnny1.GetComponent<JohnnyBuy>().JohnnyInteract2();
			this.currency.money -= 400f;
			this.inv.SubtractMoney(400f);
		}
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x00035B99 File Offset: 0x00033D99
	public void RecycleJunk()
	{
		Debug.Log("rec");
		this.recycleZone.GetComponent<recycler>().CheckRecycleTotal();
		this.eventSystem.GetComponent<MissionController>().CompleteMission(3);
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x00035BC6 File Offset: 0x00033DC6
	public void RecycleTrash()
	{
		this.recycleZoneTrash.GetComponent<recyclertrash>().CheckRecycleTotal();
	}

	// Token: 0x0600050A RID: 1290 RVA: 0x00035BD8 File Offset: 0x00033DD8
	public void SellWood()
	{
		this.recycleZoneWood.GetComponent<recyclerw>().CheckRecycleTotal();
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x00035BEA File Offset: 0x00033DEA
	public void SellOre()
	{
		this.recycleZoneOre.GetComponent<Recyclero>().CheckRecycleTotal();
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x00035BFC File Offset: 0x00033DFC
	public void HitchTrailerG()
	{
		if (this.hitchG.GetComponent<InteractiveObject>().isOpen)
		{
			float num = Vector3.Distance(this.hitchG.transform.position, this.hitchPoint.transform.position);
			float num2 = Vector3.Distance(this.hitchG.transform.position, this.hitchPointF.transform.position);
			if (num < 2f || num2 < 2f)
			{
				foreach (WheelCollider wheelCollider in this.trailerGen.GetComponentsInChildren<WheelCollider>())
				{
					wheelCollider.motorTorque = 2f;
					wheelCollider.brakeTorque = 0f;
				}
				this.trailerGen.GetComponent<Rigidbody>().mass = 25f;
				iTween.RotateTo(this.hitchLinkG, iTween.Hash(new object[]
				{
					"x",
					-5f,
					"delay",
					0.1f,
					"time",
					0.1f,
					"easeType",
					"linear"
				}));
				this.iTweenArgs = iTween.Hash(Array.Empty<object>());
				int whichBall = 1;
				if (num < 2f)
				{
					this.iTweenArgs.Add("position", this.hitchPoint.transform.position);
				}
				else if (num2 < 2f)
				{
					this.iTweenArgs.Add("position", this.hitchPointF.transform.position);
					whichBall = 2;
				}
				this.iTweenArgs.Add("time", 0.6f);
				this.iTweenArgs.Add("islocal", false);
				iTween.MoveTo(this.hitchLinkG, this.iTweenArgs);
				this.hitchG.GetComponent<InteractiveObject>().isOpen = false;
				base.StartCoroutine(this.HitchTrailerJointG(whichBall));
				return;
			}
		}
		else
		{
			this.hitchG.GetComponent<InteractiveObject>().isOpen = true;
			ConfigurableJoint component = this.trailerGen.GetComponent<ConfigurableJoint>();
			component.connectedBody = null;
			component.xMotion = ConfigurableJointMotion.Free;
			component.yMotion = ConfigurableJointMotion.Free;
			component.zMotion = ConfigurableJointMotion.Free;
			foreach (WheelCollider wheelCollider2 in this.trailerGen.GetComponentsInChildren<WheelCollider>())
			{
				wheelCollider2.motorTorque = 1E-05f;
				wheelCollider2.brakeTorque = 10f;
			}
		}
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x00035E70 File Offset: 0x00034070
	public void HitchTrailer()
	{
		if (this.hitch.GetComponent<InteractiveObject>().isOpen)
		{
			float num = Vector3.Distance(this.hitch.transform.position, this.hitchPoint.transform.position);
			float num2 = Vector3.Distance(this.hitch.transform.position, this.hitchPointF.transform.position);
			if (num < 2f || num2 < 2f)
			{
				foreach (WheelCollider wheelCollider in this.trailer.GetComponentsInChildren<WheelCollider>())
				{
					wheelCollider.motorTorque = 2f;
					wheelCollider.brakeTorque = 0f;
				}
				this.trailer.GetComponent<Rigidbody>().mass = 25f;
				iTween.RotateTo(this.hitchLink, iTween.Hash(new object[]
				{
					"x",
					-5f,
					"delay",
					0.1f,
					"time",
					0.1f,
					"easeType",
					"linear"
				}));
				this.iTweenArgs = iTween.Hash(Array.Empty<object>());
				int whichBall = 1;
				if (num < 2f)
				{
					this.iTweenArgs.Add("position", this.hitchPoint.transform.position);
				}
				else if (num2 < 2f)
				{
					this.iTweenArgs.Add("position", this.hitchPointF.transform.position);
					whichBall = 2;
				}
				this.iTweenArgs.Add("time", 0.6f);
				this.iTweenArgs.Add("islocal", false);
				iTween.MoveTo(this.hitchLink, this.iTweenArgs);
				this.hitch.GetComponent<InteractiveObject>().isOpen = false;
				base.StartCoroutine(this.HitchTrailerJoint(whichBall));
				return;
			}
		}
		else
		{
			this.hitch.GetComponent<InteractiveObject>().isOpen = true;
			ConfigurableJoint component = this.trailer.GetComponent<ConfigurableJoint>();
			component.connectedBody = null;
			component.xMotion = ConfigurableJointMotion.Free;
			component.yMotion = ConfigurableJointMotion.Free;
			component.zMotion = ConfigurableJointMotion.Free;
			foreach (WheelCollider wheelCollider2 in this.trailer.GetComponentsInChildren<WheelCollider>())
			{
				wheelCollider2.motorTorque = 1E-05f;
				wheelCollider2.brakeTorque = 10f;
			}
		}
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x000360E1 File Offset: 0x000342E1
	private IEnumerator HitchTrailerJoint(int whichBall)
	{
		yield return new WaitForSeconds(0.6f);
		this.trailer.GetComponent<Rigidbody>().mass = 300f;
		ConfigurableJoint component = this.trailer.GetComponent<ConfigurableJoint>();
		if (whichBall == 1)
		{
			component.connectedBody = this.truck.GetComponent<Rigidbody>();
		}
		else if (whichBall == 2)
		{
			component.connectedBody = this.truck2.GetComponent<Rigidbody>();
		}
		component.xMotion = ConfigurableJointMotion.Locked;
		component.yMotion = ConfigurableJointMotion.Locked;
		component.zMotion = ConfigurableJointMotion.Locked;
		yield break;
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x000360F7 File Offset: 0x000342F7
	private IEnumerator HitchTrailerJointG(int whichBall)
	{
		yield return new WaitForSeconds(0.6f);
		this.trailerGen.GetComponent<Rigidbody>().mass = 300f;
		ConfigurableJoint component = this.trailerGen.GetComponent<ConfigurableJoint>();
		if (whichBall == 1)
		{
			component.connectedBody = this.truck.GetComponent<Rigidbody>();
		}
		else if (whichBall == 2)
		{
			component.connectedBody = this.truck2.GetComponent<Rigidbody>();
		}
		component.xMotion = ConfigurableJointMotion.Locked;
		component.yMotion = ConfigurableJointMotion.Locked;
		component.zMotion = ConfigurableJointMotion.Locked;
		yield break;
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x00036110 File Offset: 0x00034310
	public void HitchTrailerC()
	{
		if (this.hitchC.GetComponent<InteractiveObject>().isOpen)
		{
			float num = Vector3.Distance(this.hitchC.transform.position, this.hitchPoint.transform.position);
			float num2 = Vector3.Distance(this.hitchC.transform.position, this.hitchPointF.transform.position);
			if (num < 2f || num2 < 2f)
			{
				this.trailerBedC.thirdWheel.SetActive(true);
				foreach (WheelCollider wheelCollider in this.trailerC.GetComponentsInChildren<WheelCollider>())
				{
					wheelCollider.motorTorque = 2f;
					wheelCollider.brakeTorque = 0f;
				}
				this.trailerC.GetComponent<Rigidbody>().mass = 25f;
				iTween.RotateTo(this.hitchLinkC, iTween.Hash(new object[]
				{
					"x",
					-5f,
					"delay",
					0.1f,
					"time",
					0.1f,
					"easeType",
					"linear"
				}));
				this.iTweenArgs = iTween.Hash(Array.Empty<object>());
				this.whichHitch = 1;
				if (num < 2f)
				{
					this.iTweenArgs.Add("position", this.hitchPoint.transform.position);
				}
				else if (num2 < 2f)
				{
					this.iTweenArgs.Add("position", this.hitchPointF.transform.position);
					this.whichHitch = 2;
				}
				this.iTweenArgs.Add("time", 3f);
				this.iTweenArgs.Add("islocal", false);
				iTween.MoveTo(this.hitchLinkC, this.iTweenArgs);
				this.hitchC.GetComponent<InteractiveObject>().isOpen = false;
				base.StartCoroutine(this.HitchTrailerJointC(this.whichHitch));
				return;
			}
		}
		else
		{
			this.trailerBedC.thirdWheel.SetActive(true);
			this.hitchC.GetComponent<InteractiveObject>().isOpen = true;
			this.hitchLinkC.GetComponent<ConfigurableJoint>().connectedBody = this.trailerC.GetComponent<Rigidbody>();
			ConfigurableJoint component = this.trailerC.GetComponent<ConfigurableJoint>();
			component.connectedBody = null;
			component.xMotion = ConfigurableJointMotion.Free;
			component.yMotion = ConfigurableJointMotion.Free;
			component.zMotion = ConfigurableJointMotion.Free;
			foreach (WheelCollider wheelCollider2 in this.trailerC.GetComponentsInChildren<WheelCollider>())
			{
				wheelCollider2.motorTorque = 1E-05f;
				wheelCollider2.brakeTorque = 10f;
			}
			this.trailerC.transform.GetChild(this.trailerC.transform.childCount - 1).GetComponent<InteractiveObject>().PerformAction();
			this.trailerC.transform.GetChild(this.trailerC.transform.childCount - 1).GetComponent<BoxCollider>().enabled = true;
		}
	}

	// Token: 0x06000511 RID: 1297 RVA: 0x0003641C File Offset: 0x0003461C
	private IEnumerator HitchTrailerJointC(int whichBall)
	{
		yield return new WaitForSeconds(3f);
		this.trailerC.GetComponent<Rigidbody>().mass = 300f;
		ConfigurableJoint component = this.trailerC.GetComponent<ConfigurableJoint>();
		if (whichBall == 1)
		{
			component.connectedBody = this.truck.GetComponent<Rigidbody>();
		}
		else if (whichBall == 2)
		{
			component.connectedBody = this.truck2.GetComponent<Rigidbody>();
		}
		component.xMotion = ConfigurableJointMotion.Locked;
		component.yMotion = ConfigurableJointMotion.Locked;
		component.zMotion = ConfigurableJointMotion.Locked;
		this.trailerBedC.thirdWheel.SetActive(false);
		this.trailerC.transform.GetChild(this.trailerC.transform.childCount - 1).GetComponent<InteractiveObject>().PerformAction();
		this.trailerC.transform.GetChild(this.trailerC.transform.childCount - 1).GetComponent<BoxCollider>().enabled = false;
		yield break;
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x00036434 File Offset: 0x00034634
	public void HitchTrailerH()
	{
		if (this.hitchH.GetComponent<InteractiveObject>().isOpen)
		{
			float num = Vector3.Distance(this.hitchH.transform.position, this.hitchPoint.transform.position);
			float num2 = Vector3.Distance(this.hitchH.transform.position, this.hitchPointF.transform.position);
			if (num < 2f || num2 < 2f)
			{
				this.trailerBedH.thirdWheel.SetActive(true);
				foreach (WheelCollider wheelCollider in this.trailerH.GetComponentsInChildren<WheelCollider>())
				{
					wheelCollider.motorTorque = 0f;
					wheelCollider.brakeTorque = 0f;
				}
				this.trailerH.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
				this.trailerH.GetComponent<Rigidbody>().mass = 25f;
				this.whichHitch = 1;
				if (num >= 2f && num2 < 2f)
				{
					this.whichHitch = 2;
				}
				this.hitchH.GetComponent<InteractiveObject>().isOpen = false;
				base.StartCoroutine(this.HitchTrailerJointH(this.whichHitch));
				this.moveTrailer = true;
				return;
			}
		}
		else
		{
			this.trailerBedH.thirdWheel.SetActive(true);
			this.hitchH.GetComponent<InteractiveObject>().isOpen = true;
			this.hitchLinkH.GetComponent<ConfigurableJoint>().connectedBody = this.trailerH.GetComponent<Rigidbody>();
			ConfigurableJoint component = this.trailerH.GetComponent<ConfigurableJoint>();
			component.connectedBody = null;
			component.xMotion = ConfigurableJointMotion.Free;
			component.yMotion = ConfigurableJointMotion.Free;
			component.zMotion = ConfigurableJointMotion.Free;
			foreach (WheelCollider wheelCollider2 in this.trailerH.GetComponentsInChildren<WheelCollider>())
			{
				wheelCollider2.motorTorque = 1E-05f;
				wheelCollider2.brakeTorque = 10f;
			}
			this.trailerH.transform.GetChild(this.trailerH.transform.childCount - 1).GetComponent<InteractiveObject>().PerformAction();
			this.trailerH.transform.GetChild(this.trailerH.transform.childCount - 1).GetComponent<BoxCollider>().enabled = true;
		}
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x00036656 File Offset: 0x00034856
	private IEnumerator HitchTrailerJointH(int whichBall)
	{
		yield return new WaitForSeconds(3f);
		this.moveTrailer = false;
		this.trailerH.GetComponent<Rigidbody>().mass = 300f;
		ConfigurableJoint component = this.trailerH.GetComponent<ConfigurableJoint>();
		if (whichBall == 1)
		{
			component.connectedBody = this.truck.GetComponent<Rigidbody>();
		}
		else if (whichBall == 2)
		{
			component.connectedBody = this.truck2.GetComponent<Rigidbody>();
		}
		component.xMotion = ConfigurableJointMotion.Locked;
		component.yMotion = ConfigurableJointMotion.Locked;
		component.zMotion = ConfigurableJointMotion.Locked;
		this.trailerBedH.thirdWheel.SetActive(false);
		this.trailerH.transform.GetChild(this.trailerH.transform.childCount - 1).GetComponent<InteractiveObject>().PerformAction();
		this.trailerH.transform.GetChild(this.trailerH.transform.childCount - 1).GetComponent<BoxCollider>().enabled = false;
		if (this.whichHitch == 1)
		{
			if (Vector3.Distance(this.hitchH.transform.position, this.hitchPoint.transform.position) > 2f)
			{
				this.HitchTrailerH();
			}
		}
		else if (this.whichHitch == 2 && Vector3.Distance(this.hitchH.transform.position, this.hitchPointF.transform.position) > 2f)
		{
			this.HitchTrailerH();
		}
		yield break;
	}

	// Token: 0x06000514 RID: 1300 RVA: 0x0003666C File Offset: 0x0003486C
	public void PaintSurface()
	{
		if (this.hit.collider.gameObject.name.Contains("spraypaint"))
		{
			float r = this.hit.collider.gameObject.GetComponent<Renderer>().material.color.r;
			float g = this.hit.collider.gameObject.GetComponent<Renderer>().material.color.g;
			float b = this.hit.collider.gameObject.GetComponent<Renderer>().material.color.b;
			float @float = this.hit.collider.gameObject.GetComponent<Renderer>().material.GetFloat("_Metallic");
			float float2 = this.hit.collider.gameObject.GetComponent<Renderer>().material.GetFloat("_Glossiness");
			GameObject gameObject = this.hit.collider.gameObject;
			gameObject.GetComponent<BoxCollider>().enabled = false;
			Physics.Raycast(this.cam.transform.position, this.cam.transform.forward, out this.hitp, this.interactRange);
			gameObject.GetComponent<BoxCollider>().enabled = true;
			string name = this.hitp.collider.gameObject.name;
			AssignMaterial component = this.truck.GetComponent<AssignMaterial>();
			if (name == "cabin_low" || name == "hood_low1" || name == "cardoor_d" || name == "cardoor_p" || name == "tailgate" || name == "cap_low")
			{
				if (this.hitp.collider.gameObject.transform.parent.gameObject.name != "amc")
				{
					this.hitp.collider.gameObject.GetComponent<Renderer>().material.color = new UnityEngine.Color(r, g, b, 1f);
					this.hitp.collider.gameObject.GetComponent<Renderer>().material.SetFloat("_Metallic", @float);
					this.hitp.collider.gameObject.GetComponent<Renderer>().material.SetFloat("_Smoothness", float2);
					this.paSources.PlayOneShot(this.playerAudio[7], 1f);
					if (name == "cabin_low")
					{
						component.cabinR = r;
						component.cabinG = g;
						component.cabinB = b;
						component.cabinM = @float;
						component.cabinS = float2;
						return;
					}
					if (name == "hood_low1")
					{
						component.hoodR = r;
						component.hoodG = g;
						component.hoodB = b;
						component.hoodM = @float;
						component.hoodS = float2;
						return;
					}
					if (name == "cardoor_d")
					{
						component.doordR = r;
						component.doordG = g;
						component.doordB = b;
						component.doordM = @float;
						component.doordS = float2;
						return;
					}
					if (name == "cardoor_p")
					{
						component.doorpR = r;
						component.doorpG = g;
						component.doorpB = b;
						component.doorpM = @float;
						component.doorpS = float2;
						return;
					}
					if (name == "tailgate")
					{
						component.tailgateR = r;
						component.tailgateG = g;
						component.tailgateB = b;
						component.tailgateM = @float;
						component.tailgateS = float2;
						return;
					}
					if (name == "cap_low")
					{
						component.gasdoorR = r;
						component.gasdoorG = g;
						component.gasdoorB = b;
						component.gasdoorM = @float;
						component.gasdoorS = float2;
						return;
					}
				}
			}
			else
			{
				if (name == "body_collider.003" || name == "body_collider.007")
				{
					Material material = GameObject.Find("body_low").GetComponent<Renderer>().material;
					material.color = new UnityEngine.Color(r, g, b, 1f);
					material.SetFloat("_Metallic", @float);
					material.SetFloat("_Smoothness", float2);
					this.paSources.PlayOneShot(this.playerAudio[7], 1f);
					component.bodyR = r;
					component.bodyG = g;
					component.bodyB = b;
					component.bodyM = @float;
					component.bodyS = float2;
					return;
				}
				if (name == "DirtBike")
				{
					Material material2 = GameObject.Find("250Body").GetComponent<Renderer>().material;
					Material material3 = GameObject.Find("250Handles").GetComponent<Renderer>().material;
					Material material4 = GameObject.Find("250FrontSuspensionMesh").GetComponent<Renderer>().material;
					material2.color = new UnityEngine.Color(r, g, b, 1f);
					material3.color = new UnityEngine.Color(r, g, b, 1f);
					material4.color = new UnityEngine.Color(r, g, b, 1f);
					material2.SetFloat("_Metallic", @float);
					material3.SetFloat("_Metallic", @float);
					material4.SetFloat("_Metallic", @float);
					material2.SetFloat("_Smoothness", float2);
					material3.SetFloat("_Smoothness", float2);
					material4.SetFloat("_Smoothness", float2);
					component.motoR = r;
					component.motoG = g;
					component.motoB = b;
					component.motoM = @float;
					component.motoS = float2;
					this.paSources.PlayOneShot(this.playerAudio[7], 1f);
					return;
				}
				if (name == "fmainbody" || name == "ftailgate" || name == "finterior" || name == "f100_ddoor" || name == "f100_pdoor")
				{
					Material[] materials = this.hitp.collider.gameObject.GetComponent<Renderer>().materials;
					materials[1].color = new UnityEngine.Color(r, g, b, 1f);
					materials[1].SetFloat("_Metallic", @float);
					materials[1].SetFloat("_Glossiness", float2);
					this.paSources.PlayOneShot(this.playerAudio[7], 1f);
					if (name == "fmainbody")
					{
						component.bodyFR = r;
						component.bodyFG = g;
						component.bodyFB = b;
						component.bodyFM = @float;
						component.bodyFS = float2;
						component.painted5 = true;
						component.RemoveRust(5);
						return;
					}
					if (name == "ftailgate")
					{
						component.tailgateFR = r;
						component.tailgateFG = g;
						component.tailgateFB = b;
						component.tailgateFM = @float;
						component.tailgateFS = float2;
						component.painted1 = true;
						component.RemoveRust(1);
						return;
					}
					if (name == "finterior")
					{
						component.cabinFR = r;
						component.cabinFG = g;
						component.cabinFB = b;
						component.cabinFM = @float;
						component.cabinFS = float2;
						return;
					}
					if (name == "f100_ddoor")
					{
						component.doordFR = r;
						component.doordFG = g;
						component.doordFB = b;
						component.doordFM = @float;
						component.doordFS = float2;
						component.painted3 = true;
						component.RemoveRust(3);
						return;
					}
					if (name == "f100_pdoor")
					{
						component.doorpFR = r;
						component.doorpFG = g;
						component.doorpFB = b;
						component.doorpFM = @float;
						component.doorpFS = float2;
						component.painted2 = true;
						component.RemoveRust(2);
						return;
					}
				}
				else
				{
					if (name == "fhood")
					{
						Material[] materials2 = this.hitp.collider.gameObject.GetComponent<Renderer>().materials;
						materials2[0].color = new UnityEngine.Color(r, g, b, 1f);
						materials2[0].SetFloat("_Metallic", @float);
						materials2[0].SetFloat("_Glossiness", float2);
						component.hoodFR = r;
						component.hoodFG = g;
						component.hoodFB = b;
						component.hoodFM = @float;
						component.hoodFS = float2;
						component.painted4 = true;
						component.RemoveRust(4);
						this.paSources.PlayOneShot(this.playerAudio[7], 1f);
						return;
					}
					if (name.Contains("v8_block"))
					{
						Material[] materials3 = this.hitp.collider.gameObject.GetComponent<Renderer>().materials;
						materials3[1].color = new UnityEngine.Color(r, g, b, 1f);
						materials3[1].SetFloat("_Metallic", @float);
						materials3[1].SetFloat("_Glossiness", float2);
						component.v8blockR = r;
						component.v8blockG = g;
						component.v8blockB = b;
						component.v8blockM = @float;
						component.v8blockS = float2;
						durability component2 = this.hitp.collider.gameObject.GetComponent<durability>();
						component2.painted = true;
						component2.red = r;
						component2.green = g;
						component2.blue = b;
						component2.metallic = @float;
						component2.smoothness = float2;
						this.paSources.PlayOneShot(this.playerAudio[7], 1f);
						return;
					}
					if (name.Contains("valvecover_e") || name.Contains("v8_valvecoverD_e") || name.Contains("v8_valvecoverP_e") || name.Contains("v8_intakeman_e") || name.Contains("v8_timingcover_e") || name.Contains("v8_headD_e") || name.Contains("v8_headP_e"))
					{
						durability component3 = this.hitp.collider.gameObject.GetComponent<durability>();
						Material[] materials4 = this.hitp.collider.gameObject.GetComponent<Renderer>().materials;
						int paintSlot = this.hitp.collider.gameObject.GetComponent<durability>().paintSlot;
						materials4[paintSlot].color = new UnityEngine.Color(r, g, b, 1f);
						materials4[paintSlot].SetFloat("_Metallic", @float);
						materials4[paintSlot].SetFloat("_Glossiness", float2);
						component3.painted = true;
						component3.red = r;
						component3.green = g;
						component3.blue = b;
						component3.metallic = @float;
						component3.smoothness = float2;
						this.paSources.PlayOneShot(this.playerAudio[7], 1f);
						return;
					}
					if (name.Contains("valvecover") || name.Contains("v8_valvecoverD") || name.Contains("v8_valvecoverP") || name.Contains("v8_intakeman") || name.Contains("v8_timingcover") || name.Contains("v8_headD") || name.Contains("v8_headP"))
					{
						if (this.hitp.collider.gameObject.GetComponent<PickUp>() == null)
						{
							return;
						}
						PickUp component4 = this.hitp.collider.gameObject.GetComponent<PickUp>();
						int paintSlot2 = component4.paintSlot;
						Material[] materials5 = this.hitp.collider.gameObject.GetComponent<Renderer>().materials;
						materials5[paintSlot2].color = new UnityEngine.Color(r, g, b, 1f);
						materials5[paintSlot2].SetFloat("_Metallic", @float);
						materials5[paintSlot2].SetFloat("_Glossiness", float2);
						component4.painted = true;
						component4.red = r;
						component4.green = g;
						component4.blue = b;
						component4.metallic = @float;
						component4.smoothness = float2;
						this.paSources.PlayOneShot(this.playerAudio[7], 1f);
					}
				}
			}
		}
	}

	// Token: 0x06000515 RID: 1301 RVA: 0x000371E5 File Offset: 0x000353E5
	public void Heartbeat()
	{
		this.paSources.PlayOneShot(this.playerAudio[2], 1f);
	}

	// Token: 0x06000516 RID: 1302 RVA: 0x00037200 File Offset: 0x00035400
	public void Chew()
	{
		if (Time.time >= this.beerTimeout && this.hit.collider.gameObject.GetComponent<PickUp>().pickable && this.hit.collider.gameObject.GetComponent<PickUp>().thisDurability > 0f)
		{
			this.beerTimeout = Time.time + 4.6f;
			this.hit.collider.gameObject.GetComponent<PickUp>().thisDurability -= 1f;
			this.currency.nicotine += 30f;
			this.fpsHand2.SetActive(true);
			this.fpsHand2.GetComponent<Animator>().Play("hand_zen");
			this.paSources.PlayOneShot(this.playerAudio[12], 1f);
			float num = this.currency.water - 5f;
			if (num < 0f)
			{
				num = 0f;
			}
			float nicotine = this.currency.nicotine + 30f;
			if (num < 0f)
			{
				num = 0f;
			}
			float num2 = this.currency.stress + 10f;
			if (num2 > 100f)
			{
				num2 = 100f;
			}
			this.currency.water = num;
			this.currency.stress = num2;
			this.currency.nicotine = nicotine;
			if (this.hit.collider.gameObject.GetComponent<PickUp>().thisDurability == 0f)
			{
				this.hit.collider.gameObject.GetComponent<PickUp>().description = "Empty Zen Can";
			}
		}
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x000373AC File Offset: 0x000355AC
	public void DrinkBeer()
	{
		if (Time.time >= this.beerTimeout && this.hit.collider.gameObject.GetComponent<PickUp>().pickable && this.hit.collider.gameObject.GetComponent<PickUp>().thisDurability > 0f)
		{
			this.beerTimeout = Time.time + 4.6f;
			this.hit.collider.gameObject.GetComponent<PickUp>().thisDurability -= 1f;
			this.currency.drunk += 3;
			if (this.currency.drunk > 5)
			{
				Camera.main.GetComponent<MotionBlur>().enabled = true;
			}
			if (this.currency.drunk > 14)
			{
				Camera.main.GetComponent<BlurOptimized>().enabled = true;
			}
			if (this.currency.drunk > 23)
			{
				Camera.main.GetComponent<Fisheye>().enabled = true;
			}
			this.fpsHand2.SetActive(true);
			int num = Random.Range(0, 4);
			if (this.currency.drunk > 5 && num == 0)
			{
				this.fpsHand2.GetComponent<Animator>().Play("hand_beercrushed");
				base.StartCoroutine(this.SpawnCan(6));
			}
			else
			{
				this.fpsHand2.GetComponent<Animator>().Play("hand_beer1");
				base.StartCoroutine(this.SpawnCan(1));
			}
			this.ranNum = Random.Range(10, 12);
			this.paSources.PlayOneShot(this.playerAudio[this.ranNum], 1f);
			float num2 = this.currency.water + 20f;
			if (num2 > 100f)
			{
				num2 = 100f;
			}
			float num3 = this.currency.stress + 30f;
			if (num3 > 100f)
			{
				num3 = 100f;
			}
			this.currency.water = num2;
			this.currency.stress = num3;
			if (this.hit.collider.gameObject.GetComponent<PickUp>().thisDurability == 0f)
			{
				this.hit.collider.gameObject.GetComponent<PickUp>().description = "Empty 12 Pack";
			}
			if (this.hit.collider.gameObject.GetComponent<PickUp>().thisDurability == 6f)
			{
				Debug.Log("sixbeer");
				Achievement achievement = new Achievement("ACH_HALF");
				achievement.Trigger(true);
			}
		}
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x00037624 File Offset: 0x00035824
	public void DrinkBeer2()
	{
		if (Time.time >= this.beerTimeout && this.hit.collider.gameObject.GetComponent<PickUp>().pickable && this.hit.collider.gameObject.GetComponent<Beercase>().bottles > 0)
		{
			this.beerTimeout = Time.time + 4.6f;
			this.hit.collider.gameObject.GetComponent<Beercase>().bottles--;
			this.currency.drunk += 3;
			if (this.currency.drunk > 5)
			{
				Camera.main.GetComponent<MotionBlur>().enabled = true;
			}
			if (this.currency.drunk > 14)
			{
				Camera.main.GetComponent<BlurOptimized>().enabled = true;
			}
			if (this.currency.drunk > 23)
			{
				Camera.main.GetComponent<Fisheye>().enabled = true;
			}
			this.fpsHand2.SetActive(true);
			this.hit.collider.gameObject.GetComponent<Beercase>().Start();
			int num = Random.Range(0, 4);
			if (this.currency.drunk > 5 && num < 2)
			{
				if (num == 0)
				{
					this.fpsHand2.GetComponent<Animator>().Play("hand_beer3");
					base.StartCoroutine(this.SpawnCan(7));
				}
				else
				{
					this.fpsHand2.GetComponent<Animator>().Play("hand_beer4");
					base.StartCoroutine(this.Carbonation(4f));
					base.StartCoroutine(this.SpawnCan(8));
				}
			}
			else
			{
				this.fpsHand2.GetComponent<Animator>().Play("hand_beer2");
				base.StartCoroutine(this.SpawnCan(2));
			}
			this.paSources.PlayOneShot(this.playerAudio[9], 1f);
			float num2 = this.currency.water + 40f;
			if (num2 > 100f)
			{
				num2 = 100f;
			}
			float num3 = this.currency.stress + 60f;
			if (num3 > 100f)
			{
				num3 = 100f;
			}
			this.currency.water = num2;
			this.currency.stress = num3;
			if (this.hit.collider.gameObject.GetComponent<Beercase>().bottles == 0)
			{
				this.hit.collider.gameObject.GetComponent<PickUp>().description = "Empty 12 Pack";
			}
		}
	}

	// Token: 0x06000519 RID: 1305 RVA: 0x00037890 File Offset: 0x00035A90
	private IEnumerator Carbonation(float carbdelay)
	{
		yield return new WaitForSeconds(carbdelay);
		this.carbonationParticle.Play();
		yield break;
	}

	// Token: 0x0600051A RID: 1306 RVA: 0x000378A6 File Offset: 0x00035AA6
	private IEnumerator SpawnCan(int canType)
	{
		yield return new WaitForSeconds(4f);
		GameObject spawnObj = this.emptyCan;
		if (canType == 1)
		{
			if (Random.Range(0, 3) == 0)
			{
				spawnObj = this.emptyCanCrushed;
			}
		}
		else if (canType == 2)
		{
			spawnObj = this.emptyBottle;
		}
		else if (canType == 3)
		{
			spawnObj = this.emptyCan2;
		}
		else if (canType == 4)
		{
			spawnObj = this.emptyCanB;
		}
		else if (canType == 5)
		{
			spawnObj = this.emptyCanC;
		}
		else if (canType == 6)
		{
			this.paSources.PlayOneShot(this.playerAudio[17], 1f);
			spawnObj = this.emptyCanCrushed2;
			yield return new WaitForSeconds(0.8f);
		}
		else if (canType == 7)
		{
			spawnObj = this.emptyBottle;
			yield return new WaitForSeconds(0.5f);
			Object.Instantiate<GameObject>(spawnObj, this.canPos2.transform.position, this.canPos2.transform.rotation);
		}
		else if (canType == 8)
		{
			this.paSources.PlayOneShot(this.playerAudio[18], 1f);
			spawnObj = this.bottleFrag;
			yield return new WaitForSeconds(1f);
		}
		if (canType != 7)
		{
			Object.Instantiate<GameObject>(spawnObj, this.canPos1.transform.position, this.canPos1.transform.rotation);
		}
		this.fpsHand2.SetActive(false);
		yield break;
	}

	// Token: 0x0600051B RID: 1307 RVA: 0x000378BC File Offset: 0x00035ABC
	public void RefreshCash()
	{
		base.StartCoroutine(this.CheckDelay());
	}

	// Token: 0x0600051C RID: 1308 RVA: 0x000378CB File Offset: 0x00035ACB
	private IEnumerator CheckDelay()
	{
		yield return new WaitForSeconds(0.1f);
		this.inv.OpenCheck();
		yield break;
	}

	// Token: 0x0600051D RID: 1309 RVA: 0x000378DA File Offset: 0x00035ADA
	public void AddItem(int itemNum)
	{
		this.gasStationTill.AddToList(itemNum);
	}

	// Token: 0x0600051E RID: 1310 RVA: 0x000378E8 File Offset: 0x00035AE8
	public void PayForItems()
	{
		this.gasStationTill.PayNow();
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x000378F5 File Offset: 0x00035AF5
	public void HarvestTobacco()
	{
		this.hit.collider.gameObject.GetComponent<PlantHealth>().Harvest();
	}

	// Token: 0x06000520 RID: 1312 RVA: 0x00037911 File Offset: 0x00035B11
	public void GrabHook()
	{
		this.fpsHook.GetComponent<Renderer>().enabled = true;
		this.winchHook.GetComponent<Renderer>().enabled = false;
		this.winchObject.GetComponent<Winch>().enabled = true;
	}

	// Token: 0x06000521 RID: 1313 RVA: 0x00037946 File Offset: 0x00035B46
	public void GrabHookF()
	{
		this.fpsHook.GetComponent<Renderer>().enabled = true;
		this.winchHookF.GetComponent<Renderer>().enabled = false;
		this.winchObjectF.GetComponent<Winch>().enabled = true;
	}

	// Token: 0x06000522 RID: 1314 RVA: 0x0003797B File Offset: 0x00035B7B
	public void StartChainsaw()
	{
		this.chainsawobj.GetComponent<chainsaw2>().StartSaw();
	}

	// Token: 0x06000523 RID: 1315 RVA: 0x0003798D File Offset: 0x00035B8D
	public IEnumerator RotateEngineStand()
	{
		HingeJoint hinge = this.EngStand.GetComponent<HingeJoint>();
		JointMotor motor = hinge.motor;
		motor.force = 10000f;
		motor.targetVelocity = 90f;
		motor.freeSpin = false;
		hinge.motor = motor;
		yield return new WaitForSeconds(1f);
		motor.targetVelocity = 0f;
		hinge.motor = motor;
		yield break;
	}

	// Token: 0x06000524 RID: 1316 RVA: 0x0003799C File Offset: 0x00035B9C
	private IEnumerator InteractPassDoor()
	{
		this.iaSources = this.hit.collider.gameObject.GetComponents<AudioSource>();
		HingeJoint hinged = this.hit.collider.gameObject.GetComponent<HingeJoint>();
		JointMotor motor = hinged.motor;
		Transform thisDoor = this.hit.collider.transform;
		Vector3 localEulerAngles = thisDoor.localEulerAngles;
		hinged.useMotor = true;
		if (motor.targetVelocity == 180f)
		{
			JointLimits limits = hinged.limits;
			limits.min = -60f;
			hinged.limits = limits;
			motor.targetVelocity = -180f;
			hinged.motor = motor;
			this.iaSources[1].Play();
		}
		else
		{
			motor.targetVelocity = 180f;
			hinged.motor = motor;
			yield return new WaitForSeconds(0.5f);
			localEulerAngles = thisDoor.localEulerAngles;
			if (localEulerAngles.y >= -3f && localEulerAngles.y < 5f)
			{
				this.iaSources[0].Play();
				JointLimits limits2 = hinged.limits;
				limits2.min = 0f;
				hinged.limits = limits2;
			}
		}
		yield break;
	}

	// Token: 0x06000525 RID: 1317 RVA: 0x000379AB File Offset: 0x00035BAB
	private float NormalizeAngle(float angle)
	{
		angle %= 360f;
		if (angle < 0f)
		{
			angle += 360f;
		}
		return angle;
	}

	// Token: 0x06000526 RID: 1318 RVA: 0x000379C8 File Offset: 0x00035BC8
	private IEnumerator InteractDrivDoor()
	{
		this.iaSourcesd = this.hit.collider.gameObject.GetComponents<AudioSource>();
		HingeJoint hinged = this.hit.collider.gameObject.GetComponent<HingeJoint>();
		JointMotor motord = hinged.motor;
		Transform thisDoor = this.hit.collider.transform;
		Vector3 localEulerAngles = thisDoor.localEulerAngles;
		hinged.useMotor = true;
		JointLimits limits = hinged.limits;
		int num = Random.Range(0, 6);
		if (this.currency.drunk > 4)
		{
			num = Random.Range(0, 3);
		}
		if (motord.targetVelocity <= -180f)
		{
			motord.targetVelocity = 180f;
			motord.force = 50f;
			if (base.transform.parent != null && num == 0)
			{
				if (base.transform.parent.gameObject.name == "DriverCameraController")
				{
					this.leg.GetComponent<LegKick>().KickOpen();
				}
				else if (base.transform.parent.gameObject.name == "f100seatmount")
				{
					this.legF.GetComponent<LegKick>().KickOpen();
				}
				else
				{
					this.legC.GetComponent<LegKick>().KickOpen();
				}
				motord.targetVelocity = 500f;
				motord.force = 100f;
			}
			limits.max = 64f;
			hinged.limits = limits;
			hinged.motor = motord;
			this.iaSourcesd[1].Play();
		}
		else
		{
			motord.targetVelocity = -180f;
			motord.force = 50f;
			if (base.transform.parent != null && num == 0)
			{
				if (base.transform.parent.gameObject.name == "DriverCameraController")
				{
					this.leg.GetComponent<LegKick>().KickClose();
				}
				else if (base.transform.parent.gameObject.name == "f100seatmount")
				{
					this.legF.GetComponent<LegKick>().KickClose();
				}
				else
				{
					this.legC.GetComponent<LegKick>().KickClose();
				}
				limits.max = 85f;
				motord.targetVelocity = 100f;
				motord.force = 100f;
				hinged.limits = limits;
				hinged.motor = motord;
				yield return new WaitForSeconds(0.2f);
				limits.max = 64f;
				motord.targetVelocity = -180f;
				motord.force = 50f;
				hinged.limits = limits;
				hinged.motor = motord;
			}
			hinged.motor = motord;
			yield return new WaitForSeconds(0.5f);
			localEulerAngles = thisDoor.localEulerAngles;
			if ((localEulerAngles.y >= 0f && localEulerAngles.y < 6f) || (localEulerAngles.y >= 353f && localEulerAngles.y <= 360f))
			{
				this.iaSourcesd[0].Play();
				limits.max = 0f;
				hinged.limits = limits;
			}
		}
		yield break;
	}

	// Token: 0x06000527 RID: 1319 RVA: 0x000379D7 File Offset: 0x00035BD7
	public void CellTower1()
	{
		if (this.towerstatus1 == 1)
		{
			this.towerstatus1 = 0;
			GameObject.Find("tracker4b").GetComponent<showmission>().ActivateNextWaypoint();
		}
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x000379FD File Offset: 0x00035BFD
	public void WaterTower1()
	{
		if (this.overflowvalve == 1)
		{
			this.overflowvalve = 0;
			GameObject.Find("tracker6").GetComponent<showmission>().ActivateNextWaypoint();
		}
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x00037A24 File Offset: 0x00035C24
	public void MissionCharacter(int charNum)
	{
		if (charNum == 1)
		{
			this.businessman.GetComponent<Businessman>().Interact();
			return;
		}
		if (charNum == 2)
		{
			this.jimmy.GetComponent<Diagnostic>().fixing = false;
			this.jimmy.GetComponent<Diagnostic>().Inspect();
			return;
		}
		if (charNum == 3)
		{
			this.jimmy.GetComponent<Diagnostic>().Interact();
			return;
		}
		if (charNum == 4)
		{
			this.jimmy.GetComponent<Diagnostic>().Fix();
			return;
		}
		if (charNum == 5)
		{
			this.fireman.GetComponent<Fireman>().Interact();
			return;
		}
		if (charNum == 6)
		{
			this.jake.GetComponent<Jake>().Interact();
			return;
		}
		if (charNum == 7)
		{
			this.jiggs.GetComponent<jiggs>().Interact();
			return;
		}
		if (charNum == 8)
		{
			this.racenpc.GetComponent<RaceNpc>().Interact();
			return;
		}
		if (charNum == 9)
		{
			this.jake2.GetComponent<Jake8>().Interact();
			return;
		}
		if (charNum == 10)
		{
			this.jake2.GetComponent<Jake8>().InteractB();
			return;
		}
		if (charNum == 11)
		{
			this.contractor.GetComponent<Contractor>().Dialogue();
			return;
		}
		if (charNum == 12)
		{
			this.modGirl.GetComponent<ModWomanJobs>().Interact();
			return;
		}
		if (charNum == 13)
		{
			this.stationClerk.GetComponent<Clerk>().Interact();
		}
	}

	// Token: 0x0600052A RID: 1322 RVA: 0x00037B58 File Offset: 0x00035D58
	public void RemoveNarco()
	{
		this.hit.collider.gameObject.GetComponent<PickUp>().enabled = true;
		this.hit.collider.gameObject.GetComponent<Rigidbody>().useGravity = true;
		this.hit.collider.gameObject.GetComponent<Rigidbody>().isKinematic = false;
		GameObject.Find("tracker36b").GetComponent<showmission>().ActivateNextWaypoint();
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x00037BCC File Offset: 0x00035DCC
	public void Lotto()
	{
		if (this.hit.collider.gameObject.GetComponent<PickUp>().pickable)
		{
			this.fpc.UnlockMouse();
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			base.GetComponent<FirstPersonController>().enabled = false;
			this.lottoPanel.SetActive(true);
			Object.Destroy(this.hit.collider.gameObject);
		}
	}

	// Token: 0x0600052C RID: 1324 RVA: 0x00037C3C File Offset: 0x00035E3C
	private void ImpoundLot()
	{
		if (Vector3.Distance(this.officer.gameObject.transform.position, this.officer.idleStation.position) < 5f)
		{
			this.fpc.UnlockMouse();
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			base.GetComponent<FirstPersonController>().enabled = false;
			this.impoundPanel.SetActive(true);
		}
	}

	// Token: 0x0600052D RID: 1325 RVA: 0x00037CAC File Offset: 0x00035EAC
	private void ViewCitation()
	{
		this.fpc.UnlockMouse();
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		base.GetComponent<FirstPersonController>().enabled = false;
		GameObject gameObject = this.hit.collider.gameObject;
		this.vCanv.ViewTicket(gameObject);
	}

	// Token: 0x0600052E RID: 1326 RVA: 0x00037CFC File Offset: 0x00035EFC
	public void Eat(int eatnum)
	{
		if (Time.time >= this.beerTimeout)
		{
			this.beerTimeout = Time.time + 4.6f;
			float hunger = this.currency.hunger;
			float num = 0f;
			this.fpsHand2.SetActive(true);
			if (eatnum == 1)
			{
				num = 50f;
				this.fpsHand2.GetComponent<Animator>().Play("hand_beefareeno");
			}
			if (eatnum == 2)
			{
				num = 90f;
				this.fpsHand2.GetComponent<Animator>().Play("hand_cram");
			}
			this.paSources.PlayOneShot(this.playerAudio[Random.Range(14, 16)], 1f);
			float num2 = hunger + num;
			if (num2 > 100f)
			{
				num2 = 100f;
			}
			this.currency.hunger = num2;
			Object.Destroy(this.hit.collider.gameObject);
			if (eatnum == 1)
			{
				base.StartCoroutine(this.SpawnCan(4));
				return;
			}
			if (eatnum == 2)
			{
				base.StartCoroutine(this.SpawnCan(5));
			}
		}
	}

	// Token: 0x0600052F RID: 1327 RVA: 0x00037DFC File Offset: 0x00035FFC
	public void Drink(int drinknum)
	{
		float num = this.currency.water + 70f;
		if (num > 100f)
		{
			num = 100f;
		}
		this.currency.water = num;
	}

	// Token: 0x06000530 RID: 1328 RVA: 0x00037E38 File Offset: 0x00036038
	public void DrinkNrg()
	{
		if (Time.time >= this.beerTimeout)
		{
			this.beerTimeout = Time.time + 4.6f;
			float num = this.currency.water + 70f;
			if (num > 100f)
			{
				num = 100f;
			}
			this.currency.water = num;
			float num2 = this.currency.sleep + 50f;
			if (num2 > 100f)
			{
				num2 = 100f;
			}
			this.currency.sleep = num2;
			Object.Destroy(this.hit.collider.gameObject);
			this.fpsHand2.SetActive(true);
			this.fpsHand2.GetComponent<Animator>().Play("hand_nrg");
			this.ranNum = Random.Range(10, 12);
			this.paSources.PlayOneShot(this.playerAudio[this.ranNum], 1f);
			base.StartCoroutine(this.SpawnCan(3));
		}
	}

	// Token: 0x06000531 RID: 1329 RVA: 0x00037F30 File Offset: 0x00036130
	public void CheckOil(int whichDipstick)
	{
		if (!this.dipStick.activeSelf)
		{
			if (whichDipstick == 1)
			{
				this.dipStick.SetActive(true);
				float z = this.ucar.GetComponent<car3>().oilLevel / 100f;
				this.dipStick.transform.GetChild(1).transform.localScale = new Vector3(1f, 1f, z);
				return;
			}
			if (whichDipstick == 2)
			{
				this.dipStick.SetActive(true);
				float num = this.fourcyl.GetComponent<engine>().newOilLevel / 100f;
				Debug.Log(num);
				this.dipStick.transform.GetChild(1).transform.localScale = new Vector3(1f, 1f, num);
				return;
			}
			if (whichDipstick == 3)
			{
				this.dipStick.SetActive(true);
				float z2 = this.v8.GetComponent<enginev8>().newOilLevel / 100f;
				this.dipStick.transform.GetChild(1).transform.localScale = new Vector3(1f, 1f, z2);
				return;
			}
			if (whichDipstick == 4)
			{
				this.dipStick.SetActive(true);
				float z3 = this.i6.GetComponent<enginei6>().newOilLevel / 100f;
				this.dipStick.transform.GetChild(1).transform.localScale = new Vector3(1f, 1f, z3);
				return;
			}
		}
		else
		{
			this.dipStick.SetActive(false);
		}
	}

	// Token: 0x06000532 RID: 1330 RVA: 0x000380B0 File Offset: 0x000362B0
	public void MoonManual()
	{
		this.moonPanel.SetActive(true);
	}

	// Token: 0x06000533 RID: 1331 RVA: 0x000380BE File Offset: 0x000362BE
	public void MoonManual2()
	{
		this.moonPanel2.SetActive(true);
	}

	// Token: 0x06000534 RID: 1332 RVA: 0x000380CC File Offset: 0x000362CC
	public void twofiftyManual()
	{
		this.twofiftyPanelo.SetActive(true);
	}

	// Token: 0x06000535 RID: 1333 RVA: 0x000380DA File Offset: 0x000362DA
	public void TireManual()
	{
		this.tirePanel.SetActive(true);
	}

	// Token: 0x06000536 RID: 1334 RVA: 0x000380E8 File Offset: 0x000362E8
	public void PayPhone()
	{
		if (this.person.transform.parent == null)
		{
			this.timeOfDay = EnviroSkyMgr.instance.GetTimeOfDay();
			this.weatherId = EnviroSkyMgr.instance.GetWeatherID();
			this.payPhonePanel.SetActive(true);
			this.fpc.UnlockMouse();
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			base.GetComponent<FirstPersonController>().enabled = false;
		}
	}

	// Token: 0x06000537 RID: 1335 RVA: 0x0003815C File Offset: 0x0003635C
	public void RaceFlyer()
	{
		this.racePanel.SetActive(true);
		this.fpc.UnlockMouse();
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		base.GetComponent<FirstPersonController>().enabled = false;
	}

	// Token: 0x06000538 RID: 1336 RVA: 0x0003818D File Offset: 0x0003638D
	public void RaceFlyer2()
	{
		this.racePanel2.SetActive(true);
		this.fpc.UnlockMouse();
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		base.GetComponent<FirstPersonController>().enabled = false;
	}

	// Token: 0x06000539 RID: 1337 RVA: 0x000381BE File Offset: 0x000363BE
	public void TrophyFlyer()
	{
		this.trophyPanel.SetActive(true);
		this.fpc.UnlockMouse();
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		base.GetComponent<FirstPersonController>().enabled = false;
	}

	// Token: 0x0600053A RID: 1338 RVA: 0x000381F0 File Offset: 0x000363F0
	public void SwitchItem()
	{
		this.fpsRatchet.GetComponent<Renderer>().enabled = false;
		this.screwdriver.GetComponent<Renderer>().enabled = false;
		this.depthgauge.SetActive(false);
		this.weldgun.GetComponent<Renderer>().enabled = false;
		this.holdingWelder = false;
		this.welderVisual.SetActive(false);
		this.ratchetLever.GetComponent<Renderer>().enabled = false;
		this.multimeter.GetComponent<Renderer>().enabled = false;
		this.multimeterSelected = false;
		this.mNeedle.GetComponent<Renderer>().enabled = false;
		this.nokia.GetComponent<Renderer>().enabled = false;
		this.nokia.transform.GetChild(0).gameObject.SetActive(false);
		this.nokia.GetComponent<PhoneTime>().enabled = false;
		this.tiregauge.GetComponent<Renderer>().enabled = false;
		this.tiregaugemeter.GetComponent<Renderer>().enabled = false;
		this.tirepump.GetComponent<Renderer>().enabled = false;
		this.holdingAirPump = false;
		this.phoneOn = false;
		this.layer_mask = (LayerMask.GetMask(new string[]
		{
			"Default"
		}) | LayerMask.GetMask(new string[]
		{
			"creeper"
		}) | LayerMask.GetMask(new string[]
		{
			"Engine1"
		}));
		this.crowbar.GetComponent<Renderer>().enabled = false;
		this.dipStick.SetActive(false);
		if (this.pickaxe.GetComponent<Renderer>().enabled)
		{
			this.pickaxe.GetComponent<Renderer>().enabled = false;
			this.newPickaxe = Object.Instantiate<GameObject>(this.pickaxeObj, this.pickaxe.transform.position, this.pickaxe.transform.rotation);
			this.newPickaxe.name = "pickaxe";
		}
	}

	// Token: 0x0600053B RID: 1339 RVA: 0x000383CC File Offset: 0x000365CC
	public void ShowToolTips(int tipNum)
	{
		this.toolTipPanel.SetActive(true);
		this.toolTipPanel.transform.GetChild(tipNum).gameObject.SetActive(true);
		if (tipNum == 0)
		{
			string str = PlayerPrefs.GetString("ExitVeh").ToUpper();
			if (PlayerPrefs.GetString("ExitVeh") == "")
			{
				str = "X";
			}
			this.toolTipPanel.transform.GetChild(tipNum).transform.GetChild(0).GetComponent<Text>().text = str + ": Exit Vehicle";
		}
		else if (tipNum == 5)
		{
			string str2 = PlayerPrefs.GetString("ExitVeh").ToUpper();
			if (PlayerPrefs.GetString("ExitVeh") == "")
			{
				str2 = "X";
			}
			this.toolTipPanel.transform.GetChild(tipNum).transform.GetChild(0).GetComponent<Text>().text = str2 + ": Stand Up";
		}
		base.StartCoroutine(this.HideToolTips(tipNum));
		this.canTrack = true;
	}

	// Token: 0x0600053C RID: 1340 RVA: 0x000384D9 File Offset: 0x000366D9
	private IEnumerator HideToolTips(int tipNum)
	{
		yield return new WaitForSeconds(4f);
		this.toolTipPanel.transform.GetChild(tipNum).gameObject.SetActive(false);
		this.toolTipPanel.SetActive(false);
		this.canTrack = false;
		yield break;
	}

	// Token: 0x0600053D RID: 1341 RVA: 0x000384EF File Offset: 0x000366EF
	private IEnumerator FixMouse()
	{
		Cursor.lockState = CursorLockMode.Locked;
		yield return new WaitForSeconds(0.05f);
		Cursor.lockState = CursorLockMode.Locked;
		this.fpc.m_MouseLook.m_cursorIsLocked = true;
		Cursor.lockState = CursorLockMode.Confined;
		this.fpc.LockMouse();
		Cursor.visible = false;
		this.fpc.m_MouseLook.SetCursorLock(true);
		yield break;
	}

	// Token: 0x0600053E RID: 1342 RVA: 0x00038500 File Offset: 0x00036700
	private void Update()
	{
		if (this.moveTrailer)
		{
			float maxDistanceDelta = 1f * Time.deltaTime;
			if (this.whichHitch == 1)
			{
				this.trailerH.transform.position = Vector3.MoveTowards(this.trailerH.transform.position, this.hitchPoint.transform.position, maxDistanceDelta);
				if (Vector3.Distance(this.trailerH.transform.position, this.hitchPoint.transform.position) < 0.1f)
				{
					this.moveTrailer = false;
				}
			}
			else if (this.whichHitch == 2)
			{
				this.trailerH.transform.position = Vector3.MoveTowards(this.trailerH.transform.position, this.hitchPointF.transform.position, maxDistanceDelta);
				if (Vector3.Distance(this.trailerH.transform.position, this.hitchPointF.transform.position) < 0.2f)
				{
					this.moveTrailer = false;
				}
			}
		}
		float width = this.mapImage.rect.width;
		float height = this.mapImage.rect.height;
		float num = width * 0.345f;
		float num2 = height * 0.51f;
		float num3 = width / 10f;
		this.apos.x = (this.person.transform.position.z * (width / 2000f) - num) * -1f;
		this.apos.y = this.person.transform.position.x * (height / 1470f) - num2;
		this.arot.z = (this.person.transform.rotation.eulerAngles.y + 270f) * -1f;
		this.prt.localPosition = this.apos;
		this.prt.eulerAngles = this.arot;
		if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.P))
		{
			this.fpc.UnlockMouse();
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			this.debugMenu.SetActive(true);
		}
		if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.X))
		{
			this.cashOption.SetActive(true);
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			bool flag = true;
			if (this.phonePanel.activeSelf)
			{
				this.phonePanel.SetActive(false);
				this.fpc.enabled = true;
				this.fpc.m_MouseLook.allowMouseLook = true;
				base.StartCoroutine(this.FixMouse());
				flag = false;
			}
			if (this.payPhonePanel.activeSelf)
			{
				this.payPhonePanel.SetActive(false);
				this.fpc.enabled = true;
				base.StartCoroutine(this.FixMouse());
				flag = false;
			}
			if (this.FSM.activeSelf)
			{
				this.FSM.SetActive(false);
				this.fpc.enabled = true;
				base.StartCoroutine(this.FixMouse());
				flag = false;
			}
			if (this.inspectionCanvas.activeSelf)
			{
				this.inspectionCanvas.SetActive(false);
				this.fpc.enabled = true;
				base.StartCoroutine(this.FixMouse());
				flag = false;
			}
			if (this.violationCanv.activeSelf)
			{
				this.violationCanv.SetActive(false);
				this.fpc.enabled = true;
				base.StartCoroutine(this.FixMouse());
				flag = false;
			}
			if (this.pcCanvas.activeSelf)
			{
				this.pcCanvas.SetActive(false);
				this.fpc.enabled = true;
				base.StartCoroutine(this.FixMouse());
				this.myPC.GetComponent<AudioSource>().Stop();
				this.myPCtower.GetComponent<AudioSource>().Stop();
				flag = false;
			}
			if (this.lottoPanel.activeSelf)
			{
				this.lottoPanel.transform.GetChild(0).gameObject.GetComponent<LottoScript>().OptionExit();
				this.lottoPanel.SetActive(false);
				this.fpc.enabled = true;
				base.StartCoroutine(this.FixMouse());
				flag = false;
			}
			if (this.inventoryCanvas.activeSelf)
			{
				this.inventoryCanvas.SetActive(false);
				this.fpc.enabled = true;
				this.fpc.m_MouseLook.allowMouseLook = true;
				base.StartCoroutine(this.FixMouse());
				flag = false;
			}
			if (this.moonPanel.activeSelf)
			{
				this.moonPanel.SetActive(false);
				this.fpc.enabled = true;
				base.StartCoroutine(this.FixMouse());
				flag = false;
			}
			if (this.moonPanel2.activeSelf)
			{
				this.moonPanel2.SetActive(false);
				this.fpc.enabled = true;
				base.StartCoroutine(this.FixMouse());
				flag = false;
			}
			if (this.twofiftyPanelo.activeSelf)
			{
				this.twofiftyPanelo.SetActive(false);
				this.fpc.enabled = true;
				base.StartCoroutine(this.FixMouse());
				flag = false;
			}
			if (this.tirePanel.activeSelf)
			{
				this.tirePanel.SetActive(false);
				this.fpc.enabled = true;
				base.StartCoroutine(this.FixMouse());
				flag = false;
			}
			if (this.racePanel.activeSelf)
			{
				this.racePanel.SetActive(false);
				this.fpc.enabled = true;
				base.StartCoroutine(this.FixMouse());
				flag = false;
			}
			if (this.trophyPanel.activeSelf)
			{
				this.trophyPanel.SetActive(false);
				this.fpc.enabled = true;
				base.StartCoroutine(this.FixMouse());
				flag = false;
			}
			if (this.racePanel2.activeSelf)
			{
				this.racePanel2.SetActive(false);
				this.fpc.enabled = true;
				base.StartCoroutine(this.FixMouse());
				flag = false;
			}
			if (this.atmMenu.activeSelf)
			{
				this.atmMenu.SetActive(false);
				this.fpc.enabled = true;
				base.StartCoroutine(this.FixMouse());
				flag = false;
			}
			if (this.rgbPanel.activeSelf)
			{
				this.rgbPanel.SetActive(false);
				this.fpc.enabled = true;
				base.StartCoroutine(this.FixMouse());
				flag = false;
			}
			if (this.rgbPanelPaint.activeSelf)
			{
				this.rgbPanelPaint.SetActive(false);
				this.fpc.enabled = true;
				base.StartCoroutine(this.FixMouse());
				flag = false;
			}
			if (this.escapePanel.activeSelf)
			{
				Time.timeScale = 1f;
				AudioListener.pause = false;
				if (this.mainPanelMgr.currentPanelIndex == 0)
				{
					this.escapePanel.SetActive(false);
					this.fpc.enabled = true;
					base.StartCoroutine(this.FixMouse());
					flag = false;
				}
				else
				{
					this.mainPanelMgr.PanelAnim(0);
				}
			}
			if (flag)
			{
				this.fpc.UnlockMouse();
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
				this.escapePanel.SetActive(true);
				Time.timeScale = 0f;
				AudioListener.pause = true;
				this.phonePanel.SetActive(false);
				this.atmMenu.SetActive(false);
				this.payPhonePanel.SetActive(false);
				this.pcCanvas.SetActive(false);
				this.lottoPanel.SetActive(false);
				this.impoundPanel.SetActive(false);
				this.FSM.SetActive(false);
				this.rgbPanel.SetActive(false);
				this.rgbPanelPaint.SetActive(false);
				this.inspectionCanvas.SetActive(false);
				this.violationCanv.SetActive(false);
				base.GetComponent<FirstPersonController>().enabled = false;
			}
		}
		if (Input.GetKeyDown(KeyCode.T) && this.canTrack)
		{
			this.canTrack = false;
			this.eventSystem.GetComponent<MissionController>().TrackMission_N(this.discoveredMission);
			this.toolTipPanel.SetActive(false);
		}
		if (!this.arrested)
		{
			if (Input.GetKeyDown(this.cr.Item1))
			{
				this.SwitchItem();
			}
			if (Input.GetKeyDown(this.cr.Item2))
			{
				this.SwitchItem();
				this.fpsRatchet.GetComponent<Renderer>().enabled = true;
				this.ratchetLever.GetComponent<Renderer>().enabled = true;
				this.layer_mask = LayerMask.GetMask(new string[]
				{
					"Bolts"
				});
			}
			if (Input.GetKeyDown(this.cr.Item3))
			{
				this.SwitchItem();
				this.multimeter.GetComponent<Renderer>().enabled = true;
				this.mNeedle.GetComponent<Renderer>().enabled = true;
				this.multimeterSelected = true;
			}
			if (Input.GetKeyDown(this.cr.Item4))
			{
				this.SwitchItem();
				this.depthgauge.SetActive(true);
			}
			if (Input.GetKeyDown(this.cr.Item5))
			{
				this.SwitchItem();
				this.nokia.GetComponent<Renderer>().enabled = true;
				this.nokia.transform.GetChild(0).gameObject.SetActive(true);
				this.nokia.GetComponent<PhoneTime>().enabled = true;
				this.phoneOn = true;
			}
			if (Input.GetKeyDown(this.cr.Item6))
			{
				this.SwitchItem();
				this.tiregauge.GetComponent<Renderer>().enabled = true;
				this.tiregaugemeter.GetComponent<Renderer>().enabled = true;
			}
			if (Input.GetKeyDown(this.cr.Item7))
			{
				this.SwitchItem();
				this.crowbar.GetComponent<Renderer>().enabled = true;
			}
		}
		if (Input.GetKeyDown(this.cr.Inventory))
		{
			if (!this.inventoryCanvas.activeSelf && !this.arrested)
			{
				this.fpc.UnlockMouse();
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
				this.inventoryCanvas.SetActive(true);
				this.inventoryCanvas.GetComponent<InventoryItems>().OpenCheck();
				this.fpc.m_MouseLook.allowMouseLook = false;
			}
			else
			{
				Cursor.visible = false;
				this.fpc.LockMouse();
				this.inventoryCanvas.SetActive(false);
				base.GetComponent<FirstPersonController>().enabled = true;
				this.fpc.m_MouseLook.allowMouseLook = true;
			}
		}
		if ((Input.GetKeyDown(this.cr.PlaceInv) || Input.GetMouseButtonDown(2)) && this.pickedUpObject != null)
		{
			this.inv.AddObject(this.pickedUpObject);
		}
		if (this.phoneOn && Input.GetKeyDown(this.cr.Interact))
		{
			this.fpc.UnlockMouse();
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			base.GetComponent<FirstPersonController>().enabled = false;
			this.phonePanel.SetActive(true);
			this.phonePanel.transform.GetChild(0).gameObject.GetComponent<PhoneScript>().Start();
		}
		if (this.holdingAirPump && Vector3.Distance(this.pumpMachine.transform.position, base.transform.position) > 6f)
		{
			this.holdingAirPump = false;
			this.tirepump.GetComponent<Renderer>().enabled = false;
		}
		if (this.holdingHose && Vector3.Distance(this.garageFaucet.transform.position, base.transform.position) > 15f && Vector3.Distance(this.garageFaucet2.transform.position, base.transform.position) > 15f)
		{
			this.holdingHose = false;
			this.fpshose.SetActive(false);
		}
		if (this.holdingWelder && Vector3.Distance(this.welderMachine.transform.position, base.transform.position) > 8f)
		{
			this.holdingWelder = false;
			this.welderVisual.SetActive(true);
			this.weldgun.GetComponent<Renderer>().enabled = false;
		}
		if (Input.GetKeyDown(this.cr.Throw) && this.pickedUpObject != null)
		{
			this.ThrowObject();
		}
		if (Input.GetMouseButtonUp(0) && this.pickedUpObject != null && this.pickedUpObject.activeSelf && !this.inventoryCanvas.active && Time.time >= this.dropInterval)
		{
			this.DropObject();
		}
		if (Input.GetAxis("Mouse ScrollWheel") < 0f)
		{
			this.ratchetLever.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			if (this.pickedUpObject != null)
			{
				Quaternion rhs = Quaternion.AngleAxis(10f, Vector3.left);
				this.pickedUpObject.transform.rotation *= rhs;
			}
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0f)
		{
			this.ratchetLever.transform.localRotation = Quaternion.Euler(0f, -60f, 0f);
			if (this.pickedUpObject != null)
			{
				Quaternion rhs2 = Quaternion.AngleAxis(-10f, Vector3.left);
				this.pickedUpObject.transform.rotation *= rhs2;
			}
		}
		if (Input.GetKeyDown(this.cr.LeanOut))
		{
			this.LookOut();
		}
		if (Input.GetKeyDown(this.cr.LeanIn))
		{
			this.LeanIn();
		}
		if (Input.GetKeyUp(this.cr.LeanOut))
		{
			this.LeanBack();
		}
		if (Input.GetKeyDown(this.cr.ExitVeh))
		{
			if (!this.arrested)
			{
				if (this.whichCar == 1)
				{
					if (!this.exitMount.GetComponent<driverExit>().exitBlocked || !this.exitMountP.GetComponent<driverExit>().exitBlocked)
					{
						this.GetOut2();
					}
				}
				else if (this.whichCar == 2)
				{
					if (!this.exitMountCar.GetComponent<driverExit>().exitBlocked || !this.exitMountCarP.GetComponent<driverExit>().exitBlocked)
					{
						this.GetOut2();
					}
				}
				else if (this.whichCar == 4 || this.whichCar == 5 || this.whichCar == 6)
				{
					this.GetOut2();
				}
			}
			if (this.pokerScript.sitting)
			{
				this.pokerScript.StandUp();
			}
		}
		if (Input.GetKeyDown(this.cr.Map))
		{
			if (!this.mapCanvas.activeSelf)
			{
				this.mapCanvas.SetActive(true);
			}
			else
			{
				this.mapCanvas.SetActive(false);
			}
		}
		Input.GetKeyDown(KeyCode.Z);
		if (Input.GetMouseButtonUp(1) && this.fpsRatchet.GetComponent<Renderer>().enabled)
		{
			this.layer_mask = LayerMask.GetMask(new string[]
			{
				"Bolts"
			});
		}
		Physics.Raycast(this.cam.transform.position, this.cam.transform.forward, out this.hit, this.interactRange, this.layer_mask);
		if (Input.GetMouseButtonDown(0) && this.pickaxe.activeSelf && this.hit.transform && this.pickaxe.GetComponent<Renderer>().enabled && (this.hit.collider.name == "dirtcake" || this.hit.collider.name == "ore"))
		{
			this.pickaxe.GetComponent<Pickaxe>().Swing(this.hit.transform);
		}
		if (Input.GetMouseButtonDown(0) && this.holdingWelder && this.hit.transform && this.weldgun.GetComponent<Renderer>().enabled)
		{
			Debug.Log(this.hit.collider.gameObject.name);
			if (this.hit.collider.gameObject.GetComponent<ImpactDeformable>())
			{
				float num4 = this.welderMachine.transform.GetChild(0).gameObject.GetComponent<durability>().health;
				if (num4 > 0f)
				{
					num4 -= 2f;
					this.welderMachine.transform.GetChild(0).gameObject.GetComponent<durability>().health -= 2f;
					if (num4 < 2f)
					{
						this.welderMachine.transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = false;
						this.welderMachine.transform.GetChild(1).gameObject.GetComponent<Renderer>().enabled = true;
					}
					this.hit.collider.gameObject.GetComponent<ImpactDeformable>().Repair(0.5f, null, null);
					this.weldingFlash.Emit(1);
					int num5 = Random.Range(4, 7);
					this.paSources.PlayOneShot(this.playerAudio[num5], 1f);
				}
			}
		}
		if (this.hit.transform)
		{
			if (Time.time >= this.hitInterval || Input.GetMouseButtonDown(1) || Input.GetAxis("Mouse ScrollWheel") != 0f)
			{
				this.hitInterval = Time.time + 0.1f;
				this.interactiveObject = this.hit.collider.transform.GetComponent<InteractiveObject>();
				bool flag2 = false;
				if (this.hit.collider.name == "camshaft_e" || this.hit.collider.name == "distributor_e" || this.hit.collider.name == "battery_e" || this.hit.collider.name == "plugwires_e" || this.hit.collider.name == "crank_e" || this.hit.collider.name == "oilfilter_e" || this.hit.collider.name == "fan_e" || this.hit.collider.name == "fanbelt_e" || this.hit.collider.name == "fanclutch_e" || this.hit.collider.name == "clutch_e" || this.hit.collider.name == "piston1_e" || this.hit.collider.name == "piston2_e" || this.hit.collider.name == "piston3_e" || this.hit.collider.name == "piston4_e")
				{
					flag2 = true;
					foreach (object obj in this.hit.collider.transform)
					{
						Transform transform = (Transform)obj;
						if (transform.GetComponent<durability>() != null && transform.GetComponent<Renderer>().enabled)
						{
							flag2 = false;
						}
					}
					if (flag2 && this.hit.collider.gameObject.GetComponent<durability>().boltStr < 1f)
					{
						this.hit.collider.gameObject.GetComponent<durability>().canDetach = true;
					}
				}
				if ((this.holdingHose || this.holdingHoseTank) && this.fpshose.transform.GetChild(0).gameObject.activeSelf)
				{
					if (this.hit.collider.name == "cabin_low" || this.hit.collider.name == "cardoor_d" || this.hit.collider.name == "cardoor_p" || this.hit.collider.name == "BrakeDisk" || this.hit.collider.name == "body_collider.003" || this.hit.collider.name == "body_collider.007")
					{
						this.gd.dirty = this.gd.tireFL.material.GetFloat("_Blend");
						if (this.gd.dirty > 0f)
						{
							if (this.gd.dirty > 1f)
							{
								this.gd.dirty = 0.9f;
							}
							this.gd.dirty -= 0.03f;
						}
						this.gd.dirty2 = this.gd.body.material.GetFloat("_RustIntensity");
						if (this.gd.dirty2 > 0f)
						{
							if (this.gd.dirty2 > 1f)
							{
								this.gd.dirty2 = 0.9f;
							}
							this.gd.dirty2 -= 0.03f;
						}
						this.gd.WashDirt(1);
					}
					else if (this.hit.collider.name == "fhood" || this.hit.collider.name == "f100_ddoor" || this.hit.collider.name == "f100_pdoor" || this.hit.collider.name == "fmainbody")
					{
						this.gd.dirtyF = this.gd.tireFLF.material.GetFloat("_Blend");
						if (this.gd.dirtyF > 0f)
						{
							if (this.gd.dirtyF > 1f)
							{
								this.gd.dirtyF = 0.9f;
							}
							this.gd.dirtyF -= 0.03f;
						}
						if (this.gd.dirty2F > 0f)
						{
							if (this.gd.dirty2F > 1f)
							{
								this.gd.dirty2F = 0.9f;
							}
							this.gd.dirty2F -= 0.03f;
						}
						this.gd.WashDirt(2);
					}
					else if (this.hit.collider.name == "DirtBike")
					{
						this.gd.dirty250 = this.gd.dbtireR.material.GetFloat("_Blend");
						if (this.gd.dirty250 > 0f)
						{
							if (this.gd.dirty250 > 1f)
							{
								this.gd.dirty250 = 0.9f;
							}
							this.gd.dirty250 -= 0.03f;
						}
						this.gd.dirty2502 = this.gd.plastic.material.GetFloat("_RustIntensity");
						if (this.gd.dirty2502 > 0f)
						{
							if (this.gd.dirty2502 > 1f)
							{
								this.gd.dirty2502 = 0.9f;
							}
							this.gd.dirty2502 -= 0.03f;
						}
						this.gd.WashDirt(3);
					}
					else if (this.hit.collider.name == "tobaccoplant")
					{
						this.hit.collider.gameObject.GetComponent<PlantHealth>().Hydrate();
					}
				}
				if (this.hit.collider.name == "waterstream" && this.hit.distance < 0.65f && this.currency.water < 98f)
				{
					this.currency.water += 2f;
					if (!this.paSources.isPlaying)
					{
						if (this.hit.collider.tag == "special" && !this.ach_classy)
						{
							this.ach_classy = true;
							Achievement achievement = new Achievement("ACH_CLASSY");
							achievement.Trigger(true);
						}
						this.paSources.PlayOneShot(this.playerAudio[1], 1f);
					}
				}
				if (Input.GetMouseButtonDown(1))
				{
					if (this.hit.collider.name != "BrakeDisk")
					{
						this.layer_mask = (LayerMask.GetMask(new string[]
						{
							"Default"
						}) | LayerMask.GetMask(new string[]
						{
							"creeper"
						}) | LayerMask.GetMask(new string[]
						{
							"Engine1"
						}));
						Physics.Raycast(this.cam.transform.position, this.cam.transform.forward, out this.hit, this.interactRange, this.layer_mask, QueryTriggerInteraction.Ignore);
					}
					if (this.hit.collider.GetComponent<durability>() && (this.hit.collider.GetComponent<durability>().canDetach || this.hit.collider.GetComponent<durability>().numBolts == 0))
					{
						this.haltDetach = false;
						foreach (object obj2 in this.hit.collider.transform)
						{
							Transform transform2 = (Transform)obj2;
							if (transform2.GetComponent<durability>() != null && transform2.GetComponent<Renderer>().enabled && !this.hit.collider.gameObject.name.Contains("sparemount") && !this.hit.collider.gameObject.name.Contains("infuser") && !this.hit.collider.gameObject.name.Contains("brokenTurbo"))
							{
								this.haltDetach = true;
							}
						}
						if (this.person.transform.parent != null && this.person.transform.parent.gameObject.name != "creepermount")
						{
							this.haltDetach = true;
						}
						if ((this.hit.collider.GetComponent<Renderer>().enabled || this.hit.collider.gameObject.name == "turbo_e") && !this.haltDetach)
						{
							this.canSpawn = true;
							foreach (object obj3 in this.hit.collider.transform)
							{
								Transform transform3 = (Transform)obj3;
								if (transform3.name != "rocker" && transform3.name != "WHEEL_HOLDER" && transform3.name != "Primary Drive Gear" && transform3.name != "distwire" && transform3.name != "OilDrain" && transform3.name != "OilDrainV8" && transform3.name != "OilDrainI6" && transform3.name != "laminarOil" && transform3.name != "laminarOilV8" && transform3.name != "origin" && transform3.name != "JackBody1" && transform3.name != "JackMech1" && transform3.name != "JackHandle1" && transform3.name != "FLpos" && transform3.name != "FRpos" && transform3.name != "RLpos" && transform3.name != "RRpos" && transform3.name != "altfan" && transform3.name != "turbine" && transform3.name != "250_kickstarter" && transform3.name != "250_cam" && transform3.name != "250_gear2" && transform3.name != "Plane001.003" && transform3.name != "jerrycaninlet" && transform3.name != "universalpulley" && transform3.name != "brokenTurbo" && transform3.name != "turbotach" && transform3.name != "buddyscreen" && transform3.tag != "ignoreattach")
								{
									transform3.GetComponent<BoxCollider>().enabled = false;
								}
								if (transform3.name == "OilDrain")
								{
									this.oilBolt.SetActive(false);
								}
								else if (transform3.name == "OilDrainV8")
								{
									this.oilBoltV8.SetActive(false);
								}
								else if (transform3.name == "OilDrainI6")
								{
									this.oilBolti6.SetActive(false);
								}
								else if (transform3.name == "ww_internals")
								{
									this.hit.collider.GetComponent<Waterwheel>().canRotate = false;
									transform3.gameObject.SetActive(false);
								}
								else if (transform3.name == "brokenTurbo")
								{
									transform3.gameObject.SetActive(false);
								}
								this.canSpawn = true;
								if (transform3.name == "jerrycaninlet")
								{
									this.jerrycan.SetActive(true);
									this.jerrycan.transform.position = this.hit.collider.transform.position;
									this.jerrycan.transform.rotation = this.hit.collider.transform.rotation;
									this.jerrycan.GetComponent<Rigidbody>().useGravity = true;
									this.canSpawn = false;
								}
								if (transform3.name == "WHEEL_HOLDER")
								{
									if (!transform3.GetChild(1).GetComponent<Renderer>().enabled && !transform3.GetChild(2).GetComponent<Renderer>().enabled && !transform3.GetChild(3).GetComponent<Renderer>().enabled && !transform3.GetChild(4).GetComponent<Renderer>().enabled && !transform3.GetChild(5).GetComponent<Renderer>().enabled)
									{
										this.canSpawn = false;
									}
									else
									{
										this.i = 1;
										while (this.i < 6)
										{
											if (transform3.GetChild(this.i).gameObject.active)
											{
												this.rimNum = this.i;
												transform3.GetChild(this.i).gameObject.SetActive(false);
											}
											this.i++;
										}
										this.tireNum = 0;
										this.i = 6;
										while (this.i < 20)
										{
											if (transform3.GetChild(this.i).gameObject.active)
											{
												this.tireNum = this.i;
												this.deflation = transform3.GetChild(this.i).gameObject.GetComponent<Renderer>().material.GetFloat("_TireFlatnessT");
												transform3.GetChild(this.i).gameObject.SetActive(false);
											}
											this.i++;
										}
										this.spawnPart = this.hit.collider.GetComponent<durability>().template;
									}
									if (this.hit.collider.GetComponent<durability>().wc != null)
									{
										this.hit.collider.GetComponent<durability>().wc.radius = 0.2f;
									}
								}
								if (transform3.name == "bolt" || transform3.name == "oilcap" || transform3.name == "rocker" || transform3.name == "Primary Drive Gear" || transform3.name == "distwire" || transform3.name == "radhose_therm" || transform3.name == "altfan" || transform3.name == "hookw" || transform3.name == "hookwF" || transform3.name == "turbine" || transform3.name == "250_kickstarter" || transform3.name == "250_cam" || transform3.name == "250_gear2" || transform3.name == "250_transcap" || transform3.name == "250_oilcap" || transform3.name == "universalpulley" || transform3.name == "buddyscreen")
								{
									transform3.GetComponent<Renderer>().enabled = false;
									if (transform3.name == "bolt")
									{
										transform3.GetComponent<BoxCollider>().enabled = false;
									}
								}
								if (transform3.name == "JackBody1" || transform3.name == "JackMech1" || transform3.name == "JackHandle1")
								{
									transform3.GetComponent<Renderer>().enabled = false;
								}
								if (transform3.name == "blackberries_e" || transform3.name == "oranges_e" || transform3.name == "limes_e" || transform3.name == "ambrosia_e")
								{
									transform3.GetComponent<Renderer>().enabled = false;
									transform3.GetComponent<durability>().health = 0f;
									transform3.parent.GetChild(0).gameObject.SetActive(true);
								}
							}
							this.thisdura = this.hit.collider.GetComponent<durability>().health;
							if (this.hit.collider.name == "batterycharger_e")
							{
								this.hit.collider.transform.parent.gameObject.GetComponent<BatteryCharger>().DisconnectBattery();
								this.canSpawn = true;
							}
							if (this.hit.collider.name == "batterycharger18_e")
							{
								this.hit.collider.transform.parent.gameObject.GetComponent<BatteryCharger>().DisconnectBattery18();
								this.canSpawn = true;
							}
							if (this.hit.collider.name != "BrakeDisk")
							{
								this.hit.collider.GetComponent<Renderer>().enabled = false;
								this.spawnPart = this.hit.collider.GetComponent<durability>().template;
							}
							if (this.hit.collider.name == "oldengine_e")
							{
								this.oldeng_acc.SetActive(false);
							}
							if (this.hit.collider.name == "turbogaugeD_e")
							{
								this.hit.collider.transform.GetChild(0).gameObject.SetActive(false);
							}
							BoxCollider[] components = this.hit.collider.GetComponents<BoxCollider>();
							if (!components[0].isTrigger)
							{
								components[0].enabled = false;
							}
							else
							{
								components[1].enabled = false;
							}
							if (this.canSpawn)
							{
								if (this.spawnPart.name == "truckwheel")
								{
									if (this.hit.collider.transform.parent.gameObject.name == "SteeringJointFR2")
									{
										this.newTire = Object.Instantiate<GameObject>(this.spawnPart, this.RemoveFR.position, this.RemoveFR.rotation);
									}
									else if (this.hit.collider.transform.parent.gameObject.name == "SteeringJointFL2")
									{
										this.newTire = Object.Instantiate<GameObject>(this.spawnPart, this.RemoveFL.position, this.RemoveFL.rotation);
									}
									else if (this.hit.collider.transform.parent.gameObject.name == "AxleRearLeft")
									{
										this.newTire = Object.Instantiate<GameObject>(this.spawnPart, this.RemoveRL.position, this.RemoveRL.rotation);
									}
									else if (this.hit.collider.transform.parent.gameObject.name == "AxleRearRight")
									{
										this.newTire = Object.Instantiate<GameObject>(this.spawnPart, this.RemoveRR.position, this.RemoveRR.rotation);
									}
									else if (this.hit.collider.transform.parent.gameObject.name == "sparemount_e")
									{
										this.newTire = Object.Instantiate<GameObject>(this.spawnPart, this.hit.collider.transform.position, this.hit.collider.transform.rotation);
									}
									else if (this.hit.collider.transform.parent.gameObject.name == "SteeringJointFR2F")
									{
										this.newTire = Object.Instantiate<GameObject>(this.spawnPart, this.RemoveFRF.position, this.RemoveFRF.rotation);
									}
									else if (this.hit.collider.transform.parent.gameObject.name == "SteeringJointFL2F")
									{
										this.newTire = Object.Instantiate<GameObject>(this.spawnPart, this.RemoveFLF.position, this.RemoveFLF.rotation);
									}
									else if (this.hit.collider.transform.parent.gameObject.name == "AxleRearLeftF")
									{
										this.newTire = Object.Instantiate<GameObject>(this.spawnPart, this.RemoveRLF.position, this.RemoveRLF.rotation);
									}
									else if (this.hit.collider.transform.parent.gameObject.name == "AxleRearRightF")
									{
										this.newTire = Object.Instantiate<GameObject>(this.spawnPart, this.RemoveRRF.position, this.RemoveRRF.rotation);
									}
									else if (this.hit.collider.transform.parent.gameObject.name == "sparemountF_e")
									{
										this.newTire = Object.Instantiate<GameObject>(this.spawnPart, this.hit.collider.transform.position, this.hit.collider.transform.rotation);
									}
									this.newTire.transform.GetChild(this.rimNum).gameObject.SetActive(true);
									this.newTire.GetComponent<TireAssign>().rimNumX = this.rimNum;
									this.newTire.name = "truckwheel";
									Debug.Log(this.hit.collider.transform.parent.gameObject.name);
									if (this.modGirl.GetComponent<ModWomanJobs>().tireSlot == this.hit.collider.transform.parent.gameObject.name)
									{
										this.newTire.name = "truckwheelx";
										this.newTire.GetComponent<PickUp>().tradein = 0f;
										this.newTire.GetComponent<PickUp>().description = "Test Wheel";
										this.modGirl.GetComponent<ModWomanJobs>().tireSlot = null;
										this.modGirl.GetComponent<ModWomanJobs>().spawnedTire = this.newTire;
									}
									if (this.tireNum > 0)
									{
										this.newTire.transform.GetChild(this.tireNum).gameObject.SetActive(true);
										this.newTire.GetComponent<TireAssign>().tireNumX = this.tireNum;
										this.newTire.GetComponents<MeshCollider>()[1].enabled = true;
									}
									this.newTire.GetComponent<PickUp>().deflation = this.deflation;
									this.newTire.GetComponent<PickUp>().thisDurability = this.thisdura;
								}
								else
								{
									this.newPart = Object.Instantiate<GameObject>(this.spawnPart, this.hit.collider.transform.position, this.hit.collider.transform.rotation);
									PickUp component = this.newPart.GetComponent<PickUp>();
									component.price = 0f;
									component.thisDurability = this.thisdura;
									durability component2 = this.hit.collider.GetComponent<durability>();
									if (this.spawnPart.name == "turbo" && this.fourcyl.GetComponent<engine>().nonOemTurbo)
									{
										this.newPart.name = "TurboA";
										this.newPart.GetComponent<PickUp>().description = "Unmarked Turbo";
										this.fourcyl.GetComponent<engine>().nonOemTurbo = false;
										this.modGirl.GetComponent<ModWomanJobs>().spawnedTurbo = this.newPart;
									}
									if (this.spawnPart.name == "tractionbuddy" && this.thisdura == 0f)
									{
										component.tradein = 200f;
									}
									if (component2.painted)
									{
										component.red = component2.red;
										component.green = component2.green;
										component.blue = component2.blue;
										component.metallic = component2.metallic;
										component.smoothness = component2.smoothness;
										component.painted = true;
										Material[] materials = this.newPart.GetComponent<Renderer>().materials;
										int paintSlot = component.paintSlot;
										materials[paintSlot].color = new UnityEngine.Color(component2.red, component2.green, component2.blue, 1f);
										component2.painted = false;
										component2.red = 0f;
										component2.green = 0f;
										component2.blue = 0f;
										component2.metallic = 0f;
										component2.smoothness = 0f;
									}
								}
								if (this.hit.collider.transform.gameObject.name.Contains("batterycart"))
								{
									this.cart.GetComponent<golfcart>().RemoveBattery();
								}
								if (this.spawnPart.name.Contains("radar"))
								{
									this.hit.collider.transform.gameObject.GetComponent<RadarDetector>().enabled = false;
								}
								else if (this.spawnPart.name.Contains("tractionbuddy"))
								{
									this.hit.collider.transform.gameObject.GetComponent<TractionBuddy>().enabled = false;
									this.hit.collider.transform.gameObject.GetComponent<AudioSource>().enabled = false;
								}
								this.hit.collider.GetComponent<durability>().health = 0f;
							}
						}
					}
					if ((this.hit.collider.transform.name == "truckwheel" || this.hit.collider.transform.name == "truckwheel(Clone)") && this.crowbar.GetComponent<Renderer>().enabled)
					{
						this.tireNum = 0;
						this.i = 1;
						while (this.i < 6)
						{
							if (this.hit.collider.transform.GetChild(this.i).gameObject.active)
							{
								this.i = 6;
								while (this.i < 20)
								{
									if (this.hit.collider.transform.GetChild(this.i).gameObject.active)
									{
										this.crowbar.GetComponent<AudioSource>().Play();
										this.hit.collider.transform.GetChild(this.i).gameObject.SetActive(false);
										this.hit.collider.transform.GetChild(0).gameObject.SetActive(false);
										this.hit.collider.transform.gameObject.GetComponent<TireAssign>().tireNumX = 0;
										this.newTire = Object.Instantiate<GameObject>(this.wheelObj, this.hit.collider.transform.position, this.hit.collider.transform.rotation);
										this.newTire.transform.GetChild(this.i).gameObject.SetActive(true);
										this.newTire.transform.GetChild(0).gameObject.SetActive(true);
										this.newTire.name = "truckwheel";
										this.newTire.GetComponent<PickUp>().deflation = 100f;
										this.newTire.GetComponent<PickUp>().thisDurability = this.hit.collider.transform.gameObject.GetComponent<PickUp>().thisDurability;
										this.newTire.GetComponent<TireAssign>().rimNumX = 0;
										this.newTire.GetComponent<TireAssign>().tireNumX = this.i;
										MeshCollider[] components2 = this.newTire.GetComponents<MeshCollider>();
										components2[0].enabled = false;
										components2[1].enabled = true;
										MeshCollider[] components3 = this.hit.collider.GetComponents<MeshCollider>();
										components3[0].enabled = true;
										components3[1].enabled = false;
									}
									this.i++;
								}
							}
							this.i++;
						}
					}
				}
				if (this.multimeterSelected && this.hit.collider.transform.name.Contains("battery") && !this.mNeedle.GetComponent<InteractiveObject>().isOpen)
				{
					if (this.hit.collider.GetComponent<PickUp>() != null)
					{
						this.batteryCharge = this.hit.collider.GetComponent<PickUp>().thisDurability + 38f;
						this.mNeedle.GetComponent<InteractiveObject>().openPosition = new Vector3(-90f, 0f, this.batteryCharge);
						this.mNeedle.GetComponent<InteractiveObject>().PerformAction();
						this.multimeterActive = true;
					}
					if (this.hit.collider.GetComponent<durability>() != null)
					{
						this.batteryCharge = this.hit.collider.GetComponent<durability>().health + 38f;
						this.mNeedle.GetComponent<InteractiveObject>().openPosition = new Vector3(-90f, 0f, this.batteryCharge);
						this.mNeedle.GetComponent<InteractiveObject>().PerformAction();
						this.multimeterActive = true;
					}
				}
				if (this.multimeterActive && !this.hit.collider.transform.name.Contains("battery"))
				{
					this.multimeterActive = false;
					this.mNeedle.GetComponent<InteractiveObject>().PerformAction();
				}
				if ((this.hit.collider.transform.name.Contains("heavy_") || this.hit.collider.transform.name.Contains("rwg_")) && Input.GetKey(this.cr.Interact))
				{
					if (this.hit.collider.transform.name.Contains("heavy_"))
					{
						if (Vector3.Distance(this.trailerH.transform.position, this.hit.collider.transform.position) < 10f)
						{
							this.trailerH.GetComponent<RWGTrailer>().LoadCargo(this.hit.collider.transform.name);
							Object.Destroy(this.hit.collider.gameObject);
						}
					}
					else if (this.hit.collider.transform.name.Contains("rwg_"))
					{
						this.trailerH.GetComponent<RWGTrailer>().UnloadCargo();
					}
				}
				if (this.hit.collider.transform.name == "ignition_key")
				{
					if (Input.GetKey(this.cr.Interact))
					{
						new Random();
						if (this.keyState == 1 && Time.time >= (float)this.nextUpdate)
						{
							if (!this.carscript.usingV8 && !this.carscript.usingI6)
							{
								this.canCrank = this.fourcyl.GetComponent<engine>().canCrank;
							}
							else if (this.carscript.usingI6)
							{
								this.canCrank = this.i6.GetComponent<enginei6>().canCrank;
							}
							else
							{
								this.canCrank = this.v8.GetComponent<enginev8>().canCrank;
								if (this.v8.GetComponent<enginev8>().whichTruck != 1)
								{
									this.canCrank = false;
								}
							}
							if (this.canCrank)
							{
								this.iaSources[3].Play();
								if (!this.carscript.usingV8 && !this.carscript.usingI6)
								{
									this.fourcyl.GetComponent<engine>().DrainBat();
								}
								else if (this.carscript.usingI6)
								{
									this.i6.GetComponent<enginei6>().DrainBat();
								}
								else
								{
									this.v8.GetComponent<enginev8>().DrainBat();
								}
								this.nextUpdate = Mathf.FloorToInt(Time.time) + 1;
								this.crankTime = Random.Range(1, 4);
								if (!this.carscript.usingV8)
								{
									if (this.carscript.fastStart)
									{
										this.crankTime = 1;
									}
								}
								else if (this.carscript.fastStartV8)
								{
									this.crankTime = 1;
								}
								if (this.carscript.temperature > 50f)
								{
									this.crankTime = 1;
								}
								this.attemptedCrank = true;
								this.carscript.dimAcc();
								this.canRun = this.carscript.canRun;
								if (this.crankTime == 1 && this.canRun)
								{
									this.keyState = 2;
									this.KeyStateChange();
									if (!this.ach_decent)
									{
										this.ach_decent = true;
										Achievement achievement2 = new Achievement("ACH_DECENT");
										achievement2.Trigger(true);
									}
								}
							}
						}
					}
					else if (this.keyState == 1)
					{
						this.iaSources[3].Stop();
						this.keyState = 0;
						if (this.attemptedCrank)
						{
							this.iaSources[0].Play();
							this.carscript.turnOffAcc();
							this.attemptedCrank = false;
						}
					}
				}
				if (this.hit.collider.transform.name == "ignition_keyF")
				{
					if (Input.GetKey(this.cr.Interact))
					{
						new Random();
						if (this.keyStateF == 1 && Time.time >= (float)this.nextUpdate)
						{
							if (this.fcarscript.usingV8)
							{
								this.canCrank = this.v8.GetComponent<enginev8>().canCrank;
								if (this.v8.GetComponent<enginev8>().whichTruck != 2)
								{
									this.canCrank = false;
								}
							}
							if (this.canCrank)
							{
								this.iaSources[3].Play();
								if (this.fcarscript.usingV8)
								{
									this.v8.GetComponent<enginev8>().DrainBat();
								}
								this.nextUpdate = Mathf.FloorToInt(Time.time) + 1;
								this.crankTime = Random.Range(1, 4);
								if (this.fcarscript.temperature > 50f)
								{
									this.crankTime = 1;
								}
								this.attemptedCrank = true;
								this.canRun = this.fcarscript.canRun;
								if (this.fcarscript.fastStart)
								{
									this.crankTime = 1;
								}
								if (this.crankTime == 1 && this.canRun)
								{
									this.keyStateF = 2;
									this.KeyStateChangeF();
								}
							}
						}
					}
					else if (this.keyStateF == 1)
					{
						this.iaSources[3].Stop();
						this.keyStateF = 0;
						if (this.attemptedCrank)
						{
							this.iaSources[0].Play();
							this.fcarscript.turnOffAcc();
							this.attemptedCrank = false;
						}
					}
				}
				if (this.hit.collider.transform.name == "ignition_keyCar")
				{
					if (Input.GetKey(this.cr.Interact))
					{
						new Random();
						if (this.keyStateC == 1 && Time.time >= (float)this.nextUpdate)
						{
							if (!this.acarscript.usingI6)
							{
								if (!this.acarscript.engineRenderer.enabled)
								{
									this.canCrank = false;
								}
								else
								{
									this.canCrank = true;
								}
							}
							else
							{
								this.canCrank = this.i6.GetComponent<enginei6>().canCrank;
								if (this.i6.GetComponent<enginei6>().whichTruck != 2)
								{
									this.canCrank = false;
								}
							}
							if (this.canCrank)
							{
								this.iaSources[3].Play();
								bool usingI = this.acarscript.usingI6;
								this.nextUpdate = Mathf.FloorToInt(Time.time) + 1;
								this.crankTime = Random.Range(1, 4);
								if (this.acarscript.temperature > 50f)
								{
									this.crankTime = 1;
								}
								this.attemptedCrank = true;
								this.canRun = this.acarscript.canRun;
								if (this.crankTime == 1 && this.canRun)
								{
									this.keyStateC = 2;
									this.KeyStateChangeCar();
								}
							}
						}
					}
					else if (this.keyStateC == 1)
					{
						this.iaSources[3].Stop();
						this.keyStateC = 0;
						if (this.attemptedCrank)
						{
							this.iaSources[0].Play();
							this.ucar.GetComponent<car3>().turnOffAcc();
							this.attemptedCrank = false;
						}
					}
				}
				if (this.hit.collider.transform.name == "truckwheel")
				{
					if (this.depthgauge.active && Input.GetMouseButton(1))
					{
						float num6 = this.hit.collider.gameObject.GetComponent<PickUp>().thisDurability * 3.6f + 313f;
						if (num6 > 660f)
						{
							num6 = 660f;
						}
						Vector3 eulerAngles = new Vector3(this.deptharrow.transform.eulerAngles.x, this.deptharrow.transform.eulerAngles.y, num6);
						this.deptharrow.transform.eulerAngles = eulerAngles;
						this.paSources.PlayOneShot(this.playerAudio[13], 1f);
					}
					if (this.tiregauge.GetComponent<Renderer>().enabled || this.tirepump.GetComponent<Renderer>().enabled)
					{
						if (Input.GetMouseButton(1))
						{
							this.rimNum = 0;
							foreach (object obj4 in this.hit.collider.transform)
							{
								Transform transform4 = (Transform)obj4;
								int siblingIndex = transform4.GetSiblingIndex();
								if (siblingIndex > 0 && siblingIndex < 6 && transform4.gameObject.active)
								{
									this.rimNum = siblingIndex;
								}
								if (siblingIndex > 5 && transform4.gameObject.active)
								{
									this.tireX = this.hit.collider.transform.GetChild(siblingIndex).gameObject;
									break;
								}
							}
							if (this.tireX.GetComponent<Renderer>().enabled)
							{
								if (this.tiregauge.GetComponent<Renderer>().enabled)
								{
									this.deflation = this.hit.collider.gameObject.GetComponent<PickUp>().deflation;
									if (this.deflation < 1f)
									{
										this.deflation += 0.001f;
										this.hit.collider.gameObject.GetComponent<PickUp>().deflation = this.deflation;
										if (Time.time >= (float)this.nextUpdate)
										{
											this.screwdriver.GetComponent<AudioSource>().Play();
											this.nextUpdate = Mathf.FloorToInt(Time.time) + 1;
										}
									}
								}
								else
								{
									this.deflation = this.hit.collider.gameObject.GetComponent<PickUp>().deflation;
									if (this.deflation > 0f)
									{
										this.deflation -= 0.001f;
										this.hit.collider.gameObject.GetComponent<PickUp>().deflation = this.deflation;
										if (Time.time >= (float)this.nextUpdate)
										{
											this.tirepump.GetComponent<AudioSource>().Play();
											this.nextUpdate = Mathf.FloorToInt(Time.time) + 1;
										}
									}
								}
							}
						}
						else
						{
							this.screwdriver.GetComponent<AudioSource>().Stop();
							this.tirepump.GetComponent<AudioSource>().Stop();
						}
					}
				}
				if (this.hit.collider.transform.name == "BrakeDisk" && (this.tiregauge.GetComponent<Renderer>().enabled || this.tirepump.GetComponent<Renderer>().enabled || this.depthgauge.active))
				{
					if (Input.GetMouseButton(1))
					{
						if (this.hit.collider.transform.parent.name == "SteeringJointFL2")
						{
							this.wheelColX = this.wheelColFL;
							this.wheelHolderX = this.wheelHolderFL;
						}
						else if (this.hit.collider.transform.parent.name == "SteeringJointFR2")
						{
							this.wheelColX = this.wheelColFR;
							this.wheelHolderX = this.wheelHolderFR;
						}
						else if (this.hit.collider.transform.parent.name == "AxleRearLeft")
						{
							this.wheelColX = this.wheelColRL;
							this.wheelHolderX = this.wheelHolderRL;
						}
						else if (this.hit.collider.transform.parent.name == "AxleRearRight")
						{
							this.wheelColX = this.wheelColRR;
							this.wheelHolderX = this.wheelHolderRR;
						}
						else if (this.hit.collider.transform.parent.name == "SteeringJointFL2F")
						{
							this.wheelColX = this.wheelColFLF;
							this.wheelHolderX = this.wheelHolderFLF;
						}
						else if (this.hit.collider.transform.parent.name == "SteeringJointFR2F")
						{
							this.wheelColX = this.wheelColFRF;
							this.wheelHolderX = this.wheelHolderFRF;
						}
						else if (this.hit.collider.transform.parent.name == "AxleRearLeftF")
						{
							this.wheelColX = this.wheelColRLF;
							this.wheelHolderX = this.wheelHolderRLF;
						}
						else if (this.hit.collider.transform.parent.name == "AxleRearRightF")
						{
							this.wheelColX = this.wheelColRRF;
							this.wheelHolderX = this.wheelHolderRRF;
						}
						foreach (object obj5 in this.wheelHolderX.transform)
						{
							Transform transform5 = (Transform)obj5;
							int siblingIndex2 = transform5.GetSiblingIndex();
							if (siblingIndex2 > 5 && transform5.gameObject.active)
							{
								this.tireX = this.wheelHolderX.transform.GetChild(siblingIndex2).gameObject;
								break;
							}
						}
						if (this.tireX.GetComponent<Renderer>().enabled)
						{
							if (this.depthgauge.active)
							{
								float num7 = this.hit.collider.gameObject.GetComponent<durability>().health * 3.6f + 313f;
								if (num7 > 660f)
								{
									num7 = 660f;
								}
								Vector3 eulerAngles2 = new Vector3(this.deptharrow.transform.eulerAngles.x, this.deptharrow.transform.eulerAngles.y, num7);
								this.deptharrow.transform.eulerAngles = eulerAngles2;
								this.paSources.PlayOneShot(this.playerAudio[13], 1f);
							}
							if (this.tiregauge.GetComponent<Renderer>().enabled)
							{
								this.deflation = this.tireX.GetComponent<Renderer>().material.GetFloat("_TireFlatnessT");
								this.airmeterpos = this.percentageInflated * 0.48000000000000004 + 0.08;
								this.tiregaugemeter.GetComponent<InteractiveObject>().openPosition = new Vector3(0f, 0f, (float)this.airmeterpos);
								this.tiregaugemeter.GetComponent<InteractiveObject>().closedPosition = new Vector3(0f, 0f, (float)this.airmeterpos);
								this.tiregaugemeter.GetComponent<InteractiveObject>().PerformAction();
								if (this.deflation < 1f)
								{
									this.deflation += 0.001f;
									this.percentageInflated = (double)(1f - this.deflation);
									this.newRadius = this.percentageInflated * 0.11000000000000004 + 0.29;
									this.tireX.GetComponent<Renderer>().material.SetFloat("_TireFlatnessT", this.deflation);
									this.wheelColX.radius = (float)this.newRadius;
									if (Time.time >= (float)this.nextUpdate)
									{
										this.screwdriver.GetComponent<AudioSource>().Play();
										this.nextUpdate = Mathf.FloorToInt(Time.time) + 1;
										this.CalculateTraction();
									}
								}
							}
							else
							{
								this.deflation = this.tireX.GetComponent<Renderer>().material.GetFloat("_TireFlatnessT");
								if (this.deflation > 0f)
								{
									this.deflation -= 0.001f;
									this.percentageInflated = (double)(1f - this.deflation);
									this.newRadius = this.percentageInflated * 0.11000000000000004 + 0.29;
									this.tireX.GetComponent<Renderer>().material.SetFloat("_TireFlatnessT", this.deflation);
									this.wheelColX.radius = (float)this.newRadius;
									if (Time.time >= (float)this.nextUpdate)
									{
										this.tirepump.GetComponent<AudioSource>().Play();
										this.nextUpdate = Mathf.FloorToInt(Time.time) + 1;
										this.CalculateTraction();
									}
								}
							}
						}
					}
					else
					{
						this.screwdriver.GetComponent<AudioSource>().Stop();
						this.tirepump.GetComponent<AudioSource>().Stop();
					}
				}
				if (this.hit.collider.transform.name == "boltSusp")
				{
					if (this.emissiveBolt2 != null)
					{
						this.emissiveBolt2.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
					}
					this.emissiveBolt2 = this.hit.collider.gameObject;
					this.hit.collider.transform.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
					if (Input.GetAxis("Mouse ScrollWheel") > 0f)
					{
						this.hit.collider.transform.GetComponent<BoltScriptSusp>().Tighten();
						this.paSources.PlayOneShot(this.playerAudio[3], 0.5f);
						this.hit.collider.transform.Rotate(0f, 0f, 30f, Space.Self);
					}
					if (Input.GetAxis("Mouse ScrollWheel") < 0f)
					{
						this.hit.collider.transform.GetComponent<BoltScriptSusp>().Loosen();
						this.paSources.PlayOneShot(this.playerAudio[3], 0.5f);
						this.hit.collider.transform.Rotate(0f, 0f, -30f, Space.Self);
					}
				}
				else if (this.emissiveBolt2 != null)
				{
					this.emissiveBolt2.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
					this.emissiveBolt2 = null;
				}
				if (this.hit.collider.transform.name == "bolt" && this.hit.collider.transform.GetComponent<Renderer>().enabled)
				{
					if (this.emissiveBolt != null)
					{
						this.emissiveBolt.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
						foreach (object obj6 in this.emissiveBolt.transform.parent)
						{
							Transform transform6 = (Transform)obj6;
							if (transform6.name == "bolt")
							{
								transform6.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
							}
						}
					}
					this.emissiveBolt = this.hit.collider.gameObject;
					this.hit.collider.transform.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
					foreach (object obj7 in this.hit.collider.transform.parent)
					{
						Transform transform7 = (Transform)obj7;
						if (transform7.name == "bolt" && transform7.parent.gameObject.name != "BrakeDisk")
						{
							if (transform7 == this.hit.collider.transform)
							{
								transform7.GetComponent<Renderer>().material.SetColor("_EmissionColor", this.highlightColor);
								transform7.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
							}
							else
							{
								transform7.GetComponent<Renderer>().material.SetColor("_EmissionColor", this.highlightColorGroup);
								transform7.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
							}
						}
					}
					this.highlighted = true;
					this.aSource = this.hit.collider.transform.GetComponent<AudioSource>();
					this.numTurns = (float)this.hit.collider.transform.GetComponent<BoltScript>().boltturns;
					if (Input.GetAxis("Mouse ScrollWheel") > 0f && this.numTurns < 11f)
					{
						this.hit.collider.transform.Rotate(0f, 0f, -30f, Space.Self);
						this.hit.collider.transform.Translate(Vector3.forward * 0.002f);
						this.hit.collider.transform.GetComponent<BoltScript>().boltturns++;
						this.paSources.PlayOneShot(this.playerAudio[3], 0.5f);
						this.updatebolts = true;
						if (this.hit.collider.transform.parent.GetComponent<durability>() != null)
						{
							this.hit.collider.transform.parent.GetComponent<durability>().canDetach = false;
						}
						if (this.hit.collider.transform.parent.name == "engineblock" || this.hit.collider.transform.parent.name == "v8_block" || this.hit.collider.transform.parent.name == "i6block")
						{
							if (this.hit.collider.transform.parent.parent.name == "dirt pickup truck" || this.hit.collider.transform.parent.parent.name == "f1003" || this.hit.collider.transform.parent.parent.name == "amc")
							{
								this.hit.collider.transform.parent.GetComponent<PickUp>().pickable = false;
							}
						}
						else if (this.hit.collider.transform.parent.name == "250_block" && this.hit.collider.transform.parent.parent.name == "DirtBike")
						{
							this.hit.collider.transform.parent.GetComponent<PickUp>().pickable = false;
						}
					}
					if (Input.GetAxis("Mouse ScrollWheel") < 0f && this.numTurns > -1f)
					{
						this.hit.collider.transform.Rotate(0f, 0f, 30f, Space.Self);
						this.hit.collider.transform.Translate(-Vector3.forward * 0.002f);
						this.hit.collider.transform.GetComponent<BoltScript>().boltturns--;
						this.paSources.PlayOneShot(this.playerAudio[3], 0.5f);
						this.updatebolts = true;
						if (this.numTurns < 1f && this.hit.collider.transform.parent.name == "OilDrain")
						{
							this.hit.collider.transform.GetComponent<DrainOil>().enabled = true;
						}
						else if (this.numTurns < 1f && this.hit.collider.transform.parent.name == "OilDrainV8")
						{
							this.hit.collider.transform.GetComponent<DrainOilV8>().enabled = true;
						}
						else if (this.numTurns < 1f && this.hit.collider.transform.parent.name == "CoolantDrain")
						{
							this.hit.collider.transform.GetComponent<DrainCoolant>().enabled = true;
						}
						else if (this.numTurns < 1f && this.hit.collider.transform.parent.name == "CoolantDrainF")
						{
							this.hit.collider.transform.GetComponent<DrainCoolantF>().enabled = true;
						}
						else if (this.numTurns < 1f && this.hit.collider.transform.parent.name == "OilDrainI6")
						{
							this.hit.collider.transform.GetComponent<DrainOilI6>().enabled = true;
						}
					}
					if (this.updatebolts)
					{
						if (this.hit.collider.transform.parent.name != "OilDrain" && this.hit.collider.transform.parent.name != "OilDrainV8" && this.hit.collider.transform.parent.name != "OilDrainI6" && this.hit.collider.transform.parent.name != "CoolantDrain" && this.hit.collider.transform.parent.name != "CoolantDrainF")
						{
							int numBolts = this.hit.collider.transform.parent.GetComponent<durability>().numBolts;
							int childCount = this.hit.collider.transform.parent.childCount;
							int num8 = 0;
							for (int i = 0; i < childCount; i++)
							{
								if (this.hit.collider.transform.parent.GetChild(i).gameObject.name == "bolt")
								{
									num8 += this.hit.collider.transform.parent.GetChild(i).gameObject.GetComponent<BoltScript>().boltturns;
								}
							}
							if (numBolts > 0)
							{
								this.hit.collider.transform.parent.GetComponent<durability>().boltStr = (float)(num8 / numBolts);
							}
							if (num8 < 1)
							{
								this.hit.collider.transform.parent.GetComponent<durability>().canDetach = true;
								if (this.hit.collider.transform.parent.name == "engineblock" || this.hit.collider.transform.parent.name == "v8_block" || this.hit.collider.transform.parent.name == "i6block")
								{
									this.hit.collider.transform.parent.GetComponent<PickUp>().pickable = true;
								}
								else if (this.hit.collider.transform.parent.name == "250_block")
								{
									this.hit.collider.transform.parent.GetComponent<PickUp>().pickable = true;
								}
							}
						}
						this.updatebolts = false;
					}
				}
				else if (this.emissiveBolt != null)
				{
					this.emissiveBolt.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
					foreach (object obj8 in this.emissiveBolt.transform.parent)
					{
						Transform transform8 = (Transform)obj8;
						if (transform8.name == "bolt")
						{
							transform8.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
						}
					}
					this.emissiveBolt = null;
				}
				if (this.hit.collider.transform.GetComponent<PickUp>() != null)
				{
					this.interactiveObjectDesc = this.hit.collider.transform.GetComponent<PickUp>().description;
					this.interactiveObjectPrice = this.hit.collider.transform.GetComponent<PickUp>().price;
					if (this.interactiveObjectPrice > 0f && !this.hit.collider.transform.GetComponent<PickUp>().pickable)
					{
						this.interactiveObjectDesc = this.interactiveObjectDesc + " $" + this.interactiveObjectPrice;
					}
					if (this.hit.collider.transform.name == "MotorOil" || this.hit.collider.transform.name == "MotorOil(Clone)")
					{
						this.interactiveObjectDesc = "Motor Oil (" + this.hit.collider.transform.GetComponent<FluidHandler>().fluidlevel + ")";
					}
					else if (this.hit.collider.transform.name == "coolant" || this.hit.collider.transform.name == "coolant(Clone)")
					{
						this.interactiveObjectDesc = "Coolant (" + this.hit.collider.transform.GetComponent<FluidHandler>().fluidlevel + ")";
					}
					else if (this.hit.collider.transform.name == "twostroke" || this.hit.collider.transform.name == "twostroke(Clone)")
					{
						this.interactiveObjectDesc = "Two-Stroke Fuel (" + this.hit.collider.transform.GetComponent<FluidHandler>().fluidlevel + ")";
					}
				}
				else
				{
					this.interactiveObjectDesc = "";
				}
				if (this.hit.collider.transform.GetComponent<InteractiveObject>() != null)
				{
					this.interactiveObjectDesc = this.hit.collider.transform.GetComponent<InteractiveObject>().description;
				}
				if (this.hit.collider.transform.GetComponent<durability>() != null && (this.hit.collider.transform.GetComponent<Renderer>().enabled || this.hit.collider.gameObject.name == "turbo_e" || this.hit.collider.gameObject.name == "waterwheel_e"))
				{
					if (this.hit.collider.gameObject.name == "engineblock" || this.hit.collider.gameObject.name == "v8_block" || this.hit.collider.gameObject.name == "i6block")
					{
						this.fourcyl.GetComponent<durability>().canDetach = false;
						this.v8.GetComponent<durability>().canDetach = false;
						this.i6.GetComponent<durability>().canDetach = false;
					}
					this.reticleController.ShowDetachIcon(this.hit.collider.transform.GetComponent<durability>().canDetach);
				}
				else if (this.reticleController.detachIcon.activeSelf)
				{
					this.reticleController.ShowDetachIcon(false);
				}
			}
		}
		else
		{
			if (this.reticleController.detachIcon.activeSelf)
			{
				this.reticleController.ShowDetachIcon(false);
			}
			this.interactiveObject = null;
			this.interactiveObjectDesc = "";
		}
		this.reticleController.ShowIcon(this.interactiveObject);
		this.reticleController.ShowDescriptor(this.interactiveObjectDesc);
		if (this.holdJoint == null && this.pickedUpObject != null)
		{
			this.DropObject();
		}
		if (Input.GetKeyDown(this.cr.Interact) && this.hit.collider && !this.handcuffed)
		{
			this.colliderName = this.hit.collider.transform.name;
			string text = this.colliderName;
			uint num9 = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num9 > 2207031773U)
			{
				if (num9 > 3251581074U)
				{
					if (num9 <= 3759498163U)
					{
						if (num9 <= 3575973831U)
						{
							if (num9 <= 3396948428U)
							{
								if (num9 <= 3328766040U)
								{
									if (num9 != 3317592104U)
									{
										if (num9 != 3328766040U)
										{
											goto IL_7428;
										}
										if (!(text == "StationClerk"))
										{
											goto IL_7428;
										}
										this.MissionCharacter(13);
										goto IL_7441;
									}
									else
									{
										if (!(text == "carlights"))
										{
											goto IL_7428;
										}
										this.ToggleLightsCar();
										this.interactiveObject.PerformAction();
										goto IL_7441;
									}
								}
								else if (num9 != 3395266515U)
								{
									if (num9 != 3396948428U)
									{
										goto IL_7428;
									}
									if (!(text == "ignition_keyCar"))
									{
										goto IL_7428;
									}
									if (this.person.transform.parent != null && !this.amcImpounded)
									{
										this.KeyStateChangeCar();
										goto IL_7441;
									}
									goto IL_7441;
								}
								else
								{
									if (!(text == "trophyflyer"))
									{
										goto IL_7428;
									}
									this.TrophyFlyer();
									goto IL_7441;
								}
							}
							else if (num9 <= 3521705225U)
							{
								if (num9 != 3491258807U)
								{
									if (num9 != 3521705225U)
									{
										goto IL_7428;
									}
									if (!(text == "zen_shelf"))
									{
										goto IL_7428;
									}
									this.AddItem(8);
									this.hit.collider.gameObject.SetActive(false);
									goto IL_7441;
								}
								else
								{
									if (!(text == "bootlegger"))
									{
										goto IL_7428;
									}
									this.MissionCharacter(10);
									goto IL_7441;
								}
							}
							else if (num9 != 3545851038U)
							{
								if (num9 != 3554731717U)
								{
									if (num9 != 3575973831U)
									{
										goto IL_7428;
									}
									if (!(text == "valve_moon"))
									{
										goto IL_7428;
									}
									this.still.GetComponent<Still>().OnOff();
									this.interactiveObject.PerformAction();
									goto IL_7441;
								}
								else
								{
									if (!(text == "Couch"))
									{
										goto IL_7428;
									}
									goto IL_6963;
								}
							}
							else
							{
								if (!(text == "250_kickstarteropen"))
								{
									goto IL_7428;
								}
								this.dirtbike.GetComponent<Dirtbike>().Kick();
								goto IL_7441;
							}
						}
						else if (num9 <= 3675825724U)
						{
							if (num9 <= 3638454823U)
							{
								if (num9 != 3598365972U)
								{
									if (num9 != 3638454823U)
									{
										goto IL_7428;
									}
									if (!(text == "inner"))
									{
										goto IL_7428;
									}
									this.hit.collider.gameObject.GetComponent<vaultring>().Rotate();
									goto IL_7441;
								}
								else
								{
									if (!(text == "CorkBoard"))
									{
										goto IL_7428;
									}
									this.mg.TryCreateMission();
									goto IL_7441;
								}
							}
							else if (num9 != 3643696020U)
							{
								if (num9 != 3675825724U)
								{
									goto IL_7428;
								}
								if (!(text == "golfseat"))
								{
									goto IL_7428;
								}
								if (!this.phoneOn && !this.golfcartImpounded)
								{
									this.GetInCart();
									this.ShowToolTips(0);
									goto IL_7441;
								}
								goto IL_7441;
							}
							else if (!(text == "citation"))
							{
								goto IL_7428;
							}
						}
						else if (num9 <= 3692474291U)
						{
							if (num9 != 3690981797U)
							{
								if (num9 != 3692474291U)
								{
									goto IL_7428;
								}
								if (!(text == "spam_shelf"))
								{
									goto IL_7428;
								}
								this.AddItem(1);
								this.hit.collider.gameObject.SetActive(false);
								goto IL_7441;
							}
							else
							{
								if (!(text == "DirtBike"))
								{
									goto IL_7428;
								}
								if (!this.phoneOn && !this.dirtbikeImpounded)
								{
									this.GetInDirtbike();
									goto IL_7441;
								}
								goto IL_7441;
							}
						}
						else if (num9 != 3703074533U)
						{
							if (num9 != 3706078298U)
							{
								if (num9 != 3759498163U)
								{
									goto IL_7428;
								}
								if (!(text == "JackMech"))
								{
									goto IL_7428;
								}
								this.JackDown();
								goto IL_7441;
							}
							else
							{
								if (!(text == "fireman5"))
								{
									goto IL_7428;
								}
								this.MissionCharacter(5);
								goto IL_7441;
							}
						}
						else
						{
							if (!(text == "businessman"))
							{
								goto IL_7428;
							}
							this.MissionCharacter(1);
							goto IL_7441;
						}
					}
					else if (num9 <= 4133960905U)
					{
						if (num9 <= 3999115853U)
						{
							if (num9 <= 3967719375U)
							{
								if (num9 != 3804013108U)
								{
									if (num9 != 3967719375U)
									{
										goto IL_7428;
									}
									if (!(text == "recipe"))
									{
										goto IL_7428;
									}
									this.MoonManual2();
									goto IL_7441;
								}
								else
								{
									if (!(text == "jimmyjunks"))
									{
										goto IL_7428;
									}
									this.MissionCharacter(3);
									goto IL_7441;
								}
							}
							else if (num9 != 3989202994U)
							{
								if (num9 != 3999115853U)
								{
									goto IL_7428;
								}
								if (!(text == "tobaccoplant"))
								{
									goto IL_7428;
								}
								this.HarvestTobacco();
								goto IL_7441;
							}
							else
							{
								if (!(text == "OffSwitch"))
								{
									goto IL_7428;
								}
								this.dirtbike.GetComponent<Dirtbike>().ShutOff();
								goto IL_7441;
							}
						}
						else if (num9 <= 4066341216U)
						{
							if (num9 != 4063049472U)
							{
								if (num9 != 4066341216U)
								{
									goto IL_7428;
								}
								if (!(text == "jake8"))
								{
									goto IL_7428;
								}
								this.MissionCharacter(9);
								goto IL_7441;
							}
							else
							{
								if (!(text == "JailBed"))
								{
									goto IL_7428;
								}
								goto IL_6963;
							}
						}
						else if (num9 != 4072270541U)
						{
							if (num9 != 4093320674U)
							{
								if (num9 != 4133960905U)
								{
									goto IL_7428;
								}
								if (!(text == "Maquina_1"))
								{
									goto IL_7428;
								}
								this.PayForItems();
								goto IL_7441;
							}
							else
							{
								if (!(text == "diffswitch"))
								{
									goto IL_7428;
								}
								this.carscript.DiffLock();
								this.interactiveObject.PerformAction();
								goto IL_7441;
							}
						}
						else
						{
							if (!(text == "DebugCube"))
							{
								goto IL_7428;
							}
							this.BuyTruck(1);
							goto IL_7441;
						}
					}
					else if (num9 <= 4189685193U)
					{
						if (num9 <= 4164397004U)
						{
							if (num9 != 4142808962U)
							{
								if (num9 != 4164397004U)
								{
									goto IL_7428;
								}
								if (!(text == "beerbottles(Clone)"))
								{
									goto IL_7428;
								}
								goto IL_6B8F;
							}
							else
							{
								if (!(text == "fuego_shelf"))
								{
									goto IL_7428;
								}
								this.AddItem(2);
								this.hit.collider.gameObject.SetActive(false);
								goto IL_7441;
							}
						}
						else if (num9 != 4167867895U)
						{
							if (num9 != 4189685193U)
							{
								goto IL_7428;
							}
							if (!(text == "f100seat"))
							{
								goto IL_7428;
							}
							if (!this.phoneOn)
							{
								this.GetInF();
								this.ShowToolTips(0);
								goto IL_7441;
							}
							goto IL_7441;
						}
						else
						{
							if (!(text == "headlightsF100switch"))
							{
								goto IL_7428;
							}
							this.ToggleLightsF();
							this.interactiveObject.PerformAction();
							goto IL_7441;
						}
					}
					else if (num9 <= 4217250954U)
					{
						if (num9 != 4193326624U)
						{
							if (num9 != 4217250954U)
							{
								goto IL_7428;
							}
							if (!(text == "beefareeno_shelf"))
							{
								goto IL_7428;
							}
							this.AddItem(3);
							this.hit.collider.gameObject.SetActive(false);
							goto IL_7441;
						}
						else if (!(text == "citation(Clone)"))
						{
							goto IL_7428;
						}
					}
					else if (num9 != 4223456829U)
					{
						if (num9 != 4241530181U)
						{
							if (num9 != 4280066247U)
							{
								goto IL_7428;
							}
							if (!(text == "f100_pdoor"))
							{
								goto IL_7428;
							}
							base.StartCoroutine(this.InteractPassDoor());
							goto IL_7441;
						}
						else
						{
							if (!(text == "pc_screen2"))
							{
								goto IL_7428;
							}
							this.pcCanvas.SetActive(true);
							this.mail.Init();
							this.fpc.UnlockMouse();
							Cursor.visible = true;
							Cursor.lockState = CursorLockMode.None;
							goto IL_7441;
						}
					}
					else
					{
						if (!(text == "garagedooropener(Clone)"))
						{
							goto IL_7428;
						}
						goto IL_6A43;
					}
					this.ViewCitation();
					goto IL_7441;
				}
				if (num9 <= 2690091062U)
				{
					if (num9 <= 2492352487U)
					{
						if (num9 <= 2364885338U)
						{
							if (num9 <= 2264846670U)
							{
								if (num9 != 2244414884U)
								{
									if (num9 != 2264846670U)
									{
										goto IL_7428;
									}
									if (!(text == "cram(Clone)"))
									{
										goto IL_7428;
									}
									this.Eat(2);
									goto IL_7441;
								}
								else
								{
									if (!(text == "DK_9"))
									{
										goto IL_7428;
									}
									base.StartCoroutine(this.RotateEngineStand());
									goto IL_7441;
								}
							}
							else if (num9 != 2331974213U)
							{
								if (num9 != 2364885338U)
								{
									goto IL_7428;
								}
								if (!(text == "raceflyer"))
								{
									goto IL_7428;
								}
								this.RaceFlyer();
								goto IL_7441;
							}
							else
							{
								if (!(text == "barstool"))
								{
									goto IL_7428;
								}
								this.pokerScript.BuyIn();
								this.ShowToolTips(5);
								goto IL_7441;
							}
						}
						else if (num9 <= 2404725321U)
						{
							if (num9 != 2403103463U)
							{
								if (num9 != 2404725321U)
								{
									goto IL_7428;
								}
								if (!(text == "CallPanel"))
								{
									goto IL_7428;
								}
								this.pokerScript.Call();
								goto IL_7441;
							}
							else
							{
								if (!(text == "orangeTree"))
								{
									goto IL_7428;
								}
								goto IL_73C3;
							}
						}
						else if (num9 != 2431734851U)
						{
							if (num9 != 2488759634U)
							{
								if (num9 != 2492352487U)
								{
									goto IL_7428;
								}
								if (!(text == "recyclecheckwood"))
								{
									goto IL_7428;
								}
								this.SellWood();
								this.interactiveObject.PerformAction();
								goto IL_7441;
							}
							else
							{
								if (!(text == "cardoor_p"))
								{
									goto IL_7428;
								}
								base.StartCoroutine(this.InteractPassDoor());
								goto IL_7441;
							}
						}
						else
						{
							if (!(text == "cornmeal_shelf"))
							{
								goto IL_7428;
							}
							this.AddItem(5);
							this.hit.collider.gameObject.SetActive(false);
							goto IL_7441;
						}
					}
					else if (num9 <= 2519771727U)
					{
						if (num9 <= 2506297079U)
						{
							if (num9 != 2503443670U)
							{
								if (num9 != 2506297079U)
								{
									goto IL_7428;
								}
								if (!(text == "moonshiner"))
								{
									goto IL_7428;
								}
								this.MissionCharacter(7);
								goto IL_7441;
							}
							else
							{
								if (!(text == "ForSaleSign"))
								{
									goto IL_7428;
								}
								this.bh.BuyH();
								this.interactiveObject.PerformAction();
								goto IL_7441;
							}
						}
						else if (num9 != 2509077982U)
						{
							if (num9 != 2519771727U)
							{
								goto IL_7428;
							}
							if (!(text == "racenpc"))
							{
								goto IL_7428;
							}
							this.MissionCharacter(8);
							goto IL_7441;
						}
						else
						{
							if (!(text == "recyclecheck"))
							{
								goto IL_7428;
							}
							this.RecycleJunk();
							goto IL_7441;
						}
					}
					else if (num9 <= 2541947918U)
					{
						if (num9 != 2525264294U)
						{
							if (num9 != 2541947918U)
							{
								goto IL_7428;
							}
							if (!(text == "250manual"))
							{
								goto IL_7428;
							}
							this.twofiftyManual();
							goto IL_7441;
						}
						else if (!(text == "opener_e"))
						{
							goto IL_7428;
						}
					}
					else if (num9 != 2559627270U)
					{
						if (num9 != 2602667253U)
						{
							if (num9 != 2690091062U)
							{
								goto IL_7428;
							}
							if (!(text == "cardoor_d"))
							{
								goto IL_7428;
							}
							base.StartCoroutine(this.InteractDrivDoor());
							goto IL_7441;
						}
						else
						{
							if (!(text == "pc_screen"))
							{
								goto IL_7428;
							}
							this.pcCanvas.SetActive(true);
							this.mail.Init();
							this.fpc.UnlockMouse();
							Cursor.visible = true;
							Cursor.lockState = CursorLockMode.None;
							this.myPC.GetComponent<AudioSource>().Play();
							this.myPCtower.GetComponent<AudioSource>().Play();
							this.timeOfDay = EnviroSkyMgr.instance.GetTimeOfDay();
							this.weatherId = EnviroSkyMgr.instance.GetWeatherID();
							goto IL_7441;
						}
					}
					else
					{
						if (!(text == "valve_faucet"))
						{
							goto IL_7428;
						}
						this.hit.collider.gameObject.GetComponent<Faucet>().TurnOnOff();
						this.interactiveObject.PerformAction();
						goto IL_7441;
					}
				}
				else if (num9 <= 2899577335U)
				{
					if (num9 <= 2737739993U)
					{
						if (num9 <= 2718680436U)
						{
							if (num9 != 2696799091U)
							{
								if (num9 != 2718680436U)
								{
									goto IL_7428;
								}
								if (!(text == "outer"))
								{
									goto IL_7428;
								}
								this.hit.collider.gameObject.GetComponent<vaultring>().Rotate();
								goto IL_7441;
							}
							else
							{
								if (!(text == "ambrosiaTree"))
								{
									goto IL_7428;
								}
								goto IL_73C3;
							}
						}
						else if (num9 != 2726788986U)
						{
							if (num9 != 2737739993U)
							{
								goto IL_7428;
							}
							if (!(text == "watertowervalve"))
							{
								goto IL_7428;
							}
							if (this.overflowvalve == 1)
							{
								this.WaterTower1();
								this.interactiveObject.PerformAction();
								goto IL_7441;
							}
							goto IL_7441;
						}
						else
						{
							if (!(text == "InspectionResult"))
							{
								goto IL_7428;
							}
							this.inspectionCanvas.SetActive(true);
							this.fpc.UnlockMouse();
							Cursor.visible = true;
							Cursor.lockState = CursorLockMode.None;
							base.GetComponent<FirstPersonController>().enabled = false;
							goto IL_7441;
						}
					}
					else if (num9 <= 2747052071U)
					{
						if (num9 != 2743270691U)
						{
							if (num9 != 2747052071U)
							{
								goto IL_7428;
							}
							if (!(text == "hookw"))
							{
								goto IL_7428;
							}
							if (this.winchHook.GetComponent<Renderer>().enabled)
							{
								this.GrabHook();
								this.interactiveObject.PerformAction();
								goto IL_7441;
							}
							goto IL_7441;
						}
						else
						{
							if (!(text == "12pack"))
							{
								goto IL_7428;
							}
							goto IL_6B84;
						}
					}
					else if (num9 != 2848326284U)
					{
						if (num9 != 2870468478U)
						{
							if (num9 != 2899577335U)
							{
								goto IL_7428;
							}
							if (!(text == "f1004wd"))
							{
								goto IL_7428;
							}
							this.Enable4wF();
							this.interactiveObject.PerformAction();
							goto IL_7441;
						}
						else
						{
							if (!(text == "moonmanual"))
							{
								goto IL_7428;
							}
							this.MoonManual();
							this.eventSystem.GetComponent<MissionController>().CompleteMission(44);
							goto IL_7441;
						}
					}
					else
					{
						if (!(text == "Electr_outlet2"))
						{
							goto IL_7428;
						}
						goto IL_7441;
					}
				}
				else if (num9 <= 3049047885U)
				{
					if (num9 <= 2964476867U)
					{
						if (num9 != 2947259091U)
						{
							if (num9 != 2964476867U)
							{
								goto IL_7428;
							}
							if (!(text == "garagedooropener"))
							{
								goto IL_7428;
							}
						}
						else
						{
							if (!(text == "f100_ddoor"))
							{
								goto IL_7428;
							}
							base.StartCoroutine(this.InteractDrivDoor());
							goto IL_7441;
						}
					}
					else if (num9 != 2978308058U)
					{
						if (num9 != 3049047885U)
						{
							goto IL_7428;
						}
						if (!(text == "creeper"))
						{
							goto IL_7428;
						}
						this.LayOnCreeper();
						goto IL_7441;
					}
					else
					{
						if (!(text == "hitchG"))
						{
							goto IL_7428;
						}
						this.HitchTrailerG();
						goto IL_7441;
					}
				}
				else if (num9 <= 3092228396U)
				{
					if (num9 != 3072450280U)
					{
						if (num9 != 3092228396U)
						{
							goto IL_7428;
						}
						if (!(text == "diffswitchF"))
						{
							goto IL_7428;
						}
						this.fcarscript.DiffLock();
						this.interactiveObject.PerformAction();
						goto IL_7441;
					}
					else
					{
						if (!(text == "zencan"))
						{
							goto IL_7428;
						}
						goto IL_6B9A;
					}
				}
				else if (num9 != 3111696742U)
				{
					if (num9 != 3179732148U)
					{
						if (num9 != 3251581074U)
						{
							goto IL_7428;
						}
						if (!(text == "recyclecheckore"))
						{
							goto IL_7428;
						}
						this.SellOre();
						this.interactiveObject.PerformAction();
						goto IL_7441;
					}
					else
					{
						if (!(text == "Mech_single_rigged"))
						{
							goto IL_7428;
						}
						this.MissionCharacter(12);
						goto IL_7441;
					}
				}
				else
				{
					if (!(text == "dooropener"))
					{
						goto IL_7428;
					}
					goto IL_7441;
				}
				IL_6A43:
				this.mgd.UseDoor();
				goto IL_7441;
			}
			if (num9 <= 842836352U)
			{
				if (num9 <= 411858056U)
				{
					if (num9 <= 97338704U)
					{
						if (num9 <= 39742388U)
						{
							if (num9 <= 16901259U)
							{
								if (num9 != 2370319U)
								{
									if (num9 != 16901259U)
									{
										goto IL_7428;
									}
									if (!(text == "NiceBed"))
									{
										goto IL_7428;
									}
								}
								else
								{
									if (!(text == "Lightswitch"))
									{
										goto IL_7428;
									}
									this.hit.collider.gameObject.GetComponent<Lightswitch>().Switch();
									this.interactiveObject.PerformAction();
									this.paSources.PlayOneShot(this.playerAudio[16], 1f);
									goto IL_7441;
								}
							}
							else if (num9 != 23038205U)
							{
								if (num9 != 39742388U)
								{
									goto IL_7428;
								}
								if (!(text == "pickaxe"))
								{
									goto IL_7428;
								}
								Object.Destroy(this.hit.collider.gameObject);
								this.SwitchItem();
								this.pickaxe.GetComponent<Renderer>().enabled = true;
								goto IL_7441;
							}
							else
							{
								if (!(text == "jake7"))
								{
									goto IL_7428;
								}
								this.MissionCharacter(6);
								goto IL_7441;
							}
						}
						else if (num9 <= 75698516U)
						{
							if (num9 != 70818468U)
							{
								if (num9 != 75698516U)
								{
									goto IL_7428;
								}
								if (!(text == "buytruck2"))
								{
									goto IL_7428;
								}
								this.BuyTruck(2);
								goto IL_7441;
							}
							else
							{
								if (!(text == "JackHandle"))
								{
									goto IL_7428;
								}
								this.JackUp();
								goto IL_7441;
							}
						}
						else if (num9 != 93078660U)
						{
							if (num9 != 97338704U)
							{
								goto IL_7428;
							}
							if (!(text == "FSManual"))
							{
								goto IL_7428;
							}
							this.OpenFSM();
							goto IL_7441;
						}
						else
						{
							if (!(text == "center"))
							{
								goto IL_7428;
							}
							this.hit.collider.gameObject.GetComponent<vaultring>().Push();
							goto IL_7441;
						}
					}
					else if (num9 <= 215997667U)
					{
						if (num9 <= 149754865U)
						{
							if (num9 != 126031373U)
							{
								if (num9 != 149754865U)
								{
									goto IL_7428;
								}
								if (!(text == "Box081"))
								{
									goto IL_7428;
								}
								this.CellTower1();
								this.interactiveObject.PerformAction();
								goto IL_7441;
							}
							else
							{
								if (!(text == "buytruck1"))
								{
									goto IL_7428;
								}
								this.BuyTruck(1);
								goto IL_7441;
							}
						}
						else if (num9 != 188359500U)
						{
							if (num9 != 215997667U)
							{
								goto IL_7428;
							}
							if (!(text == "payphone"))
							{
								goto IL_7428;
							}
							this.PayPhone();
							goto IL_7441;
						}
						else
						{
							if (!(text == "ignition_keyF"))
							{
								goto IL_7428;
							}
							if (this.person.transform.parent != null && !this.f100Impounded)
							{
								this.KeyStateChangeF();
								goto IL_7441;
							}
							goto IL_7441;
						}
					}
					else if (num9 <= 360074630U)
					{
						if (num9 != 341931903U)
						{
							if (num9 != 360074630U)
							{
								goto IL_7428;
							}
							if (!(text == "johnnysale"))
							{
								goto IL_7428;
							}
							this.JohnnyInteract();
							goto IL_7441;
						}
						else
						{
							if (!(text == "atm"))
							{
								goto IL_7428;
							}
							this.atmMenu.SetActive(true);
							this.fpc.UnlockMouse();
							Cursor.visible = true;
							Cursor.lockState = CursorLockMode.None;
							base.GetComponent<FirstPersonController>().enabled = false;
							this.atmMenu.transform.GetChild(0).GetComponent<Atm>().Start();
							goto IL_7441;
						}
					}
					else if (num9 != 372328835U)
					{
						if (num9 != 374988360U)
						{
							if (num9 != 411858056U)
							{
								goto IL_7428;
							}
							if (!(text == "switch2_low"))
							{
								goto IL_7428;
							}
							this.Enable4w();
							this.interactiveObject.PerformAction();
							goto IL_7441;
						}
						else
						{
							if (!(text == "rgbswitch"))
							{
								goto IL_7428;
							}
							this.RgbAdjust(1);
							goto IL_7441;
						}
					}
					else
					{
						if (!(text == "ceiling_lamp"))
						{
							goto IL_7428;
						}
						GameObject.Find("GarageDoor01Animated1").GetComponent<gate>().SetOpen();
						goto IL_7441;
					}
				}
				else if (num9 <= 598057431U)
				{
					if (num9 <= 511099709U)
					{
						if (num9 <= 498794467U)
						{
							if (num9 != 421990911U)
							{
								if (num9 != 498794467U)
								{
									goto IL_7428;
								}
								if (!(text == "hitch_c"))
								{
									goto IL_7428;
								}
								this.HitchTrailerC();
								goto IL_7441;
							}
							else
							{
								if (!(text == "InspectionPaper"))
								{
									goto IL_7428;
								}
								this.MissionCharacter(2);
								goto IL_7441;
							}
						}
						else if (num9 != 501875436U)
						{
							if (num9 != 511099709U)
							{
								goto IL_7428;
							}
							if (!(text == "trucklights"))
							{
								goto IL_7428;
							}
							this.ToggleLights();
							this.interactiveObject.PerformAction();
							goto IL_7441;
						}
						else
						{
							if (!(text == "limeTree"))
							{
								goto IL_7428;
							}
							goto IL_73C3;
						}
					}
					else if (num9 <= 549279746U)
					{
						if (num9 != 514459595U)
						{
							if (num9 != 549279746U)
							{
								goto IL_7428;
							}
							if (!(text == "fillvalve"))
							{
								goto IL_7428;
							}
							this.interactiveObject.PerformAction();
							this.waterValve.TurnOnOff();
							goto IL_7441;
						}
						else
						{
							if (!(text == "ForSale"))
							{
								goto IL_7428;
							}
							if (this.currency.money >= 1500f)
							{
								this.inv.SubtractMoney(1500f);
								float num10 = this.currency.money - 1500f;
								this.currency.money = Mathf.Round(num10 * 100f) / 100f;
								this.bar.BuyContainer();
								goto IL_7441;
							}
							goto IL_7441;
						}
					}
					else if (num9 != 552944939U)
					{
						if (num9 != 587835571U)
						{
							if (num9 != 598057431U)
							{
								goto IL_7428;
							}
							if (!(text == "energycan(Clone)"))
							{
								goto IL_7428;
							}
							this.DrinkNrg();
							goto IL_7441;
						}
						else
						{
							if (!(text == "hookwF"))
							{
								goto IL_7428;
							}
							if (this.winchHookF.GetComponent<Renderer>().enabled)
							{
								this.GrabHookF();
								this.interactiveObject.PerformAction();
								goto IL_7441;
							}
							goto IL_7441;
						}
					}
					else
					{
						if (!(text == "RaisePanel"))
						{
							goto IL_7428;
						}
						this.pokerScript.Raise();
						goto IL_7441;
					}
				}
				else if (num9 <= 653042776U)
				{
					if (num9 <= 644042903U)
					{
						if (num9 != 641208010U)
						{
							if (num9 != 644042903U)
							{
								goto IL_7428;
							}
							if (!(text == "cop8"))
							{
								goto IL_7428;
							}
							this.ImpoundLot();
							goto IL_7441;
						}
						else
						{
							if (!(text == "yeast_shelf"))
							{
								goto IL_7428;
							}
							this.AddItem(6);
							this.hit.collider.gameObject.SetActive(false);
							goto IL_7441;
						}
					}
					else if (num9 != 649793038U)
					{
						if (num9 != 653042776U)
						{
							goto IL_7428;
						}
						if (!(text == "beerbottles"))
						{
							goto IL_7428;
						}
						goto IL_6B8F;
					}
					else
					{
						if (!(text == "hitch_h"))
						{
							goto IL_7428;
						}
						this.HitchTrailerH();
						goto IL_7441;
					}
				}
				else if (num9 <= 683817758U)
				{
					if (num9 != 658379174U)
					{
						if (num9 != 683817758U)
						{
							goto IL_7428;
						}
						if (!(text == "sugar_shelf"))
						{
							goto IL_7428;
						}
						this.AddItem(4);
						this.hit.collider.gameObject.SetActive(false);
						goto IL_7441;
					}
					else
					{
						if (!(text == "spraypaint"))
						{
							goto IL_7428;
						}
						this.PaintSurface();
						goto IL_7441;
					}
				}
				else if (num9 != 722352713U)
				{
					if (num9 != 776798965U)
					{
						if (num9 != 842836352U)
						{
							goto IL_7428;
						}
						if (!(text == "diphandleD"))
						{
							goto IL_7428;
						}
						this.CheckOil(2);
						goto IL_7441;
					}
					else
					{
						if (!(text == "airpump"))
						{
							goto IL_7428;
						}
						this.tirepump.GetComponent<Renderer>().enabled = !this.tirepump.GetComponent<Renderer>().enabled;
						this.holdingAirPump = !this.holdingAirPump;
						goto IL_7441;
					}
				}
				else
				{
					if (!(text == "Fuego"))
					{
						goto IL_7428;
					}
					this.Drink(1);
					goto IL_7441;
				}
			}
			else if (num9 <= 1667418734U)
			{
				if (num9 <= 1071706891U)
				{
					if (num9 <= 892921929U)
					{
						if (num9 <= 876391590U)
						{
							if (num9 != 870356221U)
							{
								if (num9 != 876391590U)
								{
									goto IL_7428;
								}
								if (!(text == "diphandleF"))
								{
									goto IL_7428;
								}
								this.CheckOil(3);
								goto IL_7441;
							}
							else
							{
								if (!(text == "radio_low"))
								{
									goto IL_7428;
								}
								this.radio.Toggle();
								goto IL_7441;
							}
						}
						else if (num9 != 888138491U)
						{
							if (num9 != 892921929U)
							{
								goto IL_7428;
							}
							if (!(text == "GardenHose"))
							{
								goto IL_7428;
							}
							this.hit.collider.gameObject.GetComponent<GardenHose>().Toggle();
							goto IL_7441;
						}
						else
						{
							if (!(text == "Bag(Clone)"))
							{
								goto IL_7428;
							}
							this.hit.collider.gameObject.GetComponent<GroceryBag>().OpenBag();
							goto IL_7441;
						}
					}
					else if (num9 <= 926724447U)
					{
						if (num9 != 921200170U)
						{
							if (num9 != 926724447U)
							{
								goto IL_7428;
							}
							if (!(text == "diphandleA"))
							{
								goto IL_7428;
							}
							this.CheckOil(1);
							goto IL_7441;
						}
						else
						{
							if (!(text == "FixPaper"))
							{
								goto IL_7428;
							}
							this.MissionCharacter(4);
							goto IL_7441;
						}
					}
					else if (num9 != 1025794197U)
					{
						if (num9 != 1060945399U)
						{
							if (num9 != 1071706891U)
							{
								goto IL_7428;
							}
							if (!(text == "beefareeno(Clone)"))
							{
								goto IL_7428;
							}
							this.Eat(1);
							goto IL_7441;
						}
						else
						{
							if (!(text == "diphandleI"))
							{
								goto IL_7428;
							}
							this.CheckOil(4);
							goto IL_7441;
						}
					}
					else
					{
						if (!(text == "weightbench"))
						{
							goto IL_7428;
						}
						this.bench.Bench();
						goto IL_7441;
					}
				}
				else if (num9 <= 1119300062U)
				{
					if (num9 <= 1098101706U)
					{
						if (num9 != 1097879392U)
						{
							if (num9 != 1098101706U)
							{
								goto IL_7428;
							}
							if (!(text == "LotteryTicket"))
							{
								goto IL_7428;
							}
							this.Lotto();
							goto IL_7441;
						}
						else
						{
							if (!(text == "seat_left"))
							{
								goto IL_7428;
							}
							if (!this.phoneOn)
							{
								this.GetIn();
								this.ShowToolTips(0);
								goto IL_7441;
							}
							goto IL_7441;
						}
					}
					else if (num9 != 1103573500U)
					{
						if (num9 != 1119300062U)
						{
							goto IL_7428;
						}
						if (!(text == "LotteryTicket(Clone)"))
						{
							goto IL_7428;
						}
						this.Lotto();
						goto IL_7441;
					}
					else
					{
						if (!(text == "zencan(Clone)"))
						{
							goto IL_7428;
						}
						goto IL_6B9A;
					}
				}
				else if (num9 <= 1568195178U)
				{
					if (num9 != 1310887608U)
					{
						if (num9 != 1568195178U)
						{
							goto IL_7428;
						}
						if (!(text == "rgbswitchf"))
						{
							goto IL_7428;
						}
						this.RgbAdjust(2);
						goto IL_7441;
					}
					else
					{
						if (!(text == "raceflyer2"))
						{
							goto IL_7428;
						}
						this.RaceFlyer2();
						goto IL_7441;
					}
				}
				else if (num9 != 1617767604U)
				{
					if (num9 != 1626510552U)
					{
						if (num9 != 1667418734U)
						{
							goto IL_7428;
						}
						if (!(text == "energy_shelf"))
						{
							goto IL_7428;
						}
						this.AddItem(7);
						this.hit.collider.gameObject.SetActive(false);
						goto IL_7441;
					}
					else
					{
						if (!(text == "FoldPanel"))
						{
							goto IL_7428;
						}
						this.pokerScript.Fold();
						goto IL_7441;
					}
				}
				else
				{
					if (!(text == "tailgate"))
					{
						goto IL_7428;
					}
					this.truckBed.EmptyBed();
					this.interactiveObject.PerformAction();
					goto IL_7441;
				}
			}
			else if (num9 <= 1984855396U)
			{
				if (num9 <= 1884322920U)
				{
					if (num9 <= 1737740910U)
					{
						if (num9 != 1691622651U)
						{
							if (num9 != 1737740910U)
							{
								goto IL_7428;
							}
							if (!(text == "RampObj"))
							{
								goto IL_7428;
							}
							this.trailerBedH.Constrain();
							this.trailerBedH.CheckBed();
							this.interactiveObject.PerformAction();
							goto IL_7441;
						}
						else
						{
							if (!(text == "kcswitch"))
							{
								goto IL_7428;
							}
							this.carscript.KcLight();
							this.interactiveObject.PerformAction();
							goto IL_7441;
						}
					}
					else if (num9 != 1779083341U)
					{
						if (num9 != 1884322920U)
						{
							goto IL_7428;
						}
						if (!(text == "seat"))
						{
							goto IL_7428;
						}
						if (!this.phoneOn)
						{
							this.GetInCar();
							this.ShowToolTips(0);
							goto IL_7441;
						}
						goto IL_7441;
					}
					else
					{
						if (!(text == "hose_tank"))
						{
							goto IL_7428;
						}
						this.hit.collider.gameObject.GetComponent<GardenHose2>().Toggle();
						goto IL_7441;
					}
				}
				else if (num9 <= 1926232514U)
				{
					if (num9 != 1909065151U)
					{
						if (num9 != 1926232514U)
						{
							goto IL_7428;
						}
						if (!(text == "contractor"))
						{
							goto IL_7428;
						}
						this.MissionCharacter(11);
						goto IL_7441;
					}
					else
					{
						if (!(text == "Chainsaw2"))
						{
							goto IL_7428;
						}
						this.StartChainsaw();
						goto IL_7441;
					}
				}
				else if (num9 != 1947136185U)
				{
					if (num9 != 1956264140U)
					{
						if (num9 != 1984855396U)
						{
							goto IL_7428;
						}
						if (!(text == "recyclechecktrash"))
						{
							goto IL_7428;
						}
						this.RecycleTrash();
						goto IL_7441;
					}
					else
					{
						if (!(text == "EngRelease"))
						{
							goto IL_7428;
						}
						this.EngReleaseStand();
						goto IL_7441;
					}
				}
				else
				{
					if (!(text == "hitch"))
					{
						goto IL_7428;
					}
					this.HitchTrailer();
					goto IL_7441;
				}
			}
			else if (num9 <= 2078510914U)
			{
				if (num9 <= 1994997127U)
				{
					if (num9 != 1986976571U)
					{
						if (num9 != 1994997127U)
						{
							goto IL_7428;
						}
						if (!(text == "kcswitchF"))
						{
							goto IL_7428;
						}
						this.fcarscript.KcLight();
						this.interactiveObject.PerformAction();
						goto IL_7441;
					}
					else
					{
						if (!(text == "weldergarage"))
						{
							goto IL_7428;
						}
						this.weldgun.GetComponent<Renderer>().enabled = !this.weldgun.GetComponent<Renderer>().enabled;
						this.holdingWelder = !this.holdingWelder;
						if (this.holdingWelder)
						{
							this.welderVisual.SetActive(false);
							goto IL_7441;
						}
						this.welderVisual.SetActive(true);
						goto IL_7441;
					}
				}
				else if (num9 != 1995782556U)
				{
					if (num9 != 2078510914U)
					{
						goto IL_7428;
					}
					if (!(text == "ignition_key"))
					{
						goto IL_7428;
					}
					if (this.person.transform.parent != null && !this.diamondbackImpounded)
					{
						this.KeyStateChange();
						goto IL_7441;
					}
					goto IL_7441;
				}
				else
				{
					if (!(text == "Lift"))
					{
						goto IL_7428;
					}
					this.hydraulicLift.UseLift();
					goto IL_7441;
				}
			}
			else if (num9 <= 2124116304U)
			{
				if (num9 != 2096963376U)
				{
					if (num9 != 2124116304U)
					{
						goto IL_7428;
					}
					if (!(text == "narco"))
					{
						goto IL_7428;
					}
					this.RemoveNarco();
					goto IL_7441;
				}
				else
				{
					if (!(text == "paintmachine"))
					{
						goto IL_7428;
					}
					this.RgbAdjustPaint();
					goto IL_7441;
				}
			}
			else if (num9 != 2161145173U)
			{
				if (num9 != 2187318174U)
				{
					if (num9 != 2207031773U)
					{
						goto IL_7428;
					}
					if (!(text == "12pack(Clone)"))
					{
						goto IL_7428;
					}
					goto IL_6B84;
				}
				else
				{
					if (!(text == "blackberryBush"))
					{
						goto IL_7428;
					}
					goto IL_73C3;
				}
			}
			else
			{
				if (!(text == "tiremanual"))
				{
					goto IL_7428;
				}
				this.TireManual();
				goto IL_7441;
			}
			IL_6963:
			this.AdvanceTime();
			goto IL_7441;
			IL_6B84:
			this.DrinkBeer();
			goto IL_7441;
			IL_6B8F:
			this.DrinkBeer2();
			goto IL_7441;
			IL_6B9A:
			this.Chew();
			goto IL_7441;
			IL_73C3:
			this.interactiveObject.PerformAction();
			this.paSources.PlayOneShot(this.playerAudio[8], 1f);
			goto IL_7441;
			IL_7428:
			if (this.interactiveObject != null)
			{
				this.interactiveObject.PerformAction();
			}
		}
		IL_7441:
		if (this.raceActive)
		{
			if (this.targetPositionTranform == null)
			{
				if (this.racenpc.GetComponent<RaceNpc>().whichRace == 0)
				{
					this.targetPositionTranform = GameObject.Find("target").transform;
				}
				else
				{
					this.targetPositionTranform = GameObject.Find("targetb").transform;
				}
			}
			if (this.numTargets == 0)
			{
				if (this.whichRace == 0)
				{
					this.numTargets = this.RaceTargets.childCount;
				}
				else
				{
					this.numTargets = this.RaceTargets2.childCount;
				}
			}
			if (Vector3.Distance(base.transform.position, this.targetPositionTranform.position) < 30f && this.checkpoints < this.numTargets)
			{
				this.checkpoints++;
				if (this.whichRace == 0)
				{
					this.targetPositionTranform = this.RaceTargets.GetChild(this.checkpoints);
					this.nextTarget = this.RaceTargets.GetChild(this.checkpoints + 1);
					return;
				}
				this.targetPositionTranform = this.RaceTargets2.GetChild(this.checkpoints);
				this.nextTarget = this.RaceTargets2.GetChild(this.checkpoints + 1);
			}
		}
	}

	// Token: 0x040009A9 RID: 2473
	[SerializeField]
	private float interactRange;

	// Token: 0x040009AA RID: 2474
	private InteractiveObject interactiveObject;

	// Token: 0x040009AB RID: 2475
	private string interactiveObjectDesc = "";

	// Token: 0x040009AC RID: 2476
	private float interactiveObjectPrice;

	// Token: 0x040009AD RID: 2477
	private Camera cam;

	// Token: 0x040009AE RID: 2478
	private RaycastHit hit;

	// Token: 0x040009AF RID: 2479
	private RaycastHit hitp;

	// Token: 0x040009B0 RID: 2480
	private RaycastHit hit2;

	// Token: 0x040009B1 RID: 2481
	private ReticleController reticleController;

	// Token: 0x040009B2 RID: 2482
	public GameObject specialfab;

	// Token: 0x040009B3 RID: 2483
	private string colliderName;

	// Token: 0x040009B4 RID: 2484
	public Camera FirstPersonCharacter;

	// Token: 0x040009B5 RID: 2485
	public bool drivingCar;

	// Token: 0x040009B6 RID: 2486
	private bool onCreeper;

	// Token: 0x040009B7 RID: 2487
	public GameObject pickedUpObject;

	// Token: 0x040009B8 RID: 2488
	public GameObject person;

	// Token: 0x040009B9 RID: 2489
	public GameObject personFX;

	// Token: 0x040009BA RID: 2490
	public GameObject pcCanvas;

	// Token: 0x040009BB RID: 2491
	public GameObject inspectionCanvas;

	// Token: 0x040009BC RID: 2492
	public GameObject inventoryCanvas;

	// Token: 0x040009BD RID: 2493
	public InventoryItems inv;

	// Token: 0x040009BE RID: 2494
	public GameObject myPC;

	// Token: 0x040009BF RID: 2495
	public GameObject myPCtower;

	// Token: 0x040009C0 RID: 2496
	public GameObject seatMount;

	// Token: 0x040009C1 RID: 2497
	public GameObject seatMountCar;

	// Token: 0x040009C2 RID: 2498
	public GameObject seatMountCart;

	// Token: 0x040009C3 RID: 2499
	public GameObject seatMountBike;

	// Token: 0x040009C4 RID: 2500
	public GameObject seatMountF;

	// Token: 0x040009C5 RID: 2501
	public GameObject creeperMount;

	// Token: 0x040009C6 RID: 2502
	public GameObject creeperUnmount;

	// Token: 0x040009C7 RID: 2503
	public GameObject exitMount;

	// Token: 0x040009C8 RID: 2504
	public GameObject exitMountP;

	// Token: 0x040009C9 RID: 2505
	public GameObject watchMount;

	// Token: 0x040009CA RID: 2506
	public GameObject watchMountF;

	// Token: 0x040009CB RID: 2507
	private Vector3 watchmountpos;

	// Token: 0x040009CC RID: 2508
	public GameObject leftGlass;

	// Token: 0x040009CD RID: 2509
	public bool watching;

	// Token: 0x040009CE RID: 2510
	public GameObject exitMountCar;

	// Token: 0x040009CF RID: 2511
	public GameObject exitMountCarP;

	// Token: 0x040009D0 RID: 2512
	public GameObject exitMountCart;

	// Token: 0x040009D1 RID: 2513
	public GameObject exitMountBike;

	// Token: 0x040009D2 RID: 2514
	public GameObject exitMountF;

	// Token: 0x040009D3 RID: 2515
	public GameObject exitMountFP;

	// Token: 0x040009D4 RID: 2516
	public GameObject truck;

	// Token: 0x040009D5 RID: 2517
	public GameObject truck2;

	// Token: 0x040009D6 RID: 2518
	public GameObject fourcyl;

	// Token: 0x040009D7 RID: 2519
	public GameObject v8;

	// Token: 0x040009D8 RID: 2520
	public GameObject i6;

	// Token: 0x040009D9 RID: 2521
	public GameObject ucar;

	// Token: 0x040009DA RID: 2522
	public GameObject cart;

	// Token: 0x040009DB RID: 2523
	public GameObject dirtbike;

	// Token: 0x040009DC RID: 2524
	public GameObject creeper;

	// Token: 0x040009DD RID: 2525
	public GameObject fpsHand;

	// Token: 0x040009DE RID: 2526
	public GameObject fpsHand2;

	// Token: 0x040009DF RID: 2527
	public GameObject fpsRatchet;

	// Token: 0x040009E0 RID: 2528
	public GameObject ratchetLever;

	// Token: 0x040009E1 RID: 2529
	public GameObject fpsHook;

	// Token: 0x040009E2 RID: 2530
	public GameObject winchHook;

	// Token: 0x040009E3 RID: 2531
	public GameObject winchObject;

	// Token: 0x040009E4 RID: 2532
	public GameObject winchHookF;

	// Token: 0x040009E5 RID: 2533
	public GameObject winchObjectF;

	// Token: 0x040009E6 RID: 2534
	public GameObject mNeedle;

	// Token: 0x040009E7 RID: 2535
	public GameObject multimeter;

	// Token: 0x040009E8 RID: 2536
	public GameObject screwdriver;

	// Token: 0x040009E9 RID: 2537
	public GameObject depthgauge;

	// Token: 0x040009EA RID: 2538
	public GameObject deptharrow;

	// Token: 0x040009EB RID: 2539
	public GameObject tirepump;

	// Token: 0x040009EC RID: 2540
	public GameObject nokia;

	// Token: 0x040009ED RID: 2541
	public GameObject tiregauge;

	// Token: 0x040009EE RID: 2542
	public GameObject tiregaugemeter;

	// Token: 0x040009EF RID: 2543
	public GameObject jumpercables;

	// Token: 0x040009F0 RID: 2544
	public GameObject crowbar;

	// Token: 0x040009F1 RID: 2545
	public GameObject pickaxe;

	// Token: 0x040009F2 RID: 2546
	public GameObject pickaxeObj;

	// Token: 0x040009F3 RID: 2547
	private GameObject newPickaxe;

	// Token: 0x040009F4 RID: 2548
	public GameObject weldgun;

	// Token: 0x040009F5 RID: 2549
	public bool holdingWelder;

	// Token: 0x040009F6 RID: 2550
	public ParticleSystem weldingFlash;

	// Token: 0x040009F7 RID: 2551
	public GameObject welderMachine;

	// Token: 0x040009F8 RID: 2552
	public GameObject welderVisual;

	// Token: 0x040009F9 RID: 2553
	public GameObject dipStick;

	// Token: 0x040009FA RID: 2554
	private bool phoneOn;

	// Token: 0x040009FB RID: 2555
	public GameObject phonePanel;

	// Token: 0x040009FC RID: 2556
	public GameObject payPhonePanel;

	// Token: 0x040009FD RID: 2557
	public GameObject lottoPanel;

	// Token: 0x040009FE RID: 2558
	public GameObject moonPanel;

	// Token: 0x040009FF RID: 2559
	public GameObject moonPanel2;

	// Token: 0x04000A00 RID: 2560
	public GameObject twofiftyPanelo;

	// Token: 0x04000A01 RID: 2561
	public GameObject tirePanel;

	// Token: 0x04000A02 RID: 2562
	public GameObject racePanel;

	// Token: 0x04000A03 RID: 2563
	public GameObject racePanel2;

	// Token: 0x04000A04 RID: 2564
	public GameObject trophyPanel;

	// Token: 0x04000A05 RID: 2565
	public GameObject escapePanel;

	// Token: 0x04000A06 RID: 2566
	public GameObject toolTipPanel;

	// Token: 0x04000A07 RID: 2567
	public GameObject atmMenu;

	// Token: 0x04000A08 RID: 2568
	public GameObject rgbPanel;

	// Token: 0x04000A09 RID: 2569
	public GameObject rgbPanelPaint;

	// Token: 0x04000A0A RID: 2570
	public GameObject impoundPanel;

	// Token: 0x04000A0B RID: 2571
	public MainPanelManager mainPanelMgr;

	// Token: 0x04000A0C RID: 2572
	public double newRadius;

	// Token: 0x04000A0D RID: 2573
	public float deflation;

	// Token: 0x04000A0E RID: 2574
	public double percentageInflated;

	// Token: 0x04000A0F RID: 2575
	public double wheelContainerHeight;

	// Token: 0x04000A10 RID: 2576
	public GameObject wheelHolderFL;

	// Token: 0x04000A11 RID: 2577
	public GameObject wheelHolderFR;

	// Token: 0x04000A12 RID: 2578
	public GameObject wheelHolderRL;

	// Token: 0x04000A13 RID: 2579
	public GameObject wheelHolderRR;

	// Token: 0x04000A14 RID: 2580
	public GameObject wheelHolderFLF;

	// Token: 0x04000A15 RID: 2581
	public GameObject wheelHolderFRF;

	// Token: 0x04000A16 RID: 2582
	public GameObject wheelHolderRLF;

	// Token: 0x04000A17 RID: 2583
	public GameObject wheelHolderRRF;

	// Token: 0x04000A18 RID: 2584
	private GameObject wheelHolderX;

	// Token: 0x04000A19 RID: 2585
	private float standardTrac;

	// Token: 0x04000A1A RID: 2586
	private float mudTrac;

	// Token: 0x04000A1B RID: 2587
	private float rockTrac;

	// Token: 0x04000A1C RID: 2588
	private float newDrag = 0.05f;

	// Token: 0x04000A1D RID: 2589
	private float FLbase = 1f;

	// Token: 0x04000A1E RID: 2590
	private float FRbase = 1f;

	// Token: 0x04000A1F RID: 2591
	private float RLbase = 1f;

	// Token: 0x04000A20 RID: 2592
	private float RRbase = 1f;

	// Token: 0x04000A21 RID: 2593
	private float FLtotal;

	// Token: 0x04000A22 RID: 2594
	private float FRtotal;

	// Token: 0x04000A23 RID: 2595
	private float RLtotal;

	// Token: 0x04000A24 RID: 2596
	private float RRtotal;

	// Token: 0x04000A25 RID: 2597
	private double airmeterpos;

	// Token: 0x04000A26 RID: 2598
	private GameObject tireX;

	// Token: 0x04000A27 RID: 2599
	private WheelCollider wheelColX;

	// Token: 0x04000A28 RID: 2600
	public float FLbonus;

	// Token: 0x04000A29 RID: 2601
	public float FRbonus;

	// Token: 0x04000A2A RID: 2602
	public float RLbonus;

	// Token: 0x04000A2B RID: 2603
	public float RRbonus;

	// Token: 0x04000A2C RID: 2604
	public float w4bonus;

	// Token: 0x04000A2D RID: 2605
	private int nextUpdate = 1;

	// Token: 0x04000A2E RID: 2606
	public WheelCollider wheelColFL;

	// Token: 0x04000A2F RID: 2607
	public WheelCollider wheelColFR;

	// Token: 0x04000A30 RID: 2608
	public WheelCollider wheelColRL;

	// Token: 0x04000A31 RID: 2609
	public WheelCollider wheelColRR;

	// Token: 0x04000A32 RID: 2610
	public WheelCollider wheelColFLF;

	// Token: 0x04000A33 RID: 2611
	public WheelCollider wheelColFRF;

	// Token: 0x04000A34 RID: 2612
	public WheelCollider wheelColRLF;

	// Token: 0x04000A35 RID: 2613
	public WheelCollider wheelColRRF;

	// Token: 0x04000A36 RID: 2614
	private bool canSpawn;

	// Token: 0x04000A37 RID: 2615
	private float batteryCharge;

	// Token: 0x04000A38 RID: 2616
	private int crankTime;

	// Token: 0x04000A39 RID: 2617
	private bool attemptedCrank;

	// Token: 0x04000A3A RID: 2618
	private bool multimeterActive;

	// Token: 0x04000A3B RID: 2619
	private bool multimeterSelected;

	// Token: 0x04000A3C RID: 2620
	public GameObject eventSystem;

	// Token: 0x04000A3D RID: 2621
	public GameObject ignition_keyO;

	// Token: 0x04000A3E RID: 2622
	public GameObject ignition_keyC;

	// Token: 0x04000A3F RID: 2623
	public GameObject ignition_keyF;

	// Token: 0x04000A40 RID: 2624
	public GameObject emptyCan;

	// Token: 0x04000A41 RID: 2625
	public GameObject emptyCanCrushed;

	// Token: 0x04000A42 RID: 2626
	public GameObject emptyCan2;

	// Token: 0x04000A43 RID: 2627
	public GameObject emptyCanB;

	// Token: 0x04000A44 RID: 2628
	public GameObject emptyCanC;

	// Token: 0x04000A45 RID: 2629
	public GameObject emptyCanCrushed2;

	// Token: 0x04000A46 RID: 2630
	public GameObject bottleFrag;

	// Token: 0x04000A47 RID: 2631
	public ParticleSystem carbonationParticle;

	// Token: 0x04000A48 RID: 2632
	public Transform canPos1;

	// Token: 0x04000A49 RID: 2633
	public Transform canPos2;

	// Token: 0x04000A4A RID: 2634
	public GameObject emptyBottle;

	// Token: 0x04000A4B RID: 2635
	public GameObject fpsCanBeer;

	// Token: 0x04000A4C RID: 2636
	public GameObject fpsBottleBeer;

	// Token: 0x04000A4D RID: 2637
	public GameObject fpsCanChew;

	// Token: 0x04000A4E RID: 2638
	public GameObject fpsCanEnergy;

	// Token: 0x04000A4F RID: 2639
	private AudioSource[] opensounds;

	// Token: 0x04000A50 RID: 2640
	private int ranNum;

	// Token: 0x04000A51 RID: 2641
	public GameObject truckLightR;

	// Token: 0x04000A52 RID: 2642
	public GameObject truckLightL;

	// Token: 0x04000A53 RID: 2643
	public GameObject truckLightRL;

	// Token: 0x04000A54 RID: 2644
	public Light tailLightR;

	// Token: 0x04000A55 RID: 2645
	public Light tailLightL;

	// Token: 0x04000A56 RID: 2646
	public LensFlare tailFlareR;

	// Token: 0x04000A57 RID: 2647
	public LensFlare tailFlareL;

	// Token: 0x04000A58 RID: 2648
	public GameObject truckFLightR;

	// Token: 0x04000A59 RID: 2649
	public GameObject truckFLightL;

	// Token: 0x04000A5A RID: 2650
	public GameObject truckFLightRL;

	// Token: 0x04000A5B RID: 2651
	public GameObject truckFtaillights;

	// Token: 0x04000A5C RID: 2652
	public GameObject carLightR;

	// Token: 0x04000A5D RID: 2653
	public GameObject carLightL;

	// Token: 0x04000A5E RID: 2654
	public GameObject carLightRL;

	// Token: 0x04000A5F RID: 2655
	public GameObject carTaillights;

	// Token: 0x04000A60 RID: 2656
	public GameObject JackObj;

	// Token: 0x04000A61 RID: 2657
	private int jackHeight;

	// Token: 0x04000A62 RID: 2658
	public GameObject[] hiJack;

	// Token: 0x04000A63 RID: 2659
	public GameObject[] jackBody;

	// Token: 0x04000A64 RID: 2660
	public int keyState;

	// Token: 0x04000A65 RID: 2661
	public int keyStateC;

	// Token: 0x04000A66 RID: 2662
	public int keyStateF;

	// Token: 0x04000A67 RID: 2663
	private int pagenum;

	// Token: 0x04000A68 RID: 2664
	public GameObject recycleZone;

	// Token: 0x04000A69 RID: 2665
	public GameObject recycleZoneTrash;

	// Token: 0x04000A6A RID: 2666
	public GameObject recycleZoneWood;

	// Token: 0x04000A6B RID: 2667
	public GameObject recycleZoneOre;

	// Token: 0x04000A6C RID: 2668
	public GameObject johnny1;

	// Token: 0x04000A6D RID: 2669
	public GameObject newageGirl;

	// Token: 0x04000A6E RID: 2670
	public GameObject chainsawobj;

	// Token: 0x04000A6F RID: 2671
	public TillScript gasStationTill;

	// Token: 0x04000A70 RID: 2672
	public GameObject trailer;

	// Token: 0x04000A71 RID: 2673
	public GameObject trailerGen;

	// Token: 0x04000A72 RID: 2674
	public GameObject trailerC;

	// Token: 0x04000A73 RID: 2675
	public GameObject trailerH;

	// Token: 0x04000A74 RID: 2676
	public GameObject hitchPoint;

	// Token: 0x04000A75 RID: 2677
	public GameObject hitchPointF;

	// Token: 0x04000A76 RID: 2678
	public GameObject hitch;

	// Token: 0x04000A77 RID: 2679
	public GameObject hitchG;

	// Token: 0x04000A78 RID: 2680
	public GameObject hitchLink;

	// Token: 0x04000A79 RID: 2681
	public GameObject hitchLinkG;

	// Token: 0x04000A7A RID: 2682
	public GameObject hitchC;

	// Token: 0x04000A7B RID: 2683
	public GameObject hitchLinkC;

	// Token: 0x04000A7C RID: 2684
	public GameObject hitchH;

	// Token: 0x04000A7D RID: 2685
	public GameObject hitchLinkH;

	// Token: 0x04000A7E RID: 2686
	private GameObject[] bolts;

	// Token: 0x04000A7F RID: 2687
	private GameObject spawnPart;

	// Token: 0x04000A80 RID: 2688
	private string spawnStr;

	// Token: 0x04000A81 RID: 2689
	public FirstPersonController fpc;

	// Token: 0x04000A82 RID: 2690
	public ControlRef cr;

	// Token: 0x04000A83 RID: 2691
	private int whichCar;

	// Token: 0x04000A84 RID: 2692
	private bool highlighted;

	// Token: 0x04000A85 RID: 2693
	private float numTurns;

	// Token: 0x04000A86 RID: 2694
	private AudioSource aSource;

	// Token: 0x04000A87 RID: 2695
	private int numBoltsUndone;

	// Token: 0x04000A88 RID: 2696
	private int layer_mask;

	// Token: 0x04000A89 RID: 2697
	private float currTime;

	// Token: 0x04000A8A RID: 2698
	private int currDay;

	// Token: 0x04000A8B RID: 2699
	private float newTime;

	// Token: 0x04000A8C RID: 2700
	public int weatherId;

	// Token: 0x04000A8D RID: 2701
	private bool holdingAirPump;

	// Token: 0x04000A8E RID: 2702
	public GameObject pumpMachine;

	// Token: 0x04000A8F RID: 2703
	private AudioSource[] iaSources;

	// Token: 0x04000A90 RID: 2704
	private AudioSource[] iaSourcesd;

	// Token: 0x04000A91 RID: 2705
	public AudioSource paSources;

	// Token: 0x04000A92 RID: 2706
	public AudioClip[] playerAudio;

	// Token: 0x04000A93 RID: 2707
	public bool canRun;

	// Token: 0x04000A94 RID: 2708
	public bool canCrank;

	// Token: 0x04000A95 RID: 2709
	public Vector3 enterposition;

	// Token: 0x04000A96 RID: 2710
	private Hashtable iTweenArgs;

	// Token: 0x04000A97 RID: 2711
	private float thisdura;

	// Token: 0x04000A98 RID: 2712
	private float newRust;

	// Token: 0x04000A99 RID: 2713
	private bool updatebolts;

	// Token: 0x04000A9A RID: 2714
	public car carscript;

	// Token: 0x04000A9B RID: 2715
	public car4 fcarscript;

	// Token: 0x04000A9C RID: 2716
	public car3 acarscript;

	// Token: 0x04000A9D RID: 2717
	public GameObject ucarAudio;

	// Token: 0x04000A9E RID: 2718
	public GameObject still;

	// Token: 0x04000A9F RID: 2719
	public GameObject businessman;

	// Token: 0x04000AA0 RID: 2720
	public GameObject fireman;

	// Token: 0x04000AA1 RID: 2721
	public GameObject jimmy;

	// Token: 0x04000AA2 RID: 2722
	public GameObject jake;

	// Token: 0x04000AA3 RID: 2723
	public GameObject jake2;

	// Token: 0x04000AA4 RID: 2724
	public GameObject jiggs;

	// Token: 0x04000AA5 RID: 2725
	public GameObject contractor;

	// Token: 0x04000AA6 RID: 2726
	public GameObject racenpc;

	// Token: 0x04000AA7 RID: 2727
	public GameObject modGirl;

	// Token: 0x04000AA8 RID: 2728
	public GameObject stationClerk;

	// Token: 0x04000AA9 RID: 2729
	public int towerstatus1;

	// Token: 0x04000AAA RID: 2730
	public int overflowvalve;

	// Token: 0x04000AAB RID: 2731
	public GameObject emissiveBolt;

	// Token: 0x04000AAC RID: 2732
	private GameObject emissiveBolt2;

	// Token: 0x04000AAD RID: 2733
	public bool resetEmissive;

	// Token: 0x04000AAE RID: 2734
	public Radio radio;

	// Token: 0x04000AAF RID: 2735
	public streetlights lightscript;

	// Token: 0x04000AB0 RID: 2736
	private Vector3 engStandAngle;

	// Token: 0x04000AB1 RID: 2737
	private bool rotatingEngStand;

	// Token: 0x04000AB2 RID: 2738
	public GameObject dk9;

	// Token: 0x04000AB3 RID: 2739
	public GameObject EngStand;

	// Token: 0x04000AB4 RID: 2740
	public bool haltDetach;

	// Token: 0x04000AB5 RID: 2741
	public GameObject oilBolt;

	// Token: 0x04000AB6 RID: 2742
	public GameObject oilBoltV8;

	// Token: 0x04000AB7 RID: 2743
	public GameObject oilBolti6;

	// Token: 0x04000AB8 RID: 2744
	public SleepScript sleepScript;

	// Token: 0x04000AB9 RID: 2745
	public GameObject FSM;

	// Token: 0x04000ABA RID: 2746
	public GameObject mapCanvas;

	// Token: 0x04000ABB RID: 2747
	public GameObject playerArrow;

	// Token: 0x04000ABC RID: 2748
	private RectTransform prt;

	// Token: 0x04000ABD RID: 2749
	private Vector3 apos;

	// Token: 0x04000ABE RID: 2750
	private Vector3 arot;

	// Token: 0x04000ABF RID: 2751
	public bool canTrack;

	// Token: 0x04000AC0 RID: 2752
	public int discoveredMission;

	// Token: 0x04000AC1 RID: 2753
	public MailScript mail;

	// Token: 0x04000AC2 RID: 2754
	public Transform leanDest;

	// Token: 0x04000AC3 RID: 2755
	public Transform leanDestO;

	// Token: 0x04000AC4 RID: 2756
	public GameObject cameraTransform;

	// Token: 0x04000AC5 RID: 2757
	public RectTransform mapImage;

	// Token: 0x04000AC6 RID: 2758
	public GameObject subtitles;

	// Token: 0x04000AC7 RID: 2759
	private string lastHit;

	// Token: 0x04000AC8 RID: 2760
	private bool itemsInDismount;

	// Token: 0x04000AC9 RID: 2761
	public GameObject JarCratePrefab;

	// Token: 0x04000ACA RID: 2762
	private float beerTimeout;

	// Token: 0x04000ACB RID: 2763
	public GameObject restockableItems;

	// Token: 0x04000ACC RID: 2764
	public GameObject restockableBerries;

	// Token: 0x04000ACD RID: 2765
	private GameObject newTire;

	// Token: 0x04000ACE RID: 2766
	private int i;

	// Token: 0x04000ACF RID: 2767
	private int rimNum;

	// Token: 0x04000AD0 RID: 2768
	private int tireNum;

	// Token: 0x04000AD1 RID: 2769
	public GameObject wheelObj;

	// Token: 0x04000AD2 RID: 2770
	private int FLindex;

	// Token: 0x04000AD3 RID: 2771
	private int FRindex;

	// Token: 0x04000AD4 RID: 2772
	private int RLindex;

	// Token: 0x04000AD5 RID: 2773
	private int RRindex;

	// Token: 0x04000AD6 RID: 2774
	public Transform RemoveFL;

	// Token: 0x04000AD7 RID: 2775
	public Transform RemoveFR;

	// Token: 0x04000AD8 RID: 2776
	public Transform RemoveRL;

	// Token: 0x04000AD9 RID: 2777
	public Transform RemoveRR;

	// Token: 0x04000ADA RID: 2778
	public Transform RemoveFLF;

	// Token: 0x04000ADB RID: 2779
	public Transform RemoveFRF;

	// Token: 0x04000ADC RID: 2780
	public Transform RemoveRLF;

	// Token: 0x04000ADD RID: 2781
	public Transform RemoveRRF;

	// Token: 0x04000ADE RID: 2782
	public TruckBedGrav truckBed;

	// Token: 0x04000ADF RID: 2783
	public TruckBedGrav truckBed2;

	// Token: 0x04000AE0 RID: 2784
	public TrailerBedGrav trailerBedH;

	// Token: 0x04000AE1 RID: 2785
	public TrailerBedGrav trailerBedC;

	// Token: 0x04000AE2 RID: 2786
	public GroundDetect gd;

	// Token: 0x04000AE3 RID: 2787
	private GameObject newPart;

	// Token: 0x04000AE4 RID: 2788
	public Currency currency;

	// Token: 0x04000AE5 RID: 2789
	private int enterTime;

	// Token: 0x04000AE6 RID: 2790
	public Rigidbody trb;

	// Token: 0x04000AE7 RID: 2791
	public bool raceActive;

	// Token: 0x04000AE8 RID: 2792
	public int checkpoints;

	// Token: 0x04000AE9 RID: 2793
	private Vector3 targetPosition;

	// Token: 0x04000AEA RID: 2794
	private int numTargets;

	// Token: 0x04000AEB RID: 2795
	[SerializeField]
	public Transform targetPositionTranform;

	// Token: 0x04000AEC RID: 2796
	public Transform RaceTargets;

	// Token: 0x04000AED RID: 2797
	public Transform RaceTargets2;

	// Token: 0x04000AEE RID: 2798
	public Transform nextTarget;

	// Token: 0x04000AEF RID: 2799
	public int whichRace;

	// Token: 0x04000AF0 RID: 2800
	public GameObject moneyRoll;

	// Token: 0x04000AF1 RID: 2801
	private GameObject newRoll;

	// Token: 0x04000AF2 RID: 2802
	public poker pokerScript;

	// Token: 0x04000AF3 RID: 2803
	public bool holdingHose;

	// Token: 0x04000AF4 RID: 2804
	public bool holdingHoseTank;

	// Token: 0x04000AF5 RID: 2805
	public GameObject fpshose;

	// Token: 0x04000AF6 RID: 2806
	public GameObject garageFaucet;

	// Token: 0x04000AF7 RID: 2807
	public GameObject garageFaucet2;

	// Token: 0x04000AF8 RID: 2808
	public WaterValve waterValve;

	// Token: 0x04000AF9 RID: 2809
	public GameObject debugMenu;

	// Token: 0x04000AFA RID: 2810
	public GameObject cashOption;

	// Token: 0x04000AFB RID: 2811
	public float dropInterval;

	// Token: 0x04000AFC RID: 2812
	private float hitInterval;

	// Token: 0x04000AFD RID: 2813
	public Watermax waterMax;

	// Token: 0x04000AFE RID: 2814
	public watermaxCar waterMaxC;

	// Token: 0x04000AFF RID: 2815
	public WatermaxF waterMaxF;

	// Token: 0x04000B00 RID: 2816
	public FixedJoint holdJoint;

	// Token: 0x04000B01 RID: 2817
	public HydraulicLift hydraulicLift;

	// Token: 0x04000B02 RID: 2818
	public ModernGarageOpener mgd;

	// Token: 0x04000B03 RID: 2819
	public BuyHouse bh;

	// Token: 0x04000B04 RID: 2820
	public MissionGen mg;

	// Token: 0x04000B05 RID: 2821
	public GameObject jerrycan;

	// Token: 0x04000B06 RID: 2822
	public CargoBar bar;

	// Token: 0x04000B07 RID: 2823
	private int whichHitch;

	// Token: 0x04000B08 RID: 2824
	public bool moveTrailer;

	// Token: 0x04000B09 RID: 2825
	public MainMenu mm;

	// Token: 0x04000B0A RID: 2826
	private float autoSaveTime;

	// Token: 0x04000B0B RID: 2827
	public GameObject autoSavePanel;

	// Token: 0x04000B0C RID: 2828
	private float depthDurability;

	// Token: 0x04000B0D RID: 2829
	public bool boughti6;

	// Token: 0x04000B0E RID: 2830
	public GameObject oldeng_acc;

	// Token: 0x04000B0F RID: 2831
	private bool ach_decent;

	// Token: 0x04000B10 RID: 2832
	private bool ach_classy;

	// Token: 0x04000B11 RID: 2833
	public ViolationCanv vCanv;

	// Token: 0x04000B12 RID: 2834
	public GameObject violationCanv;

	// Token: 0x04000B13 RID: 2835
	public bool diamondbackImpounded;

	// Token: 0x04000B14 RID: 2836
	public bool f100Impounded;

	// Token: 0x04000B15 RID: 2837
	public bool amcImpounded;

	// Token: 0x04000B16 RID: 2838
	public bool dirtbikeImpounded;

	// Token: 0x04000B17 RID: 2839
	public bool golfcartImpounded;

	// Token: 0x04000B18 RID: 2840
	public bool arrested;

	// Token: 0x04000B19 RID: 2841
	public bool handcuffed;

	// Token: 0x04000B1A RID: 2842
	public Officer officer;

	// Token: 0x04000B1B RID: 2843
	public bool inoutVehicle;

	// Token: 0x04000B1C RID: 2844
	public float timeOfDay;

	// Token: 0x04000B1D RID: 2845
	private bool leaned;

	// Token: 0x04000B1E RID: 2846
	public TractionBuddy buddy;

	// Token: 0x04000B1F RID: 2847
	public GameObject leg;

	// Token: 0x04000B20 RID: 2848
	public GameObject legC;

	// Token: 0x04000B21 RID: 2849
	public GameObject legF;

	// Token: 0x04000B22 RID: 2850
	public int radioNum;

	// Token: 0x04000B23 RID: 2851
	public Benchpress bench;

	// Token: 0x04000B24 RID: 2852
	public UnityEngine.Color highlightColor = new UnityEngine.Color(0f, 1f, 0f);

	// Token: 0x04000B25 RID: 2853
	public UnityEngine.Color highlightColorGroup = new UnityEngine.Color(0f, 0.4f, 0f);

	// Token: 0x04000B26 RID: 2854
	private float[,] tractionVals = new float[,]
	{
		{
			0f,
			0f,
			0f,
			0f
		},
		{
			0f,
			0f,
			0f,
			0f
		},
		{
			0f,
			0f,
			0f,
			0f
		},
		{
			0f,
			0f,
			0f,
			0f
		},
		{
			0f,
			0f,
			0f,
			0f
		},
		{
			0f,
			0f,
			0f,
			0f
		},
		{
			5f,
			1f,
			1f,
			1f
		},
		{
			3f,
			2f,
			2f,
			1f
		},
		{
			2f,
			3f,
			6f,
			3f
		},
		{
			2f,
			5f,
			3f,
			3f
		},
		{
			3f,
			3f,
			3f,
			4f
		},
		{
			4f,
			3f,
			3f,
			3f
		},
		{
			3f,
			3f,
			4f,
			3f
		},
		{
			3f,
			4f,
			3f,
			3f
		},
		{
			3f,
			3f,
			5f,
			4f
		},
		{
			2f,
			4f,
			4f,
			4f
		},
		{
			1f,
			6f,
			3f,
			4f
		},
		{
			2f,
			4f,
			4f,
			6f
		},
		{
			4f,
			3f,
			4f,
			3f
		},
		{
			3f,
			5f,
			5f,
			5f
		}
	};
}
