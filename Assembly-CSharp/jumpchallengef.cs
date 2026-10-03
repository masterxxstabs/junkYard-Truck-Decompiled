using System;
using UnityEngine;

// Token: 0x02000179 RID: 377
public class jumpchallengef : MonoBehaviour
{
	// Token: 0x06000944 RID: 2372 RVA: 0x0007DC88 File Offset: 0x0007BE88
	private void OnTriggerEnter(Collider other)
	{
		this.jumpduration = this.truck.GetComponent<car>().jumptime;
		Debug.Log(this.jumpduration);
	}

	// Token: 0x040018DC RID: 6364
	public GameObject truck;

	// Token: 0x040018DD RID: 6365
	public float jumpduration;
}
