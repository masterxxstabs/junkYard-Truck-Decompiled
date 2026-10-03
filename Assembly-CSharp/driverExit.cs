using System;
using UnityEngine;

// Token: 0x0200016E RID: 366
public class driverExit : MonoBehaviour
{
	// Token: 0x060008FD RID: 2301 RVA: 0x00075869 File Offset: 0x00073A69
	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "terrain")
		{
			this.exitBlocked = true;
		}
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x00075889 File Offset: 0x00073A89
	private void OnTriggerExit(Collider other)
	{
		if (other.transform.tag == "terrain")
		{
			this.exitBlocked = false;
		}
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x000758A9 File Offset: 0x00073AA9
	public bool IsOnDriverSide()
	{
		return Vector3.Dot(base.transform.right, Vector3.up) > this.angleThreshold;
	}

	// Token: 0x04001569 RID: 5481
	public bool exitBlocked;

	// Token: 0x0400156A RID: 5482
	private float angleThreshold = 0.65f;
}
