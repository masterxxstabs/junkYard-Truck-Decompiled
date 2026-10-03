using System;
using UnityEngine;

// Token: 0x020000C5 RID: 197
public class PlayerControl : CarControl
{
	// Token: 0x06000497 RID: 1175 RVA: 0x0002FE5B File Offset: 0x0002E05B
	private void Update()
	{
		base.ControlCar(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
	}
}
