using System;
using UnityEngine;

// Token: 0x02000038 RID: 56
public class car3 : MonoBehaviour
{
	// Token: 0x060000FA RID: 250 RVA: 0x0000C774 File Offset: 0x0000A974
	private void Start()
	{
		base.GetComponent<Rigidbody>().centerOfMass = this.centerOfMass.localPosition;
		this.rot = this.steerWheel.transform.localRotation;
		this.surfaceType = 0f;
		this.canAcc = true;
		this.em = this.exhaustSmoke.GetComponent<ParticleSystem>().emission;
		this.emb = this.backfireSmoke.GetComponent<ParticleSystem>().emission;
		this.element.enabled = false;
		this.cr = GameObject.Find("EventSystem2").GetComponent<ControlRef>();
		this.updateEngine();
	}

	// Token: 0x060000FB RID: 251 RVA: 0x0000C814 File Offset: 0x0000AA14
	public void EnableWC()
	{
		this.WheelFrontRightGo.SetActive(true);
		this.WheelFrontLeftGo.SetActive(true);
		this.WheelRearLeftGo.SetActive(true);
		this.WheelRearRightGo.SetActive(true);
		if (base.GetComponent<FixedJoint>() != null)
		{
			this.flatbed.EmptyBed();
			FixedJoint component = base.GetComponent<FixedJoint>();
			component.connectedBody = null;
			Object.Destroy(component);
		}
	}

	// Token: 0x060000FC RID: 252 RVA: 0x0000C87C File Offset: 0x0000AA7C
	private void Update()
	{
		if (this.userControlled)
		{
			this.CameraSwitch();
			this.control();
			this.surfaceType = 0f;
			if (this.surfaceType == 1f)
			{
				this.maxTorque = this.maxTorqueStatic / 4f;
			}
			else if (!this.usingI6)
			{
				this.maxTorque = this.maxTorqueStatic - (100f - this.oilLevel + (400f - this.stockdurability.health * 4f));
			}
			if (!this.intLightsOn && this.canAcc)
			{
				this.intLight1.GetComponent<Light>().enabled = true;
				this.intLightsOn = true;
			}
			this.oilEmissive = this.SetEmissive(this.oilIcon, this.oilEmissive, this.enableOil);
		}
	}

	// Token: 0x060000FD RID: 253 RVA: 0x0000C94C File Offset: 0x0000AB4C
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

