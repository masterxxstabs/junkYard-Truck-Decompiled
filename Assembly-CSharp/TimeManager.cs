using System;
using UnityEngine;

// Token: 0x0200014B RID: 331
public class TimeManager : MonoBehaviour
{
	// Token: 0x06000863 RID: 2147 RVA: 0x0006E2AC File Offset: 0x0006C4AC
	private void Update()
	{
		Time.timeScale += 1f / this.slowdownLength * Time.unscaledDeltaTime;
		Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
		if (Time.timeScale == 1f)
		{
			Time.fixedDeltaTime = Time.deltaTime;
			this.jumpCam2.enabled = false;
			this.cam1.enabled = true;
		}
	}

	// Token: 0x06000864 RID: 2148 RVA: 0x0006E31D File Offset: 0x0006C51D
	public void DoSlowmotion()
	{
		this.cam1.enabled = false;
		this.jumpCam2.enabled = true;
		Time.timeScale = this.slowdownFactor;
		Time.fixedDeltaTime = Time.timeScale * 0.2f;
	}

	// Token: 0x0400137F RID: 4991
	public float slowdownFactor = 0.05f;

	// Token: 0x04001380 RID: 4992
	public float slowdownLength = 4f;

	// Token: 0x04001381 RID: 4993
	public Camera cam1;

	// Token: 0x04001382 RID: 4994
	public Camera jumpCam2;
}
