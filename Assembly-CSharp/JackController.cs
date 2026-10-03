using System;
using UnityEngine;

// Token: 0x020000D4 RID: 212
public class JackController : MonoBehaviour
{
	// Token: 0x06000554 RID: 1364 RVA: 0x00042A84 File Offset: 0x00040C84
	private void FixedUpdate()
	{
		if (Input.GetKey(KeyCode.PageUp))
		{
			this.fork.transform.localPosition = Vector3.MoveTowards(this.fork.transform.localPosition, this.maxY, this.speedTranslate * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.PageDown))
		{
			this.fork.transform.localPosition = Vector3.MoveTowards(this.fork.transform.localPosition, this.minY, this.speedTranslate * Time.deltaTime);
		}
	}

	// Token: 0x04000B4B RID: 2891
	public Transform fork;

	// Token: 0x04000B4C RID: 2892
	public Transform mast;

	// Token: 0x04000B4D RID: 2893
	public float speedTranslate;

	// Token: 0x04000B4E RID: 2894
	public Vector3 maxY;

	// Token: 0x04000B4F RID: 2895
	public Vector3 minY;
}