	// Token: 0x060000FE RID: 254 RVA: 0x0000C9BC File Offset: 0x0000ABBC
	private float DimEmissive(GameObject emissiveObject, float emissive, bool enable)
	{
		if (enable)
		{
			emissive = 0.4f;
		}
		else
		{
			emissive = 0f;
		}
		emissive = Mathf.Clamp01(emissive);
		emissiveObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white * Mathf.LinearToGammaSpace(emissive));
		return emissive;
	}

	// Token: 0x060000FF RID: 255 RVA: 0x0000CA0C File Offset: 0x0000AC0C
	private void FixedUpdate()
	{
		if (Time.time >= (float)this.oilDrainInterval)
		{
			this.oilDrainInterval = Mathf.FloorToInt(Time.time) + 30;
			this.oilLevel -= 1f;
			if (this.oilLevel < 1f)
			{
				this.oilLevel = 0f;
			}
		}
		if (this.usingI6 && this.userControlled)
		{
			if (this.i6Transform.parent != null)
			{
				if (this.i6Transform.parent.name != "amc")
				{
					this.KillEngine();
				}
			}
			else
			{
				this.KillEngine();
			}
		}
		else if (!this.usingI6 && this.userControlled && !this.engineRenderer.enabled)
		{
			this.KillEngine();
		}
		if (Time.time > (float)this.bodyCheckInterval)
		{
			this.bodyCheckInterval = Mathf.FloorToInt(Time.time) + 10;
		}
		if (!this.controlled && this.speed > 1f)
		{
			this.HandBrake();
		}
		if (this.controlled)
		{
			if (Time.time > (float)this.materialCheckInterval && this.speed > 1f)
			{
				this.materialCheckInterval = Mathf.FloorToInt(Time.time) + 1;
				this.groundDetect.GetTerrainTexture();
				this.deepMud = this.groundDetect.deepMud;
				this.deepWater = this.groundDetect.deepWater;
			}
			if (this.deepMud == 1 && this.rb.drag < 4f)
			{
				this.rb.drag += 0.15f;
			}
			else if (this.deepWater == 1 && this.rb.drag < 5f)
			{
				this.rb.drag += 0.15f;
			}
			else
			{
				this.rb.drag = 0.05f;
			}
			this.speed = this.rb.velocity.magnitude * 3.6f;
			this.HandBrake();
			if (!this.usingI6)
			{
				if (this.oilLevel < 45f)
				{
					this.enableOil = true;
				}
				else
				{
					this.enableOil = false;
				}
			}
			else if (this.enginescript.newOilLevel < 45f)
			{
				this.enableOil = true;
			}
			else
			{
				this.enableOil = false;
			}
			this.fuel -= 0.5f * Time.deltaTime;
			this.fuelAngle = (this.fuel / 18.5f - 120f) * -1f;
			if (this.additive > 1f)
			{
				this.additive -= 0.3f * Time.deltaTime;
			}
			this.pointerFuel.transform.localRotation = Quaternion.Euler(0f, 0f, this.fuelAngle);
			if (this.jumptime > 0f)
			{
				this.jumptime += 1f;
				if (this.jumptime > 500f)
				{
					this.jumptime = 0f;
				}
			}
			if (Time.time >= (float)this.engineCheckInterval)
			{
				this.engineCheckInterval = Mathf.FloorToInt(Time.time) + 10;
				this.updateEngine();
				int num = Random.Range(0, 6);
				float num2 = 1f;
				if (this.additive > 2f)
				{
					num = 0;
					num2 = 3f;
				}
				if (num == 0)
				{
					this.stockdurability.health -= num2;
					if (this.stockdurability.health < 0f)
					{
						this.stockdurability.health = 0f;
					}
				}
				if (this.fuel < 200f)
				{
					this.SetEmissive(this.fuelIcon, this.fuelEmissive, true);
				}
				else
				{
					this.SetEmissive(this.fuelIcon, this.fuelEmissive, false);
				}
				bool flag = this.checkEngine;
				if (this.temperature > 230f)
				{
				}
			}
		}
		else
		{
			if (this.temperature > 0f)
			{
				this.temperature -= 1f * Time.deltaTime;
			}
			if (this.intLightsOn)
			{
				this.intLightsOn = false;
				this.intLight1.GetComponent<Light>().enabled = false;
			}
		}
		if (this.controlled)
		{
			this.Fdrag = this.Cdrag * Mathf.Pow(this.speed, 2f);
			this.WheelOffset(this.wheelFL, this.WheelFrontLeft);
			this.WheelOffset(this.wheelFR, this.WheelFrontRight);
			this.WheelOffset(this.wheelRL, this.WheelRearLeft);
			this.WheelOffset(this.wheelRR, this.WheelRearRight);
		}
		if (this.userControlled && this.usingI6)
		{
			if (this.fanSpin)
			{
				this.i6Pulley1.transform.Rotate(0f, 720f * Time.deltaTime, 0f);
				this.i6Pulley2.transform.Rotate(0f, 720f * Time.deltaTime, 0f);
				this.i6Pulley3.transform.Rotate(0f, 0f, 720f * Time.deltaTime);
				this.i6Fan.transform.Rotate(0f, 720f * Time.deltaTime, 0f);
			}
			this.i6Crank.transform.Rotate(0f, 720f * Time.deltaTime, 0f);
			this.i6Cam.transform.Rotate(0f, 720f * Time.deltaTime, 0f);
			this.i6Cam2.transform.Rotate(0f, 720f * Time.deltaTime, 0f);
		}
	}

	// Token: 0x06000100 RID: 256 RVA: 0x0000CFBC File Offset: 0x0000B1BC
	private void control()
	{
		if (this.userControlled)
		{
			if (this.additive > 1f)
			{
				this.additiveBonus = 200f;
			}
			else
			{
				this.additiveBonus = 0f;
			}
			if (this.controlled)
			{
				this.torque = this.maxTorque * this.cr.Vert;
				if (this.torque > 0f)
				{
					this.torque += this.additiveBonus;
				}
				if (this.cr.GasInput != 0f)
				{
					this.torque = this.maxTorque * this.cr.GasInput;
					if (this.torque > 0f)
					{
						this.torque += this.additiveBonus;
					}
				}
			}
		}
		this.maxSpeed = 20f;
		if (this.cr.Vert < 0f && this.speed > this.maxSpeed)
		{
			this.torque = 0f;
			this.frontTorque = 0f;
			this.rearTorque = 0f;
		}
		this.WheelRearRight.motorTorque = this.torque * 2f;
		this.WheelRearLeft.motorTorque = this.torque * 2f;
		this.WheelFrontLeft.motorTorque = this.torque / 2f;
		this.WheelFrontRight.motorTorque = this.torque / 2f;
		if (this.userControlled && this.controlled)
		{
			this.SteerWheelControl();
		}
	}

	// Token: 0x06000101 RID: 257 RVA: 0x0000D144 File Offset: 0x0000B344
	private void SteerWheelControl()
	{
		if (this.angleChangeTimer < 0f)
		{
			this.steerwheelOffset = Random.Range(-0.4f, 0.4f) * this.speed;
			this.angleChangeTimer += Random.Range(0.1f, 1f);
		}
		float t = this.speed / 3f / this.lowestSpeedAtSteer;
		float num = Mathf.Lerp(this.lowSpeedSteerAngle, this.highSpeedSteerAngle, t);
		if (this.cr.xAxes == 0f || this.cr.Horiz != 0f)
		{
			num *= this.cr.Horiz;
		}
		else
		{
			num *= this.cr.xAxes;
		}
		if (this.interactor.currency.drunk > 6 && this.speed > 2f)
		{
			float num2 = (float)this.interactor.currency.drunk * 0.4f;
			float num3 = Mathf.PerlinNoise(Time.time * 0.3f, 0f);
			float num4 = Mathf.Sin(Time.time * 0.6f) * (num2 * num3);
			num += num4;
		}
		this.WheelFrontRight.steerAngle = num;
		this.WheelFrontLeft.steerAngle = num;
		this.timer += Time.deltaTime * this.steerWheelRotateFactor;
		this.newAngle = Mathf.Lerp(this.oldAngle, num * 10f + this.steerwheelOffset, this.timer);
		this.steerWheelAngle = this.newAngle;
		this.steerWheel.transform.localRotation = this.rot * Quaternion.Euler(new Vector3(0f, 0f, -this.steerWheelAngle));
		this.oldAngle = this.steerWheelAngle;
		if (this.oldAngle == this.newAngle)
		{
			this.timer = 0f;
		}
		this.angleChangeTimer -= Time.deltaTime;
	}

	// Token: 0x06000102 RID: 258 RVA: 0x0000D338 File Offset: 0x0000B538
	private void WheelOffset(GameObject model, WheelCollider collider)
	{
		Vector3 position;
		Quaternion rotation;
		collider.GetWorldPose(out position, out rotation);
		model.transform.position = position;
		model.transform.rotation = rotation;
	}

	// Token: 0x06000103 RID: 259 RVA: 0x0000D368 File Offset: 0x0000B568
	private void HandBrake()
	{
		if (Input.GetKey(this.cr.Brake) || (this.cr.Vert == 0f && this.cr.GasInput == 0f && (int)this.speed == 0) || this.cr.BrakeInput > 0.1f)
		{
			this.braked = true;
			this.brakeLightL.range = 3.5f;
			this.brakeLightR.range = 3.5f;
		}
		else if (!this.userControlled || !this.controlled)
		{
			this.braked = true;
			this.brakeLightL.range = 1f;
			this.brakeLightR.range = 1f;
		}
		else
		{
			this.braked = false;
		}
		if (this.braked)
		{
			this.WheelRearLeft.brakeTorque = this.maxBrakeTorque;
			this.WheelRearRight.brakeTorque = this.maxBrakeTorque;
			this.WheelRearRight.motorTorque = 0f;
			this.WheelRearLeft.motorTorque = 0f;
			this.WheelFrontLeft.brakeTorque = this.maxBrakeTorque;
			this.WheelFrontRight.brakeTorque = this.maxBrakeTorque;
			this.WheelFrontRight.motorTorque = 0f;
			this.WheelFrontLeft.motorTorque = 0f;
			this.color.a = 1f;
			return;
		}
		this.WheelRearLeft.brakeTorque = 0f;
		this.WheelRearRight.brakeTorque = 0f;
		this.WheelFrontLeft.brakeTorque = 0f;
		this.WheelFrontRight.brakeTorque = 0f;
		this.color.a = 0.5f;
	}

	// Token: 0x06000104 RID: 260 RVA: 0x0000D520 File Offset: 0x0000B720
	private void KillEngine()
	{
		Debug.Log("killeng");
		this.keyState = 0;
		this.canRun = false;
		this.controlled = false;
		this.userControlled = false;
		this.TurnOff();
		this.turnOffAcc();
		this.person.GetComponent<Interactor>().keyStateC = 0;
		this.audioCtrl.GetComponent<AudioControlCar>().TurnOff();
	}

	// Token: 0x06000105 RID: 261 RVA: 0x0000D580 File Offset: 0x0000B780
	public void updateEngine()
	{
		this.canRun = true;
		if (this.fuel < 2f)
		{
			this.canRun = false;
			this.controlled = false;
		}
		if (this.waterFlood == 1 && this.controlled)
		{
			this.KillEngine();
		}
		if (!this.usingI6)
		{
			if (this.stockdurability.health < 1f)
			{
				this.KillEngine();
			}
		}
		else if (this.i6Transform.parent != null)
		{
			if (this.i6Transform.parent.name != "amc")
			{
				this.KillEngine();
			}
		}
		else
		{
			this.KillEngine();
		}
		this.enginescr.Refresh();
		if (Vector3.Dot(base.transform.up, Vector3.down) > 0f)
		{
			this.keyState = 3;
			this.person.GetComponent<Interactor>().keyStateC = 0;
			this.canRun = false;
			this.controlled = false;
			this.userControlled = false;
			this.WheelRearLeft.brakeTorque = 500f;
			this.WheelRearRight.brakeTorque = 500f;
			this.WheelFrontLeft.brakeTorque = 500f;
			this.WheelFrontRight.brakeTorque = 500f;
			this.TurnOff();
			this.turnOffAcc();
			this.audioCtrl.GetComponent<AudioControlCar>().TurnOff();
		}
		if (this.canRun && this.waterFlood == 0 && Vector3.Dot(base.transform.up, Vector3.down) <= 0f)
		{
			this.canRun = true;
		}
		if (this.usingI6 && this.canRun)
		{
			this.canRun = this.enginescr.canRun;
		}
	}

	// Token: 0x06000106 RID: 262 RVA: 0x0000D72C File Offset: 0x0000B92C
	public void turnOnAcc()
	{
		this.SetEmissive(this.ebrakeIcon, this.ebrakeEmissive, true);
		this.gravShell.enabled = true;
	}

	// Token: 0x06000107 RID: 263 RVA: 0x0000D74E File Offset: 0x0000B94E
	public void dimAcc()
	{
		this.DimEmissive(this.ebrakeIcon, this.ebrakeEmissive, true);
	}

	// Token: 0x06000108 RID: 264 RVA: 0x0000D764 File Offset: 0x0000B964
	public void turnOffAcc()
	{
		this.SetEmissive(this.ebrakeIcon, this.ebrakeEmissive, false);
		this.SetEmissive(this.fuelIcon, this.fuelEmissive, false);
		this.SetEmissive(this.oilIcon, this.oilEmissive, false);
		this.gravShell.enabled = false;
	}

	// Token: 0x06000109 RID: 265 RVA: 0x0000D7B9 File Offset: 0x0000B9B9
	public void TurnOff()
	{
		this.enableEmission = false;
		this.em.enabled = this.enableEmission;
	}

	// Token: 0x0600010A RID: 266 RVA: 0x0000D7D3 File Offset: 0x0000B9D3
	public void TurnOn()
	{
		if (!this.usingI6)
		{
			this.enableEmission = true;
			this.em.enabled = this.enableEmission;
		}
	}

	// Token: 0x0600010B RID: 267 RVA: 0x0000D7F5 File Offset: 0x0000B9F5
	public void RestoreAcc()
	{
		this.engineAcc.SetActive(true);
		this.usingI6 = false;
	}

	// Token: 0x0600010C RID: 268 RVA: 0x00002188 File Offset: 0x00000388
	private void CameraSwitch()
	{
	}

	// Token: 0x0600010D RID: 269 RVA: 0x0000D80C File Offset: 0x0000BA0C
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "water" || other.tag == "DynamicWater")
		{
			this.deepWater = 1;
			for (int i = 0; i < this.splashFx.Length; i++)
			{
			}
		}
	}

	// Token: 0x0600010E RID: 270 RVA: 0x0000D858 File Offset: 0x0000BA58
	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "water" || other.tag == "DynamicWater")
		{
			this.deepWater = 0;
			for (int i = 0; i < this.splashFx.Length; i++)
			{
				this.splashFx[i].SetActive(false);
			}
		}
	}

	// Token: 0x0400029E RID: 670
	public bool controlled;

	// Token: 0x0400029F RID: 671
	public bool userControlled;

	// Token: 0x040002A0 RID: 672
	public Transform centerOfMass;

	// Token: 0x040002A1 RID: 673
	public Transform steerWheel;

	// Token: 0x040002A2 RID: 674
	public float steerAngle;

	// Token: 0x040002A3 RID: 675
	public GameObject[] cams;

	// Token: 0x040002A4 RID: 676
	public Color color;

	// Token: 0x040002A5 RID: 677
	public float Cdrag = 0.2f;

	// Token: 0x040002A6 RID: 678
	public float Fdrag;

	// Token: 0x040002A7 RID: 679
	public float maxFuel = 1300f;

	// Token: 0x040002A8 RID: 680
	public float minFuel;

	// Token: 0x040002A9 RID: 681
	public float fuel;

	// Token: 0x040002AA RID: 682
	public float temperature;

	// Token: 0x040002AB RID: 683
	public float maxTemp;

	// Token: 0x040002AC RID: 684
	public float coolantLevel = 100f;

	// Token: 0x040002AD RID: 685
	public float oilLevel;

	// Token: 0x040002AE RID: 686
	private float tempAngle;

	// Token: 0x040002AF RID: 687
	private float fuelAngle;

	// Token: 0x040002B0 RID: 688
	public float maxTorque;

	// Token: 0x040002B1 RID: 689
	public float maxTorqueStatic;

	// Token: 0x040002B2 RID: 690
	public float maxBrakeTorque = 1200f;

	// Token: 0x040002B3 RID: 691
	public float MaxWheelRotateAngle = 14f;

	// Token: 0x040002B4 RID: 692
	public float lowestSpeedAtSteer = 30f;

	// Token: 0x040002B5 RID: 693
	public float lowSpeedSteerAngle = 15f;

	// Token: 0x040002B6 RID: 694
	public float highSpeedSteerAngle = 1f;

	// Token: 0x040002B7 RID: 695
	public float torque;

	// Token: 0x040002B8 RID: 696
	public float frontTorque;

	// Token: 0x040002B9 RID: 697
	public float rearTorque;

	// Token: 0x040002BA RID: 698
	public float maxSpeed;

	// Token: 0x040002BB RID: 699
	public float steerWheelAngle;

	// Token: 0x040002BC RID: 700
	public float steerWheelRotateFactor = 1f;

	// Token: 0x040002BD RID: 701
	public bool braked;

	// Token: 0x040002BE RID: 702
	public WheelCollider WheelFrontRight;

	// Token: 0x040002BF RID: 703
	public WheelCollider WheelFrontLeft;

	// Token: 0x040002C0 RID: 704
	public WheelCollider WheelRearRight;

	// Token: 0x040002C1 RID: 705
	public WheelCollider WheelRearLeft;

	// Token: 0x040002C2 RID: 706
	public GameObject WheelFrontRightGo;

	// Token: 0x040002C3 RID: 707
	public GameObject WheelFrontLeftGo;

	// Token: 0x040002C4 RID: 708
	public GameObject WheelRearRightGo;

	// Token: 0x040002C5 RID: 709
	public GameObject WheelRearLeftGo;

	// Token: 0x040002C6 RID: 710
	public GameObject wheelRay;

	// Token: 0x040002C7 RID: 711
	public float surfaceType;

	// Token: 0x040002C8 RID: 712
	public GameObject intLight1;

	// Token: 0x040002C9 RID: 713
	private bool intLightsOn;

	// Token: 0x040002CA RID: 714
	public GameObject wheelFR;

	// Token: 0x040002CB RID: 715
	public GameObject wheelFL;

	// Token: 0x040002CC RID: 716
	public GameObject wheelRR;

	// Token: 0x040002CD RID: 717
	public GameObject wheelRL;

	// Token: 0x040002CE RID: 718
	public int deepWater;

	// Token: 0x040002CF RID: 719
	public float speed;

	// Token: 0x040002D0 RID: 720
	private float oldAngle;

	// Token: 0x040002D1 RID: 721
	private float newAngle;

	// Token: 0x040002D2 RID: 722
	private float timer;

	// Token: 0x040002D3 RID: 723
	private float angleChangeTimer;

	// Token: 0x040002D4 RID: 724
	private float steerwheelOffset;

	// Token: 0x040002D5 RID: 725
	public float oilEmissive;

	// Token: 0x040002D6 RID: 726
	public float tempEmissive;

	// Token: 0x040002D7 RID: 727
	public float fuelEmissive;

	// Token: 0x040002D8 RID: 728
	public float ebrakeEmissive;

	// Token: 0x040002D9 RID: 729
	public GameObject oilIcon;

	// Token: 0x040002DA RID: 730
	public GameObject encheckIcon;

	// Token: 0x040002DB RID: 731
	public GameObject fuelIcon;

	// Token: 0x040002DC RID: 732
	public GameObject ebrakeIcon;

	// Token: 0x040002DD RID: 733
	public bool checkEngine;

	// Token: 0x040002DE RID: 734
	public bool enableOil;

	// Token: 0x040002DF RID: 735
	public GameObject pointerFuel;

	// Token: 0x040002E0 RID: 736
	public GameObject engineFan;

	// Token: 0x040002E1 RID: 737
	public bool canRun;

	// Token: 0x040002E2 RID: 738
	public bool canCrank;

	// Token: 0x040002E3 RID: 739
	public bool canAcc = true;

	// Token: 0x040002E4 RID: 740
	public bool canEasyStart;

	// Token: 0x040002E5 RID: 741
	public bool canMove;

	// Token: 0x040002E6 RID: 742
	public int keyState;

	// Token: 0x040002E7 RID: 743
	private int interval = 1;

	// Token: 0x040002E8 RID: 744
	private float nextTime;

	// Token: 0x040002E9 RID: 745
	public float jumptime;

	// Token: 0x040002EA RID: 746
	private int engineCheckInterval = 2;

	// Token: 0x040002EB RID: 747
	public engine enginescript;

	// Token: 0x040002EC RID: 748
	public enginei6 enginescr;

	// Token: 0x040002ED RID: 749
	public GameObject exhaustSmoke;

	// Token: 0x040002EE RID: 750
	public GameObject backfireSmoke;

	// Token: 0x040002EF RID: 751
	private int i;

	// Token: 0x040002F0 RID: 752
	private Quaternion rot;

	// Token: 0x040002F1 RID: 753
	private float newblend;

	// Token: 0x040002F2 RID: 754
	private bool enableEmission;

	// Token: 0x040002F3 RID: 755
	private bool enableEmissionB;

	// Token: 0x040002F4 RID: 756
	private ParticleSystem.EmissionModule em;

	// Token: 0x040002F5 RID: 757
	private ParticleSystem.EmissionModule emb;

	// Token: 0x040002F6 RID: 758
	public GameObject doorP;

	// Token: 0x040002F7 RID: 759
	public GameObject doorD;

	// Token: 0x040002F8 RID: 760
	public GameObject hood;

	// Token: 0x040002F9 RID: 761
	private int bodyCheckInterval = 1;

	// Token: 0x040002FA RID: 762
	private int oilDrainInterval = 30;

	// Token: 0x040002FB RID: 763
	public ImpactDeformable idScriptDoorP;

	// Token: 0x040002FC RID: 764
	public ImpactDeformable idScriptDoorD;

	// Token: 0x040002FD RID: 765
	public ImpactDeformable idScriptHood;

	// Token: 0x040002FE RID: 766
	public int waterFlood;

	// Token: 0x040002FF RID: 767
	public GameObject audioCtrl;

	// Token: 0x04000300 RID: 768
	public GameObject[] splashFx;

	// Token: 0x04000301 RID: 769
	public CarTrunkGrav gravShell;

	// Token: 0x04000302 RID: 770
	public Rigidbody rb;

	// Token: 0x04000303 RID: 771
	private int deepMud;

	// Token: 0x04000304 RID: 772
	public GroundDetect groundDetect;

	// Token: 0x04000305 RID: 773
	private int materialCheckInterval = 1;

	// Token: 0x04000306 RID: 774
	public Renderer element;

	// Token: 0x04000307 RID: 775
	public GameObject person;

	// Token: 0x04000308 RID: 776
	public ControlRef cr;

	// Token: 0x04000309 RID: 777
	public TrailerBedGrav flatbed;

	// Token: 0x0400030A RID: 778
	public bool usingI6;

	// Token: 0x0400030B RID: 779
	public durability stockdurability;

	// Token: 0x0400030C RID: 780
	public GameObject engineAcc;

	// Token: 0x0400030D RID: 781
	public Transform i6Transform;

	// Token: 0x0400030E RID: 782
	public Renderer engineRenderer;

	// Token: 0x0400030F RID: 783
	public GameObject i6Fan;

	// Token: 0x04000310 RID: 784
	public GameObject i6Crank;

	// Token: 0x04000311 RID: 785
	public GameObject i6Cam;

	// Token: 0x04000312 RID: 786
	public GameObject i6Cam2;

	// Token: 0x04000313 RID: 787
	public GameObject i6Pulley1;

	// Token: 0x04000314 RID: 788
	public GameObject i6Pulley2;

	// Token: 0x04000315 RID: 789
	public GameObject i6Pulley3;

	// Token: 0x04000316 RID: 790
	public bool fanSpin;

	// Token: 0x04000317 RID: 791
	public ModWomanJobs mw;

	// Token: 0x04000318 RID: 792
	public float additive;

	// Token: 0x04000319 RID: 793
	private float additiveBonus;

	// Token: 0x0400031A RID: 794
	public Light brakeLightR;

	// Token: 0x0400031B RID: 795
	public Light brakeLightL;

	// Token: 0x0400031C RID: 796
	public Interactor interactor;
}
