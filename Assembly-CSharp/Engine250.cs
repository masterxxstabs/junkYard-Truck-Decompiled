using System;
using UnityEngine;

// Token: 0x0200005D RID: 93
public class Engine250 : MonoBehaviour
{
	// Token: 0x060001AE RID: 430 RVA: 0x00011B54 File Offset: 0x0000FD54
	private void Start()
	{
		if (base.transform.parent != null)
		{
			if (base.transform.parent.name == "DirtBike")
			{
				base.transform.position = this.ebtemplate250.position;
				base.transform.rotation = this.ebtemplate250.rotation;
				base.gameObject.GetComponent<FixedJoint>().connectedBody = this.bikeRb;
				base.gameObject.GetComponent<FixedJoint>().connectedMassScale = 0.1f;
				this.ShowBolts();
				return;
			}
		}
		else
		{
			base.GetComponent<Rigidbody>().isKinematic = true;
			this.HideBolts();
			base.gameObject.GetComponent<FixedJoint>().connectedBody = this.emptyRb250;
			base.GetComponent<Rigidbody>().isKinematic = false;
		}
	}

	// Token: 0x060001AF RID: 431 RVA: 0x00011C28 File Offset: 0x0000FE28
	public void Refresh()
	{
		this.carb_cnd = this.carb_cnd_c.health;
		this.clutch_cnd = this.clutch_cnd_c.health;
		this.cylinder_cnd = this.cylinder_cnd_c.health;
		this.gasket_cnd = this.gasket_cnd_c.health;
		this.head_cnd = this.head_cnd_c.health;
		this.oilFilter_cnd = this.oilFilter_cnd_c.health;
		this.piston_cnd = this.piston_cnd_c.health;
		this.torqueReduction = 0f;
		this.tempIncrease = 0f;
		this.canRun = true;
		if (this.oilFilter_cnd < 1f)
		{
			this.oilLoss = 1f;
		}
		if (this.valveCover_cnd < 1f)
		{
			this.oilLoss = 1f;
		}
		if (this.flyWheelCover_cnd < 1f || this.caseRight_cnd < 1f)
		{
			this.oilLoss = 100f;
			this.newOilLevel = 0f;
		}
		if (this.clutchCover_cnd < 1f)
		{
			this.transFluid = 0f;
		}
		if (this.carb_cnd == 0f || this.caseRight_cnd == 0f || this.chain_cnd == 0f || this.clutch_cnd == 0f || this.cylinder_cnd == 0f || this.flyWheel_cnd == 0f || this.head_cnd == 0f || this.piston_cnd == 0f || this.newFuelLevel < 1f)
		{
			this.canRun = false;
		}
		if (base.transform.parent.name == null)
		{
			this.canRun = false;
		}
		if (this.piston_cnd < 30f)
		{
			this.torqueReduction += 100f - this.piston_cnd;
		}
		if (this.cylinder_cnd < 30f)
		{
			this.torqueReduction += 100f - this.cylinder_cnd;
		}
		if (this.gasket_cnd < 30f)
		{
			this.torqueReduction += 100f - this.gasket_cnd;
		}
		if (this.newCoolantLevel < 50f)
		{
			this.tempIncrease += 50f;
		}
		if (this.newOilLevel < 50f)
		{
			this.tempIncrease += 40f;
		}
		if (this.gasket_cnd < 5f)
		{
			this.tempIncrease += 10f;
		}
		this.db.powerDivision = 1;
		if (this.torqueReduction > 150f)
		{
			this.db.powerDivision = 2;
		}
		if (this.torqueReduction > 200f)
		{
			this.db.powerDivision = 3;
		}
		if (this.torqueReduction > 250f)
		{
			this.db.powerDivision = 4;
		}
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x00011EFC File Offset: 0x000100FC
	public void DegradeEngine()
	{
		this.Refresh();
		float num = this.tempIncrease;
		this.randPart = Random.Range(0, 27);
		if (this.newOilLevel < 50f || this.tempIncrease > 40f)
		{
			this.randPart = Random.Range(0, 12);
		}
		if (this.transFluid < 50f)
		{
			this.clutch_cnd_c.health -= 1f;
		}
		switch (this.randPart)
		{
		case 0:
			this.gasket_cnd_c.health -= 2f;
			return;
		case 1:
			this.cylinder_cnd_c.health -= 2f;
			return;
		case 2:
			this.piston_cnd_c.health -= 2f;
			return;
		case 3:
			this.clutch_cnd_c.health -= 1f;
			return;
		case 4:
			this.newCoolantLevel -= 1f;
			return;
		case 5:
			this.transFluid -= 1f;
			return;
		case 6:
			this.newOilLevel -= 1f;
			return;
		default:
			return;
		}
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x00012038 File Offset: 0x00010238
	public void addPart(string newPart, bool connect)
	{
		if (connect)
		{
			this.thisHealth = GameObject.Find(newPart).GetComponent<durability>().health;
		}
		else
		{
			this.thisHealth = 0f;
		}
		uint num = <PrivateImplementationDetails>.ComputeStringHash(newPart);
		if (num <= 1746578320U)
		{
			if (num <= 1114019698U)
			{
				if (num != 320705677U)
				{
					if (num != 725289789U)
					{
						if (num != 1114019698U)
						{
							return;
						}
						if (!(newPart == "250_chain_e"))
						{
							return;
						}
						this.chain_cnd = this.thisHealth;
						return;
					}
					else
					{
						if (!(newPart == "250_caseright_e"))
						{
							return;
						}
						this.caseRight_cnd = this.thisHealth;
						return;
					}
				}
				else
				{
					if (!(newPart == "250_flywheel_e"))
					{
						return;
					}
					this.flyWheel_cnd = this.thisHealth;
					return;
				}
			}
			else if (num != 1247629396U)
			{
				if (num != 1622020224U)
				{
					if (num != 1746578320U)
					{
						return;
					}
					if (!(newPart == "250_clutch_e"))
					{
						return;
					}
					this.clutch_cnd = this.thisHealth;
					return;
				}
				else
				{
					if (!(newPart == "250_piston_e"))
					{
						return;
					}
					this.piston_cnd = this.thisHealth;
					return;
				}
			}
			else
			{
				if (!(newPart == "250_flywheelcover_e"))
				{
					return;
				}
				this.flyWheelCover_cnd = this.thisHealth;
				return;
			}
		}
		else if (num <= 2034973847U)
		{
			if (num != 1925188252U)
			{
				if (num != 2014858235U)
				{
					if (num != 2034973847U)
					{
						return;
					}
					if (!(newPart == "250_cylinder_e"))
					{
						return;
					}
					this.cylinder_cnd = this.thisHealth;
					return;
				}
				else
				{
					if (!(newPart == "250_head_e"))
					{
						return;
					}
					this.head_cnd = this.thisHealth;
					return;
				}
			}
			else
			{
				if (!(newPart == "250_valvecover_e"))
				{
					return;
				}
				this.valveCover_cnd = this.thisHealth;
				return;
			}
		}
		else if (num <= 3443165853U)
		{
			if (num != 3081808505U)
			{
				if (num != 3443165853U)
				{
					return;
				}
				if (!(newPart == "250_carb_e"))
				{
					return;
				}
				this.carb_cnd = this.thisHealth;
				return;
			}
			else
			{
				if (!(newPart == "250_oilfilter_e"))
				{
					return;
				}
				this.oilFilter_cnd = this.thisHealth;
				return;
			}
		}
		else if (num != 3488031443U)
		{
			if (num != 4135645094U)
			{
				return;
			}
			if (!(newPart == "250_gasket_e"))
			{
				return;
			}
			this.gasket_cnd = this.thisHealth;
			return;
		}
		else
		{
			if (!(newPart == "250_clutchcover_e"))
			{
				return;
			}
			this.clutchCover_cnd = this.thisHealth;
			return;
		}
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x000122A5 File Offset: 0x000104A5
	public void HideBolts()
	{
		this.mbolt1.SetActive(false);
		this.mbolt2.SetActive(false);
	}

	// Token: 0x060001B3 RID: 435 RVA: 0x000122BF File Offset: 0x000104BF
	public void ShowBolts()
	{
		this.mbolt1.SetActive(true);
		this.mbolt2.SetActive(true);
	}

	// Token: 0x040004A4 RID: 1188
	public GameObject bike;

	// Token: 0x040004A5 RID: 1189
	public GameObject carb;

	// Token: 0x040004A6 RID: 1190
	public GameObject caseRight;

	// Token: 0x040004A7 RID: 1191
	public GameObject chain;

	// Token: 0x040004A8 RID: 1192
	public GameObject clutch;

	// Token: 0x040004A9 RID: 1193
	public GameObject clutchCover;

	// Token: 0x040004AA RID: 1194
	public GameObject cylinder;

	// Token: 0x040004AB RID: 1195
	public GameObject flyWheel;

	// Token: 0x040004AC RID: 1196
	public GameObject flyWheelCover;

	// Token: 0x040004AD RID: 1197
	public GameObject gasket;

	// Token: 0x040004AE RID: 1198
	public GameObject head;

	// Token: 0x040004AF RID: 1199
	public GameObject oilFilter;

	// Token: 0x040004B0 RID: 1200
	public GameObject piston;

	// Token: 0x040004B1 RID: 1201
	public GameObject valveCover;

	// Token: 0x040004B2 RID: 1202
	public float carb_cnd;

	// Token: 0x040004B3 RID: 1203
	public float caseRight_cnd;

	// Token: 0x040004B4 RID: 1204
	public float chain_cnd;

	// Token: 0x040004B5 RID: 1205
	public float clutch_cnd;

	// Token: 0x040004B6 RID: 1206
	public float clutchCover_cnd;

	// Token: 0x040004B7 RID: 1207
	public float cylinder_cnd;

	// Token: 0x040004B8 RID: 1208
	public float flyWheel_cnd;

	// Token: 0x040004B9 RID: 1209
	public float flyWheelCover_cnd;

	// Token: 0x040004BA RID: 1210
	public float gasket_cnd;

	// Token: 0x040004BB RID: 1211
	public float head_cnd;

	// Token: 0x040004BC RID: 1212
	public float oilFilter_cnd;

	// Token: 0x040004BD RID: 1213
	public float piston_cnd;

	// Token: 0x040004BE RID: 1214
	public float valveCover_cnd;

	// Token: 0x040004BF RID: 1215
	public durability carb_cnd_c;

	// Token: 0x040004C0 RID: 1216
	public durability caseRight_cnd_c;

	// Token: 0x040004C1 RID: 1217
	public durability chain_cnd_c;

	// Token: 0x040004C2 RID: 1218
	public durability clutch_cnd_c;

	// Token: 0x040004C3 RID: 1219
	public durability clutchCover_cnd_c;

	// Token: 0x040004C4 RID: 1220
	public durability cylinder_cnd_c;

	// Token: 0x040004C5 RID: 1221
	public durability flWheel_cnd_c;

	// Token: 0x040004C6 RID: 1222
	public durability flWheelCover_cnd_c;

	// Token: 0x040004C7 RID: 1223
	public durability gasket_cnd_c;

	// Token: 0x040004C8 RID: 1224
	public durability head_cnd_c;

	// Token: 0x040004C9 RID: 1225
	public durability oilFilter_cnd_c;

	// Token: 0x040004CA RID: 1226
	public durability piston_cnd_c;

	// Token: 0x040004CB RID: 1227
	public durability valveCover_cnd_c;

	// Token: 0x040004CC RID: 1228
	public GameObject exhaustTrails;

	// Token: 0x040004CD RID: 1229
	public float newOilLevel;

	// Token: 0x040004CE RID: 1230
	public float newCoolantLevel;

	// Token: 0x040004CF RID: 1231
	public float transFluid;

	// Token: 0x040004D0 RID: 1232
	public float newFuelLevel;

	// Token: 0x040004D1 RID: 1233
	public float newTemperature;

	// Token: 0x040004D2 RID: 1234
	public float newMaxTorque;

	// Token: 0x040004D3 RID: 1235
	public float torqueReduction;

	// Token: 0x040004D4 RID: 1236
	public float torqueIncrease;

	// Token: 0x040004D5 RID: 1237
	public bool canRun = true;

	// Token: 0x040004D6 RID: 1238
	public bool bogging;

	// Token: 0x040004D7 RID: 1239
	public float oilLoss;

	// Token: 0x040004D8 RID: 1240
	public bool whiteSmoke;

	// Token: 0x040004D9 RID: 1241
	private int randPart;

	// Token: 0x040004DA RID: 1242
	private string newPart;

	// Token: 0x040004DB RID: 1243
	private float thisHealth;

	// Token: 0x040004DC RID: 1244
	public float tempIncrease;

	// Token: 0x040004DD RID: 1245
	public Transform ebtemplate250;

	// Token: 0x040004DE RID: 1246
	public Rigidbody emptyRb250;

	// Token: 0x040004DF RID: 1247
	public Rigidbody bikeRb;

	// Token: 0x040004E0 RID: 1248
	public GameObject mbolt1;

	// Token: 0x040004E1 RID: 1249
	public GameObject mbolt2;

	// Token: 0x040004E2 RID: 1250
	public Dirtbike db;

	// Token: 0x040004E3 RID: 1251
	public float additive;
}
