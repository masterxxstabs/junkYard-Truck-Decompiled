using System;
using System.Collections;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200018E RID: 398
public class car : MonoBehaviour
{
	// Token: 0x060009B7 RID: 2487 RVA: 0x00083248 File Offset: 0x00081448
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
		if (this.usingV8 && !this.usingI6)
		{
			this.v8Brackets();
		}
		else if (this.usingI6)
		{
			this.i6Brackets();
		}
		else
		{
			this.v4Brackets();
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
		if (this.isDigital)
		{
			this.digitalDisplay.SetActive(true);
			this.pointerFuel.GetComponent<Renderer>().enabled = false;
			this.pointerTemp.GetComponent<Renderer>().enabled = false;
			this.tachNeedle.GetComponent<Renderer>().enabled = false;
			this.speedNeedle.GetComponent<Renderer>().enabled = false;
			this.rgbDial.SetActive(true);
			this.dbDash.SetColor("_EmissionColor", new UnityEngine.Color(this.rgbR, this.rgbG, this.rgbB, 1f));
			this.tachColor.color = new UnityEngine.Color(this.rgbR, this.rgbG, this.rgbB, 1f);
			this.mphColor.color = new UnityEngine.Color(this.rgbR, this.rgbG, this.rgbB, 1f);
		}
		if (this.hasSeat)
		{
			this.passSeat.SetActive(true);
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
			if (this.diffLocked)
			{
				this.diffLockLed.SetActive(true);
			}
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

	// Token: 0x060009B8 RID: 2488 RVA: 0x00083AE0 File Offset: 0x00081CE0
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

	// Token: 0x060009B9 RID: 2489 RVA: 0x00083B48 File Offset: 0x00081D48
	private void Update()
	{
		if (this.controlled)
		{
			this.control();
			if (Input.GetKeyDown(this.cr.Clutch))
			{
				this.enginescript.canMove = false;
				this.enginescripti6.canMove = false;
				this.enginescriptv8.canMove = false;
				this.clutchIn = true;
			}
			if (Input.GetKeyUp(this.cr.Clutch))
			{
				this.enginescript.canMove = true;
				this.enginescripti6.canMove = true;
				this.enginescriptv8.canMove = true;
				this.clutchIn = false;
				if (this.clutchPoppedTime > 6f)
				{
					this.clutchPoppedTime = 0.1f;
				}
			}
			if (this.cr.ClutchInput > 0.3f)
			{
				this.enginescript.canMove = false;
				this.enginescriptv8.canMove = false;
				this.enginescripti6.canMove = false;
				this.clutchIn = true;
				this.physClutchIn = true;
			}
			if (this.cr.ClutchInput < 0.3f && this.physClutchIn)
			{
				this.enginescript.canMove = true;
				this.enginescriptv8.canMove = true;
				this.enginescripti6.canMove = true;
				this.clutchIn = false;
				this.physClutchIn = false;
				if (this.clutchPoppedTime > 6f)
				{
					this.clutchPoppedTime = 0.1f;
				}
			}
			if (!this.intLightsOn && this.canAcc)
			{
				this.intLight1.GetComponent<Light>().enabled = true;
				this.intLight2.GetComponent<Light>().enabled = true;
				this.intLight3.GetComponent<Light>().enabled = true;
				this.intLight4.GetComponent<Light>().enabled = true;
				this.intLightsOn = true;
			}
			this.wd4Emissive = this.SetEmissive(this.wd4Icon, this.wd4Emissive, this.enableWd4);
			if (!this.usingV8 && !this.usingI6)
			{
				if (this.fanSpin)
				{
					this.engineFan.transform.Rotate(0f, 0f, 720f * Time.deltaTime);
				}
				this.camAndGear.transform.Rotate(0f, 0f, 720f * Time.deltaTime, Space.Self);
				this.camBearing1.transform.Rotate(0f, 0f, -720f * Time.deltaTime, Space.Self);
				this.camBearing2.transform.Rotate(0f, 0f, -720f * Time.deltaTime, Space.Self);
				this.camBearing3.transform.Rotate(0f, 0f, -720f * Time.deltaTime, Space.Self);
				this.helical30r.transform.Rotate(0f, 0f, -720f * Time.deltaTime);
				this.engineCrank.transform.Rotate(0f, 0f, 1440f * Time.deltaTime);
				this.rocker1.transform.Rotate(1f * Mathf.Sin(Time.deltaTime * this.rockerSpeed), 0f, 0f);
				this.rocker2.transform.Rotate(1f * Mathf.Sin(Time.deltaTime * (this.rockerSpeed + 1f)), 0f, 0f);
				this.rocker3.transform.Rotate(1f * Mathf.Sin(Time.deltaTime * (this.rockerSpeed + 2f)), 0f, 0f);
				this.rocker4.transform.Rotate(1f * Mathf.Sin(Time.deltaTime * (this.rockerSpeed + 3f)), 0f, 0f);
				this.rocker5.transform.Rotate(1f * Mathf.Sin(Time.deltaTime * this.rockerSpeed), 0f, 0f);
				this.rocker6.transform.Rotate(1f * Mathf.Sin(Time.deltaTime * (this.rockerSpeed + 1f)), 0f, 0f);
				this.rocker7.transform.Rotate(1f * Mathf.Sin(Time.deltaTime * (this.rockerSpeed + 2f)), 0f, 0f);
				this.rocker8.transform.Rotate(1f * Mathf.Sin(Time.deltaTime * (this.rockerSpeed + 3f)), 0f, 0f);
				this.engineBlock.transform.Rotate(0f, 0.02f * Mathf.Sin(Time.deltaTime * 20f), 0f);
				return;
			}
			if (this.usingI6)
			{
				this.i6Fan.transform.Rotate(0f, 720f * Time.deltaTime, 0f);
				this.i6Crank.transform.Rotate(0f, 720f * Time.deltaTime, 0f);
				this.i6Cam.transform.Rotate(0f, 720f * Time.deltaTime, 0f);
				this.i6Cam2.transform.Rotate(0f, 720f * Time.deltaTime, 0f);
				this.i6Pulley1.transform.Rotate(0f, 0f, 720f * Time.deltaTime);
				this.i6Pulley2.transform.Rotate(0f, 720f * Time.deltaTime, 0f);
				this.i6Pulley3.transform.Rotate(0f, 720f * Time.deltaTime, 0f);
				return;
			}
			this.crankV8.transform.Rotate(0f, -720f * Time.deltaTime, 0f);
			this.altFan.transform.Rotate(0f, -360f * Time.deltaTime, 0f);
			this.v8Fan.transform.Rotate(0f, -720f * Time.deltaTime, 0f);
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

	// Token: 0x060009BA RID: 2490 RVA: 0x0008420C File Offset: 0x0008240C
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

	// Token: 0x060009BB RID: 2491 RVA: 0x0008427C File Offset: 0x0008247C
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

	// Token: 0x060009BC RID: 2492 RVA: 0x000842CC File Offset: 0x000824CC
	public void CheckSusp()
	{
		if (this.sbolt1.boltTurns == 25 && this.sbolt2.boltTurns == 25 && this.sbolt3.boltTurns == -25 && this.sbolt4.boltTurns == -25)
		{
			Achievement achievement = new Achievement("ACH_DISASTER");
			achievement.Trigger(true);
		}
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x0008432C File Offset: 0x0008252C
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

	// Token: 0x060009BE RID: 2494 RVA: 0x000844AC File Offset: 0x000826AC
	public void KcLight()
	{
		this.kc1.enabled = !this.kc1.enabled;
		this.kc2.enabled = !this.kc2.enabled;
		this.kc3.enabled = !this.kc3.enabled;
		this.kc4.enabled = !this.kc4.enabled;
		this.kcLight.enabled = !this.kcLight.enabled;
		if (this.interactor.truckLightRL.GetComponent<Light>().enabled)
		{
			this.interactor.truckLightRL.GetComponent<Light>().enabled = false;
			this.interactor.truckLightL.GetComponent<LensFlare>().enabled = false;
			this.interactor.truckLightR.GetComponent<LensFlare>().enabled = false;
		}
	}

	// Token: 0x060009BF RID: 2495 RVA: 0x00084590 File Offset: 0x00082790
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
			if (!this.usingV8 && !this.usingI6)
			{
				if (this.temperature > 230f || (this.enginescript.headgasket_cnd < 30f && this.temperature > 210f))
				{
					if (!this.steamTrailsOn)
					{
						this.steamTrails.GetComponent<ParticleSystem>().Play();
						this.steamTrailsOn = true;
					}
				}
				else if (this.steamTrailsOn)
				{
					this.steamTrails.GetComponent<ParticleSystem>().Stop();
					this.steamTrailsOn = false;
				}
				if (this.temperature > 240f)
				{
					if (!this.steamTrailsOn2)
					{
						this.steamTrails2.GetComponent<ParticleSystem>().Play();
						this.steamTrailsOn2 = true;
					}
				}
				else if (this.steamTrailsOn2)
				{
					this.steamTrails2.GetComponent<ParticleSystem>().Stop();
					this.steamTrailsOn2 = false;
				}
			}
			else if (this.usingI6)
			{
				if (this.temperature > 230f)
				{
					if (!this.steamTrailsOni6)
					{
						this.steamTrailsi6.GetComponent<ParticleSystem>().Play();
						this.steamTrailsOni6 = true;
					}
				}
				else if (this.steamTrailsOni6)
				{
					this.steamTrailsi6.GetComponent<ParticleSystem>().Stop();
					this.steamTrailsOni6 = false;
				}
				if (this.temperature > 240f)
				{
					if (!this.steamTrailsOn2i6)
					{
						this.steamTrails2V8.GetComponent<ParticleSystem>().Play();
						this.steamTrailsOn2i6 = true;
					}
				}
				else if (this.steamTrailsOn2i6)
				{
					this.steamTrails2i6.GetComponent<ParticleSystem>().Stop();
					this.steamTrailsOn2i6 = false;
				}
			}
			else if (this.usingV8)
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
							this.mc.trophyAirtime = 1;
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
			this.tempAngle = Mathf.Abs(this.temperature / 5f) * -1.2f + 70f;
			if (this.tempAngle < 12f)
			{
				this.tempAngle = 12f;
			}
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
				this.fuel -= 0.7f * Time.deltaTime;
			}
			else
			{
				this.fuel -= 0.5f * Time.deltaTime;
			}
			this.fuelAngle = this.fuel / 18.5f - 70f;
			if (this.additive > 1f)
			{
				this.additive -= 0.3f * Time.deltaTime;
			}
			this.pointerFuel.transform.localRotation = Quaternion.Euler(0f, 0f, this.fuelAngle);
			if (this.isDigital)
			{
				if (!this.usingV8 && !this.usingI6)
				{
					if (this.enginescript.alternator_cnd > 5f)
					{
						this.voltText.text = "13\nVOLTS";
					}
					else
					{
						this.voltText.text = "0\nVOLTS";
					}
				}
				else if (this.usingI6)
				{
					if (this.enginescripti6.alternator_cnd > 5f)
					{
						this.voltText.text = "13\nVOLTS";
					}
					else
					{
						this.voltText.text = "0\nVOLTS";
					}
				}
				else if (this.enginescriptv8.alternator_cnd > 5f)
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
			if (this.boostGauge.activeSelf)
			{
				float num = 0f;
				if (this.usingI6)
				{
					if (this.enginescripti6.exhaustMan_cnd > 0f)
					{
						num = this.gb.currentPitch * 2f * (this.enginescripti6.exhaustMan_cnd / 50f);
						num /= 12f;
					}
				}
				else if (this.usingV8)
				{
					if (this.enginescriptv8.hasTurboD && this.enginescriptv8.hasTurboP)
					{
						float num2 = this.gb.currentPitch * (this.enginescriptv8.am_turboP_cnd / 50f);
						float num3 = this.gb.currentPitch * (this.enginescriptv8.am_turboD_cnd / 50f);
						num = (num2 + num3) / 10f;
					}
				}
				else
				{
					float num4 = 0f;
					float num5 = 0f;
					if (this.enginescript.hasTurbo)
					{
						num4 = this.gb.currentPitch * (this.enginescript.turbo_cnd / 50f);
					}
					if (this.enginescript.hasTurbo2)
					{
						num5 = this.gb.currentPitch * (this.enginescript.turboXL_cnd / 50f);
					}
					num = (num4 + num5) / 20f;
				}
				this.boostSlider.value = num;
				if (this.usingI6)
				{
					this.boostText.text = Mathf.Round(num * 12f).ToString();
				}
				else if (this.usingV8)
				{
					this.boostText.text = Mathf.Round(num * 16f).ToString();
				}
				else
				{
					this.boostText.text = Mathf.Round(num * 10f).ToString();
				}
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
				this.deepMud = this.groundDetect.deepMud;
				if (this.deepMud == 0)
				{
					this.mudFx2[0].SetActive(false);
					this.mudFx2[1].SetActive(false);
					this.mudFx2[2].SetActive(false);
					this.mudFx2[3].SetActive(false);
				}
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
				if (!this.usingV8 && !this.usingI6)
				{
					this.enginescript.DegradeEngine();
				}
				else if (this.usingI6)
				{
					this.enginescripti6.DegradeEngine();
				}
				else
				{
					this.enginescriptv8.DegradeEngine();
				}
				if (this.speed > 5f)
				{
					this.DegradeTires(0);
				}
				if (!this.usingV8 && !this.usingI6)
				{
					if (this.enginescript.transferCase_cnd < 15f)
					{
						this.randDisengage = Random.Range(0, 7);
						if (this.randDisengage == 2 && this.enableWd4)
						{
							this.enable4(false);
						}
					}
				}
				else if (this.usingI6)
				{
					if (this.enginescripti6.transferCase_cnd < 15f)
					{
						this.randDisengage = Random.Range(0, 7);
						if (this.randDisengage == 2 && this.enableWd4)
						{
							this.enable4(false);
						}
					}
				}
				else if (this.enginescriptv8.transferCase_cnd < 15f)
				{
					this.randDisengage = Random.Range(0, 7);
					if (this.randDisengage == 2 && this.enableWd4)
					{
						this.enable4(false);
					}
				}
			}
			if (this.deepWater == 1)
			{
				float num6 = this.WheelFrontLeft.rpm;
				num6 = Mathf.Clamp(num6, 50f, 300f);
				float startSpeed = Mathf.Lerp(0f, 5f, (num6 - 50f) / 250f);
				float constant = Mathf.Lerp(0f, 100f, (num6 - 50f) / 250f);
				ParticleSystem.EmissionModule emission = this.psFL.emission;
				ParticleSystem.EmissionModule emission2 = this.psFR.emission;
				ParticleSystem.EmissionModule emission3 = this.psRL.emission;
				ParticleSystem.EmissionModule emission4 = this.psRR.emission;
				this.psFL.startSpeed = startSpeed;
				emission.rateOverTime = constant;
				this.psFR.startSpeed = startSpeed;
				emission2.rateOverTime = constant;
				this.psRL.startSpeed = startSpeed;
				emission3.rateOverTime = constant;
				this.psRR.startSpeed = startSpeed;
				emission4.rateOverTime = constant;
			}
			if (this.deepMud == 1)
			{
				if (!this.mudFx2[0].activeSelf)
				{
					this.mudFx2[0].SetActive(true);
					this.mudFx2[1].SetActive(true);
					this.mudFx2[2].SetActive(true);
					this.mudFx2[3].SetActive(true);
				}
				float num7 = this.WheelFrontLeft.rpm;
				num7 = Mathf.Clamp(num7, 50f, 300f);
				float startSpeed2 = Mathf.Lerp(0f, 5f, (num7 - 50f) / 250f);
				float constant2 = Mathf.Lerp(0f, 100f, (num7 - 50f) / 250f);
				ParticleSystem.EmissionModule emission5 = this.psFLm.emission;
				ParticleSystem.EmissionModule emission6 = this.psFRm.emission;
				ParticleSystem.EmissionModule emission7 = this.psRLm.emission;
				ParticleSystem.EmissionModule emission8 = this.psRRm.emission;
				this.psFLm.startSpeed = startSpeed2;
				emission5.rateOverTime = constant2;
				this.psFRm.startSpeed = startSpeed2;
				emission6.rateOverTime = constant2;
				this.psRLm.startSpeed = startSpeed2;
				emission7.rateOverTime = constant2;
				this.psRRm.startSpeed = startSpeed2;
				emission8.rateOverTime = constant2;
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
				this.intLight1.GetComponent<Light>().enabled = false;
				this.intLight2.GetComponent<Light>().enabled = false;
				this.intLight3.GetComponent<Light>().enabled = false;
				this.intLight4.GetComponent<Light>().enabled = false;
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

	// Token: 0x060009C0 RID: 2496 RVA: 0x00085488 File Offset: 0x00083688
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
			if (!this.usingV8 && !this.usingI6)
			{
				if (this.enginescript.canMove)
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
			else if (this.usingI6)
			{
				if (this.enginescripti6.canMove)
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
			else if (this.enginescriptv8.canMove)
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
				this.maxSpeed = 85f;
			}
			this.frontTorque = this.torque;
		}
		if (this.frontGear == 5)
		{
			if (this.fourWheelDrive)
			{
				this.maxSpeed = 85f;
			}
			this.frontTorque = this.torque * 1.22f;
		}
		if (this.rearGear == 4)
		{
			this.maxSpeed = 85f;
			this.rearTorque = this.torque;
		}
		if (this.rearGear == 5)
		{
			this.maxSpeed = 85f;
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

	// Token: 0x060009C1 RID: 2497 RVA: 0x000859B8 File Offset: 0x00083BB8
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
		float num5 = Mathf.Lerp(20f, 1f, t);
		num5 *= this.cr.Vert;
		num5 = Mathf.Abs(num5);
		this.gasPedal.transform.localRotation = this.rot * Quaternion.Euler(new Vector3(num5, 0f, 0f));
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

	// Token: 0x060009C2 RID: 2498 RVA: 0x00085C04 File Offset: 0x00083E04
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

	// Token: 0x060009C3 RID: 2499 RVA: 0x00085DB5 File Offset: 0x00083FB5
	public void PopTire()
	{
		this.audioControl.PlaySpecified(35);
		this.tireExplosion.GetComponent<ParticleSystem>().Play();
		this.updateEngine();
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x00085DDC File Offset: 0x00083FDC
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

	// Token: 0x060009C5 RID: 2501 RVA: 0x00086280 File Offset: 0x00084480
	public void updateEngine()
	{
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
		if (!this.clutchIn)
		{
			if (!this.usingV8 && !this.usingI6)
			{
				this.enginescript.Refresh();
			}
			else if (this.usingI6)
			{
				this.enginescripti6.Refresh();
			}
			else
			{
				this.enginescriptv8.Refresh();
			}
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
		if (!this.usingV8 && !this.usingI6)
		{
			if (!this.enginescript.canRun)
			{
				this.canRun = false;
				this.controlled = false;
			}
			this.oilLevel = this.enginescript.newOilLevel;
		}
		else if (this.usingI6)
		{
			if (!this.enginescripti6.canRun)
			{
				this.canRun = false;
				this.controlled = false;
			}
			this.oilLevel = this.enginescripti6.newOilLevel;
		}
		else
		{
			if (!this.enginescriptv8.canRun)
			{
				this.canRun = false;
				this.controlled = false;
			}
			this.oilLevel = this.enginescriptv8.newOilLevel;
		}
		if (this.fuel < 2f)
		{
			this.canRun = false;
			this.controlled = false;
		}
		if (this.waterFlood == 1)
		{
			this.turnOffAcc();
			this.keyState = 0;
			this.interactor.keyState = 0;
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
		if (!this.usingV8 && !this.usingI6)
		{
			if (this.enginescript.canRun && this.waterFlood == 0 && Vector3.Dot(base.transform.up, Vector3.down) <= 0f)
			{
				this.canRun = true;
				return;
			}
		}
		else if (this.usingI6)
		{
			if (this.enginescripti6.canRun && this.waterFlood == 0 && Vector3.Dot(base.transform.up, Vector3.down) <= 0f)
			{
				this.canRun = true;
				return;
			}
		}
		else if (this.enginescriptv8.canRun && this.waterFlood == 0 && Vector3.Dot(base.transform.up, Vector3.down) <= 0f)
		{
			this.canRun = true;
		}
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x0008673C File Offset: 0x0008493C
	public void enable4(bool w4status)
	{
		if (w4status)
		{
			this.enableWd4 = true;
			this.fourWheelDrive = true;
			if (!this.usingV8 && !this.usingI6)
			{
				if (this.enginescript.transferCase_cnd < 20f)
				{
					this.audioControl.PlaySpecified(24);
					return;
				}
			}
			else if (this.usingI6)
			{
				if (this.enginescripti6.transferCase_cnd < 20f)
				{
					this.audioControl.PlaySpecified(24);
					return;
				}
			}
			else if (this.enginescriptv8.transferCase_cnd < 20f)
			{
				this.audioControl.PlaySpecified(24);
				return;
			}
		}
		else
		{
			this.enableWd4 = false;
			this.fourWheelDrive = false;
			if (!this.usingV8 && !this.usingI6)
			{
				if (this.enginescript.transferCase_cnd < 20f)
				{
					this.audioControl.PlaySpecified(24);
					return;
				}
			}
			else if (this.usingI6)
			{
				if (this.enginescripti6.transferCase_cnd < 20f)
				{
					this.audioControl.PlaySpecified(24);
					return;
				}
			}
			else if (this.enginescriptv8.transferCase_cnd < 20f)
			{
				this.audioControl.PlaySpecified(24);
			}
		}
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x00086864 File Offset: 0x00084A64
	public void turnOnAcc()
	{
		this.SetEmissive(this.wd4Icon, this.wd4Emissive, true);
		this.SetEmissive(this.oilIcon, this.oilEmissive, true);
		this.SetEmissive(this.chgIcon, this.chgEmissive, true);
		this.SetEmissive(this.ebrakeIcon, this.ebrakeEmissive, true);
		if (this.boostGauge.transform.parent.GetComponent<Renderer>().enabled)
		{
			this.boostGauge.SetActive(true);
		}
	}

	// Token: 0x060009C8 RID: 2504 RVA: 0x000868EC File Offset: 0x00084AEC
	public void dimAcc()
	{
		this.DimEmissive(this.wd4Icon, this.wd4Emissive, true);
		this.DimEmissive(this.oilIcon, this.oilEmissive, true);
		this.DimEmissive(this.chgIcon, this.chgEmissive, true);
		this.DimEmissive(this.ebrakeIcon, this.ebrakeEmissive, true);
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x00086949 File Offset: 0x00084B49
	public void turnOffAcc()
	{
		this.DarkenDash();
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x00086954 File Offset: 0x00084B54
	public void DarkenDash()
	{
		this.SetEmissive(this.wd4Icon, this.wd4Emissive, false);
		this.SetEmissive(this.oilIcon, this.oilEmissive, false);
		this.SetEmissive(this.chgIcon, this.chgEmissive, false);
		this.SetEmissive(this.ebrakeIcon, this.ebrakeEmissive, false);
		if (this.isDigital)
		{
			this.dbDash.SetColor("_EmissionColor", new UnityEngine.Color(0f, 0f, 0f, 1f));
			this.tachColor.color = new UnityEngine.Color(0f, 0f, 0f, 1f);
			this.mphColor.color = new UnityEngine.Color(0f, 0f, 0f, 1f);
		}
		if (this.boostGauge.transform.parent.GetComponent<Renderer>().enabled)
		{
			this.boostGauge.SetActive(false);
		}
	}

	// Token: 0x060009CB RID: 2507 RVA: 0x00086A54 File Offset: 0x00084C54
	public void IlluminateDash()
	{
		if (this.isDigital)
		{
			this.dbDash.SetColor("_EmissionColor", new UnityEngine.Color(this.rgbR, this.rgbG, this.rgbB, 1f));
			this.tachColor.color = new UnityEngine.Color(this.rgbR, this.rgbG, this.rgbB, 1f);
			this.mphColor.color = new UnityEngine.Color(this.rgbR, this.rgbG, this.rgbB, 1f);
		}
		if (this.boostGauge.transform.parent.GetComponent<Renderer>().enabled)
		{
			this.boostGauge.SetActive(true);
		}
	}

	// Token: 0x060009CC RID: 2508 RVA: 0x00002188 File Offset: 0x00000388
	private void CameraSwitch()
	{
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x00086B0C File Offset: 0x00084D0C
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "water" || other.tag == "DynamicWater")
		{
			this.deepWater = 1;
			this.splashFx2[0].SetActive(true);
			this.splashFx2[1].SetActive(true);
			this.splashFx2[2].SetActive(true);
			this.splashFx2[3].SetActive(true);
			if (this.exitRoutine != null)
			{
				base.StopCoroutine(this.exitRoutine);
				this.exitRoutine = null;
			}
		}
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x00086B97 File Offset: 0x00084D97
	private void OnTriggerExit(Collider other)
	{
		if ((other.tag == "water" || other.tag == "DynamicWater") && this.exitRoutine == null)
		{
			this.exitRoutine = base.StartCoroutine(this.DelayedExit());
		}
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x00086BD7 File Offset: 0x00084DD7
	private IEnumerator DelayedExit()
	{
		yield return new WaitForSeconds(1f);
		this.deepWater = 0;
		this.splashFx2[0].SetActive(false);
		this.splashFx2[1].SetActive(false);
		this.splashFx2[2].SetActive(false);
		this.splashFx2[3].SetActive(false);
		this.exitRoutine = null;
		yield break;
	}

	// Token: 0x060009D0 RID: 2512 RVA: 0x00086BE6 File Offset: 0x00084DE6
	public void v4Brackets()
	{
		this.brackets4.SetActive(true);
		this.brackets8.SetActive(false);
		this.brackets6.SetActive(false);
	}

	// Token: 0x060009D1 RID: 2513 RVA: 0x00086C0C File Offset: 0x00084E0C
	public void v8Brackets()
	{
		this.brackets4.SetActive(false);
		this.brackets8.SetActive(true);
		this.brackets6.SetActive(false);
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x00086C32 File Offset: 0x00084E32
	public void i6Brackets()
	{
		this.brackets4.SetActive(false);
		this.brackets8.SetActive(false);
		this.brackets6.SetActive(true);
	}

	// Token: 0x04001A28 RID: 6696
	public bool controlled;

	// Token: 0x04001A29 RID: 6697
	public bool userControlled;

	// Token: 0x04001A2A RID: 6698
	public bool fourWheelDrive;

	// Token: 0x04001A2B RID: 6699
	public GameObject fourcyl;

	// Token: 0x04001A2C RID: 6700
	public GameObject person;

	// Token: 0x04001A2D RID: 6701
	public Transform centerOfMass;

	// Token: 0x04001A2E RID: 6702
	public Transform steerWheel;

	// Token: 0x04001A2F RID: 6703
	public Transform theDest;

	// Token: 0x04001A30 RID: 6704
	public float steerAngle;

	// Token: 0x04001A31 RID: 6705
	public GameObject[] cams;

	// Token: 0x04001A32 RID: 6706
	public Interactor interactor;

	// Token: 0x04001A33 RID: 6707
	public UnityEngine.Color color;

	// Token: 0x04001A34 RID: 6708
	public float Cdrag = 0.2f;

	// Token: 0x04001A35 RID: 6709
	public float Fdrag;

	// Token: 0x04001A36 RID: 6710
	public float maxFuel = 1300f;

	// Token: 0x04001A37 RID: 6711
	public float minFuel;

	// Token: 0x04001A38 RID: 6712
	public float fuel;

	// Token: 0x04001A39 RID: 6713
	public float temperature = 60f;

	// Token: 0x04001A3A RID: 6714
	public float maxTemp;

	// Token: 0x04001A3B RID: 6715
	public float coolantLevel = 100f;

	// Token: 0x04001A3C RID: 6716
	public float oilLevel = 100f;

	// Token: 0x04001A3D RID: 6717
	private float tempAngle;

	// Token: 0x04001A3E RID: 6718
	private float fuelAngle;

	// Token: 0x04001A3F RID: 6719
	public float maxTorque;

	// Token: 0x04001A40 RID: 6720
	public float maxTorqueStatic = 700f;

	// Token: 0x04001A41 RID: 6721
	public float maxBrakeTorque = 500f;

	// Token: 0x04001A42 RID: 6722
	public float MaxWheelRotateAngle = 14f;

	// Token: 0x04001A43 RID: 6723
	public float lowestSpeedAtSteer = 45f;

	// Token: 0x04001A44 RID: 6724
	public float lowSpeedSteerAngle = 37f;

	// Token: 0x04001A45 RID: 6725
	public float highSpeedSteerAngle = 2f;

	// Token: 0x04001A46 RID: 6726
	public float torque;

	// Token: 0x04001A47 RID: 6727
	public float frontTorque;

	// Token: 0x04001A48 RID: 6728
	public float rearTorque;

	// Token: 0x04001A49 RID: 6729
	public float maxSpeed;

	// Token: 0x04001A4A RID: 6730
	public float steerWheelAngle;

	// Token: 0x04001A4B RID: 6731
	public float steerWheelRotateFactor = 1f;

	// Token: 0x04001A4C RID: 6732
	public bool braked;

	// Token: 0x04001A4D RID: 6733
	public float rockerSpeed = 40f;

	// Token: 0x04001A4E RID: 6734
	public bool fanSpin;

	// Token: 0x04001A4F RID: 6735
	public WheelCollider WheelFrontRight;

	// Token: 0x04001A50 RID: 6736
	public WheelCollider WheelFrontLeft;

	// Token: 0x04001A51 RID: 6737
	public WheelCollider WheelRearRight;

	// Token: 0x04001A52 RID: 6738
	public WheelCollider WheelRearLeft;

	// Token: 0x04001A53 RID: 6739
	public GameObject WheelFrontRightGo;

	// Token: 0x04001A54 RID: 6740
	public GameObject WheelFrontLeftGo;

	// Token: 0x04001A55 RID: 6741
	public GameObject WheelRearRightGo;

	// Token: 0x04001A56 RID: 6742
	public GameObject WheelRearLeftGo;

	// Token: 0x04001A57 RID: 6743
	public GameObject gasPedal;

	// Token: 0x04001A58 RID: 6744
	public GameObject wheelRay;

	// Token: 0x04001A59 RID: 6745
	public int surfaceType;

	// Token: 0x04001A5A RID: 6746
	public GameObject intLight1;

	// Token: 0x04001A5B RID: 6747
	public GameObject intLight2;

	// Token: 0x04001A5C RID: 6748
	public GameObject intLight3;

	// Token: 0x04001A5D RID: 6749
	public GameObject intLight4;

	// Token: 0x04001A5E RID: 6750
	private bool intLightsOn;

	// Token: 0x04001A5F RID: 6751
	public GameObject wheelFR;

	// Token: 0x04001A60 RID: 6752
	public GameObject wheelFL;

	// Token: 0x04001A61 RID: 6753
	public GameObject wheelRR;

	// Token: 0x04001A62 RID: 6754
	public GameObject wheelRL;

	// Token: 0x04001A63 RID: 6755
	public GameObject rotorFL;

	// Token: 0x04001A64 RID: 6756
	public int frontGear;

	// Token: 0x04001A65 RID: 6757
	public int rearGear;

	// Token: 0x04001A66 RID: 6758
	public Renderer frontGear4Rend;

	// Token: 0x04001A67 RID: 6759
	public Renderer frontGear5Rend;

	// Token: 0x04001A68 RID: 6760
	public Renderer rearGear4Rend;

	// Token: 0x04001A69 RID: 6761
	public Renderer rearGear5Rend;

	// Token: 0x04001A6A RID: 6762
	public int transmission;

	// Token: 0x04001A6B RID: 6763
	public float speed;

	// Token: 0x04001A6C RID: 6764
	private float oldAngle;

	// Token: 0x04001A6D RID: 6765
	private float newAngle;

	// Token: 0x04001A6E RID: 6766
	private float timer;

	// Token: 0x04001A6F RID: 6767
	private float angleChangeTimer;

	// Token: 0x04001A70 RID: 6768
	private float steerwheelOffset;

	// Token: 0x04001A71 RID: 6769
	public float wd4Emissive;

	// Token: 0x04001A72 RID: 6770
	public float oilEmissive;

	// Token: 0x04001A73 RID: 6771
	public float encheckEmissive;

	// Token: 0x04001A74 RID: 6772
	public float tempEmissive;

	// Token: 0x04001A75 RID: 6773
	public float fuelEmissive;

	// Token: 0x04001A76 RID: 6774
	public float chgEmissive;

	// Token: 0x04001A77 RID: 6775
	public float ebrakeEmissive;

	// Token: 0x04001A78 RID: 6776
	public GameObject wd4Icon;

	// Token: 0x04001A79 RID: 6777
	public GameObject oilIcon;

	// Token: 0x04001A7A RID: 6778
	public GameObject encheckIcon;

	// Token: 0x04001A7B RID: 6779
	public GameObject tempIcon;

	// Token: 0x04001A7C RID: 6780
	public GameObject fuelIcon;

	// Token: 0x04001A7D RID: 6781
	public GameObject chgIcon;

	// Token: 0x04001A7E RID: 6782
	public GameObject ebrakeIcon;

	// Token: 0x04001A7F RID: 6783
	public bool checkEngine;

	// Token: 0x04001A80 RID: 6784
	public bool enableWd4;

	// Token: 0x04001A81 RID: 6785
	public bool enableOil;

	// Token: 0x04001A82 RID: 6786
	public GameObject pointerTemp;

	// Token: 0x04001A83 RID: 6787
	public GameObject pointerFuel;

	// Token: 0x04001A84 RID: 6788
	public GameObject engineBlock;

	// Token: 0x04001A85 RID: 6789
	public GameObject engineFan;

	// Token: 0x04001A86 RID: 6790
	public GameObject camAndGear;

	// Token: 0x04001A87 RID: 6791
	public GameObject helical30r;

	// Token: 0x04001A88 RID: 6792
	public GameObject engineCrank;

	// Token: 0x04001A89 RID: 6793
	public GameObject rocker1;

	// Token: 0x04001A8A RID: 6794
	public GameObject rocker2;

	// Token: 0x04001A8B RID: 6795
	public GameObject rocker3;

	// Token: 0x04001A8C RID: 6796
	public GameObject rocker4;

	// Token: 0x04001A8D RID: 6797
	public GameObject rocker5;

	// Token: 0x04001A8E RID: 6798
	public GameObject rocker6;

	// Token: 0x04001A8F RID: 6799
	public GameObject rocker7;

	// Token: 0x04001A90 RID: 6800
	public GameObject rocker8;

	// Token: 0x04001A91 RID: 6801
	public GameObject camBearing1;

	// Token: 0x04001A92 RID: 6802
	public GameObject camBearing2;

	// Token: 0x04001A93 RID: 6803
	public GameObject camBearing3;

	// Token: 0x04001A94 RID: 6804
	public GameObject i6Fan;

	// Token: 0x04001A95 RID: 6805
	public GameObject i6Crank;

	// Token: 0x04001A96 RID: 6806
	public GameObject i6Cam;

	// Token: 0x04001A97 RID: 6807
	public GameObject i6Cam2;

	// Token: 0x04001A98 RID: 6808
	public GameObject i6Pulley1;

	// Token: 0x04001A99 RID: 6809
	public GameObject i6Pulley2;

	// Token: 0x04001A9A RID: 6810
	public GameObject i6Pulley3;

	// Token: 0x04001A9B RID: 6811
	public GameObject engineBlockV8;

	// Token: 0x04001A9C RID: 6812
	public GameObject crankV8;

	// Token: 0x04001A9D RID: 6813
	public GameObject altFan;

	// Token: 0x04001A9E RID: 6814
	public GameObject timingGear;

	// Token: 0x04001A9F RID: 6815
	public GameObject v8Fan;

	// Token: 0x04001AA0 RID: 6816
	public GameObject turbineD;

	// Token: 0x04001AA1 RID: 6817
	public GameObject turbineP;

	// Token: 0x04001AA2 RID: 6818
	public bool canRun;

	// Token: 0x04001AA3 RID: 6819
	public bool canCrank;

	// Token: 0x04001AA4 RID: 6820
	public bool canAcc = true;

	// Token: 0x04001AA5 RID: 6821
	public bool canEasyStart;

	// Token: 0x04001AA6 RID: 6822
	public bool canMove;

	// Token: 0x04001AA7 RID: 6823
	public int keyState;

	// Token: 0x04001AA8 RID: 6824
	private GameObject mudemitFL;

	// Token: 0x04001AA9 RID: 6825
	private GameObject mudemitFR;

	// Token: 0x04001AAA RID: 6826
	private GameObject mudemitRR;

	// Token: 0x04001AAB RID: 6827
	private GameObject mudemitRL;

	// Token: 0x04001AAC RID: 6828
	public GameObject mudBrushRL;

	// Token: 0x04001AAD RID: 6829
	public GameObject mudBrushRR;

	// Token: 0x04001AAE RID: 6830
	public GameObject mudBrushFL;

	// Token: 0x04001AAF RID: 6831
	public GameObject mudBrushFR;

	// Token: 0x04001AB0 RID: 6832
	private int interval = 1;

	// Token: 0x04001AB1 RID: 6833
	private float nextTime;

	// Token: 0x04001AB2 RID: 6834
	public float jumptime;

	// Token: 0x04001AB3 RID: 6835
	private int engineCheckInterval = 2;

	// Token: 0x04001AB4 RID: 6836
	private int materialCheckInterval = 2;

	// Token: 0x04001AB5 RID: 6837
	public GameObject heatTrails;

	// Token: 0x04001AB6 RID: 6838
	public GameObject exhaustTrails;

	// Token: 0x04001AB7 RID: 6839
	public GameObject steamTrails;

	// Token: 0x04001AB8 RID: 6840
	public GameObject steamTrails2;

	// Token: 0x04001AB9 RID: 6841
	public GameObject tireExplosion;

	// Token: 0x04001ABA RID: 6842
	public bool steamTrailsOn;

	// Token: 0x04001ABB RID: 6843
	public bool steamTrailsOn2;

	// Token: 0x04001ABC RID: 6844
	public GameObject steamTrailsV8;

	// Token: 0x04001ABD RID: 6845
	public GameObject steamTrails2V8;

	// Token: 0x04001ABE RID: 6846
	public bool steamTrailsOnV8;

	// Token: 0x04001ABF RID: 6847
	public bool steamTrailsOn2V8;

	// Token: 0x04001AC0 RID: 6848
	public GameObject steamTrailsi6;

	// Token: 0x04001AC1 RID: 6849
	public GameObject steamTrails2i6;

	// Token: 0x04001AC2 RID: 6850
	public bool steamTrailsOni6;

	// Token: 0x04001AC3 RID: 6851
	public bool steamTrailsOn2i6;

	// Token: 0x04001AC4 RID: 6852
	public bool heatTrailsOn;

	// Token: 0x04001AC5 RID: 6853
	public bool exhaustTrailsOn;

	// Token: 0x04001AC6 RID: 6854
	public engine enginescript;

	// Token: 0x04001AC7 RID: 6855
	public enginev8 enginescriptv8;

	// Token: 0x04001AC8 RID: 6856
	public enginei6 enginescripti6;

	// Token: 0x04001AC9 RID: 6857
	public GroundDetect groundDetect;

	// Token: 0x04001ACA RID: 6858
	public int deepMud;

	// Token: 0x04001ACB RID: 6859
	public int waterFlood;

	// Token: 0x04001ACC RID: 6860
	public bool jacked;

	// Token: 0x04001ACD RID: 6861
	public TruckBedGrav gravShell;

	// Token: 0x04001ACE RID: 6862
	public Renderer element;

	// Token: 0x04001ACF RID: 6863
	private int randDisengage;

	// Token: 0x04001AD0 RID: 6864
	public Rigidbody rb;

	// Token: 0x04001AD1 RID: 6865
	public bool clutchIn;

	// Token: 0x04001AD2 RID: 6866
	public bool physClutchIn;

	// Token: 0x04001AD3 RID: 6867
	private float clutchPoppedTime = 7f;

	// Token: 0x04001AD4 RID: 6868
	private float clutchTorque;

	// Token: 0x04001AD5 RID: 6869
	private float torqueBonus;

	// Token: 0x04001AD6 RID: 6870
	public bool usingV8;

	// Token: 0x04001AD7 RID: 6871
	public bool usingI6;

	// Token: 0x04001AD8 RID: 6872
	public GameObject RRholder;

	// Token: 0x04001AD9 RID: 6873
	public GameObject RLholder;

	// Token: 0x04001ADA RID: 6874
	public GameObject FRholder;

	// Token: 0x04001ADB RID: 6875
	public GameObject FLholder;

	// Token: 0x04001ADC RID: 6876
	public Transform RRpos;

	// Token: 0x04001ADD RID: 6877
	public Transform RLpos;

	// Token: 0x04001ADE RID: 6878
	public Transform FRpos;

	// Token: 0x04001ADF RID: 6879
	public Transform FLpos;

	// Token: 0x04001AE0 RID: 6880
	public GameObject brackets4;

	// Token: 0x04001AE1 RID: 6881
	public GameObject brackets8;

	// Token: 0x04001AE2 RID: 6882
	public GameObject brackets6;

	// Token: 0x04001AE3 RID: 6883
	public ControlRef cr;

	// Token: 0x04001AE4 RID: 6884
	public bool isDigital;

	// Token: 0x04001AE5 RID: 6885
	public bool fastStart;

	// Token: 0x04001AE6 RID: 6886
	public bool fastStartV8;

	// Token: 0x04001AE7 RID: 6887
	public bool cleanInterior;

	// Token: 0x04001AE8 RID: 6888
	public bool fuelSaving;

	// Token: 0x04001AE9 RID: 6889
	public bool soundMat;

	// Token: 0x04001AEA RID: 6890
	public bool hasSeat;

	// Token: 0x04001AEB RID: 6891
	public bool hasDiffLock;

	// Token: 0x04001AEC RID: 6892
	public bool has5th;

	// Token: 0x04001AED RID: 6893
	public bool hasRollbar;

	// Token: 0x04001AEE RID: 6894
	public GameObject digitalDisplay;

	// Token: 0x04001AEF RID: 6895
	public GameObject speedNeedle;

	// Token: 0x04001AF0 RID: 6896
	public GameObject tachNeedle;

	// Token: 0x04001AF1 RID: 6897
	public GameObject passSeat;

	// Token: 0x04001AF2 RID: 6898
	public GameObject interior;

	// Token: 0x04001AF3 RID: 6899
	public GameObject diffLockButton;

	// Token: 0x04001AF4 RID: 6900
	public GameObject diffLockLed;

	// Token: 0x04001AF5 RID: 6901
	public GameObject diffLock1;

	// Token: 0x04001AF6 RID: 6902
	public GameObject diffLock2;

	// Token: 0x04001AF7 RID: 6903
	public GameObject rollbar;

	// Token: 0x04001AF8 RID: 6904
	public GameObject kcSwitch;

	// Token: 0x04001AF9 RID: 6905
	public GameObject rgbDial;

	// Token: 0x04001AFA RID: 6906
	public float rgbR;

	// Token: 0x04001AFB RID: 6907
	public float rgbB;

	// Token: 0x04001AFC RID: 6908
	public float rgbG;

	// Token: 0x04001AFD RID: 6909
	public Material dbDash;

	// Token: 0x04001AFE RID: 6910
	public UnityEngine.UI.Image tachColor;

	// Token: 0x04001AFF RID: 6911
	public UnityEngine.UI.Image mphColor;

	// Token: 0x04001B00 RID: 6912
	public LensFlare kc1;

	// Token: 0x04001B01 RID: 6913
	public LensFlare kc2;

	// Token: 0x04001B02 RID: 6914
	public LensFlare kc3;

	// Token: 0x04001B03 RID: 6915
	public LensFlare kc4;

	// Token: 0x04001B04 RID: 6916
	public Light kcLight;

	// Token: 0x04001B05 RID: 6917
	public Text rpmText;

	// Token: 0x04001B06 RID: 6918
	public Text mphText;

	// Token: 0x04001B07 RID: 6919
	public Text oilText;

	// Token: 0x04001B08 RID: 6920
	public Text voltText;

	// Token: 0x04001B09 RID: 6921
	public Text tempText;

	// Token: 0x04001B0A RID: 6922
	public Text fuelText;

	// Token: 0x04001B0B RID: 6923
	public Text boostText;

	// Token: 0x04001B0C RID: 6924
	public Slider rpmSlider;

	// Token: 0x04001B0D RID: 6925
	public Slider mphSlider;

	// Token: 0x04001B0E RID: 6926
	public Slider boostSlider;

	// Token: 0x04001B0F RID: 6927
	public bool diffLocked;

	// Token: 0x04001B10 RID: 6928
	public GearBox gb;

	// Token: 0x04001B11 RID: 6929
	public AudioClip[] heavyDoor;

	// Token: 0x04001B12 RID: 6930
	public AudioSource lDoor;

	// Token: 0x04001B13 RID: 6931
	public AudioSource rDoor;

	// Token: 0x04001B14 RID: 6932
	public BoltScriptSusp sbolt1;

	// Token: 0x04001B15 RID: 6933
	public BoltScriptSusp sbolt2;

	// Token: 0x04001B16 RID: 6934
	public BoltScriptSusp sbolt3;

	// Token: 0x04001B17 RID: 6935
	public BoltScriptSusp sbolt4;

	// Token: 0x04001B18 RID: 6936
	private int i;

	// Token: 0x04001B19 RID: 6937
	private Quaternion rot;

	// Token: 0x04001B1A RID: 6938
	private float newblend;

	// Token: 0x04001B1B RID: 6939
	public AudioControl audioControl;

	// Token: 0x04001B1C RID: 6940
	public int deepWater;

	// Token: 0x04001B1D RID: 6941
	public GameObject[] splashFx;

	// Token: 0x04001B1E RID: 6942
	public GameObject[] splashFx2;

	// Token: 0x04001B1F RID: 6943
	public GameObject[] mudFx2;

	// Token: 0x04001B20 RID: 6944
	public ParticleSystem psFL;

	// Token: 0x04001B21 RID: 6945
	public ParticleSystem psFR;

	// Token: 0x04001B22 RID: 6946
	public ParticleSystem psRL;

	// Token: 0x04001B23 RID: 6947
	public ParticleSystem psRR;

	// Token: 0x04001B24 RID: 6948
	public ParticleSystem psFLm;

	// Token: 0x04001B25 RID: 6949
	public ParticleSystem psFRm;

	// Token: 0x04001B26 RID: 6950
	public ParticleSystem psRLm;

	// Token: 0x04001B27 RID: 6951
	public ParticleSystem psRRm;

	// Token: 0x04001B28 RID: 6952
	public TrailerBedGrav flatbed;

	// Token: 0x04001B29 RID: 6953
	private bool hangTimeChallenge;

	// Token: 0x04001B2A RID: 6954
	private float airTime;

	// Token: 0x04001B2B RID: 6955
	public MissionController mc;

	// Token: 0x04001B2C RID: 6956
	public ModWomanJobs mw;

	// Token: 0x04001B2D RID: 6957
	public float additive;

	// Token: 0x04001B2E RID: 6958
	private float additiveBonus;

	// Token: 0x04001B2F RID: 6959
	public GameObject boostGauge;

	// Token: 0x04001B30 RID: 6960
	private Coroutine exitRoutine;

	// Token: 0x04001B31 RID: 6961
	public Light brakeLightR;

	// Token: 0x04001B32 RID: 6962
	public Light brakeLightL;

	// Token: 0x04001B33 RID: 6963
	public Light revLightR;

	// Token: 0x04001B34 RID: 6964
	public Light revLightL;
}
