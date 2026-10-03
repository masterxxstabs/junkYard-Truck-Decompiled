using System;
using UnityEngine;

// Token: 0x02000114 RID: 276
public class PlantHealth : MonoBehaviour
{
	// Token: 0x06000744 RID: 1860 RVA: 0x0005E900 File Offset: 0x0005CB00
	public void Hydrate()
	{
		if (this.hydration < 100)
		{
			this.hydration += 3;
			this.io.description = string.Concat(new object[]
			{
				"Growth[",
				this.growthAmount,
				"%] Hydrated[",
				this.hydration,
				"%]"
			});
		}
	}

	// Token: 0x06000745 RID: 1861 RVA: 0x0005E970 File Offset: 0x0005CB70
	public void Harvest()
	{
		if (this.growthAmount >= 100)
		{
			Vector3 position = new Vector3(base.transform.position.x, base.transform.position.y + 0.5f, base.transform.position.z);
			this.io.description = "Growth[0%] Hydrated[0%]";
			this.growthAmount = 0;
			base.transform.GetChild(0).gameObject.SetActive(true);
			base.transform.GetChild(1).gameObject.SetActive(false);
			base.transform.GetChild(2).gameObject.SetActive(false);
			Object.Instantiate<GameObject>(this.leaf, position, base.transform.rotation);
		}
	}

	// Token: 0x04001044 RID: 4164
	public int hydration;

	// Token: 0x04001045 RID: 4165
	public int growthAmount;

	// Token: 0x04001046 RID: 4166
	public InteractiveObject io;

	// Token: 0x04001047 RID: 4167
	public GameObject leaf;
}
