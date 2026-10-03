using System;
using UnityEngine;

// Token: 0x02000014 RID: 20
public class Beercase : MonoBehaviour
{
	// Token: 0x0600004C RID: 76 RVA: 0x00004906 File Offset: 0x00002B06
	public void Start()
	{
		this.ReduceBeer();
	}

	// Token: 0x0600004D RID: 77 RVA: 0x00004910 File Offset: 0x00002B10
	private void ReduceBeer()
	{
		if (this.bottles < 12)
		{
			for (int i = this.bottles; i < 12; i++)
			{
				base.transform.GetChild(i).gameObject.SetActive(false);
			}
			if (this.bottles == 0)
			{
				base.GetComponent<ImpactSound>().enabled = false;
				base.GetComponent<AudioSource>().enabled = false;
			}
		}
	}

	// Token: 0x040000CF RID: 207
	public int bottles;
}
