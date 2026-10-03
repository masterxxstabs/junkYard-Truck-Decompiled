using System;
using UnityEngine;

// Token: 0x02000163 RID: 355
public class Waterwheel : MonoBehaviour
{
	// Token: 0x060008C3 RID: 2243 RVA: 0x00070EB9 File Offset: 0x0006F0B9
	private void Start()
	{
		if (this.wheel.transform.parent.gameObject.activeSelf)
		{
			this.canRotate = true;
		}
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x00070EE0 File Offset: 0x0006F0E0
	private void FixedUpdate()
	{
		if (this.canRotate)
		{
			float num = this.rpm * 360f / 60f;
			this.wheel.Rotate(0f, num * Time.deltaTime, 0f, Space.Self);
			this.pulley.Rotate(0f, num * 2f * Time.deltaTime, 0f, Space.Self);
			if (this.batteryR.enabled)
			{
				this.timer += Time.deltaTime;
				if (this.timer >= 10f)
				{
					this.timer = 0f;
					if (this.batteryDur.health < 100f)
					{
						this.batteryDur.health += 1f;
					}
				}
			}
		}
	}

	// Token: 0x04001421 RID: 5153
	public Transform wheel;

	// Token: 0x04001422 RID: 5154
	public Transform pulley;

	// Token: 0x04001423 RID: 5155
	public float rpm = 30f;

	// Token: 0x04001424 RID: 5156
	public Renderer batteryR;

	// Token: 0x04001425 RID: 5157
	public durability batteryDur;

	// Token: 0x04001426 RID: 5158
	private float timer;

	// Token: 0x04001427 RID: 5159
	public bool canRotate;
}
