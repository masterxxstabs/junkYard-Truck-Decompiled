using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000DD RID: 221
public class Lift : MonoBehaviour
{
	// Token: 0x06000592 RID: 1426 RVA: 0x000459F8 File Offset: 0x00043BF8
	private void Start()
	{
		this.DownPosition = base.transform.position;
		this.UpPosition = base.transform.position + new Vector3(0f, this.LiftHeight, 0f);
	}

	// Token: 0x06000593 RID: 1427 RVA: 0x00045A36 File Offset: 0x00043C36
	private IEnumerator Moving(Vector3 From, Vector3 To)
	{
		this.Lifting = true;
		for (float i = 0f; i < 1f; i += 0.01f)
		{
			base.transform.position = Vector3.Lerp(From, To, i);
			yield return null;
		}
		this.Lifting = false;
		yield break;
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x00045A53 File Offset: 0x00043C53
	public void LiftUp()
	{
		if (this.Lifting || this.IsUp)
		{
			return;
		}
		base.StartCoroutine(this.Moving(this.DownPosition, this.UpPosition));
		this.IsUp = true;
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x00045A86 File Offset: 0x00043C86
	public void LiftDown()
	{
		if (this.Lifting || !this.IsUp)
		{
			return;
		}
		base.StartCoroutine(this.Moving(this.UpPosition, this.DownPosition));
		this.IsUp = false;
	}

	// Token: 0x04000C04 RID: 3076
	private Vector3 DefaultPosition;

	// Token: 0x04000C05 RID: 3077
	public float LiftHeight;

	// Token: 0x04000C06 RID: 3078
	private bool Lifting;

	// Token: 0x04000C07 RID: 3079
	private bool IsUp;

	// Token: 0x04000C08 RID: 3080
	private Vector3 DownPosition;

	// Token: 0x04000C09 RID: 3081
	private Vector3 UpPosition;
}
