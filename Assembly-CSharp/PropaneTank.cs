using System;
using UnityEngine;

// Token: 0x0200011D RID: 285
public class PropaneTank : MonoBehaviour
{
	// Token: 0x0600078C RID: 1932 RVA: 0x00062328 File Offset: 0x00060528
	private void Start()
	{
		float num = (float)Mathf.RoundToInt(base.gameObject.GetComponent<PickUp>().thisDurability / 2000f * 100f);
		base.gameObject.GetComponent<PickUp>().description = "Propane Tank(" + num + ")%";
	}
}
