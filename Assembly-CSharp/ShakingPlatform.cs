using System;
using UnityEngine;

// Token: 0x020000E2 RID: 226
public class ShakingPlatform : MonoBehaviour
{
	// Token: 0x060005AB RID: 1451 RVA: 0x0004606E File Offset: 0x0004426E
	private void Start()
	{
		this.DownPos = base.transform.position;
	}

	// Token: 0x060005AC RID: 1452 RVA: 0x00046084 File Offset: 0x00044284
	private void Update()
	{
		if (this.Shaking)
		{
			if (this.GoingUp)
			{
				this.t = Mathf.MoveTowards(this.t, 1f, Time.deltaTime * this.ShakingSpeed);
			}
			else
			{
				this.t = Mathf.MoveTowards(this.t, 0f, Time.deltaTime * this.ShakingSpeed);
			}
			if (this.t == 1f && this.GoingUp)
			{
				this.GoingUp = false;
			}
			if (this.t == 0f && !this.GoingUp)
			{
				this.GoingUp = true;
			}
		}
		else
		{
			this.t = Mathf.MoveTowards(this.t, 0f, Time.deltaTime * this.ShakingSpeed);
		}
		base.transform.position = Vector3.Lerp(this.DownPos, this.UpPos, this.t);
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x00046168 File Offset: 0x00044368
	public void StartShaking()
	{
		this.UpPos = this.DownPos + new Vector3(0f, this.ShakingHeight, 0f);
		this.Shaking = true;
		this.GoingUp = true;
		this.t = this.PhaseOffset;
	}

	// Token: 0x060005AE RID: 1454 RVA: 0x000461B5 File Offset: 0x000443B5
	public void StopShaking()
	{
		this.Shaking = false;
	}

	// Token: 0x04000C25 RID: 3109
	public float ShakingSpeed;

	// Token: 0x04000C26 RID: 3110
	public float ShakingHeight;

	// Token: 0x04000C27 RID: 3111
	public float PhaseOffset;

	// Token: 0x04000C28 RID: 3112
	private bool Shaking;

	// Token: 0x04000C29 RID: 3113
	private float t;

	// Token: 0x04000C2A RID: 3114
	private bool GoingUp;

	// Token: 0x04000C2B RID: 3115
	private Vector3 DownPos;

	// Token: 0x04000C2C RID: 3116
	private Vector3 UpPos;
}
