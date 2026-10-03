using System;
using UnityEngine;

// Token: 0x02000100 RID: 256
[ExecuteInEditMode]
public class SimpleDayNight : MonoBehaviour
{
	// Token: 0x06000686 RID: 1670 RVA: 0x0004E081 File Offset: 0x0004C281
	private void Start()
	{
		this._lightEulerAngles = this.lightSource.transform.eulerAngles;
	}

	// Token: 0x06000687 RID: 1671 RVA: 0x0004E09C File Offset: 0x0004C29C
	private void Update()
	{
		if (this._prevTimeOfDay != this.timeOfDay)
		{
			this._lightEulerAngles.x = this.timeOfDay * 180f - 90f;
			this._lightEulerAngles.y = this.sunYRotation;
			this.lightSource.transform.eulerAngles = this._lightEulerAngles;
			float num = this.lightIntensityCurve.Evaluate(this.timeOfDay);
			this.lightSource.intensity = num * this.maxLightIntensity;
			float num2 = this.ambientIntensityCurve.Evaluate(this.timeOfDay);
			this.skysphereMat.SetFloat("_Exposure", num2 * this.maxSkysphereExposure);
			RenderSettings.ambientIntensity = this.ambientIntensityCurve.Evaluate(num2);
			this.reflectionProbe.RenderProbe();
		}
		this._prevTimeOfDay = this.timeOfDay;
	}

	// Token: 0x04000DAE RID: 3502
	[Range(0f, 1f)]
	public float timeOfDay = 0.5f;

	// Token: 0x04000DAF RID: 3503
	public Light lightSource;

	// Token: 0x04000DB0 RID: 3504
	public float maxLightIntensity = 1.1f;

	// Token: 0x04000DB1 RID: 3505
	public Material skysphereMat;

	// Token: 0x04000DB2 RID: 3506
	public float maxSkysphereExposure = 0.5f;

	// Token: 0x04000DB3 RID: 3507
	public AnimationCurve lightIntensityCurve;

	// Token: 0x04000DB4 RID: 3508
	public AnimationCurve ambientIntensityCurve;

	// Token: 0x04000DB5 RID: 3509
	public ReflectionProbe reflectionProbe;

	// Token: 0x04000DB6 RID: 3510
	public float sunYRotation;

	// Token: 0x04000DB7 RID: 3511
	private Vector3 _lightEulerAngles;

	// Token: 0x04000DB8 RID: 3512
	private float _prevTimeOfDay;
}
