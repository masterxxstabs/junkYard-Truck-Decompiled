using System;
using UnityEngine;

// Token: 0x02000042 RID: 66
public class CrateAssign : MonoBehaviour
{
	// Token: 0x0600013E RID: 318 RVA: 0x0000EDB4 File Offset: 0x0000CFB4
	private void Start()
	{
		float thisDurability = base.GetComponent<PickUp>().thisDurability;
		if (thisDurability >= 6f)
		{
			base.transform.GetChild(2).gameObject.SetActive(true);
			return;
		}
		if (thisDurability > 2f)
		{
			base.transform.GetChild(1).gameObject.SetActive(true);
			return;
		}
		if (thisDurability > 0f)
		{
			base.transform.GetChild(0).gameObject.SetActive(true);
		}
	}
}
