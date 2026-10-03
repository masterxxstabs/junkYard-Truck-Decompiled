using System;
using System.Collections;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000169 RID: 361
public class car4 : MonoBehaviour
{
	// Token: 0x060008D5 RID: 2261 RVA: 0x00071B54 File Offset: 0x0006FD54
	private void Start()
	{
		if (this.cr == null)
		{
			this.cr = GameObject.Find("EventSystem2").GetComponent<ControlRef>();
		}
		this.RRholder.transform.position = this.RRpos.position;
		this.RLholder.transform.position = this.RLpos.position;
		this.FLholder.transform.position = this.FLpos.position;
		this.FRholder.transform.position = this.FRpos.position;
		Transform child = this.FLpos.GetChild(0);
		Transform child2 = this.FRpos.GetChild(0);
		Transform child3 = this.RLpos.GetChild(0);
		Transform child4 = this.RRpos.GetChild(0);
		foreach (object obj in this.RLholder.transform)
		{
			((Transform)obj).position = new Vector3(child3.position.x, child3.position.y, child3.position.z);
		}
		foreach (object obj2 in this.RRholder.transform)
		{
			((Transform)obj2).position = new Vector3(child4.position.x, child4.position.y, child4.position.z);
		}
		foreach (object obj3 in this.FLholder.transform)
		{
			((Transform)obj3).position = new Vector3(child.position.x, child.position.y, child.position.z);
		}
		foreach (object obj4 in this.FRholder.transform)
		{
			((Transform)obj4).position = new Vector3(child2.position.x, child2.position.y, child2.position.z);
		}
		this.element.enabled = false;
		this.theDest = GameObject.Find("Destination").transform;
		base.GetComponent<Rigidbody>().centerOfMass = this.centerOfMass.localPosition;
		this.rot = this.steerWheel.transform.localRotation;
		this.surfaceType = 0;
		for (int i = 1; i < this.cams.Length; i++)
		{
			this.cams[i].SetActive(false);
		}
		this.canAcc = true;
		if (this.usingV8)
		{
			this.v8Brackets();
		}
		BoxCollider[] components = this.wheelFL.GetComponents<BoxCollider>();
		components[0].enabled = true;
		components[1].enabled = true;
		BoxCollider[] components2 = this.wheelFR.GetComponents<BoxCollider>();
		components2[0].enabled = true;
		components2[1].enabled = true;
		BoxCollider[] components3 = this.wheelRL.GetComponents<BoxCollider>();
		components3[0].enabled = true;
		components3[1].enabled = true;
		BoxCollider[] components4 = this.wheelRR.GetComponents<BoxCollider>();
		components4[0].enabled = true;
		components4[1].enabled = true;
		Transform transform = this.wheelFL.transform.GetChild(0).transform;
		bool flag = false;
		this.WheelFrontLeft.radius = 0.23f;
		this.i = 6;
		while (this.i < 20)
		{
			if (transform.GetChild(this.i).gameObject.active)
			{
				if (this.wheelFL.GetComponent<durability>().health < 1f)
				{
					this.wheelFL.GetComponent<durability>().health = 20f;
				}
				this.WheelFrontLeft.radius = 0.4f;
				flag = true;
				break;
			}
			this.i++;
		}
		if (!flag)
		{
			this.wheelFL.GetComponent<durability>().health = 0f;
		}
		transform = this.wheelFR.transform.GetChild(0).transform;
		flag = false;
		this.WheelFrontRight.radius = 0.23f;
		this.i = 6;
		while (this.i < 20)
		{
			if (transform.GetChild(this.i).gameObject.active)
			{
				if (this.wheelFR.GetComponent<durability>().health < 1f)
				{
					this.wheelFR.GetComponent<durability>().health = 20f;
				}
				this.WheelFrontRight.radius = 0.4f;
				flag = true;
				break;
			}
			this.i++;
		}
		if (!flag)
		{
			this.wheelFR.GetComponent<durability>().health = 0f;
		}
		transform = this.wheelFL.transform.GetChild(0).transform;
		flag = false;
		this.WheelRearLeft.radius = 0.23f;
		this.i = 6;
		while (this.i < 20)
		{
			if (transform.GetChild(this.i).gameObject.active)
			{
				if (this.wheelRL.GetComponent<durability>().health < 1f)
				{
					this.wheelRL.GetComponent<durability>().health = 20f;
				}
				this.WheelRearLeft.radius = 0.4f;
				flag = true;
				break;
			}
			this.i++;
		}
		if (!flag)
		{
			this.wheelRL.GetComponent<durability>().health = 0f;
		}
		transform = this.wheelRR.transform.GetChild(0).transform;
		flag = false;
		this.WheelRearRight.radius = 0.23f;
		this.i = 6;
		while (this.i < 20)
		{
			if (transform.GetChild(this.i).gameObject.active)
			{
				if (this.wheelRR.GetComponent<durability>().health < 1f)
				{
					this.wheelRR.GetComponent<durability>().health = 20f;
				}
				this.WheelRearRight.radius = 0.4f;
				flag = true;
				break;
			}
			this.i++;
		}
		if (!flag)
		{
			this.wheelRR.GetComponent<durability>().health = 0f;
		}
		if (this.frontGear4Rend.enabled)
		{
			foreach (object obj5 in this.frontGear4Rend.transform)
			{
				((Transform)obj5).gameObject.GetComponent<BoxCollider>().enabled = true;
			}
			using (IEnumerator enumerator = this.frontGear5Rend.transform.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj6 = enumerator.Current;
					((Transform)obj6).gameObject.GetComponent<BoxCollider>().enabled = false;
				}
				goto IL_890;
			}
		}
		if (this.frontGear5Rend.enabled)
		{
			foreach (object obj7 in this.frontGear5Rend.transform)
			{
				((Transform)obj7).gameObject.GetComponent<BoxCollider>().enabled = true;
			}
			using (IEnumerator enumerator = this.frontGear4Rend.transform.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj8 = enumerator.Current;
					((Transform)obj8).gameObject.GetComponent<BoxCollider>().enabled = false;
				}
				goto IL_890;
			}
		}
		if (!this.frontGear5Rend.enabled && !this.frontGear4Rend.enabled)
		{
			foreach (object obj9 in this.frontGear5Rend.transform)
			{
				((Transform)obj9).gameObject.GetComponent<BoxCollider>().enabled = false;
			}
			foreach (object obj10 in this.frontGear4Rend.transform)
			{
				((Transform)obj10).gameObject.GetComponent<BoxCollider>().enabled = false;
			}
		}
		IL_890:
		if (this.rearGear4Rend.enabled)
		{
			foreach (object obj11 in this.rearGear4Rend.transform)
			{
				((Transform)obj11).gameObject.GetComponent<BoxCollider>().enabled = true;
			}
			using (IEnumerator enumerator = this.rearGear5Rend.transform.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj12 = enumerator.Current;
					((Transform)obj12).gameObject.GetComponent<BoxCollider>().enabled = false;
				}
				goto IL_AB6;
			}
		}
		if (this.rearGear5Rend.enabled)
		{
			foreach (object obj13 in this.rearGear5Rend.transform)
			{
				((Transform)obj13).gameObject.GetComponent<BoxCollider>().enabled = true;
			}
			using (IEnumerator enumerator = this.rearGear4Rend.transform.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj14 = enumerator.Current;
					((Transform)obj14).gameObject.GetComponent<BoxCollider>().enabled = false;
				}
				goto IL_AB6;
			}
		}
		if (!this.rearGear5Rend.enabled && !this.rearGear4Rend.enabled)
		{
			foreach (object obj15 in this.rearGear5Rend.transform)
			{
				((Transform)obj15).gameObject.GetComponent<BoxCollider>().enabled = false;
			}
			foreach (object obj16 in this.rearGear4Rend.transform)
			{
				((Transform)obj16).gameObject.GetComponent<BoxCollider>().enabled = false;
			}
		}
		IL_AB6:
		if (this.isDigital)
		{
			this.digitalDisplay.SetActive(true);
			this.pointerFuel.GetComponent<Renderer>().enabled = false;
			this.pointerTemp.GetComponent<Renderer>().enabled = false;
			this.speedNeedle.GetComponent<Renderer>().enabled = false;
			this.rgbDial.SetActive(true);
			this.fDash.SetColor("_EmissionColor", new UnityEngine.Color(this.rgbR, this.rgbG, this.rgbB, 1f));
			this.tachColor.color = new UnityEngine.Color(this.rgbR, this.rgbG, this.rgbB, 1f);
			this.mphColor.color = new UnityEngine.Color(this.rgbR, this.rgbG, this.rgbB, 1f);
		}
		if (this.cleanInterior)
		{
			this.interior.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", UnityEngine.Color.black);
		}
		else
		{
			this.interior.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", UnityEngine.Color.white);
		}
		if (this.hasDiffLock)
		{
			this.diffLockButton.SetActive(true);
			this.diffLock1.SetActive(true);
			this.diffLock2.SetActive(true);
			this.diffLockLed.SetActive(false);
		}
		if (this.hasRollbar)
		{
			this.kcSwitch.SetActive(true);
			this.rollbar.SetActive(true);
		}
		if (this.soundMat)
		{
			this.lDoor.clip = this.heavyDoor[0];
			this.rDoor.clip = this.heavyDoor[0];
		}
	}

	// Token: 0x060008D6 RID: 2262 RVA: 0x0007287C File Offset: 0x00070A7C
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

	// Token: 0x060008D7 RID: 2263 RVA: 0x000728E4 File Offset: 0x00070AE4
	private void Update()
	{
		if (this.controlled)
		{
			this.control();
			if (Input.GetKeyDown(this.cr.Clutch))
			{
				this.enginescriptv8.canMove = false;
				this.clutchIn = true;
			}
			if (Input.GetKeyUp(this.cr.Clutch))
			{
				this.enginescriptv8.canMove = true;
				this.clutchIn = false;
				if (this.clutchPoppedTime > 6f)
				{
					this.clutchPoppedTime = 0.1f;
				}
			}
			if (this.cr.ClutchInput > 0.3f)
			{
				this.enginescriptv8.canMove = false;
				this.clutchIn = true;
				this.physClutchIn = true;
			}
			if (this.cr.ClutchInput < 0.3f && this.physClutchIn)
			{
				this.enginescriptv8.canMove = true;
				this.clutchIn = false;
				this.physClutchIn = false;
				if (this.clutchPoppedTime > 6f)
				{
					this.clutchPoppedTime = 0.1f;
				}
			}
			if (!this.intLightsOn && this.canAcc)
			{
				this.intLightsOn = true;
			}
			this.wd4Emissive = this.SetEmissive(this.wd4Icon, this.wd4Emissive, this.enableWd4);
			if (this.isDigital)
			{
				this.wd4Emissive = this.SetEmissive(this.wd4Icon2, this.wd4Emissive, this.enableWd4);
			}
			if (this.usingV8)
			{
				this.crankV8.transform.Rotate(0f, -720f * Time.deltaTime, 0f);
				this.altFan.transform.Rotate(0f, -360f * Time.deltaTime, 0f);
				if (this.fanSpin)
				{
					this.v8Fan.transform.Rotate(0f, -720f * Time.deltaTime, 0f);
				}
				if (this.enginescriptv8.am_headerD_cnd > 0f)
				{
					this.turbineD.transform.Rotate(0f, 0f, 540f * Time.deltaTime, Space.Self);
				}
				if (this.enginescriptv8.am_headerP_cnd > 0f)
				{
					this.turbineP.transform.Rotate(0f, 20f, 0f * Time.deltaTime, Space.Self);
				}
			}
		}
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x00072B2C File Offset: 0x00070D2C
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
		emissiveObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", UnityEngine.Color.white * Mathf.LinearToGammaSpace(emissive));
		return emissive;
	}

	// Token: 0x060008D9 RID: 2265 RVA: 0x00072B9C File Offset: 0x00070D9C
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
		emissiveObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", UnityEngine.Color.white * Mathf.LinearToGammaSpace(emissive));
		return emissive;
	}

	// Token: 0x060008DA RID: 2266 RVA: 0x00072BEC File Offset: 0x00070DEC
	public void CheckSusp()
	{
		if (this.sbolt1.boltTurns == 25 && this.sbolt2.boltTurns == 25 && this.sbolt3.boltTurns == -25 && this.sbolt4.boltTurns == -25)
		{
			Achievement achievement = new Achievement("ACH_DISASTER");
			achievement.Trigger(true);
		}
	}

	// Token: 0x060008DB RID: 2267 RVA: 0x00072C4C File Offset: 0x00070E4C
	public void DiffLock()
	{
		this.diffLocked = !this.diffLocked;
		if (!this.diffLocked)
		{
			this.diffLockLed.SetActive(false);
			this.lowSpeedSteerAngle = 37f;
			WheelFrictionCurve forwardFriction = this.WheelFrontLeft.forwardFriction;
			forwardFriction.stiffness = 1f;
			this.WheelFrontLeft.forwardFriction = forwardFriction;
			WheelFrictionCurve forwardFriction2 = this.WheelFrontRight.forwardFriction;
			forwardFriction2.stiffness = 1f;
			this.WheelFrontRight.forwardFriction = forwardFriction2;
			WheelFrictionCurve forwardFriction3 = this.WheelRearLeft.forwardFriction;
			forwardFriction3.stiffness = 1f;
			this.WheelRearLeft.forwardFriction = forwardFriction3;
			WheelFrictionCurve forwardFriction4 = this.WheelRearRight.forwardFriction;
			forwardFriction4.stiffness = 1f;
			this.WheelRearRight.forwardFriction = forwardFriction4;
			return;
		}
		this.diffLockLed.SetActive(true);
		this.lowSpeedSteerAngle = 25f;
		WheelFrictionCurve forwardFriction5 = this.WheelFrontLeft.forwardFriction;
		forwardFriction5.stiffness = 1.4f;
		this.WheelFrontLeft.forwardFriction = forwardFriction5;
		WheelFrictionCurve forwardFriction6 = this.WheelFrontRight.forwardFriction;
		forwardFriction6.stiffness = 1.4f;
		this.WheelFrontRight.forwardFriction = forwardFriction6;
		WheelFrictionCurve forwardFriction7 = this.WheelRearLeft.forwardFriction;
		forwardFriction7.stiffness = 1.4f;
		this.WheelRearLeft.forwardFriction = forwardFriction7;
		WheelFrictionCurve forwardFriction8 = this.WheelRearRight.forwardFriction;
		forwardFriction8.stiffness = 1.4f;
		this.WheelRearRight.forwardFriction = forwardFriction8;
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x00072DCC File Offset: 0x00070FCC
	public void KcLight()
	{
		this.kc1.enabled = !this.kc1.enabled;
		this.kc2.enabled = !this.kc2.enabled;
		this.kc3.enabled = !this.kc3.enabled;
		this.kc4.enabled = !this.kc4.enabled;
		this.kcLight.enabled = !this.kcLight.enabled;
		if (this.interactor.truckFLightRL.GetComponent<Light>().enabled)
		{
			this.interactor.truckFLightRL.GetComponent<Light>().enabled = false;
			this.interactor.truckFLightL.GetComponent<LensFlare>().enabled = false;
			this.interactor.truckFLightR.GetComponent<LensFlare>().enabled = false;
		}
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x00072EB0 File Offset: 0x000710B0
	private void FixedUpdate()
	{
		if (this.controlled)
		{
			this.speed = this.rb.velocity.magnitude * 3.6f;
			if (!this.exhaustTrailsOn)
			{
				this.exhaustTrailsOn = true;
				this.exhaustTrails.GetComponent<ParticleSystem>().Play();
			}
			if (this.speed > 15f && this.heatTrailsOn)
			{
				this.heatTrailsOn = false;
				this.heatTrails.GetComponent<ParticleSystem>().Stop();
			}
			if (this.speed <= 15f && !this.heatTrailsOn && this.temperature > 150f)
			{
				this.heatTrailsOn = true;
				this.heatTrails.GetComponent<ParticleSystem>().Play();
			}
			if (this.usingV8)
			{
				if (this.temperature > 230f)
				{
					if (!this.steamTrailsOnV8)
					{
						this.steamTrailsV8.GetComponent<ParticleSystem>().Play();
						this.steamTrailsOnV8 = true;
					}
				}
				else if (this.steamTrailsOnV8)
				{
					this.steamTrailsV8.GetComponent<ParticleSystem>().Stop();
					this.steamTrailsOnV8 = false;
				}
				if (this.temperature > 240f)
				{
					if (!this.steamTrailsOn2V8)
					{
						this.steamTrails2V8.GetComponent<ParticleSystem>().Play();
						this.steamTrailsOn2V8 = true;
					}
				}
				else if (this.steamTrailsOn2V8)
				{
					this.steamTrails2V8.GetComponent<ParticleSystem>().Stop();
					this.steamTrailsOn2V8 = false;
				}
			}
			this.HandBrake();
			if (this.speed > 5f && !this.hangTimeChallenge)
			{
				if (!this.WheelFrontRight.isGrounded)
				{
					if (!this.WheelFrontLeft.isGrounded && !this.WheelRearRight.isGrounded && !this.WheelRearLeft.isGrounded)
					{
						this.airTime += Time.deltaTime;
						if (this.airTime >= 4f)
						{
							this.hangTimeChallenge = true;
							this.mc.trophyAirtimeF = 1;
							this.mc.CheckTrophies();
						}
					}
				}
				else
				{
					this.airTime = 0f;
				}
			}
			if (this.speed > 0f && this.temperature < this.maxTemp && this.temperature < 250f)
			{
				this.temperature += 2f * Time.deltaTime;
			}
			if (this.temperature > this.maxTemp)
			{
				this.temperature -= 2f * Time.deltaTime;
			}
			this.tempAngle = Mathf.Abs(this.temperature / 6f) * -1f - 120f;
			this.pointerTemp.transform.localRotation = Quaternion.Euler(0f, 0f, this.tempAngle);
			if (this.oilLevel < 45f)
			{
				this.enableOil = true;
			}
			else
			{
				this.enableOil = false;
			}
			if (!this.fuelSaving)
			{
				this.fuel -= 0.8f * Time.deltaTime;
			}
			else
			{
				this.fuel -= 0.5f * Time.deltaTime;
			}
			this.fuelAngle = (this.fuel / 36f + 130f) * -1f;
			if (this.additive > 1f)
			{
				this.additive -= 0.3f * Time.deltaTime;
			}
			this.pointerFuel.transform.localRotation = Quaternion.Euler(0f, 0f, this.fuelAngle);
			if (this.isDigital)
			{
				if (this.enginescriptv8.alternator_cnd > 5f)
				{
					this.voltText.text = "13\nVOLTS";
				}
				else
				{
					this.voltText.text = "0\nVOLTS";
				}
				this.tempText.text = Mathf.Round(this.temperature) + "\nTEMP";
				this.oilText.text = Mathf.Round(this.oilLevel) + "\nOIL";
				this.fuelText.text = Mathf.Round(this.fuel / 10f) + "\nFUEL";
				this.mphText.text = Mathf.Round(this.speed * 0.7f) + "\nMPH";
				this.rpmText.text = Mathf.Round(this.gb.currentPitch * 4133f) - 3333f + "\nRPM";
				this.rpmSlider.value = this.gb.currentPitch * 0.533f - 0.5f;
				if (this.rpmSlider.value < 0f)
				{
					this.rpmSlider.value = 0.1f;
				}
				this.mphSlider.value = this.speed / 130f;
			}
			if (this.enginescriptv8.charging)
			{
				this.pointerAlt.transform.localRotation = Quaternion.Euler(0f, 0f, 54f);
			}
			else
			{
				this.pointerAlt.transform.localRotation = Quaternion.Euler(0f, 0f, 12f);
			}
			if (this.jumptime > 0f)
			{
				this.jumptime += 1f;
				if (this.jumptime > 500f)
				{
					this.jumptime = 0f;
				}
			}
			if (Time.time > (float)this.materialCheckInterval && this.speed > 0.5f)
			{
				this.materialCheckInterval = Mathf.FloorToInt(Time.time) + 1;
				this.groundDetect.GetTerrainTexture();
			}
			if (Time.time >= (float)this.engineCheckInterval)
			{
				this.engineCheckInterval = Mathf.FloorToInt(Time.time) + 10;
				this.updateEngine();
				if (this.fuel < 200f)
				{
					this.SetEmissive(this.fuelIcon, this.fuelEmissive, true);
				}
				else
				{
					this.SetEmissive(this.fuelIcon, this.fuelEmissive, false);
				}
				if (this.checkEngine)
				{
					this.SetEmissive(this.encheckIcon, this.encheckEmissive, true);
				}
				else
				{
					this.SetEmissive(this.encheckIcon, this.encheckEmissive, false);
				}
				if (this.temperature > 230f)
				{
					this.SetEmissive(this.tempIcon, this.tempEmissive, true);
				}
				else
				{
					this.SetEmissive(this.tempIcon, this.tempEmissive, false);
				}
				this.enginescriptv8.DegradeEngine();
				if (this.speed > 5f)
				{
					this.DegradeTires(0);
				}
				if (this.usingV8 && this.enginescriptv8.transferCase_cnd < 15f)
				{
					this.randDisengage = Random.Range(0, 7);
					if (this.randDisengage == 2 && this.enableWd4)
					{
						this.enable4(false);
					}
				}
			}
		}
		else
		{
			this.torque = 0f;
			if (this.exhaustTrailsOn)
			{
				this.exhaustTrailsOn = false;
				this.exhaustTrails.GetComponent<ParticleSystem>().Stop();
			}
			if (this.temperature > 0f)
			{
				this.temperature -= 1f * Time.deltaTime;
				if (this.temperature < 150f && this.heatTrailsOn)
				{
					this.heatTrailsOn = false;
				}
			}
			if (this.intLightsOn)
			{
				this.intLightsOn = false;
			}
			if (this.WheelRearLeft.brakeTorque == 0f)
			{
				this.WheelRearLeft.brakeTorque = 1500f;
				this.WheelRearRight.brakeTorque = 1500f;
				this.WheelFrontLeft.brakeTorque = 1500f;
				this.WheelFrontRight.brakeTorque = 1500f;
			}
		}
		this.Fdrag = this.Cdrag * Mathf.Pow(this.speed, 2f);
	}

	// Token: 0x060008DE RID: 2270 RVA: 0x00073674 File Offset: 0x00071874
	private void control()
	{
		if (this.userControlled)
		{
			if (this.additive > 1f)
			{
				this.additiveBonus = 400f;
			}
			else
			{
				this.additiveBonus = 0f;
			}
			if (this.usingV8 && this.enginescriptv8.canMove)
			{
				if (this.clutchPoppedTime > 0f && this.clutchPoppedTime < 2f)
				{
					this.torqueBonus = 250f;
					this.clutchPoppedTime += 0.1f;
				}
				else
				{
					this.torqueBonus = 0f;
				}
				if (this.clutchPoppedTime >= 2f)
				{
					this.clutchPoppedTime += 0.1f;
				}
				this.torque = this.maxTorque * this.cr.Vert;
				if (this.torque > 0f)
				{
					this.torque += this.torqueBonus + this.additiveBonus;
				}
				if (this.cr.GasInput != 0f)
				{
					this.torque = this.maxTorque * this.cr.GasInput;
					if (this.torque > 0f)
					{
						this.torque += this.torqueBonus + this.additiveBonus;
					}
				}
			}
		}
		if (!this.has5th)
		{
			this.maxSpeed = 85f;
		}
		else
		{
			this.maxSpeed = 100f;
		}
		if (this.frontGear == 0)
		{
			this.frontTorque = 0f;
		}
		if (this.rearGear == 0)
		{
			this.rearTorque = 0f;
		}
		if (this.rearGear == 0 && this.frontGear == 0)
		{
			this.maxSpeed = 0f;
		}
		if (this.frontGear == 4)
		{
			if (this.fourWheelDrive)
			{
				this.maxSpeed = 95f;
			}
			this.frontTorque = this.torque;
		}
		if (this.frontGear == 5)
		{
			if (this.fourWheelDrive)
			{
				this.maxSpeed = 95f;
			}
			this.frontTorque = this.torque * 1.22f;
		}
		if (this.rearGear == 4)
		{
			this.maxSpeed = 95f;
			this.rearTorque = this.torque;
		}
		if (this.rearGear == 5)
		{
			this.maxSpeed = 95f;
			this.rearTorque = this.torque * 1.22f;
		}
		if (this.cr.Vert != 0f && this.speed > this.maxSpeed)
		{
			this.torque = 0f;
			this.frontTorque = 0f;
			this.rearTorque = 0f;
		}
		this.WheelRearRight.motorTorque = this.rearTorque * 2f;
		this.WheelRearLeft.motorTorque = this.rearTorque * 2f;
		this.WheelFrontLeft.motorTorque = this.frontTorque / 2f;
		this.WheelFrontRight.motorTorque = this.frontTorque / 2f;
		if (this.controlled && this.userControlled)
		{
			this.SteerWheelControl();
		}
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x00073970 File Offset: 0x00071B70
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

	// Token: 0x060008E0 RID: 2272 RVA: 0x00073B64 File Offset: 0x00071D64
	private void HandBrake()
	{
		if (Input.GetKey(this.cr.Brake) || this.cr.BrakeInput > 0.1f)
		{
			this.braked = true;
			this.brakeLightL.range = 3.5f;
			this.brakeLightR.range = 3.5f;
		}
		else
		{
			this.braked = false;
			this.brakeLightL.range = 1f;
			this.brakeLightR.range = 1f;
		}
		if (this.braked)
		{
			this.WheelRearLeft.brakeTorque = this.maxBrakeTorque + 200f;
			this.WheelRearRight.brakeTorque = this.maxBrakeTorque + 200f;
			this.WheelRearRight.motorTorque = 0f;
			this.WheelRearLeft.motorTorque = 0f;
			this.WheelFrontLeft.brakeTorque = this.maxBrakeTorque - 200f;
			this.WheelFrontRight.brakeTorque = this.maxBrakeTorque - 200f;
			this.WheelFrontLeft.motorTorque = 0f;
			this.WheelFrontRight.motorTorque = 0f;
			this.color.a = 1f;
			return;
		}
		if ((this.surfaceType != 1 && this.deepMud != 1 && this.deepWater != 1) || this.speed < 1f)
		{
			this.WheelRearLeft.brakeTorque = 0f;
			this.WheelRearRight.brakeTorque = 0f;
			this.WheelFrontLeft.brakeTorque = 0f;
			this.WheelFrontRight.brakeTorque = 0f;
			this.color.a = 0.5f;
		}
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x00073D15 File Offset: 0x00071F15
	public void PopTire()
	{
		this.audioControl.PlaySpecified(36);
		this.tireExplosion.GetComponent<ParticleSystem>().Play();
		this.updateEngine();
	}

	// Token: 0x060008E2 RID: 2274 RVA: 0x00073D3C File Offset: 0x00071F3C
	public void DegradeTires(int whichCorner)
	{
		int num = Random.Range(0, 6);
		if (whichCorner == 1)
		{
			num = 1;
		}
		else if (whichCorner == 2)
		{
			num = 2;
		}
		else if (whichCorner == 3)
		{
			num = 3;
		}
		else if (whichCorner == 4)
		{
			num = 4;
		}
		if (num == 1)
		{
			this.wheelFL.GetComponent<durability>().health -= 1f;
			if (this.wheelFL.GetComponent<durability>().health <= 1f)
			{
				if (this.wheelFL.GetComponent<durability>().health == 1f)
				{
					this.PopTire();
					this.WheelFrontLeft.radius = 0.25f;
				}
				if (this.wheelFL.GetComponent<durability>().health == 0f)
				{
					Transform transform = this.wheelFL.transform.GetChild(0).transform;
					this.i = 6;
					while (this.i < 20)
					{
						if (transform.GetChild(this.i).gameObject.active)
						{
							int num2 = this.i;
							transform.GetChild(this.i).gameObject.SetActive(false);
							transform.GetChild(0).gameObject.SetActive(false);
							this.WheelFrontLeft.radius = 0.23f;
							this.updateEngine();
						}
						this.i++;
					}
					return;
				}
			}
		}
		else if (num == 2)
		{
			this.wheelFR.GetComponent<durability>().health -= 1f;
			if (this.wheelFR.GetComponent<durability>().health <= 1f)
			{
				if (this.wheelFR.GetComponent<durability>().health == 1f)
				{
					this.PopTire();
					this.WheelFrontRight.radius = 0.25f;
				}
				if (this.wheelFR.GetComponent<durability>().health == 0f)
				{
					Transform transform2 = this.wheelFR.transform.GetChild(0).transform;
					this.i = 6;
					while (this.i < 20)
					{
						if (transform2.GetChild(this.i).gameObject.active)
						{
							int num3 = this.i;
							transform2.GetChild(this.i).gameObject.SetActive(false);
							transform2.GetChild(0).gameObject.SetActive(false);
							this.WheelFrontRight.radius = 0.23f;
							this.updateEngine();
						}
						this.i++;
					}
					return;
				}
			}
		}
		else if (num == 3)
		{
			this.wheelRL.GetComponent<durability>().health -= 1f;
			if (this.wheelRL.GetComponent<durability>().health <= 1f)
			{
				if (this.wheelRL.GetComponent<durability>().health == 1f)
				{
					this.PopTire();
					this.WheelRearLeft.radius = 0.25f;
				}
				if (this.wheelRL.GetComponent<durability>().health == 0f)
				{
					Transform transform3 = this.wheelRL.transform.GetChild(0).transform;
					this.i = 6;
					while (this.i < 20)
					{
						if (transform3.GetChild(this.i).gameObject.active)
						{
							int num4 = this.i;
							transform3.GetChild(this.i).gameObject.SetActive(false);
							transform3.GetChild(0).gameObject.SetActive(false);
							this.WheelRearLeft.radius = 0.23f;
							this.updateEngine();
						}
						this.i++;
					}
					return;
				}
			}
		}
		else if (num == 4)
		{
			this.wheelRR.GetComponent<durability>().health -= 1f;
			if (this.wheelRR.GetComponent<durability>().health <= 1f)
			{
				if (this.wheelRR.GetComponent<durability>().health == 1f)
				{
					this.PopTire();
					this.WheelRearRight.radius = 0.25f;
				}
				if (this.wheelRR.GetComponent<durability>().health == 0f)
				{
					Transform transform4 = this.wheelRR.transform.GetChild(0).transform;
					this.i = 6;
					while (this.i < 20)
					{
						if (transform4.GetChild(this.i).gameObject.active)
						{
							int num5 = this.i;
							transform4.GetChild(this.i).gameObject.SetActive(false);
							transform4.GetChild(0).gameObject.SetActive(false);
							this.WheelRearRight.radius = 0.23f;
							this.updateEngine();
						}
						this.i++;
					}
				}
			}
		}
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x000741E0 File Offset: 0x000723E0
	public void updateEngine()
	{
		if (!this.hasTruck2 && this.enginescriptv8.whichTruck == 2 && this.enginescriptv8.canRun && this.enginescriptv8.canMove)
		{
			this.hasTruck2 = true;
		}
		this.frontGear = 0;
		if (this.frontGear4Rend.enabled)
		{
			this.frontGear = 4;
		}
		if (this.frontGear5Rend.enabled)
		{
			this.frontGear = 5;
		}
		this.rearGear = 0;
		if (this.rearGear4Rend.enabled)
		{
			this.rearGear = 4;
		}
		if (this.rearGear5Rend.enabled)
		{
			this.rearGear = 5;
		}
		this.oilEmissive = this.SetEmissive(this.oilIcon, this.oilEmissive, this.enableOil);
		if (!this.clutchIn && this.usingV8)
		{
			this.enginescriptv8.Refresh();
		}
		this.audioControl.rotorCount = 0;
		if (this.WheelFrontLeft.radius <= 0.23f)
		{
			this.audioControl.rotorCount++;
		}
		if (this.WheelFrontRight.radius <= 0.23f)
		{
			this.audioControl.rotorCount++;
		}
		if (this.WheelRearLeft.radius <= 0.23f)
		{
			this.audioControl.rotorCount++;
		}
		if (this.WheelRearRight.radius <= 0.23f)
		{
			this.audioControl.rotorCount++;
		}
		this.audioControl.flatCount = 0;
		if (this.wheelFR.GetComponent<durability>().health == 1f)
		{
			this.audioControl.flatCount++;
		}
		if (this.wheelFL.GetComponent<durability>().health == 1f)
		{
			this.audioControl.flatCount++;
		}
		if (this.wheelRL.GetComponent<durability>().health == 1f)
		{
			this.audioControl.flatCount++;
		}
		if (this.wheelRR.GetComponent<durability>().health == 1f)
		{
			this.audioControl.flatCount++;
		}
		if (this.usingV8 && !this.enginescriptv8.canRun)
		{
			this.canRun = false;
			this.controlled = false;
		}
		if (this.fuel < 2f)
		{
			this.canRun = false;
			this.controlled = false;
		}
		if (this.usingV8)
		{
			this.oilLevel = this.enginescriptv8.newOilLevel;
			this.oilAngle = this.oilLevel / 2.5f;
			this.pointerOil.transform.localRotation = Quaternion.Euler(0f, 0f, this.oilAngle);
		}
		if (this.waterFlood == 1)
		{
			this.turnOffAcc();
			this.keyState = 0;
			this.interactor.keyStateF = 0;
			this.canRun = false;
			this.controlled = false;
			this.userControlled = false;
			this.frontTorque = 0f;
			this.rearTorque = 0f;
			this.WheelRearLeft.brakeTorque = 1500f;
			this.WheelRearRight.brakeTorque = 1500f;
			this.WheelFrontLeft.brakeTorque = 1500f;
			this.WheelFrontRight.brakeTorque = 1500f;
		}
		if (Vector3.Dot(base.transform.up, Vector3.down) > 0f)
		{
			this.keyState = 3;
			this.interactor.keyState = 0;
			this.canRun = false;
			this.controlled = false;
			this.WheelRearLeft.brakeTorque = 500f;
			this.WheelRearRight.brakeTorque = 500f;
			this.WheelFrontLeft.brakeTorque = 500f;
			this.WheelFrontRight.brakeTorque = 500f;
			this.turnOffAcc();
		}
		if (this.usingV8 && this.enginescriptv8.canRun && this.waterFlood == 0 && Vector3.Dot(base.transform.up, Vector3.down) <= 0f)
		{
			this.canRun = true;
		}
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x000745F0 File Offset: 0x000727F0
	public void enable4(bool w4status)
	{
		if (w4status)
		{
			this.enableWd4 = true;
			this.fourWheelDrive = true;
			if (this.usingV8 && this.enginescriptv8.transferCase_cnd < 20f)
			{
				this.audioControl.PlaySpecified(24);
				return;
			}
		}
		else
		{
			this.enableWd4 = false;
			this.fourWheelDrive = false;
			if (this.usingV8 && this.enginescriptv8.transferCase_cnd < 20f)
			{
				this.audioControl.PlaySpecified(24);
			}
		}
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x0007466B File Offset: 0x0007286B
	public void turnOnAcc()
	{
		this.SetEmissive(this.wd4Icon, this.wd4Emissive, true);
		this.SetEmissive(this.oilIcon, this.oilEmissive, true);
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x00074695 File Offset: 0x00072895
	public void dimAcc()
	{
		this.DimEmissive(this.wd4Icon, this.wd4Emissive, true);
		this.DimEmissive(this.oilIcon, this.oilEmissive, true);
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x000746BF File Offset: 0x000728BF
	public void turnOffAcc()
	{
		this.DarkenDash();
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x000746C8 File Offset: 0x000728C8
	public void DarkenDash()
	{
		this.SetEmissive(this.wd4Icon, this.wd4Emissive, false);
		this.SetEmissive(this.oilIcon, this.oilEmissive, false);
		if (this.isDigital)
		{
			this.fDash.SetColor("_EmissionColor", new UnityEngine.Color(0f, 0f, 0f, 1f));
			this.tachColor.color = new UnityEngine.Color(0f, 0f, 0f, 1f);
			this.mphColor.color = new UnityEngine.Color(0f, 0f, 0f, 1f);
			this.SetEmissive(this.wd4Icon2, this.wd4Emissive, false);
		}
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x00074790 File Offset: 0x00072990
	public void IlluminateDash()
	{
		if (this.isDigital)
		{
			this.fDash.SetColor("_EmissionColor", new UnityEngine.Color(this.rgbR, this.rgbG, this.rgbB, 1f));
			this.tachColor.color = new UnityEngine.Color(this.rgbR, this.rgbG, this.rgbB, 1f);
			this.mphColor.color = new UnityEngine.Color(this.rgbR, this.rgbG, this.rgbB, 1f);
		}
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x00002188 File Offset: 0x00000388
	private void CameraSwitch()
	{
	}

	// Token: 0x060008EB RID: 2283 RVA: 0x00074820 File Offset: 0x00072A20
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

	// Token: 0x060008EC RID: 2284 RVA: 0x0007486C File Offset: 0x00072A6C
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

	// Token: 0x060008ED RID: 2285 RVA: 0x000748C5 File Offset: 0x00072AC5
	public void v8Brackets()
	{
		this.brackets8.SetActive(true);
	}

	// Token: 0x04001457 RID: 5207
	public bool controlled;

	// Token: 0x04001458 RID: 5208
	public bool userControlled;

	// Token: 0x04001459 RID: 5209
	public bool fourWheelDrive;

	// Token: 0x0400145A RID: 5210
	public GameObject person;

	// Token: 0x0400145B RID: 5211
	public Transform centerOfMass;

	// Token: 0x0400145C RID: 5212
	public Transform steerWheel;

	// Token: 0x0400145D RID: 5213
	public Transform theDest;

	// Token: 0x0400145E RID: 5214
	public float steerAngle;

	// Token: 0x0400145F RID: 5215
	public GameObject[] cams;

	// Token: 0x04001460 RID: 5216
	public Interactor interactor;

	// Token: 0x04001461 RID: 5217
	public UnityEngine.Color color;

	// Token: 0x04001462 RID: 5218
	public float Cdrag = 0.2f;

	// Token: 0x04001463 RID: 5219
	public float Fdrag;

	// Token: 0x04001464 RID: 5220
	public float maxFuel = 1300f;

	// Token: 0x04001465 RID: 5221
	public float minFuel;

	// Token: 0x04001466 RID: 5222
	public float fuel;

	// Token: 0x04001467 RID: 5223
	public float temperature = 60f;

	// Token: 0x04001468 RID: 5224
	public float maxTemp;

	// Token: 0x04001469 RID: 5225
	public float coolantLevel = 100f;

	// Token: 0x0400146A RID: 5226
	public float oilLevel = 100f;

	// Token: 0x0400146B RID: 5227
	private float tempAngle;

	// Token: 0x0400146C RID: 5228
	private float fuelAngle;

	// Token: 0x0400146D RID: 5229
	private float oilAngle;

	// Token: 0x0400146E RID: 5230
	public float maxTorque;

	// Token: 0x0400146F RID: 5231
	public float maxTorqueStatic = 700f;

	// Token: 0x04001470 RID: 5232
	public float maxBrakeTorque = 500f;

	// Token: 0x04001471 RID: 5233
	public float MaxWheelRotateAngle = 14f;

	// Token: 0x04001472 RID: 5234
	public float lowestSpeedAtSteer = 45f;

	// Token: 0x04001473 RID: 5235
	public float lowSpeedSteerAngle = 37f;

	// Token: 0x04001474 RID: 5236
	public float highSpeedSteerAngle = 2f;

	// Token: 0x04001475 RID: 5237
	public float torque;

	// Token: 0x04001476 RID: 5238
	public float frontTorque;

	// Token: 0x04001477 RID: 5239
	public float rearTorque;

	// Token: 0x04001478 RID: 5240
	public float maxSpeed;

	// Token: 0x04001479 RID: 5241
	public float steerWheelAngle;

	// Token: 0x0400147A RID: 5242
	public float steerWheelRotateFactor = 1f;

	// Token: 0x0400147B RID: 5243
	public bool braked;

	// Token: 0x0400147C RID: 5244
	public float rockerSpeed = 40f;

	// Token: 0x0400147D RID: 5245
	public bool fanSpin;

	// Token: 0x0400147E RID: 5246
	public WheelCollider WheelFrontRight;

	// Token: 0x0400147F RID: 5247
	public WheelCollider WheelFrontLeft;

	// Token: 0x04001480 RID: 5248
	public WheelCollider WheelRearRight;

	// Token: 0x04001481 RID: 5249
	public WheelCollider WheelRearLeft;

	// Token: 0x04001482 RID: 5250
	public GameObject WheelFrontRightGo;

	// Token: 0x04001483 RID: 5251
	public GameObject WheelFrontLeftGo;

	// Token: 0x04001484 RID: 5252
	public GameObject WheelRearRightGo;

	// Token: 0x04001485 RID: 5253
	public GameObject WheelRearLeftGo;

	// Token: 0x04001486 RID: 5254
	public GameObject wheelRay;

	// Token: 0x04001487 RID: 5255
	public int surfaceType;

	// Token: 0x04001488 RID: 5256
	public GameObject intLight1;

	// Token: 0x04001489 RID: 5257
	public GameObject intLight2;

	// Token: 0x0400148A RID: 5258
	public GameObject intLight3;

	// Token: 0x0400148B RID: 5259
	public GameObject intLight4;

	// Token: 0x0400148C RID: 5260
	private bool intLightsOn;

	// Token: 0x0400148D RID: 5261
	public GameObject wheelFR;

	// Token: 0x0400148E RID: 5262
	public GameObject wheelFL;

	// Token: 0x0400148F RID: 5263
	public GameObject wheelRR;

	// Token: 0x04001490 RID: 5264
	public GameObject wheelRL;

	// Token: 0x04001491 RID: 5265
	public GameObject rotorFL;

	// Token: 0x04001492 RID: 5266
	public int frontGear;

	// Token: 0x04001493 RID: 5267
	public int rearGear;

	// Token: 0x04001494 RID: 5268
	public Renderer frontGear4Rend;

	// Token: 0x04001495 RID: 5269
	public Renderer frontGear5Rend;

	// Token: 0x04001496 RID: 5270
	public Renderer rearGear4Rend;

	// Token: 0x04001497 RID: 5271
	public Renderer rearGear5Rend;

	// Token: 0x04001498 RID: 5272
	public int transmission;

	// Token: 0x04001499 RID: 5273
	public float speed;

	// Token: 0x0400149A RID: 5274
	private float oldAngle;

	// Token: 0x0400149B RID: 5275
	private float newAngle;

	// Token: 0x0400149C RID: 5276
	private float timer;

	// Token: 0x0400149D RID: 5277
	private float angleChangeTimer;

	// Token: 0x0400149E RID: 5278
	private float steerwheelOffset;

	// Token: 0x0400149F RID: 5279
	public float wd4Emissive;

	// Token: 0x040014A0 RID: 5280
	public float oilEmissive;

	// Token: 0x040014A1 RID: 5281
	public float encheckEmissive;

	// Token: 0x040014A2 RID: 5282
	public float tempEmissive;

	// Token: 0x040014A3 RID: 5283
	public float fuelEmissive;

	// Token: 0x040014A4 RID: 5284
	public float chgEmissive;

	// Token: 0x040014A5 RID: 5285
	public float ebrakeEmissive;

	// Token: 0x040014A6 RID: 5286
	public GameObject wd4Icon;

	// Token: 0x040014A7 RID: 5287
	public GameObject wd4Icon2;

	// Token: 0x040014A8 RID: 5288
	public GameObject oilIcon;

	// Token: 0x040014A9 RID: 5289
	public GameObject encheckIcon;

	// Token: 0x040014AA RID: 5290
	public GameObject tempIcon;

	// Token: 0x040014AB RID: 5291
	public GameObject fuelIcon;

	// Token: 0x040014AC RID: 5292
	public GameObject chgIcon;

	// Token: 0x040014AD RID: 5293
	public GameObject ebrakeIcon;

	// Token: 0x040014AE RID: 5294
	public bool checkEngine;

	// Token: 0x040014AF RID: 5295
	public bool enableWd4;

	// Token: 0x040014B0 RID: 5296
	public bool enableOil;

	// Token: 0x040014B1 RID: 5297
	public GameObject pointerTemp;

	// Token: 0x040014B2 RID: 5298
	public GameObject pointerFuel;

	// Token: 0x040014B3 RID: 5299
	public GameObject pointerOil;

	// Token: 0x040014B4 RID: 5300
	public GameObject pointerAlt;

	// Token: 0x040014B5 RID: 5301
	public GameObject engineBlockV8;

	// Token: 0x040014B6 RID: 5302
	public GameObject crankV8;

	// Token: 0x040014B7 RID: 5303
	public GameObject altFan;

	// Token: 0x040014B8 RID: 5304
	public GameObject timingGear;

	// Token: 0x040014B9 RID: 5305
	public GameObject v8Fan;

	// Token: 0x040014BA RID: 5306
	public GameObject turbineD;

	// Token: 0x040014BB RID: 5307
	public GameObject turbineP;

	// Token: 0x040014BC RID: 5308
	public bool canRun;

	// Token: 0x040014BD RID: 5309
	public bool canCrank;

	// Token: 0x040014BE RID: 5310
	public bool canAcc = true;

	// Token: 0x040014BF RID: 5311
	public bool canEasyStart;

	// Token: 0x040014C0 RID: 5312
	public bool canMove;

	// Token: 0x040014C1 RID: 5313
	public int keyState;

	// Token: 0x040014C2 RID: 5314
	private GameObject mudemitFL;

	// Token: 0x040014C3 RID: 5315
	private GameObject mudemitFR;

	// Token: 0x040014C4 RID: 5316
	private GameObject mudemitRR;

	// Token: 0x040014C5 RID: 5317
	private GameObject mudemitRL;

	// Token: 0x040014C6 RID: 5318
	public GameObject mudBrushRL;

	// Token: 0x040014C7 RID: 5319
	public GameObject mudBrushRR;

	// Token: 0x040014C8 RID: 5320
	public GameObject mudBrushFL;

	// Token: 0x040014C9 RID: 5321
	public GameObject mudBrushFR;

	// Token: 0x040014CA RID: 5322
	private int interval = 1;

	// Token: 0x040014CB RID: 5323
	private float nextTime;

	// Token: 0x040014CC RID: 5324
	public float jumptime;

	// Token: 0x040014CD RID: 5325
	private int engineCheckInterval = 2;

	// Token: 0x040014CE RID: 5326
	private int materialCheckInterval = 2;

	// Token: 0x040014CF RID: 5327
	public GameObject heatTrails;

	// Token: 0x040014D0 RID: 5328
	public GameObject exhaustTrails;

	// Token: 0x040014D1 RID: 5329
	public GameObject steamTrails;

	// Token: 0x040014D2 RID: 5330
	public GameObject steamTrails2;

	// Token: 0x040014D3 RID: 5331
	public GameObject tireExplosion;

	// Token: 0x040014D4 RID: 5332
	public bool steamTrailsOn;

	// Token: 0x040014D5 RID: 5333
	public bool steamTrailsOn2;

	// Token: 0x040014D6 RID: 5334
	public GameObject steamTrailsV8;

	// Token: 0x040014D7 RID: 5335
	public GameObject steamTrails2V8;

	// Token: 0x040014D8 RID: 5336
	public bool steamTrailsOnV8;

	// Token: 0x040014D9 RID: 5337
	public bool steamTrailsOn2V8;

	// Token: 0x040014DA RID: 5338
	public bool heatTrailsOn;

	// Token: 0x040014DB RID: 5339
	public bool exhaustTrailsOn;

	// Token: 0x040014DC RID: 5340
	public enginev8 enginescriptv8;

	// Token: 0x040014DD RID: 5341
	public GroundDetect groundDetect;

	// Token: 0x040014DE RID: 5342
	public int deepMud;

	// Token: 0x040014DF RID: 5343
	public int waterFlood;

	// Token: 0x040014E0 RID: 5344
	public bool jacked;

	// Token: 0x040014E1 RID: 5345
	public TruckBedGrav gravShell;

	// Token: 0x040014E2 RID: 5346
	public Renderer element;

	// Token: 0x040014E3 RID: 5347
	private int randDisengage;

	// Token: 0x040014E4 RID: 5348
	public Rigidbody rb;

	// Token: 0x040014E5 RID: 5349
	public bool clutchIn;

	// Token: 0x040014E6 RID: 5350
	public bool physClutchIn;

	// Token: 0x040014E7 RID: 5351
	private float clutchPoppedTime = 7f;

	// Token: 0x040014E8 RID: 5352
	private float clutchTorque;

	// Token: 0x040014E9 RID: 5353
	private float torqueBonus;

	// Token: 0x040014EA RID: 5354
	public bool usingV8 = true;

	// Token: 0x040014EB RID: 5355
	public GameObject RRholder;

	// Token: 0x040014EC RID: 5356
	public GameObject RLholder;

	// Token: 0x040014ED RID: 5357
	public GameObject FRholder;

	// Token: 0x040014EE RID: 5358
	public GameObject FLholder;

	// Token: 0x040014EF RID: 5359
	public Transform RRpos;

	// Token: 0x040014F0 RID: 5360
	public Transform RLpos;

	// Token: 0x040014F1 RID: 5361
	public Transform FRpos;

	// Token: 0x040014F2 RID: 5362
	public Transform FLpos;

	// Token: 0x040014F3 RID: 5363
	public GameObject brackets8;

	// Token: 0x040014F4 RID: 5364
	public ControlRef cr;

	// Token: 0x040014F5 RID: 5365
	public bool isDigital;

	// Token: 0x040014F6 RID: 5366
	public bool fastStart;

	// Token: 0x040014F7 RID: 5367
	public bool fastStartV8;

	// Token: 0x040014F8 RID: 5368
	public bool cleanInterior;

	// Token: 0x040014F9 RID: 5369
	public bool fuelSaving;

	// Token: 0x040014FA RID: 5370
	public bool soundMat;

	// Token: 0x040014FB RID: 5371
	public bool hasDiffLock;

	// Token: 0x040014FC RID: 5372
	public bool has5th;

	// Token: 0x040014FD RID: 5373
	public bool hasRollbar;

	// Token: 0x040014FE RID: 5374
	public GameObject digitalDisplay;

	// Token: 0x040014FF RID: 5375
	public GameObject speedNeedle;

	// Token: 0x04001500 RID: 5376
	public GameObject interior;

	// Token: 0x04001501 RID: 5377
	public GameObject diffLockButton;

	// Token: 0x04001502 RID: 5378
	public GameObject diffLockLed;

	// Token: 0x04001503 RID: 5379
	public GameObject diffLock1;

	// Token: 0x04001504 RID: 5380
	public GameObject diffLock2;

	// Token: 0x04001505 RID: 5381
	public GameObject rollbar;

	// Token: 0x04001506 RID: 5382
	public GameObject kcSwitch;

	// Token: 0x04001507 RID: 5383
	public GameObject rgbDial;

	// Token: 0x04001508 RID: 5384
	public float rgbR;

	// Token: 0x04001509 RID: 5385
	public float rgbB;

	// Token: 0x0400150A RID: 5386
	public float rgbG;

	// Token: 0x0400150B RID: 5387
	public Material fDash;

	// Token: 0x0400150C RID: 5388
	public UnityEngine.UI.Image tachColor;

	// Token: 0x0400150D RID: 5389
	public UnityEngine.UI.Image mphColor;

	// Token: 0x0400150E RID: 5390
	public LensFlare kc1;

	// Token: 0x0400150F RID: 5391
	public LensFlare kc2;

	// Token: 0x04001510 RID: 5392
	public LensFlare kc3;

	// Token: 0x04001511 RID: 5393
	public LensFlare kc4;

	// Token: 0x04001512 RID: 5394
	public Light kcLight;

	// Token: 0x04001513 RID: 5395
	public Text rpmText;

	// Token: 0x04001514 RID: 5396
	public Text mphText;

	// Token: 0x04001515 RID: 5397
	public Text oilText;

	// Token: 0x04001516 RID: 5398
	public Text voltText;

	// Token: 0x04001517 RID: 5399
	public Text tempText;

	// Token: 0x04001518 RID: 5400
	public Text fuelText;

	// Token: 0x04001519 RID: 5401
	public Slider rpmSlider;

	// Token: 0x0400151A RID: 5402
	public Slider mphSlider;

	// Token: 0x0400151B RID: 5403
	public bool diffLocked;

	// Token: 0x0400151C RID: 5404
	public GearBox3 gb;

	// Token: 0x0400151D RID: 5405
	public AudioClip[] heavyDoor;

	// Token: 0x0400151E RID: 5406
	public AudioSource lDoor;

	// Token: 0x0400151F RID: 5407
	public AudioSource rDoor;

	// Token: 0x04001520 RID: 5408
	public BoltScriptSusp sbolt1;

	// Token: 0x04001521 RID: 5409
	public BoltScriptSusp sbolt2;

	// Token: 0x04001522 RID: 5410
	public BoltScriptSusp sbolt3;

	// Token: 0x04001523 RID: 5411
	public BoltScriptSusp sbolt4;

	// Token: 0x04001524 RID: 5412
	private int i;

	// Token: 0x04001525 RID: 5413
	private Quaternion rot;

	// Token: 0x04001526 RID: 5414
	private float newblend;

	// Token: 0x04001527 RID: 5415
	public AudioControlF audioControl;

	// Token: 0x04001528 RID: 5416
	public int deepWater;

	// Token: 0x04001529 RID: 5417
	public GameObject[] splashFx;

	// Token: 0x0400152A RID: 5418
	public bool hasTruck2;

	// Token: 0x0400152B RID: 5419
	public TrailerBedGrav flatbed;

	// Token: 0x0400152C RID: 5420
	private bool hangTimeChallenge;

	// Token: 0x0400152D RID: 5421
	private float airTime;

	// Token: 0x0400152E RID: 5422
	public MissionController mc;

	// Token: 0x0400152F RID: 5423
	public ModWomanJobs mw;

	// Token: 0x04001530 RID: 5424
	public float additive;

	// Token: 0x04001531 RID: 5425
	private float additiveBonus;

	// Token: 0x04001532 RID: 5426
	public Light brakeLightR;

	// Token: 0x04001533 RID: 5427
	public Light brakeLightL;

	// Token: 0x04001534 RID: 5428
	public Light revLightR;

	// Token: 0x04001535 RID: 5429
	public Light revLightL;
}
