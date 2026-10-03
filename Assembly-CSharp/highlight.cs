using System;
using UnityEngine;

// Token: 0x02000174 RID: 372
public class highlight : MonoBehaviour
{
	// Token: 0x0600092D RID: 2349 RVA: 0x0007D19F File Offset: 0x0007B39F
	private void Start()
	{
		this._renderer = base.GetComponent<Renderer>();
	}

	// Token: 0x0600092E RID: 2350 RVA: 0x00002188 File Offset: 0x00000388
	private void Update()
	{
	}

	// Token: 0x04001896 RID: 6294
	private Renderer _renderer;
}
