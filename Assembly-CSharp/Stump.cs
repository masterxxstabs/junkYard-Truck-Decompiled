using System;
using UnityEngine;

// Token: 0x02000141 RID: 321
public class Stump : MonoBehaviour
{
	// Token: 0x06000847 RID: 2119 RVA: 0x0006D67A File Offset: 0x0006B87A
	private void OnJointBreak(float breakForce)
	{
		Debug.Log(breakForce);
		this.aSource.Play();
		this.jc.PullStump(this.thisStump);
	}

	// Token: 0x04001344 RID: 4932
	public AudioSource aSource;

	// Token: 0x04001345 RID: 4933
	public jiggs jc;

	// Token: 0x04001346 RID: 4934
	public int thisStump;
}
