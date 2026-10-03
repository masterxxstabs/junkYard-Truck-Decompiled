using System;
using System.Collections;
using Steamworks.Data;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x02000131 RID: 305
public class PickUp : MonoBehaviour
{
	// Token: 0x060007EF RID: 2031 RVA: 0x00066F64 File Offset: 0x00065164
	private void Start()
	{
		if (this.painted)
		{
			Material[] materials = base.GetComponent<Renderer>().materials;
			materials[this.paintSlot].color = new UnityEngine.Color(this.red, this.green, this.blue, 1f);
			if (this.metallic > 0f)
			{
				materials[this.paintSlot].SetFloat("_Metallic", this.metallic);
			}
			if (this.smoothness > 0f)
			{
				materials[this.paintSlot].SetFloat("_Glossiness", this.smoothness);
			}
		}
		if (!this.noMat && this.mat != null)
		{
			if (this.mat2 == null)
			{
				Material material = base.GetComponent<MeshRenderer>().material;
				if (this.mat.name.Contains("RustMetal"))
				{
					this.newRust = 2f - 2f * (this.thisDurability / 100f);
					material.SetFloat("_RustIntensity", this.newRust);
				}
			}
			else
			{
				Material[] materials2 = base.GetComponent<MeshRenderer>().materials;
				if (this.mat.name.Contains("RustMetal"))
				{
					this.newRust = 2f - 2f * (this.thisDurability / 100f);
					materials2[0].SetFloat("_RustIntensity", this.newRust);
				}
				if (this.mat2.name.Contains("RustMetal"))
				{
					this.newRust = 2f - 2f * (this.thisDurability / 100f);
					materials2[1].SetFloat("_RustIntensity", this.newRust);
				}
			}
		}
		if (base.name.Substring(0, 3) == "moo")
		{
			base.GetComponent<BoxCollider>().isTrigger = false;
		}
		if (base.transform.position.y < -3000f || base.transform.position.y > 3000f)
		{
			base.transform.position = GameObject.Find("LostBin").transform.position;
		}
		if (base.name.Contains("truckwheel"))
		{
			float num = this.thisDurability;
			if (this.description == "Test Wheel")
			{
				base.name = "truckwheelx";
			}
		}
		if (base.name.Contains("turbo") && this.description == "Unmarked Turbo")
		{
			base.name = "TurboA";
		}
		this.t = base.transform;
		foreach (BoxCollider boxCollider in base.GetComponents<BoxCollider>())
		{
			if (boxCollider.isTrigger && !base.name.Contains("truckwheel"))
			{
				boxCollider.enabled = false;
			}
		}
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x00067243 File Offset: 0x00065443
	public void ForceGrab()
	{
		this.OnMouseDown();
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x0006724C File Offset: 0x0006544C
	private void OnMouseDown()
	{
		BoxCollider[] components = base.GetComponents<BoxCollider>();
		foreach (BoxCollider boxCollider in components)
		{
			if (boxCollider.isTrigger)
			{
				boxCollider.enabled = true;
			}
		}
		if (this.FPS == null)
		{
			this.FPS = GameObject.Find("FPSController").transform;
		}
		if (this.reticleController == null)
		{
			this.reticleController = Object.FindObjectOfType<ReticleController>();
		}
		if (this.theDest == null)
		{
			if (this.shoulderCarry)
			{
				this.theDest = this.FPS.GetChild(0).GetChild(3);
			}
			else
			{
				this.theDest = this.FPS.GetChild(0).GetChild(2);
			}
		}
		if (this.trueMass == 0f)
		{
			this.trueMass = base.GetComponent<Rigidbody>().mass;
		}
		bool flag = false;
		if (this.theDest1 == null)
		{
			this.theDest1 = this.FPS.GetChild(0).GetChild(2);
		}
		foreach (object obj in this.theDest1)
		{
			Transform transform = (Transform)obj;
			if (transform.gameObject.active && transform.gameObject != base.gameObject)
			{
				flag = true;
				base.StartCoroutine(this.NullOther());
			}
		}
		if (this.theDest2 == null)
		{
			this.theDest2 = this.FPS.GetChild(0).GetChild(3);
		}
		foreach (object obj2 in this.theDest2)
		{
			Transform transform2 = (Transform)obj2;
			if (transform2.gameObject.active && transform2.gameObject != base.gameObject)
			{
				flag = true;
				base.StartCoroutine(this.NullOther());
			}
		}
		if (this.holding)
		{
			this.LetGo(0);
			return;
		}
		if (Vector3.Distance(base.transform.position, this.FPS.position) < 2.5f && !flag)
		{
			if (this.pickable)
			{
				this.holding = true;
				if (!this.interactWhileHolding)
				{
					base.gameObject.layer = 16;
				}
				this.isAllowedToTrigger = true;
				if (base.GetComponent<FixedJoint>())
				{
					if (base.gameObject.name == "engineblock")
					{
						base.GetComponent<FixedJoint>().connectedBody = null;
						GameObject gameObject = GameObject.Find("EmptyObjRigidbody");
						gameObject.transform.position = base.gameObject.transform.position;
						base.GetComponent<FixedJoint>().connectedBody = gameObject.GetComponent<Rigidbody>();
					}
					else if (base.gameObject.name == "v8_block")
					{
						base.GetComponent<FixedJoint>().connectedBody = null;
						GameObject gameObject2 = GameObject.Find("EmptyObjRigidbodyV8");
						gameObject2.transform.position = base.gameObject.transform.position;
						base.GetComponent<FixedJoint>().connectedBody = gameObject2.GetComponent<Rigidbody>();
					}
					else if (base.gameObject.name == "250_block")
					{
						base.GetComponent<FixedJoint>().connectedBody = null;
						GameObject gameObject3 = GameObject.Find("EmptyObjRigidbody250");
						gameObject3.transform.position = base.gameObject.transform.position;
						base.GetComponent<FixedJoint>().connectedBody = gameObject3.GetComponent<Rigidbody>();
						base.GetComponent<FixedJoint>().connectedMassScale = 50f;
						base.GetComponent<Engine250>().HideBolts();
					}
					else if (base.gameObject.name == "i6block")
					{
						base.GetComponent<FixedJoint>().connectedBody = null;
						GameObject gameObject4 = GameObject.Find("EmptyObjRigidbodyi6");
						gameObject4.transform.position = base.gameObject.transform.position;
						base.GetComponent<FixedJoint>().connectedBody = gameObject4.GetComponent<Rigidbody>();
					}
					else if (base.gameObject.name != "v8_block" && base.gameObject.name != "engineblock" && base.gameObject.name != "250_block" && base.gameObject.name != "i6block")
					{
						Object.Destroy(base.GetComponent<FixedJoint>());
					}
				}
				if (base.GetComponent<Rigidbody>().mass > 40f)
				{
					if (this.script == null)
					{
						this.script = GameObject.FindWithTag("GameController").GetComponent<Currency>();
					}
					this.script.invokeScale = 10f;
				}
				base.GetComponent<Rigidbody>().mass = this.trueMass;
				base.GetComponent<Rigidbody>().useGravity = false;
				base.GetComponent<Rigidbody>().freezeRotation = true;
				if (this.attachTo != "" || this.attachTo2 != "" || this.attachTo3 != "")
				{
					if (this.attachTo != "paintLoc")
					{
						base.GetComponent<BoxCollider>().enabled = false;
					}
					if (base.gameObject.name == "bucket")
					{
						BoxCollider[] array = components;
						for (int i = 0; i < array.Length; i++)
						{
							array[i].enabled = false;
						}
						foreach (BoxCollider boxCollider2 in base.transform.GetChild(5).GetComponents<BoxCollider>())
						{
							if (boxCollider2.gameObject.GetComponent<Renderer>().enabled)
							{
								boxCollider2.enabled = false;
							}
						}
					}
				}
				base.StartCoroutine(this.PickupDelay());
				base.transform.parent = this.theDest;
				if (this.t == null)
				{
					this.t = base.transform;
				}
				this.t.eulerAngles = new Vector3(this.t.eulerAngles.x, this.fixedRotation, this.t.eulerAngles.z);
				if (base.transform.name.Contains("missionPart") && this.missionObj == null)
				{
					this.missionObj = base.transform.gameObject;
					if (GameObject.FindWithTag("DynamicStart") != null)
					{
						GameObject.FindWithTag("DynamicStart").GetComponent<DynamicTrigger>().PickUpWaypoint();
					}
				}
				if (base.transform.name != "HiJack" && base.transform.name != "Gar_44")
				{
					base.transform.position = this.theDest.position;
					base.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
					base.GetComponent<Rigidbody>().velocity = Vector3.zero;
				}
				if (base.transform.name == "Gar_44")
				{
					Vector3 position = this.theDest.position;
					position.y = base.transform.position.y;
					base.transform.position = position;
				}
				this.colliderName = base.transform.name;
				base.transform.rotation = this.FPS.rotation;
				if (base.transform.name.Contains("log") || base.transform.name.Contains("truckwheel"))
				{
					base.transform.rotation = Quaternion.identity;
				}
				else if (base.transform.name == "transmission" || base.transform.name == "transmission(Clone)")
				{
					base.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 90f));
				}
				else if (base.transform.name == "DirtBike")
				{
					base.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				}
				if (base.transform.name == "engineblock" || base.transform.name == "v8_block" || base.transform.name == "250_block" || base.transform.name == "i6block")
				{
					foreach (Transform transform3 in base.GetComponentsInChildren<Transform>())
					{
						if (transform3.gameObject.name != "bolt" && transform3.gameObject.name != "v8_block" && transform3.gameObject.name != "engineblock" && transform3.gameObject.name != "250_block" && transform3.gameObject.name != "i6block")
						{
							transform3.gameObject.layer = 9;
						}
					}
				}
				this.FPS.GetComponent<Interactor>().dropInterval = Time.time + 0.5f;
				this.m_HoldJoint = this.theDest.gameObject.AddComponent<FixedJoint>();
				this.m_HoldJoint.breakForce = 80000f;
				this.m_HoldJoint.breakTorque = 80000f;
				this.m_HoldJoint.connectedBody = base.GetComponent<Rigidbody>();
				this.FPS.GetComponent<Interactor>().holdJoint = this.m_HoldJoint;
				return;
			}
			this.script = GameObject.FindWithTag("GameController").GetComponent<Currency>();
			if (this.price > 0f && !this.pickable && this.script.money >= this.price)
			{
				this.FPS.gameObject.GetComponent<Interactor>().inv.SubtractMoney(this.price);
				this.script.money -= this.price;
				this.script.money = Mathf.Round(this.script.money * 100f) / 100f;
				this.pickable = true;
				this.price = 0f;
				if (base.transform.name == "DirtBike")
				{
					base.gameObject.GetComponent<InteractiveObject>().description = "";
				}
				if (base.transform.name == "WheelJ")
				{
					GameObject.Find("jake7").GetComponent<Jake>().BuyTire();
				}
			}
		}
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x00067D24 File Offset: 0x00065F24
	private IEnumerator PickupDelay()
	{
		yield return new WaitForSeconds(0.3f);
		this.FPS.GetComponent<Interactor>().pickedUpObject = base.gameObject;
		yield break;
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x00067D34 File Offset: 0x00065F34
	public void LetGo(int throwing)
	{
		if (base.gameObject.activeSelf)
		{
			if (throwing == 1)
			{
				int num = 800;
				if (base.gameObject.name.Contains("trashbag"))
				{
					num = 3000;
				}
				base.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * (float)num);
			}
			if (this.FPS == null)
			{
				this.FPS = GameObject.Find("FPSController").transform;
			}
			this.FPS.GetComponent<Interactor>().pickedUpObject = null;
			Object.Destroy(this.m_HoldJoint);
			this.holding = false;
			base.gameObject.layer = 0;
			this.inTrigger = false;
			this.isAllowedToTrigger = false;
			base.GetComponent<Rigidbody>().freezeRotation = false;
			base.GetComponent<BoxCollider>().enabled = true;
			if (base.gameObject.name == "bucket")
			{
				BoxCollider[] components = base.GetComponents<BoxCollider>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].enabled = true;
				}
				foreach (BoxCollider boxCollider in base.transform.GetChild(5).GetComponents<BoxCollider>())
				{
					if (boxCollider.gameObject.GetComponent<Renderer>().enabled)
					{
						boxCollider.enabled = true;
					}
				}
			}
			if (base.transform.name == "engineblock" || base.transform.name == "v8_block" || base.transform.name == "250_block" || base.transform.name == "i6block")
			{
				foreach (Transform transform in base.GetComponentsInChildren<Transform>())
				{
					if (transform.gameObject.name != "bolt")
					{
						transform.gameObject.layer = 0;
					}
				}
			}
			if (base.gameObject.name == "FuelSage")
			{
				base.GetComponent<FluidHandler>().ReturnNozzle();
			}
			if (this.canSnap)
			{
				this.reticleController.ShowSnapIcon(false);
				if (base.gameObject.name.Substring(0, 3) == "HiJ")
				{
					base.transform.parent = null;
					if (this.truck == null)
					{
						this.truck = GameObject.Find("dirt pickup truck");
					}
					if (this.validTrigName == "HiJack_e")
					{
						this.vt = GameObject.Find(this.validTrigName);
						this.vt.GetComponent<Renderer>().enabled = true;
						this.vt.GetComponent<BoxCollider>().enabled = true;
						foreach (object obj in this.vt.transform)
						{
							((Transform)obj).GetComponent<Renderer>().enabled = true;
						}
						Object.Destroy(base.gameObject);
					}
					else if (this.validTrigName.Substring(0, 9) == "jackmount" && !this.HiJackX.transform.GetChild(0).gameObject.activeSelf)
					{
						this.HiJackX.transform.GetChild(0).gameObject.SetActive(true);
						Object.Destroy(base.gameObject);
					}
				}
				else if (base.gameObject.name == "bucket")
				{
					if (this.validTrigName == "bucket_e")
					{
						base.gameObject.transform.GetChild(0).GetComponent<Bucket>().FillKeg();
					}
					if (this.validTrigName == "bucket2_e")
					{
						base.gameObject.transform.GetChild(0).GetComponent<Bucket>().DumpBucket();
					}
				}
				else if (this.validTrigName == "revengecarinlet")
				{
					GameObject.Find("tracker42b").GetComponent<showmission>().ActivateNextWaypoint();
					BoxCollider[] components2 = this.validTrigObject.GetComponents<BoxCollider>();
					components2[0].enabled = true;
					components2[1].enabled = true;
					Object.Destroy(base.gameObject);
				}
				else if (this.validTrigName.Contains("tobaccocrate"))
				{
					base.gameObject.GetComponent<Rigidbody>().useGravity = true;
					if (this.validTrigObject.GetComponent<PickUp>().thisDurability < 6f)
					{
						Object.Destroy(base.gameObject);
						this.validTrigObject.GetComponent<PickUp>().thisDurability += 1f;
					}
					this.validTrigObject.transform.GetChild(1).gameObject.SetActive(true);
					if (this.validTrigObject.GetComponent<PickUp>().thisDurability > 2f)
					{
						this.validTrigObject.transform.GetChild(0).gameObject.SetActive(true);
					}
					if (this.validTrigObject.GetComponent<PickUp>().thisDurability == 6f)
					{
						this.validTrigObject.GetComponents<BoxCollider>()[1].enabled = false;
						this.validTrigObject.transform.GetChild(2).gameObject.SetActive(true);
					}
				}
				else if (this.validTrigName == "batterycharger_e")
				{
					GameObject gameObject = this.validTrigObject.transform.parent.gameObject;
					if (!gameObject.GetComponent<BatteryCharger>().charging)
					{
						gameObject.GetComponent<BatteryCharger>().ChargeBattery();
						gameObject.GetComponent<BatteryCharger>().batterydura.health = this.thisDurability;
						Object.Destroy(base.gameObject);
					}
					else
					{
						this.NullParent();
					}
				}
				else if (this.validTrigName == "batterycharger18_e")
				{
					GameObject gameObject2 = this.validTrigObject.transform.parent.gameObject;
					if (!gameObject2.GetComponent<BatteryCharger>().charging)
					{
						gameObject2.GetComponent<BatteryCharger>().ChargeBattery18();
						gameObject2.GetComponent<BatteryCharger>().batterydura18.health = this.thisDurability;
						Object.Destroy(base.gameObject);
					}
					else
					{
						this.NullParent();
					}
				}
				else if (this.validTrigName == "batterycart_e")
				{
					GameObject gameObject3 = GameObject.Find("golfcartfinal");
					gameObject3.GetComponent<golfcart>().batterydura.health = this.thisDurability;
					gameObject3.GetComponent<golfcart>().AddBattery();
					Object.Destroy(base.gameObject);
				}
				else if (this.validTrigName == "batterycart18_e")
				{
					GameObject gameObject4 = GameObject.Find("golfcartfinal");
					gameObject4.GetComponent<golfcart>().batterydura18.health = this.thisDurability;
					gameObject4.GetComponent<golfcart>().AddBattery18();
					Object.Destroy(base.gameObject);
				}
				else if (base.gameObject.transform.tag == "Wheel")
				{
					if (this.validTrigName == "BrakeDisk" && this.validTrigObject.name == "BrakeDisk")
					{
						this.validParentName = this.validTrigObject.transform.parent.name;
						bool flag = false;
						if (this.validParentName == "SteeringJointFL2")
						{
							this.tireX = this.validTrigObject.transform.GetChild(0).gameObject;
							this.wheelColX = GameObject.Find("wheelColliderFL");
						}
						else if (this.validParentName == "SteeringJointFR2")
						{
							this.tireX = this.validTrigObject.transform.GetChild(0).gameObject;
							this.wheelColX = GameObject.Find("wheelColliderFR");
						}
						else if (this.validParentName == "AxleRearLeft")
						{
							this.tireX = this.validTrigObject.transform.GetChild(0).gameObject;
							this.wheelColX = GameObject.Find("wheelColliderRL");
						}
						else if (this.validParentName == "AxleRearRight")
						{
							this.tireX = this.validTrigObject.transform.GetChild(0).gameObject;
							this.wheelColX = GameObject.Find("wheelColliderRR");
						}
						else if (this.validParentName == "sparemount_e" || this.validParentName == "sparemountF_e")
						{
							this.tireX = this.validTrigObject.transform.GetChild(0).gameObject;
							flag = true;
						}
						else if (this.validParentName == "SteeringJointFL2F")
						{
							this.tireX = this.validTrigObject.transform.GetChild(0).gameObject;
							this.wheelColX = GameObject.Find("wheelColliderFLF");
						}
						else if (this.validParentName == "SteeringJointFR2F")
						{
							this.tireX = this.validTrigObject.transform.GetChild(0).gameObject;
							this.wheelColX = GameObject.Find("wheelColliderFRF");
						}
						else if (this.validParentName == "AxleRearLeftF")
						{
							this.tireX = this.validTrigObject.transform.GetChild(0).gameObject;
							this.wheelColX = GameObject.Find("wheelColliderRLF");
						}
						else if (this.validParentName == "AxleRearRightF")
						{
							this.tireX = this.validTrigObject.transform.GetChild(0).gameObject;
							this.wheelColX = GameObject.Find("wheelColliderRRF");
						}
						this.i = 0;
						GameObject gameObject5 = this.tireX;
						this.rimNum = 0;
						this.tireNum = 0;
						using (IEnumerator enumerator = base.gameObject.transform.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								if (((Transform)enumerator.Current).gameObject.active)
								{
									if (this.i < 6 && this.i > 0)
									{
										this.rimNum = this.i;
									}
									if (this.i > 5)
									{
										this.tireNum = this.i;
									}
								}
								this.i++;
							}
						}
						if (this.rimNum == 0)
						{
							this.NullParent();
						}
						if (this.rimNum > 0)
						{
							this.validTrigObject.GetComponent<BoxCollider>().enabled = true;
							if (!flag)
							{
								this.validTrigObject.transform.GetChild(1).gameObject.GetComponent<BoxCollider>().enabled = true;
								this.validTrigObject.transform.GetChild(1).gameObject.GetComponent<Renderer>().enabled = true;
								this.validTrigObject.transform.GetChild(2).gameObject.GetComponent<BoxCollider>().enabled = true;
								this.validTrigObject.transform.GetChild(2).gameObject.GetComponent<Renderer>().enabled = true;
								this.validTrigObject.transform.GetChild(3).gameObject.GetComponent<BoxCollider>().enabled = true;
								this.validTrigObject.transform.GetChild(3).gameObject.GetComponent<Renderer>().enabled = true;
								this.validTrigObject.transform.GetChild(4).gameObject.GetComponent<BoxCollider>().enabled = true;
								this.validTrigObject.transform.GetChild(4).gameObject.GetComponent<Renderer>().enabled = true;
							}
							if (flag)
							{
								BoxCollider[] components3 = this.validTrigObject.GetComponents<BoxCollider>();
								if (!components3[0].isTrigger)
								{
									components3[0].enabled = true;
								}
								if (!components3[1].isTrigger)
								{
									components3[1].enabled = true;
								}
							}
							gameObject5.transform.GetChild(this.tireNum).gameObject.SetActive(true);
							gameObject5.transform.GetChild(this.rimNum).gameObject.SetActive(true);
							this.validTrigObject.GetComponent<BoxCollider>().enabled = true;
							this.validTrigObject.GetComponent<durability>().health = this.thisDurability;
							foreach (object obj2 in this.validTrigObject.transform)
							{
								Transform transform2 = (Transform)obj2;
								if (transform2.name == "bolt")
								{
									transform2.GetComponent<Renderer>().enabled = true;
									transform2.GetComponent<BoxCollider>().enabled = true;
								}
							}
							this.deflation = base.GetComponent<PickUp>().deflation;
							float num2 = 1f - this.deflation * 0.01f;
							this.newRadius = num2 * 0.110000014f + 0.29f;
							if (this.tireNum == 0)
							{
								this.newRadius = 0.23f;
								this.validTrigObject.GetComponent<durability>().health = 0f;
							}
							if (!flag)
							{
								this.wheelColX.GetComponent<WheelCollider>().radius = this.newRadius;
							}
							this.FPS.GetComponent<Interactor>().CalculateTraction();
							if (base.gameObject.name == "truckwheelx")
							{
								this.FPS.GetComponent<Interactor>().modGirl.GetComponent<ModWomanJobs>().tireSlot = this.validParentName;
							}
							base.StartCoroutine(this.DestroyWheel(base.gameObject));
							base.transform.parent = null;
						}
					}
					else
					{
						this.NullParent();
					}
					if (this.validTrigName.Contains("truckwheel"))
					{
						this.vt = this.validTrigObject;
						if (base.transform.GetChild(0).gameObject.active && !this.vt.transform.GetChild(0).gameObject.active && (this.vt.transform.GetChild(1).gameObject.active || this.vt.transform.GetChild(2).gameObject.active || this.vt.transform.GetChild(3).gameObject.active || this.vt.transform.GetChild(4).gameObject.active || this.vt.transform.GetChild(5).gameObject.active))
						{
							if (this.tireNum == 0)
							{
								this.i = 6;
								while (this.i < 20)
								{
									if (base.transform.GetChild(this.i).gameObject.active)
									{
										this.tireNum = this.i;
										break;
									}
									this.i++;
								}
							}
							this.i = 6;
							while (this.i < 20)
							{
								if (this.vt.transform.GetChild(this.i).gameObject.active)
								{
									this.canSnap = false;
									break;
								}
								this.i++;
							}
							if (this.canSnap)
							{
								this.vt.GetComponent<PickUp>().thisDurability = this.thisDurability;
								this.vt.transform.GetChild(this.tireNum).gameObject.SetActive(true);
								MeshCollider[] components4 = this.vt.GetComponents<MeshCollider>();
								components4[0].enabled = false;
								components4[1].enabled = true;
								this.vt.transform.gameObject.GetComponent<TireAssign>().tireNumX = this.tireNum;
								base.StartCoroutine(this.DestroyWheel(base.gameObject));
							}
						}
					}
					if (this.validTrigName.Contains("truckwheel"))
					{
						this.vt = this.validTrigObject;
						if (!base.transform.GetChild(0).gameObject.active && this.vt.transform.GetChild(0).gameObject.active)
						{
							bool flag2 = false;
							this.i = 6;
							while (this.i < 20)
							{
								if (base.transform.GetChild(this.i).gameObject.active)
								{
									flag2 = true;
								}
								this.i++;
							}
							this.i = 0;
							while (this.i < 6)
							{
								if (base.transform.GetChild(this.i).gameObject.active)
								{
									this.tireNum = this.i;
									break;
								}
								this.i++;
							}
							if (!flag2)
							{
								this.vt.transform.GetChild(this.tireNum).gameObject.SetActive(true);
								this.vt.transform.GetChild(0).gameObject.SetActive(false);
								MeshCollider[] components5 = this.vt.GetComponents<MeshCollider>();
								components5[0].enabled = false;
								components5[1].enabled = true;
								this.vt.transform.gameObject.GetComponent<TireAssign>().rimNumX = this.tireNum;
								base.StartCoroutine(this.DestroyWheel(base.gameObject));
							}
						}
					}
				}
				else if (base.gameObject.name == "engineblock" || base.gameObject.name == "v8_block" || base.gameObject.name == "250_block" || base.gameObject.name == "i6block")
				{
					this.vt = this.validTrigObject;
					base.gameObject.transform.parent = null;
					base.gameObject.transform.position = this.vt.transform.position;
					base.gameObject.transform.rotation = this.vt.transform.rotation;
					if (this.vt.name == "250EngMount")
					{
						GameObject gameObject6 = GameObject.Find("DirtBike");
						Rigidbody component = gameObject6.GetComponent<Rigidbody>();
						base.gameObject.GetComponent<FixedJoint>().connectedBody = component;
						base.gameObject.transform.parent = gameObject6.transform;
						base.gameObject.GetComponent<Engine250>().ShowBolts();
						base.gameObject.GetComponent<FixedJoint>().connectedMassScale = 0f;
					}
					if (this.vt.name == "ebtemplate" || this.vt.name == "ebtemplatev8" || this.vt.name == "ebtemplatei6")
					{
						base.gameObject.GetComponent<FixedJoint>().connectedBody = GameObject.Find("DK_9").GetComponent<Rigidbody>();
						this.pickable = false;
					}
					else if (this.vt.name == "ebtemplate2" || this.vt.name == "ebtemplate2v8" || this.vt.name == "ebtemplatei62")
					{
						if (this.truck == null)
						{
							this.truck = GameObject.Find("dirt pickup truck");
						}
						if (base.gameObject.name == "engineblock")
						{
							this.truck.GetComponent<car>().usingV8 = false;
							this.truck.GetComponent<car>().usingI6 = false;
							this.truck.GetComponent<car>().v4Brackets();
						}
						if (base.gameObject.name == "v8_block")
						{
							this.truck.GetComponent<car>().usingV8 = true;
							this.truck.GetComponent<car>().usingI6 = false;
							this.truck.GetComponent<car>().v8Brackets();
							base.gameObject.GetComponent<enginev8>().whichTruck = 1;
						}
						if (base.gameObject.name == "i6block")
						{
							this.truck.GetComponent<car>().usingV8 = false;
							this.truck.GetComponent<car>().usingI6 = true;
							this.truck.GetComponent<car>().i6Brackets();
							base.gameObject.GetComponent<enginei6>().whichTruck = 1;
						}
						base.gameObject.GetComponent<FixedJoint>().connectedBody = this.truck.GetComponent<Rigidbody>();
						base.gameObject.transform.parent = this.truck.transform;
						if (base.gameObject.name == "v8_block")
						{
							Achievement achievement = new Achievement("ACH_V8");
							achievement.Trigger(true);
						}
					}
					else if (this.vt.name == "ebtemplate3v8")
					{
						if (this.truck2 == null)
						{
							this.truck2 = GameObject.Find("f1003");
						}
						if (base.gameObject.name == "v8_block")
						{
							this.truck2.GetComponent<car4>().usingV8 = true;
							this.truck2.GetComponent<car4>().v8Brackets();
						}
						base.gameObject.GetComponent<enginev8>().whichTruck = 2;
						base.gameObject.GetComponent<FixedJoint>().connectedBody = this.truck2.GetComponent<Rigidbody>();
						base.gameObject.transform.parent = this.truck2.transform;
					}
					else if (this.vt.name == "ebtemplatei6C")
					{
						if (this.amc == null)
						{
							this.amc = GameObject.Find("amc");
						}
						if (base.gameObject.name == "i6block")
						{
							this.amc.GetComponent<car3>().usingI6 = true;
						}
						base.gameObject.GetComponent<enginei6>().whichTruck = 2;
						base.gameObject.GetComponent<FixedJoint>().connectedBody = this.amc.GetComponent<Rigidbody>();
						base.gameObject.transform.parent = this.amc.transform;
					}
				}
				else
				{
					if (this.validTrigName == "oldengine_e")
					{
						GameObject.Find("amc").GetComponent<car3>().RestoreAcc();
					}
					bool flag3 = false;
					if (this.enblock == null)
					{
						this.enblock = GameObject.Find("engineblock");
					}
					if (this.enblockv8 == null)
					{
						this.enblockv8 = GameObject.Find("v8_block");
					}
					if (this.enblock250 == null)
					{
						this.enblock250 = GameObject.Find("250_block");
					}
					if (this.enblocki6 == null)
					{
						this.enblocki6 = GameObject.Find("i6block");
					}
					if (base.gameObject.name.Length > 5)
					{
						if (base.gameObject.name.Contains("TurboA"))
						{
							this.enblock.GetComponent<engine>().nonOemTurbo = true;
						}
						else if (base.gameObject.name.Contains("v8_am_exhaustD"))
						{
							if (this.enblockv8.GetComponent<enginev8>().exhaustManD.GetComponent<Renderer>().enabled)
							{
								flag3 = true;
							}
						}
						else if (base.gameObject.name.Contains("v8_am_exhaustP"))
						{
							if (this.enblockv8.GetComponent<enginev8>().exhaustManP.GetComponent<Renderer>().enabled)
							{
								flag3 = true;
							}
						}
						else if (base.gameObject.name.Contains("v8_am_intake"))
						{
							if (this.enblockv8.GetComponent<enginev8>().intakeMan.GetComponent<Renderer>().enabled)
							{
								flag3 = true;
							}
						}
						else if (base.gameObject.name.Contains("v8_exhaustmanD"))
						{
							if (this.enblockv8.GetComponent<enginev8>().am_headerD.GetComponent<Renderer>().enabled)
							{
								flag3 = true;
							}
						}
						else if (base.gameObject.name.Contains("v8_exhaustmanP"))
						{
							if (this.enblockv8.GetComponent<enginev8>().am_headerP.GetComponent<Renderer>().enabled)
							{
								flag3 = true;
							}
						}
						else if (base.gameObject.name.Contains("v8_intakeman"))
						{
							if (this.enblockv8.GetComponent<enginev8>().am_intake.GetComponent<Renderer>().enabled)
							{
								flag3 = true;
							}
						}
						else if (base.gameObject.name.Contains("compound2"))
						{
							if (this.enblock.GetComponent<engine>().airFilterTurbo.GetComponent<Renderer>().enabled)
							{
								flag3 = true;
							}
						}
						else if (base.gameObject.name.Substring(0, 6) == "header")
						{
							if (this.enblock.GetComponent<engine>().exhaustMan.GetComponent<Renderer>().enabled)
							{
								flag3 = true;
							}
							else if (this.enblock.GetComponent<engine>().exhaustMan2.GetComponent<Renderer>().enabled)
							{
								flag3 = true;
							}
							else if (this.enblock.GetComponent<engine>().exhaustMan3.GetComponent<Renderer>().enabled)
							{
								flag3 = true;
							}
						}
						else if (base.gameObject.name.Substring(0, 6) == "conefi")
						{
							if (this.enblock.GetComponent<engine>().turboPipe.GetComponent<Renderer>().enabled && this.validTrigObject.transform.parent.name == "throttlebody_e")
							{
								flag3 = true;
							}
							if (this.enblock.GetComponent<engine>().compound2.GetComponent<Renderer>().enabled && this.validTrigObject.transform.parent.name == "turbo_e")
							{
								flag3 = true;
							}
						}
						else if (base.gameObject.name.Substring(0, 6) == "ramair")
						{
							if (this.enblock.GetComponent<engine>().airFilterEFI.GetComponent<Renderer>().enabled)
							{
								flag3 = true;
							}
						}
						else if (base.gameObject.name.Contains("intake") && !base.gameObject.name.Contains("i6"))
						{
							if (this.enblock.GetComponent<engine>().intakeMan.GetComponent<Renderer>().enabled)
							{
								flag3 = true;
							}
							else if (this.enblock.GetComponent<engine>().intakeManEFI.GetComponent<Renderer>().enabled)
							{
								flag3 = true;
							}
						}
						else if (base.gameObject.name.Contains("diffri") && (this.validTrigObject.transform.parent.name == "AxleRearMiddle" || this.validTrigObject.transform.parent.name == "AxleFrontMiddle"))
						{
							if (this.attachTo == "diffringgearF4_e" || this.attachTo == "diffringgearF5_e")
							{
								if (this.validTrigObject.transform.parent.GetChild(0).gameObject.GetComponent<Renderer>().enabled)
								{
									flag3 = true;
								}
								if (this.validTrigObject.transform.parent.GetChild(1).gameObject.GetComponent<Renderer>().enabled)
								{
									flag3 = true;
								}
							}
							else if (this.attachTo == "diffringgearR4_e" || this.attachTo == "diffringgearR5_e")
							{
								if (this.validTrigObject.transform.parent.GetChild(0).gameObject.GetComponent<Renderer>().enabled)
								{
									flag3 = true;
								}
								if (this.validTrigObject.transform.parent.GetChild(1).gameObject.GetComponent<Renderer>().enabled)
								{
									flag3 = true;
								}
							}
						}
						if (flag3)
						{
							this.NullParent();
						}
					}
					if (!flag3)
					{
						if (this.truck == null)
						{
							this.truck = GameObject.Find("dirt pickup truck");
						}
						if (this.truck2 == null)
						{
							this.truck2 = GameObject.Find("f1003");
						}
						if (base.gameObject.name.Contains("winch2") || base.gameObject.name.Contains("winch4"))
						{
							if (this.validTrigObject.transform.parent.name == "f1003")
							{
								GameObject.Find("hookwF").GetComponent<BoxCollider>().enabled = true;
							}
							else
							{
								GameObject.Find("hookw").GetComponent<BoxCollider>().enabled = true;
							}
							if (base.gameObject.name.Contains("winch4"))
							{
								this.validTrigObject.GetComponent<WinchType>().winchType = 2;
								this.validTrigObject.GetComponent<WinchType>().Start();
							}
							else if (base.gameObject.name.Contains("winch2"))
							{
								this.validTrigObject.GetComponent<WinchType>().winchType = 1;
								this.validTrigObject.GetComponent<WinchType>().Start();
							}
						}
						if (base.gameObject.name.Contains("v8_am_intake"))
						{
							this.tempchild = GameObject.Find("v8_distributor_e");
							BoxCollider[] components6 = this.tempchild.GetComponents<BoxCollider>();
							if (components6[0].isTrigger)
							{
								components6[0].enabled = true;
							}
							if (components6[1] != null && components6[1].isTrigger)
							{
								components6[1].enabled = true;
							}
						}
						if (base.gameObject.name.Contains("spraypaint"))
						{
							this.canSnap = false;
							this.LetGo(0);
							base.gameObject.transform.position = this.validTrigObject.transform.position;
							base.gameObject.transform.rotation = this.validTrigObject.transform.rotation;
							this.pickable = false;
							return;
						}
						if (base.gameObject.name.Contains("fruit"))
						{
							this.validTrigObject.transform.parent.GetChild(1).gameObject.GetComponent<Renderer>().enabled = false;
							this.validTrigObject.transform.parent.GetChild(1).gameObject.GetComponent<durability>().health = 0f;
							this.validTrigObject.transform.parent.GetChild(1).gameObject.GetComponents<BoxCollider>()[0].enabled = false;
							this.validTrigObject.transform.parent.GetChild(2).gameObject.GetComponent<Renderer>().enabled = false;
							this.validTrigObject.transform.parent.GetChild(2).gameObject.GetComponent<durability>().health = 0f;
							this.validTrigObject.transform.parent.GetChild(2).gameObject.GetComponents<BoxCollider>()[0].enabled = false;
							this.validTrigObject.transform.parent.GetChild(3).gameObject.GetComponent<Renderer>().enabled = false;
							this.validTrigObject.transform.parent.GetChild(3).gameObject.GetComponent<durability>().health = 0f;
							this.validTrigObject.transform.parent.GetChild(4).gameObject.GetComponents<BoxCollider>()[0].enabled = false;
							this.validTrigObject.transform.parent.GetChild(4).gameObject.GetComponent<Renderer>().enabled = false;
							this.validTrigObject.transform.parent.GetChild(4).gameObject.GetComponent<durability>().health = 0f;
							this.validTrigObject.transform.parent.GetChild(4).gameObject.GetComponents<BoxCollider>()[0].enabled = false;
						}
						this.vt = this.validTrigObject;
						if (this.vt.GetComponent<Renderer>() != null)
						{
							this.vt.GetComponent<Renderer>().enabled = true;
							if (this.painted)
							{
								this.vt.GetComponent<Renderer>().materials[this.paintSlot].color = new UnityEngine.Color(this.red, this.green, this.blue, 1f);
								durability component2 = this.vt.GetComponent<durability>();
								component2.painted = true;
								component2.red = this.red;
								component2.green = this.green;
								component2.blue = this.blue;
								component2.metallic = this.metallic;
								component2.smoothness = this.smoothness;
							}
						}
						BoxCollider[] components7 = this.validTrigObject.GetComponents<BoxCollider>();
						components7[0].enabled = true;
						components7[1].enabled = true;
						if (this.vt.GetComponent<durability>() != null)
						{
							this.vt.GetComponent<durability>().canDetach = true;
							this.vt.GetComponent<durability>().health = this.thisDurability;
						}
						this.newRust = 2f - 2f * (this.thisDurability / 100f);
						if (this.vt.GetComponent<Renderer>() != null)
						{
							Material[] materials = this.vt.GetComponent<MeshRenderer>().materials;
							materials[0].SetFloat("_RustIntensity", this.newRust);
							if (materials.Length > 1)
							{
								materials[1].SetFloat("_RustIntensity", this.newRust);
							}
						}
						if (this.truck == null)
						{
							this.truck = GameObject.Find("dirt pickup truck");
						}
						if (this.truck != null)
						{
							this.truck.GetComponent<AudioSource>().Play();
						}
						foreach (object obj3 in this.vt.transform)
						{
							Transform transform3 = (Transform)obj3;
							if (transform3.name != "rocker" && transform3.name != "distwire" && transform3.name != "laminarOil" && transform3.name != "laminarOilV8" && transform3.name != "hookw" && transform3.name != "hookwF" && transform3.name != "OilDrain" && transform3.name != "Primary Drive Gear" && transform3.name != "WHEEL_HOLDER" && transform3.name != "laminarCoolant" && transform3.name != "radhose_therm" && transform3.name != "origin" && transform3.name != "turbine" && transform3.name != "altfan" && transform3.name != "OilDrainV8" && transform3.name != "250_gear2" && transform3.name != "250_cam" && transform3.name != "250_kickstarter" && transform3.name != "Plane001.003" && transform3.name != "jerrycaninlet" && transform3.name != "blackberries" && transform3.name != "oranges" && transform3.name != "limes" && transform3.name != "ambrosia" && transform3.name != "universalpulley" && transform3.name != "OilDrainI6" && transform3.name != "brokenTurbo" && transform3.name != "turbotach" && transform3.name != "buddyscreen" && transform3.tag != "ignoreattach")
							{
								BoxCollider[] components8 = transform3.GetComponents<BoxCollider>();
								if (components8.Length > 1)
								{
									if (components8[0].isTrigger)
									{
										components8[0].enabled = true;
									}
									if (components8[1] != null && components8[1].isTrigger)
									{
										components8[1].enabled = true;
									}
								}
								else
								{
									transform3.GetComponent<BoxCollider>().enabled = true;
								}
							}
							if (transform3.name == "OilDrain")
							{
								this.FPS.GetComponent<Interactor>().oilBolt.SetActive(true);
							}
							if (transform3.name == "OilDrainV8")
							{
								this.FPS.GetComponent<Interactor>().oilBoltV8.SetActive(true);
							}
							if (transform3.name == "OilDrainI6")
							{
								this.FPS.GetComponent<Interactor>().oilBolti6.SetActive(true);
							}
							if (transform3.name == "CoolantDrain")
							{
								this.coolantBolt.GetComponent<Renderer>().enabled = true;
							}
							if (transform3.name == "buddyscreen")
							{
								transform3.GetComponent<Renderer>().enabled = true;
								this.vt.gameObject.GetComponent<TractionBuddy>().enabled = true;
								this.vt.gameObject.GetComponent<AudioSource>().enabled = true;
							}
							if (transform3.name == "ww_internals")
							{
								transform3.gameObject.SetActive(true);
								this.vt.GetComponent<Waterwheel>().enabled = true;
								this.vt.GetComponent<Waterwheel>().canRotate = true;
							}
							if (transform3.name == "bolt" || transform3.name == "rocker" || transform3.name == "hookw" || transform3.name == "hookwF" || transform3.name == "oilcap" || transform3.name == "oilinput" || transform3.name == "Primary Drive Gear" || transform3.name == "distwire" || transform3.name == "RadiatorCap" || transform3.name == "radhose_therm" || transform3.name == "oilinputv8" || transform3.name == "altfan" || transform3.name == "turbine" || transform3.name == "250_gear2" || transform3.name == "250_cam" || transform3.name == "250_kickstarter" || transform3.name == "250_oilcap" || transform3.name == "250_transcap" || transform3.name == "universalpulley")
							{
								if (transform3.name != "oilinput" && transform3.name != "oilinputv8" && transform3.name != "250oilinput" && transform3.name != "250transinput")
								{
									transform3.GetComponent<Renderer>().enabled = true;
								}
								if (transform3.name == "bolt" || transform3.name == "oilcap" || transform3.name == "oilinput" || transform3.name == "oilinputv8" || transform3.name == "250oilinput" || transform3.name == "250transinput")
								{
									transform3.GetComponent<BoxCollider>().enabled = true;
								}
							}
						}
						if (this.enblock == null)
						{
							this.enblock = GameObject.Find("engineblock");
						}
						if (this.enblockv8 == null)
						{
							this.enblockv8 = GameObject.Find("v8_block");
						}
						if (this.enblock250 == null)
						{
							this.enblock250 = GameObject.Find("250_block");
						}
						if (this.enblocki6 == null)
						{
							this.enblocki6 = GameObject.Find("i6block");
						}
						if (this.partType == 0 && this.enblock != null)
						{
							this.enblock.GetComponent<engine>().addPart(this.validTrigName, true);
						}
						if (this.partType == 1)
						{
							this.enblockv8.GetComponent<enginev8>().addPart(this.validTrigName, true);
						}
						if (this.partType == 3)
						{
							this.enblock250.GetComponent<Engine250>().addPart(this.validTrigName, true);
						}
						if (this.partType == 4)
						{
							this.enblocki6.GetComponent<enginei6>().addPart(this.validTrigName, true);
						}
						if (this.vt.name == "v8_fanbelt_e")
						{
							this.enblockv8.GetComponent<enginev8>().addPart(this.validTrigName, true);
						}
						base.transform.parent = null;
						if (this.vt.name == "infuser_e")
						{
							this.vt.transform.GetChild(0).gameObject.SetActive(false);
						}
						if (this.vt.name == "turbogaugeD_e")
						{
							this.vt.transform.GetChild(0).gameObject.SetActive(true);
						}
						if (this.vt.name == "radarD_e" || this.vt.name == "radarF_e" || this.vt.name == "radarE_e")
						{
							this.vt.GetComponent<RadarDetector>().enabled = true;
						}
						if (this.vt.name == "cbradio_e" || this.vt.name == "cbradioF_e")
						{
							this.vt.GetComponent<CbRadio>().enabled = true;
						}
						if (this.vt.name.Contains("jerrymounted"))
						{
							base.gameObject.SetActive(false);
						}
						else
						{
							Object.Destroy(base.gameObject);
						}
					}
				}
			}
			else
			{
				this.NullParent();
			}
			this.canSnap = false;
		}
		if (this.script != null)
		{
			this.script.invokeScale = 1f;
		}
		foreach (BoxCollider boxCollider2 in base.GetComponents<BoxCollider>())
		{
			if (boxCollider2.isTrigger && !base.name.Contains("truckwheel"))
			{
				boxCollider2.enabled = false;
			}
		}
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x0006A8C0 File Offset: 0x00068AC0
	private void NullParent()
	{
		base.transform.parent = null;
		if (base.gameObject.name != "FuelSage")
		{
			base.GetComponent<Rigidbody>().useGravity = true;
			base.GetComponent<Rigidbody>().isKinematic = false;
		}
		if (this.reticleController != null)
		{
			this.reticleController.ShowSnapIcon(false);
		}
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x0006A922 File Offset: 0x00068B22
	private IEnumerator NullOther()
	{
		yield return new WaitForSeconds(0.5f);
		using (IEnumerator enumerator = this.theDest.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform transform = (Transform)obj;
				if (transform.gameObject.active && transform.gameObject.name != base.gameObject.name)
				{
					transform.gameObject.transform.parent = null;
					transform.gameObject.GetComponent<Rigidbody>().useGravity = true;
					transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
					transform.gameObject.GetComponent<BoxCollider>().enabled = true;
				}
			}
			yield break;
		}
		yield break;
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x0006A931 File Offset: 0x00068B31
	private IEnumerator DestroyWheel(GameObject destWheel)
	{
		foreach (object obj in destWheel.transform)
		{
			((Transform)obj).gameObject.GetComponent<Renderer>().enabled = false;
		}
		yield return new WaitForSeconds(0.2f);
		Object.Destroy(destWheel);
		yield break;
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x0006A940 File Offset: 0x00068B40
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer != 2 && this.isAllowedToTrigger)
		{
			this.colliderName = base.transform.name;
			this.trigName = other.gameObject.name;
			this.otherObj = other.gameObject.transform;
			if (this.attachTo != null && this.trigName == this.attachTo && base.transform.parent == this.theDest)
			{
				this.validColliderName = base.transform.name;
				this.validTrigName = other.gameObject.name;
				this.validTrigObject = other.gameObject;
				if (this.otherObj.parent != null)
				{
					this.validParentName = this.otherObj.parent.name;
				}
				if (this.otherObj.GetComponent<Renderer>() == null)
				{
					this.canSnap = true;
				}
				else if (!this.otherObj.GetComponent<Renderer>().enabled)
				{
					this.canSnap = true;
				}
				if (this.validTrigName == "waterwheel_e")
				{
					this.canSnap = true;
				}
				if (this.validTrigName == "BrakeDisk")
				{
					foreach (object obj in other.transform)
					{
						Transform transform = (Transform)obj;
						if (transform.name == "WHEEL_HOLDER" && !transform.GetChild(1).gameObject.activeSelf && !transform.GetChild(2).gameObject.activeSelf && !transform.GetChild(3).gameObject.activeSelf && !transform.GetChild(4).gameObject.activeSelf && !transform.GetChild(5).gameObject.activeSelf)
						{
							this.canSnap = true;
						}
					}
				}
				if (this.validTrigName == "jackmount")
				{
					this.HiJackX = this.otherObj;
				}
				if (this.otherObj.name != "ebtemplate2" && this.otherObj.name != "ebtemplatei6" && this.otherObj.name != "ebtemplate2v8" && this.otherObj.name != "250EngMount" && this.otherObj.name != "v8_trans_e" && this.otherObj.name != "v8_hosethermostat_e" && this.otherObj.name != "v8_distributor_e" && this.otherObj.name != "transferCase_e" && this.otherObj.name != "transmission_e" && this.otherObj.name != "battery_e" && this.otherObj.name != "keg" && this.otherObj.name != "battery_e" && this.otherObj.name != "revengecarinlet" && this.otherObj.name != "winch3_e" && this.otherObj.name != "HiJack_e" && this.otherObj.name.Substring(0, 3) != "jac" && this.otherObj.name.Substring(0, 3) != "tru" && this.otherObj.name.Substring(0, 3) != "Gas" && this.otherObj.name.Substring(0, 3) != "Dif" && this.otherObj.name.Substring(0, 3) != "dif" && this.otherObj.name.Substring(0, 4) != "toba" && this.otherObj.name.Substring(0, 4) != "brus" && this.otherObj.name != "jerrycanmount_e" && this.otherObj.name != "sparemount_e" && this.otherObj.name != "jerrycanmountF_e" && this.otherObj.name != "sparemountF_e" && this.otherObj.name != "opener_e" && this.otherObj.name != "infuser_e" && this.otherObj.name != "oldengine_e" && this.otherObj.tag != "ignoreattach")
				{
					Debug.Log(this.otherObj.tag);
					if (!this.otherObj.parent.GetComponent<Renderer>().enabled)
					{
						this.canSnap = false;
					}
				}
				if (this.canSnap)
				{
					this.reticleController.ShowSnapIcon(true);
				}
			}
			if (this.attachTo2 != null && this.trigName == this.attachTo2 && base.transform.parent == this.theDest)
			{
				this.validColliderName = base.transform.name;
				this.validTrigName = other.gameObject.name;
				this.validTrigObject = other.gameObject;
				this.canSnap = true;
				this.reticleController.ShowSnapIcon(true);
			}
			if (this.attachTo3 != null && this.trigName == this.attachTo3 && base.transform.parent == this.theDest)
			{
				this.validColliderName = base.transform.name;
				this.validTrigName = other.gameObject.name;
				this.validTrigObject = other.gameObject;
				this.canSnap = true;
				this.reticleController.ShowSnapIcon(true);
			}
			if ((this.attachTo == "" && this.attachTo2 == "") || (this.attachTo == "jerrymounted_e" && this.attachTo2 == "jerrymountedF_e"))
			{
				if (this.colliderName.Contains("MotorOil"))
				{
					if (this.trigName == "oilinput")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 1;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
					else if (this.trigName == "careng")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 8;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
					else if (this.trigName == "oilinputv8")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 14;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
					else if (this.trigName == "250oilinput")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 16;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
					else if (this.trigName == "i6oilinput")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 24;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
				}
				if (this.colliderName.Contains("TransFluid") && this.trigName == "250transinput")
				{
					base.GetComponent<FluidHandler>().enabled = true;
					base.GetComponent<FluidHandler>().fluidType = 15;
					base.GetComponent<FluidHandler>().addingFluid = true;
				}
				if (this.colliderName.Contains("twostroke") && this.trigName == "fuelInletChainsaw")
				{
					base.GetComponent<FluidHandler>().enabled = true;
					base.GetComponent<FluidHandler>().fluidType = 13;
					base.GetComponent<FluidHandler>().addingFluid = true;
				}
				if (this.colliderName == "jerrycan2")
				{
					if (this.trigName == "gastankinput")
					{
						Debug.Log("x1");
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 11;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
					else if (this.trigName == "FuelInletCar")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 12;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
					else if (this.trigName == "250FuelInlet")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 18;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
					else if (this.trigName == "FuelInletF")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 21;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
					else if (this.trigName == "FuelInletRWG")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 23;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
				}
				if (this.colliderName.Contains("additive"))
				{
					if (this.trigName == "gastankinput")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 25;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
					else if (this.trigName == "FuelInletCar")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 28;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
					else if (this.trigName == "250FuelInlet")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 27;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
					else if (this.trigName == "FuelInletF")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 26;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
				}
				if (this.colliderName.Contains("coolant"))
				{
					if (this.trigName == "coolantinput")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 7;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
					else if (this.trigName == "250CoolantInlet")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 19;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
					else if (this.trigName == "coolantinputF")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 22;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
				}
				if (this.colliderName == "FuelSage")
				{
					if (this.trigName == "gastankinput")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 2;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
					else if (this.trigName == "FuelInletCar")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 3;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
					else if (this.trigName == "jerrycaninlet")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 9;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
					else if (this.trigName == "250FuelInlet")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 17;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
					else if (this.trigName == "FuelInletF")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().fluidType = 20;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
				}
				if (this.colliderName == "AirNozzle")
				{
					if (this.trigName == "tireFL")
					{
						base.GetComponent<FluidHandler>().enabled = true;
						base.GetComponent<FluidHandler>().addingFluid = true;
					}
					else
					{
						base.GetComponent<FluidHandler>().enabled = false;
						base.GetComponent<FluidHandler>().addingFluid = false;
					}
				}
				if (this.colliderName == "Chainsaw")
				{
					if (this.trigName == "stump")
					{
						base.GetComponent<chainsaw>().tree = other.gameObject;
						if (other.gameObject.GetComponent<treeinfo>().health > 0)
						{
							base.GetComponent<chainsaw>().cutting = true;
							return;
						}
						base.GetComponent<chainsaw>().cutting = false;
						return;
					}
					else if (this.trigName == "trunk1" || this.trigName == "trunk2" || this.trigName == "trunk3")
					{
						base.GetComponent<chainsaw>().tree = other.gameObject;
						if (other.gameObject != null)
						{
							base.GetComponent<chainsaw>().cutting = true;
							return;
						}
						base.GetComponent<chainsaw>().cutting = false;
						return;
					}
					else
					{
						base.GetComponent<chainsaw>().cutting = false;
					}
				}
			}
		}
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x0006B744 File Offset: 0x00069944
	private void OnTriggerExit(Collider other)
	{
		if (this.isAllowedToTrigger)
		{
			if (other.gameObject.name == this.attachTo)
			{
				this.canSnap = false;
				this.reticleController.ShowSnapIcon(false);
			}
			if ((this.attachTo == "" && this.attachTo2 == "") || (this.attachTo == "jerrymounted_e" && this.attachTo == "jerrymountedF_e"))
			{
				if (this.trigName == "gastankinput")
				{
					base.GetComponent<FluidHandler>().addingFluid = false;
				}
				if (this.colliderName.Contains("MotorOil") && (this.trigName == "oilinput" || this.trigName == "careng"))
				{
					base.GetComponent<FluidHandler>().addingFluid = false;
					base.GetComponent<FluidHandler>().enabled = false;
				}
				if (this.colliderName.Contains("coolant") && this.trigName == "coolantinput")
				{
					base.GetComponent<FluidHandler>().addingFluid = false;
					base.GetComponent<FluidHandler>().enabled = false;
				}
			}
		}
	}

	// Token: 0x0400126E RID: 4718
	private Transform theDest;

	// Token: 0x0400126F RID: 4719
	private GameObject truck;

	// Token: 0x04001270 RID: 4720
	private GameObject truck2;

	// Token: 0x04001271 RID: 4721
	private GameObject amc;

	// Token: 0x04001272 RID: 4722
	private GameObject enblock;

	// Token: 0x04001273 RID: 4723
	private GameObject enblockv8;

	// Token: 0x04001274 RID: 4724
	private GameObject enblock250;

	// Token: 0x04001275 RID: 4725
	private GameObject enblocki6;

	// Token: 0x04001276 RID: 4726
	public Transform FPS;

	// Token: 0x04001277 RID: 4727
	private Transform t;

	// Token: 0x04001278 RID: 4728
	private Transform otherObj;

	// Token: 0x04001279 RID: 4729
	public float fixedRotation = 5f;

	// Token: 0x0400127A RID: 4730
	private bool canSnap;

	// Token: 0x0400127B RID: 4731
	private string colliderName;

	// Token: 0x0400127C RID: 4732
	private string trigName;

	// Token: 0x0400127D RID: 4733
	private string validColliderName;

	// Token: 0x0400127E RID: 4734
	private string validTrigName;

	// Token: 0x0400127F RID: 4735
	private GameObject validTrigObject;

	// Token: 0x04001280 RID: 4736
	public string attachTo;

	// Token: 0x04001281 RID: 4737
	public string attachTo2;

	// Token: 0x04001282 RID: 4738
	public string attachTo3;

	// Token: 0x04001283 RID: 4739
	public bool pickable = true;

	// Token: 0x04001284 RID: 4740
	public float price;

	// Token: 0x04001285 RID: 4741
	public float tradein;

	// Token: 0x04001286 RID: 4742
	public string description;

	// Token: 0x04001287 RID: 4743
	public bool shoulderCarry;

	// Token: 0x04001288 RID: 4744
	private Currency script;

	// Token: 0x04001289 RID: 4745
	public float numBolts;

	// Token: 0x0400128A RID: 4746
	public float thisDurability;

	// Token: 0x0400128B RID: 4747
	public GameObject template;

	// Token: 0x0400128C RID: 4748
	public bool canDetach;

	// Token: 0x0400128D RID: 4749
	private bool inTrigger;

	// Token: 0x0400128E RID: 4750
	private GameObject oilBolt;

	// Token: 0x0400128F RID: 4751
	public GameObject coolantBolt;

	// Token: 0x04001290 RID: 4752
	public bool holding;

	// Token: 0x04001291 RID: 4753
	private FixedJoint m_HoldJoint;

	// Token: 0x04001292 RID: 4754
	private FixedJoint nullJoint;

	// Token: 0x04001293 RID: 4755
	private FixedJoint nullJoint2;

	// Token: 0x04001294 RID: 4756
	public FirstPersonController controller;

	// Token: 0x04001295 RID: 4757
	public bool interactWhileHolding;

	// Token: 0x04001296 RID: 4758
	private GameObject tireFL;

	// Token: 0x04001297 RID: 4759
	private GameObject tireRL;

	// Token: 0x04001298 RID: 4760
	private GameObject tireRR;

	// Token: 0x04001299 RID: 4761
	private GameObject tireFR;

	// Token: 0x0400129A RID: 4762
	private GameObject tireX;

	// Token: 0x0400129B RID: 4763
	private GameObject tireX1;

	// Token: 0x0400129C RID: 4764
	private GameObject tireX2;

	// Token: 0x0400129D RID: 4765
	private GameObject tireX3;

	// Token: 0x0400129E RID: 4766
	private GameObject tireX4;

	// Token: 0x0400129F RID: 4767
	private GameObject wheelColX;

	// Token: 0x040012A0 RID: 4768
	private Transform HiJackX;

	// Token: 0x040012A1 RID: 4769
	private GameObject tireFL1;

	// Token: 0x040012A2 RID: 4770
	private GameObject tireFR1;

	// Token: 0x040012A3 RID: 4771
	private GameObject tireRL1;

	// Token: 0x040012A4 RID: 4772
	private GameObject tireRR1;

	// Token: 0x040012A5 RID: 4773
	private GameObject wheelColFL;

	// Token: 0x040012A6 RID: 4774
	private GameObject wheelColFR;

	// Token: 0x040012A7 RID: 4775
	private GameObject wheelColRL;

	// Token: 0x040012A8 RID: 4776
	private GameObject wheelColRR;

	// Token: 0x040012A9 RID: 4777
	private float newRadius;

	// Token: 0x040012AA RID: 4778
	public float deflation;

	// Token: 0x040012AB RID: 4779
	public float deflateOffset;

	// Token: 0x040012AC RID: 4780
	private string validParentName;

	// Token: 0x040012AD RID: 4781
	private ReticleController reticleController;

	// Token: 0x040012AE RID: 4782
	private GameObject vt;

	// Token: 0x040012AF RID: 4783
	private GameObject vc;

	// Token: 0x040012B0 RID: 4784
	private float newRust;

	// Token: 0x040012B1 RID: 4785
	private int numMaterials;

	// Token: 0x040012B2 RID: 4786
	private float thisDurabilityInstan;

	// Token: 0x040012B3 RID: 4787
	public float trueMass;

	// Token: 0x040012B4 RID: 4788
	private GameObject keg;

	// Token: 0x040012B5 RID: 4789
	public int partType;

	// Token: 0x040012B6 RID: 4790
	private int tireNum;

	// Token: 0x040012B7 RID: 4791
	private int rimNum;

	// Token: 0x040012B8 RID: 4792
	private int i;

	// Token: 0x040012B9 RID: 4793
	public bool isAllowedToTrigger;

	// Token: 0x040012BA RID: 4794
	public Material mat;

	// Token: 0x040012BB RID: 4795
	public Material mat2;

	// Token: 0x040012BC RID: 4796
	public bool noMat;

	// Token: 0x040012BD RID: 4797
	private Transform theDest2;

	// Token: 0x040012BE RID: 4798
	private Transform theDest1;

	// Token: 0x040012BF RID: 4799
	private GameObject tempchild;

	// Token: 0x040012C0 RID: 4800
	private GameObject missionObj;

	// Token: 0x040012C1 RID: 4801
	public bool painted;

	// Token: 0x040012C2 RID: 4802
	public float red;

	// Token: 0x040012C3 RID: 4803
	public float green;

	// Token: 0x040012C4 RID: 4804
	public float blue;

	// Token: 0x040012C5 RID: 4805
	public float metallic;

	// Token: 0x040012C6 RID: 4806
	public float smoothness;

	// Token: 0x040012C7 RID: 4807
	public int paintSlot;
}
