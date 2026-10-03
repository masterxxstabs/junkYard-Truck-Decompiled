using System;
using UnityEngine;

// Token: 0x02000152 RID: 338
public class TobaccoManager : MonoBehaviour
{
	// Token: 0x06000878 RID: 2168 RVA: 0x0006E8E4 File Offset: 0x0006CAE4
	public void UpdatePlants()
	{
		int num = 0;
		foreach (GameObject gameObject in this.plants)
		{
			if (this.ph[num].hydration > 0 && this.ph[num].growthAmount < 100)
			{
				this.ph[num].growthAmount += 3;
				this.ph[num].hydration -= 4;
				this.pi[num].description = string.Concat(new object[]
				{
					"Growth[",
					this.ph[num].growthAmount,
					"%] Hydrated[",
					this.ph[num].hydration,
					"%]"
				});
				if (this.ph[num].growthAmount > 49 && this.ph[num].growthAmount < 100)
				{
					gameObject.transform.GetChild(0).gameObject.SetActive(false);
					gameObject.transform.GetChild(1).gameObject.SetActive(true);
					gameObject.transform.GetChild(2).gameObject.SetActive(false);
				}
				else if (this.ph[num].growthAmount >= 100)
				{
					this.pi[num].description = "Harvest";
					gameObject.transform.GetChild(0).gameObject.SetActive(false);
					gameObject.transform.GetChild(1).gameObject.SetActive(false);
					gameObject.transform.GetChild(2).gameObject.SetActive(true);
				}
			}
			num++;
		}
	}

	// Token: 0x04001391 RID: 5009
	public GameObject[] plants;

	// Token: 0x04001392 RID: 5010
	public InteractiveObject[] pi;

	// Token: 0x04001393 RID: 5011
	public PlantHealth[] ph;
}
