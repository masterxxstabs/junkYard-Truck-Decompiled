using System;
using UnityEngine;

// Token: 0x0200014F RID: 335
public class GarageDoor : MonoBehaviour
{
	// Token: 0x0600086E RID: 2158 RVA: 0x0006E815 File Offset: 0x0006CA15
	private void Start()
	{
		this.doorAnim = base.GetComponent<Animation>();
	}

	// Token: 0x0400138A RID: 5002
	private Animation doorAnim;

	// Token: 0x0400138B RID: 5003
	private bool opened;

	// Token: 0x0400138C RID: 5004
	private bool allowOpen;
}
