using System;
using UnityEngine;

// Token: 0x0200010C RID: 268
public class PassExit : MonoBehaviour
{
	// Token: 0x060006FF RID: 1791 RVA: 0x0005A9AB File Offset: 0x00058BAB
	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "terrain")
		{
			this.exitBlocked = true;
		}
	}

	// Token: 0x06000700 RID: 1792 RVA: 0x0005A9CB File Offset: 0x00058BCB
	private void OnTriggerExit(Collider other)
	{
		if (other.transform.tag == "terrain")
		{
			this.exitBlocked = false;
		}
	}

	// Token: 0x04000FD0 RID: 4048
	public bool exitBlocked;
}
