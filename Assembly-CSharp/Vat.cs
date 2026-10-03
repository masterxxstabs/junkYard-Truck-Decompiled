using System;
using UnityEngine;

// Token: 0x0200015A RID: 346
public class Vat : MonoBehaviour
{
	// Token: 0x0600089D RID: 2205 RVA: 0x0006FA30 File Offset: 0x0006DC30
	private void Start()
	{
		if (this.quality <= 0f)
		{
			this.quality = 0.01f;
		}
		if (this.quality > 1000f)
		{
			this.quality = this.stillScript.quality;
		}
		if (this.moonshine <= 0f)
		{
			this.moonshine = 0.01f;
		}
		this.UpdateVisual();
	}

	// Token: 0x0600089E RID: 2206 RVA: 0x0006FA94 File Offset: 0x0006DC94
	private void OnTriggerEnter(Collider col)
	{
		if (col.name.Contains("mooncrateE") && this.moonshine > 10f)
		{
			Object.Destroy(col.gameObject);
			float num = this.blackberryPercent / this.moonshine;
			float num2 = this.limePercent / this.moonshine;
			float num3 = this.orangePercent / this.moonshine;
			float num4 = this.ambrosiaPercent / this.moonshine;
			this.moonshine -= 10f;
			if (num > 0.7f)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.moonCrateFullB, this.spawnPoint.transform.position, base.transform.rotation);
				gameObject.name = "mooncrateB";
				this.tradePrice = 60f * this.quality;
				if (this.tradePrice > 390f)
				{
					this.tradePrice = 390f;
				}
				float num5 = this.tradePrice;
				if (float.IsNaN(this.tradePrice))
				{
					this.tradePrice = 200f;
				}
				gameObject.GetComponent<PickUp>().price = this.tradePrice;
			}
			else if (num3 > 0.7f)
			{
				GameObject gameObject2 = Object.Instantiate<GameObject>(this.moonCrateFullO, this.spawnPoint.transform.position, base.transform.rotation);
				gameObject2.name = "mooncrateO";
				this.tradePrice = 70f * this.quality;
				if (this.tradePrice > 440f)
				{
					this.tradePrice = 440f;
				}
				float num6 = this.tradePrice;
				if (float.IsNaN(this.tradePrice))
				{
					this.tradePrice = 200f;
				}
				gameObject2.GetComponent<PickUp>().price = this.tradePrice;
			}
			else if (num2 > 0.7f)
			{
				GameObject gameObject3 = Object.Instantiate<GameObject>(this.moonCrateFullL, this.spawnPoint.transform.position, base.transform.rotation);
				gameObject3.name = "mooncrateL";
				this.tradePrice = 80f * this.quality;
				if (this.tradePrice > 490f)
				{
					this.tradePrice = 490f;
				}
				float num7 = this.tradePrice;
				if (float.IsNaN(this.tradePrice))
				{
					this.tradePrice = 200f;
				}
				gameObject3.GetComponent<PickUp>().price = this.tradePrice;
			}
			else if (num4 > 0.7f)
			{
				GameObject gameObject4 = Object.Instantiate<GameObject>(this.moonCrateFullA, this.spawnPoint.transform.position, base.transform.rotation);
				gameObject4.name = "mooncrateA";
				this.tradePrice = 90f * this.quality;
				if (this.tradePrice > 550f)
				{
					this.tradePrice = 550f;
				}
				float num8 = this.tradePrice;
				if (float.IsNaN(this.tradePrice))
				{
					this.tradePrice = 200f;
				}
				gameObject4.GetComponent<PickUp>().price = this.tradePrice;
			}
			else
			{
				GameObject gameObject5 = Object.Instantiate<GameObject>(this.moonCrateFull, this.spawnPoint.transform.position, base.transform.rotation);
				gameObject5.name = "mooncrateF";
				this.tradePrice = 50f * this.quality;
				if (this.tradePrice > 300f)
				{
					this.tradePrice = 300f;
				}
				float num9 = this.tradePrice;
				if (float.IsNaN(this.tradePrice))
				{
					this.tradePrice = 190f;
				}
				gameObject5.GetComponent<PickUp>().price = this.tradePrice;
			}
			this.UpdateVisual();
			int num10 = Random.Range(0, 3);
			this.aSource.clip = this.clip[num10];
			this.aSource.Play();
			this.blackberryPercent -= 10f;
			if (this.blackberryPercent < 0f)
			{
				this.blackberryPercent = 0f;
			}
			this.orangePercent -= 10f;
			if (this.orangePercent < 0f)
			{
				this.orangePercent = 0f;
			}
			this.limePercent -= 10f;
			if (this.limePercent < 0f)
			{
				this.limePercent = 0f;
			}
			this.ambrosiaPercent -= 10f;
			if (this.ambrosiaPercent < 0f)
			{
				this.ambrosiaPercent = 0f;
			}
		}
	}

	// Token: 0x0600089F RID: 2207 RVA: 0x0006FEE0 File Offset: 0x0006E0E0
	public void UpdateVisual()
	{
		this.vatLevel = this.moonshine * 0.0025f;
		if (this.vatLevel >= 0.14f)
		{
			this.vatLevel = 0.14f;
		}
		this.vatFluid.transform.localPosition = new Vector3(this.vatFluid.transform.localPosition.x, this.vatLevel, this.vatFluid.transform.localPosition.z);
		float num = this.blackberryPercent / this.moonshine * 100f;
		float num2 = this.limePercent / this.moonshine * 100f;
		float num3 = this.orangePercent / this.moonshine * 100f;
		float num4 = this.ambrosiaPercent / this.moonshine * 100f;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0f;
		float num8 = 0f;
		float num9 = 0f;
		float num10 = 0f;
		float num11 = 0f;
		if (num > 20f)
		{
			num7 = -0.001f * num;
		}
		if (num2 > 20f)
		{
			num8 = 0.001f * num2;
		}
		if (num4 > 20f)
		{
			num5 = 0.002f * num4;
		}
		if (num3 > 20f)
		{
			num5 = 0.02f * num3;
			num9 = 0.005f * num3;
			num10 = -0.005f * num3;
		}
		float r = num5 + num6 + 0.1f;
		float g = num7 + num8 + num9 + 0.1f;
		float b = num10 + num11 + 0.1f;
		this.moonshineMaterial.material.color = new Color(r, g, b, 0.7f);
	}

	// Token: 0x040013E3 RID: 5091
	public float moonshine;

	// Token: 0x040013E4 RID: 5092
	public float quality;

	// Token: 0x040013E5 RID: 5093
	public GameObject moonCrateFull;

	// Token: 0x040013E6 RID: 5094
	public GameObject moonCrateFullB;

	// Token: 0x040013E7 RID: 5095
	public GameObject moonCrateFullO;

	// Token: 0x040013E8 RID: 5096
	public GameObject moonCrateFullL;

	// Token: 0x040013E9 RID: 5097
	public GameObject moonCrateFullA;

	// Token: 0x040013EA RID: 5098
	public GameObject fluidObj;

	// Token: 0x040013EB RID: 5099
	public Still stillScript;

	// Token: 0x040013EC RID: 5100
	public GameObject spawnPoint;

	// Token: 0x040013ED RID: 5101
	private float tradePrice;

	// Token: 0x040013EE RID: 5102
	public GameObject vatFluid;

	// Token: 0x040013EF RID: 5103
	private float vatLevel;

	// Token: 0x040013F0 RID: 5104
	public float blackberryPercent;

	// Token: 0x040013F1 RID: 5105
	public float limePercent;

	// Token: 0x040013F2 RID: 5106
	public float orangePercent;

	// Token: 0x040013F3 RID: 5107
	public float ambrosiaPercent;

	// Token: 0x040013F4 RID: 5108
	public AudioSource aSource;

	// Token: 0x040013F5 RID: 5109
	public AudioClip[] clip;

	// Token: 0x040013F6 RID: 5110
	public Renderer moonshineMaterial;
}
