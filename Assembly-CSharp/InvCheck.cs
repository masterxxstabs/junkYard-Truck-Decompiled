using System;
using UnityEngine;

// Token: 0x020000D1 RID: 209
public class InvCheck : MonoBehaviour
{
	// Token: 0x06000540 RID: 1344 RVA: 0x0003FBF4 File Offset: 0x0003DDF4
	private void Start()
	{
		foreach (object obj in base.gameObject.transform)
		{
			Transform transform = (Transform)obj;
			if (transform.gameObject.activeSelf)
			{
				transform.gameObject.SetActive(false);
			}
		}
	}
}
