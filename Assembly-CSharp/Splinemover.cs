using System;
using UnityEngine;

// Token: 0x0200013C RID: 316
public class Splinemover : MonoBehaviour
{
	// Token: 0x06000829 RID: 2089 RVA: 0x0006C99D File Offset: 0x0006AB9D
	private void Start()
	{
		this.thisTransform = base.transform;
	}

	// Token: 0x0600082A RID: 2090 RVA: 0x0006C9AB File Offset: 0x0006ABAB
	private void Update()
	{
		if (Time.time >= this.nextUpdate)
		{
			this.nextUpdate = Time.time + 0.5f;
			this.UpdateSec();
		}
	}

	// Token: 0x0600082B RID: 2091 RVA: 0x0006C9D1 File Offset: 0x0006ABD1
	private void UpdateSec()
	{
		this.thisTransform.position = this.spline.WhereOnSpline(this.followObj.position);
	}

	// Token: 0x0400130A RID: 4874
	public Spline spline;

	// Token: 0x0400130B RID: 4875
	public Transform followObj;

	// Token: 0x0400130C RID: 4876
	private Transform thisTransform;

	// Token: 0x0400130D RID: 4877
	private float nextUpdate;
}
