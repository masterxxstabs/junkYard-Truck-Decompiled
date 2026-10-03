using System;
using UnityEngine;

// Token: 0x02000013 RID: 19
public class BatteryCharger : MonoBehaviour
{
	// Token: 0x06000045 RID: 69 RVA: 0x00004550 File Offset: 0x00002750
	private void Start()
	{
		if (this.battery.GetComponent<Renderer>().enabled)
		{
			this.battery18.SetActive(false);
			this.charging18 = false;
		}
		if (this.battery18.GetComponent<Renderer>().enabled)
		{
			this.battery.SetActive(false);
			this.charging = false;
		}
		if (!this.battery.GetComponent<Renderer>().enabled)
		{
			this.charging = false;
		}
		if (!this.battery18.GetComponent<Renderer>().enabled)
		{
			this.charging18 = false;
		}
	}

	// Token: 0x06000046 RID: 70 RVA: 0x000045DC File Offset: 0x000027DC
	public void ChargeBattery()
	{
		this.looseClip1.SetActive(false);
		this.looseClip2.SetActive(false);
		this.charging = true;
		this.charging18 = false;
		this.activeClip1.SetActive(true);
		this.activeClip2.SetActive(true);
		this.battery.GetComponent<Renderer>().enabled = true;
		this.battery.GetComponent<BoxCollider>().enabled = true;
		this.battery18.SetActive(false);
		Debug.Log("bat0");
	}

	// Token: 0x06000047 RID: 71 RVA: 0x00004660 File Offset: 0x00002860
	public void ChargeBattery18()
	{
		this.looseClip1.SetActive(false);
		this.looseClip2.SetActive(false);
		this.charging18 = true;
		this.charging = false;
		this.activeClip1.SetActive(true);
		this.activeClip2.SetActive(true);
		this.battery18.GetComponent<Renderer>().enabled = true;
		this.battery18.GetComponent<BoxCollider>().enabled = true;
		this.battery.SetActive(false);
		Debug.Log("bat18");
	}

	// Token: 0x06000048 RID: 72 RVA: 0x000046E4 File Offset: 0x000028E4
	public void DisconnectBattery()
	{
		Debug.Log("dc");
		this.looseClip1.SetActive(true);
		this.looseClip2.SetActive(true);
		this.charging18 = false;
		this.charging = false;
		this.activeClip1.SetActive(false);
		this.activeClip2.SetActive(false);
		this.emissive2.SetActive(false);
		this.emissive3.SetActive(false);
		this.battery18.SetActive(true);
		this.battery.SetActive(true);
	}

	// Token: 0x06000049 RID: 73 RVA: 0x0000476C File Offset: 0x0000296C
	public void DisconnectBattery18()
	{
		Debug.Log("dc18");
		this.looseClip1.SetActive(true);
		this.looseClip2.SetActive(true);
		this.charging18 = false;
		this.charging = false;
		this.activeClip1.SetActive(false);
		this.activeClip2.SetActive(false);
		this.emissive2.SetActive(false);
		this.emissive3.SetActive(false);
		this.battery.SetActive(true);
		this.battery18.SetActive(true);
	}

	// Token: 0x0600004A RID: 74 RVA: 0x000047F4 File Offset: 0x000029F4
	private void FixedUpdate()
	{
		if (this.charging && Time.time >= (float)this.nextUpdate)
		{
			this.nextUpdate = Mathf.FloorToInt(Time.time) + 5;
			if (this.batterydura.health < 100f)
			{
				this.batterydura.health += 1f;
				this.emissive2.SetActive(true);
				this.emissive3.SetActive(false);
			}
			else
			{
				this.emissive2.SetActive(false);
				this.emissive3.SetActive(true);
			}
		}
		if (this.charging18 && Time.time >= (float)this.nextUpdate)
		{
			this.nextUpdate = Mathf.FloorToInt(Time.time) + 5;
			if (this.batterydura18.health < 100f)
			{
				this.batterydura18.health += 1f;
				this.emissive2.SetActive(true);
				this.emissive3.SetActive(false);
				return;
			}
			this.emissive2.SetActive(false);
			this.emissive3.SetActive(true);
		}
	}

	// Token: 0x040000C2 RID: 194
	public GameObject looseClip1;

	// Token: 0x040000C3 RID: 195
	public GameObject looseClip2;

	// Token: 0x040000C4 RID: 196
	public GameObject activeClip1;

	// Token: 0x040000C5 RID: 197
	public GameObject activeClip2;

	// Token: 0x040000C6 RID: 198
	public durability batterydura;

	// Token: 0x040000C7 RID: 199
	public GameObject battery;

	// Token: 0x040000C8 RID: 200
	public durability batterydura18;

	// Token: 0x040000C9 RID: 201
	public GameObject battery18;

	// Token: 0x040000CA RID: 202
	public GameObject emissive2;

	// Token: 0x040000CB RID: 203
	public GameObject emissive3;

	// Token: 0x040000CC RID: 204
	public bool charging;

	// Token: 0x040000CD RID: 205
	public bool charging18;

	// Token: 0x040000CE RID: 206
	private int nextUpdate;
}
