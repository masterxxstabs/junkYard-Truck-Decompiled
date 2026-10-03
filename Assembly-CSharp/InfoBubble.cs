using System;
using UnityEngine;

// Token: 0x02000143 RID: 323
public class InfoBubble : MonoBehaviour
{
	// Token: 0x0600084C RID: 2124 RVA: 0x0006D794 File Offset: 0x0006B994
	private void Start()
	{
		this.startOffsetTarget = base.transform.position - this.TrackTarget.position;
	}

	// Token: 0x0600084D RID: 2125 RVA: 0x0006D7B8 File Offset: 0x0006B9B8
	private void Update()
	{
		Vector3 eulers = Mathf.Sin(this.WobbleFrequency * Time.timeSinceLevelLoad) * this.WobbleAxis * this.WobbleAmplitude;
		base.transform.Rotate(eulers);
		base.transform.position = this.TrackTarget.position + this.startOffsetTarget;
	}

	// Token: 0x04001349 RID: 4937
	public Vector3 WobbleAxis = Vector3.one;

	// Token: 0x0400134A RID: 4938
	public float WobbleFrequency = 1f;

	// Token: 0x0400134B RID: 4939
	public float WobbleAmplitude = 0.25f;

	// Token: 0x0400134C RID: 4940
	public Transform TrackTarget;

	// Token: 0x0400134D RID: 4941
	private Vector3 startOffsetTarget;
}
