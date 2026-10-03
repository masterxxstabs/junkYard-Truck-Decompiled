using System;
using UnityEngine;

// Token: 0x02000121 RID: 289
public class RadarDetector : MonoBehaviour
{
	// Token: 0x060007A0 RID: 1952 RVA: 0x00062F3F File Offset: 0x0006113F
	private void Start()
	{
		this.alerted = false;
	}

	// Token: 0x060007A1 RID: 1953 RVA: 0x00062F48 File Offset: 0x00061148
	private void Update()
	{
		this.timer += Time.deltaTime;
		if (this.timer >= this.interval)
		{
			this.timer = 0f;
			float num = Vector3.Distance(base.transform.position, this.suv.position);
			if (this.officer.driving && num < this.triggerDistance && !this.alerted)
			{
				this.alerted = true;
				this.aSource.Play();
				this.renderer.material.EnableKeyword("_EMISSION");
			}
			if (this.alerted && num > 60f)
			{
				this.alerted = false;
				this.renderer.material.DisableKeyword("_EMISSION");
			}
		}
	}

	// Token: 0x04001169 RID: 4457
	public Officer officer;

	// Token: 0x0400116A RID: 4458
	public Transform suv;

	// Token: 0x0400116B RID: 4459
	public Renderer renderer;

	// Token: 0x0400116C RID: 4460
	private float timer;

	// Token: 0x0400116D RID: 4461
	private float interval = 1f;

	// Token: 0x0400116E RID: 4462
	public float triggerDistance = 40f;

	// Token: 0x0400116F RID: 4463
	public AudioSource aSource;

	// Token: 0x04001170 RID: 4464
	private bool alerted;
}
