using System;
using UnityEngine;

// Token: 0x0200004F RID: 79
public class DoorChime : MonoBehaviour
{
	// Token: 0x06000183 RID: 387 RVA: 0x00010FB8 File Offset: 0x0000F1B8
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			this.inStore = !this.inStore;
			if ((float)this.curr.playMinutes > this.lastGreet && this.inStore)
			{
				this.woman.Greet();
			}
		}
	}

	// Token: 0x0400046C RID: 1132
	public ModWoman woman;

	// Token: 0x0400046D RID: 1133
	public Currency curr;

	// Token: 0x0400046E RID: 1134
	public bool inStore;

	// Token: 0x0400046F RID: 1135
	public float lastGreet;
}
