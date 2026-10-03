using System;
using UnityEngine;

// Token: 0x020000E8 RID: 232
public class Lightbar : MonoBehaviour
{
	// Token: 0x060005CA RID: 1482 RVA: 0x00047634 File Offset: 0x00045834
	private void Update()
	{
		float f = Time.time * this.frequency * 3.1415927f * 2f;
		this.lf1.brightness = (Mathf.Sin(f) + 1f) / 2f * this.maxBrightness;
		this.lf2.brightness = (Mathf.Cos(f) + 1f) / 2f * this.maxBrightness;
		if (this.lf1.brightness > this.maxBrightness / 2f)
		{
			this.light1.enabled = true;
			this.light2.enabled = false;
			return;
		}
		this.light1.enabled = false;
		this.light2.enabled = true;
	}

	// Token: 0x04000C8C RID: 3212
	public LensFlare lf1;

	// Token: 0x04000C8D RID: 3213
	public LensFlare lf2;

	// Token: 0x04000C8E RID: 3214
	public Light light1;

	// Token: 0x04000C8F RID: 3215
	public Light light2;

	// Token: 0x04000C90 RID: 3216
	public float frequency = 1f;

	// Token: 0x04000C91 RID: 3217
	public float maxBrightness = 0.5f;
}
