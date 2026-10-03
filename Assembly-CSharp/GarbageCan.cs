using System;
using UnityEngine;

// Token: 0x020000B4 RID: 180
public class GarbageCan : MonoBehaviour
{
	// Token: 0x0600044D RID: 1101 RVA: 0x0002EB98 File Offset: 0x0002CD98
	private void Start()
	{
		base.GetComponents<AudioSource>();
		if (this.numItems > 0)
		{
			this.bagVisual.SetActive(true);
		}
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x0002EBB8 File Offset: 0x0002CDB8
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.GetComponent<PickUp>() != null && other.gameObject.GetComponent<PickUp>().tradein > 0f && !other.gameObject.name.Contains("trashbag"))
		{
			this.bagVisual.SetActive(true);
			this.numItems++;
			Object.Destroy(other.gameObject);
			int num = Random.Range(0, 4);
			this.aSources[num].Play();
			if (this.numItems == 10)
			{
				this.bagVisual.SetActive(false);
				this.numItems = 0;
				this.newBag = Object.Instantiate<GameObject>(this.bagTemplate, this.bagLoc.transform.position, this.bagLoc.transform.rotation);
			}
		}
	}

	// Token: 0x040008E8 RID: 2280
	private int numItems;

	// Token: 0x040008E9 RID: 2281
	public GameObject bagTemplate;

	// Token: 0x040008EA RID: 2282
	public Transform bagLoc;

	// Token: 0x040008EB RID: 2283
	private GameObject newBag;

	// Token: 0x040008EC RID: 2284
	public GameObject bagVisual;

	// Token: 0x040008ED RID: 2285
	public AudioSource[] aSources;
}
