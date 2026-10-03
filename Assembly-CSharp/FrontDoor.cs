using System;
using UnityEngine;

// Token: 0x020000B3 RID: 179
public class FrontDoor : MonoBehaviour
{
	// Token: 0x0600044B RID: 1099 RVA: 0x0002EB8A File Offset: 0x0002CD8A
	private void Start()
	{
		base.GetComponent<InteractiveObject>().isOpen = false;
	}
}
