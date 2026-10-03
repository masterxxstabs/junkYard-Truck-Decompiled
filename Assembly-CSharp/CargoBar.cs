using System;
using UnityEngine;

// Token: 0x0200003D RID: 61
public class CargoBar : MonoBehaviour
{
	// Token: 0x0600012B RID: 299 RVA: 0x0000E774 File Offset: 0x0000C974
	public void Start()
	{
		if (this.isOpen)
		{
			this.door.GetComponent<InteractiveObject>().enabled = true;
			this.openObjects.SetActive(true);
			this.closedObjects.SetActive(false);
			this.door.GetComponent<InteractiveObject>().enabled = true;
			this.ToggleSign(false);
			this.jakeOb.SetActive(false);
			this.bootleggerOb.SetActive(false);
			this.holdem.SetActive(true);
			return;
		}
		if (this.jake.barDialogue)
		{
			this.ToggleSign(true);
		}
	}

	// Token: 0x0600012C RID: 300 RVA: 0x0000E803 File Offset: 0x0000CA03
	public void ToggleSign(bool tf)
	{
		if (tf)
		{
			this.fsSign.SetActive(true);
			return;
		}
		this.fsSign.SetActive(false);
	}

	// Token: 0x0600012D RID: 301 RVA: 0x0000E821 File Offset: 0x0000CA21
	public void BuyContainer()
	{
		this.isOpen = true;
		this.ToggleSign(false);
		this.jake.barUnlocked = true;
		this.jakeOb.SetActive(false);
		this.bootleggerOb.SetActive(false);
	}

	// Token: 0x0400034C RID: 844
	public GameObject door;

	// Token: 0x0400034D RID: 845
	public GameObject openObjects;

	// Token: 0x0400034E RID: 846
	public GameObject closedObjects;

	// Token: 0x0400034F RID: 847
	public float money;

	// Token: 0x04000350 RID: 848
	public float moonshineSales;

	// Token: 0x04000351 RID: 849
	public float moonshineInventory;

	// Token: 0x04000352 RID: 850
	public float tobaccoSales;

	// Token: 0x04000353 RID: 851
	public float tobaccoInventory;

	// Token: 0x04000354 RID: 852
	public bool isOpen;

	// Token: 0x04000355 RID: 853
	public Jake jake;

	// Token: 0x04000356 RID: 854
	public GameObject jakeOb;

	// Token: 0x04000357 RID: 855
	public GameObject bootleggerOb;

	// Token: 0x04000358 RID: 856
	public GameObject fsSign;

	// Token: 0x04000359 RID: 857
	public GameObject holdem;
}
