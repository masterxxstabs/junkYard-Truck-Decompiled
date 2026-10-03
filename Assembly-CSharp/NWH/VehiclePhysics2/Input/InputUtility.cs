using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Input
{
	// Token: 0x020002C3 RID: 707
	public static class InputUtility
	{
		// Token: 0x06001327 RID: 4903 RVA: 0x000CA17C File Offset: 0x000C837C
		public static float GetMouseHorizontal()
		{
			float num = Mathf.Clamp(Input.mousePosition.x / (float)Screen.width, -1f, 1f);
			if (num < 0.5f)
			{
				return -(0.5f - num) * 2f;
			}
			return (num - 0.5f) * 2f;
		}

		// Token: 0x06001328 RID: 4904 RVA: 0x000CA1D0 File Offset: 0x000C83D0
		public static float GetMouseVertical()
		{
			float num = Mathf.Clamp(Input.mousePosition.y / (float)Screen.height, -1f, 1f);
			if (num < 0.5f)
			{
				return -(0.5f - num) * 2f;
			}
			return (num - 0.5f) * 2f;
		}
	}
}
