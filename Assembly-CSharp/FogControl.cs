using System;
using UnityEngine;

// Token: 0x02000035 RID: 53
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(UnderWaterFog))]
[ExecuteInEditMode]
public class FogControl : MonoBehaviour
{
	// Token: 0x060000E9 RID: 233 RVA: 0x0000BE2D File Offset: 0x0000A02D
	private void OnEnable()
	{
		this.init();
	}

	// Token: 0x060000EA RID: 234 RVA: 0x0000BE2D File Offset: 0x0000A02D
	private void Start()
	{
		this.init();
	}

	// Token: 0x060000EB RID: 235 RVA: 0x0000BE38 File Offset: 0x0000A038
	private void Update()
	{
		this.Rate += Time.deltaTime / this.FadeSpeed;
		this.Rate = Mathf.Clamp(this.Rate, 0f, this.FadeSpeed);
		if (this.cam.transform.position.y <= this.fog.height)
		{
			if (!this.fog.enabled)
			{
				this.fog.enabled = true;
			}
			this.fog.fogColor.a = Mathf.Lerp(this.fog.fogColor.a, 1f, this.Rate);
			return;
		}
		this.fog.fogColor.a = Mathf.Lerp(this.fog.fogColor.a, 0f, this.Rate * 2f);
		if (this.fog.fogColor.a <= 0.01f)
		{
			this.fog.enabled = false;
		}
	}

	// Token: 0x060000EC RID: 236 RVA: 0x0000BF40 File Offset: 0x0000A140
	private void init()
	{
		if (this.cam == null)
		{
			this.cam = base.GetComponent<Camera>();
		}
		if (this.fog == null)
		{
			this.fog = base.GetComponent<UnderWaterFog>();
		}
		if (this.cam.transform.position.y >= this.fog.height)
		{
			this.fog.fogColor.a = 0f;
		}
	}

	// Token: 0x04000284 RID: 644
	public float FadeSpeed = 10f;

	// Token: 0x04000285 RID: 645
	private float Rate = 1f;

	// Token: 0x04000286 RID: 646
	private UnderWaterFog fog;

	// Token: 0x04000287 RID: 647
	private Camera cam;
}
