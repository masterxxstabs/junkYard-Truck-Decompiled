using System;
using UnityEngine;

// Token: 0x02000193 RID: 403
public class help : MonoBehaviour
{
	// Token: 0x060009E5 RID: 2533 RVA: 0x00002188 File Offset: 0x00000388
	private void Start()
	{
	}

	// Token: 0x060009E6 RID: 2534 RVA: 0x00088009 File Offset: 0x00086209
	private void FixedUpdate()
	{
		if (Input.GetKeyDown(KeyCode.H))
		{
			this.hide = !this.hide;
		}
	}

	// Token: 0x060009E7 RID: 2535 RVA: 0x00088023 File Offset: 0x00086223
	private void OnGUI()
	{
		if (!this.hide)
		{
			GUI.Box(new Rect(0f, 0f, 300f, 80f), "Press 'C' to switch cameras \nPress 'H' to hide help\nfor driving switch button 'controlled' \n on the car script ");
		}
	}

	// Token: 0x04001B70 RID: 7024
	private bool hide;
}
