using System;
using System.Collections;
using Steamworks.Data;
using UnityEngine;

// Token: 0x020000CF RID: 207
public class InteractiveObject : MonoBehaviour
{
	// Token: 0x060004CE RID: 1230 RVA: 0x00031338 File Offset: 0x0002F538
	private void Start()
	{
		this.iTweenArgs = iTween.Hash(Array.Empty<object>());
		this.iTweenArgs.Add("position", this.openPosition);
		this.iTweenArgs.Add("time", this.animationTime);
		this.iTweenArgs.Add("islocal", true);
		AudioSource[] components = base.GetComponents<AudioSource>();
		if (components.Length != 0)
		{
			this.source = components[0];
			this.clip1 = components[0].clip;
			if (components.Length > 1)
			{
				this.clip2 = components[1].clip;
			}
			this.aSource = base.GetComponent<AudioSource>();
		}
		if (base.gameObject.name.Contains("pickaxe(Clone"))
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x00031405 File Offset: 0x0002F605
	public void ResetAction()
	{
		base.StartCoroutine(this.ResetAction2());
	}

	// Token: 0x060004D0 RID: 1232 RVA: 0x00031414 File Offset: 0x0002F614
	public IEnumerator ResetAction2()
	{
		yield return new WaitForSeconds(0.6f);
		this.PerformAction();
		yield break;
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x00031424 File Offset: 0x0002F624
	public void PerformAction()
	{
		if (this.spawnItem == null && this.spawnPrice > 0f)
		{
			if (base.gameObject.name.Contains("shop_"))
			{
				this.eventSystem = GameObject.FindWithTag("GameController");
				if (this.eventSystem.GetComponent<Currency>().money >= this.spawnPrice)
				{
					this.eventSystem.GetComponent<ModWoman>().BuyItemD(base.gameObject.name, this.spawnPrice, base.gameObject);
				}
				else
				{
					this.eventSystem.GetComponent<ModWoman>().NoMoney();
				}
			}
			else if (base.gameObject.name.Contains("shop2_"))
			{
				this.eventSystem = GameObject.Find("EventSystem2");
				if (this.eventSystem.GetComponent<Currency>().money >= this.spawnPrice)
				{
					this.eventSystem.GetComponent<ModWoman>().BuyItemF(base.gameObject.name, this.spawnPrice, base.gameObject);
				}
				else
				{
					this.eventSystem.GetComponent<ModWoman>().NoMoney();
				}
			}
		}
		if (this.spawnItem != null)
		{
			this.eventSystem = GameObject.Find("EventSystem2");
			if (this.eventSystem.GetComponent<Currency>().money >= this.spawnPrice)
			{
				this.fps = GameObject.Find("FPSController");
				this.fps.GetComponent<Interactor>().inv.SubtractMoney(this.spawnPrice);
				float num = this.eventSystem.GetComponent<Currency>().money - this.spawnPrice;
				this.eventSystem.GetComponent<Currency>().money = Mathf.Round(num * 100f) / 100f;
				GameObject gameObject = Object.Instantiate<GameObject>(this.spawnItem, base.transform.position, base.transform.rotation);
				gameObject.name = this.spawnItem.name;
				this.pu = gameObject.GetComponent<PickUp>();
				this.pu.thisDurability = 100f;
				if (this.spawnItem.name == "12pack")
				{
					this.pu.thisDurability = 12f;
				}
				if (this.spawnItem.name.Substring(0, 3) == "Gas")
				{
					this.pu.thisDurability = 2000f;
				}
				if (this.spawnItem.name == "tobaccocrate")
				{
					this.pu.thisDurability = 0f;
				}
				if (this.spawnItem.name == "truckwheel")
				{
					string name = base.name;
					uint num2 = <PrivateImplementationDetails>.ComputeStringHash(name);
					if (num2 <= 1606802320U)
					{
						if (num2 <= 734494906U)
						{
							if (num2 <= 661055026U)
							{
								if (num2 != 370024135U)
								{
									if (num2 == 661055026U)
									{
										if (name == "rim_stock")
										{
											this.j = 1;
										}
									}
								}
								else if (name == "tire_trailgrabber")
								{
									this.j = 18;
								}
							}
							else if (num2 != 717453973U)
							{
								if (num2 == 734494906U)
								{
									if (name == "tire_stock")
									{
										this.j = 6;
									}
								}
							}
							else if (name == "tire_mtz")
							{
								this.j = 14;
							}
						}
						else if (num2 <= 851249421U)
						{
							if (num2 != 784394447U)
							{
								if (num2 == 851249421U)
								{
									if (name == "rim_slotted")
									{
										this.j = 2;
									}
								}
							}
							else if (name == "tire_tsl")
							{
								this.j = 19;
							}
						}
						else if (num2 != 900632118U)
						{
							if (num2 != 1423528439U)
							{
								if (num2 == 1606802320U)
								{
									if (name == "tire_km")
									{
										this.j = 11;
									}
								}
							}
							else if (name == "tire_sx")
							{
								this.j = 17;
							}
						}
						else if (name == "tire_bogger")
						{
							this.j = 9;
						}
					}
					else if (num2 <= 2112220488U)
					{
						if (num2 <= 1749266854U)
						{
							if (num2 != 1735967129U)
							{
								if (num2 == 1749266854U)
								{
									if (name == "tire_rocker")
									{
										this.j = 15;
									}
								}
							}
							else if (name == "tire_km3")
							{
								this.j = 12;
							}
						}
						else if (num2 != 2055543905U)
						{
							if (num2 == 2112220488U)
							{
								if (name == "tire_baja")
								{
									this.j = 8;
								}
							}
						}
						else if (name == "tire_irok")
						{
							this.j = 10;
						}
					}
					else if (num2 <= 2702118948U)
					{
						if (num2 != 2152538607U)
						{
							if (num2 == 2702118948U)
							{
								if (name == "tire_krawler")
								{
									this.j = 13;
								}
							}
						}
						else if (name == "tire_swamper")
						{
							this.j = 16;
						}
					}
					else if (num2 != 2835414998U)
					{
						if (num2 != 3125233311U)
						{
							if (num2 == 4136630322U)
							{
								if (name == "beadlock")
								{
									this.j = 3;
								}
							}
						}
						else if (name == "beadlockdouble")
						{
							this.j = 5;
						}
					}
					else if (name == "beadlockslot")
					{
						this.j = 4;
					}
					if (this.j < 6)
					{
						gameObject.GetComponent<TireAssign>().rimNumX = this.j;
					}
					else
					{
						gameObject.GetComponent<TireAssign>().tireNumX = this.j;
					}
					gameObject.GetComponent<TireAssign>().Start();
				}
				base.gameObject.SetActive(false);
				if (this.spawnItem.name == "v8_am_turboD")
				{
					Debug.Log("ach");
					Achievement achievement = new Achievement("ACH_TWIN");
					achievement.Trigger(true);
				}
			}
		}
		if (this.noAnim && this.soundOverride)
		{
			this.source.PlayOneShot(this.clip1);
		}
		if (!this.noAnim)
		{
			this.colliderName = base.name;
			string text = this.colliderName;
			if (this.isOpen)
			{
				if (this.aSource)
				{
					this.source.PlayOneShot(this.clip1);
				}
				this.iTweenArgs["position"] = this.closedPosition;
				this.iTweenArgs["rotation"] = this.closedPosition;
			}
			else
			{
				if (this.aSource && this.clip2 != null)
				{
					this.source.PlayOneShot(this.clip2);
				}
				this.iTweenArgs["position"] = this.openPosition;
				this.iTweenArgs["rotation"] = this.openPosition;
			}
			this.isOpen = !this.isOpen;
			InteractiveObject.MovementType movementType = this.movementType;
			if (movementType != InteractiveObject.MovementType.Slide)
			{
				if (movementType == InteractiveObject.MovementType.Rotate)
				{
					iTween.RotateTo(base.gameObject, this.iTweenArgs);
				}
			}
			else
			{
				iTween.MoveTo(base.gameObject, this.iTweenArgs);
			}
			if (base.gameObject.name == "keg_lid")
			{
				if (!this.isOpen)
				{
					this.bucketAnim.SetActive(false);
				}
				else
				{
					this.bucketAnim.SetActive(true);
				}
			}
			if (base.gameObject.name == "RampObj")
			{
				if (this.isOpen)
				{
					this.iTweenArgs["rotation"] = new Vector3(-19f, 0f, 0f);
					iTween.RotateTo(base.gameObject, this.iTweenArgs);
					return;
				}
				this.iTweenArgs["rotation"] = new Vector3(0f, 0f, 0f);
				iTween.RotateTo(base.gameObject, this.iTweenArgs);
			}
		}
	}

	// Token: 0x04000994 RID: 2452
	[SerializeField]
	public Vector3 openPosition;

	// Token: 0x04000995 RID: 2453
	[SerializeField]
	public Vector3 closedPosition;

	// Token: 0x04000996 RID: 2454
	[SerializeField]
	private float animationTime;

	// Token: 0x04000997 RID: 2455
	[SerializeField]
	public bool isOpen;

	// Token: 0x04000998 RID: 2456
	[SerializeField]
	private InteractiveObject.MovementType movementType;

	// Token: 0x04000999 RID: 2457
	private Hashtable iTweenArgs;

	// Token: 0x0400099A RID: 2458
	private string colliderName;

	// Token: 0x0400099B RID: 2459
	private AudioSource aSource;

	// Token: 0x0400099C RID: 2460
	private AudioSource source;

	// Token: 0x0400099D RID: 2461
	private AudioClip clip1;

	// Token: 0x0400099E RID: 2462
	private AudioClip clip2;

	// Token: 0x0400099F RID: 2463
	public string description;

	// Token: 0x040009A0 RID: 2464
	public GameObject bucketAnim;

	// Token: 0x040009A1 RID: 2465
	public GameObject spawnItem;

	// Token: 0x040009A2 RID: 2466
	public float spawnPrice;

	// Token: 0x040009A3 RID: 2467
	private GameObject eventSystem;

	// Token: 0x040009A4 RID: 2468
	private GameObject fps;

	// Token: 0x040009A5 RID: 2469
	public bool noAnim;

	// Token: 0x040009A6 RID: 2470
	private PickUp pu;

	// Token: 0x040009A7 RID: 2471
	private int j;

	// Token: 0x040009A8 RID: 2472
	public bool soundOverride;

	// Token: 0x020003B5 RID: 949
	private enum MovementType
	{
		// Token: 0x040027BA RID: 10170
		Slide,
		// Token: 0x040027BB RID: 10171
		Rotate
	}
}
