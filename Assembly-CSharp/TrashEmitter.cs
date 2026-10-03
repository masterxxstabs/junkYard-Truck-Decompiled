using System;
using UnityEngine;

// Token: 0x02000158 RID: 344
public class TrashEmitter : MonoBehaviour
{
	// Token: 0x06000895 RID: 2197 RVA: 0x0006F6CC File Offset: 0x0006D8CC
	public void CreateTrash()
	{
		if (base.transform.childCount < 43)
		{
			this.randLoc1 = Random.Range(1, 23);
			this.randLoc2 = Random.Range(1, 23);
			this.newBag = Object.Instantiate<GameObject>(this.trashBagTemplate, this.trashLoc[this.randLoc1].position, Quaternion.identity);
			this.newBag.transform.parent = base.gameObject.transform;
			this.newBag2 = Object.Instantiate<GameObject>(this.trashBagTemplate, this.trashLoc[this.randLoc2].position, Quaternion.identity);
			this.newBag2.transform.parent = base.gameObject.transform;
		}
	}

	// Token: 0x040013D7 RID: 5079
	public GameObject trashBagTemplate;

	// Token: 0x040013D8 RID: 5080
	public Transform[] trashLoc;

	// Token: 0x040013D9 RID: 5081
	private int randLoc1;

	// Token: 0x040013DA RID: 5082
	private int randLoc2;

	// Token: 0x040013DB RID: 5083
	private GameObject newBag;

	// Token: 0x040013DC RID: 5084
	private GameObject newBag2;
}
