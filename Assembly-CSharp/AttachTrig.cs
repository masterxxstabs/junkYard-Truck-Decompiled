using System;
using UnityEngine;

// Token: 0x0200000F RID: 15
public class AttachTrig : MonoBehaviour
{
	// Token: 0x06000033 RID: 51 RVA: 0x000035D0 File Offset: 0x000017D0
	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("Obj Entered");
	}
}
